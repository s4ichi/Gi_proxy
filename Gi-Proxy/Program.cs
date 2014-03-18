using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace Gi_Proxy
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    //通信あとのデリゲートで使用
    public delegate void EndRequestDelegateMethod(RequestInfo Req);

    //HTTP通信のthreadデリゲードで使用
    public delegate void EndHTTPDelegateMethod(bool flg);
}
