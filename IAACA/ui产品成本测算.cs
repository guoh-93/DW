using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace IAACA
{
    public partial class ui产品成本测算 : UserControl
    {
        string cfgfilepath = "";
        DataTable t_bom;
        DataTable t_kc;
        DataTable dtM;
        DataTable dt_AddInv;
        string strcon = CPublic.Var.strConn;

        public ui产品成本测算()
        {
            InitializeComponent();
        }

      
        private void ui产品成本测算_Load(object sender, EventArgs e)
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg pz = new ERPorg.Corg();
                pz.UserLayout(this.splitContainer1, this.Name, cfgfilepath);
                fun_load();
                fun_calu();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        private void fun_load()
        {
            DateTime time = CPublic.Var.getDatetime().Date;
            string s = @"select   a.产品编码,a.子项编码,base.规格型号,base.物料名称,base.存货分类,base.自制,base.可购,base.委外,a.数量  from  基础数据物料BOM表 a 
            left join 基础数据物料信息表 base on a.子项编码=base.物料编码 where 优先级=1 ";
            t_bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = $@"with t as ( select  top 1 *  from 仓库月出入库结转表 order by 结算日期 desc)
            select  base.物料名称,base.规格型号,存货分类,kc.*,结存单价  from 基础数据物料信息表 base
            left join (select 物料编码, sum(库存总数)库存总数,MAX(受订量) 受订量,MAX(在制量)在制量,max(未领量)未领量,max(在途量) as 在途量  from 仓库物料数量表
            where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 = '仓库类别' and 布尔字段2 = 1) group by 物料编码)kc on kc.物料编码=base.物料编码
            left join  (select  jz.物料编码,jz.结存单价 from  仓库月出入库结转表 jz  
                        inner join t on jz.年=t.年 and jz.月=t.月) b
            on base.物料编码=b.物料编码 ";
            t_kc = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            dt_AddInv = new DataTable();
            dt_AddInv.Columns.Add("物料编码");
            dt_AddInv.Columns.Add("规格型号");
            dt_AddInv.Columns.Add("物料名称");
            dt_AddInv.Columns.Add("存货分类");
            dt_AddInv.Columns.Add("数量", typeof(decimal));
            dt_AddInv.Columns.Add("编号", typeof(decimal));
            gridControl2.DataSource = dt_AddInv;

            dtM = new DataTable();
            dtM.Columns.Add("产品编码");
            dtM.Columns.Add("物料编码");
            dtM.Columns.Add("规格型号");
            dtM.Columns.Add("物料名称");
            dtM.Columns.Add("存货分类");
            dtM.Columns.Add("数量", typeof(decimal));
            dtM.Columns.Add("编号", typeof(decimal));
            dtM.Columns.Add("结存单价", typeof(decimal));
            dtM.Columns.Add("库存总数", typeof(decimal));
            dtM.Columns.Add("金额", typeof(decimal));
        }

        //计算
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                dtM = new DataTable();
                dtM.Columns.Add("产品编码");
                dtM.Columns.Add("物料编码");
                dtM.Columns.Add("规格型号");
                dtM.Columns.Add("物料名称");
                dtM.Columns.Add("存货分类");
                dtM.Columns.Add("数量", typeof(decimal));
                dtM.Columns.Add("编号", typeof(decimal));
                dtM.Columns.Add("结存单价", typeof(decimal));
                dtM.Columns.Add("库存总数", typeof(decimal));
                dtM.Columns.Add("金额", typeof(decimal));
                dtM.Columns.Add("委外", typeof(Boolean));

                fun_calu();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void fun_calu()
        {
            foreach (DataRow dr in dt_AddInv.Rows)
            {
                dg_c(dr["物料编码"].ToString(),dr["物料编码"].ToString(), 1, dr["编号"].ToString());
            }
            gridControl1.DataSource = dtM;
        }

        private void dg_c(string str_cpbm, string s_物料, decimal dec, string bh)
        {
            DataRow[] r = t_bom.Select($"产品编码='{s_物料}'");
            if (r.Length > 0)
            {
                foreach (DataRow rr in r)
                {
                    //20-5-31 委外的不要继续往下算 标记出来
                    if (Convert.ToBoolean(rr["自制"]) &&  !Convert.ToBoolean(rr["委外"]))
                    {
                        dg_c(str_cpbm, rr["子项编码"].ToString(), Convert.ToDecimal(rr["数量"]) * dec, bh);
                    }
                    else
                    {
                        
                        DataRow r_add = dtM.NewRow();
                        //20-5-31
                        r_add["委外"] = Convert.ToBoolean(rr["委外"]);

                        r_add["产品编码"] = str_cpbm;
                        r_add["物料编码"] = rr["子项编码"].ToString();
                        r_add["规格型号"] = rr["规格型号"].ToString();
                        r_add["物料名称"] = rr["物料名称"].ToString();
                        r_add["存货分类"] = rr["存货分类"].ToString();
                        r_add["编号"] = bh;
                        r_add["数量"] = Convert.ToDecimal(rr["数量"]) * dec;

                        DataRow[] kc_r = t_kc.Select($"物料编码='{ rr["子项编码"].ToString()}'");
                        if (kc_r.Length > 0)
                        {
                            r_add["库存总数"] = kc_r[0]["库存总数"];
                            r_add["结存单价"] = kc_r[0]["结存单价"];
                            decimal xx = 0;
                            if(decimal.TryParse(kc_r[0]["结存单价"].ToString(),out xx))
                            {
                                r_add["金额"] = xx * Convert.ToDecimal(r_add["数量"]);
                            }
                            
                        }
                        dtM.Rows.Add(r_add);
                    }
                }
            }
            else
            {

                DataRow[] kc_r = t_kc.Select($"物料编码='{s_物料}'");
                if (kc_r.Length > 0)
                {
                    DataRow r_add = dtM.NewRow();
                    r_add["产品编码"] = str_cpbm;
                    r_add["物料编码"] = s_物料;
                    r_add["规格型号"] = kc_r[0]["规格型号"].ToString();
                    r_add["物料名称"] = kc_r[0]["物料名称"].ToString();
                    r_add["存货分类"] = kc_r[0]["存货分类"].ToString();
                    r_add["编号"] = bh;
                    r_add["数量"] = dec;
                    r_add["库存总数"] = kc_r[0]["库存总数"];
                    r_add["结存单价"] = kc_r[0]["结存单价"];
                    dtM.Rows.Add(r_add);
                }
              
            }
        }

        private void infolink()
        {
            //DateTime t = CPublic.Var.getDatetime().Date.AddDays(1);
            int x = 1;
            foreach (DataRow dr in dt_AddInv.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                try
                {
                    DataRow[] r = t_kc.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    dr["编号"] = x++;
                    dr["物料名称"] = r[0]["物料名称"].ToString();
                    dr["规格型号"] = r[0]["规格型号"].ToString();
                    dr["存货分类"] = r[0]["存货分类"].ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void gridView2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.V)
                {
                    if (gridView2.FocusedColumn.Caption == "物料编码") infolink();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                ui产品成本测算 ui = new ui产品成本测算();
                CPublic.UIcontrol.Showpage(ui, "产品材料成本测算");

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
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
