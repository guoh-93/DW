using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPpurchase
{
    public partial class UI开发部采购入库 : UserControl
    {

        #region 变量
        DataTable dt_待办;
        DataTable dt_右;
        string strcon = CPublic.Var.strConn;
        string str_入库单号 = "";
        #endregion
        public UI开发部采购入库()
        {
            InitializeComponent();
        }
        private void UI开发部采购入库_Load(object sender, EventArgs e)
        {
            try
            {
                fun_load();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void fun_load()
        {
            string sql = "select *  from  [采购记录采购单主表] where 生效=1 and 完成=0 and 采购单类型='开发采购' and 作废=0";
            dt_待办 = new DataTable();
            dt_待办 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dt_待办;
        }
        private void fun_load_明细(string str_采购单号)
        {
            string sql = string.Format(@"select 采购记录采购单明细表.*,原ERP物料编号,n原ERP规格型号  from 采购记录采购单明细表,基础数据物料信息表  
                                     where 采购记录采购单明细表.物料编码=基础数据物料信息表.物料编码 and 采购单号='{0}' and 明细完成=0 ", str_采购单号);
            dt_右 = new DataTable();
            dt_右 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            dt_右.Columns.Add("选择", typeof(bool));
            dt_右.Columns.Add("输入入库数");

            //dt_右.Columns.Add("有效库存");
            //dt_右.Columns.Add("库存总数");
            foreach (DataRow dr in dt_右.Rows)
            {
                dr["选择"] = true;
                dr["输入入库数"] = dr["未完成数量"];
            }
            gridControl2.DataSource = dt_右;
        }
        private void fun_save()
        {
            DateTime t = CPublic.Var.getDatetime();
            string ss = t.Year.ToString().Substring(2, 2);
            str_入库单号 = string.Format("DP{0}{1:D2}{2:00}{3:000}", ss, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("DP", t.Year,t.Month));
            //加 开发仓库数量表 数量  修改相应采购明细 未完成数量 完成数量  明细完成

            foreach (DataRow dr in dt_右.Rows)
            {
                if (dr["选择"].Equals(true))
                {
                    dr["完成数量"] = Convert.ToDecimal(dr["完成数量"]) + Convert.ToDecimal(dr["输入入库数"]);
                    dr["未完成数量"] = Convert.ToDecimal(dr["未完成数量"]) - Convert.ToDecimal(dr["输入入库数"]);
                    if (Convert.ToDecimal(dr["未完成数量"]) <= 0)
                    {
                        dr["明细完成"] = 1;
                    }
                }
            }
            string sql = "select * from [采购记录采购单明细表] where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                new SqlCommandBuilder(da);
                da.Update(dt_右);
            }





        }

        private void fun_完成状态()
        {
            DataTable dt = new DataTable();
            DataTable dt_1 = new DataTable();

            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            string sql_MX = string.Format("select * from [采购记录采购单明细表] where 采购单号='{0}'", dr["采购单号"]);

            using (SqlDataAdapter da = new SqlDataAdapter(sql_MX, strcon))
            {
                da.Fill(dt);
            }
            int i = 0;
            foreach (DataRow r in dt.Rows)
            {
                if (r["明细完成"].Equals(false))
                {
                    i++;
                }
            }
            if (i == 0)
            {
                string sql = string.Format("select * from  [采购记录采购单主表] where 采购单号='{0}'", dr["采购单号"]);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {

                    da.Fill(dt_1);
                    if (dt_1.Rows.Count > 0)
                    {
                        dt_1.Rows[0]["完成"] = true;
                        dt_1.Rows[0]["完成日期"] =CPublic.Var.getDatetime();
                    }

                    new SqlCommandBuilder(da);
                    da.Update(dt_1);

                }
            }


        }
        private void fun_save出入库明细()
        {
            int POS = 1;
            DataTable dt = new DataTable();
            DataRow r_代办 = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            string sql = "select * from 仓库出入库明细表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                da.Fill(dt);
              DateTime  t=  CPublic.Var.getDatetime();
                foreach (DataRow dr in dt_右.Rows)
                {
                    if (dr["选择"].Equals(true))
                    {
                       
                        DataRow r = dt.NewRow();
                        r["GUID"] = System.Guid.NewGuid();
                        r["明细类型"] = "开发采购";
                        r["单号"] = str_入库单号;
                        r["出库入库"] = "开发采购入库";
                        r["物料编码"] = dr["物料编码"];
                        r["物料名称"] = dr["物料名称"];
                        r["相关单号"] = r_代办["采购单号"];

                        r["明细号"] = str_入库单号 + POS.ToString("00");
                        r["实效数量"] = (Convert.ToDecimal(dr["输入入库数"]));
                        r["实效时间"] = t;
                        r["出入库时间"] = t;

                        string sql_pd = "select * from 仓库物料盘点表 where 有效=1";
                        using (SqlDataAdapter da1 = new SqlDataAdapter(sql_pd, strcon))
                        {

                            DataTable dt_批次号 = new DataTable();
                            da1.Fill(dt_批次号);
                            if (dt_批次号.Rows.Count > 0)
                            {
                                r["盘点有效批次号"] = dt_批次号.Rows[0]["盘点批次号"];
                            }
                            else
                            {
                                r["盘点有效批次号"] = "初始化";
                            }
                        }
                        dt.Rows.Add(r);


                    }
                    POS++;
                }
                new SqlCommandBuilder(da);
                da.Update(dt);
            }



        }
        private void fun_check()
        {
            int i = 0;
            foreach (DataRow r in dt_右.Rows)
            {
                if (r["选择"].Equals(true))
                {
                    try
                    {
                        decimal a = Convert.ToDecimal(r["输入入库数"]);
                    }
                    catch
                    {
                        throw new Exception("请正确输入领料数量格式");
                    }
                    //string sql = string.Format("select * from 仓库物料数量表 where  物料编码='{0}'", r["物料编码"].ToString());

                    //DataRow dr = CZMaster.MasterSQL.Get_DataRow(sql, strconn);
                    //if (Convert.ToDecimal(r["输入领料数量"]) > Convert.ToDecimal(r["库存总数"]))
                    //{
                    //    throw new Exception("库存总数不足！");
                    //}
                    if (Convert.ToDecimal(r["输入入库数"]) > Convert.ToDecimal(r["未完成数量"]))
                    {
                        i++;
                    }

                }
            }
            if (i > 0)
            {
                if (MessageBox.Show("入库数量大于未完成数量，是否继续？", "提醒", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {

                }
            }

        }
        private void fun_增加库存()
        {
            DateTime t = CPublic.Var.getDatetime();
            foreach (DataRow dr in dt_右.Rows)
            {
                if (dr["选择"].Equals(true))
                {
                    string sql = string.Format(@"update  [开发仓库数量表] set 库存总数=库存总数+{0},出入库时间='{1}' 
                                                where 物料编码='{2}'", dr["输入入库数"],t,dr["物料编码"]);
                    CZMaster.MasterSQL.ExecuteSQL(sql, strcon);
                }
            }
        }
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridView2.CloseEditor();
            this.BindingContext[dt_右].EndCurrentEdit();
            try
            {
                fun_check();
                if (MessageBox.Show(string.Format("确定生效入库？"), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    fun_save();
                    fun_完成状态();
                    fun_save出入库明细();
                    fun_增加库存();
                    MessageBox.Show("ok");

                    barLargeButtonItem1_ItemClick(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            fun_load();
            gridControl2.DataSource = null;
            if (gridView1.RowCount > 0)
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                fun_load_明细(dr["采购单号"].ToString());
            }
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (dr == null) return;
            fun_load_明细(dr["采购单号"].ToString());
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }



    }
}
