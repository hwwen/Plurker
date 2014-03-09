using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PlurkModule
{
    internal class GetConfig
    {
        /// <summary>
        /// 取得貨幣同義詞列表檔案位址
        /// </summary>
        /// <returns></returns>
        internal static string GetCurrencySynonymousFilePath()
        {
            if (PlurkModule.Properties.Settings.Default.DecimalSynonymousFile == null
                ||
                PlurkModule.Properties.Settings.Default.DecimalSynonymousFile == string.Empty)
                throw new Exception("貨幣同義詞檔未設定");
            string strFilePath=PlurkModule.Properties.Settings.Default.DecimalSynonymousFile;
            if (!File.Exists(strFilePath))
                throw new Exception(string.Format("貨幣同義詞檔未存在於{0},請檢查設定",strFilePath));
            return strFilePath;
        }
    }
}
