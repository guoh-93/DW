using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.IO;



namespace ERPproduct
{
    public partial class 单独打印小标签界面 : Form
    {
        string strcon = CPublic.Var.strConn;
        DataTable dt_工单 = new DataTable();
        DataTable dt_hr = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);

        public 单独打印小标签界面()
        {
            InitializeComponent();
        }
        //关闭
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            this.Close();
        }


        //确认打印
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                gridView1.CloseEditor();
                this.BindingContext[dt_工单].EndCurrentEdit();
                
                fun_check();


                PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();

                this.printDialog1.Document = this.printDocument1;
                DialogResult pdr = this.printDialog1.ShowDialog();
                if (pdr == DialogResult.OK)
                {
                
                    string PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                    DataTable dtx = dt_工单.Clone();
                    for (int i = 0; i < gridView1.DataRowCount; i++)
                    {
                        dtx.ImportRow(gridView1.GetDataRow(i));
                    }
                    DataRow[] r= dtx.Select(string.Format("选择=1"));

            
                    //DataRow[] r = dt_工单.Select(string.Format("选择=1"));
                    if (r.Length > 0)
                    {
                        if (CPublic.Var.localUser课室编号 != "0001030103") 
                        {
                          
                           
                            foreach (DataRow dr in r)
                            {
                                fun_打印(dr["生产工单号"].ToString(), Convert.ToInt32(textBox1.Text), PrinterName);
                            }
                        }
                        else //制三课 标签打印不一样
                        {
                            frm工单生效选择 frm = new frm工单生效选择();
                            int i= Convert.ToInt32(textBox1.Text);
                            foreach (DataRow dr in r)
                            {
                                frm.fun_制三标签(dr,PrinterName,true,i);
                            }


                        }
                    }
                    else
                    {
                        MessageBox.Show("请勾选工单");
                    }
                }
            }
            catch (Exception ex)
            {
                
               MessageBox.Show(ex.Message);
            }
          

        }
        //加载
        private void 单独打印小标签界面_Load(object sender, EventArgs e)
        {
            try
            {
               DateTime dtime2= CPublic.Var.getDatetime();
               DateTime dtime1 = dtime2.AddMonths(-1);
               dtime1 = new DateTime(dtime1.Year, dtime1.Month, 1);
                dtime2= new DateTime(dtime2.Year, dtime2.Month,dtime2.Day);
               barEditItem1.EditValue = dtime1;
               barEditItem2.EditValue = dtime2;
               fun_load();

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error");
                this.Close();
            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                Convert.ToInt32(textBox1.Text);
            }
            catch (Exception ex)
            {

                throw new Exception("请正确输入数字");
            }
            
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_打印(string gdh, int count, string printName)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string path = Application.StartupPath + @"\Mode\标签1.lab";

                if (printName != null && printName == "")
                {
                    printName = new PrintDocument().PrinterSettings.PrinterName;

                }
             
                //DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);


                Dictionary<string,string> dic = new Dictionary<string,string>();
                dic.Add("fore", gdh);


                int a = (int)count / 12;

                if (a == 0)
                {
                    a = 12;
                }
                else if (count % 12 == 0)
                {
                    a = count+1;
                }
                else
                {
                    a = ((int)count / 12 + 1) * 12;
                }
           


                Lprinter lp = new Lprinter(path,dic,printName,a);
                FileStream aFile = new FileStream(Application.StartupPath + @"\Mode\log.txt", FileMode.OpenOrCreate);
                StreamReader sr = new StreamReader(aFile);
                lp.Left = int.Parse(sr.ReadLine());
                lp.Top = int.Parse(sr.ReadLine());
                sr.Close();
                aFile.Close();
                lp.Start();
                //lp.DoWork();
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message+"fun_打印");
            }


        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {
            if (dt_hr.Rows.Count > 0)
            {
                //                    string sql = string.Format(@"select 生产记录生产工单表.*,包装确认 from 生产记录生产工单表,生产记录生产检验单主表 
                //                                             where 生产记录生产检验单主表.生产工单号=生产记录生产工单表.生产工单号 and  制单日期>='{0}' and 生产记录生产工单表.生产车间='{1}' and  生产记录生产工单表.生效=1 and 包装确认=0",
                //                                                 System.DateTime.Today.AddDays(-7), dt_hr.Rows[0]["生产车间"].ToString());
                string sql = string.Format(@"select 生产记录生产工单表.*,包装确认,原ERP物料编号 from 生产记录生产工单表
                                            left  join 生产记录生产检验单主表 on  生产记录生产检验单主表.生产工单号 = 生产记录生产工单表.生产工单号 
                                            left join  基础数据物料信息表 on 基础数据物料信息表.物料编码= 生产记录生产工单表.物料编码
                                            where   生产记录生产工单表.生效日期>'{0}' and  生产记录生产工单表.生效日期<'{1}' and  基础数据物料信息表.车间编号='{2}'
                                             
                                             /*where  (包装确认=0 or 包装确认 is null) */",  barEditItem1.EditValue,Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1),
                                         dt_hr.Rows[0]["生产车间"].ToString());
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    dt_工单 = new DataTable();
                    dt_工单.Columns.Add("选择", typeof(bool));


                    da.Fill(dt_工单);

                    //searchLookUpEdit1.Properties.DataSource = dt_工单;
                    //searchLookUpEdit1.Properties.DisplayMember = "生产工单号";
                    //searchLookUpEdit1.Properties.DisplayMember = "生产工单号";
                    gridControl1.DataSource = dt_工单;
                }
            }
            else
            {
                MessageBox.Show("你不属于生产部门");
                this.Close();
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_load();

            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}
