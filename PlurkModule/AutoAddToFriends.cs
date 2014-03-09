using System;
using System.Collections.Generic;
using System.Text;

using PlurkApi;
using System.Text.RegularExpressions;
namespace PlurkModule
{
    public class AutoAddToFriends
    {
        PlurkApi.PlurkApi _mApi;
        plurks UnreadPlurk;
        public AutoAddToFriends(PlurkApi.PlurkApi mApi,plurks UnReadPlurk)
        {
            this._mApi = mApi;
            this.UnreadPlurk = UnReadPlurk;
        }

        public void AddMyPlurk()
        {
            Console.WriteLine("[Module AutoAddFriend] Start");
            List<alert> FriendRequestList=this._mApi.getFriendRequests();
            int iSeq=0;
            foreach(alert alert in FriendRequestList)
            {
                switch (alert.type.ToLower())
                {
                    case "friendship_request":
                        #region 符合條件使用者跳過自動加入朋友過程,並且加入block(取消)
                        ////比對顯示名稱
                        //string strDisplayName = alert.user_info.display_name;
                        ////去除空白字元
                        //strDisplayName = Regex.Replace(strDisplayName, @"\s", "");
                        ////設定比對Pattern
                        //string strPattern = @"FAT\d{2,3}";
                        ////符合條件使用者跳過自動加入朋友過程,並且加入block
                        //if (Regex.IsMatch(strDisplayName, strPattern))
                        //{
                        //    this._mApi.block(alert.user_info.id);
                        //    continue;
                        //}
                        #endregion
                        iSeq += 1;
                        this._mApi.addAsFriend(alert.user_info.id);
                        Console.WriteLine( "成功與" + alert.user_info.nick_name +"成為朋友!" );
                        string strMSG = "";
                        switch (iSeq % 11)
                        {
                            case 0: strMSG = "哈囉!我來教你用噗仙"; break;
                            case 1: strMSG = "新朋友嗎?你需要看一下怎麼用噗仙"; break;
                            case 2: strMSG = "我來教新朋友怎麼用噗仙的"; break;
                            case 3: strMSG = "好噗仙,不會用嗎?我教你"; break;
                            case 4: strMSG = "感謝使用噗仙,這裡給你一本噗仙手冊"; break;
                            case 5: strMSG = "喜歡噗仙嗎?我也很喜歡,所以教你用"; break;
                            case 6: strMSG = "噗仙規格:"; break;
                            case 7: strMSG = "偷偷跟你說噗仙的用法,不要告訴別人哦~~"; break;
                            case 8: strMSG = "其實我注意你很久了,怎麼現在才來~~OX董~~"; break;
                            case 9: strMSG = "恭喜你得到了噗仙的使用秘笈!"; break;
                            case 10: strMSG = "看完本噗,你會對噗仙更加了解"; break;
                        }

                        Int64 iTeachPlurkId = this._mApi.plurkAdd(lang.tr_ch, Qualifier.says
                                                                , strMSG
                                                                , "2"
                                                                , alert.user_info.nick_name 
                                                                );
                        if (iTeachPlurkId != -1)
                        {
                            AutoTeach autoTeach = new AutoTeach(this._mApi,this.UnreadPlurk);
                            plurk TeachPlurk = Common.GetPlurk(this._mApi, iTeachPlurkId);
                            List<string> autoTeachContent = autoTeach.GetTeachContent();
                            foreach (string strContent in autoTeachContent)
                            {
                                this._mApi.responseAdd(Qualifier.says, strContent, iTeachPlurkId);
                            }
                            Console.WriteLine(string.Format("[Module AutoAddFriend]增加教學 Plurk id:{1}{0},內容:{2}{0} 使用者:{3}"
                                                       , Environment.NewLine
                                                       , iTeachPlurkId.ToString()
                                                       , TeachPlurk.content_raw
                                                       , Common.GetPlurkNickName(this._mApi, TeachPlurk.user_id)
                                                   ));

                        }
                        else
                        {
                            Console.WriteLine("[Module AutoAddFriend]增加" + alert.user_info.nick_name + "教學失敗");
                        }
                        break;
                    default:
                        break;
                }
            }
            Console.WriteLine("[Module AutoAddFriend] end");
            return;
        }
    }
}
