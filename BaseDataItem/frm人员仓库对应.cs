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
    public partial class frm人员仓库对应 : UserControl
    {
        DataTable dt_人;
        DataTable dt_仓库;

        string strcon = CPublic.Var.strConn;

        public frm人员仓库对应()
        {
            InitializeComponent();
        }

        private void fun_load()
        {
            string sql = @"select * from 人事基础员工表 where 部门编号='00010602'  and 在职状态 ='在职' and  权限组='仓库权限'"; // 人事和部门信息里面没有编号，组织架构维护后配上去的编号为 00010602
            dt_人 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
             gridControl1.DataSource = dt_人;
            string sql_仓库 = "select 属性值 as 仓库名称,属性字段1 as 仓库号 from  [基础数据基础属性表] where 属性类别 = '仓库类别'";
            dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_仓库, strcon);
            dt_仓库.Columns.Add("选择", typeof(bool));
            gridControl2.DataSource = dt_仓库;

        }
        private void fun_save()
        {
            DataRow dr_人员 = gridView1.GetDataRow(gridView1.FocusedRowHandle);

            string sql_存 = $"select * from 人员仓库对应表 where 工号='{dr_人员["员工号"].ToString()}' ";
            DataTable dt_存 = CZMaster.MasterSQL.Get_DataTable(sql_存, strcon);
            foreach (DataRow r in dt_仓库.Rows)
            {
                if (r["选择"].Equals(true))
                {
                    string sql = string.Format("select * from 人员仓库对应表 where 工号='{0}' and  仓库号='{1}'", dr_人员["员工号"], r["仓库号"]);
                    DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    if (dt.Rows.Count > 0)
                    {
                        continue;
                    }
                    else
                    {
                        DataRow dr_增 = dt_存.NewRow();
                        
                        dr_增["工号"] = dr_人员["员工号"];
                        dr_增["姓名"] = dr_人员["姓名"];
                        dr_增["仓库号"] = r["仓库号"];
                        dr_增["仓库名称"] = r["仓库名称"];

                        dt_存.Rows.Add(dr_增);
                    }
                }
                else
                {
                    DataRow[] rrr = dt_存.Select(string.Format("工号='{0}' and  仓库号='{1}'", dr_人员["员工号"], r["仓库号"]));
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


        private void frm人员仓库对应_Load(object sender, EventArgs e)
        {
            fun_load();


        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_load();
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (dr == null) return;
            string sql = string.Format("select * from 人员仓库对应表 where 工号='{0}'", dr["员工号"]);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            foreach (DataRow r in dt_仓库.Rows)
            {
                DataRow [] rr=dt.Select(string.Format("仓库号='{0}'", r["仓库号"]));
                if (rr.Length > 0)
                {
                    r["选择"] = true;
                }
                else
                {
                    r["选择"] = false;
                }
            }
        }
          //保存
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridView2.CloseEditor();
                this.BindingContext[dt_仓库].EndCurrentEdit();
                fun_save();
                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {

                throw new Exception("保存失败,刷新后重试");
            }
          
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (dr == null) return;

                gridView1_RowCellClick(null, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
