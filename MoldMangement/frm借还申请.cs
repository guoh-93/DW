using CZMaster;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace MoldMangement
{
    public partial class frm借还申请 : UserControl
    {
        #region 用户变量
        //DataTable dt_基础数据物料信息表;
        DataTable dt_借还申请表;
        //DataTable dt_借还申请表1;
        string str_x预订单号 = "";
        DataTable dt_明细;
        // DataTable dt_借还申请表附表;
        DataTable dt_客户信息;
        DataTable dt_员工信息;
        DataTable dt_基础数据物料信息表;
        DataTable dt_申请人;
        DataTable dt_负责人;
        DataRow dr_主;
        DataTable dt_未完成;
        bool sj = false;/// <summary>

        /// 预销售 状态
        /// </summary>
        /// 
        bool 按钮查询 = false;
        DataTable dt;
        DataTable dt_转;
        DataTable dt_物料, dt_仓库;
        CurrencyManager cmM;
        string strcon = CPublic.Var.strConn;
        DataRow drM = null;
        int i = 0;

        bool s_提交审核 = false;
        #endregion

        #region 类自用
        public frm借还申请()
        {
            InitializeComponent();
        }

        public frm借还申请(string sss, DataRow dr, DataTable dt)
        {
            InitializeComponent();
            drM = dr;
            dt_明细 = dt;
            s_提交审核 = true;
            string sql = string.Format("select * from 借还申请表 where 申请批号 = '{0}'", drM["申请批号"]);
            DataTable tttb = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            if (tttb.Rows[0]["文件GUID"].ToString() != "")
            {
                checkBox1.Checked = true;
                button2.Enabled = true;
                button5.Enabled = true;
            }
        }
        public frm借还申请(DataRow dr, bool select)
        {
            InitializeComponent();
            drM = dr;
            按钮查询 = select;
        }


        public frm借还申请(DataRow dr)
        {
            InitializeComponent();
            drM = dr;
            string sql = string.Format("select * from 借还申请表 where 申请批号 = '{0}'", drM["申请批号"]);
            DataTable tttb = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            if (tttb.Rows[0]["文件GUID"].ToString() != "")
            {
                checkBox1.Checked = true;
                button2.Enabled = true;
                button5.Enabled = true;
            }
        }

        /// <summary>
        /// a 销售与订单号 b，主表数据   c dt 明细数据 ，d 标记状态
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// 
        public frm借还申请(object a, object b, object c, object d)
        {
            InitializeComponent();
            //  drM = dr;

            str_x预订单号 = (string)a;
            DataRow dataRow = (DataRow)b;
            dt_转 = (DataTable)c;
            sj = (bool)d;

        }

        private void fun_load()
        {

            if (按钮查询)
            {
                barLargeButtonItem2.Enabled = false;
                barLargeButtonItem4.Enabled = false;
                simpleButton1.Enabled = false;
                simpleButton2.Enabled = false;

            }

            string s_11 = "";
            if (!按钮查询) s_11 = "and base.停用 = 0  ";
            string asdwq = @"select base.物料编码,base.物料名称,base.规格型号,base.图纸编号,isnull(a.库存总数,0)库存总数,a.货架描述
           ,a.仓库号,a.仓库名称, base.仓库号 as 默认仓库号,base.仓库名称 as 默认仓库,自制,base.计量单位
           from 基础数据物料信息表 base
            left join 仓库物料数量表 a on base.物料编码 = a.物料编码 and  base.仓库号=a.仓库号  where   base.在研 = 0 " + s_11;
            dt_物料 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(asdwq, strcon);
            da.Fill(dt_物料);


            dt_仓库 = new DataTable();
            string sql4 = "select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别'";
            da = new SqlDataAdapter(sql4, strcon);
            da.Fill(dt_仓库);
            repositoryItemSearchLookUpEdit2.DataSource = dt_仓库;
            repositoryItemSearchLookUpEdit2.DisplayMember = "仓库号";
            repositoryItemSearchLookUpEdit2.ValueMember = "仓库号";


            string s = " select  属性值 as  借用原因 from 基础数据基础属性表 where 属性类别='借用原因分类'";

            DataTable st = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            searchLookUpEdit2.Properties.DataSource = st;
            searchLookUpEdit2.Properties.DisplayMember = "借用原因";
            searchLookUpEdit2.Properties.ValueMember = "借用原因";

            string sql_负责人 = "select 员工号,姓名,课室,部门 from 人事基础员工表";

            dt_负责人 = CZMaster.MasterSQL.Get_DataTable(sql_负责人, strcon);

            searchLookUpEdit3.Properties.DataSource = dt_负责人;
            searchLookUpEdit3.Properties.DisplayMember = "员工号";
            searchLookUpEdit3.Properties.ValueMember = "员工号";

            DateTime t = CPublic.Var.getDatetime();
            time_申请日期.EditValue = t;
            dt_基础数据物料信息表 = new DataTable();

            string sql = @"select base.物料编码,base.物料名称,base.规格型号 ,计量单位编码,计量单位,            
                isnull(a.库存总数,0)库存总数,base.货架描述,a.仓库号,a.仓库名称 from 仓库物料数量表 a
                 left join 基础数据物料信息表 base on a.物料编码 = base.物料编码
                  where   base.在研 = 0 and a.仓库号 in (select 属性字段1 as 仓库号 from 基础数据基础属性表 where 属性类别 = '仓库类别' and 布尔字段3 = 1 )" + s_11;
            fun_GetDataTable(dt_基础数据物料信息表, sql);
            repositoryItemSearchLookUpEdit1View.BestFitColumns();
            repositoryItemSearchLookUpEdit1.DataSource = dt_基础数据物料信息表;

            sql = "select 属性字段1 as 编号,属性值 as 片区 from 基础数据基础属性表 where 属性类别 = '片区'";
            DataTable t_片区 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            lookUpEdit1.Properties.DataSource = t_片区;
            lookUpEdit1.Properties.DisplayMember = "片区";
            lookUpEdit1.Properties.ValueMember = "片区";

            //sql = "select 员工号,姓名 from 人事基础员工表";
            //dt_申请人 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            //searchLookUpEdit4.Properties.DataSource = dt_申请人;
            //searchLookUpEdit4.Properties.DisplayMember = "员工号";
            //searchLookUpEdit4.Properties.ValueMember = "员工号";

            //if (searchLookUpEdit3.EditValue != null && searchLookUpEdit3.EditValue.ToString() != "")
            //{
            //    comboBoxEdit2.Properties.Items.Clear();
            //    sql = string.Format("select  业务员 from 片区业务员对应表  where 片区='{0}'", searchLookUpEdit3.EditValue.ToString());
            //    DataTable dt_属性 = new DataTable();
            //    da = new SqlDataAdapter(sql, strcon);
            //    da.Fill(dt_属性);
            //    foreach (DataRow r in dt_属性.Rows)
            //    {
            //        comboBoxEdit2.Properties.Items.Add(r["业务员"].ToString());
            //    }
            //}
            //else
            //{
            //    comboBoxEdit2.Properties.Items.Clear();
            //    sql = "select 属性值 from 基础数据基础属性表 where 属性类别='业务员' order by 属性字段1";
            //    DataTable dt_属性 = new DataTable();
            //    da = new SqlDataAdapter(sql, strcon);
            //    da.Fill(dt_属性);
            //    foreach (DataRow r in dt_属性.Rows)
            //    {
            //        comboBoxEdit2.Properties.Items.Add(r["属性值"].ToString());
            //    }
            //}


            // repositoryItemSearchLookUpEdit1View.PopulateColumns();

            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            textBox2.Text = CPublic.Var.localUserName;
            textBox3.Text = CPublic.Var.LocalUserID;
            if (drM == null)
            {
                dt_借还申请表 = new DataTable();
                string sql3 = "select * from 借还申请表 where 1<>1";
                fun_GetDataTable(dt_借还申请表, sql3);
                dt_明细 = new DataTable();
                string sql2 = "select a.*,库存总数,c.仓库号,c.仓库名称 from 借还申请表附表 a,基础数据物料信息表 b ,仓库物料数量表 c where 1<>1";
                fun_GetDataTable(dt_明细, sql2);

            }
            else
            {
                fun_重新加载(drM);
            }
            /////
            //sj 为预订单跳转状态
            ///

            if (sj == true)
            {
                foreach (DataRow dr in dt_转.Rows)
                {
                    string sql22 = string.Format("select * from 仓库物料数量表 where 物料编码='{0}'", dr["物料编码"].ToString());
                    DataRow drdda = CZMaster.MasterSQL.Get_DataRow(sql22, strcon);
                    DataRow drrrr = dt_明细.NewRow();
                    dt_明细.Rows.Add(drrrr);
                    drrrr["物料名称"] = dr["物料名称"];
                    drrrr["规格型号"] = dr["规格型号"];
                    drrrr["物料编码"] = dr["物料编码"];
                    drrrr["仓库号"] = dr["仓库号"];
                    drrrr["仓库名称"] = dr["仓库名称"];
                    if (drdda != null)
                    {
                        drrrr["库存总数"] = drdda["库存总数"];
                        drrrr["货架描述"] = drdda["货架描述"];
                    }
                    drrrr["销售预订单号"] = dr["销售预订单号"];
                    drrrr["销售预订单明细号"] = dr["销售预订单明细号"];
                    drrrr["计量单位"] = dr["计量单位"];
                    drrrr["申请数量"] = dr["未转数量"];

                }
                gc.DataSource = dt_明细;
            }
            else
            {
                cmM = BindingContext[dt_明细] as CurrencyManager;

                gc.DataSource = dt_明细;
            }



        }
        private void fun_重新加载(DataRow Mrow)
        {

            dt_借还申请表 = new DataTable();
            string sql3 = string.Format("select * from 借还申请表 where 申请批号='{0}'", Mrow["申请批号"].ToString());
            fun_GetDataTable(dt_借还申请表, sql3);
            dt_明细 = new DataTable();
            string sql2 = string.Format(@"select a.*,isnull(库存总数,0)库存总数,a.仓库号,a.仓库名称 from 借还申请表附表 a
                                        left join 基础数据物料信息表 b  on a.物料编码=b.物料编码
                                         left join 仓库物料数量表 c  on  a.物料编码=c.物料编码  and a.仓库号=c.仓库号
                            where     申请批号='{0}'", Mrow["申请批号"].ToString());
            fun_GetDataTable(dt_明细, sql2);
            gc.DataSource = dt_明细;
            txt_申请单号.Text = Mrow["申请批号"].ToString();
            comboBoxEdit1.Text = Mrow["借用类型"].ToString();

            // comboBoxEdit1_TextChanged(null, null);

            searchLookUpEdit2.EditValue = Mrow["原因分类"].ToString();
            searchLookUpEdit3.EditValue = Mrow["借用人员ID"].ToString();
            if (Mrow["预计归还日期"] != null && Mrow["预计归还日期"].ToString() != "")
                dateEdit1.EditValue = Convert.ToDateTime(Mrow["预计归还日期"]);
            time_申请日期.EditValue = Convert.ToDateTime(Mrow["申请日期"]);
            if (Mrow["借用类型"].ToString() == "对外客户")
            {
                DataRow[] rrr = dt_客户信息.Select(string.Format("客户名称='{0}'", Mrow["相关单位"].ToString()));
                if (rrr.Length > 0)
                    searchLookUpEdit1.EditValue = rrr[0]["客户编号"].ToString();

            }
            else
            {

                searchLookUpEdit1.EditValue = Mrow["借用人员ID"].ToString();
            }


            textBox3.Text = Mrow["工号"].ToString();
            textBox2.Text = Mrow["申请人"].ToString();
            textBox1.Text = Mrow["备注"].ToString();
            /////5.21加地址 物流
            lookUpEdit1.EditValue = Mrow["片区"].ToString();
            textBox4.Text = Mrow["地址"].ToString();
            textBox5.Text = Mrow["物流信息"].ToString();
        }

        private void frm借还申请_Load(object sender, EventArgs e)
        {
            try
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

                              select a.*,b.部门名称,b.备注 as 表头备注,b.制单人,b.业务员 ,isnull(c.数量,0)锁定数量 from 销售预订单明细表 a
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

                              select a.*,b.部门名称,b.备注 as 表头备注,b.制单人,b.业务员 ,isnull(c.数量,0)锁定数量 from 销售预订单明细表 a
                              left join 销售预订单主表 b on a.销售预订单号 = b.销售预订单号
                              left join tt c on a.销售预订单明细号 = c.销售预订单明细号
                              where a.作废 = 0 and a.完成 = 0 and a.关闭 = 0 and b.作废 = 0 and b.审核 = 1 and b.关闭 = 0 and b.完成 = 0  and 部门名称 = '{0}' ", CPublic.Var.localUser部门名称);
                }
                dt_未完成 = CZMaster.MasterSQL.Get_DataTable(sql1, strcon);

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
                fun_load();
                if (Convert.ToBoolean(s_提交审核))
                {
                    fun_编辑();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void fun_编辑()
        {
            barLargeButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            gv.OptionsBehavior.Editable = !s_提交审核;
            comboBoxEdit1.Enabled = !s_提交审核;
            searchLookUpEdit2.Enabled = !s_提交审核;
            simpleButton2.Enabled = !s_提交审核;
            time_申请日期.Enabled = !s_提交审核;
            dateEdit1.Enabled = !s_提交审核;
            searchLookUpEdit1.Enabled = !s_提交审核;
            simpleButton1.Enabled = !s_提交审核;
            textBox1.Enabled = !s_提交审核;
            button4.Enabled = !s_提交审核;
        }
        #endregion

        #region 数据操作
        private void fun_GetDataTable(DataTable dt, string sql)
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                da.Fill(dt);
            }
        }

        private void fun_SetDataTable(DataTable dt, string sql)
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                new SqlCommandBuilder(da);
                da.Update(dt);
            }
        }

        //private void fun_显示() //读取相应数据并匹配显示
        //{
        //    DataRow myDataRow = gv.GetDataRow(gv.FocusedRowHandle);
        //    DataTable dt = new DataTable();
        //    string F_DriverName = myDataRow["原ERP物料编号"].ToString();
        //    string sql = "select * from 基础数据物料信息表 where 原ERP物料编号 ='" + F_DriverName + "'";
        //    fun_GetDataTable(dt, sql);
        //    string s_物料编码 = dt.Rows[0]["物料编码"].ToString();
        //    DataTable dt_仓库物料数量表 = new DataTable();
        //    string sql2 = "select * from 仓库物料数量表 where 物料编码 ='" + s_物料编码 + "'";
        //    fun_GetDataTable(dt_仓库物料数量表, sql2);
        //    //DataTable dt_基础数据物料信息表 = new DataTable();
        //    //string sql3 = "select * from 基础数据物料信息表 where 原ERP物料编号 ='" + F_DriverName + "'";
        //    //fun_GetDataTable(dt_基础数据物料信息表, sql3);
        //    myDataRow["原ERP物料编号"] = dt.Rows[0]["原ERP物料编号"];
        //    myDataRow["物料名称"] = dt.Rows[0]["物料名称"];
        //    myDataRow["n原ERP规格型号"] = dt.Rows[0]["n原ERP规格型号"];
        //    myDataRow["货架描述"] = dt.Rows[0]["货架描述"];
        //    myDataRow["仓库名称"] = dt.Rows[0]["仓库名称"];
        //    myDataRow["物料编码"] = dt.Rows[0]["物料编码"];            
        //    myDataRow["库存总数"] = dt_仓库物料数量表.Rows[0]["库存总数"];
        //    myDataRow["物料单价"] = dt.Rows[0]["n核算单价"];


        //    myDataRow["工号"] = CPublic.Var.LocalUserID;
        //    myDataRow["申请人"] = CPublic.Var.localUserName;
        //}

        #endregion

        #region 界面操作
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
        //新增
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (i == 1)
            {
                frm借还申请_Load(null, null);
                i = 0;
            }
            cmM = BindingContext[dt_明细] as CurrencyManager;
            cmM.EndCurrentEdit();
            gv.CloseEditor();
            try
            {
                // time_申请日期.EditValue = CPublic.Var.getDatetime();

                cmM.AddNew();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }




        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)//删除
        {
            cmM.EndCurrentEdit();
            gv.CloseEditor();

            try
            {
                (cmM.Current as DataRowView).Row.Delete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        #endregion

        //private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{ 
        //    if (barEditItem1.EditValue.ToString() != "")
        //    {
        //        dt_物料编号 = new DataTable();
        //        string sql = "select * from  借还申请表附表 where 申请批号 = '" + barEditItem1.EditValue + "'";
        //        fun_GetDataTable(dt_物料编号, sql);
        //        gc.DataSource = dt_物料编号;
        //        dt_借还申请表1 = new DataTable();
        //        string sql2 = "select * from  借还申请表 where 申请批号 = '" + barEditItem1.EditValue + "'";
        //        fun_GetDataTable(dt_借还申请表1, sql2);
        //        foreach (DataRow dr in dt_借还申请表.Rows)
        //        {
        //            txt_出入库申请单号.Text = dr["申请批号"].ToString();
        //            textBox2.Text = dr["申请人"].ToString();
        //            textBox3.Text = dr["工号"].ToString();
        //            time_申请日期.Text = dr["申请日期"].ToString();
        //            dateEdit1.Text = dr["预计归还日期"].ToString();
        //            textBox1.Text = dr["备注"].ToString();
        //        }
        //        i = 1;
        //    }

        //}

        //private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    string sql = "select * from 借还申请表附表 where 1<>1";
        //    fun_SetDataTable(dt_明细, sql);
        //    decimal dec_单价 = 0;

        //    decimal dec_总金额 = 0;
        //    foreach (DataRow dr in dt_明细.Rows)
        //    {
        //        dec_单价 = Convert.ToDecimal(dr["总金额"]);
        //        dec_总金额 += dec_单价;
        //    }
        //    foreach (DataRow dr in dt_借还申请表1.Rows)
        //    {
        //        dr["总金额"] = dec_总金额.ToString();
        //    }
        //    string sql2 = "select * from 借还申请表 where 1<>1";
        //    fun_SetDataTable(dt_借还申请表1, sql2);
        //    dt_明细.Clear();
        //    MessageBox.Show("修改成功");
        //}

        //private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    DataTable dt_借还申请表 = new DataTable();
        //    string sql = "select * from 借还申请表 where 申请批号 = '" + barEditItem1.EditValue.ToString() + "'";
        //    fun_GetDataTable(dt_借还申请表, sql);

        //    DataTable dt_借还申请表附表 = new DataTable();
        //    string sql1 = "select * from 借还申请表附表 where 申请批号 = '" + barEditItem1.EditValue.ToString() + "'";
        //    fun_GetDataTable(dt_借还申请表附表, sql1);
        //    foreach (DataRow dr in dt_借还申请表.Rows)
        //    {
        //        dr.Delete(); 
        //    }
        //    foreach (DataRow dr in dt_借还申请表附表.Rows)
        //    {
        //        dr.Delete();
        //    }
        //    string sql2 = "select * from 借还申请表 where 1<>1";
        //    fun_SetDataTable(dt_借还申请表, sql2);
        //    string sql3 = "select * from 借还申请表附表 where 1<>1";
        //    fun_SetDataTable(dt_借还申请表附表, sql3);
        //    MessageBox.Show("删除成功！");
        //    dt_物料编号.Clear();
        //    dt = new DataTable();
        //    string sql5 = "select 申请批号 from 借还申请表 where 借还状态 = '未审核' and 申请人 = '" + CPublic.Var.localUserName + "'";
        //    fun_GetDataTable(dt, sql5);
        //    repositoryItemSearchLookUpEdit3.DataSource = dt;
        //    repositoryItemSearchLookUpEdit3.DisplayMember = "申请批号";
        //    repositoryItemSearchLookUpEdit3.ValueMember = "申请批号";

        //}

        private void comboBoxEdit1_TextChanged(object sender, EventArgs e)
        {
            if (comboBoxEdit1.Text == "对外客户")
            {
                label8.Text = "客户名称";
                string sql = "select 客户编号,客户名称  from 客户基础信息表 where 停用=0  ";
                dt_客户信息 = new DataTable();
                fun_GetDataTable(dt_客户信息, sql);
                searchLookUpEdit1.Properties.DataSource = dt_客户信息;
                searchLookUpEdit1.Properties.ValueMember = "客户编号";
                searchLookUpEdit1.Properties.DisplayMember = "客户名称";
                searchLookUpEdit1View.PopulateColumns();
            }
            else if (comboBoxEdit1.Text == "对内人员")
            {
                label8.Text = "员工号";
                string sql = "select 员工号,姓名,课室,部门 from 人事基础员工表";
                dt_员工信息 = new DataTable();
                fun_GetDataTable(dt_员工信息, sql);
                searchLookUpEdit1.Properties.DataSource = dt_员工信息;
                searchLookUpEdit1.Properties.ValueMember = "员工号";
                searchLookUpEdit1.Properties.DisplayMember = "员工号";
                searchLookUpEdit1View.PopulateColumns();


            }

        }

        private void gv_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //try
            //{
            //   DataRow drM = (this.BindingContext[gc.DataSource].Current as DataRowView).Row;

            //    if (e.Column.Caption == "仓库号")
            //    {
            //        string sql = string.Format("select * from 仓库物料数量表 where 物料编码='{0}' and 仓库号='{1}'", drM["物料编码"], drM["仓库号"]);
            //        DataRow dr = CZMaster.MasterSQL.Get_DataRow(sql,strcon);

            //        string sql4 = string.Format("select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别' and 属性字段1='{0}'  ", drM["仓库号"]);
            //        DataRow wqwds = CZMaster.MasterSQL.Get_DataRow(sql4, strcon);
            //        drM["仓库名称"] = wqwds["仓库名称"];
            //        if (dr != null)
            //        {
            //            drM["库存总数"] = dr["库存总数"];
            //            if (drM["库存总数"].ToString() == "")
            //            {
            //                drM["库存总数"] = 0;
            //            }
            //        }
            //        else {
            //            drM["库存总数"] = 0;
            //        }




            //    }
            //}
            //    catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            //if (e.Column.Caption == "申请数量")
            //{
            //    try
            //    {
            //        //DataRow myDataRow = gv.GetDataRow(gv.FocusedRowHandle);
            //        //decimal s_单价金额 = Convert.ToDecimal(myDataRow["物料单价"]);
            //        //decimal s_借用数量 = Convert.ToDecimal(myDataRow["申请借用数量"]);
            //        //decimal s_总金额 = s_单价金额 * s_借用数量;
            //        //myDataRow["总金额"] = s_总金额;
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }
            //}
        }
        private void repositoryItemSearchLookUpEdit1View_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
                gv.CellValueChanging -= gv_CellValueChanging;
                DataRow myDataRow = gv.GetDataRow(gv.FocusedRowHandle);
                myDataRow["规格型号"] = dr["规格型号"];
                myDataRow["物料名称"] = dr["物料名称"];
                myDataRow["货架描述"] = dr["货架描述"];
                myDataRow["仓库号"] = dr["仓库号"];
                myDataRow["仓库名称"] = dr["仓库名称"];
                myDataRow["物料编码"] = dr["物料编码"];
                myDataRow["库存总数"] = dr["库存总数"];
                myDataRow["计量单位编码"] = dr["计量单位编码"];
                myDataRow["计量单位"] = dr["计量单位"];
                gv.CellValueChanging += gv_CellValueChanging;
            }
            catch (Exception ex)
            {

              
            }
            
        }



        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try

            {
                if (sj == true)
                {
                    throw new Exception("预订单转过来不可以添加新物料");
                }

                DataRow dr = dt_明细.NewRow();

                dr["预计出库日期"] = CPublic.Var.getDatetime().Date;

                dt_明细.Rows.Add(dr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }





        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {


            cmM = BindingContext[dt_明细] as CurrencyManager;

            cmM.EndCurrentEdit();
            gv.CloseEditor();

            try
            {
                int[] dr1 = gv.GetSelectedRows();
                if (dr1.Length > 0)
                {
                    for (int i = dr1.Length - 1; i >= 0; i--)
                    {
                        DataRow dr_选中 = gv.GetDataRow(dr1[i]);
                        if (dr_选中["销售预订单明细号"].ToString() != "")
                        {
                            DataRow[] dr111 = dt_未完成.Select(string.Format("销售预订单明细号 = '{0}'", dr_选中["销售预订单明细号"]));
                            dr111[0]["可转数量"] = Convert.ToDecimal(dr111[0]["可转数量"]) + Convert.ToDecimal(dr_选中["申请数量"]);
                        }
                        dr_选中.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void fun_check()
        {

            if (dt_明细.Rows.Count <= 0)
            {
                throw new Exception("你没有添加任何借用物料单！");

            }
            if (dateEdit1.Text == "")
            {
                throw new Exception("请填写预计归还日期");

            }
            if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
            {
                throw new Exception("请填写借用人");

            }
            if (searchLookUpEdit3.EditValue == null || searchLookUpEdit3.EditValue.ToString() == "")
            {
                throw new Exception("请填写复制人");

            }
            ERPorg.Corg cg = new ERPorg.Corg();
            string ss = "";
            foreach (DataRow dr2 in dt_明细.Rows)
            {
                if (dr2.RowState == DataRowState.Deleted) continue;

                DataRow[] rr = dt_物料.Select(string.Format("物料编码='{0}'", dr2["物料编码"].ToString()));
                if (rr.Length == 0) throw new Exception("物料不存在");//正常不可能会发生

                else if (Convert.ToBoolean(rr[0]["自制"]))
                {
                    string sql_新 = string.Format("select 子项编码 from 基础数据物料BOM表 where 产品编码='{0}'", dr2["物料编码"].ToString());
                    DataTable dt_x = CZMaster.MasterSQL.Get_DataTable(sql_新, strcon);
                    if (dt_x.Rows.Count == 0)
                    {
                        throw new Exception(string.Format("物料'{0}'属性为自制尚无BOM信息,请联系开发部维护BOM后再下借用单", dr2["物料编码"].ToString()));
                    }
                }



                string s_编号 = dr2["物料编码"].ToString();
                if (dr2["申请数量"].ToString() == "")
                {
                    throw new Exception("(" + s_编号 + ")" + "没有填写借用数量！");

                }
                if (Convert.ToDecimal(dr2["申请数量"]) <= 0)
                {
                    throw new Exception("(" + s_编号 + ")" + "借用数量不能小于0！");

                }
                bool bl_停产 = cg.determ_stop_product(dr2["物料编码"].ToString());

                if (bl_停产)
                {
                    if (ss != "") ss += "," + dr2["物料编码"].ToString();
                    else ss += dr2["物料编码"].ToString();
                }
                if (dr2["销售预订单明细号"].ToString() == "")
                {
                    DataRow[] dr_ymx = dt_未完成.Select(string.Format("物料编码= '{0}' and 可转数量>0", dr2["物料编码"]));
                    if (dr_ymx.Length > 0)
                    {
                        throw new Exception(dr2["物料编码"].ToString() + "物料存在未完成的预订单，请从预订单转单");
                    }
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
        }
        private void fun_save()
        {
            DateTime t = CPublic.Var.getDatetime();
            if (txt_申请单号.Text.Trim() == "")
            {
                txt_申请单号.Text = string.Format("BA{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                      t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("BA", t.Year, t.Month).ToString("0000"));
            }
            int i = 1;
            foreach (DataRow dr2 in dt_明细.Rows)
            {
                if (dr2.RowState == DataRowState.Deleted) continue;
                dr2["申请批号"] = txt_申请单号.Text;
                dr2["申请批号明细"] = txt_申请单号.Text + "-" + i++.ToString("00");
                dr2["申请日期"] = t;
                dr2["借还状态"] = "未借出";
            }

            if (dt_借还申请表.Rows.Count == 0)
            {
                dr_主 = dt_借还申请表.NewRow();
                dt_借还申请表.Rows.Add(dr_主);
            }
            else
            {
                dr_主 = dt_借还申请表.Rows[0];
            }
            if (sj == true)
            {
                dr_主["销售预订单号"] = str_x预订单号;
            }
            dr_主["借用类型"] = comboBoxEdit1.Text;
            //dr["相关单位"] = searchLookUpEdit1.EditValue.ToString();
            dr_主["原因分类"] = searchLookUpEdit2.EditValue != null ? searchLookUpEdit2.EditValue : "";

            dr_主["申请人"] = textBox2.Text;
            dr_主["工号"] = textBox3.Text;
            dr_主["申请批号"] = txt_申请单号.Text;
            dr_主["地址"] = textBox4.Text;
            dr_主["物流信息"] = textBox5.Text;
            dr_主["申请日期"] = t;
            dr_主["预计归还日期"] = dateEdit1.Text;
            dr_主["备注"] = textBox1.Text;
            dr_主["片区"] = lookUpEdit1.Text;
            //2020-4-9
            dr_主["申请人部门"] = CPublic.Var.localUser部门名称;

            // dr["借用人员"] = searchLookUpEdit1.EditValue;
            //还有相关单位      
            if (label8.Text == "客户名称")
            {
                dr_主["相关单位"] = searchLookUpEdit1.Text;
                dr_主["借用人员ID"] = searchLookUpEdit3.EditValue;
                dr_主["借用人员"] = textBox6.Text;
            }
            else
            {
                string departmentID = CPublic.Var.localUser课室编号;
                string dep = "";
                if (departmentID == "") departmentID = CPublic.Var.localUser部门编号;
                string s = string.Format("select 部门名称  from  人事基础部门表 where 部门编号='{0}'", departmentID);
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                if (dt.Rows.Count != 0) dep = dt.Rows[0]["部门名称"].ToString();
                dr_主["相关单位"] = dep;
                dr_主["借用人员ID"] = searchLookUpEdit1.EditValue;
                DataRow[] r = dt_员工信息.Select(string.Format("员工号='{0}'", searchLookUpEdit1.EditValue));
                dr_主["借用人员"] = r[0]["姓名"];

            }

            SqlDataAdapter da;
            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("barrow");

            try
            {


                if (sj == true)
                {
                    string sq131232l = "select * from 销售预订单明细表 where 1<>1";

                    DataTable dt_yu = new DataTable();
                    dt_yu = CZMaster.MasterSQL.Get_DataTable(sq131232l, strcon);
                    DataRow dt_yu_fuzhu = null;


                    string sqlss = string.Format("select * from 销售预订单主表 where 销售预订单号='{0}'  ", str_x预订单号);
                    DataTable dt_主 = CZMaster.MasterSQL.Get_DataTable(sqlss, strcon);

                    foreach (DataRow drasdasd in dt_明细.Rows)
                    {
                        if (drasdasd.RowState == DataRowState.Deleted) continue;
                        string asd = string.Format("select * from 销售预订单明细表 where 销售预订单明细号='{0}'", drasdasd["销售预订单明细号"].ToString());
                        dt_yu_fuzhu = CZMaster.MasterSQL.Get_DataRow(asd, strcon);
                        dt_yu_fuzhu["转换订单数量"] = decimal.Parse(dt_yu_fuzhu["转换订单数量"].ToString()) + decimal.Parse(drasdasd["申请数量"].ToString());
                        dt_yu_fuzhu["未转数量"] = decimal.Parse(dt_yu_fuzhu["未转数量"].ToString()) - decimal.Parse(drasdasd["申请数量"].ToString());
                        if (decimal.Parse(dt_yu_fuzhu["未转数量"].ToString()) < 0)
                        {
                            throw new Exception("数量超过订单数");
                        }
                        if (decimal.Parse(dt_yu_fuzhu["转换订单数量"].ToString()) == decimal.Parse(dt_yu_fuzhu["数量"].ToString()))
                        {
                            dt_yu_fuzhu["完成"] = true;
                        }
                    }
                    // dt_yu=dt_yu_fuzhu.Table.Columns.cop
                    dt_yu.ImportRow(dt_yu_fuzhu);////明细表处理
                    int i2 = 0;
                    foreach (DataRow dataRow in dt_yu.Rows)
                    {
                        if (bool.Parse(dataRow["完成"].ToString()) == true)
                        {
                            i2++;
                        }

                    }
                    if (i2 == dt_yu.Rows.Count)
                    {
                        if (dt_主.Rows.Count > 0)
                        {
                            dt_主.Rows[0]["完成"] = true;
                        }

                    }


                    string sq2l = "select * from 销售预订单明细表 where 1<>1";
                    SqlCommand cmd211 = new SqlCommand(sq2l, conn, ts);
                    da = new SqlDataAdapter(cmd211);
                    new SqlCommandBuilder(da);
                    da.Update(dt_yu);



                    string sq222l = "select * from 销售预订单主表 where 1<>1";
                    SqlCommand ccc123123 = new SqlCommand(sq222l, conn, ts);

                    da = new SqlDataAdapter(ccc123123);
                    new SqlCommandBuilder(da);
                    da.Update(dt_主);


                }





                SqlCommand cmd = new SqlCommand("select * from 借还申请表附表 where 1<>1", conn, ts);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt_明细);

                cmd = new SqlCommand("select * from 借还申请表 where 1<>1", conn, ts);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt_借还申请表);

                drM = dt_借还申请表.Rows[0]; //方便保存后界面重新加载数据

                ts.Commit();
                dt_明细.AcceptChanges();
                dt_借还申请表.AcceptChanges();
            }
            catch (Exception ex)
            {

                ts.Rollback();
                MessageBox.Show(ex.Message);
            }

        }
        private void barLargeButtonItem2_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            gv.CloseEditor();
            this.BindingContext[dt_明细].EndCurrentEdit();
            try
            {
                fun_check();
                fun_save();
                fun_重新加载(drM);
                MessageBox.Show("保存成功!");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem4_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //2020-5-29 订单原件 在基础数据属性表里面 属性类别 借用文件上传_不限制 ,里面得不限制  
                string bm = "select  属性字段1  from 基础数据基础属性表 where  属性类别='借用文件上传_不限制' ";
                DataTable t = CZMaster.MasterSQL.Get_DataTable(bm, strcon);
                bool bl = true;
                if (t.Rows.Count > 0)
                    if (CPublic.Var.LocalUserTeam.StartsWith(t.Rows[0][0].ToString()))
                        bl = false;
                if (bl && checkBox1.Checked == false)
                {
                    throw new Exception("订单原件没有上传");
                }
                if (txt_申请单号.Text == "") throw new Exception("没有需提交的申请");
                string sql = string.Format("select * from 借还申请表 where 申请批号 = '{0}'", txt_申请单号.Text);
                DataTable ttt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                if (ttt.Rows.Count > 0)
                {
                    if (Convert.ToBoolean(ttt.Rows[0]["提交审核"]) == true)
                    {
                        throw new Exception("该单据已提交审核");
                    }
                }
                if (MessageBox.Show(string.Format("该借用申请单是否确认提交审核？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    string departmentID = CPublic.Var.localUser课室编号;
                    string depname = "";
                    if (label8.Text == "客户名称") depname = searchLookUpEdit1.Text;
                    else
                    {
                        if (departmentID == "") departmentID = CPublic.Var.localUser部门编号;
                        string s = string.Format("select 部门名称  from  人事基础部门表 where 部门编号='{0}'", departmentID);
                        DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                        if (dt.Rows.Count > 0)
                        {
                            depname = dt.Rows[0]["部门名称"].ToString();
                        }
                    }
                    DataTable dt_审核 = ERPorg.Corg.fun_PA("生效", "借用申请单", txt_申请单号.Text, depname);




                    string ss = string.Format("select  * from 借还申请表 where 申请批号='{0}'", drM["申请批号"]);
                    DataTable temp = CZMaster.MasterSQL.Get_DataTable(ss, strcon);
                    temp.Rows[0]["提交审核"] = 1;
                    if (temp.Rows[0]["文件GUID"].ToString() == "")
                    {
                        temp.Rows[0]["文件GUID"] = dr_主["文件GUID"].ToString();
                        temp.Rows[0]["订单原件"] = dr_主["订单原件"].ToString();
                        temp.Rows[0]["文件"] = dr_主["文件"].ToString();
                        temp.Rows[0]["上传时间"] = dr_主["上传时间"].ToString();
                        temp.Rows[0]["后缀"] = dr_主["后缀"].ToString();
                    }

                    string asd = string.Format("select * from 销售预订单明细表  where 1<>1");
                    DataTable dt_all = CZMaster.MasterSQL.Get_DataTable(asd, strcon);
                    string sql_ymx = "";
                    foreach (DataRow dr in dt_明细.Rows)
                    {
                        if (dr["销售预订单明细号"].ToString() != "")
                        {
                            sql_ymx = string.Format("select * from  销售预订单明细表 where 销售预订单明细号 = '{0}'", dr["销售预订单明细号"]);
                            SqlDataAdapter da = new SqlDataAdapter(sql_ymx, strcon);
                            da.Fill(dt_all);
                            DataRow[] dt_yu_fuzhu = dt_all.Select(string.Format("销售预订单明细号='{0}'", dr["销售预订单明细号"].ToString()));
                            if (dt_yu_fuzhu.Length > 0)
                            {
                                if (dr["销售预订单明细号"].ToString() != null && dr["销售预订单明细号"].ToString() != "")
                                {
                                    dt_yu_fuzhu[0]["转换订单数量"] = decimal.Parse(dt_yu_fuzhu[0]["转换订单数量"].ToString()) + decimal.Parse(dr["申请数量"].ToString());
                                    dt_yu_fuzhu[0]["未转数量"] = decimal.Parse(dt_yu_fuzhu[0]["未转数量"].ToString()) - decimal.Parse(dr["申请数量"].ToString());
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


                    SqlConnection conn = new SqlConnection(strcon);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("生效");
                    try
                    {

                        SqlCommand cmd = new SqlCommand(ss, conn, ts);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da);
                        da.Update(temp);

                        ss = "select * from 单据审核申请表 where 1<> 1";
                        cmd = new SqlCommand(ss, conn, ts);
                        da = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da);
                        da.Update(dt_审核);

                        ss = "select * from 销售预订单明细表 where 1<> 1";
                        cmd = new SqlCommand(ss, conn, ts);
                        da = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da);
                        da.Update(dt_all);



                        ts.Commit();
                        MessageBox.Show("已提交审核");
                        drM = null;
                    }
                    catch (Exception ex)
                    {
                        ts.Rollback();
                        MessageBox.Show(ex.Message);
                    }

                    //CZMaster.MasterSQL.Save_DataTable(dt_审核, "单据审核申请表", strcon);



                    // barLargeButtonItem5_ItemClick_1(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }
        private void barLargeButtonItem5_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            txt_申请单号.Text = "";
            lookUpEdit1.EditValue = null;
            searchLookUpEdit2.EditValue = null;
            searchLookUpEdit1.EditValue = null;
            searchLookUpEdit3.EditValue = null;
            textBox6.Text = "";
            textBox1.Text = "";
            textBox2.Text = CPublic.Var.localUserName;
            textBox3.Text = CPublic.Var.LocalUserID;
            dt_明细 = dt_明细.Clone();
            gc.DataSource = dt_明细;
            dt_借还申请表 = dt_借还申请表.Clone();
        }

        private void repositoryItemSearchLookUpEdit1View_RowClick_1(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                DataRow dr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
                gv.CellValueChanging -= gv_CellValueChanging;
                DataRow myDataRow = gv.GetDataRow(gv.FocusedRowHandle);
                myDataRow["规格型号"] = dr["规格型号"];
                myDataRow["物料名称"] = dr["物料名称"];
                myDataRow["货架描述"] = dr["货架描述"];
                myDataRow["仓库号"] = dr["仓库号"];
                myDataRow["仓库名称"] = dr["仓库名称"];
                myDataRow["物料编码"] = dr["物料编码"];
                myDataRow["库存总数"] = dr["库存总数"];
                gv.CellValueChanging += gv_CellValueChanging; //加了没用 还是会触发
            }
            catch  
            {
            }
        }

        private void gc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                if (gv.FocusedColumn.Caption == "物料编码") infolink();
            }



        }

        private void gv_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                //gv.CloseEditor();
                this.BindingContext[dt_明细].EndCurrentEdit();
                DataRow dr_focus = gv.GetDataRow(gv.FocusedRowHandle);

                if (e.Column.Caption == "物料编码" && dr_focus["仓库号"].ToString()=="")
                {
                    DataRow rr = dt_基础数据物料信息表.Select(string.Format("物料编码 = '{0}'", e.Value))[0];

                    dr_focus["物料编码"] = e.Value;
                    dr_focus["物料名称"] = rr["物料名称"];
                    dr_focus["规格型号"] = rr["规格型号"];
                    dr_focus["计量单位"] = rr["计量单位"];
                    dr_focus["仓库号"] = rr["仓库号"];
                    dr_focus["仓库名称"] = rr["仓库名称"];
                    string sql1 = string.Format("select * from 仓库物料数量表 where 物料编码='{0}' and 仓库号='{1}'", dr_focus["物料编码"], dr_focus["仓库号"]);
                    DataRow dr = CZMaster.MasterSQL.Get_DataRow(sql1, strcon);
                    if (dr != null)
                    {
                        dr_focus["库存总数"] = dr["库存总数"];
                        dr_focus["货架描述"] = dr["货架描述"];
                        if (dr_focus["库存总数"].ToString() == "")
                        {
                            dr_focus["库存总数"] = 0;
                        }
                    }
                    else
                    {
                        dr_focus["库存总数"] = 0;
                        dr_focus["货架描述"] = "";
                    }
                    if (dr_focus["销售预订单明细号"].ToString() == "")
                    {
                        DataRow[] dr_ymx = dt_未完成.Select(string.Format("物料编码= '{0}' and 可转数量>0", e.Value));
                        if (dr_ymx.Length > 0)
                        {
                            dr_focus.Delete();
                            throw new Exception(e.Value + "物料存在未完成的预订单，请从预订单转单");
                        }
                    }
                }
                if (e.Column.Caption == "仓库号")
                {
                    string sql = string.Format("select * from 仓库物料数量表 where 物料编码='{0}' and 仓库号='{1}'", dr_focus["物料编码"], e.Value);
                    DataRow dr = CZMaster.MasterSQL.Get_DataRow(sql, strcon);

                    string sql4 = string.Format("select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别' and 属性字段1='{0}'  ", e.Value);
                    DataRow wqwds = CZMaster.MasterSQL.Get_DataRow(sql4, strcon);
                    dr_focus["仓库号"] = wqwds["仓库号"];
                    dr_focus["仓库名称"] = wqwds["仓库名称"];
                    if (dr != null)
                    {
                        dr_focus["库存总数"] = dr["库存总数"];
                        if (dr_focus["库存总数"].ToString() == "")
                        {
                            dr_focus["库存总数"] = 0;
                        }
                    }
                    else
                    {
                        dr_focus["库存总数"] = 0;
                    }




                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //if (comboBoxEdit1.Text == "对外客户")
                //{
                //    searchLookUpEdit3.EditValue = null;
                //    textBox6.Text = "";
                //}
                //else 
                if (comboBoxEdit1.Text == "对内人员")
                {
                    searchLookUpEdit3.EditValue = searchLookUpEdit1.EditValue;
                    DataRow[] dr = dt_负责人.Select(string.Format("员工号 = '{0}'", searchLookUpEdit1.EditValue));
                    textBox6.Text = dr[0]["姓名"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void searchLookUpEdit3_EditValueChanged(object sender, EventArgs e)
        {
            DataRow[] dr = dt_负责人.Select(string.Format("员工号 = '{0}'", searchLookUpEdit3.EditValue));
            if (dr.Length > 0)
            {
                textBox6.Text = dr[0]["姓名"].ToString();
            }

        }


        string strcon_FS = CPublic.Var.geConn("FS");
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (dt_借还申请表.Rows.Count == 0)
                {
                    dr_主 = dt_借还申请表.NewRow();
                    dt_借还申请表.Rows.Add(dr_主);
                }
                else
                {
                    dr_主 = dt_借还申请表.Rows[0];
                }
                if (dr_主 == null)
                {
                    throw new Exception("请先新增借用订单！");
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

                    string type = "";
                    //type = pathName.Substring(pathName.LastIndexOf("."), pathName.Length - pathName.LastIndexOf(".")).Replace(".", "");
                    int s = Path.GetFileName(open.FileName).LastIndexOf(".") + 1;
                    type = Path.GetFileName(open.FileName).Substring(s, Path.GetFileName(open.FileName).Length - s);

                    string strguid = "";  //记录系统自动返回的GUID
                    strguid = CFileTransmission.CFileClient.sendFile(open.FileName);
                    dr_主["文件GUID"] = strguid;
                    dr_主["订单原件"] = true;
                    dr_主["文件"] = Path.GetFileName(open.FileName);
                    dr_主["上传时间"] = CPublic.Var.getDatetime();
                    dr_主["后缀"] = type;
                    MessageBox.Show("上传成功！");
                    checkBox1.Checked = true;
                    button2.Enabled = true;
                    button5.Enabled = true;
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
                if (dt_借还申请表.Rows.Count == 0)
                {
                    throw new Exception("没有文件可以下载，请先上传文件");
                }
                else
                {
                    dr_主 = dt_借还申请表.Rows[0];
                }

                //if (dr_主 == null)
                //{
                //    throw new Exception("请重新选择采购订单！");
                //}
                if (dr_主["文件GUID"] == null || dr_主["文件GUID"].ToString() == "")
                {
                    throw new Exception("没有文件可以下载，请先上传文件");
                }

                SaveFileDialog save = new SaveFileDialog();
                // save.Filter = "(*.jpg,*.png,*.jpeg,*.bmp,*.gif)|*.jgp;*.png;*.jpeg;*.bmp;*.gif|All files(*.*)|*.*";
                save.FileName = dr_主["文件"].ToString() + "." + dr_主["后缀"].ToString();
                //save.FileName = drm["文件名"].ToString();

                if (save.ShowDialog() == DialogResult.OK)
                {
                    CFileTransmission.CFileClient.strCONN = strcon_FS;
                    CFileTransmission.CFileClient.Receiver(dr_主["文件GUID"].ToString(), save.FileName);
                    MessageBox.Show("文件下载成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (dt_借还申请表.Rows.Count == 0)
                {
                    throw new Exception("没有文件可以预览，请先上传文件");
                }
                else
                {
                    dr_主 = dt_借还申请表.Rows[0];
                }
                if (dr_主["文件GUID"] == null || dr_主["文件GUID"].ToString() == "")
                {
                    throw new Exception("没有文件可以预览，请先上传文件");
                }
                //string type = dr["后缀"].ToString();

                string foldPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\查看文件\\";
                //string fileName = foldPath + CPublic.Var.getDatetime().ToString("yyyy-MM-dd") + "T" + CPublic.Var.getDatetime().ToString("HH_mm_ss") + "Z" + "_" + Guid.NewGuid().ToString() + "." + type;
                string fileName = foldPath + dr_主["文件"].ToString();

                try
                {
                    System.IO.Directory.Delete(foldPath, true);
                }
                catch (Exception)
                {
                }
                CFileTransmission.CFileClient.strCONN = strcon_FS;
                CFileTransmission.CFileClient.Receiver(dr_主["文件GUID"].ToString(), fileName);
                System.Diagnostics.Process.Start(fileName);
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
                DataTable dt_传 = RBQ.SelectGroupByInto("", dt_明细, "物料编码,sum(申请数量) 数量,销售预订单明细号 ", "", "销售预订单明细号");

                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPSale.dll")));
                Type outerForm = outerAsm.GetType("ERPSale.frm预订单明细选择", false);
                object[] drr = new object[1];
                drr[0] = dt_传;


                 Form ui = Activator.CreateInstance(outerForm, drr) as Form;

                ui.ShowDialog();
                bool flag = Convert.ToBoolean(outerForm.GetField("flag").GetValue(ui));
                DataTable dt_ydd_gxmx = outerForm.GetField("dt_ydd_gxmx").GetValue(ui) as DataTable;
                DataTable dt_ydd_mx = outerForm.GetField("dt_ydd_mx").GetValue(ui) as DataTable;

                if (flag && dt_ydd_gxmx.Rows.Count > 0)
                {

                    dt_未完成 = dt_ydd_mx;
                    foreach (DataRow dr in dt_ydd_gxmx.Rows)
                    {

                        //DataRow[] dr = dtP.Select(string.Format("销售预订单明细号 = '{0}'", drr["销售预订单明细号"]));
                        //if (dr.Length > 0)
                        //{
                        //    dr[0]["数量"] =Convert.ToDecimal(dr[0]["数量"]) + Convert.ToDecimal(drr["此次转单数量"]);
                        //}
                        //else
                        //{
                        DataRow dr_mx = dt_明细.NewRow();
                        dt_明细.Rows.Add(dr_mx);
                        dr_mx["物料编码"] = dr["物料编码"];
                        dr_mx["申请数量"] = Convert.ToDecimal(dr["此次转单数量"]);
                        dr_mx["销售预订单号"] = dr["销售预订单号"];
                        dr_mx["销售预订单明细号"] = dr["销售预订单明细号"];
                        dr_mx["预计出库日期"] = CPublic.Var.getDatetime().Date;

                        //}


                    }
                    infolink();
                }
                //UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;
                //CPublic.UIcontrol.Showpage(ui, "预订单明细选择");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void infolink()
        {
            DateTime t = CPublic.Var.getDatetime().Date;
            foreach (DataRow dr in dt_明细.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                dr["预计出库日期"] = t;
                try
                {
                    if (dr["仓库号"].ToString() == "")
                    {
                        DataRow[] r = dt_物料.Select(string.Format("物料编码='{0}'  ", dr["物料编码"]));

                        dr["物料名称"] = r[0]["物料名称"];
                        dr["物料编码"] = r[0]["物料编码"];
                        dr["规格型号"] = r[0]["规格型号"];

                        dr["库存总数"] = r[0]["库存总数"];
                        dr["货架描述"] = r[0]["货架描述"];
                        dr["计量单位"] = r[0]["计量单位"];

                        dr["仓库号"] = r[0]["默认仓库号"].ToString();
                        dr["仓库名称"] = r[0]["默认仓库"].ToString();
                    }
                }
                catch (Exception)
                {

                }

            }

        }
    }
}
