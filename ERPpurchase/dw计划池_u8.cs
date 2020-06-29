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
    public partial class dw计划池_u8 : UserControl
    {
        #region 变量
        string strcon = CPublic.Var.strConn;
        string strcon_U8 = CPublic.Var.geConn("DW");
        DataTable dtM;
        DataTable dt_SaleOrder = new DataTable();
        DataTable dt_SaleCrderCopy;
        DataTable dt_totalcount;
        DataTable IncompleteWorkOrder = new DataTable();
        DataTable IncompleteWorkOrdercopy;
        DataTable dt_库存;
        DataTable dt_bom = new DataTable();
        DataTable saleDisplay;
        /// <summary>
        /// flag 指示用户进度 ,导入销售明细-1,导入未完成工单-2,同步BOM及库存-3 
        /// </summary>
        int flag = 0;
        bool bl_sync = false;
        bool bl_calculate = false;
        string str_log = "";

        #endregion

        public dw计划池_u8()
        {
            InitializeComponent();
        }

        private void dw计划池_u8_Load(object sender, EventArgs e)
        {
            DateTime t = CPublic.Var.getDatetime();
            DateTime t1 = new DateTime(t.Year, t.Month, 1);
            t = t1.AddMonths(3).AddDays(-1);
            dateEdit1.EditValue = t1;
            dateEdit2.EditValue = t;
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
                        sync();

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

        //同步u8bom、库存、在制、未领、在途 获取 未完成工单
        private void sync()
        {
            //未完成工单
            string s = @" select  mocode 生产订单号,invcode 物料编码,qty-QualifiedInQty 未完成数量,base.规格型号,存货分类,未领量,在途量,
 case when mocode in ( select  MoCode  from [192.168.20.150].UFDATA_008_2018.dbo.mom_moallocate 
 left join [192.168.20.150].UFDATA_008_2018.dbo.mom_orderdetail on mom_orderdetail.MoDId=mom_moallocate.MoDId
 left join [192.168.20.150].UFDATA_008_2018.dbo.mom_order  on  mom_order.MoId=mom_orderdetail.MoId
  where  mom_orderdetail.Status =3  and mom_moallocate.Qty-IssQty>0 group by MoCode) then 1 else 0 end as 标识
   from [192.168.20.150].UFDATA_008_2018.dbo.mom_orderdetail 
 left join [192.168.20.150].UFDATA_008_2018.dbo.mom_order  on mom_order.Moid=mom_orderdetail.moid
 left join 基础数据物料信息表  base on base.物料编码=invcode 
 left join  (select  物料编码,sum(库存总数)库存总数,max(未领量)未领量,max(在途量) as 在途量  from 仓库物料数量表
      where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 ='仓库类别' and 布尔字段2=1) group by 物料编码)  kc 
  on  base.物料编码=kc.物料编码
  where  mom_orderdetail.Status=3 and qty-QualifiedInQty>0 ";
            IncompleteWorkOrder = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            flag = 2;
            s = "exec sync_u8_data  "; //基础数据
            CZMaster.MasterSQL.ExecuteSQL(s, strcon);
            s = "exec sync_u8_stock"; //库存
            CZMaster.MasterSQL.ExecuteSQL(s, strcon);
            s = "exec sync_u8_bom "; //bom
            CZMaster.MasterSQL.ExecuteSQL(s, strcon);
            s = "exec  sync_UnclaimedCount"; //未领量
            CZMaster.MasterSQL.ExecuteSQL(s, strcon);
            s = @"  select  产品编码,产品名称,fx.存货分类 as 父项分类,fx.规格型号 as 父项规格,子项编码,子项名称,数量,zx.自制 as 子项自制 ,zx.存货分类 as 子项分类
    ,zx.规格型号 as 子项规格       from 基础数据物料BOM表 bom 
   left join 基础数据物料信息表 zx on bom.子项编码=zx.物料编码 
   left join 基础数据物料信息表 fx on bom.产品编码=fx.物料编码 ";
            dt_bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = @"select kc.物料编码,base.物料名称,base.规格型号,存货分类,库存总数,未领量,自制, case when 工时=0 then 0 else round(7.6/工时,2) end as 工时,新数据,车间编号,计量单位编码,计量单位  from  
                        (select  物料编码,sum(库存总数)库存总数,sum(未领量)未领量  from 仓库物料数量表
                             where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 ='仓库类别' and 布尔字段2=1) group by 物料编码)  kc 
                  left join 基础数据物料信息表 base   on  base.物料编码=kc.物料编码 ";
            dt_库存 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            flag = 3;
            bl_sync = false;
            BeginInvoke(new MethodInvoker(() =>
            {
                simpleButton4.Text = string.Format("未完成工单:{0}条\r\n库存和bom已同步", IncompleteWorkOrder.Rows.Count); ;
                simpleButton4.Enabled = false;
            }));
        }

        //查看工单明细
        private void simpleButton3_Click(object sender, EventArgs e)
        {

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



        //查看销售订单明细   
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            Form1 fm = new Form1(dt_SaleOrder, "销售订单及预订单明细");
            fm.Size = new System.Drawing.Size(1500, 900);
            fm.StartPosition = FormStartPosition.CenterParent;
            fm.ShowDialog();

        }
        //计算
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

            //   if (!dt_SaleOrder.Columns.Contains("物料编码")) throw new Exception("销售订单中不含'物料编码'列");
            int x = dt_SaleOrder.Rows.Count;
            for (int i = x - 1; i >= 0; i--)
            {
                if (dt_SaleOrder.Rows[i]["物料编码"].ToString().Trim() == "")
                {
                    dt_SaleOrder.Rows.Remove(dt_SaleOrder.Rows[i]);
                }
            }
            x = IncompleteWorkOrder.Rows.Count;
            for (int i = x - 1; i >= 0; i--)
            {
                if (IncompleteWorkOrder.Rows[i]["物料编码"].ToString().Trim() == "")
                {
                    IncompleteWorkOrder.Rows.Remove(IncompleteWorkOrder.Rows[i]);
                }
            }
            //if (!IncompleteWorkOrder.Columns.Contains("生产订单号")) throw new Exception("生产订单中不含'生产订单号'列");
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
            j = 2;
            foreach (DataRow dr in IncompleteWorkOrder.Rows)
            {
                string s = dr["物料编码"].ToString().Trim();
                if (s.Contains("S") || s.Contains("s")) //contain 不区分大小写,先判断有没有 'smt'  有就去掉
                {
                    int d = s.IndexOf("S");
                    if (d == -1) d = s.IndexOf("s");
                    s = s.Substring(0, d);
                }
                if (s.Length < 14) throw new Exception(string.Format("生产订单中第{0}行物料编码位数不对", j));
                if (!decimal.TryParse(dr["未完成数量"].ToString(), out dec)) throw new Exception(string.Format("生产订单中第{0}行数量格式不正确", j));
                j++;
            }
        }

        private void calculate()
        {
            try
            {

                calcu_check();
                str_log = "";
                dtM = new DataTable();
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
                dt_SaleCrderCopy.Columns.Add("库存总数", typeof(decimal));
                dt_SaleCrderCopy.Columns.Add("未完成工单数", typeof(decimal));
                dt_SaleCrderCopy.Columns.Add("未领量", typeof(decimal));
                //IncompleteWorkOrdercopy.Columns.Add("未领量", typeof(decimal));
                //dt_SaleCrderCopy 加入库存数量和 未完成工单数  用于核对,gridcontrol1显示

                foreach (DataRow dr in dt_SaleCrderCopy.Rows)
                {
                    if (dr["物料编码"].ToString().Trim() != "")
                    {
                        // bool bl= dt_SaleCrderCopy.Columns.Contains("物料编码");
                        DataRow[] v = dt_库存.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                        if (v.Length == 0) { str_log = str_log + dr["物料编码"] + "总表中没有"; }
                        else
                        {
                            dr["库存总数"] = v[0]["库存总数"];
                            dr["未领量"] = v[0]["未领量"];
                        }
                        DataRow[] v1 = IncompleteWorkOrder.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                        if (v1.Length > 0) dr["未完成工单数"] = v1[0]["未完成数量"];
                    }
                }

                DataColumn dc = new DataColumn("选择", typeof(bool));
                dc.DefaultValue = false;
                dt_SaleCrderCopy.Columns.Add(dc);

                foreach (DataRow dr in IncompleteWorkOrdercopy.Rows)
                {
                    DataRow[] v = dt_库存.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    dr["未领量"] = v[0]["未领量"];
                }


                //销售订单和 未完成订单汇总 成两列 物料编码 和 数量 即按物料汇总 销售订单和 未完成工单 
                MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
                DataTable dt_SaleOrder_1 = RBQ.SelectGroupByInto("", dt_SaleOrder, "物料编码,sum(数量) 数量", "", "物料编码");

                DataTable IncompleteWorkOrder_1 = RBQ.SelectGroupByInto("", IncompleteWorkOrder, "物料编码,sum(未完成数量) 数量", "", "物料编码");

                //dt_totalcount 复制 dt_库存,库存总数+未完成订单数
                dt_totalcount = dt_库存.Copy();
                dt_totalcount.Columns.Add("总数", typeof(decimal));
                dt_totalcount.Columns.Add("未完成工单数", typeof(decimal));
                dt_totalcount.Columns.Add("销售数量", typeof(decimal));

                foreach (DataRow xr in dt_totalcount.Rows)
                {
                    decimal dec = 0;
                    DataRow[] r = IncompleteWorkOrder_1.Select(string.Format("物料编码='{0}'", xr["物料编码"]));
                    if (r.Length > 0)
                    {
                        dec = Convert.ToDecimal(r[0]["数量"]);
                    }
                    xr["未完成工单数"] = dec;
                    xr["总数"] = Convert.ToDecimal(xr["库存总数"]) + dec;

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
                    if (r_total.Length == 0)
                    {
                        str_log = dr["物料编码"].ToString() + "在可用库存中未有记录";
                    }
                    decimal total = Convert.ToDecimal(r_total[0]["总数"]);
                    decimal kczs = Convert.ToDecimal(r_total[0]["库存总数"]);
                    decimal wwcgds = Convert.ToDecimal(r_total[0]["未完成工单数"]);

                    if (total >= dec_订单数) //库存加未完成>需求数
                    {
                        r_total[0]["总数"] = total - dec_订单数;

                    }
                    else
                    {
                        DataRow r_need = dtM.NewRow();
                        r_need["未完成工单数"] = wwcgds;
                        r_need["物料编码"] = r_total[0]["物料编码"];
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
                DataTable dtMcopy = dtM.Copy();
                //fun_dg(dtMcopy);

                foreach (DataRow dr in dtMcopy.Rows)
                {
                    if (dr["自制"].Equals(true))
                    {
                        if (dt_bom.Select(string.Format("产品编码='{0}'", dr["物料编码"])).Length == 0) str_log = str_log + dr["物料编码"].ToString() + "属性为自制但是没有bom; ";
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
                                bool bl_t = false;
                                if (dr["自制"] == null || dr["自制"].ToString().Trim() == "")
                                {
                                    bl_t = false;
                                }
                                else
                                {
                                    bl_t = Convert.ToBoolean(dr["自制"]);
                                }

                                fun_dg(stock_total[0]["物料编码"].ToString(), dec - total_z, bl_t);

                            }
                        }
                    }
                }
                //不缺的 也要求显示 参考数量为0 
                foreach (DataRow dr in dt_SaleOrder_1.Rows)
                {
                    DataRow[] xxx = dtM.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (xxx.Length == 0)
                    {
                        DataRow[] r_total = dt_totalcount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                        decimal total = Convert.ToDecimal(r_total[0]["总数"]);
                        decimal kczs = Convert.ToDecimal(r_total[0]["库存总数"]);
                        decimal wwcgds = Convert.ToDecimal(r_total[0]["未完成工单数"]);
                        DataRow r_need = dtM.NewRow();
                        r_need["未完成工单数"] = wwcgds;
                        r_need["物料编码"] = r_total[0]["物料编码"];
                        r_need["物料名称"] = r_total[0]["物料名称"];
                        r_need["规格型号"] = r_total[0]["规格型号"];
                        r_need["存货分类"] = r_total[0]["存货分类"];
                        r_need["库存总数"] = kczs;
                        r_need["销售数量"] = r_total[0]["销售数量"];
                        r_need["自制"] = r_total[0]["自制"];
                        r_need["参考数量"] = 0;
                        dtM.Rows.Add(r_need);
                    }
                }
                dtM.Columns.Add("最早发货日期",typeof(DateTime));
                foreach (DataRow dr in dtM.Rows)
                {
                    
                         DataRow[] rr = dt_SaleOrder.Select(string.Format("物料编码='{0}'", dr["物料编码"]),"预计发货日期 asc ");
                         if (rr.Length > 0)
                         {
                             dr["最早发货日期"] = rr[0]["预计发货日期"];
                             DataTable t = new DataTable();
                             t = ERPorg.Corg.billofM(t, dr["物料编码"].ToString(), false,dt_bom);
                             foreach (DataRow rrr in t.Rows)
                             {
                                 DataRow[] r = dtM.Select(string.Format("物料编码='{0}'", rrr["子项编码"]));
                                 // r.length 只会小于等于一条 
                                 if (r.Length == 0) continue;
                                 else
                                     r[0]["最早发货日期"] = rr[0]["预计发货日期"];

                             }

                         }
                }

                if (str_log != "")
                {
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        label6.Text = str_log;

                    }));
                }
                DataView search_source = new DataView(dtM);
                search_source.RowFilter = "销售数量>0";
                BeginInvoke(new MethodInvoker(() =>
                {
                    simpleButton5.Text = "计算完成";
                }));
                bl_calculate = false; //计算完成

                BeginInvoke(new MethodInvoker(() =>
                {
                    DataView dv = new DataView(dtM);
                    dv.RowFilter = "自制='true' ";
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
                    label6.Text = str_log + "错误原因:" + ex.Message;
                    simpleButton5.Text = "计算错误";
                }));
                bl_calculate = false;

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
                if (dt_bom.Select(string.Format("产品编码='{0}'", itemid)).Length == 0) str_log = str_log + itemid + "属性为自制但是没有bom; ";
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

        private void button2_Click(object sender, EventArgs e)
        {
            DataView dv = new DataView(dtM);
            dv.RowFilter = "自制='true'";
            gc2.DataSource = dv;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
                {
                    string s = "自制='true'";
                    DataTable ListM = new DataTable();
                    ListM = ERPorg.Corg.billofM(ListM, searchLookUpEdit1.EditValue.ToString(), true,dt_bom);
                    if (ListM.Rows.Count > 0)
                    {
                        DataView dv = new DataView(dtM);
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

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }



        private void gv2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
                if (dr == null) return;
                Thread th = new Thread(() =>
                {
                    DataTable dtz = new DataTable();
                    // dtz.Columns.Add("产品编码");
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
                        saleDisplay = dv.ToTable();
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            gridControl1.DataSource = saleDisplay;
                        }));

                    }
                    else
                    {
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            gridControl1.DataSource = dt_SaleCrderCopy.Clone();
                        }));

                    }
                    DataTable dt_x = new DataTable();
                    dt_x = ERPorg.Corg.billofM(dt_x, dr["物料编码"].ToString(), false,dt_bom);

                    s = "";
                    if (dt_x.Rows.Count > 0)
                    {
                        s = "物料编码 in (";
                        foreach (DataRow xx in dt_x.Rows)
                        {
                            s = s + "'" + xx["子项编码"].ToString() + "',";
                        }
                        s = s.Substring(0, s.Length - 1) + ")";
                        DataView dv_z = new DataView(IncompleteWorkOrdercopy);
                        dv_z.RowFilter = s;
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            gridControl2.DataSource = dv_z;
                        }));
                    }
                    else
                    {
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            gridControl2.DataSource = IncompleteWorkOrdercopy.Clone();
                        }));
                    }
                });
                th.Start();


                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gc2, new Point(e.X, e.Y));
                    gv2.CloseEditor();
                    this.BindingContext[dtM].EndCurrentEdit();
                    contextMenuStrip1.Tag = gv2;
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        private void check()
        {
            DataView v = new DataView(saleDisplay);
            v.RowFilter = "选择=1";
            DataTable t = v.ToTable();
            if (v.ToTable().Rows.Count == 0) throw new Exception("未选择关联任何销售明细");

        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //
            try
            {
                this.ActiveControl = null;
                check();

                if (bl_calculate) throw new Exception("正在计算中,不可进行此操作");


                //dt 取生产制令表 结构  
                DataTable dt = CZMaster.MasterSQL.Get_DataTable("select  * from 生产记录生产制令表  where 1=2", strcon); //此dt传入 转制令界面

                DataTable t = new DataTable(); //用户选择的销售订单
                DataView dv_1 = new DataView(saleDisplay);
                dv_1.RowFilter = "选择=1";
                t = dv_1.ToTable();
                //t.Columns["销售数量"].ColumnName = "数量";
                t.Columns["订单号"].ColumnName = "销售订单号";
                t.Columns.Add("销售订单明细号");
                foreach (DataRow dr in t.Rows)
                {
                    dr["销售订单明细号"] = dr["销售订单号"].ToString() + "-" + Convert.ToInt32(dr["行号"]).ToString("00");
                }
              


                //  DataTable tttt = gridControl1;
                //step1.遍历 t
                DataView dv = new DataView(dtM);
                string s = " 自制=1 and 参考数量>0 and 物料编码 in (";

                DataTable t_relation = new DataTable();
                foreach (DataRow dr in t.Rows)
                {
                    //取 dr["物料编码"]及其所有子项得 计算结果 
                    DataTable dt_x = new DataTable();
                    dt_x = ERPorg.Corg.billofM(dt_x, dr["物料编码"].ToString(), true,dt_bom);
                    DataColumn dc = new DataColumn("销售订单号", typeof(string));
                    dc.DefaultValue = dr["销售订单号"].ToString();
                    dt_x.Columns.Add(dc);

                    DataColumn dc1 = new DataColumn("销售订单明细号", typeof(string));
                    dc1.DefaultValue = dr["销售订单明细号"].ToString() ;
                    dt_x.Columns.Add(dc1);

                    DataColumn dc2 = new DataColumn("应完工日期", typeof(DateTime));
                    DataRow[] sr = dt_SaleCrderCopy.Select(string.Format("订单号='{0}' and 行号={1}", dr["销售订单号"].ToString(), Convert.ToInt32(dr["行号"])));
                    dc2.DefaultValue = sr[0]["应完工日期"];
                    dt_x.Columns.Add(dc2);

                    if (t_relation.Columns.Count == 0) t_relation = dt_x.Copy();
                    else t_relation.Merge(dt_x); //取到 

                    foreach (DataRow cdr in dt_x.Rows) //这边重复也没事
                    {
                        s = s + string.Format("'{0}',", cdr["子项编码"]);
                    }
                }
                s = s.Substring(0, s.Length - 1) + ")";

                dv.RowFilter = s;//这里筛选所有需要带过去的 产品、半成品,此为需要生成生产制令的清单，t_relation 为销售订单明细  及 物料的对应关系 需要根据这个生产 制令子表的记录
                DataTable tt = dv.ToTable();
                if (tt.Rows.Count == 0) throw new Exception("所选物料没有任何子项需要生产");
                s = "子项编码 in (";
                foreach (DataRow vr in tt.Rows)
                {
                    s = s + string.Format("'{0}',", vr["物料编码"]);
                }
                s = s.Substring(0, s.Length - 1) + ")";

                DataView v_relation = new DataView(t_relation);

                v_relation.RowFilter = s;

                t_relation = v_relation.ToTable();
                DataSet ds = new DataSet();
                ds.Tables.Add(dt_库存.Copy());//基础信息及库存
                ds.Tables.Add(dt_bom.Copy());
                ds.Tables.Add(t_relation.Copy());//tt中物料与销售订单的对应关系
                ds.Tables.Add(tt.Copy());//根据所有需要生成制令的物料清单
                ds.Tables.Add(t.Copy());//用户选中的 销售订单
                ui计划池转制令_u8 ui = new ui计划池转制令_u8(ds);
                CPublic.UIcontrol.Showpage(ui, "转制令确认");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void simpleButton6_Click(object sender, EventArgs e)
        {
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

        private void gv2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv2.GetFocusedRowCellValue(gv2.FocusedColumn));
                e.Handled = true;
            }
        }

        private void gridView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView1.GetFocusedRowCellValue(gridView1.FocusedColumn));
                e.Handled = true;
            }
        }

        private void gridView2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView2.GetFocusedRowCellValue(gridView2.FocusedColumn));
                e.Handled = true;
            }
        }

        private void gv2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }



        private void 查看bom明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView ff = ((sender as ToolStripDropDownItem).Owner as ContextMenuStrip).Tag as DevExpress.XtraGrid.Views.Grid.GridView;
                DataRow r = ff.GetDataRow(ff.FocusedRowHandle);
                Decimal dec;
                if (contextMenuStrip1.Tag == gv2)
                {
                    if (r["参考数量"] != DBNull.Value && r["参考数量"].ToString() != "")
                    {
                        dec = Convert.ToDecimal(r["参考数量"].ToString());
                    }
                    else
                    {
                        dec = 1;
                    }
                }
                else
                {
                    if (r["数量"] != DBNull.Value && r["数量"].ToString() != "")
                    {
                        dec = Convert.ToDecimal(r["数量"].ToString());
                    }
                    else
                    {
                        dec = 1;
                    }
                }
                ERPproduct.UI物料BOM详细数量 frm = new ERPproduct.UI物料BOM详细数量(r["物料编码"].ToString().Trim(), dec);
                CPublic.UIcontrol.AddNewPage(frm, "详细数量");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                gridView1.CloseEditor();
                this.BindingContext[saleDisplay].EndCurrentEdit();
                contextMenuStrip1.Tag = gridView1;
            }
        }

    }
}
