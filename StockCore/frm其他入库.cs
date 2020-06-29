using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace StockCore
{
    public partial class frm其他入库 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dtM;
        DataTable dtP;
        DataRow drM = null;
        DataTable dt_物料;
        DataTable dt_仓库;
        DataTable dt_人员;
        DataTable dt_代办;
        string sql_ck = "";
        frm其他出库 fm = new frm其他出库();

        #endregion

        #region 自用类
        public frm其他入库()
        {
            InitializeComponent();
            fun_物料下拉框();
        }

        public frm其他入库(DataRow dr)
        {
            InitializeComponent();
            drM = dr;
            fun_物料下拉框();
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm其他入库_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();
                //DateTime t = Convert.ToDateTime("2019-06-18 10:10:40.207");
                time_入库日期.EditValue = t;
                fun_人员();
                fun_仓库();
                fun_载入代办();
                fun_载入主表明细();
                gc.DataSource = dtP;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_仓库()
        {
            dt_仓库 = new DataTable();
            string sql4 = "select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别'";
            SqlDataAdapter da = new SqlDataAdapter(sql4, strconn);
            da.Fill(dt_仓库);
            repositoryItemSearchLookUpEdit2.DataSource = dt_仓库;
            repositoryItemSearchLookUpEdit2.DisplayMember = "仓库号";
            repositoryItemSearchLookUpEdit2.ValueMember = "仓库号";
        }
        #endregion

        #region 方法
#pragma warning disable IDE1006 // 命名样式
        private void fun_载入主表明细()
#pragma warning restore IDE1006 // 命名样式
        {
            if (drM == null)
            {
                string sql = "select * from 其他入库主表 where 1<>1";
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);

                drM = dtM.NewRow();
                dtM.Rows.Add(drM);

                sql = "select * from 其他入库子表 where 1<>1";
                dtP = new DataTable();
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtP);
                dtP.Columns.Add("库存总数",typeof(decimal));
                dtP.Columns.Add("货架描述");
                dtP.Columns.Add("仓库号");
                dtP.Columns.Add("仓库名称");
     




            }
            else
            {
                string sql = string.Format("select * from 其他入库主表 where 其他入库单号 = '{0}'", drM["其他入库单号"].ToString());
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);

                drM = dtM.Rows[0];
                dataBindHelper1.DataFormDR(drM);

                txt_入库人员ID.EditValue = drM["入库人员编号"];
                txt_入库人员.Text = drM["入库人员"].ToString();

                string sql2 = string.Format(@"select 其他入库子表.*,仓库物料数量表.库存总数 from 其他入库子表 left join 仓库物料数量表 on 其他入库子表.物料编码 = 仓库物料数量表.物料编码
                where 其他入库单号 = '{0}'", drM["其他入库单号"].ToString());
                dtP = new DataTable();
                SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
                da2.Fill(dtP);
            }
            dtP.Columns.Add("数量确认",typeof(bool));
            dtP.ColumnChanged += dtP_ColumnChanged;
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_保存主表明细(Boolean bl)
#pragma warning restore IDE1006 // 命名样式
        {
            DateTime t = CPublic.Var.getDatetime();
 
             //DateTime t = Convert.ToDateTime("2019-7-29 10:10:40.207");
    
            try
            {
                if (drM["GUID"].ToString() == "")
                {
                    drM["GUID"] = System.Guid.NewGuid();
                    txt_入库单号.Text = string.Format("QW{0}{1}{2}{3}",t.Year.ToString(),t.Month.ToString("00"),
                        t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("QW", t.Year,t.Month).ToString("0000"));
                    drM["其他入库单号"] = txt_入库单号.Text;
                    drM["创建日期"] = t;
                }
                drM["操作人员编号"] = CPublic.Var.LocalUserID;
                drM["操作人员"] = CPublic.Var.localUserName;
                drM["入库仓库"] = "";
                drM["入库人员编号"] = txt_入库人员ID.EditValue;
                drM["入库人员"] = txt_入库人员.Text;
                if (bl == true)
                {
                    drM["生效"] = true;
                    drM["生效人员编号"] = CPublic.Var.LocalUserID;
                    drM["生效日期"] = t;
                }
                dataBindHelper1.DataToDR(drM);
            }
            catch (Exception ex)
            {
                throw new Exception("主表保存出错" + ex.Message);
            }

            try
            {
                int i = 1;
                foreach (DataRow r in dtP.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    if (r["GUID"].ToString() == "")
                    {
                        r["GUID"] = System.Guid.NewGuid();
                        r["其他入库单号"] = drM["其他入库单号"];
                        r["其他入库明细号"] = drM["其他入库单号"].ToString() + "-" + i.ToString("00");
                        r["POS"] = i++;
                    }
                    if (bl == true)
                    {
                        r["生效"] = true;
                        r["生效人员编号"] = CPublic.Var.LocalUserID;
                        r["生效日期"] =t;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("明细保存出错" + ex.Message);
            }
            fun_判断出库申请();
            //仓库出入库记录中存一条记录
            DataTable dt_出入库 = fun_保存记录到出入库明细();

            //DataTable dt_库存=fm.fun_库存(1, dtP);
            DataTable dt_库存 = ERPorg.Corg.fun_库存(1,dtP);


            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("生效");
            string sql1 = "select * from 其他入库主表 where 1<>1";
            SqlCommand cmd1 = new SqlCommand(sql1, conn, ts);
            SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
            new SqlCommandBuilder(da1);
            string sql2 = "select * from 其他入库子表 where 1<>1";
            SqlCommand cmd2 = new SqlCommand(sql2, conn, ts);
            SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
            new SqlCommandBuilder(da2);
            string sql3 = "select * from 其他出入库申请主表 where 1<>1";
            SqlCommand cmd3 = new SqlCommand(sql3, conn, ts);
            SqlDataAdapter da3 = new SqlDataAdapter(cmd3);
            new SqlCommandBuilder(da3);
            string sql4 = "select * from 其他出入库申请子表 where 1<>1";
            SqlCommand cmd4 = new SqlCommand(sql4, conn, ts);
            SqlDataAdapter da4 = new SqlDataAdapter(cmd4);
            new SqlCommandBuilder(da4);
            string sql5= "select * from 仓库物料数量表 where 1<>1";
            SqlCommand cmd5 = new SqlCommand(sql5, conn, ts);
            SqlDataAdapter da5= new SqlDataAdapter(cmd5);
            new SqlCommandBuilder(da5);
            string sql6 = "select * from 仓库出入库明细表 where 1<>1";
            SqlCommand cmd6 = new SqlCommand(sql6, conn, ts);
            SqlDataAdapter da6 = new SqlDataAdapter(cmd6);
            new SqlCommandBuilder(da6);
            try
            {
                da1.Update(dtM);
                da2.Update(dtP);
                da3.Update(dt_代办);
                da4.Update(dt_出库申请);
                da5.Update(dt_库存);
                da6.Update(dt_出入库);

                ts.Commit();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void dtP_ColumnChanged(object sender, DataColumnChangeEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //try
            //{
            //    if (e.Column.Caption == "物料编码")
            //    {
            //        DataRow[] ds = dt_物料.Select(string.Format("物料编码 = '{0}'", e.Row["物料编码"]));
            //       // e.Row["原ERP物料编号"] = ds[0]["原ERP物料编号"];
            //        e.Row["物料名称"] = ds[0]["物料名称"];
            //        e.Row["规格型号"] = ds[0]["规格型号"];
            //        //e.Row["图纸编号"] = ds[0]["图纸编号"];
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_物料下拉框()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = @"select base.物料编码,base.物料名称,
            base.图纸编号,kc.库存总数,kc.仓库名称,kc.仓库号,kc.货架描述 from 基础数据物料信息表 base
            left join 仓库物料数量表 kc on base.物料编码 = kc.物料编码";
            dt_物料 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_物料);

            repositoryItemSearchLookUpEdit1.DataSource = dt_物料;
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
        }

#pragma warning disable IDE1006 // 命名样式
        private DataTable  fun_保存记录到出入库明细()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = "select * from 仓库出入库明细表 where 1<>1";
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
               
                DateTime t = CPublic.Var.getDatetime();
                //DateTime t = Convert.ToDateTime("2019-06-18 10:10:40.207");
                //DateTime t = DateTime.Now;

                foreach (DataRow r in dtP.Rows)
                {
                    if (r["数量确认"].ToString().ToLower() == "true")
                    {
                        DataRow dr = dt.NewRow();
                        dr["GUID"] = System.Guid.NewGuid();
                        dr["明细类型"] = "其他入库";
                        dr["单号"] = r["其他入库单号"].ToString();
                        dr["物料编码"] = r["物料编码"].ToString();
                        dr["物料名称"] = r["物料名称"].ToString();
                        dr["明细号"] = r["其他入库明细号"].ToString();
                        dr["相关单号"] = r["出入库申请单号"].ToString();
                        dr["仓库号"] = r["仓库号"].ToString();
                        dr["仓库名称"] = r["仓库名称"].ToString(); 
                        dr["出库入库"] = "入库";

                        string sql_1 = string.Format("select * from 人事基础员工表 where 员工号='{0}'", txt_入库人员ID.EditValue);
                        DataTable dt_xg = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);

                        dr["相关单位"] = dt_xg.Rows[0]["课室"];
                        dr["数量"] = (Decimal)0;
                        dr["标准数量"] = (Decimal)0;
                        dr["实效数量"] = Convert.ToDecimal(r["数量"].ToString());
                        dr["实效时间"] = t;
                        dr["出入库时间"] = t;
                        dr["仓库人"] =CPublic.Var.localUserName;


                        dt.Rows.Add(dr);
                    }
                }
                //new SqlCommandBuilder(da);
                //da.Update(dt);
                return dt;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm其他入库_fun_保存出入库明细");
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_人员()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format(@"select 员工号,姓名 from 人事基础员工表 where 在职状态 = '在职'");
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            dt_人员 = new DataTable();
            da.Fill(dt_人员);
            txt_入库人员ID.Properties.DataSource = dt_人员;
            txt_入库人员ID.Properties.DisplayMember = "员工号";
            txt_入库人员ID.Properties.ValueMember = "员工号";
        }

#pragma warning disable IDE1006 // 命名样式
        private void txt_入库人员ID_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (txt_入库人员ID.EditValue != null && txt_入库人员ID.EditValue.ToString() == "")
            {
              
                txt_入库人员.Text = "";
            }
            else
            {
                DataRow[] ds = dt_人员.Select(string.Format("员工号 = '{0}'", txt_入库人员ID.EditValue));
                if (ds.Length > 0)
                {
                    txt_入库人员.Text = ds[0]["姓名"].ToString();
                }
                else
                {
                    txt_入库人员.Text = "";
                }  
            }
        }
        #endregion

        #region 界面操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //新增
            try
            {
                time_入库日期.EditValue = System.DateTime.Now;
                drM = null;
                txt_入库单号.Text = "";
                txt_入库人员.Text = "";
                txt_入库仓库.Text = "";
                txt_入库人员ID.EditValue = "";
                txt_备注.Text = "";

                fun_载入主表明细();
                gc.DataSource = dtP;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //保存
            try
            {
                if (MessageBox.Show("确认保存吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    gv.CloseEditor();
                    this.BindingContext[dtP].EndCurrentEdit();
                    fun_保存主表明细(false);
                    MessageBox.Show("保存成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //生效
            try
            {
                gv.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();

                DataView dv = new DataView(dtP);
                dv.RowFilter = "数量确认=true";
                if (dv.Count == 0)
                {
                    throw new Exception("未选中任何明细");
                }

                for (int i = 0; i < dtP.Rows.Count; i++)
                {
                    if (dtP.Rows[i]["数量确认"].Equals(true))
                    {
                        continue;
                    }
                    else
                    {
                        dtP.Rows.Remove(dtP.Rows[i]);
                        i--;
                    }
                }
                //必须数量确认
                foreach (DataRow dr in dtP.Rows)
                {
                    
                    if (Convert.ToBoolean( dr["数量确认"]))
                    {
                        dr["存货核算标记"] = 1;
                        dr["结算单价"] = 0;
                        continue;
                    }
                    else
                    {
                        throw new Exception("请先确认_" + dr["物料编码"].ToString() + "_数量" );
                    }
               


                }

                fun_保存主表明细(true);  //并判断出入库申请是否完成
      
            
                foreach (DataRow r in dtP.Rows)
                {
                    if (r["数量确认"].ToString().ToLower() == "true")
                    {
                        //郭恒 2016-11-11 入库 刷新库存
                        //StockCore.StockCorer.fun_刷新库存(r["物料编码"].ToString(), Convert.ToDecimal(r["数量"]), 1);
                        StockCore.StockCorer.fun_物料数量_实际数量(r["物料编码"].ToString(),r["仓库号"].ToString(), true);
                    }
                }
                MessageBox.Show("生效成功");
                drM = null;
                txt_入库单号.Text = "";

                txt_入库人员ID.EditValue = null;
                txt_备注.Text = "";
                txt_入库人员.Text = "";
     
                txt_入库仓库.Text = ""
                    ;
                frm其他入库_Load(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = dtP.NewRow();
            dtP.Rows.Add(dr);
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            dr.Delete();
        }
        #endregion

        #region 代办
#pragma warning disable IDE1006 // 命名样式
        private void fun_载入代办()
#pragma warning restore IDE1006 // 命名样式
        {
             sql_ck = string.Format("select * from 人员仓库对应表 where 工号='{0}'", CPublic.Var.LocalUserID);
            DataTable dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_ck, strconn);
            sql_ck = "and a.仓库号  in(";
            // string sql_左 = "";
            string sql = "";


            if (CPublic.Var.LocalUserTeam == "管理员权限"||CPublic.Var.LocalUserID=="910173"|| CPublic.Var.LocalUserTeam == "财务部权限")
            {
                sql = "select * from 其他出入库申请主表 where 生效 = 1 and (完成=0 or 完成 is null) and (作废 = 0 or 作废 is null) and 申请类型 = '其他入库'";

            }
            else if (dt_仓库.Rows.Count > 0)

            {
                foreach (DataRow dr in dt_仓库.Rows)
                {
                    sql_ck = sql_ck + string.Format("'{0}',", dr["仓库号"]);

                }
                sql_ck = sql_ck.Substring(0, sql_ck.Length - 1) + ")";
                sql = string.Format(@"select 其他出入库申请主表.* from 其他出入库申请主表 where 生效 = 1 and (完成=0 or 完成 is null)
                            and (作废 = 0 or 作废 is null) and 申请类型 = '其他入库' and 出入库申请单号 in
                        ( select 出入库申请单号  from 其他出入库申请子表 a,基础数据物料信息表 b where (完成=0 or 完成 is null)  and     
                         b.物料编码=a.物料编码 {0} group by 出入库申请单号 )  ", sql_ck);

            }
            else
            {
                throw new Exception("未维护所管仓库信息");
            }
            dt_代办 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_代办);
            gc_代办.DataSource = dt_代办;
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_判断出库申请()
#pragma warning restore IDE1006 // 命名样式
        {
    
            DateTime t = CPublic.Var.getDatetime();
            //DateTime t = Convert.ToDateTime("2019-06-18 10:10:40.207");
            string sql = string.Format("select * from 其他出入库申请子表 where 出入库申请单号 = '{0}'  and 完成=0", drM["出入库申请单号"]);
            //未完成的
            dt_出库申请 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_出库申请);


            DataView dv = new DataView(dtP);
            dv.RowFilter = "数量确认=1";
            if (dv.Count == dt_出库申请.Rows.Count)
            {
                DataRow dr_申请 = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);
                dr_申请["完成"] = true;
                dr_申请["完成日期"] = t;
   

            }

            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                DataRow[] ds = dt_出库申请.Select(string.Format("出入库申请明细号 = '{0}'", dr["出入库申请明细号"]));
                ds[0]["已完成数量"] = Convert.ToDecimal(ds[0]["已完成数量"]) + Convert.ToDecimal(dr["数量"]);

                if (dr["数量确认"].Equals(true))
                {

                    ds[0]["完成"] = true;
                    ds[0]["完成日期"] = t;
                }
            }
         
        }

        DataRow dr_出库申请 = null;
        DataTable dt_出库申请 = null;
#pragma warning disable IDE1006 // 命名样式
        private void gv_代办_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                dr_出库申请 = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);
                if (dr_出库申请 == null) return;
                txt_入库人员ID.EditValue = dr_出库申请["操作人员编号"].ToString();
                drM["出入库申请单号"] = dr_出库申请["出入库申请单号"];
                txt_备注.Text = "";
                txt_入库仓库.Text = "";
                txt_入库单号.Text = "";

                dtP.Clear();
                if (CPublic.Var.LocalUserTeam == "管理员权限" || CPublic.Var.LocalUserID == "910173" || CPublic.Var.LocalUserTeam == "财务部权限")
                {
                    sql_ck = "";
                }
                string sql = string.Format(@"select a.*,isnull(库存总数,0)库存总数 from 其他出入库申请子表 a 
                                             left join 仓库物料数量表 b on a.仓库号 = b.仓库号 and  a.物料编码=b.物料编码 
                                              where   a.完成=0   and  出入库申请单号 = '{0}' {1}", dr_出库申请["出入库申请单号"],sql_ck);
                dt_出库申请 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_出库申请);
                foreach (DataRow r in dt_出库申请.Rows)
                {
                    if (r["仓库号"].Equals(""))
                    {
                        txt_入库仓库.Text = "";
                    }
                    else
                    {
                        txt_入库仓库.Text =r["仓库名称"].ToString();
                    }
                    
                    DataRow rr = dtP.NewRow();
                    dtP.Rows.Add(rr);
                    rr["物料编码"] = r["物料编码"];
                    rr["物料名称"] = r["物料名称"];
                    rr["规格型号"] = r["规格型号"];
                    rr["数量"] = r["数量"];
                    rr["仓库号"] = r["仓库号"];
                    rr["仓库名称"] = r["仓库名称"];
                    rr["库存总数"] = r["库存总数"];
                    rr["货架描述"] = r["货架描述"];                  
                    rr["出入库申请单号"] = r["出入库申请单号"];
                    rr["出入库申请明细号"] = r["出入库申请明细号"];
                    rr["备注"] = r["备注"];
                
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (MessageBox.Show("确定打印？", "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {

                DataRow dr = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);
                DataTable dt_dy = dtP.Copy();
                int count = dt_dy.Rows.Count / 14;
                if (dt_dy.Rows.Count % 14 != 0)
                {
                    count++;
                }
                   PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                this.printDialog1.Document = this.printDocument1;
                DialogResult drt = this.printDialog1.ShowDialog();
                if (drt == DialogResult.OK)
                {
                     string PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                     ItemInspection.print_FMS.fun_print_其他出库_A5(CPublic.Var.localUserName,dr["出入库申请单号"].ToString(), dt_dy, count, true, PrinterName);
                }
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            drM = null;
            frm其他入库_Load(null, null);
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_代办_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void button1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            foreach (DataRow dr in dtP.Rows)
            {
                dr["数量确认"] = true;

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

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {


            try
            {

                DataRow dr = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);
                DataTable dt_dy = dtP.Copy();
                string sql = string.Format(@"select 其他出库单号 as 编号,申请类型,a.操作人员,部门,a.备注,b.生效日期 from 其他出入库申请主表  a
                        left join 人事基础员工表 on  a.操作人员编号=人事基础员工表.员工号 
                        left join 其他出库主表 b on a.出入库申请单号=b.出入库申请单号 where    a.出入库申请单号='{0}'  ", dr["出入库申请单号"].ToString());
                System.Data.DataTable dt = new System.Data.DataTable();
                new SqlDataAdapter(sql, CPublic.Var.strConn).Fill(dt);
                dt_dy.Columns.Add("计量单位", typeof(string));
                // dt_dy.Columns.Add("仓库名称", typeof(decimal));
                //  dt_dy.Columns.Add("货架描述", typeof(decimal));
                foreach (DataRow r in dt_dy.Rows)
                {

                    string sql_1 = string.Format(@"select 计量单位,kc.仓库名称,库存总数,kc.货架描述 from 基础数据物料信息表 base,仓库物料数量表 kc
                                        where base.物料编码=kc.物料编码 and  kc.物料编码='{0}'", r["物料编码"].ToString());
                    System.Data.DataTable dt_1 = new System.Data.DataTable();
                    dt_1 = CZMaster.MasterSQL.Get_DataTable(sql_1, CPublic.Var.strConn);
                    if (dt_1.Rows.Count > 0)
                    {


                        //   r["当前库存"]= Convert.ToDecimal(dt_1.Rows[0]["库存总数"].ToString()) - Convert.ToDecimal(r["数量"].ToString());

                        r["计量单位"] = dt_1.Rows[0]["计量单位"].ToString();


                        r["仓库名称"] = dt_1.Rows[0]["仓库名称"].ToString();


                        r["货架描述"] = dt_1.Rows[0]["货架描述"].ToString();



                    }




                }

                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
                Type outerForm = outerAsm.GetType("ERPreport.Form其他入库打印", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统

                object[] drr = new object[2];
                
                drr[0] = dt;
                drr[1] = dt_dy;
             //   drr[2] = dr["出入库申请单号"].ToString();
                Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                //  UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
                ui.ShowDialog();
                // CPublic.UIcontrol.Showpage(ui, t.Rows[0]["打开界面名称"].ToString());


                // ERPreport.frm其他出库打印 frm = new ERPreport.frm送货单(dt, ttt, dtP);
                // frm.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void gv_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
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
                    dr["货架描述"] = "";
                }
                else
                {
                    dr["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
                    //  dr["有效总数"] = dt_物料数量.Rows[0]["有效总数"];
                    dr["货架描述"] = dt_物料数量.Rows[0]["货架描述"];//19-9-17解决货架更新
                }
            }
        }

        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        //private void gv_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        //{
        //    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
        //    if (e.Column.FieldName == "仓库号")
        //    {
        //        dr["仓库号"] = e.Value;
        //        DataRow[] ds = dt_仓库.Select(string.Format("仓库号 = '{0}'", dr["仓库号"]));
        //        dr["仓库名称"] = ds[0]["仓库名称"];
        //        //dr["仓库名称"] = sr["仓库名称"].ToString();
        //        string sql = "select * from 仓库物料数量表 where 物料编码 = '" + dr["物料编码"] + "' and 仓库号 = '" + dr["仓库号"] + "'";
        //        SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
        //        DataTable dt_物料数量 = new DataTable();
        //        da.Fill(dt_物料数量);
        //        if (dt_物料数量.Rows.Count == 0)
        //        {
        //            dr["库存总数"] = 0;
        //           // dr["有效总数"] = 0;
        //        }
        //        else
        //        {
        //            dr["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
        //          //  dr["有效总数"] = dt_物料数量.Rows[0]["有效总数"];
        //        }
        //    }
        //}


    }
}
