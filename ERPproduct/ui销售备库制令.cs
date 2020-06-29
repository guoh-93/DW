using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

using System.IO;


namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class ui销售备库制令 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {
        #region 成员
        //数据库连接字符串
        string strconn = CPublic.Var.strConn; 
        DataRow drr = null;
        string str_制令 = "";
        string str_制令单 = "";
        DataTable dt_视图权限;
        public Boolean a;
        DataTable dt_proZLysx;
        bool flag = false;   //用以标记是否是是改制工单  
        DataTable dt_计划池; //用以减去计划池相应数量

        /// <summary>
        /// 生产制令的主表
        /// </summary>
        DataTable dt_proZL;

        /// <summary>
        /// 生产制令的明细
        /// </summary>
        DataTable dt_proZLdetail;

        /// <summary>
        /// 物料信息表
        /// </summary>
        DataTable dt_wuliao;

        /// <summary>
        /// 用作界面显示的明细
        /// </summary>
        DataTable dt_dispalymx;

        /// <summary>
        /// 勾选的用于生效制令的DT
        /// </summary>
        DataTable dt_SXZL;

        string cfgfilepath = "";
        #endregion

        public ui销售备库制令()
        {
            InitializeComponent();
        }
        //新增
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                gv_未生效制令.CloseEditor();
                this.BindingContext[dt_proZL].EndCurrentEdit();
                fun_AddNewRow();
                gv_未生效制令.Focus();
                gv_未生效制令.FocusedRowHandle = gv_未生效制令.LocateByDisplayText(0, gridColumn2, "");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //查找物料的信息填充下拉框
#pragma warning disable IDE1006 // 命名样式
        private void fun_searchMaterial()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                SqlDataAdapter da;
                string sql = @"select base.物料编码,base.物料名称,特殊备注,base.规格型号,base.图纸编号,车间编号,库存总数,kc.仓库号,kc.仓库名称,新数据
                            from 基础数据物料信息表 base,仓库物料数量表 kc where   base.物料编码=kc.物料编码 and 
                            base.物料类型<>'原材料' ";
                dt_wuliao = new DataTable();
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_wuliao);
                repositoryItemSearchLookUpEdit1.DataSource = dt_wuliao;
                repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
                repositoryItemSearchLookUpEdit1.ValueMember  =  "物料编码";
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_searchMaterial");
                throw new Exception(ex.Message);
            }

        }
        //检查保存制令的数据的合法性
#pragma warning disable IDE1006 // 命名样式
        private void fun_checkSaveZLData()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();
                foreach (DataRow r in dt_proZL.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
            
                    if (r["物料编码"].ToString() == "")
                        throw new Exception("物料编码不能为空，请选择！");
                    if (r["制令数量"].ToString() == "")
                        throw new Exception("制令数量不能为空，请填写！");
                    r["未排单数量"] = r["制令数量"];
                    try
                    {
                        decimal dd = Convert.ToDecimal(r["制令数量"]);
                    }
                    catch
                    {
                        throw new Exception("制令数量应该是数字，请重新填写！");
                    }

                    //如果GUID是空的说明是新增的
                    if (r["GUID"].ToString().Trim() == "")
                    {
                        r["操作人员"] = CPublic.Var.localUserName;
                        r["操作人员ID"] = CPublic.Var.LocalUserID;

                        r["GUID"] = System.Guid.NewGuid();
                     
                        r["生产制令单号"] = string.Format("PM{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                            CPublic.CNo.fun_得到最大流水号("PM", t.Year, t.Month));

                        r["日期"] = t;
                        r["制单人员"] = CPublic.Var.localUserName;
                        r["制单人员ID"] = CPublic.Var.LocalUserID;
                    }

                

                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_checkSaveZLData");
                throw new Exception(ex.Message);
            }
        }

        //数据的保存
#pragma warning disable IDE1006 // 命名样式
        private void fun_SaveData()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {   //制令主表
                SqlDataAdapter da;
                string sql = "select * from 生产记录生产制令表 where 1<>1";
                da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dt_proZL);
                //制令明细表
                sql = "select * from 生产记录生产制令子表 where 1<>1";
                da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dt_proZLdetail);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_SaveData");
                throw new Exception(ex.Message);
            }
        }

        //新增行
#pragma warning disable IDE1006 // 命名样式
        private void fun_AddNewRow()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataRow r = dt_proZL.NewRow();
                r["生产制令类型"] = "销售备库";
                r["加急状态"] = "正常";
                dt_proZL.Rows.Add(r);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_AddNewRow");
                throw new Exception(ex.Message);
            }
        }
        //载入未生效的生产制令表
#pragma warning disable IDE1006 // 命名样式
        private void fun_loadsczlMain()
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

                    gv_未生效制令.RestoreLayoutFromXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

                string sql = "";
                SqlDataAdapter da;
                if (str_制令单 != "")
                {

                    sql = string.Format(@"select sz.* ,库存总数,新数据  from 生产记录生产制令表 sz
                        left  join 基础数据物料信息表 base   on  base.物料编码=sz.物料编码
                        left join  仓库物料数量表 kc  on    kc.物料编码= sz.物料编码
                       where sz.生产制令单号='{0}' and sz.关闭 = 0    and kc.仓库号=sz.仓库号              
                       and sz.生效 = 0 and sz.完成 = 0 and 操作人员ID ='{1}'  ", str_制令单, CPublic.Var.LocalUserID);
                   
                    da = new SqlDataAdapter(sql, strconn);
                    dt_proZL = new DataTable();
                    da.Fill(dt_proZL);
                    dt_proZL.Columns.Add("选择", typeof(bool));
                    dt_proZL.Columns.Add("反馈备注");

                }
                else
                {



                    sql = string.Format(@"select sz.*,库存总数,新数据    from 生产记录生产制令表 sz
                                        left  join 基础数据物料信息表 base   on  base.物料编码=sz.物料编码
                                        left join  仓库物料数量表 kc  on    kc.物料编码= sz.物料编码
                                        where sz.生效 = 0 and sz.完成 = 0  and sz.关闭 = 0  and kc.仓库号=sz.仓库号   
                                        and 生产制令类型='销售备库'    and 操作人员ID ='{0}'  ", CPublic.Var.LocalUserID);
                  

                    da = new SqlDataAdapter(sql, strconn);
                    dt_proZL = new DataTable();
                    da.Fill(dt_proZL);
                    dt_proZL.Columns.Add("选择", typeof(bool));
                    dt_proZL.Columns.Add("反馈备注");

                }
                //制令子表
                sql = @"select 生产记录生产制令子表.*,反馈备注 from 生产记录生产制令子表,销售记录销售订单明细表
                where 生产记录生产制令子表.销售订单明细号=销售记录销售订单明细表.销售订单明细号 and   1<>1";
                da = new SqlDataAdapter(sql, strconn);
                dt_proZLdetail = new DataTable();
                dt_SXZL = new DataTable();
                da.Fill(dt_proZLdetail);

                //把下拉框dt没有的数据增加到里面去
                foreach (DataRow r in dt_proZL.Rows)
                {
                    DataRow[] drr1 = dt_wuliao.Select(string.Format("物料编码='{0}'", r["物料编码"].ToString()));
                    if (drr1.Length <= 0)
                    {
                        dt_wuliao.Rows.Add(r["物料编码"], r["物料名称"], r["物料类型"], r["规格型号"], r["图纸编号"], r["生产车间"]);
                    }
                }
                gc_销售备库制令.DataSource = dt_proZL;
                //gc_关联订单.DataSource = dt_proZLdetail;
                //dt_proZL.ColumnChanged += dt_proZL_ColumnChanged;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_loadsczlMain");
                throw new Exception(ex.Message);
            }
        }

        //明细回传值处理
#pragma warning disable IDE1006 // 命名样式
        private void fun_detailDeal(DataTable dt, string danhao)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                dt_proZLdetail = dt_proZLdetail.Clone();
                //勾选返回的dt
                foreach (DataRow r in dt.Rows)
                {
                    DataRow r_zlzb = dt_proZLdetail.NewRow();
                    r_zlzb["GUID"] = System.Guid.NewGuid();
                    r_zlzb["生产制令单号"] = danhao;
                    r_zlzb["销售订单明细号"] = r["销售订单明细号"];
                    r_zlzb["销售订单号"] = r["销售订单号"];
                    r_zlzb["物料编码"] = r["物料编码"];


                    r_zlzb["物料名称"] = r["物料名称"];
                    r_zlzb["客户"] = r["客户"];
                    r_zlzb["送达日期"] = r["送达日期"];
                    r_zlzb["规格型号"] = r["规格型号"];
                    r_zlzb["图纸编号"] = r["图纸编号"];
                    r_zlzb["数量"] = r["数量"];
                    r_zlzb["计量单位"] = r["计量单位"];
                    r_zlzb["销售备注"] = r["备注"];

                    dt_proZLdetail.Rows.Add(r_zlzb);
                }
                dt_dispalymx = dt_proZLdetail.Copy();

              //  gc_关联订单.DataSource = dt_dispalymx;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_detailDeal");
                throw new Exception(ex.Message);
            }
        }
        //保存
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                gv_未生效制令.CloseEditor();
                this.BindingContext[dt_proZL].EndCurrentEdit();
                fun_checkSaveZLData();
                fun_SaveData();
                barLargeButtonItem1_ItemClick(null, null);
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    //刷新
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                gv_未生效制令.CloseEditor();
                this.BindingContext[dt_proZL].EndCurrentEdit();
                fun_loadsczlMain();
          
                fun_searchMaterial();
  
      

                if (gv_未生效制令.RowCount > 0)
                {
                    gv_未生效制令.GetDataRow(0)["选择"] = false;
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

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //try
            //{
            //    if (dt_proZL.Rows.Count <= 0)
            //        throw new Exception("无生产制令，不可新增明细！");
            //    DataRow r = (this.BindingContext[dt_proZL].Current as DataRowView).Row;
            //    if (r.RowState == DataRowState.Added)
            //        throw new Exception("你选中的生产制令是新增的，还没有保存，请先保存生产制令！");
            //    fm关联销售明细选择 fm = new fm关联销售明细选择(dt_proZLdetail, r["物料编码"].ToString(),r["生产制令单号"].ToString());
            //    fm.ShowDialog();
            //    if (fm.dt != null)
            //    {
            //        //dt_dispalymx = fm.dt;
            //        //gridView1.DataSource = dt_dispalymx;
            //        fun_detailDeal(fm.dt_保存打钩选择, r["生产制令单号"].ToString());
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (dt_dispalymx == null || dt_dispalymx.Rows.Count <= 0)
                    throw new Exception("无明细可以删除，请先新增明细！");
                //DataRow r = (this.BindingContext[dt_dispalymx].Current as DataRowView).Row;
               // DataRow r = gv_关联订单.GetDataRow(gv_关联订单.FocusedRowHandle);
                //if (MessageBox.Show(string.Format("你确定要删除明细号为\"{0}\"的明细吗？", r["销售订单明细号"].ToString()), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                //{
                //    DataRow[] dr = dt_dispalymx.Select(string.Format("销售订单明细号='{0}'", r["销售订单明细号"].ToString()));
                //    if (dr.Length > 0)
                //    {
                //        dr[0].Delete();
                //    }
                //    r.Delete();
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton3_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //制令明细表

            string sql = "select * from 生产记录生产制令子表 where 1<>1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);

            da.Update(dt_dispalymx);  //dt_proZLdetail
            MessageBox.Show("保存成功");


            gv_未生效制令_RowCellClick_1(null, null);
        }


        //删除
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (dt_proZL == null || dt_proZL.Rows.Count <= 0)
                    throw new Exception("没有生产制令可以删除！");
                DataRow r = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);
                if (r.RowState != DataRowState.Added)
                {
                     
                    if (MessageBox.Show(string.Format("请确定要删除生产制令单号为\"{0}\"的生产制令吗？", r["生产制令单号"].ToString()), "提示？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {   //删除该明细
                        foreach (DataRow r1 in dt_proZLdetail.Rows)
                        {
                            r1.Delete();
                        }
                        r.Delete();
                    }
                }
                else
                {
                    r.Delete();
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message);
                MessageBox.Show("删除失败,刷新重试");
            }
        }
#pragma warning disable IDE1006 // 命名样式
        public void fun_check制令(DataRow dr)
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format("select * from 生产记录生产制令表 where 生产制令单号='{0}'", dr["生产制令单号"].ToString());
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToDecimal(dt.Rows[0]["制令数量"]) != Convert.ToDecimal(dr["制令数量"])
                        || dt.Rows[0]["备注"].ToString() != dr["备注"].ToString() || dt.Rows[0]["预完工日期"].ToString() != dr["预完工日期"].ToString())
                    {
                        throw new Exception("制令已被修改，刷新后重试");
                    }
                    if (dt.Rows[0]["生效"].Equals(true) || dt.Rows[0]["关闭"].Equals(true))
                    {
                        throw new Exception("制令已生效或已被关闭");
                    }
                }
                else
                {

                    throw new Exception("该制令已删除,刷新后重试");
                }

            }

        }
        private void 修改制令ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                gv_未生效制令.CloseEditor();
                this.BindingContext[dt_proZL].EndCurrentEdit();
                foreach (DataRow r in dt_proZL.Rows)
                {
                    if (r.RowState == DataRowState.Added)
                        throw new Exception(string.Format("有新增未保存的制令,先执行保存操作,或者删除明细操作后再修改制令"));
                }
                DataRow dr = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);
                fun_check制令(dr);
                frm修改制令 fm = new frm修改制令(dr);
                fm.ShowDialog();
                //if (fm.flag)
                //{
                //    for (int i = 0; i < gv_关联订单.RowCount; i++)
                //    {

                //        gv_关联订单.GetDataRow(i)["计划确认日期"] = dr["预完工日期"];
                //    }

                //}
                if (fm.de_现 != 0)
                {


                    dr["制令数量"] = fm.de_现;
                    dr["未排单数量"] = fm.de_现;
                   // gv_已生效制令.CloseEditor();
                    this.BindingContext[dt_proZL].EndCurrentEdit();
                    DataTable dt_销售明细 = new DataTable();
                    //for (int i = 0; i < gv_关联订单.RowCount; i++)
                    //{
                    //    string str = string.Format("select * from 销售记录销售订单明细表 where 生效=1 and 关闭=0 and 销售订单明细号='{0}'", gv_关联订单.GetDataRow(i)["销售订单明细号"].ToString());
                    //    using (SqlDataAdapter a = new SqlDataAdapter(str, strconn))
                    //    {
                    //        a.Fill(dt_销售明细);
                    //        DataRow[] xx = dt_销售明细.Select(string.Format("销售订单明细号='{0}'", gv_关联订单.GetDataRow(i)["销售订单明细号"].ToString()));
                    //        if (xx.Length > 0)
                    //        {

                    //            xx[0]["计划确认日期"] = gv_关联订单.GetDataRow(i)["计划确认日期"];
                    //        }

                    //    }
                    //}
                    SqlDataAdapter dda;
                    SqlConnection conn = new SqlConnection(strconn);
                    conn.Open();
                    SqlTransaction xgwzl = conn.BeginTransaction("修改未生效制令");
                    try
                    {

                        SqlCommand cmd2 = new SqlCommand("select * from 销售记录销售订单明细表 where 1<>1", conn, xgwzl);
                        dda = new SqlDataAdapter(cmd2);
                        new SqlCommandBuilder(dda);
                        dda.Update(dt_销售明细);

                        string sql_1 = "select * from 生产记录生产制令表 where 1<>1";
                        cmd2 = new SqlCommand(sql_1, conn, xgwzl);
                        dda = new SqlDataAdapter(cmd2);
                        new SqlCommandBuilder(dda);
                        dda.Update(dt_proZL);
                        xgwzl.Commit();
                        MessageBox.Show("修改成功");
                    }
                    catch (Exception)
                    {
                        xgwzl.Rollback();
                        throw;
                    }

                }


            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void ui销售备库制令_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            try
            {
                fun_searchMaterial();
                fun_loadsczlMain();

                if (drr != null)
                {
                    DataRow[] r = dt_proZL.Select(string.Format("生产制令单号='{0}'", str_制令));

                    r[0]["选择"] = true;
                    gv_未生效制令.Focus();
                    gv_未生效制令.FocusedRowHandle = gv_未生效制令.LocateByDisplayText(0, gridColumn2, str_制令);

                }
                if (gv_未生效制令.RowCount > 0)
                {
                    gv_未生效制令.GetDataRow(0)["选择"] = false;
                }
                gv_未生效制令_RowCellClick_1(null, null);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
          
        }



#pragma warning disable IDE1006 // 命名样式
        private void gv_未生效制令_RowCellClick_1(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);
            if (dr == null) return;
//            if (dr != null)
//            {
//                string sql = string.Format(@"select 生产记录生产制令子表.*,[销售记录销售订单明细表].备注,反馈备注,原ERP物料编号 from 生产记录生产制令子表,销售记录销售订单明细表,基础数据物料信息表
//    
//                                        where 生产记录生产制令子表.销售订单明细号 =销售记录销售订单明细表.销售订单明细号  and 生产记录生产制令子表.物料编码=基础数据物料信息表.物料编码
//
//                                            and  生产制令单号='{0}'", dr["生产制令单号"].ToString().Trim());
//                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
//                {
//                    dt_dispalymx = new DataTable();
//                    da.Fill(dt_dispalymx);
//                    // gc_关联订单.DataSource = dt_dispalymx;
//                }

            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc_销售备库制令, new Point(e.X, e.Y));
                gv_未生效制令.CloseEditor();
                this.BindingContext[dt_proZL].EndCurrentEdit();

            }
            if (dr.RowState != DataRowState.Added  )
            {


                foreach (DevExpress.XtraGrid.Columns.GridColumn dc in gv_未生效制令.Columns)
                {
                    if (dc.FieldName != "选择" && dc.FieldName != "备注" && dc.FieldName != "预完工日期" && dc.FieldName != "加急状态" && dc.FieldName != "反馈备注")
                    {
                        dc.OptionsColumn.AllowEdit = false;
                    }
                    else
                    {
                        dc.OptionsColumn.AllowEdit = true;
                    }
                }
            }
            else
            {
                foreach (DevExpress.XtraGrid.Columns.GridColumn dc in gv_未生效制令.Columns)
                {
                    if (dc.FieldName != "预完工日期" && dc.FieldName != "选择" && dc.FieldName != "备注" && dc.FieldName != "反馈备注"
                        && dc.FieldName != "制令数量" && dc.FieldName != "物料编码" && dc.FieldName != "加急状态")
                    {
                        dc.OptionsColumn.AllowEdit = false;
                    }
                    else
                    {
                        dc.OptionsColumn.AllowEdit = true;
                    }
                }
            }
//            }
        }

 

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataRow dr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);

                DataRow rr = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);
                rr["物料名称"] = dr["物料名称"];
                rr["规格型号"] = dr["规格型号"];
                rr["图纸编号"] = dr["图纸编号"];
                rr["物料编码"] = dr["物料编码"];
                rr["生产车间"] = dr["车间编号"];
                rr["库存总数"] = dr["库存总数"];
                rr["特殊备注"] = dr["特殊备注"];
                rr["仓库号"] = dr["仓库号"];
                rr["仓库名称"] = dr["仓库名称"];
                rr["新数据"] = dr["新数据"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataRow dr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);

                DataRow rr = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);
                rr["物料名称"] = dr["物料名称"];
                rr["规格型号"] = dr["规格型号"];
                rr["图纸编号"] = dr["图纸编号"];
                rr["物料编码"] = dr["物料编码"];
                rr["生产车间"] = dr["车间编号"];
                rr["库存总数"] = dr["库存总数"];
                rr["特殊备注"] = dr["特殊备注"];
                rr["仓库号"] = dr["仓库号"];
                rr["仓库名称"] = dr["仓库名称"];
                rr["新数据"] = dr["新数据"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_未生效制令_ColumnPositionChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gv_未生效制令.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_未生效制令_ColumnWidthChanged(object sender, DevExpress.XtraGrid.Views.Base.ColumnEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gv_未生效制令.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
