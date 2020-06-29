using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
namespace ERPproduct
{
    public partial class ui拆单申请 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        bool str_新增 = true;
        DataTable dtP;
        DataTable dtM;
        DataTable dt_物料;
        DataTable dt_仓库;
        string str_单号 = "";
        DataRow dr_拆单申请;
        bool s_提交 = false;
        public ui拆单申请()
        {
            InitializeComponent();
        }
        public ui拆单申请(DataRow rr)
        {
            InitializeComponent();
            str_单号 = rr["申请单号"].ToString();
            dr_拆单申请 = rr;
            str_新增 = false;
            
        }
        public ui拆单申请(string s_拆单申请单号, DataRow dr, DataTable dt)
        {
            InitializeComponent();
            str_新增 = false;
            str_单号 = s_拆单申请单号;
            dr_拆单申请 = dr;
            dtM = dt;
            s_提交 = true;
            str_新增 = false;
        }
        private void ui拆单_Load(object sender, EventArgs e)
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";

                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(panel2, this.Name, cfgfilepath);
                fun_下拉框();
                if (str_新增)
                {
                    fun_清空();
                }
                else
                {
                    fun_load();
                }
                if (s_提交)
                {
                    fun_编辑();
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_编辑()
        {
            searchLookUpEdit1.Enabled = false;
            textBox6.Enabled = false;
            textBox5.Enabled = false;
            barLargeButtonItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barLargeButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        }

        private void fun_下拉框()
        {
            string sql = @"select  base.物料编码,base.物料名称,base.规格型号,kc.仓库号,kc.仓库名称,库存总数,base.计量单位编码,base.计量单位 from 基础数据物料信息表 base
            left  join 仓库物料数量表 kc on kc.物料编码=base.物料编码 
            where  base.物料编码 in  ( select  产品编码 from 基础数据物料BOM表  group  by 产品编码) /*and  (kc.仓库号 = '96' or kc.仓库号 = '97')*/";
            dt_物料 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_物料);
            searchLookUpEdit1.Properties.PopupFormSize = new Size(1500, 400);
            searchLookUpEdit1.Properties.DataSource = dt_物料;
            searchLookUpEdit1.Properties.DisplayMember = "物料编码";
            searchLookUpEdit1.Properties.ValueMember = "物料编码";
            

            dt_仓库 = new DataTable();
            string sql4 = "select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别'";
            da = new SqlDataAdapter(sql4, strconn);
            da.Fill(dt_仓库);
            repositoryItemSearchLookUpEdit1.DataSource = dt_仓库;
            repositoryItemSearchLookUpEdit1.DisplayMember = "仓库号";
            repositoryItemSearchLookUpEdit1.ValueMember = "仓库号";
        }

        private void fun_清空()
        {
            try
            {
                
                dateEdit1.EditValue = CPublic.Var.getDatetime();
                textBox2.Text = CPublic.Var.localUser部门名称;
                textBox1.Text = "";
                searchLookUpEdit1.EditValue = null;
                textBox3.Text = "";
                textBox4.Text = "";
                textBox5.Text = "";
                textBox6.Text = "";
                textBox7.Text = "";
                textBox8.Text = "";
                textBox9.Text = "";
                textBox10.Text = "";
                textBox11.Text = "";
                
                string sql = "select * from 拆单申请子表 where 1<>1";
                dtP = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                gc.DataSource = dtP;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_load()
        {
            string sql = string.Format("select * from 拆单申请子表 where 申请单号 = '{0}'", str_单号);
            dtP = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gc.DataSource = dtP;
            textBox1.Text = dr_拆单申请["申请单号"].ToString();
            textBox2.Text = dr_拆单申请["部门名称"].ToString();
            searchLookUpEdit1.EditValue = dr_拆单申请["物料编码"].ToString();
            textBox3.Text = dr_拆单申请["物料名称"].ToString();
            textBox4.Text = dr_拆单申请["规格型号"].ToString();
            textBox10.Text = dr_拆单申请["仓库号"].ToString();
            textBox11.Text = dr_拆单申请["仓库名称"].ToString();
            textBox8.Text = dr_拆单申请["计量单位编码"].ToString();
            textBox9.Text = dr_拆单申请["计量单位"].ToString();
            textBox7.Text = Convert.ToDecimal(dr_拆单申请["库存总数"].ToString()).ToString();
            textBox6.Text = Convert.ToDecimal(dr_拆单申请["数量"].ToString()).ToString("0");
            textBox5.Text = dr_拆单申请["备注"].ToString();
            dateEdit1.EditValue = dr_拆单申请["申请日期"].ToString();
           
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
             
                if ((e.KeyChar >= 48 && e.KeyChar <= 57) || (e.KeyChar >= 64 && e.KeyChar <= 123) || e.KeyChar == 8 || e.KeyChar == 3 || e.KeyChar == 22 || e.KeyChar == ',')
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
          
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            str_新增 = true;
            str_单号 = "";
            fun_清空();
        }

        //private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
                

        //        DataRow[] ds = dt_物料.Select(string.Format("物料编码 = '{0}'", searchLookUpEdit1.EditValue));
        //        if (ds.Length != 0)
        //        {
        //            textBox3.Text = ds[0]["物料名称"].ToString();
        //            textBox4.Text = ds[0]["规格型号"].ToString();
        //            textBox10.Text = ds[0]["仓库号"].ToString();
        //            textBox11.Text = ds[0]["仓库名称"].ToString();
        //            textBox8.Text = ds[0]["计量单位编码"].ToString();
        //            textBox9.Text = ds[0]["计量单位"].ToString();
        //            textBox7.Text = ds[0]["库存总数"].ToString();
                    
        //        }
        //        if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
        //        {
        //            textBox3.Text = "";
        //            textBox4.Text = "";
        //            textBox10.Text = "";
        //            textBox11.Text = "";
        //            textBox8.Text = "";
        //            textBox9.Text = "";
        //            textBox7.Text = "";
        //            dtP.Clear();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}


        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                this.ActiveControl = null;
                if (MessageBox.Show("是否确认拆卸单已完善？", "确认!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    fun_check();
                    fun_save(false);
                    MessageBox.Show("保存成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_save(bool bl_提交)
        {
            string s_部门 = "";
            DateTime t = CPublic.Var.getDatetime();
            string sql = string.Format("select * from 拆单申请主表 where 申请单号 = '{0}'", str_单号);
                DataTable dt_主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            if (dt_主.Rows.Count > 0)
            {
                dt_主.Rows[0]["申请人"] = CPublic.Var.localUserName;
                dt_主.Rows[0]["申请人ID"] = CPublic.Var.LocalUserID;
                dt_主.Rows[0]["申请日期"] = t;
                dt_主.Rows[0]["部门名称"] = s_部门 = textBox2.Text;
                dt_主.Rows[0]["部门编号"] = CPublic.Var.localUser部门编号;
                dt_主.Rows[0]["物料编码"] = searchLookUpEdit1.EditValue;
                dt_主.Rows[0]["物料名称"] = textBox3.Text;
                dt_主.Rows[0]["规格型号"] = textBox4.Text;
                dt_主.Rows[0]["库存总数"] = Convert.ToDecimal(textBox7.Text);
                dt_主.Rows[0]["数量"] = Convert.ToDecimal(textBox6.Text);
                dt_主.Rows[0]["计量单位编码"] = textBox8.Text;
                dt_主.Rows[0]["计量单位"] = textBox9.Text;
                dt_主.Rows[0]["仓库号"] = textBox10.Text;
                dt_主.Rows[0]["仓库名称"] = textBox11.Text;
                dt_主.Rows[0]["备注"] = textBox5.Text;
                //string sql111 = string.Format("delete 拆单申请子表 where 申请单号 = '{0}'", str_单号);
                //CZMaster.MasterSQL.ExecuteSQL(sql111,strconn);
            }
            else
            {
                DataRow dr = dt_主.NewRow();
                string s_单号 = string.Format("CD{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                                CPublic.CNo.fun_得到最大流水号("CD", t.Year, t.Month));
                str_单号 = s_单号;
                dr["申请单号"] = textBox1.Text = s_单号;
                dr["申请人"] = CPublic.Var.localUserName;
                dr["申请人ID"] = CPublic.Var.LocalUserID;
                dr["申请日期"] = t;
                dr["部门名称"] = s_部门 = textBox2.Text;
                dr["部门编号"] = CPublic.Var.localUser部门编号;
                dr["物料编码"] = searchLookUpEdit1.EditValue;
                dr["物料名称"] = textBox3.Text;
                dr["规格型号"] = textBox4.Text;
                dr["库存总数"] = Convert.ToDecimal(textBox7.Text);
                dr["数量"] = Convert.ToDecimal(textBox6.Text);
                dr["计量单位编码"] = textBox8.Text;
                dr["计量单位"] = textBox9.Text;
                dr["仓库号"] = textBox10.Text;
                dr["仓库名称"] = textBox11.Text;
                dr["备注"] = textBox5.Text;
                dt_主.Rows.Add(dr);
            }
            DataTable dt_审核 = new DataTable();
            if (bl_提交)
            {
                dt_审核 = ERPorg.Corg.fun_PA("生效", "拆单申请", textBox1.Text, s_部门); //此函数内已经区分是新增或修改了
                dt_主.Rows[0]["提交审核"] = true;
            }
            int j = 1;
            
            foreach (DataRow dr in dtP.Rows)
            {             
                dr["POS"] = j;
                dr["申请单号"] = textBox1.Text;
                dr["申请明细号"] = dr["申请单号"] + "-" + j++.ToString("00");
            }
            str_新增 = false;
            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("rw");
            try
            {
                SqlDataAdapter da;
                SqlCommand cmd = new SqlCommand("select * from 拆单申请主表 where 1<>1", conn, ts);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt_主);

                cmd = new SqlCommand("select * from 拆单申请子表 where 1<>1", conn, ts);
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
            catch (Exception)
            {
                ts.Rollback();
                throw new Exception();
            }

        }

        private void fun_check()
        {
            if (searchLookUpEdit1.EditValue.ToString() == "")
            {
                throw new Exception("未选择需拆分的物料");
            }
            if(textBox6.Text == "")
            {
                throw new Exception("未填写需拆分物料数量");
            }
            if (Convert.ToDecimal(textBox6.Text) > Convert.ToDecimal(textBox7.Text))
            {
                throw new Exception("拆分数量大于库存数量");
            }
            foreach (DataRow dr in dtP.Rows)
            {
                if(dr["数量"].ToString() == "")
                {
                    throw new Exception("未填写子项数量");
                }
                if (Convert.ToDecimal(dr["数量"]) == 0)
                {
                    throw new Exception(dr["物料编码"]+"数量为零，请确认");
                }
                if(dr["仓库号"].ToString() == "")
                {
                    throw new Exception("未选择子项仓库");
                }
            }
            string sql = string.Format("select * from 拆单申请主表 where 申请单号 = '{0}'", textBox1.Text);
            DataTable dt1 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            if (dt1.Rows.Count > 0)
            {
                if (Convert.ToBoolean(dt1.Rows[0]["提交审核"]))
                {
                    throw new Exception("该单据已提交审核");
                }
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show("是否确认清单已完善？", "提交审核!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    fun_check();
                    fun_save(true);
                    MessageBox.Show("提交审核成功");

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                
                string sql111 = string.Format("delete 拆单申请子表 where 申请单号 = '{0}'", textBox1.Text);
                CZMaster.MasterSQL.ExecuteSQL(sql111, strconn);
                string sql1 = string.Format("select * from  拆单申请子表 where 1<>1");
                dtP = CZMaster.MasterSQL.Get_DataTable(sql1, strconn);
                if (searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("请选择需拆分的物料");
                }
                if(textBox6.Text == "")
                {
                    throw new Exception("请填写数量");
                }
                if (Convert.ToDecimal(textBox7.Text) == 0)
                {
                    throw new Exception("库存总数为零,不可拆分");
                }
                if (Convert.ToDecimal(textBox6.Text) == 0)
                {
                    throw new Exception("拆分数量不可为零");
                }
                if(Convert.ToDecimal(textBox6.Text)> Convert.ToDecimal(textBox7.Text))
                {
                    throw new Exception("拆分数量大于库存数量");
                }
                string sql = string.Format(@"select a.产品编码,a.子项编码,a.子项名称,a.数量 as bom数量,base.物料编码,base.物料名称,base.规格型号,base.图纸编号,base.计量单位,base.计量单位编码,base.停用,
                                             isnull(c.库存总数,0)库存总数,c.货架描述,c.仓库号,c.仓库名称, base.仓库号 as 默认仓库号,base.仓库名称 as 默认仓库  
                                             from 基础数据物料BOM表 a
                                             left join 基础数据物料信息表 base  on a.子项编码 = base.物料编码
                                             left join 仓库物料数量表 c on base.物料编码 = c.物料编码 and  base.仓库号=c.仓库号
                                             where  产品编码 = '{0}' ", searchLookUpEdit1.EditValue);
                DataTable dt_子项物料 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                foreach (DataRow dr in dt_子项物料.Rows)
                {
                    DataRow dr_子 = dtP.NewRow();
                    dr_子["物料编码"] = dr["子项编码"];
                    dr_子["物料名称"] = dr["子项名称"];
                    dr_子["规格型号"] = dr["规格型号"];
                    dr_子["计量单位"] = dr["计量单位"];
                    dr_子["计量单位编码"] = dr["计量单位编码"];
                    dr_子["仓库号"] = textBox10.Text.Trim();
                    dr_子["仓库名称"] = textBox11.Text.Trim();
                    dr_子["库存总数"] = dr["库存总数"];
                    dr_子["bom数量"] = dr["bom数量"];
                    dr_子["停用"] = dr["停用"];
                    dr_子["数量"] =Convert.ToDecimal(textBox6.Text)* Convert.ToDecimal(dr["bom数量"].ToString());
                    dtP.Rows.Add(dr_子);
                }
                gc.DataSource = dtP;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBox6.Text.Trim() != "")
                {
                    if (Convert.ToDecimal(textBox6.Text)<=0)
                    {
                        throw new Exception("拆分数量必须大于零");
                    }
                    foreach (DataRow dr in dtP.Rows)
                    {
                        dr["数量"] = Convert.ToDecimal(textBox6.Text) * Convert.ToDecimal(dr["bom数量"].ToString());
                    }
                }
                else
                {

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void searchLookUpEdit1View_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow d = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
                textBox3.Text = d["物料名称"].ToString();
                textBox4.Text = d["规格型号"].ToString();
                textBox10.Text = d["仓库号"].ToString();
                textBox11.Text = d["仓库名称"].ToString();
                textBox8.Text = d["计量单位编码"].ToString();
                textBox9.Text = d["计量单位"].ToString();
                textBox7.Text = d["库存总数"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
