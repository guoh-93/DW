using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using DevExpress.XtraCharts;

using CZMaster;

namespace ERPproduct
{
    public partial class frm车间可视化 : UserControl
    {

        #region 变量

        string userID = CPublic.Var.LocalUserID;
        string user = CPublic.Var.localUserName;
        DataTable dt_生产关系 = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);
        string strcon = CPublic.Var.strConn;
        //DataTable dt_已完成;
        //DataTable dt_未完成;
        DataTable dt_未开工工单;
        DataTable dt_已入库工单;
        DataTable dt_在产工单数;
        DataTable dt_已检验未入库;
        DataTable dt_完工未检验;
        static int i_记数 = 0;
        //string str_大类名;
        // int i_大类数;
        #endregion


        #region 加载
        public frm车间可视化()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm车间可视化_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                //fun_加载_所有员工();
                fun_未开工工单();
                fun_已入库工单();
                fun_在产工单();
                fun_已检验未入库();
                fun_完工未检验();
                fun_加载_大类();
                tabControl1.SelectedIndex = 1;

                //fun_加载饼状图();

                //timer1.Start();
                //timer2.Start();

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "fun_load");
                MessageBox.Show(ex.Message);
            }
        }
        #endregion




#pragma warning disable IDE1006 // 命名样式
        private void fun_加载_所有员工()
#pragma warning restore IDE1006 // 命名样式
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

                    textBox1.Text = dt_bm.Rows[0]["部门名称"].ToString();


                    textBox8.Text = dt_bm.Rows[0]["领导姓名"].ToString();
                    textBox2.Text = dt_生产关系.Rows.Count.ToString();

                    gridControl1.DataSource = dt_生产关系;

                }
            }
        }

      
#pragma warning disable IDE1006 // 命名样式
        private void fun_加载_大类()
#pragma warning restore IDE1006 // 命名样式
        {

            //加载 已完成工单的 所有大类 
            string sql_大类 = string.Format(@"select 大类 from 
                                                (select 生产记录成品入库单明细表.*,基础数据物料信息表.大类,基础数据物料信息表.标准单价  from 生产记录成品入库单明细表  left join  基础数据物料信息表
                                                    on   生产记录成品入库单明细表.物料编码=基础数据物料信息表.物料编码
                                                    where  生产记录成品入库单明细表.入库车间='{0}' and 生产记录成品入库单明细表.生效日期>'{1}' and 生产记录成品入库单明细表.生效=1) a
                                                group by a.大类", dt_生产关系.Rows[0]["生产车间"], System.DateTime.Today.AddDays(-1));
            DataTable dt_dl = CZMaster.MasterSQL.Get_DataTable(sql_大类, strcon);


            textBox7.Text = dt_dl.Rows.Count.ToString();

            dt_dl.Columns.Add("该大类产品数", typeof(int));
            foreach (DataRow dr in dt_dl.Rows)
            {
                DataRow[] r = dt_已入库工单.Select(string.Format("大类='{0}'", dr["大类"].ToString()));
                int i = 0;
                foreach (DataRow rrr in r)
                {
                    i = i + Convert.ToInt32(rrr["入库数量"]);
                }
                dr["该大类产品数"] = i;

            }
            gridControl4.DataSource = dt_dl;


        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_加载饼状图()
#pragma warning restore IDE1006 // 命名样式
        {

            //chartControl1.Series.Clear();
            //Series s = new Series("各类工单情况", ViewType.Pie);

            //DataTable table = new DataTable("Table1");
            //table.Columns.Add("Name", typeof(String));
            //table.Columns.Add("Value", typeof(Int32));
            //table.Rows.Add(new object[] { "未开工工单", Convert.ToInt32(textBox4.Text) });
            //table.Rows.Add(new object[] { "已入库工单", Convert.ToInt32(textBox5.Text )});
            //table.Rows.Add(new object[] { "在产工单", Convert.ToInt32(textBox6.Text) });
            //table.Rows.Add(new object[] { "已检验未入库", Convert.ToInt32(textBox11.Text) });
            //table.Rows.Add(new object[] { "完工未检验",  Convert.ToInt32(textBox12.Text)});
      

            //s.ValueDataMembers[0] = "Value";
            //s.ArgumentDataMember = "Name";
            //s.DataSource = table;
            //chartControl1.Series.Add(s);
            ////SeriesPoint point; 
            ////for (int i = 0; i < table.Rows.Count; i++)
            ////{
            ////    point = new SeriesPoint(table.Rows[i]["Name"].ToString(), Convert.ToDouble(table.Rows[i]["Value"].ToString()));
            ////    s.Points.Add(point);
            ////}

            ////s.LegendPointOptions.PointView = PointView.ArgumentAndValues;
            ////s.Label.Font = new Font("宋体", 8);
            ////s.Label.LineLength = 50;

            ////s.DataSource = table;
            ////chartControl1.Series.Add(s);  
            //s.LegendPointOptions.PointView = PointView.ArgumentAndValues;

        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_未开工工单()
#pragma warning restore IDE1006 // 命名样式
        {
            //string sql_未开工工单 = string.Format("select * from 生产记录生产工单表 where  生产车间='{0}' and 生效=0 and 制单日期 >='{1}'", dt_生产关系.Rows[0]["生产车间"], System.DateTime.Today.AddDays(-1));
            string sql_未开工工单 = string.Format(@"select 生产记录生产工单表.*,基础数据物料信息表.大类 
                                        from 生产记录生产工单表  left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产工单表.物料编码
                            
                                        where  生产车间='{0}'and 生产记录生产工单表.生效=0 and 生产记录生产工单表.关闭=0 ", dt_生产关系.Rows[0]["生产车间"]);
           
            dt_未开工工单 = CZMaster.MasterSQL.Get_DataTable(sql_未开工工单, strcon);
            textBox4.Text = dt_未开工工单.Rows.Count.ToString(); //未开工工单数

            gridControl2.DataSource = dt_未开工工单;


        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_已入库工单()
#pragma warning restore IDE1006 // 命名样式
        {
//            string sql_已入库工单 = string.Format(@"select 生产记录成品入库单明细表.*,基础数据物料信息表.大类,基础数据物料信息表.标准单价 from 生产记录成品入库单明细表 left join  基础数据物料信息表
//                                                    on   生产记录成品入库单明细表.物料编码=基础数据物料信息表.物料编码 
//
//                                                    where  生产记录成品入库单明细表.入库车间='{0}' and 生产记录成品入库单明细表.生效日期>='{1}'", dt_生产关系.Rows[0]["生产车间"], System.DateTime.Today);
            string sql_已入库工单 = string.Format(@"select 生产记录成品入库单明细表.*,生产记录生产检验单主表.生产数量,生产记录生产检验单主表.负责人员,
                                             基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.大类,基础数据物料信息表.标准单价,库存总数
                                            ,基础数据物料信息表.原ERP物料编号 from 生产记录成品入库单明细表 left join  基础数据物料信息表
                                        on   生产记录成品入库单明细表.物料编码=基础数据物料信息表.物料编码
                                        left join 仓库物料数量表 on   仓库物料数量表.物料编码=生产记录成品入库单明细表.物料编码
                                        left join 生产记录生产检验单主表 on  生产记录成品入库单明细表.生产工单号=生产记录生产检验单主表.生产工单号
                                   where  生产记录成品入库单明细表.入库车间='{0}' and 生产记录成品入库单明细表.生效日期>'{1}'", dt_生产关系.Rows[0]["生产车间"], System.DateTime.Today.AddDays(-1));
            dt_已入库工单 = CZMaster.MasterSQL.Get_DataTable(sql_已入库工单, strcon);
            textBox5.Text = dt_已入库工单.Rows.Count.ToString(); //已入库工单数

            //求今日产值
            decimal d = 0;
            decimal d_送检总数 = 0;
            decimal d_合格数 = 0;

            foreach (DataRow dr in dt_已入库工单.Rows)
            {
                d = d + Convert.ToDecimal(dr["入库数量"]) * Convert.ToDecimal(dr["标准单价"]);
                string sql_1 = string.Format("select * from [生产记录生产检验单主表] where  生产检验单号='{0}'", dr["生产检验单号"]);
                DataRow drr = CZMaster.MasterSQL.Get_DataRow(sql_1, strcon);
                d_送检总数 = d_送检总数 + Convert.ToDecimal(drr["送检数量"]);
                d_合格数 = d_合格数 + Convert.ToDecimal(drr["合格数量"]);

            }
            textBox3.Text = d.ToString("0.00");
            if (d_送检总数 == 0)
            {
                textBox10.Text = "0";
            }
            else
            {
                textBox10.Text = (d_合格数 / d_送检总数 * 100).ToString("0") + "%";
            }
            gridControl5.DataSource = dt_已入库工单;
            //求总合格率




        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_已检验未入库()
#pragma warning restore IDE1006 // 命名样式
        {
            //string sql_已检验未入库 = string.Format(@"select * from 生产记录生产检验单主表 where 生产工单号 not in (select 生产工单号 from 生产记录成品入库单明细表 )");
            string  sql_已检验未入库 = string.Format(@"select 生产记录生产工单表.*,基础数据物料信息表.原ERP物料编号,生产记录生产检验单主表.包装确认,库存总数  from 生产记录生产工单表,基础数据物料信息表,生产记录生产检验单主表,仓库物料数量表  
                                                 where  基础数据物料信息表.物料编码=生产记录生产工单表.物料编码 and 生产记录生产工单表.物料编码=仓库物料数量表.物料编码 and 生产记录生产工单表.生产工单号= 生产记录生产检验单主表.生产工单号
                                                    and 生产记录生产工单表.生产工单号 in ( select 生产工单号 from 生产记录生产检验单主表 
                                                            where 生产工单号 not in (select 生产工单号 from 生产记录成品入库单明细表 ) and 检验日期 > '{0}') and  生产记录生产工单表.生产车间='{1}'", System.DateTime.Today.AddDays(-1),dt_生产关系.Rows[0]["生产车间"]);
            
            dt_已检验未入库 = CZMaster.MasterSQL.Get_DataTable(sql_已检验未入库, strcon);

            gridControl6.DataSource = dt_已检验未入库;

            textBox11.Text = dt_已检验未入库.Rows.Count.ToString();

        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_在产工单()
#pragma warning restore IDE1006 // 命名样式
        {    //and  生效日期>'{1}'  , System.DateTime.Today
//            string sql_在产工单 = string.Format(@"select * from 生产记录生产工单表 
//                                        where  生效=1 and  生产车间='{0}'  
//                                        and  生产工单号 not in(select 生产工单号 from [生产记录成品入库单明细表])", dt_生产关系.Rows[0]["生产车间"]);
            string  sql_在产工单 = string.Format(@"select 生产记录生产工单表.*,基础数据物料信息表.原ERP物料编号  from 生产记录生产工单表,基础数据物料信息表 
                                        where  基础数据物料信息表.物料编码=生产记录生产工单表.物料编码 and  生产记录生产工单表.生效=1 and 生产记录生产工单表.完工=0 and 生产记录生产工单表.关闭=0
                                       and 生产记录生产工单表.生产车间='{0}' and 生产记录生产工单表.生效日期<'{1}'",
                                    dt_生产关系.Rows[0]["生产车间"],System.DateTime.Today.AddDays(1));
            dt_在产工单数 = CZMaster.MasterSQL.Get_DataTable(sql_在产工单, strcon);
            textBox6.Text = dt_在产工单数.Rows.Count.ToString(); //在线工单数
            gridControl3.DataSource = dt_在产工单数;
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_完工未检验()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql_完工未检验 = "";
          
                sql_完工未检验 = string.Format(@"select 生产记录生产工单表.*,基础数据物料信息表.原ERP物料编号  from 生产记录生产工单表,基础数据物料信息表 
                                    where  基础数据物料信息表.物料编码=生产记录生产工单表.物料编码 and 生产记录生产工单表.生效=1 and 生产记录生产工单表.完工=1 
                                and 生产记录生产工单表.生产工单号 not in (select 生产工单号 from 生产记录生产检验单主表) and 生产记录生产工单表.生产车间='{0}' 
                                and 生产记录生产工单表.完工日期>'{1}'", dt_生产关系.Rows[0]["生产车间"],System.DateTime.Today.AddDays(-1));
          
           

            dt_完工未检验 = CZMaster.MasterSQL.Get_DataTable(sql_完工未检验, strcon);

            textBox12.Text = dt_完工未检验.Rows.Count.ToString();
            gridControl7.DataSource = dt_完工未检验;

        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_save()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = "select * from 车间可视化记录表 where 1<>1";
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    da.Fill(dt);
                    DataRow dr = dt.NewRow();

                    dr["日期"] = DateTime.Now;
                    dr["生产车间编号"] = textBox1.Text;
                    dr["总员工数"] = Convert.ToInt32(textBox2.Text);
                    dr["出勤员工数"] = Convert.ToInt32(textBox9.Text);
                    dr["今日产值"] = Convert.ToDecimal(textBox3.Text);
                    dr["总合格率"] = Convert.ToInt32(textBox10.Text);
                    dr["未开工工单数"] = Convert.ToInt32(textBox4.Text);

                    dr["在产工单数"] = Convert.ToInt32(textBox6.Text);
                    dr["已入库工单数"] = Convert.ToInt32(textBox5.Text);
                    dr["已检验未入库数"] = Convert.ToInt32(textBox4.Text);


                    dr["生产大类数"] = Convert.ToInt32(textBox7.Text);

                    dt.Rows.Add(dr);
                    new SqlCommandBuilder(da);
                    da.Update(dt);



                }




            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView3_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }
        //切换tabpage
#pragma warning disable IDE1006 // 命名样式
        private void timer1_Tick(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {



            i_记数 = tabControl1.SelectedIndex;
            i_记数++;
            if (i_记数 == tabControl1.TabCount)
            {
                tabControl1.SelectedIndex = 0;
                i_记数 = 0;
            }
            else
            {
                tabControl1.SelectedIndex = i_记数;
            }


        }
        //刷新 
#pragma warning disable IDE1006 // 命名样式
        private void timer2_Tick(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            if (DateTime.Now > DateTime.Parse(DateTime.Now.ToShortDateString() + "7:30:00") && DateTime.Now < DateTime.Parse(DateTime.Now.ToShortDateString() + "21:30:00"))
            {

                frm车间可视化_Load(null, null);

            }
            //
            if (DateTime.Now.CompareTo(DateTime.Today.AddHours(21)) >= 0)
            {
                string sql = string.Format("select * from 车间可视化记录表 where 日期>'{0}'", System.DateTime.Today);
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                if (dt.Rows.Count > 0)
                {
                    fun_save();
                }
            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView9_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView5_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            frm车间可视化_Load(null, null);  
        }

    }
}
