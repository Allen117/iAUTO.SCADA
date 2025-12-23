using Scada.Core.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;

namespace Scada.Form
{
    public partial class frmLogin : Telerik.WinControls.UI.RadForm
    {
        public frmLogin()
        {
            // 1. 先執行系統初始化
            InitializeComponent();

            try
            {
                // 2. 手動建立一個主題管理器 (Telerik 元件有時沒主題會畫不出來)
                Telerik.WinControls.Themes.FluentTheme fluentTheme = new Telerik.WinControls.Themes.FluentTheme();

                // 3. 建立按鈕並設定絕對顯示參數
                Telerik.WinControls.UI.RadButton btn = new Telerik.WinControls.UI.RadButton();
                btn.Text = "看到我請點我";
                btn.Size = new Size(200, 100);
                btn.Location = new Point(10, 10);
                btn.ThemeName = "Fluent"; // 指定主題
                btn.Visible = true;

                // 4. 強制置頂並加入表單
                this.Controls.Add(btn);
                btn.BringToFront();

                // 5. 改變背景顏色確認視窗有在跑
                this.BackColor = Color.LightBlue;
                var logger = new Logger(new TextFileLogSink("Log/log.txt"));
                logger.Info("Telerik 測試按鈕已嘗試掛載");
            }
            catch (Exception ex)
            {
                // 如果這裡報錯，請告訴我錯誤訊息，這就是工具箱亮不起來的真因
                MessageBox.Show("Telerik 載入失敗: " + ex.Message);
            }
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {

        }
    }
}
