using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace ERPSale
{
    public partial class frm销售明细分析界面 : UserControl
    {
        #region 成员
        /// <summary>
        /// dtM 用于装载所有需要分析的明细
        /// </summary>
        DataTable dtM;
        DataTable dt_生产 = new DataTable();
        DataTable dt_采购 = new DataTable();
        /// <summary>
        /// 保存生产、采购分析时，生效MRP3种数量
        /// </summary>
        DataTable dt_物料数量 = new DataTable();
        DataSet dateset = new DataSet();
        string strconn = CPublic.Var.strConn;
        DataTable dt_待办;
        #endregion

        #region 自用类
        public frm销售明细分析界面()
        {
            InitializeComponent();
        }

        private void frm销售明细分析界面_Enter(object sender, EventArgs e)
        {
            try
            {
                
                fun_载入数据();
                fun_载入待办();
                dt_生产.Columns.Add("物料编码");
                dt_生产.Columns.Add("物料名称");
                dt_生产.Columns.Add("规格型号");
                dt_生产.Columns.Add("图纸编号");
                dt_生产.Columns.Add("特殊备注");
                dt_生产.Columns.Add("原规格型号");
                dt_生产.Columns.Add("原ERP物料编号");
                dt_生产.Columns.Add("POS");
                dt_生产.Columns.Add("物料类型");
                dt_生产.Columns.Add("层级");
                dt_生产.Columns.Add("订单数量", typeof(Decimal));
                dt_生产.Columns.Add("欠缺数量", typeof(Decimal));
                dt_生产.Columns.Add("上级物料");
                dt_生产.Columns.Add("已计算");

                dt_采购.Columns.Add("物料编码");
                dt_采购.Columns.Add("物料名称");
                dt_采购.Columns.Add("原ERP物料编号");
                dt_采购.Columns.Add("原规格型号");
                dt_采购.Columns.Add("规格型号");
                dt_采购.Columns.Add("图纸编号");
                dt_采购.Columns.Add("物料类型");
                dt_采购.Columns.Add("物料数量", typeof(Decimal));
                dt_采购.Columns.Add("仓库参考数量", typeof(Decimal));
                dt_采购.Columns.Add("总需数量", typeof(Decimal));
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + "frm销售明细分析界面_Load");
            }
        }

        private void frm销售明细分析界面_Load(object sender, EventArgs e)
        {
        }

        void dt_待办_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column.ColumnName == "选择")
            {
                gv.CloseEditor();
                gc.BindingContext[dt_待办].EndCurrentEdit();
                foreach (DataRow r in dt_待办.Rows)
                {
                    if (r["选择"].ToString().ToLower() == "true")
                    {
                        int count = 0;
                        foreach (DataRow rr in dtM.Rows)
                        {                                
                            if (rr.RowState == DataRowState.Deleted)
                            {
                                continue;
                            }
                            if (r["销售订单明细号"].ToString() == rr["销售订单明细号"].ToString())
                            {
                                continue;
                            }
                            else
                            {
                                count++;
                            }
                        }
                        if (count == dtM.Rows.Count)
                        {
                            DataRow dr = dtM.NewRow();
                            dtM.Rows.Add(dr);
                            dr["GUID"] = r["GUID"].ToString();
                           
                            dr["可售"] = r["可售"];

                            dr["销售订单号"] = r["销售订单号"].ToString();
                            dr["销售订单明细号"] = r["销售订单明细号"].ToString();
                            dr["物料编码"] = r["物料编码"].ToString();
                            dr["物料名称"] = r["物料名称"].ToString();
                            dr["规格型号"] = r["规格型号"].ToString();
                            dr["n原ERP规格型号"] = r["n原ERP规格型号"].ToString();
                            dr["原ERP物料编号"] = r["原ERP物料编号"].ToString();
                            dr["图纸编号"] = r["图纸编号"].ToString();
                            dr["特殊备注"] = r["特殊备注"].ToString();
                            dr["数量"] = r["数量"].ToString();
                            dr["完成数量"] = r["完成数量"].ToString();
                            dr["未完成数量"] = r["未完成数量"].ToString();
                            dr["计量单位"] = r["计量单位"].ToString();
                            dr["物料类型"] = r["物料类型"];
                            gcM.DataSource = dtM;
                        }
                    }
                    else
                    {
                        DataRow[] ds = dtM.Select(string.Format("销售订单明细号 = '{0}'", r["销售订单明细号"].ToString()));
                        if (ds.Length > 0)
                        {
                            ds[0].Delete();
                        }
                    }
                }
            }
        }        
        #endregion

        #region 销售订单明细方法
        private void fun_载入数据()
        {
            try
            {
                string sql = string.Format("select * from 销售记录销售订单明细表 where 1<>1");
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                dtM.Columns.Add("物料类型");
                dtM.Columns.Add("可售",typeof(bool));

                dtM.Columns.Add("原ERP物料编号");
                dtM.Columns.Add("n原ERP规格型号");
                gcM.DataSource = dtM;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm销售明细分析界面_fun_载入数据");
            }
        }
        //没用
        private void fun_选择完后操作(不用frm销售记录销售单选择界面 fm)
        {
            try
            {
                //dr_传.Clear();
                foreach (DataRow r in fm.dt_选择.Rows)
                {
                    if (dtM.Select(string.Format("销售订单明细号 = '{0}'", r["销售订单明细号"].ToString())).Length > 0) { }
                    else
                    {
                        DataRow dr = dtM.NewRow();
                        dtM.Rows.Add(dr);
                        dr["GUID"] = r["GUID"].ToString();
                        dr["销售订单号"] = r["销售订单号"].ToString();
                        dr["销售订单明细号"] = r["销售订单明细号"].ToString();
                        dr["物料编码"] = r["物料编码"].ToString();
                        dr["物料名称"] = r["物料名称"].ToString();
                        dr["规格型号"] = r["规格型号"].ToString();
                        dr["图纸编号"] = r["图纸编号"].ToString();
                        dr["数量"] = r["数量"].ToString();
                        dr["完成数量"] = r["完成数量"].ToString();
                        dr["未完成数量"] = r["未完成数量"].ToString();
                        dr["计量单位"] = r["计量单位"].ToString();
                    }
                }
                foreach (DataRow r in dtM.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    if (fm.dt_选择.Select(string.Format("销售订单明细号 = '{0}'", r["销售订单明细号"].ToString())).Length == 0)
                    {
                        r.Delete();
                    }
                }
                gcM.DataSource = dtM;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
            }
        }
        Boolean bl = false;
        /// <summary>
        /// 分析dtM中的销售订单明细，并生成原料表（dt_采购(表一)）和产品、半成品表（dt_生产(表二)）
        /// </summary>
        /// <param name="dtM"></param>
        private void fun_生成表一和表二(DataTable dtM)
        {
            //生成dt_采购表一和dt_生产表二
            foreach (DataRow r in dtM.Rows)
            {
                //if (r_x["已计算"].ToString().ToLower() == "true")
                //{
                //    continue;
                //}
                if (r["物料类型"].ToString() == "原材料" && r["可售"].Equals (true))
                {
                    bl = true;
                    DataRow[] dss = dt_采购.Select(string.Format("物料编码 = '{0}'", r["物料编码"].ToString()));
                    if (dss.Length > 0)
                    {
                        dss[0]["总需数量"] = Convert.ToDecimal(dss[0]["总需数量"]) + Convert.ToDecimal(r["未完成数量"]);
                    }
                    else
                    {
                        DataRow dr1 = dt_采购.NewRow();
                        dt_采购.Rows.Add(dr1);
                        dr1["物料编码"] = r["物料编码"].ToString();
                        dr1["物料名称"] = r["物料名称"].ToString();
                        dr1["规格型号"] = r["规格型号"].ToString();
                        dr1["图纸编号"] = r["图纸编号"].ToString();
                        dr1["原ERP物料编号"] = r["原ERP物料编号"];
                        dr1["原规格型号"] = r["n原ERP规格型号"].ToString();
                        //dr1["物料类型"] = "可售原材料";
                        dr1["物料类型"] = "原材料";

                        dr1["物料数量"] = Convert.ToDecimal(r["未完成数量"]);
                        dr1["总需数量"] = Convert.ToDecimal(r["未完成数量"]);
                    }
                    continue;
                }
                DataRow[] ds = dt_生产.Select(string.Format("物料编码 = '{0}'", r["物料编码"].ToString()));
                if (ds.Length > 0)
                {
                    ds[0]["订单数量"] = Convert.ToDecimal(ds[0]["订单数量"]) + Convert.ToDecimal(r["未完成数量"]);
                }
                else
                {
                    DataRow dr = dt_生产.NewRow();
                    dt_生产.Rows.Add(dr);
                    dr["物料编码"] = r["物料编码"].ToString();
                    dr["物料名称"] = r["物料名称"].ToString();
                    dr["规格型号"] = r["规格型号"].ToString();
                    dr["图纸编号"] = r["图纸编号"].ToString();
                    dr["特殊备注"] = r["特殊备注"].ToString();
                    dr["原规格型号"] = r["n原ERP规格型号"].ToString();
                    dr["原ERP物料编号"] = r["原ERP物料编号"];
                    dr["POS"] = r["POS"].ToString();
                    dr["物料类型"] = "产品";
                    dr["层级"] = 0;
                    dr["订单数量"] = r["未完成数量"].ToString();
                    dr["上级物料"] = "无";
                }
                DataTable dt_返回值 = StockCore.StockCorer.fun_物料_单_计算(r["物料编码"].ToString(), "", strconn, true);
                int count = 0;
                foreach (DataTable t in dateset.Tables)
                {
                    if (t.TableName != r["物料编码"].ToString())
                    {
                        count = count + 1;
                    }
                }
                if (count == dateset.Tables.Count)
                {
                    dt_返回值.TableName = r["物料编码"].ToString();
                    dateset.Tables.Add(dt_返回值);
                }
                foreach (DataRow rr in dt_返回值.Rows)
                {
                    if (rr["节点标记"].ToString() == "叶子")
                    {
                        DataRow[] dss = dt_采购.Select(string.Format("物料编码 = '{0}'", rr["物料编码"].ToString()));
                        if (dss.Length > 0)
                        {
                            //dss[0]["物料数量"] = Convert.ToDecimal(dss[0]["物料数量"]) + Convert.ToDecimal(rr["物料数量"]) * Convert.ToDecimal(r_x["未完成数量"]);
                            dss[0]["总需数量"] = Convert.ToDecimal(dss[0]["总需数量"]) + Convert.ToDecimal(rr["物料数量"]) * Convert.ToDecimal(r["未完成数量"]);
                        }
                        else
                        {
                            DataRow dr1 = dt_采购.NewRow();
                            dt_采购.Rows.Add(dr1);
                            dr1["物料编码"] = rr["物料编码"].ToString();
                            dr1["物料名称"] = rr["物料名称"].ToString();
                            dr1["规格型号"] = rr["规格型号"].ToString();
                            dr1["图纸编号"] = rr["图纸编号"].ToString();
                            string sqlsql = string.Format("select n原ERP规格型号,原ERP物料编号 from 基础数据物料信息表 where 物料编码 = '{0}'", rr["物料编码"].ToString());
                            DataTable dtdt = new DataTable();
                            SqlDataAdapter dada = new SqlDataAdapter(sqlsql, strconn);
                            dada.Fill(dtdt);
                            if (dtdt.Rows.Count > 0)
                            {
                                dr1["原ERP物料编号"] = dtdt.Rows[0]["原ERP物料编号"];
                                dr1["原规格型号"] = dtdt.Rows[0]["n原ERP规格型号"].ToString();
                            }
                            dr1["物料类型"] = "原料";
                            //dr1["物料数量"] = Convert.ToDecimal(rr["物料数量"]) * Convert.ToDecimal(r_x["未完成数量"]);
                            dr1["物料数量"] = 0;
                            dr1["总需数量"] = Convert.ToDecimal(rr["物料数量"]) * Convert.ToDecimal(r["未完成数量"]); 
                        }
                    }
                    else  //中间
                    {
                        DataRow[] dss = dt_生产.Select(string.Format("物料编码 = '{0}' and 上级物料 = '{1}'", rr["物料编码"].ToString(), rr["上级物料"].ToString()));
                        if (dss.Length > 0)
                        {
                            dss[0]["订单数量"] = Convert.ToDecimal(dss[0]["订单数量"]) + Convert.ToDecimal(rr["物料数量"]);
                        }
                        else
                        {
                            DataRow dr2 = dt_生产.NewRow();
                            dt_生产.Rows.Add(dr2);
                            dr2["物料编码"] = rr["物料编码"].ToString();
                            dr2["物料名称"] = rr["物料名称"].ToString();
                            dr2["规格型号"] = rr["规格型号"].ToString();
                            dr2["图纸编号"] = rr["图纸编号"].ToString();
                            string sqlsql = string.Format("select n原ERP规格型号,特殊备注,原ERP物料编号 from 基础数据物料信息表 where 物料编码 = '{0}'", rr["物料编码"].ToString());
                            DataTable dtdt = new DataTable();
                            SqlDataAdapter dada = new SqlDataAdapter(sqlsql, strconn);
                            dada.Fill(dtdt);
                            if (dtdt.Rows.Count > 0)
                            {
                                dr2["原ERP物料编号"] = dtdt.Rows[0]["原ERP物料编号"];
                                dr2["原规格型号"] = dtdt.Rows[0]["n原ERP规格型号"].ToString();
                                dr2["特殊备注"] = dtdt.Rows[0]["特殊备注"].ToString();
                            }
                            dr2["POS"] = r["POS"].ToString();
                            dr2["物料类型"] = "半成品";
                            dr2["层级"] = rr["层级"].ToString();
                            dr2["订单数量"] = Convert.ToDecimal(rr["物料数量"]);
                            dr2["上级物料"] = rr["上级物料"].ToString();
                        }
                    }                    
                }
                r["已计算"] = true;
            }
            
            DataView dv = new DataView(dt_生产);
            dv.Sort = "层级";
            //计算每个产品的欠缺数量，并跟 dt_采购 和 dt_生产 进行减法
            foreach (DataRow rr in dt_生产.Rows)
            {
                if (rr["已计算"].ToString().ToLower() == "true")
                {
                    continue;
                }
                if (rr["物料类型"].ToString() == "半成品")
                {
                    continue;
                }
                fun_计算_生产(rr, rr["物料编码"].ToString());
                rr["已计算"] = true;
            }
            ////dt_采购 中每个原料和库存做比较 得出欠缺数量
            if (dt_生产.Select("欠缺数量 > '0'").Length > 0 || bl == true)
            {
                foreach (DataRow rrr in dt_采购.Rows)
                {
                    try
                    {
                        DataRow[] dd = dt_物料数量.Select(string.Format("物料编码 = '{0}'", rrr["物料编码"].ToString().Trim()));
                        if (dd.Length <= 0)
                        {
                            string sql_物料数量 = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}'", rrr["物料编码"].ToString().Trim());
                            SqlDataAdapter da_物料数量 = new SqlDataAdapter(sql_物料数量, strconn);
                            da_物料数量.Fill(dt_物料数量);
                        }
                    }
                    catch
                    {
                        string sql_物料数量 = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}'", rrr["物料编码"].ToString().Trim());
                        SqlDataAdapter da_物料数量 = new SqlDataAdapter(sql_物料数量, strconn);
                        da_物料数量.Fill(dt_物料数量);
                    }
                    DataRow[] dts = null;
                    Decimal dec_有效数量 = 0;
                    try
                    {
                        dts = dt_物料数量.Select(string.Format("物料编码 = '{0}'", rrr["物料编码"].ToString()));
                        dec_有效数量 = Convert.ToDecimal(dts[0]["库存总数"]) + Convert.ToDecimal(dts[0]["在途量"]) + Convert.ToDecimal(dts[0]["在制量"]);
                    }
                    catch {
                        CZMaster.MasterLog.WriteLog(string.Format("不存在物料{0}", rrr["物料编码"].ToString()), "MRP分析界面");
                        continue;
                    }
                    Decimal dec_MRP_计算数量 = dec_有效数量 - Convert.ToDecimal(rrr["物料数量"]) - Convert.ToDecimal(dts[0]["MRP库存锁定量"]);
                    if (dec_MRP_计算数量 < 0)
                    {
                        rrr["物料数量"] = -dec_MRP_计算数量;
                        if ((Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量) >= Convert.ToDecimal(dts[0]["库存总数"]))
                        {
                            dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["库存总数"]);
                        }
                        else
                        {
                            dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量;
                        }
                    }
                    else
                    {
                        rrr["物料数量"] = 0;
                        if ((Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量 - dec_MRP_计算数量) >= Convert.ToDecimal(dts[0]["库存总数"]))
                        {
                            dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["库存总数"]);
                        }
                        else
                        {
                            dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量 - dec_MRP_计算数量;
                        }
                    }
                }
            }
        }

        public DataSet fun_生成表一和表二(DataTable dt_生产, DataTable dt_采购, DataTable dt_物料数量, DataTable dtM,int i)
        {
            //生成dt_采购表一和dt_生产表二
            foreach (DataRow r in dtM.Rows)
            {
                //if (r_x["物料类型"].ToString() == "可售原材料")
                if (r["物料类型"].ToString() == "原材料" && r["可售"].Equals(true))

                {
                    DataRow[] dss = dt_采购.Select(string.Format("物料编码 = '{0}'", r["物料编码"].ToString()));
                    if (dss.Length > 0)
                    {
                        dss[0]["总需数量"] = Convert.ToDecimal(dss[0]["总需数量"]) + Convert.ToDecimal(r["未完成数量"]);
                    }
                    else
                    {
                        DataRow dr1 = dt_采购.NewRow();
                        dt_采购.Rows.Add(dr1);
                        dr1["物料编码"] = r["物料编码"].ToString();
                        dr1["物料名称"] = r["物料名称"].ToString();
                        dr1["规格型号"] = r["规格型号"].ToString();
                        dr1["图纸编号"] = r["图纸编号"].ToString();
                        //dr1["物料类型"] = "可售原材料";
                        dr1["物料类型"] = "原材料";

                        dr1["物料数量"] = 0;
                        dr1["总需数量"] = Convert.ToDecimal(r["未完成数量"]);
                    }
                    continue;
                }
                DataRow[] ds = dt_生产.Select(string.Format("物料编码 = '{0}'", r["物料编码"].ToString()));
                if (ds.Length > 0)
                {
                    ds[0]["订单数量"] = Convert.ToDecimal(ds[0]["订单数量"]) + Convert.ToDecimal(r["未完成数量"]);
                }
                else
                {
                    DataRow dr = dt_生产.NewRow();
                    dt_生产.Rows.Add(dr);
                    dr["物料编码"] = r["物料编码"].ToString();
                    dr["物料名称"] = r["物料名称"].ToString();
                    dr["规格型号"] = r["规格型号"].ToString();
                    dr["图纸编号"] = r["图纸编号"].ToString();
                    dr["特殊备注"] = r["特殊备注"].ToString();
                    dr["原规格型号"] = r["n原ERP规格型号"].ToString();
                    dr["POS"] = r["POS"].ToString();
                    dr["物料类型"] = "产品";
                    dr["层级"] = 0;
                    dr["订单数量"] = r["未完成数量"].ToString();
                    dr["上级物料"] = "无";
                }
                DataTable dt_返回值 = StockCore.StockCorer.fun_物料_单_计算(r["物料编码"].ToString(), "", strconn, true);
                int count = 0;
                foreach (DataTable t in dateset.Tables)
                {
                    if (t.TableName != r["物料编码"].ToString())
                    {
                        count = count + 1;
                    }
                }
                if (count == dateset.Tables.Count)
                {
                    dt_返回值.TableName = r["物料编码"].ToString();
                    dateset.Tables.Add(dt_返回值);
                }
                foreach (DataRow rr in dt_返回值.Rows)
                {
                    if (rr["节点标记"].ToString() == "叶子")
                    {
                        DataRow[] dss = dt_采购.Select(string.Format("物料编码 = '{0}'", rr["物料编码"].ToString()));
                        if (dss.Length > 0)
                        {
                            //dss[0]["物料数量"] = Convert.ToDecimal(dss[0]["物料数量"]) + Convert.ToDecimal(rr["物料数量"]) * Convert.ToDecimal(r_x["未完成数量"]);
                            dss[0]["总需数量"] = Convert.ToDecimal(dss[0]["总需数量"]) + Convert.ToDecimal(rr["物料数量"]) * Convert.ToDecimal(r["未完成数量"]);
                        }
                        else
                        {
                            DataRow dr1 = dt_采购.NewRow();
                            dt_采购.Rows.Add(dr1);
                            dr1["物料编码"] = rr["物料编码"].ToString();
                            dr1["物料名称"] = rr["物料名称"].ToString();
                            dr1["规格型号"] = rr["规格型号"].ToString();
                            dr1["图纸编号"] = rr["图纸编号"].ToString();
                            dr1["物料类型"] = "原料";
                            //dr1["物料数量"] = Convert.ToDecimal(rr["物料数量"]) * Convert.ToDecimal(r_x["未完成数量"]);
                            dr1["物料数量"] = 0;
                            dr1["总需数量"] = Convert.ToDecimal(rr["物料数量"]) * Convert.ToDecimal(r["未完成数量"]);
                        }
                    }
                    else  //中间
                    {
                        DataRow[] dss = dt_生产.Select(string.Format("物料编码 = '{0}'", rr["物料编码"].ToString()));
                        if (dss.Length > 0)
                        {
                            dss[0]["订单数量"] = Convert.ToDecimal(dss[0]["订单数量"]) + Convert.ToDecimal(rr["物料数量"]);
                        }
                        else
                        {
                            DataRow dr2 = dt_生产.NewRow();
                            dt_生产.Rows.Add(dr2);
                            dr2["物料编码"] = rr["物料编码"].ToString();
                            dr2["物料名称"] = rr["物料名称"].ToString();
                            dr2["规格型号"] = rr["规格型号"].ToString();
                            dr2["图纸编号"] = rr["图纸编号"].ToString();
                            string sqlsql = string.Format("select n原ERP规格型号,特殊备注 from 基础数据物料信息表 where 物料编码 = '{0}'", rr["物料编码"].ToString());
                            DataTable dtdt = new DataTable();
                            SqlDataAdapter dada = new SqlDataAdapter(sqlsql, strconn);
                            dada.Fill(dtdt);
                            if (dtdt.Rows.Count > 0)
                            {
                                dr2["原规格型号"] = dtdt.Rows[0]["n原ERP规格型号"].ToString();
                                dr2["特殊备注"] = dtdt.Rows[0]["特殊备注"].ToString();
                            }
                            dr2["POS"] = r["POS"].ToString();
                            dr2["物料类型"] = "半成品";
                            dr2["层级"] = rr["层级"].ToString();
                            dr2["订单数量"] = rr["物料数量"].ToString();
                            dr2["上级物料"] = rr["上级物料"].ToString();
                        }
                    }
                }
                r["已计算"] = true;
            }

            DataView dv = new DataView(dt_生产);
            dv.Sort = "层级";
            //计算每个产品的欠缺数量，并跟 dt_采购 和 dt_生产 进行减法
            foreach (DataRow rr in dt_生产.Rows)
            {
                if (rr["已计算"].ToString().ToLower() == "true")
                {
                    continue;
                }
                if (rr["物料类型"].ToString() == "半成品")
                {
                    continue;
                }
                fun_计算_生产(dt_生产, dt_采购, dt_物料数量, rr, rr["物料编码"].ToString());
                rr["已计算"] = true;
            }
            ////dt_采购 中每个原料和库存做比较 得出欠缺数量
            if (dt_生产.Select("欠缺数量 > '0'").Length > 0 || bl == true)
            {
                foreach (DataRow rrr in dt_采购.Rows)
                {
                    try
                    {
                        DataRow[] dd = dt_物料数量.Select(string.Format("物料编码 = '{0}'", rrr["物料编码"].ToString().Trim()));
                        if (dd.Length <= 0)
                        {
                            string sql_物料数量 = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}'", rrr["物料编码"].ToString().Trim());
                            SqlDataAdapter da_物料数量 = new SqlDataAdapter(sql_物料数量, strconn);
                            da_物料数量.Fill(dt_物料数量);
                        }
                    }
                    catch
                    {
                        string sql_物料数量 = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}'", rrr["物料编码"].ToString().Trim());
                        SqlDataAdapter da_物料数量 = new SqlDataAdapter(sql_物料数量, strconn);
                        da_物料数量.Fill(dt_物料数量);
                    }
                    DataRow[] dts = null;
                    Decimal dec_有效数量 = 0;
                    try
                    {
                        dts = dt_物料数量.Select(string.Format("物料编码 = '{0}'", rrr["物料编码"].ToString()));
                        dec_有效数量 = Convert.ToDecimal(dts[0]["库存总数"]) + Convert.ToDecimal(dts[0]["在途量"]) + Convert.ToDecimal(dts[0]["在制量"]);
                    }
                    catch
                    {
                        CZMaster.MasterLog.WriteLog(string.Format("不存在物料{0}", rrr["物料编码"].ToString()), "MRP分析界面");
                        continue;
                    }
                    Decimal dec_MRP_计算数量 = dec_有效数量 - Convert.ToDecimal(rrr["物料数量"]) - Convert.ToDecimal(dts[0]["MRP库存锁定量"]);
                    if (dec_MRP_计算数量 < 0)
                    {
                        rrr["物料数量"] = -dec_MRP_计算数量;
                        if ((Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量) >= Convert.ToDecimal(dts[0]["库存总数"]))
                        {
                            dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["库存总数"]);
                        }
                        else
                        {
                            dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量;
                        }
                    }
                    else
                    {
                        rrr["物料数量"] = 0;
                        if ((Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量 - dec_MRP_计算数量) >= Convert.ToDecimal(dts[0]["库存总数"]))
                        {
                            dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["库存总数"]);
                        }
                        else
                        {
                            dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量 - dec_MRP_计算数量;
                        }
                        //dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量 - dec_MRP_计算数量;
                    }
                }
            }

            DataSet dset = new DataSet();
            dset.Tables.Add(dt_采购);
            dt_采购.TableName = "采购计划";
            dset.Tables.Add(dt_生产);
            dt_生产.TableName = "生产计划";
            dset.Tables.Add(dt_物料数量);
            dt_物料数量.TableName = "物料数量";
            return dset;
        }

        private DataRow fun_计算_生产(DataTable dt_生产, DataTable dt_采购, DataTable dt_物料数量, DataRow rr, string str_TableName)
        {
            Decimal dec_MRP_计算数量 = 0;
            try
            {
                DataRow[] dd = dt_物料数量.Select(string.Format("物料编码 = '{0}'", rr["物料编码"].ToString().Trim()));
                if (dd.Length <= 0)
                {
                    string sql_物料数量 = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}'", rr["物料编码"].ToString().Trim());
                    SqlDataAdapter da_物料数量 = new SqlDataAdapter(sql_物料数量, strconn);
                    da_物料数量.Fill(dt_物料数量);
                }
            }
            catch
            {
                string sql_物料数量 = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}'", rr["物料编码"].ToString().Trim());
                SqlDataAdapter da_物料数量 = new SqlDataAdapter(sql_物料数量, strconn);
                da_物料数量.Fill(dt_物料数量);
            }
            DataRow[] dts = dt_物料数量.Select(string.Format("物料编码 = '{0}'", rr["物料编码"].ToString().Trim()));
            Decimal dec_有效数量 = 0;
            if (dts.Length > 0)
            {
                dec_有效数量 = Convert.ToDecimal(dts[0]["库存总数"]) + Convert.ToDecimal(dts[0]["在途量"]) + Convert.ToDecimal(dts[0]["在制量"]);
            }
            else
            {
                throw new Exception(string.Format("物料{0}未初始化", rr["物料编码"].ToString().Trim()));
            }
            if (rr["物料类型"].ToString() == "产品")
            {
                if (Convert.ToDecimal(dts[0]["MRP库存锁定量"]) > (Decimal)0)
                {
                    dec_MRP_计算数量 = dec_有效数量 - Convert.ToDecimal(rr["订单数量"]) - Convert.ToDecimal(dts[0]["MRP库存锁定量"]);
                }
                else
                {
                    dec_MRP_计算数量 = dec_有效数量 - Convert.ToDecimal(rr["订单数量"]);
                }
                if (dec_MRP_计算数量 < 0)
                {
                    rr["欠缺数量"] = -dec_MRP_计算数量;
                    if ((Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量) >= Convert.ToDecimal(dts[0]["库存总数"]))
                    {
                        dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["库存总数"]);
                    }
                    else
                    {
                        dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量;
                    }
                    //计算一级原料
                    foreach (DataTable t in dateset.Tables)
                    {
                        if (t.TableName == str_TableName)
                        {
                            DataRow[] drs = t.Select(string.Format("上级物料 = '{0}' and 节点标记 = '叶子'", rr["物料编码"].ToString()));
                            foreach (DataRow dsrs in drs)
                            {
                                DataRow[] drs_采购 = dt_采购.Select(string.Format("物料编码 = '{0}'", dsrs["物料编码"].ToString()));
                                drs_采购[0]["物料数量"] = Convert.ToDecimal(drs_采购[0]["物料数量"]) + (-dec_MRP_计算数量) * Convert.ToDecimal(dsrs["BOM数量"]);
                            }
                        }
                    }
                    //计算属于该产品的半成品
                    DataRow[] ds = dt_生产.Select(string.Format("上级物料 = '{0}'", rr["物料编码"].ToString()));//销售订单明细号 = '{0}'and  rr["销售订单明细号"].ToString(), 
                    if (ds.Length > 0)
                    {
                        foreach (DataRow r1 in ds)
                        {
                            fun_计算_生产(dt_生产, dt_采购, dt_物料数量, r1, str_TableName);
                        }
                    }
                }
                else
                {
                    rr["欠缺数量"] = 0;
                    if ((Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量 - dec_MRP_计算数量) >= Convert.ToDecimal(dts[0]["库存总数"]))
                    {
                        dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["库存总数"]);
                    }
                    else
                    {
                        dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量 - dec_MRP_计算数量;
                    }
                }
            }
            if (rr["物料类型"].ToString() == "半成品")
            {
                DataRow[] ds = dt_生产.Select(string.Format("物料编码 = '{0}'", rr["上级物料"].ToString()));//销售订单明细号 = '{0}'and rr["销售订单明细号"].ToString(),
                if (Convert.ToDecimal(ds[0]["欠缺数量"]) > 0)
                {
                    if (Convert.ToDecimal(dts[0]["MRP库存锁定量"]) > (Decimal)0)
                    {
                        dec_MRP_计算数量 = dec_有效数量 - Convert.ToDecimal(rr["订单数量"]) * Convert.ToDecimal(ds[0]["欠缺数量"]) - Convert.ToDecimal(dts[0]["MRP库存锁定量"]);
                    }
                    else
                    {
                        dec_MRP_计算数量 = dec_有效数量 - Convert.ToDecimal(rr["订单数量"]) * Convert.ToDecimal(ds[0]["欠缺数量"]);
                    }
                    if (dec_MRP_计算数量 < 0)
                    {
                        rr["欠缺数量"] = -dec_MRP_计算数量;
                        if ((Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量) >= Convert.ToDecimal(dts[0]["库存总数"]))
                        {
                            dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["库存总数"]);
                        }
                        else
                        {
                            dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量;
                        }

                        //计算下级原料
                        foreach (DataTable t in dateset.Tables)
                        {
                            if (t.TableName == str_TableName)
                            {
                                DataRow[] drs = t.Select(string.Format("上级物料 = '{0}' and 节点标记 = '叶子'", rr["物料编码"].ToString()));
                                foreach (DataRow dsrs in drs)
                                {
                                    DataRow[] drs_采购 = dt_采购.Select(string.Format("物料编码 = '{0}'", dsrs["物料编码"].ToString()));
                                    drs_采购[0]["物料数量"] = Convert.ToDecimal(drs_采购[0]["物料数量"]) + (-dec_MRP_计算数量) * Convert.ToDecimal(dsrs["BOM数量"]);
                                }
                            }
                        }
                        //计算下级半成品
                        DataRow[] dss = dt_生产.Select(string.Format("上级物料 = '{0}'", rr["物料编码"].ToString()));//销售订单明细号 = '{0}'and rr["销售订单明细号"].ToString(), 
                        if (dss.Length > 0)
                        {
                            foreach (DataRow r1 in dss)
                            {
                                fun_计算_生产(dt_生产, dt_采购, dt_物料数量, r1, str_TableName);
                            }
                        }
                    }
                    else
                    {
                        rr["欠缺数量"] = 0;
                        if ((Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量 - dec_MRP_计算数量) >= Convert.ToDecimal(dts[0]["库存总数"]))
                        {
                            dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["库存总数"]);
                        }
                        else
                        {
                            dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量 - dec_MRP_计算数量;
                        }
                    }
                }
                else
                {
                    rr["欠缺数量"] = 0;
                }
            }
            return rr;
        }

        private DataRow fun_计算_生产(DataRow rr, string str_TableName)
        {
            Decimal dec_MRP_计算数量 = 0;
            try
            {
                DataRow[] dd = dt_物料数量.Select(string.Format("物料编码 = '{0}'", rr["物料编码"].ToString().Trim()));
                if (dd.Length <= 0)
                {
                    string sql_物料数量 = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}'", rr["物料编码"].ToString().Trim());
                    SqlDataAdapter da_物料数量 = new SqlDataAdapter(sql_物料数量, strconn);
                    da_物料数量.Fill(dt_物料数量);
                }
            }
            catch
            {
                string sql_物料数量 = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}'", rr["物料编码"].ToString().Trim());
                SqlDataAdapter da_物料数量 = new SqlDataAdapter(sql_物料数量, strconn);
                da_物料数量.Fill(dt_物料数量);
            }
            DataRow[] dts = dt_物料数量.Select(string.Format("物料编码 = '{0}'", rr["物料编码"].ToString().Trim()));
            Decimal dec_有效数量=0;
            if (dts.Length > 0)
            {
                dec_有效数量 = Convert.ToDecimal(dts[0]["库存总数"]) + Convert.ToDecimal(dts[0]["在途量"]) + Convert.ToDecimal(dts[0]["在制量"]);
            }
            else
            {
                throw new Exception(string.Format("物料{0}未初始化", rr["物料编码"].ToString().Trim()));
            }
            if (rr["物料类型"].ToString() == "产品")
            {
                if (Convert.ToDecimal(dts[0]["MRP库存锁定量"]) > (Decimal)0)
                {
                    dec_MRP_计算数量 = dec_有效数量 - Convert.ToDecimal(rr["订单数量"]) - Convert.ToDecimal(dts[0]["MRP库存锁定量"]);
                }
                else
                {
                    dec_MRP_计算数量 = dec_有效数量 - Convert.ToDecimal(rr["订单数量"]);
                }
                if (dec_MRP_计算数量 < 0)
                {
                    rr["欠缺数量"] = -dec_MRP_计算数量;
                    if ((Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量) >= Convert.ToDecimal(dts[0]["库存总数"]))
                    {
                        dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["库存总数"]);
                    }
                    else
                    {
                        dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量;
                    }
                    //计算一级原料
                    foreach (DataTable t in dateset.Tables)
                    {
                        if (t.TableName == str_TableName)
                        {
                            DataRow[] drs = t.Select(string.Format("上级物料 = '{0}' and 节点标记 = '叶子'", rr["物料编码"].ToString()));
                            foreach (DataRow dsrs in drs)
                            {
                                DataRow[] drs_采购 = dt_采购.Select(string.Format("物料编码 = '{0}'", dsrs["物料编码"].ToString()));
                                drs_采购[0]["物料数量"] = Convert.ToDecimal(drs_采购[0]["物料数量"]) + (-dec_MRP_计算数量) * Convert.ToDecimal(dsrs["BOM数量"]);
                            }
                        }
                    }
                    //计算属于该产品的半成品
                    DataRow[] ds = dt_生产.Select(string.Format("上级物料 = '{0}'", rr["物料编码"].ToString()));//销售订单明细号 = '{0}'and  rr["销售订单明细号"].ToString(), 
                    if (ds.Length > 0)
                    {
                        foreach (DataRow r1 in ds)
                        {
                            fun_计算_生产(r1, str_TableName);
                        }
                    }
                }
                else
                {
                    rr["欠缺数量"] = 0;
                    if ((Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量 - dec_MRP_计算数量) >= Convert.ToDecimal(dts[0]["库存总数"]))
                    {
                        dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["库存总数"]);
                    }
                    else
                    {
                        dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量 - dec_MRP_计算数量;
                    }
                }
            }
            if (rr["物料类型"].ToString() == "半成品")
            {
                DataRow[] ds = dt_生产.Select(string.Format("物料编码 = '{0}'", rr["上级物料"].ToString()));//销售订单明细号 = '{0}'and rr["销售订单明细号"].ToString(),
                if (Convert.ToDecimal(ds[0]["欠缺数量"]) > 0)
                {
                    if (Convert.ToDecimal(dts[0]["MRP库存锁定量"]) > (Decimal)0)
                    {
                        dec_MRP_计算数量 = dec_有效数量 - Convert.ToDecimal(rr["订单数量"]) * Convert.ToDecimal(ds[0]["欠缺数量"]) - Convert.ToDecimal(dts[0]["MRP库存锁定量"]);
                    }
                    else
                    {
                        dec_MRP_计算数量 = dec_有效数量 - Convert.ToDecimal(rr["订单数量"]) * Convert.ToDecimal(ds[0]["欠缺数量"]);
                    }
                    if (dec_MRP_计算数量 < 0)
                    {
                        rr["欠缺数量"] = -dec_MRP_计算数量;
                        if ((Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量) >= Convert.ToDecimal(dts[0]["库存总数"]))
                        {
                            dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["库存总数"]);
                        }
                        else
                        {
                            dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量;
                        }
                       
                        //计算下级原料
                        foreach (DataTable t in dateset.Tables)
                        {
                            if (t.TableName == str_TableName)
                            {
                                DataRow[] drs = t.Select(string.Format("上级物料 = '{0}' and 节点标记 = '叶子'", rr["物料编码"].ToString()));
                                foreach (DataRow dsrs in drs)
                                {
                                    DataRow[] drs_采购 = dt_采购.Select(string.Format("物料编码 = '{0}'", dsrs["物料编码"].ToString()));
                                    drs_采购[0]["物料数量"] = Convert.ToDecimal(drs_采购[0]["物料数量"]) + (-dec_MRP_计算数量) * Convert.ToDecimal(dsrs["BOM数量"]);
                                }
                            }
                        }
                        //计算下级半成品
                        DataRow[] dss = dt_生产.Select(string.Format("上级物料 = '{0}'", rr["物料编码"].ToString()));//销售订单明细号 = '{0}'and rr["销售订单明细号"].ToString(), 
                        if (dss.Length > 0)
                        {
                            foreach (DataRow r1 in dss)
                            {
                                fun_计算_生产(r1, str_TableName);
                            }
                        }
                    }
                    else
                    {
                        rr["欠缺数量"] = 0;
                        if ((Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量 - dec_MRP_计算数量) >= Convert.ToDecimal(dts[0]["库存总数"]))
                        {
                            dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["库存总数"]);
                        }
                        else
                        {
                            dts[0]["MRP库存锁定量"] = Convert.ToDecimal(dts[0]["MRP库存锁定量"]) + dec_有效数量 - dec_MRP_计算数量;
                        }
                    }
                }
                else
                {
                    rr["欠缺数量"] = 0;
                }
            }
            return rr;
        }
        #endregion

        #region 销售待办方法
        //载入未完成数量 > 0 的记录
        private void fun_载入待办()
        {
            dt_待办 = new DataTable();
            dt_待办.Columns.Add("选择", typeof(Boolean));
            dt_待办.Columns.Add("是否已选", typeof(Boolean));
            string sql = string.Format(@"select 销售记录销售订单明细表.*,仓库物料数量表.库存总数,基础数据物料信息表.可售,基础数据物料信息表.物料类型,基础数据物料信息表.原ERP物料编号 
             ,备注    from 销售记录销售订单明细表 
            left join 仓库物料数量表  on 仓库物料数量表.物料编码=销售记录销售订单明细表.物料编码
            left join 基础数据物料信息表 on 基础数据物料信息表.物料编码=销售记录销售订单明细表.物料编码
            where 销售记录销售订单明细表.生效 = 1 and 销售记录销售订单明细表.作废 = 0 and 销售记录销售订单明细表.明细完成 = 0 and 销售记录销售订单明细表.已计算 = 0");
            string sql_大小类 = string.Format("select *  from [基础数据物料类型表] where 计划员='{0}'", CPublic.Var.LocalUserID);
             DataTable dt_关联大类 = CZMaster.MasterSQL.Get_DataTable(sql_大小类, strconn);
            if (dt_关联大类.Rows.Count > 0)
            {
                sql = sql + "and ( ";
                foreach (DataRow dr in dt_关联大类.Rows)
                {
                    sql = sql + string.Format("基础数据物料信息表.{0}='{1}' or ", dr["类型级别"], dr["物料类型名称"]);
                }
                sql = sql.Substring(0, sql.Length - 3);
                sql = sql + ")";
            }
            
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_待办);
            foreach (DataRow r in dt_待办.Rows)
            {
                r["选择"] = false;
            }
            DataView dv1 = new DataView(dt_待办);
            dv1.RowFilter = "未完成数量 > 0";
            gc.DataSource = dv1;
            dt_待办.ColumnChanged += dt_待办_ColumnChanged;
        }
        #endregion

        #region 界面操作
        //选择 没用
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                不用frm销售记录销售单选择界面 fm = new 不用frm销售记录销售单选择界面(dtM);
                fm过往明细 from = new fm过往明细();
                from.Controls.Add(fm);
                fm.Dock = DockStyle.Fill;
                from.Text = "销售记录销售单选择";
                from.ShowDialog();
                fun_选择完后操作(fm);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm销售明细分析界面_fun_载入数据");
            }
        }

        //计算
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                dt_采购.Clear();
                dt_生产.Clear();
                if (dtM.Rows.Count == 0) { }
                else
                {                    
                    int i = 0;
                    foreach (DataRow r in dtM.Rows)
                    {
                        r["POS"] = i++;
                    }
                    dt_物料数量.Clear();
                    fun_生成表一和表二(dtM);
                    frm销售明细分析计划弹窗界面 fm = new frm销售明细分析计划弹窗界面(dt_采购, dt_生产, dt_物料数量, dtM);
                    //fm销售分析弹窗 fmm = new fm销售分析弹窗();
                    //fmm.Controls.Add(fm);
                    //fmm.Text = "采购/生产计划";
                    //fmm.ShowDialog();
                    bl = false;
                    CPublic.UIcontrol.AddNewPage(fm, "采购/生产计划");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_载入数据();
            fun_载入待办();
            dt_生产.Clear();
            dt_采购.Clear();
        }
        #endregion

        private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
        {
            gv.CloseEditor();
            gc.BindingContext[dt_待办].EndCurrentEdit();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                foreach (DataRow dr in dt_待办.Rows)
                {
                    dr["选择"] = true;
                }
            }
            else
            {
                foreach (DataRow dr in dt_待办.Rows)
                {
                    dr["选择"] = false;
                }
            }
        }
    }
}
