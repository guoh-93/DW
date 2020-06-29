using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ERPSale
{
    public partial class ui供货情况统计表 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        DataTable dtM;

        public ui供货情况统计表()
        {
            InitializeComponent();
        }

        private void fun_search()
        {
            dtM = new DataTable();


           // DateTime dtm1 = Convert.ToDateTime(dateEdit1.EditValue);
            DateTime dtm1 = dateTimePicker1.Value;
            //DateTime dtm2 = Convert.ToDateTime(dateEdit2.EditValue);

            DateTime dtm2 = dtm1.AddMonths(1).AddSeconds(-1);


          //  DateTime dtm3 = Convert.ToDateTime(dateEdit1.EditValue).AddDays(-Convert.ToDateTime(dateEdit1.EditValue).DayOfYear + 1);
            DateTime dtm3 = dtm1.AddDays(-dtm1.DayOfYear + 1);
            DateTime dtm4 = dtm3.AddYears(1);
            //(b.本年货款-d.本年累计到款) as 期末应收款
            string sql_全部 = string.Format(@"select '全部客户统计'as 客户单位名称 ,* from 
                (select sum(开票税后金额)本月货款  from 销售记录销售开票主表 where 生效=1 and 作废=0 and 开票日期>='{0}'  and 开票日期<'{1}')a,
                 (select sum(开票税后金额)本年货款  from 销售记录销售开票主表 where 生效=1 and 作废=0 and 开票日期>='{2}'  and 开票日期<'{3}')b,
                (select sum(金额)本月到款 from  客户付款记录表 where 付款日期 >='{0}' and 付款日期 <='{1}' )c,
                (select sum(金额)本年累计到款 from  客户付款记录表 where 付款日期 >='{2}' and 付款日期 <='{3}')d,
                 (select sum(期末余额)as 期末应收款 from 销售客户期初期末值 where 结转日期>'{0}' and  结转日期<'{1}')e                                                                                    "

                , dtm1, dtm2.AddDays(1).AddSeconds(-1), dtm3, dtm4);

            dtM = CZMaster.MasterSQL.Get_DataTable(sql_全部, strcon);

            string sql_1 = "select  ID,片区 from [销售片区年度指标对应表] ";
            DataTable dt_片区 = new DataTable();
            dt_片区 = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);

            foreach (DataRow dr in dt_片区.Rows)
            {
                //添加每个片区总金额记录
                //(b2.本年货款-d2.本年累计到款) as 期末应收款
                string str = "<" + dr["ID"].ToString() + ">." + dr["片区"].ToString();
                string sql_片区 = string.Format(@"select  '{0}'as 客户单位名称,* from 
                         (select sum(a.开票税后金额)本月货款  from
                     (select 销售记录销售开票主表.*,片区 from  销售记录销售开票主表 left  join 客户基础信息表 on 客户基础信息表.客户编号=销售记录销售开票主表.客户编号
                      where 生效=1 and 作废=0 and 开票日期>='{1}'  and 开票日期<'{2}'  and  片区='{3}')a)a2,
                      (select sum(b.开票税后金额)本年货款  from
                     (select 销售记录销售开票主表.*,片区 from  销售记录销售开票主表 left  join 客户基础信息表 on 客户基础信息表.客户编号=销售记录销售开票主表.客户编号
                      where 生效=1 and 作废=0 and 开票日期>='{4}'  and 开票日期<'{5}'  and  片区='{3}')b)b2,
                    (select sum(金额)本月到款 from  
                   (select 客户付款记录表.*,片区 from  客户付款记录表 left  join 客户基础信息表 on 客户基础信息表.客户编号=客户付款记录表.客户编号
                    where 付款日期 >='{1}' and 付款日期 <='{2}' and  片区='{3}')c)c2,
                    (select sum(金额)本年累计到款 from  
                     (select 客户付款记录表.*,片区 from  客户付款记录表 left  join 客户基础信息表 on 客户基础信息表.客户编号=客户付款记录表.客户编号
                     where 付款日期 >='{4}' and 付款日期 <='{5}' and  片区='{3}')d)d2, 
                    (select sum(期末余额)as 期末应收款 from 销售客户期初期末值,客户基础信息表 where 客户基础信息表.客户编号=销售客户期初期末值.客户编号 
                        and 结转日期>'{1}' and  结转日期<'{2}' and 片区='{3}') e2 ",
                        str, dtm1, dtm2.AddDays(1).AddSeconds(-1), dr["片区"].ToString(), dtm3, dtm4);
                using (SqlDataAdapter da = new SqlDataAdapter(sql_片区, strcon))
                {
                    da.Fill(dtM);
                }
             //加载所有该片区的客户
                string sql_客户 = string.Format("select  客户编号,客户名称  from 客户基础信息表 where 停用=0 and 片区='{0}'", dr["片区"].ToString());
                DataTable dt_客户 = CZMaster.MasterSQL.Get_DataTable(sql_客户, strcon);
                foreach (DataRow r in dt_客户.Rows)
                {



                    //按片区
//                    string sql = string.Format(@" select a.*,b.本年货款,c.本月到款,d.本年累计到款,(b.本年货款-d.本年累计到款)期末应收款 from
// 
//          (select 客户编号,客户名称,SUM(开票税后金额)本月货款 from 销售记录销售开票主表  where  生效=1 and 作废=0 and 开票日期>='{0}'  and 开票日期<'{1}'
//          group by  客户编号,客户名称) a,
//          (select 客户编号,客户名称,SUM(开票税后金额)本年货款  from 销售记录销售开票主表  where  生效=1 and 作废=0 and 开票日期>='{2}' and 开票日期<='{3}'
//         group by  客户编号,客户名称)b,
//         (select  客户编号,客户,sum(金额)本月到款 from  客户付款记录表 where 付款日期 >='{0}' and 付款日期 <='{1}' group by 客户编号,客户)c,
//         (select  客户编号,客户,sum(金额)本年累计到款 from  客户付款记录表 where 付款日期 >='{2}' and 付款日期 <='{3}' group by 客户编号,客户)d,客户基础信息表
//          where a.客户编号=b.客户编号 and a.客户编号=c.客户编号 and a.客户编号=d.客户编号 and  客户基础信息表.客户编号=a.客户编号 and a.客户编号='{4}' ",
//                                                                                                             dtm1, dtm2, dtm3, dtm4.AddDays(1).AddSeconds(-1), r_x["客户编号"].ToString());
                    //(b.本年货款-d.本年累计到款)期末应收款
                    string sql = string.Format(@" select a.*,b.本年货款,c.本月到款,d.本年累计到款,销售客户期初期末值.期末余额 as 期末应收款 from 客户基础信息表
        left join  (select 客户编号,客户名称,SUM(开票税后金额)本月货款 from 销售记录销售开票主表  where  生效=1 and 作废=0 and 开票日期>='{0}'  and 开票日期<'{1}'
          group by  客户编号,客户名称) a  on   客户基础信息表.客户编号=a.客户编号
        left join (select 客户编号,客户名称,SUM(开票税后金额)本年货款  from 销售记录销售开票主表  where  生效=1 and 作废=0 and 开票日期>='{2}' and 开票日期<='{3}'
         group by  客户编号,客户名称)b on  客户基础信息表.客户编号=b.客户编号
        left join (select  客户编号,客户,sum(金额)本月到款 from  客户付款记录表 where 付款日期 >='{0}' and 付款日期 <='{1}' group by 客户编号,客户)c
            on  客户基础信息表.客户编号=c.客户编号
        left  join  (select  客户编号,客户,sum(金额)本年累计到款 from  客户付款记录表 where 付款日期 >='{2}' and 付款日期 <='{3}' group by 客户编号,客户)d
          on   客户基础信息表.客户编号=d.客户编号  
        left  join  销售客户期初期末值 on 客户基础信息表.客户编号=销售客户期初期末值.客户编号 and 结转日期>'{0}' and 结转日期<'{1}'
        where  a.客户编号='{4}' or  c.客户编号='{4}' ",
                  dtm1, dtm2.AddDays(1).AddSeconds(-1), dtm3, dtm4, r["客户编号"].ToString());
                    
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                    {
                        //DataRow rr = dtM.NewRow();
                        //dtM.Rows.Add(rr);
                        //rr["客户单位名称"] = r_x["客户名称"];

                        da.Fill(dt);
                        if (dt.Rows.Count == 0)
                        {
                            //rr["本月货款"] = 0;
                            //rr["本年货款"] = 0;
                            //rr["本月到款"] = 0;
                            //rr["本年累计到款"] = 0;
                            //rr["期末应收款"] = 0;
                            continue;

                        }
                        else
                        {
                            DataRow rr = dtM.NewRow();
                            dtM.Rows.Add(rr);
                            rr["客户单位名称"] = r["客户名称"];
                            rr["本月货款"] = dt.Rows[0]["本月货款"];
                            rr["本年货款"] = dt.Rows[0]["本年货款"];
                            if (rr["本年货款"].ToString()=="")
                            {
                                rr["本年货款"] = 0;
                            }
                            rr["本月到款"] = dt.Rows[0]["本月到款"];
                            if (rr["本月到款"].ToString() == "")
                            {
                                rr["本月到款"] = 0;
                            }
                            rr["本年累计到款"] = dt.Rows[0]["本年累计到款"];
                            if (rr["本年累计到款"].ToString()=="")
                            {
                                rr["本年累计到款"] = 0;
                            }
                            rr["期末应收款"] = dt.Rows[0]["期末应收款"];
                            if (rr["期末应收款"].ToString() =="")
                            {
                                rr["期末应收款"] = Convert.ToDecimal(rr["本年货款"]) - Convert.ToDecimal(rr["本年累计到款"]);
                            }
                        }
                    }
                }
                gridControl1.DataSource = dtM;

            }

        }

        private void ui供货情况统计表_Load(object sender, EventArgs e)
        {
            DateTime FirstDay = System.DateTime.Today.AddDays(-Convert.ToDateTime(CPublic.Var.getDatetime().ToShortDateString()).Day + 1);
           // DateTime LastDay = System.DateTime.Today.AddMonths(1).AddDays(-CPublic.Var.getDatetime().AddMonths(1).Day + 1); 
            dateTimePicker1.Value = FirstDay;
          //  dateEdit2.EditValue = LastDay;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                fun_search();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
