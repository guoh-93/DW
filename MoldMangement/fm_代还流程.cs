using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace MoldMangement
{
    public partial class fm_代还流程 : Form
    {
        #region 用户变量
        DataTable dt_借还申请表附表;
        DataTable dt_基础数据物料信息表;
        DataTable dt_借还申请表;
        DataTable dt_借还申请表归还记录;
        DataTable dt_仓库物料数量表;
        DataTable dt_借还申请批量归还关联;
        DataTable dt;
        string s_批号;
         string sql_ck = "";
         string strconn = CPublic.Var.strConn;
        CurrencyManager cmM;
        #endregion                                                     

        #region 类自用

        public fm_代还流程()
        {
            InitializeComponent();
        }

        private void fm_代还流程_Load(object sender, EventArgs e)
        {
//            try
//            {
//                s_批号 = string.Format("GH{0}{1}{2}{3}", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString("00"),
//                       DateTime.Now.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("GH", DateTime.Now.Year, DateTime.Now.Month).ToString("0000"));
//                dt_借还申请表附表 = new DataTable();
//                sql_ck = "借还申请批量归还关联.归还批号 in(";
//                sql_ck = sql_ck + string.Format("'{0}',", MoldMangement.frm归还流程界面.dr_当前行["申请批号"]);
//                sql_ck = sql_ck.Substring(0, sql_ck.Length - 1) + ")";
//                string sql = @"select 借还申请表归还记录.* from 借还申请表归还记录 where 申请批号 in 
//                            (select  distinct 借还申请批量归还关联.关联批号 from 借还申请批量归还关联 where " + sql_ck + ")";
//                fun_GetDataTable(dt_借还申请表附表, sql);
//                dtM.DataSource = dt_借还申请表附表;
//                dt_借还申请表附表.Columns.Add("本次归还数量");
//                foreach (DataRow dr in dt_借还申请表附表.Rows)
//                {
//                    if (dr["归还数量"].ToString() == "")
//                    {
//                        dr["本次归还数量"] = "0";
//                    }
//                    else
//                    {
//                        dr["本次归还数量"] = dr["归还数量"];
//                    }
//                }
//                decimal s_金额 = 0;
//                decimal s_总金额 = 0;
//                foreach (DataRow dr in dt_借还申请表附表.Rows)
//                {
//                    s_金额 = Convert.ToDecimal(dr["归还金额"]);
//                    s_总金额 += s_金额;
//                }
//                textBox2.Text = s_总金额.ToString();
//                textBox1.Text = MoldMangement.frm归还流程界面.dr_当前行["总金额"].ToString();
//                dt_借还申请表 = new DataTable();
//                string sql6 = "select * from 借还申请表";
//                fun_GetDataTable(dt_借还申请表, sql6);
//                dt_借还申请批量归还关联 = new DataTable();
//                string sql5 = "select * from 借还申请批量归还关联 where 1<>1";
//                fun_GetDataTable(dt_借还申请批量归还关联, sql5);
//                dt_基础数据物料信息表 = new DataTable();
//                string sql2 = "select 物料编码,原ERP物料编号,物料名称,图纸编号,标准单价,n核算单价,n原ERP规格型号 from 基础数据物料信息表";
//                fun_GetDataTable(dt_基础数据物料信息表, sql2);
//                dt_借还申请表归还记录 = new DataTable();
//                string sql4 = "select * from 借还申请表归还记录 where 1<>1";
//                fun_GetDataTable(dt_借还申请表归还记录, sql4);
//                string sql7 = "select * from 仓库出入库明细表 where 1<>1";
//                dt = new DataTable();
//                SqlDataAdapter da = new SqlDataAdapter(sql7, strconn);
//                da.Fill(dt);
//                repositoryItemSearchLookUpEdit1.DataSource = dt_基础数据物料信息表;
//                repositoryItemSearchLookUpEdit1.DisplayMember = "原ERP物料编号";
//                repositoryItemSearchLookUpEdit1.ValueMember = "原ERP物料编号";
//                cmM = BindingContext[dt_借还申请表附表] as CurrencyManager;
//            }
//            catch (Exception ex )
//            {
//                MessageBox.Show(ex.Message);
//            }
            
        }

        #endregion

        #region 数据处理
        private void fun_GetDataTable(DataTable dt, string sql)
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                da.Fill(dt);
            }
        }

        private void fun_SetDataTable(DataTable dt, string sql)
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                new SqlCommandBuilder(da);
                da.Update(dt);
            }
        }
        #endregion

        #region 界面操作      
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                time_申请日期.EditValue = System.DateTime.Now;
                DataRow dr = dt_借还申请表附表.NewRow();
                dr["申请批号"] = s_批号;
                dt_借还申请表附表.Rows.Add(dr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            cmM.EndCurrentEdit();
            gridView1.CloseEditor();
            try
            {
                (cmM.Current as DataRowView).Row.Delete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //try
            //{
            //    time_申请日期.EditValue = System.DateTime.Now;
            //    decimal s_借出总金额 = Convert.ToDecimal(textBox1.Text);
            //    decimal s_借出总金额下差 = s_借出总金额 - Convert.ToDecimal(10);
            //    decimal s_归还总金额 = Convert.ToDecimal(textBox2.Text);
            //    if (s_归还总金额 > s_借出总金额下差)
            //    {
            //        if (MessageBox.Show("此申请单可以结束，是否要结束！" + "\n借出总金额：" + s_借出总金额 + "\n归还总金额：" + s_归还总金额, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            //        {

            //            dt_仓库物料数量表 = new DataTable();
            //            foreach (DataRow dr in dt_借还申请表附表.Rows)
            //            {
            //                string s_物料编码 = dr["物料编码"].ToString();
            //                string sql3 = "select * from 仓库物料数量表 where 物料编码 = '" + s_物料编码 + "'";
            //                fun_GetDataTable(dt_仓库物料数量表, sql3);
            //            }
            //            foreach (DataRow dr in dt_仓库物料数量表.Rows)
            //            {
            //                foreach (DataRow dr2 in dt_借还申请表附表.Rows)
            //                {
            //                    if (dr["物料编码"].ToString() == dr2["物料编码"].ToString())
            //                    {
            //                        decimal s_库存总数 = Convert.ToDecimal(dr["库存总数"]);
            //                        decimal s_有效总数 = Convert.ToDecimal(dr["有效总数"]);
            //                        decimal s_归还数量 = Convert.ToDecimal(dr2["归还数量"]) - Convert.ToDecimal(dr2["本次归还数量"]);
            //                        decimal s_差额 = s_库存总数 + s_归还数量;
            //                        decimal s_差额2 = s_有效总数 + s_归还数量;
            //                        dr["库存总数"] = s_差额.ToString();
            //                        dr["有效总数"] = s_差额2.ToString();
            //                    }
            //                }
            //            }             
            //            int i = 1;
            //            foreach (DataRow dr in dt_借还申请表附表.Rows)
            //            {
            //                if (dr.RowState == DataRowState.Deleted)
            //                {
            //                    continue;
            //                }
            //                dr["申请批号明细"] = s_批号 + "-" + i;
            //                dr["归还日期"] = time_申请日期.Text;
            //                dr["借还状态"] = "归还";
            //                i++;
            //            }
            //            foreach (DataRow dr2 in dt_借还申请表.Rows)
            //            {
            //                if (MoldMangement.frm归还流程界面.dr_当前行["申请批号"].ToString() == dr2["申请批号"].ToString())
            //                {
            //                    dr2["选择"] = true;
            //                    dr2["借还状态"] = "已归还";
            //                    dr2["结束日期"] = time_申请日期.Text;
            //                }
            //            }
            //            DataRow dr3 = dt_借还申请批量归还关联.NewRow();
            //            dr3["关联批号"] = s_批号;
            //           // dr3["归还批号"] = MoldMangement.frm归还流程界面.dr_当前行["申请批号"];
            //            dt_借还申请批量归还关联.Rows.Add(dr3);
            //            fun_保存记录到出入库明细();
            //            fun_SaveData(dt_借还申请表附表, dt_借还申请表, dt_仓库物料数量表, dt_借还申请批量归还关联,dt); 
            //            MessageBox.Show("保存成功！");
            //            this.Close();
            //        }
            //    }
            //    else
            //    {
            //        dt_仓库物料数量表 = new DataTable();
            //        foreach (DataRow dr in dt_借还申请表附表.Rows)
            //        {
            //            string s_物料编码 = dr["物料编码"].ToString();
            //            string sql3 = "select * from 仓库物料数量表 where 物料编码 = '" + s_物料编码 + "'";
            //            fun_GetDataTable(dt_仓库物料数量表, sql3);
            //        }
            //        foreach (DataRow dr in dt_仓库物料数量表.Rows)
            //        {
            //            foreach (DataRow dr2 in dt_借还申请表附表.Rows)
            //            {
            //                if (dr["物料编码"].ToString() == dr2["物料编码"].ToString())
            //                {
            //                    decimal s_库存总数 = Convert.ToDecimal(dr["库存总数"]);
            //                    decimal s_有效总数 = Convert.ToDecimal(dr["有效总数"]);
            //                    decimal s_归还数量 = Convert.ToDecimal(dr2["归还数量"]) - Convert.ToDecimal(dr2["本次归还数量"]);
            //                    decimal s_差额 = s_库存总数 + s_归还数量;
            //                    decimal s_差额2 = s_有效总数 + s_归还数量;
            //                    dr["库存总数"] = s_差额.ToString();
            //                    dr["有效总数"] = s_差额2.ToString();
            //                }
            //            }
            //        }
            //        int i = 1;
            //        foreach (DataRow dr in dt_借还申请表附表.Rows)
            //        {
            //            if (dr.RowState == DataRowState.Deleted)
            //            {
            //                continue;
            //            }
            //            dr["申请批号明细"] = s_批号 + "-" + i;
            //            dr["归还日期"] = time_申请日期.Text;
            //            dr["借还状态"] = "归还";
            //            i++;
            //        }
            //        DataRow dr3 = dt_借还申请批量归还关联.NewRow();
            //        dr3["关联批号"] = s_批号;
            //        dr3["归还批号"] = MoldMangement.frm归还流程界面.dr_当前行["申请批号"];
            //        dt_借还申请批量归还关联.Rows.Add(dr3);
            //        fun_SaveData(dt_借还申请表附表, dt_借还申请表, dt_仓库物料数量表, dt_借还申请批量归还关联,dt);
            //        MessageBox.Show("保存成功！\n该单号尚未完成，请继续归还相应物品！");
            //        this.Close();
            //    }           
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private static void fun_SaveData(System.Data.DataTable dt1, System.Data.DataTable dt2, System.Data.DataTable dt3, System.Data.DataTable dt4, System.Data.DataTable dt5)
        {
            using (SqlConnection conn = new SqlConnection(CPublic.Var.strConn))
            {
                SqlTransaction transaction = null;
                try
                {
                    ///文件断层使用的数据表
                    System.Data.DataTable dt_MID1 = new System.Data.DataTable();
                    System.Data.DataTable dt_MID2 = new System.Data.DataTable();
                    System.Data.DataTable dt_MID3 = new System.Data.DataTable();
                    System.Data.DataTable dt_MID4 = new System.Data.DataTable();
                    System.Data.DataTable dt_MID5 = new System.Data.DataTable();
                    string sql;

                    conn.Open();
                    transaction = conn.BeginTransaction("CFileVibrationTransaction");

                    sql = "select * from 借还申请表归还记录 where 1<>1 ";
                    SqlDataAdapter da_MID1 = new SqlDataAdapter(new SqlCommand(sql, conn, transaction));
                    new SqlCommandBuilder(da_MID1);
                    da_MID1.Fill(dt_MID1);

                    sql = "select * from 借还申请表 where 1<>1 ";
                    SqlDataAdapter da_MID2 = new SqlDataAdapter(new SqlCommand(sql, conn, transaction));
                    new SqlCommandBuilder(da_MID2);
                    da_MID2.Fill(dt_MID2);

                    sql = "select * from 仓库物料数量表 where 1<>1 ";
                    SqlDataAdapter da_MID3 = new SqlDataAdapter(new SqlCommand(sql, conn, transaction));
                    new SqlCommandBuilder(da_MID3);
                    da_MID3.Fill(dt_MID3);

                    sql = "select * from 借还申请批量归还关联 where 1<>1 ";
                    SqlDataAdapter da_MID4 = new SqlDataAdapter(new SqlCommand(sql, conn, transaction));
                    new SqlCommandBuilder(da_MID4);
                    da_MID3.Fill(dt_MID4);

                    sql = "select * from 仓库出入库明细表 where 1<>1";
                    SqlDataAdapter da_MID5 = new SqlDataAdapter(new SqlCommand(sql, conn, transaction));
                    new SqlCommandBuilder(da_MID5);
                    da_MID3.Fill(dt_MID5);


                    dt_MID1 = dt1;
                    dt_MID2 = dt2;
                    dt_MID3 = dt3;
                    dt_MID4 = dt4;
                    dt_MID5 = dt5;
                    try
                    {
                        da_MID1.Update(dt_MID1);
                        da_MID2.Update(dt_MID2);
                        da_MID3.Update(dt_MID3);
                        da_MID4.Update(dt_MID4);
                        da_MID5.Update(dt_MID5);
                        transaction.Commit();

                        dt_MID1.Clear();
                        dt_MID2.Clear();
                        dt_MID3.Clear();
                        dt_MID4.Clear();
                        dt_MID5.Clear();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void fun_保存记录到出入库明细()
        {
            try
            {
                
                int i = 1;
                foreach (DataRow r in dt_借还申请表附表.Rows)
                {
                    //if (r["数量确认"].ToString().ToLower() == "true")
                    //{
                    DataRow dr = dt.NewRow();
                    dr["GUID"] = System.Guid.NewGuid();
                    dr["明细类型"] = "借还入库";
                    dr["单号"] = r["申请批号"].ToString();
                    dr["物料编码"] = r["物料编码"].ToString();
                    dr["物料名称"] = r["物料名称"].ToString();
                    dr["明细号"] = s_批号 + "-" + i;
                    dr["出库入库"] = "入库";

                    dr["相关单位"] = "未来电器";
                    dr["数量"] = (Decimal)0;
                    dr["标准数量"] = (Decimal)0;
                    dr["实效数量"] = Convert.ToDecimal(r["归还数量"].ToString());
                    dr["实效时间"] = System.DateTime.Now;
                    dr["出入库时间"] = System.DateTime.Now;
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
                    //}
                    i++;
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm其他入库_fun_保存出入库明细");
                throw ex;
            }
        }


        private void gridView1_CellValueChanged_1(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.Caption == "原ERP物料编号")
            {
                try
                {
                    DataRow myDataRow = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                    string s_原ERP物料编码 = myDataRow["原ERP物料编号"].ToString();
                    string sql = "select 物料编码,原ERP物料编号,物料名称,图纸编号,标准单价,n核算单价,n原ERP规格型号,仓库名称,货架描述 from 基础数据物料信息表 where 原ERP物料编号= '" + s_原ERP物料编码 + "'";
                    fun_GetDataTable(dt_基础数据物料信息表, sql);
                    foreach (DataRow dr in dt_基础数据物料信息表.Rows)
                    {
                        myDataRow["物料编码"] = dr["物料编码"];
                        myDataRow["物料名称"] = dr["物料名称"];
                        myDataRow["n原ERP规格型号"] = dr["n原ERP规格型号"];
                        myDataRow["物料单价"] = dr["n核算单价"];
                        myDataRow["仓库名称"] = dr["仓库名称"];
                        myDataRow["货架描述"] = dr["货架描述"];
                    }
                    foreach (DataRow dr in dt_借还申请表附表.Rows)
                    {
                        if (dr["归还数量"].ToString() == "")
                        {
                            dr["本次归还数量"] = "0";
                        }
                        else
                        {
                            dr["本次归还数量"] = dr["归还数量"];
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            if (e.Column.Caption == "归还数量")
            {
                try
                {
                    DataRow myDataRow = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                    decimal s_单价金额 = Convert.ToDecimal(myDataRow["物料单价"]);
                    decimal s_借用数量 = Convert.ToDecimal(myDataRow["归还数量"]);
                    decimal s_总金额 = s_单价金额 * s_借用数量;
                    decimal s_所有物品总金额 = 0;
                    myDataRow["归还金额"] = s_总金额;
                    foreach (DataRow dr in dt_借还申请表附表.Rows)
                    {
                        if (dr["归还金额"].ToString() != "")
                        {
                            s_所有物品总金额 += Convert.ToDecimal(dr["归还金额"]);

                        }
                    }
                    textBox2.Text = s_所有物品总金额.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        } 
        #endregion
    }
}
