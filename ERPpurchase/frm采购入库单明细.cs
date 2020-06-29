using CZMaster;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
namespace ERPpurchase
{
    public partial class frm采购入库单明细 : UserControl
    {
        #region
        string strcon = "";
        //操作行
        DataRow drm;
        //入库人员
        DataTable dt_入库人员;
        //选择入库明细后，回传的dt

        //入库的主表
        DataTable dt_入库主表;
        //入库的明细表
        DataTable dt_入库明细;
        //入库单号
        string strRKDH = "";
        DataTable dt_库位 = new DataTable();
        string strdanhao = "";
        DataTable dt_仓库;
        DataTable dt_供应商;

        DataTable dt_采购检验单;
        DataTable dt_quanxian;
        DataView dv;
        string cfgfilepath = "";
        #endregion

        public frm采购入库单明细()
        {
            InitializeComponent();
            strcon = CPublic.Var.strConn;

        }

        public frm采购入库单明细(string dh)
        {
            strdanhao = dh;
            InitializeComponent();
            strcon = CPublic.Var.strConn;
        }

        private void Frm采购入库单明细_Load(object sender, EventArgs e)
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
                x.UserLayout(this.splitContainer1, this.Name, cfgfilepath);

                string str_仓库 = string.Format("select * from 人员仓库对应表 where 工号='{0}'", CPublic.Var.LocalUserID);
                dt_仓库 = CZMaster.MasterSQL.Get_DataTable(str_仓库, strcon);

                Fun_下拉框选择项();
                txt_gongysid.EditValue = "";
                txt_rkygh.EditValue = CPublic.Var.LocalUserID;
                txt_rkriqi.EditValue = CPublic.Var.getDatetime();


                if (strdanhao == "") //新增进来的
                {
                    //入库单的主表
                    SqlDataAdapter da;
                    dt_入库主表 = new DataTable();
                    da = new SqlDataAdapter("select * from 采购记录采购单入库主表 where 1<>1", strcon);
                    da.Fill(dt_入库主表);
                    //入库单的明细表
                    dt_入库明细 = new DataTable();
                    da = new SqlDataAdapter(@"select rk.*,cmx.仓库号,cmx.仓库名称 from 采购记录采购单入库明细 rk
                                     left join 基础数据物料信息表 base  on base.物料编码=rk.物料编码
                                     left join 采购记录采购单明细表 cmx on cmx.采购明细号=rk.采购单明细号
                                              where 1<>1", strcon);
                    da.Fill(dt_入库明细);
                    dt_入库明细.Columns.Add("填写入库量", typeof(decimal));
                    dt_入库明细.Columns.Add("剩余入库量", typeof(decimal));
                    dt_入库明细.Columns.Add("库存总数", typeof(decimal));
                    gcrk.DataSource = dt_入库明细;
                    //新建一行
                    drm = dt_入库主表.NewRow();
                    Fun_筛选采购检验单();

                    //textBox3.SelectAll();

                }
                else
                {
                    Fun_查询(strdanhao);
                }
                // timer1.Start();
                textBox3.Focus();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 载入采购检验单的数据：检验单需要是合格的
        ///  弃用
        /// </summary>
        private void Fun_load采购检验单()
        {
            try
            {
                SqlDataAdapter da;
                string sql = "";
                sql = string.Format(@"select jy.*,已送检数,库存总数,kc.货架描述,cmx.仓库号,cmx.仓库名称,未领量 from 采购记录采购单检验主表 jy
                               left join 基础数据物料信息表 base on base.物料编码 = jy.产品编号 
                              left join   仓库物料数量表 kc  on   base.物料编码= kc.物料编码
                              left join 采购记录采购单明细表 cmx on cmx.采购明细号=jy.采购明细号
                              where 送检数量>已入库数 and  kc.仓库号=cmx.仓库号  and  (检验结果<>'不合格'or(检验结果='不合格'and (检验记录单号 in 
                                (select 检验记录单号 from 检验上传表单记录表 a ,[采购记录采购单检验主表] b
                                   where a.采购入库通知单号=b.送检单号 and 表单类型='不合格品评审单')  or 数量标记=1))) ");



                da = new SqlDataAdapter(sql, strcon);
                dt_采购检验单 = new DataTable();
                da.Fill(dt_采购检验单);
                //   dt_采购检验单.Columns.Add("赠送数量", typeof(decimal));

            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_load采购检验单");
                throw ex;
            }
        }

        /// <summary>
        /// 筛选检验单的数据：根据供应商来删选检验单的
        /// </summary>
        private void Fun_筛选采购检验单()
        {
            try
            {
                SqlDataAdapter da;
                string sql = "";
                dt_采购检验单 = new DataTable();
                string str_sql = "";
                if (CPublic.Var.localUserName.ToLower() != "admin")
                {
                    if (dt_仓库.Rows.Count > 0)
                    {
                        str_sql = "and cmx.仓库号  in(";

                        foreach (DataRow dr in dt_仓库.Rows)
                        {
                            str_sql = str_sql + string.Format("'{0}',", dr["仓库号"]);

                        }
                        str_sql = str_sql.Substring(0, str_sql.Length - 1) + ")";

                    }
                }
                /*19-10-31
                 
                 select jy.*,已送检数,kc.货架描述,kc.库存总数,cmx.仓库号,cmx.仓库名称,未领量 from 采购记录采购单检验主表 jy
                    left join  基础数据物料信息表 base on base.物料编码 = jy.产品编号
                    left join 采购记录采购单明细表 cmx on cmx.采购明细号=jy.采购明细号
                     left join 仓库物料数量表 kc on kc.物料编码=jy.产品编号   and kc.仓库号 = cmx.仓库号
                    where 入库完成 =0  and jy.完成 = 0 and jy.关闭 = 0    and
                     (检验结果<>'不合格'or(检验结果='不合格'and (检验记录单号 in 
                         (select 检验记录单号 from 检验上传表单记录表,[采购记录采购单检验主表] 
                                 where 检验上传表单记录表.采购入库通知单号=[采购记录采购单检验主表].送检单号 and 表单类型='不合格品评审单') 
                                or 数量标记=1))) and jy.供应商编号='{0}' {1}

                select jy.*,已送检数,kc.货架描述,kc.库存总数,cmx.仓库号,cmx.仓库名称,未领量 from 采购记录采购单检验主表 jy
                    left join  基础数据物料信息表 base on base.物料编码 = jy.产品编号
                      left join 采购记录采购单明细表 cmx on cmx.采购明细号=jy.采购明细号
                        left join 仓库物料数量表 kc on kc.物料编码=jy.产品编号  and kc.仓库号 = cmx.仓库号
                    where 入库完成 =0  and jy.完成 = 0 and jy.关闭 = 0    and
                     (检验结果<>'不合格'or(检验结果='不合格'and (检验记录单号 in (select 检验记录单号 from 检验上传表单记录表,[采购记录采购单检验主表] 
                    where 检验上传表单记录表.采购入库通知单号=[采购记录采购单检验主表].送检单号 and 表单类型='不合格品评审单') or 数量标记=1))) {0}

                 */
                if (txt_gongysid.EditValue.ToString() != "")
                {
                    sql = string.Format(@"select jy.*,已送检数,kc.货架描述,kc.库存总数,cmx.仓库号,cmx.仓库名称,未领量 from 采购记录采购单检验主表 jy
                    left join  基础数据物料信息表 base on base.物料编码 = jy.产品编号
                    left join 采购记录采购单明细表 cmx on cmx.采购明细号=jy.采购明细号
                     left join 仓库物料数量表 kc on kc.物料编码=jy.产品编号   and kc.仓库号 = cmx.仓库号
                    where 入库完成 =0  and jy.完成 = 0 and jy.关闭 = 0    and
                      检验结果<>'不合格' and jy.供应商编号='{0}' {1}", txt_gongysid.EditValue.ToString(), str_sql);
                }
                else
                {
                    sql = string.Format(@"select jy.*,已送检数,kc.货架描述,kc.库存总数,cmx.仓库号,cmx.仓库名称,未领量 from 采购记录采购单检验主表 jy
                    left join  基础数据物料信息表 base on base.物料编码 = jy.产品编号
                      left join 采购记录采购单明细表 cmx on cmx.采购明细号=jy.采购明细号
                        left join 仓库物料数量表 kc on kc.物料编码=jy.产品编号  and kc.仓库号 = cmx.仓库号
                    where 入库完成 =0  and jy.完成 = 0 and jy.关闭 = 0    and  检验结果<>'不合格'  {0}", str_sql);
                }
                da = new SqlDataAdapter(sql, strcon);
                da.Fill(dt_采购检验单);
                // dt_采购检验单.Columns.Add("选择", typeof(bool));
                dt_采购检验单.Columns.Add("供应商");
                dt_采购检验单.Columns.Add("可入库数", typeof(decimal));
                dt_采购检验单.Columns.Add("赠送数量", typeof(decimal));
                foreach (DataRow r in dt_采购检验单.Rows)
                {
                    string sql_math = string.Format("select * from 其他出入库申请子表 where 备注='{0}'and 物料编码='{1}'", r["采购明细号"].ToString(), r["产品编号"].ToString());
                    DataTable dt_math = CZMaster.MasterSQL.Get_DataTable(sql_math, strcon);
                    if (dt_math.Rows.Count > 0)
                    {
                        r["赠送数量"] = dt_math.Compute("sum(数量)", "true").ToString();
                    }
                    else
                    {

                        r["赠送数量"] = 0;
                    }

                    r["可入库数"] = Convert.ToDecimal(r["送检数量"]) - Convert.ToDecimal(r["不合格数量"]) - Convert.ToDecimal(r["已入库数"]);
                    if (dt_供应商 != null)
                    {
                        DataRow[] dr3 = dt_供应商.Select(string.Format("供应商ID='{0}'", r["供应商编号"].ToString()));
                        if (dr3.Length > 0)
                        {
                            r["供应商"] = dr3[0]["供应商名称"];
                        }
                    }

                    if (dt_入库明细 != null)
                    {
                        DataRow[] dr = dt_入库明细.Select(string.Format("检验记录单号='{0}'", r["检验记录单号"].ToString()));
                        if (dr.Length > 0)
                        {

                            dr[0]["剩余入库量"] = Convert.ToDecimal(r["送检数量"]) - Convert.ToDecimal(r["不合格数量"]) - Convert.ToDecimal(r["已入库数"]);
                        }
                    }
                }

                if (dt_入库明细 != null)
                {
                    foreach (DataRow r in dt_入库明细.Rows)
                    {
                        DataRow[] dr = dt_采购检验单.Select(string.Format("检验记录单号='{0}'", r["检验记录单号"].ToString()));
                        if (dr.Length < 0)
                            r.Delete();
                    }
                }
                gcJYD.DataSource = dt_采购检验单;

            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_筛选采购检验单");
                throw ex;
            }
        }



        #region  下拉框选择项
        //下拉框的选择项
        private void Fun_下拉框选择项()
        {
            SqlDataAdapter da;
            //供应商的选择
            dt_供应商 = new DataTable();
            string sql1 = "select 供应商ID,供应商名称,供应商负责人,供应商电话 from 采购供应商表";
            da = new SqlDataAdapter(sql1, strcon);
            da.Fill(dt_供应商);
            txt_gongysid.Properties.DataSource = dt_供应商;
            txt_gongysid.Properties.DisplayMember = "供应商ID";
            txt_gongysid.Properties.ValueMember = "供应商ID";

            //入库人员选择
            dt_入库人员 = new DataTable();
            string sql3 = "select 员工号,姓名,手机,部门 from 人事基础员工表";
            da = new SqlDataAdapter(sql3, strcon);
            da.Fill(dt_入库人员);
            txt_rkygh.Properties.DataSource = dt_入库人员;
            txt_rkygh.Properties.DisplayMember = "员工号";
            txt_rkygh.Properties.ValueMember = "员工号";

            dt_仓库 = new DataTable();
            string sql4 = "select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别'";
            da = new SqlDataAdapter(sql4, strcon);
            da.Fill(dt_仓库);
            repositoryItemSearchLookUpEdit5.DataSource = dt_仓库;
            repositoryItemSearchLookUpEdit5.DisplayMember = "仓库号";
            repositoryItemSearchLookUpEdit5.ValueMember = "仓库号";

        }

        //入库人员的选择
        private void SearchLookUpEdit3_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (txt_rkygh.EditValue == null)
                    txt_rkygh.EditValue = "";

                DataRow[] dr = dt_入库人员.Select(string.Format("员工号='{0}'", txt_rkygh.EditValue.ToString()));
                if (dr.Length > 0)
                {
                    txt_rkygxm.Text = dr[0]["姓名"].ToString();
                }
                else
                {
                    txt_rkygxm.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //选择供应商号带出供应商的数据
        private void Txt_gongys_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (txt_gongysid.EditValue == null)
                    txt_gongysid.EditValue = "";
                if (dt_供应商 != null && dt_供应商.Rows.Count > 0)
                {
                    DataRow[] dr = dt_供应商.Select(string.Format("供应商ID='{0}'", txt_gongysid.EditValue.ToString()));
                    if (dr.Length > 0)
                    {
                        txt_gongys.Text = dr[0]["供应商名称"].ToString();
                        txt_gysfzr.Text = dr[0]["供应商负责人"].ToString();
                        txt_gysdianhua.Text = dr[0]["供应商电话"].ToString();
                    }
                    else
                    {
                        txt_gongys.Text = "";
                        txt_gysfzr.Text = "";
                        txt_gysdianhua.Text = "";
                    }
                }
                if (dt_入库明细 != null)
                {
                    DataRow[] dr1 = dt_入库明细.Select(string.Format("供应商ID='{0}'", txt_gongysid.EditValue.ToString()));
                    if (dr1.Length <= 0)
                    {
                        dt_入库明细.Clear();
                    }
                }
                // fun_筛选采购检验单();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        //通过入库单的列表查询
        private void Fun_查询(string danhao)
        {
            try
            {
                SqlDataAdapter da;
                //入库单主表
                dt_入库主表 = new DataTable();
                string sql = string.Format("select * from 采购记录采购单入库主表 where 入库单号='{0}'", danhao);
                da = new SqlDataAdapter(sql, strcon);
                da.Fill(dt_入库主表);
                drm = dt_入库主表.NewRow();
                if (dt_入库主表.Rows.Count > 0)
                {
                    drm = dt_入库主表.Rows[0];
                }
                dataBindHelper1.DataFormDR(drm);

                //入库单明细
                dt_入库明细 = new DataTable();
                string sql1 = string.Format(@"select rk.* ,库存总数,kc.货架描述  from 采购记录采购单入库明细 rk
                                            left  join   基础数据物料信息表 base  on rk.物料编码 = base.物料编码 
                                            left  join 采购记录采购单明细表 cmx on cmx.采购明细号=rk.采购单明细号
                                            left  join   仓库物料数量表 kc  on base.物料编码 = kc.物料编码
                                                where rk.入库单号='{0}' and  cmx.仓库号=kc.仓库号", drm["入库单号"].ToString());
                da = new SqlDataAdapter(sql1, strcon);
                da.Fill(dt_入库明细);
                dt_入库明细.Columns.Add("剩余入库量", typeof(decimal));

                Fun_筛选采购检验单();
                gcrk.DataSource = dt_入库明细;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_查询");
                throw new Exception(ex.Message);
            }
        }

        void Dt_入库明细_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {

            if (e.Column.ColumnName == "仓库号")
            {
                DataRow[] dr = dt_仓库.Select(string.Format("仓库号='{0}'", e.Row["仓库号"].ToString()));
                if (dr.Length > 0)
                    e.Row["仓库名称"] = dr[0]["仓库名称"];
            }
        }

        //检查入库单的主表
        private void Fun_check入库主表()
        {
            try
            {

                DateTime t = CPublic.Var.getDatetime();
                //根据GUID来判断新增
                //2020-5-6 发现不到20秒二厂那边 两条记录 单号一样 
                //if (drm["GUID"] == DBNull.Value)
                //{
                drm["GUID"] = System.Guid.NewGuid();
                //入库单号

                //if (txt_rkdanhao.Text == "")
                //{
                txt_rkdanhao.Text = string.Format("PC{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                    CPublic.CNo.fun_得到最大流水号("PC", t.Year, t.Month));
                //}
                drm["创建日期"] = t;
                dt_入库主表.Rows.Add(drm);
                //}
                strRKDH = txt_rkdanhao.Text;
                if (txt_rkygh.Text == "")
                    throw new Exception("入库员工号不能为空，请选择入库人员的员工号！");
                //if (txt_rkygxm.Text == "")
                //    throw new Exception("入库人员的姓名不能为空，请填写入库员工的姓名！");

                if (txt_rkriqi.Text == "")
                    throw new Exception("录入日期不能为空！");
                drm["修改日期"] = t;
                drm["操作员ID"] = CPublic.Var.LocalUserID;
                drm["操作员"] = CPublic.Var.localUserName;



            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_check入库主表");
                throw new Exception(ex.Message);
            }
        }

        //检查入库单的明细表
        private void Fun_check入库明细表()
        {
            try
            {


                //4-11 入库前先判断此检验单是否关闭
                string s = string.Format("select  * from 采购记录采购单检验主表 where 关闭=0 and 检验记录单号='{0}'", dt_入库明细.Rows[0]["检验记录单号"]);
                using (SqlDataAdapter d = new SqlDataAdapter(s, strcon))
                {
                    DataTable t = new DataTable();
                    d.Fill(t);
                    if (t.Rows.Count == 0)
                    {
                        throw new Exception("该条检验单状态已修改，刷新后重试");
                    }

                }


                int pos = 0;
                //18-3-28 仍然发现会有两个物料同时入库
                if (dt_入库明细.Rows.Count > 1)
                {
                    throw new Exception("出了点问题请刷新后重试");
                }

                foreach (DataRow r in dt_入库明细.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    string x = string.Format(@"   select  采购单类型 from 采购记录采购单主表 a 
        left join  采购记录采购单明细表 b on a.采购单号=b.采购单号    where  采购明细号 ='{0}'", r["采购单明细号"]);
                    DataTable t_temp = CZMaster.MasterSQL.Get_DataTable(x, strcon);//

                    if (r["GUID"] == DBNull.Value)
                    {
                        r["GUID"] = System.Guid.NewGuid();
                    }
                    r["入库单号"] = strRKDH; //入库单号
                    r["入库POS"] = ++pos;
                    r["入库明细号"] = strRKDH + "-" + pos.ToString("00");
                    r["入库人员ID"] = txt_rkygh.EditValue.ToString();
                    r["入库人员"] = txt_rkygxm.Text;



                    if (txt_rkriqi.Text != "")
                    {
                        r["录入日期"] = txt_rkriqi.Text;
                    }
                    //检查入库明细的供应商是不是一致
                    //if (checkgys != r["供应商ID"].ToString())
                    //    throw new Exception("入库明细存在供应商不一致的情况，请重新选择入库明细！");
                    //仓库
                    //if (r["仓库ID"].ToString() == "")
                    //    r["仓库ID"] = txt_cangkuid.EditValue.ToString();
                    //throw new Exception("请选择需要入库的仓库号！");
                    //if (r["仓库ID"].ToString() != txt_cangkuid.EditValue.ToString())
                    //    throw new Exception("入库单仓库与入库明细的仓库必须要一致，请检查");
                    //if (r["仓库名称"].ToString() == "")
                    //    r["仓库名称"] = txt_cangku.Text;
                    // throw new Exception("仓库名称不能为空！");
                    //if (r["仓库名称"].ToString() != txt_cangkuid.EditValue.ToString())
                    //    throw new Exception("入库单仓库与入库明细的仓库必须要一致，请检查");

                    r["操作员ID"] = CPublic.Var.LocalUserID;
                    r["操作员"] = CPublic.Var.localUserName;

                    if (r["入库量"].ToString() == "")
                        throw new Exception("入库量不能为空，请填写入库量！");
                    try
                    {
                        decimal dd = Convert.ToDecimal(r["入库量"]);



                    }
                    catch
                    {
                        throw new Exception("入库量是数字，请检查！");
                    }
                    //if (Convert.ToDecimal(r["入库量"]) > Convert.ToDecimal(r["剩余入库量"]))
                    //    throw new Exception("入库量不能大于剩余入库量！");
                    if (Convert.ToDecimal(r["入库量"]) <= 0)
                    {
                        throw new Exception("入库量不能小于0，请填写入库量！");

                    }
                    decimal dec = (Convert.ToDecimal(r["入库量"]) - Convert.ToDecimal(r["剩余入库量"])) / Convert.ToDecimal(r["剩余入库量"]);
                    if ((Convert.ToDecimal(r["入库量"]) - Convert.ToDecimal(r["剩余入库量"])) / Convert.ToDecimal(r["剩余入库量"]) > Convert.ToDecimal(0.05))
                    {

                        throw new Exception("入库量大于采购量5%,按规定不可入库，联系采购人员");

                    }

                    if (dec < 0 && -dec > Convert.ToDecimal(0.2))
                    {
                        if (MessageBox.Show("入库量小于采购量80%,请再次确认是否继续？", "提醒", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                        {
                            throw new Exception("已取消，可修改");
                        }
                    }
                    if (r["仓库号"].ToString() == "")
                    {
                        throw new Exception("仓库号必填");
                    }
                    DataRow[] ds = dt_仓库.Select(string.Format("仓库号 = '{0}'", r["仓库号"].ToString()));
                    if (ds.Length == 0)
                    {
                        throw new Exception("仓库号不对");
                    }
                    //18-4-20 为了 从采购 到送检 到检验 到入库 单价 税率 金额 延续 
                    r["未税金额"] = Convert.ToDecimal(r["入库量"]) * Convert.ToDecimal(r["未税单价"]);
                    r["金额"] = Convert.ToDecimal(r["入库量"]) * Convert.ToDecimal(r["单价"]);


                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_check入库明细表");
                throw new Exception(ex.Message);
            }
        }

        //保存数据的方法 弃用
        private void Fun_save数据()
        {
            try
            {
                dataBindHelper1.DataToDR(drm);
                MasterSQL.Save_DataTable(dt_入库主表, "采购记录采购单入库主表", strcon);
                MasterSQL.Save_DataTable(dt_入库明细, "采购记录采购单入库明细", strcon);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_save数据");
                throw new Exception(ex.Message);
            }
        }

        //主数据的新增功能
        private void Fun_新增主表()
        {
            try
            {

                strRKDH = "";
                SqlDataAdapter da;
                dt_入库主表 = new DataTable();
                da = new SqlDataAdapter("select * from 采购记录采购单入库主表 where 1<>1", strcon);
                da.Fill(dt_入库主表);
                drm = dt_入库主表.NewRow();
                dataBindHelper1.DataFormDR(drm);
                //txt_cangkuid.EditValue = "01";
                txt_gongysid.EditValue = "";
                txt_rkygh.EditValue = CPublic.Var.LocalUserID;
                txt_rkygxm.Text = CPublic.Var.localUserName;
                txt_rkriqi.EditValue = CPublic.Var.getDatetime();
                if (dt_入库明细 != null)
                {
                    dt_入库明细.Clear();
                    gcrk.DataSource = dt_入库明细;
                }
                Fun_筛选采购检验单();
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_新增主表");
                throw new Exception(ex.Message);
            }
        }

        #region  界面的操作
        //刷新操作
        private void BarLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                Frm采购入库单明细_Load(null, null);

                textBox3.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //新增
        private void BarLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                Fun_新增主表();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //保存操作
        private void BarLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确认保存吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    DataView dv_mx = new DataView(dt_入库明细)
                    {
                        RowStateFilter = DataViewRowState.CurrentRows
                    };
                    if (dv_mx.Count <= 0)
                        throw new Exception("没有入库明细，不能进行保存操作，请选择需要入库的明细！");
                    Fun_check入库主表();//检查主表的数据
                    gvrk.CloseEditor();
                    this.BindingContext[dt_入库明细].EndCurrentEdit();
                    Fun_check入库明细表(); //检查入库的明细
                    Fun_save数据();
                    Fun_查询(drm["入库单号"].ToString());   //保存之后重新加载一遍
                    MessageBox.Show("保存成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region  生效操作
        /// <summary>
        /// 18-3-1 新物料属性
        /// </summary>
        /// <returns></returns>
        private DataTable Fun_xxx()
        {
            DataTable dt = new DataTable();

            foreach (DataRow dr in dt_入库明细.Rows)
            {
                if (dt != null && dt.Rows.Count > 0)  //可能会有一样的物料 先判断
                {
                    if (dt.Select(string.Format("物料编码='{0}'", dr["物料编码"].ToString())).Length > 0) continue;

                }
                string sql = string.Format("select * from 基础数据物料信息表  where  物料编码='{0}'", dr["物料编码"].ToString());
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    da.Fill(dt);
                    dt.Rows[dt.Rows.Count - 1]["新数据"] = 0;
                }
            }
            return dt;
        }
        //生效的方法
        private void Fun_入库生效(DataTable dt_type)
        {
            try
            {   //入库单的主表
                DateTime t = CPublic.Var.getDatetime();
                string str_id = CPublic.Var.LocalUserID;
                string str_name = CPublic.Var.localUserName;
                drm["生效"] = 1;  //生效主单
                drm["生效人员ID"] = str_id;
                drm["生效人员"] = str_name; //生效人员
                drm["生效日期"] = t;  //生效日期
                drm["修改日期"] = t;
                //入库单的明细表
                dataBindHelper1.DataToDR(drm);
                foreach (DataRow r in dt_入库明细.Rows)
                {
                    r["生效"] = 1;
                    r["生效人员ID"] = str_id;
                    r["生效人员"] = str_name;
                    r["生效日期"] = t;
                    DataRow[] dr = dt_采购检验单.Select(string.Format("检验记录单号='{0}'", r["检验记录单号"].ToString()));
                    if (dr.Length > 0)
                    {
                        dr[0]["已入库数"] = Convert.ToDecimal(dr[0]["已入库数"]) + Convert.ToDecimal(r["入库量"]);
                        if (Convert.ToDecimal(dr[0]["已入库数"]) >= Convert.ToDecimal(dr[0]["送检数量"]) - Convert.ToDecimal(dr[0]["不合格数量"]))
                        { dr[0]["入库完成"] = 1; }
                    }

                }
                //给出入库明细表添加数据
                SqlDataAdapter da1;
                string sql = "select * from 仓库出入库明细表 where 1<>1";
                da1 = new SqlDataAdapter(sql, strcon);
                DataTable dt_出入库明细 = new DataTable();
                da1.Fill(dt_出入库明细);

                foreach (DataRow r in dt_入库明细.Rows)
                {
                    DataRow dr_出入库 = dt_出入库明细.NewRow();
                    dt_出入库明细.Rows.Add(dr_出入库);
                    dr_出入库["GUID"] = System.Guid.NewGuid().ToString();
                    dr_出入库["明细类型"] = "采购入库";
                    dr_出入库["单号"] = r["入库单号"];
                    dr_出入库["物料编码"] = r["物料编码"];
                    dr_出入库["物料名称"] = r["物料名称"];
                    dr_出入库["BOM版本"] = r["BOM版本号"];
                    dr_出入库["明细号"] = r["入库明细号"];
                    dr_出入库["出库入库"] = "入库";
                    dr_出入库["相关单位"] = txt_gongys.Text;
                    dr_出入库["仓库号"] = r["仓库号"];
                    dr_出入库["仓库名称"] = r["仓库名称"];
                    r["仓库ID"] = r["仓库号"]; // 把界面选择的仓库号 赋值回 采购入库明细表 中

                    dr_出入库["相关单号"] = r["采购单明细号"];

                    dr_出入库["仓库人"] = CPublic.Var.localUserName;

                    dr_出入库["数量"] = 0;
                    dr_出入库["单位"] = r["数量单位"];
                    dr_出入库["标准数量"] = 0;
                    dr_出入库["实效数量"] = Convert.ToDecimal(r["入库量"]);
                    dr_出入库["实效时间"] = t;
                    dr_出入库["出入库时间"] = t;

                }
                DataSet ds = new DataSet();
                ds = StockCore.StockCorer.fun_出入库_采购入库(dt_入库明细.Rows[0]["物料编码"].ToString(), Convert.ToDecimal(dt_入库明细.Rows[0]["入库量"]), Convert.ToDecimal(dt_入库明细.Rows[0]["送检数量"]), dt_入库明细.Rows[0]["采购单明细号"].ToString());
                DataTable dt_cs = dt_入库明细.Copy();
                dt_cs.Columns["入库量"].ColumnName = "数量";
                DataTable dt_库存 = ERPorg.Corg.fun_库存(1, dt_cs);

                DataTable dt_y = Fun_xxx();

                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("入库生效"); //事务的名称
                SqlCommand cmd = new SqlCommand("select * from 采购记录采购单入库主表 where 1<>1", conn, ts);
                SqlCommand cmd1 = new SqlCommand("select * from 采购记录采购单入库明细 where 1<>1", conn, ts);
                SqlCommand cmd2 = new SqlCommand("select * from 采购记录采购单检验主表 where 1<>1", conn, ts);
                SqlCommand cmd3 = new SqlCommand("select * from 仓库出入库明细表 where 1<>1", conn, ts);
                SqlCommand cmd4 = new SqlCommand("select * from 采购记录采购单主表  where 1<>1", conn, ts);
                SqlCommand cmd5 = new SqlCommand("select * from 采购记录采购单明细表 where 1<>1", conn, ts);
                SqlCommand cmd6 = new SqlCommand("select * from 仓库物料数量表 where 1<>1", conn, ts);
                SqlCommand cmd7 = new SqlCommand("select * from 基础数据物料信息表 where 1<>1", conn, ts);
                //   SqlCommand cmd8 = new SqlCommand( "select * from 入库流水记录表 where 1<>1",conn,ts);
                try
                {
                    SqlDataAdapter da;
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_入库主表);

                    da = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da);
                    da.Update(dt_入库明细);

                    da = new SqlDataAdapter(cmd2);
                    new SqlCommandBuilder(da);
                    da.Update(dt_采购检验单);

                    da = new SqlDataAdapter(cmd3);
                    new SqlCommandBuilder(da);
                    da.Update(dt_出入库明细);

                    da = new SqlDataAdapter(cmd4);
                    new SqlCommandBuilder(da);
                    da.Update(ds.Tables[0]);

                    da = new SqlDataAdapter(cmd5);
                    new SqlCommandBuilder(da);
                    da.Update(ds.Tables[1]);

                    da = new SqlDataAdapter(cmd6);
                    new SqlCommandBuilder(da);
                    da.Update(dt_库存);

                    da = new SqlDataAdapter(cmd7);
                    new SqlCommandBuilder(da);
                    da.Update(dt_y);

                    //da = new SqlDataAdapter(cmd8);
                    //new SqlCommandBuilder(da);
                    //da.Update(dt_type);

                    //foreach (DataRow r in dt_入库明细.Rows)
                    //{
                    //    // 2016/11/11

                    //    //StockCore.StockCorer.fun_出入库_采购入库(r["物料编码"].ToString(), Convert.ToDecimal(r["入库量"]), Convert.ToDecimal(r["送检数量"]), r["采购单明细号"].ToString());
                    //    StockCore.StockCorer.fun_刷新库存(r["物料编码"].ToString(), Convert.ToDecimal(r["入库量"]), 1);

                    //}
                    ts.Commit();

                }
                catch (Exception)
                {
                    ts.Rollback();
                    throw new Exception("");
                }



                ////给出入库明细表添加数据
                //SqlDataAdapter da1;
                //string sql = "select * from 仓库出入库明细表 where 1<>1";
                //da1 = new SqlDataAdapter(sql, strcon);
                //DataTable dt_出入库明细 = new DataTable();
                //da1.Fill(dt_出入库明细);

                //foreach (DataRow r in dt_入库明细.Rows)
                //{
                //    DataRow dr_出入库 = dt_出入库明细.NewRow();
                //    dt_出入库明细.Rows.Add(dr_出入库);
                //    dr_出入库["GUID"] = System.Guid.NewGuid().ToString();
                //    dr_出入库["明细类型"] = "采购入库";
                //    dr_出入库["单号"] = r["入库单号"];
                //    dr_出入库["物料编码"] = r["物料编码"];
                //    dr_出入库["物料名称"] = r["物料名称"];
                //    dr_出入库["BOM版本"] = r["BOM版本号"];
                //    dr_出入库["明细号"] = r["入库明细号"];
                //    dr_出入库["出库入库"] = "入库";
                //    dr_出入库["相关单位"] = txt_gongys.Text;

                //    dr_出入库["相关单号"] = r["采购单明细号"];

                //    dr_出入库["数量"] = 0;
                //    dr_出入库["单位"] = r["数量单位"];
                //    dr_出入库["标准数量"] = 0;
                //    dr_出入库["实效数量"] = Convert.ToDecimal(r["入库量"]);
                //    dr_出入库["实效时间"] = System.DateTime.Now;
                //    dr_出入库["出入库时间"] = System.DateTime.Now;

                //    string sql_pd = "select * from 仓库物料盘点表 where 有效=1";
                //    using (SqlDataAdapter da0 = new SqlDataAdapter(sql_pd, strcon))
                //    {
                //        DataTable dt_批次号 = new DataTable();
                //        da0.Fill(dt_批次号);
                //        if (dt_批次号.Rows.Count > 0)
                //        {
                //            dr_出入库["盘点有效批次号"] = dt_批次号.Rows[0]["盘点批次号"];
                //        }
                //        else
                //        {
                //            dr_出入库["盘点有效批次号"] = "初始化";
                //        }
                //    }
                //}
                //da1 = new SqlDataAdapter(sql, strcon);
                //new SqlCommandBuilder(da1);
                //da1.Update(dt_出入库明细);

                Fun_出库批次();

                foreach (DataRow r in dt_入库明细.Rows)
                {
                    StockCore.StockCorer.fun_物料数量_实际数量(r["物料编码"].ToString(), r["仓库号"].ToString(), true);
                }
                // fun_新增主表();
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_采购单生效");
                Fun_查询(drm["入库单号"].ToString());
                throw new Exception("生效失败" + ex.Message);
            }
        }

        private void Fun_出库批次()
        {
            try
            {
                SqlDataAdapter da1;
                string sql = "select * from 领料出库批次记录表 where 1<>1";
                da1 = new SqlDataAdapter(sql, strcon);
                DataTable dt_出库批次 = new DataTable();
                da1.Fill(dt_出库批次);

                foreach (DataRow r in dt_入库明细.Rows)
                {
                    DataRow dr_出入库 = dt_出库批次.NewRow();
                    dt_出库批次.Rows.Add(dr_出入库);
                    dr_出入库["GUID"] = System.Guid.NewGuid().ToString();
                    dr_出入库["入库单号"] = r["入库单号"];
                    dr_出入库["物料编码"] = r["物料编码"];
                    dr_出入库["物料名称"] = r["物料名称"];
                    dr_出入库["入库单明细号"] = r["入库明细号"];
                    dr_出入库["数量"] = Convert.ToDecimal(r["入库量"]);
                    dr_出入库["计算数量"] = Convert.ToDecimal(r["入库量"]);
                    dr_出入库["日期"] = CPublic.Var.getDatetime();
                }
                da1 = new SqlDataAdapter(sql, strcon);
                new SqlCommandBuilder(da1);
                da1.Update(dt_出库批次);
            }
            catch (Exception)
            {

                throw;
            }

        }



        //入库单的生效操作
        private void BarLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show("请确认入库数量", "提醒", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    int index = gvJYD.FocusedRowHandle;
                    //DataView dv_mx = new DataView(dt_入库明细);
                    //dv_mx.RowStateFilter = DataViewRowState.CurrentRows;
                    if (dt_入库明细.Rows.Count <= 0)
                    {
                        throw new Exception("没有入库明细，不能进行生效操作，请选择需要入库的明细！");
                    }
                    else if (dt_入库明细.Rows.Count > 1)
                    {
                        throw new Exception("遇到问题了,请刷新重试");
                    }
                    Fun_check入库主表();
                    gvrk.CloseEditor();
                    this.BindingContext[dt_入库明细].EndCurrentEdit();
                    Fun_check入库明细表();

                    DataTable dt_采购入库 = new DataTable();
                    dt_采购入库 = StockCore.StockCorer.fun_RUKU("采购入库", dt_入库明细);

                    Fun_入库生效(dt_采购入库);
                    Fun_采购单入库状态();
                    MessageBox.Show("生效成功！");
                    Fun_新增主表();

                    if (index != 0 && index <= gvJYD.DataRowCount)
                    {
                        gvJYD.FocusedRowHandle = index;

                    }
                    else if (index > gvJYD.DataRowCount)
                    {
                        gvJYD.FocusedRowHandle = gvJYD.DataRowCount;
                    }
                    //ERPpurchase.frm采购入库单明细 frm = new frm采购入库单明细();
                    //CPublic.UIcontrol.Showpage(frm, "来料入库");
                    textBox3.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        private void RepositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
        {
            gvJYD.CloseEditor();
            this.BindingContext[dt_采购检验单].EndCurrentEdit();
        }

        #region







        #endregion

        //关闭界面
        private void BarLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }



        #region 待办点击事件

        private void GvJYD_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow r_点击行 = gvJYD.GetDataRow(gvJYD.FocusedRowHandle);
                if (r_点击行 == null) return;
                dt_入库明细 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(@"select a.*,kc.货架描述,库存总数,cmx.仓库号,cmx.仓库名称 from 采购记录采购单入库明细 a
                                              left join 基础数据物料信息表 base on base.物料编码=a.物料编码
                                              left  join  仓库物料数量表 kc on kc.物料编码=a.物料编码
                                              left  join 采购记录采购单明细表 cmx  on cmx.采购明细号=a.采购单明细号  where 1<>1", strcon);
                da.Fill(dt_入库明细);
                dt_入库明细.Columns.Add("填写入库量", typeof(decimal));
                dt_入库明细.Columns.Add("剩余入库量", typeof(decimal));
                // dt_入库明细.Columns.Add("库存总数");
                try
                {
                    //DataTable dt_物料;
                    //string sql = "";
                    DataRow r1 = dt_入库明细.NewRow();
                    r1["检验记录单号"] = r_点击行["检验记录单号"];
                    r1["送检单号"] = r_点击行["送检单号"];
                    r1["送检单明细号"] = r_点击行["送检单明细号"];
                    r1["采购单号"] = r_点击行["采购单号"];
                    r1["采购单明细号"] = r_点击行["采购明细号"];
                    r1["物料编码"] = r_点击行["产品编号"];
                    //r1["原ERP物料编号"] = r_点击行["原ERP物料编号"];
                    //r1["n原ERP规格型号"] = r_点击行["n原ERP规格型号"];
                    r1["库存总数"] = r_点击行["库存总数"];
                    r1["仓库号"] = r_点击行["仓库号"];
                    r1["仓库ID"] = r_点击行["仓库号"];

                    r1["仓库名称"] = r_点击行["仓库名称"];
                    r1["货架描述"] = r_点击行["货架描述"];
                    r1["规格型号"] = r_点击行["规格型号"];
                    r1["图纸编号"] = r_点击行["图纸编号"];
                    r1["物料名称"] = r_点击行["产品名称"];

                    //sql = string.Format(@"select  库存总数   from 仓库物料数量表  where  仓库号 = '{1}' and   物料编码='{0}'",r_点击行["产品编号"].ToString(),r_点击行["仓库ID"]);
                    //dt_物料 = MasterSQL.Get_DataTable(sql, strcon);
                    //if (dt_物料.Rows.Count > 0)
                    //{

                    //    r1["库存总数"] = dt_物料.Rows[0]["库存总数"];

                    //}
                    r1["采购数量"] = r_点击行["采购数量"];
                    r1["未税单价"] = r_点击行["未税单价"];
                    r1["单价"] = r_点击行["单价"];
                    r1["未税金额"] = r_点击行["未税金额"];
                    r1["金额"] = r_点击行["金额"];
                    r1["税率"] = r_点击行["税率"];

                    r1["送检数量"] = r_点击行["送检数量"];

                    //供应商的信息
                    r1["供应商ID"] = r_点击行["供应商编号"];
                    DataRow[] dr_cggys = dt_供应商.Select(string.Format("供应商ID='{0}'", r_点击行["供应商编号"].ToString()));
                    if (dr_cggys.Length > 0)
                    {
                        r1["供应商"] = dr_cggys[0]["供应商名称"];
                        r1["供应商负责人"] = dr_cggys[0]["供应商负责人"];
                        r1["供应商电话"] = dr_cggys[0]["供应商电话"];
                    }
                    r1["检验人ID"] = "";
                    r1["检验人"] = r_点击行["检验员"];

                    r1["价格核实"] = r_点击行["价格核实"];
                    r1["是否急单"] = r_点击行["是否急单"];

                    //if (r_点击行["数量标记"].Equals(true))             //全检         
                    //{
                    r1["入库量"] = Convert.ToDecimal(r_点击行["送检数量"]) - Convert.ToDecimal(r_点击行["不合格数量"]) - Convert.ToDecimal(r_点击行["已入库数"]);
                    r1["剩余入库量"] = Convert.ToDecimal(r_点击行["送检数量"]) - Convert.ToDecimal(r_点击行["不合格数量"]) - Convert.ToDecimal(r_点击行["已入库数"]);
                    //}
                    //else       //抽检   全入
                    //{
                    //    r1["入库量"] = Convert.ToDecimal(r_点击行["送检数量"]) - Convert.ToDecimal(r_点击行["已入库数"]);
                    //    r1["剩余入库量"] = Convert.ToDecimal(r_点击行["送检数量"]) - Convert.ToDecimal(r_点击行["已入库数"]);
                    //}


                    dt_入库明细.Rows.Add(r1);
                    txt_gongysid.EditValue = r_点击行["供应商编号"].ToString();
                    textBox2.Text = r_点击行["不合格数量"].ToString();
                    gcrk.DataSource = dt_入库明细;

                }


                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //6-10 去除 不让仓库关闭 让 采购人员 自己关闭送检单
            ////判断右键菜单是否可用
            //if (e != null && e.Button == MouseButtons.Right)
            //{
            //    contextMenuStrip1.Show(gcJYD, new Point(e.X, e.Y));
            //}
        }
        private void Fun_完成关闭(DataRow dr, string s)
        {

            ERPpurchase.frm来料入库关闭完成原因 frm = new frm来料入库关闭完成原因(dr)
            {
                Text = "记录原因"
            };

            frm.ShowDialog();
            if (frm.flag)
            {


                string sql = string.Format("select * from 采购记录采购单检验主表 where 检验记录单号 = '{0}'", dr["检验记录单号"]);
                SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                DataTable dt = new DataTable();
                DataTable t_采购明细 = new DataTable();
                DataTable t_采购主 = new DataTable();
                DateTime t = CPublic.Var.getDatetime();
                da.Fill(dt);

                if (s == "完成")
                {
                    dt.Rows[0]["完成"] = 1;
                    dt.Rows[0]["入库完成"] = 1;
                }
                else
                {
                    dt.Rows[0]["关闭"] = 1;           //区分是否是右键关闭
                }

                //new SqlCommandBuilder(da);
                //da.Update(dt);
                //关闭 赋上明细完成日期 代表 该明细已入库 ，明细完成在送检时已赋值  //分批入库的 如果关闭其中一部分的检验单  整个采购单 也会被关闭
                //需要判断  因为 有可能是 分批送检的  先检查 已送检数 是否等于 采购数  否,则未送检完， 是，则 要判断 所有的 送检单 是否完成 
                if (Convert.ToDecimal(dr["已送检数"]) == Convert.ToDecimal(dr["采购数量"]))
                {



                    string sql_1 = string.Format(@"select 采购记录采购送检单明细表.*,检验记录单号,入库完成 from 采购记录采购送检单明细表 
                                left join 采购记录采购单检验主表  on 采购记录采购单检验主表.送检单号=采购记录采购送检单明细表.送检单号
                                where  采购记录采购送检单明细表.采购单明细号='{0}' and 检验记录单号<>'{1}' and 入库完成=0  and 关闭=0", dr["采购明细号"], dr["检验记录单号"].ToString());
                    DataTable dt_1 = new DataTable();
                    using (SqlDataAdapter da_1 = new SqlDataAdapter(sql_1, strcon))  //采购单全部送检的前提下 判断除当前 送检单 是否其他 送检单 都已完成
                    {
                        da_1.Fill(dt_1);
                    }
                    if (dt_1.Rows.Count == 0) //  是 都已经处理 采购单 应该 明细完成
                    {
                        // string sql_采购明细 = string.Format("update 采购记录采购单明细表 set   明细完成=1,明细完成日期='{0}' where 采购明细号='{1}'", CPublic.Var.getDatetime(), dr["采购明细号"]);
                        string sql_采购明细 = string.Format("select * from 采购记录采购单明细表  where 采购单号='{0}'", dr["采购单号"]);
                        t_采购明细 = CZMaster.MasterSQL.Get_DataTable(sql_采购明细, strcon);
                        DataRow[] xr = t_采购明细.Select(string.Format("采购明细号='{0}'", dr["采购明细号"]));
                        xr[0]["明细完成"] = true;
                        xr[0]["明细完成日期"] = t;
                        //  CZMaster.MasterSQL.ExecuteSQL(sql_采购明细, strcon);o

                        DataRow[] rr = t_采购明细.Select(string.Format("明细完成日期 is null"));
                        if (rr.Length == 0)
                        {
                            foreach (DataRow r in t_采购明细.Rows)
                            {
                                r["总完成"] = true;
                                r["总完成日期"] = t;
                            }


                            string sql_采购主 = string.Format("select * from 采购记录采购单主表  where 采购单号='{0}'", dr["采购单号"]);
                            t_采购主 = CZMaster.MasterSQL.Get_DataTable(sql_采购主, strcon);
                            t_采购主.Rows[0]["完成"] = true;
                            t_采购主.Rows[0]["完成日期"] = t;
                            t_采购主.Rows[0]["已入库"] = true;

                        }
                        //string sql_cplt = string.Format("select * from 采购记录采购单明细表 where  明细完成日期 is null and  采购单号='{0}'", dr["采购单号"]);

                        //DataTable dt_cplt = new DataTable();
                        //dt_cplt = CZMaster.MasterSQL.Get_DataTable(sql_cplt, strcon);
                        //if (dt_cplt.Rows.Count == 0)                  //全部明细完成
                        //{
                        //明细总完成
                        //string sql_采购明细2 = string.Format("update 采购记录采购单明细表 set  总完成=1,总完成日期='{0}' where 采购单号='{1}'", CPublic.Var.getDatetime(), dr["采购单号"]);
                        //CZMaster.MasterSQL.ExecuteSQL(sql_采购明细2, strcon);
                        //主表记录完成
                        //string sql_主表完成 = string.Format("update 采购记录采购单主表  set 完成=1,完成日期='{0}',已入库=1 where  采购单号='{1}'", CPublic.Var.getDatetime(), dr["采购单号"]);
                        //CZMaster.MasterSQL.ExecuteSQL(sql_主表完成, strcon);

                    }


                }
                //记录原因 
                string sql_reason = "select * from 采购入库完成关闭原因表 where 1<>1 ";
                DataTable t_rn = CZMaster.MasterSQL.Get_DataTable(sql_reason, strcon);
                DataRow r_rn = t_rn.NewRow();
                r_rn["采购明细号"] = dr["采购明细号"];
                r_rn["检验记录单号"] = dr["检验记录单号"];
                r_rn["原因"] = frm.str;
                r_rn["物料编码"] = dr["产品编号"];
                r_rn["物料名称"] = dr["产品名称"];
                r_rn["供应商ID"] = dr["供应商编号"];
                r_rn["操作人"] = CPublic.Var.localUserName;
                r_rn["操作时间"] = t;

                t_rn.Rows.Add(r_rn);





                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction st = conn.BeginTransaction("关闭入库通知"); //事务的名称
                SqlCommand cmd = new SqlCommand("select * from 采购记录采购单检验主表  where 1<>1", conn, st);
                SqlCommand cmd1 = new SqlCommand("select * from 采购记录采购单明细表 where 1<>1", conn, st);
                SqlCommand cmd2 = new SqlCommand("select * from 采购记录采购单主表 where 1<>1", conn, st);
                SqlCommand cmd3 = new SqlCommand(sql_reason, conn, st);


                try
                {
                    SqlDataAdapter da_1;
                    da_1 = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da_1);
                    da_1.Update(dt);

                    if (t_采购明细 != null)
                    {
                        da_1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da_1);
                        da_1.Update(t_采购明细);
                    }
                    if (t_采购主 != null)
                    {
                        da_1 = new SqlDataAdapter(cmd2);
                        new SqlCommandBuilder(da_1);
                        da_1.Update(t_采购主);
                    }

                    da_1 = new SqlDataAdapter(cmd3);
                    new SqlCommandBuilder(da_1);
                    da_1.Update(t_rn);
                    st.Commit();
                }
                catch
                {
                    st.Rollback();
                }



            }
        }
        private void 关闭ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gvJYD.GetDataRow(gvJYD.FocusedRowHandle);

                if (Convert.ToDecimal(dr["已入库数"]) > 0)
                {
                    Fun_完成关闭(dr, "完成");


                }
                else
                {
                    Fun_完成关闭(dr, "关闭");

                }

                //if (MessageBox.Show(string.Format("是否要关闭检验单{0}", dr["检验记录单号"]), "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                //{

                //CZMaster.MasterSQL.ExecuteSQL(sql_1, strcon);
                //}
                StockCore.StockCorer.fun_物料数量_实际数量(dr["产品编号"].ToString(), dr["仓库号"].ToString(), true);
                Frm采购入库单明细_Load(null, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void 完成ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gvJYD.GetDataRow(gvJYD.FocusedRowHandle);


                Fun_完成关闭(dr, "完成");

                StockCore.StockCorer.fun_物料数量_实际数量(dr["产品编号"].ToString(), dr["仓库号"].ToString(), true);
                Frm采购入库单明细_Load(null, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
        private void Fun_采购单入库状态()
        {
            try
            {
                DataRow dr = gvJYD.GetDataRow(gvJYD.FocusedRowHandle);
                string sql = string.Format("select * from 采购记录采购单检验主表 where  采购单号='{0}'  and 关闭=0", dr["采购单号"]);
                string sql_s = string.Format("select * from 采购记录采购单明细表 where 采购单号 ='{0}' and 作废=0", dr["采购单号"]);
                DataTable dt_s = CZMaster.MasterSQL.Get_DataTable(sql_s, CPublic.Var.strConn);
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
                int i = 0;
                foreach (DataRow r in dt.Rows)
                {
                    if (r["入库完成"].Equals(true))
                    {
                        i++;
                        if (i == dt_s.Rows.Count)
                        {
                            string sql_1 = string.Format("select * from 采购记录采购单主表 where 采购单号='{0}'", r["采购单号"]);
                            using (SqlDataAdapter da = new SqlDataAdapter(sql_1, CPublic.Var.strConn))
                            {
                                DataTable dt_1 = new DataTable();
                                da.Fill(dt_1);
                                new SqlCommandBuilder(da);
                                dt_1.Rows[0]["已入库"] = 1;
                                da.Update(dt_1);
                            }

                        }
                        continue;
                    }
                    else
                    {
                        break;
                    }

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void GvJYD_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            //行号设置 
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString("00");
            }
        }

        private void Gvrk_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            //行号设置 
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString("00");
            }
        }

        private void Gvrk_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gvrk.GetFocusedRowCellValue(gvrk.FocusedColumn));
                e.Handled = true;
            }
        }

        private void GvJYD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gvJYD.GetFocusedRowCellValue(gvJYD.FocusedColumn));
                e.Handled = true;
            }
        }

        private void TextBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text.Length >= 13)
            {

                gvJYD.FocusedRowHandle = gvJYD.LocateByDisplayText(0, gridColumn11, textBox3.Text);
                GvJYD_RowCellClick(null, null);
                textBox3.Text = "";
                textBox3.Focus();
            }
        }

        //private void Timer1_Tick(object sender, EventArgs e)
        //{
        //    textBox3.Focus();
        //    textBox3.Text = "";
        //    timer1.Interval = 10000;
        //}

        private void GvJYD_ColumnPositionChanged(object sender, EventArgs e)
        {
            try
            {
                if (cfgfilepath != "")
                {
                    gvJYD.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void 供应商赠送ToolStripMenuItem_Click(object sender, EventArgs e)
        {


            DataRow dr = gvJYD.GetDataRow(gvJYD.FocusedRowHandle);
            Form赠送 fm = new Form赠送(dr);
            fm.ShowDialog();
            if (fm.flag == true)
            {
                //数量= fm.xiala
                DateTime t = CPublic.Var.getDatetime();
                decimal math = Convert.ToDecimal(fm.xiala);
                string str_出入库申请单号 = string.Format("QWSQ{0}{1:00}{2:0000}",
                                            t.Year, t.Month, CPublic.CNo.fun_得到最大流水号("QWSQ", t.Year, t.Month));
                //string sql_库主="select * from 其他入库主表 where 1<>1";
                //DataTable  dt_入库主=CZMaster.MasterSQL.Get_DataTable(sql_库主,strcon);
                //string sql_库son="select * from 其他入库子表 where 1<>1";
                //DataTable dt_入库子=CZMaster.MasterSQL.Get_DataTable(sql_库son,strcon);


                string sql = "select * from 其他出入库申请主表 where 1<>1";
                DataTable dt_申请主 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                string sql_son = "select * from 其他出入库申请子表 where 1<>1";
                DataTable dt_申请子 = CZMaster.MasterSQL.Get_DataTable(sql_son, strcon);
                DataRow dr_主 = dt_申请主.NewRow();
                dt_申请主.Rows.Add(dr_主);
                dr_主["GUID"] = Guid.NewGuid();
                dr_主["出入库申请单号"] = str_出入库申请单号;
                dr_主["申请类型"] = "其他入库";
                dr_主["申请日期"] = t;
                dr_主["备注"] = dr["采购明细号"].ToString();
                dr_主["相关单位"] = dr["供应商"];
                dr_主["操作人员编号"] = CPublic.Var.LocalUserID;
                dr_主["操作人员"] = CPublic.Var.localUserName;
                dr_主["生效"] = true;
                dr_主["原因分类"] = "供应商赠送";
                dr_主["生效"] = true;
                int pos = 0;
                foreach (DataRow drr in dt_入库明细.Rows)
                {
                    DataRow dr_子 = dt_申请子.NewRow();
                    dt_申请子.Rows.Add(dr_子);
                    dr_子["GUID"] = Guid.NewGuid();
                    dr_子["出入库申请单号"] = str_出入库申请单号;
                    dr_子["出入库申请明细号"] = str_出入库申请单号 + "-" + pos.ToString("00");
                    dr_子["POS"] = pos;
                    dr_子["物料编码"] = drr["物料编码"].ToString();
                    dr_子["数量"] = math;
                    dr_子["相关单位"] = dr["供应商"];
                    dr_子["备注"] = dr["采购明细号"].ToString();
                    dr_子["物料名称"] = drr["物料名称"].ToString();
                    dr_子["规格型号"] = drr["规格型号"].ToString();
                    dr_子["仓库号"] = drr["仓库号"].ToString();
                    dr_子["仓库名称"] = drr["仓库名称"].ToString();

                }


                // string QI入库单号 = string.Format("QW{0}{1:00}{2:0000}",
                //                       t.Year, t.Month, CPublic.CNo.fun_得到最大流水号("QW", t.Year, t.Month));
                ////DataRow drrr= dt_入库主.NewRow();
                //dt_入库主.Rows.Add(drrr);
                //drrr["其他入库单号"]=QI入库单号;
                //drrr["入库日期"]=t;
                //drrr["备注"]=dr["采购单号"].ToString();
                //drrr["操作人员编号"]=CPublic.Var.LocalUserID;
                //drrr["操作人员"]=CPublic.Var.localUserName;
                //drrr


                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction mt = conn.BeginTransaction("供应商赠送申请");
                try
                {

                    string sql2 = "select * from 其他出入库申请主表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql2, conn, mt);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);

                    da.Update(dt_申请主);

                    string sql_son2 = "select * from 其他出入库申请子表 where 1<>1";
                    cmd = new SqlCommand(sql_son2, conn, mt);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_申请子);
                    mt.Commit();

                }
                catch (Exception ex)
                {
                    mt.Rollback();
                    throw new Exception("退料申请失败" + ex.Message);
                }
                MessageBox.Show("申请完成");
            }



        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        //private void repositoryItemSearchLookUpEdit5View_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        //{
        //    try
        //    {
        //        DataRow sr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
        //        DataRow dr = gvrk.GetDataRow(gvrk.FocusedRowHandle);
        //        dr["仓库号"] = sr["仓库号"].ToString();
        //        dr["仓库名称"] = sr["仓库名称"].ToString();
        //        string sql = "select * form 仓库物料数量表 where 物料编码 = '"+dr["物料编码"] +"' and 仓库号 = '"+dr["仓库号"] +"'";
        //        SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
        //        DataTable dt_物料数量 = new DataTable();
        //        da.Fill(dt_物料数量);
        //        if(dt_物料数量.Rows.Count == 0)
        //        {
        //            dr["库存总数"] = 0;
        //        }
        //        else
        //        {
        //            dr["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
        //        }

        //    }

        //    catch { }
        //}

        //private void repositoryItemSearchLookUpEdit5View_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        //{
        //    try
        //    {
        //        DataRow sr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
        //        DataRow dr = gvrk.GetDataRow(gvrk.FocusedRowHandle);
        //        dr["仓库号"] = sr["仓库号"].ToString();
        //        dr["仓库名称"] = sr["仓库名称"].ToString();
        //        string sql = "select * from 仓库物料数量表 where 物料编码 = '" + dr["物料编码"] + "' and 仓库号 = '" + dr["仓库号"] + "'";
        //        SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
        //        DataTable dt_物料数量 = new DataTable();
        //        da.Fill(dt_物料数量);
        //        if (dt_物料数量.Rows.Count == 0)
        //        {
        //            dr["库存总数"] = 0;
        //        }
        //        else
        //        {
        //            dr["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
        //        }

        //    }

        //    catch { }
        //}

        private void gvrk_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = gvrk.GetDataRow(gvrk.FocusedRowHandle);
                if (e.Column.FieldName == "仓库号")
                {
                    dr["仓库号"] = e.Value;
                    DataRow[] ds = dt_仓库.Select(string.Format("仓库号 = {0}", dr["仓库号"]));
                    dr["仓库名称"] = ds[0]["仓库名称"];
                    string sql = "select * from 仓库物料数量表 where 物料编码 = '" + dr["物料编码"] + "' and 仓库号 = '" + dr["仓库号"] + "'";
                    SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                    DataTable dt_物料数量 = new DataTable();
                    da.Fill(dt_物料数量);
                    if (dt_物料数量.Rows.Count == 0)
                    {
                        dr["库存总数"] = 0;
                        dr["货架描述"] = "";
                    }
                    else
                    {
                        dr["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
                        dr["货架描述"] = dt_物料数量.Rows[0]["货架描述"];//19-9-17解决货架更新
                    }
                }
            }

            catch { }
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
                    //DataTable tt = dtM.Copy();
                    //tt.Columns.Remove("作废");
                    gcJYD.ExportToXlsx(saveFileDialog.FileName);
                    //ERPorg.Corg.TableToExcel(tt, saveFileDialog.FileName);
                    MessageBox.Show("导出成功");
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
