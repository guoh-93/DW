using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using DevExpress.XtraPrinting;
using System.IO;
namespace ERPStock
{
    public partial class frm用友现存量 : UserControl
    {
        public frm用友现存量()
        {
            InitializeComponent();
        }
        #region 成员
        string strcon = CPublic.Var.strConn;
        DataTable dtm_zong;

        string PrinterName = "";
        string cfgfilepath = "";
        DataTable dt;
        DataTable dt_仓库;

        #endregion
        private void frm用友现存量_Load(object sender, EventArgs e)
        {
            string s = "exec sync_u8_stockmx "; //基础数据
            CZMaster.MasterSQL.ExecuteSQL(s, strcon);

            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                if (File.Exists(cfgfilepath + string.Format(@"\{0}.xml", this.Name)))
                {

                    gridView1.RestoreLayoutFromXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }
                fun_load();
            }
            catch (Exception)
            {

                throw;
            }

 
        }
        string 状态="1";
        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr_当前行 = gridView1.GetDataRow(gridView1.FocusedRowHandle);

            ERPStock.仓库物料数量货架修改 fm = new 仓库物料数量货架修改(dr_当前行, 状态);

            fm.ShowDialog();

            if (fm.fl)
            {
                dr_当前行["货架描述"] = fm.hjms;
            }

        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
               // fun_check_dy();

                if (MessageBox.Show(string.Format("确定打印？"), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                    this.printDialog1.Document = this.printDocument1;
                   DialogResult dr = this.printDialog1.ShowDialog();
                  
                       PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                       Dowork();
                        //LabelPrint.LPrinter lp=new LabelPrint.LPrinter(path, dic, str_打印机, 1)
                   // }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }


        //private void fun_打印()
        //{

        //    Thread thDo;
        //    thDo = new Thread(Dowork);
        //    thDo.IsBackground = true;
        //    thDo.Start();

        //}
        private void fun_load()
        {
            //            string sql = @"select 仓库物料数量表.* ,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.仓库名称,
            //              标准单价,n销售单价,基础数据物料信息表.物料类型,基础数据物料信息表.大类,基础数据物料信息表.小类,基础数据物料信息表.货架描述 
            //                            from  仓库物料数量表 left join 基础数据物料信息表 on  仓库物料数量表.物料编码=基础数据物料信息表.物料编码";

            //            //string sql = "select *,基础数据物料信息表.n原ERP规格型号 as 规格型号  from 仓库物料数量表  left join 基础数据物料信息表  on 仓库物料数量表.物料编码 = 基础数据物料信息表.物料编码  ";
            //            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            //            {
            //                dtM = new DataTable();

            //                da.Fill(dtM);
            //                gridControl1.DataSource = dtM;
            //            }
            string sql_仓库 = "SELECT [属性值] as 仓库名称,属性字段1 as 仓库号 FROM [基础数据基础属性表] where 属性类别 ='仓库类别'";
            dt_仓库 = new DataTable();
            dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_仓库, strcon);
            searchLookUpEdit2.Properties.DataSource = dt_仓库;
            searchLookUpEdit2.Properties.ValueMember = "仓库号";
            searchLookUpEdit2.Properties.DisplayMember = "仓库名称";
            string sql4 = "select 物料编码,物料名称,规格型号 from 基础数据物料信息表 where 停用=0";
            DataTable dt_物料 = new DataTable();
            SqlDataAdapter da_物料 = new SqlDataAdapter(sql4, strcon);
            da_物料.Fill(dt_物料);
            searchLookUpEdit1.Properties.DataSource = dt_物料;
            searchLookUpEdit1.Properties.ValueMember = "物料编码";
            searchLookUpEdit1.Properties.DisplayMember = "物料编码";

            string sql = string.Format(@"select * from 人事基础员工表 where 课室='物管课'  and right(班组,1)='库' 
                        and 在职状态 ='在职'  and 员工号='{0}'", CPublic.Var.LocalUserID);
            DataTable t_c = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            if (t_c.Rows.Count > 0 || CPublic.Var.LocalUserID == "admin")
            {
                //label1.Visible = true;
               // checkBox4.Visible = true;
               // checkBox5.Visible = true;
                barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }
            else
            {
              //  label1.Visible = false;

               // checkBox4.Visible = false;
               // checkBox5.Visible = false;
                barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            }

        }
        public void Dowork()
        {
            DataTable dtx = dt.Clone();
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                dtx.ImportRow(gridView1.GetDataRow(i));
            }

            DataView dv = new DataView(dtx);
            dv.Sort = "货架描述";
            DataTable dt_dy = dv.ToTable();
            List<Dictionary<string, string>> li = new List<Dictionary<string, string>>();
            foreach (DataRow drr in dt_dy.Rows)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
               
               
                //string sql = string.Format("select 规格型号,物料名称, 物料等级 from 基础数据物料信息表 where  物料编码='{0}'", drr["物料编码"]);
                //DataRow drj = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
                dic.Add("wldj", drr["物料等级"].ToString());
                dic.Add("itemid", drr["ItemId"].ToString());
               // dic.Add("hjh", drr["货架描述"].ToString());
                dic.Add("wlmc", drr["物料名称"].ToString());
                dic.Add("wlbh", drr["物料编码"].ToString().Trim());
                dic.Add("ggxh", drr["规格型号"].ToString().Trim());
                dic.Add("ckmc", drr["仓库名称"].ToString().Trim());
                dic.Add("ckh", drr["仓库号"].ToString().Trim());
                dic.Add("free1", drr["备注1"].ToString().Trim());
                dic.Add("free2", drr["备注2"].ToString().Trim());
                li.Add(dic);
                //string path = "";
                //if (checkBox4.Checked == true)
                //{

                //      path = Application.StartupPath + string.Format(@"\Mode\货架打印.lab");
                //}
                //else
                //{
                //      path = Application.StartupPath + string.Format(@"\Mode\货架打印小.lab");
                //}
                //LabelPrint.LPrinter lp = new LabelPrint.LPrinter(path, dic, PrinterName, 1);
                //lp.DoWork();
            }
            string path =Application.StartupPath + string.Format(@"\Mode\temp货架.lab");                   
            LabelPrint.LPrinter lp = new LabelPrint.LPrinter(path, li, PrinterName, 1);
            lp.DoWork();
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
        private void fun_check()
        {

            if (checkBox1.Checked == true)
            {
                if (searchLookUpEdit2.EditValue == null || searchLookUpEdit2.EditValue.ToString() == "")
                {
                    throw new Exception("未选择仓库");
                }

            }

            if (checkBox2.Checked == true)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择物料");
                }


            }


            if (checkBox3.Checked == true)
            {
                if (textBox1.Text == "")
                {
                    throw new Exception("未选择货架");
                }

            }
        }

        private void fun_search()
        {

            string sql = string.Format(@"select a.* ,b.物料名称,b.规格型号,b.存货分类,b.物料等级 from   仓库物料数量明细表 a     left join  基础数据物料信息表 b  on a.物料编码 = b.物料编码
                       where 1=1 ");

            if (checkBox2.Checked == true)
            {
                sql = sql + string.Format("and  a.物料编码='{0}'", searchLookUpEdit1.EditValue.ToString());
            }

            if (checkBox1.Checked == true)
            {
                sql = sql + string.Format("and  a.仓库号='{0}'", searchLookUpEdit2.EditValue.ToString());
            }
            if (checkBox3.Checked == true)
            {
                sql = sql + string.Format("and a.货架描述 like'%{0}%'", textBox1.Text.ToString());
            }
   



            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                dt = new DataTable();

                //dt = WSAdapter.webservers_getdata.wsfun.GetData_ERP(sql);
                da.Fill(dt);
            }
            gridControl1.DataSource = dt;
        }

        private void gridView1_ColumnPositionChanged(object sender, EventArgs e)
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gridView1.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }

        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                gridView1.CloseEditor();
                this.BindingContext[dt].EndCurrentEdit();

            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem4_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }


    }
}
