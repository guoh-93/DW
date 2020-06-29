using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace ERPpurchase
{
    public partial class frm采购计划_未用 : UserControl
    {
        string strcon = "";
        DateTime a;
        DateTime b;
        DateTime c;
        DataView dv;
        
        public frm采购计划_未用()
        {
            InitializeComponent();
            strcon = CPublic.Var.strConn;
            fun_MaterialsFind();
        }

        #region 变量
        /// <summary>
        /// 采购计划的DT表
        /// </summary>
        DataTable dt_PurchasePlan;

        /// <summary>
        /// 物料编码的dt表
        /// </summary>
        DataTable dt_materials;

        /// <summary>
        /// 选择生成采购单的
        /// </summary>
        DataTable dt_selResult;

        /// <summary>
        /// 供应商表
        /// </summary>
        DataTable dt_GYSID;

        /// <summary>
        /// 生成的采购单
        /// </summary>
        string strCgdh = "";

        /// <summary>
        ///用作数据的保留
        /// </summary>
        DataTable dt_cgjhData;

        #endregion

        #region 加载
        //物料编码的下拉框
        private void fun_MaterialsFind()
        {
            try
            {   //物料编码
                SqlDataAdapter da;
                string sql = @"select RTRIM(基础数据物料信息表.物料编码) as 物料编码,原ERP物料编号,物料名称,规格型号,图纸编号,图纸版本,库存下限,
                             标准单价,仓库号,仓库名称,采购周期,供应商编号,默认供应商,采购供应商表.供应商名称 from 基础数据物料信息表 
                             left join 采购供应商表 on 基础数据物料信息表.默认供应商=采购供应商表.供应商ID  where 基础数据物料信息表.停用=0";
                //string sql = "select 基础数据物料信息表.物料编码,物料名称,规格型号,图纸编号,标准单价,仓库号,仓库名称,采购供应商表.供应商名称 from 基础数据物料信息表,采购供应商表 where (基础数据物料信息表.物料类型='成品' or 基础数据物料信息表.物料类型='原材料') and 供应商ID=默认供应商";
                da = new SqlDataAdapter(sql, strcon);
                dt_materials = new DataTable();
                da.Fill(dt_materials);

                //供应商表
                sql = "select * from 采购供应商表";
                da = new SqlDataAdapter(sql, strcon);
                dt_GYSID = new DataTable();
                da.Fill(dt_GYSID);

                repositoryItemSearchLookUpEdit1.PopupFormSize = new Size(1400, 400);
                repositoryItemSearchLookUpEdit1.DataSource = dt_materials;
                repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
                repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_MaterialsFind");
                throw new Exception(ex.Message);
            }
        }

        //加载采购计划表的数据
        private void fun_loadPlan()
        {
            try
            {
                string sql_gys = string.Format("select * from [采购人员关联供应商表] where  员工号='{0}'", CPublic.Var.LocalUserID);
                DataTable dt_ls = CZMaster.MasterSQL.Get_DataTable(sql_gys, strcon);
                dt_cgjhData = new DataTable();
                dt_cgjhData.Columns.Add("采购计划明细号");
                dt_cgjhData.Columns.Add("已生成采购数量");
                dt_cgjhData.Columns.Add("未完成采购数量");
                SqlDataAdapter da;
                dt_PurchasePlan = new DataTable();
                string sql = "";
                DateTime dtime1 = System.DateTime.Today.AddMonths(-3);

//                sql = string.Format(@"select 采购记录采购计划表.*,基础数据物料信息表.大类,基础数据物料信息表.供应商编号,基础数据物料信息表.默认供应商 as 供应商,仓库物料数量表.库存总数, 
//                基础数据物料信息表.采购周期,基础数据物料信息表.标准单价 as 采购价格,基础数据物料信息表.仓库号 as 仓库ID,基础数据物料信息表.仓库名称,a.季度用量 
//                from 采购记录采购计划表
//                left join (select 物料编码,-sum(实效数量)as 季度用量  from 仓库出入库明细表 where  出库入库='出库' and 实效时间>'{0}'  group by 物料编码)a 
//                on 采购记录采购计划表.物料编码=a.物料编码
//                left join 基础数据物料信息表
//                on  采购记录采购计划表.物料编码=基础数据物料信息表.物料编码 left join 仓库物料数量表 
//                on 仓库物料数量表.物料编码 = 采购记录采购计划表.物料编码
//                where 采购记录采购计划表.作废=0 and 采购记录采购计划表.未完成采购数量 > 0  ", dtime1);
                sql = string.Format(@"select 采购记录采购计划表.*,
	                基础数据物料信息表.大类,基础数据物料信息表.供应商编号,基础数据物料信息表.默认供应商 as 供应商,
	                仓库物料数量表.库存总数, 
                    基础数据物料信息表.采购周期,基础数据物料信息表.库存下限,基础数据物料信息表.标准单价 as 采购价格,基础数据物料信息表.仓库号 as 仓库ID,
                    基础数据物料信息表.仓库名称,a.季度用量,isnull(b.在途量,0) as 在途量,isnull(c.未领量,0) as 未领量,isnull(d.受订量,0) as 受订量,
                    aa.待领量 ,isnull(e.送检量,0) as 送检量,(仓库物料数量表.库存总数 - isnull(c.未领量,0) - isnull(d.受订量,0)) as 当前可用量,
                    (未完成采购数量 + 库存下限) as 实缺数量
                    from 采购记录采购计划表
                    left join (select 物料编码,-sum(实效数量)as 季度用量  from 仓库出入库明细表 
                    where  出库入库='出库' and 实效时间>'{0}'  group by 物料编码)a 
                    on 采购记录采购计划表.物料编码=a.物料编码
                    left join (select 物料编码,sum(未完成数量) as 在途量 from 采购记录采购单明细表 
                    where 生效 = 1 and 明细完成日期 is null and 总完成 = 0 and 作废 = 0 and 生效日期 > '2016-11-01 00:00:00' group by 物料编码)b
                    on 采购记录采购计划表.物料编码=b.物料编码
                    left join (select 物料编码,sum(待领料总量) as 未领量 from 生产记录生产工单待领料明细表 
                    where 完成 = 0 and 创建日期 > '2016-11-01 00:00:00' group by 物料编码)c
                    on 采购记录采购计划表.物料编码=c.物料编码
                    left join (select 物料编码,sum(未完成数量) as 受订量 from 销售记录销售订单明细表 
                    where 生效 = 'True' and 明细完成 = 'False' and 作废 = 0 and 生效日期 > '2016-11-01 00:00:00' group by 物料编码)d
                    on 采购记录采购计划表.物料编码=d.物料编码
                    left join (select 物料编码,sum(送检数量)as 送检量 from [采购记录采购送检单明细表] 
					where 生效 = 1 and 作废 = 0 and 检验完成 = 0 and 生效日期 > '2016-11-01 00:00:00' group by 物料编码)e 
					on e.物料编码 = 采购记录采购计划表.物料编码
                    left join 基础数据物料信息表
                    on  采购记录采购计划表.物料编码=基础数据物料信息表.物料编码 
                    left join 仓库物料数量表 
                    on 仓库物料数量表.物料编码 = 采购记录采购计划表.物料编码
                    right join (SELECT sum([生产记录生产制令表].[未排单数量]*[基础数据物料BOM表].[总装数量]) as 待领量,[基础数据物料BOM表].子项编码
                    FROM [生产记录生产制令表] left join [基础数据物料BOM表] 
                    on [生产记录生产制令表].物料编码 = [基础数据物料BOM表].产品编码 where [生产记录生产制令表].生效 = 1 and [生产记录生产制令表].完成 = 0
                    and ([生产记录生产制令表].[未排单数量]*[基础数据物料BOM表].[总装数量]) is not null 
                    and ([生产记录生产制令表].[未排单数量]*[基础数据物料BOM表].[总装数量]) > 0
                    group by 基础数据物料BOM表.子项编码 )aa 
                    on 采购记录采购计划表.物料编码=aa.子项编码
                    where 采购记录采购计划表.作废=0 and 采购记录采购计划表.未完成采购数量 > 0 ", dtime1);
                //order by 供应商编号


                if (CPublic.Var.LocalUserID != "admin")
                {
                    if (dt_ls.Rows.Count > 0)
                    {
                        sql = sql + "and ( 基础数据物料信息表.供应商编号= '' or";
                        foreach (DataRow dr in dt_ls.Rows)
                        {
                            sql = sql + string.Format(" 基础数据物料信息表.供应商编号='{0}' or", dr["供应商ID"]);
                        }
                        sql = sql.Substring(0, sql.Length - 2);
                        sql = sql + ")";
                    }
                    else
                    {
                        throw new Exception("你没有对应的供应商,请找信息部核实");
                    }
                }
             
                da = new SqlDataAdapter(sql, strcon);
                da.Fill(dt_PurchasePlan);
                dt_PurchasePlan.Columns.Add("选择", typeof(bool)); 
                dt_PurchasePlan.Columns.Add("输入数量");
                dt_PurchasePlan.Columns.Add("已转采购单数量");
                dt_PurchasePlan.Columns.Add("待采购数量");
                dt_PurchasePlan.Columns.Add("不显示", typeof(string));

                //dt_yzcgNum为了实时刷新已经转采购单的数量
                DataTable dt_yzcgNum;
                //给默认供应商，采购价格，仓库ID，仓库名称赋值
                foreach (DataRow r in dt_PurchasePlan.Rows)
                {
                    r["待采购数量"] = r["未完成采购数量"];          
                     
                    sql = string.Format("select sum(采购数量) from 采购记录采购单明细表 where 采购计划明细号='{0}' and 生效=0 ", r["采购计划明细号"].ToString());
                    da = new SqlDataAdapter(sql, strcon);
                    dt_yzcgNum = new DataTable();
                    da.Fill(dt_yzcgNum);
                    if (dt_yzcgNum.Rows[0][0] != DBNull.Value)
                    {
                        r["已转采购单数量"] = dt_yzcgNum.Rows[0][0];
                        if (Convert.ToDecimal(r["已转采购单数量"]) >= Convert.ToDecimal(r["待采购数量"]))
                        {
                            r["不显示"] = "1";
                        }
                        else
                        {
                            r["不显示"] = "0";
                        }
                    }
                    else
                    {
                        r["不显示"] = "0";
                    }

                    if (r["采购计划类型"].ToString() == "MRP类型")
                    {
                        dt_cgjhData.Rows.Add(r["采购计划明细号"], r["已生成采购数量"], r["未完成采购数量"]);
                        r["已生成采购数量"] = System.DBNull.Value;
                        r["未完成采购数量"] = System.DBNull.Value;
                    } 

                    DataRow[] dr_xlk = dt_materials.Select(string.Format("物料编码='{0}'", r["物料编码"].ToString()));
                    if (dr_xlk.Length <= 0)
                    {
                        dt_materials.Rows.Add(r["物料编码"], r["物料名称"], r["规格型号"], r["图纸编号"]);
                    }
                }
                dv = new DataView(dt_PurchasePlan);
                dv.RowFilter = "不显示 = '0'";
                gc_Cgplan.DataSource = dv;
                //gc_Cgplan.DataSource = dt_PurchasePlan;  
                dt_PurchasePlan.ColumnChanged += dt_PurchasePlan_ColumnChanged; 
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_loadPlan");
                throw new Exception(ex.Message);
            }
        }
        
        private void frm采购计划_Load(object sender, EventArgs e)
        {
            
        }
        #endregion

        //物料编码改变时响应的也随之改变：物料名称，规格型号，图纸编号
        void dt_PurchasePlan_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            try
            {
                if (e.Column.ColumnName == "物料编码")
                {
                    DataRow[] dr = dt_materials.Select(string.Format("物料编码='{0}'", e.Row["物料编码"].ToString()));
                    if (dr.Length > 0)
                    {
                        e.Row["物料名称"] = dr[0]["物料名称"];
                        e.Row["规格型号"] = dr[0]["规格型号"];
                        e.Row["图纸编号"] = dr[0]["图纸编号"];
                        e.Row["采购价格"] = dr[0]["标准单价"];
                        e.Row["供应商"] = dr[0]["默认供应商"];
                        e.Row["图纸版本"] = dr[0]["图纸版本"];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
      
        private void fun_check()
        {
            if (dt_PurchasePlan.Rows[dt_PurchasePlan.Rows.Count-1].RowState == DataRowState.Added)
            {

            }
            else
            {
                throw new Exception("没有新增的计划，如要转采购单请点旁边的按钮");
            }
        }

        #region 数据处理：新增，保存，数据检查

        /// <summary>
        /// 检查保存数据的合法性
        /// </summary>
        private void fun_checkData()
        {
            try
            {
                foreach (DataRow r in dt_PurchasePlan.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    if (r.RowState == DataRowState.Unchanged) continue;

                    if (r["GUID"] == DBNull.Value) //标准类型
                    {
                        if (r["物料编码"].ToString() == "")
                        {
                            throw new Exception("物料编码不能为空，请选择！");
                        }

                        r["GUID"] = System.Guid.NewGuid();
                        try
                        {
                            r["数量"] = Convert.ToDecimal(r["待采购数量"]);
                        }
                        catch { r["数量"] = 0; }

                        if (r["数量"].ToString() == "")
                        {
                            throw new Exception("计划采购数量不能为空，请填写！");
                        }

                        try
                        {
                            decimal jhcgs = Convert.ToDecimal(r["数量"]);
                        }
                        catch
                        {
                            throw new Exception("计划采购数量应该为数字，请重新填写！");
                        }

                        r["已生成采购数量"] = 0;
                        try
                        {
                            r["未完成采购数量"] = Convert.ToDecimal(r["待采购数量"]);
                        }
                        catch { r["未完成采购数量"] = 0; }
                        r["总需数量"] = 0;
                        r["日期"] = System.DateTime.Now;
                        r["操作人员ID"] = CPublic.Var.LocalUserID;
                        r["操作人员"] = CPublic.Var.localUserName;
                        r["年"] = System.DateTime.Now.Year;
                        r["月"] = System.DateTime.Now.Month;
                        r["采购周期"] = 0;
                        r["是否生成"] = "无";
                        r["备注1"] = "无";
                        r["备注2"] = "无";
                        r["备注3"] = "无";
                        r["备注4"] = "无";
                        r["备注5"] = "无";
                        r["作废"] = 0;
                        r["完成"] = 0;
                        r["生效"] = 0;
                        r["节点标记"] = "无";
                        r["生成人员ID"] = "无";
                        r["生成人员"] = "无";

                        //if (r["采购计划类型"].ToString() == "MRP类型")
                        //{
                        //    //采购计划明细号
                        //    r["采购计划明细号"] = string.Format("MRP{0}{1:00}{2:00}{3:00000}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, CPublic.CNo.fun_得到最大流水号("CG", DateTime.Now.Year, DateTime.Now.Month));
                        //}
                        //else
                        {
                            //采购计划明细号
                            r["采购计划明细号"] = string.Format("PS{0}{1:00}{2:00}{3:0000}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, CPublic.CNo.fun_得到最大流水号("PS", DateTime.Now.Year, DateTime.Now.Month));
                        }
                    }

                    if (r["采购计划类型"].ToString() == "MRP类型")
                    {
                        r.AcceptChanges();
                        //DataRow[] dr_data = dt_cgjhData.Select(string.Format("采购计划明细号='{0}'", r["采购计划明细号"].ToString()));
                        //if (dr_data.Length > 0)
                        //{
                        //    r["已生成采购数量"] = dr_data[0]["已生成采购数量"];
                        //    r["未完成采购数量"] = dr_data[0]["未完成采购数量"];
                        //}
                        //else
                        //{
                        //    r["已生成采购数量"] = 0;
                        //    r["未完成采购数量"] = 0;
                        //}
                    }
                    else
                    {
                        if (r.RowState == DataRowState.Modified)
                        {
                            r.AcceptChanges();
                        }
                        //if (r["已生成采购数量"].ToString() == "" || r["未完成采购数量"].ToString() == "")
                        //    throw new Exception("标准类型的已生成采购数量和未完成采购数量不能为空，请填写！");
                        //try
                        //{
                        //    decimal ysc = (decimal)r["已生成采购数量"];
                        //    decimal wwc = (decimal)r["未完成采购数量"];
                        //}
                        //catch
                        //{
                        //    throw new Exception("未完成采购数量，已完成采购数量，应该为数字，请检查！");
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_checkData");
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 保存数据，只能修改未采购的量
        /// </summary>
        private void fun_saveData()
        {
            try
            {
                SqlDataAdapter da;
                string sql = "select * from 采购记录采购计划表 where 1<>1";
                da = new SqlDataAdapter(sql, strcon);
                DataTable dt = new DataTable();
                da.Fill(dt);
                
                foreach (DataRow r in dt_PurchasePlan.Rows)
                {
                    if (r.RowState == DataRowState.Deleted)
                    {                        
                        continue;
                    }
                    if (r.RowState == DataRowState.Added)
                    {
                        DataRow dr = dt.NewRow();
                        dt.Rows.Add(dr);
                        dr["GUID"] = r["GUID"];
                        dr["采购计划明细号"] = r["采购计划明细号"];
                        dr["采购计划类型"] = r["采购计划类型"];
                        dr["物料编码"] = r["物料编码"];
                        dr["物料名称"] = r["物料名称"];
                        dr["规格型号"] = r["规格型号"];
                        dr["图纸编号"] = r["图纸编号"];
                        dr["图纸版本"] = r["图纸版本"];
                        dr["数量"] = r["数量"];
                        dr["已生成采购数量"] = r["已生成采购数量"];
                        dr["未完成采购数量"] = r["未完成采购数量"];
                        dr["总需数量"] = r["总需数量"];
                        dr["日期"] = r["日期"];
                        dr["操作人员ID"] = r["操作人员ID"];
                        dr["操作人员"] = r["操作人员"];
                        dr["年"] = r["年"];
                        dr["月"] = r["月"];
                        r.AcceptChanges();
                    }
                }
                new SqlCommandBuilder(da);
                da.Update(dt);

                if (str != "where")
                {
                    str = str.Substring(0, str.Length - 3);
                    string sqll = "select * from 采购记录采购计划表 " + str;
                    DataTable dtt = new DataTable();
                    SqlDataAdapter daa = new SqlDataAdapter(sqll,strcon);
                    daa.Fill(dtt);
                    foreach (DataRow dr in dtt.Rows)
                    {
                        if (dr.RowState == DataRowState.Deleted) continue;
                        dr.Delete();
                    }
                    new SqlCommandBuilder(daa);
                    daa.Update(dtt);
                    str = "where";
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_saveData");
                throw new Exception(ex.Message); 
            }
        }

        //新增行，新增采购计划
        private void fun_AddNewRow()
        {
            try
            {
                DataRow r = dt_PurchasePlan.NewRow();
                r["采购计划类型"] = "标准类型";
                r["数量"] = 0;
                r["不显示"] = 0;
                dt_PurchasePlan.Rows.Add(r);
                gv_Cgplan.FocusedRowHandle = dv.Count - 1;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_AddNewRow");
                throw new Exception(ex.Message);   
            }
        }

        #endregion

        #region  界面操作

        //刷新功能
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_loadPlan();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //新增
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv_Cgplan.CloseEditor();
                this.BindingContext[dt_PurchasePlan].EndCurrentEdit();
               
                fun_AddNewRow();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //保存
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv_Cgplan.CloseEditor();
                this.BindingContext[dt_PurchasePlan].EndCurrentEdit();
                fun_checkData();
                fun_saveData();
                foreach (DataRow r in dt_PurchasePlan.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    if (r["采购计划类型"].ToString() == "标准类型" && (r.RowState == DataRowState.Added || r.RowState == DataRowState.Modified))
                    {
                        MessageBox.Show("保存成功！"); break;
                    }
                }
                fun_loadPlan();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        string str = "where";//删除时用到
        //删除操作
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (dt_PurchasePlan.Rows.Count > 0)
                {
                    DataRow r = gv_Cgplan.GetDataRow(gv_Cgplan.FocusedRowHandle);
                    if (r["采购计划类型"].ToString() == "MRP类型")
                        throw new Exception("MRP类型的采购计划不允许删除！");
                    if (r["采购计划类型"].ToString() == "标准类型")
                    {
                        try
                        {
                            if (Convert.ToDecimal(r["已生成采购数量"]) > 0)
                            {
                                throw new Exception("具有已生成采购数量的标准类型的采购计划不允许删除！");
                            }
                        }
                        catch { }
                    }
                    if (MessageBox.Show(string.Format("确定要删除采购计划明细号为\"{0}\"的采购计划吗？",r["采购计划明细号"].ToString()), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        str = str + " 采购计划明细号 = '" + r["采购计划明细号"].ToString() + "' and";
                        r.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //关闭按钮
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        #endregion

        #region 采购单的生成操作功能
        //检查选中行的数据的有效性
        private void fun_checkChoseRow()
        {
            try
            {
                //先检查有没有尚未保存的数据
                DataView dv = new DataView(dt_PurchasePlan);
                dv.RowStateFilter = DataViewRowState.Added | DataViewRowState.Deleted;
                if (dv.Count > 0)
                    throw new Exception("采购计划表的数据，有新增的或者删除的数据尚未保存，请先保存数据！");
                //检查已经选择的数据的有效性
                string gysid = dt_selResult.Rows[0]["供应商"].ToString();
                foreach (DataRow r in dt_selResult.Rows)
                {   //供应商是否一致，进行提醒
                    if (gysid != r["供应商"].ToString())
                    {
                        if (MessageBox.Show("你所选择生成采购单的计划数据，供应商不一致，是否生成采购单？", "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            //供应商不一致可以生成采购单
                        }
                        else
                        {
                            //return;
                            throw new Exception("请重新选择一致的供应商，再生成采购单！");
                        }
                    }
                    if (r["输入数量"].ToString() == "")
                        throw new Exception("请输入需要生成采购单的数量，输入数量不能为空，请检查！");
                    try
                    {
                        decimal d = Convert.ToDecimal(r["输入数量"]);
                    }
                    catch
                    {
                        throw new Exception("输入的数量应该为数字，请重新输入！");
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_checkChoseRow");
                throw new Exception(ex.Message);
            }
        }

        //选择生成采购单的行
        private void fun_ChoseDataRow()
        {
            try
            {
                dt_selResult = dt_PurchasePlan.Clone();
                foreach (DataRow r in dt_PurchasePlan.Rows)
                {
                    if (r["选择"].Equals(true))
                    {
                        dt_selResult.Rows.Add(r.ItemArray);
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_ChoseDataRow");
                throw new Exception(ex.Message);
            }
        }

        //为了跳转到采购单
        DataRow r_pm = null;

        //生成采购单:改变表中的数量
        private void fun_newPurchase()
        {
            try
            {
                SqlDataAdapter da;
                //采购单号          
                strCgdh = string.Format("PO{0}{1:00}{2:00}{3:0000}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, CPublic.CNo.fun_得到最大流水号("PO", DateTime.Now.Year, DateTime.Now.Month)); //采购单号
                //供应商的信息数据
                DataRow[] dr_gys = dt_GYSID.Select(string.Format("供应商ID = '{0}'", dt_selResult.Rows[0]["供应商编号"]));

                int pos = 1;
                decimal zje = 0;  //计算整个采购单的总金额
                DataTable dt_money = new DataTable();

                //采购单明细表
                string sql_m = "select * from 采购记录采购单明细表 where 1<>1";
                da = new SqlDataAdapter(sql_m, strcon);
                DataTable dt_purmx = new DataTable();
                da.Fill(dt_purmx);

                foreach (DataRow r in dt_selResult.Rows)
                {
                    DataRow r1 = dt_purmx.NewRow();
                    r1["计划采购量"] = r["未完成采购数量"];
                    r1["GUID"] = System.Guid.NewGuid();
                    r1["采购单号"] = strCgdh;
                    r1["采购明细号"] = strCgdh + "-" + pos.ToString("00");
                    r1["明细类型"] = r["采购计划类型"];
                    r1["采购POS"] = pos++;
                    r1["物料编码"] = r["物料编码"];
                    r1["物料名称"] = r["物料名称"];
                    r1["规格型号"] = r["规格型号"];
                    r1["图纸编号"] = r["图纸编号"];
                    r1["仓库ID"] = r["仓库ID"];
                    r1["仓库名称"] = r["仓库名称"];
                    r1["采购数量"] = r["输入数量"];
                    r1["单价"] = r["采购价格"];
                    r1["未税单价"] = Convert.ToDecimal(r["采购价格"])/Convert.ToDecimal(1.17);
                    if (dr_gys.Length > 0)
                    {
                        r1["供应商ID"] = dr_gys[0]["供应商ID"];
                        r1["供应商"] = dt_selResult.Rows[0]["供应商"];
                        r1["供应商负责人"] = dr_gys[0]["供应商负责人"];
                        r1["供应商电话"] = dr_gys[0]["供应商电话"];
                    }
                    r1["税率"] = 17;
                    r1["金额"] = Convert.ToDecimal(r["输入数量"]) * Convert.ToDecimal(r["采购价格"]);
                    //总金额
                    zje += (decimal)r1["金额"];
                    r1["未税金额"] = ((decimal)r1["金额"] / (decimal)1.17);
                    r1["税金"] = ((decimal)r1["金额"] / (decimal)1.17) * (decimal)0.17;
                    r1["员工号"] = CPublic.Var.LocalUserID;
                    r1["采购人"] = CPublic.Var.localUserName;
                    r1["未完成数量"] = r["输入数量"];
                    r1["操作员ID"] = CPublic.Var.LocalUserID;
                    r1["操作员"] = CPublic.Var.localUserName;
                    r1["采购计划明细号"] = r["采购计划明细号"];
                    r1["生成人员"] = CPublic.Var.localUserName;
                    dt_purmx.Rows.Add(r1);
                }

                //采购单主表
                string sql = "select * from 采购记录采购单主表 where 1<>1";
                da = new SqlDataAdapter(sql, strcon);
                DataTable dt_purDt = new DataTable();
                da.Fill(dt_purDt);

                r_pm = dt_purDt.NewRow();
                r_pm["GUID"] = System.Guid.NewGuid();  //GUID
                r_pm["采购单号"] = strCgdh; //采购单号
                r_pm["采购计划日期"] = System.DateTime.Now;
                r_pm["未税金额"] = (zje / (decimal)1.17);
                r_pm["税率"] = 17;
                r_pm["总金额"] = zje;
                r_pm["税金"] = (zje / (decimal)1.17) * (decimal)0.17;
                if (dr_gys.Length > 0)
                {
                    r_pm["供应商ID"] = dr_gys[0]["供应商ID"];
                    r_pm["供应商"] = dt_selResult.Rows[0]["供应商"];
                    r_pm["供应商负责人"] = dr_gys[0]["供应商负责人"];
                    r_pm["供应商电话"] = dr_gys[0]["供应商电话"];
                }
                r_pm["员工号"] = CPublic.Var.LocalUserID;
                r_pm["经办人"] = CPublic.Var.localUserName;
                r_pm["采购公司"] = "苏州未来电器股份有限公司";
                r_pm["录入日期"] = System.DateTime.Now;
                r_pm["创建日期"] = System.DateTime.Now;
                r_pm["修改日期"] = System.DateTime.Now;
                r_pm["操作员ID"] = CPublic.Var.LocalUserID;
                r_pm["操作员"] = CPublic.Var.localUserName;
                r_pm["生成人员"] = CPublic.Var.localUserName;
                dt_purDt.Rows.Add(r_pm);

                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("newPurchase");
                //SqlCommand cmd_cgjh = new SqlCommand("select * from 采购记录采购计划表 where 1<>1", conn, ts);
                SqlCommand cmd_cgzb = new SqlCommand("select * from 采购记录采购单主表 where 1<>1", conn, ts);
                SqlCommand cmd_cgmx = new SqlCommand("select * from 采购记录采购单明细表 where 1<>1", conn, ts);

                try
                {
                    //采购单主表
                    da = new SqlDataAdapter(cmd_cgzb);
                    new SqlCommandBuilder(da);
                    da.Update(dt_purDt);
                    //采购明细表
                    da = new SqlDataAdapter(cmd_cgmx);
                    new SqlCommandBuilder(da);
                    da.Update(dt_purmx);
                    ts.Commit();
                }
                catch
                {
                    ts.Rollback();
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_newPurchase");
                throw new Exception(ex.Message);
            }
        }

        //生效操作，生成采购单
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv_Cgplan.CloseEditor();
                this.BindingContext[dt_PurchasePlan].EndCurrentEdit();
                fun_ChoseDataRow();  //选择项
                if (dt_selResult.Rows.Count<=0)
                    throw new Exception("请选择需要生成采购单的采购计划！");
                fun_checkChoseRow(); //检查选择项
                fun_newPurchase();  //采购单生成
                fun_loadPlan();

                if (MessageBox.Show(string.Format("采购单\"{0}\"生成成功，是否跳转到采购单明细界面？", strCgdh), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    frm采购单明细 fm = new frm采购单明细(r_pm);
                    CPublic.UIcontrol.AddNewPage(fm, "采购单明细");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 行 可编辑判定
        private void gv_Cgplan_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            { 
                if (dt_PurchasePlan.Rows.Count <= 0)
                    throw new Exception("目前无采购计划！");
 
                DataRow r = gv_Cgplan.GetDataRow(gv_Cgplan.FocusedRowHandle);
                if (r.RowState == DataRowState.Deleted)
                    return;
                if (r.RowState != DataRowState.Added)
                {
                    foreach (DevExpress.XtraGrid.Columns.GridColumn dc in gv_Cgplan.Columns)
                    {
                        if (dc.FieldName != "选择" && dc.FieldName != "输入数量")
                        {
                            gv_Cgplan.Columns[dc.FieldName].OptionsColumn.AllowEdit = false;
                        }
                        else
                        {
                            gv_Cgplan.Columns[dc.FieldName].OptionsColumn.AllowEdit = true;
                        }
                    }
                }
                else
                {
                    foreach (DevExpress.XtraGrid.Columns.GridColumn dc in gv_Cgplan.Columns)
                    {
                        if (dc.FieldName == "物料编码" || dc.FieldName == "待采购数量")
                        {
                            gv_Cgplan.Columns[dc.FieldName].OptionsColumn.AllowEdit = true;
                        }
                        else
                        {
                            gv_Cgplan.Columns[dc.FieldName].OptionsColumn.AllowEdit = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + "采购计划池 行 可编辑判定");
            }
        }

        #endregion

        #region  界面的细节处理：如果是采购计划类型MRP类型，已完成数量，未完成数量为空，不显示。

        //已完成采购数量
        private void repositoryItemTextEdit1_ParseEditValue(object sender, DevExpress.XtraEditors.Controls.ConvertEditValueEventArgs e)
        {
            try
            {
                decimal dd = Convert.ToDecimal(e.Value);
            }
            catch
            {
                e.Value = null;
            }
        }

        //未生成采购数量
        private void repositoryItemTextEdit2_ParseEditValue(object sender, DevExpress.XtraEditors.Controls.ConvertEditValueEventArgs e)
        {
            try
            {
                decimal dd = Convert.ToDecimal(e.Value);
            }
            catch
            {
                e.Value = null;
            }
        }

        #endregion

        private void gv_Cgplan_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Clicks == 2)
            {
                //DataRow dr = gv_Cgplan.GetDataRow(gv_Cgplan.FocusedRowHandle);
                //ERPStock.frm仓库物料数量明细 frm = new ERPStock.frm仓库物料数量明细(dr["物料编码"].ToString());
                //CPublic.UIcontrol.AddNewPage(frm, "仓库物料数量明细");
            }
        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                options.ExportMode = DevExpress.XtraPrinting.XlsxExportMode.SingleFile;
                gc_Cgplan.ExportToXlsx(saveFileDialog.FileName,options);               
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void 查看过往出库明细ToolStripMenuItem_Click(object sender, EventArgs e)
       {
           //DataRow dr = gv_Cgplan.GetDataRow(gv_Cgplan.FocusedRowHandle);
           //UI查看出库通知明细 ui = new UI查看出库通知明细(dr["物料编码"].ToString());
           //CPublic.UIcontrol.AddNewPage(ui, "过往出库通知明细");
       }

        private void frm采购计划_Enter(object sender, EventArgs e)
        {
            try
            {
                repositoryItemSearchLookUpEdit1.PopupFormSize = new Size(1000, 300);
                panel1.Visible = false;
                //fun_MaterialsFind();
                b = System.DateTime.Now;//物料编码
                fun_loadPlan();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_Cgplan_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        #region 新数据提取方法
        private void fun_FK()
        {

        }
        #endregion
    }
}
