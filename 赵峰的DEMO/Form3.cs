using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace 赵峰的DEMO
{
    public partial class Form3 : Form

    {
        string strconn = CPublic.Var.strConn;
        DataTable dtM;
        DataTable dt_制令;
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sql = "select * from 生产记录生产工单表";
            dtM = new DataTable();
            using(SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                da.Fill(dtM);
            }
            string sql1 = "select * from 生产记录生产制令表";
            dt_制令 = new DataTable();
            using (SqlDataAdapter da1 = new SqlDataAdapter(sql1, strconn))
            {
                da1.Fill(dt_制令);
            }
            DateTime t = CPublic.Var.getDatetime();
            foreach (DataRow dr in dtM.Rows)
            {
                DataRow dr_制令 = dt_制令.NewRow();
                dt_制令.Rows.Add(dr_制令);
                dr_制令["GUID"] = System.Guid.NewGuid();
                dr_制令["生产制令单号"] = string.Format("PM{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                            CPublic.CNo.fun_得到最大流水号("PM", t.Year, t.Month));
                dr_制令["生产制令类型"] = "标准类型";
                dr_制令["物料编码"] = dr["物料编码"];
                dr_制令["物料名称"] = dr["物料名称"];
                dr_制令["规格型号"] = dr["规格型号"];
                dr_制令["生产车间"] = dr["生产车间"];
                dr_制令["制令数量"] = dr["生产数量"];
                dr_制令["已排单数量"] = dr["生产数量"];
                dr_制令["未排单数量"] = 0;
                dr_制令["预开工日期"] = dr["预计开工日期"];
                dr_制令["预完工日期"] = dr["预计完工日期"];
                dr_制令["日期"] = dr["制单日期"];
                dr_制令["操作人员"] = dr["制单人员"];
                dr_制令["制单人员"] = dr["制单人员"];
                dr_制令["操作人员ID"] = dr["制单人员ID"];
                dr_制令["制单人员ID"] = dr["制单人员ID"];
                dr_制令["生效"] = dr["生效"];
                dr_制令["生效人员"] = dr["生效人"];
                dr_制令["生效人员ID"] = dr["生效人ID"];
                dr_制令["生效日期"] = dr["生效日期"];
                dr_制令["完成"] = dr["完成"];
                dr_制令["关闭"] = dr["关闭"];
                dr_制令["仓库号"] = dr["仓库号"];
                dr_制令["仓库名称"] = dr["仓库名称"];
                dr["生产制令单号"] = dr_制令["生产制令单号"];
            }
            string sql2 = "select * from 生产记录生产制令表 where 1<>1";
            SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
            new SqlCommandBuilder(da2);
            da2.Update(dt_制令);
            sql2 = "select * from 生产记录生产工单表 where 1<>1";
            da2 = new SqlDataAdapter(sql2, strconn);
            new SqlCommandBuilder(da2);
            da2.Update(dtM);
            MessageBox.Show("保存成功");
        }

        private void button2_Click(object sender, EventArgs e)
        {

            string sql = "select * from 生产记录生产工单表";
            dtM = new DataTable();
            using (SqlDataAdapter da333 = new SqlDataAdapter(sql, strconn))
            {
                da333.Fill(dtM);
            }
            sql = "select * from 生产记录生产工单待领料主表";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            DataTable dt_待领料主表 = new DataTable();
            da.Fill(dt_待领料主表);
 //           sql = @"select * from select MoCode+'-'+CONVERT(nvarchar(50), gdmx.SortSeq) 工单号,dll.* from  [192.168.20.150].UFDATA_008_2018.dbo.mom_order gd
 //left join[192.168.20.150].UFDATA_008_2018.dbo.mom_orderdetail gdmx  on gd.MoId = gdmx.MoId
 //left join[192.168.20.150].UFDATA_008_2018.dbo.mom_moallocate dll on gdmx.modid = dll.modid
 //where MoCode+'-' + CONVERT(nvarchar(50), gdmx.SortSeq) in (select  生产工单号 from 生产记录生产工单表 )";
 //           SqlDataAdapter da1 = new SqlDataAdapter(sql, strconn);
 //           DataTable dt = new DataTable();
 //           da1.Fill(dt);
            sql = "select  * from 生产记录生产工单待领料明细表 ";
            da = new SqlDataAdapter(sql, strconn);
            DataTable dt_待领料明细表 = new DataTable();
            da.Fill(dt_待领料明细表);
            DateTime t = CPublic.Var.getDatetime();
            foreach (DataRow dr in dtM.Rows)
            {
                string sql3 = @"select MoCode+'-'+CONVERT(nvarchar(50), gdmx.SortSeq) 工单号,dll.* from  [192.168.20.150].UFDATA_008_2018.dbo.mom_order gd
 left join[192.168.20.150].UFDATA_008_2018.dbo.mom_orderdetail gdmx  on gd.MoId = gdmx.MoId
 left join[192.168.20.150].UFDATA_008_2018.dbo.mom_moallocate dll on gdmx.modid = dll.modid
 where MoCode+'-' + CONVERT(nvarchar(50), gdmx.SortSeq) in (select  生产工单号 from 生产记录生产工单表 ) and MoCode+'-'+CONVERT(nvarchar(50), gdmx.SortSeq) = '" + dr["生产工单号"]+"'";
                SqlDataAdapter da1 = new SqlDataAdapter(sql3, strconn);
                DataTable dt = new DataTable();
                da1.Fill(dt);
                DataRow dr_待领料主 = dt_待领料主表.NewRow();
                dt_待领料主表.Rows.Add(dr_待领料主);
                dr_待领料主["待领料单号"] = string.Format("DL{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day,CPublic.CNo.fun_得到最大流水号("DL", t.Year, t.Month)); 
                dr_待领料主["生产工单号"] = dr["生产工单号"];
                dr_待领料主["生产工单类型"] = dr["生产工单类型"];
                dr_待领料主["生产制令单号"] = dr["生产制令单号"];
                dr_待领料主["产品编码"] = dr["物料编码"];
                dr_待领料主["产品名称"] = dr["物料名称"];
                dr_待领料主["生产数量"] = dr["生产数量"];
                dr_待领料主["规格型号"] = dr["规格型号"];
                dr_待领料主["生产车间"] = dr["生产车间"];
                dr_待领料主["创建日期"] = dr["制单日期"];
                dr_待领料主["制单人员ID"] = dr["制单人员ID"];
                dr_待领料主["制单人员"] = dr["制单人员"];
                dr_待领料主["仓库号"] = dr["仓库号"];
                dr_待领料主["仓库名称"] = dr["仓库名称"];
                int i = 1;
                foreach(DataRow dr1 in dt.Rows)
                {

                    DataRow dr_待领料明细 = dt_待领料明细表.NewRow();
                    dt_待领料明细表.Rows.Add(dr_待领料明细);
                    dr_待领料明细["待领料单号"] = dr_待领料主["待领料单号"];
                    dr_待领料明细["待领料单明细号"] = dr_待领料主["待领料单号"]+"-"+i.ToString("00");
                    dr_待领料明细["生产工单号"] = dr_待领料主["生产工单号"];
                    dr_待领料明细["生产工单类型"] = dr_待领料主["生产工单类型"];
                    dr_待领料明细["生产制令单号"] = dr_待领料主["生产制令单号"];
                    dr_待领料明细["物料编码"] = dr1["InvCode"];
                    //dr_待领料明细["物料名称"] = dr1["InvCode"];
                    //dr_待领料明细["规格型号"] = dr1["InvCode"];

                    dr_待领料明细["生产车间"] = dr_待领料主["生产车间"];
                    dr_待领料明细["待领料总量"] = dr1["Qty"];
                    dr_待领料明细["已领数量"] = dr1["IssQty"];
                    dr_待领料明细["未领数量"] = Convert.ToDecimal(dr1["Qty"]) - Convert.ToDecimal(dr1["IssQty"]);
                    if((Convert.ToDecimal(dr1["Qty"]) - Convert.ToDecimal(dr1["IssQty"])) < 0)
                    {
                        dr_待领料明细["未领数量"] = 0;
                    }
                    dr_待领料明细["生效"] = 1;
                    dr_待领料明细["生效人员"] = dr["生效人"];
                    dr_待领料明细["生效人员ID"] = dr["生效人ID"];
                    dr_待领料明细["生效日期"] = dr["生效日期"];
                    dr_待领料明细["制单人员ID"] = dr_待领料主["制单人员ID"];
                    dr_待领料明细["制单人员"] = dr_待领料主["制单人员"];
                    dr_待领料明细["创建日期"] = dr_待领料主["创建日期"];
                    if(Convert.ToUInt32(dr1["Qty"]) == Convert.ToUInt32(dr1["IssQty"]))
                    {
                        dr_待领料明细["完成"] = 1;
                    }
                    else
                    {
                        dr_待领料明细["完成"] = 0;
                    }
                    dr_待领料明细["仓库号"] = dr1["Whcode"];
                   // dr_待领料明细["仓库名称"] = dr_待领料主["仓库名称"];
                    i++;
                }


            }
            
            string sql111 = "select * from 生产记录生产工单待领料主表 where 1<>1";
            da = new SqlDataAdapter(sql111, strconn);
            new SqlCommandBuilder(da);
            da.Update(dt_待领料主表);
            string sql222 = "select * from 生产记录生产工单待领料明细表 where 1<>1";
            da = new SqlDataAdapter(sql222, strconn);
            new SqlCommandBuilder(da);
            da.Update(dt_待领料明细表);
            MessageBox.Show("保存成功");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string sql = "select * from 生产记录生产工单待领料主表";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            DataTable dt_待领料主 = new DataTable();
            da.Fill(dt_待领料主);
            sql = "select * from 生产记录生产领料单主表";
            da = new SqlDataAdapter(sql, strconn);
            DataTable dt_领料主 = new DataTable();
            da.Fill(dt_领料主);
            sql = "select * from 生产记录生产领料单明细表";
            da = new SqlDataAdapter(sql, strconn);
            DataTable dt_领料明细 = new DataTable();
            da.Fill(dt_领料明细);
            //sql = "select * from 生产记录生产工单待领料明细表";
            //da = new SqlDataAdapter(sql, strconn);
            //DataTable dt_待领料明细 = new DataTable();
            //da.Fill(dt_待领料明细);
            int flag = 0;
            DateTime t = CPublic.Var.getDatetime();
            foreach (DataRow dr_待领料主 in dt_待领料主.Rows)
            {
                string NO = string.Format("ML{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("ML", t.Year, t.Month));
                sql = "select * from 生产记录生产工单待领料明细表 where 待领料单号 = '"+dr_待领料主["待领料单号"]+"'";
                da = new SqlDataAdapter(sql, strconn);
                DataTable dt_待领料明细 = new DataTable();
                da.Fill(dt_待领料明细);
                int POS = 1;
                foreach (DataRow dr_待领料明细 in dt_待领料明细.Rows)
                {
                    if (Convert.ToInt32(dr_待领料明细["已领数量"]) > 0)
                    {
                        DataRow dr_领料明细 = dt_领料明细.NewRow();
                        dt_领料明细.Rows.Add(dr_领料明细);
                        dr_领料明细["GUID"] = System.Guid.NewGuid();
                        dr_领料明细["领料出库单号"] = NO;
                        dr_领料明细["POS"] = POS;
                        dr_领料明细["领料出库明细号"] = dr_领料明细["领料出库单号"]+ "-" + POS++.ToString("00");
                        dr_领料明细["待领料单明细号"] = dr_待领料明细["待领料单明细号"];
                        dr_领料明细["生产制令单号"] = dr_待领料明细["生产制令单号"];
                        dr_领料明细["生产工单号"] = dr_待领料明细["生产工单号"];
                        dr_领料明细["生产工单类型"] = dr_待领料明细["生产工单类型"];
                        dr_领料明细["物料编码"] = dr_待领料明细["物料编码"];
                        dr_领料明细["领料数量"] = dr_待领料明细["待领料总量"];
                        dr_领料明细["已领数量"] = dr_待领料明细["已领数量"];
                        dr_领料明细["未领数量"] = dr_待领料明细["未领数量"];
                        dr_领料明细["生效"] = 1;
                        dr_领料明细["生效人员ID"] = dr_待领料明细["生效人员ID"];
                        dr_领料明细["生效人员"] = dr_待领料明细["生效人员"];
                        dr_领料明细["生效日期"] = dr_待领料明细["生效日期"];
                        if(Convert.ToInt32(dr_待领料明细["待领料总量"]) == Convert.ToInt32(dr_待领料明细["已领数量"]))
                        {
                            dr_领料明细["完成"] = 1;
                        }
                        else
                        {
                            dr_领料明细["完成"] = 0;
                        }
                        flag = 1;
                    }
                }
                if (flag == 1)
                {
                    DataRow dr_领料主 = dt_领料主.NewRow();
                    dt_领料主.Rows.Add(dr_领料主);
                    dr_领料主["GUID"] =  System.Guid.NewGuid();
                    dr_领料主["领料出库单号"] = NO;
                    dr_领料主["待领料单号"] = dr_待领料主["待领料单号"];
                    dr_领料主["生产制令单号"] = dr_待领料主["生产制令单号"];
                    dr_领料主["生产工单号"] = dr_待领料主["生产工单号"];
                    dr_领料主["生产工单类型"] = dr_待领料主["生产工单类型"];
                    dr_领料主["物料编码"] = dr_待领料主["产品编码"];
                    dr_领料主["物料名称"] = dr_待领料主["产品名称"];
                    dr_领料主["规格型号"] = dr_待领料主["规格型号"];
                    dr_领料主["生产数量"] = dr_待领料主["生产数量"];
                    dr_领料主["生产车间"] = dr_待领料主["生产车间"];
                    dr_领料主["领料类型"] = dr_待领料主["领料类型"];
                    dr_领料主["创建日期"] = dr_待领料主["创建日期"];
                    NO = string.Format("ML{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("ML", t.Year, t.Month));
                    flag = 0;

                }
               

            }
            sql = "select * from 生产记录生产领料单主表 where 1<>1";
            da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dt_领料主);
            sql = "select * from 生产记录生产领料单明细表 where 1<>1";
            da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dt_领料明细);
            MessageBox.Show("1111111");
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }
}
