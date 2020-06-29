using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPpurchase
{
    public partial class ui关联供应商退货 : UserControl
    {
        #region  变量
        DataTable dtM;
        DataTable dt_物料编码;
        DataTable dt_采购单;
        DataTable dt_供应商;
        string strcon = CPublic.Var.strConn;
        DataTable dt_申请主;
        DataRow drM;
        bool str_新增 = true;
        #endregion



        public ui关联供应商退货()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DataRow r = dtM.NewRow();
            dtM.Rows.Add(r);
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            txt_退货申请单号.Text = "";
            txt_备注.Text = "";
            textBox2.Text = "";
            searchLookUpEdit1.EditValue = null;
            textBox2.Text = "";
            ui关联供应商退货_Load(null, null);
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
            {
                DataRow[] r = dt_供应商.Select(string.Format("供应商ID='{0}'", searchLookUpEdit1.EditValue));
                textBox1.Text = r[0]["供应商名称"].ToString();
                textBox2.Text = r[0]["税率"].ToString();
            }
            else
            {
                textBox1.Text = "";
                textBox2.Text = "";
            }
        }

        private void ui关联供应商退货_Load(object sender, EventArgs e)
        {
            string s = @"select base.物料编码,base.物料名称 ,base.规格型号,
                        base.仓库号  ,base.仓库名称   from 基础数据物料信息表 base 
                        where (base.可购=1 or 委外=1)  and base.停用= 0 and base.在研 = 0"; //布尔字段1 位是否 纳入可用量
            dt_物料编码 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            repositoryItemSearchLookUpEdit1.DataSource = dt_物料编码;
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";

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
 
            }
            else
            {
                string sql = string.Format("select * from 采购退货申请子表 where 退货申请单号 = '{0}' ", drM["退货申请单号"]);
                dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                dtM.Columns.Add("供应商编号");
 
                fun_load();
                foreach (DataRow dr in dtM.Rows)
                {
                    dr["供应商编号"] = searchLookUpEdit1.EditValue;
                }
                gc.DataSource = dtM;
            }
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

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            if (dr != null)
            {
                dr.Delete();
            }
        }
        private void infolink()
        {
            DateTime t = CPublic.Var.getDatetime().Date.AddDays(7);
            foreach (DataRow dr in dtM.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                try
                {
                    if (dr["仓库号"].ToString() == "")
                    {
                        DataRow[] r = dt_物料编码.Select(string.Format("物料编码='{0}'", dr["物料编码"]));

                        dr["物料名称"] = r[0]["物料名称"].ToString();

                        dr["规格型号"] = r[0]["规格型号"].ToString();

                        dr["仓库号"] = r[0]["仓库号"].ToString();
                        dr["仓库名称"] = r[0]["仓库名称"].ToString();
                    }
                }
                catch (Exception ex)
                {

                }

            }

        }

        //填金额自动计算单价
        private void fun_cal单价()
        {
            try
            {
                Decimal s = 0;
                decimal shlv = 0;
                if (textBox2.Text.Trim() != "")
                {
                    shlv = Convert.ToDecimal(textBox2.Text) / (decimal)100;
                }
                foreach (DataRow r in dtM.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    if (r["数量"].ToString() != "" && r["含税金额"].ToString() != "")
                    {
                       
                        r["含税单价"] = Math.Round(Convert.ToDecimal(r["含税金额"]) / Convert.ToDecimal(r["数量"]),2,MidpointRounding.AwayFromZero);  
                        if (shlv == 0)
                        {
                             
                            r["不含税单价"] = r["含税单价"];
                            r["不含税金额"] = r["含税金额"];
                        }
                        else
                        {
                            
                            r["不含税单价"] = Math.Round( Convert.ToDecimal(r["含税单价"]) / (1 + shlv),6,MidpointRounding.AwayFromZero);
                            r["不含税金额"] = Math.Round(Convert.ToDecimal(r["含税金额"]) / (1 + shlv),2,MidpointRounding.AwayFromZero);
                        }
                        s += Convert.ToDecimal(r["含税金额"]);
                         
                    }
                    else if ((r["数量"] == DBNull.Value || r["数量"].ToString() == "") && r["含税单价"].ToString() != "")
                    {
                        if (shlv == 0)
                        {
                            
                            r["不含税单价"] = r["含税单价"];
                            r["不含税金额"] = r["含税金额"];
                        }
                        else
                        {
                            r["不含税单价"] = Math.Round(Convert.ToDecimal(r["含税单价"]) / (1 + shlv),6,MidpointRounding.AwayFromZero);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //填单价自动计算金额
        private void fun_cal金额()
        {
            try
            {
                Decimal s = 0;
                decimal shlv = 0;
                if (textBox2.Text.Trim() != "")
                {
                    shlv = Convert.ToDecimal(textBox2.Text) / (decimal)100;
                }
                foreach (DataRow r in dtM.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    if (r["数量"].ToString() != "" && r["含税单价"].ToString() != "")
                    {
                        
                        r["含税金额"] =Math.Round( Convert.ToDecimal(r["含税单价"]) * Convert.ToDecimal(r["数量"]),2,MidpointRounding.AwayFromZero);  //金额
                        if (shlv == 0)
                        {
                           
                            r["不含税单价"] = r["含税单价"];
                            r["不含税金额"] = r["含税金额"];
                        }
                        else
                        {
                         
                            r["不含税单价"] = Math.Round(Convert.ToDecimal(r["含税单价"]) / (1 + shlv),6,MidpointRounding.AwayFromZero);
                            r["不含税金额"] = Math.Round(Convert.ToDecimal(r["含税金额"]) / (1 + shlv),2,MidpointRounding.AwayFromZero);
                        }
                        s += Convert.ToDecimal(r["含税金额"]);
                      
                    }
                    else if ((r["数量"] == DBNull.Value || r["数量"].ToString() == "") && r["含税单价"].ToString() != "")
                    {
                        if (shlv == 0)
                        {
                           
                            r["不含税单价"] = r["含税单价"];
                            r["不含税金额"] = r["含税金额"];
                        }
                        else
                        {
                        
                            r["不含税单价"] = Math.Round(Convert.ToDecimal(r["含税单价"]) / (1 + shlv),6,MidpointRounding.AwayFromZero);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void gv_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            DataRow r = gv.GetDataRow(gv.FocusedRowHandle);
            try
            {
                r[e.Column.FieldName] = e.Value;
            }
            catch 
            {
                r[e.Column.FieldName] = 0;
            }
           
            if (e.Column.FieldName == "数量" || e.Column.FieldName=="含税单价") 
            {
               
                fun_cal金额();
            }
            if (e.Column.FieldName == "含税金额")
            {
                fun_cal单价();
            }
            else if (e.Column.FieldName == "物料编码")
            {
                try
                {
                    // DataRow d = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
                    DataRow drr = gv.GetDataRow(gv.FocusedRowHandle);
                    drr["物料编码"] = e.Value;
                    DataRow[] ds = dt_物料编码.Select(string.Format("物料编码 = '{0}'", e.Value));
                    drr["物料编码"] = ds[0]["物料编码"];
                    drr["物料名称"] = ds[0]["物料名称"];
                    drr["规格型号"] = ds[0]["规格型号"];
                    drr["仓库号"] = ds[0]["仓库号"];
                    drr["仓库名称"] = ds[0]["仓库名称"];
 
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void gv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                if (gv.FocusedColumn.Caption == "物料编码") infolink();
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal dec = 0;
                if (!decimal.TryParse(textBox2.Text, out dec))
                {
                    textBox2.Text = "0";
                    //throw new Exception("输入内容有误");
                }
                Decimal dec税率 = Convert.ToDecimal(textBox2.Text.ToString()) / (decimal)100.00;
                // DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);

                if (dtM != null )
                {
                    foreach (DataRow drrrr in dtM.Rows)
                    {
                        if (drrrr["含税单价"].ToString() == "")
                            continue;

                        if (Convert.ToDecimal(drrrr["含税单价"]) >= (Decimal)0)
                        {

                            drrrr["不含税单价"] = Math.Round((Convert.ToDecimal(drrrr["含税单价"]) / ((Decimal)1 + dec税率)), 6);

                        }


                    }
                    fun_cal金额();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
        private void fun_check()
        {
            if(textBox1.Text.Trim()=="")
            {
                throw new Exception("供应商未选择");
            }
            if (textBox2.Text.Trim() == "")
            {
                throw new Exception("税率不可为空");
            }
            if(dtM.DefaultView.Count==0)
            {
                throw new Exception("没有明细");
            }
            foreach (DataRow dr in dtM.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                if (Convert.ToDecimal(dr["数量"]) <= 0)
                {
                    throw new Exception("退货数量不能小于等于0");
                }
                if( dr["含税单价"].ToString()=="")
                {
                    throw new Exception("含税单价为空");
                }
                if ( dr["含税金额"].ToString() == "")
                {
                    throw new Exception("含税金额为空");
                }
            }
        }
        private void fun_save()
        {
            DateTime t = CPublic.Var.getDatetime();
            string str_prsq = "";

            string sql_主 = string.Format("select * from 采购退货申请主表 where 退货申请单号 = '{0}'", txt_退货申请单号.Text);
            dt_申请主 = CZMaster.MasterSQL.Get_DataTable(sql_主, strcon);
            if (dt_申请主.Rows.Count == 0)
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
                r_z["类型"] ="无关联退货";

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


        //生效
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
    }
}
