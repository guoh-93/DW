using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
namespace ERPpurchase
{
    public partial class ui采购开票明细汇总 : UserControl
    {

        string strcon = CPublic.Var.strConn;
        string cfgfilepath = "";

        public ui采购开票明细汇总()
        {
            InitializeComponent();
        }

        private void ui采购开票明细汇总_Load(object sender, EventArgs e)
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
                x.UserLayout(this.panel2, this.Name, cfgfilepath);
                dateEdit1.EditValue = CPublic.Var.getDatetime().AddMonths(-1).ToString("yyyy-MM-dd");
                dateEdit2.EditValue = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");
                fun_load();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }


        private void fun_load()
        {

            string sql = string.Format(@"select 供应商ID,供应商名称 from 采购供应商表 where 供应商状态='在用'");
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            DataTable dt_客户 = new DataTable();
            da.Fill(dt_客户);
            searchLookUpEdit1.Properties.DataSource = dt_客户;
            searchLookUpEdit1.Properties.DisplayMember = "供应商名称";
            searchLookUpEdit1.Properties.ValueMember = "供应商ID";

            string sql1 = string.Format("select 物料编码,物料名称,规格型号 from 基础数据物料信息表 where 停用=0");
            DataTable dt_物料信息表 = CZMaster.MasterSQL.Get_DataTable(sql1,CPublic.Var.strConn);
            searchLookUpEdit2.Properties.DataSource = dt_物料信息表;
            searchLookUpEdit2.Properties.DisplayMember = "物料编码";
            searchLookUpEdit2.Properties.ValueMember = "物料编码";
            searchLookUpEdit2.Properties.PopupFormSize = new Size(1000, 450);

            comboBox1.Text = "全部";
        }

        private void fun_search()
        {
        DateTime t2= Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1);
          t2 = new DateTime(t2.Year,t2.Month,t2.Day,t2.Hour,t2.Minute,t2.Second );
            //          string sql = string.Format(@"select  * from (
            //select a.开票通知单号,a.采购单号,a.采购单明细号,a.入库单号,b.物料编码,b.规格型号,a.采购数量,入库数量,开票数量,折扣后不含税单价 as 不含税单价,
            //a.物料名称,a.税率,折扣后含税单价 as 含税单价,折扣后不含税金额 as 不含税金额,物料类型,a.发票确认日期,
            //折扣后含税金额 as 含税金额,大类,a.供应商名称,a.录入日期,c.备注2,a.供应商ID,单价备注 
            //  from   采购记录采购开票通知单明细表 a ,基础数据物料信息表 b,采购记录采购开票通知单主表 c ,采购记录采购单入库明细 d where a.开票通知单号 in (
            //select a.开票通知单号 from 采购记录采购开票通知发票核销表 a,采购记录采购开票通知单主表 b
            // where  a.开票通知单号=b.开票通知单号 and 录入日期 >'{0}' and 录入日期 <'{1}')
            //  and a.物料编码=b.物料编码 and a.开票通知单号=c.开票通知单号 and a.入库单号=d.入库单号  
            //  union 
            //  select a.开票通知单号,a.采购单号,a.采购单明细号,a.入库单号,b.物料编码,b.规格型号,a.采购数量,入库数量,开票数量,折扣后不含税单价 as 不含税单价,
            //a.物料名称,a.税率,折扣后含税单价 as 含税单价,折扣后不含税金额 as 不含税金额,物料类型,a.发票确认日期,
            //折扣后含税金额 as 含税金额,大类,a.供应商名称,a.录入日期,c.备注2,a.供应商ID,单价备注 
            //  from   采购记录采购开票通知单明细表 a ,基础数据物料信息表 b,采购记录采购开票通知单主表 c ,L采购记录采购单入库明细L  d where a.开票通知单号 in (
            //select a.开票通知单号 from 采购记录采购开票通知发票核销表 a,采购记录采购开票通知单主表 b
            // where  a.开票通知单号=b.开票通知单号 and 录入日期 >'{0}' and 录入日期 <'{1}')
            //  and a.物料编码=b.物料编码 and a.开票通知单号=c.开票通知单号 and a.入库单号=d.入库单号  )x where 1=1",
            //                    dateEdit1.EditValue,t2);
//            string sql = string.Format(@"select * from (select a.开票通知单号,a.采购单号,a.采购单明细号,a.入库单号,b.物料编码,b.规格型号,a.采购数量,入库数量,开票数量,折扣后不含税单价 as 不含税单价,
//a.物料名称,a.税率,折扣后含税单价 as 含税单价,折扣后不含税金额 as 不含税金额,物料类型,a.发票确认日期,
//折扣后含税金额 as 含税金额,大类,a.供应商名称,a.录入日期,c.备注2,a.供应商ID,单价备注 
//  from   采购记录采购开票通知单明细表 a ,基础数据物料信息表 b,采购记录采购开票通知单主表 c ,采购记录采购单入库明细 d
//   where a.开票通知单号 in (select b.开票通知单号 from 采购记录采购开票通知单主表 b
//  left join 采购记录采购开票通知发票核销表 a on a.开票通知单号=b.开票通知单号
// where  录入日期 >'{0}' and 录入日期 <'{1}')
//  and a.物料编码=b.物料编码 and a.开票通知单号=c.开票通知单号 and a.入库明细号 =d.入库明细号 )x where 1=1",
//                            dateEdit1.EditValue, t2);
//            string sql=string.Format(@"select *,round(采购不含税单价*开票数量,2) as 回冲金额 from(select a.开票通知单号, a.采购单号, a.采购单明细号, a.入库单号, b.物料编码, b.规格型号, a.采购数量, 入库数量, 开票数量, 折扣后不含税单价 as 不含税单价,
//a.物料名称, a.税率, 折扣后含税单价 as 含税单价, round(折扣后不含税金额,2) as 不含税金额, 物料类型, a.发票确认日期,case when LEFT(d.入库明细号,2)='DW' then CONVERT(decimal(18,6),d.备注6) else  mx.未税单价  end  as 采购不含税单价,
//round(折扣后含税金额,2) as 含税金额, 大类, a.供应商名称, a.录入日期, c.备注2, a.供应商ID, 单价备注,采购单类型,c.备注5
//  from 采购记录采购开票通知单明细表 a
//  left  join 基础数据物料信息表 b on a.物料编码 = b.物料编码
//left  join 采购记录采购开票通知单主表 c on a.开票通知单号 = c.开票通知单号
//left  join 采购记录采购单入库明细 d on a.入库明细号 = d.入库明细号
//  left join 采购记录采购单明细表 mx on mx.采购明细号 =d.采购单明细号 
//  left join 采购记录采购单主表 cz on cz.采购单号  =mx.采购单号 
//  where c.发票确认日期 > '{0}' and c.发票确认日期 < '{1}')x where 1 = 1", dateEdit1.EditValue, t2);

            string sql = string.Format(@"select *,round(采购不含税单价*开票数量,2) as 回冲金额 from(select a.开票通知单号, a.采购单号, a.采购单明细号, a.入库单号, b.物料编码, b.规格型号, a.采购数量, 入库数量, 开票数量, 折扣后不含税单价 as 不含税单价,
a.物料名称, a.税率, 折扣后含税单价 as 含税单价, round(折扣后不含税金额,2) as 不含税金额, 物料类型, a.发票确认日期,case when LEFT(d.入库明细号,2)='DW' then CONVERT(decimal(18,6),d.备注6) else  mx.未税单价  end  as 采购不含税单价,
round(折扣后含税金额,2) as 含税金额, 大类, a.供应商名称, a.录入日期, c.备注2, a.供应商ID, 单价备注,采购单类型,c.备注5,a.发票确认
  from 采购记录采购开票通知单明细表 a
  left  join 基础数据物料信息表 b on a.物料编码 = b.物料编码
left  join 采购记录采购开票通知单主表 c on a.开票通知单号 = c.开票通知单号
left  join 采购记录采购单入库明细 d on a.入库明细号 = d.入库明细号
  left join 采购记录采购单明细表 mx on mx.采购明细号 =d.采购单明细号 
  left join 采购记录采购单主表 cz on cz.采购单号  =mx.采购单号 
  )x where  1 = 1");

            if (comboBox1.Text == "已确认")
            {
                sql = sql + $" and  发票确认= 1 and  发票确认日期 > '{dateEdit1.EditValue}' and 发票确认日期 < '{t2}' ";
            }
            else if (comboBox1.Text == "未确认")
            {
                sql = sql +$" and  发票确认= 0 and 录入日期>'2019-5-1' and 录入日期>'{dateEdit1.EditValue}'  and 录入日期<'{t2}' ";
            }
            else
            {
                sql = sql + $@" and 录入日期>'2019-5-1' and  ((发票确认= 1 and  发票确认日期 > '{dateEdit1.EditValue}' and 发票确认日期 < '{t2}')
                   or (发票确认=0 and 录入日期>'{dateEdit1.EditValue}' and  录入日期<'{t2}')) ";
            }
            if (checkBox1.Checked == true)
            { 
                sql = sql + string.Format(" and  供应商ID='{0}'", searchLookUpEdit1.EditValue.ToString());

            }
            if (checkBox2.Checked == true)
            {
                sql = sql + string.Format(" and   开票通知单号='{0}'", textBox1.Text.ToString());
            }
            if (checkBox6.Checked == true)
            {
                sql = sql + string.Format(" and  采购单明细号 like '%{0}%' ", textBox2.Text);
            }
            //if (checkBox3.Checked == true)
            //{
            //    sql = sql + string.Format(" and 发票号='{0}'", textBox3.Text);
            //}
            if (checkBox4.Checked == true)
            {
                sql = sql + string.Format(" and  物料编码='{0}'", searchLookUpEdit2.EditValue.ToString());
            }

            sql = sql + " order by   开票通知单号 ";


            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            //decimal dec_开票数量 = 0;
            //decimal dec_含税金额 = 0;
            //decimal dec_不含税金额 = 0;


            //foreach (DataRow dr in dt.Rows)
            //{
            //    dec_开票数量 = dec_开票数量 + Convert.ToDecimal(dr["开票数量"]);
            //    dec_含税金额 = dec_含税金额 + Convert.ToDecimal(dr["含税金额"]);
            //    dec_不含税金额 = dec_不含税金额 + Convert.ToDecimal(dr["不含税金额"]);

            //}
            //DataRow r = dt.NewRow();
            //r["供应商名称"] = "总计";

            //r["开票数量"] = dec_开票数量;
            //r["含税金额"] = dec_含税金额;
            //r["不含税金额"] = dec_不含税金额;

            //dt.Rows.Add(r);
            gridControl1.DataSource = dt;

        }
        private void fun_check()
        {
            if (checkBox1.Checked == true)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择客户");
                }
            }
            if (checkBox2.Checked == true)
            {
                if (textBox1.Text == null || textBox1.Text.ToString() == "")
                {
                    throw new Exception("未填写开票号");
                }

            }
        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_check();
                fun_search();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
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

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
            
                string sql = @"select a.发票号,b.* from 采购记录采购开票通知发票核销表 a,采购记录采购开票通知单明细表 b
            where a.开票通知单号=b.开票通知单号 ";
                string j = "";
                if (checkBox3.Checked == true)
                {
                    if (textBox3.Text == "")
                    {
                        throw new Exception ("未输入发票号");
                    }
                    j = j + string.Format(" and a.发票号='{0}'", textBox3.Text);
                }
                if (checkBox5.Checked == true)
                {
                    if (textBox4.Text == "")
                    {
                        throw new Exception("未输入采购入库单号");
                    }
                    j = j + string.Format(" and  入库单号='{0}'", textBox4.Text);
                }
                if (checkBox3.Checked == false && checkBox5.Checked == false)
                {
                    throw new Exception("未输入搜索条件");

                }
                sql = sql + j;
                DataTable dt = new DataTable();
                dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("未找到记录");
                }
                else
                {
                    ERPpurchase.frm采购发票核销界面 frm = new frm采购发票核销界面(dt.Rows[0]["开票通知单号"].ToString());
                    CPublic.UIcontrol.Showpage(frm, "发票核销信息");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
          
        }

         
   

    }
}
