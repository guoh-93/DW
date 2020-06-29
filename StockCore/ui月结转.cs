using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Windows.Forms;
namespace StockCore
{
#pragma warning disable IDE1006 // 命名样式
    public partial class ui月结转 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {
        DataTable dtM;
        DataTable dt_上月结存;
        DataTable list_m;
        DataTable t_成本;
        DataTable t_工单;
        DataTable t_耗用;
        bool bl_计算 = false;
        bool bl_计算2 = false;


        public struct result
        {

            public decimal dec_数量;
            public decimal dec_单价;
            public decimal dec_金额;


        }
        DataTable dtQ;
        string strconn = CPublic.Var.strConn;
        public ui月结转()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_载入物料(int y, int M)
#pragma warning restore IDE1006 // 命名样式
        {
            DateTime t = new DateTime(y, M, 1); //上月月初
            DateTime t1 = t.AddMonths(1);

            string sql = "select * from 仓库月出入库结转表 where 1<>1";
            dtQ = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtQ);
            sql = string.Format(@" select  物料编码,本月结转数量,本月结转金额,结存单价 from 仓库月出入库结转表 jz where 年={0} and 月={1}", y, M);
            da = new SqlDataAdapter(sql, strconn);
            dt_上月结存 = new DataTable();
            da.Fill(dt_上月结存);
            DataColumn[] pk_jc = new DataColumn[1];
            pk_jc[0] = dt_上月结存.Columns["物料编码"];
            dt_上月结存.PrimaryKey = pk_jc;

            sql = string.Format(@"select  x.物料编码,base.委外 from (select 物料编码 from C_存货核算物料单价表 where 月={0} and 年={1}  union
            select  物料编码 from 仓库出入库明细表 where 出入库时间 > '{2}' and 出入库时间<'{3}' group by 物料编码)x
            left join 基础数据物料信息表 base on base.物料编码=x.物料编码 ", t1.Month, t1.Year, t1, t1.AddMonths(1));
            list_m = new DataTable();
            list_m = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            DataColumn[] pk_l = new DataColumn[1];
            pk_l[0] = list_m.Columns["物料编码"];
            list_m.PrimaryKey = pk_l;


            sql = string.Format(@" select  * from C_存货核算物料单价表 where 年={0} and 月={1}", t1.Year, t1.Month);
            t_成本 = new DataTable();
            t_成本 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            DataColumn[] pk_cb = new DataColumn[1];
            pk_cb[0] = t_成本.Columns["物料编码"];
            t_成本.PrimaryKey = pk_cb;


            sql = string.Format(@" select  * from [C_工单] where 年={0} and 月={1}", t1.Year, t1.Month);
            t_工单 = new DataTable();
            t_工单 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            DataColumn[] pk_gd = new DataColumn[1];
            pk_gd[0] = t_工单.Columns["生产工单号"];
            t_工单.PrimaryKey = pk_gd;

            sql = string.Format(@" select  * from [C_工单当期耗用] where 年={0} and 月={1}", t1.Year, t1.Month);
            t_耗用 = new DataTable();
            t_耗用 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);


        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 19-7-8 加入仓库号 字段 分仓库
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="str_出入库"></param>
        /// <param name="str_物料编码"></param>
        /// <param name="stock_id"></param>
        /// <returns></returns>
        private decimal fun_计算(DateTime d1, DateTime d2, string str_出入库, string str_物料编码, string stock_id)
#pragma warning restore IDE1006 // 命名样式
        {
            decimal dec = 0;
            if (str_出入库 == "出库")    //销售出库,其它出库,生产领料出库 //19-7-8 全部取自 仓库出入库明细表
            {
                #region 19-7-8作废 
                //string sql_1 = string.Format("select sum(已出库数量)数量 from 销售记录成品出库单明细表 where 生效日期>'{0}'and 生效日期<'{1}' and 物料编码='{2}'",d1,d2,str_物料编码);
                //using (SqlDataAdapter da_1 = new SqlDataAdapter(sql_1, strconn))
                //{
                //    DataTable dt = new DataTable();
                //    da_1.Fill(dt);
                //    if (dt != null && dt.Rows.Count > 0)
                //    {
                //        if (dt.Rows[0]["数量"] != null)
                //        {
                //            try
                //            {
                //                Convert.ToDecimal(dt.Rows[0]["数量"]);

                //            }
                //            catch (Exception)
                //            {

                //                dt.Rows[0]["数量"] = 0;
                //            }
                //            dec = dec + Convert.ToDecimal(dt.Rows[0]["数量"]);
                //        }
                //    }
                //}
                //string sql_2 = string.Format("select sum(已领数量)数量 from 生产记录生产工单待领料明细表 where 完成日期>'{0}'and 完成日期<'{1}' and 物料编码='{2}'",d1,d2,str_物料编码);
                //using (SqlDataAdapter da_1 = new SqlDataAdapter(sql_2, strconn))
                //{
                //    DataTable dt = new DataTable();
                //    da_1.Fill(dt);
                //    if (dt != null && dt.Rows.Count > 0)
                //    {
                //        if (dt.Rows[0]["数量"] != null)
                //        {
                //            try
                //            {
                //                Convert.ToDecimal(dt.Rows[0]["数量"]);

                //            }
                //            catch (Exception)
                //            {

                //                dt.Rows[0]["数量"] = 0;
                //            }
                //            dec = dec + Convert.ToDecimal(dt.Rows[0]["数量"]);
                //        }
                //    }
                //}
                //string sql_3 = string.Format(@"select ABS(sum(实效数量)) as 数量 from 仓库出入库明细表,基础数据物料信息表,其他出库子表  
                //                where 仓库出入库明细表.物料编码=基础数据物料信息表.物料编码 and 仓库出入库明细表.明细号 =其他出库子表.其他出库明细号 and 
                //            出入库时间 >'{0}' and 出入库时间 <'{1}' and 明细类型='其他出库' and 仓库出入库明细表.物料编码='{2}'", d1, d2, str_物料编码);
                //using (SqlDataAdapter da_1 = new SqlDataAdapter(sql_3, strconn))
                //{
                //    DataTable dt = new DataTable();
                //    da_1.Fill(dt);
                //    if (dt != null && dt.Rows.Count > 0)
                //    {
                //        if (dt.Rows[0]["数量"] != null)
                //        {
                //            try
                //            {
                //                Convert.ToDecimal(dt.Rows[0]["数量"]);

                //            }
                //            catch (Exception)
                //            {

                //                dt.Rows[0]["数量"] = 0;
                //            }
                //            dec = dec + Convert.ToDecimal(dt.Rows[0]["数量"]);
                //        }
                //    }
                //}
                #endregion 
                //出库在仓库出入库明细中为 负数,红字回冲的为 出库类型 正数  
                string s = string.Format(@"select  sum(实效数量*(-1)) 数量  from 仓库出入库明细表 where 出库入库='出库' and 出入库时间 >'{0}'  and 出入库时间 <'{1}' and 物料编码='{2}' and 仓库号='{3}'", d1, d2, str_物料编码, stock_id);
                using (SqlDataAdapter da_1 = new SqlDataAdapter(s, strconn))
                {
                    DataTable dt = new DataTable();
                    da_1.Fill(dt);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0]["数量"] != null)
                        {
                            try
                            {
                                Convert.ToDecimal(dt.Rows[0]["数量"]);
                            }
                            catch (Exception)
                            {
                                dt.Rows[0]["数量"] = 0;
                            }
                            dec = dec + Convert.ToDecimal(dt.Rows[0]["数量"]);
                        }
                    }
                }
            }
            else    //采购库,其它入库,退货入库 //19-7-8 
            {

                #region 19-7-8作废
                //    string sql_1 = string.Format("select sum(采购数量)数量 from 采购记录采购单入库明细 where 生效日期>'{0}'and 生效日期<'{1}' and  物料编码='{2}' ",d1,d2,str_物料编码);
                //    using (SqlDataAdapter da_1 = new SqlDataAdapter(sql_1, strconn))
                //    {
                //        DataTable dt = new DataTable();
                //        da_1.Fill(dt);
                //        if (dt != null && dt.Rows.Count > 0)
                //        {
                //            if (dt.Rows[0]["数量"] != null)
                //            {
                //                try
                //                {
                //                   Convert.ToDecimal(dt.Rows[0]["数量"]);

                //                }
                //                catch (Exception)
                //                {

                //                    dt.Rows[0]["数量"] = 0;
                //                }
                //                dec = dec + Convert.ToDecimal(dt.Rows[0]["数量"]);
                //            }
                //        }
                //    }
                //    string sql_2 = string.Format(@"select ABS(sum(实效数量)) as 数量 from 仓库出入库明细表,基础数据物料信息表,其他入库子表  
                //             where 仓库出入库明细表.物料编码=基础数据物料信息表.物料编码 and 仓库出入库明细表.明细号 =其他入库子表.其他入库明细号 and 
                //             出入库时间 >'{0}' and 出入库时间 <'{1}' and 明细类型='其他入库' and  仓库出入库明细表.物料编码='{2}'", d1, d2, str_物料编码);
                //    using (SqlDataAdapter da_1 = new SqlDataAdapter(sql_2, strconn))
                //    {
                //        DataTable dt = new DataTable();
                //        da_1.Fill(dt);
                //        if (dt != null && dt.Rows.Count > 0)
                //        {
                //            if (dt.Rows[0]["数量"] != null)
                //            {
                //                try
                //                {
                //                    Convert.ToDecimal(dt.Rows[0]["数量"]);

                //                }
                //                catch (Exception)
                //                {

                //                    dt.Rows[0]["数量"] = 0;
                //                }
                //                    dec = dec + Convert.ToDecimal(dt.Rows[0]["数量"]);


                //            }
                //        }
                //    }
                //    string sql_3 = string.Format("select sum(数量)数量 from 退货入库子表 where 生效日期>'{0}'and 生效日期<'{1}'  and 物料编码='{2}'",d1,d2,str_物料编码);
                //    using (SqlDataAdapter da_1 = new SqlDataAdapter(sql_3, strconn))
                //    {
                //        DataTable dt = new DataTable();
                //        da_1.Fill(dt);
                //        if (dt != null && dt.Rows.Count > 0)
                //        {
                //            if (dt.Rows[0]["数量"] != null)
                //            {
                //                try
                //                {
                //                    Convert.ToDecimal(dt.Rows[0]["数量"]);

                //                }
                //                catch (Exception)
                //                {

                //                    dt.Rows[0]["数量"] = 0;
                //                }
                //                dec = dec + Convert.ToDecimal(dt.Rows[0]["数量"]);
                //            }
                //        }
                //    }
                //    string sql_4 = string.Format("select sum(入库数量)数量 from 生产记录成品入库单明细表 where 生效日期>'{0}'and 生效日期<'{1}' and  物料编码='{2}'",d1,d2,str_物料编码);
                //    using (SqlDataAdapter da_1 = new SqlDataAdapter(sql_4, strconn))
                //    {
                //        DataTable dt = new DataTable();
                //        da_1.Fill(dt);
                //        if (dt != null && dt.Rows.Count > 0)
                //        {
                //            if (dt.Rows[0]["数量"] != null)
                //            {
                //                try
                //                {
                //                    Convert.ToDecimal(dt.Rows[0]["数量"]);

                //                }
                //                catch (Exception)
                //                {

                //                    dt.Rows[0]["数量"] = 0;
                //                }
                //                dec = dec + Convert.ToDecimal(dt.Rows[0]["数量"]);
                //            }
                //        }
                //    }
                #endregion

                //入库在仓库出入库明细中全部为正
                string s = string.Format(@"select  sum(实效数量) as 数量  from 仓库出入库明细表 where 出库入库='入库' and 出入库时间 >'{0}'  and 出入库时间 <'{1}' and 物料编码='{2}' and 仓库号='{3}'", d1, d2, str_物料编码, stock_id);
                using (SqlDataAdapter da_1 = new SqlDataAdapter(s, strconn))
                {
                    DataTable dt = new DataTable();
                    da_1.Fill(dt);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0]["数量"] != null)
                        {
                            try
                            {
                                Convert.ToDecimal(dt.Rows[0]["数量"]);
                            }
                            catch (Exception)
                            {
                                dt.Rows[0]["数量"] = 0;
                            }
                            dec = dec + Convert.ToDecimal(dt.Rows[0]["数量"]);
                        }
                    }
                }

            }
            return dec;
        }
        /// <summary>
        /// 不区分仓库
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="str_出入库"></param>
        /// <param name="str_物料编码"></param>
        /// <returns></returns>
        private result fun_计算(DateTime d1, DateTime d2, string str_出入库, string str_物料编码, DataSet ds_出, DataSet ds_入,bool bl_委外)
#pragma warning restore IDE1006 // 命名样式
        {
            result ss = new result();
            decimal dec = 0;
            string s = "";
            DataTable t_出汇总 = ds_出.Tables[0];
            DataTable t_sale出汇总 = ds_出.Tables[1];
            DataTable t_发料汇总 = ds_出.Tables[2];
            DataTable t_chaid = ds_出.Tables[3];
            DataTable t_th = ds_出.Tables[4];
            DataTable t_xtzhck = ds_出.Tables[5];


            DataTable t_入汇总 = ds_入.Tables[0];
            DataTable t_采购入 = ds_入.Tables[1];
            DataTable t_采购冲暂估 = ds_入.Tables[2];
            DataTable t_chaidr = ds_入.Tables[3];
            DataTable t_xtzhrk = ds_入.Tables[4];

            if (str_出入库 == "出库") //销售出库,其它出库,生产领料出库//19-7-8 全部取自仓库出入库明细表 other 那部分其他入库的数据 已经把 '出库入库'字段 改成了出库 
            {
                //出库在仓库出入库明细中为 负数,红字回冲的为 出库类型 正数   
                //20-4-10 用 t_出入明细 替代
                #region 原
                //string s = string.Format(@"select  sum(实效数量*(-1)) 数量  from 仓库出入库明细表 where 出库入库='出库' and 出入库时间 >'{0}'  
                //and 出入库时间 <'{1}' and 物料编码='{2}' and 明细类型 <> '借用出库' ", d1, d2, str_物料编码);
                //using (SqlDataAdapter da_1 = new SqlDataAdapter(s, strconn))
                //{
                //DataTable dt = new DataTable();
                //da_1.Fill(dt);

                //if (dt != null && dt.Rows.Count > 0)
                //{
                //    if (dt.Rows[0]["数量"] != null)
                //    {
                //        try
                //        {
                //            Convert.ToDecimal(dt.Rows[0]["数量"]);
                //        }
                //        catch (Exception)
                //        {
                //            dt.Rows[0]["数量"] = 0;
                //        }
                //        dec = dec + Convert.ToDecimal(dt.Rows[0]["数量"]);
                //    }
                //}
                //}
                #endregion

                DataRow[] r_出汇总 = t_出汇总.Select($"物料编码='{str_物料编码}'");
                if (r_出汇总.Length > 0)
                {
                    if (r_出汇总[0]["数量"] != null)
                    {
                        try
                        {
                            Convert.ToDecimal(r_出汇总[0]["数量"]);
                        }
                        catch (Exception)
                        {
                            r_出汇总[0]["数量"] = 0;
                        }
                        dec = dec + Convert.ToDecimal(r_出汇总[0]["数量"]);
                    }
                }


                //发出金额 = 销售出库 + 生产发料+其他出库 + 销售退货（除010202 这个在入库得时候 已经把 退货的金额也算进去了）
                decimal dec_销售出库 = 0;
                //s = string.Format(@"select  sum(round(出库数量*发出单价,2))金额 from 销售记录成品出库单明细表 
                //  where 生效日期>'{0}' and 生效日期<'{1}' and 物料编码='{2}' and 退货标识<>'是' ", d1, d2, str_物料编码);
                //DataTable t_销售出库 = CZMaster.MasterSQL.Get_DataTable(s, strconn);

                //if (t_销售出库.Rows.Count > 0)
                //{
                //    if (t_销售出库.Rows[0]["金额"] != null && t_销售出库.Rows[0]["金额"] != DBNull.Value && t_销售出库.Rows[0]["金额"].ToString() != "")
                //    {
                //        dec_销售出库 = Convert.ToDecimal(t_销售出库.Rows[0]["金额"]);
                //    }
                //}

                DataRow[] r_sale出汇总 = t_sale出汇总.Select($"物料编码='{str_物料编码}'");
                if (r_sale出汇总.Length > 0)
                {
                    if (r_sale出汇总[0]["金额"] != null && r_sale出汇总[0]["金额"] != DBNull.Value && r_sale出汇总[0]["金额"].ToString() != "")
                    {
                        dec_销售出库 = Convert.ToDecimal(r_sale出汇总[0]["金额"]);
                    }

                }

                //2020-4-10 这块先不动 
                string sql_x = "";
                //if (str_物料编码.Substring(0, 4) == "0102") ///此处因为入库的时候  已经把金额加进去了 所以出库不要再算了
                //20-4-10换成判断委外属性 
                if(bl_委外)
                {
                    sql_x = "and(存货核算标记 = 0 or(存货核算标记 = 1 and 原因分类 = '入库倒冲'))";
                }
                s = string.Format(@"select  sum(round(数量*结算单价,2))金额 from 其他出库子表 a 
                    left join 其他出入库申请主表 b on a.出入库申请单号 =b.出入库申请单号   
                    where a.生效日期>'{0}' and a.生效日期<'{1}' and 物料编码='{2}' {3} ", d1, d2, str_物料编码, sql_x);
                // 
                DataTable t_其他出库 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                decimal dec_其他出库 = 0;
                if (t_其他出库.Rows.Count > 0)
                {
                    if (t_其他出库.Rows[0]["金额"] != null && t_其他出库.Rows[0]["金额"] != DBNull.Value && t_其他出库.Rows[0]["金额"].ToString() != "")
                    {
                        dec_其他出库 = Convert.ToDecimal(t_其他出库.Rows[0]["金额"]);
                    }
                }
                /*8-18 这块已经没有了
                //开始没有红字回冲 做的其他入库  应放在出库中 数量为 负
                s = string.Format(@"select  物料编码,-sum(round(数量*结算单价,2))金额 from (
               select  物料编码, 数量, 结算单价 from 其他入库子表 a
               left join 其他出入库申请主表 b on a.出入库申请单号 = b.出入库申请单号
               where a.生效日期 > '{0}' and a.生效日期 < '{1}' and 物料编码='{2}' and 存货核算标记 = 0
               and 原因分类 <> '调拨入库')tt group by 物料编码", d1, d2, str_物料编码);
                DataTable t_other = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                decimal dec_other = 0;
                if (t_other.Rows.Count > 0)
                {
                    if (t_other.Rows[0]["金额"] != null && t_other.Rows[0]["金额"] != DBNull.Value && t_other.Rows[0]["金额"].ToString() != "")
                    {
                        dec_other = Convert.ToDecimal(t_other.Rows[0]["金额"]);
                    }
                }*/
                //发料
                //s = string.Format(@"select  * from (select 物料编码,相关单号,-SUM(实效数量 )实效数量 from 仓库出入库明细表  where 出入库时间 >'{0}'
                //and 出入库时间 <'{1}' and 明细类型 in ('领料出库','工单退料') and 物料编码='{2}' group by  物料编码,相关单号)x 
                //where 实效数量<>0 ", d1, d2, str_物料编码);
                //DataTable t_发料 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                //Decimal dec_发料 = 0;
                //foreach (DataRow dr in t_发料.Rows)
                //{
                //    DataRow[] rr = t_耗用.Select(string.Format("生产工单号='{0}' and 子项编码='{1}'", dr["相关单号"], str_物料编码));
                //    if (rr.Length > 0)
                //    {
                //        dec_发料 += Math.Round(Convert.ToDecimal(rr[0]["发出单价"]) * Convert.ToDecimal(dr["实效数量"]), 2, MidpointRounding.AwayFromZero);
                //    }
                //    else //耗用里面没有 有可能是领了直接关闭工单 但是因为一开始的程序没有限制 这部分 料未退完 算成本时没有算到
                //    {
                //        decimal d = 0;
                //        DataRow[] pp = t_成本.Select(string.Format("物料编码='{0}'", str_物料编码));
                //        if (pp.Length > 0)
                //        {
                //            d = Convert.ToDecimal(pp[0]["发出单价"]);
                //        }
                //        dec_发料 += Math.Round(d * Convert.ToDecimal(dr["实效数量"]), 2, MidpointRounding.AwayFromZero);
                //    }
                //}

                //DateTime d0 = d1.AddMonths(-1);
                // s = string.Format(@" select   sum(round(-实效数量*单价,2))金额  from 仓库出入库明细表 where 出入库时间 >'{0}'
                //and 出入库时间 <'{1}' and 物料编码='{2}' and 明细类型 in ('领料出库','工单退料','工单关闭退料','返工退料')", d1, d2, str_物料编码);
                // DataTable t_发料 = CZMaster.MasterSQL.Get_DataTable(s, strconn);

                Decimal dec_发料 = 0;
                // if (t_发料.Rows.Count > 0)
                // {
                //     if (t_发料.Rows[0]["金额"] != null && t_发料.Rows[0]["金额"] != DBNull.Value && t_发料.Rows[0]["金额"].ToString() != "")
                //     {
                //         dec_发料 = Convert.ToDecimal(t_发料.Rows[0]["金额"]);
                //     }
                // }
                DataRow[] r_发料 = t_发料汇总.Select($"物料编码='{str_物料编码}'");
                if (r_发料.Length > 0)
                {
                    if (r_发料[0]["金额"] != null && r_发料[0]["金额"] != DBNull.Value && r_发料[0]["金额"].ToString() != "")
                    {
                        dec_发料 = Convert.ToDecimal(r_发料[0]["金额"]);
                    }
                }

                //拆单的 没有生产其他出入库单据  所以需要单独取
                // s = string.Format(@" select 物料编码,SUM(round(金额,2)) 金额 from (
                //select 物料编码,ROUND(-单价*实效数量,2)金额 from 仓库出入库明细表  where 出入库时间 >'{0}' and 出入库时间 <'{1}'
                //and 明细类型 ='拆单申请出库' and 物料编码='{2}' and  实效数量<>0)fuck group by 物料编码", d1, d2, str_物料编码);
                // DataTable t_chaid = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                DataRow []r_cd= t_chaid.Select($"物料编码='{str_物料编码}'");
                decimal dec_chaid = 0;
                if (r_cd.Length > 0)
                {
                    if (r_cd[0]["金额"] != null && r_cd[0]["金额"] != DBNull.Value && r_cd[0]["金额"].ToString() != "")
                    {
                        dec_chaid = Convert.ToDecimal(r_cd[0]["金额"]);
                    }
                }

   //             s = string.Format(@"select  a.物料编码,sum(round(实效数量*发出单价,2)) 金额 from 仓库出入库明细表  a
   //left join 销售记录成品出库单明细表 b on a.明细号=b.成品出库单明细号 
   //where  明细类型='销售退货' and   出入库时间>'{0}' and  出入库时间<'{1}' 
   //and a.物料编码='{2}'  group by a.物料编码", d1, d2, str_物料编码);
   //             DataTable t_th = CZMaster.MasterSQL.Get_DataTable(s, strconn);
   //             decimal dec_th = 0;
   //             if (t_th.Rows.Count > 0)
   //             {
   //                 if (t_th.Rows[0]["金额"] != null && t_th.Rows[0]["金额"] != DBNull.Value && t_th.Rows[0]["金额"].ToString() != "")
   //                 {
   //                     dec_th = Convert.ToDecimal(t_th.Rows[0]["金额"]);
   //                 }
   //             }

                DataRow[] r_th = t_th.Select($"物料编码='{str_物料编码}'");
                decimal dec_th = 0;
                if (r_th.Length > 0)
                {
                    if (r_th[0]["金额"] != null && r_th[0]["金额"] != DBNull.Value && r_th[0]["金额"].ToString() != "")
                    {
                        dec_th = Convert.ToDecimal(r_th[0]["金额"]);
                    }
                }

                ///19-8-14 加入形态转换
               // s = string.Format(@" select 物料编码,SUM(金额)金额 from (
               //select 物料编码,ROUND(-单价*实效数量,2)金额 from 仓库出入库明细表  where 出入库时间 >'{0}' and 出入库时间 <'{1}'
               //and 明细类型 ='形态转换出库' and 物料编码='{2}' and  实效数量<>0)fuck group by 物料编码", d1, d2, str_物料编码);
               // DataTable t_xtzhrk = CZMaster.MasterSQL.Get_DataTable(s, strconn);
               // decimal dec_xtzh = 0;
               // if (t_xtzhrk.Rows.Count > 0)
               // {
               //     if (t_xtzhrk.Rows[0]["金额"] != null && t_xtzhrk.Rows[0]["金额"] != DBNull.Value && t_xtzhrk.Rows[0]["金额"].ToString() != "")
               //     {
               //         dec_xtzh = Convert.ToDecimal(t_xtzhrk.Rows[0]["金额"]);
               //     }
               // }
                DataRow[] r_xtzhck = t_xtzhck.Select($"物料编码='{str_物料编码}'");
                decimal dec_xtzh = 0;
                if (r_xtzhck.Length > 0)
                {
                    if (r_xtzhck[0]["金额"] != null && r_xtzhck[0]["金额"] != DBNull.Value && r_xtzhck[0]["金额"].ToString() != "")
                    {
                        dec_xtzh = Convert.ToDecimal(r_xtzhck[0]["金额"]);
                    }
                }

                decimal decx = 0;
                //20-4-10 换成判断委外
               // if (str_物料编码.Substring(0, 4) == "0102")  dec_th = 0;  //0102 入库金额已经算到了 红字回冲的和退货入库，涉及委外
                if (bl_委外) dec_th = 0;  //0102 入库金额已经算到了 红字回冲的和退货入库，涉及委外


                // decx = dec_销售出库 + dec_其他出库 + dec_other + dec_发料 + dec_chaid - dec_th + dec_xtzh;
                decx = dec_销售出库 + dec_其他出库 + dec_发料 + dec_chaid - dec_th + dec_xtzh;
                if (dec == 0) ss.dec_单价 = 0;
                else ss.dec_单价 = Math.Round(decx / dec, 6, MidpointRounding.AwayFromZero);
                ss.dec_金额 = decx;
                ss.dec_数量 = dec;
            }
            else  //采购库,其它入库 //19-7-8   --- 19-8-14 形态转换入库
            {
                //入库在仓库出入库明细中全部为正
                //string s = string.Format(@"select  sum(实效数量) as 数量  from 仓库出入库明细表 where 出库入库='入库' 
                // and 出入库时间 >'{0}'  and 出入库时间 <'{1}' and 物料编码='{2}' and 明细类型 <> '归还入库'  ", d1, d2, str_物料编码);
                //using (SqlDataAdapter da_1 = new SqlDataAdapter(s, strconn))
                //{
                //    DataTable dt = new DataTable();
                //    da_1.Fill(dt);
                //    if (dt != null && dt.Rows.Count > 0)
                //    {
                //        if (dt.Rows[0]["数量"] != null)
                //        {
                //            try
                //            {
                //                Convert.ToDecimal(dt.Rows[0]["数量"]);
                //            }
                //            catch (Exception)
                //            {
                //                dt.Rows[0]["数量"] = 0;
                //            }
                //            dec = dec + Convert.ToDecimal(dt.Rows[0]["数量"]);
                //            //if (str_物料编码 == "01020200000116") dec += 1109;
                //            //else if (str_物料编码 == "01020200000104")
                //            //{
                //            //    dec += 28;
                //            //}
                //            //else if (str_物料编码 == "01020200000105")
                //            //{
                //            //    dec += 13;
                //            //}
                //        }
                //    }
                //}
                DataRow[] r_入汇总 = t_入汇总.Select($"物料编码='{str_物料编码}'");
                if (r_入汇总.Length > 0)
                {
                    if (r_入汇总[0]["数量"] != null)
                    {
                        try
                        {
                            Convert.ToDecimal(r_入汇总[0]["数量"]);
                        }
                        catch (Exception)
                        {
                            r_入汇总[0]["数量"] = 0;
                        }
                        dec = dec + Convert.ToDecimal(r_入汇总[0]["数量"]);
                    }
                }



                // select  x.物料编码,isnull(入库金额,0)+isnull(gg,0) 金额,isnull(gg,0) 冲暂估 from (
                //           s = string.Format(@"select  物料编码,sum(ISNULL(开票金额,0))+sum(round((入库量-已开票数量)*采购未税单价,2)) 入库金额  from( 
                //select  a.入库单号,a.入库明细号 ,a.入库POS,a.物料编码,a.入库量,case when LEFT(入库单号,2)='DW' then  CONVERT(decimal(18,6),a.备注6) when  a.采购单明细号='' and a.备注1='采购退货' then  CONVERT(decimal(18,6),a.备注6)  else  b.未税单价  end as 采购未税单价
                //,isnull(开票数量,0)已开票数量,a.供应商 ,a.生效日期 as 入库日期,开票金额  from 采购记录采购单入库明细 a
                //left join 采购记录采购单明细表 b  on  a.采购单明细号=b.采购明细号 
                //left join ( select  入库明细号,SUM(开票数量)开票数量,SUM(未税金额)开票金额 from 采购记录采购开票通知单明细表
                //where  生效=1 and 发票确认 =1 and 发票确认日期 <'{1}'  group by 入库明细号  )ljkp  on ljkp.入库明细号=a.入库明细号
                //where a.生效=1  and  a.作废=0 and a.生效日期>'{0}'  and a.生效日期<'{1}' and a.物料编码='{2}' )c  group by 物料编码  ", d1, d2, str_物料编码);
                //           DataTable t_crk = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                decimal dec_crk = 0;
                decimal dec_冲暂估 = 0;
                DataRow[] r_crk = t_采购入.Select($"物料编码='{str_物料编码}'");
          
                if(r_crk.Length>0)
                {
                    if (r_crk[0]["入库金额"] != null && r_crk[0]["入库金额"] != DBNull.Value && r_crk[0]["入库金额"].ToString() != "")
                    {
                        dec_crk = Convert.ToDecimal(r_crk[0]["入库金额"]);
                    
                    }
                }
                //if (t_crk.Rows.Count > 0)
                //{
                //    if (t_crk.Rows[0]["入库金额"] != null && t_crk.Rows[0]["入库金额"] != DBNull.Value && t_crk.Rows[0]["入库金额"].ToString() != "")
                //    {
                //        dec_crk = Convert.ToDecimal(t_crk.Rows[0]["入库金额"]);
                //        //dec_冲暂估 = Convert.ToDecimal(t_crk.Rows[0]["冲暂估"]);
                //    }
                //}
     //           s = string.Format(@"select  物料编码,sum(round(开票金额-已开票数量*采购未税单价,2))gg  from( 
     //select  a.入库单号,a.入库明细号 ,a.入库POS,a.物料编码,a.入库量,case when LEFT(入库单号,2)='DW' then  CONVERT(decimal(18,6),a.备注6)  when  a.采购单明细号='' and a.备注1='采购退货' then  CONVERT(decimal(18,6),a.备注6)  else  b.未税单价  end as 采购未税单价
     //,isnull(开票数量,0)已开票数量,a.供应商 ,a.生效日期 as 入库日期,开票金额  from 采购记录采购单入库明细 a
     //left join 采购记录采购单明细表 b  on  a.采购单明细号=b.采购明细号 
     //left join ( select  入库明细号,SUM(开票数量)开票数量,SUM(未税金额)开票金额 from 采购记录采购开票通知单明细表
     //where  生效=1 and 发票确认 =1 and 发票确认日期>'{0}'  and 发票确认日期 <'{1}'  group by 入库明细号  )ljkp  on ljkp.入库明细号=a.入库明细号
     //where a.生效=1  and  a.作废=0 and  a.生效日期<'{0}'  )c  where  abs(已开票数量)>0 and 物料编码='{2}' group by 物料编码  ", d1, d2, str_物料编码);
     //           t_crk = CZMaster.MasterSQL.Get_DataTable(s, strconn);
     //           if (t_crk.Rows.Count > 0)
     //           {
     //               if (t_crk.Rows[0]["gg"] != null && t_crk.Rows[0]["gg"] != DBNull.Value && t_crk.Rows[0]["gg"].ToString() != "")
     //               {
     //                   dec_冲暂估 = Convert.ToDecimal(t_crk.Rows[0]["gg"]);
     //                   dec_crk += dec_冲暂估;
     //               }
     //           }
                DataRow[] r_zg= t_采购冲暂估.Select($"物料编码='{str_物料编码}'");

                if (r_zg.Length > 0)
                {
                    if (r_zg[0]["gg"] != null && r_zg[0]["gg"] != DBNull.Value && r_zg[0]["gg"].ToString() != "")
                    {
                        dec_冲暂估 = Convert.ToDecimal(r_zg[0]["gg"]);
                        dec_crk += dec_冲暂估;
                    }
                }

                //2020-4-10 这个先不动
                s = string.Format(@"select sum(ROUND(数量* 结算单价,2))金额 from  其他入库子表 a
                left join 其他出入库申请主表 b on a.出入库申请单号 = b.出入库申请单号
                where a.生效日期 > '{0}' and a.生效日期 < '{1}' and  原因分类 = '调拨入库'  
                and 物料编码 = '{2}'", d1, d2, str_物料编码);
                DataTable t_db = CZMaster.MasterSQL.Get_DataTable(s, strconn);

                s = string.Format(@"select sum(ROUND(数量* 结算单价,2))金额 from  其他入库子表 a
                left join 其他出入库申请主表 b on a.出入库申请单号 = b.出入库申请单号
                where a.生效日期 > '{0}' and a.生效日期 < '{1}' and  存货核算标记 = 1  
                and 物料编码 = '{2}'", d1, d2, str_物料编码);
                DataTable t_db2 = CZMaster.MasterSQL.Get_DataTable(s, strconn);

                decimal dec_db = 0;

                decimal dec_db2 = 0;

                decimal decx = 0;

                if (t_db.Rows.Count > 0)
                {
                    if (t_db.Rows[0]["金额"] != null && t_db.Rows[0]["金额"] != DBNull.Value && t_db.Rows[0]["金额"].ToString() != "")
                    {
                        dec_db = Convert.ToDecimal(t_db.Rows[0]["金额"]);
                    }
                }
                if (t_db2.Rows.Count > 0)
                {
                    if (t_db2.Rows[0]["金额"] != null && t_db2.Rows[0]["金额"] != DBNull.Value && t_db2.Rows[0]["金额"].ToString() != "")
                    {
                        dec_db2 = Convert.ToDecimal(t_db2.Rows[0]["金额"]);
                    }
                }

                #region   //6月再按这个算 弃用
                ////成本里面 的累计收入金额 + 其他入库调拨的 累计金额 就是 结存的 本月入库金额

                //s = string.Format(@"select sum(ROUND(数量* 结算单价,2))金额 from  其他入库子表 a
                //left join 其他出入库申请主表 b on a.出入库申请单号 = b.出入库申请单号
                //where a.生效日期 > '{0}' and a.生效日期 < '{1}' and 原因分类 = '调拨入库'
                //and 物料编码 = '{2}'", d1, d2, str_物料编码);
                //DataTable t_db = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                //decimal dec_db = 0;
                //decimal decx = 0;

                //if (t_db.Rows.Count > 0)
                //{
                //    if (t_db.Rows[0]["金额"] != null && t_db.Rows[0]["金额"] != DBNull.Value && t_db.Rows[0]["金额"].ToString() != "")
                //    {
                //        dec_db = Convert.ToDecimal(t_db.Rows[0]["金额"]);
                //    }
                //}
                //DataRow[] rt = t_成本.Select(string.Format("物料编码='{0}'", str_物料编码));
                //if (rt.Length > 0)
                //{
                //    decx = Convert.ToDecimal(rt[0]["累计入库金额"]);
                //}
                #endregion

                //
                //拆单的 没有生产其他出入库单据  所以需要单独取
               // s = string.Format(@" select 物料编码,SUM(金额)金额 from (
               //select 物料编码,ROUND(单价*实效数量,2)金额 from 仓库出入库明细表  where 出入库时间 >'{0}' and 出入库时间 <'{1}'
               //and 明细类型 ='拆单申请入库' and 物料编码='{2}' and  实效数量<>0)fuck group by 物料编码", d1, d2, str_物料编码);
               // DataTable t_chaid = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                decimal dec_chaid = 0;
               // if (t_chaid.Rows.Count > 0)
               // {
               //     if (t_chaid.Rows[0]["金额"] != null && t_chaid.Rows[0]["金额"] != DBNull.Value && t_chaid.Rows[0]["金额"].ToString() != "")
               //     {
               //         dec_chaid = Convert.ToDecimal(t_chaid.Rows[0]["金额"]);
               //     }
               // }
                DataRow[] r_cdr = t_chaidr.Select($"物料编码='{str_物料编码}'");
                if (r_cdr.Length > 0)
                {
                    if (r_cdr[0]["金额"] != null && r_cdr[0]["金额"] != DBNull.Value && r_cdr[0]["金额"].ToString() != "")
                    {
                        dec_chaid = Convert.ToDecimal(r_cdr[0]["金额"]);
                    }
                }
                ///19-8-14 加入形态转换
               // s = string.Format(@" select 物料编码,SUM(金额)金额 from (
               //select 物料编码,ROUND(单价*实效数量,2)金额 from 仓库出入库明细表  where 出入库时间 >'{0}' and 出入库时间 <'{1}'
               //and 明细类型 ='形态转换入库' and 物料编码='{2}' and  实效数量<>0)fuck group by 物料编码", d1, d2, str_物料编码);
               // DataTable t_xtzhrk = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                decimal dec_xtzh = 0;
                //if (t_xtzhrk.Rows.Count > 0)
                //{
                //    if (t_xtzhrk.Rows[0]["金额"] != null && t_xtzhrk.Rows[0]["金额"] != DBNull.Value && t_xtzhrk.Rows[0]["金额"].ToString() != "")
                //    {
                //        dec_xtzh = Convert.ToDecimal(t_xtzhrk.Rows[0]["金额"]);
                //    }
                //}
                DataRow[] r_xzr = t_xtzhrk.Select($"物料编码='{str_物料编码}'");
                if (r_xzr.Length > 0)
                {
                    if (r_xzr[0]["金额"] != null && r_xzr[0]["金额"] != DBNull.Value && r_xzr[0]["金额"].ToString() != "")
                    {
                        dec_xtzh = Convert.ToDecimal(r_xzr[0]["金额"]);
                    }
                }
                if(bl_委外) //20-4-1 判断是否委外 
                {
                    //委外的取成本表里面的 不要再取 采购入库 和  其他入库中 核算标记为1 的   拆单和形态转换的 都在
                    //此处已包含了 材料出库红字的  对应的 出库的里面 需要去除 8-18
                    dec_crk = 0;
                    dec_db2 = 0;
                    dec_xtzh = 0;
                    dec_chaid = 0;
                    if (dec_crk != 0) dec_crk = 0;
                    DataRow[] rt = t_成本.Select(string.Format("物料编码='{0}'", str_物料编码));
                    if (rt.Length > 0)
                    {
                        decx = Convert.ToDecimal(rt[0]["累计入库金额"]);
                        //decx = decx + dec_冲暂估; //委外的暂估也冲了
                    }
                }
                else if ( str_物料编码.Substring(0, 2) == "05" || str_物料编码.Substring(0, 2) == "10")
                {
                    ////成本半成品取  成本表里面的 不要再取 采购入库 和  其他入库中 核算标记为1 的 
                    //dec_crk = 0;
                    //dec_db2 = 0;
                    //DataRow[] rt = t_成本.Select(string.Format("物料编码='{0}'", str_物料编码));
                    //if (rt.Length > 0)
                    //{
                    //   decx = Convert.ToDecimal(rt[0]["累计入库金额"]);
                    //}
                    DataRow[] yr = t_工单.Select(string.Format("物料编码='{0}'", str_物料编码));
                    foreach (DataRow dr in yr)
                    {
                        decx += Convert.ToDecimal(dr["总金额"]);
                    }
                }

                //else if (str_物料编码.Substring(0, 4) == "0102" || str_物料编码 == "01011002010240") //这里我估计应该要取 委外=1 
                //{
                //    //委外的取成本表里面的 不要再取 采购入库 和  其他入库中 核算标记为1 的   拆单和形态转换的 都在
                //    //此处已包含了 材料出库红字的  对应的 出库的里面 需要去除 8-18
                //    dec_crk = 0;
                //    dec_db2 = 0;
                //    dec_xtzh = 0;
                //    dec_chaid = 0;
                //    if (dec_crk != 0) dec_crk = 0;
                //    DataRow[] rt = t_成本.Select(string.Format("物料编码='{0}'", str_物料编码));
                //    if (rt.Length > 0)
                //    {
                //        decx = Convert.ToDecimal(rt[0]["累计入库金额"]);
                //        //decx = decx + dec_冲暂估; //委外的暂估也冲了
                //    }
                //}
                //if (str_物料编码 == "01020200000116")
                //{
                //    decx += (decimal)3682.23;
                //}
                //else if (str_物料编码 == "01020200000104")
                //{

                //    decx += (decimal)1270.94;
                //}
                //else if (str_物料编码 == "01020200000105")
                //{
                //    decx += (decimal)505.85;
                //}
                decx = decx + dec_crk + dec_db + dec_chaid + dec_db2 + dec_xtzh;
                if (dec == 0) ss.dec_单价 = 0;
                else ss.dec_单价 = Math.Round(decx / dec, 6, MidpointRounding.AwayFromZero);
                ss.dec_金额 = decx;
                ss.dec_数量 = dec;
            }
            return ss;
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (MessageBox.Show(string.Format("确认开始计算", Convert.ToInt32(barEditItem1.EditValue)), "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    if (bl_计算 || bl_计算2) throw new Exception("正在计算中,请稍候");
                    label1.Visible = true;
                    label1.Text = "正在计算中，由于数据量过大，请耐心等待几分钟....";

                    bl_计算 = true;
                    bl_计算2 = true;

                    //DateTime time = System.DateTime.Today;
                    //界面选择月份   
                    DateTime time = new DateTime(Convert.ToInt32(barEditItem2.EditValue), Convert.ToInt32(barEditItem1.EditValue), 1); //2019-5-1
                    DateTime d2 = time.AddMonths(1).AddSeconds(-1); //2019-5-31 23：59：59
                    DateTime d1 = time;
                    DateTime t3 = time.AddMonths(-1);
                    fun_载入物料(t3.Year, t3.Month);
                    ////DateTime d1 = new DateTime(time.Year, time.Month, 1);
                    ////DateTime d2 = d1.AddMonths(1).AddDays(-1);
                    ////DateTime d1 = new DateTime(2016, 12, 1);
                    ////DateTime d2 = new DateTime(2017, 1, 1);
                    //total = list_m.Rows.Count;
                    //int x1 = total / 2;
                    //DataTable dt1 = list_m.Clone();
                    //DataTable dt2 = list_m.Clone();
                    //int j = 0;
                    //foreach (DataRow dr in list_m.Rows)
                    //{
                    //    if (j++ <= x1)
                    //    {
                    //        dt1.ImportRow(dr);
                    //    }
                    //    else
                    //    {
                    //        dt2.ImportRow(dr);
                    //    }
                    //}
                    Thread th = new Thread(() =>
                    {

                        c_cal(list_m, d1, d2, label1);

                        bl_计算 = false;
                        //if (!(bl_计算 & bl_计算2))
                        //{


                        BeginInvoke(new MethodInvoker(() =>
                             {
                                 label1.Text = "计算完成";
                                 label2.Text = "";

                             }));

                        //}
                    });
                    th.Start();

                    //Thread th2 = new Thread(() =>
                    //{
                    //    c_cal(dt2, d1, d2, label2);

                    //    bl_计算2 = false;

                    //    if (!(bl_计算 & bl_计算2))
                    //    {

                    //        BeginInvoke(new MethodInvoker(() =>
                    //        {
                    //            label1.Text = "计算完成";
                    //            label2.Text = "";

                    //        }));
                    //    }
                    //});
                    //th2.Start();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                // CZMaster.MasterLog.WriteLog(ex.Message, "");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt_list_物料"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="x"> 分成两个线程处理 标记</param>
        private void c_cal(DataTable dt_list_物料, DateTime t1, DateTime t2, Label li)
        {
            int total = dt_list_物料.Rows.Count;
            int i = 0;

            string s = $@"select 物料编码,sum(实效数量*(-1)) 数量 from 仓库出入库明细表 where 出库入库 = '出库' and 出入库时间 > '{t1}'
                and 出入库时间<'{t2}'   and 明细类型<> '借用出库' group by 物料编码";
            DataTable t_出明细 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            DataColumn[] pk = new DataColumn[1];
            pk[0] = t_出明细.Columns["物料编码"];
            t_出明细.PrimaryKey = pk;


            s = $@" select  物料编码,sum(round(-实效数量*单价,2))金额  from 仓库出入库明细表 where 出入库时间 >'{t1}'
            and 出入库时间 <'{t2}'  and 明细类型 in ('领料出库','工单退料','工单关闭退料','返工退料') group by 物料编码";
            DataTable t_发料 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
             pk = new DataColumn[1];
            pk[0] = t_发料.Columns["物料编码"];
            t_发料.PrimaryKey = pk;

            s = $@"select 物料编码,sum(round(出库数量*发出单价,2))金额 from 销售记录成品出库单明细表 
              where 生效日期>'{t1}' and 生效日期<'{t2}'   and 退货标识<>'是'group by  物料编码";
            DataTable t_销售出库 = CZMaster.MasterSQL.Get_DataTable(s, strconn);


            //拆单的 没有生产其他出入库单据  所以需要单独取
            s = $@"select 物料编码,SUM(round(金额,2)) 金额 from (
               select 物料编码,ROUND(-单价*实效数量,2)金额 from 仓库出入库明细表  where 出入库时间 >'{t1}' and 出入库时间 <'{t2}'
               and 明细类型 ='拆单申请出库' and  实效数量<>0)fuck group by 物料编码";
            DataTable t_chaidc = CZMaster.MasterSQL.Get_DataTable(s, strconn);

            s = $@" select 物料编码,SUM(金额)金额 from (
               select 物料编码,ROUND(-单价*实效数量,2)金额 from 仓库出入库明细表  where 出入库时间 >'{t1}' and 出入库时间 <'{t2}'
               and 明细类型 ='形态转换出库'  and  实效数量<>0)fuck group by 物料编码";
            DataTable t_xtzhck = CZMaster.MasterSQL.Get_DataTable(s, strconn);

            s = $@"select  a.物料编码,sum(round(实效数量*发出单价,2)) 金额 from 仓库出入库明细表  a
                left join 销售记录成品出库单明细表 b on a.明细号=b.成品出库单明细号 
              where  明细类型='销售退货' and   出入库时间>'{t1}' and  出入库时间<'{t2}' group by a.物料编码";
            DataTable t_th = CZMaster.MasterSQL.Get_DataTable(s, strconn);

            DataSet ds_出 = new DataSet();
            ds_出.Tables.Add(t_出明细); //数量
            ds_出.Tables.Add(t_发料); //生产发料汇总金额
            ds_出.Tables.Add(t_销售出库); // 销售出库汇总金额
            ds_出.Tables.Add(t_chaidc); //拆单出库汇总金额
            ds_出.Tables.Add(t_th);//销售退货汇总金额
            ds_出.Tables.Add(t_xtzhck); //形态转换出库汇总金额


             s =  $@"select  物料编码,sum(实效数量) as 数量  from 仓库出入库明细表 where 出库入库='入库' 
             and 出入库时间 >'{t1}'  and 出入库时间 <'{t2}'  and 明细类型 <> '归还入库' group by 物料编码 "   ;
            DataTable t_入明细= CZMaster.MasterSQL.Get_DataTable(s, strconn);
             pk = new DataColumn[1];
            pk[0] = t_入明细.Columns["物料编码"];
            t_入明细.PrimaryKey = pk;

            s = $@"select  物料编码,sum(ISNULL(开票金额,0))+sum(round((入库量-已开票数量)*采购未税单价,2)) 入库金额  from( 
                select  a.入库单号,a.入库明细号 ,a.入库POS,a.物料编码,a.入库量,case when LEFT(入库单号,2)='DW' then  CONVERT(decimal(18,6),a.备注6) 
          when  a.采购单明细号='' and a.备注1='采购退货' then  CONVERT(decimal(18,6),a.备注6)  else  b.未税单价  end as 采购未税单价
                ,isnull(开票数量,0)已开票数量,a.供应商 ,a.生效日期 as 入库日期,开票金额  from 采购记录采购单入库明细 a
                left join 采购记录采购单明细表 b  on  a.采购单明细号=b.采购明细号 
                left join ( select  入库明细号,SUM(开票数量)开票数量,SUM(未税金额)开票金额 from 采购记录采购开票通知单明细表
                where  生效=1 and 发票确认 =1 and 发票确认日期 <'{t2}'  group by 入库明细号  )ljkp  on ljkp.入库明细号=a.入库明细号
                where a.生效=1  and  a.作废=0 and a.生效日期>'{t1}'  and a.生效日期<'{t2}' )c  group by 物料编码  ";
            DataTable t_采购入库 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
           pk = new DataColumn[1];
            pk[0] = t_采购入库.Columns["物料编码"];
            t_采购入库.PrimaryKey = pk;

            s =  $@"select  物料编码,sum(round(开票金额-已开票数量*采购未税单价,2))gg  from( 
            select  a.入库单号,a.入库明细号 ,a.入库POS,a.物料编码,a.入库量,case when LEFT(入库单号,2)='DW' then  CONVERT(decimal(18,6),a.备注6)  
            when  a.采购单明细号='' and a.备注1='采购退货' then  CONVERT(decimal(18,6),a.备注6)  else  b.未税单价  end as 采购未税单价
            ,isnull(开票数量,0)已开票数量,a.供应商 ,a.生效日期 as 入库日期,开票金额  from 采购记录采购单入库明细 a
            left join 采购记录采购单明细表 b  on  a.采购单明细号=b.采购明细号 
            left join ( select  入库明细号,SUM(开票数量)开票数量,SUM(未税金额)开票金额 from 采购记录采购开票通知单明细表
            where  生效=1 and 发票确认 =1 and 发票确认日期>'{t1}'  and 发票确认日期 <'{t2}'  group by 入库明细号)ljkp  on ljkp.入库明细号=a.入库明细号
            where a.生效=1  and  a.作废=0 and  a.生效日期<'{t1}'  )c  where  abs(已开票数量)>0  group by 物料编码  " ;
            DataTable t_冲暂估 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            pk = new DataColumn[1];
            pk[0] = t_冲暂估.Columns["物料编码"];
            t_冲暂估.PrimaryKey = pk;

            s =  $@" select 物料编码,SUM(金额)金额 from (
            select 物料编码,ROUND(单价*实效数量,2)金额 from 仓库出入库明细表  where 出入库时间 >'{t1}' and 出入库时间 <'{t2}'
            and 明细类型 ='拆单申请入库'  and  实效数量<>0)fuck group by 物料编码" ;
            DataTable t_chaidr = CZMaster.MasterSQL.Get_DataTable(s, strconn);

            s = $@" select 物料编码,SUM(金额)金额 from (
            select 物料编码,ROUND(单价*实效数量,2)金额 from 仓库出入库明细表  where 出入库时间 >'{t1}' and 出入库时间 <'{t2}'
            and 明细类型 ='形态转换入库' and  实效数量<>0)fuck group by 物料编码" ;
            DataTable t_xtzhrk = CZMaster.MasterSQL.Get_DataTable(s, strconn);

            DataSet ds_入 = new DataSet();
            ds_入.Tables.Add(t_入明细); //入库数量
            ds_入.Tables.Add(t_采购入库); //采购入库汇总金额
            ds_入.Tables.Add(t_冲暂估); //  
            ds_入.Tables.Add(t_chaidr); //拆单入库汇总金额
            ds_入.Tables.Add(t_xtzhrk); //形态转换入库汇总金额
 

            foreach (DataRow dr in dt_list_物料.Rows)
            {
                // if (dr["物料编码"].ToString().Substring(0, 6) != "010301") continue;

                BeginInvoke(new MethodInvoker(() =>
                {
                    li.Text = string.Format("计算中：{0}/{1}", i++, total);
                }));
                result ss = new result();
                ss = fun_计算(t1, t2, "入库", dr["物料编码"].ToString().Trim(), ds_出, ds_入,Convert.ToBoolean(dr["委外"]));
                //dr["入库数量"] = dec_入;
                result ss1 = new result();
                ss1 = fun_计算(t1, t2, "出库", dr["物料编码"].ToString().Trim(), ds_出, ds_入,Convert.ToBoolean(dr["委外"]));
                // dr["出库数量"] = dec_出;
                DataRow[] r_上月结存 = dt_上月结存.Select(string.Format("物料编码='{0}'", dr["物料编码"].ToString()));
                decimal dec_上月结存数量 = 0;
                decimal dec_上月结存金额 = 0;
                if (r_上月结存.Length == 0)
                {
                    if (ss.dec_数量 == 0 && ss1.dec_数量 == 0)
                    {
                        //上月没有 本月也没有发出收入 则
                        // continue;
                        //2019-8-16 
                        dec_上月结存数量 = 0;
                        dec_上月结存金额 = 0;
                    }
                }
                else
                {
                    try
                    {
                        dec_上月结存数量 = Convert.ToDecimal(r_上月结存[0]["本月结转数量"]);
                        dec_上月结存金额 = Convert.ToDecimal(r_上月结存[0]["本月结转金额"]);
                    }
                    catch
                    {

                    }

                }
                decimal d_本月结转数量 = dec_上月结存数量 + ss.dec_数量 - ss1.dec_数量;

                //DataRow[] r_成本 = t_成本.Select(string.Format("物料编码='{0}'", dr["物料编码"].ToString()));
                //decimal dec_收入单价 = 0;
                //decimal dec_发出单价 = 0;
                //if (r_成本.Length > 0)
                //{
                //    dec_收入单价 = Convert.ToDecimal(r_成本[0]["收入单价"]);
                //    dec_发出单价 = Convert.ToDecimal(r_成本[0]["发出单价"]);
                //}
                DataRow r = dtQ.NewRow();
                dtQ.Rows.Add(r);
                //r.ItemArray = dr.ItemArray;
                r["物料编码"] = dr["物料编码"];
                //r["物料名称"] = dr["物料名称"];
                //r["物料类型"] = dr["物料类型"];
                r["入库数量"] = ss.dec_数量;
                r["出库数量"] = ss1.dec_数量;
                r["入库金额"] = ss.dec_金额;
                r["出库金额"] = ss1.dec_金额;
                //r["年"] = System.DateTime.Now.Year.ToString();
                //r["月"] = System.DateTime.Now.Month.ToString();
                r["年"] = t1.Year;
                r["月"] = t1.Month;
                r["GUID"] = System.Guid.NewGuid();
                r["上月结转数量"] = dec_上月结存数量;
                r["上月结转金额"] = dec_上月结存金额;
                r["发出单价"] = ss1.dec_单价;
                r["收入单价"] = ss.dec_单价;

                //r["本月结转金额"] = Convert.ToDecimal(dr["本月结转金额"])+Convert.ToDecimal(M_本月结转金额);
                r["本月结转数量"] = d_本月结转数量;
                r["本月结转金额"] = dec_上月结存金额 + Convert.ToDecimal(r["入库金额"]) - Convert.ToDecimal(r["出库金额"]);
                if (d_本月结转数量 == 0) r["结存单价"] = 0;
                else
                    r["结存单价"] = Math.Round(Convert.ToDecimal(r["本月结转金额"]) / d_本月结转数量, 6, MidpointRounding.AwayFromZero);
                r["结算日期"] = t2;
                decimal a = Convert.ToDecimal(r["上月结转数量"]) - Convert.ToDecimal(r["本月结转数量"]) + Convert.ToDecimal(r["入库数量"]) - Convert.ToDecimal(r["出库数量"]);
                if (a > 0)
                {
                    r["差异数量"] = a;
                }
                else
                {
                    r["差异数量"] = -a;
                }
                decimal b = Convert.ToDecimal(r["上月结转金额"]) + Convert.ToDecimal(r["入库金额"]) - Convert.ToDecimal(r["出库金额"])
                    - Convert.ToDecimal(r["本月结转金额"]);
                if (b > 0)
                {
                    r["差异金额"] = b;

                }
                else
                {
                    r["差异金额"] = -b;
                }
            }
        }


#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                int year = Convert.ToInt32(barEditItem2.EditValue);
                int mon = Convert.ToInt32(barEditItem1.EditValue);
                DateTime t1 = new DateTime(year, mon, 1).AddMonths(1).AddHours(-1);

                if (bl_计算) throw new Exception("正在计算中...");
                label1.Visible = true;
                label1.Text = "正在保存中，由于数据量过大，请耐心等待几分钟...";
                string sql = "select * from 仓库月出入库结转表 where 1<>1";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dtQ);
                if (dtQ.Rows.Count > 0)
                {
                    sql = $@" delete 财务即时库存记录 where   时间>'{new DateTime(year, mon, 1)}' 
                   insert   into [财务即时库存记录] (生产工单号,产品编码,产品型号,产品名称,车间名称,子项编码,子项名称,本期领用数量
                        ,子项型号,子项单价,耗用单价,生产数量,时间,本期入库数量,在制品,在制金额)
                        select gd.生产工单号,产品编码,base.规格型号,base.物料名称,gd.车间名称,zx.物料编码,zx.物料名称,子项当期领用,zx.规格型号,发出单价
                        ,耗用单价,gd.生产数量,'{t1}',本期入库数量,子项在制数,ROUND(子项在制数*耗用单价,2)    from  C_工单当期耗用
                            left join 生产记录生产工单表 gd on gd.生产工单号=C_工单当期耗用.生产工单号
                            left join  (select  生产工单号,关闭 from C_工单 where 年={year} and 月={mon}) gdjs on gdjs.生产工单号=gd.生产工单号 
                            left join  基础数据物料信息表 base  on base.物料编码 =产品编码 
                            left join  基础数据物料信息表 zx  on zx.物料编码 =子项编码  where 年={year} and 月={mon} and 子项在制数 <>0   and gdjs.关闭=0
                        delete 财务即时库存记录 where   时间>'{new DateTime(year, mon, 1)}'  and 在制品=0   ";
                    CZMaster.MasterSQL.ExecuteSQL(sql, strconn);
                }
                MessageBox.Show("保存成功");
                label1.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                // CZMaster.MasterLog.WriteLog(ex.Message, "");
            }
        }
        string cfgfilepath = "";
#pragma warning disable IDE1006 // 命名样式
        private void ui月结转_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
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

                label1.Text = "";
                DateTime t = CPublic.Var.getDatetime();
                barEditItem2.EditValue = t.Year.ToString();

                barEditItem1.EditValue = t.Month.ToString();
                //如果上个月还没有结转 默认上月的月份和日期 
                DateTime t_pre = t.AddMonths(-1);
                string s = string.Format("select  count(*)x from 仓库月出入库结转表 where 年={0} and  月={1}", t_pre.Year, t_pre.Month);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                if (Convert.ToDecimal(temp.Rows[0][0]) == 0)
                {
                    barEditItem2.EditValue = t_pre.Year.ToString();
                    barEditItem1.EditValue = t_pre.Month.ToString();

                }
                barLargeButtonItem6.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barLargeButtonItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barLargeButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                if (CPublic.Var.LocalUserTeam == "财务部权限" || CPublic.Var.LocalUserTeam == "管理员权限" || CPublic.Var.LocalUserID == "admin")
                {
                    barLargeButtonItem6.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    barLargeButtonItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    barLargeButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                }



                //fun_载入物料();
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
      
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                
                ERPorg.Corg.FlushMemory();
                if (barEditItem1.EditValue == DBNull.Value || barEditItem1.EditValue.ToString() == "" || barEditItem2.EditValue == DBNull.Value || barEditItem2.EditValue.ToString() == "")
                {
                    MessageBox.Show("请先填写年份选择月份！");
                }
                else
                {
                    // fun_载入物料();
                    string sql = string.Format(@"select  a.物料编码,[入库数量],出库数量 as 出库数量,[上月结转数量],[本月结转数量],出库金额
   ,[入库金额],[年],[月],[上月结转金额],[本月结转金额],[差异数量],差异金额,发出单价,收入单价,结存单价
   ,[差异金额],[结算日期],b.物料名称,b.规格型号  from 仓库月出入库结转表 a
    left join 基础数据物料信息表 b on a.物料编码=b.物料编码  where   年 = '{0}' and 月 = '{1}'",
                        barEditItem2.EditValue.ToString(), barEditItem1.EditValue.ToString());
                    dtM = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dtM);
                    gc.DataSource = dtM;
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
            }
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                    gc.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show(string.Format("确认重置{0}月结转数据？", Convert.ToInt32(barEditItem1.EditValue)), "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                DateTime t1 = new DateTime(Convert.ToInt32(barEditItem2.EditValue), Convert.ToInt32(barEditItem1.EditValue), 1);
                DateTime t2 = t1.AddMonths(1).AddSeconds(-1);


                string s = string.Format($@"delete  仓库月出入库结转表 where 年={Convert.ToInt32(barEditItem2.EditValue)} and 月={Convert.ToInt32(barEditItem1.EditValue)} 
                 delete [财务即时库存记录] where  时间>'{t1}' and 时间<'{t2}' ");
                CZMaster.MasterSQL.ExecuteSQL(s, strconn);
                MessageBox.Show("已删除");
            }
        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                int month = Convert.ToInt32(barEditItem1.EditValue);
                int year = Convert.ToInt32(barEditItem2.EditValue);
                if (MessageBox.Show(string.Format("确认调整{0}年{1}月结转数据金额？", year, month), "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string s = string.Format(@"update 仓库月出入库结转表 set 本月结转金额=0,差异金额=本月结转金额 where 年={0} and 月={1}  
                         and 本月结转数量 =0 and 本月结转金额<>0", year, month);
                    CZMaster.MasterSQL.ExecuteSQL(s, strconn);
                    MessageBox.Show("调整成功");
                    barLargeButtonItem4_ItemClick(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}