using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;



namespace ERPreport
{
    public partial class ui模具开票 : UserControl
    {

        #region 变量
        string strcon = CPublic.Var.strConn;
        DataTable dt_待办;
        DataTable dt_客户;
        DataTable dtM;
        DataTable dtP; //记录勾选的记录 下面一张
        #endregion


        public ui模具开票()
        {
            InitializeComponent();
        }
        private void fun_待办()
        {

            string sql = string.Format(@"select 模具合同台账明细表.*,模具合同台账主表.合同编号,模具合同台账主表.厂商编号,模具合同台账主表.模具厂商,开票数量=数量 from 模具合同台账明细表
                                       left join 模具合同台账主表  on 模具合同台账主表.模具订单号=模具合同台账明细表.模具订单号
                                        where 未开票数量>0");
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                dt_待办 = new DataTable();
                da.Fill(dt_待办);
                dt_待办.Columns.Add("选择", typeof(bool));
                gridControl1.DataSource = dt_待办;
            }

        }
        private void fun_initialize()
        {
          


            //加载客户
            string sql_client = "select 客户编号,客户名称 from 客户基础信息表 where 停用=0";
            dt_客户 = new DataTable();
            dt_客户 = CZMaster.MasterSQL.Get_DataTable(sql_client, strcon);
            searchLookUpEdit1.Properties.DataSource = dt_客户;
            searchLookUpEdit1.Properties.ValueMember = "客户编号";
            searchLookUpEdit1.Properties.DisplayMember = "客户编号";



            // 模具开票通知单主表
            string sql_z = "select * from 模具开票通知单主表 where 1<>1";
            dtM = new DataTable();
            dtM = CZMaster.MasterSQL.Get_DataTable(sql_z, strcon);

            //加载 dtp 空表 模具开票通知明细表
            string sql_mx = "select * from 模具开票通知明细表 where 1<>1 ";
            dtP = new DataTable();
            dtP = CZMaster.MasterSQL.Get_DataTable(sql_mx, strcon);
            gridControl2.DataSource = dtP;

            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            searchLookUpEdit1.EditValue = "";
            textBox4.Text = "";
            textBox5.Text = "";

        }
        private void fun_save()
        {
             

            //主表
            string str_mdn = string.Format("Mdk{0}{1}{2}{3}", CPublic.Var.getDatetime().Year.ToString(), CPublic.Var.getDatetime().Month.ToString("00"),
                             CPublic.Var.getDatetime().Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("Mdk", CPublic.Var.getDatetime().Year, CPublic.Var.getDatetime().Month).ToString("0000"));
            textBox1.Text = str_mdn;
            DataRow dr = dtM.NewRow();
            dr["模具开票通知号"] = str_mdn;
            dr["总金额"] = textBox2.Text;
            dr["厂商编号"] = searchLookUpEdit1.EditValue;
            dr["厂商名称"] = textBox5.Text;
            dr["生效"] = true;
            dr["生效日期"] = CPublic.Var.getDatetime();
            dr["操作人"] = CPublic.Var.localUserName;
            dr["操作人ID"] = CPublic.Var.LocalUserID;
            dtM.Rows.Add(dr);
            //明细表
            int i = 0;
            foreach (DataRow x in dtP.Rows)
            {
                i++;
            
                x["模具开票通知号"] = str_mdn;
                x["模具开票通知明细号"] = str_mdn + i.ToString("00");
                //x["合同编号"] = x["合同编号"];
                //r["合同金额"] = x["合同金额"];
                x["生效"] = true;
                x["生效日期"] = CPublic.Var.getDatetime();
                x["生效人"] = CPublic.Var.localUserName;

                //选中的 已开票状态 
                DataRow[] r_待办 = dt_待办.Select(string.Format("明细号='{0}'", x["模具订单明细号"]));
                r_待办[0]["已开票数量"] = x["开票数量"];
                r_待办[0]["未开票数量"] =   Convert.ToDecimal(r_待办 [0]["数量"])- Convert.ToDecimal(x["开票数量"]);



            }
            string sql = "select * from 模具合同台账明细表 where 1<>1";
            string sql_M = "select * from 模具开票通知单主表 where 1<>1";
            string sql_detail = "select * from 模具开票通知明细表 where 1<>1";
            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("模具开票");
            try
            {

                SqlCommand cmm_1 = new SqlCommand(sql_M, conn, ts);
                SqlCommand cmm_2 = new SqlCommand(sql_detail, conn, ts);
                SqlCommand cmm_3 = new SqlCommand(sql, conn, ts);

                SqlDataAdapter da = new SqlDataAdapter(cmm_3);
                SqlDataAdapter da_M = new SqlDataAdapter(cmm_1);
                SqlDataAdapter da_detail = new SqlDataAdapter(cmm_2);

                new SqlCommandBuilder(da);
                new SqlCommandBuilder(da_M);
                new SqlCommandBuilder(da_detail);

                da.Update(dt_待办);
                da_M.Update(dtM);
                da_detail.Update(dtP);
                ts.Commit();
            }
            catch
            {
                ts.Rollback();
                throw new Exception("生效失败");
            }
        }
        private void ui模具开票_Load(object sender, EventArgs e)
        {
            try
            {
                fun_待办();

                fun_initialize();
                textBox4.Text = CPublic.Var.localUserName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);   
            }
           
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString("00");
            }
        }
        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ui模具开票_Load(null, null);
        }
        //生效
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridView1.CloseEditor();
                this.BindingContext[dt_待办].EndCurrentEdit();
                fun_save();
                //刷新
                fun_initialize();
                fun_待办();
                MessageBox.Show("生效成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //关闭
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void repositoryItemCheckEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                gridView1.CloseEditor();
                gridControl1.BindingContext[dt_待办].EndCurrentEdit();

                DataRow drr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
                {
                    if (searchLookUpEdit1.EditValue.ToString() != drr["厂商编号"].ToString())
                    {
                        drr["选择"] = false;
                        throw new Exception("选择了不同厂商的记录");
                    }

                }


                if (drr["选择"].Equals(true))
                {
                    DataRow dr = dtP.NewRow();
                    dtP.Rows.Add(dr);
                    dr["合同编号"] = drr["合同编号"].ToString();
                    dr["金额"] = drr["金额"].ToString();
                    dr["模具订单明细号"] = drr["明细号"].ToString();
                    dr["模具编号"] = drr["模具编号"].ToString();
                    dr["零件图号"] = drr["零件图号"].ToString();
                    dr["模具单价"] = drr["模具单价"].ToString();
                    dr["开票数量"] = drr["开票数量"].ToString();



           
                  
                    if (searchLookUpEdit1.EditValue == null ||searchLookUpEdit1.EditValue.ToString() == "")
                    {
                        searchLookUpEdit1.EditValue = drr["厂商编号"].ToString();
                    }
                }
                else
                {
                    DataRow[] r = dtP.Select(string.Format("模具订单明细号='{0}'", drr["明细号"]));
                    r[0].Delete();
                    if (dtP.Rows.Count == 0)
                    {
                        searchLookUpEdit1.EditValue = "";
                    }

                }

                System.Decimal sum = 0;

                gridControl2.DataSource = dtP;
                foreach (DataRow r in dtP.Rows)
                {
                    if (r.RowState == DataRowState.Deleted)
                    {

                        continue;
                    }
                    try
                    {
                        sum += (Decimal)r["金额"];

                    }
                    catch
                    { }
                }
                textBox2.Text = sum.ToString();

                //gridView1.MoveLast();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
          
                DataRow[] r = dt_客户.Select(string.Format("客户编号='{0}'", searchLookUpEdit1.EditValue));
                if (r.Length > 0)
                {
                    textBox5.Text = r[0]["客户名称"].ToString();
                }
                else
                {
                    textBox5.Text = "";
                }
             

        }




    }
}
