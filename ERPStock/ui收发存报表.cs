using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPStock
{
    public partial class ui收发存报表 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        DataTable dtM;

        public ui收发存报表()
        {
            InitializeComponent();
        }

        private void ui收发存报表_Load(object sender, EventArgs e)
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
        //
        private void fun_search()
        {
            string sql = @"select  jzb.*,n核算单价,规格型号,大类,小类,计量单位编码,计量单位,
 ABS(出库数量*n核算单价)as 出库金额1,(入库数量*n核算单价)as 入库金额1,(上月结转数量*n核算单价) 上月结转金额1,(本月结转数量*n核算单价) 本月结转金额1
  from 仓库月出入库结转表 jzb,基础数据物料信息表 base where  jzb.物料编码=base.物料编码";
             if (checkBox1.Checked == true)
            {
                sql = sql + string.Format(" and base.物料编码='{0}'", searchLookUpEdit1.EditValue.ToString());
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
                sql = sql + string.Format(" and jzb.仓库号='{0}' ", searchLookUpEdit4.EditValue.ToString());
            }
            if (checkBox5.Checked == true)
            {
                sql = sql + string.Format(" and base.物料类型='{0}' ", comboBox1.Text);
            }
            if (checkBox6.Checked == true)
            {
               DateTime time1 =Convert.ToDateTime (dateEdit1.EditValue);
               DateTime time2 =Convert.ToDateTime (dateEdit2.EditValue);

                sql = sql + string.Format(" and 年>='{0}' and 月>='{1}' and 年<='{2}' and 月<='{3}'",time1.Year,time1.Month,time2.Year,time2.Month);
            }
      
            dtM = new DataTable();
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dtM;
        }
        private void fun_load()
        {

            string sql = "select 物料编码,规格型号 from 基础数据物料信息表 where 停用=0";
            DataTable dt_物料 = new DataTable();
            SqlDataAdapter da_物料 = new SqlDataAdapter(sql, strcon);
            da_物料.Fill(dt_物料);
            searchLookUpEdit1.Properties.DataSource = dt_物料;
            searchLookUpEdit1.Properties.ValueMember = "物料编码";
            searchLookUpEdit1.Properties.DisplayMember = "物料编码";

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


            string sql_仓库 = "SELECT [属性值] as 仓库名称,属性字段1 as 仓库号 FROM  基础数据基础属性表 where 属性类别 ='仓库类别'";
            DataTable dt_仓库 = new DataTable();
            SqlDataAdapter da_仓库 = new SqlDataAdapter(sql_仓库, strcon);
            da_仓库.Fill(dt_仓库);
            searchLookUpEdit4.Properties.DataSource = dt_仓库;
            searchLookUpEdit4.Properties.ValueMember = "仓库号";
            searchLookUpEdit4.Properties.DisplayMember = "仓库名称";
          
        }
        private void fun_check()
        {
            if (checkBox1.Checked == true)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择物料");
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
                if (searchLookUpEdit4.EditValue == null || searchLookUpEdit4.EditValue.ToString() == "")
                {
                    throw new Exception("未选择仓库");
                }

            }
            if (checkBox5.Checked == true)
            {
                if (comboBox1.Text == null || comboBox1.Text=="")
                {
                    throw new Exception("未选择物料类型");
                }

            }
            if (checkBox6.Checked == true)
            {
                if (dateEdit1.EditValue == null || dateEdit2.EditValue == null || dateEdit1.EditValue.ToString() == "" || dateEdit2.EditValue.ToString() == "")
                {
                    throw new Exception("未填写出库号");
                }

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
                    ERPorg.Corg.TableToExcel(dtM, saveFileDialog.FileName);
                    MessageBox.Show("导出成功");
                }
            }
            else
            {
                MessageBox.Show("没有记录可以导出");

            }
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

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }
    }
}
