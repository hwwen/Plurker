using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Xml;

using PlurkApi;

namespace PlurkModule
{
    /// <summary>
    /// 股票報價相關
    /// </summary>
    public class AutoStocker
    {
        private PlurkApi.PlurkApi _mApi;
        private plurks UnreadPlurk;
        private string _strStockData;

        public AutoStocker(PlurkApi.PlurkApi mApi, plurks UnReadPlurk,string StockDataXMLFilePath)
        {
            this._mApi = mApi;
            this.UnreadPlurk = UnReadPlurk;
            this._strStockData = StockDataXMLFilePath;
        }

        public void AddResponse()
        {
            //StringBuilder sb = new StringBuilder();
            plurks plurks = this.UnreadPlurk;
            Console.WriteLine(AddResponse(plurks, true).ToString());

        }

        private string AddResponse(plurks plurks, bool checkResponse)
        {
            try
            {
                Console.WriteLine("[Module AutoStockQuery] Start");
                List<Int64> listProcessPlurk = new List<long>();
                foreach (plurk plurk in plurks)
                {
                    //if (plurk.qualifier != "asks") continue;
                    if (plurk.Plurk_Qualifier != Qualifier.asks) continue;
                    List<string> ListStockID = new List<string>();
                    int iAsks = 0
                            , iResponsed = 0;
                    string strPlurk = plurk.content_raw.ToLower().Replace("asks","").Replace("問","");
                    //是否用數字來查詢
                    Regex Regex = new Regex(@"\A\w*[0-9]{4}");
                    if (Regex.IsMatch(strPlurk))
                    {
                        //遇到連續五個數字的就不回應了
                        if (Regex.IsMatch(strPlurk, "[0-9]{5}"))
                        {
                            Console.WriteLine("Plurk:" + strPlurk + Environment.NewLine + "No Response");
                            continue;
                        }
                        foreach (Match match in Regex.Matches(strPlurk, ",?[0-9]{4},?"))
                        {
                            iAsks += 1;
                            ListStockID.Add(match.Value.Replace(",", ""));
                        }
                    }
                    else
                    {
                        string[] arrayStockName = strPlurk.Split(',');
                        ListStockID = this.GetStockIdByName(arrayStockName);
                    }
                    //大於一天以上的詢問就不回應了
                    if (plurk.posted < DateTime.Now.AddDays(-1)) continue;

                    responses resCollection = this._mApi.getResponses(plurk.plurk_id, 0);
                    foreach (response response in resCollection)
                    {
                        if (response.user_id == 6113925) iResponsed += 1;
                    }
                    iAsks = ListStockID.Count;
                    if (iAsks > 3) return "";
                    if (iAsks > iResponsed)
                    {
                        StringBuilder sbResponse = new StringBuilder();
                        int iHadResponse = 0, ithisTime = 0;
                        foreach (string strStockID in ListStockID)
                        {
                            iHadResponse += 1;
                            if (iHadResponse <= iResponsed) continue;
                            if (ithisTime > 1) return string.Format("Plurk:{0}{1}Response as :{1}{2}", strPlurk, Environment.NewLine, sbResponse.ToString());
                            ithisTime += 1;
                            string stockResponse = "";
                            //如果用名稱查不到該股票代號時
                            if (!Regex.IsMatch(strStockID, ",?[0-9]{4},?"))
                            {
                                sbResponse.AppendLine(string.Format("找不到{0}相關股票報價", strStockID));
                                listProcessPlurk.Add(plurk.plurk_id);
                                continue;
                            }
                            #region 刪除找不到股票時的回應
                            //{
                            //stockResponse = "老大!別耍我了,沒" + strStockID + "這一支的報價呀!你要不要先檢查一下股票代號呀?";
                            //if (!this._mApi.responseAdd(Qualifier.says
                            //        , stockResponse
                            //        , plurk.plurk_id))
                            //    return string.Format("AutoStocker.AddResponse Error:{0}{1}Response as :{1}{2}"
                            //        , strPlurk
                            //        , Environment.NewLine
                            //        ,stockResponse
                            //        );
                            //continue;
                            //}
                            #endregion
                            else
                            {
                                stockResponse = GetStockPrice(strStockID);
                                //if (stockResponse == "老大!別耍我了,沒" + strStockID + "這一支的報價呀!你要不要先檢查一下股票代號呀?")
                                if (stockResponse == string.Empty)
                                {
                                    sbResponse.AppendLine(string.Format("找不到{0}相關股票報價", strStockID));
                                    listProcessPlurk.Add(plurk.plurk_id);
                                    continue;
                                }
                                #region 刪除找不到股票時的回應
                                //{
                                //if (!this._mApi.responseAdd(Qualifier.says, stockResponse, plurk.plurk_id))
                                //{
                                //    sbResponse.AppendLine(string.Format("AutoStocker.AddResponse Error:{0}{1}Response as :{1}{2}", strPlurk, Environment.NewLine, stockResponse));
                                //    continue;
                                //}
                                //continue;
                                //}
                                #endregion
                                else
                                {

                                    stockResponse = (ithisTime == 1) ? 
                                                    "你要知道的" + GetStockNameById(strStockID) + ",價位是:" + stockResponse 
                                                    : 
                                                    "股票名稱:" + GetStockNameById(strStockID) + ",目前是:" + stockResponse;
                                    if (!this._mApi.responseAdd(Qualifier.says, stockResponse, plurk.plurk_id))
                                    {
                                        sbResponse.AppendLine(string.Format("AutoStocker.AddResponse Error:{0}{1}Response as :{1}{2}", strPlurk, Environment.NewLine, stockResponse));
                                        continue;
                                    }
                                    listProcessPlurk.Add(plurk.plurk_id);
                                }
                            }
                            sbResponse.AppendLine(stockResponse);
                        }

                        return string.Format("[Module AutoStockQuery]Plurk:{0}{1}Response as :{1}{2}", strPlurk, Environment.NewLine, sbResponse.ToString());
                    }

                    #region 原程式
                    //Regex Regex = new Regex(@"\A\w*[0-9]{4}");
                    //if (Regex.IsMatch(strPlurk))
                    //{
                    //    int iAsks = 0
                    //        , iResponse = 0;
                    //    //bool addResponse = true;
                    //    StringBuilder sbStock = new StringBuilder();
                    //    //遇到連續五個數字的就不回應了
                    //    if (Regex.IsMatch(strPlurk, "[0-9]{5}"))
                    //    {
                    //        Console.WriteLine("Plurk:" + strPlurk + Environment.NewLine + "No Response");
                    //        continue;
                    //    }
                    //    foreach (Match match in Regex.Matches(strPlurk, ",?[0-9]{4},?"))
                    //    {
                    //        iAsks += 1;
                    //        sbStock.AppendLine(match.Value.Replace(",", ""));
                    //    }

                    //    responses resCollection = this._mApi.getResponses(plurk.plurk_id, 0);
                    //    foreach (response response in resCollection)
                    //    {
                    //        if (response.user_id == 6113925)
                    //        {
                    //            //addResponse = false;
                    //            iResponse += 1;
                    //        }
                    //        if (checkResponse && response.qualifier == "asks")
                    //        {
                    //            string strResponse = response.content_raw;
                    //            if (Regex.IsMatch(strResponse, ",?[0-9]{4},?"))
                    //            {
                    //                //遇到連續五個數字的就不回應了
                    //                if (Regex.IsMatch(strResponse, "[0-9]{5}"))
                    //                {
                    //                    Console.WriteLine("Plurk:" + strResponse + Environment.NewLine + "No Response");
                    //                    continue;
                    //                }

                    //                foreach (Match match in Regex.Matches(strResponse, ",?[0-9]{4},?"))
                    //                {
                    //                    iAsks += 1;
                    //                    sbStock.AppendLine(match.Value.Replace(",", ""));
                    //                }
                    //            }
                    //        }
                    //    }
                    //    //回應超過六個股票代號就不回應了
                    //    if (iResponse >= 6) continue;

                    //if (iAsks > iResponse)
                    //{
                    //    MatchCollection MatchStockId = Regex.Matches(sbStock.ToString().Replace(Environment.NewLine, ","), ",?[0-9]{4},?");
                    //    StringBuilder sbResponse = new StringBuilder();
                    //    int iHadResponse = 0, ithisTime = 0;
                    //    foreach (Match match in MatchStockId)
                    //    {
                    //        iHadResponse += 1;
                    //        if (iHadResponse <= iResponse) continue;
                    //        if (ithisTime > 2) return string.Format("Plurk:{0}{1}Response as :{1}{2}", strPlurk, Environment.NewLine, sbResponse.ToString());
                    //        ithisTime += 1;
                    //        string stockID = match.Value.Replace(",", "")
                    //            , stockResponse = GetStockPrice(stockID);
                    //        if (stockResponse == "老大!別耍我了,沒" + stockID + "這一支的報價呀!你要不要先檢查一下股票代號呀?")
                    //        {
                    //            if (!this._mApi.responseAdd(Qualifier.says, stockResponse, plurk.plurk_id))
                    //                return string.Format("AutoStocker.AddResponse Error:{0}{1}Response as :{1}{2}", strPlurk, Environment.NewLine, stockResponse);
                    //        }
                    //        else
                    //        {
                    //            //string stockName = GetStockName(stockID);
                    //            stockResponse = "你要知道的" + GetStockNameById(stockID) + ",價位是:" + stockResponse;
                    //            if (!this._mApi.responseAdd(Qualifier.says, stockResponse, plurk.plurk_id))
                    //                return string.Format("AutoStocker.AddResponse Error:{0}{1}Response as :{1}{2}", strPlurk, Environment.NewLine, stockResponse);

                    //        }
                    //        sbResponse.AppendLine(stockResponse);
                    //    }

                    //    return string.Format("Plurk:{0}{1}Response as :{1}{2}", strPlurk, Environment.NewLine, sbResponse.ToString());
                    //}
                    //}
                    #endregion
                }
                if (listProcessPlurk != null && listProcessPlurk.Count > 0)
                {
                    _mApi.markAsRead(listProcessPlurk);
                    _mApi.mutePlurks(listProcessPlurk);
                }
                Console.WriteLine("[Module AutoStockQuery] End");
                return "";
            }
            catch (Exception ex)
            {
                return "AutoStock Exception:" + ex.Message;
            }
        }
        #region 取得股價
        /// <summary>
        /// 取得股價
        /// </summary>
        /// <param name="StockId">股票代號</param>
        /// <returns></returns>
        private string GetStockPrice(string StockId)
        {
            string url = "http://tw.stock.yahoo.com/q/q?s=" + StockId;
            HttpWebRequest hwRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            try
            {
                HttpWebResponse ResPage = (HttpWebResponse)hwRequest.GetResponse();
                Stream ResponseStream = ResPage.GetResponseStream();
                StreamReader sr = new StreamReader(ResponseStream
                                                , System.Text.Encoding.GetEncoding("big5"));
                Regex Regex = new Regex(@"<td align=""center"" bgcolor=""#FFFfff"" nowrap><b>[0-9]{1,}.[0-9]{0,2}</b></td>");

                string sTemp = sr.ReadToEnd();
                #region Remark Code
                //Regex RegExName = new Regex(@"<td align=""center"" width=""105""><a href=""/q/bc?s=" + StockId + @""">" + StockId + @"聯發科</a><br><a href=""/pf/pfsel?stocklist=" + StockId + @";""><font size=""-1"">加到投資組合</font><br></a></td>");
                //Regex RegExName = new Regex(@"<td align=""center"" width=""105""><a href=""" );

                //string StockName;
                //Match NameMatch = RegExName.Match(sTemp);
                //if (NameMatch.Value != "")
                //{
                //    StockName = Regex.Replace(NameMatch.Value, @"<\s*?[^>]+\s*?>", "");
                //}
                #endregion
                Match Match = Regex.Match(sTemp);
                //return Regex.Replace(Match.Value, @"<\s*?[^>]+\s*?>", "");
                if (Match.Value != "")
                {
                    return url + " (" + Regex.Replace(Match.Value, @"<\s*?[^>]+\s*?>", "") + ")";
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
            finally
            {

            }
        }
        #endregion
        #region 依傳入股票代號取得股票名稱
        /// <summary>
        /// 依傳入股票代號取得股票名稱
        /// </summary>
        /// <param name="StockId">股票代號</param>
        /// <returns></returns>
        private string GetStockNameById(string StockId)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(this._strStockData);
            XmlElement root=xmlDoc.DocumentElement;
            XmlElement thisStock = (XmlElement)root.SelectSingleNode("/root/stock[@id=" + StockId +"]");
            if (thisStock == null) return StockId;
            return thisStock.Attributes.GetNamedItem("name").Value;
        }
        #endregion
        #region 依傳入股票名稱取得股票代號
        /// <summary>
        /// 依傳入股票名稱取得股票代號
        /// </summary>
        /// <param name="ListStockName">股票名稱Collection</param>
        /// <returns></returns>
        private List<string> GetStockIdByName(string[] ListStockName)
        {
            List<string> ListStockId = new List<string>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(this._strStockData);
            XmlElement root = xmlDoc.DocumentElement;
            foreach (string strStockName in ListStockName)
            {
                string StockName = GetSysnoym(strStockName);
                if (StockName.Length>4) continue;
                string strStockId = GetStockIdByName(StockName);
                if (strStockId != string.Empty)
                {
                    ListStockId.Add(strStockId);
                }
                else
                {
                    ListStockId.Add(StockName);
                }
            }
            return ListStockId;
        }
        /// <summary>
        /// 依傳入股票名稱取得股票代號
        /// </summary>
        /// <param name="StockName">股票名稱</param>
        /// <returns></returns>
        private string GetStockIdByName(string StockName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(this._strStockData);
            XmlElement root = xmlDoc.DocumentElement;
            XmlElement thisStock = (XmlElement)root.SelectSingleNode("/root/stock[@name='" + StockName + "']");
            if (thisStock == null) return "";
            return thisStock.Attributes.GetNamedItem("id").Value;
        }
        #endregion
        #region 取得同義字
        /// <summary>
        /// 取得同義字
        /// </summary>
        /// <param name="StockName"></param>
        /// <returns></returns>
        private static string GetSysnoym(string StockName)
        {
            return StockName.Replace('臺','台');
        }
        #endregion

    }
}
