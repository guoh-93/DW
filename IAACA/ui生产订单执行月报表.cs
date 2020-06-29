using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IAACA
{
    public partial class ui生产订单执行月报表 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        DataTable dt_gd;
        DataTable dt_rk;
        DataTable dtM;

        public ui生产订单执行月报表()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            bandedGridView1.Bands.Clear();
            bandedGridView1.Columns.Clear();
            gridControl1.DataSource = null;

            DateTime t1 = new DateTime(Convert.ToInt32(comboBox1.Text.Trim()),1, 1);
            DateTime t2 = t1.AddYears(1).AddSeconds(-1);

            search(t1, t2);
        }

        private void ui生产订单执行月报表_Load(object sender, EventArgs e)
        {
            DateTime time = CPublic.Var.getDatetime();
            int yearStart = 2019;
            for (; yearStart <= time.Year; yearStart++)
            {
                comboBox1.Items.Add(yearStart.ToString());
            }
            comboBox1.Text = time.Year.ToString();
        }

        private void search(DateTime t1, DateTime t2)
        {
            string s = string.Format(@"select  部门,物料编码,物料名称,规格型号,存货分类编码,存货分类,计量单位,月,sum(生产数量)生产数量 from (
select   case when 生产工单类型='小批试制' then '工艺部' else 车间名称 end as 部门,base.物料编码,base.物料名称,base.规格型号,存货分类编码,存货分类,base.计量单位
,DATEPART(mm,gd.生效日期)月,生产数量   from   生产记录生产工单表 gd 
left join 基础数据物料信息表 base on base.物料编码=gd.物料编码 where 生效日期 >'2019-5-1' and  生效日期 >'{0}' and 生效日期 <'{1}'  and (gd.关闭=0 or (gd.关闭=1 and 已检验数量>0)) )x
  group by 部门,物料编码,物料名称,规格型号,存货分类编码,存货分类,计量单位,月 ", t1, t2);
            ///生效日期 >'2019-5-1' 系统上线时间
            DataTable dt_生产 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataColumn[] pk_sc = new DataColumn[3];
            pk_sc[0] = dt_生产.Columns["物料编码"];
            pk_sc[1] = dt_生产.Columns["部门"];
            pk_sc[2] = dt_生产.Columns["月"];
            dt_生产.PrimaryKey = pk_sc;

            MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
            DataTable total_生产 = RBQ.SelectGroupByInto("", dt_生产, "部门,物料编码,物料名称,规格型号,存货分类编码,存货分类,计量单位,sum(生产数量) 生产总量 ", "", "部门,物料编码,物料名称,规格型号,存货分类编码,存货分类,计量单位");


            s = string.Format(@"select  部门,物料编码,物料名称,规格型号,存货分类编码,存货分类,计量单位,月,sum(入库数量)入库数量 from (
 select  case when 生产工单类型='小批试制' then '工艺部' else 车间名称 end as 部门,base.物料编码,base.物料名称,base.规格型号,存货分类编码,存货分类,base.计量单位
,DATEPART(mm,rk.生效日期)月, 入库数量 from 生产记录成品入库单明细表  rk
 left join 生产记录生产工单表 gd on rk.生产工单号=gd.生产工单号 
 left join 基础数据物料信息表 base on base.物料编码=gd.物料编码
 where rk.生效日期 >'2019-5-1'  and  rk.生效日期 >'{0}' and rk.生效日期 <'{1}')x
  group by 部门,物料编码,物料名称,规格型号,存货分类编码,存货分类,计量单位,月", t1, t2);
            DataTable dt_入库 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataColumn[] pk_rk = new DataColumn[3];
            pk_rk[0] = dt_入库.Columns["物料编码"];
            pk_rk[1] = dt_入库.Columns["部门"];
            pk_rk[2] = dt_入库.Columns["月"];
            dt_入库.PrimaryKey = pk_rk;
            RBQ = new MasterMESWS.DataSetHelper();
            DataTable total_入库 = RBQ.SelectGroupByInto("", dt_入库, "部门,物料编码,物料名称,规格型号,存货分类编码,存货分类,计量单位,sum(入库数量) 入库总量 ", "", "部门,物料编码,物料名称,规格型号,存货分类编码,存货分类,计量单位");

            dtM = new DataTable();
            dtM.Columns.Add("部门");
            dtM.Columns.Add("物料编码");
            dtM.Columns.Add("物料名称");
            dtM.Columns.Add("规格型号");
            dtM.Columns.Add("存货分类编码");
            dtM.Columns.Add("存货分类");
            dtM.Columns.Add("计量单位");
            dtM.Columns.Add("生产总量", typeof(decimal));
            dtM.Columns.Add("入库总量", typeof(decimal));
            for (int i = 1; i <= t2.Month; i++)
            {
                dtM.Columns.Add(i.ToString() + "月生产数量", typeof(decimal));
                dtM.Columns.Add(i.ToString() + "月入库数量", typeof(decimal));
            }
            DataColumn[] pk = new DataColumn[2];
            pk[0] = dtM.Columns["物料编码"];
            pk[1] = dtM.Columns["部门"];
            dtM.PrimaryKey = pk;

            foreach (DataRow dr in dt_生产.Rows)
            {
                DataRow[] sr_生产 = dtM.Select(string.Format("部门='{0}' and 物料编码='{1}' ", dr["部门"].ToString().Trim(), dr["物料编码"].ToString()));
                if (sr_生产.Length > 0)
                {
                    sr_生产[0][dr["月"].ToString() + "月生产数量"] = dr["生产数量"];

                }
                else
                {
                    DataRow r_生产 = dtM.NewRow();
                    r_生产["部门"] = dr["部门"];
                    r_生产["物料编码"] = dr["物料编码"];

                    r_生产["物料名称"] = dr["物料名称"];
                    r_生产["规格型号"] = dr["规格型号"];
                    r_生产["存货分类编码"] = dr["存货分类编码"];
                    r_生产["存货分类"] = dr["存货分类"];
                    r_生产["计量单位"] = dr["计量单位"];
                    r_生产[dr["月"].ToString() + "月生产数量"] = dr["生产数量"];

                    dtM.Rows.Add(r_生产);
                }
            }
            foreach (DataRow dr in dt_入库.Rows)
            {
                DataRow[] sr_入库 = dtM.Select(string.Format("部门='{0}' and 物料编码='{1}' ", dr["部门"].ToString().Trim(), dr["物料编码"].ToString()));
                if (sr_入库.Length > 0)
                {
                    sr_入库[0][dr["月"].ToString() + "月入库数量"] = dr["入库数量"];
                }
                else
                {
                    DataRow r入库 = dtM.NewRow();
                    r入库["部门"] = dr["部门"];
                    r入库["物料编码"] = dr["物料编码"];
                    r入库["物料名称"] = dr["物料名称"];
                    r入库["规格型号"] = dr["规格型号"];
                    r入库["存货分类编码"] = dr["存货分类编码"];
                    r入库["存货分类"] = dr["存货分类"];
                    r入库["计量单位"] = dr["计量单位"];
                    r入库[dr["月"].ToString() + "月入库数量"] = dr["入库数量"];
                    dtM.Rows.Add(r入库);
                }
            }

            foreach (DataRow r_总生产 in total_生产.Rows)
            {
                DataRow[] sr_生产 = dtM.Select(string.Format("部门='{0}' and 物料编码='{1}' ", r_总生产["部门"].ToString().Trim(), r_总生产["物料编码"].ToString()));
                sr_生产[0]["生产总量"] = r_总生产["生产总量"];
            }
            foreach (DataRow r_总入库 in total_入库.Rows)
            {
                DataRow[] sr_入库 = dtM.Select(string.Format("部门='{0}' and 物料编码='{1}' ", r_总入库["部门"].ToString().Trim(), r_总入库["物料编码"].ToString()));
                sr_入库[0]["入库总量"] = r_总入库["入库总量"];
            }


            foreach (DataColumn dc in dtM.Columns)
            {
                if (!dc.ColumnName.Contains("月") && !dc.ColumnName.Contains("总"))
                {
                    DevExpress.XtraGrid.Views.BandedGrid.GridBand gb = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
                    gb.Caption = dc.ColumnName;

                    bandedGridView1.Bands.Add(gb);

                    DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn bgc = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
                    bgc.FieldName = dc.ColumnName;
                    bgc.Caption = dc.ColumnName;
                    bgc.Visible = true;

                    gb.Columns.Add(bgc);
                    gb.Width = 130;
                    gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gb.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
                    bandedGridView1.Columns.Add(bgc);


                }
                //if (dc.ColumnName.Contains("月")) 
                //{
                //    gb.Caption = dc.ColumnName.Substring(0,dc.ColumnName.IndexOf("月"));

                //    DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn bgc1 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
                //    gb.Columns.Add(bgc1);

                //}
            }
            for (int i = 1; i <= t2.Month; i++)
            {
                DevExpress.XtraGrid.Views.BandedGrid.GridBand gb = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
                gb.Caption = i.ToString() + "月";


                DevExpress.XtraGrid.Views.BandedGrid.GridBand gb1 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
                gb1.Caption = "生产数量";

                DevExpress.XtraGrid.Views.BandedGrid.GridBand gb2 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
                gb2.Caption = "入库数量";

                DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn bgc1 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
                bgc1.DisplayFormat.FormatString = "#0.##";
                bgc1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                bandedGridView1.Columns.Add(bgc1);

                bgc1.FieldName = i.ToString() + "月生产数量";
                bgc1.Caption = "生产数量";

                bgc1.Visible = true;
                gb1.Columns.Add(bgc1);

                DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn bgc2 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
                bgc2.DisplayFormat.FormatString = "#0.##";
                bgc2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                bandedGridView1.Columns.Add(bgc2);


                bgc2.FieldName = i.ToString() + "月入库数量";
                bgc2.Caption = "入库数量";
                bgc2.Visible = true;
                gb2.Columns.Add(bgc2);


                gb.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb1, gb2 });



                bandedGridView1.Bands.Add(gb);
                gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gb.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
                gb2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gb2.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
                gb1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gb1.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;

            }
            //添加总计
            DevExpress.XtraGrid.Views.BandedGrid.GridBand gb_总计 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            gb_总计.Caption = "总计";


            DevExpress.XtraGrid.Views.BandedGrid.GridBand gb_总计1 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            gb_总计1.Caption = "总生产";

            DevExpress.XtraGrid.Views.BandedGrid.GridBand gb_总计2 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            gb_总计2.Caption = "总入库";

            DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn bgc_总计1 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            bgc_总计1.DisplayFormat.FormatString = "#0.##";
            bgc_总计1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            bandedGridView1.Columns.Add(bgc_总计1);

            bgc_总计1.FieldName = "生产总量";
            bgc_总计1.Caption = "总生产";

            bgc_总计1.Visible = true;
            gb_总计1.Columns.Add(bgc_总计1);

            DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn bgc_总计2 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            bgc_总计2.DisplayFormat.FormatString = "#0.##";
            bgc_总计2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            bandedGridView1.Columns.Add(bgc_总计2);


            bgc_总计2.FieldName = "入库总量";
            bgc_总计2.Caption = "总入库";
            bgc_总计2.Visible = true;
            gb_总计2.Columns.Add(bgc_总计2);


            gb_总计.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb_总计1, gb_总计2 });

            bandedGridView1.Bands.Add(gb_总计);
            gb_总计.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gb_总计.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            gb_总计2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gb_总计2.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            gb_总计1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gb_总计1.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            gridControl1.DataSource = dtM;

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
                DevExpress.XtraPrinting.XlsxExportOptionsEx op = new DevExpress.XtraPrinting.XlsxExportOptionsEx();

                op.ExportType = DevExpress.Export.ExportType.WYSIWYG;
                bandedGridView1.OptionsPrint.AutoWidth = false;
                gridControl1.ExportToXlsx(saveFileDialog.FileName, op);
        
                MessageBox.Show("导出成功");
            }
        }
    }
}
