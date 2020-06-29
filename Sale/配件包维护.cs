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
    public partial class 配件包维护 : Form
    {
        public 配件包维护()
        {
            InitializeComponent();
        }

        public 配件包维护(string dr)
        {
            InitializeComponent();
            drM = dr;
        }

        #region 变量
      
        DataTable CheckFinished;
        string drM = "";
        bool flag = false;
        DataRow drg;
        DataTable dt_配件, dt_说明书, dt_标签;
        string strcon = CPublic.Var.strConn;


        #endregion

        private void fun_flash()
        {
            textBox1.Text = "";
            textBox2.Text = "";
           // flag = false;
        }
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                flag = true;
                fun_flash();
                //DataRow dr = dt_配件.NewRow();
                //dt_配件.Rows.Add(dr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 配件包维护_Load(object sender, EventArgs e)
        {
            string sql = string.Format("select * from 销售出库产品配件表 where 成品出库单明细号 ='{0}'", drM.ToString());
            dt_配件 = new DataTable();
            dt_配件 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dt_配件;
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
               


                if (MessageBox.Show("确认删除吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    drg = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
                    if (drg == null)
                    {
                        throw new Exception("未选中任意行不可删除");
                    }
                    drg.Delete();

                    string sql = string.Format("select * from 销售出库产品配件表 where 1<>1");
                    SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                    new SqlCommandBuilder(da);
                    da.Update(dt_配件);
                    MessageBox.Show("删除成功");
                    dt_配件.AcceptChanges();
                }



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
                gridView1.CloseEditor();
                this.BindingContext[dt_配件].EndCurrentEdit();
                string sql = string.Format("select * from 销售记录成品出库单明细表 where 成品出库单明细号='{0}'", drM.ToString());
                DataRow dr_原始值 = CZMaster.MasterSQL.Get_DataRow(sql, strcon);


                if (flag)
                {

                    DataRow dr = dt_配件.NewRow();
                    dt_配件.Rows.Add(dr);
                    dr["成品出库单明细号"] = dr_原始值["成品出库单明细号"];
                    dr["物料编码"] = dr_原始值["物料编码"];
                    dr["物料名称"] = dr_原始值["物料名称"];
                    dr["客户"] = dr_原始值["客户"];
                    dr["客户编号"] = dr_原始值["客户编号"];
                    dr["仓库号"] = dr_原始值["仓库号"];
                    dr["仓库名称"] = dr_原始值["仓库名称"];
                    dr["配件名称"] = textBox1.Text;
                    if (textBox2.Text == "")
                    {
                        if (dr["配件编码"].ToString() == "")
                        {
                            DateTime t = CPublic.Var.getDatetime();
                            dr["配件编码"] = string.Format("PJB{0}{1:00}{2:00}{3:0000}", t.Year, t.Month,
                                t.Day, CPublic.CNo.fun_得到最大流水号("PJB", t.Year, t.Month));
                        }
                    }
                    else
                    {
                        dr["配件编码"] = textBox2.Text;
                    }
                    flag = false;
                }
                else
                {
                    DataRow d22r2 = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
                    string s2ql = string.Format("select * from 销售出库产品配件表 where ID='{0}'", d22r2["ID"].ToString());
                    dt_配件 = new DataTable();
                    dt_配件 = CZMaster.MasterSQL.Get_DataTable(s2ql, strcon);
                    DataRow drsad = dt_配件.Rows[0];
                    drsad["配件名称"] = textBox1.Text;
                   // drsad["配件编码"] = textBox2.Text;

                    if (textBox2.Text == "")
                    {
                        if (drsad["配件编码"].ToString() == "")
                        {
                            DateTime t = CPublic.Var.getDatetime();
                            drsad["配件编码"] = string.Format("PJB{0}{1:00}{2:00}{3:0000}", t.Year, t.Month,
                                t.Day, CPublic.CNo.fun_得到最大流水号("PJB", t.Year, t.Month));
                        }
                    }
                    else
                    {
                        drsad["配件编码"] = textBox2.Text;
                    }

                  // dt_配件.AcceptChanges();

                }

                
                using (SqlDataAdapter da = new SqlDataAdapter("select *  from  销售出库产品配件表 where 1<>1", strcon))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt_配件);
                    MessageBox.Show("保存成功");
                    dt_配件.AcceptChanges();
                    barLargeButtonItem4_ItemClick(null,null);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

          
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string sql = string.Format("select * from 销售出库产品配件表 where 成品出库单明细号 ='{0}'", drM.ToString());
            dt_配件 = new DataTable();
            dt_配件 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dt_配件;
            flag = false;
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
          DataRow drm=(this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
          flag = false;
          textBox1.Text = drm["配件名称"].ToString();
          textBox2.Text = drm["配件编码"].ToString();
        
        }

        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            DataRow drm = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
            flag = false;
            textBox1.Text = drm["配件名称"].ToString();
            textBox2.Text = drm["配件编码"].ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
