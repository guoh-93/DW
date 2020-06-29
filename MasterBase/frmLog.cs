using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CZMaster
{
    public partial class frmLog : UserControl
    {
        public  DataTable dtLog;

        public frmLog()
        {
            InitializeComponent();

            dtLog = new DataTable();
            dtLog.Columns.Add("time", System.DateTime.Now.GetType());
            dtLog.Columns.Add("Owner");
            dtLog.Columns.Add("Log");

            gcM.DataSource = dtLog;

            dtLog.RowChanged += dtLog_RowChanged;
        }

        void dtLog_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            try
            {
                gvM.FocusedRowHandle = dtLog.Rows.Count - 1;
            }
            catch
            {
            }
        }

        void Columns_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {

        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                lock (dtLog)
                {
                    dtLog.Clear();
                }
            }
            catch
            {

            }
        }
    }
}
