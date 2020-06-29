using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
namespace ERPStock
{
    public partial class frm工单返库 : UserControl
    {
        #region 变量
        string str_仓库;
        DataTable dtM;

        DataTable dtP;
        DataTable dt_仓库;
        DataTable dt_仓库号;
        string sql_ck = "";
        string cfgfilepath = "";
        string strconn = CPublic.Var.strConn;


        #endregion

        #region 加载
        public frm工单返库()
        {
            InitializeComponent();
        }

        private void Frm工单返库_Load(object sender, EventArgs e)
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
                string sql_ck = string.Format("select * from 人员仓库对应表 where 工号='{0}'", CPublic.Var.LocalUserID);
                dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_ck, strconn);

                string sql = "select 属性值 as 仓库名称,属性字段1 as 仓库号 from 基础数据基础属性表  where 属性类别 = '仓库类别'  and 布尔字段5 = 1";
                dt_仓库号 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                repositoryItemSearchLookUpEdit1.DataSource = dt_仓库号;
                repositoryItemSearchLookUpEdit1.DisplayMember = "仓库号";
                repositoryItemSearchLookUpEdit1.ValueMember = "仓库号";
                Fun_load();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }



        }
        #endregion
        #region 函数
        private void Fun_load()
        {
            string s = CPublic.Var.LocalUserID;
            sql_ck = "and gtsmx.仓库号  in(";
            string sql_左 = "";
       
            if (dt_仓库.Rows.Count == 0 || CPublic.Var.LocalUserID == "admin"|| CPublic.Var.LocalUserID=="910173")
            {
                sql_左 = string.Format(@"  select gtsz.*  from 工单退料申请表 gtsz 
                            where 待退料号 in(select 待退料号 from 工单退料申请明细表 gtsmx 
                     left join  基础数据物料信息表 on 基础数据物料信息表.物料编码 = gtsmx.物料编码 
                     where 完成=0 and gtsmx.关闭=0   group by 待退料号 )
                     and gtsz.作废=0 and gtsz.完成=0  ");
            }
            else
            {
                foreach (DataRow dr in dt_仓库.Rows)
                {
                    sql_ck = sql_ck + string.Format("'{0}',", dr["仓库号"]);

                }
                sql_ck = sql_ck.Substring(0, sql_ck.Length - 1) + ")";
                sql_左 = string.Format(@"select gtsz.*  from 工单退料申请表 gtsz
                        where 待退料号 in(select 待退料号 from 工单退料申请明细表 gtsmx  
                     left join  基础数据物料信息表 on 基础数据物料信息表.物料编码 = gtsmx.物料编码 
                     where 完成=0 and gtsmx.关闭 =0  {0} group by 待退料号 )
                     and gtsz.作废=0 and gtsz.完成=0  ", sql_ck);
            }
            using (SqlDataAdapter da = new SqlDataAdapter(sql_左, strconn))
            {
                dtM = new DataTable();

                da.Fill(dtM);
                gridControl1.DataSource = dtM;
            }
            string sql_mx = @"select gtsmx.*,(需退料数量-已退料数量) as 输入数量 from 工单退料申请明细表 gtsmx,基础数据物料信息表 

                         where 基础数据物料信息表.物料编码= gtsmx.物料编码 and 1<>1";
            dtP = new DataTable();
            dtP = CZMaster.MasterSQL.Get_DataTable(sql_mx, strconn);
            dtP.Columns.Add("选择", typeof(bool));

            gridControl2.DataSource = dtP;

        }

        private void Fun_check()
        {
            DataView dv = new DataView(dtP);
            this.BindingContext[dv].EndCurrentEdit();

            dv.RowFilter = "选择=1";

            if (dv.Count == 0)
            {
                throw new Exception("未选择明细");

            }
            foreach (DataRow r in dtP.Rows)
            {
                if (r["选择"].Equals(true))
                {
                    decimal a = 0;
                    try
                    {
                        a = Convert.ToDecimal(r["输入数量"]);

                    }
                    catch
                    {
                        throw new Exception("请正确输入退料数量格式");

                    }
                    if (a <= 0)
                    {
                        throw new Exception("退料数量不能小于0,请重新输入");

                    }
                    if (a > Convert.ToDecimal(r["需退料数量"]) - Convert.ToDecimal(r["已退料数量"]))
                    {
                        throw new Exception("输入的退料数量大于总需退料数量");
                    }

                }
            }
        }
        private DataSet Fun_退料记录()
        {

            DataSet ds = new DataSet();
            DataRow dr_left = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            DateTime t = CPublic.Var.getDatetime();
            //DateTime t = new DateTime(2019, 7, 30);
            //return of material RM
            string str_退料单号 = string.Format("RM{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day,
            CPublic.CNo.fun_得到最大流水号("RM", t.Year, t.Month));
            string sql = "select * from 工单返库单主表 where 1<>1";
            DataTable dt_m = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            string s_mx = "select * from  工单返库单明细表 where 1<>1";
            DataTable dt_mx = CZMaster.MasterSQL.Get_DataTable(s_mx, strconn);
            DataRow dr_m = dt_m.NewRow();
            dr_m["退料单号"] = str_退料单号;
            dr_m["待退料号"] = dr_left["待退料号"];
            dr_m["工单号"] = dr_left["生产工单号"];
            dr_m["产品编码"] = dr_left["产品编号"];
            dr_m["产品名称"] = dr_left["产品名称"];
            dr_m["车间"] = dr_left["车间"];
            dr_m["日期"] = t;

            dr_m["操作人"] = CPublic.Var.localUserName;
            dt_m.Rows.Add(dr_m);
            dt_m.TableName = "主表";
            ds.Tables.Add(dt_m);
            int i = 1;
            foreach (DataRow r in dtP.Rows)
            {
                if (r["选择"].Equals(true))
                {
                    DataRow dr_mx = dt_mx.NewRow();
                    dr_mx["退料单号"] = str_退料单号;
                    dr_mx["退料明细号"] = str_退料单号 + "-" + i.ToString("00");
                    dr_mx["待退料号"] = dr_left["待退料号"];
                    dr_mx["工单号"] = dr_left["生产工单号"];
                    dr_mx["物料编码"] = r["物料编码"];
                    dr_mx["物料名称"] = r["物料名称"];
                    dr_mx["返库数量"] = Convert.ToDecimal(r["输入数量"]);
                    dr_mx["入库人ID"] = CPublic.Var.LocalUserID;
                    dr_mx["入库人员"] = CPublic.Var.localUserName;
                    dr_mx["日期"] = t;

                    dt_mx.Rows.Add(dr_mx);
                    i++;
                }
            }
            dt_mx.TableName = "明细表";
            ds.Tables.Add(dt_mx);
            return ds;
        }
        private DataSet Fun_退料申请()
        {
            DataRow dr_left = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            //DateTime t = new DateTime(2019, 7, 30);
            DateTime t = CPublic.Var.getDatetime();
            DataSet ds = new DataSet();
            string sql = string.Format("select * from  工单退料申请明细表 where 待退料号='{0}' ", dr_left["待退料号"]);
            DataTable dt_mx = dtP.Copy();
            int c_total = dt_mx.Rows.Count;
            int i = 0;
            foreach (DataRow r in dt_mx.Rows)
            {
                if (r["选择"].Equals(true))
                {
                    r["已退料数量"] = Convert.ToDecimal(r["已退料数量"]) + Convert.ToDecimal(r["输入数量"]);
                    if (Convert.ToDecimal(r["已退料数量"]) == Convert.ToDecimal(r["需退料数量"]))
                    {
                        r["完成"] = true;
                        r["完成日期"] = t;
                    }
                    if (r["完成"].Equals(true))
                    {
                        i++;
                    }
                }
            }
            string s_主 = string.Format("select * from  工单退料申请表 where 待退料号='{0}' ", dr_left["待退料号"]);
            DataTable dt_z = CZMaster.MasterSQL.Get_DataTable(s_主, strconn);
            DataTable dt_审核 = new DataTable();

            if (i == c_total)
            {
                dt_z.Rows[0]["完成"] = true;
                dt_z.Rows[0]["完成日期"] = t;
                
            }
            dt_z.TableName = "主表";
            ds.Tables.Add(dt_z);

            dt_mx.TableName = "明细表";
            ds.Tables.Add(dt_mx);
            return ds;


        }
        private DataSet Fun_待领料单()
        {
            DataRow dr_left = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            DataSet ds = new DataSet();

            //如果工单是作废的 就不用 管待发料 明细
            string s_check = string.Format(@"select 关闭  from 生产记录生产工单表 where 生产工单号='{0}'", dr_left["生产工单号"]);
            DataTable t_check = CZMaster.MasterSQL.Get_DataTable(s_check, strconn);
            //if (t_check.Rows[0]["关闭"].Equals(false))
            //{
                string s = string.Format("select * from 生产记录生产工单待领料主表 where 生产工单号='{0}'", dr_left["生产工单号"]);
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                dt.Rows[0]["完成"] = 0;
                dt.TableName = "主表";
                ds.Tables.Add(dt);
                string sql = string.Format("select * from 生产记录生产工单待领料明细表 where 生产工单号='{0}'", dr_left["生产工单号"]);
                DataTable dt_mx = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                foreach (DataRow r in dtP.Rows)
                {
                    if (r["选择"].Equals(true))
                    {
                        DataRow[] x = dt_mx.Select(string.Format("物料编码='{0}'", r["物料编码"]));
                        x[0]["已领数量"] = Convert.ToDecimal(x[0]["已领数量"]) - Convert.ToDecimal(r["输入数量"]);
                        x[0]["未领数量"] = Convert.ToDecimal(x[0]["未领数量"]) + Convert.ToDecimal(r["输入数量"]);

                        if (Convert.ToDecimal(x[0]["未领数量"]) < 0)
                        {
                            throw new Exception("生产发料单未领数量异常,未发这么多料无法退这么多料");

                        }
                        x[0]["完成"] = 0;
                        x[0]["备注1"] = "有过退料";

                    }
                }
                dt.TableName = "明细表";
                ds.Tables.Add(dt_mx);
            // }
            return ds;
        }
        private DataTable Fun_出入库表(string str_退料类型,string str_车间,DataTable dtx,DataTable dtt)
        {
            string sql = "select * from 仓库出入库明细表 where 1<>1";
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

          DateTime t = CPublic.Var.getDatetime();

            //DateTime t = new DateTime(2019,7,30) ;


            foreach (DataRow r in dtt.Rows)
            {

                DataRow dr = dt.NewRow();

                dr["GUID"] = System.Guid.NewGuid();

                dr["明细类型"] = "工单退料";

                dr["单号"] = r["退料单号"];
                dr["明细号"] = r["退料明细号"];
                dr["物料编码"] = r["物料编码"];
                dr["物料名称"] = r["物料名称"];
                dr["相关单位"] = str_车间;
                DataRow []rr=  dtx.Select(string.Format("物料编码='{0}'", r["物料编码"]));
                dr["仓库号"] = rr[0]["仓库号"];
                dr["仓库名称"] = rr[0]["仓库名称"];
                dr["出库入库"] = "出库";
                string s = string.Format("select 计量单位 from 基础数据物料信息表 where 物料编码='{0}'", dr["物料编码"]);
                using (SqlDataAdapter da = new SqlDataAdapter(s, strconn))
                {
                    DataTable table = new DataTable();
                    da.Fill(table);
                    dr["单位"] = table.Rows[0][0];

                }
                dr["实效数量"] =Convert.ToDecimal(r["返库数量"]);
                dr["实效时间"] = t;
                dr["出入库时间"] = t;
                dr["相关单号"] = r["工单号"];
                dr["仓库人"] = CPublic.Var.localUserName;

                dt.Rows.Add(dr);
            }

            return dt;

        }
        private DataTable Fun_库存()
        {
            DataTable dt = new DataTable();
           DateTime t=  CPublic.Var.getDatetime();
            //DateTime t = new DateTime(2019, 7, 30);
            foreach (DataRow r in dtP.Rows)
            {
                if (r["选择"].Equals(true))
                {
                    string sql = string.Format("select * from 仓库物料数量表 where 物料编码='{0}'", r["物料编码"]);
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                    {
                        da.Fill(dt);
                        DataRow[] x = dt.Select(string.Format("物料编码='{0}'", r["物料编码"]));
                        x[0]["库存总数"] = Convert.ToDecimal(x[0]["库存总数"]) + Convert.ToDecimal(r["输入数量"]);
                        x[0]["出入库时间"] = t;

                    }
                }
            }

            return dt;
        }
        //private void fun_主表状态(string str_工单号)
        //{
        //    string  sql=string.Format(@"select  * from 工单返库单明细表 where 工单号='{0}'",str_工单号);
        //    using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
        //    {
        //        DataTable dt = new DataTable();
        //        da.Fill(dt);

        //        DataRow[] dr = dt.Select(string.Format("明细完成=1"));

        //        if (dr.Length == dt.Rows.Count)
        //        {
        //            string sql_1 = string.Format(@"select  * from 工单返库单主表 where 工单号='{0}'", str_工单号);
        //           using (SqlDataAdapter da_1 =new SqlDataAdapter  (sql_1,strconn))
        //           {
        //               DataTable dt_1 = new DataTable();
        //               da.Fill(dt_1);
        //               dt_1.Rows[0]["完成"] = 1;
        //               dt_1.Rows[0]["完成日期"] = CPublic.Var.getDatetime();
        //               new SqlCommandBuilder(da_1);
        //               da_1.Update(dt_1);

        //            }

        //        }
        //    }
        //}
        private void Fun_loadmx(string str_待退料号)
        {
            string sql_mx = "";
            if (dt_仓库.Rows.Count == 0 || CPublic.Var.LocalUserID == "admin" || CPublic.Var.LocalUserID == "910173")
            {
                sql_mx = string.Format(@"select gtsmx.*,base.规格型号,库存总数,(需退料数量-已退料数量) as 输入数量,计量单位 as 单位,gtsmx.仓库号,gtsmx.仓库名称
                        from 工单退料申请明细表 gtsmx,基础数据物料信息表 base ,工单退料申请表,仓库物料数量表 kc 
                   where base.物料编码= gtsmx.物料编码  and gtsmx.待退料号=工单退料申请表.待退料号 and gtsmx.仓库号=kc.仓库号
                    and kc.物料编码=base.物料编码 and gtsmx.待退料号='{0}' and gtsmx.完成=0 ", str_待退料号);
            }
            else
            {
                sql_mx = string.Format(@"select gtsmx.*,base.规格型号,库存总数,(需退料数量-已退料数量) as 输入数量,计量单位 as 单位,gtsmx.仓库号,gtsmx.仓库名称
                        from 工单退料申请明细表 gtsmx,基础数据物料信息表 base ,工单退料申请表,仓库物料数量表 kc 
                   where base.物料编码= gtsmx.物料编码  and gtsmx.待退料号=工单退料申请表.待退料号 and gtsmx.仓库号=kc.仓库号
                    and kc.物料编码=base.物料编码 and gtsmx.待退料号='{0}' and gtsmx.完成=0 {1} ", str_待退料号, sql_ck);
            }

                //string sql_mx = string.Format(@"select gtsmx.*,base.规格型号,库存总数,(需退料数量-已退料数量) as 输入数量,计量单位 as 单位,gtsmx.仓库号,gtsmx.仓库名称
                //        from 工单退料申请明细表 gtsmx,基础数据物料信息表 base ,工单退料申请表,仓库物料数量表 kc 
                //   where base.物料编码= gtsmx.物料编码  and gtsmx.待退料号=工单退料申请表.待退料号 and gtsmx.仓库号=kc.仓库号
                //    and kc.物料编码=base.物料编码 and gtsmx.待退料号='{0}' and gtsmx.完成=0 {1} ", str_待退料号, sql_ck);

            dtP = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(sql_mx, strconn))
            {
                da.Fill(dtP);
                dtP.Columns.Add("选择", typeof(bool));
                foreach (DataRow dr in dtP.Rows)
                {
                    dr["选择"] = true;
                }
                gridControl2.DataSource = dtP;
            }
        }
        #endregion
        #region 界面操作
        //  刷新
        private void BarLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Fun_load();
        }
        //关闭
        private void BarLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();

        }
        // 生效
        private void BarLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridView2.CloseEditor();
            this.BindingContext[dtP].EndCurrentEdit();
            DataRow FocusedRow = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            try
            {
                Fun_check();
                if (MessageBox.Show("确定返库？", "核实!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DataSet ds_待领料 = new DataSet();
                    if (FocusedRow["退料类型"].ToString() != "返工退料")
                    {
                        ds_待领料 = Fun_待领料单();
                    }
                    DataTable dtx = dtP.Copy();
                    dtx.Columns["输入数量"].ColumnName = "数量";
                    DataView dv = new DataView(dtx);
                    dv.RowFilter = "选择=1";
                    dtx = dv.ToTable();
                    DataTable dt_库存 = ERPorg.Corg.fun_库存(1, dtx);
                    DataSet ds_退料 = Fun_退料记录();
                    DataSet ds_退料申请 = Fun_退料申请();
                    DataTable dt_出入库 = Fun_出入库表(FocusedRow["退料类型"].ToString(), FocusedRow["车间"].ToString(), dtx, ds_退料.Tables[1]);
                    DataTable dt_审核 =new DataTable();
                    if (ds_退料申请.Tables[0].Rows[0]["退料类型"].ToString() == "工单关闭退料" && Convert.ToBoolean(ds_退料申请.Tables[0].Rows[0]["完成"]))
                    {
                        string sql_审核 = string.Format("select * from 单据审核申请表 where 关联单号 = '{0}' and 单据类型 = '工单关闭'", ds_退料申请.Tables[0].Rows[0]["生产工单号"].ToString());
                        dt_审核 = CZMaster.MasterSQL.Get_DataTable(sql_审核, strconn);
                        if (dt_审核.Rows.Count > 0)
                        {

                        }
                        else
                        {
                            dt_审核 = ERPorg.Corg.fun_PA("关闭", "工单关闭", ds_退料申请.Tables[0].Rows[0]["生产工单号"].ToString(), "", ds_退料申请.Tables[0].Rows[0]["操作人ID"].ToString());
                        }
                    }


                    string sql = "select * from 工单返库单主表 where 1<>1";
                    string s_mx = "select * from  工单返库单明细表 where 1<>1";
                    SqlConnection conn = new SqlConnection(strconn);
                    conn.Open();
                    SqlTransaction mrm = conn.BeginTransaction("工单退料");
                    try
                    {
                        SqlCommand cmm_1 = new SqlCommand(sql, conn, mrm);
                        SqlCommand cmm_2 = new SqlCommand(s_mx, conn, mrm);
                        SqlDataAdapter da_主表 = new SqlDataAdapter(cmm_1);
                        SqlDataAdapter da_明细表 = new SqlDataAdapter(cmm_2);
                        new SqlCommandBuilder(da_主表);
                        new SqlCommandBuilder(da_明细表);
                        da_主表.Update(ds_退料.Tables[0]);
                        da_明细表.Update(ds_退料.Tables[1]);
                        if (FocusedRow["退料类型"].ToString() != "返工退料" && ds_待领料.Tables.Count != 0)
                        {
                            sql = string.Format("select * from 生产记录生产工单待领料主表 where  1<>1");
                            s_mx = string.Format("select * from 生产记录生产工单待领料明细表 where 1<>1");
                            cmm_1 = new SqlCommand(sql, conn, mrm);
                            cmm_2 = new SqlCommand(s_mx, conn, mrm);
                            da_主表 = new SqlDataAdapter(cmm_1);
                            da_明细表 = new SqlDataAdapter(cmm_2);
                            new SqlCommandBuilder(da_主表);
                            new SqlCommandBuilder(da_明细表);
                            da_主表.Update(ds_待领料.Tables[0]);
                            da_明细表.Update(ds_待领料.Tables[1]);
                        }
                        sql = string.Format("select * from 工单退料申请表 where  1<>1");
                        s_mx = string.Format("select * from 工单退料申请明细表 where 1<>1");
                        cmm_1 = new SqlCommand(sql, conn, mrm);
                        cmm_2 = new SqlCommand(s_mx, conn, mrm);
                        da_主表 = new SqlDataAdapter(cmm_1);
                        da_明细表 = new SqlDataAdapter(cmm_2);
                        new SqlCommandBuilder(da_主表);
                        new SqlCommandBuilder(da_明细表);
                        da_主表.Update(ds_退料申请.Tables[0]);
                        if (dt_审核.Rows.Count>0)
                        {
                            sql = "select * from 单据审核申请表 where 1<>1";
                            cmm_1 = new SqlCommand(sql, conn, mrm);
                            da_主表 = new SqlDataAdapter(cmm_1);
                            new SqlCommandBuilder(da_主表);
                            da_主表.Update(dt_审核);
                        }
                        da_明细表.Update(ds_退料申请.Tables[1]);
                        sql = "select * from 仓库出入库明细表 where 1<>1";
                        cmm_1 = new SqlCommand(sql, conn, mrm);
                        da_主表 = new SqlDataAdapter(cmm_1);
                        new SqlCommandBuilder(da_主表);
                        da_主表.Update(dt_出入库);
                        sql = "select * from 仓库物料数量表 where 1<>1";
                        cmm_1 = new SqlCommand(sql, conn, mrm);
                        da_主表 = new SqlDataAdapter(cmm_1);
                        new SqlCommandBuilder(da_主表);
                        da_主表.Update(dt_库存);
                        mrm.Commit();
                    }
                    catch (Exception ex)
                    {
                        mrm.Rollback();
                        throw new Exception("工单退料失败");
                    }
                    MessageBox.Show("生效成功");
                    Fun_load();
                }
            }

            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        #endregion



        #region 控件事件
        private void GridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }

        }


        private void GridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                Fun_loadmx(dr["待退料号"].ToString());

            }
            catch (Exception ex)
            {

                throw;
            }


        }

        private void 明细完成ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("是否确认完成这条明细？", "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {

                DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                string sql = string.Format("select * from 工单退料申请明细表 where 物料编码='{0}' and 待退料号='{1}'", dr["物料编码"], dr["待退料号"]);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dt.Rows[0]["完成"] = 1;
                    dt.Rows[0]["完成日期"] = CPublic.Var.getDatetime();
                    new SqlCommandBuilder(da);
                    da.Update(dt);
                }
                Fun_loadmx(dr["待退料号"].ToString());
                //  fun_主表状态(dr["工单号"].ToString());

            }
        }

        #endregion

        private void GridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void GridView2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gridControl2, new Point(e.X, e.Y));
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void GridView1_ColumnWidthChanged(object sender, DevExpress.XtraGrid.Views.Base.ColumnEventArgs e)
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gridView1.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void GridView1_ColumnPositionChanged(object sender, EventArgs e)
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gridView1.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
                    gridControl1.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);

            }
        }

        //20-1-14 可修改仓库
        private void gridView2_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                if (e.Column.FieldName == "仓库号")
                {
                    dr["仓库号"] = e.Value;
                    DataRow[] ds = dt_仓库号.Select(string.Format("仓库号 = '{0}'", dr["仓库号"]));
                    dr["仓库名称"] = ds[0]["仓库名称"];
                    //dr["仓库名称"] = sr["仓库名称"].ToString();
                    string sql = "select * from 仓库物料数量表 where 物料编码 = '" + dr["物料编码"] + "' and 仓库号 = '" + dr["仓库号"] + "'";
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_物料数量 = new DataTable();
                    da.Fill(dt_物料数量);
                    if (dt_物料数量.Rows.Count == 0)
                    {
                        dr["库存总数"] = 0;                        
                    }
                    else
                    {
                        dr["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
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
