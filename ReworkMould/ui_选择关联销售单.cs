using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ReworkMould
{
    public partial class ui_选择关联销售单 : UserControl
    {
        #region 变量

        private DataRow dr_制令明细;
        private string strcon = CPublic.Var.strConn;
        private string cfgfilepath = "";
        public bool flag = false;
        public DataTable dt_xsmx = null;
        private DataTable dt_销售明细;
        private string str_wl = "";

        #endregion 变量

        public ui_选择关联销售单()
        {
            InitializeComponent();
        }

        public ui_选择关联销售单(string s)
        {
            InitializeComponent();
            flag = false;
            str_wl = s;
        }

        private void ui_选择关联销售单_Load(object sender, EventArgs e)
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
                x.UserLayout(panel2, this.Name, cfgfilepath);

                string sql = @"select  x.*,case when zlzb.销售订单明细号 is null then 0 else 1 end as 已关联 from [V_CalPoolTotal] x
          left join(select 销售订单明细号 from 生产记录生产制令子表 group by 销售订单明细号) zlzb on zlzb.销售订单明细号 = x.销售订单明细号";
                dt_销售明细 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                DataColumn dcc = new DataColumn("选择", typeof(bool));
                dcc.DefaultValue = false;
                dt_销售明细.Columns.Add(dcc);

                Thread th = new Thread(() =>
                {
                    DataTable dtz = new DataTable();
                    // dtz.Columns.Add("产品编码");
                    string s = string.Format(@"with parent_bom(产品编码,子项编码,仓库号,仓库名称,bom_level ) as
                   (select  产品编码,子项编码,仓库号,仓库名称,1 as level from 基础数据物料BOM表
                    where 子项编码='{0}'
                      union all
                   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,b.bom_level+1  from 基础数据物料BOM表 a
                   inner join parent_bom b on a.子项编码=b.产品编码  )
                      select  * from parent_bom ", str_wl);
                    dtz = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    // dtz = ERPorg.Corg.fun_GetFather(dtz, dr["物料编码"].ToString(), 0, true);
                    //加入他自身
                    DataRow rrr = dtz.NewRow();
                    rrr["产品编码"] = str_wl;
                    dtz.Rows.Add(rrr);
                    if (dtz.Rows.Count > 0)
                    {
                        //19-8-20
                        s = string.Format("物料编码 in (");
                        foreach (DataRow xx in dtz.Rows)
                        {
                            s = s + "'" + xx["产品编码"].ToString() + "',";
                        }
                        s = s.Substring(0, s.Length - 1) + ")";
                        DataView dv = new DataView(dt_销售明细);
                        dv.RowFilter = s;
                        dt_销售明细 = dv.ToTable();
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            gridControl1.DataSource = dt_销售明细;
                        }));
                    }
                    else
                    {
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            gridControl1.DataSource = dt_销售明细.Clone();
                        }));
                    }
                });
                th.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            flag = false;
            this.ParentForm.Close();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                flag = false;
                gridView1.CloseEditor();
                this.BindingContext[dt_销售明细].EndCurrentEdit();
                DataRow[] dr_mx = dt_销售明细.Select("选择 = true");
                if (dr_mx.Length <= 0) throw new Exception("未勾选明细，请确认");
                DataView dv = new DataView(dt_销售明细);
                dv.RowFilter = "选择 = 1";
                DataTable dt = dv.ToTable();
                dt_xsmx = dt.Copy();
                flag = true;
                this.ParentForm.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); ;
            }
        }
    }
}