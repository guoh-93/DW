using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;

using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ReworkMould
{
    public partial class ui返修申请 : UserControl
    {
        #region
        string strcon = CPublic.Var.strConn;
        DataTable dt_PickingList;
        DataTable dt_inventory;
        string str_单号 = "";
        bool bl_新增 = true;
        #endregion


        public ui返修申请()
        {
            InitializeComponent();
        }
        /// <summary>
        /// ss为需修改的申请单号
        /// </summary>
        /// <param name="ss"></param>
        public ui返修申请(string ss)
        {
            InitializeComponent();
            str_单号 = ss; 
            bl_新增 = false;
            textBox1.Text = str_单号;
        }

        private void fun_load()
        {
            string s = @"select  base.物料编码,base.物料名称,base.规格型号,base.仓库号,base.仓库名称,库存总数,base.计量单位编码,base.计量单位 from 基础数据物料信息表 base
            left  join 仓库物料数量表 kc on kc.物料编码=base.物料编码 and base.仓库号=kc.仓库号
            where 自制=1 and 停用=0";

            dt_inventory = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            searchLookUpEdit1.Properties.DataSource = dt_inventory;
            searchLookUpEdit1.Properties.DisplayMember = "物料编码";
            searchLookUpEdit1.Properties.ValueMember = "物料编码";
            if (!bl_新增)
            {
               
                s = string.Format("select *  from 新_返修申请主表 where 申请单号='{0}'", str_单号);
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                searchLookUpEdit1.EditValue = dt.Rows[0]["返修产品编码"].ToString(); //触发事件 加载dt_PickingList
                s = string.Format("select  * from 新_返修申请子表 where 申请单号='{0}' order by POS", str_单号);
                dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                int c=dt.Rows.Count;

                for (int i = c - 1; i >= 0; i--)
                {
                    DataRow[] r = dt_PickingList.Select(string.Format("物料编码 ='{0}'", dt.Rows[i]["物料编码"]));
                    if (r.Length > 0) { r[0]["选择"] = true; r[0]["数量"] = dt.Rows[i]["数量"]; }
                    else
                    {

                        dt.Rows[i].Delete(); //从后往前遍历 若要删除不影响前面的索引
                    }
                }
                gridControl1.DataSource = dt_PickingList;
            }
 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bl_提交"> 指示是否提交,区分保存与提交审核</param>
        private void fun_save(bool bl_提交)
        {
            string s_车间 = "";
            DateTime t = CPublic.Var.getDatetime();
            string s_主 = string.Format("select  * from 新_返修申请主表 where 申请单号='{0}'",str_单号);
            DataTable dt_主 = CZMaster.MasterSQL.Get_DataTable(s_主, strcon);
            if (dt_主.Rows.Count > 0)
            {
                dt_主.Rows[0]["返修类型"] = comboBoxEdit1.EditValue;
                dt_主.Rows[0]["返修产品编码"] = searchLookUpEdit1.EditValue.ToString();
                string str_目标产品 = "";
                if (comboBoxEdit1.EditValue.ToString() == "A->A")
                {
                    dt_主.Rows[0]["目标产品编码"] = str_目标产品 = searchLookUpEdit1.EditValue.ToString();
                }
                else
                {
                    dt_主.Rows[0]["目标产品编码"] = str_目标产品 = searchLookUpEdit2.EditValue.ToString();
                }
                dt_主.Rows[0]["数量"] = dt_PickingList.Rows[0]["数量"];  //在加载pickinglist的时候 已经把产品insert at 第一条了此处数量即为 返修数量
                dt_主.Rows[0]["制单人员"] = CPublic.Var.localUserName;
                dt_主.Rows[0]["制单人员ID"] = CPublic.Var.LocalUserID;
                dt_主.Rows[0]["制单日期"] = t;
                dt_主.Rows[0]["生产备注"] = textBox6.Text;
                dt_主.Rows[0]["预完工日期"] =Convert.ToDateTime(dateEdit1.EditValue);

                string s = string.Format("select  车间编号,车间 from 基础数据物料信息表 where 物料编码='{0}'", str_目标产品);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                dt_主.Rows[0]["车间编号"] = temp.Rows[0]["车间编号"];
                dt_主.Rows[0]["车间名称"] = s_车间 = temp.Rows[0]["车间"].ToString();
            }
            else
            {
                DataRow dr = dt_主.NewRow();
                string s_单号 = string.Format("RW{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                                CPublic.CNo.fun_得到最大流水号("RW", t.Year, t.Month));
                dr["申请单号"] = textBox1.Text = s_单号;
                dr["返修类型"] = comboBoxEdit1.EditValue;
                dr["返修产品编码"] = searchLookUpEdit1.EditValue.ToString();
                string str_目标产品 = "";
                if (comboBoxEdit1.EditValue.ToString() == "A->A")
                {
                    dr["目标产品编码"] = str_目标产品 = searchLookUpEdit1.EditValue.ToString();
                }
                else
                {
                    dr["目标产品编码"] = str_目标产品 = searchLookUpEdit2.EditValue.ToString();
                }
                dr["数量"] = dt_PickingList.Rows[0]["数量"];  //在加载pickinglist的时候 已经把产品insert at 第一条了此处数量即为 返修数量
                dr["制单人员"] = CPublic.Var.localUserName;
                dr["制单人员ID"] = CPublic.Var.LocalUserID;
                dr["制单日期"] = t;
                dr["生产备注"] = textBox6.Text;
                dr["预完工日期"] = Convert.ToDateTime(dateEdit1.EditValue);
                string s = string.Format("select  车间编号,车间 from 基础数据物料信息表 where 物料编码='{0}'", str_目标产品);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                dr["车间编号"] = temp.Rows[0]["车间编号"];
                dr["车间名称"] = s_车间= temp.Rows[0]["车间"].ToString();
                dt_主.Rows.Add(dr);
            }
            DataTable dt_子 = new DataTable();
            DataTable dt_审核 = new DataTable();
            if(bl_提交) dt_审核 = ERPorg.Corg.fun_PA("返修申请", textBox1.Text, s_车间); //此函数内已经区分是新增或修改了
            if (bl_新增)
            {
                string s_子 = string.Format("select * from 新_返修申请子表 where 1=2");
                dt_子 = CZMaster.MasterSQL.Get_DataTable(s_子, strcon);
                DataView dv = new DataView(dt_PickingList);
                dv.RowFilter = "选择=1";
                int i = 1;
                foreach (DataRow r in dv.ToTable().Rows)
                {
                    DataRow r_子 = dt_子.NewRow();
                    r_子["GUID"] = System.Guid.NewGuid() ;
                    r_子["申请单号"] = textBox1.Text;
                    r_子["POS"] = i;
                    r_子["申请明细号"] = textBox1.Text + i++.ToString("00");
                    r_子["物料编码"] = r["物料编码"];
                    r_子["物料名称"] = r["物料名称"];
                    r_子["规格型号"] = r["规格型号"];
                    r_子["数量"] = r["数量"];
                    r_子["计量单位编码"] = r["计量单位编码"];
                    r_子["计量单位"] = r["计量单位"];
                    r_子["仓库号"] = r["仓库号"];
                    r_子["仓库名称"] = r["仓库名称"];
                    dt_子.Rows.Add(r_子);
                }
            }
            else
            {
                string s_子 = string.Format("select * from 新_返修申请子表 where 申请单号='{0}'", textBox1.Text);
                dt_子 = CZMaster.MasterSQL.Get_DataTable(s_子, strcon);
                DataView dv = new DataView(dt_PickingList);
                dv.RowFilter = "选择=1";
                DataTable dtx = dv.ToTable();
                int c = dt_子.Rows.Count;
                //先遍历dt_子 去掉取消勾的
                for (int i = c - 1; i >= 0; i--)
                {
                  DataRow []r= dtx.Select(string.Format("物料编码='{0}'", dt_子.Rows[i]["物料编码"]));
                  if (r.Length == 0) dt_子.Rows[i].Delete();
                }
                //再遍历 dtx ,同步dt_子中数据或者新增
                int x =1;
                foreach (DataRow dr in dtx.Rows)
                {
                    DataRow[] r = dt_子.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (r.Length > 0)
                    {
                        r[0]["数量"] = dr["数量"];
                        r[0]["申请明细号"] = textBox1.Text + "-" + x.ToString("00");
                        r[0]["POS"] = x++;
                    }
                    else
                    {
                        DataRow r_子 = dt_子.NewRow();
                        r_子["GUID"] = System.Guid.NewGuid();
                        r_子["申请单号"] = textBox1.Text;
                        r_子["POS"] = x;
                        r_子["申请明细号"] = textBox1.Text + x++.ToString("00");
                        r_子["物料编码"] = dr["物料编码"];
                        r_子["物料名称"] = dr["物料名称"];
                        r_子["规格型号"] =dr["规格型号"];
                        r_子["数量"] = dr["数量"];
                        r_子["计量单位编码"] = dr["计量单位编码"];
                        r_子["计量单位"] = dr["计量单位"];
                        r_子["仓库号"] = dr["仓库号"];
                        r_子["仓库名称"] = dr["仓库名称"];
                        dt_子.Rows.Add(r_子);
                    }

                }
            }
            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("rw");
            try
            {
                SqlDataAdapter da;
                SqlCommand cmd = new SqlCommand(s_主, conn, ts);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt_主);
                cmd = new SqlCommand("select * from 新_返修申请子表 where 1=2", conn, ts);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt_子);
                if (dt_审核.Columns.Count > 0)
                {
                    cmd = new SqlCommand("select * from 单据审核申请表 where 1=2", conn, ts);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_审核);
                }
                ts.Commit();
                
            }
            catch
            {
                ts.Rollback();
                throw new Exception("生效失败");
            }
        }
        private void comboBoxEdit1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboBoxEdit1.EditValue.ToString() == "A->B") //返修品与最终产品不一致  
            {
                label6.Visible = true;
                label7.Visible = true;
                label8.Visible = true;
                textBox4.Visible = true;
                textBox5.Visible = true;
                searchLookUpEdit2.Visible = true;
                searchLookUpEdit2.Properties.DataSource = dt_inventory;
                searchLookUpEdit2.Properties.DisplayMember = "物料编码";
                searchLookUpEdit2.Properties.ValueMember = "物料编码";
            }
            else
            {
                label6.Visible = false;
                label7.Visible = false;
                label8.Visible = false;
                textBox4.Visible = false;
                textBox5.Visible = false;
                searchLookUpEdit2.Visible = false;
            }
        }
        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
            {
                //先把所有子项加载出来
                dt_PickingList = new DataTable();
                dt_PickingList = ERPorg.Corg.billofM(dt_PickingList, searchLookUpEdit1.EditValue.ToString());
                string s = "base.物料编码 in (";
                foreach (DataRow dr in dt_PickingList.Rows)
                {
                    s += "'" + dr["子项编码"].ToString() + "',";
                }
                s = s.Substring(0, s.Length - 1) + ") order by 物料编码";
                s = @"select base.物料编码,base.物料名称,base.规格型号,base.仓库号,base.仓库名称,库存总数,计量单位编码,计量单位 from 基础数据物料信息表 base
                    Left join 仓库物料数量表 kc on kc.物料编码=base.物料编码 and base.仓库号=kc.仓库号  where " + s;
                dt_PickingList = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                //再把返修品 插到第一行
                DataRow[] r_产品 = dt_inventory.Select(string.Format("物料编码='{0}'", searchLookUpEdit1.EditValue));
                DataRow r = dt_PickingList.NewRow();
                r["物料编码"] = r_产品[0]["物料编码"];
                r["物料名称"] = r_产品[0]["物料名称"];
                r["规格型号"] = r_产品[0]["规格型号"];
                r["仓库号"] = r_产品[0]["仓库号"];
                r["仓库名称"] = r_产品[0]["仓库名称"];
                r["计量单位编码"] = r_产品[0]["计量单位编码"];
                r["计量单位"] = r_产品[0]["计量单位"];
                r["库存总数"] = r_产品[0]["库存总数"];
                dt_PickingList.Rows.InsertAt(r, 0);
                dt_PickingList.Columns.Add("数量", typeof(decimal));
                dt_PickingList.Columns.Add("选择", typeof(bool));
                gridControl1.DataSource = dt_PickingList;
                textBox2.Text = r_产品[0]["物料名称"].ToString();
                textBox3.Text = r_产品[0]["规格型号"].ToString();
            }
        }
        private void fun_check()
        {
            if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
            {
                throw new Exception("未选择需返修的产品");
            }
            if (searchLookUpEdit2.Visible == true && (searchLookUpEdit2.EditValue == null || searchLookUpEdit2.EditValue.ToString() == ""))
            {
                throw new Exception("未选择返修后产品");
            }
            if (comboBoxEdit1.EditValue == null || comboBoxEdit1.EditValue.ToString() == "") throw new Exception("未选择返修类型");
            DataView dv = new DataView(dt_PickingList);
            dv.RowFilter = "选择=1";
            if (dv.Count == 90) throw new Exception("未选择领任何料");
            foreach (DataRow dr in dv.ToTable().Rows)
            {
                decimal dec = 0;
                if (!decimal.TryParse(dr["数量"].ToString(), out dec)) throw new Exception("数量输入有误,请检查");
                else if (dec < 0) throw new Exception("输入数量不可小于0");
            }

            if (dateEdit1.EditValue == null || dateEdit1.EditValue == "")  throw new Exception("未选择预完工日期");
         


        }
        private void ui返修申请_Load(object sender, EventArgs e)
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

        private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit2.EditValue != null && searchLookUpEdit2.EditValue.ToString() != "")
            {
                DataRow[] r = dt_inventory.Select(string.Format("物料编码='{0}'", searchLookUpEdit2.EditValue));
                textBox5.Text = r[0]["物料名称"].ToString();
                textBox4.Text = r[0]["规格型号"].ToString();
            }
        }
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        //save
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                gridView1.CloseEditor();
                this.BindingContext[dt_PickingList].EndCurrentEdit();
                this.ActiveControl = null;
                
                fun_check();
                fun_save(false);
                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        //提交审核
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridView1.CloseEditor();
                this.BindingContext[dt_PickingList].EndCurrentEdit();
                this.ActiveControl = null;
                fun_check();
                fun_save(true);
                MessageBox.Show("提交成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

        }
    }
}
