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
using System.Reflection;
namespace ERPSale
{
    public partial class 销售发货通知单查询 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dtM = new DataTable();
        public 销售发货通知单查询()
        {
            InitializeComponent();
        }

        private void 销售发货通知单查询_Load(object sender, EventArgs e)
        {
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(this.panel2, this.Name, cfgfilepath);
            dateEdit1.EditValue = CPublic.Var.getDatetime().AddMonths(-1).ToString("yyyy-MM-dd");
            dateEdit2.EditValue = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");
            fun_load();
        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {

        }

        private void fun_search()
        {

            DateTime t1 = Convert.ToDateTime(dateEdit1.EditValue);
            t1 = new DateTime(t1.Year, t1.Month, t1.Day);
            DateTime t2 = Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1);
            t2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);
            if (t2 < t1)
            {
                throw new Exception("结束时间需大于开始时间！");
            }
            string sql = string.Format(@"with cpck as (select 出库通知单明细号,MAX(生效日期) 上次出库日期 from 销售记录成品出库单明细表  group by 出库通知单明细号)
,djsh as (select 关联单号,待审核人  from  单据审核申请表 where 单据类型='销售发货申请' and 作废=0 )
,bz as (select CONVERT(nvarchar,POS)x ,属性值,属性字段1 as 包装描述 from  基础数据基础属性表 where 属性类别='包装方式'),
 result as (select cm.*,szb.销售订单号,szb.目标客户,szb.销售部门,szb.部门编号,客户订单号,包装方式编号,包装方式,包装描述,cpck.上次出库日期,待审核人,tzzb.审核日期,tzzb.出库日期
           ,tzzb.备注 as 发货备注 ,tzzb.审核,tzzb.审核人员    from 销售记录销售出库通知单明细表 cm
           left join 销售记录销售出库通知单主表 tzzb on tzzb.出库通知单号 = cm.出库通知单号
           left join 销售记录销售订单明细表 smx on smx.销售订单明细号=cm.销售订单明细号
           left  join  销售记录销售订单主表 szb on szb.销售订单号=smx.销售订单号
           left join cpck   on cpck.出库通知单明细号 = cm.出库通知单明细号
           left join  bz on x=包装方式编号 
           left join  djsh on djsh.关联单号=tzzb.出库通知单号             
          where cm.作废 = 0 and tzzb.作废 = 0 and tzzb.创建日期>='{0}' and tzzb.创建日期<='{1}'),
kc as   (select 物料编码,sum(库存总数) 库存总数,max(受订量) 受订量,max(在制量) 在制量,max(未领量) 未领量,max(在途量) 在途量 from 仓库物料数量表 
        where (仓库号 in (select 属性字段1 from 基础数据基础属性表 where  属性类别 = '仓库类别'  and  布尔字段2 = 1 ))  group by 物料编码)
          select  result.*,kc.库存总数 from result
          left join kc on kc.物料编码= result.物料编码 where 1=1 ", t1, t2);


            string sql_补 = "";
            if (checkBox1.Checked == true)
            {
                sql_补 = string.Format(@" and  客户编号 = '{0}'", searchLookUpEdit1.EditValue.ToString());
                sql += sql_补;
            }
            if (checkBox2.Checked == true)
            {
                sql_补 = string.Format(@" and  目标客户 like '%{0}%'", searchLookUpEdit3.EditValue.ToString());
                sql += sql_补;
            }


            if (checkBox3.Checked == true)
            {
                sql = sql + string.Format(" and  出库通知单号 = '{0}' ", textBox1.Text.ToString());
                sql += sql_补;

            }
            if (checkBox4.Checked == true)
            {
                sql = sql + string.Format(" and  result.物料编码='{0}'", searchLookUpEdit2.EditValue.ToString());
                sql += sql_补;

            }
            sql = sql + "order by 出库通知单号";
            dtM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            gridControl1.DataSource = dtM;
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
                if (searchLookUpEdit3.EditValue == null || searchLookUpEdit3.EditValue.ToString() == "")
                {
                    throw new Exception("未选择目标客户");
                }
            }
            if (checkBox3.Checked == true)
            {
                if (textBox1.Text == null || textBox1.Text.ToString() == "")
                {
                    throw new Exception("未填写发货通知单号");
                }
            }
            if (checkBox4.Checked == true)
            {
                if (searchLookUpEdit2.EditValue == null || searchLookUpEdit2.EditValue.ToString() == "")
                {
                    throw new Exception("未选择物料");
                }
            }
        }
        private void fun_load()
        {
            try
            {
                string sql = string.Format(@"select 客户编号,客户名称 from 客户基础信息表 where 停用=0");
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                DataTable dt_客户 = new DataTable();
                da.Fill(dt_客户);
                searchLookUpEdit1.Properties.DataSource = dt_客户;
                searchLookUpEdit1.Properties.DisplayMember = "客户名称";
                searchLookUpEdit1.Properties.ValueMember = "客户编号";

                string sql2 = string.Format(@"select 客户编号,客户名称 from 客户基础信息表 where 停用=0");
                SqlDataAdapter da2 = new SqlDataAdapter(sql, strconn);
                DataTable dt_目标客户 = new DataTable();
                da.Fill(dt_目标客户);
                searchLookUpEdit3.Properties.DataSource = dt_目标客户;
                searchLookUpEdit3.Properties.DisplayMember = "客户名称";
                searchLookUpEdit3.Properties.ValueMember = "客户名称";

                string sql_1 = string.Format(@"select 物料编码,规格型号,物料名称,大类,小类 from 基础数据物料信息表 where 停用=0");
                SqlDataAdapter da_1 = new SqlDataAdapter(sql_1, strconn);
                DataTable dt_物料 = new DataTable();
                da_1.Fill(dt_物料);
                searchLookUpEdit2.Properties.DataSource = dt_物料;
                searchLookUpEdit2.Properties.DisplayMember = "物料编码";
                searchLookUpEdit2.Properties.ValueMember = "物料编码";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void simpleButton1_Click_1(object sender, EventArgs e)
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

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {


                if (gridView1.GetRow(e.RowHandle) == null)
                {
                    return;
                }


                if (e.Column.Caption == "完成")
                {
                    string s = gridView1.GetRowCellValue(e.RowHandle, "完成").ToString();
                    if (s != null && s != "")
                    {
                        bool t = bool.Parse(gridView1.GetRowCellValue(e.RowHandle, "完成").ToString());
                        if (t == true)
                        {
                            e.Appearance.BackColor = Color.LightGreen;

                        }
                        else if (t == false)
                        {
                            e.Appearance.BackColor = Color.Pink;

                        }
                    }
                }

                //if (e.Column.Caption == "完成")
                //{
                //    DataRow dr = gridView1.GetDataRow(e.RowHandle);
                //    try
                //    {
                //        if (bool.Parse(e.CellValue.ToString()) ==true)
                //        {
                //            e.Appearance.BackColor = Color.Red;

                //        }
                //        else
                //        {
                //            e.Appearance.BackColor = Color.Green;
                //        }
                //    }
                //    catch
                //    {
                //    }

                //}


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                if (dtM == null || dtM.Columns.Count == 0 || dtM.Rows.Count == 0)
                {

                    throw new Exception("没有数据可以导出");
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    //DataTable tt = dtM.Copy();
                    //tt.Columns.Remove("作废");
                    gridControl1.ExportToXlsx(saveFileDialog.FileName);
                    //ERPorg.Corg.TableToExcel(tt, saveFileDialog.FileName);
                    MessageBox.Show("导出成功");
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if(e.Clicks==2)
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, "ERPStock.dll"));//dr["dll全路径"] = "动态载入dll.dll"
                Type outerForm = outerAsm.GetType("ERPStock.ui可用库存查询", false);//动态载入dll.UI动态载入窗体
                object[] r = new object[1];
                r[0] = dr["物料编码"].ToString();
                UserControl ui = Activator.CreateInstance(outerForm, r) as UserControl;
                CPublic.UIcontrol.Showpage(ui, "可用库存查询");
            }
        }
    }
}
