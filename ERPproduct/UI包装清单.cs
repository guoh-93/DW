using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CPublic;
using System.Data.SqlClient;

namespace ERPproduct
{
    public partial class UI包装清单 : UserControl
    {


        # region    变量

        string strcon = CPublic.Var.strConn;

        #endregion


        public UI包装清单()
        {
            InitializeComponent();
        }

      

        private void UI包装清单_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }
         //刷新
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            string sql = "select 基础数据包装清单表.*,基础数据物料信息表.原ERP物料编号 from [基础数据包装清单表],基础数据物料信息表  where 1<>1";
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource=dt;
            textBox1.Focus();

        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }

        }

   

#pragma warning disable IDE1006 // 命名样式
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if(e.KeyChar==13)
            {
            if (textBox1.Text != null)
            {
                if (textBox1.Text.ToString() == "")
                {
                    //刷新 清空
                    barLargeButtonItem1_ItemClick(null, null);
                }
                else
                {
                    textBox1.SelectAll();
                    string sql = string.Format("select * from 生产记录生产工单表  where 生产工单号='{0}'", textBox1.Text);
                    DataTable dt = new DataTable();
                    dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    if (dt.Rows.Count > 0)
                    {
                        textBox2.Text = dt.Rows[0]["物料编码"].ToString();
                        textBox3.Text = dt.Rows[0]["物料名称"].ToString();
                        textBox4.Text = dt.Rows[0]["原规格型号"].ToString();
                        string sql_1 = string.Format(@"select 基础数据包装清单表.*,基础数据物料信息表.原ERP物料编号 from [基础数据包装清单表] 
                                                       left join   基础数据物料信息表 on 基础数据物料信息表.物料编码=基础数据包装清单表.物料编码
                                                        where 基础数据包装清单表.成品编码='{0}'", dt.Rows[0]["物料编码"].ToString());

                        DataTable dt_1 = new DataTable();
                        dt_1 = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);

                        gridControl1.DataSource = dt_1;


                    }
                    else
                    {
                        MessageBox.Show("未找到该条工单信息");
                        barLargeButtonItem1_ItemClick(null, null);


                    }

                }


                }
            }
        }

    
    }
}
