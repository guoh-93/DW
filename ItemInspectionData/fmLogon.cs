using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ItemInspectionData
{
    public partial class fmLogon : Form
    {
        public fmLogon()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            string strsql = "select * from 用户  where   用户ID = '{0}' and 密码 = '{1}'";
            strsql = string.Format(strsql, textBox1.Text, textBox2.Text);
            using (SqlDataAdapter da = new SqlDataAdapter(strsql, CPublic.Var.strConn))
            {
                try
                {
                    da.Fill(dt);
                    if (dt.Rows.Count != 0)
                    {
                        CPublic.Var.LocalUserID = textBox1.Text;
                        CPublic.Var.localUserName = dt.Rows[0]["用户名"].ToString();
                        CPublic.Var.LocalUserTeam = dt.Rows[0]["权限组"].ToString();
                        DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show("用户名或密码不正确");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
