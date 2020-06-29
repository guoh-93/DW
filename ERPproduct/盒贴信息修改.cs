using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using CPublic;


namespace ERPproduct
{
    public partial class 盒贴信息修改 : Form
    {

        string strcon = CPublic.Var.strConn;
        public 盒贴信息修改()
        {
            InitializeComponent();
        }



        //关闭
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            this.Close();

        }
        //保存
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_save();
            MessageBox.Show("ok");
        }

        private void 盒贴信息修改_Load(object sender, EventArgs e)
        {
            fun_load_工单下拉框();
            fun_mb();
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_save()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format(@"select * from 生产记录生产检验单主表  where 生产工单号='{0}'",
                                     searchLookUpEdit1.EditValue);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    dr["模板名称"] = comboBox1.Text;
                    dr["参数"] = textBox7.Text;
                    dr["产品型号"] = textBox1.Text;
                    dr["额定电压"] = textBox3.Text;
                    dr["机种"] = textBox6.Text;
                    dr["产品名称"] = textBox4.Text;
                    dr["订单号"] = textBox5.Text;
                    dr["盒装数量"] = textBox2.Text;
                    dr["客户料号"] = textBox8.Text;
                }
                new SqlCommandBuilder(da);
                da.Update(dt);
                
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_load_工单下拉框()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format("select * from 生产记录生产检验单主表 where 生效日期>'{0}'", System.DateTime.Today.AddMonths(-1).AddDays(-15));
            DataTable dt = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                da.Fill(dt);
                searchLookUpEdit1.Properties.DataSource = dt;
                searchLookUpEdit1.Properties.DisplayMember ="生产工单号";
                searchLookUpEdit1.Properties.ValueMember = "生产工单号";

            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_mb()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = "select * from 基础数据基础属性表 where 属性类别 = '盒贴模板'";
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            da.Fill(dt);
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow r in dt.Rows)
                {
                    if (r["属性类别"].ToString() == "盒贴模板")
                    {
                        comboBox1.Items.Add(r["属性值"].ToString());
                    }

                }
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format("select * from 生产记录生产检验单主表  where 生产工单号='{0}'", searchLookUpEdit1.EditValue);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    comboBox1.Text = dt.Rows[0]["模板名称"].ToString();
                    textBox7.Text = dt.Rows[0]["参数"].ToString();
                    textBox1.Text = dt.Rows[0]["产品型号"].ToString();
                    textBox3.Text = dt.Rows[0]["额定电压"].ToString();
                    textBox6.Text = dt.Rows[0]["机种"].ToString();
                    textBox4.Text = dt.Rows[0]["产品名称"].ToString();
                    textBox5.Text = dt.Rows[0]["订单号"].ToString();
                    textBox2.Text = dt.Rows[0]["盒装数量"].ToString();
                    textBox8.Text = dt.Rows[0]["客户料号"].ToString();

                }
                else
                {

                    comboBox1.Text = "";
                    textBox7.Text = "";
                    textBox1.Text = "";
                    textBox3.Text = "";
                    textBox6.Text = "";
                    textBox4.Text = "";
                    textBox5.Text = "";
                    textBox2.Text = "";
                    textBox8.Text = "";

                }

            }
        }
    }

}
