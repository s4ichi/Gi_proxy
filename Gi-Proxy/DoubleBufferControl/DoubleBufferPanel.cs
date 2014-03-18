using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Gi_Proxy
{
    /// <summary>
    /// パネル継承してダブルバッファを追加したパネルオブジェクト
    /// </summary>
    public class DoubleBufferPanel : Panel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DoubleBufferPanel()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();
        }
    }
}
