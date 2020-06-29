using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 屈大海的DEMO
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

            devGridControlCustom1.UserName = CPublic.Var.LocalUserID;
            devGridControlCustom1.strConn = CPublic.Var.strConn;

            DataTable dt = CZMaster.MasterSQL.Get_DataTable("select * from 人事基础员工表 ", CPublic.Var.strConn);

            gcM.DataSource = dt;



        }

        private void button1_Click(object sender, EventArgs e)
        {
            CZMaster.fmGridControlCustom fm = new CZMaster.fmGridControlCustom();
            fm.strconn = CPublic.Var.strConn;
            fm.ShowDialog();
        }
    }
}
