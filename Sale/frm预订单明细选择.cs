using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace ERPSale
{
    public partial class frm预订单明细选择 : Form
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dt_预订单明细;
        DataTable dt_勾选明细;
        public bool flag = false;
        public DataTable dt_ydd_gxmx = null;
        public DataTable dt_ydd_mx = null;
        DataTable dtmx;
        public frm预订单明细选择()
        {
            InitializeComponent();
        }

        public frm预订单明细选择(DataTable dtP)
        {
            InitializeComponent();
            dtmx = dtP;

        }

        private void frm预订单明细选择_Load(object sender, EventArgs e)
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(this.panel2, this.Name, cfgfilepath);
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_load()
        {
            string sql = "";
            if (CPublic.Var.LocalUserTeam == "管理员权限" || CPublic.Var.LocalUserID == "admin")
            {
                sql = @"  with tt as ( select 销售预订单明细号, sum(数量)数量 from (
                              select   销售预订单明细号, sum(数量)数量 from 销售记录销售订单明细表 aa
                              left join 销售记录销售订单主表 bb on aa.销售订单号 = bb.销售订单号
                              where bb.作废= 0 and bb.关闭= 0 and bb.完成= 0 and bb.审核= 0 and bb.待审核= 0 and bb.生效 = 0
                              and aa.作废= 0 and aa.关闭= 0 and aa.明细完成= 0 and aa.生效= 0 and 销售预订单明细号 <>''   group by 销售预订单明细号
                              union   
                              select 销售预订单明细号,sum(申请数量)数量 from 借还申请表附表 cc 
                              left join 借还申请表 dd on cc.申请批号 = dd.申请批号
                              where dd.作废 = 0 and dd.审核=0 and dd.提交审核 = 0 and cc.作废=0 and 销售预订单明细号 <>''
                              group by 销售预订单明细号) aaa group by 销售预订单明细号) 

                              select a.*,b.部门名称,b.备注 as 表头备注,b.制单人,b.业务员 ,isnull(c.数量,0)锁定数量,b.制单日期 from 销售预订单明细表 a
                              left join 销售预订单主表 b on a.销售预订单号 = b.销售预订单号
                              left join tt c on a.销售预订单明细号 = c.销售预订单明细号
                              where a.作废 = 0 and a.完成 = 0 and a.关闭 = 0 and b.作废 = 0 and b.审核 = 1 and b.关闭 = 0 and b.完成 = 0    ";
            }
            else
            {
                sql = string.Format(@" with tt as ( select 销售预订单明细号, sum(数量)数量 from (
                              select   销售预订单明细号, sum(数量)数量 from 销售记录销售订单明细表 aa
                              left join 销售记录销售订单主表 bb on aa.销售订单号 = bb.销售订单号
                              where bb.作废= 0 and bb.关闭= 0 and bb.完成= 0 and bb.审核= 0 and bb.待审核= 0 and bb.生效 = 0
                              and aa.作废= 0 and aa.关闭= 0 and aa.明细完成= 0 and aa.生效= 0 and 销售预订单明细号 <>''   group by 销售预订单明细号
                              union   
                              select 销售预订单明细号,sum(申请数量)数量 from 借还申请表附表 cc 
                              left join 借还申请表 dd on cc.申请批号 = dd.申请批号
                              where dd.作废 = 0 and dd.审核=0 and dd.提交审核 = 0 and cc.作废=0 and 销售预订单明细号 <>''
                              group by 销售预订单明细号) aaa group by 销售预订单明细号) 

                              select a.*,b.部门名称,b.备注 as 表头备注,b.制单人,b.业务员 ,isnull(c.数量,0)锁定数量,b.制单日期 from 销售预订单明细表 a
                              left join 销售预订单主表 b on a.销售预订单号 = b.销售预订单号
                              left join tt c on a.销售预订单明细号 = c.销售预订单明细号
                              where a.作废 = 0 and a.完成 = 0 and a.关闭 = 0 and b.作废 = 0 and b.审核 = 1 and b.关闭 = 0 and b.完成 = 0 and 部门名称 = '{0}' ", CPublic.Var.localUser部门名称);
            }
            dt_预订单明细 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            DataColumn dc = new DataColumn("选择", typeof(bool));
            dc.DefaultValue = false;
            dt_预订单明细.Columns.Add(dc);
            dt_预订单明细.Columns.Add("可转数量", typeof(decimal));
            foreach(DataRow dr in dt_预订单明细.Rows)
            {
                DataRow[] dr_mx = dtmx.Select(string.Format("销售预订单明细号 = '{0}'", dr["销售预订单明细号"]));
                if (dr_mx.Length > 0)
                {
                    dr["可转数量"] = Convert.ToDecimal(dr["未转数量"]) - Convert.ToDecimal(dr["锁定数量"]) - Convert.ToDecimal(dr_mx[0]["数量"]);
                }
                else
                {
                    dr["可转数量"] = Convert.ToDecimal(dr["未转数量"]) - Convert.ToDecimal(dr["锁定数量"]);
                }
                 
            }
            DataView dv = new DataView(dt_预订单明细);
            dv.RowFilter = "可转数量>0";
            gridControl1.DataSource = dv;
        }

         

        

         

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try

            {

                flag = false;
                gridView2.CloseEditor();
                this.BindingContext[dt_勾选明细].EndCurrentEdit();

                fun_check();
                //DataView dv = new DataView(dt_物料下拉框);
                //dv.RowFilter = "选择 = True";
               // DataTable dt_cun = new DataTable();
              //  dt_cun = dv.ToTable();
                if (dt_勾选明细.Rows.Count > 0)
                {
                    flag = true;
                    dt_ydd_gxmx = new DataTable();
                    dt_ydd_gxmx = dt_勾选明细.Copy();

                    dt_ydd_mx = new DataTable();
                    dt_ydd_mx = dt_预订单明细.Copy();

                    foreach(DataRow dr in dt_勾选明细.Rows)
                    {
                        DataRow[] dr_mx = dt_ydd_mx.Select(string.Format("销售预订单明细号 = '{0}'", dr["销售预订单明细号"]));
                        dr_mx[0]["可转数量"] = Convert.ToDecimal(dr_mx[0]["可转数量"]) - Convert.ToDecimal(dr["此次转单数量"]);
                    }
                }


                barLargeButtonItem2_ItemClick(null, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_check()
        {
            foreach (DataRow dr in dt_勾选明细.Rows)
            {
                DataRow[] dr_mx = dt_预订单明细.Select(string.Format("销售预订单明细号 = '{0}'", dr["销售预订单明细号"]));
                if (dr_mx.Length > 0)
                {
                    if(Convert.ToDecimal(dr["此次转单数量"])> Convert.ToDecimal(dr_mx[0]["可转数量"]))
                    {
                        throw new Exception("此次转单数量超出预订单可转数量，请确认");
                    }
                }
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
        {
            gridView1.CloseEditor();
            this.BindingContext[gridView1].EndCurrentEdit();
            try
            {
                string sql = "select 销售预订单号,销售预订单明细号,物料编码,物料名称,规格型号,数量 as 预订单数量,转换订单数量,未转数量,数量-转换订单数量 as 此次转单数量 from  销售预订单明细表 where 1<>1";
                dt_勾选明细 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                foreach (DataRow dr in dt_预订单明细.Rows)
                {
                    if (Convert.ToBoolean(dr["选择"]) == true)
                    {
                        DataRow dr_勾选 = dt_勾选明细.NewRow();
                        dt_勾选明细.Rows.Add(dr_勾选);
                        dr_勾选["销售预订单号"] = dr["销售预订单号"];
                        dr_勾选["销售预订单明细号"] = dr["销售预订单明细号"];
                        dr_勾选["物料编码"] = dr["物料编码"];
                        dr_勾选["物料名称"] = dr["物料名称"];
                        dr_勾选["规格型号"] = dr["规格型号"];
                        dr_勾选["预订单数量"] = Convert.ToDecimal(dr["数量"]);
                        dr_勾选["转换订单数量"] = Convert.ToDecimal(dr["转换订单数量"]);
                        dr_勾选["未转数量"] = Convert.ToDecimal(dr["未转数量"]);
                        dr_勾选["此次转单数量"] = Convert.ToDecimal(dr["可转数量"]);
                    }
                }

                gridControl2.DataSource = dt_勾选明细;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

    }
}
