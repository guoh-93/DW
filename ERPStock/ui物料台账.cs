using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
namespace ERPStock
{
    public partial class ui物料台账 : UserControl
    {

        string strcon = CPublic.Var.strConn;

        DataTable dtM = new DataTable();
        public ui物料台账()
        {
            InitializeComponent();
        }
        private void ui物料台账_Load(object sender, EventArgs e)
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime().Date;
                dateEdit2.EditValue = t;
                dateEdit1.EditValue = t.AddMonths(-3);
                load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); 
            }
        }
        private void check()
        {
            DateTime t1 = Convert.ToDateTime(dateEdit1.EditValue).Date;
            DateTime t2 = Convert.ToDateTime(dateEdit2.EditValue).Date.AddDays(1).AddSeconds(-1);
            if (t1 > t2) throw new Exception("时间区间选择有误,起始时间大于终止时间");
            if (checkBox2.Checked)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                    throw new Exception("仓库未选择");
            }
            if (searchLookUpEdit5.EditValue == null || searchLookUpEdit5.EditValue.ToString() == "")
                throw new Exception("物料编码未选择");
        }
        string cfgfilepath = "";
        private void load()
        {
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(this.panel2, this.Name, cfgfilepath);
            string sql_仓库 = "SELECT [属性值] as 仓库名称,属性字段1 as 仓库号 FROM  [基础数据基础属性表] where 属性类别 ='仓库类别'";
            DataTable dt_仓库 = new DataTable();
            SqlDataAdapter da_仓库 = new SqlDataAdapter(sql_仓库, strcon);
            da_仓库.Fill(dt_仓库);
            searchLookUpEdit1.Properties.DataSource = dt_仓库;
            searchLookUpEdit1.Properties.ValueMember = "仓库号";
            searchLookUpEdit1.Properties.DisplayMember = "仓库名称";
            string sql4 = "select 物料编码,规格型号,物料名称 from 基础数据物料信息表 where 停用=0";
            DataTable dt_物料 = new DataTable();
            SqlDataAdapter da_物料 = new SqlDataAdapter(sql4, strcon);
            da_物料.Fill(dt_物料);
            searchLookUpEdit5.Properties.DataSource = dt_物料;
            searchLookUpEdit5.Properties.ValueMember = "物料编码";
            searchLookUpEdit5.Properties.DisplayMember = "物料编码";
        }
        // 
        private void search(DateTime t1, DateTime t2, string str_物料编码)
        {
            string s = $@" select 明细类型,出库入库,明细号,相关单号,物料类型,a.仓库名称,a.仓库号,a.仓库人,实效数量,出入库时间,b.规格型号,产品线,a.物料名称  
   ,a.相关单位,大类,小类,a.物料编码  ,b.计量单位,gd.物料编码 as 产品编码,gd.物料名称 as 产品名称,出库通知单明细号,原因分类  from  仓库出入库明细表 a with (NOLOCK)
   left join 基础数据物料信息表 b   on  a.物料编码=b.物料编码   
   left join 销售记录成品出库单明细表 sa with (NOLOCK) on sa.成品出库单明细号=明细号 
   left join 生产记录生产工单表 gd  with (NOLOCK) on gd.生产工单号=相关单号
   left join 其他出入库申请主表 qtm with (NOLOCK) on qtm.出入库申请单号 =相关单号 
                                    where a.物料编码='{str_物料编码}' and  a.出入库时间>='{t1}' 
                                    and a.出入库时间< '{t2}' order by 出入库时间 desc,出库入库 asc";
            DataTable t_明细 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            dtM = new DataTable();
            dtM = t_明细.Clone();
            dtM.Columns.Add("时点库存", typeof(decimal));

            string sql_仓库 = "";
            if (checkBox2.Checked) sql_仓库 = $" and aa.仓库号='{searchLookUpEdit1.EditValue.ToString()}'";
            s = $@"select aa.物料编码,aa.仓库号,aa.仓库名称,(aa.库存总数-isnull(xx.出入数量,0))库存总数 from 仓库物料数量表 aa  
          left join (select 物料编码,SUM(实效数量) as 出入数量, 仓库号 from 仓库出入库明细表 with (NOLOCK)  where 出入库时间 > '{t2}' group by 物料编码, 仓库号) xx
          on xx.物料编码 = aa.物料编码 and xx.仓库号 = aa.仓库号  where  aa.物料编码='{str_物料编码}' {sql_仓库} "; //取截止到时间点的库存
            DataTable t_库存 = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            foreach (DataRow dr in t_明细.Rows)
            {
                DataRow rr = dtM.NewRow();
                rr.ItemArray = dr.ItemArray;
                DataRow[] tr = t_库存.Select($"仓库号='{dr["仓库号"].ToString()}'");
                decimal dec = 0;
                if (tr.Length > 0)
                {
                    dec = Convert.ToDecimal(tr[0]["库存总数"]);
                    //先赋值后减 
                    tr[0]["库存总数"] = dec - Convert.ToDecimal(dr["实效数量"]);
                }
                rr["时点库存"] = dec;
                dtM.Rows.Add(rr);
            }
            gridControl1.DataSource = dtM;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                check();
                DateTime t1 = Convert.ToDateTime(dateEdit1.EditValue).Date;
                DateTime t2 = Convert.ToDateTime(dateEdit2.EditValue).Date.AddDays(1).AddSeconds(-1);
                search(t1, t2, searchLookUpEdit5.EditValue.ToString());
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

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
    }
}
