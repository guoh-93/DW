using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraCharts;

namespace ERPSale
{
    public partial class ui销售数量汇总折线图 : UserControl
    {
        #region
        string strcon = CPublic.Var.strConn;
        DataTable dt_折线;

        DataTable dt_大类;
        DataTable dt_小类;

        DataTable dtM=new DataTable (); // 中间显示数据的dt
        #endregion



        public ui销售数量汇总折线图()
        {
            InitializeComponent();
        }

        private void ui销售数量汇总折线图_Load(object sender, EventArgs e)
        {
            DateTime dtime = CPublic.Var.getDatetime();
            
           DateTime  t1 = new DateTime(dtime.Year, 1, 1);
            dateEdit1.EditValue = t1;
            dateEdit2.EditValue = dtime;
            fun_load();

        }
        private void fun_search()
        {

            dtM = new DataTable();
            string str = "";
            string str_值 = "";
            if (checkBox1.Checked == true)
            {

                    dtM.Columns.Add("产品线");
                    str = "产品线";
                    str_值 = comboBox1.Text.ToString();
            }
            else if (checkBox2.Checked == true)
            {
                if (checkBox3.Checked == true)
                {
                    dtM.Columns.Add("小类");

                    str = "小类";
                    str_值 = searchLookUpEdit3.EditValue.ToString();
                }
                else
                {
                    dtM.Columns.Add("大类");
                    str = "大类";
                    str_值 = searchLookUpEdit2.EditValue.ToString();

                }

            }
            else if (checkBox4.Checked == true)
            {
                dtM.Columns.Add("物料");

                str = "原ERP物料编号";
                str_值 = searchLookUpEdit1.EditValue.ToString();
            }
            //先判断时间段 要汇总几个月的数据              
            DateTime time1 = Convert.ToDateTime(dateEdit1.EditValue);
            DateTime time2 = Convert.ToDateTime(dateEdit2.EditValue);

            int i = time1.Month;
            int j = time2.Month;

            for (int x=i; x <= j;x++ )
            {
                dtM.Columns.Add(string.Format("{0}月", x));
                
            }
           // dtM.Columns.Add(string.Format("{0}月", time2.Month));

            time1 = Convert.ToDateTime(dateEdit1.EditValue);
            DateTime t = new DateTime(time1.Year, time1.Month + 1, 1);
            DataRow dr = dtM.NewRow();
            dr[0] = str_值;

            dtM.Rows.Add(dr);
            for (int k = 1; i <= j; i++, k++)
            {
                string sql = string.Format(@"select sum(已出库数量)已出库数量 from 销售记录成品出库单明细表,基础数据物料信息表 
                where 销售记录成品出库单明细表.物料编码=基础数据物料信息表.物料编码 and 销售记录成品出库单明细表.作废=0 
                and 销售记录成品出库单明细表.生效=1 and  生效日期>'{0}' and  生效日期<'{1}' and {2}='{3}'  group by {2}", time1, t, str, str_值);
                DataTable dt = new DataTable();
                dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                if (dt.Rows.Count== 0)
                {
                    dr[k] = 0;
                }
                else
                {
                    dr[k] =Convert.ToDecimal(dt.Rows[0][0]).ToString("0.00");
                }
                time1 = new DateTime(time1.Year, time1.Month + 1, 1);  // 起始日期可能并不是某个月的第一天 保证下个月条件为 从 1号到 再下个月1号
                t = t.AddMonths(1);
                }

            

            gridControl1.DataSource = dtM;
            gridControl1.MainView.PopulateColumns();
        }
        private void fun_zxt()           //加载折线图
        {
            chartControl1.Series.Clear();
            Series s = new Series("销售数量—月份折线图", ViewType.Line);

          
            s.ArgumentScaleType = ScaleType.Qualitative;

            for (int i = 1; i < dtM.Columns.Count; i++)
            {
                //string argument = dtM.Rows[0][i].ToString();//参数名称

                string argument = dtM.Columns[i].Caption.ToString();//参数名称

                SeriesPoint point;
                if (dtM.Columns[i].Caption.ToString() != "")
                {
                    decimal value = Convert.ToDecimal(dtM.Rows[0][i]);//参数值

                   


                    point = new SeriesPoint(argument, value , 0);
                    point.IsEmpty = false;
                    s.Points.Add(point);


                    point = new SeriesPoint(argument, value , 1);
                    point.IsEmpty = false;
                    s.Points.Add(point);

                }
                else
                {
                    point = new SeriesPoint(argument, 0, 0);
                    point.IsEmpty = true;
                    s.Points.Add(point);

                }

            }
            chartControl1.Series.Add(s);

            s.DataSource = dtM;
        }
        private void fun_check()
        {
            if (checkBox1.Checked == false && checkBox2.Checked == false && checkBox4.Checked == false  )
            {
                throw new Exception("未选择筛选条件");
            }
        }
        private void fun_load()      //加载 大类 下拉框 
        {
            string sql_大类 = "select 物料类型名称 as 大类 from  [基础数据物料类型表] where 类型级别='大类' order by 物料类型名称";
            dt_大类 = CZMaster.MasterSQL.Get_DataTable(sql_大类, strcon);
            searchLookUpEdit2.Properties.DataSource = dt_大类;
            searchLookUpEdit2.Properties.DisplayMember = "大类";
            searchLookUpEdit2.Properties.ValueMember = "大类";

            string sql = "select 原ERP物料编号 as 物料编号,物料名称,规格型号,n原ERP规格型号  from 基础数据物料信息表  where 停用=0 ";
            DataTable dt_物料 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            searchLookUpEdit1.Properties.DataSource = dt_物料;
            searchLookUpEdit1.Properties.DisplayMember = "物料编号";
            searchLookUpEdit1.Properties.ValueMember = "物料编号";


        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_check();
                fun_search();
                fun_zxt(); //加载折线图 
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }



        private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {
            string sql = string.Format(@"select 物料类型名称 as 小类 from  [基础数据物料类型表] where 
            上级类型GUID in  (select 物料类型GUID from [基础数据物料类型表]
            where 类型级别='大类' and 物料类型名称='{0}' ) order by 物料类型名称", searchLookUpEdit2.EditValue.ToString());
            dt_小类 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            searchLookUpEdit3.Properties.DataSource = dt_小类;
            searchLookUpEdit3.Properties.DisplayMember = "小类";
            searchLookUpEdit3.Properties.ValueMember = "小类";
        }
    }
}
