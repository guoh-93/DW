using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERPpurchase
{
    public partial class Form1 : Form
    {
        DataTable dtM;


        public Form1 ()
        {
           
        }
        public Form1(DataTable dt,string title)
        {
            InitializeComponent();
            dtM = dt;
            this.Text = title;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            gridControl1.DataSource = dtM;
            this.gridView1.Columns[1].BestFit();
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gridView1_KeyUp(object sender, KeyEventArgs e)
        {
           
                if (e.Control && e.KeyCode == Keys.C)
                {
                    Clipboard.SetDataObject(gridView1.GetFocusedRowCellValue(gridView1.FocusedColumn));
                    e.Handled = true;
                }
            
        }
    }
}
