using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ERPproduct
{
    public partial class 待发料查询 : UserControl
    {
        public 待发料查询()
        {
            InitializeComponent();
        }


        DataTable dtM;
        DataRow drM;


        private void simpleButton1_Click(object sender, EventArgs e)
        {


            string sql = @"select  销售送货明细表.*,销售记录销售订单主表.部门编号  from 销售送货明细表 
                           left join 销售记录销售订单主表 on 销售送货明细表.销售订单号 = 销售记录销售订单主表.销售订单号 where 1=1 ";
            if (checkBox9.Checked == true)
            {
                sql = sql + string.Format(@"and 创建日期>='{0}' and 创建日期<='{1}'",
                    dateEdit1.EditValue, Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1));
            }
            if (checkBox1.Checked == true)
            {
                sql = sql + string.Format(" and 客户编号='{0}'", searchLookUpEdit1.EditValue.ToString());
            }
            if (checkBox2.Checked == true)
            {
                sql = sql + string.Format(" and 大类='{0}'", searchLookUpEdit2.EditValue.ToString());

            }
            if (checkBox3.Checked == true)
            {
                sql = sql + string.Format(" and 小类='{0}'", searchLookUpEdit3.EditValue.ToString());
            }
   

            dtM = new DataTable();
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
            gridControl1.DataSource = dtM;



        }

        private void 待发料查询_Load(object sender, EventArgs e)
        {

            string sql = string.Format("select * from 生产记录生产工单表 where 关闭!='1'  ");
            DataTable dt_工单 = CZMaster.MasterSQL.Get_DataTable(sql,CPublic.Var.strConn);
            searchLookUpEdit3.Properties.DataSource = dt_工单;
            searchLookUpEdit3.Properties.DisplayMember = "生产工单号";
            searchLookUpEdit3.Properties.ValueMember = "生产工单号";



             sql = string.Format("select * from 生产记录生产工单待领料明细表 where 关闭!='1'  ");
            DataTable dt_领料 = CZMaster.MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
            searchLookUpEdit1.Properties.DataSource = dt_工单;
            searchLookUpEdit1.Properties.DisplayMember = "生产工单号";
            searchLookUpEdit1.Properties.ValueMember = "生产工单号";
            


        }
    }
}
