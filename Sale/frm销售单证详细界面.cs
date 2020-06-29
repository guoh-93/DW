using CZMaster;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
namespace ERPSale
{
    public partial class frm销售单证详细界面 : UserControl
    {
        #region 成员
        /// <summary>
        /// 是否提交 如果已提交 不可以修改
        /// </summary>
        bool bl_istj = false;

        /// <summary>
        /// 新增：true；修改：false。
        /// </summary>
        Boolean bl_新增or修改 = true;

        string cfgfilepath = "";

        string CrmSo = "";

        /// <summary>
        /// 新增明细：dr = dtM.NewRow()；修改明细：dr = gv.GetDataRow(gv.FocusedRowHandle);
        /// </summary>
        DataRow dr = null;

        /// <summary>
        /// 新增订单：drM = dtM.NewRow()；修改订单：drM = gv.GetDataRow(gv.FocusedRowHandle);
        /// </summary>
        DataRow drM = null;

        DataTable dt_bom;

        // DataTable dt_物料_bom;
        DataTable dt_stock;

        DataTable dt_包装方式;

        DataTable dt_币种;

        DataTable dt_客户;

        DataTable dt_未完成 = new DataTable();

        /// <summary>
        /// 基础数据物料信息表,物料名称,物料编码
        /// </summary>
        DataTable dt_物料下拉框;

        /// <summary>
        /// 销售订单主表
        /// </summary>
        DataTable dtM = null;

        /// <summary>
        /// 明细表
        /// </summary>
        DataTable dtP = new DataTable();

        bool sj = false;

        string srt_预定单号 = "";

        string str_单据状态 = "";

        string str_销售订单号 = "";

        string strconn = CPublic.Var.strConn;

        //销售订单号
        string strSoNo = "";
        bool ypd = false;
        #endregion
        //20-6-1  备注8里面记录选择的包材配件包 的用量 或者 '标记'

        #region 自用类
        public frm销售单证详细界面()
        {
            InitializeComponent();
            bl_新增or修改 = true;
            fun_载入();
        }
        public frm销售单证详细界面(DataRow dr, DataTable dt)
        {
            InitializeComponent();
            bl_新增or修改 = true;
            drM = dr;
            dtM = dt;
        }
        public frm销售单证详细界面(string ssssss, DataRow dr, DataTable dt, bool xs订单)
        {
            InitializeComponent();
            bl_新增or修改 = true;
            srt_预定单号 = ssssss;
            sj = xs订单;
            DataTable drnn = dr.Table.Clone();
            string sql = "select * from 销售记录销售订单主表 where 1<>1";
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

            drM = dtM.NewRow();
            dtM.Rows.Add(drM);
            foreach (DataColumn dc in dr.Table.Columns)

            {
                foreach (DataColumn dcp in drM.Table.Columns)
                {
                    if (dc.ColumnName == dcp.ColumnName)
                    {
                        drM[dcp.ColumnName] = dr[dc.ColumnName];
                    }
                }


            }
            drM["客户名"] = dr["客户名称"];
            drM["汇率"] = 1;
            drM["币种"] = "人民币";
            // = drasd;

            //DataRow dewr = dtM.NewRow();
            //dtM.Rows.Add(dewr);
            drM["销售预订单号"] = srt_预定单号;
            drM["guid"] = System.Guid.NewGuid();
            drM["审核"] = false;
            drM["审核日期"] = DBNull.Value;



        }

        public frm销售单证详细界面(string s_销售订单号, DataRow dr, DataTable dt)
        {
            InitializeComponent();
            bl_新增or修改 = false;

            str_销售订单号 = s_销售订单号;
            drM = dr;
            dtM = dt;
            if (dr["销售预订单号"].ToString() != "")
            {
                ypd = true;
            }
            if (Convert.ToBoolean(drM["待审核"]))
            {
                string sql = string.Format("select * from 单据审核申请表 where 关联单号 = '{0}'", drM["销售订单号"].ToString());
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                DataTable dt1111 = new DataTable();
                da.Fill(dt1111);
                if (dt1111.Rows[0]["待审核人ID"].ToString() == CPublic.Var.LocalUserID)
                {
                    bl_istj = false;
                    barLargeButtonItem7.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                }
                else
                {
                    bl_istj = true;
                }

            }

        }

        public frm销售单证详细界面(object 销售订单号, object dr, object dt)
        {
            InitializeComponent();
            bl_新增or修改 = false;
            str_销售订单号 = (string)销售订单号;
            ypd = true;
            drM = (DataRow)dr;
            dtM = (DataTable)dt;
            if (Convert.ToBoolean(drM["待审核"]))
            {
                string sql = string.Format("select * from 单据审核申请表 where 关联单号 = '{0}'", drM["销售订单号"].ToString());
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                DataTable dt1111 = new DataTable();
                da.Fill(dt1111);
                if (dt1111.Rows[0]["待审核人ID"].ToString() == CPublic.Var.LocalUserID)
                {
                    bl_istj = false;
                    barLargeButtonItem7.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                }
                else
                {
                    bl_istj = true;
                }

            }
            timer1.Start();
        }


        private void frm销售单证详细界面_Load(object sender, EventArgs e)
        {
            string sql1 = "";
            if (CPublic.Var.LocalUserTeam == "管理员权限" || CPublic.Var.LocalUserID == "admin")
            {
                sql1 = @" with tt as ( select 销售预订单明细号, sum(数量)数量 from (
                              select   销售预订单明细号, sum(数量)数量 from 销售记录销售订单明细表 aa
                              left join 销售记录销售订单主表 bb on aa.销售订单号 = bb.销售订单号
                              where bb.作废= 0 and bb.关闭= 0 and bb.完成= 0 and bb.审核= 0 and bb.待审核= 0 and bb.生效 = 0
                              and aa.作废= 0 and aa.关闭= 0 and aa.明细完成= 0 and aa.生效= 0 and 销售预订单明细号 <>''   group by 销售预订单明细号
                              union   
                              select 销售预订单明细号,sum(申请数量)数量 from 借还申请表附表 cc 
                              left join 借还申请表 dd on cc.申请批号 = dd.申请批号
                              where dd.作废 = 0 and dd.审核=0 and dd.提交审核 = 0 and cc.作废=0 and 销售预订单明细号 <>''
                              group by 销售预订单明细号) aaa group by 销售预订单明细号) 

                              select a.*,b.部门名称,b.备注 as 表头备注,b.制单人,b.业务员 ,isnull(c.数量,0)锁定数量,b.制单日期 from 销售预订单明细表 a
                              left join 销售预订单主表 b on a.销售预订单号 = b.销售预订单号
                              left join tt c on a.销售预订单明细号 = c.销售预订单明细号
                              where a.作废 = 0 and a.完成 = 0 and a.关闭 = 0 and b.作废 = 0 and b.审核 = 1 and b.关闭 = 0 and b.完成 = 0 ";
            }
            else
            {
                sql1 = string.Format(@"  with tt as ( select 销售预订单明细号, sum(数量)数量 from (
                              select   销售预订单明细号, sum(数量)数量 from 销售记录销售订单明细表 aa
                              left join 销售记录销售订单主表 bb on aa.销售订单号 = bb.销售订单号
                              where bb.作废= 0 and bb.关闭= 0 and bb.完成= 0 and bb.审核= 0 and bb.待审核= 0 and bb.生效 = 0
                              and aa.作废= 0 and aa.关闭= 0 and aa.明细完成= 0 and aa.生效= 0 and 销售预订单明细号 <>''   group by 销售预订单明细号
                              union   
                              select 销售预订单明细号,sum(申请数量)数量 from 借还申请表附表 cc 
                              left join 借还申请表 dd on cc.申请批号 = dd.申请批号
                              where dd.作废 = 0 and dd.审核=0 and dd.提交审核 = 0 and cc.作废=0 and 销售预订单明细号 <>''
                              group by 销售预订单明细号) aaa group by 销售预订单明细号) 

                              select a.*,b.部门名称,b.备注 as 表头备注,b.制单人,b.业务员 ,isnull(c.数量,0)锁定数量,b.制单日期 from 销售预订单明细表 a
                              left join 销售预订单主表 b on a.销售预订单号 = b.销售预订单号
                              left join tt c on a.销售预订单明细号 = c.销售预订单明细号
                              where a.作废 = 0 and a.完成 = 0 and a.关闭 = 0 and b.作废 = 0 and b.审核 = 1 and b.关闭 = 0 and b.完成 = 0  and 部门名称 = '{0}' ", CPublic.Var.localUser部门名称);
            }
            dt_未完成 = CZMaster.MasterSQL.Get_DataTable(sql1, strconn);

            DataColumn dc = new DataColumn("选择", typeof(bool));
            dc.DefaultValue = false;
            dt_未完成.Columns.Add(dc);
            dt_未完成.Columns.Add("可转数量", typeof(decimal));
            DataColumn[] pk = new DataColumn[2];
            pk[0] = dt_未完成.Columns["销售预订单号"];
            pk[1] = dt_未完成.Columns["销售预订单明细号"];
            dt_未完成.PrimaryKey = pk;
            foreach (DataRow dr1 in dt_未完成.Rows)
            {
                dr1["可转数量"] = Convert.ToDecimal(dr1["未转数量"]) - Convert.ToDecimal(dr1["锁定数量"]);

            }
            //devGridControlCustom1.UserName = CPublic.Var.LocalUserID;
            //devGridControlCustom1.strConn = CPublic.Var.strConn;
            //barLargeButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";

            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(panel4, this.Name, cfgfilepath);
            txt_录入人员.Text = CPublic.Var.localUserName;

            load_dropDownList();

            fun_载入订单();
            fun_载入明细();

            //fun_配件包();
            if (bl_istj)
            {

                string s = string.Format("select * from 销售记录销售订单主表 where 销售订单号='{0}'", txt_销售订单号.Text);
                DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                if (t.Rows.Count > 0)
                {
                    if (Convert.ToBoolean(t.Rows[0]["审核"]))
                    {

                        str_单据状态 = "已审核";
                        label27.Visible = true;
                        label27.Text = str_单据状态;

                    }
                    else
                    {
                        str_单据状态 = "审核中";
                        label27.Visible = true;
                        label27.Text = str_单据状态;
                        barLargeButtonItem8.Enabled = true;
                    }
                }
                fun_编辑();

            }

            if (checkBox1.Checked == true)
            {
                button4.Enabled = true;
                button2.Enabled = true;
            }
            else
            {
                button4.Enabled = false;
                button2.Enabled = false;
            }
            strSoNo = txt_销售订单号.Text.ToString();
            txt_日期.EditValue = CPublic.Var.getDatetime();

            //dtP.ColumnChanged += dtP_ColumnChanged;


        }

        private void fun_编辑()
        {
            try
            {
                if (bl_istj)
                {
                    string s = string.Format("select * from 销售记录销售订单主表 where 销售订单号='{0}'", txt_销售订单号.Text);
                    DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                    if (t.Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(t.Rows[0]["审核"]))
                        {

                            str_单据状态 = "已审核";
                            label27.Visible = true;
                            label27.Text = str_单据状态;

                        }
                        else
                        {
                            str_单据状态 = "审核中";
                            label27.Visible = true;
                            label27.Text = str_单据状态;
                        }
                    }
                }
                else
                {
                    str_单据状态 = "";
                    label27.Visible = false;
                }
                // barLargeButtonItem6.Enabled = !bl_istj;              
                barLargeButtonItem2.Enabled = !bl_istj;
                barLargeButtonItem7.Enabled = !bl_istj;
                txt_订单方式.Enabled = !bl_istj;
                SL_片区.Enabled = !bl_istj;
                txt_业务员.Enabled = !bl_istj;
                searchLookUpEdit1.Enabled = !bl_istj;
                txt_客户订单号.Enabled = !bl_istj;
                cd_账期.Enabled = !bl_istj;
                comboBox3.Enabled = !bl_istj;
                txt_销售备注.Enabled = !bl_istj;
                txt_税率.Enabled = !bl_istj;
                txt_税前金额.Enabled = !bl_istj;
                txt_金额.Enabled = !bl_istj;
                comboBox2.Enabled = !bl_istj;
                comboBox1.Enabled = !bl_istj;
                searchLookUpEdit3.Enabled = !bl_istj;
                txt_日期.Enabled = !bl_istj;
                textBox4.Enabled = !bl_istj;
                comboBoxEdit1.Enabled = !bl_istj;
                button1.Enabled = !bl_istj;
                button3.Enabled = !bl_istj;
                checkBox1.Enabled = !bl_istj;
                barLargeButtonItem3.Enabled = !bl_istj;
                simpleButton2.Enabled = !bl_istj;
                simpleButton3.Enabled = !bl_istj;
                button6.Enabled = !bl_istj;
                //repositoryItemSearchLookUpEdit1.ReadOnly = true;
                //repositoryItemSearchLookUpEdit2.ReadOnly = true;
                gv.OptionsBehavior.Editable = !bl_istj;

            }
            catch { }
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
                    Decimal dec税率 = Convert.ToDecimal(Convert.ToInt32(txt_税率.EditValue) / (Decimal)100);
                    if (r["数量"].ToString() != "" && r["税后单价"].ToString() != "")
                    {
                        r["税后金额"] = Math.Round((Decimal)r["税后单价"] * (Decimal)r["数量"], 2, MidpointRounding.AwayFromZero);
                        sum += (Decimal)r["税后金额"];

                        r["税前单价"] = Math.Round((Convert.ToDecimal(r["税后单价"]) / ((Decimal)1 + dec税率)), 2, MidpointRounding.AwayFromZero);
                        r["税前金额"] = Math.Round((Decimal)r["税前单价"] * (Decimal)r["数量"], 2, MidpointRounding.AwayFromZero);
                        sum1 += (Decimal)r["税前金额"];
                    }
                }
                catch
                {
                    if (blErr)
                    {
                        throw new Exception(string.Format("{0}的单价或物料出错！", r["物料名称"].ToString()));
                    }
                }
            }
            txt_税前金额.Text = sum1.ToString("#0.####");
            txt_金额.Text = sum.ToString("#0.####");
        }

        //        private void fun_配件包()
        //        {
        //            try
        //            {
        //                string sql_pjb = @" select b.物料编码,b.物料名称 ,b.规格型号,
        //                                          b.物料类型,b.大类,b.小类 from (select 父项编码 from 配件包 group by 父项编码) a
        //                                          left join 基础数据物料信息表 b on a.父项编码  = b.物料编码";
        //                DataTable dt_配件包 = CZMaster.MasterSQL.Get_DataTable(sql_pjb, strconn);
        //                //searchLookUpEdit3.Properties.DataSource = dt_配件包;
        //                //searchLookUpEdit3.Properties.ValueMember = "物料编码";
        //                //searchLookUpEdit3.Properties.DisplayMember = "物料编码";
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show(ex.Message);
        //            }
        //        }
        private void fun_物料下拉框()
        {
            string sql_bom = @"select  基础数据物料信息表.物料编码,新数据,原ERP物料编号,基础数据物料信息表.物料名称,基础数据物料信息表.图纸编号,大类,n原ERP规格型号,stock.仓库号,stock.仓库名称,库存总数 
  ,货架描述 from  基础数据物料信息表  left join 仓库物料数量表 stock  on 基础数据物料信息表.物料编码=stock.物料编码 
  where 基础数据物料信息表.物料编码 in (select 物料编码 from 基础数据物料信息表 
left  join (select  产品编码,COUNT(*)总 from  基础数据物料BOM表 group by 产品编码)z on z.产品编码=基础数据物料信息表.物料编码
left  join  (select 产品编码,COUNT(可售)可售 from ( 
select  基础数据物料BOM表.*,可售 from  基础数据物料BOM表,基础数据物料信息表 where 基础数据物料BOM表.子项编码= 基础数据物料信息表.物料编码)a where  a.可售=1 
group by 产品编码)b on  基础数据物料信息表.物料编码=b.产品编码 where z.总=b.可售)";
            dt_bom = CZMaster.MasterSQL.Get_DataTable(sql_bom, strconn);

            searchLookUpEdit2.Properties.DataSource = dt_bom;
            searchLookUpEdit2.Properties.ValueMember = "物料编码";
            searchLookUpEdit2.Properties.DisplayMember = "原ERP物料编号";

        }
        //private void dtP_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        //{
        //try
        //{
        //    Decimal dec税率 = Convert.ToDecimal(Convert.ToInt32(txt_税率.EditValue) / (Decimal)100);
        //    if (e.Column.ColumnName == "物料编码")
        //    {
        //        dtP.ColumnChanged -= dtP_ColumnChanged;
        //        string ss = e.Row["物料编码"].ToString();
        //        DataRow[] ds = dt_物料下拉框.Select(string.Format("物料编码 = '{0}'", ss));
        //        try
        //        {


        //            e.Row["新数据"] = ds[0]["新数据"].ToString();
        //            e.Row["物料名称"] = ds[0]["物料名称"].ToString();
        //            e.Row["计量单位"] = ds[0]["计量单位"].ToString();
        //          //  e.Row["n原ERP规格型号"] = ds[0]["n原ERP规格型号"].ToString();
        //            e.Row["规格型号"] = ds[0]["规格型号"].ToString();
        //            e.Row["特殊备注"] = ds[0]["特殊备注"].ToString();
        //            //Decimal dec税率 = Convert.ToDecimal(Convert.ToInt32(txt_税率.EditValue) / (Decimal)100);
        //            try
        //            {
        //                decimal dec = 0;
        //                dec = fun_客户物料单价(ds[0]);
        //                e.Row["税后单价"] = dec;
        //                //  e.Row["税后单价"] = fun_明细金额(ds[0]).ToString("0.000000");
        //                //e.Row["税前单价"] = (fun_明细金额(ds[0]) / ((Decimal)1 + dec税率)).ToString("0.000000");
        //                e.Row["税前单价"] = dec / ((Decimal)1 + dec税率);
        //            }
        //            catch
        //            {
        //                //产品标准单价   5/18 改为销售单价 
        //                e.Row["税后单价"] = (Convert.ToDecimal(ds[0]["n销售单价"])).ToString("0.000000");
        //                e.Row["税前单价"] = (Convert.ToDecimal(ds[0]["n销售单价"]) / ((Decimal)1 + dec税率)).ToString("0.000000");
        //            }
        //            dtP.ColumnChanged += dtP_ColumnChanged;

        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception(ss + " 该物料不可售");
        //        }
        //    }
        //}
        //catch (Exception ex)
        //{

        //    MessageBox.Show(ex.Message);

        //}
        //}
        //        private void txt_客户编号_EditValueChanged(object sender, EventArgs e)
        //        {
        //            try
        //            {
        //                DataRow[] ds = dt_属性.Select(string.Format("客户编号 = '{0}'", searchLookUpEdit1.EditValue));
        //                if (ds.Length != 0)
        //                {
        //                    txt_客户名称.Text = ds[0]["客户名称"].ToString();
        //                    txt_客户负责人.Text = ds[0]["联系人"].ToString();
        //                    txt_电话号码.Text = ds[0]["手机"].ToString();
        //                    txt_业务员.Text = ds[0]["业务员"].ToString();

        //                }
        //                if (searchLookUpEdit1.EditValue.ToString() == "")
        //                {
        //                    txt_客户名称.Text = "";
        //                    txt_客户负责人.Text = "";
        //                    txt_电话号码.Text = "";
        //                    txt_业务员.Text = "";
        //                }
        //                //                string sqlll = @"select 产品对应关系表.客户料号,产品对应关系表.客户规格型号,产品对应关系表.税前价格,产品对应关系表.税后价格,a.* 
        //                //  from 产品对应关系表
        //                //  right join 
        //                //  (select 基础数据物料信息表.物料名称,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.物料编码,基础数据物料信息表.规格,基础数据物料信息表.n原ERP规格型号,
        //                //  基础数据物料信息表.计量单位,基础数据物料信息表.标准单价,基础数据物料信息表.特殊备注,
        //                //  仓库物料数量表.有效总数,仓库物料数量表.库存总数,仓库物料数量表.在制量,仓库物料数量表.受订量
        //                //  from 基础数据物料信息表 join 仓库物料数量表 
        //                //  on 基础数据物料信息表.物料编码 = 仓库物料数量表.物料编码
        //                //  where (基础数据物料信息表.物料类型 = '成品' or 基础数据物料信息表.物料类型 = '半成品'or 基础数据物料信息表.物料类型 = '可售原材料') and 基础数据物料信息表.停用 = 0) a
        //                //  on 产品对应关系表.产品编号 = a.物料编码";

        //                string sqlll = @"select 产品对应关系表.客户料号,产品对应关系表.客户规格型号,产品对应关系表.税前价格,产品对应关系表.税后价格,a.* 
        //  from 产品对应关系表
        //  right join 
        //  (select 基础数据物料信息表.物料名称,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.物料编码,基础数据物料信息表.规格,基础数据物料信息表.n原ERP规格型号,
        //  基础数据物料信息表.计量单位,基础数据物料信息表.标准单价,基础数据物料信息表.n销售单价,基础数据物料信息表.特殊备注,
        //  仓库物料数量表.有效总数,仓库物料数量表.库存总数,仓库物料数量表.在制量,仓库物料数量表.受订量
        //  from 基础数据物料信息表 join 仓库物料数量表 
        //  on 基础数据物料信息表.物料编码 = 仓库物料数量表.物料编码
        //  where 基础数据物料信息表.可售=1 and 基础数据物料信息表.停用 = 0) a
        //  on 产品对应关系表.产品编号 = a.物料编码";
        //                SqlDataAdapter dad = new SqlDataAdapter(sqlll, strconn);
        //                dt_物料下拉框.Clear();
        //                dad.Fill(dt_物料下拉框);

        //            }
        //            catch (Exception ex)
        //            {
        //                CZMaster.MasterLog.WriteLog(ex.Message, "销售单界面_txt_客户编号_EditValueChanged");
        //            }
        //        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {

        }

        //计算税前金额
        private void txt_金额_TextChanged(object sender, EventArgs e)
        {
            //if (txt_金额.Text.ToString() != "")
            //{
            //    Decimal de_金额 = Convert.ToDecimal(txt_金额.Text.ToString());
            //    try
            //    {
            //        Decimal dec税率 = Convert.ToDecimal(Convert.ToInt32(txt_税率.EditValue) / (Decimal)100);
            //        txt_税前金额.Text = (de_金额 / ((Decimal)1 + dec税率)).ToString("0.000000");
            //    }
            //    catch 
            //    {
            //        txt_税前金额.Text = "0.000000";
            //    }
            //}
            //else
            //{
            //    txt_税前金额.Text = "0.000000";
            //}
            //if (Convert.ToDecimal(txt_税前金额.Text) > Convert.ToDecimal(200000))
            //{
            //    MessageBox.Show("是否走评审流程？", "询问？", MessageBoxButtons.OKCancel);
            //}
        }
        #endregion

        #region 方法
        DataView dv_bzfs;

        DataTable t_安装配件包;

        private void fun_Check_明细()
        {
            DateTime t = CPublic.Var.getDatetime().Date;
            ERPorg.Corg cg = new ERPorg.Corg();
            string ss = "";
            foreach (DataRow r in dtP.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;
                if (r["送达日期"].ToString() == "")
                {
                    throw new Exception("缺少送达日期！");
                }
                if (Convert.ToDateTime(r["送达日期"]) < t) throw new Exception("送达日期不可小于当天");
                r["预计发货日期"] = r["送达日期"];
                if (Convert.ToDecimal(r["数量"].ToString()) <= 0)
                {
                    throw new Exception(string.Format("订单数量不可以小于等于0,物料:{0}！", r["物料编码"]));
                }
                if (r["GUID"].ToString() == "") r["GUID"] = System.Guid.NewGuid();
                if (r["包装方式编号"] == null) { r["包装方式编号"] = ""; r["包装方式"] = ""; }
                if (r["安装配件包编号"] == null) r["安装配件包编号"] = "";

                //2020-5-31 暂不限制 只提示
                //if (r["物料编码"].ToString().StartsWith("10") && r["包装方式编号"].ToString() == "")
                //{

                //    throw new Exception("包装方式未选择,请确认");
                //}
                ////物料是否有效
                //string sql_物料是否有效 = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"].ToString());
                //SqlDataAdapter da = new SqlDataAdapter(sql_物料是否有效, strconn);
                //DataTable dt_基础物料 = new DataTable();
                //da.Fill(dt_基础物料);
                //if (dt_基础物料.Rows.Count == 0)
                //{
                //    throw new Exception(string.Format("物料'{0}'无效，基础数据物料信息表中不存在该物料信息", r["物料编码"].ToString()));
                //}

                //物料是否初始化
                //string sql_物料是否初始化 = string.Format("select * from 仓库物料表 where 物料编码 = '{0}'", r["物料编码"].ToString());
                //SqlDataAdapter da_物料是否初始化 = new SqlDataAdapter(sql_物料是否初始化, strconn);
                //DataTable dt_物料是否初始化 = new DataTable();
                //da_物料是否初始化.Fill(dt_物料是否初始化);
                //if (dt_物料是否初始化.Rows.Count == 0)
                //{
                //    throw new Exception(string.Format("物料'{0}'无效，仓库物料表中不存在该物料信息", r["物料编码"].ToString()));
                //}
                //// 新产品 是否有BOM
                DataRow[] rr = dt_物料下拉框.Select(string.Format("物料编码='{0}'", r["物料编码"].ToString()));
                if (rr.Length == 0) throw new Exception("物料不存在");//正常不可能会发生

                else if (Convert.ToBoolean(rr[0]["自制"]))
                {
                    string sql_新 = string.Format("select 子项编码 from 基础数据物料BOM表 where 产品编码='{0}'", r["物料编码"].ToString());
                    DataTable dt_x = CZMaster.MasterSQL.Get_DataTable(sql_新, strconn);
                    if (dt_x.Rows.Count == 0)
                    {
                        throw new Exception(string.Format("物料'{0}'属性为自制尚无BOM信息,请联系开发部维护BOM后再下销售订单", r["物料编码"].ToString()));
                    }
                }
                if (r["销售预订单明细号"].ToString() == "")
                {
                    if (txt_销售订单号.Text != "")
                    {
                        string sql_1 = $"select * from 销售记录销售订单主表 where 销售订单号 = '{txt_销售订单号.Text}'";
                        DataTable dtt = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);
                        if (dtt.Rows.Count > 0)
                        {
                            DataRow[] dr_ymx = dt_未完成.Select(string.Format("物料编码= '{0}' and 可转数量>0 and 制单日期<'{1}'", r["物料编码"], Convert.ToDateTime(dtt.Rows[0]["创建日期"])));
                            if (dr_ymx.Length > 0)
                            {
                                throw new Exception(r["物料编码"].ToString() + "物料存在未完成的预订单，请从预订单转订单");
                            }
                        }

                    }
                    else
                    {
                        DataRow[] dr_ymx = dt_未完成.Select(string.Format("物料编码= '{0}' and 可转数量>0", r["物料编码"]));
                        if (dr_ymx.Length > 0)
                        {
                            throw new Exception(r["物料编码"].ToString() + "物料存在未完成的预订单，请从预订单转订单");
                        }
                    }

                }

                bool bl_停产 = cg.determ_stop_product(r["物料编码"].ToString());

                if (bl_停产)
                {
                    if (ss != "") ss += "," + r["物料编码"].ToString();
                    else ss += r["物料编码"].ToString();
                }
            }
            if (ss != "")
            {
                ss += "子项中有已停产或将停产物料,是否确认继续下单";
                if (MessageBox.Show(ss, "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                { }
                else
                {
                    throw new Exception("已取消");
                }
            }


            //MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
            //DataTable temp = new DataTable();
            //temp = RBQ.SelectGroupByInto("", dtP, "物料编码,备注,count(数量) 次数", "", "物料编码,备注");
            //DataView dv = new DataView(temp);
            //dv.RowFilter = "次数>1";
            //if (dv.Count > 0) throw new Exception("有相同明细包括备注也一样,请检查");
        }

        /// <summary>
        /// 保存前，相关的重要数据需要CHECK DATA，如果CHECK出问题，给出合适的提示
        /// </summary>
        private void fun_Check_主表()
        {
            try
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("客户不能为空");
                }
                if (txt_日期.EditValue == null)
                {
                    throw new Exception("日期为空");
                }
                if (cd_账期.Text == "")
                {
                    throw new Exception("帐期为空");
                }
                if (txt_业务员.Text == "")
                {
                    throw new Exception("业务员为空");
                }
                if (txt_销售备注.Text == "")
                {
                    txt_销售备注.Text = " ";
                }
                //if (txt_开票状态.Text == "")
                //{
                //    txt_开票状态.Text = " ";
                //}
                if (txt_订单方式.Text == "")
                {
                    txt_订单方式.Text = " ";
                }
                if (drM["文件GUID"].ToString() != "")
                {
                    drM["订单原件"] = true;
                }
                else
                {
                    drM["订单原件"] = false;
                }

                if (comboBox5.Text == "")
                {
                    throw new Exception("销售单类型");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





        private void fun_保存订单()
        {
            try
            {
                DateTime time = CPublic.Var.getDatetime();



                if (bl_新增or修改 == true)
                {
                    drM["GUID"] = System.Guid.NewGuid();
                    drM["创建日期"] = time;
                    drM["部门编号"] = CPublic.Var.localUser部门编号;
                    drM["销售部门"] = CPublic.Var.localUser部门名称;
                    drM["录入人员ID"] = CPublic.Var.LocalUserID;
                    drM["录入人员"] = CPublic.Var.localUserName;
                }

                //if (sj==true)
                //{
                //    drM["销售预定单号"] = srt_预定单号;

                //}
                try
                {
                    //drM["录入人员"] = CPublic.Var.localUserName;
                    //drM["录入人员ID"] = CPublic.Var.LocalUserID;
                    drM["修改日期"] = time;
                    drM["日期"] = txt_日期.EditValue;
                    drM["合同号"] = searchLookUpEdit3.EditValue.ToString();
                    drM["合同名称"] = searchLookUpEdit3.Text.ToString();
                    if (comboBox5.Text.ToString().Equals("国内"))
                    {
                        drM["国内"] = true;
                    }
                    else if (comboBox5.Text.ToString().Equals("国外"))
                    {
                        drM["国外"] = true;
                    }
                    else
                    {
                        drM["国内"] = false;
                        drM["国外"] = false;

                    }
                    //if (CPublic.Var.localUser部门编号 == "00010301")   //为区分 不是 销售部门下的单子   销售部门分为一部二部 直接判断是不是制造部下的
                    //{
                    //    drM["备注10"] = "计划下单";
                    //}

                    dataBindHelper1.DataToDR(drM);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售单界面保存订单");
                throw new Exception(ex.Message);
            }
        }

        private void fun_保存明细()
        {
            try
            {
                int i = 1;
                DateTime t = CPublic.Var.getDatetime();
                //string str = "";
                foreach (DataRow r in dtP.Rows)
                {
                    if (r.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    //str= str+StockCore.StockCorer.fun_flag(r_x["物料编码"].ToString(),true);

                    r["POS"] = i++;
                    r["销售订单号"] = strSoNo;
                    r["销售订单明细号"] = strSoNo + "-" + Convert.ToInt32(r["POS"]).ToString("00");
                    r["客户"] = txt_客户名称.Text.ToString();
                    r["客户编号"] = searchLookUpEdit1.EditValue.ToString();
                    r["含税销售价"] = r["税后单价"];
                    r["税率"] = txt_税率.Text.ToString();
                    r["未完成数量"] = r["数量"].ToString();
                    r["未通知数量"] = r["数量"].ToString();
                    r["录入人员ID"] = CPublic.Var.LocalUserID;
                    r["修改日期"] = t;
                }
                //if (str.Trim() != "")
                //{
                //    throw new Exception(str);
                //}
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售单界面保存明细");
                throw new Exception("保存失败！" + ex.Message);
            }
        }

        //17-12-14 
        private Decimal fun_客户物料单价(DataRow r)
        {

            string sql = string.Format("select * from 客户产品单价表 where 客户编号 = '{0}' and 物料编码 = '{1}'", searchLookUpEdit1.EditValue.ToString(), r["物料编码"].ToString());
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            decimal de_单价 = 0;
            if (dt.Rows.Count > 0)
            {
                de_单价 = Convert.ToDecimal(dt.Rows[0]["单价"]);
            }
            else
            {

                throw new Exception("抛出异常使之取基础表的销售单价");
            }
            return de_单价;
        }

        //17-12-14 发现 这个一直没有用
        private Decimal fun_明细金额(DataRow r)
        {
            //有合同价使用合同价
            string sql = string.Format("select * from 产品对应关系表 where 客户编号 = '{0}' and 产品编号 = '{1}'", searchLookUpEdit1.EditValue.ToString(), r["物料编码"].ToString());
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            decimal de_金额 = 0;
            if (Convert.ToDecimal(dt.Rows[0]["税后价格"]) > (Decimal)0)
            {
                de_金额 = Convert.ToDecimal(dt.Rows[0]["税后价格"]);
            }
            else
            {
                de_金额 = Convert.ToDecimal(dt.Rows[0]["税前价格"]) * Convert.ToDecimal(1.17);
            }
            return de_金额;
        }

        private void fun_强载()
        {
            string sql = string.Format("select * from 销售记录销售订单主表 where 销售订单号 = '{0}'", txt_销售订单号.Text);
            dtM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            if (dtM.Rows.Count > 0)
            {
                drM = dtM.Rows[0];
                dataBindHelper1.DataFormDR(drM);
                searchLookUpEdit3.EditValue = drM["合同号"];
            }




            if (dtP != null)
            {
                dtP.Clear();
            }
            //if (bl_新增or修改 == false)
            {
                string sqll = string.Format(@"select 销售记录销售订单明细表.*,基础数据物料信息表.原ERP物料编号,新数据  from 销售记录销售订单明细表 
                left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 销售记录销售订单明细表.物料编码 
                where 销售订单号 = '{0}' order by  POS asc ", txt_销售订单号.Text);
                using (SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn))
                {
                    daa.Fill(dtP);
                    gc.DataSource = dtP;
                }
            }

        }

        private void fun_强载_补()
        {
            string sql = string.Format("select * from L销售记录销售订单主表L where 销售订单号 = '{0}'", txt_销售订单号.Text);
            dtM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            drM = dtM.Rows[0];
            dataBindHelper1.DataFormDR(drM);

            if (dtP != null)
            {
                dtP.Clear();
            }
            //if (bl_新增or修改 == false)
            {
                string sqll = string.Format(@"select L销售记录销售订单明细表L.*,基础数据物料信息表.原ERP物料编号,新数据  from L销售记录销售订单明细表L
                left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = L销售记录销售订单明细表L.物料编码 
                where 销售订单号 = '{0}' order by  POS asc ", txt_销售订单号.Text);
                using (SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn))
                {
                    daa.Fill(dtP);
                    gc.DataSource = dtP;
                }
            }

        }

        private void fun_清空()
        {
            txt_电话号码.Text = "";
            txt_订单方式.Text = "";
            txt_金额.Text = "";
            //txt_开票状态.Text = "";
            // txt_客户编号.Text = "";
            txt_客户负责人.Text = "";
            txt_客户名称.Text = "";
            txt_日期.EditValue = CPublic.Var.getDatetime();
            txt_税率.Text = "0";
            txt_销售备注.Text = "";
            txt_销售订单号.Text = "";
            txt_业务员.Text = "";
            txt_税前金额.Text = "";
            txt_录入人员.Text = CPublic.Var.localUserName;
            txt_客户订单号.Text = "";
            searchLookUpEdit1.EditValue = "";
            cd_账期.Text = "";
        }

        private void fun_生效()
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();
                fun_Check_明细();
                //主表生效
                drM["生效"] = 1;
                drM["生效日期"] = t;
                drM["修改日期"] = t;

                drM["生效人员ID"] = CPublic.Var.LocalUserID;
                drM["生效人员"] = CPublic.Var.localUserName;


                fun_保存订单();
                //明细生效
                foreach (DataRow r in dtP.Rows)
                {
                    //if (r["是否有箱贴"].Equals(true))
                    //{

                    //}

                    r["生效"] = 1;
                    r["生效日期"] = t;
                }
                fun_保存明细();
                if (comboBoxEdit1.EditValue != null && comboBoxEdit1.EditValue.ToString() == "补开")
                {
                    fun_事务_保存_补();
                    fun_直接生成入库单();
                    fun_强载_补();
                }
                else
                {
                    fun_事务_保存(false);
                    fun_强载();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            try
            {
                //fun_强载();
                if (comboBoxEdit1.EditValue == null || comboBoxEdit1.EditValue.ToString() == "")
                {
                    //受订量变化，有效总量变化
                    foreach (DataRow r in dtP.Rows)
                    {
                        //  StockCore.StockCorer.fun_物料数量_实际数量(r["物料编码"].ToString(),r["仓库号"].ToString(), true);
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "明细界面生效");
                throw new Exception("生效失败");
            }
        }

        private void fun_生效失败()
        {
            try
            {
                //主表生效
                drM["生效"] = 0;
                drM["生效日期"] = DBNull.Value;
                drM["生效人员ID"] = "";
                drM["生效人员"] = "";
                fun_保存订单();
                //明细生效
                foreach (DataRow r in dtP.Rows)
                {
                    r["生效"] = 0;
                    r["生效日期"] = DBNull.Value;
                }
                fun_保存明细();
                fun_事务_保存(false);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm销售单证详细界面_fun_生效失败");
            }
        }

        /// <summary>
        /// 使用事务保存主表数据、子表数据
        /// </summary>
        private void fun_事务_保存(bool s_提交审核)
        {

            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("生效");
            try
            {

                string sq212l = "select * from 销售记录销售订单明细表 where 1<>1";
                SqlCommand cmd4 = new SqlCommand(sq212l, conn, ts);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd4))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dtP);
                }

                string sql7 = "select * from 销售记录销售订单主表 where 1<>1";
                SqlCommand cmd7 = new SqlCommand(sql7, conn, ts);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd7))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dtM);
                }

                ts.Commit();

            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw ex;
            }

            //if (sj == true)
            //{




            //    if (bl_新增or修改)
            //    {
            //        foreach (DataRow dr in dtP.Rows)
            //        {
            //            DataRow[] dt_yu_fuzhu = dt_all.Select(string.Format("销售预订单明细号='{0}'", dr["销售预订单明细号"].ToString()));
            //            if (dt_yu_fuzhu.Length > 0)
            //            {
            //                dt_yu_fuzhu[0]["转换订单数量"] = decimal.Parse(dt_yu_fuzhu[0]["转换订单数量"].ToString()) + decimal.Parse(dr["数量"].ToString());
            //                dt_yu_fuzhu[0]["未转数量"] = decimal.Parse(dt_yu_fuzhu[0]["未转数量"].ToString()) - decimal.Parse(dr["数量"].ToString());
            //                if (decimal.Parse(dt_yu_fuzhu[0]["未转数量"].ToString()) < 0)
            //                {
            //                    throw new Exception("数量超过预订单数");
            //                }
            //                if (decimal.Parse(dt_yu_fuzhu[0]["转换订单数量"].ToString()) == decimal.Parse(dt_yu_fuzhu[0]["数量"].ToString()))
            //                {
            //                    dt_yu_fuzhu[0]["完成"] = true;
            //                }
            //            }
            //        }
            //    }

            //    int i = 0;
            //    foreach (DataRow dataRow in dt_all.Rows)
            //    {
            //        if (bool.Parse(dataRow["完成"].ToString()) == true)
            //        {
            //            i++;
            //        }
            //    }
            //    if (i == dt_all.Rows.Count)
            //    {
            //        if (dt_主.Rows.Count > 0)
            //        {
            //            dt_主.Rows[0]["完成"] = true;
            //        }
            //    }

        }

        /// <summary>
        /// 补开 用
        /// </summary>
        private void fun_事务_保存_补()
        {
            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("生效");
            try
            {
                {
                    string sql = "select * from L销售记录销售订单明细表L where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dtP);
                    }
                }
                {
                    string sql = "select * from L销售记录销售订单主表L where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dtM);
                    }
                }
                ts.Commit();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw ex;
            }
        }

        private void fun_销售订单号()
        {
            DateTime t = CPublic.Var.getDatetime();
            strSoNo = string.Format("SO{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SO", t.Year, t.Month, t.Day).ToString("0000"));
        }

        /// <summary>
        /// 直接按钮进入时，
        /// 载入订单主表：： 新增：载入空
        /// </summary>
        private void fun_载入()
        {
            string sqll = "";
            sqll = "select * from 销售记录销售订单主表 where 1<>1";
            using (SqlDataAdapter daM = new SqlDataAdapter(sqll, strconn))
            {
                dtM = new DataTable();
                daM.Fill(dtM);
                //gc.DataSource = dtM;
                drM = dtM.NewRow();
                dtM.Rows.Add(drM);
            }
        }

        /// <summary>
        /// 订单界面进入时，
        /// 将dr的值赋给txt：：新增的时候，载入为空；修改的时候，载入 drM 的数据.
        /// </summary>
        private void fun_载入订单()
        {
            if (bl_新增or修改 == true)
            {
                if (sj == true)
                {
                    dataBindHelper1.DataFormDR(drM);


                }
            }
            else
            {
                dataBindHelper1.DataFormDR(drM);
                searchLookUpEdit3.EditValue = drM["合同号"];
                txt_业务员.Text = drM["业务员"].ToString();
                cd_账期.Text = drM["帐期"].ToString();
                txt_税率.Text = drM["税率"].ToString();
                comboBox2.Text = drM["币种"].ToString();
                comboBox3.Text = drM["目标客户"].ToString();
            }
        }

        /// <summary>
        /// 订单界面进入时，
        /// 载入订单明细：： 新增的时候，载入为空；修改的时候，载入 销售单号 为 str_销售单号 的数据.
        /// </summary>
        private void fun_载入明细()
        {
            try
            {
                string sql = "";
                //新增的时候，载入为空；
                if (bl_新增or修改 == true && sj != true)
                {
                    sql = @"select a.*,新数据  from 销售记录销售订单明细表 a,基础数据物料信息表 b
                                where a.物料编码=b.物料编码 and  1<>1";
                }
                //主界面双击进入 修改的时候，载入 销售单号 为 str_销售单号 的数据


                if (sj == true)
                {
                    sql = string.Format(@"select a.*,新数据 from 销售预订单明细表 a,基础数据物料信息表 b
                                where a.物料编码=b.物料编码 and  a. 销售预订单号 = '{0}'  and 完成='0'   and 作废='0'  order by  a.销售预订单明细号 ", srt_预定单号);
                    string sql2 = @"select a.*,新数据 from 销售记录销售订单明细表 a,基础数据物料信息表 b
                                where a.物料编码=b.物料编码 and  1<>1";

                    dtP = CZMaster.MasterSQL.Get_DataTable(sql2, strconn);

                    DataTable da_zhuan = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (da_zhuan.Rows.Count > 0)
                    {

                        foreach (DataRow dsdasd in da_zhuan.Rows)
                        {
                            DataRow dr = dtP.NewRow();
                            dtP.Rows.Add(dr);
                            dr["物料编码"] = dsdasd["物料编码"];
                            dr["数量"] = decimal.Parse(dsdasd["未转数量"].ToString());
                            dr["规格型号"] = dsdasd["规格型号"];
                            dr["物料名称"] = dsdasd["物料名称"];
                            dr["销售预订单号"] = dsdasd["销售预订单号"];
                            dr["销售预订单明细号"] = dsdasd["销售预订单明细号"];
                            dr["税前单价"] = dsdasd["税前单价"];
                            dr["税后单价"] = dsdasd["税后单价"];
                            dr["税前金额"] = dsdasd["税前金额"];

                            dr["税后金额"] = dsdasd["税后金额"];
                            dr["仓库号"] = dsdasd["仓库号"];
                            dr["仓库名称"] = dsdasd["仓库名称"];
                            //dr["物料编码"] = dsdasd["物料编码"];
                            //dr["物料编码"] = dsdasd["物料编码"];






                        }





                    }





                }




                if (bl_新增or修改 == false)
                {
                    sql = string.Format(@"select a.*,新数据  from 销售记录销售订单明细表 a,基础数据物料信息表 b
                                where a.物料编码=b.物料编码 and  销售订单号 = '{0}'", str_销售订单号);
                }
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    if (sj == true)
                    {
                        gc.DataSource = dtP;
                    }
                    else
                    {
                        da.Fill(dtP);
                        gc.DataSource = dtP;
                    }


                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售单界面fun_载入");
            }
        }
        private void fun_直接生成入库单()
        {
            DateTime t = CPublic.Var.getDatetime();
            DataTable dt = new DataTable();
            DataTable dt_mx = new DataTable();
            string sql = "select  * from L销售记录成品出库单主表L where 1<>1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt);
            string sql_mx = "select  * from L销售记录成品出库单明细表L where 1<>1";
            da = new SqlDataAdapter(sql_mx, strconn);
            da.Fill(dt_mx);

            DataRow dr = dt.NewRow();

            dr["GUID"] = System.Guid.NewGuid();
            dr["成品出库单号"] = string.Format("SA{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("SA", t.Year, t.Month));

            dr["修改日期"] = t;
            dr["操作员ID"] = CPublic.Var.LocalUserID;
            dr["操作员"] = CPublic.Var.localUserName;
            dr["日期"] = t;
            dr["创建日期"] = t;
            dr["修改日期"] = t;
            dr["生效"] = true;
            dr["生效日期"] = t;
            dr["客户"] = txt_客户名称.Text;

            dt.Rows.Add(dr);
            int pos = 1;
            foreach (DataRow r in dtP.Rows)     //
            {
                if (r.RowState == DataRowState.Deleted) continue;




                DataRow dr_mx = dt_mx.NewRow();
                if (dr_mx["GUID"] == DBNull.Value)
                {
                    dr_mx["GUID"] = System.Guid.NewGuid();
                }
                dr_mx["成品出库单号"] = dr["成品出库单号"]; //入库单号

                dr_mx["成品出库单明细号"] = dr["成品出库单号"].ToString() + "-" + pos.ToString("00");
                dr_mx["POS"] = pos++;
                dr_mx["销售订单号"] = r["销售订单号"];

                dr_mx["销售订单明细号"] = r["销售订单明细号"];
                dr_mx["物料编码"] = r["物料编码"];
                dr_mx["物料名称"] = r["物料名称"];
                dr_mx["出库数量"] = r["数量"];
                dr_mx["已出库数量"] = r["数量"];
                dr_mx["未开票数量"] = r["数量"];
                dr_mx["计量单位"] = r["计量单位"];
                dr_mx["规格型号"] = r["规格型号"];
                dr_mx["客户"] = txt_客户名称.Text;
                dr_mx["客户编号"] = searchLookUpEdit1.EditValue.ToString();
                dr_mx["生效"] = true;
                dr_mx["生效日期"] = t;

                dr_mx["n原ERP规格型号"] = r["n原ERP规格型号"];

                dr_mx["销售备注"] = r["备注"];

                dr_mx["备注1"] = "补开";





                dt_mx.Rows.Add(dr_mx);
            }

            CZMaster.MasterSQL.Save_DataTable(dt, "L销售记录成品出库单主表L", strconn);
            CZMaster.MasterSQL.Save_DataTable(dt_mx, "L销售记录成品出库单明细表L", strconn);

        }

        //20-6-1
        private void load_dropDownList()
        {

            string sql = "select 属性字段1 as 编号,属性值 as 片区 from 基础数据基础属性表 where 属性类别 = '片区'";
            DataTable t = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            SL_片区.Properties.DataSource = t;
            SL_片区.Properties.DisplayMember = "片区";
            SL_片区.Properties.ValueMember = "片区";
            sql = "select  属性字段1 as 仓库号,属性值 as 仓库名称  from  基础数据基础属性表 where 属性类别='仓库类别' and 布尔字段1=1";
            dt_stock = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            repositoryItemGridLookUpEdit1.DataSource = dt_stock;
            repositoryItemGridLookUpEdit1.DisplayMember = "仓库号";
            repositoryItemGridLookUpEdit1.ValueMember = "仓库号";

            txt_业务员.Properties.Items.Clear();
            sql = "select 属性值 from 基础数据基础属性表 where 属性类别='业务员' order by POS";
            DataTable dt_属性 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_属性);
            foreach (DataRow r in dt_属性.Rows)
            {
                txt_业务员.Properties.Items.Add(r["属性值"].ToString());
            }

            string sqlqq = "select 合同名称,合同号, 客户编号,客户名称,合同开始日期,合同结束日期,签订人,签订日期,合同类型,合同文件,合同状态,有效期,对方合同号 from 客户签订合同表";
            DataTable dt_合同 = CZMaster.MasterSQL.Get_DataTable(sqlqq, strconn);
            searchLookUpEdit3.Properties.DataSource = dt_合同;
            searchLookUpEdit3.Properties.DisplayMember = "合同名称";   // Text，即显式的文本
            searchLookUpEdit3.Properties.ValueMember = "合同号";    // Value，即实际的值




            dt_物料下拉框 = new DataTable();

            string sql2 = @"select base.物料名称,新数据,base.物料编码,base.规格型号,a.仓库号,a.仓库名称,a.货架描述,base.仓库号 as 默认仓库号,base.仓库名称 as 默认仓库,自制,
                             base.计量单位,base.特殊备注,isnull(a.有效总数,0)有效总数,isnull(a.库存总数,0)库存总数,isnull(a.在制量,0)在制量,isnull(a.受订量,0)受订量  
                             from 基础数据物料信息表 base    left  join 仓库物料数量表 a on base.物料编码 = a.物料编码 and a.仓库号=base.仓库号
                             where (base.内销= 1 or 外销=1)  and base.停用 = 0 and base.在研 = 0";
            da = new SqlDataAdapter(sql2, strconn);
            da.Fill(dt_物料下拉框);

            repositoryItemSearchLookUpEdit1.PopupFormSize = new Size(1500, 400);
            repositoryItemSearchLookUpEdit1.DataSource = dt_物料下拉框;
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";

            txt_税率.Properties.Items.Clear();
            txt_订单方式.Properties.Items.Clear();
            string sql3 = "select * from 基础数据基础属性表 where 属性类别 in( '订单方式','税率','账期')";
            DataTable dt_属性值 = new DataTable();
            SqlDataAdapter da_属性值 = new SqlDataAdapter(sql3, strconn);
            da_属性值.Fill(dt_属性值);
            foreach (DataRow r in dt_属性值.Rows)
            {
                if (r["属性类别"].ToString() == "税率")
                {
                    txt_税率.Properties.Items.Add(r["属性值"].ToString());
                }
                if (r["属性类别"].ToString() == "订单方式")
                {
                    txt_订单方式.Properties.Items.Add(r["属性值"].ToString());
                }
                if (r["属性类别"].ToString() == "账期")
                {
                    cd_账期.Properties.Items.Add(r["属性值"].ToString());

                }
            }
            sql = "select 客户编号,客户名称,联系人,手机,业务员,账期,固定电话,税率,币种,国内,国外 from 客户基础信息表 where 停用=0 ";
            dt_客户 = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_客户);
            searchLookUpEdit1.Properties.DataSource = dt_客户;
            searchLookUpEdit1.Properties.DisplayMember = "客户编号";
            searchLookUpEdit1.Properties.ValueMember = "客户编号";
            Thread th = new Thread(() =>
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    DataTable dt_目标客户 = dt_客户.Copy();
                    DataColumn[] pk = new DataColumn[1];
                    pk[0] = dt_目标客户.Columns["客户编号"];
                    dt_目标客户.PrimaryKey = pk;

                    foreach (DataRow dr1 in dt_目标客户.Rows)
                    {
                        comboBox3.Items.Add(dr1["客户名称"]);
                    }


                }));

            });

            th.IsBackground = true;
            th.Start();
            //19-4-2 此处暂时用客户信息里面的 币种 group by
            //sql = "select  币种 from 客户基础信息表  group by 币种";
            //DataTable temp = new DataTable();
            //da = new SqlDataAdapter(sql, strconn);
            //da.Fill(temp);
            //foreach (DataRow r in temp.Rows)
            //{
            //    comboBox2.Items.Add(r["币种"].ToString());
            //}
            //string sql_bzfs = "select  POS as 编号,属性值 as 包装方式,属性字段1 as 描述 from  基础数据基础属性表  where 属性类别='包装方式' order by 编号";
            //2020-5-29 
            string sql_bzfs = "select  物料编码 as 编号 ,物料名称 as 包装方式 ,特殊备注 as 用量,规格型号,自定义项1,自定义项2 from 基础数据物料信息表 where 存货分类='包材配件包' and 停用=0";
            dt_包装方式 = CZMaster.MasterSQL.Get_DataTable(sql_bzfs, strconn);
            dv_bzfs = new DataView(dt_包装方式);
            repositoryItemSearchLookUpEdit2.DataSource = dv_bzfs;
            repositoryItemSearchLookUpEdit2.ValueMember = "编号";
            repositoryItemSearchLookUpEdit2.DisplayMember = "编号";



            sql = @"select * from 汇率维护表 where 年 = (select max(年) from 汇率维护表) 
              and 月 = (select max(月) from 汇率维护表 where 年 = (select max(年) from 汇率维护表) )";
            dt_币种 = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_币种);
            comboBox2.DataSource = dt_币种;
            comboBox2.ValueMember = "币种";
            comboBox2.DisplayMember = "币种";

            sql = "select CRMSoCode,DistributorName,memo from somain where bl=0";
            string strcon_aliyun = string.Format("server={0};User Id={1};password={2};Database={3};CharSet=utf8", CPublic.Var.li_CFG["aliyun_server"].ToString(),
                      CPublic.Var.li_CFG["aliyun_UID"].ToString(), CPublic.Var.li_CFG["aliyun_PWD"].ToString(), CPublic.Var.li_CFG["aliyun_database"].ToString());
            MySqlDataAdapter a = new MySqlDataAdapter(sql, strcon_aliyun);
            DataTable dt_crm = new DataTable();
            a.Fill(dt_crm);
            searchLookUpEdit4.Properties.DataSource = dt_crm;
            searchLookUpEdit4.Properties.DisplayMember = "CRMSoCode";
            searchLookUpEdit4.Properties.ValueMember = "CRMSoCode";

            sql = "select  物料编码,物料名称,规格型号,特殊备注 as 用量 from 基础数据物料信息表 where 自定义项1='安装配件包'";
            t_安装配件包 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            //foreach (DataRow dr in t_安装配件包.Rows)
            //{
            //    repositoryItemCheckedComboBoxEdit1.Items.Add(dr["物料编码"].ToString(), dr["物料名称"].ToString() + " " + dr["规格型号"].ToString());
            //}
            //repositoryItemCheckedComboBoxEdit1.SeparatorChar = ',';
            ///20-6-16 安装配件包也要变成单选的下拉  searchLookUpedit

            repositoryItemSearchLookUpEdit3.DataSource = t_安装配件包;
            repositoryItemSearchLookUpEdit3.DisplayMember = "规格型号";
            repositoryItemSearchLookUpEdit3.ValueMember = "物料编码";

        }
        #endregion

        #region 订单操作
        //新增订单
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataView dv = new DataView(dtM);
                dv.RowStateFilter = DataViewRowState.Added;
                if (dv.Count > 0)
                {
                    if (MessageBox.Show("当前有未保存的销售单，是否放弃保存？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        //清空
                        fun_清空();
                        dtP.Clear();
                        fun_载入明细();
                    }
                }
                else
                {
                    fun_清空();
                    bl_新增or修改 = true;

                    dtP.Clear();
                    fun_载入明细();

                    drM = dtM.NewRow();
                    dtM.Rows.Add(drM);
                    strSoNo = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //保存
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (dtP.Rows.Count <= 0)
                {
                    throw new Exception("当前无明细保存");
                }

                gv.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                this.ActiveControl = null;
                //if (sj != true && ypd == false)
                //{
                //    fun_预定单判断();////19-8-1号加
                //}
                //2020-5-31 
                if (MessageBox.Show("确认包装方式等信息已完善,确认请继续？", "提醒!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    fun_Check_主表();
                    fun_Check_明细();
                    //check明细金额

                    fun_明细金额变化(true);
                    if (bl_新增or修改 == true)
                    {
                        fun_销售订单号();
                        txt_销售订单号.Text = strSoNo;
                    }
                    fun_保存订单();
                    fun_保存明细();
                    if (comboBoxEdit1.EditValue != null && comboBoxEdit1.EditValue.ToString() == "补开")
                    {
                        fun_事务_保存_补();
                        fun_强载_补();
                    }
                    else
                    {
                        fun_事务_保存(false);
                        //19-10-29  需要回馈给CRM销售订单号
                        if (CrmSo.Trim() != "")
                        {
                            string strcon_aliyun = string.Format("server={0};User Id={1};password={2};Database={3};CharSet=utf8", CPublic.Var.li_CFG["aliyun_server"].ToString(),
                            CPublic.Var.li_CFG["aliyun_UID"].ToString(), CPublic.Var.li_CFG["aliyun_PWD"].ToString(), CPublic.Var.li_CFG["aliyun_database"].ToString());
                            string s = string.Format(" select * from  somain  where bl=0 and CRMSoCode='{0}'", CrmSo);
                            using (MySqlDataAdapter da = new MySqlDataAdapter(s, strcon_aliyun))
                            {
                                DataTable dt_somain = new DataTable();
                                da.Fill(dt_somain);
                                dt_somain.Rows[0]["SoCode"] = txt_销售订单号.Text.Trim();
                                //dt_somain.Rows[0]["bl"] = true; 改为到物流确认 赋值为true
                                new MySqlCommandBuilder(da);
                                da.Update(dt_somain);
                            }
                        }
                        fun_强载();
                    }
                    //保存完变成修改状态        
                    bl_新增or修改 = false;
                    //强载一遍
                    MessageBox.Show("保存成功！");
                    fun_强载();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("保存失败！{0}", ex.Message));
            }
        }
        //2019-3-18 改
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (sj == true)
                {
                    throw new Exception("预订单前往查询作废");
                }
                if (MessageBox.Show(string.Format("该销售单是否确认作废？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    if (drM != null)
                    {
                        DateTime t = CPublic.Var.getDatetime();
                        drM["作废"] = 1;
                        drM["作废人员"] = CPublic.Var.localUserName;
                        drM["作废人员ID"] = CPublic.Var.LocalUserID;
                        drM["作废日期"] = t;
                    }
                    gv.CloseEditor();
                    this.BindingContext[dtP].EndCurrentEdit();
                    foreach (DataRow r_x in dtP.Rows)
                    {
                        if (r_x.RowState == DataRowState.Deleted)
                        {
                            continue;
                        }
                        r_x["作废"] = 1;
                        r_x["作废日期"] = CPublic.Var.getDatetime();
                    }
                    fun_保存明细();
                    fun_事务_保存(false);


                    MessageBox.Show("已作废");
                    ERPSale.frm销售单证详细界面 frm = new frm销售单证详细界面();
                    CPublic.UIcontrol.Showpage(frm, "订单录入");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }



        //关闭
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            CPublic.UIcontrol.ClosePage();
        }

        private void gv_RowCellClick_1(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            //if (e.Button == System.Windows.Forms.MouseButtons.Left && e.Clicks == 2)
            //{
            //    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            //    ERPSale.fm过往明细 fm = new ERPSale.fm过往明细(dr["物料编码"].ToString());
            //    fm.ShowDialog();
            //}
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (dr == null) return;
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gc, new Point(e.X, e.Y));
                }
                if (dr["销售预订单明细号"].ToString() != "")
                {
                    gridColumn3.OptionsColumn.ReadOnly = true;
                    gridColumn3.OptionsColumn.AllowEdit = false;
                }
                else
                {
                    gridColumn3.OptionsColumn.ReadOnly = false;
                    gridColumn3.OptionsColumn.AllowEdit = true;
                }
                if (e.Column.FieldName == "数量")
                {
                    if (dr["备注8"].ToString().Contains("标记") || dr["备注9"].ToString().Contains("标记")) //这俩字段 标记包材配件包和安装配件包的 
                    {
                        e.Column.OptionsColumn.AllowEdit = false;
                    }
                    else
                    {
                        e.Column.OptionsColumn.AllowEdit = true;

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 明细操作
        //新增明细
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //if (sj == true)
                //{
                //    throw new Exception("预订单转销售单不可新增");
                //}

                dr = dtP.NewRow();
                if (dtP.Rows.Count > 0)
                {
                    dr["送达日期"] = dtP.Rows[0]["送达日期"];
                    //dr["包装方式编号"] = dtP.Rows[0]["包装方式编号"];
                    //dr["包装方式"] = dtP.Rows[0]["包装方式"];
                }
                else
                {
                    dr["送达日期"] = CPublic.Var.getDatetime().Date.AddDays(1);
                }
                dr["GUID"] = System.Guid.NewGuid();
                dtP.Rows.Add(dr);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售单界面保存订单");
            }
        }

        //删除明细
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                //DataRow r = gv.GetDataRow(gv.FocusedRowHandle);
                //r.Delete();
                //fun_明细金额变化();
                int[] dr1 = gv.GetSelectedRows();
                if (dr1.Length > 0)
                {
                    for (int i = dr1.Length - 1; i >= 0; i--)
                    {
                        DataRow dr_选中 = gv.GetDataRow(dr1[i]);
                        if (dr_选中["销售预订单明细号"].ToString() != "")
                        {
                            DataRow[] dr111 = dt_未完成.Select(string.Format("销售预订单明细号 = '{0}'", dr_选中["销售预订单明细号"]));
                            dr111[0]["可转数量"] = Convert.ToDecimal(dr111[0]["可转数量"]) + Convert.ToDecimal(dr_选中["数量"]);
                        }
                        dr_选中.Delete();
                    }
                    fun_明细金额变化();
                    DataRow drs = gv.GetDataRow(Convert.ToInt32(dr1[0]));
                    if (drs != null) gv.SelectRow(dr1[0]);
                    else if (gv.GetDataRow(Convert.ToInt32(dr1[0]) - 1) != null)
                        gv.SelectRow(Convert.ToInt32(dr1[0]) - 1);
                }



            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售单界面删除明细");
            }
        }
        #endregion
        string strcon_FS = CPublic.Var.geConn("FS");
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ERPorg.Corg.FlushMemory();
            ERPSale.frm销售单证详细界面 frm = new frm销售单证详细界面();
            CPublic.UIcontrol.Showpage(frm, "订单录入");
        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {


                if (MessageBox.Show(string.Format("该销售单是否确认提交审核？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    gv.CloseEditor();
                    this.BindingContext[dtP].EndCurrentEdit();
                    fun_Check_主表();
                    fun_Check_明细();
                    // DataRow r_upper = ERPorg.Corg.fun_hr_upper("销售单", CPublic.Var.LocalUserID);
                    //if (r_upper == null)
                    //{
                    //    throw new Exception("人事组织关系中未维护你或你领导的信息,请确认");

                    //}
                    if (txt_销售订单号.Text != "")
                    {

                        string asd = string.Format("select * from 销售预订单明细表  where 1<>1");
                        DataTable dt_all = CZMaster.MasterSQL.Get_DataTable(asd, strconn);

                        string sql_ymx = "";
                        foreach (DataRow dr in dtP.Rows)
                        {
                            if (dr.RowState == DataRowState.Deleted) continue;
                            if (dr["销售预订单明细号"].ToString() != "")
                            {
                                sql_ymx = string.Format("select * from  销售预订单明细表 where 销售预订单明细号 = '{0}'", dr["销售预订单明细号"]);
                                SqlDataAdapter da = new SqlDataAdapter(sql_ymx, strconn);
                                da.Fill(dt_all);
                                DataRow[] dt_yu_fuzhu = dt_all.Select(string.Format("销售预订单明细号='{0}'", dr["销售预订单明细号"].ToString()));
                                if (dt_yu_fuzhu.Length > 0)
                                {
                                    if (dr["销售预订单明细号"].ToString() != null && dr["销售预订单明细号"].ToString() != "")
                                    {
                                        dt_yu_fuzhu[0]["转换订单数量"] = decimal.Parse(dt_yu_fuzhu[0]["转换订单数量"].ToString()) + decimal.Parse(dr["数量"].ToString());
                                        dt_yu_fuzhu[0]["未转数量"] = decimal.Parse(dt_yu_fuzhu[0]["未转数量"].ToString()) - decimal.Parse(dr["数量"].ToString());
                                        if (decimal.Parse(dt_yu_fuzhu[0]["未转数量"].ToString()) < 0)
                                        {
                                            throw new Exception("数量超过预订单数");
                                        }
                                        if (decimal.Parse(dt_yu_fuzhu[0]["转换订单数量"].ToString()) == decimal.Parse(dt_yu_fuzhu[0]["数量"].ToString()))
                                        {
                                            dt_yu_fuzhu[0]["完成"] = true;
                                        }
                                    }
                                }

                            }

                        }
                        //int i = 0;
                        //foreach (DataRow dataRow in dt_all.Rows)
                        //{
                        //    if (bool.Parse(dataRow["完成"].ToString()) == true)
                        //    {
                        //        i++;
                        //    }
                        //}
                        //if (i == dt_all.Rows.Count)
                        //{
                        //    if (dt_主.Rows.Count > 0)
                        //    {
                        //        dt_主.Rows[0]["完成"] = true;
                        //    }
                        //}
                        //DataTable dt_审核 = fun_PA(txt_销售订单号.Text, r_upper);
                        DataTable dt_审核 = ERPorg.Corg.fun_PA("生效", "销售单", txt_销售订单号.Text, txt_客户名称.Text);


                        string s = string.Format("select  * from  销售记录销售订单主表 where 作废=0 and 生效=0 and 销售订单号='{0}'", txt_销售订单号.Text);
                        DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                        if (dt.Rows.Count > 0)
                        {
                            dt.Rows[0]["待审核"] = true;



                            fun_明细金额变化(true);
                            if (bl_新增or修改 == true)
                            {
                                fun_销售订单号();
                                txt_销售订单号.Text = strSoNo;
                            }
                            dataBindHelper1.DataToDR(dt.Rows[0]);

                            fun_保存明细();
                        }
                        else
                        {
                            throw new Exception("单据状态已更改刷新后重试");

                        }

                        // da.Update(dt);
                        SqlConnection conn = new SqlConnection(strconn);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("pur"); //事务的名称
                        SqlCommand cmd1 = new SqlCommand("select * from 销售记录销售订单主表 where 1<>1", conn, ts);
                        SqlCommand cmd = new SqlCommand("select * from 单据审核申请表 where 1<>1", conn, ts);
                        SqlCommand cmd2 = new SqlCommand("select * from 销售记录销售订单明细表 where 1<>1", conn, ts);
                        SqlCommand cmd3 = new SqlCommand("select * from 销售预订单明细表 where 1<>1", conn, ts);

                        try
                        {
                            SqlDataAdapter da;
                            da = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da);
                            da.Update(dt);
                            da = new SqlDataAdapter(cmd);
                            new SqlCommandBuilder(da);
                            da.Update(dt_审核);
                            da = new SqlDataAdapter(cmd2);
                            new SqlCommandBuilder(da);
                            da.Update(dtP);
                            da = new SqlDataAdapter(cmd3);
                            new SqlCommandBuilder(da);
                            da.Update(dt_all);

                            ts.Commit();
                            MessageBox.Show("已提交审核");
                            barLargeButtonItem8.Enabled = true;
                            bl_istj = true;
                            fun_编辑();
                        }
                        catch
                        {
                            ts.Rollback();
                            throw new Exception("提交出错了,请刷新后重试");
                        }

                    }
                    else
                    {
                        throw new Exception("先保存后提交审核");
                    }

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show(string.Format("该销售单是否确认撤销提交？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string sql = string.Format("select * from 销售记录销售订单主表 where 销售订单号 = '{0}'", txt_销售订单号.Text);
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_撤销 = new DataTable();
                    da.Fill(dt_撤销);
                    sql = string.Format("select * from 单据审核申请表  where  单据类型='销售单' and 作废=0 and 审核=0 and 关联单号 = '{0}'", txt_销售订单号.Text);
                    da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_审核申请 = new DataTable();
                    da.Fill(dt_审核申请);
                    sql = "select * from  销售预订单明细表";
                    DataTable dt_ymx = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    sql = "select * from 销售预订单主表";
                    DataTable dt_yz = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                    if (bl_istj)
                    {
                        if (Convert.ToBoolean(dt_撤销.Rows[0]["审核"]))
                        {
                            throw new Exception("销售单已审核，请联系审核人弃审！");
                        }
                        else
                        {
                            if (dt_撤销.Rows.Count > 0)
                            {
                                if (Convert.ToBoolean(dt_撤销.Rows[0]["待审核"]))
                                {
                                    dt_撤销.Rows[0]["待审核"] = 0;
                                    if (dt_审核申请.Rows.Count > 0)
                                    {
                                        dt_审核申请.Rows[0].Delete();
                                    }
                                    foreach (DataRow dr in dtP.Rows)
                                    {
                                        if (dr["销售预订单明细号"].ToString() != "")
                                        {
                                            DataRow[] dr_ymx = dt_ymx.Select(string.Format("销售预订单明细号 = '{0}'", dr["销售预订单明细号"]));
                                            dr_ymx[0]["转换订单数量"] = Convert.ToDecimal(dr_ymx[0]["转换订单数量"]) - Convert.ToDecimal(dr["数量"]);
                                            dr_ymx[0]["未转数量"] = Convert.ToDecimal(dr_ymx[0]["未转数量"]) + Convert.ToDecimal(dr["数量"]);
                                            dr_ymx[0]["完成"] = false;
                                            DataRow[] dr_yz = dt_yz.Select(string.Format("销售预订单号 = '{0}'", dr["销售预订单号"]));
                                            dr_yz[0]["完成"] = false;
                                        }
                                    }
                                    SqlConnection conn = new SqlConnection(strconn);
                                    conn.Open();
                                    SqlTransaction ts = conn.BeginTransaction("pur"); //事务的名称
                                    SqlCommand cmd1 = new SqlCommand("select * from 单据审核申请表 where 1<>1", conn, ts);
                                    SqlCommand cmd = new SqlCommand("select * from 销售记录销售订单主表 where 1<>1", conn, ts);
                                    SqlCommand cmd2 = new SqlCommand("select * from 销售预订单主表 where 1<>1", conn, ts);
                                    SqlCommand cmd3 = new SqlCommand("select * from 销售预订单明细表 where 1<>1", conn, ts);

                                    try
                                    {

                                        da = new SqlDataAdapter(cmd1);
                                        new SqlCommandBuilder(da);
                                        da.Update(dt_审核申请);
                                        da = new SqlDataAdapter(cmd);
                                        new SqlCommandBuilder(da);
                                        da.Update(dt_撤销);
                                        da = new SqlDataAdapter(cmd2);
                                        new SqlCommandBuilder(da);
                                        da.Update(dt_yz);
                                        da = new SqlDataAdapter(cmd3);
                                        new SqlCommandBuilder(da);
                                        da.Update(dt_ymx);

                                        ts.Commit();
                                        MessageBox.Show("撤销成功");
                                        bl_istj = false;
                                        fun_编辑();
                                        drM["待审核"] = 0;
                                        drM.AcceptChanges();

                                        barLargeButtonItem8.Enabled = false;
                                    }
                                    catch
                                    {
                                        ts.Rollback();
                                        throw new Exception("提交出错了,请刷新后重试");
                                    }
                                    //sql = "select * from 单据审核申请表 where 1<>1";
                                    //da = new SqlDataAdapter(sql, strconn);
                                    //new SqlCommandBuilder(da);
                                    //da.Update(dt_审核申请);
                                    //sql = "select * from 销售记录销售订单主表 where 1<>1";
                                    //da = new SqlDataAdapter(sql, strconn);
                                    //new SqlCommandBuilder(da);
                                    //da.Update(dt_撤销);
                                    //sql = "select * from 销售预订单主表 where 1<>1";
                                    //da = new SqlDataAdapter(sql, strconn);
                                    //new SqlCommandBuilder(da);
                                    //da.Update(dt_yz);
                                    //sql = "select * from 销售预订单明细表 where 1<>1";
                                    //da = new SqlDataAdapter(sql, strconn);
                                    //new SqlCommandBuilder(da);
                                    //da.Update(dt_ymx);
                                    //MessageBox.Show("撤销成功");
                                    //bl_istj = false;
                                    //fun_编辑();
                                    //drM["待审核"] = 0;
                                    //drM.AcceptChanges();

                                    //barLargeButtonItem8.Enabled = false;
                                }
                            }
                        }

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
            try
            {
                if (drM == null)
                {
                    throw new Exception("请重新选择销售订单！");
                }

                SaveFileDialog save = new SaveFileDialog();
                save.FileName = drM["文件"].ToString();
                if (save.ShowDialog() == DialogResult.OK)
                {
                    CFileTransmission.CFileClient.strCONN = strcon_FS;
                    CFileTransmission.CFileClient.Receiver(drM["文件GUID"].ToString(), save.FileName);
                    MessageBox.Show("下载成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (drM == null)
                {
                    throw new Exception("请先新增销售订单！");
                }
                OpenFileDialog open = new OpenFileDialog();
                if (open.ShowDialog() == DialogResult.OK)
                {
                    FileInfo info = new FileInfo(open.FileName);      //判定上传文件的大小
                    //long maxlength = info.Length;
                    //if (maxlength > 1024 * 1024 * 8)
                    //{
                    //    throw new Exception("上传的文件不能超过1M，请重新选择后再上传！");//drM
                    //}
                    MasterFileService.strWSDL = CPublic.Var.strWSConn;
                    CFileTransmission.CFileClient.strCONN = strcon_FS;
                    string strguid = "";  //记录系统自动返回的GUID
                    strguid = CFileTransmission.CFileClient.sendFile(open.FileName);
                    drM["文件GUID"] = strguid;
                    drM["订单原件"] = true;
                    drM["文件"] = Path.GetFileName(open.FileName);
                    drM["上传时间"] = CPublic.Var.getDatetime();
                    if (drM["销售订单号"].ToString() != "")
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter("select * from 销售记录销售订单主表 where 1<>1", strconn))
                        {
                            new SqlCommandBuilder(da);
                            da.Update(dtM);
                        }

                    }
                    MessageBox.Show("上传成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //右击预览
        private void button5_Click(object sender, EventArgs e)
        {
            //try
            //{


            //    送货单样式预览 f1 = new 送货单样式预览();
            //    f1.ShowDialog();
            //    if (送货单样式预览.dd!=null)
            //    {
            //         /textBox2.Text = 送货单样式预览.dd.ToString();
            //    }

            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}

        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            try
            {

                if (drM["文件GUID"] == null || drM["文件GUID"].ToString() == "")
                {
                    throw new Exception("没有文件可以查看，请先上传文件");
                }
                //string type = dr["后缀"].ToString();

                string foldPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\查看文件\\";
                //string fileName = foldPath + CPublic.Var.getDatetime().ToString("yyyy-MM-dd") + "T" + CPublic.Var.getDatetime().ToString("HH_mm_ss") + "Z" + "_" + Guid.NewGuid().ToString() + "." + type;
                string fileName = foldPath + drM["文件"].ToString();

                try
                {
                    System.IO.Directory.Delete(foldPath, true);
                }
                catch (Exception)
                {
                }
                CFileTransmission.CFileClient.strCONN = strcon_FS;
                CFileTransmission.CFileClient.Receiver(drM["文件GUID"].ToString(), fileName);
                System.Diagnostics.Process.Start(fileName);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 19-10-29 获取CRM订单数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                if (CrmSo != "") throw new Exception(CrmSo + "尚未保存,不可继续获取,可以刷新后重新获取");
                //string strcon_aliyun = string.Format(@"server={0};User Id={1};password={2};Database={3};CharSet=utf8", CPublic.Var.li_CFG["aliyun_server"].ToString().Trim(),
                //       CPublic.Var.li_CFG["aliyun_UID"].ToString().Trim(), CPublic.Var.li_CFG["aliyun_PWD"].ToString().Trim(), CPublic.Var.li_CFG["aliyun_database"].ToString().Trim());

                string strcon_aliyun = string.Format("server={0};User Id={1};password={2};Database={3};CharSet=utf8", CPublic.Var.li_CFG["aliyun_server"].ToString(),
                      CPublic.Var.li_CFG["aliyun_UID"].ToString(), CPublic.Var.li_CFG["aliyun_PWD"].ToString(), CPublic.Var.li_CFG["aliyun_database"].ToString());
                string s = string.Format(@"select  * from somain  where bl=0 and CRMSoCode='{0}'", searchLookUpEdit4.EditValue.ToString());
                MySqlDataAdapter da = new MySqlDataAdapter(s, strcon_aliyun);
                DataTable dt_somain = new DataTable();
                da.Fill(dt_somain);
                if (dt_somain.Rows.Count == 0)
                {
                    CrmSo = "";
                    throw new Exception("未找到数据,请确认CRM订单号是否正确");
                }
                else
                {
                    CrmSo = searchLookUpEdit4.EditValue.ToString();
                    drM["备注1"] = CrmSo;
                    searchLookUpEdit1.EditValue = dt_somain.Rows[0]["DistributorCode"].ToString().Trim();
                    txt_日期.EditValue = Convert.ToDateTime(dt_somain.Rows[0]["Cdate"]);
                    txt_销售备注.Text = dt_somain.Rows[0]["memo"].ToString().Trim();
                }
                s = string.Format(" select InvCode,sum(amount)数量 from  so_details where   CRMSoCode='{0}' group by CRMSoCode,InvCode  ", searchLookUpEdit4.EditValue.ToString());
                da = new MySqlDataAdapter(s, strcon_aliyun);
                DataTable dt_sodetail = new DataTable();
                da.Fill(dt_sodetail);
                if (dt_sodetail.Rows.Count == 0)
                {
                    CrmSo = "";
                    throw new Exception("该单号未找到明细,请通知CRM查验该单号数据是否正确");
                }
                foreach (DataRow dr in dt_sodetail.Rows)
                {
                    DataRow dr_add = dtP.NewRow();
                    dr_add["物料编码"] = dr["InvCode"];
                    dr_add["数量"] = dr["数量"];
                    dtP.Rows.Add(dr_add);
                }
                infolink();
            }
            catch (Exception ex)
            {
                CrmSo = "";
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 针对包材配件包的规则 
        /// </summary>
        private void cal_baoz()
        {

            DataTable t_bz = new DataTable();
            t_bz.Columns.Add("编码");
            t_bz.Columns.Add("用量");
            //20-6-10
            //第一种 汇总 然后除用量  结果向上取整
            //第二种 汇总 然后除用量 余数要 单独用 对应箱装
            t_bz.Columns.Add("类型");

            t_bz.Columns.Add("父项总数", typeof(decimal));


            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                if (dr["备注10"].ToString().Contains("标记"))
                {
                    decimal dec_sl = 0;
                    if (dr["数量"].ToString() != "") dec_sl = Convert.ToDecimal(dr["数量"]);

                    DataRow[] tr = t_bz.Select($"编码='{dr["包装方式编号"].ToString()}'");
                    if (tr.Length > 0)
                    {
                        tr[0]["父项总数"] = dec_sl + Convert.ToDecimal(tr[0]["父项总数"]);
                    }
                    else
                    {
                        DataRow rr = t_bz.NewRow();
                        rr["编码"] = dr["包装方式编号"];
                        rr["用量"] = Convert.ToDecimal(dr["备注8"].ToString());
                        rr["父项总数"] = dr["数量"];
                        rr["类型"] = dr["备注10"];// 零头箱补齐的规则 这里面会有 "标记_零头箱补齐"
                        t_bz.Rows.Add(rr);
                    }
                }
            }


            foreach (DataRow xr in t_bz.Rows)
            {

                decimal dec_fz = 0;
                if (xr["父项总数"].ToString() != "")
                {
                    dec_fz = Convert.ToDecimal(xr["父项总数"]);
                }
                if (xr["类型"].ToString() == "标记")
                {
                    decimal dec = Math.Ceiling(dec_fz / Convert.ToDecimal(xr["用量"]));
                    DataRow[] ty = dtP.Select($"物料编码='{xr["编码"].ToString()}'");
                    if (ty.Length > 0) ty[0]["数量"] = dec;
                }
                else
                {
                    decimal dec = Math.Floor(dec_fz / Convert.ToDecimal(xr["用量"]));
                    DataRow[] ty = dtP.Select($"物料编码='{xr["编码"].ToString()}'");
                    if (ty.Length > 0)
                    {
                        ty[0]["数量"] = dec;
                        if (dec == 0) ty[0].Delete();
                    }
                    else
                    {
                        if (dec > 0)
                        {
                            DataRow xxr = dtP.NewRow();
                            xxr["物料编码"] = xr["编码"].ToString();
                            xxr["数量"] = dec;
                            xxr["备注8"] = "标记_零头箱补齐";
                            dtP.Rows.Add(xxr);
                            infolink();
                        }

                    }

                    int i_余数 = (int)dec_fz % Convert.ToInt32(xr["用量"]);
                    //需要补的零头箱
                    DataRow[] dr = dt_包装方式.Select($" 自定义项2='{xr["编码"].ToString()}' and 用量='{i_余数}' ");

                    DataRow[] yy = dtP.Select($"物料编码<>'{xr["编码"].ToString()}' and 备注8='标记_零头箱补齐'");
                    if (yy.Length > 0) yy[0].Delete();

                    if (i_余数 != 0 && dr.Length > 0)
                    {
                        DataRow[] xx = dtP.Select($"物料编码='{dr[0]["编号"].ToString()}' and 备注8='标记_零头箱补齐'");


                        if (xx.Length == 0)
                        {
                            DataRow xxr = dtP.NewRow();
                            xxr["物料编码"] = dr[0]["编号"].ToString();
                            xxr["数量"] = 1;
                            xxr["备注8"] = "标记_零头箱补齐";


                            dtP.Rows.Add(xxr);
                            infolink();
                        }
                        else
                        {
                            xx[0]["数量"] = 1;
                        }
                    }
                }
            }
            bool xj = false;
            foreach (DataRow jj in t_bz.Rows)
            {
                if (jj["类型"].ToString() == "标记_零头箱补齐")
                {
                    xj = true;
                    break;
                }
            }

            if (!xj)
            {
                for (int i = dtP.Rows.Count - 1; i >= 0; i--)
                {
                    if (dtP.Rows[i].RowState == DataRowState.Deleted) continue;
                    if (dtP.Rows[i]["备注8"].ToString() == "标记_零头箱补齐") dtP.Rows[i].Delete();

                }
            }

            for (int i = dtP.Rows.Count - 1; i >= 0; i--)
            {
                if (dtP.Rows[i].RowState == DataRowState.Deleted) continue;
                if (dtP.Rows[i]["备注8"].ToString() == "标记")
                {
                    DataRow[] pp = t_bz.Select($"编码='{dtP.Rows[i]["物料编码"].ToString()}'");
                    if (pp.Length == 0)
                    {
                        //dtP.Rows.RemoveAt(i);
                        dtP.Rows[i].Delete();
                    }
                }
            }


        }

        private void cal_安装配件包()
        {

            DataTable t_bz = new DataTable();
            t_bz.Columns.Add("编码");
            t_bz.Columns.Add("用量", typeof(decimal));
            t_bz.Columns.Add("父项总数", typeof(decimal));


            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                if (dr["物料编码"].ToString().StartsWith("10"))
                {
                    if (dr["安装配件包编号"].ToString() != "")
                    {
                        //20-6-17 这边用的是之前的 安装配件包 下拉多选的 checkcombox  现改为 searchlookupedit 只能单选 但是也不影响
                        string[] s_编号 = dr["安装配件包编号"].ToString().Trim().Split(',');
                        for (int i = 0; i < s_编号.Length; i++)
                        {
                            s_编号[i] = s_编号[i].Trim();
                        }
                        decimal dec_sl = 0;
                        if (dr["数量"].ToString() != "") dec_sl = Convert.ToDecimal(dr["数量"]);
                        foreach (string s in s_编号)
                        {
                            DataRow[] tr = t_bz.Select($"编码='{s}'");
                            if (tr.Length > 0)
                            {
                                tr[0]["父项总数"] = dec_sl + Convert.ToDecimal(tr[0]["父项总数"]);
                            }
                            else
                            {
                                DataRow[] p = t_安装配件包.Select($"物料编码='{s}'");
                                DataRow rr = t_bz.NewRow();
                                rr["编码"] = s;
                                rr["用量"] = p[0]["用量"];
                                rr["父项总数"] = dr["数量"];
                                t_bz.Rows.Add(rr);
                            }
                        }




                    }

                }
            }



            foreach (DataRow xr in t_bz.Rows)
            {
                decimal dec_fz = 0;
                if (xr["父项总数"].ToString() != "")
                {
                    dec_fz = Convert.ToDecimal(xr["父项总数"]);
                }

                decimal dec = Math.Ceiling(dec_fz / Convert.ToDecimal(xr["用量"]));
                DataRow[] ty = dtP.Select($"物料编码='{xr["编码"].ToString()}'");
                if (ty.Length > 0) ty[0]["数量"] = dec;
            }

            for (int i = dtP.Rows.Count - 1; i >= 0; i--)
            {
                if (dtP.Rows[i].RowState == DataRowState.Deleted) continue;
                if (dtP.Rows[i]["备注9"].ToString() == "标记")
                {
                    DataRow[] pp = t_bz.Select($"编码='{dtP.Rows[i]["物料编码"].ToString()}'");
                    if (pp.Length == 0)
                    {
                        //dtP.Rows.RemoveAt(i);
                        dtP.Rows[i].Delete();
                    }
                }
            }


        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                button4.Enabled = true;
                button2.Enabled = true;
                button5.Enabled = true;
            }
            else
            {
                button4.Enabled = false;
                button2.Enabled = false;
                button5.Enabled = false;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataRow[] ds = dt_币种.Select(string.Format("币种 = '{0}'", comboBox2.Text));
                if (ds.Length != 0)
                {
                    textBox4.Text = ds[0]["汇率"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void gv_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            if (e.Value.ToString() != "")
            {
                Decimal dec税率 = Convert.ToDecimal(Convert.ToInt32(txt_税率.EditValue) / (Decimal)100);
                if (e.Column.FieldName == "税后单价" || e.Column.Caption == "数量")
                {
                    if (dr["税后单价"].ToString() != "" && Convert.ToDecimal(dr["税后单价"]) >= (Decimal)0)
                    {
                        //if (e.Row["税前单价"] == DBNull.Value || Convert.ToDecimal(e.Row["税前单价"]) != (Convert.ToDecimal(e.Row["税后单价"]) / ((Decimal)1 + dec税率)))
                        {
                            dr["税前单价"] = (Convert.ToDecimal(dr["税后单价"]) / ((Decimal)1 + dec税率)).ToString("0.000000");
                        }
                    }
                    fun_明细金额变化();
                    if (e.Column.Caption == "数量" && dr["包装方式编号"].ToString() != "" && dr["物料编码"].ToString().StartsWith("10"))
                    { cal_baoz(); }
                    if (e.Column.Caption == "数量" && dr["安装配件包编号"].ToString() != "" && dr["物料编码"].ToString().StartsWith("10"))
                    { cal_安装配件包(); }
                }
                else if (e.Column.FieldName == "税前单价")
                {
                    if (Convert.ToDecimal(dr["税前单价"]) >= (Decimal)0)
                    {
                        //if (dr["税后单价"] == DBNull.Value || Convert.ToDecimal(dr["税前单价"]) != (Convert.ToDecimal(dr["税后单价"]) / ((Decimal)1 + dec税率)))
                        {
                            dr["税后单价"] = (Convert.ToDecimal(dr["税前单价"]) * ((Decimal)1 + dec税率)).ToString("0.000000");
                        }
                    }
                    fun_明细金额变化();
                }
                else if (e.Column.FieldName == "仓库号")
                {
                    DataRow[] r = dt_stock.Select(string.Format("仓库号='{0}'", e.Value.ToString()));
                    dr["仓库名称"] = r[0]["仓库名称"];
                    //if(dr["物料编码"].ToString()!="")
                    //{
                    //   DataRow []sr= dt_物料下拉框.Select(string.Format("物料编码='{0}' and 仓库号='{1}'",dr["物料编码"],e.Value.ToString()))
                    //        }
                }
            }
            if (e.Column.FieldName == "包装方式编号") //包装方式编号 就是 要添的物料编码
            {
                DataRow[] r = dt_包装方式.Select(string.Format("编号='{0}'", e.Value.ToString()));
                if (r.Length > 0)
                {
                    dr["包装方式"] = r[0]["包装方式"];
                    DataRow[] r_qf = dt_包装方式.Select($"[自定义项2]='{r[0]["编号"].ToString()}'");
                    //if (dr["物料编码"].ToString().StartsWith("10"))
                    //{
                    dr["备注8"] = r[0]["用量"];
                    dr["备注10"] = "标记_零头箱补齐";
                    DataRow[] b = dtP.Select($"物料编码='{e.Value.ToString()}'");
                    if (b.Length == 0)
                    {
                        DataRow r_b = dtP.NewRow();
                        r_b["物料编码"] = e.Value.ToString();
                        if (dr["数量"].ToString() != "")
                            r_b["数量"] = Math.Ceiling(Convert.ToDecimal(dr["数量"]) / Convert.ToDecimal(dr["备注8"]));
                        if (r_qf.Length == 0)
                        {
                            r_b["备注8"] = "标记"; //这个标记是包装方式的编码
                            dr["备注10"] = "标记";
                        }
                        else
                        {
                            r_b["备注8"] = "标记_零头箱补齐"; //这边只加标准箱  下面cal_baoz() 里面校正
                        }
                        dtP.Rows.Add(r_b);
                        infolink();
                    }
                    //}
                }
                else
                {
                    dr["包装方式"] = "";
                }
                cal_baoz();
            }

            else if (e.Column.FieldName == "安装配件包编号")
            {
                //2020-6-1
                #region 这里是之前用 checkedcombox 的版本

                //DataRow[] r = t_安装配件包.Select(string.Format("物料编码='{0}'", e.Value.ToString()));
                //string[] s_编号 = e.Value.ToString().Trim().Split(',');
                //for (int i = 0; i < s_编号.Length; i++)
                //{
                //    s_编号[i] = s_编号[i].Trim();
                //}

                //if (dr["物料编码"].ToString().StartsWith("10"))
                //{
                //    string s_配件包名称 = "";

                //    foreach (string s in s_编号)
                //    {
                //        if (s != "")
                //        {
                //            DataRow[] b = dtP.Select($"物料编码='{s}'");
                //            if (b.Length == 0)
                //            {
                //                DataRow[] r_安装包 = t_安装配件包.Select($"物料编码='{s}'");
                //                s_配件包名称 += r_安装包[0]["规格型号"].ToString() + " ";
                //                DataRow r_b = dtP.NewRow();
                //                r_b["物料编码"] = s;
                //                if (dr["数量"].ToString() != "")
                //                    r_b["数量"] = Math.Ceiling(Convert.ToDecimal(dr["数量"]) / Convert.ToDecimal(r_安装包[0]["用量"]));
                //                r_b["备注9"] = "标记"; //这个标记是 安装配件包  
                //                dtP.Rows.Add(r_b);
                //            }
                //        }

                //    }
                //    dr["安装配件包"] = s_配件包名称;
                //    infolink();
                //    cal_安装配件包();
                //}
                //dr["备注9"] = r[0]["包装方式"];

                //if (dr["物料编码"].ToString().StartsWith("10"))
                //{
                //    dr["备注8"] = r[0]["用量"];
                //    DataRow[] b = dtP.Select($"物料编码='{e.Value.ToString()}'");
                //    if (b.Length == 0)
                //    {
                //        DataRow r_b = dtP.NewRow();
                //        r_b["物料编码"] = e.Value.ToString();
                //        if (dr["数量"].ToString() != "")
                //            r_b["数量"] = Math.Ceiling(Convert.ToDecimal(dr["数量"]) / Convert.ToDecimal(dr["备注8"]));
                //        r_b["备注8"] = "标记"; //这个标记是包装方式的编码
                //        dtP.Rows.Add(r_b);
                //        infolink();

                //        cal_baoz();
                //    }
                //    else
                //    {
                //        cal_baoz();
                //    }
                //}

                #endregion
                DataRow[] r_安装 = t_安装配件包.Select(string.Format("物料编码='{0}'", e.Value.ToString()));

                if (r_安装.Length > 0)
                {
                    dr["安装配件包"] = r_安装[0]["规格型号"]; //这里先用规格型号
                    DataRow[] b = dtP.Select($"物料编码='{e.Value.ToString()}'");
                    if (b.Length == 0)
                    {

                        DataRow r_b = dtP.NewRow();
                        r_b["物料编码"] = e.Value.ToString();
                        if (dr["数量"].ToString() != "")
                            r_b["数量"] = Math.Ceiling(Convert.ToDecimal(dr["数量"]) / Convert.ToDecimal(r_安装[0]["用量"]));
                        r_b["备注9"] = "标记"; //这个标记是 安装配件包  
                        dtP.Rows.Add(r_b);
                    }
                    infolink();
                    cal_安装配件包();
                }
            }

        }

        private void gv_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "物料编码")
                {

                    Decimal dec税率 = Convert.ToDecimal(Convert.ToInt32(txt_税率.EditValue) / (Decimal)100);
                    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                    if (dr["销售预订单明细号"].ToString() == "")
                    {
                        DataRow[] dr_ymx = dt_未完成.Select(string.Format("物料编码= '{0}' and 可转数量>0", e.Value));
                        if (dr_ymx.Length > 0)
                        {
                            dr.Delete();
                            throw new Exception(e.Value + "物料存在未完成的预订单，请从预订单转订单");
                        }
                    }
                    DataRow rr = dt_物料下拉框.Select(string.Format("物料编码 = '{0}'", e.Value))[0];


                    try
                    {

                        dr["物料编码"] = e.Value;
                        dr["新数据"] = rr["新数据"].ToString();
                        dr["物料名称"] = rr["物料名称"].ToString();
                        dr["计量单位"] = rr["计量单位"].ToString();
                        //  e.Row["n原ERP规格型号"] = ds[0]["n原ERP规格型号"].ToString();
                        dr["规格型号"] = rr["规格型号"].ToString();
                        dr["特殊备注"] = rr["特殊备注"].ToString();
                        dr["仓库号"] = rr["仓库号"].ToString();
                        dr["仓库名称"] = rr["仓库名称"].ToString();
                        //Decimal dec税率 = Convert.ToDecimal(Convert.ToInt32(txt_税率.EditValue) / (Decimal)100);
                        try
                        {
                            decimal dec = 0;
                            dec = fun_客户物料单价(rr);
                            dr["税后单价"] = dec;
                            //  e.Row["税后单价"] = fun_明细金额(ds[0]).ToString("0.000000");
                            //e.Row["税前单价"] = (fun_明细金额(ds[0]) / ((Decimal)1 + dec税率)).ToString("0.000000");
                            dr["税前单价"] = dec / ((Decimal)1 + dec税率);
                        }
                        catch
                        {
                            //产品标准单价   5/18 改为销售单价 
                            decimal dec = 0;
                            dr["税后单价"] = dec;
                            dr["税前单价"] = dec;
                        }


                    }
                    catch (Exception ex)
                    {
                        throw new Exception(e.Value + ":" + ex.Message);
                    }
                }
                else if (e.Column.FieldName == "包装方式编号")
                {
                    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                    dr["备注10"] = "";
                    DataRow[] x = dtP.Select($"物料编码='{dr["包装方式编号"].ToString()}' and 备注10='标记_零头箱补齐'");
                    if (x.Length > 0)
                    {
                        x[0].Delete();
                    }

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);

            }
        }
        private void gv_CustomRowCellEditForEditing(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName == "包装方式编号")
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (dr != null)
                {
                    if (e.RowHandle == gv.FocusedRowHandle)
                    {
                        dv_bzfs.RowFilter = $"自定义项1 like '%{dr["物料编码"].ToString().Substring(0, 6)}%' and 自定义项2=''";
                        e.RepositoryItem = repositoryItemSearchLookUpEdit2;
                        repositoryItemSearchLookUpEdit2.PopupFormSize = new Size(800, 450);
                    }
                    else
                    {
                        e.RepositoryItem = repositoryItemTextEdit1;
                    }
                }
            }
        }

        private void gv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                if (gv.FocusedColumn.Caption == "物料编码") infolink();
                else if (gv.FocusedColumn.Caption == "包装方式编号")
                {
                    foreach (DataRow dr in dtP.Rows)
                    {
                        if (dr.RowState == DataRowState.Deleted) continue;
                        try
                        {
                            if (dr["包装方式编号"].ToString() != "")
                            {
                                DataRow[] r = dt_包装方式.Select(string.Format("编号='{0}'", dr["包装方式编号"]));
                                if (r.Length > 0)
                                    dr["包装方式"] = r[0]["包装方式"];

                            }
                        }
                        catch
                        {

                        }
                    }
                }
                else if (gv.FocusedColumn.Caption == "含税单价" || gv.FocusedColumn.Caption == "数量")
                {

                    fun_明细金额变化();
                }
            }
        }

        private void infolink()
        {
            DateTime t = CPublic.Var.getDatetime().Date.AddDays(1);

            if (dtP.Rows.Count >= 0)
            {
                if (dtP.Rows[0].RowState != DataRowState.Deleted)
                {

                    if (dtP.Rows[0]["送达日期"] != null && dtP.Rows[0]["送达日期"].ToString() != "")
                        t = Convert.ToDateTime(dtP.Rows[0]["送达日期"]);

                }
                //if (dtP.Rows[0]["包装方式编号"] != null && dtP.Rows[0]["包装方式编号"].ToString() != "")
                //{
                //    s_bzNo = dtP.Rows[0]["包装方式编号"].ToString();

                //    s_bzName = dtP.Rows[0]["包装方式"].ToString();

                //}
            }

            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                try
                {
                    if (dr["仓库号"].ToString() == "")
                    {
                        DataRow[] r = dt_物料下拉框.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                        dr["送达日期"] = t;
                        dr["新数据"] = r[0]["新数据"].ToString();
                        dr["物料名称"] = r[0]["物料名称"].ToString();
                        dr["计量单位"] = r[0]["计量单位"].ToString();
                        dr["规格型号"] = r[0]["规格型号"].ToString();
                        dr["特殊备注"] = r[0]["特殊备注"].ToString();
                        dr["仓库号"] = r[0]["默认仓库号"].ToString();
                        dr["仓库名称"] = r[0]["默认仓库"].ToString();
                        ////20-05-29
                        //if (dr["物料编码"].ToString().StartsWith("10") && dr["包装方式编号"].ToString() == "")
                        //{
                        //    dr["包装方式编号"] = s_bzNo;
                        //    dr["包装方式"] = s_bzName;
                        //}
                    }
                }
                catch
                {

                }
            }
        }

        private void searchLookUpEdit1_EditValueChanged_1(object sender, EventArgs e)
        {
            try
            {

                DataRow[] ds = dt_客户.Select(string.Format("客户编号 = '{0}'", searchLookUpEdit1.EditValue));
                if (ds.Length != 0)
                {
                    //  txt_客户编号.Text = ds[0]["客户编号"].ToString();
                    //if(searchLookUpEdit4.EditValue == null)
                    //{
                    //    searchLookUpEdit4.EditValue = ds[0]["客户名称"].ToString();
                    //}
                    if (bool.Parse(ds[0]["国内"].ToString()))
                    {
                        comboBox5.Text = "国内";
                    }
                    else if
                      (bool.Parse(ds[0]["国外"].ToString()))
                    {
                        comboBox5.Text = "国外";
                    }
                    else
                    {
                        comboBox5.Text = "";
                    }




                    comboBox3.Text = txt_客户名称.Text = ds[0]["客户名称"].ToString();
                    txt_客户负责人.Text = ds[0]["联系人"].ToString();
                    txt_电话号码.Text = ds[0]["手机"].ToString();
                    if (txt_电话号码.Text.ToString() == "")
                    {
                        txt_电话号码.Text = ds[0]["固定电话"].ToString();
                    }

                    cd_账期.Text = ds[0]["账期"].ToString();
                    //txt_业务员.Text = ds[0]["业务员"].ToString();
                    //txt_税率.Text = ds[0]["税率"].ToString();
                    //comboBox2.Text = ds[0]["币种"].ToString();

                    if (dtP.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtP.Rows)
                        {
                            dr["税后单价"] = fun_客户物料单价(dr);
                            decimal dec税率 = Convert.ToDecimal(txt_税率.Text);
                            decimal dec = 0;
                            decimal.TryParse((Convert.ToDecimal(dr["税后单价"]) / ((Decimal)1 + dec税率 / 100)).ToString(), out dec);
                            dr["税前单价"] = dec;
                        }

                        fun_明细金额变化();
                    }
                }
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    // txt_客户编号.Text = "";
                    comboBox3.Text = txt_客户名称.Text = "";
                    txt_客户负责人.Text = "";
                    txt_电话号码.Text = "";
                    txt_业务员.Text = "";
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售单界面_searchLookUpEdit1_EditValueChanged");
            }

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEdit2.EditValue == null || searchLookUpEdit2.EditValue.ToString() == "")
                {
                    MessageBox.Show("未选择物料");
                }
                else if (textBox1.Text == null || textBox1.Text.ToString() == "" || Convert.ToInt32(textBox1.Text) == 0)
                {
                    MessageBox.Show("份数未正确填写");
                }
                else
                {
                    string sql_mx = string.Format(@"select 基础数据物料BOM表.*,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.物料编码,n原ERP规格型号,库存总数,货架描述,仓库名称,需求数量={1}*数量 from 基础数据物料BOM表,基础数据物料信息表,仓库物料数量表
                                                   where    基础数据物料BOM表.子项编码= 基础数据物料信息表.物料编码 and   基础数据物料BOM表.子项编码= 仓库物料数量表.物料编码 
                                           and  产品编码='{0}'  ", searchLookUpEdit2.EditValue.ToString(), Convert.ToInt32(textBox1.Text));

                    DataTable dt = new DataTable();
                    dt = CZMaster.MasterSQL.Get_DataTable(sql_mx, strconn);

                    sql_mx = string.Format(@"select 子项编码,组,优先级 from 基础数据物料BOM表,基础数据物料信息表,仓库物料数量表
                                                   where    基础数据物料BOM表.子项编码= 基础数据物料信息表.物料编码 and   基础数据物料BOM表.子项编码= 仓库物料数量表.物料编码 
                                                   and  产品编码='{0}' and 组<>''  group by 子项编码,组,优先级 order by 组,优先级 ", searchLookUpEdit2.EditValue.ToString());
                    DataTable t_组 = CZMaster.MasterSQL.Get_DataTable(sql_mx, strconn);
                    if (t_组.Rows.Count > 0)
                    {

                        foreach (DataRow r in t_组.Rows)
                        {
                            DataRow[] xr = dt.Select(string.Format("物料编码 ='{0}'", r["子项编码"]));
                            if (xr.Length == 0) continue; //1461行可能会将对应同组的替代料删除 因此会找不到
                            if (Convert.ToDecimal(xr[0]["库存总数"]) < Convert.ToDecimal(xr[0]["需求数量"]))
                            {
                                string s = xr[0]["组"].ToString();
                                xr[0].Delete();
                                if (dt.Select(string.Format("组='{0}'", s)).Length == 0)
                                {
                                    throw new Exception(string.Format("组{0}替代料库存都不够", s));
                                }
                            }
                            else
                            {
                                DataRow[] rr = dt.Select(string.Format("物料编码<>'{0}' and 组='{1}' and 优先级<>'{2}'", r["子项编码"].ToString(), r["组"].ToString(), Convert.ToInt32(r["优先级"])));
                                for (int i = 0; i < rr.Length; i++)
                                {
                                    rr[i].Delete();
                                }


                            }
                        }
                    }

                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr.RowState == DataRowState.Deleted) continue;
                        DataRow drr = dtP.NewRow();
                        dtP.Rows.Add(drr);
                        drr["GUID"] = System.Guid.NewGuid();

                        drr["物料编码"] = dr["子项编码"].ToString().Trim();

                        drr["数量"] = dr["需求数量"];

                    }
                    fun_明细金额变化();

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
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                Form物料多选 fm = new Form物料多选();
                fm.ShowDialog();
                if (fm.flag == true && fm.dt_wul.Rows.Count > 0)
                {


                    foreach (DataRow drr in fm.dt_wul.Rows)
                    {

                        dtP.ImportRow(drr);


                    }
                    infolink();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            try
            {
                MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
                DataTable dt_传 = RBQ.SelectGroupByInto("", dtP, "物料编码,sum(数量) 数量,销售预订单明细号 ", "", "销售预订单明细号");
                frm预订单明细选择 fm = new frm预订单明细选择(dt_传);
                fm.ShowDialog();
                if (fm.flag == true && fm.dt_ydd_mx.Rows.Count > 0)
                {

                    dt_未完成 = fm.dt_ydd_mx;
                    foreach (DataRow drr in fm.dt_ydd_gxmx.Rows)
                    {

                        //DataRow[] dr = dtP.Select(string.Format("销售预订单明细号 = '{0}'", drr["销售预订单明细号"]));
                        //if (dr.Length > 0)
                        //{
                        //    dr[0]["数量"] =Convert.ToDecimal(dr[0]["数量"]) + Convert.ToDecimal(drr["此次转单数量"]);
                        //}
                        //else
                        //{
                        DataRow dr_mx = dtP.NewRow();
                        dtP.Rows.Add(dr_mx);
                        dr_mx["物料编码"] = drr["物料编码"];
                        dr_mx["数量"] = Convert.ToDecimal(drr["此次转单数量"]);
                        dr_mx["销售预订单号"] = drr["销售预订单号"];
                        dr_mx["销售预订单明细号"] = drr["销售预订单明细号"];
                        //}
                    }
                    infolink();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SL_片区_EditValueChanged(object sender, EventArgs e)
        {
            if (SL_片区.EditValue != null && SL_片区.EditValue.ToString() != "")
            {
                txt_业务员.Properties.Items.Clear();
                string sql = string.Format("select  业务员 from 片区业务员对应表  where 片区='{0}'", SL_片区.EditValue.ToString());
                DataTable dt_属性 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_属性);
                foreach (DataRow r in dt_属性.Rows)
                {
                    txt_业务员.Properties.Items.Add(r["业务员"].ToString());
                }
            }
            else
            {
                txt_业务员.Properties.Items.Clear();
                string sql = "select 属性值 from 基础数据基础属性表 where 属性类别='业务员' order by 属性字段1";
                DataTable dt_属性 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_属性);
                foreach (DataRow r in dt_属性.Rows)
                {
                    txt_业务员.Properties.Items.Add(r["属性值"].ToString());
                }
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;

            //小数点的处理。
            if ((int)e.KeyChar == 12290)                           //小数点
            {
                if (textBox4.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(textBox4.Text, out oldf);
                    b2 = float.TryParse(textBox4.Text + e.KeyChar.ToString(), out f);
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (bl_新增or修改)
                {
                    str_单据状态 = "";
                    label27.Visible = false;
                }
                else
                {

                    if (bl_istj)
                    {
                        fun_编辑();
                        string s = string.Format("select * from 销售记录销售订单主表 where 销售订单号='{0}'", txt_销售订单号.Text);
                        DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                        if (t.Rows.Count > 0)
                        {
                            if (Convert.ToBoolean(t.Rows[0]["审核"]))
                            {

                                str_单据状态 = "已审核";
                                label27.Visible = true;
                                label27.Text = str_单据状态;

                            }
                            else
                            {
                                str_单据状态 = "审核中";
                                label27.Visible = true;
                                label27.Text = str_单据状态;
                            }
                        }



                    }
                    else
                    {
                        str_单据状态 = "";
                        label27.Visible = false;
                    }
                }
            }
            catch (Exception)
            {


            }

        }

        private void txt_金额_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //判断按键是不是要输入的类型。
                if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                    e.Handled = true;

                //小数点的处理。
                if ((int)e.KeyChar == 46)                           //小数点
                {
                    if (txt_金额.Text.Length <= 0)
                        e.Handled = true;   //小数点不能在第一位
                    else
                    {
                        float f;
                        float oldf;
                        bool b1 = false, b2 = false;
                        b1 = float.TryParse(txt_金额.Text, out oldf);
                        b2 = float.TryParse(txt_金额.Text + e.KeyChar.ToString(), out f);
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

        private void txt_税率_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                Decimal dec税率 = Convert.ToDecimal(Convert.ToInt32(txt_税率.EditValue) / (Decimal)100);
                foreach (DataRow dr in dtP.Rows)
                {
                    if (dr["税后单价"].ToString() != "" && Convert.ToDecimal(dr["税后单价"]) >= (Decimal)0)
                    {
                        dr["税前单价"] = (Convert.ToDecimal(dr["税后单价"]) / ((Decimal)1 + dec税率)).ToString("0.000000");

                    }

                }
                fun_明细金额变化();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txt_税前金额_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //判断按键是不是要输入的类型。
                if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                    e.Handled = true;

                //小数点的处理。
                if ((int)e.KeyChar == 46)                           //小数点
                {
                    if (txt_税前金额.Text.Length <= 0)
                        e.Handled = true;   //小数点不能在第一位
                    else
                    {
                        float f;
                        float oldf;
                        bool b1 = false, b2 = false;
                        b1 = float.TryParse(txt_税前金额.Text, out oldf);
                        b2 = float.TryParse(txt_税前金额.Text + e.KeyChar.ToString(), out f);
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

        private void 维护箱贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (dr["销售订单明细号"].ToString() == "") throw new Exception("请在生成销售订单号后再维护箱贴信息");
                if (txt_客户名称.Text != "")
                {

                    ERPSale.fm销售合同箱贴数据维护 fm = new ERPSale.fm销售合同箱贴数据维护(txt_客户订单号.Text, dr);
                    fm.ShowDialog();

                    if (fm.bl) //有箱贴
                    {
                        dr["是否有箱贴"] = true;
                        dr.AcceptChanges();
                    }
                }
                else
                {
                    throw new Exception("先选择客户再进行箱贴维护");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
