using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace StockCore
{
    public partial class ui材料出库申请 : UserControl
    {

        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dtM;
        DataTable dtP;
        DataTable dt_出入库子;
        DataRow drM = null;
        DataTable dt_物料;
        DataTable dt_仓库, dt_项目;
        DataTable dt_bom;
        DataTable dt_审核;
        string cfgfilepath = "";
        string s_业务单号 = "";
        string s_原因分类 = "";
        DataRow dr_参数;
        DataTable dt_分类 = new DataTable();
        DataView dv;

        bool s_归还 = false;
        bool s_新增 = false;
        bool s_修改 = false;
        bool b_v = false;

        #endregion



        public ui材料出库申请()
        {
            InitializeComponent();
            fun_物料下拉框();

        }


        public ui材料出库申请(DataRow dr_c)
        {
            InitializeComponent();
            drM = dr_c;
            s_修改 = true;
            fun_物料下拉框();
            if (dr_c["原因分类"].ToString() == "委外加工")
            {
                simpleButton1.Enabled = false;
                simpleButton2.Enabled = false;
                // txt_申请类型.Enabled = false;
                searchLookUpEdit1.Enabled = false;
            }
            string sql = string.Format("select * from 其他出入库申请主表 where 出入库申请单号 = '{0}'", drM["出入库申请单号"]);
            DataTable dt_xiu = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            if (dt_xiu.Rows.Count > 0)
            {
                if (Convert.ToBoolean(dt_xiu.Rows[0]["红字回冲"]) == true)
                {
                    checkBox1.Checked = true;
                    textBox2.Text = drM["业务单号"].ToString();
                    textBox2.Enabled = false;
                    searchLookUpEdit1.Enabled = false;
                    checkBox1.Enabled = false;

                }
            }
        }
        //查看 
        public ui材料出库申请(DataRow dr_c, bool bl_查看)
        {
            InitializeComponent();
            drM = dr_c;
            fun_物料下拉框();
            bl_查看 = true;
            panel3.Visible = false;
            barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barLargeButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            gv.OptionsBehavior.Editable = false;
            if (dr_c["原因分类"].ToString() == "委外加工")
            {
                simpleButton1.Enabled = false;
                simpleButton2.Enabled = false;
                // txt_申请类型.Enabled = false;
                searchLookUpEdit1.Enabled = false;
            }
            string sql = string.Format("select * from 其他出入库申请主表 where 出入库申请单号 = '{0}'", drM["出入库申请单号"]);
            DataTable dt_xiu = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            if (dt_xiu.Rows.Count > 0)
            {
                if (Convert.ToBoolean(dt_xiu.Rows[0]["红字回冲"]) == true)
                {
                    checkBox1.Checked = true;
                    textBox2.Text = drM["业务单号"].ToString();
                    textBox2.Enabled = false;
                    searchLookUpEdit1.Enabled = false;
                    checkBox1.Enabled = false;

                }
            }
        }



        public ui材料出库申请(string s, string s1, DataTable dt11)
        {
            InitializeComponent();
            fun_物料下拉框();
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
            //txt_申请类型.Enabled = false;
            searchLookUpEdit1.Enabled = false;

        }


        private void txt_申请类型_Properties_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {

        }

        private void ui材料出库申请_Load(object sender, EventArgs e)
        {
            try
            {
                time_申请日期.EditValue = CPublic.Var.getDatetime();
                //time_申请日期.EditValue = Convert.ToDateTime("2019-9-25 19:00:00");
                // time_申请日期.EditValue =DateTime.Now;
                //fun_物料下拉框(); 每个构造函数里加
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

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            txt_出入库申请单号.Text = "";
            //txt_申请类型.Text = "";
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
        #region 方法
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
                        dr["计量单位"] = r[0]["计量单位"];
                        dr["计量单位编码"] = r[0]["计量单位编码"];
                    }
                }
                catch (Exception)
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
                        DataRow[] ds = dt_仓库.Select(string.Format("仓库号 = {0}", dr["仓库号"]));
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
                catch (Exception)
                {

                }

            }

        }

        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {

            bool Bl = true; //指示是否需要检验重复项
            if (searchLookUpEdit1.EditValue != null && (searchLookUpEdit1.EditValue.ToString() == "委外加工" || searchLookUpEdit1.EditValue.ToString() == "研发领料"))
            {
                Bl = false;
            }
            if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() == "研发领料")
            {
                if (searchLookUpEdit2.EditValue == null || searchLookUpEdit2.EditValue.ToString() == "")
                {
                    throw new Exception("研发领料未加项目 ");
                }
            }

            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted)
                {
                    continue;
                }
                decimal dec = 0;
                if (!decimal.TryParse(dr["数量"].ToString(), out dec))
                {
                    throw new Exception("数量输入不正确");
                }
                if (!decimal.TryParse(dr["库存总数"].ToString(), out dec))
                {
                    throw new Exception("仓库有误请确认");
                }
                //try
                //{
                //    Convert.ToDecimal(dr["数量"]);
                //}
                //catch (Exception)
                //{
                //    throw new Exception("数量输入不正确");

                //}

                if (Bl)
                {
                    DataRow[] r = dtP.Select(string.Format("物料编码='{0}' and 仓库号='{1}'", dr["物料编码"].ToString(), dr["仓库号"]));
                    if (r.Length > 1)
                    {
                        throw new Exception(string.Format("选择了重复物料{0},请确认", dr["物料编码"]));
                    }
                }
                if (Convert.ToDecimal(dr["数量"]) > Convert.ToDecimal(dr["库存总数"]))
                {
                    throw new Exception(string.Format("选择物料申请数量大于库存数量,物料:{0}", dr["物料编码"]));

                }
            }
            DataTable dt_xg;
            if (txt_出入库申请单号.Text != "")
            {
                foreach (DataRow dr_修改 in dtP.Rows)
                {
                    if (dr_修改.RowState == DataRowState.Deleted) continue;

                    string sql = string.Format("select * from  其他出入库申请子表 where 出入库申请明细号 = '{0}'", dr_修改["出入库申请明细号"]);
                    dt_xg = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt_xg.Rows.Count > 0)
                    {
                        if (Convert.ToDecimal(dt_xg.Rows[0]["已完成数量"].ToString()) > 0)
                        {
                            if (Convert.ToDecimal(dr_修改["数量"]) != Convert.ToDecimal(dt_xg.Rows[0]["数量"]))
                            {
                                throw new Exception("该物料已有出库记录，不可修改");
                            }
                        }
                    }
                }
            }

            //if (txt_申请类型.EditValue == null || txt_申请类型.Text == "")
            //{
            //    throw new Exception("请选择申请类型");

            //}
            //if (txt_备注.Text.ToString().Trim() == ""&& searchLookUpEdit1.EditValue.ToString() != "研发领料")
            //{
            //    throw new Exception("备注已改为必填项,请填写");
            //}
            if (txt_备注.Text.ToString().Trim() == "")
            {
                throw new Exception("表头备注为必填项,请填写");
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
                    if (Convert.ToDecimal(dr["数量"]) < 0)
                    {
                        throw new Exception("数量不能小于0");
                    }
                    else if (Convert.ToDecimal(dr["数量"]) == 0)
                    {
                        throw new Exception("数量不能为0");
                    }
                }
                else
                {
                    if (Convert.ToDecimal(dr["数量"]) >= 0)
                    {
                        throw new Exception("红字回冲单数量需输入负数");
                    }
                    if (textBox2.Text == "" || textBox2.Text == null)
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
                    //txt_申请类型.Text = "其他出库";
                    checkBox1.Enabled = false;
                    DataRow[] ds = dt_分类.Select(string.Format("原因分类 = '{0}'", s_原因分类));
                    if (ds.Length == 0)
                    {
                        DataRow dr_new = dt_分类.NewRow();
                        dt_分类.Rows.Add(dr_new);
                        dr_new["原因分类"] = s_原因分类;
                    }
                    searchLookUpEdit1.EditValue = s_原因分类;
                    textBox2.Text = s_业务单号;
                    sql = @"select a.*,库存总数,b.仓库名称  from 其他出入库申请子表 a ,仓库物料数量表 b ,基础数据物料信息表 c
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
                            dr["数量"] = -(Convert.ToDecimal(dr_出入库子["已完成数量"]) - Convert.ToDecimal(dr_出入库子["申请归还总数"]));
                        }
                    }


                }
                else
                {
                    sql = @"select a.*,库存总数,b.仓库名称  from 其他出入库申请子表 a ,仓库物料数量表 b ,基础数据物料信息表 c
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
                searchLookUpEdit2.EditValue = drM["项目号"];

                //                string sql2 = string.Format(@"select a.*,b.库存总数  from 其他出入库申请子表 a 
                //                  left join 仓库物料数量表 b on a.物料编码 = b.物料编码
                //                where 出入库申请单号 = '{0}' and a.仓库号=b.仓库号", drM["出入库申请单号"].ToString());
                string sql2 = string.Format(@"select a.*,isnull(kc.库存总数,0)库存总数  from 其他出入库申请子表  a
                  left join 仓库物料数量表 kc on a.物料编码 = kc.物料编码 and a.仓库号=kc.仓库号
                where 出入库申请单号 = '{0}'   order by  pos", drM["出入库申请单号"].ToString()); // 1 组 2优先级
                //2020-5-19
                //委外备注1,委外备注2 之前按前面条件 排序 我忘记为什么了  现在研发要求顺序不变 

                dtP = new DataTable();
                SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
                da2.Fill(dtP);
            }

            //dtP.ColumnChanged += dtP_ColumnChanged;
        }

        private void fun_保存主表明细(Boolean bl)
#pragma warning restore IDE1006 // 命名样式
        {
            DateTime t = CPublic.Var.getDatetime();
            //DateTime t =Convert.ToDateTime("2019-9-25 19:00:00");

            try
            {
                if (txt_出入库申请单号.Text == "")
                {
                    s_新增 = true;
                    if (drM["GUID"].ToString() == "")
                    {
                        drM["GUID"] = System.Guid.NewGuid();
                        txt_出入库申请单号.Text = string.Format("DWLS{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                             t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("DWLS", t.Year, t.Month).ToString("0000"));
                        drM["出入库申请单号"] = txt_出入库申请单号.Text;
                        drM["申请日期"] = t;
                    }
                }
                drM["操作人员编号"] = CPublic.Var.LocalUserID;
                drM["操作人员"] = CPublic.Var.localUserName;
                if (bl == true)
                {
                    //drM["生效"] = true;
                    //drM["生效人员编号"] = CPublic.Var.LocalUserID;
                    //drM["生效日期"] = t;
                    drM["待审核"] = true;

                    dt_审核 = ERPorg.Corg.fun_PA("生效", "材料出库申请", txt_出入库申请单号.Text, textBox1.Text);
                }
                //dataBindHelper1.DataToDR(drM);
                drM["备注"] = txt_备注.Text;
                //   drM["申请类型"] = txt_申请类型.EditValue;

                if (textBox2.Text != "")
                {
                    drM["业务单号"] = textBox2.Text;
                }
                drM["红字回冲"] = checkBox1.Checked;
                drM["部门名称"] = CPublic.Var.localUser部门名称;
                drM["申请类型"] = "材料出库";
                drM["单据类型"] = "材料出库";
                drM["原因分类"] = searchLookUpEdit1.EditValue.ToString();
                if (searchLookUpEdit1.EditValue.ToString() == "研发领料")
                {
                    drM["项目号"] = searchLookUpEdit2.EditValue.ToString();
                    drM["项目名称"] = searchLookUpEdit2.Text.ToString();
                }

                int i;
                bool bl_判断 = true;
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
                    else if (r.RowState == DataRowState.Added) bl_判断 = false;

                    if (r["GUID"].ToString() == "")
                    {
                        r["GUID"] = System.Guid.NewGuid();
                        r["出入库申请单号"] = drM["出入库申请单号"];

                        r["出入库申请明细号"] = drM["出入库申请单号"].ToString() + "-" + i.ToString("00");
                        r["POS"] = i++;
                    }
                    //if (bl)
                    //{
                    //    r["生效"] = true;
                    //    r["生效人员编号"] = CPublic.Var.LocalUserID;
                    //    r["生效日期"] = t;

                    //}
                    if (s_新增 == false)
                    {
                        if (r["已完成数量"] == null || r["已完成数量"].ToString() == "")
                        {
                            bl_判断 = false;
                        }
                        else
                        {
                            if (checkBox1.Checked)
                            {
                                if (Convert.ToDecimal(r["已完成数量"]) > Convert.ToDecimal(r["数量"]))
                                {
                                    bl_判断 = false;
                                }
                            }
                            else
                            {
                                if (Convert.ToDecimal(r["已完成数量"]) < Convert.ToDecimal(r["数量"]))
                                {
                                    bl_判断 = false;
                                }
                            }

                        }

                    }
                }

                if (s_新增 == false)
                {
                    if (bl_判断)
                    {
                        drM["完成"] = true;
                        drM["完成日期"] = t;
                    }
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
                    if (bl)
                    {
                        sql = "select * from 单据审核申请表 where 1<>1";
                        cmd = new SqlCommand(sql, conn, ts);
                        da = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da);
                        da.Update(dt_审核);
                    }


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
            catch (Exception ex)
            {
                throw new Exception("主表保存出错" + ex.Message);
            }
        }
        #endregion

        private void fun_物料下拉框()
        {
            string sql = @"select base.物料编码,base.物料名称,base.规格型号,base.图纸编号,isnull(a.库存总数,0)库存总数,a.货架描述
           ,a.仓库号,a.仓库名称, base.仓库号 as 默认仓库号,base.仓库名称 as 默认仓库, base.计量单位编码, base.计量单位,base.停用
           from 基础数据物料信息表 base
            left join 仓库物料数量表 a on base.物料编码 = a.物料编码 and  base.仓库号=a.仓库号 where left(base.物料编码,2) not in ('30','20')"  /*where   停用=0*/ ;
            dt_物料 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_物料);

            repositoryItemSearchLookUpEdit1.DataSource = dt_物料;
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";

            //暂时不用的 不用管先
            //string sql_bom = @"select a.物料编码,原ERP物料编号,a.物料名称,a.规格型号,
            //a.规格,大类,n原ERP规格型号,库存总数,b.货架描述,b.仓库名称 from 基础数据物料信息表 a,仓库物料数量表 b where  a.物料编码=b.物料编码 and
            //a.物料编码 in  (select  产品编码  from 基础数据物料BOM表 group by 产品编码 ) and  停用=0";
            //dt_bom = CZMaster.MasterSQL.Get_DataTable(sql_bom, strconn);
            //searchLookUpEdit2.Properties.DataSource = dt_bom;
            //searchLookUpEdit2.Properties.ValueMember = "物料编码";
            //searchLookUpEdit2.Properties.DisplayMember = "原ERP物料编号"; 
            dt_仓库 = new DataTable();
            string sql4 = "select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别' and 布尔字段5 = 1";
            da = new SqlDataAdapter(sql4, strconn);
            da.Fill(dt_仓库);
            repositoryItemSearchLookUpEdit2.DataSource = dt_仓库;
            repositoryItemSearchLookUpEdit2.DisplayMember = "仓库号";
            repositoryItemSearchLookUpEdit2.ValueMember = "仓库号";
            string s = "";

            if (!CPublic.Var.LocalUserTeam.Contains("管理员") && CPublic.Var.LocalUserID != "admin")
            {
                s = string.Format(" and 属性值 in (select 原因分类 from 部门原因分类配置表  where 部门编号='{0}') ", CPublic.Var.localUser部门编号);
            }
            sql = string.Format(@"select  属性值 as 原因分类,属性字段1 as 说明 from  基础数据基础属性表 
            where 属性类别='原因分类' and (属性字段2 = '材料出库' or 属性字段2 = '') {0} order by 属性值", s);
            dt_分类 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            //DataRow dr = dt_分类.NewRow();
            //dt_分类.Rows.Add(dr);
            //dr["原因分类"] = "入库倒冲";

            searchLookUpEdit1.Properties.DataSource = dt_分类;
            searchLookUpEdit1.Properties.ValueMember = "原因分类";
            searchLookUpEdit1.Properties.DisplayMember = "原因分类";

            sql = "select * from 基础信息项目管理表 where 状态='在研'";

            dt_项目 = new DataTable();

            dt_项目 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            searchLookUpEdit2.Properties.DataSource = dt_项目;
            searchLookUpEdit2.Properties.ValueMember = "项目号";
            searchLookUpEdit2.Properties.DisplayMember = "项目名称";

            textBox1.Text = CPublic.Var.localUser部门名称;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                time_申请日期.EditValue = CPublic.Var.getDatetime();
                searchLookUpEdit1.Enabled = true;
                textBox2.Enabled = true;
                drM = null;
                txt_出入库申请单号.Text = "";
                checkBox1.Checked = false;
                checkBox1.Visible = false;
                textBox2.Text = "";
                txt_备注.Text = "";
                // txt_申请类型.EditValue = "";
                fun_载入主表明细();
                gc.DataSource = dtP;
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
                gv.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                fun_check();

                fun_保存主表明细(false);
                string sql = string.Format("select  * from 其他出入库申请主表 where 出入库申请单号='{0}'", txt_出入库申请单号.Text);
                drM = CZMaster.MasterSQL.Get_DataRow(sql, strconn);
                sql = string.Format(@"select a.*,kc.库存总数  from 其他出入库申请子表  a
                  left join 仓库物料数量表 kc on a.物料编码 = kc.物料编码
                where 出入库申请单号 = '{0}'  and a.仓库号=kc.仓库号 order by  pos ", txt_出入库申请单号.Text);
                dtP = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                gc.DataSource = dtP;
                MessageBox.Show("保存成功");
                drM.AcceptChanges();

                // barLargeButtonItem5_ItemClick(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DataRow dr = dtP.NewRow();
            dtP.Rows.Add(dr);
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt_材料申请子 = new DataTable();
                if (txt_出入库申请单号.Text != "")
                {
                    string sql = string.Format("select * from 其他出入库申请子表 where 出入库申请单号 = '0'", txt_出入库申请单号.Text);
                    dt_材料申请子 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                }
                int[] dr1 = gv.GetSelectedRows();
                if (dr1.Length > 0)
                {
                    for (int i = dr1.Length - 1; i >= 0; i--)
                    {
                        DataRow dr_选中 = gv.GetDataRow(dr1[i]);
                        if (dt_材料申请子.Rows.Count > 0)
                        {
                            DataRow[] dr_申请子 = dt_材料申请子.Select(string.Format("出入库申请明细号 = '{0}'", dr_选中["出入库申请明细号"]));
                            if (dr_申请子.Length > 0)
                            {
                                if (Convert.ToDecimal(dr_申请子[0]["已完成数量"]) > 0)
                                {
                                    throw new Exception(dr_选中["物料编码"] + "有已完成数量，不可删除");
                                }
                            }
                        }
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

        private void gv_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (e.Column.Caption == "物料编码")
                {

                    dr["物料编码"] = e.Value;
                    DataRow[] ds = dt_物料.Select(string.Format("物料编码 = '{0}'", dr["物料编码"].ToString()));

                    dr["物料名称"] = ds[0]["物料名称"];
                    dr["规格型号"] = ds[0]["规格型号"];
                    dr["库存总数"] = ds[0]["库存总数"];
                    dr["货架描述"] = ds[0]["货架描述"];
                    dr["仓库名称"] = ds[0]["仓库名称"];
                    dr["仓库号"] = ds[0]["仓库号"];
                    dr["计量单位"] = ds[0]["计量单位"];
                    dr["计量单位编码"] = ds[0]["计量单位编码"];
                    //e.Row["图纸编号"] = ds[0]["图纸编号"];
                }
                else if (e.Column.FieldName == "仓库号")
                {
                    dr["仓库号"] = e.Value;
                    DataRow[] ds = dt_仓库.Select(string.Format("仓库号 = {0}", dr["仓库号"]));
                    dr["仓库名称"] = ds[0]["仓库名称"];
                    string sql = "select * from 仓库物料数量表 where 物料编码 = '" + dr["物料编码"] + "' and 仓库号 = '" + dr["仓库号"] + "'";
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_物料数量 = new DataTable();
                    da.Fill(dt_物料数量);
                    if (dt_物料数量.Rows.Count == 0)
                    {
                        dr["库存总数"] = 0;
                        dr["货架描述"] = "";
                    }
                    else
                    {
                        dr["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
                        dr["货架描述"] = dt_物料数量.Rows[0]["货架描述"];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        private void gv_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
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

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

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

        private void barLargeButtonItem2_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
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
                where 出入库申请单号 = '{0}'  and a.仓库号=kc.仓库号 order by pos ", txt_出入库申请单号.Text);
                dtP = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                gc.DataSource = dtP;
                MessageBox.Show("保存成功");
                fun_载入主表明细();


                // barLargeButtonItem5_ItemClick(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
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

        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEdit1.EditValue.ToString() == "研发领料")
                {
                    label3.Visible = true;
                    searchLookUpEdit2.Visible = true;
                }
                else
                {
                    label3.Visible = false;
                    searchLookUpEdit2.Visible = false;
                }
            }
            catch (Exception)
            {


            }

        }

        private void simpleButton3_Click(object sender, EventArgs e)
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
        //2020-6-1 红字允许检验库  蓝字不允许 选检验库
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (dt_仓库 != null)
            {
                if (checkBox1.Checked) //红字可以回检验1  96     检验2  97  
                {

                    DataRow[] rr = dt_仓库.Select("仓库号='96'");
                    if (rr.Length == 0)
                    {
                        dt_仓库.Rows.Add(new object[] { "96", "检验1" });
                        dt_仓库.Rows.Add(new object[] { "97", "检验2" });

                    }
                }
                else
                {
                    DataRow[] r_dtp = dtP.Select("仓库号='96' or 仓库号='97'");
                    if (r_dtp.Length > 0)
                    {
                        foreach (DataRow r in r_dtp)
                        {
                            r["仓库号"] = "";

                            r["仓库名称"] = "";
                            r["库存总数"] = DBNull.Value;

                        }
                    }
                    DataRow[] rr = dt_仓库.Select("仓库号='96' or 仓库号='97' ");
                    if (rr.Length >= 0)
                    {
                        for (int i = rr.Length - 1; i >= 0; i--)
                            dt_仓库.Rows.Remove(rr[i]);

                    }
                }
            }
        }

        private void simpleButton4_Click(object sender, EventArgs e)
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
