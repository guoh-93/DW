using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ERPStock
{
    public partial class frm退货申请查询界面 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        DataTable dtM = new DataTable();

        public frm退货申请查询界面()
        {
            InitializeComponent();
        }

        private void frm退货申请查询界面_Load(object sender, EventArgs e)
        {
            try
            {
                bar_日期_后.EditValue = System.DateTime.Today.AddDays(1).AddSeconds(-1);
                bar_日期_前.EditValue = System.DateTime.Today.AddDays(-14);
                bar_单据状态.EditValue = "已生效";
                fun_载入();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region 方法
        private void fun_载入()
        {
            try
            {
                if (dtM != null)
                {
                    dtM.Clear();
                }
                string s_组合 = @"select x.*,总金额 from 退货申请主表 x
                 left join (select 退货申请单号,sum(税后金额)总金额 from  退货申请子表 group by 退货申请单号 ) a
                    on x.退货申请单号=a.退货申请单号 {0}";
                string s_组合1 = "where ";

                if (bar_日期_前.EditValue != null && bar_日期_后.EditValue != null && bar_日期_前.EditValue.ToString() != "" && bar_日期_后.EditValue.ToString() != "")
                {
                    s_组合1 += " x.申请日期 >= '" + ((DateTime)bar_日期_前.EditValue).ToString("yyyy-MM-dd HH:mm:ss") + "'" + " and x.申请日期 <= '" + Convert.ToDateTime(bar_日期_后.EditValue).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss") + "' and ";
                }
                if (bar_单据状态.EditValue != null && bar_单据状态.EditValue.ToString() != "")
                {
                    if (bar_单据状态.EditValue.ToString() == "已生效")
                    {
                        s_组合1 += "x.生效 = 'True' and ";
                    }
                    if (bar_单据状态.EditValue.ToString() == "未生效")
                    {
                        s_组合1 += "x.生效 = 'False' and ";
                    }
                    if (bar_单据状态.EditValue.ToString() == "所有")
                    { }
                }
                if (s_组合1 != "where ")
                {
                    s_组合1 = s_组合1.Substring(0, s_组合1.Length - 4);
                    s_组合 = string.Format(s_组合, s_组合1);
                }
                SqlDataAdapter da = new SqlDataAdapter(s_组合, strconn);
                da.Fill(dtM);
                gc.DataSource = dtM;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "退货申请主表_刷新操作");
                throw ex;
            }
        }
        #endregion

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_载入();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                gc.ExportToXlsx(saveFileDialog.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        private void refalsh_single(DataRow dr, string ID)
        {

            string sql = $"select * from  退货申请主表  where 退货申请单号 = '{ID}'";
            DataRow r = CZMaster.MasterSQL.Get_DataRow(sql, CPublic.Var.strConn);
            dr.ItemArray = r.ItemArray;
            dr.AcceptChanges();

            if (dr["操作人员编号"].ToString() == CPublic.Var.LocalUserID || CPublic.Var.LocalUserTeam.Contains("管理员"))
            {
                barLargeButtonItem8.Enabled = true;
            }
            else
            {
                barLargeButtonItem8.Enabled = false;
            }
        }
        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {

                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                refalsh_single(dr, dr["退货申请单号"].ToString());
                if (e.Clicks == 1 && e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    string sql = string.Format("select 退货申请子表.*,a.物料名称,a.规格型号 from 退货申请子表 left join 基础数据物料信息表 a on 退货申请子表.物料编码 = a.物料编码 where 退货申请单号 = '{0}'", dr["退货申请单号"]);
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt);
                    gcP.DataSource = dt;
                }
                else if (e.Clicks == 2)
                {
                    if (!Convert.ToBoolean(dr["审核"]) && !Convert.ToBoolean(dr["作废"]) && !Convert.ToBoolean(dr["完成"]) && !Convert.ToBoolean(dr["提交审核"]))
                    {
                        frm退货申请界面 frm = new frm退货申请界面(dr);
                        DataRow r = gv.GetDataRow(gv.FocusedRowHandle);
                        CPublic.UIcontrol.Showpage(frm, "退货申请");
                    }
                    else
                    {
                        throw new Exception("该单据的状态不可双击进行修改");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv.GetFocusedRowCellValue(gv.FocusedColumn));
                e.Handled = true;
            }
        }

        private void gvP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gvP.GetFocusedRowCellValue(gvP.FocusedColumn));
                e.Handled = true;
            }
        }

        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gvP_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            if (!Convert.ToBoolean(dr["完成"]) && !Convert.ToBoolean(dr["作废"]) && !Convert.ToBoolean(dr["审核"]) && !Convert.ToBoolean(dr["提交审核"]))
            {
                DateTime t = CPublic.Var.getDatetime();
                // delete  退货申请子表 where 退货申请单号='{dr["退货申请单号"].ToString()}'
                string sql = $@"update 退货申请主表 set 作废=1,作废日期='{t}',作废人='{CPublic.Var.localUserName}' where 退货申请单号='{dr["退货申请单号"].ToString()}'";
                CZMaster.MasterSQL.ExecuteSQL(sql, strconn);
                MessageBox.Show("已作废");
                fun_载入();
            }
            else
            {
                MessageBox.Show("该单据的状态不可作废");

            }
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {


        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {


            DataRow drM = (this.BindingContext[gc.DataSource].Current as DataRowView).Row;
            DataTable dtm = (DataTable)this.gcP.DataSource;
            Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
            Type outerForm = outerAsm.GetType("ERPreport.退货申请打印啊", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统

            object[] drr = new object[2];

            drr[0] = drM;
            drr[1] = dtm;
            //   drr[2] = dr["出入库申请单号"].ToString();
            Form ui = Activator.CreateInstance(outerForm, drr) as Form;
            //  UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
            ui.ShowDialog();






        }
        private void fun_按出库单查(string s)
        {
            string x = string.Format(@"select x.*,总金额 from 退货申请主表 x
     left join (select 退货申请单号,sum(税后金额)总金额 from  退货申请子表 group by 退货申请单号 ) a
    on x.退货申请单号=a.退货申请单号  where x.退货申请单号 
    in  (select  退货申请单号 from 退货申请子表 where 出库明细号 like '%{0}%' group by 退货申请单号)", s);
            dtM = CZMaster.MasterSQL.Get_DataTable(x, strconn);
            gc.DataSource = dtM;



        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (barEditItem1.EditValue == null || barEditItem1.EditValue.ToString() == "") throw new Exception("未录入出库单");
                fun_按出库单查(barEditItem1.EditValue.ToString());

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void gv_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (dr == null)
                {
                    string sql = string.Format(@"select 退货申请子表.*,a.物料名称,a.规格型号 from 退货申请子表 left join 基础数据物料信息表 a 
                         on 退货申请子表.物料编码 = a.物料编码 where 1=2");
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt);
                    gcP.DataSource = dt;
                }
                else
                {
                    string sql = string.Format(@"select 退货申请子表.*,a.物料名称,a.规格型号 from 退货申请子表 left join 基础数据物料信息表 a on 退货申请子表.物料编码 = a.物料编码 where 退货申请单号 = '{0}'", dr["退货申请单号"]);
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt);
                    gcP.DataSource = dt;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //撤销提交
        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                refalsh_single(dr, dr["退货申请单号"].ToString());
                if (Convert.ToBoolean(dr["审核"]) || Convert.ToBoolean(dr["作废"]))
                {
                    throw new Exception("单据状态不支持撤回");
                }
                else
                {
                    if (!Convert.ToBoolean(dr["提交审核"]))
                    {
                        throw new Exception("单据未提交不需撤回");
                    }

                }
                if (MessageBox.Show(string.Format("该销售单是否确认撤销提交？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string sql = string.Format("select * from 单据审核申请表  where  单据类型='销售退货'  and  关联单号 = '{0}' and 作废=0 and 审核=0", dr["退货申请单号"].ToString());
                    DataTable dt_审核申请 = new DataTable();
                    dt_审核申请 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt_审核申请.Rows.Count > 0)
                    {
                        dt_审核申请.Rows[0].Delete();
                    }
                    dr["提交审核"] = false;

                    //dt_审核申请.TableName = "单据审核申请表";
                    //dtM.TableName = "退货申请主表";
                    Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();


                    dic.Add("单据审核申请表", dt_审核申请);
                    dic.Add("退货申请主表", dtM);


                    ERPorg.Corg cg = new ERPorg.Corg();
                    cg.save(dic);
                    MessageBox.Show("撤回成功");
                    refalsh_single(dr, dr["退货申请单号"].ToString());

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
