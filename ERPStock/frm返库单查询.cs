using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace ERPStock
{
    public partial class frm返库单查询 : UserControl
    {

        DataTable dt_车间;
        DataTable dtM;
        string strcon = CPublic.Var.strConn;
        string cfgfilepath = "";
        public frm返库单查询()
        {
            InitializeComponent();
            DateTime t = CPublic.Var.getDatetime().Date;
            barEditItem1.EditValue = t.AddDays(-15);
            barEditItem2.EditValue = t;

        }

        private void frm返库单查询_Load(object sender, EventArgs e)
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(this.splitContainer1, this.Name, cfgfilepath);
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //关闭
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
        private void fun_load()
        {
            //dt_车间 = new DataTable();
            //dt_车间 = CZMaster.MasterSQL.Get_DataTable("生产", strcon);
            //if (dt_车间.Rows.Count > 0)
            //{
            dtM = new DataTable();
            DateTime dtm1 = Convert.ToDateTime(barEditItem1.EditValue);
            DateTime dtm2 = Convert.ToDateTime(barEditItem2.EditValue).AddDays(1);

            string sql = string.Format(@"select a.*,b.物料名称,b.规格型号  from 工单返库单主表 a 
            left join 基础数据物料信息表 b on a.产品编码 = b.物料编码  where  日期 >'{0}' and 日期<'{1}'", dtm1, dtm2);
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dtM;
            //}

        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {

            try

            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string sql = string.Format(@"select  退料明细号,a.物料编码,b.物料名称,b.规格型号,返库数量,c.仓库号,c.仓库名称,ISNULL(d.货架描述,'')货架描述  from 工单返库单明细表 a 
                                left join 基础数据物料信息表 b on  a.物料编码=b.物料编码 
                                left join 仓库出入库明细表 c on  a.退料明细号=c.明细号 
                                left join 仓库物料数量表 d on c.物料编码=d.物料编码 and c.仓库号=d.仓库号
                                  where 退料单号='{0}'", dr["退料单号"]);
                DataTable dt = new DataTable();
                dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                gridControl2.DataSource = dt;
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




        }

        
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try

            {
                DataRow drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
                DataTable dtm = (DataTable)this.gridControl2.DataSource;
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
                Type outerForm = outerAsm.GetType("ERPreport.返库单", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
                                                                           //    CPublic.UIcontrol.Showpage(ui, t.Rows[0]["打开界面名称"].ToString());
                object[] drr = new object[2];

                drr[0] = drM;
                drr[1] = dtm;
                //   drr[2] = dr["出入库申请单号"].ToString();
                Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                //  UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
                ui.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }





        }
        //查看工单信息
        private void 查看工单信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow r = gridView1.GetDataRow(gridView1 .FocusedRowHandle);
            ERPproduct.ui工单查看跳转 ui = new ERPproduct.ui工单查看跳转(r["生产工单号"].ToString());
            CPublic.UIcontrol.Showpage(ui,"工单信息查询");

        }
    }
}
