namespace Gi_Proxy
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ファイルToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenGraphSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveNameAndGraph = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.FormClose = new System.Windows.Forms.ToolStripMenuItem();
            this.フィルタリングToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolFilteringOn = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolFilteringOff = new System.Windows.Forms.ToolStripMenuItem();
            this.ウィンドウモードToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolWindowMin = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolWindowMax = new System.Windows.Forms.ToolStripMenuItem();
            this.グラフ描画ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolGraphDrawVelvety = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolGraphDrawRough = new System.Windows.Forms.ToolStripMenuItem();
            this.ツールTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewBlockList = new System.Windows.Forms.ToolStripMenuItem();
            this.Setting = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.BlockToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.AnBlockToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.AccessToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.閉じるToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NotifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.doubleBufferPanel1 = new Gi_Proxy.DoubleBufferPanel();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ファイルToolStripMenuItem,
            this.フィルタリングToolStripMenuItem,
            this.ウィンドウモードToolStripMenuItem,
            this.グラフ描画ToolStripMenuItem,
            this.ツールTToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1184, 26);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ファイルToolStripMenuItem
            // 
            this.ファイルToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenGraphSelect,
            this.SaveNameAndGraph,
            this.toolStripSeparator1,
            this.FormClose});
            this.ファイルToolStripMenuItem.Name = "ファイルToolStripMenuItem";
            this.ファイルToolStripMenuItem.Size = new System.Drawing.Size(85, 22);
            this.ファイルToolStripMenuItem.Text = "ファイル(&F)";
            // 
            // OpenGraphSelect
            // 
            this.OpenGraphSelect.Name = "OpenGraphSelect";
            this.OpenGraphSelect.Size = new System.Drawing.Size(167, 22);
            this.OpenGraphSelect.Text = "グラフを開く(&O)";
            this.OpenGraphSelect.Click += new System.EventHandler(this.OpenGraphSelect_Click);
            // 
            // SaveNameAndGraph
            // 
            this.SaveNameAndGraph.Name = "SaveNameAndGraph";
            this.SaveNameAndGraph.Size = new System.Drawing.Size(167, 22);
            this.SaveNameAndGraph.Text = "グラフを保存(&S)";
            this.SaveNameAndGraph.Click += new System.EventHandler(this.SaveNameAndGraph_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(164, 6);
            // 
            // FormClose
            // 
            this.FormClose.Name = "FormClose";
            this.FormClose.Size = new System.Drawing.Size(167, 22);
            this.FormClose.Text = "終了(&X)";
            this.FormClose.Click += new System.EventHandler(this.FormClose_Click);
            // 
            // フィルタリングToolStripMenuItem
            // 
            this.フィルタリングToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolFilteringOn,
            this.ToolFilteringOff});
            this.フィルタリングToolStripMenuItem.Name = "フィルタリングToolStripMenuItem";
            this.フィルタリングToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.フィルタリングToolStripMenuItem.Text = "フィルタリング(&B)";
            // 
            // ToolFilteringOn
            // 
            this.ToolFilteringOn.Name = "ToolFilteringOn";
            this.ToolFilteringOn.Size = new System.Drawing.Size(116, 22);
            this.ToolFilteringOn.Text = "ON(&T)";
            this.ToolFilteringOn.Click += new System.EventHandler(this.ToolFilteringOn_Click);
            // 
            // ToolFilteringOff
            // 
            this.ToolFilteringOff.Name = "ToolFilteringOff";
            this.ToolFilteringOff.Size = new System.Drawing.Size(116, 22);
            this.ToolFilteringOff.Text = "OFF(&F)";
            this.ToolFilteringOff.Click += new System.EventHandler(this.ToolFilteringOff_Click);
            // 
            // ウィンドウモードToolStripMenuItem
            // 
            this.ウィンドウモードToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolWindowMin,
            this.ToolWindowMax});
            this.ウィンドウモードToolStripMenuItem.Name = "ウィンドウモードToolStripMenuItem";
            this.ウィンドウモードToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.ウィンドウモードToolStripMenuItem.Text = "ウィンドウサイズ(&W)";
            // 
            // ToolWindowMin
            // 
            this.ToolWindowMin.Name = "ToolWindowMin";
            this.ToolWindowMin.Size = new System.Drawing.Size(135, 22);
            this.ToolWindowMin.Text = "960×640";
            this.ToolWindowMin.Click += new System.EventHandler(this.ToolWindowMin_Click);
            // 
            // ToolWindowMax
            // 
            this.ToolWindowMax.Name = "ToolWindowMax";
            this.ToolWindowMax.Size = new System.Drawing.Size(135, 22);
            this.ToolWindowMax.Text = "1200×800";
            this.ToolWindowMax.Click += new System.EventHandler(this.ToolWindowMax_Click);
            // 
            // グラフ描画ToolStripMenuItem
            // 
            this.グラフ描画ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolGraphDrawVelvety,
            this.ToolGraphDrawRough});
            this.グラフ描画ToolStripMenuItem.Name = "グラフ描画ToolStripMenuItem";
            this.グラフ描画ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.グラフ描画ToolStripMenuItem.Text = "グラフ描画(&M)";
            // 
            // ToolGraphDrawVelvety
            // 
            this.ToolGraphDrawVelvety.Name = "ToolGraphDrawVelvety";
            this.ToolGraphDrawVelvety.Size = new System.Drawing.Size(161, 22);
            this.ToolGraphDrawVelvety.Text = "Velvety描画(&S)";
            this.ToolGraphDrawVelvety.Click += new System.EventHandler(this.GraphDrawvelvety_Click);
            // 
            // ToolGraphDrawRough
            // 
            this.ToolGraphDrawRough.Name = "ToolGraphDrawRough";
            this.ToolGraphDrawRough.Size = new System.Drawing.Size(161, 22);
            this.ToolGraphDrawRough.Text = "Rough描画(&A)";
            this.ToolGraphDrawRough.Click += new System.EventHandler(this.GraphDrawBatch_Click);
            // 
            // ツールTToolStripMenuItem
            // 
            this.ツールTToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ViewBlockList,
            this.Setting});
            this.ツールTToolStripMenuItem.Name = "ツールTToolStripMenuItem";
            this.ツールTToolStripMenuItem.Size = new System.Drawing.Size(74, 22);
            this.ツールTToolStripMenuItem.Text = "ツール(&T)";
            // 
            // ViewBlockList
            // 
            this.ViewBlockList.Name = "ViewBlockList";
            this.ViewBlockList.Size = new System.Drawing.Size(196, 22);
            this.ViewBlockList.Text = "ブロックリストの表示";
            this.ViewBlockList.Click += new System.EventHandler(this.ViewBlockList_Click);
            // 
            // Setting
            // 
            this.Setting.Name = "Setting";
            this.Setting.Size = new System.Drawing.Size(196, 22);
            this.Setting.Text = "設定(&S)";
            this.Setting.Click += new System.EventHandler(this.Setting_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Menu;
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(400, 743);
            this.panel1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "label2";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(36, 144);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(315, 20);
            this.comboBox1.TabIndex = 9;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(36, 254);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(315, 19);
            this.textBox1.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 220);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "label1";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(36, 289);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(315, 82);
            this.button2.TabIndex = 6;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(193, 5);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(207, 69);
            this.button4.TabIndex = 5;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(0, 5);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(196, 69);
            this.button3.TabIndex = 4;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(501, 680);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.Size = new System.Drawing.Size(791, 743);
            this.dataGridView1.TabIndex = 3;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BlockToolStripMenu,
            this.AnBlockToolStripMenu,
            this.toolStripSeparator2,
            this.AccessToolStripMenu,
            this.toolStripSeparator3,
            this.閉じるToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(329, 104);
            // 
            // BlockToolStripMenu
            // 
            this.BlockToolStripMenu.Name = "BlockToolStripMenu";
            this.BlockToolStripMenu.Size = new System.Drawing.Size(328, 22);
            this.BlockToolStripMenu.Text = "ブロック(&B)";
            this.BlockToolStripMenu.Click += new System.EventHandler(this.BlockToolStripMenu_Click);
            // 
            // AnBlockToolStripMenu
            // 
            this.AnBlockToolStripMenu.Name = "AnBlockToolStripMenu";
            this.AnBlockToolStripMenu.Size = new System.Drawing.Size(328, 22);
            this.AnBlockToolStripMenu.Text = "ブロック解除(&A)";
            this.AnBlockToolStripMenu.Click += new System.EventHandler(this.AnBlockToolStripMenu_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(325, 6);
            // 
            // AccessToolStripMenu
            // 
            this.AccessToolStripMenu.Name = "AccessToolStripMenu";
            this.AccessToolStripMenu.Size = new System.Drawing.Size(328, 22);
            this.AccessToolStripMenu.Text = "このドメインのアクセスしたページを出力する";
            this.AccessToolStripMenu.Click += new System.EventHandler(this.AccessToolStripMenu_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(325, 6);
            // 
            // 閉じるToolStripMenuItem
            // 
            this.閉じるToolStripMenuItem.Name = "閉じるToolStripMenuItem";
            this.閉じるToolStripMenuItem.Size = new System.Drawing.Size(328, 22);
            this.閉じるToolStripMenuItem.Text = "閉じる";
            // 
            // NotifyIcon1
            // 
            this.NotifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("NotifyIcon1.Icon")));
            this.NotifyIcon1.Text = "DebagMode";
            this.NotifyIcon1.Visible = true;
            this.NotifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.SoftName_MouseDoubleClick);
            // 
            // doubleBufferPanel1
            // 
            this.doubleBufferPanel1.Location = new System.Drawing.Point(400, 24);
            this.doubleBufferPanel1.Name = "doubleBufferPanel1";
            this.doubleBufferPanel1.Size = new System.Drawing.Size(552, 582);
            this.doubleBufferPanel1.TabIndex = 2;
            this.doubleBufferPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.doubleBufferPanel1_Paint);
            this.doubleBufferPanel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.doubleBufferPanel1_MouseDown);
            this.doubleBufferPanel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.doubleBufferPanel1_MouseMove);
            this.doubleBufferPanel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.doubleBufferPanel1_MouseUp);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 762);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.doubleBufferPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1200, 800);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1200, 800);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Panel panel1;
        private DoubleBufferPanel doubleBufferPanel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ToolStripMenuItem ファイルToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenGraphSelect;
        private System.Windows.Forms.ToolStripMenuItem SaveNameAndGraph;
        private System.Windows.Forms.ToolStripMenuItem フィルタリングToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolFilteringOn;
        private System.Windows.Forms.ToolStripMenuItem ToolFilteringOff;
        private System.Windows.Forms.ToolStripMenuItem グラフ描画ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolGraphDrawVelvety;
        private System.Windows.Forms.ToolStripMenuItem ToolGraphDrawRough;
        private System.Windows.Forms.ToolStripMenuItem ツールTToolStripMenuItem;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ToolStripMenuItem Setting;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem BlockToolStripMenu;
        private System.Windows.Forms.ToolStripMenuItem AnBlockToolStripMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem AccessToolStripMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem 閉じるToolStripMenuItem;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem ViewBlockList;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem FormClose;
        private System.Windows.Forms.NotifyIcon NotifyIcon1;
        private System.Windows.Forms.ToolStripMenuItem ウィンドウモードToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolWindowMin;
        private System.Windows.Forms.ToolStripMenuItem ToolWindowMax;
    }
}

