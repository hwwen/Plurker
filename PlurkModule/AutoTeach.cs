using System;
using System.Collections.Generic;
using System.Text;

using PlurkApi;

namespace PlurkModule
{
    public class AutoTeach:BasicObject
    {
        private static List<string> _ListStrTeach = new List<string>();

        public AutoTeach(PlurkApi.PlurkApi mApi,plurks UnreadPlurk)
        {
            this.MApi = mApi;
            _ListStrTeach = new List<string>();
            SetTeachContent();
            this.UnReadPlurk = UnreadPlurk;
        }
        public  List<string> GetTeachContent()
        {
            if (_ListStrTeach != null)
            {
                return _ListStrTeach;
            }
            else
            {
                SetTeachContent();
                return _ListStrTeach;
            }
        }

        private  void SetTeachContent()
        {
            _ListStrTeach.Add("噗仙規格 ");
            _ListStrTeach.Add("1.股價查詢：用**問**+股票代號或名稱會回應價位及提供連結至奇摩股市該股票代號相關網頁,一次頂多只會回應2支股票,超過會因為防洪規則被擋掉,同一噗裡股票代號不可問超過3個,超過3個的直接不回應,問多個股票時需要用逗號隔開");
            _ListStrTeach.Add("2.搶五樓：並且如果五樓被搶走時,會依照回應時間及回應內容比對是真的被搶走,還是噗仙自己idle太久,被搶走時會唉唉叫一下的功能");
            _ListStrTeach.Add("3.匯率計算：用**問** 加上 **匯率怎麼用**,會回應相關教學");
            _ListStrTeach.Add("4.美食推薦,噗的主文內容裡有**晚餐**或**下午茶**或**宵夜**,噗仙會自己挑一則食記出來回應你");
            _ListStrTeach.Add("5.忘了怎麼用噗仙就 請用**問** 加上 **噗仙怎麼用**,就會回應教學內容");
        }
        public void AddMyPlurk()
        {
            Console.WriteLine("[Module AutoTeach] Start");

            List<Int64> listProcessPlurk = new List<long>();
            foreach (plurk plurk in UnReadPlurk)
            {
                //if (plurk.qualifier != "asks") continue;
                if (plurk.Plurk_Qualifier != Qualifier.asks) continue;
                bool blnAddTeach = true;
                #region 比對回應-已回應過的就不再回應(取消)
                //responses plurkRes = this.MApi.getResponses(plurk.plurk_id, 5);
                //foreach (response res in plurkRes)
                //{
                //    //已回應過的就不再回應
                //    if (res.user_id == this.MApi.uid)
                //    {
                //        blnAddTeach = false;
                //        break;
                //    }
                //}
                #endregion
                if (plurk.content_raw.IndexOf("噗仙怎麼用", 0) != -1
                    &&
                    blnAddTeach==true
                    )
                {
                    foreach (string strTeachContent in _ListStrTeach)
                    {
                        this.MApi.responseAdd(Qualifier.says, strTeachContent, plurk.plurk_id);
                    }
                    Console.WriteLine(string.Format("[Module AutoTeach]增加教學 Plurk id:{1}{0},內容:{2}{0} 使用者:{3}"
                                                    ,Environment.NewLine
                                                    ,plurk.plurk_id.ToString()
                                                    ,plurk.content_raw
                                                    , Common.GetPlurkNickName(this.MApi, plurk.owner_id)
                                                ));
                    listProcessPlurk.Add(plurk.plurk_id);
                }
            }
            if (listProcessPlurk != null && listProcessPlurk.Count > 0)
            {
                MApi.markAsRead(listProcessPlurk);
                MApi.mutePlurks(listProcessPlurk);
            }
            Console.WriteLine("[Module AutoTeach] End");
            return;

        }
    }
}
