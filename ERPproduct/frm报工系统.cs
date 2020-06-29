using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class frm报工系统 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {
        DataTable dt_生产关系 = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);
        public frm报工系统()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            frm报工 fm1 = new frm报工();
            fm1.ShowDialog();
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            frm结工 fm2 = new frm结工();
            fm2.ShowDialog();
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton4_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            frm工时查看 fm1 = new frm工时查看();
            fm1.ShowDialog();
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton3_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            frm现场情况 frm1 = new frm现场情况();
            frm1.ShowDialog();
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_加载_顶层()
#pragma warning restore IDE1006 // 命名样式
        {
            //加载 车间领导  和 所有人员
            string sql = string.Format("select * from 人事基础员工表 where 部门编号='{0}' ", dt_生产关系.Rows[0]["生产车间"]);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {


                    //dt_生产关系 为 该车间所有人员 信息的dt
                    //dt_生产关系 = ERPorg.Corg.fun_hr("生产", dt.Rows[0]["员工号"].ToString());


                    string sql_部门领导 = string.Format("select * from [人事基础部门表] where 部门编号='{0}'", dt_生产关系.Rows[0]["生产车间"].ToString());
                    DataTable dt_bm = CZMaster.MasterSQL.Get_DataTable(sql_部门领导, CPublic.Var.strConn);

                    label4.Text = dt_bm.Rows[0]["部门名称"].ToString();


                    label5.Text = dt_bm.Rows[0]["领导姓名"].ToString();
                    label6.Text = "100%";



                }
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm报工系统_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_加载_顶层();
            }
            catch (Exception ex)
            {
                MessageBox.Show("该界面仅有车间人员有相应内容显示");
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void timer1_Tick(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            label7.Text = System.DateTime.Today.ToLongDateString() + "\n" + System.DateTime.Now.ToLongTimeString();
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton5_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            frm作业指导预览 frm = new frm作业指导预览();
            frm.ShowDialog();
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton6_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            frm快速补料界面 frm = new frm快速补料界面();

            frm.ShowDialog();

        }
    }
}
