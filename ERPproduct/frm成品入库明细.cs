using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class frm成品入库明细 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {
        #region 变量
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        string strcon = "";

        /// <summary>
        /// 成品检验单号
        /// </summary>
        string str_成品入库单号 = "";

        /// <summary>
        /// 成品入库的明细表
        /// </summary>
        DataTable dt_明细;
        DataTable dt_入库主表;
        DataTable dt_检验单;
        DataTable dt_仓库;
        DataTable dt_wl;
        DataTable dt_仓库号;
        DataTable dt_out_main;

        string sql_ck = "";
        string cfgfilepath = "";
        DataTable dt_打印1, dt_打印2;
        /// <summary>
        /// 人事员工表
        /// </summary>
        DataTable dt_人员;
        #endregion

        #region 加载
        public frm成品入库明细()
        {
            InitializeComponent();
            strcon = CPublic.Var.strConn;
        }

        public frm成品入库明细(string strorder)
        {
            InitializeComponent();
            str_成品入库单号 = strorder; //成品检验单号
            strcon = CPublic.Var.strConn;
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm成品入库明细_Load(object sender, EventArgs e)
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
                    gv_检验单.RestoreLayoutFromXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }
                //devGridControlCustom1.UserName = CPublic.Var.LocalUserID;
                //devGridControlCustom1.strConn = CPublic.Var.strConn;
                dt_人员 = ERPorg.Corg.fun_hr("仓库", CPublic.Var.LocalUserID);
                if (dt_人员.Rows.Count > 0)
                {
                    dataBindHelper1.DataFormDR(dt_人员.Rows[0]);
                }
                string sql_ckry = string.Format("select * from 人员仓库对应表 where 工号='{0}'", CPublic.Var.LocalUserID);
                dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_ckry, strcon);
                Fun_下拉框选择项();
                fun_load();

                txt_luruTime.EditValue = Convert.ToDateTime(CPublic.Var.getDatetime().ToString("yyyy-MM-dd"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Fun_下拉框选择项()
        {
            dt_仓库号 = new DataTable();
            string sql4 = "select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别'  and 布尔字段6=1";
            SqlDataAdapter da = new SqlDataAdapter(sql4, strcon);
            da.Fill(dt_仓库号);
            repositoryItemSearchLookUpEdit1.DataSource = dt_仓库号;
            repositoryItemSearchLookUpEdit1.DisplayMember = "仓库号";
            repositoryItemSearchLookUpEdit1.ValueMember = "仓库号";
        }
        #endregion

        #region  调用的方法
        //载入代办事项
#pragma warning disable IDE1006 // 命名样式
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {
            sql_ck = "and gd.仓库号  in(";
            string sql = "";
            if (dt_仓库.Rows.Count > 0)
            {
                foreach (DataRow dr in dt_仓库.Rows)
                {
                    sql_ck = sql_ck + string.Format("'{0}',", dr["仓库号"]);

                }

                sql_ck = sql_ck.Substring(0, sql_ck.Length - 1) + ")";

                sql = string.Format(@"select jy.*,生产制令单号,预计完工日期 ,isnull(未领量,0)未领量,gd.生产工单类型
            from 生产记录生产检验单主表 jy 
            left join 仓库物料数量表 kc  on jy.物料编码=kc.物料编码 and jy.仓库号 =kc.仓库号
            left join 生产记录生产工单表 gd on jy.生产工单号=gd.生产工单号
            where jy.生效 = 1 and jy.作废 = 0 and jy.完成 = 0 and  jy.已检验数量-jy.报废数-已入库数量>0
            /*  and  jy.包装确认 = 1 */ {0}  order by 预计完工日期 ", sql_ck);
                // 19-3-13 东屋这边暂时没有包装确认节点 /* and  jy.包装确认 = 1*/ 
            }
            else
            {
                sql = @"      select jy.*,生产制令单号,预计完工日期,isnull(未领量,0)未领量,gd.生产工单类型
            from 生产记录生产检验单主表 jy
            left join 仓库物料数量表 kc  on jy.物料编码=kc.物料编码 and jy.仓库号 =kc.仓库号
            left join 生产记录生产工单表 gd on jy.生产工单号=gd.生产工单号
            where jy.生效 = 1 and jy.作废 = 0 and jy.完成 = 0 and  jy.已检验数量-jy.报废数-已入库数量>0
            /* and   jy.包装确认 = 1 */  order by 预计完工日期 ";
                // 19-3-13 东屋这边暂时没有包装确认节点 /* and  jy.包装确认 = 1*/ 
            }
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                dt_检验单 = new DataTable();
                dt_检验单.Columns.Add("选择", typeof(bool));
                da.Fill(dt_检验单);
                gc_检验单.DataSource = dt_检验单;
                //DataView dv = new DataView(dt_检验单);
                //dv.RowFilter = "完成 = 0";
                //gc_检验单.DataSource = dv;
            }
            string sql_mx = @"select rkmx.*,生产制令单号,完工,工单负责人ID
                                        from 生产记录成品入库单明细表 rkmx,基础数据物料信息表 base,生产记录生产工单表 gd
                                        where base.物料编码= rkmx.物料编码 and gd.生产工单号=rkmx.生产工单号  and  1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_mx, strcon))
            {
                dt_明细 = new DataTable();
                dt_明细.Columns.Add("重检合格数");
                dt_明细.Columns.Add("合格数量");
                dt_明细.Columns.Add("库存总数", typeof(decimal));
                dt_明细.Columns.Add("仓库号");
                dt_明细.Columns.Add("仓库名称");
                dt_明细.Columns.Add("货架描述");
                //20-5-11 生产工单类型
                dt_明细.Columns.Add("生产工单类型");

                da.Fill(dt_明细);
                gc.DataSource = dt_明细;
            }
            #region 无用
            if (str_成品入库单号 != "")
            {
                DataTable dt = new DataTable();
                string sql_1 = string.Format(@"select rkmx.*,生产制令单号,rkz.成品入库单号,完工,库存总数,gd.仓库号,gd.仓库名称,rkz.录入日期,工单负责人ID                                
                                                from 生产记录成品入库单明细表 rkmx left join 生产记录成品入库单主表 rkz
                                                on  rkmx.成品入库单号=rkz.成品入库单号
                                                left join 仓库物料数量表 kc on    kc.物料编码= rkmx.物料编码
                                                left  join  生产记录生产工单表 gd on gd.生产工单号=rkz.生产工单号
                                                where rkmx.成品入库单号='{0}' and kc.仓库号=gd.仓库号 ", str_成品入库单号);
                using (SqlDataAdapter da = new SqlDataAdapter(sql_1, strcon))
                {
                    da.Fill(dt_明细);
                    if (dt_明细.Rows.Count > 0)
                    {
                        DataRow[] dr = dt_检验单.Select(string.Format("生产检验单号='{0}'", dt_明细.Rows[0]["生产检验单号"]));
                        dr[0]["选择"] = true;
                        txt_Cprkdan.Text = str_成品入库单号;
                        txt_luruTime.EditValue = Convert.ToDateTime(Convert.ToDateTime(dt_明细.Rows[0]["录入日期"]).ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        MessageBox.Show("数据有误");
                    }
                }
            }
            #endregion
        }

        private void Fun_check()
        {
            int i = 0;
            foreach (DataRow dr in dt_检验单.Rows)
            {
                if (dr["选择"].Equals(true))
                {
                    i++;
                }
            }
            if (i == 0)
            {
                throw new Exception("未选择检验单入库");
            }

            if (txt_luruTime.EditValue == null)
            {
                throw new Exception("请选择录入时间");
            }
            foreach (DataRow r in dt_明细.Rows)
            {
                if (Convert.ToDecimal(r["入库数量"]) > Convert.ToDecimal(r["合格数量"]) + Convert.ToDecimal(r["重检合格数"]))
                {
                    throw new Exception("入库数量大于可入库数量");
                }
                if (Convert.ToDecimal(r["入库数量"]) <= 0)
                {
                    throw new Exception("入库数量需大于0");
                }
                if (r["仓库号"].ToString() == "")
                {
                    throw new Exception("仓库号必填");
                }
            }
        }
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 2018-12-3 加入 入库倒冲
        /// dt_入库主表,dt_apply_main ,dt_明细,dt_apply_detail, 
        /// </summary>
        /// <returns></returns>
        private DataSet fun_save()
#pragma warning restore IDE1006 // 命名样式
        {
            DataSet ds = new DataSet();
            DateTime t = CPublic.Var.getDatetime();
            string str_入库单号 = "";

            //生产记录成品入库单主表    生产记录成品入库单明细表
            if (txt_Cprkdan.Text == "")   //  若有值 则是 已存在的 入库单子
            {
                str_入库单号 = string.Format("MM{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("MM", t.Year, t.Month));
                txt_Cprkdan.Text = str_入库单号;

                string sql = "select * from 生产记录成品入库单主表 where 1<>1 ";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    dt_入库主表 = new DataTable();
                    da.Fill(dt_入库主表);
                    DataRow dr = dt_入库主表.NewRow();
                    dr["GUID"] = System.Guid.NewGuid();
                    dr["入库人员"] = txt_peopleName.Text;
                    dr["入库人员工号"] = textBox1.Text;
                    dr["生效"] = true;
                    dr["生效人员ID"] = CPublic.Var.LocalUserID;
                    dr["生效人员"] = CPublic.Var.localUserName;
                    dr["生效日期"] = t;
                    //dr["生效"] = true;
                    dataBindHelper1.DataToDR(dr);
                    dt_入库主表.Rows.Add(dr);
                    ds.Tables.Add(dt_入库主表);
                }
            }
            else  //如果是 列表界面转过来的 保存主表 
            {
                str_入库单号 = str_成品入库单号;
                string sql = string.Format("select * from 生产记录成品入库单主表 where 成品入库单号='{0}'", str_成品入库单号);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    dt_入库主表 = new DataTable();
                    da.Fill(dt_入库主表);
                    dt_入库主表.Rows[0]["入库人员"] = txt_peopleName.Text;
                    dt_入库主表.Rows[0]["入库人员工号"] = textBox1.Text;
                    dt_入库主表.Rows[0]["生效"] = true;
                    dt_入库主表.Rows[0]["生效人员ID"] = CPublic.Var.LocalUserID;
                    dt_入库主表.Rows[0]["生效人员"] = CPublic.Var.localUserName;
                    dt_入库主表.Rows[0]["生效日期"] = t;
                    ds.Tables.Add(dt_入库主表);
                }
            }

            string sql_mx = string.Format("select * from 生产记录成品入库单明细表 where 1<>1");

            string s = "select  * from 其他出入库申请主表 where 1<>1";
            DataTable dt_apply_main = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select  * from 其他出入库申请子表 where 1<>1";
            DataTable dt_apply_detail = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select * from  其他出库主表 where 1<>1";
            DataTable dt_out_main = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select * from  其他出库子表 where 1<>1";
            DataTable dt_out_detail = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            s = "select * from  仓库出入库明细表 where 1<>1";
            DataTable dt_出入库明细 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            // DataTable dt_审核 = new DataTable();   18-12-5 其他出入库申请不需要审批流

            using (SqlDataAdapter da = new SqlDataAdapter(sql_mx, strcon))
            {
                int pos = 1;
                DataTable dt_判断库存 = new DataTable();
                foreach (DataRow dr in dt_明细.Rows)
                {
                    dr["GUID"] = System.Guid.NewGuid();
                    dr["成品入库单号"] = txt_Cprkdan.Text;
                    dr["POS"] = pos.ToString("00");
                    dr["成品入库单明细号"] = str_入库单号 + "-" + pos.ToString("00");
                    dr["入库仓库ID"] = dr["仓库号"];
                    dr["入库仓库"] = dr["仓库名称"];
                    dr["入库人员ID"] = textBox1.Text;
                    dr["入库人员"] = txt_peopleName.Text;
                    dr["修改日期"] = t;
                    dr["生效"] = true;
                    dr["生效人员ID"] = CPublic.Var.LocalUserID;
                    dr["生效人员"] = CPublic.Var.localUserName;
                    dr["生效日期"] = t;
                    pos++;

                    ///19-6-28如果是 返修工单 不需要 生产 入库倒冲的 
                    //s = string.Format("select 生产工单类型 from 生产记录生产工单表 where 生产工单号='{0}' and 生产工单类型<>'返修工单'", dr["生产工单号"]);
                    //DataTable t_temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    //if (t_temp.Rows.Count > 0)
                    //{

                    //    //查询该明细子项有没有  wiptype 为领料的,如果有 则再其他出入库申请和其他出库中增加
                    //    s = string.Format(@"select 物料编码,物料名称,base.规格型号,数量,bom.仓库号,bom.仓库名称 from 基础数据物料BOM表 bom
                    //        left join 基础数据物料信息表 base on bom.子项编码=base.物料编码 where WIPType ='入库倒冲' and 产品编码='{0}'", dr["物料编码"]);
                    //    DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    //    if (temp.Rows.Count > 0)
                    //    {
                    //        if (!bl)
                    //        {
                    //            s_applyNo = string.Format("DWLS{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                    //             t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("DWLS", t.Year, t.Month).ToString("0000"));
                    //            s_out_No = string.Format("LS{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                    //            t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("LS", t.Year, t.Month).ToString("0000"));

                    //            DataRow dr_apply_main = dt_apply_main.NewRow();
                    //            dt_apply_main.Rows.Add(dr_apply_main);
                    //            dr_apply_main["GUID"] = System.Guid.NewGuid();
                    //            dr_apply_main["出入库申请单号"] = s_applyNo;
                    //            dr_apply_main["申请日期"] = t;
                    //            dr_apply_main["申请类型"] = "材料出库";
                    //            dr_apply_main["备注"] = str_入库单号;
                    //            dr_apply_main["操作人员编号"] = CPublic.Var.LocalUserID;
                    //            dr_apply_main["操作人员"] = CPublic.Var.localUserName;
                    //            dr_apply_main["生效"] = true;
                    //            dr_apply_main["生效日期"] = t;
                    //            dr_apply_main["生效人员编号"] = CPublic.Var.LocalUserID;
                    //            dr_apply_main["完成"] = true;
                    //            dr_apply_main["完成日期"] = t;
                    //            dr_apply_main["原因分类"] = "入库倒冲";
                    //            dr_apply_main["单据类型"] = "材料出库";


                    //            DataRow dr_out_main = dt_out_main.NewRow();
                    //            dt_out_main.Rows.Add(dr_out_main);
                    //            dr_out_main["GUID"] = System.Guid.NewGuid();
                    //            dr_out_main["其他出库单号"] = s_out_No;
                    //            dr_out_main["出库类型"] = "材料出库";
                    //            dr_out_main["操作人员编号"] = CPublic.Var.LocalUserID;
                    //            dr_out_main["操作人员"] = CPublic.Var.localUserName;
                    //            dr_out_main["出库日期"] = t;
                    //            dr_out_main["生效"] = true;
                    //            dr_out_main["生效日期"] = t;
                    //            dr_out_main["创建日期"] = t;
                    //            dr_out_main["出入库申请单号"] = s_applyNo;

                    //            // dt_审核 = ERPorg.Corg.fun_PA("生效", "其他出入库申请单", s_applyNo, "入库倒冲"); 
                    //            bl = true;

                    //        }
                    //        //根据列表生成其他出入库申请子表记录 和 其他出库子表记录     审核申请表 记录
                    //        foreach (DataRow rr in temp.Rows)
                    //        {
                    //            DataRow dr_apply_detail = dt_apply_detail.NewRow();
                    //            dt_apply_detail.Rows.Add(dr_apply_detail);
                    //            dr_apply_detail["GUID"] = System.Guid.NewGuid();
                    //            dr_apply_detail["出入库申请单号"] = s_applyNo;
                    //            dr_apply_detail["POS"] = i;
                    //            dr_apply_detail["出入库申请明细号"] = s_applyNo + "-" + i.ToString("00");
                    //            dr_apply_detail["物料编码"] = rr["物料编码"];

                    //            dr_apply_detail["规格型号"] = rr["规格型号"];

                    //            dr_apply_detail["物料名称"] = rr["物料名称"];
                    //            dr_apply_detail["数量"] = Convert.ToDecimal(rr["数量"]) * Convert.ToDecimal(dr["入库数量"]);//倒冲数量=bom数量*成品入库数量

                    //            //  dr_apply_detail["备注"] = dr["物料编码"].ToString();//其他出入库申请主表备注记录成品入库单号 这边子表备注就记录成品编码
                    //            //19-6-23  计算 财务得 成本核算得时候 改为 工单号
                    //            dr_apply_detail["备注"] = dr["生产工单号"].ToString();//其他出入库申请主表备注记录成品入库单号 这边子表备注就记录成品编码

                    //            dr_apply_detail["生效"] = true;
                    //            dr_apply_detail["生效日期"] = t;
                    //            dr_apply_detail["生效人员编号"] = CPublic.Var.LocalUserID;
                    //            dr_apply_detail["完成"] = true;
                    //            dr_apply_detail["完成日期"] = t;
                    //            dr_apply_detail["仓库号"] = rr["仓库号"];
                    //            dr_apply_detail["仓库名称"] = rr["仓库名称"];
                    //            try
                    //            {
                    //                dt_判断库存 = ERPorg.Corg.fun_库存(-1, dt_apply_detail);
                    //            }         
                    //            catch
                    //            {
                    //                throw new Exception("入库倒冲的料不足");
                    //            }




                    //            DataRow dr_out_detail = dt_out_detail.NewRow();
                    //            dt_out_detail.Rows.Add(dr_out_detail);
                    //            dr_out_detail["物料编码"] = rr["物料编码"];
                    //            //dr_其他出库子["原ERP物料编号"] = dr_借用明细["原ERP物料编号"];
                    //            dr_out_detail["物料名称"] = rr["物料名称"];
                    //            dr_out_detail["数量"] = Convert.ToDecimal(dr_apply_detail["数量"]);

                    //            dr_out_detail["规格型号"] = rr["规格型号"];
                    //            // dr_其他出库子["图纸编号"] = rr["图纸编号"];
                    //            dr_out_detail["其他出库单号"] = s_out_No;
                    //            dr_out_detail["POS"] = i;
                    //            dr_out_detail["其他出库明细号"] = s_out_No + "-" + i.ToString("00");
                    //            dr_out_detail["GUID"] = System.Guid.NewGuid();
                    //            dr_out_detail["备注"] = dr["生产工单号"].ToString();
                    //            dr_out_detail["生效"] = true;
                    //            dr_out_detail["生效日期"] = t;
                    //            dr_out_detail["生效人员编号"] = CPublic.Var.LocalUserID;
                    //            dr_out_detail["完成"] = true;
                    //            dr_out_detail["完成日期"] = t;
                    //            dr_out_detail["完成人员编号"] = CPublic.Var.LocalUserID;
                    //            dr_out_detail["出入库申请单号"] = s_applyNo;
                    //            dr_out_detail["出入库申请明细号"] = dr_apply_detail["出入库申请明细号"];

                    //            DataRow dr_出入库 = dt_出入库明细.NewRow();
                    //            dt_出入库明细.Rows.Add(dr_出入库);
                    //            dr_出入库["GUID"] = System.Guid.NewGuid();
                    //            dr_出入库["明细类型"] = "入库倒冲";
                    //            dr_出入库["单号"] = s_out_No;
                    //            dr_出入库["出库入库"] = "出库";
                    //            dr_出入库["物料编码"] = rr["物料编码"];
                    //            dr_出入库["物料名称"] = rr["物料名称"];
                    //            dr_出入库["仓库号"] = rr["仓库号"];
                    //            dr_出入库["仓库名称"] = rr["仓库名称"];
                    //            dr_出入库["明细号"] = dr_out_detail["其他出库明细号"];
                    //            dr_出入库["相关单号"] = dr["生产工单号"];

                    //            //string ss = string.Format("select 车间名称 from 生产记录生产工单表 where 生产工单号='{0}'", dr["生产工单号"].ToString());
                    //            //DataTable t_s = CZMaster.MasterSQL.Get_DataTable(ss, strcon);
                    //            //dr_出入库["相关单位"] = t_s.Rows[0]["车间名称"];
                    //            dr_出入库["实效数量"] = -(Convert.ToDecimal(dr_out_detail["数量"]));
                    //            dr_出入库["实效时间"] = t;
                    //            dr_出入库["出入库时间"] = t;
                    //            i++;
                    //        }
                    //    }
                    //}
                }




                ds.Tables.Add(dt_明细);
                // ds.Tables.Add(dt_apply_main);
                //   ds.Tables.Add(dt_out_main);


                //ds.Tables.Add(dt_apply_detail);
                //ds.Tables.Add(dt_out_main);
                //ds.Tables.Add(dt_out_detail);
                //ds.Tables.Add(dt_出入库明细);
                //ds.Tables.Add(dt_判断库存);
                //  ds.Tables.Add(dt_审核);    
            }
            return ds;
        }

#pragma warning disable IDE1006 // 命名样式
        private DataTable fun_检验单()
#pragma warning restore IDE1006 // 命名样式
        {
            //检验单 完成 赋值1
            foreach (DataRow dr in dt_明细.Rows)
            {
                DataRow[] ds = dt_检验单.Select(string.Format("生产检验单号 = '{0}'", dr["生产检验单号"]));
                ds[0]["已入库数量"] = Convert.ToDecimal(ds[0]["已入库数量"]) + Convert.ToDecimal(dr["入库数量"]);
                if (Convert.ToDecimal(ds[0]["已入库数量"]) >= Convert.ToDecimal(ds[0]["送检数量"]) - Convert.ToDecimal(ds[0]["报废数"]))
                {
                    ds[0]["完成"] = 1;
                    ds[0]["完成日期"] = CPublic.Var.getDatetime();
                    ds[0]["完成人员ID"] = CPublic.Var.LocalUserID;
                    ds[0]["完成人员"] = CPublic.Var.localUserName;
                }
            }

            string sql_jyd = "select * from 生产记录生产检验单主表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_jyd, strcon))
            {
                return dt_检验单;
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private DataSet fun_虚拟库存()
#pragma warning restore IDE1006 // 命名样式
        {
            DataSet ds = new DataSet();
            DataTable dt_BOM;
            DataTable dt_明细 = new DataTable();
            DataTable dt_主表 = new DataTable();

            foreach (DataRow dr in dt_明细.Rows)
            {
                decimal dec_生产数量 = 0;
                decimal dec_入库数量 = 0;
                string sql_BOM = string.Format("select * from 基础数据物料BOM表 where 物料编码='{0}'", dr["物料编码"].ToString());
                using (SqlDataAdapter da = new SqlDataAdapter(sql_BOM, strcon))
                {
                    dt_BOM = new DataTable();
                    da.Fill(dt_BOM);
                }

                //搜索相应工单的 生产数量
                string sql_工单 = string.Format("select * from 生产记录生产工单表 where 生产工单号='{0}'", dr["生产工单号"].ToString());
                using (SqlDataAdapter da = new SqlDataAdapter(sql_工单, strcon))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        dec_生产数量 = Convert.ToDecimal(dt.Rows[0]["生产数量"]);
                    }
                }
                // 搜索 生产记录成品入库单明细表 中 对应 工单 入库数量 之和
                string sql_入库单 = string.Format("select * from 生产记录成品入库单明细表 where 生产工单号='{0}'", dr["生产工单号"].ToString());
                using (SqlDataAdapter da = new SqlDataAdapter(sql_入库单, strcon))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow rrr in dt.Rows)
                        {
                            dec_入库数量 += Convert.ToDecimal(rrr["入库数量"]);
                        }
                    }
                }

                foreach (DataRow dr_bom in dt_BOM.Rows)
                {
                    // 保存明细表
                    string sql_mx = string.Format(
                        "select * from 生产记录车间虚拟库存明细表 where 物料编码='{0}'and 生产车间='{1}' and 生产工单号='{2}'",
                        dr_bom["子项编码"], dr["入库车间"], dr["生产工单号"]);
                    using (SqlDataAdapter da = new SqlDataAdapter(sql_mx, strcon))
                    {
                        da.Fill(dt_明细);
                        if (dt_明细.Rows.Count > 0) //找到记录, 主表中只应有一条  修改
                        {
                            dt_明细.Rows[0]["未用数量"] = Convert.ToDecimal(dt_明细.Rows[0]["未用数量"]) - (Convert.ToDecimal(dr["入库数量"]) / (dec_生产数量 - dec_入库数量)) * Convert.ToDecimal(dt_明细.Rows[0]["未用数量"]);
                            dt_明细.Rows[0]["修改日期"] = CPublic.Var.getDatetime();
                        }
                        //new SqlCommandBuilder(da);
                        //da.Update(dt);
                    }
                    //保存 虚拟主表
                    string sql_1 = string.Format("select * from 生产记录车间虚拟库存表 where  生产车间='{0}'and 物料编码='{1}' ", dr["入库车间"], dr_bom["子项编码"]);
                    using (SqlDataAdapter da = new SqlDataAdapter(sql_1, strcon))
                    {
                        da.Fill(dt_主表);
                        if (dt_主表.Rows.Count > 0)
                        {
                            dt_主表.Rows[0]["未用数量"] = Convert.ToDecimal(dt_主表.Rows[0]["未用数量"]) - (Convert.ToDecimal(dr["入库数量"]) / (dec_生产数量 - dec_入库数量)) * Convert.ToDecimal(dt_主表.Rows[0]["未用数量"]);
                            dt_主表.Rows[0]["车间数量"] = dt_主表.Rows[0]["未用数量"];
                            dt_主表.Rows[0]["修改日期"] = CPublic.Var.getDatetime();
                        }
                        //new SqlCommandBuilder(da);
                        //da.Update(dt_主表);
                    }
                }
            }
            ds.Tables.Add(dt_主表);
            ds.Tables.Add(dt_明细);
            return ds;
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 18-3-1 新物料属性
        /// </summary>
        /// <returns></returns>
        private DataTable fun_xxx()
#pragma warning restore IDE1006 // 命名样式
        {
            DataTable dt = new DataTable();

            foreach (DataRow dr in dt_明细.Rows)
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

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 18-11-30 入库倒冲 扣料的 放在这个函数里
        /// </summary>
        /// <returns></returns>
        private DataTable fun_save出入库明细()
#pragma warning restore IDE1006 // 命名样式
        {
            DateTime t = CPublic.Var.getDatetime();
            DataTable dt = new DataTable();
            string sql = "select * from 仓库出入库明细表 where 1<>1";
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            foreach (DataRow dr in dt_明细.Rows)
            {

                DataRow r = dt.NewRow();
                r["GUID"] = System.Guid.NewGuid();
                r["明细类型"] = "生产入库";
                r["单号"] = txt_Cprkdan.Text;
                r["出库入库"] = "入库";
                r["物料编码"] = dr["物料编码"];
                r["物料名称"] = dr["物料名称"];
                r["仓库号"] = dr["仓库号"];
                r["仓库名称"] = dr["仓库名称"];
                r["明细号"] = dr["成品入库单明细号"];
                r["相关单号"] = dr["生产工单号"];
                r["仓库人"] = CPublic.Var.localUserName;


                string ss = string.Format("select 车间名称 from 生产记录生产工单表 where 生产工单号='{0}'", dr["生产工单号"].ToString());
                DataTable t_s = CZMaster.MasterSQL.Get_DataTable(ss, strcon);
                r["相关单位"] = t_s.Rows[0]["车间名称"];
                r["实效数量"] = (Convert.ToDecimal(dr["入库数量"]));
                r["实效时间"] = t;
                r["出入库时间"] = t;

                dt.Rows.Add(r);


            }
            return dt;
        }
        //
#pragma warning disable IDE1006 // 命名样式
        private void fun_save_zf()
#pragma warning restore IDE1006 // 命名样式
        {
            foreach (DataRow dr in dt_明细.Rows)
            {
                //StockCore.StockCorer.fun_刷新库存(dr["物料编码"].ToString(), Convert.ToDecimal(dr["入库数量"]), 1);
                StockCore.StockCorer.fun_物料数量_实际数量(dr["物料编码"].ToString().Trim(), dr["仓库号"].ToString(), true);
            }
        }
        //入库主表和明细表的生效字段 
        //不要用
#pragma warning disable IDE1006 // 命名样式
        private void fun_生效状态()
#pragma warning restore IDE1006 // 命名样式
        {
            //DataSet ds = new DataSet();
            DataTable dt_入库明细 = new DataTable();
            DataTable dt_入库主表 = new DataTable();

            string sql = string.Format("select * from 生产记录成品入库单明细表 where 成品入库单号='{0}'", txt_Cprkdan.Text);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                da.Fill(dt_入库明细);
                foreach (DataRow dr in dt_入库明细.Rows)
                {
                    dr["生效"] = true;
                    dr["生效人员ID"] = CPublic.Var.LocalUserID;
                    dr["生效人员"] = CPublic.Var.localUserName;
                    dr["生效日期"] = CPublic.Var.getDatetime();
                }
                new SqlCommandBuilder(da);
                da.Update(dt_入库明细);
                //ds.Tables.Add(dt_入库明细);
            }

            string sql1 = string.Format("select * from 生产记录成品入库单主表 where 成品入库单号='{0}'", txt_Cprkdan.Text);
            using (SqlDataAdapter da = new SqlDataAdapter(sql1, strcon))
            {
                da.Fill(dt_入库主表);
                if (dt_入库主表.Rows.Count > 0)
                {
                    dt_入库主表.Rows[0]["生效"] = true;
                    dt_入库主表.Rows[0]["生效人员ID"] = CPublic.Var.LocalUserID;
                    dt_入库主表.Rows[0]["生效人员"] = CPublic.Var.localUserName;
                    dt_入库主表.Rows[0]["生效日期"] = CPublic.Var.getDatetime();
                    new SqlCommandBuilder(da);
                    da.Update(dt_入库主表);
                    //ds.Tables.Add(dt_入库主表);
                }
                else
                {
                    MessageBox.Show("这条数据有误");
                }
            }
            //return ds;
        }
        #endregion

        #region 界面操作
        //刷新
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_load();
            txt_Cprkdan.Text = "";
            txt_stockName.Text = "";
            textBox1.Text = "";
            textBox2.Text = "";
            txt_luruTime.EditValue = Convert.ToDateTime(CPublic.Var.getDatetime().ToString("yyyy-MM-dd"));
            txt_peopleName.Text = "";
            if (dt_人员.Rows.Count > 0)
            {
                dataBindHelper1.DataFormDR(dt_人员.Rows[0]);
            }
        }

        //关闭
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_保存按钮保存()
#pragma warning restore IDE1006 // 命名样式
        {
            string str_入库单号 = "";
            //生产记录成品入库单主表    生产记录成品入库单明细表
            if (txt_Cprkdan.Text == "")   //  若有值 则是 已存在的 入库单子
            {
                DateTime t = CPublic.Var.getDatetime();
                str_入库单号 = string.Format("MM{0}{1:D2}{2:D4}", t.Year, t.Month, CPublic.CNo.fun_得到最大流水号("MM", t.Year, t.Month));
                txt_Cprkdan.Text = str_入库单号;

                string sql = "select * from 生产记录成品入库单主表 where 1<>1 ";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    dt_入库主表 = new DataTable();
                    da.Fill(dt_入库主表);
                    DataRow dr = dt_入库主表.NewRow();
                    dr["GUID"] = System.Guid.NewGuid();
                    dr["入库人员"] = txt_peopleName.Text;
                    dr["入库人员工号"] = textBox1.Text;

                    //dr["生效"] = true;
                    dataBindHelper1.DataToDR(dr);
                    dt_入库主表.Rows.Add(dr);

                }
            }
            else  //如果是 列表界面转过来的 保存主表
            {
                str_入库单号 = str_成品入库单号;
                string sql = string.Format("select * from 生产记录成品入库单主表 where 成品入库单号='{0}'", str_成品入库单号);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    dt_入库主表 = new DataTable();
                    da.Fill(dt_入库主表);
                    dt_入库主表.Rows[0]["入库人员"] = txt_peopleName.Text;
                    dt_入库主表.Rows[0]["入库人员工号"] = textBox1.Text;


                }
            }
            string sql_mx = string.Format("select * from 生产记录成品入库单明细表 where 1<>1");
            using (SqlDataAdapter da = new SqlDataAdapter(sql_mx, strcon))
            {
                int pos = 0;
                foreach (DataRow dr in dt_明细.Rows)
                {
                    dr["GUID"] = System.Guid.NewGuid();
                    dr["成品入库单号"] = txt_Cprkdan.Text;
                    dr["POS"] = pos.ToString("00");
                    dr["成品入库单明细号"] = str_入库单号 + "-" + pos.ToString("00");
                    dr["入库仓库ID"] = textBox2.Text;
                    dr["入库仓库"] = txt_stockName.Text;
                    dr["入库人员ID"] = textBox1.Text;
                    dr["入库人员"] = txt_peopleName.Text;
                    dr["修改日期"] = CPublic.Var.getDatetime();
                    pos++;
                }

            }
        }
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 17-10-8 外加工开票记录
        /// 传入需 生成采购待开票记录的 dt
        /// </summary>
        /// <returns></returns>
        private DataSet fun_kpfjl(DataTable dtP)
#pragma warning restore IDE1006 // 命名样式
        {

            DateTime t = CPublic.Var.getDatetime();
            DataSet ds = new DataSet();
            string sql = "";
            sql = "select * from [L采购记录采购单入库明细L]  where 1<>1";
            DataTable dt_frk = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            sql = "select * from [L采购记录采购单入库主表L]  where 1<>1";
            DataTable dt_rkz = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            DataRow dr_fz = dt_rkz.NewRow();
            dr_fz["GUID"] = System.Guid.NewGuid();
            string str_入库单号 = string.Format("PC{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                      CPublic.CNo.fun_得到最大流水号("PC", t.Year, t.Month));
            dr_fz["入库单号"] = str_入库单号;
            //dr_fz["供应商ID"] = textBox2.Text;

            //dr_fz["供应商"] = txt_供应商.Text;
            dr_fz["修改日期"] = dr_fz["录入日期"] = dr_fz["创建日期"] = t;
            dr_fz["操作员ID"] = CPublic.Var.LocalUserID;
            dr_fz["操作员"] = CPublic.Var.localUserName;
            dr_fz["生效"] = true;
            dr_fz["备注1"] = "外加工";
            dt_rkz.Rows.Add(dr_fz);
            int i = 1;
            foreach (DataRow rt in dtP.Rows)
            {

                #region 生成采购入库明细 待开票

                DataRow rr = dt_frk.NewRow();
                rr["GUID"] = System.Guid.NewGuid();
                rr["入库单号"] = str_入库单号;
                rr["入库明细号"] = str_入库单号 + "-" + i.ToString("00");
                rr["入库POS"] = i;
                //外加工 采购单号字段存 工单号 ，采购明细号字段 存 成品入库单号明细号 
                rr["采购单号"] = rt["成品入库单号"].ToString();
                rr["采购单明细号"] = rt["成品入库单明细号"].ToString();
                rr["送检单号"] = rt["生产工单号"].ToString();

                rr["物料编码"] = rt["物料编码"];
                rr["物料名称"] = rt["物料名称"];
                rr["图纸编号"] = rt["图纸编号"];
                string ss = string.Format("select * from 基础数据物料信息表 where 物料编码='{0}'", rt["物料编码"].ToString());
                using (SqlDataAdapter da = new SqlDataAdapter(ss, strcon))
                {
                    DataTable temp = new DataTable();
                    da.Fill(temp);
                    decimal dec = 0;
                    try
                    {
                        dec = Convert.ToDecimal(temp.Rows[0]["工时"]);
                        if (dec == 0)
                        {
                            throw new Exception(rt["物料编码"].ToString() + "的定额工时不正确");
                        }
                    }
                    catch (Exception)
                    {
                        throw new Exception(rt["物料编码"].ToString() + "的定额工时不正确");
                    }

                    rr["单价"] = 160 / dec;
                    rr["金额"] = Convert.ToDecimal(rr["单价"]) * Convert.ToDecimal(rt["入库数量"]);
                    rr["未税单价"] = Convert.ToDecimal(rr["单价"]) / (decimal)1.03;
                    rr["未税金额"] = Convert.ToDecimal(rr["未税单价"]) * Convert.ToDecimal(rt["入库数量"]);
                    rr["税率"] = 3;

                }
                //   入库数量/定额*160   税率为3

                //仅为 土吉  单价 计算公式： 160/定额工时 税率 3
                rr["供应商ID"] = "10000132";
                rr["供应商"] = "相城区土吉五金经营部";
                rr["生效"] = true;
                rr["录入日期"] = rr["生效日期"] = t;
                rr["操作员ID"] = rr["生效人员ID"] = rr["入库人员ID"] = CPublic.Var.LocalUserID;
                rr["操作员"] = rr["生效人员"] = rr["入库人员"] = CPublic.Var.localUserName;
                rr["入库量"] = Convert.ToDecimal(rt["入库数量"]);
                rr["备注1"] = "外加工";
                rr["价格核实"] = true;
                dt_frk.Rows.Add(rr);
                i++;
                #endregion
            }
            ds.Tables.Add(dt_rkz); //主表记录
            ds.Tables.Add(dt_frk); //明细
            return ds;
        }
        //保存
        //private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    try
        //    {
        //        gv.CloseEditor();
        //        this.BindingContext[dt_明细].EndCurrentEdit();
        //        int i = 0;
        //        foreach (DataRow r in dt_明细.Rows)
        //        {
        //            string sql = string.Format("select 物料状态,更改预计完成时间 from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"].ToString());
        //            DataTable t = new DataTable();
        //            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
        //            da.Fill(t);

        //            if (t.Rows[0]["物料状态"].ToString() == "更改")
        //            {
        //                DateTime time = (DateTime)t.Rows[0]["更改预计完成时间"];
        //                MessageBox.Show(string.Format("物料{0}为更改状态，不能入库，预计完成时间：{1}", r["物料编码"].ToString(), time.ToString("yyyy-MM-dd")));
        //                i = 1;
        //                break;
        //            }
        //        }
        //        if (i == 1)
        //        {
        //        }
        //        else
        //        {
        //            fun_check();
        //            fun_save();
        //            MessageBox.Show("保存成功！");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        //打印
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

        #endregion

        #region  成品入库单的生效

        //生效
        //2018-11-30 东屋电气 需要增加 在成品或者半成品入库时 bom中 入库倒冲的物料需要以其他出库的方式按比例出库
        //其他出入库申请主子表   其他出库主子表 仓库出入库明细表
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            gv.CloseEditor();
            this.BindingContext[dt_明细].EndCurrentEdit();
            gv_检验单.CloseEditor();
            this.BindingContext[dt_检验单].EndCurrentEdit();


            int i = 0;
            foreach (DataRow r in dt_明细.Rows)
            {
                string sql = string.Format("select 物料状态,更改预计完成时间 from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"].ToString());
                DataTable t = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                da.Fill(t);
                if (t.Rows[0]["物料状态"].ToString() == "更改")
                {
                    DateTime time = (DateTime)t.Rows[0]["更改预计完成时间"];
                    MessageBox.Show(string.Format("物料{0}为更改状态，不能入库，预计完成时间：{1}", r["物料编码"].ToString(), time.ToString("yyyy-MM-dd")));
                    i = 1;
                    break;
                }
            }

            if (i == 0)
            {
                try
                {
                    Fun_check();
                    if (MessageBox.Show(string.Format("是否确认入库信息？"), "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {



                        DataTable dt_czmx = dt_明细.Copy();
                        DataTable dt_工单状态 = StockCore.StockCorer.fun_成品出库_工单状态(dt_czmx);
                        DataTable dt_制令状态 = StockCore.StockCorer.fun_成品出库_制令状态(dt_工单状态);

                        //2019-12-26 浏览 这个虚拟库存是没有用的 
                        DataSet ds1 = fun_虚拟库存();
                        DataTable dt = fun_检验单();
                        DataSet ds2 = fun_save();
                        DataTable dt_kc = dt_明细.Copy();
                        dt_kc.Columns["入库数量"].ColumnName = "数量";
                        dt_kc.Columns["货架描述"].ColumnName = "新货架描述";

                        DataTable dt_库存 = ERPorg.Corg.fun_库存(1, dt_kc);


                        DataTable dt_成品入库 = new DataTable();
                        dt_成品入库 = StockCore.StockCorer.fun_RUKU("成品入库", dt_明细);


                        DataTable dt2 = fun_save出入库明细();
                        DataTable dt_y = fun_xxx();
                        string sql_虚拟主表 = "select * from 生产记录车间虚拟库存表 where 1<>1";
                        string sql_虚拟明细表 = "select * from 生产记录车间虚拟库存明细表 where 1<>1"; ;
                        string sql_检验单 = "select * from 生产记录生产检验单主表 where 1<>1";
                        string sql_成品入库主表 = "select * from 生产记录成品入库单主表 where 1<>1 ";
                        string sql_成品入库明细表 = "select * from 生产记录成品入库单明细表 where 1<>1 ";

                        string sql_出入库 = "select * from 仓库出入库明细表 where 1<>1";
                        string sql_库存 = "select * from 仓库物料数量表 where 1<>1";
                        string sql_基础表 = "select * from 基础数据物料信息表 where 1<>1";
                        SqlConnection conn = new SqlConnection(strcon);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("生产入库");
                        try
                        {
                            SqlCommand cmm = new SqlCommand("select * from 生产记录生产工单表 where 1<>1", conn, ts);
                            SqlDataAdapter da = new SqlDataAdapter(cmm);
                            new SqlCommandBuilder(da);
                            da.Update(dt_工单状态);

                            cmm = new SqlCommand("select * from 生产记录生产制令表  where 1<>1", conn, ts);
                            da = new SqlDataAdapter(cmm);
                            new SqlCommandBuilder(da);
                            da.Update(dt_制令状态);

                            cmm = new SqlCommand(sql_虚拟主表, conn, ts);
                            da = new SqlDataAdapter(cmm);
                            new SqlCommandBuilder(da);
                            da.Update(ds1.Tables[0]);

                            cmm = new SqlCommand(sql_虚拟明细表, conn, ts);
                            da = new SqlDataAdapter(cmm);
                            new SqlCommandBuilder(da);
                            da.Update(ds1.Tables[1]);
                            cmm = new SqlCommand(sql_检验单, conn, ts);
                            da = new SqlDataAdapter(cmm);
                            new SqlCommandBuilder(da);
                            da.Update(dt);
                            cmm = new SqlCommand(sql_成品入库主表, conn, ts);
                            da = new SqlDataAdapter(cmm);
                            new SqlCommandBuilder(da);
                            da.Update(ds2.Tables[0]);

                            cmm = new SqlCommand(sql_成品入库明细表, conn, ts);
                            da = new SqlDataAdapter(cmm);
                            new SqlCommandBuilder(da);
                            da.Update(ds2.Tables[1]);



                            cmm = new SqlCommand(sql_库存, conn, ts);
                            da = new SqlDataAdapter(cmm);
                            new SqlCommandBuilder(da);
                            da.Update(dt_库存);
                            cmm = new SqlCommand(sql_出入库, conn, ts);
                            da = new SqlDataAdapter(cmm);
                            new SqlCommandBuilder(da);
                            da.Update(dt2);
                            cmm = new SqlCommand(sql_基础表, conn, ts);
                            da = new SqlDataAdapter(cmm);
                            new SqlCommandBuilder(da);
                            da.Update(dt_y);



                            //if (ds2.Tables[1].Rows.Count > 0) //入库倒冲有数据
                            //{
                            //    string s = "select  * from 其他出入库申请主表 where 1<>1";
                            //    cmm = new SqlCommand(s, conn, ts);
                            //    da = new SqlDataAdapter(cmm);
                            //    new SqlCommandBuilder(da);
                            //    da.Update(ds2.Tables[1]);

                            //    s = "select  * from 其他出入库申请子表 where 1<>1";
                            //    cmm = new SqlCommand(s, conn, ts);
                            //    da = new SqlDataAdapter(cmm);
                            //    new SqlCommandBuilder(da);
                            //    da.Update(ds2.Tables[3]);

                            //    cmm = new SqlCommand(sql_其他出库主, conn, ts);
                            //    da = new SqlDataAdapter(cmm);
                            //    new SqlCommandBuilder(da);
                            //    da.Update(ds2.Tables[4]);

                            //    cmm = new SqlCommand(sql_其他出库子, conn, ts);
                            //    da = new SqlDataAdapter(cmm);
                            //    new SqlCommandBuilder(da);
                            //    da.Update(ds2.Tables[5]);

                            //    cmm = new SqlCommand(sql_出入库, conn, ts);
                            //    da = new SqlDataAdapter(cmm);
                            //    new SqlCommandBuilder(da);
                            //    da.Update(ds2.Tables[6]);

                            //    cmm = new SqlCommand(sql_库存, conn, ts);
                            //    da = new SqlDataAdapter(cmm);
                            //    new SqlCommandBuilder(da);
                            //    da.Update(ds2.Tables[7]);

                            //    //s = "select  * from 单据审核申请表 where 1<>1";
                            //    //cmm = new SqlCommand(s, conn, ts);
                            //    //da = new SqlDataAdapter(cmm);
                            //    //new SqlCommandBuilder(da);
                            //    //da.Update(ds2.Tables[4]);

                            //}
                            ts.Commit();
                            ////////
                            ///5.21增加打印   
                            ///dt_打印1
                            ///
                            dt_打印1 = new DataTable();
                            dt_打印1 = ds2.Tables[0].Copy();
                            dt_打印2 = new DataTable();
                            dt_打印2 = ds2.Tables[1].Copy();

                            if (MessageBox.Show("确认打印成品入库单吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                            {

                                DataRow drM = dt_打印1.Rows[0];

                                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
                                Type outerForm = outerAsm.GetType("ERPreport.生产成品入库", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统

                                if (dt_打印2.Columns.Contains("库位") == false)
                                {
                                    dt_打印2.Columns.Add("库位", typeof(string));
                                }
                                foreach (DataRow dri in dt_打印2.Rows)
                                {
                                    string sql = string.Format("select * from 仓库物料数量表  where 物料编码='{0}'", dri["物料编码"].ToString());
                                    using (SqlDataAdapter sdas = new SqlDataAdapter(sql, strcon))
                                    {
                                        dt_wl = new DataTable();
                                        da.Fill(dt_wl);


                                    }


                                    if (dt_wl.Rows.Count > 0)
                                    {
                                        dri["库位"] = dt_wl.Rows[0]["货架描述"].ToString();
                                    }
                                    dri["入库数量"] = decimal.Parse(dri["入库数量"].ToString()).ToString("0.######");

                                }

                                //  CPublic.UIcontrol.Showpage(ui, t.Rows[0]["打开界面名称"].ToString());
                                object[] drr = new object[2];

                                drr[0] = drM;
                                drr[1] = dt_打印2;


                                //   drr[2] = dr["出入库申请单号"].ToString();
                                Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                                //  UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
                                ui.ShowDialog();
                            }
                        }
                        catch (Exception ex)
                        {
                            ts.Rollback();
                            throw ex;
                        }

                        //刷新四个量了
                        fun_save_zf();
                        MessageBox.Show("生效完成");
                        barLargeButtonItem1_ItemClick(null, null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    barLargeButtonItem1_ItemClick(null, null);

                }
            }

        }
        #endregion
        //左边选择检验单 赋值到右边的入库明细 dt_明细
#pragma warning disable IDE1006 // 命名样式
        private void repositoryItemCheckEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow r = gv_检验单.GetDataRow(gv_检验单.FocusedRowHandle);
            if (e.NewValue.Equals(true))
            {

                DataRow rr = dt_明细.NewRow();

                rr["生产检验单号"] = r["生产检验单号"];
                rr["生产制令单号"] = r["生产制令单号"];
                rr["入库车间"] = r["生产车间"];
                rr["生产工单号"] = r["生产工单号"];
                rr["物料编码"] = r["物料编码"];
                rr["物料名称"] = r["物料名称"];
                rr["规格型号"] = r["规格型号"];
                rr["生产工单类型"] = r["生产工单类型"];
                if (r["生产工单类型"].ToString() == "返修工单")
                {
                    //20-5-11 如果是返修工单  只能从哪领的入哪个库 
                    string xx = $@"select   a.返修产品编码,a.申请单号,zl.生产工单号 ,a.审核,ck.仓库号,ck.仓库名称  from  新_返修申请主表 a
 left join 新_返修申请子表 b on a.申请单号=b.申请单号 and 返修产品编码=物料编码 
 left join ( select  c.*,zl.备注3 as fxdh from 生产记录生产工单表 c 
          left join   生产记录生产制令表 zl on c.生产制令单号 =zl.生产制令单号)zl on  fxdh=a.申请单号       
 left join (select  c.生产工单号,mx.物料编码,mx.仓库号,mx.仓库名称  from  仓库出入库明细表 mx 
			left join  生产记录生产工单表 c on c.生产工单号=mx.相关单号 
			where 明细类型 ='领料出库'  ) ck on ck.生产工单号=zl.生产工单号 and ck.物料编码 =b.物料编码 
 where  zl.生产工单号 ='{r["生产工单号"].ToString()}'";
                    DataTable t_temp = CZMaster.MasterSQL.Get_DataTable(xx, strcon);
                    if (t_temp.Rows.Count > 0)
                    {
                        rr["仓库号"] = t_temp.Rows[0]["仓库号"];
                        rr["仓库名称"] = t_temp.Rows[0]["仓库名称"];
                    }

                    else
                    {
                        rr["仓库号"] = r["仓库号"];
                        rr["仓库名称"] = r["仓库名称"];
                    }
                }
                else
                {
                    rr["仓库号"] = r["仓库号"];
                    rr["仓库名称"] = r["仓库名称"];
                }
                rr["工单负责人ID"] = r["负责人员ID"];
                rr["入库数量"] = Convert.ToDecimal(r["合格数量"]) - Convert.ToDecimal(r["已入库数量"]) + Convert.ToDecimal(r["重检合格数"]);
                // rr["原ERP物料编号"] = r["原ERP物料编号"];
                rr["合格数量"] = Convert.ToDecimal(r["合格数量"]); //   - Convert.ToDecimal(r["已入库数量"]) + Convert.ToDecimal(r["重检合格数"]);  17-11-15 不知道之前为什么写成这样  ，影响 工单 的完成状态 
                rr["重检合格数"] = Convert.ToDecimal(r["重检合格数"]);
                rr["已入库数量"] = r["已入库数量"]; // 检验单的 已入库数量
                                          //查询默认客户
                string sql_kh = string.Format(@"select  库存总数,货架描述  from  仓库物料数量表 where  物料编码='{0}' and 仓库号='{1}'", rr["物料编码"].ToString(), r["仓库号"]);
                using (SqlDataAdapter da = new SqlDataAdapter(sql_kh, strcon))
                {
                    DataTable temp = new DataTable();
                    da.Fill(temp);
                    if (temp.Rows.Count > 0)
                    {
                        //rr["客户ID"] = dt_kh.Rows[0]["客户编号"];
                        //rr["客户名称"] = dt_kh.Rows[0]["客户名称"];
                        rr["库存总数"] = Convert.ToDecimal(temp.Rows[0]["库存总数"]);
                        rr["货架描述"] = temp.Rows[0]["货架描述"].ToString();
                    }
                    else
                    {

                        rr["库存总数"] = 0;
                        rr["货架描述"] = "";
                    }
                }
                dt_明细.Rows.Add(rr);

            }
            else
            {
                dt_明细.Select(string.Format("生产检验单号='{0}'", r["生产检验单号"]))[0].Delete();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_检验单_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_检验单_KeyDown(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv_检验单.GetFocusedRowCellValue(gv_检验单.FocusedColumn));
                e.Handled = true;
            }
        }

        private void 完成ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv_检验单.GetDataRow(gv_检验单.FocusedRowHandle);
                frm成品入库关闭原因 frm = new frm成品入库关闭原因(dr);
                frm.ShowDialog();
                if (frm.flag)
                {
                    //if (MessageBox.Show(string.Format("确定不入库该工单剩下数量？"), "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    //{


                    string s = string.Format(@"select * from  [生产记录生产工单表]   where 生产工单号='{0}'", dr["生产工单号"].ToString());
                    DataTable dt_工单 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    dt_工单.Rows[0]["完成"] = true;
                    dt_工单.Rows[0]["完成日期"] = CPublic.Var.getDatetime();

                    //string sql = string.Format(@"update [生产记录生产工单表] set 完成=1,完成日期='{0}' where 生产工单号='{1}'"
                    //, Convert.ToDateTime(CPublic.Var.getDatetime().ToString("yyyy-MM-dd HH:mm:ss")), dr["生产工单号"].ToString());
                    //CZMaster.MasterSQL.ExecuteSQL(sql, strcon);
                    s = string.Format(@"select * from  [生产记录生产检验单主表]   where 生产检验单号='{0}'", dr["生产检验单号"].ToString());
                    DataTable dt_检验 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    dt_检验.Rows[0]["完成"] = true;
                    dt_检验.Rows[0]["完成日期"] = CPublic.Var.getDatetime();
                    dt_检验.Rows[0]["完成人员"] = CPublic.Var.localUserName;
                    dt_检验.Rows[0]["完成人员ID"] = CPublic.Var.LocalUserID;

                    //string sql_1 = string.Format(@"update [生产记录生产检验单主表] set 完成=1,完成日期='{0}',完成人员='{1}',完成人员ID='{2}' where 生产检验单号='{3}'"
                    //, Convert.ToDateTime(CPublic.Var.getDatetime().ToString("yyyy-MM-dd HH:mm:ss")), CPublic.Var.localUserName, CPublic.Var.LocalUserID, dr["生产检验单号"].ToString());
                    //CZMaster.MasterSQL.ExecuteSQL(sql_1, strcon);


                    //记录原因 
                    string sql_reason = "select  * from 生产入库完成关闭原因表 where 1<>1 ";
                    DataTable t_rn = CZMaster.MasterSQL.Get_DataTable(sql_reason, strcon);
                    DataRow r_rn = t_rn.NewRow();
                    r_rn["生产工单号"] = dr["生产工单号"];
                    r_rn["生产检验单号"] = dr["生产检验单号"];
                    r_rn["原因"] = frm.str;
                    r_rn["物料编码"] = dr["物料编码"];

                    r_rn["操作人"] = CPublic.Var.localUserName;
                    r_rn["操作时间"] = CPublic.Var.getDatetime();
                    t_rn.Rows.Add(r_rn);
                    SqlConnection conn = new SqlConnection(strcon);
                    conn.Open();
                    SqlTransaction st = conn.BeginTransaction("关闭成品入库"); //事务的名称
                    SqlCommand cmd = new SqlCommand("select * from 生产记录生产工单表  where 1<>1", conn, st);
                    SqlCommand cmd1 = new SqlCommand("select * from 生产记录生产检验单主表 where 1<>1", conn, st);

                    SqlCommand cmd2 = new SqlCommand(sql_reason, conn, st);


                    try
                    {
                        SqlDataAdapter da_1;
                        da_1 = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da_1);
                        da_1.Update(dt_工单);



                        da_1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da_1);
                        da_1.Update(dt_检验);


                        da_1 = new SqlDataAdapter(cmd2);
                        new SqlCommandBuilder(da_1);
                        da_1.Update(t_rn);
                        st.Commit();
                        StockCore.StockCorer.fun_物料数量_实际数量(dr["物料编码"].ToString(), dr["仓库号"].ToString(), true);
                        MessageBox.Show("ok");
                        barLargeButtonItem1_ItemClick(null, null);
                    }
                    catch (Exception ex)
                    {
                        st.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("失败了。" + ex.Message);

            }


        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_检验单_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc_检验单, new Point(e.X, e.Y));
                gv_检验单.CloseEditor();
                this.BindingContext[dt_检验单].EndCurrentEdit();

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

        private void gv_检验单_ColumnPositionChanged(object sender, EventArgs e)
        {
            try
            {
                if (cfgfilepath != "")
                {
                    gv_检验单.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void gv_检验单_ColumnWidthChanged(object sender, DevExpress.XtraGrid.Views.Base.ColumnEventArgs e)
        {
            try
            {
                if (cfgfilepath != "")
                {
                    gv_检验单.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //private void repositoryItemSearchLookUpEdit1View_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        //{
        //    DataRow sr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
        //    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
        //    dr["仓库号"] = sr["仓库号"].ToString();
        //    dr["仓库名称"] = sr["仓库名称"].ToString();
        //    string sql = "select * from 仓库物料数量表 where 物料编码 = '" + dr["物料编码"] + "' and 仓库号 = '" + dr["仓库号"] + "'";
        //    SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
        //    DataTable dt_物料数量 = new DataTable();
        //    da.Fill(dt_物料数量);
        //    if (dt_物料数量.Rows.Count == 0)
        //    {
        //        dr["库存总数"] = 0;
        //    }
        //    else
        //    {
        //        dr["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
        //    }

        //}



        //private void repositoryItemSearchLookUpEdit1View_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        //{
        //    DataRow sr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
        //    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
        //    dr["仓库号"] = sr["仓库号"].ToString();
        //    dr["仓库名称"] = sr["仓库名称"].ToString();
        //    string sql = "select * from 仓库物料数量表 where 物料编码 = '" + dr["物料编码"] + "' and 仓库号 = '" + dr["仓库号"] + "'";
        //    SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
        //    DataTable dt_物料数量 = new DataTable();
        //    da.Fill(dt_物料数量);
        //    if (dt_物料数量.Rows.Count == 0)
        //    {
        //        dr["库存总数"] = 0;
        //    }
        //    else
        //    {
        //        dr["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
        //    }
        //}

        private void gv_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            try
            {
                //if (e.Column.FieldName == "仓库号" && dr["生产工单类型"].ToString() == "返修工单")
                //{
                //    s = dr["仓库号"].ToString();
                //    throw new Exception("返修工单不可修改仓库");
                //}
                if (e.Column.FieldName == "仓库号")
                {
                    dr["仓库号"] = e.Value;
                    DataRow[] ds = dt_仓库号.Select(string.Format("仓库号 = '{0}'", dr["仓库号"]));
                    if (ds.Length == 0)
                    {
                        dr["仓库名称"] = "";
                        dr["库存总数"] = 0;

                    }
                    else
                    {
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //gv.SetRowCellValue(e.RowHandle,e.Column,s);

                // dr["仓库号"] = s;
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
                    gc_检验单.ExportToXlsx(saveFileDialog.FileName);
                    //ERPorg.Corg.TableToExcel(tt, saveFileDialog.FileName);
                    MessageBox.Show("导出成功");
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void 查看BOM明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv_检验单.GetDataRow(gv_检验单.FocusedRowHandle);
                decimal aa = 0;
                aa = Convert.ToDecimal(dr["送检数量"].ToString()) - Convert.ToDecimal(dr["已入库数量"].ToString());
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, "ERPproduct.dll"));
                Type outerForm = outerAsm.GetType("ERPproduct.UI物料BOM详细数量", false);

                object[] r = new object[2];
                r[0] = dr["物料编码"].ToString();
                r[1] = aa;
                UserControl ui = Activator.CreateInstance(outerForm, r) as UserControl;
                CPublic.UIcontrol.Showpage(ui, "物料BOM信息查询");

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow rr = gv.GetDataRow(gv.FocusedRowHandle);

            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip2.Show(gc, new Point(e.X, e.Y));

            }
        }

        private void 查询BOM信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                decimal aa = 0;
                aa = Convert.ToDecimal(dr["入库数量"].ToString());
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, "ERPproduct.dll"));
                Type outerForm = outerAsm.GetType("ERPproduct.UI物料BOM详细数量", false);

                object[] r = new object[2];
                r[0] = dr["物料编码"].ToString();
                r[1] = aa;
                UserControl ui = Activator.CreateInstance(outerForm, r) as UserControl;
                CPublic.UIcontrol.Showpage(ui, "物料BOM信息查询");

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void repositoryItemSearchLookUpEdit1_Popup(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (dr["生产工单类型"].ToString() == "返修工单")
                {
                    textBox3.Focus();
                    throw new Exception("返修工单不可修改仓库");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow drM = (this.BindingContext[gc_检验单.DataSource].Current as DataRowView).Row;
            DataTable dtm = (DataTable)this.gc.DataSource;
            Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
            Type outerForm = outerAsm.GetType("ERPreport.生产成品入库", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
                                                                          //  CPublic.UIcontrol.Showpage(ui, t.Rows[0]["打开界面名称"].ToString());
            object[] drr = new object[2];

            drr[0] = drM;
            drr[1] = dtm;
            //   drr[2] = dr["出入库申请单号"].ToString();
            Form ui = Activator.CreateInstance(outerForm, drr) as Form;
            //  UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
            ui.ShowDialog();






        }
    }
}
