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

namespace ERPpurchase
{
    public partial class ui拒收核销 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dt_未核销明细;

        public ui拒收核销()
        {
            InitializeComponent();
        }

        private void ui拒收核销_Load(object sender, EventArgs e)
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

                DateTime t = CPublic.Var.getDatetime();
                dateEdit1.EditValue = "2019-10-17";
                dateEdit2.EditValue = t;
                fun_下拉框();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void fun_下拉框()
        {
            string sql = "select 物料编码,物料名称,规格型号 from  基础数据物料信息表";
            DataTable dt_物料 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            searchLookUpEdit2.Properties.DataSource = dt_物料;
            searchLookUpEdit2.Properties.DisplayMember = "物料编码";
            searchLookUpEdit2.Properties.ValueMember = "物料编码";

            sql = "select 供应商ID,供应商名称 from  采购供应商表 ";
            DataTable dt_供应商 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            searchLookUpEdit1.Properties.DataSource = dt_供应商;
            searchLookUpEdit1.Properties.DisplayMember = "供应商ID";
            searchLookUpEdit1.Properties.ValueMember = "供应商名称";
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

        private string fun_check()
        {
            string sql_补 = "";
            if (checkBox1.Checked)
            {
                if (dateEdit1.EditValue != null || dateEdit1.EditValue.ToString() != "")
                {
                    sql_补 += string.Format(" and 生效日期>'{0}'", dateEdit1.EditValue);
                }
                DateTime t = Convert.ToDateTime(dateEdit2.EditValue).Date.AddDays(1).AddSeconds(-1);
                if (dateEdit2.EditValue != null && dateEdit2.EditValue.ToString() != "")
                {
                    sql_补 += string.Format(" and 生效日期<'{0}'", t);
                }
            }
            if (checkBox2.Checked)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择供应商");
                }
                else
                {
                    sql_补 += string.Format(" and 供应商 like '%{0}%' ", searchLookUpEdit1.EditValue);
                }

            }
            if (checkBox3.Checked)
            {
                if (textBox1.Text == null || textBox1.Text.ToString() == "")
                {
                    throw new Exception("未填写采购单号");
                }
                else
                {
                    sql_补 += string.Format(" and  采购单号 like '%{0}%'", textBox1.Text.Trim());
                }
            }
            if (checkBox4.Checked)
            {
                if (searchLookUpEdit2.EditValue == null || searchLookUpEdit2.EditValue.ToString() == "")
                {
                    throw new Exception("未选择物料编码");
                }
                else
                {
                    sql_补 += string.Format(" and 物料编码 = '{0}'", searchLookUpEdit2.EditValue);
                }

            }
            if (checkBox5.Checked)
            {
                if (textBox2.Text == null || textBox2.Text.ToString() == "")
                {
                    throw new Exception("未填写送检单号");
                }
                else
                {
                    sql_补 += string.Format(" and 送检单号 like '%{0}%'", textBox2.Text.Trim());
                }
            }




            return sql_补;
             
        }

        private void fun_search(string s_条件)
        {
            string sql = string.Format(" select *,-送检数量-累计拒收核销数量 as 可核销数量,-送检数量 as 拒收数量 from  采购记录采购送检单明细表 where 拒收核销确认 = 0 and 送检单类型 = '拒收' and 拒收核销关闭 = 0 {0}", s_条件);
            dt_未核销明细 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            DataColumn dc = new DataColumn("选择", typeof(bool));
            dc.DefaultValue = false;
            dt_未核销明细.Columns.Add(dc);

            gridControl1.DataSource = dt_未核销明细;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridView2.CloseEditor();
                this.BindingContext[dt_未核销明细].EndCurrentEdit();
                DataView dv = new DataView(dt_未核销明细);
                dv.RowFilter = "选择 = true";
                if (dv.Count == 0)
                {
                    throw new Exception("未勾选拒收明细");
                }
                DateTime t = CPublic.Var.getDatetime();
                if (MessageBox.Show("是否确认核销勾选的拒收单？", "请确认!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string sql = "select * from  采购拒收核销记录表 where 1<>1";
                    DataTable dt_拒收核销记录 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    
                    foreach (DataRow dr in dt_未核销明细.Rows)
                    {
                        if (!Convert.ToBoolean(dr["选择"])) continue;
                        if (Convert.ToDecimal(dr["可核销数量"]) > (Convert.ToDecimal(dr["拒收数量"]) - Convert.ToDecimal(dr["累计拒收核销数量"])))
                        {
                            throw new Exception("核销数量超出拒收数量");
                        }
                        dr["累计拒收核销数量"] = Convert.ToDecimal(dr["可核销数量"]) + Convert.ToDecimal(dr["累计拒收核销数量"]);
                        if (Convert.ToDecimal(dr["累计拒收核销数量"]) == Convert.ToDecimal(dr["拒收数量"]))
                        {
                            dr["拒收核销确认"] = true;
                        }
                        DataRow dr_核销记录 = dt_拒收核销记录.NewRow();
                        dt_拒收核销记录.Rows.Add(dr_核销记录);
                        dr_核销记录["GUID"] = System.Guid.NewGuid();
                        dr_核销记录["送检单明细号"] = dr["送检单明细号"].ToString();
                        dr_核销记录["物料编码"] = dr["物料编码"].ToString();
                        dr_核销记录["核销数量"] = Convert.ToDecimal(dr["可核销数量"].ToString());
                        dr_核销记录["核销人员"] = CPublic.Var.localUserName;
                        dr_核销记录["核销人员ID"] = CPublic.Var.LocalUserID;
                        dr_核销记录["核销时间"] = t;
                    }

                    SqlConnection conn = new SqlConnection(strconn);
                    conn.Open();
                    SqlTransaction thrk = conn.BeginTransaction("拒收核销");
                    try
                    {
                        string sql1 = "select * from 采购记录采购送检单明细表 where 1<>1";
                        SqlCommand cmd1 = new SqlCommand(sql1, conn, thrk);
                        SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(dt_未核销明细);

                        sql1 = "select * from 采购拒收核销记录表 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(dt_拒收核销记录);
                       
                        thrk.Commit();
                        MessageBox.Show("核销成功");
                        simpleButton1_Click(null,null);

                    }
                    catch (Exception ex)
                    {
                        thrk.Rollback();
                        throw ex;
                    }

                }
                
                
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
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                    gridControl1.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void 关闭ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                string sql = string.Format("select * from 采购记录采购送检单明细表 where 送检单明细号 = '{0}' and 拒收核销确认 = 0 and 送检单类型 = '拒收' and 拒收核销关闭 = 0 ", dr["送检单明细号"]);
                DataTable dt_关闭 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                if (MessageBox.Show("是否确认关闭核销此单据？", "请确认!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    if (dt_关闭.Rows.Count == 0)
                    {
                        throw new Exception("该单据状态已更改，请重新查询");
                    }
                    else
                    {
                        dt_关闭.Rows[0]["拒收核销关闭"] = true;
                        dt_关闭.Rows[0]["拒收核销关闭人"] = CPublic.Var.localUserName;
                        dt_关闭.Rows[0]["拒收核销关闭时间"] = CPublic.Var.getDatetime() ;
                        string sql_关闭 = "select * from 采购记录采购送检单明细表 where 1<>1";
                        SqlDataAdapter da = new SqlDataAdapter(sql_关闭, strconn);
                        new SqlCommandBuilder(da);
                        da.Update(dt_关闭);
                        MessageBox.Show("关闭成功");
                        dt_未核销明细.Rows.Remove(dr);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gridControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                
            }
        }
    }
}
