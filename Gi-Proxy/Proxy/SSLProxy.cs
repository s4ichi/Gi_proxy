using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Gi_Proxy
{
    /*
     
     
     * 未完成
     
     
     */


    /// <summary>
    /// SSL通信クラス
    /// </summary>
    public class SSLProxy
    {
        //クライアントからのリクエスト
        RequestInfo SslRequest;
        //クライアントのソケット
        Socket SslClientSocket;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SSLProxy(RequestInfo R, Socket C)
        {
            SslRequest = R;
            SslClientSocket = C;
        }

        /// <summary>
        /// SSLに使う証明書の初期設定
        /// </summary>
        static public void SettingSSL()
        {

            return;
        }


        //証明書の内容を表示するメソッド
        private static void PrintCertificate(X509Certificate certificate)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine("Subject={0}", certificate.Subject);
            Console.WriteLine("Issuer={0}", certificate.Issuer);
            Console.WriteLine("Format={0}", certificate.GetFormat());
            Console.WriteLine("ExpirationDate={0}", certificate.GetExpirationDateString());
            Console.WriteLine("EffectiveDate={0}", certificate.GetEffectiveDateString());
            Console.WriteLine("KeyAlgorithm={0}", certificate.GetKeyAlgorithm());
            Console.WriteLine("PublicKey={0}", certificate.GetPublicKeyString());
            Console.WriteLine("SerialNumber={0}", certificate.GetSerialNumberString());
            Console.WriteLine("===========================================");
        }

        /// <summary>
        /// サーバーの証明書の検証
        /// </summary>
        private static Boolean RemoteCertificateValidationCallback(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //PrintCertificate(certificate);
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                //Console.WriteLine("サーバー証明書の検証に成功しました\n");
                return true;
            }
            else
            {
                //何かサーバー証明書検証エラーが発生している
                //SslPolicyErrors列挙体には、Flags属性があるので、
                //エラーの原因が複数含まれているかもしれない。
                //そのため、&演算子で１つ１つエラーの原因を検出する。
                if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) ==
                    SslPolicyErrors.RemoteCertificateChainErrors)
                {
                    //Console.WriteLine("ChainStatusが、空でない配列を返しました");
                }

                if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) ==
                    SslPolicyErrors.RemoteCertificateNameMismatch)
                {
                    //Console.WriteLine("証明書名が不一致です");
                }

                if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNotAvailable) ==
                    SslPolicyErrors.RemoteCertificateNotAvailable)
                {
                    //Console.WriteLine("証明書が利用できません");
                }

                //検証失敗とする
                return false;
            }
        }

        /// <summary>
        /// SSL通信を行う際のメソッド
        /// </summary>
        /// <returns>送れたか否か</returns>
        public bool SSLSendRequest()
        {
            const string CRLF = "\r\n";
            try
            {
                //オリジンサーバーのソケット宣言して接続だけする
                TcpClient ServerSocket = new TcpClient();
                IPAddress[] ipAddresses = Dns.GetHostAddresses(SslRequest.HostName);
                ServerSocket.Connect(new IPEndPoint(ipAddresses[0], SslRequest.Port));

                //サーバーSSL
                SslStream ServerSSLStream = new SslStream(ServerSocket.GetStream(), false, RemoteCertificateValidationCallback);
                ServerSSLStream.AuthenticateAsClient(SslRequest.HostName);

                Console.WriteLine("サーバー証明完了");

                //クライアントSSL
                string path = @"C:\etc\demoCA\cacert.crt";
                string pass = "IRC_saichi";
                NetworkStream n = new NetworkStream(SslClientSocket);
                SslStream ClientSSLStream = new SslStream(n, false, RemoteCertificateValidationCallback);
                ClientSSLStream.AuthenticateAsServer(new X509Certificate2(path, pass));

                Console.WriteLine("クライアント証明完了");

                //クライアントに返すOKを送信
                string ConnectionOK = "HTTP/1.1 200 Connection established" + CRLF + CRLF;
                ClientSSLStream.Write(ASCIIEncoding.ASCII.GetBytes(ConnectionOK));
                Console.WriteLine("HTTP/1.1 200 Connection established");

                byte[] RequestBuffer = new byte[4096];
                byte[] ResponseBuffer = new byte[1];

                //クライアントから要求を取得
                //SslClientSocket.Receive(RequestBuffer);
                ClientSSLStream.Read(RequestBuffer, 0, RequestBuffer.Length);
                Console.WriteLine("要求受信");

                //SSLでオリジンサーバに送信
                ServerSSLStream.Write(RequestBuffer);
                Console.WriteLine("要求送信");

                //オリジンサーバからのレスポンスをクライアントに直接返す
                while (ServerSSLStream.Read(ResponseBuffer, 0, ResponseBuffer.Length) != 0)
                {
                    string outp = ASCIIEncoding.ASCII.GetString(ResponseBuffer);
                    Console.Write(outp);
                    ClientSSLStream.Write(ResponseBuffer);
                }

                //各種解放
                SslClientSocket.Disconnect(false);
                SslClientSocket.Dispose();
                SslClientSocket.Close();
                //ServerSocket.Disconnect(false);
                //ServerSocket.Dispose();
                ServerSocket.Close();
                ServerSSLStream.Close();

                return true;
            }
            catch (Exception E)
            {
                Console.WriteLine("SSL*******");
                Console.WriteLine(E.ToString());
                //Console.WriteLine(SslRequest.HostName);
                Console.WriteLine("**********");
                return false;
            }
        }
    }
}
