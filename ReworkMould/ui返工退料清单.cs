using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace ReworkMould
{
    public partial class ui返工退料清单 : System.Windows.Forms.Form
    {
        #region 
        /// <summary>
        /// 是否保存
        /// </summary>
        public bool issave = false;
        /// <summary>
        /// 此datatable 与 传入的dt_列表结构一致
        /// </summary>
        public DataTable dt_退料列表;
        
        #endregion 

        public ui返工退料清单()
        {
            InitializeComponent();
        

        }
        public ui返工退料清单(DataTable dt_列表)
        {
            InitializeComponent();
            dt_退料列表 = dt_列表.Copy();

        }
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            gridView1.CloseEditor();
            this.BindingContext[dt_退料列表].EndCurrentEdit();
            this.ActiveControl = null;
            issave = true;

            this.Close();
        }


        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void ui返工退料清单_Load(object sender, EventArgs e)
        {

            gridControl1.DataSource = dt_退料列表;

        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DataRow dr in dt_退料列表.Rows)
            {
                dr["选择"] = false;
            }
        }
    }
}
