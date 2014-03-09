using System;
using System.Collections.Generic;
using System.Text;

using PlurkApi;
namespace PlurkModule
{
    /// <summary>
    /// 基礎類別
    /// </summary>
    public class BasicObject
    {
        /// <summary>
        /// API Object
        /// </summary>
        internal PlurkApi.PlurkApi MApi;
        /// <summary>
        /// 集合:未讀噗
        /// </summary>
        internal plurks UnReadPlurk;
    }
}
