using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERPproduct
{
    public partial class fm返工序列号 : Form
    {
        DataRow dr_参数;
        public fm返工序列号()
        {
            InitializeComponent();

        }
        public fm返工序列号(DataRow drp)
        {
            InitializeComponent();
            dr_参数 = drp;
        }
#pragma warning disable IDE1006 // 命名样式
        private void fm返工序列号_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_传值();

        }

        #region
#pragma warning disable IDE1006 // 命名样式
        private void fun_传值()
#pragma warning restore IDE1006 // 命名样式
        {
            label2.Text = dr_参数["返工原因"].ToString();
            label5.Text = dr_参数["数量"].ToString();

        }




        #endregion
        //enter 事件

#pragma warning disable IDE1006 // 命名样式
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            if (e.KeyCode == Keys.Enter)//如果输入的是回车键
            {
                barLargeButtonItem1_ItemClick(null, null);

            }

        }
        int i;
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //Button button2 = new Button();
            //button2.Click+=new EventHandler(button1_Click);
            try
            {
                if (Convert.ToInt32(dr_参数["数量"]) > Convert.ToInt32(label7.Text))
                {
                using (SqlDataAdapter da = new SqlDataAdapter("select * from 成品检验返工原因与产品序列号对应关系表 where 1<>1", CPublic.Var.strConn))
                {
                    DataTable test_暂存 = new DataTable();
                    da.Fill(test_暂存);
                    DataRow dr_新增行 = test_暂存.NewRow();
                    test_暂存.Rows.Add(dr_新增行);
                    if (textBox1.Text.ToString() != "")
                    {
                        dr_新增行["产品序列号"] = textBox1.Text.ToString();
                        dr_新增行["返工编号"] = dr_参数["返工编号"].ToString();
                        dr_新增行["返工原因"] = dr_参数["返工原因"].ToString();
                        dr_新增行["数量"] = 1;
                        new SqlCommandBuilder(da);
                        da.Update(test_暂存);
                       // MessageBox.Show("保存成功");
                    }
                }
                textBox1.Clear();
                if (Convert.ToInt32(label7.Text) == 0)
                {
                    i = 1;
                }
            

               label7.Text =(Convert.ToInt32(label7.Text)+i).ToString();
                }
            }
            catch { }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (Convert.ToInt32(label7.Text) == Convert.ToInt32(dr_参数["数量"]))
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("扫描序列号条数不正确,确定输入正确可点击右上方“X”关闭");
                return;
            }
        }
    }

}

