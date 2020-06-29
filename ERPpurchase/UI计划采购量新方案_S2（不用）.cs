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
namespace ERPpurchase
{
    public partial class UI计划采购量新方案_S2 : UserControl
    {
        #region 成员
        DataTable dtM1 = null;/*未使用*/
        DataTable dtM2 = null;/*原材料*/
        DataTable dtP = null;/*成品*/
        DataTable dt_selResult = null;
        DataRow r_pm = null;
        DataTable dt_采购供应商 = null;

        string str_采购单号 = "";
        string strcon = CPublic.Var.strConn;
        string str_person = "";
        //记录是采购员是谁 or  制造部老大或高管 要看个人的
        string cfgfilepath = "";
        #endregion

        #region 自用类
        public UI计划采购量新方案_S2()
        {
            InitializeComponent();
        }

        private void UI计划采购量新方案_Load(object sender, EventArgs e)
        {
            try
            {


                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                if (File.Exists(cfgfilepath + string.Format(@"\{0}.xml", this.Name)))
                {

                    gv.RestoreLayoutFromXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

                checkBox1.Checked = false;
                fun_载入供应商();
                DateTime t = CPublic.Var.getDatetime();
                t = t.AddMonths(-3);
                bar_日期.EditValue = new DateTime(t.Year, t.Month, 1);
                string sql = string.Format("select * from 人事基础员工表 where 员工号='{0}'", CPublic.Var.LocalUserID);
                DataTable dt = new DataTable();
                dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                if ((dt.Rows[0]["部门编号"].ToString() == "00010301" && dt.Rows[0]["职务"].ToString() == "部长") || dt.Rows[0]["权限组"].ToString() == "公司高管权限")
                {
                    str_person = "admin";  //ID
                    barStaticItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    barEditItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    string sql_1 = "Select 员工号,姓名 from  [采购人员关联供应商表] group by  员工号,姓名";
                    DataTable dt_1 = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);
                    repositoryItemSearchLookUpEdit2.DataSource = dt_1;
                    repositoryItemSearchLookUpEdit2.ValueMember = "员工号";
                    repositoryItemSearchLookUpEdit2.DisplayMember = "姓名";


                }
                else
                {
                    str_person = CPublic.Var.LocalUserID;
                    barStaticItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    barEditItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                }
                //这里筛选条件有可能要改为  物料的 物料类型 为 库存商品 的
                string s = @"select 物料编码,物料名称,规格型号  from 基础数据物料信息表 base 
                inner join (select  产品编码 from 基础数据物料BOM表  group by 产品编码 ) a   on base.物料编码=a.产品编码 ";
                DataTable dt_产品 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                searchLookUpEdit1.Properties.DataSource = dt_产品;
                searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                searchLookUpEdit1.Properties.ValueMember = "物料编码";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 界面操作
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (barEditItem1.Visibility == DevExpress.XtraBars.BarItemVisibility.Always && barEditItem1.EditValue != null && barEditItem1.EditValue.ToString() != "")
                {
                    str_person = barEditItem1.EditValue.ToString();
                }
                else
                {
                    str_person = CPublic.Var.LocalUserID;
                }

                string sql_gys = string.Format("select * from [采购人员关联供应商表] where  员工号='{0}'", str_person);
                DataTable dt_ls = CZMaster.MasterSQL.Get_DataTable(sql_gys, strcon);

                checkBox1.Checked = false;

                dtM2 = StockCore.StockCorer.fun_销售受订量_S2(gv, Convert.ToDateTime(bar_日期.EditValue), dt_ls, str_person);

                dtM2.Columns.Add("选择", typeof(Boolean));
                dtM2.Columns.Add("输入采购数量", typeof(Decimal));
                //dtP = StockCore.StockCorer.fun_计划_需生产();
                gv.ViewCaption = "采购";
                //gc2.DataSource = dtP;
                DataView dv = new DataView(dtM2);
                dv.RowFilter = "欠缺数量包含安全库存 > 0 or 库存总数<库存下限";
                //dv.RowFilter = "受订量a > 0 ";

                gc.DataSource = dv;
                DataTable dtX = dv.ToTable();
                DataTable xxx = new DataTable();
                dtX.AcceptChanges();
                dtX.Columns["受订量a"].ColumnName = "参考量";

                //2018-10-16
                string ss = "select * from 计划池物料参考量表  ";//未送检清单
                DataTable dt_cklAll = CZMaster.MasterSQL.Get_DataTable(ss, strcon);


                ss = "select * from 计划池物料参考量表 where 物料编码 in (select  物料编码  from 采购记录采购单明细表  where 作废=0 and 明细完成 =0 group by 物料编码) ";//未送检清单
                DataTable tt = CZMaster.MasterSQL.Get_DataTable(ss, strcon);
                foreach (DataRow dr in tt.Rows)
                {
                    DataRow[] rr = dtM2.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (rr.Length > 0)
                    {
                        dr["参考量"] = rr[0]["受订量a"];

                    }
                    if (dr["参考量"] == DBNull.Value || dr["参考量"].ToString() == null || dr["参考量"].ToString() == "")
                    {
                        dr["参考量"] = 0;
                    }
                }
                foreach (DataRow dr in dtX.Rows)  //dtX有dt_参考量中没有的 加入
                {
                    if (dr["参考量"] == DBNull.Value || dr["参考量"].ToString() == null || dr["参考量"].ToString() == "")
                    {
                        dr["参考量"] = 0;
                    }
                    DataRow[] rr = dt_cklAll.Select(string.Format("物料编码='{0}'", dr["物料编码"].ToString()));
                    if (rr.Length == 0)
                    {
                        DataRow r = tt.NewRow();
                        tt.Rows.Add(r);
                        r["物料编码"] = dr["物料编码"];
                        r["参考量"] = dr["参考量"];
                    }
                }
             
                string s = "select * from 计划池物料参考量表 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(s, strcon))
                {
                    new SqlCommandBuilder(da);
       
                    da.Update(tt); //2018-10-16
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                //gridControl1.ExportToXls(saveFileDialog.FileName, options);  
                if (xtraTabControl1.SelectedTabPage.Name == "xtraTabPage1")
                {
                    gc.ExportToXlsx(saveFileDialog.FileName);
                }
                else
                {
                    gc2.ExportToXlsx(saveFileDialog.FileName);
                }
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region 事件
 
        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            //行号设置 
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString("00");
            }
        }
        #endregion

        #region 方法
        private void fun_载入供应商()
        {
            string sql = "select * from 采购供应商表";
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            dt_采购供应商 = new DataTable();
            da.Fill(dt_采购供应商);
        }
        #endregion

        #region 生成采购单
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            /*生成采购单*/
            try
            {
                gv.CloseEditor();
                this.BindingContext[dtM2].EndCurrentEdit();

                fun_选择行();
                if (dt_selResult.Rows.Count <= 0)
                    throw new Exception("请选择需要生成采购单的行！");
                fun_check选择行();
                fun_采购单生成();

                if (MessageBox.Show(string.Format("采购单\"{0}\"生成成功，是否跳转到采购单明细界面？", str_采购单号), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    frm采购单明细 fm = new frm采购单明细(r_pm);
                    CPublic.UIcontrol.AddNewPage(fm, "采购单明细");
                }
                // barLargeButtonItem2_ItemClick(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_选择行()
        {
            try
            {
                dt_selResult = dtM2.Clone();
                foreach (DataRow r in dtM2.Rows)
                {
                    if (r["选择"].Equals(true))
                    {
                        dt_selResult.Rows.Add(r.ItemArray);
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_ChoseDataRow");
                throw new Exception(ex.Message);
            }
        }

        private void fun_check选择行()
        {
            try
            {
                //检查已经选择的数据的有效性
                string gysid = dt_selResult.Rows[0]["默认供应商"].ToString();
                foreach (DataRow r in dt_selResult.Rows)
                {   //供应商是否一致，进行提醒
                    if (gysid != r["默认供应商"].ToString())
                    {
                        if (MessageBox.Show("你所选择生成采购单的计划数据，供应商不一致，是否生成采购单？", "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            //供应商不一致可以生成采购单
                        }
                        else
                        {
                            throw new Exception("请重新选择一致的供应商，再生成采购单！");
                        }
                    }
                    if (r["输入采购数量"].ToString() == "")
                        throw new Exception("请输入需要生成采购单的数量，输入数量不能为空，请检查！");
                    try
                    {
                        Decimal d = Convert.ToDecimal(r["输入采购数量"]);
                    }
                    catch
                    {
                        throw new Exception("输入的数量应该为数字，请重新输入！");
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_checkChoseRow");
                throw new Exception(ex.Message);
            }
        }

        private void fun_采购单生成()
        {
            try
            {
                SqlDataAdapter da;
                DateTime time = CPublic.Var.getDatetime();
                //采购单号          
                str_采购单号 = string.Format("PO{0}{1:00}{2:00}{3:0000}", time.Year, time.Month, time.Day, CPublic.CNo.fun_得到最大流水号("PO", time.Year, time.Month)); //采购单号
                //供应商的信息数据
                DataRow[] dr_gys = dt_采购供应商.Select(string.Format("供应商ID = '{0}'", dt_selResult.Rows[0]["供应商编号"]));

                int pos = 1;
                Decimal str_总金额 = 0;  //计算整个采购单的总金额

                //采购单明细表
                string sql_m = "select * from 采购记录采购单明细表 where 1<>1";
                da = new SqlDataAdapter(sql_m, strcon);
                DataTable dt_purmx = new DataTable();
                da.Fill(dt_purmx);
                decimal dec_税比 = 0;
                decimal dec_税 = 0;

                foreach (DataRow r in dt_selResult.Rows)
                {
                    DataRow r1 = dt_purmx.NewRow();
                    //r1["计划采购量"] = r["未完成采购数量"];
                    r1["GUID"] = System.Guid.NewGuid();
                    r1["采购单号"] = str_采购单号;
                    r1["采购明细号"] = str_采购单号 + "-" + pos.ToString("00");
                    r1["明细类型"] = "采购计划";
                    r1["采购POS"] = pos++;
                    r1["物料编码"] = r["物料编码"];
                    r1["物料名称"] = r["物料名称"];
                    r1["规格型号"] = r["规格型号"];
                    r1["图纸编号"] = r["图纸编号"];
                    r1["仓库号"] = r["仓库号"];
                    r1["仓库名称"] = r["仓库名称"];
                    r1["采购数量"] = r["输入采购数量"];
                    r1["未完成数量"] = r["输入采购数量"];
                    r1["计量单位编码"] = r["计量单位编码"];
                    r1["计量单位"] = r["计量单位"];
                    if (dr_gys.Length > 0)
                    {
                        r1["供应商ID"] = dr_gys[0]["供应商ID"];
                        r1["供应商"] = dt_selResult.Rows[0]["默认供应商"];
                        r1["供应商负责人"] = dr_gys[0]["供应商负责人"];
                        r1["供应商电话"] = dr_gys[0]["供应商电话"];
                    }
                    string sqll = string.Format(@"select a.*,税率 from 采购供应商物料单价表 a
                    left  join 采购供应商表 b on a.供应商ID=b.供应商ID
                    where a.供应商ID = '{0}' and 物料编码 = '{1}'", r1["供应商ID"], r1["物料编码"]);
                    DataTable t = new DataTable();
                    SqlDataAdapter daa = new SqlDataAdapter(sqll, strcon);
                    daa.Fill(t);
                    if (t.Rows.Count > 0)
                    {
                        dec_税 = Convert.ToDecimal(t.Rows[0]["税率"]);
                        dec_税比 = 1 + dec_税 / (decimal)100;

                        r1["单价"] = t.Rows[0]["单价"];
                        r1["采购价"] = t.Rows[0]["单价"];
                        r1["未税单价"] = Convert.ToDecimal(r1["单价"]) / dec_税比;
                        r1["税率"] = t.Rows[0]["税率"];
                        r1["金额"] = Convert.ToDecimal(r["输入采购数量"]) * Convert.ToDecimal(r1["单价"]);
                        str_总金额 += (decimal)r1["金额"];
                        r1["未税金额"] = (decimal)r1["金额"] / dec_税比;
                        r1["税金"] = (decimal)r1["金额"] - (decimal)r1["未税金额"];
                    }
                    else
                    {
                        dec_税比 = 1;
                        dec_税 = 0;
                        r1["单价"] = 0;
                        r1["采购价"] = 0;
                        r1["未税单价"] = 0;
                        r1["税率"] = 0;
                        r1["金额"] = 0;
                        r1["未税金额"] = 0;
                        r1["税金"] = 0;
                    }

                    // r1["未税单价"] = Convert.ToDecimal(r1["单价"]) / Convert.ToDecimal(1.17);
                    //  r1["税率"] = r1["税率"] = t.Rows[0]["单价"];
                    //r1["金额"] = Convert.ToDecimal(r["输入采购数量"]) * Convert.ToDecimal(r1["单价"]);
                    //str_总金额 += (decimal)r1["金额"];
                    //r1["未税金额"] = ((decimal)r1["金额"] / (decimal)1.17);
                    //r1["税金"] = ((decimal)r1["金额"] / (decimal)1.17) * (decimal)0.17;
                    r1["员工号"] = CPublic.Var.LocalUserID;
                    r1["采购人"] = CPublic.Var.localUserName;
                    r1["未完成数量"] = r["输入采购数量"];
                    r1["操作员ID"] = CPublic.Var.LocalUserID;
                    r1["操作员"] = CPublic.Var.localUserName;
                    r1["生成人员"] = CPublic.Var.localUserName;
                    dt_purmx.Rows.Add(r1);
                }

                //采购单主表
                string sql = "select * from 采购记录采购单主表 where 1<>1";
                da = new SqlDataAdapter(sql, strcon);
                DataTable dt_purDt = new DataTable();
                da.Fill(dt_purDt);

                r_pm = dt_purDt.NewRow();
                r_pm["GUID"] = System.Guid.NewGuid();
                r_pm["采购单号"] = str_采购单号;
                r_pm["采购计划日期"] = time;
                r_pm["未税金额"] = str_总金额 / dec_税比;
                r_pm["税率"] = dec_税;
                r_pm["总金额"] = str_总金额;
                r_pm["税金"] = str_总金额 - str_总金额 / dec_税比;
                if (dr_gys.Length > 0)
                {
                    r_pm["供应商ID"] = dr_gys[0]["供应商ID"];
                    r_pm["供应商"] = dt_selResult.Rows[0]["默认供应商"];
                    r_pm["供应商负责人"] = dr_gys[0]["供应商负责人"];
                    r_pm["供应商电话"] = dr_gys[0]["供应商电话"];
                }
                r_pm["员工号"] = CPublic.Var.LocalUserID;
                r_pm["经办人"] = CPublic.Var.localUserName;
                r_pm["采购公司"] = "苏州未来电器股份有限公司";
                r_pm["录入日期"] = time;
                r_pm["创建日期"] = time;
                r_pm["修改日期"] = time;
                r_pm["操作员ID"] = CPublic.Var.LocalUserID;
                r_pm["操作员"] = CPublic.Var.localUserName;
                r_pm["生成人员"] = CPublic.Var.localUserName;
                dt_purDt.Rows.Add(r_pm);

                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("newPurchase");
                SqlCommand cmd_cgzb = new SqlCommand("select * from 采购记录采购单主表 where 1<>1", conn, ts);
                SqlCommand cmd_cgmx = new SqlCommand("select * from 采购记录采购单明细表 where 1<>1", conn, ts);

                try
                {
                    //采购单主表
                    da = new SqlDataAdapter(cmd_cgzb);
                    new SqlCommandBuilder(da);
                    da.Update(dt_purDt);
                    //采购明细表
                    da = new SqlDataAdapter(cmd_cgmx);
                    new SqlCommandBuilder(da);
                    da.Update(dt_purmx);
                    ts.Commit();
                }
                catch
                {
                    ts.Rollback();
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_newPurchase");
                throw new Exception(ex.Message);
            }
        }
        #endregion

        DataTable dt_存;
        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (dr == null) return;
                if (e.Clicks == 2)
                {

                    //ERPStock.frm仓库物料数量明细 frm = new ERPStock.frm仓库物料数量明细(dr["物料编码"].ToString());
                    //CPublic.UIcontrol.AddNewPage(frm, "仓库物料数量明细");\
                    Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, @"ERPStock.dll"));
                    Type outerForm = outerAsm.GetType("ERPStock.frm仓库物料数量明细", false);
                    //  Form ui = Activator.CreateInstance(outerForm) as Form;
                    object[] dic = new object[2];
                    dic[0] = dr["物料编码"].ToString();
                    dic[1] = dr["仓库号"].ToString();

                    UserControl ui = Activator.CreateInstance(outerForm, dic) as UserControl; // 过往出口明细 构造函数 有两个参数,string ,datetime 
                    CPublic.UIcontrol.Showpage(ui, "仓库物料数量明细");

                }
                if (e != null && e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gc, new Point(e.X, e.Y));
                    gv.CloseEditor();


                }
                
                    using (SqlDataAdapter da = new SqlDataAdapter("select 产品编码 from 基础数据物料BOM表 where 1<>1", strcon))
                    {
                        DataTable dt_结构 = new DataTable();
                        da.Fill(dt_结构);
                        dt_存 = dt_结构.Copy();

                    }

                    string a_物料 = dr["物料编码"].ToString();
                    //fun_GetDataByChildNo(a_物料);

                    fun_GetFather(a_物料,1);
                    DataTable dt_未完成制令 = new DataTable();
                    foreach (DataRow drr in dt_存.Rows)
                    {
                        string str2 = string.Format("select 生产制令单号,生产制令类型,物料编码,物料名称,规格型号,制令数量,已排单数量,未排单数量,预完工日期,生效,完成  from 生产记录生产制令表 where 物料编码 ='{0}'and 完成=0", drr["产品编码"].ToString());
                        using (SqlDataAdapter da = new SqlDataAdapter(str2, strcon))
                        {
                            da.Fill(dt_未完成制令);

                        }
                    }
                    gc1.DataSource = dt_未完成制令;

            

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




        }

        /// <summary>
        /////子项目及编码 找父项
        /// </summary>


        private DataTable fun_GetDataByChildNo(string a_物料)
        {
            DataTable dt_物料1 = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter("select 产品编码 from 基础数据物料BOM表 where 子项编码='" + a_物料 + "'", strcon))
            {
                 da.Fill(dt_物料1);
            }
            return dt_物料1;
        }
        private void fun_GetFather(string a_物料,decimal dec)
        {
            DataTable dt1 = fun_GetDataByChildNo(a_物料);
            foreach (DataRow dr in dt1.Rows)
            {
                DataTable dt2 = fun_GetDataByChildNo(dr["产品编码"].ToString());
                DataRow r = dt_存.NewRow();
                r["产品编码"] = dr["产品编码"].ToString();

                dt_存.Rows.Add(r);
                if (dt2.Rows.Count > 0)
                {
                 fun_GetFather(dr["产品编码"].ToString(),dec);
                }
               

            }
       }




        private void 查看蓝图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                string strConn_FS = CPublic.Var.geConn("FS");
                string sql = string.Format(@"select * from [基础物料蓝图表] where 物料号='{0}'  and 版本=0", dr["原ERP物料编号"]);
                DataRow rr = CZMaster.MasterSQL.Get_DataRow(sql, strcon);

                if (rr == null || rr["文件地址"] == null || rr["文件地址"].ToString() == "")
                {
                    throw new Exception("未上传文件,没有文件可以查看");
                }
                string type = rr["后缀"].ToString();
                string foldPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\查看文件\\";
                //     string fileName = foldPath + DateTime.Now.ToString("yyyy-MM-dd") + "T" + DateTime.Now.ToString("HH_mm_ss") + "Z" + "_" + Guid.NewGuid().ToString() + "." + type;
                string fileName = foldPath + "预览." + type;
                try
                {
                    System.IO.Directory.Delete(foldPath, true);
                }
                catch (Exception)
                {

                }
                CFileTransmission.CFileClient.strCONN = strConn_FS;
                CFileTransmission.CFileClient.Receiver(rr["文件地址"].ToString(), fileName);

                ItemInspection.ui预览文件 ui = new ItemInspection.ui预览文件(fileName);
                CPublic.UIcontrol.Showpage(ui, "预览文件");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_ColumnPositionChanged(object sender, EventArgs e)
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gv.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
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

        private void gv_ColumnWidthChanged(object sender, DevExpress.XtraGrid.Views.Base.ColumnEventArgs e)
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gv.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void gv1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv1.GetFocusedRowCellValue(gv1.FocusedColumn));
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() == "")
                {
                    DataTable ListM = new DataTable();
                              string s = "select  产品编码,子项编码  from 基础数据物料BOM表 ";
                DataTable dt_bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    ListM = ERPorg.Corg.billofM(ListM, searchLookUpEdit1.EditValue.ToString(), true,dt_bom);

                    DataView dv = new DataView(dtP);
                      s = "(计算量包含安全库存 > 0   or 库存总数<库存下限)  and 物料编码 in (";

                    foreach (DataRow dr in ListM.Rows)
                    {
                        s = s + string.Format("'{0}',", dr["子项编码"]);
                    }
                    s = s.Substring(0, s.Length - 1) + ")";
                    dv.RowFilter = s; 
                    gc2.DataSource = dv;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataView dv = new DataView(dtP);
            dv.RowFilter = "计算量包含安全库存 > 0  or 库存总数<库存下限 ";
            gc2.DataSource = dv;
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                gc.DataSource = dtM2;
            }
            else
            {
                DataView dv = new DataView(dtM2);
                dv.RowFilter = "欠缺数量包含安全库存 > 0";
                gc.DataSource = dv;
            }
        }


        #region  2019-1-17 根据东屋实际情况 修改   郭恒
        /// <summary>
        /// 获取未完成销售订单明细 ,未完成工单,未完成采购单,
        /// </summary>
        private void load()
        {

        }






        #endregion



    }
}
