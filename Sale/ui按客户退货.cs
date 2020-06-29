using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ERPSale
{
    public partial class ui按客户退货 : UserControl
    {
        DataTable dt_客户;
        string strcon = CPublic.Var.strConn;
        DataTable dt_物料;
        DataTable dtM;
        DataTable dtP;

        DataRow drM = null;

        public ui按客户退货()
        {
            InitializeComponent();
        }


        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //生效
            try
            {
                gv.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                fun_check();

                fun_保存主表明细(true);
                MessageBox.Show("生效成功");
                refsh(txt_申请单号.Text);



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ui按客户退货_Load(object sender, EventArgs e)
        {
            try
            {
                fun_load();
                fun_载入主表明细();
                gc.DataSource = dtP;
            }
            catch (Exception ex)
            {

            }

        }

        private void fun_load()
        {
            string sql = string.Format(@"select 客户编号,客户名称 from 客户基础信息表");
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            dt_客户 = new DataTable();
            da.Fill(dt_客户);
            searchLookUpEdit1.Properties.DataSource = dt_客户;
            searchLookUpEdit1.Properties.DisplayMember = "客户编号";
            searchLookUpEdit1.Properties.ValueMember = "客户编号";

            ///select base.物料编码,base.物料名称,base.规格型号,isnull(a.库存总数,0)库存总数,a.货架描述
           //,a.仓库号,a.仓库名称, base.仓库号 as 默认仓库号,base.仓库名称 as 默认仓库
           //from 基础数据物料信息表 base
           //left join 仓库物料数量表 a on base.物料编码 = a.物料编码 and  base.仓库号 = a.仓库号
           //where(内销 = 1 or 外销 = 1) and 停用 = 0
            sql = @"select base.物料编码,base.物料名称,base.规格型号 
           from 基础数据物料信息表 base   where (内销=1 or 外销=1) and 停用=0";
            dt_物料 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            repositoryItemSearchLookUpEdit1.DataSource = dt_物料;
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";

            time_申请日期.EditValue = Convert.ToDateTime("2017-1-1");


        }
        private void fun_check()
        {
            if (textBox2.Text == "")
            {
                throw new Exception("业务单据未输入");
            }
            if (time_申请日期.EditValue == null && time_申请日期.EditValue.ToString() == "")
            {
                throw new Exception("业务单据日期未选择");
            }
            if(Convert.ToDateTime(time_申请日期.EditValue)> Convert.ToDateTime("2018-1-1"))
            {
                throw new Exception("业务单据日期不可在2018年之后");
            }
            if (searchLookUpEdit1.EditValue == null && searchLookUpEdit1.EditValue.ToString() == "")
            {
                throw new Exception("客户未选择");
            }
            if (gv.DataRowCount == 0)
                throw new Exception("未添加数据");
            foreach(DataRow dr in  dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;

                decimal dec = 0;
                if (!decimal.TryParse(dr["数量"].ToString(), out dec)) throw new Exception("输入数量有误");

                if(dec<=0) throw new Exception("输入数量需大于0");
            }

        }

        private void refsh(string  s)
        {
            string sql = string.Format("select * from 退货申请主表 where 退货申请单号 = '{0}'",s);
            dtM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            da.Fill(dtM);
            drM = dtM.Rows[0];
            dataBindHelper1.DataFormDR(drM);

            string sql2 = string.Format(@"select 退货申请子表.*,a.物料名称,a.规格型号 from 退货申请子表
                left join 基础数据物料信息表 a on 退货申请子表.物料编码 = a.物料编码
                where 退货申请单号 = '{0}'", s);
            dtP = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(sql2, strcon);
            da2.Fill(dtP);
            gc.DataSource = dtP;
        }
        private void fun_载入主表明细()
        {
            if (drM == null)
            {
                string sql = "select * from 退货申请主表 where 1<>1";
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                da.Fill(dtM);
                drM = dtM.NewRow();
                dtM.Rows.Add(drM);

                sql = @"select 退货申请子表.*,a.物料名称,a.规格型号 from 退货申请子表 
                left join 基础数据物料信息表 a on 退货申请子表.物料编码 = a.物料编码  where 1<>1";
                dtP = new DataTable();
                da = new SqlDataAdapter(sql, strcon);
                da.Fill(dtP);
            }
            else
            {
                string sql = string.Format("select * from 退货申请主表 where 退货申请单号 = '{0}'", drM["退货申请单号"].ToString());
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                da.Fill(dtM);
                drM = dtM.Rows[0];
                dataBindHelper1.DataFormDR(drM);

                string sql2 = string.Format(@"select 退货申请子表.*,a.物料名称,a.规格型号 from 退货申请子表
                left join 基础数据物料信息表 a on 退货申请子表.物料编码 = a.物料编码
                where 退货申请单号 = '{0}'", drM["退货申请单号"].ToString());
                dtP = new DataTable();
                SqlDataAdapter da2 = new SqlDataAdapter(sql2, strcon);
                da2.Fill(dtP);

            }
         
            // dtP.ColumnChanged += dtP_ColumnChanged;
        }
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {


                int[] dr1 = gv.GetSelectedRows();
                if (dr1.Length > 0)
                {
                    for (int i = dr1.Length - 1; i >= 0; i--)
                    {
                        DataRow dr_选中 = gv.GetDataRow(dr1[i]);
                        dr_选中.Delete();
                    }
                    DataRow drs = gv.GetDataRow(Convert.ToInt32(dr1[0]));
                    if (drs != null) gv.SelectRow(dr1[0]);
                    else if (gv.GetDataRow(Convert.ToInt32(dr1[0]) - 1) != null)
                        gv.SelectRow(Convert.ToInt32(dr1[0]) - 1);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DataRow dr = dtP.NewRow();


            dtP.Rows.Add(dr);
        }


        private void infolink()
        {

            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                try
                {
                    DataRow[] r = dt_物料.Select(string.Format("物料编码='{0}'  ", dr["物料编码"]));
                    dr["物料名称"] = r[0]["物料名称"];
                    dr["物料编码"] = r[0]["物料编码"];
                    dr["规格型号"] = r[0]["规格型号"];
                }
                catch (Exception ex)
                {

                }

            }

        }

        private void gc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                if (gv.FocusedColumn.Caption == "物料编码") infolink();

            }
        }

        private void searchLookUpEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            try
            {
                if (e.NewValue==null  || e.NewValue.ToString() == "")
                {
                    textBox1.Text = "";
                    return;
                }

                //根据客户删选销售单
                textBox1.Text = dt_客户.Select(string.Format("客户编号='{0}'", e.NewValue))[0]["客户名称"].ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_保存主表明细(Boolean bl)
        {
            DateTime t = Convert.ToDateTime( time_申请日期.EditValue).Date; //业务单据日期
            DateTime time = CPublic.Var.getDatetime();

            string str_id = CPublic.Var.LocalUserID;
            string str_name = CPublic.Var.localUserName;
            try
            {


                if (drM["GUID"].ToString() == "")
                {
                    drM["GUID"] = System.Guid.NewGuid();
                    txt_申请单号.Text = string.Format("THSQ{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                        t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("THSQ", t.Year, t.Month).ToString("0000"));
                    drM["退货申请单号"] = txt_申请单号.Text;
                }
                drM["操作人员编号"] = str_id;
                drM["操作人员"] = str_name;
                if (drM["申请日期"] == null || drM["申请日期"].ToString() == "")
                    drM["申请日期"] = time;
                if (bl == true)
                {
                    drM["生效"] = true;
                    drM["生效人员编号"] = str_id;
                    drM["生效日期"] = time;
                    drM["完成"] = false;
                }
                dataBindHelper1.DataToDR(drM);
                drM["业务单据日期"] = t;
                drM["部门编号"] = CPublic.Var.localUser部门编号;
                drM["部门名称"] = CPublic.Var.localUser部门名称;
            }
            catch (Exception ex)
            {
                throw new Exception("主表保存出错" + ex.Message);
            }

            try
            {
                int i = 1;
                DataRow[] tr = dtP.Select("pos=max(pos)");
                if (tr.Length > 0) i = Convert.ToInt32(tr[0]["POS"])+1;
                foreach (DataRow r in dtP.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                 
                    if (r["GUID"].ToString() == "")
                    {
                        r["GUID"] = System.Guid.NewGuid();
                        r["退货申请单号"] = drM["退货申请单号"];
                        r["退货申请明细号"] = drM["退货申请单号"].ToString() + "-" + i.ToString("00");
                        r["POS"] = i++;
                    }
                    if (bl == true)
                    {
                        r["生效"] = true;
                        r["生效人员编号"] = str_id;
                        r["生效日期"] = t;
                        r["完成"] = false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("明细保存出错" + ex.Message);
            }

            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("生效");
            try
            {
                {
                    string sql = "select * from 退货申请主表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dtM);
                    }
                }
                {
                    string sql = "select * from 退货申请子表 where 1<>1";
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

        private void gv_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                try
                {
                    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
 
                    if (e.Column.Caption == "物料编码")
                    {

                        dr["物料编码"] = e.Value;
                        DataRow[] ds = dt_物料.Select(string.Format("物料编码 = '{0}'", dr["物料编码"].ToString()));

                        dr["物料名称"] = ds[0]["物料名称"];
                        dr["规格型号"] = ds[0]["规格型号"];
         
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
