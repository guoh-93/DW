using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERPSale
{
    public partial class frm关闭订单窗口 : Form
    {
        public Boolean blResult = false;
        public Decimal decResult = 0;
        DataRow dr = null;
        DataTable dt = null;
        DataTable dt_记录 = null;
        string strconn = CPublic.Var.strConn;

        public frm关闭订单窗口()
        {
            InitializeComponent();
        }

        public frm关闭订单窗口(DataRow drM, DataTable dtP)
        {
            InitializeComponent();
            dr = drM;
            dt = dtP;
        }

        private void frm关闭订单窗口_Load(object sender, EventArgs e)
        {
            dataBindHelper1.DataFormDR(dr);
            fun_载入表结构();
            fun_读入订单详情();
        }

        private void fun_载入表结构()
        {
            string sql = "select * from 销售记录关闭订单记录表 where 1<>1";
            dt_记录 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_记录);
        }

        private void fun_保存()
        {
            string sql = "select * from 销售记录关闭订单记录表 where 1<>1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dt_记录);
            foreach (DataRow r in dt.Rows)
            {
                try
                {
                    r["关闭数量"] = Convert.ToDecimal(dt_记录.Select(string.Format("物料编码 = '{0}'", r["物料编码"].ToString()))[0]["数量"]);
                }
                catch
                {
                    r["关闭数量"] = 0;
                }
            }
        }

        private void fun_读入订单详情()
        {
            foreach (DataRow r in dt.Rows)
            {
                if (Convert.ToDecimal(r["未完成数量"]) > 0)
                {
                    DataRow rr = dt_记录.NewRow();
                    dt_记录.Rows.Add(rr);
                    rr["GUID"] = System.Guid.NewGuid();
                    rr["销售订单号"] = dr["销售订单号"];
                    rr["客户编号"] = dr["客户编号"];
                    rr["客户名称"] = dr["客户名"];
                    rr["物料编码"] = r["物料编码"];
                    rr["物料名称"] = r["物料名称"];
                    rr["数量"] = Convert.ToDecimal(r["未完成数量"]);
                    rr["原因"] = "默认原因 - " + CPublic.Var.LocalUserID;
                    rr["日期"] = System.DateTime.Now;
                }
            }
            gc.DataSource = dt_记录;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            gv.CloseEditor();
            gc.BindingContext[dt_记录].EndCurrentEdit();
            blResult = true;
            fun_保存();
            this.Close();
        }
    }
}
