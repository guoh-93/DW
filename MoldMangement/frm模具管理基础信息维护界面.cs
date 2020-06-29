using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace MoldMangement
{
    public partial class frm模具管理基础信息维护界面 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dtM = null;
        DataRow drM = null;
        DataView dv = null;
        Boolean isReady = false;
        Boolean NewlyAdded = false;
        DataTable dt_原材料 = null;
        public static DataRow dr = null;
        #endregion

        #region 自用类
        public frm模具管理基础信息维护界面()
        {
            InitializeComponent();
        }

        private void frm模具管理基础信息维护界面_Load(object sender, EventArgs e)
        {
            try
            {
                //devGridControlCustom1.strConn = CPublic.Var.strConn;
                //devGridControlCustom1.UserName = CPublic.Var.localUserName;
                this.gv.IndicatorWidth = 40;
                fun_下拉框();
                fun_载入();
                drM = dtM.NewRow();
                fun_清空(drM);
                isReady = false;
                fun_原材料();
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "模具管理_载入时发生了点小错误，嘿嘿嘿。");
            }
        }

        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {

                drM = gv.GetDataRow(gv.FocusedRowHandle);
                dataBindHelper1.DataFormDR(drM);
                txt_模具编码.Text = drM["模具编码"].ToString();
                textBox2.Text = drM["模具名称"].ToString();
                //快速选择
                if (checkBox3.Checked == true)
                {
                    isReady = true;
                    int i = 0;
                    foreach (DataRow dr in dtM.Rows)
                    {
                        if (dr.RowState == DataRowState.Deleted) continue;
                        if (dr.RowState == DataRowState.Added)
                        {
                            if (MessageBox.Show("存在未保存的数据，是否需要保存？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                i = 1;
                            }
                            else
                            {
                                dr.Delete();
                                break;
                            }
                        }
                    }
                    if (i == 0)
                    {
                        drM = gv.GetDataRow(gv.FocusedRowHandle);
                        dataBindHelper1.DataFormDR(drM);
                        textBox19.Text = drM["模具价格"].ToString() ;
                       textBox13.Text= drM["穴数"].ToString();
                        date_入库日期.EditValue = drM["入库日期"];
                    }
                }
                if (e != null && e.Button == MouseButtons.Right)
                {
                    dr = gv.GetDataRow(gv.FocusedRowHandle);
                    contextMenuStrip1.Show(gc, new Point(e.X, e.Y));
                    gv.CloseEditor();
                    this.BindingContext[dtM].EndCurrentEdit();
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "模具管理_快速选择时发生了点小错误，嘿嘿嘿。");
            }
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEdit1.EditValue.ToString() == "" || searchLookUpEdit1.EditValue == null)
                { }
                else
                {
                    DataRow[] ds = dt_原材料.Select(string.Format("物料编码 = '{0}'",searchLookUpEdit1.EditValue.ToString().Trim()));
                    txt_零件名称.Text = ds[0]["物料名称"].ToString();
                    txt_零件图号.Text = ds[0]["图纸编号"].ToString();
                   
                }
            }
            catch(Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "模具管理_原材料发生了点小错误，嘿嘿嘿。");
            }
        }
        #endregion

        #region 方法
        private void fun_原材料()
        {
            try
            {
                string sql = "select 物料编码,物料名称,规格型号,图纸编号 from 基础数据物料信息表 where 物料类型 <> '成品'";
                dt_原材料 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql,strconn);
                da.Fill(dt_原材料);
                searchLookUpEdit1.Properties.DataSource = dt_原材料;
                searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                searchLookUpEdit1.Properties.ValueMember = "物料编码";
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "模具管理_原材料发生了点小错误，嘿嘿嘿。");
            }
        }

        private void fun_下拉框()
        {
            try
            {
                com_保养属性.Properties.Items.Clear();
                com_模具类型.Properties.Items.Clear();
                com_模具属性.Properties.Items.Clear();
                com_在库状态.Properties.Items.Clear();
                com_主备模.Properties.Items.Clear();
                string sql = "select * from 基础数据基础属性表 order by POS";
                DataTable dt_属性 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_属性);
                foreach (DataRow r in dt_属性.Rows)
                {
                    if (r["属性类别"].ToString().Equals("保养属性"))
                    {
                        com_保养属性.Properties.Items.Add(r["属性值"].ToString());
                    }
                    if (r["属性类别"].ToString().Equals("模具类型"))
                    {
                        com_模具类型.Properties.Items.Add(r["属性值"].ToString());
                    }
                    if (r["属性类别"].ToString().Equals("模具属性"))
                    {
                        com_模具属性.Properties.Items.Add(r["属性值"].ToString());
                    }
                    if (r["属性类别"].ToString().Equals("在库状态"))
                    {
                        com_在库状态.Properties.Items.Add(r["属性值"].ToString());
                    }
                    if (r["属性类别"].ToString().Equals("主备模"))
                    {
                        com_主备模.Properties.Items.Add(r["属性值"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "模具管理_下拉框发生了点小错误，嘿嘿嘿。");
            }
        }

        private void fun_载入()
        {
            string sql = string.Format(@"select 模具管理基础信息表.*,模具物料信息关联表.审核1 from 模具管理基础信息表 
  LEFT join  模具物料信息关联表 on 模具物料信息关联表.模具编号 = 模具管理基础信息表.模具编号 order by 模具物料信息关联表.模具编号");
            dtM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            gc.DataSource = dtM;
        }
        private void fun_check()
        {
            if (textBox19.Text == "") throw new Exception("模具价格为必填项");
            if (textBox13.Text == "") throw new Exception("穴数为必填项");
            if (textBox12.Text == "") throw new Exception("使用寿命为必填项");
            try
            {
                decimal dec = Convert.ToDecimal(textBox19.Text);
            }
            catch (Exception)
            {
              throw new Exception("模具价格格式输入不正确");
            }
            try
            {
                decimal dec = Convert.ToDecimal(textBox13.Text);
            }
            catch (Exception)
            {
                throw new Exception("穴数格式输入不正确");
            }
            try
            {
                decimal dec = Convert.ToDecimal(textBox12.Text);
            }
            catch (Exception)
            {
                throw new Exception("使用寿命格式输入不正确");
            }
        }
        private void fun_保存()
        {
            if (NewlyAdded == true)
            {
                if (isReady == false)
                {
                    dtM.Rows.Add(drM);
                    string s = "select  max(序号) from  [模具管理基础信息表] ";
                    DataRow dr = CZMaster.MasterSQL.Get_DataRow(s, strconn);
                    drM["模具编号"] = txt_模具编码.Text = "MJ" + Convert.ToInt32(dr["序号"]).ToString("0000");
                    //txt_模具编码.Text = "MJ" + dtM.Rows.Count.ToString("0000");
                    //drM["GUID"] = System.Guid.NewGuid();
                }
                string sql = string.Format("select * from 模具管理基础信息表 where 1<>1");
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                dataBindHelper1.DataToDR(drM);
                drM["模具价格"] =textBox19.Text;
                drM["穴数"] = textBox13.Text;
                drM["入库日期"] = date_入库日期.EditValue;
                da.Update(dtM);
                dtM.AcceptChanges();
                isReady = true;
                NewlyAdded = false;
            }
            if (NewlyAdded == false)
            {
                dataBindHelper1.DataToDR(drM);
                drM["模具价格"] = textBox19.Text;
                drM["穴数"] = textBox13.Text;
                drM["入库日期"] = date_入库日期.EditValue;
                string sql = string.Format("select * from 模具管理基础信息表 where 1<>1");
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dtM);
            }

        }

        private void fun_新增()
        {
            NewlyAdded = true;
            isReady = true;
            drM = dtM.NewRow();
            dtM.Rows.Add(drM);
          
            //drM["GUID"] = System.Guid.NewGuid();
            fun_清空(drM);
            string sql = "select max(convert(int,序号))序号 from  [模具管理基础信息表]";
            DataRow dr = CZMaster.MasterSQL.Get_DataRow(sql,strconn);
            drM["模具编号"] = txt_模具编码.Text = "MJ" + (Convert.ToInt32(dr["序号"])+1).ToString("0000");
            drM["序号"] = (Convert.ToInt32(dr["序号"]) + 1).ToString("0000");
        }

        private void fun_刷新()
        {
            fun_载入();
            drM = dtM.NewRow();
            fun_清空(drM);

            isReady = false;
        }

        private void fun_清空(DataRow dr)
        {
            date_入库日期.EditValue = System.DateTime.Now;

            string sql = string.Format("select * from 模具管理基础信息表 where 1<>1");
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            DataTable dt = new DataTable();
              da.Fill(dt);

            //foreach (DataColumn dc in dr.Table.Columns)
            //{
            //    if (dc.ColumnName == "入库日期" || dc.ColumnName == "模具编号" || dc.ColumnName == "GUID") continue;
            //    if (dc.ColumnName == "穴数" || dc.ColumnName == "已使用模次" || dc.ColumnName == "模具价格" || dc.ColumnName == "浇杆重量" || dc.ColumnName == "零件重量" || dc.ColumnName == "一级保养费用")
            //    {
            //        dr[dc.ColumnName] = 0;
            //    }
            //    else
            //    {
            //        dr[dc.ColumnName] = "";
            //    }
            //}
              DataRow r = dt.NewRow();
              dataBindHelper1.DataFormDR(r);
              textBox19.Text ="";
              textBox13.Text = "";
        }

        private void fun_复制()
        {
            isReady = true;
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            dataBindHelper2.DataFormDR(dr);
        }
        #endregion

        #region 界面操作
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_刷新();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                CZMaster.MasterLog.WriteLog(ex.Message, "模具管理_刷新时发生了点小错误，嘿嘿嘿。");
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                int i = 0;
                foreach (DataRow dr in dtM.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted) continue;
                    if (dr.RowState == DataRowState.Added)
                    {
                        if (MessageBox.Show("存在未保存的数据，是否需要保存？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            i = 1;
                        }
                        else
                        {
                            dr.Delete();
                            break;
                        }
                    }
                }
                if (i == 0)
                {
                    fun_新增();
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "模具管理_新增时发生了点小错误，嘿嘿嘿。");
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                fun_check();
                fun_保存();

                MessageBox.Show("保存成功！");
                barLargeButtonItem1_ItemClick(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                CZMaster.MasterLog.WriteLog(ex.Message, "");
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fun_复制();
        }
        #endregion

        #region checkbox
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            gc.DataSource = dtM;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {//已修改

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
            {
                button1.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
            }
        }
        #endregion

        private void textBox12_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }

        private void 物料信息关联ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dr != null)
            {
                MoldMangement.frm模具物料信息关联 fm = new frm模具物料信息关联();
                fm.ShowDialog();
            }
        }

        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gv_ShownEditor(object sender, EventArgs e)
        {
            gv.ActiveEditor.MouseWheel += ActiveEditor_MouseWheel;
        }

        void ActiveEditor_MouseWheel(object sender, MouseEventArgs e)
        {
            gv.ActiveEditor.MouseWheel -= ActiveEditor_MouseWheel;
            gv.CloseEditor();
            this.BindingContext[gc.DataSource].EndCurrentEdit();
        }

        private void gv_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (gv.GetRow(e.RowHandle) == null)
                {
                    return;
                }
                int j = gv.RowCount; 

                if (gv.GetRowCellValue(e.RowHandle, "审核1").Equals(true))
                {
                    e.Appearance.BackColor = Color.Pink;
                    //e.Appearance.BackColor2 = Color.Pink;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            if (MessageBox.Show(string.Format("确认彻底删除模具:{0}的记录？删除将不可恢复！",dr["模具编号"].ToString()), "警告！", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                dr.Delete();
                string sql = string.Format("select * from 模具管理基础信息表 where 1<>1");
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dtM);
                MessageBox.Show("已删除");
                barLargeButtonItem1_ItemClick(null, null);

            }

        }

     

    }
}
