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
namespace BaseData
{
    public partial class frm未完成待办事项 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dt_物料;
        public frm未完成待办事项()
        {
            InitializeComponent();
        }

        private void frm未完成待办事项_Load(object sender, EventArgs e)
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
                dateEdit1.EditValue = CPublic.Var.getDatetime().AddMonths(-1).ToString("yyyy-MM-dd");
                dateEdit2.EditValue = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_load()
        {
            string sql = "select 物料编码,物料名称,规格型号 from 基础数据物料信息表";
            dt_物料 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            searchLookUpEdit1.Properties.DataSource = dt_物料;
            searchLookUpEdit1.Properties.DisplayMember = "物料编码";
            searchLookUpEdit1.Properties.ValueMember = "物料编码";

            sql = "select 类型 from V_未完成待办事项 group by 类型";
            DataTable dt_类型 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            //comboBox1.DataSource = dt_类型;
            //comboBox1.DisplayMember = "类型";
            //comboBox1.ValueMember = "类型";

            checkedComboBoxEdit1.Properties.DataSource = dt_类型;
            checkedComboBoxEdit1.Properties.DisplayMember = "类型";
            checkedComboBoxEdit1.Properties.ValueMember = "类型";
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                string s_条件 = "";
                s_条件 = fun_check();
                fun_search(s_条件);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_search(string s_条件)
        {
            string sql = string.Format("select * from  V_未完成待办事项 where 1 =1 {0}", s_条件);
            DataTable dt_未完成单据 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gridControl1.DataSource = dt_未完成单据;
        }


        private string fun_check()
        {
            string s = "";
            if (checkBox1.Checked == true)
            {
                DateTime t = Convert.ToDateTime(dateEdit2.EditValue).Date.AddDays(1).AddSeconds(-1);
                if (dateEdit1.EditValue != null || dateEdit1.EditValue.ToString() != "")
                {
                    //if (t < Convert.ToDateTime(dateEdit1.EditValue))
                    //{
                    //    throw new Exception("结束时间需大于开始时间！");
                    //}
                    s += string.Format(" and 申请日期>'{0}'", dateEdit1.EditValue);
                }
                
                
                if (dateEdit2.EditValue != null && dateEdit2.EditValue.ToString() != "")
                {
                    s += string.Format(" and 申请日期<'{0}'", t);
                }
            }
            if (checkBox2.Checked == true)
            {
                if (textBox1.Text == null || textBox1.Text.ToString() == "")
                {
                    throw new Exception("未填写单号");
                }
                else
                {
                    s += string.Format(" and 单号 like '%{0}%'", textBox1.Text);
                }
            }
            if (checkBox3.Checked == true)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择物料");
                }
                else
                {
                    s += string.Format(" and 物料编码= '{0}'", searchLookUpEdit1.EditValue.ToString());
                }

            }
            if (checkBox4.Checked == true)
            {
                if (checkedComboBoxEdit1.EditValue == null || checkedComboBoxEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择单据类型");
                }
                else
                {
                    string s_类型 = checkedComboBoxEdit1.EditValue.ToString();
                    
                    string[] s_单据类型 = s_类型.Split(',');
                    
                    string p = "";
                    foreach ( string  x in s_单据类型)
                    {
                        p += "'" + x.Trim() + "',";
                    }
                    p=p.Substring(0, p.Length -1);
                    s += string.Format(" and 类型 in ({0})",p);

                    //if(s_单据类型.Length == 1)
                    //{
                    //    s += string.Format(" and 类型 = '{0}'", s_单据类型[0].ToString());
                    //}
                    //else
                    //{


                    //    s += "and (";
                    //    for (int i = 0; i < s_单据类型.Length; i++)
                    //    {
                    //        s_单据类型[0] = s_单据类型[0].Trim();
                    //        s += string.Format("类型 = '{0}' ", s_单据类型[0].ToString());
                    //    }
                    //}
                    //for(int i = 0; i < s_单据类型.Length; i++)
                    //{
                    //    s_单据类型[0] = s_单据类型[0].Trim();
                    //    s += string.Format("  类型 = '{0}'", s_单据类型[0].ToString());
                    //}
                    //  s += string.Format(" and 类型= '{0}'", comboBox1.Text);
                }

            }
            return s;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
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
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

    }
}
