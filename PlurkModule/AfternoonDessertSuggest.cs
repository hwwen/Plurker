using System;
using System.Collections.Generic;
using System.Text;

using PlurkApi;

namespace PlurkModule
{
    public class Suggest
    {

        public static string GetAfternoonDessertSuggestion(string SuggestionXMLPath)
        {
            List<FoodItem> FoodItemList = FoodItemCollection.GetFoodItemCollection(SuggestionXMLPath, "afternoon");
            if (FoodItemList == null) return string.Empty;
            if (FoodItemList.Count == 0) return string.Empty;
            Random randObj = new Random();
            FoodItem FoodItem = FoodItemList[randObj.Next(0, FoodItemList.Count - 1)];
            if (FoodItem.IntroLink.Trim() == string.Empty || FoodItem.FoodName.Trim() == string.Empty) return string.Empty;
            return string.Format("今日下午茶推薦:{0} ({1}) {2} {3}",FoodItem.IntroLink,FoodItem.FoodName,FoodItem.ParaGraph,FoodItem.ImagePath);
        }
        public static string GetDinnerSuggestion(string SuggestionXMLPath)
        {
            List<FoodItem> FoodItemList = FoodItemCollection.GetFoodItemCollection(SuggestionXMLPath, "dinner");
            if (FoodItemList == null) return string.Empty;
            if (FoodItemList.Count == 0) return string.Empty;
            Random randObj = new Random();
            FoodItem FoodItem = FoodItemList[randObj.Next(0, FoodItemList.Count - 1)];
            if (FoodItem.IntroLink.Trim() == string.Empty || FoodItem.FoodName.Trim() == string.Empty) return string.Empty;
            return string.Format("今日晚餐推薦:{0} ({1}) {2} {3}", FoodItem.IntroLink, FoodItem.FoodName, FoodItem.ParaGraph, FoodItem.ImagePath);
        }
        public static string GetMidnightSnackSuggestion(string SuggestionXMLPath)
        {
            List<FoodItem> FoodItemList = FoodItemCollection.GetFoodItemCollection(SuggestionXMLPath, "midnightsnack");
            if (FoodItemList == null) return string.Empty;
            if (FoodItemList.Count == 0) return string.Empty;
            Random randObj = new Random();
            FoodItem FoodItem = FoodItemList[randObj.Next(0, FoodItemList.Count - 1)];
            if (FoodItem.IntroLink.Trim() == string.Empty || FoodItem.FoodName.Trim() == string.Empty) return string.Empty;
            return string.Format("肚子餓了嗎?今日宵夜推薦:{0} ({1}) {2} {3}", FoodItem.IntroLink, FoodItem.FoodName, FoodItem.ParaGraph, FoodItem.ImagePath);
        }

    }
}
