using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;



namespace ERPproduct
{
    public partial class frm领料明细选择界面 : UserControl
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        string strcon="";

        /// <summary>
        /// 回传的工单，已经选择过的工单。
        /// </summary>
        string strReturngd = "";

        /// <summary>
        /// 默认仓库ID
        /// </summary>
        string strDefalutStockID = "";

        public frm领料明细选择界面(DataTable dt,string CkID)
        {
            //strReturngd = gongdan;

            strDefalutStockID = CkID;
            InitializeComponent();
            strcon = CPublic.Var.strConn;
        }


        #region   变量




        #endregion


        DataTable dt_getDt;

        /// <summary>
        /// 选择的工单的那一行
        /// </summary>
        DataRow dr_select = null;

        /// <summary>
        /// 回传领料单
        /// </summary>
        public DataTable dt_Returnliaodan;

        /// <summary>
        /// 选择好的工单
        /// </summary>
        DataTable dt_selectGd;



        /// <summary>
        /// 生产工单表
        /// </summary>
        DataTable dt_WorkOrder;

        /// <summary>
        /// 生产工单表的DV
        /// </summary>
        DataView dv_WorkOrder;

        /// <summary>
        /// 显示
        /// </summary>
        DataTable dt_BomDisPlay;



        #region  加载

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 获取领料单明细的一个结构DT
        /// </summary>
        private void fun_GetliaoMx()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                SqlDataAdapter da;
                string sql = "";
                //生产领料单明细
                sql = "select * from 生产记录生产领料单明细表 where 1<>1";
                dt_Returnliaodan = new DataTable();
                da = new SqlDataAdapter(sql, strcon);
                da.Fill(dt_Returnliaodan);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_GetliaoMx");
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 获取生产记录工单表中的信息：工单是生效的，未完成的，未生产数量是大于0的
        /// </summary>
        private void fun_getGDdata()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                SqlDataAdapter da;
                string sql = "";
                //生产记录生产工单表
                if (txt_gdtime1.EditValue != null && txt_gdtime2.EditValue != null && txt_gdtime1.EditValue.ToString() != "" && txt_gdtime2.EditValue.ToString() != "")
                {
                    if (Convert.ToDateTime(txt_gdtime1.EditValue) > Convert.ToDateTime(txt_gdtime2.EditValue))
                        throw new Exception("起始时间不能大于结束时间！");
                    sql = string.Format(@"select 生产记录生产工单表.*,生产记录生产领料单主表.领料单号 from 生产记录生产工单表 left join 生产记录生产领料单主表 
                            on 生产记录生产工单表.生产工单号=生产记录生产领料单主表.生产工单号 where 生产记录生产工单表.预计开工日期>='{0}' and 生产记录生产工单表.预计开工日期<='{1}'
                            and 生产记录生产工单表.生效=1 and 生产记录生产工单表.未检验数量>0"
                            ,txt_gdtime1.EditValue, txt_gdtime2.EditValue);
                }
                else
                {
                    sql = @"select 生产记录生产工单表.*,生产记录生产领料单主表.领料单号 from 生产记录生产工单表 left join 生产记录生产领料单主表 
                           on 生产记录生产工单表.生产工单号=生产记录生产领料单主表.生产工单号 where 生产记录生产工单表.生效=1 and 生产记录生产工单表.未检验数量>0";
                }
                dt_WorkOrder = new DataTable();
                da = new SqlDataAdapter(sql, strcon);
                da.Fill(dt_WorkOrder);
                dt_WorkOrder.Columns.Add("选择", typeof(bool));

                //DataRow [] drr= dt_WorkOrder.Select("distinct(生产工单号)");
                //int i = drr.Length;

                ////回传一个工单号过来，如果这个工单号已经选择过了，那么已选择就要赋值
                //DataRow[] dr = dt_WorkOrder.Select(string.Format("生产工单号='{0}'", strReturngd));
                //if (dr.Length > 0)
                //{
                //    dr[0]["已选择"] = 1;
                //}
                string straaa = "";
                foreach (DataRow r in dt_WorkOrder.Rows)
                {
                    if (r["生产工单号"].ToString() != straaa)
                    {
                        straaa = r["生产工单号"].ToString();
                    }
                    else
                    {
                       // r.AcceptChanges();
                        r.Delete();
                    }
                }
                dt_WorkOrder.AcceptChanges();
               
                dv_WorkOrder = new DataView(dt_WorkOrder);
                //dt_WorkOrder=dv_WorkOrder.ToTable(true);

                dv_WorkOrder.RowFilter = "领料单号 is null";
                gc_selectGD.DataSource = dv_WorkOrder;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_getGDdata");
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm领料明细选择界面_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                devGridControlCustom1.UserName = CPublic.Var.LocalUserID;
                devGridControlCustom1.strConn = CPublic.Var.strConn;
                txt_gdtime1.EditValue = System.DateTime.Today.AddDays(-7);  //起始时间
                txt_gdtime2.EditValue = System.DateTime.Today.AddDays(1).AddSeconds(-1);  //终止时间
                fun_GetliaoMx();
                fun_getGDdata();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion


        #region   选择领料工单

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 工单选择的有效性检查
        /// </summary>
        private void fun_GdOrderCheck()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {   //勾选工单
                foreach (DataRowView drv in dv_WorkOrder)
                {
                    if (drv.Row["选择"].Equals(true))
                    {
                        dr_select = drv.Row;
                    }
                }
                if (dr_select == null)
                    throw new Exception("尚未勾选需要领料的工单，请先勾选工单！");
                DataRow[] dr = dt_BomDisPlay.Select("选择=True");
                if (dr.Length <=0)
                    throw new Exception("尚未勾选需要领料的物料，请勾选需要领料的物料！");
                //遍历领料清单：如果领料清单的工单跟勾选的工单不一致，就要抛出错误
                foreach (DataRow r in dt_BomDisPlay.Rows)
                {
                    if (r["生产工单号"].ToString() != dr_select["生产工单号"].ToString())
                        throw new Exception(string.Format("领料清单中\"{0}\"物料编码,不是工单\"{1}\"所要领的物料！", r["物料编码"].ToString(), dr_select["生产工单号"].ToString()));
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_GdOrderCheck");
                throw ex;
            }
        }
        
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 获取该工单所需要的物料，一层物料
        /// 找到该物料的子项，一一赋值到回传的DT中去
        /// </summary>
        private void fun_GetOrderBom()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                //获取勾选的物料
                foreach(DataRow r in dt_BomDisPlay.Rows)
                {
                    if (r["选择"].Equals(true))
                    {
                        DataRow r1 = dt_Returnliaodan.NewRow();
                        r1["生产制令单号"] = dr_select["生产制令单号"];
                        r1["生产工单号"] = dr_select["生产工单号"];
                        r1["工单负责人"] = dr_select["工单负责人"];
                        r1["生产线"] = dr_select["生产线"];
                        r1["生产工单类型"] = dr_select["生产工单类型"];
                        r1["物料编码"] = r["子项编码"];
                        r1["物料名称"] = r["子项名称"];
                        r1["图纸编号"] = r["图纸编号"];
                        r1["规格型号"] = r["规格型号"];
                        r1["物料类型"] = r["物料类型"];
                        r1["领料数量"] = r["待领料量"];
                        dt_Returnliaodan.Rows.Add(r1);
                    }   
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_GetOrderBom");
                throw ex;
            }
        }

     
        #endregion




        #region 界面操作

        //工单选择行变化:焦点变化
#pragma warning disable IDE1006 // 命名样式
        private void gv_selectGD_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (e.FocusedRowHandle >= 0)
                {
                    DataRowView drv = this.BindingContext[dv_WorkOrder].Current as DataRowView;
                    SqlDataAdapter da;
                    string sql = string.Format("select * from 基础数据物料BOM表 where 物料编码='{0}'", drv.Row["物料编码"].ToString());
                    dt_BomDisPlay = new DataTable();
                    da = new SqlDataAdapter(sql, strcon);
                    da.Fill(dt_BomDisPlay);
                    dt_BomDisPlay.Columns.Add("生产工单号");
                    dt_BomDisPlay.Columns.Add("选择", typeof(bool));
                    dt_BomDisPlay.Columns.Add("仓库号");
                    dt_BomDisPlay.Columns.Add("仓库名称");
                    dt_BomDisPlay.Columns.Add("需领料量");
                    dt_BomDisPlay.Columns.Add("已领料量");
                    dt_BomDisPlay.Columns.Add("待领料量");
                    dt_BomDisPlay.Columns.Add("图纸编号");
                    dt_BomDisPlay.Columns.Add("规格型号");
                    dt_BomDisPlay.Columns.Add("物料类型");
                    DataTable dt_cangku = new DataTable();  //相应物料所在的仓库
                    foreach (DataRow r in dt_BomDisPlay.Rows)
                    {
                        r["生产工单号"] = drv.Row["生产工单号"];
                        dt_cangku.Clear();
                        sql = string.Format("select 图纸编号,规格型号,仓库号,仓库名称,物料类型 from 基础数据物料信息表 where 物料编码='{0}'", r["子项编码"].ToString());
                        da = new SqlDataAdapter(sql, strcon);
                        da.Fill(dt_cangku);
                        if (dt_cangku.Rows.Count > 0)
                        {
                            r["仓库号"] = dt_cangku.Rows[0]["仓库号"];
                            r["仓库名称"] = dt_cangku.Rows[0]["仓库名称"];
                            r["图纸编号"] = dt_cangku.Rows[0]["图纸编号"];
                            r["规格型号"] = dt_cangku.Rows[0]["规格型号"];
                            r["物料类型"] = dt_cangku.Rows[0]["物料类型"];
                        }
                        r["需领料量"] = (Convert.ToDecimal(r["数量"]) * Convert.ToDecimal(drv.Row["生产数量"])).ToString(".0000");
                        //计算已领料量
                        sql=string.Format("select sum(领料数量) from 生产记录生产领料单明细表 where 生产工单号='{0}' and  物料编码='{1}'",drv.Row["生产工单号"].ToString(),r["子项编码"].ToString());
                        da=new SqlDataAdapter(sql,strcon);
                        DataTable dt_jisuan=new DataTable();
                        da.Fill(dt_jisuan);
                        if (dt_jisuan.Rows[0][0] != DBNull.Value)
                        {
                            r["已领料量"] = dt_jisuan.Rows[0][0];
                        }
                        else
                        {
                            r["已领料量"] = 0.0000;
                        }
                        r["待领料量"] = Convert.ToDecimal(r["需领料量"]) - Convert.ToDecimal(r["已领料量"]);
                    }
                    gv_bom.ViewCaption = string.Format("生产工单\"{0}\"的领料清单", drv.Row["生产工单号"].ToString());
                    gc_bom.DataSource = dt_BomDisPlay;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //查询操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                //fun_GdOrderCheck();
                //DataRow r = dt_WorkOrder.NewRow();
                //dt_WorkOrder.Rows.Add(r);
                fun_getGDdata();
                gv_selectGD.RowClick += gv_selectGD_RowClick;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        void gv_selectGD_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
          
        }

        //确定选择的列
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (dt_WorkOrder.Rows.Count > 0)
                {
                    gv_selectGD.CloseEditor();
                    this.BindingContext[dt_WorkOrder].EndCurrentEdit();
                }
                if (dt_BomDisPlay.Rows.Count > 0)
                {
                    gv_bom.CloseEditor();
                    this.BindingContext[dt_BomDisPlay].EndCurrentEdit();
                }
                fun_GdOrderCheck();
                fun_GetOrderBom();
                this.ParentForm.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //选择工单的gridcontrol选择列需要互斥
#pragma warning disable IDE1006 // 命名样式
        private void repositoryItemCheckEdit1_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                foreach (DataRowView drv in dv_WorkOrder)
                {
                    if (drv.Row["选择"].Equals(true))
                    {
                       drv.Row["选择"] = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //取消操作：即关闭窗口的操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            this.ParentForm.Close();
        }

        //工单物料清单的行变化事件
#pragma warning disable IDE1006 // 命名样式
        private void gv_bom_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (e.FocusedRowHandle >= 0)
                {
                    DataRowView drv = this.BindingContext[dv_WorkOrder].Current as DataRowView;  //当前选中的
                    if (!drv.Row["选择"].Equals(true))
                    {
                        gv_bom.OptionsBehavior.Editable = false;

                        foreach (DataRow r in dt_BomDisPlay.Rows)
                        {
                            r["选择"] = false;
                        }
                    }
                    else
                    {
                        DataRow r = (this.BindingContext[dt_BomDisPlay].Current as DataRowView).Row;
                        if (r["仓库号"].ToString() != strDefalutStockID || Convert.ToDecimal(r["待领料量"])<=0) //如果物料的默认仓库和操作员进来的仓库不一致，该物料就不能选择。
                        {
                            gv_bom.OptionsBehavior.Editable = false;
                        }
                        else
                        {
                            gv_bom.OptionsBehavior.Editable = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //工单补料的勾选项
#pragma warning disable IDE1006 // 命名样式
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (dt_WorkOrder != null)
                {
                    if (checkBox1.Checked == true)    //如果勾选，则出现所有的工单号
                    {
                        dv_WorkOrder = new DataView(dt_WorkOrder);
                        //dv_WorkOrder.RowFilter = "distinct(生产工单号)";
                        // gv_selectGD.FocusedRowChanged += gv_selectGD_FocusedRowChanged;
                    }
                    else
                    {
                        dv_WorkOrder.RowFilter = "领料单号 is null";  //如果没有勾选则只出现领料单中没有出现的工单
                        foreach (DataRow r in dt_WorkOrder.Rows)
                        {
                            if (r["领料单号"] != DBNull.Value)
                            {
                                r["选择"] = false;
                            }
                        }
                    }

                    gc_selectGD.DataSource = dv_WorkOrder;
                    dt_BomDisPlay.Clear();
                    gc_bom.DataSource = dt_BomDisPlay;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion














    }
}
