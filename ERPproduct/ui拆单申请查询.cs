using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
namespace ERPproduct
{
    public partial class ui拆单申请查询 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        DataTable dtM;
        string cfgfilepath = "";
        public ui拆单申请查询()
        {
            InitializeComponent();
            DateTime t = CPublic.Var.getDatetime().Date;
            t = t.AddDays(1).AddSeconds(-1);
            barEditItem2.EditValue = t;
            barEditItem1.EditValue = t.AddMonths(-1).Date;
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        DataTable dtP;
        private void fun_load()
        {
            DateTime t = Convert.ToDateTime(barEditItem1.EditValue).Date;
            DateTime t1 = Convert.ToDateTime(barEditItem2.EditValue).Date.AddDays(1).AddSeconds(-1);
            string sql = string.Format("select * from 拆单申请主表 where   申请日期>'{0}' and 申请日期<'{1}' ", t, t1);
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dtM;
            sql= $@"select   a.*,b.申请人,b.申请日期,b.部门名称 ,b.备注 as 表头备注,b.审核,b.审核人员,b.审核日期,b.提交审核   from 拆单申请子表 a
  left join 拆单申请主表 b on a.申请单号 = b.申请单号 where  b.申请日期>'{t}' and b.申请日期<'{t1}' " ;

            dtP = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl3.DataSource = dtP;
        }

        

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (dr == null) return;
                fun_mx(dr["申请单号"].ToString());
                if (e != null && e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                    gridView1.CloseEditor();
                    this.BindingContext[dtM].EndCurrentEdit();
                }
            }
            catch  
            {
 
            }
          
        }

        private void fun_mx(string s)
        {
            string x = string.Format(@" select  * from 拆单申请子表 where 申请单号='{0}' ", s);
            DataTable dt_mx = CZMaster.MasterSQL.Get_DataTable(x, strcon);
            gridControl2.DataSource = dt_mx;
        }

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string s = string.Format("select * from 拆单申请主表 where 申请单号='{0}'", dr["申请单号"]);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                if (temp.Rows.Count > 0)
                {
                    if (temp.Rows[0]["审核"].Equals(true))
                    {

                        throw new Exception("已通过审核,不可修改");
                    }
                    else if (temp.Rows[0]["提交审核"].Equals(true))
                    {
                        throw new Exception("已提交审核,不可修改,撤销后再试");
                    }
                }
                else
                {
                    throw new Exception("单据异常,刷新后重试");

                }
                ui拆单申请 ui = new ui拆单申请(dr);
                CPublic.UIcontrol.Showpage(ui, "拆单申请修改");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string s = string.Format("select * from 拆单申请主表 where 申请单号='{0}' and 审核=0", dr["申请单号"]);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                if (temp.Rows.Count > 0)
                {
                    if (MessageBox.Show("确认将该条记录取消提交审核", "警告", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        if (Convert.ToBoolean(temp.Rows[0]["作废"]) == true)
                        {
                            throw new Exception("该单据已作废");
                        }
                        temp.Rows[0]["提交审核"] = 0;
                        s = string.Format("select  * from 单据审核申请表 where 关联单号='{0}'", dr["申请单号"]);
                        DataTable dtt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                        if (dtt.Rows.Count > 0)
                        {
                            dtt.Rows[0].Delete();
                        }
                        //事务的名称
                        try
                        {
                            string sql = "select * from 拆单申请主表 where 1<>1";
                            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                            new SqlCommandBuilder(da);
                            da.Update(temp);
                            sql = "select * from 单据审核申请表 where 1<>1";
                            da = new SqlDataAdapter(sql, strcon);
                            new SqlCommandBuilder(da);
                            da.Update(dtt);
                            dr["提交审核"] = false;
                            dr.AcceptChanges();
                            MessageBox.Show("撤销成功");

                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show("操作失败" + " " + ex.Message);
                        }
                    }
                }
                else
                {
                    throw new Exception("单据状态已审核");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ui拆单申请查询_Load(object sender, EventArgs e)
        {
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(this.tabControl1, this.Name, cfgfilepath);
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (tabControl1.SelectedTab.Text == "拆单申请明细")
                {
                    barLargeButtonItem1.Enabled = false;
                }
                else
                {
                    barLargeButtonItem1.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
