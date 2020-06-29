using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace BaseData
{

    public partial class frm采购人员关联供应商 : UserControl
    {
        #region    变量
        string strcon = CPublic.Var.strConn;
        DataTable dt_采购人员;
        DataTable dt_供应商;

        #endregion
        public frm采购人员关联供应商()
        {
            InitializeComponent();
        }
        private void frm采购人员关联供应商_Load(object sender, EventArgs e)
        {
            fun_load();
        }

        private void fun_load()
        {
            // 课室='计划课' or   17/12/19去掉 计划采购部门分开 各司其职  
            //19 -4-2 revise
            string sql_人员 = string.Format(@"SELECT 员工号,姓名 FROM  人事基础员工表  where   部门='采购部' and 在职状态='在职'");
            using (SqlDataAdapter da = new SqlDataAdapter(sql_人员, strcon))
            {
                dt_采购人员 = new DataTable();
                da.Fill(dt_采购人员);
                gridControl1.DataSource = dt_采购人员;
            }
            string sql_供应商 = "SELECT *  FROM   [采购供应商表] where 供应商状态='在用'";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_供应商, strcon))
            {
                dt_供应商 = new DataTable();

                da.Fill(dt_供应商);
                dt_供应商.Columns.Add("选择", typeof(bool));
                gridControl2.DataSource = dt_供应商;



            }
        }
        private void fun_保存()
        {
            string sql_存="select * from [采购人员关联供应商表]";
            DataTable dt_存 = CZMaster.MasterSQL.Get_DataTable(sql_存, strcon);
            DataRow dr_人员 = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            foreach (DataRow r in dt_供应商.Rows)
            {
                if (r["选择"].Equals(true))
                {
                    string sql = string.Format("select * from [采购人员关联供应商表] where 员工号='{0}' and  供应商ID='{1}'", dr_人员["员工号"], r["供应商ID"]);
                    DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    if (dt.Rows.Count > 0)
                    {
                        continue;
                    }
                    else
                    {
                        DataRow dr_增= dt_存.NewRow();
                        dr_增["GUID"] = System.Guid.NewGuid();
                        dr_增["员工号"] = dr_人员["员工号"];
                        dr_增["姓名"] = dr_人员["姓名"];
                        dr_增["供应商ID"] = r["供应商ID"];
                        dr_增["供应商名"] = r["供应商名称"];

                        dt_存.Rows.Add(dr_增);
                    }
                }
                else
                {
                    DataRow []rrr= dt_存.Select(string.Format("员工号='{0}' and  供应商ID='{1}'",dr_人员["员工号"],r["供应商ID"])) ;
                    if (rrr.Length > 0)
                    {
                        rrr[0].Delete();

                    }
                    else
                    {
                        continue;
                    }

                }
            }

            SqlDataAdapter da = new SqlDataAdapter(sql_存, strcon);
            new SqlCommandBuilder(da);
            da.Update(dt_存);

        }
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_load();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        //保存
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridView2.CloseEditor();
                this.BindingContext[dt_供应商].EndCurrentEdit();
                fun_保存();
                MessageBox.Show("ok");
            }
            catch (Exception ex)
            {

                MessageBox.Show("保存失败");
            }
           
        }
         
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            string sql=string.Format("select * from [采购人员关联供应商表] where 员工号='{0}'",dr["员工号"]);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql,strcon);
            foreach (DataRow r in dt_供应商.Rows)
            {
                if (dt.Select(string.Format("供应商ID='{0}'", r["供应商ID"])).Length > 0)
                {
                    r["选择"] = true;

                }
                else
                {
                    r["选择"] = false;

                }
        
            }


        }

       

    }
}
