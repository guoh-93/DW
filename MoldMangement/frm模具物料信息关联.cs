using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace MoldMangement
{
    public partial class frm模具物料信息关联 : Form
    {
        CurrencyManager cmM;
        DataTable dt_模具物料信息关联表;
        DataTable dt_物料信息表;
        public frm模具物料信息关联()
        {
            InitializeComponent();
        }

        private void fun_GetDataTable(DataTable dt, string sql)
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                da.Fill(dt);
            }
        }

        private void fun_SetDataTable(DataTable dt, string sql)
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                new SqlCommandBuilder(da);
                da.Update(dt);
            }
        }

        private void frm模具物料信息关联_Load(object sender, EventArgs e)
        {
            try
            {
                this.gv.IndicatorWidth = 40;
                txt_模具编号.Text = MoldMangement.frm模具管理基础信息维护界面.dr["模具编号"].ToString();
                txt_存放库位.Text = MoldMangement.frm模具管理基础信息维护界面.dr["存放库位"].ToString();
                txt_零件材料.Text = MoldMangement.frm模具管理基础信息维护界面.dr["所用零件材料"].ToString();
                txt_零件图号.Text = MoldMangement.frm模具管理基础信息维护界面.dr["零件图号"].ToString();
                txt_产品型号.Text = MoldMangement.frm模具管理基础信息维护界面.dr["产品型号"].ToString();
                dt_模具物料信息关联表 = new DataTable();
                string sql = string.Format(@"SELECT 基础数据物料信息表.*,模具物料信息关联表.模具编号,模具物料信息关联表.审核1 FROM 模具物料信息关联表
	                                        LEFT JOIN 基础数据物料信息表 ON 模具物料信息关联表.物料编码=基础数据物料信息表.物料编码 
                                            where 模具物料信息关联表.模具编号='{0}'", txt_模具编号.Text);
                fun_GetDataTable(dt_模具物料信息关联表, sql);
                dt_物料信息表 = new DataTable();
                string sql2 = "select 物料编码,原ERP物料编号,物料名称,规格型号,图纸编号,n原ERP规格型号,n仓库描述 from 基础数据物料信息表";
                fun_GetDataTable(dt_物料信息表, sql2);
                repositoryItemSearchLookUpEdit1.DataSource = dt_物料信息表;
                repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
                repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
                gc.DataSource = dt_模具物料信息关联表;
                cmM = BindingContext[dt_模具物料信息关联表] as CurrencyManager;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);          
            }
        }

        private void gv_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.Caption == "物料编码")
            {
                try
                {
                    DataRow myDataRow = gv.GetDataRow(gv.FocusedRowHandle);
                    string s_物料编码 = myDataRow["物料编码"].ToString();
                    DataRow[] drArr = dt_物料信息表.Select("物料编码= '" + s_物料编码 + "'");
                    myDataRow["原ERP物料编号"] = drArr[0].ItemArray[1];
                    myDataRow["物料名称"] = drArr[0].ItemArray[2];
                    myDataRow["规格型号"] = drArr[0].ItemArray[3];
                    myDataRow["图纸编号"] = drArr[0].ItemArray[4];
                    myDataRow["n原ERP规格型号"] = drArr[0].ItemArray[5];
                    myDataRow["n仓库描述"] = drArr[0].ItemArray[6];
                    myDataRow["模具编号"] = txt_模具编号.Text;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv.CloseEditor();
                (this.BindingContext[dt_模具物料信息关联表] as CurrencyManager).EndCurrentEdit();
                string sql = "select * from 模具物料信息关联表 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt_模具物料信息关联表);
                }
                MessageBox.Show("保存成功！");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            cmM.EndCurrentEdit();
            gv.CloseEditor();
            try
            {
                cmM.AddNew();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            cmM.EndCurrentEdit();
            gv.CloseEditor();
            try
            {
                (cmM.Current as DataRowView).Row.Delete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv.CloseEditor();
                (this.BindingContext[dt_模具物料信息关联表] as CurrencyManager).EndCurrentEdit();
                string sql = "select * from 模具物料信息关联表 where 1<>1";
                foreach (DataRow dr in dt_模具物料信息关联表.Rows)
                {
                    dr["审核1"] = true;
                }
                using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
                {
                    new SqlCommandBuilder(da);

                    da.Update(dt_模具物料信息关联表);
                }
                DataTable dt = new DataTable();
                string sql3 = string.Format("select * from 模具管理基础信息表 where 模具编号 = '{0}'", txt_模具编号.Text);
                fun_GetDataTable(dt, sql3);
                foreach (DataRow dr in dt.Rows)
                {
                    dr["审核"] = true;
                }
                string sql2 = string.Format("select * from 模具管理基础信息表 where  1<>1");
                fun_SetDataTable(dt, sql2);
                MessageBox.Show("保存成功！");
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
                gv.CloseEditor();
                (this.BindingContext[dt_模具物料信息关联表] as CurrencyManager).EndCurrentEdit();
                string sql = "select * from 模具物料信息关联表 where 1<>1";
                foreach (DataRow dr in dt_模具物料信息关联表.Rows)
                {
                    dr["审核1"] = false;
                }
                using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
                {
                    new SqlCommandBuilder(da);

                    da.Update(dt_模具物料信息关联表);
                }
                DataTable dt = new DataTable();
                string sql3 = string.Format("select * from 模具管理基础信息表 where 模具编号 = '{0}'", txt_模具编号.Text);
                fun_GetDataTable(dt, sql3);
                foreach (DataRow dr in dt.Rows)
                {
                    dr["审核"] = false;
                }
                string sql2 = string.Format("select * from 模具管理基础信息表 where  1<>1");
                fun_SetDataTable(dt, sql2);
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
