using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace PlurkModule
{
    public class FoodItemCollection
    {
        public FoodItemCollection()
        {
        }

        public static List<FoodItem> GetFoodItemCollection(string FoodXmlPath, string FoodKind)
        {
            XmlDocument xDoc = new XmlDocument();
            XmlNode FoodKindNode;
            List<FoodItem> FoodItemList = new List<FoodItem>();
            xDoc.Load(FoodXmlPath);
            XmlElement root = xDoc.DocumentElement;
            XmlNode XmlNode = root.SelectSingleNode("/root/" + FoodKind);
            if (XmlNode == null)
                throw new Exception("XML設定錯誤,無" + FoodKind + "節點資料");
            XmlNodeList XmlNodeList = XmlNode.SelectNodes("/root/" + FoodKind+"/item");

            foreach (XmlNode xmlNode in XmlNodeList)
            {
                if (xmlNode.ChildNodes.Count>=3)
                {
                    FoodItem FoodItem = new FoodItem();
                    FoodItem.FoodName = xmlNode.ChildNodes[0].InnerText;
                    FoodItem.IntroLink = xmlNode.ChildNodes[1].InnerText;
                    FoodItem.ParaGraph =(xmlNode.ChildNodes[2].InnerText.Length>20? xmlNode.ChildNodes[2].InnerText.Substring(0,20):"");
                    if (xmlNode.ChildNodes.Count >= 4)
                        FoodItem.ImagePath = xmlNode.ChildNodes[3].InnerText;
                    else
                        FoodItem.ImagePath = "";
                    FoodItemList.Add(FoodItem);
                }
            }
            return FoodItemList;
        }
    }

    public class FoodItem
    {
        private string _FoodName;
        /// <summary>
        /// 名稱
        /// </summary>
        public string FoodName
        {
            get { 
                    return _FoodName; 
                }
            set { _FoodName = value; }
        }

        private string _IntroLink;
        /// <summary>
        /// 介紹食記連結
        /// </summary>
        public string IntroLink
        {
            get { return _IntroLink; }
            set { _IntroLink = value; }
        }

        private string _Paragraph;
        /// <summary>
        /// 範例段落
        /// </summary>
        public string ParaGraph
        {
            get { return _Paragraph; }
            set { _Paragraph = value; }
        }

        private string _imagePath;

        public string ImagePath
        {
            get { return _imagePath; }
            set { _imagePath = value; }
        }
	
    }
}
