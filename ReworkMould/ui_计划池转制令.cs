using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace ReworkMould
{
    public partial class ui_计划池转制令 : UserControl
    {
        #region 变量
        DataTable t_库存;
        DataTable list_PA;
        DataTable dtM;
        DataTable dtP;
        string strcon = CPublic.Var.strConn;
        string cfgfilepath = "";
        bool bool_生效 = false;
        #endregion
        public ui_计划池转制令()
        {
            InitializeComponent();
        }

        public ui_计划池转制令(DataTable dt)
        {
            InitializeComponent();
            list_PA = dt;
            bool_生效 = false;
        }

        private void ui_计划池转制令_Load(object sender, EventArgs e)
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
                x.UserLayout(splitContainer1, this.Name, cfgfilepath);
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_load()
        {
            string s = @"select  zl.*,新数据,工时,拼板数量 from  生产记录生产制令表 zl
                        left join 基础数据物料信息表 base on zl.物料编码=base.物料编码  where 1=2";
            dtM = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataColumn dc = new DataColumn("库存总数", typeof(int));
            dc.DefaultValue = 0;
            dtM.Columns.Add(dc);
            DataColumn dc1 = new DataColumn("人力", typeof(int));
            dc1.DefaultValue = 1;
            dtM.Columns.Add(dc1);
            dtM.Columns.Add("总需数量", typeof(decimal));
            dtM.Columns.Add("建议开工日期", typeof(DateTime));
            dtM.Columns.Add("领料类型");
            dtM.Columns.Add("计划单明细号");

            s = "select  * from 生产记录生产制令子表 where 1=2";
            dtP = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            dtP.Columns.Add("表头备注");

            s = @"select 子项编码,wiptype  from  基础数据物料BOM表 where WIPType in ('虚拟','领料') group by 子项编码,wiptype ";
            DataTable t_wiptype = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            s = "select * from V_pooltotal ";
            t_库存 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataColumn[] pk = new DataColumn[1];
            pk[0] = t_库存.Columns["物料编码"];
            t_库存.PrimaryKey = pk;

            foreach (DataRow dr in list_PA.Rows)
            {
                DataRow r = dtM.NewRow();
                DataRow[] rtype = t_wiptype.Select(string.Format("子项编码='{0}'", dr["物料编码"]));
                if (rtype.Length == 1)
                {
                    r["领料类型"] = rtype[0]["wiptype"];
                }
                else
                {
                    r["领料类型"] = "领料";
                }
                r["GUID"] = System.Guid.NewGuid();
                r["生产制令类型"] = "计划类型";
                r["加急状态"] = "正常";

                r["物料编码"] = dr["物料编码"];
                r["总需数量"]  = dr["参考数量"];
                r["制令数量"] =Convert.ToDecimal(dr["计划数量"])-Convert.ToDecimal(dr["已转数量"]);
                r["班组"] = dr["班组名称"];
                r["班组ID"] = dr["班组编号"];

                r["建议开工日期"] = Convert.ToDateTime(dr["开工日期"]);
                r["物料名称"] = dr["物料名称"];
                r["规格型号"] = dr["规格型号"];
                r["计划单明细号"] = dr["计划单明细号"];
                DataRow[] dbase = t_库存.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                r["工时"] = dbase[0]["工时"];
                r["生产车间"] = dbase[0]["车间编号"];
                //DataTable tt = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);
                //if (tt.Rows.Count > 0)
                //{
                //    r["生产车间"] = tt.Rows[0]["生产车间"];
                //}
                r["新数据"] = dbase[0]["新数据"];
                r["拼板数量"] = dbase[0]["拼板数量"];

                r["库存总数"] = dbase[0]["库存总数"];
                
                double dob = Convert.ToDouble(r["工时"]) / Convert.ToDouble(r["人力"]); //那么 多个人 就是 工时/人力
                int x = 0;
                if (dob > 0)
                {
                    // x = (int)Math.Ceiling(Convert.ToDouble(cx[0]["制令数量"]) / dob);
                    x = (int)Math.Ceiling(Convert.ToDouble(r["制令数量"]) * dob);
                    r["预完工日期"] = Convert.ToDateTime(r["建议开工日期"]).AddDays(x);
                    
                }
                //DataRow[] zlmx = t_relation.Select(string.Format("子项编码='{0}'", dr["物料编码"]));
                //DateTime? t = null;


                //r["预完工日期"] = t;
                dtM.Rows.Add(r);
            }

            

            dtM.Columns.Add("已转工单数", typeof(decimal));
            dtM.Columns.Add("已转制令数", typeof(decimal));


            foreach (DataRow dr in dtM.Rows)
            {
                string sss = string.Format(@"SELECT     物料编码, SUM(制令数量) AS 已转制令数, SUM(已排单数量) AS 已转工单数
                            FROM dbo.生产记录生产制令表  WHERE 完成 = 0 and  关闭 = 0  and 物料编码 ='{0}'GROUP BY 物料编码", dr["物料编码"]);
                DataTable t = CZMaster.MasterSQL.Get_DataTable(sss, strcon);
                if (t.Rows.Count > 0)
                {
                    dr["已转制令数"] = t.Rows[0]["已转制令数"];
                    dr["已转工单数"] = t.Rows[0]["已转工单数"];
                }
                else
                {
                    dr["已转制令数"] = 0;
                    dr["已转工单数"] = 0;

                }

            }

            gc.DataSource = dtM;
        }

        //关闭
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show(string.Format("确认是否关闭此界面"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {

                CPublic.UIcontrol.ClosePage();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(string.Format("确认删除当前选中行？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    int[] dr1 = gv.GetSelectedRows();
                    if (dr1.Length > 0)
                    {
                        for (int i = dr1.Length - 1; i >= 0; i--)
                        {
                            DataRow dr_选中 = gv.GetDataRow(dr1[i]);
                            DataRow[] dr_1 = dtP.Select(string.Format("GUID = '{0}'",dr_选中["GUID"]));
                            if (dr_1.Length>0)
                            {
                                for (int j = dr_1.Length - 1; j >= 0; j--)
                                {
                                    DataRow dr_选中1 = gv_关联订单.GetDataRow(dr1[j]);
                                    dr_选中1.Delete();
                                }
                            }
                            dr_选中.Delete();

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(string.Format("确认删除当前选中行？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    int[] dr1 = gv_关联订单.GetSelectedRows();
                    if (dr1.Length > 0)
                    {
                        for (int i = dr1.Length - 1; i >= 0; i--)
                        {
                            DataRow dr_选中 = gv_关联订单.GetDataRow(dr1[i]);                          
                            dr_选中.Delete();

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            try
            {
                gv.CloseEditor();
                this.ActiveControl = null;
                if (dtM.Rows.Count == 0 )
                {
                    throw new Exception("无生产制令，不可新增明细！");
                }
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                
                Form2 fm = new Form2();
                ui_选择关联销售单 ui = new ui_选择关联销售单(dr["物料编码"].ToString());
                fm.Controls.Add(ui);
                fm.Text = "关联销售单";
                ui.Dock = DockStyle.Fill;
                fm.ShowDialog();
                if (ui.flag &&ui.dt_xsmx.Rows.Count>0)
                {
                    string s_销售备注 = "";
                    foreach (DataRow dr_mx in ui.dt_xsmx.Rows)
                    {
                        DataRow[] dr_p = dtP.Select(string.Format("销售订单明细号 = '{0}' and GUID = '{1}'", dr_mx["销售订单明细号"], dr["GUID"]));
                        if (dr_p.Length > 0)
                        {                             
                            dr_p[0]["物料编码"] = dr_mx["物料编码"];
                            dr_p[0]["物料名称"] = dr_mx["物料名称"];
                            dr_p[0]["客户"] = dr_mx["客户"];
                            dr_p[0]["规格型号"] = dr_mx["规格型号"];
                            dr_p[0]["送达日期"] = Convert.ToDateTime(dr_mx["预计发货日期"]);
                            dr_p[0]["数量"] = Convert.ToDecimal(dr_mx["数量"]);
                            dr_p[0]["销售备注"] = dr_mx["备注"];
                            dr_p[0]["表头备注"] = dr_mx["表头备注"];

                        }
                        else
                        {
                            DataRow dr_1 = dtP.NewRow();
                            dtP.Rows.Add(dr_1);
                            dr_1["GUID"] = dr["GUID"]; 
                            dr_1["销售订单明细号"] = dr_mx["销售订单明细号"];
                            dr_1["销售订单号"] = dr_mx["销售订单号"];
                            dr_1["物料编码"] = dr_mx["物料编码"];
                            dr_1["物料名称"] = dr_mx["物料名称"];
                            dr_1["客户"] = dr_mx["客户"];
                            dr_1["规格型号"] = dr_mx["规格型号"];
                            dr_1["送达日期"] = Convert.ToDateTime(dr_mx["预计发货日期"]);
                            dr_1["数量"] = Convert.ToDecimal(dr_mx["数量"]);
                            dr_1["销售备注"] = dr_mx["备注"];
                            dr_1["表头备注"] = dr_mx["表头备注"];
                        }
                        s_销售备注 = dr_mx["备注"] + "|" + s_销售备注;

                    }
                    dr["备注"] = s_销售备注.Substring(0,s_销售备注.Length-1);
                }
                DataView dv = new DataView(dtP);
                dv.RowFilter = $"GUID = '{dr["GUID"]}'";
                gc_关联订单.DataSource = dv;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv.CloseEditor();
                this.ActiveControl = null;
                check();

                save();

                MessageBox.Show("ok");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void save()
        {
            DateTime t = CPublic.Var.getDatetime();
            string yy = t.Year.ToString().Substring(2, 2);
            string s_ph = string.Format("{0}{1:00}{2:00}{3:0000}", yy, t.Month, t.Day,
                          CPublic.CNo.fun_得到最大流水号("JHPH", t.Year, t.Month));
            DataTable dt_计划明细 = CZMaster.MasterSQL.Get_DataTable("select * from 主计划计划生成单_制令", strcon);
             
            foreach (DataRow dr in dtM.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                dr["生产制令单号"] = string.Format("PM{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                          CPublic.CNo.fun_得到最大流水号("PM", t.Year, t.Month));
              
                dr["未排单数量"] = dr["制令数量"];
                dr["预开工日期"] = dr["建议开工日期"];
                dr["预计完工日期"] = dr["预完工日期"];

                dr["日期"] = t;
                dr["制单人员"] = dr["操作人员"] = dr["生效人员"] = CPublic.Var.localUserName;
                dr["制单人员ID"] = dr["操作人员ID"] = dr["生效人员ID"] = CPublic.Var.LocalUserID;

                string s = string.Format(@"select  仓库号,仓库名称,b_班组编号,b_班组名称  from 基础数据物料信息表 where 物料编码='{0}'", dr["物料编码"]);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                dr["仓库号"] = temp.Rows[0]["仓库号"];
                dr["备注5"] = s_ph;
                dr["班组ID"] = dr["班组ID"];
                dr["班组"] = dr["班组"];




                //然后根据 物料编码去  dtP中找 制令子记录 
                DataRow[] r_mx = dtP.Select(string.Format("GUID='{0}'", dr["GUID"]));
                foreach (DataRow r in r_mx)
                {
                    r["生产制令单号"] = dr["生产制令单号"];                   
                }

                DataRow[] dr_计划明细 = dt_计划明细.Select(string.Format("计划单明细号 = '{0}'",dr["计划单明细号"]));
                if (dr_计划明细.Length>0)
                {
                    if (Convert.ToDecimal(dr_计划明细[0]["计划数量"])>(Convert.ToDecimal(dr_计划明细[0]["已转数量"]) + Convert.ToDecimal(dr["制令数量"])))
                    {
                        dr_计划明细[0]["已转数量"] = Convert.ToDecimal(dr_计划明细[0]["已转数量"]) + Convert.ToDecimal(dr["制令数量"]);
                    }
                    else
                    {
                        dr_计划明细[0]["已转数量"] = Convert.ToDecimal(dr_计划明细[0]["计划数量"]);
                    }
                    
                }



            }
            for (int i = dtP.Rows.Count - 1; i >= 0; i--)
            {
                if (dtP.Rows[i]["生产制令单号"].ToString() == "")
                {
                    dtP.Rows[i].Delete();
                }
            }
            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("转制令");
            try
            {
                string sql = "select * from 生产记录生产制令表 where 1<>1";
                SqlCommand cmm = new SqlCommand(sql, conn, ts);
                SqlDataAdapter da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(dtM);
                //制令明细表
                sql = "select * from 生产记录生产制令子表 where 1<>1";
                cmm = new SqlCommand(sql, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(dtP);

                sql = "select * from 主计划计划生成单_制令 where 1<>1";
                cmm = new SqlCommand(sql, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(dt_计划明细);

                ts.Commit();
                bool_生效 = true;
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw new Exception(ex.Message);
            }

        }

        private void check()
        {
            this.ActiveControl = null;
            if (bool_生效) throw new Exception("已转为制令，不可重复操作");
            foreach (DataRow dr in dtM.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                if (dr["建议开工日期"].ToString() == "" || dr["预完工日期"].ToString() == "")
                    throw new Exception("开工完工日期未确认");
                decimal dec;
                if (!decimal.TryParse(dr["制令数量"].ToString().Trim(), out dec)) throw new Exception("制令数量输入不正确");
                if (dec <= 0) throw new Exception("制令数量不可小于或等于0");
                DataRow[] dr_关联 = dtP.Select(string.Format("GUID = '{0}'", dr["GUID"]));
               // if (dr_关联.Length == 0) throw new Exception(dr["物料编码"]+"物料没有关联订单，请确认");
            }
        }

        private void gv_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                DataView dv = new DataView(dtP);
                dv.RowFilter = $"GUID = '{dr["GUID"]}'";
                gc_关联订单.DataSource = dv;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
                    
           
        }

        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (dr == null) return;
                DataView dv = new DataView(dtP);
                dv.RowFilter = $"GUID = '{dr["GUID"]}'";
                gc_关联订单.DataSource = dv;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

       

        private void gv_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Value.ToString() != "")
                {
                    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                    if (e.Column.Caption != "生产备注")
                    {
                        if (e.Column.Caption == "制令数量")
                        {
                            DataTable ListM = new DataTable();
                            ListM = ERPorg.Corg.billofM_带数量(ListM, dr["物料编码"].ToString(), false);
                            foreach (DataRow Mr in dtM.Rows)
                            {
                                DataRow[] Mr_r = ListM.Select(string.Format("子项编码='{0}'", Mr["物料编码"]));
                                if (Mr_r.Length > 0)
                                {
                                    decimal dec = Convert.ToDecimal(dr["制令数量"]) * Convert.ToDecimal(Mr_r[0]["数量"]);
                                    if (dec < Convert.ToDecimal(Mr["总需数量"])) Mr["制令数量"] = dec;
                                    else Mr["制令数量"] = Mr["总需数量"];
                                }
                            }
                        }
                        if (Convert.ToDouble(dr["工时"]) == 0) return;
                        double a = Convert.ToDouble(dr["制令数量"]) * (Convert.ToDouble(dr["工时"]) / Convert.ToDouble(dr["人力"]));
                        int x = (int)Math.Ceiling(a);
                        if (e.Column.Caption != "预完工日期")
                        {
                            dr["预完工日期"] = Convert.ToDateTime(dr["建议开工日期"]).AddDays(x);
                        }
                        else
                        {
                            dr["建议开工日期"] = Convert.ToDateTime(dr["预完工日期"]).AddDays(-x);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
