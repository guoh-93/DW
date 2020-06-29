using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using DevExpress.XtraCharts;

namespace OperationalData
{
    public partial class frm运营数据可视化界面 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        System.Timers.Timer tm;
        System.Timers.Timer tm2;
        DataTable dtM;
        DataTable dtM_同期;
        DataRow drM_年;
        DataRow drM_年_同期;
        DataRow drM_月;
        DataRow drM_月_同期;
        DataRow drM_日;
        DataRow drM_日_同期;
        DataTable dt_7天 = new DataTable();
        SecondaryAxisY[] sAY = new SecondaryAxisY[50];
        #endregion

        #region 自用类
        public frm运营数据可视化界面()
        {
            InitializeComponent();
        }

        private void frm运营数据可视化界面_Load(object sender, EventArgs e)
        {
            try
            {
                lab_标题.Text = string.Format("今天是{0}年{1}月{2}日", System.DateTime.Today.Year.ToString("0000"), System.DateTime.Today.Month.ToString("00")
                    , System.DateTime.Today.Day.ToString("00"));
                //第一次打开界面直接读取数据
                fun_载入数据_总览();
                dataBindHelper_年.DataFormDR(drM_年);
                dataBindHelper_月.DataFormDR(drM_月);
                dataBindHelper_日.DataFormDR(drM_日);
                fun_同期对比();
                dataBindHelper_年_同期.DataFormDR(drM_年_同期);
                dataBindHelper_月_同期.DataFormDR(drM_月_同期);
                dataBindHelper_日_同期.DataFormDR(drM_日_同期);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "运营数据界面_界面加载");
            }
        }
        #endregion

        #region 数据读取线程
        //private void fun_Run()
        //{
        //    tm = new System.Timers.Timer() { AutoReset = false, Interval = 10000 };//600000
        //    //tm.AutoReset = false;
        //    //tm.Interval = 10800;
        //    tm.Elapsed += tm_Elapsed;
        //    tm.Start();
        //}

        //int i = 0;
        //void tm_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    try
        //    {
        //        fun_载入数据_总览();
        //        i = i + 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        CZMaster.MasterLog.WriteLog(ex.Message, "运营数据界面");
        //    }
        //    finally
        //    {
        //        tm.Start();
        //    }
        //}

        private void fun_载入数据_总览()
        {
            string sql = string.Format("select * from 运营数据管理层数据表 where 年 = '{0}' and (月 = '{1}' or 月 = '0') and (日 = '{2}' or 日 = '0')", 
                System.DateTime.Today.Year, System.DateTime.Today.Month, System.DateTime.Today.Day);
            SqlDataAdapter da = new SqlDataAdapter(sql,strconn);
            dtM = new DataTable();
            da.Fill(dtM);
            try
            {
                drM_年 = dtM.Select(string.Format("年 = '{0}' and 月 = '0' and 日 = '0'", System.DateTime.Today.Year))[0];
            }
            catch
            {
                DataRow dr = dtM.NewRow();
                dtM.Rows.Add(dr);
                foreach (DataColumn dc in dtM.Columns)
                {
                    if (dc.ColumnName == "GUID") continue;
                    if (dc.ColumnName == "日期") continue;
                    dr[dc.ColumnName] = 0;
                }
                drM_年 = dr;
            }
            try
            {
                drM_月 = dtM.Select(string.Format("月 = '{0}' and 日 = '0'", System.DateTime.Today.Month))[0];
            }
            catch
            {
                DataRow dr = dtM.NewRow();
                dtM.Rows.Add(dr);
                foreach (DataColumn dc in dtM.Columns)
                {
                    if (dc.ColumnName == "GUID") continue;
                    if (dc.ColumnName == "日期") continue;
                    dr[dc.ColumnName] = 0;
                }
                drM_月 = dr;
            }
            try
            {
                drM_日 = dtM.Select(string.Format("日 = '{0}'", System.DateTime.Today.Day))[0];
            }
            catch
            {
                DataRow dr = dtM.NewRow();
                dtM.Rows.Add(dr);
                foreach (DataColumn dc in dtM.Columns)
                {
                    if (dc.ColumnName == "GUID") continue;
                    if (dc.ColumnName == "日期") continue;
                    dr[dc.ColumnName] = 0;
                }
                drM_日 = dr;
            }
        }
        private void fun_同期对比()
        {
            string sql = string.Format("select * from 运营数据管理层数据表 where 年 = '{0}' and (月 = '{1}' or 月 = '0') and (日 = '{2}' or 日 = '0')",
                System.DateTime.Today.Year - 1, System.DateTime.Today.Month, System.DateTime.Today.Day);
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            dtM_同期 = new DataTable();
            da.Fill(dtM_同期);
            if (dtM_同期.Rows.Count == 0)
            {
                DataRow dr = dtM_同期.NewRow();
                dtM_同期.Rows.Add(dr);
                foreach (DataColumn dc in dtM_同期.Columns)
                {
                    if (dc.ColumnName == "GUID") continue;
                    if (dc.ColumnName == "日期") continue;
                    dr[dc.ColumnName] = 0;
                }
                drM_年_同期 = dr;
                drM_月_同期 = dr;
                drM_日_同期 = dr;
            }
            else
            {
                drM_年_同期 = dtM_同期.Select(string.Format("年 = '{0}' and 月 = '0' and 日 = '0'", System.DateTime.Today.Year - 1))[0];
                drM_月_同期 = dtM_同期.Select(string.Format("月 = '{0}' and 日 = '0'", System.DateTime.Today.Month))[0];
                drM_日_同期 = dtM_同期.Select(string.Format("日 = '{0}'", System.DateTime.Today.Day))[0];
            }
        }
        //第二种定时器方案
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                fun_载入数据_总览();
                dataBindHelper_年.DataFormDR(drM_年);
                dataBindHelper_月.DataFormDR(drM_月);
                dataBindHelper_日.DataFormDR(drM_日);
                fun_同期对比();
                dataBindHelper_年_同期.DataFormDR(drM_年_同期);
                dataBindHelper_月_同期.DataFormDR(drM_月_同期);
                dataBindHelper_日_同期.DataFormDR(drM_日_同期);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "运营数据界面");
            }
        }
        #endregion

        #region 获取指定日期的数据
        private void fun_7天数据(int i_年, int i_月, int i_日)
        {
            dt_7天.Clear();
            string sql = string.Format("select * from 运营数据管理层数据表 where 年 = '{0}' and 月 = '{1}' and 日 = '{2}'", i_年, i_月, i_日);
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_7天);
        }
        #endregion

        #region 折线图
        //return dt_折线图数据源;
        private DataTable CreateData(string str_类型)
        {
            DateTime dt_today = DateTime.Today;
            //创建dt
            DataTable dt_折线图数据源 = new DataTable();
            dt_折线图数据源.Columns.Add(new DataColumn("类型"));
            dt_折线图数据源.Columns.Add(new DataColumn(string.Format("{0}-{1}", dt_today.Month, dt_today.Day), typeof(decimal)));
            for (int i = 1; i <= 6; i++)
            {
                dt_折线图数据源.Columns.Add(new DataColumn(string.Format("{0}-{1}", dt_today.AddDays(-i).Month, dt_today.AddDays(-i).Day), typeof(decimal)));
            }

            DataRow dr = dt_折线图数据源.NewRow();
            dt_折线图数据源.Rows.Add(dr);
            dr["类型"] = str_类型;
            try
            {
                fun_7天数据(dt_today.Year, dt_today.Month, dt_today.Day);
                try
                {
                    dr[string.Format("{0}-{1}", dt_today.Month, dt_today.Day)] = dt_7天.Rows[0][str_类型];
                }
                catch
                {
                    dr[string.Format("{0}-{1}", dt_today.Month, dt_today.Day)] = 0;
                }
                for (int i = 1; i <= 6; i++)
                {
                    fun_7天数据(dt_today.AddDays(-i).Year, dt_today.AddDays(-i).Month, dt_today.AddDays(-i).Day);
                    dr[string.Format("{0}-{1}", dt_today.AddDays(-i).Month, dt_today.AddDays(-i).Day)] = dt_7天.Rows[0][str_类型];
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
            }
            return dt_折线图数据源;

        }

        private void CreateChart(DataTable dt, string str_类型, ChartControl chart)
        {
            #region Series
            //创建几个图形的对象
            Series series1 = CreateSeries(str_类型, ViewType.Line, dt, 0);
            //Series series2 = CreateSeries("人均月薪", ViewType.Line, dt, 1);
            #endregion

            //List<Series> list = new List<Series>() { series1, series2, series3, series4, series5 };
            List<Series> list = new List<Series>() { series1 };
            chart.Series.AddRange(list.ToArray());
            chart.Legend.Visible = true;
            ////chart.SeriesTemplate.LabelsVisibility = DefaultBoolean.True; //没有找到该属性zf

            for (int i = 0; i < list.Count; i++)
            {
                //list[i].View.Color = colorList[i];
                CreateAxisY(list[i], chart);
            }
        }

        // 根据数据创建一个图形展现
        private Series CreateSeries(string caption, ViewType viewType, DataTable dt, int rowIndex)
        {
            Series series = new Series(caption, viewType);
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                string argument = "";//参数名称
                decimal value = 0;//参数值
                try
                {
                    argument = dt.Columns[i].ColumnName;//参数名称
                    value = (decimal)dt.Rows[rowIndex][i];//参数值
                }
                catch
                {
                    argument = dt.Columns[i].ColumnName;//参数名称
                    value = 0;//参数值
                }
                series.Points.Add(new SeriesPoint(argument, value));
            }
            //必须设置ArgumentScaleType的类型，否则显示会转换为日期格式，导致不是希望的格式显示
            //也就是说，显示字符串的参数，必须设置类型为ScaleType.Qualitative
            series.ArgumentScaleType = ScaleType.Qualitative;
            //series.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;//显示标注标签 //没有找到该属性zf           
            return series;
        }

        // 创建图表的第二坐标系
        private SecondaryAxisY CreateAxisY(Series series, ChartControl chart)
        {
            SecondaryAxisY myAxis = new SecondaryAxisY(series.Name);
            ((XYDiagram)chart.Diagram).SecondaryAxesY.Add(myAxis);
            ((LineSeriesView)series.View).AxisY = myAxis;
            myAxis.Title.Text = series.Name;
            myAxis.Title.Alignment = StringAlignment.Far; //顶部对齐
            myAxis.Title.Visible = true; //显示标题
            myAxis.Title.Font = new Font("宋体", 9.0f);
            Color color = series.View.Color;//设置坐标的颜色和图表线条颜色一致
            myAxis.Title.TextColor = color;
            myAxis.Label.TextColor = color;
            myAxis.Color = color;
            for (int i = 0; i < sAY.Length; i++)
            {
                if (sAY[i] == null)
                {
                    sAY[i] = myAxis;
                    break;
                }
            }
            return myAxis;
        }
        #endregion

        #region 界面操作
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                dataBindHelper_年.DataFormDR(drM_年);
                dataBindHelper_月.DataFormDR(drM_月);
                dataBindHelper_日.DataFormDR(drM_日);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "运营数据界面_刷新");
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //tm.Close();
            CPublic.UIcontrol.ClosePage();
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            //删除多余坐标系
            for (int i = 0; i < sAY.Length; i++)
            {
                if (sAY[i] == null) continue;
                try                      
                {
                    ((XYDiagram)chartControl1.Diagram).SecondaryAxesY.Clear();
                }
                catch { }
            }
            chartControl1.Series.Clear();
            //发货额
            DataTable dt = CreateData("发货税前总额");
            CreateChart(dt, "发货税前总额", chartControl1);
            DataTable dt2 = CreateData("发货税后总额");
            CreateChart(dt2, "发货税后总额", chartControl1);
            ((XYDiagram)chartControl1.Diagram).AxisY.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (SecondaryAxisY say in sAY)
            {
                if (say == null) continue;
                try
                {
                    ((XYDiagram)chartControl2.Diagram).SecondaryAxesY.Clear();
                }
                catch { }
            }
            chartControl2.Series.Clear();
            //订单额
            DataTable dt = CreateData("销售订单税前金额");
            CreateChart(dt, "销售订单税前金额", chartControl2);
            DataTable dt2 = CreateData("销售订单税后金额");
            CreateChart(dt2, "销售订单税后金额", chartControl2);
            ((XYDiagram)chartControl2.Diagram).AxisY.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //开票
            foreach (SecondaryAxisY say in sAY)
            {
                if (say == null) continue;
                try
                {
                    ((XYDiagram)chartControl3.Diagram).SecondaryAxesY.Clear();
                }
                catch { }
            }
            chartControl3.Series.Clear();
            //订单额
            DataTable dt = CreateData("销售开票税前金额");
            CreateChart(dt, "销售开票税前金额", chartControl3);
            DataTable dt2 = CreateData("销售开票税后金额");
            CreateChart(dt2, "销售开票税后金额", chartControl3);
            ((XYDiagram)chartControl3.Diagram).AxisY.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //未开票
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //入库金额 
            foreach (SecondaryAxisY say in sAY)
            {
                if (say == null) continue;
                try
                {
                    ((XYDiagram)chartControl5.Diagram).SecondaryAxesY.Clear();
                }
                catch { }
            }
            chartControl5.Series.Clear();
            //订单额
            DataTable dt = CreateData("入库金额");
            CreateChart(dt, "入库金额", chartControl5);
            ((XYDiagram)chartControl5.Diagram).AxisY.Visible = false;
        }

        #region 折线图预备方案
        // fun(dtM);
        //private void fun(DataTable dt)
        //{
        //    //设置标题
        //    ChartTitle chartTitle1 = new DevExpress.XtraCharts.ChartTitle();
        //    chartTitle1.Text = "发货额";
        //    chartTitle1.Font = new Font("宋体", 18F, System.Drawing.FontStyle.Bold);
        //    chartControl1.Titles.Clear();
        //    chartControl1.Titles.Add(chartTitle1);

        //    DataView dv = new DataView(dt);//new一个视图  
        //    //dv.RowFilter = "日 <> 0";
        //    chartControl1.DataSource = dv;//绑定数据源              
        //    chartControl1.SeriesDataMember = "发货税前总额";//指示图绑定的列名              
        //    chartControl1.SeriesTemplate.ArgumentDataMember = "日";//X轴显示值的列名             
        //    chartControl1.SeriesTemplate.ValueDataMembers.AddRange(new string[] { "发货税前总额" });
        //    chartControl1.SeriesTemplate.View = new StackedBarSeriesView();

        //    DevExpress.XtraCharts.LineSeriesView barseriesview1 = new DevExpress.XtraCharts.LineSeriesView();
        //    chartControl1.SeriesTemplate.View = barseriesview1;
        //}
        #endregion

        #region 折线图预备方案2
        ////0607 因为office没有装完全的原因，运行时报错
        //ChartSpaceClass myspace = new ChartSpaceClass();
        //ChChart chday = myspace.Charts.Add(0);
        //chday.Type = ChartChartTypeEnum.chChartTypeLineMarkers;//格式为折线图
        //chday.HasLegend = true;
        //chday.HasTitle = true;
        //chday.Title.Font.Bold = true;
        //chday.Title.Font.Size = 12;
        //chday.Title.Font.Color = "#ff3300";
        //chday.Axes[0].HasTitle = true;
        //chday.Axes[0].Title.Caption = "";//横轴
        //chday.Axes[0].HasTitle = true;
        //chday.Axes[1].Title.Caption = "";//纵轴
        //chday.Axes[1].HasTitle = true;
        //chday.Axes[0].Title.Font.Bold = chday.Axes[1].Title.Font.Bold = true;
        //chday.Axes[0].Title.Font.Color = chday.Axes[1].Title.Font.Color = "#994400";

        ////获取七天的数据
        //string[] data_最近七天 = new string[7];
        //Decimal[] dec_最近七天 = new Decimal[7];
        //dec_最近七天[0] = Convert.ToDecimal("22");
        //dec_最近七天[1] = Convert.ToDecimal("22");
        //dec_最近七天[2] = Convert.ToDecimal("22");
        //dec_最近七天[3] = Convert.ToDecimal("22");
        //dec_最近七天[4] = Convert.ToDecimal("22");
        //dec_最近七天[5] = Convert.ToDecimal("22");
        //dec_最近七天[6] = Convert.ToDecimal("22");
        //chday.Title.Caption = "最近七天" + "" + "走势图";

        ////横轴名称
        //int data_i = 0;
        //for (int i = 0; i < 6; i++)
        //{
        //    data_最近七天[data_i] = string.Format("{0:" + string.Format("{0:D2}",i) + "\r\n月\r\n" + "}",(i+1).ToString());
        //    data_i++;
        //}
        //string strValue = "";
        //string strCateory = "";
        ////数据格式化
        //for (int i = 0; i < data_最近七天.Length; i++)
        //{
        //    strCateory += data_最近七天[i] + '\t';
        //}
        //for (int i = 0; i < dec_最近七天.Length; i++)
        //{
        //    strValue += dec_最近七天[i].ToString() + '\t';
        //}

        ////添加序列
        //chday.SeriesCollection.Add(0);
        //chday.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimSeriesNames,(int)ChartSpecialDataSourcesEnum.chDataLiteral,"销售额");
        //chday.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimCategories, (int)ChartSpecialDataSourcesEnum.chDataLiteral, strCateory);
        //chday.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimValues, (int)ChartSpecialDataSourcesEnum.chDataLiteral, strValue);
        ////输出成GIF文件
        //string str_折线图 = Application.StartupPath + @"\Images\rate.gif";
        //myspace.ExportPicture(str_折线图, "GIF", pictureBox1.Width, pictureBox1.Height);
        //pictureBox1.ImageLocation = str_折线图;
        #endregion
    }
}
