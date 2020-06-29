using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERPproduct
{
    public partial class frm改制工单 : Form
    {
        string strconn = CPublic.Var.strConn;
        DataTable dt_领料单_主 = null; 
        DataTable dt_领料单_子 = null;
        DataTable dt_工单 = null;
        DataRow dr_制令 = null;
        DataTable dt_物料 = null;
        DataTable dt_成品 = null;
        public Boolean a = false ;

        public frm改制工单(DataRow dr)
        {
            InitializeComponent();
            dr_制令 = dr;
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm改制工单_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                a = false;
                dataBindHelper1.DataFormDR(dr_制令);
                string sql2 = string.Format(@"select 目标物料编码,[目标物料名称],基础数据物料信息表.原ERP物料编号 as 物料编号,基础数据物料信息表.规格型号,库存总数,有效总数,在制量,受订量 from 改制对应关系表 
                left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 改制对应关系表.目标物料编码 
                left join 仓库物料数量表 on 改制对应关系表.目标物料编码 = 仓库物料数量表.物料编码
                where 可改制物料编码 ='{0}'", dr_制令["物料编码"]);
                SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
                dt_成品 = new DataTable();
                da2.Fill(dt_成品);
                //载入工单和领料单结构
                fun_载入工单();
                fun_载入领料();
                fun_载入物料();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region 方法
#pragma warning disable IDE1006 // 命名样式
        private void fun_载入物料()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = @"select 基础数据物料信息表.物料编码,基础数据物料信息表.原ERP物料编号 as 物料编号,基础数据物料信息表.物料名称,基础数据物料信息表.规格型号,
            库存总数,有效总数,在途量,未领量 from 基础数据物料信息表 
            left join 仓库物料数量表 on 基础数据物料信息表.物料编码 = 仓库物料数量表.物料编码 
            where 物料类型 = '原材料' or 物料类型 = '半成品' or 物料类型 = '可售原材料'";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            dt_物料 = new DataTable();
            da.Fill(dt_物料);

            //repositoryItemSearchLookUpEdit1.DataSource = dt_物料;
            //repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
            //repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_生成记录()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                #region 转工单
                Decimal dec_计划生产量 = Convert.ToDecimal(dr_制令["计划生产量"]);
                DataRow dr = dt_工单.NewRow();
                dt_工单.Rows.Add(dr);
                DateTime t = CPublic.Var.getDatetime();

                string ss = t.Year.ToString().Substring(2, 2);
                 string strMoNo = string.Format("MO{0}{1:D2}{2:00}{3:0000}", ss, t.Month,t.Day, CPublic.CNo.fun_得到最大流水号("MO",t.Year,t.Month, t.Day));
                
                dr["生产工单号"] = strMoNo;
                dr["生产工单类型"] = "改制工单";
                dr["加急状态"] = dr_制令["加急状态"];
                dr["GUID"] = System.Guid.NewGuid();
                dr["生产制令单号"] = dr_制令["生产制令单号"];
                dr["物料编码"] = dr_制令["物料编码"];
                dr["物料名称"] = dr_制令["物料名称"];
                dr["规格型号"] = dr_制令["规格型号"];
                dr["原规格型号"] = dr_制令["原规格型号"];
                dr["特殊备注"] = dr_制令["特殊备注"];
                dr["预计完工日期"] = dr_制令["预完工日期"];
                dr["生产数量"] = dr_制令["制令数量"];
                //if (dec_计划生产量 <= 0)
                //{
                //    dr["计划生产量"] = -Convert.ToDecimal(dr["生产数量"]);
                //    dec_计划生产量 = dec_计划生产量 - Convert.ToDecimal(dr["生产数量"]);
                //}
                //else
                //{
                //    dr["计划生产量"] = dec_计划生产量 - Convert.ToDecimal(dr["生产数量"]);
                //    dec_计划生产量 = dec_计划生产量 - Convert.ToDecimal(dr["生产数量"]);
                //}
                dr["未检验数量"] = dr_制令["制令数量"];
                dr["图纸编号"] = dr_制令["图纸编号"];
                dr["生产车间"] = dr_制令["生产车间"]; //存储的是车间编号
                //dr["车间名称"] = dr_制令["部门名称"]; 
                dr["制单人员ID"] = CPublic.Var.LocalUserID;
                dr["制单人员"] = CPublic.Var.localUserName;
                dr["制单日期"] =t;
                //dr["生效"] = 1;
                //dr["生效人"] = CPublic.Var.localUserName;
                //dr["生效人ID"] = CPublic.Var.LocalUserID;
                //dr["生效日期"] = System.DateTime.Now;
                #endregion

                #region 领料单主
                string str_待领料单号 = string.Format("DL{0}{1:00}{2:0000}",
                    t.Year,t.Month, CPublic.CNo.fun_得到最大流水号("DL", t.Year, t.Month));
                DataRow r = dt_领料单_主.NewRow();
                r["待领料单号"] = str_待领料单号;
                r["领料类型"] = "工单领料";
                r["生产工单号"] = dr["生产工单号"];
                r["生产制令单号"] = dr["生产制令单号"];
                r["生产工单类型"] = dr["生产工单类型"];
                r["生产车间"] = dr["生产车间"];
                r["物料编码"] = dr["物料编码"];
                r["产品名称"] = dr["物料名称"];
                string sql_1 = string.Format("select * from 基础数据物料信息表 where 物料编码='{0}'", dr["物料编码"].ToString());
                using (SqlDataAdapter da_1 = new SqlDataAdapter(sql_1, strconn))
                {
                    DataTable dt_1 = new DataTable();
                    da_1.Fill(dt_1);
                    if (dt_1.Rows.Count > 0)
                    {
                        r["仓库号"] = dt_1.Rows[0]["仓库号"];
                        r["仓库名称"] = dt_1.Rows[0]["仓库名称"];
                    }
                }
                //r["领料人ID"] = searchLookUpEdit2.EditValue;
                //r["领料人"] = textBox16.Text;
                r["规格型号"] = dr["规格型号"];
                r["原规格型号"] = dr["原规格型号"];
                r["图纸编号"] = dr["图纸编号"];
                r["生产数量"] = Convert.ToDecimal(dr["生产数量"]);
                r["创建日期"] = t;
                r["加急状态"] = dr["加急状态"];
                r["制单人员"] = CPublic.Var.localUserName;
                r["制单人员ID"] = CPublic.Var.LocalUserID;
                //r["工单负责人"] = textBox15.Text;
                //r["工单负责人ID"] = searchLookUpEdit1.EditValue;
                dt_领料单_主.Rows.Add(r);
                #endregion

                #region 领料单子
                int pos = 1;
                foreach (DataRow rr in dt_领料单_子.Rows)
                {
                    if (rr.RowState == DataRowState.Deleted) continue;
                    rr["待领料单号"] = str_待领料单号;
                    rr["待领料单明细号"] = str_待领料单号 + pos.ToString("00");
                    rr["生产工单号"] = dr["生产工单号"];
                    rr["生产制令单号"] = dr["生产制令单号"];
                    rr["生产车间"] = dr["生产车间"]; //车间编号
                    string sql_11 = string.Format("select * from 基础数据物料信息表 where 物料编码='{0}'", dr["物料编码"].ToString());
                    using (SqlDataAdapter da_1 = new SqlDataAdapter(sql_11, strconn))
                    {
                        DataTable dt_临时 = new DataTable();
                        da_1.Fill(dt_临时);
                        if (dt_临时.Rows.Count > 0)
                        {
                            rr["仓库号"] = dt_临时.Rows[0]["仓库号"];
                            rr["仓库名称"] = dt_临时.Rows[0]["仓库名称"];
                        }
                    }
                    rr["未领数量"] = Convert.ToDecimal(rr["待领料总量"]);
                    rr["创建日期"] =t;
                    rr["修改日期"] = t;
                    rr["制单人员"] = CPublic.Var.localUserName;
                    rr["制单人员ID"] = CPublic.Var.LocalUserID;

                    //rr["工单负责人"] = textBox15.Text;
                    //rr["工单负责人ID"] = searchLookUpEdit1.EditValue;
                    //rr["领料人ID"] = searchLookUpEdit2.EditValue;
                    //rr["领料人"] = textBox16.Text;
                    pos++;
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 载入保存
#pragma warning disable IDE1006 // 命名样式
        private void fun_载入工单()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = "select * from 生产记录生产工单表 where 1<>1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            dt_工单 = new DataTable();
            da.Fill(dt_工单);

        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_载入领料()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = "select * from 生产记录生产工单待领料主表 where 1<>1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            dt_领料单_主 = new DataTable();
            da.Fill(dt_领料单_主);

            string sqll = "select * from 生产记录生产工单待领料明细表 where 1<>1";
            SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn);
            dt_领料单_子 = new DataTable();
            daa.Fill(dt_领料单_子);
            dt_领料单_子.Columns.Add("物料编号");
            //dt_领料单_子.ColumnChanged += dt_领料单_子_ColumnChanged;
            gc.DataSource = dt_领料单_子;
        }

        //void dt_领料单_子_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        //{
        //    if (e.Column.ColumnName == "物料编码")
        //    {
        //        DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
        //        DataRow[] ds = dt_物料.Select(string.Format("物料编码 = '{0}'", e.Row["物料编码"].ToString()));
        //        if (ds.Length > 0)
        //        {
        //            dr["物料编码"] = e.Row["物料编码"].ToString();
        //            e.Row["物料名称"] = ds[0]["物料名称"];
        //            e.Row["规格型号"] = ds[0]["规格型号"];
        //            e.Row["物料编号"] = ds[0]["物料编号"];
        //        }
        //        ds = dt_成品.Select(string.Format("可改制物料编码 = '{0}'", e.Row["物料编码"].ToString()));
        //        if (ds.Length > 0)
        //        {
        //            dr["物料编码"] = e.Row["物料编码"].ToString();
        //            e.Row["物料名称"] = ds[0]["可改制物料名称"];
        //            e.Row["规格型号"] = ds[0]["规格型号"];
        //            e.Row["物料编号"] = ds[0]["物料编号"];
        //        }
        //    }
        //}

#pragma warning disable IDE1006 // 命名样式
        private void fun_临时BOM()
#pragma warning restore IDE1006 // 命名样式
        {
            //string sql = "select * from 生产记录生产工单表 where 1<>1";
            //SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            //DataTable dt = new DataTable();
            //da.Fill(dt);
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_保存工单()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = "select * from 生产记录生产工单表 where 1<>1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dt_工单);
            //更新制令为生效状态
            DateTime t = CPublic.Var.getDatetime();
            string sql_1 = string.Format(@"update 生产记录生产制令表 set 生效=1,生效日期='{0}',生效人员='{1}',生效人员ID='{2}',预完工日期='{3}',生产制令类型='改制制令' 
                                    where  生产制令单号='{4}'", Convert.ToDateTime(t.ToString("yyyy-MM-dd HH:mm:ss"))
                                                        , CPublic.Var.localUserName, CPublic.Var.LocalUserID, dr_制令["预完工日期"], textBox1.Text.Trim());
            CZMaster.MasterSQL.ExecuteSQL(sql_1, strconn);
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_保存领料()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = "select * from 生产记录生产工单待领料主表 where 1<>1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dt_领料单_主);

            string sqll = "select * from 生产记录生产工单待领料明细表 where 1<>1";
            SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn);
            new SqlCommandBuilder(daa);
            daa.Update(dt_领料单_子);
        }
        #endregion

        #region 界面操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                gv.CloseEditor();
                this.BindingContext[dt_领料单_子].EndCurrentEdit();
                
                fun_生成记录();
                fun_保存工单();
                fun_保存领料();
                a = true;
                this.Close();
                //MessageBox.Show("保存成功");
                foreach (DataRow dr in dt_工单.Rows)
                {
                    StockCore.StockCorer.fun_物料数量_实际数量(dr_制令["物料编码"].ToString(),dr["仓库号"].ToString(),true);
                }
                foreach (DataRow dr in dt_领料单_子.Rows)
                {
                    StockCore.StockCorer.fun_物料数量_实际数量(dr["物料编码"].ToString(), dr["仓库号"].ToString(), true);
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
            a = false;
            this.Close();
        }

#pragma warning disable IDE1006 // 命名样式
        private void button1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = dt_领料单_子.NewRow();
            dt_领料单_子.Rows.Add(dr);
            dr["生产工单类型"] = "改制工单";
            repositoryItemSearchLookUpEdit1.DataSource = dt_成品;
            repositoryItemSearchLookUpEdit1.DisplayMember = "目标物料编码";
            repositoryItemSearchLookUpEdit1.ValueMember = "目标物料编码";
        }

#pragma warning disable IDE1006 // 命名样式
        private void button3_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = dt_领料单_子.NewRow();
            dt_领料单_子.Rows.Add(dr);
            dr["生产工单类型"] = "改制工单";
            repositoryItemSearchLookUpEdit1.DataSource = dt_物料;
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
        }

#pragma warning disable IDE1006 // 命名样式
        private void button2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            dr.Delete();
        }
        #endregion

#pragma warning disable IDE1006 // 命名样式
        private void repositoryItemSearchLookUpEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);

            if (e.NewValue != null)
            {
                DataRow[] ds = dt_物料.Select(string.Format("物料编码 = '{0}'", e.NewValue.ToString()));
                if (ds.Length > 0)
                {
                    dr["物料编码"] = e.NewValue;

                    dr["物料名称"] = ds[0]["物料名称"];
                    dr["规格型号"] = ds[0]["规格型号"];
                    dr["物料编号"] = ds[0]["物料编号"];
                }
                ds = dt_成品.Select(string.Format("目标物料编码 = '{0}'", e.NewValue.ToString()));
                if (ds.Length > 0)
                {
                    dr["物料编码"] = e.NewValue;
                    dr["物料名称"] = ds[0]["目标物料名称"];
                    dr["规格型号"] = ds[0]["规格型号"];
                    dr["物料编号"] = ds[0]["物料编号"];
                }
            }
            else
            {
               
                dr["物料名称"] = "";
                dr["规格型号"] = "";
                dr["物料编号"] = ""; 
            }
        }
    }
}
