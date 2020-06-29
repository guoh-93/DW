using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
namespace ERPpurchase
{
    public partial class ui需求量计算 : UserControl
    {

        string strcon = CPublic.Var.strConn;
        bool bl_calculate = false;
        int flag = 0;
        DataTable dt_import;
        string  cfgfilepath="";
        public ui需求量计算()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (flag == 0)
                {
                    var ofd = new OpenFileDialog();
                    if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        //  dt_SaleOrder = ERPorg.Corg.ExcelXLSX(ofd);
                        bool bl = ERPorg.Corg.IsFileInUse(ofd.FileName);
                        if (bl) throw new Exception("文件已打开或被占用中");

                        Thread th = new Thread(() =>
                        {
                            BeginInvoke(new MethodInvoker(() =>
                            {
                                button1.Enabled = false;
                                button1.Text = "导入中..";
                            }));
                            dt_import = ERPorg.Corg.ReadExcelToDataTable(ofd.FileName);

                            //DateTime t = CPublic.Var.getDatetime().Date;
                            //string fileName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DW导入销售单\\计划";
                            //if (Directory.Exists(fileName) == false)
                            //{
                            //    Directory.CreateDirectory(fileName);
                            //}
                            int x = dt_import.Rows.Count;
                            for (int i = x - 1; i >= 0; i--)
                            {
                                if (dt_import.Rows[i]["物料编码"].ToString().Trim() == "")
                                {
                                    dt_import.Rows.Remove(dt_import.Rows[i]);
                                }
                            }

                            flag = 1;
                            BeginInvoke(new MethodInvoker(() =>
                            {
                                button1.Text = string.Format("明细:{0}条", dt_import.Rows.Count);
                            }));
                        });
                        th.Start();

                    }
                }
                else
                {
                    throw new Exception("请按步骤操作");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {

                //if (!bl_sync)
                //{
                flag = 0;
                button1.Enabled = true;
                button1.Text = "导入";
                button2.Enabled = true;
                button2.Text = "计算";
                gc2.DataSource = null;
                bl_calculate = false;
                //}
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

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (flag < 1) throw new Exception("信息尚未准备完全,请按步骤操作");
                if (bl_calculate) throw new Exception("正在计算中..");


                Thread th = new Thread(cal);
                th.IsBackground = true;
                th.Start();
                bl_calculate = true;
                button2.Text = "正在计算中..";


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// 19-7-1 采购根据生产主计划计算缺料情况
        /// </summary>
        private void cal()
        {
            DataTable t_back = new DataTable();
            t_back.Columns.Add("物料编码");
            t_back.Columns.Add("需求数量",typeof(decimal));
 
            string s = @"select  产品编码,子项编码,数量,自制,委外,可购 from 基础数据物料BOM表 bom 
                         left join 基础数据物料信息表  base  on base.物料编码=bom.子项编码 ";
            DataTable t_bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
             s = @"select  base.物料编码,物料名称,规格型号,存货分类,库存总数 from 基础数据物料信息表 base
    left join (select  物料编码,sum(库存总数) as 库存总数  from   仓库物料数量表 where   
    (仓库号 in (select  属性字段1  from    基础数据基础属性表  where 属性类别 = '仓库类别' and 布尔字段2 = 1))  group by 物料编码)kc 
    on kc.物料编码=base.物料编码   where 停用=0 ";
            DataTable t_base = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            foreach(DataRow dr in dt_import.Rows)
            {
              DataRow []r_bom= t_bom.Select(string.Format("产品编码='{0}'", dr["物料编码"]));
                if(r_bom.Length>0)
                {
                    foreach(DataRow r in r_bom)
                    {
                        if(Convert.ToBoolean(r["自制"]))
                        {
                            decimal dec_需求 = Convert.ToDecimal(r["数量"]) * Convert.ToDecimal(dr["数量"]);

                            t_back = dg_xq(t_back, t_bom, dec_需求, r["子项编码"].ToString());

                        }
                        else if(Convert.ToBoolean(r["委外"]))
                        {
                            decimal dec_需求 = 0;
                            DataRow[] check = t_back.Select(string.Format("物料编码='{0}'", r["子项编码"]));
                            if (check.Length > 0)
                            {
                                check[0]["需求数量"]= dec_需求 = Convert.ToDecimal(check[0]["需求数量"]) + Convert.ToDecimal(r["数量"]) * Convert.ToDecimal(dr["数量"]);
                            }
                            else
                            {
                                DataRow r_add = t_back.NewRow();
                                r_add["物料编码"] = r["子项编码"];
                                r_add["需求数量"] = dec_需求 = Convert.ToDecimal(r["数量"]) * Convert.ToDecimal(dr["数量"]);
                                t_back.Rows.Add(r_add);
                            }
                            
                            t_back = dg_xq(t_back, t_bom, dec_需求, r["子项编码"].ToString());
                        }
                        else if(Convert.ToBoolean(r["可购"]))
                        {
                           DataRow []check=t_back.Select(string.Format("物料编码='{0}'", r["子项编码"]));
                            if (check.Length > 0)
                            {
                                check[0]["需求数量"]= Convert.ToDecimal(check[0]["需求数量"])+ Convert.ToDecimal(r["数量"]) * Convert.ToDecimal(dr["数量"]);
                            }
                            else
                            {
                                DataRow r_add = t_back.NewRow();
                                r_add["物料编码"] = r["子项编码"];
                                r_add["需求数量"] = Convert.ToDecimal( r["数量"])*Convert.ToDecimal(dr["数量"]);
                                t_back.Rows.Add(r_add);
                            }
                        }
                    }

  
                }
                else
                {
                    DataRow[] check = t_back.Select(string.Format("物料编码='{0}'",dr["物料编码"]));
                    if (check.Length > 0)
                    {
                        check[0]["需求数量"] = Convert.ToDecimal(check[0]["需求数量"]) +  Convert.ToDecimal(dr["数量"]);
                    }
                    else
                    {
                        DataRow r_add = t_back.NewRow();
                        r_add["物料编码"] = dr["物料编码"];
                        r_add["需求数量"] =  Convert.ToDecimal(dr["数量"]);
                        t_back.Rows.Add(r_add);
                    }
                    
                }
            }

            t_back.Columns.Add("物料名称");
            t_back.Columns.Add("规格型号");
            t_back.Columns.Add("存货分类");
            t_back.Columns.Add("库存总数",typeof(decimal));
            foreach (DataRow dr in t_back.Rows)
            {
              DataRow []rr =t_base.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                dr["物料名称"] = rr[0]["物料名称"];
                dr["规格型号"] = rr[0]["规格型号"];
                dr["存货分类"] = rr[0]["存货分类"];
                if (rr[0]["库存总数"] == null || rr[0]["库存总数"] == DBNull.Value || rr[0]["库存总数"].ToString() == "")
                {
                    dr["库存总数"] = 0;
                }
                else
                {
                    dr["库存总数"] = rr[0]["库存总数"];
                }
            }

            BeginInvoke(new MethodInvoker(() =>
            {
                gc2.DataSource = t_back;
                bl_calculate = false;
            }));
   


        }

        private DataTable dg_xq(DataTable t_return,DataTable t_bom,decimal dec_需求,string str_物料编码)
        {
            DataRow []rr= t_bom.Select(string.Format("产品编码='{0}'", str_物料编码));

            if (rr.Length > 0)
            {
                foreach (DataRow r in rr)
                {
                    if (Convert.ToBoolean(r["自制"]))
                    {
                        decimal dec_need = Convert.ToDecimal(r["数量"]) * dec_需求;

                        t_return = dg_xq(t_return, t_bom, dec_need, r["子项编码"].ToString());

                    }
                    else if (Convert.ToBoolean(r["委外"]))
                    {
                        decimal dec_need = 0;
                        DataRow[] check = t_return.Select(string.Format("物料编码='{0}'", r["子项编码"]));
                        if (check.Length > 0)
                        {
                            check[0]["需求数量"] = dec_need = Convert.ToDecimal(check[0]["需求数量"]) + Convert.ToDecimal(r["数量"]) * dec_需求;
                        }
                        else
                        {
                            DataRow r_add = t_return.NewRow();
                            r_add["物料编码"] = r["子项编码"];
                            r_add["需求数量"] = dec_need = Convert.ToDecimal(r["数量"]) * dec_需求;
                            t_return.Rows.Add(r_add);
                        }

                        t_return = dg_xq(t_return, t_bom, dec_need, r["子项编码"].ToString());
                    }
                    else if (Convert.ToBoolean(r["可购"]))
                    {
                        DataRow[] check = t_return.Select(string.Format("物料编码='{0}'", r["子项编码"]));
                        if (check.Length > 0)
                        {
                            check[0]["需求数量"] = Convert.ToDecimal(check[0]["需求数量"]) + Convert.ToDecimal(r["数量"]) * dec_需求 ;
                        }
                        else
                        {
                            DataRow r_add = t_return.NewRow();
                            r_add["物料编码"] = r["子项编码"];
                            r_add["需求数量"] = Convert.ToDecimal(r["数量"]) * dec_需求;
                            t_return.Rows.Add(r_add);
                        }
                    }
                }


            }

            return t_return;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                gc2.ExportToXlsx(saveFileDialog.FileName);
                MessageBox.Show("导出成功");
            }
        }

        private void ui需求量计算_Load(object sender, EventArgs e)
        {
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(this.panel2, this.Name, cfgfilepath);

        }
    }
}
