using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace ERPpurchase
{
    
    public partial class ui来料已检待入库 : UserControl
    {

        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dt_采购检验单;
        public ui来料已检待入库()
        {
            InitializeComponent();
        }

        private void ui来料已检待入库查询_Load(object sender, EventArgs e)
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
                x.UserLayout(this.panel1, this.Name, cfgfilepath);
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
               
            }
        }

        private void fun_load()
        {
             
            SqlDataAdapter da;
            string sql = "";
            dt_采购检验单 = new DataTable();
                    
            sql =  @"select jy.*,已送检数,kc.货架描述,kc.库存总数,cmx.仓库号,cmx.仓库名称,未领量 from 采购记录采购单检验主表 jy
            left join  基础数据物料信息表 base on base.物料编码 = jy.产品编号
                left join 采购记录采购单明细表 cmx on cmx.采购明细号=jy.采购明细号
                left join 仓库物料数量表 kc on kc.物料编码=jy.产品编号  and kc.仓库号 = cmx.仓库号
            where 入库完成 =0  and jy.完成 = 0 and jy.关闭 = 0    and
                (检验结果<>'不合格'or(检验结果='不合格'and (检验记录单号 in (select 检验记录单号 from 检验上传表单记录表,[采购记录采购单检验主表] 
            where 检验上传表单记录表.采购入库通知单号=[采购记录采购单检验主表].送检单号 and 表单类型='不合格品评审单') or 数量标记=1)))  "  ;
                
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_采购检验单);
            // dt_采购检验单.Columns.Add("选择", typeof(bool));
             
            dt_采购检验单.Columns.Add("可入库数", typeof(decimal));
            dt_采购检验单.Columns.Add("赠送数量", typeof(decimal));
            foreach (DataRow r in dt_采购检验单.Rows)
            {
                string sql_math = string.Format("select * from 其他出入库申请子表 where 备注='{0}'and 物料编码='{1}'", r["采购明细号"].ToString(), r["产品编号"].ToString());
                DataTable dt_math = CZMaster.MasterSQL.Get_DataTable(sql_math, strconn);
                if (dt_math.Rows.Count > 0)
                {
                    r["赠送数量"] = dt_math.Compute("sum(数量)", "true").ToString();
                }
                else
                {

                    r["赠送数量"] = 0;
                }

                r["可入库数"] = Convert.ToDecimal(r["送检数量"]) - Convert.ToDecimal(r["不合格数量"]) - Convert.ToDecimal(r["已入库数"]);
                

                 
            }


            gridControl1.DataSource = dt_采购检验单;

             
             
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);

                if (Convert.ToDecimal(dr["已入库数"]) > 0)
                {
                    Fun_完成关闭(dr, "完成");


                }
                else
                {
                    Fun_完成关闭(dr, "关闭");

                }

                //if (MessageBox.Show(string.Format("是否要关闭检验单{0}", dr["检验记录单号"]), "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                //{

                //CZMaster.MasterSQL.ExecuteSQL(sql_1, strcon);
                //}
                StockCore.StockCorer.fun_物料数量_实际数量(dr["产品编号"].ToString(), dr["仓库号"].ToString(), true);
                ui来料已检待入库查询_Load(null, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Fun_完成关闭(DataRow dr, string s)
        {
            ERPpurchase.frm来料入库关闭完成原因 frm = new frm来料入库关闭完成原因(dr)
            {
                Text = "记录原因"
            };

            frm.ShowDialog();
            if (frm.flag)
            {


                string sql = string.Format("select * from 采购记录采购单检验主表 where 检验记录单号 = '{0}'", dr["检验记录单号"]);
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                DataTable dt = new DataTable();
                DataTable t_采购明细 = new DataTable();
                DataTable t_采购主 = new DataTable();
                da.Fill(dt);

                if (s == "完成")
                {
                    dt.Rows[0]["完成"] = 1;
                    dt.Rows[0]["入库完成"] = 1;
                }
                else
                {
                    dt.Rows[0]["关闭"] = 1;           //区分是否是右键关闭
                }

                //new SqlCommandBuilder(da);
                //da.Update(dt);
                //关闭 赋上明细完成日期 代表 该明细已入库 ，明细完成在送检时已赋值  //分批入库的 如果关闭其中一部分的检验单  整个采购单 也会被关闭
                //需要判断  因为 有可能是 分批送检的  先检查 已送检数 是否等于 采购数  否,则未送检完， 是，则 要判断 所有的 送检单 是否完成 
                if (Convert.ToDecimal(dr["已送检数"]) == Convert.ToDecimal(dr["采购数量"]))
                {



                    string sql_1 = string.Format(@"select 采购记录采购送检单明细表.*,检验记录单号,入库完成 from 采购记录采购送检单明细表 
                                left join 采购记录采购单检验主表  on 采购记录采购单检验主表.送检单号=采购记录采购送检单明细表.送检单号
                                where  采购记录采购送检单明细表.采购单明细号='{0}' and 检验记录单号<>'{1}' and 入库完成=0  and 关闭=0", dr["采购明细号"], dr["检验记录单号"].ToString());
                    DataTable dt_1 = new DataTable();
                    using (SqlDataAdapter da_1 = new SqlDataAdapter(sql_1, strconn))  //采购单全部送检的前提下 判断除当前 送检单 是否其他 送检单 都已完成
                    {
                        da_1.Fill(dt_1);
                    }
                    if (dt_1.Rows.Count == 0) //  是 都已经处理 采购单 应该 明细完成
                    {
                        // string sql_采购明细 = string.Format("update 采购记录采购单明细表 set   明细完成=1,明细完成日期='{0}' where 采购明细号='{1}'", CPublic.Var.getDatetime(), dr["采购明细号"]);
                        string sql_采购明细 = string.Format("select * from 采购记录采购单明细表  where 采购单号='{0}'", dr["采购单号"]);
                        t_采购明细 = CZMaster.MasterSQL.Get_DataTable(sql_采购明细, strconn);
                        DataRow[] xr = t_采购明细.Select(string.Format("采购明细号='{0}'", dr["采购明细号"]));
                        xr[0]["明细完成"] = true;
                        xr[0]["明细完成日期"] = CPublic.Var.getDatetime();
                        //  CZMaster.MasterSQL.ExecuteSQL(sql_采购明细, strcon);o

                        DataRow[] rr = t_采购明细.Select(string.Format("明细完成日期 is null"));
                        if (rr.Length == 0)
                        {
                            foreach (DataRow r in t_采购明细.Rows)
                            {
                                r["总完成"] = true;
                                r["总完成日期"] = CPublic.Var.getDatetime();
                            }


                            string sql_采购主 = string.Format("select * from 采购记录采购单主表  where 采购单号='{0}'", dr["采购单号"]);
                            t_采购主 = CZMaster.MasterSQL.Get_DataTable(sql_采购主, strconn);
                            t_采购主.Rows[0]["完成"] = true;
                            t_采购主.Rows[0]["完成日期"] = CPublic.Var.getDatetime();
                            t_采购主.Rows[0]["已入库"] = true;

                        }
                        //string sql_cplt = string.Format("select * from 采购记录采购单明细表 where  明细完成日期 is null and  采购单号='{0}'", dr["采购单号"]);

                        //DataTable dt_cplt = new DataTable();
                        //dt_cplt = CZMaster.MasterSQL.Get_DataTable(sql_cplt, strcon);
                        //if (dt_cplt.Rows.Count == 0)                  //全部明细完成
                        //{
                        //明细总完成
                        //string sql_采购明细2 = string.Format("update 采购记录采购单明细表 set  总完成=1,总完成日期='{0}' where 采购单号='{1}'", CPublic.Var.getDatetime(), dr["采购单号"]);
                        //CZMaster.MasterSQL.ExecuteSQL(sql_采购明细2, strcon);
                        //主表记录完成
                        //string sql_主表完成 = string.Format("update 采购记录采购单主表  set 完成=1,完成日期='{0}',已入库=1 where  采购单号='{1}'", CPublic.Var.getDatetime(), dr["采购单号"]);
                        //CZMaster.MasterSQL.ExecuteSQL(sql_主表完成, strcon);

                    }


                }
                //记录原因 
                string sql_reason = "select * from 采购入库完成关闭原因表 where 1<>1 ";
                DataTable t_rn = CZMaster.MasterSQL.Get_DataTable(sql_reason, strconn);
                DataRow r_rn = t_rn.NewRow();
                r_rn["采购明细号"] = dr["采购明细号"];
                r_rn["检验记录单号"] = dr["检验记录单号"];
                r_rn["原因"] = frm.str;
                r_rn["物料编码"] = dr["产品编号"];
                r_rn["物料名称"] = dr["产品名称"];
                r_rn["供应商ID"] = dr["供应商编号"];
                r_rn["操作人"] = CPublic.Var.localUserName;
                r_rn["操作时间"] = CPublic.Var.getDatetime();

                t_rn.Rows.Add(r_rn);





                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction st = conn.BeginTransaction("关闭入库通知"); //事务的名称
                SqlCommand cmd = new SqlCommand("select * from 采购记录采购单检验主表  where 1<>1", conn, st);
                SqlCommand cmd1 = new SqlCommand("select * from 采购记录采购单明细表 where 1<>1", conn, st);
                SqlCommand cmd2 = new SqlCommand("select * from 采购记录采购单主表 where 1<>1", conn, st);
                SqlCommand cmd3 = new SqlCommand(sql_reason, conn, st);


                try
                {
                    SqlDataAdapter da_1;
                    da_1 = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da_1);
                    da_1.Update(dt);

                    if (t_采购明细 != null)
                    {
                        da_1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da_1);
                        da_1.Update(t_采购明细);
                    }
                    if (t_采购主 != null)
                    {
                        da_1 = new SqlDataAdapter(cmd2);
                        new SqlCommandBuilder(da_1);
                        da_1.Update(t_采购主);
                    }

                    da_1 = new SqlDataAdapter(cmd3);
                    new SqlCommandBuilder(da_1);
                    da_1.Update(t_rn);
                    st.Commit();
                }
                catch
                {
                    st.Rollback();
                }



            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    //DataTable tt = dtM.Copy();
                    //tt.Columns.Remove("作废");
                    gridView2.ExportToXlsx(saveFileDialog.FileName);
                    //ERPorg.Corg.TableToExcel(tt, saveFileDialog.FileName);
                    MessageBox.Show("导出成功");
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
