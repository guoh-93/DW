using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Data.SqlClient;

namespace ERPproduct
{


#pragma warning disable IDE1006 // 命名样式
    public partial class ui返修打印 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {
        string strcon = CPublic.Var.strConn;
        DataTable dt_物料;
        DataTable dt_模板名称;
        DataTable dt_客户;
        string para_物料 = "";
        DataRow dr_para = null;
        /// <summary>
        /// 标记 是否正在打印过程中 针对箱贴 
        /// </summary>
        bool flag = false;
        int boxcount = 0; //盒装数量 
        int makecount = 0;
        int i_箱装 = 0;
        string qsm;//起始码
        string printer_箱贴 = "";
        int count; //总箱数
        int xc = 1; // 箱次

        int ys;//余数
        int qs_序列;
        public ui返修打印()
        {
            InitializeComponent();
        }
        public ui返修打印(DataRow dr_c)
        {
            InitializeComponent();
            dr_para = dr_c;
            para_物料 = dr_c["物料编码"].ToString();
            //textBox6.Text = dr_para["生产工单号"].ToString().Trim();

        }


#pragma warning disable IDE1006 // 命名样式
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format(@"select 物料编码,物料名称,规格型号,存货分类 from 基础数据物料信息表   where 自制=1");

            dt_物料 = new DataTable();
            dt_物料 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            searchLookUpEdit1.Properties.DataSource = dt_物料;
            searchLookUpEdit1.Properties.ValueMember = "物料编码";
            searchLookUpEdit1.Properties.DisplayMember = "物料编码";

            searchLookUpEdit3.Properties.DataSource = dt_物料;
            searchLookUpEdit3.Properties.ValueMember = "物料编码";
            searchLookUpEdit3.Properties.DisplayMember = "物料编码";


            string sql_1 = "select 属性值 as 模板名称 from 基础数据基础属性表 where  属性类别 = '盒贴模板' order by  属性值";

            dt_模板名称 = new DataTable();
            dt_模板名称 = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);
            searchLookUpEdit2.Properties.DataSource = dt_模板名称;
            searchLookUpEdit2.Properties.ValueMember = "模板名称";
            searchLookUpEdit2.Properties.DisplayMember = "模板名称";

            sql_1 = "select 属性值 as 模板名称 from 基础数据基础属性表 where  属性类别 = '箱贴模板' order by  属性值";
            dt_模板名称 = new DataTable();
            dt_模板名称 = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);
            searchLookUpEdit4.Properties.DataSource = dt_模板名称;
            searchLookUpEdit4.Properties.ValueMember = "模板名称";
            searchLookUpEdit4.Properties.DisplayMember = "模板名称";

            sql_1 = "select 属性值  from 基础数据基础属性表 where  属性类别 = '机型' order by  属性值";
            dt_模板名称 = new DataTable();
            dt_模板名称 = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);
            foreach (DataRow dr in dt_模板名称.Rows)
            {
                cmb_jz.Items.Add(dr["属性值"].ToString());

            }


            sql_1 = "select  客户编号,客户名称 from  客户基础信息表 where 停用=0 ";
            dt_客户 = new DataTable();
            dt_客户 = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);

            searchLookUpEdit6.Properties.DataSource = dt_客户;
            searchLookUpEdit6.Properties.ValueMember = "客户编号";
            searchLookUpEdit6.Properties.DisplayMember = "客户名称";
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_盒贴(string str_物料编码, string str_kh)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = string.Format("select * from BQ_HZXX where wlbh = '{0}' ", str_物料编码);
                if (str_kh != "") sql = sql + string.Format(" and khbh='{0}'", str_kh);
                //if (str_mbmc != "") sql = sql + string.Format(" and mbmc='{0}'", str_mbmc);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    //txt_规格型号.Text = dt.Rows[0]["cpxh"].ToString().Trim();
                    txt_物料名称.Text = dt.Rows[0]["cpmc"].ToString().Trim();
                    textBox3.Text = dt.Rows[0]["hzsl"].ToString().Trim();

                    searchLookUpEdit6.EditValue = dt.Rows[0]["khbh"].ToString().Trim();

                   

                    txt_客户料号.Text = dt.Rows[0]["khlh"].ToString().Trim();
                    // txt_机种.Text = dt.Rows[0]["jz"].ToString().Trim();
                    cmb_jz.Text = dt.Rows[0]["ggxh"].ToString().Trim();
                    textBox5.Text = dt.Rows[0]["cpxh"].ToString().Trim();
                    txt_参数.Text = dt.Rows[0]["cs"].ToString().Trim();
                    txt_订单号.Text = dt.Rows[0]["ddh"].ToString().Trim();
                    DataView dv = new DataView(dt);
                    DataTable dtsds = dv.ToTable(true, "dymb");
                    dtsds.Columns["dymb"].ColumnName = "模板名称";
                    searchLookUpEdit2.Properties.DataSource = dtsds;
                    searchLookUpEdit2.Properties.ValueMember = "模板名称";
                    searchLookUpEdit2.Properties.DisplayMember = "模板名称";
                    searchLookUpEdit2.EditValue = dt.Rows[0]["dymb"].ToString().Trim();
                }
                else
                {
                    string sql_1 = "select 属性值 as 模板名称 from 基础数据基础属性表 where  属性类别 = '盒贴模板' order by  属性值";

                    dt_模板名称 = new DataTable();
                    dt_模板名称 = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);
                    searchLookUpEdit2.Properties.DataSource = dt_模板名称;
                    searchLookUpEdit2.Properties.ValueMember = "模板名称";
                    searchLookUpEdit2.Properties.DisplayMember = "模板名称";
                }


                sql = string.Format(@"select  a.客户编号,a.客户名称 from  客户基础信息表 a   left join   BQ_HZXX   b  on   
                 a.客户编号=b.khbh 
                where  a.停用=0 and b.wlbh='{0}' group by a.客户编号,a.客户名称", str_物料编码.ToString());
                dt_客户 = new DataTable();
                dt_客户 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                if (dt_客户.Rows.Count > 0)
                {

                    //DataView dv = new DataView(dt_客户);
                    //DataTable dtmc = dv.ToTable(true, "客户编号", "客户名称");//common_name:所要筛选的字段
                    searchLookUpEdit6.Properties.DataSource = dt_客户;
                    searchLookUpEdit6.Properties.ValueMember = "客户编号";
                    searchLookUpEdit6.Properties.DisplayMember = "客户名称";

                }
                else
                {
                    string sql_1 = "select  客户编号,客户名称 from  客户基础信息表 where 停用=0 ";
                    dt_客户 = new DataTable();
                    dt_客户 = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);

                    searchLookUpEdit6.Properties.DataSource = dt_客户;
                    searchLookUpEdit6.Properties.ValueMember = "客户编号";
                    searchLookUpEdit6.Properties.DisplayMember = "客户名称";
                }







            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void ui返修打印_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            textBox9.Text = System.DateTime.Today.ToString("yyyy-MM-dd");
            textBox8.Text = CPublic.Var.LocalUserID;
            fun_load();
            if (para_物料 != "")
            {

                searchLookUpEdit1.EditValue = para_物料;
                string sql = string.Format(@"select  生产记录生产检验单主表.*,原ERP物料编号 from 生产记录生产检验单主表,基础数据物料信息表  
                        
                             where  生产记录生产检验单主表.物料编码=基础数据物料信息表.物料编码 and   生产记录生产检验单主表.物料编码='{0}' order by 生产记录生产检验单主表.生效日期 desc", para_物料);
                DataTable dt = new DataTable();
                dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                if (dt.Rows.Count > 0)
                {
                    searchLookUpEdit2.EditValue = dt.Rows[0]["模板名称"].ToString().Trim();
                    //  txt_电压.Text = dt.Rows[0]["额定电压"].ToString().Trim();
                    txt_客户料号.Text = dt.Rows[0]["客户料号"].ToString().Trim();
                    textBox3.Text = dt.Rows[0]["盒装数量"].ToString().Trim();
                    // txt_规格型号.Text = dt.Rows[0]["原规格型号"].ToString().Trim();
                    txt_物料名称.Text = dt.Rows[0]["物料名称"].ToString().Trim();
                    txt_参数.Text = dt.Rows[0]["参数"].ToString().Trim();
                    cmb_jz.Text = dt.Rows[0]["机种"].ToString().Trim();
                    txt_订单号.Text = dt.Rows[0]["订单号"].ToString().Trim();
                    //  textBox4.Text = dt.Rows[0]["原ERP物料编号"].ToString().Trim();

                }
            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            /// <summary>
            /// 备用功能 未测试
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            //if (searchLookUpEdit1.Text != "" && searchLookUpEdit6.Text.Trim()!="")
            //{
            //    string sql = string.Format("select * from BQ_HZXX where wlbh = '{0}' ", searchLookUpEdit1.EditValue);
            //    if (searchLookUpEdit6.Text != "") sql = sql + string.Format(" and khbh='{0}'", searchLookUpEdit6.EditValue);
            //    //if (str_mbmc != "") sql = sql + string.Format(" and mbmc='{0}'", str_mbmc);
            //    DataTable dt = new DataTable();
            //    SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            //    da.Fill(dt);
            //    DataRow[] dtg = dt.Select(string.Format("dymb='{0}'", searchLookUpEdit6.Text.Trim()));

            //    if (dtg!=null )
            //    {
            //        //txt_规格型号.Text = dt.Rows[0]["cpxh"].ToString().Trim();
            //        txt_物料名称.Text = dtg[0]["cpmc"].ToString().Trim();
            //        textBox3.Text = dtg[0]["hzsl"].ToString().Trim();
            //        txt_客户料号.Text = dtg[0]["khlh"].ToString().Trim();
            //        // txt_机种.Text = dt.Rows[0]["jz"].ToString().Trim();
            //        cmb_jz.Text = dtg[0]["ggxh"].ToString().Trim();
            //        textBox5.Text = dtg[0]["cpxh"].ToString().Trim();
            //        txt_参数.Text = dtg[0]["cs"].ToString().Trim();
            //        txt_订单号.Text = dtg[0]["ddh"].ToString().Trim();
            //        searchLookUpEdit2.EditValue = dtg[0]["mbmc"].ToString().Trim();
            //        searchLookUpEdit6.EditValue = dtg[0]["khbh"].ToString().Trim();
            //    }
            //}





            // txt_规格型号.Enabled = false;
            txt_客户料号.Enabled = false;
            txt_物料名称.Enabled = false;
            // cmb_jz.Enabled = false;
            txt_订单号.Enabled = false;
            // txt_电压.Enabled = false;

            if (searchLookUpEdit2.EditValue != null)
            {

                if (searchLookUpEdit2.EditValue.ToString() == "中钞科堡-外箱贴")
                {
                    label10.Text = "物料号1";
                    label9.Text = "物料号2";
                    label17.Text = "物料号3";
                    txt_客户料号.Enabled = true;
                    txt_物料名称.Enabled = true;
                }


                else if (searchLookUpEdit2.EditValue.ToString() == "标配通用箱贴")
                {
                    label9.Text = "收货单位";

                    txt_客户料号.Enabled = true;
                    txt_物料名称.Enabled = true;
                }
                else if (searchLookUpEdit2.EditValue.ToString() == "通用盒贴")
                {
                    label9.Text = "参数";

                    txt_客户料号.Enabled = true;
                    txt_物料名称.Enabled = true;
                }
                else if (searchLookUpEdit2.EditValue.ToString() == "广电运通-成都农商行外箱标贴")
                {
                    label9.Text = "参数";

                    txt_客户料号.Enabled = true;
                    txt_物料名称.Enabled = true;
                }
                else if (searchLookUpEdit2.EditValue.ToString() == "日立-工行盒贴")
                {
                    label9.Text = "参数";
                    txt_物料名称.Enabled = true;
                    txt_客户料号.Enabled = true;
                    cmb_jz.Enabled = true;

                }
                else if (searchLookUpEdit2.EditValue.ToString() == "工行盒贴")
                {
                    label9.Text = "参数";
                    txt_物料名称.Enabled = true;
                    txt_客户料号.Enabled = true;
                    cmb_jz.Enabled = true;

                }
                else if (searchLookUpEdit2.EditValue.ToString() == "日立-工行外箱贴")
                {
                    label9.Text = "参数";

                    //txt_规格型号.Enabled = true;
                    //txt_电压.Enabled = true;
                    //txt_电压.Enabled = true;
                    txt_物料名称.Enabled = true;
                    txt_客户料号.Enabled = true;
                }
                else if (searchLookUpEdit2.EditValue.ToString() == "威海新北洋盒贴")
                {
                    label9.Text = "参数";

                    //txt_规格型号.Enabled = true;
                    //txt_电压.Enabled = true;
                    txt_客户料号.Enabled = true;
                    txt_物料名称.Enabled = true;


                }
                else if (searchLookUpEdit2.EditValue.ToString() == "怡化-江苏银行盒贴")
                {
                    label9.Text = "版本号";
                    //txt_规格型号.Enabled = true;
                    //txt_电压.Enabled = true;
                    txt_客户料号.Enabled = true;
                    txt_物料名称.Enabled = true;

                    cmb_jz.Enabled = true;
                }
                else if (searchLookUpEdit2.EditValue.ToString() == "怡化-江苏银行外箱标贴")
                {
                    label9.Text = "参数";
                    //txt_规格型号.Enabled = true;
                    //txt_电压.Enabled = true;

                    txt_客户料号.Enabled = true;
                    txt_物料名称.Enabled = true;
                }
                else if (searchLookUpEdit2.EditValue.ToString() == "怡化-农行盒贴")
                {

                    txt_客户料号.Enabled = true;
                    txt_物料名称.Enabled = true;
                    label9.Text = "版本号";
                    //textBox4.Enabled = true;

                }
                else if (searchLookUpEdit2.EditValue.ToString() == "怡化-农行外箱贴")
                {
                    label9.Text = "版本号";
                    txt_客户料号.Enabled = true;
                    txt_物料名称.Enabled = true;
                }




                else if (searchLookUpEdit2.EditValue.ToString() == "怡化-建行盒贴")
                {

                    txt_客户料号.Enabled = true;
                    txt_物料名称.Enabled = true;
                    label9.Text = "版本号";
                    // txt_参数.Text=
                    //textBox4.Enabled = true;

                }
                else if (searchLookUpEdit2.EditValue.ToString() == "怡化-建行外箱贴")
                {
                    label9.Text = "参数";
                    txt_客户料号.Enabled = true;
                    txt_物料名称.Enabled = true;

                }








                else if (searchLookUpEdit2.EditValue.ToString() == "深圳赞融-招行盒贴")
                {
                    label9.Text = "参数";
                    textBox1.Enabled = true;
                    txt_客户料号.Enabled = true;
                    txt_物料名称.Enabled = true;
                }
                else if (searchLookUpEdit2.EditValue.ToString() == "深圳赞融-招行外箱贴")
                {
                    label9.Text = "送货单位";
                    textBox1.Enabled = true;
                    txt_客户料号.Enabled = true;
                    txt_物料名称.Enabled = true;
                }
                else if (searchLookUpEdit2.EditValue.ToString() == "怡化-天府盒贴")
                {
                    label9.Text = "参数";
                    textBox1.Enabled = true;
                    txt_客户料号.Enabled = true;
                    txt_物料名称.Enabled = true;
                }
                else if (searchLookUpEdit2.EditValue.ToString() == "诺雅克模板")
                {
                    label9.Text = "参数";
                    // txt_规格型号.Enabled = true;
                    txt_物料名称.Enabled = true;
                    txt_客户料号.Enabled = true;
                    //txt_电压.Enabled = true;
                }
                else if (searchLookUpEdit2.EditValue.ToString() == "怡化-天府银行外箱贴")
                {
                    label9.Text = "参数";

                    txt_客户料号.Enabled = true;
                    txt_物料名称.Enabled = true;
                }
                else if (searchLookUpEdit2.EditValue.ToString() == "广电运通-成都农商行盒贴")
                {
                    label9.Text = "参数";

                    //  txt_规格型号.Enabled = true;
                    txt_物料名称.Enabled = true;
                    txt_客户料号.Enabled = true;

                }
                string s = searchLookUpEdit2.EditValue.ToString();

                if (s.Contains("箱贴"))
                {
                    label5.Text = "箱装数量";
                    xzs(1);
                }
                else
                {
                    label5.Text = "盒装数量";
                    xzs(2);

                }

                #region 弃用
                //else if (searchLookUpEdit2.EditValue.ToString() == "欠压英文模板")
                //{
                //    label9.Text = "参数";

                //    //txt_规格型号.Enabled = true;
                //    txt_物料名称.Enabled = true;
                //    // txt_电压.Enabled = true;
                //}

                //else if (searchLookUpEdit2.EditValue.ToString() == "正泰英文版")
                //{
                //    label9.Text = "参数";

                //    //txt_规格型号.Enabled = true;
                //    //txt_电压.Enabled = true;
                //    txt_物料名称.Enabled = true;
                //}
                //if (searchLookUpEdit2.EditValue.ToString() == "常熟外发模板")
                //{
                //    label9.Text = "参数";

                //    // txt_规格型号.Enabled = true;
                //    txt_物料名称.Enabled = true;
                //}
                //if (searchLookUpEdit2.EditValue.ToString() == "良信模板")
                //{
                //    label9.Text = "参数";
                //    //txt_规格型号.Enabled = true;
                //    //txt_电压.Enabled = true;
                //    txt_客户料号.Enabled = true;
                //    txt_物料名称.Enabled = true;
                //}

                ////17-11-15  制七课主任要求添加  中性模板 要求默认打印 原规格
                //if (CPublic.Var.localUser部门编号.ToString() == "0001030107")
                //{
                //    if (searchLookUpEdit2.EditValue.ToString() == "中性模板")
                //    {

                //        // textBox5.Text = txt_规格型号.Text.Trim();

                //    }
                //    else
                //    {
                //        textBox5.Text = textBox1.Text.Trim();
                //    }

                //}

                #endregion
            }


        }
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 2019-1-25  在模板值变化事件中 默认searchLookUpEdit2有值
        /// i=1 箱装数量 ;i=2 盒装数量
        /// </summary>
        private void xzs(int i)
#pragma warning restore IDE1006 // 命名样式
        {
          
            if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "") //物料选择了的 前提下
            {
                string sql = "where 1=1 ";

                sql =sql+ string.Format("and  wlbh='{0}'", searchLookUpEdit1.EditValue.ToString());


                if (searchLookUpEdit6.EditValue != null && searchLookUpEdit6.EditValue.ToString() != "") //客户
                {
                    sql = sql + string.Format(" and khbh='{0}'", searchLookUpEdit6.EditValue.ToString());
                }
                sql +=  string.Format(" and mbmc='{0}'", searchLookUpEdit2.EditValue.ToString());
                if (i == 1)
                {
                    sql = "select  cs,cpmc,cpxh,khlh,箱装数量 from [BQ_HZXX]  " + sql;     
                    DataTable t = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    if (t.Rows.Count > 0)
                    {
                        textBox3.Text = t.Rows[0]["箱装数量"].ToString();
                        txt_参数.Text = t.Rows[0]["cs"].ToString();    
                        txt_客户料号.Text = t.Rows[0]["khlh"].ToString();
                        txt_物料名称.Text = t.Rows[0]["cpmc"].ToString();
                        textBox1.Text = textBox5.Text = t.Rows[0]["cpxh"].ToString();

                    }
                }
                else
                {
                    sql = "select  cs,cpmc,cpxh,khlh,hzsl from [BQ_HZXX]  " + sql;
                    DataTable t = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    if (t.Rows.Count > 0)
                    {
                        textBox3.Text = t.Rows[0]["hzsl"].ToString();
                        txt_参数.Text = t.Rows[0]["cs"].ToString();
                        txt_客户料号.Text = t.Rows[0]["khlh"].ToString();
                        txt_物料名称.Text = t.Rows[0]["cpmc"].ToString();
                        textBox1.Text = textBox5.Text = t.Rows[0]["cpxh"].ToString();
                    }
                }
            }

        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_check_箱贴()
#pragma warning restore IDE1006 // 命名样式
        {
            if (searchLookUpEdit3.EditValue == DBNull.Value || searchLookUpEdit3.EditValue == null || searchLookUpEdit4.EditValue == DBNull.Value || searchLookUpEdit4.EditValue == null)
            {
                throw new Exception("未选择打印信息");
            }

            if (searchLookUpEdit5.EditValue == DBNull.Value || searchLookUpEdit5.EditValue == null)
            {
                throw new Exception("未选择客户信息");
            }
            if (textBox15.Text.Trim() == "")
            {
                throw new Exception("未填写起始序列号");
            }
            if (textBox14.Text.Trim() == "")
            {
                throw new Exception("箱装数量未填写");
            }
            else
            {
                try
                {
                    int x = Convert.ToInt32(textBox14.Text);

                }
                catch (Exception ex)
                {

                    throw new Exception("箱装数量填写有误");
                }


            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {
            if (searchLookUpEdit1.EditValue == DBNull.Value || searchLookUpEdit1.EditValue == null || searchLookUpEdit2.EditValue == DBNull.Value || searchLookUpEdit2.EditValue == null)
            {
                throw new Exception("未选择打印信息");
            }
            //if (textBox6.Text == "")
            //{
            //    string ss = DateTime.Now.Year.ToString().Substring(2, 2);

            //    string str_工单 = string.Format("MO{0}{1:D2}{2:00}{3:0000}", ss, DateTime.Now.Month, DateTime.Now.Day, CPublic.CNo.fun_得到最大流水号("MO", DateTime.Now.Year, DateTime.Now.Month, System.DateTime.Now.Day));
            //    textBox6.Text = str_工单;

            //}
            if (textBox2.Text == "")
            {
                throw new Exception("请填写制造数量");

            }

            else
            {
                int a = 0;
                try
                {
                    a = Convert.ToInt32(textBox2.Text);

                }
                catch (Exception ex)
                {

                    throw new Exception("制造数量输入不正确");

                }
                if (a <= 0)
                {
                    throw new Exception("制造数量不能小于0");

                }
            }


            if (textBox3.Text == "")
            {
                throw new Exception("盒装数量不能为空");

            }
            else
            {
                int a = 0;
                try
                {
                    a = Convert.ToInt32(textBox3.Text);

                }
                catch (Exception ex)
                {

                    throw new Exception("盒装数量输入不正确");

                }
                if (a <= 0)
                {
                    throw new Exception("盒装数量不能小于0");

                }
            }

        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_打印(string str_打印机)
#pragma warning restore IDE1006 // 命名样式
        {
            int c_余数 = 0;
            int count;
            //count = -1;
            //DataRow dr = dt_dy.Rows[0];
            int In_生产数量 = Convert.ToInt32(textBox2.Text);
            int i_盒装数 = Convert.ToInt32(textBox3.Text);



            //打印份数

            if (In_生产数量 % i_盒装数 == 0)
            {
                count = In_生产数量 / i_盒装数;
            }
            else
            {
                c_余数 = In_生产数量 % i_盒装数;
                count = In_生产数量 / i_盒装数 + 1;
            }



            string path = Application.StartupPath + string.Format(@"\Mode\{0}.lab", searchLookUpEdit2.EditValue);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            //默认 模板 都拥有 jgddh wlbh eddy khlh jyrq （常熟 两个模板 日期不一样）
            //dic.Add("jgddh", textBox6.Text);
            //dic.Add("wlbh", textBox4.Text.Trim().ToString());

            //dic.Add("eddy", txt_电压.Text.Trim().ToString());

            //dic.Add("khlh", txt_客户料号.Text.Trim().ToString());


            //dic.Add("cpxh", textBox5.Text.Trim());
            ////  dic.Add("cpmc", textBox5.Text.Trim());
            //dic.Add("hzsl", textBox3.Text.ToString());
            //dic.Add("jyy", textBox8.Text.ToString());
            //dic.Add("rq", Convert.ToDateTime(textBox9.Text).ToString("yyyy-MM-dd"));

            if (searchLookUpEdit2.EditValue.ToString().Trim() == "工行盒贴")
            {
                dic.Add("cpxh", textBox5.Text.Trim());
                dic.Add("jx", cmb_jz.Text.ToString().Trim());
                dic.Add("cs", txt_参数.Text.ToString().Trim()); // 标准  左   右
                dic["cpmc"] = txt_物料名称.Text.ToString();
                // dic.Remove("检验日期");
                dic.Remove("jyy");
                dic.Remove("hzsl");
                label9.Text = "参数";

            }

            else if (searchLookUpEdit2.EditValue.ToString().Trim() == "中钞科堡-外箱贴")
            {
                dic.Add("cpmc", txt_物料名称.Text.ToString().Trim());
                dic.Add("wldm1", txt_客户料号.Text.ToString().Trim());
                dic.Add("wldm2", txt_参数.Text.ToString().Trim()); // 标准  左   右
                dic.Add("wldm3", textBox5.Text.ToString().Trim()); // 标准  左   右


            }



            else if (searchLookUpEdit2.EditValue.ToString().Trim() == "标配通用箱贴")
            {
                dic.Add("cpxh", textBox5.Text.Trim());
                label9.Text = "收货单位";
                dic.Add("cs", txt_参数.Text.ToString().Trim()); //  收货单位
                dic.Add("wlmc", txt_物料名称.Text.ToString());
                dic.Add("wldm", txt_客户料号.Text.ToString());
                if (count == 1)
                {
                    dic["sl"] = In_生产数量.ToString();
                }
                else
                {
                    dic["sl"] = i_盒装数.ToString();

                }

            }
            else if (searchLookUpEdit2.EditValue.ToString().Trim() == "通用盒贴")
            {

                label9.Text = "参数";
                dic.Add("jx", cmb_jz.Text.ToString());
                dic.Add("cpmc", txt_物料名称.Text.ToString());
                dic.Add("wldm", txt_客户料号.Text.ToString());


            }
            else if (searchLookUpEdit2.EditValue.ToString().Trim() == "怡化-建行外箱贴")
            {
                label9.Text = "参数";
                dic.Add("wldm", txt_客户料号.Text.Trim());
                dic.Add("wlmc", txt_物料名称.Text.ToString().Trim());

                if (count == 1)
                {
                    dic["sl"] = In_生产数量.ToString();
                }
                else
                {
                    dic["sl"] = i_盒装数.ToString();

                }

                string rq = Convert.ToDateTime(textBox9.Text).ToShortDateString().ToString();//2005-11-5 
                dic["rq"] = rq;
                // dic.Add("cs", txt_参数.Text.ToString().Trim()); // 标准  左   右

            }
            else if (searchLookUpEdit2.EditValue.ToString().Trim() == "怡化-建行盒贴")
            {
                dic.Add("cpmc", txt_物料名称.Text.ToString().Trim());
                dic.Add("cpxh", textBox5.Text.ToString().Trim());
                dic.Add("bbh", txt_参数.Text.ToString().Trim()); // 标准  左   右
                dic["wldm"] = txt_客户料号.Text.Trim();
                label9.Text = "参数";

            }




            else if (searchLookUpEdit2.EditValue.ToString().Trim() == "广电运通-成都农商行外箱标贴")
            {
                label9.Text = "参数";
                dic.Add("wlmc", txt_物料名称.Text.ToString());
                if (count == 1)
                {
                    dic["sl"] = In_生产数量.ToString();
                }
                else
                {
                    dic["sl"] = i_盒装数.ToString();

                }
                dic.Add("wldm", txt_客户料号.Text.ToString());
                //// dic.Add("", textBox4.Text.ToString());
                //  dic.Remove("jgddh");

                //  dic.Remove("wlbh");


            }
            else if (searchLookUpEdit2.EditValue.ToString().Trim() == "日立-工行盒贴")
            {
                dic.Add("cpmc", txt_物料名称.Text.ToString());
                dic.Add("cpxh", textBox5.Text.ToString());
                dic.Add("wldm", txt_客户料号.Text.ToString());

                label9.Text = "参数";
                //dic.Add("jx", cmb_jz.Text.ToString());


                //if (count == 1)
                //{
                //    dic["hzsl"] = In_生产数量.ToString();
                //}

            }
            else if (searchLookUpEdit2.EditValue.ToString().Trim() == "日立-工行外箱贴")
            {
                dic.Add("wlmc", txt_物料名称.Text.ToString());

                dic.Add("wlgg", textBox5.Text.ToString());

                dic.Add("wldm", txt_客户料号.Text.ToString());

                label9.Text = "参数";
                // dic.Add("jx", cmb_jz.Text.ToString());


                if (count == 1)
                {
                    dic["sl"] = In_生产数量.ToString();
                }
                else
                {
                    dic["sl"] = i_盒装数.ToString();

                }
                ////dic["jgddh"] = textBox6.Text.ToString();
                ////dic["wlbh"] = textBox4.Text.ToString();
                ////dic["eddy"] = txt_电压.Text.ToString();
                ////dic["khlh"] = txt_客户料号.Text.ToString();
                //dic.Remove("jyrq");



            }

            else if (searchLookUpEdit2.EditValue.ToString().Trim() == "威海新北洋盒贴")
            {

                label9.Text = "参数";
                dic.Add("cpmc", txt_物料名称.Text.ToString());
                dic.Add("xh", textBox5.Text.ToString());
                dic.Add("wlbm", txt_客户料号.Text.ToString());
                // dic.Add("azmlb", textBox4.Text.ToString());

                dic.Add("jx", cmb_jz.Text.ToString());



                //// dic["cpxh"] = txt_规格型号.Text.Trim();
                //dic["cpxh"] = textBox5.Text.Trim();

                //dic.Add("cpmc", txt_物料名称.Text.ToString());
                //if (count == 1)
                //{
                //    dic["hzsl"] = In_生产数量.ToString();
                //}
                //dic.Remove("khlh");
                //dic.Remove("eddy");

                // dic.Add("scph", textBox6.Text.ToString());
            }
            else if (searchLookUpEdit2.EditValue.ToString().Trim() == "怡化-天府银行外箱贴")
            {
                label9.Text = "参数";
                dic.Add("wlmc", txt_物料名称.Text.ToString());


                if (count == 1)
                {
                    dic["sl"] = In_生产数量.ToString();
                }
                else
                {
                    dic["sl"] = i_盒装数.ToString();

                }

                dic.Add("wldm", txt_客户料号.Text.ToString());
                string ss = DateTime.Now.Year.ToString().Substring(2, 2);
                string rq = fun_date(Convert.ToDateTime(textBox9.Text).ToString("yyMM"));
                dic["rq"] = rq;

            }
            else if (searchLookUpEdit2.EditValue.ToString().Trim() == "怡化-江苏银行盒贴")
            {
                label9.Text = "版本号";
                dic.Add("cpmc", txt_物料名称.Text.ToString());
                dic.Add("cpxh", textBox5.Text.ToString());
                dic.Add("wldm", txt_客户料号.Text.ToString());
                dic.Add("BBH", txt_参数.Text.ToString());
                // dic.Add("azmlb", textBox4.Text.ToString());

                // dic.Add("jx", cmb_jz.Text.ToString());



                //dic.Add("cpmc", txt_物料名称.Text.ToString());
                //if (count == 1)
                //{
                //    dic["hzsl"] = In_生产数量.ToString();
                //}
                //dic.Remove("jgddh");

                //dic.Remove("khlh");
                //dic.Remove("eddy");


            }
            else if (searchLookUpEdit2.EditValue.ToString().Trim() == "怡化-江苏银行外箱标贴")
            {

                label9.Text = "参数";
                dic.Add("wlmc", txt_物料名称.Text.ToString());
                string ss = DateTime.Now.Year.ToString().Substring(2, 2);
                string rq = fun_date(Convert.ToDateTime(textBox9.Text).ToString("yyMM"));
                dic["rq"] = rq;
                dic.Add("wldm", txt_客户料号.Text.ToString());



                // dic.Add("azmlb", textBox4.Text.ToString());

                //dic.Add("cpmc", txt_物料名称.Text.ToString());
                if (count == 1)
                {
                    dic["sl"] = In_生产数量.ToString();
                }
                else
                {
                    dic["sl"] = i_盒装数.ToString();

                }
                //dic.Add("cs", txt_参数.Text.ToString());
                //dic.Remove("jgddh");

                //dic.Remove("wlbh");

            }

            else if (searchLookUpEdit2.EditValue.ToString().Trim() == "怡化-农行盒贴")
            {
                label9.Text = "参数";
                dic.Add("cpmc", txt_物料名称.Text.ToString());
                dic.Add("cpxh", textBox5.Text.ToString());
                dic.Add("wldm", txt_客户料号.Text.ToString());
                dic.Add("bbh", txt_参数.Text.ToString());

                //string ss = DateTime.Now.Year.ToString().Substring(2, 2);
                //string rq = fun_date(Convert.ToDateTime(textBox9.Text).ToString("yyMM"));
                //dic["jyrq"] = rq;
                //dic["jyrq"] = Convert.ToDateTime(dt_dy.Rows[0]["检验日期"]).ToString("yyMM");
            }

            else if (searchLookUpEdit2.EditValue.ToString().Trim() == "广电运通-成都农商行盒贴")
            {
                label9.Text = "参数";
                dic.Add("cpmc", txt_物料名称.Text.ToString());

                dic.Add("wldm", txt_客户料号.Text.ToString());
            }


            else if (searchLookUpEdit2.EditValue.ToString().Trim() == "怡化-农行外箱贴")
            {
                label9.Text = "参数";
                if (count == 1)
                {
                    dic["sl"] = In_生产数量.ToString();
                }
                else
                {
                    dic["sl"] = i_盒装数.ToString();

                }
                dic.Add("cpmc", txt_物料名称.Text.ToString());

                dic.Add("wldm", txt_客户料号.Text.ToString());
                string ss = DateTime.Now.Year.ToString().Substring(2, 2);
                string rq = fun_date(Convert.ToDateTime(textBox9.Text).ToString("yyMM"));
                dic["rq"] = rq;
            }
            else if (searchLookUpEdit2.EditValue.ToString().Trim() == "怡化-天府盒贴")
            {
                label9.Text = "参数";
                dic.Add("cpmc", txt_物料名称.Text.ToString());

                dic.Add("cpxh", textBox5.Text.ToString());

                dic.Add("wldm", txt_客户料号.Text.ToString());
                //dic.Add("jx", cmb_jz.Text.ToString());
            }
            else if (searchLookUpEdit2.EditValue.ToString().Trim() == "深圳赞融-招行盒贴")
            {
                label9.Text = "参数";
                dic.Add("cpmc", txt_物料名称.Text.ToString());

                dic.Add("jx", cmb_jz.Text.ToString());

                // dic.Add("wldm", txt_客户料号.Text.ToString());
                //dic.Add("jx", cmb_jz.Text.ToString());
            }
            else if (searchLookUpEdit2.EditValue.ToString().Trim() == "深圳赞融-招行外箱贴")
            {
                label9.Text = "参数";
                dic.Add("wlmc", txt_物料名称.Text.ToString());

                dic.Add("shdw", ("(" + txt_参数.Text.Trim().ToString() + ")").ToString().Trim());

                if (count == 1)
                {
                    dic["sl"] = In_生产数量.ToString();
                }
                else
                {
                    dic["sl"] = i_盒装数.ToString();

                }
                //dic.Add("jx", cmb_jz.Text.ToString());
            }

            #region 弃用

            //else if (searchLookUpEdit2.EditValue.ToString().Trim() == "辅助英文模板" || searchLookUpEdit2.EditValue.ToString().Trim() == "分励英文模板" || searchLookUpEdit2.EditValue.ToString().Trim() == "欠压英文模板" || searchLookUpEdit2.EditValue.ToString().Trim() == "辅报英文模板" || searchLookUpEdit2.EditValue.ToString().Trim() == "闭合英文模板" || searchLookUpEdit2.EditValue.ToString().Trim() == "报警英文模板")
            //{

            //    dic.Add("cpmc", txt_物料名称.Text.ToString());
            //    //17-10-11 
            //    dic["khlh"] = txt_客户料号.Text.ToString();

            //    if (count == 1)
            //    {
            //        dic["hzsl"] = In_生产数量.ToString();
            //    }
            //    dic.Remove("jgddh");
            //    dic.Remove("wlbh");
            //    // 17-10-11
            //    //  dic.Remove("khlh");
            //}
            //if (searchLookUpEdit2.EditValue.ToString().Trim() == "宏美模板")
            //{



            //    //int icount = (int)Convert.ToDecimal(textBox11.Text.ToString()) / Convert.ToInt32(textBox8.Text.ToString());// 生产数 除以 盒装数量 取整 
            //    int icount = In_生产数量 / Convert.ToInt32(textBox3.Text.ToString());
            //    int i_余数 = In_生产数量 % Convert.ToInt32(textBox3.Text.ToString());
            //    if (In_生产数量 % Convert.ToInt32(textBox3.Text.ToString()) != 0)
            //    {
            //        icount = icount + 1;
            //    }
            //    for (int i = 1; i <= icount; i++)
            //    {
            //        dic = new Dictionary<string, string>();

            //        //dic.Add("wlbh", dt_dy.Rows[0]["物料编码"].ToString());
            //        //dic.Add("jgddh", "");
            //        dic.Add("khlh", txt_客户料号.Text.ToString());
            //        dic.Add("jyy", textBox8.Text.ToString());
            //        //dic.Add("jz", dt_dy.Rows[0]["机种"].ToString());
            //        dic.Add("cpmc", txt_物料名称.Text.ToString());

            //        dic.Add("jz", Convert.ToDateTime(textBox9.Text).ToString("yyMMdd") + cmb_jz.Text.ToString().Trim() + "10");
            //        //dic.Add("cpxh", txt_规格型号.Text.Trim());
            //        dic.Add("cpxh", textBox5.Text.Trim());


            //        //dic["jgddh"]=textBox5.Text.ToString().Trim()+(i+1).ToString("000")+Convert.ToDateTime(dt_dy.Rows[0]["检验日期"]).ToString("yyMMdd");
            //        dic["jgddh"] = txt_订单号.Text.ToString().Trim() + i.ToString("000").Trim() + dic["jz"];
            //        if (i_余数 != 0 && (i == icount || icount == 1))
            //        {
            //            //int a = (int)Convert.ToDecimal(dt_dy.Rows[0]["生产数量"]);
            //            dic["hzsl"] = i_余数.ToString();
            //        }
            //        else
            //        {
            //            dic["hzsl"] = textBox3.Text.ToString();
            //        }

            //        Lprinter lp1 = new Lprinter(path, dic, str_打印机, 1);
            //        //lp1.Start();
            //        lp1.DoWork();
            //        if (i == 1)
            //        {
            //            if (MessageBox.Show("是否继续？", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
            //            {
            //                continue;
            //            }
            //            else
            //            {
            //                count = 1;
            //                return;
            //            }
            //        }
            //    }

            //    //fun_save_打印历史记录();


            //}
            //if (searchLookUpEdit2.EditValue.ToString().Trim() == "台安模板" || searchLookUpEdit2.EditValue.ToString().Trim() == "芜湖德力西" || searchLookUpEdit2.EditValue.ToString().Trim() == "芜湖德力西英文")
            //{
            //    dic.Add("cpmc", txt_物料名称.Text.ToString());
            //    if (count == 1)
            //    {
            //        dic["hzsl"] = In_生产数量.ToString();
            //    }
            //    dic.Add("jz", cmb_jz.Text.ToString());
            //    dic.Add("ddh", txt_订单号.Text.ToString());
            //    dic.Remove("jgddh");

            //    dic.Remove("wlbh");
            //}

            #endregion 弃用


            Lprinter lp = new Lprinter(path, dic, str_打印机, 1);
            lp.DoWork();
            if (count > 1)
            {
                if (MessageBox.Show("是否继续打印?", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    if (c_余数 == 0)
                    {
                        Lprinter lp1 = new Lprinter(path, dic, str_打印机, count - 1);
                        lp1.Start();

                    }
                    else
                    {
                        Lprinter lp1 = new Lprinter(path, dic, str_打印机, count - 2);
                        lp1.DoWork();
                        dic["sl"] = c_余数.ToString();
                        Lprinter lp2 = new Lprinter(path, dic, str_打印机, 1);
                        lp2.DoWork();
                    }
                }
                //else
                //{
                //    count = 1;
                //}

            }
        }
#pragma warning disable IDE1006 // 命名样式
        private string fun_date(string str)
#pragma warning restore IDE1006 // 命名样式
        {
            string sss = "";
            foreach (char c in str)
            {
                if (c == '1')
                {
                    sss = sss + 'A';
                }
                else if (c == '2')
                {
                    sss = sss + 'B';
                }
                else if (c == '3')
                {

                    sss = sss + 'C';
                }
                else if (c == '4')
                {
                    sss = sss + 'D';
                }
                else if (c == '5')
                {
                    sss = sss + 'E';
                }
                else if (c == '6')
                {
                    sss = sss + 'F';
                }
                else if (c == '7')
                {
                    sss = sss + 'G';
                }
                else if (c == '8')
                {
                    sss = sss + 'H';
                }
                else if (c == '9')
                {
                    sss = sss + 'I';
                }
                else
                {
                    sss = sss + 'J';

                }


            }
            return sss;
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_打印箱贴()
#pragma warning restore IDE1006 // 命名样式
        {


            //int boxcount = 0;
            //int makecount = 0;
            try
            {
                //boxcount = Convert.ToInt32(textBox14.Text);
                //makecount = Convert.ToInt32(Convert.ToDecimal(textBox17.Text));
                //if (boxcount != 0)
                //{

                //    int count = makecount / boxcount;
                //    int ys = makecount % boxcount;
                //    if (makecount % boxcount != 0)
                //    {
                //        count++;
                //    }

                string path = Application.StartupPath + string.Format(@"\Mode\{0}.lab", searchLookUpEdit4.EditValue.ToString());
                //if (textBox13.Text.Trim() == "通用箱贴")
                //{
                //    Dictionary<string, string> dic = new Dictionary<string, string>();
                //    //默认 模板 都拥有 jgddh wlbh eddy khlh jyrq （常熟 两个模板 日期不一样）
                //    dic.Add("gdh", dt_dy.Rows[0]["生产工单号"].ToString());
                //    dic.Add("dyzs", count.ToString());
                //    dic.Add("xzsl", boxcount.ToString());
                //    dic.Add("ys", ys.ToString());
                //    Lprinter lp_box = new Lprinter(path, dic, printer, count);
                //    lp_box.DoWork();
                //}
                //dic.Add("hzsl", textBox8.Text.ToString());
                if (searchLookUpEdit4.EditValue.ToString() == "日新箱贴")
                {
                    List<Dictionary<string, string>> li = new List<Dictionary<string, string>>();
                    int i_箱装;
                    if (count == 1 && ys != 0)
                    {
                        i_箱装 = ys;
                    }
                    else
                    {
                        i_箱装 = boxcount;
                    }
                    string qsm = textBox15.Text.Trim(); //起始码
                    int qs_序列 = Convert.ToInt32(textBox16.Text); //起始序列

                    Dictionary<string, string> dic1 = new Dictionary<string, string>();
                    dic1.Add("xzsl", i_箱装.ToString());
                    dic1.Add("cpmc", textBox13.Text.Trim());
                    dic1.Add("cpxh", textBox11.Text.Trim());
                    dic1.Add("eddy", textBox10.Text.Trim());
                    dic1.Add("dl", textBox12.Text.Trim());
                    dic1.Add("khmc", searchLookUpEdit5.Text.ToString());
                    dic1.Add("qsm", qsm);
                    int xx = qs_序列 + i_箱装 - 1;
                    string x = qsm.Substring(0, 7) + xx.ToString().PadLeft(14, '0');
                    qsm = qsm.Substring(0, 7) + (xx + 1).ToString().PadLeft(14, '0');//下一张的起始号

                    dic1.Add("jsm", x);

                    qs_序列 = xx + 1;

                    Lprinter lp1 = new Lprinter(path, dic1, printer_箱贴, 1);
                    lp1.DoWork();
                    if (count > 1)
                    {
                        if (MessageBox.Show("是否继续打印?", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {

                            for (int i = 2; i <= count; i++)
                            {
                                Dictionary<string, string> dic = new Dictionary<string, string>();

                                if (i == count && ys != 0)
                                {
                                    i_箱装 = ys;
                                }
                                else
                                {
                                    i_箱装 = boxcount;
                                }
                                //这个是 一箱的序列码最后一张 
                                int i_1 = qs_序列 + i_箱装 - 1;
                                string x1 = qsm.Substring(0, 7) + i_1.ToString().PadLeft(14, '0');
                                //郁说 箱贴上不需要有 检验码
                                //  x1 = x1 + ERPorg.Corg.fun_gccode(x);
                                dic.Add("xzsl", i_箱装.ToString());
                                dic.Add("cpmc", textBox13.Text.Trim());
                                dic.Add("cpxh", textBox11.Text.Trim());
                                dic.Add("eddy", textBox10.Text.Trim());
                                dic.Add("dl", textBox12.Text.Trim());
                                dic.Add("khmc", searchLookUpEdit5.Text.ToString());
                                dic.Add("qsm", qsm);
                                qsm = qsm.Substring(0, 7) + (i_1 + 1).ToString().PadLeft(14, '0'); //下一张起始码
                                dic.Add("jsm", x1);

                                qs_序列 = i_1 + 1;
                                li.Add(dic);
                            }

                            Lprinter lp = new Lprinter(path, li, printer_箱贴, 1);
                            lp.DoWork();
                        }

                    }
                }
                if (searchLookUpEdit4.EditValue.ToString() == "河南电网箱贴_36")
                {
                    List<Dictionary<string, string>> li = new List<Dictionary<string, string>>();
                    //int i_箱装;
                    if ((count == 1 && ys != 0) || (xc == count && ys != 0))
                    {
                        i_箱装 = ys;
                    }
                    else
                    {
                        i_箱装 = boxcount;
                    }

                    Dictionary<string, string> dic1 = new Dictionary<string, string>();
                    dic1.Add("xzsl", i_箱装.ToString());
                    dic1.Add("xc", xc.ToString());
                    dic1.Add("总箱数", count.ToString());

                    //dic1.Add("cpmc", textBox13.Text.Trim());
                    dic1.Add("cpxh", textBox11.Text.Trim());
                    //dic1.Add("eddy", textBox10.Text.Trim());
                    //dic1.Add("dl", textBox12.Text.Trim());
                    dic1.Add("khmc", searchLookUpEdit5.Text.ToString());
                    dic1.Add("qsm", qsm);
                    int xx = qs_序列 + i_箱装 - 1;
                    string x = qsm.Substring(0, 7) + xx.ToString().PadLeft(14, '0');
                    qsm = qsm.Substring(0, 7) + (xx + 1).ToString().PadLeft(14, '0');//下一张的起始号

                    dic1.Add("jsm", x);
                    for (int e = 1; e <= i_箱装; e++)
                    {
                        int xx_1 = qs_序列 + e - 1;
                        string x_1 = qsm.Substring(0, 7) + xx_1.ToString().PadLeft(14, '0');
                        dic1.Add(string.Format("zh{0}", e), x_1);
                    }
                    qs_序列 = xx + 1;

                    Lprinter lp1 = new Lprinter(path, dic1, printer_箱贴, 1);
                    lp1.DoWork();
                    button2.Visible = true;
                    xc++; //第几箱 箱次增加
                    if (xc > count) //打印结束
                    {

                        button2.Visible = false;

                    }
                    //if (count > 1)
                    //{
                    //    if (MessageBox.Show("是否继续打印?", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    //    {

                    //        for (int i = 2; i <= count; i++)
                    //        {
                    //            Dictionary<string, string> dic = new Dictionary<string, string>();

                    //            if (i == count && ys != 0)
                    //            {
                    //                i_箱装 = ys;
                    //            }
                    //            else
                    //            {
                    //                i_箱装 = boxcount;
                    //            }
                    //            //这个是 一箱的序列码最后一张 
                    //            int i_1 = qs_序列 + i_箱装 - 1;
                    //            string x1 = qsm.Substring(0, 7) + i_1.ToString().PadLeft(14, '0');
                    //            //郁小平说 箱贴上不需要有 检验码
                    //            //  x1 = x1 + ERPorg.Corg.fun_gccode(x);
                    //            dic.Add("xzsl", i_箱装.ToString());
                    //            dic.Add("总箱数", count.ToString());

                    //            dic.Add("xc", i.ToString());
                    //            dic.Add("cpxh", textBox11.Text.Trim());
                    //            //dic.Add("eddy", textBox10.Text.Trim());
                    //            //dic.Add("dl", textBox12.Text.Trim());
                    //            dic.Add("khmc", searchLookUpEdit5.Text.ToString());
                    //            dic.Add("qsm", qsm);
                    //            qsm = qsm.Substring(0, 7) + (i_1 + 1).ToString().PadLeft(14, '0'); //下一张起始码
                    //            dic.Add("jsm", x1);
                    //            for (int e = 1; e <= i_箱装; e++)
                    //            {
                    //                int xx_1 = qs_序列 + e - 1;
                    //                string x_1 = qsm.Substring(0, 7) + xx_1.ToString().PadLeft(14, '0');
                    //                dic.Add(string.Format("zh{0}", e), x_1);
                    //            }
                    //            qs_序列 = i_1 + 1;
                    //            li.Add(dic);
                    //        }

                    //        Lprinter lp = new Lprinter(path, li, printer_箱贴, 1);
                    //        lp.DoWork();
                    //    }

                    //}
                }
                if (searchLookUpEdit4.EditValue.ToString() == "河南电网箱贴_24")
                {
                    List<Dictionary<string, string>> li = new List<Dictionary<string, string>>();
                    //  int i_箱装;
                    if ((count == 1 && ys != 0) || (xc == count && ys != 0))
                    {
                        i_箱装 = ys;
                    }
                    else
                    {
                        i_箱装 = boxcount;
                    }
                    //string qsm = textBox15.Text.Trim(); //起始码
                    // int qs_序列 = Convert.ToInt32(textBox16.Text); //起始序列

                    Dictionary<string, string> dic1 = new Dictionary<string, string>();
                    dic1.Add("xzsl", i_箱装.ToString());
                    dic1.Add("xc", xc.ToString());
                    dic1.Add("总箱数", count.ToString());

                    //dic1.Add("cpmc", textBox13.Text.Trim());
                    dic1.Add("cpxh", textBox11.Text.Trim());
                    //dic1.Add("eddy", textBox10.Text.Trim());
                    //dic1.Add("dl", textBox12.Text.Trim());
                    dic1.Add("khmc", searchLookUpEdit5.Text.ToString());
                    dic1.Add("qsm", qsm);
                    int xx = qs_序列 + i_箱装 - 1;
                    string x = qsm.Substring(0, 7) + xx.ToString().PadLeft(14, '0');
                    qsm = qsm.Substring(0, 7) + (xx + 1).ToString().PadLeft(14, '0');//下一张的起始号

                    dic1.Add("jsm", x);
                    for (int e = 1; e <= i_箱装; e++)
                    {
                        int xx_1 = qs_序列 + e - 1;
                        string x_1 = qsm.Substring(0, 7) + xx_1.ToString().PadLeft(14, '0');
                        dic1.Add(string.Format("zh{0}", e), x_1);
                    }
                    qs_序列 = xx + 1;

                    Lprinter lp1 = new Lprinter(path, dic1, printer_箱贴, 1);
                    lp1.DoWork();
                    button2.Visible = true;
                    xc++;
                    if (xc > count) //打印结束
                    {

                        button2.Visible = false;

                    }
                    //if (count > 1)
                    //{
                    //    if (MessageBox.Show("是否继续打印?", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    //    {

                    //        for (int i = 2; i <= count; i++)
                    //        {
                    //            Dictionary<string, string> dic = new Dictionary<string, string>();

                    //            if (i == count && ys != 0)
                    //            {
                    //                i_箱装 = ys;
                    //            }
                    //            else
                    //            {
                    //                i_箱装 = boxcount;
                    //            }
                    //            //这个是 一箱的序列码最后一张 
                    //            int i_1 = qs_序列 + i_箱装 - 1;
                    //            string x1 = qsm.Substring(0, 7) + i_1.ToString().PadLeft(14, '0');
                    //            //郁小平说 箱贴上不需要有 检验码
                    //            //  x1 = x1 + ERPorg.Corg.fun_gccode(x);
                    //            dic.Add("xzsl", i_箱装.ToString());
                    //            dic.Add("总箱数", count.ToString());

                    //            dic.Add("xc", i.ToString());
                    //            dic.Add("cpxh", textBox11.Text.Trim());
                    //            //dic.Add("eddy", textBox10.Text.Trim());
                    //            //dic.Add("dl", textBox12.Text.Trim());
                    //            dic.Add("khmc", searchLookUpEdit5.Text.ToString());
                    //            dic.Add("qsm", qsm);
                    //            qsm = qsm.Substring(0, 7) + (i_1 + 1).ToString().PadLeft(14, '0'); //下一张起始码
                    //            dic.Add("jsm", x1);
                    //            for (int e = 1; e <= i_箱装; e++)
                    //            {
                    //                int xx_1 = qs_序列 + e - 1;
                    //                string x_1 = qsm.Substring(0, 7) + xx_1.ToString().PadLeft(14, '0');
                    //                dic.Add(string.Format("zh{0}", e), x_1);
                    //            }
                    //            qs_序列 = i_1 + 1;
                    //            li.Add(dic);
                    //        }

                    //        Lprinter lp = new Lprinter(path, li, printer_箱贴, 1);
                    //        lp.DoWork();
                    //    }

                    //}
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (xtraTabControl1.SelectedTabPage.Name == "xtraTabPage1")
                {
                    fun_check();
                    if (MessageBox.Show(string.Format("确定打印？"), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {

                        PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                        this.printDialog1.Document = this.printDocument1;
                        DialogResult dr = this.printDialog1.ShowDialog();
                        if (dr == DialogResult.OK)
                        {
                            string PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                            fun_打印(PrinterName);
                        }
                    }
                }
                else
                {

                    fun_check_箱贴();

                    boxcount = Convert.ToInt32(textBox14.Text);
                    xc = 1;
                    makecount = Convert.ToInt32(Convert.ToDecimal(textBox17.Text));
                    if (boxcount != 0)
                    {

                        int count = makecount / boxcount; //总箱数
                        int ys = makecount % boxcount;
                        if (makecount % boxcount != 0)
                        {
                            count++;
                        }
                    }
                    fun_打印箱贴();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }



#pragma warning disable IDE1006 // 命名样式
        private void button1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fm箱贴序列码检验码 fm = new fm箱贴序列码检验码();
            fm.StartPosition = FormStartPosition.CenterScreen;
            fm.MaximizeBox = false;
            fm.FormBorderStyle = FormBorderStyle.FixedDialog;
            fm.ShowDialog();
            if (fm.flag)
            {
                textBox15.Text = fm.s;
                textBox16.Text = fm.i_起始.ToString();
            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void searchLookUpEdit3_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (searchLookUpEdit3.EditValue != null)
            {
                DataRow[] dr = dt_物料.Select(string.Format("物料编码='{0}'", searchLookUpEdit3.EditValue.ToString()));
                // textBox11.Text = dr[0]["n原ERP规格型号"].ToString();
                // textBox7.Text = dr[0]["原ERP物料编号"].ToString().Trim();
                //  textBox13.Text = dr[0]["物料名称"].ToString();
                string str_kh = "";
                // string str_mbmc = "";
                if (searchLookUpEdit6.EditValue != null && searchLookUpEdit6.EditValue.ToString() != "") str_kh = searchLookUpEdit6.EditValue.ToString();
                // if (searchLookUpEdit2.EditValue != null && searchLookUpEdit2.EditValue.ToString() != "") str_kh = searchLookUpEdit2.EditValue.ToString();
                fun_盒贴(textBox7.Text, str_kh);

                //if (dr[0]["产品线"].ToString() == "智能终端电器")
                //{
                //  textBox5.Text = textBox1.Text; //新规格

                //}
                //else
                //{

                //    textBox5.Text = txt_规格型号.Text;

                //}
            }
            else
            {
                //textBox4.Text = "";
                textBox11.Text = "";
                textBox7.Text = "";
                textBox13.Text = "";
            }
        }
        //继续打印
#pragma warning disable IDE1006 // 命名样式
        private void button2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_打印箱贴();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (xtraTabControl1.SelectedTabPage.Name == "xtraTabPage1")
                {
                    fun_check();
                    if (MessageBox.Show(string.Format("确定打印？"), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {

                        PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();

                        this.printDialog1.Document = this.printDocument1;
                        DialogResult dr = this.printDialog1.ShowDialog();
                        if (dr == DialogResult.OK)
                        {
                            string PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                            fun_打印(PrinterName);
                        }
                    }
                }
                else
                {

                    fun_check_箱贴();
                    qsm = textBox15.Text.Trim(); //起始码
                    boxcount = Convert.ToInt32(textBox14.Text);//相撞数量
                    makecount = Convert.ToInt32(Convert.ToDecimal(textBox17.Text));//生产数量
                    qs_序列 = Convert.ToInt32(textBox16.Text); //起始序列
                    xc = 1;
                    if (boxcount != 0)
                    {

                        count = makecount / boxcount; //总箱数
                        ys = makecount % boxcount;//余数
                        if (makecount % boxcount != 0)
                        {
                            count++;
                        }
                    }

                    if (count == 1 && ys != 0)
                    {
                        i_箱装 = ys;
                    }
                    else
                    {
                        i_箱装 = boxcount;
                    }
                    try
                    {
                        printer_箱贴 = CPublic.Var.li_CFG["printer_chest"].ToString();
                    }
                    catch
                    {
                        throw new Exception("未配置箱贴打印机");
                    }
                    fun_打印箱贴();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void button3_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (MessageBox.Show(string.Format("确定打印第{0}箱？", textBox18.Text), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                int i = Convert.ToInt32(textBox18.Text);
                fun_打印箱贴(i);

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void textBox18_KeyPress(object sender, KeyPressEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 单独打印某箱
        /// </summary>
        private void fun_打印箱贴(int xc_e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string path = Application.StartupPath + string.Format(@"\Mode\{0}.lab", searchLookUpEdit4.EditValue.ToString());
                string qsm_e = textBox15.Text.Trim(); //起始码
                boxcount = Convert.ToInt32(textBox14.Text);
                makecount = Convert.ToInt32(Convert.ToDecimal(textBox17.Text));
                if (boxcount != 0)
                {

                    count = makecount / boxcount; //总箱数
                    ys = makecount % boxcount;
                    if (makecount % boxcount != 0)
                    {
                        count++;
                    }
                }
                //if (searchLookUpEdit4.EditValue.ToString() == "日新箱贴")
                //{
                //    List<Dictionary<string, string>> li = new List<Dictionary<string, string>>();
                //    int i_箱装;
                //    if (xc_e == 1 && ys != 0)
                //    {
                //        i_箱装 = ys;
                //    }
                //    else
                //    {
                //        i_箱装 = boxcount;
                //    }
                //    string qsm = textBox15.Text.Trim(); //起始码

                //    int qs_序列 = Convert.ToInt32(textBox16.Text); //起始序列

                //    Dictionary<string, string> dic1 = new Dictionary<string, string>();
                //    dic1.Add("xzsl", i_箱装.ToString());
                //    dic1.Add("cpmc", textBox13.Text.Trim());
                //    dic1.Add("cpxh", textBox11.Text.Trim());
                //    dic1.Add("eddy", textBox10.Text.Trim());
                //    dic1.Add("dl", textBox12.Text.Trim());
                //    dic1.Add("khmc", searchLookUpEdit5.Text.ToString());
                //    dic1.Add("qsm", qsm);
                //    int xx = qs_序列 + i_箱装 - 1;
                //    string x = qsm.Substring(0, 7) + xx.ToString().PadLeft(14, '0');
                //    qsm = qsm.Substring(0, 7) + (xx + 1).ToString().PadLeft(14, '0');//下一张的起始号

                //    dic1.Add("jsm", x);

                //    qs_序列 = xx + 1;

                //    Lprinter lp1 = new Lprinter(path, dic1, printer_箱贴, 1);
                //    lp1.DoWork();

                //}
                if (searchLookUpEdit4.EditValue.ToString() == "河南电网箱贴_36")
                {
                    // List<Dictionary<string, string>> li = new List<Dictionary<string, string>>();
                    //int i_箱装;
                    if ((xc_e == 1 && ys != 0) || (xc_e == count && ys != 0))
                    {
                        i_箱装 = ys;
                    }
                    else
                    {
                        i_箱装 = boxcount;
                    }

                    Dictionary<string, string> dic1 = new Dictionary<string, string>();
                    dic1.Add("xzsl", i_箱装.ToString());
                    dic1.Add("xc", xc_e.ToString());
                    dic1.Add("总箱数", count.ToString());

                    //dic1.Add("cpmc", textBox13.Text.Trim());
                    dic1.Add("cpxh", textBox11.Text.Trim());
                    //dic1.Add("eddy", textBox10.Text.Trim());
                    //dic1.Add("dl", textBox12.Text.Trim());
                    dic1.Add("khmc", searchLookUpEdit5.Text.ToString());
                    //qsm_e重新定义 然后 重新计算 
                    int qs_序列_e = Convert.ToInt32(qsm_e.Substring(7, 14)); //起始序列
                    int xx = qs_序列_e + (xc_e - 1) * boxcount; //第xc_e箱的起始序列
                    string x = qsm_e.Substring(0, 7) + xx.ToString().PadLeft(14, '0');//第xc_e箱的起始序列
                    //qsm = qsm.Substring(0, 7) + (xx + 1).ToString().PadLeft(14, '0');//下一张的起始号
                    string x_js = x.Substring(0, 7) + (xx + i_箱装 - 1).ToString().PadLeft(14, '0'); //第xc_e的结束码
                    dic1.Add("qsm", x);
                    dic1.Add("jsm", x_js);
                    for (int e = 1; e <= i_箱装; e++)
                    {
                        int xx_1 = xx + e - 1;
                        string x_1 = x.Substring(0, 7) + xx_1.ToString().PadLeft(14, '0');
                        dic1.Add(string.Format("zh{0}", e), x_1);
                    }
                    //qs_序列 = xx + 1;

                    Lprinter lp1 = new Lprinter(path, dic1, printer_箱贴, 1);
                    lp1.DoWork();


                }
                if (searchLookUpEdit4.EditValue.ToString() == "河南电网箱贴_24")
                {
                    if ((xc_e == 1 && ys != 0) || (xc_e == count && ys != 0))
                    {
                        i_箱装 = ys;
                    }
                    else
                    {
                        i_箱装 = boxcount;
                    }

                    Dictionary<string, string> dic1 = new Dictionary<string, string>();
                    dic1.Add("xzsl", i_箱装.ToString());
                    dic1.Add("xc", xc_e.ToString());
                    dic1.Add("总箱数", count.ToString());

                    //dic1.Add("cpmc", textBox13.Text.Trim());
                    dic1.Add("cpxh", textBox11.Text.Trim());
                    //dic1.Add("eddy", textBox10.Text.Trim());
                    //dic1.Add("dl", textBox12.Text.Trim());
                    dic1.Add("khmc", searchLookUpEdit5.Text.ToString());
                    //qsm_e重新定义 然后 重新计算 
                    int qs_序列_e = Convert.ToInt32(qsm_e.Substring(7, 14)); //起始序列
                    int xx = qs_序列_e + (xc_e - 1) * boxcount; //第xc_e箱的起始序列
                    string x = qsm_e.Substring(0, 7) + xx.ToString().PadLeft(14, '0');//第xc_e箱的起始序列
                    //qsm = qsm.Substring(0, 7) + (xx + 1).ToString().PadLeft(14, '0');//下一张的起始号
                    string x_js = x.Substring(0, 7) + (xx + i_箱装 - 1).ToString().PadLeft(14, '0'); //第xc_e的结束码
                    dic1.Add("qsm", x);
                    dic1.Add("jsm", x_js);
                    for (int e = 1; e <= i_箱装; e++)
                    {
                        int xx_1 = xx + e - 1;
                        string x_1 = x.Substring(0, 7) + xx_1.ToString().PadLeft(14, '0');
                        dic1.Add(string.Format("zh{0}", e), x_1);
                    }
                    //qs_序列 = xx + 1;

                    Lprinter lp1 = new Lprinter(path, dic1, printer_箱贴, 1);
                    lp1.DoWork();
                }
                //}
                //else
                //{
                //    throw new Exception("箱装数量为0");
                //}


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        //
#pragma warning disable IDE1006 // 命名样式
        private void button4_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_check_箱贴();

            qsm = textBox15.Text.Trim(); //起始码
            boxcount = Convert.ToInt32(textBox14.Text);//箱装数量
            makecount = Convert.ToInt32(Convert.ToDecimal(textBox17.Text));//生产数量
            qs_序列 = Convert.ToInt32(textBox16.Text); //起始序列
            xc = Convert.ToInt32(textBox18.Text);
            // int xx = qs_序列 + (xc - 1) * boxcount; //第xc箱的起始序列
            qs_序列 = qs_序列 + (xc - 1) * boxcount;
            qsm = qsm.Substring(0, 7) + qs_序列.ToString().PadLeft(14, '0');//第xc箱的起始序列
            //qsm = qsm.Substring(0, 7) + (xx + 1).ToString().PadLeft(14, '0');//下一张的起始号

            if (boxcount != 0)
            {

                count = makecount / boxcount; //总箱数
                ys = makecount % boxcount;
                if (makecount % boxcount != 0)
                {
                    count++;
                }
            }

            if (count == 1 && ys != 0)
            {
                i_箱装 = ys;
            }
            else
            {
                i_箱装 = boxcount;
            }
            try
            {
                printer_箱贴 = CPublic.Var.li_CFG["printer_chest"].ToString();
            }
            catch
            {
                throw new Exception("未配置箱贴打印机");
            }
            fun_打印箱贴();
        }

#pragma warning disable IDE1006 // 命名样式
        private void textBox9_TextChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void textBox1_TextChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            label9.Text = "参数";
            //searchLookUpEdit1.Text = "";
            //textBox1.Text = "";
            //txt_物料名称.Text = "";
            //txt_参数.Text = "";

            //cmb_jz.Text = "";
            //textBox5.Text = "";
            //searchLookUpEdit2.Text = "";
            //textBox3.Text = "";
            //txt_客户料号.Text = "";
            //txt_订单号.Text = "";

            //textBox2.Text = "";

        }

        //客户变化 
#pragma warning disable IDE1006 // 命名样式
        private void searchLookUpEdit6_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (searchLookUpEdit1.EditValue != null)
            {
                string str_kh = "";
                // string str_mbmc = "";

                if (searchLookUpEdit6.EditValue != null && searchLookUpEdit6.EditValue.ToString() != "") str_kh = searchLookUpEdit6.EditValue.ToString();
                // if (searchLookUpEdit2.EditValue != null && searchLookUpEdit2.EditValue.ToString() != "") str_kh = searchLookUpEdit2.EditValue.ToString();

                fun_盒贴(searchLookUpEdit1.EditValue.ToString(), str_kh);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (searchLookUpEdit1.EditValue != null)
            {
                DataRow[] dr = dt_物料.Select(string.Format("物料编码='{0}'", searchLookUpEdit1.EditValue.ToString()));
                textBox1.Text = dr[0]["规格型号"].ToString();


                //   textBox4.Text = dr[0]["原ERP物料编号"].ToString().Trim();
                //  txt_规格型号.Text = "";
                string str_kh = "";
                // string str_mbmc = "";

                if (searchLookUpEdit6.EditValue != null && searchLookUpEdit6.EditValue.ToString() != "") str_kh = searchLookUpEdit6.EditValue.ToString();
                // if (searchLookUpEdit2.EditValue != null && searchLookUpEdit2.EditValue.ToString() != "") str_kh = searchLookUpEdit2.EditValue.ToString();

                fun_盒贴(searchLookUpEdit1.EditValue.ToString(), str_kh);

                //if (dr[0]["产品线"].ToString() == "智能终端电器")
                //{
                // textBox5.Text = textBox1.Text; //新规格
                //if (searchLookUpEdit2.EditValue != null && searchLookUpEdit2.EditValue.ToString() == "中性模板")
                //{

                //    textBox5.Text = txt_规格型号.Text;
                //}

                //}
                //else
                //{

                // textBox5.Text = txt_规格型号.Text;

                // }
            }
            else
            {
                textBox1.Text = "";
                //txt_规格型号.Text = "";
                txt_物料名称.Text = "";
            }

        }





    }
}
