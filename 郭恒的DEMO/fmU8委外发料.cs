using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace 郭恒的DEMO
{
    public partial class fmU8委外发料 : Form
    {
        string strcon = CPublic.Var.strConn;
        string stcon_u8 = CPublic.Var.geConn("DW");



        public fmU8委外发料()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string s = "select  *  from OM_MOMain";
            DataTable t_u8_委外订单 = CZMaster.MasterSQL.Get_DataTable(s, stcon_u8);
            s = @"select  ccode,iVouchRowNo,cInvStd,cInvName,cInvCName ,OM_MOMaterials.*  from OM_MOMaterials 
left join   OM_MODetails  on OM_MOMaterials.MoDetailsID=OM_MODetails.MoDetailsID
left join   OM_MOMain  on OM_MOMain.MOID=OM_MODetails.MOID
left join inventory on inventory.cInvCode=OM_MOMaterials.cInvCode
left join  InventoryClass on InventoryClass.cInvCCode=OM_MOMaterials.cWhCode";
            DataTable dt_委外材料清单 = CZMaster.MasterSQL.Get_DataTable(s, stcon_u8);
            s = "select  * from 其他出入库申请主表 where 1=2";
            DataTable t_其他申请主 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select  * from 其他出入库申请子表 where 1=2";
            DataTable t_其他申请子 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            foreach (DataRow dr in t_u8_委外订单.Rows)
            {
                DataRow r = t_其他申请主.NewRow();
                r["GUID"] = System.Guid.NewGuid();
                r["出入库申请单号"] = dr["MOID"];
                r["申请日期"] = dr["dDate"];
                r["申请类型"] = "其他出库";
                r["备注"] = dr["cCode"];
                r["操作人员编号"] = dr["cPersonCode"];
                r["操作人员"] = dr["cMaker"];
                r["生效人员编号"] = dr["cVerifier"];
                r["原因分类"] = "委外加工";
                r["生效"] = 1;
                r["生效日期"] = dr["dVerifyDate"];
                r["审核"] = 1;
                r["审核日期"] = dr["dVerifyDate"]; ;


                t_其他申请主.Rows.Add(r);
                DataRow[] r_zi = dt_委外材料清单.Select(string.Format("MOID='{0}'",dr["MOID"]));
                int x = 1;
                foreach (DataRow rr in r_zi)
                {
                    DataRow r_申请子 = t_其他申请子.NewRow();
                    r_申请子["GUID"] = System.Guid.NewGuid();
                    r_申请子["出入库申请单号"] = rr["MOID"];
                    r_申请子["出入库申请明细号"] = rr["MOID"]+"-"+x.ToString("00");
                    r_申请子["POS"] =x++;
                    r_申请子["物料编码"] = rr["cInvCode"];
                    r_申请子["数量"] = rr["iQuantity"];
                    r_申请子["物料名称"] = rr["cInvName"];
                    r_申请子["规格型号"] = rr["cInvStd"];
                    r_申请子["备注"] = rr["ccode"]+"-"+Convert.ToInt32(rr["iVouchRowNo"]).ToString("00");
                    r_申请子["生效"] = 1;
                    r_申请子["生效日期"] = dr["dVerifyDate"];
                    r_申请子["生效人员编号"] = dr["cVerifier"];
                    r_申请子["委外备注2"] = "1";
                    r_申请子["仓库号"] = rr["cWhCode"];
                    r_申请子["仓库名称"] = rr["cInvCName"];
                    decimal dec = 0;
                    if (rr["iSendQTY"] != DBNull.Value )
                        dec = Convert.ToDecimal(rr["iSendQTY"]);
                    r_申请子["已完成数量"] = dec;
                    if (dec == Convert.ToDecimal(rr["iQuantity"]))
                        r_申请子["完成"] = 1;
                    t_其他申请子.Rows.Add(r_申请子);
                }
            }

            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("save");
            try
            {
                s = "select  * from 其他出入库申请主表 where 1<>1";
                SqlCommand cmm = new SqlCommand(s, conn, ts);
                SqlDataAdapter da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(t_其他申请主);

                s = "select  * from 其他出入库申请子表 where 1<>1";
                cmm = new SqlCommand(s, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(t_其他申请子);


                ts.Commit();
              s=@"update 其他出入库申请子表 set 仓库名称 = 基础数据物料信息表.仓库名称, 仓库号 = 基础数据物料信息表.仓库号 from 基础数据物料信息表
       where 其他出入库申请子表.仓库名称 = '' and 其他出入库申请子表.物料编码 = 基础数据物料信息表.物料编码 and left(出入库申请单号,4) not in ('QWSQ', 'DWQC')";
                CZMaster.MasterSQL.ExecuteSQL(s, strcon);
            }
            catch (Exception ex)
            {
                ts.Rollback();
                MessageBox.Show(ex.Message);

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DateTime t = new DateTime (2019,4,30);
            string s= string.Format(@"select mx.* from 其他出入库申请子表 mx
           left  join 其他出入库申请主表 zb on zb.出入库申请单号 = mx.出入库申请单号
          where 原因分类 = '委外加工' and 申请日期<'2019-5-1'  and 已完成数量> 0");
            DataTable t_mx = CZMaster.MasterSQL.Get_DataTable(s,strcon);

        s= @" select * from 其他出入库申请主表 where 原因分类 = '委外加工' and 申请日期<'2019-5-1'
         and 出入库申请单号 in (select 出入库申请单号 from 其他出入库申请子表 where 出入库申请单号 in 
         (select 出入库申请单号 from 其他出入库申请主表 where 原因分类 = '委外加工' and 申请日期<'2019-5-1') and 已完成数量> 0)";
            DataTable t_z = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            s = "select  * from 其他出库主表 where 1=2";
            DataTable t_其他出主 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select  * from 其他出库子表 where 1=2";
            DataTable t_其他出子 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            foreach (DataRow dr in t_z.Rows)
            {
                DataRow r = t_其他出主.NewRow();
                r["GUID"] = System.Guid.NewGuid();
                string s_Dh= string.Format("QT{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
               t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("QT", t.Year, t.Month).ToString("0000"));
                r["出库日期"] = t;
                r["出库类型"] = "其他出库";
                r["其他出库单号"] = s_Dh;
                r["备注"] = "U8导入委外材料出库";
                r["操作人员编号"] =dr["操作人员编号"];
                r["操作人员"] = dr["操作人员编号"];
                r["生效人员编号"] = "U8导入"+dr["生效人员编号"];
                r["生效"] = 1;
                r["生效日期"] =t;
                r["创建日期"] =  t ;
                r["出入库申请单号"] = dr["出入库申请单号"];
                t_其他出主.Rows.Add(r);
                DataRow[] r_zi = t_mx.Select(string.Format("出入库申请单号='{0}'", dr["出入库申请单号"]));
                int x = 1;
                foreach (DataRow rr in r_zi)
                {
                    if (Convert.ToDecimal(rr["已完成数量"]) > 0)
                    {
                        DataRow r_其他出子 = t_其他出子.NewRow();
                        r_其他出子["GUID"] = System.Guid.NewGuid();
                        r_其他出子["出入库申请单号"] = rr["出入库申请单号"];
                        r_其他出子["出入库申请明细号"] = rr["出入库申请明细号"];
                        r_其他出子["其他出库单号"] = s_Dh;
                        r_其他出子["其他出库明细号"] = s_Dh + "-" + x.ToString("00");
                        r_其他出子["POS"] = x++;
                        r_其他出子["物料编码"] = rr["物料编码"];
                        r_其他出子["数量"] = rr["已完成数量"];
                        r_其他出子["物料名称"] = rr["物料名称"];
                        r_其他出子["规格型号"] = rr["规格型号"];
                        r_其他出子["备注"] = rr["备注"];
                        r_其他出子["生效"] = 1;
                        r_其他出子["生效日期"] = t;
                        r_其他出子["生效人员编号"] = rr["生效人员编号"];
                        t_其他出子.Rows.Add(r_其他出子);
                    }
                }

            }

            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("save");
            try
            {
                s = "select  * from 其他出库主表 where 1<>1";
                SqlCommand cmm = new SqlCommand(s, conn, ts);
                SqlDataAdapter da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(t_其他出主);

                s = "select  * from 其他出库子表 where 1<>1";
                cmm = new SqlCommand(s, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(t_其他出子);
                ts.Commit();
        
            }
            catch (Exception ex)
            {
                ts.Rollback();
                MessageBox.Show(ex.Message);

            }
        }

        //
        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
