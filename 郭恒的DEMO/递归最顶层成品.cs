using System;
using System.Data;
using System.Windows.Forms;
using System.Threading;
namespace 郭恒的DEMO
{
    public partial class 递归最顶层成品 : Form
    {
        DataTable dtP;
        DataTable dtM;
        string strcon = CPublic.Var.strConn;


        public 递归最顶层成品()
        {
            InitializeComponent();
        }


        private void fun_load_半成品()
        {
            string sql = string.Format("select * from [半成品-{0}]", textBox1.Text);
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            dtP = new DataTable();
            dtP.Columns.Add("半成品编号");
            dtP.Columns.Add("顶级父项编号");
            //dtP.Columns.Add("产品线");
            //dtP.Columns.Add("大类");
            dtP.Columns.Add("物料名称");
            dtP.Columns.Add("n原ERP规格型号");
            dtP.Columns.Add("图纸编号");
            dtP.Columns.Add("大类");
            dtP.Columns.Add("小类");
            dtP.Columns.Add("n核算单价");
            dtP.Columns.Add("父项产品线");
            dtP.Columns.Add("父项大类");
            dtP.Columns.Add("父项编号");
            dtP.Columns.Add("父项名称");
            dtP.Columns.Add("父项小类");
            dtP.Columns.Add("父项单价");
            dtP.Columns.Add("父项规格");


        }

        private void fun_load_原材料()
        {
            string sql = string.Format("select * from [原材料-{0}]", textBox1.Text);
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            dtP = new DataTable();
            dtP.Columns.Add("原材料编号");
            dtP.Columns.Add("物料名称");
            dtP.Columns.Add("n原ERP规格型号");
            dtP.Columns.Add("图纸编号");
            dtP.Columns.Add("大类");
            dtP.Columns.Add("小类");
            dtP.Columns.Add("n核算单价");
            dtP.Columns.Add("父项产品线");
            dtP.Columns.Add("父项大类");
            dtP.Columns.Add("父项编号");
            dtP.Columns.Add("父项名称");
            dtP.Columns.Add("父项小类");
            dtP.Columns.Add("父项单价");
            dtP.Columns.Add("父项规格");

        }
        private void fun_load_all()
        {
            string sql = string.Format("select 原ERP物料编号 as 物料编号  from 基础数据物料信息表 ");
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            dtP = new DataTable();
            dtP.Columns.Add("原材料编号");
            dtP.Columns.Add("物料名称");
            dtP.Columns.Add("n原ERP规格型号");
            dtP.Columns.Add("图纸编号");
            dtP.Columns.Add("大类");
            dtP.Columns.Add("小类");
            dtP.Columns.Add("n核算单价");
            dtP.Columns.Add("父项产品线");
            dtP.Columns.Add("父项大类");
            dtP.Columns.Add("父项编号");
            dtP.Columns.Add("父项名称");
            dtP.Columns.Add("父项小类");
            dtP.Columns.Add("父项单价");
            dtP.Columns.Add("父项规格");

        }
        private void fun_运算_成品(string str_原材料, string str_物料编号)
        {
            DataTable dt = new DataTable();
            //先去 对应表中找 如果有 
            string s = string.Format("select  *  from  物料顶级父项对应关系 where 原材料编号='{0}'", str_物料编号);
            dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            if (dt.Rows.Count == 0)
            {

                s = string.Format(@"select  a.产品编码,c.大类 as 产品大类,c.产品线,c.原ERP物料编号  from 基础数据物料BOM表 a,基础数据物料信息表 b,基础数据物料信息表 c
            where a.子项编码=b.物料编码 and c.物料编码=产品编码 and b.原ERP物料编号 ='{0}'", str_物料编号);
                dt = CZMaster.MasterSQL.Get_DataTable(s, strcon); // 上一级父项 
                if (dt.Rows.Count == 0) //没有父项即为顶层物料
                {
                    //if (str_原材料 != str_物料编号)
                    //{
                    s = string.Format("select   原ERP物料编号,物料名称,产品线,大类,小类,n原ERP规格型号,n核算单价 from 基础数据物料信息表 where 原ERP物料编号='{0}'", str_物料编号);
                    DataTable dt_1 = CZMaster.MasterSQL.Get_DataTable(s, strcon);

                    s = string.Format("select   物料名称,大类,小类,图纸编号,n原ERP规格型号,n核算单价 from 基础数据物料信息表 where 原ERP物料编号='{0}'", str_原材料);
                    DataTable dt_原料 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    if (dt_原料.Rows.Count != 0) //有的物料 现在的 基础表中没有
                    {
                        //if (dtP.Select(string.Format("原材料编号='{0}' and 父项编号 ='{1}'",str_原材料, dt_1.Rows[0]["原ERP物料编号"].ToString())).Length==0)
                        //{
                        DataRow r = dtP.NewRow();
                        r["父项产品线"] = dt_1.Rows[0]["产品线"];
                        r["父项大类"] = dt_1.Rows[0]["大类"];
                        r["父项规格"] = dt_1.Rows[0]["n原ERP规格型号"];
                        r["父项小类"] = dt_1.Rows[0]["小类"];
                        r["父项编号"] = dt_1.Rows[0]["原ERP物料编号"];
                        r["父项名称"] = dt_1.Rows[0]["物料名称"];
                        r["父项单价"] = dt_1.Rows[0]["n核算单价"];
                        r["n原ERP规格型号"] = dt_原料.Rows[0]["n原ERP规格型号"];
                        r["图纸编号"] = dt_原料.Rows[0]["图纸编号"];
                        r["大类"] = dt_原料.Rows[0]["大类"];
                        r["小类"] = dt_原料.Rows[0]["小类"];
                        r["n核算单价"] = dt_原料.Rows[0]["n核算单价"];
                        r["原材料编号"] = str_原材料;
                        r["物料名称"] = dt_原料.Rows[0]["物料名称"];
                        dtP.Rows.Add(r);
                        //}
                    }
                    else
                    {
                        DataRow r = dtP.NewRow();
                        r["原材料编号"] = str_原材料;
                        r["物料名称"] = "现基础信息表中没有该物料";
                        dtP.Rows.Add(r);

                    }


                    //  }
                }
                else  //还有上一层 
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        fun_运算_成品(str_原材料, dr["原ERP物料编号"].ToString());  //传入父项编号 继续寻找顶族

                    }

                }

            }
            else
            {
                foreach (DataRow xr in dt.Rows)
                {
                    //if (dtP.Select(string.Format("原材料编号='{0}' and 父项编号 ='{1}'", str_原材料, xr["父项编号"].ToString())).Length == 0)
                    //{
                    DataRow r = dtP.NewRow();
                    r["父项产品线"] = xr["父项产品线"];
                    r["父项大类"] = xr["父项大类"];
                    r["父项规格"] = xr["父项规格"];
                    r["父项小类"] = xr["小类"];
                    r["父项编号"] = xr["父项编号"];
                    r["父项名称"] = xr["父项名称"];
                    r["父项单价"] = xr["父项单价"];


                    r["n原ERP规格型号"] = xr["n原ERP规格型号"];
                    r["图纸编号"] = xr["图纸编号"];
                    r["大类"] = xr["大类"];
                    r["小类"] = xr["小类"];
                    r["n核算单价"] = xr["n核算单价"];
                    r["原材料编号"] = str_原材料;
                    r["物料名称"] = xr["物料名称"];
                    dtP.Rows.Add(r);
                    //}
                }
            }
        }
        //搜他的顶级父项赋给dtP   
        //两个变量初始一样
        private void fun_运算(string str_原始半成品, string str_物料编号)
        {
            string s = string.Format(@"select  a.产品编码,c.大类 as 产品大类,c.产品线,c.原ERP物料编号  from 基础数据物料BOM表 a,基础数据物料信息表 b,基础数据物料信息表 c
            where a.子项编码=b.物料编码 and c.物料编码=产品编码 and b.原ERP物料编号 ='{0}'", str_物料编号);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon); // 上一级父项 
            if (dt.Rows.Count == 0) //没有父项即为顶层物料
            {

                s = string.Format("select   原ERP物料编号,物料名称,产品线,大类,小类,n原ERP规格型号,n核算单价 from 基础数据物料信息表 where 原ERP物料编号='{0}'", str_物料编号);
                DataTable dt_1 = CZMaster.MasterSQL.Get_DataTable(s, strcon);

                s = string.Format("select   物料名称,大类,小类,图纸编号,n原ERP规格型号,n核算单价 from 基础数据物料信息表 where 原ERP物料编号='{0}'", str_原始半成品);
                DataTable dt_原料 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                if (dt_原料.Rows.Count != 0) //有的物料 现在的 基础表中没有
                {
                    DataRow r = dtP.NewRow();
                    r["半成品编号"] = str_原始半成品;
                    r["n原ERP规格型号"] = dt_原料.Rows[0]["n原ERP规格型号"];
                    r["图纸编号"] = dt_原料.Rows[0]["图纸编号"];
                    r["大类"] = dt_原料.Rows[0]["大类"];
                    r["小类"] = dt_原料.Rows[0]["小类"];
                    r["n核算单价"] = dt_原料.Rows[0]["n核算单价"];

                    r["物料名称"] = dt_原料.Rows[0]["物料名称"];

                    r["顶级父项编号"] = str_物料编号;
                    r["父项产品线"] = dt_1.Rows[0]["产品线"];
                    r["父项大类"] = dt_1.Rows[0]["大类"];
                    r["父项规格"] = dt_1.Rows[0]["n原ERP规格型号"];
                    r["父项小类"] = dt_1.Rows[0]["小类"];
                    r["父项编号"] = dt_1.Rows[0]["原ERP物料编号"];
                    r["父项名称"] = dt_1.Rows[0]["物料名称"];
                    r["父项单价"] = dt_1.Rows[0]["n核算单价"];
                    dtP.Rows.Add(r);
                }
                else
                {
                    DataRow r = dtP.NewRow();
                    r["半成品编号"] = str_原始半成品;
                    r["物料名称"] = "现基础信息表中没有该物料";
                    dtP.Rows.Add(r);

                }
            }
            else  //还有上一层 
            {
                foreach (DataRow dr in dt.Rows)
                {
                    fun_运算(str_原始半成品, dr["原ERP物料编号"].ToString());  //传入父项编号 继续寻找顶级

                } 

            }

        }
        private void button1_Click(object sender, EventArgs e)
        {

            fun_load_半成品();
            int i = 1;
            int j = dtM.Rows.Count;
            foreach (DataRow dr in dtM.Rows)
            {
                string s = dr["物料编号"].ToString();
                fun_运算(s, s);
                i++;
                label2.Text = i.ToString() + "/" + j.ToString();
                Application.DoEvents();
            }
            gridControl1.DataSource = dtP;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                // toExcel(dtP, saveFileDialog.FileName);
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                gridControl1.ExportToXlsx(saveFileDialog.FileName);

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
        //public void toExcel(System.Data.DataTable data, string output)
        //{
        //    try
        //    {
        //        //创建工作薄  
        //        HSSFWorkbook workbook = new HSSFWorkbook();
        //        //创建一个表sheet  
        //        ISheet sheet = workbook.CreateSheet("sheet");
        //        //创建第一行,新创建的表是没有单元格的,每一个需要写入数据的单元格都要手动创建  
        //        IRow row = sheet.CreateRow(0);
        //        //将列名写入表的第一行  
        //        for (int i = 0; i < data.Columns.Count; i++)
        //        {
        //            row.CreateCell(i);
        //            sheet.GetRow(0).GetCell(i).SetCellValue(data.Columns[i].ColumnName);
        //        }
        //        //写入数据  
        //        for (int i = 1; i <= data.Rows.Count; i++)
        //        {
        //            row = sheet.CreateRow(i);
        //            for (int j = 0; j < data.Columns.Count; j++)
        //            {
        //                row.CreateCell(j);
        //                sheet.GetRow(i).GetCell(j).SetCellValue(data.Rows[i - 1][j].ToString());
        //            }
        //        }
        //        FileStream file = new FileStream(output, FileMode.Create);
        //        workbook.Write(file);
        //        file.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}  

        private void button3_Click(object sender, EventArgs e)
        {
            fun_load_原材料();
            int i = 1;
            int j = dtM.Rows.Count;
            foreach (DataRow dr in dtM.Rows)
            {
                string s = dr["物料编号"].ToString();
                fun_运算_成品(s, s);
                i++;
                label2.Text = i.ToString() + "/" + j.ToString();
                Application.DoEvents();
            }
            gridControl1.DataSource = dtP;
        }
        //搜他的顶级父项赋给dtP   
        //两个变量初始一样
        private void fun_运算_大类(string str_原材料, string str_物料编号)
        {

            string s = string.Format(@"select  a.产品编码,c.大类 as 产品大类,c.产品线,c.原ERP物料编号  from 基础数据物料BOM表 a,基础数据物料信息表 b,基础数据物料信息表 c
            where a.子项编码=b.物料编码 and c.物料编码=产品编码 and b.原ERP物料编号 ='{0}'", str_物料编号);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon); // 上一级父项 
            if (dt.Rows.Count == 0) //没有父项即为顶层物料
            {
                //if (str_原材料 != str_物料编号)
                //{
                s = string.Format("select   产品线,大类 from 基础数据物料信息表 where 原ERP物料编号='{0}'", str_物料编号);
                DataTable dt_1 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                DataRow[] rr = dtP.Select(string.Format(" 原材料编号='{0}' and  父项产品线='{1}' and 父项大类='{2}'", str_原材料, dt_1.Rows[0]["产品线"], dt_1.Rows[0]["大类"]));
                if (rr.Length == 0)
                {
                    s = string.Format("select   物料名称,大类,小类,图纸编号,n原ERP规格型号,n核算单价 from 基础数据物料信息表 where 原ERP物料编号='{0}'", str_原材料);
                    DataTable dt_原料 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    if (dt.Rows.Count != 0) //有的物料 现在的 基础表中没有
                    {
                        DataRow r = dtP.NewRow();
                        r["父项产品线"] = dt_1.Rows[0]["产品线"];
                        r["父项大类"] = dt_1.Rows[0]["大类"];
                        r["图纸编号"] = dt_原料.Rows[0]["图纸编号"];
                        r["大类"] = dt_原料.Rows[0]["大类"];
                        r["n原ERP规格型号"] = dt_原料.Rows[0]["n原ERP规格型号"];
                        r["小类"] = dt_原料.Rows[0]["小类"];
                        r["n核算单价"] = dt_原料.Rows[0]["n核算单价"];
                        r["原材料编号"] = str_原材料;
                        r["物料名称"] = dt_原料.Rows[0]["物料名称"];
                        dtP.Rows.Add(r);
                    }
                    else
                    {
                        DataRow r = dtP.NewRow();
                        r["原材料编号"] = str_原材料;
                        r["物料名称"] = "现基础信息表中没有该物料";
                        dtP.Rows.Add(r);

                    }

                }
                //  }
            }
            else  //还有上一层 
            {
                foreach (DataRow dr in dt.Rows)
                {
                    fun_运算_大类(str_原材料, dr["原ERP物料编号"].ToString());  //传入父项编号 继续寻找顶族

                }

            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            fun_load_all();
            int i = 1;
            int j = dtM.Rows.Count;
            foreach (DataRow dr in dtM.Rows)
            {
                string s = dr["物料编号"].ToString();
                fun_运算_成品(s, s);
                i++;
                label2.Text = i.ToString() + "/" + j.ToString();
                Application.DoEvents();
            }
            gridControl1.DataSource = dtP;
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dtP = new DataTable();
            dtP.Columns.Add("原材料编号");
            dtP.Columns.Add("物料名称");
            dtP.Columns.Add("n原ERP规格型号");
            dtP.Columns.Add("图纸编号");
            dtP.Columns.Add("大类");
            dtP.Columns.Add("小类");
            dtP.Columns.Add("n核算单价");
            dtP.Columns.Add("父项产品线");
            dtP.Columns.Add("父项大类");
            dtP.Columns.Add("父项编号");
            dtP.Columns.Add("父项名称");
            dtP.Columns.Add("父项小类");
            dtP.Columns.Add("父项单价");
            dtP.Columns.Add("父项规格");
            fun_运算_成品(textBox2.Text, textBox2.Text);
            gridControl1.DataSource = dtP;
        }

        DataTable t_bom = new DataTable();
        private void button6_Click(object sender, EventArgs e)
        {
            Thread th = new Thread(() =>
            {
                string s = @" select   物料编码,物料名称,规格型号 from 基础数据物料信息表 where 可购=1 and 自制=0 and LEFT(物料编码,2) not in('30','11')
     and 物料编码 in (select 子项编码 from 基础数据物料BOM表 group by 子项编码) and 物料编码 not in ('05020202020115','05020202020123') order by 物料编码";
                DataTable t_list = CZMaster.MasterSQL.Get_DataTable(s, strcon);

                s = @"select  产品编码,产品名称,fx.存货分类 as 父项分类,fx.规格型号 as 父项规格,子项编码,子项名称,数量,zx.委外 as 子项委外,zx.自制 as 子项自制,zx.可购 as 子项可购,zx.存货分类 as 子项分类
                ,zx.规格型号 as 子项规格,wiptype from 基础数据物料BOM表 bom
               left join 基础数据物料信息表 zx on bom.子项编码 = zx.物料编码
               left join 基础数据物料信息表 fx on bom.产品编码 = fx.物料编码 ";
                t_bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                DataColumn[] pk_bom = new DataColumn[2];
                pk_bom[0] = t_bom.Columns["产品编码"];
                pk_bom[1] = t_bom.Columns["子项编码"];
                t_bom.PrimaryKey = pk_bom;
                dtM = new DataTable();
                dtM.Columns.Add("原码");
                dtM.Columns.Add("产品编码");
                DataColumn[] pk = new DataColumn[2];
                pk[0] = dtM.Columns["产品编码"];
                pk[1] = dtM.Columns["原码"];
                dtM.PrimaryKey = pk;
                int x = 1;
                int total = t_list.Rows.Count;
                foreach (DataRow r in t_list.Rows)
                {
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        button6.Text = x + "/" + total;

                    }));
                    main_Plan_calu_dg(dtM, r["物料编码"].ToString(), r["物料编码"].ToString());
                    x++;
                }
                BeginInvoke(new MethodInvoker(() =>
                {
                    gridControl2.DataSource = dtM;

                }));
                
            });
            th.IsBackground = true;
            th.Start();
          


         
          
        }


        private void main_Plan_calu_dg(DataTable dtM, string str,string str_y)
        {
            DataRow[] r_PPool = t_bom.Select($"子项编码='{str}'");
            if (r_PPool.Length == 0)
            {
                if (str == str_y) return;
                if (dtM.Select($"原码='{str_y}' and 产品编码='{str}'").Length == 0)
                {
                    DataRow r = dtM.NewRow();
                    r["原码"] = str_y;
                    r["产品编码"] = str;
                    dtM.Rows.Add(r);
                }
            }
            foreach (DataRow rr in r_PPool)
            {
                DataRow[] r_dg = t_bom.Select($"子项编码='{rr["产品编码"]}'");
                if(r_dg.Length==0)
                {
                    if (dtM.Select($"原码='{str_y}' and 产品编码='{rr["产品编码"].ToString()}'").Length == 0)
                    {
                        DataRow r = dtM.NewRow();
                        r["原码"] = str_y;
                        r["产品编码"] = rr["产品编码"].ToString();
                        dtM.Rows.Add(r);
                    }
                }
                else
                {
                    main_Plan_calu_dg(dtM, rr["产品编码"].ToString(), str_y);

                }
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                //gridControl1.ExportToXls(saveFileDialog.FileName, options);  

                gridControl2.ExportToXlsx(saveFileDialog.FileName);
                
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
