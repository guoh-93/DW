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

namespace ERPpurchase
{
    public partial class ui来料已检待入库查询 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";

        DataTable dt_数据 = new DataTable();
        public ui来料已检待入库查询()
        {
            InitializeComponent();
        }

        private void ui来料已检待入库查询_Load(object sender, EventArgs e)
        {
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(this.panel2, this.Name, cfgfilepath);
            fun_下拉框();
        }

        private void fun_下拉框()
        {
            try
            {
                string sql = "select 供应商ID,供应商名称 from 采购供应商表 where 供应商状态 = '在用'";
                DataTable dt_供应商 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                searchLookUpEdit2.Properties.DataSource = dt_供应商;
                searchLookUpEdit2.Properties.DisplayMember = "供应商ID";
                searchLookUpEdit2.Properties.ValueMember = "供应商ID";

                sql = "select 物料编码,物料名称,规格型号 from 基础数据物料信息表 where 停用=0";
                DataTable dt_物料 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                searchLookUpEdit1.Properties.DataSource = dt_物料;
                searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                searchLookUpEdit1.Properties.ValueMember = "物料编码";
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_check();
                fun_load();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
           
        }

        private void fun_load()
        {
            try
            {
                string sql = @"select jy.*,已送检数,kc.货架描述,kc.库存总数,cmx.仓库号,cmx.仓库名称,未领量 from 采购记录采购单检验主表 jy
                    left join  基础数据物料信息表 base on base.物料编码 = jy.产品编号
                      left join 采购记录采购单明细表 cmx on cmx.采购明细号=jy.采购明细号
                        left join 仓库物料数量表 kc on kc.物料编码=jy.产品编号  and kc.仓库号 = cmx.仓库号
                    where 入库完成 =0  and jy.完成 = 0 and jy.关闭 = 0    and
                     (检验结果<>'不合格'or(检验结果='不合格'and (检验记录单号 in (select 检验记录单号 from 检验上传表单记录表,[采购记录采购单检验主表] 
                    where 检验上传表单记录表.采购入库通知单号=[采购记录采购单检验主表].送检单号 and 表单类型='不合格品评审单') or 数量标记=1)))";

                string sql_补 = "";
                if (checkBox1.Checked == true)
                {
                    sql_补 = string.Format(@" and jy.检验记录单号 = '{0}'", textBox1.Text);
                    sql += sql_补;
                }
                if (checkBox2.Checked == true)
                {
                    sql_补 = string.Format(@" and jy.送检单号 = '{0}'", textBox2.Text);
                    sql += sql_补;
                }
                if (checkBox3.Checked == true)
                {
                    sql_补 = string.Format(@" and jy.采购单号 = '{0}'", textBox3.Text);
                    sql += sql_补;
                }
                if (checkBox5.Checked == true)
                {
                    sql_补 = string.Format(@" and jy.产品编号 = '{0}'", searchLookUpEdit1.EditValue.ToString());
                    sql += sql_补;
                }
                if (checkBox4.Checked == true)
                {
                    sql_补 = string.Format(@" and jy.供应商编号 = '{0}'", searchLookUpEdit2.EditValue.ToString());
                    sql += sql_补;
                }
                dt_数据 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                dt_数据.Columns.Add("未入库数",typeof(decimal));
                foreach (DataRow dr in dt_数据.Rows)
                {
                    dr["未入库数"] =Convert.ToDecimal(Convert.ToDecimal(dr["送检数量"]) - Convert.ToDecimal(dr["不合格数量"]) - Convert.ToDecimal(dr["已入库数"]));
                }
                gridControl1.DataSource = dt_数据;
            }
            catch (Exception ex)
            {

                throw ex ;
            }
        }

        private void fun_check()
        {
            if (checkBox1.Checked == true)
            {
                if (textBox1.Text == null || textBox1.Text.ToString() == "")
                {
                    throw new Exception("未填写检验记录单号");
                }
            }
            if (checkBox2.Checked == true)
            {
                if (textBox2.Text == null || textBox2.Text.ToString() == "")
                {
                    throw new Exception("未填写送检单号");
                }
            }
            if (checkBox3.Checked == true)
            {
                if (textBox3.Text == null || textBox3.Text.ToString() == "")
                {
                    throw new Exception("未填写采购单号");
                }

            }
            if (checkBox4.Checked == true)
            {
                if (searchLookUpEdit2.EditValue == null || searchLookUpEdit2.EditValue.ToString() == "")
                {
                    throw new Exception("未选择供应商");
                }

            }
            if (checkBox5.Checked == true)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择物料编码");
                }

            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                if (dt_数据 == null || dt_数据.Columns.Count == 0 || dt_数据.Rows.Count == 0)
                {

                    throw new Exception("没有数据可以导出");
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    //DataTable tt = dtM.Copy();
                    //tt.Columns.Remove("作废");
                    gridControl1.ExportToXlsx(saveFileDialog.FileName);
                    //ERPorg.Corg.TableToExcel(tt, saveFileDialog.FileName);
                    MessageBox.Show("导出成功");
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void 关闭ToolStripMenuItem_Click(object sender, EventArgs e)
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
                
                    fun_check();
                    fun_load();
                

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
                DateTime t = CPublic.Var.getDatetime();
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
                        xr[0]["明细完成日期"] =t;
                        //  CZMaster.MasterSQL.ExecuteSQL(sql_采购明细, strcon);o

                        DataRow[] rr = t_采购明细.Select(string.Format("明细完成日期 is null"));
                        if (rr.Length == 0)
                        {
                            foreach (DataRow r in t_采购明细.Rows)
                            {
                                r["总完成"] = true;
                                r["总完成日期"] =t;
                            }


                            string sql_采购主 = string.Format("select * from 采购记录采购单主表  where 采购单号='{0}'", dr["采购单号"]);
                            t_采购主 = CZMaster.MasterSQL.Get_DataTable(sql_采购主, strconn);
                            t_采购主.Rows[0]["完成"] = true;
                            t_采购主.Rows[0]["完成日期"] =t;
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
                r_rn["操作时间"] = t;

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

        private void gridView2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {

                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
