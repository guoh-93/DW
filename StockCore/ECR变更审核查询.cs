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

namespace StockCore
{
    public partial class ECR变更审核查询 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dt_审核;
        public ECR变更审核查询()
        {
            InitializeComponent();
        }

        private void ECR变更审核查询_Load(object sender, EventArgs e)
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
                x.UserLayout(this.panel1, this.Name, cfgfilepath);
                DateTime t = CPublic.Var.getDatetime().Date;
                barEditItem2.EditValue = t.AddDays(1);
                barEditItem1.EditValue = t.AddMonths(-3);
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }           
        }

        private void fun_load()
        {
            string sql = "select sh.*,sq.申请日期 from  ECR变更申请审核表 sh left join  ECR变更申请单主表 sq on sh.申请单号 = sq.申请单号  where sh.审核 = 1 ";

            if (CPublic.Var.LocalUserID != "admin" && CPublic.Var.LocalUserTeam != "管理员权限")
            {
                sql = sql + "and 审核部门 = '" + CPublic.Var.localUser部门名称 +"'";
            }
            dt_审核 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gridControl1.DataSource = dt_审核;
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (dr == null) return;
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 查询明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            StockCore.ui_ECR变更申请 ui = new ui_ECR变更申请(dr);
            CPublic.UIcontrol.Showpage(ui, "申请明细查询");
        }

        private void 添加审核意见ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                frm_添加意见 frm = new frm_添加意见(dr);
                frm.Text = "添加意见";
                frm.ShowDialog();
                if (frm.bl_保存 == true)
                {
                    string sql = $"select * from ECR变更申请审核表 where 申请单号 = '{dr["申请单号"].ToString()}' and 审核部门 = '{dr["审核部门"].ToString()}'";
                    DataTable dt_审核意见 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt_审核意见.Rows.Count>0)
                    {
                        dt_审核意见.Rows[0]["审核意见"] = dt_审核意见.Rows[0]["审核意见"]+","+ frm.str_意见;
                        SqlDataAdapter da = new SqlDataAdapter("select * from ECR变更申请审核表 where 1<>1",strconn);
                        new SqlCommandBuilder(da);
                        da.Update(dt_审核意见);
                        MessageBox.Show("添加意见成功");

                        sql = $@"select sh.*,sq.申请日期 from  ECR变更申请审核表 sh left join  ECR变更申请单主表 sq on sh.申请单号 = sq.申请单号 
                                where sh.审核 = 1 and sh.申请单号 = '{dr["申请单号"].ToString()}' and sh.审核部门 = '{dr["审核部门"].ToString()}'";
                        dt_审核意见 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                        dr.ItemArray = dt_审核意见.Rows[0].ItemArray;

                        dr.AcceptChanges();
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
