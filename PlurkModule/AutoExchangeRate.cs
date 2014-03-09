using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Net;

using PlurkApi;
using PlurkModule.net.webservicex.www;
using System.Text.RegularExpressions;

namespace PlurkModule
{
    public class AutoExchangeRate
    {
        private PlurkApi.PlurkApi _mApi;
        private plurks _UnreadPlurk;
        private XmlDocument xmlDoc = new XmlDocument();
        private XmlElement root;
        public AutoExchangeRate(PlurkApi.PlurkApi mApi, plurks UnReadPlurk)
        {
            xmlDoc.Load(PlurkModule.GetConfig.GetCurrencySynonymousFilePath());
            root = xmlDoc.DocumentElement;
            this._mApi = mApi;
            this._UnreadPlurk = UnReadPlurk;
        }
        public void AddPlurk()
        {
            Console.WriteLine("[Module AutoExchangeRate] Start");
            List<Int64> listProcessPlurk = new List<long>();
            if (_UnreadPlurk.Count == 0)
                Console.WriteLine("[Module AutoExchangeRate] No such request");
            else
            {
                foreach (plurk plurk in _UnreadPlurk)
                {
                    try
                    {
                        //not ask and resume next
                        if (plurk.Plurk_Qualifier != Qualifier.asks) continue;
                        //已經回應過的就不再處理
                        #region 
                        bool blnContinue=true;
                        responses responses = _mApi.getResponses(plurk.plurk_id, 0);
                        foreach (response response in responses)
                        {
                            if (response.user_id == _mApi.uid)
                            {
                                blnContinue = false;
                                break;
                            }
                        }
                        if (!blnContinue) continue;
                        #endregion
                        //詢問教學時,進入教學回應後,往下一則
                        #region
                        if (plurk.content.IndexOf("匯率怎麼用") != -1)
                        {
                            this.TeachHowToUse(plurk);
                            listProcessPlurk.Add(plurk.plurk_id);
                            continue;
                        }
                        #endregion
                        if (plurk.content.IndexOf("換") == -1) continue;
                        //
                        if (Regex.IsMatch(plurk.content, @"\w{2,4}\d+換\w{2,4}"))
                        {
                            Match Match = Regex.Match(plurk.content, @"(?<origin>\D{2,4})(?<amount>\d+)換(?<dest>\D{2,4})");
                            Console.WriteLine(String.Format("[Module AutoExchangeRate]{0}    origin:{1}{0}    amount:{2}{0}    dest:{3}"
                                                            , Environment.NewLine
                                                            , Match.Groups["origin"].ToString()
                                                            , Match.Groups["amount"].ToString()
                                                            , Match.Groups["dest"].ToString()
                                ));
                            string strOriCurName = this.ParseCurrencyName(Match.Groups["origin"].ToString())
                                , strDestCurName = this.ParseCurrencyName(Match.Groups["dest"].ToString());
                            int intOriAmount = 0;
                            try
                            {
                                intOriAmount = int.Parse(Match.Groups["amount"].ToString());
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(string.Format("[Module AutoExchangeRate] convert error {0}    amount:{1}     Error reason:{2}" 
                                                                ,Environment.NewLine
                                                                , Match.Groups["amount"].ToString()
                                                                , ex.Message
                                                ));
                                continue;
                            }
                            string strRes = this.Calulator(strOriCurName, strDestCurName, intOriAmount);
                            listProcessPlurk.Add(plurk.plurk_id);
                            if (!_mApi.responseAdd(Qualifier.says, strRes, plurk.plurk_id))
                            {
                                Console.WriteLine(string.Format("[Module AutoExchangeRate] convert error {0}    amount:{1}"
                                                               , Environment.NewLine
                                                               , Match.Groups["amount"].ToString()
                                               ));
                            }
                            else
                            {
                                Console.WriteLine(string.Format("[Module AutoExchangeRate] convert Succese {0}    Content:{1}"
                                                                , Environment.NewLine
                                                                , strRes
                                                ));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[Module AutoExchangeRate] Error reason:" +ex.Message);
                        continue;
                    }
                }

                if (listProcessPlurk != null && listProcessPlurk.Count > 0)
                    _mApi.markAsRead(listProcessPlurk);
                //_mApi.mutePlurk(listProcessPlurk);
            }

            Console.WriteLine("[Module AutoExchangeRate] End");
        }

        #region 計算
        /// <summary>
        /// 計算
        /// </summary>
        /// <param name="strOriCurName">原幣</param>
        /// <param name="strDestCurName">目的幣</param>
        /// <param name="iAmount">金額</param>
        /// <returns></returns>
        private string Calulator(string strOriCurName,string strDestCurName,int iAmount)
        {
            try
            {
                //取得原幣名稱
                XmlElement CurrentyXmlElement = (XmlElement)root.SelectSingleNode("/root/synonymous/currency/item[@value=\"" + strOriCurName + "\"]");
                strOriCurName = CurrentyXmlElement.ParentNode.Attributes.GetNamedItem("name").Value;
                if (CurrentyXmlElement == null) return string.Empty;
                //取得目的幣名稱
                CurrentyXmlElement = (XmlElement)root.SelectSingleNode("/root/synonymous/currency/item[@value=\"" + strDestCurName + "\"]");
                strDestCurName = CurrentyXmlElement.ParentNode.Attributes.GetNamedItem("name").Value;
                if (CurrentyXmlElement == null) return string.Empty;

                double rate = GetExrate(strOriCurName, strDestCurName)
                    , dAmount = (double)iAmount;

                string strMes = string.Format("匯率計算:可由{0} 金額:{1}換成{2} 金額:{3} 轉換匯率:{4}"
                                                , strOriCurName
                                                , iAmount.ToString()
                                                , strDestCurName
                                                , Math.Round(dAmount * rate, 6)
                                                ,rate
                                            );

                return strMes;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Module AutoExchangeRate] Method:Calulator Error reason:"+ ex.Message);
                return string.Empty;
            }
        }
        #endregion

        #region 取得匯率
        private double GetExrate(string strFromCurName,string strToCurName)
        {
            CurrencyConvertor CC = new CurrencyConvertor();
            double rate = CC.ConversionRate((Currency)Enum.Parse(typeof(Currency), strFromCurName)
                                            , (Currency)Enum.Parse(typeof(Currency), strToCurName)
                                            );
            Console.WriteLine("[Module AutoExchangeRate] Rate:" +rate.ToString());
            return rate;
        }
        #endregion
        #region 整理貨幣名稱字串方法
        /// <summary>
        /// 整理貨幣名稱字串
        /// </summary>
        /// <param name="CurrencyName"></param>
        /// <returns></returns>
        private string ParseCurrencyName(string CurrencyName)
        {
            CurrencyName = CurrencyName.Replace("臺", "台");
            CurrencyName = CurrencyName.Replace("磅", "鎊");
            CurrencyName = CurrencyName.ToUpper().Trim();
            return CurrencyName;
        }
        #endregion
        #region 教學
        /// <summary>
        /// 教學
        /// </summary>
        /// <param name="plurk"></param>
        private void TeachHowToUse(plurk plurk)
        {
            try
            {
                #region 取得XML中的教學內容
                List<string> teachStrList = new List<string>();
                XmlNodeList TCNodeList = root.SelectNodes("/root/teachcontent/item");
                if (TCNodeList == null)
                {
                    Console.WriteLine("[Module AutoExchangeRate] Teach content is Empty!");
                    return;
                }
                else
                {
                    foreach (XmlNode xmlNode in TCNodeList)
                    {
                        XmlElement XmlElement = (XmlElement)xmlNode;
                        if (XmlElement == null) continue;
                        teachStrList.Add(XmlElement.FirstChild.Value);
                    }
                }
                #endregion 
                #region 將教學內容丟到回應中
                foreach (string str in teachStrList)
                {
                    if (!_mApi.responseAdd(Qualifier.says, str, plurk.plurk_id))
                    {
                        Console.WriteLine(string.Format("[Module AutoExchangeRate] teach Fail{0}    method:{1}{0}    content:{2}"
                                    , Environment.NewLine
                                    , "TeachHowToUse"
                                    , str
                        ));
                    }
                }
                #endregion
                #region 列出已支援幣別
                StringBuilder sbResponse = new StringBuilder();
                sbResponse.Append("目前已支援幣別:");
                bool blnFirst = true;
                XmlNodeList CurrentyNodeList = root.SelectNodes("/root/synonymous/currency");
                foreach (XmlNode XmlNode in CurrentyNodeList)
                {
                    sbResponse.Append(((blnFirst)? "" :";")
                                    + XmlNode.Attributes["ctname"].Value);
                    blnFirst = false;
                }
                if (!_mApi.responseAdd(Qualifier.says, sbResponse.ToString(), plurk.plurk_id))
                {
                    Console.WriteLine(string.Format("[Module AutoExchangeRate] teach Fail{0}    method:{1}{0}    content:{2}"
                                , Environment.NewLine
                                , "TeachHowToUse"
                                , sbResponse.ToString()
                    ));
                    return;
                }
                #endregion
                //finish teach method
                Console.WriteLine("[Module AutoExchangeRate] Teach finish 使用者:" + Common.GetPlurkNickName(this._mApi, plurk.owner_id));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("[Module AutoExchangeRate] error{0}    method:{1}{0}    reason:{2}"
                                    ,Environment.NewLine
                                    ,"TeachHowToUse"
                                    ,ex.Message
                                    ));
            }
        }
        #endregion
    }
}
    