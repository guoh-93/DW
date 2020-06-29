using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Data.SqlClient;


namespace ERPpurchase
{
    public partial class ui临时用采购池 : UserControl
    {

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
        /// <summary>
        /// flag 指示用户进度 ,导入销售明细--1,导入未完成工单--2,同步BOM及库存--3  计算完成--4
        /// </summary>
        int flag = 0;
        bool bl_sync = false;
        bool bl_calculate = false;
        string str_log = "";
        public ui临时用采购池()
        {
            InitializeComponent();
        }



        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (flag == 0)
                {
                    var ofd = new OpenFileDialog();
                    if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        //  dt_SaleOrder = ERPorg.Corg.ExcelXLSX(ofd);
                        bool bl = ERPorg.Corg.IsFileInUse(ofd.FileName);
                        if (bl) throw new Exception("文件已打开或被占用中");

                        Thread th = new Thread(() =>
                        {
                            BeginInvoke(new MethodInvoker(() =>
                            {
                                simpleButton1.Enabled = false;
                                simpleButton1.Text = "导入中..";
                            }));
                            dt_SaleOrder = ERPorg.Corg.ReadExcelToDataTable(ofd.FileName);

                            //DateTime t = CPublic.Var.getDatetime().Date;
                            //string fileName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DW导入销售单\\计划";
                            //if (Directory.Exists(fileName) == false)
                            //{
                            //    Directory.CreateDirectory(fileName);
                            //}
                            int x = dt_SaleOrder.Rows.Count;
                            for (int i = x - 1; i >= 0; i--)
                            {
                                if (dt_SaleOrder.Rows[i]["物料编码"].ToString().Trim() == "")
                                {
                                    dt_SaleOrder.Rows.Remove(dt_SaleOrder.Rows[i]);
                                }
                            }

                            flag = 1;
                            BeginInvoke(new MethodInvoker(() =>
                            {
                                simpleButton1.Text = string.Format("销售明细:{0}条", dt_SaleOrder.Rows.Count);
                            }));
                        });
                        th.Start();

                    }
                }
                else
                {
                    throw new Exception("请按步骤操作");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {


            try
            {
                if (flag == 1)
                {


                    Thread th = new Thread(() =>
                       {
                           BeginInvoke(new MethodInvoker(() =>
                           {
                               simpleButton2.Enabled = false;
                               simpleButton2.Text = "拉取数据中..";
                           }));

                           //未完成工单
                           string s = @"select  mocode 生产订单号,invcode 物料编码,qty-QualifiedInQty 未完成数量,base.规格型号,存货分类,未领量,在途量  from [192.168.20.150].UFDATA_008_2018.dbo.mom_orderdetail 
 left join [192.168.20.150].UFDATA_008_2018.dbo.mom_order  on mom_order.Moid=mom_orderdetail.moid
 left join 基础数据物料信息表  base on base.物料编码=invcode 
 left join  (select  物料编码,sum(库存总数)库存总数,max(未领量)未领量,max(在途量) as 在途量  from 仓库物料数量表
      where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 ='仓库类别' and 布尔字段2=1) group by 物料编码)  kc 
  on  base.物料编码=kc.物料编码
  where  mom_orderdetail.Status=3 and qty-QualifiedInQty>0 ";
                           IncompleteWorkOrder = CZMaster.MasterSQL.Get_DataTable(s, strcon);

                           //未完成采购单   未完成采购单 加 未完成委外单
                           s = @"  select  b.cpoid,b.cInvCode 物料编码 ,b.采购数量,采购数量-isnull(入库数量,0) as 未入库数量,base.规格型号,存货分类,库存总数,在途量,未领量   from  
   (select cgd.cPOID,cgmx.cInvCode,sum(cgmx.iQuantity) as 采购数量  from [192.168.20.150].UFDATA_008_2018.dbo.PO_Pomain cgd
			left join [192.168.20.150].UFDATA_008_2018.dbo.PO_Podetails cgmx on cgd.POID=cgmx.POID where cgd.cState=1  group by cgd.cPOID,cgmx.cInvCode)b
left join (select  cInvCode,sum(isnull(iQuantity,0))入库数量,cPOID from  [192.168.20.150].UFDATA_008_2018.dbo.rdrecords01 where cPOID is not null  group by cInvCode,cPOID)a
on a.cPOID=b.cPOID and a.cInvCode=b.cInvCode  
left join 基础数据物料信息表 base on  base.物料编码=b.cInvCode
left join  (select  物料编码,sum(库存总数)库存总数,max(未领量)未领量,max(在途量) as 在途量  from 仓库物料数量表
                    where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 ='仓库类别' and 布尔字段2=1) group by 物料编码)  kc 
 on  base.物料编码=kc.物料编码 where 采购数量-isnull(入库数量,0)>0
 union 
  select  OM_MOMain.cCode as cpoid,cInvCode 物料编码,iQuantity 采购数量,iQuantity-isnull(freceivedqty,0) as 未入库数量,base.规格型号,存货分类,库存总数,在途量,未领量
   from [192.168.20.150].UFDATA_008_2018.dbo.OM_MODetails
 inner join [192.168.20.150].UFDATA_008_2018.dbo.OM_MOMain on OM_MOMain.MOID=OM_MODetails.MOID
left join 基础数据物料信息表 base on  base.物料编码= cInvCode
left join  (select  物料编码,sum(库存总数)库存总数,max(未领量)未领量,max(在途量) as 在途量  from 仓库物料数量表
                             where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 ='仓库类别' and 布尔字段2=1) group by 物料编码)  kc 
 on  base.物料编码=kc.物料编码
 where OM_MOMain.cState=1 and iQuantity>isnull(freceivedqty,0) ";
                           IncompletePO = CZMaster.MasterSQL.Get_DataTable(s, strcon);

                           flag = 2;
                           BeginInvoke(new MethodInvoker(() =>
                           {
                               simpleButton2.Text = string.Format("未完成工单:{0}条\r\n未完成采购单{1}条", IncompleteWorkOrder.Rows.Count, IncompletePO.Rows.Count);
                           }));
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


            }


        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            if (flag == 2)
            {
                simpleButton3.Text = "正在同步中..稍候";

                Thread th = new Thread(fun_sync);
                th.IsBackground = true;
                th.Start();
                bl_sync = true;

            }
            else
            {
                MessageBox.Show("请按步骤操作");
            }
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            if (!bl_sync)
            {
                flag = 0;
                str_log = "";
                bl_calculate = false;
                label4.Text = "";
                simpleButton1.Enabled = true;
                simpleButton1.Text = "导入销售订单";

                simpleButton2.Enabled = true;
                simpleButton2.Text = "导入未完成工单";

                simpleButton3.Enabled = true;
                simpleButton3.Text = "同步相关BOM,库存";
                simpleButton4.Text = "开始计算";
            }
        }
        private void fun_sync()
        {
            string s = "";
            //string s = "exec sync_u8_data "; //基础信息
            //CZMaster.MasterSQL.ExecuteSQL(s, strcon);
            //s = "exec sync_u8_stock "; //库存
            //CZMaster.MasterSQL.ExecuteSQL(s, strcon);
            //s = "exec sync_u8_bom "; //bom
            //CZMaster.MasterSQL.ExecuteSQL(s, strcon);
            //s = "exec  sync_u8_OnMake"; //在制量
            //CZMaster.MasterSQL.ExecuteSQL(s, strcon);
            //s = "exec  sync_UnclaimedCount;exec sync_u8_InTransit"; //未领量在途
            //CZMaster.MasterSQL.ExecuteSQL(s, strcon);

            s = @"  select  产品编码,产品名称,fx.存货分类 as 父项分类,fx.规格型号 as 父项规格,子项编码,子项名称,数量,zx.自制 as 子项自制,zx.可购 as 子项可购,zx.存货分类 as 子项分类
    ,zx.规格型号 as 子项规格       from 基础数据物料BOM表 bom 
   left join 基础数据物料信息表 zx on bom.子项编码=zx.物料编码 
   left join 基础数据物料信息表 fx on bom.产品编码=fx.物料编码 ";
            dt_bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = @" select kc.物料编码,base.物料名称,base.规格型号,存货分类,库存总数,未领量,自制,在途量,在制量,isNull(委外在途,0)委外在途,可购,0 需求数量  from  
  (select  物料编码,sum(库存总数)库存总数,max(未领量)未领量,max(在途量) as 在途量,max(在制量)在制量  from 仓库物料数量表
    where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 ='仓库类别' and 布尔字段2=1) group by 物料编码)  kc 
    left join 基础数据物料信息表 base   on  base.物料编码=kc.物料编码
    left  join (  select  cinvcode, sum(iQuantity-ISNULL(freceivedqty,0))委外在途  from (
 select  OM_MOMain.cCode ,MODetailsID,cInvCode,iQuantity,freceivedqty,dbCloseDate,cState from [192.168.20.150].UFDATA_008_2018.dbo.OM_MODetails
 inner join [192.168.20.150].UFDATA_008_2018.dbo.OM_MOMain on OM_MOMain.MOID=OM_MODetails.MOID
 where OM_MOMain.cState=1)ww group by cInvCode) ww on ww.cinvcode=base.物料编码";
            dt_库存 = CZMaster.MasterSQL.Get_DataTable(s, strcon);




            flag = 3;
            bl_sync = false;

            BeginInvoke(new MethodInvoker(() =>
            {
                simpleButton3.Text = "已同步,并加载完成";
                simpleButton3.Enabled = false;

            }));
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            try
            {
                if (flag < 1) throw new Exception("信息尚未准备完全,请按步骤操作");
                if (bl_calculate) throw new Exception("正在计算中..");


                Thread th = new Thread(cal_9_18);
                th.IsBackground = true;
                th.Start();
                bl_calculate = true;
                label6.Text = "正在计算中..";


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
            //   j = 2;
            //foreach (DataRow dr in IncompleteWorkOrder.Rows)
            //{
            //    string s = dr["物料编码"].ToString().Trim();
            //    if (s.Contains("S") || s.Contains("s")) //contain 不区分大小写,先判断有没有 'smt'  有就去掉
            //    {
            //        int d = s.IndexOf("S");
            //        if (d == -1) d = s.IndexOf("s");
            //        s = s.Substring(0, d);
            //    }
            //    if (s.Length < 14) throw new Exception(string.Format("生产订单中第{0}行物料编码位数不对", j));
            //    if (!decimal.TryParse(dr["未完成数量"].ToString(), out dec)) throw new Exception(string.Format("生产订单中第{0}行数量格式不正确", j));
            //    j++;
            //}
        }
        /// <summary>
        /// 19-7-1  采购根据生产主计划计算缺料情况
        /// 19-9-18 采购需求变更 
        /// </summary>
        private void cal()
        {

            //ERPorg.Corg.result rs = new ERPorg.Corg.result();
            //rs = ERPorg.Corg.fun_pool( dt_SaleOrder, true);
            //dtM = rs.dtM;
            //dt_totalcount = rs.TotalCount;
            ////DataView dv = new DataView(dtM);
            //////dv.RowFilter = "可购=1 and 参考量>0";
            ////dv.RowFilter = "需求数量>0";
            //DataView dv = new DataView(dt_totalcount);
            //dv.RowFilter = "需求数量>0";
            //BeginInvoke(new MethodInvoker(() =>
            //{
            //    gc2.DataSource = dv;
            //    simpleButton4.Text = "计算完成";
            //}));
            DataTable dtM = new DataTable();
            dtM.Columns.Add("物料编码");
            dtM.Columns.Add("物料名称");
            dtM.Columns.Add("规格型号");
            dtM.Columns.Add("需求数量", typeof(decimal));
            dtM.Columns.Add("自制", typeof(bool));
            dtM.Columns.Add("可购", typeof(bool));
            dtM.Columns.Add("未领量", typeof(decimal));
            dtM.Columns.Add("库存总数", typeof(decimal));
            dtM.Columns.Add("受订量", typeof(decimal));
            dtM.Columns.Add("在制量", typeof(decimal));
            dtM.Columns.Add("在途量", typeof(decimal));




            string s = "select * from V_pooltotal";
            dt_totalcount = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            foreach (DataRow dr in dt_SaleOrder.Rows)
            {
                DataTable dt_x = new DataTable();
                dt_x = ERPorg.Corg.billofM_带数量(dt_x, dr["物料编码"].ToString(), false);
                foreach (DataRow rr in dt_x.Rows)
                {
                    decimal dec = Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(rr["数量"]);
                    DataRow[] rrr = dtM.Select(string.Format("物料编码='{0}'", rr["子项编码"]));
                    if (rrr.Length == 0)
                    {
                        DataRow tr = dtM.NewRow();
                        tr["物料编码"] = rr["子项编码"];
                        DataRow[] base_info = dt_totalcount.Select(string.Format("物料编码='{0}'", tr["物料编码"]));
                        tr["物料名称"] = base_info[0]["物料名称"];
                        tr["规格型号"] = base_info[0]["规格型号"];
                        tr["自制"] = base_info[0]["自制"];
                        tr["可购"] = base_info[0]["可购"];
                        tr["库存总数"] = base_info[0]["库存总数"];
                        tr["受订量"] = base_info[0]["受订量"];
                        tr["在制量"] = base_info[0]["在制量"];
                        tr["未领量"] = base_info[0]["未领量"];
                        tr["在途量"] = base_info[0]["在途量"];
                        tr["需求数量"] = dec;
                        dtM.Rows.Add(tr);
                    }
                    else
                    {
                        rrr[0]["需求数量"] = dec + Convert.ToDecimal(rrr[0]["需求数量"]);
                    }
                }

            }
            BeginInvoke(new MethodInvoker(() =>
            {
                gc2.DataSource = dtM;
            }));
        }


        private void cal_9_18()
        {
            try
            {
                //BeginInvoke(new MethodInvoker(() =>
                //{
                //    label6.Text = "正在计算中,请稍候...";
                //}));

                ERPorg.Corg.result rs = new ERPorg.Corg.result();
                rs = ERPorg.Corg.fun_pool(dt_SaleOrder, true);
                dtM = rs.dtM;
                DataColumn dc = new DataColumn("选择", typeof(bool));
                dc.DefaultValue = false;
                dtM.Columns.Add(dc);
                //dtM.Columns.Add("最早发货日期", typeof(DateTime));
                dt_bom = rs.Bom;
                dt_totalcount = rs.TotalCount;
                dt_SaleOrder = rs.salelist_mx;
                IncompletePO = rs.Polist_mx;
                str_log = rs.str_log;
                //dt_SaleOrder.Columns.Add("应完工日期", typeof(DateTime));

                //foreach (DataRow saleR in dt_SaleOrder.Rows)
                //{
                //    saleR["应完工日期"] = Convert.ToDateTime(saleR["最早发货日期"]).AddDays(-1);
                //}

                dt_SaleCrderCopy = dt_SaleOrder.Copy();

                //19-10-10 两种版本二合一
                dtM.Columns.Add("总需求", typeof(decimal));

                DataTable dtM_总需求 = new DataTable();

                dtM_总需求.Columns.Add("物料编码");
                dtM_总需求.Columns.Add("物料名称");
                dtM_总需求.Columns.Add("规格型号");
                dtM_总需求.Columns.Add("存货分类");
                dtM_总需求.Columns.Add("需求数量", typeof(decimal));
                dtM_总需求.Columns.Add("自制", typeof(bool));
                dtM_总需求.Columns.Add("可购", typeof(bool));
                dtM_总需求.Columns.Add("未领量", typeof(decimal));
                dtM_总需求.Columns.Add("库存总数", typeof(decimal));
                dtM_总需求.Columns.Add("受订量", typeof(decimal));
                dtM_总需求.Columns.Add("在制量", typeof(decimal));
                dtM_总需求.Columns.Add("在途量", typeof(decimal));
                //这里的需求数量、总需求是界面上的订单用量 是界面上的
                foreach (DataRow dr in dt_SaleOrder.Rows)
                {
                    DataTable dt_x = new DataTable();
                    dt_x = ERPorg.Corg.billofM_带数量(dt_x, dr["物料编码"].ToString(), false);
                    foreach (DataRow rr in dt_x.Rows)
                    {
                        decimal dec = Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(rr["数量"]);
                        DataRow[] rrr = dtM_总需求.Select(string.Format("物料编码='{0}'", rr["子项编码"]));
                        if (rrr.Length == 0)
                        {
                            DataRow tr = dtM_总需求.NewRow();
                            tr["物料编码"] = rr["子项编码"];
                            DataRow[] base_info = dt_totalcount.Select(string.Format("物料编码='{0}'", tr["物料编码"]));
                            tr["物料名称"] = base_info[0]["物料名称"];
                            tr["规格型号"] = base_info[0]["规格型号"];
                            tr["存货分类"] = base_info[0]["存货分类"];
                            tr["自制"] = base_info[0]["自制"];
                            tr["可购"] = base_info[0]["可购"];
                            tr["库存总数"] = base_info[0]["库存总数"];
                            tr["受订量"] = base_info[0]["受订量"];
                            tr["在制量"] = base_info[0]["在制量"];
                            tr["未领量"] = base_info[0]["未领量"];
                            tr["在途量"] = base_info[0]["在途量"];
                            tr["需求数量"] = dec;
                            dtM_总需求.Rows.Add(tr);
                        }
                        else
                        {
                            rrr[0]["需求数量"] = dec + Convert.ToDecimal(rrr[0]["需求数量"]);
                        }
                    }

                }

                foreach (DataRow dr in dtM_总需求.Rows)
                {
                    DataRow[] yy = dtM.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (yy.Length == 0)
                    {
                        DataRow r_need =  dtM.NewRow();
                        //r_need["在制量"] = dr["在制量"];
                        r_need["物料编码"] =dr["物料编码"];
                        r_need["在途量"] = dr["在途量"];
                        r_need["未领量"] = dr["未领量"];
                        r_need["物料名称"] =dr["物料名称"];
                        r_need["规格型号"] =dr["规格型号"];
                        r_need["存货分类"] = dr["存货分类"];
                        r_need["库存总数"] = dr["库存总数"];
                        r_need["受订量"] = dr["受订量"];
                        r_need["自制"] = dr["自制"];
                        //r_need["工时"] = dr["工时"];
                        r_need["总需求"] = dr["需求数量"];
                        dtM.Rows.Add(r_need);
                    }
                    else
                    {
                        yy[0]["总需求"] = dr["需求数量"];
                    }
                }


                bl_calculate = false;
                BeginInvoke(new MethodInvoker(() =>
                {
                    if (rs.str_log != "")
                    {
                        label6.Text = rs.str_log;
                    }
                    else
                    {
                        label6.Text = "---";
                    }
                    DataView dv = new DataView(dtM);
                    dv.RowFilter = "总需求>0 and (可购=1 or 委外=1)";
                    gc2.DataSource = dv;
                    //DataTable search_source = dt_SaleOrder.Copy();
                    //searchLookUpEdit1.Properties.DataSource = search_source;
                    //searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                    //searchLookUpEdit1.Properties.ValueMember = "物料编码";

                }));
            }
            catch (Exception ex)
            {
                bl_calculate = false;
                BeginInvoke(new MethodInvoker(() =>
                {
                    label6.Text = "错误原因:" + ex.Message;
                    bl_calculate = false;
                }));
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
                //19-3-28 未完成工单 换成 在制量  再减去未领量
                foreach (DataRow dr in dt_SaleCrderCopy.Rows)
                {
                    if (dr["物料编码"].ToString().Trim() != "")
                    {

                        DataRow[] v = dt_库存.Select(string.Format("物料编码='{0}'", dr["物料编码"]));

                        if (v.Length == 0)
                        {
                            str_log = str_log + "," + dr["物料编码"].ToString();
                        }
                        else
                        {
                            dr["库存总数"] = v[0]["库存总数"];
                            dr["未领量"] = v[0]["未领量"];
                            dr["在途量"] = v[0]["在途量"];
                            dr["未完成工单数"] = v[0]["在制量"];
                            //DataRow[] v1 = IncompleteWorkOrder.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                            //if (v1.Length > 0) dr["未完成工单数"] = v1[0]["未完成数量"];
                        }
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
                    //DataRow[] r = IncompleteWorkOrder_1.Select(string.Format("物料编码='{0}'", xr["物料编码"]));
                    //if (r.Length > 0)
                    //{
                    //    dec = Convert.ToDecimal(r[0]["数量"]);
                    //}
                    dec = Convert.ToDecimal(xr["在制量"]);
                    xr["未完成工单数"] = dec;
                    // 在途量  存储过程中已经算了 委外在途量 这里不能再加了+ Convert.ToDecimal(xr["委外在途"])
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
                            label4.Text = str_log;

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
                        //decimal dec_n = 0;

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
                BeginInvoke(new MethodInvoker(() =>
                {
                    simpleButton4.Text = "计算完成";
                    flag = 4;
                }));


                bl_calculate = false; //计算完成
                method(gc2, gd =>
                {
                    DataView dv = new DataView(dtM_PurchasePool);
                    dv.RowFilter = "可购='true'";
                    gc2.DataSource = dv;
                    searchLookUpEdit1.Properties.DataSource = search_source;
                    searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                    searchLookUpEdit1.Properties.ValueMember = "物料编码";


                });
            }
            catch (Exception ex)
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    label4.Text = "错误原因:" + ex.Message;
                    simpleButton4.Text = "计算错误";
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
        private void method<T>(T c, Action<T> action) where T : DevExpress.XtraGrid.GridControl
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => action(c)));
            }
            else
                action(c);
        }

        private void gv2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gv2_KeyDown(object sender, KeyEventArgs e)
        {
            if (gv2.FocusedColumn.Caption == "物料编码" || gv2.FocusedColumn.Caption == "规格型号")
            {
                if (e.Control && e.KeyCode == Keys.C)
                {
                    Clipboard.SetDataObject(gv2.GetFocusedRowCellValue(gv2.FocusedColumn));
                    e.Handled = true;
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
                    ListM = ERPorg.Corg.billofM(ListM, searchLookUpEdit1.EditValue.ToString(), true, dt_bom);
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

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Excel文件(*.xlsx)|*.xlsx";
            if (save.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(save.FileName, FileMode.Create, FileAccess.Write);
                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = string.Format("select * from 基础记录打印模板表 where 模板名 = '计划池导入销售模板'");
                dtPP = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                if (dtPP.Rows.Count == 0) throw new Exception("没有该模板");
                System.IO.File.WriteAllBytes(save.FileName + ".xlsx", (byte[])dtPP.Rows[0]["数据"]);
                MessageBox.Show(" 下载成功");

            }
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                gridView1.CloseEditor();
                contextMenuStrip1.Tag = gridView1;

            }
        }

        private void 查看bom明细ToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                gc2.ExportToXlsx(saveFileDialog.FileName);
                MessageBox.Show("导出成功");
            }
        }
        string[] sArray = null;

        private void simpleButton7_Click(object sender, EventArgs e)
        {

            ///      dtM_PurchasePool

            // DataTable test_暂存 = (DataTable)gc2.DataSource;
            int a = 0;
            string pi = "";
            DataTable test_暂存;

            try
            {
                if (flag < 4)
                {


                    throw new Exception("当前无数据可提交");

                }
                else
                {


                    test_暂存 = dtM_PurchasePool.Copy();
                    test_暂存.Columns.Add("日期");
                    test_暂存.Columns.Add("批号");
                    test_暂存.Columns.Add("提交人");
                    test_暂存.Columns.Add("提交人ID");
                }




                if (MessageBox.Show("确认提交吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {

                    DateTime dtime = CPublic.Var.getDatetime();
                    string dtie = "";

                    dtie = dtime.Year.ToString() + dtime.Month.ToString("00") + dtime.Day.ToString("00");
                    string sql_原 = string.Format("SELECT MAX(批号) as 批号 FROM 财务采购提交确认表 where left (批号,8) ='{0}'", dtie);
                    DataTable dt_原数据 = CZMaster.MasterSQL.Get_DataTable(sql_原, strcon);
                    if (dt_原数据.Rows[0]["批号"].ToString() != "")
                    {
                        string drws = dt_原数据.Rows[0]["批号"].ToString();

                        sArray = drws.Split('-');// 一定是单引 

                        a = Convert.ToInt32(sArray[1].ToString());
                        pi = (dtie.ToString() + "-" + (a + 1)).ToString();
                    }
                    else
                    {
                        a = 0;
                        pi = (dtie.ToString() + "-" + (a + 1)).ToString();
                    }



                    string sql = string.Format("select * from 财务采购提交确认表 where left (批号,8) ='{0}' and 提交人ID='{1}' ", dtie, CPublic.Var.LocalUserID);
                    DataTable dt_lost = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    if (dt_lost.Rows.Count > 0)
                    {
                        for (int i = dt_lost.Rows.Count - 1; i >= 0; i--)
                        {
                            DataRow dr = dt_lost.Rows[i];
                            dr.Delete();

                        }

                        //foreach (DataRow dr in dt_lost.Rows)
                        //{

                        //    dr.Delete();
                        //}

                    }

                    foreach (DataRow drg in test_暂存.Rows)
                    {


                        drg["日期"] = dtime;

                        drg["批号"] = pi;
                        textBox1.Text = pi.ToString();
                        drg["提交人"] = CPublic.Var.localUserName;

                        drg["提交人ID"] = CPublic.Var.LocalUserID;
                        dt_lost.ImportRow(drg);

                    }
                    using (SqlDataAdapter da = new SqlDataAdapter("select * from 财务采购提交确认表 where 1<>1", strcon))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dt_lost);
                        MessageBox.Show("提交成功");
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
            try
            {
                if (bl_sync) throw new Exception("正在数据同步..");
                if (bl_calculate) throw new Exception("正在计算中..");
                CPublic.UIcontrol.ClosePage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton8_Click(object sender, EventArgs e)
        {
            if (flag < 4) throw new Exception("尚未计算完成,稍候");
            DataView dv = new DataView(dt_totalcount);
            dv.RowFilter = "需求数量>0";
            DataTable dt = dv.ToTable();
            dt.Columns.Remove("总数");
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {

                ERPorg.Corg.TableToExcel(dt, saveFileDialog.FileName);
                MessageBox.Show("导出成功");
            }

        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                ERPorg.Corg.TableToExcel(dt_totalcount, saveFileDialog.FileName);
                MessageBox.Show("导出成功");
            }
        }

        private void ui临时用采购池_Load(object sender, EventArgs e)
        {

        }
    }
}
