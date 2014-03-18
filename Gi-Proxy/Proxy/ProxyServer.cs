using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;
using System.Drawing;

namespace Gi_Proxy
{
    //クライアントのソケット受け取って通信をするクラス
    public class ProxyServer
    {
        //末字
        const string CRLF = "\r\n";
        //接続するクライアントのソケット
        public Socket ClientSocket { get; private set; }
        //クライアントから受け取るリクエスト情報を格納するリスト
        public RequestInfo Request;
        //デリゲート用
        private EndRequestDelegateMethod CallBack;
        //BANホスト名を格納する
        private ICollection<string> BanHostName;

        /*上位プロキシテスト用*/
        //上位プロキシ切り替えフラグ
        private bool UpperProxyFlg = false;
        //上位プロキシ情報
        private UpperProxy UP;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="Client">クライアントのソケット</param>
        public ProxyServer(Socket Client, EndRequestDelegateMethod CallBackDelegateMethod, ICollection<string> Bans, UpperProxy Up)
        {
            this.ClientSocket = Client;
            this.Request = new RequestInfo();
            this.CallBack = CallBackDelegateMethod;
            this.BanHostName = Bans;
            this.UP = new UpperProxy(Up.IPAdress, Up.Port);
            if (UP.IPAdress != "") UpperProxyFlg = true;
        }

        /// <summary>
        /// クライアントのリクエスト取得部分
        /// </summary>
        /// <returns>送れたか否か</returns>
        private bool GetRequest()
        {
            //returns
            string RequestLine1 = "", RequestLine2 = "";
            //クライアントから取得するバッファ
            byte[] RequestBuffer = new byte[1];

            try
            {
                while (true)
                {
                    //取得
                    if (ClientSocket.Receive(RequestBuffer) == 0)
                    {
                        return false;
                    }

                    /***メッセージ終了解析部***/
                    //1バイトずつ受信しているため通信の終わりを判定する必要がある

                    string Message = ASCIIEncoding.ASCII.GetString(RequestBuffer);
                    RequestLine1 += Message;
                    RequestLine2 += Message;

                    if (RequestLine1.EndsWith(CRLF))
                    {
                        Request.Data.Add(RequestLine1.Trim());
                        RequestLine1 = "";
                    }
                    if (RequestLine2.EndsWith(CRLF + CRLF))
                    {
                        //Console.Write(RequestLine2);
                        Request.OldMessage = RequestLine2;
                        return true;
                    }
                    /********************/
                }

            }
            catch (Exception E)
            {
                Console.Write("***GetRequestError***\n" + E + "\n\n");
                return false;
            }
        }

        /// <summary>
        /// 取得の失敗またはフィルタリングがされていたときに返す
        /// </summary>
        /// <param name="Server">サーバーのソケット</param>
        /// <returns>送れたか否か</returns>
        private bool ErrorSendResponse()
        {
            try
            {
                //リソースからhtmlファイルを読み込み
                string TextHtml = Gi_Proxy.Properties.Resources.index;

                //エンコード
                Encoding JISenc = Encoding.GetEncoding("Shift_JIS");
                byte[] bytes = JISenc.GetBytes(TextHtml);

                //送信
                ClientSocket.Send(bytes);

                ClientSocket.Disconnect(false);
                ClientSocket.Dispose();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// threadpoolで動かしている流れの部分。uri解析も行う
        /// </summary>
        /// <param name="obj"></param>
        public void Run(object obj)
        {
            try
            {
                //クライアントからのリクエスト取得
                if (GetRequest() == false)
                {
                    ErrorSendResponse();
                    return;
                }
               
                //Httpメソッドが"CONNECT"だったら
                if (Request.Data[0].IndexOf("CONNECT") == 0)
                {
                    //未完成につき対象外と同じ仕様
                    Console.WriteLine("対象外のメソッドです");
                    ErrorSendResponse();
                    ClientSocket.Disconnect(false);
                    ClientSocket.Dispose();
                    return;
                }
                //Httpメソッドが"GET"だったら
                else if (Request.Data[0].IndexOf("GET") == 0)
                {
                    //httpメソッド
                    Request.Method = Request.Data[0].Split(' ')[0];
                    string[] s = { "://" };

                    //プロトコル
                    Request.Protocol = Request.Data[0].Split(' ')[1].Split(s, StringSplitOptions.None)[0];
                    if (Request.Protocol == "http")
                    {
                        Request.Port = 80;
                    }
                    else
                    {
                        Console.Write("対象外のメソッド");
                        Request.Port = 80;
                        ClientSocket.Disconnect(false);
                        ClientSocket.Dispose();
                        return;
                    }

                    //ホスト名
                    Request.HostName = Request.Data[0].Split(' ')[1].Replace(Request.Protocol + "://", "").Split('/')[0];
                    //通信のバージョン
                    Request.Ver = Request.Data[0].Split(' ')[2];

                    //アクセスするファイル名
                    Request.FileName = Request.Data[0].Split(' ')[1].Replace(Request.Protocol + "://", "").Replace(Request.HostName, "");
                    //?以下を消す
                    string file = Request.Data[0].Split(' ')[1].Replace(Request.Protocol + "://", "").Replace(Request.HostName, "").Split('?')[0];

                    //ルートディレクトリが指定されていなかった場合の対処
                    if (Request.FileName == "") Request.FileName = "/";

                    Request.Data[0] = Request.Method + ' ' + Request.FileName + ' ' + Request.Ver;

                    //オリジンサーバーに送る最終的なメッセージの生成
                    foreach (string S in Request.Data)
                    {
                        Request.Message += S;
                        Request.Message += CRLF;
                    }

                    //もし要求されてきたホストがBANされてたら
                    if (BanHostName.Contains(Request.HostName) || BanHostName.Contains("localhost"))
                    {
                        ErrorSendResponse();
                        return;
                    }

                    //上位プロキシが設定されていたら
                    if (UpperProxyFlg)
                    {
                        UP.Send(Request.OldMessage);
                        UP.ReceiveToSend(ClientSocket);
                        ClientSocket.Close();
                    }
                    else
                    {
                        //Httpで送信する
                        HTTPProxy Http = new HTTPProxy(Request, ClientSocket);

                        //送信部分
                        if (Http.SendRequest() == false)
                        {
                            ErrorSendResponse();
                            return;
                        }
                    }

                    //デリゲート部分
                    CallBack(Request);
                }
                else
                {
                    Console.WriteLine("対象外のメソッドです");
                    ErrorSendResponse();
                    ClientSocket.Disconnect(false);
                    ClientSocket.Dispose();
                }
            }
            catch (Exception E)
            {
                Console.Write("***RunError***\n" + E + "\n****************\n\n");
            }
        }
    }
}
