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
    public partial class ui小批订单 : UserControl
    {
        public ui小批订单()
        {
            InitializeComponent();
            strconn = CPublic.Var.strConn;
        }
                
         #region 成员
        //数据库连接字符串


        string cfgfilepath = "";
        //有变化的做保存
        string strconn = CPublic.Var.strConn;
        DataRow drr = null;
        string str_制令 = "";
        string str_制令单 = "";
        DataTable dt_视图权限;
        public Boolean a;
        DataTable dt_proZLysx;
        bool flag = false;   //用以标记是否是是改制工单  
        DataTable dt_计划池; //用以减去计划池相应数量
        #endregion

 
        #region  变量

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

        #endregion

        #region 类加载
        //查找物料的信息填充下拉框
#pragma warning disable IDE1006 // 命名样式
        private void fun_searchMaterial()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                SqlDataAdapter da;
                string sql = @"select base.物料编码,base.物料名称,特殊备注,base.规格型号,kc.仓库号,kc.仓库名称,
                       base.图纸编号,车间编号,库存总数,新数据
                       from 基础数据物料信息表 base,仓库物料数量表 kc
where   base.物料编码=kc.物料编码 and base.自制=1 and kc.仓库号  in (select  属性字段1 as 仓库号 from  基础数据基础属性表 where 属性类别='仓库类别' and 布尔字段1=1 )";//base.物料类型<>'原材料'
                dt_wuliao = new DataTable();
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_wuliao);
                repositoryItemSearchLookUpEdit1.DataSource = dt_wuliao;
                // rsl.PopulateColumns();
                repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
                repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";


                fun_search3();
              

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_searchMaterial");
                throw new Exception(ex.Message);
            }

        }

   
#pragma warning disable IDE1006 // 命名样式
        private void fun_loadsczlMain()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                string sql = "";                        
                  sql = string.Format(@"select *   from 小批订单表 where  1<>1 ");
                    
                    dt_proZL = new DataTable();
                    dt_proZL=   CZMaster.MasterSQL.Get_DataTable(sql,strconn);
                  //  da.Fill(dt_proZL);
                    dt_proZL.Columns.Add("选择", typeof(bool));
                    dt_proZL.Columns.Add("库存总数",typeof(decimal)); 
                    dt_proZL.Columns.Add("反馈备注");      
                   gc_未生效制令.DataSource = dt_proZL;
       
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_loadsczlMain");
                throw new Exception(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_search3()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = "select 项目名称,项目号 from 基础信息项目管理表";
            DataTable dt_item = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            repositoryItemSearchLookUpEdit3.DataSource = dt_item;
            // rsl.PopulateColumns();
            repositoryItemSearchLookUpEdit3.DisplayMember = "项目名称";
            repositoryItemSearchLookUpEdit3.ValueMember = "项目号";



        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_load已生效制令()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                SqlDataAdapter da;
                string sql = "";
                if (date_前.EditValue != null && date_前.EditValue.ToString() != "" && date_后.EditValue != null && date_后.EditValue.ToString() != "")
                {
                    sql = string.Format("and sczl.生效日期 >= '{0}' and sczl.生效日期 <= '{1}'", Convert.ToDateTime(date_前.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(date_后.EditValue).AddDays(1).ToString("yyyy-MM-dd"));
                }
                if (CPublic.Var.LocalUserID == "admin" || CPublic.Var.LocalUserTeam == "公司高管权限")//|| CPublic.Var.LocalUserID == "2101" || CPublic.Var.LocalUserID == "2233" || CPublic.Var.LocalUserID == "4136" || CPublic.Var.LocalUserID == "2106"
                {
                    sql = string.Format(@"select sczl.*,isnull(a.已转工单数,0)已转工单数,库存总数,新数据 from 小批订单表 sczl
        left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
        left join  仓库物料数量表 kc   on    kc.物料编码= sczl.物料编码
        left join (select sum(生产数量) as 已转工单数,订单号  from  生产记录生产工单表 where 关闭= 0  group by 订单号) a  on sczl.订单号=a.订单号
       where  sczl.关闭=0 and sczl.未排单数量>0 and sczl.生效 = 1 and sczl.关闭=0   and sczl.生效日期 >= '2016-12-1' and  sczl.仓库号=kc.仓库号 {0} and sczl.生产制令类型='研发样品' ", sql);
                }
                else
                {
                    sql = string.Format(@"select sczl.*,isnull(a.已转工单数,0)已转工单数,库存总数,新数据  from 小批订单表 sczl
                                           left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
                                              left join  仓库物料数量表 kc on    kc.物料编码= sczl.物料编码
                           left join (select sum(生产数量) as 已转工单数,订单号  from  生产记录生产工单表 where 关闭= 0  group by 订单号) a  
                                    on sczl.订单号=a.订单号
                                 where  sczl.生效 = 1 and  (操作人员ID='{0}'  or 生产制令类型='销售备库') and  sczl.未排单数量>0  
                                    and sczl.生效日期 >= '2016-12-1' and sczl.关闭=0 and sczl.仓库号=kc.仓库号  {1} and sczl.生产制令类型='研发样品' ", CPublic.Var.LocalUserID, sql);
                }



                //sql += " and ( ";
                //foreach (DataRow r in dt_视图权限.Rows)
                //{
                //    sql += "操作人员ID = '" + r["工号"].ToString().Trim() + "' or ";
                //}
                //sql = sql.Substring(0, sql.Length - 3);
                //sql = sql + " )";
                da = new SqlDataAdapter(sql, strconn);
                dt_proZLysx = new DataTable();
                da.Fill(dt_proZLysx);
                gc_已生效制令.DataSource = dt_proZLysx;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


    
        #endregion

        #region 调用的相关方法

        //新增行
#pragma warning disable IDE1006 // 命名样式
        private void fun_AddNewRow()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataRow r = dt_proZL.NewRow();

                dt_proZL.Rows.Add(r);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_AddNewRow");
                throw new Exception(ex.Message);
            }
        }

        //生效减去计划池的数量    11/14     gh  //如果是MRP 类型就不用  调用这个减计划池 
        //最新的 计划采购界面 就不用 计划池了  生产记录生产计划表 也不用了

#pragma warning disable IDE1006 // 命名样式
        private DataTable fun_减计划池()
#pragma warning restore IDE1006 // 命名样式
        {
            DataTable dt = new DataTable();

            foreach (DataRow dr in dt_proZL.Rows)
            {
                if (dr["选择"].Equals(true) && dr["生产制令类型"].ToString() == "标准类型")
                {

                    if (dt.Rows.Count == 0)
                    {
                        string sql = string.Format("select  * from [生产记录生产计划表] where 物料编码='{0}'", dr["物料编码"]);
                        using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                        {
                            da.Fill(dt);
                        }

                    }

                    DataRow[] r = dt.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (r.Length == 0)     //加载所有 需要操作的数据
                    {
                        string sql = string.Format("select  * from [生产记录生产计划表] where 物料编码='{0}'", dr["物料编码"]);
                        using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                        {
                            da.Fill(dt);
                        }
                        DataRow[] rr = dt.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                        if (rr.Length > 0)
                        {
                            decimal dec = Convert.ToDecimal(rr[0]["计划数量"]) - Convert.ToDecimal(dr["订单数量"]);
                            if (dec > 0)
                            {
                                rr[0]["未生成数量"] = rr[0]["计划数量"] = dec;

                            }
                            else
                            {
                                rr[0]["未生成数量"] = rr[0]["计划数量"] = 0;
                            }
                        }

                    }
                    else
                    {

                        decimal dec = Convert.ToDecimal(r[0]["计划数量"]) - Convert.ToDecimal(dr["订单数量"]);
                        if (dec > 0)
                        {
                            r[0]["未生成数量"] = r[0]["计划数量"] = dec;
                        }
                        else
                        {
                            r[0]["未生成数量"] = r[0]["计划数量"] = 0;
                        }
                    }


                }
            }

            return dt;
        }

        //检测制令明细的数据的合法性
#pragma warning disable IDE1006 // 命名样式
        private void fun_checkSaveMXData()
#pragma warning restore IDE1006 // 命名样式
        {
            //try
            //{
            //    string strzld = "";
            //    int pos = 1;
            //    foreach (DataRow r in dt_proZLdetail.Rows)
            //    {
            //      // if(str)

            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        //检查保存制令的数据的合法性
#pragma warning disable IDE1006 // 命名样式
        private void fun_checkSaveZLData()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();
                string   str_name = CPublic.Var.localUserName;
                string str_id = CPublic.Var.LocalUserID;
                foreach (DataRow r in dt_proZL.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                  
                    if (r["物料编码"].ToString() == "")
                        throw new Exception("物料编码不能为空，请选择！");
                    if (r["预完工日期"].ToString() == "")
                        throw new Exception("预完工日期不能为空，请选择！");
                    if (r["订单数量"].ToString() == "")
                        throw new Exception("订单数量不能为空，请填写！");                  
                    try
                    {
                        decimal dd = Convert.ToDecimal(r["订单数量"]);
                    }
                    catch
                    {
                        throw new Exception("订单数量应该是数字，请重新填写！");
                    }

                    //如果GUID是空的说明是新增的
                    if (r["GUID"].ToString().Trim() == "")
                    {
                        r["操作人员"] = str_name;
                        r["操作人员ID"] = str_id;

                        r["GUID"] = System.Guid.NewGuid();

                        r["订单号"] = string.Format("XP{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                            CPublic.CNo.fun_得到最大流水号("XP", t.Year, t.Month));
                      //  r["项目名称"]=repositoryItemSearchLookUpEdit3
                        r["日期"] = t;
                        r["制单人员"] = str_name;
                        r["制单人员ID"] = str_id;
                    }

                
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_checkSaveZLData");
                throw new Exception(ex.Message);
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_checkZLSX()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
          
                //检查勾选的是否有明细,无明细是不能生效的。
                //foreach (DataRow r in dt_SXZL.Rows)
                //{
                //    DataRow[] dr = dt_proZLdetail.Select(string.Format("订单号='{0}'", r["订单号"].ToString()));
                //    if (dr.Length <= 0)
                //        throw new Exception(string.Format("订单号\"{0}\"，无制令明细，不可生效！", r["订单号"].ToString()));
                //}
                //循环制令子表检测有没有新增的没保存的
                string str = "";

                //物料是否有效
                foreach (DataRow r in dt_SXZL.Rows)
                {
                    if (r["预完工日期"].ToString() == "")
                        throw new Exception("请填写预完工日期");
                    //物料是否有效
                    string sql_物料是否有效 = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"].ToString());
                    DataTable dt_基础物料 = new DataTable();
                    dt_基础物料 = CZMaster.MasterSQL.Get_DataTable(sql_物料是否有效, CPublic.Var.strConn);
                    if (dt_基础物料.Rows.Count == 0)
                    {
                        throw new Exception(string.Format("物料'{0}'无效，基础数据物料信息表中不存在该物料信息", r["物料编码"].ToString()));
                    }

                    //物料是否初始化
                    //string sql_物料是否初始化 = string.Format("select * from 仓库物料表 where 物料编码 = '{0}'", r["物料编码"].ToString());
                    //DataTable dt_物料是否初始化 = new DataTable();
                    //dt_物料是否初始化 = CZMaster.MasterSQL.Get_DataTable(sql_物料是否初始化, CPublic.Var.strConn);
                    //if (dt_物料是否初始化.Rows.Count == 0)
                    //{
                    //    throw new Exception(string.Format("物料'{0}'无效，仓库物料表中不存在该物料信息", r["物料编码"].ToString()));
                    //}
                    str = str + StockCore.StockCorer.fun_flag(r["物料编码"].ToString(), false);

                }
                if (str.Trim() != "")
                {
                    throw new Exception(str);
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_checkZLSX");
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

                foreach (DataRow r in dt_proZL.Rows)
                {

                    if (r.RowState == DataRowState.Deleted)
                    {

                        continue;
                    }

                        r["生效"] = 1;
                        r["生效人员ID"] = CPublic.Var.LocalUserID;
                        r["生效人员"] = CPublic.Var.localUserName;
                        r["生效日期"] = CPublic.Var.getDatetime(); ;
                  
                }

                SqlDataAdapter da;
                string sql = "select * from 小批订单表 where 1<>1";
                da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dt_proZL);
                
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_SaveData");
                throw new Exception(ex.Message);
            }
        }

        #endregion

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                gv_未生效制令.CloseEditor();
                this.BindingContext[dt_proZL].EndCurrentEdit();
                fun_loadsczlMain();
              //  fun_load已生效制令();
                fun_searchMaterial();
                //button2.Text = "显示所有";
                gv_已生效制令.ViewCaption = "未排单生效制令";

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

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (dt_proZL == null || dt_proZL.Rows.Count <= 0)
                    throw new Exception("没有订单可以删除！");
                DataRow r = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);
                if (r.RowState != DataRowState.Added)
                {
                    if (MessageBox.Show(string.Format("请确定要删除订单号为\"{0}\"的生产制令吗？", r["订单号"].ToString()), "提示？", MessageBoxButtons.OKCancel) == DialogResult.OK)
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
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {



                gv_未生效制令.CloseEditor();
                this.BindingContext[dt_proZL].EndCurrentEdit();
                if (dt_proZL.Rows.Count<=0)
                {
                    throw new Exception("当前无数据");
                }

                fun_checkSaveZLData();
             //   fun_checkZLSX();
                fun_SaveData();
                barLargeButtonItem1_ItemClick(null, null);
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void ui小批订单_Load(object sender, EventArgs e)
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


                if (File.Exists(cfgfilepath + string.Format(@"\{0}.xml", this.Name + "1")))
                {

                    gv_未生效制令.RestoreLayoutFromXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name + "1"));
                }
           
                if (File.Exists(cfgfilepath + string.Format(@"\{0}.xml", this.Name + "3")))
                {

                    gv_已生效制令.RestoreLayoutFromXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name + "3"));
                }

                // fun_search3();
                fun_searchMaterial();
                fun_loadsczlMain();
             //   fun_searchMaterial()
              //  fun_load已生效制令();

                if (drr != null)
                {
                    DataRow[] r = dt_proZL.Select(string.Format("订单号='{0}' ", str_制令));

                    r[0]["选择"] = true;
                    gv_未生效制令.Focus();
                    gv_未生效制令.FocusedRowHandle = gv_未生效制令.LocateByDisplayText(0, gridColumn2, str_制令);
                    gv_未生效制令.SelectRow(gv_未生效制令.FocusedRowHandle);
                }
                if (gv_未生效制令.RowCount > 0)
                {
                    gv_未生效制令.GetDataRow(0)["选择"] = false;
                }
             //   gv_sczlmain_RowCellClick_1(null, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_未生效制令_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {


        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_未生效制令_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
          

        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_未生效制令_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Column.FieldName == "物料编码")
            {
                gv_未生效制令.CloseEditor();
                this.BindingContext[dt_proZL].EndCurrentEdit();
                DataRow drM = (this.BindingContext[gc_未生效制令.DataSource].Current as DataRowView).Row;
                string sql = string.Format(@"select base.物料编码,base.物料名称,特殊备注,base.规格型号,kc.仓库号,kc.仓库名称,
                       base.图纸编号,车间编号,库存总数,新数据
                       from 基础数据物料信息表 base,仓库物料数量表 kc
      where   base.物料编码=kc.物料编码 and base.自制=1 and kc.仓库号  in (select  属性字段1 as 仓库号 from  基础数据基础属性表 where 属性类别='仓库类别' and 布尔字段1=1 ) and base.物料编码='{0}' ", drM["物料编码"].ToString());//base.物料类型<>'原材料'
                dt_wuliao = new DataTable();

                dt_wuliao = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                if (dt_wuliao.Rows.Count > 0)
                {
                    drM["规格型号"] = dt_wuliao.Rows[0]["规格型号"];
                    drM["图纸编号"] = dt_wuliao.Rows[0]["图纸编号"];
                    // drM["生产车间"] = dt_wuliao.Rows[0]["生产车间"];
                  
                         drM["物料名称"] = dt_wuliao.Rows[0]["物料名称"];
                    drM["库存总数"] = dt_wuliao.Rows[0]["库存总数"];

                }
            }


        }

#pragma warning disable IDE1006 // 命名样式
        private void button4_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

       //     sql = string.Format("and sczl.生效日期 >= '{0}' and sczl.生效日期 <= '{1}'", );

            string sql = string.Format(@" select * from 小批订单表 where 生效日期>'{0}' and 生效日期 <'{1}' ", Convert.ToDateTime(date_前.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(date_后.EditValue).AddDays(1).ToString("yyyy-MM-dd"));

            DataTable dt = new DataTable();
            dt =CZMaster.MasterSQL.Get_DataTable(sql,strconn);
            gc_已生效制令.DataSource=dt;


        }

#pragma warning disable IDE1006 // 命名样式
        private void label4_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void label2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void label1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void date_后_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void date_前_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }
    }
}
