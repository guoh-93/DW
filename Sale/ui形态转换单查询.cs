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
using System.Reflection;

namespace ERPSale
{
    public partial class ui形态转换单查询 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dtM;
        DataTable dtP;
        public ui形态转换单查询()
        {
            InitializeComponent();
        }

        private void 形态转换单查询_Load(object sender, EventArgs e)
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
                x.UserLayout(this.tabControl1, this.Name, cfgfilepath);
                DateTime t = CPublic.Var.getDatetime().Date;
                t = t.AddDays(1).AddSeconds(-1);
                barEditItem2.EditValue = t;
                barEditItem1.EditValue = t.AddMonths(-1).Date;
                fun_load();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

        private void fun_load()
        {
            DateTime t = Convert.ToDateTime(barEditItem1.EditValue).Date;
            DateTime t1 = Convert.ToDateTime(barEditItem2.EditValue).Date.AddDays(1).AddSeconds(-1);
            string sql = string.Format("select * from 销售形态转换主表 where   申请日期>'{0}' and 申请日期<'{1}' ", t, t1);
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dtM;

            string x =$@" select  a.*,b.申请人,b.申请日期,b.审核人员,b.审核日期,b.审核,b.提交审核,部门名称,b.备注 as 表头备注 from 销售形态转换子表  a
            left join 销售形态转换主表   b on a.形态转换单号=b.形态转换单号 where b.申请日期>'{t}' and b.申请日期<'{t1}' " ;
            DataTable dtP = CZMaster.MasterSQL.Get_DataTable(x, strcon);
            gridControl3.DataSource = dtP;

        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string s = string.Format("select * from 销售形态转换主表 where 形态转换单号='{0}' and 审核=0", dr["形态转换单号"]);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                if (temp.Rows.Count > 0)
                {
                    if (MessageBox.Show("确认将该条记录取消提交审核", "警告", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        if(Convert.ToBoolean(temp.Rows[0]["作废"]) == true)
                        {
                            throw new Exception("该单据已作废");
                        }
                        temp.Rows[0]["提交审核"] = 0;
                        s = string.Format("select  * from 单据审核申请表 where 关联单号='{0}'", dr["形态转换单号"]);
                        DataTable dtt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                        if (dtt.Rows.Count > 0)
                        {
                            dtt.Rows[0].Delete();
                        }
                        //事务的名称
                        try
                        {
                            string sql = "select * from 销售形态转换主表 where 1<>1";
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


        //修改
        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string s = string.Format("select * from 销售形态转换主表 where 形态转换单号='{0}'", dr["形态转换单号"]);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                if (temp.Rows.Count > 0)
                {
                    if (temp.Rows[0]["审核"].Equals(true))
                    {

                        throw new Exception("该单据已通过审核,不可修改");
                    }
                    else if (temp.Rows[0]["提交审核"].Equals(true))
                    {
                        throw new Exception("该单据已提交审核,不可修改,撤销后再试");
                    }
                    else if (temp.Rows[0]["作废"].Equals(true))
                    {
                        throw new Exception("该单据已作废,不可修改");
                    }
                }
                else
                {
                    throw new Exception("单据异常,刷新后重试");

                }
                ui形态转换单 ui = new ui形态转换单(dr);
                CPublic.UIcontrol.Showpage(ui, "形态转换申请修改");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        //private void 作废ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
        //        if (MessageBox.Show("确认将该条记录作废", "警告", MessageBoxButtons.OKCancel) == DialogResult.OK)
        //        {
        //            string s = string.Format("select * from 销售形态转换单表 where 形态转换单号='{0}'", dr["形态转换单号"]);
        //            DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
        //            if (temp.Rows.Count > 0)
        //            {
        //                if (temp.Rows[0]["作废"].Equals(true))
        //                {

        //                    throw new Exception("已作废,无需作废");
        //                }
        //            }
        //            else
        //            {
        //                throw new Exception("单据异常,刷新后重试");

        //            }
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        //private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        //{
        //    DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
        //   // fun_mx(dr["申请单号"].ToString());
        //    if (e != null && e.Button == MouseButtons.Right)
        //    {
        //        contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
        //        gridView1.CloseEditor();
        //        this.BindingContext[dtM].EndCurrentEdit();
        //    }
        //}

        private void gridView1_RowCellClick_1(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                fun_mx(dr["形态转换单号"].ToString());
                if (e != null && e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                    gridView1.CloseEditor();
                    this.BindingContext[dtM].EndCurrentEdit();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_mx(string s)
        {
            string x = string.Format(@" select  * from 销售形态转换子表 where 形态转换单号='{0}' ", s);
            DataTable dt_mx = CZMaster.MasterSQL.Get_DataTable(x, strcon);
            gridControl2.DataSource = dt_mx;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dras = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;

         
            Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
            Type outerForm = outerAsm.GetType("ERPreport.形态转换", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
                string x = string.Format(@" select  * from 销售形态转换子表 where 形态转换单号='{0}' ", dras["形态转换单号"].ToString());
                DataTable dtm = CZMaster.MasterSQL.Get_DataTable(x, strcon);

            object[] drr = new object[2];

            drr[0] = dras;
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

        private void 作废ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string s = string.Format("select * from 销售形态转换主表 where 形态转换单号='{0}'", dr["形态转换单号"]);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                if (temp.Rows.Count > 0)
                {
                    if (temp.Rows[0]["审核"].Equals(true))
                    {

                        throw new Exception("已通过审核,不可作废");
                    }
                    else if (temp.Rows[0]["提交审核"].Equals(true))
                    {
                        throw new Exception("已提交审核,不可作废,撤销后再试");
                    }
                }
                else
                {
                    throw new Exception("单据异常,刷新后重试");

                }
                if (MessageBox.Show("确认作废此单据", "警告", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    temp.Rows[0]["作废"] = true;
                    temp.Rows[0]["作废日期"] = t;
                    temp.Rows[0]["作废人员"] = CPublic.Var.localUserName;
                    temp.Rows[0]["作废人员ID"] = CPublic.Var.LocalUserID;

                    string sql = "select * from  销售形态转换主表 where 1<>1";
                    SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                    new SqlCommandBuilder(da);
                    da.Update(temp);
                    MessageBox.Show("作废成功");
                    dr["作废"] = true;
                    dr.AcceptChanges();
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
                if (tabControl1.SelectedTab.Text == "形态转换明细")
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
