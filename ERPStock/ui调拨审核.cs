using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;

namespace ERPStock
{
    public partial class ui调拨审核 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dtM;
        DataTable dtP;
        DataTable dt_人员;
        DataTable dt_代办;
        DataTable dt_仓库;
        #endregion

        public ui调拨审核()
        {
            InitializeComponent();
        }

        private void fun_load()
        {
            string sql = "select  *  from  调拨申请主表 where  作废=0 and 提交审核 = 1 and 完成=0 and  审核=0 and 生效=0   ";
            dtM = new DataTable();
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gc_代办.DataSource = dtM;
            fun_仓库();

        }

        private void fun_仓库()
        {
            dt_仓库 = new DataTable();
            string sql4 = "select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别'";
            SqlDataAdapter  da = new SqlDataAdapter(sql4, strconn);
            da.Fill(dt_仓库);
            repositoryItemSearchLookUpEdit2.DataSource = dt_仓库;
            repositoryItemSearchLookUpEdit2.DisplayMember = "仓库号";
            repositoryItemSearchLookUpEdit2.ValueMember = "仓库号";
        }

        private void fun_mx(DataRow dr)
        {
            string sql = string.Format(@"select  mx.*,(数量-已处理数量) as 输入数量,isnull(库存总数,0)库存总数,base.物料名称 from 调拨申请明细表 mx 
                                    left join 仓库物料数量表 kc on kc.物料编码=mx.物料编码 and mx.原仓库号=kc.仓库号  
                                   left  join 基础数据物料信息表 base on base.物料编码=mx.物料编码
                                  where 完成=0  and 调拨申请单号='{0}'", dr["调拨申请单号"]);
            dtP = new DataTable();
            dtP = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            DataColumn dc = new DataColumn("选择", typeof(bool));
            dc.DefaultValue = false;
            dtP.Columns.Add(dc);

            gc.DataSource = dtP;

        }
        private void fun_check()
        {

            DataView vv = new DataView(dtP);
            vv.RowFilter = "选择=1";
            if (vv.Count == 0) throw new Exception("未选择任何记录");
            foreach (DataRow dr in vv.ToTable().Rows)
            {
             
                string sql = string.Format("select 库存总数 from 仓库物料数量表 where  物料编码='{0}' and 仓库号='{1}'", dr["物料编码"], dr["原仓库号"]);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                if (temp.Rows.Count > 0)
                {
                    if (Convert.ToDecimal(temp.Rows[0]["库存总数"]) < Convert.ToDecimal(dr["输入数量"]))
                    {
                        throw new Exception("库存不足");
                    }
                }
                else
                {
                   throw new Exception(string.Format("未找到物料：{0}在仓库：{1}的记录,尝试刷新后重试", dr["物料编码"], dr["原仓库"]));
                }

            }

        }

        private void fun_save()
        {
            DataRow dr_左 = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);
            string str_id = CPublic.Var.LocalUserID;
            string str_name = CPublic.Var.localUserName;
            DateTime t = CPublic.Var.getDatetime(); 
            //DateTime t = new DateTime(2019, 7, 14);

            string s_sq = "select * from 其他出入库申请主表 where 1<>1";
            DataTable dt_申请 = CZMaster.MasterSQL.Get_DataTable(s_sq, strconn);
            s_sq = "select * from 其他出库主表 where 1<>1";
            DataTable dt_出库 = CZMaster.MasterSQL.Get_DataTable(s_sq, strconn);
            s_sq = "select * from 其他出库子表 where 1<>1";
            DataTable dt_出库_子 = CZMaster.MasterSQL.Get_DataTable(s_sq, strconn);
            s_sq = "select * from 其他入库主表 where 1<>1";
            DataTable dt_入库 = CZMaster.MasterSQL.Get_DataTable(s_sq, strconn);
            s_sq = "select * from 其他入库子表 where 1<>1";
            DataTable dt_入库_子 = CZMaster.MasterSQL.Get_DataTable(s_sq, strconn);
            s_sq = "select * from 其他出入库申请子表 where 1<>1";
            DataTable dt_申请明细 = CZMaster.MasterSQL.Get_DataTable(s_sq, strconn);
            s_sq = "select * from 仓库出入库明细表 where 1<>1";
            DataTable dt_出入库明细 = CZMaster.MasterSQL.Get_DataTable(s_sq, strconn);
            #region 申请主表两条记录
            string s_部门 = string.Format("select 部门 from 人事基础员工表 where 员工号='{0}'",dr_左["申请人员"]);
            DataTable dt_reny = CZMaster.MasterSQL.Get_DataTable(s_部门, strconn);
            s_部门 = "";
            if (dt_reny.Rows.Count>0)
            {
                s_部门 = dt_reny.Rows[0]["部门"].ToString();
            }
            DataRow dr = dt_申请.NewRow();
            dr["GUID"] = System.Guid.NewGuid();
            string s_out = string.Format("QWSQ{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
            t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("QWSQ", t.Year, t.Month).ToString("0000"));
            dr["出入库申请单号"] = s_out;
            dr["申请日期"] = t;
            dr["申请类型"] = "其他出库";
            dr["备注"] = dr_左["备注"];
            dr["操作人员编号"] = str_id;
            dr["操作人员"] = str_name;
            dr["生效日期"] = t;
            dr["生效"] = true;
            dr["生效人员编号"] = str_id;
            dr["完成日期"] = t;
            dr["完成"] = true;
            dr["原因分类"] = "调拨出库";
            dr["部门名称"] = s_部门;   
            //2019-5-22  
            dr["业务单号"] =dr_左["调拨申请单号"];
            dt_申请.Rows.Add(dr);
            DataRow dr_入 = dt_申请.NewRow();
            dr_入["GUID"] = System.Guid.NewGuid();
            string s_in = string.Format("QWSQ{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
             t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("QWSQ", t.Year, t.Month).ToString("0000"));
            dr_入["出入库申请单号"] = s_in;
            dr_入["申请日期"] = t;
            dr_入["申请类型"] = "其他入库";
            dr_入["备注"] = dr_左["备注"];
            dr_入["操作人员编号"] = str_id;
            dr_入["操作人员"] = str_name;
            dr_入["生效日期"] = t;
            dr_入["生效"] = true;
            dr_入["生效人员编号"] = str_id;
            dr_入["完成日期"] = t;
            dr_入["完成"] = true;
            dr_入["原因分类"] = "调拨入库";
            dr_入["部门名称"] = s_部门;
            //2019-5-22  
            dr_入["业务单号"] = dr_左["调拨申请单号"];

            dt_申请.Rows.Add(dr_入);
            #endregion
            #region 其他出库主
            string s出库_no = string.Format("QT{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                  t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("QT", t.Year, t.Month).ToString("0000"));
            DataRow dr_出库主 = dt_出库.NewRow();
            dr_出库主["GUID"] = System.Guid.NewGuid();
            dr_出库主["其他出库单号"] = s出库_no;
            dr_出库主["创建日期"] = t;
            dr_出库主["操作人员编号"] = str_id;
            dr_出库主["操作人员"] = str_name;
            dr_出库主["出库仓库"] = "";
            dr_出库主["领用人员"] = "";
            dr_出库主["领用人员编号"] = "";
            dr_出库主["生效"] = true;
            dr_出库主["生效人员编号"] = str_id;
            dr_出库主["生效日期"] = t;
            dr_出库主["出库日期"] = t;
            dr_出库主["出库类型"] = "其他出库";
            dr_出库主["出入库申请单号"] = s_out;
            dt_出库.Rows.Add(dr_出库主);
            #endregion

            #region 其他入库主
            string s入库_no = string.Format("QW{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                  t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("QW", t.Year, t.Month).ToString("0000"));
            DataRow dr_入库主 = dt_入库.NewRow();
            dr_入库主["GUID"] = System.Guid.NewGuid();
            dr_入库主["其他入库单号"] = s入库_no;
            dr_入库主["创建日期"] = t;
            dr_入库主["操作人员编号"] = str_id;
            dr_入库主["操作人员"] = str_name;

            dr_入库主["入库人员"] = str_id;
            dr_入库主["入库人员编号"] = str_name;
            dr_入库主["生效"] = true;
            dr_入库主["生效人员编号"] = str_id;
            dr_入库主["生效日期"] = t;
            dr_入库主["入库日期"] = t;
            //dr_入库主["出库类型"] = "其他入库";
            dr_入库主["出入库申请单号"] = s_in;
            dt_入库.Rows.Add(dr_入库主);
            #endregion
            int pos = 1;
            foreach (DataRow r in dtP.Rows)
            {
                if (Convert.ToBoolean(r["选择"]))
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    #region  仓库出入库明细
                    DataRow xr = dt_出入库明细.NewRow();
                    xr["GUID"] = System.Guid.NewGuid();
                    xr["明细类型"] = "其他出库";
                    xr["单号"] = s出库_no;
                    xr["相关单号"] = s_out;
                    xr["物料编码"] = r["物料编码"].ToString();
                    xr["物料名称"] = r["物料名称"].ToString();
                    xr["明细号"] = s出库_no + "-" + pos.ToString("00");
                    xr["出库入库"] = "出库";
                    xr["实效数量"] = -Convert.ToDecimal(r["输入数量"].ToString());
                    xr["仓库号"] = r["原仓库号"];
                    xr["仓库名称"] = r["原仓库"];
                    xr["实效时间"] = t;
                    xr["出入库时间"] = t;
                    xr["仓库人"] = CPublic.Var.localUserName;
                    dt_出入库明细.Rows.Add(xr);
               
                    DataRow xr1 = dt_出入库明细.NewRow();
                    xr1["GUID"] = System.Guid.NewGuid();
                    xr1["明细类型"] = "其他入库";
                    xr1["单号"] = s入库_no;
                    xr1["相关单号"] = s_in;
                    xr1["物料编码"] = r["物料编码"].ToString();
                    xr1["物料名称"] = r["物料名称"].ToString();
                    xr1["明细号"] = s入库_no + "-" + pos.ToString("00");
                    xr1["出库入库"] = "入库";
                    xr1["实效数量"] = Convert.ToDecimal(r["输入数量"].ToString());
                    xr1["仓库号"] = dr_左["目标仓库号"];
                    xr1["仓库名称"] = dr_左["目标仓库"];
                    xr1["实效时间"] = t;
                    xr1["出入库时间"] = t;
                    xr1["仓库人"] = CPublic.Var.localUserName;
                    dt_出入库明细.Rows.Add(xr1);
                    string s = string.Format("select * from 人事基础部门表 where 部门编号='{0}'", CPublic.Var.localUser课室编号);
                    DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                    if (temp.Rows.Count > 0)
                    {
                        dr["相关单位"] = temp.Rows[0]["部门名称"];
                        xr["相关单位"] = temp.Rows[0]["部门名称"];
                        xr1["相关单位"] = temp.Rows[0]["部门名称"];
                    }
                    // 输入数量
                    //xr["实效数量"] = -Convert.ToDecimal(r["数量"].ToString());
                    //xr1["实效数量"] = Convert.ToDecimal(r["数量"].ToString());
                    #endregion
                    #region 其他申请子表记录
                    DataRow dr_其他出申请子表 = dt_申请明细.NewRow();
                    dr_其他出申请子表["GUID"] = System.Guid.NewGuid();
                    dr_其他出申请子表["出入库申请单号"] = s_out;
                    dr_其他出申请子表["出入库申请明细号"] = s_out + "-" + pos.ToString("00");
                    dr_其他出申请子表["POS"] = pos;
                    dr_其他出申请子表["物料编码"] = r["物料编码"].ToString();
                    dr_其他出申请子表["物料名称"] = r["物料名称"].ToString();
                   // dr_其他出申请子表["数量"] = r["数量"];
                    dr_其他出申请子表["数量"] = r["输入数量"];

                    dr_其他出申请子表["规格型号"] = r["规格型号"].ToString();
                    dr_其他出申请子表["完成"] = true;
                    dr_其他出申请子表["完成日期"] = t;
                    dr_其他出申请子表["生效"] = true;
                    dr_其他出申请子表["生效人员编号"] = CPublic.Var.LocalUserID;
                    dr_其他出申请子表["生效日期"] = t;
                    dr_其他出申请子表["仓库号"] = r["原仓库号"];
                    dr_其他出申请子表["仓库名称"] = r["原仓库"];
                    // dr_其他出申请子表["货架描述"]  
                    dt_申请明细.Rows.Add(dr_其他出申请子表);
                    DataRow dr_1 = dt_申请明细.NewRow();
                    dr_1["GUID"] = System.Guid.NewGuid();
                    dr_1["出入库申请单号"] = s_in;
                    dr_1["出入库申请明细号"] = s_in + "-" + pos.ToString("00");
                    dr_1["POS"] = pos;
                    dr_1["物料编码"] = r["物料编码"].ToString();
                    dr_1["物料名称"] = r["物料名称"].ToString();
                  //  dr_1["数量"] = r["数量"];
                    dr_1["数量"] = r["输入数量"];

                    dr_1["规格型号"] = r["规格型号"].ToString();
                    dr_1["完成"] = true;
                    dr_1["完成日期"] = t;
                    dr_1["生效"] = true;
                    dr_1["生效人员编号"] = str_id;
                    dr_1["生效日期"] = t;
                    dr_1["仓库号"] = dr_左["目标仓库号"];
                    dr_1["仓库名称"] = dr_左["目标仓库"];
                    dr_1["货架描述"] = r["新货架描述"];
                    dt_申请明细.Rows.Add(dr_1);
                    #endregion
                    #region 其他出库子表记录
                    DataRow dr_其他出子表 = dt_出库_子.NewRow();
                    dr_其他出子表["GUID"] = System.Guid.NewGuid();
                    dr_其他出子表["其他出库单号"] = s出库_no;
                    dr_其他出子表["其他出库明细号"] = s出库_no + "-" + pos.ToString("00");
                    dr_其他出子表["POS"] = pos;
                    dr_其他出子表["物料编码"] = r["物料编码"].ToString();
                    dr_其他出子表["物料名称"] = r["物料名称"].ToString();
                    dr_其他出子表["数量"] = r["输入数量"];
                    //dr_其他出子表["数量"] = r["数量"];

                    dr_其他出子表["规格型号"] = r["规格型号"].ToString();
                    dr_其他出子表["完成"] = true;
                    dr_其他出子表["完成日期"] = t;
                    dr_其他出子表["生效"] = true;
                    dr_其他出子表["生效人员编号"] = CPublic.Var.LocalUserID;
                    dr_其他出子表["生效日期"] = t;
                    dr_其他出子表["出入库申请单号"] = s_out;
                    dr_其他出子表["出入库申请明细号"] = s_out + "-" + pos.ToString("00");
                    dt_出库_子.Rows.Add(dr_其他出子表);

                    #endregion
                    #region 入库子表记录
                    DataRow dr_入子 = dt_入库_子.NewRow();
                    dr_入子["GUID"] = System.Guid.NewGuid();
                    dr_入子["其他入库单号"] = s入库_no;
                    dr_入子["其他入库明细号"] = s入库_no + "-" + pos.ToString("00");
                    dr_入子["POS"] = pos;
                    dr_入子["物料编码"] = r["物料编码"].ToString();
                    dr_入子["物料名称"] = r["物料名称"].ToString();
                  //  dr_入子["数量"] = r["数量"];
                    dr_入子["数量"] = r["输入数量"];
                    dr_入子["规格型号"] = r["规格型号"].ToString();
                    dr_入子["生效"] = true;
                    dr_入子["生效人员编号"] = CPublic.Var.LocalUserID;
                    dr_入子["生效日期"] = t;
                    dr_入子["出入库申请单号"] = s_in;
                    dr_入子["出入库申请明细号"] = s_in + "-" + pos.ToString("00");
                    dt_入库_子.Rows.Add(dr_入子);
                    #endregion
                    pos++;

                    r["已处理数量"] = Convert.ToDecimal(r["已处理数量"]) + Convert.ToDecimal(r["输入数量"]);  
                    if (Convert.ToDecimal(r["输入数量"]) >= Convert.ToDecimal(r["数量"]) - Convert.ToDecimal(r["已处理数量"]))
                    {
                        r["完成"] = 1;
                        r["完成日期"] = t;
                    }
                }
            }
            //dt_代办 
            DataView v = new DataView(dtP);
            v.RowFilter = "完成=0";
            if(v.Count==0)
            {
                dr_左["生效"] = true;
                dr_左["审核"] = true;
                //dr_左["审核日期"] = t;
                dr_左["审核人员"] = str_name;
                dr_左["审核人员ID"] = str_id;
                dr_左["生效人员ID"] = str_id;
                dr_左["生效人员"] = str_name;
            }
            v = new DataView(dtP);
            v.RowFilter = "选择=1";
            DataTable t_mx = v.ToTable();
  

            t_mx.Columns["原仓库号"].ColumnName = "仓库号";
            t_mx.Columns["原仓库"].ColumnName = "仓库名称";
            t_mx.Columns["数量"].ColumnName = "申请数量";
            t_mx.Columns["输入数量"].ColumnName = "数量";
            DataTable t_kc = ERPorg.Corg.fun_库存(-1, t_mx);
            DataTable dt_货架描述;
            foreach (DataRow tt in t_mx.Rows)
            {
                tt["仓库号"] = dr_左["目标仓库号"];
                tt["仓库名称"] = dr_左["目标仓库"];
                string sql_货架描述 = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}' and 仓库号 = '{1}'",tt["物料编码"].ToString(),tt["仓库号"].ToString());
                dt_货架描述 = CZMaster.MasterSQL.Get_DataTable(sql_货架描述, strconn);
                if (dt_货架描述.Rows.Count > 0)
                {
                    tt["新货架描述"] = dt_货架描述.Rows[0]["货架描述"];
                }
                else
                {
                    tt["新货架描述"] = "";
                }
            }

            DataTable t_kc_2 = ERPorg.Corg.fun_库存(1, t_mx);
            t_kc.Merge(t_kc_2);

            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction dbsh = conn.BeginTransaction("调拨审核");
            try
            {
                string sql1 = "select * from 其他出入库申请主表 where 1<>1";
                SqlCommand cmd1 = new SqlCommand(sql1, conn, dbsh);
                SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(dt_申请);
                sql1 = "select * from 其他出入库申请子表 where 1<>1";
                cmd1 = new SqlCommand(sql1, conn, dbsh);
                da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(dt_申请明细);

                sql1 = "select * from 其他出库主表 where 1<>1";
                cmd1 = new SqlCommand(sql1, conn, dbsh);
                da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(dt_出库);

                sql1 = "select * from 其他出库子表 where 1<>1";
                cmd1 = new SqlCommand(sql1, conn, dbsh);
                da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(dt_出库_子);

                sql1 = "select * from 其他入库主表 where 1<>1";
                cmd1 = new SqlCommand(sql1, conn, dbsh);
                da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(dt_入库);

                sql1 = "select * from 其他入库子表 where 1<>1";
                cmd1 = new SqlCommand(sql1, conn, dbsh);
                da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(dt_入库_子);
                sql1 = "select * from 仓库出入库明细表 where 1<>1";
                cmd1 = new SqlCommand(sql1, conn, dbsh);
                da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(dt_出入库明细);

                sql1 = "select * from 仓库物料数量表 where 1<>1";
                cmd1 = new SqlCommand(sql1, conn, dbsh);
                da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(t_kc);

                sql1 = "select * from 调拨申请主表 where 1<>1";
                cmd1 = new SqlCommand(sql1, conn, dbsh);
                da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(dtM);

                sql1 = "select * from 调拨申请明细表 where 1<>1";
                cmd1 = new SqlCommand(sql1, conn, dbsh);
                da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(dtP);
                dbsh.Commit();
            }
            catch (Exception ex)
            {
                dbsh.Rollback();
                throw new Exception(ex.Message + "调拨审核出现问题,尝试刷新后重试");
            }






        }
        private void ui调拨审核_Load(object sender, EventArgs e)
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
                x.UserLayout(splitContainer1, this.Name, cfgfilepath);
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void gv_代办_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);

            fun_mx(dr);
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show("是否确认审核当前选中申请单？", "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    this.ActiveControl = null;
                    fun_check();
                    fun_save();
                    barLargeButtonItem1_ItemClick(null, null);
                    MessageBox.Show("审核成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (dtP != null) gc.DataSource = dtP.Clone();
            fun_load();
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {





        }

        private void barLargeButtonItem7_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            DataRow drM = (this.BindingContext[gc_代办.DataSource].Current as DataRowView).Row;
            DataTable dtm = (DataTable)this.gc.DataSource;
            Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
            Type outerForm = outerAsm.GetType("ERPreport.调拨单", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统

            object[] drr = new object[2];

            drr[0] = drM;
            drr[1] = dtm;
            //   drr[2] = dr["出入库申请单号"].ToString();
            Form ui = Activator.CreateInstance(outerForm, drr) as Form;
            //  UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
            ui.ShowDialog();

        }

        private void gv_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (e.Column.FieldName == "原仓库号")
                {
                    dr["原仓库号"] = e.Value;
                    DataRow[] ds = dt_仓库.Select(string.Format("仓库号 = {0}", dr["原仓库号"]));
                    dr["原仓库"] = ds[0]["仓库名称"];
                    string sql = "select * from 仓库物料数量表 where 物料编码 = '" + dr["物料编码"] + "' and 仓库号 = '" + dr["原仓库号"] + "'";
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_物料数量 = new DataTable();
                    da.Fill(dt_物料数量);
                    if (dt_物料数量.Rows.Count == 0)
                    {
                        dr["库存总数"] = 0;
                        dr["新货架描述"] = "";
                    }
                    else
                    {
                        dr["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
                        dr["新货架描述"] = dt_物料数量.Rows[0]["货架描述"];//19-9-17解决货架更新
                    }
                }
            }

            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_代办_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                DataRow dr = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);
                fun_mx(dr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
