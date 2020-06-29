using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ERPproduct
{
    public partial class ui电气发货看板 : UserControl
    {
        public ui电气发货看板()
        {
            InitializeComponent();
        }


        string strcon = CPublic.Var.strConn;

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }



        private void fun_load()
        {
            //try

            //{
            DateTime t1 = CPublic.Var.getDatetime();
            string t2 = t1.ToString("yyyy-MM-dd");
            /////
            ///苏  物流确认完成算完成
            ///19-10-23   增加 and b.完成=0
            string sql = string.Format(@"   select  a.作废,a.审核,a.审核日期,a.出库日期,a.包装日期,a.厂区,a.是否加急, b.*  from  销售记录销售出库通知单主表 a 
                    left  join  销售记录销售出库通知单明细表 b   on  a.出库通知单号 = b.出库通知单号  where a.审核=1 and a.作废=0 and  b.物流确认=0 
                   and b.完成=0 ");
            DataTable dt_出库 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);



            dt_出库.Columns.Add("包装方式", typeof(string));
            dt_出库.Columns.Add("未包装数", typeof(decimal));
            dt_出库.Columns["出库数量"].ColumnName = "数量";
            dt_出库.Columns["出库通知单号"].ColumnName = "通知单号";
            dt_出库.Columns.Remove("备注");
            dt_出库.Columns["销售备注"].ColumnName = "备注";

            //19-10-23 增加 and a.领取完成=0  
            string sq_借出 = string.Format(@"    select a.* ,b.申请人,b.工号,b.包装确认, b.目标客户 from  借还申请表附表 a
 left join  借还申请表 b on a.申请批号 = b.申请批号  where b.审核=1 and b.作废=0 and  b.物流确认=0   and a.领取完成=0   ");
            DataTable dt_借出 = CZMaster.MasterSQL.Get_DataTable(sq_借出, strcon);

            // dt_出库.Columns["操作员ID"].ColumnName = "厂区";

            dt_借出.Columns.Add("未包装数", typeof(decimal));

            dt_借出.Columns["申请数量"].ColumnName = "数量";
            dt_借出.Columns["申请批号"].ColumnName = "通知单号";
            dt_借出.Columns["目标客户"].ColumnName = "客户";
            dt_借出.Columns["申请日期"].ColumnName = "包装日期";

            DataTable dt_显示 = new DataTable();
            dt_显示.Columns.Add("物料编码", typeof(string));
            dt_显示.Columns.Add("物料名称", typeof(string));
            dt_显示.Columns.Add("规格型号", typeof(string));
            dt_显示.Columns.Add("通知单号", typeof(string));
            dt_显示.Columns.Add("包装方式", typeof(string));
            dt_显示.Columns.Add("客户", typeof(string));
            dt_显示.Columns.Add("备注", typeof(string));
            dt_显示.Columns.Add("包装日期", typeof(DateTime));
            dt_显示.Columns.Add("包装确认", typeof(bool));
            dt_显示.Columns.Add("物流确认", typeof(bool));
            dt_显示.Columns.Add("包装抽检", typeof(bool));
            dt_显示.Columns.Add("数量", typeof(decimal));
            dt_显示.Columns.Add("未包装数", typeof(decimal));
            dt_显示.Columns.Add("厂区", typeof(string));
            dt_显示.Columns.Add("是否加急", typeof(string));
            dt_显示.Columns.Add("已包装数量", typeof(decimal));
            foreach (DataRow dr in dt_出库.Rows)
            {
                if (dr["物料名称"].ToString() != "维修劳务" && dr["物料名称"].ToString() != "维护劳务" && dr["厂区"].ToString() != "生产二厂")
                {
                    string sq = string.Format("select * from 销售记录销售订单明细表 where 销售订单明细号='{0}'", dr["销售订单明细号"]);
                    DataRow d22r = CZMaster.MasterSQL.Get_DataRow(sq, strcon);
                    dr["包装方式"] = d22r["包装方式"];
                    if (dr["包装方式"].ToString() == "其他方式" || dr["包装方式"].ToString() == "")
                    {
                        dr["包装方式"] = dr["包装方式"].ToString();
                    }
                    else
                    {
                        string sql4 = string.Format("select 属性字段1 as 包装描述,属性值 as  包装名称 from 基础数据基础属性表 where 属性类别 =  '包装方式' and 属性值='{0}' ", dr["包装方式"].ToString());
                        DataTable dt_bao = CZMaster.MasterSQL.Get_DataTable(sql4, CPublic.Var.strConn);

                        dr["包装方式"] = dt_bao.Rows[0]["包装描述"].ToString();
                    }
                    DataRow dr_显示 = dt_显示.NewRow();
                    dr_显示 = dr;
                    dt_显示.ImportRow(dr_显示);

                }

            }

            foreach (DataRow dr in dt_借出.Rows)
            {
                if (dr["物料名称"].ToString() != "维修劳务" && dr["物料名称"].ToString() != "维护劳务")
                {

                    DataRow dr_显示 = dt_显示.NewRow();
                    dr_显示 = dr;
                    dt_显示.ImportRow(dr_显示);

                }

            }


            DataTable dt_z = dt_显示.Clone();

            foreach (DataRow dr in dt_显示.Rows)
            {
                if (dr["是否加急"].ToString() == "")
                {
                    dr["是否加急"] = false;
                }
                if (decimal.Parse(dr["已包装数量"].ToString()) > 0)
                {
                    decimal a = decimal.Parse(dr["数量"].ToString()) - decimal.Parse(dr["已包装数量"].ToString());
                    dr["未包装数"] = a;
                }
                else
                {
                    dr["未包装数"] = dr["数量"];
                }


                bool cun = true;
                if (bool.Parse(dr["包装确认"].ToString()) == true && bool.Parse(dr["包装抽检"].ToString()) == true && bool.Parse(dr["物流确认"].ToString()) == false)
                {
                    cun = false;

                }
                if (cun == true)
                {
                    string text = dr["通知单号"].ToString();
                    string typeName = text.Substring(text.Length - 8, 8);
                    dr["通知单号"] = typeName;
                    dt_z.ImportRow(dr);
                }
            }

            dt_z.DefaultView.Sort = "通知单号";//按Id倒序
                                           //dt_z.DefaultView.Sort = "Id DESC,Name desc";//按Id倒序和Name倒序
            DataTable sdas = dt_z.DefaultView.ToTable();//返回一个新的DataTable


            // dt_显示.AcceptChanges();
            gridControl1.DataSource = sdas;


            DataView dv = dt_z.DefaultView;
            DataTable DistTable = dv.ToTable("Dist", true, "通知单号");
            int 总订单数 = DistTable.Rows.Count;
            label1.Text = 总订单数.ToString();



            int i_未完成量 = dt_z.Rows.Count;
            label8.Text = i_未完成量.ToString();




            sql = string.Format(@"   select  a.作废,a.审核,a.审核日期,b.*  from  销售记录销售出库通知单主表 a 
                    left  join  销售记录销售出库通知单明细表 b   on  a.出库通知单号 = b.出库通知单号  where a.出库日期>='{0}' and a.出库日期<='{1}' 
                 and a.审核=1 and a.作废=0   and b.物料名称<>'维修劳务'  ", t1.ToString("yyyy-MM-dd"), DateTime.Parse(t2).AddDays(1).AddSeconds(-1));

            DataTable dt_zong = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            dt_zong = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            sql = string.Format(@"  select a.* ,b.申请人,b.包装确认 from  借还申请表附表 a
                 left join  借还申请表 b on a.申请批号 = b.申请批号  where b.审核=1 and b.作废=0  
                and a.申请日期>'{0}' and a.申请日期<='{1}'   ", t1.ToString("yyyy-MM-dd"), DateTime.Parse(t2).AddDays(1).AddSeconds(-1));

            DataTable dt_zong_借出 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            dt_zong_借出 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            int 当日需包装数 = dt_zong.Rows.Count + dt_zong_借出.Rows.Count;
            label5.Text = 当日需包装数.ToString();





            sql = string.Format(@"   select  a.作废,a.审核,a.审核日期,b.*  from  销售记录销售出库通知单主表 a 
                    left  join  销售记录销售出库通知单明细表 b   on  a.出库通知单号 = b.出库通知单号  where a.出库日期>='{0}' 
                  and a.出库日期<='{1}'  and a.审核=1 and a.作废=0  and a.物流确认=0 ", t1.ToString("yyyy-MM-dd"), DateTime.Parse(t2).AddDays(1).AddSeconds(-1));

            DataTable data = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            sql = string.Format(@"  select a.* ,b.申请人,b.包装确认 from  借还申请表附表 a
              left join  借还申请表 b on a.申请批号 = b.申请批号  where b.审核=1 and b.作废=0  
                and a.申请日期>'{0}' and a.申请日期<='{1}'  and a.物流确认=0   ", t1.ToString("yyyy-MM-dd"), DateTime.Parse(t2).AddDays(1).AddSeconds(-1));

            DataTable data_借出 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);


            int 当日未包装数 = data.Rows.Count + data_借出.Rows.Count;
            label9.Text = 当日未包装数.ToString();










            sql = string.Format(@"   select  a.作废,a.审核,a.审核日期,b.*  from  销售记录销售出库通知单主表 a 
                    left  join  销售记录销售出库通知单明细表 b   on  a.出库通知单号 = b.出库通知单号  where a.物流确认日期>='{0}' and a.物流确认日期<='{1}'  and a.审核=1 and a.作废=0 and  b.物流确认=1   ", t1.ToString("yyyy-MM-dd"), DateTime.Parse(t2).AddDays(1).AddSeconds(-1));
            DataTable dt_k = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            sql = string.Format(@"  select a.* ,b.申请人,b.包装确认 from  借还申请表附表 a
 left join  借还申请表 b on a.申请批号 = b.申请批号  where b.审核=1 and b.作废=0  and a.物流确认日期>'{0}' and a.物流确认日期<'{1}'and b.物流确认=1   ", t1.ToString("yyyy-MM-dd"), DateTime.Parse(t2).AddDays(1).AddSeconds(-1));
            DataTable dt__借出 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            int i_完成量 = dt_k.Rows.Count + dt__借出.Rows.Count;
            label7.Text = i_完成量.ToString();



            sql = string.Format(@"   select  a.作废,a.审核,a.审核日期,b.*  from  销售记录销售出库通知单主表 a 
                   left  join  销售记录销售出库通知单明细表 b   on  a.出库通知单号 = b.出库通知单号   where  a.审核=1 and a.作废=0 and  b.包装确认=1 and b.包装抽检=0   and b.物流确认=0  and b.物料名称<>'维修劳务' and b.物料名称<>'维护劳务'   ");

            DataTable dt_cj = CZMaster.MasterSQL.Get_DataTable(sql, strcon);


            sql = string.Format(@"    select a.* ,b.申请人,b.包装确认 from  借还申请表附表 a
 left join  借还申请表 b on a.申请批号 = b.申请批号  where b.审核=1 and b.作废=0  and b.包装确认=1  and b.包装抽检=0 and  b.物流确认=0 ");

            DataTable dt_cj借出 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            int i_等待检验量 = dt_cj.Rows.Count + dt_cj借出.Rows.Count;
            label13.Text = i_等待检验量.ToString();


            //double i_完成率 = 0;
            //if (decimal.Parse(i_完成量.ToString()) == 0 || decimal.Parse(i_包装总数.ToString()) == 0)
            //{

            //}
            //else
            //{
            //    i_完成率 = i_完成量 / i_包装总数;
            //    label10.Text = i_完成率.ToString("0.00") + "%";
            //}
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            fun_load();

        }






        private void ui电气发货看板_Load(object sender, EventArgs e)
        {
            try

            {
                fun_load();

                timer1.Start();
                timer2.Start();
                timer3.Start();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {



                label12.Text = System.DateTime.Today.ToLongDateString() + "  " + System.DateTime.Now.ToLongTimeString();

            }
            catch (Exception)
            {


            }

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (gridView1.IsLastRow)
            {
                gridView1.MoveFirst();
            }
            else
            {
                gridView1.MoveNextPage();
            }

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (gridView1.GetRow(e.RowHandle) == null)
                {
                    return;
                }
                //if (Convert.ToBoolean(gridView1.GetRowCellValue(e.RowHandle, "是否加急")))
                //{
                //    e.Appearance.BackColor = Color.Pink;
                //}

                if (bool.Parse(gridView1.GetRowCellValue(e.RowHandle, "是否加急").ToString()) == true)
                {
                    e.Appearance.BackColor = Color.Pink;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //19-10-23 
        private void gridView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.D)
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
        }
    }
}
