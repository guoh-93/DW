using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ERPSale
{
    public partial class ui销售预订单录入 : UserControl
    {
        //123456测试
        #region
        string strconn = CPublic.Var.strConn;

        DataTable dt_物料下拉框;
        DataTable dt_客户;
        DataTable dt_stock;

        DataTable dtP = new DataTable();
        /// <summary>
        /// 新增：true；修改：false。
        /// </summary>
        Boolean bl_新增or修改 = true;
        Boolean bl_作废 = false;
        DataTable dt_作废审;
        string str_预订单号 = "";
        Boolean s_跳转 = false;
        DataRow drM = null;
        /// <summary>
        /// 销售预订单主表
        /// </summary>
        DataTable dtM = null;
        /// <summary>
        /// 新增明细：dr = dtM.NewRow()；修改明细：dr = gv.GetDataRow(gv.FocusedRowHandle);
        /// </summary>
        DataRow dr = null;
        #endregion
        DataTable dt_select;
        DataRow dr_select;
        public ui销售预订单录入()
        {
            InitializeComponent();
            bl_新增or修改 = true;
            barLargeButtonItem2.Enabled = true;
            barLargeButtonItem3.Enabled = true;
            barLargeButtonItem4.Enabled = true;
            txt_业务员.Enabled = true;
            txt_税率.Enabled = true;
            searchLookUpEdit1.Enabled = true;
            txt_日期.Enabled = true;
            txt_销售备注.Enabled = true;
            button1.Enabled = true;
            button3.Enabled = true;
            button2.Enabled = true;

        }


        public ui销售预订单录入(DataRow  DRR , DataTable DT)
        {
            InitializeComponent();
            dt_select = DT;
            dr_select = DRR;
        }


        public ui销售预订单录入(string s, DataRow dr, DataTable dt)
        {
            InitializeComponent();
            bl_新增or修改 = false;
            s_跳转 = true;
            str_预订单号 = s;
            txt_预订单号.Text = str_预订单号;
            drM = dr;
            dtM = dt;
            button3.Enabled = false;
        }
        public ui销售预订单录入(DataTable dt_预, DataTable dt)
        {
            InitializeComponent();
            bl_新增or修改 = false;
             
            str_预订单号 = dt_预.Rows[0]["销售预订单号"].ToString();
            txt_预订单号.Text = str_预订单号;
            drM = dt_预.Rows[0];
            dtM = dt;
           //button3.Enabled = false;
        }
        //public ui销售预订单录入(string s, DataRow dr, DataTable dt)
        //{
        //    InitializeComponent();
        //    bl_新增or修改 = false;
        //    str_预订单号 = s;
        //    drM = dr;
        //    dtM = dt;
        //}


        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ERPSale.ui销售预订单录入 ui = new ui销售预订单录入();
            CPublic.UIcontrol.Showpage(ui, "预订单录入");
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        /// <summary>
        /// 保存前，相关的重要数据需要CHECK DATA，如果CHECK出问题，给出合适的提示
        /// </summary>
        private void fun_Check_主表()
        {
            try
            {
                //预订单不限制
                //if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                //{
                //    throw new Exception("客户不能为空");
                //}
                if (txt_日期.EditValue == null)
                {
                    throw new Exception("日期为空");
                }

                if (txt_业务员.Text == "")
                {
                    throw new Exception("业务员为空");
                }
                if (txt_销售备注.Text == "")
                {
                    txt_销售备注.Text = " ";
                }



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Load_dropDownList()
        {
            txt_业务员.Properties.Items.Clear();
            string sql = "select 属性值 from 基础数据基础属性表 where 属性类别='业务员' order by POS";
            DataTable dt_属性 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_属性);
            foreach (DataRow r in dt_属性.Rows)
            {
                txt_业务员.Properties.Items.Add(r["属性值"].ToString());
            }

            dt_物料下拉框 = new DataTable();

            string sql2 = @"select base.物料名称,新数据,base.物料编码,base.规格型号,a.仓库号,a.仓库名称,a.货架描述,base.仓库号 as 默认仓库号,base.仓库名称 as 默认仓库,自制,
                             base.计量单位,base.标准单价,n销售单价,base.特殊备注,isnull(a.有效总数,0)有效总数,isnull(a.库存总数,0)库存总数,isnull(a.在制量,0)在制量,isnull(a.受订量,0)受订量  
                             from 基础数据物料信息表 base    left  join 仓库物料数量表 a on base.物料编码 = a.物料编码 and a.仓库号=base.仓库号
                             where (base.内销= 1 or 外销=1)  and base.停用 = 0 and base.在研 = 0";
            da = new SqlDataAdapter(sql2, strconn);
            da.Fill(dt_物料下拉框);
            repositoryItemSearchLookUpEdit1.PopupFormSize = new Size(1500, 400);
            repositoryItemSearchLookUpEdit1.DataSource = dt_物料下拉框;
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";

           // txt_tax.Text = "";

            sql = "select 客户编号,客户名称,业务员,账期,税率 from 客户基础信息表 where 停用=0 ";
            dt_客户 = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_客户);
            searchLookUpEdit1.Properties.DataSource = dt_客户;
            searchLookUpEdit1.Properties.DisplayMember = "客户编号";
            searchLookUpEdit1.Properties.ValueMember = "客户编号";


            sql = "select  属性字段1 as 仓库号,属性值 as 仓库名称  from  基础数据基础属性表 where 属性类别='仓库类别' and 布尔字段1=1";
            dt_stock = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            repositoryItemGridLookUpEdit1.DataSource = dt_stock;
            repositoryItemGridLookUpEdit1.DisplayMember = "仓库号";
            repositoryItemGridLookUpEdit1.ValueMember = "仓库号";


        }


        /// <summary>
        /// 直接按钮进入时，
        /// 载入订单主表：： 新增：载入空
        /// </summary>
        private void fun_载入()
        {
            string sqll = "";
            sqll = "select * from  销售预订单主表 where 1<>1";
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
        /// 载入订单明细：： 新增的时候，载入为空；修改的时候，载入 销售单号 为 str_销售单号 的数据.
        /// </summary>


        //计算明细金额，以及总金额
        private void fun_明细金额变化(Boolean blErr = false)
        {
            System.Decimal sum = 0;
            System.Decimal sum1 = 0;
            ERPorg.Corg cg = new ERPorg.Corg();
            string ss = "";
            foreach (DataRow r in dtP.Rows)
            {
                if (r.RowState == DataRowState.Deleted)
                {
                    continue;
                }
                try
                {
                    if (r["数量"].ToString() != "" && r["税后单价"].ToString() != "")
                    {
                        r["税后金额"] = ((Decimal)r["税后单价"] * (Decimal)r["数量"]).ToString("0.######");
                        sum += (Decimal)r["税后金额"];
                        r["税前金额"] = ((Decimal)r["税前单价"] * (Decimal)r["数量"]).ToString("0.######");
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
            txt_税前金额.Text = sum1.ToString("#0.####");
            txt_金额.Text = sum.ToString("#0.####");
        }

        private void fun_强载()
        {
            string sql = string.Format("select * from 销售预订单主表 where 销售预订单号 = '{0}'", txt_预订单号.Text);
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
           
                string sqll = string.Format(@"select smx.*,base.规格型号,新数据 from 销售预订单明细表 smx
                left join 基础数据物料信息表  base on base.物料编码 = smx.物料编码 
                where 销售预订单号 = '{0}' order by  POS asc ", txt_预订单号.Text);
                using (SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn))
                {
                    daa.Fill(dtP);
                    gc.DataSource = dtP;
                }
            

        }
        private string Fun_预订单号()
        {
            string s = "";
            DateTime t = CPublic.Var.getDatetime();
            s = string.Format("DY{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("DY", t.Year, t.Month, t.Day).ToString("0000"));
            return s;

        }
        private void Ui销售预订单录入_Load(object sender, EventArgs e)
        {
            try
            {
                fun_载入();
                if (bl_新增or修改 == false)
                {
                    fun_强载();
                }
                if (s_跳转 == true)
                {
                    fun_跳转();
                }
                txt_日期.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txt_录入人员.Text = CPublic.Var.localUserName;
                string sql = string.Format("select *  from 人事基础员工表 where 员工号='{0}'",CPublic.Var.LocalUserID);
                DataTable dt_bumen = CZMaster.MasterSQL.Get_DataTable(sql,strconn);

                if (dt_bumen.Rows.Count > 0)
                {
                    textBox1.Text = dt_bumen.Rows[0]["部门"].ToString();
                }
               
          
                Load_dropDownList();
                fun_载入明细();
               // fun_物料下拉框();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void fun_跳转()
        {
            barLargeButtonItem2.Enabled = false;
            barLargeButtonItem3.Enabled = false;
            barLargeButtonItem4.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            txt_业务员.Enabled = false;
            txt_税率.Enabled = false;
            searchLookUpEdit1.Enabled = false;
            txt_销售备注.Enabled = false;
            barLargeButtonItem1.Enabled = false;
            gv.OptionsBehavior.Editable = false;


            txt_日期.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
           
                dr = dtP.NewRow();
                //if (dtP.Rows.Count > 0)
                //{
                //    dr["预计发货日期"] = dtP.Rows[0]["预计发货日期"];
                //}
                dr["GUID"] = System.Guid.NewGuid();


                dtP.Rows.Add(dr);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售单界面保存订单");
            }
        }
        //保存订单
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                gv.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                if (dtP.Rows.Count <= 0)
                {
                    throw new Exception("没有明细可以保存");
                }

                fun_Check_主表();
                //check明细金额
                fun_明细金额变化(true);
                if (bl_新增or修改 == true)
                {

                    txt_预订单号.Text = Fun_预订单号();
                }
                fun_保存订单();

                fun_保存明细();

                fun_事务_保存();
                fun_强载();

                //保存完变成修改状态        
                bl_新增or修改 = false;

                MessageBox.Show("保存成功！");
               // barLargeButtonItem2.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("保存失败！{0}", ex.Message));
            }
        }

        private void fun_保存订单()
        {
            try
            {
                DateTime time = CPublic.Var.getDatetime();
                if (bl_新增or修改 == true)
                {
                    drM["销售预订单号"] = txt_预订单号.Text;
                    drM["税率"] = txt_税率.Text;
                    drM["订单日期"] = time;
                    drM["制单日期"] = time;
                    drM["制单人"] = CPublic.Var.localUserName;
                    drM["制单人ID"] = CPublic.Var.LocalUserID;
                    dataBindHelper1.DataToDR(drM);
                }
                else
                {
                    string sql = string.Format("select * from 销售预订单主表 where 销售预订单号 ='{0}'", drM["销售预订单号"]);
                    DataTable dt1 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt1.Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(dt1.Rows[0]["作废"]))
                        {
                            throw new Exception("该单据已作废,不可保存");
                        }
                    }
                    drM["修改日期"] = time;
                    drM["修改人"] = CPublic.Var.localUserName;
                    drM["订单日期"] = time;
               //     drM["部门编号"] = CPublic.Var.localUser部门编号;
                    dataBindHelper1.DataToDR(drM);

                }
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        private void fun_保存明细()
        {
            try
            {

                //string str = "";
                int i = 1;
                foreach (DataRow r in dtP.Rows)
                {
                    if (r.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }
                    DataRow[] rr = dt_物料下拉框.Select(string.Format("物料编码='{0}'", r["物料编码"].ToString()));
                    if (rr.Length == 0) throw new Exception("物料不存在");//正常不可能会发生

                    else if (Convert.ToBoolean(rr[0]["自制"]))
                    {
                        string sql_新 = string.Format("select 子项编码 from 基础数据物料BOM表 where 产品编码='{0}'", r["物料编码"].ToString());
                        DataTable dt_x = CZMaster.MasterSQL.Get_DataTable(sql_新, strconn);
                        if (dt_x.Rows.Count == 0)
                        {
                            throw new Exception(string.Format("物料'{0}'属性为自制尚无BOM信息,请联系开发部维护BOM后再下预订单", r["物料编码"].ToString()));
                        }
                    }

                   

                    if (r["预计发货日期"].ToString() == "")
                    {
                        throw new Exception("预计发货日期不能为空");
                    }

                    r["POS"] = i++;
                    r["销售预订单号"] = txt_预订单号.Text;
                    //if (r["销售预订单明细号"].ToString()=="")
                    r["销售预订单明细号"] = txt_预订单号.Text + "-" + Convert.ToInt32(r["POS"]).ToString("00");
                    //}
                    if (r["转换订单数量"].ToString()!=""&&     decimal.Parse(r["转换订单数量"].ToString())>0 )
                    {
                        r["未转数量"] = decimal.Parse(r["数量"].ToString()) - decimal.Parse(r["转换订单数量"].ToString());
                    }
                    else
                    {
                        r["未转数量"] = r["数量"];
                    }

                   

                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售单界面保存明细");
                throw new Exception("保存失败！" + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow r = gv.GetDataRow(gv.FocusedRowHandle);
                r.Delete();
                fun_明细金额变化();
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售单界面删除明细");
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show(string.Format("该销售预定单是否确认提交审核？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    // DataRow r_upper = ERPorg.Corg.fun_hr_upper("销售单", CPublic.Var.LocalUserID);
                    //if (r_upper == null)
                    //{
                    //    throw new Exception("人事组织关系中未维护你或你领导的信息,请确认");

                    //}
                    if (txt_预订单号.Text != "")
                    {
                        
                        fun_明细金额变化(true);


                        //DataTable dt_审核 = fun_PA(txt_销售订单号.Text, r_upper);
                        string s = string.Format("select  * from  销售预订单主表 where 关闭=0  and 销售预订单号='{0}'", txt_预订单号.Text);
                        DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                        if (dt.Rows.Count > 0)
                        {
                            if (Convert.ToBoolean(dt.Rows[0]["作废"]))
                            {
                                throw new Exception("该单据已作废，不可提交审核");
                            }
                            dt.Rows[0]["提交审核"] = true;
                        }

                        else
                        {
                            throw new Exception("单据状态已更改刷新后重试");

                        }
                        DataTable dt_审核 = ERPorg.Corg.fun_PA("生效", "销售预订单", txt_预订单号.Text, txt_客户名称.Text);

                       
                        fun_保存明细();
                        // da.Update(dt);
                        SqlConnection conn = new SqlConnection(strconn);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("pur"); //事务的名称
                        SqlCommand cmd1 = new SqlCommand("select * from 销售预订单主表 where 1<>1", conn, ts);
                        SqlCommand cmd2 = new SqlCommand("select * from 销售预订单明细表 where 1<>1", conn, ts);
                        SqlCommand cmd = new SqlCommand("select * from 单据审核申请表 where 1<>1", conn, ts);

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
                            ts.Commit();
                            MessageBox.Show("已提交审核");
                            barLargeButtonItem3.Enabled = false;
                            barLargeButtonItem2.Enabled = false;
                            barLargeButtonItem4.Enabled = false;
                            txt_业务员.Enabled = false;
                            txt_税率.Enabled = false;
                            searchLookUpEdit1.Enabled = false;
                            txt_日期.Enabled = false;
                            txt_销售备注.Enabled = false;
                            button1.Enabled = false;
                            button3.Enabled = false;
                            button2.Enabled = false;
                            
                            
                        }
                        catch
                        {
                            ts.Rollback();
                            throw new Exception("提交出错了,请刷新后重试");
                        }
                    }
                    else
                    {
                        throw new Exception("先保存后审核");
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        //private void fun_check_作废()
        //{
        //    string sql = string.Format("select * from 销售预订单明细表 where 预订单号='{0}'", txt_预订单号.Text);
        //    DataTable t = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
        //    //重新加载一遍
        //    foreach (DataRow dr in dtP.Rows)
        //    {


        //    }


        //}
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if(MessageBox.Show(string.Format("是否确认作废？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    gv.CloseEditor();
                    this.BindingContext[dtP].EndCurrentEdit();

                    if (txt_预订单号.Text != "")
                    {
                        string sql_预 = string.Format("select * from 销售预订单主表 where 销售预订单号 = '{0}'", txt_预订单号.Text);
                        DataTable dt_预 = CZMaster.MasterSQL.Get_DataTable(sql_预, strconn);
                        if (dt_预.Rows.Count > 0)
                        {
                            if(Convert.ToBoolean( dt_预.Rows[0]["审核"]) == true)
                            {
                                throw new Exception("该单据已审核，请联系上级弃审后作废");
                            }
                        }
                    }
                    bl_作废 = true;

                    if (drM != null)
                    {
                        DateTime t = CPublic.Var.getDatetime();
                        drM["作废"] = 1;
                        drM["作废人员"] = CPublic.Var.localUserName;
                        drM["作废人员ID"] = CPublic.Var.LocalUserID;
                        drM["作废日期"] = t;

                    }

                    foreach (DataRow r_x in dtP.Rows)
                    {
                        if (r_x.RowState == DataRowState.Deleted)
                        {
                            continue;
                        }
                        if (Convert.ToBoolean(r_x["完成"]))
                        {

                        }
                        else
                        {
                            r_x["作废"] = 1;
                            r_x["作废日期"] = CPublic.Var.getDatetime();
                        }                     
                    }
                    string sql = string.Format("select * from 单据审核申请表 where 关联单号 = '{0}' and 单据类型 = '销售预订单'", txt_预订单号.Text);
                    dt_作废审 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt_作废审.Rows.Count > 0)
                    {
                        dt_作废审.Rows[0]["作废"] = true;
                        
                    }
                    fun_保存明细();
                    fun_事务_保存();


                    MessageBox.Show("已作废");
                    barLargeButtonItem1_ItemClick(null, null);
                }
                   
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_事务_保存()
        {
            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("预订单保存");
            try
            {
                {
                    string sql = "select * from 销售预订单明细表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dtP);
                    }
                }
                {
                    string sql = "select * from 销售预订单主表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dtM);
                    }
                }
                if (bl_作废)
                {
                    
                        string sql = "select * from 单据审核申请表 where 1<>1";
                        SqlCommand cmd = new SqlCommand(sql, conn, ts);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            new SqlCommandBuilder(da);
                            da.Update(dt_作废审);
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

        private void gv_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                //gv.CloseEditor();
                //this.BindingContext[dtP].EndCurrentEdit();
                DataRow drM = gv.GetDataRow(gv.FocusedRowHandle);

                if (e.Column.Caption == "物料编码")
                {
                    string sql = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", e.Value);
                    DataTable dt_物料 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt_物料.Rows.Count > 0)
                    {
                        drM["物料编码"] = dt_物料.Rows[0]["物料编码"];
                        drM["物料名称"] = dt_物料.Rows[0]["物料名称"];
                        drM["规格型号"] = dt_物料.Rows[0]["规格型号"];
                        drM["计量单位"] = dt_物料.Rows[0]["计量单位"];
                        drM["新数据"] = dt_物料.Rows[0]["新数据"];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                
            }

        }
        private void gv_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {


            try

            {

                if (e.Value.ToString() != "")
                {
                    Decimal dec税率 = Convert.ToDecimal(Convert.ToInt32(txt_税率.EditValue) / (Decimal)100);
                    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
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
                    }
                    if (e.Column.FieldName == "税前单价")
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


                    if (e.Column.FieldName == "仓库号")
                    {
                        DataRow[] r = dt_stock.Select(string.Format("仓库号='{0}'", e.Value.ToString()));
                        dr["仓库名称"] = r[0]["仓库名称"];
                        //if(dr["物料编码"].ToString()!="")
                        //{
                        //   DataRow []sr= dt_物料下拉框.Select(string.Format("物料编码='{0}' and 仓库号='{1}'",dr["物料编码"],e.Value.ToString()))

                        //        }
                    }


                    if (e.Column.FieldName == "物料编码")
                    {
                        DataRow[] r = dt_物料下拉框.Select(string.Format("物料编码='{0}'", e.Value.ToString()));
                        /// dr["仓库名称"] = r[0]["仓库名称"];
                        if (r != null)
                        {
                            DataRow drM = (this.BindingContext[gc.DataSource].Current as DataRowView).Row;
                            // drM[""]= r[0][""];




                        }
                    }

                    if (e.Column.FieldName == "数量")
                    {

                        DataRow drM = (this.BindingContext[gc.DataSource].Current as DataRowView).Row;
                        if (drM["完成"].ToString()!="" && bool.Parse(drM["完成"].ToString()) == true  )
                        {
                            throw new Exception("当前数据已完成，无法修改");
                        }
                        if (drM["作废"].ToString() != "" && bool.Parse(drM["作废"].ToString()) == true)
                        {
                            throw new Exception("当前数据作废，无法修改");
                        }


                        // drM[""]= r[0][""];

                        if (drM["转换订单数量"].ToString()!="")
                        {
                            if (decimal.Parse(drM["数量"].ToString()) < decimal.Parse(drM["转换订单数量"].ToString()))
                            {
                                throw new Exception("修改数量不得低于已转换数量");
                            }
                        }


                       



                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                Ui销售预订单录入_Load(null, null);
            }







        }
 

        #region  方法

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


        private void fun_载入明细()
        {
                try
                {
                    string sql = "";
                    //新增的时候，载入为空；
                    if (bl_新增or修改 == true)
                    {
                        sql = @"select a.*,新数据 from 销售预订单明细表 a,基础数据物料信息表 b
                                where a.物料编码=b.物料编码 and  1<>1";
                    }
                    //主界面双击进入 修改的时候，载入 销售单号 为 str_销售单号 的数据
                    //if (bl_新增or修改 == false)
                    //{
                    //    sql = string.Format(@"select a.*,新数据 from 销售预订单明细表 a,基础数据物料信息表 b
                    //            where a.物料编码=b.物料编码 and  销售订单号 = '{0}'", str_预订单号);
                    //}
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                    {
                        da.Fill(dtP);
                        gc.DataSource = dtP;
                    }
                }
                catch (Exception ex)
                {
                    CZMaster.MasterLog.WriteLog(ex.Message, "销售单界面fun_载入");
                }
           
        }




        #endregion

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                Decimal dec税率 = Convert.ToDecimal(Convert.ToInt32(txt_税率.EditValue) / (Decimal)100);
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (dr == null) return;
                DataRow rr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);

                string ss = dr["物料编码"].ToString();
                try
                {


                    dr["新数据"] = rr["新数据"].ToString();
                    dr["物料名称"] = rr["物料名称"].ToString();
                    dr["计量单位"] = rr["计量单位"].ToString();
                    // e.Row["n原ERP规格型号"] = ds[0]["n原ERP规格型号"].ToString();
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
                        dr["税后单价"] = (Convert.ToDecimal(rr["n销售单价"])).ToString("0.######");
                        dr["税前单价"] = (Convert.ToDecimal(rr["n销售单价"]) / ((Decimal)1 + dec税率)).ToString("0.######");
                    }


                }
                catch (Exception ex)
                {
                    throw new Exception(ss + " 该物料不可售");
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);

            }
        }

        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            //try
            //{
            //    if (txt_税率.EditValue.ToString().Trim() == "") txt_税率.EditValue = 0;
            //    Decimal dec税率 = Convert.ToDecimal(Convert.ToInt32(txt_税率.EditValue) / (Decimal)100);
            //    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            //    DataRow rr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);

            //    string ss = dr["物料编码"].ToString();
            //    try
            //    {

            //        dr["新数据"] = rr["新数据"].ToString();

            //        dr["物料名称"] = rr["物料名称"].ToString();
            //        dr["计量单位"] = rr["计量单位"].ToString();
            //        //  e.Row["n原ERP规格型号"] = ds[0]["n原ERP规格型号"].ToString();
            //        dr["规格型号"] = rr["规格型号"].ToString();
                   
            //        dr["特殊备注"] = rr["特殊备注"].ToString();
            //        dr["仓库号"] = rr["仓库号"].ToString();
            //        dr["仓库名称"] = rr["仓库名称"].ToString();
            //        //Decimal dec税率 = Convert.ToDecimal(Convert.ToInt32(txt_税率.EditValue) / (Decimal)100);
            //        try
            //        {
            //            decimal dec = 0;
            //            dec = fun_客户物料单价(rr);
            //            dr["税后单价"] = dec;
            //            //  e.Row["税后单价"] = fun_明细金额(ds[0]).ToString("0.000000");
            //            //e.Row["税前单价"] = (fun_明细金额(ds[0]) / ((Decimal)1 + dec税率)).ToString("0.000000");
            //            dr["税前单价"] = dec / ((Decimal)1 + dec税率);
            //        }
            //        catch
            //        {
            //            //产品标准单价   5/18 改为销售单价 
            //            dr["税后单价"] = (Convert.ToDecimal(rr["n销售单价"])).ToString("0.000000");
            //            dr["税前单价"] = (Convert.ToDecimal(rr["n销售单价"]) / ((Decimal)1 + dec税率)).ToString("0.000000");
            //        }


            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception(ss + " 该物料不可售");
            //    }

            //}
            //catch (Exception ex)
            //{

            //    MessageBox.Show(ex.Message);

            //}
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {

                DataRow[] ds = dt_客户.Select(string.Format("客户编号 = '{0}'", searchLookUpEdit1.EditValue));
                if (ds.Length != 0)
                {
                    txt_客户名称.Text = ds[0]["客户名称"].ToString();

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
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售单界面_searchLookUpEdit1_EditValueChanged");
            }
        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show(string.Format("该销售预订单是否确认撤销提交？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string sql = string.Format("select * from 销售预订单主表 where 销售预订单号 = '{0}'", txt_预订单号.Text);
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_撤销 = new DataTable();
                    da.Fill(dt_撤销);
                    sql = string.Format("select * from 单据审核申请表  where  审核=0 and 单据类型='销售预订单' and 作废=0 关联单号 = '{0}'", txt_预订单号.Text);
                    da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_审核申请 = new DataTable();
                    da.Fill(dt_审核申请);
                  
                        if (Convert.ToBoolean(dt_撤销.Rows[0]["审核"]))
                        {
                            throw new Exception("销售单已审核，请联系审核人弃审！");
                        }
                        else
                        {
                            if (dt_撤销.Rows.Count > 0)
                            {
                                if (Convert.ToBoolean(dt_撤销.Rows[0]["提交审核"]))
                                {
                                    dt_撤销.Rows[0]["提交审核"] = 0;
                                    if (dt_审核申请.Rows.Count > 0)
                                    {
                                        dt_审核申请.Rows[0].Delete();
                                    }
                                    sql = "select * from 单据审核申请表 where 1<>1";
                                    da = new SqlDataAdapter(sql, strconn);
                                    new SqlCommandBuilder(da);
                                    da.Update(dt_审核申请);
                                    sql = "select * from 销售预订单主表 where 1<>1";
                                    da = new SqlDataAdapter(sql, strconn);
                                    new SqlCommandBuilder(da);
                                    da.Update(dt_撤销);
                                    MessageBox.Show("撤销成功");
                                                                    
                                    drM["提交审核"] = 0;
                                    drM.AcceptChanges();

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
        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            string sql = string.Format("select *  from 销售记录销售订单主表 where 1<>1 ") ;
            DataTable dt_销售 = CZMaster.MasterSQL.Get_DataTable(sql,strconn);
            sql = string.Format("select * from 销售记录销售订单明细表 where 1<>1 ");
            DataTable dt_销售明细 = CZMaster.MasterSQL.Get_DataTable(sql,strconn);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try

            {

                gv.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (bool.Parse(dr["完成"].ToString())==true)
                {
                    throw new Exception("当前数据已完成");

                }
                if (MessageBox.Show(string.Format("是否确认作废此条明细？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string sql = string.Format("select * from  销售预订单明细表 where  销售预订单明细号 ='{0}'", dr["销售预订单明细号"]);

                    DataTable dt_lins = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt_lins.Rows.Count > 0)
                    {
                        foreach (DataRow drr in dt_lins.Rows)
                        {
                            drr["作废"] = true;
                            drr["作废日期"] = DateTime.Now.ToString();
                            drr["完成"] = true;
                            drr["特殊备注"] = drr["特殊备注"].ToString() + ",作废完成";

                        }

                    }

                    string ssss = "select * from 销售预订单明细表 where 1<>1 ";
                    using (SqlDataAdapter da = new SqlDataAdapter(ssss, strconn))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dt_lins);
                        MessageBox.Show("保存成功");
                        sql = $@"select smx.*,base.规格型号,新数据 from 销售预订单明细表 smx
                left join 基础数据物料信息表  base on base.物料编码 = smx.物料编码
                where 销售预订单明细号 = '{dr["销售预订单明细号"]}'";  
                        dt_lins = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                        dr.ItemArray = dt_lins.Rows[0].ItemArray;

                        dr.AcceptChanges();
                        // Ui销售预订单录入_Load(null, null);
                    }
                }
                    


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (dr == null) return;
                string sql = string.Format("select * from 销售预订单明细表 where 销售预订单明细号 = '{0}'", dr["销售预订单明细号"]);

                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                if (s_跳转)
                {

                }
                else
                {
                    if (dt.Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(dt.Rows[0]["完成"]) == true || Convert.ToBoolean(dt.Rows[0]["作废"]) == true)
                        {
                            button3.Enabled = false;
                            button2.Enabled = false;
                        }
                        else
                        {
                            button3.Enabled = true;
                            button2.Enabled = true;
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void infolink()
        {
            DateTime t = CPublic.Var.getDatetime().AddMonths(1);
            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                try
                {
                    if (dr["仓库号"].ToString() == "")
                    {
                        DataRow[] r = dt_物料下拉框.Select(string.Format("物料编码='{0}'", dr["物料编码"]));

                        dr["预计发货日期"] = t;
            
                        dr["物料名称"] = r[0]["物料名称"].ToString();
                        dr["计量单位"] = r[0]["计量单位"].ToString();
                        dr["规格型号"] = r[0]["规格型号"].ToString();
                        //  dr["特殊备注"] = r[0]["特殊备注"].ToString();
                        dr["仓库号"] = r[0]["默认仓库号"].ToString();
                        dr["仓库名称"] = r[0]["默认仓库"].ToString();
                    }
                }
                catch
                {

                }
            }

        }

        private void gv_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.V)
                {
                    if (gv.FocusedColumn.Caption == "物料编码") infolink();
                    
                }
            }
            catch (Exception )
            {
 
            }
          
        }
    }

}
