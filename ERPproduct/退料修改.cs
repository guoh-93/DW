using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPproduct
{
    public partial class 退料修改 : UserControl
    {
        public 退料修改()
        {
            InitializeComponent();
        }
        DataTable dt_mx;
        DataTable dt_main;
        string s_tlh;
        string strcon = CPublic.Var.strConn;

        public 退料修改( string 待退料号)
        {
            InitializeComponent();
            s_tlh = 待退料号;


        }

        private void 退料修改_Load(object sender, EventArgs e)
        {
            try

            {
                string sql = string.Format("select * from 工单退料申请表 where  待退料号='{0}' ", s_tlh);
                dt_main = new DataTable();
                dt_main = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                sql = string.Format(@"select tx.*,tz.生产工单号     from 工单退料申请明细表  tx   
left join   工单退料申请表  tz  on tz.待退料号 = tx.待退料号 

where  tx.待退料号='{0}' and tx.完成=0 ", s_tlh);
                dt_mx = new DataTable();
                dt_mx = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                dt_mx.Columns.Add("已领数量",typeof(decimal));

                foreach (DataRow dr in dt_mx.Rows)
                {
                    //string sq = string.Format("select * from  生产记录生产工单待领料明细表   where 生产工单号='{0}' and 物料编码 ='{1}'", dr["生产工单号"], dr["物料编码"]);

                    string sq = string.Format(@"select a.*, 人事基础部门表.部门名称 from(select sdlmx.*,
                           kc.有效总数, kc.库存总数 from 生产记录生产工单待领料明细表 sdlmx
 
                            left
                                            join 仓库物料数量表 kc on kc.物料编码 = sdlmx.物料编码 and sdlmx.仓库号 = kc.仓库号
            
                                       left
                                            join 基础数据物料信息表 base on base.物料编码 = sdlmx.物料编码
                           where   sdlmx.生产工单号 = '{0}'  and sdlmx.物料编码='{1}' ) a
                           left join 人事基础部门表 on 人事基础部门表.部门编号 = a.生产车间   where 已领数量 > 0", dr["生产工单号"], dr["物料编码"]);

                    DataTable dt = CZMaster.MasterSQL.Get_DataTable(sq,strcon);
                    if (dt.Rows.Count>0)
                    {
                        dr["已领数量"] = dt.Rows[0]["已领数量"];

                    }
                }

                dataBindHelper1.DataFormDR(dt_main.Rows[0]);
                gridControl2.DataSource = dt_mx;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        private void fun_save()
#pragma warning restore IDE1006 // 命名样式
        {


            gridView2.CloseEditor();
            this.BindingContext[dt_mx].EndCurrentEdit();
            foreach (DataRow r in dt_mx.Rows )
            {
                if (r.RowState == DataRowState.Deleted)
                {

                    continue;
                }


            }



            ////Waiting for return order material
            //DateTime t = CPublic.Var.getDatetime();
            //string str_待退料号 = string.Format("WR{0}{1:00}{2:0000}",
            //                                   t.Year, t.Month, CPublic.CNo.fun_得到最大流水号("WR", t.Year, t.Month));
            //// 1.生成退料申请单 
            //dt_主 = new DataTable();
            //dt_主 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            //dt_mx = new DataTable();
            //dt_mx = CZMaster.MasterSQL.Get_DataTable(sql_mx, strcon);
            //DataRow r_m = dt_主.NewRow();
            //dt_主.Rows.Add(r_m);
            //r_m["待退料号"] = str_待退料号;
            //r_m["生产工单号"] = textBox2.Text;
            //r_m["车间"] = str_车间;
            //r_m["产品编号"] = textBox3.Text;
            //r_m["产品名称"] = textBox7.Text;
            //r_m["操作人"] = textBox6.Text;
            //r_m["操作时间"] = t;
            //r_m["备注"] = textBox4.Text;
            //r_m["退料类型"] = "工单退料";
            //int i = 0;
            //foreach (DataRow dr in dtM.Rows)
            //{
            //    if (dr["选择"].Equals(true))
            //    {
            //        i++;
            //        DataRow r_mx = dt_mx.NewRow();

            //        r_mx["待退料号"] = str_待退料号;
            //        r_mx["待退料明细号"] = str_待退料号 + "-" + i.ToString("00");
            //        r_mx["POS"] = i;
            //        r_mx["物料编码"] = dr["物料编码"];
            //        r_mx["物料名称"] = dr["物料名称"];
            //        r_mx["仓库号"] = dr["仓库号"];
            //        //if (复状态 != 1)
            //        //{
            //        r_mx["仓库名称"] = dr["仓库名称"];
            //        r_mx["需退料数量"] = dr["输入退料数量"];
            //        dt_mx.Rows.Add(r_mx);
            //        //}
            //    }
            //}
            string sql = "select * from 工单退料申请表 where 1<>1";
            string sql_mx = "select * from 工单退料申请明细表 where 1<>1";
            dataBindHelper1.DataToDR(dt_main.Rows[0]);
            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction mt = conn.BeginTransaction("工单退料申请");
            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn, mt);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt_main);

                cmd = new SqlCommand(sql_mx, conn, mt);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt_mx);

                mt.Commit();
                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {
                mt.Rollback();
                throw new Exception("退料申请失败" + ex.Message);
            }



        }
#pragma warning disable IDE1006 // 命名样式
//        private void fun_check()
//#pragma warning restore IDE1006 // 命名样式
//        {

//            string s = string.Format("select  * from  [工单退料申请表]  where 生产工单号='{0}' and 完成=0 and 作废=0 ", textBox2.Text);
//            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
//            if (dt.Rows.Count > 0)
//            {
//                throw new Exception("该工单尚有未入库的退料申请,请通知仓库审核后再进行操作。");
//                //if (MessageBox.Show("该工单尚有未入库的退料申请,请通知仓库审核后再进行操作。", "警告", MessageBoxButtons.OKCancel)!= DialogResult.OK)
//                //{
//                //    throw new Exception("已取消提交");
//                //}
//            }
//            DataView dv = new DataView(dtM);
//            dv.RowFilter = "选择=1";
//            if (dv.Count == 0)
//            {
//                throw new Exception("未选择需退料的物料");
//            }

//            foreach (DataRow r in dtM.Rows)
//            {
//                if (r["选择"].Equals(true))
//                {
//                    decimal a = 0;
//                    try
//                    {
//                        a = Convert.ToDecimal(r["输入退料数量"]);
//                    }
//                    catch
//                    {
//                        throw new Exception("请正确输入退料数量格式");
//                    }
//                    if (a <= 0)
//                    {
//                        throw new Exception("退料数量不能小于0,请重新输入");
//                    }
//                    if (a > Convert.ToDecimal(r["已领数量"]))
//                    {
//                        throw new Exception("输入的退料数量大于已领料数量");
//                    }
//                }
//            }
//        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_save();
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();

        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try

            {
             DataRow  drM = (this.BindingContext[gridControl2.DataSource].Current as DataRowView).Row;
                drM.Delete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
