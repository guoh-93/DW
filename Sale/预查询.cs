using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;

namespace ERPSale
{
    public partial class 预查询 : UserControl
    {
        public 预查询()
        {
            InitializeComponent();
        }
        #region 成员
        //yyyy-MM-dd HH:mm:ss 时间格式
        string UserID = CPublic.Var.LocalUserID;
        string strconn = CPublic.Var.strConn;
        DataTable dtM = new DataTable();
        DataTable dtP;
        DataRow drM;
        DataTable dt_订单明细;
        DataTable dt_销售人员;

        DataTable t_片区 = ERPorg.Corg.fun_业务员片区(CPublic.Var.localUserName); //19-4-4 东屋暂时用不到
        string strConn_FS = CPublic.Var.geConn("FS");
        string cfgfilepath = "";
        #endregion
        private void gridControl1_Click(object sender, EventArgs e)
        {

        }

        private void barEditItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ERPorg.Corg.FlushMemory();
                if (dtP != null)
                {
                    dtP.Clear();
                }
                if (dtM != null)
                {
                    dtM.Clear();
                }
                ///  string ssss = "";
                ///ssss = 
                ///bar_销售订单号.ToString();
                //视图权限
                //      dt_销售人员 = ERPorg.Corg.fun_hr("销售", CPublic.Var.LocalUserID);

                string s_组合 = @"select sz.*,khfl.类别名称 from 销售预订单主表 sz
                                left  join 客户基础信息表 k on k.客户编号=sz.客户编号 
                                left join 客户分类表 khfl  on khfl.客户分类编码=k.客户分类编码 where 1=1";
                string s_条件 = "";
                string mx_条件 = "";

                if (bar_日期_前.EditValue.ToString() != null && bar_日期_后.EditValue.ToString() != null)
                {
                    DateTime dttime1 = Convert.ToDateTime(bar_日期_前.EditValue).Date ;  //日期开始范围
                    DateTime dttime2 = Convert.ToDateTime(bar_日期_后.EditValue).Date.AddDays(1).AddSeconds(-1);  //日期结束范围
                    if (dttime1 > dttime2)
                        throw new Exception("第一个时间不能大于第二个时间！");
                    s_条件 =$"and sz.制单日期>'{dttime1}' and sz.制单日期<'{dttime2}' " ;
                    mx_条件  = $"and sz.制单日期>'{dttime1}' and sz.制单日期<'{dttime2}' ";



                }
                if (bar_单据状态.EditValue.ToString() != "")
                {
                    if (bar_单据状态.EditValue.ToString() == "全部")
                    {

                    }
                    else if (bar_单据状态.EditValue.ToString() == "未完成")
                    {
                        s_条件 += string.Format(" and  sz.完成=0 and sz.作废=0 and sz.关闭=0  ");
                        mx_条件 += $" and  a.完成=0 and a.作废=0 and a.关闭=0 and  sz.完成=0 and sz.作废=0 and sz.关闭=0";

                    }
                    

                }
                s_组合 += s_条件;
                SqlDataAdapter da = new SqlDataAdapter(s_组合, strconn);
                da.Fill(dtM);

                gridControl1.DataSource = dtM;
                string s_Mx = @"select  a.*,sz.备注 as 表头备注,部门名称,sz.审核 from 销售预订单明细表 a
                        left join 销售预订单主表 sz on a.销售预订单号 = sz.销售预订单号 where 1=1 ";
                s_Mx += mx_条件;
                DataTable dt_mx = CZMaster.MasterSQL.Get_DataTable(s_Mx, strconn);
                gridControl3.DataSource = dt_mx;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售单证主表界面_刷新操作");
                throw ex;
            }
        }

        private void 预查询_Load(object sender, EventArgs e)
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
                x.UserLayout(this.panel3, this.Name, cfgfilepath);
                DateTime t = CPublic.Var.getDatetime();
                string sql = "select * from 销售预订单主表 where 1<>1";
                dtM = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                bar_单据状态.EditValue = "未完成";
                bar_日期_前.EditValue = Convert.ToDateTime(t.AddMonths(-3).ToString("yyyy-MM-dd"));
                bar_日期_后.EditValue = Convert.ToDateTime(t.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd"));
                barLargeButtonItem2_ItemClick(null, null);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
        private void refresh_single(string s_单号)
        {
            string sql = string.Format(@"select sz.*,khfl.类别名称 from 销售预订单主表 sz
                                left  join 客户基础信息表 k on k.客户编号=sz.客户编号 
                                left join 客户分类表 khfl  on khfl.客户分类编码=k.客户分类编码 where  销售预订单号='{0}'", s_单号);
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            DataRow[] r_1 = dtM.Select(string.Format("销售预订单号='{0}'", s_单号));
            r_1[0].ItemArray = dt.Rows[0].ItemArray;
        }
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                

                DataRow drM = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                bool bl = false;
                refresh_single(drM["销售预订单号"].ToString());
                if (CPublic.Var.LocalUserTeam == "管理员权限")
                {
                    bl = true;
                }
                else
                {
                    string sql_1 = $"select 在职状态,部门编号 from 人事基础员工表 where 员工号 = '{drM["制单人ID"].ToString()}'";
                    DataTable dt_人事 = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);
                    if (dt_人事.Rows.Count > 0)
                    {
                        if (dt_人事.Rows[0]["在职状态"].ToString() == "离职")
                        {
                            if (dt_人事.Rows[0]["部门编号"].ToString() == CPublic.Var.localUser部门编号)
                            {
                                bl = true;
                            }
                        }
                        else
                        {
                            if (drM["制单人ID"].ToString() == CPublic.Var.LocalUserID)
                            {
                                bl = true;
                            }
                        }
                    }
                }

                barLargeButtonItem7.Enabled = bl;
                string sql = string.Format(@"select  xs.* ,ck.仓库号,ck.仓库名称,dw.库存总数   from 销售预订单明细表   xs
                left  join  基础数据物料信息表 ck  on   ck.物料编码=xs.物料编码
                 left join  仓库物料数量表 dw  on  dw.物料编码=ck.物料编码 and ck.仓库号=dw.仓库号
                 where xs.销售预订单号 ='{0}' order by POS", drM["销售预订单号"].ToString());
                
                dtP = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                dtP.Columns.Add("未转订单数", typeof(decimal));
                foreach (DataRow dr in dtP.Rows)
                {
                    decimal a = decimal.Parse(dr["数量"].ToString()) - decimal.Parse(dr["转换订单数量"].ToString());
                    dr["未转订单数"] = a;
                }
                gridControl2.DataSource = dtP;

                if (e.Button == MouseButtons.Right)
                {
                    if (Convert.ToBoolean(drM["审核"]) == true && (drM["部门名称"].ToString() == CPublic.Var.localUser部门名称 ||CPublic.Var.LocalUserTeam =="管理员权限"))
                    {
                        明细变更ToolStripMenuItem.Visible = true;
                    }
                    else
                    {
                        明细变更ToolStripMenuItem.Visible = false;
                    }
                    contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                    contextMenuStrip1.Tag = gridControl1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 转销售单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try

            {

                DataRow drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
                string sql_查询 = string.Format("select * from 销售预订单主表 where 销售预订单号 = '{0}'", drM["销售预订单号"]);
                DataTable dt_预 = CZMaster.MasterSQL.Get_DataTable(sql_查询, strconn);
                if (dt_预.Rows.Count > 0)
                {
                    //19-8-12审核开启再添加判断
                    //if (Convert.ToBoolean(dt_预.Rows[0]["审核"]) == false)
                    //{
                    //    throw new Exception("该单据未审核，无法转销售单");
                    //}
                    if (Convert.ToBoolean(dt_预.Rows[0]["完成"]) == true)
                    {
                        throw new Exception("该单据已全部完成，无法转销售单");
                    }
                    if (Convert.ToBoolean(dt_预.Rows[0]["关闭"]) == true)
                    {
                        throw new Exception("该单据已关闭，无法转销售单");
                    }
                    if (Convert.ToBoolean(dt_预.Rows[0]["作废"]) == true)
                    {
                        throw new Exception("该单据已作废，无法转销售单");
                    }
                    if (Convert.ToBoolean(dt_预.Rows[0]["审核"]) == false)
                    {
                        throw new Exception("该单据未审核，无法转销售单");
                    }

                }
                //if (bool.Parse( drM["完成"].ToString())==true)
                //{
                //    throw new Exception("当前数据已全部完成，无法转销售单");
                //}
                //if (bool.Parse(drM["关闭"].ToString()) == true)
                //{
                //    throw new Exception("当前数据已关闭，无法转销售单");
                //}
                //if (bool.Parse(drM["作废"].ToString()) == true)
                //{
                //    throw new Exception("当前数据作废，无法转销售单");
                //}


                bool 转 = true;
                string xs = "";
                xs = drM["销售预订单号"].ToString();
                frm销售单证详细界面 fm = new frm销售单证详细界面(xs, drM, dtM, 转);
                // fm.Dock = System.Windows.Forms.DockStyle.Fill;
                CPublic.UIcontrol.AddNewPage(fm, "销售订单");



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ERPSale.ui销售预订单录入 ui = new ui销售预订单录入();
            CPublic.UIcontrol.Showpage(ui, "预订单录入");
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void 转借用单ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try

            {

                DataRow drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
                string sql_查询 = string.Format("select * from 销售预订单主表 where 销售预订单号 = '{0}'", drM["销售预订单号"]);
                DataTable dt_预 = CZMaster.MasterSQL.Get_DataTable(sql_查询, strconn);
                if (dt_预.Rows.Count > 0)
                {
                    //19-8-12审核开启再添加判断
                    //if (Convert.ToBoolean(dt_预.Rows[0]["审核"]) == false)
                    //{
                    //    throw new Exception("该单据未审核，无法转销售单");
                    //}
                    if (Convert.ToBoolean(dt_预.Rows[0]["完成"]) == true)
                    {
                        throw new Exception("该单据已全部完成，无法转借用单");
                    }
                    if (Convert.ToBoolean(dt_预.Rows[0]["关闭"]) == true)
                    {
                        throw new Exception("该单据已关闭，无法转借用单");
                    }
                    if (Convert.ToBoolean(dt_预.Rows[0]["作废"]) == true)
                    {
                        throw new Exception("该单据已作废，无法转借用单");
                    }
                    if (Convert.ToBoolean(dt_预.Rows[0]["审核"]) == false)
                    {
                        throw new Exception("该单据未审核，无法转借用单");
                    }

                }
                //if (bool.Parse(drM["完成"].ToString()) == true)
                //{
                //    throw new Exception("当前数据已全部完成，无法转借还申请");
                //}
                //if (bool.Parse(drM["关闭"].ToString()) == true)
                //{
                //    throw new Exception("当前数据已关闭，无法转借还申请");
                //}
                //if (bool.Parse(drM["作废"].ToString()) == true)
                //{
                //    throw new Exception("当前数据作废，无法转借还申请");
                //}


                bool 转 = true;
                string xs = "";
                xs = drM["销售预订单号"].ToString();

                string sql = @"select  xs.* ,ck.仓库号,ck.仓库名称,dw.库存总数   from 销售预订单明细表   xs
                left  join  基础数据物料信息表 ck  on    ck.物料编码=xs.物料编码
                 left join  仓库物料数量表 dw  on  dw.物料编码=ck.物料编码 and ck.仓库号=dw.仓库号
                 where 1<>1";
                // DataTable dt_渔村明细=
                DataTable dt_预订单明细 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                dt_预订单明细.Columns.Add("未转订单数", typeof(decimal));
                foreach (DataRow dr in dtP.Rows)
                {
                    if (!Convert.ToBoolean(dr["关闭"])&&!Convert.ToBoolean(dr["完成"])&&!Convert.ToBoolean(dr["作废"]))
                    {
                        DataRow dr_预订单明细 = dt_预订单明细.NewRow();
                        dt_预订单明细.Rows.Add(dr_预订单明细);
                        dr_预订单明细["销售预订单号"] = dr["销售预订单号"];
                        dr_预订单明细["销售预订单明细号"] = dr["销售预订单明细号"];
                        dr_预订单明细["物料编码"] = dr["物料编码"];
                        dr_预订单明细["物料名称"] = dr["物料名称"];
                        dr_预订单明细["规格型号"] = dr["规格型号"];
                        dr_预订单明细["预计发货日期"] =Convert.ToDateTime(dr["预计发货日期"]);
                        dr_预订单明细["未转订单数"] =Convert.ToDecimal(dr["未转订单数"]);
                        dr_预订单明细["数量"] = Convert.ToDecimal(dr["数量"]);
                        dr_预订单明细["转换订单数量"] = Convert.ToDecimal(dr["转换订单数量"]);
                        dr_预订单明细["未转数量"] = Convert.ToDecimal(dr["未转数量"]);
                        dr_预订单明细["POS"] = dr["POS"];
                        dr_预订单明细["仓库号"] = dr["仓库号"];
                        dr_预订单明细["仓库名称"] = dr["仓库名称"];
                        dr_预订单明细["计量单位"] = dr["计量单位"];
                    }
                }


                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "MoldMangement.dll")));  //  ERPproduct.dll
                Type outerForm = outerAsm.GetType("MoldMangement.frm借还申请", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统

                object[] drr = new object[4];

                drr[0] = xs;
                drr[1] = drM;
                drr[2] = dt_预订单明细;
                drr[3] = 转;
                //   drr[2] = dr["出入库申请单号"].ToString();
                //Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;
                CPublic.UIcontrol.Showpage(ui, "借还申请");
                //   ui.ShowDialog();




                //frm借还申请 fm = new frm借还申请(xs, drM, dtM, 转);
                //// fm.Dock = System.Windows.Forms.DockStyle.Fill;
                //CPublic.UIcontrol.AddNewPage(fm, "销售订单");



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }






        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            //DataTable dtm = (DataTable)this.gcP.DataSource;
            try

            {

                if (MessageBox.Show("确认作废吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {

                    DataRow dr_预查询 = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
                    string sql_查询 = string.Format("select * from 销售预订单主表 where 销售预订单号 = '{0}'", dr_预查询["销售预订单号"]);
                    DataTable dt_预 = CZMaster.MasterSQL.Get_DataTable(sql_查询, strconn);

                    if (bool.Parse(dt_预.Rows[0]["作废"].ToString()) == true)
                    {
                        throw new Exception("当前单据已作废");
                    }
                    if (bool.Parse(dt_预.Rows[0]["审核"].ToString()) == true)
                    {
                        throw new Exception("当前单据已审核，请联系上级弃审后作废");
                    }

                    string sql = string.Format("select * from  销售预订单明细表 where 销售预订单号='{0}'", dr_预查询["销售预订单号"].ToString());
                    DataTable dt_预查询 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    foreach (DataRow dr in dt_预查询.Rows)
                    {
                        if (Convert.ToBoolean(dr["完成"]) == false)
                        {
                            dr["作废"] = true;
                            dr["作废日期"] = DateTime.Now.ToString();
                        }

                        /// dr["作废"] = true;
                    }
                    string sql_main = string.Format("select * from 销售预订单主表 where 销售预订单号='{0}'", dr_预查询["销售预订单号"].ToString());
                    DataTable dt_main = CZMaster.MasterSQL.Get_DataTable(sql_main, strconn);
                    foreach (DataRow dww in dt_main.Rows)
                    {
                        dww["作废"] = true;
                        dww["作废人员"] = CPublic.Var.localUserName;
                        dww["作废人员ID"] = CPublic.Var.LocalUserID;
                        dww["作废日期"] = DateTime.Now.ToString();
                    }

                    if (bool.Parse(dt_预.Rows[0]["提交审核"].ToString()) == true)
                    {
                        throw new Exception("当前单据已审核，请联系上级弃审后作废");
                    }
                    string sql_审 = string.Format("select * from 单据审核申请表 where 关联单号 = '{0}' and 单据类型 = '销售预订单'", dt_预.Rows[0]["销售预订单号"].ToString());
                    DataTable dt_作废审 = CZMaster.MasterSQL.Get_DataTable(sql_审, strconn);
                    if (dt_作废审.Rows.Count > 0)
                    {
                        dt_作废审.Rows[0]["作废"] = true;

                    }



                    SqlConnection conn = new SqlConnection(strconn);
                    conn.Open();
                    SqlTransaction cktz = conn.BeginTransaction("出库通知修改");
                    try
                    {
                        string sql_z = string.Format(@"select * from 销售预订单明细表 where 1<>1");
                        string sql_主 = string.Format(@"select * from 销售预订单主表 where 1<>1");
                        string sql_审核 = string.Format(@"select * from 单据审核申请表 where 1<>1");

                        SqlCommand cmm_0 = new SqlCommand(sql_z, conn, cktz);

                        SqlCommand cmm_1 = new SqlCommand(sql_主, conn, cktz);
                        SqlCommand cmm_2 = new SqlCommand(sql_审核, conn, cktz);

                        SqlDataAdapter da = new SqlDataAdapter(cmm_0);
                        SqlDataAdapter da1 = new SqlDataAdapter(cmm_1);
                        SqlDataAdapter da2 = new SqlDataAdapter(cmm_2);


                        new SqlCommandBuilder(da);
                        new SqlCommandBuilder(da1);
                        new SqlCommandBuilder(da2);

                        da.Update(dt_预查询);
                        da1.Update(dt_main);
                        if (dt_作废审.Rows.Count > 0)
                        {
                            da2.Update(dt_作废审);
                        }


                        dt_预查询.AcceptChanges();
                        dt_main.AcceptChanges();
                        cktz.Commit();
                        MessageBox.Show("ok");
                        dr_预查询["作废"] = 1;
                        dr_预查询.AcceptChanges();
                    }
                    catch
                    {
                        cktz.Rollback();
                        throw new Exception("保存失败,请重试");
                    }




                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




        }

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try

            {

                //  throw new Exception("正在调试误使用");

                DataRow drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
                string sql_查询 = string.Format("select * from 销售预订单主表 where 销售预订单号 = '{0}'", drM["销售预订单号"]);
                DataTable dt_预 = CZMaster.MasterSQL.Get_DataTable(sql_查询, strconn);
                string sql = string.Format("select * from 销售预订单明细表 where 销售预订单号='{0}'", drM["销售预订单号"]);
                DataTable dt_mx = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                //foreach (DataRow dr  in dt_mx.Rows)
                //{
                //    if (decimal.Parse( dr["转换订单数量"].ToString())>0)
                //    {
                //        throw new Exception("当前预订单有已转过其他单据无法修改");
                //    }

                //}

                if (bool.Parse(dt_预.Rows[0]["完成"].ToString()) == true)

                {
                    throw new Exception("当前订单已完成");

                }

                if (bool.Parse(dt_预.Rows[0]["作废"].ToString()) == true)
                {
                    throw new Exception("当前订单已作废");
                }
                if (CPublic.Var.localUser部门名称 != dt_预.Rows[0]["部门名称"].ToString())
                {
                    throw new Exception("该单据属于其他部门，不可修改");
                }
                if (Convert.ToBoolean(dt_预.Rows[0]["审核"]) == true)
                {
                    throw new Exception("该单据已审核，请联系上级弃审后再修改");
                }


                ui销售预订单录入 fm = new ui销售预订单录入(dt_预, dt_mx);
                // fm.Dock = System.Windows.Forms.DockStyle.Fill;
                CPublic.UIcontrol.AddNewPage(fm, "预订单录入");


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确认弃审吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                    string sql = string.Format("select * from 销售预订单主表 where 销售预订单号 ='{0}'", dr["销售预订单号"]);
                    DataTable dt_主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt_主.Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(dt_主.Rows[0]["完成"]) == true)
                        {
                            throw new Exception("该单据已完成，不能弃审");
                        }
                        if (Convert.ToBoolean(dt_主.Rows[0]["作废"]) == true)
                        {
                            throw new Exception("该单据已作废，不能弃审");
                        }
                        if (Convert.ToBoolean(dt_主.Rows[0]["锁定"]) == true)
                        {
                            throw new Exception("该单据已锁定");
                        }
                        DataTable dt_审核 = new DataTable();
                        dt_审核 = ERPorg.Corg.fun_PA("弃审", "销售预订单弃审申请", dt_主.Rows[0]["销售预订单号"].ToString(), dt_主.Rows[0]["部门名称"].ToString());
                        dt_主.Rows[0]["锁定"] = true;

                        SqlConnection conn = new SqlConnection(strconn);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("pur"); //事务的名称
                        SqlCommand cmd = new SqlCommand("select * from 销售预订单主表 where 1<>1", conn, ts);
                        SqlCommand cmd1 = new SqlCommand("select * from 单据审核申请表 where 1<>1", conn, ts);

                        try
                        {

                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            new SqlCommandBuilder(da);
                            da.Update(dt_主);
                            da = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da);
                            da.Update(dt_审核);
                            ts.Commit();
                            MessageBox.Show("弃审申请成功");
                        }
                        catch
                        {
                            ts.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        [DllImport("user32.dll")]
        public static extern int GetFocus();
        ///获取 当前拥有焦点的控件
        private Control  GetFocusedControl()
        {
            Control c = null;
           // string focusedControl = null;
            IntPtr handle = (IntPtr)GetFocus();

            if (handle == null)
                this.FindForm().KeyPreview = true;
            else
            {
                 c = Control.FromHandle(handle);//这就是
                //focusedControl =
                //c.Parent.TopLevelControl.Name.ToString();
            }

            return c;

        }

        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Control c= GetFocusedControl();
            if (c != null && c.GetType().Equals(gridControl3.GetType()))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                    DevExpress.XtraGrid.GridControl gc = (c) as DevExpress.XtraGrid.GridControl;

                    gc.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            else
            {

                MessageBox.Show("若要导出请先选中要导出的表格(鼠标点一下表格)");
            }
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                DataRow drM = gridView1.GetDataRow(gridView1.FocusedRowHandle);

                bool bl = false;

                if (CPublic.Var.LocalUserTeam == "管理员权限")
                {
                    bl = true;
                }
                else
                {
                    string sql_1 = $"select 在职状态,部门编号 from 人事基础员工表 where 员工号 = '{drM["制单人ID"].ToString()}'";
                    DataTable dt_人事 = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);
                    if (dt_人事.Rows.Count > 0)
                    {
                        if (dt_人事.Rows[0]["在职状态"].ToString() == "离职")
                        {
                            if (dt_人事.Rows[0]["部门编号"].ToString() == CPublic.Var.localUser部门编号)
                            {
                                bl = true;
                            }
                        }
                        else
                        {
                            if (drM["制单人ID"].ToString() == CPublic.Var.LocalUserID)
                            {
                                bl = true;
                            }
                        }
                    }
                }

                barLargeButtonItem7.Enabled = bl;
                //if (drM["审核人"].ToString() == CPublic.Var.LocalUserID)
                //{
                //    barLargeButtonItem7.Enabled = true;
                //}
                //else
                //{
                //    barLargeButtonItem7.Enabled = false;
                //}
                string sql = string.Format(@"select  xs.* ,ck.仓库号,ck.仓库名称,dw.库存总数   from 销售预订单明细表   xs
                left  join  基础数据物料信息表 ck  on    ck.物料编码=xs.物料编码
                 left join  仓库物料数量表 dw  on  dw.物料编码=ck.物料编码 and ck.仓库号=dw.仓库号
                 where xs.销售预订单号 ='{0}' order by POS", drM["销售预订单号"].ToString());
                // DataTable dt_渔村明细=
                dtP = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                dtP.Columns.Add("未转订单数", typeof(decimal));
                foreach (DataRow dr in dtP.Rows)
                {
                    decimal a = decimal.Parse(dr["数量"].ToString()) - decimal.Parse(dr["转换订单数量"].ToString());
                    dr["未转订单数"] = a;
                }
                gridControl2.DataSource = dtP;
            }
            catch
            {

               
            }
        }
 

        private void 明细变更ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try

            {

                //  throw new Exception("正在调试误使用");

                DevExpress.XtraGrid.GridControl g =contextMenuStrip1.Tag as DevExpress.XtraGrid.GridControl;
                DataRow drM = (this.BindingContext[g.DataSource].Current as DataRowView).Row;
                string sql_查询 = string.Format("select * from 销售预订单主表 where 销售预订单号 = '{0}'", drM["销售预订单号"]);
                DataTable dt_预 = CZMaster.MasterSQL.Get_DataTable(sql_查询, strconn);
                string sql = string.Format("select * from 销售预订单明细表 where 销售预订单号='{0}' and 完成 = 0 and 作废 = 0 and 关闭 =0 ", drM["销售预订单号"]);
                DataTable dt_mx = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                sql = $"select * from 销售预订单变更申请 where  销售预订单号='{ drM["销售预订单号"]}' and 作废 = 0 and 审核 = 0";
                DataTable dt_变更申请 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                
                
                if (Convert.ToBoolean(dt_预.Rows[0]["锁定"]) == true)
                {
                    throw new Exception("该单据已锁定，不可变更");
                }
                if (Convert.ToBoolean(dt_预.Rows[0]["完成"]) == true)
                {
                    throw new Exception("该单据已完成，不可变更");
                }
                if (Convert.ToBoolean(dt_预.Rows[0]["作废"]) == true)
                {
                    throw new Exception("该单据已作废，不可变更");
                }
                if (dt_变更申请.Rows.Count>0)
                {        
                    throw new Exception("存在未审核的变更申请单，请确认");                     
                }
                DataTable dt_mx_copy = dt_mx.Copy();

                dt_mx_copy.Columns.Add("更改数量", typeof(decimal));
                foreach (DataRow dr in dt_mx_copy.Rows)
                {
                    dr["更改数量"] = Convert.ToDecimal(dr["未转数量"]);
                }

                ui_预订单变更 fm = new ui_预订单变更(dt_预, dt_mx_copy);
                // fm.Dock = System.Windows.Forms.DockStyle.Fill;
                CPublic.UIcontrol.AddNewPage(fm, "预订单变更");


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gridView3_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow  drM = gridView3.GetDataRow(gridView3.FocusedRowHandle);
            if (e.Button == MouseButtons.Right)
            {
                if (Convert.ToBoolean(drM["审核"]) == true && (drM["部门名称"].ToString() == CPublic.Var.localUser部门名称 || CPublic.Var.LocalUserTeam == "管理员权限"))
                {
                    明细变更ToolStripMenuItem.Visible = true;
                }
                else
                {
                    明细变更ToolStripMenuItem.Visible = false;
                }
                contextMenuStrip1.Show(gridControl3, new Point(e.X, e.Y));
                contextMenuStrip1.Tag = gridControl3;
                转销售单ToolStripMenuItem.Visible = false;
                转借用单ToolStripMenuItem.Visible = false;
                修改ToolStripMenuItem.Visible = false;

            }

        }
    }
}
