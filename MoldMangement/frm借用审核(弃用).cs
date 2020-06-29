using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace MoldMangement
{
    public partial class frm借用审核 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        DataTable dt_申请单;
        DataTable dt_借还申请表附表;
        public frm借用审核()
        {
            InitializeComponent();
        }

        private void frm借还审核_Load(object sender, EventArgs e)
        {
            try
            {
                fun_代办();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //string sql = "select * from 借还申请表 where 审核 = '0' and 借还状态 = '未审核'";
            
            //using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            //{
            //    dt_申请单 = new DataTable();
            //    da.Fill(dt_申请单);
            //}
            //gc1.DataSource = dt_申请单;
        }

        private void fun_代办()
        {
            string sql = string.Format("select 员工号,部门 from 人事基础员工表 where 员工号 = '{0}'",CPublic.Var.LocalUserID);
            DataTable dt_部门 =  CZMaster.MasterSQL.Get_DataTable(sql, strconn);

            if (CPublic.Var.LocalUserID == "admin")
            {
                sql = "select [借还申请表].*,人事基础员工表.部门 from [借还申请表] left join 人事基础员工表 on [借还申请表].[工号] = 人事基础员工表.员工号 where 审核 = '0' and 借还状态 = '未审核'";
                
            }
            else
            {
                sql = string.Format(@" select [借还申请表].*,人事基础员工表.部门 from [借还申请表] left join 人事基础员工表 on [借还申请表].[工号] = 人事基础员工表.员工号 where 部门 = '{0}' and 审核 = '0' and 借还状态 = '未审核'", dt_部门.Rows[0]["部门"]);
                
            }
            dt_申请单 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_申请单);
            gc1.DataSource = dt_申请单;

        }


        //审核
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);
               
                //string sql3 = "select * from 借还申请表 where 申请单号 = '" + dr["申请单号"].ToString() + "'";
               
                //string sql = "select * from 借还申请表附表 where 申请批号 = '" + dr["申请批号"].ToString()+ "'";
                //SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                //dt_借还申请表附表 = new DataTable();
                //da.Fill(dt_借还申请表附表);
                if (dt_借还申请表附表.Rows.Count > 0)
                {
                    foreach (DataRow r in dt_借还申请表附表.Rows)
                    {
                        r["借还状态"] = "未领取物料";
                    }
                }
                string sql2 = "select * from 借还申请表附表 where 1<>1";
                using (SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn))
                {
                    new SqlCommandBuilder(da2);
                    da2.Update(dt_借还申请表附表);
                }
                dr["审核"] = true;
                dr["借还状态"] = "未领取物料";
                dr["审核时间"] = CPublic.Var.getDatetime();
                dr["审核人员"] = CPublic.Var.localUserName;
                string sql1 = "select * from 借还申请表 where 1<>1";
                using (SqlDataAdapter da1 = new SqlDataAdapter(sql1, strconn))
                {
                    new SqlCommandBuilder(da1);
                    da1.Update(dt_申请单);
                }
                
                MessageBox.Show("审核完成");
                dt_申请单.Clear();
                dt_借还申请表附表.Clear();
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }


        }


        //关闭
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }


        private void gv1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);
            string sql = "select * from 借还申请表附表 where 申请批号 = '" + dr["申请批号"].ToString() + "'";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            dt_借还申请表附表 = new DataTable();
            da.Fill(dt_借还申请表附表);
            gc2.DataSource = dt_借还申请表附表;
        }


        private void gv1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        //刷新
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_代办();
        }



    }
}
