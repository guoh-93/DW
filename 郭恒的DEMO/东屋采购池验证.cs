using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
 using System.Threading;

namespace 郭恒的DEMO
{
    public partial class 东屋采购池验证 : Form
    {
        public 东屋采购池验证()
        {
            InitializeComponent();
        }
        DataTable dt_SaleOrder = new DataTable();
        int flag = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (flag == 0)
                {
                    var ofd = new OpenFileDialog();
                    if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        //  dt_SaleOrder = ERPorg.Corg.ExcelXLSX(ofd);
                        bool bl = ERPorg.Corg.IsFileInUse(ofd.FileName);
                        if (bl) throw new Exception("文件已打开或被占用中");

                        Thread th = new Thread(() =>
                        {
                            BeginInvoke(new MethodInvoker(() =>
                            {
                                button1.Enabled = false;
                                button1.Text = "导入中..";
                            }));
                            dt_SaleOrder = ERPorg.Corg.ReadExcelToDataTable(ofd.FileName);

                            dt_SaleOrder.Columns.Add("库存总数",typeof(decimal));

                            

                            int x = dt_SaleOrder.Rows.Count;
                            for (int i = x - 1; i >= 0; i--)
                            {
                                if (dt_SaleOrder.Rows[i]["物料编码"].ToString().Trim() == "")
                                {
                                    dt_SaleOrder.Rows.Remove(dt_SaleOrder.Rows[i]);
                                }
                            }
                            foreach (DataRow dr in dt_SaleOrder.Rows)
                            {
                                string s = string.Format("select  * from 仓库物料数量表  where 物料编码='{0}'", dr["物料编码"]);
                                DataTable tt = CZMaster.MasterSQL.Get_DataTable(s, CPublic.Var.strConn);
                                dr["库存总数"] = tt.Rows[0]["库存总数"];

                            }
                            flag = 1;
                            BeginInvoke(new MethodInvoker(() =>
                            {
                                button1.Text = string.Format("销售明细:{0}条", dt_SaleOrder.Rows.Count);
                            }));
                        });
                        th.Start();

                    }
                }
                else
                {
                    throw new Exception("请按步骤操作");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataTable dtP = dt_SaleOrder.Clone();
            //1. 先计算这个料的所有父项 
            DataTable dt = new DataTable();
            dt = ERPorg.Corg.fun_GetFather(dt, textBox1.Text,1,true);

            //2.匹配销售订单中记录和库存
            foreach (DataRow dr in dt.Rows)
            {
               DataRow []xr= dt_SaleOrder.Select(string.Format("物料编码='{0}'",dr["产品编码"]));
               foreach (DataRow rr in xr)
               {
                   dtP.ImportRow(rr);
               }
            }
            ERPorg.Corg.TableToExcel(dtP, @"C:\Users\GH\Desktop\111.xlsx");


        }
    }
}
