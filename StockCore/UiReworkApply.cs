using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
 

namespace StockCore
{
    public partial class UiReworkApply : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dtM;
        DataTable dtP;
        DataRow drM = null;
        DataTable dt_物料;
        DataTable dt_bom;

        #endregion
        public UiReworkApply()
        {
            InitializeComponent();
        }
        public UiReworkApply(DataRow dr_c)
        {
            InitializeComponent();
            drM = dr_c;
        }
#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = dtP.NewRow();
            dtP.Rows.Add(dr);
        }


        #region 方法
#pragma warning disable IDE1006 // 命名样式
        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {

            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                DataRow[] r = dtP.Select(string.Format("物料编码='{0}'", dr["物料编码"].ToString()));
                if (r.Length > 1)
                {
                    throw new Exception(string.Format("选择了重复物料{0},请确认", dr["原ERP物料编号"]));
                }

            }

            if (txt_申请类型.EditValue == null || txt_申请类型.Text == "")
            {
                throw new Exception("请选择申请类型");

            }
            if (txt_备注.Text.ToString().Trim() == "")
            {
                throw new Exception("备注已改为必填项,请填写");

            }

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
                if (Convert.ToDecimal(dr["数量"]) <= 0)
                {
                    throw new Exception("数量不能小于0");
                }

            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_保存主表明细(Boolean bl)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (txt_出入库申请单号.Text == "")
                {
                    if (drM["GUID"].ToString() == "")
                    {
                        drM["GUID"] = System.Guid.NewGuid();
                        txt_出入库申请单号.Text = string.Format("RMA{0}{1}{2}{3}", CPublic.Var.getDatetime().Year.ToString(), CPublic.Var.getDatetime().Month.ToString("00"),
                             CPublic.Var.getDatetime().Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("RMA", CPublic.Var.getDatetime().Year, CPublic.Var.getDatetime().Month).ToString("0000"));
                        drM["出入库申请单号"] = txt_出入库申请单号.Text;
                        drM["申请日期"] = CPublic.Var.getDatetime();
                    }
                }
                drM["操作人员编号"] = CPublic.Var.LocalUserID;
                drM["操作人员"] = CPublic.Var.localUserName;
                if (bl == true)
                {
                    drM["生效"] = true;
                    drM["生效人员编号"] = CPublic.Var.LocalUserID;
                    drM["生效日期"] = CPublic.Var.getDatetime();
                }
                //dataBindHelper1.DataToDR(drM);
                drM["备注"] = txt_备注.Text;
                drM["申请类型"] = txt_申请类型.EditValue;

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

                        r["出入库申请明细号"] = drM["出入库申请单号"].ToString() + i.ToString("00");
                        r["POS"] = i++;
                    }
                    if (bl == true)
                    {
                        r["生效"] = true;
                        r["生效人员编号"] = CPublic.Var.LocalUserID;
                        r["生效日期"] = CPublic.Var.getDatetime();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("明细保存出错" + ex.Message);
            }

            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("生效");
            try
            {
                {
                    string sql = "select * from 返修出入库申请主表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dtM);
                    }
                }
                {
                    string sql = "select * from 返修出入库申请子表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dtP);
                    }
                }
                ts.Commit();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw ex;
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_载入主表明细()
#pragma warning restore IDE1006 // 命名样式
        {
            if (drM == null)
            {
                string sql = "select * from 返修出入库申请主表 where 1<>1";
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);

                drM = dtM.NewRow();
                dtM.Rows.Add(drM);

                sql = @"select 返修出入库申请子表.*,库存总数,仓库名称,货架描述 from 返修出入库申请子表,返修仓库物料数量表,基础数据物料信息表
                        where   返修出入库申请子表.物料编码=返修仓库物料数量表.物料编码 and 
                            返修出入库申请子表.物料编码=基础数据物料信息表.物料编码 and 1<>1";
                dtP = new DataTable();
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtP);
            }
            else
            {
                string sql = string.Format("select * from 返修出入库申请主表 where 出入库申请单号 = '{0}'", drM["出入库申请单号"].ToString());
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);

                drM = dtM.Rows[0];
                dataBindHelper1.DataFormDR(drM);

                string sql2 = string.Format(@"select 返修出入库申请子表.*,返修仓库物料数量表.库存总数,仓库名称,货架描述 from 返修出入库申请子表 
                  left join 返修仓库物料数量表 on 返修出入库申请子表.物料编码 = 返修仓库物料数量表.物料编码
                    left join   基础数据物料信息表 on 基础数据物料信息表.物料编码=返修出入库申请子表.物料编码
                where 出入库申请单号 = '{0}'", drM["出入库申请单号"].ToString());
                dtP = new DataTable();
                SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
                da2.Fill(dtP);
            }

            //dtP.ColumnChanged += dtP_ColumnChanged;
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_物料下拉框()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = @"select 基础数据物料信息表.物料编码,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.物料名称,基础数据物料信息表.n原ERP规格型号,
            基础数据物料信息表.规格,基础数据物料信息表.图纸编号,返修仓库物料数量表.库存总数,货架描述,仓库名称 from 基础数据物料信息表 
            left join 返修仓库物料数量表 on 基础数据物料信息表.物料编码 = 返修仓库物料数量表.物料编码  where  停用=0";
            dt_物料 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_物料);

            repositoryItemSearchLookUpEdit1.DataSource = dt_物料;
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
//            string sql_bom = @"select 基础数据物料信息表.物料编码,原ERP物料编号,基础数据物料信息表.物料名称,基础数据物料信息表.规格型号,
//            基础数据物料信息表.规格,大类,n原ERP规格型号,库存总数,货架描述,仓库名称 from 基础数据物料信息表,仓库物料数量表 where  基础数据物料信息表.物料编码=仓库物料数量表.物料编码 and
//            基础数据物料信息表.物料编码 in  (select  产品编码  from 基础数据物料BOM表 group by 产品编码 ) and  停用=0";
//            dt_bom = CZMaster.MasterSQL.Get_DataTable(sql_bom, strconn);
//            searchLookUpEdit2.Properties.DataSource = dt_bom;
//            searchLookUpEdit2.Properties.ValueMember = "物料编码";
//            searchLookUpEdit2.Properties.DisplayMember = "原ERP物料编号";

        }
        private void UiReworkApply_Load(object sender, EventArgs e)
        {
            try
            {
                time_申请日期.EditValue = CPublic.Var.getDatetime();
                fun_载入主表明细();
                fun_物料下拉框();
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
            //新增
            try
            {
                time_申请日期.EditValue = CPublic.Var.getDatetime();
                drM = null;
                txt_出入库申请单号.Text = "";
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
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            txt_出入库申请单号.Text = "";
            txt_申请类型.Text = "";
            txt_备注.Text = "";
            fun_物料下拉框();
            string sql = "select * from 返修出入库申请主表 where 1<>1";
            dtM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            gc.DataSource = dtM;

         
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //生效
            try
            {

                gv.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                fun_check();

                fun_保存主表明细(true);
                MessageBox.Show("生效成功");
                barLargeButtonItem1_ItemClick(null, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            dr.Delete();
        }

#pragma warning disable IDE1006 // 命名样式
        private void repositoryItemSearchLookUpEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);

                if (e.NewValue != null && e.NewValue.ToString() != "")
                {
                    DataRow[] ds = dt_物料.Select(string.Format("物料编码 = '{0}'", e.NewValue));
                    dr["原ERP物料编号"] = ds[0]["原ERP物料编号"];
                    dr["物料名称"] = ds[0]["物料名称"];
                    dr["n原ERP规格型号"] = ds[0]["n原ERP规格型号"];
                    dr["库存总数"] = ds[0]["库存总数"];
                    dr["货架描述"] = ds[0]["货架描述"];
                    dr["仓库名称"] = ds[0]["仓库名称"];
                }
                else
                {
                    dr["原ERP物料编号"] = "";
                    dr["物料名称"] = "";
                    dr["n原ERP规格型号"] = "";
                    dr["库存总数"] = "";
                    dr["货架描述"] = "";
                    dr["仓库名称"] = "";
                }
                //e.Row["图纸编号"] = ds[0]["图纸编号"];

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }
    }
}
        #endregion