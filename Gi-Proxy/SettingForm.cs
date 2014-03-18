using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Gi_Proxy
{
    /// <summary>
    /// 設定画面のフォーム
    /// </summary>
    public partial class SettingForm : Form
    {
        public SettingState ChangeSetting;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SettingForm(SettingState St)
        {
            InitializeComponent();

            tabPage1.Text = "上位プロキシ";
            tabPage3.Text = "機能制限";

            ChangeSetting = St;

            /*設定状況でフォームの変化*/

            if (St.Ups.IPAdress.Length == 0)
            {
                label1.Enabled = false;
                label2.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                checkBox1.Checked = false;
            }
            else
            {
                checkBox1.Checked = true;
                textBox1.Text = St.Ups.IPAdress;
                textBox2.Text = St.Ups.Port.ToString();
            }

            if (St.PassStatus)
            {
                if (St.PassSet)
                {
                    label10.Text = "認証中";
                }
                else
                {
                    label10.Text = "未認証";
                    checkBox1.Enabled = false;
                    checkBox3.Enabled = false;
                }
                checkBox3.Checked = true;
            }
            else
            {
                label10.Text = "パスコード未設定";
                label9.Enabled = false;
                label10.Enabled = false;
                label11.Enabled = false;
                textBox3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
            }

            /*ここまでデザイン*/
        }

        /// <summary>
        /// 設定内容を全て保存する
        /// </summary>
        private void SaveAll()
        {
            //機能制限
            if (ChangeSetting.PassStatus == true && ChangeSetting.PassSet == false)
            {
                MessageBox.Show("操作権限がありません。設定->機能制限より認証をしてください。", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //上位プロキシの保存部
            if (checkBox1.Checked == false)
            {
                ChangeSetting.Ups.IPAdress = "";
                ChangeSetting.Ups.Port = 0;
            }
            else
            {
                try
                {
                    ChangeSetting.Ups.IPAdress = textBox1.Text;
                    ChangeSetting.Ups.Port = Convert.ToInt32(textBox2.Text);
                    Gi_Proxy.Properties.Settings.Default.UpperIP = ChangeSetting.Ups.IPAdress;
                    Gi_Proxy.Properties.Settings.Default.UpperPort = ChangeSetting.Ups.Port;
                }
                catch
                {
                    MessageBox.Show("上位プロキシの設定が正しく入力されていない場合があります。再度見なおしてください", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ChangeSetting.Ups.IPAdress = "";
                    ChangeSetting.Ups.Port = 0;
                    Gi_Proxy.Properties.Settings.Default.UpperIP = ChangeSetting.Ups.IPAdress;
                    Gi_Proxy.Properties.Settings.Default.UpperPort = ChangeSetting.Ups.Port;
                }
            }
        }

        /// <summary>
        /// OK
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            SaveAll();
            this.Close();
        }

        /// <summary>
        /// cansel
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// apply
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            SaveAll();
        }

        /// <summary>
        /// 上位プロキシの設定切り替え
        /// </summary>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                label1.Enabled = true;
                label2.Enabled = true;
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                checkBox1.Checked = true;
            }
            else
            {
                label1.Enabled = false;
                label2.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                checkBox1.Checked = false;
            }
        }

        /// <summary>
        /// 機能制限の設定切り替え
        /// </summary>
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                label9.Enabled = true;
                label10.Enabled = true;
                label11.Enabled = true;
                textBox3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
            }
            else
            {
                Gi_Proxy.Properties.Settings.Default.EncryptPass = "";
                Gi_Proxy.Properties.Settings.Default.Save();
                ChangeSetting.PassStatus = false;
                label10.Text = "パスコード未設定";
                label9.Enabled = false;
                label10.Enabled = false;
                label11.Enabled = false;
                textBox3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
            }
        }

        /// <summary>
        /// 認証ボタン&設定ボタン
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                MessageBox.Show("パスコードを入力してください。");
            }
            else
            {
                if (ChangeSetting.PassStatus)
                {
                    var f = ChangeSetting.CheckPass(textBox3.Text);
                    if (f)
                    {
                        MessageBox.Show("認証に成功しました。");
                        label10.Text = "認証中";
                        checkBox1.Enabled = true;
                        checkBox3.Enabled = true;
                        ChangeSetting.PassSet = true;
                    }
                    else
                    {
                        MessageBox.Show("認証に失敗しました。");
                        label10.Text = "未認証";
                        ChangeSetting.PassSet = false;
                    }
                }
                else
                {
                    ChangeSetting.SetPassword(textBox3.Text);
                    MessageBox.Show("パスコードを適用しました。");
                    label10.Text = "認証中";
                    ChangeSetting.PassStatus = true;
                    ChangeSetting.PassSet = true;
                }
            }
            textBox3.Text = "";
        }

        /// <summary>
        /// 認証解除ボタン
        /// </summary>
        private void button5_Click(object sender, EventArgs e)
        {
            if (ChangeSetting.PassStatus)
            {
                SaveAll();
                ChangeSetting.PassSet = false;
                checkBox1.Enabled = false;
                checkBox3.Enabled = false;
                label10.Text = "未認証";
                MessageBox.Show("アプリケーションの認証が切られました。\nこれにより機能が一部制限されます。", "権限変更", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                this.Close();
            }
        }

        /// <summary>
        /// 設定内容をconfigに反映
        /// </summary>
        private void SettingForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Gi_Proxy.Properties.Settings.Default.UpperIP = ChangeSetting.Ups.IPAdress;
            Gi_Proxy.Properties.Settings.Default.UpperPort = ChangeSetting.Ups.Port;
            Gi_Proxy.Properties.Settings.Default.PassSet = ChangeSetting.PassSet;
            Gi_Proxy.Properties.Settings.Default.PassState = ChangeSetting.PassStatus;
            Gi_Proxy.Properties.Settings.Default.Save();
        }
    }
}