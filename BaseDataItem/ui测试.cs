using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using CPublic;
using DevExpress.XtraTab;
using System.Drawing.Drawing2D;
using System.Collections;
using DevExpress.Utils;
namespace BaseData
{
    public partial class ui测试 : UserControl
    {
        //如果添加或删除界面字段，需要更改4处
        #region 成员
        DataTable dtM;   // xtra3
        DataTable dt1;   // xtra2 扩展属性表
        int strNo = 0;   //1表示新增；2表示查询，用于修改
        SqlDataAdapter da;
        string strshow;
        int a_查询时使用;
        int a2 = 1;
        int a1 = 1;
        public string str_物料编码 = "";
        public string str_物料名称 = "";
        public string str_规格 = "";
        public string str_原规格型号 = "";
        string strconn = CPublic.Var.strConn;

        DataRow rrr; //???不知道什么作用2016 7 29

        DataView dv;       //用于显示旧数据
        DataTable dt_stock;
        DataTable dt_供应商;
        DataTable dt_车间;
        DataTable dt_属性;

        public DataTable dt_成员;
        string str_新增or修改 = "";
        DataTable dt_保存修改 = null;
        #endregion
        public ui测试()
        {
            InitializeComponent();
        }
        public static class aaaa
        {
            public static List<frm基础数据物料BOM> FM2 = new List<frm基础数据物料BOM>();

            public static void fun_(string str, string strr, string strrr, string str_规格)
            {
                foreach (frm基础数据物料BOM fm in FM2)
                {
                    fm.str_物料编码 = str;
                    fm.str_物料名称 = strr;
                    fm.str_规格 = strrr;
                    fm.str_原规格型号 = str_规格;
                    fm.fun_载入数据();
                }
            }
        }

        private void ui测试_Load(object sender, EventArgs e)
        {
            try
            {
                string sql = string.Format("SELECT * from 人事基础员工表 where 员工号 = '{0}'", CPublic.Var.LocalUserID);
                DataTable tr = new DataTable();
                SqlDataAdapter ada = new SqlDataAdapter(sql, strconn);
                ada.Fill(tr);
                if (tr.Rows[0]["部门"].ToString() == "物管课")
                {
                    pan_采购.Enabled = false;
                    pan_开发.Enabled = false;
                    pan_销售.Enabled = false;
                    pan_仓库.Enabled = true;
                }
                if (tr.Rows[0]["部门编号"].ToString() == "00010101")
                {
                    pan_采购.Enabled = false;
                    pan_开发.Enabled = false;
                    pan_销售.Enabled = true;
                    pan_仓库.Enabled = false;
                }
                if (tr.Rows[0]["部门"].ToString() == "开发一部" || tr.Rows[0]["部门"].ToString() == "开发二部")
                {
                    pan_采购.Enabled = true;
                    pan_开发.Enabled = true;
                    pan_销售.Enabled = true;
                    pan_仓库.Enabled = true;
                }
                if (tr.Rows[0]["课室"].ToString() == "计划课" || tr.Rows[0]["课室"].ToString() == "采购课")
                {
                    pan_采购.Enabled = true;
                    pan_开发.Enabled = false;
                    pan_销售.Enabled = true;
                    pan_仓库.Enabled = false;
                }

                devGridControlCustom1.UserName = CPublic.Var.LocalUserID;
                devGridControlCustom1.strConn = CPublic.Var.strConn;
                fun_载入刷新();
                //checkBox5.Enabled = false;
                //checkBox4.Enabled = false;
                fun_下拉框();
                fun_下拉框searchlookup();
                BaseData.frm基础数据物料BOM fm = new BaseData.frm基础数据物料BOM();
                fm.Dock = System.Windows.Forms.DockStyle.Fill;
                xtra.SelectedTabPage = xtraTabPage4;
                xtraTabPage4.Controls.Add(fm);
                CZMaster.DevGridControlHelper.Helper(this);
                //fun_载入数据(); //基础数据界面  用于快速选择数据
                frm基础数据物料BOM.XTC = this.xtra;
                fm基础数据包装清单_物料信息扩展界面.XTC = this.xtra;
                frm基础数据物料替换.XTC = this.xtra;
                frm成品检验盒贴信息维护.XTC = this.xtra;
                UI基础数据BOM信息复制.XTC = this.xtra;
                xtra.SelectedTabPage = xtraTabPage5;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
            }
        }
        #region 方法
        /// <summary>
        /// 给所有下拉框赋值
        /// </summary>
        public void fun_下拉框()
        {
            cb3_产品线.Properties.Items.Clear();
            //cb6_大类.Properties.Items.Clear();
            cb5_电压.Properties.Items.Clear();
            cb8_极数.Properties.Items.Clear();
            tb6_物料类型.Properties.Items.Clear();
            cb1_物料等级.Properties.Items.Clear();
            cb10_壳架等级.Properties.Items.Clear();
            cb12.Properties.Items.Clear();
            cb2.Properties.Items.Clear();
            cb9.Properties.Items.Clear();
            cb_物料属性.Properties.Items.Clear();
            cb_ESD等级.Properties.Items.Clear();

            string sql1 = "";
            sql1 = "select 员工号,姓名,部门,岗位 from 人事基础员工表";
            DataTable dt_people = CZMaster.MasterSQL.Get_DataTable(sql1, strconn);
            txt_负责人.Properties.DataSource = dt_people;
            txt_负责人.Properties.ValueMember = "姓名";
            txt_负责人.Properties.DisplayMember = "姓名";

            string sql2 = "select 物料类型名称 from 基础数据物料类型表 where 类型级别 = '大类' order by 物料类型名称";
            DataTable dt = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
            da2.Fill(dt);
            //foreach (DataRow r in dt.Rows)
            //{
            //    cb6_大类1.Properties.Items.Add(r["物料类型名称"].ToString());
            //}
            cb6_大类.Properties.DataSource = dt;
            cb6_大类.Properties.ValueMember = "物料类型名称";
            cb6_大类.Properties.DisplayMember = "物料类型名称";

            string sql = "select * from 基础数据基础属性表 order by POS";
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

                if (r["属性类别"].ToString().Equals("电压"))
                {
                    cb5_电压.Properties.Items.Add(r["属性值"].ToString());
                }

                if (r["属性类别"].ToString().Equals("极数"))
                {
                    cb8_极数.Properties.Items.Add(r["属性值"].ToString());
                }

                if (r["属性类别"].ToString().Equals("物料类型"))
                {
                    tb6_物料类型.Properties.Items.Add(r["属性值"].ToString());
                }

                if (r["属性类别"].ToString().Equals("物料等级"))
                {
                    cb1_物料等级.Properties.Items.Add(r["属性值"].ToString());
                }

                if (r["属性类别"].ToString().Equals("壳架等级"))
                {
                    cb10_壳架等级.Properties.Items.Add(r["属性值"].ToString());
                }

                if (r["属性类别"].ToString().Equals("主辅料"))
                {
                    cb12.Properties.Items.Add(r["属性值"].ToString());
                }

                if (r["属性类别"].ToString().Equals("物料来源"))
                {
                    cb2.Properties.Items.Add(r["属性值"].ToString());
                }
                if (r["属性类别"].ToString().Equals("计量单位"))
                {
                    cb9.Properties.Items.Add(r["属性值"].ToString());
                }
                if (r["属性类别"].ToString().Equals("物料属性"))
                {
                    cb_物料属性.Properties.Items.Add(r["属性值"].ToString());
                }
                if (r["属性类别"].ToString().Equals("ESD等级"))
                {
                    cb_ESD等级.Properties.Items.Add(r["属性值"].ToString());
                }
            }
        }


        private void fun_下拉框searchlookup()
        {
            //供应商
            string sql_供应商 = "select 供应商ID,供应商名称 from 采购供应商表 where 供应商状态 = '在用'";
            dt_供应商 = new DataTable();
            SqlDataAdapter da_供应商 = new SqlDataAdapter(sql_供应商, strconn);
            da_供应商.Fill(dt_供应商);
            cb_供应商编号.Properties.DataSource = dt_供应商;
            cb_供应商编号.Properties.DisplayMember = "供应商ID";
            cb_供应商编号.Properties.ValueMember = "供应商ID";
            //车间
            string sql_车间 = "select 部门编号,部门名称 from 人事基础部门表 where 部门编号 >= '0001030101' and 部门编号 <= '0001030107'";
            dt_车间 = new DataTable();
            SqlDataAdapter da_车间 = new SqlDataAdapter(sql_车间, strconn);
            da_车间.Fill(dt_车间);
            DataRow dr = dt_车间.NewRow();
            dr["部门编号"] = "";
            dr["部门名称"] = "";
            dt_车间.Rows.Add(dr);
            cb_车间编号.Properties.DataSource = dt_车间;
            cb_车间编号.Properties.DisplayMember = "部门编号";
            cb_车间编号.Properties.ValueMember = "部门编号";
            //仓库
            string sql = @"select 属性字段1 as 仓库编号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 = '仓库类别'order by 仓库编号 ";
            DataTable dt_仓库 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_仓库);
            cb_仓库编号.Properties.DataSource = dt_仓库;
            cb_仓库编号.Properties.DisplayMember = "仓库编号";
            cb_仓库编号.Properties.ValueMember = "仓库编号";
        }

        /// <summary>
        /// 物料信息扩展属性，给xtra2加载数据
        /// 加载表格，无数据，可做为清空使用
        /// </summary>
        public void fun_xtra2()
        {
            //dt1 = new DataTable();
            //string sql1 = "select POS,物料信息扩展属性 from 基础数据物料信息扩展属性表";
            //SqlDataAdapter da1 = new SqlDataAdapter(sql1, strconn);
            //da1.Fill(dt1);
            //dt1.Columns.Add("属性值");
            //gc.DataSource = dt1;
            //gv.Columns[0].Visible = false;
            //gv.Columns[1].OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            //gv.Columns[1].OptionsColumn.AllowEdit = false;
            //gv.Columns[2].Width = 317;
            //gv.Columns[2].OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
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
                fun_xtra2();
                fun_载入数据();
                if (strNo == 2)  //查询状态刷新
                {
                    if (textBox1.Text == "")
                    {
                        fun_清空数据();
                        txt_物料编码.ReadOnly = false;
                    }
                    else
                    {
                        fun_查询();
                    }
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
        public void fun_查询()
        {
            try
            {
                fun_xtra2();
                //扩展属性
                //try
                //{
                //    string str1 = string.Format("select * from 基础数据物料信息扩展表 where 物料编码 = '{0}'", textBox1.Text);
                //    DataTable dt = new DataTable();
                //    SqlDataAdapter daa = new SqlDataAdapter(str1, strconn);
                //    daa.Fill(dt);
                //    foreach (DataRow dr in dt1.Rows)
                //    {
                //        foreach (DataRow drr in dt.Rows)
                //        {
                //            if (dr["物料信息扩展属性"].ToString() == drr["物料属性"].ToString())
                //            {
                //                dr["属性值"] = drr["属性值"].ToString();
                //            }
                //        }
                //    }
                //    gc.DataSource = dt1;
                //}
                //catch { }

                //基础属性
                fun_清空数据();
                string str = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", textBox1.Text);
                SqlDataAdapter da = new SqlDataAdapter(str, strconn);
                DataTable dtM1 = new DataTable();
                new SqlCommandBuilder(da);
                da.Fill(dtM1);
                a_查询时使用 = dtM1.Rows.Count;
                if (dtM1.Rows.Count > 0)
                {
                    txt_物料编码.Text = dtM1.Rows[0]["物料编码"].ToString();
                    txt_物料编码.ReadOnly = true;
                    tb2.Text = dtM1.Rows[0]["物料编码"].ToString();
                    tb3_物料名称.Text = dtM1.Rows[0]["物料名称"].ToString();
                    tb4_规格型号.Text = dtM1.Rows[0]["规格型号"].ToString();
                    tb5.Text = dtM1.Rows[0]["图纸编号"].ToString();
                    tb6_物料类型.Text = dtM1.Rows[0]["物料类型"].ToString();
                    cb3_产品线.Text = dtM1.Rows[0]["产品线"].ToString();
                    cb6_大类.Text = dtM1.Rows[0]["大类"].ToString();
                    cb7_小类.Text = dtM1.Rows[0]["小类"].ToString();
                    cb4_规格.Text = dtM1.Rows[0]["规格"].ToString();
                    cb1_物料等级.Text = dtM1.Rows[0]["物料等级"].ToString();
                    cb10_壳架等级.Text = dtM1.Rows[0]["壳架等级"].ToString();
                    cb8_极数.Text = dtM1.Rows[0]["极数"].ToString();
                    cb5_电压.Text = dtM1.Rows[0]["电压"].ToString();
                    cb_结构代码.Text = dtM1.Rows[0]["细分功能结构代码"].ToString();
                    cb_分断能力.Text = dtM1.Rows[0]["分断能力"].ToString();
                    cb_保护特性.Text = dtM1.Rows[0]["保护特性"].ToString();
                    cb_断路器型号.Text = dtM1.Rows[0]["断路器型号"].ToString();
                    cb_漏电.Text = dtM1.Rows[0]["漏电"].ToString();

                    tb10.Text = dtM1.Rows[0]["客户"].ToString();
                    cb9.Text = dtM1.Rows[0]["计量单位"].ToString();
                    tb9.Text = dtM1.Rows[0]["标准单价"].ToString();
                    tb15.Text = dtM1.Rows[0]["库存上限"].ToString();
                    tb16.Text = dtM1.Rows[0]["库存下限"].ToString();
                    tb8.Text = dtM1.Rows[0]["克重"].ToString();
                    cb11_环保.Text = dtM1.Rows[0]["环保"].ToString();
                    cb_ESD等级.Text = dtM1.Rows[0]["ESD等级"].ToString();
                    tb14.Text = dtM1.Rows[0]["库位编号"].ToString();
                    tb17.Text = dtM1.Rows[0]["库位描述"].ToString();
                    cb2.Text = dtM1.Rows[0]["物料来源"].ToString();
                    tb11.Text = dtM1.Rows[0]["采购周期"].ToString();
                    txt_默认供应商.Text = dtM1.Rows[0]["默认供应商"].ToString();
                    //tb13.Text = dtM1.Rows[0]["标签打印"].ToString();
                    checkBox10.Checked = Convert.ToBoolean(dtM1.Rows[0]["标签打印"]);
                    tb12.Text = dtM1.Rows[0]["最小包装"].ToString();
                    cb12.Text = dtM1.Rows[0]["主辅料"].ToString();
                    //cb_stock.Text = dtM1.Rows[0]["仓库号"].ToString();
                    txt_货架编号.Text = dtM1.Rows[0]["货架编号"].ToString();
                    txt_货架描述.Text = dtM1.Rows[0]["货架描述"].ToString();
                    checkBox9.Checked = Convert.ToBoolean(dtM1.Rows[0]["新数据"]);     //
                    teshubeizhu.Text = dtM1.Rows[0]["特殊备注"].ToString();     //
                    wuliaobeizhu.Text = dtM1.Rows[0]["物料备注"].ToString();     //
                    yuanguigexinghao.Text = dtM1.Rows[0]["原规格型号"].ToString();     //
                    xilei.Text = dtM1.Rows[0]["细类"].ToString();     //
                    xiaoshoudanjia.Text = dtM1.Rows[0]["n销售单价"].ToString();     //
                    hesuandanjia.Text = dtM1.Rows[0]["n核算单价"].ToString();     //
                    cb_仓库编号.Text = dtM1.Rows[0]["仓库号"].ToString();     //
                    cangkumiaoshu.Text = dtM1.Rows[0]["仓库名称"].ToString();     //
                    yuanERPguigexinghao.Text = dtM1.Rows[0]["n原ERP规格型号"].ToString();     //
                    xinghaozixiang.Text = dtM1.Rows[0]["型号子项"].ToString();
                    txt_车间.Text = dtM1.Rows[0]["车间"].ToString();
                    txt_工时.Text = dtM1.Rows[0]["工时"].ToString();
                    txt_负责人.Text = dtM1.Rows[0]["负责人"].ToString();
                    checkBox5.Checked = Convert.ToBoolean(dtM1.Rows[0]["停用"]);

                    checkBox6.Checked = Convert.ToBoolean(dtM1.Rows[0]["可售"]);
                    checkBox8.Checked = Convert.ToBoolean(dtM1.Rows[0]["可购"]);

                    checkBox4.Checked = Convert.ToBoolean(dtM1.Rows[0]["生效"]);
                    cb_供应商编号.EditValue = dtM1.Rows[0]["供应商编号"].ToString();
                    cb_车间编号.EditValue = dtM1.Rows[0]["车间编号"].ToString();
                    txt_图纸版本.Text = dtM1.Rows[0]["图纸版本"].ToString();
                    cb_物料属性.EditValue = dtM1.Rows[0]["物料属性"].ToString();
                    try
                    {
                        txt_物料状态.EditValue = dtM1.Rows[0]["物料状态"].ToString();
                        txt_更改预计完成时间.EditValue = dtM1.Rows[0]["更改预计完成时间"];
                    }
                    catch { }
                    txt_审核.Text = dtM1.Rows[0]["审核"].ToString();
                    strNo = 2;  //2表示修改状态
                }
                else
                {
                    strshow = "没有该数据！";
                    textBox1.Text = "";
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
                if (cb_车间编号.EditValue == null || cb_车间编号.EditValue.ToString() == "")
                {
                    cb_车间编号.EditValue = "";
                }
                if (tb3_物料名称.Text == "")
                {
                    strshow = "物料名称不能为空！"; tb3_物料名称.Focus();
                    return false;
                }
                if (tb4_规格型号.Text == "")
                {
                    strshow = "型号不能为空！"; tb4_规格型号.Focus();
                    return false;
                }
                if (tb6_物料类型.Text == "")
                {
                    strshow = "物料类型不能为空！"; tb6_物料类型.Focus();
                    return false;
                }
                if (cb3_产品线.EditValue.ToString() == "")
                {
                    //strshow = "请选择产品线！"; cb3_产品线.Focus();
                    //return false;
                }
                if (cb6_大类.EditValue.ToString() == "")
                {
                    strshow = "请选择大类！"; cb6_大类.Focus();
                    return false;
                }
                if (cb7_小类.EditValue.ToString() == "")
                {
                    strshow = "请选择小类！"; cb7_小类.Focus();
                    return false;
                }
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
                //if (cb_车间编号.EditValue.ToString() == "" || cb_车间编号.EditValue == null)
                //{
                //    cb_车间编号.EditValue = "";
                //}
                if (cb_物料属性.EditValue.ToString() == "" || cb_物料属性.EditValue == null)
                {
                    cb_物料属性.EditValue = "";
                }

            }
            catch (Exception ex)
            {
                strshow = ex.Message;
            }
            return true;
        }  //检查基础数据

        public void fun_清空数据()
        {

            cb_漏电.Text = "";
            cb_结构代码.Text = "";
            cb_分断能力.Text = "";
            cb_保护特性.Text = "";
            cb_断路器型号.Text = "";
            ;
            txt_物料编码.Text = "";
            tb2.Text = "";
            tb3_物料名称.Text = "";
            tb4_规格型号.Text = "";
            tb5.Text = "";
            tb6_物料类型.SelectedIndex = -1;
            txt_默认供应商.Text = "";
            tb8.Text = "";
            tb9.Text = "";
            tb10.Text = "";
            tb11.Text = "";
            tb12.Text = "";
            checkBox10.Checked = false;
            tb14.Text = "";
            tb15.Text = "";
            tb16.Text = "";
            tb17.Text = "";
            checkBox4.Checked = false;
            checkBox5.Checked = false;

            checkBox6.Checked = false;
            checkBox8.Checked = false;
            txt_审核.Text = "";
            cb1_物料等级.SelectedIndex = -1;
            cb2.SelectedIndex = -1;
            cb3_产品线.SelectedIndex = -1;
            cb4_规格.Text = "";
            cb5_电压.SelectedIndex = -1;
            cb6_大类.EditValue = "";
            cb7_小类.SelectedIndex = -1;
            cb8_极数.SelectedIndex = -1;
            cb9.SelectedIndex = -1;
            cb10_壳架等级.SelectedIndex = -1;
            cb11_环保.SelectedIndex = -1;
            cb_ESD等级.SelectedIndex = -1;
            cb12.SelectedIndex = -1;
            //cb_stock.SelectedIndex = -1;
            txt_货架编号.Text = "";
            txt_货架描述.Text = "";
            txt_工时.Text = "";
            txt_车间.Text = "";
            checkBox9.Checked = false;    //
            xinghaozixiang.Text = "";
            teshubeizhu.Text = "";      //
            wuliaobeizhu.Text = "";      //
            yuanguigexinghao.Text = "";      //
            xilei.Text = "";     //
            xiaoshoudanjia.Text = "";      //
            hesuandanjia.Text = "";      //
            cb_仓库编号.Text = "";  //
            cangkumiaoshu.Text = "";    //
            yuanERPguigexinghao.Text = "";     //
            txt_负责人.Text = "";
            txt_更改预计完成时间.EditValue = null;
            txt_物料状态.SelectedIndex = -1;
            cb_供应商编号.EditValue = null;
            cb_车间编号.EditValue = null;
            txt_图纸版本.Text = "";
            cb_物料属性.SelectedIndex = -1;
            //checkBox8.Checked = false;
        }  //清空基础属性数据

        public void fun_新增()
        {
            fun_清空数据();
            txt_物料编码.ReadOnly = false;
            strNo = 1;                       //1表示新增状态
            textBox1.Text = "";
            fun_xtra2();
            button4.Text = "预览";
        }

        public void fun_规格()
        {
            if (tb6_物料类型.EditValue != null && tb6_物料类型.EditValue.ToString() == "成品")
            {
                cb4_规格.Text = "";
                if (cb7_小类.Text.ToString() != "")
                {//电压
                    cb4_规格.Text = cb7_小类.Text.ToString() + "-";
                }
                cb4_规格.Text = cb4_规格.Text.ToString() + tb4_规格型号.Text.ToString();
                if (cb5_电压.Text.ToString() != "")
                {//电压
                    cb4_规格.Text = cb4_规格.Text.ToString() + "-" + cb5_电压.Text.ToString();
                }
                if (cb8_极数.Text.ToString() != "")
                {//极数
                    cb4_规格.Text = cb4_规格.Text.ToString() + "-" + cb8_极数.Text.ToString();
                }
                if (cb11_环保.Text.ToString() == "环保")
                {//环保
                    cb4_规格.Text = cb4_规格.Text.ToString() + "[H]";
                }
                cb4_规格.Text = cb4_规格.Text.ToString() + "." + xinghaozixiang.Text.ToString();
            }
        }

        public string fun_盒贴规格()
        {
            string str_盒贴规格 = "";
            str_盒贴规格 = cb7_小类.Text.ToString() + "-" + tb4_规格型号.Text.ToString();
            if (cb5_电压.Text.ToString() != "")
            {//电压
                str_盒贴规格 = str_盒贴规格 + "-" + cb5_电压.Text.ToString();
            }
            if (cb8_极数.Text.ToString() != "")
            {//极数
                str_盒贴规格 = str_盒贴规格 + "-" + cb8_极数.Text.ToString();
            }
            if (cb11_环保.Text.ToString() == "环保")
            {//环保
                str_盒贴规格 = str_盒贴规格 + "[H]";
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
                textBox1.Text = "";
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

        public void fun_基础属性保存()
        {
            try
            {
                if (strNo == 1 || strNo == 0)  //0为初始状态，1为新增状态
                {
                    str_新增or修改 = "新增";
                    fun_验证物料编号();
                    dtM.AcceptChanges();
                    DataRow dr = dtM.NewRow();
                    dr["物料编码"] = txt_物料编码.Text;
                    dr["物料编码"] = tb2.Text;
                    dr["物料名称"] = tb3_物料名称.Text;
                    dr["规格型号"] = tb4_规格型号.Text;
                    dr["图纸编号"] = tb5.Text;
                    dr["物料类型GUID"] = System.Guid.NewGuid().ToString();
                    dr["物料类型"] = tb6_物料类型.Text;
                    dr["产品线GUID"] = System.Guid.NewGuid().ToString();
                    dr["产品线"] = cb3_产品线.Text;
                    dr["大类GUID"] = System.Guid.NewGuid().ToString();
                    dr["大类"] = cb6_大类.EditValue.ToString();
                    dr["小类GUID"] = System.Guid.NewGuid().ToString();
                    dr["小类"] = cb7_小类.Text;
                    //    规格由几项拼装而成
                    if (cb3_产品线.EditValue != null && cb3_产品线.EditValue.ToString() == "智能终端电器")
                    {
                        fun_智能_规格();
                    }
                    else
                    {
                    fun_加1();
                    fun_规格();
                    }
                    dr["规格"] = cb4_规格.Text;
                    dr["物料等级"] = cb1_物料等级.Text;
                    dr["壳架等级"] = cb10_壳架等级.Text;
                    dr["极数"] = cb8_极数.Text;
                    dr["电压"] = cb5_电压.Text;

                    dr["细分功能结构代码"] = cb_结构代码.Text;
                    dr["分断能力"] = cb_分断能力.Text;
                    dr["保护特性"] = cb_保护特性.Text;
                    dr["断路器型号"] = cb_断路器型号.Text;
                    dr["漏电"] = cb_漏电.Text;


                    dr["客户"] = tb10.Text;
                    dr["计量单位"] = cb9.Text;
                    dr["标准单价"] = tb9.Text;
                    dr["库存上限"] = tb15.Text;
                    dr["库存下限"] = tb16.Text;
                    dr["克重"] = tb8.Text;
                    dr["环保"] = cb11_环保.Text;
                    dr["ESD等级"] = cb_ESD等级.Text;
                    dr["库位编号"] = tb14.Text;
                    dr["库位描述"] = tb17.Text;
                    dr["物料来源"] = cb2.Text;
                    dr["采购周期"] = tb11.Text;
                    dr["默认供应商"] = txt_默认供应商.Text;
                    dr["标签打印"] = checkBox10.Checked;
                    dr["最小包装"] = tb12.Text;
                    dr["主辅料"] = cb12.Text;
                    dr["停用"] = checkBox5.Checked;


                    dr["可售"] = checkBox6.Checked;
                    dr["可购"] = checkBox8.Checked;

                    dr["生效"] = checkBox4.Checked;
                    //dr["仓库号"] = cb_stock.Text;
                    dr["货架编号"] = txt_货架编号.Text;
                    dr["货架描述"] = txt_货架描述.Text;
                    dr["新数据"] = true;
                    dr["型号子项"] = xinghaozixiang.Text;
                    dr["特殊备注"] = teshubeizhu.Text;
                    dr["物料备注"] = wuliaobeizhu.Text;
                    dr["原规格型号"] = yuanguigexinghao.Text;
                    dr["细类"] = xilei.Text;
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
                        dr["审核日期"] = time;
                    }
                    dr["车间"] = txt_车间.Text;
                    dr["工时"] = txt_工时.Text;
                    dr["盒贴规格型号"] = fun_盒贴规格();
                    dr["负责人"] = txt_负责人.Text;
                    dr["修改人"] = CPublic.Var.localUserName;
                    dr["修改人ID"] = CPublic.Var.LocalUserID;
                    dr["修改日期"] = System.DateTime.Now;
                    dr["物料属性"] = cb_物料属性.EditValue;
                    if (dr["是否初始化"].ToString() != "是")
                    {
                        dr["是否初始化"] = "否";
                    }

                    try
                    {
                        dr["供应商编号"] = cb_供应商编号.EditValue;
                        dr["车间编号"] = cb_车间编号.EditValue;
                        dr["图纸版本"] = txt_图纸版本.Text;
                        dr["物料状态"] = txt_物料状态.EditValue;
                        dr["更改预计完成时间"] = txt_更改预计完成时间.EditValue;
                    }
                    catch { }
                    dtM.Rows.Add(dr);
                    new SqlCommandBuilder(da);
                    da.Update(dtM);
                    strshow = string.Format("物料编码为{0}的基础数据新增成功！", txt_物料编码.Text);
                    strNo = 2;  //新增后可以立即修改
                    txt_物料编码.ReadOnly = true;

                    string sql = string.Format("select 物料编码,是否初始化 from 基础数据物料信息表 where 物料编码 = '{0}'", dr["物料编码"]);
                    using (SqlDataAdapter daa = new SqlDataAdapter(sql, strconn))
                    {
                        DataTable dtt = new DataTable();
                        daa.Fill(dtt);
                        if (dtt.Rows[0]["是否初始化"].ToString() != "是")
                        {
                            dtt.Rows[0]["是否初始化"] = "是";

                            string a = "";
                            string b = dr["物料编码"].ToString();
                            decimal c = 0;
                            //StockCore.StockCorer.fun_Init初始化仓库物料(a, b, c);
                        }

                        new SqlCommandBuilder(daa);
                        daa.Update(dtt);
                    }
                }
                else if (strNo == 2)  //2为修改状态
                {
                    str_新增or修改 = "修改";
                    dt_保存修改 = new DataTable();
                    //string sql_保存修改 = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", tb2.Text);
                    string sql_保存修改 = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", txt_物料编码.Text);

                    SqlDataAdapter da_保存修改 = new SqlDataAdapter(sql_保存修改, strconn);
                    da_保存修改.Fill(dt_保存修改);
                    dt_保存修改.Rows[0]["物料编码"] = txt_物料编码.Text;
                    dt_保存修改.Rows[0]["物料编码"] = tb2.Text;
                    dt_保存修改.Rows[0]["物料名称"] = tb3_物料名称.Text;
                    dt_保存修改.Rows[0]["规格型号"] = tb4_规格型号.Text;
                    dt_保存修改.Rows[0]["图纸编号"] = tb5.Text;
                    dt_保存修改.Rows[0]["物料类型"] = tb6_物料类型.Text;
                    dt_保存修改.Rows[0]["产品线"] = cb3_产品线.Text;
                    dt_保存修改.Rows[0]["大类"] = cb6_大类.EditValue.ToString();
                    dt_保存修改.Rows[0]["小类"] = cb7_小类.Text;
                    //  规格型号
                    if (cb3_产品线.EditValue != null && cb3_产品线.EditValue.ToString() == "智能终端电器")
                    {
                        fun_智能_规格();
                    }
                    else
                    {
                        fun_规格();
                    }
                    dt_保存修改.Rows[0]["规格"] = cb4_规格.Text;
                    dt_保存修改.Rows[0]["物料等级"] = cb1_物料等级.Text;
                    dt_保存修改.Rows[0]["壳架等级"] = cb10_壳架等级.Text;
                    dt_保存修改.Rows[0]["极数"] = cb8_极数.Text;
                    dt_保存修改.Rows[0]["电压"] = cb5_电压.Text;

                    dt_保存修改.Rows[0]["细分功能结构代码"] = cb_结构代码.Text;
                    dt_保存修改.Rows[0]["分断能力"] = cb_分断能力.Text;
                    dt_保存修改.Rows[0]["保护特性"] = cb_保护特性.Text;
                    dt_保存修改.Rows[0]["断路器型号"] = cb_断路器型号.Text;
                    dt_保存修改.Rows[0]["漏电"] = cb_漏电.Text;

                    dt_保存修改.Rows[0]["客户"] = tb10.Text;
                    dt_保存修改.Rows[0]["计量单位"] = cb9.Text;
                    dt_保存修改.Rows[0]["标准单价"] = tb9.Text;
                    dt_保存修改.Rows[0]["库存上限"] = tb15.Text;
                    dt_保存修改.Rows[0]["库存下限"] = tb16.Text;
                    dt_保存修改.Rows[0]["克重"] = tb8.Text;
                    dt_保存修改.Rows[0]["环保"] = cb11_环保.Text;
                    dt_保存修改.Rows[0]["ESD等级"] = cb_ESD等级.Text;
                    dt_保存修改.Rows[0]["库位编号"] = tb14.Text;
                    dt_保存修改.Rows[0]["库位描述"] = tb17.Text;
                    dt_保存修改.Rows[0]["物料来源"] = cb2.Text;
                    dt_保存修改.Rows[0]["采购周期"] = tb11.Text;
                    dt_保存修改.Rows[0]["默认供应商"] = txt_默认供应商.Text;
                    dt_保存修改.Rows[0]["标签打印"] = checkBox10.Checked;
                    dt_保存修改.Rows[0]["最小包装"] = tb12.Text;
                    dt_保存修改.Rows[0]["主辅料"] = cb12.Text;
                    dt_保存修改.Rows[0]["停用"] = checkBox5.Checked;

                    dt_保存修改.Rows[0]["可售"] = checkBox6.Checked;
                    dt_保存修改.Rows[0]["可购"] = checkBox8.Checked;

                    dt_保存修改.Rows[0]["生效"] = checkBox4.Checked;
                    //dt_保存修改.Rows[0]["仓库号"] = cb_stock.Text; txt_货架编号
                    dt_保存修改.Rows[0]["货架编号"] = txt_货架编号.Text;
                    dt_保存修改.Rows[0]["货架描述"] = txt_货架描述.Text;
                    dt_保存修改.Rows[0]["新数据"] = true;
                    dt_保存修改.Rows[0]["型号子项"] = xinghaozixiang.Text;
                    dt_保存修改.Rows[0]["特殊备注"] = teshubeizhu.Text;
                    dt_保存修改.Rows[0]["物料备注"] = wuliaobeizhu.Text;
                    dt_保存修改.Rows[0]["原规格型号"] = yuanguigexinghao.Text;
                    dt_保存修改.Rows[0]["细类"] = xilei.Text;
                    dt_保存修改.Rows[0]["n销售单价"] = xiaoshoudanjia.Text;
                    dt_保存修改.Rows[0]["n核算单价"] = hesuandanjia.Text;
                    dt_保存修改.Rows[0]["仓库号"] = cb_仓库编号.Text;
                    dt_保存修改.Rows[0]["仓库名称"] = cangkumiaoshu.Text;
                    dt_保存修改.Rows[0]["n原ERP规格型号"] = yuanERPguigexinghao.Text;
                    dt_保存修改.Rows[0]["审核"] = txt_审核.Text;
                    if (str_id != "")
                    {
                        dt_保存修改.Rows[0]["审核人ID"] = str_id;
                        dt_保存修改.Rows[0]["审核人"] = str_name;
                        dt_保存修改.Rows[0]["审核日期"] = time;
                    }
                    dt_保存修改.Rows[0]["工时"] = txt_工时.Text;
                    dt_保存修改.Rows[0]["车间"] = txt_车间.Text;
                    dt_保存修改.Rows[0]["盒贴规格型号"] = fun_盒贴规格();
                    dt_保存修改.Rows[0]["负责人"] = txt_负责人.Text;
                    dt_保存修改.Rows[0]["修改人"] = CPublic.Var.localUserName;
                    dt_保存修改.Rows[0]["修改人ID"] = CPublic.Var.LocalUserID;
                    dt_保存修改.Rows[0]["修改日期"] = System.DateTime.Now;
                    dt_保存修改.Rows[0]["物料属性"] = cb_物料属性.EditValue;
                    try
                    {
                        dt_保存修改.Rows[0]["供应商编号"] = cb_供应商编号.EditValue;
                        dt_保存修改.Rows[0]["车间编号"] = cb_车间编号.EditValue;
                        dt_保存修改.Rows[0]["图纸版本"] = txt_图纸版本.Text.ToString();
                        dt_保存修改.Rows[0]["物料状态"] = txt_物料状态.EditValue.ToString();
                        dt_保存修改.Rows[0]["更改预计完成时间"] = txt_更改预计完成时间.EditValue;
                    }
                    catch { }
                    new SqlCommandBuilder(da_保存修改);
                    da_保存修改.Update(dt_保存修改);
                    strshow = string.Format("物料编码为{0}的基础数据修改成功！", txt_物料编码.Text);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void fun_扩展属性保存()
        {
            //每次都先删，在增
            //string str = "select * from 基础数据物料信息扩展表 where 1<> 1";
            //DataTable dt = new DataTable();
            //SqlDataAdapter da = new SqlDataAdapter(str, strconn);
            //da.Fill(dt);

            //string strdelete = string.Format("delete from 基础数据物料信息扩展表 where 物料编码 = '{0}'", tb1.Text);
            //SqlConnection conn = new SqlConnection(strconn);
            //conn.Open();
            //SqlCommand cmd = new SqlCommand(strdelete, conn);
            //cmd.ExecuteNonQuery();
            //conn.Close();
            //cmd.Dispose();

            //foreach (DataRow dr1 in dt1.Rows)
            //{
            //    DataRow dr = dt.NewRow();
            //    dr["物料编码"] = tb1.Text;
            //    dr["物料属性"] = dr1["物料信息扩展属性"].ToString();
            //    dr["属性值"] = dr1["属性值"].ToString();
            //    dr["POS"] = dr1["POS"];
            //    dt.Rows.Add(dr);
            //}
            //new SqlCommandBuilder(da);
            //this.BindingContext[dt].EndCurrentEdit();
            //da.Update(dt);
            //strshow = "扩展属性保存成功!";          
        }

        public void fun_xtra生效选择()
        {
            if (checkBox1.Checked == false)
            {
                if (checkBox2.Checked == false)
                {
                    if (checkBox3.Checked == false)
                    {
                        if (checkBox7.Checked == false)
                        {

                        }
                        else
                        {
                            checkBox1.Checked = false;
                            checkBox2.Checked = false;
                            checkBox3.Checked = false;
                        }
                    }
                    else
                    {
                        checkBox1.Checked = false;
                        checkBox2.Checked = false;
                        checkBox7.Checked = false;
                    }
                }
                else
                {
                    checkBox1.Checked = false;
                    checkBox3.Checked = false;
                    checkBox7.Checked = false;
                }
            }
            else
            {
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox7.Checked = false;
            }

        }

        /// <summary>
        /// 自动实现子项 +1
        /// </summary>
        public void fun_加1()
        {
            try
            {// and 特殊备注 = '{5}'    , teshubeizhu.Text.ToString()
                string str2 = string.Format("小类 = '{0}' and 规格型号 = '{1}' and 电压 = '{2}' and 极数 = '{3}' and 环保 = '{4}'", cb7_小类.Text.ToString(), tb4_规格型号.Text.ToString(), cb5_电压.Text.ToString(), cb8_极数.Text.ToString(), cb11_环保.Text.ToString());
                string str3 = string.Format("小类 = '{0}' and 规格型号 = '{1}' and 电压 = '{2}' and 极数 = '{3}' and 环保 = '{4}' and 特殊备注 = '{5}'",
                    cb7_小类.Text.ToString(), tb4_规格型号.Text.ToString(), cb5_电压.Text.ToString(), cb8_极数.Text.ToString(), cb11_环保.Text.ToString(), teshubeizhu.Text.ToString());
                string str = string.Format("select * from 基础数据物料信息表 where {0}", str2);
                SqlDataAdapter da = new SqlDataAdapter(str, strconn);
                DataTable t = new DataTable();
                da.Fill(t);
                int s_暂时 = 0;
                //获取 型号子项 最大值
                foreach (DataRow r in t.Rows)
                {
                    if (Convert.ToInt32(r["型号子项"]) > s_暂时)
                    {
                        s_暂时 = Convert.ToInt32(r["型号子项"]);
                    }
                }
                //判断本次是否需要 +1
                string str_1 = string.Format("select * from 基础数据物料信息表 where {0}", str3);
                SqlDataAdapter da2 = new SqlDataAdapter(str_1, strconn);
                DataTable tt = new DataTable();
                da2.Fill(tt);
                if (tt.Rows.Count <= 0)
                {
                    xinghaozixiang.Text = (s_暂时 + 1).ToString();
                }
                if (Convert.ToInt32(tt.Rows[0]["型号子项"]) <= 0)
                {
                    xinghaozixiang.Text = (s_暂时 + 1).ToString();
                }
            }
            catch { }
        }

        /// <summary>
        /// 基础数据界面:载入数据；dv：显示新数据
        /// </summary>
        public void fun_载入数据()
        {
            dtM = new DataTable();
            //string sql = "select * from 基础数据物料信息表";
            string sql = @"select * from 基础数据物料信息表";  //以后只显示审核过的数据 7.28
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);

            //当修改过数据保存后，新数据自动消失，点全部时，查看所有数据 //用于显示旧数据 7.28
            dv = new DataView(dtM);
            dv.RowFilter = "新数据 = 0";

            gcc.DataSource = dv;

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
            str_智能规格 = cb7_小类.EditValue.ToString().Trim() + "-" + cb_功能类别.Text.ToString().Trim() + cb_结构代码.Text.ToString().Trim() + cb10_壳架等级.EditValue.ToString().Trim()
            + cb_分断能力.Text.ToString().Trim() + "_" + cb_保护特性.Text.ToString().Trim() + cb_额定电流.Text.ToString().Trim() + "/" + cb8_极数.EditValue.ToString().Trim() + cb_断路器型号.Text.ToString().Trim();
            if (cb_漏电.Text != null && cb_漏电.Text.ToString() != "")
            {
                str_智能规格 = str_智能规格 + "-" + cb_漏电.Text.ToString();
                if (cb5_电压.Text != null && cb5_电压.Text.ToString() != "")
                {
                    str_智能规格 = str_智能规格 + "_" + cb5_电压.EditValue.ToString().Trim();
                }
            }
            cb4_规格.Text = str_智能规格;


        }
        private void fun_物料编码()
        {
            txt_物料编码.Text = cb3_产品线.Text + cb6_大类.EditValue.ToString() + "0" + tb2.Text;
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
            if (tb5.Text != null && tb5.Text != "" && yuanERPguigexinghao.Text != null && yuanERPguigexinghao.Text != "")
            {
                str = tb5.Text + "' and n原ERP规格型号 = '" + yuanERPguigexinghao.Text + "'";
                sql = string.Format("select * from 基础数据物料信息表 where 图纸编号 = '{0}", str);
                dt = new DataTable();
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    throw new Exception("请检查'图纸编号'和'n原ERP规格型号'是否重复！");
                }
            }
        }
        #endregion

        #region 主界面和所有物料界面
        string str_name = "";
        string str_id = "";
        DateTime time;
        //审核
        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (txt_审核.Text == "待审核" && txt_审核.Text != "")
            {
                txt_审核.Text = "已审核";
                str_name = CPublic.Var.localUserName;
                str_id = CPublic.Var.LocalUserID;
                time = System.DateTime.Now;
            }
            else
            {
                txt_审核.Text = "待审核";
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //fun_载入刷新();
            if (txt_物料编码.Text != "")
            {
                string sql = string.Format("select  * from 基础数据物料信息表 where 物料编码='{0}'", txt_物料编码.Text);
                DataTable dt = new DataTable();
                dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                DataRow[] r_1 = dtM.Select(string.Format("物料编码='{0}'", txt_物料编码.Text));
                r_1[0].ItemArray = dt.Rows[0].ItemArray;
            }
            //checkBox11.Checked = false;
            cb7_小类.Properties.Items.Clear();
            fun_新增();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_新增();
            cb7_小类.Properties.Items.Clear();
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //DataRow dr = dtM.NewRow();
            //try
            //{
            //    gv.CloseEditor();
            //    if (txt_物料编码.Text == "")
            //    {
            //        MessageBox.Show("请先输入物料编码！");
            //    }
            //    else
            //    {
            //        if (cangkumiaoshu.Text == "成品库")
            //        {
            //            if (!fun_check())
            //            {
            //                MessageBox.Show(strshow);
            //            }
            //            else
            //            {
            //                fun_基础属性保存();
            //                MessageBox.Show(strshow);
            //            }
            //        }
            //        else
            //        {
            //            if (tb8.Text == "")
            //            {
            //                tb8.Text = Convert.ToString(0);
            //            }
            //            if (tb11.Text == "")
            //            {
            //                tb11.Text = Convert.ToString(0);
            //            }
            //            if (tb12.Text == "")
            //            {
            //                tb12.Text = Convert.ToString(0);
            //            }
            //            if (tb15.Text == "")
            //            {
            //                tb15.Text = Convert.ToString(0);
            //            }
            //            if (tb16.Text == "")
            //            {
            //                tb16.Text = Convert.ToString(0);
            //            }
            //            if (xiaoshoudanjia.Text == "")
            //            {
            //                xiaoshoudanjia.Text = "0";
            //            }
            //            if (hesuandanjia.Text == "")
            //            {
            //                hesuandanjia.Text = "0";
            //            }
            //            if (tb9.Text == "")
            //            {
            //                tb9.Text = "0";
            //            }
            //            if (xinghaozixiang.Text == "")
            //            {
            //                xinghaozixiang.Text = "0";
            //            }
            //            if (txt_工时.Text == "")
            //            {
            //                txt_工时.Text = "0";
            //            }
            //            if (cb_车间编号.EditValue.ToString() == "" || cb_车间编号.EditValue == null)
            //            {
            //                cb_车间编号.EditValue = "";
            //            }
            //            if (cb_物料属性.EditValue.ToString() == "" || cb_物料属性.EditValue == null)
            //            {
            //                cb_物料属性.EditValue = "";
            //            }
            //            fun_基础属性保存();
            //            MessageBox.Show(strshow);
            //        }
            //    }
            //    button4.Text = "预览";
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            //try
            //{
            //    string sql = "select * from 基础数据物料信息修改日志表 where 1<>1";
            //    DataTable dtttt = new DataTable();
            //    SqlDataAdapter daaaa = new SqlDataAdapter(sql, strconn);
            //    daaaa.Fill(dtttt);
            //    new SqlCommandBuilder(daaaa);
            //    DataRow drrrr = dtttt.NewRow();
            //    dtttt.Rows.Add(drrrr);
            //    if (str_新增or修改 == "修改")
            //    {
            //        DataRow[] ds = dtM.Select(string.Format("物料编码 = '{0}'", txt_物料编码.Text));
            //        int i = ds.Length;
            //        dr.ItemArray = ds[0].ItemArray;

            //        string str_修改内容 = "修改了：";
            //        //DataTable dt_保存修改
            //        foreach (DataColumn dc in dt_保存修改.Columns)
            //        {
            //            string str1 = dt_保存修改.Rows[0][dc.Caption].ToString();
            //            string str2 = dr[dc.Caption].ToString();
            //            if (str1 != str2)
            //            {
            //                str_修改内容 = str_修改内容 + dc.Caption + "的值，" + "原：" + str2 + "，现：" + str1 + "；";
            //            }
            //        }
            //        drrrr["GUID"] = System.Guid.NewGuid();
            //        drrrr["姓名"] = CPublic.Var.localUserName;
            //        drrrr["员工号"] = CPublic.Var.LocalUserID;
            //        drrrr["日期"] = System.DateTime.Now;
            //        drrrr["内容"] = str_修改内容;
            //        drrrr["物料编码"] = txt_物料编码.Text;
            //        //MessageBox.Show(str_修改内容);
            //    }
            //    else
            //    {
            //        drrrr["GUID"] = System.Guid.NewGuid();
            //        drrrr["姓名"] = CPublic.Var.localUserName;
            //        drrrr["员工号"] = CPublic.Var.LocalUserID;
            //        drrrr["日期"] = System.DateTime.Now;
            //        drrrr["内容"] = "新增物料：" + txt_物料编码.Text;
            //        drrrr["物料编码"] = txt_物料编码.Text;
            //    }
            //    daaaa.Update(dtttt);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            //if (txt_物料编码.Text != "")
            //{
            //    string sql = string.Format("select  * from 基础数据物料信息表 where 物料编码='{0}'", txt_物料编码.Text);
            //    DataTable dt = new DataTable();
            //    dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            //    DataRow[] r_1 = dtM.Select(string.Format("物料编码='{0}'", txt_物料编码.Text));
            //    r_1[0].ItemArray = dt.Rows[0].ItemArray;
            //}
        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "") { }
            else
            {
                fun_查询();
                xtra.SelectedTabPage = xtraTabPage5;
                if (a_查询时使用 > 0) { }
                else
                {
                    MessageBox.Show(strshow);
                }
            }
            str_物料编码 = textBox1.Text;
            str_物料名称 = tb3_物料名称.Text;
            str_规格 = cb4_规格.Text;
            str_原规格型号 = yuanERPguigexinghao.Text;
            aaaa.fun_(str_物料编码, str_物料名称, str_规格, str_原规格型号);
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //生效
            if (a2 == 1)
            {
                checkBox4.Checked = true;
                a2 = 2;
            }
            else if (a2 == 2)
            {
                checkBox4.Checked = false;
                a2 = 1;
            }
        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //停用            
            if (a1 == 1)
            {
                checkBox5.Checked = true;
                a1 = 2;
            }
            else if (a1 == 2)
            {
                checkBox5.Checked = false;
                a1 = 1;
            }
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox1.Text == "") { }
                else
                {
                    fun_查询(); xtra.SelectedTabPage = xtraTabPage5;
                    if (a_查询时使用 > 0) { }
                    else
                    {
                        MessageBox.Show(strshow);
                    }
                    str_物料编码 = textBox1.Text;

                    aaaa.fun_(str_物料编码, str_物料名称, str_规格, str_原规格型号);
                }
            }
        }

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
                gcc.DataSource = dv;   //用于显示旧数据
                //gcc.DataSource = dt;
            }
        }

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
            if (checkBox15.Checked == true)
            {
                if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
                {
                    DataRow rrr = gvv.GetDataRow(e.RowHandle);
                    //rrr = (this.BindingContext[gvv.DataSource].Current as DataRowView).Row;
                    //DataRow[] ds = dtM.Select(string.Format("物料编码 = '{0}'", dr["物料编码"].ToString()));
                    txt_物料编码.Text = rrr["物料编码"].ToString();
                    txt_物料编码.ReadOnly = true;
                    tb2.Text = rrr["物料编码"].ToString();
                    tb3_物料名称.Text = rrr["物料名称"].ToString();
                    tb4_规格型号.Text = rrr["规格型号"].ToString();
                    tb5.Text = rrr["图纸编号"].ToString();
                    tb6_物料类型.Text = rrr["物料类型"].ToString();
                    cb3_产品线.Text = rrr["产品线"].ToString();
                    cb6_大类.EditValue = rrr["大类"].ToString();
                    cb7_小类.Text = rrr["小类"].ToString();
                    cb4_规格.Text = rrr["规格"].ToString();
                    cb1_物料等级.Text = rrr["物料等级"].ToString();
                    cb10_壳架等级.Text = rrr["壳架等级"].ToString();
                    cb8_极数.Text = rrr["极数"].ToString();
                    cb5_电压.Text = rrr["电压"].ToString();

                    cb_结构代码.Text = rrr["细分功能结构代码"].ToString();
                    cb_分断能力.Text = rrr["分断能力"].ToString();
                    cb_保护特性.Text = rrr["保护特性"].ToString();
                    cb_断路器型号.Text = rrr["断路器型号"].ToString();
                    cb_漏电.Text = rrr["漏电"].ToString();

                    tb10.Text = rrr["客户"].ToString();
                    cb9.Text = rrr["计量单位"].ToString();
                    tb9.Text = rrr["标准单价"].ToString();
                    tb15.Text = rrr["库存上限"].ToString();
                    tb16.Text = rrr["库存下限"].ToString();
                    tb8.Text = rrr["克重"].ToString();
                    cb11_环保.Text = rrr["环保"].ToString();
                    cb_ESD等级.Text = rrr["ESD等级"].ToString();
                    tb14.Text = rrr["库位编号"].ToString();
                    tb17.Text = rrr["库位描述"].ToString();
                    cb2.Text = rrr["物料来源"].ToString();
                    tb11.Text = rrr["采购周期"].ToString();
                    txt_默认供应商.Text = rrr["默认供应商"].ToString();
                    checkBox10.Checked = Convert.ToBoolean(rrr["标签打印"]);
                    tb12.Text = rrr["最小包装"].ToString();
                    cb12.Text = rrr["主辅料"].ToString();
                    txt_货架编号.Text = rrr["货架编号"].ToString();
                    txt_货架描述.Text = rrr["货架描述"].ToString();
                    txt_车间.Text = rrr["车间"].ToString();
                    txt_工时.Text = rrr["工时"].ToString();
                    checkBox9.Checked = Convert.ToBoolean(rrr["新数据"]);      //
                    teshubeizhu.Text = rrr["特殊备注"].ToString();
                    xinghaozixiang.Text = rrr["型号子项"].ToString();//
                    wuliaobeizhu.Text = rrr["物料备注"].ToString();     //
                    yuanguigexinghao.Text = rrr["原规格型号"].ToString();     //
                    xilei.Text = rrr["细类"].ToString();     //
                    xiaoshoudanjia.Text = rrr["n销售单价"].ToString();     //
                    hesuandanjia.Text = rrr["n核算单价"].ToString();     //
                    cb_仓库编号.Text = rrr["仓库号"].ToString();     //
                    cangkumiaoshu.Text = rrr["仓库名称"].ToString();     //
                    yuanERPguigexinghao.Text = rrr["n原ERP规格型号"].ToString();
                    checkBox5.Checked = Convert.ToBoolean(rrr["停用"]);


                    checkBox6.Checked = Convert.ToBoolean(rrr["可售"]);
                    checkBox8.Checked = Convert.ToBoolean(rrr["可购"]);

                    checkBox4.Checked = Convert.ToBoolean(rrr["生效"]);
                    txt_负责人.Text = rrr["负责人"].ToString();
                    txt_更改预计完成时间.EditValue = rrr["更改预计完成时间"];
                    txt_物料状态.EditValue = rrr["物料状态"].ToString();
                    cb_供应商编号.EditValue = rrr["供应商编号"].ToString();
                    cb_车间编号.EditValue = rrr["车间编号"].ToString();
                    txt_图纸版本.Text = rrr["图纸版本"].ToString();
                    cb_物料属性.EditValue = rrr["物料属性"].ToString();
                    txt_审核.Text = rrr["审核"].ToString();
                    strNo = 2;
                }
            }
            //判断右键菜单是否可用
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gcc, new Point(e.X, e.Y));
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow r = gvv.GetDataRow(gvv.FocusedRowHandle);
                DataRow[] ds = dtM.Select(string.Format("物料编码 = '{0}'", r["物料编码"].ToString()));
                DataRow dr = dtM.NewRow();
                dr.ItemArray = ds[0].ItemArray;
                tb2.Text = dr["物料编码"].ToString();
                tb3_物料名称.Text = dr["物料名称"].ToString();
                tb4_规格型号.Text = dr["规格型号"].ToString();
                tb5.Text = dr["图纸编号"].ToString();
                tb6_物料类型.Text = dr["物料类型"].ToString();
                cb3_产品线.Text = dr["产品线"].ToString();
                cb6_大类.EditValue = dr["大类"].ToString();
                cb7_小类.Text = dr["小类"].ToString();
                cb4_规格.Text = dr["规格"].ToString();
                cb1_物料等级.Text = dr["物料等级"].ToString();
                cb10_壳架等级.Text = dr["壳架等级"].ToString();
                cb8_极数.Text = dr["极数"].ToString();
                cb5_电压.Text = dr["电压"].ToString();

                cb_结构代码.Text = dr["细分功能结构代码"].ToString();
                cb_分断能力.Text = dr["分断能力"].ToString();
                cb_保护特性.Text = dr["保护特性"].ToString();
                cb_断路器型号.Text = dr["断路器型号"].ToString();
                cb_漏电.Text = dr["漏电"].ToString();


                tb10.Text = dr["客户"].ToString();
                cb9.Text = dr["计量单位"].ToString();
                tb9.Text = dr["标准单价"].ToString();
                tb15.Text = dr["库存上限"].ToString();
                tb16.Text = dr["库存下限"].ToString();
                tb8.Text = dr["克重"].ToString();
                cb11_环保.Text = dr["环保"].ToString();
                cb_ESD等级.Text = dr["ESD等级"].ToString();
                tb14.Text = dr["库位编号"].ToString();
                tb17.Text = dr["库位描述"].ToString();
                cb2.Text = dr["物料来源"].ToString();
                tb11.Text = dr["采购周期"].ToString();
                txt_默认供应商.Text = dr["默认供应商"].ToString();
                checkBox10.Checked = Convert.ToBoolean(dr["标签打印"]);
                tb12.Text = dr["最小包装"].ToString();
                cb12.Text = dr["主辅料"].ToString();
                checkBox5.Checked = Convert.ToBoolean(dr["停用"]);


                checkBox6.Checked = Convert.ToBoolean(dr["可售"]);
                checkBox8.Checked = Convert.ToBoolean(dr["可购"]);

                checkBox4.Checked = Convert.ToBoolean(dr["生效"]);
                txt_货架编号.Text = dr["货架编号"].ToString();
                txt_货架描述.Text = dr["货架描述"].ToString();
                txt_车间.Text = dr["车间"].ToString();
                txt_工时.Text = dr["工时"].ToString();
                checkBox9.Checked = Convert.ToBoolean(dr["新数据"]);      //
                teshubeizhu.Text = dr["特殊备注"].ToString();
                xinghaozixiang.Text = dr["型号子项"].ToString();//
                wuliaobeizhu.Text = dr["物料备注"].ToString();     //
                yuanguigexinghao.Text = dr["原规格型号"].ToString();     //
                xilei.Text = dr["细类"].ToString();     //
                xiaoshoudanjia.Text = dr["n销售单价"].ToString();     //
                hesuandanjia.Text = dr["n核算单价"].ToString();     //
                cb_仓库编号.Text = dr["仓库号"].ToString();     //
                cangkumiaoshu.Text = dr["仓库名称"].ToString();     //
                yuanERPguigexinghao.Text = dr["n原ERP规格型号"].ToString();
                txt_负责人.Text = dr["负责人"].ToString();
                txt_更改预计完成时间.EditValue = dr["更改预计完成时间"];
                txt_物料状态.EditValue = dr["物料状态"].ToString();
                cb_供应商编号.EditValue = dr["供应商编号"].ToString();
                cb_车间编号.EditValue = dr["车间编号"].ToString();
                txt_图纸版本.Text = dr["图纸版本"].ToString();
                cb_物料属性.EditValue = dr["物料属性"].ToString();
                txt_审核.Text = dr["审核"].ToString();
                //strNo = 2;
            }
            catch
            {
                //标签打印
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (button4.Text == "预览")
            {
                if (cb3_产品线.EditValue != null && cb3_产品线.EditValue.ToString() == "智能终端电器")
                {
                    fun_智能_规格();

                }
                else
                {
                fun_加1();
                fun_规格();
                }
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

        private void cb6_大类_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string sqll = string.Format("select 物料类型GUID from 基础数据物料类型表 where 物料类型名称 = '{0}'", cb6_大类.EditValue.ToString());
                DataTable dt = new DataTable();
                SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn);
                daa.Fill(dt);

                string sql = string.Format("select 物料类型名称 from 基础数据物料类型表 where 上级类型GUID = '{0}' order by 物料类型名称", dt.Rows[0]["物料类型GUID"].ToString());
                DataTable dt_大小类 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_大小类);
                cb7_小类.Properties.Items.Clear();
                foreach (DataRow r in dt_大小类.Rows)
                {
                    cb7_小类.Properties.Items.Add(r["物料类型名称"].ToString());
                }
            }
            catch { }
        }

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
            }
            catch { }
        }

        private void cb_车间编号_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_车间编号.EditValue != null)
                {
                    DataRow[] ds = dt_车间.Select(string.Format("部门编号 = '{0}'", cb_车间编号.EditValue.ToString()));
                    if (ds.Length > 0)
                    {
                        txt_车间.Text = ds[0]["部门名称"].ToString();
                    }
                }
            }
            catch { }
        }

        private void tb6_物料类型_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (tb6_物料类型.EditValue != null)
                {
                    //当不是成品时，不自动生成规格，并且规格可以编辑
                    if (tb6_物料类型.EditValue.ToString() != "" && tb6_物料类型.EditValue.ToString() != "成品")
                    {
                        cb4_规格.ReadOnly = false;
                        button4.Visible = false;
                    }
                    else   //成品
                    {
                        cb4_规格.ReadOnly = true;
                        button4.Visible = true;

                        checkBox6.Checked = true;  //可售
                        checkBox8.Checked = false;
                    }
                    if (tb6_物料类型.EditValue.ToString() == "原材料")
                    {
                        checkBox6.Checked = false;

                        checkBox8.Checked = true;  //可购

                    }

                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + "tb6_物料类型_EditValueChanged");
            }
        }

        private void cb_仓库编号_EditValueChanged(object sender, EventArgs e)
        {
            if (cb_仓库编号.EditValue != null && cb_仓库编号.EditValue.ToString() != "")
            {
                //选完仓库编号后，显示仓库名称
                DataRow[] ds = dt_属性.Select(string.Format("属性字段1 = '{0}'", cb_仓库编号.EditValue.ToString()));
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




    }
}
