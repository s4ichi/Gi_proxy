using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Gi_Proxy
{
    /// <summary>
    /// 上位プロキシに通信行うクラス
    /// </summary>
    public class UpperProxy
    {
        //上位プロキシIP,ポート
        public string IPAdress;
        public Int32 Port;

        private Socket ServerSocket;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="I">IPアドレス</param>
        /// <param name="P">ポート</param>
        public UpperProxy(string I, Int32 P)
        {
            IPAdress = I;
            Port = P;
        }

        /// <summary>
        /// 上位に送信
        /// </summary>
        /// <param name="Message">送信内容</param>
        public void Send(string Message)
        {
            try
            {
                //上位プロキシ用送信メソッド
                ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //サーバーと接続
                ServerSocket.Connect(IPAdress, Port);

                //送信
                ServerSocket.Send(ASCIIEncoding.ASCII.GetBytes(Message));
            }
            catch (Exception E)
            {
                Console.WriteLine("UpperProxyErrer*****");
                Console.WriteLine(E.ToString());
                Console.WriteLine("********************\n");
            }
        }

        /// <summary>
        /// 上位から受信→クライアントに返す
        /// </summary>
        /// <param name="Client">クライアントのソケット</param>
        public void ReceiveToSend(Socket Client)
        {
            try
            {
                string res = "";
                byte[] ResponseBuffer = new byte[1];
                while (true)
                {
                    int d = ServerSocket.Receive(ResponseBuffer);
                    if (d == 0) break;
                    res += ASCIIEncoding.ASCII.GetString(ResponseBuffer);
                    int cnt = Client.Send(ResponseBuffer);

                    //接続が切れてたら終了
                    if (!Client.Connected || cnt == 0) break;
                    if (!ServerSocket.Connected) break;
                }
                ServerSocket.Close();

            }
            catch (Exception E)
            {
                Console.WriteLine("UpperSendErrer*****");
                Console.WriteLine(E.ToString());
                Console.WriteLine("*******************\n");
            }
        }
    }
}
