using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;

using PlurkApi;
using System.Reflection;

namespace Plurker
{
    class Program
    {
        private static Boolean blnRoot = false;
        static void Main(string[] args)
        {
            Console.WriteLine("Creating timer: {0}\n",
                                          DateTime.Now.ToString("h:mm:ss"));

            try
            {
                PlurkApi.PlurkApi mApi = new PlurkApi.PlurkApi(GetConfig.GetApiKey());
                mApi.login(GetConfig.GetUserName(), GetConfig.GetPassword());
                TimerCallback callback = new TimerCallback(Tick);
                // create a one second timer tick
                Timer stateTimer = new Timer(callback, mApi, 0, GetConfig.GetTimerInterval());
                
                // loop here forever
                for (; ; ) 
                {
                    try
                    {
                        if (!blnRoot) RootingJob(mApi);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[Error]" + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                //程式結束
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                if (ex.InnerException != null) Console.WriteLine(ex.InnerException.Message);
                Console.WriteLine("Press enter to terminate process...");
                Console.ReadLine();
            }
           
        }
        /// <summary>
        /// 批次處理
        /// </summary>
        /// <param name="mApi"></param>
        static public void RootingJob(PlurkApi.PlurkApi mApi)
        {
            blnRoot = true;
            StringBuilder sb = new StringBuilder();
            do
            {
                List<Thread> threadList = new List<Thread>();
                try
                {
                    DateTime now = DateTime.Now;
                    Console.WriteLine(string.Format("本批次開始....開始時間:{0}",now.ToString("MM-dd hh:mm:ss")));
                    plurks UnReadPlurk = PlurkModule.Common.GetUnReadPlurk(mApi);

                    #region thread listing

                    string strGuId = Guid.NewGuid().ToString();
                    //回應股價
                    if (GetConfig.BootStock())
                    {
                        Thread autoStockThread = new Thread(new ParameterizedThreadStart(PlurkModule.ThreadCallFunction.CallForStockQuery));
                        autoStockThread.Name = "Stock-" + strGuId;
                        threadList.Add(autoStockThread);
                    }
                    //自動搶五樓
                    if (GetConfig.BootFifth())
                    {
                        Thread autoGet5Thread = new Thread(new ParameterizedThreadStart(PlurkModule.ThreadCallFunction.CallForGetFifth));
                        autoGet5Thread.Name = "5-" + strGuId;
                        threadList.Add(autoGet5Thread);
                    }
                    // 出來面對
                    if (GetConfig.BootFaceIt())
                    {
                        //Thread autoFaceThread = new Thread(new ParameterizedThreadStart(PlurkModule.AutoFace));
                        //autoFaceThread.Name = "Face-" + strGuId  ;
                        //threadList.Add(autoFaceThread);
                    }
                    //外匯
                    if (GetConfig.AutoExrate())
                    {
                        Thread autoExchangeRate = new Thread(new ParameterizedThreadStart(PlurkModule.ThreadCallFunction.CallForExrate));
                        autoExchangeRate.Name = "exrate-" + strGuId;
                        threadList.Add(autoExchangeRate);
                    }
                    //美食
                    if (GetConfig.AutoFoodSuggest())
                    {
                        Thread autoFoodSuggest = new Thread(new ParameterizedThreadStart(PlurkModule.ThreadCallFunction.CallForFoodSuggest));
                        autoFoodSuggest.Name = "foodsuggest-" + strGuId;
                        threadList.Add(autoFoodSuggest);
                    }

                    #endregion
                    #region thread processing
                    foreach (Thread Thread in threadList)
                    {
                        string[] aryThreadName = Thread.Name.Split('-');

                        switch (aryThreadName[0].ToLower())
                        {
                            case "5":
                                PlurkModule.FifthParameter FifthParameter = new PlurkModule.FifthParameter();
                                FifthParameter.Api = mApi;
                                FifthParameter.TimerBootMSec = GetConfig.GetTimerInterval();
                                FifthParameter.UnReadPlurk = UnReadPlurk;
                                FifthParameter.XmlFilePath = GetConfig.GetfifthDataFile();
                                Thread.Start(FifthParameter);
                                break;
                            case "face":
                                break;
                            case "stock":
                                PlurkModule.StockerParameter StockerParameter = new PlurkModule.StockerParameter();
                                StockerParameter.Api = mApi;
                                StockerParameter.UnReadPlurk = UnReadPlurk;
                                StockerParameter.XmlFilePath = GetConfig.GetStockDataFile();
                                Thread.Start(StockerParameter);
                                break;
                            case "exrate":
                                PlurkModule.AutoExrateParameter AutoExrateParameter = new PlurkModule.AutoExrateParameter();
                                AutoExrateParameter.Api = mApi;
                                AutoExrateParameter.UnReadPlurk = UnReadPlurk;
                                Thread.Start(AutoExrateParameter);
                                break;
                            case "foodsuggest":
                                PlurkModule.AutoFoodSuggestParameter AutoFoodSuggestParameter = new PlurkModule.AutoFoodSuggestParameter();
                                AutoFoodSuggestParameter.Api = mApi;
                                AutoFoodSuggestParameter.UnReadPlurk = UnReadPlurk;
                                AutoFoodSuggestParameter.XmlFilePath = GetConfig.GetFoodDataFile();
                                Thread.Start(AutoFoodSuggestParameter);
                                break;
                            default:
                                break;
                        }
                        GC.Collect();
                    }
                    #endregion
                    #region thread terminating

                    bool blnAllThreadIsDead = false;
                    do
                    {
                        blnAllThreadIsDead = false;
                        int iThreadCount = 0;
                        foreach (Thread Thread in threadList)
                        {
                            if (Thread.IsAlive)
                            {
                                blnAllThreadIsDead = false;
                                break;
                            }
                            iThreadCount++;
                        }
                        if (iThreadCount == threadList.Count)
                            blnAllThreadIsDead = true;
                        if (blnAllThreadIsDead)
                        {
                            #region //將已讀的噗標示靜音
                            if (!GetConfig.DebugMode())
                            {
                                List<Int64> ListUnReadPlurkId = new List<Int64>();
                                List<Int64> ListNeedToMutePlurkId = new List<Int64>();
                                foreach (plurk plurk in UnReadPlurk)
                                {
                                    if (plurk.response_count > 5) ListNeedToMutePlurkId.Add(plurk.plurk_id);
                                    ListUnReadPlurkId.Add(plurk.plurk_id);
                                }
                                mApi.markAsRead(ListUnReadPlurkId);
                                //回應數大於5的就靜音,以節省資源
                                mApi.mutePlurks(ListNeedToMutePlurkId);
                            }
                            #endregion
                            Console.WriteLine(string.Format("本批次結束....結束時間:{0}", DateTime.Now.ToString("MM-dd hh:mm:ss")));
                        }
                    }
                    while (!blnAllThreadIsDead);
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine("error " + ex.Message);
                    sb.AppendLine(string.Format("{0}Error time:{1}{0}Reason:{2}", Environment.NewLine, DateTime.Now.ToString("MM-dd hh:mm:ss"), ex.Message));
                    blnRoot = false;
                    throw;
                }
                finally
                {
                    foreach (Thread thread in threadList)
                        if (thread.IsAlive) thread.Abort();
                }
            } while (blnRoot);

            if (!blnRoot)
            {
                string sfilePath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) +"\\errorlog.txt";
                if (!File.Exists(sfilePath)) File.CreateText(sfilePath);
                sb.AppendLine("**********************");
                File.AppendAllText(sfilePath, sb.ToString(), Encoding.UTF8);//Log記錄
            }
        }

        static public void Tick(Object stateInfo)
        {
            Console.WriteLine(string.Format("定時排程啟動....啟動時間:{0}", DateTime.Now.ToString("MM-dd hh:mm:ss")));
            PlurkApi.PlurkApi mApi = (PlurkApi.PlurkApi)stateInfo;
            #region thread listing
            List<Thread> threadList = new List<Thread>();
            string strGuId = Guid.NewGuid().ToString();
            plurks UnReadPlurk = PlurkModule.Common.GetUnReadPlurk(mApi);
            DateTime now = DateTime.Now;
            //時間到說話
            if (GetConfig.BootHello())
            {
                Thread autoHelloThread = new Thread(new ParameterizedThreadStart(PlurkModule.ThreadCallFunction.CallForSayHello));
                autoHelloThread.Name = "Hello-" + strGuId;
                threadList.Add(autoHelloThread);
            }
            //自動加入朋友要求
            if (GetConfig.BootAddFriend())
            {
                Thread autoAddFriendThread = new Thread(new ParameterizedThreadStart(PlurkModule.ThreadCallFunction.CallForAddFriend));
                autoAddFriendThread.Name = "Friend-" + strGuId;
                threadList.Add(autoAddFriendThread);
            }
            //自動教學
            if (GetConfig.BootTeach())
            {
                Thread autoTeachThread = new Thread(new ParameterizedThreadStart(PlurkModule.ThreadCallFunction.CallForTeach));
                autoTeachThread.Name = "Teach-" + strGuId;
                threadList.Add(autoTeachThread);
            }
            #endregion
            #region thread processing
            foreach (Thread Thread in threadList)
            {
                string[] aryThreadName = Thread.Name.Split('-');

                switch (aryThreadName[0].ToLower())
                {
                    case "friend":
                        PlurkModule.AddFriednParameter AddFriednParameter = new PlurkModule.AddFriednParameter();
                        AddFriednParameter.Api = mApi;
                        AddFriednParameter.UnReadPlurk = UnReadPlurk;
                        Thread.Start(AddFriednParameter);
                        break;
                    case "hello":
                        PlurkModule.SayHelloParameter SayHelloParameter = new PlurkModule.SayHelloParameter();
                        SayHelloParameter.Api = mApi;
                        SayHelloParameter.ExecDateTime = now;
                        SayHelloParameter.FoodXmlFilePath = GetConfig.GetFoodDataFile();
                        SayHelloParameter.UnReadPlurk = UnReadPlurk;
                        Thread.Start(SayHelloParameter);
                        break;
                    case "teach":
                        PlurkModule.TeachParameter TeachParameter = new PlurkModule.TeachParameter();
                        TeachParameter.Api = mApi;
                        TeachParameter.UnReadPlurk = UnReadPlurk;
                        Thread.Start(TeachParameter);
                        break;
                    default:
                        break;
                }
                GC.Collect();
            }
            #endregion
            #region thread terminating

            bool blnAllThreadIsDead = false;
            do
            {
                blnAllThreadIsDead = false;
                int iThreadCount = 0;
                foreach (Thread Thread in threadList)
                {
                    if (Thread.IsAlive)
                    {
                        blnAllThreadIsDead = false;
                        break;
                    }
                    iThreadCount++;
                }
                if (iThreadCount==threadList.Count)
                    blnAllThreadIsDead = true;
                //if (blnAllThreadIsDead)
                //{
                //    blnAllThreadIsDead = true;
                    #region //將已讀的噗標示起來
                //    /*
                //    if (!GetConfig.DebugMode())
                //    {
                //        List<Int64> ListUnReadPlurkId = new List<Int64>();
                //        List<Int64> ListNeedToMutePlurkId = new List<Int64>();
                //        foreach (plurk plurk in UnReadPlurk)
                //        {
                //            if (plurk.response_count > 5) ListNeedToMutePlurkId.Add(plurk.plurk_id);
                //            ListUnReadPlurkId.Add(plurk.plurk_id);
                //        }
                //        mApi.markAsRead(ListUnReadPlurkId);
                //        //回應數大於5的就靜音,以節省資源
                //        mApi.mutePlurks(ListNeedToMutePlurkId);
                //    }
                //     */
                    #endregion
                //    //Console.WriteLine(string.Format("定時排程結束....{0}結束時間:{1}", Environment.NewLine, DateTime.Now.ToString("MM-dd hh:mm:ss")));
                //}
            }
            while (!blnAllThreadIsDead);
            Console.WriteLine(string.Format("定時排程結束....結束時間:{0}",  DateTime.Now.ToString("MM-dd hh:mm:ss")));
            #endregion
        }
    }
}
