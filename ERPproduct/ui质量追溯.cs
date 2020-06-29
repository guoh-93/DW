using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class ui质量追溯 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {

        #region 变量
        string strcon = CPublic.Var.strConn;
        string str_物料编码 = "";
        string str_原物料号 = "";
        string str_订单明细号 = "";

        string str_制令单号 = "";



        #endregion
        public ui质量追溯()
        {
            InitializeComponent();
        }


        //搜索
#pragma warning disable IDE1006 // 命名样式
        private void simpleButton2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_清空();
                fun_check();
                fun_basedata();
                fun_smess();
                fun_pmess();
                fun_pickmess();
                tabControl1.SelectedTab = tabPage1;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //查看bom
#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (str_物料编码 != "")
            {
                ERPproduct.UI物料BOM详细数量 ui = new UI物料BOM详细数量(str_物料编码, 1);
                CPublic.UIcontrol.Showpage(ui, "物料BOM");
            }

        }
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 修改盒贴的label 的caption 
        /// </summary>
        /// <param name="str_盒贴名称"></param>
        private void fun_ln(string str_盒贴名称)
#pragma warning restore IDE1006 // 命名样式
        {
            //默认不能用 
            sle_19.Enabled = false;
            sle_20.Enabled = false;
            sle_21.Enabled = false;
            sle_23.Enabled = false;
            sle_24.Enabled = false;
            txt_模板.Enabled = false;
            txt_cs.Enabled = false;
            st_6.Text = "机种";
            st_24.Text = "产品型号：";
            st_29.Text = "产品名称：";

            if (str_盒贴名称 == "通用模板")
            {
                st_24.Text = "产品型号：";
                st_29.Text = "产品名称：";
                sle_19.Enabled = true;
                txt_模板.Enabled = true;
                sle_20.Enabled = true;
                sle_21.Enabled = true;
            }
            if (str_盒贴名称 == "通用模板电流")
            {
                st_24.Text = "产品型号：";
                st_29.Text = "产品名称：";
                sle_19.Enabled = true;
                txt_模板.Enabled = true;
                sle_20.Enabled = true;
                sle_21.Enabled = true;
            }
            if (str_盒贴名称 == "中性模板")
            {
                st_24.Text = "产品型号：";
                st_29.Text = "产品名称：";
                sle_19.Enabled = true;
                txt_模板.Enabled = true;
                sle_20.Enabled = true;
                sle_21.Enabled = true;
            }
            if (str_盒贴名称 == "常熟模板")
            {
                st_24.Text = "产品型号：";
                st_29.Text = "产品名称：";
                sle_19.Enabled = true;
                txt_模板.Enabled = true;
                sle_20.Enabled = true;
                sle_21.Enabled = true;
            }
            if (str_盒贴名称 == "正泰模板")
            {
                st_24.Text = "适配断路器：";
                st_29.Text = "附件名称：";
                sle_19.Enabled = true;
                txt_模板.Enabled = true;
                txt_cs.Enabled = true;
                sle_20.Enabled = true;
                sle_21.Enabled = true;
            }
            if (str_盒贴名称 == "宁波施耐德")
            {
                st_24.Text = "型号规格：";
                st_29.Text = "产品名称：";
                sle_19.Enabled = true;
                sle_21.Enabled = true;
            }
            if (str_盒贴名称 == "温州德力西")
            {
                st_24.Text = "零部件名称：";
                st_29.Text = "零部件编码：";
                sle_19.Enabled = true;
                sle_21.Enabled = true;
            }
            if (str_盒贴名称 == "台安模板")
            {
                st_24.Text = "型号：";
                st_29.Text = "品名：";
                sle_19.Enabled = true;
                sle_21.Enabled = true;
                sle_20.Enabled = true;
                sle_23.Enabled = true;
                sle_24.Enabled = true;
                txt_模板.Enabled = true;
            }
            if (str_盒贴名称 == "诺雅克模板")
            {
                st_24.Text = "规格型号：";
                st_29.Text = "物料名称：";
                sle_19.Enabled = true;
                sle_21.Enabled = true;
                sle_20.Enabled = true;
                txt_模板.Enabled = true;
            }
            if (str_盒贴名称 == "分励英文模板")
            {
                st_24.Text = "规格型号：";
                st_29.Text = "物料名称：";
                sle_19.Enabled = true;
                sle_21.Enabled = true;
                txt_模板.Enabled = true;
            }
            if (str_盒贴名称 == "闭合英文模板")
            {
                st_24.Text = "规格型号：";
                st_29.Text = "物料名称：";
                sle_19.Enabled = true;
                sle_21.Enabled = true;
                txt_模板.Enabled = true;
            }
            if (str_盒贴名称 == "欠压英文模板")
            {
                st_24.Text = "规格型号：";
                st_29.Text = "物料名称：";
                sle_19.Enabled = true;
                sle_21.Enabled = true;
                txt_模板.Enabled = true;
            }
            if (str_盒贴名称 == "辅助英文模板")
            {
                st_24.Text = "规格型号：";
                st_29.Text = "物料名称：";
                sle_19.Enabled = true;
                sle_21.Enabled = true;
                txt_模板.Enabled = true;
            }
            if (str_盒贴名称 == "辅报英文模板")
            {
                st_24.Text = "规格型号：";
                st_29.Text = "物料名称：";
                sle_19.Enabled = true;
                sle_21.Enabled = true;
                txt_模板.Enabled = true;
            }
            if (str_盒贴名称 == "报警英文模板")
            {
                st_24.Text = "规格型号：";
                st_29.Text = "物料名称：";
                sle_19.Enabled = true;
                sle_21.Enabled = true;
                txt_模板.Enabled = true;
            }
            if (str_盒贴名称 == "芜湖德力西")
            {
                st_24.Text = "规格型号：";
                st_29.Text = "物料名称：";
                st_6.Text = "对方型号：";
                sle_19.Enabled = true;
                sle_21.Enabled = true;
                sle_20.Enabled = true;
                sle_23.Enabled = true;
            }
            if (str_盒贴名称 == "芜湖德力西英文")
            {
                st_24.Text = "规格型号：";
                st_29.Text = "物料名称：";
                st_6.Text = "对方型号：";
                sle_19.Enabled = true;
                sle_21.Enabled = true;
                sle_20.Enabled = true;
                sle_23.Enabled = true;
            }
            if (str_盒贴名称 == "宏美模板")
            {
                st_24.Text = "规格型号：";
                st_29.Text = "物料名称：";
                st_6.Text = "LOT/SN";
                sle_19.Enabled = true;
                sle_21.Enabled = true;
                sle_20.Enabled = true;
                sle_23.Enabled = true;
                sle_24.Enabled = true;
            }
            if (str_盒贴名称 == "正泰英文版")
            {
                st_24.Text = "型号规格：";
                st_29.Text = "产品名称：";
                sle_19.Enabled = true;
                sle_21.Enabled = true;
            }
            if (str_盒贴名称 == "常熟外发模板")
            {
                st_24.Text = "型号规格：";
                st_29.Text = "产品名称：";
                sle_19.Enabled = true;
                sle_21.Enabled = true;
            }
            if (str_盒贴名称 == "良信模板")
            {
                st_24.Text = "规格";
                st_29.Text = "品名";
                sle_19.Enabled = true;
                txt_模板.Enabled = true;
                sle_20.Enabled = true;
                sle_21.Enabled = true;
            }

        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_inf()
#pragma warning restore IDE1006 // 命名样式
        {

            DataTable dt = ERPorg.Corg.fun_客户("停用=0 and  客户编号 in (select  客户编号  from 销售记录销售订单主表  group by 客户编号)");
         searchLookUpEdit1.Properties.DataSource = dt;
         searchLookUpEdit1.Properties.DisplayMember = "客户名称";
         searchLookUpEdit1.Properties.ValueMember ="客户编号";
         string sql = string.Format(@"select  b.物料编码,b.原ERP物料编号,b.n原ERP规格型号,b.规格,大类,小类  from 
                                (select  物料编码  from 生产记录生产工单表 group  by 物料编码)a,基础数据物料信息表 b where   a.物料编码=b.物料编码 ");
         DataTable tt = new DataTable();
         using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
         {
             da.Fill(tt);
             searchLookUpEdit2.Properties.DataSource = tt;
             searchLookUpEdit2.Properties.DisplayMember = "原ERP物料编号";
             searchLookUpEdit2.Properties.ValueMember = "物料编码";
         }
          sql = string.Format(@"select  物料编码,原ERP物料编号, n原ERP规格型号,规格,大类,小类  from 
                                 基础数据物料信息表   where  停用=0 and 物料类型<>'成品'  ");
         DataTable t_w = new DataTable();
         using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
         {
             da.Fill(t_w);
             searchLookUpEdit5.Properties.DataSource = t_w;
             searchLookUpEdit5.Properties.DisplayMember = "原ERP物料编号";
             searchLookUpEdit5.Properties.ValueMember = "物料编码";
         }




        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_盒贴(string str_合贴_物料编码)
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format("select * from BQ_HZXX where wlbh = '{0}'", str_合贴_物料编码);
            DataTable dt = new DataTable();
            SqlDataAdapter daM = new SqlDataAdapter(sql, strcon);
            daM.Fill(dt);

            if (dt.Rows.Count != 0)
            {
                DataRow dr = dt.Rows[0];
                fun_ln(dt.Rows[0]["mbmc"].ToString().Trim());

                dataBindHelper1.DataFormDR(dt.Rows[0]);
                if (st_6.Text.ToString() != "机种" && st_6.Text.ToString() != "LOT/SN")
                {
                    sle_23.Text = dt.Rows[0]["ggxh"].ToString();
                }
                else
                {
                    sle_23.Text = dt.Rows[0]["jz"].ToString();
                }
            }

        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_basedata()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format(@"select  原ERP物料编号,n原ERP规格型号,规格,仓库名称,货架描述,车间,产品线,大类,小类
                                            from 基础数据物料信息表 where 物料编码='{0}'", str_物料编码);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataBindHelper1.DataFormDR(dt.Rows[0]);
            }
            string sql_bzqd = string.Format(@"select a.*,原ERP物料编号 ,n原ERP规格型号 from 基础数据包装清单表 a
                                           left  join  基础数据物料信息表 b  on b.物料编码=a.物料编码
                                           where 成品编码 ='{0}'", str_物料编码);
            using (SqlDataAdapter da = new SqlDataAdapter(sql_bzqd, strcon))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                gc_包装.DataSource = dt;
            }
            fun_盒贴(str_原物料号);

        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {

            str_制令单号 ="";
            str_物料编码 = "";
            str_原物料号 = "";
            str_订单明细号 = "";
            if (checkBox1.Checked == true)
            {
                 
                string sql = string.Format(@"select  基础数据物料信息表.原ERP物料编号,基础数据物料信息表.物料编码,基础数据物料信息表.大类,基础数据物料信息表.小类,车间,产品线
                              from 销售记录销售订单明细表 left join 基础数据物料信息表  on 销售记录销售订单明细表.物料编码 = 基础数据物料信息表.物料编码   
                              where 销售记录销售订单明细表.作废 = 0  and 销售订单明细号='{0}'", textBox2.Text.ToString());
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        str_物料编码 = dt.Rows[0]["物料编码"].ToString();
                        str_订单明细号 = textBox2.Text.ToString();
                        str_原物料号 = dt.Rows[0]["原ERP物料编号"].ToString();
                    }
                    else
                    {

                        throw new Exception("未找到该销售明细");
                    }
                }

                sql = string.Format("select  * from 生产记录生产制令子表 where 销售订单明细号='{0}'", textBox2.Text.ToString());
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        str_制令单号 = dt.Rows[0]["生产制令单号"].ToString();

                    }
                }
            }
            if (checkBox6.Checked == true)
            {
                
                string sql = string.Format(@"select  a.生产制令单号,a.物料编码,b.原ERP物料编号
                                    from 生产记录生产工单表 a,基础数据物料信息表 b  where  a.物料编码=b.物料编码 and 生产工单号='{0}'", textBox1.Text.ToString());
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        str_制令单号 = dt.Rows[0]["生产制令单号"].ToString();
                        str_物料编码 = dt.Rows[0]["物料编码"].ToString();
                        str_原物料号 = dt.Rows[0]["原ERP物料编号"].ToString();
                    }
                    else
                    {

                        throw new Exception("未找到该工单所对应制令");
                    }
                }
                sql = string.Format("select  * from 生产记录生产制令子表 where 生产制令单号='{0}'", str_制令单号);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {

                        str_订单明细号 = dt.Rows[0]["销售订单明细号"].ToString();

                    }
                }

            }
            if (checkBox2.Checked == true)
            {
                //if (textBox13.Text.Length <10)
                //{
                //    throw new Exception("请确认输入是否有误");
                //}
                string ss = "MO"+textBox13.Text.Substring(0,10);
                string sql = string.Format(@"select  a.生产制令单号,a.物料编码,b.原ERP物料编号
                                    from 生产记录生产工单表 a,基础数据物料信息表 b  where  a.物料编码=b.物料编码 and 生产工单号='{0}'", ss);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        str_制令单号 = dt.Rows[0]["生产制令单号"].ToString();
                        str_物料编码 = dt.Rows[0]["物料编码"].ToString();
                        str_原物料号 = dt.Rows[0]["原ERP物料编号"].ToString();
                    }
                    else
                    {

                        throw new Exception("请确认输入是否有误");
                    }
                }
                sql = string.Format("select  * from 生产记录生产制令子表 where 生产制令单号='{0}'", str_制令单号);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {

                        str_订单明细号 = dt.Rows[0]["销售订单明细号"].ToString();

                    }
                }

            }

            if (checkBox1.Checked == false && checkBox6.Checked == false && checkBox2.Checked == false)
            {
                
                    throw new Exception("未勾选筛选条件");
                
            }
           


        }
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 销售相关信息
        /// </summary>
        private void fun_smess()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format(@"select 销售记录销售订单明细表.*,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.大类,基础数据物料信息表.小类,车间,产品线,录入人员 
                              from 销售记录销售订单明细表 left join 基础数据物料信息表  on 销售记录销售订单明细表.物料编码 = 基础数据物料信息表.物料编码   
                              left  join 销售记录销售订单主表   on 销售记录销售订单主表.销售订单号=销售记录销售订单明细表.销售订单号
                              where 销售记录销售订单明细表.作废 = 0  and 销售订单明细号='{0}' and 销售记录销售订单明细表.生效=1", str_订单明细号);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                gc_明细.DataSource = dt;
            }

            sql = string.Format(@"select 销售记录成品出库单明细表.*,操作员 as 出库人员,基础数据物料信息表.原ERP物料编号 from 销售记录成品出库单明细表 
                left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 销售记录成品出库单明细表.物料编码 
                left  join 销售记录成品出库单主表   on 销售记录成品出库单明细表.成品出库单号 =销售记录成品出库单主表.成品出库单号 
                where 销售订单明细号 = '{0}'   and 销售记录成品出库单明细表.作废=0", str_订单明细号);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dt1 = new DataTable();
                da.Fill(dt1);
                gcP.DataSource = dt1;
            }
            sql = string.Format(@"select 销售记录销售开票明细表.*,开票员,销售订单明细号,销售记录成品出库单明细表.客户,原ERP物料编号,基础数据物料信息表.n原ERP规格型号 
                              from 销售记录销售开票明细表,销售记录成品出库单明细表,基础数据物料信息表,销售记录销售开票主表 
                              where 销售记录销售开票明细表.成品出库单明细号=销售记录成品出库单明细表.成品出库单明细号 and  销售记录销售开票明细表.销售开票通知单号 =销售记录销售开票主表.销售开票通知单号
                              and 基础数据物料信息表.物料编码=销售记录销售开票明细表.产品编码 and 销售订单明细号 = '{0}'", str_订单明细号);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dt2 = new DataTable();
                da.Fill(dt2);
                gridControl1.DataSource = dt2;
            }
            DateTime time = CPublic.Var.getDatetime();
            time = time.AddMonths(-6);
            time = new DateTime(time.Year, time.Month, time.Day);
            sql = string.Format(@"select 销售记录成品出库单明细表.*,操作员 as 出库人员,基础数据物料信息表.原ERP物料编号 from 销售记录成品出库单明细表 
                left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 销售记录成品出库单明细表.物料编码 and 作废=0
                left join 销售记录成品出库单主表  on 销售记录成品出库单主表.成品出库单号 = 销售记录成品出库单明细表.成品出库单号  
                where 基础数据物料信息表.物料编码 = '{0}' and 销售记录成品出库单明细表.生效日期>'{1}'", str_物料编码, time);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dt3 = new DataTable();
                da.Fill(dt3);
                gridControl2.DataSource = dt3;
            }
        }
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 生产相关信息
        /// </summary>
        private void fun_pmess()
#pragma warning restore IDE1006 // 命名样式
        {

            string sql = string.Format(@"                       
                  select 生产记录生产制令表.*,a.已转工单数,基础数据物料信息表.原ERP物料编号 from 生产记录生产制令表
                   left join 基础数据物料信息表 on   生产记录生产制令表.物料编码=基础数据物料信息表.物料编码
                  left join (select sum(生产数量) as 已转工单数,生产制令单号  from  生产记录生产工单表 where 关闭= 0  group by 生产制令单号) a  
                   on 生产记录生产制令表.生产制令单号=a.生产制令单号
                  where  生产记录生产制令表.关闭=0  and 生产记录生产制令表.生效 = 1   and 生产记录生产制令表.生产制令单号='{0}'", str_制令单号);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                gc_制令.DataSource = dt;
            }

            sql = string.Format(@"select 生产记录生产工单表.* ,基础数据物料信息表.[原ERP物料编号]  from 生产记录生产工单表 
                left join   基础数据物料信息表 on 基础数据物料信息表.物料编码=生产记录生产工单表.物料编码  
                where    生产记录生产工单表.关闭=0   and 生产制令单号='{0}'", str_制令单号);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dt1 = new DataTable();
                da.Fill(dt1);
                gc_工单.DataSource = dt1;
            }
            sql = string.Format(@"select 生产记录生产检验单主表.*,生产制令单号,基础数据物料信息表.原ERP物料编号 from 生产记录生产检验单主表
             left join 基础数据物料信息表 on  基础数据物料信息表.物料编码=生产记录生产检验单主表.物料编码  
             left join 生产记录生产工单表 on 生产记录生产工单表.生产工单号=生产记录生产检验单主表.生产工单号
             where  生产制令单号 ='{0}'", str_制令单号);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dt2 = new DataTable();
                da.Fill(dt2);
                gc_check.DataSource = dt2;
            }
            sql = string.Format(@"select 生产记录成品入库单明细表.*,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.n原ERP规格型号,生产制令单号 from 生产记录成品入库单明细表 
				left join 生产记录生产工单表 on 生产记录生产工单表.生产工单号=生产记录成品入库单明细表.生产工单号
                left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录成品入库单明细表.物料编码
                where 生产制令单号='{0}'", str_制令单号);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dt3 = new DataTable();
                da.Fill(dt3);
                gc_rk.DataSource = dt3;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_pickmess()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format(@"select 生产记录生产领料单主表.*,生产记录生产工单待领料主表.领料类型 as 类型,基础数据物料信息表.[原ERP物料编号],基础数据物料信息表.n原ERP规格型号 from 生产记录生产领料单主表
         left join  生产记录生产工单待领料主表 on 生产记录生产工单待领料主表.待领料单号=生产记录生产领料单主表.待领料单号
         left join 基础数据物料信息表 on  基础数据物料信息表.物料编码=生产记录生产领料单主表.物料编码 where 生产记录生产领料单主表.生产制令单号='{0}' and 生产记录生产领料单主表.生产工单号<>''", str_制令单号);
             using (SqlDataAdapter da =new SqlDataAdapter (sql,strcon))
              {
                 DataTable dt  =new DataTable ();
                 da.Fill(dt);
                 gc_领料.DataSource=dt;
               }
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_cgrecord()
#pragma warning restore IDE1006 // 命名样式
        {
            DateTime time = CPublic.Var.getDatetime();
            time =time.AddMonths(-6);
            time =new DateTime (time.Year,time.Month,time.Day);
            string sql = string.Format(@"select  a.采购单明细号,a.入库明细号,a.检验记录单号,原ERP物料编号,d.物料名称,d.图纸编号,d.n原ERP规格型号,d.仓库名称,检验人,入库人员,c.生效人员 as 采购员,入库量 
            ,d.计量单位,a.供应商,a.生效日期 as 入库日期 from  采购记录采购单入库明细 a left join 采购记录采购单检验主表 b on b.检验记录单号 =a.检验记录单号
            left join  采购记录采购单明细表 c on c.采购明细号=a.采购单明细号 
            left join  基础数据物料信息表  d on d.物料编码=a.物料编码 
            where a.物料编码='{0}' and a.生效日期>'{1}'", searchLookUpEdit5.EditValue,time);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dt =new DataTable ();
                da.Fill(dt);
                gridControl3.DataSource = dt;
            }


        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_父项()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format(@"exec parbom '{0}'", searchLookUpEdit5.EditValue);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            da.Fill(dt);
            gc_BOM.DataSource = dt;

        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_清空()
#pragma warning restore IDE1006 // 命名样式
        {
            foreach (Control ctr in tabPage1.Controls)
            {
                if (ctr is TextBox)
                {
                    ctr.Text = "";
                }
            }
            //gc_包装.DataSource = null;
            //gc_明细.DataSource = null;
            //gcP.DataSource = null;
            //gridControl1.DataSource = null;  
            //gc_制令.DataSource = null;
            //gc_工单.DataSource = null;
            //gc_check.DataSource = null;
            //gc_rk.DataSource = null;
            //gc_领料.DataSource = null;
            //gc_llmx.DataSource = null;
        }
        
#pragma warning disable IDE1006 // 命名样式
        private void gv_工单_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_check_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_rk_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void gridView12_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_BOM_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void gvP_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void gv_包装_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_领料_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr =gv_领料.GetDataRow (gv_领料.FocusedRowHandle);
            string sql = string.Format(@"select 生产记录生产领料单明细表.*,基础数据物料信息表.仓库名称,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.n原ERP规格型号
            ,基础数据物料信息表.货架描述 from 生产记录生产领料单明细表,基础数据物料信息表 
            where 生产记录生产领料单明细表.物料编码=基础数据物料信息表.物料编码 and  领料出库单号='{0}'", dr["领料出库单号"]);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                gc_llmx.DataSource = dt;
            }


        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_领料_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_llmx_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void 查看检验记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage3)
            {
                DataRow dr = gv_check.GetDataRow(gv_check.FocusedRowHandle);
                frm成品检验_视图 ui = new frm成品检验_视图(dr["生产检验单号"].ToString(), true);
                CPublic.UIcontrol.Showpage(ui, "成品检验记录");
            }
            else
            {
             
                DataRow dr = gridView12.GetDataRow(gridView12.FocusedRowHandle);
                ItemInspection.print_Check.fun_print_Check(dr["检验记录单号"].ToString(), true);

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_check_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc_check, new Point(e.X, e.Y));
                gv_check.CloseEditor();
                查看采购信息ToolStripMenuItem.Visible = false;
                查看检验记录ToolStripMenuItem.Visible = true;

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView6_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (searchLookUpEdit1.EditValue != null )
            {
                DateTime t = CPublic.Var.getDatetime();
                t = t.AddYears(-1);
                t = new DateTime(t.Year, t.Month, t.Day);
                string sql = string.Format(@"select   a.销售订单明细号,b.物料编码,数量,b.规格型号,b.物料名称 from 销售记录销售订单明细表 a,基础数据物料信息表 b  
	          where 客户编号='{0}' and a.物料编码=b.物料编码 and 生效日期>'{1}' and 作废=0 and a.关闭=0", searchLookUpEdit1.EditValue, t);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    searchLookUpEdit3.Properties.DataSource = dt;
                    searchLookUpEdit3.Properties.DisplayMember = "销售订单明细号";
                    searchLookUpEdit3.Properties.ValueMember = "销售订单明细号";
                }
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void searchLookUpEdit3_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (searchLookUpEdit3.EditValue != null )
            {
                textBox2.Text = searchLookUpEdit3.EditValue.ToString();
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (searchLookUpEdit2.EditValue != null)
            {
                DateTime t = CPublic.Var.getDatetime();
                t = t.AddYears(-1);
                t = new DateTime(t.Year, t.Month, t.Day);
                string sql = string.Format(@"   select   a.生产工单号,b.物料编码,生产数量,b.规格型号,b.物料名称   from 生产记录生产工单表 a,基础数据物料信息表 b  
	 where   a.物料编码=b.物料编码 and a.关闭=0  and a.物料编码='{0}' and a.生效日期>'{1}' ", searchLookUpEdit2.EditValue,t);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    searchLookUpEdit4.Properties.DataSource = dt;
                    searchLookUpEdit4.Properties.DisplayMember = "生产工单号";
                    searchLookUpEdit4.Properties.ValueMember = "生产工单号";
                    
                }
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void searchLookUpEdit4_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (searchLookUpEdit4.EditValue != null)
            {
                textBox1.Text = searchLookUpEdit4.EditValue.ToString();
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void ui质量追溯_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_inf();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton3_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (searchLookUpEdit5.EditValue != null &&  searchLookUpEdit5.EditValue.ToString() != "")
                {
                    fun_父项();
                    fun_cgrecord();
                    
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_llmx_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc_llmx, new Point(e.X, e.Y));
                查看检验记录ToolStripMenuItem.Visible = false;
                查看采购信息ToolStripMenuItem.Visible = true;
                gv_check.CloseEditor();
            }
        }

        private void 查看采购信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv_llmx.GetDataRow(gv_llmx.FocusedRowHandle);
                searchLookUpEdit5.EditValue = dr["物料编码"].ToString();
                tabControl1.SelectTab(tabPage5);

                simpleButton3_Click(null, null);

            }
            catch (Exception)
            {
                
                throw;
            }
          

        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView12_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gridControl3, new Point(e.X, e.Y));
                查看采购信息ToolStripMenuItem.Visible = true;
                查看检验记录ToolStripMenuItem.Visible = true;
                gv_check.CloseEditor();
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (checkBox1.Checked == true)
            {
                checkBox2.Checked = false;
                checkBox6.Checked = false;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (checkBox6.Checked == true)
            {
                checkBox1.Checked = false;
                checkBox2.Checked = false;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (checkBox2.Checked == true)
            {
                checkBox1.Checked = false;
                checkBox6.Checked = false;
            }
        }

   

       

    }
}
