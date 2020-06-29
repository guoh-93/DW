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
namespace ERPproduct
{
    public partial class ui待发料查询 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dt_发料 = new DataTable();
        public ui待发料查询()
        {
            InitializeComponent();
        }

        private void ui待发料查询_Load(object sender, EventArgs e)
        {
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(this.panel2, this.Name, cfgfilepath);
            fun_下拉框();
        }

        private void fun_下拉框()
        {
            try
            {
                string sql = "select 物料编码,物料名称,规格型号 from 基础数据物料信息表 where 停用=0 and 物料编码 in (select  产品编码 from 基础数据物料BOM表  group  by 产品编码)";
                DataTable dt_产品 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                searchLookUpEdit1.Properties.DataSource = dt_产品;
                searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                searchLookUpEdit1.Properties.ValueMember = "物料编码";

                string sql1 = "select 物料编码,物料名称,规格型号 from 基础数据物料信息表 where 停用=0 and 物料编码 in (select  子项编码 from 基础数据物料BOM表  group  by 子项编码)";
                DataTable dt_子项 = CZMaster.MasterSQL.Get_DataTable(sql1, strconn);
                searchLookUpEdit2.Properties.DataSource = dt_子项;
                searchLookUpEdit2.Properties.DisplayMember = "物料编码";
                searchLookUpEdit2.Properties.ValueMember = "物料编码";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_load()
        {
            try
            {
                string sql = @" select scgd.生产工单号,scgd.生产制令单号,scgd.物料编码 as 产品编码,scgd.物料名称 as 产品名称,
                                       scgd.规格型号 as 产品规格型号,scgd.仓库名称 as 产品仓库名称,scgd.仓库号 as 产品仓库号, 
                                       scdl.待领料单号,scdl.物料编码 as 子项编码,scdl.物料名称 as 子项名称,scdl.规格型号 as 子项规格型号,scdl.待领料总量,
                                       scdl.已领数量,scdl.未领数量,scdl.完成,scdl.仓库号 as 子项仓库号,scdl.仓库名称 as 子项仓库名称,scdl.计量单位编号,scdl.计量单位
                                       from  生产记录生产工单表 scgd
                                        left join 生产记录生产工单待领料主表 scdlz on scdlz.生产工单号 = scgd.生产工单号
                                       left join 生产记录生产工单待领料明细表 scdl on scdl.待领料单号 = scdlz.待领料单号 
                                       where scdl.完成 = 0 and scdl.未领数量 > 0 and scgd.关闭 = 0 and scdlz.关闭 = 0";
                
                string sql_补 = "";
                if (checkBox1.Checked == true)
                {
                    sql_补 = string.Format(@" and scgd.生产制令单号 = '{0}'", textBox1.Text);
                    sql += sql_补;
                }
                if (checkBox2.Checked == true)
                {
                    sql_补 = string.Format(@" and scdl.待领料单号 = '{0}'", textBox2.Text);
                    sql += sql_补;
                }
                if (checkBox3.Checked == true)
                {
                    sql_补 = string.Format(@" and scgd.生产工单号 = '{0}'", textBox3.Text);
                    sql += sql_补;
                }
                if (checkBox4.Checked == true)
                {
                    sql_补 = string.Format(@" and scgd.物料编码 = '{0}'", searchLookUpEdit1.EditValue.ToString());
                    sql += sql_补;
                }
                if (checkBox5.Checked == true)
                {
                    sql_补 = string.Format(@" and scdl.物料编码 = '{0}'", searchLookUpEdit2.EditValue.ToString());
                    sql += sql_补;
                }
                if (checkBox6.Checked == true)
                {
                    sql_补 = string.Format(@" and scgd.规格型号 like '%{0}%'", textBox4.Text);
                    sql += sql_补;
                }
                if (checkBox7.Checked == true)
                {
                    sql_补 = string.Format(@" and scdl.规格型号 like '%{0}%'", textBox5.Text);
                    sql += sql_补;
                }
                dt_发料 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                gridControl1.DataSource = dt_发料;
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
                    throw new Exception("未填写生产制令单号");
                }
            }
            if (checkBox2.Checked == true)
            {
                if (textBox2.Text == null || textBox2.Text.ToString() == "")
                {
                    throw new Exception("未填写待发料单号");
                }
            }
            if (checkBox3.Checked == true)
            {
                if (textBox3.Text == null || textBox3.Text.ToString() == "")
                {
                    throw new Exception("未填写生产工单号");
                }

            }
            if (checkBox4.Checked == true)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择产品编码");
                }

            }
            if (checkBox5.Checked == true)
            {
                if (searchLookUpEdit2.EditValue == null || searchLookUpEdit2.EditValue.ToString() == "")
                {
                    throw new Exception("未选择子项编码");
                }

            }
            if (checkBox6.Checked == true)
            {
                if (textBox4.Text == null || textBox4.Text.ToString() == "")
                {
                    throw new Exception("未填写产品规格");
                }
            }
            if (checkBox7.Checked == true)
            {
                if (textBox5.Text == null || textBox5.Text.ToString() == "")
                {
                    throw new Exception("未填写子项规格");
                }

            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            
        }

        private void barLargeButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void simpleButton1_Click_1(object sender, EventArgs e)
        {
            try
            {
                fun_check();
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

                if (dt_发料 == null || dt_发料.Columns.Count == 0 || dt_发料.Rows.Count == 0)
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
                    gridControl1.ExportToXlsx(saveFileDialog.FileName);
                    //ERPorg.Corg.TableToExcel(tt, saveFileDialog.FileName);
                    MessageBox.Show("导出成功");
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
