using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.IO;
using DevExpress.XtraTab;
using System.Text.RegularExpressions;


namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class frm制令转工单 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {
        #region
        public static DevExpress.XtraTab.XtraTabControl XTC;
        #endregion

        #region 变量
        string strconn = CPublic.Var.strConn;
        DataTable dtM;
        DataTable dt_子表;
        DataTable dt_辅助;
        DataTable dt_工单;
        DataTable dt_仓库;
        DataView dvM;
        string strMoNo = "";
        DataTable dt_bom;

        DataTable dt_cun;
        int flag_小标签打印 = 0;
        #endregion

        #region   加载
        public frm制令转工单()
        {
            InitializeComponent();
        }
        string cfgfilepath;
#pragma warning disable IDE1006 // 命名样式
        private void frm制令转工单_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }


                if (File.Exists(cfgfilepath + string.Format(@"\{0}.xml", this.Name + "1")))
                {

                    gvM.RestoreLayoutFromXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }


                devGridControlCustom1.UserName = CPublic.Var.LocalUserID;
                devGridControlCustom1.strConn = CPublic.Var.strConn;
                string str_用户ID = CPublic.Var.LocalUserID;//当前用户ID
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (gvM.GetRow(e.RowHandle) == null)
                {
                    return;
                }
                int j = gvM.RowCount;
                //for (int i = 0; i < j; i++)
                //{
                if (gvM.GetRowCellValue(e.RowHandle, "加急状态").ToString() == "加急")
                {
                    e.Appearance.BackColor = Color.Red;
                    e.Appearance.BackColor2 = Color.Red;
                }
                if (gvM.GetRowCellValue(e.RowHandle, "加急状态").ToString() == "急")
                {
                    e.Appearance.BackColor = Color.Yellow;
                    e.Appearance.BackColor2 = Color.Yellow;
                }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 函数
#pragma warning disable IDE1006 // 命名样式
        void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {

            string s = "select  产品编码,子项编码  from 基础数据物料BOM表 ";
            dt_bom = new DataTable();
            dt_bom = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            DataTable dt_生产 = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);

            string sql = "";

            if (dt_生产 == null || dt_生产.Rows.Count == 0 || dt_生产.Rows[0]["生产车间"].ToString() == "")
            {

                sql = string.Format(@"select a.已转工单数,mzl.*,x.车间编号,x.车间名称 as 车间,工时备注,kc.库存总数,base.工时 as 工时定额
               from 生产记录生产制令表 mzl    left join 基础数据物料信息表 base on base.物料编码= mzl.物料编码 
               left join  仓库物料数量表 kc on  kc.物料编码= mzl.物料编码  and  mzl.仓库号=kc.仓库号  
            left join  (select 属性字段1 as 车间编号,属性值 as 车间名称 from 基础数据基础属性表 where 属性类别 = '生产车间') x on x.车间编号=mzl.生产车间
             left join (select sum(生产数量) as 已转工单数,生产制令单号  from  生产记录生产工单表 where 关闭= 0  group by 生产制令单号) a  on mzl.生产制令单号=a.生产制令单号
            where mzl.未排单数量 > 0 and mzl.生效 = 1 and mzl.完成 = 0 and mzl.关闭 = 0 and (a.已转工单数<制令数量 or a.已转工单数 is null) and 生产制令类型<>'返修制令'  order by mzl.生效日期 ");
             

            }
            else
            {
                sql = string.Format(@"select a.已转工单数,mzl.*,x.车间编号,x.车间名称 as 车间,工时备注,kc.库存总数,base.工时 as 工时定额 
                      from 生产记录生产制令表 mzl    left join 基础数据物料信息表 base on base.物料编码= mzl.物料编码 
                      left join  仓库物料数量表 kc on  kc.物料编码= mzl.物料编码 and mzl.仓库号=kc.仓库号
                       left join (select sum(生产数量) as 已转工单数,生产制令单号  from  生产记录生产工单表 where 关闭= 0  group by 生产制令单号) a  on mzl.生产制令单号=a.生产制令单号
                 left join  (select 属性字段1 as 车间编号,属性值 as 车间名称 from 基础数据基础属性表 where 属性类别 = '生产车间') x on x.车间编号=mzl.生产车间                       
                        where mzl.未排单数量 > 0  and mzl.生效 = 1 and (a.已转工单数<制令数量 or a.已转工单数 is null) and 生产制令类型<>'返修制令'
                        and mzl.完成 = 0 and mzl.关闭 = 0   order by mzl.生效日期  ");
                //and 车间编号 = '{0}', dt_生产.Rows[0]["生产车间"].ToString() // 5-9 去掉限制
            }
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                dtM = new DataTable();
                da.Fill(dtM);
                dtM.Columns.Add("请选择", typeof(bool));
                dtM.Columns.Add("生产数量");
                //dtM.Columns.Add("已转工单数");
                dtM.Columns.Add("单个工单数量");
            }
            dvM = new DataView(dtM);
            gcM.DataSource = dvM;
            //  筛选条件 生效为1 
            string sql_工单 = "select * from 生产记录生产工单表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_工单, strconn))
            {
                dt_工单 = new DataTable();
                da.Fill(dt_工单);
            }
            string sql1 = "select * from 生产记录生产制令子表 ";
            using (SqlDataAdapter da = new SqlDataAdapter(sql1, strconn))
            {
                dt_辅助 = new DataTable();
                dt_子表 = new DataTable();
                da.Fill(dt_辅助);
                dt_辅助.Columns.Add("生产工单号");

                dt_子表 = dt_辅助.Clone();
                dt_子表.Columns.Add("作废");
                dt_子表.Columns.Add("仓库号");
                dt_子表.Columns.Add("关闭");
                dt_子表.Columns.Add("关闭日期");
                dt_子表.Columns.Add("关闭人员ID");
                dt_子表.Columns.Add("关闭人员");
              //  dt_子表.Columns.Add();
            }
            fun_下拉框();

        }

        private void fun_下拉框()
        {
            try
            {

                dt_仓库 = new DataTable();
                string sql4 = "select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别'";
                SqlDataAdapter da = new SqlDataAdapter(sql4, strconn);
                da.Fill(dt_仓库);
                repositoryItemSearchLookUpEdit1.DataSource = dt_仓库;
                repositoryItemSearchLookUpEdit1.DisplayMember = "仓库号";
                repositoryItemSearchLookUpEdit1.ValueMember = "仓库号";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        void fun_保存制令表(DataTable dt)
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = "select * from 生产记录生产制令表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                new SqlCommandBuilder(da);
                da.Update(dt);
            }
        }
  

#pragma warning disable IDE1006 // 命名样式
        void fun_保存工单表()
#pragma warning restore IDE1006 // 命名样式
        {
            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("转工单");
            try
            {
                SqlCommand cmm = new SqlCommand("select * from 生产记录生产工单表 where 1<>1", conn, ts);
                SqlDataAdapter da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(dt_工单);

                cmm = new SqlCommand("select * from 生产记录生产工单子表 where 1<>1", conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(dt_子表);
                ts.Commit();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                MessageBox.Show(ex.Message);

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {
            //int i = 0;
            dvM.RowFilter = null;
            dvM.RowFilter = "请选择 = True";
            dt_cun = new DataTable();
            dt_cun = dvM.ToTable();

            if (dt_cun.Rows.Count == 0)
            {
                dvM.RowFilter = "已转工单数<=制令数量";

                throw new Exception("没有选择项，请勾选");
            }
            foreach (DataRow dr in dt_cun.Rows)
            {
                //if (Convert.ToDecimal(dr["工时定额"])<=0)
                //{
                //    throw new Exception("该物料没有工时定额,请先联系维护定额人员。");
                //}
                string sql = string.Format("select  制令数量,关闭 from 生产记录生产制令表 where 生产制令单号='{0}'", dr["生产制令单号"].ToString());
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (Convert.ToDecimal(dt.Rows[0]["制令数量"]) != Convert.ToDecimal(dr["制令数量"]))
                    {
                        throw new Exception("制令数量已修改，请刷新后重试");
                    }

                    if (dt.Rows[0]["关闭"].Equals(true))
                    {
                        throw new Exception("制令已关闭,请刷新后重试");
                    }

                }
                if (dr["已转工单数"]==null || dr["已转工单数"] == DBNull.Value|| dr["已转工单数"].ToString() =="")
                {
                    dr["已转工单数"] = 0;
                }
                decimal a;
                if (dr["生产数量"].ToString() == "")
                {
                    throw new Exception("请输入生产数量");
                }



                a = Convert.ToDecimal(dr["生产数量"]) + Convert.ToDecimal(dr["已转工单数"]) - Convert.ToDecimal(dr["制令数量"]);
                if (a > 0)
                {
                    //根据动物需求,改提示为限制2019-7-16
                    //if (MessageBox.Show("生产数量大于制令数量,是否继续", "警告！", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    //{
                    //}
                    //else
                    //{
                        throw new Exception("生产数量大于制令数量");
                    //}
                }

                try
                {
                    decimal dd = Convert.ToDecimal(dr["生产数量"]);
                    decimal ddd = 1;
                    if (dr["单个工单数量"].ToString().Trim() != "")
                    {
                        string s = dr["单个工单数量"].ToString();
                        ddd = Convert.ToDecimal(dr["单个工单数量"]);
                    }
                    if (dd <= 0 || ddd <= 0)
                    {
                        throw new Exception("输入数量不能小于0");

                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("请正确输入数量");
                }
                //}
            }


        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                
                DataTable dt_gd2 = new DataTable();
                string sql = string.Format(@"SELECT [子项编码],数量*{0} as 需求数量,[子项名称],[数量],base.[计量单位],WIPType
                                        ,isnull(库存总数,0)库存总数,isnull(在途量,0)在途量,isnull(在制量,0)在制量,isnull(受订量,0)受订量,isnull(未领量,0)未领量
                                        ,isnull(有效总数,0)有效总数,isnull(大类,'')大类,isnull(小类,'')小类,bom.仓库号,bom.仓库名称 FROM [基础数据物料BOM表] bom
                                        left join 仓库物料数量表 kc on   kc.物料编码=子项编码 and bom.仓库号=kc.仓库号
                                        left join  基础数据物料信息表 base on  base.物料编码= kc.物料编码
                                        where  产品编码='{1}' and  bom.主辅料='主料'", Convert.ToDecimal(dr["制令数量"]), dr["物料编码"]);


                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    da.Fill(dt_gd2);
                    gc.DataSource = dt_gd2;
                }


                if (e != null && e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gcM, new Point(e.X, e.Y));
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {


            gridView1_RowCellClick(null, null);

        }


#pragma warning disable IDE1006 // 命名样式
        private void fun_关闭制令()
#pragma warning restore IDE1006 // 命名样式
        {

            DataTable dt = new DataTable();
            DataRow[] dr_选择 = dtM.Select(string.Format("请选择='{0}'", true));
            for (int i = 0; i < dr_选择.Length; i++)
            {
                string sql_临时 = string.Format("select * from 生产记录生产制令表 where 生产制令单号='{0}'", dr_选择[i]["生产制令单号"]);
                using (SqlDataAdapter da = new SqlDataAdapter(sql_临时, strconn))
                {
                    da.Fill(dt);
                    DataRow[] drr = dt.Select(string.Format("生产制令单号='{0}'", dr_选择[i]["生产制令单号"]));
                    drr[0]["关闭"] = true;
                    drr[0]["关闭人员"] = CPublic.Var.localUserName;
                    drr[0]["关闭人员ID"] = CPublic.Var.LocalUserID;
                    drr[0]["关闭日期"] = CPublic.Var.getDatetime();

                }
            }

            fun_保存制令表(dt);

        }
        #endregion

        #region 界面操作
        //刷新
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_load();
            barEditItem2.EditValue = "";
            gc.DataSource = null;

        }
        //转工单 按钮 
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataTable dt_生产 = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);

                gvM.CloseEditor();
                this.BindingContext[dvM].EndCurrentEdit();
                this.BindingContext[dtM].EndCurrentEdit();

                DateTime dt1 = CPublic.Var.getDatetime();
                fun_check();
                DataRow rrr = gvM.GetDataRow(gvM.FocusedRowHandle);

                string sql = string.Format("select 物料状态,仓库号,更改预计完成时间 from 基础数据物料信息表 where 物料编码 = '{0}'", rrr["物料编码"].ToString());
                DataTable t = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(t);
                if (t.Rows[0]["物料状态"].ToString() == "更改")
                {
                    DateTime time = (DateTime)t.Rows[0]["更改预计完成时间"];
                    MessageBox.Show(string.Format("当前物料为更改状态，不能转工单，预计完成时间：{0}", time.ToString("yyyy-MM-dd")));
                }
                else
                {
                    string str_打印机 = "";
                    //19-3-15 东屋暂无小标签打印 
                    flag_小标签打印 = 0;
                    //if (MessageBox.Show(string.Format("是否确认打印小标签？"), "确认", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    //{
                    //    flag_小标签打印 = 1;
                    //    this.printDialog1.Document = this.printDocument1;
                    //    try
                    //    {
                    //        str_打印机 = CPublic.Var.li_CFG["printer_tag"].ToString();
                    //    }
                    //    catch
                    //    {

                    //    }
                    //    if (str_打印机 == "")
                    //    {

                    //        DialogResult result = this.printDialog1.ShowDialog();
                    //        if (result == DialogResult.OK)
                    //        {

                    //            str_打印机 = this.printDocument1.PrinterSettings.PrinterName;

                    //        }
                    //        else
                    //        {
                    //            flag_小标签打印 = 0;

                    //        }
                    //    }
                    //}

                    DateTime tt = CPublic.Var.getDatetime();
                    string ss = tt.Year.ToString().Substring(2, 2);
                    string yyyy = tt.Year.ToString();
                    string mm = tt.Month.ToString("00");
                    string dd = tt.Day.ToString("00");
                    if (barEditItem2.EditValue != null && barEditItem2.EditValue.ToString() != "")
                    {
                        yyyy = Convert.ToDateTime(barEditItem2.EditValue).Year.ToString();
                        ss = yyyy.Substring(2, 2);

                        mm = Convert.ToDateTime(barEditItem2.EditValue).Month.ToString("00");
                        dd = Convert.ToDateTime(barEditItem2.EditValue).Day.ToString("00");
                    }

                    foreach (DataRow dr in dt_cun.Rows)
                    {

                        {

                            decimal dec_计划生产量 = Convert.ToDecimal(dr["计划生产量"]);


                            if (dr["单个工单数量"].ToString() != "")
                            {
                                int j_生产数 = Convert.ToInt32(dr["生产数量"]);
                                int j_单个工单数 = Convert.ToInt32(dr["单个工单数量"].ToString());
                                int j_工单数 = j_生产数 / j_单个工单数;
                                if (j_生产数 % j_单个工单数 != 0)
                                {
                                    j_工单数 = j_工单数 + 1;
                                }

                                for (int i = 0; i < j_工单数; i++)
                                {
                                    strMoNo = string.Format("MO{0}{1:D2}{2:00}{3:0000}", ss, mm, dd, CPublic.CNo.fun_得到最大流水号("MO", Convert.ToInt32(yyyy), Convert.ToInt32(mm), Convert.ToInt32(dd)));
                                    DataRow drrr = dt_工单.NewRow(); //
                                    drrr["生产工单号"] = strMoNo;
                                    drrr["生产工单类型"] = dr["生产制令类型"];
                                    drrr["加急状态"] = dr["加急状态"];
                                    drrr["GUID"] = System.Guid.NewGuid();
                                    drrr["生产制令单号"] = dr["生产制令单号"];
                                    drrr["物料编码"] = dr["物料编码"];
                                    drrr["物料名称"] = dr["物料名称"];
                                    drrr["规格型号"] = dr["规格型号"];
                                    drrr["班组ID"] = dr["班组ID"];
                                    drrr["班组"] = dr["班组"];

                                    drrr["仓库号"] = dr["仓库号"];
                                    drrr["仓库名称"] = dr["仓库名称"];
                                    drrr["特殊备注"] = dr["特殊备注"];
                                    drrr["备注1"] = dr["备注"];
                                    drrr["预计完工日期"] = dr["预完工日期"];
                                    drrr["工时备注"] = dr["工时备注"];

                                    if (i == j_工单数 - 1 && j_生产数 % j_单个工单数 != 0)
                                    {
                                        drrr["生产数量"] = j_生产数 % j_单个工单数;
                                    }
                                    else
                                    {
                                        drrr["生产数量"] = j_单个工单数;
                                    }

                                    if (Convert.ToDecimal(dr["工时定额"]) > 0)
                                    {
                                        //drrr["工时"] = Convert.ToDecimal(drrr["生产数量"]) / Convert.ToDecimal(dr["工时定额"]);
                                      drrr["工时"] = Convert.ToDecimal(drrr["生产数量"]) * Convert.ToDecimal(dr["工时定额"]); //2019-7-30
                                    }
                                    drrr["版本备注"] = dr["版本备注"];
                                    drrr["未检验数量"] = drrr["生产数量"];
                                    drrr["图纸编号"] = dr["图纸编号"];
                                    drrr["生产车间"] = dr["车间编号"];
                                    drrr["车间名称"] = dr["车间"];


                                    drrr["制单人员ID"] = CPublic.Var.LocalUserID;
                                    drrr["制单人员"] = CPublic.Var.localUserName;
                                    drrr["制单日期"] = tt;
                                    drrr["生效"] = 0;
                                    drrr["生效人"] = "";
                                    drrr["生效人ID"] = "";
                                    drrr["生效日期"] = DBNull.Value;
                                    dt_工单.Rows.Add(drrr);
                                    //保存工单子表
                                    DataRow[] rr = dt_辅助.Select(string.Format("生产制令单号='{0}'", dr["生产制令单号"].ToString().Trim()));
                                    if (rr.Length > 0)
                                    {
                                        DataRow drr = dt_子表.NewRow();
                                        drr.ItemArray = rr[0].ItemArray;
                                        drr["生产工单号"] = strMoNo;
                                        dt_子表.Rows.Add(drr);
                                    }
                                    if (flag_小标签打印 == 1)
                                    {
                                        int count = (int)(Convert.ToDecimal(drrr["生产数量"]));

                                        //this.printDialog1.Document = this.printDocument1;
                                        //DialogResult result = this.printDialog1.ShowDialog();
                                        //if (result == DialogResult.OK)
                                        //{
                                        //    //string str_打印机 = new PrintDocument().PrinterSettings.PrinterName;
                                        //    str_打印机 = this.printDocument1.PrinterSettings.PrinterName;

                                        //}
                                        fun_小标签打印(count, str_打印机, dr["图纸编号"].ToString());
                                    }
                                }
                            }
                            else
                            {
                                strMoNo = string.Format("MO{0}{1:D2}{2:00}{3:0000}", ss, mm, dd, CPublic.CNo.fun_得到最大流水号("MO", Convert.ToInt32(yyyy), Convert.ToInt32(mm), Convert.ToInt32(dd)));
                                DataRow drrr = dt_工单.NewRow(); //
                                drrr["生产工单号"] = strMoNo;
                                drrr["生产工单类型"] = dr["生产制令类型"];
                                drrr["加急状态"] = dr["加急状态"];
                                drrr["预计完工日期"] = dr["预完工日期"];
                                drrr["GUID"] = System.Guid.NewGuid();
                                drrr["生产制令单号"] = dr["生产制令单号"];
                                drrr["物料编码"] = dr["物料编码"];
                                drrr["版本备注"] = dr["版本备注"];
                                drrr["物料名称"] = dr["物料名称"];
                                drrr["规格型号"] = dr["规格型号"];
                                drrr["仓库号"] = dr["仓库号"];
                                drrr["仓库名称"] = dr["仓库名称"];
                                drrr["特殊备注"] = dr["特殊备注"];
                                drrr["备注1"] = dr["备注"];
                                //     drrr["作废"] = 0;
                                drrr["工时备注"] = dr["工时备注"];
                                drrr["班组ID"] = dr["班组ID"];
                                drrr["班组"] = dr["班组"];
                                if (Convert.ToDecimal(dr["工时定额"]) > 0)
                                {
                                   // drrr["工时"] = Convert.ToDecimal(dr["生产数量"]) / Convert.ToDecimal(dr["工时定额"]);
                                    drrr["工时"] = Convert.ToDecimal(dr["生产数量"])* Convert.ToDecimal(dr["工时定额"]); // 19-7-30 
                                }
                                drrr["生产数量"] = dr["生产数量"];
                                drrr["未检验数量"] = drrr["生产数量"];

                                drrr["图纸编号"] = dr["图纸编号"];

                                drrr["生产车间"] = dr["车间编号"];
                                drrr["车间名称"] = dr["车间"];


                                drrr["制单人员ID"] = CPublic.Var.LocalUserID;
                                drrr["制单人员"] = CPublic.Var.localUserName;
                                drrr["制单日期"] = tt;
                                drrr["生效"] = 0;
                                drrr["生效人"] = "";
                                drrr["生效人ID"] = "";
                                drrr["生效日期"] = DBNull.Value;
                                dt_工单.Rows.Add(drrr);
                                //保存工单子表
                                DataRow[] rr = dt_辅助.Select(string.Format("生产制令单号='{0}'", dr["生产制令单号"].ToString().Trim()));
                                if (rr.Length > 0)
                                {
                                    DataRow drr = dt_子表.NewRow();
                                    drr.ItemArray = rr[0].ItemArray;
                                    drr["生产工单号"] = strMoNo;
                                    drr["仓库号"] = dr["仓库号"].ToString();
                                    dt_子表.Rows.Add(drr);
                                }
                                if (flag_小标签打印 == 1)
                                {
                                    int count = (int)(Convert.ToDecimal(dr["生产数量"]));

                                    //this.printDialog1.Document = this.printDocument1;
                                    //DialogResult result= this.printDialog1.ShowDialog();
                                    //if (result == DialogResult.OK)
                                    //{
                                    //    //string str_打印机 = new PrintDocument().PrinterSettings.PrinterName;
                                    //    str_打印机 = this.printDocument1.PrinterSettings.PrinterName;

                                    //}
                                    fun_小标签打印(count, str_打印机, dr["图纸编号"].ToString());
                                }

                            }

                        }

                    }

                    flag_小标签打印 = 0;

                    fun_保存工单表();
                    // fun_保存工单子表();



                    if (MessageBox.Show("转工单成功,是否跳转至工单生效界面？", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        ERPproduct.frm工单生效选择 frm = new frm工单生效选择(strMoNo);
                        CPublic.UIcontrol.Showpage(frm, "工单生效界面");
                    }

                    barLargeButtonItem1_ItemClick(null, null);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        //关闭
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }
        //打印小标签
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //if (MessageBox.Show(string.Format("是否确认打印小标签？"), "确认", MessageBoxButtons.OKCancel) == DialogResult.OK)
            //{
            //    fun_小标签打印();

            //}

        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_小标签打印(int i_生产数, string str_打印机, string str_图号)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string path = Application.StartupPath + @"\Mode\标签1.lab";

                if (strMoNo != "")
                {
                    if (CPublic.Var.localUser课室编号 == "0001030103")   //制三课需要 打印特殊的 标签
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        string str = strMoNo.Substring(3, 10) + "P" + i_生产数.ToString("00000");
                        dic.Add("gdh", str);
                        string target = "579.";
                        Regex r = new Regex(string.Format(@"(?<={0}+).*", target));
                        string result = r.Match(str_图号).Value.Replace(".", "");
                        dic.Add("th", result); // 图号 
                        dic.Add("rgjm", result); // 工号简码
                        str = str + result + CPublic.Var.localUser工号简码;
                        dic.Add("rwm", str); //总 二维码

                    }
                    else
                    {

                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("fore", strMoNo);
                        int a = (int)i_生产数 / 12;

                        if (a == 0) a = 12;
                        else if (i_生产数 % 12 == 0)
                        {
                            a = i_生产数 + 1;
                        }
                        else
                        {
                            a = ((int)i_生产数 / 12 + 1) * 12;
                        }
                        Lprinter lp = new Lprinter(path, dic, str_打印机, a);
                        FileStream aFile = new FileStream(Application.StartupPath + @"\Mode\log.txt", FileMode.OpenOrCreate);
                        StreamReader sr = new StreamReader(aFile);
                        lp.Left = int.Parse(sr.ReadLine());
                        lp.Top = int.Parse(sr.ReadLine());
                        sr.Close();
                        lp.Start();
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //位置模板偏移
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            模板位置偏移 fm = new 模板位置偏移();
            fm.ShowDialog();
        }
        //单独打印小标签
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            单独打印小标签界面 fm = new 单独打印小标签界面();
            fm.StartPosition = FormStartPosition.CenterScreen;
            fm.ShowDialog();
        }
        //关闭制令
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (MessageBox.Show("确定关闭制令？", "核实!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    gvM.CloseEditor();
                    this.BindingContext[dtM].EndCurrentEdit();
                    this.BindingContext[dvM].EndCurrentEdit();
                    //gcP.Focus();
                    gcM.Focus();
                    fun_关闭制令();

                    MessageBox.Show("制令已关闭");
                    barLargeButtonItem1_ItemClick(null, null);
                }

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region 右键菜单
        private void 查看详细BOmToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DataRow r = gvM.GetDataRow(gvM.FocusedRowHandle);
            decimal a = 1;
            if (r["生产数量"].ToString().Trim() != "")
            {
                a = Convert.ToDecimal(r["生产数量"]);
            }
            UI物料BOM详细数量 UI = new UI物料BOM详细数量(r["物料编码"].ToString().Trim(), a);
            CPublic.UIcontrol.AddNewPage(UI, "物料BOM信息");
        }
        #endregion

#pragma warning disable IDE1006 // 命名样式
        private void gvM_MouseUp(object sender, MouseEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Button == MouseButtons.Left)
            {


                int[] dr = gvM.GetSelectedRows();
                if (dr.Length > 1)
                {
                    for (int i = 0; i < dr.Length; i++)
                    {
                        DataRow r = gvM.GetDataRow(dr[i]);
                        if (r["请选择"].Equals(true))
                        {
                            r["请选择"] = 0;

                        }
                        else
                        {
                            r["请选择"] = 1;
                        }

                    }

                    gvM.MoveBy(dr[dr.Length - 1]);
                }
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gvM_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        { 
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_KeyDown(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv.GetFocusedRowCellValue(gv.FocusedColumn));
                e.Handled = true;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (gv.GetRow(e.RowHandle) == null)
                {
                    return;
                }
                int j = gv.RowCount;

                if (Convert.ToDecimal(gv.GetRowCellValue(e.RowHandle, "需求数量")) >= Convert.ToDecimal(gv.GetRowCellValue(e.RowHandle, "库存总数")))
                {
                    e.Appearance.BackColor = Color.Pink;
                    e.Appearance.BackColor2 = Color.Pink;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip2.Show(gc, new Point(e.X, e.Y));
            }
        }

        private void 查看图纸ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                string strConn_FS = CPublic.Var.geConn("FS");
                string sql = string.Format(@"select * from [基础物料蓝图表] where 物料号='{0}'  and 版本=0", dr["子项编码"]);
                DataRow rr = CZMaster.MasterSQL.Get_DataRow(sql, strconn);

                if (rr == null || rr["文件地址"] == null || rr["文件地址"].ToString() == "")
                {
                    throw new Exception("未上传文件,没有文件可以查看");
                }
                string type = rr["后缀"].ToString();
                string foldPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\查看文件\\";

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

#pragma warning disable IDE1006 // 命名样式
        private void gvM_ColumnFilterChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            try
            {

                if (cfgfilepath != "")
                {
                    gvM.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gvM_ColumnPositionChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gvM.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
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
                    DataRow[] ds = dt_仓库.Select(string.Format("仓库号 = '{0}'", dr["仓库号"]));
                    dr["仓库名称"] = ds[0]["仓库名称"];
                    //dr["仓库名称"] = sr["仓库名称"].ToString();
                    string sql = "select * from 仓库物料数量表 where 物料编码 = '" + dr["子项编码"] + "' and 仓库号 = '" + dr["仓库号"] + "'";
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_物料数量 = new DataTable();
                    da.Fill(dt_物料数量);
                    if (dt_物料数量.Rows.Count == 0)
                    {
                        dr["库存总数"] = 0;
                        dr["在途量"] = 0;
                        dr["在制量"] = 0;
                        dr["受订量"] = 0;
                        dr["未领量"] = 0;
                        dr["有效总数"] = 0;
                        // dr["有效总数"] = 0;
                    }
                    else
                    {
                        dr["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
                        dr["在途量"] = dt_物料数量.Rows[0]["库存总数"];
                        dr["在制量"] = dt_物料数量.Rows[0]["库存总数"];
                        dr["受订量"] = dt_物料数量.Rows[0]["库存总数"];
                        dr["未领量"] = dt_物料数量.Rows[0]["库存总数"];
                        dr["有效总数"] = dt_物料数量.Rows[0]["库存总数"];
                        //  dr["有效总数"] = dt_物料数量.Rows[0]["有效总数"];
                    }
                }
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
                DataTable dt2 = dvM.ToTable();

                if (MessageBox.Show("可能造成当前界面当前操作内容丢失，请完成操作后刷新？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {

                    foreach (DataRow dr in dtM.Rows)
                    {

                        string v_number = "";

                        DataTable dt_x = new DataTable();
                        dt_x = ERPorg.Corg.billofM(dt_x, dr["物料编码"].ToString(), true, dt_bom);
                        if (dt_x.Rows.Count > 0)
                        {
                            foreach (DataRow drr in dt_x.Rows)
                            {
                                string sql1 = string.Format(@"  SELECT 文件名, 物料号, 版本 FROM 程序版本维护表 WHERE 版本 = (SELECT MAX(版本) FROM    程序版本维护表 where  物料号 ='{0}' and 停用='0' ) and 物料号 = '{0}'  and 停用='0' ", drr["子项编码"]);
                                DataRow dr_banbe = CZMaster.MasterSQL.Get_DataRow(sql1, strconn);
                                if (dr_banbe != null)
                                {
                                    if (dr_banbe["文件名"].ToString() != "")
                                    {
                                        if (v_number == "")
                                        {
                                            v_number = v_number + dr_banbe["文件名"].ToString();
                                        }
                                        else
                                        {
                                            v_number = v_number + ";" + dr_banbe["文件名"].ToString();
                                        }
                                        //break;
                                    }

                                }
                            }
                        }
                        // DataRow dr2 = dtM.Select("");
                        dr["版本备注"] = v_number.ToString();


                    }

                    SqlDataAdapter da;
                    string sql = "select * from 生产记录生产制令表 where 1<>1";
                    da = new SqlDataAdapter(sql, strconn);
                    new SqlCommandBuilder(da);
                    da.Update(dtM);

                    MessageBox.Show("刷新成功");
                }

               

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

