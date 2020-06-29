using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ERPSale
{
    public partial class ui过往出库明细 : UserControl
    {
        //只需起始日期
        DateTime t1;
     
       //物料编码
        string str_物料 = "";

        public ui过往出库明细(string ss,DateTime c_t1  )
        {
            InitializeComponent();
            t1 = c_t1;
       
            str_物料 = ss;
        }
        //public ui过往出库明细(string  str_客户,DateTime t)
        //{
        //    InitializeComponent();

        //}
        private void fun_load()
        {
            string s = string.Format(@"select  a.*  FROM 销售记录成品出库单明细表 a 
                left  join 基础数据物料信息表 b on  a.物料编码=b.物料编码
                left  join 销售记录成品出库单主表 c on  a.成品出库单号=c.成品出库单号 
                where b.物料编码 ='{0}' and c.生效日期>'{1}' order by c.成品出库单号", str_物料, t1);
            DataTable dt = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(s, CPublic.Var.strConn))
            {
                da.Fill(dt);
                gcP.DataSource = dt;
            }
        }

        private void ui过往出库明细_Load(object sender, EventArgs e)
        {
            fun_load();
        }

        private void gvP_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

    }
}
