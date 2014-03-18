using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.PowerPacks;
using System.Drawing;
using System.Windows.Forms;


namespace Gi_Proxy
{
    /// <summary>
    /// グラフの線の情報を持つクラス
    /// </summary>
    public class Edge
    {
        //このエッジはどの座標へのエッジか
        public Node ToNode { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="To">向かうノード名</param>
        public Edge(Node To)
        {
            this.ToNode = To;
        }

        /// <summary>
        /// 線Object
        /// </summary>
        public LineShape edge = new LineShape();
    }
}
