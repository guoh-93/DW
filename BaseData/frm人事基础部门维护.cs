using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CPublic;
using CZMaster;


namespace BaseData
{
    public partial class frm人事基础部门维护 : UserControl
    {

        #region  公有成员


        #endregion



        #region 私有成员

        /// <summary>
        /// 主数据表,部门表
        /// </summary>
        DataTable dtM;

        CurrencyManager cmM;

        /// <summary>
        /// 进行主表的复制
        /// </summary>
        DataTable dt1;

        /// <summary>
        /// 人员表
        /// </summary>
        DataTable dtP;
        
        /// <summary>
        /// 提示信息
        /// </summary>
        string errormes = "";


       

        #endregion


        #region 类加载


        public frm人事基础部门维护()
        {
            InitializeComponent();
        }

        private void frm人事基础部门维护_Load(object sender, EventArgs e)
        {
            fun_主数据加载();
            dtP = MasterSQL.Get_DataTable("select 员工号,姓名 from  人事基础员工表", CPublic.Var.strConn);
            dt1 = dtM.Copy();

            repositoryItemSearchLookUpEdit1.DataSource = dt1;
            repositoryItemSearchLookUpEdit1.DisplayMember = "部门名称";
            repositoryItemSearchLookUpEdit1.ValueMember = "部门GUID";

            repositoryItemSearchLookUpEdit2.DataSource = dtP;
            repositoryItemSearchLookUpEdit2.DisplayMember = "姓名";
            repositoryItemSearchLookUpEdit2.ValueMember = "员工号";
        }



        #endregion


        #region  其他数据处理

        /// <summary>
        /// 新增行
        /// </summary>
        private void fun_AddRow()
        {
            try
            {
                cmM.AddNew();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }



        /// <summary>
        /// 数据的合法性的检查
        /// </summary>
        private Boolean fun_Check()
        {
            try
            {
                //检查部门编号是否为空
                foreach (DataRow r in dtM.Rows)
                {
                    if (r["部门编号"].ToString() =="")
                    {
                        errormes="部门编号有空值，请检查！";
                        return false;
                    }

                }

                //检查部门名称是否为空
                foreach (DataRow r in dtM.Rows)
                {
                    if (r["部门名称"].ToString() == "")
                    {
                        errormes="部门名称有空值，请检查！";
                        return false;
                    }
                }


                  //不能选择本部门作为上级部门
                foreach (DataRow r in dtM.Rows)
                {
                    if (r["部门GUID"].ToString().Equals(r["上级部门"].ToString()))
                    {
                        errormes="不能选本部门作为上级部门，请检查！";
                        return false;
                    }
                }


            }
            catch (Exception ex)
            {
                  errormes=ex.Message;
            }

            return true;
        }

        #endregion


        #region  数据库的读取与保存

        /// <summary>
        /// 数据库读取主数据的方法
        /// </summary>
        private void fun_主数据加载()
        {
                string sql = "select * from 人事基础部门表";
                dtM = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
                cmM = this.BindingContext[dtM] as CurrencyManager;
                gcM.DataSource = dtM;
        }


        /// <summary>
        /// 数据删除的方法
        /// </summary>
        private void fun_删除()
        {
  
            try
            {
                DataRow r = (this.BindingContext[dtM].Current as DataRowView).Row;

                if (MessageBox.Show(string.Format("你确定要删除部门\"{0}\"的数据吗？", r["部门名称"].ToString()), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    r.Delete();
                    MasterSQL.Save_DataTable(dtM, "人事基础部门表", CPublic.Var.strConn);
                    errormes = "删除成功！";
                }

            }
            catch(Exception ex)
            {
                errormes = ex.Message+"删除失败！";
            }


        }


        /// <summary>
        /// 部门信息新增或者修改之后的保存
        /// </summary>
        private void fun_保存()
        {
                try
                {
                    foreach (DataRow r in dtM.Rows)
                    {
                        if (r["部门GUID"] == DBNull.Value)  //如果GUID是空的话
                        {
                            r["部门GUID"] = System.Guid.NewGuid().ToString();
                        }
                    }

                    MasterSQL.Save_DataTable(dtM, "人事基础部门表", CPublic.Var.strConn);
                    errormes = "保存成功！";

                }
                catch (Exception ex)
                {
                    errormes=ex.Message+"保存失败！";

                }
 
        }

        #endregion


        #region   界面操作


        /// <summary>
        /// 界面刷新（数据库读取加载数据）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_主数据加载();

            dtP=MasterSQL.Get_DataTable("select 员工号,姓名 from  人事基础员工表",CPublic.Var.strConn);
            dt1 = dtM.Copy();
                  
            repositoryItemSearchLookUpEdit1.DataSource = dt1;
            repositoryItemSearchLookUpEdit1.DisplayMember = "部门名称";
            repositoryItemSearchLookUpEdit1.ValueMember = "部门GUID";

            repositoryItemSearchLookUpEdit2.DataSource = dtP;
            repositoryItemSearchLookUpEdit2.DisplayMember = "姓名";
            repositoryItemSearchLookUpEdit2.ValueMember = "员工号";
        }


        /// <summary>
        /// 新增操作（新增部门）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
               gvM.CloseEditor();
               cmM.EndCurrentEdit();    //先停止编辑 然后再新增行
               fun_AddRow();   //新增一行
        }



        /// <summary>
        /// 数据的删除操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
              gvM.CloseEditor();
              cmM.EndCurrentEdit();    //先停止编辑 然后再新增行
              fun_删除();
              fun_主数据加载();
        }


        /// <summary>
        /// 数据的保存操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
              gvM.CloseEditor();
              cmM.EndCurrentEdit();    //先停止编辑 然后再新增行


              if (fun_Check())
              {
                  fun_保存();
                  fun_主数据加载();
                  MessageBox.Show(errormes);
              }
              else
              {
                  MessageBox.Show(errormes);
              }
            
        }


        #endregion

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }









    }
}
