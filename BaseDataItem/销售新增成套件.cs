using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using System.IO;
using DevExpress.XtraTreeList.Nodes;

namespace BaseData
{
    public partial class 销售新增成套件 : UserControl
    {
        #region 成员

        DataTable dtM;
        int strNo = 0;
        SqlDataAdapter da;

        string cfgfilepath = "";
        string strshow;

        DataView dv;
        DataTable dt_车间;
        DataTable dt_属性;
        DataTable dt_单位;

        string str_新增or修改 = "";
        DataTable dt_保存修改 = null;

        string str_物料编码 = "";
        string str_物料名称 = "";
        string str_规格 = "";
        string str_原规格型号 = "";

        string strconn = CPublic.Var.strConn;
        #endregion


        public 销售新增成套件()
        {
            InitializeComponent();
        }

        string a_存货分类编码 = "";
        public static class aaaa
        {
            public static List<销售成套件BOM> FM2 = new List<销售成套件BOM>();

            public static void fun_(string str, string strr, string strrr)
            {
                foreach (销售成套件BOM fm in FM2)
                {
                    fm.str_物料编码 = str;
                    fm.str_物料名称 = strr;
                    fm.str_规格 = strrr;

                    fm.fun_载入数据();

                }
            }
        }

        private void 销售新增成套件_Load(object sender, EventArgs e)
        {
            try
            {
                barLargeButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                fun_载入刷新();
                BeginInvoke(new MethodInvoker(() =>
                {
                    fun_下拉框();
                    fun_下拉框searchlookup();
                    销售成套件BOM fm = new 销售成套件BOM();
                    fm.Dock = System.Windows.Forms.DockStyle.Fill;
                    //xtra.SelectedTabPage = xtraTabPage4;
                    tabPage3.Controls.Add(fm);
                    //CZMaster.DevGridControlHelper.Helper(this);
                    fm.fun_载入数据();
                    tabControl1.SelectedTab = tabPage3;
                    tabControl1.SelectedTab = tabPage1;

                    销售成套件BOM.STC = this.tabControl1;
                    //fun_载入数据(); //基础数据界面  用于快速选择数据

                }));





            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
            }
        }

        private void fun_下拉框()
        {

            cb_扫描方式.Items.Clear();
            cb_锁芯.Items.Clear();
            string sql = "select * from 基础数据基础属性表 order by 属性类别,属性值";
            dt_属性 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_属性);
            foreach (DataRow r in dt_属性.Rows)
            {
                if (r["属性类别"].ToString().Equals("扫描方式"))
                {
                    cb_扫描方式.Items.Add(r["属性值"].ToString());
                }
                if (r["属性类别"].ToString().Equals("锁芯"))
                {
                    cb_锁芯.Items.Add(r["属性值"].ToString());
                }
            }

            sql = "select 属性值 as 计量单位,属性字段1 as 计量单位编码 from 基础数据基础属性表 where 属性类别 = '计量单位'";
            da = new SqlDataAdapter(sql, strconn);
            dt_单位 = new DataTable();
            da.Fill(dt_单位);

            searchLookUpEdit1.Properties.DataSource = dt_单位;
            searchLookUpEdit1.Properties.DisplayMember = "计量单位编码";
            searchLookUpEdit1.Properties.ValueMember = "计量单位编码";
        }

        private void fun_下拉框searchlookup()
        {
            //车间
            string sql = "select 属性字段1 as 部门编号,属性值 as 部门名称 from  基础数据基础属性表  where 属性类别 = '生产车间' order by 部门编号";
            dt_车间 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_车间);

            cb_车间编号.Properties.DataSource = dt_车间;
            cb_车间编号.Properties.DisplayMember = "部门编号";
            cb_车间编号.Properties.ValueMember = "部门编号";
            //仓库
            sql = @"select 属性字段1 as 仓库编号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 = '仓库类别'order by 仓库编号 ";
            DataTable dt_仓库 = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_仓库);
            cb_仓库编号.Properties.DataSource = dt_仓库;
            cb_仓库编号.Properties.DisplayMember = "仓库编号";
            cb_仓库编号.Properties.ValueMember = "仓库编号";

            //滑盖颜色
            sql = @"select 属性字段1 as  滑盖颜色,属性值 as 颜色说明 from 基础数据基础属性表 where 属性类别 = '滑盖颜色'order by 滑盖颜色 ";
            DataTable dt_hgcol = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_hgcol);
            sl_滑盖颜色.Properties.DataSource = dt_hgcol;
            sl_滑盖颜色.Properties.DisplayMember = "滑盖颜色";
            sl_滑盖颜色.Properties.ValueMember = "滑盖颜色";

            //壳体颜色
            sql = @"select 属性字段1 as  壳体颜色,属性值 as 颜色说明 from 基础数据基础属性表 where 属性类别 = '壳体颜色'order by 壳体颜色 ";
            DataTable dt_ktcol = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_ktcol);
            sl_壳体颜色.Properties.DataSource = dt_ktcol;
            sl_壳体颜色.Properties.DisplayMember = "壳体颜色";
            sl_壳体颜色.Properties.ValueMember = "壳体颜色";
        }

        private void fun_载入刷新()
        {
            try
            {
                Thread th = new Thread(fun_N_加载数据);
                th.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_N_加载数据()
        {
            try
            {
                string s = "select * from  基础数据物料信息表 where left(存货分类编码,2) = 10";
                dtM = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(s, strconn))
                {
                    da.Fill(dtM);
                }
                method(gcc, gd => fun_界面设置());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_界面设置()
        {
            string s = "select  *  from 基础数据存货分类表 where left(存货分类编码,2)=10 order by   存货分类编码 ";
            DataTable tt = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            treeList1.OptionsBehavior.PopulateServiceColumns = true;
            treeList1.KeyFieldName = "GUID";
            treeList1.ParentFieldName = "上级类型GUID";
            treeList1.DataSource = tt;
            treeList1.ExpandAll();

            gcc.DataSource = dtM;
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(tabControl1, this.Name, cfgfilepath);
        }

        private void method<T>(T c, Action<T> action) where T : DevExpress.XtraGrid.GridControl
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => action(c)));
            }
            else
                action(c);
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void treeList1_MouseClick(object sender, MouseEventArgs e)
        {
            if (treeList1.Nodes.Count > 0)
            {
                if (treeList1.Selection[0] == null) return;
            }
            else
            {
                return;
            }
            TreeListNode n = treeList1.Selection[0];
            string s = n.GetValue("存货分类编码").ToString();
            DataView v = new DataView(dtM);
            v.RowFilter = String.Format("存货分类编码 like '{0}%'", s);
            gridControl1.DataSource = v;
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barLargeButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            if (e.TabPage.Name == "tabPage2")
            {
                barLargeButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }
            else if (e.TabPage.Name == "tabPage1")
            {
                barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (treeList1.Nodes.Count > 0)
                {
                    if (treeList1.Selection[0] == null) return;
                }
                else
                {
                    return;
                }


                TreeListNode n = treeList1.Selection[0];
                if (n.HasChildren) throw new Exception("此分类还有子级分类,不可在此分类下新增物料");
                fun_新增();
                txt_分类编码.Text = n.GetValue("存货分类编码").ToString();
                txt_存货分类.Text = n.GetValue("存货分类名称").ToString();
                tabControl1.SelectedTab = tabPage2;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void fun_新增()
        {
            fun_清空数据();
            strNo = 1;
        }

        private void fun_清空数据()
        {
            cb_锁芯.Text = "";
            cb_扫描方式.Text = "";
            txt_物料编码.Text = "";
            tb3_物料名称.Text = "";
            txt_分类编码.Text = "";
            txt_存货分类.Text = "";
            cb4_规格.Text = "";
            cb_仓库编号.Text = "";
            txt_车间.Text = "";
            cangkumiaoshu.Text = "";
            cb_车间编号.EditValue = null;
            sl_壳体颜色.EditValue = null;
            sl_滑盖颜色.EditValue = null;
            searchLookUpEdit1.EditValue = null;
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //DataRow dr = dtM.NewRow();
            try
            {
                DataTable init = new DataTable();


                bool bl = fun_check();
                DateTime time= CPublic.Var.getDatetime(); 
                if (txt_物料编码.Text == "")
                {
                    string x = txt_分类编码.Text;
                    string s = string.Format(@"select  max(物料编码)物料编码 from 基础数据物料信息表 where 存货分类编码='{0}'", x);
                    x = x.PadRight(10, '0');
                    DataTable temp = new DataTable();
                    temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                    if (temp.Rows.Count == 0 || temp.Rows[0]["物料编码"].ToString() == "") x = x + "0001";
                    else
                    {
                        s = temp.Rows[0]["物料编码"].ToString();
                        s = (Convert.ToInt32(temp.Rows[0]["物料编码"].ToString().Substring(10, 4)) + 1).ToString().PadLeft(4, '0');
                        x = x + s;
                    }
                    txt_物料编码.Text = x;

                }
                if (txt_物料编码.Text == "")
                {
                    MessageBox.Show("请先输入物料编码！");
                }
                else
                {


                    if (cb_车间编号.EditValue == null || cb_车间编号.EditValue.ToString() == "")
                    {
                        cb_车间编号.EditValue = "";
                    }
                    //处理dtm
                    init=fun_基础属性保存();
                    MessageBox.Show(strshow);

                }
                string sql = "select * from 基础数据物料信息修改日志表 where 1<>1";
                DataTable dtttt = new DataTable();
                SqlDataAdapter daaaa = new SqlDataAdapter(sql, strconn);
                daaaa.Fill(dtttt);
           
                DataRow drrrr = dtttt.NewRow();
                dtttt.Rows.Add(drrrr);
                if (str_新增or修改 == "修改")
                {
                    DataRow[] ds = dtM.Select(string.Format("物料编码 = '{0}'", txt_物料编码.Text));
                    int i = ds.Length;
                    //dr.ItemArray = ds[0].ItemArray;
                    string str_修改内容 = "修改了：";
                    //DataTable dt_保存修改
                    foreach (DataColumn dc in dt_保存修改.Columns)
                    {
                        string str1 = dt_保存修改.Rows[0][dc.Caption].ToString();
                        string str2 = dt_保存修改.Rows[0][dc.Caption,DataRowVersion.Original].ToString();
                        if (str1 != str2)
                        {
                            str_修改内容 = str_修改内容 + dc.Caption + "的值，" + "原：" + str2 + "，现：" + str1 + "；";
                        }
                    }
                    drrrr["GUID"] = System.Guid.NewGuid();
                    drrrr["姓名"] = CPublic.Var.localUserName;
                    drrrr["员工号"] = CPublic.Var.LocalUserID;
                    drrrr["日期"] = time;
                    drrrr["内容"] = str_修改内容;
                    drrrr["物料编码"] = txt_物料编码.Text;
                    //MessageBox.Show(str_修改内容);
                }
                else
                {
                    drrrr["GUID"] = System.Guid.NewGuid();
                    drrrr["姓名"] = CPublic.Var.localUserName;
                    drrrr["员工号"] = CPublic.Var.LocalUserID;
                    drrrr["日期"] = time;
                    drrrr["内容"] = "新增物料：" + txt_物料编码.Text;
                    drrrr["物料编码"] = txt_物料编码.Text;
                }

                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("成套件");
                try
                {
                    SqlCommand cmm = new SqlCommand("select * from 基础数据物料信息表 where 1<>1", conn, ts);
                    SqlDataAdapter da = new SqlDataAdapter(cmm);
                    new SqlCommandBuilder(da);
                    da.Update(dtM);
                    cmm = new SqlCommand(sql, conn, ts);
                    da = new SqlDataAdapter(cmm);
                    new SqlCommandBuilder(da);
                    da.Update(dtttt);
                    if(init!=null && init.Columns.Count>0)
                    {
                        cmm = new SqlCommand("select * from 仓库物料数量表 where 1=2", conn, ts);
                        da = new SqlDataAdapter(cmm);
                        new SqlCommandBuilder(da);
                        da.Update(init);
                    }

                    ts.Commit();
                    if (txt_物料编码.Text != "" && str_新增or修改 == "修改")
                    {
                        sql = string.Format("select  * from 基础数据物料信息表 where 物料编码='{0}'", txt_物料编码.Text);
                        DataTable dt = new DataTable();
                        dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                        DataRow[] r_1 = dtM.Select(string.Format("物料编码='{0}'", txt_物料编码.Text));
                        r_1[0].ItemArray = dt.Rows[0].ItemArray;
                    }
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

        private DataTable  fun_基础属性保存()
        {
            try
            {
                DataTable dt_init = new DataTable();
              
                DateTime time1 = CPublic.Var.getDatetime();
                if (strNo == 1 || strNo == 0)  //0为初始状态，1为新增状态
                {
                    str_新增or修改 = "新增";
                    dtM.AcceptChanges();
                    DataRow dr = dtM.NewRow();
                    dr["物料编码"] = txt_物料编码.Text;
                    dr["物料编码"] = txt_物料编码.Text;  // 物料编码 与物料编码一致即可  tb2.Text
                    dr["物料名称"] = tb3_物料名称.Text;
                    dr["存货分类"] = txt_存货分类.Text;
                    dr["存货分类编码"] = txt_分类编码.Text;
                    dr["规格"] = cb4_规格.Text;
                    dr["n原ERP规格型号"] = dr["规格型号"] = cb4_规格.Text;
                    dr["锁芯"] = cb_锁芯.Text; // 东屋 细分功能结构代码 改为 锁芯                  
                    if (sl_滑盖颜色.EditValue != null && sl_滑盖颜色.EditValue.ToString() != "")
                    {
                        dr["滑盖颜色"] = sl_滑盖颜色.EditValue.ToString();
                    }
                    if (sl_壳体颜色.EditValue != null && sl_壳体颜色.EditValue.ToString() != "")
                    {
                        dr["壳体颜色"] = sl_壳体颜色.EditValue.ToString();
                    }
                    dr["扫描方式"] = cb_扫描方式.Text; //东屋 功能类别改为 扫描方式
                    dr["有无蓝牙"] = chkBx_蓝牙.Checked;
                    dr["计量单位编码"] = searchLookUpEdit1.EditValue;
                    dr["计量单位"] = textBox2.Text;
                    dr["新数据"] = true;
                    dr["仓库号"] = cb_仓库编号.Text;
                    dr["仓库名称"] = cangkumiaoshu.Text;
                    dr["车间"] = txt_车间.Text;
                    dr["修改人"] = CPublic.Var.localUserName;
                    dr["修改人ID"] = CPublic.Var.LocalUserID;
                    dr["修改日期"] = time1;
                    dr["是否初始化"] = "是";
                    try
                    {
                        dr["车间编号"] = cb_车间编号.EditValue;
                    }
                    catch { }
                    dtM.Rows.InsertAt(dr,0);
                  //  da = new SqlDataAdapter("select * from 基础数据物料信息表 where 1<>1", strconn);
                   // new SqlCommandBuilder(da);
                  //  da.Update(dtM);

                    strshow = string.Format("物料编码为{0}的基础数据新增成功！", txt_物料编码.Text);
                    strNo = 2;  //新增后可以立即修改
                    txt_物料编码.ReadOnly = true;
                    dt_init= StockCore.StockCorer.Init_stock(dr);
                          
                       // }

                       // new SqlCommandBuilder(daa);
                        //daa.Update(dtt);

                    }
                 
                else if (strNo == 2)  //2为修改状态
                {
                    str_新增or修改 = "修改";
                   // dt_保存修改 = new DataTable();
                    //string sql_保存修改 = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", txt_物料编码.Text);
                    //SqlDataAdapter da_保存修改 = new SqlDataAdapter(sql_保存修改, strconn);
                    //da_保存修改.Fill(dt_保存修改);
                    DataRow [] Mod_r = dtM.Select(string.Format("物料编码='{0}'", txt_物料编码.Text));
                    if (Mod_r.Length > 0)
                    {
                        Mod_r[0]["物料编码"] = dt_保存修改.Rows[0]["物料编码"] = txt_物料编码.Text;
                        Mod_r[0]["物料名称"] = tb3_物料名称.Text;
                        Mod_r[0]["存货分类"] = txt_存货分类.Text;
                        Mod_r[0]["存货分类编码"] = txt_分类编码.Text;
                        Mod_r[0]["规格"] = cb4_规格.Text;
                        Mod_r[0]["规格型号"] = cb4_规格.Text;
                        Mod_r[0]["锁芯"] = cb_锁芯.Text;
                        Mod_r[0]["扫描方式"] = cb_扫描方式.Text;
                        Mod_r[0]["滑盖颜色"] = sl_滑盖颜色.Text.ToString();
                        Mod_r[0]["壳体颜色"] = sl_壳体颜色.Text.ToString();
                        Mod_r[0]["有无蓝牙"] = chkBx_蓝牙.Checked;
                        Mod_r[0]["计量单位"] = textBox2.Text;
                        Mod_r[0]["计量单位编码"] = searchLookUpEdit1.EditValue;
                        Mod_r[0]["新数据"] = true;
                        Mod_r[0]["仓库号"] = cb_仓库编号.Text;
                        Mod_r[0]["仓库名称"] = cangkumiaoshu.Text;
                        Mod_r[0]["原规格型号"] = dt_保存修改.Rows[0]["n原ERP规格型号"] = dt_保存修改.Rows[0]["规格"];
                        Mod_r[0]["车间"] = txt_车间.Text;
                        Mod_r[0]["修改人"] = CPublic.Var.localUserName;
                        Mod_r[0]["修改人ID"] = CPublic.Var.LocalUserID;
                        Mod_r[0]["修改日期"] = time1;
                    }
                    try
                    {

                        Mod_r[0]["车间编号"] = cb_车间编号.EditValue;

                    }
                    catch { }
                    //new SqlCommandBuilder(da_保存修改);
                    //da_保存修改.Update(dt_保存修改);
                    strshow = string.Format("物料编码为{0}的基础数据修改成功！", txt_物料编码.Text);
                }
                //17-10-9 成品半成品 关联 计划员
                return dt_init;
            
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private Boolean fun_check()
        {
            try
            {
                if (cb_车间编号.EditValue == null || cb_车间编号.EditValue.ToString() == "")
                {
                    throw new Exception("车间编号不能为空");
                }
                if (cb_仓库编号.EditValue == null || cb_仓库编号.EditValue.ToString() == "")
                {
                    throw new Exception("仓库编号不能为空");
                }
                if (tb3_物料名称.Text == "")
                {
                    throw new Exception("物料名称不能为空");
                }
                if (txt_存货分类.Text == "" || txt_分类编码.Text == "")
                {
                    throw new Exception("物料名称不能为空");
                }
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("计量单位不能为空");
                }
                if (cb4_规格.Text == "")
                {
                    //strshow = "型号不能为空！"; tb4_规格型号.Focus();
                    //return false;
                    throw new Exception("型号不能为空");

                }
                if (cb4_规格.Text.ToString() != "")
                {
                    string sql = string.Format("select 物料编码,规格 from 基础数据物料信息表 where 规格='{0}' and 物料编码 <>'{1}'", cb4_规格.Text, txt_物料编码.Text);
                    DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt.Rows.Count > 0)
                    {
                        //strshow = "已有重复规格";
                        //return false;
                        throw new Exception("已有重复规格");

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                strshow = ex.Message;
                throw new Exception(ex.Message);
            }
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
                {
                    DataRow[] r = dt_单位.Select(string.Format("计量单位编码='{0}'", searchLookUpEdit1.EditValue));
                    if (r.Length > 0) textBox2.Text = r[0]["计量单位"].ToString();
                    else textBox2.Text = "";

                }
                else
                {

                    textBox2.Text = "";
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void cb_仓库编号_EditValueChanged(object sender, EventArgs e)
        {
            if (cb_仓库编号.EditValue != null && cb_仓库编号.EditValue.ToString() != "")
            {
                //选完仓库编号后，显示仓库名称
                DataRow[] ds = dt_属性.Select(string.Format("属性字段1 = '{0}'", cb_仓库编号.EditValue.ToString()));
                if (ds.Length > 0)
                {
                    cangkumiaoshu.Text = ds[0]["属性值"].ToString();
                }
            }
            else
            {
                cangkumiaoshu.Text = "";
            }
        }

        private void cb_车间编号_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_车间编号.EditValue != null && cb_车间编号.EditValue.ToString() != "")
                {
                    DataRow[] ds = dt_车间.Select(string.Format("部门编号 = '{0}'", cb_车间编号.EditValue.ToString()));
                    if (ds.Length > 0)
                    {
                        txt_车间.Text = ds[0]["部门名称"].ToString();
                    }
                }
                else
                {
                    txt_车间.Text = "";

                }
            }
            catch { }
        }

        private void 跳转BOM信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                销售成套件BOM.STC = this.tabControl1;
                DataRow dr = gvv.GetDataRow(gvv.FocusedRowHandle);
                str_物料编码 = dr["物料编码"].ToString();
                str_物料名称 = dr["物料名称"].ToString();
                str_规格 = dr["规格"].ToString();
                str_原规格型号 = dr["规格型号"].ToString();
                aaaa.fun_(str_物料编码, str_物料名称, str_规格);

                tabControl1.SelectedTab = tabPage3;
            }
            catch { }
        }

        private void gridView2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {

            DataRow rrr = gvv.GetDataRow(e.RowHandle);
            txt_物料编码.Text = rrr["物料编码"].ToString();
            txt_物料编码.ReadOnly = true;
            tb3_物料名称.Text = rrr["物料名称"].ToString();
            cb4_规格.Text = rrr["规格型号"].ToString();
            txt_存货分类.Text = rrr["存货分类"].ToString();
            txt_分类编码.Text = rrr["存货分类编码"].ToString();
            cb_锁芯.Text = rrr["锁芯"].ToString();
            sl_壳体颜色.EditValue = rrr["壳体颜色"].ToString();
            sl_滑盖颜色.EditValue = rrr["滑盖颜色"].ToString();
            chkBx_蓝牙.Checked = Convert.ToBoolean(rrr["有无蓝牙"]);
            cb_扫描方式.Text = rrr["扫描方式"].ToString();
            searchLookUpEdit1.EditValue = rrr["计量单位编码"].ToString();
            cb_仓库编号.Text = rrr["仓库号"].ToString();     //
            cangkumiaoshu.Text = rrr["仓库名称"].ToString();
            cb_车间编号.EditValue = rrr["车间编号"].ToString();
            strNo = 2;

            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gcc, new Point(e.X, e.Y));
            }

        }
        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (txt_物料编码.Text != "")
            {
                string sql = string.Format("select  * from 基础数据物料信息表 where 物料编码='{0}'", txt_物料编码.Text);
                DataTable dt = new DataTable();
                dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                DataRow[] r_1 = dtM.Select(string.Format("物料编码='{0}'", txt_物料编码.Text));
                r_1[0].ItemArray = dt.Rows[0].ItemArray;
            }
            //checkBox11.Checked = false;

            fun_新增();
            a_存货分类编码 = "";
            fun_载入数据();
        }

        private void fun_载入数据()
        {
            dtM = new DataTable();
            //string sql = "select * from 基础数据物料信息表";
            string sql = @"select base.*,a.版本 as sop版本 from 基础数据物料信息表 base 
                            left  join (select 类别名称,max(版本) as 版本 from 作业指导书文件表 group by 类别名称) a on base.物料编码=a.类别名称   ";  //以后只显示审核过的数据 7.28
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);

            //当修改过数据保存后，新数据自动消失，点全部时，查看所有数据 //用于显示旧数据 16.7.28 18-3新数据用于表示 所有新增的物料
            dv = new DataView(dtM);
            dv.RowFilter = "新数据 = 0";
            if (a_存货分类编码 != "")
            {
                dv.RowFilter = string.Format("存货分类编码 ='{0}'", a_存货分类编码);

            }
            gcc.DataSource = dv;

        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Clicks == 2)
                {
                    DataRow rrr = gridView1.GetDataRow(e.RowHandle);

                    txt_物料编码.Text = rrr["物料编码"].ToString();
                    txt_物料编码.ReadOnly = true;

                    tb3_物料名称.Text = rrr["物料名称"].ToString();


                    txt_存货分类.Text = rrr["存货分类"].ToString();
                    txt_分类编码.Text = rrr["存货分类编码"].ToString();


                    cb_锁芯.Text = rrr["锁芯"].ToString();

                    sl_壳体颜色.EditValue = rrr["壳体颜色"].ToString();
                    sl_滑盖颜色.EditValue = rrr["滑盖颜色"].ToString();
                    chkBx_蓝牙.Checked = Convert.ToBoolean(rrr["有无蓝牙"]);
                    //cb_保护特性.Text = rrr["保护特性"].ToString();
                    //cb_断路器型号.Text = rrr["断路器型号"].ToString();
                    //cb_漏电.Text = rrr["漏电"].ToString();
                    cb_扫描方式.Text = rrr["扫描方式"].ToString();
                    searchLookUpEdit1.EditValue = rrr["计量单位编码"].ToString();
                    //  textBox2.Text = rrr["计量单位"].ToString();
                    txt_车间.Text = rrr["车间"].ToString();
                    cb_仓库编号.Text = rrr["仓库号"].ToString();     //
                    cangkumiaoshu.Text = rrr["仓库名称"].ToString();     //                    
                    cb_车间编号.EditValue = rrr["车间编号"].ToString();


                    tabControl1.SelectedTab = tabPage2;
                    str_新增or修改 = "修改";

                    strNo = 2;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
