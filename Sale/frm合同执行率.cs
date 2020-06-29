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
    public partial class frm合同执行率 : UserControl
    {

        #region 变量
        string strcon = CPublic.Var.strConn;
        DataTable dtM = new DataTable(); 

        #endregion 

        #region 加载
        public frm合同执行率()
        {
            InitializeComponent();
            dateEdit1.EditValue = Convert.ToDateTime(System.DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd"));
            dateEdit2.EditValue = Convert.ToDateTime(System.DateTime.Today.ToString("yyyy-MM-dd"));

        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            fun_load();
        }
        #endregion 
        private void fun_search()
        {
            string str="";
              string str_条件="";
            if (checkBox1.Checked == true)
            {
                str_条件 = str_条件 + string.Format(" and xmx.客户编号='{0}'", searchLookUpEdit1.EditValue.ToString());
            }
             if (checkBox2.Checked == true)
            {
                str_条件 = str_条件 + string.Format(" and xmx.销售订单号 like '%{0}%'", textBox1.Text.ToString());

            }
            //if (checkBox4.Checked == true)
            //{
            //    str_条件 = str_条件 + string.Format(" and 片区='{0}'", searchLookUpEdit2.EditValue.ToString());

            //}
            if (checkBox3.Checked == true)
            {
                str_条件 = str_条件 + string.Format(" and 送达日期>='{0}' and 送达日期<='{1}'", dateEdit3.EditValue.ToString(), Convert.ToDateTime(dateEdit4.EditValue).AddDays(1).AddSeconds(-1));
            }
            if (checkBox5.Checked == true)
            {
                if (comboBox1.Text == "销售下单")
                {
                    str_条件 = str_条件 + " and d.备注10=''";
                }
                else
                {
                    str_条件 = str_条件 + " and  d.备注10='计划下单'";

                }
            }
            string sql = string.Format(@"select xmx.*,a.出库量*税后单价 as 期内交付金额,xmx.生效日期 下单日期 ,
                b.未交货量*税后单价 as 期外交付金额,计划确认日期,c.出库日期 as 最近一次出库日期 ,xmx.物料编码 from 销售记录销售订单明细表 xmx
                left  join (select xckmx.销售订单明细号,SUM(出库数量)出库量 from 销售记录成品出库单明细表 xckmx,销售记录销售订单明细表 y where xckmx.销售订单明细号=y.销售订单明细号 
            and  xckmx.生效日期<=送达日期 and   xckmx.备注1=''
	         group by xckmx.销售订单明细号 )a on a.销售订单明细号=xmx.销售订单明细号
              left join (select 销售记录成品出库单明细表.销售订单明细号,SUM(出库数量)未交货量 from 销售记录成品出库单明细表,销售记录销售订单明细表 x  where 销售记录成品出库单明细表.销售订单明细号=x.销售订单明细号 
            and  销售记录成品出库单明细表.生效日期>送达日期 and   销售记录成品出库单明细表.备注1='' group by 销售记录成品出库单明细表.销售订单明细号 )b on b.销售订单明细号=xmx.销售订单明细号
             left  join 客户基础信息表 on 客户基础信息表.客户名称 = xmx.客户
            left  join (select 销售订单明细号,MAX(生效日期)出库日期  from 销售记录成品出库单明细表 where 备注1=''  group by 销售订单明细号)c on c.销售订单明细号=xmx.销售订单明细号
             left join  基础数据物料信息表 on 基础数据物料信息表.物料编码= xmx.物料编码
            left  join  销售记录销售订单主表 d on d.销售订单号=xmx.销售订单号
              where xmx.作废=0 and xmx.关闭=0 and xmx.生效日期>='{1}' 
              and xmx.生效日期<='{2}'{3}",
                                     str,dateEdit1.EditValue, Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1),str_条件);

             DataTable dt = new DataTable();
             dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
             gridControl1.DataSource = dt;
             dt.Columns.Add("期内未交付金额",typeof(double));
             dt.Columns.Add("合同履行率", typeof(double));
           
             dt.Columns.Add("期外未交付金额", typeof(double));


             foreach (DataRow dr in dt.Rows)
             {
                 if (dr["期内交付金额"] == DBNull.Value)
                 {
                     dr["期内交付金额"] = 0;
                 }
                 dr["期内未交付金额"] = Convert.ToDecimal(dr["税后金额"]) - Convert.ToDecimal(dr["期内交付金额"]);
                 if (dr["期外交付金额"] == DBNull.Value)
                 {
                     dr["期外交付金额"] = 0;
                 }
                 dr["期外未交付金额"] = Convert.ToDecimal(dr["税后金额"]) - Convert.ToDecimal(dr["期内交付金额"]) - Convert.ToDecimal(dr["期外交付金额"]);

                 if (Convert.ToDecimal(dr["税后金额"]) == 0)
                 {
                     dr["合同履行率"] = 0;
                 }
                 else
                 {
                     dr["合同履行率"] = Convert.ToDecimal(dr["期内交付金额"]) / Convert.ToDecimal(dr["税后金额"]);
                 }
             }
        }
        private void fun_check()
        {
            if (checkBox1.Checked == true)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择客户");
                }
            }
            if (checkBox2.Checked == true)
            {
                if (textBox1.Text == null || textBox1.Text.ToString() == "")
                {
                    throw new Exception("未填入订单号");
                }

            }
        
            if (checkBox3.Checked == true)
            {
                if (dateEdit3.EditValue == null || dateEdit4.EditValue == null || dateEdit3.EditValue.ToString() == "" || dateEdit4.EditValue.ToString() == "")
                {
                    throw new Exception("未选择要求交货日期");
                }
            }
            //if (checkBox4.Checked == true)
            //{
            //    if (searchLookUpEdit2.EditValue == null || searchLookUpEdit2.EditValue.ToString() == "")
            //    {
            //        throw new Exception("未填写片区");
            //    }
            //}
            if (checkBox5.Checked == true)
            {
                if (comboBox1.Text == null || comboBox1.Text == "")
                {
                    throw new Exception("未选择下单部门");
                }
            }
        }
        private void fun_load()
        {
            string sql = string.Format(@"select 客户编号,客户名称 from 客户基础信息表");
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            DataTable dt_客户 = new DataTable();
            da.Fill(dt_客户);
            searchLookUpEdit1.Properties.DataSource = dt_客户;
            searchLookUpEdit1.Properties.DisplayMember = "客户名称";
            searchLookUpEdit1.Properties.ValueMember = "客户编号";
           
            //string sql_片区 = "SELECT [属性值] as 片区 from [基础数据基础属性表] where 属性类别 ='片区'";
            //DataTable dt_片区 = new DataTable();
            //SqlDataAdapter da_片区 = new SqlDataAdapter(sql_片区, strcon);
            //da_片区.Fill(dt_片区);
            //searchLookUpEdit2.Properties.DataSource = dt_片区;
            //searchLookUpEdit2.Properties.ValueMember = "片区";
            //searchLookUpEdit2.Properties.DisplayMember = "片区";
        }
        
        private void gridControl1_Click(object sender, EventArgs e)
        {
              
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_check();
                fun_search();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                gridControl1.ExportToXlsx(saveFileDialog.FileName);

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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





    }
}
