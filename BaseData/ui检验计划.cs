using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
namespace BaseData
{
    public partial class ui检验计划 : UserControl
    {
        public ui检验计划()
        {
            InitializeComponent();
        }

        #region  变量

        DataTable dt_boom, dt_合, dt_push, dt_main2,dt_re;
        DataTable dt_totalcount;
        string strconn = CPublic.Var.strConn;
        DataTable dtM;  
        DataTable dtp_数;
        bool bl_sync = false;
        bool bl_calculate = false;
        string str_log = "";
        DataTable dt_物料周期;
        string strconn1 = CPublic.Var.geConn("DW");
   
        #endregion

        #region  方式1



        public static DataTable billofM(DataTable dt_return, string str, string str_生产制令单号, string str_预完工日期, string str_制令数量, DataTable dt_集合)
        {

      
            DataRow[] dr = dt_集合.Select(string.Format("产品编码='{0}'", str.ToString()));
            DataTable dt = ERPorg.Corg.datrowToDataTable(dr);
            dt.Columns.Add("需求数量", typeof(Decimal));        
            dt.Columns.Add("预完工日期");
            dt.Columns.Add("制令数量", typeof(Decimal));
            dt_return = dt.Clone();
                    
            DataTable dt_cp = dt.Copy();
            foreach (DataRow r in dt_cp.Rows)
            {
                r["制令数量"] = str_制令数量;
                r["预完工日期"] = str_预完工日期;
                decimal a = 0;
                a = Convert.ToDecimal(r["制令数量"].ToString()) * Convert.ToDecimal(r["数量"].ToString());
                r["需求数量"] = a;
                DataRow[] ds = dt_集合.Select(string.Format("产品编码='{0}'", r["子项编码"].ToString()));
                DataTable dt_return2 = ERPorg.Corg.datrowToDataTable(dr);
                if (ds.Length > 0)
                {
                    DataTable temp = dt_return.Copy();

               dt_return=    fun_dg_billofM(temp, dt_return2, str_生产制令单号, str_预完工日期, str_制令数量, a,dt_集合);
                    ///结构体，数据dt
                }
                else
                {
/////保存dt  返回值

                    //DataRow drr = dt_return.NewRow();
                    //dr = r;
                    //dt_return.ImportRow(drr);

                    DataRow drr = dt_return.NewRow();
                    drr = r;
                    dt_return.ImportRow(drr);




                    //// dt_return1.Rows.InsertAt(dr, 0);



                }
                
            }
            return dt_return;
        }


        private static DataTable fun_dg_billofM( DataTable dt, DataTable dt_子, string str_生产制令单号, string str_预完工日期, string str_制令数量, decimal needmath,DataTable dt_集合)
        {

            dt_子.Columns.Add("需求数量", typeof(Decimal));
            dt_子.Columns.Add("预完工日期");
            dt_子.Columns.Add("制令数量", typeof(Decimal));

            DataTable dt_return = new DataTable() ;

            dt_return = dt_子.Clone();

            if (dt_子.Rows.Count > 0)
            {
                foreach (DataRow xr in dt_子.Rows)
                {
                    xr["制令数量"] = str_制令数量;
                    xr["预完工日期"] = str_预完工日期;
                    decimal a = 0;
                    a = needmath * Convert.ToDecimal(xr["数量"].ToString());
                    xr["需求数量"] = a;
                    DataRow[] ds = dt.Select(string.Format("产品编码='{0}'", xr["子项编码"].ToString()));
                    DataTable dt_zi = ERPorg.Corg.datrowToDataTable(ds);

                    if (ds.Length > 0 && Convert.ToBoolean(xr["可购"].ToString()) == false)
                    {
                        a = a * Convert.ToDecimal(xr["数量"].ToString());
                        xr["需求数量"] = a;
                   fun_dg_billofM(dt, dt_zi, str_生产制令单号, str_预完工日期, str_制令数量, a,dt_集合);
                    }
                    else
                    {
                        DataRow drr = dt_return.NewRow();
                        drr = xr;
                        dt_return.ImportRow(drr);
                       
                    }
                }
            }
            return dt_return;
        }
     
        #endregion

        #region  版本2


        public DataTable GetAllDataTable(DataSet ds)
        {
            DataTable newDataTable = ds.Tables[0].Clone();                //创建新表 克隆以有表的架构。
            object[] objArray = new object[newDataTable.Columns.Count];   //定义与表列数相同的对象数组 存放表的一行的值。
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                for (int j = 0; j < ds.Tables[i].Rows.Count; j++)
                {
                    ds.Tables[i].Rows[j].ItemArray.CopyTo(objArray, 0);    //将表的一行的值存放数组中。
                    newDataTable.Rows.Add(objArray);                       //将数组的值添加到新表中。
                }
            }
            return newDataTable;                                           //返回新表。
        }///合并多表

//        decimal needmath = 0;
//        public static DataTable billofM(DataTable dt_return1, string str, string str_生产制令单号)
//        {

         
//            DataTable dt_return = new DataTable();

//            string std = @"select a.产品编码,b.物料名称 as 产品名称, a.子项编码,a.子项名称,c.规格型号 as 子项规格,a.数量,b.可购 from 基础数据物料BOM表 a  
//           left join 基础数据物料信息表 c on a.子项编码 = c.物料编码
//            left join 基础数据物料信息表 b on a.产品编码 = b.物料编码";
//            DataTable dt_集合 = new DataTable();
//            using (SqlDataAdapter da = new SqlDataAdapter(std, CPublic.Var.strConn))
//            {
//                da.Fill(dt_集合);
//            }
//            DataRow[] dr = dt_集合.Select("产品编码='{0}'", str);
//            DataTable dt = ERPorg.Corg.datrowToDataTable(dr);      
//            dt.Columns.Add("需求数量", typeof(Decimal));
//          dt_return1 = dt_return.Clone();
         
//            DataTable dt_cp = dt.Copy();
//            foreach (DataRow r in dt_cp.Rows)
//            {
//                decimal a = 0;
//                a = Convert.ToDecimal(r["制令数量"].ToString()) * Convert.ToDecimal(r["数量"].ToString());
//                r["需求数量"] = a;
//                DataRow[] ds = dt_集合.Select(string.Format("产品编码='{0}'", r["子项编码"].ToString()));

//                DataTable temp = ERPorg.Corg.datrowToDataTable(ds);
//                temp.Columns.Add("需求数量", typeof(Decimal));
//                if (ds.Length > 0)
//                {

//                    fun_dg_billofM(dt_return1, dt_return, temp, str_生产制令单号, a);
//                }
//                else
//                {

//                    DataRow dr1 = dt_return.NewRow();
//                    dr1["订单号"] = str_生产制令单号.ToString();
//                    dr1["产品编码"] = str;
//                    dr1["产品名称"] = r["产品名称"].ToString();
//                    dr1["子项编码"] = r["子项编码"].ToString();
//                    dr1["子项名称"] = r["子项名称"].ToString();
//                    dr1["子项规格"] = r["子项规格"].ToString();
//                    dr1["数量"] = Convert.ToDecimal(r["数量"]);

//                    dr1["日期"] = Convert.ToDateTime(str_日期);
//                    dt.Rows.Add(dr1);
                
//                    // dt_return1.Rows.InsertAt(dr, 0);

//                }

//            }
//            return dt_return1;
//        }


//        private static DataTable fun_dg_billofM(DataTable dt_return1, DataTable dt, DataTable dt_子, string str_生产制令单号, decimal needmath)
//        {

//            if (dt_子.Rows.Count > 0)
//            {

//                foreach (DataRow xr in dt_子.Rows)
//                {
//                    decimal a = needmath;
//                    a = needmath * Convert.ToDecimal(xr["数量"].ToString());
//                    xr["需求数量"] = a;


//                    if (dt.Select(string.Format("子项编码='{0}'", xr["子项编码"])).Length > 0) continue;
//                    //else
//                    //{
//                    //    dt.ImportRow(xr);
//                    //}
//                    string s = string.Format(@"select  b.预完工日期,b.生产制令单号,b.物料名称,b.规格型号,b.物料编码,b.制令数量, c.可购,
//    a.* from 基础数据物料BOM表   a  
//left join 基础数据物料信息表 c on a.产品编码= c.物料编码 
//left join (select *  from 生产记录生产制令表 where 完成=0)b on   b.物料编码=a.产品编码  where a.产品编码='{0}' and b.生产制令单号='{1}' ", xr["子项编码"], str_生产制令单号);
//                    //                    string s = string.Format(@"select  b.预完工日期,b.生产制令单号,b.物料名称,b.规格型号,b.物料编码,b.制令数量, c.可购,  a.* from 基础数据物料BOM表   a  
//                    //left join 基础数据物料信息表 c on a.产品编码= c.物料编码 
//                    //left join (select *  from 生产记录生产制令表 where 完成='0')   b on   b.物料编码=a.产品编码   
//                    //where a.产品编码='{0}'and  b.生产制令单号='{1}' ", xr["子项编码"], str_生产制令单号);
//                    DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, CPublic.Var.strConn);
//                    temp.Columns.Add("需求数量", typeof(Decimal));
//                    if (temp.Rows.Count > 0 && Convert.ToBoolean(xr["可购"].ToString()) == false)
//                    {
//                        a = a * Convert.ToDecimal(xr["数量"].ToString());
//                        xr["需求数量"] = a;
//                        fun_dg_billofM(dt_return1, dt, temp, str_生产制令单号, a);
//                    }
//                    else
//                    {
//                        dt_return1.ImportRow(xr);

//                    }


//                }
//            }


//            return dt_return1;
//        }
     

        #endregion


        #region  版本3
        private void fun_selectBoM()
        {

            string s = @"  select  产品编码,产品名称,fx.规格型号 as 父项规格,子项编码,子项名称,数量,zx.可购 as 子项可购 
    ,zx.规格型号 as 子项规格       from 基础数据物料BOM表 bom 
   left join 基础数据物料信息表 zx on bom.子项编码=zx.物料编码 
   left join 基础数据物料信息表 fx on bom.产品编码=fx.物料编码";
         dt_boom = CZMaster.MasterSQL.Get_DataTable(s,strconn);

        }


        private void ui检验计划_Load(object sender, EventArgs e)
        {
            //DateTime t = CPublic.Var.getDatetime().Date;
            //date_后.EditValue = t;

            //t = t.AddMonths(-3);
            //t = new DateTime(t.Year, t.Month, 1);
            //date_前.EditValue = t;
//            dt_合 = new DataTable();
//            string std = @"select a.产品编码,b.物料名称 as 产品名称, a.子项编码,a.子项名称,c.规格型号 as 子项规格,a.数量,b.可购  from 基础数据物料BOM表 a  
//           left join 基础数据物料信息表 c on a.子项编码 = c.物料编码
//            left join 基础数据物料信息表 b on a.产品编码 = b.物料编码";

//            using (SqlDataAdapter da = new SqlDataAdapter(std, CPublic.Var.strConn))
//            {
//                da.Fill(dt_合);
//            }
           // string wsq = @"select b.dDate 单据日期 ,  b.cCode 采购到货单号, a.cInvCode  物料编码,a.iQuantity 到货数量,a.fValidInQuan 已入库数量  from    [192.168.20.150].UFDATA_008_2018.dbo.  PU_ArrivalVouchs  a
           //    left join  (select *  from  [192.168.20.150].UFDATA_008_2018.dbo. PU_ArrivalVouch   where cCloser is null) b   
           //on  a.ID=  b.ID  where   a.bGsp=0";


            string wsq = @"  
       select sx.*   from   采购记录采购送检单明细表 sx        
       left join ( select * from 采购记录采购单明细表 where 作废=0 and 明细完成=0 and 生效=1 ) xs  on sx.采购单明细号 =xs.采购明细号
       where  sx.作废=0 and sx.生效=1 and sx.检验完成=0  and sx.送检单类型<>'拒收' and sx.备注4<>'免检'
       
        ";




            dt_push = new DataTable();
            dt_push = CZMaster.MasterSQL.Get_DataTable(wsq, strconn);
        }

     
        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {

            try
            {
               // gridView1.CloseEditor();
              //  this.BindingContext[dt_显示].EndCurrentEdit();
               DataRow   drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
                ///////////// 剩下的功能 筛选    dt_push

               DataRow[] dr = dt_push.Select(string.Format("物料编码='{0}'", drM["子项编码"].ToString()));
               if (dr.Length > 0)
               {

                   DataTable dt = ERPorg.Corg.datrowToDataTable(dr);
                   DataTable dt_p = dt.Clone();
                  foreach(DataRow  drr in dt.Rows ){
                      if (decimal.Parse(drr["已检验数"].ToString()) < decimal.Parse(drr["送检数量"].ToString()))
                      {
                          //DataRow dtp = dt_p.NewRow();
                          //dtp = drr;
                         // dtp.ImportRow(drr);

                          DataRow drrr = dt_p.NewRow();
                          drrr = drr;
                          dt_p.ImportRow(drrr);

                     }
                  
                  
                  }

                   //dt.Columns.Add("需求数量");
                   //foreach (DataRow drr in dt.Rows)
                   //{
                   //    drr["需求数量"] = decimal.Parse(drr["到货数量"].ToString()) * decimal.Parse(drM["数量"].ToString());

                   //}
                  gridControl2.DataSource = dt_p;
               }
               else
               {
                   throw new Exception("当前物料无数据");
               }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
       
        private void fun_select()
        {
            //simpleButton1.Text = "查询中，请稍后...";
       

//            string sql = (@"select  b.MoCode 生产制令单号  , a.CloseUser 关闭人,c.StartDate 开工日期 ,a.InvCode 物料编码,(a.Qty -a.QualifiedInQty) 未领数量  from  [192.168.20.150].UFDATA_008_2018.dbo.mom_orderdetail   a
// left join    [192.168.20.150].UFDATA_008_2018.dbo. mom_order   b  on b.Moid=a.moid
//left join  [192.168.20.150].UFDATA_008_2018.dbo. mom_morder  c on a.ModId=c.ModId 
//  where  a.Status=3 and qty-QualifiedInQty>0  
//and a.CloseUser is null"  );
            string sql = (@"  
  select mx.生产制令单号,mx.物料编码,mx.物料名称,mx.未领数量, zb.预完工日期 from   生产记录生产工单待领料明细表  mx  
  left join  生产记录生产制令表  zb  on mx.生产制令单号=zb.生产制令单号 
  where mx.完成=0  and mx.未领数量>0");

            if (date_前.Text.ToString() != "")
            {
                sql = sql + string.Format(" and zb.预完工日期  >='{0}'", Convert.ToDateTime(date_前.EditValue).ToString("yyyy-MM-dd"));

            }
            if (date_后.Text.ToString() != "")
            {
                sql = sql + string.Format(" and zb.预完工日期  <='{0}'", Convert.ToDateTime(date_后.EditValue).AddDays(1).ToString("yyyy-MM-dd"));

            }


             

            DataTable dt_制令主数据 = new DataTable();
            dt_制令主数据 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

            dt_制令主数据.Columns.Add("库存总数", typeof(decimal));
            dt_制令主数据.Columns.Add("可购", typeof(bool));
            //MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
            //DataTable dt_SaleOrder_1 = RBQ.SelectGroupByInto("", dt_制令主数据, "物料编码,sum(数量) 数量", "", "物料编码");
            //DataTable IncompleteWorkOrder_1 = RBQ.SelectGroupByInto("", IncompleteWorkOrder, "物料编码,sum(未完成数量) 数量", "", "物料编码");

            dt_re = new DataTable();
            dt_re.Columns.Add("生产单号", typeof(string));
            dt_re.Columns.Add("产品编码", typeof(string));
            dt_re.Columns.Add("产品名称", typeof(string));
            dt_re.Columns.Add("父项规格", typeof(string));
            dt_re.Columns.Add("子项编码", typeof(string));
            dt_re.Columns.Add("子项名称", typeof(string));
            dt_re.Columns.Add("子项规格", typeof(string));
            dt_re.Columns.Add("参考数量", typeof(decimal));
            dt_re.Columns.Add("日期", typeof(DateTime));//送达日期
            dt_re.Columns.Add("到货日期", typeof(DateTime));
            dt_re.Columns.Add("制令数量", typeof(decimal));
            dt_re.Columns.Add("库存总数", typeof(decimal));
            dt_re.Columns.Add("可购", typeof(bool));
            dt_制令主数据.Columns.Add("规格型号");
            DataTable dt_main = dt_制令主数据.Clone();
            dt_main.Columns.Add("参考数量", typeof(decimal));

            dt_totalcount = dtp_数.Copy();
            dt_totalcount.Columns.Add("总数", typeof(decimal));
            foreach (DataRow xr in dt_totalcount.Rows)
            {

                xr["总数"] = xr["库存总数"].ToString();

            }
            foreach (DataRow rt in dt_制令主数据.Rows)
            {
                DataRow[] rs = dtp_数.Select(string.Format("物料编码='{0}'", rt["物料编码"]));
                rt["可购"] = Convert.ToBoolean(rs[0]["可购"]);
                if (rs.Length != 0)
                {
                    rt["库存总数"] = Convert.ToDecimal(rs[0]["库存总数"]);
                    rt["规格型号"] = rs[0]["规格型号"].ToString();
                }
                else
                {
                    rt["库存总数"] = 0;
                }
                if (decimal.Parse(rt["未领数量"].ToString()) > decimal.Parse(rt["库存总数"].ToString()))
                {
                    DataRow drr = dt_main.NewRow();
                    dt_main.Rows.Add(drr);
                    drr["参考数量"] = decimal.Parse(rt["未领数量"].ToString()) - decimal.Parse(rt["库存总数"].ToString());
                    drr["生产制令单号"] = rt["生产制令单号"].ToString();
                    //drr["关闭人"] = rt["关闭人"].ToString();
                    drr["预完工日期"] = rt["预完工日期"].ToString();
                    drr["物料编码"] = rt["物料编码"].ToString();
                    drr["物料名称"] = rt["物料名称"].ToString();
                    drr["规格型号"] = rt["规格型号"].ToString();
                    drr["未领数量"] = rt["未领数量"].ToString();
                    drr["库存总数"] = rt["库存总数"].ToString();
                    drr["可购"] = Convert.ToBoolean(rt["可购"]);
                    drr["参考数量"] = decimal.Parse(rt["未领数量"].ToString()) - decimal.Parse(rt["库存总数"].ToString());
                }

            }/////dt_main 缺料主数据

            dt_main2 = dt_main.Copy();


            foreach (DataRow dr in dt_main2.Rows)
            {
                if (dr["可购"].Equals(true))
                {
                    // if (dt_合.Select(string.Format("产品编码='{0}'", dr["物料编码"])).Length == 0) str_log = str_log + dr["物料编码"].ToString() + "属性为可购但是没有bom; ";

                    DataRow r_need = dt_re.NewRow();
                    r_need["子项编码"] = dr["物料编码"];
                  r_need["子项名称"] = dr["物料名称"];
                    r_need["子项规格"] = dr["规格型号"];
                    r_need["库存总数"] = dr["库存总数"];
                    r_need["可购"] = dr["可购"];
                    r_need["日期"] = dr["预完工日期"];
                    r_need["生产单号"] = dr["生产制令单号"];
                    r_need["参考数量"] = dr["未领数量"];
                    dt_re.Rows.Add(r_need);
                  //  stock_total[0]["总数"] = 0;


                }
                else
                {
                    DataRow[] br = dt_合.Select(string.Format("产品编码='{0}'and 子项可购=1", dr["物料编码"].ToString()));
                    DataTable dt_br = ERPorg.Corg.datrowToDataTable(br);

                    if (br.Length > 0) //找到需要可购的半成品 
                    {
                        decimal dec_缺 = Convert.ToDecimal(dr["参考数量"].ToString());
                        foreach (DataRow brr in dt_br.Rows)
                        {

                            decimal dec = dec_缺 * Convert.ToDecimal(brr["数量"]); //这是自制半成品所需的量   
                            DataRow[] stock_total = dt_totalcount.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                            decimal total_z = Convert.ToDecimal(stock_total[0]["总数"]);
                            if (total_z >= dec) //库存加未完成>需求数
                            {
                                stock_total[0]["总数"] = total_z - dec;
                            }
                            else
                            {
                                DataRow[] fr = dt_main2.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                                if (fr.Length > 0)
                                {
                                    fr[0]["参考数量"] = Convert.ToDecimal(fr[0]["参考数量"]) + dec - total_z;
                                }
                                else
                                {
                                    DataRow r_need = dt_re.NewRow();
                                    r_need["子项编码"] = stock_total[0]["物料编码"];
                                    r_need["子项名称"] = stock_total[0]["物料名称"];
                                    r_need["子项规格"] = stock_total[0]["规格型号"];
                                    r_need["库存总数"] = stock_total[0]["库存总数"];
                                    r_need["可购"] = stock_total[0]["可购"];
                                    r_need["日期"] = dr["预完工日期"];
                                    r_need["生产单号"] = dr["生产制令单号"];
                                    r_need["参考数量"] = dec - total_z;
                                    dt_re.Rows.Add(r_need);
                                    stock_total[0]["总数"] = 0;
                                }

                                //缺的才需要继续往叶子节点递归 不缺不需要
                                bool bl_t = false;
                                if (dr["可购"] == null || dr["可购"].ToString().Trim() == "")
                                {
                                    bl_t = false;
                                }
                                else
                                {
                                    bl_t = Convert.ToBoolean(dr["可购"]);
                                }

                                fun_dg(stock_total[0]["物料编码"].ToString(), dec - total_z, bl_t, dr["生产制令单号"].ToString(), DateTime.Parse(dr["预完工日期"].ToString()));

                            }
                }
             
                    }
              }
            }
            //a.iQuantity 到货数量,a.fValidInQuan 已入库数量
            //         string ssq = "select    cInvCode as 物料编码  from  [192.168.20.150].UFDATA_008_2018.dbo.PU_ArrivalVouchs   where    iQuantity >fValidInQuan    group by cInvCode ";

            string ssq = "select  物料编码 from 采购记录采购送检单明细表  where   送检数量>已检验数   group by 物料编码   ";

            DataTable dt_cheek = CZMaster.MasterSQL.Get_DataTable(ssq, strconn);
            DataTable dt_显示 = new DataTable();
            dt_显示 = dt_re.Clone();
            foreach (DataRow dr in dt_cheek.Rows)
            {
                DataRow[] rows = dt_re.Select(string.Format("子项编码='{0}'", dr["物料编码"].ToString()));

                if (rows.Length > 0)
                {
                    foreach (DataRow rs in rows)
                    {
                        DataRow drrr = dt_显示.NewRow();
                        drrr = rs;
                        dt_显示.ImportRow(rs);
                    }
                }
            }

            MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
            DataTable dt_汇总 = RBQ.SelectGroupByInto("", dt_显示, "子项编码,子项名称,子项规格,sum(参考数量) 所缺数量,min(日期) 日期", "", "子项编码,子项名称,子项规格");
            dt_汇总.Columns.Add("库存总数", typeof(decimal));

            foreach (DataRow rt in dt_汇总.Rows)
            {
                DataRow[] rs = dtp_数.Select(string.Format("物料编码='{0}'", rt["子项编码"]));
                if (rs.Length != 0)
                {
                    rt["库存总数"] = Convert.ToDecimal(rs[0]["库存总数"]);
                }
                else
                {
                    rt["库存总数"] = 0;
                }
            }
            dt_汇总.Columns.Add("建议检验日期", typeof(DateTime));
            foreach (DataRow dr in dt_汇总.Rows)
            {

                dr["建议检验日期"] = DateTime.Parse(dr["日期"].ToString()).AddDays(-1);

            } 

            BeginInvoke(new MethodInvoker(() =>
            {
                simpleButton1.Text = "查询";
            }));
            bl_calculate = true; //计算完成
            Method(gridControl1, gd =>
            {
                DataView dv = new DataView(dt_汇总);
                dv.Sort = "日期 asc";
                gridControl1.DataSource = dt_汇总;
            });
        }

        private void Method<T>(T c, Action<T> action) where T : DevExpress.XtraGrid.GridControl
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => action(c)));
            }
            else
                action(c);
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (date_前.Text.ToString() == "" && date_后.Text.ToString() == "")
                {
                    throw new Exception("请选择开工时间");
                }
                bl_calculate = false;
                if (bl_sync==false)
                {
                    throw new Exception("请先同步BOM，库存！");
                }     
           
               simpleButton1.Text = "正在查询计算中..稍候";
                Thread th2 = new Thread(fun_select);
                th2.IsBackground = true;
                th2.Start();
              //  bl_sync = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        #region  版本2递归



    private void fun_dg(string itemid, decimal dec_需求, bool bl_made,string makingNumber , DateTime  startTime)
        {
            if (bl_made)
            {
                if (dt_合.Select(string.Format("产品编码='{0}'", itemid)).Length == 0) str_log = str_log + itemid + "属性为自制但是没有bom; ";
            }

            DataRow[] br = dt_合.Select(string.Format("产品编码='{0}'and 子项可购=1", itemid));
            if (br.Length > 0) //找到需要自制的半成品 
            {
                decimal dec_缺 = dec_需求;

                foreach (DataRow brr in br)
                {
                    decimal dec = dec_缺 * Convert.ToDecimal(brr["数量"]); //这是自制半成品所需的量 
                    DataRow[] stock_total = dt_totalcount.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                    decimal total_z = Convert.ToDecimal(stock_total[0]["总数"]);
                    if (total_z >= dec) //库存加未完成>需求数
                    {
                        stock_total[0]["总数"] = total_z - dec;
                    }
                    else
                    {
                        DataRow[] fr = dt_main2.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                        if (fr.Length > 0)
                        {
                            fr[0]["参考数量"] = Convert.ToDecimal(fr[0]["参考数量"]) + dec - total_z;
                        }
                        else
                        {
                            DataRow r_need = dt_re.NewRow();
                           // r_need["未完成工单数"] = stock_total[0]["未完成工单数"];
                            r_need["子项编码"] = stock_total[0]["物料编码"];
                            r_need["子项名称"] = stock_total[0]["物料名称"];
                            r_need["子项规格"] = stock_total[0]["规格型号"];
                           // r_need["存货分类"] = stock_total[0]["存货分类"];
                            r_need["库存总数"] = stock_total[0]["库存总数"];
                            r_need["生产单号"] = makingNumber;
                            r_need["日期"] = startTime;
                            r_need["可购"] = stock_total[0]["可购"];
                            r_need["参考数量"] = dec - total_z;
                            dt_re.Rows.Add(r_need);
                            stock_total[0]["总数"] = 0;
                        }
                        fun_dg(stock_total[0]["物料编码"].ToString(), dec - total_z, Convert.ToBoolean(stock_total[0]["可购"]), makingNumber,startTime);
                    }
                }
            }
        }
        #endregion


        #region  其他


        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
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

        private void simpleButton3_Click(object sender, EventArgs e)
        {

            simpleButton3.Text = "正在同步中..稍候";
            Thread th = new Thread(fun_数据);
            th.IsBackground = true;
            th.Start();
            bl_sync = true;
            simpleButton3.Text = "已同步,并加载完成";

        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView1.GetFocusedRowCellValue(gridView1.FocusedColumn));
                e.Handled = true;
            }

        }

        private void gridView2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView2.GetFocusedRowCellValue(gridView2.FocusedColumn));
                e.Handled = true;
            }
        }


        //        private  static DataTable billofM(DataTable dt_return1, string str, string str_生产制令单号, string str_预完工日期, string str_制令数量)
        //        {

        //            DataTable dt = new DataTable();
        //            DataTable dt_return = new DataTable();
        //            DataTable dt_b=

        //        //    DataRow[] stock_total = dt.Select(string.Format("物料编码='{0}'", str));

        //            string s = string.Format(@"select  c.可购,  a.* from 基础数据物料BOM表   a  
        //left join 基础数据物料信息表 c on a.产品编码= c.物料编码 
        //  where a.产品编码='{0}'", str);
        //            // string s = string.Format("select  子项编码 from 基础数据物料BOM表 where 产品编码='{0}'", str);
        //            using (SqlDataAdapter da = new SqlDataAdapter(s, CPublic.Var.strConn))
        //            {
        //                da.Fill(dt_return);
        //                da.Fill(dt);

        //                dt_return.Columns.Add("需求数量", typeof(Decimal));
        //                dt.Columns.Add("需求数量", typeof(Decimal));
        //                dt_return.Columns.Add("预完工日期");
        //                dt.Columns.Add("预完工日期");
        //                dt_return.Columns.Add("制令数量", typeof(Decimal));
        //                dt.Columns.Add("制令数量", typeof(Decimal));


        //                dt_return1 = dt_return.Clone();
        //            }
        //            //if (includeItself) {
        //            //    DataRow dr = dt_return.NewRow(); dr["子项编码"] = str; dt_return.Rows.InsertAt(dr, 0); 
        //            //}
        //            DataTable dt_cp = dt.Copy();
        //            foreach (DataRow r in dt_cp.Rows)
        //            {
        //                r["制令数量"] = str_制令数量;
        //                r["预完工日期"] = str_预完工日期;
        //                decimal a = 0;
        //                ////a = Convert.ToDecimal(r["制令数量"].ToString()) * Convert.ToDecimal(r["数量"].ToString());
        //                r["需求数量"] = a;

        //                s = string.Format(@"select  c.可购,  a.* from 基础数据物料BOM表   a  
        //left join 基础数据物料信息表 c on a.产品编码= c.物料编码 
        //  where a.产品编码='{0}'", r["子项编码"].ToString());
        //                // s = string.Format("select  子项编码 from 基础数据物料BOM表 where 产品编码='{0}'", r["子项编码"]);
        //                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, CPublic.Var.strConn);
        //                temp.Columns.Add("需求数量", typeof(Decimal));
        //                temp.Columns.Add("制令数量", typeof(Decimal));
        //                temp.Columns.Add("预完工日期");
        //                if (temp.Rows.Count > 0 && Convert.ToBoolean(r["可购"].ToString()) == false)
        //                {

        //                  //  fun_dg_billofM(dt_return1, dt_return, temp, str_生产制令单号, str_预完工日期, str_制令数量, a);
        //                }
        //                else
        //                {

        //                    if (dt_return1.Select(string.Format("子项编码='{0}'", r["子项编码"])).Length > 0) continue;

        //                    DataRow dr = dt_return1.NewRow();
        //                    dr = r;
        //                    dt_return1.ImportRow(dr);

        //                    // dt_return1.Rows.InsertAt(dr, 0);

        //                }

        //            }
        //            return dt_return1;
        //        }


        //        private static DataTable fun_dg_billofM(DataTable dt_return1, DataTable dt, DataTable dt_子, string str_生产制令单号, string str_预完工日期, string str_制令数量, decimal needmath)
        //        {

        //            if (dt_子.Rows.Count > 0)
        //            {

        //                foreach (DataRow xr in dt_子.Rows)
        //                {
        //                    xr["制令数量"] = str_制令数量;
        //                    xr["预完工日期"] = str_预完工日期;

        //                    decimal a = needmath;
        //                    a = needmath * Convert.ToDecimal(xr["数量"].ToString());
        //                    xr["需求数量"] = a;

        //                    if (dt.Select(string.Format("子项编码='{0}'", xr["子项编码"])).Length > 0) continue;
        //                    //else
        //                    //{
        //                    //    dt.ImportRow(xr);
        //                    //}
        //                    string s = string.Format(@"select  c.可购,  a.* from 基础数据物料BOM表   a  
        //left join 基础数据物料信息表 c on a.产品编码= c.物料编码 
        //  where a.产品编码='{0}'", xr["子项编码"]);
        //                    //                    string s = string.Format(@"select  b.预完工日期,b.生产制令单号,b.物料名称,b.规格型号,b.物料编码,b.制令数量, c.可购,  a.* from 基础数据物料BOM表   a  
        //                    //left join 基础数据物料信息表 c on a.产品编码= c.物料编码 
        //                    //left join (select *  from 生产记录生产制令表 where 完成='0')   b on   b.物料编码=a.产品编码   
        //                    //where a.产品编码='{0}'and  b.生产制令单号='{1}' ", xr["子项编码"], str_生产制令单号);
        //                    DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, CPublic.Var.strConn);
        //                    temp.Columns.Add("需求数量", typeof(Decimal));
        //                    if (temp.Rows.Count > 0 && Convert.ToBoolean(xr["可购"].ToString()) == false)
        //                    {
        //                        a = a * Convert.ToDecimal(xr["数量"].ToString());
        //                        xr["需求数量"] = a;
        //                       // fun_dg_billofM(dt_return1, dt, temp, str_生产制令单号, str_预完工日期, str_制令数量, a);
        //                    }
        //                    else
        //                    {
        //                        dt_return1.ImportRow(xr);

        //                    }


        //                }
        //            }


        //            return dt_return1;
        //        }
     

        #endregion


        private void fun_数据()
        {
          

            //加载BOM
          string  s = @"  select  产品编码,产品名称,fx.存货分类 as 父项分类,fx.规格型号 as 父项规格,子项编码,子项名称,数量,zx.可购 as 子项可购 ,zx.存货分类 as 子项分类
    ,zx.规格型号 as 子项规格       from 基础数据物料BOM表 bom 
   left join 基础数据物料信息表 zx on bom.子项编码=zx.物料编码 
   left join 基础数据物料信息表 fx on bom.产品编码=fx.物料编码 ";
            dt_合 = new DataTable();
            dt_合 = CZMaster.MasterSQL.Get_DataTable(s,strconn);
            //加载物料的 库存，在途量，未领量




            //string s = "select  * from 生产记录生产制令表 where 关闭=0 and 完成=0 and 生产制令类型 <>'返修制令' ";
            //dt_制令 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //s = " select  产品编码,子项编码,数量,WIPType from 基础数据物料BOM表 where 主辅料='主料' ";
            //bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = @"select  base.物料名称,base.规格型号,base.可购,   kc.* from 基础数据物料信息表 base
                    left join (select 物料编码, sum(库存总数)库存总数,MAX(受订量) 受订量,MAX(在制量)在制量,max(未领量)未领量,max(在途量) as 在途量  from 仓库物料数量表
                 where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 = '仓库类别' and 布尔字段2 = 1) group by 物料编码)kc on kc.物料编码=base.物料编码 ";

            dtp_数 = new DataTable();
            dtp_数 = CZMaster.MasterSQL.Get_DataTable(s, strconn);




         //   s = @"select kc.物料编码,base.物料名称,base.规格型号,存货分类,库存总数,未领量,可购  from  
         //               (select  物料编码,sum(库存总数)库存总数,sum(未领量)未领量  from 仓库物料数量表
         //                    where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 ='仓库类别' and 布尔字段2=1) group by 物料编码)  kc 
         //         left join 基础数据物料信息表 base   on  base.物料编码=kc.物料编码 ";
         //dtp_数 = new DataTable();
         //   dtp_数 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            
            
//            string sff = @" select kc.物料编码,库存总数,未领量,在途量  from  
//                        (select  物料编码,sum(库存总数)库存总数,max(未领量)未领量,MAX(在途量) 在途量 from 仓库物料数量表
//                             where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 ='仓库类别' and 布尔字段2=1) group by 物料编码)  kc 
//                  left join 基础数据物料信息表 base   on  base.物料编码=kc.物料编码
//                  where 可购=1";
//            using (SqlDataAdapter da = new SqlDataAdapter(sff, strconn))
//            {
//                dtp_数 = new DataTable();
//                da.Fill(dtp_数);
//            }

            string sdd = "select 物料编码,采购周期 from 基础数据物料信息表";
            using (SqlDataAdapter da = new SqlDataAdapter(sdd, strconn))
            {
                dt_物料周期 = new DataTable();
                da.Fill(dt_物料周期);
            }

        }

  
        #endregion

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void date_后_EditValueChanged(object sender, EventArgs e)
        {

        }

    }
}
