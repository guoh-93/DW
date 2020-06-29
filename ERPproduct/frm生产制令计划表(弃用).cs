using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using CZMaster;

namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class frm生产制令计划表 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {
        string strcon = "";

        public frm生产制令计划表()
        {
            InitializeComponent();
            strcon = CPublic.Var.strConn;
        }

        /// <summary>
        /// 生产计划表的dt
        /// </summary>
        DataTable dt_product;

        /// <summary>
        /// 生产明细的dt
        /// </summary>
        DataTable dt_productdetail;

        /// <summary>
        /// 物料信息表
        /// </summary>
        DataTable dt_wuliaoxinxi;

        /// <summary>
        /// 生产计划单号
        /// </summary>
        string strSCSN;

        /// <summary>
        /// 选择的生产计划
        /// </summary>
        DataTable dt_rescjh;

        /// <summary>
        /// 用来显示的明细
        /// </summary>
        DataTable dt_displaymx;

        /// <summary>
        /// 用来保留MRP的数据
        /// </summary>
        DataTable dt_DataMRP;

        /// <summary>
        /// 生产制令单号
        /// </summary>
        string strprozld = "";

        /// <summary>
        /// 权限的dt表
        /// </summary>
        DataTable dt_quanxian;


        //人员关联的大类
        DataTable dt_关联大类;






        #region 类加载
        //下拉框   物料信息的dt
#pragma warning disable IDE1006 // 命名样式
        private void fun_FindMaterials()
#pragma warning restore IDE1006 // 命名样式
        {
//            try
//            {
//                SqlDataAdapter da;
//                //                string sql = @"select 基础数据物料信息表.物料编码,原ERP物料编号,物料名称,物料类型,规格,特殊备注,n原ERP规格型号,图纸编号,车间,产品线,客户,客户基础信息表.客户名称 from 基础数据物料信息表,客户基础信息表 
//                //                                         where (基础数据物料信息表.客户=客户基础信息表.客户编号) and (基础数据物料信息表.物料类型='成品' or 基础数据物料信息表.物料类型='半成品')";
//                string sql = @"select 基础数据物料信息表.物料编码,原ERP物料编号,物料名称,物料类型,规格,特殊备注,n原ERP规格型号,图纸编号,车间编号,产品线,客户,客户基础信息表.客户名称 
//                    from 基础数据物料信息表 left join 客户基础信息表 
//                    on (基础数据物料信息表.客户=客户基础信息表.客户编号) 
//                    where (基础数据物料信息表.物料类型='成品' or 基础数据物料信息表.物料类型='半成品')";
//                dt_wuliaoxinxi = new DataTable();
//                da = new SqlDataAdapter(sql, strcon);
//                da.Fill(dt_wuliaoxinxi);

//                repositoryItemSearchLookUpEdit1.PopupFormSize = new Size(1400, 400);
//                repositoryItemSearchLookUpEdit1.DataSource = dt_wuliaoxinxi;
//                repositoryItemSearchLookUpEdit1.DisplayMember = "原ERP物料编号";
//                repositoryItemSearchLookUpEdit1.ValueMember = "原ERP物料编号";
//            }
//            catch (Exception ex)
//            {
//                CZMaster.MasterLog.WriteLog(ex.Message + " fun_FindMaterials");
//                throw new Exception(ex.Message);
//            }
        }

        //载入生产计划表的数据
#pragma warning disable IDE1006 // 命名样式
        private void fun_loadProductPlan()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                //dt_yzzlsl已转制令数量，为的实时刷新该数量让用户知道
                DataTable dt_yzzlsl = new DataTable();
                //dt_DataMRP数量为的是，记录MRP类型原有的已生成数量和未生成数量
                //dt_DataMRP = new DataTable();
                //dt_DataMRP.Columns.Add("生产计划单号");
                //dt_DataMRP.Columns.Add("已生成数量");
                //dt_DataMRP.Columns.Add("未生成数量");
                //增加输入生产数量和已转制令数量
                //增加单据权限
                SqlDataAdapter da;
                string sql = "";

                dt_productdetail = new DataTable();
                sql = "select *  from 销售记录销售订单明细表  where 1<>1";
                da = new SqlDataAdapter(sql, strcon);
                da.Fill(dt_productdetail);
                dt_productdetail.Columns.Add("选择", typeof(bool));
                dt_productdetail.Columns.Add("已选择", typeof(bool));
                dt_productdetail.Columns.Add("生产计划单号");

                dt_product = new DataTable();
                if (CPublic.Var.LocalUserID == "admin")
                {

//                    sql = @"select 生产记录生产计划表.*,
//				基础数据物料信息表.大类,基础数据物料信息表.小类,基础数据物料信息表.原ERP物料编号,n原ERP规格型号,
//				仓库物料数量表.库存总数,b.已转制令数量,a.在制量,d.受订量 
//				from 生产记录生产计划表
//                left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产计划表.物料编码
//                left join (select sum(制令数量) as 已转制令数量,物料编码 from 生产记录生产制令表 where  生效=0 group by 物料编码) as b  
//				on  b.物料编码=生产记录生产计划表.物料编码
//				left join (select 物料编码, sum(生产数量) as 在制量 from 生产记录生产工单表 
//				where 生效 = 1 and 完成 = 0 and 关闭 = 0 group by 物料编码)a
//				on 生产记录生产计划表.物料编码=a.物料编码
//				left join (select 物料编码,sum(未完成数量) as 受订量 from 销售记录销售订单明细表 
//				where 生效 = 'True' and 明细完成 = 'False' and 作废 = 0 group by 物料编码)d
//				on 生产记录生产计划表.物料编码=d.物料编码
//				left join 仓库物料数量表 on 仓库物料数量表.物料编码 = 生产记录生产计划表.物料编码
//                where 未生成数量>0 ";

                    sql = @"select 生产记录生产计划表.*,基础数据物料信息表.特殊备注 as 特备,
				基础数据物料信息表.大类,基础数据物料信息表.小类,基础数据物料信息表.原ERP物料编号,n原ERP规格型号,
				仓库物料数量表.库存总数,b.已转制令数量,在制量,受订量 
				from 生产记录生产计划表
                left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产计划表.物料编码
                left join (select sum(制令数量) as 已转制令数量,物料编码 from 生产记录生产制令表 where  生效=0 group by 物料编码) as b  
				on  b.物料编码=生产记录生产计划表.物料编码
				left join 仓库物料数量表 on 仓库物料数量表.物料编码 = 生产记录生产计划表.物料编码
                where 未生成数量>0 ";
                }
                else
                {

                    sql = string.Format(@"select 生产记录生产计划表.*,基础数据物料信息表.特殊备注 as 特备,
				基础数据物料信息表.大类,基础数据物料信息表.小类,基础数据物料信息表.原ERP物料编号,n原ERP规格型号,
				仓库物料数量表.库存总数,b.已转制令数量,在制量,受订量  
				from 生产记录生产计划表
                left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产计划表.物料编码
                left join (select sum(制令数量) as 已转制令数量,物料编码 from 生产记录生产制令表 where  生效=0 group by 物料编码) as b  
				on  b.物料编码=生产记录生产计划表.物料编码
				
				left join 仓库物料数量表 on 仓库物料数量表.物料编码 = 生产记录生产计划表.物料编码
                where 未生成数量>0 and 操作人员ID='{0}' ", CPublic.Var.LocalUserID);
                }
                // //按大小类分配到人
                //string sql_大小类 = string.Format("select *  from [基础数据物料类型表] where 计划员='{0}'", CPublic.Var.LocalUserID);
                //dt_关联大类 = CZMaster.MasterSQL.Get_DataTable(sql_大小类, strcon);
                //if (dt_关联大类.Rows.Count > 0)
                //{
                //    sql = sql + "and ( ";
                //    foreach (DataRow dr in dt_关联大类.Rows)
                //    {
                //        sql = sql + string.Format("基础数据物料信息表.{0}='{1}' or ", dr["类型级别"], dr["物料类型名称"]);
                //    }
                //    sql = sql.Substring(0, sql.Length - 3);
                //    sql = sql + ")";
                //}

                da = new SqlDataAdapter(sql, strcon);
                da.Fill(dt_product);

                dt_product.Columns.Add("选择", typeof(bool));
                dt_product.Columns.Add("输入生产数量");

                dt_product.Columns.Add("加急状态");
                dt_product.Columns.Add("显示计划数量");
                dt_product.Columns.Add("不显示", typeof(string));
                foreach (DataRow r in dt_product.Rows)
                {
                    dt_yzzlsl.Clear();
                    r["加急状态"] = "正常";

                    //r["物料编码"] = r["物料编码"].ToString().Trim();
                    //若果载入的物料编码，不在下拉框的dt中，就把该物料编码的信息加到下来框的dt中
                    //DataRow[] drr1 = dt_wuliaoxinxi.Select(string.Format("物料编码='{0}'", r["物料编码"].ToString()));
                    //if (drr1.Length <= 0)
                    //{
                    //    dt_wuliaoxinxi.Rows.Add(r["物料编码"], r["物料名称"], r["物料类型"], r["规格型号"], r["图纸编号"], r["生产线"], r["客户ID"], r["客户名称"]);
                    //}



                    //实时刷新已转制令的数量，已转制令的数量是根据生产计划单号，找到制令表里未生效的制令单号的物料编码，把数量加起来
                    //sql = string.Format("select sum(制令数量) as 已转制令数量 from 生产记录生产制令表 where 生产计划单号='{0}' and 生效=0", r["生产计划单号"].ToString());
                    //da = new SqlDataAdapter(sql, strcon);
                    //da.Fill(dt_yzzlsl);
                    //if (dt_yzzlsl.Rows.Count > 0)
                    //{
                    //if (dt_yzzlsl.Rows[0]["已转制令数量"] == null || dt_yzzlsl.Rows[0]["已转制令数量"] == DBNull.Value)
                    //{
                    //    dt_yzzlsl.Rows[0]["已转制令数量"] = 0;
                    //}
                    //if (r["已转制令数量"] == null || r["已转制令数量"] == DBNull.Value)
                    //{
                    //    r["已转制令数量"] = 0;
                    //}
                    //r["已转制令数量"] = dt_yzzlsl.Rows[0]["已转制令数量"];
                    if (r["未生成数量"] == DBNull.Value)
                    {
                        r["未生成数量"] = 0;
                    }

                    //if (r["已转制令数量"].ToString()=="" ||Convert.ToDecimal(r["已转制令数量"]) <= Convert.ToDecimal(r["未生成数量"]))
                    if (r["已转制令数量"].ToString() == "")
                    {
                        r["不显示"] = "0";
                    }
                    else if (Convert.ToDecimal(r["已转制令数量"]) >= Convert.ToDecimal(r["未生成数量"]))
                    {
                        r["不显示"] = "1";
                    }
                    else
                    {
                        r["不显示"] = "0";

                    }

                    //}

                    //如果是MRP类型，已生成数量和未生成数量显示未空
                    if (r["生产计划类型"].ToString() == "MRP类型")
                    {
                        r["显示计划数量"] = r["未生成数量"];
                        // dt_DataMRP.Rows.Add(r["生产计划单号"], r["已生成数量"], r["未生成数量"]);
                        ////把数据库有的数据先存到某个dt中去（dt_DataMRP中去）
                        r["已生成数量"] = 0;
                        r["未生成数量"] = 0;
                    }
                    else
                    {
                        r["显示计划数量"] = r["未生成数量"];
                    }
                }
                DataView dv = new DataView(dt_product);
                dv.RowFilter = "不显示 = '0'";
                gc_planproduct.DataSource = dv;
                dt_product.ColumnChanged += dt_product_ColumnChanged;
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + " fun_loadProductPlan");
                throw new Exception(ex.Message);
            }
        }

        //新增的时候选择物料编码，带出相应的数据（物料名称,客户名称）
#pragma warning disable IDE1006 // 命名样式
        void dt_product_ColumnChanged(object sender, DataColumnChangeEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (e.Column.ColumnName == "原ERP物料编号")
                {
                    DataRow[] dr = dt_wuliaoxinxi.Select(string.Format("原ERP物料编号='{0}'", e.Row["原ERP物料编号"].ToString()));
                    if (dr.Length > 0)
                    {
                        e.Row["物料编码"] = dr[0]["物料编码"];

                        e.Row["物料名称"] = dr[0]["物料名称"];
                        e.Row["规格型号"] = dr[0]["规格"];
                        e.Row["图纸编号"] = dr[0]["图纸编号"];
                        e.Row["客户ID"] = dr[0]["客户"];
                        e.Row["客户名称"] = dr[0]["客户名称"];

                        //e.Row["生产车间"] = dr[0]["车间"];
                        e.Row["生产车间"] = dr[0]["车间编号"];


                        e.Row["物料类型"] = dr[0]["物料类型"];
                        e.Row["特殊备注"] = dr[0]["特殊备注"];
                        e.Row["原规格型号"] = dr[0]["n原ERP规格型号"];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm生产制令计划表_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                devGridControlCustom1.UserName = CPublic.Var.LocalUserID;
                devGridControlCustom1.strConn = CPublic.Var.strConn;
                //权限的dt表
                dt_quanxian = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);
                fun_FindMaterials();
                fun_loadProductPlan();
                if (gv_计划.RowCount > 0)
                {
                    gv_计划.GetDataRow(0)["选择"] = false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region  调用方法
        //检查保存的生产计划单的数据
#pragma warning disable IDE1006 // 命名样式
        private void fun_checkSaveData()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                foreach (DataRow r in dt_product.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    if (r["生产计划类型"].ToString() == "")
                        throw new Exception("生产计划类型不能为空，请选择！");
                    //如果GUID为空，那就是新增的          
                    if (r["GUID"] == DBNull.Value)
                    {
                        r["GUID"] = System.Guid.NewGuid();
                        if (r["生产计划类型"].ToString() == "MRP类型")
                        {
                            strSCSN = string.Format("MRP{0}{1:00}{2:0000}", DateTime.Now.Year, DateTime.Now.Month, CPublic.CNo.fun_得到最大流水号("CG", DateTime.Now.Year, DateTime.Now.Month));
                        }
                        else
                        {
                            strSCSN = string.Format("PP{0}{1:00}{2:0000}", DateTime.Now.Year, DateTime.Now.Month, CPublic.CNo.fun_得到最大流水号("CG", DateTime.Now.Year, DateTime.Now.Month));
                        }
                        r["生产计划单号"] = strSCSN;
                        r["日期"] = System.DateTime.Now;
                        r["制单人员"] = CPublic.Var.localUserName;
                        r["制单人员ID"] = CPublic.Var.LocalUserID;
                        r["操作人员"] = CPublic.Var.localUserName;
                        r["操作人员ID"] = CPublic.Var.LocalUserID;
                        r["计划数量"] = r["显示计划数量"];
                        try
                        {
                            decimal djh = Convert.ToDecimal(r["计划数量"]);
                        }
                        catch
                        {
                            throw new Exception(string.Format("物料{0}的计划数量不是数字，请填写！", r["原ERP物料编号"]));
                        }
                    }

                    if (r["原ERP物料编号"].ToString() == "")
                        //throw new Exception("物料编码不能为空，请选择！");
                        if (r["计划数量"].ToString() == "")
                            throw new Exception(string.Format("物料{0}的计划数量不能为空，请填写！",r["原ERP物料编号"]));


                    if (r["生产计划类型"].ToString() == "MRP类型")
                    {
                        r.AcceptChanges();
                        //    DataRow[] dr_num = dt_DataMRP.Select(string.Format("生产计划单号='{0}'", r["生产计划单号"].ToString()));
                        //    if (dr_num.Length > 0)
                        //    {
                        //        r["已生成数量"] = dr_num[0]["已生成数量"];
                        //        r["未生成数量"] = dr_num[0]["未生成数量"];
                        //    }
                        //    else
                        //    {
                        //        r["已生成数量"] = 0;
                        //        r["未生成数量"] = 0;
                        //    }
                    }
                    else
                    {
                        if (r["已生成数量"].ToString() == "" || r["未生成数量"].ToString() == "")
                        {
                            // throw new Exception("标准类型的已生成数量和未生成数量不能为空，请填写！");
                            r["已生成数量"] = 0;
                            r["未生成数量"] = r["计划数量"];
                        }
                        try
                        {
                            decimal dd = Convert.ToDecimal(r["已生成数量"]);
                            decimal cc = Convert.ToDecimal(r["未生成数量"]);
                        }
                        catch
                        {
                            throw new Exception("已完成数量,为未完成数量中有不为数字，请检查！");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + " fun_checkSaveData");
                throw new Exception(ex.Message);
            }
        }

        //保存数据
#pragma warning disable IDE1006 // 命名样式
        private void fun_saveData()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                SqlDataAdapter da;
                string sql = "select * from 生产记录生产计划表 where 1<>1";
                da = new SqlDataAdapter(sql, strcon);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow r in dt_product.Rows)
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
                        dr["生产计划单号"] = r["生产计划单号"];
                        dr["生产计划类型"] = r["生产计划类型"];
                        dr["物料编码"] = r["物料编码"];
                        dr["物料名称"] = r["物料名称"];
                        dr["规格型号"] = r["规格型号"];
                        //dr["原规格型号"] = r["原规格型号"];
                        dr["原规格型号"] = r["n原ERP规格型号"];

                        dr["图纸编号"] = r["图纸编号"];
                        dr["生产线"] = r["生产线"];
                        dr["生产车间"] = r["生产车间"];
                        dr["物料类型"] = r["物料类型"];
                        dr["客户ID"] = r["客户ID"];
                        dr["客户名称"] = r["客户名称"];
                        dr["日期"] = r["日期"];
                        dr["操作人员ID"] = r["操作人员ID"];
                        dr["操作人员"] = r["操作人员"];
                        dr["制单人员ID"] = r["制单人员ID"];
                        dr["制单人员"] = r["制单人员"];
                        dr["计划数量"] = r["计划数量"];
                        dr["特殊备注"] = r["特殊备注"];
                        r.AcceptChanges();
                    }
                }
                new SqlCommandBuilder(da);
                da.Update(dt);

                if (str != "")
                {
                    str = str.Substring(0, str.Length - 3);
                    string sqll = "select * from 生产记录生产计划表 where" + str;
                    DataTable dtt = new DataTable();
                    SqlDataAdapter daa = new SqlDataAdapter(sqll, strcon);
                    daa.Fill(dtt);
                    foreach (DataRow dr in dtt.Rows)
                    {
                        if (dr.RowState == DataRowState.Deleted) continue;
                        dr.Delete();
                    }
                    new SqlCommandBuilder(daa);
                    daa.Update(dtt);
                    str = "";
                }

                //da.Update(dt_product);
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + " fun_saveData");
                throw new Exception(ex.Message);
            }
        }

        //新增一行
#pragma warning disable IDE1006 // 命名样式
        private void fun_AddNewRow()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataRow r = dt_product.NewRow();
                r["生产计划类型"] = "标准类型";
                r["不显示"] = 0;
                r["操作人员ID"] = CPublic.Var.LocalUserID;
                r["操作人员"] = CPublic.Var.localUserName;
                dt_product.Rows.Add(r);

                //gv_计划.FocusedRowHandle = dt_product.Rows.Count - 1;
                gv_计划.FocusedRowHandle = gv_计划.DataRowCount - 1;

            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + " fun_AddNewRow");
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region 界面操作
        //刷新功能
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_loadProductPlan();
                gc_detailproduct.DataSource = new DataTable();
                foreach (DevExpress.XtraGrid.Columns.GridColumn dc in gv_计划.Columns)
                {
                    if (dc.FieldName == "选择" || dc.FieldName == "输入生产数量" || dc.FieldName == "加急状态")
                    //if ( dc.FieldName == "输入生产数量" || dc.FieldName == "加急状态")
                    {
                        dc.OptionsColumn.AllowEdit = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //新增功能
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                gv_计划.CloseEditor();
                this.BindingContext[dt_product].EndCurrentEdit();
                fun_AddNewRow();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        string str = "";//删除时用到
        //删除功能
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (dt_product.Rows.Count <= 0)
                    throw new Exception("无生产计划可以删除！");
                DataRow r = gv_计划.GetDataRow(gv_计划.FocusedRowHandle);

                if (r.RowState != DataRowState.Added)
                {
                    if (r["生产计划类型"].ToString() == "MRP类型")
                        throw new Exception("MRP类型计划不允许删除！");
                    if (r["生产计划类型"].ToString() == "标准类型" && Convert.ToDecimal(r["已生成数量"]) > 0)
                        throw new Exception("标准类型的已生成数量大于0时，该计划不允许删除");
                    if (MessageBox.Show(string.Format("你确定要删除生产计划单号为\"{0}\"的生产计划吗？", r["生产计划单号"].ToString()), "提示？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        str = str + " 生产计划单号 = '" + r["生产计划单号"].ToString() + "' and";
                        r.Delete();
                    }
                }
                else
                {
                    r.Delete();  //新增行时可以删除的。
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //保存操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                gv_计划.CloseEditor();
                this.BindingContext[dt_product].EndCurrentEdit();

                fun_checkSaveData();
                fun_saveData();
                fun_loadProductPlan();

                DataRow[] dr = dt_product.Select(string.Format("生产计划类型='标准类型'"));
                if (dr.Length > 0)
                {
                    MessageBox.Show("保存成功！");
                }
                //保存完直接刷新一遍 
                fun_loadProductPlan();
                gc_detailproduct.DataSource = new DataTable();
                dt_product.AcceptChanges();
                foreach (DevExpress.XtraGrid.Columns.GridColumn dc in gv_计划.Columns)
                {
                    if (dc.FieldName == "输入生产数量" || dc.FieldName == "加急状态")
                    {
                        dc.OptionsColumn.AllowEdit = true;
                    }
                }
                //gv_计划.FocusedRowHandle = dt_product.Rows.Count - 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //关闭按钮
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }
        #endregion

        #region 转生产制令

        //选择需要转制令的计划
#pragma warning disable IDE1006 // 命名样式
        private void fun_selectJH()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
               

                DataView dv = new DataView(dt_product);
                dv.RowStateFilter = DataViewRowState.Added | DataViewRowState.Deleted;
                if (dv.Count > 0)
                    throw new Exception("计划表存在新增或者删除的行，尚未进行保存，请先保存之后再转生产制令！");
                dt_rescjh = dt_product.Clone();
                foreach (DataRow r in dt_product.Rows)
                {
                    if (r["选择"].Equals(true))
                    {
                        //int[] i = gv_计划.GetSelectedRows();
                        //foreach (int j in i)
                        //{
                        //    DataRow r = gv_计划.GetDataRow(j);
                        dt_rescjh.Rows.Add(r.ItemArray);
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + "　fun_selectJH");
                throw new Exception(ex.Message);
            }
        }


#pragma warning disable IDE1006 // 命名样式
        private void fun_deleteDT(DataTable dt_delete)
#pragma warning restore IDE1006 // 命名样式
        {
            foreach (DataRow r in dt_delete.Rows)
            {
                DataRow[] dr = dt_rescjh.Select(string.Format("生产计划单号='{0}'", r["生产计划单号"].ToString()));
                if (dr.Length <= 0)
                {
                    r.Delete();
                }
                else
                {
                    if (r["物料编码"].ToString() != dr[0]["物料编码"].ToString())
                        throw new Exception(string.Format("生产计划单号\"{0}\",计划与明细存在不一致的物料编码，请检查，重新选择！", r["生产计划单号"].ToString()));
                }
            }
        }


        //转制令检测
#pragma warning disable IDE1006 // 命名样式
        private void fun_checkToZL()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (dt_rescjh.Rows.Count <= 0)
                    throw new Exception("请选择需要转生产制令的生产计划！");
                foreach (DataRow r in dt_rescjh.Rows)
                {
                    SqlDataAdapter da;
                    string sql = string.Format("select 物料类型 from 基础数据物料信息表 where 物料编码='{0}'", r["物料编码"].ToString());
                    DataTable dt_materialsType = new DataTable();
                    da = new SqlDataAdapter(sql, strcon);
                    da.Fill(dt_materialsType);
                    if (dt_materialsType.Rows[0][0].ToString() == "成品" && r["生产计划类型"].ToString()!= "标准类型")
                    {
                        //if (r["生产计划类型"].ToString() == "MRP类型")
                        //{
                        //    //1、检查勾选转制令的计划单号下面有没有明细，如果没有明细，需要先新增。
                        DataRow[] dr = dt_productdetail.Select(string.Format("生产计划单号='{0}'", r["生产计划单号"].ToString()));
                        if (dr.Length <= 0)
                            //MessageBox.Show(string.Format("勾选的生产计划单号\"{0}\"无明细，是否需要新增明细！", r["生产计划单号"].ToString()));
                            throw new Exception(string.Format("勾选的生产计划单号\"{0}\"无明细，请选中该行新增明细！", r["生产计划单号"].ToString()));
                        //MessageBox.Show(string.Format("勾选的生产计划单号\"{0}\"无明细，请选中该行新增明细！", r["生产计划单号"].ToString()));
                        //}

                        //if (r["生产计划类型"].ToString() == "标准类型")
                        //{
                        //    //1、检查勾选转制令的计划单号下面有没有明细，如果没有明细，需要先新增。
                        //    DataRow[] dr = dt_productdetail.Select(string.Format("生产计划单号='{0}'", r["生产计划单号"].ToString()));
                        //    if (dr.Length <= 0)
                        //       MessageBox.Show(string.Format("勾选的生产计划单号\"{0}\"无明细！", r["生产计划单号"].ToString()));
                        ////}
                    }
                    //if (r["生产计划类型"].ToString() == "MRP类型")
                    //{
                    //    //1、检查勾选转制令的计划单号下面有没有明细，如果没有明细，需要先新增。
                    //    DataRow[] dr = dt_productdetail.Select(string.Format("生产计划单号='{0}'", r["生产计划单号"].ToString()));
                    //    if (dr.Length <= 0)
                    //        throw new Exception(string.Format("勾选的生产计划单号\"{0}\"无明细，请选中该行新增明细！", r["生产计划单号"].ToString()));
                    //}
                    if (r["输入生产数量"].ToString() == "")  //2、检查数量，输入的是生产数量是用户输入的，并检查输入的是否是数字。
                        throw new Exception("选择转生产制令的计划单，输入生产数量不能为空，请填写！");
                    try
                    {
                        decimal dd = Convert.ToDecimal(r["输入生产数量"]);
                    }
                    catch
                    {
                        throw new Exception("输入生产数量应该为数字，请检查！");
                    }
                }

                //勾选的转制令的计划，如果明细多了，就删除，如果有该明细，且物料编码不一致。              
                foreach (DataRow r in dt_productdetail.Rows)
                {
                    DataRow[] dr = dt_rescjh.Select(string.Format("生产计划单号='{0}'", r["生产计划单号"].ToString()));
                    if (dr.Length <= 0)
                    {
                        r.AcceptChanges();
                        r.Delete();
                    }
                    else
                    {
                        if (r["物料编码"].ToString().Trim() != dr[0]["物料编码"].ToString().Trim())
                            throw new Exception(string.Format("生产计划单号\"{0}\",计划与明细存在不一致的物料编码，请检查，重新选择！", r["生产计划单号"].ToString()));
                    }
                }
                dt_productdetail.AcceptChanges();
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_checkToZL");
                throw new Exception(ex.Message);
            }
        }

        //转生产制令
#pragma warning disable IDE1006 // 命名样式
        private void fun_ToSHCZL()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                //制令主表
                DataTable dt_zlmain = new DataTable();
                //制令子表
                DataTable dt_zlmx = new DataTable();
                DateTime time = CPublic.Var.getDatetime();
                //生产记录生产制令表
                SqlDataAdapter da;
                string sql = "select * from 生产记录生产制令表 where 1<>1";
                da = new SqlDataAdapter(sql, strcon);
                da.Fill(dt_zlmain);
                //生产记录生产制令子表
                sql = "select * from 生产记录生产制令子表 where 1<>1";
                da = new SqlDataAdapter(sql, strcon);
                da.Fill(dt_zlmx);

                foreach (DataRow r in dt_rescjh.Rows)
                {
                    //转生产制令主表
                    DataRow r_zlmian = dt_zlmain.NewRow();
                    r_zlmian["GUID"] = System.Guid.NewGuid();
                    //标准类型的单号
                    strprozld = string.Format("PM{0}{1:00}{2:00}{3:0000}", time.Year, time.Month, time.Day, CPublic.CNo.fun_得到最大流水号("CG", time.Year, time.Month));
                    r_zlmian["生产制令单号"] = strprozld;
                    r_zlmian["生产计划单号"] = r["生产计划单号"];
                    r_zlmian["生产制令类型"] = r["生产计划类型"];
                    r_zlmian["物料编码"] = r["物料编码"];
                    r_zlmian["物料名称"] = r["物料名称"];
                    r_zlmian["图纸编号"] = r["图纸编号"];
                    r_zlmian["规格型号"] = r["规格型号"];
                    //r_zlmian["原规格型号"] = r["原规格型号"];
                    r_zlmian["原规格型号"] = r["n原ERP规格型号"];

                    r_zlmian["特殊备注"] = r["特殊备注"];
                   
                    //r_zlmian["备注"] = r["特殊备注"];
                   

                    string sql_cj = string.Format("select * from 基础数据物料信息表 where 物料编码='{0}'", r["物料编码"]);
                    using (SqlDataAdapter da1 = new SqlDataAdapter(sql_cj, strcon))
                    {
                        DataTable dt_chejian = new DataTable();
                        da1.Fill(dt_chejian);
                        if (dt_chejian.Rows.Count > 0 && dt_chejian.Rows[0]["车间编号"].ToString() != "")
                        {
                            r_zlmian["生产车间"] = dt_chejian.Rows[0]["车间编号"];
                        }
                    }
                    r_zlmian["客户ID"] = r["客户ID"];
                    r_zlmian["客户名称"] = r["客户名称"];
                    r_zlmian["制令数量"] = r["输入生产数量"];
                    r_zlmian["未排单数量"] = r["输入生产数量"];
                    r_zlmian["计划生产量"] = r["显示计划数量"];
                    r_zlmian["日期"] = time;
                    r_zlmian["操作人员"] = CPublic.Var.localUserName;
                    r_zlmian["操作人员ID"] = CPublic.Var.LocalUserID;
                    r_zlmian["加急状态"] = r["加急状态"];

                    //转生产制令子表
                    DataRow[] t = dt_productdetail.Select(string.Format("生产计划单号='{0}'", r["生产计划单号"].ToString()));
                    //string saledanhao = "";
                    int pos = 1;
                    foreach (DataRow r1 in t)
                    {   //把销售单和销售单明细连成字符串
                        //r_zlmian["销售订单明细号"] = r_zlmian["销售订单明细号"].ToString() + r1["销售订单明细号"] + "|";
                        //if (saledanhao != r1["销售订单号"].ToString())
                        //{
                        //    r_zlmian["销售订单号"] = r_zlmian["销售订单号"].ToString() + r1["销售订单号"] + "|";
                        //}
                        DataRow r_zlmx = dt_zlmx.NewRow();
                        r_zlmx["GUID"] = System.Guid.NewGuid();
                        r_zlmx["生产制令单号"] = strprozld;
                        r_zlmx["POS"] = pos++;
                        r_zlmx["销售订单明细号"] = r1["销售订单明细号"];
                        r_zlmx["销售订单号"] = r1["销售订单号"];
                        r_zlmx["物料编码"] = r1["物料编码"];


                        r_zlmx["销售备注"] = r1["备注"];

                        r_zlmx["物料名称"] = r1["物料名称"];
                        r_zlmx["规格型号"] = r1["规格型号"];
                        r_zlmx["n原ERP规格型号"] = r1["n原ERP规格型号"];
                        r_zlmx["特殊备注"] = r1["特殊备注"];
                        r_zlmx["图纸编号"] = r1["图纸编号"];
                        r_zlmx["客户"] = r1["客户"];
                        r_zlmx["送达日期"] = r1["送达日期"];
                        r_zlmx["计量单位"] = r1["计量单位"];
                        r_zlmx["数量"] = r1["数量"];
                        dt_zlmx.Rows.Add(r_zlmx);
                    }

                    //生产计划表
                    DataRow[] drr = dt_product.Select(string.Format("生产计划单号='{0}'", r_zlmian["生产计划单号"].ToString()));
                    if (drr.Length > 0)
                    {
                        if (r_zlmian["销售订单号"] != DBNull.Value)
                        {
                            drr[0]["销售订单号"] = r_zlmian["销售订单号"];
                        }

                        if (r_zlmian["销售订单明细号"] != DBNull.Value)
                        {
                            drr[0]["销售订单明细号"] = r_zlmian["销售订单明细号"];
                        }
                    }

                    foreach (DataRow r1 in dt_product.Rows)
                    {
                        if (r1["生产计划类型"].ToString() == "MRP类型")
                        {
                            r1.AcceptChanges();
                            //DataRow[] dr_num = dt_DataMRP.Select(string.Format("生产计划单号='{0}'", r1["生产计划单号"].ToString()));
                            //if (dr_num.Length > 0)
                            //{
                            //    r1["已生成数量"] = dr_num[0]["已生成数量"];
                            //    r1["未生成数量"] = dr_num[0]["未生成数量"];
                            //}
                            //else
                            //{
                            //    r1["已生成数量"] = 0;
                            //    r1["未生成数量"] = 0;
                            //}
                        }
                    }
                    dt_zlmain.Rows.Add(r_zlmian);
                }

                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("ZLD");
                SqlCommand cmd_scjh = new SqlCommand("select * from 生产记录生产计划表 where 1<>1", conn, ts);
                SqlCommand cmd_zld = new SqlCommand("select * from 生产记录生产制令表 where 1<>1", conn, ts);
                SqlCommand cmd_zlmx = new SqlCommand("select * from 生产记录生产制令子表 where 1<>1", conn, ts);
                try
                {   //生产计划表
                    da = new SqlDataAdapter(cmd_scjh);
                    new SqlCommandBuilder(da);
                    da.Update(dt_product);
                    //制令主表
                    da = new SqlDataAdapter(cmd_zld);
                    new SqlCommandBuilder(da);
                    da.Update(dt_zlmain);
                    //制令子表
                    da = new SqlDataAdapter(cmd_zlmx);
                    new SqlCommandBuilder(da);
                    da.Update(dt_zlmx);
                    ts.Commit();
                }
                catch
                {
                    ts.Rollback();
                }
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + " fun_ToSHCZL");
                throw new Exception(ex.Message);
            }
        }

        //转生产制令操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            gv_计划.CloseEditor();
            this.BindingContext[dt_product].EndCurrentEdit();

            DataRow dr = gv_计划.GetDataRow(gv_计划.FocusedRowHandle);
            string sql = string.Format("select 物料状态,更改预计完成时间 from 基础数据物料信息表 where 物料编码 = '{0}'", dr["物料编码"].ToString());
            DataTable t = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            da.Fill(t);
            if (t.Rows[0]["物料状态"].ToString() == "更改")
            {
                DateTime time = (DateTime)t.Rows[0]["更改预计完成时间"];
                if (MessageBox.Show(string.Format("当前物料为更改状态，预计完成时间：{0}，是否继续？", time.ToString("yyyy-MM-dd")), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    try
                    {
                        //if (dt_productdetail == null || dt_productdetail.Rows.Count <= 0)
                        //    throw new Exception("没有明细不能进行转生产制令操作，请先新增明细！");
                        if (dt_product == null || dt_product.Rows.Count <= 0)
                            throw new Exception("没有生产计划不能进行转生产制令操作！");
                        gv_计划.CloseEditor();
                        this.BindingContext[dt_product].EndCurrentEdit();
                        fun_selectJH();
                        fun_checkToZL();  //转制令检测
                        fun_ToSHCZL();  //转制令
                        if (MessageBox.Show("转生产制令单成功，已保存！是否跳转生产制令界面？", "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            frm生产制令表 fm = new frm生产制令表(dr, strprozld);
                            CPublic.UIcontrol.AddNewPage(fm, "制令生效");
                        }
                        if (dt_displaymx != null)
                        {
                            dt_displaymx.Clear();
                        }
                        if (dt_productdetail != null)
                        {
                            dt_productdetail.Clear();
                        }
                        fun_loadProductPlan();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                try
                {
                    //if (dt_productdetail == null || dt_productdetail.Rows.Count <= 0)
                    //    throw new Exception("没有明细不能进行转生产制令操作，请先新增明细！");
                 
                    if (dt_product == null || dt_product.Rows.Count <= 0)
                        throw new Exception("没有生产计划不能进行转生产制令操作！");
                    gv_计划.CloseEditor();
                    this.BindingContext[dt_product].EndCurrentEdit();
                    fun_selectJH();
                    fun_checkToZL();  //转制令检测
                    fun_ToSHCZL();  //转制令
                    if (MessageBox.Show("转生产制令单成功，已保存！是否跳转生产制令界面？", "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        frm生产制令表 fm = new frm生产制令表(dr, strprozld);
                        //CPublic.UIcontrol.AddNewPage(fm, "生产制令");
                        CPublic.UIcontrol.Showpage(fm, "生产制令");

                    }
                    if (dt_displaymx != null)
                    {
                        dt_displaymx.Clear();
                    }
                    if (dt_productdetail != null)
                    {
                        dt_productdetail.Clear();
                    }
                    fun_loadProductPlan();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        #endregion

        #region  关联销售单
        //明细回传值处理
#pragma warning disable IDE1006 // 命名样式
        private void fun_detailDeal(DataTable dt, string danhao)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                //给dt加上生产计划单号，关联的销售单号对应相应的生产计划单号
                dt.Columns.Add("生产计划单号");
                if (dt_productdetail == null)  //明细的dt
                {
                    dt_productdetail = dt.Clone();
                }
                if (dt_displaymx == null)
                {
                    dt_displaymx = dt.Clone();
                }
                //向总的明细dt中加入选择项
                foreach (DataRow r in dt.Rows)
                {
                    r["生产计划单号"] = danhao;
                    dt_productdetail.Rows.Add(r.ItemArray);
                }
                //在总的明细dt中找到当前的生产计划单号的明细，用来显示
                DataRow[] dr = dt_productdetail.Select(string.Format("生产计划单号='{0}'", danhao));
                dt_displaymx.Clear();
                foreach (DataRow r5 in dr)
                {
                    dt_displaymx.Rows.Add(r5.ItemArray);
                }
                gc_detailproduct.DataSource = dt_displaymx;
                // gc_detailproduct.DataSource = dt;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_detailDeal");
                throw new Exception(ex.Message);
            }
        }

        //新增明细的操作
#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //try
            //{
            //    if (dt_product.Rows.Count <= 0)
            //        throw new Exception("无生产计划，不可新增明细！");
            //    //DataRow r = (this.BindingContext[dt_product].Current as DataRowView).Row;
            //    DataRow r = gv_计划.GetDataRow(gv_计划.FocusedRowHandle);
            //    if (r.RowState == DataRowState.Added)
            //        throw new Exception("你选中的生产计划是新增的，还没有保存，请先保存生产计划！");
            //    //选择关联的销售单，只能选择跟生产计划相一致的物料编码
            //    //fm关联销售明细选择 fm = new fm关联销售明细选择(dt_productdetail,r["物料编码"].ToString());
            //    fm关联销售明细选择 fm = new fm关联销售明细选择(dt_displaymx, r["物料编码"].ToString());
            //    fm.ShowDialog();
            //    if (fm.dt_保存打钩选择 != null)
            //    {
            //        fun_detailDeal(fm.dt_保存打钩选择, r["生产计划单号"].ToString());
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        //删除明细的操作
#pragma warning disable IDE1006 // 命名样式
        private void simpleButton2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {   //删除显示在界面上的明细
                if (dt_displaymx == null || dt_displaymx.Rows.Count <= 0)
                    throw new Exception("无明细可以删除,请先新增明细！");
                DataRow r = gv_关联订单.GetDataRow(gv_关联订单.FocusedRowHandle);


                if (MessageBox.Show(string.Format("你确定要删除明细号为\"{0}\"的明细吗？", r["销售订单明细号"].ToString()), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    //删除总的明细dt中的该明细
                    DataRow[] dr = dt_productdetail.Select(string.Format("销售订单明细号='{0}'", r["销售订单明细号"].ToString()));
                    if (dr.Length > 0)
                    {
                        dr[0].Delete();
                    }
                    r.Delete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region  其他界面处理
        //列不可编辑的设置。
#pragma warning disable IDE1006 // 命名样式
        private void gv_planproduct_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (e.FocusedRowHandle >= 0)
                {
                    //DataRow r = (this.BindingContext[dt_product].Current as DataRowView).Row;
                    DataRow r = gv_计划.GetDataRow(gv_计划.FocusedRowHandle);
                    //显示不同的明细
                    if (dt_productdetail != null && dt_productdetail.Rows.Count > 0)
                    {
                        dt_displaymx = dt_productdetail.Clone();
                        DataRow[] dr = dt_productdetail.Select(string.Format("生产计划单号='{0}'", r["生产计划单号"].ToString()));
                        foreach (DataRow r5 in dr)
                        {
                            dt_displaymx.Rows.Add(r5.ItemArray);

                        }
                        gc_detailproduct.DataSource = dt_displaymx;
                    }


                    //是否可以编辑的设定
                    if (r.RowState != DataRowState.Added)
                    {
                        if (r["生产计划类型"].ToString() == "MRP类型")
                        {
                            foreach (DevExpress.XtraGrid.Columns.GridColumn dc in gv_计划.Columns)
                            {
                                if (dc.FieldName != "选择" && dc.FieldName != "输入生产数量" && dc.FieldName != "加急状态")
                                //if ( dc.FieldName != "输入生产数量" && dc.FieldName != "加急状态")
                                {
                                    dc.OptionsColumn.AllowEdit = false;
                                }
                                else
                                {
                                    dc.OptionsColumn.AllowEdit = true;

                                }
                            }
                        }
                        else if (r["生产计划类型"].ToString() == "标准类型")
                        {
                            foreach (DevExpress.XtraGrid.Columns.GridColumn dc in gv_计划.Columns)
                            {
                                if (dc.FieldName != "选择" && dc.FieldName != "输入生产数量" && dc.FieldName != "加急状态")
                                //if ( dc.FieldName != "输入生产数量" && dc.FieldName != "加急状态")
                                {
                                    dc.OptionsColumn.AllowEdit = false;
                                }
                            }
                        }
                        //else
                        //{
                        //    foreach (DevExpress.XtraGrid.Columns.GridColumn dc in gv_planproduct.Columns)
                        //    {
                        //        if (dc.FieldName!="生产计划类型" && dc.FieldName != "生产计划单号" && dc.FieldName != "物料名称" && dc.FieldName != "规格型号" && dc.FieldName != "图纸编号" && dc.FieldName != "生产线" && dc.FieldName != "客户名称")
                        //        {
                        //            dc.OptionsColumn.AllowEdit = true;
                        //        }
                        //    }
                        //}
                    }
                    else
                    {
                        foreach (DevExpress.XtraGrid.Columns.GridColumn dc in gv_计划.Columns)
                        {
                            //if (dc.FieldName=="加急状态" && dc.FieldName!="生产计划类型" && dc.FieldName != "生产计划单号" && dc.FieldName != "物料名称" && dc.FieldName != "规格型号" && dc.FieldName != "图纸编号" && dc.FieldName != "生产车间" && dc.FieldName != "客户名称")
                            //{
                            //    dc.OptionsColumn.AllowEdit = true;
                            //}
                            if (dc.FieldName == "选择" || dc.FieldName == "显示计划数量" || dc.FieldName == "加急状态" || dc.FieldName == "原ERP物料编号")
                            //if ( dc.FieldName == "显示计划数量" || dc.FieldName == "加急状态" || dc.FieldName == "物料编码")
                            {
                                dc.OptionsColumn.AllowEdit = true;
                            }
                            if (dc.FieldName == "输入生产数量")
                            {
                                dc.OptionsColumn.AllowEdit = false;
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

        //已完成数量
#pragma warning disable IDE1006 // 命名样式
        private void repositoryItemTextEdit1_ParseEditValue(object sender, DevExpress.XtraEditors.Controls.ConvertEditValueEventArgs e)
#pragma warning restore IDE1006 // 命名样式
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

        //未完成数量
#pragma warning disable IDE1006 // 命名样式
        private void repositoryItemTextEdit2_ParseEditValue(object sender, DevExpress.XtraEditors.Controls.ConvertEditValueEventArgs e)
#pragma warning restore IDE1006 // 命名样式
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
        // 查看详细数量
#pragma warning disable IDE1006 // 命名样式
        private void gv_planproduct_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc_planproduct, new Point(e.X, e.Y));
                gv_计划.CloseEditor();
                this.BindingContext[dt_product].EndCurrentEdit();

            }
        }

        private void 查看物料BOM信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow r = gv_计划.GetDataRow(gv_计划.FocusedRowHandle);
            decimal dec;
            if (r["输入生产数量"] != DBNull.Value && r["输入生产数量"].ToString() != "")
            {
                dec = Convert.ToDecimal(r["输入生产数量"].ToString());

            }
            else
            {
                dec = 1;
            }
        
            ERPproduct.UI物料BOM详细数量 frm = new UI物料BOM详细数量(r["物料编码"].ToString().Trim(),dec);
            CPublic.UIcontrol.AddNewPage(frm, "详细数量");
        }

        private void 查看过往制令ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow r = gv_计划.GetDataRow(gv_计划.FocusedRowHandle);

            UI查看制令列表 UI = new UI查看制令列表(r["物料编码"].ToString().Trim());
            CPublic.UIcontrol.AddNewPage(UI, "过往制令");


        }

        private void 过往通知出库记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow r = gv_计划.GetDataRow(gv_计划.FocusedRowHandle);

            UI查看出库通知明细 ui = new UI查看出库通知明细(r["物料编码"].ToString().Trim());
            CPublic.UIcontrol.AddNewPage(ui, "过往通知出库记录");

        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_计划_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_计划_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //if (checkBox1.CheckState == CheckState.Checked)
            //{
            //    int[] dr = gv_计划.GetSelectedRows();
            //    foreach (int i in dr)
            //    {
            //        DataRow r = gv_计划.GetDataRow(i);
            //        r["选择"] = false;

            //    }
            //}
            //else
            //{
            //    int[] dr = gv_计划.GetSelectedRows();
            //    foreach (int i in dr)
            //    {
            //        DataRow r = gv_计划.GetDataRow(i);
            //        r["选择"] = true;

            //    }
            //}
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_关联订单_KeyDown(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv_关联订单.GetFocusedRowCellValue(gv_关联订单.FocusedColumn));
                e.Handled = true;
            }
        }







    }
}
