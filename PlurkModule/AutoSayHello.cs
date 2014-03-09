using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using PlurkApi;

namespace PlurkModule
{
    public class AutoSayHello:BasicObject
    {
        string FoodXMLPath = "";
        DateTime NOW;
        string TestMessage;
        public AutoSayHello(PlurkApi.PlurkApi mApi, string FoodXmlPath,DateTime Now)
        {
            this.MApi = mApi;
            this.FoodXMLPath = FoodXmlPath;
            this.NOW = Now;
        }

        public AutoSayHello(PlurkApi.PlurkApi mApi, string FoodXmlPath, DateTime Now,string testMessage)
        {
            this.MApi = mApi;
            this.FoodXMLPath = FoodXmlPath;
            this.NOW = Now;
            this.TestMessage = testMessage;
        }
        public void AddMyPlurk()
        {
            Console.WriteLine("[Module AutoSayHello] Start");
            string strMsg = "";
            DayKind dk = Common.GetWhatDayIsToday(this.NOW);
            #region 整點
            switch (this.NOW.Hour)
            {
                case 8:
                    switch (dk)
                    {
                        case DayKind.ChineseNewYearEve:
                            strMsg = "今天除夕!快起床準備划龍舟了(誤)";
                            break;
                        case DayKind.ChineseNewYear:
                            strMsg = "過年當然是要吃粽子吃到隔天早上八點才對呀!(還是誤)";
                            break;
                        case DayKind.ChineseLunaDay:
                            strMsg = "很好,中秋節是該早起準備拜年才是惹";
                            break;
                        case DayKind.ChineseDragonBoatFestival:
                            strMsg = "端午節耶!快點準備月餅和袖子呀!!!";
                            break;
                        case DayKind.WeekVocation:
                            strMsg = "";
                            break;
                        default:
                            strMsg = "";
                            break;                                    
                    }
                    break;
                case 18:
                    switch (dk)
                    {
                        case DayKind.ChineseNewYearEve:
                            strMsg = "除夕划龍舟到下午六點真是年輕人的表現呀(狀態顯示為:汗水淋漓)";
                            break;
                        case DayKind.ChineseNewYear:
                            strMsg = "別吵,昨天晚上賞月烤肉,噗仙現在要補眠";
                            break;
                        case DayKind.ChineseValentine:
                            strMsg = "七夕常下雨,大家記得要帶雨具，不要在今天濕身了哦~~~~(devil)";
                            break;
                        case DayKind.ChineseLunaDay:
                            strMsg = "孩子呀,中秋節晚上要記得趕回家圍爐哦!";
                            break;
                        case DayKind.ChineseDragonBoatFestival:
                            strMsg = "端午節的晚餐應該要吃月餅嗎??";
                            break;
                        case DayKind.WeekVocation:
                            strMsg = "";
                            break;
                        default:
                            strMsg = "";
                            break;
                    }
                    break;
                case 20:
                    switch (dk)
                    {
                        case DayKind.ChineseLunaDay:
                            strMsg = "娘子,快跟牛魔王一起出來看月亮!!";
                            break;
                        default:
                            strMsg = "";
                            break;
                    }
                    break;
               
                case 23:
                    switch (dk)
                    {
                        case DayKind.Xmas:
                            strMsg = "快去煙囪底下抓聖誔老人!";
                            break;
                        default:
                            strMsg = "";
                            break;
                    }
                    break; 
                default:
                    break;
            }
            #endregion

            if (strMsg.Trim() != "" && strMsg.Trim() != string.Empty)
                MApi.plurkAdd(PlurkApi.lang.tr_ch
                                 , PlurkApi.Qualifier.says
                                 , strMsg
                                 , PlurkCommentType.OnlyFriend
                                    , "");
            if (strMsg.Trim() == string.Empty)
                Console.WriteLine(string.Format("[Module AutoSayHello]Time:{0} No Message", this.NOW.ToString("yyyy-MM-dd hh:mm"), Environment.NewLine));
            else
                Console.WriteLine(string.Format("[Module AutoSayHello]Time:{0}{1}Message:{2}", this.NOW.ToString("yyyy-MM-dd hh:mm"), Environment.NewLine, strMsg));

            Console.WriteLine("[Module AutoSayHello] End");
        }
    }
}

