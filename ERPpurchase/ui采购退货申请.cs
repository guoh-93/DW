using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPpurchase
{
    public partial class ui采购退货申请 : UserControl
    {
        #region  变量

        DataTable dtM;
        DataTable dt_采购单;
        DataTable dt_供应商;
        string strcon = CPublic.Var.strConn;
        DataTable dt_申请主;
        DataRow drM;
        bool str_新增 = true;


        #endregion



        public ui采购退货申请()
        {
            InitializeComponent();
        }

        public ui采购退货申请(DataRow dr)
        {
            InitializeComponent();
            drM = dr;
            str_新增 = false;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            txt_退货申请单号.Text = "";
            txt_备注.Text = "";
            cb_采购明细.Text = "";
            textBox2.Text = "";
            searchLookUpEdit1.EditValue = null;
            textBox2.Text = "";
            ui采购退货申请_Load(null, null);
        }

        private void ui采购退货申请_Load(object sender, EventArgs e)
        {
            dt_供应商 = ERPorg.Corg.fun_供应商("供应商状态='在用'");
            searchLookUpEdit1.Properties.DataSource = dt_供应商;
            searchLookUpEdit1.Properties.DisplayMember = "供应商ID";
            searchLookUpEdit1.Properties.ValueMember = "供应商ID";
            if (str_新增)
            {
                string sql = "select * from 采购退货申请子表 where 1<>1 ";
                dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                dtM.Columns.Add("供应商编号");
                gc.DataSource = dtM;
                //string sql_主 = "select * from 采购退货申请主表 where 1<>1 ";
                //dt_申请主 = CZMaster.MasterSQL.Get_DataTable(sql_主, strcon);
            }
            else
            {
                string sql = string.Format("select * from 采购退货申请子表 where 退货申请单号 = '{0}' ",drM["退货申请单号"]);
                dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                dtM.Columns.Add("供应商编号");
                
                
                //string sql_主 = string.Format("select * from 采购退货申请主表 where 退货申请单号 = '{0}'",drM["退货申请单号"]) ;
                //dt_申请主 = CZMaster.MasterSQL.Get_DataTable(sql_主, strcon);
                fun_load();
                foreach(DataRow dr in dtM.Rows)
                {
                    dr["供应商编号"] = searchLookUpEdit1.EditValue;
                }
                gc.DataSource = dtM;
            }
            fun_采购明细();
        }

        private void fun_load()
        {
            txt_退货申请单号.Text = drM["退货申请单号"].ToString();
            txt_备注.Text = drM["备注"].ToString();
            searchLookUpEdit1.EditValue = drM["供应商编号"].ToString();
            textBox1.Text = drM["供应商名称"].ToString();
            textBox2.Text = dtM.Rows[0]["税率"].ToString();
            //cb_采购明细.EditValue = dtM.Rows[0]["采购明细"].ToString();
        }

        private void fun_采购明细()
        {
            string str_条件 = "";

            if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
            {
                str_条件 = string.Format(" and b.供应商ID='{0}'", searchLookUpEdit1.EditValue.ToString());

            }
            DateTime t = CPublic.Var.getDatetime().AddYears(-1);
            t = new DateTime(t.Year, 1, 1);

            //            string sql = string.Format(@"select 采购明细号,完成数量,a.物料编码,b.物料名称,供应商ID,供应商,a.原ERP物料编号,a.图纸编号,a.n原ERP规格型号  
            //               ,b.单价,b.未税单价,b.税率 from  采购记录采购单明细表 b  left join 基础数据物料信息表 a  on   a.物料编码=b.物料编码
            //               where   明细完成日期 is not null and 生效日期>'{0}' {1}", t, str_条件);

            string sql = string.Format(@"select 采购明细号,送检单明细号,入库明细号,入库单号,完成数量,a.物料编码,b.物料名称,b.供应商ID,b.供应商,a.图纸编号,a.规格型号,b.仓库号,b.仓库名称
               ,b.单价,b.未税单价,b.税率 from  采购记录采购单明细表 b  left join 基础数据物料信息表 a  on   a.物料编码=b.物料编码
               left  join 采购记录采购单入库明细 c on c.采购单明细号=b.采购明细号
               where    b.生效日期>'{0}'  and c.生效日期 is not null {1} ", t, str_条件);
            dt_采购单 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            cb_采购明细.Properties.DataSource = dt_采购单;
            cb_采购明细.Properties.DisplayMember = "入库单号";
            cb_采购明细.Properties.ValueMember = "入库明细号";

        }



        private void cb_采购明细_EditValueChanged(object sender, EventArgs e)
        {
            if (cb_采购明细.EditValue != null && cb_采购明细.EditValue.ToString() != "")
            {
                DataRow[] dr = dt_采购单.Select(string.Format("入库明细号='{0}' ", cb_采购明细.EditValue));
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    searchLookUpEdit1.EditValue = dr[0]["供应商ID"].ToString();
                }
                DataRow r = dtM.NewRow();
                r["物料编码"] = dr[0]["物料编码"];
                r["物料名称"] = dr[0]["物料名称"];
                r["规格型号"] = dr[0]["规格型号"];
                //20-3-23 默认不合格品1
                r["仓库号"] = "08";
                r["仓库名称"] = "不合格品1";
                r["税率"] = dr[0]["税率"];
                r["供应商编号"] = dr[0]["供应商ID"];
                r["图纸编号"] = dr[0]["图纸编号"];
                r["采购明细"] =dr[0]["采购明细号"];
                r["送检单明细号"] = dr[0]["送检单明细号"];

                r["入库单号"] = cb_采购明细.Text;
                r["采购入库明细号"] = cb_采购明细.EditValue;


                r["含税单价"] = dr[0]["单价"];
                r["不含税单价"] = dr[0]["未税单价"];
                textBox2.Text = dr[0]["税率"].ToString();
                dtM.Rows.Add(r);
            }
        }
        private void fun_save()
        {
            DateTime t = CPublic.Var.getDatetime();
            string str_prsq = "";

            string sql_主 = string.Format("select * from 采购退货申请主表 where 退货申请单号 = '{0}'", txt_退货申请单号.Text);
            dt_申请主 = CZMaster.MasterSQL.Get_DataTable(sql_主, strcon);      
            if(dt_申请主.Rows.Count == 0)
            {
                str_prsq = string.Format("PRSQ{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("PRSQ", t.Year, t.Month));
                txt_退货申请单号.Text = str_prsq;
                DataRow r_z = dt_申请主.NewRow();
                r_z["退货申请单号"] = str_prsq = txt_退货申请单号.Text;
                r_z["申请日期"] = t;
                r_z["供应商编号"] = searchLookUpEdit1.EditValue.ToString();
                r_z["供应商名称"] = textBox1.Text;
                r_z["申请人ID"] = CPublic.Var.LocalUserID;
                r_z["申请人"] = CPublic.Var.localUserName;
                r_z["备注"] = txt_备注.Text;
                r_z["生效"] = 1;
                r_z["生效日期"] = t;
                dt_申请主.Rows.Add(r_z);
            }
            else
            {
                dt_申请主.Rows[0]["退货申请单号"] = str_prsq = txt_退货申请单号.Text;
                dt_申请主.Rows[0]["申请日期"] = t;
                dt_申请主.Rows[0]["供应商编号"] = searchLookUpEdit1.EditValue.ToString();
                dt_申请主.Rows[0]["供应商名称"] = textBox1.Text;
                dt_申请主.Rows[0]["申请人ID"] = CPublic.Var.LocalUserID;
                dt_申请主.Rows[0]["申请人"] = CPublic.Var.localUserName;
                dt_申请主.Rows[0]["备注"] = txt_备注.Text;
                dt_申请主.Rows[0]["生效"] = 1;
                dt_申请主.Rows[0]["生效日期"] = t;
                
            }
            //if (str_新增)
            //{
            //    str_prsq = string.Format("PRSQ{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("PRSQ", t.Year, t.Month));
            //    txt_退货申请单号.Text = str_prsq;
            //}
            ////string str_prsq = string.Format("PRSQ{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("PRSQ", t.Year, t.Month));
            
            //// 申请主表记录
            //DataRow r_z = dt_申请主.NewRow();
            //r_z["退货申请单号"] = str_prsq = txt_退货申请单号.Text;
            //r_z["申请日期"] = t;
            //r_z["供应商编号"] = searchLookUpEdit1.EditValue.ToString();
            //r_z["供应商名称"] = textBox1.Text;
            //r_z["申请人ID"] = CPublic.Var.LocalUserID;
            //r_z["申请人"] = CPublic.Var.localUserName;
            //r_z["备注"] = txt_备注.Text;
            //r_z["生效"] = 1;
            //r_z["生效日期"] = t;

            //dt_申请主.Rows.Add(r_z);
            decimal dec_含税 = 0;
            decimal dec_不含税 = 0;
            int i = 1;
            foreach (DataRow r in dtM.Rows)
            {

                if (r.RowState == DataRowState.Deleted)
                {
                    continue;
                }
                r["退货申请单号"] = str_prsq;
                r["退货申请明细号"] = str_prsq + "-" + i++.ToString("00");
                r["生效"] = 1;
                r["生效日期"] = t;
                dec_含税 = dec_含税 + Convert.ToDecimal(r["含税金额"]);

                dec_不含税 = dec_不含税 + Convert.ToDecimal(r["不含税金额"]);
            }
            dt_申请主.Rows[0]["含税总金额"] = dec_含税;
            dt_申请主.Rows[0]["不含税总金额"] = dec_不含税;



            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction cgthsq = conn.BeginTransaction("采购退货申请");
            try
            {
                {
                    string sql = "select * from 采购退货申请主表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, cgthsq);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dt_申请主);
                    }
                }
                {
                    string sql = "select * from 采购退货申请子表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, cgthsq);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dtM);
                    }
                }
                cgthsq.Commit();
            }
            catch (Exception ex)
            {
                cgthsq.Rollback();
                throw ex;
            }
        }
        private void fun_check()
        {
            foreach (DataRow dr in dtM.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                if (Convert.ToDecimal(dr["数量"]) < 0)
                {
                    throw new Exception("退货数量不能小于0");

                }
                if (dr["供应商编号"].ToString().Trim() != searchLookUpEdit1.EditValue.ToString().Trim())
                {
                    throw new Exception("有条目的供应商不一致请检查");

                }
            }
        }
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                gv.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                fun_check();
                fun_save();
                MessageBox.Show("申请成功");
                str_新增 = true;
                barLargeButtonItem1_ItemClick(null, null);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            if (dr != null)
            {
                dr.Delete();
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
            {
                DataRow[] r = dt_供应商.Select(string.Format("供应商ID='{0}'", searchLookUpEdit1.EditValue));
                textBox1.Text = r[0]["供应商名称"].ToString();

                textBox2.Text = r[0]["税率"].ToString();
                fun_采购明细();
            }
            else
            {
                textBox1.Text = "";
                textBox2.Text = "";
                fun_采购明细();
            }

        }

        private void gv_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (e.Column.FieldName == "含税单价")
                {

                    try
                    {
                        decimal dec = Convert.ToDecimal(e.Value);
                        decimal dec_q = dec / (1 + Convert.ToDecimal(textBox2.Text) / 100);
                        dr["不含税单价"] = dec_q;
                        if (dr["数量"] != null && dr["数量"].ToString() != "")
                        {
                            dr["含税金额"] = dec * Convert.ToDecimal(dr["数量"]);

                            dr["不含税金额"] = dec_q * Convert.ToDecimal(dr["数量"]);
                        }
                    }
                    catch (Exception)
                    {

                        throw new Exception("不含税单价格式不正确,请检查");
                    }


                }

                if (e.Column.FieldName == "不含税单价")
                {

                    try
                    {
                        decimal dec = Convert.ToDecimal(e.Value);
                        decimal dec_q = dec * (1 + Convert.ToDecimal(textBox2.Text) / 100);
                        dr["含税单价"] = dec_q;
                        if (dr["数量"] != null && dr["数量"].ToString() != "")
                        {
                            dr["不含税金额"] = dec * Convert.ToDecimal(dr["数量"]);

                            dr["含税金额"] = dec_q * Convert.ToDecimal(dr["数量"]);
                        }
                    }
                    catch (Exception)
                    {

                        throw new Exception("含税单价格式不正确,请检查");
                    }
                }
                if (e.Column.Caption == "数量")
                {
                    //DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                    try
                    {

                        if (dr["含税单价"] != null && dr["含税单价"].ToString() != "")
                        {
                            decimal dec = Convert.ToDecimal(dr["含税单价"]);
                            dr["含税金额"] = dec * Convert.ToDecimal(e.Value);
                            dr["不含税金额"] = dec / (1 + Convert.ToDecimal(textBox2.Text) / 100) * Convert.ToDecimal(e.Value);
                        }
                    }
                    catch (Exception)
                    {

                        throw new Exception("数量格式不正确,请检查");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }







    }
}
