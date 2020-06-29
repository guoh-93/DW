using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BaseData
{
    public partial class ui_新增编码_配置区域权限 : UserControl
    {
        DataTable dt_权限组;
        DataTable dt_区域;

        string strcon = CPublic.Var.strConn;


        public ui_新增编码_配置区域权限()
        {
            InitializeComponent();
        }
        private void fun_load()
        {
            string sql = @"select  * from 功能权限权限组表 "; 
            dt_权限组 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dt_权限组;
            string sql_仓库 = "select 属性值 as 区域  from  [基础数据基础属性表] where 属性类别 = '新增物料_区域'";
            dt_区域 = CZMaster.MasterSQL.Get_DataTable(sql_仓库, strcon);
            dt_区域.Columns.Add("选择", typeof(bool));
            gridControl2.DataSource = dt_区域;

             
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string sql = string.Format("select * from 新增编码_权限组区域配置表 where 权限组='{0}'", dr["权限组"]);
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                foreach (DataRow r in dt_区域.Rows)
                {
                    DataRow[] rr = dt.Select(string.Format("区域='{0}'", r["区域"]));
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }
        private void fun_save()
        {
            DataRow dr_权限组 = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            string sql_存 = $"select * from 新增编码_权限组区域配置表 where 权限组='{dr_权限组["权限组"].ToString()}' ";
            DataTable dt_存 = CZMaster.MasterSQL.Get_DataTable(sql_存, strcon);
        
            foreach (DataRow r in dt_区域.Rows)
            {
                if (r["选择"].Equals(true))
                {
                    string sql = string.Format("select * from 新增编码_权限组区域配置表 where 权限组='{0}' and  区域='{1}'", dr_权限组["权限组"], r["区域"]);
                    DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    if (dt.Rows.Count > 0)
                    {
                        continue;
                    }
                    else
                    {
                        DataRow dr_增 = dt_存.NewRow();

                        dr_增["工号"] = dr_权限组["权限组"];
                    
                        dr_增["区域"] = r["区域"];
    
                        dt_存.Rows.Add(dr_增);
                    }
                }
                else
                {
                    DataRow[] rrr = dt_存.Select(string.Format("权限组='{0}' and  区域='{1}'", dr_权限组["权限组"], r["区域"]));
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

            CZMaster.MasterSQL.Save_DataTable(dt_存, "新增编码_权限组区域配置表",strcon);
   
        }
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridView2.CloseEditor();
                this.BindingContext[dt_区域].EndCurrentEdit();
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

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_load();
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
