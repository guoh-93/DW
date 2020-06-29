using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ERPSale
{
    public partial class frm销售记录销售开票详细界面_视图 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string str_开票单号 = "";
        DataTable dtP;
        DataTable dt_mx;
        DataRow drM;
        /// <summary>
        /// 判断是否变更产品单价 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="dr"></param>
        bool f_price = false;
        /// <summary>
        /// 客户产品单价表中 新增或修改   和 修改日志
        /// table[0] 新增
        /// table[1] 修改
        /// table[2] 修改日志
        /// </summary>

        DataSet ds = new DataSet();
        public frm销售记录销售开票详细界面_视图(string str, DataRow dr)
        {
            InitializeComponent();
            str_开票单号 = str;
            drM = dr;
        }

        private void frm销售记录销售开票详细界面_视图_Load(object sender, EventArgs e)
        {
            try
            {
                gridColumn1.OptionsColumn.AllowEdit = false;
                gridColumn2.OptionsColumn.AllowEdit = false;

                if (CPublic.Var.LocalUserTeam == "财务部权限" || CPublic.Var.LocalUserTeam == "管理员权限" || CPublic.Var.LocalUserID == "admin")
                {
                    gridColumn1.OptionsColumn.AllowEdit = true;
                    gridColumn2.OptionsColumn.AllowEdit = true;
                    gridColumn3.Visible = true;
                    gridColumn13.Visible = true;
                    gridColumn3.OptionsColumn.AllowEdit = true;
                    gridColumn13.OptionsColumn.AllowEdit = true;
                    panel3.Visible = true;
                    barLargeButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                }
                string sql = "select 属性值 from 基础数据基础属性表 where 属性类别='币种' order by POS";
                DataTable dt_币种 = new DataTable();
                dt_币种 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                v.Items.Clear();
                foreach (DataRow dr in dt_币种.Rows)
                {
                    v.Items.Add(dr["属性值"]);
                }


                dataBindHelper1.DataFormDR(drM);



                if (str_开票单号 != "")
                {
                    sql = string.Format("select * from 销售记录销售开票明细表 where  销售开票通知单号='{0}'", str_开票单号);
                    dt_mx = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt_mx);
                }


                if (decimal.Parse(drM["汇率"].ToString()) != 0)
                {
                    textBox2.Text = drM["汇率"].ToString();

                }


                if (drM["币种"].ToString() != "")
                {
                    v.Text = drM["币种"].ToString();
                    if (drM["类别"].ToString() == "")
                    {
                        if (drM["币种"].ToString() == "人民币")
                        {
                            comboBox1.Text = "国内";
                        }
                        else
                        {
                            comboBox1.Text = "国外";

                        }
                    }

                }
                fun_载入明细1();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void fun_载入明细1()
        {
            string sql = string.Format(@"select a.*  from 销售记录销售开票明细表 a  
                                where   销售开票通知单号= '{0}'", str_开票单号);
            dtP = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtP);
            //gc.DataSource = dtP;

            DateTime time = CPublic.Var.getDatetime();


            sql = string.Format("select * from 汇率维护表 where 年='{0}'and 月='{1}' and 币种='{2}' ", time.Year, time.Month, drM["币种"].ToString());
            DataTable dt_bz = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            if (dt_bz.Rows.Count > 0)
            {
                if (textBox2.Text == "")
                    dt_bz.Rows[0]["汇率"].ToString();
                decimal a = decimal.Parse(dt_bz.Rows[0]["汇率"].ToString());
                foreach (DataRow dr in dtP.Rows)
                {
                    if (dr["本币税后金额"].ToString() == "")
                        dr["本币税后金额"] = Math.Round(decimal.Parse(dr["开票税后金额"].ToString()) * a, 2, MidpointRounding.AwayFromZero);
                    if (Convert.ToDecimal(dr["税额"]) == 0)//19-9-24 税额后加的
                    {
                        dr["税额"] = Math.Round(Convert.ToDecimal(dr["开票税后金额"]) - Convert.ToDecimal(dr["开票税前金额"]), 2);
                    }
                }

            }
            else
            {
                throw new Exception("该币种当月未维护汇率");
            }
            gc.DataSource = dtP;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            // DataRow drM = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;
            DataTable dtm = (DataTable)this.gc.DataSource;
            Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
            Type outerForm = outerAsm.GetType("ERPreport.销售开票", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统

            object[] drr = new object[2];

            drr[0] = drM;
            drr[1] = dtm;
            //drr[2] = textBox1.Text.ToString();
            //   drr[2] = dr["出入库申请单号"].ToString();
            Form ui = Activator.CreateInstance(outerForm, drr) as Form;
            //  UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
            ui.ShowDialog();



        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        DataTable dt;
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                this.ActiveControl = null;
                fun_载入明细();
                //fun_check();



                if (txt_开票票号.Text == null)
                {
                    throw new Exception("请先填写开票单号");
                }
                //string sql_1 = string.Format("select * from [销售记录销售开票主表] where 开票票号='{0}'", txt_开票票号.Text.ToString().Trim());
                //DataTable dt_判断开票单是否存在 = new DataTable();
                //dt_判断开票单是否存在 = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);
                //if (dt_判断开票单是否存在.Rows.Count > 0)
                //{
                //    throw new Exception("开票号重复请重新确认");

                //}
                drM["开票票号"] = txt_开票票号.Text.ToString();
                string sql = string.Format("select * from 销售记录销售开票主表 where 销售开票通知单号 = '{0}'", str_开票单号);
                dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                if (Convert.ToBoolean(dt.Rows[0]["审核"]))
                {
                    throw new Exception("该单据已审核,无需再次审核");
                }
                new SqlCommandBuilder(da);
                DateTime t = CPublic.Var.getDatetime();
                dt.Rows[0]["开票票号"] = txt_开票票号.Text.ToString();
                dt.Rows[0]["审核"] = true;
                dt.Rows[0]["审核人员"] = CPublic.Var.localUserName;
                dt.Rows[0]["审核人员ID"] = CPublic.Var.LocalUserID;
                dt.Rows[0]["审核人员ID"] = CPublic.Var.LocalUserID;
                dt.Rows[0]["汇率"] = decimal.Parse(textBox2.Text);
                dt.Rows[0]["币种"] = v.Text.ToString();
                dt.Rows[0]["审核日期"] = t;
                dt.Rows[0]["生效"] = true;
                dt.Rows[0]["生效日期"] = t;
                //string sql2 = string.Format("select * from 销售记录销售开票明细表 where 销售开票通知单号 = '{0}'", str_开票单号);
                //SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
                //new SqlCommandBuilder(da2);
                decimal dec_税前总额 = 0;
                decimal dec_含税总额 = 0;

                foreach (DataRow r in dtP.Rows)
                {
                    r["开票票号"] = txt_开票票号.Text.ToString();
                    r["开票明细号"] = txt_开票票号.Text.ToString() + "-" + Convert.ToInt32(r["POS"]).ToString("00");
                    r["生效"] = true;

                    // r["本币税后金额"] = true;
                    r["生效日期"] = t;
                    dec_税前总额 += Convert.ToDecimal(r["开票税前金额"]);
                    dec_含税总额 += Convert.ToDecimal(r["开票税后金额"]);
                }
                dt.Rows[0]["开票税前金额"] = dec_税前总额;
                dt.Rows[0]["开票税后金额"] = dec_含税总额;


                //da.Update(dt);
                //da2.Update(dtP);
                //fun_客户产品单价();
                fun_已开票数量();
                fun_事务_保存();

                barLargeButtonItem3.Enabled = false;
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void fun_check()
        {
            ds = fun_客户产品单价();
            if (ds.Tables[1].Rows.Count > 0)  ///有需要修改的记录  
            {
                //弹窗提示 是否更新       
                if (MessageBox.Show(string.Format("有变更的单价是否更新到客户单价对照表？"), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    f_price = true;
                }

            }
        }

        //导出
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //SaveFileDialog saveFileDialog = new SaveFileDialog();
                //saveFileDialog.Title = "导出txt";
                //saveFileDialog.Filter = "txt文件(*.txt)|*.txt";
                //DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                //if (dialogResult == DialogResult.OK)
                //{
                //    //DevExpress.XtraPrinting.XlsExportOptions options = new DevExpress.XtraPrinting.XlsExportOptions();
                //    DevExpress.XtraPrinting.TextExportOptions options = new DevExpress.XtraPrinting.TextExportOptions();
                //    //options.ExportMode = DevExpress.XtraPrinting.XlsExportMode.SingleFile;
                //    options.TextExportMode = DevExpress.XtraPrinting.TextExportMode.Text;
                //    gc.ExportToText(saveFileDialog.FileName, options);
                //    //gc.ExportToXls(saveFileDialog.FileName, options);
                //    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}
                //drM  dr_传
                string dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string strSoNo = string.Format("{0}{1}{2}{3}", DateTime.Now.Year.ToString().Substring(2, 2), DateTime.Now.Month.ToString("00"),
                    DateTime.Now.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("XSKP", DateTime.Now.Year, DateTime.Now.Month).ToString("000"));

                string file = string.Format(dir + @"\\{0}_{1}.txt", strSoNo, System.DateTime.Today.ToString("yyyy-MM-dd"));

                string content = strSoNo + "," + dtP.Rows.Count + "," + txt_客户名称.Text + ",,,,";
                foreach (DataRow dr in dtP.Rows)
                {
                    string sql_1 = string.Format(@"select 销售记录成品出库单明细表.*,销售记录销售订单明细表.税率  from 销售记录成品出库单明细表,销售记录销售订单明细表
                    where 销售记录成品出库单明细表.销售订单明细号= 销售记录销售订单明细表.销售订单明细号 and 成品出库单明细号='{0}'", dr["成品出库单明细号"]);
                    DataTable dt = new DataTable();
                    dt = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);
                    string str_税率 = "";

                    if (dt.Rows.Count > 0)
                    {
                        str_税率 = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[0]["税率"]) / 100).ToString("0.00");

                    }
                    content = content + Environment.NewLine + dr["产品名称"].ToString() + "," + dr["计量单位"].ToString() + "," + dr["规格型号"].ToString()
                        + "," + dr["开票数量"].ToString() + "," + Convert.ToDecimal(dr["开票税后金额"]).ToString("0.00") + "," + str_税率 + ",1601,0";
                }
                if (File.Exists(file) == true)
                {
                    System.IO.File.WriteAllText(file, content);
                }
                else
                {
                    FileStream myFs = new FileStream(file, FileMode.Create);
                    StreamWriter mySw = new StreamWriter(myFs);
                    mySw.Write(content);
                    mySw.Close();
                    myFs.Close();
                }
                MessageBox.Show("已完成导出！");
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");

            }
        }

        /// <summary>
        /// /待办
        /// </summary>
        /// 
        DataTable dt_待办;
        private void fun_载入待办()
        {
            try
            {
                //string sql = "select * from 销售记录成品出库单明细表 where 生效 = 1 and 未开票数量 > 0";

                string sql = @"select scmx.*,税前单价,round(税前单价*出库数量,2)税前金额,税后单价,round(税后单价*出库数量,2)税后金额,sz.客户订单号
          from 销售记录成品出库单明细表 scmx,销售记录销售订单明细表 smx,销售记录销售订单主表 sz
         where scmx.销售订单明细号= smx.销售订单明细号 and scmx.生效 = 1 and (未开票数量 > 0  or (scmx.备注1<>'' and 未开票数量<0)) and scmx.作废=0
        and smx.销售订单号=sz.销售订单号  /*and  smx.关闭=0*/  ";
                dt_待办 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_待办);
                string sql_补 = "";
                sql_补 = @"select scmx.*,税前单价,round(税前单价*出库数量,2)税前金额,税后单价,round(税后单价*出库数量,2)税后金额 
                            from L销售记录成品出库单明细表L scmx,L销售记录销售订单明细表L smx 
                           where scmx.销售订单明细号= smx.销售订单明细号  
                              and scmx.生效 = 1 and (未开票数量 > 0 or (备注1<>'' and 未开票数量<0)) and scmx.作废=0  and smx.关闭=0 ";

                SqlDataAdapter da_1 = new SqlDataAdapter(sql_补, strconn);

                dt_待办.Columns.Add("选择", typeof(Boolean));


                //foreach (DataRow r_x in dt_待办.Rows)
                //{
                //    r_x["选择"] = false;
                //}
                // dv_待办 = new DataView(dt_待办);


                // gc_待办.DataSource = dt_待办;
                //dt_待办.ColumnChanged += dt_待办_ColumnChanged;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "开票界面_fun_载入待办");
            }
        }



        DataTable dtP1;
        private void fun_载入明细()
        {
            try
            {
                string sql = "";
                //if (bl_新增or修改 == true)
                //{
                //    sql = @"select 销售记录销售开票明细表.* from 销售记录销售开票明细表  where     1<>1";
                //}
                //else
                //{
                sql = string.Format(@"select 销售记录销售开票明细表.* from 销售记录销售开票明细表
                                          
                                        where   销售开票通知单号 = '{0}' order by CONVERT(int,POS)", textBox3.Text.ToString());

                //} 
                dtP1 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtP1);
                //dtP.AcceptChanges();
                dtP1.Columns.Add("未开票数量", typeof(decimal));
                dtP1.Columns.Add("已出库数量", typeof(decimal));

                //gc.DataSource = dtP;
                //if (bl_新增or修改 == false)
                //{
                //成品出库单明细号
                foreach (DataRow dr in dtP1.Rows)
                {
                    DataRow[] r = dt_待办.Select(string.Format("成品出库单明细号='{0}'", dr["成品出库单明细号"]));
                    if (r.Length == 0)
                    {
                        throw new Exception("存在上次保存的记录在待办事项找不到的记录,请确认本单子明细是否存在问题");
                    }
                    r[0]["选择"] = true;
                    dr["已出库数量"] = r[0]["已出库数量"];
                    dr["未开票数量"] = r[0]["未开票数量"];
                    //}

                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "开票界面_fun_载入明细");
            }
        }
        /// <summary>
        /// 17-12-16 、、18.9.10 开票时先将没有对应关系的 加进去 ，修改的 因为 
        /// </summary>
        /// <returns></returns>
        private DataSet fun_客户产品单价()
        {
            DataSet ds = new DataSet();
            string s = "select  * from 客户产品单价表 where 1<>1";
            DateTime time = CPublic.Var.getDatetime();
            DataTable dt_增 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            DataTable dt_改 = dt_增.Clone();
            s = "select  * from 销售单价修改记录表 where 1<>1";
            DataTable dt_修改记录 = CZMaster.MasterSQL.Get_DataTable(s, strconn);

            foreach (DataRow dr in dtP1.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;

                int i = dtP.Select(string.Format("产品编码='{0}'", dr["产品编码"].ToString())).Length;
                if (i == 1)  // 有多条记录的不管 
                {
                    s = string.Format("select * from 客户产品单价表 where 物料编码='{0}' and 客户编号='{1}' ", dr["产品编码"].ToString(), txt_客户编号.Text.ToString());
                    DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                    if (temp.Rows.Count == 0) //先在对应表中 查找 有？continue;查找tem中有没有？  如果没有 插入 temp 
                    {
                        DataRow[] tr = temp.Select(string.Format("物料编码='{0}'", dr["产品编码"].ToString()));
                        if (tr.Length == 0)
                        {
                            DataRow r = dt_增.NewRow();
                            r["客户编号"] = txt_客户编号.Text.ToString();
                            r["物料编码"] = dr["产品编码"].ToString();
                            r["单价"] = dr["开票税后单价"].ToString();
                            r["修改时间"] = time;
                            dt_增.Rows.Add(r);
                        }
                    }
                    else  //单价对应表中有记录  修改
                    {
                        if (Convert.ToDecimal(temp.Rows[0]["单价"]) != Convert.ToDecimal(dr["开票税后单价"]))
                        {
                            dt_改.ImportRow(temp.Rows[0]);

                            //string ss = string.Format("select  * from 客户产品单价表 where 物料编码='{0}'", dr["产品编码"].ToString());
                            //using ( SqlDataAdapter da =new SqlDataAdapter (ss,strconn))
                            //{
                            //    da.Fill(dt_改);
                            DataRow[] rr = dt_改.Select(string.Format("物料编码='{0}'", dr["产品编码"].ToString()));
                            rr[0]["单价"] = Convert.ToDecimal(dr["开票税后单价"]);
                            rr[0]["修改时间"] = time;

                            //}
                            DataRow r_modified = dt_修改记录.NewRow();
                            r_modified["物料编码"] = dr["产品编码"];
                            r_modified["原单价"] = temp.Rows[0]["单价"];
                            r_modified["修改单价"] = Convert.ToDecimal(dr["开票税后单价"]);
                            r_modified["修改日期"] = time;
                            r_modified["修改人"] = CPublic.Var.localUserName;
                            dt_修改记录.Rows.Add(r_modified);
                        }

                    }
                }

            }
            ds.Tables.Add(dt_增);
            ds.Tables.Add(dt_改);
            ds.Tables.Add(dt_修改记录);


            return ds;
        }
        //开票数
        DataTable dt_已开票数量;
        /// <summary>
        /// 8-26 因为 劳务类 已经自动生成 出库单 所以需要再修改
        /// </summary>
        private void fun_已开票数量()
        {
            dt_已开票数量 = new DataTable();
            foreach (DataRow r in dtP.Rows)
            {

                if (r.RowState == DataRowState.Deleted)
                {
                    continue;
                }
                if (r["产品名称"].ToString().Contains("劳务") && r["成品出库单明细号"].ToString() == "")
                {

                    string sql = string.Format(" select * from 销售记录销售出库通知单明细表   where 出库通知单明细号 = '{0}'", r["出库通知单明细号"].ToString().Trim());
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt_已开票数量);
                    DataRow[] ds = dt_已开票数量.Select(string.Format("出库通知单明细号 = '{0}'", r["出库通知单明细号"].ToString().Trim()));
                    if (ds.Length > 0)
                    {
                        ds[0]["累计开票数量"] = Convert.ToDecimal(ds[0]["累计开票数量"]) + Convert.ToDecimal(r["开票数量"]);
                    }

                }
                else
                {
                    string sql = string.Format("select * from 销售记录成品出库单明细表 where 成品出库单明细号 = '{0}'", r["成品出库单明细号"].ToString().Trim());
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt_已开票数量);
                    sql = string.Format("select * from L销售记录成品出库单明细表L where 成品出库单明细号 = '{0}'", r["成品出库单明细号"].ToString().Trim());
                    da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt_已开票数量);

                    DataRow[] ds = dt_已开票数量.Select(string.Format("成品出库单明细号 = '{0}'", r["成品出库单明细号"].ToString().Trim()));

                    ds[0]["已开票数量"] = Convert.ToDecimal(ds[0]["已开票数量"]) + Convert.ToDecimal(r["开票数量"]);
                    ds[0]["未开票数量"] = Convert.ToDecimal(ds[0]["未开票数量"]) - Convert.ToDecimal(r["开票数量"]);

                }

            }
        }



        private void fun_事务_保存()
        {
            //17-12-16 
            //   DataTable dt_客户产品= fun_客户产品单价();
            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction kpsx = conn.BeginTransaction("开票生效");
            try
            {
                string sql = "select * from 销售记录销售开票明细表 where 1<>1";
                SqlCommand cmd = new SqlCommand(sql, conn, kpsx);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dtP);

                }
                sql = "select * from 销售记录销售开票主表 where 1<>1";
                cmd = new SqlCommand(sql, conn, kpsx);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt);
                }

                //sql = "select * from 客户产品单价表 where 1<>1";
                //cmd = new SqlCommand(sql, conn, kpsx);
                //SqlDataAdapter da1 = new SqlDataAdapter(cmd);
                //new SqlCommandBuilder(da1);
                //if (f_price) //true 则合并修改 和新增的   false  只更新新增的
                //{
                //    ds.Tables[0].Merge(ds.Tables[1]);
                //    sql = "select  * from 销售单价修改记录表 where 1=2 ";
                //    SqlCommand cmd1 = new SqlCommand(sql, conn, kpsx);
                //    SqlDataAdapter da2 = new SqlDataAdapter(cmd1);
                //    new SqlCommandBuilder(da2);
                //    da2.Update(ds.Tables[2]);
                //}
                //da1.Update(ds.Tables[0]);




                if (dt_已开票数量 != null && dt_已开票数量.Columns.Contains("成品出库单号"))
                {
                    //12/19   dt_已开票数量 分为两个部分  销售记录成品出库单明细表 没 通知单号的 是补开的
                    DataTable dt_辅助 = dt_已开票数量.Clone();
                    for (int i = 0; i < dt_已开票数量.Rows.Count; i++)
                    {
                        string sql_z = string.Format(@"select  * from 销售记录成品出库单明细表 where 成品出库单明细号='{0}'", dt_已开票数量.Rows[i]["成品出库单明细号"]);
                        DataTable dt_z = CZMaster.MasterSQL.Get_DataTable(sql_z, strconn);
                        if (dt_z.Rows.Count == 0)
                        {
                            dt_辅助.ImportRow(dt_已开票数量.Rows[i]);
                            dt_已开票数量.Rows.Remove(dt_已开票数量.Rows[i]);
                            i--;
                        }
                    }
                    sql = "select * from 销售记录成品出库单明细表 where 1<>1";
                    cmd = new SqlCommand(sql, conn, kpsx);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dt_已开票数量);
                    }
                    sql = "select * from L销售记录成品出库单明细表L where 1<>1";
                    cmd = new SqlCommand(sql, conn, kpsx);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dt_辅助);
                    }


                }
                else
                {
                    sql = "select * from 销售记录销售出库通知单明细表 where 1<>1";
                    cmd = new SqlCommand(sql, conn, kpsx);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_已开票数量);

                }
                kpsx.Commit();
            }
            catch (Exception ex)
            {
                kpsx.Rollback();
                throw ex;
            }
        }

        private void gv_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                decimal dec_税率 = Convert.ToDecimal(drM["税率"]);
                //if (e.Column.FieldName == "开票税前单价")
                //{
                //    dr["开票税后单价"] = Convert.ToDecimal(dr["开票税前单价"]) * (1 + dec_税率 / (decimal)100);
                //    dr["开票税前金额"] = Convert.ToDecimal(dr["开票税前单价"]) * Convert.ToDecimal(dr["开票数量"]);
                //    dr["开票税后金额"] = Convert.ToDecimal(dr["开票税后单价"]) * Convert.ToDecimal(dr["开票数量"]);

                //}
                //else if (e.Column.FieldName == "开票税后单价")
                //{
                //    dr["开票税前单价"] = Convert.ToDecimal(dr["开票税后单价"]) / (1 + dec_税率 / (decimal)100);
                //    dr["开票税前金额"] = Convert.ToDecimal(dr["开票税前单价"]) * Convert.ToDecimal(dr["开票数量"]);
                //    dr["开票税后金额"] = Convert.ToDecimal(dr["开票税后单价"]) * Convert.ToDecimal(dr["开票数量"]);
                //}

                if (e.Column.FieldName == "开票税前金额")
                {
                    //dr["开票税前单价"]= Math.Round( Convert.ToDecimal(e.Value) / Convert.ToDecimal(dr["开票数量"]),6,MidpointRounding.AwayFromZero);
                    //dr["开票税后单价"] = Convert.ToDecimal(dr["开票税前单价"]) * (1 + dec_税率 / (decimal)100);
                    //dr["开票税后金额"] = Convert.ToDecimal(dr["开票税后单价"]) * Convert.ToDecimal(dr["开票数量"]);

                    dr["开票税前单价"] = Math.Round(Convert.ToDecimal(e.Value) / Convert.ToDecimal(dr["开票数量"]), 6);
                    dr["开票税后单价"] = Math.Round(Convert.ToDecimal(dr["开票税前单价"]) * (1 + dec_税率 / 100), 6);
                    dr["开票税后金额"] = Math.Round(Convert.ToDecimal(dr["开票税后单价"]) * Convert.ToDecimal(dr["开票数量"]), 2);
                    dr["税额"] = Math.Round(Convert.ToDecimal(dr["开票税后金额"]) - Convert.ToDecimal(e.Value), 2);


                }
                else if (e.Column.FieldName == "税额")
                {
                    dr["开票税后金额"] = Math.Round(Convert.ToDecimal(dr["开票税前金额"]) + Convert.ToDecimal(e.Value), 2);
                    dr["开票税后单价"] = Math.Round(Convert.ToDecimal(dr["开票税后金额"]) / Convert.ToDecimal(dr["开票数量"]), 2);
                    decimal a = decimal.Parse(textBox2.Text.ToString());
                    dr["本币税后金额"] = Math.Round(decimal.Parse(dr["开票税后金额"].ToString()) * a, 2, MidpointRounding.AwayFromZero);
                }
                else if (e.Column.FieldName == "本币税后金额")
                {
                    decimal dec_old = Math.Round(Convert.ToDecimal(dr["本币税后金额"]), 2, MidpointRounding.AwayFromZero);
                    if (Math.Abs(dec_old - Convert.ToDecimal(e.Value)) > (decimal)0.06)
                    {
                        throw new Exception("超出可修改范围");
                    }
                    dr["本币税后金额"] = e.Value;


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {


        }
        public bool IsNumberic(string oText)
        {
            try
            {
                Decimal Number = Convert.ToDecimal(oText);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try

            {
                if (IsNumberic(textBox2.Text.ToString()) == true)
                {
                    decimal a = decimal.Parse(textBox2.Text.ToString());
                    if (dtP != null)
                    {
                        foreach (DataRow dr in dtP.Rows)
                        {
                            //if (dr["本币税后金额"].ToString() == "")

                            dr["本币税后金额"] = Math.Round(decimal.Parse(dr["开票税后金额"].ToString()) * a, 2, MidpointRounding.AwayFromZero);

                        }

                    }


                    gc.DataSource = dtP;


                }
                else
                {
                    throw new Exception("请输入数字");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void v_TextChanged(object sender, EventArgs e)
        {
            try

            {
                DateTime time = CPublic.Var.getDatetime();


                string sql = string.Format("select * from 汇率维护表 where 年='{0}'and 月='{1}' and 币种='{2}' ", time.Year, time.Month, v.Text.ToString());
                DataTable dt_bz = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                if (dt_bz.Rows.Count > 0)
                {
                    if (dtP != null)
                    {

                        textBox2.Text = dt_bz.Rows[0]["汇率"].ToString();
                        decimal a = decimal.Parse(dt_bz.Rows[0]["汇率"].ToString());
                        foreach (DataRow dr in dtP.Rows)
                        {
                            if (dr["本币税后金额"].ToString() == "")
                                dr["本币税后金额"] = Math.Round(decimal.Parse(dr["开票税后金额"].ToString()) * a, 2, MidpointRounding.AwayFromZero);

                        }
                    }
                }
                else
                {
                    throw new Exception("该币种当月未维护汇率");
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
                // string lb= ""; //类别 区分是 国内还是国外

                string s = string.Format(@"select a.*,cc.科目编码 x,cc.科目名称 y from 销售记录销售开票明细表 a
     left join  销售记录销售开票主表 b on a.销售开票通知单号=b.销售开票通知单号
     left join (select   [存货分类编码],[科目编码],[科目名称]  from [科目_销售发票] where  类别 in ('','{0}')  )cc
     on cc.存货分类编码=left(a.产品编码,len(cc.存货分类编码)) 
     where   a.销售开票通知单号= '{1}'", comboBox1.Text.Trim(), textBox3.Text);
                DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                foreach (DataRow r in t.Rows)
                {
                    string s1 = r["销售开票通知单号"].ToString();
                    string s2 = r["POS"].ToString();

                    DataRow[] rr = dtP.Select(string.Format("销售开票通知单号='{0}' and pos='{1}' ", r["销售开票通知单号"], r["POS"]));


                    //DataRow[] r3 = dtP.Select(string.Format("销售开票通知单号='{0}' and pos='{1}'", s1, s2));
                    if (rr[0]["科目编码"] == null || rr[0]["科目编码"].ToString() == "")
                    {
                        rr[0]["科目编码"] = r["x"];
                        rr[0]["科目名称"] = r["y"];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {

            try
            {
                CZMaster.MasterSQL.Save_DataTable(dtP, "销售记录销售开票明细表", strconn);
                MessageBox.Show("保存成功");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void fun_check_pz()
        {
            if (txt_开票票号.Text == "")
            {
                throw new Exception("开票票号未录入");
            }
            if (comboBox1.Text == "")
            {
                throw new Exception("类别未选择");
            }
            foreach (DataRow dr in dtP.Rows)
            {
                if (dr["科目编码"] == null || dr["科目编码"].ToString() == "")
                {
                    throw new Exception("存在科目编码为空请检查");
                }
                if (dr["科目名称"] == null || dr["科目名称"].ToString() == "")
                {
                    throw new Exception("存在科目名称为空请检查");
                }
            }
        }
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //这边是一个开票通知单是一笔凭证
                fun_check_pz();
                DateTime t_now = CPublic.Var.getDatetime();

                DateTime time = Convert.ToDateTime(drM["开票日期"]);
                int year = time.Year;
                int month = time.Month;
                string strcon_u8 = CPublic.Var.geConn("DW");
                string ERP_凭证号 = "";
                string U8_凭证号 = "";
                string s_发票号 = txt_开票票号.Text.Trim();

                //foreach (DataRow rt in dt_发票核销表.Rows)
                //{
                //    if (rt.RowState == DataRowState.Deleted) continue;
                //    s_发票号 += rt["发票号"].ToString();
                //    if (irow++ != dt_发票核销表.Rows.Count) s_发票号 += "/";
                //}

                string x = string.Format("select  * from 财务凭证表 where 单据号='{0}'", textBox3.Text); //开票通知单号
                DataTable t_erp = CZMaster.MasterSQL.Get_DataTable(x, strconn);

                if (t_erp.Rows.Count == 0)
                {
                    ERP_凭证号 = CPublic.CNo.fun_得到最大流水号("PZ", year, month).ToString();

                }
                else
                {
                    ERP_凭证号 = t_erp.Rows[0]["凭证号"].ToString();
                    U8_凭证号 = t_erp.Rows[0]["U8凭证号"].ToString();
                }
                //这边需要根据名称 去u8搜一下客户编码  因为这边可能编码不一样
                string kh = string.Format("select ccuscode from Customer where ccusname = '{0}'", drM["客户名称"]);
                DataTable dt_kh = CZMaster.MasterSQL.Get_DataTable(kh, strcon_u8);

                if (dt_kh.Rows.Count == 0) throw new Exception("因本系统与U8用该名称不一致,未找到客户编号,请将两个客户名称一致");
                string strkh = dt_kh.Rows[0]["ccuscode"].ToString();
                MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
                DataTable dtP_copy = dtP.Copy();
                //DataColumn dc = new DataColumn("汇率",typeof(decimal));
                //dc.DefaultValue = Convert.ToDecimal(textBox2.Text) ;
                //dtP_copy.Columns.Add(dc);
                foreach (DataRow dr in dtP_copy.Rows)
                {
                    dr["开票税前金额"] = Math.Round(Convert.ToDecimal(dr["开票税前金额"]) * Convert.ToDecimal(textBox2.Text), 2, MidpointRounding.AwayFromZero);
                }
                DataTable dt_凭证 = RBQ.SelectGroupByInto("", dtP_copy, "科目编码,科目名称,sum(开票税前金额) 不含税金额", "", "科目编码,科目名称");
                decimal dec_本币总额 = 0;
                foreach (DataRow r_bb in dtP.Rows)
                {
                    dec_本币总额 += Convert.ToDecimal(r_bb["本币税后金额"]);
                }
                decimal dec_总税额 = Math.Round(Convert.ToDecimal(drM["开票税后金额"]), 2) - Math.Round(Convert.ToDecimal(drM["开票税前金额"]), 2);

                string s = string.Format("select * from GL_accvouch where iyear={0} and iperiod={1} and ino_id='{2}'", year, month, U8_凭证号);
                DataTable dt_u8 = CZMaster.MasterSQL.Get_DataTable(s, strcon_u8);
                if (U8_凭证号 != "") //已有数据 需要把原来的先删除 再增加
                {
                    for (int l = dt_u8.Rows.Count - 1; l >= 0; l--)
                    {
                        dt_u8.Rows[l].Delete();
                    }
                    for (int j = t_erp.Rows.Count - 1; j >= 0; j--)
                    {
                        t_erp.Rows[j].Delete();
                    }
                }
                else
                {
                    string xx = string.Format("select isnull(MAX(ino_id),0) 凭证号 from GL_accvouch where iyear={0} and iperiod={1}", year, month);
                    DataRow pzh = CZMaster.MasterSQL.Get_DataRow(xx, strcon_u8);
                    U8_凭证号 = (Convert.ToInt32(pzh[0]) + 1).ToString();
                }
                string cdigest = "";
                string yszk = ""; // 应收账款 对应科目
                string yszk_name = ""; // 应收账款 对应科目

                if (comboBox1.Text == "国外")
                {
                    cdigest = "销售 " + txt_客户名称.Text + " " + Math.Round(Convert.ToDecimal(drM["开票税后金额"]), 2).ToString() + v.Text + " 发票号: " + s_发票号;
                    yszk = "112201";
                    yszk_name = "国外应收账款";
                }
                else
                {
                    cdigest = "销售 " + txt_客户名称.Text + " 发票号: " + s_发票号;
                    yszk = "112202";
                    yszk_name = "国内应收账款";

                }
                int i = 1; //行号
                //销售先录入应收账款 区分国内国外
                #region 借方金额  总金额
                DataRow rr = dt_u8.NewRow();
                rr["iperiod"] = month;
                rr["csign"] = "记";
                rr["isignseq"] = 1;
                rr["ino_id"] = U8_凭证号;
                rr["inid"] = i;
                rr["dbill_date"] = time.Date;
                rr["bdelete"] = 0;
                rr["bvouchedit"] = 1; //可修改
                rr["bvouchAddordele"] = 0; //bvouchAddordele 是否可增删
                rr["bvouchmoneyhold"] = 0; //凭证合计金额是否保值 
                rr["bvalueedit"] = 1; //分录数值是否可修改 
                rr["bcodeedit"] = 1; //分录科目是否可修改  
                rr["bPCSedit"] = 1; //分录往来项是否可修改   
                rr["bDeptedit"] = 1; //分录部门是否可修改    
                rr["bItemedit"] = 1; //分录项目是否可修改 
                rr["bCusSupInput"] = 0; //分录往来项是否必输  
                rr["ccus_id"] = strkh;//这个需要录入客户
                rr["idoc"] = -1;
                rr["cbill"] = CPublic.Var.localUserName;
                rr["dt_date"] = Convert.ToDateTime(drM["开票日期"]).Date;
                rr["ctext1"] = ERP_凭证号;
                rr["cdigest"] = cdigest;
                // ctext1里面存放我们的凭证号
                rr["ccode"] = yszk;
                rr["md"] = dec_本币总额;
                if (comboBox1.Text == "国外")
                {
                    rr["md_f"] = Math.Round(Convert.ToDecimal(drM["开票税后金额"]), 2);
                    rr["cexch_name"] = drM["币种"].ToString();
                    rr["nfrat"] = drM["汇率"].ToString();
                }
                // rr["ccodeexch_equal"] = rr["ccode_equal"] = exch;          //对应的都是进项税
                rr["coutaccset"] = "008";
                rr["doutbilldate"] = time.Date;
                rr["RowGuid"] = System.Guid.NewGuid();
                rr["iyear"] = year;
                rr["iYPeriod"] = year.ToString() + month.ToString("00");
                rr["tvouchtime"] = t_now;
                dt_u8.Rows.Add(rr);

                DataRow rr_erp = t_erp.NewRow();
                rr_erp["凭证号"] = ERP_凭证号;
                rr_erp["U8凭证号"] = U8_凭证号;
                rr_erp["inid"] = i;
                rr_erp["摘要"] = cdigest;
                rr_erp["制单日期"] = time;
                rr_erp["制单人"] = CPublic.Var.localUserName;
                rr_erp["年"] = year;
                rr_erp["月"] = month;
                rr_erp["科目编号"] = yszk;
                rr_erp["科目名称"] = yszk_name;
                rr_erp["借方金额"] = dec_本币总额;
                rr_erp["单据号"] = textBox3.Text.Trim();
                t_erp.Rows.Add(rr_erp);
                i++;
                #endregion

                //这里新增的是 按科目汇总后的开票明细
                foreach (DataRow r_pz in dt_凭证.Rows)
                {
                    DataRow r = dt_u8.NewRow();
                    r["iperiod"] = month;
                    r["csign"] = "记";
                    r["isignseq"] = 1;
                    r["ino_id"] = U8_凭证号;
                    r["inid"] = i;
                    r["dbill_date"] = time.Date;
                    r["idoc"] = -1;
                    r["bdelete"] = 0;
                    r["bvouchedit"] = 1; //可修改
                    r["bvouchAddordele"] = 0; //bvouchAddordele 是否可增删
                    r["bvouchmoneyhold"] = 0; //凭证合计金额是否保值 
                    r["bvalueedit"] = 1; //分录数值是否可修改 
                    r["bcodeedit"] = 1; //分录科目是否可修改  
                    r["bPCSedit"] = 1; //分录往来项是否可修改   
                    r["bDeptedit"] = 1; //分录部门是否可修改    
                    r["bItemedit"] = 1; //分录项目是否可修改 
                    r["bCusSupInput"] = 0; //分录往来项是否必输  

                    r["cbill"] = CPublic.Var.localUserName;
                    r["ctext1"] = ERP_凭证号;
                    r["cdigest"] = cdigest;
                    // ctext1里面存放我们的凭证号
                    r["ccode"] = r_pz["科目编码"];
                    r["mc"] = r_pz["不含税金额"];
                    r["ccodeexch_equal"] = r["ccode_equal"] = yszk; //对应的都是销项税额
                    r["coutaccset"] = "008";
                    r["doutbilldate"] = time.Date;
                    r["RowGuid"] = System.Guid.NewGuid();
                    r["iyear"] = year;
                    r["iYPeriod"] = year.ToString() + month.ToString("00");
                    r["tvouchtime"] = t_now;
                    dt_u8.Rows.Add(r);

                    DataRow r_erp = t_erp.NewRow();
                    r_erp["凭证号"] = ERP_凭证号;
                    r_erp["U8凭证号"] = U8_凭证号;
                    r_erp["inid"] = i;
                    r_erp["摘要"] = cdigest;
                    r_erp["制单日期"] = time;
                    r_erp["制单人"] = CPublic.Var.localUserName;
                    r_erp["年"] = year;
                    r_erp["月"] = month;
                    r_erp["科目编号"] = r_pz["科目编码"];
                    r_erp["科目名称"] = r_pz["科目名称"];
                    r_erp["贷方金额"] = r_pz["不含税金额"];
                    r_erp["单据号"] = textBox3.Text.Trim();
                    t_erp.Rows.Add(r_erp);
                    i++;
                }
                //科目明细项已经增加进去了 还要增加 销项税 和 借方金额的总金额
                //19-11-28 财务要求 进项税 根据 有几张发票 增加几行进项税 
                #region 销项税  国内要 国外不要
                if ((comboBox1.Text == "国内"))
                {
                    DataRow r1 = dt_u8.NewRow();
                    r1["iperiod"] = month;
                    r1["csign"] = "记";
                    r1["isignseq"] = 1;
                    r1["ino_id"] = U8_凭证号;
                    r1["inid"] = i;
                    r1["dbill_date"] = time.Date;
                    r1["bdelete"] = 0;
                    r1["bvouchedit"] = 1; //可修改
                    r1["bvouchAddordele"] = 0; //bvouchAddordele 是否可增删
                    r1["bvouchmoneyhold"] = 0; //凭证合计金额是否保值 
                    r1["bvalueedit"] = 1; //分录数值是否可修改 
                    r1["bcodeedit"] = 1; //分录科目是否可修改  
                    r1["bPCSedit"] = 1; //分录往来项是否可修改   
                    r1["bDeptedit"] = 1; //分录部门是否可修改    
                    r1["bItemedit"] = 1; //分录项目是否可修改 
                    r1["bCusSupInput"] = 0; //分录往来项是否必输  
                    r1["idoc"] = -1;
                    r1["cbill"] = CPublic.Var.localUserName;
                    r1["ctext1"] = ERP_凭证号;
                    r1["cdigest"] = cdigest;
                    //        // ctext1里面存放我们的凭证号
                    r1["ccode"] = "22210107"; //销项税
                                              //这里是总的 税金                          //
                                              //r1["mc"] = Math.Round(Convert.ToDecimal(txt_cgshuijin.Text), 2, MidpointRounding.AwayFromZero);
                                              //这是每个发票得 税金 明细
                    r1["mc"] = dec_总税额;

                    r1["ccodeexch_equal"] = r1["ccode_equal"] = yszk;          //对应的都是销项税
                    r1["coutaccset"] = "008";
                    r1["doutbilldate"] = time.Date;
                    r1["RowGuid"] = System.Guid.NewGuid();
                    r1["iyear"] = year;
                    r1["iYPeriod"] = year.ToString() + month.ToString("00");
                    r1["tvouchtime"] = t_now;
                    dt_u8.Rows.Add(r1);


                    DataRow r_erp1 = t_erp.NewRow();
                    r_erp1["凭证号"] = ERP_凭证号;
                    r_erp1["U8凭证号"] = U8_凭证号;
                    r_erp1["inid"] = i;
                    r_erp1["摘要"] = cdigest;
                    r_erp1["制单日期"] = time;
                    r_erp1["制单人"] = CPublic.Var.localUserName;
                    r_erp1["年"] = year;
                    r_erp1["月"] = month;
                    r_erp1["科目编号"] = "22210107"; //销项税
                    r_erp1["科目名称"] = "销项税";

                    r_erp1["贷方金额"] = Math.Round(Convert.ToDecimal(drM["开票税后金额"]), 2) - Math.Round(Convert.ToDecimal(drM["开票税前金额"]), 2); ;
                    r_erp1["单据号"] = textBox3.Text.Trim();
                    t_erp.Rows.Add(r_erp1);
                    i++;
                }
                #endregion
                string exch = "";
                int int_ex = 1;
                DataRow U8_r = null;
                foreach (DataRow exr in dt_u8.Rows)
                {
                    if (exr.RowState == DataRowState.Deleted) continue;
                    if (int_ex == 1)
                    {
                        U8_r = exr;
                        int_ex++;
                        continue;
                    }

                    exch = exch + exr["ccode"];
                    if (int_ex++ != dt_u8.DefaultView.Count) exch = exch + ",";
                }

                U8_r["ccodeexch_equal"] = exch;
                string exch_ffff = exch;
                if (exch.Length > 50) //U8这个吊字段  长度50 
                {
                    exch_ffff = exch_ffff.Substring(0, 50);
                    exch_ffff = exch_ffff.Substring(0, exch_ffff.LastIndexOf(','));
                }
                U8_r["ccode_equal"] = exch_ffff;
                drM["U8凭证号"] = U8_凭证号;
                drM["ERP凭证号"] = ERP_凭证号;
                drM["bl_pz"] = true;


                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("pur"); //事务的名称
                SqlCommand cmd1 = new SqlCommand("select * from 销售记录销售开票主表 where 1<>1", conn, ts);
                SqlCommand cmd = new SqlCommand(x, conn, ts);

                try
                {

                    SqlDataAdapter da;
                    da = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da);
                    da.Update(drM.Table);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(t_erp);
                    CZMaster.MasterSQL.Save_DataTable(dt_u8, "GL_accvouch", strcon_u8);
                    ts.Commit();
                    MessageBox.Show("生成凭证成功");

                }
                catch (Exception ex)
                {
                    ts.Rollback();
                    throw new Exception(ex.Message);
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
                string u8_凭证号 = drM["U8凭证号"].ToString();
                if (u8_凭证号 == "") throw new Exception("没有凭证可删除");
                DateTime time = Convert.ToDateTime(drM["开票日期"]);
                int year = time.Year;
                int month = time.Month;
                string sql = $"select count(*)xx from 仓库月出入库结转表 where 结算日期 >='{time}' order by 结算日期 desc";
                DataRow r_temp = CZMaster.MasterSQL.Get_DataRow(sql, strconn);
                if (Convert.ToInt32(r_temp[0]) > 0)
                {
                    throw new Exception($"{year}年{month}月已结账不可删除");
                }


                if (MessageBox.Show(string.Format("是否确认删除U8凭证号'{0}'？", drM["U8凭证号"].ToString()), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    string s = string.Format("delete GL_accvouch where iyear={0} and iperiod={1} and ino_id='{2}'", year, month, u8_凭证号);
                    CZMaster.MasterSQL.ExecuteSQL(s, strconn);
                    s = string.Format(@"delete  财务凭证表 where U8凭证号='{0}' and 年='{1}' and 月='{2}'
                       update 销售记录销售开票主表 set U8凭证号='',ERP凭证号='',bl_pz=0  where 销售开票通知单号='{3}'", u8_凭证号, year, month, textBox3.Text.Trim());
                    CZMaster.MasterSQL.ExecuteSQL(s, strconn);
                    MessageBox.Show("凭证已删除");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
