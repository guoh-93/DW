using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace BaseData
{
    public partial class frm成品检验盒贴信息维护 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        string str_合贴模板 = "";
        DataTable dtM = null;
        DataTable dtP = null;
        DataTable dt_math;
        public static DevExpress.XtraTab.XtraTabControl XTC;
        string str_成品编码 = "";
        string str_成品名称 = "";    
        Boolean bl = false;
        Boolean flag = false; //指示修改还是新增  新增为true

        #endregion

        #region 自用类
        public frm成品检验盒贴信息维护()
        {
            InitializeComponent();
        }

        public frm成品检验盒贴信息维护(string str, string strr)
        {
            InitializeComponent();
            str_成品编码 = str;
            str_成品名称 = strr;
            bl = true;
        }

        private void frm成品检验盒贴信息维护_Load(object sender, EventArgs e)
        {
            try
            {
                label1.Text = "";
                fun_载入代办();
                fun_盒贴名称下拉框();
                //默认不能用 
                sle_19.Enabled = false;
                sle_20.Enabled = false;
                sle_21.Enabled = false;
                sle_23.Enabled = false;
                sle_24.Enabled = false;
                //ddlb_1.Enabled = false;
                ddlb_3.Enabled = false;
                if (bl == true)
                {
                    label1.Text = string.Format("当前产品为：{0}{1}", str_成品编码, str_成品名称);
                  fun_载入合贴模板(str_成品编码);
              //      dr["wlbh"] = str_成品编码;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gvP_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow r = gvP.GetDataRow(gvP.FocusedRowHandle);
                if (r != null)
                {
                    string s = string.Format(@" select BQ_HZXX.*,客户名称 from BQ_HZXX 
                                        left join 客户基础信息表 kh  on kh.客户编号=BQ_HZXX.khbh  where 1<>1");
                    dtM = CZMaster.MasterSQL.Get_DataTable(s, strconn);

                    label1.Text = string.Format("当前产品为：{0}{1}", r["物料编码"].ToString().Trim(), r["物料名称"].ToString().Trim());
                    fun_载入合贴模板(r["物料编码"].ToString());
                    //dr["cpmc"] = r["物料名称"];
                   // if(dr!=null)   dr["wlbh"] = r["物料编码"].ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ddlb_2_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                fun_盒贴信息(ddlb_2.EditValue.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 方法
        private void fun_载入代办()
        {
            string sql = string.Format(" select  * from 基础数据物料信息表  where left(存货分类编码,2)='10' ");
            dtP = new DataTable();
            SqlDataAdapter da_属性 = new SqlDataAdapter(sql, strconn);
            da_属性.Fill(dtP);
            //DataRow dr = dtP.NewRow();
            //dtP.Rows.Add(dr);
            //dr["物料编码"] = "03342";
            //((DevExpress.XtraEditors.Repository.RepositoryItemComboBox)this.bar_盒贴模板.Edit).Items.Add(r["属性值"].ToString());
            gcP.DataSource = dtP;
            
        }
        DataTable dt_khmb;
        private   void  fun_载入合贴模板(string str_合贴_物料编码)
        {
            //wlbh,mbmc,id,客户名称,客户编号
            string sql = string.Format(@"select *  from BQ_HZXX 
                                        left join 客户基础信息表 kh  on kh.客户编号=BQ_HZXX.khbh
                                                    where wlbh = '{0}'", str_合贴_物料编码);

            dt_khmb = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gridControl1.DataSource = dt_khmb;
     
            flag = false;
            //dtM = new DataTable();
            //SqlDataAdapter daM = new SqlDataAdapter(sql, strconn);
            //daM.Fill(dtM);
            //if (dtM.Rows.Count != 0)
            //{
            //    dr = dtM.Rows[0];
            //    fun_盒贴信息(dtM.Rows[0]["mbmc"].ToString().Trim());
            //    dataBindHelper1.DataFormDR(dtM.Rows[0]);
            //    if (st_6.Text.ToString() != "机种" && st_6.Text.ToString() != "LOT/SN")
            //    {
            //        sle_23.Text = dtM.Rows[0]["ggxh"].ToString().Trim();
            //    }
            //    else
            //    {
            //        sle_23.Text = dtM.Rows[0]["jz"].ToString().Trim();
            //    }
            //}
            //else
            //{
            // ddlb_1.EditValue = "";
            ddlb_2.EditValue = ""; //模板名称
            ddlb_3.EditValue = ""; //参数
            searchLookUpEdit1.EditValue = null;
            sle_19.Text = "";
            sle_20.Text = "";
            sle_21.Text = "";
            sle_23.Text = "";
            sle_24.Text = "";
            sle_4.Text = "";
            //默认不能用 
            sle_19.Enabled = false;
            sle_20.Enabled = false;
            sle_21.Enabled = false;
            sle_23.Enabled = false;
            sle_24.Enabled = false;
            // ddlb_1.Enabled = false;
            ddlb_3.Enabled = false;
            //   // MessageBox.Show("不存在该产品的盒贴模板");
            //    dr = dtM.NewRow();
            //    dtM.Rows.Add(dr);
            //}
           // return dt_khmb;
        }
        DataRow dr;
        private void fun_盒贴名称下拉框()
        {
            string sql = string.Format("select * from 基础数据基础属性表 where 属性类别 = '{0}' or 属性类别 = '{1}'", "盒贴模板", "盒贴参数");
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt);
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow r in dt.Rows)
                {
                    if (r["属性类别"].ToString() == "盒贴模板")
                    {
                        ddlb_2.Properties.Items.Add(r["属性值"].ToString());
                    }
                    //if (r["属性类别"].ToString() == "盒贴模板电压")
                    //{
                    //    ddlb_1.Properties.Items.Add(r["属性值"].ToString());
                    //}
                    if (r["属性类别"].ToString() == "盒贴参数")
                    {
                        ddlb_3.Properties.Items.Add(r["属性值"].ToString());
                    }
                }
            }
            else
            {
                throw new Exception("无数据");
            }
            sql = "select  客户编号,客户名称 from  客户基础信息表 where 停用=0 ";
            DataTable  dt_客户 = new DataTable();
            dt_客户 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

            searchLookUpEdit1.Properties.DataSource = dt_客户;
            searchLookUpEdit1.Properties.ValueMember = "客户编号";
            searchLookUpEdit1.Properties.DisplayMember = "客户名称";

        }

        private void fun_盒贴信息(string str_盒贴名称)
        {
            //默认不能用 
            sle_19.Enabled = false;
            sle_20.Enabled = false;
            sle_21.Enabled = false;
            sle_23.Enabled = false;
            sle_24.Enabled = false;
            //ddlb_1.Enabled = false;
            ddlb_3.Enabled = false;
            st_6.Text = "机种";
            st_24.Text = "产品型号：";
            st_29.Text = "产品名称：";
            if (str_盒贴名称 == "工行盒贴")
            {
                st_24.Text = "产品型号：";
                st_29.Text = "产品名称：";
                st_6.Text = "机型：";
                st_7.Text = "参数";
                st_26.Text = "客户料号";
                sle_23.Enabled = true;
                sle_19.Enabled = true;
                ddlb_3.Enabled = true;
                sle_20.Enabled = true;
                sle_21.Enabled = true;
            }


            if (str_盒贴名称 == "通用盒贴")
            {
                st_26.Text = "客户料号";
                st_24.Text = "产品型号：";
                st_29.Text = "产品名称：";
                st_6.Text = "机型：";
                st_7.Text = "收货单位";
                sle_23.Enabled = true;
                sle_19.Enabled = true;
                ddlb_3.Enabled = true;
                sle_20.Enabled = true;
                sle_21.Enabled = true;

            }
            if (str_盒贴名称 == "标配通用箱贴")
            {
                st_26.Text = "客户料号";
                st_24.Text = "产品型号：";
                st_29.Text = "产品名称：";
                st_6.Text = "机型：";
                st_7.Text = "收货单位";

                sle_23.Enabled = true;
                sle_19.Enabled = true;
                ddlb_3.Enabled = true;
                sle_20.Enabled = true;
                sle_21.Enabled = true;

            }
            if (str_盒贴名称 == "广电运通-成都农商行外箱标贴")
            {
                st_26.Text = "客户料号";
                    st_24.Text = "产品型号：";
                    st_29.Text = "产品名称：";
                    st_6.Text = "机型：";
                    st_7.Text = "参数";

                    sle_23.Enabled = true;
                    sle_19.Enabled = true;
                    ddlb_3.Enabled = true;
                    sle_20.Enabled = true;
                    sle_21.Enabled = true;
 
                }


            if (str_盒贴名称 == "怡化-建行盒贴")
            {
                st_26.Text = "客户料号";
                st_24.Text = "产品型号：";
                st_29.Text = "产品名称：";
                st_6.Text = "机型：";
                st_7.Text = "参数";
                sle_23.Enabled = true;
                sle_19.Enabled = true;
                ddlb_3.Enabled = true;
                sle_20.Enabled = true;
                sle_21.Enabled = true;


            }
            if (str_盒贴名称 == "怡化-建行外箱贴")
            {
                st_26.Text = "客户料号";
                st_24.Text = "产品型号：";
                st_29.Text = "产品名称：";
                st_6.Text = "机型：";
                st_7.Text = "参数";
                sle_23.Enabled = true;
                sle_19.Enabled = true;
                ddlb_3.Enabled = true;
                sle_20.Enabled = true;
                sle_21.Enabled = true;
            }






            if (str_盒贴名称 == "日立-工行盒贴")
            {
                st_26.Text = "客户料号";
                    st_24.Text = "产品型号：";
                    st_29.Text = "产品名称：";
                    st_6.Text = "机型：";
                    st_7.Text = "参数";
                    sle_23.Enabled = true;
                    sle_19.Enabled = true;
                    ddlb_3.Enabled = true;
                    sle_20.Enabled = true;
                    sle_21.Enabled = true;
                }
            if (str_盒贴名称 == "日立-工行外箱贴")
            {
                st_26.Text = "客户料号";
                    st_24.Text = "产品型号：";
                    st_29.Text = "产品名称：";
                    st_6.Text = "机型：";
                    st_7.Text = "参数";
                    sle_23.Enabled = true;
                    sle_19.Enabled = true;
                    ddlb_3.Enabled = true;
                    sle_20.Enabled = true;
                    sle_21.Enabled = true;
                }
            if (str_盒贴名称 == "威海新北洋盒贴")
            {
                st_26.Text = "客户料号";
                    st_24.Text = "产品型号：";
                    st_29.Text = "产品名称：";
                    st_6.Text = "机型：";
                    st_7.Text = "参数";
                    sle_23.Enabled = true;
                    sle_19.Enabled = true;
                    ddlb_3.Enabled = true;
                    sle_20.Enabled = true;
                    sle_21.Enabled = true;


                }
            if (str_盒贴名称 == "怡化-江苏银行盒贴")
            {
                st_26.Text = "客户料号";
                    st_24.Text = "产品型号：";
                    st_29.Text = "产品名称：";
                    st_6.Text = "机型：";
                    st_7.Text = "参数";
                    sle_23.Enabled = true;
                    sle_19.Enabled = true;
                    ddlb_3.Enabled = true;
                    sle_20.Enabled = true;
                    sle_21.Enabled = true;
                }
            if (str_盒贴名称 == "怡化-江苏银行外箱标贴")
            {
                st_26.Text = "客户料号";
                st_24.Text = "产品型号：";
                st_29.Text = "产品名称：";
                st_6.Text = "机型：";
                st_7.Text = "参数";
                sle_23.Enabled = true;
                sle_19.Enabled = true;
                ddlb_3.Enabled = true;
                sle_20.Enabled = true;
                sle_21.Enabled = true;
                }
            if (str_盒贴名称 == "怡化-农行盒贴")
            {
                st_26.Text = "客户料号";
                    st_24.Text = "产品型号：";
                    st_29.Text = "产品名称：";
                    st_6.Text = "机型：";
                    st_7.Text = "参数";
                    sle_23.Enabled = true;
                    sle_19.Enabled = true;
                    ddlb_3.Enabled = true;
                    sle_20.Enabled = true;
                    sle_21.Enabled = true;
                
                }
            if (str_盒贴名称 == "怡化-农行外箱贴")
            {
                st_26.Text = "客户料号";
                    st_24.Text = "产品型号：";
                    st_29.Text = "产品名称：";
                    st_6.Text = "机型：";
                    st_7.Text = "参数";
                    sle_23.Enabled = true;
                    sle_19.Enabled = true;
                    ddlb_3.Enabled = true;
                    sle_20.Enabled = true;
                    sle_21.Enabled = true;
                }
            if (str_盒贴名称 == "怡化-天府盒贴")
            {
                st_26.Text = "客户料号";
                    st_24.Text = "产品型号：";
                    st_29.Text = "产品名称：";
                    st_6.Text = "机型：";
                    st_7.Text = "参数";
                    sle_23.Enabled = true;
                    sle_19.Enabled = true;
                    ddlb_3.Enabled = true;
                    sle_20.Enabled = true;
                    sle_21.Enabled = true;
                }

            if (str_盒贴名称 == "怡化-天府银行外箱贴")
            {
                st_26.Text = "客户料号";
                    st_24.Text = "产品型号：";
                    st_29.Text = "产品名称：";
                    st_6.Text = "机型：";
                    st_7.Text = "参数";
                    sle_23.Enabled = true;
                    sle_19.Enabled = true;
                    ddlb_3.Enabled = true;
                    sle_20.Enabled = true;
                    sle_21.Enabled = true;
                }
            if (str_盒贴名称 == "广电运通-成都农商行盒贴")
            {
                st_26.Text = "客户料号";
                    st_24.Text = "产品型号：";
                    st_29.Text = "产品名称：";
                    st_6.Text = "机型：";
                    st_7.Text = "参数";
                    sle_23.Enabled = true;
                    sle_19.Enabled = true;
                    ddlb_3.Enabled = true;
                    sle_20.Enabled = true;
                    sle_21.Enabled = true;
                }

            if (str_盒贴名称 == "深圳赞融-招行盒贴")
            {
                st_26.Text = "客户料号";
                st_24.Text = "产品型号：";
                st_29.Text = "产品名称：";
                st_6.Text = "机型：";
                st_7.Text = "参数";
                sle_23.Enabled = true;
                sle_19.Enabled = true;
                ddlb_3.Enabled = true;
                sle_20.Enabled = true;
                sle_21.Enabled = true;
            }

            if (str_盒贴名称 == "深圳赞融-招行外箱贴")
            {
                st_26.Text = "客户料号";
                st_24.Text = "产品型号：";
                st_29.Text = "产品名称：";
                st_6.Text = "机型：";
                st_7.Text = "参数";
                sle_23.Enabled = true;
                sle_19.Enabled = true;
                ddlb_3.Enabled = true;
                sle_20.Enabled = true;
                sle_21.Enabled = true;
            }

            if( str_盒贴名称=="中钞科堡-外箱贴" ){

                st_24.Text = "物料号1：";
                st_7.Text = "物料号2：";
                st_26.Text = "物料号3：";
               // st_7.Text = "参数";
                st_6.Text = "机型：";
                st_29.Text = "产品名称：";
             
                sle_23.Enabled = true;
                sle_19.Enabled = true;
                ddlb_3.Enabled = true;
                sle_20.Enabled = true;
                sle_21.Enabled = true;

            }






            if (str_盒贴名称 == "通用模板")
            {
                st_26.Text = "客户料号";
                st_24.Text = "产品型号：";
                st_29.Text = "产品名称：";
                st_6.Text = "机型：";
                st_7.Text = "参数";
                sle_19.Enabled = true;
                //ddlb_1.Enabled = true;
                sle_20.Enabled = true;
                sle_21.Enabled = true;
            }
            if (str_盒贴名称 == "通用模板电流")
            {
                st_26.Text = "客户料号";
                st_24.Text = "产品型号：";
                st_29.Text = "产品名称：";
                sle_19.Enabled = true;
                //ddlb_1.Enabled = true;
                sle_20.Enabled = true;
                sle_21.Enabled = true;
            }
            if (str_盒贴名称 == "中性模板")
            {
                st_24.Text = "产品型号：";
                st_29.Text = "产品名称：";
                sle_19.Enabled = true;
               // ddlb_1.Enabled = true;
                sle_20.Enabled = true;
                sle_21.Enabled = true;
            }
            if (str_盒贴名称 == "常熟模板")
            {
                st_24.Text = "产品型号：";
                st_29.Text = "产品名称：";
                sle_19.Enabled = true;
               // ddlb_1.Enabled = true;
                sle_20.Enabled = true;
                sle_21.Enabled = true;
            }
            if (str_盒贴名称 == "正泰模板")
            {
                st_24.Text = "适配断路器：";
                st_29.Text = "附件名称：";
                sle_19.Enabled = true;
                //ddlb_1.Enabled = true;
                ddlb_3.Enabled = true;
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
                //ddlb_1.Enabled = true;
            }
            if (str_盒贴名称 == "诺雅克模板")
            {
                st_24.Text = "规格型号：";
                st_29.Text = "物料名称：";
                sle_19.Enabled = true;
                sle_21.Enabled = true;
                sle_20.Enabled = true;
                //ddlb_1.Enabled = true;
            }
            if (str_盒贴名称 == "分励英文模板")
            {
                st_24.Text = "规格型号：";
                st_29.Text = "物料名称：";
                sle_19.Enabled = true;
                sle_21.Enabled = true;
               // ddlb_1.Enabled = true;
            }
            if (str_盒贴名称 == "闭合英文模板")
            {
                st_24.Text = "规格型号：";
                st_29.Text = "物料名称：";
                sle_19.Enabled = true;
                sle_21.Enabled = true;
               // ddlb_1.Enabled = true;
            }
            if (str_盒贴名称 == "欠压英文模板")
            {
                st_24.Text = "规格型号：";
                st_29.Text = "物料名称：";
                sle_19.Enabled = true;
                sle_21.Enabled = true;
                //ddlb_1.Enabled = true;
            }
            if (str_盒贴名称 == "辅助英文模板")
            {
                st_24.Text = "规格型号：";
                st_29.Text = "物料名称：";
                sle_19.Enabled = true;
                sle_21.Enabled = true;
               // ddlb_1.Enabled = true;
            }
            if (str_盒贴名称 == "辅报英文模板")
            {
                st_24.Text = "规格型号：";
                st_29.Text = "物料名称：";
                sle_19.Enabled = true;
                sle_21.Enabled = true;
                //ddlb_1.Enabled = true;
            }
            if (str_盒贴名称 == "报警英文模板")
            {
                st_24.Text = "规格型号：";
                st_29.Text = "物料名称：";
                sle_19.Enabled = true;
                sle_21.Enabled = true;
                //ddlb_1.Enabled = true;
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
                //ddlb_1.Enabled = true;
                sle_20.Enabled = true;
                sle_21.Enabled = true;
            }

        }

        private void fun_保存()
        {
          
             DataRow r_P=    gvP.GetDataRow(gvP.FocusedRowHandle);
                if (flag) //新增
                {
                    DataRow rM = dtM.NewRow();
                    dataBindHelper1.DataToDR(rM);
                    if (ddlb_2.Text == "中钞科堡-外箱贴")
                    {
                        rM["备用字段1"] = sle_19.Text;
                        rM["备用字段2"] = sle_20.Text;
                        rM["备用字段3"] = ddlb_3.Text;

                    }
                    rM["dymb"] = rM["mbmc"].ToString().Trim();
                    rM["khbh"] = searchLookUpEdit1.EditValue == null ? "" : searchLookUpEdit1.EditValue.ToString();
                    rM["wlbh"] = r_P["物料编码"];
                
                    if (st_6.Text != "机种" && st_6.Text != "LOT/SN")
                    {
                        rM["ggxh"] = sle_23.Text.Trim();
                    }
                    else
                    {
                        rM["jz"] = sle_23.Text.Trim();
                    }
                    dtM.Rows.Add(rM);

                }
                else
                {


                    if (textBox1.Text == "")
                    {
                        textBox1.Text = "0";  //箱装数量
                    }
                    // sle_4.Text = "";
                    dataBindHelper1.DataToDR(dr);
                    if (ddlb_2.Text == "中钞科堡-外箱贴")
                    {
                        dr["备用字段1"] = sle_19.Text;
                        dr["备用字段2"] = sle_20.Text;
                        dr["备用字段3"] = ddlb_3.Text;

                    }
                    dr["khbh"] = searchLookUpEdit1.EditValue == null ? "" : searchLookUpEdit1.EditValue.ToString();
                    dr["dymb"] = dr["mbmc"].ToString().Trim();
                    if (st_6.Text != "机种" && st_6.Text != "LOT/SN")
                    {
                        dr["ggxh"] = sle_23.Text.Trim();
                    }
                    else
                    {
                        dr["jz"] = sle_23.Text.Trim();
                    }
                   


                    //if(!flag){
                    //    DataView dv = new DataView(dtM);
                    //   DataTable dt=  dv.ToTable();

                    //}


                }

                string sql = "select * from BQ_HZXX where 1<>1";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);

                da.Update(dtM);
                MessageBox.Show("保存成功");
                dtM.AcceptChanges();

                fun_刷新(r_P["物料编码"].ToString().Trim());
                      

               

                DataRow[] ds = dtP.Select(string.Format("物料编码 = '{0}'", r_P["物料编码"].ToString().Trim()));

               // DataView dv= gridControl
                if (ds[0]["是否有盒贴"].ToString() == "否")
                {
                    sql = "select 物料编码,物料名称,规格型号,是否有盒贴 from 基础数据物料信息表 where 1<>1";
                    ds[0]["是否有盒贴"] = "是";
                    da = new SqlDataAdapter(sql, strconn);
                    new SqlCommandBuilder(da);
                    da.Update(dtP);
                    dtP.AcceptChanges();
                }
            
        }
        #endregion

        #region 界面操作
        //保存
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //if (dtM != null)
            //{
            try
            {
                if (MessageBox.Show("是否要保存？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    fun_保存();
                    //MessageBox.Show("保存成功");
                    fun_flash();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
              
            //}
        }
        //关闭
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (bl == false)
            {
                CPublic.UIcontrol.ClosePage();
            }
            else
            {
                if (XTC.TabPages.Count == 1) { }
                if (XTC.SelectedTabPage.Text == "首页") { }
                DevExpress.XtraTab.XtraTabPage xtp = null;
                try
                {
                    xtp = XTC.SelectedTabPage;
                    XTC.SelectedTabPageIndex = XTC.SelectedTabPageIndex - 1;
                }
                catch { }
                try
                {
                    xtp.Controls[0].Dispose();
                    XTC.TabPages.Remove(xtp);
                    xtp.Dispose();
                }
                catch { }
            }
        }
        #endregion

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow r_focused = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            string s = string.Format(@" select BQ_HZXX.*,客户名称 from BQ_HZXX 
                                        left join 客户基础信息表 kh  on kh.客户编号=BQ_HZXX.khbh
                                                    where wlbh = '{0}' and mbmc='{1}' and khbh='{2}'", r_focused["wlbh"], r_focused["mbmc"], r_focused["客户编号"]);

            dtM = new DataTable();
            SqlDataAdapter daM = new SqlDataAdapter(s, strconn);
            daM.Fill(dtM);
            if (dtM.Rows.Count != 0) //有且仅有一条
            {
                dr = dtM.Rows[0];
                fun_盒贴信息(dtM.Rows[0]["mbmc"].ToString().Trim()); //设置各label名称 和 textbox 的 enable 属性
                dataBindHelper1.DataFormDR(dtM.Rows[0]);
                if (st_6.Text.ToString() != "机种" && st_6.Text.ToString() != "LOT/SN")
                {
                    sle_23.Text = dtM.Rows[0]["ggxh"].ToString().Trim();
                }
                else
                {
                    sle_23.Text = dtM.Rows[0]["jz"].ToString().Trim();
                }
                searchLookUpEdit1.EditValue = dr["khbh"].ToString();
            }
            else
            {
                // ddlb_1.EditValue = "";
                ddlb_2.EditValue = "";
                ddlb_3.EditValue = "";
                sle_19.Text = "";
                sle_20.Text = "";
                sle_21.Text = "";
                sle_23.Text = "";
                sle_24.Text = "";
                sle_4.Text = "";
                //默认不能用 
                sle_19.Enabled = false;
                sle_20.Enabled = false;
                sle_21.Enabled = false;
                sle_23.Enabled = false;
                sle_24.Enabled = false;
                // ddlb_1.Enabled = false;
                ddlb_3.Enabled = false;
                searchLookUpEdit1.EditValue = null;

            }
            flag = false;
        }
        //选中物料信息 客户模板信息
        private void button1_Click(object sender, EventArgs e)
        {
            flag = true;
            // ddlb_1.EditValue = "";
            ddlb_2.EditValue = "";
            ddlb_3.EditValue = "";
            sle_19.Text = "";
            sle_20.Text = "";
            sle_21.Text = "";
            sle_23.Text = "";
            sle_24.Text = "";
            sle_4.Text = "";
            //默认不能用 
            sle_19.Enabled = false;
            sle_20.Enabled = false;
            sle_21.Enabled = false;
            sle_23.Enabled = false;
            sle_24.Enabled = false;
            // ddlb_1.Enabled = false;
            ddlb_3.Enabled = false;
            searchLookUpEdit1.EditValue = null;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (dr != null)
                {
                    if (MessageBox.Show("确认删除吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        DataRow r_P = gvP.GetDataRow(gvP.FocusedRowHandle);
                        dr.Delete();
                        string sql = string.Format("select * from BQ_HZXX where 1<>1");
                        SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                        new SqlCommandBuilder(da);
                        da.Update(dt_khmb);
                        MessageBox.Show("删除成功");
                        dt_khmb.AcceptChanges();

                        if (dt_khmb.Rows.Count <= 0)
                        {
                            DataRow[] ds = dtP.Select(string.Format("物料编码 = '{0}'", r_P["物料编码"].ToString().Trim()));

                            sql = "select 物料编码,物料名称,规格型号,是否有盒贴 from 基础数据物料信息表 where 1<>1";
                            ds[0]["是否有盒贴"] = "否";
                            da = new SqlDataAdapter(sql, strconn);
                            new SqlCommandBuilder(da);
                            da.Update(dtP);
                            dtP.AcceptChanges();
                            //     string sql2 = string.Format("select 物料编码,物料名称,规格型号,是否有盒贴 from 基础数据物料信息表 where 物料编码='{0}'", r_P["物料编码"].ToString().Trim());
                            //string sql22 = string.Format("select 物料编码,物料名称,规格型号,是否有盒贴 from 基础数据物料信息表 where 1<>1");
                            //DataRow dr2 = CZMaster.MasterSQL.Get_DataRow(sql2, strconn);
                            //dr2["是否有盒贴"] = "否";
                            //da = new SqlDataAdapter(sql22, strconn);
                            //   new SqlCommandBuilder(da);
                            //     da.Update(dtP);
                            //     dtP.LoadDataRow(dr2.row);
                            //     dtP.AcceptChanges();
                            // //}   
                        }
                    }
                }
                else
                {
                    throw new Exception("未选中行不可删除");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        private void fun_刷新(string a)
        {

            string sql = string.Format(@"select *  from BQ_HZXX 
                                        left join 客户基础信息表 kh  on kh.客户编号=BQ_HZXX.khbh
                                                    where wlbh = '{0}'", a);

            dt_khmb = new DataTable();
            dt_khmb = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gridControl1.DataSource = dt_khmb;


        }
        private void fun_flash()
        {
            ddlb_2.EditValue = "";
            ddlb_3.EditValue = "";
            st_7.Text = "参数";
            sle_19.Text = "";
            sle_20.Text = "";
            sle_21.Text = "";
            sle_23.Text = "";
            sle_24.Text = "";
            sle_4.Text = "";
            //默认不能用 
            sle_19.Enabled = false;
            sle_20.Enabled = false;
            sle_21.Enabled = false;
            sle_23.Enabled = false;
            sle_24.Enabled = false;
            // ddlb_1.Enabled = false;
            ddlb_3.Enabled = false;
            searchLookUpEdit1.EditValue = null;

        }

        private void gvP_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gvP.GetFocusedRowCellValue(gvP.FocusedColumn));
                e.Handled = true;
            }
        }
    }
}
