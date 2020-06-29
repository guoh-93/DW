using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERPproduct
{
    public partial class frm修改制令 : Form
    {
        DataRow dr_制令 = null;
        string strconn = CPublic.Var.strConn;
        Decimal de_原 = 0;
        public Decimal de_差值 = 0;
        public Decimal de_现 = 0;
        public bool flag = false;  //指示是否保存

        public frm修改制令(DataRow dr)
        {
            InitializeComponent();
            dr_制令 = dr;
            de_原 = Convert.ToDecimal(dr["制令数量"]);
        }
#pragma warning disable IDE1006 // 命名样式
        private void frm修改制令_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = @"select 属性字段1 as 仓库编号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 = '仓库类别'order by 仓库编号 ";
            DataTable dt_仓库 = new DataTable();
            SqlDataAdapter   da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_仓库);
            searchLookUpEdit1.Properties.DataSource = dt_仓库;
            searchLookUpEdit1.Properties.DisplayMember = "仓库名称";
            searchLookUpEdit1.Properties.ValueMember = "仓库编号";

            sql = @" select 属性字段1 as 车间编号,属性值 as 车间名称 from 基础数据基础属性表 where 属性类别 = '生产车间' ";
            DataTable dt_车间 = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_车间);
            searchLookUpEdit2.Properties.DataSource = dt_车间;
            searchLookUpEdit2.Properties.DisplayMember = "车间名称";
            searchLookUpEdit2.Properties.ValueMember = "车间编号";

            sql = @" select  属性字段1 as 班组编号,属性值 as 班组 from  基础数据基础属性表  where 属性类别='班组'";
            DataTable  dt_班组 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            searchLookUpEdit3.Properties.DataSource = dt_班组;
            searchLookUpEdit3.Properties.DisplayMember = "班组";
            searchLookUpEdit3.Properties.ValueMember = "班组编号";

            dataBindHelper1.DataFormDR(dr_制令);


            dateEdit1.EditValue = dr_制令["预完工日期"];
        }
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            this.Close();
        }
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            this.ActiveControl = null;
            dataBindHelper1.DataToDR(dr_制令);
            dr_制令["仓库号"] = searchLookUpEdit1.EditValue;  
            dr_制令["仓库名称"] = searchLookUpEdit1.Text;
            dr_制令["班组"] = searchLookUpEdit3.Text;
            dr_制令["班组ID"] = searchLookUpEdit3.EditValue;


            dr_制令["生产车间"] = searchLookUpEdit2.EditValue;

            if (dateEdit1.EditValue != null && dateEdit1.EditValue != DBNull.Value)
            {
                dr_制令["预完工日期"] = dateEdit1.EditValue;
            }
            else
            {
                dr_制令["预完工日期"] = DBNull.Value;
            }
            if (de_原 < Convert.ToDecimal(dr_制令["制令数量"]))
            {
                de_差值 = Convert.ToDecimal(dr_制令["制令数量"]) - de_原;  
            }
            de_现 = Convert.ToDecimal(dr_制令["制令数量"]);
            flag = true;
            this.Close();
        }
    }
}
