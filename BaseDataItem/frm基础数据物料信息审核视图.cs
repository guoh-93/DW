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
    public partial class frm基础数据物料信息审核视图 : UserControl
    {
        DataTable dtM = null;
        DataTable dt_BOM = null;
        DataTable dt_包装 = null;
        DataTable dt_合贴 = null;
        DataTable dt_记录 = null;
        string strconn = CPublic.Var.strConn;

        public frm基础数据物料信息审核视图()
        {
            InitializeComponent();
        }

        private void frm基础数据物料信息审核视图_Load(object sender, EventArgs e)
        {
            fun_载入物料();
            label1.Text = "";
        }

        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                //载入BOM
                fun_载入BOM(dr["物料编码"].ToString().Trim());
                //载入包装
                fun_载入包装(dr["物料编码"].ToString().Trim());
                //载入合贴
                fun_载入合贴(dr["物料编码"].ToString().Trim());
                //载入修改记录
                fun_载入修改记录(dr["物料编码"].ToString().Trim());
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
            }
        }

        //没用了
        private void gv_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (gv.GetRowCellValue(e.RowHandle, "审核").ToString() == "已审核")
            {
                e.Appearance.BackColor = Color.LightBlue;
                e.Appearance.BackColor2 = Color.LightBlue;
            }
        }

        private void fun_载入物料()
        {
            string sql = "select * from 基础数据物料信息表 where 停用 = 0 and 物料类型 = '成品' and 审核 = '待审核'";
            //string sql = "select * from 基础数据物料信息表 where 停用 = 0 and 物料类型 = '成品'";
            dtM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            DataView dv = new DataView(dtM);
            dv.RowFilter = "审核 = '待审核'";
            gc.DataSource = dtM;
        }

        private void fun_载入BOM(string str)
        {
            string sql = string.Format(@"select 基础数据物料BOM表.*,基础数据物料信息表.图纸编号 from 基础数据物料BOM表 
            left join 基础数据物料信息表 on 基础数据物料BOM表.子项编码 = 基础数据物料信息表.物料编码 where 基础数据物料BOM表.产品编码 = '{0}'", str);
            dt_BOM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_BOM);
            gc_BOM.DataSource = dt_BOM;
        }

        private void fun_载入包装(string str)
        {
            string sql = string.Format("select * from 基础数据包装清单表 where 成品编码 = '{0}'", str);
            dt_包装 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_包装);
            gc_包装.DataSource = dt_包装;
        }

        private void fun_载入合贴(string str)
        {
            string sql = string.Format("select * from BQ_HZXX where wlbh = '{0}'", str);
            dt_合贴 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_合贴);
            if (dt_合贴.Rows.Count != 0)
            {
                fun_盒贴信息(dt_合贴.Rows[0]["mbmc"].ToString().Trim());
                dataBindHelper1.DataFormDR(dt_合贴.Rows[0]);
                if (st_6.Text != "机种" && st_6.Text != "LOT/SN")
                {
                    sle_23.Text = dt_合贴.Rows[0]["ggxh"].ToString();
                }
                else
                {
                    sle_23.Text = dt_合贴.Rows[0]["jz"].ToString();
                }
            }
            else
            {
                ddlb_1.EditValue = "";
                ddlb_2.EditValue = "";
                ddlb_3.EditValue = "";
                sle_19.Text = "";
                sle_20.Text = "";
                sle_21.Text = "";
                sle_23.Text = "";
                sle_24.Text = "";
                sle_4.Text = "";
                label1.Text = "不存在该产品的盒贴模板";
            }
        }

        private void fun_盒贴信息(string str_盒贴名称)
        {
            st_6.Text = "机种";
            st_24.Text = "产品型号";
            st_29.Text = "产品名称";

            if (str_盒贴名称 == "通用模板")
            {
                st_24.Text = "产品型号";
                st_29.Text = "产品名称";
            }
            if (str_盒贴名称 == "通用模板电流")
            {
                st_24.Text = "产品型号";
                st_29.Text = "产品名称";
            }
            if (str_盒贴名称 == "中性模板")
            {
                st_24.Text = "产品型号";
                st_29.Text = "产品名称";
            }
            if (str_盒贴名称 == "常熟模板")
            {
                st_24.Text = "产品型号";
                st_29.Text = "产品名称";
            }
            if (str_盒贴名称 == "正泰模板")
            {
                st_24.Text = "适配断路器";
                st_29.Text = "附件名称";
            }
            if (str_盒贴名称 == "宁波施耐德")
            {
                st_24.Text = "型号规格";
                st_29.Text = "产品名称";
            }
            if (str_盒贴名称 == "温州德力西")
            {
                st_24.Text = "零部件名称";
                st_29.Text = "零部件编码";
            }
            if (str_盒贴名称 == "台安模板")
            {
                st_24.Text = "型号";
                st_29.Text = "品名";
            }
            if (str_盒贴名称 == "诺雅克模板")
            {
                st_24.Text = "规格型号";
                st_29.Text = "物料名称";
            }
            if (str_盒贴名称 == "分励英文模板")
            {
                st_24.Text = "规格型号";
                st_29.Text = "物料名称";
            }
            if (str_盒贴名称 == "闭合英文模板")
            {
                st_24.Text = "规格型号";
                st_29.Text = "物料名称";
            }
            if (str_盒贴名称 == "欠压英文模板")
            {
                st_24.Text = "规格型号";
                st_29.Text = "物料名称";
            }
            if (str_盒贴名称 == "辅助英文模板")
            {
                st_24.Text = "规格型号";
                st_29.Text = "物料名称";
            }
            if (str_盒贴名称 == "辅报英文模板")
            {
                st_24.Text = "规格型号";
                st_29.Text = "物料名称";
            }
            if (str_盒贴名称 == "报警英文模板")
            {
                st_24.Text = "规格型号";
                st_29.Text = "物料名称";
            }
            if (str_盒贴名称 == "芜湖德力西")
            {
                st_24.Text = "规格型号";
                st_29.Text = "物料名称";
                st_6.Text = "对方型号";
            }
            if (str_盒贴名称 == "芜湖德力西英文")
            {
                st_24.Text = "规格型号";
                st_29.Text = "物料名称";
                st_6.Text = "对方型号";
            }
            if (str_盒贴名称 == "宏美模板")
            {
                st_24.Text = "规格型号";
                st_29.Text = "物料名称";
                st_6.Text = "LOT/SN";
            }
            if (str_盒贴名称 == "正泰英文版")
            {
                st_24.Text = "型号规格";
                st_29.Text = "产品名称";
            }
            if (str_盒贴名称 == "常熟外发模板")
            {
                st_24.Text = "型号规格";
                st_29.Text = "产品名称";
            }
        }

        private void fun_载入修改记录(string str)
        {
            string sql = string.Format("select * from 基础数据BOM信息修改记录表 where 成品编码 = '{0}'", str);
            dt_记录 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_记录);
            gc_记录.DataSource = dt_记录;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //审核
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (dr["审核"].ToString() == "待审核")
                {
                    if (MessageBox.Show("是否确认？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        dr["审核"] = "已审核";
                        dr["审核人ID"] = CPublic.Var.LocalUserID;
                        dr["审核人"] = CPublic.Var.localUserName;
                        dr["审核日期"] = System.DateTime.Now;
                        //保存
                        string sql = "select * from 基础数据物料信息表 where 1<>1";
                        SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                        new SqlCommandBuilder(da);
                        da.Update(dtM);
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string sql = "select * from 基础数据物料信息表 where 停用 = 0 and 物料类型 = '成品' and 审核 = '待审核'";
            //string sql = "select * from 基础数据物料信息表 where 停用 = 0 and 物料类型 = '成品'";
            dtM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            DataView dv = new DataView(dtM);
            dv.RowFilter = "审核 = '待审核'";
            gc.DataSource = dtM;
        }
    }
}
