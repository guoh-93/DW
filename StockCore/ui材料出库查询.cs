using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace StockCore
{
    public partial class ui材料出库查询 : UserControl
    {
        string str_cg = "";
        public ui材料出库查询()
        {
            InitializeComponent();
        }
        public ui材料出库查询(string str)
        {
            InitializeComponent();
            str_cg = str;
            gvM.FindFilterText = str;
            bar_单据状态.EditValue = "所有";


        }

        string strconn = CPublic.Var.strConn;
        DataTable dtM = new DataTable();
        private void ui材料出库查询_Load(object sender, EventArgs e)
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime().Date;
                bar_日期后.EditValue = t.AddDays(1).AddSeconds(-1);
                bar_日期前.EditValue = t.AddMonths(-3);
                if (str_cg != "")
                {
                    string s = string.Format("select  创建日期 from 采购记录采购单主表  where 采购单号 ='{0}' ", str_cg);
                    DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                    if (dt.Rows[0][0] != null && Convert.ToDateTime(dt.Rows[0][0]) < t.AddMonths(-3))
                    {
                        bar_日期前.EditValue = Convert.ToDateTime(dt.Rows[0][0]);
                    }


                }


                if (bar_单据状态.EditValue == null || bar_单据状态.EditValue.ToString() == "")
                    bar_单据状态.EditValue = "未完成";
                fun_载入();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void refresh_single(string s_单号)
        {
            string sql = string.Format(@"select * from 其他出入库申请主表  where 出入库申请单号='{0}'", s_单号);
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            DataRow[] r_1 = dtM.Select(string.Format("出入库申请单号='{0}'", s_单号));
            r_1[0].ItemArray = dt.Rows[0].ItemArray;
        }


        private void fun_载入()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (dtM != null)
                {
                    dtM.Clear();
                }
                string s_组合 = @"select * from (select aa.*,bb.待审核人 as 待审核人_1  from 其他出入库申请主表 aa
                        left join(select 关联单号, 待审核人 from 单据审核申请表 where 单据类型 = '材料出库申请'  group by 关联单号, 待审核人
                        ) bb on aa.出入库申请单号 = bb.关联单号) a {0}";
                string s_组合1 = "where ";

                if (bar_日期前.EditValue != null && bar_日期后.EditValue != null && bar_日期前.EditValue.ToString() != "" && bar_日期后.EditValue.ToString() != "")
                {
                    s_组合1 += " a.申请日期 >= '" + ((DateTime)bar_日期前.EditValue).Date.ToString("yyyy-MM-dd HH:mm:ss") + "'" + " and a.申请日期 <= '" + ((DateTime)bar_日期后.EditValue).Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss") + "' and ";
                }
                if (bar_单据状态.EditValue != null && bar_单据状态.EditValue.ToString() != "")
                {
                    if (bar_单据状态.EditValue.ToString() == "完成")
                    {
                        s_组合1 += "a.完成 = 1 and ";
                    }
                    if (bar_单据状态.EditValue.ToString() == "未完成")
                    {
                        s_组合1 += "a.完成 <>1 and ";
                    }
                    if (bar_单据状态.EditValue.ToString() == "所有")
                    { }
                }
                s_组合1 += "a.单据类型 = '材料出库' and ";
                if (s_组合1 != "where ")
                {
                    s_组合1 = s_组合1.Substring(0, s_组合1.Length - 4);
                    s_组合 = string.Format(s_组合, s_组合1);
                }
                SqlDataAdapter da = new SqlDataAdapter(s_组合, strconn);
                da.Fill(dtM);
                gcM.DataSource = dtM;

                string x = @"select  mx.*,a.申请类型,a.项目名称,a.业务单号,a.备注 as 表头备注,a.单据类型,a.原因分类,a.待审核 as 提交审核,a.审核  
                ,a.操作人员  from 其他出入库申请子表 mx   left join 其他出入库申请主表 a on mx.出入库申请单号=a.出入库申请单号 " + s_组合1;
                DataTable t_P = CZMaster.MasterSQL.Get_DataTable(x, strconn);
                gridControl1.DataSource = t_P;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ERPorg.Corg.FlushMemory();
            fun_载入();
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show(string.Format("是否确认撤销提交？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                    if (dr == null)
                        throw new Exception("请先选择需要作废的记录");
                    if (dr["完成"].ToString().ToLower() == "true")
                        throw new Exception("该记录已完成，不需要作废");
                    if (dr["待审核"].ToString().ToLower() == "true")
                        throw new Exception("该记录已提交审核，不可作废");
                    dr["作废"] = true;
                    dr["作废日期"] = System.DateTime.Now;
                    dr["作废人员编号"] = CPublic.Var.LocalUserID;

                    string sql = string.Format("select * from 其他出入库申请子表 where 出入库申请单号 = '{0}'", dr["出入库申请单号"]);
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt);
                    foreach (DataRow r in dt.Rows)
                    {
                        dr["作废"] = true;
                        dr["作废日期"] = System.DateTime.Now;
                        dr["作废人员编号"] = CPublic.Var.LocalUserID;
                    }
                    sql = "select * from 其他出入库申请子表 where 1<> 1";
                    da = new SqlDataAdapter(sql, strconn);
                    new SqlCommandBuilder(da);
                    da.Update(dt);

                    sql = "select * from 其他出入库申请主表 where 1<> 1";
                    da = new SqlDataAdapter(sql, strconn);
                    new SqlCommandBuilder(da);
                    da.Update(dtM);
                    MessageBox.Show("已作废:" + dr["出入库申请单号"].ToString());
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                DataRow drM = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;
                if (!Convert.ToBoolean(drM["审核"])) throw new Exception("未审核单据,不可打印");
                DataTable dtm = (DataTable)this.gcP.DataSource;
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
                Type outerForm = outerAsm.GetType("ERPreport.Form其他出入库申请", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
                object[] drr = new object[3];
                drr[0] = drM;
                drr[1] = dtm;
                drr[2] = "材料出库申请";
                //   drr[2] = dr["出入库申请单号"].ToString();
                Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                //  UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
                ui.ShowDialog();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gvM_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Clicks == 1 && e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                    if (dr == null) return;
                    refresh_single(dr["出入库申请单号"].ToString());
                    if (CPublic.Var.localUserName == dr["操作人员"].ToString() || CPublic.Var.LocalUserTeam.Contains("管理员"))
                    {
                        barLargeButtonItem7.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    }
                    else
                    {
                        barLargeButtonItem7.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    }
                    if (CPublic.Var.localUserName == dr["审核人员"].ToString() || CPublic.Var.LocalUserTeam.Contains("管理员"))
                    {
                        barLargeButtonItem8.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    }
                    else
                    {
                        barLargeButtonItem8.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    }


                    string sql = "";
                    if (dr["申请类型"].ToString() == "材料出库")
                    {
                        sql = string.Format(@"select 其他出入库申请子表.*,isnull(已处理数量,0)已处理数量  from 其他出入库申请子表
                        left  join (select  出入库申请明细号,sum(数量)as 已处理数量 from  其他出库子表 where 生效=1 group by  出入库申请明细号)a 
                        on a.出入库申请明细号=其他出入库申请子表.出入库申请明细号
                        where 其他出入库申请子表.出入库申请单号 = '{0}' order by 其他出入库申请子表.POS", dr["出入库申请单号"]);
                    }
                    else
                    {
                        sql = string.Format(@"select 其他出入库申请子表.*,isnull(已处理数量,0)已处理数量 from 其他出入库申请子表
                        left  join (select  出入库申请明细号,sum(数量)as 已处理数量 from  其他入库子表 where 生效=1 group by  出入库申请明细号)a 
                        on a.出入库申请明细号=其他出入库申请子表.出入库申请明细号
                        where 其他出入库申请子表.出入库申请单号 = '{0}' order by 其他出入库申请子表.POS", dr["出入库申请单号"]);
                    }
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt);
                    gcP.DataSource = dt;
                }

                if (e != null && e.Button == MouseButtons.Right)
                {
                    DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                    refresh_single(dr["出入库申请单号"].ToString());
                    contextMenuStrip1.Show(gcM, new Point(e.X, e.Y));
                    gvM.CloseEditor();
                    this.BindingContext[dtM].EndCurrentEdit();

                    if (dr["申请类型"].ToString() == "材料出库")
                    {
                        归还2ToolStripMenuItem.Visible = true;
                    }
                    else
                    {
                        归还2ToolStripMenuItem.Visible = false;
                    }
                    if (dr["原因分类"].ToString() == "委外加工")
                    {
                        contextMenuStrip1.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                string sql = string.Format("select * from 其他出入库申请主表 where 出入库申请单号 = '{0}' ", dr["出入库申请单号"].ToString());
                DataTable dt11 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                if (dt11.Rows[0]["待审核"].Equals(true))
                {
                    throw new Exception("该记录已提交审核");
                }
                if (dt11.Rows[0]["作废"].Equals(true))
                {
                    throw new Exception("该记录已作废");
                }
                if (dt11.Rows[0]["完成"].Equals(true))
                {
                    MessageBox.Show("该记录已完成不能修改");
                }
                else
                {
                    StockCore.ui材料出库申请 frm = new ui材料出库申请(dr);
                    CPublic.UIcontrol.Showpage(frm, "申请明细");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 归还2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                string s = dr["出入库申请单号"].ToString();
                string s1 = dr["原因分类"].ToString();
                string sql_归还 = string.Format(@"select aa.*,isnull(aaa.申请归还总数,0)申请归还总数,isnull(aaa.实还数量,0)实还数量,isnull(已处理数量,0)已处理数量 from 其他出入库申请子表 aa
                                                left join
                                                (
                                                select  x.物料编码,sum(x.申请归还总数)申请归还总数,x.仓库号,x.仓库名称,x.货架描述,x.业务单号,sum(y.实还数量)实还数量 from (
                                                select a.出入库申请单号,a.出入库申请明细号,a.物料编码,sum(a.数量)申请归还总数,a.仓库号,a.仓库名称,a.货架描述,业务单号  from 其他出入库申请子表  a
                                                left join 其他出入库申请主表 b  on   a.出入库申请单号 =b.出入库申请单号 
                                                where 业务单号 ='{0}'  group by a.出入库申请单号,a.出入库申请明细号,a.物料编码,a.仓库号,a.仓库名称,a.货架描述,业务单号)x
                                                left join (
                                                select a.出入库申请单号,a.出入库申请明细号,SUM(rk.数量)实还数量,a.物料编码,a.仓库号,a.仓库名称,a.货架描述,业务单号  from 其他入库子表 rk
                                                left join 其他出入库申请子表 a on rk.出入库申请明细号=a.出入库申请明细号
                                                left join 其他出入库申请主表 b on  a.出入库申请单号 =b.出入库申请单号 
                                                where 业务单号 ='{0}' group by a.出入库申请单号,a.出入库申请明细号,a.物料编码,a.仓库号,a.仓库名称,a.货架描述,业务单号 )y
                                                on x.出入库申请明细号=y.出入库申请明细号
                                                group by x.物料编码,x.仓库号,x.仓库名称,x.货架描述,x.业务单号) aaa

                                                on aa.物料编码 = aaa.物料编码 and aa.仓库号 = aaa.仓库号
                                                left  join (select  出入库申请明细号,sum(数量)as 已处理数量 from  其他出库子表 where 生效=1 group by  出入库申请明细号)abc 
                                                on abc.出入库申请明细号=aa.出入库申请明细号  
                                                 where 出入库申请单号 = '{0}'", dr["出入库申请单号"].ToString());
                DataTable dt_明细 = CZMaster.MasterSQL.Get_DataTable(sql_归还, strconn);
                string sql = @"select a.*,库存总数,b.仓库名称 from 其他出入库申请子表 a ,仓库物料数量表 b ,基础数据物料信息表 c
                        where   a.物料编码=b.物料编码 and 
                            a.物料编码=c.物料编码 and 1<>1";
                DataTable dtP = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                DataTable dt_仓 = new DataTable();
                foreach (DataRow dr_出入库子 in dt_明细.Rows)
                {
                    //19-6-17申请子表增加 已完成数量  以处理数量替换成已完成数量
                    if (Convert.ToDecimal(dr_出入库子["已完成数量"]) != 0 && Convert.ToDecimal(dr_出入库子["已完成数量"]) > Convert.ToDecimal(dr_出入库子["申请归还总数"]))
                    {
                        string sql111 = "select * from 仓库物料数量表 where 物料编码 = '" + dr_出入库子["物料编码"] + "' and 仓库号 = '" + dr_出入库子["仓库号"] + "'";
                        dt_仓 = CZMaster.MasterSQL.Get_DataTable(sql111, strconn);
                        DataRow dr11 = dtP.NewRow();
                        dtP.Rows.Add(dr11);
                        dr11["物料编码"] = dr_出入库子["物料编码"];
                        dr11["物料名称"] = dr_出入库子["物料名称"];
                        dr11["规格型号"] = dr_出入库子["规格型号"];
                        dr11["货架描述"] = dr_出入库子["货架描述"];
                        if (dt_仓.Rows.Count == 0)
                        {
                            dr["库存总数"] = 0;

                        }
                        else
                        {
                            dr11["库存总数"] = dt_仓.Rows[0]["库存总数"];
                        }
                        dr11["仓库号"] = dr_出入库子["仓库号"];
                        dr11["仓库名称"] = dr_出入库子["仓库名称"];
                        dr11["数量"] = Convert.ToDecimal(dr_出入库子["已完成数量"]) - Convert.ToDecimal(dr_出入库子["申请归还总数"]);
                    }
                }
                if (dr["作废"].Equals(true))
                {
                    throw new Exception("该记录已作废");

                }
                if (dr["红字回冲"].Equals(true))
                {
                    throw new Exception("该单据是红字回冲单，不可归还");

                }
                if (dtP.Rows.Count <= 0)
                {
                    throw new Exception("该单据无可做归还操作的物料！");
                }



                else
                {
                    StockCore.ui材料出库申请 frm = new ui材料出库申请(s, s1, dt_明细);
                    CPublic.UIcontrol.Showpage(frm, "申请明细");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void gvM_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                string sql = "";
                if (dr["申请类型"].ToString() == "材料出库")
                {
                    sql = string.Format(@"select 其他出入库申请子表.*,isnull(已处理数量,0)已处理数量  from 其他出入库申请子表
                        left  join (select  出入库申请明细号,sum(数量)as 已处理数量 from  其他出库子表 where 生效=1 group by  出入库申请明细号)a 
                        on a.出入库申请明细号=其他出入库申请子表.出入库申请明细号
                        where 其他出入库申请子表.出入库申请单号 = '{0}' order by 其他出入库申请子表.POS ", dr["出入库申请单号"]);
                }
                else
                {
                    sql = string.Format(@"select 其他出入库申请子表.*,isnull(已处理数量,0)已处理数量 from 其他出入库申请子表
                        left  join (select  出入库申请明细号,sum(数量)as 已处理数量 from  其他入库子表 where 生效=1 group by  出入库申请明细号)a 
                        on a.出入库申请明细号=其他出入库申请子表.出入库申请明细号
                        where 其他出入库申请子表.出入库申请单号 = '{0}' order by 其他出入库申请子表.POS", dr["出入库申请单号"]);
                }
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                gcP.DataSource = dt;
            }
            catch
            {


            }
        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show(string.Format("是否确认撤销提交？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                    if (dr == null)
                        throw new Exception("请先选择需要撤销提交的记录");
                    if (dr["完成"].ToString().ToLower() == "true")
                        throw new Exception("该记录已完成，不可撤销提交");
                    if (dr["审核"].ToString().ToLower() == "true")
                        throw new Exception("该记录已审核，不可撤销提交");
                    string sql = $"select * from 其他出入库申请主表 where 出入库申请单号 = '{dr["出入库申请单号"]}'";
                    DataTable dt_出入库申请主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    string sql_1 = $"select * from 单据审核申请表 where 审核=0 and 作废=0 and 单据类型 ='材料出库申请' and  关联单号 = '{dr["出入库申请单号"]}'";
                    DataTable dt_单据审核 = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);
                    if (dt_出入库申请主.Rows.Count > 0)
                    {
                        dt_出入库申请主.Rows[0]["待审核"] = false;
                        if (dt_单据审核.Rows.Count > 0)
                        {
                            dt_单据审核.Rows[0].Delete();
                        }

                        SqlConnection conn = new SqlConnection(strconn);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("pur"); //事务的名称
                        SqlCommand cmd1 = new SqlCommand("select * from 其他出入库申请主表 where 1<>1", conn, ts);
                        SqlCommand cmd = new SqlCommand("select * from 单据审核申请表 where 1<>1", conn, ts);

                        try
                        {
                            SqlDataAdapter da;
                            da = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da);
                            da.Update(dt_出入库申请主);
                            da = new SqlDataAdapter(cmd);
                            new SqlCommandBuilder(da);
                            da.Update(dt_单据审核);
                            ts.Commit();
                            MessageBox.Show("撤销提交成功");
                            barLargeButtonItem1_ItemClick(null, null);
                        }
                        catch
                        {
                            ts.Rollback();
                            throw new Exception("提交出错了,请刷新后重试");
                        }
                    }



                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //弃审
        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                string s = $"select * from 其他出入库申请主表 where 审核=1 and 完成=0 and 作废=0 and 出入库申请单号='{dr["出入库申请单号"].ToString()}'";
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                if (temp.Rows.Count == 0) throw new Exception("单据状态有误请确认");
                else
                {
                    s = $"select  * from 其他出入库申请子表 where  出入库申请单号='{dr["出入库申请单号"].ToString()}' and 已完成数量>0";
                    temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                    if (temp.Rows.Count > 0)
                    {
                        throw new Exception("已有出库不可弃审");
                    }
                }
                s = $"update 其他出入库申请主表 set 生效=0,生效日期=null,生效人员编号='',待审核=0,审核=0,审核人员='',审核人员ID='',审核日期=null where 出入库申请单号='{dr["出入库申请单号"].ToString()}' ";
                CZMaster.MasterSQL.ExecuteSQL(s, strconn);
                MessageBox.Show("弃审完成");
                refresh_single(dr["出入库申请单号"].ToString());

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (ActiveControl != null && ActiveControl.GetType().Equals(gridControl1.GetType()))
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Title = "导出Excel";
                    saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                    DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                    if (dialogResult == DialogResult.OK)
                    {
                        DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                        DevExpress.XtraGrid.GridControl gc = (ActiveControl) as DevExpress.XtraGrid.GridControl;

                        gc.ExportToXlsx(saveFileDialog.FileName);
                        DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                }
                else
                {

                    MessageBox.Show("若要导出请先选中要导出的表格");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
