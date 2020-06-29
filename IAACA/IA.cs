using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
namespace IAACA
{

    public class IA
    {
        #region 
        string strcon = CPublic.Var.strConn;
        string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

        #endregion
        /// <summary>
        /// 计算入库原材料物料的发出单价
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public DataTable Cal_inv(DateTime t1, DateTime t2)
        {
            DataTable TReturn = new DataTable();
            TReturn.Columns.Add("物料编码");
            TReturn.Columns.Add("规格型号");
            TReturn.Columns.Add("物料名称");
            TReturn.Columns.Add("存货分类编码");
            TReturn.Columns.Add("存货分类");

            
            DataColumn dc = new DataColumn("发出单价", typeof(decimal));
            dc.DefaultValue = 0;
            DataColumn dc1 = new DataColumn("收入单价", typeof(decimal));
            dc.DefaultValue = 0;
            DataColumn dc2 = new DataColumn("累计入库金额", typeof(decimal));
            dc.DefaultValue = 0;
            DataColumn dc3 = new DataColumn("累计入库数量", typeof(decimal)); //累计采购入库数量
            dc.DefaultValue = 0;
            DataColumn dc4 = new DataColumn("上期结存金额", typeof(decimal));
            dc.DefaultValue = 0;
            DataColumn dc5 = new DataColumn("上期结存数量", typeof(decimal));
            dc.DefaultValue = 0;
            TReturn.Columns.Add(dc);
            TReturn.Columns.Add(dc1);
            TReturn.Columns.Add(dc2);
            TReturn.Columns.Add(dc3);
            TReturn.Columns.Add(dc4);
            TReturn.Columns.Add(dc5);

            DataColumn[] pk_R = new DataColumn[1];
            pk_R[0] = TReturn.Columns["物料编码"];
            TReturn.PrimaryKey = pk_R;


            //此处月末结转没问题  但是 如果是次月结转 这个 已开票数量 未开票数量 就不是 月末时间点的值了   
            //因此换一种搜索方式 取固定时间点的累计开票数量
            //string s = string.Format(@"select  入库明细号,rk.物料编码,dd.采购单号,入库量,采购单类型,mx.未税单价 as 采购单价,已开票量,入库量-已开票量 as 未开票量
            //from 采购记录采购单入库明细 rk  left join 采购记录采购单主表 dd  on rk.采购单号 = dd.采购单号
            //left  join 采购记录采购单明细表 mx on mx.采购明细号 = rk.采购单明细号
            //where rk.生效日期 > '{0}' and rk.生效日期 < '{1}' and 采购单类型<>'委外采购' ", t1, t2);
            string s = string.Format(@"select  x.*,isnull(累计开票数量,0) as 已开票量,入库量-isnull(累计开票数量,0) as 未开票量 from (
 select 入库明细号,case when LEFT(入库单号,2)='DW' then CONVERT(decimal(18,6),rk.备注6) else  mx.未税单价  end  as 采购单价 ,rk.物料编码,rk.入库量,rk.物料名称,rk.规格型号,rk.供应商
   from 采购记录采购单入库明细 rk  left join 采购记录采购单主表 dd  on rk.采购单号 = dd.采购单号
    left  join 采购记录采购单明细表 mx on mx.采购明细号 = rk.采购单明细号
    where rk.生效日期 > '{0}' and rk.生效日期 < '{1}' and 采购单类型<>'委外采购')x
  left join (  select  rk.入库明细号,sum(开票数量)累计开票数量,SUM(kptz.未税金额)开票未税金额   from 采购记录采购单入库明细 rk
   left join 采购记录采购单主表 dd  on rk.采购单号 = dd.采购单号
   left join 采购记录采购单明细表 mx on mx.采购明细号 = rk.采购单明细号
   inner join 采购记录采购开票通知单明细表 kptz on kptz.入库明细号 = rk.入库明细号
    where rk.生效日期 > '{0}' and rk.生效日期 < '{1}' and 发票确认日期 >'{0}'    and 发票确认日期<'{1}'   
    and 采购单类型<>'委外采购' and kptz.生效=1 group by rk.入库明细号,rk.物料编码,rk.入库量,mx.未税单价 )y 
on x.入库明细号=y.入库明细号", t1, t2);

            //t_rkkp 为采购入库及开票情况     
            DataTable t_rkkp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //所有当期采购入库需要计算出库单价的 原料  6-25 需要取 结存和 本期入库的
            s = string.Format(@"select rk.物料编码,base.规格型号,base.物料名称,存货分类 ,存货分类编码  from 采购记录采购单入库明细   rk
                   left join 采购记录采购单主表 dd  on rk.采购单号 = dd.采购单号 
                left join 基础数据物料信息表 base on base.物料编码=rk.物料编码
                where rk.生效日期>'{0}' and rk.生效日期 <'{1}' and 采购单类型<>'委外采购' 
                group by rk.物料编码,base.规格型号,base.物料名称,存货分类 ,存货分类编码 ", t1, t2);
            DataTable t_Inv_list = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            //本期开票明细，包含本期入库本期开票 和 往期入库和本期开票
            string s1 = string.Format(@"select  rk.入库明细号,rk.物料编码,dd.采购单号,采购单类型,kptz.开票数量,kptz.单价,kptz.未税单价,kptz.未税金额     from 采购记录采购单入库明细 rk
        left join 采购记录采购单主表 dd  on rk.采购单号 = dd.采购单号
        left join 采购记录采购单明细表 mx on mx.采购明细号 = rk.采购单明细号
        inner join 采购记录采购开票通知单明细表 kptz on kptz.入库明细号 = rk.入库明细号
        where 发票确认日期  > '{0}' and  发票确认日期 < '{1}' and 采购单类型<>'委外采购' and kptz.生效=1 order by 入库明细号", t1, t2);

            DataTable t_当期开票 = CZMaster.MasterSQL.Get_DataTable(s1, strcon);
            //计算累计本期入库金额 和 累计入库数量
            DateTime tx = t1.AddMonths(-1);
            s = string.Format(" select  * from 仓库月出入库结转表 where 年='{0}' and 月='{1}'", tx.Year, tx.Month);
            DataTable t_结转表 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataColumn[] pk_jz = new DataColumn[1];
            pk_jz[0] = t_结转表.Columns["物料编码"];
            t_结转表.PrimaryKey = pk_jz;


            foreach (DataRow dr in t_Inv_list.Rows)
            {
                decimal dec_rksum = 0;
                decimal dec_rkNumSum = 0;
                DataRow[] r_rk = t_rkkp.Select(string.Format("物料编码='{0}'", dr["物料编码"]));

                foreach (DataRow r in r_rk) //此物料当期入库清单  
                {
                    decimal dec_未开票量 = Convert.ToDecimal(r["未开票量"]);
                    if (Convert.ToDecimal(r["已开票量"]) != 0)  //此处计算 开票数量部分的金额合计
                    {
                        DataRow[] r_kplist = t_当期开票.Select(string.Format("入库明细号='{0}'", r["入库明细号"]));
                        foreach (DataRow rr in r_kplist)
                        {
                            // Convert.ToDecimal(rr["开票数量"]) * Convert.ToDecimal(rr["未税单价"]);
                            dec_rksum += Convert.ToDecimal(rr["未税金额"]);
                        }
                        //剩余未开票部分 金额合计
                        dec_rksum += dec_未开票量 * Convert.ToDecimal(r["采购单价"]);
                    }
                    else //此入库单未有过开票 取采购价
                    {
                        dec_rksum += Convert.ToDecimal(r["采购单价"]) * dec_未开票量;

                    }
                    dec_rkNumSum += Convert.ToDecimal(r["入库量"]);
                }

                DataRow r_back = TReturn.NewRow();
                r_back["物料编码"] = dr["物料编码"];
                r_back["物料名称"] = dr["物料名称"];
                r_back["规格型号"] = dr["规格型号"];
                r_back["存货分类"] = dr["存货分类"];
                r_back["存货分类编码"] = dr["存货分类编码"];
                r_back["累计入库金额"] = Math.Round(dec_rksum, 2, MidpointRounding.AwayFromZero);
                r_back["累计入库数量"] = dec_rkNumSum;
                DataRow[] rx = t_结转表.Select(string.Format("物料编码='{0}'", r_back["物料编码"]));
                if (rx.Length > 0)
                {
                    r_back["上期结存金额"] = rx[0]["本月结转金额"];
                    r_back["上期结存数量"] = rx[0]["本月结转数量"];
                }
                TReturn.Rows.Add(r_back);
            }
            //计算 红字回冲 本期开票的往期入库单  冲暂估  and 采购单类型<>'委外采购' 
            //2020-4-1 采购前期退货 按照退货单的单价 这数据没有关联采购单
            s = string.Format(@"  select  物料编码,物料名称,规格型号,存货分类,存货分类编码,SUM(冲暂估金额)冲暂估金额 from (
 select  *, 开票未税金额-未税单价*累计开票数量 as 冲暂估金额 from (
 select  rk.入库明细号,rk.物料编码,base.物料名称,base.规格型号,存货分类,存货分类编码, 
 case when LEFT(rk.入库明细号,2)='DW' then  CONVERT(decimal(18,6),rk.备注6) when  rk.采购单明细号='' and rk.备注1='采购退货' then  CONVERT(decimal(18,6),rk.备注6)   else  mx.未税单价  end as 未税单价
 ,sum(开票数量)累计开票数量,SUM(kptz.未税金额)开票未税金额   from 采购记录采购单入库明细 rk
   left join 采购记录采购单主表 dd  on rk.采购单号 = dd.采购单号
   left join 采购记录采购单明细表 mx on mx.采购明细号 = rk.采购单明细号
   inner join 采购记录采购开票通知单明细表 kptz on kptz.入库明细号 = rk.入库明细号
   left join 基础数据物料信息表 base on base.物料编码=rk.物料编码 
   where  rk.生效日期 < '{0}' and 发票确认日期 >'{0}'  and 发票确认日期<'{1}'   
 and kptz.生效=1 group by rk.入库明细号,rk.采购单明细号,rk.物料编码,base.物料名称,rk.备注1,base.规格型号,存货分类,存货分类编码,rk.备注6,rk.未税单价,mx.未税单价)v)vv 
 group by 物料编码,物料名称,规格型号,存货分类,存货分类编码", t1, t2);
            DataTable t_冲暂估 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            foreach (DataRow r_冲暂估 in t_冲暂估.Rows)
            {
                DataRow[] result = TReturn.Select(string.Format("物料编码='{0}'", r_冲暂估["物料编码"]));

                if (result.Length > 0)
                {
                    result[0]["累计入库金额"] = Math.Round(Convert.ToDecimal(result[0]["累计入库金额"]) + Convert.ToDecimal(r_冲暂估["冲暂估金额"]), 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    DataRow r_back = TReturn.NewRow();
                    r_back["物料编码"] = r_冲暂估["物料编码"];
                    r_back["物料名称"] = r_冲暂估["物料名称"];
                    r_back["规格型号"] = r_冲暂估["规格型号"];
                    r_back["存货分类"] = r_冲暂估["存货分类"];
                    r_back["存货分类编码"] = r_冲暂估["存货分类编码"];
                    r_back["累计入库金额"] = Math.Round(Convert.ToDecimal(r_冲暂估["冲暂估金额"]), 2, MidpointRounding.AwayFromZero);
                    r_back["累计入库数量"] = 0;
                    DataRow[] rx = t_结转表.Select(string.Format("物料编码='{0}'", r_back["物料编码"]));
                    if (rx.Length > 0)
                    {
                        r_back["上期结存金额"] = rx[0]["本月结转金额"];
                        r_back["上期结存数量"] = rx[0]["本月结转数量"];
                    }
                    TReturn.Rows.Add(r_back);
                }
            }

            ///计算其他入库 生产用辅料，仓库盘点,拆旧入库  单价为0，调拨 形态转换,调换,期末结存单价
            ///首先 第一步把其他入库单据的单价赋值上去  
            ///and left(rk.物料编码,2)='01'  限制去除 19- 6-27   and left(rk.物料编码,2)='01'
            //19-7-19 材料出库单/其他出库单往期出库本期红字回冲的 需要计算
            //19-8-7 加入往期退货入库的  
            s = string.Format(@"select  base.物料编码,base.规格型号,base.物料名称,存货分类,存货分类编码  from 其他入库子表 rk
                   left join 基础数据物料信息表 base on base.物料编码 = rk.物料编码
                    where rk.生效日期 > '{0}' and rk.生效日期 < '{1}' 
                    group by base.物料编码,base.规格型号,base.物料名称,存货分类,存货分类编码
                   union 
                   select  base.物料编码,base.规格型号,base.物料名称,存货分类,存货分类编码  from 其他出库子表 rk
                    left join 其他出入库申请主表 rksq on rk.出入库申请单号 =rksq.出入库申请单号 
                   left join 基础数据物料信息表 base on base.物料编码 = rk.物料编码
                    where rk.生效日期 > '{0}' and rk.生效日期 < '{1}'  and 存货核算标记=1   and 原因分类<>'入库倒冲'
                    group by base.物料编码,base.规格型号,base.物料名称,存货分类,存货分类编码 ", t1, t2);
            DataTable t_入库物料 = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            s = string.Format(@" select  物料编码,数量,原因分类,结算单价  from 其他入库子表 rk
                    left join 其他出入库申请主表 rksq on rk.出入库申请单号 =rksq.出入库申请单号 
                    where rk.生效日期 > '{0}' and rk.生效日期 < '{1}' 
                    and 存货核算标记 =1 and 结算单价 is not null 
                   union all
                     select  物料编码,-1*数量 数量,原因分类,结算单价  from 其他出库子表 rk
                    left join 其他出入库申请主表 rksq on rk.出入库申请单号 =rksq.出入库申请单号 
                      where rk.生效日期 > '{0}' and rk.生效日期 < '{1}' 
                     and 存货核算标记 =1 and 原因分类<>'入库倒冲' and 结算单价 is not null  ", t1, t2); //红字回冲的    数量取反
            DataTable t_入库明细 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            foreach (DataRow r in t_入库物料.Rows)
            {
          

                DataRow[] result = TReturn.Select(string.Format("物料编码='{0}'", r["物料编码"]));
                decimal dec_rksum = 0; //累计入库金额
                decimal dec_rkNumSum = 0; //累计入库数量
                if (result.Length > 0)
                {
                    dec_rksum = Convert.ToDecimal(result[0]["累计入库金额"]);
                    dec_rkNumSum = Convert.ToDecimal(result[0]["累计入库数量"]);
                }
                DataRow[] r_list = t_入库明细.Select(string.Format("物料编码='{0}'", r["物料编码"]));
                foreach (DataRow rr in r_list)
                {
                    //   string yy = rr["原因分类"].ToString();
                    dec_rkNumSum += Convert.ToDecimal(rr["数量"]);
                    dec_rksum += Math.Round(Convert.ToDecimal(rr["数量"]) * Convert.ToDecimal(rr["结算单价"]), 2, MidpointRounding.AwayFromZero);

                }
                if (result.Length > 0)
                {
                    result[0]["累计入库金额"] = Math.Round(dec_rksum, 2, MidpointRounding.AwayFromZero);
                    result[0]["累计入库数量"] = dec_rkNumSum;
                }
                else
                {
                    DataRow r_back = TReturn.NewRow();
                    r_back["物料编码"] = r["物料编码"];
                    r_back["物料名称"] = r["物料名称"];
                    r_back["规格型号"] = r["规格型号"];
                    r_back["存货分类"] = r["存货分类"];
                    r_back["存货分类编码"] = r["存货分类编码"];
                    r_back["累计入库金额"] = Math.Round(dec_rksum, 2, MidpointRounding.AwayFromZero);
                    r_back["累计入库数量"] = dec_rkNumSum;
                    DataRow[] rx = t_结转表.Select(string.Format("物料编码='{0}'", r_back["物料编码"]));
                    if (rx.Length > 0)
                    {
                        r_back["上期结存金额"] = rx[0]["本月结转金额"];
                        r_back["上期结存数量"] = rx[0]["本月结转数量"];
                    }
                    TReturn.Rows.Add(r_back);
                }
            }

            //19-7-22 拆单入库 也需要计算
            s = string.Format(@" select a.物料编码,数量,单价,base.物料名称,base.规格型号,base.存货分类,base.存货分类编码 from 仓库出入库明细表 a  
                  left join 基础数据物料信息表 base on base.物料编码= a.物料编码 
                  where 出入库时间 >'{0}' and 出入库时间 <'{1}'  and 明细类型 = '拆单申请入库'  and 实效数量<>0", t1, t2);
            DataTable dt_拆单入 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            foreach (DataRow r in dt_拆单入.Rows)
            {

                DataRow[] result = TReturn.Select(string.Format("物料编码='{0}'", r["物料编码"]));
                decimal dec_rksum = 0; //累计入库金额
                decimal dec_rkNumSum = 0; //累计入库数量
                if (result.Length > 0)
                {
                    dec_rksum = Convert.ToDecimal(result[0]["累计入库金额"]);
                    dec_rkNumSum = Convert.ToDecimal(result[0]["累计入库数量"]);
                }


                dec_rkNumSum += Convert.ToDecimal(r["数量"]);
                dec_rksum += Math.Round(Convert.ToDecimal(r["数量"]) * Convert.ToDecimal(r["单价"]), 2, MidpointRounding.AwayFromZero);

                if (result.Length > 0)
                {
                    result[0]["累计入库金额"] = Math.Round(dec_rksum, 2, MidpointRounding.AwayFromZero);
                    result[0]["累计入库数量"] = dec_rkNumSum;
                }
                else
                {
                    DataRow r_back = TReturn.NewRow();
                    r_back["物料编码"] = r["物料编码"];
                    r_back["物料名称"] = r["物料名称"];
                    r_back["规格型号"] = r["规格型号"];
                    r_back["存货分类"] = r["存货分类"];
                    r_back["存货分类编码"] = r["存货分类编码"];
                    r_back["累计入库金额"] = Math.Round(dec_rksum, 2, MidpointRounding.AwayFromZero);
                    r_back["累计入库数量"] = dec_rkNumSum;
                    DataRow[] rx = t_结转表.Select(string.Format("物料编码='{0}'", r_back["物料编码"]));
                    if (rx.Length > 0)
                    {
                        r_back["上期结存金额"] = rx[0]["本月结转金额"];
                        r_back["上期结存数量"] = rx[0]["本月结转数量"];
                    }
                    TReturn.Rows.Add(r_back);
                }
            }

            s = string.Format(@"select  单号,a.物料编码,发出单价,实效数量,存货分类,存货分类编码,base.物料名称,base.规格型号  from 仓库出入库明细表  a
   left join 销售记录成品出库单明细表 b on a.明细号=b.成品出库单明细号 
   left join 基础数据物料信息表 base on base.物料编码=a.物料编码 
                 where 明细类型 = '销售退货' and 出入库时间> '{0}' and 出入库时间<'{1}' and 发出单价 is not null ", t1, t2);
            DataTable dt_退 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            foreach (DataRow r in dt_退.Rows)
            {

                DataRow[] result = TReturn.Select(string.Format("物料编码='{0}'", r["物料编码"]));
                decimal dec_rksum = 0; //累计入库金额
                decimal dec_rkNumSum = 0; //累计入库数量
                if (result.Length > 0)
                {
                    dec_rksum = Convert.ToDecimal(result[0]["累计入库金额"]);
                    dec_rkNumSum = Convert.ToDecimal(result[0]["累计入库数量"]);
                }


                dec_rkNumSum += Convert.ToDecimal(r["实效数量"]);
                dec_rksum += Math.Round(Convert.ToDecimal(r["实效数量"]) * Convert.ToDecimal(r["发出单价"]), 2, MidpointRounding.AwayFromZero);

                if (result.Length > 0)
                {
                    result[0]["累计入库金额"] = Math.Round(dec_rksum, 2, MidpointRounding.AwayFromZero);
                    result[0]["累计入库数量"] = dec_rkNumSum;
                }
                else
                {
                    DataRow r_back = TReturn.NewRow();
                    r_back["物料编码"] = r["物料编码"];
                    r_back["物料名称"] = r["物料名称"];
                    r_back["规格型号"] = r["规格型号"];
                    r_back["存货分类"] = r["存货分类"];
                    r_back["存货分类编码"] = r["存货分类编码"];
                    r_back["累计入库金额"] = Math.Round(dec_rksum, 2, MidpointRounding.AwayFromZero);
                    r_back["累计入库数量"] = dec_rkNumSum;
                    DataRow[] rx = t_结转表.Select(string.Format("物料编码='{0}'", r_back["物料编码"]));
                    if (rx.Length > 0)
                    {
                        r_back["上期结存金额"] = rx[0]["本月结转金额"];
                        r_back["上期结存数量"] = rx[0]["本月结转数量"];
                    }
                    TReturn.Rows.Add(r_back);
                }
            }
            s = "select  物料编码,规格型号,物料名称,存货分类,存货分类编码 from 基础数据物料信息表 ";

            DataTable t_基础 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            foreach (DataRow tr in t_结转表.Rows)
            {
                if (tr["物料编码"].ToString().Trim().Substring(0, 2).ToString() == "01")
                {
                    DataRow[] ttr = TReturn.Select(string.Format("物料编码='{0}'", tr["物料编码"]));
                    if (ttr.Length > 0) continue;
                    DataRow[] bs = t_基础.Select(string.Format("物料编码='{0}'", tr["物料编码"]));
                    DataRow r_back = TReturn.NewRow();
                    r_back["物料编码"] = tr["物料编码"];
                    r_back["物料名称"] = bs[0]["物料名称"];
                    r_back["规格型号"] = bs[0]["规格型号"];
                    r_back["存货分类"] = bs[0]["存货分类"];
                    r_back["存货分类编码"] = bs[0]["存货分类编码"];
                    r_back["累计入库金额"] = 0;
                    r_back["累计入库数量"] = 0;
                    r_back["发出单价"] = tr["发出单价"];
                    r_back["上期结存金额"] = tr["本月结转金额"];
                    r_back["上期结存数量"] = tr["本月结转数量"];

                    TReturn.Rows.Add(r_back);

                    //DataRow[] t = TReturn.Select(string.Format("物料编码='{0}'", "10990500000009"));
                    //if (t.Length > 0)
                    //{
                    //    break;
                    //}
                }

            }

            foreach (DataRow dr in TReturn.Rows)
            {
                //计算出库单价
                //DataRow[] rx = t_结转表.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                decimal dec_上期结存金额 = 0;
                decimal dec_上期结存数 = 0;

                if (dr["上期结存金额"] == null || dr["上期结存金额"].ToString() == "")
                {

                    dr["上期结存金额"] = 0;
                }
                else
                {
                    dec_上期结存金额 = Convert.ToDecimal(dr["上期结存金额"]);
                }
                if (dr["上期结存数量"] == null || dr["上期结存数量"].ToString() == "")
                {

                    dr["上期结存数量"] = 0;
                }
                else
                {
                    dec_上期结存数 = Convert.ToDecimal(dr["上期结存数量"]);
                }
                decimal dec_rkNumSum = Convert.ToDecimal(dr["累计入库数量"]);
                decimal dec_rksum = Math.Round(Convert.ToDecimal(dr["累计入库金额"]), 2, MidpointRounding.AwayFromZero);
                //if (rx.Length > 0)// 过往有记录
                //{
                //    dec_上期结存金额 = Convert.ToDecimal(rx[0]["本月结转金额"]);
                //    dec_上期结存数 = Convert.ToDecimal(rx[0]["本月结转数量"]);
                //    dec_上期发出单价 = Convert.ToDecimal(rx[0]["发出单价"]);
                //    dec_上期收入单价 = Convert.ToDecimal(rx[0]["收入单价"]);
                //}
                //if (dec_上期结存数 + dec_rkNumSum == 0 || dec_rkNumSum == 0) //本期出库入库数量 累计0 
                //{
                //    dr["发出单价"] = dec_上期发出单价;
                //    dr["收入单价"] = dec_上期收入单价;
                //}
                //else
                //{
                if (dec_上期结存数 + dec_rkNumSum == 0) dr["发出单价"] = 0;
                else dr["发出单价"] = Math.Round((dec_上期结存金额 + dec_rksum) / (dec_上期结存数 + dec_rkNumSum), 6, MidpointRounding.AwayFromZero);
                if (dec_rkNumSum == 0) dr["收入单价"] = 0;
                else dr["收入单价"] = Math.Round(dec_rksum / dec_rkNumSum, 6, MidpointRounding.AwayFromZero);
                //}
            }

            //19-6-28插入先计算半成品  为了不影响原来程序   这边就 略作修改 原成本计算 函数 返回一个 半成品 成本 临时用表 

            DataSet ds_bcp = new DataSet();
            ds_bcp.Tables.Add(TReturn.Copy());
            ds_bcp.Tables.Add(t_结转表);
            DataTable dt_bcp临时 = Cal_半成品(ds_bcp, t1, t2);


            ///计算委外的入库单价 
            //所有当期入库需要计算出库单价的 原料
            s = string.Format(@"select rk.物料编码,base.规格型号,base.物料名称,存货分类 ,存货分类编码  from 采购记录采购单入库明细   rk
                   left join 采购记录采购单主表 dd  on rk.采购单号 = dd.采购单号 
                   left join 基础数据物料信息表 base on base.物料编码=rk.物料编码
                    where rk.生效日期>'{0}' and rk.生效日期 <'{1}' and 采购单类型='委外采购' 
                    group by rk.物料编码,base.规格型号,base.物料名称,存货分类 ,存货分类编码 ", t1, t2);
            DataTable t_Inv_listWW = CZMaster.MasterSQL.Get_DataTable(s, strcon);///

            s = string.Format(@"select  x.*,isnull(累计开票数量,0) as 已开票量,入库量-isnull(累计开票数量,0) as 未开票量,y.开票未税金额 from (
    select 入库明细号,入库单号,case when LEFT(入库单号,2)='DW' then rk.未税单价 else  mx.未税单价  end  as 采购单价, rk.物料编码, rk.入库量, rk.物料名称, rk.规格型号, rk.供应商
    from 采购记录采购单入库明细 rk  left join 采购记录采购单主表 dd  on rk.采购单号 = dd.采购单号
    left join 采购记录采购单明细表 mx on mx.采购明细号 = rk.采购单明细号
    where rk.生效日期 > '{0}' and rk.生效日期 < '{1}' and 采购单类型 = '委外采购')x
    left join(select  rk.入库明细号, sum(开票数量)累计开票数量, SUM(kptz.未税金额)开票未税金额   from 采购记录采购单入库明细 rk
    left join 采购记录采购单主表 dd  on rk.采购单号 = dd.采购单号
     left join 采购记录采购单明细表 mx on mx.采购明细号 = rk.采购单明细号
     inner join 采购记录采购开票通知单明细表 kptz on kptz.入库明细号 = rk.入库明细号
      where rk.生效日期 > '{0}' and rk.生效日期 < '{1}' and 发票确认日期 > '{0}' and 发票确认日期 < '{1}'
     and 采购单类型 = '委外采购' and kptz.生效 = 1 group by rk.入库明细号, rk.物料编码, rk.入库量, mx.未税单价)y on x.入库明细号 = y.入库明细号", t1, t2);
            DataTable t_当期委外入库 = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            //s = string.Format(@"select  a.*,b.备注 as 备注b,d.物料编码 ,d.物料名称 as 名称,d.规格型号,供应商,f.数量 as BOM数量,原因分类  from  其他出库子表 a
            //inner join 其他出入库申请子表 b  on a.出入库申请明细号 = b.出入库申请明细号
            //inner join 其他出入库申请主表 c  on c.出入库申请单号 = b.出入库申请单号
            //inner join 基础数据物料信息表 d  on d.物料编码 = a.物料编码
            //inner join 采购记录采购单明细表 e on e.采购明细号 = b.备注
            //inner join 基础数据物料信息表 fx on fx.物料编码 = e.物料编码
            //left join 委外加工BOM表 f on f.产品编号 = fx.物料编码 and f.子项编号 = a.物料编码
            //where 原因分类 in ('委外加工', '委外补料','委外退料')   and abs(委外已核量)< abs(a.数量) ");

            //这里 取其他出库子表上的单价,本期的取本期算出的;本期没有,取往期结存;若没有取往期发出，递归往上 往期的取往期算完了 赋值上去的
            s = string.Format(@"select hx.*,smx.生效日期,ck.生效日期 as 出库日期,结算单价 from [委外核销明细表] hx
            left join 其他出库子表 ck on hx.其他出库明细号=ck.其他出库明细号
            left join 其他出入库申请子表  smx on smx.出入库申请明细号=ck.出入库申请明细号
            where 核销日期>='{0}' and 核销日期<'{1}' and smx.作废=0 ", t1, t2);
            DataTable t_委外原料 = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            foreach (DataRow dr in t_Inv_listWW.Rows)
            {
                decimal dec_rksum = 0; //加工费 + 材料费
                decimal dec_rkNumSum = 0;
                DataRow[] xx = t_当期委外入库.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                foreach (DataRow xr in xx) //遍历的当期某个编码的 委外入库清单
                {
                    decimal dec_未开票量 = Convert.ToDecimal(xr["未开票量"]);
                    if (Convert.ToDecimal(xr["已开票量"]) > 0)  //此处计算 开票数量部分的金额合计
                    {
                        dec_rksum += Convert.ToDecimal(xr["开票未税金额"]);
                        //剩余未开票部分 金额合计
                        dec_rksum += dec_未开票量 * Convert.ToDecimal(xr["采购单价"]);
                    }
                    else //此入库单未有过开票 取采购价
                    {
                        dec_rksum += Convert.ToDecimal(xr["采购单价"]) * dec_未开票量;
                    }
                    dec_rkNumSum += Convert.ToDecimal(xr["入库量"]);
                    DataRow[] r_委外原料 = t_委外原料.Select(string.Format("入库单号='{0}' ", xr["入库单号"]));
                    decimal dec_材料金额 = 0;
                    foreach (DataRow r_料 in r_委外原料)
                    {
                        //取原料的单价
                        decimal dec_单价 = 0;
                        DateTime time = Convert.ToDateTime(r_料["出库日期"]);
                        if (time >= t1) //本期发出 先取本期算出的发出单价,如果本期没有 上期一定有 
                        {
                            DataRow[] r_单价 = TReturn.Select(string.Format("物料编码='{0}'", r_料["子项编码"]));
                            if (r_单价.Length == 0)
                            {
                                // 19-6-28新增 有可能为05码 本月新增 再取
                                //19-12-03发现 生产领料里面 有委外得物料 且 是本月刚发生得  即上面算半成品得 是没有这个物料得 发出单价得
                                DataRow[] r = dt_bcp临时.Select(string.Format("物料编码='{0}'", r_料["子项编码"]));
                                if (r.Length == 0)
                                {
                                    DataRow[] r_单价2 = t_结转表.Select(string.Format("物料编码='{0}'", r_料["子项编码"]));
                                    if (r_单价2.Length > 0)
                                    {
                                        dec_单价 = Convert.ToDecimal(r_单价2[0]["结存单价"]);   ///本期发出的 本期没有算出 取上期的结存单价还没有取发出单价
                                        if (dec_单价 == 0) dec_单价 = Convert.ToDecimal(r_单价2[0]["发出单价"]);
                                    }
                                }
                                else
                                {
                                    dec_单价 = Convert.ToDecimal(r[0]["发出单价"]);
                                }
                            }
                            else
                            {
                                dec_单价 = Convert.ToDecimal(r_单价[0]["发出单价"]);
                                //本期没有发出说明 上期也没有结存
                                //if (dec_单价 == 0)
                                //{
                                //    dec_单价 = fun_单价(time.AddMonths(-1), r_料["子项编码"].ToString());
                                //}
                            }
                        }
                        else  //往期发出 取往期发出单价   往期发出的 单据上必有单价  ---预防 以前导入的 2018年数据没有
                        {

                            dec_单价 = Convert.ToDecimal(r_料["结算单价"]);
                            //string x = string.Format(@"select  * from 仓库月出入库结转表 where 物料编码='{2}' and 年='{0}' and 月='{1}' ", time.Year, time.Month, r_料["子项编码"].ToString());
                            //DataTable temp = CZMaster.MasterSQL.Get_DataTable(x, strcon);
                            ////temp.rows.count 必定大于0
                            //if (temp.Rows.Count == 0)
                            //{
                            //    string xxx = r_料["子项编码"].ToString();
                            //}
                            //dec_单价 = Convert.ToDecimal(temp.Rows[0]["发出单价"]);                    //往期的 优先取往期的 发出单价 没有再取 结存单价
                            //if (dec_单价 == 0) dec_单价 = Convert.ToDecimal(temp.Rows[0]["结存单价"]);

                        }
                        //dec_材料金额 = Convert.ToDecimal(dr["入库量"]) * Convert.ToDecimal(r_料["BOM数量"]) * dec_单价;
                        dec_材料金额 += Convert.ToDecimal(r_料["物料核销数"]) * dec_单价;
                    }
                    dec_rksum += Math.Round(dec_材料金额, 2, MidpointRounding.AwayFromZero);
                }
                DataRow[] tr = TReturn.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                if (tr.Length > 0)
                {
                    dec_rksum = Convert.ToDecimal(tr[0]["累计入库金额"]) + dec_rksum;
                    dec_rkNumSum = Convert.ToDecimal(tr[0]["累计入库数量"]) + dec_rkNumSum;
                    tr[0]["累计入库金额"] = Math.Round(dec_rksum, 2, MidpointRounding.AwayFromZero);
                    tr[0]["累计入库数量"] = dec_rkNumSum;
                    //计算出库单价
                    DataRow[] rx = t_结转表.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    decimal dec_上期结存金额 = 0;
                    decimal dec_上期结存数 = 0;
                    decimal dec_上期发出单价 = 0;
                    decimal dec_上期收入单价 = 0;
                    if (rx.Length > 0)// 过往有记录
                    {
                        dec_上期结存金额 = Convert.ToDecimal(rx[0]["本月结转金额"]);
                        dec_上期结存数 = Convert.ToDecimal(rx[0]["本月结转数量"]);
                        dec_上期发出单价 = Convert.ToDecimal(rx[0]["发出单价"]);
                        dec_上期收入单价 = Convert.ToDecimal(rx[0]["收入单价"]);
                    }
                    if (dec_上期结存数 + dec_rkNumSum == 0) //本期出库入库数量 累计0 
                    {
                        tr[0]["发出单价"] = dec_上期发出单价;
                        tr[0]["收入单价"] = dec_上期收入单价;
                    }
                    else
                    {
                        tr[0]["发出单价"] = Math.Round((dec_上期结存金额 + dec_rksum) / (dec_上期结存数 + dec_rkNumSum), 6, MidpointRounding.AwayFromZero);
                        tr[0]["收入单价"] = Math.Round(dec_rksum / dec_rkNumSum, 6, MidpointRounding.AwayFromZero);
                    }
                }
                else
                {
                    DataRow r_back = TReturn.NewRow();
                    r_back["物料编码"] = dr["物料编码"];
                    r_back["物料名称"] = dr["物料名称"];
                    r_back["规格型号"] = dr["规格型号"];
                    r_back["存货分类"] = dr["存货分类"];
                    r_back["存货分类编码"] = dr["存货分类编码"];
                    r_back["累计入库金额"] = Math.Round(dec_rksum, 2, MidpointRounding.AwayFromZero);
                    r_back["累计入库数量"] = dec_rkNumSum;

                    //计算出库单价
                    DataRow[] rx = t_结转表.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    decimal dec_上期结存金额 = 0;
                    decimal dec_上期结存数 = 0;
                    decimal dec_上期发出单价 = 0;
                    decimal dec_上期收入单价 = 0;
                    if (rx.Length > 0)// 过往有记录
                    {
                        dec_上期结存金额 = Convert.ToDecimal(rx[0]["本月结转金额"]);
                        dec_上期结存数 = Convert.ToDecimal(rx[0]["本月结转数量"]);
                        dec_上期发出单价 = Convert.ToDecimal(rx[0]["发出单价"]);
                        dec_上期收入单价 = Convert.ToDecimal(rx[0]["收入单价"]);
                    }
                    if (dec_上期结存数 + dec_rkNumSum == 0) //本期出库入库数量 累计0 
                    {
                        r_back["发出单价"] = dec_上期发出单价;
                        r_back["收入单价"] = dec_上期收入单价;
                    }
                    else
                    {
                        r_back["发出单价"] = Math.Round((dec_上期结存金额 + dec_rksum) / (dec_上期结存数 + dec_rkNumSum), 6, MidpointRounding.AwayFromZero);
                        r_back["收入单价"] = Math.Round(dec_rksum / dec_rkNumSum, 6, MidpointRounding.AwayFromZero);
                    }
                    TReturn.Rows.Add(r_back);
                }
            }

            #region   19-8-6 形态转换和新老编码库存调整 需要用算出来发出单价赋值给对应的入库然后重新计算加权平均值

            //DataTable temp = new DataTable();
            //temp.Columns.Add("物料编码");
            //temp.Columns.Add("累计数量", typeof(decimal));
            //temp.Columns.Add("累计金额", typeof(decimal));
            List<string> li = new List<string>();
            s = string.Format(@"select x.*,b.物料编码 转换前编码,b.形态转换明细号 as 转换前明细号   from (
      select a.形态转换单号,a.形态转换明细号,类型,组号,a.物料编码,base.物料名称,base.规格型号,base.存货分类,base.存货分类编码,a.数量,单价 from 销售形态转换子表 a
        left join 基础数据物料信息表 base on base.物料编码 =a.物料编码
    left join 销售形态转换主表 b on a.形态转换单号=b.形态转换单号 
left join  仓库出入库明细表  crmx on crmx.明细号 =形态转换明细号
    where b.审核日期>'{0}' and b.审核日期<'{1}' and 类型='转换后' )x 
    left join 销售形态转换子表 b on b.形态转换单号=x.形态转换单号 and x.组号=b.组号 
    and  b.类型='转换前' ", t1, t2);
            DataTable dt_形态转换 = CZMaster.MasterSQL.Get_DataTable(s, strcon);


            foreach (DataRow dr in dt_形态转换.Rows)
            {

                DataRow[] result = TReturn.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                decimal dec_rksum = 0; //累计入库金额
                decimal dec_rkNumSum = 0; //累计入库数量
                if (result.Length > 0)
                {
                    dec_rksum = Convert.ToDecimal(result[0]["累计入库金额"]);
                    dec_rkNumSum = Convert.ToDecimal(result[0]["累计入库数量"]);
                }
                dec_rkNumSum += Convert.ToDecimal(dr["数量"]);

                decimal dec_zh = 0;
                if (dr["单价"] != null && dr["单价"].ToString() != "" && Convert.ToDecimal(dr["单价"]) != 0)
                {
                    dec_zh = Convert.ToDecimal(dr["单价"]);
                }
                else
                {
                    DataRow[] result_zh = TReturn.Select(string.Format("物料编码='{0}'", dr["转换前编码"])); //这里不一定有成品 所以这里只能先算原材料  这里原材料肯定有

                    if (result_zh.Length > 0)
                    {
                        dec_zh = Convert.ToDecimal(result_zh[0]["发出单价"]);
                    }
                    else
                    {
                        DataRow[] rx = t_结转表.Select(string.Format("物料编码='{0}'", dr["转换前编码"]));



                        if (rx.Length > 0)
                            dec_zh = Convert.ToDecimal(rx[0]["结存单价"]);
                        else
                        {
                            DataRow[] r_ls = dt_bcp临时.Select(string.Format("物料编码='{0}'", dr["转换前编码"]));
                            if (r_ls.Length > 0)
                            {
                                dec_zh = Convert.ToDecimal(r_ls[0]["发出单价"]);
                            }
                            else
                            {
                                string xxxx = dr["转换前编码"].ToString();

                            }

                        }

                    }
                    dr["单价"] = dec_zh;
                }
                dec_rksum += Math.Round(Convert.ToDecimal(dr["数量"]) * dec_zh, 2, MidpointRounding.AwayFromZero);
                if (result.Length > 0)
                {
                    result[0]["累计入库金额"] = Math.Round(dec_rksum, 2, MidpointRounding.AwayFromZero);
                    result[0]["累计入库数量"] = dec_rkNumSum;
                }
                else
                {
                    DataRow r_back = TReturn.NewRow();
                    r_back["物料编码"] = dr["物料编码"];
                    r_back["物料名称"] = dr["物料名称"];
                    r_back["规格型号"] = dr["规格型号"];
                    r_back["存货分类"] = dr["存货分类"];
                    r_back["存货分类编码"] = dr["存货分类编码"];
                    r_back["累计入库金额"] = Math.Round(dec_rksum, 2, MidpointRounding.AwayFromZero);
                    r_back["累计入库数量"] = dec_rkNumSum;
                    DataRow[] rx = t_结转表.Select(string.Format("物料编码='{0}'", r_back["物料编码"]));
                    if (rx.Length > 0)
                    {
                        r_back["上期结存金额"] = rx[0]["本月结转金额"];
                        r_back["上期结存数量"] = rx[0]["本月结转数量"];
                    }
                    TReturn.Rows.Add(r_back);
                }
                if (!li.Contains(dr["物料编码"].ToString()))
                    li.Add(dr["物料编码"].ToString());

            }



            //foreach (DataRow dr in dt_新老.Rows)
            //{
            //    DataRow[] result = TReturn.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
            //    decimal dec_rksum = 0; //累计入库金额
            //    decimal dec_rkNumSum = 0; //累计入库数量
            //    if (result.Length > 0)
            //    {
            //        dec_rksum = Convert.ToDecimal(result[0]["累计入库金额"]);
            //        dec_rkNumSum = Convert.ToDecimal(result[0]["累计入库数量"]);
            //    }
            //    dec_rkNumSum += Convert.ToDecimal(dr["数量"]);
            //    DataRow[] result_xl = TReturn.Select(string.Format("物料编码='{0}'", dr["旧编码"])); //这里不一定有成品 所以这里只能先算原材料  这里原材料肯定有
            //    decimal dec_xl = 0;
            //    if (result_xl.Length > 0)
            //    {
            //        dec_xl = Convert.ToDecimal(result_xl[0]["发出单价"]);
            //    }
            //    else
            //    {
            //        DataRow[] rx = t_结转表.Select(string.Format("物料编码='{0}'", dr["旧编码"]));
            //        dec_xl = Convert.ToDecimal(rx[0]["结存单价"]);
            //    }
            //    dr["单价"] = dec_xl;
            //    dec_rksum += Math.Round(Convert.ToDecimal(dr["数量"]) * dec_xl, 2, MidpointRounding.AwayFromZero);
            //    if (result.Length > 0)
            //    {
            //        result[0]["累计入库金额"] = Math.Round(dec_rksum, 2, MidpointRounding.AwayFromZero);
            //        result[0]["累计入库数量"] = dec_rkNumSum;
            //    }
            //    else
            //    {
            //        DataRow r_back = TReturn.NewRow();
            //        r_back["物料编码"] = dr["物料编码"];
            //        r_back["物料名称"] = dr["物料名称"];
            //        r_back["规格型号"] = dr["规格型号"];
            //        r_back["存货分类"] = dr["存货分类"];
            //        r_back["存货分类编码"] = dr["存货分类编码"];
            //        r_back["累计入库金额"] = Math.Round(dec_rksum, 2, MidpointRounding.AwayFromZero);
            //        r_back["累计入库数量"] = dec_rkNumSum;
            //        DataRow[] rx = t_结转表.Select(string.Format("物料编码='{0}'", r_back["物料编码"]));
            //        if (rx.Length > 0)
            //        {
            //            r_back["上期结存金额"] = rx[0]["本月结转金额"];
            //            r_back["上期结存数量"] = rx[0]["本月结转数量"];
            //        }
            //        TReturn.Rows.Add(r_back);
            //    }
            //    if (!li.Contains(dr["物料编码"].ToString()))
            //        li.Add(dr["物料编码"].ToString());
            //}
            ///需要加入 更新单据单价 方便 出入库结存的 计算
            string Path_xtzh = DesktopPath + @"\形态转换明细.xlsx";
            // string Path_新老 = DesktopPath + @"\新老编码库存调整.xlsx";
            if (!File.Exists(Path_xtzh))
            {
                File.Create(Path_xtzh).Dispose(); ;
            }
            //if (!File.Exists(Path_新老))
            //{
            //    File.Create(Path_新老);
            //}

            //Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory
            ERPorg.Corg.TableToExcel(dt_形态转换, Path_xtzh);
            // ERPorg.Corg.TableToExcel(dt_新老, Path_新老);
            foreach (string wl in li)
            {
                decimal Pre_Bal = 0;
                decimal Pre_Bal_Amt = 0;
                DataRow[] rx = t_结转表.Select(string.Format("物料编码='{0}'", wl));
                if (rx.Length > 0)
                {
                    Pre_Bal = Convert.ToDecimal(rx[0]["本月结转数量"]);
                    Pre_Bal_Amt = Convert.ToDecimal(rx[0]["本月结转金额"]);
                }
                DataRow[] result = TReturn.Select(string.Format("物料编码='{0}'", wl));

                result[0]["发出单价"] = Math.Round((Pre_Bal_Amt + Convert.ToDecimal(result[0]["累计入库金额"])) / (Pre_Bal + Convert.ToDecimal(result[0]["累计入库数量"])), 6, MidpointRounding.AwayFromZero);
                result[0]["收入单价"] = Math.Round(Convert.ToDecimal(result[0]["累计入库金额"]) / Convert.ToDecimal(result[0]["累计入库数量"]), 6, MidpointRounding.AwayFromZero);
            }
            #endregion
            TReturn.PrimaryKey = null;
            fun_返写存货单据单价(TReturn, dt_形态转换, t1, t2);
            return TReturn;
        }
        //往上期取结存单价 
        //19-7-1不需要递归 基本废了 只要用取上期结存 就行了
        private decimal fun_单价(DateTime t, string str_wl)
        {
            decimal dec = 0;
            string x = string.Format(@"select  * from 仓库月出入库结转表 where 物料编码='{2}' and 年='{0}' and 月='{1}' ", t.Year, t.Month, str_wl);
            DataTable temp = CZMaster.MasterSQL.Get_DataTable(x, strcon);
            if (temp.Rows.Count == 0 && (t.Year > 2018 || t.Month >= 12))
            {
                dec = fun_单价(t.AddMonths(-1), str_wl);
            }
            else
            {
                if (temp.Rows.Count > 0)
                {
                    dec = Convert.ToDecimal(temp.Rows[0]["结存单价"]);
                    if (dec == 0 && Convert.ToDecimal(temp.Rows[0]["本月结转数量"]) == 0)
                        dec = Convert.ToDecimal(temp.Rows[0]["发出单价"]);
                    else return dec;
                    if (dec == 0 && (t.Year >= 2018 || t.Month >= 12))
                    {
                        dec = fun_单价(t.AddMonths(-1), str_wl);
                    }
                }
            }
            return dec;
        }
        /// <summary>
        /// 不需要递归 直接取上期结存
        /// </summary>
        /// <param name="t"></param>
        /// <param name="str_wl"></param>
        /// <returns></returns>
        private decimal fun_单价x(DateTime t, string str_wl)
        {
            decimal dec = 0;
            string x = string.Format(@"select  * from 仓库月出入库结转表 where 物料编码='{2}' and 年='{0}' and 月='{1}' ", t.Year, t.Month, str_wl);
            DataTable temp = CZMaster.MasterSQL.Get_DataTable(x, strcon);
            if (temp.Rows.Count > 0)
            {
                if (temp.Rows[0]["结存单价"] == null || temp.Rows[0]["结存单价"].ToString() == "") dec = 0;
                else dec = Convert.ToDecimal(temp.Rows[0]["结存单价"]);

            }
            return dec;
        }

        public void fun_返写存货单据单价(DataTable t, DataTable t_xtzh, DateTime time1, DateTime time2)
        {
            string s = string.Format("delete C_存货核算物料单价表 where 年={0} and 月={1}", time1.Year, time1.Month);
            CZMaster.MasterSQL.ExecuteSQL(s, strcon);

            s = string.Format("delete C_存货核算物料单价表_bak where 年={0} and 月={1}", time1.Year, time1.Month);
            CZMaster.MasterSQL.ExecuteSQL(s, strcon);

            s = "select * from C_存货核算物料单价表 where 1=2";
            DataTable t_save = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            foreach (DataRow dr in t.Rows)
            {
                DataRow r = t_save.NewRow();
                r["物料编码"] = dr["物料编码"];
                r["发出单价"] = dr["发出单价"];
                r["收入单价"] = dr["收入单价"];
                r["累计入库金额"] = dr["累计入库金额"];
                r["累计入库数量"] = dr["累计入库数量"];
                r["年"] = time1.Year;
                r["月"] = time1.Month;
                t_save.Rows.Add(r);
            }
            string s_bak = "select * from C_存货核算物料单价表_bak where 1=2";
            DataTable t_save_bak = CZMaster.MasterSQL.Get_DataTable(s_bak, strcon);
            foreach (DataRow dr in t.Rows)
            {
                DataRow r = t_save_bak.NewRow();
                r["物料编码"] = dr["物料编码"];
                r["发出单价"] = dr["发出单价"];
                r["收入单价"] = dr["收入单价"];
                r["累计入库金额"] = dr["累计入库金额"];
                r["累计入库数量"] = dr["累计入库数量"];
                r["年"] = time1.Year;
                r["月"] = time1.Month;
                t_save_bak.Rows.Add(r);
            }

            DataTable dt_xt = new DataTable();
            string x_xt = string.Format("select  * from 仓库出入库明细表 where 明细类型 in ('形态转换出库','形态转换入库') and 出入库时间>'{0}' and 出入库时间<'{1}'", time1, time2);
            dt_xt = CZMaster.MasterSQL.Get_DataTable(x_xt, strcon);
            foreach (DataRow dr in t_xtzh.Rows)
            {
                DataRow[] r_after = dt_xt.Select(string.Format("明细号='{0}'", dr["形态转换明细号"].ToString()));
                r_after[0]["单价"] = dr["单价"];
                DataRow[] r_before = dt_xt.Select(string.Format("明细号='{0}'", dr["转换前明细号"].ToString()));
                r_before[0]["单价"] = dr["单价"];
            }

            //   string xl = string.Format(@"select  a.* from 其他入库子表 a
            //   left join 其他出入库申请主表 b on a.出入库申请单号 =b.出入库申请单号 
            //   where a.生效日期>'{0}' and a.生效日期<'{1}' and 原因分类='新老编码库存调整'", time1, time2);
            //            DataTable dt_new = CZMaster.MasterSQL.Get_DataTable(xl, strcon);
            //            foreach (DataRow r in dt_新老.Rows)
            //            {
            //                DataRow[] rr = dt_new.Select(string.Format("物料编码='{0}'", r["物料编码"]));
            //                foreach (DataRow r1 in rr)
            //                {
            //                    r1["结算单价"] = r["单价"];
            //                }
            //            }
            //            string lao = string.Format(@"select  a.* from 其他出库子表 a
            //left join 其他出入库申请主表 b on a.出入库申请单号 =b.出入库申请单号 
            //where a.生效日期>'{0}' and a.生效日期<'{1}' and 原因分类='新老编码库存调整'", time1, time2);
            //            DataTable dt_Old = CZMaster.MasterSQL.Get_DataTable(xl, strcon);
            //            foreach (DataRow r in dt_新老.Rows)
            //            {
            //                DataRow[] rr = dt_Old.Select(string.Format("物料编码='{0}'", r["旧编码"]));
            //                foreach (DataRow r1 in rr)
            //                {
            //                    r1["结算单价"] = r["单价"];
            //                }
            //            }

            //SqlConnection conn = new SqlConnection(strcon);
            //SqlCommand cmd = new SqlCommand(s, conn);
            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //new SqlCommandBuilder(da);
            //da.Update(t_save);

            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction cchhs = conn.BeginTransaction("chhs");
            try
            {
                SqlCommand cmd = new SqlCommand(s, conn, cchhs);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    new SqlCommandBuilder(da);
                    da.Update(t_save);
                }
                cmd = new SqlCommand(s_bak, conn, cchhs);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    new SqlCommandBuilder(da);
                    da.Update(t_save_bak);
                }


                cmd = new SqlCommand(x_xt, conn, cchhs);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt_xt);

                }
                //xl = "select * from 其他出库子表 where 1=2 ";
                //cmd = new SqlCommand(xl, conn, cchhs);
                //using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                //{
                //    new SqlCommandBuilder(da);
                //    da.Update(dt_Old);

                //}
                //xl = "select * from 其他入库子表 where 1=2 ";
                //cmd = new SqlCommand(xl, conn, cchhs);
                //using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                //{
                //    new SqlCommandBuilder(da);
                //    da.Update(dt_new);

                //}

                cchhs.Commit();
            }
            catch (Exception ex)
            {
                cchhs.Rollback();
                throw ex;
            }


        }

        /// <summary>
        /// 3.t_成本->C_存货核算物料单价表
        /// 2.t_本期在制-> 
        /// 1.t_工单->C_工单当期费用表
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        public void fun_保存过程数量(DataSet ds, DateTime time1, DateTime time2)
        {
            //ds.Tables[2] 成本
            string s = string.Format("delete C_存货核算物料单价表 where 年={0} and 月={1}", time1.Year, time1.Month);
            CZMaster.MasterSQL.ExecuteSQL(s, strcon);
            string s1 = "select * from C_存货核算物料单价表 where 1=2";
            DataTable t_save = CZMaster.MasterSQL.Get_DataTable(s1, strcon);
            foreach (DataRow dr in ds.Tables[2].Rows)
            {
                DataRow r = t_save.NewRow();
                r["物料编码"] = dr["物料编码"];
                r["发出单价"] = dr["发出单价"];
                r["收入单价"] = dr["收入单价"];
                r["累计入库金额"] = dr["累计入库金额"];
                r["累计入库数量"] = dr["累计入库数量"];
                r["年"] = time1.Year;
                r["月"] = time1.Month;
                t_save.Rows.Add(r);
            }

            s = string.Format("delete C_工单 where 年={0} and 月={1}", time1.Year, time1.Month);
            CZMaster.MasterSQL.ExecuteSQL(s, strcon);
            string s2 = "select * from C_工单 where 1=2";
            DataTable t_gd = CZMaster.MasterSQL.Get_DataTable(s2, strcon);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DataRow r = t_gd.NewRow();
                r["生产工单号"] = dr["生产工单号"];
                r["生产工单类型"] = dr["生产工单类型"];
                r["物料编码"] = dr["物料编码"];
                r["生产数量"] = dr["生产数量"];
                r["当期入库量"] = dr["当期入库量"];
                r["累计入库量"] = dr["累计入库量"];
                r["累计报废数"] = dr["累计报废数"];
                r["当期报废数"] = dr["当期报废数"];
                r["关闭"] = dr["关闭"];
                r["期末数量"] = dr["期末数量"];
                r["当期完成数量"] = dr["当期完成数量"];
                r["入库单价"] = dr["入库单价"];
                r["材料金额"] = dr["材料金额"];
                r["总金额"] = dr["总金额"];
                r["辅材分摊"] = dr["辅材分摊"];
                r["工时"] = dr["工时"];
                r["工单工时"] = dr["工单工时"];
                r["制造费用"] = dr["制造费用"];
                r["人工费用"] = dr["人工费用"];
                r["软件费用"] = dr["软件费用"];
                r["年"] = time1.Year;
                r["月"] = time1.Month;
                t_gd.Rows.Add(r);
            }
            s = string.Format("delete C_工单当期耗用 where 年={0} and 月={1}", time1.Year, time1.Month);
            CZMaster.MasterSQL.ExecuteSQL(s, strcon);
            string s3 = "select * from C_工单当期耗用 where 1=2";
            DataTable t_gdhy = CZMaster.MasterSQL.Get_DataTable(s3, strcon);
            foreach (DataRow dr in ds.Tables[1].Rows)
            {

                DataRow r = t_gdhy.NewRow();
                r["生产工单号"] = dr["生产工单号"];
                r["生产数量"] = dr["生产数量"];
                r["产品编码"] = dr["产品编码"];
                r["本期入库数量"] = dr["本期入库数量"];
                r["累计报废数"] = dr["累计报废数"];
                r["当期报废数"] = dr["当期报废数"];
                r["累计入库数量"] = dr["累计入库数量"];
                r["子项编码"] = dr["子项编码"];
                r["本期耗用数量"] = dr["本期耗用数量"];
                r["子项期初数"] = dr["子项期初数"];
                r["子项期初金额"] = dr["子项期初金额"];
                r["子项当期领用"] = dr["子项当期领用"];
                r["子项在制数"] = dr["子项在制数"];
                r["发出单价"] = dr["发出单价"];
                r["耗用单价"] = dr["耗用单价"];

                r["年"] = time1.Year;
                r["月"] = time1.Month;
                t_gdhy.Rows.Add(r);
            }
            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction cchhs = conn.BeginTransaction("chhs");
            try
            {
                SqlCommand cmd = new SqlCommand(s1, conn, cchhs);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    new SqlCommandBuilder(da);
                    da.Update(t_save);
                }



                cmd = new SqlCommand(s2, conn, cchhs);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    new SqlCommandBuilder(da);
                    da.Update(t_gd);
                }
                cmd = new SqlCommand(s3, conn, cchhs);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    new SqlCommandBuilder(da);
                    da.Update(t_gdhy);
                }

                cchhs.Commit();
            }
            catch (Exception ex)
            {
                cchhs.Rollback();
                throw ex;
            }






        }
        ///成本核算  计算半成品 和成品得 入库单价  然后 再计算 出库单价  此处需要递归
        /////DataTable dt_存货核算
        public DataSet Cal_成本(DataSet ds, DateTime t1, DateTime t2, decimal dec_辅材, decimal dec_制造, decimal dec_人工)
        {
            try
            {
                DataSet ds_return = new DataSet();

                DataTable dt_存货核算 = ds.Tables[0];
                DataTable dt_结转 = ds.Tables[1];
                string s = string.Format(@"select  * from [财务即时库存记录] where 1=2");
                DataTable t_本期在制 = CZMaster.MasterSQL.Get_DataTable(s, strcon);//空的dt 更新 本期在制 
                DateTime t0 = t1.AddMonths(-1);
                //期初在制
                //19-8-12  入库倒冲加入 存货核算标记=0 的 然后 =1的是 以前5、6月份入库了但是没有入库倒冲的 在7月份集中处理了 
                s = string.Format(@"select  * from [财务即时库存记录] where 时间>'{0}' and 时间<'{1}' and 核算标记=1", t0, t1);
                DataTable t_上期在制 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                s = string.Format(@"
            declare @tabel Table
            (产品编码 [nvarchar](20) 
            ,子项编码 [nvarchar](20) 
           ,数量 [decimal](18,6) );
        with temp_bom(z,产品编码,子项编码,仓库号,仓库名称,wiptype,子项类型,数量,bom类型,优先级,bom_level ) as
       (select 产品编码 as z,产品编码,子项编码,仓库号,仓库名称,WIPType,子项类型,数量,bom类型,优先级,1 as level from 基础数据物料BOM表   
        union all 
          select b.产品编码 as z, a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型, convert(decimal(18, 6),a.数量*b.数量) as 数量,
           a.bom类型,a.优先级,b.bom_level+1  from 基础数据物料BOM表 a
          inner join temp_bom b on a.产品编码=b.子项编码 where b.wiptype='虚拟') 
          
          insert @tabel(产品编码,子项编码, 数量)
           select  z,子项编码, sum(数量)数量 from  temp_bom    where wiptype ='入库倒冲' and 优先级=1
          group by z  ,子项编码  

     select 生产工单号,领料类型,SUM(待领料总量)待领料总量,SUM(已领) 已领,CONVERT(decimal(18,6),sum(BOM数量)) BOM数量,物料编码,产品编码,生产数量 from (
       select  mx.生产工单号,领料类型 ,待领料总量,已领,BOM数量,物料编码,产品编码 ,生产数量  from 生产记录生产工单待领料明细表  mx
      left join 生产记录生产工单待领料主表 zb on mx.待领料单号 =zb.待领料单号 
      left join (select  待领料单明细号,SUM(领料数量)已领 from 生产记录生产领料单明细表 where 生效日期 >'{1}' and 生效日期<'{2}' group by 待领料单明细号)yl 
      on  yl.待领料单明细号=mx.待领料单明细号        
       where 领料类型<>'生产补料' and mx.生产工单类型<>'生产补料'
            and mx.生产工单号 in ( select  生产工单号  from (
           select gd.生产工单号,生产工单类型,物料编码,生产数量,isnull(累计入库量,0)累计入库量,ISNULL(jy.报废数,0)报废数,gd.关闭  from 生产记录生产工单表 gd
           left join (select 生产工单号,sum(入库数量)累计入库量 from 生产记录成品入库单明细表 where  生效日期>'{1}' and 生效日期<'{2}' group by 生产工单号)x 
           on x.生产工单号=gd.生产工单号 
           left join (select  生产工单号,sum(报废数)报废数  from 生产记录生产检验单主表 where 报废数 >0 and  生效日期>'{1}' and 生效日期<'{2}' group by 生产工单号)jy
           on jy.生产工单号=gd.生产工单号
            where   生效日期>'{1}' and 生效日期<'{2}' )y where  (累计入库量>0 or 关闭 =0)
            union  select  生产工单号 from 财务即时库存记录 where 时间>'{0}' and 时间<'{1}' 
            union  select  生产工单号 from 生产记录生产领料单明细表 where  生效日期>'{1}' and 生效日期<'{2}'  group by 生产工单号   
            union  select  生产工单号 from 生产记录成品入库单明细表 where  生效日期>'{1}' and 生效日期<'{2}'  group by 生产工单号 
            union   select  工单号 as 生产工单号  from 工单返库单主表 where 日期>'{1}' and 日期<'{2}' ) )gdbom   group by 生产工单号,领料类型,物料编码,产品编码,生产数量 
          union 
         select 生产工单号,'入库倒冲' as 领料类型,SUM(待领料总量)待领料总量,SUM(已领) 已领,BOM数量,物料编码,产品编码,生产数量 from (
     select  a.备注 as 生产工单号 ,a.数量 as 待领料总量,a.数量 as 已领,k.数量 as BOM数量,a.物料编码,gd.物料编码 as 产品编码,生产数量   from 其他出库子表 a
     left join 其他出入库申请主表   b on a.出入库申请单号=b.出入库申请单号
     left join 生产记录生产工单表 gd on gd.生产工单号 =a.备注
    left join  @tabel  k  on k.产品编码=gd.物料编码 and 子项编码=a.物料编码 
     where  存货核算标记 =0 and 原因分类 ='入库倒冲' and a.生效日期 >'{1}' and a.生效日期<'{2}')tt group by 生产工单号,BOM数量,物料编码,产品编码,生产数量 
      union 
       select  ff.工单号 生产工单号,'返工退料' 领料类型,-SUM(返库数量) 待领料总量,-SUM(返库数量)已领,SUM(返库数量)/avg(生产数量) BOM数量,ff.物料编码,gd.物料编码 as 产品编码,生产数量 
        from 工单返库单明细表 ff
         left join 工单返库单主表 f on f.退料单号=ff.退料单号
		 left join 工单退料申请表 fk on fk.待退料号=ff.待退料号
		 left join  生产记录生产工单表 gd on gd.生产工单号=ff.工单号
        where  ff.日期 >'{1}' and ff.日期<'{2}'  and 退料类型='返工退料' group by ff.工单号,ff.物料编码,gd.物料编码,生产数量", t0, t1, t2); //这里入库倒冲的还没有放进去 导致下面算的时候 漏了 入库倒冲的一块
                DataTable t_工单bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                //19-11-6
           

                DataColumn[] pk1 = new DataColumn[3];
                pk1[0] = t_工单bom.Columns["生产工单号"];
                pk1[1] = t_工单bom.Columns["物料编码"];
                pk1[2] = t_工单bom.Columns["领料类型"];
                t_工单bom.PrimaryKey = pk1;


                foreach (DataRow dr in t_工单bom.Rows)
                {
                    if (dr["BOM数量"] == DBNull.Value || Convert.ToDecimal(dr["BOM数量"]) == 0)
                    {
                        dr["BOM数量"] = Convert.ToDecimal(dr["待领料总量"]) / Convert.ToDecimal(dr["生产数量"]);
                    }
                }
           

                ///所有需要计算得工单  只需要修改后半个生效日期   5 -1为 正式启用系统时间
                ///19-7-17 这里少了 往期生效 当时往期没有领料的工单 需要加进去
                ///   or gd.生产工单号 ='MO1907180015' 
 //               s = string.Format(@"  select *,当期入库量 as 当期完成数量 from (
 //          select  *,case  when 关闭=1 then 0 else 生产数量-累计入库量-累计报废数  end as  期末数量   from (
 //          select gd.生产工单号,生产工单类型,物料编码,生产数量,ISNULL(当期入库量,0)当期入库量,isnull(累计入库量,0)累计入库量,ISNULL(ljjy.累计报废数,0)累计报废数
 //          ,ISNULL(jy.当期报废数,0)当期报废数,case when  gd.关闭=1 and gd.关闭日期<'{2}' then 1 else 0 end as 关闭  from 生产记录生产工单表 gd
 //          left join (select 生产工单号,sum(入库数量)当期入库量 from 生产记录成品入库单明细表 where  生效日期>'{1}' and 生效日期<'{2}' group by 生产工单号)x 
 //          on x.生产工单号=gd.生产工单号 
 //              left join (select 生产工单号,sum(入库数量)累计入库量 from 生产记录成品入库单明细表 where  生效日期>'2019-5-1' and 生效日期<'{2}' group by 生产工单号)lj
 //          on lj.生产工单号=gd.生产工单号 
 //          left join (select  生产工单号,sum(报废数)累计报废数  from 生产记录生产检验单主表 where 报废数 >0 and  生效日期>'2019-5-1' and 生效日期<'{2}' group by 生产工单号)ljjy
 //          on ljjy.生产工单号=gd.生产工单号
 //           left join (select  生产工单号,sum(报废数)当期报废数  from 生产记录生产检验单主表 where 报废数 >0 and  生效日期>'{1}' and 生效日期<'{2}' group by 生产工单号)jy
 //          on jy.生产工单号=gd.生产工单号
 //           where   (生效日期>'{1}' and 生效日期<'{2}') 

 //  or gd.生产工单号 in  (  select  生产工单号  from (
 //select  生产工单号 from 财务即时库存记录 where 时间>'{0}' and 时间<'{1}' group by 生产工单号 
 //union 
 //  select 相关单号  as 生产工单号  from  仓库出入库明细表 where  明细类型 in ('领料出库','入库倒冲') 
 //           and 出入库时间>'{1}' and 出入库时间<'{2}'  group by 相关单号 
 //union
 //        select 相关单号  as 生产工单号  from 仓库出入库明细表 where 明细类型 in ('返工退料','工单关闭退料','工单退料')
 //           and 出入库时间>'{1}' and 出入库时间<'{2}' group by 相关单号 )x  )

        
 //          )y )yy  ", t0, t1, t2);   //0 为上期初  1 为 上期末 本期初，2 为 本期末  5-1 为系统起始时间
 //                                    //where  (累计入库量>0 or 关闭 =0)   19-7-24去除


                s = $@"declare @t1 Table (生产工单号 [nvarchar](20) PRIMARY KEY )
 insert into @t1(生产工单号)
 select  生产工单号 from 财务即时库存记录 where 时间>'{t0}' and 时间<'{t1}' group by 生产工单号 
 union 
   select 相关单号  as 生产工单号  from  仓库出入库明细表 where  明细类型 in ('领料出库','入库倒冲') 
            and 出入库时间>'{t1}' and 出入库时间<'{t2}'  group by 相关单号 
 union
         select 相关单号  as 生产工单号  from 仓库出入库明细表 where 明细类型 in ('返工退料','工单关闭退料','工单退料')
            and 出入库时间>'{t1}' and 出入库时间<'{t2}' group by 相关单号 
            
 declare @t2 Table (生产工单号  [nvarchar](20) PRIMARY KEY,生产工单类型 [nvarchar](10),物料编码 [nvarchar](20),生产数量 decimal(18,2),当期入库量 decimal(18,2) 
                  ,累计入库量 decimal(18,2),累计报废数  decimal(18,2),当期报废数 decimal(18,2),关闭 int  );
 insert into @t2(生产工单号 ,生产工单类型 ,物料编码,生产数量 ,当期入库量,累计入库量 ,累计报废数 ,当期报废数 ,关闭 )    
  select gd.生产工单号,生产工单类型,物料编码,生产数量,ISNULL(当期入库量,0)当期入库量,isnull(累计入库量,0)累计入库量,ISNULL(ljjy.累计报废数,0)累计报废数
           ,ISNULL(jy.当期报废数,0)当期报废数,case when  gd.关闭=1 and gd.关闭日期<'{t2}' then 1 else 0 end as 关闭  from 生产记录生产工单表 gd
           left join (select 生产工单号,sum(入库数量)当期入库量 from 生产记录成品入库单明细表 where  生效日期>'{t1}' and 生效日期<'{t2}' group by 生产工单号)x 
           on x.生产工单号=gd.生产工单号 
               left join (select 生产工单号,sum(入库数量)累计入库量 from 生产记录成品入库单明细表 where  生效日期>'2019-5-1' and 生效日期<'{t2}' group by 生产工单号)lj
           on lj.生产工单号=gd.生产工单号 
           left join (select  生产工单号,sum(报废数)累计报废数  from 生产记录生产检验单主表 where 报废数 >0 and  生效日期>'2019-5-1' and 生效日期<'{t2}' group by 生产工单号)ljjy
           on ljjy.生产工单号=gd.生产工单号
            left join (select  生产工单号,sum(报废数)当期报废数  from 生产记录生产检验单主表 where 报废数 >0 and  生效日期>'{t1}' and 生效日期<'{t2}' group by 生产工单号)jy
           on jy.生产工单号=gd.生产工单号
            where   (生效日期>'{t1}' and 生效日期<'{t2}')  or gd.生产工单号 in (select  生产工单号 from @t1) 
 
    select *,当期入库量 as 当期完成数量 from (
           select  *,case  when 关闭=1 then 0 else 生产数量-累计入库量-累计报废数  end as  期末数量   from    @t2 )yy  ";    //0 为上期初  1 为 上期末 本期初，2 为 本期末  5-1 为系统起始时间
                                     //where  (累计入库量>0 or 关闭 =0)   19-7-24去除


                /* or gd.生产工单号 in (select  生产工单号 from 财务即时库存记录 where 时间>'{0}' and 时间<'{1}') 
      
         or gd.生产工单号 in  (select 生产工单号 from (
         select  生产工单号,物料编码,SUM(领料数量)领料数量,SUM(返库数量)返库数量 from (
         select 相关单号  as 生产工单号,物料编码,-sum(实效数量)领料数量,0 返库数量 from  仓库出入库明细表 where  明细类型 in ('领料出库','入库倒冲') 
            and 出入库时间>'{1}' and 出入库时间<'{2}'  and 相关单号 not in (select  a.备注  from 其他出库子表 a
                                                        left join 其他出入库申请主表  b on a.出入库申请单号 =b.出入库申请单号
                                                        where 原因分类='入库倒冲' and a.生效日期 >'{1}' and b.生效日期<'{2}' and 存货核算标记 =1) 
                                                            group by 相关单号,物料编码 
             union 
             select 相关单号  as 生产工单号,物料编码,0 领料数量,sum(实效数量)返库数量 from 仓库出入库明细表 where 明细类型 in ('返工退料','工单关闭退料','工单退料')
            and 出入库时间>'{1}' and 出入库时间<'{2}' group by 相关单号,物料编码 )x group by 生产工单号,物料编码 )kk  )*/
                DataTable t_工单 = CZMaster.MasterSQL.Get_DataTable(s, strcon); // 其中包括期末在制数量
                t_工单.Columns.Add("入库单价", typeof(decimal));
                t_工单.Columns.Add("材料金额", typeof(decimal));
                t_工单.Columns.Add("总金额", typeof(decimal));

                DataColumn dc = new DataColumn("辅材分摊", typeof(decimal));
                DataColumn dc1 = new DataColumn("工时", typeof(decimal));
                DataColumn dc2 = new DataColumn("工单工时", typeof(decimal));
                DataColumn dc3 = new DataColumn("制造费用", typeof(decimal));
                DataColumn dc4 = new DataColumn("人工费用", typeof(decimal));
                DataColumn dc5 = new DataColumn("软件费用", typeof(decimal));
                dc.DefaultValue = 0;
                dc3.DefaultValue = 0;
                dc4.DefaultValue = 0;
                dc5.DefaultValue = 0;
                t_工单.Columns.Add(dc);
                t_工单.Columns.Add(dc1);
                t_工单.Columns.Add(dc2);
                t_工单.Columns.Add(dc3);
                t_工单.Columns.Add(dc4);
                t_工单.Columns.Add(dc5);
                //19-11-6
                DataColumn[] pk = new DataColumn[1];
                pk[0] = t_工单.Columns["生产工单号"];
                t_工单.PrimaryKey = pk;
                //不参与运算
                /*@"生产工单号 in ('MO1907050027','MO1907180015','MO1908080002','MO1908090011','MO1908170003','MO1908210019'
               ,'MO1908210020','MO1908210021','MO1908210022','MO1908230018','MO1908270018','MO1908280002','MO1908280007','MO1908290006','MO1908300045')"*/
                //DataRow[] rrrr = t_工单.Select(string.Format(@"生产工单号 in ('MO1908300032','MO1908300034','MO1909040001','MO1909060008','MO1909120021','MO1909250008'
                //,'MO1910170027','MO1910290006')"));
                DataRow[] rrrr = t_工单.Select(string.Format(@"生产工单号 in ('MO1910250020','MO1911070017','MO1911070019','MO2001200005','MO2005120003')"));
 
                foreach (DataRow dr in rrrr)
                {
                    t_工单.Rows.Remove(dr);
                }
                //t_工单.Rows.Remove(rrrr[0]);
                //DataRow[] rrrrr = t_工单.Select(string.Format("生产工单号='{0}'", "MO1911070019"));
                //t_工单.Rows.Remove(rrrrr[0]);
                DataTable dt_本期耗用 = new DataTable();
                dt_本期耗用.Columns.Add("生产工单号");
                dt_本期耗用.Columns.Add("生产数量", typeof(decimal));
                dt_本期耗用.Columns.Add("产品编码");
                dt_本期耗用.Columns.Add("本期入库数量", typeof(decimal));
                dt_本期耗用.Columns.Add("累计报废数", typeof(decimal));
                dt_本期耗用.Columns.Add("当期报废数", typeof(decimal));
                dt_本期耗用.Columns.Add("累计入库数量", typeof(decimal));
                dt_本期耗用.Columns.Add("子项编码");
                dt_本期耗用.Columns.Add("本期耗用数量", typeof(decimal));
                dt_本期耗用.Columns.Add("子项期初数", typeof(decimal));
                dt_本期耗用.Columns.Add("子项期初金额", typeof(decimal));
                dt_本期耗用.Columns.Add("子项当期领用", typeof(decimal));
                dt_本期耗用.Columns.Add("子项在制数", typeof(decimal));
                dt_本期耗用.Columns.Add("发出单价", typeof(decimal));
                dt_本期耗用.Columns.Add("耗用单价", typeof(decimal));

                //19-11-6
                DataColumn[] pk_hy = new DataColumn[2];
                pk_hy[0] = dt_本期耗用.Columns["生产工单号"];
                pk_hy[1] = dt_本期耗用.Columns["子项编码"];
                dt_本期耗用.PrimaryKey = pk_hy;

                #region 19-8-8之前的版本
                //          s = string.Format(@" select 生产工单号,物料编码,SUM(领料数量) 领料数量,SUM(返库数量)返库数量,sum(实际领料数)实际领料数 from (
                //       select lmx.*,isnull(返库数量,0)返库数量,领料数量-isnull(返库数量,0) as 实际领料数 from (select  ll.生产工单号,ll.物料编码,sum(领料数量)领料数量 from 生产记录生产领料单明细表 ll  
                //       left join 生产记录生产工单待领料明细表 dll on ll.待领料单明细号=dll.待领料单明细号 
                //       where ll.生效日期 >'{0}' and  ll.生效日期 <'{1}'   group by ll.生产工单号,ll.物料编码)lmx
                //       left  join (select  ff.工单号,物料编码,sum(返库数量)返库数量 from 工单返库单明细表 ff
                //left join 工单返库单主表 f on f.退料单号=ff.退料单号
                //left join 工单退料申请表 fk on fk.待退料号=ff.待退料号 
                //       where ff.日期> '{0}' and  ff.日期<'{1}'  and  退料类型<>'返工退料'   group by ff.工单号,物料编码)fk on fk.工单号=lmx.生产工单号 and lmx.物料编码=fk.物料编码
                //union 
                //select  ff.工单号 as 生产工单号,物料编码,0 领料数量,sum(返库数量)返库数量,-sum(返库数量)实际领料数  from 工单返库单明细表  ff
                //left join 工单返库单主表 f on f.退料单号=ff.退料单号
                //left join 工单退料申请表 fk on fk.待退料号=ff.待退料号 
                //where ff.日期> '{0}' and  ff.日期<'{1}' and  退料类型='返工退料'  
                // group by ff.工单号,物料编码
                //       union 
                //       select  生产工单号,物料编码,SUM(领料数量) 领料数量,0,SUM(实际领料数)实际领料数 from (
                //       select  cz.备注 as 生产工单号 ,cz.物料编码 ,cz.数量 as 领料数量,0 返库数量,数量 as 实际领料数  from 其他出库子表  cz
                //       left join 其他出入库申请主表 sz on cz.出入库申请单号 =sz.出入库申请单号
                //       where 原因分类 ='入库倒冲' and LEFT(cz.备注,2)='MO' and cz.生效日期 >'{0}' and cz.生效日期<'{1}')u group by 生产工单号,物料编码 )x 
                //     group by  生产工单号,物料编码", t1, t2);

                #endregion


                //本期领料 总领料量+返库的   还要加入 入库倒冲的料
                //8-12 需要去掉本期没有入库的入库倒冲（5、6月份的未完成的入库倒冲） 原因分类='入库倒冲' and 存货核算标记 =1
                ///*and 相关单号 not in (select   备注 from  t)*/
                //     s = string.Format(@"select  *,领料数量-返库数量 as 实际领料数 from (
                //      select  生产工单号,物料编码,SUM(领料数量)领料数量,SUM(返库数量)返库数量 from (
                //      select 相关单号  as 生产工单号,物料编码,-sum(实效数量)领料数量,0 返库数量 from  仓库出入库明细表 where  明细类型 in ('领料出库','入库倒冲') 
                // and 出入库时间>'{0}' and 出入库时间<'{1}' and 相关单号 not in (select  a.备注  from 其他出库子表 a
                //   left join 其他出入库申请主表  b on a.出入库申请单号 =b.出入库申请单号
                //  where 原因分类='入库倒冲' and a.生效日期 >'{0}' and b.生效日期<'{1}' and 存货核算标记 =1) group by 相关单号,物料编码 
                //  union 
                //  select 相关单号  as 生产工单号,物料编码,0 领料数量,sum(实效数量)返库数量 from 仓库出入库明细表 where 明细类型 in ('返工退料','工单关闭退料','工单退料')
                // and 出入库时间>'{0}' and 出入库时间<'{1}'
                //group by 相关单号,物料编码 )x group by 生产工单号,物料编码)kk ", t1, t2);
                s = string.Format(@"  with t as (select  a.备注  from 其他出库子表 a
              left join 其他出入库申请主表  b on a.出入库申请单号 =b.出入库申请单号
             where 原因分类='入库倒冲' and a.生效日期 >'{0}' and b.生效日期<'{1}' and 存货核算标记 =1)
   ,t1 as (select 相关单号  as 生产工单号,物料编码,0 领料数量,sum(实效数量)返库数量 from 仓库出入库明细表 
             where 明细类型 in ('返工退料','工单关闭退料','工单退料')
            and 出入库时间>'{0}' and 出入库时间<'{1}' 
           group by 相关单号,物料编码) 
    ,t2 as (select  生产工单号,物料编码,SUM(领料数量)领料数量,SUM(返库数量)返库数量 from (
                 select 相关单号  as 生产工单号,物料编码,-sum(实效数量)领料数量,0 返库数量 from  仓库出入库明细表 where  明细类型 in ('领料出库','入库倒冲') 
            and 出入库时间>'{0}' and 出入库时间<'{1}' /*and 相关单号 not in (select   备注 from  t)*/ group by 相关单号,物料编码 
             union   select  * from t1
              )x group by 生产工单号,物料编码)
   select  *,领料数量-返库数量 as 实际领料数 from  t2 ",t1,t2);
                DataTable t_领料全 = CZMaster.MasterSQL.Get_DataTable(s, strcon);


                string gbwtl = string.Format(@"select  生产工单号  from 工单退料申请表 where   
              (完成 = 0  or(完成 = 1 and  完成日期 > '{0}'))  and 作废 = 0   group by 生产工单号", t2);
                DataTable dt_关闭未退料 = CZMaster.MasterSQL.Get_DataTable(gbwtl, strcon);
                ///2019-8-8 领料中有一部分是替代料 在待领料明细中是 补料 然后在t_工单bom 中没有 需要加入 然后 标记为替代料
                foreach (DataRow dr in t_领料全.Rows)
                {
                    if (Convert.ToDecimal(dr["实际领料数"]) != 0)
                    {
                        DataRow[] rr = t_工单bom.Select(string.Format("生产工单号='{0}' and 物料编码='{1}' ", dr["生产工单号"], dr["物料编码"]));
                        if (rr.Length == 0)
                        {
                            DataRow rx = t_工单bom.NewRow();
                            rx["生产工单号"] = dr["生产工单号"];
                            rx["领料类型"] = "替代料";
                            rx["待领料总量"] = dr["实际领料数"];
                            rx["已领"] = dr["实际领料数"];
                            rx["物料编码"] = dr["物料编码"];

                            DataRow[] r_ls = t_工单.Select(string.Format("生产工单号='{0}'", dr["生产工单号"]));
                            rx["产品编码"] = r_ls[0]["物料编码"];
                            rx["生产数量"] = r_ls[0]["生产数量"];
                            t_工单bom.Rows.Add(rx);
                        }
                    }
                }


                //19-6-26 算期末在制 和本期耗用： 期初 +当期领 -本期完工耗用（加补料）=期末
                foreach (DataRow dr in t_工单.Rows)
                {
                    //if (dr["生产工单号"].ToString() == "MO1907020045")
                    //{

                    //}
                    bool bl_完成 = false;
                    if (Convert.ToDecimal(dr["期末数量"]) == 0) //已完成  所有都耗用 本期在制为0 
                    {
                        bl_完成 = true;
                        //19-8-9 需要判断 关单关闭 尚有退料未完成部分 需要把bl_完成=false    期末数量 这个字段已 除此处外 基本无用 不用考虑
                        if (Convert.ToBoolean(dr["关闭"]))
                        {
                            DataRow[] r_gbwtl = dt_关闭未退料.Select(string.Format("生产工单号='{0}'", dr["生产工单号"]));
                            if (r_gbwtl.Length > 0)
                            {
                                bl_完成 = false;
                            }

                        }

                    }
                    //bool bl_完成 = false;
                    //if (Convert.ToDecimal(dr["期末数量"]) == 0) //已完成  所有都耗用 本期在制为0 
                    //{
                    //    bl_完成 = true;
                    //}
                    DataRow[] y = t_上期在制.Select(string.Format("生产工单号='{0}'", dr["生产工单号"]));
                    foreach (DataRow yy in y)
                    {
                        DataRow xr = dt_本期耗用.NewRow();
                        xr["生产工单号"] = dr["生产工单号"];
                        xr["产品编码"] = dr["物料编码"];
                        xr["生产数量"] = dr["生产数量"];
                        xr["本期入库数量"] = dr["当期入库量"];
                        xr["累计入库数量"] = dr["累计入库量"];
                        xr["累计报废数"] = dr["累计报废数"];
                        xr["当期报废数"] = dr["当期报废数"];
                        xr["子项编码"] = yy["子项编码"];

                        xr["子项期初数"] = Convert.ToDecimal(yy["在制品"]);
                        xr["子项期初金额"] = Convert.ToDecimal(yy["在制金额"]);

                        DataRow[] r_sq = t_领料全.Select(string.Format("生产工单号='{0}' and 物料编码='{1}'", dr["生产工单号"], yy["子项编码"]));
                        if (r_sq.Length > 0) xr["子项当期领用"] = Convert.ToDecimal(r_sq[0]["实际领料数"]);
                        else xr["子项当期领用"] = 0;
                        if (bl_完成)
                        {
                            xr["本期耗用数量"] = Convert.ToDecimal(xr["子项期初数"]) + Convert.ToDecimal(xr["子项当期领用"]);
                            xr["子项在制数"] = 0;
                        }
                        else
                        {
                            DataRow[] re = t_工单bom.Select(string.Format("生产工单号='{0}' and 物料编码='{0}'", dr["生产工单号"], yy["子项编码"]));
                            //19-11-6
                           // DataRow re = t_工单bom.Rows.Find(new object[] { dr["生产工单号"], yy["子项编码"] });

                             if (re.Length > 0)
                            //if (re != null)
                            {
                                if (re[0]["领料类型"].ToString() == "入库倒冲")
                                //因为 入库倒冲放到了完工的地方 所以需要判断有没有 全部入库  全部入库则全部耗用，入库一部分则按 bom数量计算
                                {
                                    if (bl_完成)
                                    {
                                        xr["本期耗用数量"] = Convert.ToDecimal(xr["子项期初数"]) + Convert.ToDecimal(xr["子项当期领用"]);
                                        xr["子项在制数"] = 0;
                                    }
                                    else
                                    {
                                        xr["本期耗用数量"] = Convert.ToDecimal(re[0]["BOM数量"]) * Convert.ToDecimal(dr["当期完成数量"]);
                                        xr["子项在制数"] = Convert.ToDecimal(xr["子项期初数"]) + Convert.ToDecimal(xr["子项当期领用"]) - Convert.ToDecimal(xr["本期耗用数量"]);
                                    }
                                }
                                else
                                {
                                    xr["本期耗用数量"] = Convert.ToDecimal(re[0]["BOM数量"]) * Convert.ToDecimal(dr["当期完成数量"]);
                                    xr["子项在制数"] = Convert.ToDecimal(xr["子项期初数"]) + Convert.ToDecimal(xr["子项当期领用"]) - Convert.ToDecimal(xr["本期耗用数量"]);
                                }
                            }
                            else
                            {
                                xr["本期耗用数量"] = 0;
                                xr["子项在制数"] = Convert.ToDecimal(xr["子项期初数"]) + Convert.ToDecimal(xr["子项当期领用"]) - Convert.ToDecimal(xr["本期耗用数量"]);
                                //if (dr["生产工单号"].ToString() == "MO1905200013")
                                //{
                                //    xr["本期耗用数量"] = xr["子项在制数"];
                                //    xr["子项在制数"] = 0;
                                //}

                            }
                        }
                        dt_本期耗用.Rows.Add(xr);
                    }
                    //期初+当期领用（含退料） 全部耗用 
                    DataRow[] r_工单bom = t_工单bom.Select(string.Format("生产工单号='{0}'", dr["生产工单号"]));

          

                    // DataRow [] r_期初在制=t_上期在制.Select(string.Format("生产工单号='{0}'", dr["生产工单号"]));

                    foreach (DataRow r in r_工单bom) //又在制先把在制加进去
                    {
                        if (r["领料类型"].ToString() == "替代料")
                        {
                            DataRow[] erer = dt_本期耗用.Select(string.Format("生产工单号='{0}' and 子项编码='{1}'", dr["生产工单号"], r["物料编码"]));
                            if (erer.Length == 0)
                            {
                                DataRow xr = dt_本期耗用.NewRow();
                                xr["生产工单号"] = dr["生产工单号"];
                                xr["产品编码"] = dr["物料编码"];
                                xr["生产数量"] = dr["生产数量"];
                                xr["本期入库数量"] = dr["当期入库量"];
                                xr["累计入库数量"] = dr["累计入库量"];
                                xr["累计报废数"] = dr["累计报废数"];
                                xr["当期报废数"] = dr["当期报废数"];
                                xr["子项编码"] = r["物料编码"];
                                DataRow[] r_sq = t_上期在制.Select(string.Format("生产工单号='{0}' and 子项编码='{1}'", dr["生产工单号"], r["物料编码"]));
                                if (r_sq.Length > 0)
                                {
                                    xr["子项期初数"] = Convert.ToDecimal(r_sq[0]["在制品"]);
                                    xr["子项期初金额"] = Convert.ToDecimal(r_sq[0]["在制金额"]);
                                }
                                else
                                {
                                    xr["子项期初数"] = 0;
                                    xr["子项期初金额"] = 0;
                                }
                                r_sq = t_领料全.Select(string.Format("生产工单号='{0}' and 物料编码='{1}'", dr["生产工单号"], r["物料编码"]));
                                if (r_sq.Length > 0) xr["子项当期领用"] = Convert.ToDecimal(r_sq[0]["实际领料数"]);
                                else xr["子项当期领用"] = 0;
                                //替代料这边如果有入库 补领的全部耗用
                                if (Convert.ToDecimal(xr["本期入库数量"]) > 0)
                                {
                                    xr["本期耗用数量"] = Convert.ToDecimal(xr["子项期初数"]) + Convert.ToDecimal(xr["子项当期领用"]);
                                    xr["子项在制数"] = 0;
                                }
                                else
                                {
                                    xr["本期耗用数量"] = 0;
                                    xr["子项在制数"] = Convert.ToDecimal(xr["子项期初数"]) + Convert.ToDecimal(xr["子项当期领用"]);
                                }
                                dt_本期耗用.Rows.Add(xr);
                            }
                            else
                            {
                                DataRow[] r_sq = t_领料全.Select(string.Format("生产工单号='{0}' and 物料编码='{1}'", dr["生产工单号"], r["物料编码"]));
                                if (r_sq.Length > 0) erer[0]["子项当期领用"] = Convert.ToDecimal(r_sq[0]["实际领料数"]);
                                erer[0]["本期耗用数量"] = Convert.ToDecimal(erer[0]["本期耗用数量"]) + Convert.ToDecimal(erer[0]["子项当期领用"]);//理论上只会有一次
                                erer[0]["子项在制数"] = 0;
                            }
                        }
                        else
                        {
                            DataRow[] erer = dt_本期耗用.Select(string.Format("生产工单号='{0}' and 子项编码='{1}'", dr["生产工单号"], r["物料编码"]));
                            if (erer.Length == 0)
                            {
                                DataRow xr = dt_本期耗用.NewRow();
                                xr["生产工单号"] = dr["生产工单号"];
                                xr["产品编码"] = dr["物料编码"];
                                xr["生产数量"] = dr["生产数量"];
                                xr["本期入库数量"] = dr["当期入库量"];
                                xr["累计入库数量"] = dr["累计入库量"];
                                xr["累计报废数"] = dr["累计报废数"];
                                xr["当期报废数"] = dr["当期报废数"];
                                xr["子项编码"] = r["物料编码"];
                                DataRow[] r_sq = t_上期在制.Select(string.Format("生产工单号='{0}' and 子项编码='{1}'", dr["生产工单号"], r["物料编码"]));
                                if (r_sq.Length > 0)
                                {
                                    xr["子项期初数"] = Convert.ToDecimal(r_sq[0]["在制品"]);
                                    xr["子项期初金额"] = Convert.ToDecimal(r_sq[0]["在制金额"]);
                                }
                                else
                                {
                                    xr["子项期初数"] = 0;
                                    xr["子项期初金额"] = 0;
                                }
                                r_sq = t_领料全.Select(string.Format("生产工单号='{0}' and 物料编码='{1}'", dr["生产工单号"], r["物料编码"]));
                                if (r_sq.Length > 0) xr["子项当期领用"] = Convert.ToDecimal(r_sq[0]["实际领料数"]);
                                else xr["子项当期领用"] = 0;
                                if (bl_完成)
                                {
                                    xr["本期耗用数量"] = Convert.ToDecimal(xr["子项期初数"]) + Convert.ToDecimal(xr["子项当期领用"]);
                                    xr["子项在制数"] = 0;
                                }
                                else
                                {
                                    if (r["领料类型"].ToString() == "入库倒冲")
                                    {
                                        if (bl_完成)
                                        {
                                            xr["本期耗用数量"] = Convert.ToDecimal(xr["子项期初数"]) + Convert.ToDecimal(xr["子项当期领用"]);
                                            xr["子项在制数"] = 0;
                                        }
                                        else
                                        {
                                            xr["本期耗用数量"] = Convert.ToDecimal(r["BOM数量"]) * Convert.ToDecimal(dr["当期完成数量"]);
                                            xr["子项在制数"] = Convert.ToDecimal(xr["子项期初数"]) + Convert.ToDecimal(xr["子项当期领用"]) - Convert.ToDecimal(xr["本期耗用数量"]);
                                        }
                                    }
                                    else
                                    {
                                        xr["本期耗用数量"] = Convert.ToDecimal(r["BOM数量"]) * Convert.ToDecimal(dr["当期完成数量"]);
                                        xr["子项在制数"] = Convert.ToDecimal(xr["子项期初数"]) + Convert.ToDecimal(xr["子项当期领用"]) - Convert.ToDecimal(xr["本期耗用数量"]);
                                    }
                                }

                                dt_本期耗用.Rows.Add(xr);

                            }
                            else if (!bl_完成)
                            {
                                DataRow[] r_sq = t_领料全.Select(string.Format("生产工单号='{0}' and 物料编码='{1}'", dr["生产工单号"], r["物料编码"]));
                                if (r_sq.Length > 0) erer[0]["子项当期领用"] = Convert.ToDecimal(r_sq[0]["实际领料数"]);
                                //if (r["领料类型"].ToString() == "入库倒冲")
                                //{
                                //    erer[0]["本期耗用数量"] = Convert.ToDecimal(erer[0]["子项期初数"]) + Convert.ToDecimal(erer[0]["子项当期领用"]);
                                //    erer[0]["子项在制数"] = 0;
                                //}
                                //else
                                //{
                                erer[0]["本期耗用数量"] = Convert.ToDecimal(r["BOM数量"]) * Convert.ToDecimal(dr["当期完成数量"]);
                                erer[0]["子项在制数"] = Convert.ToDecimal(erer[0]["子项期初数"]) + Convert.ToDecimal(erer[0]["子项当期领用"]) - Convert.ToDecimal(erer[0]["本期耗用数量"]);

                                //}
                            }
                            else
                            {
                                DataRow[] r_sq = t_领料全.Select(string.Format("生产工单号='{0}' and 物料编码='{1}'", dr["生产工单号"], r["物料编码"]));
                                if (r_sq.Length > 0) erer[0]["子项当期领用"] = Convert.ToDecimal(r_sq[0]["实际领料数"]);


                                erer[0]["本期耗用数量"] = Convert.ToDecimal(erer[0]["子项期初数"]) + Convert.ToDecimal(erer[0]["子项当期领用"]);

                                erer[0]["子项在制数"] = 0;

                            }
                        }
                    }



                }
                s = "select  物料编码,规格型号,物料名称,存货分类,存货分类编码 from 基础数据物料信息表  ";
                DataTable t_基础 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                //19-11-6
                DataColumn[] pk_基础 = new DataColumn[1];
                pk_基础[0] = t_基础.Columns["物料编码"];
                t_基础.PrimaryKey = pk_基础;

                s = "select  产品编码,工时 from [2019财务工时] ";
                DataTable t_工时 = CZMaster.MasterSQL.Get_DataTable(s, strcon);

                s = "select  产品编码,单价 from [2019财务软件费用] ";
                DataTable t_软件费用 = CZMaster.MasterSQL.Get_DataTable(s, strcon);


                ds.Tables.Add(dt_本期耗用);
                ds.Tables.Add(t_工单);
                ds.Tables.Add(t_基础);


                //  DataTable t_成本 = dt_存货核算.Clone();
                DataTable t_成本 = dt_存货核算.Clone();

                t_成本.TableName = "成本";
                foreach (DataRow dr in dt_存货核算.Rows)
                {
                    if (dr["物料编码"].ToString().Substring(0, 2) != "05" && dr["物料编码"].ToString().Substring(0, 2) != "10")

                        t_成本.ImportRow(dr);
                }
            //取原材料的最终发出单价 
            ;
                ds.Tables.Add(t_成本);
                ds.Tables.Add(t_上期在制);

                decimal dec_总工时 = 0;
                ///此处还需要根据 物料汇总   物料，平均入库单价  把 返修的再算出来后 汇总 即可 
                foreach (DataRow dr in t_工单.Rows)
                {

                    DataRow[] r_软件费 = t_软件费用.Select(string.Format("产品编码='{0}'", dr["物料编码"]));
                    if (r_软件费.Length > 0)
                        dr["软件费用"] = Convert.ToDecimal(r_软件费[0]["单价"]) * Convert.ToDecimal(dr["当期完成数量"]);
                    //if (dr["生产工单类型"].ToString() == "返修工单")// 之前说只需要返修的工单 要扣除 领料的软件费用  现在是所有的工单
                    //{
                    //若为返修工单 还需要判断是否需要扣除 领料的软件费用
                    DataRow[] r_返修领料 = dt_本期耗用.Select(string.Format("生产工单号='{0}'", dr["生产工单号"]));
                    foreach (DataRow r in r_返修领料)
                    {
                        if (r["子项编码"].ToString().Substring(0, 2) == "10")
                        {
                            DataRow[] rr = t_软件费用.Select(string.Format("产品编码='{0}'", r["子项编码"]));
                            if (rr.Length > 0)
                            {
                                dr["软件费用"] = Convert.ToDecimal(dr["软件费用"]) - Convert.ToDecimal(rr[0]["单价"]) * Convert.ToDecimal(dr["当期完成数量"]);
                            }
                        }
                    }
                    //}
                    //else  //这边周连梅说 返修的工单 没有人统计工时  所以暂时不需要分摊
                    //{
                    if (dr["生产工单类型"].ToString() != "返修工单")
                    {
                        DataRow[] r_工时 = t_工时.Select(string.Format("产品编码='{0}'", dr["物料编码"]));
                        if (r_工时.Length > 0)
                        {
                            dr["工时"] = r_工时[0]["工时"];
                            dr["工单工时"] = Math.Round(Convert.ToDecimal(dr["工时"]) * Convert.ToDecimal(dr["当期完成数量"]), 4, MidpointRounding.AwayFromZero);
                            dec_总工时 += Convert.ToDecimal(dr["工单工时"]);
                        }
                    }


                    //if (Convert.ToDecimal(dr["当期完成数量"]) == 0) dr["入库单价"] = 0;
                    //else
                    //{
                    //    dr["入库单价"] = dec_累计金额 / Convert.ToDecimal(dr["当期完成数量"]);
                    //}

                }
                foreach (DataRow dr in t_工单.Rows)
                {
                    if (dr["生产工单类型"].ToString() != "返修工单")
                    {

                        if (dr["工单工时"] != DBNull.Value && dr["工单工时"].ToString() != "")
                        {
                            dr["辅材分摊"] = Math.Round(Convert.ToDecimal(dr["工单工时"]) / dec_总工时 * dec_辅材, 4, MidpointRounding.AwayFromZero);
                            dr["人工费用"] = Math.Round(Convert.ToDecimal(dr["工单工时"]) / dec_总工时 * dec_人工, 4, MidpointRounding.AwayFromZero);
                            dr["制造费用"] = Math.Round(Convert.ToDecimal(dr["工单工时"]) / dec_总工时 * dec_制造, 4, MidpointRounding.AwayFromZero);
                        }
                    }
                    //dr["总金额"] = Convert.ToDecimal(dr["材料金额"])+Convert.ToDecimal(dr["辅材分摊"])+
                    //    Convert.ToDecimal(dr["人工费用"]) + Convert.ToDecimal(dr["制造费用"])+ Convert.ToDecimal(dr["软件费用"]);
                    //if (Convert.ToDecimal(dr["当期完成数量"]) == 0) dr["入库单价"] = 0;
                    //else
                    //{
                    //    dr["入库单价"] = Convert.ToDecimal(dr["总金额"]) / Convert.ToDecimal(dr["当期完成数量"]);
                    //}
                }

                ///此处还需要根据 物料汇总   物料，平均入库单价  把 返修的再算出来后 汇总 即可 
                foreach (DataRow dr in t_工单.Rows)
                {

                    //if (dr["生产工单号"].ToString() == "MO1907020045")
                    //{

                    //}
                    DataRow[] r_本期消耗 = dt_本期耗用.Select(string.Format("生产工单号='{0}'", dr["生产工单号"]));
                    decimal dec_累计金额 = 0;
                    decimal dec_累计数量 = 0;
                    foreach (DataRow e in r_本期消耗)
                    {
                        //if ( e["子项编码"].ToString() == "05010300000013")
                        //{

                        //}
                        //  存货核算 已经和上期的结存 合并  默认原材料的 r.lenth>0 恒成立
                        //DataRow[] r = dt_存货核算.Select(string.Format("物料编码='{0}'", e["子项编码"]));
                        decimal b = 0;
                        if (e["子项编码"].ToString().Substring(0, 2) == "01")
                        {
                            DataRow[] r = t_成本.Select(string.Format("物料编码='{0}'", e["子项编码"]));
                            if (r.Length == 0) //说明该成品或者半成品的 成本尚未算出 先算这个的发出单价
                            {
                                b = fun_dg(ds, e["子项编码"].ToString(), t1); //b为 e["子项编码"]的最终 发出单价 
                                DataRow[] ir = t_成本.Select(string.Format("物料编码='{0}'", e["子项编码"].ToString()));
                                if (ir.Length == 0)
                                {
                                    DataRow cr = t_成本.NewRow();
                                    DataRow[] bs = t_基础.Select(string.Format("物料编码='{0}'", e["子项编码"]));
                                    cr["物料名称"] = bs[0]["物料名称"];
                                    cr["规格型号"] = bs[0]["规格型号"];
                                    cr["存货分类"] = bs[0]["存货分类"];
                                    cr["存货分类编码"] = bs[0]["存货分类编码"];
                                    cr["物料编码"] = e["子项编码"];
                                    //cr["物料名称"] = e["物料名称"];
                                    //cr["规格型号"] = e["规格型号"];
                                    //cr["存货分类"] = e["存货分类"];
                                    //cr["存货分类编码"] = e["存货分类编码"];
                                    cr["发出单价"] = Math.Round(b, 6, MidpointRounding.AwayFromZero);
                                    t_成本.Rows.Add(cr);
                                }
                            }
                            else
                            {
                                b = Convert.ToDecimal(r[0]["发出单价"]);
                            }
                        }
                        else // 05-   10-- 
                        {
                            DataRow[] r = t_成本.Select(string.Format("物料编码='{0}'", e["子项编码"]));
                            if (r.Length > 0)  //已与其他入库 加权平均算过了 直接取这个就行
                            {
                                b = Convert.ToDecimal(r[0]["发出单价"]);
                            }
                            else
                            {
                                b = fun_dg(ds, e["子项编码"].ToString(), t1); //b为 e["子项编码"]的最终 发出单价 
                                DataRow[] ir = t_成本.Select(string.Format("物料编码='{0}'", e["子项编码"].ToString()));
                                if (ir.Length == 0)
                                {
                                    DataRow cr = t_成本.NewRow();
                                    DataRow[] bs = t_基础.Select(string.Format("物料编码='{0}'", e["子项编码"]));
                                    cr["物料名称"] = bs[0]["物料名称"];
                                    cr["规格型号"] = bs[0]["规格型号"];
                                    cr["存货分类"] = bs[0]["存货分类"];
                                    cr["存货分类编码"] = bs[0]["存货分类编码"];
                                    cr["物料编码"] = e["子项编码"];
                                    DataRow[] rg = dt_存货核算.Select(string.Format("物料编码='{0}'", e["子项编码"]));
                                    if (rg.Length > 0)
                                    {
                                        cr["累计入库金额"] = rg[0]["累计入库金额"];
                                        cr["累计入库数量"] = rg[0]["累计入库数量"];
                                    }
                                    //cr["物料名称"] = e["物料名称"];
                                    //cr["规格型号"] = e["规格型号"];
                                    //cr["存货分类"] = e["存货分类"];
                                    //cr["存货分类编码"] = e["存货分类编码"];
                                    cr["发出单价"] = Math.Round(b, 6, MidpointRounding.AwayFromZero);
                                    t_成本.Rows.Add(cr);
                                }
                            }
                        }
                        //b为本月发出单价 但是如果上月有结存 需要去平均发出单价  (上期金额+本期发出金额 ) /（上期在制品+本期领用数量）
                        e["发出单价"] = Math.Round(b, 6, MidpointRounding.AwayFromZero);
                        decimal dec_耗用 = b; //本月耗用单价
                        decimal dec_本期金额 = 0;
                        DataRow[] rt_上期 = t_上期在制.Select(string.Format("生产工单号='{0}' and 子项编码='{1}'", e["生产工单号"], e["子项编码"]));
                        if (rt_上期.Length > 0)
                        {
                            dec_本期金额 = Math.Round(Convert.ToDecimal(e["子项当期领用"]) * b, 2, MidpointRounding.AwayFromZero);
                            if (Convert.ToDecimal(e["子项当期领用"]) + Convert.ToDecimal(rt_上期[0]["在制品"]) != 0)
                            {

                                dec_耗用 = (Convert.ToDecimal(rt_上期[0]["在制金额"]) + dec_本期金额) / (Convert.ToDecimal(e["子项当期领用"]) + Convert.ToDecimal(rt_上期[0]["在制品"]));
                            }
                            else
                            {
                                if (Convert.ToDecimal(rt_上期[0]["在制品"]) == 0)
                                {
                                }

                                dec_耗用 = Math.Abs((Convert.ToDecimal(rt_上期[0]["在制金额"]) / Convert.ToDecimal(rt_上期[0]["在制品"])));
                                dec_耗用 = Math.Round(dec_耗用, 6, MidpointRounding.AwayFromZero);
                                e["发出单价"] = dec_耗用;
                            }
                        }
                        e["耗用单价"] = dec_耗用 = Math.Round(dec_耗用, 6, MidpointRounding.AwayFromZero);
                        if (e["本期耗用数量"] == DBNull.Value || e["本期耗用数量"] == null)
                        {
                            dec_累计金额 += Convert.ToDecimal(rt_上期[0]["在制金额"]);
                        }
                        else
                        {
                            dec_累计金额 += Math.Round(Convert.ToDecimal(e["本期耗用数量"]) * dec_耗用, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    dr["材料金额"] = Math.Round(dec_累计金额, 2, MidpointRounding.AwayFromZero);
                    decimal dec_总 = Math.Round(Convert.ToDecimal(dr["材料金额"]) + Convert.ToDecimal(dr["辅材分摊"]) +
                    Convert.ToDecimal(dr["人工费用"]) + Convert.ToDecimal(dr["制造费用"]) + Convert.ToDecimal(dr["软件费用"]), 2, MidpointRounding.AwayFromZero);
                    dr["总金额"] = dec_总;

                    if (Convert.ToDecimal(dr["当期完成数量"]) == 0) dr["入库单价"] = 0;
                    else
                    {
                        if (Convert.ToDecimal(dr["当期完成数量"])==0)
                        {
                        }
                        dr["入库单价"] = Math.Round(Convert.ToDecimal(dr["总金额"]) / Convert.ToDecimal(dr["当期完成数量"]), 6, MidpointRounding.AwayFromZero);
                    }
                }

                //  ERPorg.Corg.TableToExcel(dt_本期耗用, @"C:\Users\GH\Desktop\本期耗用.xlsx");
                MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
                DataTable t_gd = RBQ.SelectGroupByInto("", t_工单, "物料编码,sum(总金额) 总金额,sum(材料金额) 材料金额,sum(辅材分摊) 辅材分摊,sum(人工费用) 人工费用,sum(制造费用) 制造费用,sum(当期完成数量) 当期完成数量,sum(软件费用) 软件费用", "", "物料编码");
                foreach (DataRow r_gd in t_gd.Rows)
                {

                    decimal dec_期初金额 = 0;
                    decimal dec_期初数量 = 0;
                    decimal je_sum = 0;
                    decimal dec_sum = 0;
                    DataRow[] xx = dt_结转.Select(string.Format("物料编码='{0}'", r_gd["物料编码"]));
                    if (xx.Length == 0)
                    {
                        dec_期初金额 = 0;
                        dec_期初数量 = 0;
                    }
                    else
                    {
                        dec_期初金额 = Convert.ToDecimal(xx[0]["本月结转金额"]);
                        dec_期初数量 = Convert.ToDecimal(xx[0]["本月结转数量"]);
                    }
                    //入库有金额
                    DataRow[] rrr = dt_存货核算.Select(string.Format("物料编码='{0}'", r_gd["物料编码"]));
                    if (rrr.Length > 0) //先取 dt_存货核算中的 金额 和 数量
                    {
                        if (rrr[0]["累计入库金额"] == DBNull.Value || rrr[0]["累计入库金额"].ToString() == "")
                            je_sum = 0;
                        else
                        { je_sum = Convert.ToDecimal(rrr[0]["累计入库金额"]); }
                        if (rrr[0]["累计入库数量"] == DBNull.Value || rrr[0]["累计入库数量"].ToString() == "")
                            dec_sum = 0;
                        else dec_sum = Convert.ToDecimal(rrr[0]["累计入库数量"]);

                    }
                    DataRow[] yy = t_成本.Select(string.Format("物料编码='{0}'", r_gd["物料编码"]));
                    if (yy.Length == 0)
                    {
                        DataRow rr = t_成本.NewRow();
                        rr["物料编码"] = r_gd["物料编码"];
                        DataRow[] bs = t_基础.Select(string.Format("物料编码='{0}'", r_gd["物料编码"]));
                        rr["物料名称"] = bs[0]["物料名称"];
                        rr["规格型号"] = bs[0]["规格型号"];
                        rr["存货分类"] = bs[0]["存货分类"];
                        rr["存货分类编码"] = bs[0]["存货分类编码"];
                        //rr["累计入库金额"] =Math.Round(Convert.ToDecimal(r_gd["材料金额"]) + Convert.ToDecimal(r_gd["辅材分摊"])+ Convert.ToDecimal(r_gd["人工费用"])
                        //    + Convert.ToDecimal(r_gd["制造费用"])+ Convert.ToDecimal(r_gd["软件费用"]) + je_sum,2, MidpointRounding.AwayFromZero);
                        rr["累计入库金额"] = Math.Round(Convert.ToDecimal(r_gd["总金额"]) + je_sum, 2, MidpointRounding.AwayFromZero);
                        rr["累计入库数量"] = Convert.ToDecimal(r_gd["当期完成数量"]) + dec_sum;
                        decimal dec = Convert.ToDecimal(rr["累计入库数量"]);
                        if (dec == 0)
                        {
                            rr["收入单价"] = 0;
                        }
                        else
                        {
                            rr["收入单价"] = Math.Round(Convert.ToDecimal(rr["累计入库金额"]) / dec, 6, MidpointRounding.AwayFromZero);
                        }
                        if (dec_期初数量 + dec == 0)
                        { rr["发出单价"] = 0; }
                        else rr["发出单价"] = Math.Round((dec_期初金额 + Convert.ToDecimal(rr["累计入库金额"])) / (dec_期初数量 + dec), 6, MidpointRounding.AwayFromZero);
                        t_成本.Rows.Add(rr);
                    }
                    else
                    {
                        //yy[0]["累计入库金额"] = Math.Round(Convert.ToDecimal(r_gd["材料金额"])  +Convert.ToDecimal(r_gd["辅材分摊"]) + Convert.ToDecimal(r_gd["人工费用"])
                        //    + Convert.ToDecimal(r_gd["制造费用"]) + Convert.ToDecimal(r_gd["软件费用"]) + je_sum,2, MidpointRounding.AwayFromZero);
                        yy[0]["累计入库金额"] = Math.Round(Convert.ToDecimal(r_gd["总金额"]) + je_sum, 2, MidpointRounding.AwayFromZero);
                        yy[0]["累计入库数量"] = Convert.ToDecimal(r_gd["当期完成数量"]) + dec_sum;
                        decimal dec = Convert.ToDecimal(yy[0]["累计入库数量"]);
                        if (dec == 0)
                        {
                            yy[0]["收入单价"] = 0;
                        }
                        else
                        {
                            yy[0]["收入单价"] = Math.Round(Convert.ToDecimal(yy[0]["累计入库金额"]) / dec, 6, MidpointRounding.AwayFromZero);
                        }
                        if (dec_期初数量 + dec == 0)
                        { yy[0]["发出单价"] = 0; }
                        else yy[0]["发出单价"] = Math.Round((dec_期初金额 + Convert.ToDecimal(yy[0]["累计入库金额"])) / (dec_期初数量 + dec), 6, MidpointRounding.AwayFromZero);
                    }
                }

                foreach (DataRow tr in dt_存货核算.Rows)
                {

                    DataRow[] r_jy = t_成本.Select(string.Format("物料编码='{0}'", tr["物料编码"]));
                    if (r_jy.Length > 0)
                    {
                        if ((Convert.ToDecimal(r_jy[0]["发出单价"]) == 0 || (r_jy[0]["累计入库数量"] == DBNull.Value || r_jy[0]["累计入库数量"].ToString() == ""
                        || Convert.ToDecimal(r_jy[0]["累计入库数量"]) == 0)))
                        {
                            r_jy[0]["发出单价"] = tr["发出单价"];
                            r_jy[0]["收入单价"] = tr["收入单价"];
                            r_jy[0]["累计入库金额"] = Math.Round(Convert.ToDecimal(tr["累计入库金额"]), 2, MidpointRounding.AwayFromZero);
                            r_jy[0]["累计入库数量"] = tr["累计入库数量"];
                        }
                    }
                    else
                    {
                        DataRow cr = t_成本.NewRow();
                        DataRow[] bs = t_基础.Select(string.Format("物料编码='{0}'", tr["物料编码"]));
                        cr["物料名称"] = bs[0]["物料名称"];
                        cr["规格型号"] = bs[0]["规格型号"];
                        cr["存货分类"] = bs[0]["存货分类"];
                        cr["存货分类编码"] = bs[0]["存货分类编码"];
                        cr["物料编码"] = tr["物料编码"];
                        cr["发出单价"] = tr["发出单价"];
                        cr["收入单价"] = tr["收入单价"];
                        cr["累计入库金额"] = Math.Round(Convert.ToDecimal(tr["累计入库金额"]), 2, MidpointRounding.AwayFromZero);
                        cr["累计入库数量"] = tr["累计入库数量"];
                        //cr["结存单价"] = r_wcr["结存单价"];
                        t_成本.Rows.Add(cr);
                    }
                }
                //结转都没有。
                foreach (DataRow r_wcr in dt_结转.Rows)
                {
                    DataRow[] r_jy = t_成本.Select(string.Format("物料编码='{0}'", r_wcr["物料编码"]));
                    if (r_jy.Length > 0)
                    {
                        if (Convert.ToDecimal(r_jy[0]["发出单价"]) == 0 && (r_jy[0]["累计入库数量"] == DBNull.Value || r_jy[0]["累计入库数量"].ToString() == ""
                          || Convert.ToDecimal(r_jy[0]["累计入库数量"]) == 0))
                        {
                            r_jy[0]["发出单价"] = r_wcr["结存单价"];

                        }

                    }
                    else
                    {
                        DataRow[] pr = dt_存货核算.Select(string.Format("物料编码='{0}'", r_wcr["物料编码"]));
                        if (pr.Length > 0)
                        {
                            DataRow cr = t_成本.NewRow();
                            DataRow[] bs = t_基础.Select(string.Format("物料编码='{0}'", r_wcr["物料编码"]));
                            cr["物料名称"] = bs[0]["物料名称"];
                            cr["规格型号"] = bs[0]["规格型号"];
                            cr["存货分类"] = bs[0]["存货分类"];
                            cr["存货分类编码"] = bs[0]["存货分类编码"];
                            cr["物料编码"] = r_wcr["物料编码"];
                            cr["发出单价"] = pr[0]["发出单价"];
                            cr["收入单价"] = pr[0]["收入单价"];
                            cr["累计入库金额"] = Math.Round(Convert.ToDecimal(pr[0]["累计入库金额"]), 2, MidpointRounding.AwayFromZero);
                            cr["累计入库数量"] = pr[0]["累计入库数量"];

                            //cr["结存单价"] = r_wcr["结存单价"];
                            t_成本.Rows.Add(cr);
                        }
                        else
                        {
                            DataRow cr = t_成本.NewRow();
                            DataRow[] bs = t_基础.Select(string.Format("物料编码='{0}'", r_wcr["物料编码"]));
                            cr["物料名称"] = bs[0]["物料名称"];
                            cr["规格型号"] = bs[0]["规格型号"];
                            cr["存货分类"] = bs[0]["存货分类"];
                            cr["存货分类编码"] = bs[0]["存货分类编码"];
                            cr["物料编码"] = r_wcr["物料编码"];
                            cr["发出单价"] = r_wcr["结存单价"];
                            cr["收入单价"] = 0;
                            cr["累计入库金额"] = 0;
                            cr["累计入库数量"] = 0;
                            //cr["结存单价"] = r_wcr["结存单价"];
                            t_成本.Rows.Add(cr);
                        }
                    }
                }

                //19-8-12 注释修改
                DataRow[] yyr = dt_本期耗用.Select("发出单价=0");
                List<string> li = new List<string>();
                foreach (DataRow dr in yyr)
                {
                    if (dr["子项编码"].ToString().Substring(0, 2) == "05" || dr["子项编码"].ToString().Substring(0, 2) == "10")
                    {
                        DataRow[] vv = t_成本.Select(string.Format("物料编码='{0}'", dr["子项编码"])); //vv.lenth 恒>0
                        decimal dec_fc = 0;
                        if (vv[0]["发出单价"] != null && vv[0]["发出单价"].ToString() != "")
                        {
                            dec_fc = Convert.ToDecimal(vv[0]["发出单价"]);
                        }
                        if (Convert.ToDecimal(dr["发出单价"]) == 0 && dec_fc != 0)
                        {
                            dr["发出单价"] = vv[0]["发出单价"];
                            decimal dec_耗用 = Convert.ToDecimal(dr["发出单价"]); //本月耗用单价
                            decimal dec_本期金额 = 0;
                            DataRow[] rt_上期 = t_上期在制.Select(string.Format("生产工单号='{0}' and 子项编码='{1}'", dr["生产工单号"], dr["子项编码"]));
                            if (rt_上期.Length > 0)
                            {
                                dec_本期金额 = Math.Round(Convert.ToDecimal(dr["子项当期领用"]) * dec_耗用, 2, MidpointRounding.AwayFromZero);
                                if (Convert.ToDecimal(dr["子项当期领用"]) + Convert.ToDecimal(rt_上期[0]["在制品"]) != 0)
                                {
                                    dec_耗用 = (Convert.ToDecimal(rt_上期[0]["在制金额"]) + dec_本期金额) / (Convert.ToDecimal(dr["子项当期领用"]) + Convert.ToDecimal(rt_上期[0]["在制品"]));
                                }
                                else
                                {
                                    dec_耗用 = Math.Abs((Convert.ToDecimal(rt_上期[0]["在制金额"]) / Convert.ToDecimal(rt_上期[0]["在制品"])));
                                    dec_耗用 = Math.Round(dec_耗用, 6, MidpointRounding.AwayFromZero);
                                }

                            }
                            dr["耗用单价"] = dec_耗用 = Math.Round(dec_耗用, 6, MidpointRounding.AwayFromZero); ;
                        }
                    }
                    if (!li.Contains(dr["生产工单号"].ToString()))
                    {
                        li.Add(dr["生产工单号"].ToString());
                    }
                    ///此处递归 dr["产品编码"] 对应的工单 
                }


                foreach (string l in li) //需要重算的物料 
                {
                    DataRow[] ppp = t_工单.Select(string.Format("生产工单号='{0}'", l));
                    List<string> ls = new List<string>();
                    NewMethod(dt_结转, t_工单, dt_本期耗用, t_成本, t_上期在制, ppp[0]["物料编码"].ToString(), ls);
                }

                //    //19-8-9 需要找所有父项 都要重新算 不然耗用表不对
                //    DataTable dt_父项 = new DataTable();
                //    dt_父项 = ERPorg.Corg.fun_GetFather(dt_父项,dr["产品编码"].ToString(),1,false);
                //    foreach (DataRow r_f in dt_父项.Rows)
                //    {
                //        DataRow [] rowx= t_工单.Select(string.Format("物料编码='{0}'",r_f["产品编码"]));
                //        foreach (DataRow rowxx in rowx )
                //        {

                //            if (!li.Contains(rowxx["生产工单号"].ToString()))
                //            {
                //                li.Add(rowxx["生产工单号"].ToString());
                //            }

                //        }

                //    }
                //}
                ///19-8-11修改
                //foreach (DataRow dr in dt_本期耗用.Rows)
                //{

                //    if ((dr["子项编码"].ToString().Substring(0, 2) == "05" || dr["子项编码"].ToString().Substring(0, 2) == "10")&& Convert.ToDecimal(dr["发出单价"] ==0 )
                //    {

                //        DataRow[] vv = t_成本.Select(string.Format("物料编码='{0}'", dr["子项编码"])); //vv.lenth 恒>0
                //                                                                             //if (Convert.ToDecimal(dr["发出单价"]) == 0 && Convert.ToDecimal(vv[0]["发出单价"]) != 0)
                //                                                                             //{
                //        dr["发出单价"] = vv[0]["发出单价"];
                //        //dr["耗用单价"] = vv[0]["发出单价"];
                //        decimal dec_耗用 = Convert.ToDecimal(dr["发出单价"]); //本月耗用单价
                //        decimal dec_本期金额 = 0;
                //        DataRow[] rt_上期 = t_上期在制.Select(string.Format("生产工单号='{0}' and 子项编码='{1}'", dr["生产工单号"], dr["子项编码"]));
                //        if (rt_上期.Length > 0)
                //        {
                //            dec_本期金额 = Math.Round(Convert.ToDecimal(dr["子项当期领用"]) * dec_耗用, 2, MidpointRounding.AwayFromZero);
                //            if (Convert.ToDecimal(dr["子项当期领用"]) + Convert.ToDecimal(rt_上期[0]["在制品"]) != 0)
                //            {
                //                dec_耗用 = (Convert.ToDecimal(rt_上期[0]["在制金额"]) + dec_本期金额) / (Convert.ToDecimal(dr["子项当期领用"]) + Convert.ToDecimal(rt_上期[0]["在制品"]));
                //            }
                //            else
                //            {
                //                dec_耗用 = Math.Abs((Convert.ToDecimal(rt_上期[0]["在制金额"]) / Convert.ToDecimal(rt_上期[0]["在制品"])));
                //                dec_耗用 = Math.Round(dec_耗用, 6, MidpointRounding.AwayFromZero);
                //            }

                //        }
                //        dr["耗用单价"] = dec_耗用;

                //        //}
                //        if (!li.Contains(dr["生产工单号"].ToString()))
                //        {
                //            li.Add(dr["生产工单号"].ToString());
                //        }
                //    }
                //}
                //19-8-12 修改 此处需要递归





                //foreach (string l in li) //需要重算的物料 
                //{


                //    DataRow[] gd = t_工单.Select(string.Format("生产工单号='{0}'", l));

                //    DataRow[] r_本期消耗 = dt_本期耗用.Select(string.Format("生产工单号='{0}'", l));
                //    decimal dec_累计金额 = 0;

                //    foreach (DataRow e in r_本期消耗)
                //    {
                //        dec_累计金额 += Math.Round(Convert.ToDecimal(e["本期耗用数量"]) * Convert.ToDecimal(e["耗用单价"]), 2, MidpointRounding.AwayFromZero);
                //    }

                //    gd[0]["材料金额"] = Math.Round(dec_累计金额, 2, MidpointRounding.AwayFromZero);
                //    gd[0]["总金额"] = Math.Round(Convert.ToDecimal(gd[0]["材料金额"]) + Convert.ToDecimal(gd[0]["辅材分摊"]) +
                //        Convert.ToDecimal(gd[0]["人工费用"]) + Convert.ToDecimal(gd[0]["制造费用"]) + Convert.ToDecimal(gd[0]["软件费用"]), 2, MidpointRounding.AwayFromZero);
                //    if (Convert.ToDecimal(gd[0]["当期完成数量"]) == 0) gd[0]["入库单价"] = 0;
                //    else
                //    {
                //        gd[0]["入库单价"] = Math.Round(Convert.ToDecimal(gd[0]["总金额"]) / Convert.ToDecimal(gd[0]["当期完成数量"]), 6, MidpointRounding.AwayFromZero);
                //    }
                //    DataRow[] yy = t_成本.Select(string.Format("物料编码='{0}'", gd[0]["物料编码"]));
                //    decimal dec_期初金额 = 0;
                //    decimal dec_期初数量 = 0;
                //    decimal je_sum = 0;
                //    decimal dec_sum = 0;
                //    DataRow[] xx = dt_结转.Select(string.Format("物料编码='{0}'", gd[0]["物料编码"]));
                //    if (xx.Length == 0)
                //    {
                //        dec_期初金额 = 0;
                //        dec_期初数量 = 0;
                //    }
                //    else
                //    {
                //        dec_期初金额 = Convert.ToDecimal(xx[0]["本月结转金额"]);
                //        dec_期初数量 = Convert.ToDecimal(xx[0]["本月结转数量"]);
                //    }

                //    //yy[0]["累计入库金额"] = Math.Round(Convert.ToDecimal(r_gd["材料金额"])  +Convert.ToDecimal(r_gd["辅材分摊"]) + Convert.ToDecimal(r_gd["人工费用"])
                //    //    + Convert.ToDecimal(r_gd["制造费用"]) + Convert.ToDecimal(r_gd["软件费用"]) + je_sum,2, MidpointRounding.AwayFromZero);
                //    yy[0]["累计入库金额"] = Math.Round(Convert.ToDecimal(gd[0]["总金额"]) + je_sum, 2, MidpointRounding.AwayFromZero);
                //    yy[0]["累计入库数量"] = Convert.ToDecimal(gd[0]["当期完成数量"]) + dec_sum;
                //    decimal dec = Convert.ToDecimal(yy[0]["累计入库数量"]);
                //    if (dec == 0)
                //    {
                //        yy[0]["收入单价"] = 0;
                //    }
                //    else
                //    {
                //        yy[0]["收入单价"] = Math.Round(Convert.ToDecimal(yy[0]["累计入库金额"]) / dec, 6, MidpointRounding.AwayFromZero);
                //    }
                //    if (dec_期初数量 + dec == 0)
                //    { yy[0]["发出单价"] = 0; }
                //    else yy[0]["发出单价"] = Math.Round((dec_期初金额 + Convert.ToDecimal(yy[0]["累计入库金额"])) / (dec_期初数量 + dec), 6, MidpointRounding.AwayFromZero);


                //}
                string Path_bqhy = DesktopPath + @"\本期耗用.xlsx";
                if (!File.Exists(Path_bqhy))
                {
                    File.Create(Path_bqhy).Dispose(); ;
                }

                ERPorg.Corg.TableToExcel(dt_本期耗用, Path_bqhy);
                RBQ = new MasterMESWS.DataSetHelper();
                t_gd = RBQ.SelectGroupByInto("", t_工单, "物料编码,sum(总金额) 总金额,sum(材料金额) 材料金额,sum(辅材分摊) 辅材分摊,sum(人工费用) 人工费用,sum(制造费用) 制造费用,sum(当期完成数量) 当期完成数量,sum(软件费用) 软件费用", "", "物料编码");
                foreach (DataRow r_gd in t_gd.Rows)
                {

                    decimal dec_期初金额 = 0;
                    decimal dec_期初数量 = 0;
                    decimal je_sum = 0;
                    decimal dec_sum = 0;
                    DataRow[] xx = dt_结转.Select(string.Format("物料编码='{0}'", r_gd["物料编码"]));
                    if (xx.Length == 0)
                    {
                        dec_期初金额 = 0;
                        dec_期初数量 = 0;
                    }
                    else
                    {
                        dec_期初金额 = Convert.ToDecimal(xx[0]["本月结转金额"]);
                        dec_期初数量 = Convert.ToDecimal(xx[0]["本月结转数量"]);
                    }
                    //入库有金额
                    DataRow[] rrr = dt_存货核算.Select(string.Format("物料编码='{0}'", r_gd["物料编码"]));
                    if (rrr.Length > 0) //先取 dt_存货核算中的 金额 和 数量
                    {
                        if (rrr[0]["累计入库金额"] == DBNull.Value || rrr[0]["累计入库金额"].ToString() == "")
                            je_sum = 0;
                        else
                        { je_sum = Convert.ToDecimal(rrr[0]["累计入库金额"]); }
                        if (rrr[0]["累计入库数量"] == DBNull.Value || rrr[0]["累计入库数量"].ToString() == "")
                            dec_sum = 0;
                        else dec_sum = Convert.ToDecimal(rrr[0]["累计入库数量"]);

                    }
                    DataRow[] yy = t_成本.Select(string.Format("物料编码='{0}'", r_gd["物料编码"]));
                    if (yy.Length == 0)
                    {
                        DataRow rr = t_成本.NewRow();
                        rr["物料编码"] = r_gd["物料编码"];
                        DataRow[] bs = t_基础.Select(string.Format("物料编码='{0}'", r_gd["物料编码"]));
                        rr["物料名称"] = bs[0]["物料名称"];
                        rr["规格型号"] = bs[0]["规格型号"];
                        rr["存货分类"] = bs[0]["存货分类"];
                        rr["存货分类编码"] = bs[0]["存货分类编码"];
                        //rr["累计入库金额"] = Math.Round(Convert.ToDecimal(r_gd["材料金额"]) + Convert.ToDecimal(r_gd["辅材分摊"]) + Convert.ToDecimal(r_gd["人工费用"])
                        //    + Convert.ToDecimal(r_gd["制造费用"]) + Convert.ToDecimal(r_gd["软件费用"]) + je_sum,2,MidpointRounding.AwayFromZero);
                        rr["累计入库金额"] = Math.Round(Convert.ToDecimal(r_gd["总金额"]) + je_sum, 2, MidpointRounding.AwayFromZero);
                        rr["累计入库数量"] = Convert.ToDecimal(r_gd["当期完成数量"]) + dec_sum;
                        decimal dec = Convert.ToDecimal(rr["累计入库数量"]);
                        if (dec == 0)
                        {
                            rr["收入单价"] = 0;
                        }
                        else
                        {
                            rr["收入单价"] = Math.Round(Convert.ToDecimal(rr["累计入库金额"]) / dec, 6, MidpointRounding.AwayFromZero);
                        }
                        if (dec_期初数量 + dec == 0)
                        { rr["发出单价"] = 0; }
                        else rr["发出单价"] = Math.Round((dec_期初金额 + Convert.ToDecimal(rr["累计入库金额"])) / (dec_期初数量 + dec), 6, MidpointRounding.AwayFromZero);
                        t_成本.Rows.Add(rr);
                    }
                    else
                    {
                        //yy[0]["累计入库金额"] = Math.Round(Convert.ToDecimal(r_gd["材料金额"]) + Convert.ToDecimal(r_gd["辅材分摊"]) + Convert.ToDecimal(r_gd["人工费用"])
                        //    + Convert.ToDecimal(r_gd["制造费用"]) + Convert.ToDecimal(r_gd["软件费用"]) + je_sum,2,MidpointRounding.AwayFromZero);
                        yy[0]["累计入库金额"] = Math.Round(Convert.ToDecimal(r_gd["总金额"]) + je_sum, 2, MidpointRounding.AwayFromZero);
                        yy[0]["累计入库数量"] = Convert.ToDecimal(r_gd["当期完成数量"]) + dec_sum;
                        decimal dec = Convert.ToDecimal(yy[0]["累计入库数量"]);
                        if (dec == 0)
                        {
                            yy[0]["收入单价"] = 0;
                        }
                        else
                        {
                            yy[0]["收入单价"] = Math.Round(Convert.ToDecimal(yy[0]["累计入库金额"]) / dec, 6, MidpointRounding.AwayFromZero);
                        }
                        if (dec_期初数量 + dec == 0)
                        { yy[0]["发出单价"] = 0; }
                        else yy[0]["发出单价"] = Math.Round((dec_期初金额 + Convert.ToDecimal(yy[0]["累计入库金额"])) / (dec_期初数量 + dec), 6, MidpointRounding.AwayFromZero);
                    }
                }
                string Path_gd = DesktopPath + @"\工单.xlsx";
                if (!File.Exists(Path_gd))
                {
                    File.Create(Path_gd).Dispose(); ;
                }


                string Path_cb = DesktopPath + @"\成本.xlsx";
                if (!File.Exists(Path_cb))
                {
                    File.Create(Path_cb).Dispose(); ;
                }

                ERPorg.Corg.TableToExcel(t_工单, Path_gd);
                ERPorg.Corg.TableToExcel(t_成本, Path_cb);
                DataTable dt_工单_b = t_工单.Copy();
                dt_工单_b.TableName = "工单";
                ds_return.Tables.Add(dt_工单_b);
                DataTable dt_本期耗用_b = dt_本期耗用.Copy();
                dt_本期耗用_b.TableName = "本期耗用";
                ds_return.Tables.Add(dt_本期耗用_b);

                DataTable t_成本_b = t_成本.Copy();
                t_成本_b.TableName = "成本单价";
                ds_return.Tables.Add(t_成本_b);


                return ds_return;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private static void NewMethod(DataTable dt_结转, DataTable t_工单, DataTable dt_本期耗用, DataTable t_成本, DataTable t_上期在制, string cpbm, List<string> ls)
        {
            //gg.length==0 终止
            DataRow[] gg = t_工单.Select(string.Format("物料编码='{0}' ", cpbm));
            decimal d_lei = 0;
            decimal d_leiN = 0;
            foreach (DataRow gd in gg)
            {
                DataRow[] r_本期消耗 = dt_本期耗用.Select(string.Format("生产工单号='{0}'", gd["生产工单号"]));
                decimal dec_累计金额 = 0;
                foreach (DataRow dr in r_本期消耗)
                {

                    if (dr["子项编码"].ToString().Substring(0, 2) == "05" || dr["子项编码"].ToString().Substring(0, 2) == "10")
                    {
                        DataRow[] vv = t_成本.Select(string.Format("物料编码='{0}'", dr["子项编码"])); //vv.lenth 恒>0
                        decimal dec_fc = 0;
                        if (vv[0]["发出单价"] != null && vv[0]["发出单价"].ToString() != "")
                        {
                            dec_fc = Convert.ToDecimal(vv[0]["发出单价"]);
                        }
                        dr["发出单价"] = dec_fc;
                        decimal dec_耗用 = dec_fc; //本月耗用单价
                        decimal dec_本期金额 = 0;
                        DataRow[] rt_上期 = t_上期在制.Select(string.Format("生产工单号='{0}' and 子项编码='{1}'", dr["生产工单号"], dr["子项编码"]));
                        if (rt_上期.Length > 0)
                        {
                            dec_本期金额 = Math.Round(Convert.ToDecimal(dr["子项当期领用"]) * dec_耗用, 2, MidpointRounding.AwayFromZero);
                            if (Convert.ToDecimal(dr["子项当期领用"]) + Convert.ToDecimal(rt_上期[0]["在制品"]) != 0)
                            {
                                dec_耗用 = (Convert.ToDecimal(rt_上期[0]["在制金额"]) + dec_本期金额) / (Convert.ToDecimal(dr["子项当期领用"]) + Convert.ToDecimal(rt_上期[0]["在制品"]));
                                dec_耗用 = Math.Round(dec_耗用, 6, MidpointRounding.AwayFromZero);
                            }
                            else
                            {
                                dec_耗用 = Math.Abs((Convert.ToDecimal(rt_上期[0]["在制金额"]) / Convert.ToDecimal(rt_上期[0]["在制品"])));
                                dec_耗用 = Math.Round(dec_耗用, 6, MidpointRounding.AwayFromZero);
                            }
                            //当期没有领料或退料的  有期初的 发出单价和 耗用单价一样 
                            if (Convert.ToDecimal(dr["子项当期领用"]) <= 0)
                            {
                                dr["发出单价"] = dec_耗用;
                            }
                        }
                        dr["耗用单价"] = dec_耗用;

                    }

                    dec_累计金额 += Math.Round(Convert.ToDecimal(dr["本期耗用数量"]) * Convert.ToDecimal(dr["耗用单价"]), 2, MidpointRounding.AwayFromZero);
                }

                gd["材料金额"] = Math.Round(dec_累计金额, 2, MidpointRounding.AwayFromZero);
                gd["总金额"] = Math.Round(Convert.ToDecimal(gd["材料金额"]) + Convert.ToDecimal(gd["辅材分摊"]) +
                    Convert.ToDecimal(gd["人工费用"]) + Convert.ToDecimal(gd["制造费用"]) + Convert.ToDecimal(gd["软件费用"]), 2, MidpointRounding.AwayFromZero);
                if (Convert.ToDecimal(gd["当期完成数量"]) == 0) gd["入库单价"] = 0;
                else
                {
                    gd["入库单价"] = Math.Round(Convert.ToDecimal(gd["总金额"]) / Convert.ToDecimal(gd["当期完成数量"]), 6, MidpointRounding.AwayFromZero);
                }
                d_lei += Convert.ToDecimal(gd["总金额"]);
                d_leiN += Convert.ToDecimal(gd["当期完成数量"]);
            }
            DataRow[] yy = t_成本.Select(string.Format("物料编码='{0}'", cpbm));
            decimal dec_期初金额 = 0;
            decimal dec_期初数量 = 0;

            DataRow[] xx = dt_结转.Select(string.Format("物料编码='{0}'", cpbm));
            if (xx.Length == 0)
            {
                dec_期初金额 = 0;
                dec_期初数量 = 0;
            }
            else
            {
                dec_期初金额 = Convert.ToDecimal(xx[0]["本月结转金额"]);
                dec_期初数量 = Convert.ToDecimal(xx[0]["本月结转数量"]);
            }
            //yy[0]["累计入库金额"] = Math.Round(Convert.ToDecimal(r_gd["材料金额"])  +Convert.ToDecimal(r_gd["辅材分摊"]) + Convert.ToDecimal(r_gd["人工费用"])
            //    + Convert.ToDecimal(r_gd["制造费用"]) + Convert.ToDecimal(r_gd["软件费用"]) + je_sum,2, MidpointRounding.AwayFromZero);
            yy[0]["累计入库金额"] = Math.Round(d_lei, 2, MidpointRounding.AwayFromZero);
            yy[0]["累计入库数量"] = d_leiN;
            decimal dec = Convert.ToDecimal(yy[0]["累计入库数量"]);
            if (dec == 0)
            {
                yy[0]["收入单价"] = 0;
            }
            else
            {
                yy[0]["收入单价"] = Math.Round(Convert.ToDecimal(yy[0]["累计入库金额"]) / dec, 6, MidpointRounding.AwayFromZero);
            }
            if (dec_期初数量 + dec == 0)
            { yy[0]["发出单价"] = 0; }
            else
            {
                yy[0]["发出单价"] = Math.Round((dec_期初金额 + Convert.ToDecimal(yy[0]["累计入库金额"])) / (dec_期初数量 + dec), 6, MidpointRounding.AwayFromZero);
            }
            if (!ls.Contains(cpbm))
            {

                ls.Add(cpbm);
                DataRow[] x = dt_本期耗用.Select(string.Format("子项编码='{0}'", cpbm));
                foreach (DataRow xr in x)
                {
                    //if (!ls.Contains(cpbm))
                    //{
                    ls.Add(xr["产品编码"].ToString());
                    NewMethod(dt_结转, t_工单, dt_本期耗用, t_成本, t_上期在制, xr["产品编码"].ToString(), ls);

                    // }
                }
            }
        }





        /// <summary>
        /// 先算05码的生产成本 作为 上面存货核算计算时 委外的发出单价 ，此单价需更新到 当期委外发料单上去
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public DataTable Cal_半成品(DataSet ds, DateTime t1, DateTime t2)
        {
            DataTable dt_存货核算 = ds.Tables[0];
            DataTable dt_结转 = ds.Tables[1];
            string s = "";

            DateTime t0 = t1.AddMonths(-1);

            //期初在制
            s = string.Format(@"select  * from [财务即时库存记录] where 时间>'{0}' and 时间<'{1}'", t0, t1);
            DataTable t_上期在制 = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            s = string.Format(@"
            declare @tabel Table
            (产品编码 [nvarchar](20) 
            ,子项编码 [nvarchar](20) 
           ,数量 [decimal](18,6) );
        with temp_bom(z,产品编码,子项编码,仓库号,仓库名称,wiptype,子项类型,数量,bom类型,优先级,bom_level ) as
       (select 产品编码 as z,产品编码,子项编码,仓库号,仓库名称,WIPType,子项类型,数量,bom类型,优先级,1 as level from 基础数据物料BOM表   
        union all 
          select b.产品编码 as z, a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型, convert(decimal(18, 6),a.数量*b.数量) as 数量,
           a.bom类型,a.优先级,b.bom_level+1  from 基础数据物料BOM表 a
          inner join temp_bom b on a.产品编码=b.子项编码 where b.wiptype='虚拟') 
          
          insert @tabel(产品编码,子项编码, 数量)
           select  z,子项编码, sum(数量)数量 from  temp_bom    where wiptype ='入库倒冲' and 优先级=1
          group by z  ,子项编码  

      select 生产工单号,领料类型,SUM(待领料总量)待领料总量,SUM(已领) 已领,CONVERT(decimal(18,6),sum(BOM数量)) BOM数量,物料编码,产品编码,生产数量 from (
       select  mx.生产工单号,领料类型 ,待领料总量,已领,BOM数量,物料编码,产品编码 ,生产数量  from 生产记录生产工单待领料明细表  mx
      left join 生产记录生产工单待领料主表 zb on mx.待领料单号 =zb.待领料单号 
      left join (select  待领料单明细号,SUM(领料数量)已领 from 生产记录生产领料单明细表 where 生效日期 >'{1}' and 生效日期<'{2}' group by 待领料单明细号)yl 
      on  yl.待领料单明细号=mx.待领料单明细号        
       where 领料类型<>'生产补料' and mx.生产工单类型<>'生产补料'
            and mx.生产工单号 in ( select  生产工单号  from (
           select gd.生产工单号,生产工单类型,物料编码,生产数量,isnull(累计入库量,0)累计入库量,ISNULL(jy.报废数,0)报废数,gd.关闭  from 生产记录生产工单表 gd
           left join (select 生产工单号,sum(入库数量)累计入库量 from 生产记录成品入库单明细表 where  生效日期>'{1}' and 生效日期<'{2}' group by 生产工单号)x 
           on x.生产工单号=gd.生产工单号 
           left join (select  生产工单号,sum(报废数)报废数  from 生产记录生产检验单主表 where 报废数 >0 and  生效日期>'{1}' and 生效日期<'{2}' group by 生产工单号)jy
           on jy.生产工单号=gd.生产工单号
            where   生效日期>'{1}' and 生效日期<'{2}' )y where  (累计入库量>0 or 关闭 =0)
            union  select  生产工单号 from 财务即时库存记录 where 时间>'{0}' and 时间<'{1}' 
            union  select  生产工单号 from 生产记录生产领料单明细表 where  生效日期>'{1}' and 生效日期<'{2}'  group by 生产工单号   
            union  select  生产工单号 from 生产记录成品入库单明细表 where  生效日期>'{1}' and 生效日期<'{2}'  group by 生产工单号 
            union   select  工单号 as 生产工单号  from 工单返库单主表 where 日期>'{1}' and 日期<'{2}' ) )gdbom   group by 生产工单号,领料类型,物料编码,产品编码,生产数量 
          union 
         select 生产工单号,'入库倒冲' as 领料类型,SUM(待领料总量)待领料总量,SUM(已领) 已领,BOM数量,物料编码,产品编码,生产数量 from (
     select  a.备注 as 生产工单号 ,a.数量 as 待领料总量,a.数量 as 已领,k.数量 as BOM数量,a.物料编码,gd.物料编码 as 产品编码,生产数量   from 其他出库子表 a
     left join 其他出入库申请主表   b on a.出入库申请单号=b.出入库申请单号
     left join 生产记录生产工单表 gd on gd.生产工单号 =a.备注
    left join  @tabel  k  on k.产品编码=gd.物料编码 and 子项编码=a.物料编码 
     where  存货核算标记 =0 and 原因分类 ='入库倒冲' and a.生效日期 >'{1}' and a.生效日期<'{2}')tt group by 生产工单号,BOM数量,物料编码,产品编码,生产数量 
      union 
       select  ff.工单号 生产工单号,'返工退料' 领料类型,-SUM(返库数量) 待领料总量,-SUM(返库数量)已领,SUM(返库数量)/avg(生产数量) BOM数量,ff.物料编码,gd.物料编码 as 产品编码,生产数量 
        from 工单返库单明细表 ff
         left join 工单返库单主表 f on f.退料单号=ff.退料单号
		 left join 工单退料申请表 fk on fk.待退料号=ff.待退料号
		 left join  生产记录生产工单表 gd on gd.生产工单号=ff.工单号
        where  ff.日期 >'{1}' and ff.日期<'{2}'  and 退料类型='返工退料' group by ff.工单号,ff.物料编码,gd.物料编码,生产数量", t0, t1, t2); //这里入库倒冲的还没有放进去 导致下面算的时候 漏了 入库倒冲的一块
            DataTable t_工单bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //19-11-6
            DataColumn[] pk1 = new DataColumn[3];
            pk1[0] = t_工单bom.Columns["生产工单号"];
            pk1[1] = t_工单bom.Columns["物料编码"];
            pk1[2] = t_工单bom.Columns["领料类型"];
            t_工单bom.PrimaryKey = pk1;


            foreach (DataRow dr in t_工单bom.Rows)
            {
                if (dr["BOM数量"] == DBNull.Value || Convert.ToDecimal(dr["BOM数量"]) == 0)
                {
                    dr["BOM数量"] = Convert.ToDecimal(dr["待领料总量"]) / Convert.ToDecimal(dr["生产数量"]);
                }
            }

            ///所有需要计算得工单 此处只需要半成品 05码  只需要修改后半个生效日期   5 -1为 正式启用系统时间
            s = $@"declare @t1 Table (生产工单号 [nvarchar](20) PRIMARY KEY )
 insert into @t1(生产工单号)
 select  生产工单号 from 财务即时库存记录 where 时间>'{t0}' and 时间<'{t1}' group by 生产工单号 
 union 
   select 相关单号  as 生产工单号  from  仓库出入库明细表 where  明细类型 in ('领料出库','入库倒冲') 
            and 出入库时间>'{t1}' and 出入库时间<'{t2}'  group by 相关单号 
 union
         select 相关单号  as 生产工单号  from 仓库出入库明细表 where 明细类型 in ('返工退料','工单关闭退料','工单退料')
            and 出入库时间>'{t1}' and 出入库时间<'{t2}' group by 相关单号 
            
 declare @t2 Table (生产工单号  [nvarchar](20) PRIMARY KEY,生产工单类型 [nvarchar](10),物料编码 [nvarchar](20),生产数量 decimal(18,2),当期入库量 decimal(18,2) 
                  ,累计入库量 decimal(18,2),累计报废数  decimal(18,2),当期报废数 decimal(18,2),关闭 int  );
 insert into @t2(生产工单号 ,生产工单类型 ,物料编码,生产数量 ,当期入库量,累计入库量 ,累计报废数 ,当期报废数 ,关闭 )    
  select gd.生产工单号,生产工单类型,物料编码,生产数量,ISNULL(当期入库量,0)当期入库量,isnull(累计入库量,0)累计入库量,ISNULL(ljjy.累计报废数,0)累计报废数
           ,ISNULL(jy.当期报废数,0)当期报废数,case when  gd.关闭=1 and gd.关闭日期<'{t2}' then 1 else 0 end as 关闭  from 生产记录生产工单表 gd
           left join (select 生产工单号,sum(入库数量)当期入库量 from 生产记录成品入库单明细表 where  生效日期>'{t1}' and 生效日期<'{t2}' group by 生产工单号)x 
           on x.生产工单号=gd.生产工单号 
               left join (select 生产工单号,sum(入库数量)累计入库量 from 生产记录成品入库单明细表 where  生效日期>'2019-5-1' and 生效日期<'{t2}' group by 生产工单号)lj
           on lj.生产工单号=gd.生产工单号 
           left join (select  生产工单号,sum(报废数)累计报废数  from 生产记录生产检验单主表 where 报废数 >0 and  生效日期>'2019-5-1' and 生效日期<'{t2}' group by 生产工单号)ljjy
           on ljjy.生产工单号=gd.生产工单号
            left join (select  生产工单号,sum(报废数)当期报废数  from 生产记录生产检验单主表 where 报废数 >0 and  生效日期>'{t1}' and 生效日期<'{t2}' group by 生产工单号)jy
           on jy.生产工单号=gd.生产工单号
            where   (生效日期>'{t1}' and 生效日期<'{t2}')  or gd.生产工单号 in (select  生产工单号 from @t1) 
 
      select *,当期入库量 as 当期完成数量 from (
           select  *,case  when 关闭=1 then 0 else 生产数量-累计入库量-累计报废数  end as  期末数量   from    @t2 )yy  ";   //0 为上期初  1 为 上期末 本期初，2 为 本期末  5-1 为系统起始时间
            DataTable t_工单 = CZMaster.MasterSQL.Get_DataTable(s, strcon); // 其中包括期末在制数量

            t_工单.Columns.Add("入库单价", typeof(decimal));
            t_工单.Columns.Add("材料金额", typeof(decimal));
            t_工单.Columns.Add("总金额", typeof(decimal));
            DataColumn dc = new DataColumn("辅材分摊", typeof(decimal));
            DataColumn dc1 = new DataColumn("工时", typeof(decimal));
            DataColumn dc2 = new DataColumn("工单工时", typeof(decimal));
            DataColumn dc3 = new DataColumn("制造费用", typeof(decimal));
            DataColumn dc4 = new DataColumn("人工费用", typeof(decimal));
            DataColumn dc5 = new DataColumn("软件费用", typeof(decimal));
            dc.DefaultValue = 0;
            dc3.DefaultValue = 0;
            dc4.DefaultValue = 0;
            dc5.DefaultValue = 0;
            t_工单.Columns.Add(dc);
            t_工单.Columns.Add(dc1);
            t_工单.Columns.Add(dc2);
            t_工单.Columns.Add(dc3);
            t_工单.Columns.Add(dc4);
            t_工单.Columns.Add(dc5);
            //19-11-6
            DataColumn[] pk = new DataColumn[1];
            pk[0] = t_工单.Columns["生产工单号"];
            t_工单.PrimaryKey = pk;

            //不参与运算
            //DataRow[] rrrr = t_工单.Select(string.Format(@"生产工单号 in ('MO1907050027','MO1907180015','MO1908080002','MO1908090011','MO1908170003','MO1908210019'
            //,'MO1908210020','MO1908210021','MO1908210022','MO1908230018','MO1908270018','MO1908280002','MO1908280007','MO1908290006','MO1908300045')"));
            DataRow[] rrrr = t_工单.Select(string.Format(@"生产工单号 in ('MO1910250020','MO1911070017','MO1911070019','MO2001200005','MO2005120003')"));
            foreach (DataRow dr in rrrr)
            {
                t_工单.Rows.Remove(dr);
            }

            rrrr = t_工单.Select(string.Format(@"生产工单号 in ('MO1911070019' )"));

            // s = string.Format(@"select *,当期入库量+当期报废数 as 当期完成数 from (select  *,case  when 关闭=1 then 0 else 生产数量-累计入库量-报废数  end as  期末数量  from (
            //select gd.生产工单号,生产工单类型,物料编码,生产数量,isnull(累计入库量,0)累计入库量,ISNULL(jy.报废数,0)报废数,gd.关闭  from 生产记录生产工单表 gd
            //left join (select 生产工单号,sum(入库数量)累计入库量 from 生产记录成品入库单明细表 where  生效日期>'2019-5-1' and 生效日期<'{0}' group by 生产工单号)x 
            //on x.生产工单号=gd.生产工单号 
            //left join (select  生产工单号,sum(报废数)报废数  from 生产记录生产检验单主表 where 报废数 >0 and  生效日期>'2019-5-1' and 生效日期<'{0}' group by 生产工单号)jy
            // on jy.生产工单号=gd.生产工单号 where   生效日期>'2019-5-1' and 生效日期<'{0}' )y where  生产数量-累计入库量-报废数>0 
            // and (累计入库量>0 or 关闭 =0))y  where y.期末数量>0 ", t2);
            // DataTable t_在制工单 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //算期末在制物料 6-21
            //foreach (DataRow r in t_在制工单.Rows)
            //{
            //    DataRow[] r_工单bom = t_工单bom.Select(string.Format("生产工单号='{0}'", r["生产工单号"].ToString()));
            //    decimal dec_期末数量 = Convert.ToDecimal(r["期末数量"]);
            //    foreach (DataRow r_在制 in r_工单bom)
            //    {
            //        decimal dec_bom = Convert.ToDecimal(r_在制["BOM数量"]);
            //        if (dec_bom == 0)
            //        {
            //            dec_bom = Convert.ToDecimal(r_在制["待领料总数"]) / Convert.ToDecimal(r_在制["生产数量"]);
            //        }
            //        DataRow rAdd = t_本期在制.NewRow();
            //        rAdd["生产工单号"] = r_在制["生产工单号"];
            //        rAdd["产品编码"] = r_在制["产品编码"];
            //        rAdd["子项编码"] = r_在制["物料编码"];
            //        rAdd["生产数量"] = r_在制["生产数量"];
            //        rAdd["待领料总数"] = r_在制["待领料总数"];
            //        rAdd["已领数量"] = r_在制["已领"];
            //        rAdd["时间"] = t2.AddDays(-1);
            //        //这里如果存在 完成数量大于 最小发料套数就是不对得
            //        //按照工单bom 和期末数量 算在制 
            //        rAdd["在制品"] = Convert.ToDecimal(r_在制["期末数量"]) * dec_bom;
            //        t_本期在制.Rows.Add(rAdd);
            //    }
            //}

            DataTable dt_本期耗用 = new DataTable();
            dt_本期耗用.Columns.Add("生产工单号");
            dt_本期耗用.Columns.Add("生产数量", typeof(decimal));
            dt_本期耗用.Columns.Add("产品编码");
            dt_本期耗用.Columns.Add("本期入库数量", typeof(decimal));
            dt_本期耗用.Columns.Add("累计报废数", typeof(decimal));
            dt_本期耗用.Columns.Add("当期报废数", typeof(decimal));
            dt_本期耗用.Columns.Add("累计入库数量", typeof(decimal));
            dt_本期耗用.Columns.Add("子项编码");
            dt_本期耗用.Columns.Add("本期耗用数量", typeof(decimal));
            dt_本期耗用.Columns.Add("子项期初数", typeof(decimal));
            dt_本期耗用.Columns.Add("子项期初金额", typeof(decimal));

            dt_本期耗用.Columns.Add("子项当期领用", typeof(decimal));
            dt_本期耗用.Columns.Add("子项在制数", typeof(decimal));
            dt_本期耗用.Columns.Add("发出单价", typeof(decimal));
            dt_本期耗用.Columns.Add("耗用单价", typeof(decimal));
            //19-11-6
            DataColumn[] pk_hy = new DataColumn[2];
            pk_hy[0] = dt_本期耗用.Columns["生产工单号"];
            pk_hy[1] = dt_本期耗用.Columns["子项编码"];
            dt_本期耗用.PrimaryKey = pk_hy;


            //本期领料 总领料量+返库的   还要加入 入库倒冲的料
            s = string.Format(@"with t as (select  a.备注  from 其他出库子表 a
              left join 其他出入库申请主表  b on a.出入库申请单号 =b.出入库申请单号
             where 原因分类='入库倒冲' and a.生效日期 >'{0}' and b.生效日期<'{1}' and 存货核算标记 =1)
   ,t1 as (select 相关单号  as 生产工单号,物料编码,0 领料数量,sum(实效数量)返库数量 from 仓库出入库明细表 
             where 明细类型 in ('返工退料','工单关闭退料','工单退料')
            and 出入库时间>'{0}' and 出入库时间<'{1}' 
           group by 相关单号,物料编码) 
    ,t2 as (select  生产工单号,物料编码,SUM(领料数量)领料数量,SUM(返库数量)返库数量 from (
                 select 相关单号  as 生产工单号,物料编码,-sum(实效数量)领料数量,0 返库数量 from  仓库出入库明细表 where  明细类型 in ('领料出库','入库倒冲') 
            and 出入库时间>'{0}' and 出入库时间<'{1}' /*and 相关单号 not in (select   备注 from  t)*/ group by 相关单号,物料编码 
             union   select  * from t1
              )x group by 生产工单号,物料编码)
   select  *,领料数量-返库数量 as 实际领料数 from  t2", t1, t2);
            DataTable t_领料全 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //19-6-26 算期末在制 和本期耗用： 期初 +当期领 -本期完工耗用（加补料）=期末


            string gbwtl = string.Format(@"select  生产工单号  from 工单退料申请表 where   
              (完成 = 0  or(完成 = 1 and  完成日期 > '{0}'))  and 作废 = 0   group by 生产工单号", t2);
            DataTable dt_关闭未退料 = CZMaster.MasterSQL.Get_DataTable(gbwtl, strcon);

            ///2019-8-8 领料中有一部分是替代料 在待领料明细中是 补料 然后在t_工单bom 中没有 需要加入 然后 标记为替代料
            foreach (DataRow dr in t_领料全.Rows)
            {
                if (Convert.ToDecimal(dr["实际领料数"]) != 0)
                {
                    DataRow[] rr = t_工单bom.Select(string.Format("生产工单号='{0}' and 物料编码='{1}' ", dr["生产工单号"], dr["物料编码"]));
                    if (rr.Length == 0)
                    {
                        DataRow rx = t_工单bom.NewRow();
                        rx["生产工单号"] = dr["生产工单号"];
                        rx["领料类型"] = "替代料";
                        rx["待领料总量"] = dr["实际领料数"];
                        rx["已领"] = dr["实际领料数"];
                        rx["物料编码"] = dr["物料编码"];

                        DataRow[] r_ls = t_工单.Select(string.Format("生产工单号='{0}'", dr["生产工单号"]));
                        rx["产品编码"] = r_ls[0]["物料编码"];
                        rx["生产数量"] = r_ls[0]["生产数量"];
                        t_工单bom.Rows.Add(rx);
                    }
                }
            }


            foreach (DataRow dr in t_工单.Rows)
            {
                if (dr["物料编码"].ToString().Substring(0, 2) == "10") continue;
                bool bl_完成 = false;
                if (Convert.ToDecimal(dr["期末数量"]) == 0) //已完成  所有都耗用 本期在制为0 
                {
                    bl_完成 = true;
                    //19-8-9 需要判断 关单关闭 尚有退料未完成部分 需要把bl_完成=false    期末数量 这个字段已 除此处外 基本无用 不用考虑
                    if (Convert.ToBoolean(dr["关闭"]))
                    {
                        DataRow[] r_gbwtl = dt_关闭未退料.Select(string.Format("生产工单号='{0}'", dr["生产工单号"]));
                        if (r_gbwtl.Length > 0)
                        {
                            bl_完成 = false;
                        }

                    }

                }
                DataRow[] y = t_上期在制.Select(string.Format("生产工单号='{0}'", dr["生产工单号"]));
                foreach (DataRow yy in y)
                {
                    DataRow xr = dt_本期耗用.NewRow();

                    xr["生产工单号"] = dr["生产工单号"];
                    xr["产品编码"] = dr["物料编码"];
                    xr["生产数量"] = dr["生产数量"];
                    xr["本期入库数量"] = dr["当期入库量"];
                    xr["累计入库数量"] = dr["累计入库量"];
                    xr["累计报废数"] = dr["累计报废数"];
                    xr["当期报废数"] = dr["当期报废数"];
                    xr["子项编码"] = yy["子项编码"];

                    xr["子项期初数"] = Convert.ToDecimal(yy["在制品"]);
                    xr["子项期初金额"] = Convert.ToDecimal(yy["在制金额"]);

                    DataRow[] r_sq = t_领料全.Select(string.Format("生产工单号='{0}' and 物料编码='{1}'", dr["生产工单号"], yy["子项编码"]));
                    if (r_sq.Length > 0) xr["子项当期领用"] = Convert.ToDecimal(r_sq[0]["实际领料数"]);
                    else xr["子项当期领用"] = 0;
                    if (bl_完成)
                    {
                        xr["本期耗用数量"] = Convert.ToDecimal(xr["子项期初数"]) + Convert.ToDecimal(xr["子项当期领用"]);
                        xr["子项在制数"] = 0;
                    }
                    else
                    {
                        DataRow[] re = t_工单bom.Select(string.Format("生产工单号='{0}' and 物料编码='{0}'", dr["生产工单号"], yy["子项编码"]));
                        if (re.Length > 0)
                        {
                            xr["本期耗用数量"] = Convert.ToDecimal(re[0]["BOM数量"]) * Convert.ToDecimal(dr["当期完成数量"]);
                            xr["子项在制数"] = Convert.ToDecimal(xr["子项期初数"]) + Convert.ToDecimal(xr["子项当期领用"]) - Convert.ToDecimal(xr["本期耗用数量"]);
                        }
                        else
                        {
                            xr["本期耗用数量"] = 0;
                            xr["子项在制数"] = Convert.ToDecimal(xr["子项期初数"]) + Convert.ToDecimal(xr["子项当期领用"]) - Convert.ToDecimal(xr["本期耗用数量"]);
                        }
                    }
                    dt_本期耗用.Rows.Add(xr);

                }
                //期初+当期领用（含退料） 全部耗用 
                //19-8-8 发现有些东西是替代料  待领料明细表里面没有  但是 领料全 里面有 
                DataRow[] r_工单bom = t_工单bom.Select(string.Format("生产工单号='{0}'", dr["生产工单号"]));
                // DataRow [] r_期初在制=t_上期在制.Select(string.Format("生产工单号='{0}'", dr["生产工单号"]));
                foreach (DataRow r in r_工单bom) //又在制先把在制加进去
                {
                    if (r["领料类型"].ToString() == "替代料")
                    {
                        DataRow[] erer = dt_本期耗用.Select(string.Format("生产工单号='{0}' and 子项编码='{1}'", dr["生产工单号"], r["物料编码"]));
                        if (erer.Length == 0)
                        {
                            DataRow xr = dt_本期耗用.NewRow();
                            xr["生产工单号"] = dr["生产工单号"];
                            xr["产品编码"] = dr["物料编码"];
                            xr["生产数量"] = dr["生产数量"];
                            xr["本期入库数量"] = dr["当期入库量"];
                            xr["累计入库数量"] = dr["累计入库量"];
                            xr["累计报废数"] = dr["累计报废数"];
                            xr["当期报废数"] = dr["当期报废数"];
                            xr["子项编码"] = r["物料编码"];
                            DataRow[] r_sq = t_上期在制.Select(string.Format("生产工单号='{0}' and 子项编码='{1}'", dr["生产工单号"], r["物料编码"]));
                            if (r_sq.Length > 0)
                            {
                                xr["子项期初数"] = Convert.ToDecimal(r_sq[0]["在制品"]);
                                xr["子项期初金额"] = Convert.ToDecimal(r_sq[0]["在制金额"]);
                            }
                            else
                            {
                                xr["子项期初数"] = 0;
                                xr["子项期初金额"] = 0;
                            }
                            r_sq = t_领料全.Select(string.Format("生产工单号='{0}' and 物料编码='{1}'", dr["生产工单号"], r["物料编码"]));
                            if (r_sq.Length > 0) xr["子项当期领用"] = Convert.ToDecimal(r_sq[0]["实际领料数"]);
                            else xr["子项当期领用"] = 0;
                            //替代料这边补领的全部耗用
                            xr["本期耗用数量"] = Convert.ToDecimal(xr["子项期初数"]) + Convert.ToDecimal(xr["子项当期领用"]);
                            xr["子项在制数"] = 0;
                            dt_本期耗用.Rows.Add(xr);

                        }

                        else
                        {
                            DataRow[] r_sq = t_领料全.Select(string.Format("生产工单号='{0}' and 物料编码='{1}'", dr["生产工单号"], r["物料编码"]));

                            if (r_sq.Length > 0) erer[0]["子项当期领用"] = Convert.ToDecimal(r_sq[0]["实际领料数"]);
                            erer[0]["本期耗用数量"] = Convert.ToDecimal(erer[0]["本期耗用数量"]) + Convert.ToDecimal(erer[0]["子项当期领用"]);//理论上只会有一次
                            erer[0]["子项在制数"] = 0;

                        }
                    }
                    else
                    {
                        DataRow[] erer = dt_本期耗用.Select(string.Format("生产工单号='{0}' and 子项编码='{1}'", dr["生产工单号"], r["物料编码"]));
                        if (erer.Length == 0)
                        {
                            DataRow xr = dt_本期耗用.NewRow();
                            xr["生产工单号"] = dr["生产工单号"];
                            xr["产品编码"] = dr["物料编码"];
                            xr["生产数量"] = dr["生产数量"];
                            xr["本期入库数量"] = dr["当期入库量"];
                            xr["累计入库数量"] = dr["累计入库量"];
                            xr["累计报废数"] = dr["累计报废数"];
                            xr["当期报废数"] = dr["当期报废数"];
                            xr["子项编码"] = r["物料编码"];
                            DataRow[] r_sq = t_上期在制.Select(string.Format("生产工单号='{0}' and 子项编码='{1}'", dr["生产工单号"], r["物料编码"]));
                            if (r_sq.Length > 0)
                            {
                                xr["子项期初数"] = Convert.ToDecimal(r_sq[0]["在制品"]);
                                xr["子项期初金额"] = Convert.ToDecimal(r_sq[0]["在制金额"]);
                            }
                            else
                            {
                                xr["子项期初数"] = 0;
                                xr["子项期初金额"] = 0;
                            }
                            r_sq = t_领料全.Select(string.Format("生产工单号='{0}' and 物料编码='{1}'", dr["生产工单号"], r["物料编码"]));
                            if (r_sq.Length > 0) xr["子项当期领用"] = Convert.ToDecimal(r_sq[0]["实际领料数"]);
                            else xr["子项当期领用"] = 0;
                            if (bl_完成)
                            {
                                xr["本期耗用数量"] = Convert.ToDecimal(xr["子项期初数"]) + Convert.ToDecimal(xr["子项当期领用"]);
                                xr["子项在制数"] = 0;
                            }
                            else
                            {
                                decimal dec_bom = Convert.ToDecimal(r["BOM数量"]);

                                xr["本期耗用数量"] = dec_bom * Convert.ToDecimal(dr["当期完成数量"]);
                                xr["子项在制数"] = Convert.ToDecimal(xr["子项期初数"]) + Convert.ToDecimal(xr["子项当期领用"]) - Convert.ToDecimal(xr["本期耗用数量"]);
                            }

                            dt_本期耗用.Rows.Add(xr);

                        }
                        else if (!bl_完成)
                        {
                            DataRow[] r_sq = t_领料全.Select(string.Format("生产工单号='{0}' and 物料编码='{1}'", dr["生产工单号"], r["物料编码"]));
                            if (r_sq.Length > 0) erer[0]["子项当期领用"] = Convert.ToDecimal(r_sq[0]["实际领料数"]);
                            erer[0]["本期耗用数量"] = Convert.ToDecimal(r["BOM数量"]) * Convert.ToDecimal(dr["当期完成数量"]);
                            erer[0]["子项在制数"] = Convert.ToDecimal(erer[0]["子项期初数"]) + Convert.ToDecimal(erer[0]["子项当期领用"]) - Convert.ToDecimal(erer[0]["本期耗用数量"]);
                        }
                        else
                        {
                            DataRow[] r_sq = t_领料全.Select(string.Format("生产工单号='{0}' and 物料编码='{1}'", dr["生产工单号"], r["物料编码"]));
                            if (r_sq.Length > 0) erer[0]["子项当期领用"] = Convert.ToDecimal(r_sq[0]["实际领料数"]);
                            erer[0]["本期耗用数量"] = Convert.ToDecimal(erer[0]["子项期初数"]) + Convert.ToDecimal(erer[0]["子项当期领用"]);

                            erer[0]["子项在制数"] = 0;

                        }
                    }
                }



                //if (Convert.ToDecimal(dr["期末数量"]) == 0) continue;
                ////期末数量
                //DataRow rAdd = t_本期在制.NewRow();
                //rAdd["生产工单号"] = dr["生产工单号"];
                //rAdd["产品编码"] = dr["物料编码"];
                //rAdd["子项编码"] = r_在制["物料编码"];
                //rAdd["生产数量"] = r_在制["生产数量"];
                //rAdd["待领料总数"] = r_在制["待领料总数"];
                //rAdd["已领数量"] = r_在制["已领"];
                //rAdd["时间"] = t2.AddDays(-1);
                ////这里如果存在 完成数量大于 最小发料套数就是不对得
                ////按照工单bom 和期末数量 算在制 
                //rAdd["在制品"] = Convert.ToDecimal(r_在制["期末数量"]) * dec_bom;
                //t_本期在制.Rows.Add(rAdd);
            }

            s = "select  物料编码,规格型号,物料名称,存货分类,存货分类编码 from 基础数据物料信息表  ";
            DataTable t_基础 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //19-11-6
            DataColumn[] pk_基础 = new DataColumn[1];
            pk_基础[0] = t_基础.Columns["物料编码"];
            t_基础.PrimaryKey = pk_基础;
            //t_领料全- 期末在制= 本期入库耗用明细
            //DataTable t_本期耗用 = t_领料全.Copy();
            //t_本期耗用.Columns.Add("本期耗用", typeof(decimal));
            //foreach (DataRow r_本期耗用 in t_本期耗用.Rows)
            //{
            //    decimal dec = 0;
            //    DataRow[] rr = t_本期在制.Select(string.Format("生产工单号='{0}' and 子项编码='{1}'", r_本期耗用["生产工单号"], r_本期耗用["物料编码"]));
            //    if (rr.Length > 0)
            //    {
            //        dec = Convert.ToDecimal(rr[0]["在制品"]);
            //    }
            //    r_本期耗用["本期耗用"] = Convert.ToDecimal(r_本期耗用["实际领料数"]) - dec;
            //}

            ds.Tables.Add(dt_本期耗用);
            ds.Tables.Add(t_工单);
            ds.Tables.Add(t_基础);


            DataTable t_半成品 = dt_存货核算.Clone();
            t_半成品.TableName = "半成品";
            foreach (DataRow dr in dt_存货核算.Rows)
            {
                if (dr["物料编码"].ToString().Substring(0, 2) != "05" & dr["物料编码"].ToString().Substring(0, 2) != "10")

                    t_半成品.ImportRow(dr);
            }
     //取原材料的最终发出单价 
     ;
            ds.Tables.Add(t_半成品);
            ds.Tables.Add(t_上期在制);

            ///此处还需要根据 物料汇总   物料，平均入库单价  把 返修的再算出来后 汇总 即可 
            foreach (DataRow dr in t_工单.Rows)
            {
                if (dr["物料编码"].ToString().Substring(0, 2) == "10") continue;

                //返修工单 领取得成品 或这 半成品  取上期结存单价  原材料取本期算出得发出单价
                if (dr["生产工单类型"].ToString() == "返修工单")
                {

                    DataRow[] r_本期消耗 = dt_本期耗用.Select(string.Format("生产工单号='{0}'", dr["生产工单号"]));
                    decimal dec_累计金额 = 0;
                    decimal dec_累计数量 = 0;
                    foreach (DataRow e in r_本期消耗)
                    {
                        //  存货核算 已经和上期的结存 合并  默认原材料的 r.lenth>0 恒成立
                        //DataRow[] r = dt_存货核算.Select(string.Format("物料编码='{0}'", e["子项编码"]));
                        decimal b = 0;
                        if (e["子项编码"].ToString().Substring(0, 2) == "01")
                        {
                            DataRow[] r = t_半成品.Select(string.Format("物料编码='{0}'", e["子项编码"]));
                            if (r.Length == 0) //说明该成品或者半成品的 成本尚未算出 先算这个的发出单价
                            {
                                b = fun_dg(ds, e["子项编码"].ToString(), t1); //b为 e["子项编码"]的最终 发出单价 
                                DataRow[] ir = t_半成品.Select(string.Format("物料编码='{0}'", e["子项编码"].ToString()));
                                if (ir.Length == 0)
                                {
                                    DataRow cr = t_半成品.NewRow();
                                    DataRow[] bs = t_基础.Select(string.Format("物料编码='{0}'", e["子项编码"]));
                                    cr["物料名称"] = bs[0]["物料名称"];
                                    cr["规格型号"] = bs[0]["规格型号"];
                                    cr["存货分类"] = bs[0]["存货分类"];
                                    cr["存货分类编码"] = bs[0]["存货分类编码"];
                                    cr["物料编码"] = e["子项编码"];
                                    DataRow[] rg = dt_存货核算.Select(string.Format("物料编码='{0}'", e["子项编码"]));
                                    if (rg.Length > 0)
                                    {
                                        cr["累计入库金额"] = rg[0]["累计入库金额"];
                                        cr["累计入库数量"] = rg[0]["累计入库数量"];
                                    }

                                    cr["发出单价"] = b;
                                    t_半成品.Rows.Add(cr);
                                }
                            }
                            else
                            {
                                b = Convert.ToDecimal(r[0]["发出单价"]);
                            }
                        }
                        else // 05-   10-- 
                        {

                            DataRow[] r = t_半成品.Select(string.Format("物料编码='{0}'", e["子项编码"]));
                            if (r.Length > 0)  //已与其他入库 加权平均算过了 直接取这个就行
                            {
                                b = Convert.ToDecimal(r[0]["发出单价"]);
                            }
                            else
                            {
                                b = fun_dg(ds, e["子项编码"].ToString(), t1); //b为 e["子项编码"]的最终 发出单价 
                                DataRow[] ir = t_半成品.Select(string.Format("物料编码='{0}'", e["子项编码"].ToString()));
                                if (ir.Length == 0)
                                {
                                    DataRow cr = t_半成品.NewRow();
                                    DataRow[] bs = t_基础.Select(string.Format("物料编码='{0}'", e["子项编码"]));
                                    cr["物料名称"] = bs[0]["物料名称"];
                                    cr["规格型号"] = bs[0]["规格型号"];
                                    cr["存货分类"] = bs[0]["存货分类"];
                                    cr["存货分类编码"] = bs[0]["存货分类编码"];
                                    cr["物料编码"] = e["子项编码"];
                                    DataRow[] rg = dt_存货核算.Select(string.Format("物料编码='{0}'", e["子项编码"]));
                                    if (rg.Length > 0)
                                    {
                                        cr["累计入库金额"] = rg[0]["累计入库金额"];
                                        cr["累计入库数量"] = rg[0]["累计入库数量"];
                                    }
                                    //cr["物料名称"] = e["物料名称"];
                                    //cr["规格型号"] = e["规格型号"];
                                    //cr["存货分类"] = e["存货分类"];
                                    //cr["存货分类编码"] = e["存货分类编码"];
                                    cr["发出单价"] = b;
                                    t_半成品.Rows.Add(cr);

                                }

                            }
                        }
                        e["发出单价"] = b;

                        dec_累计金额 += Convert.ToDecimal(e["本期耗用数量"]) * b;
                        //dec_累计数量 += Convert.ToDecimal(e["本期耗用数量"]);

                        //DataRow[] r = dt_存货核算.Select(string.Format("物料编码='{0}'", e["子项编码"]));
                        //if (r.Length > 0)
                        //{
                        //    e["发出单价"] = Convert.ToDecimal(r[0]["发出单价"]);
                        //    dec_累计金额 += Convert.ToDecimal(e["本期耗用数量"]) * Convert.ToDecimal(r[0]["发出单价"]);
                        //}

                        //else // 取上期结存单价
                        //{
                        //    try
                        //    {
                        //        decimal dec_单价 = 0;
                        //        //dec_单价 = fun_单价(t1.AddMonths(-1), e["子项编码"].ToString());
                        //        DataRow[] rp = dt_结转.Select(string.Format("物料编码='{0}'", e["子项编码"]));
                        //        dec_单价 = Convert.ToDecimal(rp[0]["结存单价"]);
                        //        dec_累计金额 += Convert.ToDecimal(e["本期耗用数量"]) * dec_单价;
                        //    }
                        //    catch (Exception ex)
                        //    {

                        //        throw new Exception(e["子项编码"].ToString());
                        //    }

                        //}
                    }
                    dr["材料金额"] = dec_累计金额;

                }
                else// 正常工单和 小批试制
                {

                    DataRow[] r_本期消耗 = dt_本期耗用.Select(string.Format("生产工单号='{0}'", dr["生产工单号"]));
                    decimal dec_累计金额 = 0;
                    decimal dec_累计数量 = 0;
                    foreach (DataRow e in r_本期消耗)
                    {

                        //这里dt_存货核算 已经和上期的结存 合并  默认原材料的 r.lenth>0 恒成立
                        //DataRow[] r = dt_存货核算.Select(string.Format("物料编码='{0}'", e["子项编码"]));
                        DataRow[] r = t_半成品.Select(string.Format("物料编码='{0}'", e["子项编码"]));
                        decimal b = 0;
                        if (r.Length == 0) //说明该成品或者半成品的 成本尚未算出 先算这个的发出单价
                        {
                            b = fun_dg(ds, e["子项编码"].ToString(), t1); //b为 e["子项编码"]的最终 发出单价 
                            DataRow[] ir = t_半成品.Select(string.Format("物料编码='{0}'", e["子项编码"].ToString()));
                            if (ir.Length == 0)
                            {
                                DataRow cr = t_半成品.NewRow();
                                DataRow[] bs = t_基础.Select(string.Format("物料编码='{0}'", e["子项编码"]));
                                cr["物料名称"] = bs[0]["物料名称"];
                                cr["规格型号"] = bs[0]["规格型号"];
                                cr["存货分类"] = bs[0]["存货分类"];
                                cr["存货分类编码"] = bs[0]["存货分类编码"];
                                cr["物料编码"] = e["子项编码"];
                                //cr["物料名称"] = e["物料名称"];
                                //cr["规格型号"] = e["规格型号"];
                                //cr["存货分类"] = e["存货分类"];
                                //cr["存货分类编码"] = e["存货分类编码"];
                                cr["发出单价"] = b;
                                t_半成品.Rows.Add(cr);
                            }
                        }
                        else
                        {
                            b = Convert.ToDecimal(r[0]["发出单价"]);
                        }
                        e["发出单价"] = b;


                        dec_累计金额 += Convert.ToDecimal(e["本期耗用数量"]) * b;

                    }


                    if (Convert.ToDecimal(dr["当期完成数量"]) == 0) dr["入库单价"] = 0;
                    else
                    {
                        dr["入库单价"] = Math.Round(dec_累计金额 / Convert.ToDecimal(dr["当期完成数量"]), 6, MidpointRounding.AwayFromZero);
                    }
                    dr["材料金额"] = Math.Round(dec_累计金额, 2, MidpointRounding.AwayFromZero);
                }
            }

            // ERPorg.Corg.TableToExcel(t_工单, @"C:\Users\GH\Desktop\工单.xlsx");

            //
            MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
            DataTable t_gd = RBQ.SelectGroupByInto("", t_工单, "物料编码,sum(总金额) 总金额,sum(材料金额) 材料金额,sum(当期完成数量) 当期完成数量", "", "物料编码");

            foreach (DataRow r_gd in t_gd.Rows)
            {
                if (r_gd["物料编码"].ToString().Substring(0, 2) == "10") continue;
                decimal dec_期初金额 = 0;
                decimal dec_期初数量 = 0;
                decimal je_sum = 0;
                decimal dec_sum = 0;
                DataRow[] xx = dt_结转.Select(string.Format("物料编码='{0}'", r_gd["物料编码"]));
                if (xx.Length == 0)
                {
                    dec_期初金额 = 0;
                    dec_期初数量 = 0;
                }
                else
                {
                    dec_期初金额 = Convert.ToDecimal(xx[0]["本月结转金额"]);
                    dec_期初数量 = Convert.ToDecimal(xx[0]["本月结转数量"]);
                }
                DataRow[] rrr = dt_存货核算.Select(string.Format("物料编码='{0}'", r_gd["物料编码"]));
                if (rrr.Length > 0) //先取 dt_存货核算中的 金额 和 数量
                {
                    if (rrr[0]["累计入库金额"] == DBNull.Value || rrr[0]["累计入库金额"].ToString() == "")
                        je_sum = 0;
                    else
                    { je_sum = Convert.ToDecimal(rrr[0]["累计入库金额"]); }
                    if (rrr[0]["累计入库数量"] == DBNull.Value || rrr[0]["累计入库数量"].ToString() == "")
                        dec_sum = 0;
                    else dec_sum = Convert.ToDecimal(rrr[0]["累计入库数量"]);
                    //      //+ Convert.ToDecimal(r_gd["材料金额"]) + Convert.ToDecimal(r_gd["当期完成数量"])
                    //      rrr[0]["累计入库金额"] = r_gd["材料金额"];
                    //rrr[0]["累计入库数量"] = r_gd["当期完成数量"] ;
                    //if (dec_期初数量 + dec_sum == 0)
                    //{
                    //    rrr[0]["发出单价"] =0;
                    //}
                    //else
                    //{
                    //    rrr[0]["发出单价"] = (dec_期初金额 + je_sum) / (dec_期初数量 + dec_sum);


                }

                DataRow[] yy = t_半成品.Select(string.Format("物料编码='{0}'", r_gd["物料编码"]));
                if (yy.Length == 0)
                {
                    DataRow rr = t_半成品.NewRow();
                    rr["物料编码"] = r_gd["物料编码"];
                    DataRow[] bs = t_基础.Select(string.Format("物料编码='{0}'", r_gd["物料编码"]));
                    rr["物料名称"] = bs[0]["物料名称"];
                    rr["规格型号"] = bs[0]["规格型号"];
                    rr["存货分类"] = bs[0]["存货分类"];
                    rr["存货分类编码"] = bs[0]["存货分类编码"];
                    rr["累计入库金额"] = Math.Round(Convert.ToDecimal(r_gd["材料金额"]) + je_sum, 2, MidpointRounding.AwayFromZero);
                    rr["累计入库数量"] = Convert.ToDecimal(r_gd["当期完成数量"]) + dec_sum;
                    decimal dec = Convert.ToDecimal(rr["累计入库数量"]);
                    if (dec == 0)
                    {
                        rr["收入单价"] = 0;
                    }
                    else
                    {
                        rr["收入单价"] = Convert.ToDecimal(rr["累计入库金额"]) / dec;
                    }
                    if (dec_期初数量 + dec == 0)
                    { rr["发出单价"] = 0; }
                    else rr["发出单价"] = (dec_期初金额 + Convert.ToDecimal(rr["累计入库金额"])) / (dec_期初数量 + dec);
                    t_半成品.Rows.Add(rr);
                }
                else
                {
                    yy[0]["累计入库金额"] = Math.Round(Convert.ToDecimal(r_gd["材料金额"]) + je_sum, 2, MidpointRounding.AwayFromZero);
                    yy[0]["累计入库数量"] = Convert.ToDecimal(r_gd["当期完成数量"]) + dec_sum;
                    decimal dec = Convert.ToDecimal(yy[0]["累计入库数量"]);
                    if (dec == 0)
                    {
                        yy[0]["收入单价"] = 0;
                    }
                    else
                    {
                        yy[0]["收入单价"] = Convert.ToDecimal(yy[0]["累计入库金额"]) / dec;
                    }
                    if (dec_期初数量 + dec == 0)
                    { yy[0]["发出单价"] = 0; }
                    else yy[0]["发出单价"] = (dec_期初金额 + Convert.ToDecimal(yy[0]["累计入库金额"])) / (dec_期初数量 + dec);

                }

            }
            foreach (DataRow tr in dt_存货核算.Rows)
            {
                if (tr["物料编码"].ToString() == "05")
                {
                    DataRow[] r_jy = t_半成品.Select(string.Format("物料编码='{0}'", tr["物料编码"]));
                    if (r_jy.Length > 0) continue;
                    else
                    {
                        DataRow cr = t_半成品.NewRow();
                        DataRow[] bs = t_基础.Select(string.Format("物料编码='{0}'", tr["物料编码"]));
                        cr["物料名称"] = bs[0]["物料名称"];
                        cr["规格型号"] = bs[0]["规格型号"];
                        cr["存货分类"] = bs[0]["存货分类"];
                        cr["存货分类编码"] = bs[0]["存货分类编码"];
                        cr["物料编码"] = tr["物料编码"];
                        cr["发出单价"] = tr["结存单价"];
                        cr["收入单价"] = 0;
                        //cr["结存单价"] = r_wcr["结存单价"];
                        t_半成品.Rows.Add(cr);
                    }
                }
            }
            //结转都没有。
            foreach (DataRow r_wcr in dt_结转.Rows)
            {
                if (r_wcr["物料编码"].ToString() == "05")
                {
                    DataRow[] r_jy = t_半成品.Select(string.Format("物料编码='{0}'", r_wcr["物料编码"]));
                    if (r_jy.Length > 0) continue;
                    else
                    {
                        DataRow[] pr = dt_存货核算.Select(string.Format("物料编码='{0}'", r_wcr["物料编码"]));
                        if (pr.Length > 0)
                        {
                            DataRow cr = t_半成品.NewRow();
                            DataRow[] bs = t_基础.Select(string.Format("物料编码='{0}'", r_wcr["物料编码"]));
                            cr["物料名称"] = bs[0]["物料名称"];
                            cr["规格型号"] = bs[0]["规格型号"];
                            cr["存货分类"] = bs[0]["存货分类"];
                            cr["存货分类编码"] = bs[0]["存货分类编码"];
                            cr["物料编码"] = r_wcr["物料编码"];
                            cr["发出单价"] = pr[0]["发出单价"];
                            cr["收入单价"] = pr[0]["收入单价"];
                            cr["累计入库金额"] = pr[0]["累计入库金额"];
                            cr["累计入库数量"] = pr[0]["累计入库数量"];

                            //cr["结存单价"] = r_wcr["结存单价"];
                            t_半成品.Rows.Add(cr);
                        }
                        else
                        {
                            DataRow cr = t_半成品.NewRow();
                            DataRow[] bs = t_基础.Select(string.Format("物料编码='{0}'", r_wcr["物料编码"]));
                            cr["物料名称"] = bs[0]["物料名称"];
                            cr["规格型号"] = bs[0]["规格型号"];
                            cr["存货分类"] = bs[0]["存货分类"];
                            cr["存货分类编码"] = bs[0]["存货分类编码"];
                            cr["物料编码"] = r_wcr["物料编码"];
                            cr["发出单价"] = pr[0]["发出单价"];
                            cr["收入单价"] = pr[0]["收入单价"];
                            cr["累计入库金额"] = pr[0]["累计入库金额"];
                            cr["累计入库数量"] = pr[0]["累计入库数量"];
                            //cr["结存单价"] = r_wcr["结存单价"];
                            t_半成品.Rows.Add(cr);
                        }
                    }
                }
            }

            return t_半成品;
        }

        /// <summary>
        /// t_成本中找不到记录才需要调用 此递归函数
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="str_产品编码"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        private decimal fun_dg(DataSet ds, string str_产品编码, DateTime t1)
        {
            //if (str_产品编码 == "05010201020023")
            //{

            //}
            DataTable dt_存货核算 = ds.Tables[0];
            DataTable dt_结转 = ds.Tables[1];
            DataTable t_本期耗用 = ds.Tables[2];
            DataTable t_工单 = ds.Tables[3];
            DataTable t_基础 = ds.Tables[4];
            // ds.Tables[5]; 为 t_成本
            DataTable t_上期在制 = ds.Tables[6];

            decimal dec = 0;

            DataRow[] dr = t_工单.Select(string.Format("当期完成数量>0  and  物料编码='{0}' and 生产工单类型<>'返修工单'", str_产品编码));
            //取该编码的所有工单 算出入库总额 然后 加上上期总额 和 其他入库里面的 累计入库金额 算 发出单价 
            if (dr.Length > 0)
            {
                decimal dec_生产总价 = 0; //此物料所有工单的成本 （不含返工）
                decimal dec_生产总数 = 0;//此物料所有工单当期完成数量

                //这里算出此产品 每个工单的 收入单价 
                foreach (DataRow rr in dr)
                {
                    DataRow[] r_本期消耗 = t_本期耗用.Select(string.Format("生产工单号='{0}'", rr["生产工单号"]));
                    decimal dec_累计金额 = 0; //料的总金额

                    foreach (DataRow e in r_本期消耗)
                    {
                        //这里dt_存货核算 已经和上期的结存 合并  默认原材料的 r.lenth>0 恒成立
                        DataRow[] r = ds.Tables[5].Select(string.Format("物料编码='{0}'", e["子项编码"]));
                        decimal b = 0;
                        if (e["子项编码"].ToString().Substring(0, 2) != "01") //如果不是原材料 
                        {
                            if (r.Length == 0) // 并且 t_成本中不存在此记录 先计算此半成品的 结存单价
                            {
                                b = fun_dg(ds, e["子项编码"].ToString(), t1);
                                DataRow[] yx = ds.Tables[5].Select(string.Format("物料编码='{0}'", e["子项编码"]));
                                if (yx.Length == 0)
                                {
                                    DataRow cr = ds.Tables[5].NewRow();
                                    cr["物料编码"] = e["子项编码"];
                                    DataRow[] bs = t_基础.Select(string.Format("物料编码='{0}'", e["子项编码"]));
                                    cr["物料名称"] = bs[0]["物料名称"];
                                    cr["规格型号"] = bs[0]["规格型号"];
                                    cr["存货分类"] = bs[0]["存货分类"];
                                    cr["存货分类编码"] = bs[0]["存货分类编码"];
                                    DataRow[] rg = dt_存货核算.Select(string.Format("物料编码='{0}'", e["子项编码"]));
                                    if (rg.Length > 0)
                                    {
                                        cr["累计入库金额"] = rg[0]["累计入库金额"];
                                        cr["累计入库数量"] = rg[0]["累计入库数量"];
                                    }

                                    cr["发出单价"] = b;
                                    ds.Tables[5].Rows.Add(cr);
                                }

                            }
                            else
                            {
                                b = Convert.ToDecimal(r[0]["发出单价"]);
                            }
                        }
                        else //如果是 原材料 
                        {
                            if (r.Length == 0)  //是原材料还没有发出单价 就是有问题的  
                            {
                                string ss = e["子项编码"].ToString();

                                //DataRow[]tr = dt_存货核算.Select("物料编码='01020200000290'");
                                //ss = e["子项编码"].ToString();
                                //DataRow[] tr2 = dt_存货核算.Select(string.Format("物料编码='{0}'", "01020200000290"));
                                //ss = e["子项编码"].ToString();
                                // 19-12-3 有个料是当月委外采购得并用于生产 
                                b = 0;
                                // throw new Exception(e["子项编码"].ToString() + "无发出单价");
                            }
                            else
                            {
                                b = Convert.ToDecimal(r[0]["发出单价"]); //原材料
                            }
                        }
                        decimal dec_耗用 = b; //本月耗用单价
                        decimal dec_本期金额 = 0;
                        DataRow[] rt_上期 = t_上期在制.Select(string.Format("生产工单号='{0}' and 子项编码='{1}'", e["生产工单号"], e["子项编码"]));
                        if (rt_上期.Length > 0)
                        {
                            dec_本期金额 = Math.Round(Convert.ToDecimal(e["子项当期领用"]) * b, 2, MidpointRounding.AwayFromZero);
                            if (Convert.ToDecimal(e["子项当期领用"]) + Convert.ToDecimal(rt_上期[0]["在制品"]) != 0)
                            {
                                dec_耗用 = (Convert.ToDecimal(rt_上期[0]["在制金额"]) + dec_本期金额) / (Convert.ToDecimal(e["子项当期领用"]) + Convert.ToDecimal(rt_上期[0]["在制品"]));
                            }
                            else
                            {
                                dec_耗用 = Math.Abs((Convert.ToDecimal(rt_上期[0]["在制金额"]) / Convert.ToDecimal(rt_上期[0]["在制品"])));
                                dec_耗用 = Math.Round(dec_耗用, 6, MidpointRounding.AwayFromZero);
                                e["发出单价"] = dec_耗用;
                            }
                        }
                        e["耗用单价"] = dec_耗用 = Math.Round(dec_耗用, 6, MidpointRounding.AwayFromZero);
                        dec_累计金额 += Math.Round(Convert.ToDecimal(e["本期耗用数量"]) * dec_耗用, 2, MidpointRounding.AwayFromZero);

                        //  dec_累计金额 += Convert.ToDecimal(e["本期耗用数量"]);

                    }
                    //料总额加所有的分摊的
                    dec_累计金额 += Math.Round(Convert.ToDecimal(rr["辅材分摊"]) + Convert.ToDecimal(rr["制造费用"]) + Convert.ToDecimal(rr["软件费用"]) + Convert.ToDecimal(rr["人工费用"]), 2, MidpointRounding.AwayFromZero);
                    if (Convert.ToDecimal(rr["当期完成数量"]) == 0)
                    {
                        rr["入库单价"] = 0;
                    }
                    else
                    {
                        rr["入库单价"] = Math.Round(dec_累计金额 / Convert.ToDecimal(rr["当期完成数量"]), 6, MidpointRounding.AwayFromZero); //生产数量-期末数
                    }

                    dec_生产总价 += Math.Round(dec_累计金额, 2, MidpointRounding.AwayFromZero);

                    dec_生产总数 += Convert.ToDecimal(rr["当期完成数量"]);
                }
                dec_生产总价 = Math.Round(dec_生产总价, 2, MidpointRounding.AwayFromZero);
                decimal dec_期初金额 = 0;
                decimal dec_期初数量 = 0;

                DataRow[] xx = dt_结转.Select(string.Format("物料编码='{0}'", str_产品编码));
                if (xx.Length > 0)
                {
                    dec_期初金额 = Convert.ToDecimal(xx[0]["本月结转金额"]);
                    dec_期初数量 = Convert.ToDecimal(xx[0]["本月结转数量"]);
                }
                if (str_产品编码 == "05010104020001")
                {

                }
                DataRow[] xo = dt_存货核算.Select(string.Format("物料编码='{0}'", str_产品编码));

                if (xo.Length > 0)
                {
                    DataRow rr = ds.Tables[5].NewRow();
                    rr["物料编码"] = str_产品编码;
                    DataRow[] bs = t_基础.Select(string.Format("物料编码='{0}'", str_产品编码));
                    rr["物料名称"] = bs[0]["物料名称"];
                    rr["规格型号"] = bs[0]["规格型号"];
                    rr["存货分类"] = bs[0]["存货分类"];
                    rr["存货分类编码"] = bs[0]["存货分类编码"];
                    rr["累计入库金额"] = Math.Round(dec_生产总价 + Convert.ToDecimal(xo[0]["累计入库金额"]), 2, MidpointRounding.AwayFromZero);
                    rr["累计入库数量"] = dec_生产总数 + Convert.ToDecimal(xo[0]["累计入库数量"]);
                    //rr["收入单价"] = dec_期初金额 / dec_期初数量;
                    rr["发出单价"] = dec = Math.Round((dec_期初金额 + dec_生产总价 + Convert.ToDecimal(xo[0]["累计入库金额"])) / (dec_期初数量 + dec_生产总数 + Convert.ToDecimal(xo[0]["累计入库数量"])), 6, MidpointRounding.AwayFromZero);
                    ds.Tables[5].Rows.Add(rr);
                }
                else
                {

                    DataRow rr = ds.Tables[5].NewRow();
                    rr["物料编码"] = str_产品编码;
                    DataRow[] bs = t_基础.Select(string.Format("物料编码='{0}'", str_产品编码));
                    rr["物料名称"] = bs[0]["物料名称"];
                    rr["规格型号"] = bs[0]["规格型号"];
                    rr["存货分类"] = bs[0]["存货分类"];
                    rr["存货分类编码"] = bs[0]["存货分类编码"];
                    rr["累计入库金额"] = Math.Round(dec_生产总价, 2, MidpointRounding.AwayFromZero);
                    rr["累计入库数量"] = dec_生产总数;
                    //rr["收入单价"] = dec_期初金额 / dec_期初数量;
                    rr["发出单价"] = dec = Math.Round((dec_期初金额 + dec_生产总价) / (dec_期初数量 + dec_生产总数), 6, MidpointRounding.AwayFromZero);
                    ds.Tables[5].Rows.Add(rr);
                }

            }
            else
            {
                DataRow[] r_chhes = dt_存货核算.Select(string.Format("物料编码='{0}'", str_产品编码));
                if (r_chhes.Length > 0)
                {
                    dec = Convert.ToDecimal(r_chhes[0]["发出单价"]);
                }
                else
                {
                    dec = fun_单价x(t1.AddMonths(-1), str_产品编码);
                }
            }
            return dec;
        }
    }
}
