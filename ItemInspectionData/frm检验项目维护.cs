using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ItemInspectionData
{
    public partial class frm检验项目维护 : UserControl
    {
        #region 用户变量
        DataTable dtP;
        CurrencyManager cmM;
        #endregion

        #region 类自用
        public frm检验项目维护()
        {
            InitializeComponent();
        }
        private void frm检验项目维护_Load(object sender, EventArgs e)
        {
            try
            {
                fun_readData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 数据库操作
        /// <summary>
        /// 读
        /// </summary>
        private void fun_readData()
        {
            dtP = new DataTable();
            string sql = "SELECT * FROM [基础数据物料检验项目表] order by [POS]";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                try
                {
                    da.Fill(dtP);
                    gcM.DataSource = dtP;
                    cmM = this.BindingContext[dtP] as CurrencyManager;
                    gvM.ViewCaption = "检验项目：" + dtP.Rows.Count.ToString();
                    //dtP.TableNewRow += dtP_TableNewRow;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        //void dtP_TableNewRow(object sender, DataTableNewRowEventArgs e)
        //{
        //    e.Row["POS"] = dtP.Rows.Count + 1;
        //}
        /// <summary>
        /// 写
        /// </summary>
        private void fun_saveData()
        {
            string sql = "SELECT * FROM [基础数据物料检验项目表] where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                new SqlCommandBuilder(da);
                try
                {
                    da.Update(dtP);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        #region 数据处理
        /// <summary>
        /// 新增行
        /// </summary>
        private void fun_AddData()
        {
            try
            {
                cmM.AddNew();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 删除行
        /// </summary>
        private void fun_DeleteData()
        {
            try
            {
                (cmM.Current as DataRowView).Row.Delete();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void fun_Check()
        {
            int count = gvM.RowCount;
            for (int i = dtP.Rows.Count - 1; i >= 0; i--)
            {
                if (dtP.Rows[i].RowState == DataRowState.Deleted)
                {
                    continue;
                }
                dtP.Rows[i]["POS"] = count;
                count--;
            }
        }
        #endregion

        #region 界面相关
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gvM.CloseEditor();
            cmM.EndCurrentEdit();
            try
            {
                fun_AddData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gvM.CloseEditor();
            cmM.EndCurrentEdit();
            try
            {
                fun_DeleteData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gvM.CloseEditor();
            cmM.EndCurrentEdit();
            try
            {
                fun_Check();
                fun_saveData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                fun_readData();
                return;
            }
            fun_readData();
            MessageBox.Show("OK");
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                CPublic.UIcontrol.ClosePage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 显示行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvM_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString().Trim();
            }
        }
        #endregion
    }
}
