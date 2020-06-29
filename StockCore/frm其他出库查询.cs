using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace StockCore
{
    public partial class frm其他出库查询 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dtM = new DataTable();
        DataTable dt_mx;
        DataTable dt_仓库人员;
        string cfgfilepath = "";
        #endregion

        #region 自用类
        public frm其他出库查询()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm其他出库查询_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(this.splitContainer1, this.Name, cfgfilepath);
                DateTime t = CPublic.Var.getDatetime().Date;
                bar_日期_后.EditValue = t.AddDays(1).AddSeconds(-1);
                bar_日期_前.EditValue = t.AddMonths(-2);
                bar_单据状态.EditValue = "已生效";
                //if (CPublic.Var.localUserName == "admin" || CPublic.Var.LocalUserTeam == "管理员权限" || CPublic.Var.localUser部门编号 == "00010602")
                //{
                //    barLargeButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                //}
                fun_载入();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);

                if (dr == null) return;
                if (CPublic.Var.localUserName == "admin" || CPublic.Var.LocalUserTeam == "管理员权限" || CPublic.Var.LocalUserID == dr["操作人员编号"].ToString())
                {
                    if (dr["原因分类"].ToString() != "调拨出库")
                    {
                        barLargeButtonItem5.Enabled = true;
                    }
                    else
                    {
                        barLargeButtonItem5.Enabled = false;
                    }


                }
                else
                {
                    barLargeButtonItem5.Enabled = false;
                }

                string sql = string.Format(@"select  rz.*,crls.仓库号,crls.仓库名称  from 其他出库子表 rz
                  left join 仓库出入库明细表 crls on   rz.其他出库明细号 = crls.明细号  where 其他出库单号='{0}'", dr["其他出库单号"]);
                dt_mx = new DataTable();
                dt_mx= CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                gridControl1.DataSource = dt_mx;
                //if (e.Clicks == 2 && e.Button == System.Windows.Forms.MouseButtons.Left)
                //{
                //    frm其他出库 fm = new frm其他出库(dr);
                //    CPublic.UIcontrol.AddNewPage(fm, "其他出库");
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 方法
#pragma warning disable IDE1006 // 命名样式
        private void fun_载入()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (dtM != null)
                {
                    dtM.Clear();
                }

                //视图权限
                //dt_仓库人员 = ERPorg.Corg.fun_hr("仓库", CPublic.Var.LocalUserID);

                string s_组合 = @"select rz.*,原因分类,sz.备注 as 申请备注,业务单号,部门,sz.操作人员 as 申请人 from 其他出库主表 rz
    left join 其他出入库申请主表 sz  on rz.出入库申请单号 = sz.出入库申请单号
     left join 人事基础员工表 rs  on rs.员工号 = sz.操作人员编号 {0}";

                string s_组合1 = "where 1=1 and ";
                //if (CPublic.Var.LocalUserTeam != "管理员")
                //{
                //    if (dt_仓库人员.Rows.Count != 0)
                //    {
                //        s_组合1 += " ( ";
                //        foreach (DataRow r in dt_仓库人员.Rows)
                //        {
                //            s_组合1 += "录入人员ID = '" + r["工号"].ToString().Trim() + "' or ";
                //        }
                //        s_组合1 = s_组合1.Substring(0, s_组合1.Length - 3);
                //        s_组合1 = s_组合1 + " ) ";
                //        s_组合1 += " and ";
                //    }
                //    else
                //    {
                //        throw new Exception("你没有对应的视图权限,请找信息部核实");
                //    }
                //}

                if (bar_日期_前.EditValue != null && bar_日期_后.EditValue != null && bar_日期_前.EditValue.ToString() != "" && bar_日期_后.EditValue.ToString() != "")
                {
                    s_组合1 += " rz.创建日期 >= '" + ((DateTime)bar_日期_前.EditValue).ToString("yyyy-MM-dd HH:mm:ss") + "'" + " and rz.创建日期 <= '" + ((DateTime)bar_日期_后.EditValue).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss") + "' and ";
                }
                if (bar_单据状态.EditValue != null && bar_单据状态.EditValue.ToString() != "")
                {
                    if (bar_单据状态.EditValue.ToString() == "已生效")
                    {
                        s_组合1 += "rz.生效 = 'True' and ";
                    }
                    if (bar_单据状态.EditValue.ToString() == "未生效")
                    {
                        s_组合1 += "rz.生效 = 'False' and ";
                    }
                    if (bar_单据状态.EditValue.ToString() == "已完成")
                    {
                        s_组合1 += "rz.完成 = 'True' and ";
                    }
                    if (bar_单据状态.EditValue.ToString() == "未完成")
                    {
                        s_组合1 += "rz.完成 = 'False' and ";
                    }
                    if (bar_单据状态.EditValue.ToString() == "所有")
                    { }
                }
                if (s_组合1 != "where 1=1 and ")
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
                CZMaster.MasterLog.WriteLog(ex.Message, "其他出库主表_刷新操作");
                throw ex;
            }
        }
        #endregion

        #region 界面操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_载入();
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }
        #endregion

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
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

 

 

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (MessageBox.Show("确定打印？", "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                    string sql_zb = string.Format("select * from 其他出库子表 where 其他出库单号 = '{0}'", dr["其他出库单号"]);
                    DataTable dt_dy = CZMaster.MasterSQL.Get_DataTable(sql_zb, strconn);
                    string sql = string.Format(@"select 其他出库单号 as 编号,申请类型,a.操作人员,部门,a.备注,b.生效日期 from 其他出入库申请主表  a
                        left join 人事基础员工表 on  a.操作人员编号=人事基础员工表.员工号 
                        left join 其他出库主表 b on a.出入库申请单号=b.出入库申请单号 where    a.出入库申请单号='{0}'  ", dr["出入库申请单号"].ToString());
                    System.Data.DataTable dt = new System.Data.DataTable();
                    new SqlDataAdapter(sql, CPublic.Var.strConn).Fill(dt);
                    dt_dy.Columns.Add("计量单位", typeof(string));
                    dt_dy.Columns.Add("仓库名称", typeof(string));
                    dt_dy.Columns.Add("货架描述", typeof(string));
                    // dt_dy.Columns.Add("仓库名称", typeof(decimal));
                    //  dt_dy.Columns.Add("货架描述", typeof(decimal));
                    foreach (DataRow r in dt_dy.Rows)
                    {

                        string sql_1 = string.Format(@"select crk.物料编码,crk.仓库名称,crk.货架描述,base.计量单位 from 仓库出入库明细表 crk left join 基础数据物料信息表 base on crk.物料编码 = base.物料编码
                                        where 单号 = '{0}'", r["其他出库单号"].ToString());
                        System.Data.DataTable dt_1 = new System.Data.DataTable();
                        dt_1 = CZMaster.MasterSQL.Get_DataTable(sql_1, CPublic.Var.strConn);
                        if (dt_1.Rows.Count > 0)
                        {


                            //   r["当前库存"]= Convert.ToDecimal(dt_1.Rows[0]["库存总数"].ToString()) - Convert.ToDecimal(r["数量"].ToString());

                            r["计量单位"] = dt_1.Rows[0]["计量单位"].ToString();
                            r["仓库名称"] = dt_1.Rows[0]["仓库名称"].ToString();
                            r["货架描述"] = dt_1.Rows[0]["货架描述"].ToString();
                        }
                    }
                    Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
                    Type outerForm = outerAsm.GetType("ERPreport.frm其他出库打印", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统

                    object[] drr = new object[3];
                    drr[0] = dt;
                    drr[1] = dt_dy;
                    drr[2] = "其他出库";
                    Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                    //  UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
                    ui.ShowDialog();




                    //ItemInspection.print_FMS.fun_print_其他出库_A5(dr["操作人员"].ToString(),dr["出入库申请单号"].ToString(), dt_dy);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            DateTime t = CPublic.Var.getDatetime();

            try
            {
                if (MessageBox.Show(string.Format("是否确认撤销此单据？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string sql = string.Format("select * from 其他出库主表 where 其他出库单号 = '{0}'", dr["其他出库单号"].ToString());
                    DataTable dt_其他出库主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    sql = string.Format("select * from 其他出库子表 where 其他出库单号 = '{0}'", dr["其他出库单号"].ToString());
                    DataTable dt_其他出库子 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    DataTable dt_出入库明细 = new DataTable();
                    DataTable dt_物料 = new DataTable();
                    DataTable dt_出入库申请主 = new DataTable();
                    DataTable dt_出入库申请子 = new DataTable();
                    Boolean s_归还记录 = false;
                    DateTime dttt = Convert.ToDateTime(dr["生效日期"]);

                    if(CPublic.Var.LocalUserID != "admin"&&CPublic.Var.LocalUserTeam != "管理员权限")
                    {
                        if (t.Month != dttt.Month)
                        {
                            throw new Exception("该订单不是当月出库，不能撤回");
                        }
                    }

                    
                    if (dt_其他出库主.Rows.Count > 0)
                    {

                        sql = string.Format("select * from 其他出入库申请主表 where 出入库申请单号 = '{0}'", dt_其他出库主.Rows[0]["出入库申请单号"].ToString());
                        dt_出入库申请主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                        sql = string.Format("select * from 其他出入库申请子表 where 出入库申请单号 = '{0}'", dt_其他出库主.Rows[0]["出入库申请单号"].ToString());
                        dt_出入库申请子 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                        dt_出入库申请主.Rows[0]["完成"] = false;
                        //   dt_出入库申请主.Rows[0]["完成日期"] = DateTime.;
                        //foreach (DataRow dr_申请子 in dt_出入库申请子.Rows)
                        //{
                        //    dr_申请子["完成"] = false;
                        //    //   dr_申请子["完成日期"] = null;
                        //}
                        
                        foreach(DataRow dr_出库子 in dt_其他出库子.Rows)
                        {
                            if (Convert.ToDecimal(dr_出库子["归还数量"]) > 0)
                            {
                                s_归还记录 = true;
                            }
                            DataRow[] ds = dt_出入库申请子.Select(string.Format("出入库申请明细号 = '{0}'", dr_出库子["出入库申请明细号"].ToString()));
                            ds[0]["完成"] = false;
                            ds[0]["已完成数量"] = 0;
                        }
                        if(s_归还记录 == true)
                        {
                            throw new Exception("该单据已有归还记录,不可撤回");
                        }

                        sql = string.Format("select * from 仓库出入库明细表 where 单号 = '{0}'", dr["其他出库单号"].ToString());
                        dt_出入库明细 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                        foreach (DataRow dr_明细 in dt_出入库明细.Rows)
                        {
                            dr_明细["实效数量"] = -Convert.ToDecimal(dr_明细["实效数量"]);
                            dr_明细["数量"] = Convert.ToDecimal(dr_明细["实效数量"]);
                        }
                        dt_物料 = ERPorg.Corg.fun_库存(1, dt_出入库明细);
                        for (int i = dt_出入库明细.Rows.Count - 1; i >= 0; i--)
                        {
                            dt_出入库明细.Rows[i].Delete();
                        }
                        for (int i = dt_其他出库子.Rows.Count - 1; i >= 0; i--)
                        {
                            dt_其他出库子.Rows[i].Delete();
                        }
                        

                        dt_其他出库主.Rows[0].Delete();


                        SqlConnection conn = new SqlConnection(strconn);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("撤销");

                        try
                        {
                            string sql1 = "select * from 其他出入库申请主表 where 1<>1";
                            string sql2 = "select * from 其他出入库申请子表 where 1<>1";
                            string sql3 = "select * from 其他出库主表 where 1<>1";
                            string sql4 = "select * from 其他出库子表 where 1<>1";
                            string sql5 = "select * from 仓库出入库明细表 where 1<>1";
                            string sql6 = "select * from 仓库物料数量表 where 1<>1";

                            SqlCommand cmd1 = new SqlCommand(sql1, conn, ts);
                            SqlCommand cmd2 = new SqlCommand(sql2, conn, ts);
                            SqlCommand cmd3 = new SqlCommand(sql3, conn, ts);
                            SqlCommand cmd4 = new SqlCommand(sql4, conn, ts);
                            SqlCommand cmd5 = new SqlCommand(sql5, conn, ts);
                            SqlCommand cmd6 = new SqlCommand(sql6, conn, ts);

                            SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da1);
                            da1.Update(dt_出入库申请主);

                            SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                            new SqlCommandBuilder(da2);
                            da2.Update(dt_出入库申请子);

                            SqlDataAdapter da3 = new SqlDataAdapter(cmd3);
                            new SqlCommandBuilder(da3);
                            da3.Update(dt_其他出库主);

                            SqlDataAdapter da4 = new SqlDataAdapter(cmd4);
                            new SqlCommandBuilder(da4);
                            da4.Update(dt_其他出库子);

                            SqlDataAdapter da5 = new SqlDataAdapter(cmd5);
                            new SqlCommandBuilder(da5);
                            da5.Update(dt_出入库明细);

                            SqlDataAdapter da6 = new SqlDataAdapter(cmd6);
                            new SqlCommandBuilder(da6);
                            da6.Update(dt_物料);

                            
                            //fun_载入();
                            ts.Commit();
                            MessageBox.Show("撤回成功");

                            dtM.Rows.Remove(dr);
                        }
                        catch (Exception)
                        {
                            ts.Rollback();
                            throw;
                        }





                    }



                }
            
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




        }
    }
}
