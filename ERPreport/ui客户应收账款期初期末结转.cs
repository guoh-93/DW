using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPreport
{
    public partial class ui客户应收账款期初期末结转 : UserControl
    {

        #region
        string strcon = CPublic.Var.strConn;
        DataTable dtm = new DataTable();
        /// <summary>
        /// 指示是否要删除当前月进行重新计算
        /// </summary>
        bool flag = false;


        #endregion
        public ui客户应收账款期初期末结转()
        {
            InitializeComponent();
        }
        public ui客户应收账款期初期末结转(bool bl_View)
        {
            InitializeComponent();
            barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barStaticItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            barEditItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always; 

        }

        private void ui客户应收账款期初期末结转_Load(object sender, EventArgs e)
        {
            if (CPublic.Var.localUser部门编号 != "00010401")
            {
 
                barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barStaticItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barEditItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always; 
            }

            DateTime t=CPublic.Var.getDatetime();
            t=new DateTime (t.Year,t.Month,1);
            barEditItem1.EditValue = t;

        }
        private void fun_load(int y ,int m)
        {
            string ss="";
            if (barEditItem2.EditValue != null && barEditItem2.EditValue.ToString() != "" )
            {
                if (barEditItem2.EditValue.ToString() == "销售部")
                {

                    //销售订单主表的 备注10  记录 是否是 计划下单
                    ss = " and 销售客户期初期末值.客户编号 not in (select  客户编号 from 销售记录销售订单主表   where 备注10<>'' group by 客户编号) ";
                }
                else
                {
                    ss = " and 销售客户期初期末值.客户编号  in (select  客户编号 from 销售记录销售订单主表   where 备注10<>'' group by 客户编号) ";

                }
            }
        
            string sql = string.Format(@"select 销售客户期初期末值.*,客户基础信息表.客户名称 as 最新名称,客户基础信息表.片区,开票总额,付款总额
                                        from 销售客户期初期末值   left  join 客户基础信息表 on 客户基础信息表.客户编号=销售客户期初期末值.客户编号
                        left  join (select SUM(开票金额)开票总额,SUM(收款金额)付款总额,客户编号 from [销售客户期初期末值] where 年={0} and 月<={1} and 客户编号<>'' group by 客户编号 )b
                          on b.客户编号=[销售客户期初期末值].客户编号   where 销售客户期初期末值.客户编号=客户基础信息表.客户编号 {2}  and 年={0} and 月={1}", y, m, ss);
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dt;

        }
        /// <summary>
        /// t1 上个月一号
        /// t2 所需结转月1号
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        private void fun_jz(DateTime t1,DateTime t2)
        {
            label2.Text ="准备数据中...";
            DateTime t_now = CPublic.Var.getDatetime();
            string sql = string.Format("select * from 销售客户期初期末值 where 月='{0}' and 年='{1}'",t1.Month,t1.Year);
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            dtm = dt.Clone();
            foreach (DataRow dr in dt.Rows)
            {
                DataRow r = dtm.NewRow();
                r["客户编号"] = dr["客户编号"];
                r["客户名称"] = dr["客户名称"];
                r["期初余额"] = dr["期末余额"];
                r["开票金额"] = 0;
                r["收款金额"] = 0;
                r["期末余额"] = 0;
                r["结转日期"] = t_now;
                r["年"] = t2.Year;
                r["月"] = t2.Month;

                dtm.Rows.Add(r);
            }
            string sql_开票汇总 =string.Format(@"select 客户编号,客户名称 from 客户基础信息表   where 客户编号 in (  
  select * from  (select 客户编号  from  销售记录销售开票主表  group by 客户编号 )a 
  where  客户编号 not in (select 客户编号 from [销售客户期初期末值] where 年={0} and  月={1})) ", t1.Year, t1.Month);  //and 客户编号<>'880117'除了迪飞达   其他的有没有规则是什么？ 待定 

            dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql_开票汇总, strcon);

            foreach (DataRow dr in dt.Rows)  //加入 原本期初期末中没有的客户（开票主表中）
            {
                DataRow r = dtm.NewRow();
                r["客户编号"] = dr["客户编号"];
                r["客户名称"] = dr["客户名称"];
                r["期初余额"] = 0;
                r["开票金额"] = 0;
                r["收款金额"] = 0;
                r["期末余额"] = 0;
                r["结转日期"] = t_now;
                r["年"] = t2.Year;
                r["月"] = t2.Month;
                dtm.Rows.Add(r);
            }

            string sql_付款 = string.Format(@"select 客户编号,客户名称 from 客户基础信息表   where 客户编号 in (  
  select * from  (select 客户编号  from  客户付款记录表  where 客户编号<>'' group by 客户编号 )a 
  where  客户编号 not in (select 客户编号 from [销售客户期初期末值] where 年={0} and 月={1} )
  and 客户编号 not in (select 客户编号  from  销售记录销售开票主表  group by 客户编号) )", t1.Year, t1.Month);  //and 客户编号<>'880117'

            dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql_付款, strcon);

            //加入 原本期初期末中没有的客户（开票主表中）
            foreach (DataRow dr in dt.Rows)  
            {
                DataRow r = dtm.NewRow();
                r["客户编号"] = dr["客户编号"];
                r["客户名称"] = dr["客户名称"];
                r["期初余额"] = 0;
                r["开票金额"] = 0;
                r["收款金额"] = 0;
                r["期末余额"] = 0;
                r["结转日期"] = t_now;
                r["年"] = t2.Year;
                r["月"] = t2.Month;
                dtm.Rows.Add(r);
            }

            int i = 0;
            //到这里 加载出所有要结转的 客户的 dtm  然后循环dtm 进行结转 叠加 开票金额 和 付款金额
            foreach (DataRow r in dtm.Rows)
            {
                label2.Text = i++.ToString() + "/" + dtm.Rows.Count.ToString();
                Application.DoEvents();
                string s = string.Format(@"select 客户编号,SUM(开票税后金额) 开票金额 from 销售记录销售开票主表  
 where 生效=1 and 开票日期 >'{0}' and  开票日期 <'{1}' and 客户编号='{2}' and  作废=0 group by 客户编号 ",t1.AddMonths(1),t2.AddMonths(1),r["客户编号"]);

                DataTable t_kp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                if (t_kp.Rows.Count > 0)
                {
                    r["开票金额"] = t_kp.Rows[0]["开票金额"];
                }

                string s_fk = string.Format(@"select 客户编号,SUM(金额) 付款金额 from  客户付款记录表
   where  付款日期 >'{0}' and  付款日期<'{1}' and 客户编号='{2}' group by 客户编号", t1.AddMonths(1), t2.AddMonths(1),r["客户编号"]);
                DataTable t_fk = CZMaster.MasterSQL.Get_DataTable(s_fk,strcon);
                if (t_fk.Rows.Count > 0)
                {
                    r["收款金额"] = t_fk.Rows[0]["付款金额"];
                }
                r["期末余额"] = Convert.ToDecimal(r["期初余额"]) + Convert.ToDecimal(r["开票金额"]) - Convert.ToDecimal(r["收款金额"]);


            }

            label2.Text = "";
            label1.Text = "计算完成，正在保存";
            Application.DoEvents();
            string l="select *  from  销售客户期初期末值 where 1<>1";
            using(SqlDataAdapter da =new SqlDataAdapter (l,strcon))
            {
                new SqlCommandBuilder(da);
                da.Update(dtm);
            }
            label1.Text = "保存成功";

        }

        private void fun_delete(DateTime t)
        {

            string sql = string.Format("delete  销售客户期初期末值 where 年={0} and 月={1}", t.Year, t.Month);
            CZMaster.MasterSQL.ExecuteSQL(sql,strcon);

        }
        private bool  fun_check(DateTime t)
        {
            bool bl=false;
            flag = false;
            int year = t.Year;
            int month = t.Month;

            string sql = string.Format("select count(*)列数 from  销售客户期初期末值 where 年='{0}' and 月='{1}'",year,month);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            if (Convert.ToDecimal(dt.Rows[0]["列数"]) > 0)
            {
                //throw new Exception(string.Format("{0}月已做过结算",month));
                if (MessageBox.Show(string.Format("{0}月已做过结算,是否对{0}月重新进行期末结转？", t.Month), "确认!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    bl = true;
                    flag = true;
                }
                else
                {
                   bl= false;
                }
            }
            else
            {
                bl = true;
            }
            return bl;
        }
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                // 这个月初  算上个月的 结转
                //DateTime t=CPublic.Var.getDatetime();
                // DateTime t2 = new DateTime(t.Year, t.Month,1);
                //t = t.AddMonths(-1);       //上月初   
                //t2 = t2.AddSeconds(-1);
                label1.Text = "进度条:";
                DateTime t = Convert.ToDateTime(barEditItem1.EditValue.ToString());

                t = new DateTime(t.Year,t.Month,1);


              //  DateTime t2 = t.AddMonths(1);
                DateTime t2 = t.AddMonths(-1);

                //t2 = t2.AddSeconds(-1);
                //DateTime t2 = new DateTime(2017,2,1);
                if (MessageBox.Show(string.Format("确认对{0}月进行期末结转？",t.Month), "确认!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                   bool bl= fun_check(t);
                   if (bl)
                   {
                       if (flag)
                       {
                           fun_delete(t);
                       }
                       fun_jz(t2, t);
                   }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
             try
            {
                DateTime t=Convert.ToDateTime(barEditItem1.EditValue.ToString());
                t = new DateTime(t.Year, t.Month, 1);

                fun_load(t.Year,t.Month);
                gridView1.ViewCaption = string.Format("{0}月明细", t.Month);
                gridView1.OptionsView.ShowViewCaption = true;
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

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
    }
}
