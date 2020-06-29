using DevExpress.Utils;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList.Nodes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace BaseData
{
    public partial class frm基础物料数据信息 : UserControl
    {
        //如果添加或删除界面字段，需要更改4处
        #region 成员
        DataTable dtM;   // xtra3
                         //DataTable dt1;   // xtra2 扩展属性表
                         /// <summary>
                         ///  //1表示新增；2表示查询，用于修改
                         /// </summary>
        int strNo = 0;
        SqlDataAdapter da;
        string strshow;
        int a_查询时使用;
        // int a2 = 1;
        //int a1 = 1;
        public string str_物料编码 = "";
        public string str_物料名称 = "";
        public string str_规格 = "";
        public string str_原规格型号 = "";
        string strconn = CPublic.Var.strConn;
        string s_页面 = "";

        // DataRow rrr; //???不知道什么作用2016 7 29

        DataView dv;       ///显示旧数据
        // DataTable dt_stock;
        DataTable dt_供应商;
        DataTable dt_车间;
        DataTable dt_班组;
        DataTable dt_属性;
        DataTable dt_员工;
        DataTable dt_单位;

        DataTable dt_unit; //计量单位
        public DataTable dt_成员;
        string str_新增or修改 = "";
        DataTable dt_保存修改 = null;
        string cfgfilepath = "";
        #endregion

        #region 自用类
        public frm基础物料数据信息()
        {
            InitializeComponent();
        }
        string a_存货分类编码 = "";
        public static class aaaa
        {
            public static List<frm基础数据物料BOM> FM2 = new List<frm基础数据物料BOM>();

            public static void fun_(string str, string strr, string strrr)
            {
                foreach (frm基础数据物料BOM fm in FM2)
                {
                    fm.str_物料编码 = str;
                    fm.str_物料名称 = strr;
                    fm.str_规格 = strrr;

                    fm.fun_载入数据();

                }
            }
        }

        private void frm基础物料数据信息_Load(object sender, EventArgs e)
        {
            try
            {
                barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                //string permgroup = CPublic.Var.LocalUserTeam;
                //string s = string.Format("select * from [权限组按钮权限表] where 权限组='{0}' and 权限类型='员工信息维护' ", permgroup);
                //DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                //foreach (object item in barManager1.Items)
                //{
                //    if (item.GetType() == typeof(DevExpress.XtraBars.BarLargeButtonItem))
                //    {
                //        DevExpress.XtraBars.BarLargeButtonItem x = item as DevExpress.XtraBars.BarLargeButtonItem;
                //        x.Enabled = ERPorg.Corg.btn_perm(t, x.Caption);

                //    }
                //    //item.Enabled = ERPorg.Corg.btn_perm(t,item.Caption);

                //}
                string sql = string.Format("select * from 人事基础员工表 where 员工号  = '{0}'", CPublic.Var.LocalUserID);
                dt_员工 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                if (dt_员工.Rows.Count > 0)
                {
                    if (dt_员工.Rows[0]["权限组"].ToString() != "管理员权限" && dt_员工.Rows[0]["权限组"].ToString() != "供应链权限" && dt_员工.Rows[0]["权限组"].ToString() != "开发部权限" && dt_员工.Rows[0]["权限组"].ToString() != "工艺部权限")
                    {
                        barLargeButtonItem2.Enabled = false;
                        barLargeButtonItem3.Enabled = false;
                        // barLargeButtonItem4.Enabled = false;
                        barLargeButtonItem5.Enabled = false;
                        barLargeButtonItem6.Enabled = false;
                        barLargeButtonItem8.Enabled = false;
                    }
                }

                if (CPublic.Var.localUser部门编号 == "00010602") //仓库
                {
                    pan_采购.Enabled = false;
                    pan_开发.Enabled = false;
                    pan_开发2.Enabled = false;
                    pan_仓库.Enabled = true;
                    contextMenuStrip1.Enabled = false;
                }
                else if (CPublic.Var.localUser部门编号.Contains("000104") || CPublic.Var.localUser部门编号.Contains("000105"))
                {
                    pan_采购.Enabled = false;
                    pan_开发.Enabled = false;
                    pan_开发2.Enabled = false;
                    pan_仓库.Enabled = false;
                    contextMenuStrip1.Enabled = false;
                }
                else if (CPublic.Var.LocalUserTeam == "开发部权限" || CPublic.Var.LocalUserTeam.Contains("管理员"))
                {
                    pan_采购.Enabled = true;
                    pan_开发.Enabled = true;
                    pan_开发2.Enabled = true;
                    pan_仓库.Enabled = true;
                    contextMenuStrip1.Enabled = true;
                }
                else if (CPublic.Var.LocalUserTeam == "采购权限" || CPublic.Var.localUser部门编号.Contains("000107"))
                {
                    pan_采购.Enabled = true;
                    pan_开发.Enabled = false;
                    pan_开发2.Enabled = false;
                    pan_仓库.Enabled = false;
                    contextMenuStrip1.Enabled = false;
                }
                else if (CPublic.Var.localUser部门编号 == "00010601")
                {
                    pan_采购.Enabled = false;
                    pan_开发.Enabled = false;
                    pan_开发2.Enabled = false;
                    pan_仓库.Enabled = true;
                    contextMenuStrip1.Enabled = false;

                }
                else
                {
                    pan_采购.Enabled = false;
                    pan_开发.Enabled = false;
                    pan_开发2.Enabled = false;
                    pan_仓库.Enabled = false;
                    contextMenuStrip1.Enabled = false;
                    barLargeButtonItem4.Enabled = false;
                }
        
                fun_载入刷新();

                //checkBox5.Enabled = false;
                //checkBox4.Enabled = false;
                BeginInvoke(new MethodInvoker(() =>
                {
                    fun_下拉框();
                    fun_下拉框searchlookup();
                    BaseData.frm基础数据物料BOM fm = new BaseData.frm基础数据物料BOM();
                    fm.Dock = System.Windows.Forms.DockStyle.Fill;
                    //xtra.SelectedTabPage = xtraTabPage4;
                    xtraTabPage4.Controls.Add(fm);
                    //CZMaster.DevGridControlHelper.Helper(this);
                    //fun_载入数据(); //基础数据界面  用于快速选择数据
                    xtra.SelectedTabPage = xtraTabPage4;
                    xtra.SelectedTabPage = xtraTabPage1;
                    frm基础数据物料BOM.XTC = this.xtra;
                }));


                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(this.xtra, this.Name, cfgfilepath);
                //fm基础数据包装清单_物料信息扩展界面.XTC = this.xtra;
                // frm基础数据物料替换.XTC = this.xtra;
                //frm成品检验盒贴信息维护.XTC = this.xtra;
                //UI基础数据BOM信息复制.XTC = this.xtra;
                //ui蓝图维护.XTC = this.xtra;
                //ui物料小标签信息维护.XTC = this.xtra;
                // ui作业指导书上传.XTC = this.xtra;
                //计量器具台账.UI维护.XTC = this.xtra;

                // xtra.SelectedTabPage = xtraTabPage5;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 给所有下拉框赋值
        /// </summary>
        public void fun_下拉框()
        {
            cb3_产品线.Properties.Items.Clear();
            //cb6_大类.Properties.Items.Clear();
            //cb5_电压.Properties.Items.Clear();
            //cb8_极数.Properties.Items.Clear();
            tb6_产品类别.Properties.Items.Clear();
            cb1_物料等级.Properties.Items.Clear();
            //cb10_壳架等级.Properties.Items.Clear();
            cb12.Properties.Items.Clear();
            cb2.Properties.Items.Clear();
            //cb9.Properties.Items.Clear();
            //cb_物料属性.Properties.Items.Clear();
            //checkedListBox1.Items.Clear();
            //checkedComboBoxEdit1.Properties.Items.Clear();
            //cb_ESD等级.Properties.Items.Clear();
            cb_扫描方式.Items.Clear();
            // cb_锁芯.Items.Clear();
            cb_商品分类.Items.Clear();

            // cb_保护特性.Items.Clear();
            // cb_额定电流.Items.Clear();
            // cb_断路器型号.Items.Clear();
            //  cb_漏电.Items.Clear();

            string sql1 = "";
            sql1 = "select 员工号,姓名,部门,岗位 from 人事基础员工表 where 在职状态='在职'";
            DataTable dt_people = CZMaster.MasterSQL.Get_DataTable(sql1, strconn);
            txt_负责人.Properties.DataSource = dt_people;
            txt_负责人.Properties.ValueMember = "姓名";
            txt_负责人.Properties.DisplayMember = "姓名";
            sql1 = "select 员工号,姓名  from 人事基础员工表 where 班组='计划课'  and  在职状态='在职' ";
            DataTable dt_计划 = CZMaster.MasterSQL.Get_DataTable(sql1, strconn);
            serl_计划员.Properties.DataSource = dt_计划;
            serl_计划员.Properties.ValueMember = "员工号";
            serl_计划员.Properties.DisplayMember = "姓名";

            DataTable dt_环保 = new DataTable();
            dt_环保.Columns.Add("环保等级");
            DataRow dr = dt_环保.NewRow();
            dr["环保等级"] = "环保";
            dt_环保.Rows.Add(dr);
            dr = dt_环保.NewRow();
            dr["环保等级"] = "环保2.0";
            dt_环保.Rows.Add(dr);
            dr = dt_环保.NewRow();
            dr["环保等级"] = "不环保";
            dt_环保.Rows.Add(dr);
            cb11_环保.Properties.DataSource = dt_环保;
            cb11_环保.Properties.ValueMember = "环保等级";
            cb11_环保.Properties.DisplayMember = "环保等级";
            cb11_环保.Text = "环保2.0";
            string sql2 = "select 物料类型名称 from 基础数据物料类型表 where 类型级别 = '大类' order by 物料类型名称";
            DataTable dt = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
            da2.Fill(dt);
            //foreach (DataRow r in dt.Rows)
            //{
            //    cb6_大类1.Properties.Items.Add(r["物料类型名称"].ToString());
            //}
            //cb6_大类.Properties.DataSource = dt;
            //cb6_大类.Properties.ValueMember = "物料类型名称";
            //cb6_大类.Properties.DisplayMember = "物料类型名称";
            //sl_大类智能.Properties.DataSource = dt;
            //sl_大类智能.Properties.ValueMember = "物料类型名称";
            //sl_大类智能.Properties.DisplayMember = "物料类型名称";
            string sql = "select * from 基础数据基础属性表 order by 属性类别,属性值";
            dt_属性 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_属性);
            foreach (DataRow r in dt_属性.Rows)
            {
                if (r["属性类别"].ToString().Equals("生产线"))
                {
                    cb3_产品线.Properties.Items.Add(r["属性值"].ToString());
                }

                //if (r["属性类别"].ToString().Equals("大类"))   
                //{
                //    cb6_大类.Properties.Items.Add(r["属性值"].ToString());
                //}

                //if (r["属性类别"].ToString().Equals("电压"))
                //{
                //    cb5_电压.Properties.Items.Add(r["属性值"].ToString());
                //}

                //if (r["属性类别"].ToString().Equals("极数"))
                //{
                //    cb8_极数.Properties.Items.Add(r["属性值"].ToString());
                //}
                if (r["属性类别"].ToString().Equals("产品类别"))
                {
                    tb6_产品类别.Properties.Items.Add(r["属性值"].ToString());
                }
                if (r["属性类别"].ToString().Equals("商品分类"))
                {
                    cb_商品分类.Items.Add(r["属性值"].ToString());
                }
                //if (r["属性类别"].ToString().Equals("硬件版本"))
                //{
                //    comboBox1.Items.Add(r["属性值"].ToString());
                //}
                //if (r["属性类别"].ToString().Equals("把手类型"))
                //{
                //    comboBox5.Items.Add(r["属性值"].ToString());
                //}
                if (r["属性类别"].ToString().Equals("商品类型"))
                {
                    comboBox2.Items.Add(r["属性值"].ToString());
                }
                if (r["属性类别"].ToString().Equals("物料等级"))
                {
                    cb1_物料等级.Properties.Items.Add(r["属性值"].ToString());
                }
                if (r["属性类别"].ToString().Equals("扫描方式"))
                {
                    cb_扫描方式.Items.Add(r["属性值"].ToString());
                }
                //if (r["属性类别"].ToString().Equals("锁芯"))
                //{
                //    cb_锁芯.Items.Add(r["属性值"].ToString());
                //}


                //if (r["属性类别"].ToString().Equals("保护特性"))
                //{
                //    cb_保护特性.Items.Add(r["属性值"].ToString());
                //}
                //if (r["属性类别"].ToString().Equals("额定电流"))
                //{
                //    cb_额定电流.Items.Add(r["属性值"].ToString());
                //}
                //if (r["属性类别"].ToString().Equals("断路器型号"))
                //{
                //    cb_断路器型号.Items.Add(r["属性值"].ToString());
                //}

                //if (r["属性类别"].ToString().Equals("漏电"))
                //{
                //    cb_漏电.Items.Add(r["属性值"].ToString());
                //}

                //if (r["属性类别"].ToString().Equals("壳架等级"))
                //{
                //    cb10_壳架等级.Properties.Items.Add(r["属性值"].ToString());
                //}

                if (r["属性类别"].ToString().Equals("主辅料"))
                {
                    cb12.Properties.Items.Add(r["属性值"].ToString());
                }

                if (r["属性类别"].ToString().Equals("物料来源"))
                {
                    cb2.Properties.Items.Add(r["属性值"].ToString());
                }
                //if (r["属性类别"].ToString().Equals("计量单位"))
                //{
                //    cb9.Properties.Items.Add(r["属性值"].ToString());
                //}
                if (r["属性类别"].ToString().Equals("物料属性"))
                {
                    //cb_物料属性.Properties.Items.Add(r["属性值"].ToString());
                    // checkedListBox1.Items.Add(r["属性值"].ToString());

                    //checkedComboBoxEdit1.Properties.Items.Add(r["属性值"].ToString());
                }
                //if (r["属性类别"].ToString().Equals("ESD等级"))
                //{
                //    cb_ESD等级.Properties.Items.Add(r["属性值"].ToString());
                //}



            }

            sql = "select 属性值 as 计量单位,属性字段1 as 计量单位编码 from 基础数据基础属性表 where 属性类别 = '计量单位'";
            da = new SqlDataAdapter(sql, strconn);
            dt_单位 = new DataTable();
            da.Fill(dt_单位);

            searchLookUpEdit1.Properties.DataSource = dt_单位;
            searchLookUpEdit1.Properties.DisplayMember = "计量单位编码";
            searchLookUpEdit1.Properties.ValueMember = "计量单位编码";

        }
        private void fun_下拉框searchlookup()
        {
            //供应商
            string sql = "select 供应商ID,供应商名称 from 采购供应商表 where 供应商状态 = '在用'";
            dt_供应商 = new DataTable();
            SqlDataAdapter da_供应商 = new SqlDataAdapter(sql, strconn);
            da_供应商.Fill(dt_供应商);
            cb_供应商编号.Properties.DataSource = dt_供应商;
            cb_供应商编号.Properties.DisplayMember = "供应商ID";
            cb_供应商编号.Properties.ValueMember = "供应商ID";
            //车间
            sql = "select 属性字段1 as 部门编号,属性值 as 部门名称 from  基础数据基础属性表  where 属性类别 = '生产车间' order by 部门编号";
            dt_车间 = new DataTable();
            SqlDataAdapter da_车间 = new SqlDataAdapter(sql, strconn);
            da_车间.Fill(dt_车间);
            //DataRow dr = dt_车间.NewRow();
            //dr["部门编号"] = "";
            //dr["部门名称"] = "";
            //dt_车间.Rows.Add(dr);
            cb_车间编号.Properties.DataSource = dt_车间;
            cb_车间编号.Properties.DisplayMember = "部门编号";
            cb_车间编号.Properties.ValueMember = "部门编号";

            //班组
            sql = "select 属性字段1 as b_班组编号,属性值 as b_班组名称 from  基础数据基础属性表  where 属性类别 = '班组' order by POS";
            dt_班组 = new DataTable();
            SqlDataAdapter da_班组 = new SqlDataAdapter(sql, strconn);
            da_班组.Fill(dt_班组);
            //DataRow dr = dt_车间.NewRow();
            //dr["部门编号"] = "";
            //dr["部门名称"] = "";
            //dt_车间.Rows.Add(dr);
            searchLookUpEdit4.Properties.DataSource = dt_班组;
            searchLookUpEdit4.Properties.DisplayMember = "b_班组编号";
            searchLookUpEdit4.Properties.ValueMember = "b_班组编号";

            //仓库
            sql = @"select 属性字段1 as 仓库编号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 = '仓库类别'order by 仓库编号 ";
            DataTable dt_仓库 = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_仓库);
            cb_仓库编号.Properties.DataSource = dt_仓库;
            cb_仓库编号.Properties.DisplayMember = "仓库编号";
            cb_仓库编号.Properties.ValueMember = "仓库编号";
            ////寄售客户
            //sql = @"select 客户编号,客户名称 from 客户基础信息表  where 停用=0";
            //dt_寄售客户 = new DataTable();
            //da = new SqlDataAdapter(sql, strconn);
            //da.Fill(dt_寄售客户);
            //s_寄售客户.Properties.DataSource = dt_寄售客户;
            //s_寄售客户.Properties.DisplayMember = "客户编号";
            //s_寄售客户.Properties.ValueMember = "客户编号";

            //sql = @"select 属性字段1 as  滑盖颜色,属性值 as 颜色说明 from 基础数据基础属性表 where 属性类别 = '滑盖颜色'order by 滑盖颜色 ";
            //DataTable dt_hgcol = new DataTable();
            //da = new SqlDataAdapter(sql, strconn);
            //da.Fill(dt_hgcol);
            //sl_滑盖颜色.Properties.DataSource = dt_hgcol;
            //sl_滑盖颜色.Properties.DisplayMember = "滑盖颜色";
            //sl_滑盖颜色.Properties.ValueMember = "滑盖颜色";
            sql = @"select 属性值 as 壳体颜色, 属性字段2 as 颜色说明  from 基础数据基础属性表 where 属性类别 = '壳体颜色'order by 壳体颜色 ";
            DataTable dt_ktcol = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_ktcol);
            sl_壳体颜色.Properties.DataSource = dt_ktcol;
            sl_壳体颜色.Properties.DisplayMember = "壳体颜色";
            sl_壳体颜色.Properties.ValueMember = "壳体颜色";

            sql = @"select 属性值 as 硬件版本, 属性字段2 as 中文说明  from 基础数据基础属性表 where 属性类别 = '硬件版本'order by 硬件版本 ";
            DataTable dt_yjbb = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_yjbb);
            searchLookUpEdit2.Properties.DataSource = dt_yjbb;
            searchLookUpEdit2.Properties.DisplayMember = "硬件版本";
            searchLookUpEdit2.Properties.ValueMember = "硬件版本";

            sql = @"select 属性值 as 把手类型, 属性字段2 as 中文说明  from 基础数据基础属性表 where 属性类别 = '把手类型'order by 把手类型 ";
            DataTable dt_bslx = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_bslx);
            searchLookUpEdit3.Properties.DataSource = dt_bslx;
            searchLookUpEdit3.Properties.DisplayMember = "把手类型";
            searchLookUpEdit3.Properties.ValueMember = "把手类型";

        }



        /// <summary>
        /// 作用1：载入数据库数据
        /// 作用2：刷新数据库数据
        /// strNo = 2，查询状态，物料编码为只读
        /// </summary>
        public void fun_载入刷新()
        {
            try
            {
                Thread th = new Thread(fun_N_加载数据);
                th.Start();
                checkBox11.Checked = true;

                //fun_载入数据();
                if (strNo == 2)  //查询状态刷新
                {
                    //if (textBox1.Text == "")
                    //{
                    //    fun_清空数据();
                    //    txt_物料编码.ReadOnly = false;
                    //}
                    //else
                    //{
                    //    fun_查询(textBox1.Text);
                    //}
                }
                else if (strNo == 0 || strNo == 1)   //初始、新增状态刷新
                {
                    fun_清空数据();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 根据物料编码 textBox1.Text 查询该物料的基础属性和扩展属性
        /// </summary>
        public void fun_查询(string wl)
        {
            try
            {

                //基础属性
                fun_清空数据();
                string str = string.Format(@"select base.*,a.版本 as sop版本 from 基础数据物料信息表 base 
                            left  join (select 类别名称,max(版本) as 版本 from 作业指导书文件表 group by 类别名称) a on base.物料编码=a.类别名称  where 物料编码 = '{0}'", wl);
                SqlDataAdapter da = new SqlDataAdapter(str, strconn);
                DataTable dtM1 = new DataTable();
                new SqlCommandBuilder(da);
                da.Fill(dtM1);
                a_查询时使用 = dtM1.Rows.Count;
                if (dtM1.Rows.Count > 0)
                {
                    txt_物料编码.Text = dtM1.Rows[0]["物料编码"].ToString();
                    txt_物料编码.ReadOnly = true;
                    // tb2.Text = dtM1.Rows[0]["物料编码"].ToString();
                    tb3_物料名称.Text = dtM1.Rows[0]["物料名称"].ToString();
                    cb4_规格.Text = tb4_规格型号.Text = dtM1.Rows[0]["规格型号"].ToString();
                    txt_存货分类.Text = dtM1.Rows[0]["存货分类"].ToString();
                    txt_分类编码.Text = dtM1.Rows[0]["存货分类编码"].ToString();
                    tb5.Text = dtM1.Rows[0]["自定义项1"].ToString();
                    zdy2.Text = dtM1.Rows[0]["自定义项2"].ToString();

                    textBox1.Text = dtM1.Rows[0]["拼板数量"].ToString();
                    cb_商品分类.Text = dtM1.Rows[0]["物料类型"].ToString();
                    cb3_产品线.Text = dtM1.Rows[0]["产品线"].ToString();
                    //cb6_大类.Text = dtM1.Rows[0]["大类"].ToString();
                    //cb7_小类.Text = dtM1.Rows[0]["小类"].ToString();
                    t_dl.Text = dtM1.Rows[0]["大类"].ToString();
                    t_xl.Text = dtM1.Rows[0]["小类"].ToString();
                    // cb4_规格.Text = dtM1.Rows[0]["规格"].ToString();
                    cb1_物料等级.Text = dtM1.Rows[0]["物料等级"].ToString();
                    //cb10_壳架等级.Text = dtM1.Rows[0]["壳架等级"].ToString();
                    //cb8_极数.Text = dtM1.Rows[0]["极数"].ToString();
                    //cb5_电压.Text = dtM1.Rows[0]["电压"].ToString();
                    //cb_锁芯.Text = dtM1.Rows[0]["锁芯"].ToString();
                    tb6_产品类别.Text = dtM1.Rows[0]["产品类别"].ToString(); //分段能力改为 产品类别 
                                                                     // sl_滑盖颜色.EditValue = dtM1.Rows[0]["滑盖颜色"].ToString();
                    sl_壳体颜色.EditValue = dtM1.Rows[0]["壳体颜色"].ToString();
                    chkBx_蓝牙.Checked = Convert.ToBoolean(dtM1.Rows[0]["有无蓝牙"]);
                    checkBox3.Checked = Convert.ToBoolean(dtM1.Rows[0]["ECN"]);
                    searchLookUpEdit2.EditValue = dtM1.Rows[0]["硬件版本"].ToString();
                    checkBox8.Checked = Convert.ToBoolean(dtM1.Rows[0]["是否联动"]);
                    checkBox7.Checked = Convert.ToBoolean(dtM1.Rows[0]["有无天地钩"]);
                    textBox4.Text = dtM1.Rows[0]["导向片规格"].ToString();
                    searchLookUpEdit3.EditValue = dtM1.Rows[0]["把手类型"].ToString();
                    comboBox2.Text = dtM1.Rows[0]["商品类型"].ToString();
                    //comboBox4.Text = dtM1.Rows[0]["锁体状态"].ToString();
                    //cb_保护特性.Text = dtM1.Rows[0]["保护特性"].ToString();
                    //cb_断路器型号.Text = dtM1.Rows[0]["断路器型号"].ToString();
                    //cb_漏电.Text = dtM1.Rows[0]["漏电"].ToString();
                    cb_扫描方式.Text = dtM1.Rows[0]["扫描方式"].ToString();
                    //cb_额定电流.Text = dtM1.Rows[0]["额定电流"].ToString();


                    tb10.Text = dtM1.Rows[0]["客户"].ToString();
                    searchLookUpEdit1.EditValue = dtM1.Rows[0]["计量单位编码"].ToString();
                    tb9.Text = dtM1.Rows[0]["标准单价"].ToString();
                    tb15.Text = dtM1.Rows[0]["库存上限"].ToString();
                    tb16.Text = dtM1.Rows[0]["库存下限"].ToString();
                    tb8.Text = dtM1.Rows[0]["克重"].ToString();
                    cb11_环保.EditValue = dtM1.Rows[0]["环保"].ToString();
                    cb_供应状态.EditValue = dtM1.Rows[0]["供应状态"].ToString();

                    // cb_ESD等级.Text = dtM1.Rows[0]["ESD等级"].ToString();
                    //tb14.Text = dtM1.Rows[0]["库位编号"].ToString();
                    //tb17.Text = dtM1.Rows[0]["库位描述"].ToString();
                    cb2.Text = dtM1.Rows[0]["物料来源"].ToString();
                    tb11.Text = dtM1.Rows[0]["采购周期"].ToString();
                    txt_默认供应商.Text = dtM1.Rows[0]["默认供应商"].ToString();
                    textBox3.Text = dtM1.Rows[0]["采购供应商备注"].ToString();


                    checkBox10.Checked = Convert.ToBoolean(dtM1.Rows[0]["标签打印"]);
                    tb12.Text = dtM1.Rows[0]["最小包装"].ToString();
                    cb12.Text = dtM1.Rows[0]["主辅料"].ToString();

                    txt_货架编号.Text = dtM1.Rows[0]["货架编号"].ToString();
                    txt_货架描述.Text = dtM1.Rows[0]["货架描述"].ToString();
                    checkBox9.Checked = Convert.ToBoolean(dtM1.Rows[0]["新数据"]);     //
                    teshubeizhu.Text = dtM1.Rows[0]["特殊备注"].ToString();     //
                    wuliaobeizhu.Text = dtM1.Rows[0]["物料备注"].ToString();     //
                    // yuanguigexinghao.Text = dtM1.Rows[0]["原规格型号"].ToString();     //
                    //xilei.Text = dtM1.Rows[0]["细类"].ToString();     //
                    xiaoshoudanjia.Text = dtM1.Rows[0]["n销售单价"].ToString();     //
                    hesuandanjia.Text = dtM1.Rows[0]["n核算单价"].ToString();     //
                    cb_仓库编号.Text = dtM1.Rows[0]["仓库号"].ToString();     //
                    cangkumiaoshu.Text = dtM1.Rows[0]["仓库名称"].ToString();     //
                    // yuanERPguigexinghao.Text = dtM1.Rows[0]["n原ERP规格型号"].ToString();     //
                    // xinghaozixiang.Text = dtM1.Rows[0]["型号子项"].ToString();

                    txt_车间.Text = dtM1.Rows[0]["车间"].ToString();
                    textBox6.Text = dtM1.Rows[0]["b_班组名称"].ToString();
                    txt_工时.Text = dtM1.Rows[0]["工时"].ToString();
                    textBox5.Text = dtM1.Rows[0]["工艺工时"].ToString();
                    txt_负责人.Text = dtM1.Rows[0]["负责人"].ToString();
                    checkBox5.Checked = Convert.ToBoolean(dtM1.Rows[0]["停用"]);
                    checkBox2.Checked = Convert.ToBoolean(dtM1.Rows[0]["BOM确认"]);


                    checkBox6.Checked = Convert.ToBoolean(dtM1.Rows[0]["可售"]);
                    ck_可购.Checked = Convert.ToBoolean(dtM1.Rows[0]["可购"]);
                    // checkBox18.Checked = Convert.ToBoolean(dtM1.Rows[0]["铆压"]);
                    ck_内销.Checked = Convert.ToBoolean(dtM1.Rows[0]["内销"]);
                    ck_外销.Checked = Convert.ToBoolean(dtM1.Rows[0]["外销"]);
                    ck_委外.Checked = Convert.ToBoolean(dtM1.Rows[0]["委外"]);
                    ck_自制.Checked = Convert.ToBoolean(dtM1.Rows[0]["自制"]);
                    ck_资产.Checked = Convert.ToBoolean(dtM1.Rows[0]["资产"]);
                    ck_应税劳务.Checked = Convert.ToBoolean(dtM1.Rows[0]["应税劳务"]);
                    checkBox1.Checked = Convert.ToBoolean(dtM1.Rows[0]["在研"]);
                    ck_虚拟件.Checked = Convert.ToBoolean(dtM1.Rows[0]["虚拟件"]);

                    checkBox17.Checked = Convert.ToBoolean(dtM1.Rows[0]["有无蓝图"]);


                    checkBox4.Checked = Convert.ToBoolean(dtM1.Rows[0]["生效"]);
                    cb_供应商编号.EditValue = dtM1.Rows[0]["供应商编号"].ToString();
                    cb_车间编号.EditValue = dtM1.Rows[0]["车间编号"].ToString();
                    searchLookUpEdit4.EditValue = dtM1.Rows[0]["b_班组编号"].ToString();
                    //s_寄售客户.EditValue = dtM1.Rows[0]["寄售客户ID"].ToString();



                    txt_图纸版本.Text = dtM1.Rows[0]["图纸版本"].ToString();
                    cb_物料属性.EditValue = dtM1.Rows[0]["物料属性"].ToString();
                    try
                    {
                        txt_物料状态.EditValue = dtM1.Rows[0]["物料状态"].ToString();
                        //txt_更改预计完成时间.EditValue = dtM1.Rows[0]["更改预计完成时间"];
                    }
                    catch { }
                    txt_审核.Text = dtM1.Rows[0]["审核"].ToString();
                    strNo = 2;  //2表示修改状态
                }
                else
                {
                    strshow = "没有该数据！";
                    // textBox1.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public Boolean fun_check()
        {
            try
            {
                //  9-11 又先去除限制
                //if(checkBox5.Checked == true)
                //{
                //    string sql=string.Format("select 库存总数 from 仓库物料数量表 where 物料编码='{0}'",txt_物料编码.Text);
                //    DataTable t =CZMaster.MasterSQL.Get_DataTable(sql,strconn);
                //    if (Convert.ToDecimal(t.Rows[0]["库存总数"]) > 0)
                //    {
                //        strshow = "财务要求该物料有库存,不可停用";
                //        return false;
                //    }
                //}
                //if (cb3_产品线.EditValue == null || cb3_产品线.EditValue.ToString() == "")
                //{
                //    strshow = "产品线不可为空";
                //    return false;
                //}
                if (cb_车间编号.EditValue == null || cb_车间编号.EditValue.ToString() == "")
                {
                    cb_车间编号.EditValue = "";
                }
                if (ck_自制.Checked == true)
                {
                    if (searchLookUpEdit4.EditValue == null || searchLookUpEdit4.EditValue.ToString() == "")
                    {
                        throw new Exception("该物料属性为自制，班组必填");
                    }
                }

                if (searchLookUpEdit2.EditValue == null || searchLookUpEdit2.EditValue.ToString() == "")
                {
                    searchLookUpEdit2.EditValue = "";
                }
                if (searchLookUpEdit3.EditValue == null || searchLookUpEdit3.EditValue.ToString() == "")
                {
                    searchLookUpEdit3.EditValue = "";
                }

                if (textBox1.Text == null || textBox1.Text == "")
                {
                    textBox1.Text = Convert.ToDecimal("0").ToString();
                }
                if (cb_仓库编号.EditValue == null || cb_仓库编号.EditValue.ToString() == "")
                {
                    // strshow = "仓库不能为空";
                    // return false;
                    throw new Exception("仓库不能为空");
                }
                if (cb11_环保.EditValue == null || cb11_环保.EditValue.ToString() == "")
                {
                    cb11_环保.EditValue = "";
                }
                if (cb_供应状态.EditValue == null || cb_供应状态.EditValue.ToString() == "")
                {
                    cb_供应状态.EditValue = "";
                }

                if (tb3_物料名称.Text == "")
                {
                    //strshow = "物料名称不能为空！"; tb3_物料名称.Focus();
                    //return false;
                    throw new Exception("物料名称不能为空");
                }
                if (txt_存货分类.Text == "" || txt_分类编码.Text == "")
                {
                    //strshow = "存货分类不能为空！";
                    //return false;
                    throw new Exception("存货分类为空,当前不是新增状态");
                }
                if (searchLookUpEdit1.EditValue == null && searchLookUpEdit1.EditValue.ToString() == "")
                {
                    //strshow = "计量单位不能为空！";
                    //return false;
                    throw new Exception("计量单位不能为空");
                }

                if (cb4_规格.Text == "")
                {
                    //strshow = "型号不能为空！"; tb4_规格型号.Focus();
                    //return false;
                    throw new Exception("型号不能为空");

                }
                //19-5-21 
                if (cb4_规格.Text.ToString() != "" && txt_分类编码.Text.Substring(0, 2) == "10") //19-5-21 库存商品才判断 其他的不判断
                {
                    if (strNo == 2)
                    {

                    }
                    else
                    {
                        string sql = string.Format("select 物料编码,规格型号 from 基础数据物料信息表 where 规格型号='{0}' and 物料编码 <>'{1}' and left(物料编码,2)='10'", cb4_规格.Text, txt_物料编码.Text);
                        DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                        if (dt.Rows.Count > 0)
                        {
                            //strshow = "已有重复规格";
                            //return false;
                            throw new Exception("已有重复规格");
                        }
                    }

                }
                //if (tb6_物料类型.Text == "")
                //{
                //    //strshow = "物料类型不能为空！"; tb6_物料类型.Focus();
                //    //return false;
                //    throw new Exception("物料类型不能为空");

                //}
                //if (tb6_物料类型.EditValue.ToString() != "原材料")
                //{
                //    if (cb_车间编号.EditValue == null || cb_车间编号.EditValue.ToString() == "")
                //    {
                //        //strshow = "半成品、成品车间不能为空";
                //        //return false;
                //        throw new Exception("物料类型不能为空");
                //    }

                //    if (serl_计划员.EditValue == null || serl_计划员.EditValue.ToString() == "")
                //    {
                //        //string ss = string.Format("select * from 计划人员关联物料表 where 物料编码='{0}' ", txt_物料编码.Text);
                //        //using (SqlDataAdapter da = new SqlDataAdapter(ss, strconn))
                //        //{
                //        //    DataTable temp=new DataTable ();
                //        //    da.Fill(temp);
                //        //    if (temp.Rows.Count == 0)
                //        //    {
                //        strshow = "成品,半成品必须关联计划员";
                //        return false;
                //        //}

                //        //}
                //    }
                //}
                //if (cb3_产品线.EditValue.ToString() == "")
                //{
                //strshow = "请选择产品线！"; cb3_产品线.Focus();
                //return false;
                //}

                //if (textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "")
                //{
                //    strshow = "对外产品线或对外大类或对外小类未填写";
                //    return false;

                //}

                //if (cb11_环保.EditValue == null || cb11_环保.EditValue.ToString() == "")
                //{
                //    //strshow = "环保选项为必选项";
                //    //return false;
                //    throw new Exception("环保选项为必选项");
                //}
                //if (cb6_大类.EditValue.ToString() == "")
                //{
                //    //strshow = "请选择大类！"; cb6_大类.Focus();
                //    //return false;
                //    throw new Exception("请选择大类");
                //}
                //if (cb7_小类.EditValue.ToString() == "")
                //{
                //    //strshow = "请选择小类！"; cb7_小类.Focus();
                //    //return false;
                //    throw new Exception("请选择小类");
                //}
                //if (sl_大类智能.EditValue==null )
                //{
                //    sl_大类智能.EditValue = "";
                //}
                //if (cb_小类智能.EditValue == null)
                //{
                //    cb_小类智能.EditValue = "";
                //}
                if (tb8.Text == "")
                {
                    tb8.Text = Convert.ToString(0);
                }
                if (tb11.Text == "")
                {
                    tb11.Text = Convert.ToString(0);
                }
                if (tb12.Text == "")
                {
                    tb12.Text = Convert.ToString(0);
                }
                if (tb15.Text == "")
                {
                    tb15.Text = Convert.ToString(0);
                }
                if (tb16.Text == "")
                {
                    tb16.Text = Convert.ToString(0);
                }
                if (xiaoshoudanjia.Text == "")
                {
                    xiaoshoudanjia.Text = "0";
                }
                if (hesuandanjia.Text == "")
                {
                    hesuandanjia.Text = "0";
                }
                if (tb9.Text == "")
                {
                    tb9.Text = "0";
                }
                if (txt_工时.Text == "")
                {
                    txt_工时.Text = "0";
                }
                if (textBox5.Text == "")
                {
                    textBox5.Text = "0";
                }
                //if (cb_车间编号.EditValue.ToString() == "" || cb_车间编号.EditValue == null)
                //{
                //    cb_车间编号.EditValue = "";
                //}


                if (cb_物料属性.EditValue == null || cb_物料属性.EditValue.ToString() == "")
                {
                    cb_物料属性.EditValue = "";
                }
                if (comboBox2.Text == "锁具")
                {
                    if (checkBox8.Checked)
                    {
                        throw new Exception("是否联动不能勾选");
                    }
                    if (checkBox7.Checked)
                    {
                        throw new Exception("有无天地钩不能勾选");
                    }
                    if (textBox4.Text != null && textBox4.Text != "")
                    {
                        throw new Exception("导向片规格不可填");
                    }

                }
                else if (comboBox2.Text == "锁体")
                {
                    if (chkBx_蓝牙.Checked)
                    {
                        throw new Exception("有无蓝牙不能勾选");
                    }
                    if (sl_壳体颜色.Text != null && sl_壳体颜色.Text != "")
                    {
                        throw new Exception("壳体颜色不可填");
                    }
                    if (searchLookUpEdit2.EditValue != null && searchLookUpEdit2.EditValue.ToString().Trim() != "")
                    {
                        throw new Exception("硬件版本不可填");
                    }
                    if (searchLookUpEdit3.EditValue != null && searchLookUpEdit3.EditValue.ToString() != "")
                    {
                        throw new Exception("把手类型不可填");
                    }

                }

                return true;
            }
            catch (Exception ex)
            {

                strshow = ex.Message;
                throw new Exception(ex.Message);


            }

        }  //检查基础数据

        public void fun_清空数据()
        {

            //cb_漏电.Text = "";
            //  cb_锁芯.Text = "";
            cb_商品分类.Text = "";
            //cb_保护特性.Text = "";
            //cb_断路器型号.Text = "";
            cb_扫描方式.Text = "";
            //  cb_额定电流.Text = "";
            //txt_智能型号.Text ="";

            txt_物料编码.Text = "";
            tb2.Text = "";
            tb3_物料名称.Text = "";
            txt_分类编码.Text = "";
            txt_存货分类.Text = "";
            tb4_规格型号.Text = "";
            tb5.Text = "";
            zdy2.Text = "";

            textBox1.Text = "";
            tb6_产品类别.SelectedIndex = -1;
            txt_默认供应商.Text = "";
            tb8.Text = "";
            tb9.Text = "";
            tb10.Text = "";
            tb11.Text = "";
            tb12.Text = "";
            checkBox10.Checked = false;
            //  tb14.Text = "";
            tb15.Text = "";
            tb16.Text = "";

            searchLookUpEdit2.EditValue = null;
            checkBox8.Checked = false;
            checkBox7.Checked = false;
            textBox4.Text = "";
            searchLookUpEdit3.EditValue = null;
            comboBox2.Text = null;
            comboBox2.Enabled = true;
            comboBox4.Text = "";
            cb_商品分类.Text = null;
            //  tb17.Text = "";
            checkBox4.Checked = false;
            checkBox5.Checked = false;
            checkBox2.Checked = false;

            checkBox6.Checked = false;
            ck_可购.Checked = false;
            //  checkBox18.Checked = false;
            ck_内销.Checked = false;
            ck_外销.Checked = false;
            ck_委外.Checked = false;
            ck_自制.Checked = false;
            ck_资产.Checked = false;
            ck_应税劳务.Checked = false;
            checkBox1.Checked = false;
            ck_虚拟件.Checked = false;

            checkBox17.Checked = false;

            txt_审核.Text = "";
            cb1_物料等级.SelectedIndex = -1;
            cb2.SelectedIndex = -1;
            cb3_产品线.SelectedIndex = -1;
            cb4_规格.Text = "";
            // cb5_电压.SelectedIndex = -1;
            //cb6_大类.EditValue = "";
            //cb7_小类.SelectedIndex = -1;
            t_dl.Text = "";
            t_xl.Text = "";

            // cb8_极数.SelectedIndex = -1;
            //cb9.SelectedIndex = -1;
            // cb10_壳架等级.SelectedIndex = -1;
            // cb11_环保.SelectedIndex = -1;
            //cb_ESD等级.SelectedIndex = -1;
            cb12.SelectedIndex = -1;
            //cb_stock.SelectedIndex = -1;
            txt_货架编号.Text = "";
            txt_货架描述.Text = "";
            txt_工时.Text = "";
            textBox5.Text = "";
            txt_车间.Text = "";
            textBox6.Text = "";
            checkBox9.Checked = true;    //18-1-31 字段重用 新增的 为新数据
            xinghaozixiang.Text = "";
            //textBox2.Text = "";
            teshubeizhu.Text = "";      //
            wuliaobeizhu.Text = "";      //
            yuanguigexinghao.Text = "";      //
            //xilei.Text = "";     //
            xiaoshoudanjia.Text = "";      //
            hesuandanjia.Text = "";      //
            cb_仓库编号.Text = "";  //
            cangkumiaoshu.Text = "";    //
            yuanERPguigexinghao.Text = "";     //
            txt_负责人.Text = "";
            //txt_更改预计完成时间.EditValue = null;
            txt_物料状态.SelectedIndex = -1;
            cb_供应商编号.EditValue = null;
            cb_车间编号.EditValue = null;
            searchLookUpEdit4.EditValue = null;
            sl_壳体颜色.EditValue = null;
            // sl_滑盖颜色.EditValue = null;

            // s_寄售客户.EditValue = null;
            searchLookUpEdit1.EditValue = null;
            serl_计划员.EditValue = null;
            cb11_环保.EditValue = null;
            cb_供应状态.EditValue = null;

            txt_图纸版本.Text = "";
            cb_物料属性.SelectedIndex = -1;
            //checkBox8.Checked = false;
        }  //清空基础属性数据

        public void fun_新增()
        {
            fun_清空数据();
            //  txt_物料编码.ReadOnly = false;
            strNo = 1;                       //1表示新增状态
                                             // textBox1.Text = "";

            button4.Text = "预览";
        }

        /// <summary>
        /// 智能家居产品线 并且为二代锁 才用这个规则
        /// </summary>
        public void fun_规格()
        {
            if (tb6_产品类别.EditValue != null && tb6_产品类别.EditValue.ToString() == "成品")
            {
                cb4_规格.Text = "";//产品类别-壳体颜色-滑盖颜色-有无蓝牙-扫描方式-锁芯
                if (tb6_产品类别.Text.ToString() != "")
                {
                    cb4_规格.Text = tb6_产品类别.Text.ToString() + "-";
                }
                if (sl_壳体颜色.EditValue != null && sl_壳体颜色.EditValue.ToString() != "")
                {
                    cb4_规格.Text = cb4_规格.Text.ToString() + sl_壳体颜色.EditValue.ToString();
                }
                //if (sl_滑盖颜色.EditValue != null && sl_滑盖颜色.EditValue.ToString() != "")
                //{
                //    cb4_规格.Text = cb4_规格.Text.ToString() + sl_滑盖颜色.EditValue.ToString();
                //}
                if (chkBx_蓝牙.Checked == true)
                {
                    cb4_规格.Text = cb4_规格.Text.ToString() + "-N";
                }
                if (cb_扫描方式.Text.ToString() != "")
                {
                    cb4_规格.Text = cb4_规格.Text.ToString() + "-" + cb_扫描方式.Text.ToString();
                }
                //if (cb_锁芯.Text.ToString() != "")
                //{
                //    cb4_规格.Text = cb4_规格.Text.ToString() + "-" + cb_锁芯.Text.ToString();
                //}

                // cb4_规格.Text = cb4_规格.Text.ToString() + tb4_规格型号.Text.ToString();
                //if (cb5_电压.Text.ToString() != "")
                //{//电压
                //    cb4_规格.Text = cb4_规格.Text.ToString() + "-" + cb5_电压.Text.ToString();
                //}
                //if (cb8_极数.Text.ToString() != "")
                //{//极数
                //    cb4_规格.Text = cb4_规格.Text.ToString() + "-" + cb8_极数.Text.ToString();
                //}
                //if (cb11_环保.EditValue != null && cb11_环保.EditValue.ToString() == "环保")
                //{//环保  环保1.0默认
                //    cb4_规格.Text = cb4_规格.Text.ToString() + "[H]";
                //}
                //if (cb11_环保.EditValue != null && cb11_环保.EditValue.ToString() == "环保2.0")
                //{//环保2.0 
                //    cb4_规格.Text = cb4_规格.Text.ToString() + "[H2]";
                //}
                // cb4_规格.Text = cb4_规格.Text.ToString() + "." + xinghaozixiang.Text.ToString();
            }
        }

        public string fun_盒贴规格()
        {
            string str_盒贴规格 = "";
            str_盒贴规格 = t_xl.Text + "-" + tb4_规格型号.Text.ToString();
            //if (cb5_电压.Text.ToString() != "")
            //{//电压
            //    str_盒贴规格 = str_盒贴规格 + "-" + cb5_电压.Text.ToString();
            //}
            //if (cb8_极数.Text.ToString() != "")
            //{//极数
            //    str_盒贴规格 = str_盒贴规格 + "-" + cb8_极数.Text.ToString();
            //}
            if (cb11_环保.EditValue.ToString() == "环保")
            {//环保
                str_盒贴规格 = str_盒贴规格 + "[H]";
            }
            if (cb11_环保.EditValue.ToString() == "环保2.0")
            {//环保
                str_盒贴规格 = str_盒贴规格 + "[H2]";
            }
            return str_盒贴规格;
        }

        public void fun_删除()
        {
            try
            {
                string strdelete = string.Format("delete from 基础数据物料信息表 where 物料编码 = '{0}'", txt_物料编码.Text);
                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlCommand cmd = new SqlCommand(strdelete, conn);
                cmd.ExecuteNonQuery();

                conn.Close();
                //fun_扩展属性删除();
                fun_清空数据();
                strNo = 0;
                txt_物料编码.ReadOnly = false;
                //textBox1.Text = "";
                strshow = "删除成功！";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void fun_扩展属性删除()
        {
            //string strdelete = string.Format("delete from 基础数据物料信息扩展表 where 物料编码 = '{0}'", tb1.Text);
            //SqlConnection conn = new SqlConnection(strconn);
            //conn.Open();
            //SqlCommand cmd = new SqlCommand(strdelete, conn);
            //cmd.ExecuteNonQuery();
            //conn.Close();
            //cmd.Dispose();
            //fun_xtra2();
        }


        /// <summary>
        /// 19-10-15 这里面之前自2015年以来 都没有用事务保存  很不安全
        /// 保存了 dtM
        /// </summary>
        public void fun_基础属性保存()
        {
            try
            {
                DataTable dt_仓库数量 = new DataTable();
                DataTable dt_基础save = new DataTable();

                DateTime time1 = CPublic.Var.getDatetime();
                if (strNo == 1 || strNo == 0)  //0为初始状态，1为新增状态
                {
                    str_新增or修改 = "新增";
                    fun_验证物料编号();
                    dtM.AcceptChanges();
                    DataRow dr = dtM.NewRow();
                    dr["物料编码"] = txt_物料编码.Text;
                    dr["原ERP物料编号"] = txt_物料编码.Text;  // 物料编码 与物料编码一致即可  tb2.Text
                    dr["物料名称"] = tb3_物料名称.Text;
                    dr["存货分类"] = txt_存货分类.Text;
                    dr["存货分类编码"] = txt_分类编码.Text;
                    dr["自定义项1"] = tb5.Text;
                    dr["自定义项2"] = zdy2.Text;

                    

                    dr["拼板数量"] = textBox1.Text;
                    if (t_dl.Text == "") //根据 四位产品线 六位大类 八位小类
                    {
                        string s = $@"select  a.存货分类编码,a.存货分类名称,isnull(b.存货分类编码,'') as 产品线编码,isnull(b.存货分类名称,'') as 产品线,
               isnull(c.存货分类编码,'') as 大类编码,isnull(c.存货分类名称,'') as 大类,isnull(d.存货分类编码,'') as 小类编码,isnull(d.存货分类名称,'') as 小类 from[基础数据存货分类表] a
               left join[基础数据存货分类表] b on left(a.存货分类编码, 4) = b.存货分类编码
               left join[基础数据存货分类表] c on left(a.存货分类编码, 6) = c.存货分类编码
               left join[基础数据存货分类表] d on left(a.存货分类编码, 8) = d.存货分类编码
               where a.存货分类编码 = '{ txt_分类编码.Text}'";
                        DataTable tx = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                        if (tx.Rows.Count > 0)
                        {
                            dr["产品线GUID"] = tx.Rows[0]["产品线编码"].ToString();
                            dr["产品线"] = cb3_产品线.Text;
                            if (tx.Rows[0]["大类编码"].ToString().Length == 6)
                            {
                                dr["大类GUID"] = tx.Rows[0]["大类编码"].ToString();
                                dr["大类"] = t_dl.Text;
                            }
                            if (tx.Rows[0]["小类编码"].ToString().Length == 8)
                            {
                                dr["小类GUID"] = tx.Rows[0]["小类编码"].ToString();
                                dr["小类"] = t_xl.Text;
                            }
                        }
                    }
                    dr["物料类型GUID"] = System.Guid.NewGuid().ToString();
                    dr["物料类型"] = cb_商品分类.Text;
                    //dr["产品线GUID"] = System.Guid.NewGuid().ToString();
                    //dr["产品线"] = cb3_产品线.Text;
                    //dr["大类GUID"] = System.Guid.NewGuid().ToString();
                    //dr["大类"] = t_dl.Text;

                    //dr["小类GUID"] = System.Guid.NewGuid().ToString();
                    //dr["小类"] = t_xl.Text;

                    //dr["对外产品线"] = textBox2.Text;
                    //dr["对外大类"] = textBox3.Text;
                    //dr["对外小类"] = textBox4.Text;
                    //dr["大类智能"] = sl_大类智能.EditValue.ToString();
                    //dr["小类智能"] = cb_小类智能.EditValue.ToString();
                    //    规格由几项拼装而成
                    //if (cb3_产品线.EditValue != null && cb3_产品线.EditValue.ToString() == "智能终端电器")
                    //{
                    //    if (cb4_规格.Text == "")
                    //    {
                    //        fun_智能_规格();
                    //    }
                    //}
                    //else
                    //{
                    //    fun_加1();

                    //19-11-14
                    //if (cb4_规格.Text.Trim() != "") fun_规格();
                    //}


                    dr["规格"] = cb4_规格.Text;
                    dr["n原ERP规格型号"] = dr["规格型号"] = cb4_规格.Text;
                    //dr["原ERP规格型号"] = dr["规格型号"] = tb4_规格型号.Text; //原ERP规格型号 与规格型号 一致
                    dr["物料等级"] = cb1_物料等级.Text;
                    //dr["壳架等级"] = cb10_壳架等级.Text;
                    //dr["极数"] = cb8_极数.Text;
                    //dr["电压"] = cb5_电压.Text;

                    //dr["锁芯"] = cb_锁芯.Text; // 东屋 细分功能结构代码 改为 锁芯
                    dr["产品类别"] = tb6_产品类别.Text; // 东屋 分段能力改为 产品类别 
                    //if (sl_滑盖颜色.EditValue != null && sl_滑盖颜色.EditValue.ToString() != "")
                    //{ dr["滑盖颜色"] = sl_滑盖颜色.EditValue.ToString(); }
                    if (sl_壳体颜色.EditValue != null && sl_壳体颜色.EditValue.ToString() != "")
                    { dr["壳体颜色"] = sl_壳体颜色.EditValue.ToString(); }
                    dr["扫描方式"] = cb_扫描方式.Text; //东屋 功能类别改为 扫描方式
                    dr["有无蓝牙"] = checkBox17.Checked;
                    dr["ECN"] = checkBox3.Checked;
                    dr["硬件版本"] = searchLookUpEdit2.EditValue.ToString();
                    dr["是否联动"] = checkBox8.Checked;
                    dr["有无天地钩"] = checkBox7.Checked;
                    dr["导向片规格"] = textBox4.Text;
                    dr["把手类型"] = searchLookUpEdit3.EditValue.ToString();
                    //dr["锁体状态"] = comboBox4.Text;
                    dr["商品类型"] = comboBox2.Text;
                    dr["客户"] = tb10.Text;
                    dr["计量单位编码"] = searchLookUpEdit1.EditValue;
                    dr["计量单位"] = textBox2.Text;
                    dr["标准单价"] = tb9.Text;
                    dr["库存上限"] = tb15.Text;
                    dr["库存下限"] = tb16.Text;
                    dr["克重"] = tb8.Text;
                    dr["环保"] = cb11_环保.EditValue.ToString();
                    dr["供应状态"] = cb_供应状态.EditValue.ToString();
                    dr["物料来源"] = cb2.Text;
                    dr["采购周期"] = tb11.Text;
                    dr["默认供应商"] = txt_默认供应商.Text;
                    dr["采购供应商备注"] = textBox3.Text;
                    dr["标签打印"] = checkBox10.Checked;
                    dr["最小包装"] = tb12.Text;
                    dr["主辅料"] = cb12.Text;
                    dr["停用"] = checkBox5.Checked;
                    dr["BOM确认"] = checkBox2.Checked;
                    dr["可售"] = checkBox6.Checked;
                    dr["可购"] = ck_可购.Checked;
                    dr["内销"] = ck_内销.Checked;
                    dr["外销"] = ck_外销.Checked;
                    dr["委外"] = ck_委外.Checked;
                    dr["自制"] = ck_自制.Checked;
                    dr["资产"] = ck_资产.Checked;
                    dr["应税劳务"] = ck_应税劳务.Checked;
                    dr["在研"] = checkBox1.Checked;
                    dr["虚拟件"] = ck_虚拟件.Checked;
                    dr["有无蓝图"] = checkBox17.Checked;
                    dr["生效"] = checkBox4.Checked;
                    dr["货架编号"] = txt_货架编号.Text;
                    dr["货架描述"] = txt_货架描述.Text;
                    dr["新数据"] = true;
                    dr["型号子项"] = xinghaozixiang.Text;
                    dr["特殊备注"] = teshubeizhu.Text;
                    dr["物料备注"] = wuliaobeizhu.Text;
                    dr["原规格型号"] = yuanguigexinghao.Text;
                    dr["n销售单价"] = xiaoshoudanjia.Text;
                    dr["n核算单价"] = hesuandanjia.Text;
                    dr["仓库号"] = cb_仓库编号.Text;
                    dr["仓库名称"] = cangkumiaoshu.Text;
                    dr["n原ERP规格型号"] = yuanERPguigexinghao.Text;
                    dr["审核"] = txt_审核.Text;
                    if (str_id != "")
                    {
                        dr["审核人ID"] = str_id;
                        dr["审核人"] = str_name;
                        dr["审核日期"] = time1;
                    }
                    dr["车间"] = txt_车间.Text;
                    dr["b_班组名称"] = textBox6.Text;
                    dr["工时"] = txt_工时.Text;
                    dr["工艺工时"] = textBox5.Text;
                    dr["负责人"] = txt_负责人.Text;
                    dr["修改人"] = CPublic.Var.localUserName;
                    dr["修改人ID"] = CPublic.Var.LocalUserID;
                    dr["添加日期"] = time1;
                    dr["修改日期"] = time1;
                    dr["物料属性"] = cb_物料属性.EditValue;
                    dr["是否初始化"] = "是";
                    try
                    {
                        dr["供应商编号"] = cb_供应商编号.EditValue;
                        dr["车间编号"] = cb_车间编号.EditValue;
                        dr["b_班组编号"] = searchLookUpEdit4.EditValue;
                        //dr["寄售客户ID"] = s_寄售客户.EditValue;
                        dr["图纸版本"] = txt_图纸版本.Text;
                        dr["物料状态"] = txt_物料状态.EditValue;
                    }
                    catch { }
                    dtM.Rows.Add(dr);
                    ///2019-10-15 修改 
                    string a = dr["货架描述"].ToString();
                    string b = dr["物料编码"].ToString();
                    decimal c = 0;
                    string stockid = dr["仓库号"].ToString();
                    string stockname = dr["仓库名称"].ToString();
                    dt_仓库数量 = StockCore.StockCorer.fun_Init初始化仓库物料(dr, a, b, c);

                    dt_基础save = dtM.Copy();
                    //da = new SqlDataAdapter("select * from 基础数据物料信息表 where 1<>1", strconn);
                    //new SqlCommandBuilder(da);
                    //da.Update(dtM);
                    strshow = string.Format("物料编码为{0}的基础数据新增成功！", txt_物料编码.Text);
 

                }
                else if (strNo == 2)  //2为修改状态
                {
                    str_新增or修改 = "修改";
                    dt_保存修改 = new DataTable();
                    // string s= checkedComboBoxEdit1.Text;
                    string sql_保存修改 = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", txt_物料编码.Text);

                    SqlDataAdapter da_保存修改 = new SqlDataAdapter(sql_保存修改, strconn);
                    da_保存修改.Fill(dt_保存修改);

                    dt_保存修改.Rows[0]["物料编码"] = dt_保存修改.Rows[0]["物料编码"] = txt_物料编码.Text;
                    //dt_保存修改.Rows[0]["物料编码"] = tb2.Text;
                    dt_保存修改.Rows[0]["物料名称"] = tb3_物料名称.Text;
                    //dt_保存修改.Rows[0]["规格型号"] = tb4_规格型号.Text;
                    dt_保存修改.Rows[0]["自定义项1"] = tb5.Text;
                    dt_保存修改.Rows[0]["自定义项2"] = zdy2.Text;

                    
                    dt_保存修改.Rows[0]["拼板数量"] = textBox1.Text;
                    dt_保存修改.Rows[0]["物料类型"] = cb_商品分类.Text;
                    dt_保存修改.Rows[0]["产品线"] = cb3_产品线.Text;
                    dt_保存修改.Rows[0]["大类"] = t_dl.Text;
                    dt_保存修改.Rows[0]["小类"] = t_xl.Text;


                    dt_保存修改.Rows[0]["存货分类"] = txt_存货分类.Text;
                    dt_保存修改.Rows[0]["存货分类编码"] = txt_分类编码.Text;
                    
                    dt_保存修改.Rows[0]["规格"] = cb4_规格.Text;
                    dt_保存修改.Rows[0]["规格型号"] = cb4_规格.Text;
                    dt_保存修改.Rows[0]["物料等级"] = cb1_物料等级.Text;
                    
                    dt_保存修改.Rows[0]["产品类别"] = tb6_产品类别.Text;
                    dt_保存修改.Rows[0]["扫描方式"] = cb_扫描方式.Text;
                    
                    dt_保存修改.Rows[0]["壳体颜色"] = sl_壳体颜色.Text.ToString();
                    dt_保存修改.Rows[0]["有无蓝牙"] = chkBx_蓝牙.Checked;
                    dt_保存修改.Rows[0]["ECN"] = checkBox3.Checked;
                    dt_保存修改.Rows[0]["硬件版本"] = searchLookUpEdit2.EditValue;
                    dt_保存修改.Rows[0]["是否联动"] = checkBox8.Checked;
                    dt_保存修改.Rows[0]["有无天地钩"] = checkBox7.Checked;
                    dt_保存修改.Rows[0]["导向片规格"] = textBox4.Text;
                    dt_保存修改.Rows[0]["把手类型"] = searchLookUpEdit3.EditValue;
                    dt_保存修改.Rows[0]["商品类型"] = comboBox2.Text;
                 
                    dt_保存修改.Rows[0]["客户"] = tb10.Text;
                    dt_保存修改.Rows[0]["计量单位"] = textBox2.Text;
                    dt_保存修改.Rows[0]["计量单位编码"] = searchLookUpEdit1.EditValue;
                    dt_保存修改.Rows[0]["标准单价"] = tb9.Text;
                    dt_保存修改.Rows[0]["库存上限"] = tb15.Text;
                    dt_保存修改.Rows[0]["库存下限"] = tb16.Text;
                    dt_保存修改.Rows[0]["克重"] = tb8.Text;
                    dt_保存修改.Rows[0]["环保"] = cb11_环保.EditValue;
                    dt_保存修改.Rows[0]["供应状态"] = cb_供应状态.EditValue;

                    dt_保存修改.Rows[0]["物料来源"] = cb2.Text;
                    dt_保存修改.Rows[0]["采购周期"] = tb11.Text;
                    dt_保存修改.Rows[0]["默认供应商"] = txt_默认供应商.Text;
                    dt_保存修改.Rows[0]["采购供应商备注"] = textBox3.Text;
                    dt_保存修改.Rows[0]["标签打印"] = checkBox10.Checked;
                    dt_保存修改.Rows[0]["最小包装"] = tb12.Text;
                    dt_保存修改.Rows[0]["主辅料"] = cb12.Text;
                    dt_保存修改.Rows[0]["停用"] = checkBox5.Checked;
                    dt_保存修改.Rows[0]["BOM确认"] = checkBox2.Checked;
                    dt_保存修改.Rows[0]["可售"] = checkBox6.Checked;
                    dt_保存修改.Rows[0]["可购"] = ck_可购.Checked;
                    dt_保存修改.Rows[0]["内销"] = ck_内销.Checked;
                    dt_保存修改.Rows[0]["外销"] = ck_外销.Checked;
                    dt_保存修改.Rows[0]["委外"] = ck_委外.Checked;
                    dt_保存修改.Rows[0]["自制"] = ck_自制.Checked;
                    dt_保存修改.Rows[0]["资产"] = ck_资产.Checked;
                    dt_保存修改.Rows[0]["应税劳务"] = ck_应税劳务.Checked;
                    dt_保存修改.Rows[0]["在研"] = checkBox1.Checked;
                    dt_保存修改.Rows[0]["虚拟件"] = ck_虚拟件.Checked;
                    dt_保存修改.Rows[0]["有无蓝图"] = checkBox17.Checked;
                    dt_保存修改.Rows[0]["生效"] = checkBox4.Checked;
                    dt_保存修改.Rows[0]["货架编号"] = txt_货架编号.Text;
                    dt_保存修改.Rows[0]["货架描述"] = txt_货架描述.Text;
                    dt_保存修改.Rows[0]["新数据"] = true;
                    dt_保存修改.Rows[0]["特殊备注"] = teshubeizhu.Text;
                    dt_保存修改.Rows[0]["物料备注"] = wuliaobeizhu.Text;
                    // dt_保存修改.Rows[0]["细类"] = xilei.Text;
                    dt_保存修改.Rows[0]["n销售单价"] = xiaoshoudanjia.Text;
                    dt_保存修改.Rows[0]["n核算单价"] = hesuandanjia.Text;
                    dt_保存修改.Rows[0]["仓库号"] = cb_仓库编号.Text;
                    dt_保存修改.Rows[0]["仓库名称"] = cangkumiaoshu.Text;
                    dt_保存修改.Rows[0]["原规格型号"] = dt_保存修改.Rows[0]["n原ERP规格型号"] = dt_保存修改.Rows[0]["规格"];
                    dt_保存修改.Rows[0]["审核"] = txt_审核.Text;
                    if (str_id != "")
                    {
                        dt_保存修改.Rows[0]["审核人ID"] = str_id;
                        dt_保存修改.Rows[0]["审核人"] = str_name;
                        dt_保存修改.Rows[0]["审核日期"] = time1;
                    }
                    dt_保存修改.Rows[0]["工时"] = txt_工时.Text;
                    dt_保存修改.Rows[0]["工艺工时"] = textBox5.Text;
                    dt_保存修改.Rows[0]["车间"] = txt_车间.Text;
                    dt_保存修改.Rows[0]["b_班组名称"] = textBox6.Text;
                    //dt_保存修改.Rows[0]["盒贴规格型号"] = fun_盒贴规格();
                    dt_保存修改.Rows[0]["负责人"] = txt_负责人.Text;
                    dt_保存修改.Rows[0]["修改人"] = CPublic.Var.localUserName;
                    dt_保存修改.Rows[0]["修改人ID"] = CPublic.Var.LocalUserID;
                    dt_保存修改.Rows[0]["修改日期"] = time1;
                    dt_保存修改.Rows[0]["物料属性"] = cb_物料属性.EditValue;
                    try
                    {
                        dt_保存修改.Rows[0]["供应商编号"] = cb_供应商编号.EditValue;
                        dt_保存修改.Rows[0]["车间编号"] = cb_车间编号.EditValue;
                        dt_保存修改.Rows[0]["b_班组编号"] = searchLookUpEdit4.EditValue;
                        dt_保存修改.Rows[0]["图纸版本"] = txt_图纸版本.Text.ToString();
                        dt_保存修改.Rows[0]["物料状态"] = txt_物料状态.EditValue.ToString();
                    }
                    catch { }
                    ///2019-10-15 修改 
                    string a = dt_保存修改.Rows[0]["货架描述"].ToString();
                    string b = dt_保存修改.Rows[0]["物料编码"].ToString();
                    decimal c = 0;
                    string stockid = dt_保存修改.Rows[0]["仓库号"].ToString();
                    string stockname = dt_保存修改.Rows[0]["仓库名称"].ToString();

                    dt_仓库数量 = StockCore.StockCorer.fun_Init初始化仓库物料(dt_保存修改.Rows[0], a, b, c);
                    dt_基础save = dt_保存修改.Clone();
                    dt_基础save.ImportRow(dt_保存修改.Rows[0]);
 

                    strshow = string.Format("物料编码为{0}的基础数据修改成功！", txt_物料编码.Text);
                    //TreeListNode n = treeList1.Selection[0];
                    //string s = n.GetValue("存货分类编码").ToString();
                    //DataView v = new DataView(dtM);
                    //v.RowFilter = String.Format("存货分类编码 like '{0}%'", s);
                    //gridControl1.DataSource = v;
                }
                //2019-10-15 


                //17-10-9 成品半成品 关联 计划员
                //19-10-15 修改
                DataTable t = new DataTable();
                if (serl_计划员.EditValue != null && serl_计划员.EditValue.ToString() != "")
                {
                    string sql = string.Format("select  * from 计划人员关联物料表 where 物料编码='{0}'", txt_物料编码.Text, serl_计划员.EditValue.ToString());
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                    {

                        da.Fill(t);
                        if (t.Rows.Count == 0)
                        {

                            DataRow r_计划 = t.NewRow();
                            r_计划["工号"] = serl_计划员.EditValue.ToString();
                            r_计划["物料编码"] = txt_物料编码.Text;
                            r_计划["原物料号"] = tb2.Text;
                            t.Rows.Add(r_计划);
                            //new SqlCommandBuilder(da);
                            //da.Update(t);
                        }
                        else
                        {
                            if (t.Rows[0]["工号"].ToString() != serl_计划员.EditValue.ToString())
                            {
                                t.Rows[0].Delete();
                                DataRow r_计划 = t.NewRow();
                                r_计划["工号"] = serl_计划员.EditValue.ToString();
                                r_计划["物料编码"] = txt_物料编码.Text;
                                r_计划["原物料号"] = tb2.Text;
                                t.Rows.Add(r_计划);
                                //new SqlCommandBuilder(da);
                                //da.Update(t);
                            }
                        }
                    }
                }
                DataTable dt_修改日志 = fun_修改日志();
                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("基础信息增加");
                try
                {

                    string sql = "select * from 基础数据物料信息表  where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_基础save);

                    if (dt_仓库数量 != null && dt_仓库数量.Columns.Count > 0)
                    {
                        sql = "select * from 仓库物料数量表 where 1<>1";
                        cmd = new SqlCommand(sql, conn, ts);
                        da = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da);
                        da.Update(dt_仓库数量);
                    }
                    if (t != null && t.Columns.Count > 0)
                    {
                        sql = "select * from 计划人员关联物料表 where 1<>1";
                        cmd = new SqlCommand(sql, conn, ts);
                        da = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da);
                        da.Update(t);
                    }
                    sql = "select * from 基础数据物料信息修改日志表 where 1<>1";
                    cmd = new SqlCommand(sql, conn, ts);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_修改日志);

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
                throw ex;
            }
        }

       
        /// <summary>
        /// 自动实现子项 +1
        /// </summary>
        public void fun_加1()
        {
            //try
            //{// and 特殊备注 = '{5}'    , teshubeizhu.Text.ToString()
            //    string str_环保 = "";
            //    if (cb11_环保.EditValue.ToString() == "不环保")
            //    {
            //        str_环保 = "";
            //    }
            //    else
            //    {
            //        str_环保 = cb11_环保.EditValue.ToString();
            //    }

            //    //  string str_环保 = cb11_环保.EditValue.ToString();
            //    string str2 = "";
            //    string str3 = "";
            //    if (str_环保 == "")
            //    {
            //        str2 = string.Format("小类 = '{0}' and 规格型号 = '{1}' and 电压 = '{2}' and 极数 = '{3}' and (环保 = '' or 环保='不环保')  ", cb7_小类.Text.ToString(), tb4_规格型号.Text.ToString(), cb5_电压.Text.ToString(), cb8_极数.Text.ToString(), str_环保);
            //        str3 = string.Format("小类 = '{0}' and 规格型号 = '{1}' and 电压 = '{2}' and 极数 = '{3}' and (环保 = ''or 环保='不环保')  and 特殊备注 = '{5}' and 物料编码<>'{6}'",
            //        cb7_小类.Text.ToString(), tb4_规格型号.Text.ToString(), cb5_电压.Text.ToString(), cb8_极数.Text.ToString(), str_环保, teshubeizhu.Text.ToString(), tb2.Text.Trim());
            //    }
            //    else
            //    {
            //        str2 = string.Format("小类 = '{0}' and 规格型号 = '{1}' and 电压 = '{2}' and 极数 = '{3}' and 环保 = '{4}' ", cb7_小类.Text.ToString(), tb4_规格型号.Text.ToString(), cb5_电压.Text.ToString(), cb8_极数.Text.ToString(), str_环保);
            //        str3 = string.Format("小类 = '{0}' and 规格型号 = '{1}' and 电压 = '{2}' and 极数 = '{3}' and 环保 = '{4}' and 特殊备注 = '{5}' and 物料编码<>'{6}'",
            //                           cb7_小类.Text.ToString(), tb4_规格型号.Text.ToString(), cb5_电压.Text.ToString(), cb8_极数.Text.ToString(), str_环保, teshubeizhu.Text.ToString(), tb2.Text.Trim());
            //    }
            //    //string str3 = string.Format("小类 = '{0}' and 规格型号 = '{1}' and 电压 = '{2}' and 极数 = '{3}' and 环保 = '{4}' and 特殊备注 = '{5}'",
            //    //    cb7_小类.Text.ToString(), tb4_规格型号.Text.ToString(), cb5_电压.Text.ToString(), cb8_极数.Text.ToString(), str_环保, teshubeizhu.Text.ToString());


            //    string str = string.Format("select * from 基础数据物料信息表 where {0}", str2);
            //    SqlDataAdapter da = new SqlDataAdapter(str, strconn);
            //    DataTable t = new DataTable();
            //    da.Fill(t);
            //    int s_暂时 = 0;
            //    //获取 型号子项 最大值
            //    foreach (DataRow r in t.Rows)
            //    {
            //        if (Convert.ToInt32(r["型号子项"]) > s_暂时)
            //        {
            //            s_暂时 = Convert.ToInt32(r["型号子项"]);
            //        }
            //    }
            //    //判断本次是否需要 +1
            //    string str_1 = string.Format("select * from 基础数据物料信息表 where {0}", str3);
            //    SqlDataAdapter da2 = new SqlDataAdapter(str_1, strconn);
            //    DataTable tt = new DataTable();
            //    da2.Fill(tt);

            //    //这儿有点问题 应该 搜除了当前物料的 增加的时候 都要 +1 修改情况下 =0 不要修改 >=1 ,+1
            //    if (str_新增or修改 == "新增")
            //    {
            //        xinghaozixiang.Text = (s_暂时 + 1).ToString();


            //    }
            //    else
            //    {
            //        if (tt.Rows.Count > 0)
            //        {
            //            xinghaozixiang.Text = (s_暂时 + 1).ToString();
            //            if (Convert.ToInt32(tt.Rows[0]["型号子项"]) <= 0)
            //            {
            //                xinghaozixiang.Text = (s_暂时 + 1).ToString();
            //            }
            //        }
            //    }
            //    //if (tt.Rows.Count <= 0)
            //    //{
            //    //    xinghaozixiang.Text = (s_暂时 + 1).ToString();
            //    //}
            //    //if (Convert.ToInt32(tt.Rows[0]["型号子项"]) <= 0)
            //    //{
            //    //    xinghaozixiang.Text = (s_暂时 + 1).ToString();
            //    //}
            //}
            //catch { }
        }

        
        /*****基础 物料信息 界面****/
        private void fun_界面设置()
        {
            gcc.DataSource = dtM;
            //gvv.PopulateColumns();

            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(xtra, this.Name, cfgfilepath);

            string s = "select  *  from 基础数据存货分类表 order by   存货分类编码 ";
            DataTable tt = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            treeList1.OptionsBehavior.PopulateServiceColumns = true;
            treeList1.KeyFieldName = "GUID";
            treeList1.ParentFieldName = "上级类型GUID";
            treeList1.DataSource = tt;
            treeList1.CollapseAll();
 
        }
        private void fun_N_加载数据()
        {
            try
            {
          
                dtM = new DataTable();
                string sx = string.Format(@" select   base.*,a.版本 as sop版本,isnull(检验标准,0)检验标准,isnull(有无BOM,0)有无BOM,isnull(有无软件版本,0)有无软件版本,isnull(生产过,0)生产过   from 基础数据物料信息表 base
                left  join (select 类别名称,max(版本) as 版本 from 作业指导书文件表 group by 类别名称) a on base.物料编码=a.类别名称
                left join (select  产品编码,CONVERT(bit,1) 检验标准 from [基础数据物料检验要求表] group by 产品编码
                union   select   cpbh 产品编码,CONVERT(bit,1) 检验标准 from [ZZ_JYXM] group by cpbh)x on x.产品编码=物料编码 
                left join (select 产品编码,CONVERT(bit,1) 有无BOM from 基础数据物料BOM表 group by 产品编码 )y on y.产品编码=物料编码 
                left join (select  物料号,CONVERT(bit,1)  有无软件版本 from  程序版本维护表 where 停用=0 group by 物料号)rjbb on rjbb.物料号=base.物料编码 
                left join (select  物料编码,CONVERT(bit,1) 生产过 from 生产记录生产工单表 group by 物料编码) sc on sc.物料编码=base.物料编码 ");
                using (SqlDataAdapter da = new SqlDataAdapter(sx, strconn))
                {
                    da.Fill(dtM);
                }
                BeginInvoke(new MethodInvoker(() =>
                {
                    gcc.DataSource = dtM;
                }));
                method(gcc, gd => fun_界面设置());
            }
            catch (Exception)
            {

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
        /// <summary>
        /// 基础数据界面:载入数据；dv：显示新数据
        /// </summary>
        public void fun_载入数据()
        {
            dtM = new DataTable();
            //string sql = "select * from 基础数据物料信息表";
            string sql = @"  select   base.*,a.版本 as sop版本,isnull(检验标准,0)检验标准,isnull(有无BOM,0)有无BOM,isnull(有无软件版本,0)有无软件版本,isnull(生产过,0)生产过   from 基础数据物料信息表 base
          left  join (select 类别名称,max(版本) as 版本 from 作业指导书文件表 group by 类别名称) a on base.物料编码=a.类别名称
          left join (select  产品编码,CONVERT(bit,1) 检验标准 from [基础数据物料检验要求表] group by 产品编码
                 union   select   cpbh 产品编码,CONVERT(bit,1) 检验标准 from [ZZ_JYXM] group by cpbh)x on x.产品编码=物料编码 
          left join (select 产品编码,CONVERT(bit,1) 有无BOM from 基础数据物料BOM表 group by 产品编码 )y on y.产品编码=物料编码  
          left join (select  物料号,CONVERT(bit,1) 有无软件版本 from  程序版本维护表 where 停用=0 group by 物料号)rjbb on rjbb.物料号=base.物料编码  
            left join (select  物料编码,CONVERT(bit,1) 生产过 from 生产记录生产工单表 group by 物料编码) sc on sc.物料编码=base.物料编码";  //以后只显示审核过的数据 7.28
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);

            //当修改过数据保存后，新数据自动消失，点全部时，查看所有数据 //用于显示旧数据 16.7.28 18-3新数据用于表示 所有新增的物料
            dv = new DataView(dtM);
            dv.RowFilter = "新数据 = 0";
            if (a_存货分类编码 != "")
            {
                dv.RowFilter = string.Format("存货分类编码 ='{0}'", a_存货分类编码);

            }
            gcc.DataSource = dv;
            //2020-1-13
            try
            {
                treeList1_MouseClick(null, null);
            }
            catch (Exception)
            { }

            //    5.24日 隐藏
            //gcM.DataSource = dtM;
        }

        public void fun_生效选择()
        {
            if (checkBox14.Checked == false)
            {
                if (checkBox12.Checked == false)
                {
                    if (checkBox13.Checked == false)
                    {
                        if (checkBox11.Checked == false)
                        {

                        }
                        else
                        {
                            checkBox14.Checked = false;
                            checkBox12.Checked = false;
                            checkBox13.Checked = false;
                        }
                    }
                    else
                    {
                        checkBox14.Checked = false;
                        checkBox12.Checked = false;
                        checkBox11.Checked = false;
                    }
                }
                else
                {
                    checkBox14.Checked = false;
                    checkBox13.Checked = false;
                    checkBox11.Checked = false;
                }
            }
            else
            {
                checkBox12.Checked = false;
                checkBox13.Checked = false;
                checkBox11.Checked = false;
            }

        }
        private void fun_智能_规格()
        {
            string str_智能规格 = "";
            //if (cb7_小类.EditValue == null)
            //{
            //    MessageBox.Show("未选择小类");
            //    return;
            //}
            //str_智能规格 = cb7_小类.EditValue.ToString().Trim();
            str_智能规格 = t_xl.Text;
            //if (cb_扫描方式.Text.ToString().Trim() != "" || cb_锁芯.Text.ToString().Trim() != "" || cb10_壳架等级.Text.ToString().Trim() != "" || cb_产品类别.Text.ToString().Trim() != "")
            //{
            //    str_智能规格 = str_智能规格 + "-" + cb_扫描方式.Text.ToString().Trim() + cb_锁芯.Text.ToString().Trim()
            //        + cb10_壳架等级.EditValue.ToString().Trim() + cb_产品类别.Text.ToString().Trim();
            //}

            //if (cb_保护特性.Text.ToString().Trim() != "" || cb_额定电流.Text.ToString().Trim() != "")
            //{
            //    str_智能规格 = str_智能规格 + "_" + cb_保护特性.Text.ToString().Trim() + cb_额定电流.Text.ToString().Trim();
            //}
            //if (cb8_极数.EditValue == null) cb8_极数.EditValue = "";
            ////if (cb8_极数.EditValue.ToString().Trim() != "" || cb_断路器型号.Text.ToString().Trim() != "")
            ////{
            ////    str_智能规格 = str_智能规格 + "/" + cb8_极数.EditValue.ToString().Trim() + cb_断路器型号.Text.ToString().Trim();
            ////}

            ////if (cb_漏电.Text.ToString().Trim() != "")
            ////{
            ////    str_智能规格 = str_智能规格 + "-" + cb_漏电.Text.ToString();


            ////}
            //if (cb5_电压.Text.ToString().Trim() != "")
            //{

            //    str_智能规格 = str_智能规格 + "_" + cb5_电压.EditValue.ToString().Trim();


            //}
            //if (str_智能规格 != "")
            //{
            //    string str = string.Format("select max(智能型号子项)数,count(*)总 from 基础数据物料信息表 where 智能型号 like '{0}%'", str_智能规格);
            //    SqlDataAdapter da = new SqlDataAdapter(str, strconn);
            //    DataTable t = new DataTable();
            //    da.Fill(t);
            //    if (t.Rows.Count > 0)
            //    {
            //        if (Convert.ToInt32(t.Rows[0]["总"]) != 0)
            //        {
            //            str_智能规格 = str_智能规格 + "." + t.Rows[0]["总"];
            //        }
            //    }

            //    cb4_规格.Text = str_智能规格;

            //    tb4_规格型号.Text = str_智能规格;
            //}


        }
  
        private void fun_验证物料编号()
        {
            string str = tb2.Text;
            string sql = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", str);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                throw new Exception("请检查'物料编码'是否重复！");
            }
            //20-5-31 图纸编号 改为 自定义项1 用
            //tb5.Text != null && tb5.Text != "" &&
            if ( yuanERPguigexinghao.Text != null && yuanERPguigexinghao.Text != "")
            {
                //tb5.Text + 
                //str = "' and n原ERP规格型号 = '" + yuanERPguigexinghao.Text + "'" + "and 物料名称='" + tb3_物料名称.Text + "'";
                //sql = string.Format("select * from 基础数据物料信息表 where 图纸编号 = '{0}", str);
                sql = $"select * from 基础数据物料信息表 where n原ERP规格型号 = '{yuanERPguigexinghao.Text}' and  物料名称='{tb3_物料名称.Text}'";

                dt = new DataTable();
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    throw new Exception("请检查'物料名称'和'规格型号'是否重复！");
                }
            }
        }
        #endregion

        #region 主界面和所有物料界面
        string str_name = "";
        string str_id = "";
        // DateTime time;
        //审核
        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (txt_审核.Text == "待审核" && txt_审核.Text != "")
            {
                txt_审核.Text = "已审核";
                str_name = CPublic.Var.localUserName;
                str_id = CPublic.Var.LocalUserID;
                //  time = System.DateTime.Now;
            }
            else
            {
                txt_审核.Text = "待审核";
            }
        }

        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
             ERPorg.Corg.FlushMemory();
            // fun_载入刷新();
            //if (txt_物料编码.Text != "" && str_新增or修改 != "新增")
            //{
            //    string sql = string.Format("select  * from 基础数据物料信息表 where 物料编码='{0}'", txt_物料编码.Text);
            //    DataTable dt = new DataTable();
            //    dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            //    DataRow[] r_1 = dtM.Select(string.Format("物料编码='{0}'", txt_物料编码.Text));
            //    r_1[0].ItemArray = dt.Rows[0].ItemArray;
            //}
            //checkBox11.Checked = false;
            //cb7_小类.Properties.Items.Clear();
            t_xl.Text = "";
            fun_新增();
            a_存货分类编码 = "";
            fun_载入数据();
            

        }

        //新增
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (treeList1.Nodes.Count > 0)
                {
                    if (treeList1.Selection[0] == null) return;
                }
                else
                {
                    return;
                }


                TreeListNode n = treeList1.Selection[0];
                if (n.HasChildren) throw new Exception("此分类还有子级分类,不可在此分类下新增物料");
                fun_新增();
                //cb7_小类.Properties.Items.Clear();
                txt_分类编码.Text = n.GetValue("存货分类编码").ToString();
                txt_存货分类.Text = n.GetValue("存货分类名称").ToString();
                //根据 四位产品线 六位大类 八位小类

                string dxl = $@"select  a.存货分类编码,a.存货分类名称,isnull(b.存货分类编码,'') as 产品线编码,isnull(b.存货分类名称,'') as 产品线,
               isnull(c.存货分类编码,'') as 大类编码,isnull(c.存货分类名称,'') as 大类,isnull(d.存货分类编码,'') as 小类编码,isnull(d.存货分类名称,'') as 小类 from[基础数据存货分类表] a
               left join[基础数据存货分类表] b on left(a.存货分类编码, 4) = b.存货分类编码
               left join[基础数据存货分类表] c on left(a.存货分类编码, 6) = c.存货分类编码
               left join[基础数据存货分类表] d on left(a.存货分类编码, 8) = d.存货分类编码
               where a.存货分类编码 = '{ txt_分类编码.Text}'";
                DataTable t = CZMaster.MasterSQL.Get_DataTable(dxl, strconn);
                if (t.Rows.Count > 0)
                {
                    cb3_产品线.Text = t.Rows[0]["产品线"].ToString();
                    if (t.Rows[0]["大类编码"].ToString().Length == 6)
                        t_dl.Text = t.Rows[0]["大类"].ToString();
                    if (t.Rows[0]["小类编码"].ToString().Length == 8)
                        t_xl.Text = t.Rows[0]["小类"].ToString();
                }


                xtra.SelectedTabPage = xtraTabPage5;
                str_新增or修改 = "新增";
                dv = new DataView(dtM);
                dv.RowFilter = string.Format("存货分类编码='{0}'", txt_分类编码.Text);
                dv.Sort = "物料编码 desc";
                gcc.DataSource = dv;
                if (dv.Count > 0)
                {
                    DataRow dr = dv[0].Row;
                    tb3_物料名称.Text = dr["物料名称"].ToString();
                    tb4_规格型号.Text = dr["规格型号"].ToString();
                    cb4_规格.Text = dr["规格型号"].ToString();
                    tb5.Text = dr["自定义项1"].ToString();
                    zdy2.Text = dr["自定义项2"].ToString();
                    cb_商品分类.Text = dr["物料类型"].ToString();
                    cb3_产品线.Text = dr["产品线"].ToString();
                    //cb6_大类.EditValue = dr["大类"].ToString();
                    //cb7_小类.Text = dr["小类"].ToString();

                    //t_dl.Text = dr["大类"].ToString();
                    //t_xl.Text = dr["小类"].ToString();


                    //txt_存货分类.Text = dr["存货分类"].ToString();
                    //txt_分类编码.Text = dr["存货分类编码"].ToString();
                    cb1_物料等级.Text = dr["物料等级"].ToString();
                    //cb10_壳架等级.Text = dr["壳架等级"].ToString();
                    //cb_锁芯.Text = dr["锁芯"].ToString();
                    tb6_产品类别.Text = dr["产品类别"].ToString();
                    //  sl_滑盖颜色.EditValue = dr["滑盖颜色"].ToString();
                    sl_壳体颜色.EditValue = dr["壳体颜色"].ToString();
                    chkBx_蓝牙.Checked = Convert.ToBoolean(dr["有无蓝牙"]);
                    checkBox3.Checked = Convert.ToBoolean(dr["ECN"]);
                    searchLookUpEdit2.EditValue = dr["硬件版本"].ToString();
                    checkBox8.Checked = Convert.ToBoolean(dr["是否联动"]);
                    checkBox7.Checked = Convert.ToBoolean(dr["有无天地钩"]);
                    textBox4.Text = dr["导向片规格"].ToString();
                    searchLookUpEdit3.EditValue = dr["把手类型"].ToString();
                    comboBox2.Text = dr["商品类型"].ToString();
                    //comboBox4.Text = dr["锁体状态"].ToString();
                    cb_扫描方式.Text = dr["扫描方式"].ToString();
                    // cb_额定电流.Text = dr["额定电流"].ToString();
                    tb10.Text = dr["客户"].ToString();
                    searchLookUpEdit1.EditValue = dr["计量单位编码"].ToString();
                    //tb9.Text = dr["标准单价"].ToString();
                    tb15.Text = dr["库存上限"].ToString();
                    tb16.Text = dr["库存下限"].ToString();
                    tb8.Text = dr["克重"].ToString();
                    cb_供应状态.Text = dr["供应状态"].ToString();

                    //textBox1.Text = dr["拼板数量"].ToString();
                    cb11_环保.EditValue = dr["环保"].ToString();
                    if (cb11_环保.EditValue == null || cb11_环保.EditValue.ToString() == "")
                    { cb11_环保.EditValue = "环保"; }
                    // cb_ESD等级.Text = dr["ESD等级"].ToString();
                    cb12.Text = dr["主辅料"].ToString();
                    cb2.Text = dr["物料来源"].ToString();
                    //tb11.Text = dr["采购周期"].ToString();
                    //txt_默认供应商.Text = dr["默认供应商"].ToString();
                    //textBox3.Text = dr["采购供应商备注"].ToString();
                    //checkBox10.Checked = Convert.ToBoolean(dr["标签打印"]);
                    //tb12.Text = dr["最小包装"].ToString();
                    checkBox5.Checked = false;
                    checkBox2.Checked = Convert.ToBoolean(dr["BOM确认"]);
                    checkBox6.Checked = Convert.ToBoolean(dr["可售"]);
                    ck_可购.Checked = Convert.ToBoolean(dr["可购"]);
                    //checkBox18.Checked = Convert.ToBoolean(dr["铆压"]);
                    ck_内销.Checked = Convert.ToBoolean(dr["内销"]);
                    ck_外销.Checked = Convert.ToBoolean(dr["外销"]);
                    ck_委外.Checked = Convert.ToBoolean(dr["委外"]);
                    ck_自制.Checked = Convert.ToBoolean(dr["自制"]);
                    ck_资产.Checked = Convert.ToBoolean(dr["资产"]);
                    ck_应税劳务.Checked = Convert.ToBoolean(dr["应税劳务"]);
                    checkBox1.Checked = Convert.ToBoolean(dr["在研"]);
                    ck_虚拟件.Checked = Convert.ToBoolean(dr["虚拟件"]);
                    //checkBox17.Checked = Convert.ToBoolean(dr["有无蓝图"]);
                    checkBox4.Checked = Convert.ToBoolean(dr["生效"]);


                    txt_车间.Text = dr["车间"].ToString();
                    textBox6.Text = dr["b_班组名称"].ToString();
                    //txt_工时.Text = dr["工时"].ToString();
                    // textBox5.Text = dr["工艺工时"].ToString();
                    checkBox9.Checked = true;      //18-1-31 字段重用 新增物料为true

                    teshubeizhu.Text = dr["特殊备注"].ToString();

                    wuliaobeizhu.Text = dr["物料备注"].ToString();     //

                    //xilei.Text = dr["细类"].ToString();     //
                    //xiaoshoudanjia.Text = dr["n销售单价"].ToString();     //
                    //hesuandanjia.Text = dr["n核算单价"].ToString();     //
                    cb_仓库编号.Text = dr["仓库号"].ToString();     //
                    cangkumiaoshu.Text = dr["仓库名称"].ToString();     //

                    txt_负责人.Text = dr["负责人"].ToString();
                    //  txt_更改预计完成时间.EditValue = dr["更改预计完成时间"];
                    txt_物料状态.EditValue = dr["物料状态"].ToString();
                    //cb_供应商编号.EditValue = dr["供应商编号"].ToString();
                    cb_车间编号.EditValue = dr["车间编号"].ToString();
                    searchLookUpEdit4.EditValue = dr["b_班组编号"].ToString();
                    //s_寄售客户.EditValue = dr["寄售客户ID"].ToString();

                    txt_图纸版本.Text = dr["图纸版本"].ToString();
                    cb_物料属性.EditValue = dr["物料属性"].ToString();

                }
                if (txt_分类编码.Text.Trim().Substring(0, 4) == "0501")
                {
                    txt_物料编码.Enabled = true;
                    txt_物料编码.ReadOnly = false;

                    string x = txt_分类编码.Text;
                    string s = string.Format(@"select  max(物料编码)物料编码 from 基础数据物料信息表 where 存货分类编码='{0}'", x);
                    x = x.PadRight(10, '0');
                    DataTable temp = new DataTable();
                    temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                    if (temp.Rows.Count == 0 || temp.Rows[0]["物料编码"].ToString() == "") x = x + "0001";
                    else
                    {
                        s = temp.Rows[0]["物料编码"].ToString();
                        s = (Convert.ToInt32(temp.Rows[0]["物料编码"].ToString().Substring(10, 4)) + 1).ToString().PadLeft(4, '0');
                        x = x + s;
                    }
                    txt_物料编码.Text = x;
                }


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


        }

        //删除
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //if (tb1.Text == "")
            //{
            //    MessageBox.Show("没有数据！请先选择数据！");
            //}
            //else
            //{
            //    if (MessageBox.Show("确定要删除该数据吗？", "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
            //    {
            //        fun_删除();
            //        MessageBox.Show(strshow);
            //    }
            //}
        }

        //保存
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = dtM.NewRow();
            try
            {
                bool bl = fun_check();
                //gv.CloseEditor();
                if (str_新增or修改 == "新增" && txt_物料编码.Text == "")
                {
                    string x = txt_分类编码.Text;
                    string s = string.Format(@"select  max(物料编码)物料编码 from 基础数据物料信息表 where 存货分类编码='{0}'", x);
                    x = x.PadRight(10, '0');
                    DataTable temp = new DataTable();
                    temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                    if (temp.Rows.Count == 0 || temp.Rows[0]["物料编码"].ToString() == "") x = x + "0001";
                    else
                    {
                        s = temp.Rows[0]["物料编码"].ToString();
                        s = (Convert.ToInt32(temp.Rows[0]["物料编码"].ToString().Substring(10, 4)) + 1).ToString().PadLeft(4, '0');
                        x = x + s;
                    }
                    txt_物料编码.Text = x;
                }
                if (txt_物料编码.Text == "")
                {
                    MessageBox.Show("请先输入物料编码！");
                }
                else
                {
                    if (cangkumiaoshu.Text == "成品库")
                    {
                        if (bl == false)
                        {
                            MessageBox.Show(strshow);
                        }
                        else
                        {
                            fun_基础属性保存();
                            MessageBox.Show(strshow);
                        }
                    }
                    else
                    {
                        if (tb8.Text == "")
                        {
                            tb8.Text = Convert.ToString(0);
                        }
                        if (tb11.Text == "")
                        {
                            tb11.Text = Convert.ToString(0);
                        }
                        if (tb12.Text == "")
                        {
                            tb12.Text = Convert.ToString(0);
                        }
                        if (tb15.Text == "")
                        {
                            tb15.Text = Convert.ToString(0);
                        }
                        if (tb16.Text == "")
                        {
                            tb16.Text = Convert.ToString(0);
                        }
                        if (xiaoshoudanjia.Text == "")
                        {
                            xiaoshoudanjia.Text = "0";
                        }
                        if (hesuandanjia.Text == "")
                        {
                            hesuandanjia.Text = "0";
                        }
                        if (tb9.Text == "")
                        {
                            tb9.Text = "0";
                        }
                        if (xinghaozixiang.Text == "")
                        {
                            xinghaozixiang.Text = "0";
                        }
                        //if (textBox2.Text == "")
                        //{
                        //    textBox2.Text = "0";
                        //}
                        if (txt_工时.Text == "")
                        {
                            txt_工时.Text = "0";
                        }
                        if (textBox5.Text == "")
                        {
                            textBox5.Text = "0";
                        }
                        if (cb_车间编号.EditValue == null || cb_车间编号.EditValue.ToString() == "")
                        {
                            cb_车间编号.EditValue = "";
                        }
                        if (searchLookUpEdit4.EditValue == null || searchLookUpEdit4.EditValue.ToString() == "")
                        {
                            searchLookUpEdit4.EditValue = "";
                        }
                        if (cb_物料属性.EditValue == null || cb_物料属性.EditValue.ToString() == "")
                        {
                            cb_物料属性.EditValue = "";
                        }
 
                        fun_基础属性保存();
                        MessageBox.Show(strshow);
                    }
                }
                button4.Text = "预览";

                ///



                if (txt_物料编码.Text != "" && str_新增or修改 == "修改")
                {
                    refresh_single(txt_物料编码.Text);
                }
                else if (str_新增or修改 == "新增")
                {
                    strNo = 2;  //新增后可以立即修改
                    txt_物料编码.ReadOnly = true;
                    foreach (frm基础数据物料BOM fm in aaaa.FM2)
                    {
                        string xx = string.Format(@" select (a.物料编码) as 子项编码,(a.物料名称) as 子项名称,a.规格型号,b.仓库号,b.仓库名称,b.货架描述,b.库存总数
                            ,大类,小类,a.物料属性,a.图纸编号,a.计量单位编码,a.计量单位,虚拟件,ECN,拼板数量
                          ,a.仓库号 as 默认仓库号,a.仓库名称 as  默认仓库   from 基础数据物料信息表 a   
                            left join 仓库物料数量表 b on a.物料编码=b.物料编码 where a.物料编码='{0}'", dr["物料编码"].ToString());
                        DataRow r_temp = CZMaster.MasterSQL.Get_DataRow(xx, strconn);
                        fm.dt_物料名称.ImportRow(r_temp);
                     
                    }
                }

                txt_物料编码.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void refresh_single(string s_物料)
        {
            string sql = string.Format(@" select   base.*,a.版本 as sop版本,isnull(检验标准,0)检验标准,isnull(有无BOM,0)有无BOM,isnull(有无软件版本,0)有无软件版本
              ,isnull(生产过,0)生产过  from 基础数据物料信息表 base
                left  join(select 类别名称, max(版本) as 版本 from 作业指导书文件表 group by 类别名称) a on base.物料编码 = a.类别名称
                left join(select  产品编码, CONVERT(bit, 1) 检验标准 from[基础数据物料检验要求表] group by 产品编码
                        union   select   cpbh 产品编码, CONVERT(bit, 1) 检验标准 from[ZZ_JYXM] group by cpbh)x on x.产品编码 = 物料编码
                 left join(select 产品编码, CONVERT(bit, 1) 有无BOM from 基础数据物料BOM表 group by 产品编码)y on y.产品编码 = 物料编码 
              left join (select  物料号,CONVERT(bit,1) 有无软件版本 from  程序版本维护表 where 停用=0 group by 物料号)rjbb on rjbb.物料号=base.物料编码
                left join (select  物料编码,CONVERT(bit,1) 生产过 from 生产记录生产工单表 group by 物料编码) sc on sc.物料编码=base.物料编码
               where base.物料编码='{0}'", s_物料);
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            DataRow[] r_1 = dtM.Select(string.Format("物料编码='{0}'", s_物料));
            r_1[0].ItemArray = dt.Rows[0].ItemArray;

            if (Convert.ToBoolean(r_1[0]["停用"]))
            {
                barLargeButtonItem6.Caption = "取消停用";
            }
            else
            {
                barLargeButtonItem6.Caption = "停用";
            }
        }
        private DataTable fun_修改日志()
        {
            DateTime time = CPublic.Var.getDatetime();

            string sql = "select * from 基础数据物料信息修改日志表 where 1<>1";
            DataTable dtttt = new DataTable();
            SqlDataAdapter daaaa = new SqlDataAdapter(sql, strconn);
            daaaa.Fill(dtttt);
            new SqlCommandBuilder(daaaa);
            DataRow drrrr = dtttt.NewRow();
            dtttt.Rows.Add(drrrr);
            if (str_新增or修改 == "修改")
            {
                DataRow[] ds = dtM.Select(string.Format("物料编码 = '{0}'", txt_物料编码.Text));
                int i = ds.Length;
                //dr.ItemArray = ds[0].ItemArray;
                string str_修改内容 = "修改了：";
                //DataTable dt_保存修改
                foreach (DataColumn dc in dt_保存修改.Columns)
                {
                    string str1 = dt_保存修改.Rows[0][dc.Caption].ToString();
                    string str2 = ds[0][dc.Caption].ToString();
                    if (str1 != str2)
                    {
                        str_修改内容 = str_修改内容 + dc.Caption + "的值，" + "原：" + str2 + "，现：" + str1 + "；";
                    }
                }
                drrrr["GUID"] = System.Guid.NewGuid();
                drrrr["姓名"] = CPublic.Var.localUserName;
                drrrr["员工号"] = CPublic.Var.LocalUserID;
                drrrr["日期"] = time;
                drrrr["内容"] = str_修改内容;
                drrrr["物料编码"] = txt_物料编码.Text;
                //MessageBox.Show(str_修改内容);
            }
            else
            {
                drrrr["GUID"] = System.Guid.NewGuid();
                drrrr["姓名"] = CPublic.Var.localUserName;
                drrrr["员工号"] = CPublic.Var.LocalUserID;
                drrrr["日期"] = time;
                drrrr["内容"] = "新增物料：" + txt_物料编码.Text;
                drrrr["物料编码"] = txt_物料编码.Text;
            }
            return dtttt;
            //daaaa.Update(dtttt);
        }


        //关闭
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
         
            CPublic.UIcontrol.ClosePage();
        }

        //查询
        public void button2_Click(object sender, EventArgs e)
        {
            //if (textBox1.Text == "") { }
            //else
            //{
            //    fun_查询(textBox1.Text);
            //    xtra.SelectedTabPage = xtraTabPage5;
            //    if (a_查询时使用 > 0) { }
            //    else
            //    {
            //        MessageBox.Show(strshow);
            //    }
            //}
            //str_物料编码 = textBox1.Text;
            //str_物料名称 = tb3_物料名称.Text;
            //str_规格 = cb4_规格.Text;
            //// str_原规格型号 = yuanERPguigexinghao.Text;
            //aaaa.fun_(str_物料编码, str_物料名称, str_规格);

        }


        //生效
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ////生效
            //if (a2 == 1)
            //{
            //    checkBox4.Checked = true;
            //    a2 = 2;
            //}
            //else if (a2 == 2)
            //{
            //    checkBox4.Checked = false;
            //    a2 = 1;
            //}
        }

        //停用 或者取消停用
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                //停用            
                string sql_停用 = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", txt_物料编码.Text);
                DataTable t = CZMaster.MasterSQL.Get_DataTable(sql_停用, strconn);
                //if (Convert.ToBoolean(t.Rows[0]["停用"]) == true) 
                //{
                //} 
                DateTime t_t = CPublic.Var.getDatetime();
                if (t.Rows.Count == 0) throw new Exception("未选择物料或所选物料有问题");
                DataRow[] r_1 = dtM.Select(string.Format("物料编码='{0}'", txt_物料编码.Text));

                string ys = "";
                if (!Convert.ToBoolean(t.Rows[0]["停用"]))// 非停用状态 要停用
                {
                    ys = "停用原因填写";

                }
                else //停用状态要取消停用
                {
                    ys = "取消停用原因";
                    //throw new Exception("已是停用状态");
                }
                //20-5-27 停用增加 停用原因  --停用原因 加在 [基础数据物料信息修改日志表] 的内容里面
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"MoldMangement.dll")));
                Type outerForm = outerAsm.GetType("MoldMangement.fm_手动完成备注", false);
                Form ui = Activator.CreateInstance(outerForm) as Form;
                //取得控件  --上传按钮不用,隐藏 ，窗体text 需要更改 
                FieldInfo[] fi = outerForm.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                foreach (FieldInfo info in fi)
                {
                    if (info.FieldType == typeof(DevExpress.XtraBars.BarManager))
                    {
                        DevExpress.XtraBars.BarManager barManege = (info.GetValue(ui)) as DevExpress.XtraBars.BarManager;
                        foreach (DevExpress.XtraBars.BarItem bi in barManege.Items)
                        {
                            if (bi.Name == "barLargeButtonItem3")
                            {
                                bi.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                                break;
                            }
                        }
                        break;

                    }
                }
                ui.Text = ys;
                ui.ShowDialog();
                bool bl_继续 = true;
                //是否继续操作
                int int_状态 = Convert.ToInt32(outerForm.GetField("s_状态").GetValue(ui));
                string reason = outerForm.GetField("s_手动完成原因").GetValue(ui).ToString();
                //DataTable dt_ydd_mx = outerForm.GetField("dt_ydd_mx").GetValue(ui) as DataTable;
                if (int_状态 == 1)
                {



                    string sql_库存 = string.Format("select 物料编码,sum(库存总数)库存总数 from 仓库物料数量表 where 物料编码 = '{0}' and 仓库号 in (select 属性字段1 as 仓库号 from 基础数据基础属性表 where 属性类别 = '仓库类别' and 布尔字段1 = 1) group by 物料编码", txt_物料编码.Text);
                    DataTable dt_库存 = CZMaster.MasterSQL.Get_DataTable(sql_库存, strconn);
                    if (dt_库存.Rows.Count > 0)
                    {
                        if (ys == "停用原因填写")
                        {
                            if (Convert.ToDecimal(dt_库存.Rows[0]["库存总数"]) > 0)
                            {
                                if (MessageBox.Show("该物料有库存，是否确认停用该物料？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    checkBox5.Checked = true;
                                    t.Rows[0]["停用"] = true;
                                    t.Rows[0]["停用时间"] = t_t;
                                    r_1[0]["停用"] = 1;
                                    r_1[0]["停用时间"] = t_t;
                                }
                                else
                                {
                                    bl_继续 = false;
                                }
                            }
                            else
                            {
                                checkBox5.Checked = true;
                                t.Rows[0]["停用"] = true;
                                t.Rows[0]["停用时间"] = t_t;
                                r_1[0]["停用"] = 1;
                                r_1[0]["停用时间"] = t_t;
                            }
                        }
                        else
                        {
                            checkBox5.Checked = false;
                            t.Rows[0]["停用"] = false;
                            t.Rows[0]["停用时间"] = DBNull.Value ;
                            r_1[0]["停用"] = false;
                            r_1[0]["停用时间"] = DBNull.Value;
                        }
                    }
                    else
                    {
                        if (ys == "停用原因填写")
                        {
                            checkBox5.Checked = true;
                            t.Rows[0]["停用"] = true;
                            t.Rows[0]["停用时间"] = t_t;
                            r_1[0]["停用"] = 1;
                            r_1[0]["停用时间"] = t_t;
                        }
                        else
                        {
                            checkBox5.Checked = false;
                            t.Rows[0]["停用"] = false;
                            t.Rows[0]["停用时间"] = DBNull.Value;
                            r_1[0]["停用"] = false;
                            r_1[0]["停用时间"] = DBNull.Value;

                        }
                    }
                    //a1 = 2;
                    #region 20-5-27 已停用 不需要再停用
                    //else if (Convert.ToBoolean(t.Rows[0]["停用"]) == true)
                    //{
                    //    checkBox5.Checked = false;
                    //    t.Rows[0]["停用"] = false;
                    //    t.Rows[0]["停用时间"] = t_t;
                    //    r_1[0]["停用"] = 0;
                    //    //a1 = 1;
                    //    r_1[0]["停用时间"] = t_t;
                    //}
                    #endregion
                    if (bl_继续)
                    {
                        string x = "select  * from [基础数据物料信息修改日志表] where 1=2";
                        DataTable t_修改日志 = CZMaster.MasterSQL.Get_DataTable(x, strconn);
                        DataRow r_m = t_修改日志.NewRow();
                        r_m["GUID"] = System.Guid.NewGuid();
                        r_m["姓名"] = CPublic.Var.localUserName;
                        r_m["员工号"] = CPublic.Var.LocalUserID;
                        r_m["日期"] = t_t;
                        r_m["内容"] = ys+":" + reason;
                        r_m["物料编码"] = txt_物料编码.Text;
                        t_修改日志.Rows.Add(r_m);
                        SqlConnection conn = new SqlConnection(strconn);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("物料停用");
                        try
                        {
                            SqlCommand cmd = new SqlCommand(x, conn, ts);
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            new SqlCommandBuilder(da);
                            da.Update(t_修改日志);

                            x = "select * from 基础数据物料信息表 where 1<> 1";
                            cmd = new SqlCommand(x, conn, ts);
                            da = new SqlDataAdapter(cmd);
                            new SqlCommandBuilder(da);
                            da.Update(t);

                            ts.Commit();
                            r_1[0].AcceptChanges();
                            dtM.AcceptChanges();
                            TreeListNode n = treeList1.Selection[0];
                            string s = n.GetValue("存货分类编码").ToString();
                            DataView v = new DataView(dtM);
                            v.RowFilter = String.Format("存货分类编码 like '{0}%'", s);
                            gridControl1.DataSource = v;
                            MessageBox.Show("修改成功");
                        }
                        catch (Exception ex)
                        {
                            ts.Rollback();
                            MessageBox.Show(ex.Message);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region 4选1
        //private void checkBox1_CheckedChanged(object sender, EventArgs e)
        //{
        //    //未生效
        //    checkBox2.Checked = false;
        //    checkBox3.Checked = false;
        //    checkBox7.Checked = false;
        //    fun_xtra生效选择();
        //    DataTable dt = dtM.Clone();
        //    DataRow[] rs = dtM.Select("生效 = false and 停用 = false");
        //    for (int i = 0; i < rs.Length; i++)
        //    {
        //        dt.Rows.Add(rs[i].ItemArray);
        //    }
        //    if (checkBox1.Checked == true)
        //    {
        //        gcM.DataSource = dt;
        //    }
        //    else
        //    {
        //        gcM.DataSource = dtM;
        //    }
        //}

        //private void checkBox3_CheckedChanged(object sender, EventArgs e)
        //{
        //    checkBox1.Checked = false;
        //    checkBox2.Checked = false;
        //    //已生效
        //    checkBox7.Checked = false;
        //    fun_xtra生效选择();
        //    DataTable dt = dtM.Clone();
        //    DataRow[] rs = dtM.Select("生效 = true and 停用 = false");
        //    for (int i = 0; i < rs.Length; i++)
        //    {
        //        dt.Rows.Add(rs[i].ItemArray);
        //    }
        //    if (checkBox3.Checked == true)
        //    {
        //        gcM.DataSource = dt;
        //    }
        //    else
        //    {
        //        gcM.DataSource = dtM;
        //    }
        //}

        //private void checkBox2_CheckedChanged(object sender, EventArgs e)
        //{
        //    checkBox1.Checked = false;
        //    //停用
        //    checkBox3.Checked = false;
        //    checkBox7.Checked = false;
        //    fun_xtra生效选择();
        //    DataTable dt = dtM.Clone();
        //    DataRow[] rs = dtM.Select("停用 = true");
        //    for (int i = 0; i < rs.Length; i++)
        //    {
        //        dt.Rows.Add(rs[i].ItemArray);
        //    }
        //    if (checkBox2.Checked == true)
        //    {
        //        gcM.DataSource = dt;
        //    }
        //    else
        //    {
        //        gcM.DataSource = dtM;
        //    }
        //}

        //private void checkBox7_CheckedChanged(object sender, EventArgs e)
        //{
        //    checkBox1.Checked = false;
        //    checkBox2.Checked = false;
        //    checkBox3.Checked = false;
        //    //全部
        //    fun_xtra生效选择();
        //    gcM.DataSource = dtM;
        //}
        #endregion

        //文本框回车
        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //   // if (textBox1.Text == "") { }
            //    else
            //    {
            //        fun_查询(textBox1.Text); xtra.SelectedTabPage = xtraTabPage5;
            //        if (a_查询时使用 > 0) { }
            //        else
            //        {
            //            MessageBox.Show(strshow);
            //        }
            //        str_物料编码 = textBox1.Text;

            //        aaaa.fun_(str_物料编码, str_物料名称, str_规格);
            //    }
            //}
        }
        #endregion

        #region 基础 物料信息 界面
        private void panel6_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Black, 1);
            pen.DashStyle = DashStyle.Dash;
            e.Graphics.DrawRectangle(pen, panel9.DisplayRectangle);
        }

        //基础数据界面    显示选择项
        private void button5_Click(object sender, EventArgs e)
        {
        }
        //未生效
        private void checkBox14_CheckedChanged(object sender, EventArgs e)
        {
            checkBox12.Checked = false;
            checkBox13.Checked = false;
            checkBox11.Checked = false;
            fun_生效选择();
            DataTable dt = dtM.Clone();
            DataRow[] rs = dtM.Select("生效 = false and 停用 = false");
            for (int i = 0; i < rs.Length; i++)
            {
                dt.Rows.Add(rs[i].ItemArray);
            }
            if (checkBox14.Checked == true)
            {
                gcc.DataSource = dt;
            }
            else
            {
                gcc.DataSource = dtM;
            }
        }

        //已生效
        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            checkBox14.Checked = false;
            checkBox12.Checked = false;
            checkBox11.Checked = false;
            fun_生效选择();
            DataTable dt = dtM.Clone();
            DataRow[] rs = dtM.Select("生效 = true and 停用 = false");
            for (int i = 0; i < rs.Length; i++)
            {
                dt.Rows.Add(rs[i].ItemArray);
            }
            if (checkBox13.Checked == true)
            {
                gcc.DataSource = dt;
            }
            else
            {
                gcc.DataSource = dtM;
            }
        }

        //停用
        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            checkBox14.Checked = false;
            checkBox13.Checked = false;
            checkBox11.Checked = false;
            fun_生效选择();
            DataTable dt = dtM.Clone();
            DataRow[] rs = dtM.Select("停用 = true");
            for (int i = 0; i < rs.Length; i++)
            {
                dt.Rows.Add(rs[i].ItemArray);
            }
            if (checkBox12.Checked == true)
            {
                gcc.DataSource = dt;
            }
            else
            {
                gcc.DataSource = dtM;
            }
        }

        //全部
        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            checkBox14.Checked = false;
            checkBox12.Checked = false;
            checkBox13.Checked = false;
            checkBox16.Checked = false;
            fun_生效选择();
            if (checkBox11.Checked == true)
            {
                gcc.DataSource = dtM;
            }
            else
            {
                dv = new DataView(dtM);
                dv.RowFilter = "新数据 =1";
                gcc.DataSource = dv;   //用于显示旧数据
                //gcc.DataSource = dt;
            }
        }

        //快速选择
        private void checkBox15_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox15.Checked == true)
            {
                button7.Enabled = false;
            }
            if (checkBox15.Checked == false)
            {
                button7.Enabled = true;
            }
        }

        private void gvv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gvv.GetDataRow(gvv.FocusedRowHandle);
                if (dr == null) return;
                refresh_single(dr["物料编码"].ToString());
                if (checkBox15.Checked == true)
                {
                    if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
                    {
                        DataRow rrr = gvv.GetDataRow(e.RowHandle);

                        txt_物料编码.Text = rrr["物料编码"].ToString();
                        txt_物料编码.ReadOnly = true;
                        tb2.Text = rrr["物料编码"].ToString();
                        tb3_物料名称.Text = rrr["物料名称"].ToString();
                        cb4_规格.Text = tb4_规格型号.Text = rrr["规格型号"].ToString();
                        tb5.Text = rrr["自定义项1"].ToString();
                        zdy2.Text = rrr["自定义项2"].ToString();
                        textBox1.Text = rrr["拼板数量"].ToString();

                        cb3_产品线.Text = rrr["产品线"].ToString();
                        //cb6_大类.EditValue = rrr["大类"].ToString();
                        //cb7_小类.Text = rrr["小类"].ToString();

                        t_dl.Text = rrr["大类"].ToString();
                        t_xl.Text = rrr["小类"].ToString();

                        txt_存货分类.Text = rrr["存货分类"].ToString();
                        txt_分类编码.Text = rrr["存货分类编码"].ToString();
                        cb_供应状态.Text = rrr["供应状态"].ToString();


                        // cb4_规格.Text = rrr["规格"].ToString();
                        cb1_物料等级.Text = rrr["物料等级"].ToString();
                        //cb10_壳架等级.Text = rrr["壳架等级"].ToString();
                        //cb8_极数.Text = rrr["极数"].ToString();
                        //cb5_电压.Text = rrr["电压"].ToString();

                        //     cb_锁芯.Text = rrr["锁芯"].ToString();
                        tb6_产品类别.Text = rrr["产品类别"].ToString();
                        sl_壳体颜色.EditValue = rrr["壳体颜色"].ToString();
                        // sl_滑盖颜色.EditValue = rrr["滑盖颜色"].ToString();
                        chkBx_蓝牙.Checked = Convert.ToBoolean(rrr["有无蓝牙"]);
                        checkBox3.Checked = Convert.ToBoolean(rrr["ECN"]);

                        searchLookUpEdit2.EditValue = rrr["硬件版本"].ToString();
                        checkBox8.Checked = Convert.ToBoolean(rrr["是否联动"]);
                        checkBox7.Checked = Convert.ToBoolean(rrr["有无天地钩"]);
                        textBox4.Text = rrr["导向片规格"].ToString();
                        searchLookUpEdit3.EditValue = rrr["把手类型"].ToString();
                        if (rrr["商品类型"].ToString() == "")
                        {
                            comboBox2.Text = null;
                        }
                        else
                        {
                            comboBox2.Text = rrr["商品类型"].ToString();
                        }
                        if (rrr["物料类型"].ToString() == "")
                        {
                            cb_商品分类.Text = null;
                        }
                        else
                        {
                            cb_商品分类.Text = rrr["物料类型"].ToString();
                        }

                        //comboBox4.Text = rrr["锁体状态"].ToString();

                        //cb_保护特性.Text = rrr["保护特性"].ToString();
                        //cb_断路器型号.Text = rrr["断路器型号"].ToString();
                        //cb_漏电.Text = rrr["漏电"].ToString();
                        cb_扫描方式.Text = rrr["扫描方式"].ToString();
                        //cb_额定电流.Text = rrr["额定电流"].ToString();



                        tb10.Text = rrr["客户"].ToString();
                        searchLookUpEdit1.EditValue = rrr["计量单位编码"].ToString();
                        //  textBox2.Text = rrr["计量单位"].ToString();

                        tb9.Text = rrr["标准单价"].ToString();
                        tb15.Text = rrr["库存上限"].ToString();
                        tb16.Text = rrr["库存下限"].ToString();
                        tb8.Text = rrr["克重"].ToString();
                        cb11_环保.EditValue = rrr["环保"].ToString();
                        cb_供应状态.EditValue = rrr["供应状态"].ToString();

                        // cb_ESD等级.Text = rrr["ESD等级"].ToString();
                        //tb14.Text = rrr["库位编号"].ToString();
                        //tb17.Text = rrr["库位描述"].ToString();
                        cb2.Text = rrr["物料来源"].ToString();
                        tb11.Text = rrr["采购周期"].ToString();
                        txt_默认供应商.Text = rrr["默认供应商"].ToString();
                        textBox3.Text = rrr["采购供应商备注"].ToString();
                        checkBox10.Checked = Convert.ToBoolean(rrr["标签打印"]);
                        tb12.Text = rrr["最小包装"].ToString();
                        cb12.Text = rrr["主辅料"].ToString();
                        txt_货架编号.Text = rrr["货架编号"].ToString();
                        txt_货架描述.Text = rrr["货架描述"].ToString();
                        txt_车间.Text = rrr["车间"].ToString();
                        textBox6.Text = rrr["b_班组名称"].ToString();
                        txt_工时.Text = rrr["工时"].ToString();
                        textBox5.Text = rrr["工艺工时"].ToString();
                        checkBox9.Checked = Convert.ToBoolean(rrr["新数据"]);      //
                        teshubeizhu.Text = rrr["特殊备注"].ToString();
                        // xinghaozixiang.Text = rrr["型号子项"].ToString();//

                        wuliaobeizhu.Text = rrr["物料备注"].ToString();     //
                                                                        //yuanguigexinghao.Text = rrr["原规格型号"].ToString();     //
                                                                        //xilei.Text = rrr["细类"].ToString();     //
                        xiaoshoudanjia.Text = rrr["n销售单价"].ToString();     //
                        hesuandanjia.Text = rrr["n核算单价"].ToString();     //
                        cb_仓库编号.Text = rrr["仓库号"].ToString();     //
                        cangkumiaoshu.Text = rrr["仓库名称"].ToString();     //
                                                                         //yuanERPguigexinghao.Text = rrr["n原ERP规格型号"].ToString();
                        checkBox5.Checked = Convert.ToBoolean(rrr["停用"]);
                        checkBox2.Checked = Convert.ToBoolean(rrr["BOM确认"]);

                        checkBox6.Checked = Convert.ToBoolean(rrr["可售"]);
                        ck_可购.Checked = Convert.ToBoolean(rrr["可购"]);
                        //checkBox18.Checked = Convert.ToBoolean(rrr["铆压"]);
                        ck_内销.Checked = Convert.ToBoolean(rrr["内销"]);
                        ck_外销.Checked = Convert.ToBoolean(rrr["外销"]);
                        ck_委外.Checked = Convert.ToBoolean(rrr["委外"]);
                        ck_自制.Checked = Convert.ToBoolean(rrr["自制"]);
                        checkBox1.Checked = Convert.ToBoolean(rrr["在研"]);
                        ck_资产.Checked = Convert.ToBoolean(rrr["资产"]);
                        ck_虚拟件.Checked = Convert.ToBoolean(rrr["虚拟件"]);

                        checkBox17.Checked = Convert.ToBoolean(rrr["有无蓝图"]);


                        checkBox4.Checked = Convert.ToBoolean(rrr["生效"]);
                        txt_负责人.Text = rrr["负责人"].ToString();
                        //  txt_更改预计完成时间.EditValue = rrr["更改预计完成时间"];
                        txt_物料状态.EditValue = rrr["物料状态"].ToString();
                        cb_供应商编号.EditValue = rrr["供应商编号"].ToString();
                        cb_车间编号.EditValue = rrr["车间编号"].ToString();
                        searchLookUpEdit4.EditValue = rrr["b_班组编号"].ToString();
                        //s_寄售客户.EditValue = rrr["寄售客户ID"].ToString();

                        txt_图纸版本.Text = rrr["图纸版本"].ToString();
                        cb_物料属性.EditValue = rrr["物料属性"].ToString();
                        txt_审核.Text = rrr["审核"].ToString();
                        strNo = 2;

                        string ss = string.Format("select * from 计划人员关联物料表 where 物料编码='{0}' ", txt_物料编码.Text);
                        using (SqlDataAdapter da = new SqlDataAdapter(ss, strconn))
                        {
                            DataTable temp = new DataTable();
                            da.Fill(temp);
                            if (temp.Rows.Count > 0)
                            {
                                serl_计划员.EditValue = temp.Rows[0]["工号"];
                            }
                            else
                            {
                                serl_计划员.EditValue = null;

                            }

                        }
                    }
                }
                //判断右键菜单是否可用
                if (dt_员工.Rows.Count > 0)
                {
                    if (dt_员工.Rows[0]["权限组"].ToString() == "管理员权限" || dt_员工.Rows[0]["权限组"].ToString() == "开发部权限" || dt_员工.Rows[0]["权限组"].ToString() == "工艺部权限")
                    {
                        if (e != null && e.Button == MouseButtons.Right)
                        {
                            DataRow rrr = gvv.GetDataRow(e.RowHandle);
                            if (rrr["虚拟件"].Equals(true)) 虚拟件维护ToolStripMenuItem.Visible = true;
                            else 虚拟件维护ToolStripMenuItem.Visible = false;

                            contextMenuStrip1.Show(gcc, new Point(e.X, e.Y));
                        }
                    }
                }

            }
            catch (Exception  ex )
            {


            }
        }

        //复制 只有新增物料的时候 会用
        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                strNo = 1;
                DataRow r = gvv.GetDataRow(gvv.FocusedRowHandle);
                DataRow[] ds = dtM.Select(string.Format("物料编码 = '{0}'", r["物料编码"].ToString()));
                DataRow dr = dtM.NewRow();
                dr.ItemArray = ds[0].ItemArray;
                // tb2.Text = dr["物料编码"].ToString();
                tb3_物料名称.Text = dr["物料名称"].ToString();
                tb4_规格型号.Text = dr["规格型号"].ToString();
                cb4_规格.Text = dr["规格型号"].ToString();
                tb5.Text = dr["自定义项1"].ToString();
                zdy2.Text =dr["自定义项2"].ToString();
                textBox1.Text = dr["拼板数量"].ToString();
                cb_商品分类.Text = dr["物料类型"].ToString();
                cb3_产品线.Text = dr["产品线"].ToString();
                //cb6_大类.EditValue = dr["大类"].ToString();
                //cb7_小类.Text = dr["小类"].ToString();
                t_dl.Text = dr["大类"].ToString();
                t_xl.Text = dr["小类"].ToString();

                txt_存货分类.Text = dr["存货分类"].ToString();
                txt_分类编码.Text = dr["存货分类编码"].ToString();
                // cb4_规格.Text = dr["规格"].ToString();
                cb1_物料等级.Text = dr["物料等级"].ToString();
                //cb10_壳架等级.Text = dr["壳架等级"].ToString();
                //cb8_极数.Text = dr["极数"].ToString();
                //cb5_电压.Text = dr["电压"].ToString();

                //   cb_锁芯.Text = dr["锁芯"].ToString();
                tb6_产品类别.Text = dr["产品类别"].ToString();
                //cb_保护特性.Text = dr["保护特性"].ToString();
                //cb_断路器型号.Text = dr["断路器型号"].ToString();
                //cb_漏电.Text = dr["漏电"].ToString();、

                //    sl_滑盖颜色.EditValue = dr["滑盖颜色"].ToString();
                sl_壳体颜色.EditValue = dr["壳体颜色"].ToString();

                chkBx_蓝牙.Checked = Convert.ToBoolean(dr["有无蓝牙"]);
                checkBox3.Checked = Convert.ToBoolean(dr["ECN"]);
                searchLookUpEdit2.EditValue = dr["硬件版本"].ToString();
                checkBox8.Checked = Convert.ToBoolean(dr["是否联动"]);
                checkBox7.Checked = Convert.ToBoolean(dr["有无天地钩"]);
                textBox4.Text = dr["导向片规格"].ToString();
                searchLookUpEdit3.EditValue = dr["把手类型"].ToString();
                comboBox2.Text = dr["商品类型"].ToString();
                //comboBox4.Text = dr["锁体状态"].ToString();
                cb_扫描方式.Text = dr["扫描方式"].ToString();
                //cb_额定电流.Text = dr["额定电流"].ToString();
                //txt_智能型号.Text = dr["智能型号"].ToString();

                tb10.Text = dr["客户"].ToString();
                searchLookUpEdit1.EditValue = dr["计量单位编码"].ToString();
                tb9.Text = dr["标准单价"].ToString();
                tb15.Text = dr["库存上限"].ToString();
                tb16.Text = dr["库存下限"].ToString();
                tb8.Text = dr["克重"].ToString();
                cb11_环保.EditValue = dr["环保"].ToString();
                cb_供应状态.EditValue = dr["供应状态"].ToString();

                // cb_ESD等级.Text = dr["ESD等级"].ToString();
                //tb14.Text = dr["库位编号"].ToString();
                //tb17.Text = dr["库位描述"].ToString();
                cb2.Text = dr["物料来源"].ToString();
                tb11.Text = dr["采购周期"].ToString();
                txt_默认供应商.Text = dr["默认供应商"].ToString();

                textBox3.Text = dr["采购供应商备注"].ToString();
                checkBox10.Checked = Convert.ToBoolean(dr["标签打印"]);
                tb12.Text = dr["最小包装"].ToString();
                checkBox5.Checked = Convert.ToBoolean(dr["停用"]);
                checkBox2.Checked = Convert.ToBoolean(dr["BOM确认"]);
                checkBox6.Checked = Convert.ToBoolean(dr["可售"]);
                ck_可购.Checked = Convert.ToBoolean(dr["可购"]);
                //checkBox18.Checked = Convert.ToBoolean(dr["铆压"]);
                ck_内销.Checked = Convert.ToBoolean(dr["内销"]);
                ck_外销.Checked = Convert.ToBoolean(dr["外销"]);
                ck_委外.Checked = Convert.ToBoolean(dr["委外"]);
                ck_自制.Checked = Convert.ToBoolean(dr["自制"]);
                ck_资产.Checked = Convert.ToBoolean(dr["资产"]);
                ck_应税劳务.Checked = Convert.ToBoolean(dr["应税劳务"]);
                checkBox1.Checked = Convert.ToBoolean(dr["在研"]);
                ck_虚拟件.Checked = Convert.ToBoolean(dr["虚拟件"]);

                checkBox17.Checked = Convert.ToBoolean(dr["有无蓝图"]);
                checkBox4.Checked = Convert.ToBoolean(dr["生效"]);
                txt_货架编号.Text = dr["货架编号"].ToString();
                txt_货架描述.Text = dr["货架描述"].ToString();
                txt_车间.Text = dr["车间"].ToString();
                textBox6.Text = dr["b_班组名称"].ToString();
                txt_工时.Text = dr["工时"].ToString();
                textBox5.Text = dr["工艺工时"].ToString();
                checkBox9.Checked = false;      //18-1-31 字段重用 新增物料为true

                teshubeizhu.Text = dr["特殊备注"].ToString();

                wuliaobeizhu.Text = dr["物料备注"].ToString();     //

                //xilei.Text = dr["细类"].ToString();     //
                xiaoshoudanjia.Text = dr["n销售单价"].ToString();     //
                hesuandanjia.Text = dr["n核算单价"].ToString();     //
                cb_仓库编号.Text = dr["仓库号"].ToString();     //
                cangkumiaoshu.Text = dr["仓库名称"].ToString();     //

                txt_负责人.Text = dr["负责人"].ToString();
                //  txt_更改预计完成时间.EditValue = dr["更改预计完成时间"];
                txt_物料状态.EditValue = dr["物料状态"].ToString();
                cb_供应商编号.EditValue = dr["供应商编号"].ToString();
                cb_车间编号.EditValue = dr["车间编号"].ToString();
                searchLookUpEdit4.EditValue = dr["b_班组编号"].ToString();
                //s_寄售客户.EditValue = dr["寄售客户ID"].ToString();

                txt_图纸版本.Text = dr["图纸版本"].ToString();
                cb_物料属性.EditValue = dr["物料属性"].ToString();
                txt_审核.Text = dr["审核"].ToString();
                //if (tb6_物料类型.EditValue != null && tb6_物料类型.EditValue.ToString() != "成品")
                //{
                //    cb4_规格.ReadOnly = false;
                //}
                //else
                //{
                //    cb4_规格.ReadOnly = true;

                //}
                //if (cb3_产品线.EditValue != null && cb3_产品线.EditValue.ToString() != "智能终端电器")
                //{
                //    cb4_规格.ReadOnly = true;
                //}
                //else
                //{
                //    cb4_规格.ReadOnly = false;

                //}
                //strNo = 2;
            }
            catch
            {
                //标签打印
            }
        }

        //预览
        private void button4_Click(object sender, EventArgs e)
        {
            if (button4.Text == "预览")
            {
                fun_规格();
                button4.Text = "清除";
            }
            else
            {
                cb4_规格.Text = "";
                button4.Text = "预览";
            }
        }

        private void checkBox16_CheckedChanged(object sender, EventArgs e)
        {
            checkBox14.Checked = false;
            checkBox12.Checked = false;
            checkBox13.Checked = false;
            checkBox11.Checked = false;
            if (checkBox16.Checked == false)
            {
                gcc.DataSource = dv;
            }
            else
            {
                DataView dvv = new DataView(dtM);
                dvv.RowFilter = "新数据 = true";
                gcc.DataSource = dvv;   //用于显示旧数据
                //gcc.DataSource = dt;
            }
        }
        #endregion

        #region 右键菜单
        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //   Clipboard.SetDataObject(gvv.GetFocusedRowCellValue(gvv.FocusedColumn));
        }

        private void 跳转BOM信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow rr = gvv.GetDataRow(gvv.FocusedRowHandle);
                if (rr["物料编码"].ToString().Substring(0, 2) == "01" && !Convert.ToBoolean(rr["委外"]))
                {
                    throw new Exception("01码不是委外属性不可以维护BOM");
                }

                frm基础数据物料BOM.XTC = this.xtra;
                DataRow dr = gvv.GetDataRow(gvv.FocusedRowHandle);
                str_物料编码 = dr["物料编码"].ToString();
                str_物料名称 = dr["物料名称"].ToString();
                str_规格 = dr["规格"].ToString();
                str_原规格型号 = dr["规格型号"].ToString();

                aaaa.fun_(str_物料编码, str_物料名称, str_规格);

                xtra.SelectedTabPage = xtraTabPage4;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 跳转包装清单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gvv.GetDataRow(gvv.FocusedRowHandle);
            string str = dr["物料编码"].ToString();
            string strr = dr["物料名称"].ToString();
            fm基础数据包装清单_物料信息扩展界面.XTC = this.xtra;
            fm基础数据包装清单_物料信息扩展界面 fm = new fm基础数据包装清单_物料信息扩展界面(str, strr);
            XtraTabPage xtp = xtra.TabPages.Add("包装清单");
            xtp.Name = "包装清单";
            xtp.ShowCloseButton = DefaultBoolean.Default;
            xtp.Controls.Add(fm);
            fm.Dock = DockStyle.Fill;
            xtra.SelectedTabPage = xtp;
        }

        private void 物料替换ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm基础数据物料替换.XTC = this.xtra;
            //仅限原材料和半成品需要替换
            DataRow dr = gvv.GetDataRow(gvv.FocusedRowHandle);
            if (dr["产品类别"].ToString() == "成品")
            {
                MessageBox.Show("该物料为成品，BOM结构中不能替换");
            }
            else
            {
                string str = dr["物料编码"].ToString();
                string strr = dr["物料名称"].ToString();
                string srt = dr["n原ERP规格型号"].ToString();
                frm基础数据物料替换 fm = new frm基础数据物料替换(str, strr, srt);
                XtraTabPage xtp = xtra.TabPages.Add("物料替换");
                xtp.ShowCloseButton = DefaultBoolean.Default;
                xtp.Controls.Add(fm);
                fm.Dock = DockStyle.Fill;
                xtra.SelectedTabPage = xtp;
            }
        }
        //BOM信息复制
        private void bOM信息复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gvv.GetDataRow(gvv.FocusedRowHandle);
            UI基础数据BOM信息复制.XTC = this.xtra;

            UI基础数据BOM信息复制 ui = new UI基础数据BOM信息复制(dr["物料编码"].ToString());

            XtraTabPage xtp = xtra.TabPages.Add("BOM信息复制");
            xtp.Name = "xtraBOM";
            xtp.ShowCloseButton = DefaultBoolean.Default;
            xtp.Controls.Add(ui);
            ui.Dock = DockStyle.Fill;
            xtra.SelectedTabPage = xtp;

        }

        private void 盒贴信息维护ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //仅限成品需要替换
            try
            {
                DataRow dr = gvv.GetDataRow(gvv.FocusedRowHandle);
                if (dr["产品类别"].ToString() == "半成品" || dr["产品类别"].ToString() == "原材料")
                {
                    MessageBox.Show("该物料为半成品或者原材料，没有相应的盒贴信息");
                }
                else
                {
                    frm成品检验盒贴信息维护.XTC = this.xtra;
                    string str = dr["物料编码"].ToString();
                    string strr = dr["物料名称"].ToString();
                    frm成品检验盒贴信息维护 fm = new frm成品检验盒贴信息维护(str, strr);
                    XtraTabPage xtp = xtra.TabPages.Add("盒贴维护");
                    xtp.ShowCloseButton = DefaultBoolean.Default;
                    xtp.Controls.Add(fm);
                    fm.Dock = DockStyle.Fill;
                    xtra.SelectedTabPage = xtp;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }



        private void 物料修改日志ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gvv.GetDataRow(gvv.FocusedRowHandle);
            BaseData.frm物料修改日志 frm = new BaseData.frm物料修改日志(dr["物料编码"].ToString(), dr["物料名称"].ToString(), dr["n原ERP规格型号"].ToString());
            CPublic.UIcontrol.AddNewPage(frm, "物料修改日志");
        }
        #endregion

        #region 触发事件
        //private void cb6_大类_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string sqll = string.Format("select 物料类型GUID from 基础数据物料类型表 where 物料类型名称 = '{0}'", cb6_大类.EditValue.ToString());
        //        DataTable dt = new DataTable();
        //        SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn);
        //        daa.Fill(dt);

        //        string sql = string.Format("select 物料类型名称 from 基础数据物料类型表 where 上级类型GUID = '{0}' order by 物料类型名称", dt.Rows[0]["物料类型GUID"].ToString());
        //        DataTable dt_大小类 = new DataTable();
        //        SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
        //        da.Fill(dt_大小类);
        //        cb7_小类.Properties.Items.Clear();
        //        foreach (DataRow r in dt_大小类.Rows)
        //        {
        //            cb7_小类.Properties.Items.Add(r["物料类型名称"].ToString());
        //        }
        //    }
        //    catch { }
        //} //由大类决定小类



        private void cb_供应商编号_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_供应商编号.EditValue != null)
                {
                    DataRow[] ds = dt_供应商.Select(string.Format("供应商ID = '{0}'", cb_供应商编号.EditValue.ToString()));
                    if (ds.Length > 0)
                    {
                        txt_默认供应商.Text = ds[0]["供应商名称"].ToString();
                    }
                }
                else
                {
                    txt_默认供应商.Text = "";
                }
            }
            catch { }
        }

        private void cb_车间编号_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_车间编号.EditValue != null && cb_车间编号.EditValue.ToString() != "")
                {
                    DataRow[] ds = dt_车间.Select(string.Format("部门编号 = '{0}'", cb_车间编号.EditValue.ToString()));
                    if (ds.Length > 0)
                    {
                        txt_车间.Text = ds[0]["部门名称"].ToString();
                    }
                }
                else
                {
                    txt_车间.Text = "";

                }
            }
            catch { }
        }

        private void gvv_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {


                if (gvv.GetRowCellValue(e.RowHandle, "审核") != null && gvv.GetRowCellValue(e.RowHandle, "审核").ToString() == "已审核")
                {
                    e.Appearance.BackColor = Color.LightBlue;
                    e.Appearance.BackColor2 = Color.LightBlue;
                }
            }
            catch (Exception)
            {


            }

        }



        private void tb6_物料类型_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (tb6_产品类别.EditValue != null)
                {
                    //当不是成品时，不自动生成规格，并且规格可以编辑
                    if (tb6_产品类别.EditValue.ToString() != "" && tb6_产品类别.EditValue.ToString() != "成品")
                    {
                        // cb4_规格.ReadOnly = false;
                        button4.Visible = false;
                    }
                    else   //成品
                    {
                        //cb4_规格.ReadOnly = true;
                        button4.Visible = true;

                        checkBox6.Checked = true;  //可售
                        ck_可购.Checked = false;
                    }
                    if (tb6_产品类别.EditValue.ToString() == "原材料")
                    {
                        checkBox6.Checked = false;

                        ck_可购.Checked = true;  //可购

                    }

                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + "tb6_物料类型_EditValueChanged");
            }
        }

        private void cb_仓库编号_EditValueChanged_1(object sender, EventArgs e)
        {

            if (cb_仓库编号.EditValue != null && cb_仓库编号.EditValue.ToString() != "")
            {
                //选完仓库编号后，显示仓库名称
                DataRow[] ds = dt_属性.Select(string.Format("属性字段1 = '{0}' and  属性类别 = '仓库类别'", cb_仓库编号.EditValue.ToString()));
                if (ds.Length > 0)
                {
                    cangkumiaoshu.Text = ds[0]["属性值"].ToString();
                }
            }
            else
            {
                cangkumiaoshu.Text = "";
            }
        }
        #endregion

        private void button1_Click_1(object sender, EventArgs e)
        {
            fun_智能_规格();
        }

        private void cb3_产品线_EuiditValueChanged(object sender, EventArgs e)
        {
            if (cb3_产品线.EditValue != null && cb3_产品线.EditValue.ToString() == "智能终端电器")
            {
                //cb4_规格.ReadOnly = false;

            }
            else
            {
                //  cb4_规格.ReadOnly = true;

            }
        }

        private void gvv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        //private void s_寄售客户_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (s_寄售客户.EditValue != null && s_寄售客户.EditValue.ToString() != "")
        //        {

        //            DataRow[] ds = dt_寄售客户.Select(string.Format("客户编号 = '{0}'", s_寄售客户.EditValue.ToString()));
        //            if (ds.Length > 0)
        //            {
        //                textBox2.Text = ds[0]["客户名称"].ToString();
        //            }
        //        }
        //        else
        //        {
        //            textBox2.Text = "";

        //        }
        //    }
        //    catch { }


        //}

        //private void gvv_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (gvv.FocusedColumn.Caption == "物料编码" || gvv.FocusedColumn.Caption == "规格型号")
        //    {
        //        if (e.Control && e.KeyCode == Keys.C)
        //        {
        //            Clipboard.SetDataObject(gvv.GetFocusedRowCellValue(gvv.FocusedColumn));
        //            e.Handled = true;
        //        }
        //    }
        //}

        private void 蓝图维护ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ui蓝图维护.XTC = this.xtra;
            DataRow dr = gvv.GetDataRow(gvv.FocusedRowHandle);
            ui蓝图维护 ui = new ui蓝图维护(dr);
            XtraTabPage xtp = xtra.TabPages.Add("物料蓝图维护");
            xtp.Name = "xtraBluePrint";
            xtp.ShowCloseButton = DefaultBoolean.Default;
            xtp.Controls.Add(ui);
            ui.Dock = DockStyle.Fill;
            xtra.SelectedTabPage = xtp;
        }

        private void 标签维护ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ui物料小标签信息维护.XTC = this.xtra;
            DataRow dr = gvv.GetDataRow(gvv.FocusedRowHandle);

            ERPorg.Form1 fm = new ERPorg.Form1();
            fm.Text = "维护标签信息";
            ui物料小标签信息维护 ui = new ui物料小标签信息维护(dr);
            //XtraTabPage xtp = xtra.TabPages.Add("物料标签维护");
            fm.Controls.Add(ui);
            ui.Dock = DockStyle.Fill;
            fm.StartPosition = FormStartPosition.CenterScreen;
            fm.Size = new Size(800, 600);
            fm.ShowDialog();
            if (ui.bl_save)
            {
                refresh_single(dr["物料编码"].ToString());

            }

            //xtp.Name = "xtra标签维护";
            //xtp.ShowCloseButton = DefaultBoolean.Default;
            //xtp.Controls.Add(ui);
            //ui.Dock = DockStyle.Fill;
            //xtra.SelectedTabPage = xtp;




        }

        private void 作业指导书维护ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ui作业指导书上传.XTC = this.xtra;
            DataRow dr = gvv.GetDataRow(gvv.FocusedRowHandle);
            ui作业指导书上传 ui = new ui作业指导书上传(dr);
            XtraTabPage xtp = xtra.TabPages.Add("作业指导书维护");
            xtp.Name = "xtra作业指导书";
            xtp.ShowCloseButton = DefaultBoolean.Default;
            xtp.Controls.Add(ui);
            ui.Dock = DockStyle.Fill;
            xtra.SelectedTabPage = xtp;
        }



        //private void gvv_ColumnPositionChanged(object sender, EventArgs e)
        //{
        //    try
        //    {

        //        if (cfgfilepath != "")
        //        {
        //            gvv.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
        //        }

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        private void 委外BOM维护ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ui委外BOM维护.XTC = this.xtra;
            DataRow dr = gvv.GetDataRow(gvv.FocusedRowHandle);
            ui委外BOM维护 ui = new ui委外BOM维护(dr);
            XtraTabPage xtp = xtra.TabPages.Add("委外BOM维护");
            xtp.Name = "委外BOM维护";
            xtp.ShowCloseButton = DefaultBoolean.Default;
            xtp.Controls.Add(ui);
            ui.Dock = DockStyle.Fill;
            xtra.SelectedTabPage = xtp;
        }

        private void 版本维护ToolStripMenuItem_Click(object sender, EventArgs e)
        {



            ui版本维护.XTC = this.xtra;
            DataRow dr = gvv.GetDataRow(gvv.FocusedRowHandle);
            ui版本维护 ui = new ui版本维护(dr);
            XtraTabPage xtp = xtra.TabPages.Add("版本维护");
            xtp.Name = "版本维护";
            xtp.ShowCloseButton = DefaultBoolean.Default;
            xtp.Controls.Add(ui);
            ui.Dock = DockStyle.Fill;
            xtra.SelectedTabPage = xtp;


        }



        private void tb11_KeyPress_1(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar != '\b' && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void 明牌信息维护ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, "MoldMangement.dll"));//dr["dll全路径"] = "动态载入dll.dll"
            Type outerForm = outerAsm.GetType("MoldMangement.UI明牌", false);//动态载入dll.UI动态载入窗体
            UserControl ui = Activator.CreateInstance(outerForm) as UserControl;

            //MoldMangement.UI维护.XTC = this.xtra;
            DataRow dr = gvv.GetDataRow(gvv.FocusedRowHandle);
            // MoldMangement.UI维护 ui = new MoldMangement.UI维护(dr);
            XtraTabPage xtp = xtra.TabPages.Add("铭牌信息维护");
            xtp.Name = "铭牌信息维护";
            xtp.ShowCloseButton = DefaultBoolean.Default;
            xtp.Controls.Add(ui);
            ui.Dock = DockStyle.Fill;
            xtra.SelectedTabPage = xtp;
        }

        private void 单位换算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gvv.GetDataRow(gvv.FocusedRowHandle);
            Ui单位换算 ui = new Ui单位换算(dr);
            CPublic.UIcontrol.Showpage(ui, "单位换算维护");


        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
                {
                    DataRow[] r = dt_单位.Select(string.Format("计量单位编码='{0}'", searchLookUpEdit1.EditValue));
                    if (r.Length > 0) textBox2.Text = r[0]["计量单位"].ToString();
                    else textBox2.Text = "";

                }
                else
                {

                    textBox2.Text = "";
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void 虚拟件维护ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gvv.GetDataRow(gvv.FocusedRowHandle);
            ui虚拟件维护 ui = new ui虚拟件维护(dr);
            CPublic.UIcontrol.Showpage(ui, "虚拟件维护");
        }


        private void 包装方式维护ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DataRow dr = gvv.GetDataRow(gvv.FocusedRowHandle);
            // form包装方式 ui = new form包装方式(dr);
            //  CPublic.UIcontrol.Showpage(ui, "虚拟件维护");
        }

        private void xtra_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barLargeButtonItem9.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            if (e.Page.Name == "xtraTabPage5")
            {
                barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barLargeButtonItem9.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                s_页面 = "xtraTabPage5";
            }
            else if (e.Page.Name == "xtraTabPage1")
            {
                barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barLargeButtonItem9.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                s_页面 = "xtraTabPage1";
            }
            else if(e.Page.Name == "xtraTabPage4")
            {
                barLargeButtonItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never ;
                barLargeButtonItem6.Visibility = DevExpress.XtraBars.BarItemVisibility.Never ;
                s_页面 = "xtraTabPage4";
            }

        }

        private void treeList1_MouseClick(object sender, MouseEventArgs e)
        {
            if (treeList1.Nodes.Count > 0)
            {
                if (treeList1.Selection[0] == null) return;
            }
            else
            {
                return;
            }
            TreeListNode n = treeList1.Selection[0];
            string s = n.GetValue("存货分类编码").ToString();
            DataView v = new DataView(dtM);
            v.RowFilter = String.Format("存货分类编码 like '{0}%'", s);
            gridControl1.DataSource = v;



        }


        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow rrr = gridView1.GetDataRow(e.RowHandle);
                if (rrr == null) return;
                refresh_single(rrr["物料编码"].ToString());
                if (e.Clicks == 2)
                {


                    dv = new DataView(dtM);
                    dv.RowFilter = string.Format("物料编码='{0}'", rrr["物料编码"]);
                    gcc.DataSource = dv;
                    txt_物料编码.Text = rrr["物料编码"].ToString();
                    txt_物料编码.ReadOnly = true;
                    tb2.Text = rrr["物料编码"].ToString();
                    tb3_物料名称.Text = rrr["物料名称"].ToString();
                    cb4_规格.Text = tb4_规格型号.Text = rrr["规格型号"].ToString();
                    tb5.Text = rrr["自定义项1"].ToString();
                    zdy2.Text = rrr["自定义项2"].ToString();
                    textBox1.Text = rrr["拼板数量"].ToString();
                    cb_商品分类.Text = rrr["物料类型"].ToString();
                    cb3_产品线.Text = rrr["产品线"].ToString();
                    //cb6_大类.EditValue = rrr["大类"].ToString();
                    //cb7_小类.Text = rrr["小类"].ToString();
                    t_dl.Text = rrr["大类"].ToString();
                    t_xl.Text = rrr["小类"].ToString();

                    txt_存货分类.Text = rrr["存货分类"].ToString();
                    txt_分类编码.Text = rrr["存货分类编码"].ToString();

                    // cb4_规格.Text = rrr["规格"].ToString();
                    cb1_物料等级.Text = rrr["物料等级"].ToString();
                    //cb10_壳架等级.Text = rrr["壳架等级"].ToString();
                    //cb8_极数.Text = rrr["极数"].ToString();
                    //cb5_电压.Text = rrr["电压"].ToString();

                    //       cb_锁芯.Text = rrr["锁芯"].ToString();
                    tb6_产品类别.Text = rrr["产品类别"].ToString();
                    sl_壳体颜色.EditValue = rrr["壳体颜色"].ToString();
                    // sl_滑盖颜色.EditValue = rrr["滑盖颜色"].ToString();
                    chkBx_蓝牙.Checked = Convert.ToBoolean(rrr["有无蓝牙"]);
                    checkBox3.Checked = Convert.ToBoolean(rrr["ECN"]);
                    searchLookUpEdit2.EditValue = rrr["硬件版本"].ToString();
                    checkBox8.Checked = Convert.ToBoolean(rrr["是否联动"]);
                    checkBox7.Checked = Convert.ToBoolean(rrr["有无天地钩"]);
                    textBox4.Text = rrr["导向片规格"].ToString();
                    searchLookUpEdit3.EditValue = rrr["把手类型"].ToString();
                    if (rrr["商品类型"].ToString() == "")
                    {
                        comboBox2.Text = null;
                    }
                    else
                    {
                        comboBox2.Text = rrr["商品类型"].ToString();
                    }
                    if (rrr["物料类型"].ToString() == "")
                    {
                        cb_商品分类.Text = null;
                    }
                    else
                    {
                        cb_商品分类.Text = rrr["物料类型"].ToString();
                    }

                    //comboBox4.Text = rrr["锁体状态"].ToString();
                    //cb_保护特性.Text = rrr["保护特性"].ToString();
                    //cb_断路器型号.Text = rrr["断路器型号"].ToString();
                    //cb_漏电.Text = rrr["漏电"].ToString();
                    cb_扫描方式.Text = rrr["扫描方式"].ToString();
                    //cb_额定电流.Text = rrr["额定电流"].ToString();



                    tb10.Text = rrr["客户"].ToString();
                    searchLookUpEdit1.EditValue = rrr["计量单位编码"].ToString();
                    //  textBox2.Text = rrr["计量单位"].ToString();

                    tb9.Text = rrr["标准单价"].ToString();
                    tb15.Text = rrr["库存上限"].ToString();
                    tb16.Text = rrr["库存下限"].ToString();
                    tb8.Text = rrr["克重"].ToString();
                    cb11_环保.EditValue = rrr["环保"].ToString();
                    cb_供应状态.EditValue = rrr["供应状态"].ToString();

                    //cb_ESD等级.Text = rrr["ESD等级"].ToString();
                    //tb14.Text = rrr["库位编号"].ToString();
                    //tb17.Text = rrr["库位描述"].ToString();
                    cb2.Text = rrr["物料来源"].ToString();
                    tb11.Text = rrr["采购周期"].ToString();
                    txt_默认供应商.Text = rrr["默认供应商"].ToString();
                    textBox3.Text = rrr["采购供应商备注"].ToString();
                    checkBox10.Checked = Convert.ToBoolean(rrr["标签打印"]);
                    tb12.Text = rrr["最小包装"].ToString();
                    cb12.Text = rrr["主辅料"].ToString();
                    txt_货架编号.Text = rrr["货架编号"].ToString();
                    txt_货架描述.Text = rrr["货架描述"].ToString();
                    txt_车间.Text = rrr["车间"].ToString();
                    textBox6.Text = rrr["b_班组名称"].ToString();
                    txt_工时.Text = rrr["工时"].ToString();
                    textBox5.Text = rrr["工艺工时"].ToString();
                    checkBox9.Checked = Convert.ToBoolean(rrr["新数据"]);      //
                    teshubeizhu.Text = rrr["特殊备注"].ToString();
                    // xinghaozixiang.Text = rrr["型号子项"].ToString();//

                    wuliaobeizhu.Text = rrr["物料备注"].ToString();     //
                    //yuanguigexinghao.Text = rrr["原规格型号"].ToString();     //
                    //xilei.Text = rrr["细类"].ToString();     //
                    xiaoshoudanjia.Text = rrr["n销售单价"].ToString();     //
                    hesuandanjia.Text = rrr["n核算单价"].ToString();     //
                    cb_仓库编号.Text = rrr["仓库号"].ToString();     //
                    cangkumiaoshu.Text = rrr["仓库名称"].ToString();     //
                    //yuanERPguigexinghao.Text = rrr["n原ERP规格型号"].ToString();
                    checkBox5.Checked = Convert.ToBoolean(rrr["停用"]);
                    checkBox2.Checked = Convert.ToBoolean(rrr["BOM确认"]);

                    checkBox6.Checked = Convert.ToBoolean(rrr["可售"]);
                    ck_可购.Checked = Convert.ToBoolean(rrr["可购"]);
                    //checkBox18.Checked = Convert.ToBoolean(rrr["铆压"]);
                    ck_内销.Checked = Convert.ToBoolean(rrr["内销"]);
                    ck_外销.Checked = Convert.ToBoolean(rrr["外销"]);
                    ck_委外.Checked = Convert.ToBoolean(rrr["委外"]);
                    ck_自制.Checked = Convert.ToBoolean(rrr["自制"]);
                    checkBox1.Checked = Convert.ToBoolean(rrr["在研"]);
                    ck_资产.Checked = Convert.ToBoolean(rrr["资产"]);
                    ck_虚拟件.Checked = Convert.ToBoolean(rrr["虚拟件"]);

                    checkBox17.Checked = Convert.ToBoolean(rrr["有无蓝图"]);


                    checkBox4.Checked = Convert.ToBoolean(rrr["生效"]);
                    txt_负责人.Text = rrr["负责人"].ToString();
                    //  txt_更改预计完成时间.EditValue = rrr["更改预计完成时间"];
                    txt_物料状态.EditValue = rrr["物料状态"].ToString();
                    cb_供应商编号.EditValue = rrr["供应商编号"].ToString();
                    cb_车间编号.EditValue = rrr["车间编号"].ToString();
                    searchLookUpEdit4.EditValue = rrr["b_班组编号"].ToString();
                    //s_寄售客户.EditValue = rrr["寄售客户ID"].ToString();

                    txt_图纸版本.Text = rrr["图纸版本"].ToString();
                    cb_物料属性.EditValue = rrr["物料属性"].ToString();
                    txt_审核.Text = rrr["审核"].ToString();

                    xtra.SelectedTabPage = xtraTabPage5;
                    str_新增or修改 = "修改";

                    strNo = 2;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void checkBox17_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox17.Checked) button1.Visible = true;
            else button1.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string s = string.Format("select top 1 * from 基础物料蓝图表 where 物料号 = '{0}' order by 版本 desc", txt_物料编码.Text);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            if (dt.Rows.Count > 0)
            {
                string strConn_FS = CPublic.Var.geConn("FS");
                DataRow dr = dt.Rows[0];
                if (dr["文件地址"] == null || dr["文件地址"].ToString() == "")
                {
                    throw new Exception("没有文件可以查看，请先上传文件");
                }
                string type = dr["后缀"].ToString();
                string foldPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\查看文件\\";
                //string fileName = foldPath + CPublic.Var.getDatetime().ToString("yyyy-MM-dd") + "T" + CPublic.Var.getDatetime().ToString("HH_mm_ss") + "Z" + "_" + Guid.NewGuid().ToString() + "." + type;
                string fileName = foldPath + "预览文件" + "." + type;

                try
                {
                    System.IO.Directory.Delete(foldPath, true);
                }
                catch (Exception)
                {
                }
                CFileTransmission.CFileClient.strCONN = strConn_FS;

                CFileTransmission.CFileClient.Receiver(dr["文件地址"].ToString(), fileName);
                System.Diagnostics.Process.Start(fileName);
            }
        }

        private void barLargeButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (s_页面 == "xtraTabPage5")
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Title = "导出Excel";
                    saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                    DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                    if (dialogResult == DialogResult.OK)
                    {
                        DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();


                        gcc.ExportToXlsx(saveFileDialog.FileName);


                        DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else if (s_页面 == "xtraTabPage1")
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Title = "导出Excel";
                    saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                    DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                    if (dialogResult == DialogResult.OK)
                    {
                        DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();


                        gridControl1.ExportToXlsx(saveFileDialog.FileName);


                        DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                //SaveFileDialog saveFileDialog = new SaveFileDialog();
                //saveFileDialog.Title = "导出Excel";
                //saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                //DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                //if (dialogResult == DialogResult.OK)
                //{
                //    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();


                //    gridControl1.ExportToXlsx(saveFileDialog.FileName);


                //    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");

            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '.' && this.textBox1.Text.IndexOf(".") != -1)
            {
                e.Handled = true;
            }

            if (!((e.KeyChar >= 48 && e.KeyChar <= 57) || e.KeyChar == '.' || e.KeyChar == 8))
            {
                e.Handled = true;
            }
        }

        private void 相关改制产品维护ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 铭牌信息维护ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("当前数据是否已保存？", "确认？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    sync_check();
                    fun_sync_crm();
                }



            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        private void sync_check()
        {
            if (txt_物料编码.Text.Trim() == "")
            {
                throw new Exception("请先选择物料");
            }
            if (tb6_产品类别.Text.Trim() == "")
            {
                throw new Exception("同步至CRM，产品类别必填");
            }
            if (cb_商品分类.Text.Trim() == "")
            {
                throw new Exception("同步至CRM，商品分类必填");
            }
            if (comboBox2.Text.Trim() == "")
            {
                throw new Exception("同步至CRM，商品类别必填");
            }
            if (xiaoshoudanjia.Text.Trim() == "")
            {
                throw new Exception("同步至CRM，销售单价必填");
            }
        }

        private void fun_sync_crm()
        {
            string s = "select  * from [CRM产品需要字段表] ";
            string xx = "产品类别,物料类型,物料编码,物料名称,n销售单价,商品类型";//单独成列得字段
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = "";
            foreach (DataRow dr in dt.Rows)
            {
                s += dr["字段名"].ToString() + ",";
            }
            if (s.Length > 0)
                s = s + xx;
            else throw new Exception("CRM产品需要字段表中无内容");

            //string sql =string.Format(@"select {0} from 基础数据物料信息表 where 规格型号 like 'EH-7000%' " +
            //    "and left(物料编码,2) in ('05','10')",s);
            string strcon_aliyun = string.Format("server={0};User Id={1};password={2};Database={3};CharSet=utf8", CPublic.Var.li_CFG["aliyun_server"].ToString(),
                       CPublic.Var.li_CFG["aliyun_UID"].ToString(), CPublic.Var.li_CFG["aliyun_PWD"].ToString(), CPublic.Var.li_CFG["aliyun_database"].ToString());
            string ssss = string.Format(" select * from  inventory_new  where  InvCode='{0}'", txt_物料编码.Text.Trim());
            MySqlDataAdapter da = new MySqlDataAdapter(ssss, strcon_aliyun);
            DataTable dt_somain = new DataTable();
            da.Fill(dt_somain);

            DataTable dt_copy = dt_somain.Clone();
            string sql = string.Format(@"select {0} from 基础数据物料信息表 where 物料编码='{1}'", s, txt_物料编码.Text);
            DataTable dt_基础 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

            foreach (DataRow dr in dt_基础.Rows)
            {
                foreach (DataColumn dc in dt_基础.Columns)
                {
                    if (xx.Contains(dc.ColumnName)) continue;

                    //如果属性值为空 不需要加入 10-28 
                    //等数据全了以后  加入限制   去除空的属性数据
                    if (dr[dc.ColumnName] == null || dr[dc.ColumnName].ToString().Trim() == "") continue;
                    DataRow dr_crm = dt_copy.NewRow();

                    //锁具属性 ： 壳体颜色,硬件版本,把手类型,waresSeries  
                    DataRow[] ttr = dt.Select(string.Format("字段名='{0}'", dc.ColumnName));
                    if (ttr.Length > 0 && ttr[0]["描述"].ToString() == dr["商品类型"].ToString())
                    {

                        dr_crm["FieldName"] = dc.ColumnName;
                        dr_crm["Value"] = dr[dc.ColumnName];
                    }
                    else
                    {

                        continue;


                    }

                    //锁体属性 ：有无蓝牙,是否联动,有无天地钩,导向片规格 
                    dr_crm["InvCode"] = dr["物料编码"];
                    //dr_crm["FieldName"] = dc.ColumnName;
                    //dr_crm["Value"] = dr[dc.ColumnName];

                    dr_crm["InvName"] = dr["物料名称"];
                    dr_crm["InvStd"] = dr["物料名称"];

                    dr_crm["InvType"] = dr["商品类型"];//界面需要增加字段    锁具/锁体
                    dr_crm["ItemClass"] = dr["产品类别"];
                    dr_crm["WaresSeries"] = dr["物料类型"];
                    dr_crm["Price"] = dr["n销售单价"];
                    string sql_属性 = string.Format("select * from 基础数据基础属性表 where 属性类别 = '{0}' and 属性值 = '{1}'", dc.ColumnName, dr[dc.ColumnName]);
                    DataTable dt111 = CZMaster.MasterSQL.Get_DataTable(sql_属性, strconn);
                    if (dt111.Rows.Count > 0)
                    {
                        dr_crm["ValueDisc"] = dt111.Rows[0]["属性字段2"];//规定属性字段2为给CRM的中文描述
                    }
                    else
                    {
                        if (dc.ColumnName.Contains("有无") || dc.ColumnName.Contains("是否"))
                        {
                            if (Convert.ToBoolean(dr[dc.ColumnName]))
                            {
                                dr_crm["ValueDisc"] = dc.ColumnName.ToString().Substring(0, 1);
                            }
                            else
                            {
                                dr_crm["ValueDisc"] = dc.ColumnName.ToString().Substring(1, 1);
                            }
                        }
                        else
                        {
                            byte[] byte_len = System.Text.Encoding.Default.GetBytes(dr[dc.ColumnName].ToString());
                            if (byte_len.Length == 2)
                            {
                                dr_crm["ValueDisc"] = dr[dc.ColumnName];
                            }
                            else
                            {
                                //throw new Exception(dc.ColumnName + "属性没有中文描述");
                            }

                        }
                    }
                    dr_crm["isUP"] = true;
                    dt_copy.Rows.Add(dr_crm);
                }
            }


            if (dt_somain.Rows.Count == 0)//没有新增  有修改
            {
                foreach (DataRow rr in dt_copy.Rows)
                {
                    dt_somain.ImportRow(rr);
                }
            }
            else
            {
                for (int i = dt_somain.Rows.Count - 1; i >= 0; i--)
                {
                    dt_somain.Rows[i].Delete();
                }
                foreach (DataRow rr in dt_copy.Rows)
                {
                    dt_somain.ImportRow(rr);
                }
            }

            new MySqlCommandBuilder(da);
            da.Update(dt_somain);
            MessageBox.Show("同步成功");
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox2.Text == "锁具")
                {
                    checkBox8.Checked = false;
                    checkBox8.Enabled = false;
                    checkBox7.Checked = false;
                    checkBox7.Enabled = false;
                    textBox4.Text = "";
                    textBox4.Enabled = false;
                    chkBx_蓝牙.Enabled = true;
                    sl_壳体颜色.Enabled = true;
                    searchLookUpEdit2.Enabled = true;
                    searchLookUpEdit3.Enabled = true;
                }
                else if (comboBox2.Text == "锁体")
                {
                    chkBx_蓝牙.Checked = false;
                    chkBx_蓝牙.Enabled = false;
                    sl_壳体颜色.Text = "";
                    sl_壳体颜色.Enabled = false;
                    searchLookUpEdit2.Text = "";
                    searchLookUpEdit2.Enabled = false;
                    searchLookUpEdit3.Text = "";
                    searchLookUpEdit3.Enabled = false;
                    checkBox8.Enabled = true;
                    checkBox7.Enabled = true;
                    textBox4.Enabled = true;
                }
                else
                {
                    chkBx_蓝牙.Enabled = true;
                    sl_壳体颜色.Enabled = true;
                    searchLookUpEdit2.Enabled = true;
                    searchLookUpEdit3.Enabled = true;
                    checkBox8.Enabled = true;
                    checkBox7.Enabled = true;
                    textBox4.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cb_商品分类_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_商品分类.Text == "锁体")
                {
                    comboBox2.Text = "锁体";
                    comboBox2.Enabled = false;
                }
                else
                {
                    comboBox2.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void searchLookUpEdit4_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEdit4.EditValue != null && searchLookUpEdit4.EditValue.ToString() != "")
                {
                    DataRow[] ds = dt_班组.Select(string.Format("b_班组编号 = '{0}'", searchLookUpEdit4.EditValue.ToString()));
                    if (ds.Length > 0)
                    {
                        textBox6.Text = ds[0]["b_班组名称"].ToString();
                    }
                }
                else
                {
                    textBox6.Text = "";

                }
            }
            catch { }
        }

        private void 预览bom层级ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DataRow dr = gvv.GetDataRow(gvv.FocusedRowHandle);
            string s = $@" select* from(
                       select 产品编码 from 基础数据物料BOM表
                      union select  产品编码 from 基础数据BOM修改主表 where 审核= 0)x
                      where 产品编码 = '{dr["物料编码"].ToString()}'";
            DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            if (temp.Rows.Count > 0)
            {
                ui包含未审核bom查询 ui = new ui包含未审核bom查询(dr["物料编码"].ToString());
                CPublic.UIcontrol.Showpage(ui, "预览bom层级");
            }
            else
            {
                MessageBox.Show("选中物料无bom或者未审核的bom");
            }

        }
    }
}
