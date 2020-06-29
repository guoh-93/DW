using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
namespace ERPStock
{
    public partial class ui采购退货出库 : UserControl
    {

        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dtM;
        DataTable dtP;
        DataTable dt_明细;
        DataTable dt_出入库明细;

        DataRow drM = null;
        DataTable dt_仓库;
        DataTable dt_人员;
        string sql_ck = "";
        DataTable dt_代办;
        #endregion


        public ui采购退货出库()
        {
            InitializeComponent();
        }

        private void ui采购退货出库_Load(object sender, EventArgs e)
        {
            try
            {
                dateEdit1.EditValue = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");
                fun_载入代办();

                fun_载入主表明细();
                fun_下拉框();
                gc.DataSource = dtP;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_下拉框()
        {
            dt_仓库 = new DataTable();
            string sql4 = "select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别'";
            SqlDataAdapter da = new SqlDataAdapter(sql4, strconn);
            da.Fill(dt_仓库);
            repositoryItemSearchLookUpEdit2.DataSource = dt_仓库;
            repositoryItemSearchLookUpEdit2.DisplayMember = "仓库号";
            repositoryItemSearchLookUpEdit2.ValueMember = "仓库号";
        }

        private void fun_载入代办()
        {
            sql_ck = string.Format("select * from 人员仓库对应表 where 工号='{0}'", CPublic.Var.LocalUserID);
            DataTable dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_ck, strconn);
            sql_ck = "and a.仓库号  in( ";

            string sql = "";
            if (dt_仓库.Rows.Count == 0 && CPublic.Var.LocalUserID == "admin")
            {
                sql = "select * from 采购退货申请主表 where 生效 = 1 and 完成 = 0 and 作废=0";
                //sql_ck = "";
            }
            else
            {
                foreach (DataRow dr in dt_仓库.Rows)
                {
                    sql_ck = sql_ck + string.Format("'{0}',", dr["仓库号"]);

                }
                sql_ck = sql_ck.Substring(0, sql_ck.Length - 1) + ")";

                sql = string.Format(@"select 采购退货申请主表.* from 采购退货申请主表 
                        where 生效 = 1 and  完成 = 0  and 作废=0 and
                      退货申请单号 in( select 退货申请单号  from 采购退货申请子表 a,基础数据物料信息表 base where 完成 =0 and  作废=0 
                       and   base.物料编码=a.物料编码 {0} group by 退货申请单号 ) ", sql_ck);

            }

            //   string sql = "select * from 采购退货申请主表 where 生效 = 1 and 完成 = 0";
            dt_代办 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_代办);
            gc_代办.DataSource = dt_代办;
        }

        private void fun_载入主表明细()
        {
            if (drM == null)
            {
                string sql = "select * from 采购退货出库主表 where 1<>1";
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                drM = dtM.NewRow();
                dtM.Rows.Add(drM);
                sql = @"select tz.*,kc.库存总数,货架描述 from 采购退货出库子表 tz
                       left join 仓库物料数量表 kc on tz.物料编码 = kc.物料编码
                          where 1<>1";
                dtP = new DataTable();
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtP);
            }
            else
            {
                string sql = string.Format("select * from 采购退货出库主表 where 退货出库主表 = '{0}'", drM["退货入库单号"].ToString());
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                drM = dtM.Rows[0];
                string sql2 = string.Format(@"select tz.*,kc.库存总数 from 采购退货出库子表 tz
                   left join 仓库物料数量表 kc on tz.物料编码 = kc.物料编码 and kc.仓库号=tz.仓库号
                   where 退货出库单号 = '{0}'", drM["退货出库单号"].ToString());
                dtP = new DataTable();
                SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
                da2.Fill(dtP);

            }

        }


        private void fun_save()
        {
            try
            {
                //主表
                DateTime t = CPublic.Var.getDatetime();


                txt_出库单号.Text = string.Format("PR{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("PR", t.Year, t.Month).ToString("0000"));
                drM["退货出库单号"] = txt_出库单号.Text;
                drM["日期"] = t;
                drM["操作人ID"] = CPublic.Var.LocalUserID;
                drM["操作人"] = CPublic.Var.localUserName;
                drM["退货申请单号"] = textBox1.Text;
                drM["供应商ID"] = textBox2.Text;
                drM["供应商名称"] = txt_供应商.Text;
                // 明细表
                string sql = "select * from 采购退货出库子表 where 1<>1";
                dt_明细 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                sql = "select * from 仓库出入库明细表 where 1<>1";
                dt_出入库明细 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                //生成负的出库记录 并标记出为退货的 
                sql = "select * from [采购记录采购单入库明细]  where 1<>1";
                DataTable dt_frk = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                sql = "select * from [采购记录采购单入库主表]  where 1<>1";
                DataTable dt_rkz = CZMaster.MasterSQL.Get_DataTable(sql, strconn);


                sql = "select * from [采购记录采购单明细表]  where 1<>1";
                DataTable dt_cgmx = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                sql = "select * from [其他出入库申请主表]  where 1<>1";
                DataTable t_qcmain = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                sql = "select * from [其他出入库申请子表]  where 1<>1";
                DataTable t_qcmx = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                sql = "select * from 其他出库主表 where 1<>1";
                DataTable  dt_材料出库主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                sql = "select * from 其他出库子表 where 1<>1";
                DataTable dt_材料出库子 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
 
                DataRow dr_fz = dt_rkz.NewRow();
                dr_fz["GUID"] = System.Guid.NewGuid();
                string str_入库单号 = string.Format("PC{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                CPublic.CNo.fun_得到最大流水号("PC", t.Year, t.Month));
                dr_fz["入库单号"] = str_入库单号;
                dr_fz["供应商ID"] = textBox2.Text;
                dr_fz["供应商"] = txt_供应商.Text;
                dr_fz["修改日期"] = dr_fz["录入日期"] = dr_fz["创建日期"] = t;
                dr_fz["操作员ID"] = CPublic.Var.LocalUserID;
                dr_fz["操作员"] = CPublic.Var.localUserName;
                dr_fz["生效"] = true;
                dr_fz["备注1"] = "采购退货";
                dt_rkz.Rows.Add(dr_fz);
                int i = 1;
                int i_cl = 1;
                if (bl_委外退)
                {
                    //有委外采购的退货  一个负（委外退料） 一个正 委外补料
                    DataRow r_qcmain_hz = t_qcmain.NewRow();
                    r_qcmain_hz["GUID"] = System.Guid.NewGuid();

                    r_qcmain_hz["出入库申请单号"] = str_材料申请_红字;

                    r_qcmain_hz["申请日期"] = t;
                    r_qcmain_hz["操作人员编号"] = "";
                    r_qcmain_hz["操作人员"] ="系统自动生成";
                    r_qcmain_hz["生效"] = true;
                    r_qcmain_hz["审核"] = true;
                    r_qcmain_hz["完成"] = true;
                    r_qcmain_hz["完成日期"] = t;
                    r_qcmain_hz["审核日期"] = t;
                    r_qcmain_hz["待审核"] = true;
                    r_qcmain_hz["生效日期"] = t;
                    r_qcmain_hz["备注"] = "委外采购退货自动生成";//关联采购单号
                    r_qcmain_hz["申请类型"] = "材料出库";
                    r_qcmain_hz["单据类型"] = "材料出库";
                    r_qcmain_hz["原因分类"] = "委外退料";
                    r_qcmain_hz["红字回冲"] = true;
                    t_qcmain.Rows.Add(r_qcmain_hz);

                    DataRow dr_材料出库_hz = dt_材料出库主.NewRow();
                    dt_材料出库主.Rows.Add(dr_材料出库_hz);
                    dr_材料出库_hz["GUID"] = System.Guid.NewGuid();
                    dr_材料出库_hz["其他出库单号"] = str_材料出_红字;
                    dr_材料出库_hz["出库类型"] = "材料出库";
                    //dr_材料出库主["操作人员编号"] = CPublic.Var.LocalUserID;
                    dr_材料出库_hz["操作人员"] = "系统自动生成";
                    dr_材料出库_hz["出库日期"] = t;
                    dr_材料出库_hz["生效"] = true;
                    dr_材料出库_hz["生效日期"] = t;
                    dr_材料出库_hz["创建日期"] = t;
                    dr_材料出库_hz["出入库申请单号"] = str_材料申请_红字;

                    DataRow r_qcmain = t_qcmain.NewRow();
                    r_qcmain["GUID"] = System.Guid.NewGuid();
                    r_qcmain["出入库申请单号"] = str_材料申请;
                    r_qcmain["申请日期"] = t;
                    r_qcmain["操作人员编号"] = "";
                    r_qcmain["操作人员"] = "系统自动生成";
                    r_qcmain["生效"] = true;
                    r_qcmain["审核"] = true;
                    r_qcmain["完成"] = true;
                    r_qcmain["完成日期"] = t;
                    r_qcmain["审核日期"] = t;
                    r_qcmain["待审核"] = true;
                    r_qcmain["生效日期"] = t;
                    r_qcmain["备注"] = "委外采购退货自动生成";//关联采购单号
                    r_qcmain["申请类型"] = "材料出库";
                    r_qcmain["单据类型"] = "材料出库";
                    r_qcmain["原因分类"] = "委外补料";
                    r_qcmain["红字回冲"] = false;
                    t_qcmain.Rows.Add(r_qcmain);

                    DataRow dr_材料出库主 = dt_材料出库主.NewRow();
                    dt_材料出库主.Rows.Add(dr_材料出库主);
                    dr_材料出库主["GUID"] = System.Guid.NewGuid();
                    dr_材料出库主["其他出库单号"] = str_材料出;
                    dr_材料出库主["出库类型"] = "材料出库";
                    //dr_材料出库主["操作人员编号"] = CPublic.Var.LocalUserID;
                    dr_材料出库主["操作人员"] = "系统自动生成";
                    dr_材料出库主["出库日期"] = t;
                    dr_材料出库主["生效"] = true;
                    dr_材料出库主["生效日期"] = t;
                    dr_材料出库主["创建日期"] = t;
                    dr_材料出库主["出入库申请单号"] = str_材料申请;
                }

                foreach (DataRow rt in dtP.Rows)
                {
                    if (rt["选择"].Equals(true))
                    {
                        rt["完成"] = 1;
                        rt["完成日期"] = t;
                        #region  退货出库明细
                        DataRow r = dt_明细.NewRow();
                        r["退货出库单号"] = txt_出库单号.Text;
                        r["退货出库明细号"] = txt_出库单号.Text + "-" + i.ToString("00");
                        r["物料编码"] = rt["物料编码"];
                        // r["原ERP物料编号"] = rt["原ERP物料编号"];
                        r["数量"] = rt["数量"];
                        r["物料名称"] = rt["物料名称"];
                        r["规格型号"] = rt["规格型号"];
                        r["日期"] = t;
                        r["采购明细号"] = rt["采购明细"];
                        r["退货申请明细号"] = rt["退货申请明细号"];
                        dt_明细.Rows.Add(r);
                        #endregion
                        #region  仓库出入库明细
                        DataRow r_mx = dt_出入库明细.NewRow();
                        r_mx["单号"] = txt_出库单号.Text;
                        r_mx["GUID"] = System.Guid.NewGuid();
                        r_mx["明细类型"] = "采购退货";

                        r_mx["出库入库"] = "入库";
                        r_mx["物料编码"] = rt["物料编码"];
                        r_mx["物料名称"] = rt["物料名称"];
                        //r["BOM版本"] = dr["BOM版本"];
                        r_mx["明细号"] = txt_出库单号.Text + "-" + i.ToString("00"); ;
                        r_mx["相关单号"] = rt["采购明细"];
                        r_mx["仓库号"] = rt["仓库号"];
                        r_mx["仓库名称"] = rt["仓库名称"];

                        r_mx["相关单位"] = txt_供应商.Text;
                        r_mx["实效数量"] = -Convert.ToDecimal(rt["数量"]);
                        r_mx["实效时间"] = t;
                        r_mx["出入库时间"] = t;
                        r_mx["盘点有效批次号"] = "初始化";
                        r_mx["仓库人"] = CPublic.Var.localUserName;
                        dt_出入库明细.Rows.Add(r_mx);
                        #endregion
                        #region 生成负的采购入库明细
                        DataRow rr = dt_frk.NewRow();
                        rr["GUID"] = System.Guid.NewGuid();
                        rr["入库单号"] = str_入库单号;
                        rr["入库明细号"] = str_入库单号 + "-" + i.ToString("00");
                        rr["入库POS"] = i;
                        if (rt["采购明细"].ToString().Contains("-"))
                        {
                            rr["采购单号"] = rt["采购明细"].ToString().Split('-')[0];
                        }
                        rr["采购单明细号"] = rt["采购明细"].ToString();
                        rr["物料编码"] = rt["物料编码"];
                        rr["物料名称"] = rt["物料名称"];
                        string ss = string.Format("select * from [采购退货申请子表] where  退货申请单号='{0}' and 采购明细='{1}'"
                        , textBox1.Text, rt["采购明细"].ToString());
                        using (SqlDataAdapter da = new SqlDataAdapter(ss, strconn))
                        {
                            DataTable temp = new DataTable();
                            da.Fill(temp);
                            rr["图纸编号"] = temp.Rows[0]["图纸编号"];
                            rr["未税单价"] = temp.Rows[0]["不含税单价"];
                            rr["单价"] = temp.Rows[0]["含税单价"];
                            rr["税率"] = temp.Rows[0]["税率"];
                            rr["未税金额"] = temp.Rows[0]["不含税金额"];
                            rr["金额"] = temp.Rows[0]["含税金额"];
                        }
                        rr["供应商ID"] = textBox2.Text;
                        rr["生效"] = true;
                        rr["录入日期"] = rr["生效日期"] = t;
                        rr["供应商"] = txt_供应商.Text;
                        rr["操作员ID"] = rr["生效人员ID"] = rr["入库人员ID"] = CPublic.Var.LocalUserID;
                        rr["操作员"] = rr["生效人员"] = rr["入库人员"] = CPublic.Var.localUserName;
                        rr["入库量"] = -Convert.ToDecimal(rt["数量"]);
                        rr["备注1"] = "采购退货";
                        //19-12-4新增
                        rr["备注2"] = rt["退货申请明细号"];
                        //20-4-9 22：48 前期采购退货 没有采购单 暂估价要存起来根据规则就放到备注6里面  开票单价不一样 成本核算会出问题
                        rr["备注6"] = rr["未税单价"].ToString();

                        rr["仓库ID"] = rt["仓库号"];
                        rr["仓库名称"] = rt["仓库名称"];
                        rr["价格核实"] = true;
                        dt_frk.Rows.Add(rr);
                        i++;
                        #endregion
                        //如果是委外的 自动生成 出库 和 入库
                        #region 委外 自动生成 退料 再发料  
                   
                        if (rt["采购单类型"].ToString() == "委外采购")
                        {
                            string x = $@"select  * from 其他出入库申请子表 a 
                                left join 其他出入库申请主表 b on a.出入库申请单号 = b.出入库申请单号
                                 where a.备注 = '{rt["采购明细"].ToString()}' and 原因分类 = '委外加工'";
                            //这里取他原来的 材料出库记录  不直接从bom取 bom可能会更改
                            DataTable tem = CZMaster.MasterSQL.Get_DataTable(x, strconn);
                            foreach (DataRow ww_r in tem.Rows)
                            {
                                #region 红字
                                DataRow dr_材料申请子 = t_qcmx .NewRow();
                                t_qcmx.Rows.Add(dr_材料申请子);
                                dr_材料申请子["GUID"] = System.Guid.NewGuid();
                                dr_材料申请子["出入库申请单号"] = str_材料申请_红字;
                                dr_材料申请子["POS"] = i_cl;
                                dr_材料申请子["出入库申请明细号"] = str_材料申请_红字 + "-" + i_cl.ToString("00");
                                dr_材料申请子["物料编码"] = ww_r["物料编码"];
                                dr_材料申请子["规格型号"] = ww_r["规格型号"];
                                dr_材料申请子["物料名称"] = ww_r["物料名称"];
                                dr_材料申请子["数量"]  =- Convert.ToDecimal(ww_r["委外bom数量"])  * Convert.ToDecimal(rt["数量"]);// bom数量* 退货数量
                                dr_材料申请子["已完成数量"] = Convert.ToDecimal(dr_材料申请子["数量"]);
                                dr_材料申请子["备注"] = rt["采购明细"].ToString();  
                                dr_材料申请子["生效"] = true;
                                dr_材料申请子["生效日期"] = t;
                                dr_材料申请子["生效人员编号"] = "系统自动生成";
                                dr_材料申请子["完成"] = true;
                                dr_材料申请子["完成日期"] = t;
                                dr_材料申请子["仓库号"] = ww_r["仓库号"];
                                dr_材料申请子["仓库名称"] = ww_r["仓库名称"];
                                dr_材料申请子["货架描述"] = ww_r["货架描述"];
                                dr_材料申请子["委外备注1"] = ww_r["委外备注1"].ToString();
                                dr_材料申请子["委外备注2"] = ww_r["委外备注2"].ToString();
                                dr_材料申请子["委外bom数量"] = ww_r["委外bom数量"].ToString();

                                DataRow dr_材料出库子 = dt_材料出库子.NewRow();
                                dt_材料出库子.Rows.Add(dr_材料出库子);
                                dr_材料出库子["物料编码"] = ww_r["物料编码"]; ;
                                //dr_其他出库子["原ERP物料编号"] = dr_借用明细["原ERP物料编号"];
                                dr_材料出库子["物料名称"] = ww_r["物料名称"];
                                dr_材料出库子["数量"] = Convert.ToDecimal(dr_材料申请子["数量"]);

                                dr_材料出库子["规格型号"] = ww_r["规格型号"];
                                // dr_其他出库子["图纸编号"] = rr["图纸编号"];
                                dr_材料出库子["其他出库单号"] = str_材料出_红字;
                                dr_材料出库子["POS"] = i_cl;
                                dr_材料出库子["其他出库明细号"] = str_材料出_红字 + "-" + i_cl.ToString("00");
                                dr_材料出库子["GUID"] = System.Guid.NewGuid();
                                dr_材料出库子["备注"] = rt["采购明细"].ToString();
                                dr_材料出库子["生效"] = true;
                                dr_材料出库子["生效日期"] = t;
                                dr_材料出库子["生效人员编号"] = "系统自动生成";
                                dr_材料出库子["完成"] = true;
                                dr_材料出库子["完成日期"] = t;
                                dr_材料出库子["完成人员编号"] = "系统自动生成";
                                dr_材料出库子["出入库申请单号"] = str_材料申请_红字;
                                dr_材料出库子["出入库申请明细号"] = dr_材料申请子["出入库申请明细号"];
                           
                                DataRow dr_出入库 = dt_出入库明细.NewRow();
                                dt_出入库明细.Rows.Add(dr_出入库);
                                dr_出入库["GUID"] = System.Guid.NewGuid();
                                dr_出入库["明细类型"] = "材料出库";
                                dr_出入库["单号"] = str_材料出_红字;
                                dr_出入库["出库入库"] = "出库";
                                dr_出入库["物料编码"] = ww_r["物料编码"];
                                dr_出入库["物料名称"] = ww_r["物料名称"];
                                dr_出入库["仓库号"] = ww_r["仓库号"];
                                dr_出入库["仓库名称"] = ww_r["仓库名称"];
                                dr_出入库["明细号"] = dr_材料出库子["其他出库明细号"];
                                dr_出入库["相关单号"] = str_材料申请_红字;
                                dr_出入库["实效数量"] = -(Convert.ToDecimal(dr_材料出库子["数量"]));
                                dr_出入库["实效时间"] = t;
                                dr_出入库["出入库时间"] = t;
           
                                #endregion
                                #region  补料
                                DataRow dr_材料申请子_补 = t_qcmx.NewRow();
                                t_qcmx.Rows.Add(dr_材料申请子_补);
                                dr_材料申请子_补["GUID"] = System.Guid.NewGuid();
                                dr_材料申请子_补["出入库申请单号"] = str_材料申请;
                                dr_材料申请子_补["POS"] = i_cl;
                                dr_材料申请子_补["出入库申请明细号"] = str_材料申请 + "-" + i_cl.ToString("00");
                                dr_材料申请子_补["物料编码"] = ww_r["物料编码"];
                                dr_材料申请子_补["规格型号"] = ww_r["规格型号"];
                                dr_材料申请子_补["物料名称"] = ww_r["物料名称"];
                                dr_材料申请子_补["数量"] = Convert.ToDecimal(ww_r["委外bom数量"]) * Convert.ToDecimal(rt["数量"]);// bom数量* 退货数量
                                dr_材料申请子_补["已完成数量"] = Convert.ToDecimal(dr_材料申请子_补["数量"]);

                                dr_材料申请子_补["备注"] = rt["采购明细"].ToString();

                                dr_材料申请子_补["生效"] = true;
                                dr_材料申请子_补["生效日期"] = t;
                                dr_材料申请子_补["生效人员编号"] = "系统自动生成";
                                dr_材料申请子_补["完成"] = true;
                                dr_材料申请子_补["完成日期"] = t;
                                dr_材料申请子_补["仓库号"] = ww_r["仓库号"];
                                dr_材料申请子_补["仓库名称"] = ww_r["仓库名称"];
                                dr_材料申请子_补["货架描述"] = ww_r["货架描述"];
                                dr_材料申请子_补["委外备注1"] = ww_r["委外备注1"].ToString();
                                dr_材料申请子_补["委外备注2"] = ww_r["委外备注2"].ToString();
                                dr_材料申请子_补["委外bom数量"] = ww_r["委外bom数量"].ToString();

                                DataRow dr_材料出库子_补 = dt_材料出库子.NewRow();
                                dt_材料出库子.Rows.Add(dr_材料出库子_补);
                                dr_材料出库子_补["物料编码"] = ww_r["物料编码"]; ;
                                //dr_其他出库子["原ERP物料编号"] = dr_借用明细["原ERP物料编号"];
                                dr_材料出库子_补["物料名称"] = ww_r["物料名称"];
                                dr_材料出库子_补["数量"] = Convert.ToDecimal(dr_材料申请子_补["数量"]);

                                dr_材料出库子_补["规格型号"] = ww_r["规格型号"];
                                // dr_其他出库子["图纸编号"] = rr["图纸编号"];
                                dr_材料出库子_补["其他出库单号"] = str_材料出;
                                dr_材料出库子_补["POS"] = i_cl;
                                dr_材料出库子_补["其他出库明细号"] = str_材料出+ "-" + i_cl.ToString("00");
                                dr_材料出库子_补["GUID"] = System.Guid.NewGuid();
                                dr_材料出库子_补["备注"] = rt["采购明细"].ToString();
                                dr_材料出库子_补["生效"] = true;
                                dr_材料出库子_补["生效日期"] = t;
                                dr_材料出库子_补["生效人员编号"] = "系统自动生成";
                                dr_材料出库子_补["完成"] = true;
                        


                                dr_材料出库子_补["完成日期"] = t;
                                dr_材料出库子_补["完成人员编号"] = "系统自动生成";
                                dr_材料出库子_补["出入库申请单号"] = str_材料申请;
                                dr_材料出库子_补["出入库申请明细号"] = dr_材料申请子_补["出入库申请明细号"];

                                DataRow dr_出入库_1 = dt_出入库明细.NewRow();
                                dt_出入库明细.Rows.Add(dr_出入库_1);
                                dr_出入库_1["GUID"] = System.Guid.NewGuid();
                                dr_出入库_1["明细类型"] = "材料出库";
                                dr_出入库_1["单号"] = str_材料出;
                                dr_出入库_1["出库入库"] = "出库";
                                dr_出入库_1["物料编码"] = ww_r["物料编码"];
                                dr_出入库_1["物料名称"] = ww_r["物料名称"];
                                dr_出入库_1["仓库号"] = ww_r["仓库号"];
                                dr_出入库_1["仓库名称"] = ww_r["仓库名称"];
                                dr_出入库_1["明细号"] = dr_材料出库子_补["其他出库明细号"];
                                dr_出入库_1["相关单号"] = str_材料申请;
                                dr_出入库_1["实效数量"] = -(Convert.ToDecimal(dr_材料出库子_补["数量"]));
                                dr_出入库_1["实效时间"] = t;
                                dr_出入库_1["出入库时间"] = t;
                                i_cl++;  
                                #endregion
                            }

                            //20-5-18  还要返回来料送检里面  即 采购明细 送检完成 =0 ,未送检数量要扣减
                            x = $@"select * from 采购记录采购单明细表 where 采购明细号='{rt["采购明细"].ToString()}'";
                            using (SqlDataAdapter da = new SqlDataAdapter(x, strconn))
                            {
                                da.Fill(dt_cgmx);
                            }
                            dt_cgmx.Rows[dt_cgmx.Rows.Count - 1]["明细完成"] = 0;
                            dt_cgmx.Rows[dt_cgmx.Rows.Count - 1]["明细完成日期"] = DBNull.Value ;
                            dt_cgmx.Rows[dt_cgmx.Rows.Count - 1]["总完成日期"] = DBNull.Value;
                            dt_cgmx.Rows[dt_cgmx.Rows.Count - 1]["总完成"] = 0;
                            dt_cgmx.Rows[dt_cgmx.Rows.Count - 1]["已送检数"] = Convert.ToDecimal(dt_cgmx.Rows[dt_cgmx.Rows.Count - 1]["已送检数"]) - Convert.ToDecimal(rt["数量"]);
                            dt_cgmx.Rows[dt_cgmx.Rows.Count - 1]["完成数量"] = Convert.ToDecimal(dt_cgmx.Rows[dt_cgmx.Rows.Count - 1]["完成数量"]) - Convert.ToDecimal(rt["数量"]);
                            dt_cgmx.Rows[dt_cgmx.Rows.Count - 1]["未完成数量"] = Convert.ToDecimal(dt_cgmx.Rows[dt_cgmx.Rows.Count - 1]["未完成数量"])+ Convert.ToDecimal(rt["数量"]);
                        }
                        #endregion
                    }
                }
                DataTable dt_库存 = ERPorg.Corg.fun_库存(-1, dtP);
                DataTable dt_完成状态 = fun_完成状态();//申请主表
                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction cgth = conn.BeginTransaction("采购退货");
                string sql1 = "select * from 采购退货出库主表 where 1<>1";
                SqlCommand cmd1 = new SqlCommand(sql1, conn, cgth);
                SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                string sql2 = "select * from 采购退货出库子表 where 1<>1";
                SqlCommand cmd2 = new SqlCommand(sql2, conn, cgth);
                SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                new SqlCommandBuilder(da2);
                string sql3 = "select * from [采购退货申请主表] where 1<>1";
                SqlCommand cmd3 = new SqlCommand(sql3, conn, cgth);
                SqlDataAdapter da3 = new SqlDataAdapter(cmd3);
                new SqlCommandBuilder(da3);
                string sql4 = "select * from [采购退货申请子表] where 1<>1";
                SqlCommand cmd4 = new SqlCommand(sql4, conn, cgth);
                SqlDataAdapter da4 = new SqlDataAdapter(cmd4);
                new SqlCommandBuilder(da4);
                string sql5 = "select * from [仓库出入库明细表] where 1<>1";
                SqlCommand cmd5 = new SqlCommand(sql5, conn, cgth);
                SqlDataAdapter da5 = new SqlDataAdapter(cmd5);
                new SqlCommandBuilder(da5);
                string sql6 = "select * from [采购记录采购单入库主表] where 1<>1";
                SqlCommand cmd6 = new SqlCommand(sql6, conn, cgth);
                SqlDataAdapter da6 = new SqlDataAdapter(cmd6);
                new SqlCommandBuilder(da6);
                string sql7 = "select * from [采购记录采购单入库明细] where 1<>1";
                SqlCommand cmd7 = new SqlCommand(sql7, conn, cgth);
                SqlDataAdapter da7 = new SqlDataAdapter(cmd7);
                new SqlCommandBuilder(da7);
                string sql8 = "select * from [仓库物料数量表] where 1<>1";
                SqlCommand cmd8 = new SqlCommand(sql8, conn, cgth);
                SqlDataAdapter da8 = new SqlDataAdapter(cmd8);
                new SqlCommandBuilder(da8);
                try
                {
                    da1.Update(dtM);
                    da2.Update(dt_明细);
                    da3.Update(dt_完成状态);
                    da4.Update(dtP);
                    da5.Update(dt_出入库明细);
                    da6.Update(dt_rkz);
                    da7.Update(dt_frk);
                    da8.Update(dt_库存);

                    if(bl_委外退)
                    {
                        sql8 = "select * from [采购记录采购单明细表] where 1<>1";
                        cmd8 = new SqlCommand(sql8, conn, cgth);
                        da8 = new SqlDataAdapter(cmd8);
                        new SqlCommandBuilder(da8);
                        da8.Update(dt_cgmx);

                        sql8 = "select * from [其他出入库申请主表] where 1<>1";
                        cmd8 = new SqlCommand(sql8, conn, cgth);
                        da8 = new SqlDataAdapter(cmd8);
                        new SqlCommandBuilder(da8);
                        da8.Update(t_qcmain);

                        sql8 = "select * from [其他出入库申请子表] where 1<>1";
                        cmd8 = new SqlCommand(sql8, conn, cgth);
                        da8 = new SqlDataAdapter(cmd8);
                        new SqlCommandBuilder(da8);
                        da8.Update(t_qcmx);

                        sql8 = "select * from 其他出库主表 where 1<>1";
                        cmd8 = new SqlCommand(sql8, conn, cgth);
                        da8 = new SqlDataAdapter(cmd8);
                        new SqlCommandBuilder(da8);
                        da8.Update(dt_材料出库主);

                        sql8 = "select * from 其他出库子表 where 1<>1";
                        cmd8 = new SqlCommand(sql8, conn, cgth);
                        da8 = new SqlDataAdapter(cmd8);
                        new SqlCommandBuilder(da8);
                        da8.Update(dt_材料出库子);

                    }

                    cgth.Commit();
                }
                catch (Exception ex)
                {
                    cgth.Rollback();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_载入代办();

            drM = null;
            txt_出库单号.Text = "";
            txt_备注.Text = "";
            fun_载入主表明细();
            gc.DataSource = dtP;
        }
        private DataTable fun_完成状态()
        {
            DateTime t = CPublic.Var.getDatetime();
            string str_条件 = "";
            bool bl = true;

            DataTable dt_返回 = new DataTable();
            foreach (DataRow dr in dtP.Rows)
            {
                if (dr["选择"].Equals(true))
                {
                    str_条件 = str_条件 + ",'" + dr["物料编码"].ToString() + "'";
                    dr["完成"] = true;
                    dr["完成日期"] = t;
                }
                else
                {
                    bl = false;
                }
            }
            if (str_条件.Length > 1)
            {
                str_条件 = str_条件.Substring(1, str_条件.Length - 1);
                str_条件 = string.Format("and 物料编码 not in ({0})", str_条件);
            }
            string sql_MX = string.Format("select * from 采购退货申请子表  where 退货申请单号='{0}' and 完成=0  {1}", textBox1.Text, str_条件);

            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql_MX, strconn);
            if (dt.Rows.Count > 0)
            {
                bl = false;

            }
            if (bl)
            {

                //string sql = string.Format("select * from  生产记录生产工单待领料主表 where 生产工单号='{0}' ", textBox2.Text);
                string sql = string.Format("select * from  [采购退货申请主表] where 退货申请单号='{0}' ", textBox1.Text);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {

                    da.Fill(dt_返回);
                    if (dt_返回.Rows.Count > 0)
                    {
                        dt_返回.Rows[0]["完成"] = true;
                        dt_返回.Rows[0]["完成日期"] = t;
                    }

                }
            }

            return dt_返回;
        }
        //private  DataTable fun_库存(int i_正负, DataTable T)
        //{
        //    DataTable dt = new DataTable();
        //    foreach (DataRow dr in T.Rows)
        //    {
        //        if (dr["选择"].Equals(true))
        //        {
        //            string sql = string.Format("select * from 仓库物料数量表 where 物料编码='{0}'", dr["物料编码"].ToString());
        //            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
        //            {
        //                da.Fill(dt);
        //            }
        //            DataRow[] x = dt.Select(string.Format("物料编码='{0}'", dr["物料编码"].ToString()));
        //            x[0]["库存总数"] = Convert.ToDecimal(x[0]["库存总数"]) + i_正负 * Convert.ToDecimal(dr["数量"].ToString());

        //            x[0]["有效总数"] = Convert.ToDecimal(x[0]["有效总数"]) + i_正负 * Convert.ToDecimal(dr["数量"].ToString());

        //            x[0]["出入库时间"] = CPublic.Var.getDatetime();

        //        }

        //    }

        //    return dt;
        //}

        //2020-4-29 委外采购退货自动生成 退料和补料 一正一负 
        string str_材料申请 = "";
        string str_材料申请_红字 = "";
        string str_材料出 = "";
        string str_材料出_红字 = "";
        bool bl_委外退 = false;  //标记是否有委外退货
        private void fun_check()
        {
            DataView dv = new DataView(dtP);
            dv.RowFilter = "选择=1";
            if (dv.Count == 0)
            {
                throw new Exception("未选择任何明细");
            }
     
            foreach (DataRow dr in dv.ToTable().Rows)
            {
                if (Convert.ToDecimal(dr["库存总数"]) < Convert.ToDecimal(dr["数量"]))
                {
                    throw new Exception("选中明细中有带退货记录库存不足");
                }
                if(dr["采购单类型"].ToString()=="委外采购")
                {
                    bl_委外退 = true;
                }
            }
            DateTime t = CPublic.Var.getDatetime();
            if(bl_委外退)
            {
                str_材料申请 = string.Format("DWLS{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                          t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("DWLS", t.Year, t.Month).ToString("0000"));
                str_材料申请_红字 = string.Format("DWLS{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                        t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("DWLS", t.Year, t.Month).ToString("0000"));


                str_材料出 = string.Format("LS{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                           t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("LS", t.Year, t.Month).ToString("0000"));

                str_材料出_红字 = string.Format("LS{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
               t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("LS", t.Year, t.Month).ToString("0000"));
            }
        }



        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确认生效吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    gv.CloseEditor();
                    this.BindingContext[dtP].EndCurrentEdit();
                    fun_check();
                    fun_save();
                    MessageBox.Show("生效成功");
                    barLargeButtonItem1_ItemClick(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_代办_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr_退货申请 = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);
                txt_备注.Text = "";
                txt_备注.Text = dr_退货申请["备注"].ToString();
                textBox1.Text = dr_退货申请["退货申请单号"].ToString();
                txt_供应商.Text = dr_退货申请["供应商名称"].ToString();
                textBox2.Text = dr_退货申请["供应商编号"].ToString();

                //dtP.Clear();
                string sql = string.Format(@"select a.*,isnull(库存总数,0)库存总数,货架描述,isnull(采购单类型,'') 采购单类型  from 采购退货申请子表 a 
                left join 仓库物料数量表 b  on  a.物料编码=b.物料编码 and a.仓库号=b.仓库号   
                left join 采购记录采购单明细表 mx on mx.采购明细号=a.采购明细
                left join 采购记录采购单主表 zb on zb.采购单号=mx.采购单号     
                where  a.退货申请单号 = '{0}' and a.完成=0 {1}", dr_退货申请["退货申请单号"], sql_ck);
                dtP = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtP);
                dtP.Columns.Add("选择", typeof(bool));

                gc.DataSource = dtP;

                //foreach (DataRow dr in dtP.Rows)
                //{
                //    //20-3-23默认不合格1
                //    dr["仓库名称"] = "不合格品1";
                //    dr["仓库号"] = "08";
                //}
                //foreach (DataRow r in dt_退货申请.Rows)
                //{

                //    DataRow rr = dtP.NewRow();

                //    dtP.Rows.Add(rr);


                //    rr["物料编码"] = r["物料编码"];
                //    rr["物料名称"] = r["物料名称"];
                //    rr["n原ERP规格型号"] = r["n原ERP规格型号"];
                //    rr["数量"] = r["数量"];
                //    rr["原ERP物料编号"] = r["原ERP物料编号"];
                //    rr["退货申请单号"] = r["退货申请单号"];
                //    rr["退货申请明细号"] = r["退货申请明细号"];
                //    if (r["已入库数量"].ToString() == "") r["已入库数量"] = 0;
                //    rr["实际数量"] = Convert.ToDecimal(r["数量"]) - Convert.ToDecimal(r["已入库数量"]);
                //    rr["可入库数量"] = Convert.ToDecimal(r["数量"]) - Convert.ToDecimal(r["已入库数量"]);
                //    rr["税后单价"] = r["税后单价"];
                //    rr["税后金额"] = r["税后金额"];

                //}
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

        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
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
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                    gc_代办.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);

            }
        }
    }
}
