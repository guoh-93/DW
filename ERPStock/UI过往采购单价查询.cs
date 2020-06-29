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

 


    public partial class UI过往采购单价查询 : UserControl
    {

        #region 变量

        DataTable dtM = new DataTable();
        DataTable dt_物料; 
        string strcon = CPublic.Var.strConn;
        string str_wlbh = "";

        #endregion

        public UI过往采购单价查询()
        {
            InitializeComponent();
            DateTime dtime = CPublic.Var.getDatetime();
            dtime = new DateTime(dtime.Year, dtime.Month, 1);

            barEditItem1.EditValue = dtime;

            barEditItem2.EditValue = dtime.AddMonths(1).AddSeconds(-1);


        }
        public UI过往采购单价查询(string wlbh)
        {
            InitializeComponent();
            str_wlbh = wlbh;

            bar2.Visible = false;
        }



        private void UI过往采购单价查询_Load(object sender, EventArgs e)
        {
            try
            {
                fun_load();
                if (str_wlbh != "")
                {
              
                    fun_search2();

                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                CZMaster.MasterLog.WriteLog(ex.Message);
            }
            
        }


        private void fun_load()
        {
            string sql = string.Format(@"select 物料编码,规格型号,物料名称,存货分类  from 基础数据物料信息表 
                                where 物料编码 in(select 物料编码 from 采购记录采购单明细表 group by 物料编码)");
            using(SqlDataAdapter da=new SqlDataAdapter (sql,strcon))
            {
                dt_物料 = new DataTable();
                da.Fill(dt_物料);
                searchLookUpEdit1.Properties.DataSource = dt_物料;
                searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                searchLookUpEdit1.Properties.ValueMember = "物料编码";

            }
        }
        private void fun_search()
        {
                  DateTime dt1=Convert.ToDateTime(barEditItem1.EditValue).AddSeconds(-1);
                  DateTime dt2=Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1);

            string sql = string.Format(@"select 供应商,单价,未税单价,采购数量,生效日期 from 采购记录采购单明细表,基础数据物料信息表 
                    where 采购记录采购单明细表.物料编码=基础数据物料信息表.物料编码 and 基础数据物料信息表.物料编码='{0}' 
                        and 采购记录采购单明细表.作废=0  and  生效日期>='{1}' and  生效日期 <= '{2}'", searchLookUpEdit1.EditValue, dt1.ToString("yyyy-MM-dd"), dt2.ToString("yyyy-MM-dd"));
           
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                dtM = new DataTable();
                da.Fill(dtM);
                gridControl1.DataSource = dtM;
                if (dtM.Rows.Count == 0)
                {
                    MessageBox.Show("未找到记录");
                }

            }
         
        }
        private void fun_search2()
        {

            searchLookUpEdit1.EditValue = str_wlbh;


            string sql = string.Format(@"select top 50 a.供应商,单价,未税单价,采购数量,a.生效日期 from 采购记录采购单明细表 a,基础数据物料信息表 b, 采购记录采购单主表 c 
                    where a.物料编码=b.物料编码 and a.采购单号 =c.采购单号  and a.物料编码 ='{0}'
                        and a.作废=0 and c.作废=0  and c.生效=1 order by a.生效日期 desc ", str_wlbh );

            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                dtM = new DataTable();
                da.Fill(dtM);
                gridControl1.DataSource = dtM;
                if (dtM.Rows.Count == 0)
                {
                    throw new Exception ("未找到记录");
                }
                
            }

        }
        //查找
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_search();
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
            {
                string sql = string.Format("select 物料编码,规格型号,物料名称,存货分类   from 基础数据物料信息表 where 物料编码='{0}'", searchLookUpEdit1.EditValue);

                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql,strcon);
                
                if (dt.Rows.Count > 0)
                {
                    dt_物料.ImportRow(dt.Rows[0]);
                    textBox1.Text = dt.Rows[0]["规格型号"].ToString();
                    textBox3.Text = dt.Rows[0]["物料名称"].ToString();

                }
                else
                {
                    MessageBox.Show("未找到该条数据");
                }
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
         //导出
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                ERPorg.Corg.TableToExcel(dtM,saveFileDialog.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
