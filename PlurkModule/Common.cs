using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using PlurkApi;

namespace PlurkModule
{
    public class Common
    {
        /// <summary>
        /// 取得暱稱
        /// </summary>
        /// <param name="mApi"></param>
        /// <param name="PlurkId"></param>
        /// <returns></returns>
        public static string GetPlurkNickName(PlurkApi.PlurkApi mApi, Int64 PlurkId)
        {
            try
            {
                publicProfile pProfile = mApi.getPublicProfile(PlurkId.ToString());
                return pProfile.user_info.nick_name;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        /// <summary>
        /// 取得Plurk
        /// </summary>
        /// <param name="mApi"></param>
        /// <param name="PlurkId"></param>
        /// <returns></returns>
        public static plurk GetPlurk(PlurkApi.PlurkApi mApi, Int64 PlurkId)
        {
            try
            {
                return mApi.getPlurk(PlurkId);
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 取得未讀取的Plurk
        /// </summary>
        /// <param name="mApi"></param>
        /// <returns></returns>
        public static plurks GetUnReadPlurk(PlurkApi.PlurkApi mApi)
        {
            plurks returnPlurks = new plurks();
            returnPlurks = mApi.getUnreadPlurks(DateTime.Now,200);
            #region "取私噗"
            foreach (plurk plurk in mApi.getPlurks(DateTime.Now,100,PlurkTypeFilter.only_private))
            {
                bool addYN = true;
                foreach (plurk checkP in returnPlurks)
                {
                    if (checkP.plurk_id == plurk.plurk_id)
                    {
                        addYN = false;
                        break;
                    }

                    if (checkP.is_unread != 1)
                    {
                        addYN = false;
                        break;
                    }
                }
                if (addYN)
                    returnPlurks.Add(plurk);
            }
            #endregion
            return returnPlurks;
        }
        /// <summary>
        /// 取得今天的日期種類
        /// </summary>
        /// <returns></returns>
        public static DayKind GetWhatDayIsToday(DateTime Today)
        {
            TaiwanLunisolarCalendar TLC = new TaiwanLunisolarCalendar();
            //農曆日期
            DateTime LunaDate = new DateTime(TLC.GetYear(Today)
                                            , TLC.GetMonth(Today)
                                            , TLC.GetDayOfMonth(Today));
            //西元日期
            DateTime DCDate = Today;

            if (!TLC.IsLeapMonth(LunaDate.Year, LunaDate.Month))//處理農曆閏月
            {
                switch (LunaDate.Month)
                {
                    case 1:
                        //農曆一月
                        
                        switch (LunaDate.Day)
                        {
                            //初一到初五
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                                return DayKind.ChineseNewYear;
                        }
                        break;
                    case 5:
                        //五月初五,端午節
                        if (LunaDate.Day == 5)
                            return DayKind.ChineseDragonBoatFestival;
                        break;
                    case 7:
                        //七夕情人節
                        if (LunaDate.Day == 7)
                            return DayKind.ChineseValentine;
                        break;
                    case 8:
                        //八月十五中秋節
                        if (LunaDate.Day == 15)
                            return DayKind.ChineseLunaDay;
                        break;
                    case 12:
                        //除夕
                        if (LunaDate.DayOfYear == (DateTime.IsLeapYear(DCDate.Year) ? 366 : 365))
                            return DayKind.ChineseNewYearEve;
                        break;
                }
            }
            switch (DCDate.Month)
            {
                case 2:
                    switch (DCDate.Day)
                    {
                        case 14:
                            return DayKind.ValentineDay;
                        case 28:
                            return DayKind.Taiwan228;
                    }
                    break;
                case 12:
                    if (DCDate.Day == 24)
                        return DayKind.Xmas;
                    break;
            }
            switch (DCDate.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                case DayOfWeek.Sunday:
                    return DayKind.WeekVocation;
            }
            return DayKind.WorkDay;
        }

        public static void PrintUseTime(string strMSG,  int iStart)
        {
            int iEnd = System.Environment.TickCount;
            double dblDenomenator = 1000;
            Console.WriteLine
                (
                    string.Format
                    (
                        "{0},共使用{1}秒",
                        strMSG
                        ,
                        Convert.ToString(Convert.ToDouble(iEnd - iStart) / dblDenomenator)
                    ) 
                );
        }
    }
    /// <summary>
    /// 日期種類
    /// </summary>
    public enum DayKind
    {
        /// <summary>
        /// 一般工作日
        /// </summary>
        WorkDay=0
        ,
        /// <summary>
        /// 除夕
        /// </summary>
        ChineseNewYearEve = 1
        ,
        /// <summary>
        /// 農曆新年
        /// </summary>
        ChineseNewYear=2
        ,
        /// <summary>
        /// 端午節
        /// </summary>
        ChineseDragonBoatFestival=3
        , 
        /// <summary>
        /// 中秋
        /// </summary>
        ChineseLunaDay = 4
        ,
        /// <summary>
        /// 228
        /// </summary>
        Taiwan228=5
        ,
        /// <summary>
        /// 一般週休假日
        /// </summary>
        WeekVocation=6
        ,
        /// <summary>
        /// 七夕
        /// </summary>
        ChineseValentine=7
        ,
        /// <summary>
        /// 聖誔節
        /// </summary>
        Xmas=8
        ,
        /// <summary>
        /// 情人節
        /// </summary>
        ValentineDay=9
    }

}
