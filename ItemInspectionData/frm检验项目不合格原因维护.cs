using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;

namespace ItemInspectionData
{
    public partial class frm检验项目不合格原因维护 : UserControl
    {
        #region 用户变量
        DataTable dtP, dtM;
        CurrencyManager cmM;
        #endregion

        #region 类自用
        public frm检验项目不合格原因维护()
        {
            InitializeComponent();
        }

        private void frm检验项目不合格维护_Load(object sender, EventArgs e)
        {
            fun_readData();
            fillCMD();
        }

        #endregion

        #region 数据库操作
        /// <summary>
        /// 读
        /// </summary>
        private void fun_readData()
        {
            dtP = new DataTable();
            string sql = "SELECT * FROM [基础数据检验项目不合格表]";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                try
                {
                    da.Fill(dtP);
                    gcM.DataSource = dtP;
                    cmM = this.BindingContext[dtP] as CurrencyManager;
                    gvM.ViewCaption = "检验项目不合格原因维护：" + dtP.Rows.Count.ToString();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        /// <summary>
        /// 下拉填充
        /// </summary>
        private void fillCMD()
        {
            dtM = new DataTable();
            string sql_1 = "SELECT * FROM [基础数据物料检验项目表] order by [POS]";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_1, CPublic.Var.strConn))
            {
                try
                {
                    da.Fill(dtM);
                    repositoryItemSearchLookUpEdit1.DataSource = dtM;
                    repositoryItemSearchLookUpEdit1.ValueMember = "检验项目";
                    repositoryItemSearchLookUpEdit1.DisplayMember = "检验项目";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        /// <summary>
        /// 写
        /// </summary>
        private void fun_saveData()
        {
            string sql = "SELECT * FROM [基础数据检验项目不合格表]";
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
        #endregion


        private void ShowData()
        {
            if (AddData.DataArray() == 0)
            {
                MessageBox.Show("OK");
            }
            else
            {
                MessageBox.Show("NO");
            }
        }

        #region 界面相关
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_AddData();
                gvM.CloseEditor();
                cmM.EndCurrentEdit();




                //print_Check.fun_print_Check("IC20150300044");
                //print_Unqualified.fun_print_Unqualified("IC20150300044");


            //    System.Windows.Forms.OpenFileDialog fd = new OpenFileDialog();
            //    try
            //    {
            //        if (fd.ShowDialog() == DialogResult.OK)
            //        {
            //            AddData.GUID = System.Guid.NewGuid().ToString();
            //            AddData.Operator = CPublic.Var.localUserName;
            //            AddData.Ofd = fd;
            //            Thread thread1 = new Thread(new ThreadStart(ShowData));
            //            thread1.Start();
            //        }
            //        else
            //        {
            //            MessageBox.Show("导入失败！");
            //        }
            //    }
            //    catch
            //    {
            //        MessageBox.Show("请选择相应的excel.xlsx文件", "警告");
            //    }
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
            try
            {
                fun_DeleteData();
                gvM.CloseEditor();
                cmM.EndCurrentEdit();
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
                fun_saveData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                fun_readData();
                return;
            }
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
        #endregion



    }
}
