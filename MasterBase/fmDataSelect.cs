using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CZMaster
{
    public partial class fmDataSelect : Form
    {
        string sql;
        string strcaption;
        string valuemember;
        string strconn;

        public DataRow drResult = null;
        public string strResult;
        public DataTable dtResult;

        public fmDataSelect( string strCaption, string valueMember,string sql,string strConn)
        {
            this.sql = sql;
            this.strcaption = strCaption;
            this.valuemember = valueMember;
            this.strconn = strConn;
            InitializeComponent();
        }

        private void fmDataSelect_Load(object sender, EventArgs e)
        {
            this.Text = strcaption;
            DataTable dt = MasterSQL.Get_DataTable(sql, strconn);
            dtResult = dt;
            //gcM.DataSource = dtResult;
            gcM.DataSource = dt;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            drResult = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;
            strResult = drResult[valuemember].ToString();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void gcM_DoubleClick(object sender, EventArgs e)
        {
            button1_Click(null, null);
        }


    }
}
