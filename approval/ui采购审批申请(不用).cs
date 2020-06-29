using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace approval
{
    public partial class ui采购审批申请 : UserControl
    {
        #region  变量

        DataTable dtM;
        DataTable dt_采购单;

        string strcon = CPublic.Var.strConn;
        DataTable dt_申请主;


        #endregion

        public ui采购审批申请()
        {
            InitializeComponent();
        }
        //生效
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv1.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                fun_check();
                fun_save();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
          
        }
        private void fun_check()
        {
            if (dtM.Rows.Count == 0)
            {
                throw new Exception("没有明细,请确认");
            }
            if (comboBox1.Text=="")
            {
                throw new Exception("未选择类型");
            }
        }
        //根据配置表  获取审核人
        private void fun_审核人()
        {
       



        }
        private void fun_save()
        {
            DateTime t = CPublic.Var.getDatetime();
            string str_pa = string.Format("PA{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("PA", t.Year, t.Month));
            textBox1.Text = str_pa;
            // 申请主表记录
            DataRow r_z = dt_申请主.NewRow();
            r_z["审核申请单号"] = str_pa;
            r_z["申请时间"] = t;
            r_z["申请人ID"] = CPublic.Var.LocalUserID;
            r_z["申请人"] = CPublic.Var.localUserName;
            r_z["总金额"] = Convert.ToDecimal(textBox3.Text);
         

            dt_申请主.Rows.Add(r_z);
 
            int i = 1;
            foreach (DataRow r in dtM.Rows)
            {
                r["审核申请单号"] = str_pa;
                r["POS"] = i;
                r["审核申请明细号"] = str_pa + "-" + i++.ToString("00");
            }
 

            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction cgthsq = conn.BeginTransaction("审核申请");
            try
            {
                {
                    string sql = "select * from  单据审核申请主表  where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, cgthsq);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dt_申请主);

                    }
                }
                {
                    string sql = "select * from 单据审核申请子表 where 1<>1";
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
        private void fun_load()
        {
            DateTime t=CPublic.Var.getDatetime();
            t=t.AddMonths(-1);
            string sql = string.Format("select  * from 采购记录采购单主表 where 操作员ID='{0}' and 创建日期>'{1}'", CPublic.Var.LocalUserID, t);
                using(SqlDataAdapter da =new SqlDataAdapter (sql,strcon))
                {
                    dt_采购单 = new DataTable();
                    da.Fill(dt_采购单);
                    sl_采购单.Properties.DataSource = dt_采购单;
                    sl_采购单.Properties.ValueMember = "采购单号";
                    sl_采购单.Properties.DisplayMember = "采购单号";


                }
                sql = "select  a.*,供应商ID,供应商,税率,总金额 as 金额 from  单据审核申请子表 a left join 采购记录采购单主表  b on 采购单号=关联单号  where 1<>1 ";
                dtM = CZMaster.MasterSQL.Get_DataTable(sql,strcon);
                gc1.DataSource = dtM;
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);
            if (dr != null)
            {
                dr.Delete();
            }

        }

        private void sl_采购单_EditValueChanged(object sender, EventArgs e)
        {
            if (sl_采购单.EditValue != null && sl_采购单.EditValue.ToString() != "")
            {
                DataRow[] dr = dt_采购单.Select(string.Format("采购单号='{0}'", sl_采购单.EditValue));
            
                DataRow r = dtM.NewRow();
      
                r["税率"] = dr[0]["税率"];
                r["供应商ID"] = dr[0]["供应商ID"];
                r["供应商"] = dr[0]["供应商"];
                r["关联单号"] = sl_采购单.EditValue;
                r["金额"] = dr[0]["总金额"];
                dtM.Rows.Add(r);
            }
        }
        private decimal fun_计算金额()
        {
            decimal dec = 0;
            if (dtM.Rows.Count > 0)
            {
                foreach (DataRow dr in dtM.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted) continue;
                    dec = dec + Convert.ToDecimal(dr["金额"]);

                }
            }
            return dec;

        }

        private void gv1_RowCountChanged(object sender, EventArgs e)
        {
            try
            {
                if (gv1.RowCount > 0)
                {
                    decimal dec = fun_计算金额();
                    textBox3.Text = Math.Round(dec, 2).ToString();
                }
                else
                {
                    textBox3.Text = "0";
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void ui采购审批申请_Load(object sender, EventArgs e)
        {
            try
            {
                fun_load();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
