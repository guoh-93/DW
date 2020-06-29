using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
namespace ERPreport
{
    public partial class ui财务打印 : UserControl
    {
        #region 变量
        string strcon = CPublic.Var.strConn;
        string sql = "";
        DataTable dtM;
        string str_打印机 = "";
        /// <summary>
        /// false 为采购单
        /// true 为销售单
        /// </summary>
        string str_单据类型 = "";
        #endregion

        public ui财务打印()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_check();
                fun_条件();
                fun_search();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void fun_check()
        {
            if (dateEdit1.EditValue == null || dateEdit2.EditValue == null || dateEdit1.EditValue.ToString() == "" || dateEdit2.EditValue.ToString() == "")
            {
                throw new Exception("未选择日期");
            }
            if (comboBox1.Text.ToString() == "")
            {
                throw new Exception("未选择单据类型");
            }
            if (checkBox1.Checked == true)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择相关单位");
                }
            }
        }
        private void fun_条件()
        {
            sql = "";
            DateTime t = Convert.ToDateTime(dateEdit2.EditValue).AddDays(1);
            t = new DateTime(t.Year, t.Month, t.Day);

            sql = string.Format(" and 创建日期>'{0}' and 创建日期<'{1}' ", dateEdit1.EditValue, t);
         
 
                if (comboBox1.Text.ToString() == "销售订单")
                {
                    sql = string.Format(@"select [销售订单号],[客户编号],[业务员],[税率],[订单方式],[税前金额],[税后金额],[帐期],[客户名],[创建日期],[销售备注]
                    from 销售记录销售订单主表 where 生效=1  ") + sql;
                    if (checkBox1.Checked == true)
                    {
                        sql = sql + " and 客户编号='" + searchLookUpEdit1.EditValue + "'";
                    }
                }
                else if (comboBox1.Text.ToString() == "采购订单")
                {
                    sql = string.Format(@"select [采购单号],[未税金额],[税率],[税金],[总金额],[供应商ID],[供应商],[员工号],[经办人],创建日期 
                from 采购记录采购单主表 where 生效=1   ") + sql;

                    if (checkBox1.Checked == true)
                    {
                        sql = sql + " and 供应商ID='" + searchLookUpEdit1.EditValue + "'";
                    }
                }
                else if (comboBox1.Text.ToString() == "生产工单")
                {
                    sql = string.Format(" and 制单日期>'{0}' and 制单日期<'{1}' ", dateEdit1.EditValue, t);
                    if (checkBox2.Checked == true)
                    {
                        sql = sql + " and a.物料编码='" + searchLookUpEdit2.EditValue + "'";
                    }
                    sql = string.Format(@"select  生产工单号,生产制令单号,a.物料编码,a.物料名称,原ERP物料编号,a.原规格型号,a.规格型号,加急状态,生产数量,车间名称,工单负责人ID,工单负责人,a.生效日期,备注1 from 生产记录生产工单表 a
                            left join  基础数据物料信息表 b on  b.物料编码=a.物料编码  where a.关闭 =0 " ) + sql;
                }
                else if (comboBox1.Text.ToString() == "其他出库")
                {

                    sql = @"select  其他出库单号,a.出库类型,a.出入库申请单号,b.备注1,a.领用人员,a.操作人员,b.原因分类 from  其他出库主表  a
                         left join 其他出入库申请主表 b  on  a.出入库申请单号=b.出入库申请单号 where 作废=0 " + sql;

                }
                else
                {
                    throw new Exception("单据类型不对");

                }


           //  sql = sql + string.Format("'{0}'", searchLookUpEdit1.EditValue);
            
       

        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            checkBox1.Visible = true;
            searchLookUpEdit1.Visible = true;
            checkBox2.Visible = false;
            searchLookUpEdit2.Visible = false;
            string s = "";
            if (comboBox1.Text.ToString() == "销售订单")
            {
                s = "select 客户编号 as 编号,客户名称 as 名称 from 客户基础信息表 where 停用=0";

            }
            else if (comboBox1.Text.ToString() == "采购订单")
            {
                s = "select 供应商ID as 编号,供应商名称  as 名称 from 采购供应商表 where 供应商状态='在用'";
            }
            else if (comboBox1.Text.ToString() == "生产工单")
            {
                checkBox2.Visible = true;
                searchLookUpEdit2.Visible = true;
            }
            else if (comboBox1.Text.ToString() == "其他出库")
            {

                checkBox1.Visible = false;
                searchLookUpEdit1.Visible = false;
                checkBox2.Visible = false;
                searchLookUpEdit2.Visible = false;
            }
            else
            {
                throw new Exception("单据类型不对");

            }
            if (comboBox1.Text.ToString() != "生产工单" && comboBox1.Text.ToString() != "其他出库")
            {
                DataTable dt = new DataTable();
                dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                searchLookUpEdit1.Properties.DataSource = dt;
                searchLookUpEdit1.Properties.DisplayMember = "名称";
                searchLookUpEdit1.Properties.ValueMember = "编号";
            }

        }
        private void fun_search()
        {
            dtM = new DataTable();
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dtM;
            gridControl1.MainView.PopulateColumns();
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

            if (MessageBox.Show(string.Format("确认打印"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                this.printDialog1.Document = this.printDocument1;

                DialogResult result = this.printDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {

                    str_打印机 = this.printDocument1.PrinterSettings.PrinterName;

                    fun_打印();
                }
            }
        }

        private void fun_打印()
        {
            if (comboBox1.Text.ToString() == "销售订单")
            {
                str_单据类型 = "销售订单";
            }
            else if (comboBox1.Text.ToString() == "采购订单")
            {
                str_单据类型 = "采购订单";
            }
            else if (comboBox1.Text.ToString() == "生产工单")
            {
                str_单据类型 = "生产工单";
            }
            else if (comboBox1.Text.ToString() == "其他出库")
            {
                str_单据类型 = "其他出库";
            }
            Thread thDo;
            thDo = new Thread(Dowork);
            thDo.IsBackground = true;
            thDo.Start();

        }
        public void Dowork()
        {
            if (str_单据类型 == "销售订单")
            {
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    ItemInspection.print_FMS.fun_销售单(gridView1.GetDataRow(i)["销售订单号"].ToString(), str_打印机);
                }
            }
            else if (str_单据类型 == "采购订单")
            {
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    ItemInspection.print_FMS.fun_采购单(gridView1.GetDataRow(i)["采购单号"].ToString(), str_打印机);
                }

            }
            else if (str_单据类型 == "生产工单")
            {
                DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                ItemInspection.print_FMS.fun_print_生产工单_A5(r, 1,false,str_打印机);
            }
            else if (str_单据类型 == "其他出库")
            {
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    string str_仓管员 = gridView1.GetDataRow(i)["操作人员"].ToString();
                    string s_申请单号 = gridView1.GetDataRow(i)["出入库申请单号"].ToString();
       
                    string sql = string.Format(@"select 其他出库子表.*,库存总数,货架描述 from 其他出库子表,仓库物料数量表,基础数据物料信息表 
           where 其他出库子表.物料编码=仓库物料数量表.物料编码 and 基础数据物料信息表.物料编码=仓库物料数量表.物料编码  and 出入库申请单号='{0}'", s_申请单号);
                          DataTable dtP= CZMaster.MasterSQL.Get_DataTable(sql, strcon);

                          int count = dtP.Rows.Count / 14;
                          if (dtP.Rows.Count % 14 != 0)
                          {
                              count++;
                          }
                    ItemInspection.print_FMS.fun_print_其他出库_A5(str_仓管员,s_申请单号,dtP,count,true,str_打印机);
                }

            }
        }

        private void ui财务打印_Load(object sender, EventArgs e)
        {
            string s = "";
             s = "select 物料编码,原ERP物料编号,图纸编号,n原ERP规格型号,规格  from 基础数据物料信息表 where 停用=0";
           
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            searchLookUpEdit2.Properties.DataSource = dt;
            searchLookUpEdit2.Properties.DisplayMember = "原ERP物料编号";
            searchLookUpEdit2.Properties.ValueMember = "物料编码";

        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
 
     
    }
}
