using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraCharts;
using CZMaster;

namespace BaseData
{
    public partial class frm人员基础分析 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        DataTable dtM = new DataTable();

        //以下为 学历分布人数
        int dl_博士;
        int dl_硕士研究生;
        int dl_本科;
        int dl_大专;
        int dl_中专;
        int dl_高中;
        int dl_初中及以下;

        //以下为 年龄分布人数
        int dl_20岁以下 =0;
        int dl_20至29岁 = 0;
        int dl_30至39岁 = 0;
        int dl_40至49岁 = 0;
        int dl_50及50岁以上 = 0;

        //以下为 性别分布人数

        int dl_男;
        int dl_女;


        public frm人员基础分析()
        {
            InitializeComponent();
        }


        private void fun_load()
        {
            string sql = @"select *,case when([是否特岗]='true') then '是' else  '否' end as 特岗,DATEDIFF(dd, 出生年月, getdate())/365 as 年龄
                                                from 人事基础员工表 ";
            dtM = MasterSQL.Get_DataTable(sql, strconn);
        }

       

        private void fun_离职率()
        {
            try
            {
                DataTable dt_离职率 = new DataTable();
                if (!dt_离职率.Columns.Contains("月份"))
                {
                    dt_离职率.Columns.Add("月份");
                }
                if (!dt_离职率.Columns.Contains("新入职人数"))
                {
                    dt_离职率.Columns.Add("新入职人数");
                }
                if (!dt_离职率.Columns.Contains("离职人数"))
                {
                    dt_离职率.Columns.Add("离职人数");
                } if (!dt_离职率.Columns.Contains("实际人数"))
                {
                    dt_离职率.Columns.Add("实际人数");
                }
                if (!dt_离职率.Columns.Contains("离职率"))
                {
                    dt_离职率.Columns.Add("离职率");
                }

                DateTime t = CPublic.Var.getDatetime();

                string sql = string.Format(@"select count(员工号) from 人事基础员工表 where 入职年月 < '{0}-1-1' and 在职状态 = '在职'",t.Year);
                decimal dl_总人数 = Convert.ToDecimal(CZMaster.MasterSQL.Get_DataTable(sql, strconn).Rows[0][0]);
                decimal dl_入职人数;
                decimal dl_离职人数;
                decimal dl_实际人数 = dl_总人数;
                for (int i = 1; i <= 12; i++)
                {
                    DataRow dr = dt_离职率.NewRow();
                    dt_离职率.Rows.Add(dr);
                    dr["月份"] = i.ToString() + "月";
                    if (i != 12)
                    {
                        string sql1 = string.Format(@"select count(员工号) from 人事基础员工表 where 入职年月 >='{0}-{1}-1' and 入职年月<'{2}-{3}-1'",t.Year, i,t.Year, i + 1);
                        dl_入职人数 = Convert.ToDecimal(CZMaster.MasterSQL.Get_DataTable(sql1, strconn).Rows[0][0]);
                    }
                    else
                    {
                        string sql1 = string.Format(@"select count(员工号) from 人事基础员工表 where 入职年月>='{0}-12-1' and 入职年月<='{1}-12-31'",t.Year,t.Year);
                        dl_入职人数 = Convert.ToDecimal(CZMaster.MasterSQL.Get_DataTable(sql1, strconn).Rows[0][0]);
                    }
                    if (i != 12)
                    {
                        string sql1 = string.Format(@"select count(员工号) from 人事基础员工表 where 离职时间 >='{0}-{1}-1' and 离职时间<'{2}-{3}-1'", t.Year, i, t.Year, i + 1);
                        dl_离职人数 = Convert.ToDecimal(CZMaster.MasterSQL.Get_DataTable(sql1, strconn).Rows[0][0]);
                    }
                    else
                    {
                        string sql1 = string.Format(@"select count(员工号) from 人事基础员工表 where 离职时间>='{0}-12-1' and 离职时间<='{1}-12-31'", t.Year, t.Year);
                        dl_离职人数 = Convert.ToDecimal(CZMaster.MasterSQL.Get_DataTable(sql1, strconn).Rows[0][0]);
                    }

                    dr["新入职人数"] = dl_入职人数.ToString();
                    dr["离职人数"] = dl_离职人数.ToString();
                    dl_实际人数 = dl_实际人数 + dl_入职人数 - dl_离职人数;

                    dr["实际人数"] = dl_实际人数.ToString();
                    dr["离职率"] = (dl_离职人数 / dl_实际人数).ToString("P");                
                }
                gc.DataSource = dt_离职率;


            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_图表位置()
        {
            //panel2.Height = this.Height / 11 * 6;
            
            //1.Width = this.Width / 3;
            //chartControl2.Width = this.Width / 3;
            //chartControl3.Width = this.Width / 3;
        }

        private void fun_加载饼状图()
        {
            try
            {
                chartControl1.Series.Clear();
                Series s = new Series("学历结构图", ViewType.Pie);

                DataTable table = new DataTable("Table1");
                table.Columns.Add("Name", typeof(String));
                table.Columns.Add("Value", typeof(Int32));
                table.Rows.Add(new object[] { "博士", dl_博士 });
                table.Rows.Add(new object[] { "硕士研究生", dl_硕士研究生 });
                table.Rows.Add(new object[] { "本科", dl_本科 });
                table.Rows.Add(new object[] { "大专", dl_大专 });
                table.Rows.Add(new object[] { "中专", dl_中专 });
                table.Rows.Add(new object[] { "高中", dl_高中 });
                table.Rows.Add(new object[] { "初中及以下", dl_初中及以下 });

                s.ValueDataMembers[0] = "Value";
                s.ArgumentDataMember = "Name";
                s.DataSource = table;
                s.LegendPointOptions.PointView = PointView.Argument;

                //s.ShowInLegend = false;
                s.Label.Font = new Font("宋体", 15, FontStyle.Bold);
                s.Label.LineLength = 6;
                ((PiePointOptions)(s.PointOptions)).PercentOptions.ValueAsPercent = true;
                ((PiePointOptions)(s.PointOptions)).PercentOptions.PercentageAccuracy = 4;
                ((PiePointOptions)(s.PointOptions)).ValueNumericOptions.Format = NumericFormat.Percent;
                ((PiePointOptions)(s.PointOptions)).PointView = PointView.Values;
                //s.LegendPointOptions.ValueNumericOptions.Format = NumericFormat.Percent;
                chartControl1.Series.Add(s);
                (s.Label as PieSeriesLabel).Position = PieSeriesLabelPosition.TwoColumns;
                //--
                chartControl2.Series.Clear();
                Series s_2 = new Series("年龄结构图", ViewType.Pie);

                DataTable table_2 = new DataTable("Table2");
                table_2.Columns.Add("Name", typeof(String));
                table_2.Columns.Add("Value", typeof(Int32));
                table_2.Rows.Add(new object[] { "20岁以下", dl_20岁以下 });
                table_2.Rows.Add(new object[] { "20至29岁", dl_20至29岁 });
                table_2.Rows.Add(new object[] { "30至39岁", dl_30至39岁 });
                table_2.Rows.Add(new object[] { "40至49岁", dl_40至49岁 });
                table_2.Rows.Add(new object[] { "50及50岁以上", dl_50及50岁以上 });

                s_2.ValueDataMembers[0] = "Value";
                s_2.ArgumentDataMember = "Name";
                s_2.DataSource = table_2;
                s_2.LegendPointOptions.PointView = PointView.Argument;

                s_2.Label.Font = new Font("宋体", 15, FontStyle.Bold);
                s_2.Label.LineLength = 6;
                ((PiePointOptions)(s_2.PointOptions)).PercentOptions.ValueAsPercent = true;
                ((PiePointOptions)(s_2.PointOptions)).PercentOptions.PercentageAccuracy = 4;
                ((PiePointOptions)(s_2.PointOptions)).ValueNumericOptions.Format = NumericFormat.Percent;
                ((PiePointOptions)(s_2.PointOptions)).PointView = PointView.Values;
                chartControl2.Series.Add(s_2);
                (s_2.Label as PieSeriesLabel).Position = PieSeriesLabelPosition.TwoColumns;

                chartControl3.Series.Clear();
                Series s_3 = new Series("男女比例图", ViewType.Pie);

                DataTable table_3 = new DataTable("Table3");
                table_3.Columns.Add("Name", typeof(String));
                table_3.Columns.Add("Value", typeof(Int32));
                table_3.Rows.Add(new object[] { "男", dl_男 });
                table_3.Rows.Add(new object[] { "女", dl_女 });

                s_3.ValueDataMembers[0] = "Value";
                s_3.ArgumentDataMember = "Name";
                s_3.DataSource = table_3;
                s_3.LegendPointOptions.PointView = PointView.Argument;

                s_3.Label.Font = new Font("宋体", 15, FontStyle.Bold);
                s_3.Label.LineLength = 6;
                ((PiePointOptions)(s_3.PointOptions)).PercentOptions.ValueAsPercent = true;
                ((PiePointOptions)(s_3.PointOptions)).PercentOptions.PercentageAccuracy = 4;
                ((PiePointOptions)(s_3.PointOptions)).ValueNumericOptions.Format = NumericFormat.Percent;
                ((PiePointOptions)(s_3.PointOptions)).PointView = PointView.Values;
                chartControl3.Series.Add(s_3);
                (s_3.Label as PieSeriesLabel).Position = PieSeriesLabelPosition.TwoColumns;
            }
            catch { }
        }

        private void fun_性别分布()
        {
            try
            {
                string sql_男 = @"select count(员工号) from 人事基础员工表 where 性别 = '男'";
                dl_男 = Convert.ToInt32(CZMaster.MasterSQL.Get_DataTable(sql_男, strconn).Rows[0][0]);

                string sql_女 = @"select count(员工号) from 人事基础员工表 where 性别 = '女'";
                dl_女 = Convert.ToInt32(CZMaster.MasterSQL.Get_DataTable(sql_女, strconn).Rows[0][0]);
            }
            catch { }
        }

        private void fun_年龄分布()
        {
            try
            {
                //string sql_20岁以下 = @"select count(员工号)  from 人事基础员工表 where 年龄 < '20'";
                //dl_20岁以下 = Convert.ToInt32(CZMaster.MasterSQL.Get_DataTable(sql_20岁以下, strconn).Rows[0][0]);

                //string sql_20至29岁 = @"select count(员工号)  from 人事基础员工表 where 年龄 >= '20' and 年龄 <30";
                //dl_20至29岁 = Convert.ToInt32(CZMaster.MasterSQL.Get_DataTable(sql_20至29岁, strconn).Rows[0][0]);

                //string sql_30至39岁 = @"select count(员工号)  from 人事基础员工表 where 年龄 >= '30' and 年龄 <40";
                //dl_30至39岁 = Convert.ToInt32(CZMaster.MasterSQL.Get_DataTable(sql_30至39岁, strconn).Rows[0][0]);

                //string sql_40至49岁 = @"select count(员工号)  from 人事基础员工表 where 年龄 >= '40' and 年龄 <50";
                //dl_40至49岁 = Convert.ToInt32(CZMaster.MasterSQL.Get_DataTable(sql_40至49岁, strconn).Rows[0][0]);

                //string sql_50及50岁以上 = @"select count(员工号)  from 人事基础员工表 where 年龄 >= '50'";
                //dl_50及50岁以上 = Convert.ToInt32(CZMaster.MasterSQL.Get_DataTable(sql_50及50岁以上, strconn).Rows[0][0]);


                foreach (DataRow dr in dtM.Rows)
                {
                    if (Convert.ToInt32(dr["年龄"]) < 20)
                    {
                        dl_20岁以下++;
                    }
                    if (Convert.ToInt32(dr["年龄"]) >= 20 || Convert.ToInt32(dr["年龄"]) < 30)
                    {
                        dl_20至29岁++;
                    }
                    if (Convert.ToInt32(dr["年龄"]) >= 30 || Convert.ToInt32(dr["年龄"]) < 40)
                    {
                        dl_30至39岁++;
                    }
                    if (Convert.ToInt32(dr["年龄"]) >= 40 || Convert.ToInt32(dr["年龄"]) < 50)
                    {
                        dl_40至49岁++;
                    }
                    if (Convert.ToInt32(dr["年龄"]) >= 50)
                    {
                        dl_50及50岁以上++;
                    }
                }
            }
            catch { }

        }

        private void fun_学历分布()
        {
//            string sql_未开工工单 = string.Format(@"select count (a.生产工单号) from  (select 生产记录生产工单表.*,基础数据物料信息表.大类 
//                                        from 生产记录生产工单表  left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产工单表.物料编码
//                                        where  生产记录生产工单表.生效日期>'2017-1-1'and 生产车间='{0}'and 生产记录生产工单表.生效=0 and 生产记录生产工单表.关闭=0 ) a", dt_生产关系.Rows[0]["生产车间"]);
//            dl_未开工 = Convert.ToInt32(CZMaster.MasterSQL.Get_DataTable(sql_未开工工单, strcon).Rows[0][0]);
            try
            {
                string sql_博士 = @"select count(员工号) from 人事基础员工表 where 学历 = '博士'";
                dl_博士 = Convert.ToInt32(CZMaster.MasterSQL.Get_DataTable(sql_博士, strconn).Rows[0][0]);

                string sql_硕士研究生 = @"select count(员工号) from 人事基础员工表 where 学历 = '硕士'";
                dl_硕士研究生 = Convert.ToInt32(CZMaster.MasterSQL.Get_DataTable(sql_硕士研究生, strconn).Rows[0][0]);

                string sql_本科 = @"select count(员工号) from 人事基础员工表 where 学历 = '本科'";
                dl_本科 = Convert.ToInt32(CZMaster.MasterSQL.Get_DataTable(sql_本科, strconn).Rows[0][0]);

                string sql_大专 = @"select count(员工号) from 人事基础员工表 where 学历 = '大专'";
                dl_大专 = Convert.ToInt32(CZMaster.MasterSQL.Get_DataTable(sql_大专, strconn).Rows[0][0]);

                string sql_中专 = @"select count(员工号) from 人事基础员工表 where 学历 = '中专'";
                dl_中专 = Convert.ToInt32(CZMaster.MasterSQL.Get_DataTable(sql_中专, strconn).Rows[0][0]);

                string sql_高中 = @"select count(员工号) from 人事基础员工表 where 学历 = '高中'";
                dl_高中 = Convert.ToInt32(CZMaster.MasterSQL.Get_DataTable(sql_高中, strconn).Rows[0][0]);

                string sql_初中及以下 = @"select count(员工号) from 人事基础员工表 where 学历 = '初中' or 学历 = '初中以下' or 学历 = ''";
                dl_初中及以下 = Convert.ToInt32(CZMaster.MasterSQL.Get_DataTable(sql_初中及以下, strconn).Rows[0][0]);
            }
            catch { }
        }

        private void frm人员基础分析_Load(object sender, EventArgs e)
        {
            try
            {
                fun_load();
                fun_离职率();
                fun_学历分布();
                fun_年龄分布();
                fun_性别分布();
                fun_加载饼状图();
                fun_图表位置();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


    }
}
