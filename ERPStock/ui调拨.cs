using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ERPStock
{
    public partial class ui调拨 : UserControl
    {


        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dtM;
        DataTable dtP;
        DataRow drM = null;
        DataTable dt_物料;
        Boolean s_提交 = false;
        DataTable dt_仓库;

        public ui调拨(string s_调拨申请单号, DataRow dr, DataTable dt)
        {
            InitializeComponent();
           // str_新增 = false;
          //  str_单号 = s_销售订单号;
            drM = dr;
            dtP = dt;
            s_提交 = true;
        }
        #endregion

        public ui调拨()
        {
            InitializeComponent();
        }

        public ui调拨(DataRow dr)
        {
            InitializeComponent();
            drM = dr;
        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DataRow dr = dtP.NewRow();
            dr["GUID"] = System.Guid.NewGuid();
            dtP.Rows.Add(dr);
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                dr.Delete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
      

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        private void fun_载入主表明细()
        {
            if (drM == null)
            {
                string sql = "select * from 调拨申请主表 where 1<>1";
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                drM = dtM.NewRow();
                dtM.Rows.Add(drM);

                sql = @"select a.*,物料名称,库存总数  from 调拨申请明细表 a ,仓库物料数量表 b 
                        where   a.物料编码=b.物料编码     and 1<>1";
                dtP = new DataTable();
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtP);
          
            }
            else
            {
                string sql = string.Format("select * from 调拨申请主表 where 调拨申请单号 = '{0}'", drM["调拨申请单号"].ToString());
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);

                drM = dtM.Rows[0];
                 dataBindHelper1.DataFormDR(drM);
                 searchLookUpEdit1.EditValue = drM["目标仓库号"].ToString().Trim();
                string sql2 = string.Format(@"select  a.*,物料名称,b.规格型号,库存总数   from 调拨申请明细表 a ,仓库物料数量表 b 
                where a.物料编码=b.物料编码  and 原仓库号=b.仓库号  and 调拨申请单号='{0}'", drM["调拨申请单号"].ToString());
                dtP = new DataTable();
                SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
                da2.Fill(dtP);
                
            }
            gc.DataSource = dtP;
        }
        private void fun_保存主表明细()
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();
                   
                //DateTime t = new DateTime(2019, 7, 14);

                if (txt_申请单号.Text == "") 
                {
                        txt_申请单号.Text = string.Format("MTA{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                             t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("MTA", t.Year, t.Month).ToString("0000"));
                        drM["调拨申请单号"] = txt_申请单号.Text;
                        drM["申请日期"] = t;
                }
                //drM["生效"] = true;
                //drM["生效人员ID"] = CPublic.Var.LocalUserID;
                //drM["生效人"] = CPublic.Var.localUserName;
                drM["申请人员"] = CPublic.Var.localUserName;
                //drM["生效日期"] = t;
                drM["备注"] = textBox2.Text;
                drM["目标仓库号"] = searchLookUpEdit1.EditValue;
                drM["目标仓库"] = textBox1.Text;
                if (Convert.ToBoolean(s_提交))
                {
                    drM["提交审核"] = true;
                }

                int i = 1;
                foreach (DataRow r in dtP.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    if (r["GUID"].ToString() == "")
                    {
                        r["GUID"] = System.Guid.NewGuid();
                    }
                    r["调拨申请单号"] = drM["调拨申请单号"];
                    r["调拨申请明细号"] = drM["调拨申请单号"].ToString() + "-" + i.ToString("00");
                    r["POS"] = i++;
               
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("生效");
            try
            {
             
                    string sql = "select * from  调拨申请主表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dtM);
                    }

                    sql = "select * from 调拨申请明细表 where 1<>1";
                    cmd = new SqlCommand(sql, conn, ts);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dtP);
                    }
               
                ts.Commit();

            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw ex;
            }
        }
        private void fun_物料下拉框()
        {
            string sql = @"select base.物料编码,base.物料名称,base.规格型号,base.图纸编号,isnull(a.库存总数,0)库存总数,a.货架描述,a.仓库号,a.仓库名称,ISNULL(正在申请数量,0)已申请未入库数量 from 基础数据物料信息表 base
            left join 仓库物料数量表 a on base.物料编码 = a.物料编码 
            left join (select *,(数量-已处理数量)正在申请数量 from (              
         select 调拨申请明细表.物料编码,调拨申请明细表.原仓库号,sum(调拨申请明细表.数量) 数量,sum(调拨申请明细表.已处理数量) 已处理数量   from 调拨申请明细表 
         left join 调拨申请主表 on 调拨申请明细表.调拨申请单号 = 调拨申请主表.调拨申请单号 
         where 审核 =0 and 提交审核 =1 and 调拨申请明细表.完成 = 0    and 调拨申请主表.作废 =0
         group by 调拨申请明细表.物料编码,原仓库号) aaa )bbb on bbb.物料编码 = a.物料编码 and bbb.原仓库号 = a.仓库号   ";
            dt_物料 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_物料);
            repositoryItemSearchLookUpEdit1.DataSource = dt_物料;
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
            sql = @"select 属性字段1 as 仓库编号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 = '仓库类别'order by 仓库编号 ";
            dt_仓库 = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_仓库);
            searchLookUpEdit1.Properties.DataSource = dt_仓库;
            searchLookUpEdit1.Properties.DisplayMember = "仓库编号";
            searchLookUpEdit1.Properties.ValueMember = "仓库编号";

            repositoryItemSearchLookUpEdit2.DataSource = dt_仓库;
            repositoryItemSearchLookUpEdit2.ValueMember = "仓库编号";
            repositoryItemSearchLookUpEdit2.DisplayMember = "仓库编号";
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
            {
                textBox1.Text = "";
            }
            else
            {
                DataRow[] xr = dt_仓库.Select(string.Format("仓库编号='{0}'", searchLookUpEdit1.EditValue));
                textBox1.Text = xr[0]["仓库名称"].ToString();
            }
        }
        private void fun_check()
        {
            if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString().Trim() == "") throw new Exception("目标仓库未选择");
            if (dtP.Rows.Count ==0)
            {
                throw new Exception("没有明细，请确认");
            }
            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                try
                {
                    Convert.ToDecimal(dr["数量"]);
                }
                catch (Exception)
                {

                    throw new Exception("数量输入不正确");
                }
              
                DataRow[] r = dtP.Select(string.Format("物料编码='{0}'", dr["物料编码"].ToString()));
                if (r.Length > 1)
                {
                    throw new Exception(string.Format("选择了重复物料{0},请确认", dr["物料编码"]));
                }

                if (Convert.ToDecimal(dr["数量"]) > Convert.ToDecimal(dr["库存总数"]))
                {
                    throw new Exception(string.Format("选择物料申请调拨数量大于库存数量,物料:{0}", dr["物料编码"]));

                }
                if (dr["原仓库号"].ToString() == searchLookUpEdit1.EditValue.ToString().Trim())
                {
                    throw new Exception("明细中有原仓库与目标仓库一样,请确认");
                }

            }


            if (textBox2.Text.ToString().Trim() == "")
            {
                throw new Exception("备注为必填项,请填写");

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
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow d = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (d != null)
                {
                   dr["物料名称"] = d["物料名称"];
                    dr["物料编码"] = d["物料编码"];
                    dr["规格型号"] = d["规格型号"];
                    dr["库存总数"] = d["库存总数"];
                    dr["新货架描述"] = d["货架描述"];
                    dr["原仓库号"] = d["仓库号"];
                    dr["原仓库"] = d["仓库名称"];
                }
                else
                {
                    dr["物料编码"] = "";
                    dr["物料名称"] = "";
                    dr["规格型号"] = "";
                    dr["库存总数"] = "";
                    dr["新货架描述"] = "";
                    dr["原仓库"] = "";
                    dr["原仓库号"] = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            DataRow d = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);

                if (d != null)
                {
                    dr["物料名称"] = d["物料名称"];
                    dr["物料编码"] = d["物料编码"];
                    dr["规格型号"] = d["规格型号"];
                    dr["库存总数"] = d["库存总数"];
                    dr["新货架描述"] = d["货架描述"];
                    dr["原仓库号"] = d["仓库号"];
                    dr["原仓库"] = d["仓库名称"];
                }
                else
                {
                    dr["物料编码"] = "";
                    dr["物料名称"] = "";
                    dr["规格型号"] = "";
                    dr["库存总数"] = "";
                    dr["新货架描述"] = "";
                    dr["原仓库"] = "";
                    dr["原仓库号"] = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            txt_申请单号.Text = "";
        
            searchLookUpEdit1.EditValue = null;
            textBox2.Text = "";
            fun_物料下拉框();
            string sql = "select * from 调拨申请主表 where 1<>1";
            dtM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            drM = null;
          
            fun_载入主表明细();
 
        }

        private void ui调拨_Load(object sender, EventArgs e)
        {
            try
            {
                fun_物料下拉框();
                fun_载入主表明细();
                if (Convert.ToBoolean(s_提交))
                {
                    fun_编辑();
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_编辑()
        {
            barLargeButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barLargeButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            searchLookUpEdit1.Enabled = !s_提交;
            textBox2.Enabled = !s_提交;
            simpleButton1.Enabled = !s_提交;
            simpleButton2.Enabled = !s_提交;
            gv.OptionsBehavior.Editable = !s_提交;




        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                gv.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                fun_check();

                fun_保存主表明细();
                fun_载入主表明细();
                MessageBox.Show("保存成功");
               // barLargeButtonItem3_ItemClick(null, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                 if (txt_申请单号.Text == "") throw new Exception("先保存后提交");
                if (MessageBox.Show(string.Format("该调拨单是否确认提交审核？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    //string depname = "";
                    //string departmentID=CPublic.Var.localUser课室编号;
                    //if (departmentID == "") departmentID = CPublic.Var.localUser部门编号;
                    //string s=string.Format("select 部门名称  from  人事基础部门表 where 部门编号='{0}'",departmentID);
                    //DataTable  dt=CZMaster.MasterSQL.Get_DataTable(s,strconn);
                    //if (dt.Rows.Count > 0) depname = dt.Rows[0]["部门名称"].ToString();
                    //  DataTable dt_审核 = ERPorg.Corg.fun_PA("生效","调拨申请单", txt_申请单号.Text, depname);
                    // CZMaster.MasterSQL.Save_DataTable(dt_审核,"单据审核申请表",strconn);

                    s_提交 = true;
                    fun_保存主表明细();
                    fun_载入主表明细();
                    MessageBox.Show("已提交审核");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        private void gc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                if (gv.FocusedColumn.Caption == "物料编码") infolink();
                else if (gv.FocusedColumn.Caption == "当前仓库号")
                {
                    infolink_stock();

                }
            }
            
        }

        private void infolink_stock()
        {
            try
            {
                foreach (DataRow dr in dtP.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted) continue;
                  
                    string sql = "select * from 仓库物料数量表 where 物料编码 = '" + dr["物料编码"] + "' and 仓库号 = '" + dr["原仓库号"] + "'";
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_物料数量 = new DataTable();
                    da.Fill(dt_物料数量);
                    if (dt_物料数量.Rows.Count == 0)
                    {
                        DataRow[] ds = dt_仓库.Select(string.Format("仓库编号 = '{0}'", dr["原仓库号"]));
                        dr["原仓库"] = ds[0]["仓库名称"];
                        dr["库存总数"] = 0;
                    }
                    else
                    {
                        dr["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
                        dr["原仓库号"] = dt_物料数量.Rows[0]["仓库号"].ToString();
                        dr["原仓库"] = dt_物料数量.Rows[0]["仓库名称"].ToString();

                    }


 

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void infolink()
        {
            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                try
                {
                    if (dr["原仓库号"].ToString() == "")
                    {
                        DataRow[] r = dt_物料.Select(string.Format("物料编码='{0}'  ", dr["物料编码"]));

                        dr["物料名称"] = r[0]["物料名称"];
                        dr["物料编码"] = r[0]["物料编码"];
                        dr["规格型号"] = r[0]["规格型号"];

                        dr["库存总数"] = r[0]["库存总数"];
                        //dr["货架描述"] = r[0]["货架描述"];

                        dr["原仓库号"] = r[0]["仓库号"].ToString();
                        dr["原仓库"] = r[0]["仓库名称"].ToString();
                    }
                }
                catch (Exception ex)
                {

                }

            }
        }

        private void gv_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                this.BindingContext[dtP].EndCurrentEdit();
                DataRow dr_focus = gv.GetDataRow(gv.FocusedRowHandle);
                if (e.Column.Caption == "当前仓库号")
                {
                    string sql = string.Format("select * from 仓库物料数量表 where 物料编码='{0}' and 仓库号='{1}'", dr_focus["物料编码"], e.Value);
                    DataRow dr = CZMaster.MasterSQL.Get_DataRow(sql, strconn);

                    string sql4 = string.Format("select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别' and 属性字段1='{0}'  ", e.Value);
                    DataRow wqwds = CZMaster.MasterSQL.Get_DataRow(sql4, strconn);
                    dr_focus["原仓库号"] = wqwds["仓库号"];
                    dr_focus["原仓库"] = wqwds["仓库名称"];
                    if (dr != null)
                    {
                        dr_focus["库存总数"] = dr["库存总数"];
                        if (dr_focus["库存总数"].ToString() == "")
                        {
                            dr_focus["库存总数"] = 0;
                        }
                    }
                    else
                    {
                        dr_focus["库存总数"] = 0;
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
