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
namespace ERPSale
{
    public partial class 销售订单查询 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable t_片区 = ERPorg.Corg.fun_业务员片区(CPublic.Var.localUserName);
        DataTable dtM;
        public 销售订单查询()
        {
            InitializeComponent();
        }

        private void 销售订单查询_Load(object sender, EventArgs e)
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
                dateEdit1.EditValue = CPublic.Var.getDatetime().AddDays(-15).ToString("yyyy-MM-dd");
                dateEdit2.EditValue = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");
                fun_load();
                
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
                string sql = string.Format(@"select 客户编号,客户名称 from 客户基础信息表 where 停用=0");
                if (t_片区.Rows.Count > 0)
                {
                    string sx = " and  片区 in (";
                    foreach (DataRow r in t_片区.Rows)
                    {
                        sx = sx + string.Format("'{0}',", r["片区"]);
                    }
                    sx = sx.Substring(0, sx.Length - 1) + ")";
                    sql = sql + sx;
                }
                SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                DataTable dt_客户 = new DataTable();
                da.Fill(dt_客户);
                searchLookUpEdit1.Properties.DataSource = dt_客户;
                searchLookUpEdit1.Properties.DisplayMember = "客户名称";
                searchLookUpEdit1.Properties.ValueMember = "客户编号";

                string sql2 = string.Format(@"select 客户编号,客户名称 from 客户基础信息表 where 停用=0");
                SqlDataAdapter da2 = new SqlDataAdapter(sql, strcon);
                DataTable dt_目标客户 = new DataTable();
                da.Fill(dt_目标客户);
                searchLookUpEdit3.Properties.DataSource = dt_目标客户;
                searchLookUpEdit3.Properties.DisplayMember = "客户名称";
                searchLookUpEdit3.Properties.ValueMember = "客户名称";

                string sql_1 = string.Format(@"select 物料编码,规格型号,物料名称,大类,小类 from 基础数据物料信息表 where 停用=0");
                SqlDataAdapter da_1 = new SqlDataAdapter(sql_1, strcon);
                DataTable dt_物料 = new DataTable();
                da_1.Fill(dt_物料);
                searchLookUpEdit2.Properties.DataSource = dt_物料;
                searchLookUpEdit2.Properties.DisplayMember = "物料编码";
                searchLookUpEdit2.Properties.ValueMember = "物料编码";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
           
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
                string sql = string.Format(@" select xz.销售订单号,xm.销售订单明细号,xz.销售备注,xz.客户编号,xz.客户名,xz.目标客户,xz.审核,xz.审核日期,xz.关闭,xm.明细完成,
                                                     xm.物料编码,xm.物料名称,xm.规格型号,xm.数量,xm.完成数量,xm.未完成数量,xm.备注,xm.作废,xm.作废日期,xm.备注3,
                                                     xm.已通知数量,xm.未通知数量,xm.包装方式编号,xm.包装方式,xm.仓库名称,xz.录入人员,xz.创建日期,xm.送达日期,xm.销售预订单明细号
                                              from 销售记录销售订单主表 xz 
                                              left join 销售记录销售订单明细表 xm on xz.销售订单号 = xm.销售订单号
                                              where xz.作废 = 0   and   xz.创建日期>='{0}' and xz.创建日期<='{1}'", t1,t2);

                string sql_补 = "";
                if (checkBox1.Checked == true)
                {
                    sql_补 = string.Format(@" and xz.客户编号 = '{0}'", searchLookUpEdit1.EditValue.ToString());
                    sql += sql_补;
                }
                if (checkBox2.Checked == true)
                {
                    sql_补 = string.Format(@" and xz.目标客户 like '%{0}%'", searchLookUpEdit3.EditValue.ToString());
                    sql += sql_补;
                }


                if (checkBox3.Checked == true)
                {
                    sql = sql + string.Format(" and xz.销售订单号='{0}'", textBox1.Text.ToString());
                    sql += sql_补;

                }
                if (checkBox4.Checked == true)
                {
                    sql = sql + string.Format(" and xm.物料编码='{0}'", searchLookUpEdit2.EditValue.ToString());
                    sql += sql_补;

                }
                if (checkBox5.Checked == true)
                {
                    if (comboBox1.Text == "全部")
                    {

                    }
                    else if(comboBox1.Text == "关闭")
                    {
                        sql_补 = " and xz.关闭 = 1 ";
                        sql += sql_补;
                    }
                    else if (comboBox1.Text == "未关闭")
                    {
                        sql_补 = " and xz.关闭 = 0 ";
                        sql += sql_补;
                    }
                     

                }
                sql = sql + "order by xz.销售订单号";
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql,strcon);
                da.Fill(dtM);
                gridControl1.DataSource = dtM;
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
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择客户");
                }
            }
            if (checkBox2.Checked == true)
            {
                if (searchLookUpEdit3.EditValue == null || searchLookUpEdit3.EditValue.ToString() == "")
                {
                    throw new Exception("未选择目标客户");
                }
            }
            if (checkBox3.Checked == true)
            {
                if (textBox1.Text == null || textBox1.Text.ToString() == "")
                {
                    throw new Exception("未填写销售订单号");
                }

            }          
            if (checkBox4.Checked == true)
            {
                if (searchLookUpEdit2.EditValue == null || searchLookUpEdit2.EditValue.ToString() == "")
                {
                    throw new Exception("未选择物料");
                }

            }
            if (checkBox5.Checked == true)
            {
                if (comboBox1.Text == null || comboBox1.Text  == "")
                {
                    throw new Exception("未选择订单状态");
                }

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

        private void simpleButton1_Click_1(object sender, EventArgs e)
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
    }
}
