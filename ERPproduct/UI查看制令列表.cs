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
    public partial class UI查看制令列表 : UserControl
    {
        #region 变量
        string str_物料编码;
        string strcon = CPublic.Var.strConn;
        DateTime dtime = System.DateTime.Today;
         
        #endregion

        public UI查看制令列表( string s)
        {
            InitializeComponent();
            this.str_物料编码 = s;
            barEditItem1.EditValue = Convert.ToDateTime(dtime.AddMonths(-2).ToString("yyyy-MM-dd"));
            barEditItem2.EditValue = Convert.ToDateTime(dtime.ToString("yyyy-MM-dd"));

        }
        private void UI查看制令列表_Load(object sender, EventArgs e)
        {
            fun_load(str_物料编码);

        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_load(string s)
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format(@"select 生产记录生产制令表.*  from [生产记录生产制令表],基础数据物料信息表 
                                where    [生产记录生产制令表].物料编码= 基础数据物料信息表.物料编码  and 
                                生产记录生产制令表.关闭=0 and 生产记录生产制令表.日期>='{0}' and 生产记录生产制令表.日期<='{1}'
                                        and 生产记录生产制令表.物料编码='{2}'", barEditItem1.EditValue,Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1), str_物料编码);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dt = new DataTable();                  
                da.Fill(dt);
                gridControl1.DataSource = dt;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_load(str_物料编码);
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

    


    }
}
