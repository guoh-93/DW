using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPpurchase
{
    public partial class frm生产制令表_不用 : UserControl
    {
        //数据库连接字符串
        string strcon = "";

        public frm生产制令表_不用()
        {
            InitializeComponent();
            strcon = CPublic.Var.strConn;
        }

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

        //查找物料的信息
        private void fun_searchMaterial()
        {
            try
            {
                SqlDataAdapter da;
                string sql = @"select 基础数据物料信息表.物料编码,物料名称,物料类型,规格型号,图纸编号,图纸版本,产品线,客户,客户基础信息表.客户名称 
                                ,库存总数 from 基础数据物料信息表,客户基础信息表,仓库物料数量表
                                where (基础数据物料信息表.客户=客户基础信息表.客户编号) and 仓库物料数量表.物料编码=基础数据物料信息表.物料编码
                                and 基础数据物料信息表.物料类型='成品' and 基础数据物料信息表.停用=0";
                dt_wuliao = new DataTable();
                da = new SqlDataAdapter(sql, strcon);
                da.Fill(dt_wuliao);
                repositoryItemSearchLookUpEdit1.DataSource = dt_wuliao;
                repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
                repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_searchMaterial");
                throw new Exception(ex.Message);
            }  

        }

        //载入未生效的生产制令表
        private void fun_loadsczlMain()
        {
            try
            {
                SqlDataAdapter da;
                string sql="select * from 生产记录生产制令表 where 生效=0";
                da = new SqlDataAdapter(sql, strcon);
                dt_proZL = new DataTable();
                da.Fill(dt_proZL);

                //把下拉框dt没有的数据增加到里面去
                foreach (DataRow r in dt_proZL.Rows)
                {
                    DataRow[] drr1 = dt_wuliao.Select(string.Format("物料编码='{0}'", r["物料编码"].ToString()));
                    if (drr1.Length <= 0)
                    {
                        dt_wuliao.Rows.Add(r["物料编码"], r["物料名称"], r["物料类型"], r["规格型号"], r["图纸编号"], r["生产线"], r["客户ID"], r["客户名称"]);
                    }
                }
                gc_sczlmain.DataSource = dt_proZL;
                dt_proZL.ColumnChanged += dt_proZL_ColumnChanged;

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_loadsczlMain");
                throw new Exception(ex.Message);
            }
        }

        void dt_proZL_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            try
            {
                if (e.Column.ColumnName == "物料编码")
                {
                    DataRow[] dr = dt_wuliao.Select(string.Format("物料编码='{0}'", e.Row["物料编码"].ToString()));
                    if (dr.Length > 0)
                    {
                        e.Row["物料名称"] = dr[0]["物料名称"];
                        e.Row["规格型号"] = dr[0]["规格型号"];
                        e.Row["图纸编号"] = dr[0]["图纸编号"];
                        e.Row["客户ID"] = dr[0]["客户"];
                        e.Row["客户名称"] = dr[0]["客户名称"];
                        e.Row["生产线"] = dr[0]["产品线"];
                        e.Row["库存总数"] = dr[0]["库存总数 "];

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void frm生产制令表_Load(object sender, EventArgs e)
        {
            try
            {
                devGridControlCustom1.UserName = CPublic.Var.LocalUserID;
                devGridControlCustom1.strConn = CPublic.Var.strConn;
                devGridControlCustom2.UserName = CPublic.Var.LocalUserID;
                devGridControlCustom2.strConn = CPublic.Var.strConn;
                fun_searchMaterial();
                fun_loadsczlMain();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



           
        //新增行
        private void fun_AddNewRow()
        {
            try
            {
                DataRow r = dt_proZL.NewRow();
                r["生产制令类型"] = "标准类型";
                dt_proZL.Rows.Add(r);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_AddNewRow");
                throw new Exception(ex.Message);
            }
        }

        //检查保存制令的数据的合法性
        private void fun_checkSaveZLData()
        {
            try
            {
                foreach (DataRow r in dt_proZL.Rows)
                {   //如果GUID是空的说明是新增的
                    if (r["GUID"] == DBNull.Value)
                    {
                        r["GUID"] = System.Guid.NewGuid();
                        r["生产制令单号"] = string.Format("PM{0}{1:00}{2:0000}", DateTime.Now.Year, DateTime.Now.Month, CPublic.CNo.fun_得到最大流水号("CG", DateTime.Now.Year, DateTime.Now.Month));
                        r["日期"] = System.DateTime.Now;
                        r["制单人员"] = CPublic.Var.localUserName;
                        r["制单人员ID"] = CPublic.Var.LocalUserID;
                    }

                    if (r["生产制令类型"].ToString() == "")
                        throw new Exception("生产制令类型不能为空，请选择！");
                    if (r["物料编码"].ToString() == "")
                        throw new Exception("物料编码不能为空，请选择！");
                    if (r["制令数量"].ToString() == "")
                        throw new Exception("制令数量不能为空，请填写！");
                    try
                    {
                        decimal dd = Convert.ToDecimal(r["制令数量"]);
                    }
                    catch
                    {
                        throw new Exception("制令数量应该是数字，请重新填写！");
                    }
                    r["操作人员"] = CPublic.Var.localUserName;
                    r["操作人员ID"] = CPublic.Var.LocalUserID;
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_checkSaveZLData");
                throw new Exception(ex.Message);
            }
        }

        //数据的保存
        private void fun_SaveData()
        {
            try
            {   //制令主表
                SqlDataAdapter da;
                string sql = "select * from 生产记录生产制令表 where 1<>1";
                da = new SqlDataAdapter(sql, strcon);
                new SqlCommandBuilder(da);
                da.Update(dt_proZL);
                //制令明细表
                sql = "select * from 生产记录生产制令子表 where 1<>1";
                da = new SqlDataAdapter(sql, strcon);
                new SqlCommandBuilder(da);
                da.Update(dt_proZLdetail);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_SaveData");
                throw new Exception(ex.Message);
            }
        }



        #region  界面的操作
        //刷新按钮
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv_sczlmain.CloseEditor();
                this.BindingContext[dt_proZL].EndCurrentEdit();
                fun_loadsczlMain();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //新增操作
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv_sczlmain.CloseEditor();
                this.BindingContext[dt_proZL].EndCurrentEdit();
                fun_AddNewRow();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //删除操作
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (dt_proZL == null || dt_proZL.Rows.Count <= 0)
                    throw new Exception("没有生产制令可以删除！");
                DataRow r=(this.BindingContext[dt_proZL].Current as DataRowView).Row;
                if (MessageBox.Show(string.Format("请确定要删除生产制令单号为\"{0}\"的生产制令吗？", r["生产制令单号"].ToString()), "提示？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {   //删除该明细
                    foreach (DataRow r1 in dt_proZLdetail.Rows)
                    {
                        r1.Delete();
                    }
                    r.Delete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //保存操作
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv_sczlmain.CloseEditor();
                this.BindingContext[dt_proZL].EndCurrentEdit();
                fun_checkSaveZLData();
                fun_SaveData();
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //关闭操作
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        #endregion


        #region   生效操作









        //生效操作
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {


        }

        #endregion




        //行的变化
        private void gv_sczlmain_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {

                DataRow r = (this.BindingContext[dt_proZL].Current as DataRowView).Row;

                SqlDataAdapter da;
                string sql = string.Format("select * from 生产记录生产制令子表 where 生产制令单号='{0}'", r["生产制令单号"].ToString());
                dt_proZLdetail = new DataTable();
                da = new SqlDataAdapter(sql, strcon);
                da.Fill(dt_proZLdetail);

                gc_zldetail.DataSource = dt_proZLdetail;

                if (r.RowState != DataRowState.Added)
                {
                    foreach (DevExpress.XtraGrid.Columns.GridColumn dc in gv_sczlmain.Columns)
                    {
                        if (dc.FieldName != "选择")
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
                    foreach (DevExpress.XtraGrid.Columns.GridColumn dc in gv_sczlmain.Columns)
                    {
                        if (dc.FieldName != "选择" && dc.FieldName != "生产制令类型" && dc.FieldName != "制令数量")
                        {
                            dc.OptionsColumn.AllowEdit = false;
                        }
                        else
                        {
                            dc.OptionsColumn.AllowEdit = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #region  明细新增的界面

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (dt_proZL.Rows.Count <= 0)
                    throw new Exception("无生产制令，不可新增明细！");
                DataRow r = (this.BindingContext[dt_proZL].Current as DataRowView).Row;
                if (r.RowState == DataRowState.Added)
                    throw new Exception("你选中的生产制令是新增的，还没有保存，请先保存生产制令！");

                //fm关联销售明细选择 fm = new fm关联销售明细选择(dt_displaymx);

                //fm.ShowDialog();
                //if (fm.dt_保存打钩选择 != null)
                //{
                //    fun_detailDeal(fm.dt_保存打钩选择, r["生产计划单号"].ToString());
                //}  



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        #endregion

















    }
}
