using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace IAACA
{
    public partial class ui手工赋单价 : UserControl
    {
        /// <summary>
        /// 1、材料出库红字
        /// 2、形态转换
        /// 3、拆单
        /// 4、退货入库
        /// 5、其他入库
        /// </summary>
        int x = 0;
        DataTable dtM;
        string strcon = CPublic.Var.strConn;
        int year_全局 = 0;
        int Month_全局 = 0;

        public ui手工赋单价()
        {
            InitializeComponent();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            x = 1;

            int year = year_全局 = Convert.ToInt32(textBox1.Text);
            int month = Month_全局 = Convert.ToInt32(textBox2.Text);
            DateTime t1 = new DateTime(year, month, 1); //结算月 初 
            // DateTime t2 = new DateTime(2019, 8, 1); //结算月 末
            DateTime t2 = t1.AddMonths(1); //结算月 末
            string sql = string.Format(@"select  rk.其他出库明细号 明细号,rk.出入库申请单号 申请号,物料编码,  数量,原因分类,结算单价,申请日期
            ,rksq.操作人员 as 申请人,rksq.备注 as 申请备注,业务单号 from 其他出库子表 rk
                    left join 其他出入库申请主表 rksq on rk.出入库申请单号 =rksq.出入库申请单号 
                      where rk.生效日期 > '{0}' and rk.生效日期 < '{1}' 
                     and 存货核算标记 =1 and 原因分类<>'入库倒冲'", t1, t2);
            dtM = new DataTable();
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dtM;

            gridControl1.MainView.PopulateColumns();

            gridView1.ViewCaption = "其他出库红字";
            fun_设置编辑状态();
        }
        //形态转换
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            x = 2;

            int year = year_全局 = Convert.ToInt32(textBox1.Text);
            int month = Month_全局 = Convert.ToInt32(textBox2.Text);
            DateTime t1 = new DateTime(year, month, 1); //结算月 初 
            // DateTime t2 = new DateTime(2019, 8, 1); //结算月 末
            DateTime t2 = t1.AddMonths(1); //结算月 末
            string sql = string.Format(@"  select x.*,b.物料编码 转换前编码,b.形态转换明细号 as 转换前明细号   from (
      select a.形态转换单号,a.形态转换明细号,类型,组号,a.物料编码,base.物料名称,base.规格型号,base.存货分类,base.存货分类编码,数量 from 销售形态转换子表 a
        left join 基础数据物料信息表 base on base.物料编码      =a.物料编码
    left join 销售形态转换主表 b on a.形态转换单号=b.形态转换单号 
    where b.审核日期>'{0}' and b.审核日期<'{1}' and 类型='转换后' )x 
    left join 销售形态转换子表 b on b.形态转换单号=x.形态转换单号 and x.组号=b.组号 
    and  b.类型='转换前' ", t1, t2);
            dtM = new DataTable();
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            gridControl1.DataSource = dtM;
            gridControl1.MainView.PopulateColumns();

            gridView1.ViewCaption = "形态转换";
            fun_设置编辑状态();
        }

        //拆单
        private void simpleButton4_Click(object sender, EventArgs e)
        {
            x = 3;
            int year = year_全局 = Convert.ToInt32(textBox1.Text);
            int month = Month_全局 = Convert.ToInt32(textBox2.Text);
            DateTime t1 = new DateTime(year, month, 1); //结算月 初 
            // DateTime t2 = new DateTime(2019, 8, 1); //结算月 末
            DateTime t2 = t1.AddMonths(1); //结算月 末
            string sql = string.Format(@" select 明细号,a.物料编码,数量,单价,base.物料名称,base.规格型号,base.存货分类,base.存货分类编码 from 仓库出入库明细表 a
                 left join 基础数据物料信息表 base on base.物料编码 = a.物料编码
                  where 出入库时间 > '{0}' and 出入库时间<'{1}'  and 明细类型 in ('拆单申请入库','拆单申请出库') and 实效数量<>0", t1, t2);
            dtM = new DataTable();
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            gridControl1.DataSource = dtM;
            gridControl1.MainView.PopulateColumns();

            gridView1.ViewCaption = "拆单";
            fun_设置编辑状态();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            x = 5;
            int year = year_全局 = Convert.ToInt32(textBox1.Text);
            int month = Month_全局 = Convert.ToInt32(textBox2.Text);
            DateTime t1 = new DateTime(year, month, 1); //结算月 初 
            // DateTime t2 = new DateTime(2019, 8, 1); //结算月 末
            DateTime t2 = t1.AddMonths(1); //结算月 末
            string sql = string.Format(@"select  rk.其他入库明细号 明细号,rk.出入库申请单号 申请号,物料编码,
                    数量,原因分类,结算单价,申请日期,rksq.操作人员 as 申请人,rksq.备注 as 申请备注 from 其他入库子表 rk
                    left join 其他出入库申请主表 rksq on rk.出入库申请单号 =rksq.出入库申请单号 
                      where rk.生效日期 > '{0}' and rk.生效日期 < '{1}' 
                     and 存货核算标记 =1 ", t1, t2);
            dtM = new DataTable();
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            gridControl1.DataSource = dtM;
            gridControl1.MainView.PopulateColumns();
            gridView1.ViewCaption = "其他入库";
            fun_设置编辑状态();

        }
        //退货入库
        private void simpleButton5_Click(object sender, EventArgs e)
        {
            x = 4;
            int year = year_全局 = Convert.ToInt32(textBox1.Text);
            int month = Month_全局 = Convert.ToInt32(textBox2.Text);
            DateTime t1 = new DateTime(year, month, 1); //结算月 初 
            // DateTime t2 = new DateTime(2019, 8, 1); //结算月 末
            DateTime t2 = t1.AddMonths(1); //结算月 末
            string sql = string.Format(@"select  明细号,a.物料编码,发出单价,实效数量,存货分类,存货分类编码,base.物料名称,base.规格型号,b.客户
                    ,b.出库数量 as 原单出库数量,ykp as 原单已开票数量,b.累计退货数量  from 仓库出入库明细表  a
                     left join 销售记录成品出库单明细表 b on a.明细号=b.成品出库单明细号 
                     left join 基础数据物料信息表 base on base.物料编码=a.物料编码 
                     left join ( select  成品出库单号,物料编码,SUM(已开票数量)ykp from 销售记录成品出库单明细表  group by 成品出库单号,物料编码)xx
                      on xx.成品出库单号=a.单号 and a.物料编码=xx.物料编码
                     where 明细类型 = '销售退货' and 出入库时间> '{0}' and 出入库时间<'{1}'", t1, t2);
            dtM = new DataTable();
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dtM;
            gridControl1.MainView.PopulateColumns();
            gridView1.ViewCaption = "销售退货";
            fun_设置编辑状态();
        }

        private void fun_设置编辑状态()
        {
            foreach (DevExpress.XtraGrid.Columns.GridColumn gc in gridView1.Columns)
            {
                if (gc.FieldName.Contains("单价"))
                {
                    gc.OptionsColumn.AllowEdit = true;
                }
                else
                {
                    gc.OptionsColumn.AllowEdit = false;
                    if (gc.FieldName.Contains("数量"))
                    {
                        gc.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                        gc.DisplayFormat.FormatString = "#0.######";
                    }
                }
            }

        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (gridView1.FocusedColumn.FieldName.Contains("单价"))
            {
                gridView1.OptionsBehavior.Editable = true;
            }
            else
            {
                gridView1.OptionsBehavior.Editable = false;

            }
        }

        private void ui手工赋单价_Load(object sender, EventArgs e)
        {
            DateTime t = CPublic.Var.getDatetime().Date;
            textBox1.Text = t.Year.ToString();
            textBox2.Text = t.Month.ToString();
        }
        //保存
        private void simpleButton6_Click(object sender, EventArgs e)
        {
            try
            {

                this.ActiveControl = null;
                fun_check();
                if (MessageBox.Show(string.Format("确认保存？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    fun_save(x);
                    MessageBox.Show("保存成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void  fun_check()
        {
            if(year_全局 != Convert.ToInt32(textBox1.Text)  ||  Month_全局!= Convert.ToInt32(textBox2.Text))
            {
                throw new Exception("界面上年月与查询时不一致,请确认");
            }
            string s = string.Format("select  count(*)x from 仓库月出入库结转表 where 年={0}  and 月={1}",year_全局,Month_全局);
            DataTable t = CZMaster.MasterSQL.Get_DataTable(s,strcon);
            if (Convert.ToInt32(t.Rows[0]["x"]) > 0) throw new Exception("该月已结转不可修改");
        }
        private void fun_save(int x)
        {
            int year = Convert.ToInt32(textBox1.Text);
            int month = Convert.ToInt32(textBox2.Text);
            DateTime t1 = new DateTime(year, month, 1); //结算月 初 
            // DateTime t2 = new DateTime(2019, 8, 1); //结算月 末
            DateTime t2 = t1.AddMonths(1); //结算月 末
            DataTable dt_save = new DataTable();
            if (x == 1) // 材料出库红字 //一般来说 红字回冲为1的 存货核算标记都是 1
            {
                string s = string.Format(@"select a.* from 其他出库子表 a
                left join 其他出入库申请主表 b on a.出入库申请单号 = b.出入库申请单号
                where b.红字回冲 = 1 and 存货核算标记=1 and a.生效日期 > '{0}' and a.生效日期<'{1}'", t1, t2);
                dt_save = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                DataView dv = new DataView(dtM);
                dv.RowStateFilter = DataViewRowState.ModifiedCurrent;
                foreach (DataRow dr in dv.ToTable().Rows)
                {
                    DataRow[] r = dt_save.Select(string.Format("其他出库明细号='{0}'", dr["明细号"]));
                    if (r.Length > 0)
                    {
                        r[0]["结算单价"] = dr["结算单价"];
                    }
                    else
                    {
                        throw new Exception("数据有误");
                    }
                }
                CZMaster.MasterSQL.Save_DataTable(dt_save, "其他出库子表", strcon);
            }
            else if (x == 2) //形态转换 暂不要
            {

            }
            else if (x == 3) //3  拆单
            {
                string s = string.Format(@"select a.* from 仓库出入库明细表 a
               where 出入库时间 > '{0}' and 出入库时间<'{1}'  and 明细类型 in ('拆单申请入库', '拆单申请出库') and 实效数量<>0", t1, t2);
                dt_save = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                DataView dv = new DataView(dtM);
                dv.RowStateFilter = DataViewRowState.ModifiedCurrent;

                foreach (DataRow dr in dv.ToTable().Rows)
                {
                    DataRow[] r = dt_save.Select(string.Format("明细号='{0}'", dr["明细号"]));
                    if (r.Length > 0)
                    {
                        r[0]["单价"] = dr["单价"];
                    }
                    else
                    {
                        throw new Exception("数据有误");
                    }
                }
                CZMaster.MasterSQL.Save_DataTable(dt_save, "仓库出入库明细表", strcon);

            }
            else if (x == 4) //4 销售退货
            {

                string s = string.Format(@" select b.*  from 销售记录成品出库单明细表 b
                     left join 仓库出入库明细表 a on a.明细号=b.成品出库单明细号 
                     where 明细类型 = '销售退货' and 出入库时间> '{0}' and 出入库时间<'{1}'", t1, t2);
                dt_save = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                DataView dv = new DataView(dtM);
                dv.RowStateFilter = DataViewRowState.ModifiedCurrent;

                foreach (DataRow dr in dv.ToTable().Rows)
                {
                    DataRow[] r = dt_save.Select(string.Format("成品出库单明细号='{0}'", dr["明细号"]));
                    if (r.Length > 0)
                    {
                        r[0]["发出单价"] = dr["发出单价"];
                    }
                    else
                    {
                        throw new Exception("数据有误");
                    }
                }
                CZMaster.MasterSQL.Save_DataTable(dt_save, "销售记录成品出库单明细表", strcon);

            }
            else if (x == 5)  //其他入库
            {


                string s = string.Format(@"select a.* from 其他入库子表 a
                left join 其他出入库申请主表 b on a.出入库申请单号 = b.出入库申请单号
                where  存货核算标记=1 and  a.生效日期 > '{0}' and a.生效日期<'{1}'", t1, t2);
                dt_save = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                DataView dv = new DataView(dtM);
                dv.RowStateFilter = DataViewRowState.ModifiedCurrent;
                foreach (DataRow dr in dv.ToTable().Rows)
                {
                    DataRow[] r = dt_save.Select(string.Format("其他入库明细号='{0}'", dr["明细号"]));
                    if (r.Length > 0)
                    {
                        r[0]["结算单价"] = dr["结算单价"];
                    }
                    else
                    {
                        throw new Exception("数据有误");
                    }
                }
                CZMaster.MasterSQL.Save_DataTable(dt_save, "其他入库子表", strcon);

            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView1.GetFocusedRowCellValue(gridView1.FocusedColumn));
                e.Handled = true;
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                gridControl1.ExportToXlsx(saveFileDialog.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
    }
}
