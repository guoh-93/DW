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
    public partial class frm成品入库单列表视图 : UserControl
    {
        #region 变量
        DataTable dtM;
        
        string strcon = CPublic.Var.strConn;
        string str_成品入库单号;


        #endregion


        #region 加载

        public frm成品入库单列表视图()
        {
            InitializeComponent();
        }
        public frm成品入库单列表视图(string str_成品入库单号)
        {
            InitializeComponent();
            this.str_成品入库单号 = str_成品入库单号;
        }


#pragma warning disable IDE1006 // 命名样式
        private void frm成品入库单列表视图_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_load();
        }

        #endregion

        #region 函数
#pragma warning disable IDE1006 // 命名样式
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = string.Format(@"select 生产记录成品入库单明细表.*,基础数据物料信息表.原ERP物料编号 from 生产记录成品入库单明细表 
                left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录成品入库单明细表.物料编码 
                where 成品入库单号='{0}'", str_成品入库单号);
                dtM = new DataTable();
                dtM = CZMaster.MasterSQL.Get_DataTable(sql,strcon);
                string sql_1 = string.Format("select * from 生产记录成品入库单主表 where 成品入库单号='{0}'", str_成品入库单号);
                DataRow dr = CZMaster.MasterSQL.Get_DataRow(sql_1,strcon);
                dataBindHelper1.DataFormDR(dr);
                gridControl1.DataSource = dtM;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_save()
#pragma warning restore IDE1006 // 命名样式
        {

            gridView1.CloseEditor();
            this.BindingContext[dtM].EndCurrentEdit();

            string sql = "select * from 生产记录成品入库单明细表 where1  ";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                new SqlCommandBuilder(da);
                da.Update(dtM);
            }
        }
        #endregion
        //关闭 
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }
        //保存
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_save();
            }
            catch (Exception ex)
            {
             CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_save");

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }




    }
}
