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
    public partial class 生产已检待入库查询 : UserControl
    {
        string cfgfilepath  = "";
        string strconn = CPublic.Var.strConn;
        DataTable dt_数据 = new DataTable();
        public 生产已检待入库查询()
        {
            InitializeComponent();
        }

        private void 生产已检待入库查询_Load(object sender, EventArgs e)
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
            string sql = "select 物料编码,物料名称,规格型号 from  基础数据物料信息表 where 停用 = 0 and 物料编码 in (select  产品编码 from 基础数据物料BOM表  group  by 产品编码)";
            DataTable dt_物料 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            searchLookUpEdit1.Properties.DataSource = dt_物料;
            searchLookUpEdit1.Properties.DisplayMember = "物料编码";
            searchLookUpEdit1.Properties.ValueMember = "物料编码";
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_check();
                fun_load();
            }
            catch(Exception ex)
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
                    throw new Exception("未填写生产检验单号");
                }
            }
            if (checkBox2.Checked == true)
            {
                if (textBox2.Text == null || textBox2.Text.ToString() == "")
                {
                    throw new Exception("未填写生产工单号");
                }
            }
            if (checkBox3.Checked == true)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择物料编码");
                }

            }
        }

        private void fun_load()
        {
            try
            {
                string sql = string.Format(@"select jy.*,生产制令单号,预计完工日期 
                                             from 生产记录生产检验单主表 jy
                                             left join 生产记录生产工单表 gd on jy.生产工单号=gd.生产工单号
                                             where jy.生效 = 1 and jy.作废 = 0 and jy.完成 = 0 and gd.作废 = 0 and   jy.已检验数量-jy.报废数-已入库数量>0
                                              ");

                string sql_补 = "";
                if (checkBox1.Checked == true)
                {
                    sql_补 = string.Format(@" and jy.生产检验单号 = '{0}'", textBox1.Text);
                    sql += sql_补;
                }
                if (checkBox2.Checked == true)
                {
                    sql_补 = string.Format(@" and jy.生产工单号 = '{0}'", textBox2.Text);
                    sql += sql_补;
                }
                if (checkBox3.Checked == true)
                {
                    sql_补 = string.Format(@" and jy.物料编码 = '{0}'", searchLookUpEdit1.EditValue.ToString());
                    sql += sql_补;
                }

                sql = sql + " order by 预计完工日期";
                dt_数据 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                
                foreach(DataRow dr in dt_数据.Rows)
                {
                    dr["未入库数量"] =Convert.ToDecimal( Convert.ToDecimal(dr["已检验数量"]) - Convert.ToDecimal(dr["报废数"]) - Convert.ToDecimal(dr["已入库数量"]));
                }
                gridControl1.DataSource = dt_数据;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                if (dt_数据 == null || dt_数据.Columns.Count == 0 || dt_数据.Rows.Count == 0)
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
