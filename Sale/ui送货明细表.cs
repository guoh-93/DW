using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ERPSale
{
    public partial class ui送货明细表 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        DataTable t_片区 = ERPorg.Corg.fun_业务员片区(CPublic.Var.localUserName);
        DataTable dtM;
        public ui送货明细表()
        {
            InitializeComponent();
        }

        private void ui送货明细表_Load(object sender, EventArgs e)
        {
           
            dateEdit1.EditValue = CPublic.Var.getDatetime().AddDays(-15).ToString("yyyy-MM-dd");
            dateEdit2.EditValue = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");
            fun_load();

            if (CPublic.Var.LocalUserTeam != "公司高管权限" && CPublic.Var.LocalUserTeam != "营销助理权限" && CPublic.Var.LocalUserTeam != "ADMIN权限"  && CPublic.Var.LocalUserTeam != "财务部权限")
            {
                gridView1.Columns.Remove(gridColumn13);
                gridView1.Columns.Remove(gridColumn14);
          
            }
           
           
        }

        private void fun_load()
        {

            string sql = string.Format(@"select 客户编号,客户名称 from 客户基础信息表 ");
            if (t_片区.Rows.Count > 0)
            {
                string sx = " where 片区 in (";
                foreach (DataRow r in t_片区.Rows)
                {
                    sx = sx + string.Format("'{0}',", r["片区"]);
                }
                sx = sx.Substring(0, sx.Length - 1) + ")";
                sql = sql + sx;
            }
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            DataTable dt_客户 = new DataTable();
            da.Fill(dt_客户);
            searchLookUpEdit1.Properties.DataSource = dt_客户;
            searchLookUpEdit1.Properties.DisplayMember = "客户名称";
            searchLookUpEdit1.Properties.ValueMember = "客户编号";
            string sql2 = "select 物料类型名称 from 基础数据物料类型表 where 类型级别 = '大类' order by 物料类型名称";
            DataTable dt = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(sql2, strcon);
            da2.Fill(dt);

            searchLookUpEdit2.Properties.DataSource = dt;
            searchLookUpEdit2.Properties.ValueMember = "物料类型名称";
            searchLookUpEdit2.Properties.DisplayMember = "物料类型名称";
            string sql3 = "select 物料类型名称 from 基础数据物料类型表 where 类型级别 = '小类' order by 物料类型名称";
            DataTable dt_小类 = new DataTable();
            SqlDataAdapter da1 = new SqlDataAdapter(sql3, strcon);
            da1.Fill(dt_小类);
            searchLookUpEdit3.Properties.DataSource = dt_小类;
            searchLookUpEdit3.Properties.ValueMember = "物料类型名称";
            searchLookUpEdit3.Properties.DisplayMember = "物料类型名称";
            string sql_片区 = "SELECT [属性值] as 片区 FROM [基础数据基础属性表] where 属性类别 ='片区'";
            DataTable dt_片区 = new DataTable();
            SqlDataAdapter da_片区 = new SqlDataAdapter(sql_片区, strcon);
            da_片区.Fill(dt_片区);
            searchLookUpEdit4.Properties.DataSource = dt_片区;
            searchLookUpEdit4.Properties.ValueMember = "片区";
            searchLookUpEdit4.Properties.DisplayMember = "片区";
        }
        private void fun_search()
        {
//            string sql = string.Format(@"select 销售记录成品出库单明细表.*,销售记录销售出库通知单主表.创建日期,原ERP物料编号,产品线,大类,小类,对外产品线,对外大类,对外小类,税后单价,
//                                 round(出库数量*税后单价,2) as 送货金额,片区,销售记录销售订单主表.备注10 as 下单备注 from 销售记录成品出库单明细表
//                         left  join  销售记录销售出库通知单主表 on  销售记录成品出库单明细表.出库通知单号=销售记录销售出库通知单主表.出库通知单号
//                         left  join 基础数据物料信息表 on 基础数据物料信息表.物料编码=销售记录成品出库单明细表.物料编码
//                         left  join  销售记录销售订单明细表  on  销售记录销售订单明细表.销售订单明细号=销售记录成品出库单明细表.销售订单明细号
//                         left  join  销售记录销售订单主表  on  销售记录销售订单主表.销售订单号=销售记录销售订单明细表.销售订单号
//                         left  join 客户基础信息表 on 客户基础信息表.客户编号 = 销售记录成品出库单明细表.客户编号
//                         where 销售记录成品出库单明细表.作废=0   and 销售记录销售订单明细表.关闭=0 and 销售记录销售订单明细表.作废=0 and 销售记录销售订单明细表.生效=1 ");
            string sql = @"select  *  from 销售送货明细表   where 1=1 ";
            if (checkBox9.Checked == true)
            {
                sql = sql + string.Format(@"and 创建日期>='{0}' and 创建日期<='{1}'",
                    dateEdit1.EditValue, Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1));
            }
            if (checkBox1.Checked == true)
            {
                sql = sql + string.Format(" and 客户编号='{0}'", searchLookUpEdit1.EditValue.ToString());
            }
            if (checkBox2.Checked == true)
            {
                sql = sql + string.Format(" and 大类='{0}'", searchLookUpEdit2.EditValue.ToString());

            }
            if (checkBox3.Checked == true)
            {
                sql = sql + string.Format(" and 小类='{0}'", searchLookUpEdit3.EditValue.ToString());
            }
            if (checkBox4.Checked == true)
            {
                sql = sql + string.Format(" and 生效日期>='{0}' and 生效日期<='{1}'", dateEdit3.EditValue, Convert.ToDateTime(dateEdit4.EditValue).AddDays(1).AddSeconds(-1));
            }
            if (checkBox5.Checked == true)
            {
                sql = sql + string.Format(" and 销售订单明细号 like '%{0}%' ", textBox1.Text);
            }
            if (checkBox6.Checked == true)
            {
                sql = sql + string.Format(" and 成品出库单号 like '%{0}%' ", textBox2.Text);
            }
            if (checkBox7.Checked == true)
            {
                sql = sql + string.Format(" and 片区 ='{0}' ", searchLookUpEdit4.EditValue);
            }
            if (checkBox8.Checked == true)
            {
                sql = sql + string.Format(" and 规格型号 like '%{0}%' ", textBox3.Text);
            }
            if (checkBox10.Checked == true)
            {
                if (comboBox2.Text == "销售部")
                {
                    //销售记录销售订单主表.
                    sql = sql + " and 备注10=''";
                }
                else if (comboBox2.Text == "生产部")
                {
                    sql = sql + " and 备注10<>''";
                    
                }
            }
            if (t_片区.Rows.Count > 0)
            {
                string sx = " and 片区 in (";
                foreach (DataRow r in t_片区.Rows)
                {
                    sx = sx + string.Format("'{0}',", r["片区"]);
                }
                sx = sx.Substring(0, sx.Length - 1)+")";
                sql = sql + sx;
            }
            //if (CPublic.Var.localUserName != "admin" && CPublic.Var.LocalUserTeam != "管理员权限")
            //{
            //    string sql1 = "and left(部门编号,6) = '" + CPublic.Var.localUser部门编号.Substring(0,6) + "'";
            //    sql = sql + sql1;
            //}

            dtM = new DataTable();
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dtM;
          
        }
        private void fun_check()
        {
            if (checkBox9.Checked == true)
            {
                if (dateEdit1.EditValue == null || dateEdit2.EditValue == null || dateEdit1.EditValue.ToString() == "" || dateEdit2.EditValue.ToString() == "")
                {
                    throw new Exception("未选择出库通知时间");
                }
            }

            if (checkBox1.Checked == true)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择客户");
                }
            }
            if (checkBox2.Checked == true)
            {
                if (searchLookUpEdit2.EditValue == null || searchLookUpEdit2.EditValue.ToString() == "")
                {
                    throw new Exception("未选择大类");
                }

            }
            if (checkBox3.Checked == true)
            {
                if (searchLookUpEdit3.EditValue == null || searchLookUpEdit3.EditValue.ToString() == "")
                {
                    throw new Exception("未选择小类");
                }
            }
            if (checkBox4.Checked == true)
            {
                if (dateEdit3.EditValue == null || dateEdit4.EditValue == null || dateEdit3.EditValue.ToString() == "" || dateEdit4.EditValue.ToString() == "")
                {
                    throw new Exception("未选择出库日期");
                }

            }
            if (checkBox5.Checked == true)
            {
                if (textBox1.Text.ToString() == "")
                {
                    throw new Exception("未填写订单号");
                }

            }
            if (checkBox6.Checked == true)
            {
                if (textBox2.Text.ToString() == "")
                {
                    throw new Exception("未填写出库号");
                }

            }
            if (checkBox7.Checked == true)
            {
                if (searchLookUpEdit4.EditValue == null || searchLookUpEdit4.EditValue.ToString() == "")
                {
                    throw new Exception("未填写片区");
                }

            }
            if (checkBox8.Checked == true)
            {
                if (textBox3.Text.ToString() == "")
                {
                    throw new Exception("未选择产品");
                }

            }
            if (checkBox10.Checked == true)
            {
                if (comboBox2.Text == null || comboBox2.Text.ToString() == "")
                {
                    throw new Exception("下单部门未选择");
                }

            }
        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (checkBox8.Checked == true)
            {
                if (textBox3.Text.Length > 4)
                {
                    string sql = string.Format("select 规格型号 from 基础数据物料信息表 where 规格型号 like '{0}%' ", textBox3.Text);
                    DataTable dt = new DataTable();
                    dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    listBox1.Items.Clear();
                    foreach (DataRow dr in dt.Rows)
                    {
                        listBox1.Items.Add(dr["规格型号"]);
                    }
                    listBox1.Visible = true;
                }
            }
        }

        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            textBox3.Text = listBox1.SelectedItem.ToString(); ;
            listBox1.Visible = false;
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView1.GetFocusedRowCellValue(gridView1.FocusedColumn));
                e.Handled = true;
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (dtM != null && dtM.Columns.Count > 0 && dtM.Rows.Count > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    gridControl1.ExportToXlsx(saveFileDialog.FileName);
                    MessageBox.Show("导出成功");
                }
            }
            else
            {
                MessageBox.Show("无记录可导出");
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                ERPorg.Corg.FlushMemory();
                fun_check();
                fun_search();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void barDockControlTop_Click(object sender, EventArgs e)
        {

        }

        private void barDockControlBottom_Click(object sender, EventArgs e)
        {

        }

        private void barDockControlLeft_Click(object sender, EventArgs e)
        {

        }

        private void barDockControlRight_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void searchLookUpEdit4_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void dateEdit4_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void dateEdit3_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void searchLookUpEdit3_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void dateEdit2_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void dateEdit1_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
