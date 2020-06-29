using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CPublic;
using CZMaster;


namespace DetectionPlatformBaseData
{
    public partial class fm检测动作子表维护 : Form
    {

        
        #region  变量

        string jcid = "";
        string jczpos = "";
        string jcmc = "";
        DataTable dtP;  //检测动作子表 只是加载部分
        /// <summary>
        /// 某一个检测ID的全部动作
        /// </summary>
        DataTable dtP1; //加载全部的动作子表
        DataTable dt;  //ABB动作表
        string strms = "";


        #endregion


        #region  类加载

        public fm检测动作子表维护(string jcid, string jcmc, string jczpos)
        {
            InitializeComponent();
            this.jcid = jcid;
            this.jczpos = jczpos;
            this.jcmc = jcmc;
        }

        private void fm检测动作子表维护_Load(object sender, EventArgs e)
        {
            try
            {
                #region gridcontrol汉化代码
                //DevExpress.Accessibility.AccLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressUtilsLocalizationCHS();
                ////DevExpress.XtraBars.Localization.BarLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraBarsLocalizationCHS();
                ////DevExpress.XtraCharts.Localization.ChartLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraChartsLocalizationCHS();
                //DevExpress.XtraEditors.Controls.Localizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraEditorsLocalizationCHS();
                //DevExpress.XtraGrid.Localization.GridLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraGridLocalizationCHS();
                //DevExpress.XtraLayout.Localization.LayoutLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraLayoutLocalizationCHS();
                ////DevExpress.XtraNavBar.NavBarLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraNavBarLocalizationCHS();
                ////DevExpress.XtraPivotGrid.Localization.PivotGridLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraPivotGridLocalizationCHS();
                //DevExpress.XtraPrinting.Localization.PreviewLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraPrintingLocalizationCHS();
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

                //下拉的选择框
                fun_加载动作表();
                repositoryItemSearchLookUpEdit1.DataSource = dt;
                repositoryItemSearchLookUpEdit1.ValueMember = "动作ID";
                repositoryItemSearchLookUpEdit1.DisplayMember = "动作ID";     
                fun_load();
                fun_loadDtp();
                dtP.ColumnChanged += dtP_ColumnChanged;
                (this.BindingContext[dtP] as CurrencyManager).PositionChanged += fm检测动作子表维护_PositionChanged;
                //当前的维护说明
                label12.Text = jcid;
                label13.Text = jcmc;
                label14.Text = jczpos;
                strms= "当前维护的检测编号：" + jcid + " | 检测名称：" + jcmc + " | 检测组POS：" + jczpos;
                gvM.ViewCaption = strms;
               
            }          
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        void dtP_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column.ColumnName == "动作ID")
            {
                DataTable dt111 = repositoryItemSearchLookUpEdit1.DataSource as DataTable;
                DataRow[] rs = dt.Select(string.Format("动作ID = '{0}'", e.Row["动作ID"].ToString()));
                if (rs.Length > 0)
                {
                    e.Row["动作描述"] = rs[0]["动作描述"];
                    e.Row["动作说明"] = rs[0]["动作说明"];
                    if (Convert.ToInt32(rs[0]["动作参数个数"]) > 0)  //有参数的个数的时候
                    {
                        e.Row["参数个数及说明"] = rs[0]["动作参数个数"].ToString() + @" 个 " + rs[0]["动作参数说明"].ToString();
                    }
                    if (Convert.ToInt32(rs[0]["动作参数个数"])==0)
                    {
                        e.Row["参数个数及说明"] = "";
                    }
                }

            }
        }

        void fm检测动作子表维护_PositionChanged(object sender, EventArgs e)
        {
            int i = 0;
            foreach (DataRow r in dtP.Rows)
            {
                if (r.RowState != DataRowState.Deleted)
                {
                    i += 1;
                }
            }
            if(i!=0)
            {
                DataRow r = (this.BindingContext[dtP].Current as DataRowView).Row;
                if (r["动作ID"].ToString() != "")
                {
                    DataRow[] dr = dt.Select(string.Format("动作ID='{0}'", r["动作ID"].ToString()));

                    label6.Text = r["动作ID"].ToString();
                    label7.Text = dr[0]["动作说明"].ToString();
                    label8.Text = dr[0]["动作参数个数"].ToString();
                    if (dr[0]["动作参数说明"].ToString() == "")
                    {
                        label9.Text = "无";
                    }
                    if (dr[0]["动作参数说明"].ToString() != "")
                    {
                        label9.Text = dr[0]["动作参数说明"].ToString();
                    }
                    //label1.Text = "当前选中动作编号：" + r["动作ID"].ToString() + "  |说明：" + dr[0]["动作说明"].ToString() + "  |参数个数：" + dr[0]["动作参数个数"].ToString() + "  |参数说明：" + dr[0]["动作参数说明"].ToString();             
                    gvM.ViewCaption = "当前选中动作编号：" + r["动作ID"].ToString() + "  |说明：" + dr[0]["动作说明"].ToString() + "  |参数个数：" + dr[0]["动作参数个数"].ToString() + "  |参数说明：" + dr[0]["动作参数说明"].ToString();
                } 
            }
     
          
              
          
            
        }


        #endregion


        #region 数据检查

        //数据的检查
        private void fun_check()
        {
            foreach (DataRow r in dtP.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;
                //检测组内POS
                if (r["检测组内POS"].ToString() == "")
                    throw new Exception("检测组内动作顺序不允许为空，请检查！");
                try
                {
                    int i = Convert.ToInt32(r["检测组内POS"].ToString());
                }
                catch
                {
                    throw new Exception("检测组内动作顺序需要是数字，请检查！");
                }
                //报表节点：填写的是电压点。要求是数字
                if (r["报表节点"].ToString() != "")
                {
                    try
                    {
                        double i = Convert.ToDouble(r["报表节点"].ToString());
                    }
                    catch
                    {
                        throw new Exception("报表节点是数字，请填写数字！");
                    }
                }

                DataRow[] dr = dtP.Select(string.Format("检测组内POS='{0}'", r["检测组内POS"].ToString()));
                if (dr.Length > 1)
                    throw new Exception(string.Format("检测组内动作顺序\"{0}\",有重复，请检测！", r["检测组内POS"].ToString()));
                #region 注释代码行

                //动作POS
                //if (r["动作POS"].ToString() == "")
                //    throw new Exception("动作顺序不允许为空，请检查！");
                //try
                //{
                //    int i = Convert.ToInt32(r["动作POS"].ToString());
                //}
                //catch
                //{
                //    throw new Exception("动作顺序需要是数字，请检查！");
                //}

                //检测动作顺序是否有重复
                //DataRow[] dr1 = dtP.Select(string.Format("检测ID='{0}' and 动作POS='{1}'",jcid,r["动作POS"].ToString()));   //整个dtp1子表
                //if (dr1.Length > 1)
                //    throw new Exception(string.Format("动作顺序\"{0}\",有重复，请重新填写！", r["动作POS"].ToString()));

                #endregion   
                ////参数的个数检查
                if (r["动作ID"].ToString() == "")
                    throw new Exception("动作编号不能空，请选择！");


                #region 动作参数个数检查
                DataRow[] dr2 = dt.Select(string.Format("动作ID='{0}'", r["动作ID"].ToString()));
                switch(Convert.ToInt32(dr2[0]["动作参数个数"]))
                {
                    case 0:;break;
                    case 1: if (r["P1"].ToString() == "")
                        {
                            r["P2"] = ""; r["P3"] = ""; r["P4"] = ""; r["P5"] = "";
                            throw new Exception(string.Format("动作编号\"{0}\"有1个参数，请填写参数1 ！", r["动作ID"].ToString()));
                        }        
                        break;
                    case 2: if (r["P1"].ToString() == "" || r["P2"].ToString() == "")
                        {
                            r["P3"] = ""; r["P4"] = ""; r["P5"] = "";
                            throw new Exception(string.Format("动作编号\"{0}\"有2个参数，请填写参数1，参数2 ！", r["动作ID"].ToString()));
                        }
                        break;
                    case 3: if (r["P1"].ToString() == "" || r["P2"].ToString() == "" || r["P3"].ToString() == "")
                        {
                            r["P4"] = ""; r["P5"] = "";
                            throw new Exception(string.Format("动作编号\"{0}\"有3个参数，请填写参数1，参数2，参数3 ！", r["动作ID"].ToString()));
                        }
                        break;
                    case 4: if (r["P1"].ToString() == "" || r["P2"].ToString() == "" || r["P3"].ToString() == "" || r["P4"].ToString() == "")
                        {
                            r["P5"] = "";
                            throw new Exception(string.Format("动作编号\"{0}\"有4个参数，请填写参数1，参数2，参数3，参数4 ！", r["动作ID"].ToString()));
                        }                           
                        break;
                    case 5:if(r["P1"].ToString()=="" || r["P2"].ToString()=="" || r["P3"].ToString()==""||r["P4"].ToString()==""||r["P5"].ToString()=="")
                            throw new Exception(string.Format("动作编号\"{0}\"有5个参数，请填写参数1，参数2，参数3，参数4，参数5 ！", r["动作ID"].ToString()));
                        break;
                }
                #endregion
            }
        }


        #endregion



        #region   数据加载

        //新增一列
        private void fun_新增()
        {
            DataRow r = dtP.NewRow();
            r["动作子表GUID"] = System.Guid.NewGuid().ToString();
            r["检测ID"] = jcid;
            r["检测名称"] = jcmc;
            r["检测组POS"] = jczpos;
            dtP.Rows.Add(r);
        }

        //动作表的加载
        private void fun_加载动作表()
        {
            string sql = "select * from ABB动作表";
            dt = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));          
        }

        //加载某一动作组的详细动作
        private void fun_load()
        {
            string sql = string.Format("select * from ABB检测组动作子表 where 检测ID='{0}' and 检测名称='{1}' and 检测组POS='{2}' order by 检测组内POS", jcid, jcmc, jczpos);
            dtP= MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
            gcM.DataSource = dtP;
        }

        //全部加载
        private void fun_loadDtp()
        {
            string sql = string.Format("select * from ABB检测组动作子表 where 检测ID='{0}' and 检测名称='{1}'", jcid, jcmc);
            dtP1 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
        }

        //数据的保存
        private void fun_保存()
       {     
          //当dtp有数据的时候，排布检测组内的POS
           if (dtP.Rows.Count > 0 && dtP != null)
           {
               DataView dv = new DataView(dtP);
               dv.Sort = "检测组内POS";
               int j = 1;
               foreach (DataRowView drv in dv)
               {
                   DataRow r = drv.Row;
                   drv.Row["检测组内POS"] = j++;
               }
               MasterSQL.Save_DataTable(dtP, "ABB检测组动作子表", CPublic.Var.geConn("PLC"));
           }
          
           fun_loadDtp();  //全部加载出来进行排布动作组顺序
           DataView dv1 = new DataView(dtP1);
           dv1.Sort = "检测组POS,检测组内POS";
           int i = 1;
           foreach (DataRowView drv in dv1)
           {
               DataRow r = drv.Row;
               drv.Row["动作POS"] = i++;
           }
           MasterSQL.Save_DataTable(dtP1, "ABB检测组动作子表", CPublic.Var.geConn("PLC"));  
        }


        #endregion


        #region  界面相关操作

        //刷新操作
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_load();
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
                gvM.CloseEditor();
                (this.BindingContext[dtP] as CurrencyManager).EndCurrentEdit();
                fun_新增();
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
                if (dtP==null || dtP.Rows.Count <=0)
                    throw new Exception("没有数据可以删除！");
                gvM.CloseEditor();
                (this.BindingContext[dtP] as CurrencyManager).EndCurrentEdit();
                DataRow r = (this.BindingContext[dtP].Current as DataRowView).Row;
                if (MessageBox.Show(string.Format("确定删除动作编号为\"{0}\"的动作吗？", r["动作ID"].ToString()), "提示？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    r.Delete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //数据的保存操作
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (dtP == null || dtP.Rows.Count <= 0)
                    throw new Exception("没有数据！");
                gvM.CloseEditor();
                (this.BindingContext[dtP] as CurrencyManager).EndCurrentEdit();
                fun_check();
                fun_保存();
                fun_load();
                MessageBox.Show("保存成功！");
                dtP.ColumnChanged += dtP_ColumnChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //窗口关闭
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        #endregion



        #region      修改代码

        private void repositoryItemSearchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
          


        }


        #endregion























    }
}
