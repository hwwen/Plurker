using System;
using System.Collections.Generic;
using System.Text;

namespace PlurkModule
{
    #region 基底參數物件
    /// <summary>
    /// 基底參數物件
    /// </summary>
    public class PlurkerParameter
    {
        private PlurkApi.PlurkApi _Api;
        /// <summary>
        /// Plurk API
        /// </summary>
        public PlurkApi.PlurkApi Api
        {
            get { return _Api; }
            set { _Api = value; }
        }
        private PlurkApi.plurks _UnRead;
        /// <summary>
        /// 未讀噗
        /// </summary>
        public PlurkApi.plurks UnReadPlurk
        {
            get { return _UnRead; }
            set { _UnRead = value; }
        }
    }
    #endregion
    #region 搶五樓參數集合物件
    /// <summary>
    /// 搶五樓參數集合物件
    /// </summary>
    public class FifthParameter : PlurkerParameter
    {
        private string _xmlDataPath;
        /// <summary>
        /// 存放五樓回應的XML路徑
        /// </summary>
        public string XmlFilePath
        {
            get { return _xmlDataPath; }
            set { _xmlDataPath = value; }
        }
        private int _TimerBootMSec;
        /// <summary>
        /// 作動時間
        /// </summary>
        public int TimerBootMSec
        {
            get { return _TimerBootMSec; }
            set { _TimerBootMSec = value; }
        }
    }
    #endregion
    #region AutoStocker Parameter Collection Object
    public class StockerParameter : PlurkerParameter
    {
        private string _XmlFilePath;
        /// <summary>
        /// Stock data XML File path
        /// </summary>
        public string XmlFilePath
        {
            get { return _XmlFilePath; }
            set { _XmlFilePath = value; }
        }
	
    }
    #endregion
    #region AutoTeach Parameter Collection Ojbect
    public class TeachParameter : PlurkerParameter
    {
    }
    #endregion
    #region AutoSayHello Parameter collection object
    public class SayHelloParameter : PlurkerParameter
    {
        private DateTime _now;

        /// <summary>
        /// Thread processing time
        /// </summary>
        public DateTime ExecDateTime
        {
            get { return _now; }
            set { _now = value; }
        }
        private string _foodXmlPath;

        public string FoodXmlFilePath
        {
            get { return _foodXmlPath; }
            set { _foodXmlPath = value; }
        }
	
    }
    #endregion
    #region AutoAddFriend parameter collection object
    public class AddFriednParameter : PlurkerParameter
    {
    }
    #endregion
    #region AutoExrate parameter collectionobject
    public class AutoExrateParameter : PlurkerParameter
    {
    }
    #endregion
    #region AutoFoodSuggest parameter collectionobject
    public class AutoFoodSuggestParameter : PlurkerParameter
    {
        /// <summary>
        /// Food content XML file real path
        /// </summary>
        public string XmlFilePath;
    }
    #endregion


    public class ThreadCallFunction
    {
        public static void CallForGetFifth(object objFifthParameter)
        {
            FifthParameter FifthParameter = (FifthParameter)objFifthParameter;//轉換型別
            if (FifthParameter == null) return;//不成功就傳出
            AutoGetFifthFloor AutoGetFifthFloor = new AutoGetFifthFloor(FifthParameter.Api
                                                                        , FifthParameter.TimerBootMSec
                                                                        , FifthParameter.UnReadPlurk
                                                                        , FifthParameter.XmlFilePath);
            AutoGetFifthFloor.AddMyPlurk();
        }

        public static void CallForTeach(object objTeachParameter)
        {
            TeachParameter TeachParameter = (TeachParameter)objTeachParameter;//轉換型別
            if (TeachParameter == null) return;//不成功就傳出
            PlurkModule.AutoTeach AutoTeach = new AutoTeach(TeachParameter.Api, TeachParameter.UnReadPlurk);
            AutoTeach.AddMyPlurk();
        }

        public static void CallForAddFriend(object objAddFriednParameter)
        {
            AddFriednParameter AddFriednParameter = (AddFriednParameter)objAddFriednParameter;//轉換型別
            if (AddFriednParameter == null) return;//不成功就傳出
            PlurkModule.AutoAddToFriends AutoAddToFriends = new AutoAddToFriends(AddFriednParameter.Api, AddFriednParameter.UnReadPlurk);
            AutoAddToFriends.AddMyPlurk();
        }

        public static void CallForSayHello(object objSayHelloParameter)
        {
            SayHelloParameter SayHelloParameter = (SayHelloParameter)objSayHelloParameter;//轉換型別
            if (SayHelloParameter == null) return;//不成功就傳出
            PlurkModule.AutoSayHello AutoSayHello = new AutoSayHello(SayHelloParameter.Api
                                                                , SayHelloParameter.FoodXmlFilePath
                                                                , SayHelloParameter.ExecDateTime);
            AutoSayHello.AddMyPlurk();
        }
        //PyJxDS8MttU5FG+vhVJDmA==
        public static void CallForStockQuery(object objStockerParameter)
        {
            StockerParameter StockerParameter = (StockerParameter)objStockerParameter;//轉換型別
            if (StockerParameter == null) return;//不成功就傳出
            PlurkModule.AutoStocker AutoStocker = new AutoStocker(StockerParameter.Api
                                                                , StockerParameter.UnReadPlurk
                                                                , StockerParameter.XmlFilePath);
            AutoStocker.AddResponse();
        }

        public static void CallForExrate(object objExrateParameter)
        {
            AutoExrateParameter AutoExrateParameter = (AutoExrateParameter)objExrateParameter;//convert type
            if (AutoExrateParameter == null) return;
            PlurkModule.AutoExchangeRate AutoExchangeRate = new AutoExchangeRate(AutoExrateParameter.Api
                                                                                , AutoExrateParameter.UnReadPlurk);
            AutoExchangeRate.AddPlurk();
        }

        public static void CallForFoodSuggest(object objFoodSuggestParameter)
        {
            AutoFoodSuggestParameter AutoFoodSuggestParameter = (AutoFoodSuggestParameter)objFoodSuggestParameter;
            if (AutoFoodSuggestParameter == null) return;
            PlurkModule.AutoFoodSuggest AutoFoodSuggest = new AutoFoodSuggest(AutoFoodSuggestParameter.Api, AutoFoodSuggestParameter.UnReadPlurk, AutoFoodSuggestParameter.XmlFilePath);
            AutoFoodSuggest.AddMyPlurk();
        }
    }

}
