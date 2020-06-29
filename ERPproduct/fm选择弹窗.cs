using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class fm选择弹窗 : Form
#pragma warning restore IDE1006 // 命名样式
    {
        string flag="";
        string strcon = CPublic.Var.strConn;



        public fm选择弹窗( string s)
        {
            InitializeComponent();
            flag = s;
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {
            if (flag == "模具")
            {
                string sql = "select  模具编号,零件图号,工装编号,存放库位 from 模具管理基础信息表 where 在库状态='在库'";
                using (SqlDataAdapter da = new SqlDataAdapter(sql,strcon))
                {
                    DataTable dt =new DataTable ();
                    da.Fill(dt);
                    searchLookUpEdit1.Properties.DataSource = dt;
                    searchLookUpEdit1.Properties.DisplayMember = "模具编号";
                    searchLookUpEdit1.Properties.ValueMember = "模具编号";

                }
            }


        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_confirm()
#pragma warning restore IDE1006 // 命名样式
        {
            if (flag == "模具")
            {



            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fm选择弹窗_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_load();
            }
            catch ( Exception ex)
            {
                MessageBox.Show(ex.Message);          
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }









    }
}
