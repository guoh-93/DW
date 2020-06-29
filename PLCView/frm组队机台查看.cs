using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CZMaster;


namespace PLCView
{
    public partial class frm多机台查看 : UserControl
    {
        public frm多机台查看()
        {
            InitializeComponent();
        }

        DataTable dt_检测机台表;

        private void frm多机台查看_Load(object sender, EventArgs e)
        {

            string sql = string.Format("select * from 检测机台表 where 工控机='{0}'", System.Environment.MachineName);
            dt_检测机台表 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));

            //计算没一个缩略图的分辨率
            int h=0;
            int w=0;
            if (dt_检测机台表.Rows.Count > 0)
            {
                 h = this.Size.Height / dt_检测机台表.Rows.Count;
                 w = this.Size.Width / dt_检测机台表.Rows.Count;
            }
            //动态的添加每一个缩略图
            foreach (DataRow r in dt_检测机台表.Rows)
            {
                Panel p1 = new Panel();
                p1.Dock = DockStyle.Left;
                p1.Size = new Size(w, h);
                this.Controls.Add(p1);
                PLCView.frm缩略视图查看 fm = new PLCView.frm缩略视图查看(r["机台名称"].ToString());//接收机台的名称
                fm.Dock = DockStyle.Fill;
                p1.Controls.Add(fm);
            }
        }
    }
}
