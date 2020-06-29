
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;


namespace BaseData
{
    public partial class ui在线专利数据库 : UserControl
    {
        public ui在线专利数据库()
        {
            InitializeComponent();
            webBrowser1.ScriptErrorsSuppressed = false;
            SuppressScriptErrorsOnly(webBrowser1);
        }


        private void SuppressScriptErrorsOnly(WebBrowser browser)
        {
            // 确信 ScriptErrorsSuppressed 设为 false.    
            browser.ScriptErrorsSuppressed = false;

            // 处理 DocumentCompleted 事件以访问 Document 对象.    
            browser.DocumentCompleted +=
                new WebBrowserDocumentCompletedEventHandler(
                    browser_DocumentCompleted);
        }

        private void browser_DocumentCompleted(object sender,
            WebBrowserDocumentCompletedEventArgs e)
        {
            ((WebBrowser)sender).Document.Window.Error +=
                new HtmlElementErrorEventHandler(Window_Error);
        }

        private void Window_Error(object sender,
            HtmlElementErrorEventArgs e)
        {
            // 忽略该错误并抑制错误对话框    
            e.Handled = true;

        }

       

   

    




    }
}
