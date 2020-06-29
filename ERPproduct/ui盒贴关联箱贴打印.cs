using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ERPproduct
{
    public partial class ui盒贴关联箱贴打印 : UserControl
    {
        #region
        string strcon = CPublic.Var.strConn;
        DataTable dt_扫描队列;
        Dictionary<string, int> dic = new Dictionary<string, int>();
        int i_箱次 = 1;
        string str_通知单号 = "";
        #endregion
        public ui盒贴关联箱贴打印()
        {
            InitializeComponent();
            textBox1.Focus();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();

        }

        private void ui盒贴关联箱贴打印_Load(object sender, EventArgs e)
        {

            try
            {
                textBox7.Text = "1";

                fun_load();
                // dt_扫描队列.TableNewRow += Dt_扫描队列_TableNewRow;
                dt_扫描队列.RowChanged += Dt_扫描队列_RowChanged;
                textBox1.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Dt_扫描队列_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            try
            {
                decimal dec = 0;
                if (!decimal.TryParse(textBox3.Text, out dec)) throw new Exception("箱装数量不合法");
                if (dt_扫描队列.Rows.Count == dec)
                {
                    string Printer_箱贴 = CPublic.Var.li_CFG["printer_box"].ToString();
                    //打印箱贴
                    string pth_mb = System.Windows.Forms.Application.StartupPath + @"\prttmp\11-28箱贴模板_A5.xlsx";
                    DateTime t = CPublic.Var.getDatetime();

                    string s_箱装系统批号 = string.Format("XTDY{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("XTDY", t.Year, t.Month));

                    string sql = "SELECT  *  FROM  [Print_箱贴打印记录] where 1<>1 ";
                    DataTable dt111 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

                    foreach (DataRow dr in dt_扫描队列.Rows)
                    {
                        DataRow dr1 = dt111.NewRow();
                        dt111.Rows.Add(dr1);
                        dr1["箱装系统批号"] = s_箱装系统批号;
                        dr1["LockID"] = dr["LockID"];
                        dr1["CTNo1"] = dr["CTNo1"];
                        dr1["CTNo2"] = dr["CTNo2"];
                        dr1["箱次"] = i_箱次;
                        dr1["总箱数"] = Convert.ToInt32(textBox5.Text);
                        ;


                        dr1["打印时间"] = t;
                    }

                    CZMaster.MasterSQL.Save_DataTable(dt111, "Print_箱贴打印记录", strcon);
                    i_箱次 = Convert.ToInt32(textBox7.Text.ToString().Trim());
                    ItemInspection.print_FMS.print_箱贴(pth_mb, s_箱装系统批号, i_箱次++, Convert.ToInt32(textBox5.Text), textBox4.Text.Trim(), dt_扫描队列, false, Printer_箱贴);
                    textBox7.Text = i_箱次.ToString();
                    t = t.Date;
                    string s = string.Format("SELECT  箱装系统批号,打印时间  FROM  [Print_箱贴打印记录] where 打印时间 >='{0}' group by 箱装系统批号,打印时间", t);
                    DataTable dt_历史 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    gridControl2.DataSource = dt_历史;
                    //然后清空
                    dt_扫描队列 = dt_扫描队列.Clone();
                    gridControl1.DataSource = dt_扫描队列;
                    dt_扫描队列.RowChanged += Dt_扫描队列_RowChanged;
                    textBox4.Text = "";
                    label6.Text = "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }



        private void fun_load()
        {
            dt_扫描队列 = new DataTable();
            dt_扫描队列.Columns.Add("LockID");
            dt_扫描队列.Columns.Add("CTNo1");
            dt_扫描队列.Columns.Add("CTNo2");
            dt_扫描队列.Columns.Add("扫描时间", typeof(DateTime));

            DateTime t = CPublic.Var.getDatetime().Date;
            string s = string.Format("SELECT  箱装系统批号,打印时间  FROM  [Print_箱贴打印记录] where 打印时间 >='{0}' group by 箱装系统批号,打印时间", t);
            DataTable dt_历史 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gridControl2.DataSource = dt_历史;
        }
        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string x = textBox1.Text;
                    fun_识别(x);
                    textBox1.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_识别(string x)
        {
            string s = string.Format(@"select  * from [Print_发货SN号对应表] where SerialNum='{0}'
             or SerialNum2='{0}' or  LockID1='{0}' ", x); //这个是盒贴打印记录表
            DataTable dt_temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            if (dt_temp.Rows.Count == 0)
            {
                textBox1.Text = "";
                throw new Exception("未找到数据,请确认条码是否正确");
            }
            else
            {
                DataRow[] dr1 = dt_扫描队列.Select(string.Format("LockID = '{0}' ", dt_temp.Rows[0]["LockID1"]));
                if (dr1.Length > 0)
                {
                    textBox1.Text = "";
                    throw new Exception("该盒贴已扫描，请确认");
                }
                if (textBox4.Text.Trim() == "")
                {
                    s = string.Format(@"select a.*,b.客户名 from Print_发货SN号对应表 a
               left join 销售记录销售出库通知单主表 b on a.出库通知单号 = b.出库通知单号 where SerialNum='{0}'", dt_temp.Rows[0]["SerialNum"]);
                    DataTable tt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    if (tt.Rows.Count > 0)
                    {
                        textBox4.Text = tt.Rows[0]["客户名"].ToString();
                        str_通知单号 = tt.Rows[0]["出库通知单号"].ToString();
                        if (!dic.Keys.Contains(str_通知单号))
                        {
                            i_箱次 = 1;
                            dic.Add(str_通知单号, Convert.ToInt32(textBox5.Text.Trim()));

                        }
                        else
                        {
                            if (dic[str_通知单号] != Convert.ToInt32(textBox5.Text.Trim()))
                            {
                                throw new Exception("当前总箱数与同一发货单之前打印的总箱数不同,请检查");
                            }
                        }

                    }
                }

                //检查是否出库通知单号是否与界面一致 20-5-19
                string s_gl = dt_temp.Rows[0]["出库通知单号"].ToString().Trim().ToLower();
                string s_in = textBox6.Text.ToString().Trim().ToLower();
                if (s_gl != s_in) throw new Exception("扫描的这张盒贴识别有误");



                DataRow rr = dt_扫描队列.NewRow();
                rr["LockID"] = dt_temp.Rows[0]["LockID1"];
                rr["CTNo1"] = dt_temp.Rows[0]["SerialNum"];
                rr["CTNo2"] = dt_temp.Rows[0]["SerialNum2"];

                //19-12-18加
                rr["扫描时间"] = CPublic.Var.getDatetime();
                dt_扫描队列.Rows.Add(rr);
                label6.Text = dt_扫描队列.Rows.Count.ToString();
            }


            gridControl1.DataSource = dt_扫描队列;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {


                if (MessageBox.Show(string.Format("确认打印当前箱贴？当前扫描队列中有{0}盒", dt_扫描队列.Rows.Count), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    if (dt_扫描队列.Rows.Count == 0) throw new Exception("未有扫描产品不可打印");

                    string Printer_箱贴 = CPublic.Var.li_CFG["printer_box"].ToString();

                    DateTime t = CPublic.Var.getDatetime();
                    string s_箱装系统批号 = string.Format("XTDY{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("XTDY", t.Year, t.Month));

                    string sql = "SELECT  *  FROM  [Print_箱贴打印记录] where 1<>1 ";
                    DataTable dt111 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

                    foreach (DataRow dr in dt_扫描队列.Rows)
                    {
                        DataRow dr1 = dt111.NewRow();
                        dt111.Rows.Add(dr1);
                        dr1["箱装系统批号"] = s_箱装系统批号;
                        dr1["LockID"] = dr["LockID"];
                        dr1["CTNo1"] = dr["CTNo1"];
                        dr1["CTNo2"] = dr["CTNo2"];
                        //19-12-18加
                        //   dr1["扫描时间"] = CPublic.Var.getDatetime();
                        dr1["打印时间"] = t;
                    }
                    CZMaster.MasterSQL.Save_DataTable(dt111, "Print_箱贴打印记录", strcon);
                    //打印箱贴
                    string pth_mb = System.Windows.Forms.Application.StartupPath + @"\prttmp\11-28箱贴模板_A5.xlsx";
                    ItemInspection.print_FMS.print_箱贴(pth_mb, s_箱装系统批号, i_箱次++, Convert.ToInt32(textBox5.Text), textBox4.Text.Trim(), dt_扫描队列, false, Printer_箱贴);
                    textBox7.Text = i_箱次.ToString();
                    t = t.Date;
                    string s = string.Format("SELECT  箱装系统批号,打印时间  FROM  [Print_箱贴打印记录] where 打印时间 >='{0}' group by 箱装系统批号,打印时间", t);
                    DataTable dt_历史 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    gridControl2.DataSource = dt_历史;
                    //然后清空
                    dt_扫描队列 = dt_扫描队列.Clone();
                    gridControl1.DataSource = dt_扫描队列;
                    textBox4.Text = "";
                    label6.Text = "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void gridView2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {


                DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                string x = string.Format(" select  * from [Print_箱贴打印记录] where 箱装系统批号='{0}'", dr["箱装系统批号"]);
                DataTable dt_历史明细 = CZMaster.MasterSQL.Get_DataTable(x, strcon);
                gridControl3.DataSource = dt_历史明细;
                if (e != null && e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gridControl2, new Point(e.X, e.Y));

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gridView2_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                string x = string.Format(" select  * from [Print_箱贴打印记录] where 箱装系统批号='{0}'", dr["箱装系统批号"]);
                DataTable dt_历史明细 = CZMaster.MasterSQL.Get_DataTable(x, strcon);
                gridControl3.DataSource = dt_历史明细;
            }
            catch (Exception)
            {

            }
        }

        private void textBox1_KeyUp_1(object sender, KeyEventArgs e)
        {
            try
            {
                //20-2-21
                int in_总箱数 = 0;
                if (textBox5.Text.Trim() == "") throw new Exception("总箱数不可为空,请先录入总箱数");
                try
                {
                    int.TryParse(textBox5.Text.Trim(), out in_总箱数);
                }
                catch (Exception)
                {
                    throw new Exception("输入总箱数格式有问题");
                }

                if (e.KeyCode == Keys.Enter)
                {
                    string x = textBox1.Text;
                    fun_识别(x);
                    textBox1.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 补打ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                string x = string.Format(" select  * from [Print_箱贴打印记录] where 箱装系统批号='{0}'", dr["箱装系统批号"]);
                DataTable dt_历史明细 = CZMaster.MasterSQL.Get_DataTable(x, strcon);
                string sql = string.Format(@" select a.*, b.客户名 from Print_发货SN号对应表 a
                                              left join 销售记录销售出库通知单主表 b on a.出库通知单号 = b.出库通知单号 where LockID1 = '{0}'", dt_历史明细.Rows[0]["LockID"]);
                DataTable dt111 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);


                string Printer_箱贴 = CPublic.Var.li_CFG["printer_box"].ToString();
                //打印箱贴
                string pth_mb = System.Windows.Forms.Application.StartupPath + @"\prttmp\11-28箱贴模板_A5.xlsx";
                ItemInspection.print_FMS.print_箱贴(pth_mb, dt_历史明细.Rows[0]["箱装系统批号"].ToString(), Convert.ToInt32(dt_历史明细.Rows[0]["箱次"]), Convert.ToInt32(dt_历史明细.Rows[0]["总箱数"]), dt111.Rows[0]["客户名"].ToString(), dt_历史明细, false, Printer_箱贴);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gridControl2_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime t1 = Convert.ToDateTime(dateEdit1.EditValue).Date;
            DateTime t2 = Convert.ToDateTime(dateEdit2.EditValue).Date;
            string s = string.Format("SELECT  箱装系统批号,打印时间  FROM  [Print_箱贴打印记录] where 打印时间 >='{0}' and 打印时间<='{1}' group by 箱装系统批号,打印时间", t1, t2);
            DataTable dt_历史 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gridControl2.DataSource = dt_历史;
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {



        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {



        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
                { e.Handled = true; }
            }
            catch
            {


            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
                { e.Handled = true; }
            }
            catch
            {


            }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
                { e.Handled = true; }
            }
            catch
            {


            }
        }
    }
}
