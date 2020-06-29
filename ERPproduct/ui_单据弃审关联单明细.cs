using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.InteropServices;

namespace ERPproduct
{
    public partial class ui_单据弃审关联单明细 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dt_单据审核申请;
        DataTable dt_原始物料;
        DataTable dt_现物料;
        public ui_单据弃审关联单明细()
        {
            InitializeComponent();
        }

        private void ui_单据弃审关联单明细_Load(object sender, EventArgs e)
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(this.panel1, this.Name, cfgfilepath);
                DateTime t = CPublic.Var.getDatetime();
                barEditItem4.EditValue = t.Date.AddDays(1).AddSeconds(-1);
                barEditItem1.EditValue = t.Date.AddDays(-15);
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void fun_load()
        {
            DateTime t1 = Convert.ToDateTime(barEditItem1.EditValue).Date;
            DateTime t2 = Convert.ToDateTime(barEditItem4.EditValue).Date.AddDays(1).AddSeconds(-1);
            if (t1 > t2)
            {
                throw new Exception("开始时间不能大于结束时间");
            }
            string sql = string.Format("select * from 单据审核申请表 where 作废 = 0 and 审核 = 1 and 审核时间>='{0}' and 审核时间 <='{1}' and 操作类型 = '弃审'",t1,t2);
            dt_单据审核申请 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gridControl1.DataSource = dt_单据审核申请;
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string sql = string.Format("select 物料编码,物料名称,规格型号,数量 from 单据弃审关联单明细 where 审核申请单号 = '{0}'",dr["审核申请单号"]);
                dt_原始物料 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                gridControl2.DataSource = dt_原始物料;
                if(dr["单据类型"].ToString() == "销售单弃审申请")
                {
                    sql = string.Format(@"select 物料编码, 物料名称, 规格型号, 数量, 生效,生效日期 as 生效时间 from 销售记录销售订单明细表
                                          where 销售订单号 = '{0}' and 作废 = 0 ",dr["关联单号"]);
                    dt_现物料 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                }
                if (dr["单据类型"].ToString() == "销售预订单弃审申请")
                {
                    sql = string.Format(@"select 物料编码,物料名称,规格型号,数量,审核 as 生效,审核日期 as 生效时间  from 销售预订单明细表   a
                                          left join 销售预订单主表 b on a.销售预订单号 = b.销售预订单号
                                          where a.销售预订单号 = '{0}' and a.作废 = 0 ", dr["关联单号"]);
                    dt_现物料 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                }
                if (dr["单据类型"].ToString() == "借用申请单弃审申请")
                {
                    sql = string.Format(@"select 物料编码,物料名称,规格型号,申请数量 as 数量,审核 as 生效,审核日期 as 生效时间  from 借还申请表附表  a
                                          left join 借还申请表 b on a.申请批号 = b.申请批号
                                        where a.申请批号 = '{0}' and a.作废 = 0 ", dr["关联单号"]);
                    dt_现物料 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                }
                gridControl3.DataSource = dt_现物料;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        [DllImport("user32.dll")]
        public static extern int GetFocus();
        ///获取 当前拥有焦点的控件
        private Control GetFocusedControl()
        {
            Control c = null;
            // string focusedControl = null;
            IntPtr handle = (IntPtr)GetFocus();

            if (handle == null)
                this.FindForm().KeyPreview = true;
            else
            {
                c = Control.FromHandle(handle);//这就是
                //focusedControl =
                //c.Parent.TopLevelControl.Name.ToString();
            }

            return c;

        }
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Control c = GetFocusedControl();
            if (c != null && c.GetType().Equals(gridControl1.GetType()))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                    DevExpress.XtraGrid.GridControl gc = (c) as DevExpress.XtraGrid.GridControl;

                    gc.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            else
            {

                MessageBox.Show("若要导出请先选中要导出的表格(鼠标点一下表格)");
            }

        }
    }
}
