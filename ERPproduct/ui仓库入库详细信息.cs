using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using DevExpress.XtraPrinting;
namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class ui仓库入库详细信息 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {
        #region

        string strcon = CPublic.Var.strConn;
        DataTable dt_仓库 = new DataTable();
        DataTable dtM = new DataTable();
        #endregion

        public ui仓库入库详细信息()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void ui仓库入库详细信息_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_load();
                label11.Left = this.Width * 4 / 5;

                timer1.Start();
                timer2.Start();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
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

                sql = string.Format(@" select jyz.*,生产制令单号,部门名称 as 车间,(合格数量-已入库数量+jyz.重检合格数)可入库数量,待出库数量,预计完工日期 
            from 生产记录生产检验单主表 jyz
          
             left join 人事基础部门表  on 人事基础部门表.部门编号 =生产车间  
            left join 生产记录生产工单表 gd on jyz.生产工单号=gd.生产工单号
            left  join  (select   物料编码,SUM(未出库数量)待出库数量 from 销售记录销售出库通知单明细表  where 完成=0 and 作废=0 and 未出库数量>0 group by 物料编码)c
            on c.物料编码=jyz.物料编码
            where jyz.生效 = 1 and jyz.作废 = 0 and jyz.完成 = 0   and jyz.包装确认 = 1 {0}   order by 预计完工日期", sql_ck);



            }
            else
            {
                sql = @" select jyz.*,生产制令单号,部门名称 as 车间,(合格数量-已入库数量+jyz.重检合格数)可入库数量,待出库数量 ,预计完工日期 
            from 生产记录生产检验单主表  jyz
            left join 人事基础部门表  on 人事基础部门表.部门编号 =生产车间              
            left join 生产记录生产工单表 gd on jyz.生产工单号=gd.生产工单号
            left  join  (select   物料编码,SUM(未出库数量)待出库数量 from 销售记录销售出库通知单明细表  where 完成=0 and 作废=0 and 未出库数量>0 group by 物料编码)c
            on c.物料编码=jyz.物料编码
            where jyz.生效 = 1 and jyz.作废 = 0 and jyz.完成 = 0   and jyz.包装确认 = 1    order by 预计完工日期 ";
            }
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

            if (dt_仓库.Rows.Count == 0 || CPublic.Var.LocalUserID == "admin")
            {
                sql_ck = "";
            }
            sql = string.Format(@"   select  jyz.*,生产制令单号,(合格数量-已入库数量+jyz.重检合格数)可入库数量,
             待出库数量,库存总数,预计完工日期,s.出库通知单明细号,s.未出库数量 
            from 生产记录生产检验单主表 jyz
            left join 仓库物料数量表 kc on kc.物料编码 = jyz.物料编码 
            left join  生产记录生产工单子表 m on m.生产工单号=jyz.生产工单号 
            left  join 销售记录销售出库通知单明细表 s on s.销售订单明细号=m.销售订单明细号
            left join 生产记录生产工单表 gd on jyz.生产工单号=gd.生产工单号
            left  join  (select   物料编码,SUM(未出库数量)待出库数量 from 销售记录销售出库通知单明细表  where 完成=0 and 作废=0 and 未出库数量>0 group by 物料编码)c
            on c.物料编码=jyz.物料编码
            where jyz.生效 = 1 and jyz.作废 = 0 and jyz.完成 = 0  and gd.仓库号=kc.仓库号  and jyz.包装确认 = 1 and 待出库数量>0 {0}  order by jyz.物料编码", sql_ck);
            //            }
            //            else
            //            {
            //                sql = @"   select  生产记录生产检验单主表.*,生产制令单号,(合格数量-已入库数量+生产记录生产检验单主表.重检合格数)可入库数量,
            //             待出库数量,库存总数,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.n原ERP规格型号,预计完工日期,s.出库通知单明细号,s.未出库数量 
            //            from 生产记录生产检验单主表 
            //            left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产检验单主表.物料编码 
            //            left join 仓库物料数量表 on 仓库物料数量表.物料编码 = 基础数据物料信息表.物料编码 
            //            left join  生产记录生产工单子表 m on m.生产工单号=生产记录生产检验单主表.生产工单号 
            //            left  join 销售记录销售出库通知单明细表 s on s.销售订单明细号=m.销售订单明细号
            //            left join 生产记录生产工单表 on 生产记录生产检验单主表.生产工单号=生产记录生产工单表.生产工单号
            //            left  join  (select   物料编码,SUM(未出库数量)待出库数量 from 销售记录销售出库通知单明细表  where 完成=0 and 作废=0 and 未出库数量>0 group by 物料编码)c
            //            on c.物料编码=生产记录生产检验单主表.物料编码
            //            where 生产记录生产检验单主表.生效 = 1 and 生产记录生产检验单主表.作废 = 0 and 生产记录生产检验单主表.完成 = 0 
            //            and 生产记录生产检验单主表.包装确认 = 1 and 待出库数量>0  order by 基础数据物料信息表.原ERP物料编号 ";
            //            }
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dtP = new DataTable();
                //      dtM.Columns.Add("选择", typeof(bool));
                da.Fill(dtP);
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
            left  join 基础数据物料信息表   on 基础数据物料信息表.物料编码=base.物料编码 where base.生效日期 >'{0}' {1} group by  入库人员", t, sql_ck);
            }
            else
            {
                ss = string.Format(@" select  入库人员,COUNT(*)单数 from 生产记录成品入库单明细表 base
            left  join 基础数据物料信息表   on 基础数据物料信息表.物料编码=base.物料编码 where base.生效日期 >'{0}'  group by  入库人员", t);

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
                label3.Text = (i_入库数 + dtM.Rows.Count).ToString(); //总单数

                label9.Text = Math.Round(((decimal)i_入库数 / (decimal)(i_入库数 + dtM.Rows.Count) * 100), 2).ToString() + "%";
                DataRow r = dt_人员.NewRow();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    r[i] = dt.Rows[i]["单数"];
                }
                dt_人员.Rows.Add(r);
                gridControl2.DataSource = dt_人员;
                System.Drawing.Font f = new System.Drawing.Font("宋体", 14, FontStyle.Regular);
                for (int i = 0; i < gridView2.Columns.Count; i++)
                {
                    gridView2.Columns[i].AppearanceHeader.Font = f;
                    gridView2.Columns[i].AppearanceCell.Font = f;
                }
            }

            #endregion



        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
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

#pragma warning disable IDE1006 // 命名样式
        private void timer1_Tick(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_load();
            }
            catch (Exception)
            {

                throw;
            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void timer2_Tick(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            label11.Text = System.DateTime.Today.ToLongDateString() + "\n" + System.DateTime.Now.ToLongTimeString();
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {



                DevExpress.XtraPrinting.XlsxExportOptions options = new XlsxExportOptions(TextExportMode.Text, false, false);


                gridControl1.ExportToXlsx(saveFileDialog.FileName, options);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }





    }
}
