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
    /// 本类型可以对界面gridcontrol进行自定义控制
    /// 自定义分二部分
    /// 1.字段描述，开发者自定义字段描述和显示，未来功能扩展到用户自定义字段描述和显示，这个功能受用户权限的影响,这个数据必需在adminui下保存。
    /// 2.表头格式，用户对自定义表头格式。自动保存用户的表头格式。这个功能受用户的影响。
    /// 3.默认表头格式，管理员adminui用户保存的数据为默认表头格式。
    /// 需要表 Sys_GridControlField，Sys_GridControlLayout
    /// </summary>
    [ProvideProperty("DevGridControlCustom", typeof(DevExpress.XtraGrid.GridControl))]
    public class DevGridControlCustom : System.ComponentModel.Component, System.ComponentModel.IExtenderProvider
    {


        private System.ComponentModel.Container components;
        Dictionary<DevExpress.XtraGrid.GridControl, string> LIC = new Dictionary<DevExpress.XtraGrid.GridControl, string>();

        /// <summary>
        /// 表示这个表格的列是自动生成的还是自带的。
        /// 如果是自动生成，那么caption后置。
        /// </summary>
        Dictionary<DevExpress.XtraGrid.GridControl, string> LIC_cutcol = new Dictionary<DevExpress.XtraGrid.GridControl, string>();

        public Dictionary<string, string> BindSetting = new Dictionary<string, string>();

        public DevGridControlCustom()
        {
            this.components = new System.ComponentModel.Container();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        bool IExtenderProvider.CanExtend(object target)
        {
            if (target is DevExpress.XtraGrid.GridControl)
            {
                return true;
            }
            return false;
        }

        #region 属性设定
        /// <summary>
        /// 这是默认用户。它可以设定各个GD的默认值
        /// </summary>
        public const string strAdmin = "adminui";

        private string userName = ""; 
        /// <summary>
        /// 当前用户名
        /// </summary>
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        private string authority = "default";

        /// <summary>
        /// 当前用户权限
        /// 本组件第一个版本先不关注用户权限涉及的问题，所有用户按一个权限处理
        /// </summary>
        public string Authority
        {
            get { return authority; }
            set { authority = value; }
        }

        private string strconn = "";
        /// <summary>
        /// 保存数据用的数据库
        /// </summary>
        public string strConn
        {
            get { return strconn; }
            set { strconn = value; }
        }

        private Boolean blAutoSave = true;

        /// <summary>
        /// 自动保存数据
        /// </summary>
        public Boolean AutoSave
        {
            get { return blAutoSave; }
            set { blAutoSave = value; }
        }
        #endregion

        #region 扩展属性设定
        [
        DefaultValue(""),
        ]
        public string GetDevGridControlCustom(DevExpress.XtraGrid.GridControl control)
        {
            if (LIC.ContainsKey(control))
            {
                return LIC[control];
            }
            else
            {
                return "";
            }
        }

        public void SetDevGridControlCustom(DevExpress.XtraGrid.GridControl control, string value)
        {
            if (value == null)
            {
                value = "";
            }
            if(value == "")
                return;
            if (LIC.ContainsKey(control))
            {
                LIC[control] = value;
            }
            else
            {
                LIC.Add(control, value);
                if (this.DesignMode) return;
                if (control.MainView != null)
                {
                    LIC_cutcol.Add(control,"Y");
                }
                control.DataSourceChanged += control_DataSourceChanged;
                //if (blAutoSave)
                //{
                //    try
                //    {
   
                //        control.ViewRegistered += control_ViewRegistered;
                //        //(control.MainView as DevExpress.XtraGrid.Views.Grid.GridView).ColumnPositionChanged += DevGridControlCustom_ColumnPositionChanged;
                //        //(control.MainView as DevExpress.XtraGrid.Views.Grid.GridView).ColumnWidthChanged += DevGridControlCustom_ColumnWidthChanged;
                //    }
                //    catch(Exception ex)
                //    {
                //        CZMaster.MasterLog.WriteLog(ex.Message, "DevGridControlCustom AutoSave =:" + value);
                //    }
                //}
            }
        }

        void control_ViewRegistered(object sender, DevExpress.XtraGrid.ViewOperationEventArgs e)
        {
            if (blAutoSave)
            {
                DevExpress.XtraGrid.GridControl c = sender as DevExpress.XtraGrid.GridControl;
                try
                {
                    (c.MainView as DevExpress.XtraGrid.Views.Grid.GridView).ColumnPositionChanged += DevGridControlCustom_ColumnPositionChanged;
                    (c.MainView as DevExpress.XtraGrid.Views.Grid.GridView).ColumnWidthChanged += DevGridControlCustom_ColumnWidthChanged;
                }
                catch (Exception ex)
                {
                    CZMaster.MasterLog.WriteLog(ex.Message, "DevGridControlCustom AutoSave =:" + LIC[c]);
                }
            }
        }

        #endregion


        #region 数据处理
        void control_DataSourceChanged(object sender, EventArgs e)
        {
            if (UserName == "" || strconn == "") { return; }

            try
            {
                DevExpress.XtraGrid.GridControl c = sender as DevExpress.XtraGrid.GridControl;
                fun_ReadCustom(c);

                if (blAutoSave)
                {
                    try
                    {
                        (c.MainView as DevExpress.XtraGrid.Views.Grid.GridView).ColumnPositionChanged += DevGridControlCustom_ColumnPositionChanged;
                        (c.MainView as DevExpress.XtraGrid.Views.Grid.GridView).ColumnWidthChanged += DevGridControlCustom_ColumnWidthChanged;
                    }
                    catch (Exception ex)
                    {
                        CZMaster.MasterLog.WriteLog(ex.Message, "DevGridControlCustom AutoSave =:" + LIC[c]);
                    }
                }
            }
            catch(Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "DevGridControlCustom :" + LIC[sender as DevExpress.XtraGrid.GridControl]);
            }
        }

        private void fun_ReadCustom(DevExpress.XtraGrid.GridControl gc)
        {
            string gckey = LIC[gc];
            string sql = string.Format("select * from Sys_GridControl where gcKey = '{0}'", gckey);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            if (dt.Rows.Count == 0)
            {
                //如果没有主表
                //新增主表，新增副表
                DataRow r = dt.NewRow();
                dt.Rows.Add(r);
                CZMaster.DataTableFun.fun_单行数据赋值(r, new Object[] { "gckey", gckey, "gcDesc", gckey, });
                MasterSQL.Save_DataTable(dt, "Sys_GridControl", strconn);

                //新增字段表，先删除，后新增
                sql = string.Format("select * from Sys_GridControlField where gcKey = '{0}' ", gckey, authority);
                DataTable dtField = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                foreach (DataRow r1 in dtField.Rows)
                {
                    r1.Delete();
                }
                MasterSQL.Save_DataTable(dtField, "Sys_GridControlField", strconn);

                //删除自定义设置
                sql = string.Format("DELETE FROM Sys_GridControlLayout WHERE gckey = '{0}' ", gckey);
                CZMaster.MasterSQL.ExecuteSQL(sql, strconn);
            }
            else
            {

                fun_ReadField(gc,Authority);


                if (fun_ReadtXML(gckey, gc, UserName) == false)
                {
                    fun_ReadtXML(gckey, gc, strAdmin);
                }
            }

            ///初始化字段
            if (userName == strAdmin)
            {
                sql = string.Format("select * from Sys_GridControlField where gcKey = '{0}' ", gckey, authority);
                DataTable dtField = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                if (dtField.Rows.Count == 0)
                {
                    foreach (DevExpress.XtraGrid.Columns.GridColumn gcl in (gc.MainView as DevExpress.XtraGrid.Views.Grid.GridView).Columns)
                    {
                        if (LIC_cutcol.ContainsKey(gc))
                        {
                            dtField.Rows.Add(0, gckey, authority, gcl.Name, gcl.FieldName, gcl.Caption , true);
                        }
                        else
                        {
                            dtField.Rows.Add(0, gckey, authority, gcl.Name, gcl.FieldName,  gcl.FieldName, true);
                        }
                        
                    }
                    MasterSQL.Save_DataTable(dtField, "Sys_GridControlField", strconn);
                }
            }
            
        }

        private void fun_ReadField(DevExpress.XtraGrid.GridControl gc, string strAuthority)
        {
            try
            {
                string sql = string.Format("select * from Sys_GridControlField where gcKey = '{0}' ", LIC[gc], authority);
                DataTable dtField = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                List<DevExpress.XtraGrid.Columns.GridColumn > li = new List<DevExpress.XtraGrid.Columns.GridColumn>();
                foreach (DevExpress.XtraGrid.Columns.GridColumn gcl in (gc.MainView as DevExpress.XtraGrid.Views.Grid.GridView).Columns)
                {
                    try
                    {
                        DataRow[] rs = dtField.Select(string.Format("colName = '{0}'",gcl.Name));
                        gcl.Caption = rs[0]["colText"].ToString();
                        if((Boolean)rs[0]["disp"] == false)li.Add(gcl);
                    }
                    catch
                    {

                    }
                }
                foreach (DevExpress.XtraGrid.Columns.GridColumn gcl in li)
                {
                    (gc.MainView as DevExpress.XtraGrid.Views.Grid.GridView).Columns.Remove(gcl);
                }
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message, "DevGridControlCustom");
            }
        }

        private Boolean fun_ReadtXML(string gcKey, DevExpress.XtraGrid.GridControl gc,string urName)
        {
            //读取默认格式

            try
            {
                //保存格式
                string sql = "";
                sql = string.Format("select * from Sys_GridControlLayout WHERE gckey = '{0}' and Ur = '{1}'", gcKey, urName);
                DataTable dtLayout = CZMaster.MasterSQL.Get_DataTable(sql, strConn);
                string xmlfile = MasterSQL.GetCurrCoustDir("temp\\") + System.Guid.NewGuid().ToString();


                if (dtLayout.Rows.Count > 0)
                {
                    if (dtLayout.Rows[0]["xml"] == DBNull.Value) return false;
                    byte[] xml = (byte[])dtLayout.Rows[0]["xml"];
                    System.IO.File.WriteAllBytes(xmlfile, xml);
                    gc.MainView.RestoreLayoutFromXml(xmlfile);
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message, "DevGridControlCustom");
                return false;
            }
        }

        private void fun_SaveXML(DevExpress.XtraGrid.GridControl gc)
        {
            try
            {
                string gcKey = LIC[gc];
                //保存格式
                string sql = "";
                sql = string.Format("select * from Sys_GridControlLayout WHERE gckey = '{0}' and Ur = '{1}'", gcKey, UserName);
                DataTable dtLayout = CZMaster.MasterSQL.Get_DataTable(sql, strConn);
                string xmlfile = MasterSQL.GetCurrCoustDir("temp\\") + System.Guid.NewGuid().ToString();
                gc.MainView.SaveLayoutToXml(xmlfile);
                byte[] xml = System.IO.File.ReadAllBytes(xmlfile);
                if (dtLayout.Rows.Count > 0)
                {
                    dtLayout.Rows[0]["xml"] = xml;
                }
                else
                {
                    dtLayout.Rows.Add(0, gcKey, UserName, xml);
                }
                CZMaster.MasterSQL.Save_DataTable(dtLayout, "Sys_GridControlLayout", strConn);
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message, "DevGridControlCustom");
            }
        }

        #region 宽度变化和位置变化
        /// <summary>
        /// 宽度变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DevGridControlCustom_ColumnWidthChanged(object sender, DevExpress.XtraGrid.Views.Base.ColumnEventArgs e)
        {
            if (AutoSave)
                fun_SaveXML((sender as DevExpress.XtraGrid.Views.Grid.GridView).GridControl);
        }
        /// <summary>
        /// 位置变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DevGridControlCustom_ColumnPositionChanged(object sender, EventArgs e)
        {
            if (AutoSave)
            {
                DevExpress.XtraGrid.Columns.GridColumn gcl = sender as DevExpress.XtraGrid.Columns.GridColumn;

                fun_SaveXML(gcl.View.GridControl);
            }
        }
        #endregion


        #endregion
    }
}
