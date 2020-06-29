using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Threading;

namespace ReworkMould
{
    public partial class ui主计划生成单 : UserControl
    {

        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";

        /// <summary>
        /// 判断是保存后关闭还是直接关闭
        /// </summary>
        public bool bl = false;
        bool bl_c=false;//是否正在计算
        DataTable dtM;
        DataTable dt_主计划主;
        DataTable dt_主计划子;
        DataTable dt_生产计划;
        DataTable dt_采购计划;
        DataTable dt_SaleOrder;
        DataTable dt_total;
        DataTable dt_bom;
        string str_log;
        bool bl_生产 = false;
        bool bl_采购 = false;
        string s_主计划单号;
        public ui主计划生成单()
        {
            InitializeComponent();
        }

        public ui主计划生成单(DataTable t)
        {
            InitializeComponent();
            dtM = t; 
        }
        //2020-6-3
        private void fun_qx()
        {
            if(!CPublic.Var.LocalUserTeam.Contains("主计划") &&  CPublic.Var.localUserName!="admin")
            {
                barLargeButtonItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    barLargeButtonItem7.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            }
        }


        private void ui主计划生产单_Load(object sender, EventArgs e)
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
                x.UserLayout(panel2, this.Name, cfgfilepath);
                fun_qx();
                fun_load();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_load()
        {
            dtM.Columns.Add("主计划单号", typeof(string));
            dtM.Columns.Add("主计划备注",typeof(string));
            dtM.Columns.Add("可转数量",typeof(decimal));
          //  dtM.Columns.Add("预计开工日期", typeof(DateTime));
            dtM.Columns.Add("预计耗时", typeof(decimal));

            foreach (DataRow dr in dtM.Rows)
            {
                dr["可转数量"] = Convert.ToDecimal(dr["未完成数量"].ToString()) - Convert.ToDecimal(dr["已转数量"].ToString());
                dr["预计耗时"] =Math.Round(Convert.ToDecimal(dr["可转数量"].ToString()) * Convert.ToDecimal(dr["工时"].ToString()),2,MidpointRounding.AwayFromZero);

            }
            gridControl1.DataSource = dtM;
       }

        #region  暂不用 
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {


       
            //try
            //{
            //    if (bl_c) throw new Exception("计算尚未完成");

            //    gridView1.CloseEditor();
            //    gridView1.UpdateCurrentRow();
            //    fun_check();
            //    if (bl_生产 == false)
            //    {
            //        throw new Exception("请先计算");
            //    }
                
            //    fun_save();
            //    bl = true;
            //    bl_生产 = false;
            //    bl_采购 = false;
            //    //barLargeButtonItem4.Enabled = false;
            //    //barLargeButtonItem6.Enabled = false;
            //    this.ParentForm.Close();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        //private void fun_save()
        //{
        //    DateTime t = CPublic.Var.getDatetime();
        //    //string s_主计划单号 = string.Format("MS{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day,
        //    //              CPublic.CNo.fun_得到最大流水号("MS", t.Year, t.Month));
        //    string sql_主 = "select * from  主计划主表 where 1<>1";
        //    dt_主计划主 = CZMaster.MasterSQL.Get_DataTable(sql_主, strconn);
        //    string sql_子 = "select * from  主计划子表 where 1<>1";
        //    dt_主计划子 = CZMaster.MasterSQL.Get_DataTable(sql_子, strconn);
        //    string sqll_xm = "select * from 销售记录销售订单明细表 where 1<>1";
        //    DataTable dt_xm = CZMaster.MasterSQL.Get_DataTable(sqll_xm, strconn);

        //    string sql_ym = "select * from 销售预订单明细表  where 1<>1";
        //    DataTable dt_ym = CZMaster.MasterSQL.Get_DataTable(sql_ym, strconn);

        //    string sql_jm = "select * from 借还申请表附表  where 1<>1";
        //    DataTable dt_jm = CZMaster.MasterSQL.Get_DataTable(sql_jm, strconn);

        //    string sql_xp = "select * from 生产记录生产制令表  where 1<>1";
        //    DataTable dt_xp = CZMaster.MasterSQL.Get_DataTable(sql_xp, strconn);



        //    int i = 1;
        //    DataRow dr_主计划主 = dt_主计划主.NewRow();
        //    dt_主计划主.Rows.Add(dr_主计划主);
        //    dr_主计划主["主计划单号"] = s_主计划单号;
        //    dr_主计划主["GUID"] = System.Guid.NewGuid();
        //    dr_主计划主["制单人"] = CPublic.Var.localUserName;
        //    dr_主计划主["制单人ID"] = CPublic.Var.LocalUserID;
        //    dr_主计划主["制单日期"] = t;
        //    dr_主计划主["生效"] = true;
        //    foreach(DataRow dr in dtM.Rows)
        //    {
        //        DataRow dr_主计划子 = dt_主计划子.NewRow();
        //        dt_主计划子.Rows.Add(dr_主计划子);
        //        dr_主计划子["主计划单号"] = s_主计划单号;
        //        dr_主计划子["主计划明细号"] = s_主计划单号+"-"+i.ToString("00");
        //        dr_主计划子["关联订单号"] = dr["销售订单号"].ToString();
        //        dr_主计划子["关联订单明细号"] = dr["销售订单明细号"].ToString();
        //        dr_主计划子["物料编码"] = dr["物料编码"].ToString();
        //        dr_主计划子["物料名称"] = dr["物料名称"].ToString();
        //        dr_主计划子["规格型号"] = dr["规格型号"].ToString();
        //        dr_主计划子["客户名称"] = dr["客户名称"].ToString();
        //        dr_主计划子["目标客户"] = dr["目标客户"].ToString();
        //        dr_主计划子["生效日期"] = dr["生效日期"].ToString();
        //        dr_主计划子["下单日期"] = dr["下单日期"].ToString();
        //        dr_主计划子["订单制单人"] = dr["制单人"].ToString();
        //        dr_主计划子["预计发货日期"] = dr["预计发货日期"].ToString();
        //     //   dr_主计划子["预计开工日期"] = dr["预计开工日期"].ToString();
        //        dr_主计划子["表头备注"] = dr["表头备注"].ToString();
        //        dr_主计划子["备注"] = dr["备注"].ToString();
        //        dr_主计划子["POS"] =i++;
        //        dr_主计划子["主计划备注"] = dr["主计划备注"].ToString();
        //        dr_主计划子["存货分类"] = dr["存货分类"].ToString();
        //        dr_主计划子["主计划转单日期"] = t;
        //        dr_主计划子["此次转单数量"] = Convert.ToDecimal(dr["可转数量"].ToString());
        //        dr_主计划子["销售数量"] = Convert.ToDecimal(dr["数量"].ToString());
        //        //dr_主计划子["库存总数"] = Convert.ToDecimal(dr["库存总数"].ToString());
        //       // dr_主计划子["在途量"] = Convert.ToDecimal(dr["在途量"].ToString());
        //        //dr_主计划子["在制量"] = Convert.ToDecimal(dr["在制量"].ToString());
        //       // dr_主计划子["未领量"] = Convert.ToDecimal(dr["未领量"].ToString());
        //        dr_主计划子["转单未完成数量"] = Convert.ToDecimal(dr_主计划子["此次转单数量"].ToString());
        //        dr_主计划子["数量"] = Convert.ToDecimal(dr_主计划子["此次转单数量"].ToString());
        //        sqll_xm = string.Format("select * from 销售记录销售订单明细表 where 销售订单明细号 = '{0}'", dr["销售订单明细号"]);
        //        SqlDataAdapter da = new SqlDataAdapter(sqll_xm, strconn);
        //        da.Fill(dt_xm);


        //        sql_ym = string.Format("select * from 销售预订单明细表 where 销售预订单明细号 = '{0}'", dr["销售订单明细号"]);
        //        da = new SqlDataAdapter(sql_ym, strconn);
        //        da.Fill(dt_ym);



        //        sql_jm = string.Format("select * from 借还申请表附表 where 申请批号明细 = '{0}'", dr["销售订单明细号"]);
        //        da = new SqlDataAdapter(sql_jm, strconn);
        //        da.Fill(dt_jm);

        //        sql_xp = string.Format("select * from 生产记录生产制令表 where 生产制令单号 = '{0}'", dr["销售订单明细号"]);
        //        da = new SqlDataAdapter(sql_xp, strconn);
        //        da.Fill(dt_xp);

        //        DataRow[] dr11 = dt_xm.Select(string.Format("销售订单明细号 = '{0}'", dr["销售订单明细号"]));
        //        DataRow[] dr22 = dt_ym.Select(string.Format("销售预订单明细号 = '{0}'", dr["销售订单明细号"]));
        //        DataRow[] dr33 = dt_jm.Select(string.Format("申请批号明细 = '{0}'", dr["销售订单明细号"]));
        //        DataRow[] dr44 = dt_xp.Select(string.Format("生产制令单号 = '{0}'", dr["销售订单明细号"]));



        //        if (dr11.Length > 0)
        //        {
        //            dr11[0]["已转数量"] = Convert.ToDecimal(dr11[0]["已转数量"]) + Convert.ToDecimal(dr_主计划子["此次转单数量"]);
        //        }

        //        if (dr22.Length > 0)
        //        {
        //            dr22[0]["已转数量"] = Convert.ToDecimal(dr22[0]["已转数量"]) + Convert.ToDecimal(dr_主计划子["此次转单数量"]);
        //        }

        //        if (dr33.Length > 0)
        //        {
        //            dr33[0]["已转数量"] = Convert.ToDecimal(dr33[0]["已转数量"]) + Convert.ToDecimal(dr_主计划子["此次转单数量"]);
        //        }
        //        if (dr44.Length > 0)
        //        {
        //            dr44[0]["已转数量"] = Convert.ToDecimal(dr44[0]["已转数量"]) + Convert.ToDecimal(dr_主计划子["此次转单数量"]);
        //        }






        //    }

        //    string sql11 = "select* from  生产计划主表 where 1 <> 1";
        //    DataTable dt_生产计划主 = CZMaster.MasterSQL.Get_DataTable(sql11, strconn);
        //    sql11 = "select* from  生产计划明细表 where 1 <> 1";
        //    DataTable dt_生产计划明细 = CZMaster.MasterSQL.Get_DataTable(sql11, strconn);
        //    string s_单号 = "";
        //    int iii = 1;
        //    if (dt_生产计划.Rows.Count > 0)
        //    {
        //        s_单号 = string.Format("PS{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("PS", t.Year, t.Month, t.Day));
        //        DataRow dr_cgz = dt_生产计划主.NewRow();
        //        dt_生产计划主.Rows.Add(dr_cgz);
        //        dr_cgz["GUID"] = System.Guid.NewGuid();
        //        dr_cgz["生产计划单号"] = s_单号;
        //        dr_cgz["生效人"] = CPublic.Var.localUserName;
        //        dr_cgz["生效人ID"] = CPublic.Var.LocalUserID;
        //        dr_cgz["生效日期"] = t;
        //        foreach (DataRow dr_mx in dt_生产计划.Rows)
        //        {
        //            if (Convert.ToBoolean(dr_mx["自制"]) == true)
        //            {
        //                DataRow dr_scmx = dt_生产计划明细.NewRow();
        //                dt_生产计划明细.Rows.Add(dr_scmx);
        //                dr_scmx["生产计划单号"] = s_单号;
        //                dr_scmx["生产计划明细号"] = dr_scmx["生产计划单号"] + "-" + iii.ToString("0000");
        //                dr_scmx["POS"] = iii++;
        //                dr_scmx["物料编码"] = dr_mx["物料编码"];
        //                dr_scmx["存货分类"] = dr_mx["存货分类"];
        //                dr_scmx["物料名称"] = dr_mx["物料名称"];
        //                dr_scmx["规格型号"] = dr_mx["规格型号"];
        //                dr_scmx["最早发货日期"] = dr_mx["最早发货日期"];
        //                dr_scmx["需求数量"] = Convert.ToDecimal(dr_mx["需求数量"]);
        //                dr_scmx["参考数量"] = Convert.ToDecimal(dr_mx["参考数量"]);
        //                dr_scmx["仓库号"] = dr_mx["仓库号"];
        //                dr_scmx["仓库名称"] = dr_mx["仓库名称"];
        //                dr_scmx["已转数量"] = 0; 
        //                dr_scmx["未转数量"] = Convert.ToDecimal(dr_mx["参考数量"]);
        //                dr_scmx["主计划单号"] = s_主计划单号;
        //                if (dr_mx["订单用量"] != DBNull.Value)
        //                {
        //                    dr_scmx["订单用量"] = Convert.ToDecimal(dr_mx["订单用量"]);
        //                }
        //                //if (dr_mx["最早预计开工日期"].ToString() != "")
        //                //{
        //                //    dr_scmx["最早预计开工日期"] = dr_mx["最早预计开工日期"];
        //                //}
        //            }

        //        }

        //    }
        //    else
        //    {
        //        throw new Exception("没有可保存的数据");
        //    }




        //    string sql_1 = "select* from  采购计划主表 where 1 <> 1";
        //    DataTable dt_采购计划主 = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);
        //    sql_1 = "select* from  采购计划明细表 where 1 <> 1";
        //    DataTable dt_采购计划明细 = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);
        //    string s_单号_1 = "";
        //    int i_1 = 1;
        //    if (dt_采购计划.Rows.Count > 0)
        //    {
        //        s_单号_1 = string.Format("PP{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("PP", t.Year, t.Month, t.Day));
        //        DataRow dr_cgz = dt_采购计划主.NewRow();
        //        dt_采购计划主.Rows.Add(dr_cgz);
        //        dr_cgz["GUID"] = System.Guid.NewGuid();
        //        dr_cgz["采购计划单号"] = s_单号_1;
        //        dr_cgz["生效人"] = CPublic.Var.localUserName;
        //        dr_cgz["生效人ID"] = CPublic.Var.LocalUserID;
        //        dr_cgz["生效日期"] = t;

        //        foreach (DataRow dr_mx in dt_采购计划.Rows)
        //        {
        //            if (Convert.ToBoolean(dr_mx["可购"]) == true || Convert.ToBoolean(dr_mx["委外"]) == true)
        //            {
        //                DataRow dr_cgmx = dt_采购计划明细.NewRow();
        //                dt_采购计划明细.Rows.Add(dr_cgmx);
        //                dr_cgmx["采购计划单号"] = s_单号_1;
        //                dr_cgmx["采购计划明细号"] = dr_cgmx["采购计划单号"] + "-" + i_1.ToString("0000");
        //                dr_cgmx["主计划单号"] = s_主计划单号;
        //                dr_cgmx["POS"] = i_1++;
        //                dr_cgmx["物料编码"] = dr_mx["物料编码"];
        //                dr_cgmx["存货分类"] = dr_mx["存货分类"];
        //                dr_cgmx["物料名称"] = dr_mx["物料名称"];
        //                dr_cgmx["规格型号"] = dr_mx["规格型号"];
        //                dr_cgmx["参考数量"] = Convert.ToDecimal(dr_mx["参考数量"]);
        //                dr_cgmx["参考数量(含安全库存)"] = Convert.ToDecimal(dr_mx["参考数量(含安全库存)"]);
        //                dr_cgmx["最早发货日期"] = dr_mx["最早发货日期"];
        //                dr_cgmx["仓库号"] = dr_mx["仓库号"];
        //                dr_cgmx["仓库名称"] = dr_mx["仓库名称"];
        //                dr_cgmx["已转数量"] = 0;
        //                dr_cgmx["未转数量"] = Convert.ToDecimal(dr_mx["参考数量"]);
        //                dr_cgmx["采购周期"] = dr_mx["采购周期"];
        //                dr_cgmx["最小包装"] = Convert.ToDecimal(dr_mx["最小包装"]);
        //                dr_cgmx["需求数量"] = Convert.ToDecimal(dr_mx["需求数量"]);
        //                dr_cgmx["订单用量"] = Convert.ToDecimal(dr_mx["订单用量"]);
        //                //if (dr_mx["最早预计开工日期"].ToString() != "")
        //                //{
        //                //    dr_cgmx["最早预计开工日期"] = dr_mx["最早预计开工日期"];
        //                //}
        //            }


        //        }

        //    }
        //    else
        //    {
        //        throw new Exception("没有可保存的数据");
        //    }






        //    SqlConnection conn = new SqlConnection(strconn);
        //    conn.Open();
        //    SqlTransaction ts = conn.BeginTransaction("主计划单生成");
        //    try
        //    {
        //        string sql = "select * from 主计划主表 where 1<>1";
        //        SqlCommand cmm = new SqlCommand(sql, conn, ts);
        //        SqlDataAdapter da = new SqlDataAdapter(cmm);
        //        new SqlCommandBuilder(da);
        //        da.Update(dt_主计划主);
        //        //主计划明细表
        //        sql = "select * from 主计划子表 where 1<>1";
        //        cmm = new SqlCommand(sql, conn, ts);
        //        da = new SqlDataAdapter(cmm);
        //        new SqlCommandBuilder(da);
        //        da.Update(dt_主计划子);

        //        sql = "select * from 销售记录销售订单明细表 where 1<>1";
        //        cmm = new SqlCommand(sql, conn, ts);
        //        da = new SqlDataAdapter(cmm);
        //        new SqlCommandBuilder(da);
        //        da.Update(dt_xm);

        //        sql = "select * from 销售预订单明细表 where 1<>1";
        //        cmm = new SqlCommand(sql, conn, ts);
        //        da = new SqlDataAdapter(cmm);
        //        new SqlCommandBuilder(da);
        //        da.Update(dt_ym);

        //        sql = "select * from 借还申请表附表 where 1<>1";
        //        cmm = new SqlCommand(sql, conn, ts);
        //        da = new SqlDataAdapter(cmm);
        //        new SqlCommandBuilder(da);
        //        da.Update(dt_jm);

        //        sql = "select * from 生产记录生产制令表 where 1<>1";
        //        cmm = new SqlCommand(sql, conn, ts);
        //        da = new SqlDataAdapter(cmm);
        //        new SqlCommandBuilder(da);
        //        da.Update(dt_xp);

        //        string sql_生产计划主 = "select * from  生产计划主表 where 1<>1";
        //        string sql_生产计划明细 = "select * from  生产计划明细表 where 1<>1";

        //        cmm = new SqlCommand(sql_生产计划主, conn, ts);
        //        da = new SqlDataAdapter(cmm);
        //        new SqlCommandBuilder(da);
        //        da.Update(dt_生产计划主);

        //        cmm = new SqlCommand(sql_生产计划明细, conn, ts);
        //        da = new SqlDataAdapter(cmm);
        //        new SqlCommandBuilder(da);
        //        da.Update(dt_生产计划明细);

        //        string sql_采购计划主 = "select * from  采购计划主表 where 1<>1";
        //        string sql_采购计划明细 = "select * from  采购计划明细表 where 1<>1";

        //        cmm = new SqlCommand(sql_采购计划主, conn, ts);
        //        da = new SqlDataAdapter(cmm);
        //        new SqlCommandBuilder(da);
        //        da.Update(dt_采购计划主);

        //        cmm = new SqlCommand(sql_采购计划明细, conn, ts);
        //        da = new SqlDataAdapter(cmm);
        //        new SqlCommandBuilder(da);
        //        da.Update(dt_采购计划明细);

        //        ts.Commit();
        //        MessageBox.Show("转单成功");
        //    }
        //    catch (Exception ex)
        //    {
        //        ts.Rollback();
        //        throw new Exception(ex.Message);
        //    }

        //}
        #endregion

        private void fun_check()
        {
            foreach(DataRow dr in dtM.Rows)
            {
                decimal dec;
                decimal ddd;
                if (!decimal.TryParse(dr["可转数量"].ToString().Trim(), out dec)) throw new Exception("可转数量输入不正确");
                if (dec <= 0) throw new Exception("可转数量不可小于或等于0");
                ddd = Convert.ToDecimal(dr["数量"].ToString()) - Convert.ToDecimal(dr["已转数量"].ToString());
                //if (dec > ddd)
                //{
                //    throw new Exception("输入的可转数量已超出");

                //}
                 
            }
            
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {


            try
            {
                if (bl_c)
                {
                    MessageBox.Show("正在计算中,请稍后");
                }
                else
                {
                    label1.Text = "正在计算中,请稍候...";
                    bl_c = true;
                    gridView1.CloseEditor();
                    this.ActiveControl = null;
                    //gridView1.UpdateCurrentRow();
                    fun_check();
                    //DateTime t = CPublic.Var.getDatetime();
                    //s_主计划单号 = string.Format("MS{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                    //              CPublic.CNo.fun_得到最大流水号("MS", t.Year, t.Month));

                    //string sql_子 = "select * from  主计划子表 where 1<>1";
                    //DataTable dt_主子 = CZMaster.MasterSQL.Get_DataTable(sql_子, strconn);
                    //int i = 1;
                    //dt_主子.Columns.Add("库存总数", typeof(decimal));
                    //dt_主子.Columns.Add("在途量", typeof(decimal));
                    //dt_主子.Columns.Add("在制量", typeof(decimal));
                    //dt_主子.Columns.Add("未领量", typeof(decimal));
                    //foreach (DataRow dr in dtM.Rows)
                    //{
                    //    DataRow dr_主计划子 = dt_主子.NewRow();
                    //    dt_主子.Rows.Add(dr_主计划子);
                    //    dr_主计划子["主计划单号"] = s_主计划单号;
                    //    dr_主计划子["主计划明细号"] = s_主计划单号 + "-" + i.ToString("0000");
                    //    dr_主计划子["关联订单号"] = dr["销售订单号"].ToString();
                    //    dr_主计划子["关联订单明细号"] = dr["销售订单明细号"].ToString();
                    //    dr_主计划子["物料编码"] = dr["物料编码"].ToString();
                    //    dr_主计划子["物料名称"] = dr["物料名称"].ToString();
                    //    dr_主计划子["规格型号"] = dr["规格型号"].ToString();
                    //    dr_主计划子["客户名称"] = dr["客户名称"].ToString();
                    //    dr_主计划子["目标客户"] = dr["目标客户"].ToString();
                    //    dr_主计划子["生效日期"] = dr["生效日期"].ToString();
                    //    dr_主计划子["下单日期"] = dr["下单日期"].ToString();
                    //    dr_主计划子["订单制单人"] = dr["制单人"].ToString();
                    //    dr_主计划子["预计发货日期"] = dr["预计发货日期"].ToString();
                    //    //   dr_主计划子["预计开工日期"] = dr["预计开工日期"].ToString();
                    //    dr_主计划子["表头备注"] = dr["表头备注"].ToString();
                    //    dr_主计划子["备注"] = dr["备注"].ToString();
                    //    dr_主计划子["POS"] = i++;
                    //    dr_主计划子["主计划备注"] = dr["主计划备注"].ToString();
                    //    dr_主计划子["存货分类"] = dr["存货分类"].ToString();
                    //    dr_主计划子["主计划转单日期"] = t;
                    //    dr_主计划子["此次转单数量"] = Convert.ToDecimal(dr["可转数量"].ToString());
                    //    dr_主计划子["销售数量"] = Convert.ToDecimal(dr["数量"].ToString());
                    //    dr_主计划子["库存总数"] = Convert.ToDecimal(dr["库存总数"].ToString());
                    //    dr_主计划子["在途量"] = Convert.ToDecimal(dr["在途量"].ToString());
                    //    dr_主计划子["在制量"] = Convert.ToDecimal(dr["在制量"].ToString());
                    //    dr_主计划子["未领量"] = Convert.ToDecimal(dr["未领量"].ToString());
                    //    dr_主计划子["转单未完成数量"] = Convert.ToDecimal(dr_主计划子["此次转单数量"].ToString());
                    //    dr_主计划子["数量"] = Convert.ToDecimal(dr_主计划子["此次转单数量"].ToString());

                    //}
                    Action<DataTable> th = new Action<DataTable>(calculate);
                    //th.BeginInvoke(dtM,null,null);
                    th.BeginInvoke(dtM, null,null);

                    //Thread th = new Thread(() =>
                    //{
                    //    calculate(dtM);

                    //});
                    //th.IsBackground = true;
                    //th.Start();

                }
            }
            catch (Exception ex)
            {
                bl_c = false;
                BeginInvoke(new MethodInvoker(() =>
                {
                  label1.Text=  "错误原因:" +ex.Message;
                }));
            }
        }

        private void calculate(DataTable dtM)
        {
            try
            {
                //BeginInvoke(new MethodInvoker(() =>
                //{
                    
                //}));
                ERPorg.Corg.result_主计划 rs = new ERPorg.Corg.result_主计划();
                rs = ERPorg.Corg.fun_pool_all(dtM);
                dt_采购计划 = rs.dtM_采购池.Copy();
                dt_生产计划 = rs.dtM.Copy();
                dt_total = rs.TotalCount;

                dt_SaleOrder = new DataTable();

                dt_SaleOrder = rs.salelist_mx.Copy();
               // IncompletePO = rs.Polist_mx;
                str_log = rs.str_log;
                dt_bom = rs.Bom;
                dt_SaleOrder.Columns.Add("应完工日期", typeof(DateTime));
                foreach (DataRow saleR in dt_SaleOrder.Rows)
                {
                    saleR["应完工日期"] = Convert.ToDateTime(saleR["预计发货日期"]).AddDays(-1);
                }
                bl_生产 = true;
                bl_c = false;
                BeginInvoke(new MethodInvoker(() =>
                {
                    label1.Text = "计算完成";
                    //bl_calculate = false;
                }));

            }
            catch (Exception ex)
            {
                ////bl_calculate = false;
                BeginInvoke(new MethodInvoker(() =>
                {
                    label1.Text = "错误原因:" + ex.Message;
                    bl_c = false;
                    bl_生产 = false;
                }));
                
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (bl_c) throw new Exception("计算尚未完成");
                if (!bl_生产) throw new Exception("未计算或计算未完成");

                gridView1.CloseEditor();
                gridView1.UpdateCurrentRow();
                DateTime t = CPublic.Var.getDatetime();
                fun_check();
                
                Form2 fm = new Form2();
                ui_采购计划 ui = new ui_采购计划(dt_采购计划,dt_SaleOrder,str_log);
                fm.Controls.Add(ui);
                fm.Text = "原材料MRP";
                fm.WindowState = FormWindowState.Maximized;
                ui.Dock = DockStyle.Fill;
                fm.ShowDialog();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (bl_c) throw new Exception("计算尚未完成");

                if (!bl_生产) throw new Exception("未计算或计算未完成");

                Form2 fm = new Form2();
                ui_生产计划 ui = new ui_生产计划(dt_生产计划, dt_SaleOrder, str_log);
                fm.Controls.Add(ui);
                fm.Text = "半成品MRP";
                fm.WindowState = FormWindowState.Maximized;
                ui.Dock = DockStyle.Fill;
                fm.ShowDialog();

                //ui_生产池_1 ui = new ui_生产池_1(dt_生产计划, dt_采购计划, dt_SaleOrder, dt_bom, dt_total, str_log);
                //CPublic.UIcontrol.Showpage(ui, "物料需求计划");
                //bl_生产 = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
              
                if (e.Column.FieldName == "可转数量" && e.Value.ToString().Trim()!="")
                {

                    DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                    dr["预计耗时"] = Convert.ToDecimal(e.Value) * Convert.ToDecimal(dr["工时"]);


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 查看生产子项ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!bl_生产) throw new Exception("未计算或计算未完成");
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                Form2 fm = new Form2();
                ui_生产子项 ui = new ui_生产子项(dt_total,dr["物料编码"].ToString().Trim());
                fm.Controls.Add(ui);
                fm.Text = "生产子项";
                fm.WindowState = FormWindowState.Maximized;
                ui.Dock = DockStyle.Fill;
                fm.ShowDialog();                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 查看采购子项ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!bl_生产) throw new Exception("未计算或计算未完成");
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                Form2 fm = new Form2();
                ui_采购子项 ui = new ui_采购子项(dt_total, dr["物料编码"].ToString().Trim());
                fm.Controls.Add(ui);
                fm.Text = "采购子项";
                fm.WindowState = FormWindowState.Maximized;
                ui.Dock = DockStyle.Fill;
                fm.ShowDialog();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {

            
              
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (bl_c) throw new Exception("计算尚未完成");

                if (!bl_生产) throw new Exception("未计算或计算未完成");


                DataView dv = new DataView(dt_生产计划);
                dv.RowFilter = "订单用量>0";
                DataTable dt_生产计划_1 = dv.ToTable();

                ui_生产池_1 ui = new ui_生产池_1(dt_生产计划_1, dt_采购计划, dt_SaleOrder, dt_bom, dt_total, str_log,dtM);
                CPublic.UIcontrol.Showpage(ui, "物料需求计划");
                bl_生产 = true;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
          
          
            try
            {
                DataRow dr = gridView1.GetDataRow(e.RowHandle);
                if (dr == null) return;

                decimal dec = 0;
                if (!decimal.TryParse(dr["数量"].ToString(), out dec)) return;
                if (!decimal.TryParse(dr["已转数量"].ToString(), out dec)) return;
                if (!decimal.TryParse(dr["可转数量"].ToString(), out dec)) return;

                decimal ddd = Convert.ToDecimal(dr["数量"].ToString()) - Convert.ToDecimal(dr["已转数量"].ToString());
                if (Convert.ToDecimal(gridView1.GetRowCellValue(e.RowHandle, "可转数量")) > ddd)
                {
                    e.Appearance.BackColor = Color.Pink;
                }
                 
            }
            catch  
            { 

                
            }
        }
    }
}
