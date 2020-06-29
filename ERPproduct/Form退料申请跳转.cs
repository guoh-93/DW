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
    public partial class Form退料申请跳转 : Form
    {

        #region 变量
        string strcon = CPublic.Var.strConn;
        DataTable dtM;
        DataTable dt_下拉;
        DataTable dt_生产;
        DataRow drM;
        DataRow drrr_带;
        string str数据类型;
        DataTable dt_带;
        public int 次数;


        public DataTable dt_返回;
        int 复状态 = 0;
        // string str_待退料号_传 = "";
        string str_车间 = "";
        string str_关闭原因 = "";
        #endregion

        public Form退料申请跳转()
        {
            InitializeComponent();
        }

        public Form退料申请跳转(string str_工单号, int 状态, DataRow drm, string str_数据类型)
        {
            InitializeComponent();
            textBox2.Text = str_工单号;
            复状态 = 状态;
            drrr_带 = drm;
            str数据类型 = str_数据类型;
            fun_search();
        }
        //工单关闭退料
        public Form退料申请跳转(string str_工单号, int 状态, DataRow drm, string str_数据类型, DataTable dt, string str_原因)
        {
            dt_带 = dt.Copy();
            if (dt_带.Rows.Count > 0)
            {
                if (dt_带.Rows[0]["生产工单号"].ToString() != str_工单号.ToString())
                {
                    次数 = 0;

                }
                else
                {
                    次数 = 1;///次数为1 同一条数据是二次运行
                }
            }
            InitializeComponent();
            textBox2.Text = str_工单号;
            复状态 = 状态;
            drrr_带 = drm;
            str数据类型 = str_数据类型;
            str_关闭原因 = str_原因;
            fun_search();
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                gridView1.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                fun_check();
                fun_save();
                MessageBox.Show("生效成功");
                if (复状态 == 1)
                {
                    复状态 = 0;
                    barLargeButtonItem2_ItemClick(null, null);
                }
                textBox2.Text = "";
                //  fun_load()(null, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #region

#pragma warning disable IDE1006 // 命名样式
        private void fun_search()
#pragma warning restore IDE1006 // 命名样式
        {
            //只退已经领过的 
            string sql_1 = string.Format("select * from 生产记录生产工单待领料主表  where 生产工单号='{0}' ", textBox2.Text);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);


            dataBindHelper1.DataFormDR(dt.Rows[0]);
            textBox6.Text = CPublic.Var.localUserName;


            if (复状态 == 1)
            {
                string sql2 = @"select a.*,b.有效总数,b.库存总数 from 生产记录生产工单待领料明细表  a
                            left join 仓库物料数量表 b
                              on  b.物料编码=  a.物料编码  where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql2, strcon))
                {
                    dtM = new DataTable();
                    da.Fill(dtM);
                    dtM.Columns.Add("选择", typeof(bool));
                    dtM.Columns.Add("输入退料数量",typeof(decimal));
                    dtM.Columns.Add("领料类型", typeof(string));

                    // dtM.Columns.Add("计量单位");
                    gridControl1.DataSource = dtM;
                    dt_返回 = new DataTable();
                    dt_返回 = dtM.Clone();
                }
                decimal 剩余数量 = Convert.ToDecimal(drrr_带["生产数量"].ToString()) - Convert.ToDecimal(drrr_带["部分完工数"].ToString());
                if (剩余数量 <= 0)
                {
                    剩余数量 = 0;
                }
                string sql = string.Format("select * from 生产记录生产工单待领料明细表 where 生产工单号='{0}' and 已领数量>0", textBox2.Text);
                using (SqlDataAdapter da2 = new SqlDataAdapter(sql, strcon))
                {
                    DataTable dt_需求 = new DataTable();
                    da2.Fill(dt_需求);
                    // gridControl1.DataSource = dt_需求;
                    foreach (DataRow dr in dt_需求.Rows)//总数据
                    {
                        DataRow drrp = dtM.NewRow();
                        dtM.Rows.Add(drrp);
                        drrp["生产工单号"] = dr["生产工单号"];
                        drrp["领料类型"] = dr["wiptype"];

                        drrp["已领数量"] = dr["已领数量"];
                        drrp["未领数量"] = dr["未领数量"];
                        drrp["生产车间"] = dr["生产车间"].ToString();
                        drrp["BOM数量"] = dr["BOM数量"];
                        //decimal a = Convert.ToDecimal(dr["已领数量"]);
                        drrp["输入退料数量"] = dr["已领数量"].ToString();
                        drrp["待领料总量"] = dr["已领数量"];
                        drrp["物料编码"] = dr["物料编码"];
                        drrp["物料名称"] = dr["物料名称"];
                        drrp["仓库号"] = dr["仓库号"].ToString();
                        drrp["仓库名称"] = dr["仓库名称"].ToString();
                        drrp["计量单位"] = dr["计量单位"].ToString();
                        drrp["规格型号"] = dr["规格型号"].ToString();
                        //20-1-14 可以直接从待领料明细表中取仓库号，仓库名称，计量单位，规格型号
                        //string sql_物料 = string.Format("select 物料编码,物料名称,计量单位,规格型号,车间编号,仓库号,仓库名称 from 基础数据物料信息表 where 物料编码='{0}'", dr["物料编码"]);
                        //using (SqlDataAdapter 大 = new SqlDataAdapter(sql_物料, strcon))
                        //{
                        //    DataTable dt_物料 = new DataTable();
                        //    大.Fill(dt_物料);
                        //    DataRow drp = dt_物料.Rows[0];
                        //    drrp["仓库号"] = drp["仓库号"].ToString();
                        //    drrp["仓库名称"] = drp["仓库名称"].ToString();
                        //    drrp["计量单位"] = drp["计量单位"].ToString();
                        //    drrp["规格型号"] = drp["规格型号"].ToString();
                        //}
                        string sql_库存 = string.Format("select * from 仓库物料数量表 where 物料编码='{0}' and 仓库号='{1}'", dr["物料编码"], dr["仓库号"]);
                        using (SqlDataAdapter 大 = new SqlDataAdapter(sql_库存, strcon))
                        {
                            DataTable dt_库存 = new DataTable();
                            大.Fill(dt_库存);
                            if (dt_库存.Rows.Count == 0)
                            {
                                drrp["库存总数"] = 0;
                            }
                            else
                            {
                                DataRow drp = dt_库存.Rows[0];
                                drrp["库存总数"] = drp["库存总数"].ToString();
                            }
                        }
                        drrp["生产工单号"] = textBox2.Text.ToString();
                    }
                }
                //DataView dv = dtM.DefaultView;
                //dv.RowFilter = "已领数量 > 0";
                gridControl1.DataSource = dtM;
                if (次数 == 1)
                {
                    foreach (DataRow dr in dt_带.Rows)
                    {
                        foreach (DataRow drr in dtM.Rows)
                        {
                            if (dr["生产工单号"].ToString() == drr["生产工单号"].ToString() && dr["物料编码"].ToString() == drr["物料编码"].ToString())
                            {
                                drr["选择"] = true;
                            }
                        }
                    }
                }
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {
            DateTime t = CPublic.Var.getDatetime().Date;
            dt_生产 = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);
            if (dt_生产.Rows.Count > 0)
            {
                string sql = string.Format(@"select 生产工单号,base.规格型号,生产数量,gd.物料编码,base.物料名称,计量单位,base.车间
                from 生产记录生产工单表 gd left join 基础数据物料信息表 base on gd.物料编码= base.物料编码
                where gd.生效日期>'{0}' and  生产车间='{1}' and 完工=0", t.AddMonths(-9), dt_生产.Rows[0]["生产车间"]);

                dt_下拉 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                //repositoryItemSearchLookUpEdit1.DataSource = dt_下拉;
                //repositoryItemSearchLookUpEdit1.DisplayMember = "生产工单号";
                //repositoryItemSearchLookUpEdit1.ValueMember = "生产工单号";
                str_车间 = dt_下拉.Rows[0]["车间"].ToString();
            }
            else
            {
                throw new Exception("未识别你是哪个车间,请检查");
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_save()
#pragma warning restore IDE1006 // 命名样式
        {

            //Waiting for return order material
            DateTime t = CPublic.Var.getDatetime();
            string str_待退料号 = string.Format("WR{0}{1:00}{2:0000}",
                                               t.Year, t.Month, CPublic.CNo.fun_得到最大流水号("WR", t.Year, t.Month));

            // 1.生成退料申请单 
            string sql = "select * from 工单退料申请表 where 1<>1";
            DataTable dt_主 = new DataTable();
            dt_主 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            //  dt_主.Columns.Add("退料类型");
            string sql_mx = "select * from 工单退料申请明细表 where 1<>1";
            DataTable dt_mx = new DataTable();
            dt_mx = CZMaster.MasterSQL.Get_DataTable(sql_mx, strcon);
            //  dt_mx.Columns.Add("仓库号");
            // dt_mx.Columns.Add("仓库名称");
            DataRow r_m = dt_主.NewRow();
            dt_主.Rows.Add(r_m);
            r_m["待退料号"] = str_待退料号;
            r_m["生产工单号"] = textBox2.Text;
            r_m["车间"] = str_车间;
            r_m["产品编号"] = drrr_带["物料编码"];
            r_m["产品名称"] = drrr_带["物料名称"];
            r_m["操作人"] = CPublic.Var.localUserName;
            r_m["操作人ID"] = CPublic.Var.LocalUserID;
            r_m["操作时间"] = t;
            r_m["备注"] = "工单关闭";
            r_m["退料类型"] = "工单关闭退料";
            //  r_m["类型"] = str数据类型;
            int i = 0;
            foreach (DataRow dr in dtM.Rows)
            {
                if (dr["选择"].Equals(true))
                {
                    i++;
                    DataRow r_mx = dt_mx.NewRow();

                    r_mx["待退料号"] = str_待退料号;
                    r_mx["待退料明细号"] = str_待退料号 + "-" + i.ToString("00");
                    r_mx["POS"] = i;
                    r_mx["物料编码"] = dr["物料编码"];
                    r_mx["物料名称"] = dr["物料名称"];
                    r_mx["仓库号"] = dr["仓库号"];
                    //if (复状态 != 1)
                    //{
                    r_mx["仓库名称"] = dr["仓库名称"];
                    r_mx["需退料数量"] = dr["输入退料数量"];
                    dt_mx.Rows.Add(r_mx);
                    //}
                }
            }


            //string sql_工单号 = string.Format("select * from 生产记录生产工单表 where 生产工单号={0}", textBox2.Text.ToString());
            //DataTable dt_工单号 = new DataTable();
            //dt_工单号 = CZMaster.MasterSQL.Get_DataTable(sql_工单号, strcon);
            //foreach (DataRow dr in dt_工单号.Rows)
            //{
            //    dr["备注2"] = str数据类型;

            //}

            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction mt = conn.BeginTransaction("工单退料申请");
            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn, mt);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt_主);

                if (复状态 == 1)
                {
                    string sql_检验 = string.Format("select * FROM 生产记录生产工单表 WHERE 生产工单号='{0}'and 生产制令单号='{1}'and 物料编码='{2}'  ", drrr_带["生产工单号"].ToString(), drrr_带["生产制令单号"].ToString(), drrr_带["物料编码"].ToString());
                    DataTable dt_检验 = new DataTable();
                    using (SqlDataAdapter da检验 = new SqlDataAdapter(sql_检验, strcon))
                    {
                        dt_检验 = new DataTable();
                        da检验.Fill(dt_检验);
                    }

                    DataRow dr = dt_检验.Rows[0];
                    dr["状态"] = true;
                    dr["备注2"] = str数据类型;
                    dr["备注3"] = str_关闭原因;
                    string sql_检 = "select * from 生产记录生产工单表 where 1<>1 ";
                    cmd = new SqlCommand(sql_检, conn, mt);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_检验); textBox2.Text.ToString();


                    //DataTable dt_申请 = ERPorg.Corg.fun_PA("关闭", "工单关闭", textBox2.Text.ToString(), "");
                    //string 单据审核申请表 = "select * from 单据审核申请表 where 1<>1 ";


                    //cmd = new SqlCommand(单据审核申请表, conn, mt);
                    //da = new SqlDataAdapter(cmd);
                    //new SqlCommandBuilder(da);
                    //da.Update(dt_申请);
                }


                cmd = new SqlCommand(sql_mx, conn, mt);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt_mx);
                mt.Commit();
            }
            catch (Exception ex)
            {
                mt.Rollback();
                throw new Exception("退料申请失败" + ex.Message);
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {

            DataView dv = new DataView(dtM);
            dv.RowFilter = "选择=1";
            if (dv.Count == 0)
            {
                throw new Exception("未选择需退料的物料");
            }
            foreach (DataRow r in dtM.Rows)
            {
                if (r["选择"].Equals(true))
                {
                    decimal a = 0;
                    try
                    {
                        a = Convert.ToDecimal(r["输入退料数量"]);

                    }
                    catch
                    {
                        throw new Exception("请正确输入退料数量格式");

                    }
                    if (a <= 0)
                    {
                        throw new Exception("退料数量不能小于0,请重新输入");

                    }
                    if (a > Convert.ToDecimal(r["待领料总量"]))
                    {
                        throw new Exception("输入的退料数量大于领料数量");
                    }
                    if (r["仓库号"].ToString() =="")
                    {
                        throw new Exception("没有仓库号不能退库");
                    }
                }
            }


        }

        #endregion

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            this.Close();
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

        private void Form退料申请跳转_Load(object sender, EventArgs e)
        {
            try
            {
                fun_load();
                if (textBox2.Text != null && 复状态 != 1)
                {
                    fun_search();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (e.Column.FieldName == "选择")
                {
                    if (Boolean.Parse(e.Value.ToString()) == true)
                    {
                        DataRow dr = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
                        dr["生产工单号"] = textBox2.Text.ToString();
                        dt_返回.Rows.Add(dr.ItemArray);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox8.Text.Trim() == "") throw new Exception("未输入齐套数");
                decimal dec = 0;
                if(!decimal.TryParse(textBox8.Text.Trim(),out dec))
                {
                    throw new Exception("齐套数输入不正确,请确认");
                }
                if(dtM==null || dtM.Rows.Count==0) throw new Exception("没有明细请确认");

                foreach(DataRow dr in dtM.Rows)
                {
                    dr["选择"] = true;
                    dr["输入退料数量"] =  dec*Convert.ToDecimal(dr["BOM数量"]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void textBox8_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            { e.Handled = true; }
        }
    }
}
