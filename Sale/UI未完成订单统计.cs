using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
namespace ERPSale
{
    public partial class UI未完成订单统计 : UserControl
    {

        string cfgfilepath = "";
        string strcon = CPublic.Var.strConn;
        public UI未完成订单统计()
        {
            InitializeComponent();
        }

        private void UI未完成订单统计_Load(object sender, EventArgs e)
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
                x.UserLayout(this.panel2, this.Name, cfgfilepath);
                fun_load();
            }
            catch (Exception)
            {

                throw;
            }

           
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
        private void fun_load()
        {
            string sql = string.Format(@"select 客户编号,客户名称 from 客户基础信息表");
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            DataTable  dt_客户 = new DataTable();
            da.Fill(dt_客户);
            sl_客户.Properties.DataSource = dt_客户;
            sl_客户.Properties.DisplayMember = "客户名称";
            sl_客户.Properties.ValueMember = "客户编号";
            string sql2 = "select 物料类型名称 from 基础数据物料类型表 where 类型级别 = '大类' order by 物料类型名称";
            DataTable dt = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(sql2, strcon);
            da2.Fill(dt);
       
            sl_大类.Properties.DataSource = dt;
            sl_大类.Properties.ValueMember = "物料类型名称";
            sl_大类.Properties.DisplayMember = "物料类型名称";
            string sql3 = "select 物料类型名称 from 基础数据物料类型表 where 类型级别 = '小类' order by 物料类型名称";
            DataTable dt_小类 = new DataTable();
            SqlDataAdapter da1 = new SqlDataAdapter(sql3, strcon);
            da1.Fill(dt_小类);
            sl_小类.Properties.DataSource = dt_小类;
            sl_小类.Properties.ValueMember = "物料类型名称";
            sl_小类.Properties.DisplayMember = "物料类型名称";
        }
    
        private void fun_search()
        {
            string sql = string.Format(@"select a.*,b.部门编号,销售部门,片区,b.录入人员 as 制单人 from 销售未完成订单统计  a
            left join 销售记录销售订单主表 b  on a.销售订单号 = b.销售订单号  where 1=1 ");
            if (checkBox1.Checked == true)
            {
                //sql = sql + string.Format(" and 销售记录销售订单明细表.客户编号='{0}'", sl_客户.EditValue.ToString());
                sql = sql + string.Format(" and 客户编号='{0}'", sl_客户.EditValue.ToString());

            }
            if (checkBox2.Checked == true)
            {
                sql = sql + string.Format(" and 大类='{0}'", sl_大类.EditValue.ToString());

            }
            if (checkBox3.Checked == true)
            {
                sql = sql + string.Format(" and 小类='{0}'", sl_小类.EditValue.ToString());
            }
            if (checkBox4.Checked == true)
            {
                DateTime t1 = Convert.ToDateTime(dateEdit1.EditValue);
                t1 = new DateTime(t1.Year, t1.Month, t1.Day);
                DateTime t2 = Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1);
                t2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);

                sql = sql + string.Format(" and 送达日期>='{0}' and 送达日期<='{1}'",t1,t2);
            }
            if (checkBox5.Checked == true)
            {
                //sql = sql + string.Format(" and 销售记录销售订单明细表.n原ERP规格型号='{0}'", textBox1.Text);
                sql = sql + string.Format(" and 规格型号='{0}'", textBox1.Text);

            }
            if (checkBox6.Checked == true)
            {

                DateTime t1 = Convert.ToDateTime(dateEdit4.EditValue);
                t1 = new DateTime(t1.Year, t1.Month, t1.Day);
                DateTime t2 = Convert.ToDateTime(dateEdit3.EditValue).AddDays(1).AddSeconds(-1);
                t2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);

                //sql = sql + string.Format(" and 销售记录销售订单主表.创建日期>='{0}' and 销售记录销售订单主表.创建日期<='{1}'", t1,t2);
                sql = sql + string.Format(" and 定购日期>='{0}' and 定购日期<='{1}'", t1, t2);

            }
            if (CPublic.Var.localUserName != "admin" && CPublic.Var.LocalUserTeam != "管理员权限")
            {
                string sql1 = "and b.部门编号 = '" + CPublic.Var.localUser部门编号 + "'";
                sql = sql + sql1;
            }
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql,strcon);
            gridControl1.DataSource = dt;

        }
        private void fun_check()
        {
            if (checkBox1.Checked == true)
            {
                if (sl_客户.EditValue == null || sl_客户.EditValue.ToString() == "")
                {
                    throw new Exception("未选择客户");
                }
            }
            if (checkBox2.Checked == true)
            {
                if (sl_大类.EditValue == null || sl_大类.EditValue.ToString() == "")
                {
                    throw new Exception("未选择大类");
                }

            }
            if (checkBox3.Checked == true)
            {
                if (sl_小类.EditValue == null || sl_小类.EditValue.ToString() == "")
                {
                    throw new Exception("未选择小类");
                }
            }
            if (checkBox4.Checked == true)
            {
               if (dateEdit1.EditValue == null ||dateEdit2.EditValue == null ||dateEdit1.EditValue.ToString() == "" || dateEdit2.EditValue.ToString() == "")
                {
                    throw new Exception("未选择时间");
                }

            }
            if (checkBox5.Checked == true)
            {
                if (textBox1.Text == null || textBox1.Text .ToString() == "")
                {
                    throw new Exception("未选择产品");
                }
                
            }
            if (checkBox6.Checked == true)
            {
                if (dateEdit4.EditValue == null || dateEdit3.EditValue == null || dateEdit3.EditValue.ToString() == "" || dateEdit4.EditValue.ToString() == "")
                {
                    throw new Exception("未选择订单创建日期");
                }

            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                ERPorg.Corg.FlushMemory();
                fun_check();
                fun_search();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked == true)
            {
                if (textBox1.Text.Length > 4)
                {
                    string sql = string.Format("select 规格型号 from 基础数据物料信息表 where 规格型号 like '{0}%' ", textBox1.Text);
                    DataTable dt = new DataTable();
                    dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    listBox1.Items.Clear();
                    foreach (DataRow dr in dt.Rows)
                    {
                        listBox1.Items.Add(dr["规格型号"]);
                    }
                    listBox1.Visible = true;
                }
            }
        }

        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            textBox1.Text = listBox1.SelectedItem.ToString(); ;
            listBox1.Visible = false;
        
        }

        private void gridView4_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            } 
        }

        private void gridView4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView4.GetFocusedRowCellValue(gridView4.FocusedColumn));
                e.Handled = true;
            }
        }

        private void barLargeButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
            
                gridControl1.ExportToXlsx(saveFileDialog.FileName);
            
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
