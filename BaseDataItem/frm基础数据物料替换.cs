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
    public partial class frm基础数据物料替换 : UserControl
    {
        #region
        string strconn = CPublic.Var.strConn;
        string str_物料编码;
        string str_物料名称;
        string str_规格;
        DataTable dtM = null;
        DataTable dtP = null;
        DataTable dt_原材料 = null;
        public static DevExpress.XtraTab.XtraTabControl XTC;
        #endregion

        #region
        public frm基础数据物料替换()
        {
            InitializeComponent();
        }

        public frm基础数据物料替换(string str,string strr,string srt)
        {
            InitializeComponent();
            str_物料编码 = str;
            str_物料名称 = strr;
            str_规格 = srt;
        }

        private void frm基础数据物料替换_Load(object sender, EventArgs e)
        {
            try
            {
                label1.Text = "";
                fun_载入源成品(str_物料编码);
                fun_载入原材料();
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "物料替换1");
            }
        }

        private void gv_源_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gv_源.GetDataRow(gv_源.FocusedRowHandle);
                string sql = string.Format(@"select bom.产品编码,子项编码,fx.物料名称 as 产品名称,zx.物料名称 子项名称,bom.数量,bom.仓库号,bom.仓库名称 from 基础数据物料BOM表 bom
 left join 基础数据物料信息表 as  fx on  bom.产品编码=fx.物料编码
 left join 基础数据物料信息表 as  zx on  bom.子项编码=zx.物料编码
where 产品编码 = '{0}'", dr["产品编码"].ToString());
                dtP = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtP);
                gc_BOM.DataSource = dtP;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "物料替换2");
            }
        }

        private void txt_物料编码_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                DataRow[] ds = dt_原材料.Select(string.Format("物料编码 = '{0}'", txt_物料编码.EditValue.ToString()));
                txt_物料名称.Text = ds[0]["物料名称"].ToString();
                txt_规格型号.Text = ds[0]["规格型号"].ToString();
                txt_大类.Text = ds[0]["大类"].ToString();
                txt_小类.Text = ds[0]["小类"].ToString();
                txt_图纸编号.Text = ds[0]["图纸编号"].ToString();
                textBox1.Text = ds[0]["仓库号"].ToString();
                textBox2.Text = ds[0]["仓库名称"].ToString();

                // txt_原物料编码.Text = ds[0]["物料编码"].ToString(); 
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "物料替换3");
            }
        }
        #endregion

        #region
        /// <summary>
        /// 哪些成品BOM中使用 该原料
        /// </summary>
        private void fun_载入源成品(string str_原材料)
        {
            label1.Text = string.Format("原材料：{0}-{1}-{2}", str_物料编码, str_物料名称, str_规格);
            //string sql = string.Format(@"select 基础数据物料BOM表.产品编码,基础数据物料BOM表.产品名称,基础数据物料信息表.规格型号,基础数据物料BOM表.子项编码,基础数据物料BOM表.子项名称
            string sql = string.Format(@"select bom.*,base.规格型号,base.物料名称
                    from 基础数据物料BOM表 bom left join 基础数据物料信息表 base
                    on bom.产品编码 = base.物料编码 
                    where bom.子项编码 = '{0}'", str_原材料);
            dtM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            dtM.Columns.Add("选择", typeof(Boolean));
            gc_源.DataSource = dtM;

        }

        private void fun_载入原材料()
        {
           // string sql = "select 物料编码,物料名称,物料类型,规格型号,大类,小类,图纸编号 from 基础数据物料信息表 where (物料类型 = '原材料' or 物料类型 = '半成品') and 停用 = 0";
            string sql = string.Format(@"select 物料编码,物料名称,物料类型,规格型号,大类,小类,图纸编号,存货分类编码,存货分类,仓库号,仓库名称 from 基础数据物料信息表
                        where left(存货分类编码,2) in ('01','05') and 停用 = 0", str_物料编码);
            dt_原材料 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_原材料);
            txt_物料编码.Properties.DataSource = dt_原材料;
            txt_物料编码.Properties.ValueMember = "物料编码";
            txt_物料编码.Properties.DisplayMember = "物料编码";
        }
        #endregion

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                if (MessageBox.Show("是否确认替换该物料？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DataTable dt_BOM修改;
                    string sql_1 = string.Format("select * from 基础数据BOM信息修改记录表 where 1<>1");
                    dt_BOM修改 = new DataTable();
                    SqlDataAdapter da_1 = new SqlDataAdapter(sql_1, strconn);
                    da_1.Fill(dt_BOM修改);
                    //确认替换物料
                    DateTime t = CPublic.Var.getDatetime();
                    foreach (DataRow dr in dtM.Rows)
                    {

                       
                        if (dr["选择"].ToString().ToLower() == "true")
                        {
                                DataRow rr = dt_BOM修改.NewRow();
                                rr["修改人"] = CPublic.Var.localUserName;
                                rr["修改人ID"] = CPublic.Var.LocalUserID;
                                rr["修改日期"] = t;
                                rr["修改原因"] = "替换";
                                rr["成品编码"] = dr["产品编码"];
                                rr["成品名称"] = dr["物料名称"];
                                rr["修改属性"] = "替换";
                                rr["更改前物料"] = str_物料编码;
                                rr["更改后物料"] = txt_物料编码.EditValue.ToString();

                                dt_BOM修改.Rows.Add(rr);
                            dr["子项编码"] = txt_物料编码.EditValue.ToString();
                            dr["子项名称"] = txt_物料名称.Text;
                            dr["仓库号"] = textBox1.Text;
                            dr["仓库名称"] = textBox2.Text;
                            dr["物料替换"] = "已替换";
                            dr["替换人"] = CPublic.Var.localUserName;
                            dr["替换人ID"] = CPublic.Var.LocalUserID;
                            dr["替换日期"] = t;
                        }
                    }
                    string sql = "select * from 基础数据物料BOM表 where 1<>1";
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    new SqlCommandBuilder(da);
                    da.Update(dtM);

                    CZMaster.MasterSQL.Save_DataTable(dt_BOM修改, "基础数据BOM信息修改记录表", strconn);
                    MessageBox.Show("保存成功");
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "物料替换4");
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //foreach (DataRow dr in dtM.Rows)
            //{
            //    dr["选择"] = true;
            //}

            for (int i = 0; i < gv_源.DataRowCount; i++)
            {
                gv_源.GetDataRow(i)["选择"] = true;

            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DataRow dr in dtM.Rows)
            {
                dr["选择"] = false;
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show("是否批量删除该物料？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DataTable dt_BOM修改;
                    string sql_1 = string.Format("select * from 基础数据BOM信息修改记录表 where 1<>1");
                    dt_BOM修改 = new DataTable();
                    SqlDataAdapter da_1 = new SqlDataAdapter(sql_1, strconn);
                    da_1.Fill(dt_BOM修改);
                    //确认替换物料
                    foreach (DataRow dr in dtM.Rows)
                    {
                        if (dr.RowState == DataRowState.Deleted) continue;
                        if (dr["选择"].ToString().ToLower() == "true")
                        {
                            DataRow rr = dt_BOM修改.NewRow();
                            rr["修改人"] = CPublic.Var.localUserName;
                            rr["修改人ID"] = CPublic.Var.LocalUserID;
                            rr["修改日期"] = CPublic.Var.getDatetime();
                            rr["修改原因"] = "批量删除";
                            rr["成品编码"] = dr["产品编码"];
                            rr["成品名称"] = dr["产品名称"];
                            rr["修改属性"] = "删除";
                            rr["更改前物料"] = str_物料编码;
                            rr["更改后物料"] = "删除";

                            dt_BOM修改.Rows.Add(rr);
                            dr.Delete();
                        }
                    }
                    string sql = "select * from 基础数据物料BOM表 where 1<>1";
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    new SqlCommandBuilder(da);
                    da.Update(dtM);

                    CZMaster.MasterSQL.Save_DataTable(dt_BOM修改, "基础数据BOM信息修改记录表", strconn);
                    MessageBox.Show("保存成功");
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "物料替换4");
            }
        }

        private void gv_源_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gv_BOM_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }
    }
}
