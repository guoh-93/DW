using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
using System.Text.RegularExpressions;

namespace MoldMangement
{
    public partial class UI查询 : UserControl
    {

        //static string strConn = "Password = a; Persist Security Info = True; User ID = sa; Initial Catalog = fms; Data Source=XINREN";
        string strConn = CPublic.Var.strConn;
        DataTable dt = new DataTable();
        DataTable dt_下表 = new DataTable();
        DataRow dr;

        public UI查询()
        {
            InitializeComponent();
        }

        private void fun_load()
        {
            fun_作废();
            //barEditItem1.EditValue = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            //barEditItem2.EditValue = DateTime.Now.ToString("yyyy-MM-dd");
            barEditItem1.EditValue = CPublic.Var.getDatetime().AddDays(-7).ToString("yyyy-MM-dd");
            barEditItem2.EditValue = CPublic.Var.getDatetime().AddDays(1).ToString("yyyy-MM-dd");
            barEditItem3.EditValue = "未完成";
            string a = Convert.ToString(barEditItem1.EditValue);
            string aa = Convert.ToString(barEditItem2.EditValue);
            string aaa = Convert.ToString(barEditItem3.EditValue);
            string sql = "select * from 计量器具申请主表 where 申请时间> ='" + a + "' and 申请时间< = '" + aa + "' and 作废 = 'false'and 完成 = 'false'";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
            {
                dt = new DataTable();
                da.Fill(dt);

            }
            gc1.DataSource = dt;
            dt_下表.Clear();
            
        }


        private void UI查询_Load(object sender, EventArgs e)
        {
            fun_load();
           
        }



        //导出
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {

                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions(TextExportMode.Text, false, false);
                gc1.ExportToXlsx(saveFileDialog.FileName, options);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
        //打印
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string str = "";
            //int t = 1;
            string str_打印机;
            //bool 生效;

            PrintDialog printDialog1 = new PrintDialog();
            printDialog1.UseEXDialog = true;
            this.printDialog1.Document = this.printDocument1;
            //printDialog1.Document = printDocument1;    
             if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                print.fun_申请(dt, dr, printDialog1.PrinterSettings.PrinterName, false);

            }
        }
        //关闭
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        //查询
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_查询();
            dt_下表.Clear();
        }

        private void fun_查询()
        {
            string a = Convert.ToString(barEditItem1.EditValue);
            string aa = Convert.ToString(barEditItem2.EditValue);
            string aaa = Convert.ToString(barEditItem3.EditValue);
            if (a != "" || a != null || aa != "" || aa != null)
            {
                //if (aaa == "已生效")
                //{
                //    string sql = "select * from 计量器具申请主表 where 申请时间>='" + a + "' and 申请时间<= '" + aa + "' and 生效 = 'true'";
                //    using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
                //    {
                //        dt = new DataTable();
                //        da.Fill(dt);

                //    }
                //    gc1.DataSource = dt;

                //}
                //else if (aaa == "未生效")
                //{
                //    string sql = "select * from 计量器具申请主表 where 申请时间>='" + a + "' and 申请时间<= '" + aa + "' and 生效 = 'false'";
                //    using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
                //    {
                //        dt = new DataTable();
                //        da.Fill(dt);

                //    }
                //    gc1.DataSource = dt;

                //}
                if (aaa == "已完成")
                {
                    string sql = "select * from 计量器具申请主表 where 申请时间>='" + a + "' and 申请时间<= '" + aa + "' and 完成 = 'true' and 作废 = 'false'";
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
                    {
                        dt = new DataTable();
                        da.Fill(dt);

                    }
                    gc1.DataSource = dt;
                    
                }
                else if (aaa == "未完成")
                {
                    string sql = "select * from 计量器具申请主表 where 申请时间>='" + a + "' and 申请时间<= '" + aa + "' and 完成 = 'false' and 作废 = 'false'";
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
                    {
                        dt = new DataTable();
                        da.Fill(dt);

                    }
                    gc1.DataSource = dt;

                }
                else if (aaa == "已作废")
                {
                    string sql = "select * from 计量器具申请主表 where 申请时间>='" + a + "' and 申请时间<= '" + aa + "' and 作废 = 'true' and 完成 = 'false'";
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
                    {
                        dt = new DataTable();
                        da.Fill(dt);

                    }
                    gc1.DataSource = dt;

                }
                else if (aaa == "未作废")
                {
                    string sql = "select * from 计量器具申请主表 where 申请时间>='" + a + "' and 申请时间<= '" + aa + "' and 作废 = 'false' and 完成 = 'false'";
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
                    {
                        dt = new DataTable();
                        da.Fill(dt);

                    }
                    gc1.DataSource = dt;

                }
                else if (aaa == "所有")
                {
                    string sql = "select * from 计量器具申请主表 where 申请时间>='" + a + "' and 申请时间<= '" + aa + "'";
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
                    {
                        dt = new DataTable();
                        da.Fill(dt);

                    }
                    gc1.DataSource = dt;
                    
                }
            }
            else {

                MessageBox.Show("日期不能为空，请选择日期进行查询");
            
            }

        }
        //点击事件
        private void gv1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            dt_下表.Clear();
            dr = gv1.GetDataRow(gv1.FocusedRowHandle);
            //dataBindHelper1.DataFormDR(dr);
            string sql3 = "select * from 计量器具申请明细表 where 申请单号= '" + dr["申请单号"].ToString() + "' order by 申请明细号 asc";
            //string sql2 = string.Format("select * from 计量器具申请明细表 where 申请单号='{0}' order by 申请明细号 asc", dr["申请单号"].ToString ());
            using (SqlDataAdapter da = new SqlDataAdapter(sql3, strConn))
            {
                da.Fill(dt_下表);
                //gc2 .DataSource =dt_右表 ;
            }
            gc2.DataSource = dt_下表;
        }

        private void gv1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gv2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void fun_作废()
        {
            DataTable dt_作废 = new DataTable();
            DateTime t = CPublic.Var.getDatetime().AddDays(-7);
            string sql = "select * from 计量器具申请主表 where 申请时间 <'" + t + "'and 完成 = 'false'";
            //string sql = string.Format("select * from 计量器具申请主表 where 申请时间='{0}'", t);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
            {
                dt_作废 = new DataTable();
                da.Fill(dt_作废);

            }
            // DataRow[] drr = dt.Select(string.Format("生效= 'false'"));
            foreach (DataRow r in dt_作废.Rows)
            {
            //    //int a = drr["申请时间"];
                //if (DateTime.Compare(t,) >= 0)
                //{
                
                //}
                r["作废"] = true;
                r["作废时间"] = CPublic.Var.getDatetime();
                //r["作废人员"] = CPublic.Var.localUserName;
            }
            

            string sql2 = "select * from 计量器具申请主表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql2, strConn))
            {

                new SqlCommandBuilder(da);
                da.Update(dt_作废);
            }
           
        
        }

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr;
            dr = gv1.GetDataRow(gv1.FocusedRowHandle);
            if (Convert.ToBoolean(dr["作废"].ToString()) == true && Convert.ToBoolean(dr["完成"].ToString()) == false)
            {
                MessageBox.Show("申请单已作废，无法修改");
            }
            else if (Convert.ToBoolean(dr["完成"].ToString()) == true && Convert.ToBoolean(dr["作废"].ToString()) == false)
             {
                 MessageBox.Show("申请单已完成，无法修改");

             }
             else
             {
                 UI修改 ui = new UI修改(dr);
                 CPublic.UIcontrol.Showpage(ui, "修改");
             }
        }

        private void gc1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc1, new Point(e.X, e.Y));

            }
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gv1.CloseEditor();//关闭编辑状态
            this.BindingContext[dt].EndCurrentEdit();//关闭编辑状态
            //if ()
            //{
            
            //}
            DataRow[] drr = dt.Select(string.Format("完成= 'true'"));
            DataRow[] drr2 = dt.Select(string.Format("作废= 'true'"));
            if (drr.Length >0)
            {               
                dr = gv1.GetDataRow(gv1.FocusedRowHandle);
              
                foreach (DataRow rr in drr)
                {
                    //DateTime t = Convert.ToDateTime(rr["完成时间"].ToString ());
                    //DateTime t_现在 = DateTime.Now;
                    if (Convert.ToBoolean(rr["作废"].ToString()) == true)
                    {
                        MessageBox.Show("有已经作废的申请单无法操作完成，请检查重新操作");

                        //fun_load();
                        return;
                    }
                    //else if (Convert.ToBoolean(rr["生效"].ToString()) == false)
                    //{
                    //    MessageBox.Show("该单没有审核生效，无法操作完成");
                    //    //fun_load();
                    //    return;
                    //}
                    else if (Convert.ToBoolean(rr["完成"].ToString()) == true )
                    {
                        if (rr["完成时间"].ToString() == null || rr["完成时间"].ToString() == "")
                        {
                            rr["完成"] = true;
                            rr["完成时间"] = CPublic.Var.getDatetime();
                        }
                        else
                        {
                            rr["完成"] = true;
                            rr["完成时间"] = rr["完成时间"];
                        }

                    }
                }
                   
              }
               
            else if(drr2.Length >0)
            {
                dr = gv1.GetDataRow(gv1.FocusedRowHandle);

                foreach (DataRow rr2 in drr2)
                {
                   
                    if (Convert.ToBoolean(rr2["完成"].ToString()) == true)
                    {
                        MessageBox.Show("有已经完成的申请单无法作废，请检查重新操作");                       
                        return;
                    }
                    
                    else if (Convert.ToBoolean(rr2["作废"].ToString()) == true)
                    {
                        if (Convert.ToInt32(rr2["总金额"].ToString()) > 0)
                        {
                            MessageBox.Show("作废申请单不可以输入金额");
                            return;
                        }
                        else if (rr2["作废时间"].ToString() == null || rr2["作废时间"].ToString() == "")
                        {
                            rr2["作废"] = true;
                            rr2["作废时间"] = CPublic.Var.getDatetime();
                            rr2["作废人员"] = CPublic.Var.localUserName;
                        }
                        else
                        {
                            rr2["作废"] = true;
                            rr2["作废时间"] = rr2["完成时间"];
                        }                       
                    } 
                }
               

            }
            else {
                MessageBox.Show("操作错误，没有勾选任何数据就直接确认");
                return;
            }

            string sql = "select * from 计量器具申请主表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
            {
                new SqlCommandBuilder(da);
                da.Update(dt);

            }
            fun_下保();
            //MessageBox.Show("确认成功");
            fun_load();

            
        }
        //刷新
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_load();
            dt_下表.Clear();
        }

        //private void fun_金额()
        //{
        //    if (dt.Rows.Count > 0)
        //    {
        //        foreach (DataRow r in dt.Rows)
        //        {
        //            if (Convert.ToBoolean(r["作废"].ToString()) == true)
        //            {
        //                this.gridColumn7.OptionsColumn.AllowEdit = false;
                        
        //            }
        //        }
        //    }
        //}
        private void fun_下保()
        {
            string sql = "select * from 计量器具申请明细表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
            {
                new SqlCommandBuilder(da);
                da.Update(dt_下表);

            }
            MessageBox.Show("确认成功");
           
        }


    }
}
