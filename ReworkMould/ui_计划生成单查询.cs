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

namespace ReworkMould
{
    public partial class ui_计划生成单查询 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dt_计划单;
        DataTable dt_计划单明细;
        public ui_计划生成单查询()
        {
            InitializeComponent();
        }

        private void ui_计划生成单查询_Load(object sender, EventArgs e)
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
                x.UserLayout(panel1, this.Name, cfgfilepath);
                DateTime t1 = CPublic.Var.getDatetime().Date.AddMonths(-3);
                DateTime t2 = CPublic.Var.getDatetime();

                barEditItem1.EditValue = t1;
                barEditItem2.EditValue = t2;

                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_load()
        {
            DateTime t1 = Convert.ToDateTime(barEditItem1.EditValue);
            DateTime t2 = Convert.ToDateTime(barEditItem2.EditValue);
            if (t1>t2)
            {
                throw new Exception("时间输入有误，请确认");
            }
            string sql = $"select 计划单号,计划生成人,计划生成日期,关闭,关闭人 from 主计划计划生成单_制令 where 计划生成日期>='{t1}' and 计划生成日期 <='{t2}' group by 计划单号,计划生成人,计划生成日期,关闭,关闭人 ";
            dt_计划单 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dt_计划单;
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (dr == null) return;
                string sql = $"select * from 主计划计划生成单_制令 where 计划单号 = '{dr["计划单号"]}'";
                dt_计划单明细 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                if (!dt_计划单明细.Columns.Contains("选择"))
                {
                    DataColumn dc = new DataColumn("选择", typeof(bool));
                    dc.DefaultValue = 0;
                    dt_计划单明细.Columns.Add(dc);
                }
                gridControl2.DataSource = dt_计划单明细;
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

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string sql = $"select * from 主计划计划生成单_制令 where 计划单号 = '{dr["计划单号"]}'";
                dt_计划单明细 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                if (!dt_计划单明细.Columns.Contains("选择"))
                {
                    DataColumn dc = new DataColumn("选择", typeof(bool));
                    dc.DefaultValue = 0;
                    dt_计划单明细.Columns.Add(dc);
                }
                gridControl2.DataSource = dt_计划单明细;
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
                gridView2.CloseEditor();
                this.ActiveControl = null;
                fun_check();
                DataView dv = new DataView(dt_计划单明细);
                dv.RowFilter = "选择 = 1";
                DataTable dt = dv.ToTable();
                ui_计划池转制令 ui = new ui_计划池转制令(dt);
                CPublic.UIcontrol.Showpage(ui, "转制令确认");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_check()
        {
            DataRow[] dr = dt_计划单明细.Select("选择 = true");
            if (dr.Length == 0) throw new Exception("未选择明细，请确认");
        }

        private void gridView2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                if (dr == null) return;
                if (Convert.ToBoolean(dr["关闭"]))
                {
                    gridColumn26.OptionsColumn.ReadOnly = true;
                    gridColumn26.OptionsColumn.AllowEdit = false;
                }
                else
                {
                    if (Convert.ToDecimal(dr["计划数量"]) - Convert.ToDecimal(dr["已转数量"]) <= 0)
                    {
                        gridColumn26.OptionsColumn.ReadOnly = true;
                        gridColumn26.OptionsColumn.AllowEdit = false;

                    }
                    else
                    {
                        gridColumn26.OptionsColumn.ReadOnly = false;
                        gridColumn26.OptionsColumn.AllowEdit = true;
                    }
                }
                
                //if (e.Button == MouseButtons.Right)
                //{
                //    contextMenuStrip2.Show(gridControl2, new Point(e.X, e.Y));
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 关闭ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (dr == null) return;
                string sql = $"select * from 主计划计划生成单_制令 where 计划单号 = '{dr["计划单号"]}'";
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

                if (MessageBox.Show(string.Format("该销售单是否确认撤销提交？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    foreach (DataRow dr_1 in dt.Rows)
                    {
                        dr_1["关闭人"] = CPublic.Var.localUserName;
                        dr_1["关闭"] = true;
                        dr_1["关闭日期"] = t;
                    }

                    SqlConnection conn = new SqlConnection(strcon);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("关闭");
                    try
                    {
                        //主计划明细表
                        sql = "select * from 主计划计划生成单_制令 where 1<>1";
                        SqlCommand cmm = new SqlCommand(sql, conn, ts);
                        SqlDataAdapter da = new SqlDataAdapter(cmm);
                        new SqlCommandBuilder(da);
                        da.Update(dt);

                        ts.Commit();
                        MessageBox.Show("关闭成功");
                        fun_load();

                    }
                    catch (Exception ex)
                    {
                        ts.Rollback();
                        throw new Exception(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 关闭明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
