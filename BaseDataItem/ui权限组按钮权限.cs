using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace BaseData
{
    public partial class ui权限组按钮权限 : UserControl
    {

        #region 变量
        string strcon = CPublic.Var.strConn;
        DataTable dt_pgroup, dt_ptype, dt_btn;

        #endregion
        public ui权限组按钮权限()
        {
            InitializeComponent();
        }
        private void ui权限组按钮权限_Load(object sender, EventArgs e)
        {
            dt_btn = new DataTable();
            string sql = "  select  属性值 as 按钮,选择=convert(bit,0)  from [基础数据基础属性表] where 属性类别='按钮'  ";
            dt_btn = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gc_权限组.DataSource = dt_btn;
            fun_load();
        }
        private void fun_load()
        {
            string s = "select 权限组 from 功能权限权限组表 ";
            dt_pgroup = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            searchLookUpEdit1.Properties.DataSource = dt_pgroup;
            searchLookUpEdit1.Properties.DisplayMember = "权限组";
            searchLookUpEdit1.Properties.ValueMember = "权限组";
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            string s = string.Format("select  *  from [功能权限权限组权限表] where 权限组='{0}'", searchLookUpEdit1.EditValue.ToString());
            dt_ptype = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gc_权限.DataSource = dt_ptype;
        }

        private void gv_权限_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gv_权限.GetDataRow(gv_权限.FocusedRowHandle); 
            string s = string.Format("select  * from 权限组按钮权限表 where 权限组='{0}' and 权限类型='{1}'", searchLookUpEdit1.EditValue.ToString(), dr["权限类型"]);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            foreach (DataRow x in dt.Rows)
            {
                dt_btn.Select(string.Format("按钮='{0}'", x["按钮"].ToString()))[0]["选择"] = true;

            }



        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确认保存修改？", "确认!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    this.ActiveControl = null;
                    fun_save();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message);
            }
        }

        private void fun_save()
        {
            DataRow dr = gv_权限.GetDataRow(gv_权限.FocusedRowHandle);
            string s = string.Format("select  * from 权限组按钮权限表 where 权限组='{0}' and 权限类型='{1}'", searchLookUpEdit1.EditValue.ToString(), dr["权限类型"]);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            foreach (DataRow r in dt_btn.Rows)
            {
                DataRow[] x = dt.Select(string.Format("按钮='{0}'", r["按钮"]));
                if (x.Length > 0 && r["选择"].Equals(false)) //
                {
                    x[0].Delete();
                }
                else if (x.Length == 0 && r["选择"].Equals(true))
                {

                    DataRow rr = dt.NewRow();
                    rr["权限组"] = searchLookUpEdit1.EditValue.ToString();
                    rr["权限类型"] = dr["权限类型"];
                    rr["按钮"] = r["按钮"];
                    dt.Rows.Add(rr);
                }
            }
            //更新dt 
            CZMaster.MasterSQL.Save_DataTable(dt, "权限组按钮权限表",strcon);


        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }



    }
}
