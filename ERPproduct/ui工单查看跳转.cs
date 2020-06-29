using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace ERPproduct
{
    public partial class ui工单查看跳转 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        string cfgfilepath = "";
        /// <summary>
        /// 用于扩展入库
        /// 0 表示正常界面操作
        /// 1 表示 传入工单
        /// </summary>
        int i = 0;
        string str_gd = "";

        public ui工单查看跳转()
        {
            InitializeComponent();
        }
        //
        public ui工单查看跳转(string s_工单号)
        {
            InitializeComponent();
            i = 1;
            str_gd = s_工单号.Trim() ;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
                    gc.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        
        private  string  fun_check()
        {
            string s = "";
            if (checkBox1.Checked)
            {
                if (dateEdit1.EditValue != null || dateEdit1.EditValue.ToString() != "")
                {
                    s += string.Format(" and 制单日期>'{0}'", dateEdit1.EditValue);
                }
                DateTime t = Convert.ToDateTime(dateEdit2.EditValue).Date.AddDays(1).AddSeconds(-1);
                if (dateEdit2.EditValue != null && dateEdit2.EditValue.ToString() != "")
                {
                    s += string.Format(" and 制单日期<'{0}'", t);
                }
            }
            if(checkBox2.Checked)
            {
                if (textBox1.Text == null || textBox1.Text.ToString() == "")
                {
                    throw new Exception("未填写物料编码");
                }
                else
                {
                    s += string.Format(" and a.物料编码= '{0}'", textBox1.Text.Trim());
                }
               
            }
            if (checkBox3.Checked)
            {
                if (textBox2.Text == null || textBox2.Text.ToString() == "")
                {
                    throw new Exception("未填写生产工单号");
                }
                else
                {
                    s += string.Format(" and  a.生产工单号 like '%{0}%'", textBox2.Text.Trim());
                }
             }
            if (checkBox4.Checked)
            {
                if (textBox3.Text == null || textBox3.Text.ToString() == "")
                {
                    throw new Exception("未填写生产制令单号");
                }
                else
                {
                    s += string.Format(" and a.生产制令单号 like '%{0}%'", textBox3.Text.Trim());
                }
                
            }
            if (checkBox5.Checked)
            {
                if (textBox4.Text == null || textBox4.Text.ToString() == "")
                {
                    throw new Exception("未填写子项编码");
                }
                else
                {
                    string ll = string.Format("select 产品编码  from 基础数据物料BOM表 where 子项编码='{0}' ", textBox4.Text.Trim());
                    DataTable temp = CZMaster.MasterSQL.Get_DataTable(ll,strcon);
                    string xx = "(";
                    foreach (DataRow r  in temp.Rows)
                    {
                        xx += "'" + r["产品编码"].ToString() + "',";
                    }
                    if (temp.Rows.Count == 0) xx = "";
                    else
                    {
                        xx = xx.Substring(0, xx.Length - 1)+')';
                    }
                    s += string.Format(" and a.物料编码  in {0}",xx);
                }
            }
            return s;
        }
        private void fun_search(string s_条件)
        {
            string s = string.Format(@"select a.*  from 生产记录生产工单表 a
                left join   基础数据物料信息表 b on b.物料编码=a.物料编码  
                where a.制单日期>='2019-5-1' {0}", s_条件);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            gc.DataSource = dt;

        }

        private void simpleButton1_Click(object sender, EventArgs e)
         {
            try
            {
                //查询时 界面i==0 设置为 正常界面操作
                i = 0;
                string s_条件 = "";
                 s_条件 = fun_check();
                fun_search(s_条件);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        
        private void ui工单查看跳转_Load(object sender, EventArgs e)
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
                string s_条件 = "";
        
                if (i == 1)
                {
                    s_条件 += string.Format("生产工单号='{0}'", str_gd);
                    fun_search(s_条件);
                }
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
    }
}
