using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace StockCore
{
    public partial class UIRemakeIn : UserControl
    {

        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dtM;
        DataTable dtP;
        DataRow drM = null;
        DataTable dt_物料;
        DataTable dt_人员;
        DataTable dt_代办;
        string sql_ck = "";
        frm其他出库 fm = new frm其他出库();

        #endregion

        public UIRemakeIn()
        {
            InitializeComponent();
        }


        #region 方法
#pragma warning disable IDE1006 // 命名样式
        private void fun_判断出库申请()
#pragma warning restore IDE1006 // 命名样式
        {
            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                DataRow[] ds = dt_出库申请.Select(string.Format("出入库申请明细号 = '{0}'", dr["出入库申请明细号"]));
                if (dr["数量确认"].ToString().ToLower() == "true")
                {
                    ds[0]["完成"] = true;
                    ds[0]["完成日期"] = CPublic.Var.getDatetime();
                }
                //if (ds[0]["已入库数量"] == null) ds[0]["已入库数量"] = 0;
                //ds[0]["已入库数量"] = Convert.ToDecimal(ds[0]["已入库数量"]) + Convert.ToDecimal(dr["实际数量"]);
            }
            int count = 0;
            foreach (DataRow dr in dt_出库申请.Rows)
            {
                if (dr["完成"].ToString().ToLower() == "true")
                {
                    count = count + 1;
                }
            }
            DataRow dr_申请 = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);
            string sql_check = string.Format(@"select count(*)总数,count(完成)已完成 from  返修出入库申请子表 where 出入库申请单号 ='{0}'",
                dr_申请["出入库申请单号"]);//因为数据库中完成 没有默认值 为NULl
            DataRow dr_check = CZMaster.MasterSQL.Get_DataRow(sql_check, strconn);
            int i = Convert.ToInt32(dr_check["总数"]) - Convert.ToInt32(dr_check["已完成"]);

            if (count == i)
            {
                dr_出库申请["完成"] = true;
                dr_出库申请["完成日期"] = CPublic.Var.getDatetime();
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void dtP_ColumnChanged(object sender, DataColumnChangeEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (e.Column.Caption == "物料编码")
                {
                    DataRow[] ds = dt_物料.Select(string.Format("物料编码 = '{0}'", e.Row["物料编码"]));
                    e.Row["原ERP物料编号"] = ds[0]["原ERP物料编号"];
                    e.Row["物料名称"] = ds[0]["物料名称"];
                    e.Row["n原ERP规格型号"] = ds[0]["n原ERP规格型号"];
                    //e.Row["图纸编号"] = ds[0]["图纸编号"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_载入主表明细()
#pragma warning restore IDE1006 // 命名样式
        {
            if (drM == null)
            {
                string sql = "select * from 返修入库主表 where 1<>1";
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);

                drM = dtM.NewRow();
                dtM.Rows.Add(drM);

                sql = "select * from 返修入库子表 where 1<>1";
                dtP = new DataTable();
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtP);
                dtP.Columns.Add("库存总数");
                dtP.Columns.Add("货架描述");
            }
            else
            {
                string sql = string.Format("select * from 返修入库主表 where 返修入库单号 = '{0}'", drM["返修入库单号"].ToString());
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);

                drM = dtM.Rows[0];
                dataBindHelper1.DataFormDR(drM);

                txt_入库人员ID.EditValue = drM["入库人员编号"];
                txt_入库人员.Text = drM["入库人员"].ToString();

                string sql2 = string.Format(@"select 返修入库子表.*,返修仓库物料数量表.库存总数 from 返修入库子表 left join 返修仓库物料数量表 on 返修入库子表.物料编码 = 返修仓库物料数量表.物料编码
                where 返修入库单号 = '{0}'", drM["返修入库单号"].ToString());
                dtP = new DataTable();
                SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
                da2.Fill(dtP);
            }
            dtP.Columns.Add("数量确认", typeof(bool));
          dtP.ColumnChanged += dtP_ColumnChanged;
        }

        DataRow dr_出库申请 = null;

        DataTable dt_出库申请 = null;
#pragma warning disable IDE1006 // 命名样式
        public DataTable fun_库存(int i_正负, DataTable T)
#pragma warning restore IDE1006 // 命名样式
        {
            DataTable dt = new DataTable();
            foreach (DataRow dr in T.Rows)
            {
                if (dr["数量确认"].Equals(true))
                {
                    string sql = string.Format("select * from 返修仓库物料数量表 where 物料编码='{0}'", dr["物料编码"].ToString());
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                    {
                        da.Fill(dt);
                    }
                    if (dt.Rows.Count > 0)
                    {
                        DataRow[] x = dt.Select(string.Format("物料编码='{0}'", dr["物料编码"].ToString()));
                        x[0]["库存总数"] = Convert.ToDecimal(x[0]["库存总数"]) + i_正负 * Convert.ToDecimal(dr["数量"].ToString());
                    }
                    else
                    {
                        DataRow r = dt.NewRow();
                        r["GUID"] = new Guid();
                        r["物料编码"] = dr["物料编码"];
                        r["物料名称"] = dr["物料名称"];
                        r["规格型号"] = dr["n原ERP规格型号"];
                        r["图纸编号"] = dr["图纸编号"];
                        r["库存总数"] = Convert.ToDecimal(dr["数量"].ToString());
                        r["有效总数"] = Convert.ToDecimal(dr["数量"].ToString());
                        r["物料名称"] = dr["物料名称"];
                        dt.Rows.Add(r);
                    }
                }

            }

            return dt;
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_保存主表明细(Boolean bl)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();
                if (drM["GUID"].ToString() == "")
                {
                    drM["GUID"] = System.Guid.NewGuid();
                    txt_入库单号.Text = string.Format("RMI{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                        t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("RMI", t.Year, t.Month).ToString("0000"));
                    drM["返修入库单号"] = txt_入库单号.Text;
                    drM["创建日期"] = CPublic.Var.getDatetime();
                }
                drM["操作人员编号"] = CPublic.Var.LocalUserID;
                drM["操作人员"] = CPublic.Var.localUserName;
                drM["入库仓库"] = "";
                drM["入库人员编号"] = txt_入库人员ID.EditValue;
                drM["入库人员"] = txt_入库人员.Text;
                if (bl == true)
                {
                    drM["生效"] = true;
                    drM["生效人员编号"] = CPublic.Var.LocalUserID;
                    drM["生效日期"] = CPublic.Var.getDatetime();
                }
                dataBindHelper1.DataToDR(drM);
            }
            catch (Exception ex)
            {
                throw new Exception("主表保存出错" + ex.Message);
            }

            try
            {
                int i = 1;
                foreach (DataRow r in dtP.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    if (r["GUID"].ToString() == "")
                    {
                        r["GUID"] = System.Guid.NewGuid();
                        r["返修入库单号"] = drM["返修入库单号"];
                        r["返修入库明细号"] = drM["返修入库单号"].ToString() + i.ToString("00");
                        r["POS"] = i++;
                    }
                    if (bl == true)
                    {
                        r["生效"] = true;
                        r["生效人员编号"] = CPublic.Var.LocalUserID;
                        r["生效日期"] = CPublic.Var.getDatetime();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("明细保存出错" + ex.Message);
            }
            fun_判断出库申请();
            //仓库出入库记录中存一条记录
            DataTable dt_出入库 = fun_保存记录到出入库明细();

            DataTable dt_库存 =  fun_库存(1, dtP);

            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("生效");
            string sql1 = "select * from 返修入库主表 where 1<>1";
            SqlCommand cmd1 = new SqlCommand(sql1, conn, ts);
            SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
            new SqlCommandBuilder(da1);
            string sql2 = "select * from 返修入库子表 where 1<>1";
            SqlCommand cmd2 = new SqlCommand(sql2, conn, ts);
            SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
            new SqlCommandBuilder(da2);
            string sql3 = "select * from 返修出入库申请主表 where 1<>1";
            SqlCommand cmd3 = new SqlCommand(sql3, conn, ts);
            SqlDataAdapter da3 = new SqlDataAdapter(cmd3);
            new SqlCommandBuilder(da3);
            string sql4 = "select * from 返修出入库申请子表 where 1<>1";
            SqlCommand cmd4 = new SqlCommand(sql4, conn, ts);
            SqlDataAdapter da4 = new SqlDataAdapter(cmd4);
            new SqlCommandBuilder(da4);
            string sql5 = "select * from 返修仓库物料数量表 where 1<>1";
            SqlCommand cmd5 = new SqlCommand(sql5, conn, ts);
            SqlDataAdapter da5 = new SqlDataAdapter(cmd5);
            new SqlCommandBuilder(da5);
            string sql6 = "select * from 返修仓库出入库明细表 where 1<>1";
            SqlCommand cmd6 = new SqlCommand(sql6, conn, ts);
            SqlDataAdapter da6 = new SqlDataAdapter(cmd6);
            new SqlCommandBuilder(da6);
            try
            {
                da1.Update(dtM);
                da2.Update(dtP);
                da3.Update(dt_代办);
                da4.Update(dt_出库申请);
                da5.Update(dt_库存);
                da6.Update(dt_出入库);

                ts.Commit();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw ex;
            }
        }
         
#pragma warning disable IDE1006 // 命名样式
        private void fun_物料下拉框()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = @"select 基础数据物料信息表.物料编码,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.物料名称,基础数据物料信息表.n原ERP规格型号,
            基础数据物料信息表.图纸编号,返修仓库物料数量表.库存总数 from 基础数据物料信息表 
            left join 返修仓库物料数量表 on 基础数据物料信息表.物料编码 = 返修仓库物料数量表.物料编码";
            dt_物料 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_物料);

            repositoryItemSearchLookUpEdit1.DataSource = dt_物料;
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_载入代办()
#pragma warning restore IDE1006 // 命名样式
        {
            //sql_ck = string.Format("select * from 人员仓库对应表 where 工号='{0}'", CPublic.Var.LocalUserID);
            //DataTable dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_ck, strconn);
            //sql_ck = "and 基础数据物料信息表.仓库号  in(";
            //string sql_左 = "";
            string sql = "";
            //if (dt_仓库.Rows.Count == 0 && CPublic.Var.LocalUserID == "admin")
            //{
                sql = "select * from 返修出入库申请主表 where 生效 = 1 and (完成=0 or 完成 is null) and (作废 = 0 or 作废 is null) and 申请类型 = '返修入库'";

//            }
//            else
//            {
//                foreach (DataRow dr in dt_仓库.Rows)
//                {
//                    sql_ck = sql_ck + string.Format("'{0}',", dr["仓库号"]);

//                }
//                sql_ck = sql_ck.Substring(0, sql_ck.Length - 1) + ")";
//                sql = string.Format(@"select 返修出入库申请主表.* from 返修出入库申请主表 where 生效 = 1 and (完成=0 or 完成 is null)
//                            and (作废 = 0 or 作废 is null) and 申请类型 = '返修入库' and 出入库申请单号 in
//                        ( select 出入库申请单号  from 返修出入库申请子表,基础数据物料信息表 where 完成 = 0  and     
//                         基础数据物料信息表.物料编码=返修出入库申请子表.物料编码 {0} group by 出入库申请单号 )  ", sql_ck);

//            }
            dt_代办 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_代办);
            gc_代办.DataSource = dt_代办;
        }

#pragma warning disable IDE1006 // 命名样式
        private DataTable fun_保存记录到出入库明细()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = "select * from 返修仓库出入库明细表 where 1<>1";
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                foreach (DataRow r in dtP.Rows)
                {
                    if (r["数量确认"].ToString().ToLower() == "true")
                    {
                        DataRow dr = dt.NewRow();
                        dr["GUID"] = System.Guid.NewGuid();
                        dr["明细类型"] = "返修入库";
                        dr["单号"] = r["返修入库单号"].ToString();
                        dr["物料编码"] = r["物料编码"].ToString();
                        dr["物料名称"] = r["物料名称"].ToString();
                        dr["明细号"] = r["返修入库明细号"].ToString();
                        dr["出库入库"] = "入库";

                        dr["相关单位"] = "未来电器";
                        dr["数量"] = (Decimal)0;
                        dr["标准数量"] = (Decimal)0;
                        dr["实效数量"] = Convert.ToDecimal(r["数量"].ToString());
                        dr["实效时间"] = CPublic.Var.getDatetime();
                        dr["出入库时间"] = CPublic.Var.getDatetime();
                        string sql_pd = string.Format(@"select 仓库物料盘点表.盘点批次号 from [仓库物料盘点表] left join [仓库物料盘点明细表] 
                                                    on 仓库物料盘点表.盘点批次号 = [仓库物料盘点明细表].盘点批次号 
                                                    where [仓库物料盘点表].有效 = 0 and [仓库物料盘点明细表].物料编码 = '{0}'", r["物料编码"].ToString().Trim());
                        using (SqlDataAdapter da1 = new SqlDataAdapter(sql_pd, strconn))
                        {
                            DataTable dt_批次号 = new DataTable();
                            da1.Fill(dt_批次号);
                            if (dt_批次号.Rows.Count > 0)
                            {
                                dr["盘点有效批次号"] = dt_批次号.Rows[0]["盘点批次号"];
                            }
                            else
                            {
                                dr["盘点有效批次号"] = "初始化";
                            }
                        }
                        dt.Rows.Add(dr);
                    }
                }
                //new SqlCommandBuilder(da);
                //da.Update(dt);
                return dt;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm返修入库_fun_保存出入库明细");
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_人员()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format(@"select 员工号,姓名 from 人事基础员工表 where 在职状态 = '在职'");
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            dt_人员 = new DataTable();
            da.Fill(dt_人员);
            txt_入库人员ID.Properties.DataSource = dt_人员;
            txt_入库人员ID.Properties.DisplayMember = "员工号";
            txt_入库人员ID.Properties.ValueMember = "员工号";
        }

#pragma warning disable IDE1006 // 命名样式
        private void txt_入库人员ID_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (txt_入库人员ID.EditValue != null && txt_入库人员ID.EditValue.ToString() == "")
            {

                txt_入库人员.Text = "";
            }
            else
            {
                DataRow[] ds = dt_人员.Select(string.Format("员工号 = '{0}'", txt_入库人员ID.EditValue));
                if (ds.Length > 0)
                {
                    txt_入库人员.Text = ds[0]["姓名"].ToString();
                }
                else
                {
                    txt_入库人员.Text = "";
                }
            }
        }
        #endregion

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            drM = null;
            UIRemakeIn_Load(null, null);
        }

        private void UIRemakeIn_Load(object sender, EventArgs e)
        {
            try
            {
                fun_物料下拉框();
                time_入库日期.EditValue = System.DateTime.Now;
                fun_人员();
                fun_载入代办();
                fun_载入主表明细();
                gc.DataSource = dtP;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //生效
            try
            {
                gv.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();

                for (int i = 0; i < dtP.Rows.Count; i++)
                {
                    if (dtP.Rows[i]["数量确认"].Equals(true))
                    {
                        continue;
                    }
                    else
                    {
                        dtP.Rows.Remove(dtP.Rows[i]);
                        i--;
                    }

                }
                if (dtP.Rows.Count == 0)
                {
                    gv_代办_RowCellClick(null, null);
                    throw new Exception("未选择明细");
                }
                //必须数量确认
                foreach (DataRow dr in dtP.Rows)
                {
                    if (dr["数量确认"].ToString().ToLower() == "true")
                    {
                        continue;
                    }
                    else
                    {
                        throw new Exception("请先确认_" + dr["物料编码"].ToString() + "_数量");
                    }
                }

                fun_保存主表明细(true);  //并判断出入库申请是否完成

              
                MessageBox.Show("生效成功");
                drM = null;
                UIRemakeIn_Load(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();

        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_代办_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                dr_出库申请 = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);
                if (dr_出库申请 == null) return;
                txt_入库人员ID.EditValue = dr_出库申请["操作人员编号"].ToString();
                drM["出入库申请单号"] = dr_出库申请["出入库申请单号"];
                txt_备注.Text = "";
                txt_入库仓库.Text = "";
                txt_入库单号.Text = "";

                dtP.Clear();
                string sql = string.Format(@"select 返修出入库申请子表.*,库存总数,货架描述 from 返修出入库申请子表,返修仓库物料数量表,基础数据物料信息表
                        where 返修出入库申请子表.物料编码=返修仓库物料数量表.物料编码 and 返修出入库申请子表.物料编码=基础数据物料信息表.物料编码
                        and (返修出入库申请子表.完成 is null or 返修出入库申请子表.完成=0) and  出入库申请单号 = '{0}' {1}", dr_出库申请["出入库申请单号"], sql_ck);
                dt_出库申请 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_出库申请);
                foreach (DataRow r in dt_出库申请.Rows)
                {

                    DataRow rr = dtP.NewRow();
                    rr["物料编码"] = r["物料编码"];
                    rr["物料名称"] = r["物料名称"];
                    rr["n原ERP规格型号"] = r["n原ERP规格型号"];
                    rr["数量"] = r["数量"];
                    rr["库存总数"] = r["库存总数"];
                    rr["货架描述"] = r["货架描述"];
                    rr["原ERP物料编号"] = r["原ERP物料编号"];
                    rr["出入库申请单号"] = r["出入库申请单号"];
                    rr["出入库申请明细号"] = r["出入库申请明细号"];
                    rr["备注"] = r["备注"];
                    //if (r["已入库数量"].ToString() == "") r["已入库数量"] = 0;
                    //rr["实际数量"] = Convert.ToDecimal(r["数量"]) - Convert.ToDecimal(r["已入库数量"]);
                    //rr["可入库数量"] = Convert.ToDecimal(r["数量"]) - Convert.ToDecimal(r["已入库数量"]);
                    dtP.Rows.Add(rr);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void button1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            foreach (DataRow dr in dtP.Rows)
            {
                dr["数量确认"] = true;

            }
        }
    }
}
