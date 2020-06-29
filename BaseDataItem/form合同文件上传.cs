using CZMaster;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;


namespace BaseData
{
    public partial class form合同文件上传 : Form
    {
        public form合同文件上传()
        {
            InitializeComponent();
        }

        public form合同文件上传(DataRow dr )
        {
            InitializeComponent();
            dr_rowsNow = dr;

        }


        #region 成员
        DataTable dtM, dt_属性;               //主表
        DataTable dt_relationshipTable;
        SqlDataAdapter da;
        DataView dv;
        DataRow dr_rowsNow;
        string strconn = CPublic.Var.strConn;
        DataTable dt_客户;
        string strcon_FS = CPublic.Var.geConn("FS");
        //  strConn_FS
        DataTable dt_合同子表;
        #endregion
        private void form合同文件上传_Load(object sender, EventArgs e)
        {

        }








        private void fun_文件初始()
        {
            try
            {
                string sql = "selezct 属性值 from 基础数据基础属性表 where 属性类别 ='销售相关文件'";
                dt_属性 = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
                repositoryItemSearchLookUpEdit2.DataSource = dt_属性;
                repositoryItemSearchLookUpEdit2.DisplayMember = "属性值";
                repositoryItemSearchLookUpEdit2.ValueMember = "属性值";

                string sql1 = string.Format("select * from 合同相关文件表 where 销售开票通知单号='{0}'", dr_rowsNow["销售开票通知单号"]);
                dt_relationshipTable = MasterSQL.Get_DataTable(sql1, CPublic.Var.strConn);
                //cmM = this.BindingContext[dt1] as CurrencyManager;
                gcM1.DataSource = dt_relationshipTable;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_文件初始");
                throw new Exception(ex.Message);
            }

        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
     
    }
}
