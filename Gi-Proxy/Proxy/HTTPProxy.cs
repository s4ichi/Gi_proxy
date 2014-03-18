using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Gi_Proxy
{
    /// <summary>
    /// HTTP通信用クラス
    /// </summary>
    public class HTTPProxy
    {
        //クライアント
        Socket HttpClientSocket;
        //HTTPリクエスト内容
        RequestInfo HttpRequest;
        //サーバー
        Socket ServerSocket;

        //パラレルで動作させて通信する最大の数
        private const int MaxSendThread = 100;
        //現在のスレッド数
        //private int NowSendThread = 0;
        //スレッドで送信する際の数合わせ
        //private long CountReadData = 0;
        //private long SentReadData = 0;

        //デリゲートメソッド格納用
        private EndHTTPDelegateMethod CallBack;
        //送信が正しく完了したか
        //private bool ThreadBoolean;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HTTPProxy(RequestInfo R, Socket C)
        {
            HttpRequest = R;
            HttpClientSocket = C;
            CallBack = new EndHTTPDelegateMethod(this.EndSend);
            //ThreadBoolean = true;
        }

        /// <summary>
        /// デリゲートメソッド
        /// </summary>
        /// <param name="flg">送信の結果</param>
        public void EndSend(bool flg)
        {
            //NowSendThread--;
            //if (!flg) ThreadBoolean = false;
        }

        /// <summary>
        /// パラレルで送受信行う
        /// </summary>
        /// <param name="Thcnt">パラレス数</param>
        /// <returns>送信の結果</returns>
        private bool ThreadSend(int Thcnt)
        {
            //パラレル
            //Parallel.For(0, Thcnt, id => 
            //{
            //    SendResponse();
            //    //NowSendThread++;
            //});

            //送受信終わるまで待機
            //while (NowSendThread != 0) ;
            SendResponse();
            //解放
            HttpClientSocket.Disconnect(false);
            HttpClientSocket.Dispose();
            return true;
        }


        /// <summary>
        /// クライアントへサーバーからのバッファを返す
        /// </summary>
        /// <param name="Server">サーバーのソケット</param>
        /// <returns>送れたか否か</returns>
        private void SendResponse()
        {
            //サーバーから返ってくるバッファ
            byte[] ResponseBuffer = new byte[1];

            try
            {
                int d;
                //すべて受け取り尽くすまで繰り返す
                while (true)
                {
                    //サーバから受け取る
                    d = ServerSocket.Receive(ResponseBuffer);
                    if (d == 0) break;
                    //long Num = CountReadData;
                    //CountReadData++;

                    //while (SentReadData != Num) ;
                    //クライアント送信
                    HttpClientSocket.Send(ResponseBuffer);
                    //SentReadData++;
                    
                    //接続が切れてんならおしまい
                    if (!HttpClientSocket.Connected) break;
                    if (!ServerSocket.Connected) break;
                }

                //CallBack(true);
            }
            catch (Exception E)
            {
                Console.Write("***SendResponseError***\n" + E + "\n\n" + HttpRequest.Message + "\n***************\n\n");
                //CallBack(false);
            }
        }

        /// <summary>
        /// サーバーへ整形したリクエストを送信
        /// </summary>
        /// <param name="Port">サーバーのポート番号</param>
        /// <param name="HostName">サーバーのホスト名</param>
        /// <param name="RequestMessage">サーバーへのリクエスト本文</param>
        /// <returns>送れたか否か</returns>
        public bool SendRequest()
        {
            try
            {
                //サーバーのソケット用意
                ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //サーバーと接続
                ServerSocket.Connect(HttpRequest.HostName, HttpRequest.Port);
                //サーバーにリクエストを送信
                ServerSocket.Send(ASCIIEncoding.ASCII.GetBytes(HttpRequest.Message));

                //クライアントへ受け取ったデータを送信
                if (ThreadSend(MaxSendThread) == false)
                {
                    ServerSocket.Disconnect(false);
                    ServerSocket.Dispose();
                    return false;
                }

                ServerSocket.Disconnect(false);
                ServerSocket.Dispose();

                return true;
            }
            catch (Exception E)
            {
                Console.Write("***SendRequestError***\n" + HttpRequest.HostName + "\n" + E + "\n***************\n\n");
                return false;
            }
        }
    }
}
