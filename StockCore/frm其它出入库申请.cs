using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace StockCore
{
#pragma warning disable IDE1006 // 命名样式
    public partial class frm其它出入库申请 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dtM;
        DataTable dtP;
        DataTable dt_出入库子;
        DataRow drM = null;
        DataTable dt_物料;
        DataTable dt_仓库;
        DataTable dt_bom;
        string cfgfilepath = "";
        string s_业务单号 = "";
        string s_原因分类 = "";
        DataRow dr_参数;
        DataTable dt_分类 = new DataTable();
        DataView dv;


        bool s_归还 = false;
        #endregion

        public frm其它出入库申请()
        {
            InitializeComponent();
        }
        public frm其它出入库申请(DataRow dr_c)
        {
            InitializeComponent();
            drM = dr_c;
            if (dr_c["原因分类"].ToString() == "委外加工")
            {    

                simpleButton1.Enabled = false;
                simpleButton2.Enabled = false;
                txt_申请类型.Enabled = false;
                dt_分类.Columns.Add("原因分类");
                dt_分类.Columns.Add("说明");
                DataRow r = dt_分类.NewRow();
                r["原因分类"] = "委外加工";
                dt_分类.Rows.Add(r);
                searchLookUpEdit1.Properties.DataSource = dt_分类;
                searchLookUpEdit1.Properties.DisplayMember = "原因分类";
                searchLookUpEdit1.Properties.ValueMember = "原因分类";
                searchLookUpEdit1.EditValue = "委外加工";
          
                searchLookUpEdit1.Enabled = false;
            }
            string sql = string.Format("select * from 其他出入库申请主表 where 出入库申请单号 = '{0}'",drM["出入库申请单号"]);
            DataTable dt_xiu = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            if (dt_xiu.Rows.Count > 0)
            {
                if(Convert.ToBoolean(dt_xiu.Rows[0]["红字回冲"]) == true)
                {
                    checkBox1.Checked = true;
                    textBox2.Text = drM["业务单号"].ToString();
                    textBox2.Enabled = false;
                    searchLookUpEdit1.Enabled = false;
                    checkBox1.Enabled = false;

                }
            }
        }
        public frm其它出入库申请(string s, string s1, DataTable dt11)
        {
            InitializeComponent();
            s_归还 = true;
            s_业务单号 = s;     
            s_原因分类 = s1;
            dt_出入库子 = dt11;
            //checkBox1.Visible = true;
            checkBox1.Checked = true;
           // checkBox1.Enabled = false;
          //  label9.Visible = true;
            textBox2.Enabled = false;
            simpleButton1.Enabled = false;
            txt_申请类型.Enabled = false;
            searchLookUpEdit1.Enabled = false;

        }
#pragma warning disable IDE1006 // 命名样式
        private void frm其它出入库申请_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DateTime t1= CPublic.Var.getDatetime();
                // DateTime t1 = Convert.ToDateTime("2019-9-30 19:00:00");


                time_申请日期.EditValue = t1;
                // time_申请日期.EditValue =DateTime.Now;
                fun_物料下拉框();
                fun_载入主表明细();

                gc.DataSource = dtP;

                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(this.gc, this.Name, cfgfilepath);
                


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region 方法
#pragma warning disable IDE1006 // 命名样式
        private void fun_载入主表明细()
#pragma warning restore IDE1006 // 命名样式
        {
            if (drM == null)
            {

                string sql = "select * from 其他出入库申请主表 where 1<>1";
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);

                drM = dtM.NewRow();
                dtM.Rows.Add(drM);
                if (s_归还.Equals(true))
                {
                    txt_出入库申请单号.Text = "";
                    txt_申请类型.Text = "其他出库";
                    checkBox1.Enabled = false;
                    DataRow[] ds = dt_分类.Select(string.Format("原因分类 = '{0}'", s_原因分类));
                    if(ds.Length == 0)
                    {
                        DataRow dr_new = dt_分类.NewRow();
                        dt_分类.Rows.Add(dr_new);
                        dr_new["原因分类"] = s_原因分类; 
                    }
                    searchLookUpEdit1.EditValue = s_原因分类;
                    textBox2.Text = s_业务单号;
                    sql = @"select a.*,库存总数,b.仓库名称 from 其他出入库申请子表 a ,仓库物料数量表 b ,基础数据物料信息表 c
                        where   a.物料编码=b.物料编码 and 
                            a.物料编码=c.物料编码 and 1<>1";
                    dtP = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                    DataTable dt_仓 = new DataTable();
                    foreach (DataRow dr_出入库子 in dt_出入库子.Rows)
                    {
                        if (Convert.ToDecimal(dr_出入库子["已完成数量"]) != 0 && Convert.ToDecimal(dr_出入库子["已完成数量"]) > Convert.ToDecimal(dr_出入库子["申请归还总数"]))
                        {
                            string sql111 = "select * from 仓库物料数量表 where 物料编码 = '" + dr_出入库子["物料编码"] + "' and 仓库号 = '" + dr_出入库子["仓库号"] + "'";
                            dt_仓 = CZMaster.MasterSQL.Get_DataTable(sql111, strconn);
                            DataRow dr = dtP.NewRow();
                            dtP.Rows.Add(dr);
                            dr["物料编码"] = dr_出入库子["物料编码"];
                            dr["物料名称"] = dr_出入库子["物料名称"];
                            dr["规格型号"] = dr_出入库子["规格型号"];
                            dr["货架描述"] = dr_出入库子["货架描述"];
                            if (dt_仓.Rows.Count == 0)
                            {
                                dr["库存总数"] = 0;

                            }
                            else
                            {
                                dr["库存总数"] = dt_仓.Rows[0]["库存总数"];
                            }
                            dr["仓库号"] = dr_出入库子["仓库号"];
                            dr["仓库名称"] = dr_出入库子["仓库名称"];
                            dr["数量"] =-(Convert.ToDecimal(dr_出入库子["已完成数量"]) - Convert.ToDecimal(dr_出入库子["申请归还总数"]));
                        }
                    }


                }
                else
                {
                    sql = @"select a.*,库存总数,b.仓库名称 from 其他出入库申请子表 a ,仓库物料数量表 b ,基础数据物料信息表 c
                        where   a.物料编码=b.物料编码 and 
                            a.物料编码=c.物料编码 and 1<>1";
                    dtP = new DataTable();
                    da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dtP);
                }

            }
            else
            {
                string sql = string.Format("select * from 其他出入库申请主表 where 出入库申请单号 = '{0}'", drM["出入库申请单号"].ToString());
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);

                drM = dtM.Rows[0];
                dataBindHelper1.DataFormDR(drM);

                //                string sql2 = string.Format(@"select a.*,b.库存总数  from 其他出入库申请子表 a 
                //                  left join 仓库物料数量表 b on a.物料编码 = b.物料编码
                //                where 出入库申请单号 = '{0}' and a.仓库号=b.仓库号", drM["出入库申请单号"].ToString());
                string sql2 = string.Format(@"select a.*,isnull(kc.库存总数,0)库存总数  from 其他出入库申请子表  a
                  left join 仓库物料数量表 kc on a.物料编码 = kc.物料编码 and a.仓库号=kc.仓库号
                where 出入库申请单号 = '{0}'   order by 委外备注1,委外备注2", drM["出入库申请单号"].ToString()); // 1 组 2优先级
              
                dtP = new DataTable();
                SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
                da2.Fill(dtP);
            }

            //dtP.ColumnChanged += dtP_ColumnChanged;
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bl">指示是普通保存还是生效</param>
        /// 
        private void fun_保存主表明细(Boolean bl)
#pragma warning restore IDE1006 // 命名样式
        {
             DateTime t = CPublic.Var.getDatetime();
           // DateTime t = Convert.ToDateTime("2019-9-30 19:00:00");


            try
            {
                if (txt_出入库申请单号.Text == "")
                {
                    if (drM["GUID"].ToString() == "")
                    {
                        drM["GUID"] = System.Guid.NewGuid();
                        txt_出入库申请单号.Text = string.Format("QWSQ{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                             t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("QWSQ", t.Year, t.Month).ToString("0000"));
                        drM["出入库申请单号"] = txt_出入库申请单号.Text;
                        drM["申请日期"] = t;
                    }
                }
                drM["操作人员编号"] = CPublic.Var.LocalUserID;
                drM["操作人员"] = CPublic.Var.localUserName;
                if (bl == true)
                {
                    drM["生效"] = true;
                    drM["生效人员编号"] = CPublic.Var.LocalUserID;
                    drM["生效日期"] = t;
                }
                //dataBindHelper1.DataToDR(drM);
                drM["备注"] = txt_备注.Text;
                drM["申请类型"] = txt_申请类型.EditValue;
                
                if (textBox2.Text != "")
                {
                    drM["业务单号"] = textBox2.Text;
                }
                
                drM["红字回冲"] = checkBox1.Checked;
                
                drM["部门名称"]=CPublic.Var.localUser部门名称;
                drM["原因分类"] = searchLookUpEdit1.EditValue;
            }
            catch (Exception ex)
            {
                throw new Exception("主表保存出错" + ex.Message);
            }

            try
            {
                int i;
                DataRow[] rr = dtP.Select("POS=Max(POS)");
                if (rr.Length > 0)
                {
                    i = Convert.ToInt32(rr[0]["POS"]) + 1;
                }
                else
                {
                    i = 1;
                }
                foreach (DataRow r in dtP.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    if (r["GUID"].ToString() == "")
                    {
                        r["GUID"] = System.Guid.NewGuid();
                        r["出入库申请单号"] = drM["出入库申请单号"];

                        r["出入库申请明细号"] = drM["出入库申请单号"].ToString() + "-" + i.ToString("00");
                        r["POS"] = i++;
                    }
                    if (bl)
                    {
                        r["生效"] = true;
                        r["生效人员编号"] = CPublic.Var.LocalUserID;
                        r["生效日期"] = t;

                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception("明细保存出错" + ex.Message);
            }
            //18-12-5其他出入库申请不需要审批流  
            //DataTable dt_审核 = new DataTable();
            //if (bl)
            //{
            //    string departmentID = CPublic.Var.localUser课室编号;
            //    string dep = "";
            //    if (departmentID == "") departmentID = CPublic.Var.localUser部门编号;
            //    string s = string.Format("select 部门名称  from  人事基础部门表 where 部门编号='{0}'", departmentID);
            //    DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            //    if (dt.Rows.Count != 0) dep = dt.Rows[0]["部门名称"].ToString();
            //   dt_审核 = ERPorg.Corg.fun_PA("生效", "其他出入库申请单", drM["出入库申请单号"].ToString(), dep);
            //}
            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("生效");
            try
            {

                string sql = "select * from 其他出入库申请主表 where 1<>1";
                SqlCommand cmd = new SqlCommand(sql, conn, ts);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dtM);

                sql = "select * from 其他出入库申请子表 where 1<>1";
                cmd = new SqlCommand(sql, conn, ts);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dtP);
                //18-12-5其他出入库申请不需要审批流  
                //if (bl)
                //{
                //    sql = "select  * from 单据审核申请表 where 1<>1";
                //    cmd = new SqlCommand(sql, conn, ts);
                //    da = new SqlDataAdapter(cmd);
                //    new SqlCommandBuilder(da);
                //    da.Update(dt_审核);
                //}
                ts.Commit();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw ex;
            }
        }
        //不用
#pragma warning disable IDE1006 // 命名样式
        private void dtP_ColumnChanged(object sender, DataColumnChangeEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (e.Column.Caption == "物料编码")
                {
                    DataRow[] ds = dt_物料.Select(string.Format("物料编码 = '{0}'", e.Row["物料编码"]));
                    e.Row["原ERP物料编号"] = ds[0]["原ERP物料编号"];
                    e.Row["物料名称"] = ds[0]["物料名称"];
                    e.Row["n原ERP规格型号"] = ds[0]["n原ERP规格型号"];
                    e.Row["库存总数"] = ds[0]["库存总数"];
                    e.Row["货架描述"] = ds[0]["货架描述"];
                    e.Row["仓库名称"] = ds[0]["仓库名称"];
                    e.Row["仓库号"] = ds[0]["仓库号"];
                    //e.Row["图纸编号"] = ds[0]["图纸编号"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {
            
            bool Bl = true; //指示是否需要检验重复项
            if (searchLookUpEdit1.EditValue != null && (searchLookUpEdit1.EditValue.ToString() == "委外加工" || searchLookUpEdit1.EditValue.ToString() == "研发领料"))
            {
                Bl = false;
            }

            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted)
                {
                    continue;
                }
                try
                {
                    Convert.ToDecimal(dr["数量"]);
                }
                catch (Exception)
                {

                    throw new Exception("数量输入不正确");
                }

                if (Bl)
                {
                    DataRow[] r = dtP.Select(string.Format("物料编码='{0}' and 仓库号='{1}'", dr["物料编码"].ToString(), dr["仓库号"]));
                    if (r.Length > 1)
                    {
                        throw new Exception(string.Format("选择了重复物料{0},请确认", dr["物料编码"]));
                    }
                }
                //if (txt_申请类型.Text == "其他出库" && Convert.ToDecimal(dr["数量"]) > Convert.ToDecimal(dr["库存总数"]))
                //{
                //    throw new Exception(string.Format("选择物料申请数量大于库存数量,物料:{0}", dr["物料编码"]));

                //}

            }

            if (txt_申请类型.EditValue == null || txt_申请类型.Text == "")
            {
                throw new Exception("请选择申请类型");

            }
            if (txt_备注.Text.ToString().Trim() == "")
            {
                throw new Exception("备注已改为必填项,请填写");
            }
            if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
            {
                throw new Exception("请选择原因分类");
            }
            decimal aa = 0;
            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                try
                {
                    decimal a = Convert.ToDecimal(dr["数量"]);

                    decimal b = Convert.ToDecimal(dr["库存总数"]);
                }
                catch (Exception)
                {

                    throw new Exception("请正确输入数量格式");
                }
                if (checkBox1.Checked == false)
                {
                    if (Convert.ToDecimal(dr["数量"]) <= 0)
                    {
                        throw new Exception("数量不能小于0");
                    }
                }
                else
                {
                    if (Convert.ToDecimal(dr["数量"]) >= 0)
                    {
                        throw new Exception("红字回冲单数量需输入负数");
                    }
                    if(textBox2.Text == ""|| textBox2.Text ==null)
                    {
                        throw new Exception("业务单号必填");
                    }

                }

                if (s_归还)
                {
                    DataRow[] dr_1 = dt_出入库子.Select(string.Format("物料编码 = '{0}' and 仓库号 = '{1}'", dr["物料编码"], dr["仓库号"]));
                    if (dr_1.Length > 0)
                    {
                        aa = Convert.ToDecimal(dr["数量"]) + Convert.ToDecimal(dr_1[0]["申请归还总数"]);
                        if (aa > Convert.ToDecimal(dr_1[0]["已处理数量"]))
                        {
                            throw new Exception("申请归还数量超出已借数量");
                        }
                    }
                    aa = 0;
                }
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_物料下拉框()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = @"select base.物料编码,base.物料名称,base.规格型号,base.图纸编号,isnull(a.库存总数,0)库存总数,a.货架描述
           ,a.仓库号,a.仓库名称, base.仓库号 as 默认仓库号,base.仓库名称 as 默认仓库 
           from 基础数据物料信息表 base
            left join 仓库物料数量表 a on base.物料编码 = a.物料编码 and  base.仓库号=a.仓库号  /*where   停用=0*/";
            dt_物料 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_物料);

            repositoryItemSearchLookUpEdit1.DataSource = dt_物料;
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";

            //暂时不用的 不用管先
            string sql_bom = @"select a.物料编码,原ERP物料编号,a.物料名称,a.规格型号,
            a.规格,大类,n原ERP规格型号,库存总数,b.货架描述,b.仓库名称 from 基础数据物料信息表 a,仓库物料数量表 b where  a.物料编码=b.物料编码 and
            a.物料编码 in  (select  产品编码  from 基础数据物料BOM表 group by 产品编码 ) and  停用=0";
            dt_bom = CZMaster.MasterSQL.Get_DataTable(sql_bom, strconn);
            searchLookUpEdit2.Properties.DataSource = dt_bom;
            searchLookUpEdit2.Properties.ValueMember = "物料编码";
            searchLookUpEdit2.Properties.DisplayMember = "原ERP物料编号";
            //


            //dt_分类 = new DataTable();
            //sql = "select  属性值 as 原因分类,属性字段1 as 说明 from  基础数据基础属性表 where 属性类别='原因分类' order by 属性值";
            //dt_分类 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            //if (drM != null && drM["原因分类"].ToString() == "委外加工")
            //{
            //    DataRow dr = dt_分类.NewRow();
            //    dr["原因分类"] = "委外加工";
            //    dt_分类.Rows.Add(dr);
            //    barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            //    barLargeButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            //    barLargeButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            //    barLargeButtonItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            //    txt_备注.Enabled = false;
            //    gridColumn7.OptionsColumn.AllowEdit = false;
            //    gridColumn13.Visible = true;
            //    gridColumn13.VisibleIndex = 9;
            //    gridColumn14.Visible = true;
            //    gridColumn14.VisibleIndex = 10;

            //}
            //searchLookUpEdit1.Properties.DataSource = dt_分类;
            //searchLookUpEdit1.Properties.ValueMember = "原因分类";
            //searchLookUpEdit1.Properties.DisplayMember = "原因分类";


            dt_仓库 = new DataTable();
            string sql4 = "select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别'";
            da = new SqlDataAdapter(sql4, strconn);
            da.Fill(dt_仓库);
            repositoryItemSearchLookUpEdit2.DataSource = dt_仓库;
            repositoryItemSearchLookUpEdit2.DisplayMember = "仓库号";
            repositoryItemSearchLookUpEdit2.ValueMember = "仓库号";

        }
        #endregion

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //新增
            try
            {
                 //DateTime t1 = Convert.ToDateTime("2019 -9-30 19:00:00");
                DateTime t1 = CPublic.Var.getDatetime();

                time_申请日期.EditValue = t1;
                //time_申请日期.EditValue = DateTime.Now;
                s_归还 = false;
                txt_申请类型.Enabled = true;
                searchLookUpEdit1.Enabled = true;
                textBox2.Enabled = true;
                drM = null;
                txt_出入库申请单号.Text = "";
                checkBox1.Checked = false;
                checkBox1.Visible = false;
                textBox2.Text = "";
                txt_备注.Text = "";
                txt_申请类型.EditValue = "";
                fun_载入主表明细();
                gc.DataSource = dtP;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //保存
            try
            {
                gv.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                fun_check();

                fun_保存主表明细(false);
                string sql = string.Format("select  * from 其他出入库申请主表 where 出入库申请单号='{0}'", txt_出入库申请单号.Text);
                drM = CZMaster.MasterSQL.Get_DataRow(sql, strconn);
                sql = string.Format(@"select a.*,kc.库存总数  from 其他出入库申请子表  a
                  left join 仓库物料数量表 kc on a.物料编码 = kc.物料编码
                where 出入库申请单号 = '{0}'  and a.仓库号=kc.仓库号 order by 委外备注1,委外备注2", txt_出入库申请单号.Text);
                dtP = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                gc.DataSource = dtP;
                MessageBox.Show("保存成功");
                // barLargeButtonItem5_ItemClick(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 保存生成 审核单
        /// </summary>
        /// 
        private DataTable fun_PA(string str_采购单号)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow r_upper = ERPorg.Corg.fun_hr_upper("采购单", CPublic.Var.LocalUserID);
            if (r_upper == null)
            {
                throw new Exception("未找到你的上级审核人员");
            }
            string department = CPublic.Var.localUser课室编号;
            if (department == "") department = CPublic.Var.localUser部门编号;
            using (SqlDataAdapter da = new SqlDataAdapter(string.Format("select 部门名称 from  人事基础部门表 where 部门编号='{0}'", department), strconn))
            {
                DataTable temp = new DataTable();
                da.Fill(temp);
                department = temp.Rows[0]["部门名称"].ToString();
            }

            DataTable dt_申请;
            string s = string.Format("select * from  单据审核申请表 where 关联单号='{0}'", str_采购单号);
            dt_申请 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
             DateTime t = CPublic.Var.getDatetime();
            //DateTime t = Convert.ToDateTime("2019-9-30 19:00:00");

            string str_pa = "";
            if (dt_申请.Rows.Count == 0)
            {
                str_pa = string.Format("AP{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("AP", t.Year, t.Month));
                // 申请主表记录
                DataRow r_z = dt_申请.NewRow();
                r_z["审核申请单号"] = str_pa;
                r_z["关联单号"] = txt_出入库申请单号.Text;



                r_z["相关单位"] = department;
                r_z["单据类型"] = "其他出入库申请单";
                //decimal dec = Convert.ToDecimal(txt_cgshhje.Text);
                //r_z["总金额"] = dec;
                r_z["申请人ID"] = CPublic.Var.LocalUserID;
                r_z["申请人"] = CPublic.Var.localUserName;
                r_z["申请时间"] = t;
                r_z["待审核人ID"] = r_upper["工号"];
                r_z["待审核人"] = r_upper["姓名"];

                dt_申请.Rows.Add(r_z);
            }
            else
            {
                str_pa = dt_申请.Rows[0]["审核申请单号"].ToString();
                //decimal dec = Convert.ToDecimal(txt_cgshhje.Text);
                //dt_申请.Rows[0]["总金额"] = dec;

                dt_申请.Rows[0]["相关单位"] = department;
                dt_申请.Rows[0]["待审核人ID"] = r_upper["工号"];
                dt_申请.Rows[0]["待审核人"] = r_upper["姓名"];
                dt_申请.Rows[0]["申请时间"] = t;
                dt_申请.Rows[0]["申请人ID"] = CPublic.Var.LocalUserID;
                dt_申请.Rows[0]["申请人"] = CPublic.Var.localUserName;
                //if (CPublic.Var.LocalUserID == temp.Rows[0]["工号"].ToString())   //下采购单的人和审核人一致 提交上级
                //{
                //    dt_申请.Rows[0]["待审核人ID"] = temp.Rows[1]["工号"];
                //    dt_申请.Rows[0]["待审核人"] = temp.Rows[1]["姓名"];
                //}
            }

            return dt_申请;
        }
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                gv.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                fun_check();
                //  drM["待审核"] = true;



                fun_保存主表明细(true);

                MessageBox.Show("提交成功");
                s_归还 = false;
                //   label9.Visible = false;
                checkBox1.Visible = false;
                barLargeButtonItem5_ItemClick(null, null);
            }
            catch (Exception ex)
            {
                // barLargeButtonItem5_ItemClick(null, null);

                MessageBox.Show(ex.Message);
            }
            //生效
            //try
            //{

            //    gv.CloseEditor();
            //    this.BindingContext[dtP].EndCurrentEdit();
            //    fun_check();

            //    fun_保存主表明细(true);
            //    MessageBox.Show("生效成功");
            //    barLargeButtonItem5_ItemClick(null, null);

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = dtP.NewRow();
            dtP.Rows.Add(dr);
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                int[] dr1 = gv.GetSelectedRows();
                if (dr1.Length > 0)
                {
                    for (int i = dr1.Length - 1; i >= 0; i--)
                    {
                        DataRow dr_选中 = gv.GetDataRow(dr1[i]);
                        dr_选中.Delete();
                    }

                    DataRow drs = gv.GetDataRow(Convert.ToInt32(dr1[0]));
                    if (drs != null) gv.SelectRow(dr1[0]);
                    else if (gv.GetDataRow(Convert.ToInt32(dr1[0]) - 1) != null)
                        gv.SelectRow(Convert.ToInt32(dr1[0]) - 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            txt_出入库申请单号.Text = "";
            txt_申请类型.Text = "";
            searchLookUpEdit1.EditValue = null;
            txt_备注.Text = "";
            checkBox1.Visible = false;
            simpleButton1.Enabled = true;
            fun_物料下拉框();
            string sql = "select * from 其他出入库申请主表 where 1<>1";
            dtM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            // gc.DataSource = dtM;

            barLargeButtonItem1_ItemClick(null, null);
        }
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 东屋暂时不用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton3_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            if (searchLookUpEdit2.EditValue == null || searchLookUpEdit2.EditValue.ToString() == "")
            {
                MessageBox.Show("未选择物料");
            }
            else
            {
                string sql_mx = string.Format(@"select 基础数据物料BOM表.*,基础数据物料信息表.原ERP物料编号,n原ERP规格型号,库存总数,货架描述,仓库名称,仓库号 from 基础数据物料BOM表,基础数据物料信息表,仓库物料数量表
                                                   

where    基础数据物料BOM表.子项编码= 基础数据物料信息表.物料编码 and   基础数据物料BOM表.子项编码= 仓库物料数量表.物料编码 
                                           and  产品编码='{0}'", searchLookUpEdit2.EditValue.ToString());


                //                string sql_mx = string.Format(@"select 基础数据物料BOM表.*,基础数据物料信息表.原ERP物料编号,n原ERP规格型号,库存总数,货架描述,仓库名称,仓库号 from 基础数据物料BOM表,基础数据物料信息表,仓库物料数量表


                //where    基础数据物料BOM表.子项编码= 基础数据物料信息表.物料编码 and   基础数据物料BOM表.子项编码= 仓库物料数量表.物料编码 
                //                                           and  产品编码='{0}'", searchLookUpEdit2.EditValue.ToString());


                //string sql = string.Format(@"select    ");


                DataTable dt = new DataTable();
                dt = CZMaster.MasterSQL.Get_DataTable(sql_mx, strconn);
                foreach (DataRow dr in dt.Rows)
                {
                    DataRow drr = dtP.NewRow();
                    drr["物料编码"] = dr["子项编码"].ToString().Trim();
                    drr["原ERP物料编号"] = dr["原ERP物料编号"];
                    drr["物料名称"] = dr["子项名称"];
                    drr["n原ERP规格型号"] = dr["n原ERP规格型号"];
                    drr["库存总数"] = dr["库存总数"];
                    drr["货架描述"] = dr["货架描述"];
                    drr["仓库名称"] = dr["仓库名称"];
                    drr["仓库号"] = dr["仓库号"];
                    if (textBox1.Text.ToString() != "")
                    {
                        drr["数量"] = Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(textBox1.Text);
                    }
                    else
                    {
                        drr["数量"] = Convert.ToDecimal(dr["数量"]);
                    }

                    dtP.Rows.Add(drr);
                }

            }
        }

        //private void repositoryItemSearchLookUpEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        //{
        //try
        //{
        //    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);

        //    if (e.NewValue != null && e.NewValue.ToString() != "")
        //    {
        //        DataRow[] ds = dt_物料.Select(string.Format("物料编码 = '{0}'", e.NewValue));
        //        dr["原ERP物料编号"] = ds[0]["原ERP物料编号"];
        //        dr["物料名称"] = ds[0]["物料名称"];
        //        dr["n原ERP规格型号"] = ds[0]["n原ERP规格型号"];
        //        dr["库存总数"] = ds[0]["库存总数"];
        //        dr["货架描述"] = ds[0]["货架描述"];
        //        dr["仓库名称"] = ds[0]["仓库名称"];
        //    }
        //    else
        //    {
        //        dr["原ERP物料编号"] = "";
        //        dr["物料名称"] = "";
        //        dr["n原ERP规格型号"] = "";
        //        dr["库存总数"] = "";
        //        dr["货架描述"] = "";
        //        dr["仓库名称"] = "";
        //    }
        //    //e.Row["图纸编号"] = ds[0]["图纸编号"];

        //}
        //catch (Exception ex)
        //{
        //    MessageBox.Show(ex.Message);
        //}
        //}
        //try
        //{
        //    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);

        //    if (e.NewValue != null && e.NewValue.ToString() != "")
        //    {
        //        DataRow[] ds = dt_物料.Select(string.Format("物料编码 = '{0}'", e.NewValue));
        //        dr["原ERP物料编号"] = ds[0]["原ERP物料编号"];
        //        dr["物料名称"] = ds[0]["物料名称"];
        //        dr["n原ERP规格型号"] = ds[0]["n原ERP规格型号"];
        //        dr["库存总数"] = ds[0]["库存总数"];
        //        dr["货架描述"] = ds[0]["货架描述"];
        //        dr["仓库名称"] = ds[0]["仓库名称"];
        //    }
        //    else
        //    {
        //        dr["原ERP物料编号"] = "";
        //        dr["物料名称"] = "";
        //        dr["n原ERP规格型号"] = "";
        //        dr["库存总数"] = "";
        //        dr["货架描述"] = "";
        //        dr["仓库名称"] = "";
        //    }
        //    //e.Row["图纸编号"] = ds[0]["图纸编号"];

        //}
        //catch (Exception ex)
        //{
        //    MessageBox.Show(ex.Message);
        //}
#pragma warning disable IDE1006 // 命名样式
        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        //导入
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                DataTable dt = new DataTable();
                var ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    dt = ERPorg.Corg.ExcelXLSX(ofd);

                    foreach (DataRow dr in dt.Rows)
                    {
                        DataRow rr = dt_物料.NewRow();
                        if (dt_物料.Select(string.Format("原ERP物料编号='{0}'", dr["物料编号"])).Length > 0)
                        {
                            rr = dt_物料.Select(string.Format("原ERP物料编号='{0}'", dr["物料编号"]))[0];
                        }
                        else
                        {
                            string s = string.Format(@"select base.物料编码,base.原ERP物料编号,base.物料名称,base.规格型号,
            base.规格,base.图纸编号,仓库物料数量表.库存总数,货架描述,仓库名称,仓库号 from 基础数据物料信息表 base 
            left join 仓库物料数量表 on base.物料编码 = 仓库物料数量表.物料编码 where base.物料编码='{0}' ", dr["物料编号"]);
                            using (SqlDataAdapter da = new SqlDataAdapter(s, strconn))
                            {
                                DataTable temp = new DataTable();
                                da.Fill(temp);
                                if (temp.Rows.Count > 0)
                                {
                                    dt_物料.ImportRow(temp.Rows[0]);
                                    rr = temp.Rows[0];
                                }
                            }
                        }
                        if (Convert.ToDecimal(rr["库存总数"]) == 0) continue;

                        DataRow r = dtP.NewRow();

                        //  r["原ERP物料编号"] = rr["原ERP物料编号"];
                        r["数量"] = rr["库存总数"];

                        r["物料名称"] = rr["物料名称"];
                        r["n原ERP规格型号"] = rr["n原ERP规格型号"];
                        r["库存总数"] = rr["库存总数"];
                        r["货架描述"] = rr["货架描述"];
                        r["仓库名称"] = rr["仓库名称"];
                        r["仓库号"] = rr["仓库号"];
                        r["物料编码"] = rr["物料编码"].ToString();
                        //dt_物料.Select(string.Format("原ERp物料编号='{0}'", dr["物料编号"]))[0]["物料编码"].ToString();

                        dtP.Rows.Add(r);




                        //                                    s = string.Format(@"select 基础数据物料信息表.物料编码,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.物料名称,基础数据物料信息表.n原ERP规格型号,
                        //            基础数据物料信息表.规格,基础数据物料信息表.图纸编号,仓库物料数量表.库存总数,货架描述,仓库名称 from 基础数据物料信息表 
                        //            left join 仓库物料数量表 on 基础数据物料信息表.物料编码 = 仓库物料数量表.物料编码 where 基础数据物料信息表.原ERp物料编号='{0}' ", dr["物料编码"]);
                        //                                    using (SqlDataAdapter da1 = new SqlDataAdapter(s, strconn))
                        //                                    {
                        //                                        temp = new DataTable();
                        //                                        da1.Fill(temp);

                        //                                        dt_物料.ImportRow(temp.Rows[0]);
                        //                                        rr = temp.Rows[0];
                        //                                        DataRow r = dtP.NewRow();

                        //                                        r["原ERP物料编号"] = rr["原ERP物料编号"];
                        //                                        r["数量"] = dr["库存总数"];

                        //                                        r["物料名称"] = rr["物料名称"];
                        //                                        r["n原ERP规格型号"] = rr["n原ERP规格型号"];
                        //                                        r["库存总数"] = rr["库存总数"];
                        //                                        r["货架描述"] = rr["货架描述"];
                        //                                        r["仓库名称"] = rr["仓库名称"];
                        //                                        r["物料编码"] = rr["物料编码"].ToString();
                        //                                        //dt_物料.Select(string.Format("原ERp物料编号='{0}'", dr["物料编号"]))[0]["物料编码"].ToString();

                        //                                        dtP.Rows.Add(r);
                        //}



                    }





                }
                else
                {
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }










#pragma warning disable IDE1006 // 命名样式
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            DataRow d = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);

            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (dr == null) return;
                if (d != null)
                {


                    dr["物料名称"] = d["物料名称"];
                    dr["物料编码"] = d["物料编码"];

                    dr["规格型号"] = d["规格型号"];
                    dr["库存总数"] = d["库存总数"];
                    dr["货架描述"] = d["货架描述"];
                    dr["仓库号"] = d["仓库号"];
                    dr["仓库名称"] = d["仓库名称"];
                }
                else
                {
                    dr["物料编码"] = "";
                    dr["物料名称"] = "";
                    dr["规格型号"] = "";
                    dr["库存总数"] = "";
                    dr["货架描述"] = "";
                    dr["仓库名称"] = "";
                    dr["仓库号"] = "";
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }



#pragma warning disable IDE1006 // 命名样式
        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow d = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);

            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);

                if (d != null)
                {


                    dr["物料名称"] = d["物料名称"];
                    dr["物料编码"] = d["物料编码"];

                    dr["规格型号"] = d["规格型号"];
                    dr["库存总数"] = d["库存总数"];
                    dr["货架描述"] = d["货架描述"];
                    dr["仓库号"] = d["仓库号"];
                    dr["仓库名称"] = d["仓库名称"];
                }
                else
                {
                    dr["物料编码"] = "";
                    dr["物料名称"] = "";
                    dr["规格型号"] = "";
                    dr["库存总数"] = "";
                    dr["货架描述"] = "";
                    dr["仓库名称"] = "";
                    dr["仓库号"] = "";
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void infolink()
        {
            DateTime t = CPublic.Var.getDatetime().Date.AddDays(1);

            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
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
                        dr["仓库号"] = r[0]["默认仓库号"].ToString();
                        dr["仓库名称"] = r[0]["默认仓库"].ToString();
                    }
                }
                catch (Exception ex)
                {

                }

            }

        }
        private void infolink_stock()
        {
            DateTime t = CPublic.Var.getDatetime().Date.AddDays(1);
            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                try
                {
                    string sql = "select * from 仓库物料数量表 where 物料编码 = '" + dr["物料编码"] + "' and 仓库号 = '" + dr["仓库号"] + "'";
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_物料数量 = new DataTable();
                    da.Fill(dt_物料数量);
                    if (dt_物料数量.Rows.Count == 0)
                    {
                        DataRow[] ds = dt_仓库.Select(string.Format("仓库号 = '{0}'", dr["仓库号"]));
                        dr["仓库名称"] = ds[0]["仓库名称"];
                        dr["库存总数"] = 0;
                    }
                    else
                    {
                        dr["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
                        dr["仓库号"] = dt_物料数量.Rows[0]["仓库号"].ToString();
                        dr["仓库名称"] = dt_物料数量.Rows[0]["仓库名称"].ToString();

                    }



                }
                catch (Exception ex)
                {

                }

            }

        }

        private void gc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                if (gv.FocusedColumn.Caption == "物料编码") infolink();
                else if (gv.FocusedColumn.Caption == "仓库号")
                {
                    infolink_stock();

                }
            }
        }

        private void gv_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (e.Column.FieldName == "仓库号")
                {
                    dr["仓库号"] = e.Value;
                    DataRow[] ds = dt_仓库.Select(string.Format("仓库号 = '{0}'", dr["仓库号"]));
                    dr["仓库名称"] = ds[0]["仓库名称"];
                    string sql = "select * from 仓库物料数量表 where 物料编码 = '" + dr["物料编码"] + "' and 仓库号 = '" + dr["仓库号"] + "'";
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_物料数量 = new DataTable();
                    da.Fill(dt_物料数量);
                    if (dt_物料数量.Rows.Count == 0)
                    {
                        dr["库存总数"] = 0;
                    }
                    else
                    {
                        dr["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
                    }
                }
                else if (e.Column.Caption == "物料编码")
                {

                    dr["物料编码"] = e.Value;
                    DataRow[] ds = dt_物料.Select(string.Format("物料编码 = '{0}'", e.Value));
                    dr["物料名称"] = ds[0]["物料名称"];
                    dr["规格型号"] = ds[0]["规格型号"];
                    dr["库存总数"] = ds[0]["库存总数"];
                    dr["货架描述"] = ds[0]["货架描述"];
                    dr["仓库名称"] = ds[0]["仓库名称"];
                    dr["仓库号"] = ds[0]["仓库号"];
                    //e.Row["图纸编号"] = ds[0]["图纸编号"];
                }
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox2.Text == "")
                {
                    throw new Exception("请输入业务单号");
                }
                string str_单号 = textBox2.Text;
                业务单号查询 fm = new 业务单号查询(str_单号);
                fm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //private void txt_申请类型_EditValueChanged(object sender, EventArgs e)
        //{


        //    string s = txt_申请类型.EditValue.ToString();
        //    if (s.ToString() == "其他入库")
        //    {
        //        label8.Visible = true;
        //        textBox2.Visible = true;
        //        button1.Visible = true;
        //    }
        //    else
        //    {
        //        label8.Visible = false;
        //        textBox2.Visible = false;
        //        button1.Visible = false;
        //    }
        //}


        private void gc_MouseUp(object sender, MouseEventArgs e)
        {
            //try
            //{
            //    if (e.Button == MouseButtons.Left)
            //    {
            //        int[] dr = gv.GetSelectedRows();
            //        if (dr.Length > 1)
            //        {
            //            for (int i=dr.Length-1; i >=0; i--)
            //            {
            //                dr_选中 = gv.GetDataRow(dr[i]);
            //                dr_选中.Delete();

            //            }

            //        }
            //    }
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void gv_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {

                if (gv.GetRowCellValue(e.RowHandle, "数量") != null && gv.GetRowCellValue(e.RowHandle, "数量").ToString() != "" &&
                    gv.GetRowCellValue(e.RowHandle, "库存总数") != null && gv.GetRowCellValue(e.RowHandle, "库存总数").ToString() != "")
                {
                    decimal dec = Convert.ToDecimal(gv.GetRowCellValue(e.RowHandle, "数量"));
                    decimal dec_kc = Convert.ToDecimal(gv.GetRowCellValue(e.RowHandle, "库存总数"));

                    if (dec > dec_kc)
                    {
                        e.Appearance.BackColor = Color.Pink;

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txt_申请类型_Properties_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            try
            {
                if (e.NewValue.ToString() == "其他出库")
                {
                    string sql = "select  属性值 as 原因分类,属性字段1 as 说明 from  基础数据基础属性表 where 属性类别='原因分类' and (属性字段2 = '出库' or 属性字段2 = '') order by 属性值";
                    dt_分类 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    searchLookUpEdit1.Properties.DataSource = dt_分类;
                    searchLookUpEdit1.Properties.ValueMember = "原因分类";
                    searchLookUpEdit1.Properties.DisplayMember = "原因分类";
                    checkBox1.Visible = true;
                    
                    checkBox1.Enabled = true;
                }
                else if (e.NewValue.ToString() == "其他入库")
                {
                    string sql = "select  属性值 as 原因分类,属性字段1 as 说明 from  基础数据基础属性表 where 属性类别='原因分类' and (属性字段2 = '入库' or 属性字段2 = '') order by 属性值";
                    dt_分类 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    searchLookUpEdit1.Properties.DataSource = dt_分类;
                    searchLookUpEdit1.Properties.ValueMember = "原因分类";
                    searchLookUpEdit1.Properties.DisplayMember = "原因分类";
                    checkBox1.Checked = false;
                    checkBox1.Visible = false;
                   
                }
                else
                {
                    dt_分类.Clear();
                }



            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message); ;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
            {
                txt_申请类型.Enabled = false;
                
            }
            else
            {
                txt_申请类型.Enabled = true;
                
            }
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            try

            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                Form出入多选 fm = new Form出入多选();
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

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            try
            {
                infolink_stock();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
