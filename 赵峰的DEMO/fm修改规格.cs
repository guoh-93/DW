using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 赵峰的DEMO
{
    public partial class fm修改规格 : Form
    {
        string strconn = CPublic.Var.strConn;
        DataTable dt;

        public fm修改规格()
        {
            InitializeComponent();
        }

        private void fun()
        {
            string sql = "select * from [FMS].[dbo].[基础数据物料信息表],(SELECT 规格,COUNT(*) as nb FROM [FMS].[dbo].[基础数据物料信息表] where len(规格)>0  group by 规格) as new  where [基础数据物料信息表].规格 = new.规格 and new.nb>=2  order by [基础数据物料信息表].规格";
            dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt);
            int i = dt.Rows.Count;
            dv = new DataView(dt);
            dv.RowFilter = "已计算 = 0";
            gc.DataSource = dv;
        }
        DataView dv;
        private void fm修改规格_Load(object sender, EventArgs e)
        {
            devGridControlCustom1.UserName = CPublic.Var.LocalUserID;
            devGridControlCustom1.strConn = CPublic.Var.strConn;
            fun();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //save
            string sql = "select * from [FMS].[dbo].[基础数据物料信息表] where 1<>1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dt);
            MessageBox.Show("OK");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dv.Count != 0)
            {
                foreach (DataRowView rr in dv)
                {
                    textBox1.Text = rr["小类"].ToString();
                    textBox2.Text = rr["规格型号"].ToString();
                    textBox3.Text = rr["电压"].ToString();
                    textBox4.Text = rr["极数"].ToString();
                    textBox5.Text = rr["环保"].ToString();
                    if (textBox1.Text != "")
                    {
                        DataView dv2 = new DataView(dt);
                        string sss = string.Format("小类 = '{0}' and 规格型号 = '{1}'", textBox1.Text, textBox2.Text);
                        if (textBox3.Text != "" || textBox3.Text != null)
                        {
                            sss = sss + " and 电压 = '" + textBox3.Text + "'";
                        }
                        if (textBox4.Text != "" || textBox4.Text != null)
                        {
                            sss = sss + " and 极数 = '" + textBox4.Text + "'";
                        }
                        if (textBox5.Text != "" || textBox5.Text != null)
                        {
                            sss = sss + " and 环保 = '" + textBox5.Text + "'";
                        }
                        dv2.RowFilter = sss;
                        int i = 0;
                        foreach (DataRowView r in dv2)
                        {
                            i++;
                            r["新规格"] = r["规格"].ToString().Substring(0, r["规格"].ToString().Length - 1) + i.ToString();
                            r["型号子项"] = i;
                            r["已计算"] = true;
                        }
                        gc.DataSource = dv2;
                    }
                    break;
                }
                gc.DataSource = dv;
                button2_Click(null, null);
            }
            else
            {
                button1_Click(null, null);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            gc.DataSource = dv;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            gc.DataSource = dt;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (DataRow r in dt.Rows)
            {
                r["规格"] = r["新规格"];
            }
        }
    }
}
