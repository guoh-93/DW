using CZMaster;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class frm快速检验 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {

        string str_大类 = "";
        string cfgfilepath = "";
        DataTable dt_车间信息 = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);

        #region 成员
        /// <summary>
        /// 数据库的连接字符串
        /// </summary>
        string strcon = CPublic.Var.strConn;
        // string strconn2 = "Password=dwDZ@123;Persist Security Info=True;User ID=sa;Initial Catalog=UFDATA_995_2018;Data Source=192.168.20.150;Pooling=true;Max Pool Size=40000;Min Pool Size=0";
        string strconn2 = CPublic.Var.geConn("DW");
        /// <summary>
        /// 用来查询的检验单
        /// </summary>
        string str_检验单 = "";

        /// <summary>
        /// 人员表的DT
        /// </summary>
        DataTable dt_people;

        /// <summary>
        /// 送检部门DT表
        /// </summary>
        DataTable dt_deparment;
        DataTable dt_工单;
        string str_检验单号 = "";
        DataRow drr; // drM
        DataTable dtM;
        DataTable test_暂存;

        DataTable dt_检验项目保存;
        DataTable dt_返工;
        string str_生产工单 = "";
        decimal str_检验入库数量 = 0;
        string str_入库单号 = "";
        string str_物料编码 = "";
        // ERPorg.Corg.BardCodeHooK BarCode = new ERPorg.Corg.BardCodeHooK(); 
        //DataView dv;
        #endregion

        #region 自用类
        public frm快速检验()
        {
            InitializeComponent();
        }
        public frm快速检验(string str生产工单, decimal str检验入库数量, string str入库单号, string str物料编码)
        {
            InitializeComponent();
            strcon = CPublic.Var.strConn;
            str_生产工单 = str生产工单;
            str_检验入库数量 = str检验入库数量;
            str_物料编码 = str物料编码;
            str_入库单号 = str入库单号;
            if (str生产工单 != "")
            {
                fun_加载成品信息();
                fun_保存工单表和基础表();
            }
            //  BarCode.BarCodeEvent += new  ERPorg.Corg.BardCodeHooK.BardCodeDeletegate(BarCode_BarCodeEvent);  
        }

        //public frm快速检验(string jianyandan)
        //{
        //    str_检验单 = jianyandan;
        //    InitializeComponent();
        //    strcon = CPublic.Var.strConn;
        //}
        string str000 = "";
#pragma warning disable IDE1006 // 命名样式
        private void frm快速检验_Load(object sender, EventArgs e)
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
                if (File.Exists(cfgfilepath + string.Format(@"\{0}.xml", this.Name)))
                {

                    gv_scgd.RestoreLayoutFromXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }
                test_暂存 = new DataTable();
                test_暂存.Columns.Add("产品序列号");
                test_暂存.Columns.Add("状态");

                panel3.Visible = false;

                barStaticItem1.Caption = "当前登录人员：" + CPublic.Var.localUserName;
                string str = CPublic.Var.LocalUserID;
                using (SqlDataAdapter da = new SqlDataAdapter("select 厂区 from 人事基础员工表 where 员工号 ='" + str + "'", CPublic.Var.strConn))
                {
                    DataTable dt_厂区 = new DataTable();
                    da.Fill(dt_厂区);

                    panel12.Visible = false;
                    panel13.Visible = false;
                    this.gridColumn32.OptionsColumn.AllowEdit = false;
                    this.gridColumn32.Visible = false;


                }
                //加载下拉
                fun_getBasicData();
                fun_mb();

                //单据关闭 删除 隐藏
                barLargeButtonItem6.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                fun_检验工单代办事项();
           

                txt_songjiantime.EditValue = CPublic.Var.getDatetime().ToShortDateString();

                if (str_检验单 != "")
                {
                    string sql = string.Format("select * from 快速检验生产检验单主表 where 生产检验单号='{0}'", str_检验单);

                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        try
                        {
                            dt_工单.Select(string.Format("生产工单号='{0}'", dt.Rows[0]["生产工单号"]));
                            dataBindHelper2.DataFormDR(dt_工单.Rows[0]);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("此条记录有误");
                        }

                    }

                }
                //载入空的drr
                string sql_jyd = string.Format("select * from 快速检验生产检验单主表 where 1<>1 ");
                using (SqlDataAdapter da1 = new SqlDataAdapter(sql_jyd, strcon))
                {
                    dtM = new DataTable();
                    da1.Fill(dtM);
                    drr = dtM.NewRow();
                    dtM.Rows.Add(drr);
                }
                com_结论.EditValue = "合格";


                //直接选中str_生产工单
                if (str_生产工单 != null)
                {

                    DataRow[] ds = dt_定位.Select(string.Format("备注5='{0}'and 物料编码='{1}' and 未检验数量>='{2}'", str_生产工单, str_物料编码, Convert.ToDecimal(str_检验入库数量)));
                    if (ds.Length > 0)
                    {
                        gv_scgd.Focus();
                        str000 = ds[0]["生产工单号"].ToString();
                        gv_scgd.FocusedRowHandle = gv_scgd.LocateByDisplayText(0, gridColumn6, str000);
                        gv_scgd.TopRowIndex = gv_scgd.FocusedRowHandle;
                        gv_scgd.SelectRow(gv_scgd.FocusedRowHandle);


                        gv_scgd_RowCellClick_1(null, null);
                        if (str_生产工单 != null) txt_checknum.Text = str_检验入库数量.ToString("f2");
                    }
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        void dt_返工_ColumnChanged(object sender, DataColumnChangeEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (e.Column.ColumnName == "返工编号")
                {
                    DataRow[] ds = dt_返工原因表.Select(string.Format("fgbh = '{0}'", e.Row["返工编号"]));
                    e.Row["返工原因"] = ds[0]["fgyy"];

                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "成品检验_返工原因列变化事件出错");
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_scgd_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (gv_scgd.GetRow(e.RowHandle) == null)
                {
                    return;
                }
                int j = gv_scgd.RowCount;
                for (int i = 0; i < j; i++)
                {
                    if (gv_scgd.GetRowCellValue(e.RowHandle, "加急状态").ToString() == "加急")
                    {
                        e.Appearance.BackColor = Color.Red;
                        e.Appearance.BackColor2 = Color.Red;
                    }
                    if (gv_scgd.GetRowCellValue(e.RowHandle, "加急状态").ToString() == "急")
                    {
                        e.Appearance.BackColor = Color.Pink;
                        e.Appearance.BackColor2 = Color.Pink;
                    }
                    if (gv_scgd.GetRowCellValue(e.RowHandle, "部分完工").Equals(true))
                    {

                        e.Appearance.BackColor = Color.Yellow;
                        e.Appearance.BackColor2 = Color.Yellow;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 方法 代办事项
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 给 员工ID 下拉框赋值
        /// </summary>
        private void fun_getBasicData()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                string sql = "";
                if (dt_车间信息.Rows.Count > 0)
                {
                    sql = string.Format("select 员工号,姓名,部门,岗位 from 人事基础员工表 where 课室编号='{0}' and 在职状态='在职'", dt_车间信息.Rows[0]["生产车间"]);
                }
                else
                {
                    sql = string.Format("select 员工号,姓名,部门,岗位 from 人事基础员工表 where  在职状态='在职'");

                }
                dt_people = MasterSQL.Get_DataTable(sql, strcon);
                txt_songjianren.Properties.DataSource = dt_people;
                txt_songjianren.Properties.ValueMember = "员工号";
                txt_songjianren.Properties.DisplayMember = "员工号";

                //sql = "select 部门编号,部门名称,领导姓名 from 人事基础部门表";
                //dt_deparment = MasterSQL.Get_DataTable(sql, strcon);
                //txt_songjiandepID.Properties.DataSource = dt_deparment;
                //txt_songjiandepID.Properties.ValueMember = "部门编号";
                //txt_songjiandepID.Properties.DisplayMember = "部门编号";
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_getBasicData");
                throw ex;
            }
        }

        //2016.06.28 修改：生产车间改为部门编号，添加部门名称
        DataTable dt_定位;
#pragma warning disable IDE1006 // 命名样式
        private void fun_检验工单代办事项()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataTable dt_生产 = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);
                //暂时注释部分 
                //                if (dt_生产.Rows.Count == 0)
                //                {
                //                    MessageBox.Show("你没有该视图权限");
                //                }
                //                else
                //                {
                //                    string sql = "";
                //                    if (dt_生产.Rows[0]["生产车间"].ToString() == "")//理论上只有管理员才是 “” 的
                //                    {

                //                    }
                //                    else
                //                    {

                //                    }

                string sql = @"select a.*,b.物料编码 from 生产记录生产工单表 a left join 基础数据物料信息表 b on a.物料编码 = b.物料编码
                where a.检验完成 = 0 and a.关闭 = 0 and a.未检验数量 >0 and a.生效 = 1 and 完成=0  
              and (a.完工 = 1 or (a.部分完工=1 and a.在线检验数<部分完工数)) and (a.生产数量-a.在线检验数)>0 ";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    dt_工单 = new DataTable();
                    da.Fill(dt_工单);
                    dt_定位 = dt_工单.Copy();
                    gc_scgd.DataSource = dt_工单;
                }
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_检验工单代办事项");
                throw ex;
            }
        }
        #endregion

        #region  调用的方法

#pragma warning disable IDE1006 // 命名样式
        private void fun_盒贴(string str_物料编码)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = string.Format("select * from BQ_HZXX where wlbh = '{0}'", str_物料编码);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                da.Fill(dt);
                dataBindHelper3.DataFormDR(dt.Rows[0]);

                try
                {
                    if (dt_车间信息.Rows[0]["生产车间"].ToString().Trim() == "0001030104")
                    {

                        if (comboBox1.Text.Trim() == "芜湖德力西" || comboBox1.Text.Trim() == "芜湖德力西英文")
                        {
                            txt_订单号.Text = txt_shenchgdh.Text.ToString().Trim();
                        }
                    }

                }
                catch (Exception ex)
                {
                    CZMaster.MasterLog.WriteLog(ex.Message, "成品检验-盒贴信息-dataBindHelper赋值出错");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_检验项目(DataRow rr)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                //先取 检验记录明细 表结构
                string sql_表结构 = "select * from 成品检验检验记录明细表 where 1 <> 1";
                SqlDataAdapter da_表结构 = new SqlDataAdapter(sql_表结构, strcon);
                dt_检验项目保存 = new DataTable();
                da_表结构.Fill(dt_检验项目保存);

                //取产品的大小类信息
                string sql_物料信息 = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", rr["物料编码"].ToString());
                DataTable dt_物料信息 = new DataTable();
                SqlDataAdapter da_物料信息 = new SqlDataAdapter(sql_物料信息, strcon);
                da_物料信息.Fill(dt_物料信息);
                //先按 cpbh 找，再按规格型号找， 再按 小类 找，最后按 大类 找

                string sql_检验项目 = string.Format("select * from ZZ_JYXM where cpbh = '{0}' order by xh", dt_物料信息.Rows[0]["物料编码"].ToString());
                SqlDataAdapter da_检验项目 = new SqlDataAdapter(sql_检验项目, strcon);
                DataTable dt_检验项目 = new DataTable();
                da_检验项目.Fill(dt_检验项目);
                if (dt_检验项目.Rows.Count == 0)
                {

                    sql_检验项目 = string.Format("select * from ZZ_JYXM where dl = '{0}' order by xh", dt_物料信息.Rows[0]["规格型号"].ToString());
                    da_检验项目 = new SqlDataAdapter(sql_检验项目, strcon);
                    dt_检验项目 = new DataTable();
                    da_检验项目.Fill(dt_检验项目);
                    if (dt_检验项目.Rows.Count == 0)
                    {

                        sql_检验项目 = string.Format("select * from ZZ_JYXM where dl = '{0}' order by xh", dt_物料信息.Rows[0]["存货分类"].ToString());
                        da_检验项目 = new SqlDataAdapter(sql_检验项目, strcon);
                        dt_检验项目 = new DataTable();
                        da_检验项目.Fill(dt_检验项目);
                        if (dt_检验项目.Rows.Count == 0)
                        {
                            sql_检验项目 = string.Format("select * from ZZ_JYXM where dl = '{0}' order by xh", dt_物料信息.Rows[0]["大类"].ToString());
                            da_检验项目 = new SqlDataAdapter(sql_检验项目, strcon);
                            dt_检验项目 = new DataTable();
                            da_检验项目.Fill(dt_检验项目);
                            //if (dt_检验项目.Rows.Count == 0)
                            //{
                            //    sql_检验项目 = string.Format("select * from ZZ_JYXM where dl = '{0}' order by xh", dt_物料信息.Rows[0]["xl"].ToString());
                            //    da_检验项目 = new SqlDataAdapter(sql_检验项目, strcon);
                            //    dt_检验项目 = new DataTable();
                            //    da_检验项目.Fill(dt_检验项目);
                            //    if (dt_检验项目.Rows.Count == 0)
                            //    {
                            //        sql_检验项目 = string.Format("select * from ZZ_JYXM where dl = '{0}' order by xh", dt_物料信息.Rows[0]["dl"].ToString());
                            //        da_检验项目 = new SqlDataAdapter(sql_检验项目, strcon);
                            //        dt_检验项目 = new DataTable();
                            //        da_检验项目.Fill(dt_检验项目);
                            //    }
                            //}
                        }
                    }
                }

                str_大类 = dt_物料信息.Rows[0]["大类"].ToString();
                if (dt_检验项目.Rows.Count != 0)
                {
                    foreach (DataRow r in dt_检验项目.Rows)
                    {
                        DataRow dr = dt_检验项目保存.NewRow();
                        dt_检验项目保存.Rows.Add(dr);
                        dr["大类"] = r["dl"].ToString().Trim();
                        dr["序号"] = r["xh"].ToString().Trim();
                        dr["检验项目"] = r["jyxm"].ToString().Trim();
                        dr["检验要求"] = r["jyyq"].ToString().Trim();
                        dr["检测水平"] = r["jxsp"].ToString().Trim();
                        dr["合格水平"] = r["hgsp"].ToString().Trim();
                        dr["wjbm"] = r["wjbm"].ToString();
                        if (r["bz1"].ToString().Trim() == "0")
                        {
                            //A-H 打上√
                            dr["a"] = "√";
                            dr["b"] = "√";
                            dr["c"] = "√";
                            dr["d"] = "√";
                            dr["e"] = "√";
                            dr["f"] = "√";
                            dr["g"] = "√";
                            dr["h"] = "√";
                        }
                        else
                        {
                            //什么都不敢，A-H需要手动写数据
                            dr["a"] = "";
                            dr["b"] = "";
                            dr["c"] = "";
                            dr["d"] = "";
                            dr["e"] = "";
                            dr["f"] = "";
                            dr["g"] = "";
                            dr["h"] = "";
                        }
                    }
                }
                else
                {
                    //MessageBox.Show("找不到该物料的检验明细！");
                }
                //dv = new DataView(dt_检验项目保存);
                //dv.Sort = "序号";
                gc.DataSource = dt_检验项目保存;
                if (dt_检验项目保存.Select("len(检验项目)<6 and 检验项目 like '尺寸%'").Length > 0)
                {
                    MessageBox.Show("有尺寸需要输入", "提示");
                }

                fun_载入返工原因();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_返工记录()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = "select * from 快速检验检验记录返工表 where 1<>1";
                dt_返工 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                da.Fill(dt_返工);
                gcP.DataSource = dt_返工;
                dt_返工.ColumnChanged += dt_返工_ColumnChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /* 保存检验明细；保存返工记录；保存合贴信息到主表 */
#pragma warning disable IDE1006 // 命名样式
        private DataTable fun_保存检验明细()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                gv.CloseEditor();
                gc.BindingContext[dt_检验项目保存].EndCurrentEdit();
                //gc.BindingContext[dv].EndCurrentEdit();

                string sql = "select * from 成品检验检验记录明细表 where 1<>1";
                SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                //new SqlCommandBuilder(da);
                if (str_检验单号 == "")
                {
                    throw new Exception("没有生成检验单号");
                }
                foreach (DataRow dr in dt_检验项目保存.Rows)
                {
                    dr["生产检验单号"] = str_检验单号;
                }
                DataTable dt = dt_检验项目保存.Copy();
                return dt;
                //da.Update(dt_检验项目保存);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message);
                throw ex;
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private DataTable fun_保存返工信息()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                gvP.CloseEditor();
                gcP.BindingContext[dt_返工].EndCurrentEdit();
                if (dt_返工.Rows.Count != 0)
                {
                    string sql = "select * from 快速检验检验记录返工表 where 1<>1";
                    SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                    //new SqlCommandBuilder(da);
                    if (str_检验单号 == "")
                    {
                        throw new Exception("没有生成检验单号");
                    }
                    foreach (DataRow dr in dt_返工.Rows)
                    {
                        dr["生产检验单号"] = str_检验单号;
                    }

                    //da.Update(dt_返工);
                }
                DataTable dt = dt_返工.Copy();
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        DataTable dt_返工原因表;
        //返工原因载入
#pragma warning disable IDE1006 // 命名样式
        private void fun_载入返工原因()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = string.Format("select * from ZZ_FGYY ", str_大类);//where dl = '{0}' //拉所有的原因
                dt_返工原因表 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                da.Fill(dt_返工原因表);
                if (dt_返工原因表.Rows.Count == 0)
                {
                    sql = string.Format("select * from ZZ_FGYY where dl = '{0}'", "通用");
                    da = new SqlDataAdapter(sql, strcon);
                    da.Fill(dt_返工原因表);
                }
                repositoryItemSearchLookUpEdit1.DataSource = dt_返工原因表;
                repositoryItemSearchLookUpEdit1.DisplayMember = "fgyy";
                repositoryItemSearchLookUpEdit1.ValueMember = "fgyy";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //20160222 ZF end

        //输入合格数量 ，计算 不合格数量      送检数量-合格数量
#pragma warning disable IDE1006 // 命名样式
        private void txt_passnum_TextChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (txt_passnum.Text != "")
                {
                    //检查合格数是否小于送检数
                    if (Convert.ToDecimal(txt_passnum.Text) > Convert.ToDecimal(txt_checknum.Text))
                    {
                        MessageBox.Show("合格数量大于送检数,重新输入");
                        txt_passnum.Text = "";
                        return;
                    }
                    Decimal dec_送检数量 = Convert.ToDecimal(txt_checknum.Text);
                    Decimal dec_合格数量;

                    dec_合格数量 = Convert.ToDecimal(txt_passnum.Text);
                    string a = Convert.ToString(dec_送检数量 - dec_合格数量);
                    txt_NGnum.Text = a;
                    txt_重检合格数.Text = a;
                    txt_报废数.Text = "0";
                    //ZF
                    txt_返工数量.Text = a;
                    Decimal de_一次合格 = (dec_合格数量 / dec_送检数量);
                    txt_一次合格率.Text = de_一次合格.ToString("00.00%");
                    Decimal de_总计合格 = 0;
                    if (txt_重检合格数.Text != "")
                    {
                        Decimal dec_重检合格数量 = Convert.ToDecimal(txt_重检合格数.Text);
                        de_总计合格 = (dec_合格数量 + dec_重检合格数量) / dec_送检数量;
                    }
                    else
                    {
                        de_总计合格 = dec_合格数量 / dec_送检数量;
                    }
                    txt_总计合格率.Text = de_总计合格.ToString("00.00%");
                }
                else
                {
                    txt_NGnum.Text = "";
                    txt_报废数.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //当送检人员ID发生变化时，姓名也要发生变化 
#pragma warning disable IDE1006 // 命名样式
        private void txt_songjianren_EditValueChanged_1(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                //测试加下面一行[EditVal


                DataRow[] dr = dt_people.Select(string.Format("员工号='{0}'", txt_songjianren.EditValue.ToString()));
                if (dr.Length > 0)
                {
                    txt_songjianrenName.Text = dr[0]["姓名"].ToString();
                    txt_部门.Text = dr[0]["部门"].ToString();
                    txt_岗位.Text = dr[0]["岗位"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_清空()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = "select * from 快速检验生产检验单主表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                DataRow dr = dt.NewRow();
                dataBindHelper2.DataFormDR(dr);
                txt_checkdanID.Text = "";
                txt_checknum.Text = "";
                txt_passnum.Text = "";
                txt_NGnum.Text = "";
                txt_一次合格率.Text = "";
                txt_返工数量.Text = "";
                txt_重检结论.Text = "合格";
                str_检验单号 = "";
                textBox4.Text = "";
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_mb()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = "select * from 基础数据基础属性表 where 属性类别 = '盒贴模板' order by  属性值";
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            da.Fill(dt);
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow r in dt.Rows)
                {
                    if (r["属性类别"].ToString() == "盒贴模板")
                    {
                        comboBox1.Items.Add(r["属性值"].ToString());
                    }

                }
            }
            sql = "select * from 基础数据基础属性表 where 属性类别 = '箱贴模板' order by  属性值";
            dt = new DataTable();
            da = new SqlDataAdapter(sql, strcon);
            da.Fill(dt);
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow r in dt.Rows)
                {

                    comboBox3.Items.Add(r["属性值"].ToString());

                }
            }

        }
#pragma warning disable IDE1006 // 命名样式
        private DataTable fun_save()
#pragma warning restore IDE1006 // 命名样式
        {
            //仓库 "select 仓库号,仓库名称 from 基础数据物料信息表 where 物料编码 ='" + txt_chanpinID+"'"

            DataTable dt_仓 = new DataTable();
            string str3 = string.Format("select * from 基础数据物料信息表 where 物料编码='{0}'", txt_chanpinID.Text);
            using (SqlDataAdapter da1 = new SqlDataAdapter(str3, strcon))
            {

                da1.Fill(dt_仓);
            }

            string sql_Gd = "select * from 快速检验生产记录生产工单表 where 1<>1";
            DateTime t = CPublic.Var.getDatetime();


            using (SqlDataAdapter da = new SqlDataAdapter(sql_Gd, strcon))
            {

                DataRow[] dr = dt_工单.Select(string.Format("生产工单号='{0}'", txt_shenchgdh.Text));

                string sql_jyd = string.Format("select * from 快速检验生产检验单主表 where 1<>1 ");
                using (SqlDataAdapter da1 = new SqlDataAdapter(sql_jyd, strcon))
                {
                    drr["仓库号"] = dt_仓.Rows[0]["仓库号"].ToString();
                    drr["仓库名称"] = dt_仓.Rows[0]["仓库名称"].ToString();
                    drr["生产工单类型"] = dr[0]["生产工单类型"];
                    drr["生产工单号"] = dr[0]["生产工单号"];
                    drr["生产数量"] = dr[0]["生产数量"];
                    drr["特殊备注"] = dr[0]["特殊备注"];
                    if (str_入库单号 != "") drr["东屋入库单号"] = str_入库单号.ToString();

                    //if(dr[0]["东屋库存"] != null && Convert.ToDecimal(dr[0]["东屋库存"])!=0)
                    //{
                    // drr["东屋库存"] =dr[0]["东屋库存"].ToString();
                    //}
                    if (dr[0]["镀层"] != null)
                    {
                        drr["镀层"] = dr[0]["镀层"].ToString();
                    }
                    if (str_检验单号 == "")   //新增
                    {
                        drr["GUID"] = System.Guid.NewGuid();
                        str_检验单号 = string.Format("IN{0}{1:D2}{2:00}{3:0000}",
                             t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("IN", t.Year, t.Month));

                        if (str_检验单号 == "")
                        {
                            throw new Exception("检验单号为空 请 刷新再做");
                        }
                        txt_checkdanID.Text = str_检验单号;
                        drr["创建日期"] = t;
                        drr["修改日期"] = t;
                        drr["检验日期"] = t;
                    }
                    else      //修改 
                    {
                        drr["修改日期"] = t;
                    }
                    if (txt_盒装数量.Text == "")
                    {
                        txt_盒装数量.Text = "0";
                    }
                    if (str_检验单号 == "")
                    {
                        throw new Exception("检验单号为空 请 刷新再做");
                    }
                    if (txt_checkdanID.Text == "")
                    {
                        throw new Exception("检验单号为空 请 刷新再做");
                    }
                    dataBindHelper2.DataToDR(drr);
                    drr["送检日期"] = txt_songjiantime.EditValue;
                    drr["送检部门"] = txt_部门.Text;  //就是工单负责人  工单负责人部门
                    drr["规格型号"] = dr[0]["规格型号"];
                    drr["送检数量"] = Convert.ToDecimal(txt_checknum.Text);
                    drr["已检验数量"] = Convert.ToDecimal(txt_checknum.Text);
                    drr["未检验数量"] = Convert.ToDecimal(txt_productnum.Text) - Convert.ToDecimal(drr["已检验数量"]);
                    drr["合格数量"] = Convert.ToDecimal(txt_passnum.Text);
                    drr["不合格数量"] = Convert.ToDecimal(txt_NGnum.Text);
                    drr["一次合格率"] = txt_一次合格率.Text;
                    drr["返工数量"] = Convert.ToDecimal(txt_返工数量.Text);
                    drr["备注1"] = textBox4.Text.ToString();
                    drr["结论"] = (com_结论.EditValue);
                    try
                    {
                        drr["重检结论"] = (txt_重检结论.Text);
                        drr["重检合格数"] = Convert.ToDecimal(txt_重检合格数.Text);
                        drr["报废数"] = Convert.ToDecimal(txt_报废数.Text);
                    }
                    catch (Exception)
                    {
                        drr["重检合格数"] = 0;
                        drr["报废数"] = 0;
                    }
                    drr["总计合格率"] = txt_总计合格率.Text;

                    drr["加急状态"] = dr[0]["加急状态"];
                    drr["修改日期"] = t;
                    //  drr["原规格型号"] = dr[0]["原规格型号"];
                    drr["操作人员ID"] = CPublic.Var.LocalUserID;
                    drr["操作人员"] = CPublic.Var.localUserName;
                    drr["检验人员ID"] = CPublic.Var.LocalUserID;
                    drr["检验人员"] = CPublic.Var.localUserName;
                    //11/12 
                    drr["生效"] = 1;
                    drr["生效人员ID"] = CPublic.Var.LocalUserID;
                    drr["生效人员"] = CPublic.Var.localUserName;
                    drr["生效日期"] = t;

                    try
                    {
                        //if (txt_盒贴模板.Text == "")
                        //{
                        //    txt_盒贴模板.Text = "";
                        //}
                        if (comboBox1.Text == "")
                        {
                            comboBox1.Text = "";
                        }
                        if (txt_电压.Text == "")
                        {
                            txt_电压.Text = "";
                        }
                        if (txt_客户料号.Text == "")
                        {
                            txt_客户料号.Text = "";
                        }
                        if (txt_盒装数量.Text == "")
                        {
                            txt_盒装数量.Text = "0";
                        }
                        if (txt_规格型号.Text == "")
                        {
                            txt_规格型号.Text = "";
                        }
                        if (txt_物料名称.Text == "")
                        {
                            txt_物料名称.Text = "";
                        }
                        //if (txt_参数.Text == "")
                        //{
                        //    txt_参数.Text = "";
                        //}
                        if (comboBox2.Text == "")
                        {
                            comboBox2.Text = "";
                        }
                        if (txt_机种.Text == "")
                        {
                            txt_机种.Text = "";
                        }
                        if (txt_订单号.Text == "")
                        {
                            txt_订单号.Text = "";
                        }
                        //dataBindHelper2.DataToDR(drr);
                    }
                    catch (Exception ex)
                    {
                        CZMaster.MasterLog.WriteLog("成品检验", ex.Message);

                    }
                    //new SqlCommandBuilder(da1);
                    //da1.Update(dtM);
                    DataTable dt = dtM.Copy();
                    return (dt);
                }
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {
            gvP.CloseEditor();
            this.BindingContext[dt_返工].EndCurrentEdit();

            double d_不合格 = 0;
            if (txt_songjianrenName.Text.ToString() == "")
            {
                throw new Exception("请选择负责人");
            }
            if (gvP.RowCount > 0)
            {

                try
                {

                    d_不合格 = Convert.ToDouble(txt_NGnum.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("不合格数量有问题");
                }

                decimal a_新增的返工数量 = 0;
                foreach (DataRow dr in dt_返工.Rows)
                {

                    if (dr["返工原因"].ToString() == "")
                    {
                        throw new Exception("返工原因为空,请检查");
                    }
                    if (dr["数量"].ToString() == "")
                    {
                        throw new Exception("返工数量为空,请检查");
                    }

                    int a = Convert.ToInt32(dr["数量"]);
                    if (a <= 0)
                    {
                        throw new Exception("输入返工数量不能小于等于0");
                    }
                    if (a > d_不合格)
                    {
                        throw new Exception("返工数量大于不合格数量,请检查");
                    }

                    a_新增的返工数量 += Convert.ToDecimal(dr["数量"]);
                }
                decimal a_返工数量1 = Convert.ToDecimal(txt_重检合格数.Text);
                if (Convert.ToDecimal(a_新增的返工数量) != a_返工数量1)
                {
                    throw new Exception("请正确录入插入返工原因中的返工数量");
                }



            }

            if (Convert.ToDecimal(txt_checknum.Text) <= 0)
            {
                throw new Exception("送检数量不能小于0");

            }
            if (test_暂存.Rows.Count > 0)
            {
                if (test_暂存.Rows.Count != Convert.ToInt32(txt_checknum.Text))
                {
                    throw new Exception("送检数量不等于产品序列号数量");
                }

            }
        }
        #endregion

        #region  界面的操作
        //保存操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_save();
                fun_保存检验明细();
                fun_保存返工信息();
                MessageBox.Show("保存成功");
                //加载一遍
                string sql = string.Format("select * from 快速检验生产检验单主表 where 生产检验单号 = '{0}'", str_检验单号);
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                da.Fill(dtM);
                drr = dtM.Rows[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        //生效
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (MessageBox.Show("是否生效？", "询问？", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }

                fun_check();

                DataTable dt_主 = fun_save();//保存的事主表dtM    //更新检验主表
                DataTable dt_检验明细 = fun_保存检验明细();   //成品检验检验记录明细表
                DataTable dt_返工原因 = fun_保存返工信息();     //成品检验检验记录返工表
                if (test_暂存.Rows.Count > 0)
                {
                    fun_保存序号();
                }
                string sql_Gd = "select * from 快速检验生产记录生产工单表 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql_Gd, strcon))
                {


                    DataRow[] dr = dt_工单.Select(string.Format("生产工单号='{0}'", txt_shenchgdh.Text));
                    //dr[0]["工单负责人ID"] = txt_songjianren.Text;
                    // dr[0]["工单负责人"] = txt_songjianrenName.Text;

                    dr[0]["在线检验数"] = Convert.ToDecimal(dr[0]["在线检验数"]) + Convert.ToDecimal(txt_checknum.Text);
                   
                }
                //11-12备注
                //生效
                string sql_在线检验数 = "select * from 生产记录生产工单表 where 1<>1";
                //  string sql_工单已检 = "select * from 快速检验生产记录生产工单表 where 1<>1";
                string sql_主 = "select * from 快速检验生产检验单主表 where 1<>1";
                string sql_检验明细 = "select * from 成品检验检验记录明细表 where 1<>1";
                string sql_返工 = "select * from 快速检验检验记录返工表 where 1<>1";
                string sql_序列号 = "select * from 生产检验单与产品序列号对应关系表 where 1<>1";

                string sql_返工序列号 = "select * from 序列号返工原因对应表 where 1<>1";
                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("快速检验");
                try
                {



                    SqlCommand cmm_工单已检 = new SqlCommand(sql_在线检验数, conn, ts);
                    //SqlCommand cmm_工单已检 = new SqlCommand(sql_工单已检, conn, ts);
                    //SqlCommand cmm_生效字段 = new SqlCommand(sql_主, conn, ts);
                    SqlCommand cmm_检验明细 = new SqlCommand(sql_检验明细, conn, ts);
                    SqlCommand cmm_返工 = new SqlCommand(sql_返工, conn, ts);
                    SqlCommand cmm_主 = new SqlCommand(sql_主, conn, ts);
                    SqlCommand cmm_序号 = new SqlCommand(sql_序列号, conn, ts);

                    SqlCommand cmm_返工序号 = new SqlCommand(sql_返工序列号, conn, ts);

                    SqlDataAdapter da_工单已检 = new SqlDataAdapter(cmm_工单已检);
                    SqlDataAdapter da_检验明细 = new SqlDataAdapter(cmm_检验明细);
                    SqlDataAdapter da_返工 = new SqlDataAdapter(cmm_返工);
                    SqlDataAdapter da_主 = new SqlDataAdapter(cmm_主);
                    SqlDataAdapter da_序号 = new SqlDataAdapter(cmm_序号);

                    SqlDataAdapter da_返工序号 = new SqlDataAdapter(cmm_返工序号);
                    //SqlDataAdapter da_生效字段 = new SqlDataAdapter(cmm_生效字段);
                    new SqlCommandBuilder(da_工单已检);
                    new SqlCommandBuilder(da_主);
                    new SqlCommandBuilder(da_检验明细);
                    new SqlCommandBuilder(da_返工);
                    new SqlCommandBuilder(da_序号);

                    new SqlCommandBuilder(da_返工序号);


                    da_工单已检.Update(dt_工单);
                    da_主.Update(dt_主);
                    da_检验明细.Update(dt_检验明细);

                    if (test_暂存.Rows.Count > 0)
                    {
                        if (dt_返工.Rows.Count > 0)
                        {
                            DataTable dt_返工1 = dt_返工.Copy();
                            dt_返工1.Columns.Remove("产品序列号");
                            gvP.CloseEditor();//关闭编辑状态
                            this.BindingContext[dt_返工1].EndCurrentEdit();//关闭编辑状态
                            da_返工.Update(dt_返工1);
                        }
                        DataTable dt_返工序列号2 = dt_返工.Copy();
                        // dt_返工序列号2.Columns.Remove("生产检验单号");
                        da_序号.Update(dt_序列号);
                        da_返工序号.Update(dt_返工序列号2);
                    }
                    else
                    {
                        da_返工.Update(dt_返工);
                    }
                    ts.Commit();
                }
                catch (Exception ex)
                {
                    ts.Rollback();
                    MessageBox.Show(ex.Message);
                }

                MessageBox.Show("生效完成");
                str_生产工单 = "";
                if (test_暂存.Rows.Count > 0)
                {
                    if (dt_返工.Rows.Count > 0)
                    {
                        dt_返工.Columns.Remove("产品序列号");
                    }
                    ii = 0;
                    test_暂存.Clear();

                    string sqld = "delete from 缓存表产品序列号";
                    string sqldd = "delete from 缓存表返工原因";
                    using (SqlCommand cmd = new SqlCommand(sqld, conn))
                    {
                        var a = cmd.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = new SqlCommand(sqldd, conn))
                    {
                        var a = cmd.ExecuteNonQuery();
                    }
                }
                {//作用：刷新界面
                    fun_清空();
                    dt_检验项目保存.Clear();
                    dt_返工.Clear();
                    fun_检验工单代办事项();
                    //载入空的drr
                    string sql_jyd = string.Format("select * from 快速检验生产检验单主表 where 1<>1 ");
                    using (SqlDataAdapter da1 = new SqlDataAdapter(sql_jyd, strcon))
                    {
                        dtM = new DataTable();
                        da1.Fill(dtM);
                        drr = dtM.NewRow();
                        dtM.Rows.Add(drr);
                    }
                }
                textBox1.Focus();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);

            }
        }
        //刷新 
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                // textBox2.Text = null;
                fun_清空();
                fun_检验工单代办事项();
                test_暂存.Clear();
                i = 0;//i=0表示插入返工原因回归初始化
                ii = 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //关闭界面
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }
        #endregion

        //20160222 ZF 
        //插入记录 
        //dt_返工
#pragma warning disable IDE1006 // 命名样式
        private void button1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_返工原因();
        }
        int i = 0;
        int ii = 0;
#pragma warning disable IDE1006 // 命名样式
        private void fun_返工原因()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (test_暂存.Rows.Count > 0 && i == 0)
                {
                    //fun_返工记录();
                    fun_载入返工原因();
                    i = 1;
                }
                if (test_暂存.Rows.Count > 0 && ii == 0)
                {
                    dt_返工.Columns.Add("产品序列号", typeof(string));
                    ii = 1;
                }
                DataRow dr = dt_返工.NewRow();
                dt_返工.Rows.Add(dr);
                if (test_暂存.Rows.Count > 0)
                {

                    DataRow dr_序列号 = gV2.GetDataRow(gV2.FocusedRowHandle);
                    dr["产品序列号"] = dr_序列号["产品序列号"].ToString();
                    dr["数量"] = 1;
                    dr_序列号["状态"] = true;
                    this.gridColumn25.Caption = "返工数量";
                    this.gridColumn25.FieldName = "数量";
                    this.gridColumn25.Name = "gridColumn25";
                    this.gridColumn25.OptionsColumn.AllowEdit = false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //删除记录
#pragma warning disable IDE1006 // 命名样式
        private void button2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataRow dr = gvP.GetDataRow(gvP.FocusedRowHandle);
                if (test_暂存.Rows.Count == 0)
                {
                    dr.Delete();
                }
                else
                {

                    SqlConnection conn = new SqlConnection(strcon);
                    conn.Open();

                    string sqldd = "delete from 缓存表返工原因 where 产品序列号 ='" + dr["产品序列号"] + "'";
                    using (SqlCommand cmd = new SqlCommand(sqldd, conn))
                    {
                        var a = cmd.ExecuteNonQuery();
                    }
                    dt_返工.Rows.Remove(dr);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void button3_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            BaseData.frm成品检验盒贴信息维护 fm = new BaseData.frm成品检验盒贴信息维护();
            CPublic.UIcontrol.AddNewPage(fm, "盒贴信息维护");
        }

#pragma warning disable IDE1006 // 命名样式
        private void textBox2_TextChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //当输入报废数量后，自动计算重检合格数量
            try
            {

                if (txt_报废数.Text != "")
                {
                    //先检查 报废数<=不合格数
                    if ((Convert.ToDecimal(txt_报废数.Text) > (Convert.ToDecimal(txt_NGnum.Text))))
                    {
                        MessageBox.Show("请检查报废数是否填写错误");
                        return;
                    }
                    //txt_NGnum.Text
                    txt_重检合格数.Text = (Convert.ToDecimal(txt_NGnum.Text) - Convert.ToDecimal(txt_报废数.Text)).ToString();
                }
                else
                {
                    txt_重检合格数.Text = "";
                    txt_重检合格数.Text = txt_NGnum.Text.ToString();
                }
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void txt_重检合格数_TextChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                Decimal dec_送检数量 = Convert.ToDecimal(txt_checknum.Text);
                Decimal dec_合格数量;

                dec_合格数量 = Convert.ToDecimal(txt_passnum.Text);
                Decimal de_总计合格 = 0;
                if (txt_重检合格数.Text != "")
                {
                    Decimal dec_重检合格数量 = Convert.ToDecimal(txt_重检合格数.Text);
                    de_总计合格 = (dec_合格数量 + dec_重检合格数量) / dec_送检数量;
                }
                else
                {
                    de_总计合格 = dec_合格数量 / dec_送检数量;
                }
                txt_总计合格率.Text = de_总计合格.ToString("00.00%");
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        //打印
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }
        //送检数量变化事件
#pragma warning disable IDE1006 // 命名样式
        private void txt_checknum_TextChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (txt_checknum.Text != "")
                {
                    //DataRow dr = gv_scgd.GetDataRow(gv_scgd.FocusedRowHandle);
                    if (Convert.ToDecimal(txt_checknum.Text.ToString()) > dee)
                    {
                        throw new Exception("请检查送检数量是否正确");

                    }
                    txt_passnum.Text = "";
                }
                DataTable dt_缓存1;
                using (SqlDataAdapter da = new SqlDataAdapter("select * from 缓存表产品序列号", CPublic.Var.strConn))
                {
                    dt_缓存1 = new DataTable();
                    da.Fill(dt_缓存1);
                }

                if (dt_缓存1.Rows.Count > 0 && dt_返工.Rows.Count == 0)
                {
                    if (dt_缓存1.Rows[0]["生产工单号"].ToString() == txt_shenchgdh.Text.ToString() && dt_缓存1.Rows[0]["检验人员"].ToString() == CPublic.Var.localUserName && Convert.ToDecimal(dt_缓存1.Rows[0]["送检数量"]) == Convert.ToDecimal(txt_checknum.Text))
                    {
                        test_暂存 = dt_缓存1.Copy();
                        gC2.DataSource = test_暂存;

                        using (SqlDataAdapter da1 = new SqlDataAdapter("select * from 缓存表返工原因", CPublic.Var.strConn))
                        {
                            DataTable dt_缓存2 = new DataTable();
                            da1.Fill(dt_缓存2);
                            if (i == 0)
                            {
                                dt_返工.Columns.Add("产品序列号", typeof(string));
                            }
                            dt_缓存2.Columns.Remove("ID");
                            //dt_返工 = dt_缓存2.Copy();
                            foreach (DataRow dr in dt_缓存2.Rows)
                            {
                                DataRow dr1 = dt_返工.NewRow();
                                dr1.ItemArray = dr.ItemArray;
                                dr1["产品序列号"] = dr["产品序列号"].ToString();
                                dt_返工.Rows.Add(dr1);
                            }

                            gcP.DataSource = dt_返工;
                            ii = 1;
                            i = 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //搜索工单 修改 检验单 的
#pragma warning disable IDE1006 // 命名样式
        private void button4_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            盒贴信息修改 frm = new 盒贴信息修改();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();



        }

#pragma warning disable IDE1006 // 命名样式
        private void comboBox1_TextChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            txt_规格型号.Enabled = false;
            txt_客户料号.Enabled = false;
            txt_物料名称.Enabled = false;
            txt_机种.Enabled = false;
            txt_订单号.Enabled = false;
            txt_电压.Enabled = false;
            comboBox2.Enabled = false;
            label24.Text = "机种";
            label21.Text = "产品型号：";
            label22.Text = "产品名称：";

            if (comboBox1.Text.Trim() == "通用模板")
            {
                label21.Text = "产品型号：";
                label22.Text = "产品名称：";
                txt_规格型号.Enabled = true;
                txt_电压.Enabled = true;
                txt_客户料号.Enabled = true;
                txt_物料名称.Enabled = true;
            }
            if (comboBox1.Text.Trim() == "通用模板电流")
            {
                label21.Text = "产品型号：";
                label22.Text = "产品名称：";
                txt_规格型号.Enabled = true;
                txt_电压.Enabled = true;
                txt_客户料号.Enabled = true;
                txt_物料名称.Enabled = true;
            }
            if (comboBox1.Text.Trim() == "中性模板")
            {
                label21.Text = "产品型号：";
                label22.Text = "产品名称：";
                txt_规格型号.Enabled = true;
                txt_电压.Enabled = true;
                txt_客户料号.Enabled = true;
                txt_物料名称.Enabled = true;
            }
            if (comboBox1.Text.Trim() == "常熟模板")
            {
                label21.Text = "产品型号：";
                label22.Text = "产品名称：";
                txt_规格型号.Enabled = true;
                txt_电压.Enabled = true;
                txt_客户料号.Enabled = true;
                txt_物料名称.Enabled = true;
            }
            if (comboBox1.Text.Trim() == "正泰模板")
            {
                label21.Text = "适配断路器：";
                label22.Text = "附件名称：";
                txt_规格型号.Enabled = true;
                txt_电压.Enabled = true;
                comboBox2.Enabled = true;
                txt_客户料号.Enabled = true;
                txt_物料名称.Enabled = true;
            }
            if (comboBox1.Text.Trim() == "宁波施耐德")
            {
                label21.Text = "型号规格：";
                label22.Text = "产品名称：";
                txt_客户料号.Enabled = true;
                txt_物料名称.Enabled = true;
                txt_规格型号.Enabled = true;
            }
            if (comboBox1.Text.Trim() == "温州德力西")
            {
                label21.Text = "零部件名称：";
                label22.Text = "零部件编码：";
                txt_规格型号.Enabled = true;
                txt_物料名称.Enabled = true;
            }
            if (comboBox1.Text.Trim() == "台安模板")
            {
                label21.Text = "型号：";
                label22.Text = "品名：";
                txt_规格型号.Enabled = true;
                txt_物料名称.Enabled = true;
                txt_客户料号.Enabled = true;
                txt_机种.Enabled = true;
                txt_订单号.Enabled = true;
                txt_电压.Enabled = true;
            }
            if (comboBox1.Text.Trim() == "诺雅克模板")
            {
                label21.Text = "规格型号：";
                label22.Text = "物料名称：";
                txt_规格型号.Enabled = true;
                txt_物料名称.Enabled = true;
                txt_客户料号.Enabled = true;
                txt_电压.Enabled = true;
            }
            if (comboBox1.Text.Trim() == "分励英文模板")
            {
                label21.Text = "规格型号：";
                label22.Text = "物料名称：";
                txt_规格型号.Enabled = true;
                txt_物料名称.Enabled = true;
                txt_客户料号.Enabled = true;
                txt_电压.Enabled = true;
            }
            if (comboBox1.Text.Trim() == "闭合英文模板")
            {
                label21.Text = "规格型号：";
                label22.Text = "物料名称：";
                txt_规格型号.Enabled = true;
                txt_物料名称.Enabled = true;
                txt_客户料号.Enabled = true;
                txt_电压.Enabled = true;
            }
            if (comboBox1.Text.Trim() == "欠压英文模板")
            {
                label21.Text = "规格型号：";
                label22.Text = "物料名称：";
                txt_规格型号.Enabled = true;
                txt_客户料号.Enabled = true;
                txt_物料名称.Enabled = true;
                txt_电压.Enabled = true;
            }
            if (comboBox1.Text.Trim() == "辅助英文模板")
            {
                label21.Text = "规格型号：";
                label22.Text = "物料名称：";
                txt_规格型号.Enabled = true;
                txt_物料名称.Enabled = true;
                txt_客户料号.Enabled = true;
                txt_电压.Enabled = true;
            }
            if (comboBox1.Text.Trim() == "辅报英文模板")
            {
                label21.Text = "规格型号：";
                label22.Text = "物料名称：";
                txt_规格型号.Enabled = true;
                txt_物料名称.Enabled = true;
                txt_客户料号.Enabled = true;
                txt_电压.Enabled = true;
            }
            if (comboBox1.Text.Trim() == "报警英文模板")
            {
                label21.Text = "规格型号：";
                label22.Text = "物料名称：";
                txt_规格型号.Enabled = true;
                txt_物料名称.Enabled = true;
                txt_客户料号.Enabled = true;
                txt_电压.Enabled = true;
            }
            if (comboBox1.Text.Trim() == "芜湖德力西")
            {


                label21.Text = "规格型号：";
                label22.Text = "物料名称：";
                label24.Text = "对方型号：";
                txt_规格型号.Enabled = true;
                txt_物料名称.Enabled = true;
                txt_客户料号.Enabled = true;
                txt_订单号.Enabled = true;
                txt_机种.Enabled = true;
            }
            if (comboBox1.Text.Trim() == "芜湖德力西英文")
            {
                txt_订单号.Enabled = true;

                label21.Text = "规格型号：";
                label22.Text = "物料名称：";
                label24.Text = "对方型号：";
                txt_规格型号.Enabled = true;
                txt_物料名称.Enabled = true;
                txt_客户料号.Enabled = true;
                txt_订单号.Enabled = true;
                txt_机种.Enabled = true;

            }
            if (comboBox1.Text.Trim() == "宏美模板")
            {
                label21.Text = "规格型号：";
                label22.Text = "物料名称：";
                label24.Text = "LOT/SN";
                txt_规格型号.Enabled = true;
                txt_物料名称.Enabled = true;
                txt_客户料号.Enabled = true;
                txt_机种.Enabled = true;
                txt_订单号.Enabled = true;
            }
            if (comboBox1.Text.Trim() == "正泰英文版")
            {
                label21.Text = "型号规格：";
                label22.Text = "产品名称：";
                txt_规格型号.Enabled = true;
                txt_电压.Enabled = true;
                txt_物料名称.Enabled = true;
            }
            if (comboBox1.Text.Trim() == "常熟外发模板")
            {
                label21.Text = "型号规格：";
                label22.Text = "产品名称：";
                txt_规格型号.Enabled = true;
                txt_物料名称.Enabled = true;
            }
            if (comboBox1.Text.Trim() == "良信模板")
            {
                label21.Text = "规格";
                label22.Text = "品名";
                txt_规格型号.Enabled = true;
                txt_电压.Enabled = true;
                txt_客户料号.Enabled = true;
                txt_物料名称.Enabled = true;
            }


            if (comboBox1.Text.Trim() == "正泰模板")
            {
                label21.Text = "适配断路器：";
                label22.Text = "附件名称：";

            }

            else if (comboBox1.Text.Trim() == "温州德力西")
            {
                label21.Text = "零部件名称：";
                label22.Text = "零部件编码：";

            }
            else if (comboBox1.Text.Trim() == "台安模板")
            {
                label21.Text = "型号：";
                label22.Text = "品名：";

            }
 
            else
            {
                label21.Text = "规格型号：";
                label22.Text = "物料名称：";
                label24.Text = "机种";
            }
            if (dt_车间信息.Rows[0]["生产车间"].ToString().Trim() == "0001030104" && (txt_订单号.Text.Trim() == "" || txt_订单号.Text.Trim() == txt_shenchgdh.Text.ToString().Trim()))
            {

                if (comboBox1.Text.Trim() == "芜湖德力西英文" || comboBox1.Text.Trim() == "芜湖德力西")
                {
                    txt_订单号.Text = txt_shenchgdh.Text.ToString().Trim();
                }
                else
                {
                    txt_订单号.Text = "";

                }
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void button5_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            if (MessageBox.Show("是否要保存？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                DataTable dt = new DataTable();
                DataRow dr_z = gv_scgd.GetDataRow(gv_scgd.FocusedRowHandle);
                string sql = string.Format("select * from BQ_HZXX where  wlbh='{0}'", dr_z["原ERP物料编号"]);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        if (MessageBox.Show("已有该物料模板信息,是否覆盖？", "警告", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            DataRow dr = dt.Rows[0];
                            dataBindHelper3.DataToDR(dt.Rows[0]);
                            dr["dymb"] = dr["mbmc"];
                            if (label24.Text != "机种" || label24.Text != "LOT/SN")
                            {
                                dr["ggxh"] = txt_机种.Text;
                            }
                            else
                            {
                                dr["jz"] = txt_机种.Text;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        dr["wlbh"] = dr_z["原ERP物料编号"];
                        dataBindHelper3.DataToDR(dr);
                        if (label24.Text != "机种" || label24.Text != "LOT/SN")
                        {
                            dr["ggxh"] = txt_机种.Text;
                        }
                        else
                        {
                            dr["jz"] = txt_机种.Text;
                        }
                        dt.Rows.Add(dr);


                    }
                    new SqlCommandBuilder(da);
                    da.Update(dt);
                    MessageBox.Show("ok");
                }
                //MessageBox.Show("保存成功");
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_scgd_KeyDown(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {


            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv_scgd.GetFocusedRowCellValue(gv_scgd.FocusedColumn));
                e.Handled = true;
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
        private void gv_scgd_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem7_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem7_ItemClick_2(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_scgd_CustomFilterDisplayText(object sender, DevExpress.XtraEditors.Controls.ConvertEditValueEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void timer1_Tick(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            textBox1.Focus();
            textBox1.Text = "";

        }

#pragma warning disable IDE1006 // 命名样式
        private void textBox1_TextChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (textBox1.Text.Length == 12)
            {

                gv_scgd.FocusedRowHandle = gv_scgd.LocateByDisplayText(0, gridColumn6, textBox1.Text);
                gv_scgd_RowCellClick_1(null, null);
                textBox1.Text = "";
                textBox1.Focus();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }
        # region 导入DW成品数信息
        DataTable dt_DW = new DataTable();
#pragma warning disable IDE1006 // 命名样式
        private void fun_加载成品信息()
#pragma warning restore IDE1006 // 命名样式
        {
            string str;
            if (textBox2.Text.ToString() != "")
            {
                str = @" select b.*,a.*,e.* from mom_orderdetail b 
                       left join mom_order a on a.MoId=b.MoId
                       left join Inventory e on b.InvCode = e.cInvCode
                       where a.MoCode='" + "DWSD20" + textBox2.Text + "'";
            }
            else
            {
                //e.bSelf ='true' 是否自制   e.bPurchase = 'false' 是否外购
                str = @" select b.*,a.*,e.* from mom_orderdetail b 
                       left join mom_order a on a.MoId=b.MoId
                       left join Inventory e on b.InvCode = e.cInvCode
                       where a.MoCode='" + str_生产工单 + "'";
            }
            using (SqlDataAdapter da = new SqlDataAdapter(str, strconn2))
            {
                da.Fill(dt_DW);
            }
        }
        DataTable dt_生产工单 = new DataTable();
#pragma warning disable IDE1006 // 命名样式
        private void fun_保存工单表和基础表()
#pragma warning restore IDE1006 // 命名样式
        {
            string str;
            if (textBox2.Text.ToString() != "")
            {
                str = "select * from 快速检验生产记录生产工单表 where 备注5 ='" + "DWSD20" + textBox2.Text + "'";
            }
            else
            {
                str = "select * from 快速检验生产记录生产工单表 where 备注5 ='" + str_生产工单 + "'";
            }
            DataTable dt_判断工单是否存在 = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(str, CPublic.Var.strConn))
            {

                da.Fill(dt_判断工单是否存在);
            }
            if (dt_判断工单是否存在.Rows.Count == 0 && dt_DW.Rows.Count > 0)
            {

                //DataView dv = dt_DW.DefaultView;
                //dv.RowFilter = "cInvCode = '10'";
                //DataRow[] dr_筛选 = dt_DW.Select(string.Format("",));
                using (SqlDataAdapter da1 = new SqlDataAdapter("select * from 快速检验生产记录生产工单表 where 1<>1", CPublic.Var.strConn))
                {
                    da1.Fill(dt_生产工单);
                    foreach (DataRow dr_DW in dt_DW.Rows)
                    {
                        // MessageBox.Show(dr_DW["cInvCode"].ToString().Substring(0,2));
                        //if (dr_DW["cInvCode"].ToString().Substring(0,2) == "10")
                        //{

                        DataRow dr_生产工单 = dt_生产工单.NewRow();
                        dt_生产工单.Rows.Add(dr_生产工单);
                        DateTime t = CPublic.Var.getDatetime();
                        string a1 = t.Year.ToString().Substring(2, 2);
                        string a = string.Format("MO{0}{1:00}{2:00}{3:0000}", a1, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("MO", t.Year, t.Month));


                        dr_生产工单["生产工单号"] = a;
                        dr_生产工单["GUID"] = System.Guid.NewGuid();
                        dr_生产工单["物料编码"] = dr_DW["cInvCode"].ToString();
                        //DataTable dt_东屋库存 = new DataTable();
                        //string sfr =string.Format("select cInvCode ,SUM(iQuantity)数量 from currentstock where cInvCode ='{0}' group by cInvCode,cWhCode",dr_DW["cInvCode"].ToString());
                        //using (SqlDataAdapter da2 = new SqlDataAdapter(sfr,strconn2))
                        //{
                        //    da2.Fill(dt_东屋库存);
                        //}
                        //if(dt_东屋库存.Rows.Count>0)
                        //{
                        //    dr_生产工单["东屋库存"] = dt_东屋库存.Rows[0]["数量"].ToString(); 
                        //}
                        dr_生产工单["物料名称"] = dr_DW["cInvName"].ToString();
                        dr_生产工单["规格型号"] = dr_DW["cInvStd"].ToString();
                        //dr_生产工单["原规格型号"] = dr_DW["cInvStd"].ToString();
                        dr_生产工单["生产数量"] = dr_DW["Qty"].ToString();
                        //dr_生产工单["未检验数量"] = dr_DW["Qty"].ToString();
                        dr_生产工单["生产车间"] = dr_DW["MDeptCode"].ToString();
                        dr_生产工单["仓库号"] = dr_DW["WhCode"].ToString();
                        if (str_生产工单 != "")
                        {
                            dr_生产工单["备注5"] = str_生产工单;
                        }
                        else
                        {
                            if (textBox2.Text.ToString() != "") dr_生产工单["备注5"] = "DWSD20" + textBox2.Text.ToString();

                        }
                        // dr_生产工单["已检验数量"] = dr_DW["QualifiedInQty"].ToString();
                        dr_生产工单["生效"] = true;
                        dr_生产工单["加急状态"] = "正常";
                        dr_生产工单["完工"] = true;
                        // dr_生产工单["东屋入库单号"] = str_入库单号;
                        //dr_生产工单["已检验数量"] = DBNull.Value;
                        //bool b = dr_生产工单["已检验数量"].Equals(DBNull.Value);
                        //if (b == true)
                        //{
                        dr_生产工单["未检验数量"] = Convert.ToDecimal(dr_DW["Qty"]);
                        if (dr_DW["Free1"] != null)
                        {
                            dr_生产工单["镀层"] = dr_DW["Free1"].ToString();
                        }
                        //}
                    }
                }

                fun_基础表();
                fun_save_DW();
            }
            //else
            //{
            //    MessageBox.Show("单号已存在 或者 请正确输入单号");

            //}
        }

        DataTable dt_基础表 = new DataTable();
#pragma warning disable IDE1006 // 命名样式
        private void fun_基础表()
#pragma warning restore IDE1006 // 命名样式
        {
            using (SqlDataAdapter da = new SqlDataAdapter("select * from 基础数据物料信息表 where 1<>1", CPublic.Var.strConn))
            {

                da.Fill(dt_基础表);
                foreach (DataRow dr_基础表 in dt_DW.Rows)
                {
                    string stee = "select * from 基础数据物料信息表 where 物料编码 ='" + dr_基础表["cInvCode"].ToString() + "'";
                    DataTable dt_物料是否已存在 = new DataTable();
                    using (SqlDataAdapter da1 = new SqlDataAdapter(stee, CPublic.Var.strConn))
                    {

                        da1.Fill(dt_物料是否已存在);
                    }
                    if (dt_物料是否已存在.Rows.Count == 0 && dt_基础表.Select("物料编码 ='" + dr_基础表["cInvCode"].ToString() + "'").Length == 0)
                    {
                        DataRow dr_存入基 = dt_基础表.NewRow();
                        dt_基础表.Rows.Add(dr_存入基);
                        dr_存入基["物料编码"] = dr_基础表["cInvCode"].ToString();
                        dr_存入基["原ERP物料编号"] = dr_基础表["cInvCode"].ToString();
                        dr_存入基["物料名称"] = dr_基础表["cInvName"].ToString();
                        dr_存入基["规格型号"] = dr_基础表["cInvStd"].ToString();
                        dr_存入基["n原ERP规格型号"] = dr_基础表["cInvStd"].ToString();
                        dr_存入基["大类GUID"] = dr_基础表["cInvCCode"].ToString();
                        dr_存入基["供应商编号"] = dr_基础表["cVenCode"].ToString();
                        if (dr_基础表["iInvRCost"].ToString() != "")
                        {
                            dr_存入基["标准单价"] = Convert.ToDecimal(dr_基础表["iInvRCost"]);
                        }
                        if (dr_基础表["iTopSum"].ToString() != "")
                        {
                            dr_存入基["库存上限"] = Convert.ToDouble(dr_基础表["iTopSum"]);
                        }
                        if (dr_基础表["iLowSum"].ToString() != "")
                        {
                            dr_存入基["库存下限"] = Convert.ToDouble(dr_基础表["iLowSum"]);
                        }
                        if (dr_基础表["iInvWeight"].ToString() != "")
                        {
                            dr_存入基["克重"] = Convert.ToDecimal(dr_基础表["iInvWeight"]);
                        }
                        dr_存入基["生效时间"] = dr_基础表["dSDate"].ToString();
                        if (dr_基础表["dEDate"].ToString() != "")
                        {
                            dr_存入基["停用时间"] = dr_基础表["dEDate"].ToString();
                        }

                        dr_存入基["仓库号"] = dr_基础表["cDefWareHouse"].ToString();

                    }
                }
            }


        }

        //保存 2张表
#pragma warning disable IDE1006 // 命名样式
        private void fun_save_DW()
#pragma warning restore IDE1006 // 命名样式
        {
            SqlConnection conn = new SqlConnection(CPublic.Var.strConn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("生效");
            string sql1 = "select * from  快速检验生产记录生产工单表 where 1<>1";
            SqlCommand cmd1 = new SqlCommand(sql1, conn, ts);
            SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
            new SqlCommandBuilder(da1);


            string sql2 = "select * from 基础数据物料信息表  where 1<>1";
            SqlCommand cmd2 = new SqlCommand(sql2, conn, ts);
            SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
            new SqlCommandBuilder(da2);

            try
            {
                da1.Update(dt_生产工单);
                da2.Update(dt_基础表);



                ts.Commit();
                MessageBox.Show("ok");
                fun_检验工单代办事项();
                //textBox2.Text = null;
                dt_DW.Clear();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                MessageBox.Show(ex.Message);

            }


        }
        //导入DW成品信息
#pragma warning disable IDE1006 // 命名样式
        private void button6_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (textBox2.Text.ToString() == "")
                {
                    throw new Exception("没有信息可查询");
                }
                fun_加载成品信息();
                fun_保存工单表和基础表();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        #endregion
        // 查看明细
        Decimal dee = 0;
#pragma warning disable IDE1006 // 命名样式
        private void gv_scgd_RowCellClick_1(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                //主表
                DataRow r = gv_scgd.GetDataRow(gv_scgd.FocusedRowHandle);
                //检验项目
                fun_检验项目(r);
                //返工原因
                fun_返工记录();
                if (test_暂存.Rows.Count > 0)
                {
                    ii = 0;
                }

                // txt_部门名称.Text = r["部门名称"].ToString();   人事信息有了才能添加
                //dee = Convert.ToDecimal(r["未检验数量"]);
                dee = Convert.ToDecimal(r["生产数量"]) - Convert.ToDecimal(r["在线检验数"]);

                fun_清空();
                //txt_shenchgdh.Text = r["生产工单号"].ToString();

                //txt_chanpinID.Text = r["物料编码"].ToString();
                //txt_chanpinName.Text = r["物料名称"].ToString();
                //txt_productnum.Text = r["生产数量"].ToString();
                //txt_checknum.Text = r["未检验数量"].ToString();
                //txt_身缠车间.Text = r["生产车间"].ToString();

                dataBindHelper2.DataFormDR(r);
                if (r["部分完工"].Equals(true))
                {
                    if (Convert.ToInt32(r["部分完工数"].ToString()) == Convert.ToInt32(r["上次完工数"].ToString()))
                    {
                        txt_checknum.Text = r["上次完工数"].ToString();

                    }
                    else
                    {
                        txt_checknum.Text = (Convert.ToDecimal(r["部分完工数"]) - Convert.ToDecimal(r["在线检验数"])).ToString();
                    }
                }
                txt_原规格型号.Text = r["规格型号"].ToString();
                txt_checknum.Text = Convert.ToDecimal(txt_checknum.Text).ToString("f2");
                if (r["生产工单号"].ToString() == str000) txt_checknum.Text = str_检验入库数量.ToString("f2");
                txt_passnum.Text = txt_checknum.Text;
                com_结论.EditValue = "合格";

                if (txt_箱装数量.Text == "")
                {

                    txt_箱装数量.Text = "0";
                }
                //if (r["部分完工"].Equals(true))
                //{
                //    txt_checknum.Text = r["上次完工数"].ToString();
                //}
                // int i = 3;


                txt_songjiantime.EditValue = CPublic.Var.getDatetime();
                txt_songjianren.EditValue = r["工单负责人ID"].ToString();
                txt_songjianrenName.Text = r["工单负责人"].ToString();



                try
                {
                    string sql = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"].ToString());
                    DataTable dtttttt = new DataTable();
                    SqlDataAdapter daaaaa = new SqlDataAdapter(sql, strcon);
                    daaaaa.Fill(dtttttt);
                    //盒贴
                    fun_盒贴(dtttttt.Rows[0]["物料编码"].ToString().Trim());
                }
                catch
                {
                    //MessageBox.Show("找不到该物料的盒装信息！");
                }


            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "成品检验-代办事项-单击事件");
            }
        }



#pragma warning disable IDE1006 // 命名样式
        private void gv_scgd_ColumnFilterChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (cfgfilepath != "")
                {
                    gv_scgd.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }

        }
        //20160222 ZF end

        #region 注释掉的代码
        ///// <summary>
        ///// 生效函数
        ///// </summary>
        //private void fun_shengxiao()
        //{
        //    try
        //    {
        //        drm["生效"] = 1;
        //        drm["生效人员ID"] = CPublic.Var.LocalUserID;
        //        drm["生效人员"] = CPublic.Var.localUserName;

        //        SqlDataAdapter da;
        //        SqlConnection conn = new SqlConnection(strcon);
        //        SqlTransaction ts = conn.BeginTransaction("ttt");
        //        SqlCommand cmd = new SqlCommand("select * from 生产记录生产检验单主表 where 1<>1", conn, ts);
        //        SqlCommand cmd1 = new SqlCommand("select * from 生产记录生产检验单明细表 where 1<>1", conn, ts);
        //        try
        //        {
        //            da = new SqlDataAdapter(cmd);
        //            new SqlCommandBuilder(da);
        //            //da.Update(dt_checkDanMain);

        //            da = new SqlDataAdapter(cmd1);
        //            new SqlCommandBuilder(da);
        //            //da.Update(dt_checkDanDetail);

        //            ts.Commit();
        //        }
        //        catch
        //        {
        //            ts.Rollback();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MasterLog.WriteLog(ex.Message + this.Name + " fun_shengxiao");
        //        throw ex;
        //    }
        //}      
        #endregion



        //private delegate void ShowInfoDelegate(ERPorg.Corg.BardCodeHooK.BarCodes barCode);
        //void BarCode_BarCodeEvent(ERPorg.Corg.BardCodeHooK.BarCodes barCode)
        //{

        //    ShowInfo(barCode);
        //    GC.KeepAlive(barCode);
        //} 
        //private void ShowInfo(ERPorg.Corg.BardCodeHooK.BarCodes barCode)
        //{
        //    if (this.InvokeRequired)
        //    {
        //        this.BeginInvoke(new ShowInfoDelegate(ShowInfo), new object[] { barCode });

        //    }    
        //    else
        //    {


        //            gv_scgd.FocusedRowHandle = gv_scgd.LocateByDisplayText(0, gridColumn6, barCode.BarCode);
        //             gv_scgd_RowCellClick(null, null);

        //        //textBox7.Text += barCode.KeyName;
        //        ////MessageBox.Show(barCode.IsValid.ToString());  
        //    }
        //}

        #region
#pragma warning disable IDE1006 // 命名样式
        private void fun_二厂暂存()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = "select * from 生产检验单与产品序列号对应关系表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                DataTable dt_暂存 = new DataTable();
                da.Fill(dt_暂存);
            }

        }


        //enter事件
        DataTable dt_1;
#pragma warning disable IDE1006 // 命名样式
        private void textBox3_KeyDown(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (txt_chanpinID.Text == "")
                {
                    throw new Exception("请先选择你需要检验的工单");
                }
                if (e.KeyCode == Keys.Enter)//如果输入的是回车键
                {
                    test_暂存 = new DataTable();
                    test_暂存 = (DataTable)gC2.DataSource;

                    if (test_暂存 == null)
                    {
                        dt_1 = new DataTable();
                        dt_1.Columns.Add("产品序列号", typeof(string));
                        dt_1.Columns.Add("状态", typeof(bool));
                        DataRow dr = dt_1.NewRow();
                        dr["产品序列号"] = textBox3.Text.ToString();
                        dr["状态"] = false;
                        dt_1.Rows.Add(dr);
                        gC2.DataSource = dt_1;
                        test_暂存 = (DataTable)gC2.DataSource;
                        fun_缓存张表();
                    }
                    else
                    {
                        //此序列号不存在
                        if (test_暂存.Select("产品序列号 ='" + textBox3.Text + "'").Length == 0)
                        {
                            DataRow dr = test_暂存.NewRow();
                            dr["产品序列号"] = textBox3.Text.ToString();
                            dr["状态"] = false;
                            test_暂存.Rows.Add(dr);
                            gC2.DataSource = test_暂存;
                            test_暂存 = (DataTable)gC2.DataSource;
                        }
                        else
                        {
                            gC2.DataSource = test_暂存;
                            MessageBox.Show("序列号重复扫入");

                        }
                        fun_缓存张表();

                    }

                    textBox3.Clear();

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);

            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_缓存张表()
#pragma warning restore IDE1006 // 命名样式
        {
            //缓存产品序列号 表
            using (SqlDataAdapter da = new SqlDataAdapter("select * from 缓存表产品序列号 where 1<>1", CPublic.Var.strConn))
            {
                DataTable dt_缓存序列号 = new DataTable();
                da.Fill(dt_缓存序列号);
                DataRow dr = dt_缓存序列号.NewRow();
                dr["产品序列号"] = textBox3.Text.ToString();
                dr["生产工单号"] = dt_工单.Rows[0]["生产工单号"].ToString();
                dr["检验人员"] = CPublic.Var.localUserName;
                dr["送检数量"] = txt_checknum.Text.ToString();
                dr["状态"] = false;
                dt_缓存序列号.Rows.Add(dr);
                new SqlCommandBuilder(da);
                da.Update(dt_缓存序列号);
            }
            //缓存
            //SqlConnection conn = new SqlConnection(strcon);
            //conn.Open();

            //string sqldd = "delete from 缓存表返工原因";

            //using (SqlCommand cmd = new SqlCommand(sqldd,conn))
            //{
            //    var a = cmd.ExecuteNonQuery();
            //}
            using (SqlDataAdapter da1 = new SqlDataAdapter("select * from 缓存表返工原因 where 1<>1", CPublic.Var.strConn))
            {
                // DataTable dt_返工序列号2 = dt_返工.Copy();

                new SqlCommandBuilder(da1);
                da1.Update(dt_返工);
            }

        }



        DataTable dt_序列号;
        //DataRow dr_产品序列号;
#pragma warning disable IDE1006 // 命名样式
        private void fun_保存序号()
#pragma warning restore IDE1006 // 命名样式
        {
            // dr_产品序列号 = gV2.GetDataRow(gV2.FocusedRowHandle);

            string sql = "select * from 生产检验单与产品序列号对应关系表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                dt_序列号 = new DataTable();
                da.Fill(dt_序列号);
                foreach (DataRow dr_产品序列号 in test_暂存.Rows)
                {
                    DataRow dr2 = dt_序列号.NewRow();
                    dt_序列号.Rows.Add(dr2);
                    dr2["产品序列号"] = dr_产品序列号["产品序列号"].ToString();
                    dr2["生产检验单号"] = txt_checkdanID.Text.ToString();
                }
            }
        }



        //删除一个序列号
#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                DataRow drl = gV2.GetDataRow(gV2.FocusedRowHandle);
                if (drl != null)
                {
                    //drl["产品序列号"]
                    if (ii == 0)
                    {
                        dt_返工.Columns.Add("产品序列号", typeof(string));
                        ii = 1;
                    }
                    if (dt_返工.Select("产品序列号 = '" + drl["产品序列号"] + "'").Length == 0)
                    {

                        SqlConnection conn = new SqlConnection(strcon);
                        conn.Open();

                        string sqldd = "delete from 缓存表产品序列号 where 产品序列号 ='" + drl["产品序列号"] + "'";
                        using (SqlCommand cmd = new SqlCommand(sqldd, conn))
                        {
                            var a = cmd.ExecuteNonQuery();
                        }
                        test_暂存.Rows.Remove(drl);
                    }
                    else
                    {
                        throw new Exception("请先将产品序列对应的返工原因删除");
                    }

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);

            }
        }

        #endregion
        //序号
#pragma warning disable IDE1006 // 命名样式
        private void gV2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }
        //右击
#pragma warning disable IDE1006 // 命名样式
        private void gcP_MouseClick(object sender, MouseEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gcP, new Point(e.X, e.Y));

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataRow drp = gvP.GetDataRow(gvP.FocusedRowHandle);
                if (drp["返工原因"].ToString() != "" && drp["数量"].ToString() != "")
                {
                    fm返工序列号 fm = new fm返工序列号(drp);
                    fm.ShowDialog();

                }
                else
                {
                    throw new Exception("请输入返工数量");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }



#pragma warning disable IDE1006 // 命名样式
        private void gvP_CellValueChanging_1(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //选中物料编码下拉框 显示出来
            try
            {
                if (e.Column.FieldName == "返工原因")
                {

                    DataRow dr_当前行 = gvP.GetDataRow(gvP.FocusedRowHandle);
                    String str = e.Value.ToString();

                    // DataRow[] dr3 = dt_返工原因表.Select(string.Format("fgyy='{0}'", str));
                    DataRow[] dr3 = dt_返工原因表.Select("fgyy = '" + str + "'");
                    if (dr3 != null && dr3.Length > 0)
                    {
                        //DataRow row = dr3[0];
                        dr_当前行["返工编号"] = dr3[0]["fgbh"].ToString();



                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //列有改变出发事件
#pragma warning disable IDE1006 // 命名样式
        private void gv_scgd_ColumnPositionChanged_1(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gv_scgd.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }

        }
        //右击
#pragma warning disable IDE1006 // 命名样式
        private void gC2_MouseClick(object sender, MouseEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gC2, new Point(e.X, e.Y));

            }
        }

        private void 插入返工原因ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //fun_返工记录();
            //fun_载入返工原因();
            //DataRow dr = dt_返工.NewRow();
            //dt_返工.Rows.Add(dr);
            //dr["数量"] = 1;

            //this.gridColumn25.Caption = "返工数量";
            //this.gridColumn25.FieldName = "数量";
            //this.gridColumn25.Name = "gridColumn25";
            //this.gridColumn25.OptionsColumn.AllowEdit = false;
            fun_返工原因();
        }
        //颜色判断
#pragma warning disable IDE1006 // 命名样式
        private void gV2_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow r = gV2.GetDataRow(e.RowHandle);
            if (r != null)
            {
                if (r["状态"].Equals(true))
                {
                    e.Appearance.BackColor = Color.Red;
                }
            }
        }
        //enter
#pragma warning disable IDE1006 // 命名样式
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (e.KeyCode == Keys.Enter)//如果输入的是回车键
                {
                    if (textBox2.Text.ToString() == "")
                    {
                        throw new Exception("没有信息可查询");
                    }
                    fun_加载成品信息();
                    fun_保存工单表和基础表();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
                    //DataTable tt = dtM.Copy();
                    //tt.Columns.Remove("作废");
                    gc_scgd.ExportToXlsx(saveFileDialog.FileName);
                    //ERPorg.Corg.TableToExcel(tt, saveFileDialog.FileName);
                    MessageBox.Show("导出成功");
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void gv_scgd_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                //主表
                DataRow r = gv_scgd.GetDataRow(gv_scgd.FocusedRowHandle);
                //检验项目
                fun_检验项目(r);
                //返工原因
                fun_返工记录();
                if (test_暂存.Rows.Count > 0)
                {
                    ii = 0;
                }

                // txt_部门名称.Text = r["部门名称"].ToString();   人事信息有了才能添加
                //dee = Convert.ToDecimal(r["未检验数量"]);
                dee = Convert.ToDecimal(r["生产数量"]) - Convert.ToDecimal(r["在线检验数"]);

                fun_清空();
                //txt_shenchgdh.Text = r["生产工单号"].ToString();

                //txt_chanpinID.Text = r["物料编码"].ToString();
                //txt_chanpinName.Text = r["物料名称"].ToString();
                //txt_productnum.Text = r["生产数量"].ToString();
                //txt_checknum.Text = r["未检验数量"].ToString();
                //txt_身缠车间.Text = r["生产车间"].ToString();

                dataBindHelper2.DataFormDR(r);
                if (r["部分完工"].Equals(true))
                {
                    if (Convert.ToInt32(r["部分完工数"].ToString()) == Convert.ToInt32(r["上次完工数"].ToString()))
                    {
                        txt_checknum.Text = r["上次完工数"].ToString();

                    }
                    else
                    {
                        txt_checknum.Text = (Convert.ToDecimal(r["部分完工数"]) - Convert.ToDecimal(r["在线检验数"])).ToString();
                    }
                }
                txt_原规格型号.Text = r["规格型号"].ToString();
                txt_checknum.Text = Convert.ToDecimal(txt_checknum.Text).ToString("f2");
                if (r["生产工单号"].ToString() == str000) txt_checknum.Text = str_检验入库数量.ToString("f2");
                txt_passnum.Text = txt_checknum.Text;
                com_结论.EditValue = "合格";

                if (txt_箱装数量.Text == "")
                {

                    txt_箱装数量.Text = "0";
                }
                //if (r["部分完工"].Equals(true))
                //{
                //    txt_checknum.Text = r["上次完工数"].ToString();
                //}
                // int i = 3;


                txt_songjiantime.EditValue = CPublic.Var.getDatetime();
                txt_songjianren.EditValue = r["工单负责人ID"].ToString();
                txt_songjianrenName.Text = r["工单负责人"].ToString();



                try
                {
                    string sql = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"].ToString());
                    DataTable dtttttt = new DataTable();
                    SqlDataAdapter daaaaa = new SqlDataAdapter(sql, strcon);
                    daaaaa.Fill(dtttttt);
                    //盒贴
                    fun_盒贴(dtttttt.Rows[0]["物料编码"].ToString().Trim());
                }
                catch
                {
                    //MessageBox.Show("找不到该物料的盒装信息！");
                }


            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "成品检验-代办事项-单击事件");
            }
        }





        //保存返工原因和序列号
        //private void button7_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (test_暂存.Rows.Count > 0)
        //        {
        //            DataRow dr_序列号 = gV2.GetDataRow(gV2.FocusedRowHandle);
        //            DataTable dt_返工原因序列号 = dt_返工.Copy();
        //            dt_返工原因序列号.Columns.Add("产品序列号", typeof(string));
        //            using (SqlDataAdapter da = new SqlDataAdapter("select * from 序列号返工原因对应表 where 1<>1", CPublic.Var.strConn))
        //            {
        //                foreach (DataRow dr in dt_返工原因序列号.Rows)
        //                {
        //                    dr["产品序列号"] = dr_序列号["产品序列号"].ToString();
        //                }

        //                new SqlCommandBuilder(da);
        //                da.Update(dt_返工原因序列号);
        //                dt_返工.Clear();
        //                MessageBox.Show("保存成功");

        //            }
        //        }
        //        else
        //        {
        //            throw new Exception("产品序列号为空(二厂功能)");
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        MessageBox.Show(ex.Message);

        //    }
        //}





    }
}
