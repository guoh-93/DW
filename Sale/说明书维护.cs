using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ERPSale
{
    public partial class 说明书维护 : Form
    {
        public 说明书维护()
        {
            InitializeComponent();
        }


        public 说明书维护(string dr)
        {
            InitializeComponent();
            drM = dr;
        }


        #region 变量

        DataTable CheckFinished;
        string drM = "";
        bool flag = false;
        DataRow drg;
        DataTable dt_说明书, dt_标签;
        string strcon = CPublic.Var.strConn;


        #endregion

        private void fun_flash()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            // flag = false;
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                gridView2.CloseEditor();
                this.BindingContext[dt_说明书].EndCurrentEdit();
                string sql = string.Format("select * from 销售记录成品出库单明细表 where 成品出库单明细号='{0}'", drM.ToString());
                DataRow dr_原始值 = CZMaster.MasterSQL.Get_DataRow(sql, strcon);


                if (flag)
                {

                    DataRow dr = dt_说明书.NewRow();
                    dt_说明书.Rows.Add(dr);
                    dr["成品出库单明细号"] = dr_原始值["成品出库单明细号"];
                    dr["物料编码"] = dr_原始值["物料编码"];
                    dr["物料名称"] = dr_原始值["物料名称"];
                    dr["客户"] = dr_原始值["客户"];
                    dr["客户编号"] = dr_原始值["客户编号"];
                    dr["仓库号"] = dr_原始值["仓库号"];
                    dr["仓库名称"] = dr_原始值["仓库名称"];
                    dr["说明书"] = textBox1.Text;
                    if (textBox2.Text == "")
                    {
                        if (dr["说明书编码"].ToString() == "")
                        {
                            DateTime t = CPublic.Var.getDatetime();
                            dr["说明书编码"] = string.Format("PJB{0}{1:00}{2:00}{3:0000}", t.Year, t.Month,
                                t.Day, CPublic.CNo.fun_得到最大流水号("PJB", t.Year, t.Month));
                        }
                    }
                    else
                    {
                        dr["说明书编码"] = textBox2.Text;
                    }
                    flag = false;
                }
                else
                {
                    DataRow d22r2 = (this.BindingContext[gridControl2.DataSource].Current as DataRowView).Row;
                    string s2ql = string.Format("select * from 销售出库产品说明书表 where ID='{0}'", d22r2["ID"].ToString());
                    dt_说明书 = new DataTable();
                    dt_说明书 = CZMaster.MasterSQL.Get_DataTable(s2ql, strcon);
                    DataRow drsad = dt_说明书.Rows[0];
                    drsad["说明书"] = textBox1.Text;
                    // drsad["说明书编码"] = textBox2.Text;

                    if (textBox2.Text == "")
                    {
                        if (drsad["说明书编码"].ToString() == "")
                        {
                            DateTime t = CPublic.Var.getDatetime();
                            drsad["说明书编码"] = string.Format("PJB{0}{1:00}{2:00}{3:0000}", t.Year, t.Month,
                                t.Day, CPublic.CNo.fun_得到最大流水号("PJB", t.Year, t.Month));
                        }
                    }
                    else
                    {
                        drsad["说明书编码"] = textBox2.Text;
                    }

                    // dt_说明书.AcceptChanges();

                }


                using (SqlDataAdapter da = new SqlDataAdapter("select *  from  销售出库产品说明书表 where 1<>1", strcon))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt_说明书);
                    MessageBox.Show("保存成功");
                    dt_说明书.AcceptChanges();
                    barLargeButtonItem4_ItemClick(null, null);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string sql = string.Format("select * from 销售出库产品说明书表 where 成品出库单明细号 ='{0}'", drM.ToString());
            dt_说明书 = new DataTable();
            dt_说明书 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl2.DataSource = dt_说明书;
            flag = false;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                flag = true;
                fun_flash();
                //DataRow dr = dt_说明书.NewRow();
                //dt_说明书.Rows.Add(dr);
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



                if (MessageBox.Show("确认删除吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    drg = (this.BindingContext[gridControl2.DataSource].Current as DataRowView).Row;
                    if (drg == null)
                    {
                        throw new Exception("未选中任意行不可删除");
                    }
                    drg.Delete();

                    string sql = string.Format("select * from 销售出库产品说明书表 where 1<>1");
                    SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                    new SqlCommandBuilder(da);
                    da.Update(dt_说明书);
                    MessageBox.Show("删除成功");
                    dt_说明书.AcceptChanges();
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gridView2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow drm = (this.BindingContext[gridControl2.DataSource].Current as DataRowView).Row;
            flag = false;
            textBox1.Text = drm["说明书"].ToString();
            textBox2.Text = drm["说明书编码"].ToString();
        }

        private void 说明书维护_Load(object sender, EventArgs e)
        {
            string sql = string.Format("select * from 销售出库产品说明书表 where 成品出库单明细号 ='{0}'", drM.ToString());
            dt_说明书 = new DataTable();
            dt_说明书 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl2.DataSource = dt_说明书;
        }
    }
}
