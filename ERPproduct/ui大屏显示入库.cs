using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class ui大屏显示入库 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {

        #region

        string strcon = CPublic.Var.strConn;
        DataTable dt_仓库 = new DataTable();
        DataTable dtM = new DataTable();
        DateTime today = CPublic.Var.getDatetime().Date;
        #endregion


        public ui大屏显示入库()
        {
            InitializeComponent();
         
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {
            #region 待入库清单 所有的
            string sql_ckry = string.Format("select * from 人员仓库对应表 where 工号='{0}'", CPublic.Var.LocalUserID);
            dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_ckry, strcon);
            string sql_ck = "";
            sql_ck = "and gd.仓库号  in(";
            string sql = "";
            if (dt_仓库.Rows.Count > 0)
            {
                foreach (DataRow dr in dt_仓库.Rows)
                {
                    sql_ck = sql_ck + string.Format("'{0}',", dr["仓库号"]);

                }

                sql_ck = sql_ck.Substring(0, sql_ck.Length - 1) + ")";
            }
            else
            {
                sql_ck = "";

            }

            sql = string.Format(@"  select  jyz.*,生产制令单号,(合格数量-已入库数量+jyz.重检合格数)可入库数量,
             待出库数量,库存总数,预计完工日期,s.出库通知单明细号,s.未出库数量 
            from 生产记录生产检验单主表 jyz
            left join 仓库物料数量表 kc on kc.物料编码 = jyz.物料编码 
            left join  生产记录生产工单子表 m on m.生产工单号=jyz.生产工单号 
            left  join 销售记录销售出库通知单明细表 s on s.销售订单明细号=m.销售订单明细号
            left join 生产记录生产工单表 gd on jyz.生产工单号=gd.生产工单号
            left  join  (select   物料编码,SUM(未出库数量)待出库数量 from 销售记录销售出库通知单明细表  where 完成=0 and 作废=0 and 未出库数量>0 group by 物料编码)c
            on c.物料编码=jyz.物料编码
            where jyz.生效 = 1 and jyz.作废 = 0 and jyz.完成 = 0  and gd.仓库号=kc.仓库号  and jyz.包装确认 = 1 and 待出库数量>0 {0}  order by jyz.物料编码", sql_ck);


 
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                dtM = new DataTable();
                //      dtM.Columns.Add("选择", typeof(bool));
                da.Fill(dtM);
                // gridControl1.DataSource = dtM;
                label4.Text = dtM.Rows.Count.ToString(); // 待入库单数 

            }

            //加急单列表
            DataView dvM = new DataView(dtM);
            DateTime t = CPublic.Var.getDatetime().Date;
            dvM.RowFilter = string.Format("加急状态<>'正常' or 预计完工日期<'{0}' ", t);

            label7.Text = dvM.Count.ToString();
            gridControl1.DataSource = dvM;
            //有待出库的 入库记录

            if (dt_仓库.Rows.Count > 0)
            {
                sql = string.Format(@"select 规格型号,物料编码,物料名称,SUM(可入库数量)可入库数量,库存总数,待出库数量,0 as 建议出库, 0 as 建议入库 from (
            select   生产制令单号,(合格数量-已入库数量+jyz.重检合格数)可入库数量,
             待出库数量,库存总数,base.物料编码,base.物料名称 ,base.规格型号,预计完工日期 
            from 生产记录生产检验单主表 jyz
            left join 基础数据物料信息表 base on base.物料编码 = jyz.物料编码 
            left join 仓库物料数量表 kc on kc.物料编码 = base.物料编码 
            left join 生产记录生产工单表 gd on jyz.生产工单号=gd.生产工单号
            left  join  (select   物料编码,SUM(未出库数量)待出库数量 from 销售记录销售出库通知单明细表  where 完成=0 and 作废=0 and 未出库数量>0 group by 物料编码)c
            on c.物料编码=jyz.物料编码
            where jyz.生效 = 1 and jyz.作废 = 0 and jyz.完成 = 0 
            and jyz.包装确认 = 1 and 待出库数量>0 {0}) xx group by  规格型号,库存总数,待出库数量,物料编码,物料名称", sql_ck);



            }
            else
            {
                sql = @"select 规格型号,物料编码,物料名称,SUM(可入库数量)可入库数量,库存总数,待出库数量,0 as 建议出库, 0 as 建议入库  from (
            select   生产制令单号,(合格数量-已入库数量+jyz.重检合格数)可入库数量,
             待出库数量,库存总数,base.物料编码,base.物料名称 ,base.规格型号,预计完工日期 
            from 生产记录生产检验单主表 jyz
            left join 基础数据物料信息表 base on base.物料编码 = jyz.物料编码 
            left join 仓库物料数量表 kc on kc.物料编码 = base.物料编码 
            left join 生产记录生产工单表 gd on jyz.生产工单号=gd.生产工单号
            left  join  (select   物料编码,SUM(未出库数量)待出库数量 from 销售记录销售出库通知单明细表  where 完成=0 and 作废=0 and 未出库数量>0 group by 物料编码)c
            on c.物料编码=jyz.物料编码    where jyz.生效 = 1 and jyz.作废 = 0 and jyz.完成 = 0 
            and jyz.包装确认 = 1 and 待出库数量>0 ) xx group by 规格型号,库存总数,待出库数量,物料编码,物料名称 order by   规格型号";
            }

            //建议出库 先拿库存中的 东西发货
  
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dtP = new DataTable();
                //      dtM.Columns.Add("选择", typeof(bool));
                da.Fill(dtP);
           
                foreach (DataRow dr in dtP.Rows)
                {
                    decimal kc = Convert.ToDecimal(dr["库存总数"]);
                    decimal dck = Convert.ToDecimal(dr["待出库数量"]);
                    decimal rk = Convert.ToDecimal(dr["可入库数量"]);
                    if (dck > kc) //待出库大于库存
                    {
                        dr["建议出库"] = kc;
                        //库存先都出了 然后 不够的再从 待入库取 至0为止
                        if (rk - dck + kc <= 0)
                        {
                            dr["建议入库"] = 0;
                        }
                        else
                        {
                            dr["建议入库"] = rk + kc - dck;
                        }

                    }
                    else  //库存大于待出库   
                    {
                        dr["建议出库"] = dck;
                        dr["建议入库"] = rk;
                    }

                    
                }

                // gridControl1.DataSource = dtM;
                label4.Text = dtM.Rows.Count.ToString(); // 待入库单数 
                gridControl3.DataSource = dtP;
            }
            


            //DataView dvX = new DataView(dtM);
            //dvX.RowFilter = "待出库数量>0";
            // gridControl3.DataSource = dtP;



            #endregion



            #region 各人员入库单数

            // DateTime t = CPublic.Var.getDatetime().Date;
            string ss = "";
            if (dt_仓库.Rows.Count > 0)
            {

                ss = string.Format(@" select  入库人员,COUNT(*)单数 from 生产记录成品入库单明细表 base
                                left join 生产记录生产工单表 gd on gd.生产工单号=base.生产工单号
                                where base.生效日期 >'{0}' {1} group by  入库人员", t, sql_ck);
            }
            else
            {
                ss = string.Format(@" select  入库人员,COUNT(*)单数 from 生产记录成品入库单明细表 base
                        left join 生产记录生产工单表 gd on gd.生产工单号=base.生产工单号 where base.生效日期 >'{0}'  group by  入库人员", t);

            }
            using (SqlDataAdapter da = new SqlDataAdapter(ss, strcon))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                DataTable dt_人员 = new DataTable();
                int i_入库数 = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    dt_人员.Columns.Add(dr["入库人员"].ToString());
                    i_入库数 = i_入库数 + Convert.ToInt32(dr["单数"]);
                }
                label5.Text = i_入库数.ToString();   //已入库单数
                if (i_入库数 != 0)
                {
                    // label3.Text = (i_入库数 + dtM.Rows.Count).ToString(); //总单数
                    decimal dec = Math.Round(((decimal)i_入库数 / (decimal)(i_入库数 + dtM.Rows.Count) * 100), 2);
                    if (dec < 60)
                    {
                        label9.ForeColor = Color.Red;
                    }
                    else if (dec < 90)
                    {
                        label9.ForeColor = Color.Yellow;

                    }
                    else
                    {
                        label9.ForeColor = Color.White;
                    }
                    label9.Text = dec.ToString() + "%";
                    DataRow r = dt_人员.NewRow();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        r[i] = dt.Rows[i]["单数"];
                    }
                    dt_人员.Rows.Add(r);
                    gridControl2.DataSource = dt_人员;
                    gridView2.PopulateColumns();
                    System.Drawing.Font f = new System.Drawing.Font("宋体", 20, FontStyle.Bold);
                    for (int i = 0; i < gridView2.Columns.Count; i++)
                    {
                        gridView2.Columns[i].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        gridView2.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        gridView2.Columns[i].AppearanceHeader.BackColor = Color.CadetBlue;
                        gridView2.Columns[i].AppearanceHeader.ForeColor = Color.CadetBlue;
                        gridView2.Columns[i].AppearanceHeader.Font = f;
                        gridView2.Columns[i].AppearanceCell.Font = f;
                    }
                }
            }

            #endregion
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

#pragma warning disable IDE1006 // 命名样式
        private void ui大屏显示入库_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_load();
          //  label11.Left=this.Width*11/12;

            timer1.Start();
            timer2.Start();
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                //textBox16.Text = "";

                if (gridView1.GetRow(e.RowHandle) == null)
                {
                    return;
                }
                //int j = gv.RowCount;
                //for (int i = 0; i < j; i++)
                //{
                if (gridView1.GetRowCellValue(e.RowHandle, "加急状态").ToString() == "加急")
                {
                    e.Appearance.BackColor = Color.Red;
                    
                }
                else if  (gridView1.GetRowCellValue(e.RowHandle, "加急状态").ToString() == "急")
                {
                    e.Appearance.BackColor = Color.Pink;
              
                }
                if (Convert.ToDateTime(gridView1.GetRowCellValue(e.RowHandle, "预计完工日期"))<today)
                {
                    e.Appearance.BackColor = Color.Gold;
                
                }

                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView3_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (Convert.ToInt32(gridView3.GetRowCellValue(e.RowHandle, "建议出库")) > 0 || Convert.ToInt32(gridView3.GetRowCellValue(e.RowHandle, "建议数")) > 0)
            {

                e.Appearance.BackColor = Color.FromArgb(205, 232, 254);
                  //   Color.FromArgb(205, 232, 254);

            }
            //else if (Convert.ToInt32(gridView3.GetRowCellValue(e.RowHandle, "建议数")) < 0)
            //{
            //    e.Appearance.BackColor = Color.Red;
            //}
            //else
            //{
            //    e.Appearance.BackColor = Color.White;
            //}
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_CustomDrawRowIndicator_1(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void timer1_Tick(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                label13.Text = System.DateTime.Today.ToLongDateString() + "\n" + System.DateTime.Now.ToLongTimeString();

            }
            catch (Exception)
            {
                
                
            }
   
        }

#pragma warning disable IDE1006 // 命名样式
        private void timer2_Tick(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_load();

  
            }
            catch (Exception  ex )
            {
                ui大屏显示入库 ui=new ui大屏显示入库 ();
                CPublic.UIcontrol.Showpage(ui, "");
            }
            
        }


    }
}
