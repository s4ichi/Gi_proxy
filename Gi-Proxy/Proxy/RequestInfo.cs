using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Gi_Proxy
{
    /// <summary>
    /// クライアントからのリクエスト内容を解析したものを格納
    /// </summary>
    public class RequestInfo
    {
        public List<string> Data;
        public string Message;
        public string OldMessage;
        public string HostName;
        public string FileName;
        public string Ver;
        public string Protocol;
        public string Method;
        public int Port;

        public RequestInfo()
        {
            this.OldMessage = "";
            this.Data = new List<string>();
            this.Message = "";
            this.HostName = "";
            this.FileName = "";
            this.Ver = "";
            this.Protocol = "";
            this.Method = "";
            this.Port = 0;
        }
    }
}
