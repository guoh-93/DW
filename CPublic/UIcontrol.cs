using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraTab;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace CPublic
{
    public class UIcontrol
    {
        /// <summary>
        /// 模块常量
        /// </summary>
        public static DevExpress.XtraTab.XtraTabControl  XTC;
        [DllImport("kernel32.dll")]
        private static extern bool SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        //刷新存储器 
        private static void FlushMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
            }
        }

        public static Boolean ClosePage()
        {

            if (XTC.TabPages.Count == 1) return false;
            if (XTC.SelectedTabPage.Text == "首页") return false;
            DevExpress.XtraTab.XtraTabPage xtp = null;
            try
            {
                xtp = XTC.SelectedTabPage;
                XTC.SelectedTabPageIndex = XTC.SelectedTabPageIndex - 1;
            }
            catch { }
            try
            {
                xtp.Controls[0].Dispose();
                XTC.TabPages.Remove(xtp);
                xtp.Dispose();
                FlushMemory();
                return true;
            }
            catch { }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fm"></param>
        /// <param name="Caption"></param>
        public static void AddNewPage(UserControl fm, string Caption)
        {
            XtraTabPage xtp = XTC.TabPages.Add(Caption);
            xtp.ShowCloseButton = DefaultBoolean.Default;
            xtp.Controls.Add(fm);
            xtp.AllowDrop = true;
            fm.Dock = DockStyle.Fill;
            XTC.SelectedTabPage = xtp;
        }
        /// <summary>
        /// 显示页面，如果已经存在
        /// </summary>
        /// <param name="fm"></param>
        /// <param name="Caption"></param>
        public static void ShowNewPage(UserControl fm, string Caption)
        {
            foreach (XtraTabPage x in XTC.TabPages)
            {
                foreach (Control c in x.Controls)
                {
                    if (c.GetType() == fm.GetType())
                    {
                        XTC.SelectedTabPage = x;
                        return;

                    }
                }
            }
            XtraTabPage xtp = XTC.TabPages.Add(Caption);
            xtp.ShowCloseButton = DefaultBoolean.Default;
            xtp.AllowDrop = true;
            xtp.Controls.Add(fm);
            fm.Dock = DockStyle.Fill;
            XTC.SelectedTabPage = xtp;
        }
        /// <summary>
        /// 显示页面，不显示closebutton
        /// </summary>
        /// <param name="fm"></param>
        /// <param name="Caption"></param>
        public static void ShowPage_withoutclosebutton(UserControl fm, string Caption)
        {
            int i = 0;
            foreach (XtraTabPage x in XTC.TabPages)
            {
                foreach (Control c in x.Controls)
                {
                    if (c.GetType() == fm.GetType())
                    {
                        XTC.TabPages.Remove(x);
                        i = 1;
                        break;
                    }
                }
                if (i == 1)
                {
                    break;
                }

            }
            XtraTabPage xtp = XTC.TabPages.Add(Caption);
            xtp.ShowCloseButton = DefaultBoolean.False;

            xtp.Controls.Add(fm);
            fm.Dock = DockStyle.Fill;
            XTC.SelectedTabPage = xtp;
        }
        /// <summary>
        /// 关闭原有的 新开一个
        /// </summary>
        /// <param name="fm"></param>
        /// <param name="Caption"></param>
        public static void Showpage(UserControl fm, string Caption)
        {
            int i = 0;
            foreach (XtraTabPage x in XTC.TabPages)
            {
                foreach (Control c in x.Controls)
                {
                    if (c.GetType() == fm.GetType())
                    {
                        XTC.TabPages.Remove(x);
                        i = 1;
                        break;

                    }
                }
                if (i == 1)
                {
                    break;
                }

            }
            XtraTabPage xtp = XTC.TabPages.Add(Caption);
            xtp.ShowCloseButton = DefaultBoolean.Default;
            xtp.AllowDrop = true;
            xtp.Controls.Add(fm);
            fm.Dock = DockStyle.Fill;
            XTC.SelectedTabPage = xtp;
        }

        public static void closeallpage()
        {
            int c = XTC.TabPages.Count;
            if (c >= 2)
            {
                for (int i = 1; i < c; i++)
                {
                    XTC.TabPages.Remove(XTC.TabPages[1]);

                }
            }

        }
        #region 让GC可以响应鼠标滚动事件
        /// <summary>
        /// 让GC可以响应鼠标滚动事件,在readonly下GC不响应鼠标滚动事件.
        /// </summary>
        /// <param name="gv">gridcontrol的view</param>
        public static void GridControlResponseMouseWheel(DevExpress.XtraGrid.Views.Grid.GridView gv)
        {
            gv.ShownEditor += gv_ShownEditor;
        }

        static void gv_ShownEditor(object sender, EventArgs e)
        {

            (sender as DevExpress.XtraGrid.Views.Grid.GridView).ActiveEditor.MouseWheel += ActiveEditor_MouseWheel;
        }

        static void ActiveEditor_MouseWheel(object sender, MouseEventArgs e)
        {
            (sender as DevExpress.XtraGrid.Views.Grid.GridView).CloseEditor();
        }
        #endregion




    }
}
