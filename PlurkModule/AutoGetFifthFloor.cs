using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Threading;

using PlurkApi;

namespace PlurkModule
{
    public class AutoGetFifthFloor:BasicObject
    {
        private int _TimerBootMSec = 0;
        private string _fifthXMLPath = "";
        private string[] aryResponse ;
        private string[] aryStrResponse;
        //每個執行緒能處理的Plurk數目
        private int iPlurkCountPerThread = PlurkModule.Properties.Settings.Default.PlurkModule_5Floor_ThreadCount;
        /// <summary>
        /// 自動搶五樓
        /// </summary>
        /// <param name="mApi">Plurk api object</param>
        /// <param name="TimerBootMSec">timer作動時間</param>
        public AutoGetFifthFloor(PlurkApi.PlurkApi mApi, int TimerBootMSec, plurks UnreadPlurk,string fifthXmlPath)
        {
            this.MApi = mApi;
            this._TimerBootMSec = TimerBootMSec;
            this.UnReadPlurk = UnreadPlurk;
            this._fifthXMLPath = fifthXmlPath;
            this._IsAlive = false;
            this.aryResponse = this.GetXMLBranch2();
            this.aryStrResponse = this.GetXMLBranch1();
        }
        private bool _IsAlive;
        /// <summary>
        /// 是否所有Thread已經結束
        /// </summary>
        public bool IsAlive
        {
            get { return _IsAlive; }
            set { _IsAlive = value; }
        }
	
        public void AddMyPlurk()
        {
            this._IsAlive = true;
            do
            {
                plurks UnReadplurks = this.UnReadPlurk;
                //計算執行緒數量
                int iThreadCount = (UnReadplurks.Count % iPlurkCountPerThread == 0)
                                    ? (UnReadplurks.Count / iPlurkCountPerThread)
                                    : ((UnReadplurks.Count / iPlurkCountPerThread) + 1);
                List<Thread> ThreadList = new List<Thread>();
                for (int i = 1; i <= iThreadCount; i++)
                {
                    Thread Thread = new Thread(new ParameterizedThreadStart(Process));
                    ThreadList.Add(Thread);
                    //計算此執行緒需處理的plurk,將之加入collection
                    plurks plurks = new plurks();
                    for (int j = (i - 1) * iPlurkCountPerThread; j < i * iPlurkCountPerThread; j++)
                    {
                        if (j >= UnReadplurks.Count) break;
                        plurk plurk = UnReadplurks[j];
                        plurks.Add(plurk);
                    }
                    Thread.Start(plurks);
                }
                #region thread terminating
                bool blnAllThreadIsDead = false;
                do
                {
                    if (ThreadList.Count == 0)
                    {
                        blnAllThreadIsDead = true;
                        break;
                    }
                    foreach (Thread Thread in ThreadList)
                    {
                        if (Thread.IsAlive)
                        {
                            blnAllThreadIsDead = false;
                            break;
                        }
                        else
                        {
                            blnAllThreadIsDead = true;
                            break;
                        }
                    }
                }
                while (!blnAllThreadIsDead);
                #endregion
                if (blnAllThreadIsDead) this._IsAlive = false;
            } while (this._IsAlive);
            
        }
        private void Process(object objPlurks)
        {
            Console.WriteLine("[Module AutoGetFifthFloor] Start");
            plurks plurks = (plurks)objPlurks;
            if (plurks == null) return; //轉換失敗就不處理
            List<Int64> listProcessPlurk = new List<long>();
//int iStart = 0;
            foreach (plurk plurk in plurks)
            {
                #region Get each plurk content,if fail then resume next
                int ResCount = 0;
//iStart = System.Environment.TickCount;
                try
                {
                    plurk pPlurk = this.MApi.getPlurk(plurk.plurk_id);
                    if (pPlurk == null)
                    {
                        Console.WriteLine(string.Format("[Module AutoGetFifthFloor]取得Plurk錯誤,ID:{0}{1}內容:{2}", plurk.plurk_id.ToString(), Environment.NewLine, plurk.content_raw));
                        continue;
                    }

                    ResCount = pPlurk.response_count;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("[Module AutoGetFifthFloor]Error in AutoGetFifthFloor.AddMyPlurk{0}Description:{1} ", Environment.NewLine, ex.Message));
                    if (ex.InnerException!=null)
                        Console.WriteLine(string.Format("               AutoGetFifthFloor.AddMyPlurk{0}Description:{1} ", Environment.NewLine, ex.InnerException.Message));
                    continue;
                }
//Common.PrintUseTime("AutoGetFifthFloor/Process/1", iStart);
                #endregion

                #region Get the fifth floor
                if (ResCount == 4)
                {
//iStart = System.Environment.TickCount;
                    #region Old Code
                    //MApi.responseAdd(Qualifier.says, "上五樓的快活 http://img196.imageshack.us/img196/6992/cover7q.jpg", plurk.plurk_id);

                    //string[] aryStrResponse = new string[15] { 
                    //                                "各位觀眾,專業的**五樓** !"
                    //                                ,"這噗的五樓,很好**我要了**"
                    //                                ,"我想起一張專輯,陳奕迅的**上五樓的快活**" 
                    //                                ,"五樓AT力場展開!"
                    //                                ,"鬼神前鬼，在5樓現臨"
                    //                                ,"五樓是新世界的神！"
                    //                                ,"這就叫「搶到五樓」嗎？"
                    //                                ,"搶到五樓了，是也！"
                    //                                ,"五樓搶到了的說～～"
                    //                                ,"使徒來襲，五樓準備好了！"
                    //                                ,"我要成為五樓王！"
                    //                                ,"青眼白龍加上黑暗魔導師也打不過我的五樓！"
                    //                                ,"貫徹愛與真實的邪惡，可愛又迷人的反派角色，五樓！"
                    //                                ,"就決定是你了！五樓！"
                    //                                ,"天靈靈，地靈靈，噗仙噗仙，五樓降臨！"
                    //                                };
                    #endregion
                    try
                    {
                        Random randObj = new Random();
                        string strResponse = aryStrResponse[randObj.Next(0, aryStrResponse.Length)];
                        MApi.responseAdd(Qualifier.says, strResponse, plurk.plurk_id);
                        Console.WriteLine(string.Format("[Module AutoGetFifthFloor]id:{0},內容:{1},五樓成功!{2}回應內容:{3}", plurk.plurk_id.ToString(), plurk.content, Environment.NewLine, strResponse));
                        listProcessPlurk.Add(plurk.plurk_id);
                        //MApi.mutePlurk(plurk.plurk_id);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format("[Module AutoGetFifthFloor]Error in AutoGetFifthFloor.AddMyPlurk{0}Description:{1} ", Environment.NewLine, ex.Message));
                        continue;
                    }
//Common.PrintUseTime("AutoGetFifthFloor/Process/2", iStart);
                }
                #endregion
                #region Check 5th response
                if (ResCount >= 5)
                {
//iStart = System.Environment.TickCount;
                    responses res = new responses();
                    try
                    {
                        res = MApi.getResponses(plurk.plurk_id, 0);
                        bool addYN = true;
                        int iSEQ = 0;
                        foreach (response nowRes in res)
                        {
                            iSEQ += 1;
                            if (iSEQ > 5 && nowRes.user_id == this.MApi.uid)
                            {
                                addYN = false;
                                break;
                            }
                            if (iSEQ >= 6) break;
                        }
                        if (res.Count < 5) continue;
                        if (!addYN) continue;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format("[Module AutoGetFifthFloor]Error in AutoGetFifthFloor.AddMyPlurk{0}Description:{1} ", Environment.NewLine, ex.Message));
                        Console.WriteLine(string.Format("               AutoGetFifthFloor.AddMyPlurk{0}Description:{1} ", Environment.NewLine, ex.InnerException.Message));
                        continue;
                    }
//Common.PrintUseTime("AutoGetFifthFloor/Process/3", iStart);
//iStart = System.Environment.TickCount;
                    try
                    {
                        response the5thRes = res[4];
                        if (the5thRes.user_id == this.MApi.uid) continue;
                        if (the5thRes == null) continue;
                        //若是該回應時間已經超過每次作動時間+1分鐘時,就不回應被搶的五樓
                        if (the5thRes.posted < DateTime.Now.AddMinutes(0 - ((this._TimerBootMSec / 60000) + 10))) continue;
                        #region 檢查第六個以後的回應,要是噗仙已經回應過了,就不再回應,回應超過10個也不回應了
                        int iSeq = 0;
                        bool blnContinue = true;
                        if (res.Count >= 10) continue;//回應超過10個也不回應了
                        foreach (response response in res)
                        {
                            iSeq += 1;  
                            if (iSeq <= 5) continue;
                            if (response.user_id == this.MApi.uid)
                            {
                                blnContinue = false;    
                                break;
                            }
                        }
                        if (!blnContinue) continue;
                        #endregion
                        #region 檢查被搶
                        if (the5thRes.content_raw.Replace("~", "").IndexOf("五樓", 0) != -1
                            ||
                            the5thRes.content_raw.Replace("~", "").IndexOf("5樓", 0) != -1
                            ||
                            the5thRes.content_raw.Replace("~", "").IndexOf("5F", 0) != -1
                            ||
                            the5thRes.content_raw.Replace("~", "").IndexOf("5f", 0) != -1
                            )
                        {
                            //取得五樓暱稱
                            string the5thResBelonerNickName = Common.GetPlurkNickName(this.MApi, the5thRes.user_id);
                            //string the5thResBelonerNickName = Common.GetPlurkerLink(this.MApi, the5thRes.user_id);
                            if (the5thResBelonerNickName == "") return;
                            //加入連結
                            the5thResBelonerNickName = @"http://www.plurk.com/" + the5thResBelonerNickName;
                            #region Old Code
                            //string aa = Common.GetPlurkerLink(this.MApi, the5thRes.user_id);
                            //string[] aryResponse = new string[5]{
                            //        "{0} 你何苦搶我的專業五樓?!"
                            //        ,"{0} ,別得意,五樓被你搶走了,那是因為我還沒發功咧!"
                            //        ,"既然五樓被{0} 搶走了,我只好搶個"+(ResCount+1).ToString() +"樓來坐坐嘍"
                            //        ,"我都已經沒有五樓可以搶了,還留在這裡惹人嫌嗎?!"
                            //        ,"哇心愛ㄟ五樓係把郎ㄟ~~~"
                            //        };
                            #endregion
                            Random randObj = new Random();
                            string strResponse = string.Format(aryResponse[randObj.Next(0, aryResponse.Length)], the5thResBelonerNickName);
                            MApi.responseAdd(Qualifier.says, strResponse, plurk.plurk_id);
                            Console.WriteLine(string.Format("[Module AutoGetFifthFloor]id:{0},內容:{1},五樓被搶走了!{2}回應內容:{3}", plurk.plurk_id.ToString(), plurk.content, Environment.NewLine, strResponse));
                            //MApi.mutePlurk(plurk.plurk_id);
                            listProcessPlurk.Add(plurk.plurk_id);
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format("[Module AutoGetFifthFloor]Error in AutoGetFifthFloor.AddMyPlurk{0}Description:{1} ", Environment.NewLine, ex.Message));
                        Console.WriteLine(string.Format("               AutoGetFifthFloor.AddMyPlurk{0}Description:{1} ", Environment.NewLine, ex.InnerException.Message));
                        //return sb.ToString();
                        continue;
                    }
//Common.PrintUseTime("AutoGetFifthFloor/Process/4", iStart);
                }
                #endregion
            }
//iStart = System.Environment.TickCount;
            if (listProcessPlurk != null && listProcessPlurk.Count > 0)
            {
                MApi.markAsRead(listProcessPlurk);
                MApi.mutePlurks(listProcessPlurk);
            }
//Common.PrintUseTime("AutoGetFifthFloor/Process/5", iStart);
            Console.WriteLine("[Module AutoGetFifthFloor] End");
        }

        #region 取得搶到五樓的台詞字串陣列
        /// <summary>
        /// 取得搶到五樓的台詞字串陣列
        /// </summary>
        /// <returns></returns>
        private string[] GetXMLBranch1()
        {
            List<string> stringList = new List<string>();
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(this._fifthXMLPath);
            XmlElement root = xDoc.DocumentElement;
            XmlNode Branch1 = root.SelectSingleNode("/root/branch1");
            //Console.WriteLine(Branch1.InnerXml);
            foreach (XmlNode xNode in Branch1.ChildNodes)
            {
                stringList.Add(xNode.ChildNodes[0].InnerText);
            }
            return stringList.ToArray();
        }
        #endregion
        #region 取得沒搶到五樓的台詞字串陣列
        /// <summary>
        /// 取得沒搶到五樓的台詞字串陣列
        /// </summary>
        /// <returns></returns>
        private string[] GetXMLBranch2()
        {
            List<string> stringList = new List<string>();
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(this._fifthXMLPath);
            XmlElement root = xDoc.DocumentElement;
            XmlNode Branch1 = root.SelectSingleNode("/root/branch2");
            //Console.WriteLine(Branch1.InnerXml);
            foreach (XmlNode xNode in Branch1.ChildNodes)
            {
                stringList.Add(xNode.ChildNodes[0].InnerText);
            }
            return stringList.ToArray();
        }
        #endregion 

    }
}
