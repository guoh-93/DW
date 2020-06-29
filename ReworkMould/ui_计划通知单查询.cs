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

namespace ReworkMould
{
    public partial class ui_计划通知单查询 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dt_计划通知单;
        DataTable dt_计划通知单明细;

        public ui_计划通知单查询()
        {
            InitializeComponent();
        }

        private void ui_计划通知单查询_Load(object sender, EventArgs e)
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
                x.UserLayout(panel1, this.Name, cfgfilepath);
                DateTime t1 = CPublic.Var.getDatetime().Date.AddMonths(-3);
                DateTime t2 = CPublic.Var.getDatetime();

                barEditItem1.EditValue = t1;
                barEditItem2.EditValue = t2;

                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_load()
        {
            DateTime t1 = Convert.ToDateTime(barEditItem1.EditValue);
            DateTime t2 = Convert.ToDateTime(barEditItem2.EditValue);
            if (t1 > t2)
            {
                throw new Exception("时间输入有误，请确认");
            }
            string sql = $@"select a.*,b.物料名称,b.规格型号,b.存货分类,b.库存总数,b.未领量,b.委外在途,b.在途量,b.未发量,b.受订量,
                                b.默认仓库号,b.仓库名称,b.供应商编号,b.默认供应商,b.采购员,b.可购,b.委外,b.ECN,b.最小包装,b.停用,b.采购周期,
                                b.已采未审,b.采购未送检,b.已送未检,b.已检未入,b.库存下限    FROM 主计划计划通知单明细 a
                                left join V_pooltotal b ON a.物料编码 = b.物料编码 where 生效时间>='{t1}' and 生效时间 <='{t2}' ";
            dt_计划通知单明细 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl2.DataSource = dt_计划通知单明细;
        }

        //private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        //{
        //    try
        //    {
                 
        //        string sql = $@"select a.计划通知单明细号,a.物料编码,a.需求来料日期,a.预计开工日期,a.参考数量,a.通知采购数量,
        //                        a.已转采购数量,b.物料名称,b.规格型号,b.存货分类,b.库存总数,b.未领量,b.委外在途,b.在途量,b.未发量,b.受订量,
        //                        b.默认仓库号,b.仓库名称,b.供应商编号,b.默认供应商,b.采购员,b.可购,b.委外,b.ECN,b.最小包装,b.停用,b.采购周期,
        //                        b.已采未审,b.采购未送检,b.已送未检,b.已检未入,b.库存下限    FROM 主计划计划通知单明细 a
        //                        left join V_pooltotal b ON a.物料编码 = b.物料编码where 生效时间>='{t1}' and 生效时间 <='{t2}' ";
        //        dt_计划通知单明细 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
        //        gridControl2.DataSource = dt_计划通知单明细;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        //private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        //{
        //    try
        //    {
        //        DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
        //        string sql = $@"select a.计划通知单明细号,a.物料编码,a.需求来料日期,a.预计开工日期,a.参考数量,a.通知采购数量,
        //                        a.已转采购数量,b.物料名称,b.规格型号,b.存货分类,b.库存总数,b.未领量,b.委外在途,b.在途量,b.未发量,b.受订量,
        //                        b.默认仓库号,b.仓库名称,b.供应商编号,b.默认供应商,b.采购员,b.可购,b.委外,b.ECN,b.最小包装,b.停用,b.采购周期,
        //                        b.已采未审,b.采购未送检,b.已送未检,b.已检未入,b.库存下限    FROM 主计划计划通知单明细 a
        //                        left join V_pooltotal b ON a.物料编码 = b.物料编码 where 计划通知单号 = '{dr["计划通知单号"]}' ";
        //        dt_计划通知单明细 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
        //        gridControl2.DataSource = dt_计划通知单明细;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
