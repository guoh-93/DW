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
namespace StockCore
{
    public partial class 月结转查询 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dtM;
        public 月结转查询()
        {
            InitializeComponent();
        }

        private void 月结转查询_Load(object sender, EventArgs e)
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(panel2, this.Name, cfgfilepath);
                //CZMaster.DevGridControlHelper.Helper(this);
                dateEdit1.EditValue = CPublic.Var.getDatetime().AddYears(-1).ToString("yyyy-MM-dd");
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
            string sql_1 = string.Format(@"select 物料编码,规格型号,物料名称,存货分类,大类,小类 from 基础数据物料信息表 where 停用=0");
            SqlDataAdapter da_1 = new SqlDataAdapter(sql_1, strconn);
            DataTable dt_物料 = new DataTable();
            da_1.Fill(dt_物料);
            searchLookUpEdit1.Properties.DataSource = dt_物料;
            searchLookUpEdit1.Properties.DisplayMember = "物料编码";
            searchLookUpEdit1.Properties.ValueMember = "物料编码";
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

        private void fun_search()
        {
            try
            {
                DateTime t1 = Convert.ToDateTime(dateEdit1.EditValue);
                t1 = new DateTime(t1.Year, t1.Month, t1.Day);
                DateTime t2 = Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1);
                t2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);
                if (t2 < t1)
                {
                    throw new Exception("结束时间需大于开始时间！");
                }
                string sql = string.Format(@"select a.*,base.物料名称 as 物料名称b,base.规格型号,base.存货分类,base.存货分类编码  from  仓库月出入库结转表 a
             left join 基础数据物料信息表 base on a.物料编码 = base.物料编码 where   结算日期 >='{0}' and 结算日期<='{1}'", t1, t2);

                string sql_补 = "";
                //if (checkBox1.Checked == true)
                //{
                //    sql_补 = string.Format(@" and  结算日期>='{0}' and 结算日期<='{1}'", t1, t2);
                //    sql += sql_补;
                //}
                if (checkBox2.Checked == true)
                {
                    sql_补 = string.Format(@" and a.物料编码 = '{0}'", searchLookUpEdit1.EditValue.ToString());
                    sql += sql_补;
                }
                
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                gc.DataSource = dtM;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_check()
        {
            if (checkBox2.Checked == true)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择未选择物料");
                }
            }            
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
           
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ERPorg.Corg.FlushMemory();
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                if (dtM == null || dtM.Columns.Count == 0 || dtM.Rows.Count == 0)
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
    }
}
