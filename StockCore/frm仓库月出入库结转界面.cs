using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraPrinting;

namespace StockCore
{
    public partial class frm仓库月出入库结转界面 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        DataTable dtM = null;
        DataTable dtS = null;
        DataTable dtQ = null;

        public frm仓库月出入库结转界面()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm仓库月出入库结转界面_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                label1.Text = "";
                checkBox2.Checked = false;
                barEditItem2.EditValue = System.DateTime.Now.Year.ToString();
               //手工做结转时取消注释
                //fun_载入物料();
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
            }
        }
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 手动做结转时用 
        /// 
        /// </summary>
        private void fun_载入物料()
#pragma warning restore IDE1006 // 命名样式
        {
    
            int yy = Convert.ToInt32(barEditItem2.EditValue);
            int MM =Convert.ToInt32(barEditItem1.EditValue)-1;
            if (MM == 0)
            {
                yy--;
                MM = 12;
            }
            string sql = "select * from 仓库月出入库结转表 where 1<>1";
            dtM = new DataTable();
            dtQ = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            da.Fill(dtQ);



            sql = string.Format(@"select base.原ERP物料编号,base.物料名称,base.规格型号,base.物料类型,base.n原ERP规格型号,
  cjz.本月结转金额,cjz.单价,cjz.本月结转数量,cjz.差异数量,cjz.差异金额,cjz.年,n核算单价,标准单价
 ,出库数量,入库数量 ,cjz.月,cjz.上月结转金额,cjz.上月结转数量,cjz.物料编码,base.仓库名称 
  from 基础数据物料信息表 base,仓库月出入库结转表 cjz
 where   cjz.物料编码=base.物料编码 and 年={0} and 月={1}   
      union
select a.原ERP物料编号,物料名称,b.规格型号,物料类型,n原ERP规格型号,
0 as 本月结转金额,0 as 单价,isnull(b.库存总数,0) 本月结转数量,0 as 差异数量,0 as 差异金额,{0}年,n核算单价,标准单价
, 0 出库数量, 0 入库数量 ,{1} 月, 0 上月结转金额,0 上月结转数量, 物料编码,仓库名称 
 from 基础数据物料信息表 a  left join (select * from [历史物料库存备份] where 年={0} and 月={1}) b on b.原ERP物料编号=a.原ERP物料编号 
  where 物料编码 not in (select  物料编码 from [仓库月出入库结转表] where 年={0} and 月={1}) ", yy, MM);

            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            gc.DataSource = dtM;
            //sql = string.Format("select 物料编码,本月结转数量 from 仓库月出入库结转表 where 年 = '{0}' and 月 = '{1}'", System.DateTime.Now.Year.ToString(), System.DateTime.Now.AddMonths(-1).Month.ToString());
            //dtS = new DataTable();
            //da = new SqlDataAdapter(sql, strconn);
            //da.Fill(dtS);
        }

#pragma warning disable IDE1006 // 命名样式
        private DataTable fun_计算(DateTime d1, DateTime d2, string str_出入库, string str_物料编码, Boolean bl_Time = false)
#pragma warning restore IDE1006 // 命名样式
        {

 
            string sql = string.Format(@"
  select 仓库出入库明细表.物料编码,sum(实效数量) as a   from 仓库出入库明细表 ,基础数据物料信息表
                where   仓库出入库明细表.物料编码=基础数据物料信息表.物料编码 and 出库入库 = '{0}' and 仓库出入库明细表.物料编码 = '{1}'
                and 出入库时间 <= '{2}' and 出入库时间 >= '{3}'   group by  仓库出入库明细表.物料编码
               ", str_出入库, str_物料编码, d2, d1);
            // select  物料编码,SUM(a)as a  from (
            
           // union
 //select 物料编码,sum(盘亏盘盈数) a  from 盘点调整库存出入库明细表,基础数据物料信息表
 // where  盘点调整库存出入库明细表.原物料编号 =基础数据物料信息表.原ERP物料编号 and 物料编码='{1}' and 出库入库 = '{0}'  and 
 // 操作时间<'{2}' and  操作时间>'{3}'  group by 物料编码)v  group by 物料编码


            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    DataRow dr = dt.NewRow();
                    dr["物料编码"] = str_物料编码;
                    dr["a"] = 0;
                   // dr["b"] = 0;
                    dt.Rows.Add(dr);
                }
                return dt;

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                label1.Visible = true;
                label1.Text = "正在计算中，由于数据量过大，请耐心等待....";

               // DateTime time = System.DateTime.Today;   //月末结转

            //   DateTime time =new DateTime(2018,2,28) ;   //结转日期
                int yy = Convert.ToInt32(barEditItem2.EditValue);
                int MM = Convert.ToInt32(barEditItem1.EditValue);
            
               DateTime time = new DateTime(yy, MM,1);   //结转月



                DateTime d1 = time;
                DateTime d2 = d1.AddMonths(1).AddSeconds(-1);
              
                foreach (DataRow dr in dtM.Rows)
                {
                    DataTable dt_临时 = fun_计算(d1, d2, "入库", dr["物料编码"].ToString().Trim());
                    dr["入库数量"] = dt_临时.Rows[0]["a"];

                    //if (dt_临时.Rows[0]["b"] == DBNull.Value)
                    //{
                    //    dt_临时.Rows[0]["b"] = 0;
                    //}
                    //decimal M_本月结转金额 = Convert.ToDecimal(dt_临时.Rows[0]["b"]);       //入库金额

                    if (dr["入库数量"] == DBNull.Value)
                    {
                        dr["入库数量"] = 0;

                    }
                  
       
                    

                    DataTable dt_临时2 = fun_计算(d1, d2, "出库", dr["物料编码"].ToString().Trim());
                    dr["出库数量"] = dt_临时2.Rows[0]["a"];
                    //if (dt_临时2.Rows[0]["b"] == DBNull.Value)
                    //{
                    //    dt_临时2.Rows[0]["b"] = 0;

                    //}
                    
                   // M_本月结转金额 = M_本月结转金额 +Convert.ToDecimal(dt_临时2.Rows[0]["b"]); //出库金额为负数 

                    if (dr["出库数量"] == DBNull.Value)
                    {
                        dr["出库数量"] = 0;
                    }


                    if (dr["本月结转数量"] == DBNull.Value)
                    {
                        dr["本月结转数量"] = 0;
                    }
                    decimal d_本月结转数量 = Convert.ToDecimal(dr["本月结转数量"]) + Convert.ToDecimal(dr["入库数量"]) + Convert.ToDecimal(dr["出库数量"]);



                    DataRow r = dtQ.NewRow();
                    dtQ.Rows.Add(r);
                    //r.ItemArray = dr.ItemArray;
                    r["物料编码"] = dr["物料编码"];
                    r["物料名称"] = dr["物料名称"];
                    r["物料类型"] = dr["物料类型"];

                    r["入库数量"] = dr["入库数量"];
                    r["出库数量"] = dr["出库数量"];
                    //r["入库金额"] = dt_临时.Rows[0]["b"] ;
                    //r["出库金额"] = dt_临时2.Rows[0]["b"];

                    //r["年"] = 2018;
                    //r["月"] =1;
                    r["年"] = time.Year;
                    r["月"] = time.Month;
                    r["GUID"] = System.Guid.NewGuid();
                    r["上月结转数量"] = dr["本月结转数量"];

                #region  2017-11月开会 整理 核算单价 全部使用基础表中的核算单价  
            
                #endregion 


                    //if(dr["物料类型"].ToString()=="原材料")
                    //{
                    //    r["单价"] = dr["标准单价"];
                    //   // r["上月结转金额"] = Convert.ToDecimal(dr["本月结转数量"]) * Convert.ToDecimal(dr["标准单价"]);
                    //    r["上月结转金额"] = Convert.ToDecimal(dr["本月结转金额"]);
                    //}
                    //else
                    //{
                        r["单价"] = dr["n核算单价"];
                      //  r["上月结转金额"] = Convert.ToDecimal(dr["本月结转数量"]) * Convert.ToDecimal(dr["n核算单价"]);
                        r["上月结转金额"] = Convert.ToDecimal(dr["本月结转金额"]);
                    //}
                    //r["本月结转金额"] = Convert.ToDecimal(dr["本月结转金额"])+Convert.ToDecimal(M_本月结转金额);

                    r["本月结转数量"] = d_本月结转数量;

                    //if (dr["物料类型"].ToString() == "原材料")
                    //{
                    //    r["本月结转金额"] = Convert.ToDecimal(r["本月结转数量"]) * Convert.ToDecimal(dr["标准单价"]);
                    //    r["入库金额"] = Convert.ToDecimal(r["入库数量"]) * Convert.ToDecimal(dr["标准单价"]);
                    //    r["出库金额"] = Convert.ToDecimal(r["出库数量"]) * Convert.ToDecimal(dr["标准单价"]);
                    //}
                    //else
                    //{
                        r["本月结转金额"] = Convert.ToDecimal(r["本月结转数量"]) * Convert.ToDecimal(dr["n核算单价"]);
                        r["入库金额"] = Convert.ToDecimal(r["入库数量"]) * Convert.ToDecimal(dr["n核算单价"]);
                        r["出库金额"] = Convert.ToDecimal(r["出库数量"]) * Convert.ToDecimal(dr["n核算单价"]);
                    //}


               

                    r["结算日期"] = time.AddMonths(1).AddDays(-1);
                    decimal a = Convert.ToDecimal(r["上月结转数量"]) - Convert.ToDecimal(r["本月结转数量"]) + Convert.ToDecimal(r["入库数量"]) + Convert.ToDecimal(r["出库数量"]);
                    if (a > 0)
                    {
                        r["差异数量"] = a;
                    }
                    else
                    {
                        r["差异数量"] = -a;
                    }
                 

                 
                           decimal b = Convert.ToDecimal(r["上月结转金额"])
                               + Convert.ToDecimal(r["入库金额"]) +
                               Convert.ToDecimal(r["出库金额"])
                               - Convert.ToDecimal(r["本月结转金额"]);
                    if (b > 0)
                    {
                        r["差异金额"] = b;

                    }
                    else
                    {
                        r["差异金额"] = -b;
                    }
                }

             //   MessageBox.Show("计算完成");
                label1.Visible = false;
                Application.DoEvents();
                barLargeButtonItem2_ItemClick(null, null);
                MessageBox.Show("结转完成");
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                label1.Visible = true;
                label1.Text = "正在保存中，由于数据量过大，请耐心等待几分钟...";
                string sql = "select * from 仓库月出入库结转表 where 1<>1";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dtQ);
                MessageBox.Show("保存成功");
                label1.Visible = false;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (barEditItem1.EditValue == DBNull.Value || barEditItem1.EditValue.ToString() == "" || barEditItem2.EditValue == DBNull.Value || barEditItem2.EditValue.ToString() == "")
                {
                    MessageBox.Show("请先填写年份选择月份！");
                }
                else
                {
                   // fun_载入物料();
                    string sql = string.Format(@"select  a.物料编码,[入库数量],出库数量 as 出库数量,[上月结转数量],[本月结转数量],出库金额
   ,[入库金额],[年],[月],[上月结转金额],[本月结转金额],[差异数量],差异金额,发出单价,收入单价
   ,[差异金额],[结算日期],b.物料名称,b.规格型号  from 仓库月出入库结转表 a
    left join 基础数据物料信息表 b on a.物料编码=b.物料编码  where   年 = '{0}' and 月 = '{1}'",
                        barEditItem2.EditValue.ToString(), barEditItem1.EditValue.ToString());
                    dtM = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dtM);
                    gc.DataSource = dtM;
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
       
                checkBox1.Checked = true;
                barLargeButtonItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barStaticItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barEditItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barStaticItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barEditItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                gc.DataSource = null;
          
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new XlsxExportOptions(TextExportMode.Text, false, false);
                gc.ExportToXlsx(saveFileDialog.FileName, options);

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
