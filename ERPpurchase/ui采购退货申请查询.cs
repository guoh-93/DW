using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ERPpurchase
{
    public partial class ui采购退货申请查询 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dtM;
        DataTable dtP;
        DataTable dt_all_mx;
        public ui采购退货申请查询()
        {
            InitializeComponent();
        }

        private void ui采购退货申请查询_Load(object sender, EventArgs e)
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
                barEditItem6.EditValue = Convert.ToDateTime(t.ToString("yyyy-MM-dd"));
                barEditItem3.EditValue = Convert.ToDateTime(t.AddMonths(-1).ToString("yyyy-MM-dd"));

                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void fun_load()
        {
            DateTime t = Convert.ToDateTime(barEditItem3.EditValue).Date;
            DateTime t1 = Convert.ToDateTime(barEditItem6.EditValue).Date.AddDays(1).AddSeconds(-1);
            string sql = $"select * from 采购退货申请主表 where   申请日期>'{t}' and 申请日期<'{t1}' ";
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gridControl1.DataSource = dtM;

            sql = $@"select  a.*,b.作废,b.作废日期,作废人,类型  from 采购退货申请子表 a  
             left join 采购退货申请主表  b  on a.退货申请单号=b.退货申请单号 
             where   b.申请日期>'{t}' and b.申请日期<'{t1}'";
            dt_all_mx = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gridControl3.DataSource = dt_all_mx;
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (dr == null) return;
                fun_mx(dr["退货申请单号"].ToString());
                if (e != null && e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                    gridView1.CloseEditor();
                    this.BindingContext[dtM].EndCurrentEdit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_mx(string v)
        {
            string sql = string.Format("select * from 采购退货申请子表 where 退货申请单号 = '{0}'", v);
            dtP = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gridControl2.DataSource = dtP;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ERPorg.Corg.FlushMemory();
                fun_load();
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
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string s = string.Format("select * from 采购退货申请子表 where 退货申请单号='{0}'", dr["退货申请单号"]);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                if (temp.Rows.Count > 0)
                {
                    if (temp.Rows[0]["完成"].Equals(true))
                    {

                        throw new Exception("该单据已退货,不可修改");
                    }
                    if (temp.Rows[0]["作废"].Equals(true))
                    {

                        throw new Exception("该单据已作废,不可修改");
                    }
                }
                else
                {
                    throw new Exception("单据异常,刷新后重试");

                }
                ui采购退货申请 ui = new ui采购退货申请(dr);
                CPublic.UIcontrol.Showpage(ui, "采购退货申请");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string s = string.Format("select * from 采购退货申请主表 where 退货申请单号='{0}'", dr["退货申请单号"]);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = string.Format("select * from 采购退货申请子表 where 退货申请单号='{0}'", dr["退货申请单号"]);
                DataTable dt_子 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                if (temp.Rows.Count > 0)
                {
                    if (MessageBox.Show("确认将该条记录作废", "警告", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        if (Convert.ToBoolean(temp.Rows[0]["作废"]) == true)
                        {
                            throw new Exception("该单据已作废");
                        }
                        if (Convert.ToBoolean(temp.Rows[0]["完成"]) == true)
                        {
                            throw new Exception("该单据已退货，不可作废");
                        }
                        temp.Rows[0]["作废"] = true;
                        temp.Rows[0]["作废日期"] = t;
                        temp.Rows[0]["作废人"] = CPublic.Var.localUserName;
                        foreach (DataRow dr_子 in dt_子.Rows)
                        {
                            dr_子["作废"] = true;
                        }
                        SqlConnection conn = new SqlConnection(strconn);
                        conn.Open();
                        SqlTransaction cgthsq = conn.BeginTransaction("采购退货申请");
                        try
                        {

                            string sql = "select * from 采购退货申请主表 where 1<>1";
                            SqlCommand cmd = new SqlCommand(sql, conn, cgthsq);
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                new SqlCommandBuilder(da);
                                da.Update(temp);
                            }
                            sql = "select * from  采购退货申请子表 where 1<>1";
                            cmd = new SqlCommand(sql, conn, cgthsq);
                            cgthsq.Commit();
                            MessageBox.Show("作废成功");
                            fun_load();
                        }
                        catch (Exception ex)
                        {
                            cgthsq.Rollback();
                            throw ex;
                        }
                    }

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if(tabControl1.SelectedTab.Text=="采购退货明细")
                {
                    barLargeButtonItem3.Enabled = false;
                }
                else
                {
                    barLargeButtonItem3.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
