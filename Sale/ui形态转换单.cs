using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.IO;

namespace ERPSale
{
    public partial class ui形态转换单 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        bool str_新增 = true;
        string str_单号 = "";
        string cfgfilepath = "";
        DataTable dtM;
        DataTable dtP;
        DataTable dt_仓库;
        DataTable dt_物料;
        DataRow dr_形态转换;
        int flag = 0;
        int i = 1;
        bool s_提交 = false;
        public ui形态转换单(DataRow rr)
        {
            InitializeComponent();
            str_单号 = rr["形态转换单号"].ToString();
            dr_形态转换 = rr;
            str_新增 = false;
            string sql = string.Format("select max(组号) as 组号 from 销售形态转换子表 where 形态转换单号 = '{0}'", str_单号);
            DataTable t = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            i += Convert.ToInt32(t.Rows[0]["组号"].ToString());
        }
        public ui形态转换单(string s_形态转换单号, DataRow dr, DataTable dt)
        {
            InitializeComponent();
            str_新增 = false;
            str_单号 = s_形态转换单号;
            dr_形态转换 = dr;
            dtM = dt ;
            s_提交 = true;
        }
        public ui形态转换单()
        {
            InitializeComponent();
        }
        private void 形态转换单_Load(object sender, EventArgs e)
        {
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(this.panel4, this.Name, cfgfilepath);
            fun_下拉框();

            if (str_新增 == false)
            {
                //textBox3.Text = str_单号;
                fun_加载数据();
            }
            else
            {                
                fun_清空();
            }
            if (Convert.ToBoolean(s_提交))
            {
                fun_编辑();
            }
        }

        private void fun_编辑()
        {
            barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barLargeButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            dateEdit1.Enabled = !s_提交;
        //    textBox9.Enabled = !s_提交;
            textBox9.ReadOnly = true;
            button1.Enabled = !s_提交;
            button2.Enabled = !s_提交;
            gv.OptionsBehavior.Editable = !s_提交;
        }

        private void fun_加载数据()
        {
            textBox3.Text = dr_形态转换["形态转换单号"].ToString();
            textBox5.Text = dr_形态转换["申请人ID"].ToString();
            textBox8.Text = dr_形态转换["申请人"].ToString();
            textBox10.Text = dr_形态转换["部门编号"].ToString();
            textBox7.Text = dr_形态转换["部门名称"].ToString();
            dateEdit1.EditValue = dr_形态转换["申请日期"].ToString();
            textBox9.Text = dr_形态转换["备注"].ToString(); 
            string sql = string.Format("select * from 销售形态转换子表 where 形态转换单号 = '{0}'", str_单号);
            dtP = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gc.DataSource = dtP;
        }

        private void fun_清空()
        {
            textBox3.Text = "";
            textBox5.Text = CPublic.Var.LocalUserID;
            textBox8.Text = CPublic.Var.localUserName;
            textBox10.Text = CPublic.Var.localUser部门编号;
            textBox7.Text = CPublic.Var.localUser部门名称;
            textBox9.Text = "";
            str_单号 = "";
            dateEdit1.EditValue = CPublic.Var.getDatetime();
            string sql = "select * from 销售形态转换子表 where 1<>1";
            dtP = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gc.DataSource = dtP;
        }

        private void fun_下拉框()
        {
            string sql = @"select  base.物料编码,base.物料名称,base.规格型号,kc.仓库号,kc.仓库名称,库存总数,base.计量单位编码,base.计量单位 from 基础数据物料信息表 base
            left  join 仓库物料数量表 kc on kc.物料编码=base.物料编码 
            where   在研=0";
            dt_物料 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_物料);
            repositoryItemSearchLookUpEdit3.PopupFormSize = new Size(1500, 400);
            repositoryItemSearchLookUpEdit3.DataSource = dt_物料;
            repositoryItemSearchLookUpEdit3.DisplayMember = "物料编码";
            repositoryItemSearchLookUpEdit3.ValueMember = "物料编码";
            
            dt_仓库 = new DataTable();
            string sql4 = "select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别'";
            da = new SqlDataAdapter(sql4, strconn);
            da.Fill(dt_仓库);
            repositoryItemSearchLookUpEdit4.DataSource = dt_仓库;
            repositoryItemSearchLookUpEdit4.DisplayMember = "仓库号";
            repositoryItemSearchLookUpEdit4.ValueMember = "仓库号";
            

        }

        //新增功能
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                str_新增 = true;
                i = 1;
                fun_清空();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //保存功能
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show("是否确认转换单已完善？", "确认!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    gv.CloseEditor();
                    this.BindingContext[dtP].EndCurrentEdit();
                    this.ActiveControl = null;
                    fun_check();
                     
                    fun_save(false);
                    MessageBox.Show("保存成功");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //检验数据
        private void fun_check()
        {
            if(dtP.Rows.Count == 0)
            {
                throw new Exception("没有需要形态转换的物料");
            }
            foreach(DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                if(dr["物料编码"].ToString() == "")
                {
                    throw new Exception("物料编码不能为空");
                }
                if(dr["仓库号"].ToString() == "")
                {
                    throw new Exception("仓库号不能为空");
                }
                if (dr["数量"].ToString() == "")
                {
                    throw new Exception("数量不能为空");
                }
                if(dr["类型"].ToString() == "转换前")
                {
                    if (Convert.ToDecimal(dr["数量"].ToString()) > Convert.ToDecimal(dr["库存总数"].ToString()))
                    {
                        throw new Exception("要转换物料数量大于库存数量");
                    }
                }
                if (dr["类型"].ToString() == "转换后")
                {
                    DataRow[] dr1 = dtP.Select(string.Format("组号 = '{0}' and 类型 = '转换前'",Convert.ToInt32(dr["组号"])));
                    if(dr1[0]["物料编码"].ToString() == dr["物料编码"].ToString())
                    {
                        throw new Exception("组号"+Convert.ToInt32(dr["组号"])+"转换前后物料相同，请确认");
                    }
                }
                

            }

            string sql = string.Format("select * from 销售形态转换主表 where 形态转换单号 = '{0}'", textBox3.Text);
            DataTable dt1 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            if (dt1.Rows.Count > 0)
            {
                if (Convert.ToBoolean(dt1.Rows[0]["提交审核"]))
                {
                    throw new Exception("该单据已提交审核");
                }
            }
        }


        //保存数据
        private void fun_save(bool bl_提交)
        {
            string s_部门 = "";
            DateTime t = CPublic.Var.getDatetime();
            string sql = string.Format("select * from 销售形态转换主表 where 形态转换单号 = '{0}'",str_单号);
            DataTable dt_主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            
            
            if (dt_主.Rows.Count > 0)
            {               
                dt_主.Rows[0]["申请人"] = CPublic.Var.localUserName;
                dt_主.Rows[0]["申请人ID"] = CPublic.Var.LocalUserID;
                dt_主.Rows[0]["申请日期"] = t;
                dt_主.Rows[0]["部门名称"]  = s_部门 = textBox7.Text;
                dt_主.Rows[0]["部门编号"] = textBox10.Text;
                dt_主.Rows[0]["备注"] = textBox9.Text;
            }
            else
            {
                DataRow dr = dt_主.NewRow();
                string s_单号 = string.Format("XTZH{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                                CPublic.CNo.fun_得到最大流水号("XTZH", t.Year, t.Month));
                str_单号 = s_单号;
                dr["形态转换单号"] = textBox3.Text = s_单号;                            
                dr["申请人"] = CPublic.Var.localUserName;
                dr["申请人ID"] = CPublic.Var.LocalUserID;
                dr["申请日期"] = t;
                dr["部门名称"]  = s_部门 = textBox7.Text;
                dr["部门编号"] = textBox10.Text;
                dr["备注"] = textBox9.Text;
                dt_主.Rows.Add(dr);
            }
            DataTable dt_审核 = new DataTable();
            if (bl_提交)
            {
               dt_审核 = ERPorg.Corg.fun_PA("生效", "形态转换申请", textBox3.Text, s_部门); //此函数内已经区分是新增或修改了
               dt_主.Rows[0]["提交审核"] = true;
            }
            DataTable dt_子 = new DataTable();
            int j = 1;
            //string s_子 = string.Format("select * from 销售形态转换子表 where 形态转换单号 = '{0}'",str_单号);
            //dt_子 = CZMaster.MasterSQL.Get_DataTable(s_子, strconn);
            foreach (DataRow dr in dtP.Rows)
            {
                //  DataRow dr_子 = dt_子.NewRow();
                if (dr.RowState == DataRowState.Deleted) continue;
                dr["POS"] = j;
                dr["形态转换单号"] = textBox3.Text;
                dr["形态转换明细号"] = dr["形态转换单号"] + "-" + j++.ToString("00");              
            }

            str_新增 = false;
            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("rw");
            try
            {
                SqlDataAdapter da;
                SqlCommand cmd = new SqlCommand("select * from 销售形态转换主表 where 1<>1", conn, ts);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt_主);

                cmd = new SqlCommand("select * from 销售形态转换子表 where 1<>1", conn, ts);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dtP);
                if (dt_审核.Columns.Count > 0)
                {
                    cmd = new SqlCommand("select * from 单据审核申请表 where 1=2", conn, ts);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_审核);
                }
                ts.Commit();
            }
            catch(Exception ex)
            {
                ts.Rollback();
                MessageBox.Show(ex.Message);
            }
        }


        //审核功能
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show("是否确认信息已完善？", "提交审核!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    gv.CloseEditor();
                    this.BindingContext[dtP].EndCurrentEdit();
                    this.ActiveControl = null;
                    fun_check();
                    //if(str_单号 != "")
                    //{
                    //    string sql = string.Format("delete 销售形态转换子表 where 形态转换单号 = '{0}'", str_单号);
                    //    CZMaster.MasterSQL.ExecuteSQL(sql, strconn);
                    //}
                    
                    fun_save(true);
                    MessageBox.Show("提交审核成功");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            
        }


        //关闭
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

         

        private void gv_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (e.Column.FieldName == "仓库号")
                {
                    dr["仓库号"] = e.Value;
                    DataRow[] ds = dt_仓库.Select(string.Format("仓库号 = '{0}'", dr["仓库号"]));
                    dr["仓库名称"] = ds[0]["仓库名称"];
                    //dr["仓库名称"] = sr["仓库名称"].ToString();
                    string sql = "select * from 仓库物料数量表 where 物料编码 = '" + dr["物料编码"] + "' and 仓库号 = '" + dr["仓库号"] + "'";
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_物料数量 = new DataTable();
                    da.Fill(dt_物料数量);
                    if (dt_物料数量.Rows.Count == 0)
                    {
                        dr["库存总数"] = 0;
                        // dr["有效总数"] = 0;
                    }
                    else
                    {
                        dr["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
                        //  dr["有效总数"] = dt_物料数量.Rows[0]["有效总数"];
                    }
                    if (dr["类型"].ToString() == "转换前")
                    {
                        DataRow[] dr1 = dtP.Select(string.Format("组号 = '{0}' and 类型 = '转换后'", Convert.ToInt32(dr["组号"])));
                        if (e.Value.ToString() != "")
                        {
                            dr1[0]["仓库号"] = e.Value;
                            ds = dt_仓库.Select(string.Format("仓库号 = '{0}'", dr1[0]["仓库号"]));
                            dr1[0]["仓库名称"] = ds[0]["仓库名称"];
                            sql = "select * from 仓库物料数量表 where 物料编码 = '" + dr1[0]["物料编码"] + "' and 仓库号 = '" + dr1[0]["仓库号"] + "'";

                            dt_物料数量 = CZMaster.MasterSQL.Get_DataTable(sql,strconn);
                            if (dt_物料数量.Rows.Count == 0)
                            {
                                dr1[0]["库存总数"] = 0;
                                 
                            }
                            else
                            {
                                dr1[0]["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
                                 
                            }
                        }
                        else
                        {
                            dr1[0]["仓库号"] = "";
                        }
                    }
                }
                if (e.Column.FieldName == "数量")
                {
                    if (dr["类型"].ToString()=="转换前")
                    {
                        DataRow[] dr1 = dtP.Select(string.Format("组号 = '{0}' and 类型 = '转换后'", Convert.ToInt32(dr["组号"])));
                        if (e.Value.ToString() != "")
                        {
                            dr1[0]["数量"] = Convert.ToDecimal(e.Value);
                        }
                        else
                        {
                            dr1[0]["数量"] = 0;
                        }
                    }
                    
                }
                if (e.Column.FieldName =="物料编码")
                {
                    if (dr["类型"].ToString() == "转换前")
                    {
                        DataRow[] dr1 = dtP.Select(string.Format("组号 = '{0}' and 类型 = '转换后'", Convert.ToInt32(dr["组号"])));
                        if (e.Value.ToString() != "")
                        {
                            dr1[0]["计量单位编码"] = dr["计量单位编码"];
                            dr1[0]["计量单位"] = dr["计量单位"];
                            dr1[0]["仓库号"] = dr["仓库号"];
                            dr1[0]["仓库名称"] = dr["仓库名称"];
                        }
                        string sql = "select * from 仓库物料数量表 where 物料编码 = '" + dr1[0]["物料编码"] + "' and 仓库号 = '" + dr1[0]["仓库号"] + "'";

                        DataTable dt_物料数量 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                        if (dt_物料数量.Rows.Count == 0)
                        {
                            dr1[0]["库存总数"] = 0;

                        }
                        else
                        {
                            dr1[0]["库存总数"] = dt_物料数量.Rows[0]["库存总数"];

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (flag == 0)
                {
                    var ofd = new OpenFileDialog();
                    if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        //  dt_SaleOrder = ERPorg.Corg.ExcelXLSX(ofd);
                        bool bl = ERPorg.Corg.IsFileInUse(ofd.FileName);
                        if (bl) throw new Exception("文件已打开或被占用中");
                        DataTable dt_SaleOrder = ERPorg.Corg.ReadExcelToDataTable(ofd.FileName);
                      
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }
 

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {


                DataRow dr = dtP.NewRow();
                dr["组号"] = i;
                dr["类型"] = "转换前";
                dtP.Rows.Add(dr);
                DataRow dr1 = dtP.NewRow();
                dr1["组号"] = i;
                dr1["类型"] = "转换后";
                dtP.Rows.Add(dr1);
                i++;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);

                DataRow[] ds = dtP.Select(string.Format("组号 = {0}", dr["组号"]));
                if (ds.Length > 0)
                {

                    for (int i = ds.Length-1;i>=0;i--)
                    {
                        ds[i].Delete();
                    }
                    //foreach (DataRow dr_物料 in ds)
                    //{
                    //    dtP.Rows.Remove(dr_物料);
                    //}
                                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void repositoryItemSearchLookUpEdit3View_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow d = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);

                if (d != null)
                {
                    dr["物料名称"] = d["物料名称"];
                    dr["物料编码"] = d["物料编码"];
                    dr["规格型号"] = d["规格型号"];
                    dr["库存总数"] = d["库存总数"];
                    //dr["货架描述"] = d["货架描述"];
                    if(dr["类型"].ToString() == "转换后")
                    {
                        DataRow[] dr1 = dtP.Select(string.Format("组号 = '{0}' and 类型 = '转换前'",Convert.ToInt32(dr["组号"])));
                        dr["仓库号"] = dr1[0]["仓库号"];
                        dr["仓库名称"] = dr1[0]["仓库名称"];
                        dr["计量单位"] = dr1[0]["计量单位"];
                        dr["计量单位编码"] = dr1[0]["计量单位编码"];
                        string sql = string.Format("select * from  仓库物料数量表 where 物料编码 = '{0}' and 仓库号 = '{1}'", dr["物料编码"], dr["仓库号"]);
                        DataTable dt_库存 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                        if (dt_库存.Rows.Count > 0)
                        {
                            dr["库存总数"] = dt_库存.Rows[0]["库存总数"];
                        }
                        else
                        {
                            dr["库存总数"] = 0;
                        }
                    }
                    else
                    {
                        dr["仓库号"] = d["仓库号"];
                        dr["仓库名称"] = d["仓库名称"];
                        dr["计量单位"] = d["计量单位"];
                        dr["计量单位编码"] = d["计量单位编码"];

                        DataRow[] dr1 = dtP.Select(string.Format("组号 = '{0}' and 类型 = '转换后'", Convert.ToInt32(dr["组号"])));
                        dr1[0]["仓库号"] = dr["仓库号"];
                        dr1[0]["仓库名称"] = dr["仓库名称"];
                        dr1[0]["计量单位"] = dr["计量单位"];
                        dr1[0]["计量单位编码"] = dr["计量单位编码"];
                        string sql = string.Format("select * from  仓库物料数量表 where 物料编码 = '{0}' and 仓库号 = '{1}'", dr["物料编码"], dr["仓库号"]);
                        DataTable dt_库存 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                        if (dt_库存.Rows.Count > 0)
                        {
                            dr1[0]["库存总数"] = dt_库存.Rows[0]["库存总数"];
                        }
                        else
                        {
                            dr1[0]["库存总数"] = 0;
                        }
                    }
                     
                    
                }
                else
                {
                    dr["物料编码"] = "";
                    dr["物料名称"] = "";
                    dr["规格型号"] = "";
                    dr["库存总数"] = "";
                    //dr["货架描述"] = "";
                    dr["仓库名称"] = "";
                    dr["仓库号"] = "";
                    dr["计量单位"] = "";
                    dr["计量单位编码"] = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void repositoryItemSearchLookUpEdit3View_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            DataRow d = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);

                if (d != null)
                {


                    dr["物料名称"] = d["物料名称"];
                    dr["物料编码"] = d["物料编码"];
                    dr["规格型号"] = d["规格型号"];
                    dr["库存总数"] = d["库存总数"];
                    //dr["货架描述"] = d["货架描述"];
                    dr["仓库号"] = d["仓库号"];
                    dr["仓库名称"] = d["仓库名称"];
                    dr["计量单位"] = d["计量单位"];
                    dr["计量单位编码"] = d["计量单位编码"];
                }
                else
                {
                    dr["物料编码"] = "";
                    dr["物料名称"] = "";
                    dr["规格型号"] = "";
                    dr["库存总数"] = "";
                    //dr["货架描述"] = "";
                    dr["仓库名称"] = "";
                    dr["仓库号"] = "";
                    dr["计量单位"] = "";
                    dr["计量单位编码"] = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if(dr["类型"].ToString() == "转换前")
                {
                    gridColumn6.OptionsColumn.ReadOnly = false;
                    gridColumn8.OptionsColumn.ReadOnly = false;
                    gridColumn6.OptionsColumn.AllowEdit = true;
                    gridColumn8.OptionsColumn.AllowEdit = true;
                }
                else
                {
                    gridColumn6.OptionsColumn.ReadOnly = true;
                    gridColumn8.OptionsColumn.ReadOnly = true;
                    gridColumn6.OptionsColumn.AllowEdit = false;
                    gridColumn8.OptionsColumn.AllowEdit = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
