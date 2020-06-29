using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Threading;
namespace ERPSale
{
    public partial class frm销售记录销售开票详细界面 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dt_待办;
        DataView dv_待办;
        string str_待办条件 = "";
        bool chek = false;
        DataTable dt_客户;
        DataTable dt_产品;
        string cfgfilepath = "";
        string str_开票票号 = "";
        DataRow drM;
        DataTable dtM;
        DataTable dtP;
        /// <summary>
        /// 客户产品单价表中 新增或修改   和 修改日志
        /// table[0] 新增
        /// table[1] 修改
        /// table[2] 修改日志
        /// </summary>
        DataSet ds;
        /// <summary>
        /// 修改状态下 用来辅助判断开票号是否重复，
        /// 因为保存时不知道这个开票号是改成 x 还是原来就是x 
        /// </summary>
        string flag_guid = "";
        /// <summary>
        /// 指示 是否需要 修改
        /// 新增的 需修改  ds.table[1]  ds.table[2] 根据这个变量 更新
        /// </summary>
        bool f_price = false;
        /// <summary>
        /// false 为修改
        /// </summary>
        Boolean bl_新增or修改 = false;

        DataTable dt_已开票数量;
        #endregion
         
        #region 自用类
        public frm销售记录销售开票详细界面()
        {
            InitializeComponent();
            bl_新增or修改 = true;
            fun_载入();
        }
        public frm销售记录销售开票详细界面(DataRow dr, DataTable dt)
        {
            InitializeComponent();
            bl_新增or修改 = true;
            drM = dr;
            dtM = dt;
        }
        public frm销售记录销售开票详细界面(string s_开票通知单号, DataRow dr, DataTable dt)
        {
            InitializeComponent();
            bl_新增or修改 = false;
            // str_开票票号 = s_开票票号;
            a_开票通知单号 = s_开票通知单号;
             chek = true;
            flag_guid = dr["GUID"].ToString();
            textBox3.Text = dr["销售开票通知单号"].ToString();
            drM = dr;
            dtM = dt;

        }

        private void frm销售记录销售开票界面_Load(object sender, EventArgs e)
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                if (File.Exists(cfgfilepath + string.Format(@"\{0}.xml", this.Name)))
                {

                    gv_待办.RestoreLayoutFromXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }
                txt_开票员ID.Text = CPublic.Var.LocalUserID;
                txt_开票员.Text = CPublic.Var.localUserName;
                // txt_产品编码.EditValue = "";
                txt_客户编号.EditValue = "";
                //txt_产品编码.Enabled = false;
                txt_开票日期.EditValue = CPublic.Var.getDatetime().Date;
                // searchLookUpEdit2.Enabled = false;

////////////////6..6 加币种
             
              string   sql = "select 属性值 from 基础数据基础属性表 where 属性类别='币种' order by POS";
                DataTable dt_币种 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_币种);



                lookUpEdit1.Properties.DataSource = dt_币种;
                lookUpEdit1.Properties.DisplayMember = "属性值";
                lookUpEdit1.Properties.ValueMember = "属性值";


                //  DateTime dt1 = System.DateTime.Now;

                fun_客户下拉框();
                //  DateTime dt2 = System.DateTime.Now;
                //fun_产品下拉框();
                fun_载入待办();
                //   DateTime dt3 = System.DateTime.Now;
                fun_载入空主表();
                //  DateTime dt5 = System.DateTime.Now;
                fun_载入明细();
                //DateTime dt4 = System.DateTime.Now;
                //dr_传.ColumnChanged += dtP_ColumnChanged;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售开票—界面LOAD");
            }
        }

        private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
        {
            gv_待办.CloseEditor();
            gc_待办.BindingContext[dt_待办].EndCurrentEdit();
            gc_待办.BindingContext[dv_待办].EndCurrentEdit();

            DataRow drr = gv_待办.GetDataRow(gv_待办.FocusedRowHandle);
            try
            {
                if (drr["选择"].Equals(true))
                {
                    //DataRow[] r = dtP.Select(string.Format("成品出库单明细号='{0}' and 出库通知单明细号='{1}'", drr["成品出库单明细号"], drr["出库通知单明细号"]));
                    //if (r.Length == 0)
                    //{
                    //    DataRow dr = dtP.NewRow();   //dtP销售开票明细表
                    //    dtP.Rows.Add(dr);
                    //    dr["GUID"] = System.Guid.NewGuid();
                    //    dr["成品出库单明细号"] = drr["成品出库单明细号"].ToString();
                    //    dr["出库通知单明细号"] = drr["出库通知单明细号"].ToString();

                    //    dr["产品编码"] = drr["物料编码"].ToString();
                    //    dr["产品名称"] = drr["物料名称"].ToString();
                    //    //  dr["原ERP物料编号"] = drr["原ERP物料编号"].ToString();
                    //    //  dr["规格型号"] = drr["规格型号"].ToString();
                    //    dr["已出库数量"] = drr["已出库数量"].ToString();
                    //    dr["规格型号"] = drr["规格型号"].ToString();
                    //    dr["计量单位"] = drr["计量单位"].ToString();
                    //    dr["开票数量"] = drr["未开票数量"].ToString();

                    //    dr["未开票数量"] = drr["未开票数量"].ToString();

                    //    //dr["开票税前金额"] =drr["税前金额"].ToString();

                    //    //dr["开票税后金额"] = drr["税后金额"].ToString();

                    //    string sql = string.Format("select * from 销售记录销售订单明细表 where 销售订单明细号 = '{0}'", drr["销售订单明细号"].ToString());
                    //    DataTable dt = new DataTable();
                    //    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    //    da.Fill(dt);

                    //    sql = string.Format("select * from L销售记录销售订单明细表L where 销售订单明细号 = '{0}'", drr["销售订单明细号"].ToString());
                    //    da = new SqlDataAdapter(sql, strconn);

                    //    da.Fill(dt);
                    //    //textBox2.Text = dt.Rows[0]["税率"].ToString();
                    //    dr["开票税前单价"] = Math.Round(Convert.ToDecimal(dt.Rows[0]["税前单价"]), 6);
                    //    //18-5-1 国家调整税率  增加 以不含税单价 算含税单价
                    //    Decimal dec税率 = Convert.ToDecimal(textBox2.Text.ToString());
                    //    dr["开票税后单价"] = Math.Round(Convert.ToDecimal(dt.Rows[0]["税前单价"]) * (1 + dec税率 / 100), 6);

                    //    dr["开票税前金额"] = Math.Round((Convert.ToDecimal(dr["开票税前单价"]) * Convert.ToDecimal(drr["未开票数量"])), 6);
                    //    dr["开票税后金额"] = Math.Round((Convert.ToDecimal(dr["开票税后单价"]) * Convert.ToDecimal(drr["未开票数量"])), 2).ToString("0.00000");

                    //    txt_客户编号.EditValue = drr["客户编号"].ToString();
                    //    //txt_产品编码.EditValue = drr["物料编码"].ToString();
                    //}
                    
                        string s= drr["税率"].ToString();
                        decimal decm =13;
                        decimal.TryParse(s, out decm);
                        if(decm==0) textBox2.Text = drr["税率"].ToString();
                        //textBox2.Text = drr["税率"].ToString();
                    
                    if (drr["币种"] != null && drr["币种"]!=DBNull.Value && drr["币种"].ToString() != "")
                        lookUpEdit1.Text = drr["币种"].ToString();
                  
                    DataRow[] sale = dt_待办.Select(string.Format("订单号='{0}' ", drr["订单号"]));
                    drr["选择"] = false;
                    foreach (DataRow sr in sale)
                    {
                        if (sr["选择"]==null ||  !Convert.ToBoolean(sr["选择"]))
                        {

                            sr["选择"] = true;

                            DataRow[] r = dtP.Select(string.Format("成品出库单明细号='{0}' and 出库通知单明细号='{1}'", sr["成品出库单明细号"], sr["出库通知单明细号"]));
                            if (r.Length == 0)
                            {
                                DataRow dr = dtP.NewRow();   //dtP销售开票明细表
                                dtP.Rows.Add(dr);
                                dr["GUID"] = System.Guid.NewGuid();
                                dr["成品出库单明细号"] = sr["成品出库单明细号"].ToString();
                                dr["出库通知单明细号"] = sr["出库通知单明细号"].ToString();

                                dr["产品编码"] = sr["物料编码"].ToString();
                                dr["产品名称"] = sr["物料名称"].ToString();
                                //  dr["原ERP物料编号"] = drr["原ERP物料编号"].ToString();
                                //  dr["规格型号"] = drr["规格型号"].ToString();
                                dr["已出库数量"] = sr["已出库数量"].ToString();
                                dr["规格型号"] = sr["规格型号"].ToString();
                                dr["计量单位"] = sr["计量单位"].ToString();
                                //20-4-26 应该需要 - 累计退货数量
                                dr["开票数量"] = Convert.ToDecimal( sr["未开票数量"])- Convert.ToDecimal(sr["累计退货数量"]) - Convert.ToDecimal(sr["已开未审"].ToString()) ;

                                dr["未开票数量"] = sr["未开票数量"].ToString();
                                dr["累计退货数量"] = sr["累计退货数量"].ToString();

                                //dr["开票税前金额"] =drr["税前金额"].ToString();

                                //dr["开票税后金额"] = drr["税后金额"].ToString();

                                string sql = string.Format("select * from 销售记录销售订单明细表 where 销售订单明细号 = '{0}'", sr["销售订单明细号"].ToString());
                                DataTable dt = new DataTable();
                                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                                da.Fill(dt);

                                sql = string.Format("select * from L销售记录销售订单明细表L where 销售订单明细号 = '{0}'", sr["销售订单明细号"].ToString());
                                da = new SqlDataAdapter(sql, strconn);

                                da.Fill(dt);
                                //textBox2.Text = dt.Rows[0]["税率"].ToString();
                                dr["开票税前单价"] = Math.Round(Convert.ToDecimal(dt.Rows[0]["税前单价"]), 6);
                                //18-5-1 国家调整税率  增加 以不含税单价 算含税单价
                                Decimal dec税率 = Convert.ToDecimal(textBox2.Text.ToString());
                                dr["开票税后单价"] = Math.Round(Convert.ToDecimal(dt.Rows[0]["税前单价"]) * (1 + dec税率 / 100), 6);

                                dr["开票税前金额"] = Math.Round((Convert.ToDecimal(dr["开票税前单价"]) * Convert.ToDecimal(dr["开票数量"])), 2);
                                dr["开票税后金额"] = Math.Round((Convert.ToDecimal(dr["开票税后单价"]) * Convert.ToDecimal(dr["开票数量"])), 2);

                                txt_客户编号.EditValue = sr["客户编号"].ToString();
                                //txt_产品编码.EditValue = drr["物料编码"].ToString();
                            }

                        }
                    }
                }
                else
                {
                    drr["选择"]=true;
                    DataRow[] sale = dt_待办.Select(string.Format("订单号='{0}' ", drr["订单号"]));
                    foreach (DataRow sr in sale)
                    {
                        if (Convert.ToBoolean(sr["选择"]))
                        {
                            sr["选择"] = false;

                            DataRow[] r = dtP.Select(string.Format("成品出库单明细号='{0}'", sr["成品出库单明细号"]));
                            r[0].Delete();
                            if (gv.DataRowCount == 0)
                            {
                                txt_客户编号.EditValue = "";
                                txt_开票税后金额.Text = "";
                                txt_开票税前金额.Text = "";
                            }
                        }
                    }
                    //    DataRow[] r = dtP.Select(string.Format("成品出库单明细号='{0}'", drr["成品出库单明细号"]));
                    //r[0].Delete();
                    //if (gv.DataRowCount == 0)
                    //{
                    //    txt_客户编号.EditValue = "";
                    //    txt_开票税后金额.Text = "";
                    //    txt_开票税前金额.Text = "";
                    //}
                }
                    System.Decimal sum = 0;
                    System.Decimal sum1 = 0;
                    foreach (DataRow r in dtP.Rows)
                    {
                        if (r.RowState == DataRowState.Deleted)
                        {

                            continue;
                        }
                        try
                        {
                            sum += (Decimal)r["开票税后金额"];
                            sum1 += (Decimal)r["开票税前金额"];
                        }
                        catch
                        { }
                    }
                    txt_开票税前金额.Text = sum1.ToString("0.00");
                    txt_开票税后金额.Text = sum.ToString("0.00");
                    gv.MoveLast();
                }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "开票_repositoryItemCheckEdit1_CheckedChanged");
            }
        }

        private void txt_客户编号_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (txt_客户编号.EditValue != null && txt_客户编号.Text != "")
                {
                    DataRow[] ds = dt_客户.Select(string.Format("客户编号 = '{0}'", txt_客户编号.Text));
                    if (ds.Length != 0)
                    {
                        txt_客户编号.Text = ds[0]["客户编号"].ToString();
                        txt_客户名称.Text = ds[0]["客户名称"].ToString();
                        //  txt_产品编码.Enabled = true;
                        // searchLookUpEdit2.Enabled = true;
                        // dv_待办.RowFilter = "客户编号 = '" + txt_客户编号.Text + "' and " + str_待办条件;//"客户编号 = '" + txt_客户编号.Text + "' and " + 
                        dv_待办.RowFilter = "客户编号 = '" + txt_客户编号.Text + "'";//"客户编号 = '" + txt_客户编号.Text + "' and " + 

                        gc_待办.DataSource = dv_待办;
                    }

                    //txt_产品名称.Text = "";
                }
                else
                {
                    txt_客户编号.Text = "";
                    txt_客户名称.Text = "";
                    // txt_产品编码.Enabled = false;
                    //searchLookUpEdit2.Enabled = false;
                    gc_待办.DataSource = dt_待办;
                }
                //txt_产品编码.Text = "";
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "开票界面_txt_客户编号_EditValueChanged");
            }
        }
        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                DataRow[] ds = dt_客户.Select(string.Format("客户编号 = '{0}'", searchLookUpEdit1.EditValue));
                if (ds.Length != 0)
                {
                    txt_客户编号.Text = ds[0]["客户编号"].ToString();
                    txt_客户名称.Text = ds[0]["客户名称"].ToString();



                    //txt_产品编码.Enabled = true;
                }
                if (txt_客户编号.EditValue.ToString() == "")
                {
                    txt_客户编号.Text = "";
                    txt_客户名称.Text = "";
                    //txt_产品编码.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "开票界面_searchLookUpEdit1_EditValueChanged");
            }
        }

        //private void txt_产品编码_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        DataRow[] ds = dt_产品.Select(string.Format("物料编码 = '{0}'", txt_产品编码.Text));
        //        if (ds.Length != 0)
        //        {
        //            txt_产品编码.Text = ds[0]["物料编码"].ToString();
        //            txt_产品名称.Text = ds[0]["物料名称"].ToString();
        //            dv_待办.RowFilter = "客户编号 = '" + txt_客户编号.Text + "' and " + string.Format("物料编码 = '{0}'", txt_产品编码.Text) + " and " + str_待办条件;//"客户编号 = '" + txt_客户编号.Text + "' 物料编码 = '" + txt_产品编码.Text + "'" +
        //            gc_待办.DataSource = dv_待办;
        //            //dr_传.Clear();
        //            //fun_清空待办打钩();
        //        }
        //        if (txt_产品编码.EditValue.ToString() == "")
        //        {
        //            txt_产品编码.Text = "";
        //            txt_产品名称.Text = "";
        //            if (txt_客户编号.Text == "")
        //            {
        //                gc_待办.DataSource = dt_待办;
        //            }
        //            else
        //            {
        //                dv_待办.RowFilter = "客户编号 = '" + txt_客户编号.Text + "' and " + str_待办条件;//"客户编号 = '" + txt_客户编号.Text + "' and " + 
        //                gc_待办.DataSource = dv_待办;
        //            }
        //            dtP.Clear();
        //            fun_清空待办打钩();
        //            txt_开票税后金额.Text = "";
        //            txt_开票税前金额.Text = "";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CZMaster.MasterLog.WriteLog(ex.Message, "开票界面_txt_产品编码_SelectedIndexChanged");
        //    }
        //}
        //private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        DataRow[] ds = dt_产品.Select(string.Format("物料编码 = '{0}'", searchLookUpEdit2.EditValue));
        //        if (ds.Length != 0)
        //        {
        //            txt_产品编码.Text = ds[0]["物料编码"].ToString();
        //            txt_产品名称.Text = ds[0]["物料名称"].ToString();
        //            //dv_待办.RowFilter = "客户编号 = " + txt_客户编号.Text.Trim() + "物料编码 = " + txt_产品编码.Text.Trim() + str_待办条件;
        //            //gc.DataSource = dv_待办;
        //        }
        //        if (txt_产品编码.EditValue.ToString() == "")
        //        {
        //            txt_产品编码.Text = "";
        //            txt_产品名称.Text = "";
        //            //dv_待办.RowFilter = "客户编号 = " + txt_产品编码.Text + str_待办条件;
        //            //gc.DataSource = dv_待办;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CZMaster.MasterLog.WriteLog(ex.Message, "开票界面_searchLookUpEdit2_EditValueChanged");
        //    }
        //}
        #endregion

        #region 待办  方法
        private void fun_载入待办()
        {
            try
            {
                //string sql = "select * from 销售记录成品出库单明细表 where 生效 = 1 and 未开票数量 > 0";
                #region
                //                select yy.*,isnull(开票数量, 0)已开未审 from(select xx.*from(
                //select 币种, scmx.*, smx.销售订单号 as 订单号, sz.部门编号, 销售部门, sz.目标客户, sz.备注1 as 表头备注, 税前单价, (税前单价 * 出库数量)税前金额, 税后单价, (税后单价 * 出库数量)税后金额, sz.客户订单号

                //from 销售记录成品出库单明细表 scmx, 销售记录销售订单明细表 smx, 销售记录销售订单主表 sz
                //where scmx.销售订单明细号 = smx.销售订单明细号 and scmx.生效 = 1 and(未开票数量 > 0  or(scmx.备注1 <> '' and 未开票数量 < 0)) and scmx.作废 = 0
                //and smx.销售订单号 = sz.销售订单号
                //)xx union
                // select  ''币种,tzmx.ID,tzmx.GUID,''成品出库单号,''pos,'' 成品出库单明细号 ,szb.销售订单号,smx.销售订单明细号,出库通知单明细号,出库通知单号,tzmx.物料编码,tzmx.物料名称,'' BOM版本,0 as 数量 ,出库数量,出库数量 as 已出库数量
                //   ,累计开票数量 as 已开票数量, 出库数量 - 累计开票数量 as 未开票数量,0 数量,tzmx.计量单位,tzmx.规格型号,''图纸编号,tzmx.客户,tzmx.客户编号,smx.仓库号,smx.仓库名称,tzmx.生效,tzmx.生效日期,tzmx.作废,tzmx.作废时间 as 作废日期,
                //   作废人,tzmx.完成,tzmx.完成日期,备注1,备注2,smx.备注3,smx.备注4,smx.备注5,smx.备注6,smx.备注7,smx.备注8,smx.备注9,smx.备注10,tzmx.特殊备注,'' 送货方式,tzmx.销售备注,0 as 累计退货数量,'' 退货标识,0 发出单价,szb.销售订单号,部门编号,销售部门,目标客户,
                //   '' 表头备注,税前单价,smx.税前金额,税后单价,smx.税后金额,客户订单号 from 销售记录销售出库通知单明细表 tzmx
                // left join  销售记录销售订单明细表 smx  on smx.销售订单明细号 = tzmx.销售订单明细号
                // left join 销售记录销售订单主表 szb on szb.销售订单号 = smx.销售订单号
                // where left(smx.物料编码,3)= '200' and 出库数量 > 累计开票数量 and tzmx.生效日期 < '2019-7-30 18:00:00' )yy
                //left join(select  成品出库单明细号, 出库通知单明细号, sum(开票数量)开票数量 from 销售记录销售开票明细表 k

                //left join 销售记录销售开票主表 z on z.销售开票通知单号 = k.销售开票通知单号
                //where k.生效 = 0  and 创建日期 > '2019-5-1'  group by  成品出库单明细号, 出库通知单明细号, k.产品编码)d
                //      on d.成品出库单明细号 + d.出库通知单明细号 = yy.成品出库单明细号 + yy.出库通知单明细号
                //  where isnull(开票数量,0)< abs(未开票数量)  and abs(未开票数量)-累计退货数量 - abs(isnull(开票数量, 0)) > 0
                #endregion


                //Thread th = new Thread(() =>
                // {
                     string sql = @"select * from  V_销售待开票 "; //where  表头备注<>'关闭' 
                  string s = CPublic.Var.localUser部门名称;
                     if (s != "" && CPublic.Var.LocalUserTeam=="管理员权限")
                     {
                         sql += string.Format(" where 销售部门='{0}' or 销售部门=''", s);
                     }
                  // and z.提交审核 = 1  19-7-30晚更新 劳务类 也要做出库通知单 审核后自动生成出库单 开票按出库单开,7-31日之前做了发货通知单的就按原样可以根据通知单开票
                  //即7-31日以前的通知单 没有出库单
                  dt_待办 = new DataTable();
                     SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                     da.Fill(dt_待办);
                  //string sql_补 = "";
                  //sql_补 = @"select scmx.*,'',税前单价,(税前单价*出库数量)税前金额,税后单价,(税后单价*出库数量)税后金额 
                  //            from L销售记录成品出库单明细表L scmx,L销售记录销售订单明细表L smx 
                  //           where scmx.销售订单明细号= smx.销售订单明细号  
                  //              and scmx.生效 = 1 and (未开票数量 > 0 or (备注1<>'' and 未开票数量<0)) and scmx.作废=0  and smx.关闭=0 ";

                  //SqlDataAdapter da_1 = new SqlDataAdapter(sql_补, strconn);
                  //da_1.Fill(dt_待办);
                  DataColumn dc = new DataColumn("选择", typeof(Boolean));
                     dc.DefaultValue = false;
                     dt_待办.Columns.Add(dc);



                  //foreach (DataRow r_x in dt_待办.Rows)
                  //{
                  //    r_x["选择"] = false;
                  //}
                  dv_待办 = new DataView(dt_待办);
                     //string s = CPublic.Var.localUser部门名称;
                     //if (s != "")
                     //{
                     //    dv_待办.RowFilter = string.Format("销售部门='{0}' or 销售部门=''", s);
                     //}
                     //BeginInvoke(new MethodInvoker(() =>
                     //{
                         gc_待办.DataSource = dt_待办;


                     //}));

                // });
                //th.Start();
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "开票界面_fun_载入待办");
            }
        }
     

        private void fun_客户下拉框()
        {
            try
            {
                txt_客户编号.Properties.Items.Clear();

                string sql = "select 客户编号,客户名称 from 客户基础信息表";
                dt_客户 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_客户);

                foreach (DataRow r in dt_客户.Rows)
                {
                    txt_客户编号.Properties.Items.Add(r["客户编号"].ToString());
                }

                searchLookUpEdit1.Properties.DataSource = dt_客户;
                searchLookUpEdit1.Properties.DisplayMember = "客户编号";
                searchLookUpEdit1.Properties.ValueMember = "客户编号";
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "开票界面_fun_客户下拉框");
            }
        }

        //private void fun_产品下拉框()
        //{
        //    try
        //    {
        //        //txt_产品编码.Properties.Items.Clear();

        //        string sql2 = "select 物料名称,物料编码,规格型号,计量单位,标准单价 from 基础数据物料信息表 where 物料类型 = '成品'";//停用 = 0 and 生效 = 1 and 
        //        SqlDataAdapter da = new SqlDataAdapter(sql2, strconn);
        //        dt_产品 = new DataTable();
        //        da.Fill(dt_产品);

        //        foreach (DataRow r in dt_产品.Rows)
        //        {
        //            txt_产品编码.Properties.Items.Add(r["物料编码"].ToString());
        //        }

        //        searchLookUpEdit2.Properties.DataSource = dt_产品;
        //        searchLookUpEdit2.Properties.DisplayMember = "物料编码";
        //        searchLookUpEdit2.Properties.ValueMember = "物料编码";
        //    }
        //    catch (Exception ex)
        //    {
        //        CZMaster.MasterLog.WriteLog(ex.Message, "开票界面_fun_产品下拉框");
        //    }
        //}

        private void fun_清空待办打钩()
        {
            foreach (DataRow r in dt_待办.Rows)
            {
                r["选择"] = false;
            }
        }
        #endregion

        #region 开票  方法
        private void fun_载入()
        {
            try
            {
                string sql = "select * from 销售记录销售开票主表 where 1<>1";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                dtM = new DataTable();
                da.Fill(dtM);
                drM = dtM.NewRow();
                dtM.Rows.Add(drM);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm销售记录成品出库通知单界面_fun_载入");
            }
        }

        private void fun_载入空主表()
        {
            try
            {
                if (bl_新增or修改 == true)
                {

                }
                else
                {
                    dataBindHelper1.DataFormDR(drM);
                    lookUpEdit1.EditValue = drM["币种"].ToString();
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "开票界面_fun_载入空主表");
            }
        }

        private void fun_载入明细()
        {
            try
            {
                string sql = "";
                if (bl_新增or修改 == true)
                {
                    sql = @"select 销售记录销售开票明细表.* from 销售记录销售开票明细表  where     1<>1";
                }
                else
                {
                    sql = string.Format(@"select 销售记录销售开票明细表.* from 销售记录销售开票明细表
                      where   销售开票通知单号 = '{0}' order by CONVERT(int,POS)", a_开票通知单号);
                }
                dtP = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtP);
                //dtP.AcceptChanges();
                dtP.Columns.Add("未开票数量", typeof(decimal));
                dtP.Columns.Add("已出库数量", typeof(decimal));
                //20-4-27
                dtP.Columns.Add("累计退货数量", typeof(decimal));


                gc.DataSource = dtP;
                if (bl_新增or修改 == false)
                {
                    //成品出库单明细号
                    foreach (DataRow dr in dtP.Rows)
                    {
                        DataRow[] r = dt_待办.Select(string.Format("成品出库单明细号='{0}'", dr["成品出库单明细号"]));
                        if (r.Length == 0)
                        {
                            //5-28 
                            if (dr["产品名称"].ToString().Contains("劳务"))
                            {
                                DataRow[] rx = dt_待办.Select(string.Format("出库通知单明细号='{0}'", dr["出库通知单明细号"]));
                                if (rx.Length == 0)
                                    throw new Exception("存在上次保存的记录在待办事项找不到的记录,请确认本单子明细是否存在问题");
                                rx[0]["选择"] = true;
                                dr["已出库数量"] = rx[0]["已出库数量"];
                                dr["未开票数量"] = rx[0]["未开票数量"];
                                //20-4-27
                                dr["累计退货数量"] = rx[0]["累计退货数量"];


                            }
                            else
                            {
                                throw new Exception("存在上次保存的记录在待办事项找不到的记录,请确认本单子明细是否存在问题");

                            }
                        }
                        else
                        {
                            r[0]["选择"] = true;
                            dr["已出库数量"] = r[0]["已出库数量"];
                            dr["未开票数量"] = r[0]["未开票数量"];
                            //20-4-27
                            dr["累计退货数量"] = r[0]["累计退货数量"];
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "开票界面_fun_载入明细");
            }
        }
        string a_开票通知单号;
        private void fun_保存主表()
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();
                if (bl_新增or修改 == true)
                {
                    a_开票通知单号 = string.Format("SS{0}{1:D2}{2:D4}", t.Year,t.Month, CPublic.CNo.fun_得到最大流水号("SS",t.Year,t.Month)); 
                    //19-7-8发现,崔晓东做的用的SA的流水号,虽不会出错 但是用SA的流水号不是很好 SA和SS都不延续
                    drM["销售开票通知单号"] = a_开票通知单号.ToString();
                    drM["GUID"] = System.Guid.NewGuid();
                    flag_guid = drM["GUID"].ToString();
                    drM["创建日期"] = t;
                    textBox3.Text = a_开票通知单号.ToString();
                }

                try
                {
                    drM["修改日期"] = t;
                    Decimal dec_税后 = 0;
                    Decimal dec_税前 = 0;
                    drM["汇率"] = textBox4.Text;
                    foreach (DataRow r in dtP.Rows)
                    {
                        if (r.RowState == DataRowState.Deleted)
                        {
                            continue;
                        }
                        r["本币税后金额"] = Math.Round( Convert.ToDecimal(r["开票税后金额"]) * Convert.ToDecimal(drM["汇率"]),2);
                        dec_税后 = dec_税后 + Convert.ToDecimal(r["开票税后金额"]);
                        dec_税前 = dec_税前 + Convert.ToDecimal(r["开票税前金额"]);
                    }
                    drM["开票税后金额"] = dec_税后;
                    drM["开票税前金额"] = dec_税前;
                    drM["税率"] = Convert.ToInt32(textBox2.Text);
                    drM["币种"] = lookUpEdit1.Text;
                    drM["汇率"] = textBox4.Text;
                    dataBindHelper1.DataToDR(drM);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                //string sql = string.Format("select * from 销售记录销售开票主表 where 开票票号 = '{0}'", txt_开票票号.Text);
                //DataTable dtM = new DataTable();
                //SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                //da.Fill(dtM);
                //if (dtM.Rows.Count > 0)
                //{
                //    throw new Exception("开票票号重复！");
                //}
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "开票界面_");
                throw ex;
            }
        }

        private void fun_保存明细()
        {
            try
            {
                int i = 1;
                foreach (DataRow r in dtP.Rows)
                {
                    if (r.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }
                    r["POS"] = i++;
                    r["开票票号"] = txt_开票票号.Text;
                    r["开票明细号"] = txt_开票票号.Text + "-" + Convert.ToInt32(r["POS"]).ToString();
                    r["销售开票通知单号"] = a_开票通知单号.ToString();
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "开票界面_");
            }
        }
        /// <summary>
        /// 17-12-16  开票时先将没有对应关系的 加进去 ，修改的 因为 
        /// 19-5-28 东屋电器不需要
        /// </summary>
        /// <returns></returns>
        private DataSet fun_客户产品单价()
        {
            DataSet ds = new DataSet();
            string s = "select  * from 客户产品单价表 where 1<>1";
            DateTime time = CPublic.Var.getDatetime();
            DataTable dt_增 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            DataTable dt_改 = dt_增.Clone();
            s = "select  * from 销售单价修改记录表 where 1<>1";
            DataTable dt_修改记录 = CZMaster.MasterSQL.Get_DataTable(s, strconn);

            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;

                int i = dtP.Select(string.Format("产品编码='{0}'", dr["产品编码"].ToString())).Length;
                if (i == 1)  // 有多条记录的不管 
                {
                    s = string.Format("select * from 客户产品单价表 where 物料编码='{0}' and 客户编号='{1}' ", dr["产品编码"].ToString(), txt_客户编号.EditValue.ToString());
                    DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                    if (temp.Rows.Count == 0) //先在对应表中 查找 有？continue;查找tem中有没有？  如果没有 插入 temp 
                    {
                        DataRow[] tr = temp.Select(string.Format("物料编码='{0}'", dr["产品编码"].ToString()));
                        if (tr.Length == 0)
                        {
                            DataRow r = dt_增.NewRow();
                            r["客户编号"] = txt_客户编号.EditValue.ToString();
                            r["物料编码"] = dr["产品编码"].ToString();
                            r["单价"] = dr["开票税后单价"].ToString();
                            r["修改时间"] = time;
                            dt_增.Rows.Add(r);
                        }
                    }
                    else  //单价对应表中有记录  修改
                    {
                        if (Convert.ToDecimal(temp.Rows[0]["单价"]) != Convert.ToDecimal(dr["开票税后单价"]))
                        {
                            dt_改.ImportRow(temp.Rows[0]);

                            //string ss = string.Format("select  * from 客户产品单价表 where 物料编码='{0}'", dr["产品编码"].ToString());
                            //using ( SqlDataAdapter da =new SqlDataAdapter (ss,strconn))
                            //{
                            //    da.Fill(dt_改);
                            DataRow[] rr = dt_改.Select(string.Format("物料编码='{0}'", dr["产品编码"].ToString()));
                            rr[0]["单价"] = Convert.ToDecimal(dr["开票税后单价"]);
                            rr[0]["修改时间"] = time;

                            //}
                            DataRow r_modified = dt_修改记录.NewRow();
                            r_modified["物料编码"] = dr["产品编码"];
                            r_modified["原单价"] = temp.Rows[0]["单价"];
                            r_modified["修改单价"] = Convert.ToDecimal(dr["开票税后单价"]);
                            r_modified["修改日期"] = time;
                            r_modified["修改人"] = CPublic.Var.localUserName;
                            dt_修改记录.Rows.Add(r_modified);
                        }

                    }
                }

            }
            ds.Tables.Add(dt_增);
            ds.Tables.Add(dt_改);
            ds.Tables.Add(dt_修改记录);


            return ds;
        }
        private void fun_事务_保存()
        {
            //17-12-16 
            //   DataTable dt_客户产品= fun_客户产品单价();
            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction kpsx = conn.BeginTransaction("开票生效");
            try
            {
                string sql = "select * from 销售记录销售开票明细表 where 1<>1";
                SqlCommand cmd = new SqlCommand(sql, conn, kpsx);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dtP);

                }
                sql = "select * from 销售记录销售开票主表 where 1<>1";
                cmd = new SqlCommand(sql, conn, kpsx);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dtM);
                }

                //sql = "select * from 客户产品单价表 where 1<>1";
                //cmd = new SqlCommand(sql, conn, kpsx);
                //SqlDataAdapter da1 = new SqlDataAdapter(cmd);

                //new SqlCommandBuilder(da1);

                //if (f_price) //true 则合并修改 和新增的   false  只更新新增的
                //{
                //    ds.Tables[0].Merge(ds.Tables[1]);
                //    sql = "select  * from 销售单价修改记录表 where 1=2 ";
                //    SqlCommand cmd1 = new SqlCommand(sql, conn, kpsx);
                //    SqlDataAdapter da2 = new SqlDataAdapter(cmd1);
                //    new SqlCommandBuilder(da2);
                //    da2.Update(ds.Tables[2]);
                //}
                //da1.Update(ds.Tables[0]);

                //cmd = new SqlCommand(sql, conn, kpsx);

                //da1 = new SqlDataAdapter(cmd);
                //new SqlCommandBuilder(da1);

                //da1.Update(ds.Tables[1]);
                //客户单价修改记录表 
                //sql = "select  * from 销售单价修改记录表 where 1=2 ";
                //cmd = new SqlCommand(sql, conn, kpsx);
                //da1 = new SqlDataAdapter(cmd);
                //new SqlCommandBuilder(da1);
                //da1.Update(ds.Tables[2]);


                if (dt_已开票数量 != null)
                {
                    //12/19   dt_已开票数量 分为两个部分  销售记录成品出库单明细表 没 通知单号的 是补开的
                    DataTable dt_辅助 = dt_已开票数量.Clone();

                    for (int i = 0; i < dt_已开票数量.Rows.Count; i++)
                    {
                        string sql_z = string.Format(@"select  * from 销售记录成品出库单明细表 where 成品出库单明细号='{0}'", dt_已开票数量.Rows[i]["成品出库单明细号"]);
                        DataTable dt_z = CZMaster.MasterSQL.Get_DataTable(sql_z, strconn);
                        if (dt_z.Rows.Count == 0)
                        {
                            dt_辅助.ImportRow(dt_已开票数量.Rows[i]);
                            dt_已开票数量.Rows.Remove(dt_已开票数量.Rows[i]);
                            i--;
                        }

                    }

                    sql = "select * from 销售记录成品出库单明细表 where 1<>1";
                    cmd = new SqlCommand(sql, conn, kpsx);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dt_已开票数量);
                    }
                    sql = "select * from L销售记录成品出库单明细表L where 1<>1";
                    cmd = new SqlCommand(sql, conn, kpsx);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dt_辅助);
                    }
                }
               kpsx.Commit();
            }
            catch (Exception ex)
            {
                kpsx.Rollback();
                throw ex;
            }
        }

        private void fun_清空()
        {
            flag_guid = "";
            textBox3.Text = "";
            txt_开票票号.Text = "";
            txt_开票员ID.Text = CPublic.Var.LocalUserID;
            txt_开票员.Text = CPublic.Var.localUserName;
            txt_客户编号.Text = "";
            txt_客户名称.Text = "";
            //txt_产品编码.Text = "";
            //txt_产品名称.Text = "";
            txt_开票税前金额.Text = "";
            txt_开票税后金额.Text = "";
            txt_开票日期.EditValue = CPublic.Var.getDatetime();
            //txt_产品编码.Enabled = false;
            //searchLookUpEdit2.Enabled = false;
            fun_载入空主表();
            fun_载入明细();
            f_price = false;
            ds = new DataSet();
        }
        private void fun_check()
        {
            //if (txt_开票票号.Text.ToString() == "")
            //    throw new Exception("开票号不可以为空");

            //if (bl_新增or修改 == true)
            //{
            //    string sql = string.Format("select * from [销售记录销售开票主表] where 开票票号='{0}'", txt_开票票号.Text.ToString().Trim());
            //    DataTable dt = new DataTable();
            //    dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            //    if (dt.Rows.Count > 0)
            //    {
            //        throw new Exception("开票号重复请重新确认");

            //    }
            //}
            //else  
            //{
            //    string sql = string.Format("select * from [销售记录销售开票主表] where 开票票号='{0}' and GUID<>'{1}'"
            //        ,txt_开票票号.Text.ToString().Trim(),flag_guid);
            //    DataTable dt = new DataTable();
            //    dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            //    if (dt.Rows.Count > 0)
            //    {
            //        throw new Exception("开票号重复请重新确认");

            //    }


            //}
            //2020-4-26 开票按道理应该是 出库数量 -已开票数量 -累计退货数量 
            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;

             
                if (Math.Abs(Convert.ToDecimal(dr["开票数量"])) > Math.Abs(Convert.ToDecimal(dr["未开票数量"]))- Convert.ToDecimal(dr["累计退货数量"]))
                {
                    throw new Exception("开票数量超过可开票数量");
                }
                if (!dr["产品名称"].ToString().Contains("劳务")|| dr["成品出库单明细号"].ToString()!="")
                {
                    string s = string.Format(@"select   成品出库单明细号,sum(开票数量)开票数量 from 销售记录销售开票明细表 where 开票票号<>'{0}' 
                and 成品出库单明细号='{1}' and GUID<>'{2}'   group by 成品出库单明细号", txt_开票票号.Text.ToString(), dr["成品出库单明细号"].ToString(), dr["GUID"].ToString());

                    using (SqlDataAdapter da = new SqlDataAdapter(s, strconn))
                    {
                        DataTable temp = new DataTable();
                        da.Fill(temp);
                        if (temp.Rows.Count > 0)
                        {
                            if (Math.Abs(Convert.ToDecimal(dr["开票数量"])) + Math.Abs(Convert.ToDecimal(temp.Rows[0]["开票数量"])) > Math.Abs(Convert.ToDecimal(dr["已出库数量"])))
                            {
                                throw new Exception(string.Format("{0}与其他开票清单中该明细数量相加大于需总开票数量", dr["成品出库单明细号"].ToString()));
                            }
                        }
                    }
                }
                else //劳务
                {
                    string s = string.Format(@"select   出库通知单明细号,sum(开票数量)开票数量 from 销售记录销售开票明细表 where 开票票号<>'{0}' 
                and 出库通知单明细号='{1}' and GUID<>'{2}'   group by 出库通知单明细号", txt_开票票号.Text.ToString(), dr["出库通知单明细号"].ToString(), dr["GUID"].ToString());

                    using (SqlDataAdapter da = new SqlDataAdapter(s, strconn))
                    {
                        DataTable temp = new DataTable();
                        da.Fill(temp);
                        if (temp.Rows.Count > 0)
                        {
                            if (Math.Abs(Convert.ToDecimal(dr["开票数量"])) + Math.Abs(Convert.ToDecimal(temp.Rows[0]["开票数量"])) > Math.Abs(Convert.ToDecimal(dr["已出库数量"])))
                            {
                                throw new Exception(string.Format("{0}与其他开票清单中该明细数量相加大于需总开票数量", dr["成品出库单明细号"].ToString()));
                            }

                        }


                    }
                }

            }
            //19-5-28 东屋不需要
            //  ds = fun_客户产品单价();
            //if (ds.Tables[1].Rows.Count > 0)  ///有需要修改的记录  
            //{
            //    //弹窗提示 是否更新       
            //    if (MessageBox.Show(string.Format("有变更的单价是否更新到客户单价对照表？"), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
            //    {
            //        f_price = true;
            //    }

            //}



        }
        private void fun_生效()
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();
                drM["生效"] = 1;
                drM["生效日期"] = t;
                fun_保存主表();
                foreach (DataRow r in dtP.Rows)
                {
                    if (r.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }
                    r["生效"] = 1;
                    r["生效日期"] = t;
                }
                fun_保存明细();
                fun_已开票数量();

                fun_事务_保存();
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "开票界面_fun_生效");
                throw ex;
            }
        }

        private void fun_改单价()    //生效修改对应销售单上的单价           17/3/17号决定不改单价了 弃用
        {
            DataView dv = new DataView(dt_待办);
            dv.RowFilter = "选择=1";
            DataTable dt = dv.ToTable();
            Dictionary<int, String> dict = new Dictionary<int, String>();
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                string s = dr["销售订单明细号"].ToString();
                //s = s.Substring(0, s.Length - 3);


                s = s.Substring(0, s.IndexOf('-', 0));

                if (dict.ContainsValue(s))
                {
                    continue;
                }
                else
                {
                    dict.Add(i++, s);
                }
                string sql_L = string.Format(@"update L销售记录销售订单明细表L set 税前单价=a.开票税前单价,税后单价 =a.开票税后单价,税前金额=a.开票税前金额,税后金额=a.开票税后金额  from(
              select L销售记录销售订单明细表L.销售订单明细号,开票税前单价,开票税前金额,开票税后单价,开票税后金额 from L销售记录销售订单明细表L,L销售记录成品出库单明细表L,销售记录销售开票明细表
              where L销售记录销售订单明细表L.销售订单明细号=L销售记录成品出库单明细表L.销售订单明细号 and
              L销售记录成品出库单明细表L.成品出库单明细号=销售记录销售开票明细表.成品出库单明细号) a 
              where a.销售订单明细号=L销售记录销售订单明细表L.销售订单明细号 and L销售记录销售订单明细表L.销售订单明细号='{0}'", dr["销售订单明细号"]);
                CZMaster.MasterSQL.ExecuteSQL(sql_L, strconn);
                string sql = string.Format(@" update 销售记录销售订单明细表 set 税前单价=a.开票税前单价,税后单价 =a.开票税后单价,税前金额=a.开票税前金额,税后金额=a.开票税后金额  from(
              select 销售记录销售订单明细表.销售订单明细号,开票税前单价,开票税前金额,开票税后单价,开票税后金额 from 销售记录销售订单明细表,销售记录成品出库单明细表,销售记录销售开票明细表
              where 销售记录销售订单明细表.销售订单明细号=销售记录成品出库单明细表.销售订单明细号 and
              销售记录成品出库单明细表.成品出库单明细号=销售记录销售开票明细表.成品出库单明细号) a 
              where a.销售订单明细号=销售记录销售订单明细表.销售订单明细号 and 销售记录销售订单明细表.销售订单明细号='{0}'", dr["销售订单明细号"]);
                CZMaster.MasterSQL.ExecuteSQL(sql, strconn);
            }
            // 更新主表上的数据
            //获取所有需要更新的 销售单
            foreach (string str in dict.Values)
            {
                string sql_主 = string.Format(@"update 销售记录销售订单主表 set 税前金额=a.税前总金额,税后金额=a.税后总金额  from 
                (select 销售订单号,sum(税前金额)税前总金额 ,sum(税后金额)税后总金额 from  销售记录销售订单明细表  group by 销售订单号 )a
                     where a.销售订单号=销售记录销售订单主表.销售订单号 and  销售记录销售订单主表.销售订单号='{0}'", str);

                string sql_主L = string.Format(@"update L销售记录销售订单主表L set 税前金额=a.税前总金额,税后金额=a.税后总金额  from 
                (select 销售订单号,sum(税前金额)税前总金额 ,sum(税后金额)税后总金额 from  L销售记录销售订单明细表L  group by 销售订单号 )a
                     where a.销售订单号=L销售记录销售订单主表L.销售订单号 and  L销售记录销售订单主表L.销售订单号='{0}'", str);
                CZMaster.MasterSQL.ExecuteSQL(sql_主, strconn);
                CZMaster.MasterSQL.ExecuteSQL(sql_主L, strconn);

            }




        }

        private void fun_强载()
        {
            try
            {
                string sql = string.Format("select * from 销售记录销售开票主表 where 销售开票通知单号 = '{0}'", textBox3.Text);
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                drM = dtM.Rows[0];
                dataBindHelper1.DataFormDR(drM);
                {
                    string sqll = string.Format(@"select 销售记录销售开票明细表.*  from 销售记录销售开票明细表  
                        where 销售开票通知单号 = '{0}'   order by CONVERT(int,POS)", textBox3.Text);
                    using (SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn))
                    {
                        dtP = new DataTable();
                        daa.Fill(dtP);
                        dtP.Columns.Add("未开票数量", typeof(decimal));
                        //20-4-27
                        dtP.Columns.Add("累计退货数量", typeof(decimal));

                        foreach (DataRow dr in dtP.Rows)
                        {
                            DataRow[] r = dt_待办.Select(string.Format("成品出库单明细号='{0}'", dr["成品出库单明细号"]));
                            if (r.Length == 0)
                            {
                                throw new Exception("存在上次保存的记录在待办事项找不到的记录,请确认本单子明细是否存在问题");
                            }
                            r[0]["选择"] = true;
                            dr["未开票数量"] = r[0]["未开票数量"];
                            //20-4-27
                            dr["累计退货数量"] = r[0]["累计退货数量"];

                        }

                        gc.DataSource = dtP;
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "开票界面_fun_强载");
            }
        }

        private void fun_已开票数量()
        {
            dt_已开票数量 = new DataTable();
            foreach (DataRow r in dtP.Rows)
            {

                if (r.RowState == DataRowState.Deleted)
                {
                    continue;
                }
                string sql = string.Format("select * from 销售记录成品出库单明细表 where 成品出库单明细号 = '{0}'", r["成品出库单明细号"].ToString().Trim());
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_已开票数量);
                sql = string.Format("select * from L销售记录成品出库单明细表L where 成品出库单明细号 = '{0}'", r["成品出库单明细号"].ToString().Trim());
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_已开票数量);

                DataRow[] ds = dt_已开票数量.Select(string.Format("成品出库单明细号 = '{0}'", r["成品出库单明细号"].ToString().Trim()));

                ds[0]["已开票数量"] = Convert.ToDecimal(ds[0]["已开票数量"]) + Convert.ToDecimal(r["开票数量"]);
                ds[0]["未开票数量"] = Convert.ToDecimal(ds[0]["未开票数量"]) - Convert.ToDecimal(r["开票数量"]);
            }
        }
        #endregion

        #region 界面操作
        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                ERPorg.Corg.FlushMemory();
                DataView dv = new DataView(dtM);
                dv.RowStateFilter = DataViewRowState.Added;
                //if (dv.Count > 0)
                //{


                //fun_清空();
                //dtP.Clear();
                //fun_载入明细();
                //fun_载入待办();
                if (textBox3.Text != "")
                {
                    fun_强载();
                    bl_新增or修改 = false;
                }
                else
                {
                    drM = dtM.NewRow();
                    dtM.Rows.Add(drM);

                    bl_新增or修改 = true;
                    fun_清空();
                    dtP.Clear();
                    fun_载入明细();
                    fun_载入待办();
 

                }
       
                    //if (txt_开票票号.Text != "")
                    //{
                    //    fun_强载();
                    //}
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "开票界面_");
            }
        }

        //新增
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataView dv = new DataView(dtM);
                dv.RowStateFilter = DataViewRowState.Added;
                if (dv.Count > 0)
                {
                    if (MessageBox.Show("当前有未保存的开票单，是否放弃保存？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {

                        fun_清空();
                        dtP.Clear();
                        fun_载入明细();
                        fun_载入待办();
                    }
                }
                else
                {
                    drM = dtM.NewRow();
                    dtM.Rows.Add(drM);
                    
                    bl_新增or修改 = true;
                    fun_清空();
                    dtP.Clear();
                    fun_载入明细();
                    fun_载入待办();

                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "开票界面_");
            }
        }
        //保存
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                try
                {
                    gv.CloseEditor();
                    gc.BindingContext[dtP].EndCurrentEdit();
                    gv_待办.CloseEditor();
                    gc_待办.BindingContext[dt_待办].EndCurrentEdit();
                    gc_待办.BindingContext[dv_待办].EndCurrentEdit();
                    //if (txt_开票票号.Text == "")
                    //{
                    //    throw new Exception("未填发票号");
                    //}
                    if (chek != true)
                    {
                        fun_check();
                    }
                   
                    fun_保存主表();
                    fun_保存明细();

                    fun_事务_保存();
                    bl_新增or修改 = false;
                    //fun_载入待办();
                    fun_强载();
                    fun_载入待办(); //销售人员需要 保存完了后 不要勾着 她要确认是否保存成功方便操作

                    ds = new DataSet();
                    //保存完变成修改状态  

                    MessageBox.Show("保存成功");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "开票界面_");
            }
        }
        /// <summary>
        /// 保存生成 审核单
        /// </summary>
        /// 
        //private DataTable fun_PA(string str_开票通知单号)
        //{
        //    DataRow r_upper = ERPorg.Corg.fun_hr_upper("销售开票通知单", CPublic.Var.LocalUserID);
        //    if (r_upper == null)
        //    {
        //        throw new Exception("未找到你的上级审核人员");
        //    }
        //    DataTable dt_申请;
        //    string s = string.Format("select * from  单据审核申请表 where 关联单号='{0}'", str_开票通知单号);
        //    dt_申请 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
        //    DateTime t = CPublic.Var.getDatetime();
        //    string str_pa = "";
        //    if (dt_申请.Rows.Count == 0)
        //    {
        //        str_pa = string.Format("AP{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("AP", t.Year, t.Month));
        //        // 申请主表记录
        //        DataRow r_z = dt_申请.NewRow();
        //        r_z["审核申请单号"] = str_pa;
        //        r_z["关联单号"] = textBox3.Text;
        //        r_z["相关单位"] = txt_客户名称.Text;
        //        r_z["单据类型"] = "销售开票通知单";
        //        //decimal dec = Convert.ToDecimal(txt_cgshhje.Text);
        //        //r_z["总金额"] = dec;
        //        r_z["申请人ID"] = CPublic.Var.LocalUserID;
        //        r_z["申请人"] = CPublic.Var.localUserName;
        //        r_z["申请时间"] = t;
        //        r_z["待审核人ID"] = r_upper["工号"];
        //        r_z["待审核人"] = r_upper["姓名"];

        //        dt_申请.Rows.Add(r_z);
        //    }
        //    else
        //    {
        //        str_pa = dt_申请.Rows[0]["审核申请单号"].ToString();
        //        //decimal dec = Convert.ToDecimal(txt_cgshhje.Text);
        //        //dt_申请.Rows[0]["总金额"] = dec;

        //        dt_申请.Rows[0]["相关单位"] = txt_客户名称.Text;
        //        dt_申请.Rows[0]["待审核人ID"] = r_upper["工号"];
        //        dt_申请.Rows[0]["待审核人"] = r_upper["姓名"];
        //        dt_申请.Rows[0]["申请时间"] = t;
        //        dt_申请.Rows[0]["申请人ID"] = CPublic.Var.LocalUserID;
        //        dt_申请.Rows[0]["申请人"] = CPublic.Var.localUserName;

        //    }
        //    return dt_申请;
        //}
        //生效
        //private   void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    try
        //    {
        //        if (MessageBox.Show(string.Format("确定生效？"), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
        //        {

        //            gv.CloseEditor();
        //            gc.BindingContext[dtP].EndCurrentEdit();
        //            gv_待办.CloseEditor();
        //            gc_待办.BindingContext[dt_待办].EndCurrentEdit();
        //            gc_待办.BindingContext[dv_待办].EndCurrentEdit();

        //            fun_check();

        //            //fun_保存主表();
        //            //fun_保存明细();

        //            //fun_事务_保存();

        //           // fun_生效();
        //            //fun_改单价();

        //            //fun_强载();
        //            fun_载入待办();
        //            //生效完变成新增状态
        //            bl_新增or修改 = true;

        //            //fun_生效();
        //            MessageBox.Show("生效成功");

        //            fun_载入();

        //            //清空界面
        //            //fun_载入待办();
        //            fun_清空();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CZMaster.MasterLog.WriteLog(ex.Message, "开票界面_");
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        //关闭
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        #endregion

        #region 右键
        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gc, new Point(e.X, e.Y));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 查看物料明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gv_待办.GetDataRow(gv_待办.FocusedRowHandle);
            ERPStock.frm仓库物料数量明细 frm = new ERPStock.frm仓库物料数量明细(dr["物料编码"].ToString(), dr["仓库号"].ToString());
            CPublic.UIcontrol.AddNewPage(frm, "物料明细");
        }
        #endregion

        #region 修改金额
        private void gv_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Value.ToString() != "")
            {
                Decimal dec税率 = Convert.ToDecimal(textBox2.Text.ToString()) / 100;
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (e.Column.Caption == "含税单价" || e.Column.Caption == "开票数量")
                {
                    if (Convert.ToDecimal(dr["开票税后单价"]) >= (Decimal)0)
                    {
                        {
                            dr["开票税前单价"] = Math.Round((Convert.ToDecimal(dr["开票税后单价"]) / ((Decimal)1 + dec税率)), 6);
                        }
                    }
                    //fun_明细金额变化();
                }
                if (e.Column.Caption == "不含税单价")
                {
                    if (Convert.ToDecimal(dr["开票税前单价"]) >= (Decimal)0)
                    {
                        {
                            dr["开票税后单价"] = Math.Round((Convert.ToDecimal(dr["开票税前单价"]) * ((Decimal)1 + dec税率)), 6);
                        }
                    }
                    //fun_明细金额变化();
                }
                else if (e.Column.FieldName == "开票税前金额")
                {
                    dr["开票税前单价"] =Math.Round( Convert.ToDecimal(e.Value) / Convert.ToDecimal(dr["开票数量"]),6,MidpointRounding.AwayFromZero);

                    dr["开票税后单价"] = Math.Round(Convert.ToDecimal(dr["开票税前单价"]) * (1 + dec税率), 6, MidpointRounding.AwayFromZero);

                    dr["开票税后金额"] =Math.Round(Convert.ToDecimal(dr["开票税后单价"]) * Convert.ToDecimal(dr["开票数量"]),2,MidpointRounding.AwayFromZero);

                }
                else if (e.Column.FieldName == "开票税后金额")
                {
                    dr["开票税后单价"] =Math.Round(Convert.ToDecimal(e.Value) / Convert.ToDecimal(dr["开票数量"]),6,MidpointRounding.AwayFromZero);

                    dr["开票税前单价"] = Math.Round(Convert.ToDecimal(Convert.ToDecimal(dr["开票税后单价"]) / (1 + dec税率)),6,MidpointRounding.AwayFromZero);

                    dr["开票税前金额"] =Math.Round(Convert.ToDecimal(dr["开票税前单价"]) * Convert.ToDecimal(dr["开票数量"]),2,MidpointRounding.AwayFromZero);

                }

                fun_明细金额变化();
            }
        }

        //计算明细金额，以及总金额
        private void fun_明细金额变化(Boolean blErr = false)
        {
            System.Decimal sum = 0;
            System.Decimal sum1 = 0;
            foreach (DataRow r in dtP.Rows)
            {
                if (r.RowState == DataRowState.Deleted)
                {
                    continue;
                }
                try
                {
                    r["开票税后金额"] = Math.Round((Decimal)r["开票税后单价"] * (Decimal)r["开票数量"],2,MidpointRounding.AwayFromZero);
                    sum += (Decimal)r["开票税后金额"];
                    r["开票税前金额"] = Math.Round((Decimal)r["开票税前单价"] * (Decimal)r["开票数量"],2,MidpointRounding.AwayFromZero);
                    sum1 += (Decimal)r["开票税前金额"];
                }
                catch
                {
                    if (blErr)
                    {
                        throw new Exception(string.Format("{0}的单价或物料出错！", r["物料名称"].ToString()));
                    }
                }
            }

            txt_开票税前金额.Text = sum1.ToString("0.00");
            txt_开票税后金额.Text = sum.ToString("0.00");
        }
        #endregion

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                options.ExportMode = DevExpress.XtraPrinting.XlsxExportMode.SingleFile;
                gc_待办.ExportToXlsx(saveFileDialog.FileName, options);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gv_待办_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gv_待办_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc_待办, new Point(e.X, e.Y));
                gv_待办.CloseEditor();
                this.BindingContext[dt_待办].EndCurrentEdit();

            }

           




        }

        private void 关闭开票记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //
            if (MessageBox.Show("确认是否关闭选中记录", "警告！", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                DataRow dr = gv_待办.GetDataRow(gv_待办.FocusedRowHandle);
                DataView dv = new DataView(dt_待办);

                //dv.RowFilter ="选择=1";
                //DataTable dtM = dv.ToTable();
                //foreach (DataRow dr in dtM.Rows)
                //{
                string sql = string.Format(@"update   L销售记录成品出库单明细表L  set 作废=1,作废日期='{0}',作废人='{1}'  where 成品出库单明细号 ='{2}'"
                                                , CPublic.Var.getDatetime(), CPublic.Var.localUserName, dr["成品出库单明细号"]);
                CZMaster.MasterSQL.ExecuteSQL(sql, strconn);
                string sql_1 = string.Format(@"update   销售记录成品出库单明细表  set 作废=1,作废日期='{0}',作废人='{1}'  where 成品出库单明细号 ='{2}'"
                                                 , CPublic.Var.getDatetime(), CPublic.Var.localUserName, dr["成品出库单明细号"]);
                CZMaster.MasterSQL.ExecuteSQL(sql_1, strconn);
                dt_待办.Rows.Remove(dr);
                //}
                MessageBox.Show("ok");
            }

        }

        private void gv_待办_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv_待办.GetFocusedRowCellValue(gv_待办.FocusedColumn));
                e.Handled = true;
            }
        }

        private void gv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv.GetFocusedRowCellValue(gv.FocusedColumn));
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //double d = 0;
            //for (int i = 0; i < gv_待办.DataRowCount; i++)
            //{
            //    d = d +Convert.ToDouble(gv_待办.GetDataRow(i)["税后金额"]);
            //}
            //textBox1.Text = d.ToString() ;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            for (int i = 0; i < gv_待办.DataRowCount; i++)
            {
                gv_待办.GetDataRow(i)["选择"] = true;

                gv_待办.FocusedRowHandle = gv_待办.LocateByDisplayText(0, gridColumn2, gv_待办.GetDataRow(i)["成品出库单明细号"].ToString());
                repositoryItemCheckEdit1_CheckedChanged(null, null);
            }
        }

        private void gv_待办_ColumnWidthChanged(object sender, DevExpress.XtraGrid.Views.Base.ColumnEventArgs e)
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gv_待办.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void gv_待办_ColumnPositionChanged(object sender, EventArgs e)
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gv_待办.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (textBox3.Text == "")
                {
                    throw new Exception("开票通知单号不能为空，请先保存");
                }
                using (SqlDataAdapter da = new SqlDataAdapter("select * from 销售记录销售开票主表 where 销售开票通知单号='" + textBox3.Text + "'", strconn))
                {
                    DataTable dt_提交审核 = new DataTable();
                    da.Fill(dt_提交审核);
                    dt_提交审核.Rows[0]["提交审核"] = true;
                    dt_提交审核.Rows[0]["提交人"] = CPublic.Var.localUserName;
                    dt_提交审核.Rows[0]["提交人ID"] = CPublic.Var.LocalUserID;
                    dt_提交审核.Rows[0]["提交日期"] = CPublic.Var.getDatetime();
                    new SqlCommandBuilder(da);
                    da.Update(dt_提交审核);


                    MessageBox.Show("提交审核成功！");

                }


                if (MessageBox.Show("是否打印开票单据？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    string sql = string.Format("select * from  销售记录销售开票主表 where 销售开票通知单号= '{0}'", textBox3.Text);
                    DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql,strconn);
                    DataRow drM = dt.Rows[0];

                     sql = string.Format("select * from  销售记录销售开票明细表 where 销售开票通知单号= '{0}'", textBox3.Text);
                    DataTable dtm = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                   // DataTable dtm = (DataTable)this.gc.DataSource;
                    Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
                    Type outerForm = outerAsm.GetType("ERPreport.销售开票", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
                   // CPublic.UIcontrol.Showpage(ui, t.Rows[0]["打开界面名称"].ToString());
                    object[] drr = new object[2];

                    drr[0] = drM;
                    drr[1] = dtm;
                    //   drr[2] = dr["出入库申请单号"].ToString();
                    Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                    //  UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
                    ui.ShowDialog();




                }










                }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //税率变化 
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal dec = 0;
                if (!decimal.TryParse(textBox2.Text, out dec))
                {
                    //return;
                    throw new Exception("输入内容有误");
                }
                Decimal dec税率 = Convert.ToDecimal(textBox2.Text.ToString()) / (decimal)100.00;
                // DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);

                if (dtP != null)
                {
                    foreach (DataRow drrrr in dtP.Rows)
                    {

                        if (Convert.ToDecimal(drrrr["开票税后单价"]) >= (Decimal)0)
                        {

                            drrrr["开票税前单价"] = Math.Round((Convert.ToDecimal(drrrr["开票税后单价"]) / ((Decimal)1 + dec税率)), 6);

                        }


                    }
                    fun_明细金额变化();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void gv_RowCellClick_1(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);

                gv_待办.FocusedRowHandle = gv_待办.LocateByDisplayText(0, gridColumn2, dr["成品出库单明细号"].ToString());
                if (e != null && e.Button == MouseButtons.Right)
                {
                    contextMenuStrip2.Show(gc, new Point(e.X, e.Y));
                    gv.CloseEditor();
                    this.BindingContext[dtP].EndCurrentEdit();

                }

                //DataRow dr1 = gv_待办.GetDataRow(gv_待办.FocusedRowHandle);
            }
            catch (Exception)
            {


            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                gv_待办.FocusedRowHandle = gv_待办.LocateByDisplayText(0, gridColumn2, dr["成品出库单明细号"].ToString());
                DataRow dr1 = gv_待办.GetDataRow(gv_待办.FocusedRowHandle);
                dr.Delete();
                dr1["选择"] = false;
                fun_明细金额变化();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try

            {
                DateTime time = CPublic.Var.getDatetime();
                string sql = string.Format("select * from 汇率维护表 where 年='{0}'and 月='{1}' and 币种='{2}' ", time.Year, time.Month, lookUpEdit1.Text.ToString());
                DataTable dt_bz = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                DataRow[] a = dt_bz.Select(string.Format("币种='{0}'", lookUpEdit1.Text));
                if (a.Length == 0) throw new Exception(string.Format("提示：{0}月汇率尚未维护",time.Month));
                textBox4.Text = a[0]["汇率"].ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txt_开票税前金额_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //判断按键是不是要输入的类型。
                if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                    e.Handled = true;

                //小数点的处理。
                if ((int)e.KeyChar == 46)                           //小数点
                {
                    if (txt_开票税前金额.Text.Length <= 0)
                        e.Handled = true;   //小数点不能在第一位
                    else
                    {
                        float f;
                        float oldf;
                        bool b1 = false, b2 = false;
                        b1 = float.TryParse(txt_开票税前金额.Text, out oldf);
                        b2 = float.TryParse(txt_开票税前金额.Text + e.KeyChar.ToString(), out f);
                        if (b2 == false)
                        {
                            if (b1 == true)
                                e.Handled = true;
                            else
                                e.Handled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txt_开票税后金额_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //判断按键是不是要输入的类型。
                if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                    e.Handled = true;

                //小数点的处理。
                if ((int)e.KeyChar == 46)                           //小数点
                {
                    if (txt_开票税后金额.Text.Length <= 0)
                        e.Handled = true;   //小数点不能在第一位
                    else
                    {
                        float f;
                        float oldf;
                        bool b1 = false, b2 = false;
                        b1 = float.TryParse(txt_开票税后金额.Text, out oldf);
                        b2 = float.TryParse(txt_开票税后金额.Text + e.KeyChar.ToString(), out f);
                        if (b2 == false)
                        {
                            if (b1 == true)
                                e.Handled = true;
                            else
                                e.Handled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
