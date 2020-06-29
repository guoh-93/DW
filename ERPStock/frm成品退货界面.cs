using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace ERPStock
{
    public partial class frm成品退货界面 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dtM;
        DataTable dtP;
        DataRow drM = null;
        DataTable dt_物料;
        DataTable dt_人员;
        DataTable dt_代办;
        DataTable dt_仓库;
        #endregion

        public frm成品退货界面()
        {
            InitializeComponent();
        }

        public frm成品退货界面(DataRow dr)
        {
            InitializeComponent();
            drM = dr;
        }

        private void frm成品退货界面_Load(object sender, EventArgs e)
        {
            try
            {
                time_入库日期.EditValue = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");
                fun_载入主表明细();               
                fun_物料下拉框();
                fun_载入代办();
                gc.DataSource = dtP;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region 方法
        private void fun_载入主表明细()
        {
            if (drM == null)
            {
                string sql = "select * from 退货入库主表 where 1<>1";
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);

                drM = dtM.NewRow();
                dtM.Rows.Add(drM);

                sql = @"select 退货入库子表.*,kc.库存总数,kc.货架描述,出库明细号,通知单明细号,销售明细号 from 退货入库子表 
    left join 仓库物料数量表 kc on 退货入库子表.物料编码 = kc.物料编码 and  退货入库子表.仓库号=kc.仓库号
    left join 基础数据物料信息表 on   基础数据物料信息表.物料编码 = kc.物料编码
    left join 退货申请子表 on 退货申请子表.退货申请明细号=退货入库子表.退货申请明细号 where 1<>1";
                dtP = new DataTable();
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtP);
            }
            else
            {
                string sql = string.Format("select * from 退货入库主表 where 退货入库单号 = '{0}'", drM["退货入库单号"].ToString());
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);

                drM = dtM.Rows[0];
                dataBindHelper1.DataFormDR(drM);

                string sql2 = string.Format(@"select 退货入库子表.*,kc.库存总数,kc.货架描述,出库明细号,通知单明细号,销售明细号 from 退货入库子表 
             left join 仓库物料数量表 kc on 退货入库子表.物料编码 = kc.物料编码 and  退货入库子表.仓库号=kc.仓库号
             left join 基础数据物料信息表 on   基础数据物料信息表.物料编码 = kc.物料编码
             left join 退货申请子表 on 退货申请子表.退货申请明细号=退货入库子表.退货申请明细号
                where 退货入库单号 = '{0}'", drM["退货入库单号"].ToString());
                dtP = new DataTable();
                SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
                da2.Fill(dtP);
            }
            dtP.Columns.Add("可入库数量");
            dtP.ColumnChanged += dtP_ColumnChanged;
        }


        /// <summary>
        /// 19-10-24 
        /// bl用来标记是否是正常退货
        /// 19-12-04 销售退货 如果没有全部开票完成 需要 把数量回写  订单要关 自己去关
        /// 返回值从 t_ck 变成 ds_ck 0是t_ck  1是dt_salemx
        /// </summary>
        /// <param name="bl"></param>
        /// <returns></returns>
        private DataSet fun_保存主表明细(Boolean bl)
        {
            try
            {
                DataSet ds_ck = new DataSet();
                DataTable t_ck = new DataTable();
                DataTable dt_salemx = new DataTable();
                DateTime t = CPublic.Var.getDatetime();
                if (drM["GUID"].ToString() == "")
                {
                    drM["GUID"] = System.Guid.NewGuid();
                    txt_入库单号.Text = string.Format("TH{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                         t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("TH", t.Year, t.Month).ToString("0000"));
                    drM["退货入库单号"] = txt_入库单号.Text;
                    drM["创建日期"] = t;
                }
                drM["操作人员编号"] = CPublic.Var.LocalUserID;
                drM["操作人员"] = CPublic.Var.localUserName;

                drM["生效"] = true;
                drM["生效人员编号"] = CPublic.Var.LocalUserID;
                drM["生效日期"] = t;

                if (textBox1.Text.ToString().Trim() == "退金额" || textBox1.Text.ToString().Trim() == "特殊")
                {
                    drM["完成"] = 1;
                }
                dataBindHelper1.DataToDR(drM);

                int i = 1;

                foreach (DataRow r in dtP.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    if (r["GUID"].ToString() == "")
                    {
                        r["GUID"] = System.Guid.NewGuid();
                        r["退货入库单号"] = drM["退货入库单号"];
                        r["退货入库明细号"] = drM["退货入库单号"].ToString() + i.ToString("00");
                        r["POS"] = i++;
                    }

                    r["生效"] = true;
                    r["生效人员编号"] = CPublic.Var.LocalUserID;
                    r["生效日期"] = t;

                    ////19-10-24 
                    //if (textBox2.Text == "前期发货单退货")
                    //{
                    //    string sql = string.Format("select  * from 销售记录成品出库单明细表 where 1=2" );
                    //    t_ck = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    //    fun_退货开票用("前期发货单退货");
                    //}
                    //else //正常按订单退货的 
                    //{
                    //19-4-27加
                    //要把销售出库里面的累计退货数量更新
                    if (bl)
                    {
                        // 20-4-27 增加上次累计退货数量
                        decimal dec = 0;
                        string sql = string.Format("select  * from 销售记录成品出库单明细表 where 成品出库单明细号='{0}'", r["出库明细号"]);
                        using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                        {
                            da.Fill(t_ck);//这里需要在退货申请的时候做限制 不可以有重复的物料

                            DataRow[] rrx = t_ck.Select(string.Format("成品出库单明细号='{0}'", r["出库明细号"].ToString()));
                            //20-4-27
                            dec = Convert.ToDecimal(rrx[0]["累计退货数量"]);

                            rrx[0]["累计退货数量"] = Convert.ToDecimal(rrx[0]["累计退货数量"]) + Convert.ToDecimal(r["数量"]);
                        }
                        string s_出库单号 = r["出库明细号"].ToString().Split('-')[0];
                        int p = Convert.ToInt32(r["出库明细号"].ToString().Split('-')[1]);
                        DataRow[] tr = t_ck.Select(string.Format("成品出库单号='{0}' and POS={1} and 退货标识<>'是'", s_出库单号, p));
                        //如果退货数量+累计退货数量>出库数量 -已开票数量
                        //那 退货数量+累计退货数量 -（出库数量 -已开票数量） 部分 需要生成负的 出库记录
                        

                        if (Convert.ToDecimal(tr[0]["累计退货数量"]) > Convert.ToDecimal(tr[0]["出库数量"]) - Convert.ToDecimal(tr[0]["已开票数量"]))
                        {
                            //成品出库明细
                            DataRow rr = t_ck.NewRow();
                            rr["GUID"] = System.Guid.NewGuid();
                            rr["成品出库单号"] = s_出库单号;
                            int pos = 0;
                            //and 退货标识<>'是'
                            DataRow[] rg = t_ck.Select(string.Format("成品出库单号='{0}'  ", s_出库单号), "POS desc");
                            pos = Convert.ToInt32(rg[0]["POS"]);

                            //if (tr.Length > 0)
                            //    rr["POS"] = Convert.ToInt32(tr[0]["POS"]) + 1;
                            //else
                            //{
                            string s = string.Format("select  max(pos)POS from 销售记录成品出库单明细表 where 成品出库单号='{0}'", s_出库单号);
                            DataTable tt = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                            pos = Convert.ToInt32(tt.Rows[0]["POS"]) > pos ? Convert.ToInt32(tt.Rows[0]["POS"]) + 1 : pos + 1;
                            rr["POS"] = pos;
                            //}
                            rr["成品出库单明细号"] = s_出库单号 + "-" + rr["POS"].ToString();
                            rr["备注1"] = "退货";
                            rr["退货标识"] = "是";
                            try
                            {
                                rr["销售订单号"] = tr[0]["销售订单号"];
                                rr["销售订单明细号"] = tr[0]["销售订单明细号"];
                                rr["出库通知单号"] = tr[0]["出库通知单号"];
                                rr["出库通知单明细号"] = tr[0]["出库通知单明细号"];
                            }
                            catch
                            { }
                            rr["物料编码"] = r["物料编码"];
                            rr["物料名称"] = r["物料名称"];
                            //那 退货数量+累计退货数量 -（出库数量 -已开票数量） 部分 需要生成负的 出库记录
                            //20-4-27 这里的 累退数是 加上本次的退货数量 , 应该要再减去 这次之前的 所有 负的出库单
                            decimal dd = -dec + Convert.ToDecimal(tr[0]["出库数量"]) - Convert.ToDecimal(tr[0]["已开票数量"]);
                            decimal dec_应 = 0;
                            if(dd<0)
                            {
                                dec_应= -Convert.ToDecimal(r["数量"]);
                            }
                            else
                            {
                                dec_应=dd - Convert.ToDecimal(r["数量"]);
                            }
                            rr["出库数量"] =dec_应;
                            rr["已出库数量"] = dec_应;
                            rr["未开票数量"] = dec_应;
                            //rr["出库数量"] = -Convert.ToDecimal(tr[0]["累计退货数量"]) + Convert.ToDecimal(tr[0]["出库数量"]) - Convert.ToDecimal(tr[0]["已开票数量"]);
                            // rr["已出库数量"] = -Convert.ToDecimal(tr[0]["累计退货数量"]) + Convert.ToDecimal(tr[0]["出库数量"]) - Convert.ToDecimal(tr[0]["已开票数量"]);
                            // rr["未开票数量"] = -Convert.ToDecimal(tr[0]["累计退货数量"]) + Convert.ToDecimal(tr[0]["出库数量"]) - Convert.ToDecimal(tr[0]["已开票数量"]);
                            DataTable dt_1 = new DataTable();
                            string sql_1 = string.Format("select  * from 基础数据物料信息表 where 物料编码='{0}'", r["物料编码"]);
                            dt_1 = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);
                            rr["计量单位"] = dt_1.Rows[0]["计量单位"];
                            rr["规格型号"] = dt_1.Rows[0]["规格型号"];
                            rr["客户"] = tr[0]["客户"];
                            rr["客户编号"] = tr[0]["客户编号"];
                            rr["生效"] = true;
                            rr["生效日期"] = t;

                            rr["仓库号"] = tr[0]["仓库号"];
                            rr["仓库名称"] = tr[0]["仓库名称"];

                            t_ck.Rows.Add(rr);
                        }
                        //}
                        //19-12-4  15:35 销售退货如果没有全部开票完成 需要把销售订单上得 已完成数量 和 已通知数量  撤回  
                        //19-12-27 申请时增加字段 是否再发货  false 不要执行下面代码
                        if (Convert.ToBoolean(dr_退货申请["是否再发货"]))
                        {
                            string SaleOrder = tr[0]["销售订单明细号"].ToString();
                            string sale = string.Format(@"select a.*,b.未开票数量 from 销售记录销售订单明细表 a 
                left join(SELECT  销售订单明细号, SUM(已开票数量) AS 已开票数量, SUM(未开票数量 - 累计退货数量) AS 未开票数量
                FROM  dbo.销售记录成品出库单明细表  GROUP BY 销售订单明细号) b on a.销售订单明细号 = b.销售订单明细号
                where a.销售订单明细号 = '{0}'", SaleOrder);
                            using (SqlDataAdapter da = new SqlDataAdapter(sale, strconn))
                            {
                                if (dt_salemx.Columns.Count == 0)
                                {
                                    da.Fill(dt_salemx);
                                }
                                else
                                {
                                    DataRow[] r_sale = dt_salemx.Select(string.Format("销售订单明细号='{0}'", SaleOrder));
                                    if (r_sale.Length == 0)
                                    {
                                        da.Fill(dt_salemx);
                                    }
                                }
                            }
                            if (dt_salemx.Rows.Count > 0)
                            {
                                DataRow[] r_sale = dt_salemx.Select(string.Format("销售订单明细号='{0}'", SaleOrder));
                                if (Convert.ToDecimal(r_sale[0]["未开票数量"]) > 0) //未开票完成的都需要把订单数量回过去
                                {
                                    r_sale[0]["完成数量"] = Convert.ToDecimal(r_sale[0]["完成数量"]) - Convert.ToDecimal(r["数量"]);
                                    r_sale[0]["未完成数量"] = Convert.ToDecimal(r_sale[0]["未完成数量"]) + Convert.ToDecimal(r["数量"]);
                                    r_sale[0]["已通知数量"] = Convert.ToDecimal(r_sale[0]["已通知数量"]) - Convert.ToDecimal(r["数量"]);
                                    r_sale[0]["未通知数量"] = Convert.ToDecimal(r_sale[0]["未通知数量"]) + Convert.ToDecimal(r["数量"]);
                                    r_sale[0]["明细完成"] = 0;
                                    r_sale[0]["明细完成日期"] = DBNull.Value;
                                }
                            }
                        }
                    }
                }
                if (!bl)
                {
                    string sql = string.Format("select  * from 销售记录成品出库单明细表 where 1=2");
                    t_ck = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                    foreach (DataRow r in dtP.Rows)
                    {
                        if (r.RowState == DataRowState.Deleted) continue;
                        DataRow[] yy = dt_退货申请.Select(string.Format("退货申请明细号='{0}'", r["退货申请明细号"]));

                        yy[0]["销售明细号"] = r["销售明细号"];
                        yy[0]["销售明细"] = r["销售明细号"];

                        yy[0]["出库明细号"] = r["出库明细号"];


                    }
                }
                fun_判断退货申请();
                ds_ck.Tables.Add(t_ck);
                ds_ck.Tables.Add(dt_salemx);
                return ds_ck;
            }
            catch (Exception ex)
            {
                throw new Exception("明细保存出错" + ex.Message);
            }
        }
        private void dtP_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            try
            {
                if (e.Column.Caption == "物料编码")
                {
                    DataRow[] ds = dt_物料.Select(string.Format("物料编码 = '{0}'", e.Row["物料编码"]));
                    //e.Row["原ERP物料编号"] = ds[0]["原ERP物料编号"];
                    e.Row["物料名称"] = ds[0]["物料名称"];
                    e.Row["规格型号"] = ds[0]["规格型号"];
                    e.Row["货架描述"] = ds[0]["货架描述"];
                    e.Row["库存总数"] = ds[0]["库存总数"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_物料下拉框()
        {
            string sql = @"select base.物料编码,base.物料名称,base.规格型号,
            kc.库存总数,kc.货架描述 from 基础数据物料信息表 base
            left join 仓库物料数量表 kc on base.物料编码 = kc.物料编码";
            dt_物料 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_物料);

            repositoryItemSearchLookUpEdit1.DataSource = dt_物料;
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";

            sql = @"select 属性值 as 仓库名称,属性字段1 as 仓库号 from 基础数据基础属性表  where 属性类别 = '仓库类别'
                     and 属性字段1 in (96, 97) ";
            dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            repositoryItemSearchLookUpEdit2.DataSource = dt_仓库;
            repositoryItemSearchLookUpEdit2.ValueMember = "仓库号";
            repositoryItemSearchLookUpEdit2.DisplayMember = "仓库号";
        }

        private DataTable fun_保存记录到出入库明细()
        {
            try
            {
                DataRow dr_待办 = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);
                string sql = "select * from 仓库出入库明细表 where 1<>1";
                DataTable dt = new DataTable();
                DateTime t = CPublic.Var.getDatetime();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                foreach (DataRow r in dtP.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    DataRow dr = dt.NewRow();
                    dr["GUID"] = System.Guid.NewGuid();
                    dr["明细类型"] = "销售退货";
                    dr["单号"] = r["出库明细号"].ToString().Split('-')[0];

                    //dr["相关单号"] = r["退货入库明细号"].ToString();
                    dr["相关单号"] = r["销售明细号"].ToString();


                    dr["物料编码"] = r["物料编码"].ToString();
                    dr["物料名称"] = r["物料名称"].ToString();
                    dr["明细号"] = r["出库明细号"].ToString();
                    dr["出库入库"] = "出库";

                    dr["相关单位"] = dr_待办["客户"].ToString();
                    dr["数量"] = (Decimal)0;
                    dr["标准数量"] = (Decimal)0;
                    dr["实效数量"] = Convert.ToDecimal(r["数量"].ToString());
                    dr["实效时间"] = t;
                    dr["出入库时间"] = t;
                    dr["仓库号"] = r["仓库号"];
                    dr["仓库名称"] = r["仓库名称"];
                    dr["仓库人"] = CPublic.Var.localUserName;



                    //                    string sql_pd = string.Format(@"select 仓库物料盘点表.盘点批次号 from [仓库物料盘点表] left join [仓库物料盘点明细表] 
                    //                                                    on 仓库物料盘点表.盘点批次号 = [仓库物料盘点明细表].盘点批次号 
                    //                                                    where [仓库物料盘点表].有效 = 0 and [仓库物料盘点明细表].物料编码 = '{0}'", r["物料编码"].ToString().Trim());
                    //                    using (SqlDataAdapter da1 = new SqlDataAdapter(sql_pd, strconn))
                    //                    {
                    //                        DataTable dt_批次号 = new DataTable();
                    //                        da1.Fill(dt_批次号);
                    //                        if (dt_批次号.Rows.Count > 0)
                    //                        {
                    //                            dr["盘点有效批次号"] = dt_批次号.Rows[0]["盘点批次号"];
                    //                        }
                    //                        else
                    //                        {
                    //                            dr["盘点有效批次号"] = "初始化";
                    //                        }
                    //                    }
                    dt.Rows.Add(dr);
                }
                return dt;
                //new SqlCommandBuilder(da);
                //da.Update(dt);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm退货入库_fun_保存出入库明细");
                throw ex;
            }
        }

        //此为 退货入库记录 +  其他出库申请 以及 其他出库记录 和 仓库出入库明细表 出库记录 
        /// <summary>
        /// 暂不用
        /// </summary>
        /// <returns></returns>
        private DataSet fun_出入库明细_特殊()
        {

            try
            {
                DataSet ds = new DataSet();
                DateTime t = CPublic.Var.getDatetime();
                DataRow dr_待办 = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);
                string s = "select  * from  其他出入库申请主表 where 1<>1";
                DataTable dt_其他出申请主表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "select  * from  其他出入库申请子表 where 1<>1";
                DataTable dt_其他出申请子表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "select  * from  其他出库主表 where 1<>1";
                DataTable dt_其他出主表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "select  * from  其他出库子表 where 1<>1";
                DataTable dt_其他出子表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);

                s = "select  * from  返修出入库申请主表 where 1<>1";
                DataTable dt_返修入申请主表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "select  * from  返修出入库申请子表 where 1<>1";
                DataTable dt_返修入申请子表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "select  * from  返修入库主表 where 1<>1";
                DataTable dt_返修入主表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "select  * from  返修入库子表 where 1<>1";
                DataTable dt_返修入子表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);

                #region 其他申请主
                string s申请_no = string.Format("QWSQ{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
             t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("QWSQ", t.Year, t.Month).ToString("0000"));
                DataRow dr_申请主 = dt_其他出申请主表.NewRow();

                dr_申请主["GUID"] = System.Guid.NewGuid();
                dr_申请主["出入库申请单号"] = s申请_no;
                dr_申请主["申请日期"] = t;
                dr_申请主["操作人员编号"] = CPublic.Var.LocalUserID;
                dr_申请主["操作人员"] = CPublic.Var.localUserName;
                dr_申请主["生效"] = true;
                dr_申请主["完成"] = true;
                dr_申请主["生效人员编号"] = CPublic.Var.LocalUserID;
                dr_申请主["生效日期"] = t;
                dr_申请主["完成日期"] = t;
                dr_申请主["备注"] = "常熟返修";
                dr_申请主["申请类型"] = "其他出库";
                dr_申请主["原因分类"] = "特殊";
                dt_其他出申请主表.Rows.Add(dr_申请主);
                #endregion
                #region 其他出库主
                string s出库_no = string.Format("QT{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                      t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("QT", t.Year, t.Month).ToString("0000"));
                DataRow dr_出库主 = dt_其他出主表.NewRow();
                dr_出库主["GUID"] = System.Guid.NewGuid();
                dr_出库主["其他出库单号"] = s出库_no;
                dr_出库主["创建日期"] = t;
                dr_出库主["操作人员编号"] = CPublic.Var.LocalUserID;
                dr_出库主["操作人员"] = CPublic.Var.localUserName;
                dr_出库主["出库仓库"] = "";
                dr_出库主["领用人员"] = "返修";
                dr_出库主["领用人员编号"] = "返修";
                dr_出库主["生效"] = true;
                dr_出库主["生效人员编号"] = CPublic.Var.LocalUserID;
                dr_出库主["生效日期"] = t;
                dr_出库主["出库日期"] = t;
                dr_出库主["出库类型"] = "特殊";
                dr_出库主["出入库申请单号"] = s申请_no;
                dt_其他出主表.Rows.Add(dr_出库主);
                #endregion

                #region  返修入申请
                string s返修申请no = string.Format("RMA{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                         t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("RMA", t.Year, t.Month).ToString("0000"));
                DataRow dr_返修申请主 = dt_返修入申请主表.NewRow();
                dr_返修申请主["GUID"] = System.Guid.NewGuid();
                dr_返修申请主["出入库申请单号"] = s返修申请no;
                dr_返修申请主["申请日期"] = t;
                dr_返修申请主["操作人员编号"] = CPublic.Var.LocalUserID;
                dr_返修申请主["操作人员"] = CPublic.Var.localUserName;
                dr_返修申请主["生效"] = true;
                dr_返修申请主["生效人员编号"] = CPublic.Var.LocalUserID;
                dr_返修申请主["生效日期"] = t;
                dr_返修申请主["完成"] = true;
                dr_返修申请主["完成日期"] = t;
                dr_返修申请主["备注"] = "常熟返修";
                dr_返修申请主["申请类型"] = "返修入库";
                dt_返修入申请主表.Rows.Add(dr_返修申请主);

                #endregion

                #region 返修入库主
                string s返修入主_no = string.Format("RMI{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                      t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("RMI", t.Year, t.Month).ToString("0000"));
                DataRow dr_返修入主 = dt_返修入主表.NewRow();
                dr_返修入主["GUID"] = System.Guid.NewGuid();
                dr_返修入主["返修入库单号"] = s返修入主_no;
                dr_返修入主["创建日期"] = t;
                dr_返修入主["操作人员编号"] = CPublic.Var.LocalUserID;
                dr_返修入主["操作人员"] = CPublic.Var.localUserName;

                dr_返修入主["生效"] = true;
                dr_返修入主["生效人员编号"] = CPublic.Var.LocalUserID;
                dr_返修入主["生效日期"] = t;
                dr_返修入主["入库日期"] = t;
                dr_返修入主["出入库申请单号"] = s返修申请no;
                dt_返修入主表.Rows.Add(dr_返修入主);
                #endregion

                string sql = "select * from 仓库出入库明细表 where 1<>1";
                DataTable dt_出入库明细 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_出入库明细);
                string ss = "select  *  from 返修仓库出入库明细表 where 1<>1";
                DataTable dt_返修_出入明细 = CZMaster.MasterSQL.Get_DataTable(ss, strconn);

                int pos = 1;
                foreach (DataRow r in dtP.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    #region 退货入库，其他出库 仓库出入库明细  和  返修入库 的 返修库流水
                    DataRow dr = dt_出入库明细.NewRow();
                    DataRow dr1 = dt_出入库明细.NewRow();
                    DataRow dr2 = dt_返修_出入明细.NewRow();
                    dr["GUID"] = System.Guid.NewGuid();
                    dr1["GUID"] = System.Guid.NewGuid();
                    dr2["GUID"] = System.Guid.NewGuid();
                    dr["明细类型"] = "退货入库";
                    dr1["明细类型"] = "其他出库";
                    dr2["明细类型"] = "返修入库";

                    dr["单号"] = r["退货入库单号"].ToString();
                    dr1["单号"] = s出库_no;
                    dr2["单号"] = s返修入主_no;

                    dr["相关单号"] = dr_待办["退货申请单号"].ToString();
                    dr1["相关单号"] = s申请_no;
                    dr2["相关单号"] = s返修申请no;

                    dr["物料编码"] = r["物料编码"].ToString();
                    dr1["物料编码"] = r["物料编码"].ToString();
                    dr2["物料编码"] = r["物料编码"].ToString();

                    dr["物料名称"] = r["物料名称"].ToString();
                    dr1["物料名称"] = r["物料名称"].ToString();
                    dr2["物料名称"] = r["物料名称"].ToString();

                    dr["明细号"] = r["退货入库明细号"].ToString();
                    dr1["明细号"] = s出库_no + pos.ToString("00");
                    dr2["明细号"] = s返修入主_no + pos.ToString("00");

                    dr["出库入库"] = "入库";
                    dr1["出库入库"] = "出库";
                    dr2["出库入库"] = "入库";

                    dr["相关单位"] = dr_待办["客户"].ToString();
                    dr2["数量"] = (Decimal)0;
                    dr1["数量"] = (Decimal)0;
                    dr["数量"] = (Decimal)0;

                    dr["标准数量"] = (Decimal)0;
                    dr1["标准数量"] = (Decimal)0;
                    dr2["标准数量"] = (Decimal)0;

                    dr["实效数量"] = Convert.ToDecimal(r["数量"].ToString());
                    dr1["实效数量"] = -Convert.ToDecimal(r["数量"].ToString());
                    dr2["实效数量"] = Convert.ToDecimal(r["数量"].ToString());

                    dr["实效时间"] = t;
                    dr["出入库时间"] = t;
                    dr1["实效时间"] = t;
                    dr1["出入库时间"] = t;
                    dr2["实效时间"] = t;
                    dr2["出入库时间"] = t;
                    dt_出入库明细.Rows.Add(dr);
                    dt_出入库明细.Rows.Add(dr1);
                    dt_返修_出入明细.Rows.Add(dr2);
                    #endregion
                    #region 其他申请子表记录
                    DataRow dr_其他出申请子表 = dt_其他出申请子表.NewRow();

                    dr_其他出申请子表["GUID"] = System.Guid.NewGuid();
                    dr_其他出申请子表["出入库申请单号"] = s申请_no;
                    dr_其他出申请子表["出入库申请明细号"] = s申请_no + pos.ToString("00");
                    dr_其他出申请子表["POS"] = pos;
                    dr_其他出申请子表["物料编码"] = r["物料编码"].ToString();
                    dr_其他出申请子表["物料名称"] = r["物料名称"].ToString();
                    //dr_其他出申请子表["原ERP物料编号"] = r["原ERP物料编号"].ToString();
                    dr_其他出申请子表["数量"] = r["数量"];
                    dr_其他出申请子表["规格型号"] = r["规格型号"].ToString();
                    dr_其他出申请子表["完成"] = true;
                    dr_其他出申请子表["完成日期"] = t;
                    dr_其他出申请子表["生效"] = true;
                    dr_其他出申请子表["生效人员编号"] = CPublic.Var.LocalUserID;
                    dr_其他出申请子表["生效日期"] = t;
                    dt_其他出申请子表.Rows.Add(dr_其他出申请子表);

                    #endregion

                    #region 返修入申请子表记录
                    DataRow dr_返修入申请子 = dt_返修入申请子表.NewRow();

                    dr_返修入申请子["GUID"] = System.Guid.NewGuid();
                    dr_返修入申请子["出入库申请单号"] = s返修申请no;
                    dr_返修入申请子["出入库申请明细号"] = s返修申请no + pos.ToString("00");
                    dr_返修入申请子["POS"] = pos;
                    dr_返修入申请子["物料编码"] = r["物料编码"].ToString();
                    dr_返修入申请子["物料名称"] = r["物料名称"].ToString();
                    dr_返修入申请子["原ERP物料编号"] = r["原ERP物料编号"].ToString();
                    dr_返修入申请子["数量"] = r["数量"];
                    dr_返修入申请子["n原ERP规格型号"] = r["n原ERP规格型号"].ToString();
                    dr_返修入申请子["完成"] = true;
                    dr_返修入申请子["完成日期"] = t;
                    dr_返修入申请子["生效"] = true;
                    dr_返修入申请子["生效人员编号"] = CPublic.Var.LocalUserID;
                    dr_返修入申请子["生效日期"] = t;
                    dt_返修入申请子表.Rows.Add(dr_返修入申请子);

                    #endregion

                    #region 其他出库子表记录
                    DataRow dr_其他出子表 = dt_其他出子表.NewRow();
                    dr_其他出子表["GUID"] = System.Guid.NewGuid();
                    dr_其他出子表["其他出库单号"] = s出库_no;
                    dr_其他出子表["其他出库明细号"] = s出库_no + pos.ToString("00");
                    dr_其他出子表["POS"] = pos;
                    dr_其他出子表["物料编码"] = r["物料编码"].ToString();
                    dr_其他出子表["物料名称"] = r["物料名称"].ToString();
                    dr_其他出子表["原ERP物料编号"] = r["原ERP物料编号"].ToString();
                    dr_其他出子表["数量"] = r["数量"];
                    dr_其他出子表["n原ERP规格型号"] = r["n原ERP规格型号"].ToString();
                    dr_其他出子表["完成"] = true;
                    dr_其他出子表["完成日期"] = t;
                    dr_其他出子表["生效"] = true;
                    dr_其他出子表["生效人员编号"] = CPublic.Var.LocalUserID;
                    dr_其他出子表["生效日期"] = t;
                    dr_其他出子表["出入库申请单号"] = s申请_no;
                    dr_其他出子表["出入库申请明细号"] = s申请_no + pos.ToString("00");
                    dt_其他出子表.Rows.Add(dr_其他出子表);

                    #endregion

                    #region 返修入库子表记录
                    DataRow dr_返修入子表 = dt_返修入子表.NewRow();
                    dr_返修入子表["GUID"] = System.Guid.NewGuid();
                    dr_返修入子表["返修入库单号"] = s返修入主_no;
                    dr_返修入子表["返修入库明细号"] = s返修入主_no + pos.ToString("00");
                    dr_返修入子表["POS"] = pos;
                    dr_返修入子表["物料编码"] = r["物料编码"].ToString();
                    dr_返修入子表["物料名称"] = r["物料名称"].ToString();
                    dr_返修入子表["原ERP物料编号"] = r["原ERP物料编号"].ToString();
                    dr_返修入子表["数量"] = r["数量"];
                    dr_返修入子表["n原ERP规格型号"] = r["n原ERP规格型号"].ToString();

                    dr_返修入子表["生效"] = true;
                    dr_返修入子表["生效人员编号"] = CPublic.Var.LocalUserID;
                    dr_返修入子表["生效日期"] = t;
                    dt_返修入子表.Rows.Add(dr_返修入子表);
                    dr_返修入子表["出入库申请单号"] = s返修申请no;
                    dr_返修入子表["出入库申请明细号"] = s返修申请no + pos.ToString("00");
                    #endregion
                    pos++;
                }

                ds.Tables.Add(dt_出入库明细);
                ds.Tables.Add(dt_返修_出入明细);
                ds.Tables.Add(dt_其他出申请主表);
                ds.Tables.Add(dt_其他出申请子表);
                ds.Tables.Add(dt_其他出主表);
                ds.Tables.Add(dt_其他出子表);
                ds.Tables.Add(dt_返修入申请主表);
                ds.Tables.Add(dt_返修入申请子表);
                ds.Tables.Add(dt_返修入主表);
                ds.Tables.Add(dt_返修入子表);

                return ds;
                //new SqlCommandBuilder(da);
                //da.Update(dt);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm退货入库_fun_保存出入库明细");
                throw ex;
            }
        }
        private void fun_人员()
        {
            string sql = string.Format(@"select 员工号,姓名 from 人事基础员工表 where 在职状态 = '在职'");
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            dt_人员 = new DataTable();
            da.Fill(dt_人员);
            //txt_入库人员ID.Properties.DataSource = dt_人员;
            //txt_入库人员ID.Properties.DisplayMember = "员工号";
            //txt_入库人员ID.Properties.ValueMember = "员工号";
        }

        private void fun_载入代办()
        {
            string sql = "select * from 退货申请主表 where 生效 = 1 and 完成 = 0 and 作废 = 0";

            dt_代办 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_代办);
            gc_代办.DataSource = dt_代办;
        }
        /// <summary>
        /// 原先用 bl =true 表示退金额  false 为退货
        /// 现改为 string 类型 直接表示类型
        /// 19-10-24 修改为 按客户退货 退没有单据的东西 需要生成
        /// </summary>
        /// <param name="bl"></param>
        private DataSet fun_退货开票用(string s_类型)
        {

            DataSet ds = new DataSet();
            DataTable dt_订单主表;
            DataTable dt_订单明细;
            DataTable dt_发货明细;

            DataRow dr_待办 = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);
            string sql_订单主表 = "select * from 销售记录销售订单主表 where 1=2 ";
            dt_订单主表 = CZMaster.MasterSQL.Get_DataTable(sql_订单主表, strconn);
            DataRow r_订主 = dt_订单主表.NewRow();
            dt_订单主表.Rows.Add(r_订主);
            if (dr_待办["业务单据日期"] == null || dr_待办["业务单据日期"].ToString() == "")
            {
                throw new Exception("业务单据日期没有值,请确认退货申请单据,退前期的单据业务单据日期必须有值");
            }
            DateTime time = Convert.ToDateTime(dr_待办["业务单据日期"]);


            DateTime t = CPublic.Var.getDatetime();
            //销售订单主表
            string str_订单号 = string.Format("SO{0}{1}{2}{3}", time.Year.ToString(), time.Month.ToString("00"),
                 time.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SO", time.Year, time.Month, time.Day).ToString("0000"));
            r_订主["GUID"] = System.Guid.NewGuid();
            r_订主["销售订单号"] = str_订单号;
            r_订主["客户编号"] = dr_待办["客户编号"];
            r_订主["客户名"] = dr_待办["客户"];
            r_订主["录入人员"] = dr_待办["操作人员"];
            r_订主["录入人员ID"] = dr_待办["操作人员编号"];
            r_订主["创建日期"] = r_订主["生效日期"] = time;
            r_订主["日期"] = r_订主["修改日期"] = t;
            r_订主["生效"] = true;
            r_订主["备注1"] = s_类型;
            r_订主["销售备注"] = s_类型;
            r_订主["部门编号"] = dr_待办["部门编号"];
            r_订主["销售部门"] = dr_待办["部门名称"];
            // 通知单主表 
            string sql = string.Format("select  * from 销售记录销售出库通知单主表  where 1=2");
            DataTable dt_tzz = new DataTable();
            dt_tzz = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            string str_通知单号 = string.Format("SK{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("SK", t.Year, t.Month));
            DataRow r_通知 = dt_tzz.NewRow();
            dt_tzz.Rows.Add(r_通知);
            r_通知["GUID"] = System.Guid.NewGuid();
            r_通知["出库通知单号"] = str_通知单号;
            r_通知["操作员"] = CPublic.Var.localUserName;
            r_通知["操作员ID"] = CPublic.Var.LocalUserID;
            r_通知["客户编号"] = dr_待办["客户编号"];
            r_通知["客户名"] = dr_待办["客户"];
            r_通知["备注"] = s_类型;
            r_通知["出库日期"] = t;
            r_通知["创建日期"] = t;
            r_通知["修改日期"] = t;
            r_通知["生效"] = true;
            r_通知["生效日期"] = t;
            r_通知["完成"] = true;
            r_通知["完成日期"] = t;



            // 成品出库主表
            sql = string.Format("select  * from 销售记录成品出库单主表  where 1=2");
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            string str_出库单号 = string.Format("SA{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("SA", t.Year, t.Month));
            DataRow r = dt.NewRow();
            dt.Rows.Add(r);
            r["GUID"] = System.Guid.NewGuid();
            r["成品出库单号"] = str_出库单号;
            r["操作员"] = CPublic.Var.localUserName;
            r["操作员ID"] = CPublic.Var.LocalUserID;
            r["客户"] = dr_待办["客户"];
            r["日期"] = t;
            r["创建日期"] = t;
            r["修改日期"] = t;
            r["生效"] = true;
            r["生效日期"] = t;
            r["出库类型"] = "退货";

            r["出库类型"] = "前期无单据退货";

            string sql_订单明细 = "select * from 销售记录销售订单明细表 where 1=2 ";
            dt_订单明细 = CZMaster.MasterSQL.Get_DataTable(sql_订单明细, strconn);

            string sql_发货明细 = "select * from 销售记录销售出库通知单明细表 where 1=2 ";
            dt_发货明细 = CZMaster.MasterSQL.Get_DataTable(sql_发货明细, strconn);


            string sql_mx = string.Format("select  * from 销售记录成品出库单明细表  where 1=2");
            DataTable dt_mx = new DataTable();
            dt_mx = CZMaster.MasterSQL.Get_DataTable(sql_mx, strconn);
            int i = 1;
            foreach (DataRow dr in dtP.Rows)
            {
                //销售明细
                DataRow r_订明细 = dt_订单明细.NewRow();
                dt_订单明细.Rows.Add(r_订明细);
                r_订明细["GUID"] = System.Guid.NewGuid();
                r_订明细["销售订单号"] = str_订单号;
                r_订明细["POS"] = i;
                dr["销售明细号"] = r_订明细["销售订单明细号"] = str_订单号 + "-" + i.ToString();
                r_订明细["物料编码"] = dr["物料编码"];
                r_订明细["数量"] = dr["数量"];
                r_订明细["物料名称"] = dr["物料名称"];
                r_订明细["规格型号"] = dr["规格型号"];
                r_订明细["完成数量"] = dr["数量"];
                r_订明细["已通知数量"] = dr["数量"];
                r_订明细["明细完成"] = true;
                r_订明细["明细完成日期"] = time;
                r_订明细["客户"] = dr_待办["客户"];
                r_订明细["客户编号"] = dr_待办["客户编号"];
                r_订明细["生效"] = true;
                r_订明细["生效日期"] = time;
                r_订明细["备注"] = s_类型;

                // 发货明细
                DataRow r_发货明细 = dt_发货明细.NewRow();
                dt_发货明细.Rows.Add(r_发货明细);
                r_发货明细["GUID"] = System.Guid.NewGuid();
                r_发货明细["出库通知单号"] = str_通知单号;
                r_发货明细["出库通知单明细号"] = str_通知单号 + "-" + i.ToString();
                r_发货明细["销售订单明细号"] = r_订明细["销售订单明细号"];
                r_发货明细["POS"] = i;
                r_发货明细["销售订单明细号"] = str_订单号 + "-" + i.ToString();
                r_发货明细["物料编码"] = dr["物料编码"];
                r_发货明细["出库数量"] = dr["数量"];
                r_发货明细["已出库数量"] = dr["数量"];
                r_发货明细["客户"] = dr_待办["客户"];
                r_发货明细["客户编号"] = dr_待办["客户编号"];
                r_发货明细["退货标识"] = "是";
                r_发货明细["物料名称"] = dr["物料名称"];
                r_发货明细["规格型号"] = dr["规格型号"];
                r_发货明细["操作员"] = CPublic.Var.localUserName;
                r_发货明细["操作员ID"] = CPublic.Var.LocalUserID;
                r_发货明细["生效"] = true;
                r_发货明细["生效日期"] = t;
                r_发货明细["完成"] = true;
                r_发货明细["完成日期"] = time;


                //成品出库明细
                DataRow rr = dt_mx.NewRow();
                dt_mx.Rows.Add(rr);
                rr["GUID"] = System.Guid.NewGuid();
                rr["成品出库单号"] = str_出库单号;
                rr["POS"] = i;
                dr["出库明细号"] = rr["成品出库单明细号"] = str_出库单号 + "-" + i++.ToString();
                rr["备注1"] = "退货";

                rr["销售订单号"] = str_订单号;
                rr["销售订单明细号"] = r_订明细["销售订单明细号"];
                //rr["出库通知单"] = dr["通知单明细号"].ToString().Split('-')[0];
                //rr["出库通知单明细号"] = dr["通知单明细号"];
                rr["退货标识"] = "是";

                rr["物料编码"] = dr["物料编码"];
                rr["物料名称"] = dr["物料名称"];
                rr["出库数量"] = -Convert.ToDecimal(dr["数量"]);
                rr["已出库数量"] = -Convert.ToDecimal(dr["数量"]);
                rr["未开票数量"] = -Convert.ToDecimal(dr["数量"]);
                DataTable dt_1 = new DataTable();
                string sql_1 = string.Format("select  * from 基础数据物料信息表 where 物料编码='{0}'", dr["物料编码"]);
                dt_1 = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);
                rr["计量单位"] = dt_1.Rows[0]["计量单位"];
                rr["规格型号"] = dt_1.Rows[0]["规格型号"];
                rr["客户"] = dr_待办["客户"];
                rr["客户编号"] = dr_待办["客户编号"];
                rr["生效"] = true;
                rr["生效日期"] = t;
                rr["仓库号"] = dr["仓库号"];
                rr["仓库名称"] = dr["仓库名称"];
            }
            ds.Tables.Add(dt_订单主表);
            ds.Tables.Add(dt_订单明细);
            ds.Tables.Add(dt);
            ds.Tables.Add(dt_mx);
            return ds;
        }
        private void fun_check()
        {
            foreach (DataRow dr in dtP.Rows)
            {
                if (dr["仓库号"].ToString() == "" || dr["仓库名称"].ToString() == "")
                {
                    throw new Exception(dr["物料编码"].ToString() + "仓库未选择");
                }

            }


        }
        private void fun_判断退货申请()
        {
            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                DataRow[] ds = dt_退货申请.Select(string.Format("退货申请明细号 = '{0}'", dr["退货申请明细号"]));
                if (dr["数量确认"].ToString().ToLower() == "true")
                {
                    ds[0]["完成"] = true;
                    ds[0]["完成日期"] = CPublic.Var.getDatetime();
                }
                if (ds[0]["已入库数量"] == null) ds[0]["已入库数量"] = 0;
                ds[0]["已入库数量"] = Convert.ToDecimal(ds[0]["已入库数量"]) + Convert.ToDecimal(dr["实际数量"]);
            }
            int count = 0;
            foreach (DataRow dr in dt_退货申请.Rows)
            {
                if (dr["完成"].ToString().ToLower() == "true")
                {
                    count = count + 1;
                }
            }
            if (count == dt_退货申请.Rows.Count)
            {
                dr_退货申请["完成"] = true;
                dr_退货申请["完成日期"] = CPublic.Var.getDatetime();
            }
        }

        private void fun_清空()
        {
            time_入库日期.EditValue = CPublic.Var.getDatetime();
            drM = null;
            txt_入库单号.Text = "";
            txt_备注.Text = "";
            //txt_关联出库单.Text = "";
            //txt_关联销售单.Text = "";

            fun_载入主表明细();
            gc.DataSource = dtP;
        }
        #endregion

        #region 界面操作
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //新增
            try
            {
                time_入库日期.EditValue = CPublic.Var.getDatetime();
                drM = null;
                txt_入库单号.Text = "";
                txt_备注.Text = "";
                //txt_关联出库单.Text = "";
                //txt_关联销售单.Text = "";

                fun_载入主表明细();
                gc.DataSource = dtP;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //保存
            try
            {
                gv.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                fun_保存主表明细(false);
                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //生效
            try
            {

                if (MessageBox.Show("确认仓库信息正确？", "确认!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    gv.CloseEditor();
                    this.BindingContext[dtP].EndCurrentEdit();
                    fun_check();
                    for (int i = 0; i < dtP.Rows.Count; i++)
                    {
                        if (dtP.Rows[i]["数量确认"].Equals(false))
                        {
                            dtP.Rows.Remove(dtP.Rows[i]);
                            i--;
                        }
                    }

                    DataSet ds_ck = new DataSet();
                    //DataTable ck = new DataTable();
                    DataSet ds = new DataSet();
                    bool bl = true;
                    if (textBox2.Text == "前期发货单退货")
                    {
                        bl = false;
                        ds = fun_退货开票用("前期发货单退货");
                    }
                    ds_ck = fun_保存主表明细(bl);
                    DataSet ds_特殊 = null;
                    DataTable dt_出入库记录 = null;
                    DataTable dt_库存 = null;

                    dt_出入库记录 = fun_保存记录到出入库明细();

                    dt_库存 = fun_库存("仓库物料数量表", 1, dtP);

                    SqlConnection conn = new SqlConnection(strconn);
                    conn.Open();
                    SqlTransaction thrk = conn.BeginTransaction("退货入库");
                    try
                    {
                        string sql1 = "select * from 退货入库主表 where 1<>1";
                        SqlCommand cmd1 = new SqlCommand(sql1, conn, thrk);
                        SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(dtM);
                        sql1 = "select * from 退货入库子表 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(dtP);
                        sql1 = "select * from 退货申请主表 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(dt_代办);

                        sql1 = "select * from 退货申请子表 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(dt_退货申请);
                        if (!bl) //这个是 前期退货申请
                        {
                            sql1 = "select * from  销售记录销售订单主表  where 1<>1";
                            cmd1 = new SqlCommand(sql1, conn, thrk);
                            da1 = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da1);
                            da1.Update(ds.Tables[0]);

                            sql1 = "select * from  销售记录销售订单明细表 where 1<>1";
                            cmd1 = new SqlCommand(sql1, conn, thrk);
                            da1 = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da1);
                            da1.Update(ds.Tables[1]);
                            sql1 = "select * from 销售记录成品出库单主表 where 1<>1";
                            cmd1 = new SqlCommand(sql1, conn, thrk);
                            da1 = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da1);
                            da1.Update(ds.Tables[2]);
                            sql1 = "select * from 销售记录成品出库单明细表 where 1<>1";
                            cmd1 = new SqlCommand(sql1, conn, thrk);
                            da1 = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da1);
                            da1.Update(ds.Tables[3]);
                        }
                        else
                        {
                            sql1 = "select * from 销售记录成品出库单明细表 where 1<>1";
                            cmd1 = new SqlCommand(sql1, conn, thrk);
                            da1 = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da1);
                            da1.Update(ds_ck.Tables[0]);
                            //19-12-04增加
                            if (ds_ck.Tables[1].Columns.Count > 0)
                            {
                                sql1 = "select * from 销售记录销售订单明细表 where 1<>1";
                                cmd1 = new SqlCommand(sql1, conn, thrk);
                                da1 = new SqlDataAdapter(cmd1);
                                new SqlCommandBuilder(da1);
                                da1.Update(ds_ck.Tables[1]);
                            }

                        }
                        if (dt_出入库记录 != null)
                        {
                            sql1 = "select * from 仓库出入库明细表 where 1<>1";
                            cmd1 = new SqlCommand(sql1, conn, thrk);
                            da1 = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da1);
                            da1.Update(dt_出入库记录);

                            sql1 = "select * from 仓库物料数量表 where 1<>1";
                            cmd1 = new SqlCommand(sql1, conn, thrk);
                            da1 = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da1);
                            da1.Update(dt_库存);
                        }

                        if (ds_特殊 != null)
                        {
                            sql1 = "select * from 仓库出入库明细表 where 1<>1";
                            cmd1 = new SqlCommand(sql1, conn, thrk);
                            da1 = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da1);
                            da1.Update(ds_特殊.Tables[0]);

                            sql1 = "select * from 返修仓库出入库明细表 where 1<>1";
                            cmd1 = new SqlCommand(sql1, conn, thrk);
                            da1 = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da1);
                            da1.Update(ds_特殊.Tables[1]);

                            sql1 = "select * from 其他出入库申请主表  where 1<>1";
                            cmd1 = new SqlCommand(sql1, conn, thrk);
                            da1 = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da1);
                            da1.Update(ds_特殊.Tables[2]);
                            sql1 = "select * from 其他出入库申请子表  where 1<>1";
                            cmd1 = new SqlCommand(sql1, conn, thrk);
                            da1 = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da1);
                            da1.Update(ds_特殊.Tables[3]);
                            sql1 = "select * from 其他出库主表  where 1<>1";
                            cmd1 = new SqlCommand(sql1, conn, thrk);
                            da1 = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da1);
                            da1.Update(ds_特殊.Tables[4]);
                            sql1 = "select * from 其他出库子表  where 1<>1";
                            cmd1 = new SqlCommand(sql1, conn, thrk);
                            da1 = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da1);
                            da1.Update(ds_特殊.Tables[5]);

                            sql1 = "select * from 返修出入库申请主表  where 1<>1";
                            cmd1 = new SqlCommand(sql1, conn, thrk);
                            da1 = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da1);
                            da1.Update(ds_特殊.Tables[6]);

                            sql1 = "select * from 返修出入库申请子表  where 1<>1";
                            cmd1 = new SqlCommand(sql1, conn, thrk);
                            da1 = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da1);
                            da1.Update(ds_特殊.Tables[7]);
                            sql1 = "select * from 返修入库主表  where 1<>1";
                            cmd1 = new SqlCommand(sql1, conn, thrk);
                            da1 = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da1);
                            da1.Update(ds_特殊.Tables[8]);
                            sql1 = "select * from 返修入库子表  where 1<>1";
                            cmd1 = new SqlCommand(sql1, conn, thrk);
                            da1 = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da1);
                            da1.Update(ds_特殊.Tables[9]);
                            sql1 = "select * from 返修仓库物料数量表  where 1<>1";
                            cmd1 = new SqlCommand(sql1, conn, thrk);
                            da1 = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da1);
                            da1.Update(dt_库存);

                        }


                        //CZMaster.MasterSQL.Save_DataTable(dt_订单主表, "L销售记录销售订单主表L", strconn);
                        //CZMaster.MasterSQL.Save_DataTable(dt_订单明细, "L销售记录销售订单明细表L", strconn);
                        //CZMaster.MasterSQL.Save_DataTable(dt,"L销售记录成品出库单主表L", strconn);
                        //CZMaster.MasterSQL.Save_DataTable(dt_mx, "L销售记录成品出库单明细表L",strconn);
                        thrk.Commit();
                    }
                    catch (Exception ex)
                    {
                        thrk.Rollback();
                        throw ex;
                    }

                    MessageBox.Show("生效成功");
                    barLargeButtonItem5_ItemClick(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public DataTable fun_库存(string tablename, int i_正负, DataTable T)
        {
            DataTable dt = new DataTable();
            DateTime t = CPublic.Var.getDatetime();
            foreach (DataRow dr in T.Rows)
            {
                if (dr["数量确认"].Equals(true))
                {
                    string sql = string.Format("select * from {0} where 物料编码='{1}' and 仓库号='{2}'", tablename, dr["物料编码"].ToString(), dr["仓库号"]);
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                    {
                        da.Fill(dt);
                    }
                    DataRow[] y = dt.Select(string.Format("物料编码='{0}' and 仓库号='{1}'", dr["物料编码"].ToString(), dr["仓库号"].ToString()));

                    if (y.Length == 0)  //该仓库中没有  
                    {
                        DataRow r_new = dt.NewRow();
                        r_new["GUID"] = System.Guid.NewGuid();

                        r_new["物料编码"] = dr["物料编码"];
                        r_new["物料名称"] = dr["物料名称"];
                        r_new["规格型号"] = dr["规格型号"];
                        //r_new["图纸编号"] = dr["图纸编号"];
                        r_new["出入库时间"] = t;
                        r_new["仓库号"] = dr["仓库号"];
                        r_new["库存总数"] = i_正负 * Convert.ToDecimal(dr["数量"].ToString());
                        r_new["有效总数"] = i_正负 * Convert.ToDecimal(dr["数量"].ToString());

                        r_new["仓库名称"] = dr["仓库名称"];
                        dt.Rows.Add(r_new);


                        //string s = string.Format("select * from 仓库物料数量表 where 物料编码='{0}' and 仓库号='{1}' ", dr["物料编码"].ToString(),dr["仓库号"].ToString());
                        //DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                        //temp.Rows[0]["库存总数"] = 0;
                        //temp.Rows[0]["有效总数"] = 0;
                        //dt.ImportRow(temp.Rows[0]);
                        //DataRow[] x = dt.Select(string.Format("物料编码='{0}' and 仓库号='{1}' ", dr["物料编码"].ToString(), dr["仓库号"].ToString()));
                        //x[0]["库存总数"] = Convert.ToDecimal(x[0]["库存总数"]) + i_正负 * Convert.ToDecimal(dr["数量"].ToString());
                        //x[0]["出入库时间"] = t;
                        //x[0].AcceptChanges();
                        //x[0].SetAdded();
                    }
                    else
                    {
                        DataRow[] x = dt.Select(string.Format("物料编码='{0}' and 仓库号='{1}' ", dr["物料编码"].ToString(), dr["仓库号"].ToString()));
                        x[0]["库存总数"] = Convert.ToDecimal(x[0]["库存总数"]) + i_正负 * Convert.ToDecimal(dr["数量"].ToString());
                        x[0]["有效总数"] = Convert.ToDecimal(x[0]["有效总数"]) + i_正负 * Convert.ToDecimal(dr["数量"].ToString());

                        x[0]["出入库时间"] = t;

                    }

                }

            }

            return dt;
        }
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DataRow dr = dtP.NewRow();
            dtP.Rows.Add(dr);
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            dr.Delete();
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_载入代办();
            time_入库日期.EditValue = CPublic.Var.getDatetime();
            drM = null;
            txt_入库单号.Text = "";
            txt_备注.Text = "";
            //txt_关联出库单.Text = "";
            //txt_关联销售单.Text = "";
            textBox1.Text = "";
            fun_载入主表明细();
            gc.DataSource = dtP;
        }
        #endregion

        DataRow dr_退货申请 = null;
        DataTable dt_退货申请 = null;
        private void gv_代办_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                dr_退货申请 = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);

                //txt_关联出库单.Text = dr_退货申请["关联出库单"].ToString();
                //  txt_关联销售单.Text = dr_退货申请["关联销售单"].ToString();
                textBox1.Text = dr_退货申请["退货类型"].ToString();
                textBox2.Text = dr_退货申请["退货类型"].ToString();
                txt_备注.Text = dr_退货申请["备注"].ToString();
                time_入库日期.EditValue = CPublic.Var.getDatetime();
                //txt_备注.Text = "";
                txt_入库单号.Text = "";
                drM["退货申请单号"] = dr_退货申请["退货申请单号"].ToString();

                dtP.Clear();
                string sql = string.Format(@"select a.*,仓库号,仓库名称,货架描述  from 退货申请子表 a 
                left join 基础数据物料信息表 b  on a.物料编码=b.物料编码
  
                where    退货申请单号 = '{0}'", dr_退货申请["退货申请单号"]);
                dt_退货申请 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_退货申请);
                string s_分类 = "";
                foreach (DataRow r in dt_退货申请.Rows)
                {
                    if (Convert.ToBoolean(r["完成"]))
                    {
                        continue;
                    }

                    DataRow rr = dtP.NewRow();

                    dtP.Rows.Add(rr);
                    rr["物料编码"] = r["物料编码"];
                    dtP.ColumnChanged -= dtP_ColumnChanged;
                    rr["数量确认"] = true;
                    rr["销售明细号"] = r["销售明细号"];
                    rr["出库明细号"] = r["出库明细号"];
                    rr["通知单明细号"] = r["通知单明细号"];

                    rr["数量"] = r["数量"];
                    rr["退货申请单号"] = r["退货申请单号"];
                    rr["退货申请明细号"] = r["退货申请明细号"];
                    if (r["已入库数量"].ToString() == "") r["已入库数量"] = 0;
                    rr["实际数量"] = Convert.ToDecimal(r["数量"]) - Convert.ToDecimal(r["已入库数量"]);
                    rr["可入库数量"] = Convert.ToDecimal(r["数量"]) - Convert.ToDecimal(r["已入库数量"]);
                    rr["税后单价"] = r["税后单价"];
                    rr["税后金额"] = r["税后金额"];
                    //rr["仓库号"] = r["仓库号"];
                    //rr["仓库名称"] = r["仓库名称"];
                    rr["仓库号"] = "96";
                    rr["仓库名称"] = "检验1";
                    rr["货架描述"] = r["货架描述"];
                    dtP.ColumnChanged += dtP_ColumnChanged;
                }
            }
            catch (Exception ex)
            {
                dtP.ColumnChanged += dtP_ColumnChanged;

                MessageBox.Show(ex.Message);
            }
        }

        private void gv_代办_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void barLargeButtonItem4_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("确定打印？", "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {

                DataRow dr = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);
                DataTable dt_dy = dtP.Copy();
                int count = dt_dy.Rows.Count / 9;
                if (dt_dy.Rows.Count % 9 != 0)
                {
                    count++;
                }
                PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                this.printDialog1.Document = this.printDocument1;
                DialogResult drt = this.printDialog1.ShowDialog();
                if (drt == DialogResult.OK)
                {
                    string PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                    ItemInspection.print_FMS.fun_print_退货入库_A5(dr["退货申请单号"].ToString(), CPublic.Var.localUserName, dt_dy, count, false, PrinterName);
                }
            }
        }

    

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);
                string sql_退货申请主 = string.Format("select * from  退货申请主表 where 退货申请单号 = '{0}'", dr["退货申请单号"]);
                DataTable dt_退货申请主 = CZMaster.MasterSQL.Get_DataTable(sql_退货申请主, strconn);
                string sql_退货申请子 = string.Format("select * from  退货申请子表 where 退货申请单号 = '{0}'", dr["退货申请单号"]);
                DataTable dt_退货申请子 = CZMaster.MasterSQL.Get_DataTable(sql_退货申请子, strconn);
                foreach (DataRow dr1 in dt_退货申请子.Rows)
                {
                    if (Convert.ToDecimal(dr1["已入库数量"]) > 0)
                    {
                        throw new Exception("该单据已有入库记录，不能驳回");
                    }
                }
                if (MessageBox.Show("是否确认驳回该单据？", "提示!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    frm_销售退货驳回原因 fm = new frm_销售退货驳回原因(dr);
                    fm.ShowDialog();
                    if (fm.关闭 != 2)
                    {
                        if (fm.flag == true)
                        {
                            if (dt_退货申请主.Rows.Count > 0)
                            {
                                dt_退货申请主.Rows[0]["驳回原因"] = fm.yijian;
                                dt_退货申请主.Rows[0]["生效"] = false;
                                dt_退货申请主.Rows[0]["生效人员编号"] = "";
                                dt_退货申请主.Rows[0]["生效日期"] = DBNull.Value; 
                                dt_退货申请主.Rows[0]["审核"] = false;
                                dt_退货申请主.Rows[0]["审核人"] = "";
                                dt_退货申请主.Rows[0]["审核时间"] = DBNull.Value;
                                dt_退货申请主.Rows[0]["提交审核"] =false;

                            }

                            foreach (DataRow dr_退货申请子 in dt_退货申请子.Rows)
                            {
                                dr_退货申请子["生效"] = false;
                                dr_退货申请子["生效人员编号"] = "";
                                dr_退货申请子["生效日期"] = DBNull.Value;

                            }

                            SqlConnection conn = new SqlConnection(strconn);
                            conn.Open();
                            SqlTransaction ts = conn.BeginTransaction("驳回");
                            try
                            {
                                string sql1 = "select * from 退货申请主表 where 1<>1";
                                SqlCommand cmd1 = new SqlCommand(sql1, conn, ts);
                                SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                                new SqlCommandBuilder(da1);
                                string sql2 = "select * from 退货申请子表 where 1<>1";
                                SqlCommand cmd2 = new SqlCommand(sql2, conn, ts);  
                                SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                                new SqlCommandBuilder(da2);
                                da1.Update(dt_退货申请主);
                                da2.Update(dt_退货申请子);
                                ts.Commit();
                                MessageBox.Show("驳回成功");
                                barLargeButtonItem5_ItemClick(null, null);

                            }
                            catch (Exception ex)
                            {
                                ts.Rollback();
                                throw ex;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_代办_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                if (dt_代办 != null)
                {
                    dr_退货申请 = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);
                    if (dr_退货申请 != null)
                    {


                        //txt_关联出库单.Text = dr_退货申请["关联出库单"].ToString();
                        //  txt_关联销售单.Text = dr_退货申请["关联销售单"].ToString();
                        textBox1.Text = dr_退货申请["退货类型"].ToString();
                        textBox2.Text = dr_退货申请["退货类型"].ToString();
                        txt_备注.Text = dr_退货申请["备注"].ToString();
                        time_入库日期.EditValue = CPublic.Var.getDatetime();
                        //txt_备注.Text = "";
                        txt_入库单号.Text = "";
                        drM["退货申请单号"] = dr_退货申请["退货申请单号"].ToString();

                        dtP.Clear();
                        string sql = string.Format(@"select a.*,仓库号,仓库名称,货架描述  from 退货申请子表 a 
                left join 基础数据物料信息表 b  on a.物料编码=b.物料编码
  
                where    退货申请单号 = '{0}'", dr_退货申请["退货申请单号"]);
                        dt_退货申请 = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                        da.Fill(dt_退货申请);
                        string s_分类 = "";
                        foreach (DataRow r in dt_退货申请.Rows)
                        {
                            if (Convert.ToBoolean(r["完成"]))
                            {
                                continue;
                            }

                            DataRow rr = dtP.NewRow();

                            dtP.Rows.Add(rr);
                            rr["物料编码"] = r["物料编码"];
                            dtP.ColumnChanged -= dtP_ColumnChanged;
                            rr["数量确认"] = true;
                            rr["销售明细号"] = r["销售明细号"];
                            rr["出库明细号"] = r["出库明细号"];
                            rr["通知单明细号"] = r["通知单明细号"];

                            rr["数量"] = r["数量"];
                            rr["退货申请单号"] = r["退货申请单号"];
                            rr["退货申请明细号"] = r["退货申请明细号"];
                            if (r["已入库数量"].ToString() == "") r["已入库数量"] = 0;
                            rr["实际数量"] = Convert.ToDecimal(r["数量"]) - Convert.ToDecimal(r["已入库数量"]);
                            rr["可入库数量"] = Convert.ToDecimal(r["数量"]) - Convert.ToDecimal(r["已入库数量"]);
                            rr["税后单价"] = r["税后单价"];
                            rr["税后金额"] = r["税后金额"];
                            //rr["仓库号"] = r["仓库号"];
                            //rr["仓库名称"] = r["仓库名称"];
                            rr["仓库号"] = "96";
                            rr["仓库名称"] = "检验1";
                            rr["货架描述"] = r["货架描述"];
                            dtP.ColumnChanged += dtP_ColumnChanged;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                dtP.ColumnChanged += dtP_ColumnChanged;


                MessageBox.Show(ex.Message);
            }
        }

        private void gv_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "仓库号")
                {
                    DataRow dr = gv.GetDataRow(e.RowHandle);
                    string s = string.Format("select  * from 仓库物料数量表 where 物料编码='{0}' and 仓库号='{1}'", dr["物料编码"].ToString(), e.Value);
                    DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                    if (temp.Rows.Count > 0)
                    {
                        dr["仓库名称"] = temp.Rows[0]["仓库名称"];
                        dr["货架描述"] = temp.Rows[0]["货架描述"];
                        dr["库存总数"] = temp.Rows[0]["库存总数"];

                    }
                    else
                    {
                        DataRow[] r = dt_仓库.Select(string.Format("仓库号='{0}'", e.Value));
                        dr["仓库名称"] = r[0]["仓库名称"];
                        dr["货架描述"] = "";
                        dr["库存总数"] = 0;
                    }


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
