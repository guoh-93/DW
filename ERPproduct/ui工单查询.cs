using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace ERPproduct
{
    public partial class ui工单查询 : UserControl
    {
        string cfgfilepath = "";
        string strconn = CPublic.Var.strConn;
        DataTable dt_工单 = new DataTable();

        public ui工单查询()
        {
            InitializeComponent();
        }

        private void ui工单查询_Load(object sender, EventArgs e)
        {
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(this.panel2, this.Name, cfgfilepath);

            DateTime t = CPublic.Var.getDatetime().Date;
            t = t.AddDays(1).AddSeconds(-1);
            dateEdit2.EditValue = t;
            dateEdit1.EditValue = t.AddMonths(-1).Date;
            fun_下拉框();
        }

        private void fun_下拉框()
        {
            try
            {
                string sql = "select 物料编码,物料名称,规格型号 from 基础数据物料信息表 where 停用 = 0 ";
                DataTable dt_物料 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                searchLookUpEdit1.Properties.DataSource = dt_物料;
                searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                searchLookUpEdit1.Properties.ValueMember = "物料编码";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
     
        private void fun_check()
        {
            if (checkBox1.Checked == true)
            {
                if (textBox1.Text == null || textBox1.Text.ToString() == "")
                {
                    throw new Exception("未填写工单号");
                }
            }
            if (checkBox2.Checked == true)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择物料编码");
                }
            }
            if (checkBox3.Checked == true)
            {
                if (comboBox1.Text == null || comboBox1.Text.ToString() == "")
                {
                    throw new Exception("未选择工单状态");
                }

            }
        }
        private void fun_search()
        {
            DateTime t1 = Convert.ToDateTime(dateEdit1.EditValue);
            t1 = new DateTime(t1.Year, t1.Month, t1.Day);
            DateTime t2 = Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1);
            t2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);
            if (t2 < t1)
            {
                throw new Exception("结束时间需大于开始时间！");
            }

            string sql = string.Format(@"select * from  生产记录生产工单表 where 制单日期>'{0}' and 制单日期<'{1}'", t1, t2);
            string sql_补 = "";
            if(checkBox1.Checked == true)
            {
                sql_补 = string.Format(@" and 生产工单号 = '{0}'", textBox1.Text.ToString());
                sql += sql_补;
            }
            if(checkBox2.Checked == true)
            {
                sql_补 = string.Format(@" and 物料编码 = '{0}'", searchLookUpEdit1.EditValue.ToString());
                sql += sql_补;
            }
            if(checkBox3.Checked == true)
            {
                if(comboBox1.Text == "已生效")
                {
                    sql_补 = " and 生效=1 and 完工=0  and  关闭=0";
                }
                else if (comboBox1.Text == "未生效")
                {
                    sql_补 = " and 生效=0  and  关闭=0";
                }
                else if (comboBox1.Text == "已完工")
                {
                    sql_补 = " and 完工=1  and  关闭=0";
                }
                else if (comboBox1.Text == "未完工")
                {
                    sql_补 = " and 完工=0 and 生效 =1  and  关闭=0";
                }
                else if (comboBox1.Text == "已关闭")
                {
                    sql_补 = " and 关闭=1";
                }
                sql += sql_补;
            }
            dt_工单 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gc.DataSource = dt_工单;
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                if (dt_工单 == null || dt_工单.Columns.Count == 0 || dt_工单.Rows.Count == 0)
                {

                    throw new Exception("没有数据可以导出");
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    //DataTable tt = dtM.Copy();
                    //tt.Columns.Remove("作废");
                    gc.ExportToXlsx(saveFileDialog.FileName);
                    //ERPorg.Corg.TableToExcel(tt, saveFileDialog.FileName);
                    MessageBox.Show("导出成功");
                }

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

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                fun_check();
                fun_search();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
