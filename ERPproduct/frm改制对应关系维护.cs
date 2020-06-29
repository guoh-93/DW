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
    public partial class frm改制对应关系维护 : UserControl
    {
        #region 变量  
        string strcon = CPublic.Var.strConn;
        DataTable dt_all;
        DataTable dt_左;
        DataTable dt_右;
        string str_物料编码="";

        #endregion 

        #region 加载
        public frm改制对应关系维护()
        {
            InitializeComponent();
        }
        public frm改制对应关系维护(string s)
        {
            InitializeComponent();
            this.str_物料编码 = s;
        }


#pragma warning disable IDE1006 // 命名样式
        private void frm改制对应关系维护_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_load_all();
            if (str_物料编码 != "")
            {
                gridView1.FocusedRowHandle = gridView1.LocateByDisplayText(0, gridColumn1, str_物料编码);
               
            }
            fun_load_右();

        }
        #endregion



#pragma warning disable IDE1006 // 命名样式
        private void fun_load_all()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = "select 物料编码,物料名称,原ERP物料编号,n原ERP规格型号,规格型号 from 基础数据物料信息表 where 自制=1 and 停用=0";
            dt_all = new DataTable();
            dt_all = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dt_all;
            
            repositoryItemSearchLookUpEdit1.DataSource = dt_all;
            repositoryItemSearchLookUpEdit1.ValueMember ="物料编码";
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";

        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_load_右()
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (dr != null)
            {
                string sql_右 = string.Format(@"select 改制对应关系表.*,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.规格型号 from 改制对应关系表 
                                                left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 =  改制对应关系表.可改制物料编码
                                                where 目标物料编码='{0}'", dr["物料编码"]);
                dt_右 = CZMaster.MasterSQL.Get_DataTable(sql_右, strcon);
                gridControl2.DataSource = dt_右;

            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_save()
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr_左=gridView1.GetDataRow (gridView1.FocusedRowHandle);

            foreach (DataRow dr in dt_右.Rows)
            {
                if (dr.RowState == DataRowState.Deleted)
                {
                    continue;
                }
                dr["目标物料编码"] = dr_左["物料编码"];
                dr["目标物料名称"] = dr_左["物料名称"];
            }
            string  sql="select * from 改制对应关系表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                new SqlCommandBuilder(da);
                da.Update(dt_右);
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (dt_右 != null)
            {
                DataRow dr = dt_右.NewRow();

                dt_右.Rows.Add(dr);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (dt_右 != null)
            {
                DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                dr.Delete();
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void repositoryItemSearchLookUpEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.NewValue != DBNull.Value && e.NewValue.ToString() != "")
            {
                string sql=string.Format("select  * from 基础数据物料信息表 where 物料编码='{0}'",e.NewValue);

                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                    dr["规格型号"] = dt.Rows[0]["规格型号"];
                    dr["原ERP物料编号"] = dt.Rows[0]["原ERP物料编号"];
                    dr["可改制物料名称"] = dt.Rows[0]["物料名称"];
                    dr["n原ERP规格型号"] = dt.Rows[0]["n原ERP规格型号"];

                    

                }
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_load_右();
        }
        //保存
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            gridView2.CloseEditor();
            this.BindingContext[dt_右].EndCurrentEdit();
            if (MessageBox.Show(string.Format("确认保存？"), "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                fun_save();
                MessageBox.Show("ok");
            }
        }
         //刷新
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_load_右();
        }
         //关闭
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
