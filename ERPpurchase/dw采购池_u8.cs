using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace ERPpurchase
{
    public partial class dw采购池_u8 : UserControl
    {
        #region 变量
        string strcon = CPublic.Var.strConn;

        /// <summary>
        /// 延续之前计划池的 dtm就不改了
        /// </summary>
        DataTable dtM;
        DataTable dtM_PurchasePool;
        DataTable dt_SaleOrder = new DataTable();
        DataTable dt_SaleCrderCopy;
        DataTable dt_totalcount;
        DataTable IncompleteWorkOrder;
        DataTable IncompleteWorkOrdercopy;
        DataTable IncompletePO;
        DataTable IncompletePOCopy;
        /// <summary>
        /// 所有母件（父项）
        /// </summary>
        DataTable dt_parent;
        DataTable dt_库存;
        DataTable dt_bom = new DataTable();
        DataTable saleDisplay;
        /// <summary>
        /// flag 指示用户进度 ,导入销售明细--1,导入未完成工单--2,同步BOM及库存--3  计算完成--4
        /// </summary>
        int flag = 0;
        bool bl_sync = false;
        bool bl_calculate = false;
        string str_log = "";
        #endregion
        public dw采购池_u8()
        {
            InitializeComponent();
        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (flag == 0)
                {
                    string s = "";
                    if (dateEdit1.EditValue != null && dateEdit1.EditValue.ToString() != "") s = s + string.Format(" and 预计发货日期>='{0}'", dateEdit1.EditValue);
                    if (dateEdit2.EditValue != null && dateEdit2.EditValue.ToString() != "") s = s + string.Format(" and 预计发货日期<'{0}'", Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1));
                    s = string.Format(@"select  z.*,工时 from (
      select  *,销售数量-累计完成数量 as 数量 from (
      select SO_SODetails.cSOCode 订单号,iRowNo 行号,Customer.cCusCode 客户编码,Customer.cCusName 客户名称,SO_SODetails.cInvCode 物料编码,SO_SODetails.cInvName 产品名称,inventory.cInvStd 型号
      ,iQuantity 销售数量,dPreDate 预计发货日期,ISNULL(iFHQuantity,0) 累计完成数量,cFree1 颜色 ,cFree2 月牙膜,dDate 下单日期,SO_SODetails.cMemo 备注  from   [192.168.20.150].UFDATA_008_2018.dbo.SO_SODetails 
      left join [192.168.20.150].UFDATA_008_2018.dbo.inventory  on inventory.cInvCode=SO_SODetails.cInvCode 
      left join [192.168.20.150].UFDATA_008_2018.dbo.SO_SOMain on SO_SOMain.cSOCode=SO_SODetails.cSOCode
      left join  [192.168.20.150].UFDATA_008_2018.dbo.Customer on Customer.cCusCode=SO_SOMain.cCusCode
      where SO_SOMain.iStatus=1  and  cSCloser is null and ( iFHQuantity<iQuantity or iFHQuantity is null)) result
      union
      select  *,销售数量-累计完成数量  as 数量 from (
      select  cCode 订单号 ,iRowNo 行号,zb.cCusCode 客户编码,Customer.cCusName 客户名称,mx.cInvCode 物料编码,mx.cInvName 产品名称,inventory.cInvStd 型号 
      ,iQuantity 销售数量,dExpectationDate 预计发货日期,isnull(fdhquantity,0) 累计完成数量,cFree1 颜色 ,cFree2 月牙膜,dDate 下单日期,mx.cMemo 备注  from [192.168.20.150].UFDATA_008_2018.dbo.SA_PreOrderDetails mx
      left join [192.168.20.150].UFDATA_008_2018.dbo.SA_PreOrderMain zb on mx.autoid=zb.ID
      left join [192.168.20.150].UFDATA_008_2018.dbo.inventory  on inventory.cInvCode=mx.cInvCode 
      left join  [192.168.20.150].UFDATA_008_2018.dbo.Customer on Customer.cCusCode=zb.cCusCode 
      where iverifystate=2 and  cSCloser is null)y )z 
      left join  基础数据物料信息表 base  on  base.物料编码=z.物料编码  where  left(base.物料编码,2)<>'11' and  left(base.物料编码,3)<>'200'  {0}", s);
                    dt_SaleOrder = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    dt_SaleOrder.Columns.Add("应完工日期", typeof(DateTime));
                    foreach (DataRow saleR in dt_SaleOrder.Rows)
                    {
                        saleR["应完工日期"] = Convert.ToDateTime(saleR["预计发货日期"]).AddDays(-1);
                    }
                    flag = 1;
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        simpleButton1.Text = string.Format("销售明细:{0}条", dt_SaleOrder.Rows.Count);
                        simpleButton1.Enabled = false;
                    }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            Form1 fm = new Form1(dt_SaleOrder, "销售订单及预订单明细");
            fm.Size = new System.Drawing.Size(1500, 900);
            fm.StartPosition = FormStartPosition.CenterParent;
            fm.ShowDialog();
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            try
            {
                if (flag == 1)
                {
                    Thread th = new Thread(() =>
                    {
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            simpleButton4.Enabled = false;
                            simpleButton4.Text = "拉取数据中..";
                        }));
                        fun_sync();

                    });
                    th.Start();
                }
                else
                {
                    MessageBox.Show("请按步骤操作");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void fun_sync()
        {
            // string s = "";
            string s = "exec sync_u8_data "; //基础信息
            CZMaster.MasterSQL.ExecuteSQL(s, strcon);
            s = "exec sync_u8_stock "; //库存
            CZMaster.MasterSQL.ExecuteSQL(s, strcon);
            s = "exec sync_u8_bom "; //bom
            CZMaster.MasterSQL.ExecuteSQL(s, strcon);

            s = "exec  sync_UnclaimedCount;exec sync_u8_InTransit"; //未领量在途
            CZMaster.MasterSQL.ExecuteSQL(s, strcon);

            s = @"  select  产品编码,产品名称,fx.存货分类 as 父项分类,fx.规格型号 as 父项规格,子项编码,子项名称,数量,zx.自制 as 子项自制,zx.可购 as 子项可购,zx.存货分类 as 子项分类
    ,zx.规格型号 as 子项规格       from 基础数据物料BOM表 bom 
   left join 基础数据物料信息表 zx on bom.子项编码=zx.物料编码 
   left join 基础数据物料信息表 fx on bom.产品编码=fx.物料编码 ";
            dt_bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = @" select kc.物料编码,base.物料名称,base.规格型号,存货分类,库存总数,未领量,自制,在途量,isNull(委外在途,0)委外在途,可购,0 需求数量  from  
  (select  物料编码,sum(库存总数)库存总数,max(未领量)未领量,max(在途量) as 在途量  from 仓库物料数量表
    where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 ='仓库类别' and 布尔字段2=1) group by 物料编码)  kc 
    left join 基础数据物料信息表 base   on  base.物料编码=kc.物料编码
    left  join (  select  cinvcode, sum(iQuantity-ISNULL(freceivedqty,0))委外在途  from (
 select  OM_MOMain.cCode ,MODetailsID,cInvCode,iQuantity,freceivedqty,dbCloseDate,cState from [192.168.20.150].UFDATA_008_2018.dbo.OM_MODetails
 inner join [192.168.20.150].UFDATA_008_2018.dbo.OM_MOMain on OM_MOMain.MOID=OM_MODetails.MOID
 where OM_MOMain.cState=1)ww group by cInvCode) ww on ww.cinvcode=base.物料编码";
            dt_库存 = CZMaster.MasterSQL.Get_DataTable(s, strcon);


            //未完成工单
              s = @"select  mocode 生产订单号,invcode 物料编码,qty-QualifiedInQty 未完成数量,base.规格型号,存货分类,未领量,在途量  from [192.168.20.150].UFDATA_008_2018.dbo.mom_orderdetail 
 left join [192.168.20.150].UFDATA_008_2018.dbo.mom_order  on mom_order.Moid=mom_orderdetail.moid
 left join 基础数据物料信息表  base on base.物料编码=invcode 
 left join  (select  物料编码,sum(库存总数)库存总数,max(未领量)未领量,max(在途量) as 在途量  from 仓库物料数量表
      where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 ='仓库类别' and 布尔字段2=1) group by 物料编码)  kc 
  on  base.物料编码=kc.物料编码
  where  mom_orderdetail.Status=3 and qty-QualifiedInQty>0 ";
            IncompleteWorkOrder = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            //未完成采购单   未完成采购单 加 未完成委外单
            s = @" select  b.cpoid,b.cInvCode 物料编码,物料名称,b.采购数量,采购数量-isnull(入库数量,0) as 未入库数量,base.规格型号,存货分类,库存总数,在途量,未领量   from  
   (select cgd.cPOID,cgmx.cInvCode,sum(cgmx.iQuantity) as 采购数量  from [192.168.20.150].UFDATA_008_2018.dbo.PO_Pomain cgd
			left join [192.168.20.150].UFDATA_008_2018.dbo.PO_Podetails cgmx on cgd.POID=cgmx.POID where cgd.cState=1  group by cgd.cPOID,cgmx.cInvCode)b
left join (select  cInvCode,sum(isnull(iQuantity,0))入库数量,cPOID from  [192.168.20.150].UFDATA_008_2018.dbo.rdrecords01 where cPOID is not null  group by cInvCode,cPOID)a
on a.cPOID=b.cPOID and a.cInvCode=b.cInvCode  
left join 基础数据物料信息表 base on  base.物料编码=b.cInvCode
left join  (select  物料编码,sum(库存总数)库存总数,max(未领量)未领量,max(在途量) as 在途量  from 仓库物料数量表
                    where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 ='仓库类别' and 布尔字段2=1) group by 物料编码)  kc 
 on  base.物料编码=kc.物料编码 where 采购数量-isnull(入库数量,0)>0
 union 
  select  OM_MOMain.cCode as cpoid,cInvCode 物料编码,物料名称,iQuantity 采购数量,iQuantity-isnull(freceivedqty,0) as 未入库数量,base.规格型号,存货分类,库存总数,在途量,未领量
   from [192.168.20.150].UFDATA_008_2018.dbo.OM_MODetails
 inner join [192.168.20.150].UFDATA_008_2018.dbo.OM_MOMain on OM_MOMain.MOID=OM_MODetails.MOID
left join 基础数据物料信息表 base on  base.物料编码= cInvCode
left join  (select  物料编码,sum(库存总数)库存总数,max(未领量)未领量,max(在途量) as 在途量  from 仓库物料数量表
                             where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 ='仓库类别' and 布尔字段2=1) group by 物料编码)  kc 
 on  base.物料编码=kc.物料编码
 where OM_MOMain.cState=1 and iQuantity>isnull(freceivedqty,0) ";
            IncompletePO = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            flag = 3;
            bl_sync = false;

            BeginInvoke(new MethodInvoker(() =>
            {
                simpleButton4.Text = "已同步,并加载完成";
                simpleButton4.Enabled = false;

            }));
        }
        private void simpleButton5_Click(object sender, EventArgs e)
        {
            try
            {
                saleDisplay = new DataTable();
                gridControl1.DataSource = null;
                gridControl2.DataSource = null;

                if (flag < 3) throw new Exception("信息尚未准备完全,请按步骤操作");
                if (bl_calculate) throw new Exception("正在计算中..");
                Thread th = new Thread(calculate);
                th.IsBackground = true;
                th.Start();
                bl_calculate = true;
                simpleButton5.Text = "正在计算中..";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void calcu_check()
        {

            if (!dt_SaleOrder.Columns.Contains("物料编码")) throw new Exception("销售订单中不含'物料编码'列");

            if (!IncompleteWorkOrder.Columns.Contains("未完成数量")) throw new Exception("生产订单中不含'未完成数量'列");

            decimal dec = 0;
            int j = 2;
            foreach (DataRow dr in dt_SaleOrder.Rows)
            {
                string s = dr["物料编码"].ToString().Trim();
                if (s.Contains("S")) //contain 不区分大小写,先判断有没有 'smt'  有就去掉
                {
                    int d = s.IndexOf("S");
                    if (d == -1) d = s.IndexOf("s");
                    s = s.Substring(0, d + 1);

                }
                if (s.Length < 14) throw new Exception(string.Format("销售订单中第{0}行物料编码位数不对", j));
                if (!decimal.TryParse(dr["数量"].ToString(), out dec)) throw new Exception(string.Format("销售订单中第{0}行数量格式不正确", j));
                j++;

            }
        }
        private void calculate()
        {
            try
            {
              
                calcu_check();
                dtM = new DataTable();
                dtM.Columns.Add("未领量", typeof(decimal));
                dtM.Columns.Add("在途量", typeof(decimal));


                dtM.Columns.Add("未完成工单数", typeof(decimal));

                dtM.Columns.Add("物料编码");
                dtM.Columns.Add("物料名称");
                dtM.Columns.Add("规格型号");
                dtM.Columns.Add("库存总数", typeof(decimal));
                dtM.Columns.Add("存货分类");
                dtM.Columns.Add("参考数量", typeof(decimal));
                dtM.Columns.Add("销售数量", typeof(decimal));
                dtM.Columns.Add("自制", typeof(bool));

                dt_SaleCrderCopy = dt_SaleOrder.Copy();
                IncompleteWorkOrdercopy = IncompleteWorkOrder.Copy();
                IncompletePOCopy = IncompletePO.Copy();


                dt_SaleCrderCopy.Columns.Add("库存总数", typeof(decimal));
                dt_SaleCrderCopy.Columns.Add("未完成工单数", typeof(decimal));
                dt_SaleCrderCopy.Columns.Add("未领量", typeof(decimal));
                dt_SaleCrderCopy.Columns.Add("在途量", typeof(decimal));




                //dt_SaleCrderCopy 加入库存数量和 未完成工单数  用于核对,gridcontrol1显示

                foreach (DataRow dr in dt_SaleCrderCopy.Rows)
                {
                    if (dr["物料编码"].ToString().Trim() != "")
                    {

                        DataRow[] v = dt_库存.Select(string.Format("物料编码='{0}'", dr["物料编码"]));

                        dr["库存总数"] = v[0]["库存总数"];
                        dr["未领量"] = v[0]["未领量"];
                        dr["在途量"] = v[0]["在途量"];


                        DataRow[] v1 = IncompleteWorkOrder.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                        if (v1.Length > 0) dr["未完成工单数"] = v1[0]["未完成数量"];
                    }
                }
                //销售订单汇总
                MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
                DataTable dt_SaleOrder_1 = RBQ.SelectGroupByInto("", dt_SaleOrder, "物料编码,sum(数量) 数量", "", "物料编码");

       

                //未完成工单汇总
                DataTable IncompleteWorkOrder_1 = RBQ.SelectGroupByInto("", IncompleteWorkOrder, "物料编码,sum(未完成数量) 数量", "", "物料编码");
                //未完成采购单汇总
                DataTable IncompletePO_1 = RBQ.SelectGroupByInto("", IncompletePO, "物料编码,sum(未入库数量) 数量", "", "物料编码");

                //dt_totalcount 复制 dt_库存,库存总数+未完成订单数
                dt_totalcount = dt_库存.Copy();
                dt_totalcount.Columns.Add("总数", typeof(decimal));
                dt_totalcount.Columns.Add("未完成工单数", typeof(decimal));
                dt_totalcount.Columns.Add("销售数量", typeof(decimal));
                // dt_totalcount.Columns.Add("需求数量", typeof(decimal));
                foreach (DataRow xr in dt_totalcount.Rows)
                {
                    decimal dec = 0;
                    DataRow[] r = IncompleteWorkOrder_1.Select(string.Format("物料编码='{0}'", xr["物料编码"]));
                    if (r.Length > 0)
                    {
                        dec = Convert.ToDecimal(r[0]["数量"]);
                    }
                    xr["未完成工单数"] = dec;
                    xr["总数"] = Convert.ToDecimal(xr["库存总数"]) - Convert.ToDecimal(xr["未领量"]) + dec + Convert.ToDecimal(xr["在途量"]);

                    DataRow[] rs = dt_SaleOrder_1.Select(string.Format("物料编码='{0}'", xr["物料编码"]));
                    if (rs.Length > 0)
                    {
                        dec = Convert.ToDecimal(rs[0]["数量"]);
                    }
                    else
                    {
                        dec = 0;
                    }
                    xr["销售数量"] = dec;
                }
                //先计算销售列表中的产品的欠缺数量
                foreach (DataRow dr in dt_SaleOrder_1.Rows)
                {
                    decimal dec_订单数 = Convert.ToDecimal(dr["数量"]);
                    DataRow[] r_total = dt_totalcount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    decimal total = Convert.ToDecimal(r_total[0]["总数"]);
                    decimal kczs = Convert.ToDecimal(r_total[0]["库存总数"]);
                    decimal wwcgds = Convert.ToDecimal(r_total[0]["未完成工单数"]);
                    decimal dec_Unclaimed = Convert.ToDecimal(r_total[0]["未领量"]);
                    decimal dec_InTransit = Convert.ToDecimal(r_total[0]["在途量"]);
                    if (total >= dec_订单数) //库存加未完成>需求数
                    {
                        r_total[0]["总数"] = total - dec_订单数;
                    }
                    else
                    {
                        DataRow r_need = dtM.NewRow();
                        r_need["未完成工单数"] = wwcgds;
                        r_need["物料编码"] = r_total[0]["物料编码"];
                        r_need["在途量"] = dec_InTransit;
                        r_need["未领量"] = dec_Unclaimed;
                        r_need["物料名称"] = r_total[0]["物料名称"];
                        r_need["规格型号"] = r_total[0]["规格型号"];
                        r_need["存货分类"] = r_total[0]["存货分类"];
                        r_need["库存总数"] = kczs;
                        r_need["销售数量"] = r_total[0]["销售数量"];
                        r_need["自制"] = r_total[0]["自制"];
                        r_need["参考数量"] = dec_订单数 - total;
                        dtM.Rows.Add(r_need);
                        r_total[0]["总数"] = 0;
                    }
                }
                //到此处 计算了 所有在销售订单中出现的料 所缺的数量  参考数量为所缺量
                //下面计算 这些 物料的 子项 有自制属性的  所缺的量 
                //11-20 施工提出 没有bom 的提示 但是计算要继续
                DataTable dtMcopy = dtM.Copy();
                //fun_dg(dtMcopy);

                foreach (DataRow dr in dtMcopy.Rows)
                {
                    if (dr["自制"].Equals(true))
                    {
                        if (dt_bom.Select(string.Format("产品编码='{0}'", dr["物料编码"])).Length == 0) str_log = str_log + dr["物料编码"].ToString() + "属性为自制但是没有bom";
                    }
                    DataRow[] br = dt_bom.Select(string.Format("产品编码='{0}'and 子项自制=1", dr["物料编码"].ToString()));
                    if (br.Length > 0) //找到需要自制的半成品 
                    {
                        decimal dec_缺 = Convert.ToDecimal(dr["参考数量"].ToString());
                        foreach (DataRow brr in br)
                        {
                            decimal dec = dec_缺 * Convert.ToDecimal(brr["数量"]); //这是自制半成品所需的量 
                            DataRow[] stock_total = dt_totalcount.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                            decimal total_z = Convert.ToDecimal(stock_total[0]["总数"]);
                            if (total_z >= dec) //库存加未完成>需求数
                            {
                                stock_total[0]["总数"] = total_z - dec;
                            }
                            else
                            {
                                DataRow[] fr = dtM.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                                if (fr.Length > 0)
                                {
                                    fr[0]["参考数量"] = Convert.ToDecimal(fr[0]["参考数量"]) + dec - total_z;
                                }
                                else
                                {
                                    DataRow r_need = dtM.NewRow();
                                    r_need["未完成工单数"] = stock_total[0]["未完成工单数"];
                                    r_need["物料编码"] = stock_total[0]["物料编码"];
                                    r_need["物料名称"] = stock_total[0]["物料名称"];
                                    r_need["规格型号"] = stock_total[0]["规格型号"];
                                    r_need["存货分类"] = stock_total[0]["存货分类"];
                                    r_need["库存总数"] = stock_total[0]["库存总数"];
                                    r_need["销售数量"] = stock_total[0]["销售数量"];
                                    r_need["自制"] = stock_total[0]["自制"];

                                    r_need["参考数量"] = dec - total_z;
                                    dtM.Rows.Add(r_need);
                                    stock_total[0]["总数"] = 0;
                                }

                                //缺的才需要继续往叶子节点递归 不缺不需要
                                fun_dg(stock_total[0]["物料编码"].ToString(), dec - total_z, Convert.ToBoolean(dr["自制"]));

                            }
                        }
                    }
                }
                //不缺的 也要求显示 参考数量为0 

                //foreach (DataRow dr in dt_SaleOrder_1.Rows)
                //{
                //    DataRow[] xxx = dtM.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                //    if (xxx.Length == 0)
                //    {
                //        DataRow[] r_total = dt_totalcount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                //        decimal total = Convert.ToDecimal(r_total[0]["总数"]);
                //        decimal kczs = Convert.ToDecimal(r_total[0]["库存总数"]);
                //        decimal wwcgds = Convert.ToDecimal(r_total[0]["未完成工单数"]);
                //        DataRow r_need = dtM.NewRow();
                //        r_need["未完成工单数"] = wwcgds;
                //        r_need["物料编码"] = r_total[0]["物料编码"];
                //        r_need["物料名称"] = r_total[0]["物料名称"];
                //        r_need["规格型号"] = r_total[0]["规格型号"];
                //        r_need["存货分类"] = r_total[0]["存货分类"];
                //        r_need["库存总数"] = kczs;
                //        r_need["销售数量"] = r_total[0]["销售数量"];
                //        r_need["自制"] = r_total[0]["自制"];
                //        r_need["参考数量"] = 0;
                //        dtM.Rows.Add(r_need);


                //    }
                //}
                //到这里 计算计划缺多少  采购即按照 计划缺的 再次递归到叶子节点 需要买的 扔入 dt

                if (str_log != "")
                {
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        label6.Text = str_log;

                    }));
                }
                dtM_PurchasePool = new DataTable();

                dtM_PurchasePool.Columns.Add("未领量", typeof(decimal));
                dtM_PurchasePool.Columns.Add("在途量", typeof(decimal));
                dtM_PurchasePool.Columns.Add("委外在途", typeof(decimal));

                dtM_PurchasePool.Columns.Add("物料编码");
                dtM_PurchasePool.Columns.Add("物料名称");
                dtM_PurchasePool.Columns.Add("规格型号");
                dtM_PurchasePool.Columns.Add("库存总数", typeof(decimal));
                dtM_PurchasePool.Columns.Add("存货分类");
                dtM_PurchasePool.Columns.Add("参考数量", typeof(decimal));
                dtM_PurchasePool.Columns.Add("销售数量", typeof(decimal));
                dtM_PurchasePool.Columns.Add("可购", typeof(bool));
                dtM_PurchasePool.Columns.Add("自制", typeof(bool));
                dtM_PurchasePool.Columns.Add("需求数量", typeof(decimal));


                foreach (DataRow dr in dtM.Rows) //因为这里dtM就是算出的 计划池  就是算出的计划要生产的 量比如父项A 要生产100 子项B只要生产 50 个 
                {                                //原材料 只要算一层 即是所缺的原材料
                    DataRow[] r_PPool = dt_bom.Select(string.Format("产品编码='{0}'and 子项自制=0 and 子项可购=1", dr["物料编码"]));
                    foreach (DataRow rr in r_PPool)
                    {
                        decimal dec_需 = Convert.ToDecimal(dr["参考数量"]) * Convert.ToDecimal(rr["数量"]); //父项所缺数*bom数量


                        DataRow[] r_total = dt_totalcount.Select(string.Format("物料编码='{0}'", rr["子项编码"]));
                        decimal total = 0;
                        decimal kczs = 0;
                        decimal dec_wl = 0;
                        decimal dec_zt = 0;
                        if (r_total.Length == 0)
                        {
                            total = 0;
                            kczs = 0;
                            dec_wl = 0;
                            dec_zt = 0;
                        }
                        total = Convert.ToDecimal(r_total[0]["总数"]);
                        kczs = Convert.ToDecimal(r_total[0]["库存总数"]);
                        dec_wl = Convert.ToDecimal(r_total[0]["未领量"]);
                        dec_zt = Convert.ToDecimal(r_total[0]["在途量"]);
                        decimal dec_n = 0;

                        r_total[0]["需求数量"] = Convert.ToDecimal(r_total[0]["需求数量"]) + dec_需;
                        if (total - dec_需 > 0) //不缺
                        {
                            r_total[0]["总数"] = total - dec_需;
                        }
                        else //缺了
                        {
                            DataRow[] fr = dtM_PurchasePool.Select(string.Format("物料编码='{0}'", rr["子项编码"]));
                            if (fr.Length > 0)
                            {
                                fr[0]["参考数量"] = Convert.ToDecimal(fr[0]["参考数量"]) + dec_需 - total;
                            }
                            else
                            {
                                DataRow r_need = dtM_PurchasePool.NewRow();
                                r_need["未领量"] = dec_wl;
                                r_need["在途量"] = dec_zt;
                                r_need["委外在途"] = Convert.ToDecimal(r_total[0]["委外在途"]);

                                r_need["物料编码"] = r_total[0]["物料编码"];
                                r_need["物料名称"] = r_total[0]["物料名称"];
                                r_need["规格型号"] = r_total[0]["规格型号"];
                                r_need["存货分类"] = r_total[0]["存货分类"];
                                r_need["库存总数"] = kczs;
                                r_need["销售数量"] = r_total[0]["销售数量"];
                                r_need["自制"] = r_total[0]["自制"];
                                r_need["可购"] = r_total[0]["可购"];
                                r_need["参考数量"] = dec_需 - total;
                                dtM_PurchasePool.Rows.Add(r_need);
                                r_total[0]["总数"] = 0;
                            }

                        }



                    }

                }
                //18-12-3 使用人提出 加入 不缺但是有在途的 方便她催料
                DataView dv_add = new DataView(dt_totalcount);
                dv_add.RowFilter = "在途量>0 or 委外在途>0";
                DataTable dt_1 = dv_add.ToTable();
                foreach (DataRow dr in dt_1.Rows)
                {
                    DataRow[] rrr = dtM_PurchasePool.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (rrr.Length > 0) continue;
                    else
                    {
                        DataRow r_need = dtM_PurchasePool.NewRow();
                        r_need["未领量"] = dr["未领量"];
                        r_need["在途量"] = dr["在途量"]; ;
                        r_need["委外在途"] = dr["委外在途"];
                        r_need["物料编码"] = dr["物料编码"];
                        r_need["物料名称"] = dr["物料名称"];
                        r_need["规格型号"] = dr["规格型号"];
                        r_need["存货分类"] = dr["存货分类"];
                        r_need["库存总数"] = dr["库存总数"]; ;
                        r_need["销售数量"] = dr["销售数量"];
                        r_need["自制"] = dr["自制"];
                        r_need["可购"] = dr["可购"];
                        r_need["参考数量"] = 0;
                        dtM_PurchasePool.Rows.Add(r_need);
                    }
                }
                foreach (DataRow dr in dtM_PurchasePool.Rows)
                {
                    DataRow[] rr = dt_totalcount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    dr["需求数量"] = rr[0]["需求数量"];
                }



                DataTable search_source = dtM.Copy();
                foreach (DataRow dr in dt_SaleOrder.Rows)
                {
                    DataRow[] rr = search_source.Select(string.Format("物料编码='{0}'", dr["物料编码"]));

                    if (rr.Length == 0)
                    {
                        DataRow[] r_total = dt_totalcount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                        DataRow d = search_source.NewRow();
                        d["物料编码"] = r_total[0]["物料编码"];
                        d["物料名称"] = r_total[0]["物料名称"];
                        d["规格型号"] = r_total[0]["规格型号"];
                        d["存货分类"] = r_total[0]["存货分类"];
                        search_source.Rows.Add(d);
                    }
                }

                DataView dvv = new DataView(dt_totalcount);
                dvv.RowFilter = "需求数量>0";
                DataTable dt = dvv.ToTable();

                dtM_PurchasePool.Columns.Add("最早发货日期", typeof(DateTime));
                foreach (DataRow dr in dt.Rows)
                {

                    DataRow[] rr = dt_SaleOrder.Select(string.Format("物料编码='{0}'", dr["物料编码"]), "预计发货日期 asc ");
                    if (rr.Length > 0)
                    {
                        DataRow[] r = dtM_PurchasePool.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                        if (r.Length > 0) r[0]["最早发货日期"] = rr[0]["预计发货日期"];
                    }
                    else
                    {
                        DataTable t = new DataTable();
                        t = ERPorg.Corg.fun_GetFather(t, dr["物料编码"].ToString(), 1, true);
                        foreach (DataRow rrr in t.Rows)
                        {
                            DataRow[] r2 = dt_SaleOrder.Select(string.Format("物料编码='{0}'", rrr["产品编码"]), "预计发货日期 asc ");
                            if (r2.Length > 0)
                            {
                                DataRow[] r = dtM_PurchasePool.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                                // r.length 只会小于等于一条 
                                if (r.Length == 0) continue;
                                else
                                    r[0]["最早发货日期"] = r2[0]["预计发货日期"];
                            }
                        }

                    }
                }
                BeginInvoke(new MethodInvoker(() =>
                {
                    simpleButton5.Text = "计算完成";
                    flag = 4;
                }));
                bl_calculate = false; //计算完成

                  BeginInvoke(new MethodInvoker(() =>
                {
                    DataView dv = new DataView(dtM_PurchasePool);
                    dv.RowFilter = "可购='true'";
                    gc2.DataSource = dv;
                    searchLookUpEdit1.Properties.DataSource = search_source;
                    searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                    searchLookUpEdit1.Properties.ValueMember = "物料编码";
                }));
                 

            }
            catch (Exception ex)
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    label6.Text = "错误原因:" + ex.Message;
                    simpleButton5.Text = "计算错误";
                    bl_calculate = false;
                }));


            }


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemid">物料编码</param>
        /// <param name="dec_需求"></param>
        /// <param name="bl_made">是否自制</param>
        private void fun_dg(string itemid, decimal dec_需求, bool bl_made)
        {
            if (bl_made)
            {
                if (dt_bom.Select(string.Format("产品编码='{0}'", itemid)).Length == 0) str_log = str_log + (itemid + "属性为自制但是没有bom;");
            }
            DataRow[] br = dt_bom.Select(string.Format("产品编码='{0}'and 子项自制=1", itemid));
            if (br.Length > 0) //找到需要自制的半成品 
            {
                decimal dec_缺 = dec_需求;

                foreach (DataRow brr in br)
                {
                    decimal dec = dec_缺 * Convert.ToDecimal(brr["数量"]); //这是自制半成品所需的量 
                    DataRow[] stock_total = dt_totalcount.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                    decimal total_z = Convert.ToDecimal(stock_total[0]["总数"]);
                    if (total_z >= dec) //库存加未完成>需求数
                    {
                        stock_total[0]["总数"] = total_z - dec;
                    }
                    else
                    {
                        DataRow[] fr = dtM.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                        if (fr.Length > 0)
                        {
                            fr[0]["参考数量"] = Convert.ToDecimal(fr[0]["参考数量"]) + dec - total_z;
                        }
                        else
                        {
                            DataRow r_need = dtM.NewRow();
                            r_need["未完成工单数"] = stock_total[0]["未完成工单数"];
                            r_need["物料编码"] = stock_total[0]["物料编码"];
                            r_need["物料名称"] = stock_total[0]["物料名称"];
                            r_need["规格型号"] = stock_total[0]["规格型号"];
                            r_need["存货分类"] = stock_total[0]["存货分类"];
                            r_need["库存总数"] = stock_total[0]["库存总数"];
                            r_need["销售数量"] = stock_total[0]["销售数量"];
                            r_need["自制"] = stock_total[0]["自制"];

                            r_need["参考数量"] = dec - total_z;
                            dtM.Rows.Add(r_need);
                            stock_total[0]["总数"] = 0;
                        }
                        fun_dg(stock_total[0]["物料编码"].ToString(), dec - total_z, Convert.ToBoolean(stock_total[0]["自制"]));
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
                {
                    string s = "可购='true'";
                    DataTable ListM = new DataTable();
                    ListM = ERPorg.Corg.billofM(ListM, searchLookUpEdit1.EditValue.ToString(), true,dt_bom);
                    if (ListM.Rows.Count > 0)
                    {
                        DataView dv = new DataView(dtM_PurchasePool);
                        s = s + " and 物料编码 in (";
                        foreach (DataRow dr in ListM.Rows)
                        {
                            s = s + string.Format("'{0}',", dr["子项编码"]);
                        }
                        s = s.Substring(0, s.Length - 1) + ")";
                        dv.RowFilter = s;
                        gc2.DataSource = dv;
                    }
                    else
                    {
                        MessageBox.Show("无数据");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataView dv = new DataView(dtM_PurchasePool);
            dv.RowFilter = "可购='true'";
            gc2.DataSource = dv;
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
           // textBox1.Text = "";
            if (!bl_sync)
            {
                flag = 0;
                bl_calculate = false;
                label6.Text = "- - - ";
                simpleButton1.Enabled = true;
                simpleButton1.Text = "导入销售订单";
                simpleButton4.Enabled = true;
                simpleButton4.Text = "获取并同步";
                simpleButton5.Text = "开始计算";
            }
        }

        private void gv2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
            if (dr == null) return;
            Thread th = new Thread(() =>
            {
                DataTable dtz = new DataTable();

                dtz.Columns.Add("产品编码");
                dtz = ERPorg.Corg.fun_GetFather(dtz, dr["物料编码"].ToString(), 0, true);

                string s = "";
                if (dtz.Rows.Count > 0)
                {
                    s = "物料编码 in (";
                    foreach (DataRow xx in dtz.Rows)
                    {
                        s = s + "'" + xx["产品编码"].ToString() + "',";
                    }
                    s = s.Substring(0, s.Length - 1) + ")";
                    DataView dv = new DataView(dt_SaleCrderCopy);
                    dv.RowFilter = s;
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        gridControl1.DataSource = dv;
                    }));

                }
                else
                {
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        gridControl1.DataSource = dt_SaleCrderCopy.Clone();
                    }));

                }

                s = string.Format("物料编码='{0}'", dr["物料编码"].ToString());

                DataView dv_z = new DataView(IncompletePO);
                dv_z.RowFilter = s;

                BeginInvoke(new MethodInvoker(() =>
                {
                    gridControl2.DataSource = dv_z;
                }));

            });
            th.Start();
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc2, new Point(e.X, e.Y));
                gv2.CloseEditor();
                contextMenuStrip1.Tag = gv2;

            }
        }

        private void gv2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }
    }
}
