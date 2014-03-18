using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gi_Proxy
{
    /// <summary>
    /// 設定内容を表すクラス
    /// </summary>
    public class SettingState
    {
        public UpperProxy Ups;
        public bool PassStatus;//パスが設定されているか
        public bool PassSet;//パスが認証されているか

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="Up">上位プロキシ</param>
        /// <param name="Pst">パスコード系</param>
        /// <param name="Pss">パスコード系</param>
        public SettingState(UpperProxy Up,bool Pst,bool Pss)
        {
            Ups = Up;
            PassStatus = Pst;
            PassSet = Pss;
        }

        /// <summary>
        /// パスワードの設定
        /// </summary>
        /// <param name="P"></param>
        public void SetPassword(string P)
        {
            Gi_Proxy.Properties.Settings.Default.EncryptPass = Encrypt.EncryptString("miyagikougyou", P);
        }

        /// <summary>
        /// パスワード認証
        /// </summary>
        /// <param name="P"></param>
        /// <returns>正誤</returns>
        public bool CheckPass(string P)
        {
            if (Encrypt.EncryptString("miyagikougyou", P) == Gi_Proxy.Properties.Settings.Default.EncryptPass)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
