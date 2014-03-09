using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using PlurkApi;
using System.Text.RegularExpressions;

namespace PlurkModule
{
    public class AutoFace
    {
        private string _ServiceAddress = "";
        private string _MethodName = "GetImage";
        private PlurkApi.PlurkApi _mApi;
        private plurks _UnReadPlurks;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ServiceAddress">Web Service位址</param>
        /// <param name="mApi">Plurk Api物件</param>
        /// <param name="UnReadPlurks">未讀噗</param>
        public AutoFace(string ServiceAddress,PlurkApi.PlurkApi mApi,plurks UnReadPlurks)
        {
            if (ServiceAddress.Substring(ServiceAddress.Length - 1, 1) == "/") ServiceAddress = ServiceAddress.Substring(0, ServiceAddress.Length - 1);
            this._ServiceAddress = ServiceAddress;
            this._mApi = mApi;
            this._UnReadPlurks = UnReadPlurks;
        }
        
        public void AddMyPlurk()
        {
            foreach (plurk plurk in this._UnReadPlurks)
            {
                try
                {
                    //沒有關鍵字就往下跳一
                    if (plurk.content_raw.IndexOf("出來面對") == -1) continue;
                    responses responses = this._mApi.getResponses(plurk.plurk_id, 0);
                    //有回應過就不回應了
                    foreach (response response in responses)
                    {
                        if (response.user_id != this._mApi.uid)
                            continue;
                        else
                            return;
                    }
                    string strName = GetName(plurk.content_raw)
                            , strImageAddress = this._ServiceAddress+ GetImageAddress(strName);
                    this._mApi.responseAdd(Qualifier.says, strImageAddress, plurk.plurk_id);
                    Console.WriteLine(string.Format("Module:FaceIt{1}id:{0}{1}內容:{2}{1}回應:{3}"
                                                        , plurk.plurk_id.ToString()
                                                        , Environment.NewLine
                                                        , plurk.content_raw
                                                        , strImageAddress
                                                        ));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Module:FaceIt{1}id:{0}{1}內容:{2}{1}Error:{3}{1}{4}"
                                                        , plurk.plurk_id.ToString()
                                                        , Environment.NewLine
                                                        , plurk.content_raw
                                                        , ex.Message
                                                        ,ex.StackTrace
                                                        ));
                    continue;
                }
                
            }
        }

        /// <summary>
        /// 呼叫產生器
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string GetImageAddress(string str)
        {
            //建立一個 WebRequest，並設定傳輸方法為 POST  
            //網址是 WebService的網址，方法是WebService的方法 
            WebRequest request = WebRequest.Create(String.Format("{0}/imageproduce.asmx/{1}", this._ServiceAddress, this._MethodName));
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            //下面這段是對你要帶的參數編碼(這是用http post傳輸的方式)             
            //如果WebMetod是 get(int i) 那參數就是 i=10 
            //如果是 get(string str,int i) 那參數可以是 str=gogo&i=10 
            string wsData = "str=" +str ;
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(wsData);
            request.ContentLength = bs.Length;
            request.GetRequestStream().Write(bs, 0, bs.Length);

            //取得 WebResponse 的物件 然後把回傳的資料讀出 
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader sr = new StreamReader(dataStream);
            string responseFormServer = sr.ReadToEnd();
            if (responseFormServer.IndexOf("Error", StringComparison.CurrentCultureIgnoreCase) != -1)
                return "";
            else
                return Regex.Replace( Regex.Replace(responseFormServer, @"<\s*?[^>]+\s*?>", ""),@"\s","");
        }

        private string GetName(string strM)
        {
            string strPattern = @"\d*\W*\w*出來面對"
                    ,strContent=Regex.Match(strM, strPattern).Value;
            return Regex.Replace(strContent, @"\d*\W*", "").Replace("出來面對", "");
        }
    }
}
