using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class frm异常关闭 : Form
#pragma warning restore IDE1006 // 命名样式
    {
        public frm异常关闭()
        {
            InitializeComponent();
        }

        #region
        string strcon = CPublic.Var.strConn;
        DataRow r;
        public bool flag = false;  //指示是否保存
        public string str = "";

        public int 关闭 = 0;
        public string xiala = "";
        public string str_关闭原因 = "";
        #endregion
        public frm异常关闭(DataRow dr)
        {
            InitializeComponent();
            r = dr;
            this.StartPosition = FormStartPosition.CenterScreen;

            //20-5-21 这边只需检查在产在制品有没有  
            //用部分完工数量 
            string xx = $@" select dlmx.生产工单号 ,(已领数量-待领料总量/生产数量*isnull(部分完工数,0))as 在制品   from 生产记录生产工单待领料明细表 dlmx
left  join  生产记录生产工单表 gd on gd.生产工单号=dlmx.生产工单号
where   gd.生产工单号 ='{r["生产工单号"].ToString()}' ";
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(xx,strcon);
 
            //string sql_1 = string.Format("select * from 生产记录生产工单待领料明细表   where 生产工单号='{0}' ", r["生产工单号"].ToString());
            //DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);
 
            int i = 0;
            if(dt.Rows.Count > 0)
            {
                DataView dv = new DataView(dt);
                dv.RowFilter = "在制品<>0";
                if(dv.Count==0)
                {
                    textBox15.Text = "不需退料";

                }
                else
                {
                    textBox15.Text = "退料";
               
                }
                //foreach(DataRow dr_待领料 in dt.Rows)
                //{
                //    if(Convert.ToBoolean(dr_待领料["完成"]) == true && Convert.ToDecimal(dr_待领料["已领数量"]) > 0)
                //    {
                //        textBox15.Text = "退料";
                //        MessageBox.Show("该料已发料,需退料操作");
                   
                //    }
                //    else if (Convert.ToBoolean(dr_待领料["完成"]) == false && Convert.ToDecimal(dr_待领料["已领数量"]) > 0)
                //    {
                //        textBox15.Text = "退料";
                //        MessageBox.Show("该料已发料,需退料操作");
                //        break;
                //    }
                //    else
                //    {
                //        i++;
                        

                //    }
                //}
                //if(dt.Rows.Count == i)
                //{
                //    MessageBox.Show("该料未领过,无需进行退料操作");
                //    // comboBox1.Items.Add("报废");
                //    textBox15.Text = "不需退料";
                //}
            }
            //if (bool.Parse(r["生效"].ToString()) == true && dt.Rows.Count> 0)
            //{
              
               
            //   // comboBox1.Items.Add("退料");
            //    textBox15.Text = "退料";
            //    MessageBox.Show("该料已发料,需退料操作");

            //    //退料
            //}
            //else if (bool.Parse(r["生效"].ToString()) == true && dt.Rows.Count <= 0)
            //{

            //    MessageBox.Show("该料未领过,无法进行退料操作");
            //   // comboBox1.Items.Add("报废");
            //    textBox15.Text = "报废";

            //}
      
            else
            {
                //MessageBox.Show("该料未生效,无法进行退料报废操作");
              //  comboBox1.Items.Add("关闭");
                textBox15.Text = "关闭";
            }

        


        }
        
     
     

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                flag = true;
              
           
                xiala= textBox15.Text.ToString();
                str_关闭原因 = textBox16.Text.ToString();
                barLargeButtonItem1_ItemClick(null, null);
                关闭 = 1;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            this.Close();
            关闭 = 2;

        }

#pragma warning disable IDE1006 // 命名样式
        private void frm异常关闭_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            dataBindHelper1.DataFormDR(r);

           


        }
    }
}
