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

    //17-9-27 检查 这个界面 也没有用
    public partial class frm采购记录外协领料界面_弃用 : UserControl
    {
        #region 变量
        string strconn = CPublic.Var.strConn;
        DataTable dt_左;
        DataView dv_左;
        DataTable dt_右;

        string str_领料单号;
        DataTable dt_StockDt;
        DataTable dt_员工;
        //DataTable dt_默认仓库 = ERPorg.Corg.fun_hr("仓库", CPublic.Var.LocalUserID);
        #endregion

        #region 加载
        public frm采购记录外协领料界面_弃用()
        {
            InitializeComponent();
        }

        private void frm采购记录外协领料界面_Load(object sender, EventArgs e)
        {
            try
            {
                devGridControlCustom1.UserName = CPublic.Var.LocalUserID;
                devGridControlCustom1.strConn = CPublic.Var.strConn;
                devGridControlCustom2.UserName = CPublic.Var.LocalUserID;
                devGridControlCustom2.strConn = CPublic.Var.strConn;
                fun_load();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        #endregion 

        #region 函数
        private void fun_load()
        {
            string sql_左 = "select * from 采购记录外协采购待领料主表 ";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_左, strconn))
            {
                dt_左 = new DataTable();

                da.Fill(dt_左);

                dv_左 = new DataView(dt_左);
                dv_左.RowFilter = "完成=false";
                gridControl1.DataSource = dv_左;
            }

            string sql_右 = "select * from 采购记录外协采购待领料明细表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_右, strconn))
            {
                dt_右 = new DataTable();
                da.Fill(dt_右);

                dt_右.Columns.Add("选择", typeof(bool));
                dt_右.Columns.Add("输入领料数量");
                gridControl2 .DataSource = dt_右;
            }
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {

            DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (r == null) return;
            dataBindHelper1.DataFormDR(r);

            string sql = string.Format("select * from 采购记录外协采购待领料明细表 where 待领料单号='{0}'", r["待领料单号"]);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                dt_右 = new DataTable();
                da.Fill(dt_右);
                dt_右.Columns.Add("选择", typeof(bool));
                dt_右.Columns.Add("输入领料数量");
                foreach (DataRow dr in dt_右.Rows)
                {
                    dr["选择"] = true;
                    dr["输入领料数量"] = dr["未领数量"];
                }
                gridControl2.DataSource = dt_右;

            }
        }

        private void fun_check()
        {
            if (textBox5.Text == "" ||textBox5.Text == null)
            {
                throw new Exception("没有生效单据");
            }

            int i = 0;
            foreach (DataRow r in dt_右.Rows)
            {
                if (r["选择"].Equals(true))
                {
                    try
                    {
                        decimal a = Convert.ToDecimal(r["输入领料数量"]);

                    }
                    catch
                    {
                        throw new Exception("请正确输入领料数量格式");

                    }
                    string sql = string.Format("select * from 仓库物料数量表 where  物料编码='{0}'", r["物料编码"].ToString());

                    DataRow dr = CZMaster.MasterSQL.Get_DataRow(sql, strconn);
                    if (Convert.ToDecimal(r["输入领料数量"]) > Convert.ToDecimal(dr["库存总数"]))
                    {
                        throw new Exception("库存总数不足！");
                    }
                    if (Convert.ToDecimal(r["输入领料数量"]) > Convert.ToDecimal(r["未领数量"]))
                    {
                        i++;

                    }

                }


            }
            if (i > 0)
            {
                if (MessageBox.Show("领料数量大于未领数量，是否继续？", "提醒", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    throw new Exception("请修改");
                }
            }

        }

        private DataSet fun_save()
        {
            DataSet ds = new DataSet();
            if (textBox1.Text == "")  //新建的 领料出库单
            {

                str_领料单号 = string.Format("ML{0}{1:D2}{2:00}{3:0000}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, CPublic.CNo.fun_得到最大流水号("ML", DateTime.Now.Year, DateTime.Now.Month));
                //保存 主表
                string sql = "select * from 采购记录外协采购领料主表 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    textBox1.Text = str_领料单号;
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    DataRow dr = dt.NewRow();
                    dt.Rows.Add(dr);
                    dr["GUID"] = System.Guid.NewGuid();
                    dr["领料出库单号"] = textBox1.Text;

                    dataBindHelper1.DataToDR(dr);


                    dt.TableName = "主表";
                    ds.Tables.Add(dt);
                    //new SqlCommandBuilder(da);
                    //da.Update(dt);
                }
                string sql1 = "select * from 采购记录外协采购领料明细表 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql1, strconn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    //new SqlCommandBuilder(da);

                    //DataTable dt = dv_右.ToTable();
                    int pos = 0;
                    foreach (DataRow r in dt_右.Rows)
                    {
                        if (r["选择"].Equals(true))
                        {
                            DataRow dr = dt.NewRow();
                            dt.Rows.Add(dr);
                            dr["GUID"] = System.Guid.NewGuid();
                            dr["领料出库单号"] = str_领料单号;
                            dr["POS"] = pos.ToString("00");
                            dr["领料出库明细号"] = str_领料单号 + pos.ToString("00");
                            dr["领料仓库ID"] = textBox2.Text;
                            dr["领料仓库"] = textBox6.Text;
                            dr["领料库位ID"] = textBox14.Text;
                            dr["领料库位"] = textBox15.Text;

                            dr["物料名称"] = r["物料名称"];
                            dr["物料编码"] = r["物料编码"];
                           
                            dr["领料数量"] = r["输入领料数量"];
                            //这里不需要 已领数量和未领数量
                           
                            dr["领料人员ID"] = textBox7.Text;
                            dr["领料人员"] = textBox8.Text;
                           
                            dr["创建日期"] = System.DateTime.Now;
                            pos++;
                        }
                    }
                    dt.TableName = "明细表";
                    ds.Tables.Add(dt);
                    //new SqlCommandBuilder(da);
                    //da.Update(dt);
                }

            }
          

            return (ds);
        }

        private DataTable fun_完成状态()
        {
            DataTable dt = new DataTable();
            DataTable dt_1 = new DataTable();
            string sql_MX = string.Format("select * from 采购记录外协采购待领料明细表 where 采购单号='{0}'", textBox5.Text);
            using (SqlDataAdapter da = new SqlDataAdapter(sql_MX, strconn))
            {

                da.Fill(dt);
            }
            int i = 0;
            foreach (DataRow r in dt.Rows)
            {
                if (Convert.ToDecimal(r["未领数量"]) > 0)
                {
                    i++;
                }
            }
            if (i == 0)
            {
                string sql = string.Format("select * from  采购记录外协采购待领料主表 where 采购单号='{0}' ", textBox5.Text);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {

                    da.Fill(dt_1);
                    if (dt_1.Rows.Count > 0)
                    {
                        dt_1.Rows[0]["完成"] = true;
                        dt_1.Rows[0]["完成日期"] = System.DateTime.Now;
                    }

                    //new SqlCommandBuilder(da);
                    //da.Update(dt_1);

                }
            }
            return (dt_1);

        }
        
        private DataTable fun_save出入库明细()
        {
            int POS = 0;
            DataTable dt = new DataTable();
            foreach (DataRow dr in dt_右.Rows)
            {
                if (dr["选择"].Equals(true))
                {
                    string sql = "select * from 仓库出入库明细表 where 1<>1";
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                    {

                        da.Fill(dt);
                        DataRow r = dt.NewRow();
                        r["GUID"] = System.Guid.NewGuid();
                        r["明细类型"] = "领料";
                        r["单号"] = textBox1.Text;
                        r["出库入库"] = "出库";
                        r["物料编码"] = dr["物料编码"];
                        r["物料名称"] = dr["物料名称"];

                        r["明细号"] = textBox1.Text + POS.ToString("00");

                        r["实效数量"] = -(Convert.ToDecimal(dr["输入领料数量"]));
                        r["实效时间"] = System.DateTime.Now;
                        r["出入库时间"] = System.DateTime.Now;

                        string sql_pd = "select * from 仓库物料盘点表 where 有效=1";
                        using (SqlDataAdapter da1 = new SqlDataAdapter(sql_pd, strconn))
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
                        //new SqlCommandBuilder(da);
                        //da.Update(dt);
                    }
                }
                POS++;
            }
            return (dt);
        }

        private void fun_save_zf()
        {

            foreach (DataRow dr in dt_右.Rows)
            {
                if (dr["选择"].Equals(true))
                {
                    StockCore.StockCorer.fun_物料数量_实际数量(dr["物料编码"].ToString().Trim(),dr["仓库号"].ToString(), true);
                }
            }
        }
        #endregion

        #region 界面操作
        //生效
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridView2.CloseEditor();
            this.BindingContext[dt_右].EndCurrentEdit();

            try
            {
                fun_check();
                if (MessageBox.Show("确认生效？", "确认", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    foreach (DataRow dr in dt_右.Rows)
                    {
                        if (dr["选择"].Equals(true))
                        {
                            string str_物料编码 = dr["物料编码"].ToString();
                            decimal dec = Convert.ToDecimal(dr["输入领料数量"]);
                           

                            string str_采购单号 = textBox5.Text;
                            StockCore.StockCorer.fun_出入库_外协领料出库(str_物料编码, dec, str_采购单号);
                        }

                    }
                    DataSet ds_1 = fun_save();


                    DataTable dt = fun_save出入库明细();
                   
                    DataTable dt_2 = fun_完成状态();
                    string sql_领料主表 = "select * from 采购记录外协采购领料主表 where 1<>1";
                    string sql_领料明细表 = "select * from 采购记录外协采购领料明细表 where 1<>1";
                    string sql_出入库明细 = "select * from 仓库出入库明细表 where 1<>1";
                    
                    string sql_完成状态 = "select * from  采购记录外协采购待领料主表 where 1<>1";
                    SqlConnection conn = new SqlConnection(strconn);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("领料");

                    try
                    {
                        SqlCommand cmm_1 = new SqlCommand(sql_领料主表, conn, ts);
                        SqlCommand cmm_2 = new SqlCommand(sql_领料明细表, conn, ts);
                        SqlCommand cmm_3 = new SqlCommand(sql_出入库明细, conn, ts);
                        
                        SqlCommand cmm_6 = new SqlCommand(sql_完成状态, conn, ts);
                        SqlDataAdapter da_领料主表 = new SqlDataAdapter(cmm_1);
                        SqlDataAdapter da_领料明细表 = new SqlDataAdapter(cmm_2);
                        SqlDataAdapter da_出入库明细 = new SqlDataAdapter(cmm_3);
                       
                        SqlDataAdapter da_完成状态 = new SqlDataAdapter(cmm_6);

                        new SqlCommandBuilder(da_领料主表);
                        new SqlCommandBuilder(da_领料明细表);
                        new SqlCommandBuilder(da_出入库明细);
                       
                        new SqlCommandBuilder(da_完成状态);

                        da_领料主表.Update(ds_1.Tables[0]);
                        da_领料明细表.Update(ds_1.Tables[1]);
                        da_出入库明细.Update(dt);
                       
                        if (dt_2 != null)
                            da_完成状态.Update(dt_2);
                        ts.Commit();

                    }
                    catch (Exception ex)
                    {
                        ts.Rollback();

                        throw new Exception("领料生效失败");
                    }
                    // stockcore 中函数


                    fun_save_zf();
                    MessageBox.Show("生效成功");

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
            textBox1.Text = "";
            DataRow dr = dt_左.NewRow();
            dataBindHelper1.DataFormDR(dr);
            fun_load();
        }
         //关闭
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        #endregion
    }
}
