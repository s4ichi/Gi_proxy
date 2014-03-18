using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.PowerPacks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Gi_Proxy;
using System.Security.Cryptography.X509Certificates;

namespace Gi_Proxy
{
    public partial class MainForm : Form
    {
        //グラフ1つ1つのデータを管理する連想配列
        private Dictionary<string, NodeNetwork> GraphData;

        //ソフトウェア名
        private string ApplicationName = "Gi-Proxy";

        //デフォルトのグラフ
        private string MainNodeParty = "Today";

        //選択中のグラフ
        private string NowSetGraph = "Today";

        //powerpacksの親設定に必要
        private ShapeContainer sc = new ShapeContainer();

        //プロキシのスレッド
        private Thread ThProxy;

        //プロキシサーバーのソケット
        private Socket Server;

        //設定内容
        private SettingState SettingDefault;

        //デフォルト上位プロキシ
        private UpperProxy UpperProxyDefault;

        //BANホスト名
        private ICollection<string> BanList;

        //TableView用のList
        private ICollection<TableItem> NowTableItems;

        //境界線用のラベル
        private Label[] LineLabel = new Label[4];

        //説明用のラベル
        private Label[] DataLabel = new Label[4];

        //説明表示用のテキストボックス
        private TextBox[] DataTextBox = new TextBox[4];

        //円描画用の直径
        private int R = 15;

        //ズーム時最大最小直径
        private const int MinR = 5;
        private const int MaxR = 25;

        //エッジの長さの変化の割合
        private const double RateOfChange = 7.0d;

        //ドラッグイベントフラグ
        private bool DraggFlg = false;

        //ノードドラッグイベントフラグ
        private bool NodeDragFlg = false;
        private string LockNodeName = "";

        //フォームを×ボタンから消したかそれ以外か
        private bool WhichCloseButton = false; 


        /*****ToolBar処理用変数*****/

        //フィルタリング全体のON・OFF切り替え
        private bool FilteringSwitch = true;

        //粗さ滑らかさでイベントの処理切り替え
        private bool RSFlg = true;

        /**************************/


        //マウスダウンした座標
        private Point MouseDownPoint;

        //ホイールの初期値
        private int InitWheel = 0;

        /// <summary>
        /// フォームコンストラクタ
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            this.ShowInTaskbar = false;

            //初期設定
            UpperProxyDefault = new UpperProxy(Gi_Proxy.Properties.Settings.Default.UpperIP, Gi_Proxy.Properties.Settings.Default.UpperPort);
            SettingDefault = new SettingState(UpperProxyDefault,
                Gi_Proxy.Properties.Settings.Default.PassState,
                Gi_Proxy.Properties.Settings.Default.PassSet);

            //ダブルバッファ処理を入れてちらつき防止
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();

            //default初期化
            GraphData = new Dictionary<string, NodeNetwork>();
            GraphData.Add(MainNodeParty, new NodeNetwork());
            BanList = new List<string>();

            //ブロックしているリストをSettingから取得
            foreach (string Dm in Gi_Proxy.Properties.Settings.Default.Banlist.Split('/'))
            {
                if (Dm == "") continue;
                BanList.Add(Dm);
            }

            //ステータスバー表記
            NotifyIcon1.Text=ApplicationName;

            //初期ノード
            GraphData[MainNodeParty].Nodes.InitNode("localhost", 1.0d, 0.0d, 0.0d);
            GraphData[MainNodeParty].Nodes["localhost"].NodeShape.Click += new System.EventHandler(this.NodeShape_Click);
            GraphData[MainNodeParty].Nodes["localhost"].NodeShape.MouseUp += new MouseEventHandler(this.NodeShape_MouseUp);
            GraphData[MainNodeParty].Nodes["localhost"].NodeShape.MouseMove += new MouseEventHandler(this.NodeShape_MouseMove);
            GraphData[MainNodeParty].Nodes["localhost"].NodeShape.ContextMenuStrip = this.contextMenuStrip1;

            //フォームの初期化
            InitForm();

            //ダブルバッファパネルにホイールのイベントを追加する
            this.doubleBufferPanel1.MouseWheel += new MouseEventHandler(this.doubleBufferPanel1_MouseWheel);

            //BBPPsの親
            this.sc.Parent = this.doubleBufferPanel1;

            //ノードの描画を開始
            timer1.Enabled = true;

            //サーバーソケット初期化
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress IP = IPAddress.Parse("127.0.0.1");
            IPEndPoint IPEndPoint = new IPEndPoint(IP, 9000);
            Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            Server.Bind(IPEndPoint);
            Server.Listen(5);

            //クライアントからの受信を開始する
            ThProxy = new Thread(GetProxyRequest);
            ThProxy.Start();
        }

        /// <summary>
        /// フォームのデザインメソッド
        /// </summary>
        private void InitForm()
        {
            //検索用のオブジェクト
            //最初は非表示
            label1.Text = "項目の検索";
            label1.Visible = false;
            textBox1.Visible = false;

            button2.FlatStyle = FlatStyle.Flat;
            button2.BackColor = Color.LightGray;
            button2.Text = "検索";
            button2.Visible = false;
            button2.Font = new Font("メイリオ", 12);

            label1.Font = new Font("メイリオ", 12);

            //doublebufferpanel
            doubleBufferPanel1.Location = new Point(400, 24);
            doubleBufferPanel1.Size = new Size(792, 743);

            //datagridView
            dataGridView1.Visible = false;
            DataGridViewLinkColumn LinkColum = new DataGridViewLinkColumn();
            LinkColum.HeaderText = "URL";
            LinkColum.Name = "URL";
            dataGridView1.Columns.Add(LinkColum);
            dataGridView1.Columns[0].MinimumWidth = 300;
            dataGridView1.Columns[0].Resizable = DataGridViewTriState.False;
            dataGridView1.Columns.Add("HostName", "ホスト名");
            dataGridView1.Columns[1].MinimumWidth = 150;
            dataGridView1.Columns[1].Resizable = DataGridViewTriState.False;
            dataGridView1.Columns.Add("AccessTime", "アクセス日時");
            dataGridView1.Columns[2].MinimumWidth = 150;
            dataGridView1.Columns[2].Resizable = DataGridViewTriState.False;
            DataGridViewButtonColumn ButtonColom = new DataGridViewButtonColumn();
            ButtonColom.HeaderText = "ブロック";
            ButtonColom.Name = "ブロック";
            dataGridView1.Columns.Add(ButtonColom);
            dataGridView1.Columns[3].MinimumWidth = 150;
            dataGridView1.Columns[3].Resizable = DataGridViewTriState.False;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Transparent;
            dataGridView1.DefaultCellStyle.SelectionForeColor = SystemColors.ControlText;
            dataGridView1.Location = new Point(400, 25);
            dataGridView1.Size = new Size(786, 739);

            //text
            this.Text = ApplicationName;

            label2.Text = "表示グラフの選択";
            label2.Font = new Font("メイリオ", 12);

            //コレクション配列初期化
            for (int i = 0; i < 4; i++)
            {
                LineLabel[i] = new Label();
                DataLabel[i] = new Label();
                DataTextBox[i] = new TextBox();
            }

            //境界線をラベルを設定
            LineLabel[0].Location = new Point(20, 290);
            for (int i = 0; i < 3; i++)
            {
                LineLabel[i].Parent = this.panel1;
                LineLabel[i].Width = 350;
                LineLabel[i].Height = 2;
                LineLabel[i].Text = "";
                LineLabel[i].AutoSize = false;
                LineLabel[i].BorderStyle = BorderStyle.Fixed3D;
                if (i == 0) continue;
                LineLabel[i].Location = new Point(LineLabel[i - 1].Location.X, LineLabel[i - 1].Location.Y + 130);
            }

            //データラベルを設定
            DataLabel[0].Location = new Point(35, 200);
            for (int i = 0; i < 3; i++)
            {
                DataLabel[i].Parent = this.panel1;
                DataLabel[i].AutoSize = true;
                DataLabel[i].Font = new Font("メイリオ", 12);
                if (i == 0) continue;
                DataLabel[i].Location = new Point(DataLabel[i - 1].Location.X, DataLabel[i - 1].Location.Y + 130);
            }
            DataLabel[0].Text = "ドメイン";
            DataLabel[1].Text = "最終アクセス日時";
            DataLabel[2].Text = "Color";
            DataLabel[3].Text = "今後追加する予定";

            //データテキストボックスを設定
            DataTextBox[0].Location = new Point(35, 225);
            for (int i = 0; i < 3; i++)
            {
                DataTextBox[i].Parent = this.panel1;
                DataTextBox[i].ReadOnly = true;
                DataTextBox[i].Size = new Size(315, 20);
                if (i == 0) continue;
                DataTextBox[i].Location = new Point(DataTextBox[i - 1].Location.X, DataTextBox[i - 1].Location.Y + 130);
            }

            //GraphViewボタンの設定
            button3.FlatStyle = FlatStyle.Flat;
            button3.BackColor = Color.LightSkyBlue;
            button3.Text = "GraphView";
            button3.Size = new Size(200, 70);
            button3.Location = new Point(0, 2);

            //TableViewボタンの設定
            button4.FlatStyle = FlatStyle.Flat;
            button4.BackColor = Color.LightSteelBlue;
            button4.Text = "TableView";
            button4.Size = new Size(200, 70);
            button4.Location = new Point(200, 2);

            //ツールバーの初期チェック状態
            ToolGraphDrawRough.Checked = true;
            ToolGraphDrawVelvety.Checked = false;
            ToolFilteringOn.Checked = true;
            ToolFilteringOff.Checked = false;
            ToolWindowMax.Checked = true;

            //ドラッグ用のフラグ
            RSFlg = false;
            //グラフの描画感覚
            timer1.Interval = 100;

            //コンボボックス
            comboBox1.FlatStyle = FlatStyle.Standard;
            comboBox1.Items.Add(MainNodeParty);
            comboBox1.SelectedIndex = 0;

            //描画
            this.Main_Form_Paint();
        }

        /// <summary>
        /// クライアントからの要求があった場合にThreadPoolに要求を追加する
        /// </summary>
        public void GetProxyRequest()
        {
            Socket ClientAccess;
            Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 3000);
            ProxyServer Res;
            
            //要求待ち
            while (true)
            {
                try
                {
                    ClientAccess = Server.Accept();
                }
                catch
                {
                    return;
                }
                if (ClientAccess == null) continue;

                //フィルタリングのON,OFFを分ける
                if (FilteringSwitch)
                {
                    Res = new ProxyServer(ClientAccess, new EndRequestDelegateMethod(this.AddNodeInfomation), BanList, UpperProxyDefault);
                }
                else
                {
                    Res = new ProxyServer(ClientAccess, new EndRequestDelegateMethod(this.AddNodeInfomation), new List<string>(), UpperProxyDefault);
                }
                //スレッドプールに追加
                ThreadPool.QueueUserWorkItem(new WaitCallback(Res.Run));
            }
        }

        /// <summary>
        /// スレッドプールの処理用のデリゲート
        /// </summary>
        public void AddNodeInfomation(RequestInfo Req)
        {
            //もし追加されてなかったらメインフォームのイベントにNodeShapeのイベントを追加
            if (GraphData[MainNodeParty].Nodes.AddNode(Req))
            {
                //Keyエラー回避
                try
                {
                    //イベントの追加
                    GraphData[MainNodeParty].Nodes[Req.HostName].NodeShape.Click += new System.EventHandler(this.NodeShape_Click);
                    GraphData[MainNodeParty].Nodes[Req.HostName].NodeShape.MouseUp += new MouseEventHandler(this.NodeShape_MouseUp);
                    GraphData[MainNodeParty].Nodes[Req.HostName].NodeShape.MouseMove += new MouseEventHandler(this.NodeShape_MouseMove);
                    GraphData[MainNodeParty].Nodes[Req.HostName].NodeShape.ContextMenuStrip = contextMenuStrip1;
                }
                catch
                {
                    Console.WriteLine("追加部分のエラー");
                }
                //変化した力の値を変更
                lock (GraphData[MainNodeParty].Nodes)
                {
                    GraphData[MainNodeParty].Nodes[Req.HostName].G = GraphData[MainNodeParty].Nodes["localhost"].G;
                    GraphData[MainNodeParty].Nodes[Req.HostName].L = GraphData[MainNodeParty].Nodes["localhost"].L;
                }
            }
            else
            {
                //最終アクセス日時を更新
                GraphData[MainNodeParty].Nodes[Req.HostName].LastAccessTime = DateTime.Now.ToString();
            }

            //全てのアクセス記録とるので重複OK
            TableItem Ti = new TableItem();
            Ti.SetTableItem(Req.Protocol + "://" + Req.HostName + Req.FileName, Req.HostName, DateTime.Now.ToString());
            lock (GraphData[MainNodeParty].TableItems)
            {
                GraphData[MainNodeParty].TableItems.Add(Ti);
            }
        }

        /// <summary>
        /// ノードをクリック→左のメニューにノードの情報を貼っつけ
        /// </summary>
        /// <param name="sender">ノード名をこっから取り出す</param>
        private void NodeShape_Click(object sender, EventArgs e)
        {
            try
            {
                //ノードのオブジェクトをsenderから取得
                var Shape = (OvalShape)sender;

                //メニューに情報表示
                DataTextBox[0].Text = Shape.Name;
                DataTextBox[1].Text = GraphData[NowSetGraph].Nodes[Shape.Name].LastAccessTime;
                DataTextBox[2].Text = GraphData[NowSetGraph].Nodes[Shape.Name].NodeColor.Color.ToString();
            }
            catch (Exception E)
            {
                MessageBox.Show(E.ToString());
            }
        }

        /// <summary>
        /// nodeをクリックしたときのアップイベント
        /// </summary>
        private void NodeShape_MouseUp(object sender, MouseEventArgs e)
        {
            //ノードをロックしてドラッグできるようにする
            if (e.Button == MouseButtons.Left)
            {
                //ロック解除
                if (NodeDragFlg)
                {
                    NodeDragFlg = false;
                    GraphData[NowSetGraph].Nodes[LockNodeName].LockFlg = false;
                }
                else
                {
                    //ロック
                    var Shape = (OvalShape)sender;
                    Point Senter = new Point(GraphData[NowSetGraph].Nodes[Shape.Name].NodeShape.Height / 2, GraphData[NowSetGraph].Nodes[Shape.Name].NodeShape.Width / 2);
                    NodeDragFlg = true;
                    MouseDownPoint = Senter;
                    GraphData[NowSetGraph].Nodes[Shape.Name].LockFlg = true;
                    var Loca = GraphData[NowSetGraph].Nodes[Shape.Name].NodeShape.Location;
                    GraphData[NowSetGraph].Nodes[Shape.Name].NodeShape.Location = new Point(Loca.X + (Senter.X - e.X), Loca.Y + (Senter.Y - e.Y));
                    LockNodeName = Shape.Name;
                    Shape.Focus();
                }
            }
        }

        /// <summary>
        /// ロック中にマウスを動かしたらノードを動かす
        /// </summary>
        private void NodeShape_MouseMove(object sender, MouseEventArgs e)
        {
            //ノードをロックしているときに動かす
            if (NodeDragFlg)
            {
                //描画設定によって処理の切り替え
                if (RSFlg)
                {
                    GraphData[NowSetGraph].Nodes[LockNodeName].Coordinate.X += e.Location.X - MouseDownPoint.X;
                    GraphData[NowSetGraph].Nodes[LockNodeName].Coordinate.Y += e.Location.Y - MouseDownPoint.Y;
                }
            }
        }

        /// <summary>
        /// 検索ボタンを押したとき
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            //検索結果のみ出力
            string SearchSpelling = textBox1.Text;
            if (SearchSpelling.Length > 0)
            {
                ICollection<TableItem> SearchTableItems = new List<TableItem>();

                lock (GraphData[NowSetGraph].TableItems)
                {
                    foreach (TableItem TI in GraphData[NowSetGraph].TableItems)
                    {
                        if (TI.URL.Contains(SearchSpelling))
                        {
                            SearchTableItems.Add(TI);
                        }
                    }
                }
                //結果を出力
                OutputTableMenu(SearchTableItems);
            }
        }

        /// <summary>
        /// グラフ表示への遷移
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            if (doubleBufferPanel1.Visible == false)
            {
                ChangeModeToGraph();
            }
        }

        /// <summary>
        /// テーブル表示への遷移
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            if (doubleBufferPanel1.Visible == true)
            {
                OutputTableMenu(GraphData[NowSetGraph].TableItems);
                ChangeModeToTable();
            }
        }

        /// <summary>
        /// テーブルビューへ切り替えるメソッド
        /// </summary>
        private void ChangeModeToTable()
        {
            timer1.Enabled = false;
            doubleBufferPanel1.Refresh();
            doubleBufferPanel1.Visible = false;
            
            dataGridView1.Visible = true;

            for (int i = 0; i < 4; i++)
            {
                DataTextBox[i].Text = "";
                DataTextBox[i].Visible = false;
                DataLabel[i].Visible = false;
                LineLabel[i].Visible = false;
            }

            label1.Visible = true;
            textBox1.Visible = true;
            button2.Visible = true;

        }

        /// <summary>
        /// グラフビューへの切り替えるメソッド
        /// </summary>
        private void ChangeModeToGraph()
        {
            dataGridView1.Visible = false;
            doubleBufferPanel1.Visible = true;
            timer1.Enabled = true;

            for (int i = 0; i < 4; i++)
            {
                DataTextBox[i].Text = "";
                DataTextBox[i].Visible = true;
                DataLabel[i].Visible = true;
                LineLabel[i].Visible = true;
            }

            label1.Visible = false;
            textBox1.Visible = false;
            button2.Visible = false;

        }

        /// <summary>
        /// テーブルビューにアイテムを出力する
        /// </summary>
        /// <param name="Ti">出力させたいアイテムのリスト</param>
        private void OutputTableMenu(ICollection<TableItem> Ti)
        {
            NowTableItems = Ti;
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            lock (Ti)
            {
                foreach (TableItem Titem in Ti)
                {
                    this.dataGridView1.Rows.Add();
                    int idx = this.dataGridView1.Rows.Count - 1;
                    this.dataGridView1.Rows[idx].Cells[0].Value = Titem.URL;
                    this.dataGridView1.Rows[idx].Cells[1].Value = Titem.Domain;
                    this.dataGridView1.Rows[idx].Cells[2].Value = Titem.AccessTime;
                    if (BanList.Contains(Titem.Domain) || BanList.Contains("localhost"))
                    {
                        this.dataGridView1.Rows[idx].Cells[3].Value = "ブロック解除";
                    }
                    else
                    {
                        this.dataGridView1.Rows[idx].Cells[3].Value = "ブロック";
                    }
                    dataGridView1.Rows[idx].Resizable = DataGridViewTriState.False;
                }
            }
        }

        /// <summary>
        /// グリッドビューのセルをクリックしてリンクだった場合にブラウザから開く
        /// </summary>
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView Dgv = (DataGridView)sender;
            //"Link"列がクリックされた
            if (Dgv.Columns[e.ColumnIndex].Name == "URL")
            {
                try
                {
                    System.Diagnostics.Process.Start(Dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                }
                catch
                {
                    MessageBox.Show("このURLは開くことができません。","エラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            //ブロック列がクリックされた
            else if (Dgv.Columns[e.ColumnIndex].Name == "ブロック")
            {
                //機能制限
                if (SettingDefault.PassStatus == true && SettingDefault.PassSet == false)
                {
                    MessageBox.Show("操作権限がありません。設定->機能制限より認証をしてください。", "エラー",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                /*ブロック*/
                string BanNodeName;
                try
                {
                    BanNodeName = Dgv.Rows[e.RowIndex].Cells[1].Value.ToString();

                    lock (BanList)
                    {
                        if (!BanList.Contains(BanNodeName))
                        {
                            //BANした
                            BanList.Add(BanNodeName);
                            Dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "ブロック解除";
                        }
                        else
                        {
                            //BAN解除
                            BanList.Remove(BanNodeName);
                            Dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "ブロック";
                        }
                    }

                    if (Dgv.Rows[e.RowIndex].Cells[0].Value.ToString() == "NoData")
                    {
                        ViewBlockListMethod();
                    }
                    else
                    {
                        OutputTableMenu(NowTableItems);
                    }
                }
                catch
                {
                    return;
                }
            }
        }

        /// <summary>
        /// フォームにイベントを持った円と線を描画するメソッド
        /// </summary>
        private void Main_Form_Paint()
        {
            //排他処理
            lock (GraphData[NowSetGraph].Nodes)
            {
                //座標設定
                foreach (Node n in GraphData[NowSetGraph].Nodes.Values)
                {
                    int rr = R * Convert.ToInt32(n.NodeMass);
                    n.NodeShape.Parent = this.sc;
                    n.NodeShape.Location = new Point(ToScreenX(n.Coordinate.X) - rr, ToScreenY(n.Coordinate.Y) - rr);
                    n.NodeShape.Size = new Size(2 * rr, 2 * rr);
                    n.NodeShape.BorderColor = Color.Transparent;
                }
            }
        }

        /// <summary>
        /// パネルにオブジェクトを重ねてグラフィックスクラスで描画メソッド
        /// </summary>
        private void doubleBufferPanel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.LightBlue);
            lock (GraphData[NowSetGraph].Nodes)
            {
                foreach (Node n in GraphData[NowSetGraph].Nodes.Values)
                {
                    foreach (Node nn in n.LinkNode)
                    {
                        //線の描画
                        e.Graphics.DrawLine(Pens.Black, ToScreenX(n.Coordinate.X), ToScreenY(n.Coordinate.Y), ToScreenX(nn.Coordinate.X), ToScreenY(nn.Coordinate.Y));
                    }
                }
                foreach (Node n in GraphData[NowSetGraph].Nodes.Values)
                {
                    /*枠も必要なので丸を二つ重ねる*/

                    int Radius = R * Convert.ToInt32(n.NodeMass) - 3;
                    int BackRadius = R * Convert.ToInt32(n.NodeMass);
                    SolidBrush CircleColor = GraphData[NowSetGraph].Nodes[n.NodeName].NodeColor;
                    Brush BackCircleColor;
                    if (n.LockFlg) BackCircleColor = Brushes.LightCyan;
                    else if (BanList.Contains(n.NodeName)) BackCircleColor = Brushes.Red;
                    else BackCircleColor = Brushes.Black;

                    // ノードは、n.R を中心とする半径 r の円で表す
                    e.Graphics.FillEllipse(BackCircleColor, ToScreenX(n.Coordinate.X) - BackRadius, ToScreenY(n.Coordinate.Y) - BackRadius, 2 * BackRadius, 2 * BackRadius);
                    e.Graphics.FillEllipse(CircleColor, ToScreenX(n.Coordinate.X) - Radius, ToScreenY(n.Coordinate.Y) - Radius, 2 * Radius, 2 * Radius);
                }

                foreach (Node n in GraphData[NowSetGraph].Nodes.Values)
                {
                    //ドメイン名描画
                    int Radius = R * Convert.ToInt32(n.NodeMass) - 3;
                    e.Graphics.DrawString(n.NodeName, SystemFonts.DefaultFont, Brushes.Black, ToScreenX(n.Coordinate.X) - R / 2 + R, ToScreenY(n.Coordinate.Y) - R / 2 - R);
                }
            }
        }

        /// <summary>
        /// フォーム上の座標に相対座標を置き換える
        /// </summary>
        private int ToScreenX(double x)
        {
            return (int)x + this.doubleBufferPanel1.Width / 2;
        }

        /// <summary>
        /// フォーム上の座標に相対座座標を置き換える
        /// </summary>
        private int ToScreenY(double y)
        {
            return (int)y + this.doubleBufferPanel1.Height / 2;
        }

        /// <summary>
        /// フォーム上の座標に相対座標を置き換える
        /// </summary>
        private double ToNodeX(int x)
        {
            return (double)x - this.doubleBufferPanel1.Width / 2.0d;
        }

        /// <summary>
        /// フォーム上の座標に相対座座標を置き換える
        /// </summary>
        private double ToNodeY(int y)
        {
            return (double)y - this.doubleBufferPanel1.Height / 2.0d;
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        private void timer1_Tick(object sender, EventArgs e)
        {
            //座標計算・再描画
            this.Refresh();
            GraphData[NowSetGraph].Nodes.MoveAll();
            this.Main_Form_Paint();
        }

        /// <summary>
        /// メインフォームを閉じようとするとき
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //フォームの終了を押してこのイベントに入った場合にのみ終了させる
                if (!WhichCloseButton)
                {
                    this.Hide();
                    e.Cancel = true;
                    return;
                }
                WhichCloseButton = false;

                //ブロックリストの保存
                string SaveBanlistStr = "";
                foreach (string name in BanList)
                {
                    SaveBanlistStr += name + '/';
                }
                Gi_Proxy.Properties.Settings.Default.Banlist = SaveBanlistStr;
                Gi_Proxy.Properties.Settings.Default.Save();

                //操作権限があるか判断
                if (SettingDefault.PassStatus == true && SettingDefault.PassSet == false)
                {
                    //権限が無い場合は消させない
                    MessageBox.Show("操作権限がありません。設定->機能制限より認証をしてください。", "エラー",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }

                //終了時にはグラフの情報をすべて保存する
                try
                {
                    //保存先のローカルアドレス
                    string LocalSaveAdress;

                    //マイドキュメントを指定WW
                    LocalSaveAdress = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    string NowTime = DateTime.Now.ToString();
                    NowTime = ((NowTime.Replace(" ", "")).Replace("/", "")).Replace(":", "");
                    LocalSaveAdress += @"\Gi-ProxysSavedGraph\" + NowTime;

                    DirectoryInfo di = Directory.CreateDirectory(LocalSaveAdress);
                    di.Create();

                    IOXML write = new IOXML();
                    write.writeData(GraphData[MainNodeParty], @LocalSaveAdress);
                }
                catch
                {
                    return;
                }

            }
            catch (Exception E)
            {
                MessageBox.Show(E.ToString(), "Closingイベントエラー");
            }
        }

        /// <summary>
        /// メインフォームを閉じるとき
        /// </summary>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Server.Dispose();
            Server.Close();
        }

        /// <summary>
        /// パネルをクリックしたらドラッグによるイベントの制御フラグをtrueにする
        /// </summary>
        private void doubleBufferPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            //中央のボタンでドラッグするための初期処理
            doubleBufferPanel1.Focus();
            if (e.Button == MouseButtons.Middle)
            {
                DraggFlg = true;
                MouseDownPoint = e.Location;
                doubleBufferPanel1.Focus();
            }
        }

        /// <summary>
        /// パネルをクリック状態から開放したらドラッグによるイベントの制御フラグをfalseにする
        /// </summary>
        private void doubleBufferPanel1_MouseUp(object sender, MouseEventArgs e)
        {
            //マウスボタンを上げたらドラッグの動かすイベント終了
            if (e.Button == MouseButtons.Middle)
            {
                DraggFlg = false;
            }
        }

        /// <summary>
        /// マウスダウン中にドラッグしたらグラフのノードをすべて移動させる
        /// </summary>
        private void doubleBufferPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            //ノードをドラッグにより移動はこっち
            if (NodeDragFlg)
            {
                GraphData[NowSetGraph].Nodes[LockNodeName].Coordinate.X = ToNodeX(e.Location.X);
                GraphData[NowSetGraph].Nodes[LockNodeName].Coordinate.Y = ToNodeY(e.Location.Y);
            }

            //グラフ全体を移動はこっち
            if (DraggFlg)
            {
                double CoordinateMoveX = e.Location.X - MouseDownPoint.X;
                double CoordinateMoveY = e.Location.Y - MouseDownPoint.Y;
                lock (GraphData[NowSetGraph].Nodes)
                {
                    foreach (Node n in GraphData[NowSetGraph].Nodes.Values)
                    {
                        n.Coordinate.X += CoordinateMoveX;
                        n.Coordinate.Y += CoordinateMoveY;
                    }
                }
                MouseDownPoint = e.Location;
            }
        }

        /// <summary>
        /// パネル上でホイールを移動させたらズーム,アンズーム
        /// </summary>
        private void doubleBufferPanel1_MouseWheel(object sender, MouseEventArgs e)
        {
            //ズームダウン
            if (NodeDragFlg) return;
            int ChangeWheel = (e.Delta * SystemInformation.MouseWheelScrollLines / 120);
            if (ChangeWheel < InitWheel)
            {
                if (R > MinR)
                {
                    R--;
                    lock (GraphData[NowSetGraph].Nodes)
                    {
                        foreach (Node n in GraphData[NowSetGraph].Nodes.Values)
                        {
                            n.L -= RateOfChange;
                            n.G -= RateOfChange * 10;
                        }
                    }
                }
            }
            //ズームアップ
            else
            {
                if (R < MaxR)
                {
                    R++;
                    lock (GraphData[NowSetGraph].Nodes)
                    {
                        foreach (Node n in GraphData[NowSetGraph].Nodes.Values)
                        {
                            n.L += RateOfChange;
                            n.G += RateOfChange * 10;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// コンボボックスの選択を変えたときに選択グラフも変更する
        /// </summary>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //現在描画されているグラフを消す
            lock (GraphData[NowSetGraph])
            {
                foreach(Node n in GraphData[NowSetGraph].Nodes.Values)
                {
                    n.NodeShape.Enabled = false;
                }
            }
            NowSetGraph = comboBox1.SelectedItem.ToString();
            //新しくグラフを描画
            lock (GraphData[NowSetGraph])
            {
                foreach (Node n in GraphData[NowSetGraph].Nodes.Values)
                {
                    n.NodeShape.Enabled = true;
                }
            }
        }

        /// <summary>
        /// タスクトレイのアイコンのダブルクリックイベント
        /// </summary>
        private void SoftName_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //見える見えないの切り替え
            if (this.Visible == false)
            {
                this.Show();
            }
            else
            {
                this.Hide();
            }
        }

        #region toolBar

        /// <summary>
        /// フォームを閉じる
        /// </summary>
        private void FormClose_Click(object sender, EventArgs e)
        {
            //このフラグをtrueにしなければタスクバーに隠れてしまう
            WhichCloseButton = true;
            this.Close();
        }

        /// <summary>
        /// グラフ描画→なめらか描画を押したとき
        /// </summary>
        private void GraphDrawvelvety_Click(object sender, EventArgs e)
        {
            ToolGraphDrawRough.Checked = false;
            ToolGraphDrawVelvety.Checked = true;
            //ノード上でのイベント処理のフラグ
            RSFlg = true;
            //タイマーの間隔を短く
            timer1.Interval = 1;
        }

        /// <summary>
        /// グラフ描画→一括描画を押した時
        /// </summary>
        private void GraphDrawBatch_Click(object sender, EventArgs e)
        {
            ToolGraphDrawRough.Checked = true;
            ToolGraphDrawVelvety.Checked = false;
            //ノード上でのイベント処理のフラグ
            RSFlg = false;
            //タイマーの間隔を長く
            timer1.Interval = 100;
        }

        /// <summary>
        /// 960×640にウィンドウサイズを変更
        /// </summary>
        private void ToolWindowMin_Click(object sender, EventArgs e)
        {
            ToolWindowMin.Checked = true;
            ToolWindowMax.Checked = false;
            dataGridView1.Size = new Size(546, 579);
            doubleBufferPanel1.Size = new Size(552, 582);
            this.MinimumSize = new Size(960, 640);
            this.MaximumSize = new Size(960, 640);
        }

        /// <summary>
        /// 1200×800にウィンドウサイズを変更
        /// </summary>
        private void ToolWindowMax_Click(object sender, EventArgs e)
        {
            ToolWindowMin.Checked = false;
            ToolWindowMax.Checked = true;
            dataGridView1.Size = new Size(786, 739);
            doubleBufferPanel1.Size = new Size(792, 743);
            this.MinimumSize = new Size(1200, 800);
            this.MaximumSize = new Size(1200, 800);
        }

        /// <summary>
        /// フィルタリングのON
        /// </summary>
        private void ToolFilteringOn_Click(object sender, EventArgs e)
        {
            //機能制限
            if (SettingDefault.PassStatus == true && SettingDefault.PassSet == false)
            {
                MessageBox.Show("操作権限がありません。設定->機能制限より認証をしてください。", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ToolFilteringOn.Checked = true;
            ToolFilteringOff.Checked = false;
            //受信部の切り替えフラグ変更
            FilteringSwitch = true;
        }

        /// <summary>
        /// フィルタリングのOFF
        /// </summary>
        private void ToolFilteringOff_Click(object sender, EventArgs e)
        {
            //誤って操作しないように警告
            if (ToolFilteringOff.Checked == false)
            {
                var Reselt = MessageBox.Show("この操作を実行すると全てのドメインへののブロック状態が一時的に解除されます。", "警告",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);

                if (Reselt == DialogResult.Cancel)
                {
                    return;
                }
            }

            //機能制限
            if (SettingDefault.PassStatus == true && SettingDefault.PassSet == false)
            {
                MessageBox.Show("操作権限がありません。設定->機能制限より認証をしてください。", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ToolFilteringOff.Checked = true;
            ToolFilteringOn.Checked = false;
            //受信部の切り替えフラグ変更
            FilteringSwitch = false;
        }

        /// <summary>
        /// 設定画面設定
        /// </summary>
        private void Setting_Click(object sender, EventArgs e)
        {
            //設定フォームを開く
            SettingForm Sf = new SettingForm(SettingDefault);
            this.Enabled = false;
            Sf.ShowDialog();
            this.Enabled = true;

            //インスタンスの決定されたメンバを適用
            SettingDefault = Sf.ChangeSetting;
        }

        /// <summary>
        /// 名前を付けて保存をクリック
        /// </summary>
        private void SaveNameAndGraph_Click(object sender, EventArgs e)
        {
            //保存先のローカルアドレス
            string LocalSaveAdress;

            try
            {
                //マイドキュメント
                LocalSaveAdress = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string NowTime = DateTime.Now.ToString();
                NowTime = ((NowTime.Replace(" ", "")).Replace("/", "")).Replace(":", "");
                LocalSaveAdress += @"\Gi-ProxysSavedGraph\" + NowTime;

                //ディレクトリが存在しなかった場合に作成
                DirectoryInfo di = Directory.CreateDirectory(LocalSaveAdress);
                di.Create();
                //System.Diagnostics.Process.Start(LocalSaveAdress);

                //グラフとテーブルの保存
                IOXML write = new IOXML();
                write.writeData(GraphData[MainNodeParty], @LocalSaveAdress);
            }
            catch
            {
                MessageBox.Show("保存に失敗しました。");
                return;
            }
            MessageBox.Show("保存に成功しました。");
        }
        
        /// <summary>
        /// グラフを開くをクリック
        /// </summary>
        private void OpenGraphSelect_Click(object sender, EventArgs e)
        {
            //保存先のローカルアドレス
            string LocalSaveAdress = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal) +@"\SavedGraph\";

            //FolderBrowserDialogクラスのインスタンスを作成
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            //上部に表示する説明テキストを指定する
            fbd.Description = "フォルダを指定してください。";

            //フォルダが存在しなかったらの場合に想定して作成
            DirectoryInfo di = Directory.CreateDirectory(LocalSaveAdress);
            di.Create();

            //最初に選択するフォルダを指定する
            fbd.SelectedPath = LocalSaveAdress;

            //ダイアログを表示する
            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                //選択されたフォルダを表示する
                LocalSaveAdress = fbd.SelectedPath;
            }
            else
            {
                return;
            }

            int IndexLength = LocalSaveAdress.Split('\\').Length;
            string AddName = LocalSaveAdress.Split('\\')[IndexLength - 1];

            if (GraphData.ContainsKey(AddName))
            {
                MessageBox.Show("重複したグラフは開けません");
                return;
            }

            //グラフの読み込み
            IOXML read = new IOXML();
            GraphData.Add(AddName, read.readData(LocalSaveAdress));
            comboBox1.Items.Add(AddName);

            //イベント追加や演算に使用する力の適用sss
            lock (GraphData[AddName].Nodes)
            {
                foreach (Node n in GraphData[AddName].Nodes.Values)
                {
                    n.NodeShape.Click += new System.EventHandler(this.NodeShape_Click);
                    n.NodeShape.MouseUp += new MouseEventHandler(this.NodeShape_MouseUp);
                    n.NodeShape.MouseMove += new MouseEventHandler(this.NodeShape_MouseMove);
                    n.NodeShape.ContextMenuStrip = contextMenuStrip1;

                    n.L += (R - 15) * RateOfChange;
                    n.G += ((R - 15) * RateOfChange) * 10;
                }
            }
        }

        /// <summary>
        /// ブロックリストの表示
        /// </summary>
        /// 汎用性あるのでメソッド化
        private void ViewBlockListMethod()
        {
            dataGridView1.Rows.Clear();
            lock (BanList)
            {
                foreach (string h in BanList)
                {
                    this.dataGridView1.Rows.Add();
                    int idx = this.dataGridView1.Rows.Count - 1;
                    this.dataGridView1.Rows[idx].Cells[0].Value = "NoData";
                    this.dataGridView1.Rows[idx].Cells[1].Value = h;
                    this.dataGridView1.Rows[idx].Cells[2].Value = "NoData";
                    if (BanList.Contains(h) || BanList.Contains("localhost"))
                    {
                        this.dataGridView1.Rows[idx].Cells[3].Value = "ブロック解除";
                    }
                    else
                    {
                        this.dataGridView1.Rows[idx].Cells[3].Value = "ブロック";
                    }
                    dataGridView1.Rows[idx].Resizable = DataGridViewTriState.False;
                }
            }
        }

        /// <summary>
        /// ブロックリストの表示
        /// </summary>
        private void ViewBlockList_Click(object sender, EventArgs e)
        {
            ChangeModeToTable();
            ViewBlockListMethod();
        }

        #endregion

        #region ContexMenuStrip

        /// <summary>
        /// 右クリック→ブロックの処理
        /// </summary>
        private void BlockToolStripMenu_Click(object sender, EventArgs e)
        {
            //機能制限
            if (SettingDefault.PassStatus == true && SettingDefault.PassSet == false)
            {
                MessageBox.Show("操作権限がありません。設定->機能制限より認証をしてください。", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            //選択されたドメインをブロック
            string BanNodeName = DataTextBox[0].Text;
            lock (BanList)
            {
                if (!BanList.Contains(BanNodeName))
                {
                    //localhostをブロックしたときにすべてのアクセスをブロックする仕様の注意
                    if (BanNodeName == "localhost" && !BanList.Contains(BanNodeName))
                    {
                        MessageBox.Show("このノードをブロックするとノードのブロック状態に関係なく全てのネットワーク接続がブロックされます。", "注意",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                    }
                    //BANした
                    BanList.Add(BanNodeName);
                }
            }
        }

        /// <summary>
        /// 右クリック→アンブロックの処理
        /// </summary>
        private void AnBlockToolStripMenu_Click(object sender, EventArgs e)
        {
            //機能制限
            if (SettingDefault.PassStatus == true && SettingDefault.PassSet == false)
            {
                MessageBox.Show("操作権限がありません。設定->機能制限より認証をしてください。", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //ブロック解除
            string BanNodeName = DataTextBox[0].Text;
            lock (BanList)
            {
                if (BanList.Contains(BanNodeName))
                {
                    BanList.Remove(BanNodeName);
                }
            }
        }

        /// <summary>
        /// 右クリック→アクセスしたURL一覧を表示する処理
        /// </summary>
        private void AccessToolStripMenu_Click(object sender, EventArgs e)
        {
            //ドメインのすべてのアクセス履歴を検索して表示する
            string SearchSpelling = DataTextBox[0].Text;
            if (SearchSpelling.Length > 0)
            {
                ICollection<TableItem> SearchTableItems = new List<TableItem>();

                lock (GraphData[NowSetGraph].TableItems)
                {
                    foreach (TableItem TI in GraphData[NowSetGraph].TableItems)
                    {
                        if (TI.URL.Contains(SearchSpelling))
                        {
                            SearchTableItems.Add(TI);
                        }
                    }
                }
                OutputTableMenu(SearchTableItems);
            }
            ChangeModeToTable();
        }

        #endregion

    }
}