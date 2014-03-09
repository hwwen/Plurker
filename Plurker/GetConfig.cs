using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using PlurkModule;

namespace Plurker
{
    public class GetConfig
    {
        /// <summary>
        /// 取得API Key
        /// </summary>
        /// <returns></returns>
        public static string GetApiKey()
        {
            return PlurkApplication.Default.PlurkAPIKey;
        }
        /// <summary>
        /// 取得使用者名稱
        /// </summary>
        /// <returns></returns>
        public static string GetUserName()
        {
            return PlurkApplication.Default.PlurkUserName;
        }
        /// <summary>
        /// 取得使用者密碼
        /// </summary>
        /// <returns></returns>
        public static string GetPassword()
        {
            return PlurkApplication.Default.PlurkPassword;
        }

        /// <summary>
        /// 取得Timer啟動週期毫秒數
        /// </summary>
        /// <returns></returns>
        public static int GetTimerInterval()
        {
            //return PlurkApplication.Default.TimerBootMSec;
            if (PlurkApplication.Default.TimerBootMSec == 0
                ||
                PlurkApplication.Default.TimerBootMSec.ToString() == "")
            {
                return 300000;
            }
            else
            {
                return PlurkApplication.Default.TimerBootMSec;
            }
        }

        public static bool GetTurnOff()
        {
            return PlurkApplication.Default.TurnOff;
        }

        public static bool GetMaintain()
        {
            return PlurkApplication.Default.Maintain;
        }

        public static bool GetApprove()
        {
            return PlurkApplication.Default.Approve;
        }
        public static string GetMessage()
        {
            return PlurkApplication.Default.Message;
        }
        public static string GetStockDataFile()
        {
            try
            {
                string strPath = PlurkApplication.Default.stockDataPath;
                if (strPath.Trim() == string.Empty) throw new Exception("設定檔未設定股票資料檔路徑");
                if (!File.Exists(strPath)) throw new Exception("股票資料檔不存在");
                return strPath;
            }
            catch (FileNotFoundException FileNotExistEX)
            {
                throw new Exception("股票資料檔不存在");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetfifthDataFile()
        {
            try
            {
                string strPath = PlurkApplication.Default.fifthDataPath;
                if (strPath.Trim() == string.Empty) throw new Exception("設定檔未設定五樓資料檔路徑");
                if (!File.Exists(strPath)) throw new Exception("五樓資料檔不存在");
                return strPath;
            }
            catch (FileNotFoundException FileNotExistEX)
            {
                throw new Exception("五樓資料檔不存在");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetFoodDataFile()
        {
            try
            {
                string strPath = PlurkApplication.Default.foodDataPath;
                if (strPath.Trim() == string.Empty) throw new Exception("設定檔未設定食物資料檔路徑");
                if (!File.Exists(strPath)) throw new Exception("食物資料檔不存在");
                return strPath;
            }
            catch (FileNotFoundException FileNotExistEX)
            {
                throw new Exception("食物資料檔不存在");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 是否啟動股票物件
        /// </summary>
        /// <returns></returns>
        public static bool BootStock()
        {
            try
            {
                return PlurkApplication.Default.AutoStock;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 是否啟動教學
        /// </summary>
        /// <returns></returns>
        public static bool BootTeach()
        {
            try
            {
                return PlurkApplication.Default.AutoTeach;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool BootHello()
        {
            try
            {
                return PlurkApplication.Default.AutoSayHello;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool BootAddFriend()
        {
            try
            {
                return PlurkApplication.Default.AutoAddFriend;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool BootFifth()
        {
            try
            {
                return PlurkApplication.Default.AutoFifth;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool DebugMode()
        {
            try
            {
                return PlurkApplication.Default.DebugMode;
            }
            catch (Exception)
            {
                return true;
            }
        }

        public static bool BootFaceIt()
        {
            try
            {
                return PlurkApplication.Default.AutoFaceIt;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static string FaceItServiceAddress()
        {
            try
            {
                return PlurkApplication.Default.FaceItService;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static bool AutoExrate()
        {
            try
            {
                return PlurkApplication.Default.AutoExrate;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool AutoFoodSuggest()
        {
            try
            {
                return PlurkApplication.Default.AutoFoodSuggest;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
