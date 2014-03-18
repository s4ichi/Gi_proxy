using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gi_Proxy
{
    /// <summary>
    /// テーブルビューに表示する情報を保持するクラス
    /// </summary>
    public class TableItem
    {
        public string URL;
        public string Domain;
        public string AccessTime;

        public void SetTableItem(string U, string D, string A)
        {
            URL = U;
            Domain = D;
            AccessTime = A;
        }
    }
}
