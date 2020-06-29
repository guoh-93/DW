using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraCharts;
using System.Data.SqlClient;
using System.Runtime.InteropServices;


namespace ERPorg
{
    public partial class ui车间视图 : UserControl
    {
        

        #region 变量

        string userID = CPublic.Var.LocalUserID;
        string user = CPublic.Var.localUserName;
        DataTable dt_生产关系 = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);
        string strcon = CPublic.Var.strConn;
        DataTable dt_cpgd;
        DataTable dt_cpdl;
        DataTable dtM;
        /// <summary>
        /// 以下dt 都是产品数
        /// </summary>
        DataTable dt_未开工产品;
        DataTable dt_已入库产品;
        DataTable dt_在产产品;
        DataTable dt_已检未入;
        DataTable dt_完工未检;

        static int i_记数 = 0;
        //string str_大类名;
        // int i_大类数;

        //以下为 各类工单数量
        int dl_未开工;
        int dl_在产;
        int dl_完工未检;
        int dl_已检未入;
        int dl_已入
            ;
        //每个状态 按大类 分
        DataTable dt_未开工_dl;
        DataTable dt_在产_dl;
        DataTable dt_完工未检_dl;
        DataTable dt_已检未入_dl;
        DataTable dt_已入_dl;






        #endregion
        public ui车间视图()
        {
            
            InitializeComponent();
        
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
 
        private void ui车间视图_Load(object sender, EventArgs e)
        {
            try
            {
                if (dt_生产关系.Rows[0]["生产车间"].ToString() == "")
                {

                    throw new Exception("该界面只有各车间主任可查看信息");
                }
        
            //this.Dock = DockStyle.None;
            //this.Left = 0;
            //this.Top = 0;
            //this.Width = Screen.PrimaryScreen.Bounds.Width;
            //this.Height = Screen.PrimaryScreen.WorkingArea.Height;
            //SetParent(this.Handle, IntPtr.Zero);
           
            try
            {
                
                fun_加载_顶层();
                fun_在产工单();
                fun_未开工工单();
                fun_完工未检验();
                fun_已检验未入库();
                fun_已入库工单();
                fun_产品大类工单();// 产品大类工单饼状图 所需数据
                fun_loaddtm();
                fun_加载饼状图();
                fun_图表位置();
              
               
               
                timer1.Start();
                timer2.Start();
            }
            catch 
            {


                fun_加载_顶层();
                fun_在产工单();
                fun_未开工工单();
                fun_完工未检验();
                fun_已检验未入库();
                fun_已入库工单();
                fun_产品大类工单();// 产品大类工单饼状图 所需数据
                fun_loaddtm();
                fun_加载饼状图();
                fun_图表位置();
              
            }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void fun_加载_顶层()
        {
            //加载 车间领导  和 所有人员
            string sql = string.Format("select * from 人事基础员工表 where 部门编号='{0}' ", dt_生产关系.Rows[0]["生产车间"]);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {


                    //dt_生产关系 为 该车间所有人员 信息的dt
                    //dt_生产关系 = ERPorg.Corg.fun_hr("生产", dt.Rows[0]["员工号"].ToString());


                    string sql_部门领导 = string.Format("select * from [人事基础部门表] where 部门编号='{0}'", dt_生产关系.Rows[0]["生产车间"].ToString());
                    DataTable dt_bm = CZMaster.MasterSQL.Get_DataTable(sql_部门领导, strcon);

                    label4.Text = dt_bm.Rows[0]["部门名称"].ToString();


                    label5.Text = dt_bm.Rows[0]["领导姓名"].ToString();
                    label6.Text = "100%";



                }
            }
        }


        private void fun_加载_大类()
        {

            //加载 已完成工单的 所有大类 
            //            string sql_大类 = string.Format(@"select 大类 from 
            //                                                (select 生产记录成品入库单明细表.*,基础数据物料信息表.大类,基础数据物料信息表.标准单价  from 生产记录成品入库单明细表  left join  基础数据物料信息表
            //                                                    on   生产记录成品入库单明细表.物料编码=基础数据物料信息表.物料编码
            //                                                    where  生产记录成品入库单明细表.入库车间='{0}' and 生产记录成品入库单明细表.生效日期>'{1}' and 生产记录成品入库单明细表.生效=1) a
            //                                                group by a.大类", dt_生产关系.Rows[0]["生产车间"], System.DateTime.Today.AddDays(-1));
            //            DataTable dt_dl = CZMaster.MasterSQL.Get_DataTable(sql_大类, strcon);




            //            dt_dl.Columns.Add("该大类产品数", typeof(int));
            //            foreach (DataRow dr in dt_dl.Rows)
            //            {
            //                DataRow[] r = dt_已入库工单.Select(string.Format("大类='{0}'", dr["大类"].ToString()));
            //                int i = 0;
            //                foreach (DataRow rrr in r)
            //                {
            //                    i = i + Convert.ToInt32(rrr["入库数量"]);
            //                }
            //                dr["该大类产品数"] = i;

            //            }


        }

        private void fun_加载饼状图()
        {

            chartControl1.Series.Clear();
            Series s = new Series("工单进度分布图", ViewType.Pie);

            DataTable table = new DataTable("Table1");
            table.Columns.Add("Name", typeof(String));
            table.Columns.Add("Value", typeof(Int32));
            table.Rows.Add(new object[] { "未开工工单", dl_未开工 });
            table.Rows.Add(new object[] { "已入库工单", dl_已入 });
            table.Rows.Add(new object[] { "在产工单", dl_在产 });
            table.Rows.Add(new object[] { "已检验未入库", dl_已检未入 });
            table.Rows.Add(new object[] { "完工未检验", dl_完工未检 });

          
            s.ValueDataMembers[0] = "Value";
            s.ArgumentDataMember = "Name";
            s.DataSource = table;
            //s.LegendPointOptions.PointView = PointView.ArgumentAndValues;
            s.LegendPointOptions.PointView = PointView.Argument;

            //s.ShowInLegend = false;
            s.Label.Font = new Font("宋体", 15,FontStyle.Bold);
            s.Label.LineLength = 6;
            ((PiePointOptions)(s.PointOptions)).PercentOptions.ValueAsPercent = true;
            ((PiePointOptions)(s.PointOptions)).PercentOptions.PercentageAccuracy = 4;
            ((PiePointOptions)(s.PointOptions)).ValueNumericOptions.Format = NumericFormat.Percent;
            ((PiePointOptions)(s.PointOptions)).PointView = PointView.Values;
            //s.LegendPointOptions.ValueNumericOptions.Format = NumericFormat.Percent;
            chartControl1.Series.Add(s);
            (s.Label as PieSeriesLabel).Position = PieSeriesLabelPosition.TwoColumns;

            chartControl2.Series.Clear();
            Series s_2 = new Series("产品大类-工单分布图", ViewType.Pie);
            s_2.ValueDataMembers[0] = "工单数";
            s_2.ArgumentDataMember = "大类";
            s_2.DataSource = dt_cpgd;
            
            //s_2.LegendPointOptions.PointView = PointView.ArgumentAndValues;
            s_2.LegendPointOptions.PointView = PointView.Argument;

            //s_2.ShowInLegend = false;
            //s_2.LegendPointOptions.ValueNumericOptions.Format = NumericFormat.Percent;
            ((PiePointOptions)(s_2.PointOptions)).PointView = PointView.Values;

            ((PiePointOptions)(s_2.PointOptions)).PercentOptions.ValueAsPercent = true;
            ((PiePointOptions)(s_2.PointOptions)).PercentOptions.PercentageAccuracy = 4;
            ((PiePointOptions)(s_2.PointOptions)).ValueNumericOptions.Format = NumericFormat.Percent;
            s_2.Label.Font = new Font("宋体", 15, FontStyle.Bold);
            (s_2.Label as PieSeriesLabel).Position = PieSeriesLabelPosition.TwoColumns;
            s_2.Label.LineLength = 6;
            chartControl2.Series.Add(s_2);

            chartControl3.Series.Clear();
            Series s_3 = new Series("产品大类-数量分布图", ViewType.Pie);
            s_3.ValueDataMembers[0] = "产品数";
            s_3.ArgumentDataMember = "大类";
            s_3.DataSource = dt_cpdl;
            PieSeriesLabel label = s_3.Label as PieSeriesLabel;

            //s_3.LegendPointOptions.PointView = PointView.ArgumentAndValues;
            s_3.LegendPointOptions.PointView = PointView.Argument;

            //s_3.ShowInLegend = false;
            //s_3.LegendPointOptions.ValueNumericOptions.Format = NumericFormat.Percent;
            ((PiePointOptions)(s_3.PointOptions)).PointView = PointView.Values;

            ((PiePointOptions)(s_3.PointOptions)).PercentOptions.PercentageAccuracy = 4;
            ((PiePointOptions)(s_3.PointOptions)).PercentOptions.ValueAsPercent = true;
            ((PiePointOptions)(s_3.PointOptions)).ValueNumericOptions.Format = NumericFormat.Percent;
            s_3.Label.Font = new Font("宋体", 15, FontStyle.Bold);
            label.Position = PieSeriesLabelPosition.TwoColumns;
            s_3.Label.LineLength = 6;

            chartControl3.Series.Add(s_3);


        }
        /// <summary>
        /// 按大类group by 总工单数
        /// </summary>
        private void fun_产品大类工单()
        {
            DateTime dtime = System.DateTime.Today;
            string sql = string.Format(@"select count(生产工单号) as 工单数,基础数据物料信息表.大类 from 生产记录生产工单表,基础数据物料信息表 
                                   where 基础数据物料信息表.物料编码= 生产记录生产工单表.物料编码 and 生产车间='{0}' and 
                                (生效日期>'{1}' or 制单日期>'{2}' or 完成日期>'{3}' or 完工日期>'{4}')  group by 大类", dt_生产关系.Rows[0]["生产车间"], dtime.AddDays(-1), dtime, dtime, dtime);
            dt_cpgd = new DataTable();
            dt_cpgd = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            string sql_1 = string.Format(@"select sum(生产数量) as 产品数,基础数据物料信息表.大类 from 生产记录生产工单表,基础数据物料信息表 
                                   where 基础数据物料信息表.物料编码= 生产记录生产工单表.物料编码   and 生产车间='{0}' and 
                                (生效日期>'{1}' or 制单日期>'{2}' or 完成日期>'{3}' or 完工日期>'{4}')  group by 大类", dt_生产关系.Rows[0]["生产车间"], dtime.AddDays(-1), dtime, dtime, dtime);
            dt_cpdl = new DataTable();
            dt_cpdl = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);


        }

        private void fun_未开工工单()
        {
            //string sql_未开工工单 = string.Format("select * from 生产记录生产工单表 where  生产车间='{0}' and 生效=0 and 制单日期 >='{1}'", dt_生产关系.Rows[0]["生产车间"], System.DateTime.Today.AddDays(-1));
            string sql_未开工工单 = string.Format(@"select count (a.生产工单号) from  (select 生产记录生产工单表.*,基础数据物料信息表.大类 
                                        from 生产记录生产工单表  left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产工单表.物料编码
                                        where  生产记录生产工单表.生效日期>'2017-1-1'and 生产车间='{0}'and 生产记录生产工单表.生效=0 and 生产记录生产工单表.关闭=0 ) a", dt_生产关系.Rows[0]["生产车间"]);
            dl_未开工 = Convert.ToInt32(CZMaster.MasterSQL.Get_DataTable(sql_未开工工单, strcon).Rows[0][0]);
            //
            dt_未开工_dl = new DataTable();
            string sql = string.Format(@"select count (a.生产工单号) as 未开工工单数 ,大类 from  (select 生产记录生产工单表.*,基础数据物料信息表.大类 
                                        from 生产记录生产工单表  left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产工单表.物料编码
                              where 生产记录生产工单表.生效日期>'2017-1-1' and 生产车间='{0}'and 生产记录生产工单表.生效=0 and 生产记录生产工单表.关闭=0 ) a group by 大类", dt_生产关系.Rows[0]["生产车间"]);
            dt_未开工_dl = CZMaster.MasterSQL.Get_DataTable(sql, strcon);


            string sql_未开工产品数 = string.Format(@"select sum(生产数量) as 未开工产品数量,基础数据物料信息表.大类 from 生产记录生产工单表  
                                   left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产工单表.物料编码 
                           where 生产记录生产工单表.生效日期>'2017-1-1' and 生产车间='{0}'and 生产记录生产工单表.生效=0 and 生产记录生产工单表.关闭=0 group by 大类", dt_生产关系.Rows[0]["生产车间"]);
            dt_未开工产品 = new DataTable();
            dt_未开工产品 = CZMaster.MasterSQL.Get_DataTable(sql_未开工产品数, strcon);

        }

        private void fun_已入库工单()
        {
            string sql_已入库工单 = string.Format(@"select count (a.生产工单号) from  ( select 生产记录成品入库单明细表.*,生产记录生产检验单主表.生产数量,生产记录生产检验单主表.负责人员,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.大类,基础数据物料信息表.标准单价,基础数据物料信息表.原ERP物料编号 from 生产记录成品入库单明细表 left join  基础数据物料信息表
                         on   生产记录成品入库单明细表.物料编码=基础数据物料信息表.物料编码
                         left join 生产记录生产检验单主表 on  生产记录成品入库单明细表.生产工单号=生产记录生产检验单主表.生产工单号
                         where  生产记录成品入库单明细表.入库车间='{0}' and 生产记录成品入库单明细表.生效日期>'{1}')a", dt_生产关系.Rows[0]["生产车间"], System.DateTime.Today);
            dl_已入 = Convert.ToInt32(CZMaster.MasterSQL.Get_DataTable(sql_已入库工单, strcon).Rows[0][0]);

            dt_已入_dl = new DataTable();
            string sql = string.Format(@"select count (a.生产工单号) as 已入工单数 ,大类 from  (select 生产记录成品入库单明细表.*,生产记录生产检验单主表.生产数量,生产记录生产检验单主表.负责人员,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.大类,基础数据物料信息表.标准单价,基础数据物料信息表.原ERP物料编号 from 生产记录成品入库单明细表 left join  基础数据物料信息表
                        on   生产记录成品入库单明细表.物料编码=基础数据物料信息表.物料编码
                        left join 生产记录生产检验单主表 on  生产记录成品入库单明细表.生产工单号=生产记录生产检验单主表.生产工单号
                        where  生产记录成品入库单明细表.入库车间='{0}' and 生产记录成品入库单明细表.生效日期>'{1}')a group by 大类", dt_生产关系.Rows[0]["生产车间"], System.DateTime.Today);
            dt_已入_dl = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            string sql_已入库产品数 = string.Format(@"select sum(生产数量)as 已入库产品数 ,基础数据物料信息表.大类 from 生产记录成品入库单明细表 left join  基础数据物料信息表
                           on   生产记录成品入库单明细表.物料编码=基础数据物料信息表.物料编码
                           left join 生产记录生产检验单主表 on  生产记录成品入库单明细表.生产工单号=生产记录生产检验单主表.生产工单号
                           where  生产记录成品入库单明细表.入库车间='{0}' and 生产记录成品入库单明细表.生效日期>'{1}' group by 大类", dt_生产关系.Rows[0]["生产车间"], System.DateTime.Today);
            dt_已入库产品 = new DataTable();
            dt_已入库产品 = CZMaster.MasterSQL.Get_DataTable(sql_已入库产品数, strcon);
            //求今日产值
            //decimal d = 0;
            //decimal d_送检总数 = 0;
            //decimal d_合格数 = 0;

            //foreach (DataRow dr in dt_已入库工单.Rows)
            //{
            //    d = d + Convert.ToDecimal(dr["入库数量"]) * Convert.ToDecimal(dr["标准单价"]);
            //    string sql_1 = string.Format("select * from [生产记录生产检验单主表] where  生产检验单号='{0}'", dr["生产检验单号"]);
            //    DataRow drr = CZMaster.MasterSQL.Get_DataRow(sql_1, strcon);
            //    d_送检总数 = d_送检总数 + Convert.ToDecimal(drr["送检数量"]);
            //    d_合格数 = d_合格数 + Convert.ToDecimal(drr["合格数量"]);

            //}
            //textBox3.Text = d.ToString("0.00");
            //if (d_送检总数 == 0)
            //{
            //    textBox10.Text = "0";
            //}
            //else
            //{
            //    textBox10.Text = (d_合格数 / d_送检总数 * 100).ToString("0") + "%";
            //}
            //gridControl5.DataSource = dt_已入库工单;
            //求总合格率




        }
        private void fun_已检验未入库()
        {
            //string sql_已检验未入库 = string.Format(@"select * from 生产记录生产检验单主表 where 生产工单号 not in (select 生产工单号 from 生产记录成品入库单明细表 )");
            string sql_已检验未入库 = string.Format(@"select count (a.生产工单号) from  (select 生产记录生产工单表.*,基础数据物料信息表.原ERP物料编号,生产记录生产检验单主表.包装确认  from 生产记录生产工单表,基础数据物料信息表,生产记录生产检验单主表  
                                                 where  基础数据物料信息表.物料编码=生产记录生产工单表.物料编码  and 生产记录生产工单表.生产工单号= 生产记录生产检验单主表.生产工单号
                                                    and 生产记录生产工单表.生产工单号 in ( select 生产工单号 from 生产记录生产检验单主表 
                                                            where 生产工单号 not in (select 生产工单号 from 生产记录成品入库单明细表 ) and 检验日期 > '{0}') and  生产记录生产工单表.生产车间='{1}')a", System.DateTime.Today, dt_生产关系.Rows[0]["生产车间"]);
            dl_已检未入 = Convert.ToInt32(CZMaster.MasterSQL.Get_DataTable(sql_已检验未入库, strcon).Rows[0][0]);

            dt_已检未入_dl = new DataTable();
            string sql = string.Format(@"select count (a.生产工单号) as 已检未入工单数 ,大类 from  (select 生产记录生产工单表.*,基础数据物料信息表.大类,生产记录生产检验单主表.包装确认  from 生产记录生产工单表,基础数据物料信息表,生产记录生产检验单主表  
                                      where  基础数据物料信息表.物料编码=生产记录生产工单表.物料编码  and 生产记录生产工单表.生产工单号= 生产记录生产检验单主表.生产工单号
                                     and 生产记录生产工单表.生产工单号 in ( select 生产工单号 from 生产记录生产检验单主表 
                                where 生产工单号 not in (select 生产工单号 from 生产记录成品入库单明细表 ) and 检验日期 > '{0}') and  生产记录生产工单表.生产车间='{1}')a group by 大类", System.DateTime.Today, dt_生产关系.Rows[0]["生产车间"]);
            dt_已检未入_dl = CZMaster.MasterSQL.Get_DataTable(sql, strcon);


            string sql_已检验未入库产品数 = string.Format(@"select sum(生产数量)as 已检未入库产品数, 基础数据物料信息表.大类 from 生产记录生产工单表,基础数据物料信息表
                                                 where  基础数据物料信息表.物料编码=生产记录生产工单表.物料编码  
                                                    and 生产记录生产工单表.生产工单号 in ( select 生产工单号 from 生产记录生产检验单主表 
                                                            where 生产工单号 not in (select 生产工单号 from 生产记录成品入库单明细表 ) and 检验日期 > '{0}')
                                                    and  生产记录生产工单表.生产车间='{1}'group by 大类", System.DateTime.Today, dt_生产关系.Rows[0]["生产车间"]);
            dt_已检未入 = new DataTable();
            dt_已检未入 = CZMaster.MasterSQL.Get_DataTable(sql_已检验未入库产品数, strcon);

        }
        private void fun_在产工单()
        {    //and  生效日期>'{1}'  , System.DateTime.Today
            //            string sql_在产工单 = string.Format(@"select * from 生产记录生产工单表 
            //                                        where  生效=1 and  生产车间='{0}'  
            //                                        and  生产工单号 not in(select 生产工单号 from [生产记录成品入库单明细表])", dt_生产关系.Rows[0]["生产车间"]);
            string sql_在产工单 = string.Format(@"select count (a.生产工单号) from  (select 生产记录生产工单表.*,基础数据物料信息表.原ERP物料编号  from 生产记录生产工单表,基础数据物料信息表 
                                        where  基础数据物料信息表.物料编码=生产记录生产工单表.物料编码 and  生产记录生产工单表.生效=1 and 生产记录生产工单表.完工=0
                                        and 生产记录生产工单表.完成=0  and 生产记录生产工单表.生效日期>'2017-1-1'and 生产记录生产工单表.关闭=0 
                                       and 生产记录生产工单表.生产车间='{0}')a", dt_生产关系.Rows[0]["生产车间"]);            //, System.DateTime.Today
            dl_在产 = Convert.ToInt32(CZMaster.MasterSQL.Get_DataTable(sql_在产工单, strcon).Rows[0][0]);

            dt_在产_dl = new DataTable();
            string sql = string.Format(@"select count (a.生产工单号)as 在产工单数 ,大类 from  (select 生产记录生产工单表.*,基础数据物料信息表.大类  from 生产记录生产工单表,基础数据物料信息表 
                                        where  基础数据物料信息表.物料编码=生产记录生产工单表.物料编码 and  生产记录生产工单表.生效=1 and 生产记录生产工单表.完工=0  and 生产记录生产工单表.完成=0 
                                  and 生产记录生产工单表.生效日期 >'2017-1-1' and 生产记录生产工单表.关闭=0 and 生产记录生产工单表.生产车间='{0}' )a group by 大类",
                                    dt_生产关系.Rows[0]["生产车间"]);     //, System.DateTime.Today
            dt_在产_dl = CZMaster.MasterSQL.Get_DataTable(sql, strcon);



            string sql_在产产品数 = string.Format(@"select sum(生产数量)as 在产数量,基础数据物料信息表.大类  from 生产记录生产工单表,基础数据物料信息表 
                                        where  基础数据物料信息表.物料编码=生产记录生产工单表.物料编码 and  生产记录生产工单表.生效=1 and 生产记录生产工单表.完工=0 
                                      and 生产记录生产工单表.完成=0  and 生产记录生产工单表.生效日期>'2017-1-1'and 生产记录生产工单表.关闭=0 and 生产记录生产工单表.生产车间='{0}' group by 大类",
                                    dt_生产关系.Rows[0]["生产车间"]);             //System.DateTime.Today
            dt_在产产品 = new DataTable();
            dt_在产产品 = CZMaster.MasterSQL.Get_DataTable(sql_在产产品数, strcon);
        }

        private void fun_完工未检验()
        {
            string sql_完工未检验 = "";
            sql_完工未检验 = string.Format(@"select count (a.生产工单号) from  (select 生产记录生产工单表.*,基础数据物料信息表.原ERP物料编号  from 生产记录生产工单表,基础数据物料信息表 
                                    where  基础数据物料信息表.物料编码=生产记录生产工单表.物料编码 and 生产记录生产工单表.生效=1 and 生产记录生产工单表.完工=1 
                                and 生产记录生产工单表.生产工单号 not in (select 生产工单号 from 生产记录生产检验单主表) and 生产记录生产工单表.生产车间='{0}' 
                                and 生产记录生产工单表.完工日期>'{1}')a", dt_生产关系.Rows[0]["生产车间"], System.DateTime.Today);
            dl_完工未检 = Convert.ToInt32(CZMaster.MasterSQL.Get_DataTable(sql_完工未检验, strcon).Rows[0][0]);

            dt_完工未检_dl = new DataTable();
            string sql = string.Format(@"select count (a.生产工单号)as 完工未检工单数,a.大类 from  (select 生产记录生产工单表.*,基础数据物料信息表.大类  from 生产记录生产工单表,基础数据物料信息表 
                                    where  基础数据物料信息表.物料编码=生产记录生产工单表.物料编码 and 生产记录生产工单表.生效=1 and 生产记录生产工单表.完工=1 
                                and 生产记录生产工单表.生产工单号 not in (select 生产工单号 from 生产记录生产检验单主表) and 生产记录生产工单表.生产车间='{0}' 
                                and 生产记录生产工单表.完工日期>'{1}')a group by 大类", dt_生产关系.Rows[0]["生产车间"], System.DateTime.Today);
            dt_完工未检_dl = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            string sql_完工未检产品数 = string.Format(@"select sum(生产数量)as 完工未检 ,基础数据物料信息表.大类  from 生产记录生产工单表,基础数据物料信息表 
                                    where  基础数据物料信息表.物料编码=生产记录生产工单表.物料编码 and 生产记录生产工单表.生效=1 and 生产记录生产工单表.完工=1 
                                and 生产记录生产工单表.生产工单号 not in (select 生产工单号 from 生产记录生产检验单主表) and 生产记录生产工单表.生产车间='{0}' 
                                and 生产记录生产工单表.完工日期>'{1}'  group by 大类", dt_生产关系.Rows[0]["生产车间"], System.DateTime.Today);
            dt_完工未检 = new DataTable();
            dt_完工未检 = CZMaster.MasterSQL.Get_DataTable(sql_完工未检产品数, strcon);

        }

        private void fun_loaddtm()
        {
            dtM = dt_cpgd.Copy();

            dtM.Columns.Remove("工单数");

            DataColumn dc1 = new DataColumn("未开工工单数", typeof(Int32));
            dc1.DefaultValue = 0;
            dtM.Columns.Add(dc1);

            DataColumn dc2 = new DataColumn("在产工单数", typeof(Int32));
            dc2.DefaultValue = 0;
            dtM.Columns.Add(dc2);

            DataColumn dc3 = new DataColumn("完工未检工单数", typeof(Int32));
            dc3.DefaultValue = 0;
            dtM.Columns.Add(dc3);

            DataColumn dc4 = new DataColumn("已检未入工单数", typeof(Int32));
            dc4.DefaultValue = 0;
            dtM.Columns.Add(dc4);

            DataColumn dc5 = new DataColumn("已入工单数", typeof(Int32));
            dc5.DefaultValue = 0;
            dtM.Columns.Add(dc5);

            DataColumn dc6 = new DataColumn("未开工产品数量", typeof(Int32));
            dc6.DefaultValue = 0;
            dtM.Columns.Add(dc6);

            DataColumn dc7 = new DataColumn("在产数量", typeof(Int32));
            dc7.DefaultValue = 0;
            dtM.Columns.Add(dc7);

            DataColumn dc8 = new DataColumn("完工未检", typeof(Int32));
            dc8.DefaultValue = 0;
            dtM.Columns.Add(dc8);

            DataColumn dc9 = new DataColumn("已检未入库产品数", typeof(Int32));
            dc9.DefaultValue = 0;
            dtM.Columns.Add(dc9);

            DataColumn dc10 = new DataColumn("已入库产品数", typeof(Int32));
            dc10.DefaultValue = 0;
            dtM.Columns.Add(dc10);






            foreach (DataRow dr in dt_未开工产品.Rows)
            {
                DataRow[] dr1 = dtM.Select(string.Format("大类='{0}'", dr["大类"]));
                if (dr1.Length > 0)
                {
                    dr1[0]["未开工产品数量"] = dr["未开工产品数量"];
                }
            }
            foreach (DataRow dr in dt_在产产品.Rows)
            {
                DataRow[] dr1 = dtM.Select(string.Format("大类='{0}'", dr["大类"]));
                if (dr1.Length > 0)
                {
                    dr1[0]["在产数量"] = dr["在产数量"];
                }
            }

            foreach (DataRow dr in dt_完工未检.Rows)
            {
                DataRow[] dr1 = dtM.Select(string.Format("大类='{0}'", dr["大类"]));
                if (dr1.Length > 0)
                {
                    dr1[0]["完工未检"] = dr["完工未检"];
                }
            }
            foreach (DataRow dr in dt_已检未入.Rows)
            {
                DataRow[] dr1 = dtM.Select(string.Format("大类='{0}'", dr["大类"]));
                if (dr1.Length > 0)
                {
                    dr1[0]["已检未入库产品数"] = dr["已检未入库产品数"];
                }
            }
            foreach (DataRow dr in dt_已入库产品.Rows)
            {
                DataRow[] dr1 = dtM.Select(string.Format("大类='{0}'", dr["大类"]));
                if (dr1.Length > 0)
                {
                    dr1[0]["已入库产品数"] = dr["已入库产品数"];
                }
            }
            foreach (DataRow dr in dt_未开工_dl.Rows)
            {
                DataRow[] dr1 = dtM.Select(string.Format("大类='{0}'", dr["大类"]));
                if (dr1.Length > 0)
                {
                    dr1[0]["未开工工单数"] = dr["未开工工单数"];
                }
            }
            foreach (DataRow dr in dt_在产_dl.Rows)
            {
                DataRow[] dr1 = dtM.Select(string.Format("大类='{0}'", dr["大类"]));
                if (dr1.Length > 0)
                {
                    dr1[0]["在产工单数"] = dr["在产工单数"];
                }
            }
            foreach (DataRow dr in dt_完工未检_dl.Rows)
            {
                DataRow[] dr1 = dtM.Select(string.Format("大类='{0}'", dr["大类"]));
                if (dr1.Length > 0)
                {
                    dr1[0]["完工未检工单数"] = dr["完工未检工单数"];
                }
            }
            foreach (DataRow dr in dt_已检未入_dl.Rows)
            {
                DataRow[] dr1 = dtM.Select(string.Format("大类='{0}'", dr["大类"]));
                if (dr1.Length > 0)
                {
                    dr1[0]["已检未入工单数"] = dr["已检未入工单数"];
                }
            }
            foreach (DataRow dr in dt_已入_dl.Rows)
            {
                DataRow[] dr1 = dtM.Select(string.Format("大类='{0}'", dr["大类"]));
                if (dr1.Length > 0)
                {
                    dr1[0]["已入工单数"] = dr["已入工单数"];
                }
            }
            gridControl1.DataSource = dtM;


        }

        private void fun_图表位置()
        {
            //int i_起始高度 = panel3.Height;
            //chartControl1.Left = 0;
            panel2.Height = this.Height /11 * 5;
            label1.Left = this.Width / 4;
            label2.Left = this.Width / 2;
            label3.Left = this.Width / 4 * 3;
            label4.Left = (this.Width / 4) - (label4.Text.Length);
            label5.Left = this.Width / 2;
            label6.Left = this.Width / 4 * 3;
            label7.Left = label6.Right+ this.Width / 12;
            chartControl1.Width = this.Width / 4;
            chartControl2.Width = this.Width /8*3;
            chartControl3.Width = this.Width /8*3;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                fun_加载_顶层();
                fun_在产工单();
                fun_未开工工单();
                fun_完工未检验();
                fun_已检验未入库();
                fun_已入库工单();
                fun_产品大类工单();// 产品大类工单饼状图 所需数据
                fun_loaddtm();
                fun_加载饼状图();
                fun_图表位置();
            }
            catch 
            {
                
               
            }
           
        }

        private void timer2_Tick(object sender, EventArgs e)
        {

            label7.Text = System.DateTime.Today.ToLongDateString() + "\n" + System.DateTime.Now.ToLongTimeString();

        }

        private void gridControl1_Click(object sender, EventArgs e)
        {
            
        }

      

      
    }
}
