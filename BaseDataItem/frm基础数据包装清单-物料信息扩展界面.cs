using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace BaseData
{
    public partial class fm基础数据包装清单_物料信息扩展界面 : UserControl
    {
        #region
        string strconn = CPublic.Var.strConn;
        DataTable dtM = null;
        DataRow drM = null;
        string str_物料编码 = "";
        string str_物料名称 = "";
        DataTable dt_原料 = null;
        public static DevExpress.XtraTab.XtraTabControl XTC;
        DataTable dt_BOM转包装清单 = null;
        DataTable dt_修改记录 = null;

        #endregion

        #region
        public fm基础数据包装清单_物料信息扩展界面()
        {
            InitializeComponent();
        }

        public fm基础数据包装清单_物料信息扩展界面(string str, string strr)
        {
            InitializeComponent();
            str_物料编码 = str;
            str_物料名称 = strr;
        }

        public fm基础数据包装清单_物料信息扩展界面(string str, string strr, DataTable tt)
        {
            InitializeComponent();
            str_物料编码 = str;
            str_物料名称 = strr;
            dt_BOM转包装清单 = tt;
        }

        private void fm基础数据包装清单_物料信息扩展界面_Load(object sender, EventArgs e)
        {
            try
            {
                label1.Text = "";
                fun_载入(str_物料编码);
                fun_原料();
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "包装清单-物料信息");
            }
        }
        #endregion

        #region
        private void fun_原料()
        {
            string sql = "select 物料编码,物料编码,物料名称,规格,大类,小类,n原ERP规格型号,图纸编号 from 基础数据物料信息表 where (物料类型 = '原材料' or 物料类型 = '半成品') /* and 停用 = 0 */";
            dt_原料 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_原料);
            repositoryItemSearchLookUpEdit1.PopupFormSize = new Size(1400, 400);

            repositoryItemSearchLookUpEdit1.DataSource = dt_原料;
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";

            string s = "select * from 基础数据包装清单修改日志表 where 1<>1";
            dt_修改记录=CZMaster.MasterSQL.Get_DataTable(s,strconn);
        }

        private void fun_载入(string str物料编码)
        {
            string sql = string.Format(@"select a.*,物料编码,n原ERP规格型号 from 基础数据包装清单表 a
                                           left  join  基础数据物料信息表 b  on b.物料编码=a.物料编码
                                           where 成品编码 ='{0}'", str物料编码);
            dtM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            gc.DataSource = dtM;

            string sql_临时 = string.Format("select * from 基础数据物料信息表 where 物料编码 ='{0}'", str_物料编码);
            DataTable dt_临时 = CZMaster.MasterSQL.Get_DataTable(sql_临时, strconn);
            label1.Text = string.Format("当前成品编码为{0},名称：{1},规格:{2}", str物料编码, dt_临时.Rows[0]["物料名称"].ToString(), dt_临时.Rows[0]["n原ERP规格型号"].ToString());
            if (dt_BOM转包装清单 != null)
            {
                foreach (DataRow dr in dt_BOM转包装清单.Rows)
                {
                    DataRow[] ds = dtM.Select(string.Format("物料编码 = '{0}'", dr["子项编码"].ToString()));
                    if (ds.Length <= 0)
                    {
                        DataRow r = dtM.NewRow();
                        dtM.Rows.Add(r);
                        r["GUID"] = System.Guid.NewGuid();
                        r["成品名称"] = str_物料名称;
                        r["成品编码"] = str_物料编码;
                        r["物料编码"] = dr["子项编码"];
                        r["物料名称"] = dr["子项名称"];
                        r["规格型号"] = dr["规格型号"];
                        r["大类"] = dr["大类"];
                        r["小类"] = dr["小类"];
                    }
                }
            }
            dtM.ColumnChanged += dtM_ColumnChanged;
        }

        void dtM_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {   
            try
            {
                if (e.Column.ColumnName == "物料编码")
                {
                    DataRow[] ds = dt_原料.Select(string.Format("物料编码 = '{0}'", e.Row["物料编码"].ToString()));
                    if (ds.Length > 0)
                    {
                        e.Row["物料名称"] = ds[0]["物料名称"].ToString();
                 

                        e.Row["物料编码"] = ds[0]["物料编码"].ToString();
                        e.Row["大类"] = ds[0]["大类"].ToString();
                        e.Row["小类"] = ds[0]["小类"].ToString();
                        e.Row["规格型号"] = ds[0]["n原ERP规格型号"].ToString();
                        e.Row["图纸编号"] = ds[0]["图纸编号"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "包装清单-物料信息");
            }
        }
        private void fun_包装修改记录()
        {

            DateTime t = CPublic.Var.getDatetime();
            foreach (DataRow dr in dtM.Rows)
            {
                if (dr.RowState == DataRowState.Deleted)
                {
                    string str_原物料 = dr["物料编码", DataRowVersion.Original].ToString();
                    //string str_后物料 = dr["子项编码", DataRowVersion.Current].ToString();

                    DataRow rr = dt_修改记录.NewRow();
                    rr["修改人"] = CPublic.Var.localUserName;
                    rr["修改人ID"] = CPublic.Var.LocalUserID;
                    rr["修改时间"] = t;

                    rr["产品编码"] = str_物料编码;
                    rr["产品名称"] = str_物料名称;
                    rr["产品型号"] = dr["规格型号",DataRowVersion.Original];


                    rr["修改属性"] = "删除";
                    rr["更改前物料"] = str_原物料;
                    rr["更改前数量"] = dr["数量", DataRowVersion.Original].ToString();
                    dt_修改记录.Rows.Add(rr);
                
                }

                if (dr.RowState == DataRowState.Modified)
                {
                    string str_原物料 = dr["物料编码", DataRowVersion.Original].ToString();
                    string str_后物料 = dr["物料编码", DataRowVersion.Current].ToString();
                    DataRow rr = dt_修改记录.NewRow();
                    rr["修改人"] = CPublic.Var.localUserName;
                    rr["修改人ID"] = CPublic.Var.LocalUserID;
                    rr["修改时间"] = t;

                    rr["产品编码"] = str_物料编码;
                    rr["产品名称"] = str_物料名称;
                    rr["产品型号"] = dr["规格型号"];

                    rr["修改属性"] = "修改";
                    rr["更改前物料"] = str_原物料;
                    rr["更改后物料"] = str_后物料;
                    rr["更改前数量"] = dr["数量", DataRowVersion.Original].ToString();
                    rr["更改后数量"] = dr["数量", DataRowVersion.Current].ToString();
                    dt_修改记录.Rows.Add(rr);

                 
                }
                if (dr.RowState == DataRowState.Added)
                {

                    string str_后物料 = dr["物料编码", DataRowVersion.Current].ToString();
                    DataRow rr = dt_修改记录.NewRow();
                    rr["修改人"] = CPublic.Var.localUserName;
                    rr["修改人ID"] = CPublic.Var.LocalUserID;
                    rr["修改时间"] = t;

                    rr["产品编码"] = str_物料编码;
                    rr["产品名称"] = str_物料名称;
                    rr["产品型号"] = dr["规格型号"];

                    rr["修改属性"] = "增加";
                    rr["更改后物料"] = str_后物料;
                    rr["更改后数量"] = dr["数量"].ToString();

                    dt_修改记录.Rows.Add(rr);
 

                }
            }

        }
        private void fun_保存()
        {
            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("包装清单保存");
            try
            {
              
                string sql = string.Format("select * from 基础数据包装清单表 where 1<>1");
                SqlCommand cmd1 = new SqlCommand(sql, conn, ts);
                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da);
                da.Update(dtM);

                sql = string.Format("select * from 基础数据包装清单修改日志表 where 1<>1");
         
                SqlCommand cmd2= new SqlCommand(sql, conn, ts);
                da = new SqlDataAdapter(cmd2);
                new SqlCommandBuilder(da);
                da.Update(dt_修改记录);

                ts.Commit();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw new Exception(ex.Message);
            }
           

            
        }
        #endregion

        #region
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                drM = dtM.NewRow();
                dtM.Rows.Add(drM);
                drM["GUID"] = System.Guid.NewGuid();
                drM["成品名称"] = str_物料名称;
                drM["成品编码"] = str_物料编码;
            }
            catch (Exception ex)
            {

                CZMaster.MasterLog.WriteLog(ex.Message, "包装清单-物料信息");
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show("是否要删除该行？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                    dr.Delete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                CZMaster.MasterLog.WriteLog(ex.Message, "包装清单-物料信息");
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv.CloseEditor();
                gc.BindingContext[dtM].EndCurrentEdit();
                if (MessageBox.Show("是否要保存？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    fun_包装修改记录();
                    fun_保存();
                    MessageBox.Show("保存成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                CZMaster.MasterLog.WriteLog(ex.Message, "包装清单-物料信息");
            }
        }


        #endregion

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
             if (XTC.TabPages.Count == 1) { }
            if (XTC.SelectedTabPage.Text == "首页") { }
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
            }
            catch { }          
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_载入(str_物料编码);
        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsExportOptions options = new DevExpress.XtraPrinting.XlsExportOptions();

                    gc.ExportToXlsx(saveFileDialog.FileName);

                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }     
    }
}
