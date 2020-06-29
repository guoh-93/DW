
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using CZMaster;
using System.IO;
namespace ERPpurchase
{
    public partial class frm采购价格核实界面 : UserControl
    {
        bool flag = false;   //等于0 正常采购 入库   等于1 补开的 采购入库单
        string strcon = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dt_核价;
        DataTable dt_开票;
        public bool bl = false;
        public DataTable dt_已核价;

        public frm采购价格核实界面()
        {
            InitializeComponent();
        }

        string str_rukuDan = "";

        public frm采购价格核实界面(string strdh)
        {
            str_rukuDan = strdh;
            InitializeComponent();
        }
        public frm采购价格核实界面(DataTable dt_核价列表, DataTable dt_开票_cs)
        {
            dt_开票 = dt_开票_cs;
            dt_核价 = dt_核价列表;
            InitializeComponent();
        }
        public frm采购价格核实界面(DataTable dt_核价列表)
        {
            dt_核价 = dt_核价列表;
            InitializeComponent();

        }
        /// <summary>
        /// 入库明细：已经生效的，但是价格尚未核实的。
        /// </summary>
        DataTable dt_入库单明细;

        //载入需要核实单价的入库明细   
        //19-5-16 20:10 采购希望可以在这边直接改数量，然后反应到前面 需要备份一个dt出来保存 采购入库明细表 数量不改,但核价时单价金额税率会改  另一个是在界面上显示的
        private void fun_Load入库明细()
        {
            try
            {
                string sql = "";
                if (dt_核价 != null)
                {
                    dt_入库单明细 = dt_核价.Copy();
                    foreach(DataRow dr  in dt_入库单明细.Rows)
                    {
                        dr["入库量"] = Convert.ToDecimal(dr["入库量"]) - Convert.ToDecimal(dr["已开票量"]);
                        dr["金额"] =Math.Round(Convert.ToDecimal(dr["入库量"]) * Convert.ToDecimal(dr["单价"]),2,MidpointRounding.AwayFromZero);
                        dr["未税金额"] = Math.Round(Convert.ToDecimal(dr["入库量"]) * Convert.ToDecimal(dr["未税单价"]), 2, MidpointRounding.AwayFromZero);
                       // dr["税金"] = Convert.ToDecimal(dr["金额"]) - Convert.ToDecimal(dr["未税金额"]);
                    }
                }
                else
                {
                    throw new Exception("数据有误");
                }
                //else if (str_rukuDan == "")
                //{
                //    sql = @"select crmx.*  from 采购记录采购单入库明细 crmx
                //          left join  基础数据物料信息表  on 基础数据物料信息表.物料编码= crmx.物料编码
                //          where crmx.生效=1 and crmx.作废=0 and crmx.价格核实=0 and crmx.已开票量=0";
                //    dt_入库单明细 = MasterSQL.Get_DataTable(sql, strcon);
                //}
                //else
                //{
                //    sql = string.Format(@"select crmx.*  from 采购记录采购单入库明细 crmx
                //                      left join  基础数据物料信息表  on 基础数据物料信息表.物料编码= crmx.物料编码
                //                where crmx.入库明细号='{0}' and crmx.作废=0 ", str_rukuDan);
                //    dt_入库单明细 = MasterSQL.Get_DataTable(sql, strcon);
                //}
                //补的采购单 另一张表
                //if (dt_入库单明细.Rows.Count == 0)
                //{
                //    if (str_rukuDan == "")
                //    {
                //        sql = @"select crmx.*  from L采购记录采购单入库明细L  crmx
                //            left join  基础数据物料信息表  on 基础数据物料信息表.物料编码= crmx.物料编码
                //            where crmx.生效=1 and crmx.作废=0 and crmx.价格核实=0 and crmx.已开票量=0";
                //        dt_入库单明细 = MasterSQL.Get_DataTable(sql, strcon);
                //    }
                //    else
                //    {
                //        sql = string.Format(@"select crmx.* from L采购记录采购单入库明细L crmx
                //                      left join  基础数据物料信息表  on 基础数据物料信息表.物料编码= crmx.物料编码
                //                where crmx.入库明细号='{0}' and crmx.作废=0  ", str_rukuDan);
                //        dt_入库单明细 = MasterSQL.Get_DataTable(sql, strcon);
                //    }
                //    flag = true;
                //}
                if (!dt_入库单明细.Columns.Contains("税金"))
                    dt_入库单明细.Columns.Add("税金", typeof(decimal));
                foreach (DataRow dr in dt_入库单明细.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted) continue;
                    dr["税金"] = Convert.ToDecimal(dr["金额"]) - Convert.ToDecimal(dr["未税金额"]);
                }
                gridControl1.DataSource = dt_入库单明细;
                //gridView1.Columns["核实单价"].AppearanceCell.BackColor = Color.Aqua;



            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_Load入库明细");
                throw ex;
            }
        }
        //检查 税率 是否和 供应商基础信息里面的 税率一致
        private void fun_check(DataRow dr)
        {
            string sql = string.Format("select  * from 采购供应商表 where 供应商ID='{0}'", dr["供应商ID"].ToString());
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("该入库明细供应商在供应商基础信息里未找到,记下入库单号联系信息部");
                }
                else
                {
                    if (Convert.ToDecimal(dr["税率"]) != Convert.ToDecimal(dt.Rows[0]["税率"]))
                    {

                        throw new Exception("该入库明细税率与供应商基础信息里不一致,记下入库单号联系信息部");
                    }

                }

            }
        }
        private void frm采购价格核实界面_Load(object sender, EventArgs e)
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(this.panel2, this.Name, cfgfilepath);

                fun_Load入库明细();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void fun_核实单据价格()
        {
            try
            {
                //string sql = "";
                //DataTable dt_开票明细 = new DataTable() ;
                foreach (DataRow dr in dt_核价.Rows)
                {
                    DataRow[] r = dt_入库单明细.Select(string.Format("入库明细号 = '{0}'", dr["入库明细号"].ToString()));
                    if(r.Length>0) //已核价 需要更新单价税率金额   数量不改
                    {
                        dr["价格核实"] = true;

                        dr["单价"] = r[0]["单价"];
                        dr["税率"] = r[0]["税率"];
                        dr["未税单价"] = r[0]["未税单价"];
                        dr["未税金额"] =Math.Round(Convert.ToDecimal(dr["入库量"]) * Convert.ToDecimal(dr["未税单价"]),2,MidpointRounding.AwayFromZero);
                        dr["金额"] = Math.Round(Convert.ToDecimal(dr["入库量"]) * Convert.ToDecimal(dr["单价"]),2,MidpointRounding.AwayFromZero); ;
                        if (dt_开票 != null)
                        {
                            DataRow[] t = dt_开票.Select(string.Format("入库明细号 = '{0}' ", dr["入库明细号"].ToString()));
                            if (t.Length > 0)
                            {
                                t[0]["单价"] = r[0]["单价"];
                                t[0]["税率"] = r[0]["税率"];
                                t[0]["未税单价"] = r[0]["未税单价"];
                                t[0]["折扣后含税单价"] = r[0]["单价"];
                                t[0]["折扣后不含税单价"] = r[0]["未税单价"];
                                t[0]["开票数量"] = r[0]["入库量"];
                               // dr["已开票量"] = Convert.ToDecimal(dr["已开票量"]) + Convert.ToDecimal(r[0]["入库量"]);
                                t[0]["折扣后不含税金额"] =Math.Round(Convert.ToDecimal(r[0]["入库量"]) * Convert.ToDecimal(r[0]["未税单价"]),2,MidpointRounding.AwayFromZero);
                                t[0]["折扣后含税金额"] =Math.Round(Convert.ToDecimal(r[0]["入库量"]) * Convert.ToDecimal(r[0]["单价"]),2,MidpointRounding.AwayFromZero); ;
                                t[0]["金额"] = r[0]["金额"];
                                t[0]["未税金额"] = r[0]["未税金额"];
                                t[0]["税金"] = Convert.ToDecimal(r[0]["金额"]) - Convert.ToDecimal(r[0]["未税金额"]);
                                t[0]["价格核实"] = true;
                            }
                        }


                    }
                }
                //foreach (DataRow dr in dt_入库单明细.Rows)
                //{
                //    sql = string.Format("select *  from  采购记录采购开票通知单明细表 where 入库明细号='{0}'", dr["入库明细号"].ToString());
                //    dr["价格核实"] = true;

                //}
                DataTable dt_ss = fun_add(dt_核价.Rows[0]["供应商ID"].ToString(), dt_入库单明细);
                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction jghs = conn.BeginTransaction("价格核实"); //事务的名称
                try
                {
                    SqlDataAdapter da;
                    SqlCommand cmd = new SqlCommand("select * from 采购记录采购单入库明细 where 1<>1", conn, jghs);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_核价);
                    if (dt_开票 != null)
                    {
                        cmd = new SqlCommand("select * from 采购记录采购开票通知单明细表 where 1<>1", conn, jghs);
                        da = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da);
                        da.Update(dt_开票);
                    }
                    cmd = new SqlCommand("select * from 采购供应商物料单价表 where 1<>1", conn, jghs);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_ss);
                    jghs.Commit();

                }
                catch (Exception ex)
                {
                    jghs.Rollback();
                    throw new Exception(ex.Message);
                }
                //MasterSQL.Save_DataTable(dt_送检单明细, "采购记录采购送检单明细表", strcon);

                //MasterSQL.Save_DataTable(dt_检验单主表, "采购记录采购单检验主表", strcon);

                //MasterSQL.Save_DataTable(dt_入库明细, "采购记录采购单入库明细", strcon);
                //MasterSQL.Save_DataTable(dt_开票明细, "采购记录采购开票通知单明细表", strcon);

            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_核实单据价格");
                MessageBox.Show(ex.Message);
            }
        }



        //核实采购单的价格
        /// <summary>
        /// 19-5-12 弃用
        /// </summary>
        /// <param name="danjia"></param>
        private void fun_核实单据价格(decimal danjia)
        {
            try
            {
                DataRow r = (this.BindingContext[dt_入库单明细].Current as DataRowView).Row;
                string sql = "";
                DataTable dt_采购单主表 = new DataTable();
                DataTable dt_采购单明细 = new DataTable();
                DataTable dt_送检单明细;
                DataTable dt_检验单主表;
                DataTable dt_开票明细;

                DataTable dt_入库明细;
                sql = string.Format("select * from 采购记录采购送检单明细表 where 采购单明细号='{0}'", r["采购单明细号"].ToString());
                dt_送检单明细 = MasterSQL.Get_DataTable(sql, strcon);
                sql = string.Format("select * from 采购记录采购单检验主表 where 采购明细号='{0}'", r["采购单明细号"].ToString());
                dt_检验单主表 = MasterSQL.Get_DataTable(sql, strcon);
                sql = string.Format("select *  from 采购记录采购单入库明细 where 入库明细号='{0}'", r["入库明细号"].ToString());
                dt_入库明细 = MasterSQL.Get_DataTable(sql, strcon);
                sql = string.Format("select *  from  采购记录采购开票通知单明细表 where 入库明细号='{0}'", r["入库明细号"].ToString());
                dt_开票明细 = MasterSQL.Get_DataTable(sql, strcon);
                if (Convert.ToDecimal(r["单价"]) == danjia)
                {
                    //foreach (DataRow r1 in dt_送检单明细.Rows)
                    //{
                    //    r1["价格核实"] = true;
                    //}

                    //foreach (DataRow r1 in dt_检验单主表.Rows)
                    //{
                    //    r1["价格核实"] = true;
                    //}
                    dt_入库明细.Rows[0]["税率"] = r["税率"];
                    dt_入库明细.Rows[0]["单价"] = r["单价"];
                    dt_入库明细.Rows[0]["未税单价"] = r["未税单价"];
                    dt_入库明细.Rows[0]["金额"] = r["金额"];
                    dt_入库明细.Rows[0]["未税金额"] = r["未税金额"];
                    dt_入库明细.Rows[0]["价格核实"] = true;
                    //foreach (DataRow r1 in dt_入库明细.Rows)
                    //{


                    //    r1["价格核实"] = true;

                    //}
                    foreach (DataRow r1 in dt_开票明细.Rows)
                    {
                        r1["价格核实"] = true;
                    }
                }
                else
                {
                    /* 18-4-25 注释掉 
                    //修改采购单明细的单价
                    sql = string.Format("select * from 采购记录采购单明细表 where 采购单号='{0}'", r["采购单号"].ToString());
                    dt_采购单明细 = MasterSQL.Get_DataTable(sql, strcon);
                    DataRow[] dr = dt_采购单明细.Select(string.Format("采购明细号='{0}'", r["采购单明细号"].ToString()));
                    if (dr.Length > 0)
                    {
                        dr[0]["单价"] = danjia;
                        dr[0]["未税单价"] = (danjia / (1 + Convert.ToDecimal(r["税率"]) / 100)).ToString("0.000000");
                        dr[0]["金额"] = (danjia * Convert.ToDecimal(r["采购数量"])).ToString("0.000000");
                        dr[0]["未税金额"] = ((Convert.ToDecimal(dr[0]["金额"]) / (1 + Convert.ToDecimal(r["税率"]) / 100))).ToString("0.000000");
                        dr[0]["税金"] = Convert.ToDecimal(dr[0]["金额"]) - Convert.ToDecimal(dr[0]["未税金额"]);
                    }
                    //修改采购单主表的金额
                    decimal totalMoney = 0;
                    foreach (DataRow r2 in dt_采购单明细.Rows)
                    {
                        totalMoney = totalMoney + Convert.ToDecimal(r2["金额"]);
                    }
                    sql = string.Format("select * from 采购记录采购单主表 where 采购单号='{0}'", r["采购单号"].ToString());
                    dt_采购单主表 = MasterSQL.Get_DataTable(sql, strcon);
                    foreach (DataRow r2 in dt_采购单主表.Rows)
                    {
                        r2["总金额"] = totalMoney;
                        r2["未税金额"] = (totalMoney / (1 + Convert.ToDecimal(r["税率"]) / 100)).ToString("0.000000");
                        r2["税金"] = Convert.ToDecimal(r2["总金额"]) - Convert.ToDecimal(r2["未税金额"]);
                    }
                    //修改送检单的该明细
                 
                    foreach (DataRow r2 in dt_送检单明细.Rows)
                    {
                        r2["单价"] = danjia;
                        r2["未税单价"] = (danjia / (1 + Convert.ToDecimal(r["税率"]) / 100)).ToString("0.000000");
                        r2["金额"] = (danjia * Convert.ToDecimal(r2["采购数量"])).ToString("0.000000");
                        r2["未税金额"] = (Convert.ToDecimal(r2["金额"]) / (1 + Convert.ToDecimal(r["税率"]) / 100)).ToString("0.000000");
                        r2["价格核实"] = true;
                    }
                    //修改检验单的明细
                    
                    foreach (DataRow r2 in dt_检验单主表.Rows)
                    {
                        r2["单价"] = danjia;
                        r2["未税单价"] = (danjia / (1 + Convert.ToDecimal(r["税率"]) / 100)).ToString("0.000000");
                        r2["金额"] = (danjia * Convert.ToDecimal(r2["采购数量"])).ToString("0.000000");
                        r2["未税金额"] = (Convert.ToDecimal(r2["金额"]) / (1 + Convert.ToDecimal(r["税率"]) / 100)).ToString("0.000000");
                        r2["价格核实"] = true;
                    }
                    */
                    foreach (DataRow r2 in dt_开票明细.Rows)
                    {
                        r2["单价"] = danjia;
                        r2["未税单价"] = (danjia / (1 + Convert.ToDecimal(r["税率"]) / 100)).ToString("0.000000");
                        r2["金额"] = (danjia * Convert.ToDecimal(r2["采购数量"])).ToString("0.000000");
                        r2["未税金额"] = (Convert.ToDecimal(r2["金额"]) / (1 + Convert.ToDecimal(r["税率"]) / 100)).ToString("0.000000");
                        r2["价格核实"] = true;
                    }
                    //修改入库单的明细

                    dt_入库明细.Rows[0]["税率"] = r["税率"];
                    dt_入库明细.Rows[0]["单价"] = r["核实单价"];
                    dt_入库明细.Rows[0]["未税单价"] = (danjia / (1 + Convert.ToDecimal(r["税率"]) / 100)).ToString("0.000000");
                    dt_入库明细.Rows[0]["金额"] = r["金额"];
                    dt_入库明细.Rows[0]["未税金额"] = (Convert.ToDecimal(r["金额"]) / (1 + Convert.ToDecimal(r["税率"]) / 100)).ToString("0.000000");
                    dt_入库明细.Rows[0]["价格核实"] = true;
                    dt_入库明细.Rows[0]["单价备注"] = r["单价备注"];


                }

                DataTable dt_ss = fun_add(r["供应商ID"].ToString(), r["物料编码"].ToString(), danjia);

                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction jghs = conn.BeginTransaction("价格核实"); //事务的名称
                SqlCommand cmd = new SqlCommand("select * from 采购记录采购单主表 where 1<>1", conn, jghs);
                SqlCommand cmd1 = new SqlCommand("select * from 采购记录采购单明细表 where 1<>1", conn, jghs);
                try
                {
                    SqlDataAdapter da;
                    if (dt_采购单主表 != null)
                    {

                        da = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da);

                        da.Update(dt_采购单主表);
                        da = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da);
                        da.Update(dt_采购单明细);

                    }
                    SqlCommand cmd2 = new SqlCommand("select * from 采购记录采购送检单明细表 where 1<>1", conn, jghs);
                    da = new SqlDataAdapter(cmd2);
                    new SqlCommandBuilder(da);
                    da.Update(dt_送检单明细);
                    SqlCommand cmd3 = new SqlCommand("select * from 采购记录采购单检验主表 where 1<>1", conn, jghs);
                    da = new SqlDataAdapter(cmd3);
                    new SqlCommandBuilder(da);
                    da.Update(dt_检验单主表);
                    SqlCommand cmd4 = new SqlCommand("select * from 采购记录采购单入库明细 where 1<>1", conn, jghs);
                    da = new SqlDataAdapter(cmd4);
                    new SqlCommandBuilder(da);
                    da.Update(dt_入库明细);
                    SqlCommand cmd5 = new SqlCommand("select * from 采购记录采购开票通知单明细表 where 1<>1", conn, jghs);
                    da = new SqlDataAdapter(cmd5);
                    new SqlCommandBuilder(da);
                    da.Update(dt_开票明细);
                    SqlCommand cmd6 = new SqlCommand("select * from 采购供应商物料单价表 where 1<>1", conn, jghs);
                    da = new SqlDataAdapter(cmd6);
                    new SqlCommandBuilder(da);
                    da.Update(dt_ss);
                    jghs.Commit();
                }
                catch
                {
                    jghs.Rollback();
                }
                //MasterSQL.Save_DataTable(dt_送检单明细, "采购记录采购送检单明细表", strcon);

                //MasterSQL.Save_DataTable(dt_检验单主表, "采购记录采购单检验主表", strcon);

                //MasterSQL.Save_DataTable(dt_入库明细, "采购记录采购单入库明细", strcon);
                //MasterSQL.Save_DataTable(dt_开票明细, "采购记录采购开票通知单明细表", strcon);

            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_核实单据价格");
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 核实单价的时候 判断  对应供应商 是否有该条物料的 对应单价 
        /// 如有记录判断是否修改单价 
        /// 如果没有新增记录
        /// </summary>
        private DataTable fun_add(string str_供应商ID, string str_物料号, decimal dec_单价)
        {

            DataTable dt = new DataTable();
            string sql = string.Format("select  * from 采购供应商物料单价表 where 供应商ID='{0}' and 物料编码='{1}'", str_供应商ID, str_物料号);
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            if (dt.Rows.Count > 0)
            {
                dt.Rows[0]["单价"] = dec_单价;

            }
            else
            {

                DataRow r = dt.NewRow();
                r["供应商ID"] = str_供应商ID;
                r["物料编码"] = str_物料号;
                r["单价"] = dec_单价;
                dt.Rows.Add(r);

            }

            return dt;
        }



        private DataTable fun_add(string str_供应商ID, DataTable t)
        {

            DataTable dt = new DataTable();
            foreach (DataRow dr in t.Rows)
            {
                string sql = string.Format("select  * from 采购供应商物料单价表 where 供应商ID='{0}' and 物料编码='{1}'", str_供应商ID, dr["物料编码"]);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    da.Fill(dt);
                    DataRow[] r_price = dt.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (r_price.Length > 0)
                    {
                        r_price[0]["单价"] = dr["单价"];
                        r_price[0]["不含税单价"] = dr["未税单价"];
                    }
                    else
                    {

                        DataRow r = dt.NewRow();
                        r["供应商ID"] = str_供应商ID;
                        r["物料编码"] = dr["物料编码"];
                        r["单价"] = dr["单价"];
                        r["不含税单价"] = dr["未税单价"];
                        dt.Rows.Add(r);
                    }
                }


            }
            //string sql = string.Format("select  * from 采购供应商物料单价表 where 供应商ID='{0}' and 物料编码='{1}'", str_供应商ID, str_物料号);
            //dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            //if (dt.Rows.Count > 0)
            //{
            //    dt.Rows[0]["单价"] = dec_单价;

            //}
            //else
            //{

            //    DataRow r = dt.NewRow();
            //    r["供应商ID"] = str_供应商ID;
            //    r["物料编码"] = str_物料号;
            //    r["单价"] = dec_单价;
            //    dt.Rows.Add(r);

            //}

            return dt;
        }
        /// <summary>
        /// 补开的 核实单价
        /// 19-5-12 弃用
        /// </summary>
        /// <param name="danjia"></param>
        private void fun_核实单据价格_补(decimal danjia)
        {
            try
            {
                DataRow r = (this.BindingContext[dt_入库单明细].Current as DataRowView).Row;
                string sql = "";
                DataTable dt_采购单主表 = new DataTable();
                DataTable dt_采购单明细 = new DataTable();
                DataTable dt_开票明细;
                DataTable dt_入库明细;
                sql = string.Format("select *  from L采购记录采购单入库明细L where 采购单明细号='{0}'", r["采购单明细号"].ToString());
                dt_入库明细 = MasterSQL.Get_DataTable(sql, strcon);
                sql = string.Format("select *  from  采购记录采购开票通知单明细表 where 采购单明细号='{0}'", r["采购单明细号"].ToString());
                dt_开票明细 = MasterSQL.Get_DataTable(sql, strcon);
                if (Convert.ToDecimal(r["单价"]) == danjia)
                {


                    DataRow[] x = dt_入库明细.Select(string.Format("入库单号='{0}'", r["入库单号"]));
                    x[0]["税率"] = r["税率"];
                    x[0]["单价"] = r["单价"];
                    x[0]["未税单价"] = r["未税单价"];
                    x[0]["金额"] = r["金额"];
                    x[0]["未税金额"] = r["未税金额"];
                    foreach (DataRow r1 in dt_入库明细.Rows)
                    {

                        r1["价格核实"] = true;

                    }
                    foreach (DataRow r1 in dt_开票明细.Rows)
                    {

                        r1["价格核实"] = true;

                    }
                }
                else
                {
                    /* 18-4-25 注释
                    //修改采购单明细的单价
                    sql = string.Format("select * from 采购记录采购明细辅助表 where 采购单号='{0}'", r["采购单号"].ToString());
                    dt_采购单明细 = MasterSQL.Get_DataTable(sql, strcon);
                    DataRow[] dr = dt_采购单明细.Select(string.Format("采购明细号='{0}'", r["采购单明细号"].ToString()));
                    if (dr.Length > 0)
                    {
                        dr[0]["单价"] = danjia;
                        dr[0]["未税单价"] = (danjia / (1 + Convert.ToDecimal(r["税率"]) / 100)).ToString("0.000000");
                        dr[0]["金额"] = (danjia * Convert.ToDecimal(r["采购数量"])).ToString("0.000000");
                        dr[0]["未税金额"] = ((Convert.ToDecimal(dr[0]["金额"]) / (1 + Convert.ToDecimal(r["税率"]) / 100))).ToString("0.000000");
                        dr[0]["税金"] = Convert.ToDecimal(dr[0]["金额"]) - Convert.ToDecimal(dr[0]["未税金额"]);
                    }
                    //修改采购单主表的金额
                    decimal totalMoney = 0;
                    foreach (DataRow r2 in dt_采购单明细.Rows)
                    {
                        totalMoney = totalMoney + Convert.ToDecimal(r["金额"]);
                    }
                    sql = string.Format("select * from 采购记录采购单辅助主表 where 采购单号='{0}'", r["采购单号"].ToString());
                    dt_采购单主表 = MasterSQL.Get_DataTable(sql, strcon);
                    foreach (DataRow r2 in dt_采购单主表.Rows)
                    {
                        r2["总金额"] = totalMoney;
                        r2["未税金额"] = (totalMoney / (1 + Convert.ToDecimal(r["税率"]) / 100)).ToString("0.000000");
                        r2["税金"] = Convert.ToDecimal(r2["总金额"]) - Convert.ToDecimal(r2["未税金额"]);
                    }
                   */


                    //修改入库单的明细
                    foreach (DataRow r1 in dt_入库明细.Rows)
                    {
                        r1["单价"] = danjia;
                        r1["未税单价"] = (danjia / (1 + Convert.ToDecimal(r1["税率"]) / 100)).ToString("0.000000");
                        r1["金额"] = (danjia * Convert.ToDecimal(r1["采购数量"])).ToString("0.000000");
                        r1["未税金额"] = (Convert.ToDecimal(r1["金额"]) / (1 + Convert.ToDecimal(r1["税率"]) / 100)).ToString("0.000000");
                        r1["价格核实"] = true;
                        r1["单价备注"] = r["单价备注"];

                    }
                    foreach (DataRow r2 in dt_开票明细.Rows)
                    {
                        r2["单价"] = danjia;
                        r2["未税单价"] = (danjia / (1 + Convert.ToDecimal(r["税率"]) / 100)).ToString("0.000000");
                        r2["金额"] = (danjia * Convert.ToDecimal(r2["采购数量"])).ToString("0.000000");
                        r2["未税金额"] = (Convert.ToDecimal(r2["金额"]) / (1 + Convert.ToDecimal(r["税率"]) / 100)).ToString("0.000000");
                        r2["价格核实"] = true;
                    }


                }

                DataTable dt_ss = fun_add(r["供应商ID"].ToString(), r["物料编码"].ToString(), danjia);
                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction jghs = conn.BeginTransaction("价格核实"); //事务的名称
                SqlCommand cmd = new SqlCommand("select * from 采购记录采购单辅助主表 where 1<>1", conn, jghs);
                SqlCommand cmd1 = new SqlCommand("select * from 采购记录采购明细辅助表 where 1<>1", conn, jghs);
                try
                {
                    SqlDataAdapter da;
                    if (dt_采购单主表 != null)
                    {

                        da = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da);

                        da.Update(dt_采购单主表);
                        da = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da);
                        da.Update(dt_采购单明细);

                    }

                    SqlCommand cmd4 = new SqlCommand("select * from L采购记录采购单入库明细L where 1<>1", conn, jghs);
                    da = new SqlDataAdapter(cmd4);
                    new SqlCommandBuilder(da);
                    da.Update(dt_入库明细);
                    SqlCommand cmd5 = new SqlCommand("select * from 采购记录采购开票通知单明细表 where 1<>1", conn, jghs);
                    da = new SqlDataAdapter(cmd5);
                    new SqlCommandBuilder(da);
                    da.Update(dt_开票明细);
                    SqlCommand cmd6 = new SqlCommand("select * from 采购供应商物料单价表 where 1<>1", conn, jghs);
                    da = new SqlDataAdapter(cmd6);
                    new SqlCommandBuilder(da);
                    da.Update(dt_ss);
                    jghs.Commit();
                }
                catch
                {
                    jghs.Rollback();
                }
                //MasterSQL.Save_DataTable(dt_开票明细, "采购记录采购开票通知单明细表", strcon);


                //MasterSQL.Save_DataTable(dt_入库明细, "L采购记录采购单入库明细L", strcon);

            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_核实单据价格");
                throw ex;
            }
        }


        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                // decimal strDanjia = 0;
                if (dt_入库单明细 == null || dt_入库单明细.Rows.Count <= 0) return;
                gridView1.CloseEditor();
                this.BindingContext[dt_入库单明细].EndCurrentEdit();
                //  DataRow r = (this.BindingContext[dt_入库单明细].Current as DataRowView).Row;
                // DataRow r = (this.BindingContext[dt_入库单明细].Current as DataRowView).Row;

                // fun_check(r);


                //if (r["核实单价"].ToString() == "")
                //{
                //    strDanjia = Convert.ToDecimal(r["单价"]);
                //}
                //else
                //{
                //foreach (DataRow dr in dt_入库单明细.Rows)
                //{
                //try
                //{
                //    if (dr["核实单价"].ToString().Trim() == "") continue;
                //    decimal a = Convert.ToDecimal(dr["核实单价"]);

                //}
                //catch (Exception)
                //{
                //    throw new Exception(string.Format("入库明细号\"{0}\"的核实单价应该为数字，请检查并修改！", dr["入库明细号"]));
                //}
                //if (Convert.ToDecimal(dr["核实单价"]) != Convert.ToDecimal(dr["单价"]))
                //{
                //    if (dr["单价备注"].ToString().Trim() == "")
                //    {
                //        throw new Exception("核实单价与原单价不同必须填写核实备注");
                //    }
                //}

                //}

                //}

                if (MessageBox.Show(string.Format("确认信息无误吗？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    //if (flag)
                    //{
                    //    //fun_核实单据价格_补(strDanjia);
                    //}
                    //else
                    //{
                    fun_核实单据价格();
                    //}
                    bl = true;
                    dt_已核价 = dt_入库单明细.Copy();
                    this.ParentForm.Close();
                    //fun_Load入库明细();

                    MessageBox.Show("价格核实成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //界面关闭
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show(string.Format("是否确认关闭界面"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                this.ParentForm.Close();
            }
        }

        #region 弃用


        //同意调价:不含税单价不改,   含税单价= 原含税单价/1.17*1.16 || 不含税单价*1.16 
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);

            if (Convert.ToInt32(r["税率"]) == 17)
            {

                if (MessageBox.Show(string.Format("确定入库单明细号:{0},同意调价吗？", r["入库明细号"].ToString()), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    r["税率"] = 16;
                    r["单价"] = Convert.ToDecimal(r["未税单价"].ToString()) * (decimal)1.16;
                    r["金额"] = Math.Round(Convert.ToDecimal(r["单价"]) * Convert.ToDecimal(r["入库量"]),2,MidpointRounding.AwayFromZero);
                    r["未税金额"] =Math.Round(Convert.ToDecimal(r["未税单价"]) * Convert.ToDecimal(r["入库量"]),2,MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                MessageBox.Show("该税率不为17点，不可使用此功能");
            }

        }
        //不同意调价 ，含税单价不改， 不含税单价=含税单价/1.16
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);

            if (Convert.ToInt32(r["税率"]) == 17)
            {

                if (MessageBox.Show(string.Format("确定入库单明细号:{0},不同意调价吗？", r["入库明细号"].ToString()), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    r["税率"] = 16;
                    r["未税单价"] = Convert.ToDecimal(r["单价"].ToString()) / (decimal)1.16;
                    r["金额"] = Convert.ToDecimal(r["单价"]) * Convert.ToDecimal(r["入库量"].ToString());
                    r["未税金额"] = Convert.ToDecimal(r["未税单价"]) * Convert.ToDecimal(r["入库量"]);
                }
            }
            else
            {
                MessageBox.Show("该税率不为17点，不可使用此功能");
            }

        }
        #endregion

        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(e.RowHandle);
                if (e.Column.FieldName == "单价")
                {
                    decimal dec_税率 = Convert.ToDecimal(dr["税率"]);
                    dr["未税单价"] = Math.Round(Convert.ToDecimal(e.Value) / (1 + dec_税率 / 100), 6);
                    dr["金额"] = Math.Round(Convert.ToDecimal(e.Value) * Convert.ToDecimal(dr["入库量"]), 2);
                    dr["未税金额"] = Math.Round(Convert.ToDecimal(dr["未税单价"]) * Convert.ToDecimal(dr["入库量"]),2);
                    dr["税金"] = Math.Round(Convert.ToDecimal(dr["金额"]) - Convert.ToDecimal(dr["未税金额"]),2);
                }
                else if (e.Column.FieldName == "未税单价")
                {
                    decimal dec_税率 = Convert.ToDecimal(dr["税率"]);
                    dr["单价"] = Math.Round(Convert.ToDecimal(e.Value) * (1 + dec_税率 / (decimal)100), 6);
                    dr["金额"] = Math.Round(Convert.ToDecimal(dr["单价"]) * Convert.ToDecimal(dr["入库量"]),2);
                    dr["未税金额"] = Math.Round(Convert.ToDecimal(e.Value) * Convert.ToDecimal(dr["入库量"]),2);
                    dr["税金"] = Math.Round(Convert.ToDecimal(dr["金额"]) - Convert.ToDecimal(dr["未税金额"]),2);

                }
                else if (e.Column.FieldName == "税率")
                {
                    decimal dec_税率 = Convert.ToDecimal(e.Value);
                    dr["未税单价"] = Math.Round(Convert.ToDecimal(dr["单价"]) / (1 + dec_税率 / 100), 6);
                    dr["金额"] = Math.Round(Convert.ToDecimal(dr["单价"]) * Convert.ToDecimal(dr["入库量"]),2);
                    dr["未税金额"] = Math.Round(Convert.ToDecimal(dr["未税单价"]) * Convert.ToDecimal(dr["入库量"]),2);
                    dr["税金"] = Math.Round(Convert.ToDecimal(dr["金额"]) - Convert.ToDecimal(dr["未税金额"]),2);
                }
                else if (e.Column.FieldName == "未税金额")
                {
                    decimal dec_税率 = Convert.ToDecimal(dr["税率"]);
                    dr["未税单价"] = Math.Round(Convert.ToDecimal(e.Value) / Convert.ToDecimal(dr["入库量"]), 6);
                    dr["单价"] = Math.Round(Convert.ToDecimal(dr["未税单价"]) * (1 + dec_税率 / 100), 6);
                    dr["金额"] = Math.Round(Convert.ToDecimal(dr["单价"]) * Convert.ToDecimal(dr["入库量"]),2);
                    dr["税金"] = Math.Round(Convert.ToDecimal(dr["金额"]) - Convert.ToDecimal(e.Value),2);

                }
                else if (e.Column.FieldName == "金额")
                {
                    decimal dec_税率 = Convert.ToDecimal(dr["税率"]);
                    dr["单价"] = Math.Round(Convert.ToDecimal(e.Value) / Convert.ToDecimal(dr["入库量"]), 6);
                    dr["未税单价"] = Math.Round(Convert.ToDecimal(dr["单价"]) / (1 + dec_税率 / 100), 6);
                    dr["未税金额"] = Math.Round(Convert.ToDecimal(dr["未税单价"]) * Convert.ToDecimal(dr["入库量"]),2);
                    dr["税金"] = Math.Round(Convert.ToDecimal(e.Value) - Convert.ToDecimal(dr["未税金额"]),2);

                }
                else if (e.Column.FieldName == "入库量")
                {
                    
                    dr["未税金额"] = Math.Round(Convert.ToDecimal(dr["未税单价"]) * Convert.ToDecimal(e.Value), 2);
                    dr["金额"] = Math.Round(Convert.ToDecimal(dr["单价"]) * Convert.ToDecimal(e.Value), 2);

                    dr["税金"] = Math.Round(Convert.ToDecimal(dr["金额"]) - Convert.ToDecimal(dr["未税金额"]),2);

                }
                else if (e.Column.FieldName == "税金")
                {

                    // dr["未税金额"] = Math.Round(Convert.ToDecimal(dr["未税单价"]) * Convert.ToDecimal(e.Value), 6);
                    dr["金额"] = Math.Round(Convert.ToDecimal(dr["未税金额"])+ Convert.ToDecimal(e.Value) ,2);
                    dr["单价"] = Math.Round(Convert.ToDecimal(dr["金额"]) / Convert.ToDecimal(dr["入库量"]),2);

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            dt_入库单明细.Rows.Remove(dr);
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.V)
                {
                    if (gridView1.FocusedColumn.Caption == "税率")
                    {
                        foreach (DataRow dr in dt_入库单明细.Rows)
                        {
                            decimal dec_税率 = Convert.ToDecimal(dr["税率"]);

                            dr["未税单价"] = Math.Round(Convert.ToDecimal(dr["单价"]) / (1 + dec_税率 / 100), 6);
                            dr["金额"] = Math.Round(Convert.ToDecimal(dr["单价"]) * Convert.ToDecimal(dr["入库量"]), 2);
                            dr["未税金额"] = Math.Round(Convert.ToDecimal(dr["未税单价"]) * Convert.ToDecimal(dr["入库量"]),2);
                            dr["税金"] = Math.Round(Convert.ToDecimal(dr["金额"]) - Convert.ToDecimal(dr["未税金额"]),2);
                        }
                    }
                    else if(gridView1.FocusedColumn.Caption == "单价")
                    {
                        foreach (DataRow dr in dt_入库单明细.Rows)
                        {

                            decimal dec_税率 = Convert.ToDecimal(dr["税率"]);
                            dr["未税单价"] = Math.Round(Convert.ToDecimal(dr["单价"]) / (1 + dec_税率 / 100), 6);
                            dr["金额"] = Math.Round(Convert.ToDecimal(dr["单价"]) * Convert.ToDecimal(dr["入库量"]),2);
                            dr["未税金额"] = Math.Round(Convert.ToDecimal(dr["未税单价"]) * Convert.ToDecimal(dr["入库量"]),2);
                            dr["税金"] = Math.Round(Convert.ToDecimal(dr["金额"]) - Convert.ToDecimal(dr["未税金额"]),2);
                        }
                    }
                    else if(gridView1.FocusedColumn.Caption == "未税单价")
                    {
                        foreach (DataRow dr in dt_入库单明细.Rows)
                        {
                            decimal dec_税率 = Convert.ToDecimal(dr["税率"]);
                            dr["单价"] = Math.Round(Convert.ToDecimal(dr["未税单价"]) * (1 + dec_税率 / (decimal)100), 6);
                            dr["金额"] = Math.Round(Convert.ToDecimal(dr["单价"]) * Convert.ToDecimal(dr["入库量"]),2);
                            dr["未税金额"] = Math.Round(Convert.ToDecimal(dr["未税单价"]) * Convert.ToDecimal(dr["入库量"]),2);
                            dr["税金"] = Math.Round(Convert.ToDecimal(dr["金额"]) - Convert.ToDecimal(dr["未税金额"]),2);
                        }
                    }
                    else if (gridView1.FocusedColumn.Caption == "入库量")
                    {
                        foreach (DataRow dr in dt_入库单明细.Rows)
                        {
                            dr["未税金额"] = Math.Round(Convert.ToDecimal(dr["未税单价"]) * Convert.ToDecimal(dr["入库量"]),2);
                            dr["金额"] = Math.Round(Convert.ToDecimal(dr["单价"]) * Convert.ToDecimal(dr["入库量"]),2);
                            dr["税金"] = Math.Round(Convert.ToDecimal(dr["金额"]) - Convert.ToDecimal(dr["未税金额"]),2);
                        }
                    }
                    else if (gridView1.FocusedColumn.Caption == "未税金额")
                    {
                        foreach (DataRow dr in dt_入库单明细.Rows)
                        {
                            decimal dec_税率 = Convert.ToDecimal(dr["税率"]);

                            dr["未税单价"] = Math.Round(Convert.ToDecimal(dr["未税金额"]) / Convert.ToDecimal(dr["入库量"]), 6);
                            dr["单价"] = Math.Round(Convert.ToDecimal(dr["未税单价"]) * (1 + dec_税率 / 100), 6);
                            dr["金额"] = Math.Round(Convert.ToDecimal(dr["单价"]) * Convert.ToDecimal(dr["入库量"]),2);
                            dr["税金"] = Math.Round(Convert.ToDecimal(dr["金额"]) - Convert.ToDecimal(dr["未税金额"]),2);
                        }
                    }
                    else if (gridView1.FocusedColumn.Caption == "金额")
                    {
                        foreach (DataRow dr in dt_入库单明细.Rows)
                        {
                            decimal dec_税率 = Convert.ToDecimal(dr["税率"]);
                            dr["单价"] = Math.Round(Convert.ToDecimal(dr["金额"]) / Convert.ToDecimal(dr["入库量"]), 6);
                            dr["未税单价"] = Math.Round(Convert.ToDecimal(dr["单价"]) / (1 + dec_税率 / 100), 6);
                            dr["未税金额"] = Math.Round(Convert.ToDecimal(dr["未税单价"]) * Convert.ToDecimal(dr["入库量"]), 6);
                            dr["税金"] = Math.Round(Convert.ToDecimal(dr["金额"]) - Convert.ToDecimal(dr["未税金额"]),2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {  }
        }
    }
}
