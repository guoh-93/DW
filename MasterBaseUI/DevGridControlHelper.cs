using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
//using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Data;
using System.Reflection;

namespace CZMaster
{

    /// <summary>
    /// 扩展 DevGridControlHelper
    /// </summary>
    /// 
    //[ProvideProperty("DevGridControl 扩展组件", typeof(Control))]
    public class DevGridControlHelper 
    {
        //private System.ComponentModel.Container components;
        //public DevGridControlHelper()
        //{
        //    this.components = new System.ComponentModel.Container();

        //    this.
        //}
        


        /// <summary>
        /// 扩展GridControl功能
        /// 在LOAD的时候调用 CZMaster.DevGridControlHelper.Helper(this);
        /// 功能1.gc在编辑状态下接受鼠标滚动事件
        /// 功能2.gc在非编辑状态下右键拷贝功能
        /// </summary>
        /// <param name="control"></param>
        public static void Helper(Control  control)
        {
            foreach (Control c in control.Controls)
            {
                if (c is DevExpress.XtraGrid.GridControl)
                {
                    DevExpress.XtraGrid.GridControl gc = c as DevExpress.XtraGrid.GridControl;

                    Helper(gc.MainView as DevExpress.XtraGrid.Views.Grid.GridView);
                }
                else
                {
                    Helper(c);
                }
            }
        }


        #region 让GC可以响应鼠标滚动事件
        /// <summary>
        /// 调用 CZMaster.DevGridControlHelper.Helper(gv);
        /// 功能1.gc在编辑状态下接受鼠标滚动事件
        /// 功能2.gc在非编辑状态下右键拷贝功能
        /// </summary>
        /// <param name="gv">gridcontrol的view</param>
        public static void Helper(DevExpress.XtraGrid.Views.Grid.GridView gv)
        {
            gv.ShownEditor += gv_ShownEditor;
            gv.MouseUp += gv_MouseUp;
        }

        /// <summary>
        /// 右键拷贝
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void gv_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView gv = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                Clipboard.SetDataObject(gv.GetFocusedDisplayText());
            }
            catch
            {

            }
        }

        /// <summary>
        /// 弹出编辑框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void gv_ShownEditor(object sender, EventArgs e)
        {

            (sender as DevExpress.XtraGrid.Views.Grid.GridView).ActiveEditor.MouseWheel += ActiveEditor_MouseWheel;
            //(sender as DevExpress.XtraGrid.Views.Grid.GridView).ActiveEditor.MouseUp += ActiveEditor_MouseUp;
        }


        /// <summary>
        /// 响应鼠标滚动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void ActiveEditor_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                Control c = (sender as Control).Parent;
                if (c is DevExpress.XtraGrid.GridControl)
                {
                    (c as DevExpress.XtraGrid.GridControl).MainView.CloseEditor();
                }
            }
            catch
            {

            }
        }
        #endregion

    }
}
