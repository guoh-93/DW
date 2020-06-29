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
    public partial class UI工单退料申请 : UserControl
    {

        #region 变量
        string strcon = CPublic.Var.strConn;
        DataTable dtM, dt_mx, dt_主;
        DataTable dt_下拉;
        DataTable dt_生产;
        DataRow drM;
        DataRow drrr_带;
        int 复状态 = 0;
        // string str_待退料号_传 = "";
        string str_车间 = "";

        #endregion

        public UI工单退料申请()
        {
            InitializeComponent();
        }
        //public UI工单退料申请(string str)
        //{
        //    InitializeComponent();
        //    str_待退料号_传 = str;
        //    barEditItem1.EditValue = str_待退料号_传; 
        //}


        public UI工单退料申请(string str_工单号, int 状态, DataRow drm)
        {
            InitializeComponent();

            barEditItem1.EditValue = str_工单号;
            复状态 = 状态;
            drrr_带 = drm;

            barLargeButtonItem1_ItemClick(null, null);




        }


        public UI工单退料申请(DataRow dt_tl, DataTable dt_sq)
        {
            InitializeComponent();

            dt_mx = dt_sq;     
                barLargeButtonItem1_ItemClick(null, null);




        }





        public UI工单退料申请(string str_工单号)
        {
            InitializeComponent();
            barEditItem1.EditValue = str_工单号;
        }
        private void search_check()
        {
            if (barEditItem1.EditValue == null || barEditItem1.EditValue.ToString() == "")
                throw new Exception("未选择工单");
        }
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {


                search_check();

                fun_search();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void UI工单退料申请_Load(object sender, EventArgs e)
        {
            try
            {
                fun_load();
                if (barEditItem1.EditValue != null && 复状态 != 1)
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
        private void fun_search()
#pragma warning restore IDE1006 // 命名样式
        {
            //只退已经领过的 
            string sql_1 = string.Format("select * from 生产记录生产工单待领料主表  where 生产工单号='{0}' ", barEditItem1.EditValue);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);
            if (dt.Rows.Count > 0)
            {
                dataBindHelper1.DataFormDR(dt.Rows[0]);
                textBox6.Text = CPublic.Var.localUserName;
                if (barEditItem1.EditValue != null && barEditItem1.EditValue.ToString() != "" && 复状态 != 1)
                {
                    if (dt.Rows.Count > 0)
                    {
                        string sql = string.Format(@"select a.*,人事基础部门表.部门名称 from (select sdlmx.*,
                           kc.有效总数,kc.库存总数 from 生产记录生产工单待领料明细表 sdlmx
                           left join 仓库物料数量表 kc on   kc.物料编码= sdlmx.物料编码 
                           left join 基础数据物料信息表 base on base.物料编码=sdlmx.物料编码
                           where  sdlmx.仓库号=kc.仓库号 and sdlmx.生产工单号='{0}') a   
                           left join 人事基础部门表 on 人事基础部门表.部门编号 = a.生产车间   where 已领数量>0  ", barEditItem1.EditValue.ToString());

                        using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                        {
                            dtM = new DataTable();
                            da.Fill(dtM);
                            if (dtM.Rows.Count > 0)
                            {
                                dtM.Columns.Add("选择", typeof(bool));
                                dtM.Columns.Add("输入退料数量");

                                gridControl1.DataSource = dtM;

                                //foreach (DataRow dr in dtM.Rows)
                                //{
                                //    dr["选择"] = true;
                                //    dr["输入退料数量"] = dr["已领数量"];
                                //}
                            }
                            else
                            {
                                MessageBox.Show("该工单未发料或者已完工");
                            }


                        }
                    }
                    else
                    {
                        gridControl1.DataSource = dtM.Clone();
                        MessageBox.Show("请正确输入工单号");
                    }
                }

                if (barEditItem1.EditValue != null && barEditItem1.EditValue.ToString() == "")
                {
                    //根据领料单  退料
                    string sql2 = @"select *,仓库物料数量表.有效总数,仓库物料数量表.库存总数 from 生产记录生产工单待领料明细表 left join 仓库物料数量表
                                                          on   仓库物料数量表.物料编码=  生产记录生产工单待领料明细表.物料编码
                                                          where 1<>1";

                    using (SqlDataAdapter da = new SqlDataAdapter(sql2, strcon))
                    {
                        dtM = new DataTable();
                        da.Fill(dtM);
                        //if (dtM.Rows.Count > 0)
                        //{
                        dtM.Columns.Add("选择", typeof(bool));
                        dtM.Columns.Add("输入退料数量");


                        //}
                    }

                }

                if (复状态 == 1)
                {


                    string sql2 = @"select *,仓库物料数量表.有效总数,仓库物料数量表.库存总数 from 生产记录生产工单待领料明细表 left join 仓库物料数量表
                                                        on   仓库物料数量表.物料编码=  生产记录生产工单待领料明细表.物料编码
                                                         where 1<>1";



                    //  ;


                    using (SqlDataAdapter da = new SqlDataAdapter(sql2, strcon))
                    {
                        dtM = new DataTable();
                        da.Fill(dtM);

                        //if (dtM.Rows.Count > 0)
                        //{
                        dtM.Columns.Add("选择", typeof(bool));
                        dtM.Columns.Add("输入退料数量");
                        dtM.Columns.Add("计量单位");
                        // dtM.Columns.Add("库存总数");
                        //dtM.Columns.Add("计量单位");
                        //dtM.Columns.Add("规格型号");
                        gridControl1.DataSource = dtM;
                        //}
                    }

                    decimal 剩余数量 = Convert.ToDecimal(drrr_带["生产数量"].ToString()) - Convert.ToDecimal(drrr_带["部分完工数"].ToString());
                    if (剩余数量 <= 0)
                    {
                        剩余数量 = 0;
                    }
                    string sql = string.Format("select * from 基础数据物料BOM表 where 产品编码='{0}'", textBox3.Text.ToString());
                    using (SqlDataAdapter da2 = new SqlDataAdapter(sql, strcon))
                    {
                        DataTable dt_需求 = new DataTable();
                        da2.Fill(dt_需求);
                        // gridControl1.DataSource = dt_需求;
                        foreach (DataRow dr in dt_需求.Rows)
                        {
                            DataRow drrp = dtM.NewRow();
                            dtM.Rows.Add(drrp);

                            string sql_数据 = string.Format("select  b.BOM数量,b.已领数量,b.待领料总量,b.未领数量,b.物料名称,b.生产工单号   from  生产记录生产工单待领料主表 a left join 生产记录生产工单待领料明细表 b  on a.生产工单号=b.生产工单号 where b.物料编码='{0}' and b.生产工单号='{1}'", dr["子项编码"].ToString(), barEditItem1.EditValue.ToString());
                            using (SqlDataAdapter 大 = new SqlDataAdapter(sql_数据, strcon))
                            {
                                DataTable dt_数据 = new DataTable();
                                大.Fill(dt_数据);

                                DataRow drM = dt_数据.Rows[dt_数据.Rows.Count - 1];
                                drrp["待领料总量"] = drM["待领料总量"];
                                drrp["已领数量"] = drM["已领数量"];

                                drrp["未领数量"] = drM["未领数量"];
                                if (Convert.ToDecimal(drrp["未领数量"].ToString()) <= 0)
                                {
                                    drrp["未领数量"] = 0;
                                }
                                drrp["BOM数量"] = drM["BOM数量"];
                                decimal a = Convert.ToDecimal(drM["已领数量"]);
                                drrp["输入退料数量"] = (a - (Convert.ToDecimal(dr["数量"].ToString()) * Convert.ToDecimal(drrr_带["部分完工数"].ToString()))).ToString("0.00");
                                if (Convert.ToDecimal(drrp["输入退料数量"].ToString()) <= 0)
                                {
                                    drrp["输入退料数量"] = 0;
                                }
                            }
                            string sql_物料 = string.Format("select 物料编码,物料名称,计量单位,规格型号,车间编号,仓库号,仓库名称 from 基础数据物料信息表 where 物料编码='{0}'", dr["子项编码"]);
                            using (SqlDataAdapter da_x = new SqlDataAdapter(sql_物料, strcon))
                            {
                                DataTable dt_物料 = new DataTable();
                                da_x.Fill(dt_物料);
                                DataRow drp = dt_物料.Rows[0];
                                drrp["仓库号"] = drp["仓库号"].ToString();
                                drrp["仓库名称"] = drp["仓库名称"].ToString();
                                drrp["生产车间"] = drp["车间编号"].ToString();
                                drrp["计量单位"] = drp["计量单位"].ToString();
                                drrp["规格型号"] = drp["规格型号"].ToString();
                            }
                            string sql_库存 = string.Format("select * from 仓库物料数量表 where 物料编码='{0}'", dr["子项编码"]);
                            using (SqlDataAdapter da = new SqlDataAdapter(sql_库存, strcon))
                            {
                                DataTable dt_库存 = new DataTable();
                                da.Fill(dt_库存);
                                DataRow drp = dt_库存.Rows[0];
                                drrp["库存总数"] = drp["库存总数"].ToString();

                            }
                            drrp["物料编码"] = dr["子项编码"];
                            drrp["物料名称"] = dr["子项名称"];
                            //decimal x = Convert.ToDecimal(drrp["输入退料数量"].ToString());

                            //DataRow drr = dt_需求.NewRow();
                            //dt_需求.Rows.Add(drr);

                        }

                    }
                    DataView dv = dtM.DefaultView;
                    dv.RowFilter = "已领数量 > 0";
                    gridControl1.DataSource = dtM;
                }
            }
            else
            {
                dataBindHelper1.DataFormDR(dt.NewRow());
                gridControl1.DataSource = dtM.Clone();
                textBox4.Text = "";
                textBox6.Text = "";
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
                string sql = string.Format(@"select 生产工单号,base.规格型号,生产数量,gd.物料编码,base.物料名称,base.计量单位,base.车间
                from 生产记录生产工单表 gd left join 基础数据物料信息表 base on gd.物料编码= base.物料编码
                where gd.生效日期>'{0}' and  生产车间='{1}' and 完成=0", t.AddMonths(-9), dt_生产.Rows[0]["生产车间"]);
                dt_下拉 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                repositoryItemSearchLookUpEdit1.DataSource = dt_下拉;
                repositoryItemSearchLookUpEdit1.DisplayMember = "生产工单号";
                repositoryItemSearchLookUpEdit1.ValueMember = "生产工单号";
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
             dt_主 = new DataTable();
            dt_主 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            string sql_mx = "select * from 工单退料申请明细表 where 1<>1";
             dt_mx = new DataTable();
            dt_mx = CZMaster.MasterSQL.Get_DataTable(sql_mx, strcon);
            DataRow r_m = dt_主.NewRow();
            dt_主.Rows.Add(r_m);
            r_m["待退料号"] = str_待退料号;
            r_m["待退料号"] = str_待退料号;

            r_m["生产工单号"] = textBox2.Text;
            r_m["车间"] = str_车间;
            r_m["产品编号"] = textBox3.Text;
            r_m["产品名称"] = textBox7.Text;
            r_m["操作人"] = textBox6.Text;
            r_m["操作人ID"] = CPublic.Var.LocalUserID;
            r_m["操作时间"] = t;
            r_m["备注"] = textBox4.Text;
            r_m["退料类型"] = "工单退料";
            int i = 0;
            foreach (DataRow dr in dtM.Rows)
            {
                if (dr["选择"].Equals(true))
                {
                    i++;
                    DataRow r_mx = dt_mx.NewRow();

                    //20-5-7
                    r_mx["待退料号"] = str_待退料号;
                    r_mx["待领料明细号"] = dr["待领料单明细号"];
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

            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction mt = conn.BeginTransaction("工单退料申请");
            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn, mt);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt_主);

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

            string s = string.Format("select  * from  [工单退料申请表]  where 生产工单号='{0}' and 完成=0 and 作废=0 ", textBox2.Text);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            if (dt.Rows.Count > 0)
            {
                throw new Exception("该工单尚有未入库的退料申请,请通知仓库审核后再进行操作。");
                //if (MessageBox.Show("该工单尚有未入库的退料申请,请通知仓库审核后再进行操作。", "警告", MessageBoxButtons.OKCancel)!= DialogResult.OK)
                //{
                //    throw new Exception("已取消提交");
                //}
            }
            string ss = string.Format("select  * from  生产记录生产工单表 where 生产工单号='{0}' ", textBox2.Text);
            DataTable dtt = CZMaster.MasterSQL.Get_DataTable(ss, strcon);

            if (dtt.Rows.Count > 0)
            {
                if (Convert.ToBoolean(dtt.Rows[0]["关闭"]))
                {
                    throw new Exception("该工单已关闭，不可退料");
                }
                if (Convert.ToBoolean(dtt.Rows[0]["检验完成"]))
                {
                    throw new Exception("该工单已检验完成，不可退料");
                }
            }
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
                    if (a > Convert.ToDecimal(r["已领数量"]))
                    {
                        throw new Exception("输入的退料数量大于已领料数量");
                    }
                }
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
                }
                barEditItem1.EditValue = "";




                UI工单退料申请_Load(null, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show(string.Format("是否确认关闭该界面？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                CPublic.UIcontrol.ClosePage();
            }
        }
    }
}
