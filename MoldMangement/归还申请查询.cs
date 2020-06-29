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

namespace MoldMangement
{
    public partial class 归还申请查询 : UserControl
    {
        public 归还申请查询()
        {
            InitializeComponent();
        }
        string strconn = CPublic.Var.strConn;
        DataTable dtM ;
        private void 归还申请查询_Load(object sender, EventArgs e)
        {
            DateTime t = CPublic.Var.getDatetime();
            barEditItem3.EditValue = "全部";
            barEditItem2.EditValue = t.Date.AddDays(1).AddSeconds(-1);
            barEditItem1.EditValue = t.Date.AddDays(-15);
           // bar_单据状态.EditValue = "未归还";
            fun_载入();
        }


        private void fun_载入()
        {
            try
            {
                if (dtM != null)
                {
                    dtM.Clear();
                }
                DateTime t1 = Convert.ToDateTime(barEditItem1.EditValue).Date;
                DateTime t2 = Convert.ToDateTime(barEditItem2.EditValue).Date.AddDays(1).AddSeconds(-1);
                string s_条件 = string.Format(" and 归还申请日期>'{0}' and 归还申请日期<'{1}'", t1, t2);
                if (barEditItem3.EditValue.ToString() != "")
                {
                    if (barEditItem3.EditValue.ToString() == "全部")
                    {

                    }
                    if (barEditItem3.EditValue.ToString() == "已完成")
                    {
                        s_条件 = s_条件 + string.Format("and  归还完成='1'");

                    }

                    if (barEditItem3.EditValue.ToString() == "未完成")
                    {
                        s_条件 = s_条件 + string.Format("and  归还完成='0'");

                    }


                }


                string sql = "select * from 归还申请主表 where  锁定 = 0 " + s_条件;
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                dtM = new DataTable();
                da.Fill(dtM); gcM.DataSource = dtM;
            }
            catch (Exception ex)
            {
                //CZMaster.MasterLog.WriteLog(ex.Message, "退货申请主表_刷新操作");
                throw ex;
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ERPorg.Corg.FlushMemory();
            fun_载入();
        }

        private void gvM_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow r = gvM.GetDataRow(gvM.FocusedRowHandle);
                fun_detail(r["归还批号"].ToString());


          
                    if (e.Button == MouseButtons.Right)
                    {
                        contextMenuStrip1.Show(gcM, new Point(e.X, e.Y));
                    }
 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void fun_detail(string str_单号)
        {
            string sql = string.Format(@"select a.*,b.库存总数,b.仓库名称,b.货架描述 as 默认货架描述  from 归还申请子表 a 
                  left join 仓库物料数量表 b on a.物料编码 = b.物料编码 and a.仓库号=b.仓库号
                where  归还批号 = '{0}' ", str_单号);
            DataTable dtP = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtP);

            if (dtP.Rows.Count<=0)
            {
                sql = string.Format("select * from 归还申请子表 where 归还批号='{0}' ",str_单号);
                dtP = new DataTable();
                dtP = CZMaster.MasterSQL.Get_DataTable(sql,strconn);
            }

            gcP.DataSource = dtP;

        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {


            try
            {
                DataRow drM = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;
                DataTable dtm = (DataTable)this.gcP.DataSource;
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
                Type outerForm = outerAsm.GetType("ERPreport.归还申请单", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
                //CPublic.UIcontrol.Showpage(ui, t.Rows[0]["打开界面名称"].ToString());
                object[] drr = new object[2];

                drr[0] = drM;
                drr[1] = dtm;
                //   drr[2] = dr["出入库申请单号"].ToString();
                Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                //  UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
                ui.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gvM_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
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
            CPublic.UIcontrol.ClosePage();
        }

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                if (Convert.ToBoolean(dr["归还完成"]))
                {
                    throw new Exception("当前单据已全部归还"); 
                }
                else if (Convert.ToBoolean( dr["作废"]) )
                {
                    throw new Exception("当前单据已作废");
                }
                MoldMangement.ui归还申请流程 frm = new MoldMangement.ui归还申请流程(dr["归还批号"].ToString());
                CPublic.UIcontrol.AddNewPage(frm, "归还信息修改");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                DateTime t = CPublic.Var.getDatetime();
                if (MessageBox.Show(string.Format("是否确认作废此单据"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string sql = string.Format("select * from 归还申请主表 where 归还批号 ='{0}'", dr["归还批号"]);
                    DataTable dt_归还主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt_归还主.Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(dt_归还主.Rows[0]["归还完成"]) == true)
                        {
                            throw new Exception("该单据已归还完成，不可作废");
                        }
                        if (Convert.ToBoolean(dt_归还主.Rows[0]["作废"]) == true)
                        {
                            throw new Exception("该单据已作废");
                        }
                        dt_归还主.Rows[0]["作废"] = true;
                        dt_归还主.Rows[0]["作废时间"] = t;

                    }                   
                    sql = string.Format("select * from 归还申请子表 where 归还批号 ='{0}'", dr["归还批号"]);
                    DataTable dt_归还子 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    sql = string.Format("select * from 借还申请表附表 where 申请批号 = '{0}'",dt_归还主.Rows[0]["申请批号"].ToString());
                    DataTable dt_借还附 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt_归还子.Rows.Count > 0)
                    {
                        foreach(DataRow dr1 in dt_归还子.Rows)
                        {
                            if (Convert.ToBoolean(dr1["归还完成"]) == true|| Convert.ToBoolean(dr1["作废"]))
                            {

                            }
                            else
                            {
                                DataRow[] dr_借还附 = dt_借还附.Select(string.Format("申请批号明细 = '{0}'", dr1["申请批号明细"].ToString()));
                                if (dr_借还附.Length > 0)
                                {
                                    dr_借还附[0]["正在申请数"] = Convert.ToDecimal(dr_借还附[0]["正在申请数"]) - Convert.ToDecimal(dr1["需归还数量"]);
                                }
                                dr1["作废"] = true;
                                //dr1["归还完成"] = true;
                            }
                        }
                    }
                    SqlConnection conn = new SqlConnection(strconn);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("作废");
                    try
                    {

                        //事务的名称
                        SqlCommand cmd = new SqlCommand("select * from 归还申请主表 where 1<>1", conn, ts);
                        SqlCommand cmd1 = new SqlCommand("select * from 归还申请子表 where 1<>1", conn, ts);
                        SqlCommand cmd2 = new SqlCommand("select * from 借还申请表附表 where 1<>1", conn, ts);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da);
                        da.Update(dt_归还主);
                        da = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da);
                        da.Update(dt_归还子);
                        da = new SqlDataAdapter(cmd2);
                        new SqlCommandBuilder(da);
                        da.Update(dt_借还附);
                        ts.Commit();
                        MessageBox.Show("作废成功");
                    }
                    catch
                    {
                        ts.Rollback();
                        MessageBox.Show("作废失败");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
