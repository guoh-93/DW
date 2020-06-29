using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace ItemInspection
{
    public partial class ui采购到货计划 : UserControl
    {
        public ui采购到货计划()
        {
            InitializeComponent();
        }
        string strconn = CPublic.Var.strConn;
        string strconn1 = CPublic.Var.geConn("DW");
        DataTable dt_合 = new DataTable();
        DataTable dt1;
        DataTable dtp_数;
        bool bl_sync = false;
        DataTable dt_值变化;
        DataTable dt_物料周期;
        private void ui采购到货计划_Load(object sender, EventArgs e)
        {
          
         }
        private void fun_数据()
        {
          
            //加载BOM
            string std = @"select a.产品编码,b.物料名称 as 产品名称,b.规格型号 as 父项规格,a.子项编码,a.子项名称,c.规格型号 as 子项规格,a.数量,b.可购 ,b.采购周期,b.生产周期 
                           from 基础数据物料BOM表 a  
                           left join 基础数据物料信息表 c on a.子项编码 = c.物料编码
                           left join 基础数据物料信息表 b on a.产品编码 = b.物料编码";

            using (SqlDataAdapter da = new SqlDataAdapter(std, CPublic.Var.strConn))
            {
                da.Fill(dt_合);
            }

            //加载物料的 库存，在途量，未领量
            string sff = @"select kc.物料编码,库存总数,未领量,在途量  from  
                        (select  物料编码,sum(库存总数)库存总数,max(未领量)未领量,MAX(在途量) 在途量 from 仓库物料数量表
                             where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 ='仓库类别' and 布尔字段2=1) group by 物料编码)  kc 
                  left join 基础数据物料信息表 base   on  base.物料编码=kc.物料编码
                  where 可购=1";
            using (SqlDataAdapter da = new SqlDataAdapter(sff, strconn))
            {
                dtp_数 = new DataTable();
                da.Fill(dtp_数);
            }

            string sdd = "select 物料编码,采购周期 from 基础数据物料信息表";
            using (SqlDataAdapter da = new SqlDataAdapter(sdd, strconn))
            {
                dt_物料周期= new DataTable();
                da.Fill(dt_物料周期);
            }

        }

//        private void fun_load()
//        {
//            string str1 = "";
//            if (dateEdit1.Text.ToString() == "" && dateEdit2.Text.ToString() == "")
//            {
//                throw new Exception("请选择送达时间");
//            }

//        string str = string.Format(@"select a.销售订单明细号,a.物料编码,a.物料名称,a.规格型号,a.未完成数量 as 销售未完成数量,a.送达日期 as 销售送达日期,b.生产制令单号,b.数量 as 制令数量,b.送达日期 as 制令送达日期 from 销售记录销售订单明细表 a 
//  left join 生产记录生产制令子表 b on a.销售订单明细号 = b.销售订单明细号  where a.明细完成 = 0 ");
//            if(dateEdit1.Text.ToString()!="")
//            {
//                str1 = str + string.Format(" and a.送达日期 >'{0}' order by 销售送达日期", dateEdit1.EditValue);
            
//            }
//            if (dateEdit2.Text.ToString() != "")
//            {
//                str1 = str + string.Format(" and a.送达日期 <'{0}' order by 销售送达日期", Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1));

//            }
//            DataTable dtM = new DataTable();
//            using(SqlDataAdapter da = new SqlDataAdapter(str1,strconn))
//            {
//               da.Fill(dtM);
            
//            }
//            //找出相应所有末级物料 （可购=1 跳出）
//            DataTable dt_re = new DataTable();
//            dt_re.Columns.Add("订单号", typeof(string));
//            dt_re.Columns.Add("产品编码", typeof(string));
//            dt_re.Columns.Add("产品名称", typeof(string));
//             dt_re.Columns.Add("子项编码",typeof(string));
//             dt_re.Columns.Add("子项名称", typeof(string));
//             dt_re.Columns.Add("子项规格", typeof(string));
//             dt_re.Columns.Add("数量", typeof(decimal));
//             dt_re.Columns.Add("制令数量", typeof(decimal));//用不到
//             dt_re.Columns.Add("日期", typeof(DateTime));//送达日期
//             dt_re.Columns.Add("到货日期", typeof(DateTime));
//             dt_re.Columns.Add("采购日期", typeof(DateTime));
//           //创建辅助存值dt
//             DataTable dt_辅助 = new DataTable();
//             dt_辅助.Columns.Add("子项编码", typeof(string));
//               dt1 = new DataTable();

//            string str_时间="";
//            string str_单号 = "";
//            decimal dec_数量 = 0;
//            foreach (DataRow dr in dtM.Rows)
//            {
//                dt_辅助.Clear();

//                if (dr["生产制令单号"] != null && dr["生产制令单号"].ToString() != "")
//            {
//             str_时间 =dr["制令送达日期"].ToString();
//             str_单号 = dr["生产制令单号"].ToString();
//             dec_数量 = Convert.ToDecimal(dr["制令数量"]);
//            }
//            else
//            {
//                str_时间 = dr["销售送达日期"].ToString();
//                str_单号 = dr["销售订单明细号"].ToString();
//                dec_数量 = Convert.ToDecimal(dr["销售未完成数量"]);
//            }
             
//            //10010120030001
//           // dt1 =billofM_mo(str_单号,str_时间, dt_re, dr["物料编码"].ToString(),dec_数量,dt_辅助);
//               dt1 = ERPorg.Corg.billofM_mo(str_单号, str_时间, dt_re, dr["物料编码"].ToString(), dec_数量, dt_辅助,dt_合);
//            }
//            //匹配计算出到货日期 和 采购日期
//            foreach (DataRow rr in dt1.Rows)
//            {
//                DataRow[] rf = dt_合.Select(string.Format("产品编码='{0}'", rr["子项编码"].ToString()));
//                if (rf.Length > 0)
//                {
//                    if (rr["订单号"].ToString().Substring(0, 1) == "PM")
//                    {
//                        rr["采购日期"] = DateTime.Parse(rr["日期"].ToString()).AddDays(-Convert.ToInt32(rf[0]["采购周期"]));
//                    }
//                    else
//                    {

//                        rr["采购日期"] = DateTime.Parse(rr["日期"].ToString()).AddDays(-(Convert.ToInt32(rf[0]["生产周期"]))).AddDays(-(Convert.ToInt32(rf[0]["采购周期"])));

//                    }
//                }
//            }


//            gridControl1.DataSource = dt1;
           



//     }


        /// <summary>
        /// 传入一个产品编码和需返回的dt 为该产品所有末节的子项,传入dt是为了外面可以循环调用,可以不停往里dt里写入
        /// </summary>
        /// <param name="dt_return"> 仅有一列 'dt_return存储结构'</param>
        /// <param name="str">产品编码 </param>
        ///  <param name="dt_辅助">用来暂存每个物料的清单，每遍历一次清空一次</param>
        public static DataTable billofM_mo (string str_单号,string str_日期,DataTable dt_return, string str_产品,decimal dec_数量,DataTable dt_辅助)
        {
            string std = @"select a.产品编码,b.物料名称 as 产品名称, a.子项编码,a.子项名称,c.规格型号 as 子项规格,a.数量,b.可购 ,b.采购周期,b.生产周期 from 基础数据物料BOM表 a  
           left join 基础数据物料信息表 c on a.子项编码 = c.物料编码
            left join 基础数据物料信息表 b on a.产品编码 = b.物料编码";
            DataTable dt_集合 = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(std, CPublic.Var.strConn))
            {
                da.Fill(dt_集合);
            }

            DataTable dt = new DataTable();
            string s = string.Format("select  产品编码,子项编码,(数量 * '{0}')as 数量 from 基础数据物料BOM表 where 产品编码='{1}'",Convert.ToDecimal(dec_数量),str_产品);
            using (SqlDataAdapter da = new SqlDataAdapter(s, CPublic.Var.strConn))
            {
               //da.Fill(dt_return);
                da.Fill(dt);
             
            }
          //  DataTable datta = dt_集合.Copy();
          //   DataRow[] dr = datta.Select(string.Format("产品编码='{0}'", str_产品.ToString()));
          //  DataTable dt = ERPorg.Corg.datrowToDataTable(dr);
          ////  dt.Columns.Add("数量");
          //  foreach(DataRow drr  in dt.Rows ){

          //    decimal a=0;
          //   // drr["数量"] = 0;
          //    a = decimal.Parse(drr["数量"].ToString()) * dec_数量;
          //    drr["数量"]=a;

            DataTable dt_cp = dt.Copy();
            DataTable dt00 = dt_集合.Copy();
            foreach (DataRow r in dt_cp.Rows)
            {
                //s = string.Format("select  子项编码,(数量 * '{0}')as 数量 from 基础数据物料BOM表 where 产品编码='{1}'",Convert.ToDecimal(r["数量"]),r["子项编码"]);
                //DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, CPublic.Var.strConn);
                
               
                DataRow[] ds = dt00.Select(string.Format("产品编码='{0}'", str_产品));

                if (ds.Length > 0)
                {
                   
                    DataTable temp = dt_集合.Clone();
                    foreach (DataRow rs in ds)
                    {
                        DataRow[] dss = dt_集合.Select(string.Format("产品编码='{0}' and 子项编码='{1}'", rs["产品编码"], rs["子项编码"]));
                        rs["数量"] = dss[0]["数量"];
                        rs["数量"] = Convert.ToDecimal(rs["数量"]) * Convert.ToDecimal(r["数量"]);
                        temp.ImportRow(rs);

                    }
                 
                    fun_dg_billofM(dt_return,temp, str_单号, str_日期, str_产品, dt_辅助, dt_集合);
                   
                }
                else
                {
                    DataRow[] rr = dt_集合.Select(string.Format("产品编码='{0}' and  子项编码='{1}'", r["产品编码"], r["子项编码"]));

                    DataRow dr1 = dt_return.NewRow();
                    dr1["订单号"] = str_单号.ToString();
                    dr1["产品编码"] = str_产品;
                    dr1["产品名称"] = rr[0]["产品名称"].ToString();
                    dr1["子项编码"] = rr[0]["子项编码"].ToString();
                    dr1["子项名称"] = rr[0]["子项名称"].ToString();
                    dr1["子项规格"] = rr[0]["子项规格"].ToString();
                    dr1["数量"] = Convert.ToDecimal(rr[0]["数量"]) * dec_数量;
                    dr1["日期"] = Convert.ToDateTime(str_日期);
                    dt_return.Rows.Add(dr1);


                }
            }
                 return dt_return;
        }

        private static DataTable fun_dg_billofM(DataTable dt, DataTable dt_子,string str_单号,string str_日期,string str_产品,DataTable dt_辅助,DataTable dt_集合)
        {
           DataTable dt00 = dt_集合.Copy();
            if (dt_子.Rows.Count > 0)
            {
             
                foreach (DataRow xr in dt_子.Rows)
                {
//                   string s = string.Format(@"select (a.数量 * '{0}')as 数量,a.子项编码,a.子项名称,b.规格型号,b.可购 from 基础数据物料BOM表 a 
//                     left join 基础数据物料信息表 b on a.产品编码 = b.物料编码
//                     where 产品编码='{1}'",Convert.ToDecimal(xr["数量"]) ,xr["子项编码"]);
//                    DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, CPublic.Var.strConn);

                    DataRow[] ds = dt00.Select(string.Format("产品编码='{0}'", xr["子项编码"].ToString()));
                    DataTable temp = dt_集合.Clone();
                    foreach(DataRow rs in ds)
                    {
                        rs["数量"] = Convert.ToDecimal(rs["数量"]) * Convert.ToDecimal(xr["数量"]);
                        temp.ImportRow(rs);
                    }

                    if (temp.Rows.Count > 0 && temp.Rows[0]["可购"].Equals(false))
                    {
                        fun_dg_billofM(dt, temp,str_单号,str_日期,str_产品,dt_辅助,dt_集合);
                    }
                    else
                    {
                        if (dt_辅助.Select(string.Format("子项编码='{0}'", xr["子项编码"])).Length > 0) continue;
                        else
                        {
                            DataRow dr = dt_辅助.NewRow();
                            dr["子项编码"] = xr["子项编码"].ToString();
                            dt_辅助.Rows.Add(dr);

                            //dt.ImportRow(xr);
                            DataRow dr1 = dt.NewRow();
                            dr1["订单号"] = str_单号.ToString();
                            dr1["产品编码"] = str_产品;
                            dr1["产品名称"] = xr["产品名称"].ToString();
                            dr1["子项编码"] = xr["子项编码"].ToString();
                            dr1["子项名称"] = xr["子项名称"].ToString();
                            dr1["子项规格"] = xr["子项规格"].ToString();
                            dr1["数量"] = Convert.ToDecimal(xr["数量"]);
                            
                            dr1["日期"] = Convert.ToDateTime(str_日期);
                            dt.Rows.Add(dr1);
                        }
                    
                    }
                }
            }


            return dt;
        }
        //查询
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
               // fun_load();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

      

        private void gridView1_CustomDrawRowIndicator_1(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            
        }
        
     

        private void barLargeButtonItem2_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                // DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions(TextExportMode.Text, false, false);
                /// gridControl1.ExportToXlsx(saveFileDialog.FileName, options);
                //    DevExpress.XtraEditors.XtraMessageBox.Show("导出成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DataTable df = new DataTable();
                DataView dv = (DataView)gridControl1.DataSource;
                df = dv.Table;
                ERPorg.Corg.TableToExcel(df, saveFileDialog.FileName);
                MessageBox.Show("导出成功");
            }



        }

     

    

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }
        //加载采购到货计划
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (bl_sync)
                {
                    // fun_load();
                    fun_load_U8();
                }
                else
                {
                    throw new Exception("请先同步BOM，库存！");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
       //U8 load
       
        private void fun_load_U8()
        {
            string str1 = "";
            if (dateEdit1.Text.ToString() == "" && dateEdit2.Text.ToString() == "")
            {
                throw new Exception("请选择开工时间");
            }

            //            string str = string.Format(@"select a.销售订单明细号,a.物料编码,a.物料名称,a.规格型号,a.未完成数量 as 销售未完成数量,
            //a.送达日期 as 销售送达日期,b.生产制令单号,b.数量 as 制令数量,b.送达日期 as 制令送达日期 from 销售记录销售订单明细表 a 
            //  left join 生产记录生产制令子表 b on a.销售订单明细号 = b.销售订单明细号  where a.明细完成 = 0 ");

            string str = string.Format(@"select c.MoCode as 订单号,a.InvCode as 产品编码,a.Qty as 数量,b.StartDate from mom_orderdetail a  
            left join mom_morder b on a.MoDId = b.MoDId
            left join mom_order c  on  a.MoId = c.MoId
           where a.Status =2  ");

            if (dateEdit1.Text.ToString() != "")
            {
                str1 = str + string.Format(" and b.StartDate  >='{0}' order by b.StartDate", dateEdit1.EditValue);

            }
            if (dateEdit2.Text.ToString() != "")
            {
                str1 = str + string.Format(" and b.StartDate  <='{0}' order by b.StartDate", Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1));

            }
            DataTable dtM = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(str1, strconn1))
            {
                da.Fill(dtM);
            }
            //找出相应所有末级物料 （可购=1 跳出）
            DataTable dt_re = new DataTable();
            dt_re.Columns.Add("订单号", typeof(string));
            dt_re.Columns.Add("产品编码", typeof(string));
            dt_re.Columns.Add("产品名称", typeof(string));
            dt_re.Columns.Add("父项规格", typeof(string));
            dt_re.Columns.Add("子项编码", typeof(string));
            dt_re.Columns.Add("子项名称", typeof(string));
            dt_re.Columns.Add("子项规格", typeof(string));
            dt_re.Columns.Add("数量", typeof(decimal));
            dt_re.Columns.Add("制令数量", typeof(decimal));//用不到
            dt_re.Columns.Add("日期", typeof(DateTime));//送达日期
            dt_re.Columns.Add("到货日期", typeof(DateTime));
            dt_re.Columns.Add("库存总数", typeof(decimal));
            dt_re.Columns.Add("未领量", typeof(decimal));
            dt_re.Columns.Add("在途量", typeof(decimal));
           // dt_re.Columns.Add("采购日期", typeof(DateTime));
            //创建辅助存值dt 库存总数,未领量,在途量
            DataTable dt_辅助 = new DataTable();
            dt_辅助.Columns.Add("子项编码", typeof(string));
            dt1 = new DataTable();

            string str_时间 = "";
            string str_单号 = "";
            decimal dec_数量 = 0;
            foreach (DataRow dr in dtM.Rows)
            {
                dt_辅助.Clear();

                //if (dr["生产制令单号"] != null && dr["生产制令单号"].ToString() != "")
                //{
                //    str_时间 = dr["制令送达日期"].ToString();
                //    str_单号 = dr["生产制令单号"].ToString();
                //    dec_数量 = Convert.ToDecimal(dr["制令数量"]);
                //}
                //else
                //{
                str_时间 = dr["StartDate"].ToString();
                str_单号 = dr["订单号"].ToString();
                dec_数量 = Convert.ToDecimal(dr["数量"]);
                //}

                //递归
               //dt1 = billofM_mo(str_单号, str_时间, dt_re, dr["物料编码"].ToString(), dec_数量, dt_辅助);
               dt1 = ERPorg.Corg.billofM_mo(str_单号, str_时间, dt_re, dr["产品编码"].ToString(), dec_数量, dt_辅助,dt_合);
                
            }
            dt_值变化 = dt1.Copy();

            //匹配计算出建议采购日期 和 采购日期
            //foreach (DataRow rr in dt1.Rows)
            //{
            //  //  DataRow[] rf = dt_合.Select(string.Format("产品编码='{0}' and 子项编码='{1}' ", rr["产品编码"].ToString(),rr["子项编码"]));
            //    DataRow[] rf = dt_物料周期.Select(string.Format("物料编码='{0}'", rr["子项编码"]));
            //    if (rf.Length > 0)
            //    {
            //        rr["日期"] = DateTime.Parse(rr["日期"].ToString()).AddDays(-Convert.ToInt32(rf[0]["采购周期"])-1);
            //     }
            //}
            //汇总物料
            MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
            DataTable dt_汇总 = RBQ.SelectGroupByInto("", dt1, "子项编码,子项名称,子项规格,sum(数量) 数量,min(日期) 日期,库存总数,未领量,在途量", "", "子项编码,子项名称,子项规格,库存总数,未领量,在途量");
       
            //匹配数量 和 加建议采购日期
            dt_汇总.Columns.Add("建议采购日期", typeof(DateTime));
            dt_汇总.Columns.Add("标识", typeof(int));
            foreach(DataRow rt in dt_汇总.Rows)
             {
                 DataRow[] rf = dt_物料周期.Select(string.Format("物料编码='{0}'", rt["子项编码"]));
                 if (rf.Length > 0)
                 {
                     rt["建议采购日期"] = DateTime.Parse(rt["日期"].ToString()).AddDays(-Convert.ToInt32(rf[0]["采购周期"]) - 1);
                 }

                DataRow[] rs = dtp_数.Select(string.Format("物料编码='{0}'",rt["子项编码"]));
                if (rs.Length != 0)
                {
                    rt["库存总数"] = Convert.ToDecimal(rs[0]["库存总数"]);
                    rt["未领量"] = Convert.ToDecimal(rs[0]["未领量"]);
                    rt["在途量"] = Convert.ToDecimal(rs[0]["在途量"]);
                }
                else
                {
                    rt["库存总数"] = 0;
                    rt["未领量"] =0;
                    rt["在途量"] = 0;
                }
            //标识1表示： 库存不够，在途和库存加起来够需求数量的单 2表示：在途和库存都不够需求数量的单 3表示库存够的订单
                  rt["标识"]=0;
                  if (Convert.ToInt32(rt["库存总数"]) > Convert.ToInt32(rt["数量"]))
                  {
                      rt["标识"] = 3;
                  }
                  else
                  {
                      if ((Convert.ToInt32(rt["库存总数"]) + Convert.ToInt32(rt["在途量"])) >= Convert.ToInt32(rt["数量"]))
                      {
                          rt["标识"] = 1;
                      }
                      else
                      {
                          rt["标识"] =2; 
                      }
                  
                  }


             }
             DataView dv = new DataView(dt_汇总);
             dv.Sort = "建议采购日期 asc";
            DataTable dtu_加周期 = new DataTable();
            dtu_加周期 = dv.Table;
            dtu_加周期.Columns.Add("采购周期",typeof(decimal)); 
            foreach(DataRow rg in dtu_加周期.Rows)
            {
                DataRow[] rs = dt_物料周期.Select(string.Format("物料编码='{0}'", rg["子项编码"]));
                if(rs.Length>0)
                {
                rg["采购周期"] = Convert.ToDecimal(rs[0]["采购周期"]);
                }
            }


            gridControl1.DataSource = dtu_加周期;




        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
           simpleButton3.Text = "正在同步中..稍候";
            Thread th = new Thread(fun_数据);
            th.IsBackground = true;
            th.Start();
            bl_sync = true;
            simpleButton3.Text = "已同步,并加载完成";
        }

   
        private void gridView1_RowCellClick_1(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                DataRow[] ds = dt_值变化.Select(string.Format("子项编码='{0}'", dr["子项编码"]));
                DataTable dtt = ERPorg.Corg.datrowToDataTable(ds);
                gridControl2.DataSource = dtt;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void gridView1_CustomDrawRowIndicator_2(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

      

        private void gridView1_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView1.GetFocusedRowCellValue(gridView1.FocusedColumn));
                e.Handled = true;
            }
        }

        private void gridView2_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
        


        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (gridView1.GetRowCellValue(e.RowHandle,"标识").ToString() == "2")
                {
                    e.Appearance.BackColor = Color.Pink;
                }
                else
                {
                    if (gridView1.GetRowCellValue(e.RowHandle,"标识").ToString() == "1")
                    {
                        e.Appearance.BackColor = Color.Yellow;
                    }
                }
            }
            catch
            {


            }
        }





    }
}
