using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 赵峰的DEMO
{
    public partial class Fm批量初始化 : Form
    {
        string strconn = CPublic.Var.strConn;
        DataTable dt;
        DataView dv;
        DataTable dt1;

        public Fm批量初始化()
        {
            InitializeComponent();
        }

        private void Fm批量初始化_Load(object sender, EventArgs e)
        {

        }

        private void fun_载入数据()
        {
            //string sql = "select 物料编码,物料名称,是否初始化,物料类型,图纸编号,规格型号 from 基础数据物料信息表 ";
            //string sql = "select 物料编码,物料名称,物料类型,图纸编号,规格型号 from 基础数据物料信息表 where 物料编码 not in(select 物料编码 from 仓库物料数量表)";
            string sql = "select 物料编码,物料名称,图纸编号,规格型号 from 仓库物料数量表";
            dt = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {

                da.Fill(dt);
                //dt.Columns.Add("请选择", typeof(bool));
                dt.Columns.Add("库存总数");
                dt.Columns.Add("库存数量");
                dt.Columns.Add("GUID");
                dt.Columns.Add("仓库名称");
                dt.Columns.Add("仓库描述");
                dt.Columns.Add("库位描述");
                dt.Columns.Add("盘点有效批次号");
                //dt.Columns.Add("仓库号");
                dt.Columns.Add("库位号");
                dt.Columns.Add("物料描述");
                dt.Columns.Add("盘点数量");
                dt.Columns.Add("BOM版本");
                dt.Columns.Add("盘点日期");

                dt.Columns.Add("有效总数");
                dt.Columns.Add("在途量");
                dt.Columns.Add("在制量");
                dt.Columns.Add("受订量");
                dt.Columns.Add("未领量");
                dt.Columns.Add("MRP计划采购量");
                dt.Columns.Add("MRP计划生产量");
                dt.Columns.Add("MRP库存锁定量");
                dt.Columns.Add("物品单价");
                dt.Columns.Add("成本");
                dt.Columns.Add("ID");
                //dv = new DataView(dt);
                //gc.DataSource = dv;
                //dv.RowFilter = "是否初始化='否'";
                gc.DataSource = dt;
            }
            //string sql1 = "select 仓库名称,仓库号 from 基础数据仓库主表 ";
            //dt1 = new DataTable();
            //using (SqlDataAdapter da = new SqlDataAdapter(sql1, strconn))
            //{
            //    da.Fill(dt1);
            //}
        }

        private void fun_计算()
        {
            //foreach (DataRowView drv in dv)
            foreach (DataRow drv in dt.Rows)
            {
                if (drv["物料类型"].ToString() == "原材料")
                {
                    drv["GUID"] = System.Guid.NewGuid();
                    drv["仓库名称"] = "原材料库";
                    drv["仓库描述"] = "原材料库";
                    drv["库位描述"] = "原材料库位";
                    drv["库存总数"] = 10;
                    drv["库存数量"] = 10;
                    drv["物料描述"] = "";
                    drv["盘点数量"] = 0;
                    drv["BOM版本"] = "";
                    drv["盘点日期"] = System.DateTime.Now;
                    drv["库位号"] = "01-0001";
                    //drv["盘点有效批次号"] = "初始化";

                    //drv["有效总数"] = 0;
                    //drv["在途量"] = 0;
                    //drv["在制量"] = 0;
                    //drv["受订量"] = 0;
                    //drv["未领量"] = 0;
                    //drv["MRP计划采购量"] = 0;
                    //drv["MRP计划生产量"] = 0;
                    //drv["MRP库存锁定量"] = 0;
                    //drv["物品单价"] = 0;
                    //drv["成本"] = 0;
                }
                if (drv["物料类型"].ToString() == "半成品")
                {
                    drv["GUID"] = System.Guid.NewGuid();
                    drv["仓库名称"] = "半成品库";
                    drv["仓库描述"] = "半成品库";
                    drv["库位描述"] = "半成品库位";
                    drv["库存总数"] = 20;
                    drv["库存数量"] = 20;
                    drv["库位号"] = "03-0001";
                    //drv["物料描述"] = "";
                    //drv["盘点数量"] = 0;
                    //drv["BOM版本"] = "";
                    //drv["盘点日期"] = System.DateTime.Now;
                    //drv["盘点有效批次号"] = "初始化";
                    //drv["有效总数"] = 0;
                    //drv["在途量"] = 0;
                    //drv["在制量"] = 0;
                    //drv["受订量"] = 0;
                    //drv["未领量"] = 0;
                    //drv["MRP计划采购量"] = 0;
                    //drv["MRP计划生产量"] = 0;
                    //drv["MRP库存锁定量"] = 0;
                    //drv["物品单价"] = 0;
                    //drv["成本"] = 0;
                }
                if (drv["物料类型"].ToString() == "成品")
                {
                    drv["GUID"] = System.Guid.NewGuid();
                    drv["仓库名称"] = "成品库";
                    drv["仓库描述"] = "成品库";
                    drv["库位描述"] = "成品库位";
                    drv["库存总数"] = 100;
                    drv["库存数量"] = 100;
                    drv["库位号"] = "02-0001";
                    //drv["物料描述"] = "";
                    //drv["盘点数量"] = 0;
                    //drv["BOM版本"] = "";
                    //drv["盘点日期"] = System.DateTime.Now;
                    //drv["盘点有效批次号"] = "初始化";
                    //drv["有效总数"] = 0;
                    //drv["在途量"] = 0;
                    //drv["在制量"] = 0;
                    //drv["受订量"] = 0;
                    //drv["未领量"] = 0;
                    //drv["MRP计划采购量"] = 0;
                    //drv["MRP计划生产量"] = 0;
                    //drv["MRP库存锁定量"] = 0;
                    //drv["物品单价"] = 0;
                    //drv["成本"] = 0;
                }
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_载入数据();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_计算();
        }


        private void fun_修改初始化状态()
        {
           
        }
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string sqll = "select * from 仓库物料表 where 1<>1";
            SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn);
            DataTable t = new DataTable();
            daa.Fill(t);
            foreach (DataRow r in dt.Rows)
            {
                string sql = string.Format("select * from 仓库物料表 where 物料编码='{0}' ", r["物料编码"].ToString().Trim());
                DataTable dt_临时 = CZMaster.MasterSQL.Get_DataTable(sql,strconn);
                if (dt_临时.Rows.Count > 0)
                {
                    continue;
                }
                else
                {
                    //r["是否初始化"] = "是";
                    DataRow dr = t.NewRow();
                    t.Rows.Add(dr);
                    dr["GUID"] = System.Guid.NewGuid();
                    dr["仓库名称"] = r["仓库名称"];
                    dr["物料名称"] = r["物料名称"];
                    dr["规格型号"] = r["规格型号"];
                    dr["图纸编号"] = r["图纸编号"];
                    dr["仓库描述"] = r["仓库描述"];
                    dr["库位描述"] = r["库位描述"];
                    dr["物料编码"] = r["物料编码"];
                    dr["库存数量"] = r["库存数量"];
                    dr["库位号"] = r["库位号"];
                    dr["物料描述"] = "";
                    dr["盘点数量"] = 0;
                    dr["BOM版本"] = "";
                    dr["盘点日期"] = System.DateTime.Now;
                    dr["盘点有效批次号"] = "初始化";
                }

                //if (t.Select(string.Format("物料编码 = '{0}'", r["物料编码"].ToString().Trim())).Length > 0)
                //{
                //    continue;
                //}
               
                //dr["有效总数"] = 0;
                //dr["在途量"] = 0;
                //dr["在制量"] = 0;
                //dr["受订量"] = 0;
                //dr["未领量"] = 0;
                //dr["MRP计划采购量"] = 0;
                //dr["MRP计划生产量"] = 0;
                //dr["MRP库存锁定量"] = 0;
                //dr["物品单价"] = 0;
                //dr["成本"] = 0;
            }
            new SqlCommandBuilder(daa);
            daa.Update(t);
     

            //string sql_初始化状态 = "select 物料编码,是否初始化 from 基础数据物料信息表 where 1<>1  ";
            //using (SqlDataAdapter da = new SqlDataAdapter(sql_初始化状态, strconn))
            //{
            //    new SqlCommandBuilder(da);
            //    da.Fill(t);
            //}
            MessageBox.Show("ok");
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string sqll = "select * from 仓库物料数量表 where 1<>1";
            SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn);
            DataTable t = new DataTable();
            daa.Fill(t);
           


            foreach (DataRow r in dt.Rows)
            {
                string sql = string.Format("select * from 仓库物料数量表 where 物料编码='{0}' ", r["物料编码"].ToString().Trim());
                DataTable dt_临时 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                if (dt_临时.Rows.Count > 0)
                {
                    continue;
                }
                else
                {


                    DataRow dr = t.NewRow();
                    
                    dr["GUID"] = System.Guid.NewGuid();
                    dr["有效总数"] = 0;
                    dr["在途量"] = 0;
                    dr["在制量"] = 0;
                    dr["受订量"] = 0;
                    dr["未领量"] = 0;
                    dr["MRP计划采购量"] = 0;
                    dr["MRP计划生产量"] = 0;
                    dr["MRP库存锁定量"] = 0;
                    dr["物品单价"] = 0;
                    dr["成本"] = 0;

                    dr["BOM版本"] = "";
                    dr["物料名称"] = r["物料名称"];
                    dr["规格型号"] = r["规格型号"];
                    dr["图纸编号"] = r["图纸编号"];
                    dr["物料编码"] = r["物料编码"];
                    dr["库存总数"] = r["库存总数"];
                    t.Rows.Add(dr);
                }
            }
            new SqlCommandBuilder(daa);
            daa.Update(t);
            MessageBox.Show("ok");
        
        }                   
    }
}
