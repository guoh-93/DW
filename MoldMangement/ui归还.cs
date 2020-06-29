using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
namespace MoldMangement
{
    public partial class ui归还 : UserControl
    {
        public ui归还()
        {
            InitializeComponent();
        }

        DataTable dt_仓库物料数量表;
        DataRow dr_借还;
        DataTable dt_仓库号;
        string sql_ck = "";
        string strSoNo = "";
        string strconn = CPublic.Var.strConn;
        DataTable dt_借还申请表附表;
        DataTable dt_借还申请表;
        DataTable dt_主;
        DataTable dt_借xi;
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();

        }
        private void Fun_下拉框选择项()
        {
            dt_仓库号 = new DataTable();
            string sql4 = "select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别'";
            SqlDataAdapter da = new SqlDataAdapter(sql4, strconn);
            da.Fill(dt_仓库号);
            repositoryItemLookUpEdit1.DataSource = dt_仓库号;
            repositoryItemLookUpEdit1.DisplayMember = "仓库号";
            repositoryItemLookUpEdit1.ValueMember = "仓库号";
        }
        string cfgfilepath = "";
        private void ui归还_Load(object sender, EventArgs e)
        {
            try

            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(this.splitContainer1.Panel1, this.Name, cfgfilepath);


                string sql = string.Format("select * from 归还申请主表 where 归还完成=0 and 作废 = 0 and 锁定 = 0");
                dt_主 = new DataTable();
                dt_主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                gc3.DataSource = dt_主;
                gc2.DataSource =null;
                Fun_下拉框选择项();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        
        }

        private void gv3_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {

            try

            {

                if (e.Clicks == 1)
                {
                    //判断右键菜单是否可用
                    if (e != null && e.Button == MouseButtons.Right)
                    {
                       // contextMenuStrip1.Show(gc3, new Point(e.X, e.Y));
                    }


                }




                    DataRow drM = (this.BindingContext[gc3.DataSource].Current as DataRowView).Row;

                string sql = string.Format("select * from  归还申请子表 where 归还批号='{0}' and 归还完成= 0 and 作废 = 0", drM["归还批号"]);
                dt_借xi = new DataTable();
                dt_借xi = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                if (dt_借xi.Columns.Contains("选择") != true)
                {
                    DataColumn dc = new DataColumn("选择", typeof(bool));
                    dc.DefaultValue = true;
                    dt_借xi.Columns.Add(dc);



                //    .Columns.Add("选择", typeof(bool));

                }
                if (dt_借xi.Columns.Contains("录入归还数量") != true)
                {
                    dt_借xi.Columns.Add("录入归还数量", typeof(decimal));

                }
                if (dt_借xi.Columns.Contains("最大归还数量") != true)
                {
                    dt_借xi.Columns.Add("最大归还数量", typeof(decimal));

                }

                foreach (DataRow dr  in dt_借xi.Rows )
                {

                    string sql213 = string.Format("select  货架描述 from 仓库物料数量表 where 物料编码='{0}' and 仓库号='{1}'",dr["物料编码"],dr["仓库号"]);
                    DataTable dt_hj = CZMaster.MasterSQL.Get_DataTable(sql213,strconn);
                    if (dt_hj.Rows.Count>0)
                    {
                        dr["货架描述"] = dt_hj.Rows[0]["货架描述"];
                    }

                    dr["录入归还数量"] =  decimal.Parse(dr["需归还数量"].ToString())-decimal.Parse((dr["申请已归还数量"].ToString()));
                    dr["最大归还数量"] = dr["录入归还数量"];
                }

                gc2.DataSource = dt_借xi;



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
         


            try

            {
                gv3.CloseEditor();
                this.BindingContext[dt_主].EndCurrentEdit();
                gv2.CloseEditor();
                this.BindingContext[dt_借xi].EndCurrentEdit();
                DataRow dr = (this.BindingContext[gc3.DataSource].Current as DataRowView).Row;

                string sql = string.Format("select * from 归还申请主表 where 归还批号 = '0'", dr["归还批号"]);
                DataTable dt_gui = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                if (dt_gui.Rows.Count > 0)
                {
                    if (Convert.ToBoolean(dt_gui.Rows[0]["作废"]))
                    {
                        throw new Exception("改单据已作废，请刷新");
                    }
                    if (Convert.ToBoolean(dt_gui.Rows[0]["归还完成"]))
                    {
                        throw new Exception("改单据已归还完成，请刷新");
                    }
                }

                DataSet ds = fun_save(dr);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    throw new Exception("没有勾选明细");
                }
                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction st = conn.BeginTransaction("归还");
                try
                {
                    SqlCommand cmd = new SqlCommand("select  * from 归还申请子表 where 1=2", conn, st);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(ds.Tables[0]);
                    cmd = new SqlCommand("select  * from 归还申请主表 where 1=2", conn, st);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(ds.Tables[1]);

                    cmd = new SqlCommand("select  * from 借还申请表附表 where 1=2", conn, st);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(ds.Tables[2]);

                    cmd = new SqlCommand("select  * from 借还申请表 where 1=2", conn, st);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(ds.Tables[3]);

                    cmd = new SqlCommand("select  * from 仓库物料数量表 where 1=2", conn, st);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(ds.Tables[4]);

                    cmd = new SqlCommand("select  * from 仓库出入库明细表 where 1=2", conn, st);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(ds.Tables[5]);

                    st.Commit();
                    MessageBox.Show("归还操作成功");
                    // barLargeButtonItem3_ItemClick(null, null);
                    ds.Tables[0].AcceptChanges();
                    ds.Tables[1].AcceptChanges();

                }
                catch (Exception ex)
                {
                    st.Rollback();
                    throw new Exception(ex.Message);
                }






            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




            barLargeButtonItem1_ItemClick(null,null);









        }
        private DataSet fun_赠送(DataTable dt_归还记录, DataTable dt_仓库出入库明细)
        {
            DataSet ds = new DataSet();
            DateTime t = CPublic.Var.getDatetime();

            DataTable dt_销售订单主表;
            DataTable dt_销售订单明细表;
            DataTable dt_出库通知单主表;
            DataTable dt_出库通知单明细表;
            DataTable dt_成品出库单主表;
            DataTable dt_成品出库单明细表;
            DataTable dt_客户;
            string s = "select * from 销售记录销售订单主表 where 1<>1";
            dt_销售订单主表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = "select * from 销售记录销售订单明细表 where 1<>1";
            dt_销售订单明细表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = "select * from 销售记录销售出库通知单主表 where 1<>1";
            dt_出库通知单主表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = "select * from 销售记录销售出库通知单明细表 where 1<>1";
            dt_出库通知单明细表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = string.Format("select * from 客户基础信息表 where 客户名称 = '{0}'", dr_借还["相关单位"]);
            dt_客户 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = "select * from 销售记录成品出库单主表 where 1<>1";
            dt_成品出库单主表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = "select * from 销售记录成品出库单明细表 where 1<>1";
            dt_成品出库单明细表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            //s = "select * from 仓库出入库明细表 where 1<>1";
            //dt_仓库出入库明细 = CZMaster.MasterSQL.Get_DataTable(s, strconn);


            string s_销售单号 = string.Format("SO{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
          t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SO", t.Year, t.Month).ToString("0000"));
            string s_出库通知单号 = string.Format("SK{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
          t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SK", t.Year, t.Month).ToString("0000"));
            string s_成品出库单号 = string.Format("SA{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
          t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SA", t.Year, t.Month).ToString("0000"));

            DataRow dr_销售订单主 = dt_销售订单主表.NewRow();
            dt_销售订单主表.Rows.Add(dr_销售订单主);
            dr_销售订单主["GUID"] = System.Guid.NewGuid();
            dr_销售订单主["销售订单号"] = s_销售单号;
            dr_销售订单主["录入人员"] = CPublic.Var.localUserName;
            dr_销售订单主["录入人员ID"] = CPublic.Var.LocalUserID;
            dr_销售订单主["待审核"] = true;
            dr_销售订单主["审核"] = true;
            dr_销售订单主["备注1"] = dt_仓库出入库明细.Rows[0]["相关单号"].ToString().Split('-')[0]; //记录借用申请单号


            if (dt_客户.Rows.Count > 0)
            {
                dr_销售订单主["客户编号"] = dt_客户.Rows[0]["客户编号"];
                dr_销售订单主["客户名"] = dr_借还["相关单位"];
                dr_销售订单主["税率"] = dt_客户.Rows[0]["税率"];
                dr_销售订单主["业务员"] = dt_客户.Rows[0]["业务员"];
                //dr_销售订单主["客户名"] = dt_客户.Rows[0]["客户名称"];
            }
            dr_销售订单主["日期"] = t;
            dr_销售订单主["销售备注"] = "借出转赠送";

            dr_销售订单主["税前金额"] = 0;
            dr_销售订单主["税后金额"] = 0;
            dr_销售订单主["生效"] = true;
            dr_销售订单主["生效日期"] = t;
            dr_销售订单主["生效人员"] = CPublic.Var.localUserName;
            dr_销售订单主["生效人员ID"] = CPublic.Var.LocalUserID;

            dr_销售订单主["创建日期"] = t;
            dr_销售订单主["修改日期"] = t;
            dr_销售订单主["完成"] = true;
            dr_销售订单主["完成日期"] = t;
            ds.Tables.Add(dt_销售订单主表);

            DataRow dr_出库通知单主 = dt_出库通知单主表.NewRow();
            dt_出库通知单主表.Rows.Add(dr_出库通知单主);
            dr_出库通知单主["GUID"] = System.Guid.NewGuid();
            dr_出库通知单主["出库通知单号"] = s_出库通知单号;
            if (dt_客户.Rows.Count > 0)
            {
                dr_出库通知单主["客户编号"] = dt_客户.Rows[0]["客户编号"];
                dr_出库通知单主["客户名"] = dt_客户.Rows[0]["客户名称"];
            }
            dr_出库通知单主["出库日期"] = t;
            dr_出库通知单主["创建日期"] = t;
            dr_出库通知单主["修改日期"] = t;
            dr_出库通知单主["操作员ID"] = CPublic.Var.LocalUserID;
            dr_出库通知单主["操作员"] = CPublic.Var.localUserName;
            dr_出库通知单主["生效"] = true;
            dr_出库通知单主["生效日期"] = t;
            ds.Tables.Add(dt_出库通知单主表);

            DataRow dr_成品出库主 = dt_成品出库单主表.NewRow();
            dt_成品出库单主表.Rows.Add(dr_成品出库主);
            dr_成品出库主["GUID"] = System.Guid.NewGuid();
            dr_成品出库主["成品出库单号"] = s_成品出库单号;
            dr_成品出库主["操作员ID"] = CPublic.Var.LocalUserID;
            dr_成品出库主["操作员"] = CPublic.Var.localUserName;
            if (dt_客户.Rows.Count > 0)
            {
                dr_成品出库主["客户"] = dt_客户.Rows[0]["客户名称"];
            }
            dr_成品出库主["日期"] = t;
            dr_成品出库主["创建日期"] = t;
            dr_成品出库主["修改日期"] = t;
            dr_成品出库主["生效"] = true;
            dr_成品出库主["生效日期"] = t;
            ds.Tables.Add(dt_成品出库单主表);

            int i = 1;
            foreach (DataRow dr in dt_归还记录.Rows)
            {
                DataRow dr_saleDetail = dt_销售订单明细表.NewRow();
                dt_销售订单明细表.Rows.Add(dr_saleDetail);
                dr_saleDetail["GUID"] = System.Guid.NewGuid();
                dr_saleDetail["销售订单号"] = s_销售单号;
                dr_saleDetail["POS"] = i;
                dr_saleDetail["销售订单明细号"] = s_销售单号 + "-" + i.ToString("00");
                dr_saleDetail["物料编码"] = dr["物料编码"];
                dr_saleDetail["数量"] = dr["归还数量"];
                dr_saleDetail["完成数量"] = dr["归还数量"];
                dr_saleDetail["未完成数量"] = 0;
                dr_saleDetail["已通知数量"] = dr["归还数量"];
                dr_saleDetail["未通知数量"] = 0;
                dr_saleDetail["物料名称"] = dr["物料名称"];
                //dr_销售订单子["n原ERP规格型号"] = dr["n原ERP规格型号"];
                dr_saleDetail["规格型号"] = dr["规格型号"];
                // dr_销售订单子["图纸编号"] = dr["图纸编号"];
                dr_saleDetail["仓库号"] = dr["仓库号"];
                dr_saleDetail["仓库名称"] = dr["仓库名称"];
                dr_saleDetail["计量单位"] = dr["计量单位"];
                // dr_saleDetail["销售备注"] = "借出转赠送";
                dr_saleDetail["税前金额"] = 0;
                dr_saleDetail["税后金额"] = 0;
                dr_saleDetail["税前单价"] = 0;
                dr_saleDetail["税后单价"] = 0;
                dr_saleDetail["送达日期"] = t;
                if (dt_客户.Rows.Count > 0)
                {
                    dr_saleDetail["客户编号"] = dt_客户.Rows[0]["客户编号"];
                    dr_saleDetail["客户"] = dt_客户.Rows[0]["客户名称"];
                }
                dr_saleDetail["生效"] = true;
                dr_saleDetail["生效日期"] = t;
                dr_saleDetail["明细完成"] = true;
                dr_saleDetail["明细完成日期"] = t;
                dr_saleDetail["总完成"] = true;
                dr_saleDetail["总完成日期"] = t;
                dr_saleDetail["已计算"] = true;
                dr_saleDetail["录入人员ID"] = CPublic.Var.LocalUserID;
                dr_saleDetail["含税销售价"] = 0;

                DataRow dr_stockOutNotice = dt_出库通知单明细表.NewRow();
                dt_出库通知单明细表.Rows.Add(dr_stockOutNotice);
                dr_stockOutNotice["GUID"] = System.Guid.NewGuid();
                dr_stockOutNotice["出库通知单号"] = s_出库通知单号;
                dr_stockOutNotice["POS"] = i;
                dr_stockOutNotice["出库通知单明细号"] = s_出库通知单号 + "-" + i.ToString("00");
                dr_stockOutNotice["销售订单明细号"] = dr_saleDetail["销售订单明细号"];
                dr_stockOutNotice["物料编码"] = dr["物料编码"];
                dr_stockOutNotice["物料名称"] = dr["物料名称"];
                dr_stockOutNotice["出库数量"] = dr["归还数量"];
                dr_stockOutNotice["规格型号"] = dr["规格型号"];
                //dr_stockOutNotice["图纸编号"] = dr["图纸编号"];
                dr_stockOutNotice["操作员ID"] = CPublic.Var.LocalUserID;
                dr_stockOutNotice["操作员"] = CPublic.Var.localUserName;
                dr_stockOutNotice["生效"] = true;
                dr_stockOutNotice["生效日期"] = t;
                dr_stockOutNotice["完成"] = true;
                dr_stockOutNotice["完成日期"] = t;
                dr_stockOutNotice["计量单位"] = dr["计量单位"];
                dr_stockOutNotice["销售备注"] = "借出转赠送";


                if (dt_客户.Rows.Count > 0)
                {
                    dr_stockOutNotice["客户"] = dt_客户.Rows[0]["客户名称"];
                    dr_stockOutNotice["客户编号"] = dt_客户.Rows[0]["客户编号"];
                }
                dr_stockOutNotice["已出库数量"] = dr["归还数量"];
                dr_stockOutNotice["未出库数量"] = 0;
                //dr_出库通知单明细["n原ERP规格型号"] = dr["n原ERP规格型号"];

                DataRow dr_stockOutDetaail = dt_成品出库单明细表.NewRow();
                dt_成品出库单明细表.Rows.Add(dr_stockOutDetaail);
                dr_stockOutDetaail["GUID"] = System.Guid.NewGuid();
                dr_stockOutDetaail["成品出库单号"] = s_成品出库单号;
                dr_stockOutDetaail["POS"] = i;
                dr_stockOutDetaail["成品出库单明细号"] = s_成品出库单号 + "-" + i++.ToString("00");
                dr_stockOutDetaail["销售订单号"] = s_销售单号;
                dr_stockOutDetaail["销售订单明细号"] = dr_saleDetail["销售订单明细号"];
                dr_stockOutDetaail["出库通知单号"] = s_出库通知单号;
                dr_stockOutDetaail["出库通知单明细号"] = dr_stockOutNotice["出库通知单明细号"];
                dr_stockOutDetaail["物料编码"] = dr["物料编码"];
                dr_stockOutDetaail["物料名称"] = dr["物料名称"];
                dr_stockOutDetaail["出库数量"] = dr["归还数量"];
                dr_stockOutDetaail["已出库数量"] = dr["归还数量"];
                dr_stockOutDetaail["未开票数量"] = dr["归还数量"];
                dr_stockOutDetaail["规格型号"] = dr["规格型号"];
                dr_stockOutNotice["计量单位"] = dr["计量单位"];
                dr_stockOutNotice["销售备注"] = "借出转赠送";
                //dr_stockOutDetaail["图纸编号"] = dr["图纸编号"];
                if (dt_客户.Rows.Count > 0)
                {
                    dr_stockOutDetaail["客户"] = dt_客户.Rows[0]["客户名称"];
                    dr_stockOutDetaail["客户编号"] = dt_客户.Rows[0]["客户编号"];
                }
                dr_stockOutDetaail["仓库号"] = dr["仓库号"];
                dr_stockOutDetaail["仓库名称"] = dr["仓库名称"];
                dr_stockOutDetaail["生效"] = true;
                dr_stockOutDetaail["生效日期"] = t;
                //dr_成品出库明细["n原ERP规格型号"] = dr["n原ERP规格型号"];

                DataRow dr_stockcrmx = dt_仓库出入库明细.NewRow();
                dt_仓库出入库明细.Rows.Add(dr_stockcrmx);
                dr_stockcrmx["GUID"] = System.Guid.NewGuid();
                dr_stockcrmx["明细类型"] = "销售出库";
                dr_stockcrmx["单号"] = s_成品出库单号;
                dr_stockcrmx["物料编码"] = dr["物料编码"];
                dr_stockcrmx["物料名称"] = dr["物料名称"];
                dr_stockcrmx["明细号"] = dr_stockOutDetaail["成品出库单明细号"];
                dr_stockcrmx["出库入库"] = "出库";
                dr_stockcrmx["实效数量"] = "-" + dr["归还数量"];
                dr_stockcrmx["实效时间"] = t;
                dr_stockcrmx["出入库时间"] = t;
                dr_stockcrmx["相关单号"] = dr_saleDetail["销售订单明细号"];
                dr_stockcrmx["仓库号"] = dr["仓库号"];
                dr_stockcrmx["仓库名称"] = dr["仓库名称"];
                dr_stockcrmx["相关单位"] = dr_借还["相关单位"];
                dr_stockcrmx["单位"] = dr["计量单位"];


            }
            ds.Tables.Add(dt_销售订单明细表);
            ds.Tables.Add(dt_出库通知单明细表);
            ds.Tables.Add(dt_成品出库单明细表);
            //ds.Tables.Add(dt_仓库出入库明细);               

            return ds;
        }

        private DataSet fun_归还(string ss, DataRow dr_借还)
        {
            DataSet ds = new DataSet();

            string sql = string.Format("select * from 借还申请表附表 where 申请批号='{0}'", dr_借还["申请批号"]);
            DataTable dt_借还申请表附表 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);



            DateTime t = CPublic.Var.getDatetime();
            DataTable dt_归还表;
            //   DataTable dt_归还关联表;
            DataTable dt_仓库出入库明细;
            // DataTable dt_shen
            
            dt_主 = new DataTable();
             sql = string.Format("select * from 归还申请主表 where 归还批号='{0}'  and 归还完成='false'", dr_借还["归还批号"]);
            dt_主 = CZMaster.MasterSQL.Get_DataTable(sql,strconn);
            dt_主.Rows[0]["归还完成"] = true;


            dt_借xi = new DataTable(); 
            sql = string.Format("select * from 归还申请子表  where 归还批号='{0}' and 归还完成='false'  ", dr_借还["归还批号"]);
            dt_借xi = CZMaster.MasterSQL.Get_DataTable(sql,strconn);
            foreach (DataRow dr_t  in dt_借xi.Rows)
            {
                decimal dec = decimal.Parse(dr_t["需归还数量"].ToString()) - decimal.Parse((dr_t["申请已归还数量"].ToString()));
                dr_t["录入归还数量"] = dec;
                dr_t["申请已归还数量"] = decimal.Parse(dr_t["申请已归还数量"].ToString()) + dec;

                dr_t["归还完成"] = true;


                ////判断明细
                DataRow[] dr = dt_借还申请表附表.Select(string.Format("申请批号明细='{0}'", dr_t["申请批号明细"]));
                dr[0]["归还数量"] = decimal.Parse(dr[0]["归还数量"].ToString()) + decimal.Parse(dr_t["录入归还数量"].ToString());
                if (decimal.Parse(dr[0]["归还数量"].ToString()) == decimal.Parse(dr[0]["申请数量"].ToString()))
                {
                    dr[0]["归还完成"] = true;
                    dr[0]["归还日期"] = t;

                }
                dr[0]["正在申请数"] = 0;





            }



            string sql_归还 = "select * from 借还申请表归还记录 where 1<>1";
            dt_归还表 = CZMaster.MasterSQL.Get_DataTable(sql_归还, strconn);
            //sql_归还 = "select * from 借还申请批量归还关联 where 1<>1";
            //dt_归还关联表 = CZMaster.MasterSQL.Get_DataTable(sql_归还, strconn);

            sql_归还 = "select * from 仓库出入库明细表 where 1<>1";
            dt_仓库出入库明细 = CZMaster.MasterSQL.Get_DataTable(sql_归还, strconn);

            string s_归还单号 = string.Format("RA{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"), t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("RA", t.Year, t.Month).ToString("0000"));
            //string s_归还单号111 = string.Format("RA{0}",CPublic.CNo.fun_得到最大流水号("RA").ToString("0000"));
            int i = 1;
            foreach (DataRow dr in dt_借xi.Rows)
            {
                //dt_借还申请表附表 只显示未归还记录

                DataRow dr_归还 = dt_归还表.NewRow();
                dt_归还表.Rows.Add(dr_归还);
                dr_归还["guid"] = System.Guid.NewGuid();
                dr_归还["申请批号"] = s_归还单号;
                dr_归还["申请批号明细"] = s_归还单号 + "-" + i++.ToString("00");
                dr_归还["借用申请明细号"] = dr["申请批号明细"];
                dr_归还["计量单位"] = dr["计量单位"];
                dr_归还["计量单位编码"] = dr["计量单位编码"];

                dr_归还["物料编码"] = dr["物料编码"];
                dr_归还["物料名称"] = dr["物料名称"];
                dr_归还["规格型号"] = dr["规格型号"];
                dr_归还["仓库号"] = dr["仓库号"];
                dr_归还["仓库名称"] = dr["仓库名称"];
                dr_归还["备注"] = ss + "自动生成记录";

                decimal dec = decimal.Parse(dr["需归还数量"].ToString()) - decimal.Parse((dr["申请已归还数量"].ToString()));
               // = Convert.ToDecimal(dr["申请数量"]) - Convert.ToDecimal(dr["归还数量"]);
                dr_归还["归还数量"] = dec;
                dr_归还["归还日期"] = t;
                dr_归还["货架描述"] = dr["货架描述"];
                dr_归还["归还操作人"] = CPublic.Var.localUserName;

                dr["归还日期"] = t;
                dr["归还完成"] = 1;
                dr["借还状态"] = "已归还";

                dr["归还数量"] = dr["申请数量"];


                DataRow dr_仓库出入库明细 = dt_仓库出入库明细.NewRow();
                dt_仓库出入库明细.Rows.Add(dr_仓库出入库明细);
                dr_仓库出入库明细["GUID"] = System.Guid.NewGuid();
                dr_仓库出入库明细["明细类型"] = "归还入库";
                dr_仓库出入库明细["单号"] = s_归还单号;
                dr_仓库出入库明细["物料编码"] = dr["物料编码"];
                dr_仓库出入库明细["物料名称"] = dr["物料名称"];
                dr_仓库出入库明细["明细号"] = dr_归还["申请批号明细"];
                dr_仓库出入库明细["出库入库"] = "入库";
                dr_仓库出入库明细["实效数量"] = dec;
                dr_仓库出入库明细["实效时间"] = t;
                dr_仓库出入库明细["出入库时间"] = t;
                dr_仓库出入库明细["相关单号"] = dr_归还["借用申请明细号"];
                dr_仓库出入库明细["相关单位"] = dr_借还["相关单位"];
                dr_仓库出入库明细["仓库号"] = dr["仓库号"];
                dr_仓库出入库明细["仓库名称"] = dr["仓库名称"];
                dr_仓库出入库明细["单位"] = dr["计量单位"];
                //DataRow dr_归还关联 = dt_归还关联表.NewRow();
                //dt_归还关联表.Rows.Add(dr_归还关联);
                //dr_归还关联["关联批号"] = dr_借还["申请批号"];
                //dr_归还关联["归还批号"] = s_归还单号;
                //ds.Tables.Add(dt_归还关联表);
            }
            dr_借还["归还日期"] = t;
            dr_借还["归还"] = true;
            //dr_借还["借还状态"] = "已归还";
            dr_借还["手动归还原因"] = ss;

            ds.Tables.Add(dt_归还表);
            ds.Tables.Add(dt_仓库出入库明细);

            return ds;
        }
        private DataSet fun_save(DataRow dr_当前行)
        {




          DateTime t = CPublic.Var.getDatetime();
          //DateTime t = Convert.ToDateTime("2019-07-14 10:10:40.207");



            DataSet ds = new DataSet();
            string sql = string.Format("select * from 借还申请表附表 where 申请批号='{0}'", dr_当前行["申请批号"]);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            sql = string.Format("select * from 归还申请主表 where 归还批号='{0}'", dr_当前行["归还批号"]);
            DataTable dt_归还主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);


            sql = "select  * from 仓库出入库明细表  where 1=2";
            DataTable dt_inventoryFlow = CZMaster.MasterSQL.Get_DataTable(sql, strconn);



            DataTable dt_借出mx;//借还申请表附表
            DataTable dt_gui;///借还申请表
            string sql22 = "select * from 借还申请表附表 where 1<>1";
            dt_借出mx = new DataTable();
            dt_借出mx = CZMaster.MasterSQL.Get_DataTable(sql22, strconn);
          


            ///出入库记录单号
            string str_returnRecord = string.Format("RA{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
             t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("RA", t.Year, t.Month).ToString("0000"));
            int ki = 1;
            int str_选择=0;
            foreach (DataRow dr_t in dt_借xi.Rows )
            {
                if (bool.Parse(dr_t["选择"].ToString() )==true)
                {
                    str_选择++;
                }
                else
                {
                    continue;
                }
                dr_t["申请已归还数量"] = decimal.Parse(dr_t["申请已归还数量"].ToString()) + decimal.Parse(dr_t["录入归还数量"].ToString());
                
                dr_t["已归还数量"] = decimal.Parse(dr_t["已归还数量"].ToString())+ decimal.Parse(dr_t["录入归还数量"].ToString());
                if(decimal.Parse(dr_t["申请已归还数量"].ToString())> decimal.Parse(dr_t["需归还数量"].ToString()))
                {
                    throw new Exception("归还数量超出");
                }
                if (decimal.Parse(dr_t["申请已归还数量"].ToString())== decimal.Parse(dr_t["需归还数量"].ToString()))
                {
                    dr_t["归还完成"] = true;
                    dr_t["归还日期"] = t;
                }

                //////
                ///判断主表明星完成

                DataRow[] dr = dt.Select(string.Format("申请批号明细='{0}'",dr_t["申请批号明细"]));
                dr[0]["归还数量"]= decimal.Parse(dr[0]["归还数量"].ToString()) + decimal.Parse(dr_t["录入归还数量"].ToString());
                if (decimal.Parse(dr[0]["归还数量"].ToString())== decimal.Parse(dr[0]["申请数量"].ToString()))
                {
                    dr[0]["归还完成"] = true;
                    dr[0]["归还日期"] = t;

                }
                dr[0]["正在申请数"] =0;
                //DataRow dr_jc = dt_借出mx.NewRow();
                //dr_jc = dr;
                //dt_借出mx.Rows.Add(dr_jc);
                //   dt_借出mx.ImportRow(dr);



                ///////出入库记录

                DataRow r_inventoryFlow = dt_inventoryFlow.NewRow();
                r_inventoryFlow["GUID"] = System.Guid.NewGuid();
                r_inventoryFlow["明细类型"] = "归还入库";
                r_inventoryFlow["单号"] = str_returnRecord;
                r_inventoryFlow["物料名称"] = dr_t["物料名称"];
                r_inventoryFlow["物料编码"] = dr_t["物料编码"];

                string x = CPublic.Var.localUser课室编号;
                if (x == "") x = CPublic.Var.localUser部门编号;
                // x = string.Format("select  * from 人事基础部门表 where 部门编号='{0}'", x);
                //r_inventoryFlow["相关单位"] = dr_t["相关单位"];
                //DataRow r_depart = CZMaster.MasterSQL.Get_DataRow(x,strconn);
                //r_inventoryFlow["相关单位"] = r_depart["部门名称"];
                r_inventoryFlow["明细号"] = str_returnRecord + "-" + ki.ToString("00");
                r_inventoryFlow["出库入库"] = "入库";
                r_inventoryFlow["仓库号"] = dr_t["仓库号"];
                r_inventoryFlow["仓库名称"] = dr_t["仓库名称"];
                r_inventoryFlow["单位"] = dr_t["计量单位"];
                r_inventoryFlow["数量"] = dr_t["录入归还数量"];
                r_inventoryFlow["实效数量"] = dr_t["录入归还数量"];
                r_inventoryFlow["实效时间"] = r_inventoryFlow["出入库时间"] = t;
                r_inventoryFlow["相关单号"] = dr_t["归还明细号"];
                r_inventoryFlow["仓库人"] = CPublic.Var.localUserName;

                dt_inventoryFlow.Rows.Add(r_inventoryFlow);
                ki++;

            }

            if (dt_借xi.Rows.Count == str_选择)
            {
                int hhh = 0;
                foreach (DataRow dr_t in dt_借xi.Rows)
                {
                    if (bool .Parse(dr_t["归还完成"] .ToString())== true)
                    {
                        hhh++;
                    }
                }
                if (hhh== dt_借xi.Rows.Count)
                {

                    dt_归还主.Rows[0]["归还完成"] = true;
                    dt_归还主.Rows[0]["归还日期"] = t;
                }
             }
            int i = 0;
            foreach (DataRow drr in dt.Rows)
            {
                if (bool.Parse(drr["归还完成"].ToString()) == true)
                {
                    i++;
                }
            }
            string sql_主 = string.Format("select * from 借还申请表 where 申请批号='{0}'", dr_当前行["申请批号"]);
            dt_gui = new DataTable();
            dt_gui = CZMaster.MasterSQL.Get_DataTable(sql_主,strconn);
            if(i== dt.Rows.Count)
            {
                dt_gui.Rows[0]["归还"] = true;
                dt_gui.Rows[0]["归还日期"] = t;
            }
            DataView dv = new DataView(dt_借xi);
            dv.RowFilter = "选择=1";
            DataTable tt = dv.ToTable();
            if (tt.Rows.Count <= 0)
            {
                throw new Exception("当前无选择归还明细");
            }
            tt.Columns["录入归还数量"].ColumnName = "数量";
            DataTable dt_库存 = ERPorg.Corg.fun_库存(1, tt);
            //还需要一个借还申请表的 状态
            //tt.Columns["数量"].ColumnName = "录入归还数量";
            ds.Tables.Add(dt_借xi);//归还申请明细
            ds.Tables.Add(dt_归还主);///归还申请主表
            ds.Tables.Add(dt);///借出明细表   借还申请表附表 
            ds.Tables.Add(dt_gui);///借出主表   借还申请表    
            ds.Tables.Add(dt_库存); //仓库物料数量表            
            ds.Tables.Add(dt_inventoryFlow);// 仓库出入库记录

            return ds;



        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ui归还_Load(null,null);
        }

        private void gv3_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }

        }

        private void gv2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {

            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void gv2_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
                if (e.Column.FieldName == "仓库号")
                {
                    dr["仓库号"] = e.Value;
                    DataRow[] ds = dt_仓库号.Select(string.Format("仓库号 = {0}", dr["仓库号"]));
                    dr["仓库名称"] = ds[0]["仓库名称"];
                    string sql = "select * from 仓库物料数量表 where 物料编码 = '" + dr["物料编码"] + "' and 仓库号 = '" + dr["仓库号"] + "'";
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_物料数量 = new DataTable();
                    da.Fill(dt_物料数量);
                    //if (dt_物料数量.Rows[0]["货架描述"].ToString()!="")
                    //{
                    //    dr["货架描述"] = dt_物料数量.Rows[0]["货架描述"];
                    //}

                    if (dt_物料数量.Rows.Count == 0)
                    {
                        //dr["库存总数"] = 0;
                        dr["货架描述"] = "";
                    }
                    else
                    {
                       // dr["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
                        dr["货架描述"] = dt_物料数量.Rows[0]["货架描述"];
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 赠送ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dr_借还 = gv3.GetDataRow(gv3.FocusedRowHandle);
                //返回ds.tables[0]归还记录明细，ds.tables[1]归还关联,ds.tables[2]
                DataSet ds_借还 = fun_归还("借用转赠送", dr_借还);
                //保存ds_借还,dt_借用申请表，dt_借用申请表附表
                //返回ds.tables[0]dt_销售订单主表dt_，ds.tables[1]出库通知单主表,ds.tables[2]dt_成品出库单主表,ds.tables[3]dt_销售订单明细表，
                //ds.tables[4]dt_出库通知单明细表,ds.tables[5]dt_成品出库单明细表,ds.tables[6]dt_仓库出入库明细
                DataSet ds_zs = fun_赠送(ds_借还.Tables[0], ds_借还.Tables[1]);
                //保存ds_zs
                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction thrk = conn.BeginTransaction("归还转赠送");
                try
                {
                    string sql1 = "select * from 借还申请表 where 1<>1";
                    SqlCommand cmd1 = new SqlCommand(sql1, conn, thrk);
                    SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(dt_主);

                    sql1 = "select * from 借还申请表附表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(dt_借xi);

                    sql1 = "select * from 借还申请表归还记录 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(ds_借还.Tables[0]);

                    //sql1 = "select * from 借还申请批量归还关联 where 1<>1";
                    //cmd1 = new SqlCommand(sql1, conn, thrk);
                    //da1 = new SqlDataAdapter(cmd1);
                    //new SqlCommandBuilder(da1);
                    //da1.Update(ds_借还.Tables[2]);

                    sql1 = "select * from 销售记录销售订单主表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(ds_zs.Tables[0]);

                    sql1 = "select * from 销售记录销售出库通知单主表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(ds_zs.Tables[1]);

                    sql1 = "select * from 销售记录成品出库单主表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(ds_zs.Tables[2]);

                    sql1 = "select * from 销售记录销售订单明细表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(ds_zs.Tables[3]);
                    sql1 = "select * from 销售记录销售出库通知单明细表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(ds_zs.Tables[4]);

                    sql1 = "select * from 销售记录成品出库单明细表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(ds_zs.Tables[5]);

                    sql1 = "select * from 仓库出入库明细表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(ds_借还.Tables[1]);

                    thrk.Commit();
                    MessageBox.Show("归还转赠送成功");

                    barLargeButtonItem3_ItemClick(null, null);
                }
                catch (Exception ex)
                {
                    thrk.Rollback();
                    throw ex;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }

        private void gv3_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {

        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                    gc3.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);

            }
        }
    }
}
