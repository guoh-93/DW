using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ERPSale
{
    public partial class ui销售员业绩统计表 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        DataTable dt_片区;
        public ui销售员业绩统计表()
        {
            InitializeComponent();
        }

        private void fun_search_总()
        {
            this.ActiveControl = null;
         DateTime time1= Convert.ToDateTime(dateTimePicker1.Text);       //选中时间
         DateTime time2 = Convert.ToDateTime(dateTimePicker1.Text).AddMonths(1); // 选中日期下一个月
         DateTime time3 = new DateTime(Convert.ToDateTime(dateTimePicker1.Text).Year,1,1);       //选中日期的年初
         DateTime time4 = Convert.ToDateTime(dateTimePicker1.Text).AddYears(-1);    //去年同月
         DateTime time5 = new DateTime(Convert.ToDateTime(dateTimePicker1.Text).AddYears(-1).Year, 1, 1);   //去年年初
         DateTime today = CPublic.Var.getDatetime();
        // 17年 同期对比取导进去表的值
//         string sql_全 = string.Format(@"SELECT [销售片区年度指标对应表].*,a.金额,金额/年度销售指标/10000 as 完成计划 ,年金额,年金额/年度销售指标/10000 as 年完成计划
//                ,去年月金额 , (金额-去年月金额) /年度销售指标/10000 as 去年同月,去年同期金额,(年金额-去年同期金额)/年度销售指标/10000 as 去年同期,月回笼金额,月回笼金额/金额 as 月回收率
//                ,年回笼金额,年回笼金额/年金额 as 年回收率,年回笼金额/年度收款指标/10000 as 年回笼完成计划,去年月回笼金额,(月回笼金额-去年月回笼金额)/去年月回笼金额 as 去年同月回笼
//                ,去年同期回笼金额,(年回笼金额-去年同期回笼金额)/去年同期回笼金额  as  去年同期回笼
//                FROM [FMS].[dbo].[销售片区年度指标对应表]
//                left join  ( select  片区,sum(开票税后金额)金额   from 销售记录销售开票主表 
//                left  join  客户基础信息表 on  客户基础信息表.客户编号 =销售记录销售开票主表.客户编号
//                and 生效=1 and 作废=0  and  开票日期>'{0}' and 开票日期<'{1}' group by 片区)a
//                 on  a.片区=[销售片区年度指标对应表].片区
//                 left join  ( select  片区,sum(开票税后金额)年金额   from 销售记录销售开票主表 
//                left  join  客户基础信息表 on  客户基础信息表.客户编号 =销售记录销售开票主表.客户编号
//                and 生效=1 and 作废=0  and  开票日期>'{2}' and 开票日期<'{1}' group by 片区)b
//                on  b.片区=[销售片区年度指标对应表].片区
//                left join  ( select  片区,sum(开票税后金额)去年月金额   from 销售记录销售开票主表 
//                left  join  客户基础信息表 on  客户基础信息表.客户编号 =销售记录销售开票主表.客户编号
//                and 生效=1 and 作废=0  and  开票日期>'{3}' and 开票日期<'{4}' group by 片区)c
//                 on  c.片区=[销售片区年度指标对应表].片区
//                left join  ( select  片区,sum(开票税后金额)去年同期金额   from 销售记录销售开票主表 
//                left  join  客户基础信息表 on  客户基础信息表.客户编号 =销售记录销售开票主表.客户编号
//                and 生效=1 and 作废=0  and  开票日期>'{5}' and 开票日期<'{4}' group by 片区)d
//                on  d.片区=[销售片区年度指标对应表].片区 
//                  left join   ( select  片区,sum(金额)月回笼金额   from 客户付款记录表 
//left  join  客户基础信息表 on  客户基础信息表.客户编号 =客户付款记录表.客户编号
//  and  付款日期 >'{0}' and 付款日期<'{1}' group by 片区)aa
//  on  aa.片区=[销售片区年度指标对应表].片区                  
//  left join   ( select  片区,sum(金额)年回笼金额   from 客户付款记录表 
//left  join  客户基础信息表 on  客户基础信息表.客户编号 =客户付款记录表.客户编号
//  and  付款日期 >'{2}' and 付款日期<'{1}' group by 片区)bb
//  on  bb.片区=[销售片区年度指标对应表].片区 
//    left join   ( select  片区,sum(金额)去年月回笼金额   from 客户付款记录表 
//left  join  客户基础信息表 on  客户基础信息表.客户编号 =客户付款记录表.客户编号
//  and  付款日期 >'{3}' and 付款日期<'{4}' group by 片区)cc
//  on  cc.片区=[销售片区年度指标对应表].片区 
//    left join   ( select  片区,sum(金额)去年同期回笼金额   from 客户付款记录表 
//left  join  客户基础信息表 on  客户基础信息表.客户编号 =客户付款记录表.客户编号
//  and  付款日期 >'{5}' and 付款日期<'{4}' group by 片区)dd
//  on  dd.片区=[销售片区年度指标对应表].片区
//", time1, time2, time3, time4, time4.AddMonths(1), time5);    


         string sql_全 = string.Format(@"SELECT [销售片区年度指标对应表].*,a.金额,金额/(年度销售指标*10000.00000) as 完成计划 ,年金额,年金额/(年度销售指标*10000.000000) as 年完成计划
                ,去年月金额 , (金额-去年月金额) /(年度销售指标*10000.00000) as 去年同月,去年同期金额
                ,case when 去年同期金额=0 then null else(年金额-去年同期金额)/(年度销售指标*10000.000000) end as 去年同期,月回笼金额
                ,case when 金额=0 then null else 月回笼金额/金额 end as 月回收率
                ,年回笼金额,case when 年金额=0 then null else 年回笼金额/年金额 end as 年回收率,
               年回笼金额/(年度收款指标*10000.000000) as 年回笼完成计划,去年月回笼金额,
                case when 去年月回笼金额=0 then null else(月回笼金额-去年月回笼金额)/去年月回笼金额 end as 去年同月回笼
                ,去年同期回笼金额,case when 去年同期回笼金额=0 then null else (年回笼金额-去年同期回笼金额)/去年同期回笼金额  end as  去年同期回笼
                FROM  [销售片区年度指标对应表]
                left join  ( select  片区,isnull(sum(开票税后金额),0)金额   from 销售记录销售开票主表 
                left  join  客户基础信息表 on  客户基础信息表.客户编号 =销售记录销售开票主表.客户编号
                where 生效=1 and 作废=0  and  开票日期>'{0}' and 开票日期<'{1}' and 销售记录销售开票主表.客户编号 
                 not in (select  客户编号 from 销售记录销售订单主表 where 备注10<>'' group by 客户编号) group by 片区)a
                 on  a.片区=[销售片区年度指标对应表].片区

                 left join  ( select  片区,isnull(sum(开票税后金额),0)年金额   from 销售记录销售开票主表 
                left  join  客户基础信息表 on  客户基础信息表.客户编号 =销售记录销售开票主表.客户编号
                where  生效=1 and 作废=0  and  开票日期>'{2}' and 开票日期<'{1}' and 销售记录销售开票主表.客户编号 
                 not in (select  客户编号 from 销售记录销售订单主表 where 备注10<>'' group by 客户编号)  group by 片区)b
                on  b.片区=[销售片区年度指标对应表].片区 
                left join  ( select   isnull(sum(开票金额),0)去年月金额,片区   from 销售客户期初期末值 a
                left  join  客户基础信息表  b on  a.客户编号=b.客户编号 where  年={7}-1 and 月={6}  group  by 片区)c   
                 on  c.片区=[销售片区年度指标对应表].片区
                left join  ( select   isnull(sum(开票金额),0)去年同期金额,片区   from 销售客户期初期末值 a
                left  join  客户基础信息表  b on  a.客户编号=b.客户编号 where  年={7}-1 and 月<={6}  group  by 片区)d
                on  d.片区=[销售片区年度指标对应表].片区 
                  left join   ( select  片区,isnull(sum(金额),0)月回笼金额   from 客户付款记录表 
left  join  客户基础信息表 on  客户基础信息表.客户编号 =客户付款记录表.客户编号 
  where  付款日期 >'{0}' and 付款日期<'{1}' and 客户付款记录表.客户编号 
                 not in (select  客户编号 from 销售记录销售订单主表 where 备注10<>'' group by 客户编号)  group by 片区)aa
  on  aa.片区=[销售片区年度指标对应表].片区                  
  left join   ( select  片区,isnull(sum(金额),0)年回笼金额   from 客户付款记录表 
left  join  客户基础信息表 on  客户基础信息表.客户编号 =客户付款记录表.客户编号
 where  付款日期 >'{2}' and 付款日期<'{1}' and 客户付款记录表.客户编号 
                 not in (select  客户编号 from 销售记录销售订单主表 where 备注10<>'' group by 客户编号)  group by 片区)bb
  on  bb.片区=[销售片区年度指标对应表].片区 
    left join   ( select    片区,isnull(sum(收款金额),0)去年月回笼金额  from 销售客户期初期末值 a
                left  join  客户基础信息表  b on  a.客户编号=b.客户编号 where  年={7}-1 and 月={6}  group  by 片区)cc
  on  cc.片区=[销售片区年度指标对应表].片区 
    left join   (select    片区,isnull(sum(收款金额),0)去年同期回笼金额  from 销售客户期初期末值 a
                left  join  客户基础信息表  b on  a.客户编号=b.客户编号 where  年={7}-1 and 月<={6}  group  by 片区)dd
     on  dd.片区=[销售片区年度指标对应表].片区  where  [销售片区年度指标对应表].年份={7}  "
             , time1, time2, time3, time4, time4.AddMonths(1), time5, time1.Month, today.Year);

            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql_全, strcon);
            gridControl1.DataSource = dt;

            gridControl2.DataSource = dt;
            decimal dec_销售指标 = 0;
            decimal dec_收款指标 = 0;
            decimal dec_金额 = 0;
            decimal dec_完成计划 = 0;
            decimal dec_年金额 = 0;
            decimal dec_年完成计划 = 0;
            decimal dec_去年月金额 = 0;
            decimal dec_去年同月 = 0;
            decimal dec_去年同期 = 0;
            decimal dec_去年同期金额 = 0;
            decimal dec_月回笼金额 = 0;
            decimal dec_月回收率 = 0;
            decimal dec_年回笼金额 = 0;
            decimal dec_年回收率 = 0;
            decimal dec_年回笼完成计划 = 0;
            decimal dec_去年月回笼金额 = 0;
            decimal dec_去年同期回笼金额 = 0;
            decimal dec_去年同期回笼 = 0;

            foreach (DataRow r in dt.Rows)
            {
                if (r["金额"]==DBNull.Value)
                {
                    r["金额"] = 0;
                }
                if (r["完成计划"] == DBNull.Value)
                {
                    r["完成计划"] = 0;
                }

                if (r["年金额"] == DBNull.Value)
                {
                    r["年金额"] = 0;
                }
                if (r["年完成计划"] == DBNull.Value)
                {
                    r["年完成计划"] = 0;
                }
                if (r["去年月金额"] == DBNull.Value)
                {
                    r["去年月金额"] = 0;
                }
                if (r["去年同月"] == DBNull.Value)
                {
                    r["去年同月"] = 0;
                }
                if (r["去年同期"] == DBNull.Value)
                {
                    r["去年同期"] = 0;
                }
                if (r["去年同期金额"] == DBNull.Value)
                {
                    r["去年同期金额"] = 0;
                }
                if (r["月回笼金额"] == DBNull.Value)
                {
                    r["月回笼金额"] = 0;
                }
                if (r["月回收率"] == DBNull.Value)
                {
                    r["月回收率"] = 0;
                }
                if (r["年回笼金额"] == DBNull.Value)
                {
                    r["年回笼金额"] = 0;
                }
                if (r["年回笼完成计划"] == DBNull.Value)
                {
                    r["年回笼完成计划"] = 0;
                }
                if (r["去年同月回笼"] == DBNull.Value)
                {
                    r["去年同月回笼"] = 0;
                }
                if (r["去年月回笼金额"] == DBNull.Value)
                {
                    r["去年月回笼金额"] = 0;
                }
                if (r["去年同期回笼金额"] == DBNull.Value)
                {
                    r["去年同期回笼金额"] = 0;
                }
                if (r["去年同期回笼"] == DBNull.Value)
                {
                    r["去年同期回笼"] = 0;
                }
                if (r["年回收率"] == DBNull.Value)
                {
                    r["年回收率"] = 0;
                }

                dec_销售指标 = dec_销售指标 + Convert.ToDecimal(r["年度销售指标"]);
                dec_收款指标 = dec_收款指标 + Convert.ToDecimal(r["年度收款指标"]);
                dec_金额 = dec_金额 + Convert.ToDecimal(r["金额"]);
                dec_完成计划 = dec_完成计划 + Convert.ToDecimal(r["完成计划"]);
                dec_年金额 = dec_年金额 + Convert.ToDecimal(r["年金额"]);
                dec_年完成计划 = dec_年完成计划 + Convert.ToDecimal(r["年完成计划"]);
                dec_去年月金额 = dec_去年月金额 + Convert.ToDecimal(r["去年月金额"]);
                dec_去年同月 = dec_去年同月 + Convert.ToDecimal(r["去年同月"]);
                dec_去年同期 = dec_去年同期 + Convert.ToDecimal(r["去年同期"]);
                dec_去年同期金额 = dec_去年同期金额 + Convert.ToDecimal(r["去年同期金额"]);
                dec_月回笼金额 = dec_月回笼金额 + Convert.ToDecimal(r["月回笼金额"]);
                dec_月回收率 = dec_月回收率 + Convert.ToDecimal(r["月回收率"]);
                dec_年回笼金额 = dec_年回笼金额 + Convert.ToDecimal(r["年回笼金额"]);
                dec_年回收率 = dec_年回收率 + Convert.ToDecimal(r["年回收率"]);
                dec_年回笼完成计划 = dec_年回笼完成计划 + Convert.ToDecimal(r["年回笼完成计划"]);
                dec_去年月回笼金额 = dec_去年月回笼金额 + Convert.ToDecimal(r["去年月回笼金额"]);
                dec_去年同期回笼金额 = dec_去年同期回笼金额 + Convert.ToDecimal(r["去年同期回笼金额"]);
                dec_去年同期回笼 = dec_去年同期回笼 + Convert.ToDecimal(r["去年同期回笼"]);
            }
            DataRow rr = dt.NewRow();

            rr["片区"] = "总计";
            rr["年度销售指标"]= dec_销售指标;
            rr["年度收款指标"]=dec_收款指标 ;
             rr["金额"]= dec_金额 ;
             rr["完成计划"] = dec_金额 /dec_销售指标/10000;
             rr["年金额"]=dec_年金额;
             rr["年完成计划"] = dec_年金额 / dec_销售指标/10000;
             rr["去年月金额"]=dec_去年月金额;
             rr["去年同月"] = (dec_金额 - dec_去年月金额) / dec_销售指标 / 10000;
             rr["去年同期"] = (dec_年金额 - dec_去年同期金额)/ dec_销售指标 / 10000;
            rr["去年同期金额"]=dec_去年同期金额;
            rr["月回笼金额"]=dec_月回笼金额 ;
            if (dec_金额 == 0)
            {
                rr["月回收率"] = 0;
            }
            else
            {
                rr["月回收率"] = dec_月回笼金额 / dec_金额;
            }
            rr["年回笼金额"]=dec_年回笼金额;
            rr["年回收率"]=dec_年回收率 ;
            rr["年回笼完成计划"] = dec_年回笼金额/dec_收款指标/10000;

            rr["去年月回笼金额"]=dec_去年月回笼金额 ;
            if (dec_去年月回笼金额 == 0)
            {
                rr["去年同月回笼"] = 0;
            }
            else
            {
                rr["去年同月回笼"] = (dec_月回笼金额 - dec_去年月回笼金额) / dec_去年月回笼金额;
            }
            rr["去年同期回笼金额"]=dec_去年同期回笼金额;
            if (dec_去年同期回笼金额 == 0)
            {
                rr["去年同期回笼"] =0;

            }
            else
            {
                rr["去年同期回笼"] = (dec_年回笼金额 - dec_去年同期回笼金额) / dec_去年同期回笼金额;

            }
            dt.Rows.Add(rr);
        }
        private void fun_check()
        {
            string s = string.Format("select  * from  销售片区年度指标对应表 where 年份={0}", Convert.ToDateTime(dateTimePicker1.Text).Year);
            DataTable dt=CZMaster.MasterSQL.Get_DataTable(s,strcon);
            if(dt.Rows.Count==0)
            {
                throw new Exception(Convert.ToDateTime(dateTimePicker1.Text).Year+"年片区指标尚未维护");
            }
        }
     
        private void ui销售员业绩统计表_Load(object sender, EventArgs e)
        {
            //fun_search();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_check();
                fun_search_总();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

           
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                if (tabControl1.SelectedTab.Name == "tabPage1")
                {
                    gridControl1.ExportToXlsx(saveFileDialog.FileName);
                }
                else
                {
                    gridControl2.ExportToXlsx(saveFileDialog.FileName);
                }
                

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

      
    }
}
