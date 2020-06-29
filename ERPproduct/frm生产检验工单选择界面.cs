using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CZMaster;

namespace ERPproduct
{
    public partial class frm生产检验工单选择界面 : UserControl
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        string strcon = "";


        public frm生产检验工单选择界面()
        {
            InitializeComponent();
            strcon = CPublic.Var.strConn;
        }

        #region  变量

        /// <summary>
        /// 生产工单表
        /// </summary>
        DataTable dt_ProductOrder;

        /// <summary>
        /// 回传的dr
        /// </summary>
        public DataRow dr_PtOr=null;



        #endregion


#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 载入工单的数据：选择工单的条件：生效的，未检验数量大于0的
        /// </summary>
        private void fun_LoadGdData()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = "";
                //当两个时间都不为空的时候
                if (txt_riqi1.EditValue != null && txt_riqi2.EditValue != null && txt_riqi1.EditValue.ToString() != "" && txt_riqi2.EditValue.ToString() != "")
                {
                    if (Convert.ToDateTime(txt_riqi1.EditValue) > Convert.ToDateTime(txt_riqi2.EditValue))
                        throw new Exception("起始日期不能大于终止日期，请重新选择！");
                    sql = string.Format("select * from 生产记录生产工单表 where 生效=1 and 未检验数量>0 and 预计开工日期>='{0}' and 预计开工日期<='{1}'", txt_riqi1.EditValue, txt_riqi2.EditValue);
                }
                else
                {
                    sql = "select * from 生产记录生产工单表 where 生效=1 and 未检验数量>0";
                }
                dt_ProductOrder = MasterSQL.Get_DataTable(sql, strcon);
                dt_ProductOrder.Columns.Add("选择", typeof(bool));
                foreach (DataRow r in dt_ProductOrder.Rows)
                {
                    r["选择"] = false;
                }
                gc_scgd.DataSource = dt_ProductOrder;
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_LoadGdData");
                throw ex;
            }
        }


#pragma warning disable IDE1006 // 命名样式
        private void frm生产检验工单选择界面_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                devGridControlCustom1.UserName = CPublic.Var.LocalUserID;
                devGridControlCustom1.strConn = CPublic.Var.strConn;
                txt_riqi1.EditValue = System.DateTime.Today.AddDays(-7);
                txt_riqi2.EditValue = System.DateTime.Today.AddDays(1).AddSeconds(-1);
                fun_LoadGdData();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region  界面操作
        //查询操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_LoadGdData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //确定选择
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                gv_scgd.CloseEditor();
                this.BindingContext[dt_ProductOrder].EndCurrentEdit();

                foreach (DataRow r in dt_ProductOrder.Rows)
                {
                    if (r["选择"].Equals(true))
                    {
                        dr_PtOr = r;
                    }
                }
                this.ParentForm.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //工单只能选择一个，因此选择框需要互斥
#pragma warning disable IDE1006 // 命名样式
        private void repositoryItemCheckEdit1_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                foreach (DataRow r in dt_ProductOrder.Rows)
                {
                    if (r["选择"].Equals(true))
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

        //取消操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //this.ParentForm.Close();
            CPublic.UIcontrol.ClosePage();
        }

        #endregion







    }
}
