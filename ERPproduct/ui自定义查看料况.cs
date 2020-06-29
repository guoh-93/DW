using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
namespace ERPproduct
{
    public partial class ui自定义查看料况 : UserControl
    {

        #region
        string strcon = CPublic.Var.strConn;
        /// <summary>
        /// 只加载所需要的
        /// </summary>
        DataTable bom;
        DataTable dt_总需;
        DataTable dt_库存;
        DataTable dt_制令;
        DataTable dt_缺料情况表;
        DataTable dt_AddInv;
        DataTable dt_AddInv2;
        /// <summary>
        /// 其他占用量是否需要扣减此单需求量
        /// 是否由制令转过来
        /// </summary>
        string cfgfilepath = "";
        bool bl_calculate = false;


        #endregion
        public ui自定义查看料况()
        {
            InitializeComponent();
        }

        private void fun_load()
        {
            string s = "select  * from 生产记录生产制令表 where 关闭=0 and 完成=0 and 生产制令类型 <>'返修制令' ";
            dt_制令 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = " select  产品编码,子项编码,数量,WIPType,组,优先级 from 基础数据物料BOM表 where 主辅料='主料' ";
            bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = @"select  base.物料名称,base.规格型号,存货分类,kc.* from 基础数据物料信息表 base
                    left join (select 物料编码, sum(库存总数)库存总数,MAX(受订量) 受订量,MAX(在制量)在制量,max(未领量)未领量,max(在途量) as 在途量  from 仓库物料数量表
                 where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 = '仓库类别' and 布尔字段2 = 1) group by 物料编码)kc on kc.物料编码=base.物料编码 ";

            dt_库存 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            dt_AddInv = new DataTable();
            dt_AddInv.Columns.Add("物料编码");
            dt_AddInv.Columns.Add("规格型号");
            dt_AddInv.Columns.Add("物料名称");
            dt_AddInv.Columns.Add("存货分类");
            dt_AddInv.Columns.Add("数量", typeof(decimal));
            dt_AddInv.Columns.Add("编号", typeof(decimal));
            dt_AddInv2 = dt_AddInv.Clone();

            gridControl2.DataSource = dt_AddInv;
            gridControl3.DataSource = dt_AddInv2;

            ///-- 2020-5-27 增加 



            // s = string.Format("select  sum(制令数量)其他制令数  from 生产记录生产制令表" +
            //    " where  关闭=0 and 完成=0 and 物料编码='{0}' and 生产制令单号<>'{1}' group by 物料编码 ", drM["物料编码"], textBox1.Text.Trim());
            //DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //textBox2.Text = "0";
            //if (dt.Rows.Count > 0)
            //{
            //    textBox2.Text = dt.Rows[0]["其他制令数"].ToString();
            //}
        }
        private void fun_calu()
        {
            string s = @"select zl.物料编码,子项编码,SUM(制令数量 * bom.数量)总需求数量,WIPType from 生产记录生产制令表 zl
             left join 基础数据物料BOM表 bom  on zl.物料编码 = bom.产品编码
             where 关闭 = 0 and 完成=0 and 子项编码 is not null /*and WIPType<>'入库倒冲'*/ 
             and 生产制令类型<>'返修制令'   group by zl.物料编码,子项编码,WIPType";
            DataTable t1 = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            for (int i = t1.Rows.Count - 1; i >= 0; i--)
            {
                if (t1.Rows[i]["WIPType"].ToString() == "虚拟")
                {

                    //DataRow[] r = bom.Select(string.Format("产品编码='{0}'", t1.Rows[i]["子项编码"]));
                    //foreach (DataRow xn in r)
                    //{
                    //    DataRow rr = t1.NewRow();
                    //    rr["物料编码"] = t1.Rows[i]["物料编码"]; //产品编码
                    //    rr["子项编码"] = xn["子项编码"];
                    //    rr["总需求数量"] = Convert.ToDecimal(t1.Rows[i]["总需求数量"]) * Convert.ToDecimal(xn["数量"]);
                    //    rr["WIPType"] = "虚拟件子件";
                    //    t1.Rows.Add(rr);
                    //}
                    xn_zj(t1.Rows[i]["子项编码"].ToString(), t1, i, ""); //这里不需要编号 随便给一个
                    t1.Rows[i].Delete();
                }
            }

            MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
            dt_总需 = RBQ.SelectGroupByInto("", t1, "子项编码,sum(总需求数量) 总需求数量", "", "子项编码");

            s = @" select mx.物料编码,SUM(领料数量) as 总已领数量 from 生产记录生产领料单明细表 mx
                 left join 生产记录生产领料单主表 zb on zb.领料出库单号=mx.领料出库单号
            where mx.生产制令单号 in (select  生产制令单号 from 生产记录生产制令表 where 关闭 = 0 and 完成 = 0  and 生产制令类型<>'返修制令')
          and 领料类型<>'生产补料'    group by mx.物料编码";
            DataTable t_已领 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            dt_总需.Columns.Add("总已领数量", typeof(decimal));
            //dt_总需.Columns.Add("还需数量", typeof(decimal));
            foreach (DataRow dr in dt_总需.Rows)
            {
                DataRow[] r = t_已领.Select(string.Format("物料编码='{0}'", dr["子项编码"]));
                if (r.Length > 0)
                {
                    dr["总已领数量"] = r[0]["总已领数量"];
                }
                else
                {
                    dr["总已领数量"] = 0;
                }
            }
        }

        DataTable dtM;

        private void cal20_5_27()
        {
            try
            {
                DataTable dt_SaleOrder = dt_AddInv2.Copy();
                ERPorg.Corg.result_主计划 rs = new ERPorg.Corg.result_主计划();
                rs = ERPorg.Corg.fun_pool_all(dt_SaleOrder);
                dtM = rs.dtM_采购池;
                DataTable dt_sc = rs.dtM ;
                DataColumn dc = new DataColumn("选择", typeof(bool));
                dc.DefaultValue = false;
                dtM.Columns.Add(dc);
                DataTable dt_bom = rs.Bom;
                DataTable dt_totalcount = rs.TotalCount;


                DataTable dt_SaleCrderCopy = dt_SaleOrder.Copy();

                ////19-10-10 两种版本二合一
                //dtM.Columns.Add("总需求", typeof(decimal));

                //DataTable dtM_总需求 = new DataTable();

                //dtM_总需求.Columns.Add("物料编码");
                //dtM_总需求.Columns.Add("物料名称");
                //dtM_总需求.Columns.Add("规格型号");
                //dtM_总需求.Columns.Add("存货分类");
                //dtM_总需求.Columns.Add("需求数量", typeof(decimal));
                //dtM_总需求.Columns.Add("自制", typeof(bool));
                //dtM_总需求.Columns.Add("可购", typeof(bool));
                //dtM_总需求.Columns.Add("未领量", typeof(decimal));
                //dtM_总需求.Columns.Add("库存总数", typeof(decimal));
                //dtM_总需求.Columns.Add("受订量", typeof(decimal));
                //dtM_总需求.Columns.Add("在制量", typeof(decimal));
                //dtM_总需求.Columns.Add("在途量", typeof(decimal));
                ////这里的需求数量、总需求是界面上的订单用量 是界面上的
                //foreach (DataRow dr in dt_SaleOrder.Rows)
                //{
                //    DataTable dt_x = new DataTable();
                //    dt_x = ERPorg.Corg.billofM_带数量(dt_x, dr["物料编码"].ToString(), false);
                //    foreach (DataRow rr in dt_x.Rows)
                //    {
                //        decimal dec = Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(rr["数量"]);
                //        DataRow[] rrr = dtM_总需求.Select(string.Format("物料编码='{0}'", rr["子项编码"]));
                //        if (rrr.Length == 0)
                //        {
                //            DataRow tr = dtM_总需求.NewRow();
                //            tr["物料编码"] = rr["子项编码"];
                //            DataRow[] base_info = dt_totalcount.Select(string.Format("物料编码='{0}'", tr["物料编码"]));
                //            tr["物料名称"] = base_info[0]["物料名称"];
                //            tr["规格型号"] = base_info[0]["规格型号"];
                //            tr["存货分类"] = base_info[0]["存货分类"];
                //            tr["自制"] = base_info[0]["自制"];
                //            tr["可购"] = base_info[0]["可购"];
                //            tr["库存总数"] = base_info[0]["库存总数"];
                //            tr["受订量"] = base_info[0]["受订量"];
                //            tr["在制量"] = base_info[0]["在制量"];
                //            tr["未领量"] = base_info[0]["未领量"];
                //            tr["在途量"] = base_info[0]["在途量"];
                //            tr["需求数量"] = dec;
                //            dtM_总需求.Rows.Add(tr);
                //        }
                //        else
                //        {
                //            rrr[0]["需求数量"] = dec + Convert.ToDecimal(rrr[0]["需求数量"]);
                //        }
                //    }

                //}

                //foreach (DataRow dr in dtM_总需求.Rows)
                //{
                //    DataRow[] yy = dtM.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                //    if (yy.Length == 0)
                //    {
                //        DataRow r_need = dtM.NewRow();
                //        //r_need["在制量"] = dr["在制量"];
                //        r_need["物料编码"] = dr["物料编码"];
                //        r_need["在途量"] = dr["在途量"];
                //        r_need["未领量"] = dr["未领量"];
                //        r_need["物料名称"] = dr["物料名称"];
                //        r_need["规格型号"] = dr["规格型号"];
                //        r_need["存货分类"] = dr["存货分类"];
                //        r_need["库存总数"] = dr["库存总数"];
                //        r_need["受订量"] = dr["受订量"];
                //        r_need["自制"] = dr["自制"];
                //        //r_need["工时"] = dr["工时"];
                //        r_need["总需求"] = dr["需求数量"];
                //        dtM.Rows.Add(r_need);
                //    }
                //    else
                //    {
                //        yy[0]["总需求"] = dr["需求数量"];
                //    }
                //}
                //20-6-2 
                foreach (DataRow dr in dt_totalcount.Rows)
                {
                    if(Convert.ToDecimal( dr["订单用量"])>0)
                    {
                        DataRow []p= dtM.Select($"物料编码='{dr["物料编码"].ToString()}'");
                        if (p.Length > 0) continue;
                        
                         
                        DataRow r_need = dtM.NewRow();
                         r_need["可购"] = dr["可购"];
                        r_need["参考数量"] =0;
                        r_need["库存下限"] = dr["库存下限"];
                        r_need["委外在途"] = dr["委外在途"];
                        r_need["已采未审"] = dr["已采未审"];
                        r_need["采购未送检"] = dr["采购未送检"];
                        r_need["已送未检"] = dr["已送未检"];
                        r_need["已检未入"] = dr["已检未入"];
                        r_need["采购员"] = dr["采购员"];
                        r_need["默认供应商"] = dr["默认供应商"];

                        r_need["物料编码"] = dr["物料编码"];
                        r_need["在途量"] = dr["在途量"];
                        r_need["未领量"] = dr["未领量"];
                        r_need["物料名称"] = dr["物料名称"];
                        r_need["规格型号"] = dr["规格型号"];
                        r_need["存货分类"] = dr["存货分类"];
                        r_need["库存总数"] = dr["库存总数"];
                        r_need["受订量"] = dr["受订量"];
                        r_need["自制"] = dr["自制"];
                        //r_need["工时"] = dr["工时"];
                        r_need["订单用量"] = dr["订单用量"];
                        dtM.Rows.Add(r_need);
                    }
                }
 
                dtM.Columns.Add("最早到货日期", typeof(DateTime));
                dtM.Columns.Add("最早到货数量", typeof(decimal));
                dtM.Columns.Add("在制量", typeof(decimal));
                //20-5-28 计划开会需要增加 采购未送检的 最早到货日期 和 最早到货数量 
                string zz = @"select 物料编码,case when (SUM(采购数量-已送检数)>0) then sum(采购数量-已送检数) else 0 end as 最早到货数量,MIN(预计到货日期)最早到货日期 from 采购记录采购单明细表 a
                left join 采购记录采购单主表 b on b.采购单号 = a.采购单号
                where a.生效 = 1 and 明细完成 = 0 and a.作废 = 0  and b.作废 = 0 and 总完成 = 0 and a.生效日期 > '2017-12-1' group by 物料编码";
                DataTable t_zzd = CZMaster.MasterSQL.Get_DataTable(zz, strcon);
                foreach (DataRow dr in dtM.Rows)
                {
                    DataRow []yy= dt_sc.Select($"物料编码='{dr["物料编码"].ToString()}'");
                    if(yy.Length>0)
                    {
                        dr["参考数量"] = yy[0]["参考数量"];
                    }
                    DataRow []xtr= dt_totalcount.Select($"物料编码='{dr["物料编码"].ToString()}'");
                    dr["在制量"] = xtr[0]["在制量"];
                    DataRow[] r = t_zzd.Select($"物料编码='{dr["物料编码"].ToString()}'");
                    if (r.Length > 0)
                    {
                        dr["最早到货日期"] = r[0]["最早到货日期"];
                        dr["最早到货数量"] = r[0]["最早到货数量"];

                    }

                }



                bl_calculate = false;
                BeginInvoke(new MethodInvoker(() =>
                {
                    if (rs.str_log != "")
                    {
                        barStaticItem1.Caption = rs.str_log;
                    }
                    else
                    {
                        barStaticItem1.Caption = "";
                    }
                    DataView dv = new DataView(dtM);
                    dv.RowFilter = "订单用量>0 ";
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
                    barStaticItem1.Caption = "错误原因:" + ex.Message;
                    bl_calculate = false;
                }));
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = null;
                if (tabControl1.SelectedTab.Name == "tabPage1")
                {
                    foreach (DataRow dr in dt_AddInv.Rows)
                    {
                        if (dr["数量"].ToString() == "") throw new Exception("数量未输入");
                    }
                    if (bl_calculate) throw new Exception("正在计算，请稍候");

                    Thread th = new Thread(() =>
                    {
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            barStaticItem1.Caption = "正在计算中请稍候";

                        }));
                        dt_缺料情况表 = new DataTable();
                        foreach (DataRow dr in dt_AddInv.Rows)
                        {
                            dt_缺料情况表 = fun_1(dt_缺料情况表, dr);
                        }


                        bl_calculate = false;
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            gridControl1.DataSource = dt_缺料情况表;
                            barStaticItem1.Caption = "";

                        }));

                    });
                    th.IsBackground = true;
                    th.Start();
                    bl_calculate = true;
                }
                else
                {
                    if (dt_AddInv2.Rows.Count == 0) throw new Exception("没有数据可计算");
                    foreach (DataRow row in dt_AddInv2.Rows)
                    {
                        if (row["数量"].ToString() == "") throw new Exception("数量未输入");
                    }
                    if (bl_calculate) throw new Exception("正在计算中..");

                    Thread th = new Thread(cal20_5_27);
                    th.IsBackground = true;
                    th.Start();
                    bl_calculate = true;
                    barStaticItem1.Caption = "正在计算中..";
                }
            }
            catch (Exception ex)
            {
                bl_calculate = false;
                MessageBox.Show(ex.Message);
            }


        }

        //2020-5-6
        private void xn_zj(string str_wl, DataTable t, int i, string str_编号)
        {
            DataRow[] r = bom.Select(string.Format("产品编码='{0}'", str_wl));

            foreach (DataRow xn in r)
            {
                if (xn["WIPType"].ToString() == "虚拟") xn_zj(xn["子项编码"].ToString(), t, i, str_编号);
                else
                {
                    DataRow rr = t.NewRow();
                    if (str_编号 != "") //20-5-6
                    {
                        rr["编号"] = str_编号;
                        rr["此单需求数量"] = Convert.ToDecimal(t.Rows[i]["此单需求数量"]) * Convert.ToDecimal(xn["数量"]);
                        rr["bom数量"] = Convert.ToDecimal(xn["数量"]) * Convert.ToDecimal(t.Rows[i]["bom数量"]);
                        //2020-4-15
                        rr["组"] = xn["组"];
                        rr["优先级"] = xn["优先级"];
                    }
                    else
                    {
                        rr["总需求数量"] = Convert.ToDecimal(t.Rows[i]["总需求数量"]) * Convert.ToDecimal(xn["数量"]);

                    }
                    //rr["子项编码"] = t.Rows[i]["子项编码"]; //产品编码
                    rr["子项编码"] = xn["子项编码"];
                    rr["WIPType"] = "虚拟件子件";

                    t.Rows.Add(rr);
                }
            }
        }

        private DataTable fun_1(DataTable dt, DataRow drr)
        {
            DataTable t_单个制令 = new DataTable();

            string s1 = string.Format(@"  select  '{2}' as 编号 , a.*,ISNULL(已检未入数,0)已检未入数,ISNULL(已送未检数,0)已送未检数,isnull(采购未送检,0)采购未送检 from (
  select 子项编码,SUM({0}*bom.数量)此单需求数量,WIPType,bom.数量 as bom数量,bom.组,bom.优先级 from 基础数据物料信息表 zl
            left join 基础数据物料BOM表 bom  on zl.物料编码=bom.产品编码 
            where 关闭=0  and 子项编码 is not null and zl.物料编码='{1}' /*and WIPType<>'入库倒冲'*/  group by 子项编码,WIPType,bom.数量,bom.组,bom.优先级)a
             left join (  select  物料编码,sum(合格数量-已入库数量)已检未入数 from 生产记录生产检验单主表  
                      where 检验日期>'2019-5-5' and 完成=0 group by 物料编码  union 
                      select  产品编号 as 物料编码,SUM(送检数量-已入库数-不合格数量)已检未入数 from 采购记录采购单检验主表
                     where 入库完成 =0 and 关闭=0 and 检验结果 in ('合格' ,'免检')  and 完成=0  group by 产品编号  )b on a.子项编码=b.物料编码 
            left join (select  物料编码,sum(送检数量-已检验数)已送未检数  from 采购记录采购送检单明细表 where 检验完成=0 and 作废=0 and 送检数量>0 group by 物料编码
                      union select  物料编码,sum(未检验数量)已送未检数 from 生产记录生产工单表 where   关闭=0 and 完工=1 and 检验完成=0 group by 物料编码 ) x on x.物料编码=a.子项编码
           left join (select  物料编码,case when (sum(采购数量-已送检数) > 0) then sum(采购数量 - 已送检数) else 0 end as 采购未送检  from 采购记录采购单明细表 
               where  生效 =1 and  明细完成日期 is null and 作废 = 0  and 总完成 = 0  and 生效日期 > '2017-12-1' group by 物料编码)c on c.物料编码=a.子项编码 ", drr["数量"], drr["物料编码"], drr["编号"]);
            t_单个制令 = CZMaster.MasterSQL.Get_DataTable(s1, strcon);

            for (int i = t_单个制令.Rows.Count - 1; i >= 0; i--)
            {
                if (t_单个制令.Rows[i]["WIPType"].ToString() == "虚拟")
                {
                    string str_子项编码 = t_单个制令.Rows[i]["子项编码"].ToString();

                    xn_zj(str_子项编码, t_单个制令, i, drr["编号"].ToString());
                    t_单个制令.Rows[i].Delete();
                }
            }

            string s2 = string.Format(@"select mx.物料编码,SUM(领料数量) as 已领数量 from 生产记录生产领料单明细表 mx
            where mx.生产制令单号='{0}'  group by mx.物料编码", "");
            DataTable t_单个制令已领 = CZMaster.MasterSQL.Get_DataTable(s2, strcon);
            t_单个制令.Columns.Add("此单已领数量", typeof(decimal));
            t_单个制令.Columns.Add("总需数量", typeof(decimal));
            t_单个制令.Columns.Add("库存总数", typeof(decimal));
            t_单个制令.Columns.Add("物料名称");
            t_单个制令.Columns.Add("规格型号");
            t_单个制令.Columns.Add("在制量", typeof(decimal));
            t_单个制令.Columns.Add("总已领量", typeof(decimal));
            t_单个制令.Columns.Add("其他占用量", typeof(decimal));

            t_单个制令.Columns.Add("此单剩余需求", typeof(decimal));


            foreach (DataRow dr in t_单个制令.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                dr["此单已领数量"] = 0;

                DataRow[] r_t = dt_总需.Select(string.Format("子项编码='{0}'", dr["子项编码"]));
                if (r_t.Length > 0)
                {
                    dr["总需数量"] = r_t[0]["总需求数量"];
                    dr["总已领量"] = r_t[0]["总已领数量"];
                }
                else
                {
                    dr["总需数量"] = 0;
                    dr["总已领量"] = 0;
                }

                DataRow[] r_kc = dt_库存.Select(string.Format("物料编码='{0}'", dr["子项编码"]));
                if (r_kc.Length > 0)
                {
                    dr["物料名称"] = r_kc[0]["物料名称"];
                    dr["规格型号"] = r_kc[0]["规格型号"];
                    dr["在制量"] = r_kc[0]["在制量"];
                    dr["库存总数"] = r_kc[0]["库存总数"];

                }
                dr["此单剩余需求"] = Convert.ToDecimal(dr["此单需求数量"]);
                dr["其他占用量"] = Convert.ToDecimal(dr["总需数量"]) - Convert.ToDecimal(dr["总已领量"]);
            }
            foreach (DataRow xr in t_单个制令.Rows)
            {
                if (xr.RowState == DataRowState.Deleted) continue;
                if (xr["已检未入数"] == null || xr["已检未入数"].ToString() == "")
                {
                    string s = string.Format(@"select* from(select 物料编码, sum(合格数量-已入库数量)已检未入数 from 生产记录生产检验单主表
                    where 检验日期 > '2019-5-5' and 完成 = 0 group by 物料编码
                    union
                    select 产品编号 as 物料编码,SUM(送检数量-已入库数-不合格数量 )已检未入数 from 采购记录采购单检验主表
                     where 入库完成 = 0 and 关闭 = 0 and 检验结果 in ('合格' ,'免检')  and 完成 = 0   group by 产品编号)a  where 物料编码 = '{0}'", xr["子项编码"]);
                    DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    if (temp.Rows.Count == 0) xr["已检未入数"] = 0;
                    else xr["已检未入数"] = temp.Rows[0]["已检未入数"];

                    s = string.Format(@"select  * from (  
            select  物料编码,sum(送检数量-已检验数)已送未检数  from 采购记录采购送检单明细表 where 检验完成=0 and 作废=0 and 送检数量>0 group by 物料编码
                union select  物料编码,sum(未检验数量)已送未检数 from 生产记录生产工单表 where 完工=1 and 检验完成=0 group by 物料编码)a
                where 物料编码='{0}'", xr["子项编码"]);
                    temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    if (temp.Rows.Count == 0) xr["已送未检数"] = 0;
                    else xr["已送未检数"] = temp.Rows[0]["已送未检数"];

                    s = $@"select  物料编码,case when (sum(采购数量-已送检数) > 0) then sum(采购数量 - 已送检数) else 0 end as 采购未送检  from 采购记录采购单明细表 
               where 生效 = 1 and 明细完成日期 is null and 作废 = 0  and 总完成 = 0  and 生效日期 > '2017-12-1' and 物料编码='{xr["子项编码"].ToString()}' group by 物料编码";
                    temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    if (temp.Rows.Count == 0) xr["采购未送检"] = 0;
                    else xr["采购未送检"] = temp.Rows[0]["采购未送检"];

                }
            }
            t_单个制令.Columns.Add("此单可用", typeof(decimal));
            foreach (DataRow xr in t_单个制令.Rows)
            {
                if (xr.RowState == DataRowState.Deleted) continue;
                decimal dec = Convert.ToDecimal(xr["库存总数"]) + Convert.ToDecimal(xr["在制量"]) - Convert.ToDecimal(xr["其他占用量"])
               + Convert.ToDecimal(xr["已检未入数"]); // + Convert.ToDecimal(dr["已送未检数"])
                xr["此单可用"] = dec > 0 ? dec : 0;
            }



            //虚拟件没有送检未入数 和 已检未入库数
            if (dt == null || dt.Columns.Count == 0)
            {
                dt = t_单个制令;

            }
            else
            {
                dt.Merge(t_单个制令);

            }
            return dt;

        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ERPorg.Corg.FlushMemory();
                if (bl_calculate) throw new Exception("正在计算，请稍候");
                ui自定义查看料况 ui = new ui自定义查看料况();
                CPublic.UIcontrol.Showpage(ui, "自定义查看料况");

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
                if (bl_calculate) throw new Exception("正在计算，请稍候");
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                    gridControl1.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (bl_calculate) throw new Exception("正在计算，请稍候");
                CPublic.UIcontrol.ClosePage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void infolink(DataTable dt_AddInv)
        {
            try
            {



                int x = 1;
                foreach (DataRow dr in dt_AddInv.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted) continue;

                    DataRow[] r = dt_库存.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (r.Length > 0)
                    {
                        dr["编号"] = x++;
                        dr["物料名称"] = r[0]["物料名称"].ToString();
                        dr["规格型号"] = r[0]["规格型号"].ToString();
                        dr["存货分类"] = r[0]["存货分类"].ToString();
                    }


                }
                for (int i = dt_AddInv.Rows.Count - 1; i >= 0; i--)
                {
                    if (dt_AddInv.Rows[i]["编号"].ToString() == "") dt_AddInv.Rows.RemoveAt(i);
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


        }

        private void gridView2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.V)
                {
                    if (gridView2.FocusedColumn.Caption == "物料编码") infolink(dt_AddInv);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void ui自定义查看料况_Load(object sender, EventArgs e)
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg pz = new ERPorg.Corg();
                pz.UserLayout(this.splitContainer1, this.Name, cfgfilepath);
                pz.UserLayout(this.splitContainer2, this.Name, cfgfilepath);

                fun_load();
                fun_calu();


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void gridView3_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.V)
                {
                    if (gridView3.FocusedColumn.Caption == "物料编码") infolink(dt_AddInv2);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void gv2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Clicks == 2)
                {
                    DataRow r = gv2.GetDataRow(gv2.FocusedRowHandle);
                    if (r == null) return;
                    Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPStock.dll")));  //  ERPproduct.dll
                    Type outerForm = outerAsm.GetType("ERPStock.frm仓库物料数量明细", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统

                    object[] drr = new object[2];
                    drr[0] = r["物料编码"].ToString();
                    drr[1] = r["仓库号"].ToString();
                    UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;
                    string name = string.Format("物料明细({0}_{1})", r["物料编码"].ToString().Trim(), r["物料名称"].ToString().Trim());
                    CPublic.UIcontrol.AddNewPage(ui, name);
                    ui.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
