using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gi_Proxy
{
    /// <summary>
    /// ひとつのグラフ情報を格納するクラス
    /// </summary>
    public class NodeNetwork
    {
        //グラフ情報
        public NodeParty Nodes = new NodeParty();
        //テーブル情報
        public ICollection<TableItem> TableItems = new List<TableItem>();
    }
}
