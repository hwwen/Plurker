using System;
using System.Collections.Generic;
using System.Text;

using PlurkApi;

namespace PlurkModule
{
    class AutoFoodSuggest:BasicObject
    {
        private string FoodXMLFilePath;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mApi"></param>
        /// <param name="UnReadPlurk"></param>
        /// <param name="FoodXmlFilePath"></param>
        public AutoFoodSuggest(PlurkApi.PlurkApi mApi, plurks UnreadPlurk, string FoodXmlFilePath)
        {
            this.MApi = mApi;
            this.UnReadPlurk = UnreadPlurk;
            this.FoodXMLFilePath = FoodXmlFilePath;
        }

        public void AddMyPlurk()
        {
            Console.WriteLine("[Module AutoFoodSuggest] Start");
            List<long> listProcessPlurk = new List<long>();
            foreach (plurk plurk in this.UnReadPlurk)
            {
                string strResponse = "";
                if (plurk.content.IndexOf("晚餐") != -1)
                    strResponse = Suggest.GetDinnerSuggestion(this.FoodXMLFilePath);
                else if (plurk.content.IndexOf("下午茶") != -1)
                    strResponse = Suggest.GetAfternoonDessertSuggestion(this.FoodXMLFilePath);
                else if (plurk.content.IndexOf("宵夜") != -1)
                    strResponse = Suggest.GetMidnightSnackSuggestion(this.FoodXMLFilePath);
                if (strResponse != "")
                {
                    MApi.responseAdd(Qualifier.says, strResponse, plurk.plurk_id);
                    Console.WriteLine(string.Format("[Module AutoFoodSuggest] id:{0} 內容:{1}",plurk.plurk_id,strResponse));
                    listProcessPlurk.Add(plurk.plurk_id);
                }
            }
            if (listProcessPlurk != null && listProcessPlurk.Count > 0)
            {
                MApi.markAsRead(listProcessPlurk);
            }
            Console.WriteLine("[Module AutoFoodSuggest] End");
        }
    }
}
