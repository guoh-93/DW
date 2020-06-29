using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace StockCore
{
    public partial class ui料出库 : UserControl
    {
        public ui料出库()
        {
            InitializeComponent();
            fun_物料下拉框();
        }


        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dtM = null;
        DataTable dtP = null;
        DataRow drM = null;
        DataTable dt_物料;
        DataTable dt_仓库;
        DataTable dt_仓库号;
        DataTable dt_人员;
        DataTable dt_代办;

        DataRow dr_出库申请 = null;
        DataTable dt_出库申请 = null;
        string sql_ck = "";
        string cfgfilepath = "";
        #endregion

        #region 自用类


        public ui料出库(DataRow dr)
        {
            InitializeComponent();
            drM = dr;
            fun_物料下拉框();
            cb_出库类型.EditValue = drM["出库类型"];
        }






        private void fun_仓库()
        {
            dt_仓库号 = new DataTable();
            string sql4 = "select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别' and 布尔字段5 = 1";
            SqlDataAdapter da = new SqlDataAdapter(sql4, strconn);
            da.Fill(dt_仓库号);
            repositoryItemSearchLookUpEdit2.DataSource = dt_仓库号;
            repositoryItemSearchLookUpEdit2.DisplayMember = "仓库号";
            repositoryItemSearchLookUpEdit2.ValueMember = "仓库号";
        }

#pragma warning disable IDE1006 // 命名样式
        private void comboBoxEdit1_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (cb_出库类型.EditValue == null) cb_出库类型.EditValue = "";
                if (cb_出库类型.EditValue.ToString() == "其他出库")
                {
                    lab_人员.Text = "领用人员";
                    lab_人员ID.Text = "领用人员编号";
                    barLargeButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    gv.Columns["归还数量"].Visible = false;
                }
                else
                {
                    lab_人员.Text = "借用人员";
                    lab_人员ID.Text = "借用人员编号";
                    if (cb_出库类型.EditValue.ToString() == "借用出库")
                    {
                        barLargeButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        gv.Columns["归还数量"].Visible = true;
                    }
                }
            }
            catch { }
        }
        #endregion

        #region 新增出库-方法


        private void fun_载入代办()
#pragma warning restore IDE1006 // 命名样式
        {
            sql_ck = string.Format("select * from 人员仓库对应表 where 工号='{0}'", CPublic.Var.LocalUserID);
            DataTable dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_ck, strconn);
            sql_ck = "and a.仓库号  in( ";
            // string sql_左 = "";
            string sql = "";

            if (CPublic.Var.LocalUserTeam == "管理员权限" || CPublic.Var.LocalUserID == "910173" || CPublic.Var.LocalUserID == "910055")
            {
                //select * from 其他出入库申请主表 where 生效 = 1 and (完成=0 or 完成 is null) and (作废 = 0 or 作废 is null) and 申请类型 = '材料出库' /*and 审核 = 1*/
                sql = @"select 其他出入库申请主表.* from 其他出入库申请主表
                        where 生效 = 1  and  完成 = 0 and 作废 = 0   and 申请类型='材料出库' and 
                      出入库申请单号 in( select 出入库申请单号  from 其他出入库申请子表 a,基础数据物料信息表 b where 完成=0 and 作废=0   and     
                         b.物料编码=a.物料编码  group by 出入库申请单号 ) and 审核 = 1";

            }
            else if (dt_仓库.Rows.Count > 0)
            {
                foreach (DataRow dr in dt_仓库.Rows)
                {
                    sql_ck = sql_ck + string.Format("'{0}',", dr["仓库号"]);

                }
                sql_ck = sql_ck.Substring(0, sql_ck.Length - 1) + ")";

                sql = string.Format(@"select 其他出入库申请主表.* from 其他出入库申请主表
                        where 生效 = 1 and  完成 = 0 and 作废 = 0   and 申请类型='材料出库' and 
                      出入库申请单号 in( select 出入库申请单号  from 其他出入库申请子表 a,基础数据物料信息表 b where 完成=0 and 作废=0   and     
                         b.物料编码=a.物料编码 {0} group by 出入库申请单号 ) and 审核 = 1", sql_ck);

            }
            else
            {

                throw new Exception("未维护所管仓库信息");
            }

            //string sql = "select * from 其他出入库申请主表 where 生效 = 1 and 完成 = 0   and 申请类型 = '其它出库'";

            dt_代办 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_代办);
            DataTable dt_采购;
            for (int i = dt_代办.Rows.Count - 1; i >= 0; i--)
            {
                if (dt_代办.Rows[i]["原因分类"].ToString() == "委外加工")
                {
                    string sql_采购单 = string.Format("select * from 采购记录采购单主表 where 采购单号 = '{0}'", dt_代办.Rows[i]["备注"].ToString().Trim());
                    dt_采购 = CZMaster.MasterSQL.Get_DataTable(sql_采购单, strconn);
                    if (dt_采购.Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(dt_采购.Rows[0]["审核"]) == false)
                        {
                            dt_代办.Rows.Remove(dt_代办.Rows[i]);
                        }
                    }
                }
            }
            gc_代办.DataSource = dt_代办;
        }

        private void fun_判断出库申请()

        {
            DateTime t = CPublic.Var.getDatetime();
            // DateTime t= Convert.ToDateTime("2020-4-30 19:00:00");
            string sql = string.Format("select * from 其他出入库申请子表 where 出入库申请单号 = '{0}'  and 完成=0", drM["出入库申请单号"]);
            //未完成的
            dt_出库申请 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_出库申请);


            //DataView dv = new DataView(dtP);
            //dv.RowFilter = "数量确认=1";
            //if (dv.Count == dt_出库申请.Rows.Count)
            //{
            //    DataRow dr_申请 = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);
            //    dr_申请["完成"] = true;
            //    dr_申请["完成日期"] = t;
            //    drM["完成"] = true;
            //    drM["完成日期"] = t;

            //}
            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                DataRow[] ds = dt_出库申请.Select(string.Format("出入库申请明细号 = '{0}'", dr["出入库申请明细号"]));
                ds[0]["已完成数量"] = Convert.ToDecimal(ds[0]["已完成数量"]) + Convert.ToDecimal(dr["数量"]);
                if (Convert.ToBoolean(dr["数量确认"]) && Convert.ToDecimal(dr["数量"]) == Convert.ToDecimal(dr["申请数量"]) - Convert.ToDecimal(dr["已出数量"]))
                {

                    ds[0]["完成"] = true;
                    ds[0]["完成日期"] = t;
                }

            }
            DataView dv = new DataView(dt_出库申请);



            dv.RowFilter = "完成=0 or 完成 is null";


            if (dv.Count == 0)
            {

                if (dtP.Rows.Count == dt_出库申请.Rows.Count)
                {
                    DataRow dr_申请 = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);
                    dr_申请["完成"] = true;
                    dr_申请["完成日期"] = t;
                    drM["完成"] = true;
                    drM["完成日期"] = t;
                }
            }


        }



        private void fun_人员()

        {
            string sql = string.Format(@"select 员工号,姓名 from 人事基础员工表 where 在职状态 = '在职'");
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            dt_人员 = new DataTable();
            da.Fill(dt_人员);
            txt_人员ID.Properties.DataSource = dt_人员;
            txt_人员ID.Properties.DisplayMember = "员工号";
            txt_人员ID.Properties.ValueMember = "员工号";
        }


        private void txt_人员ID_EditValueChanged(object sender, EventArgs e)

        {
            if (txt_人员ID.EditValue != null && txt_人员ID.EditValue.ToString() == "")
            {
                txt_人员.Text = "";
            }
            else
            {
                DataRow[] ds = dt_人员.Select(string.Format("员工号 = '{0}'", txt_人员ID.EditValue));
                if (ds.Length > 0)
                {
                    txt_人员.Text = ds[0]["姓名"].ToString();
                }
                else
                {
                    txt_人员.Text = "";

                }
            }
        }

        private void fun_载入主表明细()
#pragma warning restore IDE1006 // 命名样式
        {
            if (drM == null)
            {
                string sql = "select * from 其他出库主表 where 1<>1";
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                drM = dtM.NewRow();
                dtM.Rows.Add(drM);
                sql = @"select ckz.*,库存总数,a.仓库号,a.仓库名称,a.货架描述,0.0 已出数量,0.0 申请数量 from 其他出库子表 ckz
                              left join 仓库物料数量表 a on a.物料编码=ckz.物料编码
                               where  1<>1";
                dtP = new DataTable();
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtP);
            }
            else //else 里面已经弃用
            {
                string sql = string.Format("select * from 其他出库主表 where 其他出库单号 = '{0}'", drM["其他出库单号"].ToString());
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                drM = dtM.Rows[0];
                dataBindHelper1.DataFormDR(drM);
                if (drM["出库类型"].ToString() == "借用出库")
                {
                    txt_人员ID.EditValue = drM["借用人员编号"];
                    txt_人员.Text = drM["借用人员"].ToString();
                }
                else
                {
                    txt_人员ID.EditValue = drM["领用人员编号"];
                    txt_人员.Text = drM["领用人员"].ToString();
                }
                string sql2 = string.Format(@"select 其他出库子表.*,a.库存总数,a.仓库号,a.仓库名称 from 其他出库子表 
                left join 仓库物料数量表 a on 其他出库子表.物料编码 = a.物料编码
                where 其他出库单号 = '{0}'", drM["其他出库单号"].ToString());
                dtP = new DataTable();
                SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
                da2.Fill(dtP);
            }
            dtP.Columns.Add("数量确认", typeof(bool));
            dtP.Columns.Add("输入数量", typeof(decimal));
            dtP.Columns.Add("已完成数量", typeof(decimal));


            //dtP.ColumnChanged += dtP_ColumnChanged;
        }

        public DataTable fun_库存(int i_正负, DataTable T)
#pragma warning restore IDE1006 // 命名样式
        {
            DataTable dt = new DataTable();
            foreach (DataRow dr in T.Rows)
            {
                if (dr["数量确认"].Equals(true))
                {
                    string sql = string.Format("select * from 仓库物料数量表 where 物料编码='{0}' and 仓库号='{1}'", dr["物料编码"].ToString(), dr["仓库号"]);
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                    {
                        da.Fill(dt);
                    }
                    DataRow[] x = dt.Select(string.Format("物料编码='{0}' and 仓库号='{1}'", dr["物料编码"].ToString(), dr["仓库号"]));
                    x[0]["库存总数"] = Convert.ToDecimal(x[0]["库存总数"]) + i_正负 * Convert.ToDecimal(dr["数量"].ToString());
                     x[0]["出入库时间"] = CPublic.Var.getDatetime();
                }
            }

            return dt;
        }


        private void fun_保存主表明细(Boolean bl)
#pragma warning restore IDE1006 // 命名样式
        {
            DateTime t = CPublic.Var.getDatetime();
              //DateTime t = Convert.ToDateTime("2020-4-30 19:00:00");
            try
            {

                if (drM["GUID"].ToString() == "")
                {
                    drM["GUID"] = System.Guid.NewGuid();
                    txt_出库单号.Text = string.Format("LS{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                   t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("LS", t.Year, t.Month).ToString("0000"));
                    drM["其他出库单号"] = txt_出库单号.Text;
                    drM["创建日期"] = t;
                }
                drM["操作人员编号"] = CPublic.Var.LocalUserID;
                drM["操作人员"] = CPublic.Var.localUserName;
                drM["出库仓库"] = "";

                drM["领用人员"] = txt_人员.Text;
                drM["领用人员编号"] = txt_人员ID.EditValue;

                if (bl == true)
                {
                    drM["生效"] = true;
                    drM["生效人员编号"] = CPublic.Var.LocalUserID;
                    drM["生效日期"] = t;
                }
                dataBindHelper1.DataToDR(drM);
            }
            catch (Exception ex)
            {
                throw new Exception("主表保存出错" + ex.Message);
            }

            try
            {

                int i = 1;
                foreach (DataRow r in dtP.Rows)
                {
                    if (r["数量确认"].Equals(true))
                    {
                        if (r.RowState == DataRowState.Deleted) continue;
                        if (r["GUID"].ToString() == "")
                        {
                            r["GUID"] = System.Guid.NewGuid();
                            r["其他出库单号"] = drM["其他出库单号"];
                            r["其他出库明细号"] = drM["其他出库单号"].ToString() + "-" + i.ToString("00");
                            r["POS"] = i++;
                        }
                        if (bl == true)
                        {
                            r["生效"] = true;
                            r["生效人员编号"] = CPublic.Var.LocalUserID;
                            r["生效日期"] = t;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("明细保存出错" + ex.Message);
            }
            DataTable dt_出入明细 = fun_保存记录到出入库明细();
            fun_判断出库申请();
            DataTable t_stockNum = dtP.Copy();
            //6-9 dt中数量已变为 申请数量  ，输入数量 才是实际操作数量 

            //dtp中未勾选的已在check时remove掉了
            DataTable dt_库存 = ERPorg.Corg.fun_库存(-1, t_stockNum);
            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("生效");
            string sql1 = "select * from 其他出库主表 where 1<>1";
            SqlCommand cmd1 = new SqlCommand(sql1, conn, ts);
            SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
            new SqlCommandBuilder(da1);
            string sql2 = "select * from 其他出库子表 where 1<>1";
            SqlCommand cmd2 = new SqlCommand(sql2, conn, ts);
            SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
            new SqlCommandBuilder(da2);
            string sql3 = "select * from 其他出入库申请主表 where 1<>1";
            SqlCommand cmd3 = new SqlCommand(sql3, conn, ts);
            SqlDataAdapter da3 = new SqlDataAdapter(cmd3);
            new SqlCommandBuilder(da3);
            string sql4 = "select * from 其他出入库申请子表 where 1<>1";
            SqlCommand cmd4 = new SqlCommand(sql4, conn, ts);
            SqlDataAdapter da4 = new SqlDataAdapter(cmd4);
            new SqlCommandBuilder(da4);
            string sql5 = "select * from 仓库出入库明细表 where 1<>1";
            SqlCommand cmd5 = new SqlCommand(sql5, conn, ts);
            SqlDataAdapter da5 = new SqlDataAdapter(cmd5);
            new SqlCommandBuilder(da5);
            string sql6 = "select * from 仓库物料数量表 where 1<>1";
            SqlCommand cmd6 = new SqlCommand(sql6, conn, ts);
            SqlDataAdapter da6 = new SqlDataAdapter(cmd6);
            new SqlCommandBuilder(da6);
            try
            {
                da1.Update(dtM);
                da2.Update(dtP);
                da3.Update(dt_代办);
                da4.Update(dt_出库申请);
                da5.Update(dt_出入明细);
                da6.Update(dt_库存);
                ts.Commit();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw ex;
            }
        }


        private void dtP_ColumnChanged(object sender, DataColumnChangeEventArgs e)

        {

        }


        private void fun_物料下拉框()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = @"select base.物料编码,base.物料名称,base.规格型号,
            base.图纸编号,a.库存总数,a.仓库号,a.仓库名称 from 基础数据物料信息表 base
            left join 仓库物料数量表 a on base.物料编码 = a.物料编码";
            dt_物料 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_物料);

            repositoryItemSearchLookUpEdit1.DataSource = dt_物料;
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
        }


        private DataTable fun_保存记录到出入库明细()

        {
            try
            {
                string sql = "select * from 仓库出入库明细表 where 1<>1";
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
               DateTime t = CPublic.Var.getDatetime();
                //DateTime t = Convert.ToDateTime("2020-4-30 19:00:00");
                foreach (DataRow r in dtP.Rows)
                {
                    if (r["数量确认"].ToString().ToLower() == "true")
                    {
                        DataRow dr = dt.NewRow();
                        dr["GUID"] = System.Guid.NewGuid();
                        if (textBox1.Text == "入库倒冲")
                        {
                            dr["明细类型"] = "入库倒冲";
                        }
                        else
                        {
                            dr["明细类型"] = cb_出库类型.EditValue;

                        }
                        dr["单号"] = r["其他出库单号"].ToString();
                        dr["物料编码"] = r["物料编码"].ToString();
                        dr["物料名称"] = r["物料名称"].ToString();
                        dr["明细号"] = r["其他出库明细号"].ToString();

                        dr["相关单号"] = r["出入库申请单号"].ToString();
                        dr["仓库号"] = r["仓库号"].ToString();
                        dr["仓库名称"] = r["仓库名称"].ToString();
                        dr["出库入库"] = "出库";

                        string sql_1 = string.Format("select * from 人事基础员工表 where 员工号='{0}'", txt_人员ID.EditValue);
                        DataTable dt_xg = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);

                        dr["相关单位"] = dt_xg.Rows[0]["课室"];

                        dr["数量"] = (Decimal)0;
                        dr["标准数量"] = (Decimal)0;
                        dr["实效数量"] = -Convert.ToDecimal(r["数量"].ToString()); //6-9 修改
                        dr["实效时间"] = t;
                        dr["出入库时间"] = t;
                        dr["仓库人"] = CPublic.Var.localUserName;


                        dt.Rows.Add(dr);
                    }
                }
                return dt;
                //new SqlCommandBuilder(da);
                //da.Update(dt);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm其他出库_fun_保存出入库明细");
                throw ex;
            }
        }



        private void fun_check()

        {

            if (cb_出库类型.Text == "")
            {
                throw new Exception("请选择出库类型");

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

                    throw new Exception("请正确输入数量格式");
                }
                if (Convert.ToDecimal(dr["数量"]) <= 0)
                {
                    throw new Exception("数量不能小于0");
                }

            }

        }
        #endregion


        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                drM = null;
                dtP.Clear();
                txt_人员ID.EditValue = null;
                txt_人员.Text = "";
                txt_出库单号.Text = "";
                cb_出库类型.EditValue = "";
                textBox1.Text = "";
                ui料出库_Load(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                date_1.EditValue = CPublic.Var.getDatetime();
                drM = null;
                txt_出库单号.Text = "";
                txt_备注.Text = "";
                txt_出库仓库.Text = "";
                txt_人员ID.EditValue = "";
                txt_人员.Text = "";
                cb_出库类型.SelectedIndex = -1;
                barLargeButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                gv.Columns["归还数量"].Visible = false;

                fun_载入主表明细();
                gc.DataSource = dtP;
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
                if (MessageBox.Show(string.Format("确认保存？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    gv.CloseEditor();
                    this.BindingContext[dtP].EndCurrentEdit();
                    fun_保存主表明细(false);
                    MessageBox.Show("保存成功");
                }
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
                gv.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
             


                DataRow rr = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);
                //2020-4-29检查单据状态是否是已审核  因为增加了 单据审批流 弃审  可能单据状态有问题
                string x = $"select  * from  其他出入库申请主表 where 审核=1 and 作废=0 and 完成=0 and 出入库申请单号='{rr["出入库申请单号"].ToString()}'";
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(x,strconn);
                if(temp.Rows.Count==0)
                {
                    throw new Exception("单据状态已更改,刷新后再试");
                }


                if (cb_出库类型.EditValue == null || cb_出库类型.EditValue.ToString() == "")
                {
                    throw new Exception("请选择出库类型");
                }
                for (int i = 0; i < dtP.Rows.Count; i++)
                {
                    if (dtP.Rows[i]["数量确认"].Equals(false))
                    {
                        dtP.Rows.Remove(dtP.Rows[i]);
                        i--;
                    }
                }
                if (dtP.Rows.Count == 0)
                {
                    gv_代办_RowCellClick(null, null);
                    throw new Exception("未选择明细");
                }
                string s_业务单号 = "";
                if (Convert.ToBoolean(rr["红字回冲"]))
                {
                    s_业务单号 = rr["业务单号"].ToString();
                }
                foreach (DataRow dr in dtP.Rows)
                {
                    if (Convert.ToBoolean(dr["数量确认"]))
                    {

                        string sql = string.Format("select * from 仓库物料数量表 where  物料编码='{0}' and 仓库号='{1}' ", dr["物料编码"].ToString(), dr["仓库号"].ToString());
                        DataTable xt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                        if (xt.Rows.Count == 0)
                        {
                            if (Convert.ToDecimal(dr["数量"])> 0)
                            {
                                throw new Exception("库存总数不足！");
                            }

                        }
                        else
                        {
                            //这里若为红字 数量为 负 库存不可能小于0  因此只需要判断 数量大于0 的情况 
                            if (Convert.ToDecimal(xt.Rows[0]["库存总数"]) < Convert.ToDecimal(dr["数量"]))
                            {
                                throw new Exception("库存不足");
                            }

                        }
                        if (Convert.ToDecimal(dr["数量"]) > Convert.ToDecimal(dr["申请数量"]) - Convert.ToDecimal(dr["已出数量"]))
                        {
                            throw new Exception("输入数量大于可出数量");
                        }
                        if (Convert.ToDecimal(dr["数量"]) > Convert.ToDecimal(dr["申请数量"]) - Convert.ToDecimal(dr["已完成数量"]))
                        {
                            throw new Exception("输入数量大于可出数量");
                        }
                        if (s_业务单号 != "")
                        {
                            sql = string.Format("select  * from 其他出库子表 where 出入库申请单号='{0}' and 物料编码='{1}'", s_业务单号, dr["物料编码"].ToString());
                            temp = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                            if (temp.Rows.Count > 0)
                            {
                                dr["结算单价"] = temp.Rows[0]["结算单价"];
                                //dr["存货核算标记"] = 1;
                                //2019-10-9  放在这边 U8里面的历史单据 在现有数据库找不到 这里不会走到 导致 红字回冲的 单据 存货核算标记没有赋值为1
                            }
                            dr["存货核算标记"] = 1;
                        }

                    }
                    else
                    {
                        dtP.Rows.Remove(dr);
                    }
                    //if (dr["数量确认"].ToString().ToLower() == "true")
                    //{
                    //    if (Convert.ToDecimal(dr["数量"]) > Convert.ToDecimal(dr["库存总数"]))
                    //    {
                    //        throw new Exception("库存不足");

                    //    }
                    //    continue;
                    //}
                    //else
                    //{
                    //    dtP.Rows.Remove(dr);
                    //}
                }
                fun_保存主表明细(true);
                //if (drM["出库类型"].ToString() == "其他出库")
                //{


                //刷新仓库库存
                foreach (DataRow r in dtP.Rows)
                {
                    if (Convert.ToBoolean(r["数量确认"]))
                    {
                        //  2016-11-11 出库 刷新库存
                        // StockCore.StockCorer.fun_刷新库存(r["物料编码"].ToString(), Convert.ToDecimal(r["数量"]), -1);

                        StockCore.StockCorer.fun_物料数量_实际数量(r["物料编码"].ToString(), r["仓库号"].ToString(), true);
                    }
                }
                //}
                MessageBox.Show("生效成功");
                barLargeButtonItem1_ItemClick(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);
                DataTable dt_dy = dtP.Copy();
                string sql = string.Format(@"select 其他出库单号 as 编号,申请类型,a.操作人员,部门,a.备注,b.生效日期 from 其他出入库申请主表  a
                        left join 人事基础员工表 on  a.操作人员编号=人事基础员工表.员工号 
                        left join 其他出库主表 b on a.出入库申请单号=b.出入库申请单号 where    a.出入库申请单号='{0}'  ", dr["出入库申请单号"].ToString());
                System.Data.DataTable dt = new System.Data.DataTable();
                new SqlDataAdapter(sql, CPublic.Var.strConn).Fill(dt);
                dt_dy.Columns.Add("计量单位", typeof(string));
                foreach (DataRow r in dt_dy.Rows)
                {

                    string sql_1 = string.Format(@"select 计量单位,kc.仓库名称,库存总数,kc.货架描述 from 基础数据物料信息表 base,仓库物料数量表 kc
                                        where base.物料编码=kc.物料编码 and  kc.物料编码='{0}'", r["物料编码"].ToString());
                    System.Data.DataTable dt_1 = new System.Data.DataTable();
                    dt_1 = CZMaster.MasterSQL.Get_DataTable(sql_1, CPublic.Var.strConn);
                    if (dt_1.Rows.Count > 0)
                    {
                        r["计量单位"] = dt_1.Rows[0]["计量单位"].ToString();
                        r["仓库名称"] = dt_1.Rows[0]["仓库名称"].ToString();
                        r["货架描述"] = dt_1.Rows[0]["货架描述"].ToString();
                    }




                }

                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
                Type outerForm = outerAsm.GetType("ERPreport.frm其他出库打印", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统

                object[] drr = new object[3];
                drr[0] = dt;
                drr[1] = dt_dy;
                drr[2] = "材料出库";
                Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                ui.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
                    gc_代办.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);

            }
        }

        private void gv_代办_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gv_代办_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                dr_出库申请 = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);
                if (dr_出库申请 == null) return;
                txt_人员ID.EditValue = dr_出库申请["操作人员编号"].ToString();
                drM["出入库申请单号"] = dr_出库申请["出入库申请单号"];
                cb_出库类型.EditValue = "";
                txt_备注.Text = "";
                txt_出库单号.Text = "";
                textBox1.Text = dr_出库申请["原因分类"].ToString();
                cb_出库类型.EditValue = dr_出库申请["申请类型"].ToString();

                dtP.Clear(); gc.DataSource = dtP;


                if (CPublic.Var.LocalUserTeam == "管理员权限" || CPublic.Var.LocalUserID == "910173" || CPublic.Var.LocalUserID == "910055")
                {
                    sql_ck = "";
                }

                string sql = "";
                if (dr_出库申请["原因分类"].ToString() == "委外加工")
                {
                    sql = $@"select a.*,c.规格型号,库存总数,isnull(已出数量,0)已出数量   from 其他出入库申请子表 a
                            left join 仓库物料数量表 c on a.物料编码=c.物料编码 and a.仓库号=c.仓库号
                            left join (select  备注 ,物料编码,sum(数量)已出数量  from 其他出库子表 group by 备注,物料编码)yc 
                            on yc.备注=a.备注 and yc.物料编码=a.物料编码  
                     where a.作废=0 and a.完成=0  and  出入库申请单号 = '{dr_出库申请["出入库申请单号"].ToString()}'{sql_ck} order by a.出入库申请明细号 ";
                }
                else
                {
                    sql = $@"select a.*,c.规格型号,库存总数,isnull(已出数量,0)已出数量   from 其他出入库申请子表 a
                            left join 仓库物料数量表 c on a.物料编码=c.物料编码 and a.仓库号=c.仓库号
                            left join (select  出入库申请明细号 ,物料编码,sum(数量)已出数量  from 其他出库子表 group by 出入库申请明细号,物料编码)yc 
                            on yc.出入库申请明细号=a.出入库申请明细号  where a.作废=0 and a.完成=0  and  出入库申请单号 = '{dr_出库申请["出入库申请单号"].ToString()}'{sql_ck} order by a.出入库申请明细号 ";
                }
                dt_出库申请 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_出库申请);
                foreach (DataRow r in dt_出库申请.Rows)
                {
                    //if (Convert.ToBoolean(r["完成"]) )
                    //{
                    //    continue;
                    //}
                    DataRow rr = dtP.NewRow();
                    dtP.Rows.Add(rr);
                    rr["库存总数"] = r["库存总数"];
                    rr["已完成数量"] = r["已完成数量"];

                    rr["物料编码"] = r["物料编码"];
                    rr["物料名称"] = r["物料名称"];
                    rr["仓库号"] = r["仓库号"];
                    rr["仓库名称"] = r["仓库名称"];
                    rr["货架描述"] = r["货架描述"];
                    rr["规格型号"] = r["规格型号"];
                    rr["数量"] = Convert.ToDecimal(r["数量"]) - Convert.ToDecimal(r["已出数量"]);
                    rr["已出数量"] = r["已出数量"];
                    rr["申请数量"] = Convert.ToDecimal(r["数量"]);
                    rr["出入库申请单号"] = r["出入库申请单号"];
                    rr["出入库申请明细号"] = r["出入库申请明细号"];
                    rr["数量确认"] = false;
                    rr["备注"] = r["备注"];

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
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
                        // dr["有效总数"] = 0;
                        dr["货架描述"] = "";//19-9-17解决货架更新
                    }
                    else
                    {
                        dr["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
                        //  dr["有效总数"] = dt_物料数量.Rows[0]["有效总数"];
                        dr["货架描述"] = dt_物料数量.Rows[0]["货架描述"];//19-9-17解决货架更新
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (gv.GetRowCellValue(e.RowHandle, "数量") != null && gv.GetRowCellValue(e.RowHandle, "数量").ToString() != "" &&
                   gv.GetRowCellValue(e.RowHandle, "库存总数") != null && gv.GetRowCellValue(e.RowHandle, "库存总数").ToString() != "")
            {
                decimal dec = Convert.ToDecimal(gv.GetRowCellValue(e.RowHandle, "数量"));
                decimal dec_kc = Convert.ToDecimal(gv.GetRowCellValue(e.RowHandle, "库存总数"));
                if (dec > dec_kc)
                {
                    e.Appearance.BackColor = Color.Pink;

                }
            }
        }

        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void ui料出库_Load(object sender, EventArgs e)
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
                x.UserLayout(this.panel5, this.Name, cfgfilepath);
                date_1.EditValue = CPublic.Var.getDatetime();
                //date_1.EditValue =  Convert.ToDateTime("2020-4-30 19:00:00");
                cb_出库类型.Text = "材料出库";
                fun_人员();
                fun_仓库();
                fun_载入代办();
                fun_载入主表明细();



                gc.DataSource = dtP;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txt_人员ID_EditValueChanged_1(object sender, EventArgs e)
        {
            if (txt_人员ID.EditValue != null && txt_人员ID.EditValue.ToString() == "")
            {
                txt_人员.Text = "";
            }
            else
            {
                DataRow[] ds = dt_人员.Select(string.Format("员工号 = '{0}'", txt_人员ID.EditValue));
                if (ds.Length > 0)
                {
                    txt_人员.Text = ds[0]["姓名"].ToString();
                }
                else
                {
                    txt_人员.Text = "";

                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            foreach (DataRow dr in dtP.Rows)
            {
                dr["数量确认"] = true;

            }

        }

        private void gv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv.GetFocusedRowCellValue(gv.FocusedColumn));
                e.Handled = true;
            }
        }

        private void gv_代办_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                DataRow dr_出库申请 = gv_代办.GetDataRow(e.FocusedRowHandle);
                if (dr_出库申请 != null)
                {
                    dr_出库申请 = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);
                    txt_人员ID.EditValue = dr_出库申请["操作人员编号"].ToString();
                    drM["出入库申请单号"] = dr_出库申请["出入库申请单号"];
                    cb_出库类型.EditValue = "";
                    txt_备注.Text = "";
                    txt_出库单号.Text = "";
                    textBox1.Text = dr_出库申请["原因分类"].ToString();
                    cb_出库类型.EditValue = dr_出库申请["申请类型"].ToString();

                    dtP.Clear(); gc.DataSource = dtP;


                    if (CPublic.Var.LocalUserTeam == "管理员权限" || CPublic.Var.LocalUserID == "910173" || CPublic.Var.LocalUserID == "910055")
                    {
                        sql_ck = "";
                    }
                    string sql = string.Format(@"select a.*,c.规格型号,库存总数,isnull(已出数量,0)已出数量   from 其他出入库申请子表 a
 left join 仓库物料数量表 c on a.物料编码=c.物料编码 and a.仓库号=c.仓库号
 left join (select  出入库申请明细号 ,物料编码,sum(数量)已出数量  from 其他出库子表 group by 出入库申请明细号,物料编码)yc 
on yc.出入库申请明细号=a.出入库申请明细号  where a.作废=0 and a.完成=0  and  出入库申请单号 = '{0}'{1} order by a.出入库申请明细号 ", dr_出库申请["出入库申请单号"], sql_ck);
                    dt_出库申请 = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt_出库申请);
                    foreach (DataRow r in dt_出库申请.Rows)
                    {
                        //if (Convert.ToBoolean(r["完成"]) )
                        //{
                        //    continue;
                        //}
                        DataRow rr = dtP.NewRow();
                        dtP.Rows.Add(rr);
                        rr["库存总数"] = r["库存总数"];
                        rr["已完成数量"] = r["已完成数量"];

                        rr["物料编码"] = r["物料编码"];
                        rr["物料名称"] = r["物料名称"];
                        rr["仓库号"] = r["仓库号"];
                        rr["仓库名称"] = r["仓库名称"];
                        rr["货架描述"] = r["货架描述"];
                        rr["规格型号"] = r["规格型号"];
                        rr["数量"] = Convert.ToDecimal(r["数量"]) - Convert.ToDecimal(r["已出数量"]);
                        rr["已出数量"] = r["已出数量"];
                        rr["申请数量"] = Convert.ToDecimal(r["数量"]);
                        rr["出入库申请单号"] = r["出入库申请单号"];
                        rr["出入库申请明细号"] = r["出入库申请明细号"];
                        rr["数量确认"] = false;
                        rr["备注"] = r["备注"];

                    }
                }

            }
            catch
            {


            }
        }
    }
}
