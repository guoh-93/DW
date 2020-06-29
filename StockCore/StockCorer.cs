using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;


namespace StockCore
{
    public static class StockCorer
    {
        private static string strconn = CPublic.Var.strConn;
        private static string strWSDL = CPublic.Var.strWSConn;

        #region 物料及数量初始化
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 2019-10-15 修改将初始化dt的返回
        /// 作者：赵峰；2015-12-4
        /// 初始化仓库物料，初始化后的仓库物料应该只有仓库数量，其它数量都为0
        /// 生成仓库物料表，物料数量表，物料数量表_历史
        /// 如果仓库物料表物料已经存在，throw 一个错误，描述：物料已经存在；其它二个物料表已经存在，那么删除后直接新建
        /// 它带三个子函数
        /// 一塌糊涂,不知道写的啥 改了,就这样把  
        /// 
        /// </summary>
        /// <param name="str_库位号">库位号</param>
        /// <param name="str_ItemNo">物料编码</param>
        /// <param name="qty">数量</param>
        /// <param name="drM">修改或新增物料的信息</param>
        public static DataTable fun_Init初始化仓库物料(DataRow drM, string str_货架描述, string str_ItemNo, Decimal qty)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataTable dt_仓库物料数量表 = new DataTable();
                str_ItemNo = str_ItemNo.Trim();
                //string sql_物料是否有效 = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", str_ItemNo);
                //SqlDataAdapter da = new SqlDataAdapter(sql_物料是否有效, strconn);
                //DataTable dt_基础物料 = new DataTable();
                //da.Fill(dt_基础物料);
                ////判断物料是否有效
                //if (dt_基础物料.Rows.Count == 0)
                //{
                //   throw new Exception(string.Format("物料'{0}'无效，基础数据物料信息表中不存在该物料信息", str_ItemNo));
                //}

                //else
                //{
                //    string sql = string.Format("select * from 仓库物料表 where 物料编码 = '{0}' and 仓库号='{1}'", str_ItemNo, stockid);
                //    using (SqlDataAdapter daa = new SqlDataAdapter(sql, strconn))
                //    {
                //        DataTable dt_仓库物料 = new DataTable();
                //        daa.Fill(dt_仓库物料);
                //        //判断 仓库物料表 物料是否已经存在
                //        if (dt_仓库物料.Rows.Count > 0)
                //        {   }
                //        else
                //        {
                //            try
                //            {
                dt_仓库物料数量表 = fun_初始化物料数量表(drM, str_ItemNo, qty);

                //SqlConnection conn = new SqlConnection(strconn);
                //conn.Open();
                //SqlTransaction ts = conn.BeginTransaction("初始化");
                //string sql_仓库物料数量表 = "select * from 仓库物料数量表 where 1<>1";
                //SqlCommand cmd_仓库物料数量表 = new SqlCommand(sql_仓库物料数量表, conn, ts);
                //SqlDataAdapter da_仓库物料数量表 = new SqlDataAdapter(cmd_仓库物料数量表);
                //new SqlCommandBuilder(da_仓库物料数量表);
                //try
                //{
                //    da_仓库物料数量表.Update(dt_仓库物料数量表);
                //    ts.Commit();
                //}
                //catch (Exception ex)
                //{
                //    ts.Rollback();
                //    throw ex;
                //}
                //    }
                //    catch (Exception ex)
                //    {
                //        throw ex;
                //    }
                //}

                //}
                //}

                return dt_仓库物料数量表;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_Init初始化仓库物料");
                throw ex;
            }
        }


#pragma warning disable IDE1006 // 命名样式
        private static DataTable fun_初始化仓库物料表(string str_货架描述, string str_ItemNo, Decimal qty, DataTable dtt)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                //string ssl = string.Format("select * from 基础数据仓库库位表 where 库位号 = '{0}'", str_库位号);
                //DataTable t = new DataTable();
                //SqlDataAdapter a = new SqlDataAdapter(ssl, strconn);
                //a.Fill(t);

                string sql = "select * from 仓库物料表 where 1<>1";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                DataRow dr = dt.NewRow();
                dr["货架描述"] = str_货架描述;
                dr["物料编码"] = str_ItemNo;
                dr["物料名称"] = dtt.Rows[0]["物料名称"].ToString();
                dr["规格型号"] = dtt.Rows[0]["规格型号"].ToString();
                dr["图纸编号"] = dtt.Rows[0]["图纸编号"].ToString();
                dr["BOM版本"] = dtt.Rows[0]["BOM版本"].ToString();
                dr["库存数量"] = qty;
                dr["仓库号"] = dtt.Rows[0]["仓库号"].ToString();
                dr["仓库名称"] = dtt.Rows[0]["仓库名称"].ToString();
                //dr["物料描述"] = "";
                //dr["仓库描述"] = t.Rows[0]["仓库类型"].ToString();
                dr["GUID"] = System.Guid.NewGuid().ToString();
                dr["盘点有效批次号"] = "初始化";
                dt.Rows.Add(dr);
                return dt;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_初始化仓库物料表");
                throw ex;
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private static DataTable fun_初始化物料数量表(DataRow dr_baseinfo, string str_ItemNo, Decimal qty)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = string.Format("select * from 仓库物料数量表 where 物料编码='{0}' and 仓库号='{1}'", str_ItemNo, dr_baseinfo["仓库号"].ToString());
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count == 0)
                    {
                        DataRow dr = dt.NewRow();
                        dr["物料编码"] = str_ItemNo;
                        dr["物料名称"] = dr_baseinfo["物料名称"].ToString();
                        dr["规格型号"] = dr_baseinfo["规格型号"].ToString();
                        dr["图纸编号"] = dr_baseinfo["图纸编号"].ToString();
                        dr["BOM版本"] = dr_baseinfo["BOM版本"].ToString();
                        dr["仓库号"] = dr_baseinfo["仓库号"].ToString();
                        dr["仓库名称"] = dr_baseinfo["仓库名称"].ToString();
                        dr["货架描述"] = dr_baseinfo["货架描述"].ToString();
                        dr["库存总数"] = qty;
                        dr["有效总数"] = qty;
                        dr["在途量"] = 0;
                        dr["在制量"] = 0;
                        dr["受订量"] = 0;
                        dr["未领量"] = 0;
                        dr["MRP计划采购量"] = 0;
                        dr["MRP计划生产量"] = 0;
                        dr["MRP库存锁定量"] = 0;
                        dr["GUID"] = System.Guid.NewGuid().ToString();
                        dt.Rows.Add(dr);
                    }
                    return dt;
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_初始化物料数量表");
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private static DataTable fun_初始化物料数量表_历史(string str_ItemNo, Decimal qty)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataTable dt = null;
                return dt;

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_初始化物料数量表_历史");
                throw ex;
            }
        }
        #endregion

        #region 
        /// <summary>
        /// 2019-4-19 郭恒
        /// </summary>
        /// <param name="str_货架描述"></param>
        /// <param name="str_ItemNo" > r 有 物料编码 物料名称,规格型号,图纸编号,仓库号,仓库名称,货架描述</param>
        public static DataTable Init_stock(DataRow r)
        {

            try
            {
                DataTable dt = new DataTable();

                string sql = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"]);
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                DataTable dt_基础物料 = new DataTable();
                da.Fill(dt_基础物料);
                //判断物料是否有效
                if (dt_基础物料.Rows.Count == 0)
                {
                    sql = string.Format("select * from 仓库物料数量表 where 物料编码='{0}' and 仓库号='{1}'", r["物料编码"], r["仓库号"]);
                    da = new SqlDataAdapter(sql, strconn);
                    {
                        dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count == 0)
                        {
                            DataRow dr = dt.NewRow();
                            dr["物料编码"] = r["物料编码"].ToString();
                            dr["物料名称"] = r["物料名称"].ToString();
                            dr["规格型号"] = r["规格型号"].ToString();
                            dr["图纸编号"] = r["图纸编号"].ToString();
                            dr["仓库号"] = r["仓库号"].ToString();
                            dr["仓库名称"] = r["仓库名称"].ToString();
                            dr["货架描述"] = r["货架描述"].ToString();
                            dr["库存总数"] = 0;
                            dr["有效总数"] = 0;
                            dr["在途量"] = 0;
                            dr["在制量"] = 0;
                            dr["受订量"] = 0;
                            dr["未领量"] = 0;
                            dr["MRP计划采购量"] = 0;
                            dr["MRP计划生产量"] = 0;
                            dr["MRP库存锁定量"] = 0;
                            dr["GUID"] = System.Guid.NewGuid().ToString();
                            dt.Rows.Add(dr);

                        }
                    }
                }
                return dt;

            }
            catch (Exception ex)

            {
                throw new Exception(ex.Message);
            }

        }
        #endregion

        #region 重写
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 2018-7-25确认没有地方用到
        /// 2018-7-25 添加参数 仓库号
        /// </summary>
        /// <param name="str_ItemNos">例如：物料编码 = '值' or 物料编码 = '值' .. </param>
        /// <returns></returns>
        public static Decimal fun_物料数量_刷新所有量(string str_ItemNos, string stock_id)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                Decimal dec_在途量 = 0;
                Decimal dec_在制量 = 0;
                Decimal dec_受订单 = 0;
                Decimal dec_未领量 = 0;
                Decimal de有效总数 = 0;
                Decimal de库存总数 = 0;

                string sql = "select * from 仓库物料数量表 where " + str_ItemNos + " and 仓库号= '" + stock_id + "'";
                DataTable dt_物料数量 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_物料数量);

                foreach (DataRow r in dt_物料数量.Rows)
                {
                    r["在途量"] = fun_物料数量_在途量(r["物料编码"].ToString(), stock_id, true);
                    r["在制量"] = fun_物料数量_在制量(r["物料编码"].ToString(), stock_id, true);
                    r["受订量"] = fun_物料数量_受订单(r["物料编码"].ToString(), stock_id, true);
                    r["未领量"] = fun_物料数量_未领量(r["物料编码"].ToString(), stock_id, true);
                    //r["库存总数"] = fun_物料数量_库存总数(r["物料编码"].ToString(), true);

                    de库存总数 = Convert.ToDecimal(r["库存总数"]);
                    dec_在途量 = Convert.ToDecimal(r["在途量"]);
                    dec_在制量 = Convert.ToDecimal(r["在制量"]);
                    dec_受订单 = Convert.ToDecimal(r["受订量"]);
                    dec_未领量 = Convert.ToDecimal(r["未领量"]);

                    de有效总数 = de库存总数 + dec_在途量 + dec_在制量 - dec_受订单 - dec_未领量;
                    r["有效总数"] = de有效总数;
                }
                try
                {
                    string sqll = "select * from 仓库物料数量表 where 1<>1";
                    SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn);
                    new SqlCommandBuilder(daa);
                    daa.Update(dt_物料数量);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return de有效总数;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_物料数量_实际数量");
                throw ex;
            }
        }
        #endregion


        #region 入库dt处理

        public static DataTable fun_RUKU(string RKtype, DataTable dt)
        {
            string sql = "select * from 入库流水记录表 where 1<>1";

            DataTable dt_rk = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

            if (RKtype == "采购入库")
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[0];
                    DataRow drr = dt_rk.NewRow();
                    dt_rk.Rows.Add(drr);
                    drr["入库明细号"] = dr["入库明细号"];
                    drr["入库单号"] = dr["入库单号"];
                    drr["入库类型"] = RKtype.ToString();
                    drr["物料名称"] = dr["物料名称"];
                    drr["物料编码"] = dr["物料编码"];
                    drr["规格型号"] = dr["规格型号"];
                    drr["仓库号"] = dr["仓库ID"];
                    drr["仓库名称"] = dr["仓库名称"];
                    drr["货架描述"] = dr["货架描述"];
                    drr["入库数量"] = dr["入库量"];
                    drr["录入日期"] = dr["录入日期"];
                    drr["操作人"] = dr["操作员"];
                    drr["操作人编号"] = dr["操作员ID"];
                }
            }



            if (RKtype == "成品入库")
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[0];
                    string sql2 = string.Format("select 货架描述  from  仓库物料数量表 where 物料编码='{0}' ", dr["物料编码"]);
                    DataRow dr_货架描述 = CZMaster.MasterSQL.Get_DataRow(sql2, strconn);

                    DataRow drr = dt_rk.NewRow();
                    dt_rk.Rows.Add(drr);
                    drr["入库明细号"] = dr["成品入库单明细号"];
                    drr["入库单号"] = dr["成品入库单号"];
                    drr["入库类型"] = RKtype.ToString();
                    drr["物料名称"] = dr["物料名称"];
                    drr["物料编码"] = dr["物料编码"];
                    drr["规格型号"] = dr["规格型号"];
                    drr["仓库号"] = dr["仓库号"];
                    drr["仓库名称"] = dr["仓库名称"];
                    drr["货架描述"] = dr_货架描述["货架描述"];
                    drr["入库数量"] = dr["入库数量"];
                    drr["录入日期"] = dr["生效日期"];
                    drr["操作人"] = dr["生效人员"];
                    drr["操作人编号"] = dr["生效人员ID"];

                }

            }

            return dt_rk;
        }
        #endregion

        #region 物料数量表运算
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 作者：赵峰；2015-12-4
        /// 计算物料的在途量，在制量，受订量，未领量，有效数量，库存总量
        /// 2018-7-25 添加参数仓库号   
        /// </summary>
        /// <param name="str_ItemNo">物料编码</param>
        /// <param name="Refresh">是否刷新数据库数据</param>
        /// <returns></returns>



        public static Decimal fun_物料数量_实际数量(string str_ItemNo, string stock_id, Boolean Refresh = false)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                //return WSAdapter.MasterErpService.WS_Fun.fun_物料数量_实际数量(str_ItemNo, strconn, "", Refresh);
                str_ItemNo = str_ItemNo.Trim();
                Decimal dec_在途量 = 0;
                Decimal dec_在制量 = 0;
                Decimal dec_受订单 = 0;
                Decimal dec_未领量 = 0;
                if (Refresh == false)
                {
                    dec_在途量 = fun_物料数量_在途量(str_ItemNo, stock_id);
                    dec_在制量 = fun_物料数量_在制量(str_ItemNo, stock_id);
                    dec_受订单 = fun_物料数量_受订单(str_ItemNo, stock_id);
                    dec_未领量 = fun_物料数量_未领量(str_ItemNo, stock_id);
                }
                else
                {
                    dec_在途量 = fun_物料数量_在途量(str_ItemNo, stock_id, true);
                    dec_在制量 = fun_物料数量_在制量(str_ItemNo, stock_id, true);
                    dec_受订单 = fun_物料数量_受订单(str_ItemNo, stock_id, true);
                    dec_未领量 = fun_物料数量_未领量(str_ItemNo, stock_id, true);
                }
                Decimal de有效总数 = 0;



                string sql = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}' and 仓库号='{1}'", str_ItemNo, stock_id);
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                DataTable dt_物料数量 = new DataTable();
                Decimal dec库存总数 = 0;
                da.Fill(dt_物料数量);
                if (dt_物料数量.Rows.Count > 0)
                {
                    try
                    {
                        dec库存总数 = Convert.ToDecimal(dt_物料数量.Rows[0]["库存总数"]);
                        //dec库存总数 = fun_物料数量_库存总数(str_ItemNo, true);
                    }
                    catch
                    {
                        dec库存总数 = 0;
                    }
                    try
                    {
                        de有效总数 = dec库存总数 + dec_在途量 + dec_在制量 - dec_受订单 - dec_未领量;
                    }
                    catch
                    {
                        de有效总数 = 0;
                    }

                    try
                    {
                        string sqll = "select * from 仓库物料数量表 where 1<>1";
                        SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn);
                        new SqlCommandBuilder(daa);
                        dt_物料数量.Rows[0]["有效总数"] = de有效总数;
                        if (Refresh == true)
                        {
                            dt_物料数量.Rows[0]["在途量"] = dec_在途量;
                            dt_物料数量.Rows[0]["在制量"] = dec_在制量;
                            dt_物料数量.Rows[0]["受订量"] = dec_受订单;
                            dt_物料数量.Rows[0]["未领量"] = dec_未领量;
                            dt_物料数量.Rows[0]["库存总数"] = dec库存总数;
                        }
                        daa.Update(dt_物料数量);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                return de有效总数;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_物料数量_实际数量");
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 2018-7-25 确认没有地方用到
        /// </summary>
        /// <param name="str_ItemNo"></param>
        /// <param name="Refresh"></param>
        /// <returns></returns>
        private static Decimal fun_物料数量_库存总数(string str_ItemNo, Boolean Refresh = false)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = string.Format("select * from 仓库物料表 where 物料编码 = '{0}'", str_ItemNo);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                string str_盘点有效批次号 = dt.Rows[0]["盘点有效批次号"].ToString();
                Decimal dec_库存总数 = 0;
                dec_库存总数 = Convert.ToDecimal(dt.Rows[0]["库存数量"]);
                if (Refresh == false)
                { }
                else
                {
                    string sql_盘点 = string.Format("select sum(实效数量) as 实效数量 from 仓库出入库明细表 where 物料编码 = '{0}' and 盘点有效批次号 = '{1}' and 实效时间 > '2016-12-01 00:00:00'", str_ItemNo, str_盘点有效批次号);
                    DataTable dt_盘点 = new DataTable();
                    SqlDataAdapter da_盘点 = new SqlDataAdapter(sql_盘点, strconn);
                    da_盘点.Fill(dt_盘点);
                    if (dt_盘点.Rows.Count > 0)
                    {
                        if (dt_盘点.Rows[0]["实效数量"] == DBNull.Value)
                        {
                            dt_盘点.Rows[0]["实效数量"] = 0;
                        }
                        dec_库存总数 = dec_库存总数 + Convert.ToDecimal(dt_盘点.Rows[0]["实效数量"]);
                    }
                }
                return dec_库存总数;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_物料数量_库存总数");
                throw ex;
            }
        }


#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 2018-7-25 新增参数 仓库号
        /// </summary>
        /// <param name="str_ItemNo"></param>
        /// <param name="stock_id"></param>
        /// <param name="Refresh"></param>
        /// <returns></returns>
        private static Decimal fun_物料数量_在途量(string str_ItemNo, string stock_id, Boolean Refresh = false)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                //return WSAdapter.MasterErpService.WS_Fun.fun_物料数量_在途量(str_ItemNo, strconn, "", Refresh);
                Decimal dec在途量 = 0;

                if (Refresh == false)
                {
                    string sql = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}' and 仓库号='{1}'", str_ItemNo, stock_id);
                    DataTable dt_在途量 = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt_在途量);
                    dec在途量 = Convert.ToDecimal(dt_在途量.Rows[0]["在途量"].ToString());
                }
                else
                {
                    //                    string sql = string.Format(@"select SUM(未完成数量) as 在途量 from 采购记录采购单明细表 where 物料编码 = '{0}' and 生效 = 1
                    //                                            and 明细完成日期 is null and 作废 = 0 and 总完成 = 0 and 生效日期 > '2016-12-01 00:00:00'", str_ItemNo);
                    //正常送检或 送检关闭 明细完成 会 赋为1   入库 赋日期
                    //来料入库关闭 明细表的总完成赋为1 ,为了不影响在途量和 其他明细入库给主记录打状态
                    //来料检验 如果不合格 事先给 明细完成日期赋值（为区分不合格后 有的可以上传评审单 继续入库,而另一部分不合格的在途一直存在不能刷新掉的问题）
                    string sql = string.Format(@" select 物料编码,sum(在途量) as 在途量 from  (
  select 物料编码,case when (SUM(采购数量-已送检数)>0) then sum(采购数量-已送检数) else 0 end as 在途量  from 采购记录采购单明细表 a      
   left join 采购记录采购单主表 b on a.采购单号=b.采购单号
where   a.生效 = 1  and a.明细完成=0 and a.作废 = 0  and b.作废=0 and 总完成 = 0 and a.生效日期 > '2017-12-1' group by 物料编码
  union all
     select cmx.物料编码,SUM(送检数量-已检验数) as 在途量  from 采购记录采购送检单明细表 sjmx
     left join 采购记录采购单明细表 cmx  on sjmx.采购单明细号=cmx.采购明细号  
      where sjmx.生效日期>'2017-12-1' and 检验完成=0 and sjmx.作废=0  and sjmx.送检单类型<>'拒收' group by cmx.物料编码
  union all
     select 产品编号 as 物料编码 ,SUM(送检数量-已入库数-不合格数量)在途量  from 采购记录采购单检验主表 jy
     left join 采购记录采购单明细表 cmx  on jy.采购明细号=cmx.采购明细号  
     where 入库完成=0 and  完成=0 and 关闭=0 and 检验日期>'2017-12-1' and 检验结果<>'不合格' group by 产品编号)x 
     where  物料编码='{0}'  group by 物料编码  ", str_ItemNo); // 已检验未入库的 不合格部分的不管

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt);
                    if (dt.Rows.Count > 0 && dt.Rows[0]["在途量"].ToString() != "")
                    {
                        dec在途量 = Convert.ToDecimal(dt.Rows[0]["在途量"].ToString());
                        //foreach (DataRow r in dt.Rows)
                        //{
                        //    dec在途量 = dec在途量 + Convert.ToDecimal(r["未完成数量"].ToString());
                        //}
                    }
                }
                return dec在途量;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_物料数量_在途量");
                return 0;
            }
        }
        //弃用
#pragma warning disable IDE1006 // 命名样式
        private static List<DataRow> fun_物料数量_在途量_R(string str_ItemNo)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                List<DataRow> li_在途量 = new List<DataRow>();
                string sql = string.Format("select * from 采购记录采购单明细表 where 物料编码 = '{0}' and 生效 = 1 and 明细完成 = 0 and 作废 = 0", str_ItemNo);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        li_在途量.Add(r);
                    }
                }
                return li_在途量;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_物料数量_在途量_R");
                return null;
            }
        }


#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 2018 -7-25 新增参数仓库号
        /// </summary>
        /// <param name="str_ItemNo"></param>
        /// <param name="stock_id"></param>
        /// <param name="Refresh"></param>
        /// <returns></returns>
        private static Decimal fun_物料数量_在制量(string str_ItemNo, string stock_id, Boolean Refresh = false)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                Decimal dec在制量 = 0;

                if (Refresh == false)
                {
                    string sql = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}' and 仓库号='{1}'", str_ItemNo, stock_id);
                    DataTable dt_在制量 = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt_在制量);
                    dec在制量 = Convert.ToDecimal(dt_在制量.Rows[0]["在制量"].ToString());
                }
                else
                {
                    string sql = string.Format(@"select SUM(生产数量-isnull(x.已入库数量,0))as 在制量 from 生产记录生产工单表
                             left  join  (select 生产工单号,sum(入库数量) as 已入库数量 from  生产记录成品入库单明细表  group by 生产工单号)x
                            on    x.生产工单号= 生产记录生产工单表.生产工单号
                            where 物料编码 = '{0}' and 仓库号='{1}' and 生效 = 1 and 完成 = 0 and 关闭 = 0 and 生效日期 > '2018-12-1'", str_ItemNo, stock_id);
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt);
                    if (dt.Rows.Count > 0 && dt.Rows[0]["在制量"].ToString() != "")
                    {
                        dec在制量 = Convert.ToDecimal(dt.Rows[0]["在制量"].ToString());
                        //foreach (DataRow r in dt.Rows)
                        //{
                        //    dec在制量 = dec在制量 + Convert.ToDecimal(r["生产数量"].ToString());
                        //}
                    }
                }
                return dec在制量;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_物料数量_在制量");
                return 0;
            }
        }
        //弃用的 
#pragma warning disable IDE1006 // 命名样式
        private static List<DataRow> fun_物料数量_在制量_R(string str_ItemNo)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                List<DataRow> li_在制量 = new List<DataRow>();
                string sql = string.Format("select * from 生产记录生产工单表 where 物料编码 = '{0}' and 生效 = 1 and 完成 = 0 and 关闭 = 0", str_ItemNo);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        li_在制量.Add(r);
                    }
                }
                return li_在制量;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_物料数量_在制量_R");
                return null;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 2018-7-25 新增参数仓库号
        /// </summary>
        /// <param name="str_ItemNo"></param>
        /// <param name="stock_id"></param>
        /// <param name="Refresh"></param>
        /// <returns></returns>
        private static Decimal fun_物料数量_受订单(string str_ItemNo, string stock_id, Boolean Refresh = false)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                //return WSAdapter.MasterErpService.WS_Fun.fun_物料数量_受订单(str_ItemNo, strconn,"", Refresh);
                Decimal dec受订量 = 0;

                if (Refresh == false)
                {
                    string sql = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}' and 仓库号='{1}'", str_ItemNo, stock_id);
                    DataTable dt_受订单 = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt_受订单);
                    dec受订量 = Convert.ToDecimal(dt_受订单.Rows[0]["受订量"].ToString());
                }
                else
                {
                    string sql = string.Format(@"select SUM(未完成数量) as 受订量 from 销售记录销售订单明细表 where 物料编码 = '{0}'  and 仓库号='{1}'  and 生效 = 1
                    and 明细完成 = 0 and 作废 = 0  and 关闭=0 and 生效日期 > '2017-12-1' ", str_ItemNo, stock_id);
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt);
                    if (dt.Rows.Count > 0 && dt.Rows[0]["受订量"].ToString() != "")
                    {
                        dec受订量 = Convert.ToDecimal(dt.Rows[0]["受订量"].ToString());
                        //foreach (DataRow r in dt.Rows)
                        //{
                        //    dec受订量 = dec受订量 + Convert.ToDecimal(r["未完成数量"].ToString());
                        //}
                    }
                }
                return dec受订量;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_物料数量_受订单");
                return 0;
            }
        }
        //弃用
#pragma warning disable IDE1006 // 命名样式
        private static List<DataRow> fun_物料数量_受订单_R(string str_ItemNo)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                List<DataRow> li_受订单 = new List<DataRow>();
                string sql = string.Format("select * from 销售记录销售订单明细表 where 物料编码 = '{0}' and 生效 = 'True' and 明细完成 = 'False' and 作废 = 0", str_ItemNo);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        li_受订单.Add(r);
                    }
                }
                return li_受订单;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_物料数量_受订单_R");
                return null;
            }
        }
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 2018-7-25 新增参数仓库号
        /// </summary>
        /// <param name="str_ItemNo"></param>
        /// <param name="stock_id"></param>
        /// <param name="Refresh"></param>
        /// <returns></returns>
        private static Decimal fun_物料数量_未领量(string str_ItemNo, string stock_id, Boolean Refresh = false)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                Decimal dec未领量 = 0;

                if (Refresh == false)
                {
                    string sql = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}' and 仓库号='{1}'", str_ItemNo, stock_id);
                    DataTable dt_未领量 = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt_未领量);
                    dec未领量 = Convert.ToDecimal(dt_未领量.Rows[0]["未领量"].ToString());
                }
                else
                {
                    string sql = string.Format(@"select SUM(待领料总量) - SUM(已领数量) as 未领量 from 生产记录生产工单待领料明细表 dlmx
                        left  join  生产记录生产工单待领料主表 on   dlmx.待领料单号= 生产记录生产工单待领料主表.待领料单号
                    where 生产记录生产工单待领料主表.关闭=0 and 物料编码 = '{0}' and dlmx.仓库号='{1}'  and dlmx.完成 = 0 
                    and dlmx.创建日期 > '2018-12-1'", str_ItemNo, stock_id);// and 生效 = 1
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt);
                    if (dt.Rows[0]["未领量"].ToString() != "" && dt.Rows.Count > 0)
                    {
                        dec未领量 = Convert.ToDecimal(dt.Rows[0]["未领量"].ToString());
                    }
                }
                return dec未领量;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_物料数量_未领量");
                return 0;
            }
        }
        //弃用
#pragma warning disable IDE1006 // 命名样式
        private static List<DataRow> fun_物料数量_未领量_R(string str_ItemNo)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                List<DataRow> li_未领量 = new List<DataRow>();
                string sql = string.Format("select * from 生产记录生产工单待领料明细表 where 物料编码 = '{0}' and 完成 = 0", str_ItemNo);// and 生效 = 1
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        li_未领量.Add(r);
                    }
                }
                return li_未领量;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_物料数量_未领量_R");
                return null;
            }
        }
        //flag 标志 出入库 出 -1 入 1
        // 16/11/11 暂时为出入库直接更新库存数量  17年  已弃用
#pragma warning disable IDE1006 // 命名样式
        public static void fun_刷新库存(string str_物料号, decimal dec_数量, int flag)
#pragma warning restore IDE1006 // 命名样式
        {
            //string sql=string.Format("update 仓库物料数量表 set 库存总数=库存总数+'{0}',出入库时间='{1}' where 物料编码='{2}'",dec_数量*flag,System.DateTime.Now,str_物料号);
            string sql = string.Format("select *  from 仓库物料数量表 where 物料编码='{0}'", str_物料号);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    dt.Rows[0]["库存总数"] = Convert.ToDecimal(dt.Rows[0]["库存总数"]) + dec_数量 * flag;
                    dt.Rows[0]["出入库时间"] = System.DateTime.Now;
                    new SqlCommandBuilder(da);
                    da.Update(dt);
                }
                else
                {
                    throw new Exception("仓库物料数量表中没有找到该物料");
                }
            }
        }
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 刷新 在途在制受订 未领
        /// dtx 为待刷新列表 
        ///   7-25 东屋 dtx中需有仓库信息 仓库号
        /// </summary>
        public static DataTable fun_四个量(DataTable dtx)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataTable dt = new DataTable();
                foreach (DataRow dr in dtx.Rows)
                {
                    string sql = string.Format("select  * from 仓库物料数量表 where 物料编码='{0}' and 仓库号='{1}'", dr["物料编码"].ToString(), dr["仓库号"].ToString());
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                    {
                        da.Fill(dt);
                        DataRow[] r = dt.Select(string.Format("物料编码='{0}'", dr["物料编码"].ToString()));
                        if (r.Length == 0) continue;
                        //在途在制受订未领 都需加入仓库号限制
                        decimal a = fun_物料数量_在途量(dr["物料编码"].ToString(), dr["仓库号"].ToString(), true);
                        decimal b = fun_物料数量_在制量(dr["物料编码"].ToString(), dr["仓库号"].ToString(), true);
                        decimal c = fun_物料数量_受订单(dr["物料编码"].ToString(), dr["仓库号"].ToString(), true);
                        decimal d = fun_物料数量_未领量(dr["物料编码"].ToString(), dr["仓库号"].ToString(), true);
                        r[0]["在途量"] = a;
                        r[0]["在制量"] = b;
                        r[0]["受订量"] = c;
                        r[0]["未领量"] = d;

                        r[0]["有效总数"] = Convert.ToDecimal(r[0]["库存总数"].ToString()) + a + b - c - d;
                    }

                }
                if (dt.Columns.Count == 0)
                {
                    string sql = string.Format("select  * from 仓库物料数量表 where 1=2");
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                    {
                        da.Fill(dt);
                    }
                }
                return dt;
            }
            catch (Exception)
            {
                throw;
            }

        }
        #region 采购单
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 作者：赵峰；2015-12-4
        /// 改变 采购计划表 中的数量,改变 物料数量表 中的MRP数量
        /// 2018-7-25 确认没有地方用到
        /// </summary>
        /// <param name="str_ItemNo">物料编码</param>
        /// <param name="Qty">采购数量</param>
        /// <param name="str_采购计划类型">采购计划类型</param>
        /// <param name="str_采购计划明细号">采购计划明细号</param>
        /// <param name="strconn">数据库连接字段</param>
        public static void fun_采购单生效(string str_ItemNo, Decimal Qty, Decimal Qty2, string str_采购计划类型, string str_采购计划明细号, string strconn)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (str_采购计划明细号 == "")
                {
                    return;
                }
                if (str_采购计划类型 != "")
                {
                    //改变采购计划表中的数量 
                    string sql2 = string.Format("select * from 采购记录采购计划表 where 采购计划明细号 = '{0}'", str_采购计划明细号);
                    DataTable dt2 = new DataTable();
                    SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
                    da2.Fill(dt2);
                    dt2.Rows[0]["已生成采购数量"] = Convert.ToDecimal(dt2.Rows[0]["已生成采购数量"]) + Qty;
                    dt2.Rows[0]["未完成采购数量"] = Convert.ToDecimal(dt2.Rows[0]["未完成采购数量"]) - Qty;
                    //dt2.Rows[0]["数量"] = Convert.ToDecimal(dt2.Rows[0]["数量"]) - Qty;
                    new SqlCommandBuilder(da2);
                    da2.Update(dt2);
                    if (str_采购计划类型 == "MRP类型")
                    {
                        //改变物料数量表中的MRP数量 
                        string sql = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}'", str_ItemNo);
                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                        da.Fill(dt);

                        //最小计划采购量
                        dt.Rows[0]["MRP计划采购量"] = Convert.ToDecimal(dt.Rows[0]["MRP计划采购量"]) - Qty2;
                        if (Convert.ToDecimal(dt.Rows[0]["MRP计划采购量"]) < 0)
                        {
                            dt.Rows[0]["MRP计划采购量"] = 0;
                        }

                        //最大库存锁定量
                        dt.Rows[0]["MRP库存锁定量"] = Convert.ToDecimal(dt.Rows[0]["MRP库存锁定量"]) + Qty2;

                        //                        string sqll = string.Format(@"select 子项编码 ,子项名称,sum((基础数据物料BOM表.数量*a.成品数量)) as 总量 from  基础数据物料BOM表,
                        //                        (SELECT 物料编码,SUM([销售记录销售订单明细表].数量)as 成品数量 FROM [FMS].[dbo].[销售记录销售订单明细表]  where 明细完成=0 and 作废=0   
                        //                        group by [销售记录销售订单明细表].物料编码)as a  where 产品编码= a.物料编码 and 子项编码 = '{0}' group by 子项编码 ,子项名称 ", str_ItemNo);



                        string sqll = string.Format(@"select SUM(数量 * a.MRP计划生产量) as 总量 from 基础数据物料BOM表 
                            left join 仓库物料数量表 as a on a.物料编码 = 基础数据物料BOM表.产品编码  
                            where a.MRP计划生产量 > 0 and 子项编码 = '{0}' ", str_ItemNo);
                        DataTable ttt = new DataTable();
                        SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn);
                        daa.Fill(ttt);
                        if (Convert.ToDecimal(dt.Rows[0]["MRP库存锁定量"]) > (Convert.ToDecimal(ttt.Rows[0]["总量"]) + Convert.ToDecimal(dt.Rows[0]["受订量"])) && Convert.ToDecimal(ttt.Rows[0]["总量"]) > 0)
                        {
                            dt.Rows[0]["MRP库存锁定量"] = Convert.ToDecimal(ttt.Rows[0]["总量"]) + Convert.ToDecimal(dt.Rows[0]["受订量"]);
                        }

                        new SqlCommandBuilder(da);
                        da.Update(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "fun_采购单生效_MRP数量变化");
                throw ex;
            }
        }
        #endregion

        #region 生产制令
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 作者：赵峰；2015-12-21
        /// 改变 生产记录生产计划表 中的数量,改变 物料数量表 中的MRP数量
        /// 2018-7-25 确认没有地方用到
        /// </summary>
        /// <param name="str_ItemNo">物料编码</param>
        /// <param name="Qty">采购数量</param>
        /// <param name="str_采购计划类型">采购计划类型</param>
        /// <param name="strconn">数据库连接字段</param>
        public static void fun_生产制令_生效(string str_ItemNo, Decimal Qty, Decimal Qty2, string str_生产制令类型, string str_生产计划单号, string strconn)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (str_生产计划单号 == "")
                {
                    return;
                }
                if (str_生产制令类型 != "")
                {
                    //MRP计划生产量↓  MRP库存锁定量↑
                    //                    if (str_生产制令类型 == "MRP类型")
                    //                    {
                    //                        string sql = string.Format(@"select 仓库物料数量表.*,基础数据物料信息表.物料类型 from 仓库物料数量表 left join 基础数据物料信息表 on 基础数据物料信息表.物料编码
                    //                         = 仓库物料数量表.物料编码 where 仓库物料数量表.物料编码 = '{0}'", str_ItemNo);
                    //                        DataTable dt = new DataTable();
                    //                        SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    //                        da.Fill(dt);

                    //                        //最小计划生产量
                    //                        dt.Rows[0]["MRP计划生产量"] = Convert.ToDecimal(dt.Rows[0]["MRP计划生产量"]) - Qty;
                    //                        if (Convert.ToDecimal(dt.Rows[0]["MRP计划生产量"]) < 0)
                    //                        {
                    //                            dt.Rows[0]["MRP计划生产量"] = 0;
                    //                        }

                    //                        //最大库存锁定量
                    //                        dt.Rows[0]["MRP库存锁定量"] = Convert.ToDecimal(dt.Rows[0]["MRP库存锁定量"]) + Qty2;
                    //                        if (dt.Rows[0]["物料类型"].ToString() == "成品")
                    //                        {
                    //                            if (Convert.ToDecimal(dt.Rows[0]["MRP库存锁定量"]) > Convert.ToDecimal(dt.Rows[0]["受订量"]) && Convert.ToDecimal(dt.Rows[0]["受订量"]) > 0)
                    //                                dt.Rows[0]["MRP库存锁定量"] = Convert.ToDecimal(dt.Rows[0]["受订量"]);
                    //                        }
                    //                        else
                    //                        {
                    ////                            string sqll = string.Format(@" select [销售记录销售订单明细表].物料编码,基础数据物料BOM表.数量,仓库物料数量表.受订量, 仓库物料数量表.库存总数 from [销售记录销售订单明细表] 
                    ////                            left join 基础数据物料BOM表 on [销售记录销售订单明细表].物料编码 = 基础数据物料BOM表.产品编码 
                    ////                            left join 仓库物料数量表 on 仓库物料数量表.物料编码 = [销售记录销售订单明细表].物料编码
                    ////                            where [销售记录销售订单明细表].明细完成 = 0 and 销售记录销售订单明细表.已计算 = 1 and 基础数据物料BOM表.子项编码 = '{0}'
                    ////                            group by [销售记录销售订单明细表].物料编码,基础数据物料BOM表.数量,仓库物料数量表.受订量, 仓库物料数量表.库存总数", str_ItemNo);
                    ////                            DataTable dt_锁定计算 = new DataTable();
                    ////                            SqlDataAdapter da_锁定计算 = new SqlDataAdapter(sqll, strconn);
                    ////                            da_锁定计算.Fill(dt_锁定计算);
                    ////                            Decimal de_求和 = 0;
                    ////                            foreach (DataRow r in dt_锁定计算.Rows)
                    ////                            {
                    ////                                if ((Convert.ToDecimal(r["受订量"]) - Convert.ToDecimal(r["库存总数"])) > 0)
                    ////                                {
                    ////                                    de_求和 = de_求和 + (Convert.ToDecimal(r["受订量"]) - Convert.ToDecimal(r["库存总数"])) * Convert.ToDecimal(r["数量"]);
                    ////                                }
                    ////                            }
                    ////                            if (Convert.ToDecimal(dt.Rows[0]["MRP库存锁定量"]) > (Convert.ToDecimal(dt.Rows[0]["受订量"]) + de_求和))
                    ////                            {
                    ////                                dt.Rows[0]["MRP库存锁定量"] = Convert.ToDecimal(dt.Rows[0]["受订量"]) + de_求和;
                    ////                            }
                    //                        }

                    //                        sql = "select * from 仓库物料数量表 where 1<>1";
                    //                        da = new SqlDataAdapter(sql, strconn);
                    //                        new SqlCommandBuilder(da);
                    //                        da.Update(dt);
                    //                    }
                    //已转数量↑  未转数量↓
                    string sql2 = string.Format("select * from 生产记录生产计划表 where 生产计划单号 = '{0}'", str_生产计划单号);
                    DataTable dt2 = new DataTable();
                    SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
                    da2.Fill(dt2);
                    dt2.Rows[0]["已生成数量"] = Convert.ToDecimal(dt2.Rows[0]["已生成数量"]) + Qty;
                    dt2.Rows[0]["未生成数量"] = Convert.ToDecimal(dt2.Rows[0]["未生成数量"]) - Qty;
                    //dt2.Rows[0]["计划数量"] = Convert.ToDecimal(dt2.Rows[0]["计划数量"]) - Qty;
                    new SqlCommandBuilder(da2);
                    da2.Update(dt2);
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "fun_生产制令_生效");
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 不用了
        /// 计算该 生产制令单号 所需领的原料，并保存到 生产记录生产工单待领料表
        /// 只有将 未领量存入数据库 才能计算未领量
        /// 循环刷新未领量
        /// </summary>
        /// <param name="str_生产制令单号"></param>
        /// <param name="strconn"></param>
        public static void fun_生产制令_待领料(string str_生产制令单号, string strconn)
#pragma warning restore IDE1006 // 命名样式
        {
            //try
            //{
            //    string sql = string.Format("select * from 生产记录生产制令表 where 生产制令单号 = '{0}'", str_生产制令单号.Trim());
            //    DataTable dt = new DataTable();
            //    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            //    da.Fill(dt);
            //    string sql2 = string.Format("select * from 基础数据物料BOM表 where 产品编码 = '{0}' and 主辅料 = '主料'", dt.Rows[0]["物料编码"].ToString());
            //    DataTable dt2 = new DataTable();
            //    SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
            //    da2.Fill(dt2);
            //    string sql4 = "select * from 生产记录生产制令单待领料表 where 1<>1";
            //    DataTable dt4 = new DataTable();
            //    SqlDataAdapter da4 = new SqlDataAdapter(sql4, strconn);
            //    da4.Fill(dt4);
            //    foreach (DataRow r in dt2.Rows)
            //    {
            //        DataRow dr = dt4.NewRow();
            //        dt4.Rows.Add(dr);
            //        dr["GUID"] = System.Guid.NewGuid();
            //        dr["生产制令单号"] = str_生产制令单号;
            //        dr["产品编码"] = dt.Rows[0]["物料编码"].ToString();
            //        dr["产品名称"] = dt.Rows[0]["物料名称"].ToString();
            //        dr["规格型号"] = dt.Rows[0]["规格型号"].ToString();
            //        dr["原规格型号"] = dt.Rows[0]["原规格型号"].ToString();
            //        dr["生产车间"] = dt.Rows[0]["生产车间"].ToString();
            //        dr["物料编码"] = r["子项编码"].ToString();
            //        dr["物料名称"] = r["子项名称"].ToString();
            //        dr["待领总数量"] = Convert.ToDecimal(r["数量"]) * Convert.ToDecimal(dt.Rows[0]["制令数量"]);
            //        dr["已领数量"] = 0;
            //        dr["未领数量"] = Convert.ToDecimal(r["数量"]) * Convert.ToDecimal(dt.Rows[0]["制令数量"]);
            //        dr["制单人员ID"] = CPublic.Var.LocalUserID;
            //        dr["制单人员"] = CPublic.Var.localUserName;
            //        dr["录入日期"] = System.DateTime.Now;
            //        dr["操作人员ID"] = CPublic.Var.LocalUserID;
            //        dr["操作人员"] = CPublic.Var.localUserName;
            //        dr["创建日期"] = System.DateTime.Now;
            //        dr["修改日期"] = System.DateTime.Now;
            //    }
            //    new SqlCommandBuilder(da4);
            //    da4.Update(dt4);
            //    //循环刷新未领量
            //    foreach (DataRow r in dt2.Rows)
            //    {
            //        fun_物料数量_实际数量(r["子项编码"].ToString(), true);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    CZMaster.MasterLog.WriteLog(ex.Message, "fun_生产制令_待领料");
            //    throw ex;
            //}
        }
        #endregion

        #region 工单生效

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 2018-7-25 确认没有地方用到
        /// </summary>
        /// <param name="str_工单类型"></param>
        /// <param name="str_ItemNo"></param>
        /// <param name="Qty"></param>
        /// <param name="Qty2"></param>
        /// <param name="strconn"></param>
        public static void fun_生产工单_生效(string str_工单类型, string str_ItemNo, Decimal Qty, Decimal Qty2, string strconn)
#pragma warning restore IDE1006 // 命名样式
        {
            if (str_工单类型 == "MRP类型")
            {
                string sql = string.Format(@"select 仓库物料数量表.*,基础数据物料信息表.物料类型 from 仓库物料数量表 left join 基础数据物料信息表 on 基础数据物料信息表.物料编码
                         = 仓库物料数量表.物料编码 where 仓库物料数量表.物料编码 = '{0}'", str_ItemNo);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);

                //最小计划生产量
                dt.Rows[0]["MRP计划生产量"] = Convert.ToDecimal(dt.Rows[0]["MRP计划生产量"]) - Qty;
                if (Convert.ToDecimal(dt.Rows[0]["MRP计划生产量"]) < 0)
                {
                    dt.Rows[0]["MRP计划生产量"] = 0;
                }

                //最大库存锁定量
                dt.Rows[0]["MRP库存锁定量"] = Convert.ToDecimal(dt.Rows[0]["MRP库存锁定量"]) + Qty2;
                if (dt.Rows[0]["物料类型"].ToString() == "成品")
                {
                    if (Convert.ToDecimal(dt.Rows[0]["MRP库存锁定量"]) > Convert.ToDecimal(dt.Rows[0]["受订量"]) && Convert.ToDecimal(dt.Rows[0]["受订量"]) > 0)
                        dt.Rows[0]["MRP库存锁定量"] = Convert.ToDecimal(dt.Rows[0]["受订量"]);
                }
                else
                {
                    //非成品
                    sql = string.Format(@"select 子项编码 ,子项名称,sum((基础数据物料BOM表.数量*a.成品数量)) as 总量 from  基础数据物料BOM表,
                        (SELECT 物料编码,SUM([销售记录销售订单明细表].数量)as 成品数量 FROM [FMS].[dbo].[销售记录销售订单明细表]  where 明细完成=0 and 作废=0   
                        group by [销售记录销售订单明细表].物料编码)as a  where 产品编码= a.物料编码 and 子项编码 = '{0}' group by 子项编码 ,子项名称 ", str_ItemNo);
                    DataTable ttt = new DataTable();
                    da = new SqlDataAdapter(sql, strconn);
                    da.Fill(ttt);
                    if (ttt.Rows.Count > 0)
                    {
                        if (Convert.ToDecimal(dt.Rows[0]["MRP库存锁定量"]) > (Convert.ToDecimal(ttt.Rows[0]["总量"]) + Convert.ToDecimal(dt.Rows[0]["受订量"])) && Convert.ToDecimal(ttt.Rows[0]["总量"]) > 0)
                            dt.Rows[0]["MRP库存锁定量"] = Convert.ToDecimal(ttt.Rows[0]["总量"]) + Convert.ToDecimal(dt.Rows[0]["受订量"]);
                    }
                }

                sql = "select * from 仓库物料数量表 where 1<>1";
                da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dt);
            }
        }
        #endregion
        #endregion

        #region 出入库数量计算
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 赵峰 2015-12-17
        /// 2017-2 修改
        /// 生效采购入库数量，并引起一系列的数量变化
        /// 判断明细是否完成 以及数量变化
        /// </summary>
        /// <param name="str_ItemNo">物料编码</param>
        /// <param name="Qty">入库数量，也是检验合格数</param>
        /// <param name="Qty2">送检数量</param>
        /// <param name="str_明细号">采购明细号</param>
        public static DataSet fun_出入库_采购入库(string str_ItemNo, Decimal Qty, Decimal Qty2, string str_明细号)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataSet ds = new DataSet();
                str_ItemNo = str_ItemNo.Trim(); str_明细号 = str_明细号.Trim();
                //DataSet dst = new DataSet();
                DataTable dst = new DataTable();

                #region 判断明细是否完成 以及数量变化
                //数量变化保存到 采购记录采购单明细表
                string sql1 = string.Format("select * from 采购记录采购单明细表 where 采购明细号 = '{0}'", str_明细号);
                DataTable dt1 = new DataTable();
                SqlDataAdapter da1 = new SqlDataAdapter(sql1, strconn);
                da1.Fill(dt1);
                dt1.Rows[0]["完成数量"] = (Decimal)dt1.Rows[0]["完成数量"] + Qty;
                dt1.Rows[0]["未完成数量"] = (Decimal)dt1.Rows[0]["未完成数量"] - Qty;

                //当明细完成时，完成 = 1
                if ((Decimal)dt1.Rows[0]["未完成数量"] <= (Decimal)0)
                {
                    dt1.Rows[0]["未完成数量"] = 0;
                    dt1.Rows[0]["明细完成"] = 1;
                    dt1.Rows[0]["明细完成日期"] = CPublic.Var.getDatetime();
                }
                //当包含该明细的出库单下的所有明细完成时，总完成 = 1，出库单的完成 = 1
                if (dt1.Rows[0]["明细完成日期"].ToString() != "")
                {

                    string sql_主表 = string.Format("select * from 采购记录采购单主表 where 采购单号 = '{0}'", dt1.Rows[0]["采购单号"].ToString());
                    string sql_mx = string.Format("select * from 采购记录采购单明细表 where 采购单号 = '{0}'", dt1.Rows[0]["采购单号"].ToString());


                    dst = fun_明细_完成操作(dt1, sql_主表, sql_mx, true);
                }

                ds.Tables.Add(dst);
                dst.TableName = "完成状态";
                ds.Tables.Add(dt1);
                dt1.TableName = "采购";
                return ds;
                #endregion

                //string sql_主 = "select * from 采购记录采购单主表 where 1<>1";
                //string sql_1 = "select * from 采购记录采购单明细表 where 1<>1";

                //SqlConnection conn = new SqlConnection(strconn);
                //conn.Open();
                //SqlTransaction ts = conn.BeginTransaction("采购入库");
                //try
                //{
                //    SqlCommand cmm_1 = new SqlCommand(sql_1, conn, ts);

                //    SqlCommand cmm_主表 = new SqlCommand(sql_主, conn, ts);
                //    SqlDataAdapter da_1 = new SqlDataAdapter(cmm_1);

                //    SqlDataAdapter da_主表 = new SqlDataAdapter(cmm_主表);
                //    new SqlCommandBuilder(da_1);

                //    new SqlCommandBuilder(da_主表);
                //    try
                //    {
                //        foreach (DataTable t in dst.Tables)
                //        {

                //            if (t.TableName == "主表")
                //            {
                //                da_主表.Update(t);
                //            }
                //        }
                //    }
                //    catch { }
                //    da_1.Update(dt1);

                //    ts.Commit();                   
                //}
                //catch
                //{
                //    //ts.Rollback();
                //    throw new Exception("生效失败_保存数量失败");
                //}
                //fun_物料数量_实际数量(str_ItemNo, true);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_出入库_采购入库");
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 赵峰 2015-12-18
        /// 库存总数 ↑ 在制量 ↓ （未测试）
        /// 车间虚拟库存数量变化  
        /// </summary>
        /// <param name="str_ItemNo"></param>
        /// <param name="Qty"></param>
        /// <param name="str_明细号">str_生产工单</param>
        public static void fun_出入库_成品入库(string str_ItemNo, Decimal Qty, string str_明细号, string str_检验单号)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                str_ItemNo = str_ItemNo.Trim(); str_明细号 = str_明细号.Trim();
                //string sql2 = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}'", str_ItemNo);
                //DataTable dt_仓库物料数量 = new DataTable();
                //SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
                //da2.Fill(dt_仓库物料数量);
                ////数量变化保存到 仓库物料数量表  库存总数↑ 在制量↓
                //dt_仓库物料数量.Rows[0]["库存总数"] = fun_出入库_物料库存总量(str_ItemNo, Qty);

                //数量变化保存到 生产记录生产工单表  已生产数量↑ 未生产数量↓
                DataTable dt_工单 = fun_成品入库_工单_已生产数量(str_ItemNo, str_明细号, Qty, strconn, str_检验单号);
                DataTable dt_制令 = fun_成品入库_制令_已生产数量(str_ItemNo, str_明细号, Qty, strconn);

                string sql_工单 = "select * from 生产记录生产工单表 where 1<>1";
                string sql_制令 = "select * from 生产记录生产制令表 where 1<>1";
                //string sql_2 = "select * from 仓库物料数量表 where 1<>1";
                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("初始化");
                try
                {
                    //SqlCommand cmm_2 = new SqlCommand(sql_2, conn, ts);
                    SqlCommand cmm_工单 = new SqlCommand(sql_工单, conn, ts);
                    SqlCommand cmm_制令 = new SqlCommand(sql_制令, conn, ts);
                    //SqlDataAdapter da_2 = new SqlDataAdapter(cmm_2);
                    SqlDataAdapter da_工单 = new SqlDataAdapter(cmm_工单);
                    SqlDataAdapter da_制令 = new SqlDataAdapter(cmm_制令);
                    //new SqlCommandBuilder(da_2);
                    new SqlCommandBuilder(da_工单);
                    new SqlCommandBuilder(da_制令);
                    try
                    {
                        da_工单.Update(dt_工单);
                        da_制令.Update(dt_制令);
                    }
                    catch { }
                    //da_2.Update(dt_仓库物料数量);
                    ts.Commit();
                }
                catch (Exception ex)
                {
                    ts.Rollback();
                    throw ex;
                }
                //刷新有效总数和5种量 在制量↓ 库存总数↑
                //fun_物料数量_实际数量(str_ItemNo, true);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_出入库_成品入库");
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 赵峰 2015-12-17
        /// 库存总数↓ MRP库存锁定量↓
        /// 判断明细是否完成 以及数量变化
        /// </summary>
        /// <param name="str_ItemNo">物料编码</param>
        /// <param name="Qty">本次出库的数量</param>
        /// <param name="str_明细号">销售订单明细号</param>
        /// dt_循环 需要循环的 dt
        public static DataSet fun_出入库_成品出库(DataTable dt_循环)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable dt_状态 = new DataTable();
                DataTable dt_销售明细 = new DataTable();
                DataTable dt_仓库 = new DataTable();
                DataTable dst = new DataTable();
                string str_销售单号 = "";
                DataTable dt_主状态 = new DataTable();

                DateTime t = CPublic.Var.getDatetime();
                foreach (DataRow r in dt_循环.Rows)
                {
                    string str_ItemNo = r["物料编码"].ToString();
                    string str_明细号 = r["销售订单明细号"].ToString().Trim();
                    Decimal Qty = (Decimal)r["出库数量"];
                    #region 判断明细是否完成 以及数量变化

                    //数量变化保存到 销售记录销售订单明细表  完成数量↑ 未完成数量↓
                    string sql1 = string.Format("select * from 销售记录销售订单明细表 where 生效=1 and 关闭=0 and 销售订单明细号 = '{0}'", str_明细号);
                    DataTable dt1 = new DataTable();
                    SqlDataAdapter da1 = new SqlDataAdapter(sql1, strconn);
                    da1.Fill(dt1);


                    new SqlCommandBuilder(da1);
                    if (dt1.Rows.Count > 0)
                    {
                        dt1.Rows[0]["完成数量"] = Convert.ToDecimal(dt1.Rows[0]["完成数量"]) + Qty;
                        dt1.Rows[0]["未完成数量"] = Convert.ToDecimal(dt1.Rows[0]["未完成数量"]) - Qty;

                        //当明细完成时，完成 = 1
                        str_销售单号 = dt1.Rows[0]["销售订单号"].ToString().Trim();

                        if ((Decimal)dt1.Rows[0]["未完成数量"] <= (Decimal)0)
                        {
                            dt1.Rows[0]["未完成数量"] = 0;
                            dt1.Rows[0]["明细完成"] = true;
                            dt1.Rows[0]["明细完成日期"] = t;
                            //if (dt_销售明细.Columns.Count == 0)
                            //{
                            //    dt_销售明细 = dt1.Clone();
                            //}
                            //dt_销售明细.ImportRow(dt1.Rows[0]);
                        }
                    }
                    if (dt_销售明细.Columns.Count == 0)
                    {
                        dt_销售明细 = dt1.Clone();
                    }
                    if (dt1.Rows.Count > 0)
                    {
                        dt_销售明细.ImportRow(dt1.Rows[0]);
                    }
                    DataView dv = new DataView(dt_销售明细);
                    dv.RowFilter = "明细完成=1";
                    dt_主状态 = dv.ToTable();

                    //当包含该明细的出库单下的所有明细完成时，总完成 = 1，出库单的完成 = 1

                    #endregion
                    //数量变化保存到 仓库物料数量表  库存总数↓ MRP库存锁定量↓  5/20 去除 改为 库存 
                    string sql2 = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}' and 仓库号='{1}'", str_ItemNo, r["仓库号"]);
                    DataTable dt_仓库物料数量 = new DataTable();
                    SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
                    da2.Fill(dt_仓库物料数量);


                    if (dt_仓库.Columns.Count == 0)
                    {
                        dt_仓库 = dt_仓库物料数量.Clone();
                        dt_仓库物料数量.Rows[0]["库存总数"] = Convert.ToDecimal(dt_仓库物料数量.Rows[0]["库存总数"]) - Qty;
                        dt_仓库物料数量.Rows[0]["出入库时间"] = t;
                        dt_仓库.ImportRow(dt_仓库物料数量.Rows[0]);
                    }
                    else
                    {
                        DataRow[] rr = dt_仓库.Select(string.Format("物料编码='{0}' and 仓库号='{1}'", str_ItemNo, r["仓库号"]));
                        if (rr.Length > 0)
                        {
                            rr[0]["库存总数"] = Convert.ToDecimal(rr[0]["库存总数"]) - Qty;
                            rr[0]["出入库时间"] = t;
                        }
                        else
                        {
                            dt_仓库物料数量.Rows[0]["库存总数"] = Convert.ToDecimal(dt_仓库物料数量.Rows[0]["库存总数"]) - Qty;
                            dt_仓库物料数量.Rows[0]["出入库时间"] = t;
                            dt_仓库.ImportRow(dt_仓库物料数量.Rows[0]);
                        }
                    }
                }
                string sql_主表 = string.Format("select * from 销售记录销售订单主表 where 销售订单号 = '{0}'", str_销售单号);
                //
                string sql_mx = string.Format("select * from 销售记录销售订单明细表 where 生效=1 and 关闭=0 and  销售订单号 = '{0}'", str_销售单号);
                //dst = fun_明细_完成操作(dt_销售明细, sql_主表, sql_mx, false);
                dst = fun_明细_完成操作(dt_主状态, sql_主表, sql_mx, false);
                if (dt_状态.Columns.Count == 0)
                {
                    dt_状态 = dst.Clone();
                }
                foreach (DataRow dr in dt_仓库.Rows)
                {
                    if (Convert.ToDecimal(dr["库存总数"]) < 0)
                    {
                        throw new Exception("库存不足");
                    }

                }
                if (dst.Rows.Count > 0)
                {
                    dt_状态.ImportRow(dst.Rows[0]);
                    ds.Tables.Add(dt_状态);
                }

                ds.Tables.Add(dt_销售明细);
                ds.Tables.Add(dt_仓库);

                return ds;

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_出入库_成品出库");
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 库存总数↓ 未领量↓ MRP库存锁定量↓ （未测试）
        /// 领料出库时的影响 产生车间库存量 （未写）
        /// </summary>
        /// <param name="str_ItemNo"></param>
        /// <param name="Qty"></param>
        /// <param name="str_明细号">生产工单号</param>
        public static void fun_出入库_领料出库(string str_ItemNo, Decimal Qty, string str_明细号, string str_工单号)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                str_ItemNo = str_ItemNo.Trim(); str_明细号 = str_明细号.Trim();
                string sql2 = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}'", str_ItemNo);
                DataTable dt_仓库物料数量 = new DataTable();
                SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
                da2.Fill(dt_仓库物料数量);
                //数量变化保存到 仓库物料数量表  库存总数↓ 未领量↓ MRP库存锁定量↓
                //dt_仓库物料数量.Rows[0]["库存总数"] = fun_出入库_物料库存总量(str_ItemNo, -Qty);
                dt_仓库物料数量.Rows[0]["MRP库存锁定量"] = Convert.ToDecimal(dt_仓库物料数量.Rows[0]["MRP库存锁定量"]) - Qty;
                if (Convert.ToDecimal(dt_仓库物料数量.Rows[0]["MRP库存锁定量"]) < 0)
                {
                    dt_仓库物料数量.Rows[0]["MRP库存锁定量"] = 0;
                }

                //领料出库时的影响               
                DataTable dt_工单 = fun_领料出库_工单领料(str_ItemNo, str_明细号, Qty, strconn);
                // DataTable dt_制令 = fun_领料出库_制令领料(str_ItemNo, str_明细号, Qty, strconn);

                DataTable dt_制令 = new DataTable();
                if (str_工单号 != "")
                { dt_制令 = fun_领料出库_制令领料(str_ItemNo, str_工单号, Qty, strconn); }

                string sql_工单 = "select * from 生产记录生产工单待领料明细表 where 1<>1";
                string sql_制令 = "select * from 生产记录生产制令单待领料表 where 1<>1";
                string sql_2 = "select * from 仓库物料数量表 where 1<>1";
                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("初始化");
                try
                {
                    SqlCommand cmm_2 = new SqlCommand(sql_2, conn, ts);
                    SqlCommand cmm_工单 = new SqlCommand(sql_工单, conn, ts);
                    SqlCommand cmm_制令 = new SqlCommand(sql_制令, conn, ts);
                    SqlDataAdapter da_2 = new SqlDataAdapter(cmm_2);
                    SqlDataAdapter da_工单 = new SqlDataAdapter(cmm_工单);
                    SqlDataAdapter da_制令 = new SqlDataAdapter(cmm_制令);
                    new SqlCommandBuilder(da_2);
                    new SqlCommandBuilder(da_工单);
                    new SqlCommandBuilder(da_制令);
                    try
                    {
                        da_工单.Update(dt_工单);
                        da_制令.Update(dt_制令);
                    }
                    catch (Exception ex)
                    {

                        CZMaster.MasterLog.WriteLog("领料出库_工单or制令的已领数量或保存出错", "stocker_领料出库");
                        throw ex;
                    }
                    da_2.Update(dt_仓库物料数量);
                    ts.Commit();
                }
                catch (Exception ex)
                {
                    ts.Rollback();
                    throw ex;
                }
                //刷新有效总数和5种量 待领量↓
                //fun_物料数量_实际数量(str_ItemNo, true);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_出入库_领料出库");
                throw ex;
            }
        }

        #region 外协
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// </summary>
        /// <param name="str_ItemNo"></param>
        /// <param name="Qty"></param>
        /// <param name="str_明细号">生产工单号</param>
        public static void fun_出入库_外协领料出库(string str_ItemNo, Decimal Qty, string str_明细号)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                str_ItemNo = str_ItemNo.Trim(); str_明细号 = str_明细号.Trim();
                //string sql2 = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}'", str_ItemNo);
                //DataTable dt_仓库物料数量 = new DataTable();
                //SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
                //da2.Fill(dt_仓库物料数量);
                //数量变化保存到 仓库物料数量表  库存总数↓ 未领量↓ MRP库存锁定量↓
                //dt_仓库物料数量.Rows[0]["库存总数"] = fun_出入库_物料库存总量(str_ItemNo, -Qty);
                //dt_仓库物料数量.Rows[0]["MRP库存锁定量"] = Convert.ToDecimal(dt_仓库物料数量.Rows[0]["MRP库存锁定量"]) - Qty;

                //领料出库时的影响               
                DataTable dt_工单 = fun_领料出库_外协领料(str_ItemNo, str_明细号, Qty, strconn);
                //DataTable dt_制令 = fun_领料出库_制令领料(str_ItemNo, str_明细号, Qty, strconn);
                string sql_工单 = "select * from 采购记录外协采购待领料明细表 where 1<>1";
                //string sql_制令 = "select * from 生产记录生产制令单待领料表 where 1<>1";
                //string sql_2 = "select * from 仓库物料数量表 where 1<>1";
                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("初始化");
                try
                {
                    //SqlCommand cmm_2 = new SqlCommand(sql_2, conn, ts);
                    SqlCommand cmm_工单 = new SqlCommand(sql_工单, conn, ts);
                    //SqlCommand cmm_制令 = new SqlCommand(sql_制令, conn, ts);
                    //SqlDataAdapter da_2 = new SqlDataAdapter(cmm_2);
                    SqlDataAdapter da_工单 = new SqlDataAdapter(cmm_工单);
                    //SqlDataAdapter da_制令 = new SqlDataAdapter(cmm_制令);
                    //new SqlCommandBuilder(da_2);
                    new SqlCommandBuilder(da_工单);
                    //new SqlCommandBuilder(da_制令);
                    try
                    {
                        da_工单.Update(dt_工单);
                        //da_制令.Update(dt_制令);
                    }
                    catch { CZMaster.MasterLog.WriteLog("领料出库_工单or制令的已领数量或保存出错", "stocker_领料出库"); }
                    //da_2.Update(dt_仓库物料数量);
                    ts.Commit();
                }
                catch (Exception ex)
                {
                    ts.Rollback();
                    throw ex;
                }
                //刷新有效总数和5种量 待领量↓
                //fun_物料数量_实际数量(str_ItemNo, true);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_出入库_领料出库");
                throw ex;
            }
        }
#pragma warning disable IDE1006 // 命名样式
        public static DataTable fun_领料出库_外协领料(string str_ItemNo, string str_生产工单, Decimal Qty, string strconn)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = string.Format("select * from 采购记录外协采购待领料明细表 where 采购单号 = '{0}' and 物料编码 = '{1}'", str_生产工单, str_ItemNo);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                dt.Rows[0]["已领数量"] = Convert.ToDecimal(dt.Rows[0]["已领数量"]) + Qty;
                dt.Rows[0]["未领数量"] = Convert.ToDecimal(dt.Rows[0]["未领数量"]) - Qty;
                //判断领料是否完成
                if (Convert.ToDecimal(dt.Rows[0]["未领数量"]) <= Convert.ToDecimal(0))
                {
                    dt.Rows[0]["完成"] = true;
                    dt.Rows[0]["完成日期"] = System.DateTime.Now;
                }
                return dt;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "fun_领料出库_外协领料");
                throw ex;
            }
        }
        #endregion

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 似乎没用     12-30 物料运算中已经刷新 
        /// 赵峰 2015-12-18
        /// （刷新）计算仓库物料数量表中的 库存总数
        /// </summary>
        /// <param name="str_ItemNo">物料编码</param>
        /// <param name="Qty">数量</param>
        /// <param name="str_库位号">库位号，不用</param>
        /// <returns></returns>
        public static Decimal fun_出入库_物料库存总量(string str_ItemNo, Decimal Qty, string str_库位号 = "")
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                str_ItemNo = str_ItemNo.Trim();
                Decimal dec = 0;
                string str_盘点有效批次号 = "";
                {
                    string sql1 = string.Format("select * from 仓库物料表 where 物料编码 = '{0}'", str_ItemNo);
                    DataTable dt1 = new DataTable();
                    SqlDataAdapter da1 = new SqlDataAdapter(sql1, strconn);
                    da1.Fill(dt1);
                    dec = Convert.ToDecimal(dt1.Rows[0]["库存数量"]);
                    str_盘点有效批次号 = dt1.Rows[0]["盘点有效批次号"].ToString();
                }
                //出入库明细刷新 库存总数
                string sql = string.Format("select * from 仓库出入库明细表 where 物料编码 = '{0}' and 盘点有效批次号 = '{1}'", str_ItemNo, str_盘点有效批次号);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                Decimal dec_库存总数 = dec;
                try
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        dec_库存总数 = dec_库存总数 + Convert.ToDecimal(r["实效数量"]);
                    }
                    //dec_库存总数 = dec_库存总数 + Qty;  //这个似乎没用 
                }
                catch
                {
                    dec_库存总数 = dec;
                }
                return dec_库存总数;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_仓库物料数量_库存总量");
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 赵峰 2015-12-18
        /// 17-2修改
        /// 判断明细是否完成
        /// 返回dt数组，用于事务保存
        /// </summary>
        /// <param name="str_sql明细语句">sql明细语句</param>
        /// <param name="str_sql主表语句">sql主表语句</param>
        /// 因为 采购明细的 明细完成字段是 送检的时候 赋的 ，但是明细完成时间是 入库时候赋的 这里为判断是否已入库加一个参数 判断是销售还是采购，采购用 明细完成时间判断
        /// <returns></returns>
        private static DataTable fun_明细_完成操作(DataTable dtl, string str_sql主表语句, string sql_明细, bool flag)    // true 表示采购 fals 表示 销售
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                //DataSet dst = new DataSet();

                DataTable dt = dtl;
                string sqll = str_sql主表语句;
                DataTable dtt = new DataTable();
                DataTable dt_mx = new DataTable();
                SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn);
                daa.Fill(dtt);
                SqlDataAdapter da_mx = new SqlDataAdapter(sql_明细, strconn);
                da_mx.Fill(dt_mx);
                int i = 0;
                foreach (DataRow r in dt_mx.Rows)
                {
                    if (flag)  // 采购
                    {
                        if (r["明细完成日期"].ToString() != "")
                        {
                            i = i + 1;
                        }
                    }
                    else
                    {
                        if (r["明细完成"].Equals(true))
                        {
                            i = i + 1;
                        }
                    }
                }

                if (i == dt_mx.Rows.Count - dt.Rows.Count)
                {
                    dtt.Rows[0]["完成"] = 1;
                    dtt.Rows[0]["完成日期"] = CPublic.Var.getDatetime();

                }
                else { }
                //try
                //{
                //dt.TableName = "明细";
                //dst.Tables.Add(dt);
                //dtt.TableName = "主表";
                //dst.Tables.Add(dtt);
                return dtt;

                //}
                //catch
                //{  }
                //return dst;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message);

                throw new Exception("出入库操作_完成赋值时出错");
            }
        }

        #region 成品入库时影响
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 17/9/18 修改
        /// </summary>
        public static DataTable fun_成品出库_工单状态(DataTable dt_mx)
#pragma warning restore IDE1006 // 命名样式
        {

            DataTable dt = new DataTable();


            //先把选中的 属于一个工单的 准备入库数量、已入库数量 合格数量 合并  
            int c = dt_mx.Rows.Count;
            for (int i = 0; i < c; i++)
            {
                DataRow[] r = dt_mx.Select(string.Format("生产工单号='{0}' and 生产检验单号<>'{1}' ", dt_mx.Rows[i]["生产工单号"].ToString(), dt_mx.Rows[i]["生产检验单号"].ToString()));
                if (r.Length >= 1)
                {
                    foreach (DataRow xr in r)
                    {
                        dt_mx.Rows[i]["入库数量"] = Convert.ToDecimal(dt_mx.Rows[i]["入库数量"]) + Convert.ToDecimal(xr["入库数量"]);
                        dt_mx.Rows[i]["已入库数量"] = Convert.ToDecimal(dt_mx.Rows[i]["已入库数量"]) + Convert.ToDecimal(xr["已入库数量"]);

                        dt_mx.Rows[i]["合格数量"] = Convert.ToDecimal(dt_mx.Rows[i]["合格数量"]) + Convert.ToDecimal(xr["合格数量"]);

                        dt_mx.Rows.Remove(xr);
                        c--;
                    }
                }
            }

            //因 dt_mx 已经是将同一工单的的 检验单的  入库数量合格数量 已入库数量 等 合并为一条 
            foreach (DataRow dr in dt_mx.Rows)
            {
                //17/12/5发现 一个工单 分批检验 都检验完成了 但是 只入一个检验单的全部数量 会把工单赋成完成
                //  因为工单上并没有 已入库数量,所以这边先判断一下 此次入库数量 +  已入库数量是否>= 生产数量   dr["入库数量"]+  另外按工单搜索总已入库数量 >= 生产数量 ? 继续：continue；
                string s = string.Format(@"select  生产记录生产工单表.生产工单号,isnull(已入库数量,0)已入库数量,ISNULL(a.报废,0)报废,生产数量 from 生产记录生产工单表
                left JOIN ( select  生产工单号,SUM(已入库数量)已入库数量,SUM(报废数) 报废  from  生产记录生产检验单主表  
                where 生产工单号='{0}'  group by 生产工单号)a  on  生产记录生产工单表.生产工单号=a.生产工单号
                where 生产记录生产工单表.生产工单号='{0}'", dr["生产工单号"].ToString());
                DataTable temp = new DataTable();
                using (SqlDataAdapter aa = new SqlDataAdapter(s, strconn))
                {
                    aa.Fill(temp);
                }
                if (Convert.ToDecimal(temp.Rows[0]["已入库数量"]) + Convert.ToDecimal(dr["入库数量"]) + Convert.ToDecimal(temp.Rows[0]["报废"]) >= Convert.ToDecimal(temp.Rows[0]["生产数量"]))
                {// 上面的 已入库数量是 用工单 groupby  检验单上的 已入库数量  和下面的 已入库数量不一样  

                    string sql = string.Format(@"select 生产记录生产工单表.*,合格数量,生产检验单号,生产记录生产检验单主表.重检合格数  from 生产记录生产工单表,生产记录生产检验单主表 
                                                where 生产记录生产工单表.生产工单号=生产记录生产检验单主表.生产工单号 and  生产记录生产检验单主表.生产检验单号 = '{0}' 
                                                    and 生产记录生产工单表.物料编码 = '{1}'", dr["生产检验单号"], dr["物料编码"]);

                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt);
                    DataRow[] rr = dt.Select(string.Format("生产检验单号='{0}'", dr["生产检验单号"].ToString()));
                    //入库量大于等于工单所有已检验数量  
                    if (rr[0]["完工"].Equals(true))
                    {
                        if (Convert.ToDecimal(dr["入库数量"]) + Convert.ToDecimal(dr["已入库数量"]) >= Convert.ToDecimal(rr[0]["合格数量"]) + Convert.ToDecimal(rr[0]["重检合格数"]) && Convert.ToDecimal(rr[0]["已检验数量"]) >= Convert.ToDecimal(rr[0]["生产数量"]))
                        {
                            string sql_退 = string.Format("select count(*)x from 工单退料申请表 where 生产工单号 = '{0}' and 完成=0 and 作废=0", dr["生产工单号"].ToString());
                            DataTable dt_tui = CZMaster.MasterSQL.Get_DataTable(sql_退, strconn);
                            if (Convert.ToInt32(dt_tui.Rows[0]["x"]) > 0)
                            {
                                throw new Exception("该单据有退料申请未完成，不可操作");
                            }
                            rr[0]["完成"] = true;
                            rr[0]["完成日期"] = CPublic.Var.getDatetime();
                        }

                    }
                }




            }


            return dt;

        }
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 17-9-18 
        /// 因 fun_成品出库_工单状态已将同一工单的数据合并 所以这部dtp 不存在重复的 工单记录
        /// 但是不同工单可能为同一制令
        /// dtP 为需要刷新状态的 工单列表 
        ///
        /// </summary>
        /// <param name="dt_mx"></param>
        /// <returns></returns>
        public static DataTable fun_成品出库_制令状态(DataTable dtP)
#pragma warning restore IDE1006 // 命名样式
        {
            DataTable dt = new DataTable();
            DataTable dtx = dtP.Copy();
            // 这里把 入库明细  根据  生产制令单号 汇总数量
            //int c = dtx.Rows.Count;
            //for (int i = 0; i < c; i++)
            //{
            //    DataRow[] r = dtx.Select(string.Format("生产制令单号='{0}' and 生产检验单号<>'{1}' ", dtx.Rows[i]["生产制令单号"].ToString(), dtx.Rows[i]["生产检验单号"].ToString()));
            //    if (r.Length >= 1)
            //    {
            //        foreach (DataRow xr in r)
            //        {
            //            dtx.Rows[i]["入库数量"] = Convert.ToDecimal(dtx.Rows[i]["入库数量"]) + Convert.ToDecimal(xr["入库数量"]);
            //            dtx.Rows[i]["合格数量"] = Convert.ToDecimal(dtx.Rows[i]["合格数量"]) + Convert.ToDecimal(xr["合格数量"]);

            //            dtx.Rows.Remove(xr);
            //            c--;
            //        }
            //    }
            //}

            //所有需要判断的制令列表
            int cc = dtx.Rows.Count;

            //int c = dtx.Rows.Count;
            //for (int i = 0; i < c; i++)
            //{
            //    DataRow[] r = dtx.Select(string.Format("生产制令单号='{0}' and 生产检验单号<>'{1}' ", dtx.Rows[i]["生产制令单号"].ToString(), dtx.Rows[i]["生产检验单号"].ToString()));
            //    if (r.Length >= 1)
            //    {
            //        foreach (DataRow xr in r)
            //        {
            //            dtx.Rows[i]["入库数量"] = Convert.ToDecimal(dtx.Rows[i]["入库数量"]) + Convert.ToDecimal(xr["入库数量"]);
            //            dtx.Rows[i]["合格数量"] = Convert.ToDecimal(dtx.Rows[i]["合格数量"]) + Convert.ToDecimal(xr["合格数量"]);

            //            dtx.Rows.Remove(xr);
            //            c--;
            //        }
            //    }
            //}
            for (int i = 0; i < cc; i++)
            {
                DataRow[] r = dtx.Select(string.Format("生产制令单号='{0}' and 生产检验单号<>'{1}'", dtx.Rows[i]["生产制令单号"].ToString(), dtx.Rows[i]["生产检验单号"].ToString()));
                if (r.Length >= 1)
                {
                    //这里是把工单一样的删掉，完成状态表示此次生效的n个工单是否全部完成,若全部完成 下面继续判断 制令是否完成



                    foreach (DataRow xr in r)
                    {
                        if (xr["完成"].Equals(false))
                        {
                            dtx.Rows[i]["完成"] = false;
                        }
                        //dtP.Rows[i]["入库数量"] = Convert.ToDecimal(dtP.Rows[i]["入库数量"]) + Convert.ToDecimal(xr["生产数量"]);
                        //dtP.Rows[i]["合格数量"] = Convert.ToDecimal(dtP.Rows[i]["合格数量"]) + Convert.ToDecimal(xr["合格数量"]);

                        dtx.Rows.Remove(xr);
                        cc--;
                    }

                }
                //foreach (DataRow rr in  r)
                //{
                //    x=Convert.ToInt32(dr[""]);

                //}
            }

            foreach (DataRow dr in dtx.Rows) //dtp为所有需要
            {
                if (dr["完成"].Equals(true)) // 这里传过来的 是已处理的 dt 工单状态已正确赋值 如果工单完成 继续判断
                {

                    string sql = string.Format(@"select 生产记录生产制令表.*,isnull(x.已入库数,0)已入库数 from 生产记录生产制令表
                    left join  (select  生产制令单号,SUM(生产数量)已入库数 from 生产记录生产工单表 b  where 完成=1  group by 生产制令单号 )x
                    on x.生产制令单号=生产记录生产制令表.生产制令单号 where   生产记录生产制令表.生产制令单号 = '{0}' and 物料编码 = '{1}'", dr["生产制令单号"].ToString(), dr["物料编码"].ToString());
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_制令 = new DataTable();
                    da.Fill(dt_制令);
                    //DataRow[] rr = dt_制令.Select(string.Format("生产制令单号='{0}'", dr["生产制令单号"]));
                    //先判断该制令是否 已排单数量>=制令数量  是  则继续判断 
                    if (dt_制令.Rows.Count == 0) return dt;
                    else if (Convert.ToInt32(dt_制令.Rows[0]["已排单数量"]) >= Convert.ToInt32(dt_制令.Rows[0]["制令数量"]))
                    {
                        //全部制令数都转工单的前提下   判断已完成工单数
                        int i_总单数 = 0;
                        int i_完成单数 = 0;
                        string ss = string.Format("select count(*)总数  from 生产记录生产工单表 where 关闭=0 and 生效=1 and 生产制令单号='{0}'", dr["生产制令单号"].ToString());
                        using (SqlDataAdapter aa = new SqlDataAdapter(ss, strconn))
                        {
                            DataTable dtt = new DataTable();
                            aa.Fill(dtt);
                            i_总单数 = Convert.ToInt32(dtt.Rows[0][0].ToString());


                        }
                        ss = string.Format("select count(*)总数  from 生产记录生产工单表 where 关闭=0 and 生效=1 and 完成=1 and  生产制令单号='{0}'", dr["生产制令单号"].ToString());
                        using (SqlDataAdapter aa = new SqlDataAdapter(ss, strconn))
                        {
                            DataTable dtt = new DataTable();
                            aa.Fill(dtt);
                            i_完成单数 = Convert.ToInt32(dtt.Rows[0][0].ToString());
                        }
                        DataRow[] r = dtP.Select(string.Format("生产制令单号='{0}' and 完成=1 ", dr["生产制令单号"].ToString()));
                        int i_此次 = r.Length;



                        //int i_此次 = r.Length;

                        if (i_总单数 == i_完成单数 + i_此次)
                        {
                            if (dt.Columns.Count == 0)
                            {

                                dt = dt_制令.Copy();
                            }
                            else
                            {
                                dt.ImportRow(dt_制令.Rows[0]);
                            }
                            DataRow[] rr = dt.Select(string.Format("生产制令单号='{0}'", dr["生产制令单号"]));
                            rr[0]["完成"] = 1;
                            rr[0]["完成日期"] = CPublic.Var.getDatetime();
                        }


                    }
                    else
                    {
                        continue;
                    }


                    //先搜索这批生效的 所有的 同一制令的  sum(入库数量)  最后 在 sum()该制令 所有已入库数量  求和 与制令数量比较


                    //然后   同一制令的数量 已合并 

                    //foreach (DataRow rx in dtP.Rows)
                    //{
                    //这里统计已入库数量 要 sum(已完成工单的 生产数量 ) 因为 一个工单 100  入99 然后 完成这张工单这样的状况
                    //                             string sql = string.Format(@"select 生产记录生产制令表.*,isnull(x.已入库数,0)已入库数 from 生产记录生产制令表
                    //                    left join  (select  生产制令单号,SUM(入库数量)已入库数 from 生产记录成品入库单明细表 a,生产记录生产工单表 b  where a.生产工单号=b.生产工单号 group by 生产制令单号 )x
                    //                    on x.生产制令单号=生产记录生产制令表.生产制令单号 where   生产记录生产制令表.生产制令单号 = '{0}' and 物料编码 = '{1}'", rx["生产制令单号"].ToString(), rx["物料编码"]);

                    //                             string sql = string.Format(@"select 生产记录生产制令表.*,isnull(x.已入库数,0)已入库数 from 生产记录生产制令表
                    //                    left join  (select  生产制令单号,SUM(生产数量)已入库数 from 生产记录生产工单表 b  where 完成=1  group by 生产制令单号 )x
                    //                    on x.生产制令单号=生产记录生产制令表.生产制令单号 where   生产记录生产制令表.生产制令单号 = '{0}' and 物料编码 = '{1}'", rx["生产制令单号"].ToString(), rx["物料编码"]);


                    //                             SqlDataAdapter da = new SqlDataAdapter(sql, strconn);

                    //                             da.Fill(dt_制令);
                    //                             DataRow[] rr = dt_制令.Select(string.Format("生产制令单号='{0}'", rx["生产制令单号"]));
                    //                             DataRow[] r_生效数 = dtx.Select(string.Format("生产制令单号='{0}'", rx["生产制令单号"]));
                    //                             int i_此次数量 = 0;
                    //                             if (r_生效数.Length >0)
                    //                             {
                    //                                 i_此次数量 = Convert.ToInt32(r_生效数[0]["入库数量"]);
                    //                             }
                    //                             if (Convert.ToInt32(rr[0]["制令数量"]) <= Convert.ToInt32(rr[0]["已入库数"]) +i_此次数量)
                    //                             {
                    //                                 rr[0]["完成"] = 1;
                    //                                 rr[0]["完成日期"] = CPublic.Var.getDatetime();
                    //                             }

                    //                         }

                    //                     else
                    //                     {
                    //                         continue;
                    //                     }




                    //    string sql1 = string.Format("select * from 生产记录生产工单表 where 生产工单号 = '{0}' and 物料编码 = '{1}'", dr["生产工单号"], dr["物料编码"]);
                    //    DataTable dt1 = new DataTable();
                    //    SqlDataAdapter da1 = new SqlDataAdapter(sql1, strconn);
                    //    da1.Fill(dt1);
                    //    string sql = string.Format("select * from 生产记录生产制令表 where 生产制令单号 = '{0}' and 物料编码 = '{1}'", dt1.Rows[0]["生产制令单号"].ToString(), dr["物料编码"]);
                    //    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    //    DataTable dt2 = new DataTable();
                    //    da.Fill(dt2);
                    //    if (dt.Columns.Count == 0)
                    //    {
                    //        dt = dt2.Copy();
                    //    }
                    //    else
                    //    {
                    //        DataRow[] r = dt.Select(string.Format("生产制令单号='{0}'", dt2.Rows[0]["生产制令单号"]));
                    //        if (r.Length == 0)
                    //        {
                    //            dt.ImportRow(dt2.Rows[0]);

                    //        }
                    //    }
                    //}

                    //    try
                    //    {
                    //        foreach (DataRow dx in dt.Rows)
                    //        {
                    //            //判断制令单是否完成
                    //            if (Convert.ToDecimal(dx["已排单数量"]) >= Convert.ToDecimal(dx["制令数量"]))
                    //            {
                    //                dt.Rows[0]["完成"] = true;
                    //                dt.Rows[0]["完成日期"] = CPublic.Var.getDatetime() ;
                    //            }
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        //wushi
                    //    }
                    //return dt;
                    //}

                }

            }
            return dt;
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 改变 生产工单 和 生产制令表 中的 已生产数量 未生产数量，判断是否完成
        /// 17-9-18 因分批检验的入库后状态有问题,改
        /// </summary>
        /// <param name="str_生产工单"></param>
        /// <param name="Qty">已入库数量总</param>
        /// <param name="strconn"></param>
        /// <returns></returns>
        private static DataTable fun_成品入库_工单_已生产数量(string str_ItemNo, string str_生产工单, Decimal Qty, string strconn, string str_检验单号)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = string.Format(@"select 生产记录生产工单表.*,合格数量,生产记录生产检验单主表.重检合格数  from 生产记录生产工单表,生产记录生产检验单主表 
                                                where 生产记录生产工单表.生产工单号=生产记录生产检验单主表.生产工单号 and  生产记录生产检验单主表.生产检验单号 = '{0}' 
                                                    and 生产记录生产工单表.物料编码 = '{1}'", str_检验单号, str_ItemNo);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                //dt.Rows[0]["已检验数量"] = Convert.ToDecimal(dt.Rows[0]["已检验数量"]) + Qty; //20180801,因为在检验单生效的时候已经计算过已检数量和未检数量，所以将该行注释
                //dt.Rows[0]["未检验数量"] = Convert.ToDecimal(dt.Rows[0]["未检验数量"]) - Qty; //8.1之前的数据，已检数量都是工单数的两倍
                //判断工单是否完成  //12/5 入库数量=合格数+重检合格数
                if (dt.Rows[0]["完工"].Equals(true))
                {
                    if (Qty >= Convert.ToDecimal(dt.Rows[0]["合格数量"]) + Convert.ToDecimal(dt.Rows[0]["重检合格数"]))
                    {
                        dt.Rows[0]["完成"] = true;
                        dt.Rows[0]["完成日期"] = CPublic.Var.getDatetime();
                    }
                    else //2017-1-3 分批入库完成状态赋不进去
                    {
                        string sql_复验 = string.Format(@"select  isnull(SUM(入库数量),0) as 已入库数量 from  生产记录成品入库单明细表  where 生产工单号 = '{0}' and 生效=1 and 作废=0", str_生产工单);
                        DataTable dt_复验 = new DataTable();
                        dt_复验 = CZMaster.MasterSQL.Get_DataTable(sql_复验, strconn);
                        if (dt_复验.Rows.Count > 0)
                        {
                            if (Qty >= Convert.ToDecimal(dt.Rows[0]["合格数量"]) + Convert.ToDecimal(dt.Rows[0]["重检合格数"]) - Convert.ToDecimal(dt_复验.Rows[0]["已入库数量"]))
                            {
                                dt.Rows[0]["完成"] = true;
                                dt.Rows[0]["完成日期"] = CPublic.Var.getDatetime();
                            }

                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "fun_成品入库_工单_已生产数量");
                throw ex;
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private static DataTable fun_成品入库_制令_已生产数量(string str_ItemNo, string str_生产工单, Decimal Qty, string strconn)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql1 = string.Format("select * from 生产记录生产工单表 where 生产工单号 = '{0}' and 物料编码 = '{1}'", str_生产工单, str_ItemNo);
                DataTable dt1 = new DataTable();
                SqlDataAdapter da1 = new SqlDataAdapter(sql1, strconn);
                da1.Fill(dt1);
                string sql = string.Format("select * from 生产记录生产制令表 where 生产制令单号 = '{0}' and 物料编码 = '{1}'", dt1.Rows[0]["生产制令单号"].ToString(), str_ItemNo);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                try
                {
                    //dt.Rows[0]["已排单数量"] = Convert.ToDecimal(dt.Rows[0]["已排单数量"]) + Qty;
                    //dt.Rows[0]["未排单数量"] = Convert.ToDecimal(dt.Rows[0]["未排单数量"]) - Qty;
                    //判断制令单是否完成
                    if (Convert.ToDecimal(dt.Rows[0]["已排单数量"]) >= Convert.ToDecimal(dt.Rows[0]["制令数量"]))
                    {
                        dt.Rows[0]["完成"] = true;
                        dt.Rows[0]["完成日期"] = System.DateTime.Now;
                    }
                }
                catch (Exception)
                {
                    //wushi
                }
                return dt;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "fun_成品入库_制令_已生产数量");
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 有问题  以检验数量什么时候扣  2016-01-12  没有被调用
        /// </summary>
        /// <param name="str_生产线">生产线</param>
        /// <param name="str_ItemNo">产品编码</param>
        /// <param name="str_生产工单">该检验单所属的生产工单号</param>
        /// <param name="Qty">检验数量</param>
        /// <param name="strconn"></param>
        public static void fun_成品入库_车间虚拟库存(string str_生产线, string str_ItemNo, string str_生产工单, Decimal Qty, string strconn)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                //获取产品 str_ItemNo 的BOM结构
                string sql_BOM = string.Format("select * from 基础数据物料BOM表 where 产品编码 = '{0}'", str_ItemNo);
                DataTable dt_BOM = new DataTable();
                SqlDataAdapter da_BOM = new SqlDataAdapter(sql_BOM, strconn);
                da_BOM.Fill(dt_BOM);
                //获取生产工单号中的生产数量
                string sql_生产数量 = string.Format("select * from 生产记录生产工单表 where 生产工单号 = '{0}'", str_生产工单);
                DataTable dt_生产数量 = new DataTable();
                SqlDataAdapter da__生产数量 = new SqlDataAdapter(sql_生产数量, strconn);
                da__生产数量.Fill(dt_生产数量);
                Decimal dec__生产数量 = Convert.ToDecimal(dt_生产数量.Rows[0]["生产数量"]);
                Decimal dec__已检验数量 = Convert.ToDecimal(dt_生产数量.Rows[0]["已检验数量"]);
                //对于 dt_BOM 中的每一行
                foreach (DataRow r in dt_BOM.Rows)
                {
                    string sql = string.Format("select * from 生产记录车间虚拟库存表 where 生产线 = '{0}' and 物料编码 = '{1}' and 生产工单号 = '{2}'", str_生产线, r["子项编码"].ToString(), str_生产工单);
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        //Qty/dec__生产数量 * 
                        dt.Rows[0]["已用数量"] = Convert.ToDecimal(dt.Rows[0]["已用数量"]) + Qty * Convert.ToDecimal(r["数量"].ToString());
                        dt.Rows[0]["未用数量"] = Convert.ToDecimal(dt.Rows[0]["未用数量"]) - Qty * Convert.ToDecimal(r["数量"].ToString());
                    }
                    else
                    {
                        //不应该没有
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCore_fun_成品入库_车间虚拟库存");
                throw ex;
            }
        }

        #endregion

        #region 领料出库时影响
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 工单领料完成，制令领料改变 已完成数量 未完成数量
        /// </summary>
        /// <param name="str_生产工单"></param>
        /// <param name="Qty">本次领料出库的数量</param>
        /// <param name="strconn"></param>
        /// <returns></returns>
        public static DataTable fun_领料出库_工单领料(string str_ItemNo, string str_待领料单号, Decimal Qty, string strconn)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = string.Format("select * from 生产记录生产工单待领料明细表 where 待领料单号 = '{0}' and 物料编码 = '{1}'", str_待领料单号, str_ItemNo);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                dt.Rows[0]["已领数量"] = Convert.ToDecimal(dt.Rows[0]["已领数量"]) + Qty;
                dt.Rows[0]["未领数量"] = Convert.ToDecimal(dt.Rows[0]["未领数量"]) - Qty;
                //判断领料是否完成
                if (Convert.ToDecimal(dt.Rows[0]["未领数量"]) <= Convert.ToDecimal(0))
                {
                    dt.Rows[0]["未领数量"] = 0;
                    dt.Rows[0]["完成"] = true;
                    dt.Rows[0]["完成日期"] = System.DateTime.Now;
                }
                return dt;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "fun_领料出库_工单领料");
                throw ex;
            }
        }
#pragma warning disable IDE1006 // 命名样式
        public static DataTable fun_领料出库_制令领料(string str_ItemNo, string str_生产工单, Decimal Qty, string strconn)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = string.Format("select * from 生产记录生产工单待领料明细表 where 生产工单号 = '{0}' and 物料编码 = '{1}'", str_生产工单, str_ItemNo);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);

                DataTable dt2 = new DataTable();
                string sql2 = string.Format("select * from 生产记录生产制令单待领料表 where 生产制令单号 = '{0}' and 物料编码 = '{1}'", dt.Rows[0]["生产制令单号"].ToString(), str_ItemNo);
                SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
                da2.Fill(dt2);
                try
                {
                    dt2.Rows[0]["已领数量"] = Convert.ToDecimal(dt2.Rows[0]["已领数量"]) + Qty;
                    dt2.Rows[0]["未领数量"] = Convert.ToDecimal(dt2.Rows[0]["未领数量"]) - Qty;
                    //判断领料是否完成                                                             
                    if (Convert.ToDecimal(dt2.Rows[0]["未领数量"]) <= Convert.ToDecimal(0))
                    {
                        dt2.Rows[0]["未领数量"] = 0;
                        dt2.Rows[0]["完成"] = true;
                        dt2.Rows[0]["完成日期"] = System.DateTime.Now;
                    }
                }
                catch (Exception)
                {
                    //wushi 
                }
                return dt2;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "fun_领料出库_制令领料");
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 2016-1-27 没用到
        /// </summary>
        /// <param name="str_生产线"></param>
        /// <param name="str_ItemNo"></param>
        /// <param name="str_生产工单"></param>
        /// <param name="Qty"></param>
        /// <param name="strconn"></param>
        public static void fun_领料出库_车间虚拟库存(string str_生产线, string str_ItemNo, string str_生产工单, Decimal Qty, string strconn)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = string.Format("select * from 生产记录车间虚拟库存表 where 生产线 = '{0}' and 物料编码 = '{1}' and 生产工单号 = '{2}'", str_生产线, str_ItemNo, str_生产工单);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    //应该是补料
                    dt.Rows[0]["车间数量"] = Convert.ToDecimal(dt.Rows[0]["车间数量"]) + Qty;
                    dt.Rows[0]["未用数量"] = Convert.ToDecimal(dt.Rows[0]["未用数量"]) + Qty;
                }
                else
                {
                    string slq = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", str_ItemNo);
                    DataTable td = new DataTable();
                    SqlDataAdapter ad = new SqlDataAdapter(slq, strconn);
                    ad.Fill(td);
                    //领料生产
                    DataRow dr = dt.NewRow();
                    dt.Rows.Add(dr);
                    dr["GUID"] = System.Guid.NewGuid(); ;
                    dr["生产线"] = str_生产线;
                    dr["物料编码"] = str_ItemNo;
                    dr["生产工单号"] = str_生产工单;
                    try
                    {
                        dr["物料名称"] = td.Rows[0]["物料名称"].ToString();
                        dr["规格型号"] = td.Rows[0]["规格型号"].ToString();
                        dr["图纸编号"] = td.Rows[0]["图纸编号"].ToString();
                    }
                    catch { }
                    dr["车间数量"] = Qty;
                    dr["已用数量"] = 0;
                    dr["未用数量"] = Qty;
                }
                new SqlCommandBuilder(da);
                da.Update(dt);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCore_fun_领料出库_车间虚拟库存");
                throw ex;
            }
        }
        #endregion
        #endregion

        #region 盘点
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 根据 str_盘点有效批次号 盘点物料编码为 str_ItemNo 的数量
        /// </summary>
        /// <param name="str_ItemNo"></param>
        /// <param name="str_盘点有效批次号"></param>
        /// <returns></returns>
        public static Decimal fun_盘点(string str_ItemNo, string str_盘点有效批次号)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = string.Format("select * from 仓库物料表 where 物料编码 = '{0}'", str_ItemNo);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                //获取上次盘点数量
                Decimal dec_盘点数量 = Convert.ToDecimal(dt.Rows[0]["盘点数量"]);
                string sql_盘点 = string.Format("select * from 仓库出入库明细表 where 物料编码 = '{0}' and 盘点有效批次号 = '{1}'", str_ItemNo, str_盘点有效批次号);
                DataTable dt_盘点 = new DataTable();
                SqlDataAdapter da_盘点 = new SqlDataAdapter(sql_盘点, strconn);
                da_盘点.Fill(dt_盘点);
                foreach (DataRow r in dt_盘点.Rows)
                {
                    //计算本次盘点数量
                    if (r["出库入库"].ToString() == "入库")
                    {
                        dec_盘点数量 = dec_盘点数量 + Convert.ToDecimal(r["实效数量"]);
                    }
                    else
                    {
                        dec_盘点数量 = dec_盘点数量 - Convert.ToDecimal(r["实效数量"]);
                    }
                }
                //将盘点记录保存至盘点记录主表 子表

                //生成盘点号
                return dec_盘点数量;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_盘点");
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 对dt表中的每一行物料，更新仓库物料表中的盘点数量，库存总数，盘点有效批次号的值
        /// 刷新6种数量
        ///  2018-7-25 确认没有地方用到 
        /// </summary>
        /// <param name="dt_物料"></param>
        /// <param name="strconn"></param>
        public static void fun_盘点生效(DataTable dt_物料, string strconn)
#pragma warning restore IDE1006 // 命名样式
        {
            //foreach (DataRow r in dt_物料.Rows)
            //{
            //    string sql = string.Format("select * from 仓库物料表 where 物料编码 = '{0}'", r["物料编码"].ToString().Trim());
            //    DataTable dt = new DataTable();
            //    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            //    da.Fill(dt);
            //    dt.Rows[0]["库存数量"] = r["盘点数量"].ToString();
            //    dt.Rows[0]["盘点数量"] = r["盘点数量"].ToString();
            //    dt.Rows[0]["盘点有效批次号"] = r["盘点有效批次号"].ToString();
            //}
            //string sql_保存 = "select * from 仓库物料表 where 1<>1";
            //SqlDataAdapter da_保存 = new SqlDataAdapter(sql_保存, strconn);
            //new SqlCommandBuilder(da_保存);
            //da_保存.Update(dt_物料);
            //foreach (DataRow r in dt_物料.Rows)
            //{
            //    fun_物料数量_实际数量(r["物料编码"].ToString().Trim(), true);
            //}
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 确认日期5号，生效日期3号，则对4号的记录回滚【盘点有效批次号】
        /// </summary>
        /// <param name="time_生效日期"></param>
        /// <param name="time_单据日期"></param>
        /// <param name="str_盘点有效批次号"></param>
        public static void fun_盘点_回滚(DateTime time_生效日期, DateTime time_单据日期, string str_盘点有效批次号, string strconn)
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format("select * from 仓库出入库明细表 where 出入库时间 >= time_生效日期 and 出入库时间 <= time_单据日期", time_生效日期, time_单据日期);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow r in dt.Rows)
                {
                    r["盘点有效批次号"] = str_盘点有效批次号;
                }
                new SqlCommandBuilder(da);
                da.Update(dt);
            }
        }
        #endregion

        #region BOM核心代码
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 赵峰 2015-12-17
        /// 计算出物料的整个BOM结构
        /// </summary>
        /// <param name="iAs">递归的层级</param>
        /// <param name="iMax">最大递归层级次数</param>
        /// <param name="dt1">用于计算数据</param>
        /// <param name="dt2">用于保留合并后的数据</param>
        /// <param name="strconn">数据库连接字段</param>
        /// <returns></returns>
        private static DataTable fun_穷尽子项(int iAs, int iMax, DataTable dt1, DataTable dt2, string strconn)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                iAs++;
                if (iAs >= iMax) throw new Exception("获取失败");
                foreach (DataRow r in dt1.Rows)
                {
                    //20160420 当BOM结构中是原材料时，不往下找。因为未来电器的BOM结构有问题
                    string sqlss = string.Format("select 物料类型 from 基础数据物料信息表 where 物料编码 = '{0}'", r["子项编码"].ToString().Trim());
                    SqlDataAdapter dass = new SqlDataAdapter(sqlss, strconn);
                    DataTable dtss = new DataTable();
                    dass.Fill(dtss);
                    if (dtss.Rows.Count > 0 && dtss.Rows[0]["物料类型"].ToString() == "原材料")
                    {
                        continue;
                    }
                    try
                    {
                        string sql = string.Format(@"select 基础数据物料BOM表.*,a.原ERP物料编号 as 父项编号,b.原ERP物料编号 as 子项编号,
                       a.图纸编号 as 父项图纸,b.图纸编号 as 子项图纸, a.n原ERP规格型号 as 父项规格,b.n原ERP规格型号 as 子项规格 from 基础数据物料BOM表 
                            left join 基础数据物料信息表 a  on  a.物料编码=基础数据物料BOM表.产品编码 
                left join  基础数据物料信息表 b  on  b.物料编码=基础数据物料BOM表.子项编码
                where 产品编码 = '{0}' and 子项编码 <> ''and 子项类型<>'采购件' and BOM类型 = '物料BOM'", r["子项编码"].ToString());
                        DataTable t = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                        da.Fill(t);
                        t.Columns.Add("层级");
                        foreach (DataRow rrr in t.Rows)
                        {
                            rrr["层级"] = iAs + 1;
                        }
                        if (t.Rows.Count > 0)
                        {
                            //dt2.Merge(t);
                            fun_合并datatable(dt2, t);
                            fun_穷尽子项(iAs, iMax, t, dt2, strconn);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                return dt2;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                iAs--;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 赵峰 2015-12-17
        /// 根据物料的BOM结构，计算物料数量和BOM数量
        /// </summary>
        /// <param name="dt_BOM">BOM结构</param>
        /// <param name="dt_基础信息">BOM结构中所有物料的基础信息</param>
        /// <param name="dt_返回值">返回BOM结构中间和叶子的物料数量，BOM数量</param>
        /// <returns></returns>
        private static DataTable fun_计算数量(DataTable dt_BOM, DataTable dt_Parent, DataTable dt_基础信息, DataTable dt_返回值, Decimal dec)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                foreach (DataRow r in dt_Parent.Rows)
                {
                    DataRow[] d = dt_基础信息.Select(string.Format("物料编码 = '{0}'", r["子项编码"].ToString()));
                    DataRow dr = dt_返回值.NewRow();
                    dr["物料编码"] = r["子项编码"].ToString();
                    dr["上级物料"] = r["产品编码"].ToString();
                    try
                    {
                        dr["物料名称"] = d[0]["物料名称"].ToString();
                        dr["规格型号"] = d[0]["规格型号"].ToString();
                        dr["图纸编号"] = d[0]["图纸编号"].ToString();
                        dr["n原ERP规格型号"] = d[0]["n原ERP规格型号"].ToString();
                        dr["规格"] = d[0]["规格"].ToString();
                        dr["大类"] = d[0]["大类"].ToString();
                        dr["小类"] = d[0]["小类"].ToString();




                    }
                    catch { }
                    dr["BOM数量"] = Convert.ToDecimal(r["数量"]);
                    dr["物料数量"] = Convert.ToDecimal(r["数量"]) * dec;

                    DataRow[] sd = dt_BOM.Select(string.Format("产品编码 = '{0}'", r["子项编码"].ToString()));
                    if (sd.Length > 0 && d[0]["物料类型"].ToString() != "原材料")
                    {
                        dr["节点标记"] = "中间";
                        DataTable td = dt_BOM.Clone();
                        foreach (DataRow rd in sd)
                        {
                            td.Rows.Add(rd.ItemArray);
                        }
                        List<Decimal> li = new List<Decimal>();
                        li.Add(dec);
                        dec = Convert.ToDecimal(dr["物料数量"]);
                        fun_计算数量(dt_BOM, td, dt_基础信息, dt_返回值, dec);
                        dec = li[0];
                    }
                    else
                    {
                        dr["节点标记"] = "叶子";
                    }
                    dt_返回值.Rows.Add(dr);
                }
                return dt_返回值;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 得到BOM结构
        /// 返回值为DS
        /// DS包括4个datatable
        /// Dt1 名称 BOM,strItemNo所有BOM结构，多层，表结构同BOM表
        /// Dt4 名称 Parent,strItemNo的父项，单层，表结构同BOM表
        /// Dt2 名称 BOM_单,strItemNo引用的子项，单层，表结构同BOM表
        /// Dt3 名称 Item,包含DT1，DT2出现的物料，表结构同物料表
        /// </summary>
        /// <param name="strItemNo">物料编码</param>
        /// <param name="strconn">数据库连接字段</param>
        /// <param name="strVer">物料版本号</param>
        /// <returns></returns>
        public static DataSet fun_得到物料BOM结构(string strItemNo, string strconn, string strVer)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataSet ds = new DataSet();
                int iMax = 20;
                int i = 0;

                string sql1 = string.Format(@"select 基础数据物料BOM表.*,a.原ERP物料编号 as 父项编号,b.原ERP物料编号 as 子项编号
                    ,a.图纸编号 as 父项图纸,b.图纸编号 as 子项图纸,a.n原ERP规格型号 as 父项规格,b.n原ERP规格型号 as 子项规格
                  from 基础数据物料BOM表 
                left join 基础数据物料信息表 a  on  a.物料编码=基础数据物料BOM表.产品编码
                left join  基础数据物料信息表 b  on  b.物料编码=基础数据物料BOM表.子项编码
                            where 产品编码 = '{0}' and 子项类型<>'采购件' and BOM类型 = '物料BOM'", strItemNo);
                DataTable dt1 = new DataTable();
                dt1.TableName = "BOM";
                SqlDataAdapter da1 = new SqlDataAdapter(sql1, strconn);
                da1.Fill(dt1);
                dt1.Columns.Add("层级");
                DataTable tt = dt1.Clone();
                foreach (DataRow r in dt1.Rows)
                {
                    r["层级"] = i + 1;
                    DataRow dr = tt.NewRow();
                    dr.ItemArray = r.ItemArray;
                    tt.Rows.Add(dr);
                }
                DataTable dt11 = fun_合并datatable(dt1, fun_穷尽子项(i, iMax, dt1, tt, strconn));
                ds.Tables.Add(fun_层级(dt11));

                DataTable dt2 = new DataTable();
                dt2.TableName = "BOM_单";
                da1.Fill(dt2);
                ds.Tables.Add(dt2);

                DataTable dt3 = new DataTable();
                string sasasasas = string.Format("select * from 基础数据物料信息表 where 1<>1");
                SqlDataAdapter sasas = new SqlDataAdapter(sasasasas, strconn);
                sasas.Fill(dt3);
                dt3.TableName = "Item";
                foreach (DataRow r in dt1.Rows)
                {
                    try
                    {
                        if (dt3.Select(string.Format("物料编码 = '{0]'", r["产品编码"].ToString())).Length == 0)
                        {
                            string sql3 = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", r["产品编码"].ToString());
                            SqlDataAdapter da3 = new SqlDataAdapter(sql3, strconn);
                            da3.Fill(dt3);
                        }
                    }
                    catch
                    {
                        string sql3 = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", r["产品编码"].ToString());
                        SqlDataAdapter da3 = new SqlDataAdapter(sql3, strconn);
                        da3.Fill(dt3);
                    }
                    try
                    {
                        if (dt3.Select(string.Format("物料编码 = '{0]'", r["子项编码"].ToString())).Length == 0)
                        {
                            string sqlas = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", r["子项编码"].ToString());
                            SqlDataAdapter daas = new SqlDataAdapter(sqlas, strconn);
                            daas.Fill(dt3);
                        }
                    }
                    catch
                    {
                        string sqlas = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", r["子项编码"].ToString());
                        SqlDataAdapter daas = new SqlDataAdapter(sqlas, strconn);
                        daas.Fill(dt3);
                    }
                }
                ds.Tables.Add(dt3);

                DataTable dt4 = new DataTable();
                string sql4 = string.Format("select * from 基础数据物料BOM表 where 子项编码 = '{0}' and BOM类型 = '物料BOM'", strItemNo);
                dt4.TableName = "Parent";
                SqlDataAdapter da4 = new SqlDataAdapter(sql4, strconn);
                da4.Fill(dt4);
                ds.Tables.Add(dt4);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 得到单个物料所有子项
        /// 返回DT有二个部分组成
        /// 返回DT的第一部分同物料表
        /// 返回DT的第二部分，第1列物料数量，第2列节点标记：中间或叶子
        /// 可用于计算产品需要多少原料
        /// </summary>
        /// <param name="strItemNo"></param>
        /// <param name="strVer"></param>
        /// <param name="blALLItem"></param>
        /// <returns></returns>
        public static DataTable fun_物料_单_计算(string strItemNo, string strVer, string strconn, Boolean blALLItem)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataSet ds = fun_得到物料BOM结构(strItemNo, strconn, strVer);
                DataTable dt_BOM = ds.Tables[0];
                DataTable dt_Parent = ds.Tables[1];
                DataTable dt_基础信息 = ds.Tables[2];
                DataTable dt_返回值 = new DataTable();
                //int i = 0;
                //int iMax = 20;
                //int count = 1;
                //第一部分
                string sqla = "select 物料编码,物料名称,规格型号,图纸编号,n原ERP规格型号,大类,小类,规格  from 基础数据物料信息表 where 1<>1";
                SqlDataAdapter daa = new SqlDataAdapter(sqla, strconn);
                daa.Fill(dt_返回值);
                dt_返回值.Columns.Add("物料数量");
                dt_返回值.Columns.Add("上级物料");
                dt_返回值.Columns.Add("BOM数量");
                dt_返回值.Columns.Add("节点标记");
                dt_返回值.Columns.Add("层级");

                //第二部分
                Decimal dec = 1;
                //fun_数量(i, iMax, dt_BOM, dt_基础信息, dt_返回值, count, strconn);
                dt_返回值 = fun_计算数量(dt_BOM, dt_Parent, dt_基础信息, dt_返回值, dec);
                foreach (DataRow r in dt_返回值.Rows)
                {
                    DataRow[] dss = dt_BOM.Select(string.Format("子项编码 = '{0}'", r["物料编码"].ToString()));
                    r["层级"] = dss[0]["层级"].ToString();
                }
                return dt_返回值;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 得到dtItemS中所有物料的所有子项
        /// 返回DT有二个部分组成
        /// 返回DT的第一部分同物料表
        /// 返回值DT第二部分，第1列物料数量，第2列节点标记：中间或叶子
        /// </summary>
        /// <param name="strItemNo">dtItemS，第一列，物料编码，第二列，BOM版本，第三列，数量</param>
        /// <param name="strVer"></param>
        /// <param name="blALLItem"></param>
        /// <returns></returns>
        public static DataTable fun_物料_多_计算(DataTable dtItemS, string strVer, string strconn, Boolean blALLItem) //暂时没用
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataTable dt_返回值表 = null;
                foreach (DataRow r in dtItemS.Rows)
                {
                    DataTable t = fun_物料_单_计算(r["物料编码"].ToString(), strVer, strconn, blALLItem);
                    if (dt_返回值表 == null)
                    {
                        dt_返回值表 = t.Clone();
                    }
                    dt_返回值表 = fun_合并datatable(dt_返回值表, t);
                }
                return dt_返回值表;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 合并两张dt，并去掉重复dt
        /// </summary>
        /// <param name="dt1">合并后保留的dt</param>
        /// <param name="dt2">被合并掉的dt</param>
        /// <returns></returns>
        private static DataTable fun_合并datatable(DataTable dt1, DataTable dt2)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                foreach (DataRow r in dt2.Rows)
                {
                    DataRow[] ds = dt1.Select(string.Format("产品编码 = '{0}' and 子项编码 = '{1}'", r["产品编码"].ToString(), r["子项编码"].ToString()));
                    if (ds.Length == 0)
                    {
                        DataRow dr = dt1.NewRow();
                        dr.ItemArray = r.ItemArray;
                        dt1.Rows.Add(dr);
                    }
                }
                return dt1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private static DataTable fun_层级(DataTable dt)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataTable dts = dt;
                foreach (DataRow r in dts.Rows)
                {
                    DataRow[] ds = dts.Select(string.Format("子项编码 = '{0}'", r["子项编码"].ToString()));
                    if (ds.Length > 1)
                    {
                        int a = 0;
                        foreach (DataRow sr in ds)
                        {
                            if (a < Convert.ToInt32(sr["层级"]))
                            {
                                a = Convert.ToInt32(sr["层级"]);
                            }
                        }
                        foreach (DataRow sr2 in ds)
                        {
                            sr2["层级"] = a;
                        }
                    }
                }
                return dts;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 领料出库批次记录

        #endregion

        #region 计算需采购需生产量
#pragma warning disable IDE1006 // 命名样式
        public static DataTable fun_计划_需生产(DateTime time)
#pragma warning restore IDE1006 // 命名样式
        {
            string str = "";
            string str2 = "";

            if (CPublic.Var.LocalUserID != "admin")
            {
                /*取大小类对应关系*/
                string sqll = string.Format(@"SELECT a.物料类型名称 as 小类,a.计划员,b.物料类型名称 as 大类
  FROM 基础数据物料类型表 as a
  left join 基础数据物料类型表 as b on b.物料类型GUID = a.上级类型GUID
   where a.类型级别 = '小类' and a.计划员 = '{0}'", CPublic.Var.LocalUserID);
                DataTable dt_小类 = new DataTable();
                SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn);
                daa.Fill(dt_小类);
                sqll = string.Format(@"SELECT 物料类型名称 as 大类,计划员
  FROM 基础数据物料类型表 where 类型级别 = '大类' and 计划员 = '{0}' group by 物料类型名称,计划员", CPublic.Var.LocalUserID);
                DataTable dt_大类 = new DataTable();
                daa = new SqlDataAdapter(sqll, strconn);
                daa.Fill(dt_大类);

                if (dt_小类.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt_小类.Rows)
                    {
                        str = str + "or (基础数据物料信息表.小类 = '" + dr["小类"] + "' and 基础数据物料信息表.大类 = '" + dr["大类"] + "')";
                    }
                    // 暂时 加个半成品 公用
                    str = str + " or 基础数据物料信息表.大类='半成品'";

                    str = str.Substring(2, str.Length - 2);
                    str = "where (" + str + ")";
                }
                if (dt_大类.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt_大类.Rows)
                    {
                        str2 = str2 + "or 基础数据物料信息表.大类 = '" + dr["大类"] + "'";
                    }
                    str2 = str2.Substring(2, str2.Length - 2);
                    str2 = "where (" + str2 + ")";
                }
            }

            string sql = string.Format(@"
select a.物料编码,a.受订量,a.物料类型, 
a.物料名称,a.n原ERP规格型号,仓库物料数量表.库存总数,仓库物料数量表.在制量,仓库物料数量表.在途量,仓库物料数量表.未领量,基础数据物料信息表.原ERP物料编号,
仓库物料数量表.未领量,基础数据物料信息表.大类,基础数据物料信息表.规格型号,基础数据物料信息表.图纸编号,isnull(基础数据物料信息表.库存下限,0) 库存下限
,基础数据物料信息表.特殊备注,基础数据物料信息表.车间编号 from 
(select [销售记录销售订单明细表].物料编码,基础数据物料信息表.物料名称,基础数据物料信息表.n原ERP规格型号,
SUM(未完成数量) as 受订量,物料类型 from [销售记录销售订单明细表] 
left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = [销售记录销售订单明细表].物料编码
where [明细完成] = 0 and [销售记录销售订单明细表].生效 = 1 and 作废 = 0 
and (总完成 = 0 or 总完成 is null) and [销售记录销售订单明细表].关闭 = 0 and 未完成数量 > 0
and 生效日期 >= '{0}'
group by [销售记录销售订单明细表].物料编码,物料类型,基础数据物料信息表.物料名称,基础数据物料信息表.n原ERP规格型号
) a 
left join 仓库物料数量表 on 仓库物料数量表.物料编码 = a.物料编码 
left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = a.物料编码
", time);
            if (str != "")
            {
                sql = sql + str;
            }
            string sql2 = string.Format(@"
select a.物料编码,a.受订量,a.物料类型, 
a.物料名称,a.n原ERP规格型号,仓库物料数量表.库存总数,仓库物料数量表.在制量,仓库物料数量表.在途量,仓库物料数量表.未领量,基础数据物料信息表.原ERP物料编号,
仓库物料数量表.未领量,基础数据物料信息表.大类,基础数据物料信息表.规格型号,基础数据物料信息表.图纸编号,isnull(基础数据物料信息表.库存下限,0) 库存下限
,基础数据物料信息表.特殊备注,基础数据物料信息表.车间编号 from 
(select [销售记录销售订单明细表].物料编码,基础数据物料信息表.物料名称,基础数据物料信息表.n原ERP规格型号,
SUM(未完成数量) as 受订量,物料类型 from [销售记录销售订单明细表] 
left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = [销售记录销售订单明细表].物料编码
where [明细完成] = 0 and [销售记录销售订单明细表].生效 = 1 and 作废 = 0 
and (总完成 = 0 or 总完成 is null) and [销售记录销售订单明细表].关闭 = 0 and 未完成数量 > 0
and 生效日期 >= '{0}'
group by [销售记录销售订单明细表].物料编码,物料类型,基础数据物料信息表.物料名称,基础数据物料信息表.n原ERP规格型号
) a 
left join 仓库物料数量表 on 仓库物料数量表.物料编码 = a.物料编码 
left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = a.物料编码
", time);
            if (str2 != "")
            {
                sql2 = sql2 + str2;
            }
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt);//小类

            DataTable dtt = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
            da2.Fill(dtt);//大类

            foreach (DataRow rr in dtt.Rows)
            {
                if (dt.Select(string.Format("物料编码 = '{0}'", rr["物料编码"])).Length > 0)
                {
                    continue;
                }
                else
                {
                    DataRow dr = dt.NewRow();
                    dt.Rows.Add(dr);
                    dr.ItemArray = rr.ItemArray;
                }
            }

            sql = @"select 物料编码,isnull(SUM(未排单数量),0) 未生效制令数量 
  from 生产记录生产制令表 where 生效 = 0 and 未排单数量 > 0 
  and 完成 = 0 and 关闭 = 0  and 日期 >= '2017/01/01'
  group by 物料编码";
            da = new SqlDataAdapter(sql, strconn);
            DataTable dt1 = new DataTable();
            da.Fill(dt1);
            sql = @"select 物料编码,isnull(SUM(未排单数量),0) 已生效制令数量 
  from 生产记录生产制令表 where 生效 = 1 and 未排单数量 > 0 
  and 完成 = 0 and 关闭 = 0 and 日期 >= '2017/01/01'
  group by 物料编码";
            da = new SqlDataAdapter(sql, strconn);
            DataTable dt2 = new DataTable();
            da.Fill(dt2);
            dt.Columns.Add("未生效制令数量", typeof(Decimal));
            dt.Columns.Add("计算量", typeof(Decimal));
            dt.Columns.Add("已生效制令数量", typeof(Decimal));
            dt.Columns.Add("计算量包含安全库存", typeof(Decimal));
            foreach (DataRow dr in dt.Rows)
            {
                dr["计算量"] = Convert.ToDecimal(dr["受订量"]) - Convert.ToDecimal(dr["库存总数"]) - Convert.ToDecimal(dr["在制量"]) - Convert.ToDecimal(dr["在途量"]) + Convert.ToDecimal(dr["未领量"]);// - 仓库物料数量表.库存总数 - 仓库物料数量表.在制量 - 仓库物料数量表.在途量 + 仓库物料数量表.未领量) as 计算量
                DataRow[] ds = dt1.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                if (ds.Length > 0)
                {
                    dr["未生效制令数量"] = ds[0]["未生效制令数量"];
                }
                else
                {
                    dr["未生效制令数量"] = 0;
                }
                ds = dt2.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                if (ds.Length > 0)
                {
                    dr["已生效制令数量"] = ds[0]["已生效制令数量"];
                }
                else
                {
                    dr["已生效制令数量"] = 0;
                }
                try
                {
                    dr["计算量包含安全库存"] = Convert.ToDecimal(dr["计算量"]) + Convert.ToDecimal(dr["库存下限"]);
                }
                catch { dr["计算量包含安全库存"] = Convert.ToDecimal(dr["计算量"]); }
            }

            string sql11 = @"select 基础数据物料信息表.物料编码,基础数据物料信息表.物料类型,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.物料名称,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.大类,
            isnull(基础数据物料信息表.库存下限,0) 库存下限,基础数据物料信息表.图纸编号,基础数据物料信息表.特殊备注,基础数据物料信息表.车间编号,基础数据物料信息表.规格型号,
            仓库物料数量表.库存总数,仓库物料数量表.在途量,仓库物料数量表.在制量,仓库物料数量表.未领量 
            from 基础数据物料信息表 
            left join 仓库物料数量表 on 基础数据物料信息表.物料编码 = 仓库物料数量表.物料编码 
            where 基础数据物料信息表.库存下限 >= 仓库物料数量表.库存总数 and  基础数据物料信息表.物料类型 != '原材料' and 基础数据物料信息表.停用 = 0";
            if (str != "")
            {
                str = str.Substring(5, str.Length - 5);
                sql11 = sql11 + " and " + str;
            }
            DataTable dt_安全库存_小类 = dt.Clone();
            da = new SqlDataAdapter(sql11, strconn);
            da.Fill(dt_安全库存_小类);

            string sql22 = @"select 基础数据物料信息表.物料编码,基础数据物料信息表.物料类型,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.物料名称,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.大类,
            isnull(基础数据物料信息表.库存下限,0) 库存下限,基础数据物料信息表.图纸编号,基础数据物料信息表.特殊备注,基础数据物料信息表.车间编号,基础数据物料信息表.规格型号,
            仓库物料数量表.库存总数,仓库物料数量表.在途量,仓库物料数量表.在制量,仓库物料数量表.未领量 
            from 基础数据物料信息表 
            left join 仓库物料数量表 on 基础数据物料信息表.物料编码 = 仓库物料数量表.物料编码 
            where 基础数据物料信息表.库存下限 >= 仓库物料数量表.库存总数 and  基础数据物料信息表.物料类型 != '原材料' and 基础数据物料信息表.停用 = 0";
            if (str2 != "")
            {
                str2 = str2.Substring(5, str2.Length - 5);
                sql22 = sql22 + " and " + str2;
            }
            DataTable dt_安全库存_大类 = dt.Clone();
            da = new SqlDataAdapter(sql22, strconn);
            da.Fill(dt_安全库存_大类);

            foreach (DataRow rr in dt_安全库存_大类.Rows)
            {
                if (dt_安全库存_小类.Select(string.Format("物料编码 = '{0}'", rr["物料编码"])).Length > 0)
                {
                    continue;
                }
                else
                {
                    DataRow dr = dt_安全库存_小类.NewRow();
                    dt_安全库存_小类.Rows.Add(dr);
                    dr.ItemArray = rr.ItemArray;
                }
            }

            //dt_安全库存_小类.Columns.Add("未生效制令数量", typeof(Decimal));
            //dt_安全库存_小类.Columns.Add("计算量", typeof(Decimal));
            //dt_安全库存_小类.Columns.Add("已生效制令数量", typeof(Decimal));
            //dt_安全库存_小类.Columns.Add("计算量包含安全库存", typeof(Decimal));

            int count = dt_安全库存_小类.Rows.Count;

            for (int i = 0; i < count; i++)
            {
                DataRow dr = dt_安全库存_小类.Rows[i];
                DataRow[] ds = dt.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                if (ds.Length > 0)
                {
                    dt_安全库存_小类.Rows.Remove(dr);
                    count--;
                    i--;
                }
                else
                {

                    dr["计算量"] = 0;
                    ds = dt1.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                    if (ds.Length > 0)
                    {
                        dr["未生效制令数量"] = ds[0]["未生效制令数量"];
                    }
                    else
                    {
                        dr["未生效制令数量"] = 0;
                    }
                    ds = dt2.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                    if (ds.Length > 0)
                    {
                        dr["已生效制令数量"] = ds[0]["已生效制令数量"];
                    }
                    else
                    {
                        dr["已生效制令数量"] = 0;
                    }

                    dr["计算量包含安全库存"] = Convert.ToDecimal(dr["库存下限"]) - Convert.ToDecimal(dr["库存总数"]) - Convert.ToDecimal(dr["在制量"]);
                }


            }
            //foreach (DataRow dr in dt_安全库存_小类.Rows)//去除重复项
            //{
            //    //if (dr.RowState == DataRowState.Deleted) continue;
            //    DataRow[] ds = dt.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
            //    if (ds.Length > 0)
            //    {
            //        //dr.Delete();
            //        continue;
            //    }
            //    else
            //    {
            //        DataRow r = dt.NewRow(); 
            //        dt.Rows.Add(r);
            //        r.ItemArray = dr.ItemArray;
            //        r["计算量"] = 0;
            //        ds = dt1.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
            //        if (ds.Length > 0)
            //        { 
            //            r["未生效制令数量"] = ds[0]["未生效制令数量"];
            //        }
            //        else
            //        {
            //            r["未生效制令数量"] = 0;
            //        }
            //        ds = dt2.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
            //        if (ds.Length > 0)
            //        {
            //            r["已生效制令数量"] = ds[0]["已生效制令数量"];
            //        }
            //        else
            //        {
            //            r["已生效制令数量"] = 0;
            //        }

            //        r["计算量包含安全库存"] = Convert.ToDecimal(r["库存下限"]) - Convert.ToDecimal(r["库存总数"]) - Convert.ToDecimal(r["在制量"]);
            //    }
            //}
            dt_安全库存_小类.AcceptChanges();
            dt.Merge(dt_安全库存_小类);
            return dt;
        }

#pragma warning disable IDE1006 // 命名样式
        public static DataTable fun_计划_需生产2(DateTime time)
#pragma warning restore IDE1006 // 命名样式
        {
            string str_计划对应物料 = "";

            if (CPublic.Var.LocalUserID != "admin" && CPublic.Var.LocalUserTeam != "公司高管权限")
            {
                //string sqll = "select * from 计划人员关联物料表 where 工号 = '" + CPublic.Var.LocalUserID + "'";
                //DataTable dt_计划人员关联物料 = new DataTable();
                //SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn);
                //daa.Fill(dt_计划人员关联物料);
                //if (dt_计划人员关联物料.Rows.Count > 0)
                //{
                //    foreach (DataRow dr in dt_计划人员关联物料.Rows)
                //    {
                //        str_计划对应物料 = str_计划对应物料 + "or 基础数据物料信息表.物料编码 = '" + dr["物料编码"] + "'";
                //    }
                //    str_计划对应物料 = str_计划对应物料.Substring(2, str_计划对应物料.Length - 2);
                //    str_计划对应物料 = "where (" + str_计划对应物料 + ")";
                //}

                //2
                //  str_计划对应物料 = "where 基础数据物料信息表.物料编码 in ( select 物料编码 from 计划人员关联物料表 where 工号 = '" + CPublic.Var.LocalUserID + "')";
                //3
                str_计划对应物料 = " left  join 计划人员关联物料表 on base.物料编码 = 计划人员关联物料表.物料编码 where (计划人员关联物料表.工号='" + CPublic.Var.LocalUserID + "') ";

                /* or 计划人员关联物料表.工号 is null */

            }

            string sql = string.Format(@"select a.物料编码,a.受订量,a.物料类型, 
a.物料名称,a.规格型号,kc.库存总数,kc.在制量,kc.在途量,kc.未领量,base.原ERP物料编号,
kc.未领量,base.大类,base.规格型号,base.图纸编号,isnull(base.库存下限,0) 库存下限
,base.特殊备注,base.车间编号,新数据 from 
(select [销售记录销售订单明细表].物料编码,ba.物料名称,ba.规格型号,
SUM(未完成数量) as 受订量,物料类型 from [销售记录销售订单明细表] 
left join 基础数据物料信息表 ba on ba.物料编码 = [销售记录销售订单明细表].物料编码
where [明细完成] = 0 and [销售记录销售订单明细表].生效 = 1 and 作废 = 0 
and (总完成 = 0 or 总完成 is null) and [销售记录销售订单明细表].关闭 = 0 and 未完成数量 > 0
and 生效日期 >= '{0}'
group by [销售记录销售订单明细表].物料编码,物料类型,ba.物料名称,ba.规格型号
) a 
left join 仓库物料数量表  kc on kc.物料编码 = a.物料编码 
left join 基础数据物料信息表 base on base.物料编码 = a.物料编码", time);
            if (str_计划对应物料 != "")
            {
                sql = sql + str_计划对应物料;
            }

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt);

            sql = @"select 物料编码,isnull(SUM(未排单数量),0) 未生效制令数量 
  from 生产记录生产制令表 where 生效 = 0 and 未排单数量 > 0 
  and 完成 = 0 and 关闭 = 0  and 日期 >= '2017/01/01'
  group by 物料编码";
            da = new SqlDataAdapter(sql, strconn);
            DataTable dt1 = new DataTable();
            da.Fill(dt1);

            sql = @"select 物料编码,isnull(SUM(未排单数量),0) 已生效制令数量 
  from 生产记录生产制令表 where 生效 = 1 and 未排单数量 > 0 
  and 完成 = 0 and 关闭 = 0 and 日期 >= '2017/01/01'
  group by 物料编码";
            da = new SqlDataAdapter(sql, strconn);
            DataTable dt2 = new DataTable();
            da.Fill(dt2);

            dt.Columns.Add("未生效制令数量", typeof(Decimal));
            dt.Columns.Add("计算量", typeof(Decimal));
            dt.Columns.Add("已生效制令数量", typeof(Decimal));
            dt.Columns.Add("计算量包含安全库存", typeof(Decimal));
            foreach (DataRow dr in dt.Rows)
            {
                dr["计算量"] = Convert.ToDecimal(dr["受订量"]) - Convert.ToDecimal(dr["库存总数"]) - Convert.ToDecimal(dr["在制量"]) - Convert.ToDecimal(dr["在途量"]) + Convert.ToDecimal(dr["未领量"]);// - 仓库物料数量表.库存总数 - 仓库物料数量表.在制量 - 仓库物料数量表.在途量 + 仓库物料数量表.未领量) as 计算量
                DataRow[] ds = dt1.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                if (ds.Length > 0)
                {
                    dr["未生效制令数量"] = ds[0]["未生效制令数量"];
                }
                else
                {
                    dr["未生效制令数量"] = 0;
                }

                ds = dt2.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                if (ds.Length > 0)
                {
                    dr["已生效制令数量"] = ds[0]["已生效制令数量"];
                }
                else
                {
                    dr["已生效制令数量"] = 0;
                }

                try
                {
                    dr["计算量包含安全库存"] = Convert.ToDecimal(dr["计算量"]) + Convert.ToDecimal(dr["库存下限"]);
                }
                catch { dr["计算量包含安全库存"] = Convert.ToDecimal(dr["计算量"]); }
            }

            string sql11 = @"select base.物料编码,base.物料类型,base.原ERP物料编号,base.物料名称,base.规格型号,base.大类,
            isnull(base.库存下限,0) 库存下限,base.图纸编号,base.特殊备注,base.车间编号,base.规格型号,
            kc.库存总数,kc.在途量,kc.在制量,kc.未领量,新数据
            from 基础数据物料信息表  base
            left join 仓库物料数量表 kc on base.物料编码 = kc.物料编码 {0} 
            where base.库存下限 >= kc.库存总数 and  base.物料类型 <>'原材料' and base.停用 = 0 {1}";
            if (str_计划对应物料 != "")
            {
                string[] x = str_计划对应物料.Split('w');
                x[1] = "and " + x[1].Substring(4, x[1].Length - 4);
                sql11 = string.Format(sql11, x[0], x[1]);
                //str_计划对应物料 = str_计划对应物料.Substring(5, str_计划对应物料.Length - 5);
                //sql11 = sql11 + " and " + str_计划对应物料;
            }
            else
            {
                sql11 = string.Format(sql11, "", "");

            }
            DataTable dt_安全库存 = dt.Clone();
            da = new SqlDataAdapter(sql11, strconn);
            da.Fill(dt_安全库存);

            int count = dt_安全库存.Rows.Count;

            for (int i = 0; i < count; i++)
            {
                DataRow dr = dt_安全库存.Rows[i];
                DataRow[] ds = dt.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                if (ds.Length > 0)
                {
                    dt_安全库存.Rows.Remove(dr);
                    count--;
                    i--;
                }
                else
                {
                    dr["计算量"] = 0;
                    ds = dt1.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                    if (ds.Length > 0)
                    {
                        dr["未生效制令数量"] = ds[0]["未生效制令数量"];
                    }
                    else
                    {
                        dr["未生效制令数量"] = 0;
                    }

                    ds = dt2.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                    if (ds.Length > 0)
                    {
                        dr["已生效制令数量"] = ds[0]["已生效制令数量"];
                    }
                    else
                    {
                        dr["已生效制令数量"] = 0;
                    }

                    dr["计算量包含安全库存"] = Convert.ToDecimal(dr["库存下限"]) - Convert.ToDecimal(dr["库存总数"]) - Convert.ToDecimal(dr["在制量"]);
                }
            }

            dt_安全库存.AcceptChanges();
            dt.Merge(dt_安全库存);
            return dt;
        }

        /*
        public static DataTable fun_采购_需采购()
        {
            string sql = @"select d.子项编码 as 物料编码,d.物料名称,d.n原ERP规格型号,d.物料类型,d.库存总数,d.在制量,d.在途量,d.受订量,d.未领量,d.计算量 from
(select c.子项编码,c.物料类型,仓库物料数量表.库存总数,仓库物料数量表.在途量,仓库物料数量表.在制量,仓库物料数量表.未领量,仓库物料数量表.受订量,
c.物料名称,c.n原ERP规格型号,
(c.计算量- 仓库物料数量表.库存总数 - 仓库物料数量表.在制量 - 仓库物料数量表.在途量 + 仓库物料数量表.未领量 + 仓库物料数量表.受订量) as 计算量 from
(select [基础数据物料BOM表].子项编码,基础数据物料信息表.物料类型,基础数据物料信息表.物料名称,基础数据物料信息表.n原ERP规格型号,(sum(数量 * b.受订量) )as 计算量
from [基础数据物料BOM表] 
left join 
(select a.物料编码,(a.受订量 - 仓库物料数量表.库存总数 - 仓库物料数量表.在制量 - 仓库物料数量表.在途量 + 仓库物料数量表.未领量) as 受订量,a.物料类型 from 
(select [销售记录销售订单明细表].物料编码,SUM(未完成数量) as 受订量,物料类型 from [销售记录销售订单明细表] 
left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = [销售记录销售订单明细表].物料编码
where [明细完成] = 0 and [销售记录销售订单明细表].生效 = 1 and 作废 = 0 
and (总完成 = 0 or 总完成 is null) and [销售记录销售订单明细表].关闭 = 0 and 未完成数量 > 0
and 生效日期 > '2016-12-01 00:00:00'
group by [销售记录销售订单明细表].物料编码,物料类型
) a 
left join 仓库物料数量表 on 仓库物料数量表.物料编码 = a.物料编码 
where (a.受订量 - 仓库物料数量表.库存总数 - 仓库物料数量表.在制量 - 仓库物料数量表.在途量 + 仓库物料数量表.未领量) > 0
) b
on b.物料编码 = [基础数据物料BOM表].产品编码
left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = [基础数据物料BOM表].[子项编码]
where [基础数据物料BOM表].产品编码 = b.物料编码
group by [基础数据物料BOM表].子项编码,基础数据物料信息表.物料类型,基础数据物料信息表.物料名称,基础数据物料信息表.n原ERP规格型号) c
left join 仓库物料数量表 on 仓库物料数量表.物料编码 = c.子项编码 
where c.计算量- 仓库物料数量表.库存总数 - 仓库物料数量表.在制量 - 仓库物料数量表.在途量 + 仓库物料数量表.未领量 + 仓库物料数量表.受订量> 0)d";
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt);
            return dt;
        }

        public static DataTable fun_采购_需采购_加上制令部分()
        {
            string sql = 
@"select d.子项编码 as 物料编码,d.物料名称,基础数据物料信息表.大类,d.n原ERP规格型号,d.物料类型,d.库存总数,d.在制量,d.在途量,d.受订量,d.未领量,d.计算量 
from
--4
(select c.子项编码,c.物料类型,仓库物料数量表.库存总数,仓库物料数量表.在途量,仓库物料数量表.在制量,仓库物料数量表.未领量,仓库物料数量表.受订量,
    c.物料名称,c.n原ERP规格型号,
    (c.计算量- 仓库物料数量表.库存总数 - 仓库物料数量表.在制量 - 仓库物料数量表.在途量 + 仓库物料数量表.未领量 + 仓库物料数量表.受订量) as 计算量 
from
--3
(select [基础数据物料BOM表].子项编码,基础数据物料信息表.物料类型,基础数据物料信息表.物料名称,基础数据物料信息表.n原ERP规格型号,(sum(数量 * b.受订量) )as 计算量
from [基础数据物料BOM表] 
left join 
--2
(select a.物料编码,(a.受订量 - 仓库物料数量表.库存总数 - 仓库物料数量表.在制量 - 仓库物料数量表.在途量 + 仓库物料数量表.未领量) as 受订量,a.物料类型 
from 
--1
(
(select aa.物料编码,case when(aa.受订量 > isnull(s.制令量,0)) then aa.受订量 else isnull(s.制令量,0) end as 受订量,aa.物料类型 
from 
    (select [销售记录销售订单明细表].物料编码,SUM(未完成数量) as 受订量,物料类型 from [销售记录销售订单明细表] 
    left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = [销售记录销售订单明细表].物料编码
    where [明细完成] = 0 and [销售记录销售订单明细表].生效 = 1 and 作废 = 0 
        and (总完成 = 0 or 总完成 is null) and [销售记录销售订单明细表].关闭 = 0 and 未完成数量 > 0
        and 生效日期 > '2016-12-01 00:00:00'
    group by [销售记录销售订单明细表].物料编码,物料类型) aa
left join 
    (select 生产记录生产制令表.物料编码,SUM(未排单数量) as 制令量,基础数据物料信息表.物料类型 
    from 生产记录生产制令表 
    left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产制令表.物料编码
    where 未排单数量 > 0 and 生产记录生产制令表.生效 = 1 and 完成 = 0 and 生产记录生产制令表.关闭 = 0
        and 生效日期 > '2016-12-01 00:00:00'
    group by 生产记录生产制令表.物料编码,基础数据物料信息表.物料类型) s
on s.物料编码 = aa.物料编码) 

union 

(select 生产记录生产制令表.物料编码,SUM(未排单数量) as 受订量,基础数据物料信息表.物料类型 
from 生产记录生产制令表 
    left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产制令表.物料编码
    where 未排单数量 > 0 and 生产记录生产制令表.生效 = 1 and 完成 = 0 and 生产记录生产制令表.关闭 = 0
        and 生效日期 > '2016-12-01 00:00:00' and 生产记录生产制令表.物料编码 not in 
        (select aa.物料编码 from 
            (select [销售记录销售订单明细表].物料编码 
            from [销售记录销售订单明细表] 
            where [明细完成] = 0 and [销售记录销售订单明细表].生效 = 1 and 作废 = 0 
            and (总完成 = 0 or 总完成 is null) and [销售记录销售订单明细表].关闭 = 0 and 未完成数量 > 0
            and 生效日期 > '2016-12-01 00:00:00'
            group by [销售记录销售订单明细表].物料编码) aa
        left join 
            (select 生产记录生产制令表.物料编码 
            from 生产记录生产制令表 
            where 未排单数量 > 0 and 生产记录生产制令表.生效 = 1 and 完成 = 0 and 生产记录生产制令表.关闭 = 0
            and 生效日期 > '2016-12-01 00:00:00'
            group by 生产记录生产制令表.物料编码) s
        on s.物料编码 = aa.物料编码
        where s.物料编码 is not null)
    group by 生产记录生产制令表.物料编码,基础数据物料信息表.物料类型)
) a
--/1按物料编码统计出销售明细未完成的部分
left join 仓库物料数量表 on 仓库物料数量表.物料编码 = a.物料编码 
where (a.受订量 - 仓库物料数量表.库存总数 - 仓库物料数量表.在制量 - 仓库物料数量表.在途量 + 仓库物料数量表.未领量) > 0
) b
--/2和仓库运算，得到缺的成品部分
on b.物料编码 = [基础数据物料BOM表].产品编码
left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = [基础数据物料BOM表].[子项编码]
where [基础数据物料BOM表].产品编码 = b.物料编码
group by [基础数据物料BOM表].子项编码,基础数据物料信息表.物料类型,基础数据物料信息表.物料名称,基础数据物料信息表.n原ERP规格型号) c
--/3缺的成品量*BOM结构，得到缺的原材料
left join 仓库物料数量表 on 仓库物料数量表.物料编码 = c.子项编码 
where c.计算量- 仓库物料数量表.库存总数 - 仓库物料数量表.在制量 - 仓库物料数量表.在途量 + 仓库物料数量表.未领量 + 仓库物料数量表.受订量> 0)d
--/4缺的原材料和仓库运算，得到缺的原材料部分
left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = d.子项编码";
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt);
            return dt;
        }
         */

        /*20170213*/
#pragma warning disable IDE1006 // 命名样式
        public static DataTable fun_销售受订量(DevExpress.XtraGrid.Views.Grid.GridView gv, DateTime time, DataTable dt_ls, string str_person)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                dtime = CPublic.Var.getDatetime();
                dtime = dtime.AddDays(-dtime.Day + 1);
                dtime1 = dtime.AddMonths(-3);
                dtime2 = dtime.AddMonths(-1);
                /*扣掉库存的受订量
                 有成品，半成品，原材料*/
                string sql = string.Format(@"select a.物料编码,a.受订量a,a.物料类型
,基础数据物料信息表.物料名称,基础数据物料信息表.大类,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.原ERP物料编号,
基础数据物料信息表.规格型号,基础数据物料信息表.图纸编号,基础数据物料信息表.计量单位,基础数据物料信息表.仓库名称,
基础数据物料信息表.供应商编号,基础数据物料信息表.默认供应商,采购供应商备注,基础数据物料信息表.仓库号 from 
(
 (select aa.物料编码,case when((aa.受订量 - 仓库物料数量表.库存总数 - 仓库物料数量表.在制量 - 仓库物料数量表.在途量 + 仓库物料数量表.未领量) > isnull(s.制令量,0)) then (aa.受订量 - 仓库物料数量表.库存总数 - 仓库物料数量表.在制量 - 仓库物料数量表.在途量 + 仓库物料数量表.未领量) else isnull(s.制令量,0) end as 受订量a,aa.物料类型 
from 
	(select [销售记录销售订单明细表].物料编码,SUM(未完成数量) as 受订量,物料类型 from [销售记录销售订单明细表] 
	left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = [销售记录销售订单明细表].物料编码
	where [明细完成] = 0 and [销售记录销售订单明细表].生效 = 1 and 作废 = 0 
		and (总完成 = 0 or 总完成 is null) and [销售记录销售订单明细表].关闭 = 0 and 未完成数量 > 0
		and 生效日期 >= '{0}'and 基础数据物料信息表.物料类型 = '成品'
	group by [销售记录销售订单明细表].物料编码,物料类型) aa
left join 
	(select 生产记录生产制令表.物料编码,SUM(未排单数量) as 制令量,基础数据物料信息表.物料类型 
	from 生产记录生产制令表 
	left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产制令表.物料编码
	where 未排单数量 > 0 /*and 生产记录生产制令表.生效 = 1*/ and 完成 = 0 and 生产记录生产制令表.关闭 = 0
		and 生产记录生产制令表.日期 >= '{0}'and 基础数据物料信息表.物料类型 = '成品'
	group by 生产记录生产制令表.物料编码,基础数据物料信息表.物料类型) s
on s.物料编码 = aa.物料编码
    left join 仓库物料数量表 on 仓库物料数量表.物料编码 = aa.物料编码 
 ) 
union 
 (select 生产记录生产制令表.物料编码,SUM(未排单数量) as 受订量a,基础数据物料信息表.物料类型 
from 生产记录生产制令表 
	left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产制令表.物料编码
	where 未排单数量 > 0 /*and 生产记录生产制令表.生效 = 1 */ and 完成 = 0 and 生产记录生产制令表.关闭 = 0 and 基础数据物料信息表.物料类型 = '成品'
		and 生产记录生产制令表.日期 >= '{0}' and 生产记录生产制令表.物料编码 not in 
		(select aa.物料编码 from 
			(select [销售记录销售订单明细表].物料编码 
			from [销售记录销售订单明细表] 
			where [明细完成] = 0 and [销售记录销售订单明细表].生效 = 1 and 作废 = 0 
			and (总完成 = 0 or 总完成 is null) and [销售记录销售订单明细表].关闭 = 0 and 未完成数量 > 0
			and 生效日期 >= '{0}' and 基础数据物料信息表.物料类型 = '成品'
			group by [销售记录销售订单明细表].物料编码) aa
		left join 
			(select 生产记录生产制令表.物料编码 
			from 生产记录生产制令表 
			where 未排单数量 > 0 /*and 生产记录生产制令表.生效 = 1*/ and 完成 = 0 and 生产记录生产制令表.关闭 = 0
			and 生产记录生产制令表.日期 >= '{0}' and 基础数据物料信息表.物料类型 = '成品'
			group by 生产记录生产制令表.物料编码) s
		on s.物料编码 = aa.物料编码
		where s.物料编码 is not null and 基础数据物料信息表.物料类型 = '成品')
	group by 生产记录生产制令表.物料编码,基础数据物料信息表.物料类型
 )
) a  
left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = a.物料编码
where a.受订量a > 0", time);//and 生产记录生产制令表.生效 = 1 
                DataTable dt1 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt1);
                gv.ViewCaption = string.Format("准备处理成品的BOM结构");
                Application.DoEvents();
                DataTable dtreturn = new DataTable();
                foreach (DataRow dr in dt1.Rows)
                {
                    DataTable t = fun_BOM(dr["物料编码"].ToString(), dt_ls, str_person);
                    foreach (DataRow r in t.Rows)
                    {
                        r["受订量a"] = Convert.ToDecimal(r["受订量a"]) * Convert.ToDecimal(dr["受订量a"]);
                    }
                    dtreturn.Merge(t);
                }
                gv.ViewCaption = string.Format("成品的BOM结构处理完毕");
                Application.DoEvents();
                sql = string.Format(@"select a.物料编码,a.受订量a,a.物料类型
,基础数据物料信息表.规格型号,基础数据物料信息表.图纸编号,基础数据物料信息表.计量单位,基础数据物料信息表.仓库名称,基础数据物料信息表.原ERP物料编号,
基础数据物料信息表.供应商编号,基础数据物料信息表.默认供应商,采购供应商备注,基础数据物料信息表.仓库号 from 

((select aa.物料编码,case when((aa.受订量-仓库物料数量表.库存总数 - 仓库物料数量表.在制量 - 仓库物料数量表.在途量 + 仓库物料数量表.未领量) > isnull(s.制令量,0)) then (aa.受订量 - 仓库物料数量表.库存总数 - 仓库物料数量表.在制量 - 仓库物料数量表.在途量 + 仓库物料数量表.未领量) else isnull(s.制令量,0) end as 受订量a,aa.物料类型 
from 
	(select [销售记录销售订单明细表].物料编码,SUM(未完成数量) as 受订量,物料类型 from [销售记录销售订单明细表] 
	left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = [销售记录销售订单明细表].物料编码
	where [明细完成] = 0 and [销售记录销售订单明细表].生效 = 1 and 作废 = 0 
		and (总完成 = 0 or 总完成 is null) and [销售记录销售订单明细表].关闭 = 0 and 未完成数量 > 0
		and 生效日期 >= '{0}'and 基础数据物料信息表.物料类型 ='半成品'
	group by [销售记录销售订单明细表].物料编码,物料类型) aa
left join 
	(select 生产记录生产制令表.物料编码,SUM(未排单数量) as 制令量,基础数据物料信息表.物料类型 
	from 生产记录生产制令表 
	left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产制令表.物料编码  
	where 未排单数量 > 0 /*and 生产记录生产制令表.生效 = 1*/ and 完成 = 0 and 生产记录生产制令表.关闭 = 0
		and 生产记录生产制令表.日期 >= '{0}'and 基础数据物料信息表.物料类型 = '半成品'
	group by 生产记录生产制令表.物料编码,基础数据物料信息表.物料类型) s
on s.物料编码 = aa.物料编码
left join 仓库物料数量表 on 仓库物料数量表.物料编码 = aa.物料编码 
)  
union 
(select 生产记录生产制令表.物料编码,SUM(未排单数量) as 受订量a,基础数据物料信息表.物料类型 
from 生产记录生产制令表 
	left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产制令表.物料编码
	where 未排单数量 > 0 /*and 生产记录生产制令表.生效 = 1*/ and 完成 = 0 and 生产记录生产制令表.关闭 = 0 and 基础数据物料信息表.物料类型 = '半成品'
		and 生产记录生产制令表.日期 >= '{0}' and 生产记录生产制令表.物料编码 not in 
		(select aa.物料编码 from 
			(select [销售记录销售订单明细表].物料编码 
			from [销售记录销售订单明细表] 
			where [明细完成] = 0 and [销售记录销售订单明细表].生效 = 1 and 作废 = 0 
			and (总完成 = 0 or 总完成 is null) and [销售记录销售订单明细表].关闭 = 0 and 未完成数量 > 0
			and 生效日期 >= '{0}' and 基础数据物料信息表.物料类型 = '半成品'
			group by [销售记录销售订单明细表].物料编码) aa
		     left join 
			(select 生产记录生产制令表.物料编码 
			from 生产记录生产制令表 
			where 未排单数量 > 0 /*and 生产记录生产制令表.生效 = 1*/ and 完成 = 0 and 生产记录生产制令表.关闭 = 0
			and 生产记录生产制令表.日期 >= '{0}' and 基础数据物料信息表.物料类型 = '半成品'
			group by 生产记录生产制令表.物料编码) s
		     on s.物料编码 = aa.物料编码
		where s.物料编码 is not null and 基础数据物料信息表.物料类型 = '半成品')
	group by 生产记录生产制令表.物料编码,基础数据物料信息表.物料类型)) a 

left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = a.物料编码
where a.受订量a > 0", time);
                DataTable dt2 = new DataTable();
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt2);
                gv.ViewCaption = string.Format("准备处理半成品的BOM结构");
                Application.DoEvents();
                foreach (DataRow dr in dt2.Rows)
                {
                    DataTable t = fun_BOM(dr["物料编码"].ToString(), dt_ls, str_person);
                    foreach (DataRow r in t.Rows)
                    {
                        r["受订量a"] = Convert.ToDecimal(r["受订量a"]) * Convert.ToDecimal(dr["受订量a"]);
                    }
                    dtreturn.Merge(t);
                }
                gv.ViewCaption = string.Format("半成品的BOM结构处理完毕");
                Application.DoEvents();

                /*left join  (select 物料编码,-sum(实效数量)as 季度用量  from 仓库出入库明细表 where  出库入库='出库' and  出入库时间>'{2}' and 
                             出入库时间<'{1}'  group by 物料编码)x on  销售记录销售订单明细表.物料编码=x.物料编码
left join  (select 物料编码,-sum(实效数量)as 上月用量  from 仓库出入库明细表 where  出库入库='出库' and  出入库时间>'{3}' and 
                             出入库时间<'{1}'  group by 物料编码)y on  销售记录销售订单明细表.物料编码=y.物料编码*/
                sql = string.Format(@"select [销售记录销售订单明细表].物料编码,SUM(未完成数量) as 受订量a,库存总数,有效总数,在途量,在制量,受订量,未领量,
基础数据物料信息表.物料名称,基础数据物料信息表.大类,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.库存下限,基础数据物料信息表.原ERP物料编号,
基础数据物料信息表.规格型号,基础数据物料信息表.图纸编号,基础数据物料信息表.计量单位,基础数据物料信息表.仓库名称,基础数据物料信息表.物料类型,
基础数据物料信息表.供应商编号,基础数据物料信息表.默认供应商,采购供应商备注,基础数据物料信息表.仓库号,基础数据物料信息表.停用,有无蓝图
from [销售记录销售订单明细表] 
left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = [销售记录销售订单明细表].物料编码
left join 仓库物料数量表 on 仓库物料数量表.物料编码 = 销售记录销售订单明细表.物料编码

where [明细完成] = 0 and [销售记录销售订单明细表].生效 = 1 and 作废 = 0 
and (总完成 = 0 or 总完成 is null) and [销售记录销售订单明细表].关闭 = 0 and 未完成数量 > 0
and 生效日期 > '{0}' and 物料类型 = '原材料' ", time);
                //aa 添了 基础数据物料信息表.物料类型,
                if (str_person != "admin")
                {
                    if (dt_ls.Rows.Count > 0)
                    {
                        sql = sql + "and ( 基础数据物料信息表.供应商编号= '' or";
                        foreach (DataRow dr in dt_ls.Rows)
                        {
                            sql = sql + string.Format(" 基础数据物料信息表.供应商编号='{0}' or", dr["供应商ID"]);
                        }
                        sql = sql.Substring(0, sql.Length - 2);
                        sql = sql + ")";
                    }
                    else
                    {
                        throw new Exception("你没有对应的供应商,请找信息部核实");
                    }
                }
                sql = sql + @" group by [销售记录销售订单明细表].物料编码,物料类型,库存总数,有效总数,在途量,在制量,受订量,未领量,
基础数据物料信息表.物料名称,基础数据物料信息表.大类,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.库存下限,基础数据物料信息表.原ERP物料编号,
基础数据物料信息表.规格型号,基础数据物料信息表.图纸编号,基础数据物料信息表.计量单位,基础数据物料信息表.仓库名称,基础数据物料信息表.停用,有无蓝图,
基础数据物料信息表.供应商编号,基础数据物料信息表.默认供应商,采购供应商备注,基础数据物料信息表.仓库号";

                DataTable dt3 = new DataTable();
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt3);

                dtreturn.Merge(dt3);
                gv.ViewCaption = string.Format("准备合并所有原材料信息");
                Application.DoEvents();

                sql = string.Format(@"select 物料编码,-sum(实效数量)as 季度用量  from 仓库出入库明细表 where  出库入库='出库' and  出入库时间>'{1}' and 
                             出入库时间<'{0}'  group by 物料编码", dtime, dtime1);
                DataTable dt4 = new DataTable();
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt4);

                sql = string.Format(@"select 物料编码,-sum(实效数量)as 上月用量  from 仓库出入库明细表 where  出库入库='出库' and  出入库时间>'{1}' and 
                             出入库时间<'{0}'  group by 物料编码", dtime, dtime2);
                DataTable dt5 = new DataTable();
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt5);


                //明细完成日期 is null 是为了处理  检验时有部分 单子 不合格 也没有 入库评审单 ，不把数量 统计进 在途 和 送检数量
                sql = string.Format(@"select 物料编码,SUM(送检数量) as 送检数量 from (            
select 采购记录采购送检单明细表.物料编码,送检数量 from 采购记录采购送检单明细表 
left join 采购记录采购单明细表 on 采购记录采购单明细表.采购明细号 = 采购记录采购送检单明细表.采购单明细号 
where  采购记录采购单明细表.明细完成日期 is null and 检验完成 = 0 and 采购记录采购送检单明细表.作废 = 0 and 采购记录采购单明细表.作废 = 0 and 采购记录采购送检单明细表.生效日期 >= '{0}' 
union
select 产品编号 as 物料编码,采购记录采购单检验主表.送检数量 from 采购记录采购单检验主表
left join 采购记录采购单明细表 on 采购记录采购单明细表.采购明细号 = 采购记录采购单检验主表.采购明细号 and 采购记录采购单明细表.作废 = 0 
left join 采购记录采购送检单主表  on 采购记录采购送检单主表.送检单号=采购记录采购单检验主表.送检单号
where 采购记录采购单明细表.明细完成日期 is null and 关闭 = 0 and 入库完成 = 0 and 检验日期 >= '{0}' and (检验结果='合格' or  采购已处理=0)) a
group by 物料编码", time);
                DataTable dt6 = new DataTable();
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt6);

                MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
                //aa//dtreturn = RBQ.SelectGroupByInto("", dtreturn, "物料编码,原ERP物料编号,物料名称,大类,n原ERP规格型号,库存下限,规格型号,图纸编号,仓库号,仓库名称,供应商编号,默认供应商,计量单位,库存总数,有效总数,在途量,在制量,受订量,未领量,sum(受订量a) 受订量a", "", "物料编码,原ERP物料编号,库存总数,有效总数,在途量,在制量,受订量,未领量,库存下限,规格型号,图纸编号,仓库号,仓库名称,供应商编号,默认供应商,计量单位");//,季度用量,上月用量
                dtreturn = RBQ.SelectGroupByInto("", dtreturn, @"物料编码,原ERP物料编号,物料名称,大类,n原ERP规格型号,库存下限,规格型号,图纸编号,仓库号,仓库名称,供应商编号
                                                    ,默认供应商,采购供应商备注,计量单位,库存总数,有效总数,在途量,在制量,受订量,未领量,sum(受订量a) 受订量a,停用,有无蓝图", "物料类型 = '原材料'",
                                                     "物料编码,原ERP物料编号,库存总数,有效总数,在途量,在制量,受订量,未领量,库存下限,规格型号,图纸编号,仓库号,仓库名称,供应商编号,默认供应商,采购供应商备注,计量单位,停用,有无蓝图");//,季度用量,上月用量
                dtreturn.Columns.Add("季度用量", typeof(Decimal));
                dtreturn.Columns.Add("上月用量", typeof(Decimal));
                dtreturn.Columns.Add("送检数量", typeof(Decimal));
                dtreturn.Columns.Add("欠缺数量不含安全库存", typeof(Decimal));
                dtreturn.Columns.Add("欠缺数量包含安全库存", typeof(Decimal));
                foreach (DataRow dr in dtreturn.Rows)
                {
                    try
                    {
                        DataRow[] ds = dt6.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                        dr["送检数量"] = ds[0]["送检数量"];
                    }
                    catch { dr["送检数量"] = 0; }
                    try
                    {
                        DataRow[] ds = dt4.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                        dr["季度用量"] = ds[0]["季度用量"];
                    }
                    catch { dr["季度用量"] = 0; }
                    try
                    {
                        DataRow[] ds = dt5.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                        dr["上月用量"] = ds[0]["上月用量"];
                    }
                    catch { dr["上月用量"] = 0; }
                    try
                    {
                        dr["欠缺数量不含安全库存"] = Convert.ToDecimal(dr["受订量a"]) + Convert.ToDecimal(dr["未领量"]) - Convert.ToDecimal(dr["库存总数"]) - Convert.ToDecimal(dr["在途量"]) - Convert.ToDecimal(dr["在制量"]);
                    }
                    catch { dr["欠缺数量不含安全库存"] = 0; }//+ Convert.ToDecimal(dr["受订量"])
                    try
                    {
                        dr["欠缺数量包含安全库存"] = Convert.ToDecimal(dr["受订量a"]) + Convert.ToDecimal(dr["未领量"]) - Convert.ToDecimal(dr["库存总数"]) - Convert.ToDecimal(dr["在途量"]) - Convert.ToDecimal(dr["在制量"]) + Convert.ToDecimal(dr["库存下限"]);
                    }
                    catch { dr["欠缺数量包含安全库存"] = 0; }// + Convert.ToDecimal(dr["受订量"])
                    try
                    {
                        dr["有效总数"] = -(Convert.ToDecimal(dr["受订量"]) + Convert.ToDecimal(dr["未领量"]) - Convert.ToDecimal(dr["库存总数"]) - Convert.ToDecimal(dr["在途量"]) - Convert.ToDecimal(dr["在制量"]));// + Convert.ToDecimal(dr["库存下限"])
                    }
                    catch { dr["有效总数"] = 0; }
                }

                gv.ViewCaption = string.Format("原材料信息合并完成");
                Application.DoEvents();

                sql = @"select 基础数据物料信息表.物料编码,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.物料名称,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.大类,
            基础数据物料信息表.库存下限,基础数据物料信息表.图纸编号,基础数据物料信息表.供应商编号,基础数据物料信息表.仓库号,基础数据物料信息表.仓库名称,基础数据物料信息表.计量单位
            ,基础数据物料信息表.默认供应商,采购供应商备注,仓库物料数量表.库存总数,仓库物料数量表.有效总数,基础数据物料信息表.规格型号,仓库物料数量表.在途量,仓库物料数量表.在制量,仓库物料数量表.受订量,仓库物料数量表.未领量 
            from 基础数据物料信息表 
            left join 仓库物料数量表 on 基础数据物料信息表.物料编码 = 仓库物料数量表.物料编码 
            where 基础数据物料信息表.库存下限 >= 仓库物料数量表.库存总数 and  基础数据物料信息表.物料类型 = '原材料' and 基础数据物料信息表.停用 = 0";
                DataTable dt_安全库存 = new DataTable();
                if (str_person != "admin")
                {
                    if (dt_ls.Rows.Count > 0)
                    {
                        sql = sql + "and ( 基础数据物料信息表.供应商编号= '' or";
                        foreach (DataRow dr in dt_ls.Rows)
                        {
                            sql = sql + string.Format(" 基础数据物料信息表.供应商编号='{0}' or", dr["供应商ID"]);
                        }
                        sql = sql.Substring(0, sql.Length - 2);
                        sql = sql + ")";
                    }
                    else
                    {
                        throw new Exception("你没有对应的供应商,请找信息部核实");
                    }
                }
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_安全库存);
                dt_安全库存.Columns.Add("欠缺数量包含安全库存", typeof(Decimal));
                dt_安全库存.Columns.Add("季度用量", typeof(Decimal));
                dt_安全库存.Columns.Add("上月用量", typeof(Decimal));
                dt_安全库存.Columns.Add("送检数量", typeof(Decimal));
                foreach (DataRow dr in dt_安全库存.Rows)//去除重复项
                {
                    if (dr.RowState == DataRowState.Deleted) continue;
                    DataRow[] ds = dtreturn.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                    if (ds.Length > 0)
                    {
                        dr.Delete();
                    }
                    else
                    {
                        try
                        {
                            DataRow[] dss = dt6.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                            dr["送检数量"] = dss[0]["送检数量"];
                        }
                        catch { dr["送检数量"] = 0; }
                        try
                        {
                            DataRow[] dss = dt4.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                            dr["季度用量"] = dss[0]["季度用量"];
                        }
                        catch { dr["季度用量"] = 0; }
                        try
                        {
                            DataRow[] dss = dt5.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                            dr["上月用量"] = dss[0]["上月用量"];
                        }
                        catch { dr["上月用量"] = 0; }

                        //if (dr["库存下限"] ==DBNull.Value || dr["库存下限"] == null || dr["库存下限"].ToString().Trim() == "")
                        //{
                        //    dr["库存下限"] = 0;
                        //}
                        //else
                        //{
                        dr["欠缺数量包含安全库存"] = Convert.ToDecimal(dr["库存下限"]) - Convert.ToDecimal(dr["库存总数"]) - Convert.ToDecimal(dr["在途量"]);

                        //}
                    }
                }
                dt_安全库存.AcceptChanges();
                dtreturn.Merge(dt_安全库存);//合并两个dt
                gv.ViewCaption = string.Format("处理完成");
                Application.DoEvents();

                return dtreturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static DateTime dtime;
        static DateTime dtime1;
        static DateTime dtime2;
        static DateTime dtime3;

#pragma warning disable IDE1006 // 命名样式
        private static DataTable fun_BOM(string strItemNo, DataTable dt_ls, string str_person)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataTable dt = new DataTable();
                int iMax = 20;
                int i = 0;
                //aa 添了 基础数据物料信息表.物料类型, （此物料类型为子项编码的，穷尽子项同）
                string sql1 = string.Format(@"select 产品编码,子项编码 as 物料编码,数量 as 受订量a,库存总数,有效总数,在途量,在制量,受订量,未领量,
基础数据物料信息表.物料名称,基础数据物料信息表.大类,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.库存下限,基础数据物料信息表.原ERP物料编号,
基础数据物料信息表.规格型号,基础数据物料信息表.图纸编号,基础数据物料信息表.计量单位,基础数据物料信息表.仓库名称,基础数据物料信息表.物料类型,
基础数据物料信息表.供应商编号,基础数据物料信息表.默认供应商,采购供应商备注,基础数据物料信息表.仓库号,基础数据物料信息表.停用,有无蓝图 from 基础数据物料BOM表
left join 仓库物料数量表 on 仓库物料数量表.物料编码 = 基础数据物料BOM表.子项编码
left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 基础数据物料BOM表.子项编码

where 产品编码 = '{0}' and 子项类型<>'采购件' and BOM类型 = '物料BOM' and 优先级 = 1 ", strItemNo);

                if (str_person != "admin")
                {
                    if (dt_ls.Rows.Count > 0)
                    {
                        sql1 = sql1 + "and ( 基础数据物料信息表.供应商编号= '' or";
                        foreach (DataRow dr in dt_ls.Rows)
                        {
                            sql1 = sql1 + string.Format(" 基础数据物料信息表.供应商编号='{0}' or", dr["供应商ID"]);
                        }
                        sql1 = sql1.Substring(0, sql1.Length - 2);
                        sql1 = sql1 + ")";
                    }
                    else
                    {
                        throw new Exception("你没有对应的供应商,请找信息部核实");
                    }
                }

                DataTable dt1 = new DataTable();
                SqlDataAdapter da1 = new SqlDataAdapter(sql1, strconn);
                da1.Fill(dt1);
                //dt1.Columns.Add("层级");
                DataTable tt = dt1.Clone();
                foreach (DataRow r in dt1.Rows)
                {
                    //r["层级"] = i + 1;
                    DataRow dr = tt.NewRow();
                    dr.ItemArray = r.ItemArray;
                    tt.Rows.Add(dr);
                }
                dt = fun_合并datatable1(dt1, fun_穷尽子项1(i, iMax, dt1, tt, strconn, dt_ls, str_person));

                //aa/*2017-03-28 将半成品剔除*/
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["物料类型"].ToString() == "半成品")//将半成品的数量 * 到原材料上，即A-B-2 B-C-1，结果为A-B-2 B-C-2，由于A-B-2中的B为半成品，在最终统计的时候排除A-B-2
                    {
                        DataRow[] ds = dt.Select(string.Format("产品编码 = '{0}'", dr["物料编码"]));
                        foreach (DataRow r in ds)
                        {
                            r["受订量a"] = Convert.ToDecimal(r["受订量a"]) * Convert.ToDecimal(dr["受订量a"]);
                        }
                    }
                }
                return dt;
                //return fun_层级1(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private static DataTable fun_穷尽子项1(int iAs, int iMax, DataTable dt1, DataTable dt2, string strconn, DataTable dt_ls, string str_person)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                iAs++;
                if (iAs >= iMax) throw new Exception("获取失败");
                foreach (DataRow r in dt1.Rows)
                {
                    //20160420 当BOM结构中是原材料时，不往下找。因为未来电器的BOM结构有问题
                    string sqlss = string.Format("select 物料类型 from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"].ToString().Trim());
                    SqlDataAdapter dass = new SqlDataAdapter(sqlss, strconn);
                    DataTable dtss = new DataTable();
                    dass.Fill(dtss);
                    if (dtss.Rows.Count > 0 && dtss.Rows[0]["物料类型"].ToString() == "原材料")
                    {
                        continue;
                    }
                    try
                    {
                        string sql = string.Format(@"select 产品编码,子项编码 as 物料编码,数量 as 受订量a,库存总数,有效总数,在途量,在制量,受订量,未领量,
                    基础数据物料信息表.物料名称,基础数据物料信息表.大类,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.库存下限,基础数据物料信息表.原ERP物料编号,
                    基础数据物料信息表.规格型号,基础数据物料信息表.图纸编号,基础数据物料信息表.计量单位,基础数据物料信息表.仓库名称,基础数据物料信息表.物料类型,
                    基础数据物料信息表.供应商编号,基础数据物料信息表.默认供应商,采购供应商备注,基础数据物料信息表.仓库号,基础数据物料信息表.停用,有无蓝图 
                    from 基础数据物料BOM表
                    left join 仓库物料数量表 on 仓库物料数量表.物料编码 = 基础数据物料BOM表.子项编码 
                    left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 基础数据物料BOM表.子项编码 

                    where 产品编码 = '{0}' and 子项编码 <> '' and 子项类型<>'采购件' and BOM类型 = '物料BOM' and 优先级 = 1", r["物料编码"].ToString());

                        if (str_person != "admin")
                        {
                            if (dt_ls.Rows.Count > 0)
                            {
                                sql = sql + "and ( 基础数据物料信息表.供应商编号= '' or";
                                foreach (DataRow dr in dt_ls.Rows)
                                {
                                    sql = sql + string.Format(" 基础数据物料信息表.供应商编号='{0}' or", dr["供应商ID"]);
                                }
                                sql = sql.Substring(0, sql.Length - 2);
                                sql = sql + ")";
                            }
                            else
                            {
                                throw new Exception("你没有对应的供应商,请找信息部核实");
                            }
                        }
                        //aa //基础数据物料信息表.物料名称,基础数据物料信息表.大类,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.库存下限,
                        //aa //基础数据物料信息表.原ERP物料编号,基础数据物料信息表.物料类型 

                        /*left join  (select 物料编码,-sum(实效数量)as 季度用量  from 仓库出入库明细表 where  出库入库='出库' and  出入库时间>'{2}' and 
                             出入库时间<'{1}'  group by 物料编码)x on  基础数据物料BOM表.子项编码=x.物料编码
left join  (select 物料编码,-sum(实效数量)as 上月用量  from 仓库出入库明细表 where  出库入库='出库' and  出入库时间>'{3}' and 
                             出入库时间<'{1}'  group by 物料编码)y on  基础数据物料BOM表.子项编码=y.物料编码*/
                        DataTable t = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                        da.Fill(t);
                        //t.Columns.Add("层级");
                        //foreach (DataRow rrr in t.Rows)
                        //{
                        //    rrr["层级"] = iAs + 1;
                        //}
                        if (t.Rows.Count > 0)
                        {
                            //dt2.Merge(t);
                            fun_合并datatable1(dt2, t);
                            fun_穷尽子项1(iAs, iMax, t, dt2, strconn, dt_ls, str_person);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                return dt2;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                iAs--;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private static DataTable fun_合并datatable1(DataTable dt1, DataTable dt2)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                foreach (DataRow r in dt2.Rows)
                {
                    DataRow[] ds = dt1.Select(string.Format("产品编码 = '{0}' and 物料编码 = '{1}'", r["产品编码"].ToString(), r["物料编码"].ToString()));
                    if (ds.Length == 0)
                    {
                        DataRow dr = dt1.NewRow();
                        dr.ItemArray = r.ItemArray;
                        dt1.Rows.Add(dr);
                    }
                }
                return dt1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private static DataTable fun_层级1(DataTable dt)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataTable dts = dt;
                foreach (DataRow r in dts.Rows)
                {
                    DataRow[] ds = dts.Select(string.Format("物料编码 = '{0}'", r["物料编码"].ToString()));
                    if (ds.Length > 1)
                    {
                        int a = 0;
                        foreach (DataRow sr in ds)
                        {
                            if (a < Convert.ToInt32(sr["层级"]))
                            {
                                a = Convert.ToInt32(sr["层级"]);
                            }
                        }
                        foreach (DataRow sr2 in ds)
                        {
                            sr2["层级"] = a;
                        }
                    }
                }
                return dts;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 下销售单,制令生效 等处 该产品有无BOM 
        ///  20-6-17 好像只有制令生效调用了
        /// </summary>
        /// <returns></returns>
        public static string fun_flag(string str_物料编码, bool bl_销售)
#pragma warning restore IDE1006 // 命名样式
        {
            string str_返回 = "";
            //无BOM


            //无仓库
            string sql = string.Format("select 车间编号,仓库名称,物料类型,工时,物料编码,n核算单价,自制 from 基础数据物料信息表 where 物料编码='{0}'", str_物料编码);
            DataRow dr = CZMaster.MasterSQL.Get_DataRow(sql, strconn);
            if (dr["仓库名称"].ToString().Trim() == "")
            {
                str_返回 = str_返回 + "物料:" + dr["物料编码"].ToString() + "没有对应仓库\n";
                //return str_返回;
            }
            if (Convert.ToBoolean(dr["自制"])) //20-6-17 修改自制的需要有bom  下销售单和 制令
            {
                string sql_1 = string.Format("select count(*)条数 from 基础数据物料BOM表 where 产品编码='{0}'", str_物料编码);
                DataRow dr_1 = CZMaster.MasterSQL.Get_DataRow(sql_1, strconn);
                if (Convert.ToDecimal(dr_1["条数"].ToString()) == 0)
                {
                    str_返回 = str_返回 + "物料:" + dr["物料编码"].ToString() + "无BOM";
                }
            }
            //if (bl_销售 == false)  //不是销售   需要判断
            //{
                
            //    string sql_1 = string.Format("select count(*)条数 from 基础数据物料BOM表 where 产品编码='{0}'", str_物料编码);
            //    DataRow dr_1 = CZMaster.MasterSQL.Get_DataRow(sql_1, strconn);
            //    if (Convert.ToDecimal(dr_1["条数"].ToString()) == 0)
            //    {
            //        str_返回 = str_返回 + "物料:" + dr["物料编码"].ToString() + "无BOM";
            //    }
            //}
            //else     // 销售
            //{
            //    if (dr["物料类型"].ToString() == "成品")
            //    {
            //        string sql_1 = string.Format("select count(*)条数 from 基础数据物料BOM表 where 产品编码='{0}'", str_物料编码);
            //        DataRow dr_1 = CZMaster.MasterSQL.Get_DataRow(sql_1, strconn);
            //        if (Convert.ToDecimal(dr_1["条数"].ToString()) == 0)
            //        {
            //            str_返回 = str_返回 + "物料:" + dr["物料编码"].ToString() + "无BOM";
            //        }
            //    }

            //}

            return str_返回;

        }
        #endregion

        #region 测试
#pragma warning disable IDE1006 // 命名样式
        public static DataTable fun_销售受订量_测试(DevExpress.XtraGrid.Views.Grid.GridView gv, DateTime time, DataTable dt_ls, string str_person)
#pragma warning restore IDE1006 // 命名样式
        {
            dtime = CPublic.Var.getDatetime();
            dtime = dtime.AddDays(-dtime.Day + 1);
            dtime1 = dtime.AddMonths(-3);
            dtime2 = dtime.AddMonths(-1);
            /*以下为销售单部分*/
            gv.ViewCaption = string.Format("准备处理销售单的原材料部分");
            Application.DoEvents();

            string sql = string.Format(@"select [销售记录销售订单明细表].物料编码,SUM(未完成数量) as 受订量a,物料类型 from [销售记录销售订单明细表] 
	left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = [销售记录销售订单明细表].物料编码
 
	where ([明细完成] = 0 and [销售记录销售订单明细表].生效 = 1 and 作废 = 0 
		and (总完成 = 0 or 总完成 is null) and [销售记录销售订单明细表].关闭 = 0 and 未完成数量 > 0
		and 生效日期 >= '{0}'and 基础数据物料信息表.物料类型 = '成品') 
       or ([明细完成] = 0 and [销售记录销售订单明细表].生效 = 1 and 作废 = 0 
		and (总完成 = 0 or 总完成 is null) and [销售记录销售订单明细表].关闭 = 0 and 未完成数量 > 0
		and 生效日期 >= '{0}'and 基础数据物料信息表.物料类型 = '半成品') 
	group by [销售记录销售订单明细表].物料编码,物料类型", time, dtime, dtime1, dtime2);
            DataTable dt1 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt1);
            DataTable dtreturn_销售单 = new DataTable();
            foreach (DataRow dr in dt1.Rows)
            {
                DataTable t = fun_BOM(dr["物料编码"].ToString(), dt_ls, str_person);
                foreach (DataRow r in t.Rows)
                {
                    r["受订量a"] = Convert.ToDecimal(r["受订量a"]) * Convert.ToDecimal(dr["受订量a"]);
                }
                dtreturn_销售单.Merge(t);  // 分解到所有的叶子节点  成品 半成品
            }
            sql = string.Format(@"select [销售记录销售订单明细表].物料编码,SUM(未完成数量) as 受订量a,库存总数,有效总数,在途量,在制量,受订量,未领量,
基础数据物料信息表.物料名称,基础数据物料信息表.大类,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.库存下限,基础数据物料信息表.原ERP物料编号,
基础数据物料信息表.规格型号,基础数据物料信息表.图纸编号,基础数据物料信息表.计量单位,基础数据物料信息表.仓库名称,基础数据物料信息表.物料类型,
基础数据物料信息表.供应商编号,基础数据物料信息表.默认供应商,采购供应商备注,基础数据物料信息表.仓库号,基础数据物料信息表.停用,有无蓝图 
from [销售记录销售订单明细表] 
left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = [销售记录销售订单明细表].物料编码
left join 仓库物料数量表 on 仓库物料数量表.物料编码 = 销售记录销售订单明细表.物料编码
where [明细完成] = 0 and [销售记录销售订单明细表].生效 = 1 and 作废 = 0 
and (总完成 = 0 or 总完成 is null) and [销售记录销售订单明细表].关闭 = 0 and 未完成数量 > 0
and 生效日期 > '{0}' and 物料类型 = '原材料' ", time, dtime, dtime1, dtime2);
            if (str_person != "admin")
            {
                if (dt_ls.Rows.Count > 0)
                {
                    sql = sql + "and ( 基础数据物料信息表.供应商编号= '' or";
                    foreach (DataRow dr in dt_ls.Rows)
                    {
                        sql = sql + string.Format(" 基础数据物料信息表.供应商编号='{0}' or", dr["供应商ID"]);
                    }
                    sql = sql.Substring(0, sql.Length - 2);
                    sql = sql + ")";
                }
                else
                {
                    throw new Exception("你没有对应的供应商,请找信息部核实");
                }
            }
            sql = sql + @" group by [销售记录销售订单明细表].物料编码,物料类型,库存总数,有效总数,在途量,在制量,受订量,未领量,
基础数据物料信息表.物料名称,基础数据物料信息表.大类,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.库存下限,基础数据物料信息表.原ERP物料编号,
基础数据物料信息表.规格型号,基础数据物料信息表.图纸编号,基础数据物料信息表.计量单位,基础数据物料信息表.仓库名称,基础数据物料信息表.停用,有无蓝图,
基础数据物料信息表.供应商编号,基础数据物料信息表.默认供应商,采购供应商备注,基础数据物料信息表.仓库号";
            dt1.Clear();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt1);
            dtreturn_销售单.Merge(dt1);
            //得到原材料集合--dtreturn_销售单

            /*以下为制令单部分*/
            gv.ViewCaption = string.Format("准备处理制令单的原材料部分");
            Application.DoEvents();

            sql = string.Format(@"select 生产记录生产制令表.物料编码,SUM(未排单数量) as 受订量a,基础数据物料信息表.物料类型 
	from 生产记录生产制令表 
	left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产制令表.物料编码
	where 未排单数量 > 0 /*and 生产记录生产制令表.生效 = 1*/ and 完成 = 0 and 生产记录生产制令表.关闭 = 0
		and 生产记录生产制令表.日期 >= '{0}'and 基础数据物料信息表.物料类型 = '成品'
	group by 生产记录生产制令表.物料编码,基础数据物料信息表.物料类型", time, dtime, dtime1, dtime2);
            dt1.Clear();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt1);
            DataTable dtreturn_制令单_成品 = new DataTable();
            foreach (DataRow dr in dt1.Rows)
            {
                DataTable t = fun_BOM(dr["物料编码"].ToString(), dt_ls, str_person);
                foreach (DataRow r in t.Rows)
                {
                    r["受订量a"] = Convert.ToDecimal(r["受订量a"]) * Convert.ToDecimal(dr["受订量a"]);
                }
                dtreturn_制令单_成品.Merge(t);
            }
            sql = string.Format(@"select 生产记录生产制令表.物料编码,SUM(未排单数量) as 受订量a,基础数据物料信息表.物料类型 
	from 生产记录生产制令表 
	left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产制令表.物料编码
	where 未排单数量 > 0 /*and 生产记录生产制令表.生效 = 1*/ and 完成 = 0 and 生产记录生产制令表.关闭 = 0
		and 生产记录生产制令表.日期 >= '{0}'and 基础数据物料信息表.物料类型 = '半成品'
	group by 生产记录生产制令表.物料编码,基础数据物料信息表.物料类型", time, dtime, dtime1, dtime2);
            dt1.Clear();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt1);
            DataTable dtreturn_制令单_半成品 = new DataTable();
            foreach (DataRow dr in dt1.Rows)
            {
                DataTable t = fun_BOM(dr["物料编码"].ToString(), dt_ls, str_person);
                foreach (DataRow r in t.Rows)
                {
                    r["受订量a"] = Convert.ToDecimal(r["受订量a"]) * Convert.ToDecimal(dr["受订量a"]);
                }
                dtreturn_制令单_半成品.Merge(t);
            }
            //比较成品和半成品的原材料大小
            foreach (DataRow r in dtreturn_制令单_半成品.Rows)
            {
                DataRow[] ds = dtreturn_制令单_成品.Select(string.Format("物料编码 = '{0}'", r["物料编码"]));
                if (ds.Length > 0)
                {
                    if (Convert.ToDecimal(r["受订量a"]) > Convert.ToDecimal(ds[0]["受订量a"]))
                    {
                        ds[0]["受订量a"] = r["受订量a"];
                    }
                }
                else
                {
                    DataRow dr = dtreturn_制令单_成品.NewRow();
                    dtreturn_制令单_成品.Rows.Add(dr);
                    dr.ItemArray = r.ItemArray;
                }
            }
            //得到原材料集合--dtreturn_制令单_成品

            /*以下为最终比较*/
            gv.ViewCaption = string.Format("正在合并最终原材料");
            Application.DoEvents();

            foreach (DataRow r in dtreturn_制令单_成品.Rows)
            {
                DataRow[] ds = dtreturn_销售单.Select(string.Format("物料编码 = '{0}'", r["物料编码"]));
                if (ds.Length > 0)
                {
                    if (Convert.ToDecimal(r["受订量a"]) > Convert.ToDecimal(ds[0]["受订量a"]))
                    {
                        ds[0]["受订量a"] = r["受订量a"];
                    }
                }
                else
                {
                    DataRow dr = dtreturn_销售单.NewRow();
                    dtreturn_销售单.Rows.Add(dr);
                    dr.ItemArray = r.ItemArray;
                }
            }

            gv.ViewCaption = string.Format("正在添加原材料的附属信息");
            Application.DoEvents();

            sql = string.Format(@"select 物料编码,-sum(实效数量)as 季度用量  from 仓库出入库明细表 where  出库入库='出库' and  出入库时间>'{1}' and 
                             出入库时间<'{0}'  group by 物料编码", dtime, dtime1);
            DataTable dt4 = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt4);

            sql = string.Format(@"select 物料编码,-sum(实效数量)as 上月用量  from 仓库出入库明细表 where  出库入库='出库' and  出入库时间>'{1}' and 
                             出入库时间<'{0}'  group by 物料编码", dtime, dtime2);
            DataTable dt5 = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt5);

            sql = string.Format(@"select 物料编码,SUM(送检数量) as 送检数量 from (            
select 采购记录采购送检单明细表.物料编码,送检数量 from 采购记录采购送检单明细表 
left join 采购记录采购单明细表 on 采购记录采购单明细表.采购明细号 = 采购记录采购送检单明细表.采购单明细号 
where  采购记录采购单明细表.明细完成日期 is null and 检验完成 = 0 and 采购记录采购送检单明细表.作废 = 0 and 采购记录采购单明细表.作废 = 0 and 采购记录采购送检单明细表.生效日期 >= '{0}' 
union
select 产品编号 as 物料编码,采购记录采购单检验主表.送检数量 from 采购记录采购单检验主表
left join 采购记录采购单明细表 on 采购记录采购单明细表.采购明细号 = 采购记录采购单检验主表.采购明细号 and 采购记录采购单明细表.作废 = 0 
left join 采购记录采购送检单主表  on 采购记录采购送检单主表.送检单号=采购记录采购单检验主表.送检单号
where 采购记录采购单明细表.明细完成日期 is null and 关闭 = 0 and 入库完成 = 0 and 检验日期 >= '{0}' and (检验结果='合格' or  采购已处理=0)) a
group by 物料编码", time);
            DataTable dt6 = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt6);

            MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
            dtreturn_销售单 = RBQ.SelectGroupByInto("", dtreturn_销售单, @"物料编码,原ERP物料编号,物料名称,大类,n原ERP规格型号,库存下限,规格型号,图纸编号,仓库号,仓库名称,供应商编号
                                                    ,默认供应商,采购供应商备注,计量单位,库存总数,有效总数,在途量,在制量,受订量,未领量,sum(受订量a) 受订量a,停用,有无蓝图", "物料类型 = '原材料'",
                                                 "物料编码,原ERP物料编号,库存总数,有效总数,在途量,在制量,受订量,未领量,库存下限,规格型号,图纸编号,仓库号,仓库名称,供应商编号,默认供应商,采购供应商备注,计量单位,停用,有无蓝图");//,季度用量,上月用量
            dtreturn_销售单.Columns.Add("季度用量", typeof(Decimal));
            dtreturn_销售单.Columns.Add("上月用量", typeof(Decimal));
            dtreturn_销售单.Columns.Add("送检数量", typeof(Decimal));
            dtreturn_销售单.Columns.Add("欠缺数量不含安全库存", typeof(Decimal));
            dtreturn_销售单.Columns.Add("欠缺数量包含安全库存", typeof(Decimal));
            foreach (DataRow dr in dtreturn_销售单.Rows)
            {
                try
                {
                    DataRow[] ds = dt6.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                    dr["送检数量"] = ds[0]["送检数量"];
                }
                catch { dr["送检数量"] = 0; }
                try
                {
                    DataRow[] ds = dt4.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                    dr["季度用量"] = ds[0]["季度用量"];
                }
                catch { dr["季度用量"] = 0; }
                try
                {
                    DataRow[] ds = dt5.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                    dr["上月用量"] = ds[0]["上月用量"];
                }
                catch { dr["上月用量"] = 0; }
                try
                {
                    dr["欠缺数量不含安全库存"] = Convert.ToDecimal(dr["受订量a"]) + Convert.ToDecimal(dr["未领量"]) - Convert.ToDecimal(dr["库存总数"]) - Convert.ToDecimal(dr["在途量"]) - Convert.ToDecimal(dr["在制量"]);
                }
                catch { dr["欠缺数量不含安全库存"] = 0; }
                try
                {
                    dr["欠缺数量包含安全库存"] = Convert.ToDecimal(dr["受订量a"]) + Convert.ToDecimal(dr["未领量"]) - Convert.ToDecimal(dr["库存总数"]) - Convert.ToDecimal(dr["在途量"]) - Convert.ToDecimal(dr["在制量"]) + Convert.ToDecimal(dr["库存下限"]);
                }
                catch { dr["欠缺数量包含安全库存"] = 0; }
                try
                {
                    dr["有效总数"] = -(Convert.ToDecimal(dr["受订量"]) + Convert.ToDecimal(dr["未领量"]) - Convert.ToDecimal(dr["库存总数"]) - Convert.ToDecimal(dr["在途量"]) - Convert.ToDecimal(dr["在制量"]));// + Convert.ToDecimal(dr["库存下限"])
                }
                catch { dr["有效总数"] = 0; }
            }

            sql = @"select 基础数据物料信息表.物料编码,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.物料名称,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.大类,
            基础数据物料信息表.库存下限,基础数据物料信息表.图纸编号,基础数据物料信息表.供应商编号,基础数据物料信息表.仓库号,基础数据物料信息表.仓库名称,基础数据物料信息表.计量单位
            ,基础数据物料信息表.默认供应商,采购供应商备注,仓库物料数量表.库存总数,仓库物料数量表.有效总数,基础数据物料信息表.规格型号,仓库物料数量表.在途量,仓库物料数量表.在制量,仓库物料数量表.受订量,仓库物料数量表.未领量 
            from 基础数据物料信息表 
            left join 仓库物料数量表 on 基础数据物料信息表.物料编码 = 仓库物料数量表.物料编码 
            where 基础数据物料信息表.库存下限 >= 仓库物料数量表.库存总数 and  基础数据物料信息表.物料类型 = '原材料' and 基础数据物料信息表.停用 = 0";
            DataTable dt_安全库存 = new DataTable();
            if (str_person != "admin")
            {
                if (dt_ls.Rows.Count > 0)
                {
                    sql = sql + "and ( 基础数据物料信息表.供应商编号= '' or";
                    foreach (DataRow dr in dt_ls.Rows)
                    {
                        sql = sql + string.Format(" 基础数据物料信息表.供应商编号='{0}' or", dr["供应商ID"]);
                    }
                    sql = sql.Substring(0, sql.Length - 2);
                    sql = sql + ")";
                }
                else
                {
                    throw new Exception("你没有对应的供应商,请找信息部核实");
                }
            }
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_安全库存);
            dt_安全库存.Columns.Add("欠缺数量包含安全库存", typeof(Decimal));
            dt_安全库存.Columns.Add("季度用量", typeof(Decimal));
            dt_安全库存.Columns.Add("上月用量", typeof(Decimal));
            dt_安全库存.Columns.Add("送检数量", typeof(Decimal));
            foreach (DataRow dr in dt_安全库存.Rows)//去除重复项
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                DataRow[] ds = dtreturn_销售单.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                if (ds.Length > 0)
                {
                    dr.Delete();
                }
                else
                {
                    try
                    {
                        DataRow[] dss = dt6.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                        dr["送检数量"] = dss[0]["送检数量"];
                    }
                    catch { dr["送检数量"] = 0; }
                    try
                    {
                        DataRow[] dss = dt4.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                        dr["季度用量"] = dss[0]["季度用量"];
                    }
                    catch { dr["季度用量"] = 0; }
                    try
                    {
                        DataRow[] dss = dt5.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                        dr["上月用量"] = dss[0]["上月用量"];
                    }
                    catch { dr["上月用量"] = 0; }

                    dr["欠缺数量包含安全库存"] = Convert.ToDecimal(dr["库存下限"]) - Convert.ToDecimal(dr["库存总数"]) - Convert.ToDecimal(dr["在途量"]);
                }
            }
            dt_安全库存.AcceptChanges();
            dtreturn_销售单.Merge(dt_安全库存);//合并两个dt
            gv.ViewCaption = string.Format("处理完成");
            Application.DoEvents();

            return dtreturn_销售单;
        }
        #endregion

        #region 2017/07/26采购池算法
        public static DataTable dt_BOM_备份;

#pragma warning disable IDE1006 // 命名样式
        public static DataTable fun_销售受订量_S2(DevExpress.XtraGrid.Views.Grid.GridView gv, DateTime time, DataTable dt_ls, string str_person)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string fName = System.IO.Directory.GetCurrentDirectory() + "\\MasterCom\\Future";
                if (Directory.Exists(fName) == false)
                {
                    Directory.CreateDirectory(fName);
                }

                dtime = CPublic.Var.getDatetime();
                //   dtime = dtime.AddDays(-dtime.Day + 1);

                string sstr = dtime.ToString("yyyy-MM-dd_HH-mm-ss");

                string fileName = System.IO.Directory.GetCurrentDirectory() + "\\MasterCom\\Future\\" + sstr + ".txt";
                FileStream fs = new FileStream(@fileName, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("GB2312"));

                dtime1 = dtime.AddMonths(-3);
                dtime2 = dtime.AddMonths(-1);
                dtime3 = dtime.AddMonths(-6);
                DataTable dtreturn = new DataTable();
                /*扣掉库存的受订量
                 有成品，半成品，原材料*/
                string sql = string.Format(@"select a.物料编码,a.受订量a,a.物料类型
,基础数据物料信息表.物料名称,基础数据物料信息表.大类,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.原ERP物料编号,
基础数据物料信息表.规格型号,基础数据物料信息表.图纸编号,基础数据物料信息表.计量单位编码,基础数据物料信息表.计量单位,基础数据物料信息表.仓库名称,
基础数据物料信息表.供应商编号,基础数据物料信息表.默认供应商,采购供应商备注,基础数据物料信息表.仓库号,新数据 from 
(
 (select aa.物料编码,case when((aa.受订量 - 仓库物料数量表.库存总数 - 仓库物料数量表.在制量 - 仓库物料数量表.在途量 + 仓库物料数量表.未领量) > isnull(s.制令量,0)) then (aa.受订量 - 仓库物料数量表.库存总数 - 仓库物料数量表.在制量 - 仓库物料数量表.在途量 + 仓库物料数量表.未领量) else isnull(s.制令量,0) end as 受订量a,aa.物料类型 
from 
	(select [销售记录销售订单明细表].物料编码,SUM(未完成数量) as 受订量,物料类型 from [销售记录销售订单明细表] 
	left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = [销售记录销售订单明细表].物料编码
	where [明细完成] = 0 and [销售记录销售订单明细表].生效 = 1 and 作废 = 0 
		and (总完成 = 0 or 总完成 is null) and [销售记录销售订单明细表].关闭 = 0 and 未完成数量 > 0
		and 生效日期 >= '{0}'and 基础数据物料信息表.物料类型 = '成品'
	group by [销售记录销售订单明细表].物料编码,物料类型) aa                    /* 这里取销售明细（未完成、未关闭、未完成数量>0、物料类型=成品的的 未完成数量总和 为 受订量）       */
left join 
	(select 生产记录生产制令表.物料编码,SUM(未排单数量) as 制令量,生产记录生产制令表.仓库号,基础数据物料信息表.物料类型
	from 生产记录生产制令表 
	left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产制令表.物料编码
	where 未排单数量 > 0 /*and 生产记录生产制令表.生效 = 1 */ and 完成 = 0 and 生产记录生产制令表.关闭 = 0
		and 生产记录生产制令表.日期 >= '{0}'and 基础数据物料信息表.物料类型 = '成品'
	group by 生产记录生产制令表.物料编码,基础数据物料信息表.物料类型,生产记录生产制令表.仓库号) s                                   /* 这里取制令（未完成、未关闭、未排单数量>0、物料类型=成品的 未排单数量 总和 为制令量）       */
on s.物料编码 = aa.物料编码
    left join 仓库物料数量表 on 仓库物料数量表.物料编码 = aa.物料编码 and s.仓库号=仓库物料数量表.仓库号
 )                                                                                                          /* 上面 受订-库存-在制-在途+未领  与 制令量 取大的 */
union 
 (select 生产记录生产制令表.物料编码,SUM(未排单数量) as 受订量a,基础数据物料信息表.物料类型                 /* 下面是 取制令 中 去除 在 上面出现过的 物料 简而言之 去 不是成品的 制令 */
from 生产记录生产制令表 
	left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产制令表.物料编码
	where 未排单数量 > 0 /*and 生产记录生产制令表.生效 = 1 */ and 完成 = 0 and 生产记录生产制令表.关闭 = 0 and 基础数据物料信息表.物料类型 = '成品'
		and 生产记录生产制令表.日期 >= '{0}' and 生产记录生产制令表.物料编码 not in 
		(select aa.物料编码 from 
			(select [销售记录销售订单明细表].物料编码 
			from [销售记录销售订单明细表] 
			where [明细完成] = 0 and [销售记录销售订单明细表].生效 = 1 and 作废 = 0 
			and (总完成 = 0 or 总完成 is null) and [销售记录销售订单明细表].关闭 = 0 and 未完成数量 > 0
			and 生效日期 >= '{0}' and 基础数据物料信息表.物料类型 = '成品'
			group by [销售记录销售订单明细表].物料编码) aa
		left join 
			(select 生产记录生产制令表.物料编码 
			from 生产记录生产制令表 
			where 未排单数量 > 0 /*and 生产记录生产制令表.生效 = 1*/ and 完成 = 0 and 生产记录生产制令表.关闭 = 0
			and 生产记录生产制令表.日期 >= '{0}' and 基础数据物料信息表.物料类型 = '成品'
			group by 生产记录生产制令表.物料编码) s
		on s.物料编码 = aa.物料编码
		where s.物料编码 is not null and 基础数据物料信息表.物料类型 = '成品')  
	group by 生产记录生产制令表.物料编码,基础数据物料信息表.物料类型
 )
) a  
left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = a.物料编码              
where a.受订量a > 0", time);//and 生产记录生产制令表.生效 = 1 
                DataTable dt1 = new DataTable();
                //  dt1 = WSAdapter.webservers_getdata.wsfun.GetData_ERP(sql);
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt1);

                sql = string.Format(@"select a.物料编码,a.受订量a,a.物料类型
,基础数据物料信息表.规格型号,基础数据物料信息表.图纸编号,基础数据物料信息表.计量单位编码,基础数据物料信息表.计量单位,基础数据物料信息表.仓库名称,基础数据物料信息表.原ERP物料编号,
基础数据物料信息表.供应商编号,基础数据物料信息表.默认供应商,采购供应商备注,基础数据物料信息表.仓库号,新数据  from 

((select aa.物料编码,case when((aa.受订量-仓库物料数量表.库存总数 - 仓库物料数量表.在制量 - 仓库物料数量表.在途量 + 仓库物料数量表.未领量) > isnull(s.制令量,0)) then (aa.受订量 - 仓库物料数量表.库存总数 - 仓库物料数量表.在制量 - 仓库物料数量表.在途量 + 仓库物料数量表.未领量) else isnull(s.制令量,0) end as 受订量a,aa.物料类型 
from 
	(select [销售记录销售订单明细表].物料编码,SUM(未完成数量) as 受订量,物料类型 from [销售记录销售订单明细表] 
	left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = [销售记录销售订单明细表].物料编码
	where [明细完成] = 0 and [销售记录销售订单明细表].生效 = 1 and 作废 = 0 
		and (总完成 = 0 or 总完成 is null) and [销售记录销售订单明细表].关闭 = 0 and 未完成数量 > 0
		and 生效日期 >= '{0}'and 基础数据物料信息表.物料类型 ='半成品'
	group by [销售记录销售订单明细表].物料编码,物料类型) aa
left join 
	(select 生产记录生产制令表.物料编码,SUM(未排单数量) as 制令量,基础数据物料信息表.物料类型,生产记录生产制令表.仓库号 
	from 生产记录生产制令表 
	left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产制令表.物料编码  
	where 未排单数量 > 0 /*and 生产记录生产制令表.生效 = 1*/ and 完成 = 0 and 生产记录生产制令表.关闭 = 0
		and 生产记录生产制令表.日期 >= '{0}'and 基础数据物料信息表.物料类型 = '半成品'
	group by 生产记录生产制令表.物料编码,基础数据物料信息表.物料类型,生产记录生产制令表.仓库号) s
on s.物料编码 = aa.物料编码
left join 仓库物料数量表 on 仓库物料数量表.物料编码 = aa.物料编码  and s.仓库号=仓库物料数量表.仓库号
)  
union 
(select 生产记录生产制令表.物料编码,SUM(未排单数量) as 受订量a,基础数据物料信息表.物料类型 
from 生产记录生产制令表 
	left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产制令表.物料编码
	where 未排单数量 > 0 /*and 生产记录生产制令表.生效 = 1*/ and 完成 = 0 and 生产记录生产制令表.关闭 = 0 and 基础数据物料信息表.物料类型 = '半成品'
		and 生产记录生产制令表.日期 >= '{0}' and 生产记录生产制令表.物料编码 not in 
		(select aa.物料编码 from 
			(select [销售记录销售订单明细表].物料编码 
			from [销售记录销售订单明细表] 
			where [明细完成] = 0 and [销售记录销售订单明细表].生效 = 1 and 作废 = 0 
			and (总完成 = 0 or 总完成 is null) and [销售记录销售订单明细表].关闭 = 0 and 未完成数量 > 0
			and 生效日期 >= '{0}' and 基础数据物料信息表.物料类型 = '半成品'
			group by [销售记录销售订单明细表].物料编码) aa
		     left join 
			(select 生产记录生产制令表.物料编码 
			from 生产记录生产制令表 
			where 未排单数量 > 0 /*and 生产记录生产制令表.生效 = 1*/ and 完成 = 0 and 生产记录生产制令表.关闭 = 0
			and 生产记录生产制令表.日期 >= '{0}' and 基础数据物料信息表.物料类型 = '半成品'
			group by 生产记录生产制令表.物料编码) s
		     on s.物料编码 = aa.物料编码
		where s.物料编码 is not null and 基础数据物料信息表.物料类型 = '半成品')
	group by 生产记录生产制令表.物料编码,基础数据物料信息表.物料类型)) a 

left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = a.物料编码
where a.受订量a > 0", time);
                DataTable dt2 = new DataTable();

                //dt2 = WSAdapter.webservers_getdata.wsfun.GetData_ERP(sql);
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt2);
                foreach (DataRow dr in dt2.Rows)
                {
                    //  DataTable t =  fun_BOM(dr["物料编码"].ToString(), dt_ls, str_person);
                    DataTable t = new DataTable();
                    //t.TableName="name";
                    t = fun_BOM(dr["物料编码"].ToString(), dt_ls, str_person);

                    foreach (DataRow r in t.Rows)
                    {
                        r["受订量a"] = Convert.ToDecimal(r["受订量a"]) * Convert.ToDecimal(dr["受订量a"]);
                    }
                    dtreturn.Merge(t);
                }
                dtreturn.Columns.Add("层级");

                //dt1.Merge(dt2);
                DataTable dt_层级 = new DataTable();
                dt_层级.Columns.Add("产品编码");
                dt_层级.Columns.Add("物料编码");
                dt_层级.Columns.Add("受订量a");
                dt_层级.Columns.Add("库存总数");
                dt_层级.Columns.Add("有效总数");
                dt_层级.Columns.Add("在途量");
                dt_层级.Columns.Add("在制量");
                dt_层级.Columns.Add("受订量");
                dt_层级.Columns.Add("未领量");
                dt_层级.Columns.Add("物料名称");
                dt_层级.Columns.Add("大类");
                dt_层级.Columns.Add("n原ERP规格型号");
                dt_层级.Columns.Add("库存下限");
                dt_层级.Columns.Add("原ERP物料编号");
                dt_层级.Columns.Add("规格型号");
                dt_层级.Columns.Add("图纸编号");
                dt_层级.Columns.Add("计量单位");
                dt_层级.Columns.Add("计量单位编码");
                dt_层级.Columns.Add("仓库名称");
                dt_层级.Columns.Add("物料类型");
                dt_层级.Columns.Add("供应商编号");
                dt_层级.Columns.Add("默认供应商");
                dt_层级.Columns.Add("采购供应商备注");
                dt_层级.Columns.Add("仓库号");
                dt_层级.Columns.Add("停用", typeof(bool));
                dt_层级.Columns.Add("有无蓝图", typeof(bool));

                dt_层级.Columns.Add("层级");
                fun_BOM分层(dt1, dt_层级, dt_ls, str_person);//dt_层级为返回的结果
                //dt_层级 =WSAdapter.webservers_getdata.wsfun.fun_BOM分层(dt_BOM_备份,dt1, dt_层级, dt_ls, str_person).Tables[0];//dt_层级为返回的结果
                //dt_BOM_备份 = WSAdapter.webservers_getdata.wsfun.fun_BOM分层(dt_BOM_备份, dt1, dt_层级, dt_ls, str_person).Tables[1];//dt_层级为返回的结果

                sql = string.Format(@"select [销售记录销售订单明细表].物料编码,SUM(未完成数量) as 受订量a,库存总数,有效总数,在途量,在制量,受订量,未领量,
基础数据物料信息表.物料名称,基础数据物料信息表.大类,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.库存下限,基础数据物料信息表.原ERP物料编号,
基础数据物料信息表.规格型号,基础数据物料信息表.图纸编号,基础数据物料信息表.计量单位编码,基础数据物料信息表.计量单位,基础数据物料信息表.仓库名称,基础数据物料信息表.物料类型,
基础数据物料信息表.供应商编号,基础数据物料信息表.默认供应商,采购供应商备注,基础数据物料信息表.仓库号,基础数据物料信息表.停用,有无蓝图,新数据 
from [销售记录销售订单明细表] 
left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = [销售记录销售订单明细表].物料编码
left join 仓库物料数量表 on 仓库物料数量表.物料编码 = 销售记录销售订单明细表.物料编码 and 仓库物料数量表.仓库号=销售记录销售订单明细表.仓库号

where [明细完成] = 0 and [销售记录销售订单明细表].生效 = 1 and 作废 = 0 
and (总完成 = 0 or 总完成 is null) and [销售记录销售订单明细表].关闭 = 0 and 未完成数量 > 0
and 生效日期 > '{0}' and 物料类型 = '原材料' ", time);
                //aa 添了 基础数据物料信息表.物料类型,
                if (str_person != "admin")
                {
                    if (dt_ls.Rows.Count > 0)
                    {
                        sql = sql + "and ( 基础数据物料信息表.供应商编号= '' or";
                        foreach (DataRow dr in dt_ls.Rows)
                        {
                            sql = sql + string.Format(" 基础数据物料信息表.供应商编号='{0}' or", dr["供应商ID"]);
                        }
                        sql = sql.Substring(0, sql.Length - 2);
                        sql = sql + ")";
                    }
                    else
                    {
                        throw new Exception("你没有对应的供应商,请找信息部核实");
                    }
                }
                sql = sql + @" group by [销售记录销售订单明细表].物料编码,物料类型,库存总数,有效总数,在途量,在制量,受订量,未领量,
基础数据物料信息表.物料名称,基础数据物料信息表.大类,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.库存下限,基础数据物料信息表.原ERP物料编号,
基础数据物料信息表.规格型号,基础数据物料信息表.图纸编号,基础数据物料信息表.计量单位编码,基础数据物料信息表.计量单位,基础数据物料信息表.仓库名称,基础数据物料信息表.停用,有无蓝图,
基础数据物料信息表.供应商编号,基础数据物料信息表.默认供应商,采购供应商备注,基础数据物料信息表.仓库号,新数据 ";

                DataTable dt3 = new DataTable();
                //dt3 = WSAdapter.webservers_getdata.wsfun.GetData_ERP(sql);
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt3);
                dt3.Columns.Add("层级");
                dtreturn.Merge(dt3);

                //foreach (DataRow dr in dt3.Rows)
                //{
                //    DataRow[] dss = dt_层级.Select(string.Format("物料编码 = '{0}'", dr["物料编码"].ToString()));
                //    if (dss.Length > 0)
                //    {
                //        dss[0]["数量"] = Convert.ToDecimal(dss[0]["数量"]) + Convert.ToDecimal(dr["受订量a"]);
                //    }
                //    else
                //    {
                //        DataRow rr = dt_层级.NewRow();
                //        dt_层级.Rows.Add(rr);
                //        rr["物料编码"] = dr["物料编码"];
                //        rr["层级"] = 1;
                //        rr["数量"] = dr["受订量a"];
                //        rr["物料类型"] = dr["物料类型"];
                //    }
                //}

                foreach (DataRow dr in dt_层级.Rows)
                {
                    if (dr["物料类型"].ToString() == "半成品")
                    {
                        sql = string.Format(@"select (库存总数+在制量-未领量) as 有效库存 from 仓库物料数量表 where 物料编码 = '{0}'", dr["物料编码"]);
                        //受订量-仓库物料数量表.库存总数 - 仓库物料数量表.在制量 - 仓库物料数量表.在途量 + 仓库物料数量表.未领量
                        DataTable dt_半成品_库存 = new DataTable();

                        //dt_半成品_库存 = WSAdapter.webservers_getdata.wsfun.GetData_ERP(sql);
                        da = new SqlDataAdapter(sql, strconn);
                        da.Fill(dt_半成品_库存);
                        if (dt_半成品_库存.Rows.Count > 0)
                        {
                            if (Convert.ToDecimal(dr["受订量a"]) > Convert.ToDecimal(dt_半成品_库存.Rows[0]["有效库存"]))
                            {
                                continue;
                            }
                            else
                            {
                                //递归
                                Decimal de = Convert.ToDecimal(dr["受订量a"]);
                                //半成品数量置为0
                                dr["受订量a"] = 0;
                                //查询半成品的BOM结构
                                fun_减去BOM子项数量(dr["物料编码"].ToString(), dt_层级, de);
                            }
                        }
                    }
                }
                DataView dv = new DataView(dt_层级);
                dv.RowFilter = "物料类型 = '原材料'";
                //dtreturn.Merge(dv.ToTable(), true, MissingSchemaAction.Ignore); //这边有问题 这边业务逻辑会出现重复物料 merge不会合并并且数量求和 改为用下面方法
                //2018-10-15
                foreach (DataRow dr in dv.ToTable().Rows)
                {
                    DataRow[] r = dtreturn.Select(string.Format("物料编码='{0}'", dr["物料编码"].ToString()));
                    if (r.Length > 0)
                    { r[0]["受订量a"] = Convert.ToDecimal(r[0]["受订量a"]) + Convert.ToDecimal(dr["受订量a"]); }
                    else
                    { dtreturn.ImportRow(dr); }
                }

                sql = string.Format(@"select 物料编码,-sum(实效数量)as 季度用量  from 仓库出入库明细表 where  出库入库='出库' and  出入库时间>'{1}' and 
                             出入库时间<'{0}'  group by 物料编码", dtime, dtime1);
                DataTable dt4 = new DataTable();
                //dt4 = WSAdapter.webservers_getdata.wsfun.GetData_ERP(sql);
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt4);

                sql = string.Format(@"select 物料编码,-sum(实效数量)as 上月用量  from 仓库出入库明细表 where  出库入库='出库' and  出入库时间>'{1}' and 
                             出入库时间<'{0}'  group by 物料编码", dtime, dtime2);
                DataTable dt5 = new DataTable();
                //dt5 = WSAdapter.webservers_getdata.wsfun.GetData_ERP(sql);
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt5);

                sql = string.Format(@"select 物料编码,-sum(实效数量)as 半年用量  from 仓库出入库明细表 where  出库入库='出库' and  出入库时间>'{1}' and 
                             出入库时间<'{0}'  group by 物料编码", dtime, dtime3);
                DataTable dtx = new DataTable();
                // dtx = WSAdapter.webservers_getdata.wsfun.GetData_ERP(sql);
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtx);

                //明细完成日期 is null 是为了处理  检验时有部分 单子 不合格 也没有 入库评审单 ，不把数量 统计进 在途 和 送检数量
                sql = string.Format(@"select 物料编码,SUM(送检数量) as 送检数量 from (            
select 采购记录采购送检单明细表.物料编码,送检数量 from 采购记录采购送检单明细表 
left join 采购记录采购单明细表 on 采购记录采购单明细表.采购明细号 = 采购记录采购送检单明细表.采购单明细号 
where  采购记录采购单明细表.明细完成日期 is null and 检验完成 = 0 and 采购记录采购送检单明细表.作废 = 0 and 采购记录采购单明细表.作废 = 0 and 采购记录采购送检单明细表.生效日期 >= '{0}' 
union
select 产品编号 as 物料编码,采购记录采购单检验主表.送检数量 from 采购记录采购单检验主表
left join 采购记录采购单明细表 on 采购记录采购单明细表.采购明细号 = 采购记录采购单检验主表.采购明细号 and 采购记录采购单明细表.作废 = 0 
left join 采购记录采购送检单主表  on 采购记录采购送检单主表.送检单号=采购记录采购单检验主表.送检单号
where 采购记录采购单明细表.明细完成日期 is null and 关闭 = 0 and 入库完成 = 0 and 检验日期 >= '{0}' and (检验结果='合格' or  采购已处理=0)) a
group by 物料编码", time);
                DataTable dt6 = new DataTable();
                // dt6 = WSAdapter.webservers_getdata.wsfun.GetData_ERP(sql);
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt6);

                MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
                //aa//dtreturn = RBQ.SelectGroupByInto("", dtreturn, "物料编码,原ERP物料编号,物料名称,大类,n原ERP规格型号,库存下限,规格型号,图纸编号,仓库号,仓库名称,供应商编号,默认供应商,计量单位,库存总数,有效总数,在途量,在制量,受订量,未领量,sum(受订量a) 受订量a", "", "物料编码,原ERP物料编号,库存总数,有效总数,在途量,在制量,受订量,未领量,库存下限,规格型号,图纸编号,仓库号,仓库名称,供应商编号,默认供应商,计量单位");//,季度用量,上月用量
                dtreturn = RBQ.SelectGroupByInto("", dtreturn, @"物料编码,原ERP物料编号,物料名称,大类,n原ERP规格型号,库存下限,规格型号,图纸编号,仓库号,仓库名称,供应商编号
                                                    ,默认供应商,采购供应商备注,计量单位编码,计量单位,库存总数,有效总数,在途量,在制量,受订量,未领量,sum(受订量a) 受订量a,停用,有无蓝图,新数据 ", "物料类型 = '原材料'",
                                                     "物料编码,原ERP物料编号,库存总数,有效总数,在途量,在制量,受订量,未领量,库存下限,规格型号,图纸编号,仓库号,仓库名称,供应商编号,默认供应商,采购供应商备注,计量单位编码,计量单位,停用,有无蓝图,新数据 ");//,季度用量,上月用量
                dtreturn.Columns.Add("季度用量", typeof(Decimal));
                dtreturn.Columns.Add("上月用量", typeof(Decimal));
                dtreturn.Columns.Add("半年用量", typeof(Decimal));
                dtreturn.Columns.Add("送检数量", typeof(Decimal));
                dtreturn.Columns.Add("欠缺数量不含安全库存", typeof(Decimal));
                dtreturn.Columns.Add("欠缺数量包含安全库存", typeof(Decimal));
                foreach (DataRow dr in dtreturn.Rows)
                {
                    try
                    {
                        DataRow[] ds = dt6.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                        dr["送检数量"] = ds[0]["送检数量"];
                    }
                    catch { dr["送检数量"] = 0; }
                    try
                    {
                        DataRow[] ds = dt4.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                        dr["季度用量"] = ds[0]["季度用量"];
                    }
                    catch { dr["季度用量"] = 0; }
                    try
                    {
                        DataRow[] ds = dt5.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                        dr["上月用量"] = ds[0]["上月用量"];
                    }
                    catch { dr["上月用量"] = 0; }
                    try
                    {
                        DataRow[] ds = dtx.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                        dr["半年用量"] = ds[0]["半年用量"];
                    }
                    catch { dr["半年用量"] = 0; }
                    try
                    {
                        dr["欠缺数量不含安全库存"] = Convert.ToDecimal(dr["受订量a"]) + Convert.ToDecimal(dr["未领量"]) - Convert.ToDecimal(dr["库存总数"]) - Convert.ToDecimal(dr["在途量"]) - Convert.ToDecimal(dr["在制量"]);
                    }
                    catch { dr["欠缺数量不含安全库存"] = 0; }//+ Convert.ToDecimal(dr["受订量"])
                    try
                    {
                        dr["欠缺数量包含安全库存"] = Convert.ToDecimal(dr["受订量a"]) + Convert.ToDecimal(dr["未领量"]) - Convert.ToDecimal(dr["库存总数"]) - Convert.ToDecimal(dr["在途量"]) - Convert.ToDecimal(dr["在制量"]) + Convert.ToDecimal(dr["库存下限"]);
                    }
                    catch { dr["欠缺数量包含安全库存"] = 0; }// + Convert.ToDecimal(dr["受订量"])
                    try
                    {
                        dr["有效总数"] = -(Convert.ToDecimal(dr["受订量"]) + Convert.ToDecimal(dr["未领量"]) - Convert.ToDecimal(dr["库存总数"]) - Convert.ToDecimal(dr["在途量"]) - Convert.ToDecimal(dr["在制量"]));// + Convert.ToDecimal(dr["库存下限"])
                    }
                    catch { dr["有效总数"] = 0; }
                }

                gv.ViewCaption = string.Format("原材料信息合并完成");
                Application.DoEvents();

                sql = @"select 基础数据物料信息表.物料编码,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.物料名称,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.大类,
            基础数据物料信息表.库存下限,基础数据物料信息表.图纸编号,基础数据物料信息表.供应商编号,基础数据物料信息表.仓库号,基础数据物料信息表.仓库名称,基础数据物料信息表.计量单位编码,基础数据物料信息表.计量单位
            ,基础数据物料信息表.默认供应商,采购供应商备注,仓库物料数量表.库存总数,仓库物料数量表.有效总数,基础数据物料信息表.规格型号,仓库物料数量表.在途量,仓库物料数量表.在制量,仓库物料数量表.受订量,仓库物料数量表.未领量 
          ,新数据 from 基础数据物料信息表 
            left join 仓库物料数量表 on 基础数据物料信息表.物料编码 = 仓库物料数量表.物料编码 and  仓库物料数量表.仓库号=基础数据物料信息表.仓库号
            where 基础数据物料信息表.库存下限 >= 仓库物料数量表.库存总数 and  基础数据物料信息表.物料类型 = '原材料' and 基础数据物料信息表.停用 = 0";
                DataTable dt_安全库存 = new DataTable();
                if (str_person != "admin")
                {
                    if (dt_ls.Rows.Count > 0)
                    {
                        sql = sql + "and ( 基础数据物料信息表.供应商编号= '' or";
                        foreach (DataRow dr in dt_ls.Rows)
                        {
                            sql = sql + string.Format(" 基础数据物料信息表.供应商编号='{0}' or", dr["供应商ID"]);
                        }
                        sql = sql.Substring(0, sql.Length - 2);
                        sql = sql + ")";
                    }
                    else
                    {
                        throw new Exception("你没有对应的供应商,请找信息部核实");
                    }
                }
                // dt_安全库存 = WSAdapter.webservers_getdata.wsfun.GetData_ERP(sql);
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_安全库存);
                dt_安全库存.Columns.Add("欠缺数量包含安全库存", typeof(Decimal));
                dt_安全库存.Columns.Add("季度用量", typeof(Decimal));
                dt_安全库存.Columns.Add("上月用量", typeof(Decimal));
                dt_安全库存.Columns.Add("半年用量", typeof(Decimal));
                dt_安全库存.Columns.Add("送检数量", typeof(Decimal));
                foreach (DataRow dr in dt_安全库存.Rows)//去除重复项
                {
                    if (dr.RowState == DataRowState.Deleted) continue;
                    DataRow[] ds = dtreturn.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                    if (ds.Length > 0)
                    {
                        dr.Delete();
                    }
                    else
                    {
                        try
                        {
                            DataRow[] dss = dt6.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                            dr["送检数量"] = dss[0]["送检数量"];
                        }
                        catch { dr["送检数量"] = 0; }
                        try
                        {
                            DataRow[] dss = dt4.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                            dr["季度用量"] = dss[0]["季度用量"];
                        }
                        catch { dr["季度用量"] = 0; }
                        try
                        {
                            DataRow[] dss = dt5.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                            dr["上月用量"] = dss[0]["上月用量"];
                        }
                        catch { dr["上月用量"] = 0; }
                        try
                        {
                            DataRow[] dss = dtx.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                            dr["半年用量"] = dss[0]["半年用量"];
                        }
                        catch { dr["半年用量"] = 0; }

                        dr["欠缺数量包含安全库存"] = Convert.ToDecimal(dr["库存下限"]) - Convert.ToDecimal(dr["库存总数"]) - Convert.ToDecimal(dr["在途量"]);
                    }
                }
                dt_安全库存.AcceptChanges();
                dtreturn.Merge(dt_安全库存);//合并两个dt
                gv.ViewCaption = string.Format("处理完成");
                Application.DoEvents();

                foreach (string str in lines)
                {
                    sw.WriteLine(str);
                }

                sw.Flush();
                sw.Close();
                fs.Close();

                return dtreturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt_参数">主要为物料编码的集合，附带每个物料的数量</param>
        /// <param name="dt_层级">物料编码，数量，层级，物料类型</param>
        private static DataTable fun_BOM分层(DataTable dt_参数, DataTable dt_层级, DataTable dt_ls, string str_person)
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format(@"select 子项编码 as 物料编码,数量,库存总数,有效总数,在途量,在制量,受订量,未领量,
基础数据物料信息表.物料名称,基础数据物料信息表.大类,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.库存下限,基础数据物料信息表.原ERP物料编号,
基础数据物料信息表.规格型号,基础数据物料信息表.图纸编号,基础数据物料信息表.计量单位编码,基础数据物料信息表.计量单位,基础数据物料信息表.仓库名称,基础数据物料信息表.物料类型,
基础数据物料信息表.供应商编号,基础数据物料信息表.默认供应商,采购供应商备注,基础数据物料信息表.仓库号,基础数据物料信息表.停用,有无蓝图
                    from 基础数据物料BOM表 
                    left join 仓库物料数量表 on 仓库物料数量表.物料编码 = 基础数据物料BOM表.子项编码
                    left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 基础数据物料BOM表.子项编码 and 基础数据物料信息表.仓库号=基础数据物料BOM表.仓库号
                    where 产品编码 = '00011' and BOM类型 = '物料BOM'");
            //SqlDataAdapter da = new SqlDataAdapter(sql, strconn); 
            DataTable dt_BOM = new DataTable();
            //dt_BOM = WSAdapter.webservers_getdata.wsfun.GetData_ERP(sql);
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_BOM);
            dt_BOM.Columns.Add("层级");
            dt_BOM_备份 = dt_BOM.Clone();

            foreach (DataRow dr in dt_参数.Rows)
            {
                DataRow[] dss = dt_层级.Select(string.Format("物料编码 = '{0}'", dr["物料编码"].ToString()));
                if (dss.Length > 0)
                {
                    dss[0]["受订量a"] = Convert.ToDecimal(dss[0]["受订量a"]) + Convert.ToDecimal(dr["受订量a"]);
                }
                else
                {
                    DataRow rr = dt_层级.NewRow();
                    dt_层级.Rows.Add(rr);
                    rr["物料编码"] = dr["物料编码"];
                    rr["层级"] = 1;
                    rr["受订量a"] = dr["受订量a"];
                    rr["物料类型"] = dr["物料类型"];
                }

                //本轮'成品'物料缺的数量
                Decimal de = Convert.ToDecimal(dr["受订量a"]);
                dt_BOM.Clear(); int i = 2;
                //获取物料dr["物料编码"]的完整BOM
                dt_BOM_备份.Merge(fun_完整BOM结构(dt_BOM, dr["物料编码"].ToString(), i, dt_ls, str_person));
                //dt_BOM_备份.Merge(WSAdapter.webservers_getdata.wsfun.fun_完整BOM结构(dt_BOM, dr["物料编码"].ToString(), i, dt_ls, str_person));
                //将物料dr["物料编码"]的完整BOM拆分到dt_层级里
                foreach (DataRow r in dt_BOM.Rows)
                {
                    DataRow[] ds = dt_层级.Select(string.Format("物料编码 = '{0}'", r["物料编码"].ToString()));
                    if (ds.Length > 0)
                    {
                        ds[0]["受订量a"] = Convert.ToDecimal(ds[0]["受订量a"]) + Convert.ToDecimal(r["数量"]) * de;
                        if (Convert.ToInt32(ds[0]["层级"]) < Convert.ToInt32(r["层级"]))
                        {
                            ds[0]["层级"] = Convert.ToInt32(r["层级"]);
                        }
                    }
                    else
                    {
                        DataRow rr = dt_层级.NewRow();
                        dt_层级.Rows.Add(rr);
                        rr["物料编码"] = r["物料编码"];
                        rr["层级"] = r["层级"];
                        rr["受订量a"] = de * Convert.ToDecimal(r["数量"]);
                        rr["物料类型"] = r["物料类型"];
                        rr["库存总数"] = r["库存总数"];
                        rr["有效总数"] = r["有效总数"];
                        rr["在途量"] = r["在途量"];
                        rr["在制量"] = r["在制量"];
                        rr["受订量"] = r["受订量"];
                        rr["未领量"] = r["未领量"];
                        rr["物料名称"] = r["物料名称"];
                        rr["大类"] = r["大类"];
                        rr["n原ERP规格型号"] = r["n原ERP规格型号"];
                        rr["库存下限"] = r["库存下限"];
                        rr["原ERP物料编号"] = r["原ERP物料编号"];
                        rr["规格型号"] = r["规格型号"];
                        rr["图纸编号"] = r["图纸编号"];
                        rr["计量单位编码"] = r["计量单位编码"];

                        rr["计量单位"] = r["计量单位"];
                        rr["仓库名称"] = r["仓库名称"];
                        rr["供应商编号"] = r["供应商编号"];
                        rr["默认供应商"] = r["默认供应商"];
                        rr["采购供应商备注"] = r["采购供应商备注"];
                        rr["仓库号"] = r["仓库号"];
                        rr["停用"] = r["停用"];
                        rr["有无蓝图"] = r["有无蓝图"];

                    }
                }
            }
            return dt_层级;
        }

#pragma warning disable IDE1006 // 命名样式
        private static DataTable fun_完整BOM结构(DataTable dt_BOM, string str_产品, int i, DataTable dt_ls, string str_person)
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format(@"select 产品编码,子项编码 as 物料编码,数量,库存总数,有效总数,在途量,在制量,受订量,未领量,
base.物料名称,base.大类,base.n原ERP规格型号,base.库存下限,base.原ERP物料编号,
base.规格型号,base.图纸编号,base.计量单位编码,base.计量单位,base.仓库名称,base.物料类型,
base.供应商编号,base.默认供应商,采购供应商备注,base.仓库号,base.停用,有无蓝图
                    from 基础数据物料BOM表 
                    left join 基础数据物料信息表 base on base.物料编码 = 基础数据物料BOM表.子项编码
                    left join 仓库物料数量表 kc on kc.物料编码 = 基础数据物料BOM表.子项编码 and kc.仓库号=base.仓库号
                    
                    where 产品编码 = '{0}' and 子项类型<>'采购件'  and BOM类型 = '物料BOM'", str_产品);

            if (str_person != "admin")
            {
                if (dt_ls.Rows.Count > 0)
                {
                    sql = sql + "and ( base.供应商编号= '' or";
                    foreach (DataRow dr in dt_ls.Rows)
                    {
                        sql = sql + string.Format(" base.供应商编号='{0}' or", dr["供应商ID"]);
                    }
                    sql = sql.Substring(0, sql.Length - 2);
                    sql = sql + ")";
                }
                else
                {
                    throw new Exception("你没有对应的供应商,请找信息部核实");
                }
            }

            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            DataTable dt_单层BOM = new DataTable();
            // dt_单层BOM = WSAdapter.webservers_getdata.wsfun.GetData_ERP(sql);

            da.Fill(dt_单层BOM);
            dt_单层BOM.Columns.Add("层级");

            if (dt_单层BOM.Rows.Count > 0)
            {
                foreach (DataRow dr in dt_单层BOM.Rows)
                {
                    dr["层级"] = i;
                    fun_完整BOM结构(dt_BOM, dr["物料编码"].ToString(), i + 1, dt_ls, str_person);
                }
                dt_BOM.Merge(dt_单层BOM);
            }
            return dt_BOM;
        }

        private static List<string> lines = new List<string>();
#pragma warning disable IDE1006 // 命名样式
        private static void fun_减去BOM子项数量(string str_物料编码, DataTable dt_层级, Decimal de_产品编码数量)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow[] ds = dt_BOM_备份.Select(string.Format("产品编码 = '{0}'", str_物料编码));
            if (ds.Length <= 0) return;
            //每个子项减去 半成品的数量 * BOM数量
            foreach (DataRow dr_子 in ds)
            {
                DataRow[] dss = dt_层级.Select(string.Format("物料编码 = '{0}'", dr_子["物料编码"]));
                if (dss.Length > 0)
                {
                    foreach (DataRow dr_子子 in dss)
                    {
                        dr_子子["受订量a"] = Convert.ToDecimal(dr_子子["受订量a"]) - de_产品编码数量 * Convert.ToDecimal(dr_子["数量"]);
                        string line = "半成品:" + str_物料编码 + ",子项：" + dr_子子["物料编码"].ToString() + ",数量(半成品数量 * BOM数量)：" + dr_子["数量"].ToString() + " * " + de_产品编码数量.ToString();
                        lines.Add(line);
                        //小于0则置为0
                        if (Convert.ToDecimal(dr_子子["受订量a"]) < 0)
                        {
                            dr_子子["受订量a"] = 0;
                        }
                        //递归该子项编码
                        fun_减去BOM子项数量(dr_子子["物料编码"].ToString(), dt_层级, de_产品编码数量);
                    }
                }

            }
        }
        #endregion


        #region 2018-6-19
#pragma warning disable IDE1006 // 命名样式
        public static DataSet fun_lld(DataSet ds, DataTable drr, string s_id, string s_name, string s_llrID, string s_llrname, string s_gdfzrID, string s_gdfzrname, string str_待领料单号, DataTable dt_库存)
#pragma warning restore IDE1006 // 命名样式
        {

            ds.Tables[0].TableName = "ds0";
            ds.Tables[1].TableName = "ds1";
            ds.Tables[2].TableName = "list_原料刷新";


            DateTime t = System.DateTime.Now;
            string strconn = CPublic.Var.strConn;


            string sql_主表 = "select * from 生产记录生产工单待领料主表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_主表, strconn))
            {
                if (ds.Tables[0].Columns.Count == 0)
                {
                    da.Fill(ds.Tables[0]);
                }
                DataRow dr = ds.Tables[0].NewRow();
                dr["待领料单号"] = str_待领料单号;
                dr["领料类型"] = "工单领料";
                dr["生产工单号"] = drr.Rows[0]["生产工单号"];
                dr["生产制令单号"] = drr.Rows[0]["生产制令单号"];
                dr["生产工单类型"] = drr.Rows[0]["生产工单类型"];

                dr["生产车间"] = drr.Rows[0]["生产车间"];   //已变成车间编号

                dr["产品编码"] = drr.Rows[0]["物料编码"];
                dr["产品名称"] = drr.Rows[0]["物料名称"];

                dr["领料人ID"] = s_llrID;
                dr["领料人"] = s_llrname;
                dr["规格型号"] = drr.Rows[0]["规格型号"];
                //dr["原规格型号"] = drr.Rows[0]["原规格型号"];
                dr["图纸编号"] = drr.Rows[0]["图纸编号"];
                dr["生产数量"] = Convert.ToDecimal(drr.Rows[0]["生产数量"]);

                dr["创建日期"] = t;
                dr["加急状态"] = drr.Rows[0]["加急状态"];
                dr["制单人员"] = s_name;
                dr["制单人员ID"] = s_id;
                dr["工单负责人"] = s_gdfzrname;
                dr["工单负责人ID"] = s_gdfzrID;
                ds.Tables[0].Rows.Add(dr);

            }
            //保存待领料主表

            //保存 待领料单明细表
            string sql_明细 = "select * from 生产记录生产工单待领料明细表 where 1<>1";
            string sql_BOM = string.Format(@"select bom.*,base.规格型号 from 基础数据物料BOM表 bom left join  基础数据物料信息表 base
                                             on bom.子项编码=base.物料编码  
                                             where bom.产品编码='{0}' and  BOM类型='物料BOM' and  bom.主辅料='主料' and 优先级=1", drr.Rows[0]["物料编码"].ToString().Trim());
            using (SqlDataAdapter da = new SqlDataAdapter(sql_BOM, strconn))
            {
                DataTable dt_bom = new DataTable();
                da.Fill(dt_bom);

                da.Fill(ds.Tables[2]);

                using (SqlDataAdapter da1 = new SqlDataAdapter(sql_明细, strconn))
                {
                    if (ds.Tables[1].Columns.Count == 0)
                    {
                        da1.Fill(ds.Tables[1]);
                    }
                    int pos = 0;
                    //先判断基础表中 改产品有无替代料
                    //没有就走原来代码 

                    //   
                    string sql = string.Format("select * from 基础数据物料信息表 where 物料编码='{0}'", drr.Rows[0]["物料编码"]);
                    DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt.Rows[0]["BOM有无备料"].Equals(true))
                    {
                        foreach (DataRow r in dt_bom.Rows)  //dt_bom只取优先级为1的 ，若不够 再取 同组 物料判断
                        {
                            decimal dec_总需数 = Convert.ToDecimal(drr.Rows[0]["生产数量"]) * Convert.ToDecimal(r["数量"]);
                            /*
                            string sql_库存 = string.Format("select * from 仓库物料数量表 where 物料编码='{0}'", r["子项编码"]);
                            DataTable tttttt = GetData_ERP(sql_库存);
                            DataRow r_库存 = tttttt.Rows[0];
                            */
                            DataRow r_库存 = dt_库存.Select(string.Format("物料编码='{0}'", r["子项编码"].ToString()))[0];

                            decimal dec_剩 = dec_总需数 - Convert.ToDecimal(r_库存["库存总数"]);

                            if (dec_剩 <= 0 || r["组"].ToString() == "")   //就 取这条
                            {
                                DataRow dr = ds.Tables[1].NewRow();
                                dr["待领料单号"] = str_待领料单号;
                                dr["待领料单明细号"] = str_待领料单号 + pos.ToString("00");
                                dr["生产工单号"] = drr.Rows[0]["生产工单号"];
                                dr["生产制令单号"] = drr.Rows[0]["生产制令单号"];
                                dr["生产工单类型"] = drr.Rows[0]["生产工单类型"];
                                dr["生产车间"] = drr.Rows[0]["生产车间"]; //车间编号
                                dr["A面位号"] = r["A面位号"];
                                dr["B面位号"] = r["B面位号"];
                                //东屋
                                dr["仓库号"] = r["仓库号"];
                                dr["仓库名称"] = r["仓库名称"];

                                dr["物料编码"] = r["子项编码"];
                                dr["物料名称"] = r["子项名称"];
                                dr["规格型号"] = r["规格型号"].ToString().Trim();
                                try
                                {
                                    dr["待领料总量"] = dec_总需数;
                                    dr["未领数量"] = dr["待领料总量"];
                                    dr["BOM数量"] = Convert.ToDecimal(r["数量"]);

                                }
                                catch (Exception)
                                {
                                    throw new Exception(string.Format("物料{0}BOM数量有问题 速找开发部确认", r["产品编码"]));

                                }
                                dr["创建日期"] = t;
                                dr["修改日期"] = t;
                                dr["制单人员"] = s_name;
                                dr["制单人员ID"] = s_id;

                                dr["工单负责人"] = s_gdfzrname;
                                dr["工单负责人ID"] = s_gdfzrID;
                                dr["领料人ID"] = s_llrID;
                                dr["领料人"] = s_llrname;
                                pos++;
                                ds.Tables[1].Rows.Add(dr);
                                r_库存["库存总数"] = Convert.ToDecimal(r_库存["库存总数"]) - dec_总需数;
                                continue;
                            }
                            else
                            {
                                //取同组 优先级不为1 的 其他几种物料  先 判断 单个库存够得

                                DataTable dt_3 = new DataTable();
                                string sql_3 = string.Format(@"select 基础数据物料BOM表.*,基础数据物料信息表.n原ERP规格型号,库存总数,需要数=0,剩余数=0  from 基础数据物料BOM表 
                                left join  基础数据物料信息表 on 基础数据物料BOM表.子项编码=基础数据物料信息表.物料编码 
                                left join 仓库物料数量表 on  仓库物料数量表.物料编码=基础数据物料信息表.物料编码
                                  where 基础数据物料BOM表.产品编码='{0}' and  基础数据物料BOM表.主辅料<>'包装'and 优先级<>1 and 组='{1}' order by 优先级"
                                       , drr.Rows[0]["物料编码"].ToString(), r["组"].ToString());
                                dt_3 = CZMaster.MasterSQL.Get_DataTable(sql_3, strconn);
                                foreach (DataRow r3 in dt_3.Rows)
                                {
                                    //decimal dec = dec_总需数 - Convert.ToDecimal(r3["库存总数"]);
                                    r_库存 = ds.Tables[3].Select(string.Format("物料编码='{0}'", r["子项编码"].ToString()))[0];

                                    decimal dec = dec_总需数 - Convert.ToDecimal(r_库存["库存总数"]);
                                    if (dec <= 0)     //某一替代料库存够     就取这个物料
                                    {
                                        DataRow dr = ds.Tables[1].NewRow();
                                        dr["待领料单号"] = str_待领料单号;
                                        dr["待领料单明细号"] = str_待领料单号 + pos.ToString("00");
                                        dr["生产工单号"] = drr.Rows[0]["生产工单号"];
                                        dr["生产制令单号"] = drr.Rows[0]["生产制令单号"];
                                        dr["生产工单类型"] = drr.Rows[0]["生产工单类型"];
                                        dr["生产车间"] = drr.Rows[0]["生产车间"]; //车间编号
                                        dr["A面位号"] = r3["A面位号"];
                                        dr["B面位号"] = r3["B面位号"];
                                        dr["仓库号"] = r3["仓库号"];
                                        dr["仓库名称"] = r3["仓库名称"];
                                        dr["物料编码"] = r3["子项编码"];
                                        dr["物料名称"] = r3["子项名称"];
                                        dr["规格型号"] = r3["规格型号"].ToString().Trim();
                                        try
                                        {
                                            dr["待领料总量"] = dec_总需数;
                                            dr["未领数量"] = dr["待领料总量"];
                                            dr["BOM数量"] = Convert.ToDecimal(r3["数量"]);
                                        }
                                        catch (Exception)
                                        {
                                            throw new Exception(string.Format("物料{0}BOM数量有问题 速找开发部确认", r3["产品编码"]));

                                        }
                                        dr["创建日期"] = t;
                                        dr["修改日期"] = t;
                                        dr["制单人员"] = s_name;
                                        dr["制单人员ID"] = s_id;

                                        dr["工单负责人"] = s_gdfzrname;
                                        dr["工单负责人ID"] = s_gdfzrID;
                                        dr["领料人ID"] = s_llrID;
                                        dr["领料人"] = s_llrname;
                                        pos++;
                                        ds.Tables[1].Rows.Add(dr);
                                        r_库存["库存总数"] = Convert.ToDecimal(r_库存["库存总数"]) - dec_总需数;
                                        dec_总需数 = -1;   //此次循环 

                                        break;
                                    }
                                }


                            }

                            if (dec_总需数 >= 0)     // 几种物料 单独一个库存不够所需领取数量时 
                            {

                                DataTable dt_替代 = new DataTable();
                                dt_替代 = fun_替代料递归(dt_替代, drr.Rows[0]["物料编码"].ToString(), dec_剩, r["组"].ToString(), 2, dt_库存);

                                //原优先级为1的物料需要多少数量
                                decimal a = 0;
                                if (dt_替代.Rows.Count > 0)
                                {
                                    a = Convert.ToDecimal(dt_替代.Rows[dt_替代.Rows.Count - 1]["剩余数"]);
                                }
                                else
                                {
                                    a = dec_总需数;
                                }
                                if (Convert.ToDecimal(r_库存["库存总数"]) + a > 0)
                                {
                                    DataRow rrr = dt_替代.NewRow();
                                    rrr["产品编码"] = r["产品编码"];
                                    rrr["子项编码"] = r["子项编码"];
                                    rrr["产品名称"] = r["产品名称"];
                                    rrr["子项名称"] = r["子项名称"];
                                    rrr["A面位号"] = r["A面位号"];
                                    rrr["B面位号"] = r["B面位号"];
                                    rrr["仓库号"] = r["仓库号"];
                                    rrr["仓库名称"] = r["仓库名称"];
                                    rrr["规格型号"] = r["规格型号"];
                                    rrr["需要数"] = Convert.ToDecimal(r_库存["库存总数"]) + a;
                                    dt_替代.Rows.Add(rrr);

                                }

                                foreach (DataRow dr_替代 in dt_替代.Rows)
                                {
                                    DataRow dr_1 = ds.Tables[1].NewRow();
                                    dr_1["待领料单号"] = str_待领料单号;
                                    dr_1["待领料单明细号"] = str_待领料单号 + pos.ToString("00");
                                    dr_1["生产工单号"] = drr.Rows[0]["生产工单号"];
                                    dr_1["生产制令单号"] = drr.Rows[0]["生产制令单号"];
                                    dr_1["生产工单类型"] = drr.Rows[0]["生产工单类型"];
                                    dr_1["生产车间"] = drr.Rows[0]["生产车间"]; //车间编号
                                    dr_1["A面位号"] = dr_替代["A面位号"];
                                    dr_1["B面位号"] = dr_替代["B面位号"];
                                    dr_1["仓库号"] = dr_替代["仓库号"];
                                    dr_1["仓库名称"] = dr_替代["仓库名称"];
                                    dr_1["物料编码"] = dr_替代["子项编码"];
                                    dr_1["物料名称"] = dr_替代["子项名称"];
                                    dr_1["规格型号"] = dr_替代["规格型号"].ToString().Trim();
                                    try
                                    {
                                        dr_1["待领料总量"] = dr_替代["需要数"];
                                        dr_1["未领数量"] = dr_1["待领料总量"];
                                        dr_1["BOM数量"] = Convert.ToDecimal(r["数量"]);

                                    }
                                    catch (Exception)
                                    {
                                        throw new Exception(string.Format("物料{0}BOM数量有问题 速找开发部确认", r["产品编码"]));

                                    }
                                    dr_1["创建日期"] = t;
                                    dr_1["修改日期"] = t;
                                    dr_1["制单人员"] = s_name;
                                    dr_1["制单人员ID"] = s_id;

                                    dr_1["工单负责人"] = s_gdfzrname;
                                    dr_1["工单负责人ID"] = s_gdfzrID;
                                    dr_1["领料人ID"] = s_llrID;
                                    dr_1["领料人"] = s_llrname;
                                    pos++;
                                    ds.Tables[1].Rows.Add(dr_1);
                                }
                            }



                        }
                    }
                    else      //BOM无备料 原来的流程
                    {

                        foreach (DataRow r in dt_bom.Rows)
                        {
                            DataRow dr = ds.Tables[1].NewRow();
                            dr["待领料单号"] = str_待领料单号;
                            dr["待领料单明细号"] = str_待领料单号 + pos.ToString("00");
                            dr["生产工单号"] = drr.Rows[0]["生产工单号"];
                            dr["生产制令单号"] = drr.Rows[0]["生产制令单号"];
                            dr["生产工单类型"] = drr.Rows[0]["生产工单类型"];
                            dr["生产车间"] = drr.Rows[0]["生产车间"]; //车间编号
                            dr["A面位号"] = r["A面位号"];
                            dr["B面位号"] = r["B面位号"];
                            dr["仓库号"] = r["仓库号"];
                            dr["仓库名称"] = r["仓库名称"];
                            dr["物料编码"] = r["子项编码"];
                            dr["物料名称"] = r["子项名称"];
                            dr["规格型号"] = r["规格型号"].ToString().Trim();
                            try
                            {
                                dr["待领料总量"] = Convert.ToDecimal(drr.Rows[0]["生产数量"]) * Convert.ToDecimal(r["数量"]);
                                dr["未领数量"] = dr["待领料总量"];

                                dr["BOM数量"] = Convert.ToDecimal(r["数量"]);

                            }
                            catch (Exception)
                            {
                                throw new Exception(string.Format("物料{0}BOM数量有问题 速找开发部确认", r["产品编码"]));

                            }
                            dr["创建日期"] = t;
                            dr["修改日期"] = t;
                            dr["制单人员"] = s_name;
                            dr["制单人员ID"] = s_id;

                            dr["工单负责人"] = s_gdfzrname;
                            dr["工单负责人ID"] = s_gdfzrID;
                            dr["领料人ID"] = s_llrID;
                            dr["领料人"] = s_llrname;
                            pos++;
                            ds.Tables[1].Rows.Add(dr);
                            DataRow rr_kc = ds.Tables[3].Select(string.Format("物料编码='{0}'", r["子项编码"].ToString()))[0];
                            decimal d = Convert.ToDecimal(rr_kc["库存总数"]) - Convert.ToDecimal(dr["待领料总量"]);
                            rr_kc["库存总数"] = d >= 0 ? d : 0;

                        }

                    }
                }
            }


            return (ds);



        }
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 2018-6-19 新增一个参数  dt_库存
        /// </summary>
        /// <param name="dt_传递"></param>
        /// <param name="str_产品"></param>
        /// <param name="dec_总需"></param>
        /// <param name="str_组"></param>
        /// <param name="i_顺序"></param>
        /// <returns></returns>
        private static DataTable fun_替代料递归(DataTable dt_传递, string str_产品, decimal dec_总需, string str_组, int i_顺序, DataTable dt_库存)  //取替代料 
#pragma warning restore IDE1006 // 命名样式
        {


            DataTable dt = new DataTable();
            string sql = string.Format(@"select 基础数据物料BOM表.*,基础数据物料信息表.规格型号,需要数=0,剩余数=0  from 基础数据物料BOM表 
                                left join  基础数据物料信息表 on 基础数据物料BOM表.子项编码=基础数据物料信息表.物料编码 
                                
            where 基础数据物料BOM表.产品编码='{0}' and  基础数据物料BOM表.主辅料='主料'and 优先级='{1}' and 组='{2}' ", str_产品, i_顺序, str_组); //这里是按照顺序 一个一个取的替代料
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);


            if (dt.Rows.Count > 0)
            {
                if (dt_传递.Columns.Count == 0)
                {
                    dt_传递 = dt.Clone();
                }
                if (dec_总需 > 0)
                {
                    DataRow r_库存 = dt_库存.Select(string.Format("物料编码='{0}'", dt.Rows[0]["子项编码"].ToString()))[0];
                    // if (dec_总需 <= Convert.ToDecimal(dt.Rows[0]["库存总数"]))   //顺序为i的替代料 库存够

                    if (dec_总需 <= Convert.ToDecimal(r_库存["库存总数"]))   //顺序为i的替代料 库存够
                    {
                        dt.Rows[0]["需要数"] = dec_总需;
                        dt.Rows[0]["剩余数"] = 0;

                        r_库存["库存总数"] = Convert.ToDecimal(r_库存["库存总数"]) - dec_总需;

                        dt_传递.ImportRow(dt.Rows[0]);
                        dec_总需 = 0;
                        return dt;
                    }
                    //else if (Convert.ToDecimal(dt.Rows[0]["库存总数"]) > 0)        //有替代料 但仍然不够

                    else if (Convert.ToDecimal(r_库存["库存总数"]) > 0)        //有替代料 但仍然不够
                    {
                        dt.Rows[0]["需要数"] = Convert.ToDecimal(r_库存["库存总数"]);

                        dec_总需 = dec_总需 - Convert.ToDecimal(r_库存["库存总数"]);
                        dt.Rows[0]["剩余数"] = dec_总需;
                        dt_传递.ImportRow(dt.Rows[0]);
                        r_库存["库存总数"] = 0;
                        dt_传递 = fun_替代料递归(dt_传递, str_产品, dec_总需, str_组, ++i_顺序, dt_库存);

                    }
                }


            }
            return dt_传递;

        }



        #endregion
        //19-11-21
        /// <summary>
        /// dt_生效记录,需要生产工单的记录
        /// 需要物料编码列 
        /// </summary>
        public static DataTable KCRecord(DataTable dt_生效记录)
        {
            string kc_s = "select  * from 仓库物料数量表 where 1=2";
            DataTable dt_kc = CZMaster.MasterSQL.Get_DataTable(kc_s, strconn);
            foreach (DataRow rr in dt_生效记录.Rows)
            {
                kc_s = string.Format(@"with temp_bom(产品编码,子项编码,仓库号,仓库名称,wiptype,子项类型,数量,bom类型,优先级,bom_level ) as
    (select  产品编码,子项编码,仓库号,仓库名称,WIPType,子项类型,数量,bom类型,优先级,1 as level from 基础数据物料BOM表   
     where 产品编码='{0}'
    union all 
   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型, convert(decimal(18, 6),a.数量*b.数量) as 数量,
    a.bom类型,a.优先级,b.bom_level+1  from 基础数据物料BOM表 a
     inner join temp_bom b on a.产品编码=b.子项编码 where b.wiptype='虚拟') 
   select   子项编码,base.物料名称 as 子项名称,base.规格型号 as 子项规格,wiptype,子项类型,
   sum(数量)数量,bom类型,a.仓库号,a.仓库名称, bom_level,base.计量单位,base.计量单位编码,库存总数 from  temp_bom a
   left join 基础数据物料信息表 base  on base.物料编码=a.子项编码
  left join 仓库物料数量表 kc on kc.物料编码=a.子项编码 and kc.仓库号=a.仓库号
  where wiptype<>'虚拟'  and 优先级=1  and 库存总数 is null 
  group by   子项编码,base.物料名称 ,wiptype,子项类型 ,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号
   ,base.计量单位,base.计量单位编码,库存总数", rr["物料编码"]);
                DataTable kc_temp = CZMaster.MasterSQL.Get_DataTable(kc_s, strconn);
                if (kc_temp.Rows.Count > 0)
                {
                    foreach (DataRow r_kc in kc_temp.Rows)
                    {
                        DataRow[] rr_kc = dt_kc.Select(string.Format("物料编码='{0}'", r_kc["子项编码"]));
                        if (rr_kc.Length == 0)
                        {
                            DataRow addRow = dt_kc.NewRow();
                            addRow["GUID"] = System.Guid.NewGuid();
                            addRow["物料名称"] = r_kc["子项名称"];
                            addRow["物料编码"] = r_kc["子项编码"];
                            addRow["规格型号"] = r_kc["子项规格"];
                            addRow["仓库号"] = r_kc["仓库号"];
                            addRow["仓库名称"] = r_kc["仓库名称"];
                            dt_kc.Rows.Add(addRow);
                        }
                    }
                }

            }
            return dt_kc;
        }


        public static DataSet fun_lld(DataSet ds, DataTable drr, string s_id, string s_name, string s_llrID, string s_llrname, string s_gdfzrID, string s_gdfzrname, string str_待领料单号)
        {
            ds.Tables[0].TableName = "ds0";
            ds.Tables[1].TableName = "ds1";
            ds.Tables[2].TableName = "list_原料刷新";
            ds.Tables[3].TableName = "list_库存缓存";
            DateTime t = CPublic.Var.getDatetime();

            string sql_主表 = "select * from 生产记录生产工单待领料主表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_主表, strconn))
            {
                if (ds.Tables[0].Columns.Count == 0)
                {
                    da.Fill(ds.Tables[0]);
                }
                DataRow dr = ds.Tables[0].NewRow();
                dr["待领料单号"] = str_待领料单号;
                dr["领料类型"] = "工单领料";
                dr["生产工单号"] = drr.Rows[0]["生产工单号"];
                dr["生产制令单号"] = drr.Rows[0]["生产制令单号"];
                dr["生产工单类型"] = drr.Rows[0]["生产工单类型"];
                dr["生产车间"] = drr.Rows[0]["生产车间"];   //已变成车间编号
                dr["产品编码"] = drr.Rows[0]["物料编码"];
                dr["产品名称"] = drr.Rows[0]["物料名称"];
                dr["领料人ID"] = s_llrID;
                dr["领料人"] = s_llrname;
                dr["规格型号"] = drr.Rows[0]["规格型号"];
                //dr["原规格型号"] = drr.Rows[0]["原规格型号"];
                dr["图纸编号"] = drr.Rows[0]["图纸编号"];
                dr["生产数量"] = Convert.ToDecimal(drr.Rows[0]["生产数量"]);
                dr["创建日期"] = t;
                dr["加急状态"] = drr.Rows[0]["加急状态"];
                dr["制单人员"] = s_name;
                dr["制单人员ID"] = s_id;
                dr["工单负责人"] = s_gdfzrname;
                dr["工单负责人ID"] = s_gdfzrID;
                ds.Tables[0].Rows.Add(dr);
            }
            //保存待领料主表

            //保存 待领料单明细表
            string sql_明细 = "select * from 生产记录生产工单待领料明细表 where 1<>1";
            //未来电器 
            //            string sql_BOM = string.Format(@"select 基础数据物料BOM表.*,基础数据物料信息表.n原ERP规格型号 from 基础数据物料BOM表 left join  基础数据物料信息表
            //                                             on 基础数据物料BOM表.子项编码=基础数据物料信息表.物料编码  
            //                                             where 基础数据物料BOM表.产品编码='{0}' and  基础数据物料BOM表.主辅料='主料' and 优先级=1", drr.Rows[0]["物料编码"].ToString().Trim());
            string sql_BOM = string.Format(@"select bom.*,base.规格型号 from 基础数据物料BOM表 bom left join  基础数据物料信息表 base  on bom.子项编码=base.物料编码  
               where bom.产品编码='{0}' and  BOM类型='物料BOM' and WIPType in ('领料','虚拟') and  bom.主辅料='主料' and 优先级=1", drr.Rows[0]["物料编码"].ToString().Trim());
            using (SqlDataAdapter da = new SqlDataAdapter(sql_BOM, strconn))
            {
                DataTable dt_bom = new DataTable();
                da.Fill(dt_bom);
                da.Fill(ds.Tables[2]);
                using (SqlDataAdapter da1 = new SqlDataAdapter(sql_明细, strconn))
                {
                    if (ds.Tables[1].Columns.Count == 0)
                    {
                        da1.Fill(ds.Tables[1]);
                    }
                    int pos = 0;
                    //先判断基础表中 该产品有无替代料
                    //没有就走原来代码 
                    //   
                    string sql = string.Format("select * from 基础数据物料信息表 where 物料编码='{0}'", drr.Rows[0]["物料编码"]);
                    DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt.Rows[0]["BOM有无备料"].Equals(true))
                    {
                        foreach (DataRow r in dt_bom.Rows)  //dt_bom只取优先级为1的 ，若不够 再取 同组 物料判断
                        {
                            //设定未 虚拟件子项下不允许有替代料 所以只有一层 不需递归;需要在ds.Tables[3]中添加这些子项的库存信息
                            //19-4-8 虚拟件底下可能还有虚拟件
                            if (r["WIPType"].ToString().Trim() == "虚拟")
                            {
                                string x = string.Format(@"select 产品编码,a.物料编码,库存总数,有效总数,b.计量单位 as bom单位,单位换算标识,单位换算标识 from 仓库物料数量表 a
                                left  join 基础数据物料信息表 base on base.物料编码=a.物料编码
                                Left  join 基础数据物料BOM表 b on a.物料编码=b.子项编码 and a.仓库号=b.仓库号 where   产品编码='{0}'", r["子项编码"]);
                                DataTable temp_stock = CZMaster.MasterSQL.Get_DataTable(x, strconn);
                                DataRow[] rrr = ds.Tables[3].Select(string.Format("产品编码='{0}'", r["子项编码"].ToString()));
                                if (rrr.Length == 0) //该成品未加载过
                                {
                                    foreach (DataRow dr_stock in temp_stock.Rows)
                                    {
                                        if (ds.Tables[3].Select(string.Format("物料编码='{0}'", dr_stock["物料编码"].ToString())).Length == 0) //dt_MIcach 里先找有没有  没有就添进去
                                        {
                                            ds.Tables[3].ImportRow(dr_stock);
                                            if (dr_stock["单位换算标识"].Equals(true)) //
                                            {
                                                string ss = string.Format("select  * from 计量单位换算表 where 物料编码='{0}'", dr_stock["物料编码"]);
                                                using (SqlDataAdapter a = new SqlDataAdapter(ss, strconn))
                                                {
                                                    DataTable dt_jl = new DataTable();
                                                    da.Fill(dt_jl);
                                                    DataRow[] r1 = dt_jl.Select(string.Format("计量单位='{0}'", dr_stock["bom单位"].ToString().Trim()));
                                                    DataRow[] r2 = dt_jl.Select(string.Format("计量单位='{0}'", dr_stock["库存单位"].ToString().Trim()));
                                                    decimal dec = Convert.ToDecimal(r2[0]["换算率"]) / Convert.ToDecimal(r1[0]["换算率"]);
                                                    //DataRow []rr=  dt.Select(string.Format("计量单位='{0}'", dr["库存单位"].ToString().Trim()));
                                                    dr_stock["有效总数"] = dec * Convert.ToDecimal(dr_stock["有效总数"]);
                                                    dr_stock["库存总数"] = dec * Convert.ToDecimal(dr_stock["库存总数"]);
                                                }
                                            }
                                        }

                                    }
                                }
                                string s = string.Format(@" with temp_bom(产品编码,子项编码,仓库号,仓库名称,wiptype,子项类型,数量,bom类型,优先级,bom_level ) as
 (select  产品编码,子项编码,仓库号,仓库名称,WIPType,子项类型,数量,bom类型,优先级,1 as level from 基础数据物料BOM表    where 产品编码='{0}'
   union all 
   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型, convert(decimal(18, 6),a.数量*b.数量) as 数量,a.bom类型,a.优先级,b.bom_level+1  from 基础数据物料BOM表 a
   inner join temp_bom b on a.产品编码=b.子项编码 where b.wiptype='虚拟'  
   ) 
      select  产品编码,fx.物料名称 as  产品名称,子项编码,base.物料名称 as 子项名称,wiptype,子项类型,sum(数量)数量,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号  
      ,base.计量单位,base.计量单位编码 from  temp_bom a
  left join 基础数据物料信息表 base  on base.物料编码=a.子项编码
  left join 基础数据物料信息表  fx  on fx.物料编码=a.产品编码 where wiptype not in('入库倒冲','虚拟') and 优先级=1
  group by 产品编码,fx.物料名称 ,子项编码,base.物料名称 ,wiptype,子项类型 ,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号
  ,base.计量单位,base.计量单位编码", r["子项编码"]);
                                DataTable temp_virtual = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                                foreach (DataRow vr in temp_virtual.Rows)
                                {
                                    DataRow vdr = ds.Tables[1].NewRow();
                                    vdr["待领料单号"] = str_待领料单号;
                                    vdr["待领料单明细号"] = str_待领料单号 + "-" + pos.ToString("00");
                                    vdr["生产工单号"] = drr.Rows[0]["生产工单号"];
                                    vdr["生产制令单号"] = drr.Rows[0]["生产制令单号"];
                                    vdr["生产工单类型"] = drr.Rows[0]["生产工单类型"];
                                    vdr["生产车间"] = drr.Rows[0]["生产车间"]; //车间编号
                                    vdr["wiptype"] = "虚拟件子件"; ;
                                    //vdr["B面位号"] = vr["B面位号"];
                                    vdr["仓库号"] = vr["仓库号"];
                                    vdr["仓库名称"] = vr["仓库名称"];
                                    vdr["计量单位编号"] = vr["计量单位编码"];
                                    vdr["计量单位"] = vr["计量单位"];
                                    vdr["物料编码"] = vr["子项编码"];
                                    vdr["物料名称"] = vr["子项名称"];
                                    vdr["规格型号"] = vr["规格型号"].ToString().Trim();
                                    try
                                    {
                                        vdr["待领料总量"] = Convert.ToDecimal(drr.Rows[0]["生产数量"]) * Convert.ToDecimal(r["数量"]) * Convert.ToDecimal(vr["数量"]);
                                        vdr["未领数量"] = vdr["待领料总量"];
                                        //vdr["BOM数量"] = Convert.ToDecimal(vr["数量"]);

                                        vdr["BOM数量"] = Convert.ToDecimal(r["数量"]) * Convert.ToDecimal(vr["数量"]);
                                    }
                                    catch (Exception)
                                    {
                                        throw new Exception(string.Format("物料{0}BOM数量有问题 速找开发部确认", r["产品编码"]));
                                    }
                                    vdr["创建日期"] = t;
                                    vdr["修改日期"] = t;
                                    vdr["制单人员"] = s_name;
                                    vdr["制单人员ID"] = s_id;

                                    vdr["工单负责人"] = s_gdfzrname;
                                    vdr["工单负责人ID"] = s_gdfzrID;
                                    vdr["领料人ID"] = s_llrID;
                                    vdr["领料人"] = s_llrname;
                                    pos++;
                                    ds.Tables[1].Rows.Add(vdr);
                                    DataRow rvr_kc = ds.Tables[3].Select(string.Format("物料编码='{0}'", vr["子项编码"].ToString()))[0];
                                    //decimal d = Convert.ToDecimal(rr_kc["库存总数"]) - Convert.ToDecimal(dr["待领料总量"]); //2018-9-18 改判断库存总数为有效库存
                                    // rr_kc["库存总数"]  = d>=0?d:0;
                                    decimal vd = Convert.ToDecimal(rvr_kc["有效总数"]) - Convert.ToDecimal(vdr["待领料总量"]);
                                    rvr_kc["有效总数"] = vd >= 0 ? vd : 0;

                                }

                                continue;
                            }

                            decimal dec_总需数 = Convert.ToDecimal(drr.Rows[0]["生产数量"]) * Convert.ToDecimal(r["数量"]);

                            DataRow r_库存 = ds.Tables[3].Select(string.Format("物料编码='{0}'", r["子项编码"].ToString()))[0];

                            // decimal dec_剩 = dec_总需数 - Convert.ToDecimal(r_库存["库存总数"]); 2018-9-18 改库存总数为有效库存
                            decimal dec_剩 = dec_总需数 - Convert.ToDecimal(r_库存["有效总数"]);


                            if (dec_剩 <= 0 || r["组"].ToString() == "")   //库存够 后者没有替代料 就 取这条
                            {
                                DataRow dr = ds.Tables[1].NewRow();
                                dr["待领料单号"] = str_待领料单号;
                                dr["待领料单明细号"] = str_待领料单号 + "-" + pos.ToString("00");
                                dr["生产工单号"] = drr.Rows[0]["生产工单号"];
                                dr["生产制令单号"] = drr.Rows[0]["生产制令单号"];
                                dr["生产工单类型"] = drr.Rows[0]["生产工单类型"];
                                dr["生产车间"] = drr.Rows[0]["生产车间"]; //车间编号
                                dr["A面位号"] = r["A面位号"];
                                dr["B面位号"] = r["B面位号"];
                                dr["wiptype"] = r["wiptype"];

                                //东屋
                                dr["仓库号"] = r["仓库号"];
                                dr["仓库名称"] = r["仓库名称"];
                                dr["计量单位编号"] = r["计量单位编码"];
                                dr["计量单位"] = r["计量单位"];


                                //

                                dr["物料编码"] = r["子项编码"];
                                dr["物料名称"] = r["子项名称"];
                                dr["规格型号"] = r["规格型号"].ToString().Trim(); //东屋

                                //dr["规格型号"] = r["n原ERP规格型号"].ToString().Trim(); 未来 
                                try
                                {
                                    dr["待领料总量"] = dec_总需数;
                                    dr["未领数量"] = dr["待领料总量"];
                                    dr["BOM数量"] = Convert.ToDecimal(r["数量"]);

                                }
                                catch (Exception)
                                {
                                    throw new Exception(string.Format("物料{0}BOM数量有问题 速找开发部确认", r["产品编码"]));
                                }
                                dr["创建日期"] = t;
                                dr["修改日期"] = t;
                                dr["制单人员"] = s_name;
                                dr["制单人员ID"] = s_id;
                                dr["工单负责人"] = s_gdfzrname;
                                dr["工单负责人ID"] = s_gdfzrID;
                                dr["领料人ID"] = s_llrID;
                                dr["领料人"] = s_llrname;
                                pos++;
                                ds.Tables[1].Rows.Add(dr);
                                //  r_库存["库存总数"] = Convert.ToDecimal(r_库存["库存总数"]) - dec_总需数; 2018-9-18 改判断库存总数为有效总数
                                r_库存["有效总数"] = Convert.ToDecimal(r_库存["有效总数"]) - dec_总需数;

                                continue;
                            }
                            else
                            {
                                //取同组 优先级不为1 的 其他几种物料  先 判断 单个库存够得

                                DataTable dt_3 = new DataTable();
                                string sql_3 = string.Format(@"select bom.*,base.规格型号,库存总数,有效总数,需要数=0,剩余数=0,bom.计量单位 as bom单位,kc.计量单位 as 库存单位  from 基础数据物料BOM表 bom
                    left join  基础数据物料信息表 base on bom.子项编码=base.物料编码   left join 仓库物料数量表 kc on  kc.物料编码=base.物料编码 and kc.仓库号=bom.仓库号
                    where bom.产品编码='{0}' and  bom.主辅料<>'包装'and 优先级<>1 and 组='{1}' order by 优先级", drr.Rows[0]["物料编码"].ToString(), r["组"].ToString());
                                dt_3 = CZMaster.MasterSQL.Get_DataTable(sql_3, strconn);
                                DataRow[] xx = dt_3.Select(string.Format("单位换算标识=true"));
                                if (xx.Length > 0)
                                {
                                    foreach (DataRow dr in xx)
                                    {
                                        string ss = string.Format("select  * from 计量单位换算表 where 物料编码='{0}'", dr["物料编码"]);
                                        using (SqlDataAdapter aa = new SqlDataAdapter(ss, strconn))
                                        {
                                            DataTable tt = new DataTable();
                                            aa.Fill(tt);
                                            DataRow[] r1 = tt.Select(string.Format("计量单位='{0}'", dr["bom单位"].ToString().Trim()));
                                            DataRow[] r2 = tt.Select(string.Format("计量单位='{0}'", dr["库存单位"].ToString().Trim()));
                                            decimal dec = Convert.ToDecimal(r2[0]["换算率"]) / Convert.ToDecimal(r1[0]["换算率"]);

                                            dr["有效总数"] = dec * Convert.ToDecimal(dr["有效总数"]);
                                            dr["库存总数"] = dec * Convert.ToDecimal(dr["库存总数"]);
                                        }

                                    }

                                }



                                ///在替代料中寻找 库存够的 一个 找到跳出
                                foreach (DataRow r3 in dt_3.Rows)
                                {

                                    r_库存 = ds.Tables[3].Select(string.Format("物料编码='{0}'", r["子项编码"].ToString()))[0];

                                    // decimal dec = dec_总需数 - Convert.ToDecimal(r_库存["库存总数"]); //2018-9-18 改判断库存为有效库存
                                    decimal dec = dec_总需数 - Convert.ToDecimal(r_库存["有效总数"]);


                                    if (dec <= 0)     //某一替代料库存够     就取这个物料
                                    {
                                        DataRow dr = ds.Tables[1].NewRow();
                                        dr["待领料单号"] = str_待领料单号;
                                        dr["待领料单明细号"] = str_待领料单号 + "-" + pos.ToString("00");
                                        dr["生产工单号"] = drr.Rows[0]["生产工单号"];
                                        dr["生产制令单号"] = drr.Rows[0]["生产制令单号"];
                                        dr["生产工单类型"] = drr.Rows[0]["生产工单类型"];
                                        dr["生产车间"] = drr.Rows[0]["生产车间"]; //车间编号
                                        dr["A面位号"] = r3["A面位号"];
                                        dr["B面位号"] = r3["B面位号"];
                                        //东屋
                                        dr["仓库号"] = r3["仓库号"];
                                        dr["仓库名称"] = r3["仓库名称"];
                                        dr["wiptype"] = r3["wiptype"];


                                        dr["计量单位编号"] = r3["计量单位编码"];
                                        dr["计量单位"] = r3["计量单位"];

                                        dr["物料编码"] = r3["子项编码"];
                                        dr["物料名称"] = r3["子项名称"];
                                        dr["规格型号"] = r3["规格型号"].ToString().Trim();
                                        try
                                        {
                                            dr["待领料总量"] = dec_总需数;
                                            dr["未领数量"] = dr["待领料总量"];
                                            dr["BOM数量"] = Convert.ToDecimal(r3["数量"]);
                                        }
                                        catch (Exception)
                                        {
                                            throw new Exception(string.Format("物料{0}BOM数量有问题 速找开发部确认", r3["产品编码"]));

                                        }
                                        dr["创建日期"] = t;
                                        dr["修改日期"] = t;
                                        dr["制单人员"] = s_name;
                                        dr["制单人员ID"] = s_id;

                                        dr["工单负责人"] = s_gdfzrname;
                                        dr["工单负责人ID"] = s_gdfzrID;
                                        dr["领料人ID"] = s_llrID;
                                        dr["领料人"] = s_llrname;
                                        pos++;
                                        ds.Tables[1].Rows.Add(dr);
                                        //r_库存["库存总数"] = Convert.ToDecimal(r_库存["库存总数"]) - dec_总需数; 2018-9-18 改判断库存总数为有效总数
                                        r_库存["有效总数"] = Convert.ToDecimal(r_库存["有效总数"]) - dec_总需数;

                                        dec_总需数 = -1;   //此次循环 

                                        break;
                                    }
                                }


                            }
                            //409行 没有找到一个库存单独就够的
                            if (dec_总需数 >= 0)     // 几种物料 单独一个库存不够所需领取数量时 
                            {

                                DataTable dt_替代 = new DataTable();
                                dt_替代 = fun_替代料递归(dt_替代, drr.Rows[0]["物料编码"].ToString(), dec_剩, r["组"].ToString(), 2, ds.Tables[3]);

                                //原优先级为1的物料需要多少数量
                                decimal a = 0;
                                if (dt_替代.Rows.Count > 0)
                                {
                                    a = Convert.ToDecimal(dt_替代.Rows[dt_替代.Rows.Count - 1]["剩余数"]);
                                }
                                else
                                {
                                    a = dec_总需数;
                                }
                                // if (Convert.ToDecimal(r_库存["库存总数"]) + a > 0)    2018-9-18 改判断库存总数为有效总数
                                if (Convert.ToDecimal(r_库存["有效总数"]) + a > 0)
                                {
                                    DataRow rrr = dt_替代.NewRow();
                                    rrr["产品编码"] = r["产品编码"];
                                    rrr["子项编码"] = r["子项编码"];
                                    rrr["产品名称"] = r["产品名称"];
                                    rrr["子项名称"] = r["子项名称"];
                                    rrr["A面位号"] = r["A面位号"];
                                    rrr["B面位号"] = r["B面位号"];
                                    rrr["wiptype"] = r["wiptype"];
                                    //东屋
                                    rrr["仓库号"] = r["仓库号"];
                                    rrr["仓库名称"] = r["仓库名称"];
                                    rrr["规格型号"] = r["规格型号"];
                                    // rrr["需要数"] = Convert.ToDecimal(r_库存["库存总数"]) + a;
                                    rrr["需要数"] = Convert.ToDecimal(r_库存["有效总数"]) + a;
                                    dt_替代.Rows.Add(rrr);
                                }
                                foreach (DataRow dr_替代 in dt_替代.Rows)
                                {
                                    DataRow dr_1 = ds.Tables[1].NewRow();
                                    dr_1["待领料单号"] = str_待领料单号;
                                    dr_1["待领料单明细号"] = str_待领料单号 + "-" + pos.ToString("00");
                                    dr_1["生产工单号"] = drr.Rows[0]["生产工单号"];
                                    dr_1["生产制令单号"] = drr.Rows[0]["生产制令单号"];
                                    dr_1["生产工单类型"] = drr.Rows[0]["生产工单类型"];
                                    dr_1["生产车间"] = drr.Rows[0]["生产车间"]; //车间编号
                                    dr_1["A面位号"] = dr_替代["A面位号"];
                                    dr_1["B面位号"] = dr_替代["B面位号"];
                                    dr_1["wiptype"] = dr_替代["wiptype"];
                                    dr_1["仓库号"] = dr_替代["仓库号"];
                                    dr_1["仓库名称"] = dr_替代["仓库名称"];
                                    dr_1["计量单位编号"] = dr_替代["计量单位编码"];
                                    dr_1["计量单位"] = dr_替代["计量单位"];
                                    dr_1["物料编码"] = dr_替代["子项编码"];
                                    dr_1["物料名称"] = dr_替代["子项名称"];
                                    dr_1["规格型号"] = dr_替代["规格型号"].ToString().Trim();
                                    try
                                    {
                                        dr_1["待领料总量"] = dr_替代["需要数"];
                                        dr_1["未领数量"] = dr_1["待领料总量"];
                                        dr_1["BOM数量"] = Convert.ToDecimal(r["数量"]);
                                    }
                                    catch (Exception)
                                    {
                                        throw new Exception(string.Format("物料{0}BOM数量有问题 速找开发部确认", r["产品编码"]));
                                    }
                                    dr_1["创建日期"] = t;
                                    dr_1["修改日期"] = t;
                                    dr_1["制单人员"] = s_name;
                                    dr_1["制单人员ID"] = s_id;
                                    dr_1["工单负责人"] = s_gdfzrname;
                                    dr_1["工单负责人ID"] = s_gdfzrID;
                                    dr_1["领料人ID"] = s_llrID;
                                    dr_1["领料人"] = s_llrname;
                                    pos++;
                                    ds.Tables[1].Rows.Add(dr_1);
                                }
                            }
                        }
                    }
                    else      //BOM无备料 原来的流程
                    {

                        foreach (DataRow r in dt_bom.Rows)
                        {
                            if (r["WIPType"].ToString().Trim() == "虚拟") //设定 虚拟件子项下不允许有替代料 所以只有一层 不需递归;需要在ds.Tables[3]中添加这些子项的库存信息
                            {                                              //19-11-14 针对虚拟件底下的虚拟件 玛德
                                string x = string.Format(@"select 产品编码,a.物料编码,库存总数,有效总数,b.计量单位 as bom单位,单位换算标识,单位换算标识 from 仓库物料数量表 a
                                left  join 基础数据物料信息表  base on base.物料编码=a.物料编码
                                Left  join 基础数据物料BOM表 b on a.物料编码=b.子项编码 and a.仓库号=b.仓库号 where  产品编码='{0}'", r["子项编码"]);
                                DataTable temp_stock = CZMaster.MasterSQL.Get_DataTable(x, strconn);
                                DataRow[] rrr = ds.Tables[3].Select(string.Format("产品编码='{0}'", r["子项编码"].ToString()));
                                if (rrr.Length == 0) //该成品未加载过
                                {
                                    foreach (DataRow dr_stock in temp_stock.Rows)
                                    {
                                        if (ds.Tables[3].Select(string.Format("物料编码='{0}'", dr_stock["物料编码"].ToString())).Length == 0) //dt_MIcach 里先找有没有  没有就添进去
                                        {
                                            ds.Tables[3].ImportRow(dr_stock);
                                            if (dr_stock["单位换算标识"].Equals(true)) //
                                            {
                                                string ss = string.Format("select  * from 计量单位换算表 where 物料编码='{0}'", dr_stock["物料编码"]);
                                                using (SqlDataAdapter a = new SqlDataAdapter(ss, strconn))
                                                {
                                                    DataTable dt_jl = new DataTable();
                                                    da.Fill(dt_jl);
                                                    DataRow[] r1 = dt_jl.Select(string.Format("计量单位='{0}'", dr_stock["bom单位"].ToString().Trim()));
                                                    DataRow[] r2 = dt_jl.Select(string.Format("计量单位='{0}'", dr_stock["库存单位"].ToString().Trim()));
                                                    decimal dec = Convert.ToDecimal(r2[0]["换算率"]) / Convert.ToDecimal(r1[0]["换算率"]);
                                                    //DataRow []rr=  dt.Select(string.Format("计量单位='{0}'", dr["库存单位"].ToString().Trim()));
                                                    dr_stock["有效总数"] = dec * Convert.ToDecimal(dr_stock["有效总数"]);
                                                    dr_stock["库存总数"] = dec * Convert.ToDecimal(dr_stock["库存总数"]);
                                                }
                                            }
                                        }
                                    }
                                }
                                //发料清单 如有虚拟件 取其子件 并去除他本身
                                //19-11-14 这里只用在已经是虚拟件上面  不改变原有的框架 涉及替代料 修改工程太大 
                                string s = string.Format(@"with temp_bom(产品编码,子项编码,仓库号,仓库名称,wiptype,子项类型,数量,bom类型,优先级,bom_level ) as
 (select  产品编码,子项编码,仓库号,仓库名称,WIPType,子项类型,数量,bom类型,优先级,1 as level from 基础数据物料BOM表    where 产品编码='{0}'
   union all 
   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型, convert(decimal(18, 6),a.数量*b.数量) as 数量,a.bom类型,a.优先级,b.bom_level+1  from 基础数据物料BOM表 a
   inner join temp_bom b on a.产品编码=b.子项编码 where b.wiptype='虚拟'  
   ) 
      select  产品编码,fx.物料名称 as  产品名称,子项编码,base.物料名称 as 子项名称,wiptype,子项类型,sum(数量)数量,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号  
      ,base.计量单位,base.计量单位编码 from  temp_bom a
  left join 基础数据物料信息表 base  on base.物料编码=a.子项编码
  left join 基础数据物料信息表  fx  on fx.物料编码=a.产品编码 where wiptype not in('入库倒冲','虚拟') and 优先级=1
  group by 产品编码,fx.物料名称 ,子项编码,base.物料名称 ,wiptype,子项类型 ,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号
  ,base.计量单位,base.计量单位编码", r["子项编码"]);
                                DataTable temp_virtual = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                                foreach (DataRow vr in temp_virtual.Rows)
                                {
                                    DataRow vdr = ds.Tables[1].NewRow();
                                    vdr["待领料单号"] = str_待领料单号;
                                    vdr["待领料单明细号"] = str_待领料单号 + "-" + pos.ToString("00");
                                    vdr["生产工单号"] = drr.Rows[0]["生产工单号"];
                                    vdr["生产制令单号"] = drr.Rows[0]["生产制令单号"];
                                    vdr["生产工单类型"] = drr.Rows[0]["生产工单类型"];
                                    vdr["生产车间"] = drr.Rows[0]["生产车间"]; //车间编号
                                    //vdr["A面位号"] = vr["A面位号"];
                                    //vdr["B面位号"] = vr["B面位号"];
                                    vdr["wiptype"] = "虚拟件子件";
                                    vdr["仓库号"] = vr["仓库号"];
                                    vdr["仓库名称"] = vr["仓库名称"];
                                    vdr["计量单位编号"] = vr["计量单位编码"];
                                    vdr["计量单位"] = vr["计量单位"];
                                    vdr["物料编码"] = vr["子项编码"];
                                    vdr["物料名称"] = vr["子项名称"];
                                    vdr["规格型号"] = vr["规格型号"].ToString().Trim();
                                    try
                                    {
                                        vdr["待领料总量"] = Convert.ToDecimal(drr.Rows[0]["生产数量"]) * Convert.ToDecimal(r["数量"]) * Convert.ToDecimal(vr["数量"]);
                                        vdr["未领数量"] = vdr["待领料总量"];

                                        vdr["BOM数量"] = Convert.ToDecimal(vr["数量"]);

                                    }
                                    catch (Exception)
                                    {
                                        throw new Exception(string.Format("物料{0}BOM数量有问题 速找开发部确认", r["产品编码"]));

                                    }
                                    vdr["创建日期"] = t;
                                    vdr["修改日期"] = t;
                                    vdr["制单人员"] = s_name;
                                    vdr["制单人员ID"] = s_id;
                                    vdr["工单负责人"] = s_gdfzrname;
                                    vdr["工单负责人ID"] = s_gdfzrID;
                                    vdr["领料人ID"] = s_llrID;
                                    vdr["领料人"] = s_llrname;
                                    pos++;
                                    ds.Tables[1].Rows.Add(vdr);
                                    DataRow[] rvr_kc = ds.Tables[3].Select(string.Format("物料编码='{0}'", vr["子项编码"].ToString()));
                                    if (rvr_kc.Length > 0)
                                    {
                                        //decimal d = Convert.ToDecimal(rr_kc["库存总数"]) - Convert.ToDecimal(dr["待领料总量"]); //2018-9-18 改判断库存总数为有效库存
                                        // rr_kc["库存总数"]  = d>=0?d:0;
                                        decimal vd = Convert.ToDecimal(rvr_kc[0]["有效总数"]) - Convert.ToDecimal(vdr["待领料总量"]);
                                        rvr_kc[0]["有效总数"] = vd >= 0 ? vd : 0;
                                    }
                                }

                                continue;
                            }

                            DataRow dr = ds.Tables[1].NewRow();
                            dr["待领料单号"] = str_待领料单号;
                            dr["待领料单明细号"] = str_待领料单号 + "-" + pos.ToString("00");
                            dr["生产工单号"] = drr.Rows[0]["生产工单号"];
                            dr["生产制令单号"] = drr.Rows[0]["生产制令单号"];
                            dr["生产工单类型"] = drr.Rows[0]["生产工单类型"];
                            dr["生产车间"] = drr.Rows[0]["生产车间"]; //车间编号
                            dr["A面位号"] = r["A面位号"];
                            dr["B面位号"] = r["B面位号"];
                            dr["wiptype"] = r["wiptype"];
                            dr["仓库号"] = r["仓库号"];
                            dr["仓库名称"] = r["仓库名称"];
                            dr["计量单位编号"] = r["计量单位编码"];
                            dr["计量单位"] = r["计量单位"];
                            dr["物料编码"] = r["子项编码"];
                            dr["物料名称"] = r["子项名称"];
                            dr["规格型号"] = r["规格型号"].ToString().Trim();
                            try
                            {
                                dr["待领料总量"] = Convert.ToDecimal(drr.Rows[0]["生产数量"]) * Convert.ToDecimal(r["数量"]);
                                dr["未领数量"] = dr["待领料总量"];
                                dr["BOM数量"] = Convert.ToDecimal(r["数量"]);
                            }
                            catch (Exception)
                            {
                                throw new Exception(string.Format("物料{0}BOM数量有问题 速找开发部确认", r["产品编码"]));

                            }
                            dr["创建日期"] = t;
                            dr["修改日期"] = t;
                            dr["制单人员"] = s_name;
                            dr["制单人员ID"] = s_id;

                            dr["工单负责人"] = s_gdfzrname;
                            dr["工单负责人ID"] = s_gdfzrID;
                            dr["领料人ID"] = s_llrID;
                            dr["领料人"] = s_llrname;
                            pos++;
                            ds.Tables[1].Rows.Add(dr);
                            DataRow[] rr_kc = ds.Tables[3].Select(string.Format("物料编码='{0}'", r["子项编码"].ToString()));
                            if (rr_kc.Length > 0)
                            {
                                decimal d = Convert.ToDecimal(rr_kc[0]["有效总数"]) - Convert.ToDecimal(dr["待领料总量"]);
                                rr_kc[0]["有效总数"] = d >= 0 ? d : 0;

                            }
                            //decimal d = Convert.ToDecimal(rr_kc["库存总数"]) - Convert.ToDecimal(dr["待领料总量"]); //2018-9-18 改判断库存总数为有效库存
                            // rr_kc["库存总数"]  = d>=0?d:0;

                        }

                    }
                }
            }

            return (ds);

        }



    }
}
