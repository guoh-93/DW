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
    public partial class fm_当前物品归还代还流程 : Form
    {
        DataTable dt_借还申请表附表;
        DataTable dt_基础数据物料信息表;
        DataTable dt_借还申请表;
        DataTable dt_借还申请表归还记录;
        DataTable dt_仓库物料数量表;
        CurrencyManager cmM;
        public fm_当前物品归还代还流程()
        {
            InitializeComponent();
        }

        private void fm_当前物品归还代还流程_Load(object sender, EventArgs e)
        {
            //dt_借还申请表附表 = new DataTable();
            //string sql = "select * from 借还申请表归还记录 where 申请批号 = '" + MoldMangement.frm归还流程界面.s_申请批号 + "'";
            //fun_GetDataTable(dt_借还申请表附表, sql);
            //dtM.DataSource = dt_借还申请表附表;
            //dt_借还申请表附表.Columns.Add("本次归还数量");

            //decimal s_金额 = 0;
            //decimal s_总金额 = 0;
            //foreach (DataRow dr in dt_借还申请表附表.Rows)
            //{
            //    s_金额 = Convert.ToDecimal(dr["归还金额"]);
            //    s_总金额 += s_金额;
            //}
            //textBox2.Text = s_总金额.ToString();
            //dt_借还申请表 = new DataTable();
            //string sql3 = "select * from 借还申请表 where 申请批号 = '" + MoldMangement.frm归还流程界面.s_申请批号 + "'";
            //fun_GetDataTable(dt_借还申请表, sql3);
            //foreach (DataRow dr in dt_借还申请表.Rows)
            //{
            //    textBox1.Text = dr["总金额"].ToString();
            //}
            //dt_基础数据物料信息表 = new DataTable();
            //string sql2 = "select 物料编码,原ERP物料编号,物料名称,图纸编号,标准单价,n核算单价,n原ERP规格型号 from 基础数据物料信息表";
            //fun_GetDataTable(dt_基础数据物料信息表, sql2);
            //dt_借还申请表归还记录 = new DataTable();
            //string sql4 = "select * from 借还申请表归还记录 where 1<>1";
            //fun_GetDataTable(dt_借还申请表归还记录, sql4);
            //cmM = BindingContext[dt_借还申请表附表] as CurrencyManager;
            //time_申请日期.EditValue = System.DateTime.Now;
            //if (dt_借还申请表附表.Rows.Count == 0)
            //{
            //    foreach (DataRow dr in MoldMangement.frm借还流程.dt_借还申请表附表.Rows)
            //    {
            //        foreach (DataRow dr2 in dt_基础数据物料信息表.Rows)
            //        {
            //            if (dr["物料编码"].ToString() == dr2["物料编码"].ToString())
            //            {

            //                DataRow dr3 = dt_借还申请表附表.NewRow();
            //                dr3["申请批号"] = MoldMangement.frm借还流程.s_申请批号;
            //                dr3["物料编码"] = dr["物料编码"];
            //                dr3["物料名称"] = dr["物料名称"];
            //                dr3["n原ERP规格型号"] = dr["n原ERP规格型号"];
            //                dr3["原ERP物料编号"] = dr["原ERP物料编号"];
            //                dr3["仓库名称"] = dr["仓库名称"];
            //                dr3["货架描述"] = dr["货架描述"];
            //                dr3["物料单价"] = dr2["n核算单价"];
            //                dr3["归还数量"] = "0";
            //                dr3["归还金额"] = "0";
            //                dt_借还申请表附表.Rows.Add(dr3);
            //            }
            //        }
            //    }
            //}
            //foreach (DataRow dr in dt_借还申请表附表.Rows)
            //{
            //    if (dr["归还数量"].ToString() == "")
            //    {
            //        dr["本次归还数量"] = "0";
            //    }
            //    else
            //    {
            //        dr["本次归还数量"] = dr["归还数量"];
            //    }
            //}

        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                time_申请日期.EditValue = System.DateTime.Now;
                DataRow dr = dt_借还申请表附表.NewRow();
                dr["申请批号"] = MoldMangement.frm借还流程.s_申请批号;
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
            try
            {
                decimal s_借出总金额 = Convert.ToDecimal(textBox1.Text);
                decimal s_借出总金额下差 = s_借出总金额 - (s_借出总金额 * Convert.ToDecimal(0.05));
                decimal s_归还总金额 = Convert.ToDecimal(textBox2.Text);
                if (s_归还总金额 > s_借出总金额下差)
                {
                    if (MessageBox.Show("此申请单可以结束，是否要结束！" + "\n借出总金额：" + s_借出总金额 + "\n归还总金额：" + s_归还总金额, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        dt_仓库物料数量表 = new DataTable();
                        foreach (DataRow dr in dt_借还申请表附表.Rows)
                        {
                            string s_物料编码 = dr["物料编码"].ToString();
                            string sql3 = "select * from 仓库物料数量表 where 物料编码 = '" + s_物料编码 + "'";
                            fun_GetDataTable(dt_仓库物料数量表, sql3);
                        }
                        foreach (DataRow dr in dt_仓库物料数量表.Rows)
                        {
                            foreach (DataRow dr2 in dt_借还申请表附表.Rows)
                            {
                                if (dr["物料编码"].ToString() == dr2["物料编码"].ToString())
                                {
                                    decimal s_库存总数 = Convert.ToDecimal(dr["库存总数"]);
                                    decimal s_有效总数 = Convert.ToDecimal(dr["有效总数"]);
                                    decimal s_归还数量 = Convert.ToDecimal(dr2["归还数量"]) - Convert.ToDecimal(dr2["本次归还数量"]);
                                    decimal s_差额 = s_库存总数 + s_归还数量;
                                    decimal s_差额2 = s_有效总数 + s_归还数量;
                                    dr["库存总数"] = s_差额.ToString();
                                    dr["有效总数"] = s_差额2.ToString();
                                }
                            }
                        }
                        string sql4 = "select * from 仓库物料数量表 where 1<>1";
                        fun_SetDataTable(dt_仓库物料数量表, sql4);
                        int i = 1;
                        foreach (DataRow dr in dt_借还申请表附表.Rows)
                        {
                            if (dr.RowState == DataRowState.Deleted)
                            {
                                continue;
                            }
                            dr["申请批号明细"] = MoldMangement.frm借还流程.s_申请批号 + "-" + i;
                            dr["归还日期"] = time_申请日期.Text;
                            dr["借还状态"] = "归还";
                            i++;
                        }
                        string sql = "select * from 借还申请表归还记录 where 1<>1";
                        fun_SetDataTable(dt_借还申请表附表, sql);
                        foreach (DataRow dr in dt_借还申请表.Rows)
                        {
                            dr["选择"] = true;
                            dr["借还状态"] = "已归还";
                            dr["结束日期"] = time_申请日期.Text;
                        }
                        string sql2 = "select * from 借还申请表 where 1<>1";
                        fun_SetDataTable(dt_借还申请表, sql2);
                        MessageBox.Show("保存成功！");
                        this.Close();
                    }
                }
                else
                {
                    dt_仓库物料数量表 = new DataTable();
                    foreach (DataRow dr in dt_借还申请表附表.Rows)
                    {
                        string s_物料编码 = dr["物料编码"].ToString();
                        string sql3 = "select * from 仓库物料数量表 where 物料编码 = '" + s_物料编码 + "'";
                        fun_GetDataTable(dt_仓库物料数量表, sql3);
                    }
                    foreach (DataRow dr in dt_仓库物料数量表.Rows)
                    {
                        foreach (DataRow dr2 in dt_借还申请表附表.Rows)
                        {
                            if (dr["物料编码"].ToString() == dr2["物料编码"].ToString())
                            {
                                decimal s_库存总数 = Convert.ToDecimal(dr["库存总数"]);
                                decimal s_有效总数 = Convert.ToDecimal(dr["有效总数"]);
                                decimal s_归还数量 = Convert.ToDecimal(dr2["归还数量"]) - Convert.ToDecimal(dr2["本次归还数量"]);
                                decimal s_差额 = s_库存总数 + s_归还数量;
                                decimal s_差额2 = s_有效总数 + s_归还数量;
                                dr["库存总数"] = s_差额.ToString();
                                dr["有效总数"] = s_差额2.ToString();
                            }
                        }
                    }
                    string sql4 = "select * from 仓库物料数量表 where 1<>1";
                    fun_SetDataTable(dt_仓库物料数量表, sql4);
                    int i = 1;
                    foreach (DataRow dr in dt_借还申请表附表.Rows)
                    {
                        if (dr.RowState == DataRowState.Deleted)
                        {
                            continue;
                        }
                        dr["申请批号明细"] = MoldMangement.frm借还流程.s_申请批号 + "-" + i;
                        dr["归还日期"] = time_申请日期.Text;
                        dr["借还状态"] = "归还";
                        i++;
                    }
                    string sql = "select * from 借还申请表归还记录 where 1<>1";
                    fun_SetDataTable(dt_借还申请表附表, sql);
                    string sql2 = "select * from 借还申请表 where 1<>1";
                    fun_SetDataTable(dt_借还申请表, sql2);
                    MessageBox.Show("保存成功！\n该单号尚未完成，请继续归还相应物品！");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

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

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
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
    }
}
