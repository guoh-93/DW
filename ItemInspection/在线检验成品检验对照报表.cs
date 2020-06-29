using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace ItemInspection
{
    public partial class 在线检验成品检验对照报表 : UserControl
    {
        public 在线检验成品检验对照报表()
        {
            InitializeComponent();
        }
        DataTable dtM;
        DataTable dtMM;
        string strcoon = CPublic.Var.strConn;
        string cfgfilepath = "";
        private void 在线检验成品检验对照报表_Load(object sender, EventArgs e)
        {
            DateTime t = CPublic.Var.getDatetime();
            barEditItem1.EditValue = t.AddDays(-1);
            barEditItem2.EditValue = t;


            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout( splitContainer1, this.Name, cfgfilepath);
        }
        //查询
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_加载();
            dtM.Columns.Add("不良原因(在线检)",typeof(string));
            dtM.Columns.Add("不良原因(总检)", typeof(string));
           
        }
       
        private void fun_加载()
        {
            try
            {
                //以前崔晓东做的 拉取得u8工单后再检验得
                //           string str = string.Format(@" select a.生产检验单号 as 在线检验单号,a.物料编码,a.物料名称,a.规格型号,a.检验日期,a.东屋入库单号,a.一次合格率 as 在线一次合格率,a.总计合格率 as 在线总计合格率,
                //b.一次合格率 as 总检一次合格率,b.总计合格率 as 总检总计合格率,b.生产检验单号 as 成品检验单号,b.生产数量,b.送检数量,b.合格数量 from 快速检验生产检验单主表 a 
                //left join 生产记录生产检验单主表 b on a.东屋入库单号 = b.东屋入库单号  where b.东屋入库单号 is not null and a.东屋入库单号 is not null and 
                //a.检验日期>='{0}' and a.检验日期<='{1}'", barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));

                //19-12-19 郭恒  现要求修改
                string sql = string.Format(@"   select  xx.生产工单号,xx.物料编码,base.物料名称,base.规格型号,xx.送检数量 as 在线检数量,xx.合格数量 as 在线一次合格数,xx.重检合格数 as 在线重检合格数,xx.在线一次合格率,xx.在线总合格率,yy.送检数量 as 总检数量
     ,yy.合格数量 as 总检一次合格数,yy.重检合格数 as 总检重检合格数,yy.总检一次合格率,yy.总检总合格率 from (
     select  *,合格数量/送检数量 as 在线一次合格率, (合格数量+重检合格数)/送检数量 as 在线总合格率 from (
     select  生产工单号,物料编码,sum(送检数量)送检数量,sum(合格数量)合格数量,SUM(重检合格数)重检合格数  from 快速检验生产检验单主表
     where  检验日期>='{0}' and  检验日期<='{1}'  group by  生产工单号,物料编码) zxj )xx 
     left join (     
     select  *,合格数量/送检数量 as 总检一次合格率, (合格数量+重检合格数)/送检数量 as 总检总合格率 from (
     select  生产工单号,物料编码,sum(送检数量)送检数量,sum(合格数量)合格数量,SUM(重检合格数)重检合格数  from 生产记录生产检验单主表 
      group by  生产工单号,物料编码) zj ) yy on yy.生产工单号=xx.生产工单号 
       left join 基础数据物料信息表 base on base.物料编码=xx.物料编码  ", Convert.ToDateTime(barEditItem1.EditValue).Date, Convert.ToDateTime(barEditItem2.EditValue).Date.AddDays(1).AddSeconds(-1));

                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcoon))
                {
                    dtM = new DataTable();
                    da.Fill(dtM);

                }
                gc.DataSource = dtM;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
         }

        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                using (SqlDataAdapter da = new SqlDataAdapter(@"select a.*,b.生产工单号  from 成品检验检验记录返工表  a
            left join 生产记录生产检验单主表 b  on a.生产检验单号 = b.生产检验单号 where 生产工单号='" + dr["生产工单号"] + "'", strcoon))
                {
                    DataTable dt_返工 = new DataTable();
                    da.Fill(dt_返工);
                    gc1.DataSource = dt_返工;

                }

                using (SqlDataAdapter da = new SqlDataAdapter(@" select a.*,b.生产工单号 from 快速检验检验记录返工表 a
            left join 快速检验生产检验单主表 b  on a.生产检验单号 = b.生产检验单号 where 生产工单号='" + dr["生产工单号"] + "'", strcoon))
                {
                    DataTable dt_不良= new DataTable();
                    da.Fill(dt_不良);
                    gridControl1.DataSource = dt_不良;

                }






            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        //导出
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //fun_处理数据();
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                    options.ExportMode = DevExpress.XtraPrinting.XlsxExportMode.SingleFile;
                    DataTable dtm_copy = dtM.Copy();
                    //dtm_copy.Columns.Remove("成品检验单号");
                    //dtm_copy.Columns.Remove("在线检验单号");
                    gc.ExportToXlsx(saveFileDialog.FileName,options);
                   // ERPorg.Corg.TableToExcel(dtm_copy, saveFileDialog.FileName);


                    DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void fun_处理数据()
        { 
          //加载成品检返工原因
            string stre = string.Format(@"select a.*,b.检验日期 from 成品检验检验记录返工表 a left join 生产记录生产检验单主表 b on a.生产检验单号 = b.生产检验单号 where b.检验日期>='{0}' and b.检验日期<='{1}'", barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));
            DataTable dt_返工 = new DataTable();
            using(SqlDataAdapter da = new SqlDataAdapter(stre,strcoon))
           {
               da.Fill(dt_返工);
           }
        //加载在线检不良原因
            string stree = string.Format(@"select a.*,b.检验日期 from 快速检验检验记录返工表 a left join 快速检验生产检验单主表 b on a.生产检验单号 = b.生产检验单号 where b.检验日期>='{0}' and b.检验日期<='{1}'", barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));

            DataTable dt_不良 = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(stree, strcoon))
            {
                da.Fill(dt_不良);
            }

           foreach(DataRow r in dtM.Rows)
           {
               DataRow[] rs = dt_不良.Select(string.Format("生产检验单号='{0}'", r["在线检验单号"].ToString()));
               DataRow[] rs1 = dt_返工.Select(string.Format("生产检验单号='{0}'", r["成品检验单号"].ToString()));
               if(rs.Length>0)
               {
                 foreach(DataRow rr in rs)
                  {
                     
                      if (r["不良原因(在线检)"].ToString() == "")
                      {
                          r["不良原因(在线检)"] = rr["返工原因"].ToString() + "*" + rr["数量"].ToString();
                      }
                      else
                      {
                          r["不良原因(在线检)"] = r["不良原因(在线检)"].ToString() + "," + rr["返工原因"].ToString() + "*" + rr["数量"].ToString();
                      }
                  
                  }
             }

               if (rs1.Length > 0)
               {
                   foreach (DataRow rr1 in rs1)
                   {

                       if (r["不良原因(总检)"].ToString() == "")
                       {
                           r["不良原因(总检)"] = rr1["返工原因"].ToString() + "*" + rr1["数量"].ToString();
                       }
                       else
                       {
                           r["不良原因(总检)"] = r["不良原因(总检)"].ToString() + "," + rr1["返工原因"].ToString() + "*" + rr1["数量"].ToString();
                       }

                   }
               }




           }





        }

        private void fun_处理数据1()
        { 
          //加载所有 在线检不良现象
            string strr =string.Format(@"select 物料编码,返工编号,返工原因,SUM(数量)返工数量,COUNT(数量)单数 from (
          select fg.*,物料编码 from  [快速检验检验记录返工表] fg
          left join 快速检验生产检验单主表 kjy on fg.生产检验单号=kjy.生产检验单号  where kjy.检验日期>='{0}' and kjy.检验日期<='{1}')x
          group by 物料编码,返工编号,返工原因",barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));
            DataTable dt_不良 = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(strr, strcoon))
            {
                da.Fill(dt_不良);
            }
            //加载物料检验记录
            string stree = string.Format(@"select  物料编码,物料名称,规格型号,SUM(生产数量)生产数量,SUM(送检数量)送检数量,SUM(合格数量)在线检合格数量
    from ( select 生产工单号,物料编码,物料名称,规格型号,MAX(生产数量)生产数量,SUM(送检数量)送检数量,SUM(合格数量)合格数量 from 快速检验生产检验单主表 where 检验日期>='{0}' and 检验日期<='{1}'group by 生产工单号,物料编码,物料名称,规格型号)x
    group by 物料编码,物料名称,规格型号", barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));
              dtMM = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(stree, strcoon))
            {
                da.Fill(dtMM);
                dtMM.Columns.Add("合格数量(总检)", typeof(decimal));
                dtMM.Columns.Add("合格率(总检)", typeof(string));
                dtMM.Columns.Add("合格率(在线检)", typeof(string));
                dtMM.Columns.Add("不良原因(在线检)", typeof(string));
                dtMM.Columns.Add("不良原因(总检)", typeof(string));
            }
            //加载所有 总检不良现象
            string strr1 = string.Format(@"select 物料编码,返工编号,返工原因,SUM(数量)返工数量,COUNT(数量)单数 from (
          select fg.*,物料编码 from  [成品检验检验记录返工表] fg
          left join 生产记录生产检验单主表 kjy on fg.生产检验单号=kjy.生产检验单号  where kjy.检验日期>='{0}' and kjy.检验日期<='{1}')x
          group by 物料编码,返工编号,返工原因", barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));
            DataTable dt_返工 = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(strr1, strcoon))
            {
                da.Fill(dt_返工);
            }

         foreach(DataRow r in dtMM.Rows)
         {
             r["合格数量(总检)"] = 0;
             DataRow[] rs = dt_不良.Select(string.Format("物料编码='{0}'", r["物料编码"].ToString()));
             DataRow[] rs1 = dt_返工.Select(string.Format("物料编码='{0}'", r["物料编码"].ToString()));
             if (rs.Length > 0)
             {
                 foreach (DataRow rr in rs)
                 {
                
                 if (r["不良原因(在线检)"].ToString() == "")
                     {
                         r["不良原因(在线检)"] = rr["返工原因"].ToString() + "*" + rr["返工数量"].ToString();
                     }
                     else
                     {
                         r["不良原因(在线检)"] = r["不良原因(在线检)"].ToString() + "," + rr["返工原因"].ToString() + "*" + rr["返工数量"].ToString();
                     }

                 }
             }

             if (rs1.Length > 0)
             {
                 
                 decimal sd = 0;
                 foreach (DataRow rr1 in rs1)
                 {

                     sd = Convert.ToDecimal(rr1["返工数量"]) + sd;
                     r["合格数量(总检)"] = Convert.ToDecimal(r["送检数量"]) - sd;
                     if (r["不良原因(总检)"].ToString() == "")
                     {
                         r["不良原因(总检)"] = rr1["返工原因"].ToString() + "*" + rr1["返工数量"].ToString();
                     }
                     else
                     {
                         r["不良原因(总检)"] = r["不良原因(总检)"].ToString() + "," + rr1["返工原因"].ToString() + "*" + rr1["返工数量"].ToString();
                     }

                 }
             }

             if (Convert.ToDecimal(r["合格数量(总检)"]) == 0)
             {
                 r["合格数量(总检)"] = Convert.ToDecimal(r["送检数量"]);
             }

              Decimal dbdata =Convert.ToDecimal(r["合格数量(总检)"]) / Convert.ToDecimal(r["送检数量"]) * 100;
              decimal a = Math.Round(dbdata,2, MidpointRounding.AwayFromZero);//小数点保存两位
             r["合格率(总检)"] = a + "%";

              Decimal dbdata1 =Convert.ToDecimal(r["在线检合格数量"]) / Convert.ToDecimal(r["送检数量"]) * 100;
              decimal a1 = Math.Round(dbdata,2, MidpointRounding.AwayFromZero);//小数点保存两位

              r["合格率(在线检)"] = a1 + "%"; 


         


         }

        

        
        
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_处理数据1();
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                    options.ExportMode = DevExpress.XtraPrinting.XlsxExportMode.SingleFile;

                    ERPorg.Corg.TableToExcel(dtMM, saveFileDialog.FileName);


                    DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }





    }
}
