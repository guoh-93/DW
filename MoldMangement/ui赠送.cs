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
using CZMaster;

namespace MoldMangement
{
    public partial class ui赠送 : UserControl
    {
        public ui赠送()
        {
            InitializeComponent();
        }

        DataRow dr_借还;
        DataTable dt_借还;
        DataTable dt_借xi, dt_主;
        DataTable dt_可还明细;
        DataSet ds_借还 = null;
        DataSet ds_zs=null;
        DataTable dt_客户信息;
        DataTable dt_归还申请主;
        DataTable dt_归还申请子;
        DataRow dr_申请主;

        string strconn = CPublic.Var.strConn;

        public ui赠送(DataRow dr )
        {
            dr_借还 = dr;
            
            //dt_借xi = dt;
            
            InitializeComponent();
        }

        private void ui赠送_Load(object sender, EventArgs e)
        {

            try

            {
                string sql_归还申请主 = "select * from 归还申请主表 where 1<>1";
                dt_归还申请主 = CZMaster.MasterSQL.Get_DataTable(sql_归还申请主, strconn);
                string sql_归还申请子 = "select * from 归还申请子表 where 1<>1";
                dt_归还申请子 = CZMaster.MasterSQL.Get_DataTable(sql_归还申请子, strconn);


                string sql = "select 客户编号,客户名称  from 客户基础信息表 where 停用=0  ";
                dt_客户信息 = CZMaster.MasterSQL.Get_DataTable(sql,strconn);
            
                searchLookUpEdit1.Properties.DataSource = dt_客户信息;
                searchLookUpEdit1.Properties.ValueMember = "客户名称";
                searchLookUpEdit1.Properties.DisplayMember = "客户名称";


                dataBindHelper1.DataFormDR(dr_借还);

                sql = string.Format("select * from   借还申请表  where 申请批号='{0}' ", dr_借还["申请批号"]);
                dt_借还 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                //dt_借还.Columns.Add("文件GUID",typeof(string));
                //dt_借还.Columns.Add("附件",typeof(bool));
                //dt_借还.Columns.Add("文件",typeof(string));
                //dt_借还.Columns.Add("上传时间",typeof(DateTime));
                //dt_借还.Columns.Add("后缀",typeof(string));


                sql = string.Format("select * from   借还申请表附表  where 申请批号='{0}' and 归还完成=0 ", dr_借还["申请批号"]);
                dt_借xi = new DataTable();
                dt_借xi = CZMaster.MasterSQL.Get_DataTable(sql,strconn);
                //DataColumn dc = new DataColumn("选择", typeof(bool));
                //dc.DefaultValue = false;
                //dt_借xi.Columns.Add(dc);
               // dt_借xi.Columns.Add("选择", typeof(bool));
                dt_借xi.Columns.Add("请输入赠送数量", typeof(decimal));
                dt_可还明细 = dt_借xi.Clone();
                foreach (DataRow dr in dt_借xi.Rows)
                {
                    dr["请输入赠送数量"]= decimal.Parse(dr["已借出数量"].ToString()) - decimal.Parse(dr["归还数量"].ToString()) - decimal.Parse(dr["正在申请数"].ToString());
                    if(Convert.ToDecimal(dr["请输入赠送数量"]) > 0)
                    {
                        DataRow drmx = dt_可还明细.NewRow();
                        dt_可还明细.Rows.Add(drmx);
                    
                        drmx["申请批号"] = dr["申请批号"];
                        drmx["申请批号明细"] = dr["申请批号明细"];
                        drmx["物料编码"] = dr["物料编码"];
                        drmx["物料名称"] = dr["物料名称"];
                        drmx["规格型号"] = dr["规格型号"];
                        //drmx["申请日期"] = Convert.ToDateTime(dr["申请日期"]);
                        drmx["归还完成"] = dr["归还完成"];
                        //drmx["归还日期"] = Convert.ToDateTime(dr["归还日期"]);
                        drmx["申请数量"] = Convert.ToDecimal(dr["申请数量"]);
                        drmx["计量单位编码"] = dr["计量单位编码"];
                        drmx["计量单位"] = dr["计量单位"];
                        drmx["归还数量"] = Convert.ToDecimal(dr["归还数量"]);
                        drmx["备注"] = dr["备注"];
                        drmx["货架描述"] = dr["货架描述"];
                        drmx["仓库号"] = dr["仓库号"];
                        drmx["仓库名称"] = dr["仓库名称"];
                        drmx["领取完成"] = dr["领取完成"];
                        drmx["已借出数量"] = Convert.ToDecimal(dr["已借出数量"]);
                        drmx["借还状态"] = dr["借还状态"];
                        drmx["请输入赠送数量"] = Convert.ToDecimal(dr["请输入赠送数量"]); 
                        drmx["正在申请数"] = Convert.ToDecimal(dr["正在申请数"]);
                    }
                }
                DataColumn dc = new DataColumn("选择", typeof(bool));
                dc.DefaultValue = false;
                dt_可还明细.Columns.Add(dc);
                gcP.DataSource = dt_可还明细;




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //try
            //{
            //    gvP.CloseEditor();
            //    this.BindingContext[dt_借xi].EndCurrentEdit();


            //    string sqlw = string.Format("select * from 借还申请表 where 申请批号='{0}'  ",dr_借还["申请批号"]);
            //    dt_主 = new DataTable();
            //    dt_主 = CZMaster.MasterSQL.Get_DataTable(sqlw,strconn);
            //    // 转赠送带过来有可能是 部门 必须要选择 客户
            //    if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.Text.ToString() == "")
            //        throw new Exception("客户为必填项");
            //    dr_借还["相关单位"] = searchLookUpEdit1.Text.ToString().Trim();


            //    DateTime t = CPublic.Var.getDatetime();

            //    int j = 0;//记录完成次数
            //    foreach (DataRow dr_t in dt_借xi.Rows)
            //    {
            //        if (bool.Parse(dr_t["选择"].ToString()) == true)
            //        {
            //            dr_t["归还数量"] = decimal.Parse(dr_t["归还数量"].ToString()) + decimal.Parse(dr_t["请输入赠送数量"].ToString());
            //            if (decimal.Parse(dr_t["归还数量"].ToString()) == decimal.Parse(dr_t["申请数量"].ToString()))
            //            {
            //                dr_t["归还完成"] = true;
            //                dr_t["归还日期"] = t;
            //            }
            //            j++;
            //        }
            //    }
            //    int a = 0;
            //    if (j == dt_借xi.Rows.Count)
            //    {

            //        foreach (DataRow dj in dt_借xi.Rows)
            //        {
            //            if (bool.Parse(dj["归还完成"].ToString()) == true)
            //            {
            //                a++;
            //            }

            //        }
            //    }
            //    if (a == dt_借xi.Rows.Count)
            //    {
            //        dt_主.Rows[0]["归还"] = true;
            //        dt_主.Rows[0]["归还日期"] = t;


            //        dt_主.Rows[0]["手动归还原因"] = "有赠送";

            //    }





            //    DataSet ds_借还 = new DataSet();
            //    DataSet ds_zs = new DataSet();
            //       //dr_借还 = gvM.GetDataRow(gvM.FocusedRowHandle);
            //       //返回ds.tables[0]归还记录明细，ds.tables[1]归还关联,ds.tables[2]
            //     ds_借还 = fun_归还("借用转赠送", dr_借还, dt_借xi);
            //    //保存ds_借还,dt_借用申请表，dt_借用申请表附表
            //    //返回ds.tables[0]dt_销售订单主表dt_，ds.tables[1]出库通知单主表,ds.tables[2]dt_成品出库单主表,ds.tables[3]dt_销售订单明细表，
            //    //ds.tables[4]dt_出库通知单明细表,ds.tables[5]dt_成品出库单明细表,ds.tables[6]dt_仓库出入库明细
            //     ds_zs = fun_赠送(ds_借还.Tables[1], ds_借还.Tables[0]);
            //    //保存ds_zs
            //    SqlConnection conn = new SqlConnection(strconn);
            //    conn.Open();
            //    SqlTransaction thrk = conn.BeginTransaction("归还转赠送");
            //    try
            //    {
            //        string sql1 = "select * from 借还申请表 where 1<>1";
            //        SqlCommand cmd1 = new SqlCommand(sql1, conn, thrk);
            //        SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
            //        new SqlCommandBuilder(da1);
            //        da1.Update(dt_主);

            //        sql1 = "select * from 借还申请表附表 where 1<>1";
            //        cmd1 = new SqlCommand(sql1, conn, thrk);
            //        da1 = new SqlDataAdapter(cmd1);
            //        new SqlCommandBuilder(da1);
            //        da1.Update(dt_借xi);

            //        sql1 = "select * from 借还申请表归还记录 where 1<>1";
            //        cmd1 = new SqlCommand(sql1, conn, thrk);
            //        da1 = new SqlDataAdapter(cmd1);
            //        new SqlCommandBuilder(da1);
            //        da1.Update(ds_借还.Tables[1]);


            //        sql1 = "select * from 销售记录销售订单主表 where 1<>1";
            //        cmd1 = new SqlCommand(sql1, conn, thrk);
            //        da1 = new SqlDataAdapter(cmd1);
            //        new SqlCommandBuilder(da1);
            //        da1.Update(ds_zs.Tables[0]);

            //        sql1 = "select * from 销售记录销售出库通知单主表 where 1<>1";
            //        cmd1 = new SqlCommand(sql1, conn, thrk);
            //        da1 = new SqlDataAdapter(cmd1);
            //        new SqlCommandBuilder(da1);
            //        da1.Update(ds_zs.Tables[1]);

            //        sql1 = "select * from 销售记录成品出库单主表 where 1<>1";
            //        cmd1 = new SqlCommand(sql1, conn, thrk);
            //        da1 = new SqlDataAdapter(cmd1);
            //        new SqlCommandBuilder(da1);
            //        da1.Update(ds_zs.Tables[2]);

            //        sql1 = "select * from 销售记录销售订单明细表 where 1<>1";
            //        cmd1 = new SqlCommand(sql1, conn, thrk);
            //        da1 = new SqlDataAdapter(cmd1);
            //        new SqlCommandBuilder(da1);
            //        da1.Update(ds_zs.Tables[3]);
            //        sql1 = "select * from 销售记录销售出库通知单明细表 where 1<>1";
            //        cmd1 = new SqlCommand(sql1, conn, thrk);
            //        da1 = new SqlDataAdapter(cmd1);
            //        new SqlCommandBuilder(da1);
            //        da1.Update(ds_zs.Tables[4]);

            //        sql1 = "select * from 销售记录成品出库单明细表 where 1<>1";
            //        cmd1 = new SqlCommand(sql1, conn, thrk);
            //        da1 = new SqlDataAdapter(cmd1);
            //        new SqlCommandBuilder(da1);
            //        da1.Update(ds_zs.Tables[5]);

            //        sql1 = "select * from 仓库出入库明细表 where 1<>1";
            //        cmd1 = new SqlCommand(sql1, conn, thrk);
            //        da1 = new SqlDataAdapter(cmd1);
            //        new SqlCommandBuilder(da1);
            //        da1.Update(ds_借还.Tables[0]);

            //       thrk.Commit();
            //        MessageBox.Show("归还转赠送成功");
            //        CPublic.UIcontrol.ClosePage();

            //    }
            //    catch (Exception ex)
            //    {
            //        thrk.Rollback();
            //        throw ex;
            //    }


            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            try
            {
                gvP.CloseEditor();
                this.BindingContext[dt_可还明细].EndCurrentEdit();
                
                fun_check();
                DateTime t = CPublic.Var.getDatetime();
                string sqlw = string.Format("select * from 借还申请表 where 申请批号='{0}'  ", dr_借还["申请批号"]);
                dt_主 = new DataTable();
                dt_主 = CZMaster.MasterSQL.Get_DataTable(sqlw, strconn);
                // 转赠送带过来有可能是 部门 必须要选择 客户
               
                dr_借还["相关单位"] = searchLookUpEdit1.Text.ToString().Trim();
                

                string str_归还单号 = string.Format("GH{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                    CPublic.CNo.fun_得到最大流水号("GH", t.Year, t.Month));
                DataRow[] dr_客户 = dt_客户信息.Select(string.Format("客户名称 = '{0}'",searchLookUpEdit1.EditValue));
                if (dt_归还申请主.Rows.Count == 0)
                {
                    dr_申请主 = dt_归还申请主.NewRow();
                    dt_归还申请主.Rows.Add(dr_申请主);
                }
                else
                {
                    dr_申请主 = dt_归还申请主.Rows[0];
                }
                dr_申请主["归还批号"] = str_归还单号;
                dr_申请主["申请批号"] = textBox1.Text;
                dr_申请主["归还操作人"] = textBox2.Text;
                dr_申请主["备注"] = textBox6.Text;
                dr_申请主["归还申请日期"] = t;
                dr_申请主["归还完成"] = false;
                dr_申请主["原因分类"] = textBox4.Text;
                dr_申请主["借用类型"] = textBox3.Text;
                dr_申请主["归还说明"] = textBox10.Text;
                dr_申请主["客户名称"] = searchLookUpEdit1.EditValue;
                dr_申请主["目标客户"] = textBox7.Text;
                dr_申请主["归还方式"] = "借用转客户试用";
                //dr_申请主["文件GUID"] = dt_借还.Rows[0]["文件GUID"];
                //dr_申请主["附件"] =Convert.ToBoolean( dt_借还.Rows[0]["附件"]);
                //dr_申请主["文件"] = dt_借还.Rows[0]["文件"];
                //dr_申请主["上传时间"] = dt_借还.Rows[0]["上传时间"];
                //dr_申请主["后缀"] = dt_借还.Rows[0]["后缀"];
                if (dr_客户.Length > 0)
                {
                    dr_申请主["客户编号"] = dr_客户[0]["客户编号"];
                }
               
                dr_申请主["锁定"] = true;


                int i = 1;
                foreach (DataRow dr in dt_可还明细.Rows)
                {
                    if (!Convert.ToBoolean(dr["选择"])) continue;
                    DataRow dr_归还申请子 = dt_归还申请子.NewRow();
                    dt_归还申请子.Rows.Add(dr_归还申请子);
                    dr_归还申请子["归还批号"] = str_归还单号;
                    dr_归还申请子["POS"] = i;
                    dr_归还申请子["归还明细号"] = str_归还单号 + "-" + Convert.ToInt32(dr_归还申请子["POS"]).ToString("00");
                    dr_归还申请子["申请批号"] = dr["申请批号"];
                    dr_归还申请子["申请批号明细"] = dr["申请批号明细"];
                    dr_归还申请子["物料编码"] = dr["物料编码"];
                    dr_归还申请子["物料名称"] = dr["物料名称"];
                    dr_归还申请子["规格型号"] = dr["规格型号"];
                    dr_归还申请子["备注"] = dr["备注"];
                    dr_归还申请子["货架描述"] = dr["货架描述"];
                    dr_归还申请子["仓库名称"] = dr["仓库名称"];
                    dr_归还申请子["仓库号"] = dr["仓库号"];
                    dr_归还申请子["需归还数量"] = Convert.ToDecimal(dr["请输入赠送数量"]);
                    dr_归还申请子["计量单位"] = dr["计量单位"];
                    dr_归还申请子["计量单位编码"] = dr["计量单位编码"];
                    dr_归还申请子["借用数量"] = Convert.ToDecimal(dr["申请数量"]);
                    dr_归还申请子["归还完成"] = false;
                    DataRow[] dr_借还 = dt_借xi.Select(string.Format("申请批号明细 = '{0}'",dr["申请批号明细"]));
                    dr_借还[0]["正在申请数"] = Convert.ToDecimal(dr["正在申请数"]) + Convert.ToDecimal(dr["请输入赠送数量"]);
                    i++;
                }
                DataTable dt_审核 = ERPorg.Corg.fun_PA("生效", "借用转客户试用申请单", str_归还单号, searchLookUpEdit1.EditValue.ToString());

                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction thrk = conn.BeginTransaction("借用转客户试用");
                try
                {
                    string sql1 = "select * from 归还申请主表 where 1<>1";
                    SqlCommand cmd1 = new SqlCommand(sql1, conn, thrk);
                    SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(dt_归还申请主);

                    sql1 = "select * from 归还申请子表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(dt_归还申请子);

                    sql1 = "select * from 借还申请表附表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(dt_借xi);

                    sql1 = "select * from 单据审核申请表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(dt_审核);

                    thrk.Commit();
                    MessageBox.Show("借用转客户试用申请成功");
                    CPublic.UIcontrol.ClosePage();

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

        private void fun_check()
        {
            if (checkBox1.Checked == false)
            {
                throw new Exception("附件未上传");
            }
            if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.Text.ToString() == "")
                throw new Exception("客户为必填项");
            DataView dv = new DataView(dt_可还明细);
            dv.RowFilter = "选择 = true";
            if (dv.Count == 0)
            {
                throw new Exception("未勾选明细，请确认");
            }
            foreach (DataRow dr in dt_可还明细.Rows)
            {
                if (!Convert.ToBoolean(dr["选择"])) continue;
                if ((Convert.ToDecimal(dr["已借出数量"]) - Convert.ToDecimal(dr["正在申请数"]) - Convert.ToDecimal(dr["归还数量"])) > 0)
                {
                    if (Convert.ToDecimal(dr["请输入赠送数量"]) <= 0)
                    {
                        throw new Exception("转赠送数量不可小于等于零");
                    }
                    if (Convert.ToDecimal(dr["请输入赠送数量"]) > (Convert.ToDecimal(dr["已借出数量"]) - Convert.ToDecimal(dr["归还数量"]) - Convert.ToDecimal(dr["正在申请数"])))
                    {
                        throw new Exception("转赠送数量超出借出数量");
                    }
                }

            }
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
            s = "select * from 仓库出入库明细表 where 1<>1";

           // dt_仓库出入库明细 = CZMaster.MasterSQL.Get_DataTable(s, strconn);

            // DateTime t = CPublic.Var.getDatetime();
            string s_销售单号 = string.Format("SO{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SO", t.Year, t.Month, t.Day).ToString("0000"));
            string s_出库通知单号 = string.Format("SK{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SK", t.Year, t.Month).ToString("0000"));
            string s_成品出库单号 = string.Format("SA{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SA", t.Year, t.Month).ToString("0000"));

            // = string.Format("SO{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
            //t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SO", t.Year, t.Month).ToString("0000"));


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
            dr_销售订单主["销售备注"] = "借出转赠送"+":"+ textBox10.Text;

            dr_销售订单主["部门编号"] = CPublic.Var.localUser部门编号;

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
                dr_stockOutNotice["销售备注"] = "借出转赠送" + ":" + textBox10.Text;


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
         //  ds.Tables.Add(dt_仓库出入库明细);               

            return ds;
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }



        string strcon_FS = CPublic.Var.geConn("FS");
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                //if (dr_借还 == null)
                //{
                //    throw new Exception("请先新增采购订单！");
                //}
                
                if (dt_归还申请主.Rows.Count == 0)
                {
                    dr_申请主 = dt_归还申请主.NewRow();
                    dt_归还申请主.Rows.Add(dr_申请主);
                }
                else
                {
                    dr_申请主 = dt_归还申请主.Rows[0];
                }



                OpenFileDialog open = new OpenFileDialog();
                if (open.ShowDialog() == DialogResult.OK)
                {
                    FileInfo info = new FileInfo(open.FileName);      //判定上传文件的大小
                    //long maxlength = info.Length;
                    //if (maxlength > 1024 * 1024 * 8)
                    //{
                    //    throw new Exception("上传的文件不能超过1M，请重新选择后再上传！");//drM
                    MasterFileService.strWSDL = CPublic.Var.strWSConn;
                    CFileTransmission.CFileClient.strCONN = strcon_FS;

                    string type = "";
                    //type = pathName.Substring(pathName.LastIndexOf("."), pathName.Length - pathName.LastIndexOf(".")).Replace(".", "");
                    int s = Path.GetFileName(open.FileName).LastIndexOf(".") + 1;
                    type = Path.GetFileName(open.FileName).Substring(s, Path.GetFileName(open.FileName).Length - s);

                    string strguid = "";  //记录系统自动返回的GUID
                    strguid = CFileTransmission.CFileClient.sendFile(open.FileName);
                    dr_申请主["文件GUID"] = strguid;
                    dr_申请主["附件"] = true;
                    dr_申请主["文件"] = Path.GetFileName(open.FileName);
                    dr_申请主["上传时间"] = CPublic.Var.getDatetime();
                    dr_申请主["后缀"] = type;
                    MessageBox.Show("上传成功！");
                    checkBox1.Checked = true;
                    button2.Enabled = true;
                    button5.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
   

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dt_归还申请主.Rows.Count == 0)
                {
                    throw new Exception("没有文件可以下载，请先上传文件");
                }
                else
                {
                    dr_申请主 = dt_归还申请主.Rows[0];
                }

                //if (dr_主 == null)
                //{
                //    throw new Exception("请重新选择采购订单！");
                //}
                if (dr_申请主["文件GUID"] == null || dr_申请主["文件GUID"].ToString() == "")
                {
                    throw new Exception("没有文件可以下载，请先上传文件");
                }

                SaveFileDialog save = new SaveFileDialog();
                // save.Filter = "(*.jpg,*.png,*.jpeg,*.bmp,*.gif)|*.jgp;*.png;*.jpeg;*.bmp;*.gif|All files(*.*)|*.*";
                save.FileName = dr_申请主["文件"].ToString() + "." + dr_申请主["后缀"].ToString();
                //save.FileName = drm["文件名"].ToString();

                if (save.ShowDialog() == DialogResult.OK)
                {
                    CFileTransmission.CFileClient.strCONN = strcon_FS;
                    CFileTransmission.CFileClient.Receiver(dr_申请主["文件GUID"].ToString(), save.FileName);
                    MessageBox.Show("文件下载成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (dt_归还申请主.Rows.Count == 0)
                {
                    throw new Exception("没有文件可以预览，请先上传文件");
                }
                else
                {
                    dr_申请主 = dt_归还申请主.Rows[0];
                }
                if (dr_申请主["文件GUID"] == null || dr_申请主["文件GUID"].ToString() == "")
                {
                    throw new Exception("没有文件可以预览，请先上传文件");
                }
                //string type = dr["后缀"].ToString();

                string foldPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\查看文件\\";
                //string fileName = foldPath + CPublic.Var.getDatetime().ToString("yyyy-MM-dd") + "T" + CPublic.Var.getDatetime().ToString("HH_mm_ss") + "Z" + "_" + Guid.NewGuid().ToString() + "." + type;
                string fileName = foldPath + dr_申请主["文件"].ToString();

                try
                {
                    System.IO.Directory.Delete(foldPath, true);
                }
                catch (Exception)
                {
                }
                CFileTransmission.CFileClient.strCONN = strcon_FS;
                CFileTransmission.CFileClient.Receiver(dr_申请主["文件GUID"].ToString(), fileName);
                System.Diagnostics.Process.Start(fileName);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private DataSet fun_归还(string ss, DataRow dr_借还 ,DataTable dt_借xi)
        {
            DataSet ds = new DataSet();

            //string sql = string.Format("select * from 借还申请表附表 where 申请批号='{0}'", dr_借还["申请批号"]);
            //DataTable dt_借还申请表附表 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);



            DateTime t = CPublic.Var.getDatetime();
            DataTable dt_归还表;
            //   DataTable dt_归还关联表;
            DataTable dt_仓库出入库明细;
            // DataTable dt_shen

            //dt_主 = new DataTable();
            //sql = string.Format("select * from 归还申请主表 where 归还批号='{0}'  and 归还完成='false'", dr_借还["归还批号"]);
            //dt_主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            //dt_主.Rows[0]["归还完成"] = true;


            //dt_借xi = new DataTable();
            //sql = string.Format("select * from 归还申请子表  where 归还批号='{0}' and 归还完成='false'  ", dr_借还["归还批号"]);
            //dt_借xi = CZMaster.MasterSQL.Get_DataTable(sql, strconn);


            string sql_归还 = "select * from 借还申请表归还记录 where 1<>1";
            dt_归还表 = CZMaster.MasterSQL.Get_DataTable(sql_归还, strconn);
      

            //sql_归还 = "select * from 借还申请批量归还关联 where 1<>1";
            //dt_归还关联表 = CZMaster.MasterSQL.Get_DataTable(sql_归还, strconn);

            sql_归还 = "select * from 仓库出入库明细表 where 1<>1";
            dt_仓库出入库明细 = CZMaster.MasterSQL.Get_DataTable(sql_归还, strconn);

            string s_归还单号 = string.Format("RA{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"), t.Day.ToString("00")
                , CPublic.CNo.fun_得到最大流水号("RA", t.Year, t.Month).ToString("0000"));
            //string s_归还单号111 = string.Format("RA{0}",CPublic.CNo.fun_得到最大流水号("RA").ToString("0000"));
            int i = 1;
            foreach (DataRow dr in dt_借xi.Rows)
            {
                //dt_借还申请表附表 只显示未归还记录
                if (!Convert.ToBoolean(dr["选择"]))  continue;   //2019-10-12  发现 不管选不选勾  都所有记录归还   后  增加

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
                dr_归还["备注"] = ss + "自动生成记录"+ dr_借还["申请批号"];

                decimal dec = decimal.Parse(dr["请输入赠送数量"].ToString());
                // = Convert.ToDecimal(dr["申请数量"]) - Convert.ToDecimal(dr["归还数量"]);
                dr_归还["归还数量"] = dec;
                dr_归还["归还日期"] = t;
                dr_归还["货架描述"] = dr["货架描述"];
                dr_归还["归还操作人"] = CPublic.Var.localUserName;

                //dr["归还日期"] = t;
                //dr["归还完成"] = 1;
                //dr["借还状态"] = "已归还";

                //dr["归还数量"] = dr["申请数量"];

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
        
            //ds.Tables.Add(dt_主);//借还申请表
            //ds.Tables.Add(dt_借xi); //借还申请表附表
            ds.Tables.Add(dt_仓库出入库明细);// 仓库出入库明细表
            ds.Tables.Add(dt_归还表);//借还申请表归还记录
            return ds;
        }

    }
}
