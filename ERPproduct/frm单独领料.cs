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
    public partial class frm单独领料 : UserControl
    {
        #region  变量
        string strcon = CPublic.Var.strConn;
        DataTable dtM;
        DataTable dt_物料信息;
        DataTable dt_工单;//存放 绑定 工单 
        DataTable dt_仓库;//存放 仓库列表 
        DataTable dt_默认人员信息 = ERPorg.Corg.fun_hr("仓库", CPublic.Var.LocalUserID);
        DataRow drm;


        #endregion

        #region 加载

        public frm单独领料()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm单独领料_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                devGridControlCustom1.UserName = CPublic.Var.LocalUserID;
                devGridControlCustom1.strConn = CPublic.Var.strConn;
                fun_load();
                if (dt_默认人员信息.Rows.Count > 0)
                {
                    searchLookUpEdit1.EditValue = dt_默认人员信息.Rows[0]["仓库号"];
                    textBox3.Text = dt_默认人员信息.Rows[0]["仓库名称"].ToString();

                    textBox1.Text = dt_默认人员信息.Rows[0]["用户ID"].ToString();
                    textBox6.Text = dt_默认人员信息.Rows[0]["工号"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion



        #region 函数
        //加载所有物料 下拉框
#pragma warning disable IDE1006 // 命名样式
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {
            dtM = new DataTable();
            dtM.Columns.Add("物料编码");
            dtM.Columns.Add("物料名称");
            dtM.Columns.Add("规格型号");
            //  dtM.Columns.Add("规格");
            dtM.Columns.Add("图纸编号");
            dtM.Columns.Add("领料数量");
            // dtM.Columns.Add("n原ERP规格型号");
            dtM.Columns.Add("库存总数");
            dtM.Columns.Add("货架描述");
            dtM.Columns.Add("仓库名称");
            dtM.Columns.Add("仓库号");


            gridControl1.DataSource = dtM;
            //string sql = "select 物料编码,物料名称,规格型号,图纸编号,规格 from 基础数据物料信息表 where 物料类型 <> '成品'";
            string sql = @"select base.物料编码,isnull(库存总数,0)库存总数,base.物料名称,base.规格型号,
                          base.图纸编号,base.货架描述,a.仓库名称,a.仓库号
                          from 基础数据物料信息表 base  left join 仓库物料数量表 a on  base.物料编码=a.物料编码  where base.停用=0";

            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                dt_物料信息 = new DataTable();
                da.Fill(dt_物料信息);
                repositoryItemSearchLookUpEdit1.DataSource = dt_物料信息;
                repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
                repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            }
            // cb_TypeOfM  里面需要赋值 领料类型



            textBox1.Text = CPublic.Var.localUserName;
            textBox6.Text = CPublic.Var.LocalUserID;


            //这里提交申请需要 人事员工组织关系表 的上下级关系 需要知道当前人员 是哪个组织关系的 
            DataTable dt = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);
            if (dt.Rows.Count > 0 && dt.Rows[0]["生产车间"].ToString() != "")
            {
                textBox2.Text = dt.Rows[0]["生产车间"].ToString();
                string sql_2 = string.Format("select * from  人事基础部门表 where 部门编号='{0}'", dt.Rows[0]["生产车间"].ToString());
                DataRow dr = CZMaster.MasterSQL.Get_DataRow(sql_2, strcon);
                if (dr == null)
                {

                }
                else
                {
                    textBox11.Text = dr["部门名称"].ToString();
                }
            }
            //加载仓库信息
            string sql_仓库 = "select 属性字段1 as 仓库编号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 = '仓库类别'";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_仓库, strcon))
            {
                dt_仓库 = new DataTable();
                da.Fill(dt_仓库);
                searchLookUpEdit1.Properties.DataSource = dt_仓库;
                searchLookUpEdit1.Properties.DisplayMember = "仓库编号";
                searchLookUpEdit1.Properties.ValueMember = "仓库编号";
            }
           

        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {

        
            foreach (DataRow dr in dtM.Rows)
            {
                if (dr["领料数量"].ToString() == "")
                {
                    throw new Exception("请输入领料数量");
                }
                try
                {
                    decimal a = Convert.ToDecimal(dr["领料数量"]);
                    if (a <= 0)
                    {
                        throw new Exception("领料数量不能小于等于0，,请重新输入");

                    }

                }
                catch
                {
                    throw new Exception("请正确输入领料数量格式");

                }
            
                if (Convert.ToDecimal(dr["领料数量"]) > Convert.ToDecimal(dr["库存总数"]))
                {
                    throw new Exception("库存总数不足！");
                }
            }

        }
        /// <summary>
        /// 2018-8-10 其他领料需要提交申请后 经审批  保存至《单据审核申请表》中 OTA
        /// 人事基础员工表 中添加 组织关系 指示 当前人员 在哪个组织关系表中
        /// 
        /// </summary>
        //private void fun_newsave()
        //{


        //}
#pragma warning disable IDE1006 // 命名样式
        private DataSet fun_save()
#pragma warning restore IDE1006 // 命名样式
        {
            DateTime t = CPublic.Var.getDatetime();
            string str_id = CPublic.Var.LocalUserID;
            string str_name = CPublic.Var.localUserName;
            DataSet ds = new DataSet();
            string str_待领料单号 = string.Format("DL{0}{1:00}{2:00}{3:0000}",
                                                 t.Year, t.Month, t.Day,
                                                CPublic.CNo.fun_得到最大流水号("DL", t.Year, t.Month));
            textBox5.Text = str_待领料单号;
            // 主表
            string sql_主表 = "select * from 生产记录生产工单待领料主表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_主表, strcon))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                DataRow dr = dt.NewRow();
                dataBindHelper1.DataToDR(dr);
                dr["待领料单号"] = str_待领料单号;
                dr["领料人"] = textBox1.Text;
                dr["领料人ID"] = textBox6.Text; 
                dr["领料类型"] = "单独领料";
                dr["创建日期"] = t;
                dr["制单人员"] = str_name;
                dr["制单人员ID"] = str_id;
                dt.Rows.Add(dr);
                ds.Tables.Add(dt);
            }
            //明细表
            string sql_明细 = "select * from 生产记录生产工单待领料明细表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_明细, strcon))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                int pos = 1;
                foreach (DataRow r in dtM.Rows)
                {
                    DataRow dr = dt.NewRow();
                    dt.Rows.Add(dr);
                    dr["待领料单号"] = str_待领料单号;
                    dr["待领料单明细号"] = str_待领料单号 + "-" + pos++.ToString("00");
                    dr["物料编码"] = r["物料编码"];
                    dr["物料名称"] = r["物料名称"];
                    dr["规格型号"] = r["规格型号"];
                    //dr["图纸编号"] = r["图纸编号"];
                    dr["待领料总量"] = r["领料数量"];
                    dr["未领数量"] = r["领料数量"];

                    dr["仓库号"] = r["仓库号"];
                    dr["仓库名称"] = r["仓库名称"];
                    dr["制单人员"] = str_name;
                    dr["制单人员ID"] = str_id;

                    dr["创建日期"] = t;


                    dataBindHelper1.DataToDR(dr);
                    pos++;

                }
                ds.Tables.Add(dt);
  
            }
            return ds;
        }

        //private DataSet  fun_虚拟库存()
        //{
        //    DataSet ds = new DataSet();
        //    DataTable dt_主表 = new DataTable();
        //    DataTable dt_明细 = new DataTable();
        //    foreach (DataRow dr in dtM.Rows)
        //    {
        //        //虚拟库存主表
        //        string sql = string.Format(
        //            "select * from 生产记录车间虚拟库存表 where 生产车间='{0}'and 物料编码 = '{1}'",
        //            textBox2.Text, dr["物料编码"].ToString().Trim());

        //        using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
        //        {
        //            da.Fill(dt_主表);
        //            if (dt_主表.Rows.Count > 0) //找到了 加减数量
        //            {
        //                dt_主表.Rows[0]["车间数量"] = Convert.ToDecimal(dt_主表.Rows[0]["车间数量"]) + Convert.ToDecimal(dr["领料数量"]);
        //                dt_主表.Rows[0]["未用数量"] = Convert.ToDecimal(dt_主表.Rows[0]["未用数量"]) + Convert.ToDecimal(dr["领料数量"]);
        //                dt_主表.Rows[0]["修改日期"] = System.DateTime.Now;
        //            }
        //            else    //数据库中没有数据记录 添加一条
        //            {
        //                DataRow r = dt_主表.NewRow();
        //                r["GUID"] = System.Guid.NewGuid();
        //                r["物料编码"] = dr["物料编码"];
        //                r["物料名称"] = dr["物料名称"];
        //                r["生产车间"] = textBox2.Text;
        //                //r["生产工单号"] = searchLookUpEdit2.EditValue;
        //                r["车间数量"] = Convert.ToDecimal(dr["领料数量"]);
        //                r["未用数量"] = Convert.ToDecimal(dr["领料数量"]);
        //                r["修改日期"] = System.DateTime.Now;
        //                dt_主表.Rows.Add(r);
        //            }
        //            //new SqlCommandBuilder(da);
        //            //da.Update(dt_主表);
        //        }
        //        string sql_明细 = string.Format(
        //            "select * from 生产记录车间虚拟库存明细表 where 生产车间='{0}' and  生产工单号='{1}' and 物料编码='{2}'",
        //            textBox2.Text, searchLookUpEdit2.EditValue, dr["物料编码"]);

        //        using (SqlDataAdapter da = new SqlDataAdapter(sql_明细, strcon))
        //        {
        //            da.Fill(dt_明细);
        //            if (dt_明细.Rows.Count > 0)
        //            {
        //                dt_明细.Rows[0]["领料数量"] = Convert.ToDecimal(dt_明细.Rows[0]["领料数量"]) + Convert.ToDecimal(dr["领料数量"]);
        //                dt_明细.Rows[0]["未用数量"] = Convert.ToDecimal(dt_明细.Rows[0]["未用数量"]) + Convert.ToDecimal(dr["领料数量"]);
        //                dt_明细.Rows[0]["修改日期"] = System.DateTime.Now;
        //            }
        //            else
        //            {
        //                DataRow r = dt_明细.NewRow();

        //                r["物料编码"] = dr["物料编码"];
        //                r["物料名称"] = dr["物料名称"];
        //                r["规格型号"] = dr["规格型号"];
        //                r["图纸编号"] = dr["图纸编号"];
        //                r["领料数量"] = dr["领料数量"];
        //                r["未用数量"] = dr["领料数量"];

        //                //r["生产工单号"] = searchLookUpEdit2.EditValue;
        //                r["生产车间"] = textBox2.Text;
        //                r["领料人"] = CPublic.Var.localUserName;
        //                r["领料人ID"] = CPublic.Var.LocalUserID;
        //                r["创建日期"] = System.DateTime.Now;
        //                r["修改日期"] = System.DateTime.Now;
        //                dt_明细.Rows.Add(r);
        //            }
        //            //new SqlCommandBuilder(da);
        //            //da.Update(dt_明细);
        //        }
        //    }
        //    ds.Tables.Add(dt_主表);
        //    ds.Tables.Add(dt_明细);
        //    return ds;

        //}


        //private DataTable  fun_出入库明细()
        //{

        //    string sql = "select * from 仓库出入库明细表 where 1<>1";
        //    using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
        //    {
        //        DataTable dt = new DataTable();
        //        da.Fill(dt);
        //        int POS = 0;
        //        foreach (DataRow dr in dtM.Rows)
        //        {
        //            DataRow r = dt.NewRow();
        //            r["GUID"] = System.Guid.NewGuid();
        //            r["明细类型"] = "单独领料";
        //            r["单号"] = textBox5.Text;
        //            r["出库入库"] = "出库";
        //            r["物料编码"] = dr["物料编码"];
        //            r["物料名称"] = dr["物料名称"];
        //            //r["BOM版本"] = dr["BOM版本"];
        //            r["明细号"] =textBox5.Text + POS.ToString("00"); ;
        //            POS++;

        //            //r["数量"] =  

        //            //r["单位"]=

        //            //r["标准数量"] =
        //            r["实效数量"] = -(Convert.ToDecimal(dr["领料数量"]));
        //            r["实效时间"] = System.DateTime.Now;
        //            r["出入库时间"] = System.DateTime.Now;

        //            string sql_pd = "select * from 仓库物料盘点表 where 有效=1";
        //            using (SqlDataAdapter da1 = new SqlDataAdapter(sql_pd, strcon))
        //            {

        //                DataTable dt_批次号 = new DataTable();
        //                da1.Fill(dt_批次号);
        //                if (dt_批次号.Rows.Count > 0)
        //                {
        //                    r["盘点有效批次号"] = dt_批次号.Rows[0]["盘点批次号"];
        //                }
        //                else
        //                {
        //                    r["盘点有效批次号"] = "初始化";
        //                }
        //            }
        //            dt.Rows.Add(r);

        //            new SqlCommandBuilder(da);
        //            da.Update(dt);
        //        }
        //        return dt;
        //    }
        //}
        #endregion



        //增加物料
#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            dtM.NewRow();
            dtM.Rows.Add();
        }
        //删除
#pragma warning disable IDE1006 // 命名样式
        private void simpleButton2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            r.Delete();
        }
        //生效
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            try
            {
                if (dtM.Rows.Count > 0)
                {
                    gridView1.CloseEditor();
                    this.BindingContext[dtM].EndCurrentEdit();

                    fun_check();
                    DataSet ds_1 = fun_save();

                    //DataSet ds_2= fun_虚拟库存();
                    //DataTable  dt= fun_出入库明细();
                    string sql_待领料主表 = "select * from 生产记录生产工单待领料主表 where 1<>1";
                    string sql_待领料明细表 = "select * from 生产记录生产工单待领料明细表 where 1<>1";

                    //string sql_虚拟主表 = "select * from 生产记录车间虚拟库存表 where 1<>1";
                    //string sql_虚拟明细表 = "select * from 生产记录车间虚拟库存明细表 where 1<>1";
                    // string sql_出入库 = "select * from 仓库出入库明细表 where 1<>1";
                    SqlConnection conn = new SqlConnection(strcon);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("单独领料");
                    try
                    {
                        SqlCommand cmm_1 = new SqlCommand(sql_待领料主表, conn, ts);
                        SqlCommand cmm_2 = new SqlCommand(sql_待领料明细表, conn, ts);
                        //SqlCommand cmm_3 = new SqlCommand(sql_虚拟主表, conn, ts);
                        //SqlCommand cmm_4 = new SqlCommand(sql_虚拟明细表, conn, ts);
                        //SqlCommand cmm_5 = new SqlCommand(sql_出入库, conn, ts);

                        SqlDataAdapter da_待领料主表 = new SqlDataAdapter(cmm_1);
                        SqlDataAdapter da_待领料明细表 = new SqlDataAdapter(cmm_2);
                        //SqlDataAdapter da_虚拟主表 = new SqlDataAdapter(cmm_3);
                        //SqlDataAdapter da_虚拟明细表 = new SqlDataAdapter(cmm_4);
                        //SqlDataAdapter da_出入库 = new SqlDataAdapter(cmm_5);
                        new SqlCommandBuilder(da_待领料主表);
                        new SqlCommandBuilder(da_待领料明细表);
                        //new SqlCommandBuilder(da_虚拟主表);
                        //new SqlCommandBuilder(da_虚拟明细表);
                        //new SqlCommandBuilder(da_出入库);

                        da_待领料主表.Update(ds_1.Tables[0]);
                        da_待领料明细表.Update(ds_1.Tables[1]);
                        //da_虚拟主表.Update(ds_2.Tables[0]);
                        //da_虚拟明细表.Update(ds_2.Tables[1]);
                        //da_出入库.Update(dt);

                        ts.Commit();


                    }
                    catch (Exception ex)
                    {
                        ts.Rollback();
                        throw new Exception("单独领料失败");
                    }

                    foreach (DataRow dr in dtM.Rows)
                    {
                        StockCore.StockCorer.fun_物料数量_实际数量(dr["物料编码"].ToString().Trim(),dr["仓库号"].ToString(), true);
                    }
                    MessageBox.Show("生效成功");
                    barLargeButtonItem1_ItemClick(null, null);

                }
                else
                {
                    MessageBox.Show("请添加物料");
                }

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
                if (dt_默认人员信息.Rows.Count > 0)
                {
                    searchLookUpEdit1.EditValue = dt_默认人员信息.Rows[0]["仓库号"];
                    textBox3.Text = dt_默认人员信息.Rows[0]["仓库名称"].ToString();

                    textBox1.Text = dt_默认人员信息.Rows[0]["用户ID"].ToString();
                    textBox6.Text = dt_默认人员信息.Rows[0]["工号"].ToString();
                }

                textBox2.Text = "";

                textBox3.Text = "";
                textBox5.Text = "";
                textBox4.Text = "";

                textBox7.Text = "";
                //searchLookUpEdit2.EditValue = null;
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }

        //private void repositoryItemSearchLookUpEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        //{
            //if (e.NewValue == null)
            //{
            //    DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            //    r = dtM.NewRow();
            //}

            //else if (e.NewValue.ToString() != "")
            //{
            //    DataRow[] dr = dt_物料信息.Select(string.Format("物料编码='{0}'  ", e.NewValue));
            //    if (dr.Length > 0)
            //    {
            //        DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            //        r["物料名称"] = dr[0]["物料名称"];
            //        r["规格型号"] = dr[0]["规格型号"];
            //        r["n原ERP规格型号"] = dr[0]["n原ERP规格型号"];
            //        r["图纸编号"] = dr[0]["图纸编号"];
            //        r["规格"] = dr[0]["规格"];
            //        r["货架描述"] = dr[0]["货架描述"];
            //        r["仓库名称"] = dr[0]["仓库名称"];
            //        r["库存总数"] = dr[0]["库存总数"];
            //    }

            //}
            //else
            //{
            //    MessageBox.Show("数据有误");
            //}
        //}
        // 领料仓库 变更
        //private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        //{
        //    DataRow[] dr = dt_仓库.Select(string.Format("仓库编号 ='{0}'", searchLookUpEdit1.EditValue));
        //    if (dr.Length > 0)
        //    {
        //        textBox3.Text = dr[0]["仓库名称"].ToString();
        //    }
        //}

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void repositoryItemSearchLookUpEdit1View_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
 
            DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            r["物料名称"] = dr["物料名称"];
            r["规格型号"] = dr["规格型号"];
            r["图纸编号"] = dr["图纸编号"];
            r["货架描述"] = dr["货架描述"];
            r["仓库号"] = dr["仓库号"];
            r["仓库名称"] = dr["仓库名称"];
            r["库存总数"] = dr["库存总数"];
        }
        //防止用户点击行前面的空白 选中  ,不会触发 RowCellClick 事件
#pragma warning disable IDE1006 // 命名样式
        private void repositoryItemSearchLookUpEdit1View_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);

            DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            r["物料名称"] = dr["物料名称"];
            r["规格型号"] = dr["规格型号"];

            r["图纸编号"] = dr["图纸编号"];

            r["货架描述"] = dr["货架描述"];
            r["仓库名称"] = dr["仓库名称"];
            r["库存总数"] = dr["库存总数"];
            r["仓库号"] = dr["仓库号"];


        }



    }

}
