using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
namespace ERPSale
{
    public partial class ui销售发票明细表 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        DataTable t_片区 = ERPorg.Corg.fun_业务员片区(CPublic.Var.localUserName);
        string cfgfilepath = "";
        public ui销售发票明细表()
        {
            InitializeComponent();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions( );

                gridControl1.ExportToXlsx(saveFileDialog.FileName,options );

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ui销售发票明细表_Load(object sender, EventArgs e)
        {


            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(this.panel2, this.Name, cfgfilepath);

            dateEdit1.EditValue = CPublic.Var.getDatetime().AddDays(-15).ToString("yyyy-MM-dd");
            dateEdit2.EditValue = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");
            fun_load();

            if (CPublic.Var.LocalUserTeam != "公司高管权限" && CPublic.Var.LocalUserTeam != "财务部权限"  && CPublic.Var.LocalUserTeam != "营销部权限" && CPublic.Var.LocalUserTeam != "ADMIN权限")
            {
                gridColumn3.Visible = false;
                gridColumn13.Visible = false;
                gridColumn14.Visible = false;
            }
        }

        private void fun_load()
        {

            string sql = string.Format(@"select 客户编号,客户名称 from 客户基础信息表 where 停用=0");
            if (t_片区.Rows.Count > 0)
            {
                string sx = " and  片区 in (";
                foreach (DataRow r in t_片区.Rows)
                {
                    sx = sx + string.Format("'{0}',", r["片区"]);
                }
                sx = sx.Substring(0, sx.Length - 1) + ")";
                sql = sql + sx;
            }
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            DataTable dt_客户 = new DataTable();
            da.Fill(dt_客户);
            searchLookUpEdit1.Properties.DataSource = dt_客户;
            searchLookUpEdit1.Properties.DisplayMember = "客户名称";
            searchLookUpEdit1.Properties.ValueMember = "客户编号";

            string sql_1 = string.Format(@"select 物料编码,规格型号,物料名称,大类,小类 from 基础数据物料信息表 where 停用=0");
            SqlDataAdapter da_1 = new SqlDataAdapter(sql_1, strcon);
            DataTable dt_物料 = new DataTable();
            da_1.Fill(dt_物料);
            searchLookUpEdit2.Properties.DataSource = dt_物料;
            searchLookUpEdit2.Properties.DisplayMember = "物料编码";
            searchLookUpEdit2.Properties.ValueMember = "物料编码";
      
        }
        private void fun_search()
        {

            DateTime t1 = Convert.ToDateTime(dateEdit1.EditValue);
            t1 = new DateTime(t1.Year, t1.Month, t1.Day);
            DateTime t2 = Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1);
            t2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);

            ///19-10-10    round(skpmx.本币税后金额/(1+skxzb.税率/100.00),4) as 本币不含税金额
            /////case when skxzb.币种='人民币' then round(skpmx.开票税前金额,2)  else 本币税后金额 end as 本币不含税金额  ,
            string sql = string.Format(@"select skxzb.销售开票通知单号,skxzb.币种,skxzb.汇率,skpmx.开票票号,成品出库单号,base.物料名称,skpmx.成品出库单明细号,skpmx.出库通知单明细号,base.物料编码 
  ,tzmx.客户,scmx.备注1,开票日期,base.规格型号,产品线,szb.备注10,base.物料类型,smx.销售订单号,大类,小类,对外产品线,对外大类,对外小类,开票数量
 ,skpmx.开票税前单价 ,round(skpmx.开票税前金额,2)开票税前金额,含税销售价,开票税后单价,round(skpmx.开票税后金额,2) as 开票金额,round(本币税后金额,2) as 本币金额,skxzb.税率,szb.销售部门, 
 case when skxzb.币种='人民币' then round(skpmx.开票税前金额,2)  else round(本币税后金额,2) end as 本币不含税金额  ,发出单价,round(发出单价*开票数量,2) as 发出金额,scmx.仓库号,scmx.仓库名称 from 销售记录销售开票明细表 skpmx
 left join  销售记录销售开票主表 skxzb on  skpmx.销售开票通知单号=skxzb.销售开票通知单号 
 left join 销售记录成品出库单明细表 scmx on skpmx.成品出库单明细号= scmx.成品出库单明细号
 left join 销售记录销售出库通知单明细表 tzmx on tzmx.出库通知单明细号=skpmx.出库通知单明细号 
 left join 客户基础信息表 kh on  skxzb.客户编号=kh.客户编号 
 left join 基础数据物料信息表 base on base.物料编码=skpmx.产品编码
 left join 销售记录销售订单明细表 smx on smx.销售订单明细号=tzmx.销售订单明细号 
 left join 销售记录销售订单主表 szb on szb.销售订单号=smx.销售订单号
 where   skpmx.生效=1 and  skxzb.开票日期>='{0}' and skxzb.开票日期<='{1}' and  skxzb.作废=0", t1, t2);

//  //2019-2 弃用 不需要管  表里没有值
//            string sql_补 = string.Format(@"select  skpmx.开票票号,成品出库单号,scmx.物料名称,skpmx.成品出库单明细号,base.物料编码 ,scmx.客户,scmx.备注1,开票日期,base.规格型号,产品线
//,'' as 备注10 ,base.物料类型,smx.销售订单号,大类,小类,对外产品线,对外大类,对外小类,开票数量,含税销售价,开票税后单价,skpmx.开票税后金额 as 开票金额 ,scmx.仓库号,scmx.仓库名称 
//from 销售记录销售开票明细表 skpmx,[L销售记录成品出库单明细表L] scmx,销售记录销售开票主表 skxzb,客户基础信息表 kh,基础数据物料信息表 base,L销售记录销售订单明细表L smx
//   where [skpmx].成品出库单明细号= scmx.成品出库单明细号 and skxzb.客户编号=kh.客户编号  and base.物料编码=skpmx.产品编码
//   and smx.销售订单明细号=scmx.销售订单明细号 and smx.关闭=0 and smx.生效=1
//       and skpmx.开票票号=skxzb.开票票号  and skpmx.生效=1  and  skxzb.开票日期>='{0}' 
//    and skxzb.开票日期<='{1}'  and  skxzb.作废=0", t1, t2);
            if (checkBox1.Checked == true)
            {
                sql = sql + string.Format(" and scmx.客户编号='{0}'", searchLookUpEdit1.EditValue.ToString());
              //  sql_补 = sql_补 + string.Format(" and scmx.客户编号='{0}'", searchLookUpEdit1.EditValue.ToString());
            }
            if (checkBox2.Checked == true)
            {
                sql = sql + string.Format(" and skpmx.开票票号='{0}'", textBox1.Text.ToString());
               // sql_补 = sql_补 + string.Format(" and skpmx.开票票号='{0}'", textBox1.Text.ToString());

            }
            if (checkBox3.Checked == true)
            {
                sql = sql + string.Format(" and base.物料类型='{0}'", comboBox1.Text.ToString());
               // sql_补 = sql_补 + string.Format(" and base.物料类型='{0}'", comboBox1.Text.ToString());
            }
            if (checkBox4.Checked == true)
            {
                sql = sql + string.Format(" and base.物料编码='{0}'",searchLookUpEdit2.EditValue.ToString());
               // sql_补 = sql_补 + string.Format(" and base.物料编码='{0}'", searchLookUpEdit2.EditValue.ToString());

            }
            if (checkBox5.Checked == true)
            {
                if (comboBox2.Text == "销售部")
                {
                   // sql = sql + " and 销售记录销售订单主表.备注10=''";
                    sql = sql + " and  scmx.客户编号 not in (select  客户编号 from 销售记录销售订单主表 where 备注10<>''  group by 客户编号)";
                   // sql_补 = sql_补 + " and scmx.客户编号 not in (select  客户编号 from 销售记录销售订单主表 where 备注10<>''  group by 客户编号)";
                }

                    //L销售记录销售订单主表L 等表中存的为 系统切换时导入的数据 或者是 退货 退金额 补开的 记录  
                    //生产部门下的销售单 不会存放在里面
                else if (comboBox2.Text == "生产部")
                {
                    sql = sql + " and  scmx.客户编号  in (select  客户编号 from 销售记录销售订单主表 where 备注10<>''  group by 客户编号)";
                   // sql_补 = sql_补 + " and  scmx.客户编号 in (select  客户编号 from 销售记录销售订单主表 where 备注10<>''  group by 客户编号)";
                    //sql = sql + " and 销售记录销售订单主表.备注10<>''";
                  //  sql_补 = sql_补 + " and 1<>1";
                }
            }

            //if (t_片区.Rows.Count > 0)
            //{
            //    string sx = " and 片区 in (";
            //    foreach (DataRow r in t_片区.Rows)
            //    {
            //        sx = sx + string.Format("'{0}',", r["片区"]);
            //    }
            //    sx = sx.Substring(0, sx.Length - 1) + ")";
            //    sql = sql + sx;
            //    sql_补 = sql_补 + sx;

            //}
            sql = sql + " order by skpmx.开票票号 ";

           // sql_补 = sql_补 + " order by skpmx.开票票号 ";

            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            //using (SqlDataAdapter da = new SqlDataAdapter(sql_补, strcon))
            //{
            //    da.Fill(dt);
            //}
            //decimal dec_开票数量 = 0;
            //decimal dec_开票金额 = 0;


            //foreach (DataRow dr in dt.Rows)
            //{
            //    dec_开票数量 = dec_开票数量 + Convert.ToDecimal(dr["开票数量"]);
            //    dec_开票金额=dec_开票金额+Convert.ToDecimal(dr["开票金额"]);      
            //}
            //DataRow r = dt.NewRow();
            //r["开票数量"] = dec_开票数量;
            //r["开票金额"] = dec_开票金额;
            //r["开票票号"] = "总计：";
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
            if (checkBox3.Checked == true)
            {
                if (comboBox1.Text == null || comboBox1.Text.ToString() == "")
                {
                    throw new Exception("物料类型未选择");
                }

            }

            if (checkBox4.Checked == true)
            {
                if (searchLookUpEdit2.EditValue == null || searchLookUpEdit2.EditValue.ToString() == "")
                {
                    throw new Exception("未选择物料");
                }

            }
            if (checkBox5.Checked == true)
            {
                if (comboBox2.Text == null || comboBox2.Text.ToString() == "")
                {
                    throw new Exception("下单部门未选择");
                }

            }
            
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                ERPorg.Corg.FlushMemory();
                fun_check();
                fun_search();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }
    }
}
