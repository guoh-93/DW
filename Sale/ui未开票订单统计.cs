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
    public partial class ui未开票订单统计 : UserControl
    {
        string strcon=CPublic.Var.strConn;
        //DataTable t_片区 = ERPorg.Corg.fun_业务员片区(CPublic.Var.localUserName);
        DataTable dtm;
         string cfgfilepath;

        public ui未开票订单统计()
        {
            InitializeComponent();
        }

        private void ui未开票订单统计_Load(object sender, EventArgs e)
        {
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";

            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(panel2, this.Name, cfgfilepath);
            fun_load();

                  if (CPublic.Var.LocalUserTeam != "公司高管权限" && CPublic.Var.LocalUserTeam != "营销部权限" 
                         && CPublic.Var.LocalUserTeam != "管理员权限" && CPublic.Var.LocalUserTeam != "财务部权限")
                  {
                      gridColumn9.Visible = false;
                      //gridColumn11.Visible = false;
 
                  }

        }

        private void fun_load()
        {
            string sql = string.Format(@"select 客户编号,客户名称 from 客户基础信息表  where 停用=0  ");

            
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            DataTable dt_客户 = new DataTable();
            da.Fill(dt_客户);
            searchLookUpEdit1.Properties.DataSource = dt_客户;
            searchLookUpEdit1.Properties.DisplayMember = "客户名称";
            searchLookUpEdit1.Properties.ValueMember = "客户编号";


           
        }
        private void fun_search()
        {
            /*19-11-06 保持跟 待开票列表一致
        //   string sql = string.Format(@"select  scmx.*,税后金额,scmx.销售备注,片区,(发出单价*未开票数量)as 金额
        //        ,产品线,scmx.生效日期 as 出库日期   FROM  销售记录成品出库单明细表 scmx
        //left join 销售记录销售订单明细表 smx on smx.销售订单明细号=scmx.销售订单明细号
        //left join 客户基础信息表 kh on kh.客户编号= scmx.客户编号
        //left join 销售记录销售出库通知单明细表 sctmx on sctmx.出库通知单明细号=scmx.出库通知单明细号 
        //left  join  基础数据物料信息表 on 基础数据物料信息表.物料编码=scmx.物料编码 
        //where scmx.作废=0 and scmx.生效=1 and 未开票数量<>0");

        //    string sql_补 = string.Format(@"select  scmx.*,税后单价,税后金额,scmx.销售备注,片区,(税后单价*未开票数量)as 未开票金额
        //        ,产品线,scmx.生效日期 as 出库日期 FROM  L销售记录成品出库单明细表L scmx
        //left join L销售记录销售订单明细表L smx on smx.销售订单明细号=scmx.销售订单明细号
        //left join 客户基础信息表 kh on kh.客户编号= scmx.客户编号
        //left join 销售记录销售出库通知单明细表 sctmx on sctmx.出库通知单明细号=scmx.出库通知单明细号 
        //left  join  基础数据物料信息表 on 基础数据物料信息表.物料编码=scmx.物料编码 
        //where scmx.作废=0 and scmx.生效=1  and 未开票数量<>0");
        */

            string sql = @" select yy.*,isnull(开票数量, 0)已开未审 from(select xx.*from(
            select 已出库数量 - 已开票数量 - 累计退货数量 as 实际未开票数量,币种, scmx.*, smx.销售订单号 as 订单号, sz.部门编号, 销售部门, sz.目标客户, sz.备注1 as 表头备注, 税前单价, (税前单价 * 出库数量)税前金额, 税后单价, (税后单价 * 出库数量)税后金额, sz.客户订单号
                  from 销售记录成品出库单明细表 scmx
                  left join 销售记录销售订单明细表 smx on scmx.销售订单明细号 = smx.销售订单明细号 
                  left join  销售记录销售订单主表 sz on     smx.销售订单号 = sz.销售订单号
                 where   scmx.生效 = 1 and(已出库数量 - 已开票数量 - 累计退货数量 > 0  or(scmx.备注1 <> '' and 已出库数量 - 已开票数量 - 累计退货数量   < 0)) and scmx.作废 = 0
                 )xx union
                  select  出库数量 - 累计开票数量 as 实际未开票数量,''币种,tzmx.ID,tzmx.GUID,''成品出库单号,''pos,'' 成品出库单明细号 ,szb.销售订单号,smx.销售订单明细号,出库通知单明细号,出库通知单号,tzmx.物料编码,tzmx.物料名称,'' BOM版本,0 as 数量 ,出库数量,出库数量 as 已出库数量
                    ,累计开票数量 as 已开票数量, 出库数量 - 累计开票数量 as 未开票数量,0 数量,tzmx.计量单位,tzmx.规格型号,''图纸编号,tzmx.客户,tzmx.客户编号,smx.仓库号,smx.仓库名称,tzmx.生效,tzmx.生效日期,tzmx.作废,tzmx.作废时间 as 作废日期,
                    作废人,tzmx.完成,tzmx.完成日期,备注1,备注2,smx.备注3,smx.备注4,smx.备注5,smx.备注6,smx.备注7,smx.备注8,smx.备注9,smx.备注10,tzmx.特殊备注,'' 送货方式,tzmx.销售备注,0 as 累计退货数量,'' 退货标识,0 发出单价,szb.销售订单号,部门编号,销售部门,目标客户,
                    '' 表头备注,税前单价,smx.税前金额,税后单价,smx.税后金额,客户订单号 from 销售记录销售出库通知单明细表 tzmx
                  left join  销售记录销售订单明细表 smx  on smx.销售订单明细号 = tzmx.销售订单明细号
                  left join 销售记录销售订单主表 szb on szb.销售订单号 = smx.销售订单号
                  where left(smx.物料编码,3)= '200' and 出库数量 > 累计开票数量 and tzmx.生效日期 < '2019-7-30 18:00:00' )yy
                 left join(select  成品出库单明细号, 出库通知单明细号, sum(开票数量)开票数量 from 销售记录销售开票明细表 k
                 left  join 销售记录销售开票主表 z on z.销售开票通知单号 = k.销售开票通知单号
                 where k.生效 = 0  and 创建日期 > '2019-5-1'  group by  成品出库单明细号, 出库通知单明细号, k.产品编码)d
                       on d.成品出库单明细号 + d.出库通知单明细号 = yy.成品出库单明细号 + yy.出库通知单明细号
                   where   abs(已出库数量)-累计退货数量 - abs(isnull(开票数量, 0)) > 0 and 作废=0";

            ///20-1-14
            /// abs(未开票数量)-累计退货数量 - abs(isnull(开票数量, 0))



            //if (checkBox1.Checked == true)
            //{
            //    if (comboBox1.Text.ToString() == "退货")
            //    {
            //        sql = sql + string.Format(" and 备注1='{0}'", comboBox1.Text.ToString());
            //       // sql_补 = sql_补 + string.Format(" and 备注1='{0}'", comboBox1.Text.ToString());

            //    }
            //    else
            //    {
            //        sql = sql + string.Format(" and 备注1=''");
            //       // sql_补 = sql_补 + string.Format(" and 备注1=''");

            //    }
            //}
            if (checkBox3.Checked == true)
            {
                sql = sql + string.Format("  and 客户编号='{0}'", searchLookUpEdit1.EditValue.ToString());
               // sql_补 = sql_补 + string.Format("  and scmx.客户编号='{0}'", searchLookUpEdit1.EditValue.ToString());

            }
            if (checkBox2.Checked == true)
            {
                sql = sql + string.Format("and  销售订单明细号 like '%{0}%'", textBox1.Text);
                //sql_补 = sql_补 + string.Format("and  scmx.销售订单明细号 like '%{0}%'", textBox1.Text);

            }
            //if (checkBox4.Checked == true)
            //{
            //    DateTime t1 = Convert.ToDateTime(dateEdit1.EditValue);
            //    t1 = new DateTime(t1.Year, t1.Month, t1.Day);
            //    DateTime t2 = Convert.ToDateTime(dateEdit2.EditValue).AddDays(1);
            //    t2 = new DateTime(t2.Year, t2.Month, t2.Day);

            //    //sql = sql + string.Format(" and   CONVERT(datetime,SUBSTRING(出库通知单号,5,6),112)>='{0}' and   CONVERT(datetime,SUBSTRING(出库通知单号,5,6),112)<'{1}'",t1, t2);
            //    //sql_补 = sql_补 + string.Format(" and   CONVERT(datetime,SUBSTRING(出库通知单号,5,6),112)>='{0}' and   CONVERT(datetime,SUBSTRING(出库通知单号,5,6),112)<'{1}'",t1,t2);
            //    sql = sql + string.Format(" and  sctmx.生效日期>='{0}' and  sctmx.生效日期<'{1}'", t1, t2);
            //   // sql_补 = sql_补 + string.Format(" and   sctmx.生效日期>='{0}' and   sctmx.生效日期<'{1}'", t1, t2);
            //}
            //if (checkBox5.Checked == true)
            //{
            //    sql = sql + string.Format(" and 片区= '{0}' ", searchLookUpEdit2.EditValue.ToString());
            //   // sql_补 = sql_补 + string.Format(" and 片区= '{0}' ", searchLookUpEdit2.EditValue.ToString());

            //}
            //else
            //{
            //    if (t_片区.Rows.Count > 0)
            //    {

            //        string sx = " and 片区 in (";
            //        foreach (DataRow r in t_片区.Rows)
            //        {
            //            sx = sx + string.Format("'{0}',", r["片区"]);
            //        }
            //        sx = sx.Substring(0, sx.Length - 1) + ")";
            //        sql = sql + sx;
            //        //sql_补 = sql_补 + sx;

            //    }

            //}
            if (checkBox6.Checked == true)
            {
                DateTime t1 = Convert.ToDateTime(dateEdit3.EditValue);
                t1 = new DateTime(t1.Year, t1.Month, t1.Day);
                DateTime t2 = Convert.ToDateTime(dateEdit4.EditValue).AddDays(1).AddSeconds(-1);
                t2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);
                sql = sql + string.Format(" and  生效日期>='{0}' and   生效日期<='{1}' ", t1, t2);
               // sql_补 = sql_补 + string.Format(" and scmx.生效日期>='{0}' and scmx.生效日期<='{1}' ", t1, t2);

            }
            dtm = new DataTable();
            dtm = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            //using (SqlDataAdapter da = new SqlDataAdapter(sql_补, strcon))
            //{
            //    da.Fill(dtm);
            //}
            
            gridControl1.DataSource = dtm;

            //if (checkBox1.Checked != true)
            //{
            //    foreach (DataRow dr in dtm.Rows)
            //    {
            //        if (dr["备注1"].ToString() == "") dr["备注1"] = "销售送货";
            //    }
            //}
        }
        private void fun_check()
        {
            
            if (checkBox2.Checked == true)
            {
                if (textBox1.Text == null || textBox1.Text.ToString() == "")
                {
                    throw new Exception("未填写订单号");
                }

            }
            if (checkBox3.Checked == true)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择客户");
                }
            }
            //if (checkBox4.Checked == true)
            //{
            //    if (dateEdit1.EditValue == null || dateEdit2.EditValue == null || dateEdit1.EditValue.ToString() == "" || dateEdit2.EditValue.ToString() == "")
            //    {
            //        throw new Exception("未选择通知日期");
            //    }

            //}
            //if (checkBox5.Checked == true)
            //{
            //    if (searchLookUpEdit2.EditValue.ToString() == "")
            //    {
            //        throw new Exception("未选择销售组");
            //    }

            //}
            if (checkBox6.Checked == true)
            {
                if (dateEdit3.EditValue == null || dateEdit4.EditValue == null || dateEdit3.EditValue.ToString() == "" || dateEdit4.EditValue.ToString() == "")
                {
                    throw new Exception("未选择出库日期");
                }

            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

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

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (dtm != null && dtm.Columns.Count > 0 && dtm.Rows.Count > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    //  DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                    gridControl1.ExportToXlsx(saveFileDialog.FileName);
                    MessageBox.Show("导出成功");
             
                }
            }
            else
            {
                MessageBox.Show("无记录可导出");
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

       
    }
}
