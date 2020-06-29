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
    public partial class ui成品对应信息维护 : UserControl
    {
        public ui成品对应信息维护()
        {
            InitializeComponent();
        }


        #region 变量

        DataTable CheckFinished;
        DataRow drM;
        DataTable dt_配件, dt_说明书, dt_标签;
        string strcon = CPublic.Var.strConn;


        #endregion

        private void gridControl2_Load(object sender, EventArgs e)
        {
          
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DateTime t=Convert.ToDateTime(  barEditItem2.EditValue).AddDays(1).AddSeconds(-1);;
            string sql = string.Format("select 物料名称,成品出库单明细号,物料编码,客户,客户编号,仓库号,仓库名称 from 销售记录成品出库单明细表 where 生效日期>'{0}'and 生效日期<'{1}'", barEditItem1.EditValue, t);
            DataTable dt_aggradage = CZMaster.MasterSQL.Get_DataTable(sql,strcon);
        if (dt_aggradage.Rows.Count > 0)
        {
            searchLookUpEdit1.Properties.DataSource = dt_aggradage;
            searchLookUpEdit1.Properties.DisplayMember = "物料名称";
            searchLookUpEdit1.Properties.ValueMember = "成品出库单明细号";

        }
      
        }

        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                gridView1.CloseEditor();
                this.BindingContext[dt_配件].EndCurrentEdit();

            }
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {         
            string sql = string.Format("select * from 销售出库产品配件表 where 成品出库单明细号 ='{0}'", searchLookUpEdit1.EditValue.ToString());
          dt_配件 = CZMaster.MasterSQL.Get_DataTable(sql,strcon);
            gridControl1.DataSource = dt_配件;
            sql = string.Format("select * from 销售出库产品说明书表 where 成品出库单明细号 ='{0}'", searchLookUpEdit1.EditValue.ToString());
          dt_说明书 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl2.DataSource = dt_说明书;
            sql = string.Format("select * from 销售出库产品标签表 where 成品出库单明细号 ='{0}'", searchLookUpEdit1.EditValue.ToString());
           dt_标签 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl3.DataSource = dt_标签;
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            string sql = string.Format("select * from 销售出库产品配件表 where 成品出库单明细号 ='{0}'", searchLookUpEdit1.EditValue.ToString());
            dt_配件 = new DataTable();
            dt_配件 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dt_配件;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEdit1.Text=="")
                {
                    throw new Exception("请先选择明细");
            }
                DataRow dr = dt_配件.NewRow();
                dt_配件.Rows.Add(dr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
                if (drM == null)
                {
                    throw new Exception("未选中任意行不可删除");
                }
                drM.Delete();         
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            try
            {
            gridView1.CloseEditor();
            this.BindingContext[dt_配件].EndCurrentEdit();
            string sql=string.Format("select * from 销售记录成品出库单明细表 where 成品出库单明细号='{0}'",searchLookUpEdit1.EditValue);
            DataRow dr_原始值=CZMaster.MasterSQL.Get_DataRow(sql,strcon);
            foreach(DataRow dr  in dt_配件.Rows ){
                if (dr.RowState == DataRowState.Deleted)
                {
                    continue;
                }
                dr["成品出库单明细号"] = dr_原始值["成品出库单明细号"];
                dr["物料编码"] = dr_原始值["物料编码"];
                dr["物料名称"] = dr_原始值["物料名称"];
                dr["客户"] = dr_原始值["客户"];
                dr["客户编号"] = dr_原始值["客户编号"];
                dr["仓库号"] = dr_原始值["仓库号"];
                dr["仓库名称"] = dr_原始值["仓库名称"];
                dr["配件名称"] = dr["配件名称"];
                if (dr["配件编码"].ToString()=="")
                {  DateTime t = CPublic.Var.getDatetime();
                dr["配件编码"] = string.Format("PJB{0}{1:00}{2:00}{3:0000}", t.Year, t.Month,
                    t.Day, CPublic.CNo.fun_得到最大流水号("PJB", t.Year, t.Month));                   
               }
            }      
            using (SqlDataAdapter da = new SqlDataAdapter("select *  from  销售出库产品配件表 where 1<>1", strcon))
            {
                new SqlCommandBuilder(da);
                da.Update(dt_配件);
                MessageBox.Show("保存成功");
                dt_配件.AcceptChanges();
            }
       
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ui成品对应信息维护_Load(object sender, EventArgs e)
        {
            DateTime t = CPublic.Var.getDatetime();
            barEditItem1.EditValue = DateTime.Parse(t.AddDays(-7).ToString());
            barEditItem2.EditValue = t.AddDays(1).AddSeconds(-1);
        }

        private void simpleButton8_Click(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEdit1.Text == "")
                {
                    throw new Exception("请先选择明细");
                }
                DataRow dr = dt_说明书.NewRow();
                dt_说明书.Rows.Add(dr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton7_Click(object sender, EventArgs e)
        {
            try
            {
                drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
                if (drM == null)
                {
                    throw new Exception("未选中任意行不可删除");
                }
                drM.Delete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            try
            {
                gridView1.CloseEditor();
                this.BindingContext[dt_说明书].EndCurrentEdit();
                string sql = string.Format("select * from 销售记录成品出库单明细表 where 成品出库单明细号='{0}'", searchLookUpEdit1.EditValue);
                DataRow dr_原始值 = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
                foreach (DataRow dr in dt_说明书.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }
                    dr["成品出库单明细号"] = dr_原始值["成品出库单明细号"];
                    dr["物料编码"] = dr_原始值["物料编码"];
                    dr["物料名称"] = dr_原始值["物料名称"];
                    dr["客户"] = dr_原始值["客户"];
                    dr["客户编号"] = dr_原始值["客户编号"];
                    dr["仓库号"] = dr_原始值["仓库号"];
                    dr["仓库名称"] = dr_原始值["仓库名称"];
                    dr["说明书"] = dr["说明书"];
                    if (dr["说明书编码"].ToString() == "")
                    {
                        DateTime t = CPublic.Var.getDatetime();
                        dr["说明书编码"] = string.Format("PJB{0}{1:00}{2:00}{3:0000}", t.Year, t.Month,
                            t.Day, CPublic.CNo.fun_得到最大流水号("PJB", t.Year, t.Month));
                    }
                }
                using (SqlDataAdapter da = new SqlDataAdapter("select *  from  销售出库产品说明书表 where 1<>1", strcon))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt_说明书);
                    MessageBox.Show("保存成功");
                    dt_说明书.AcceptChanges();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            string sql = string.Format("select * from 销售出库产品说明书表 where 成品出库单明细号 ='{0}'", searchLookUpEdit1.EditValue.ToString());
            dt_说明书 = new DataTable();
            dt_说明书 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dt_说明书;
        }

        private void 维护配件包ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ////if (dt_配件.Rows.Count < 0)
            ////{
            ////    throw new Exception("未选中行 不可进入");
            ////}

             DataRow dr_当前行 = gridView1.GetDataRow(gridView1.FocusedRowHandle);
             string 编码 = searchLookUpEdit1.EditValue.ToString();
             ERPSale.配件包维护 fm = new 配件包维护(编码);

            fm.ShowDialog();

         
        }

        private void 维护说明书ToolStripMenuItem_Click(object sender, EventArgs e)
        {
           // DataRow dr_当前行 = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            string 编码 = searchLookUpEdit1.EditValue.ToString();
            ERPSale.说明书维护 fm = new 说明书维护(编码);

            fm.ShowDialog();
        }

        private void 维护标签ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string 编码 = searchLookUpEdit1.EditValue.ToString();
            ERPSale.标签维护 fm = new 标签维护(编码);

            fm.ShowDialog();

        }


           







    }
}
