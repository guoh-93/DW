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
    public partial class ui归还转耗用 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        DataRow dr_借还;
        DataTable dt_归还清单;
        DataTable dt_归还申请主;
        DataTable dt_归还申请子;
        DataTable dt_可还明细;
        DataRow dr_申请主;
        DataTable   dt_主;
        string cfgfilepath = "";
        bool s_跳转 = false;
       
        
        public DataSet ds_耗用;


        public bool bl_转耗用 = false;

        public ui归还转耗用()
        {
            InitializeComponent();
        }

        public ui归还转耗用(DataRow dr, DataTable dt)
        {
            InitializeComponent();
            dr_借还 = dr;
            dt_归还清单 = dt;
        }
        


        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ParentForm.Close();
        }

        private void ui归还转耗用_Load(object sender, EventArgs e)
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

                if (s_跳转)
                {

                }
                string sql_归还申请主 = "select * from 归还申请主表 where 1<>1";
                dt_归还申请主 = CZMaster.MasterSQL.Get_DataTable(sql_归还申请主, strconn);
                string sql_归还申请子 = "select * from 归还申请子表 where 1<>1";
                dt_归还申请子 = CZMaster.MasterSQL.Get_DataTable(sql_归还申请子, strconn);
                x.UserLayout(this.panel4, this.Name, cfgfilepath);
                dataBindHelper1.DataFormDR(dr_借还);
                textBox8.Text = CPublic.Var.localUser部门名称;
                string sql = string.Format("select * from   借还申请表附表  where 申请批号='{0}' and 归还完成=0 ", dr_借还["申请批号"]);
                dt_归还清单 = new DataTable();
                dt_归还清单 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                //DataColumn dc = new DataColumn("选择", typeof(bool));
                //dc.DefaultValue = false;
                //dt_归还清单.Columns.Add(dc);
                // dt_借xi.Columns.Add("选择", typeof(bool));
                dt_归还清单.Columns.Add("最大归还数", typeof(decimal));
                dt_可还明细 = dt_归还清单.Clone();
                foreach (DataRow dr in dt_归还清单.Rows)
                {
                    dr["最大归还数"] = decimal.Parse(dr["已借出数量"].ToString()) - decimal.Parse(dr["归还数量"].ToString()) - decimal.Parse(dr["正在申请数"].ToString());
                    if (Convert.ToDecimal(dr["最大归还数"]) > 0)
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
                        drmx["最大归还数"] = Convert.ToDecimal(dr["最大归还数"]);
                        drmx["正在申请数"] = Convert.ToDecimal(dr["正在申请数"]);
                    }
                }
                DataColumn dc = new DataColumn("选择", typeof(bool));
                dc.DefaultValue = false;
                dt_可还明细.Columns.Add(dc);
                gcP.DataSource = dt_可还明细;
                fun_下拉();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);                
            }
        }

        private void fun_下拉()
        {
            string s = "";
            if (CPublic.Var.LocalUserTeam != "管理员权限" && CPublic.Var.LocalUserID != "admin")
            {
                s = string.Format(" and 属性值 in (select 原因分类 from 部门原因分类配置表  where 部门编号='{0}') ", CPublic.Var.localUser部门编号);
            }
            string sql = string.Format(@"select  属性值 as 原因分类,属性字段1 as 说明 from  基础数据基础属性表 
            where 属性类别='原因分类' and (属性字段2 = '材料出库' or 属性字段2 = '') {0} order by 属性值", s);
            DataTable dt_分类 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            //DataRow dr = dt_分类.NewRow();
            //dt_分类.Rows.Add(dr);
            //dr["原因分类"] = "入库倒冲";


            searchLookUpEdit1.Properties.DataSource = dt_分类;
            searchLookUpEdit1.Properties.ValueMember = "原因分类";
            searchLookUpEdit1.Properties.DisplayMember = "原因分类";
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
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
                 
                dr_借还["相关单位"] = textBox8.Text.ToString().Trim();


                string str_归还单号 = string.Format("GH{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                    CPublic.CNo.fun_得到最大流水号("GH", t.Year, t.Month));
                //DataRow[] dr_客户 = dt_客户信息.Select(string.Format("客户名称 = '{0}'", searchLookUpEdit1.EditValue));
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
                dr_申请主["归还操作人"] = CPublic.Var.localUserName;
                dr_申请主["备注"] = textBox6.Text;
                dr_申请主["归还申请日期"] = t;
                dr_申请主["归还完成"] = false;
                dr_申请主["原因分类"] = searchLookUpEdit1.EditValue;
                dr_申请主["借用类型"] = textBox3.Text;
                dr_申请主["归还说明"] = textBox10.Text;
               // dr_申请主["客户名称"] = textBox7.Text;
                dr_申请主["目标客户"] = textBox7.Text;
                dr_申请主["归还方式"] = "借用转耗用";
                //dr_申请主["文件GUID"] = dt_借还.Rows[0]["文件GUID"];
                //dr_申请主["附件"] =Convert.ToBoolean( dt_借还.Rows[0]["附件"]);
                //dr_申请主["文件"] = dt_借还.Rows[0]["文件"];
                //dr_申请主["上传时间"] = dt_借还.Rows[0]["上传时间"];
                //dr_申请主["后缀"] = dt_借还.Rows[0]["后缀"];
                

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
                    dr_归还申请子["需归还数量"] = Convert.ToDecimal(dr["最大归还数"]);
                    dr_归还申请子["计量单位"] = dr["计量单位"];
                    dr_归还申请子["计量单位编码"] = dr["计量单位编码"];
                    dr_归还申请子["借用数量"] = Convert.ToDecimal(dr["申请数量"]);
                    dr_归还申请子["归还完成"] = false;
                    DataRow[] dr_借还明细 = dt_归还清单.Select(string.Format("申请批号明细 = '{0}'", dr["申请批号明细"]));
                    dr_借还明细[0]["正在申请数"] = Convert.ToDecimal(dr["正在申请数"]) + Convert.ToDecimal(dr["最大归还数"]);
                    i++;
                }
                DataTable dt_审核 = ERPorg.Corg.fun_PA("生效", "借用转耗用申请单", str_归还单号, textBox8.Text.ToString());

                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction thrk = conn.BeginTransaction("借用转耗用申请");
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
                    da1.Update(dt_归还清单);

                    sql1 = "select * from 单据审核申请表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(dt_审核);

                    thrk.Commit();
                    MessageBox.Show("借用转耗用申请成功");
                    this.ParentForm.Close();

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

        //借用转耗用要审核 这个弃用
        private DataSet fun_save(string str_借用单号)
        {
            DataSet ds = new DataSet();
            DateTime t = CPublic.Var.getDatetime();
            try
            {
                DataTable dt_材料出库申请主表;
                DataTable dt_材料出库申请子表;
                DataTable dt_材料出库主;
                DataTable dt_材料出库子;
                DataTable dt_仓库出入库明细;

                string s = "select * from 其他出入库申请主表 where 1<>1";
                dt_材料出库申请主表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "select * from 其他出入库申请子表 where 1<>1";
                dt_材料出库申请子表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "select * from 其他出库主表 where 1<>1";
                dt_材料出库主 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "select * from 其他出库子表 where 1<>1";
                dt_材料出库子 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "select * from 仓库出入库明细表  where 1<>1";
                dt_仓库出入库明细 = CZMaster.MasterSQL.Get_DataTable(s, strconn);

                string str_材料出库申请单号 = string.Format("DWLS{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                             t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("DWLS", t.Year, t.Month).ToString("0000"));
                string str_材料出库单号 = string.Format("LS{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                   t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("LS", t.Year, t.Month).ToString("0000"));
                int i = 1;
                DataRow dr_材料申请主 = dt_材料出库申请主表.NewRow();
                dt_材料出库申请主表.Rows.Add(dr_材料申请主);
                dr_材料申请主["GUID"] = System.Guid.NewGuid();
                dr_材料申请主["出入库申请单号"] = str_材料出库申请单号;
                dr_材料申请主["申请日期"] = t;
                dr_材料申请主["申请类型"] = "材料出库";
                dr_材料申请主["备注"] = "借用转耗用：" + str_借用单号;
                dr_材料申请主["操作人员编号"] = CPublic.Var.LocalUserID;
                dr_材料申请主["操作人员"] = CPublic.Var.localUserName;
                dr_材料申请主["生效"] = true;
                dr_材料申请主["生效日期"] = t;
                dr_材料申请主["生效人员编号"] = CPublic.Var.LocalUserID;
                dr_材料申请主["完成"] = true;
                dr_材料申请主["完成日期"] = t;
                dr_材料申请主["原因分类"] = searchLookUpEdit1.EditValue;
                dr_材料申请主["单据类型"] = "材料出库";

                DataRow dr_材料出库主 = dt_材料出库主.NewRow();
                dt_材料出库主.Rows.Add(dr_材料出库主);
                dr_材料出库主["GUID"] = System.Guid.NewGuid();
                dr_材料出库主["其他出库单号"] = str_材料出库单号;
                dr_材料出库主["出库类型"] = "材料出库";
                dr_材料出库主["操作人员编号"] = CPublic.Var.LocalUserID;
                dr_材料出库主["操作人员"] = CPublic.Var.localUserName;
                dr_材料出库主["出库日期"] = t;
                dr_材料出库主["生效"] = true;
                dr_材料出库主["生效日期"] = t;
                dr_材料出库主["创建日期"] = t;
                dr_材料出库主["出入库申请单号"] = str_材料出库申请单号;

                foreach (DataRow dr in dt_归还清单.Rows)
                {
                    DataRow dr_材料申请子 = dt_材料出库申请子表.NewRow();
                    dt_材料出库申请子表.Rows.Add(dr_材料申请子);
                    dr_材料申请子["GUID"] = System.Guid.NewGuid();
                    dr_材料申请子["出入库申请单号"] = str_材料出库申请单号;
                    dr_材料申请子["POS"] = i;
                    dr_材料申请子["出入库申请明细号"] = str_材料出库申请单号 + "-" + i.ToString("00");
                    dr_材料申请子["物料编码"] = dr["物料编码"];
                    dr_材料申请子["规格型号"] = dr["规格型号"];
                    dr_材料申请子["物料名称"] = dr["物料名称"];
                    dr_材料申请子["数量"] = dr["最大归还数"];//倒冲数量=bom数量*成品入库数量

                    //  dr_apply_detail["备注"] = dr["物料编码"].ToString();//其他出入库申请主表备注记录成品入库单号 这边子表备注就记录成品编码
                    //19-6-23  计算 财务得 成本核算得时候 改为 工单号
                    dr_材料申请子["备注"] = dr["申请批号明细"];//其他出入库申请主表备注记录成品入库单号 这边子表备注就记录成品编码

                    dr_材料申请子["生效"] = true;
                    dr_材料申请子["生效日期"] = t;
                    dr_材料申请子["生效人员编号"] = CPublic.Var.LocalUserID;
                    dr_材料申请子["完成"] = true;
                    dr_材料申请子["完成日期"] = t;
                    dr_材料申请子["仓库号"] = dr["仓库号"];
                    dr_材料申请子["仓库名称"] = dr["仓库名称"];
                    dr_材料申请子["货架描述"] = dr["货架描述"];


                    DataRow dr_材料出库子 = dt_材料出库子.NewRow();
                    dt_材料出库子.Rows.Add(dr_材料出库子);
                    dr_材料出库子["物料编码"] = dr["物料编码"];
                    //dr_其他出库子["原ERP物料编号"] = dr_借用明细["原ERP物料编号"];
                    dr_材料出库子["物料名称"] = dr["物料名称"];
                    dr_材料出库子["数量"] = Convert.ToDecimal(dr_材料申请子["数量"]);

                    dr_材料出库子["规格型号"] = dr["规格型号"];
                    // dr_其他出库子["图纸编号"] = rr["图纸编号"];
                    dr_材料出库子["其他出库单号"] = str_材料出库单号;
                    dr_材料出库子["POS"] = i;
                    dr_材料出库子["其他出库明细号"] = str_材料出库单号 + "-" + i.ToString("00");
                    dr_材料出库子["GUID"] = System.Guid.NewGuid();
                    dr_材料出库子["备注"] =  str_借用单号;
                    dr_材料出库子["生效"] = true;
                    dr_材料出库子["生效日期"] = t;
                    dr_材料出库子["生效人员编号"] = CPublic.Var.LocalUserID;
                    dr_材料出库子["完成"] = true;
                    dr_材料出库子["完成日期"] = t;
                    dr_材料出库子["完成人员编号"] = CPublic.Var.LocalUserID;
                    dr_材料出库子["出入库申请单号"] = str_材料出库申请单号;
                    dr_材料出库子["出入库申请明细号"] = dr_材料申请子["出入库申请明细号"];

                    DataRow dr_出入库 = dt_仓库出入库明细.NewRow();
                    dt_仓库出入库明细.Rows.Add(dr_出入库);
                    dr_出入库["GUID"] = System.Guid.NewGuid();
                    dr_出入库["明细类型"] = "材料出库";
                    dr_出入库["单号"] = str_材料出库单号;
                    dr_出入库["出库入库"] = "出库";
                    dr_出入库["物料编码"] = dr["物料编码"];
                    dr_出入库["物料名称"] = dr["物料名称"];
                    dr_出入库["仓库号"] = dr["仓库号"];
                    dr_出入库["仓库名称"] = dr["仓库名称"];
                    dr_出入库["明细号"] = dr_材料出库子["其他出库明细号"];
                    dr_出入库["相关单号"] = str_材料出库申请单号;

                    //string ss = string.Format("select 车间名称 from 生产记录生产工单表 where 生产工单号='{0}'", dr["生产工单号"].ToString());
                    //DataTable t_s = CZMaster.MasterSQL.Get_DataTable(ss, strcon);
                    //dr_出入库["相关单位"] = t_s.Rows[0]["车间名称"];
                    dr_出入库["实效数量"] = -(Convert.ToDecimal(dr_材料出库子["数量"]));
                    dr_出入库["实效时间"] = t;
                    dr_出入库["出入库时间"] = t;
                    i++;
                }
                ds.Tables.Add(dt_材料出库申请主表);
                ds.Tables.Add(dt_材料出库申请子表);
                ds.Tables.Add(dt_材料出库主);
                ds.Tables.Add(dt_材料出库子);
                ds.Tables.Add(dt_仓库出入库明细);

            }
            catch (Exception ex)
            {
                throw ex;
            }


            return ds;
        }

        private void fun_check()
        {
            if(searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
            {
                throw new Exception("原因分类未选择，不可生效");
            }
            if (checkBox1.Checked == false)
            {
                 throw new Exception("附件未上传");
            }
            DataView dv = new DataView(dt_可还明细);
            dv.RowFilter = "选择= true";
            if(dv.Count == 0)
            {
                throw new Exception("未勾选明细,请确认");
            }
            
            foreach (DataRow dr in dt_可还明细.Rows)
            {
                if (!Convert.ToBoolean(dr["选择"])) continue;
                if ((Convert.ToDecimal(dr["已借出数量"]) - Convert.ToDecimal(dr["正在申请数"]) - Convert.ToDecimal(dr["归还数量"])) > 0)
                {
                    if (Convert.ToDecimal(dr["最大归还数"]) <= 0)
                    {
                        throw new Exception("转耗用数量不可小于等于零");
                    }
                    if (Convert.ToDecimal(dr["最大归还数"]) > (Convert.ToDecimal(dr["已借出数量"]) - Convert.ToDecimal(dr["归还数量"]) - Convert.ToDecimal(dr["正在申请数"])))
                    {
                        throw new Exception("转耗用数量超出借出数量");
                    }
                }

            }
            
           
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gvP.GetDataRow(gvP.FocusedRowHandle);
                dt_归还清单.Rows.Remove(dr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gvP_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gcP, new Point(e.X, e.Y));
            }
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
    }
}
