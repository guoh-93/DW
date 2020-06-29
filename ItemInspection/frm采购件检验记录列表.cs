using CPublic;
using CZMaster;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.IO;



namespace ItemInspection
{
    public partial class frm采购件检验记录列表 : UserControl
    {
        #region 类加载
        string strconn = CPublic.Var.strConn;
        public DateTime time1;
        public DateTime time2;
        public string dw = "";

        private DataTable dtCP = null;
        private DataTable dtGYS = null;
        string cfgfilepath = "";

        public frm采购件检验记录列表()
        {
            InitializeComponent();
        }

        private void frm采购件检验记录列表_Load(object sender, EventArgs e)
        {
            fun_Init();
            barCheckItem1_CheckedChanged(null, null);
            if (dw != "")
            {
                barCheckCZY.Checked = true;
                barLargeButtonItem1_ItemClick(null, null);
                //this.gvM.ActiveFilterString = string.Format("gysmc LIKE '%{0}%'", dw);
                this.gvM.ActiveFilterString = string.Format("供应商名称 LIKE '%{0}%'", dw);

            }
        }

        #endregion 类加载

        #region 打印

        private void fun_Print(string JYJG, string JYDDH)
        {
            if (JYJG == "")
            {
                return;
            }
            //if (JYJG == "通过")
            //{
            //    print_Check.fun_print_Check(JYDDH);
            //    return;
            //}
            //if (JYJG == "不通过")
            //{
            print_Check.fun_print_Check(JYDDH);
            //print_Unqualified.fun_print_Unqualified(JYDDH);
            return;
            //}
        }

        #endregion 打印

        private void fun_Init()
        {
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(this, this.Name, cfgfilepath);
            dtEdit1.EditValue = System.DateTime.Today.AddDays(-7);
            dtEdit2.EditValue = System.DateTime.Today.AddDays(1).AddSeconds(-1);
            if (time1.Equals(Convert.ToDateTime("0001/1/1 0:00:00")) == false && time2.Equals(Convert.ToDateTime("0001/1/1 0:00:00")) == false)
            {
                dtEdit1.EditValue = time1;
                dtEdit2.EditValue = time2;
            }
            DataTable dt = new DataTable();
            string s = string.Format("select 职务 from  人事基础员工表 where 员工号='{0}'", CPublic.Var.LocalUserID);
            dt = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            if (dt.Rows[0]["职务"].ToString().Trim() !=""|| CPublic.Var.LocalUserID=="admin"|| CPublic.Var.LocalUserTeam=="公司高管权限")
            {
                barLargeButtonItem12.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }
        }

        private void fun_ReadData()
        {
        }

        private void barCheckItem1_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (barCheckCZY.Checked)
            {
                barCheckCZY.Caption = "显示所有操作员";
            }
            else
            {
                barCheckCZY.Caption = string.Format("操作员:{0} ", CPublic.Var.localUserName);
            }
        }
        DataTable dt_来料检记录;
        private void fun_刷新数据()
        {
            //DateTime t1 = CPublic.Var.getDatetime();
            DateTime time1 =Convert.ToDateTime(dtEdit1.EditValue).Date;
            DateTime time2 = Convert.ToDateTime(dtEdit2.EditValue).Date.AddDays(1).AddSeconds(-1);

            string sll = string.Format("select * from 人事基础员工表 where 员工号 = '{0}'", CPublic.Var.LocalUserID);
            DataTable ttt = new DataTable();
            SqlDataAdapter aaa = new SqlDataAdapter(sll, CPublic.Var.strConn);
            aaa.Fill(ttt);

            string sql = string.Format(@"   select a.*,base.存货分类,base.大类,base.小类,base.物料等级,c.备注2,c.送检日期,d.确认到货日期 from 采购记录采购单检验主表  a
            left join 基础数据物料信息表 base on base.物料编码 = a.产品编号 
            left join 采购记录采购送检单明细表 c on a.送检单明细号 = c.送检单明细号
            left join 采购记录采购送检单主表 d on d.送检单号 = c.送检单号

            where  a.关闭=0 and (a.检验日期 >= '{0}' and a.检验日期 <= '{1}') {2} order by ID", time1,time2, "{0}");
            if (barCheckCZY.Checked == false && CPublic.Var.localUserName != "admin" && (ttt.Rows[0]["职务"].ToString() != "课长" &&
                ttt.Rows[0]["职务"].ToString() != "部长" && ttt.Rows[0]["职务"].ToString() != "副部长" && ttt.Rows[0]["权限组"].ToString() != "品质部主管权限")
                && CPublic.Var.LocalUserTeam=="管理员权限"&& CPublic.Var.LocalUserTeam == "品质部主管权限")
            {
                sql = string.Format(sql, string.Format(" and a.操作员 = '{0}'", CPublic.Var.localUserName));
            }
            else
            {
                sql = string.Format(sql, "");
            }
            dt_来料检记录 = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);

            //if (dtCP == null)
            //{
            //    dtCP = MasterSQL.Get_DataTable("select 物料编码,规格型号,物料名称 from 基础数据物料信息表", CPublic.Var.strConn);
            //    //dtCP = MasterSQL.Get_DataTable("select cpbh,ggxh,cpmc from cp", CPublic.Var.geConn("WL"));
            //}
            //if (dtGYS == null)
            //{
            //    dtGYS = MasterSQL.Get_DataTable("select 供应商ID,供应商名称 from 采购供应商表", CPublic.Var.strConn);
            //    //dtGYS = MasterSQL.Get_DataTable("select gysbh,gysmc from gys", CPublic.Var.geConn("WL"));
            //}
            //CPublic.CConstrFun.fun_数据关联扩展(dt_来料检记录, dtCP, new string[] { "产品编号|物料编码" }, new string[] { "规格型号", "物料名称" });
            //CPublic.CConstrFun.fun_数据关联扩展(dt_来料检记录, dtGYS, new string[] { "供应商编号|供应商ID" }, new string[] { "供应商名称" });
           
            
            //try
            //{
            //    dt.Columns.Add("合格率");
            //    fun_数据合格率(dt);
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            dt_来料检记录.Columns.Add("不合格原因");
            dt_来料检记录.Columns.Add("标记",typeof(int));

            DateTime t = CPublic.Var.getDatetime();
            string s = string.Format(@"select a.检验记录单号,不合格原因,a.不合格数量 from  [采购记录采购单检验明细表] a
    left join 采购记录采购单检验主表 b  on a.检验记录单号=b.检验记录单号  where b.检验日期>'{0}' and b.检验日期<'{1}' and 不合格原因<>''",time1, time2);
            DataTable dt_不合格原因 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = string.Format(@"SELECT  采购入库通知单号,[表单类型],[表单名称] FROM  [检验上传表单记录表]");
            DataTable dt_表单 = CZMaster.MasterSQL.Get_DataTable(s, strconn);



            foreach (DataRow dr in dt_来料检记录.Rows)
            {
                if (dr["检验结果"].ToString() == "不合格")
                {
                    //string s = string.Format("select 不合格原因 from 采购记录采购单检验明细表 where 检验记录单号='{0}' and 不合格原因<>''", dr["检验记录单号"]);
                    //DataTable dt = new DataTable();
                    //dt = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                    DataRow []dt = dt_不合格原因.Select(string.Format("检验记录单号='{0}'", dr["检验记录单号"].ToString()));
                    int i = 1;
                    foreach (DataRow r in dt)
                    {
                        dr["不合格原因"] = dr["不合格原因"].ToString()+" "+i.ToString() + "." + r["不合格原因"].ToString() + "。";
                        i++;
                    }

                }
               //  string ss = string.Format(@"select 表单类型 FROM 检验上传表单记录表 where 采购入库通知单号 ='{0}' ", dr["送检单号"]);
                
                     //DataTable dt1 = new DataTable();
                     //dt1 = CZMaster.MasterSQL.Get_DataTable(ss,strconn);
                    DataRow []dt1= dt_表单.Select(string.Format("采购入库通知单号 ='{0}'", dr["送检单号"]));
                     if (dt1.Length > 0) //有表单
                     {
                         if (dt1[0]["表单类型"].ToString() == "不合格品评审单")
                         {
                             dr["标记"] = 1;  //不合格品评审单
                         }
                         else
                         {
                             dr["标记"] = 2; // 其他单据类型
                         }
                     }
                     else   //无表单
                     {
                         dr["标记"] = 0;

                     }

            }
           
            //DateTime t2 = CPublic.Var.getDatetime();
            gcM.DataSource = dt_来料检记录;


            gvM.ViewCaption = string.Format(gvM.Tag.ToString(), dt_来料检记录.Rows.Count);
        }

        private void fun_数据合格率(DataTable dt)
        {
            foreach (DataRow r in dt.Rows)
            {
                //string sql = string.Format("  select 采购记录采购单明细表.*,基础数据物料信息表.物料名称,基础数据物料信息表.规格型号 from 采购记录采购单明细表,基础数据物料信息表 where  采购记录采购单明细表.物料编码=基础数据物料信息表.物料编码 and  采购单号 = '{0}'", r["采购入库通知单号"].ToString());
                 //5月11号 
                string sql = string.Format("  select 采购记录采购单明细表.*,基础数据物料信息表.物料名称,基础数据物料信息表.规格型号 from 采购记录采购单明细表,基础数据物料信息表 where  采购记录采购单明细表.物料编码=基础数据物料信息表.物料编码 and  采购单号 = '{0}'", r["采购单号"].ToString());
                
                //string sql = string.Format("  select cgrktzdmx.*,cp.cpmc,cp.ggxh from cgrktzdmx,cp where  cgrktzdmx.ylbh=cp.cpbh and  cgrktzdh = '{0}'", r["采购入库通知单号"].ToString());
                DataTable dt_All = MasterSQL.Get_DataTable(sql, Var.strConn);
                int num_All = dt_All.Rows.Count;
                if (num_All == 0)
                {
                    continue;
                }
                int num_OK = 0;
                int num_NO = 0;
                foreach (DataRow rr in dt_All.Rows)
                {
                    sql = string.Format("select * from 采购记录采购单检验主表 where 采购单号='{0}' and 产品编号='{1}'", rr["采购单号"].ToString(), rr["物料编码"].ToString());
                    DataTable dt_mem = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
                    if (dt_mem.Rows.Count > 0)
                    {
                        if (dt_mem.Rows[0]["检验结果"].ToString() == "通过")
                        {
                            num_OK++;
                        }
                        if (dt_mem.Rows[0]["检验结果"].ToString() == "不通过")
                        {
                            num_NO++;
                        }
                    }
                }
                if (num_All != num_OK + num_NO)
                {
                    r["合格率"] = string.Format("还有{0}条没有检测", num_All - (num_OK + num_NO));
                }
                if (num_All == num_OK + num_NO)
                {
                    string num = (((float)num_OK / (float)num_All) * 100).ToString();
                    if (num.IndexOf(".") < 0)
                    {
                        num = num + ".";
                    }
                    num = num.PadRight(5, '0');

                    num = num.Substring(0, num.IndexOf(".") + 3);
                    r["合格率"] = num + "%";
                }
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                
                fun_刷新数据();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
           // string sql = "select 员工号 as 员工编号,姓名 as 员工姓名 from 人事基础员工表";

            string sql = "select 员工号 as 员工编号,姓名 as 员工姓名 from 人事基础员工表 where 部门='品质部'  and 在职状态 ='在职'   ";


            fmDataSelect fm = new fmDataSelect("请选择员工", "员工姓名", sql, CPublic.Var.strConn);
            if (fm.ShowDialog() == DialogResult.OK)
            {
                CPublic.Var.localUserName = fm.strResult;
                LocalDataSetting.addLocalData("APPUser", CPublic.Var.localUserName);
            }
            barCheckItem1_CheckedChanged(null, null);
        }

        private void gcM_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                DataRow dr_1= gvM.GetDataRow(gvM.FocusedRowHandle);
                if (dr_1 != null)
                {

                    frm采购件检验记录 ui = new frm采购件检验记录(dr_1);
                    CPublic.UIcontrol.Showpage(ui, "采购记录明细");
                    //frm采购件检验记录 fm = new frm采购件检验记录();
                    //fm.strJYDDH = dr_1["检验记录单号"].ToString();
                    //fm.strCPBH = dr_1["产品编号"].ToString();
                    //CPublic.UIcontrol.AddNewPage(fm, string.Format("采购件检验记录[{0}]", fm.strJYDDH));
                }
            }
            catch (Exception)
            {
                
                throw;
            }
                    
           
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                DataRow rm = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;

                if (MessageBox.Show("请问是否打印检验记录单号：" + rm["检验记录单号"].ToString() + "所在当前行？", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    //fun_Print(rm["检验结果"].ToString(), rm["检验记录单号"].ToString());  



                    DataTable dt = new System.Data.DataTable();
                    string sqlstr = "SELECT [检验记录单号],[产品编号] ,[供应商编号],[送检单号],[检验日期],[送检数量],[抽检数量],[检验员],[检验结果],[不合格数量] FROM 采购记录采购单检验主表 WHERE [检验记录单号]='{0}'";
                    sqlstr = string.Format(sqlstr, rm["检验记录单号"].ToString());

                    new SqlDataAdapter(sqlstr, CPublic.Var.strConn).Fill(dt);
                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("没有找到这个检验记录单号");
                    }




                    System.Data.DataTable dt_circulation = new System.Data.DataTable();


                    sqlstr = "SELECT[POS],[检验项目],[检验要求],[抽检数],[扩大值],[检验下限],[检验上限],[允许下限],[允许上限],[合格] ,[不合格原因],[不合格数量] FROM 采购记录采购单检验明细表 where [检验记录单号]='{0}'order by [POS]";
                    sqlstr = string.Format(sqlstr, rm["检验记录单号"].ToString());
                    new SqlDataAdapter(sqlstr, CPublic.Var.strConn).Fill(dt_circulation);

                    if (dt_circulation.Rows.Count == 0)
                    {
                        throw new Exception("该条无检验记录");
                    }


                    Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
                    Type outerForm = outerAsm.GetType("ERPreport.采购检验单", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统

                    object[] drr = new object[3];
                    drr[0] = dt;
                    drr[1] = rm;
                    drr[2] = dt_circulation;
                    Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                    //  UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
                    ui.ShowDialog();



                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }









            //string strDefaultPrinter = new PrintDocument().PrinterSettings.PrinterName;
            //try
            //{
            //    DataRow rm = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;

            //    if (MessageBox.Show("请问是否打印检验记录单号：" + rm["检验记录单号"].ToString() + "所在当前行？", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
            //    {

            //        //PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();

            //        //printDialog1.Document = this.printDocument;
            //        //DialogResult dr = printDialog1.ShowDialog();
            //        if (DialogResult.OK == MessageBox.Show(strDefaultPrinter, "打印机确认？", MessageBoxButtons.OKCancel))
            //        {
            //            fun_Print(rm["检验结果"].ToString(), rm["检验记录单号"].ToString());
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow rm = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;
                //if (MessageBox.Show("请问是否导出检验记录单号：" + rm["检验记录单号"].ToString() + "所在当前行到Excel？", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                //{
                //print_Check.fun_print_Check_ToExcel(rm["检验记录单号"].ToString(), true);
                //}
                if (MessageBox.Show("请问是否导出检验记录单号：" + rm["检验记录单号"].ToString() + "所在当前行到Excel？", "提示", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    return;
                }

                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "目标位置";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string foldPath = dialog.SelectedPath;

                    string fileName = foldPath + "\\" + rm["检验记录单号"].ToString() + ".xlsx";

                    System.IO.Directory.CreateDirectory(foldPath);

                    if (System.IO.File.Exists(fileName) == true)
                    {
                        if (MessageBox.Show("文件已存在是否覆盖", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            print_Check.fun_print_Check_ToExcel(rm["检验记录单号"].ToString(), fileName, true);
                        }
                    }
                    if (System.IO.File.Exists(fileName) == false)
                    {
                        print_Check.fun_print_Check_ToExcel(rm["检验记录单号"].ToString(), fileName, true);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 查看明细
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>E:\新建文件夹\ItemInspection\ui成品检验不良现象分析.cs
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                frm采购件检验记录 fm = new frm采购件检验记录();
                try
                {
                    fm.strJYDDH = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row["检验记录单号"].ToString();
                    fm.strCPBH = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row["产品编号"].ToString();
                }
                catch (Exception)
                {
                    throw new Exception("请选择一行数据");
                }

                CPublic.UIcontrol.AddNewPage(fm, string.Format("采购件检验记录[{0}]", fm.strJYDDH));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 表单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow rm = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;
                using (fm表单 fm = new fm表单())
                {
                    //fm.str_送检单 = rm["采购入库通知单号"].ToString();
                    fm.strTZJno = rm["送检单号"].ToString();

                    fm.strCPBM = rm["产品编号"].ToString();
                    if (fm.ShowDialog() != DialogResult.OK)
                    {
                        fm.Close();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gvM_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
            dr["关闭"] = true;
            string sql = "select * from 采购记录采购单检验主表 where 1<>1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dt_来料检记录);
        }
        //
        private void gvM_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (gvM.GetRow(e.RowHandle) == null)
                {
                    return;
                }

//                if (gvM.GetRowCellValue(e.RowHandle, "检验结果").ToString() == "不合格")
//                {

//                    string sql = string.Format(@"select 检验上传表单记录表.* FROM 检验上传表单记录表,[采购记录采购单检验主表] where 检验上传表单记录表.采购入库通知单号=
//                                       [采购记录采购单检验主表].送检单号 and 表单类型='不合格品评审单' and 检验记录单号='{0}'",gvM.GetDataRow(e.RowHandle)["检验记录单号"].ToString());
//                    DataTable dt = new DataTable();
//                    dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
//                    if (dt.Rows.Count> 0)
//                    {
//                        e.Appearance.BackColor = Color.Red;
                        
//                    }
//                }
                int  x=  Convert.ToInt32(gvM.GetRowCellValue(e.RowHandle, "标记"));
                if (x>0)
                {
                   if(x>1)      //其他表单类型 
                   {
                       e.Appearance.BackColor = Color.BurlyWood;
                   }
                   else   //   不合格品评审单
                   {
                        e.Appearance.BackColor = Color.Red;
                   }
                    
                }
                
            }
            catch
            {

            }
                
                
        }

        private void barLargeButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
            if (dr != null)
            {
                DataTable dt_dy = dt_来料检记录.Clone();
                dt_dy.ImportRow(dr);
                //   ItemInspection.print_FMS.fun_P_采购入库通知单(dt_dy);

                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
                Type outerForm = outerAsm.GetType("ERPreport.来料入", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统

                object[] drr = new object[1];
                drr[0] = dt_dy;

                Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                //  UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
                
                ui.ShowDialog();

            }


            //if (MessageBox.Show("是否打印当前选中行", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
            //{
            //    Thread thDo;
            //    thDo = new Thread(Dowork);
            //    //Dowork();
            //    thDo.IsBackground = true;
            //    thDo.Start();
            //}
        }
        public void Dowork()
        {
            DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
            if (dr != null)
            {
                DataTable dt_dy = dt_来料检记录.Clone();
                dt_dy.ImportRow(dr);
                ItemInspection.print_FMS.fun_P_采购入库通知单(dt_dy);
            }
        }

        private void barLargeButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                gcM.ExportToXlsx(saveFileDialog.FileName);

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //撤回未检验状态
        private void barLargeButtonItem12_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                string str = "";
                str = dr["送检单号"].ToString().Substring(0,2);
                if (str.ToString() != "SJ")
                {
                    throw new Exception("该单据不支持撤回！若需撤回，请联系信息部");
                }

                
                string sql = string.Format("select  * from  采购记录采购单检验主表  where 检验记录单号='{0}'", dr["检验记录单号"].ToString().Trim());
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToDecimal(dt.Rows[0]["已入库数"])==0)
                    {
                        if (MessageBox.Show(string.Format("确认撤销该条检验记录？"), "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            //if (dr != null)
                            //{
                            string sql_zb = string.Format("select  * from 采购记录采购单检验主表 where  检验记录单号='{0}'", dr["检验记录单号"]);
                           

                            DataTable dt_zb = new DataTable();
                            
                            dt_zb = CZMaster.MasterSQL.Get_DataTable(sql_zb, CPublic.Var.strConn);
                          
                            if (dt_zb.Rows.Count > 0)
                            {
                                dt_zb.Rows[0]["关闭"]=1;
                            }
                            //到货类型的送检单对应的检验单
                            string sql_送检明细 = string.Format("select  * from  采购记录采购送检单明细表  where 送检单明细号 ='{0}' ", dr["送检单明细号"]);
                            DataTable dt_送检明细 = new DataTable();
                            dt_送检明细 = CZMaster.MasterSQL.Get_DataTable(sql_送检明细, CPublic.Var.strConn);
                            DataTable dt_caigou = new DataTable();
                            if (dt_送检明细.Rows.Count > 0)
                            {
                                dt_送检明细.Rows[0]["检验完成"] = 0;
                                dt_送检明细.Rows[0]["已检验数"] = Convert.ToDecimal(dt_送检明细.Rows[0]["已检验数"]) - Convert.ToDecimal(dr["送检数量"]);
                                if (Convert.ToDecimal(dr["不合格数量"]) > 0)
                                {
                                    dt_送检明细.Rows[0]["已拒收数"] = Convert.ToDecimal(dt_送检明细.Rows[0]["已拒收数"]) - Convert.ToDecimal(dr["不合格数量"]);
                                    string sql_拒收 = string.Format(@"select top 1 * from 采购记录采购送检单明细表 where 送检单号= '{0}' and POS>1  and 送检单类型 = '拒收' and -送检数量 = '{1}'  order by POS desc ", dr["送检单号"],dr["不合格数量"]);//双保险
                                    SqlDataAdapter da = new SqlDataAdapter(sql_拒收, strconn);
                                    da.Fill(dt_送检明细);
                                    dt_送检明细.Rows[dt_送检明细.Rows.Count - 1].Delete();
                                    string sql_采购 = string.Format("select * from 采购记录采购单明细表 where 采购明细号 = '{0}'", dr["采购明细号"]);
                                    dt_caigou = CZMaster.MasterSQL.Get_DataTable(sql_采购, strconn);
                                    if (dt_caigou.Rows.Count > 0)
                                    {
                                        dt_caigou.Rows[0]["已送检数"] = Convert.ToDecimal(dt_caigou.Rows[0]["已送检数"]) + Convert.ToDecimal(dr["不合格数量"]);
                                    }
                                }
                               // dt_送检明细.Rows[0]["已检验数"] = Convert.ToDecimal(dt_送检明细.Rows[0]["已检验数"]) - Convert.ToDecimal(dr["送检数量"]);
                                
                            }

                            SqlConnection conn = new SqlConnection(strconn);
                            conn.Open();
                            SqlTransaction ts = conn.BeginTransaction("撤销检验");
                            try
                            {
                                string sql_1 = "select  * from 采购记录采购送检单明细表 where 1<>1 ";
                                SqlCommand cmm_1 = new SqlCommand(sql_1, conn, ts);
                                string sql_2 = "select * from 采购记录采购单检验主表 where 1<>1";
                                SqlCommand cmm_2= new SqlCommand(sql_2, conn, ts);
                                 
                                    
                                SqlDataAdapter da_1 = new SqlDataAdapter(cmm_1);
                                SqlDataAdapter da_2 = new SqlDataAdapter(cmm_2);
                         
                                new SqlCommandBuilder(da_1);
                                new SqlCommandBuilder(da_2);

                                da_1.Update(dt_送检明细);
                                da_2.Update(dt_zb);

                                if (Convert.ToDecimal(dr["不合格数量"]) > 0)
                                {
                                    string sql_3 = "select * from 采购记录采购单明细表 where 1<>1";
                                    SqlCommand cmm_3 = new SqlCommand(sql_3, conn, ts);
                                    SqlDataAdapter da_3 = new SqlDataAdapter(cmm_3);
                                    new SqlCommandBuilder(da_3);
                                    da_3.Update(dt_caigou);
                                }

                                ts.Commit();
                                dt_来料检记录.Rows.Remove(dr);                                


                            }
                            catch (Exception ex)
                            {
                                ts.Rollback();
                                throw new Exception("保存失败,刷新后重试");
                            }





                            MessageBox.Show("已撤销");
 
                        }
                    }
                    else
                    {
                        throw new Exception("这条记录已有入库记录");
                    }

                }
                else
                {
                    throw new Exception("未找到该送检单，请确认？");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem13_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        



        }

        private void barLargeButtonItem14_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
         
        }
            
    }
}