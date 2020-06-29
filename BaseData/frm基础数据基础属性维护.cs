using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CPublic;
using CZMaster;
using System.Data.SqlClient;

namespace BaseData
{
    public partial class frm基础数据基础属性维护 : UserControl
    {


        #region   公共成员


        #endregion



        #region   私有成员

        CurrencyManager cmM;

        /// <summary>
        /// 维护的属性大类的表
        /// </summary>
        DataTable dtP;

        /// <summary>
        /// 主表，属性表
        /// </summary>
        DataTable dtM;

        /// <summary>
        /// 记录当前处理的大类
        /// </summary>
        string strDQCLSXDL = "";

        /// <summary>
        /// 记录当前处理大类的属性描述
        /// </summary>
        string strDQDLMS = "";

        #endregion 



        #region    类加载

        public frm基础数据基础属性维护()
        {
            InitializeComponent();
        }


        private void frm基础数据基础属性维护_Load(object sender, EventArgs e)
        {
            try
            {
                #region gridcontrol汉化代码
               // DevExpress.Accessibility.AccLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressUtilsLocalizationCHS();
                //DevExpress.XtraBars.Localization.BarLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraBarsLocalizationCHS();
                //DevExpress.XtraCharts.Localization.ChartLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraChartsLocalizationCHS();
              //  DevExpress.XtraEditors.Controls.Localizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraEditorsLocalizationCHS();
               // DevExpress.XtraGrid.Localization.GridLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraGridLocalizationCHS();
               // DevExpress.XtraLayout.Localization.LayoutLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraLayoutLocalizationCHS();
                //DevExpress.XtraNavBar.NavBarLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraNavBarLocalizationCHS();
                //DevExpress.XtraPivotGrid.Localization.PivotGridLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraPivotGridLocalizationCHS();
              //  DevExpress.XtraPrinting.Localization.PreviewLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraPrintingLocalizationCHS();
                //DevExpress.XtraReports.Localization.ReportLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraReportsLocalizationCHS();
                //DevExpress.XtraRichEdit.Localization.XtraRichEditLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraRichEditLocalizationCHS();
                //DevExpress.XtraRichEdit.Localization.RichEditExtensionsLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraRichEditExtensionsLocalizationCHS();
                //DevExpress.XtraScheduler.Localization.SchedulerLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraSchedulerLocalizationCHS();
                //DevExpress.XtraScheduler.Localization.SchedulerExtensionsLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraSchedulerExtensionsLocalizationCHS();
                //DevExpress.XtraSpellChecker.Localization.SpellCheckerLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraSpellCheckerLocalizationCHS();
                //DevExpress.XtraTreeList.Localization.TreeListLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraTreeListLocalizationCHS();
                //DevExpress.XtraVerticalGrid.Localization.VGridLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraVerticalGridLocalizationCHS();
                //DevExpress.XtraWizard.Localization.WizardLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraWizardLocalizationCHS();
                #endregion
                fun_属性大类下拉();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            this.txt_choosedalei.Edit.KeyDown += Edit_KeyDown;

        }





        #endregion



        #region 其他数据处理

        /// <summary>
        /// 数据的合法性检查
        /// </summary>
        private void fun_Check()
        {
            int x = 0;
            //检查排列顺序不能为空,排列顺序是否有重复，排列顺序必须为数字，属性值不能为空
            foreach (DataRow r in dtM.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;  //如果该行打上delete标记就跳过
                //检查1不输入的话，默认为0
                if (r["POS"].ToString() == "")
                {
                    r["POS"] = 0;
                }

                //检查2 如果输入的话，检查是不是数字
                try
                {
                    x = int.Parse(r["POS"].ToString());
                }
                catch
                {
                    throw new Exception("排列顺序需要输入数字，请检查！");
                }
                //检查3
                if (r["属性值"].ToString() == "")
                {
                    throw new Exception("属性值的数据不能为空，请检查！");
                }

                //检查属性值，不能重复
                DataRow[] dr = dtM.Select(string.Format("属性值='{0}'", r["属性值"]));
                if (dr.Length >= 2)
                {
                    throw new Exception("属性值有重复，请检查！");
                }
            }
        }


        #endregion



        #region 数据库读取和保存


        /// <summary>
        /// 属性大类的下拉框的方法
        /// </summary>
        private void fun_属性大类下拉()
        {
            string sql = "select 属性值,属性描述,权限 from  基础数据基础属性分类表 order by POS ";

            dtP = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
            //foreach (DataRow r in dtP.Rows)
            //{
            //    ((DevExpress.XtraEditors.Repository.RepositoryItemComboBox)this.selectdalei_txt.Edit).Items.Add(r["属性值"].ToString());
            //}
            repositoryItemSearchLookUpEdit1.DataSource = dtP;
            repositoryItemSearchLookUpEdit1.ValueMember = "属性值";
            repositoryItemSearchLookUpEdit1.DisplayMember = "属性值";

        }


        /// <summary>
        /// 主数据加载，加载的是某个大类的主数据
        /// </summary>
        /// <param name="strlb"></param>
        private void fun_主数据加载(string strlb)
        {
            string sql = "select * from 基础数据基础属性表 where 属性类别='" + strlb + "' order by POS";

            dtM = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
            cmM = this.BindingContext[dtM] as CurrencyManager;
            gcM.DataSource = dtM;

            strDQCLSXDL = strlb;  //传递属性大类，即把属性值赋给一个全局的字符串

        }


        /// <summary>
        /// 数据新增或者修改之后的保存方法;
        /// </summary>
        private void fun_主数据的保存()
        {
            DataView dv = new DataView(dtM);

            dv.Sort = "POS";

            int j = 0;
            foreach (DataRowView drv in dv)
            {
                DataRow r = drv.Row;
                if (r["属性值GUID"] == DBNull.Value)  //表示GUID不存在值
                {
                    r["属性类别"] = strDQCLSXDL;
                    r["属性描述"] = strDQDLMS;
                    r["属性值GUID"] = System.Guid.NewGuid().ToString();   //GUID的产生方式
                    r["首字母"] = r["属性值"].ToString().Substring(0, 1);
                }

                drv.Row["POS"] = j++;
            }

            MasterSQL.Save_DataTable(dtM, "基础数据基础属性表", CPublic.Var.strConn);
            fun_主数据加载(strDQCLSXDL);   //如果少了这段代码会出现并发性错误
        }


        /// <summary>
        /// 数据的删除方法
        /// </summary>
        //private void fun_数据删除()
        //{
        //   DataRow r = (this.BindingContext[dtM].Current as DataRowView).Row;
        //   if (MessageBox.Show(string.Format("你确定要删除属性类别\"{0}\" 的属性值 \"{1}\" 吗？", r["属性类别"].ToString(), r["属性值"].ToString()), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
        //   {
        //       if (r["属性值GUID"] == DBNull.Value)
        //       {
        //           r.Delete();
        //       }
        //       else
        //       {
        //           r.Delete();
        //           MasterSQL.Save_DataTable(dtM, "基础数据基础属性表", CPublic.Var.strConn);
        //       }

        //       throw new Exception("删除成功！");
        //   }       
        //}


        /// <summary>
        /// 数据的刷新操作
        /// </summary>
        private void fun_刷新()
        {
            if (txt_choosedalei.EditValue == null)
            {
                throw new Exception("请选择需要维护的大类！");
            }

            DataRow[] dr = dtP.Select(string.Format("属性值='{0}'", txt_choosedalei.EditValue.ToString()));

            if (dr.Length > 0)
            {
                if (!dr[0]["属性描述"].ToString().Equals(""))
                {
                    gvM.ViewCaption = "当前维护大类是：" + dr[0]["属性值"].ToString() + "      大类的作用是：" + dr[0]["属性描述"].ToString();   //属性值
                    strDQDLMS = dr[0]["属性描述"].ToString();   //记录属性描述
                    fun_主数据加载(dr[0]["属性值"].ToString());
                }
                else
                {
                    gvM.ViewCaption = "当前维护的大类是：" + dr[0]["属性值"].ToString();   //属性值
                    strDQDLMS = "";   //记录属性描述
                    fun_主数据加载(dr[0]["属性值"].ToString());
                }
            }
            else
            {
                throw new Exception("没有该大类，请重新选择！");
            }
        }


        #endregion



        #region  界面的操作

        /// <summary>
        ///按回车键，进行刷新查询操作。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Edit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    DataView dv = new DataView(dtM);
                    dv.RowStateFilter = DataViewRowState.ModifiedCurrent | DataViewRowState.Added | DataViewRowState.Deleted;   //当前修改，增加，和删除的状态

                    if (dv.Count > 0)
                    {
                        if (MessageBox.Show(string.Format("大类\"{0}\"数据发生更改，如不需保存，请确定！", strDQCLSXDL), "询问！", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            fun_刷新();
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        fun_刷新();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }


        }
        private void fun_columnscaption()
        {

            gridColumn1.Visible = false;
            gridColumn2.Visible = false;
            gridColumn3.Visible = false;
            gridColumn8.Visible = false;
            gridColumn9.Visible = false;
            gridColumn10.Visible = false;
            gridColumn11.Visible = false;
            gridColumn12.Visible = false;
            gridColumn13.Visible = false;
            gridColumn14.Visible = false;


            if (txt_choosedalei.EditValue.ToString() == "审批流单据类型")
            {
                gridColumn6.Caption = "主表名称";
                gridColumn7.Caption = "明细表名称";
                gridColumn1.Visible = true;
                gridColumn2.Visible = true;
                gridColumn3.Visible = true;
                gridColumn8.Visible = true;
                gridColumn1.Caption = "单号字段名";
                gridColumn2.Caption = "数量字段名";
                gridColumn3.Caption = "料号字段名";
                gridColumn2.Caption = "名称字段名";
            }
            else if (txt_choosedalei.EditValue.ToString() == "仓库类别")
            {

                gridColumn6.Caption = "仓库号";
                gridColumn7.Caption = "厂区"; //属性字段2
                gridColumn9.Visible = true;
                gridColumn10.Visible = true;
                gridColumn11.Visible = true;
                gridColumn12.Visible = true;
                gridColumn13.Visible = true;
                gridColumn14.Visible = true;


                gridColumn9.Caption = "纳入可用量"; //布尔字段1
                gridColumn10.Caption = "参与需求运算";//布尔字段2
                gridColumn11.Caption = "可发货";//布尔字段3
                gridColumn12.Caption = "BOM可选";//布尔字段4
                gridColumn13.Caption = "可发料";//布尔字段5
                gridColumn14.Caption = "可入库";//布尔字段6


            }
            else if (txt_choosedalei.EditValue.ToString() == "壳体颜色")
            {
                gridColumn6.Visible = false;
                gridColumn7.Visible = true;
                gridColumn7.Caption = "CRM中文说明";
            }
            else if (txt_choosedalei.EditValue.ToString() == "硬件版本")
            {
                gridColumn6.Visible = false;
                gridColumn7.Visible = true;
                gridColumn7.Caption = "CRM中文说明";
            }
            else if (txt_choosedalei.EditValue.ToString() == "把手类型")
            {
                gridColumn6.Visible = false;
                gridColumn7.Visible = true;
                gridColumn7.Caption = "CRM中文说明";
            }
            else if (txt_choosedalei.EditValue.ToString() == "业务员")
            {

                gridColumn6.Caption = "员工号";
                gridColumn7.Caption = "属性字段2";
            }
            else if (txt_choosedalei.EditValue.ToString() == "原因分类")
            {

                gridColumn6.Caption = "原因描述";
                gridColumn7.Caption = "属性字段2";
            }
            else if (txt_choosedalei.EditValue.ToString() == "计量单位")
            {

                gridColumn6.Caption = "单位编码";
                gridColumn7.Caption = "属性字段2";
            }
            else
            {
                gridColumn6.Caption = "属性字段1";
                gridColumn7.Caption = "属性字段2";

            }

        }


        /// <summary>
        /// 界面的刷新操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_columnscaption();
                DataView dv = new DataView(dtM);
                dv.RowStateFilter = DataViewRowState.ModifiedCurrent | DataViewRowState.Added | DataViewRowState.Deleted;   //当前修改，增加，和删除的状态

                if (dv.Count > 0)
                {
                    if (MessageBox.Show(string.Format("大类\"{0}\"数据发生更改，如不需保存，请确定！", strDQCLSXDL), "询问！", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        fun_刷新();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    fun_刷新();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// 新增操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (dtM == null)
            {
                MessageBox.Show("请选择需要维护大类！！！");
                return;
            }
            else
            {
                gvM.CloseEditor();
                cmM.EndCurrentEdit();
                try
                {
                    cmM.AddNew();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (dtM == null)
            {
                MessageBox.Show("请选择需要维护大类！！！");
                return;
            }
            else
            {
                try
                {
                    gvM.CloseEditor();
                    cmM.EndCurrentEdit();

                    if (dtM.Rows.Count == 0) throw new Exception("无数据!");
                    DataRow r = (this.BindingContext[dtM].Current as DataRowView).Row;



                    if (MessageBox.Show(string.Format("你确定要删除属性类别\"{0}\" 的属性值 \"{1}\" 吗？", r["属性类别"].ToString(), r["属性值"].ToString()), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        r.Delete();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }



        /// <summary>
        /// 保存操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (dtM == null)
            {
                MessageBox.Show("请选择需要维护大类！！！");
                return;
            }
            else
            {
                gvM.CloseEditor();
                cmM.EndCurrentEdit();
                try
                {
                    fun_Check();
                    fun_主数据的保存();
                    MessageBox.Show("保存成功！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        #endregion

        private void txt_choosedalei_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (txt_choosedalei.EditValue == null && dtM!=null)
                {
                 
                    gcM.DataSource = dtM.Clone();
                    return;
                }
                fun_columnscaption();
                DataView dv = new DataView(dtM);
                dv.RowStateFilter = DataViewRowState.ModifiedCurrent | DataViewRowState.Added | DataViewRowState.Deleted;   //当前修改，增加，和删除的状态

                if (dv.Count > 0)
                {
                    if (MessageBox.Show(string.Format("大类\"{0}\"数据发生更改，如不需保存，请确定！", strDQCLSXDL), "询问！", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        fun_刷新();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    fun_刷新();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
