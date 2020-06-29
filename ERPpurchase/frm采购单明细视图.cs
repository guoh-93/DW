using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CZMaster;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace ERPpurchase
{
    public partial class frm采购单明细视图 : UserControl
    {
        #region 成员
        DataRow drM;
        DataTable dtM;
        DataTable dtP;
        string podan;
        string strconn = CPublic.Var.strConn;
        DataTable dt_物料编码 = new DataTable();
        DataTable dt_产品金额对照 = new DataTable();
        int POS = 0;
        string cfgfilepath = "";
        #endregion

        #region
        public frm采购单明细视图(string strpodan)
        {
            podan = strpodan;
            InitializeComponent();
        }

        public frm采购单明细视图()
        {
            InitializeComponent();
        }

        private void frm采购单明细视图_Load(object sender, EventArgs e)
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
                x.UserLayout(this.panel3, this.Name, cfgfilepath);

                barEditItem5.EditValue = "款到发货";
                fun_物料下拉框();
                fun_载入供应商();
                fun_载入数据();
                if (drM["作废"].Equals(true) || drM["审核"].Equals(true))
                {
                    // barLargeButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    simpleButton1.Visible = false;
                    // simpleButton2.Visible = false;
                    //gvm.OptionsBehavior.Editable = false;
                    gridColumn2.OptionsColumn.AllowEdit = false;
                    gridColumn5.OptionsColumn.AllowEdit = false;
                    gridColumn12.OptionsColumn.AllowEdit = false;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }




        private void fun_载入供应商()
        {
            try
            {
                string sql2 = "select 供应商ID,供应商名称 from 采购供应商表 where 供应商状态 = '在用' order by 供应商ID";
                DataTable dt_供应商表 = new DataTable();
                dt_供应商表 = MasterSQL.Get_DataTable(sql2, CPublic.Var.strConn);

                searchLookUpEdit1.Properties.DataSource = dt_供应商表;
                searchLookUpEdit1.Properties.DisplayMember = "供应商ID";
                searchLookUpEdit1.Properties.ValueMember = "供应商ID";
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_下拉框的采购供应商表");
                throw new Exception(ex.Message);
            }
        }

        //查询明细
        private void fun_载入数据()
        {
            try
            {
                string sql = string.Format("select * from 采购记录采购单主表 where 采购单号='{0}'", podan);
                dtM = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
                if (dtM.Rows[0]["审核人员ID"].ToString() == CPublic.Var.LocalUserID||CPublic.Var.LocalUserTeam=="管理员权限")
                {
                    barLargeButtonItem1.Enabled = true;
                }
                else
                {
                    barLargeButtonItem1.Enabled = false;
                }
                if (dtM.Rows.Count > 0)
                {
                    drM = dtM.Rows[0];
                    textBox1.Text = drM["作废日期"].ToString();
                    dataBindHelper1.DataFormDR(drM);
                }
                if (drM["已检验"].Equals(true))
                {
                    barLargeButtonItem5.Enabled = false;
                }
                string sql1 = string.Format(@"select 采购记录采购单明细表.* from 采购记录采购单明细表              
                where 采购记录采购单明细表.采购单号='{0}'", podan);
                dtP = MasterSQL.Get_DataTable(sql1, CPublic.Var.strConn);
                string sql_pos = string.Format("select max(采购POS) from 采购记录采购单明细表 where 采购单号='{0}'", podan);
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(sql_pos, strconn))
                {
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        POS = Convert.ToInt32(dt.Rows[0][0]);
                    }
                }

                if (dtP.Rows.Count > 0)
                    gcm.DataSource = dtP;
                dtP.ColumnChanged += dtP_ColumnChanged;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_SearchDetail");
                throw new Exception(ex.Message);
            }
        }


        private void dtP_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {

            //if (e.Column.ColumnName == "物料编码")
            //{
            //    DataRow[] dr = dt_物料编码.Select(string.Format("物料编码='{0}'", e.Row["物料编码"].ToString()));
            //    if (dr.Length > 0)
            //    {
            //        e.Row["物料名称"] = dr[0]["物料名称"];
            //        e.Row["数量单位"] = dr[0]["计量单位"];
            //        e.Row["规格型号"] = dr[0]["规格"];
            //        e.Row["图纸编号"] = dr[0]["图纸编号"];
            //        e.Row["图纸版本"] = dr[0]["图纸版本"];
            //        e.Row["仓库ID"] = dr[0]["仓库号"];
            //        e.Row["仓库名称"] = dr[0]["仓库名称"];

            //        e.Row["原ERP物料编号"] = dr[0]["原ERP物料编号"];
            //        e.Row["n原ERP规格型号"] = dr[0]["n原ERP规格型号"];




            //        DataRow[] dr1 = dt_产品金额对照.Select(string.Format("产品编号='{0}'", e.Row["物料编码"].ToString()));
            //        if (dr1.Length > 0)
            //        {
            //            if (Convert.ToInt32(dr1[0]["采购价格"]) != 0)
            //            {
            //                e.Row["单价"] = dr1[0]["采购价格"];
            //            }
            //            else
            //            {
            //                e.Row["单价"] = dr[0]["标准单价"];
            //            }
            //        }
            //        else
            //        {
            //            e.Row["单价"] = dr[0]["标准单价"];
            //        }
            //    }
            //}
            if (e.Column.ColumnName == "采购数量" || e.Column.ColumnName == "单价")
            {
                fun_金额的变化();
            }
        }
        Decimal shlv = 0;
        //单据的总金额
        decimal djzje = 0;
        //订单的未税金额
        decimal ddwsje = 0;
        private void fun_金额的变化()
        {
            Decimal s = 0;
            shlv = Convert.ToDecimal(txt_shuilv.Text) / 100;
            foreach (DataRow r in dtP.Rows)
            {
                if (r["采购数量"].ToString() != "" && r["单价"].ToString() != "")
                {
                    r["金额"] = (Convert.ToDecimal(r["采购数量"]) * Convert.ToDecimal(r["单价"])).ToString("0.000000");  //金额
                    r["税金"] = ((Convert.ToDecimal(r["金额"]) / (1 + shlv)) * shlv).ToString("0.000000");   //计算税金
                    r["未税单价"] = (Convert.ToDecimal(r["单价"]) / (1 + shlv)).ToString("0.000000");
                    r["未税金额"] = (Convert.ToDecimal(r["金额"]) / (1 + shlv)).ToString("0.000000");
                    s += Convert.ToDecimal(r["金额"]);
                    r["未完成数量"] = r["采购数量"];
                }
            }
            txt_shuihouje.Text = s.ToString("0.00000");
            ddwsje = s / (1 + shlv);
            txt_weishuije.Text = ddwsje.ToString("0.00000");

            txt_shuijin.Text = ((s / (1 + shlv)) * shlv).ToString("0.000000");  //计算税金
            djzje = s;
        }

        private void gvm_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gcm, new Point(e.X, e.Y));
                }

                DataRow dr = gvm.GetDataRow(gvm.FocusedRowHandle);
                if (dr == null) return;
                if (dr["明细完成"].Equals(true))
                {
                    gridColumn2.OptionsColumn.AllowEdit = false;
                    gridColumn5.OptionsColumn.AllowEdit = false;
                    gridColumn12.OptionsColumn.AllowEdit = false;
                    gridColumn9.OptionsColumn.AllowEdit = false;
                }
                else
                {

                    gridColumn9.OptionsColumn.AllowEdit = true;

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        private void 采购入库明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gvm.GetDataRow(gvm.FocusedRowHandle);
            string name = string.Format("采购入库明细({0}_{1})", dr["物料编码"].ToString().Trim(), dr["物料名称"].ToString().Trim());
            frm采购入库视图 frm = new frm采购入库视图(dr["采购单号"].ToString().Trim(), 2);
            CPublic.UIcontrol.AddNewPage(frm, name);
        }

        #region 界面
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
              
                foreach (DataRow tr in dtP.Rows)
                {
                    if (Convert.ToDecimal(tr["已送检数"]) > 0) throw new Exception("已有送检不可整单作废");
                }
                if (textBox2.Text == "委外采购")
                {
                    //并且料已经发了 不可以作废
                        throw new Exception("委外采购单暂不支持整单关闭功能");
                }
                if (MessageBox.Show(string.Format("你确定要作废采购单\"{0}\"", txt_caigoudh.Text), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DateTime t1 = CPublic.Var.getDatetime();
                    //作废
                    drM["作废"] = true;
                    drM["作废日期"] = t1;
                    drM["作废人员ID"] = CPublic.Var.LocalUserID;
                    drM["作废人员"] = CPublic.Var.localUserName;
                    //drM["完成"] = true;
                    //drM["完成日期"] = System.DateTime.Now;
                    foreach (DataRow dr in dtP.Rows)
                    {
                        dr["作废"] = true;
                        dr["作废日期"] = t1;
                        dr["作废人员ID"] = CPublic.Var.LocalUserID;
                        dr["作废人员"] = CPublic.Var.localUserName;
                        //dr["明细完成"] = true;
                        //dr["明细完成日期"] = System.DateTime.Now;
                        //dr["总完成"] = true;
                        //dr["总完成日期"] = System.DateTime.Now;
                    }
                    SqlConnection conn = new SqlConnection(strconn);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("采购修改保存"); //事务的名称
                    string s_审核 = string.Format("select  * from 单据审核申请表 where 关联单号='{0}'", txt_caigoudh.Text);
                    DataTable dt_审核 = CZMaster.MasterSQL.Get_DataTable(s_审核, strconn);
                    dt_审核.Rows[0]["作废"] = true;

                    try
                    {


                        string sql_主 = "select * from 采购记录采购单主表 where 1<>1";
                        SqlCommand cmd = new SqlCommand(sql_主, conn, ts);
                        SqlDataAdapter da_主 = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da_主);
                        da_主.Update(dtM);

                        string sql_明细 = "select * from 采购记录采购单明细表 where 1<>1";
                        cmd = new SqlCommand(sql_明细, conn, ts);
                        SqlDataAdapter da_明细 = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da_明细);
                        da_明细.Update(dtP);

                        cmd = new SqlCommand(s_审核, conn, ts);
                        da_明细 = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da_明细);
                        da_明细.Update(dt_审核);

                        ts.Commit();
                    }
                    catch
                    {
                        ts.Rollback();
                        throw new Exception("保存出错");
                    }

                    //刷新在途量
                    foreach (DataRow r in dtP.Rows)
                    {
                        StockCore.StockCorer.fun_物料数量_实际数量(r["物料编码"].ToString(), r["仓库号"].ToString(), true);
                    }
                    MessageBox.Show("已作废");



                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DateTime t = CPublic.Var.getDatetime();
            //关闭采购单
            drM["完成"] = true;
            drM["完成日期"] = t;
            string sql_主 = "select * from 采购记录采购单主表 where 1<>1";
            SqlDataAdapter da_主 = new SqlDataAdapter(sql_主, strconn);
            new SqlCommandBuilder(da_主);
            da_主.Update(dtM);

            foreach (DataRow dr in dtP.Rows)
            {
                dr["明细完成"] = true;
                dr["明细完成日期"] = t;
                dr["总完成"] = true;
                dr["总完成日期"] = t;
            }
            string sql_明细 = "select * from 采购记录采购单明细表 where 1<>1";
            SqlDataAdapter da_明细 = new SqlDataAdapter(sql_明细, strconn);
            new SqlCommandBuilder(da_明细);
            da_明细.Update(dtP);

            //刷新在途量
            foreach (DataRow r in dtP.Rows)
            {
                StockCore.StockCorer.fun_物料数量_实际数量(r["物料编码"].ToString(), r["仓库号"].ToString(), true);
            }
            MessageBox.Show("已关闭");

        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //关闭
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //打印

        }
        #endregion

        #region
        private void fun_物料下拉框()
        {

            string sqlll = @"select base.物料编码,base.物料名称,base.规格型号,base.图纸编号,base.计量单位,
                        base.标准单价,kc.仓库号,kc.仓库名称,base.图纸版本,kc.库存总数,kc.有效总数,kc.在途量
                        from 基础数据物料信息表 base  left join 仓库物料数量表 kc on base.物料编码 = kc.物料编码 
                            where base.可购=1 and base.停用= 0";
            dt_物料编码 = MasterSQL.Get_DataTable(sqlll, strconn);

            repositoryItemSearchLookUpEdit1.PopupFormSize = new Size(1000, 400);
            repositoryItemSearchLookUpEdit1.DataSource = dt_物料编码;
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";

            //string sql4 = "select * from 产品金额对照表";
            //dt_产品金额对照 = MasterSQL.Get_DataTable(sql4, CPublic.Var.strConn);
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();

                gvm.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                DataRow r = dtP.NewRow();
                dtP.Rows.Add(r);

                //gvm.FocusedRowHandle = gvm.LocateByDisplayText(0, gridColumn28, "");
                if (dtP.Rows.Count > 0)
                {
                    r["到货日期"] = dtP.Rows[0]["到货日期"];
                }
                r["GUID"] = System.Guid.NewGuid().ToString();
                r["采购POS"] = ++POS;
                r["采购单号"] = txt_caigoudh.Text;
                r["采购明细号"] = txt_caigoudh.Text + "-" + POS.ToString("00");
                r["供应商ID"] = searchLookUpEdit1.EditValue;
                r["供应商"] = txt_cggys.Text;
                r["供应商负责人"] = txt_gysfzr.Text;
                r["供应商电话"] = txt_gysdh.Text;
                r["税率"] = txt_shuilv.Text;
                r["生效"] = 1;
                r["员工号"] = r["生效人员ID"] = CPublic.Var.LocalUserID;
                r["采购人"] = r["生效人员"] = CPublic.Var.localUserName;
                r["生效日期"] = t;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        bool bl_作废 = false;
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();

                if (dtP.Rows.Count <= 0)
                    throw new Exception("没有采购单明细可以作废！");
                DataRow rr = gvm.GetDataRow(gvm.FocusedRowHandle);
                if (Convert.ToDecimal(rr["已送检数"]) > 0) { throw new Exception("已有送检数,可修改采购数量,或者在送检界面,完成该明细"); }

                if(textBox2.Text.Trim()=="委外采购")
                {
                    ////改为取发料最小情况  19-6-20
                    //string sss = string.Format("select * from 其他出入库申请子表  where 备注='{0}' ", rr["采购明细号"]);
                    //DataTable t_1 = CZMaster.MasterSQL.Get_DataTable(sss, strconn);
                    //DataRow[] r = t_1.Select(string.Format("已完成数量=min(已完成数量)"));

                    //if (r.Length > 0)
                    //{
                    //    decimal dec = Convert.ToDecimal(r[0]["已完成数量"]) / (Convert.ToDecimal(r[0]["数量"]) / Convert.ToDecimal(rr["采购数量"]));
                    //    if (Convert.ToDecimal(rr["已送检数"])< dec) throw new Exception(string.Format("已发料况只允许到货数量为{0}", dec));
                    //}
                    //else
                    //{
                    //    throw new Exception("该条委外采购记录没有相应的委外发料申请单,请确认");
                    //}
                    ////if (t.Rows.Count > 0) throw new Exception("原料尚未发出,不可送检");

                    //if (Convert.ToDecimal(dt_songjianMx.Rows[0]["送检数量"]) > Convert.ToDecimal(dt_songjianMx.Rows[0]["可送检数量"]))
                        throw new Exception("委外采购单暂不允许使用此功能");
                }
                //rr.Delete();
                rr["作废"] = 1;
                rr["作废人员"] = CPublic.Var.localUserName;
                rr["作废人员ID"] = CPublic.Var.LocalUserID;
                rr["作废日期"] = t;
                Decimal s = 0;
                shlv = Convert.ToDecimal(txt_shuilv.Text) / 100;
                foreach (DataRow r in dtP.Rows)
                {
                    if (rr["作废"].Equals(true)) continue;
                    if (r.RowState == DataRowState.Deleted) continue;
                    s += Convert.ToDecimal(r["金额"]);
                }
                txt_shuihouje.Text = s.ToString("0.000000"); //总金额
                ddwsje = s / (1 + shlv);
                txt_shuijin.Text = ((s / (1 + shlv)) * shlv).ToString("0.000000");  //计算税金
                djzje = s;

                barLargeButtonItem5_ItemClick(null, null);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void fun_check采购单明细()
        {
            // int pos = 1;
            foreach (DataRow r in dtP.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;
                DataRow[] dr_cfx = dtP.Select(string.Format("物料编码='{0}'", r["物料编码"].ToString()));
                if (dr_cfx.Length > 1)
                    throw new Exception(string.Format("物料编码\"{0}\"有重复，只允许有一个！", r["物料编码"].ToString()));

                //物料编码的检查：物料编码是由订单号+采购顺序的POS
                if (r["物料编码"].ToString() == "")
                    throw new Exception("请选择物料编码，物料编码不能为空！");
                //采购数量的检查
                if (r["采购数量"].ToString() == "")
                    throw new Exception("请填写采购数量，采购数量不能为空！");
                try
                {
                    double d = Convert.ToDouble(r["采购数量"]);
                }
                catch
                {
                    throw new Exception("采购数量是数字，请检查一下，重新填写！");
                }

                try
                {
                    double d = Convert.ToDouble(r["单价"]);
                    if (d <= 0)
                    {
                        throw new Exception("单价不能小于0！");
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                if (r["采购价"] == DBNull.Value || r["采购价"].ToString() == "" || Convert.ToDecimal(r["采购价"]) == 0)
                {
                    r["采购价"] = r["单价"];
                }
                //到货日期的检查
                if (r["到货日期"].ToString() == "")
                    throw new Exception("请选择到货日期，到货日期不能为空！");
                r["操作员ID"] = CPublic.Var.LocalUserID;
                r["操作员"] = CPublic.Var.localUserName;
            }
        }

        private DataTable fun_PA(string str_采购单号)
        {

            DataTable dt_申请;
            string s = string.Format("select * from  单据审核申请表 where 关联单号='{0}'", str_采购单号);
            dt_申请 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            DateTime t = CPublic.Var.getDatetime();
            string str_pa = "";
            DataRow r_upper = ERPorg.Corg.fun_hr_upper("采购单", CPublic.Var.LocalUserID);
            if (dt_申请.Rows.Count == 0)
            {
                str_pa = string.Format("PA{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("PA", t.Year, t.Month));
                // 申请主表记录
                DataRow r_z = dt_申请.NewRow();
                r_z["审核申请单号"] = str_pa;
                r_z["关联单号"] = txt_caigoudh.Text;
                r_z["单据类型"] = "采购";
                decimal dec = Convert.ToDecimal(txt_shuihouje.Text);
                r_z["总金额"] = dec;

                //  s = string.Format("select  * from  审核人员金额权限配置表 where 金额上限>{0} and 类型='采购' order by 金额上限 ", dec);
                //DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);

                r_z["待审核人ID"] = r_upper["工号"];
                r_z["待审核人"] = r_upper["姓名"];
                r_z["申请时间"] = t;
                r_z["申请人ID"] = CPublic.Var.LocalUserID;
                r_z["申请人"] = CPublic.Var.localUserName;
                //if (CPublic.Var.LocalUserID == temp.Rows[0]["工号"].ToString())   //下采购单的人和审核人一致 提交上级
                //{
                //    r_z["待审核人ID"] = temp.Rows[1]["工号"];
                //    r_z["待审核人"] = temp.Rows[1]["姓名"];
                //}
                dt_申请.Rows.Add(r_z);
            }
            else
            {
                str_pa = dt_申请.Rows[0]["审核申请单号"].ToString();
                decimal dec = Convert.ToDecimal(txt_shuihouje.Text);
                dt_申请.Rows[0]["总金额"] = dec;
                //s = string.Format("select  * from  审核人员金额权限配置表 where 金额上限>{0} and 类型='采购' order by 金额上限 ", dec);
                //DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                dt_申请.Rows[0]["待审核人ID"] = r_upper["工号"];
                dt_申请.Rows[0]["待审核人"] = r_upper["姓名"];
                dt_申请.Rows[0]["申请时间"] = t;
                dt_申请.Rows[0]["申请人ID"] = CPublic.Var.LocalUserID;
                dt_申请.Rows[0]["申请人"] = CPublic.Var.localUserName;
                //if (CPublic.Var.LocalUserID == temp.Rows[0]["工号"].ToString())   //下采购单的人和审核人一致 提交上级
                //{
                //    dt_申请.Rows[0]["待审核人ID"] = temp.Rows[1]["工号"];
                //    dt_申请.Rows[0]["待审核人"] = temp.Rows[1]["姓名"];
                //}
            }

            return dt_申请;
        }
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gvm.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                fun_check采购单明细();
                txt_xiugairiqi.Text = CPublic.Var.getDatetime().ToString();
                dataBindHelper1.DataToDR(drM);
                //drM["修改日期"] = CPublic.Var.getDatetime();
                drM["未税金额"] = ddwsje;
                if (drM["操作员ID"].ToString() == "")
                {
                    drM["操作员ID"] = CPublic.Var.LocalUserID;
                    drM["操作员"] = CPublic.Var.localUserName;
                }
                DataTable dt = new DataTable();
                if (!bl_作废)
                {
                    dt = fun_PA(txt_caigoudh.Text);
                }
                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("pur"); //事务的名称
                SqlCommand cmd1 = new SqlCommand("select * from 采购记录采购单主表 where 1<>1", conn, ts);
                SqlCommand cmd = new SqlCommand("select * from 采购记录采购单明细表 where 1<>1", conn, ts);

                try
                {
                    SqlDataAdapter da;
                    da = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da);
                    da.Update(dtM);

                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dtP);
                    if (!bl_作废)
                    {
                        cmd = new SqlCommand("select * from 单据审核申请表 where 1<>1", conn, ts);
                        da = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da);
                        da.Update(dt);
                    }
                    ts.Commit();
                }
                catch
                {
                    ts.Rollback();
                }

                foreach (DataRow r in dtP.Rows)
                {
                    StockCore.StockCorer.fun_物料数量_实际数量(r["物料编码"].ToString(), r["仓库号"].ToString(), true);
                }
                //19-8-12 方便起见
                if (txt_caigoudh.Text != "")
                {
                    string s = @" update 采购记录采购送检单主表  set 供应商ID='{1}',供应商='{2}'   where 采购单号='{0}'
                                 update  采购记录采购送检单主表  set 供应商ID='{1}',供应商='{2}'   where 采购单号='{0}'  
                                  update 采购记录采购单检验主表 set 供应商编号='{1}',供应商名称='{2}'  where 采购单号='{0}'  
                               update   采购记录采购单入库明细  set  供应商ID='{1}',供应商='{2}' where 采购单号='{0}'
                                update 采购记录采购单入库主表 set 供应商ID='{1}',供应商 ='{2}' 
                                where 入库单号 in (select 入库单号  from 采购记录采购单入库明细 where  采购单号 ='{0}') ";
                    CZMaster.MasterSQL.ExecuteSQL(s, strconn);
                }
                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
        // 供应商修改 

        private void searchLookUpEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (e.OldValue != null && e.OldValue.ToString() != "")
            {
                string sql = string.Format("select 供应商ID,供应商名称,供应商负责人,供应商电话,交期,税率 from 采购供应商表 where 供应商ID='{0}'", e.NewValue);
                DataTable dt = new DataTable();

                dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                if (dt.Rows.Count > 0)
                {
                    txt_cggys.Text = dt.Rows[0]["供应商名称"].ToString().Trim();
                    txt_gysfzr.Text = dt.Rows[0]["供应商负责人"].ToString().Trim();
                    txt_gysdh.Text = dt.Rows[0]["供应商电话"].ToString().Trim();

                    txt_shuilv.Text = dt.Rows[0]["税率"].ToString().Trim();

                    foreach (DataRow dr in dtP.Rows)
                    {
                        dr["供应商ID"] = e.NewValue.ToString().Trim();
                        dr["供应商"] = dt.Rows[0]["供应商名称"].ToString().Trim();
                        dr["供应商负责人"] = dt.Rows[0]["供应商负责人"].ToString().Trim();
                        dr["供应商电话"] = dt.Rows[0]["供应商电话"].ToString().Trim();
                        dr["税率"] = dt.Rows[0]["税率"].ToString().Trim();

                    }
                }


            }
        }

        private void gvm_ColumnPositionChanged(object sender, EventArgs e)
        {
            try
            {
                if (cfgfilepath != "")
                {
                    gvm.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void repositoryItemSearchLookUpEdit1View_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);

            DataRow r = gvm.GetDataRow(gvm.FocusedRowHandle);
            if (r == null) return;
            r["物料名称"] = dr["物料名称"];
            r["数量单位"] = dr["计量单位"];
            r["规格型号"] = dr["规格型号"];
            r["图纸编号"] = dr["图纸编号"];
            r["图纸版本"] = dr["图纸版本"];
            r["仓库号"] = dr["仓库号"];
            r["仓库名称"] = dr["仓库名称"];


            string sql4 = string.Format(@"select * from 采购供应商物料单价表 where 物料编码='{0}' and 供应商ID='{1}'", dr["物料编码"].ToString(), searchLookUpEdit1.EditValue);
            dt_产品金额对照 = CZMaster.MasterSQL.Get_DataTable(sql4, strconn);
            DataRow[] dr1 = dt_产品金额对照.Select(string.Format("产品编号='{0}'", dr["物料编码"].ToString()));
            if (dr1.Length > 0)
            {
                if (Convert.ToInt32(dr1[0]["采购价格"]) != 0)
                {
                    r["单价"] = dr1[0]["采购价格"];
                }
                else
                {
                    r["单价"] = dr["标准单价"];
                }
            }
            else
            {
                r["单价"] = dr["标准单价"];
            }


            fun_金额的变化();

        }

        private void repositoryItemSearchLookUpEdit1View_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            DataRow dr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);

            DataRow r = gvm.GetDataRow(gvm.FocusedRowHandle);

            r["物料名称"] = dr["物料名称"];
            r["数量单位"] = dr["计量单位"];
            r["规格型号"] = dr["规格型号"];
            r["图纸编号"] = dr["图纸编号"];
            r["图纸版本"] = dr["图纸版本"];
            r["仓库号"] = dr["仓库号"];
            r["仓库名称"] = dr["仓库名称"];






            DataRow[] dr1 = dt_产品金额对照.Select(string.Format("产品编号='{0}'", dr["物料编码"].ToString()));
            if (dr1.Length > 0)
            {
                if (Convert.ToInt32(dr1[0]["采购价格"]) != 0)
                {
                    r["单价"] = dr1[0]["采购价格"];
                }
                else
                {
                    r["单价"] = dr["标准单价"];
                }
            }
            else
            {
                r["单价"] = dr["标准单价"];
            }


            fun_金额的变化();
        }

        private void barLargeButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show(string.Format("该采购单是否确认弃审？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string sql = string.Format("select * from 采购记录采购单主表 where 采购单号 = '{0}'", txt_caigoudh.Text);
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_撤销 = new DataTable();
                    da.Fill(dt_撤销);
                    sql = string.Format("select * from 采购记录采购单明细表 where 采购单号 = '{0}'", txt_caigoudh.Text);
                    da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_撤销子 = new DataTable();
                    da.Fill(dt_撤销子);
                    sql = string.Format("select * from 单据审核申请表 where 关联单号 = '{0}'", txt_caigoudh.Text);
                    da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_审核申请 = new DataTable();
                    da.Fill(dt_审核申请);
                    //sql = string.Format("select * from 单据审核日志表 where 审核申请单号 = '{0}'", txt_caigoudh.Text);
                    //da = new SqlDataAdapter(sql, strconn);
                    //DataTable dt_审核日志 = new DataTable();
                    //da.Fill(dt_审核日志);
                    if (dt_撤销.Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(dt_撤销.Rows[0]["审核"]))
                        {
                            //20-1月上旬  这里需要判断如果 委外材料出库单 有出去的数量 不可以弃审   暂时先让供应链操作
                            if (dt_撤销.Rows[0]["采购单类型"].ToString() == "委外采购" )
                            {
                                if (CPublic.Var.LocalUserTeam != "管理员权限" && CPublic.Var.LocalUserID != "admin")
                                {

                                    throw new Exception("委外采购单暂不支持弃审，请联系供应链进行操作");
                                }
                            }

                            dt_撤销.Rows[0]["审核"] = 0;
                            dt_撤销.Rows[0]["审核人员"] = "";
                            dt_撤销.Rows[0]["审核人员ID"] = "";
                            //dt_撤销.Rows[0]["审核日期"] = DBNull.Value;
                            dt_撤销.Rows[0]["生效"] = 0;
                            dt_撤销.Rows[0]["生效人员"] = "";
                            dt_撤销.Rows[0]["生效人员ID"] = "";
                            //dt_撤销.Rows[0]["生效日期"] = DBNull.Value;


                            if (dt_审核申请.Rows.Count > 0)
                            {
                                dt_审核申请.Rows[0]["审核"] = 0;
                            }
                            //if (dt_审核日志.Rows.Count > 0)
                            //{
                            //    dt_审核日志.Rows[0].Delete();
                            //}
                            string sql_jhmx = $"select * from 主计划计划通知单明细 where 生效 = 1 and 关闭 = 0";
                            DataTable dt_计划通知明细 = CZMaster.MasterSQL.Get_DataTable(sql_jhmx, strconn);                            
                            if (dt_撤销子.Rows.Count > 0)
                            {
                                foreach (DataRow dr1 in dt_撤销子.Rows)
                                {
                                    dr1["生效"] = 0;
                                    dr1["生效人员"] = "";
                                    dr1["生效人员ID"] = "";

                                    if (dr1["备注9"].ToString() != "")
                                    {
                                        DataRow[] dr_1 = dt_计划通知明细.Select($"计划通知单明细号 = '{dr1["备注9"]}'");
                                        if (dr_1.Length > 0)
                                        {                                           
                                            dr_1[0]["已转采购数量"] = Convert.ToDecimal(dr_1[0]["已转采购数量"])- Convert.ToDecimal(dr1["采购数量"]);
                                            dr_1[0]["完成"] = 0;
                                        }
                                    }
                                }
                            }
                            SqlConnection conn = new SqlConnection(strconn);
                            conn.Open();
                            SqlTransaction ts = conn.BeginTransaction("pur"); //事务的名称
                            SqlCommand cmd = new SqlCommand("select * from 采购记录采购单主表 where 1<>1", conn, ts);
                            SqlCommand cmd1 = new SqlCommand("select * from 单据审核申请表 where 1<>1", conn, ts);
                            SqlCommand cmd2 = new SqlCommand("select * from 采购记录采购单明细表 where 1<>1", conn, ts);
                            SqlCommand cmd3 = new SqlCommand("select * from 主计划计划通知单明细 where 1<>1", conn, ts);
                            try
                            {

                                da = new SqlDataAdapter(cmd);
                                new SqlCommandBuilder(da);
                                da.Update(dt_撤销);
                                da = new SqlDataAdapter(cmd1);
                                new SqlCommandBuilder(da);
                                da.Update(dt_审核申请);
                                da = new SqlDataAdapter(cmd2);
                                new SqlCommandBuilder(da);
                                da.Update(dt_撤销子);
                                da = new SqlDataAdapter(cmd3);
                                new SqlCommandBuilder(da);
                                da.Update(dt_计划通知明细);
                                ts.Commit();
                            }
                            catch
                            {
                                ts.Rollback();
                            }
                            MessageBox.Show("弃审成功");
                            barLargeButtonItem1.Enabled =false;
                            CPublic.UIcontrol.ClosePage();
                            ERPpurchase.frm采购单明细 fm = new frm采购单明细(dt_撤销.Rows[0]);
                            CPublic.UIcontrol.AddNewPage(fm, "采购明细");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {


            try

            {

                DataTable dt_dy = (DataTable)this.gcm.DataSource;

                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
                Type outerForm = outerAsm.GetType("ERPreport.采购合同", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统

                object[] drr = new object[5];
                drr[0] = drM;
                drr[1] = dt_dy;
                drr[2] = barEditItem5.EditValue.ToString();
                ERPorg.Corg cg = new ERPorg.Corg();
                string str_含税 = Math.Round(Convert.ToDecimal(txt_shuihouje.Text), 2, MidpointRounding.AwayFromZero).ToString();
                string str_不含税 = Math.Round(Convert.ToDecimal(txt_weishuije.Text), 2, MidpointRounding.AwayFromZero).ToString();

                drr[3] = cg.NumToChinese(str_含税);//含税金额
                drr[4] = cg.NumToChinese(str_不含税); //不含税
                //   drr[2] = dr["出入库申请单号"].ToString();
                Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                //  UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
                ui.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 历史采购价ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gvm.GetDataRow(gvm.FocusedRowHandle);
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, "ERPStock.dll"));//dr["dll全路径"] = "动态载入dll.dll"
                Type outerForm = outerAsm.GetType("ERPStock.fm空窗体", false);//动态载入dll.UI动态载入窗体
                Form fm = (Form)Activator.CreateInstance(outerForm);

                Type outerui = outerAsm.GetType("ERPStock.UI过往采购单价查询", false);//动态载入dll.UI动态载入窗体
                object[] r = new object[1];
                r[0] = dr["物料编码"].ToString();
                UserControl ui = Activator.CreateInstance(outerui, r) as UserControl;

                fm.Controls.Add(ui);
                ui.Dock = DockStyle.Fill;
                fm.Text = "历史采购价";
                fm.Size = new System.Drawing.Size(1200, 550);
                fm.StartPosition = FormStartPosition.CenterScreen;
                fm.ShowDialog();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
