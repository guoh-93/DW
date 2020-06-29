using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Windows.Forms;
namespace ERPproduct
{
    public partial class ui_标签记录列表 : UserControl
    {
        string str_BQ = CPublic.Var.geConn("BQ");
        string strcon = CPublic.Var.strConn;
        DataRow dr_s;
        bool Print_bl = false;

        public ui_标签记录列表(DataRow dr)
        {
            InitializeComponent();
            dr_s = dr;

        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                gridControl1.ExportToXlsx(saveFileDialog.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定删除该工单所对应的SN号吗？请确认！", "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                string s = $@"delete  from Print_ShareLockInfo where MakeOrder='{dr_s["生产工单号"].ToString()}'  
                            update 生产记录生产工单表 set MaxNo=0 where  生产工单号='{dr_s["生产工单号"].ToString()}'";
                CZMaster.MasterSQL.ExecuteSQL(s, strcon);
                s = $"delete  from ShareLockInfo where TaskNo='{dr_s["生产工单号"].ToString()}'";
                CZMaster.MasterSQL.ExecuteSQL(s, str_BQ);
                MessageBox.Show("已删除");
                fun_load();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("确认以所选日期生成该工单的SN号么？请确认！", "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DateTime t = Convert.ToDateTime(dateEdit1.EditValue).Date;
                    string s = string.Format("select * from 生产记录生产工单表 where 生产工单号='{0}'", dr_s["生产工单号"].ToString());
                    DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    if (Convert.ToInt32(dt.Rows[0]["MaxNo"]) + Convert.ToInt32(textBox1.Text) > Convert.ToInt32(dt.Rows[0]["生产数量"]))
                    {
                        throw new Exception("已超出数量,请确认");
                    }

                    ERPorg.Corg xx = new ERPorg.Corg();
                    DataSet ds_sn = xx.fun_SN(dt, t, Convert.ToInt32(textBox1.Text));

                    SqlConnection conn = new SqlConnection(strcon);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("sn号");
                    try
                    {

                        string sql = "select  * from 生产记录生产工单表 where 1=2";

                        SqlCommand cmm = new SqlCommand(sql, conn, ts);
                        SqlDataAdapter da = new SqlDataAdapter(cmm);
                        new SqlCommandBuilder(da);
                        da.Update(ds_sn.Tables[2]);

                        sql = "select * from Print_ShareLockInfo where 1=2 ";
                        cmm = new SqlCommand(sql, conn, ts);
                        da = new SqlDataAdapter(cmm);
                        new SqlCommandBuilder(da);
                        da.Update(ds_sn.Tables[1]);

                        ts.Commit();
                        if (ds_sn != null)
                        {
                            ///2019-10-16  这边要保存另一个数据库  目前我不知道怎么两个数据用类似事务的方式一起保存 
                            string str_BQ = CPublic.Var.geConn("BQ");
                            CZMaster.MasterSQL.Save_DataTable(ds_sn.Tables[0], "ShareLockInfo", str_BQ);
                        }
                        MessageBox.Show("生成成功");
                        fun_load();


                    }
                    catch (Exception ex)
                    {
                        ts.Rollback();
                        throw new Exception("生成失败" + ex.Message);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ui_标签记录列表_Load(object sender, EventArgs e)
        {
            try
            {
                fun_load();


                //string strc_BQ = CPublic.Var.geConn("BQ");
                //string s = string.Format("select  * from [ShareLockInfo] where taskNo='{0}' ", dr_s["生产工单号"]);
                //DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strc_BQ);
                //DataView dv = new DataView(dt);
                //dv.RowFilter = "LockID=0";
                //if (dv.Count > 0) throw new Exception("该工单尚有锁号未回写,请稍候再试");

                //string l = string.Format(@"select  a.*,FCCID from  Print_ShareLockInfo  a
                //left join 生产记录生产工单表 b  on a.MakeOrder = b.生产工单号
                //left join [基础物料标签维护信息表] c on c.物料编号 = b.物料编码 where MakeOrder='{0}' order by ctNo ", dr_s["生产工单号"]);
                //DataTable dt_2 = CZMaster.MasterSQL.Get_DataTable(l, strcon);


                //foreach (DataRow r in dt_2.Rows)
                //{
                //    DataRow[] pr = dt.Select(string.Format("CTNo='{0}'", r["CTNo"]));
                //    if (pr.Length > 0)
                //    {
                //        r["MacAddr"] = pr[0]["MacAddr"];
                //        r["LockID"] = pr[0]["LockID"];
                //        r["CheckFlag"] = pr[0]["CheckFlag"];
                //    }
                //}

                //gridControl1.DataSource = dt_2;




            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void fun_load()
        {

            string strc_BQ = CPublic.Var.geConn("BQ");
            string s = string.Format("select  * from [ShareLockInfo] where taskNo='{0}' ", dr_s["生产工单号"]);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strc_BQ);
            DataView dv = new DataView(dt);
            dv.RowFilter = "LockID=0";
            if (dv.Count > 0) throw new Exception("该工单尚有锁号未回写,请稍候再试");

            string l = string.Format(@"select  a.*,FCCID from  Print_ShareLockInfo  a
                left join 生产记录生产工单表 b  on a.MakeOrder = b.生产工单号
                left join [基础物料标签维护信息表] c on c.物料编号 = b.物料编码 where MakeOrder='{0}' order by ctNo ", dr_s["生产工单号"]);
            DataTable dt_2 = CZMaster.MasterSQL.Get_DataTable(l, strcon);



            foreach (DataRow r in dt_2.Rows)
            {
                DataRow[] pr = dt.Select(string.Format("CTNo='{0}'", r["CTNo"]));
                if (pr.Length > 0)
                {
                    r["MacAddr"] = pr[0]["MacAddr"];
                    r["LockID"] = pr[0]["LockID"];
                    r["CheckFlag"] = pr[0]["CheckFlag"];
                }
            }

            gridControl1.DataSource = dt_2;
        }



        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            { e.Handled = true; }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string Printer_标签 = "";
                try
                {

                    Printer_标签 = CPublic.Var.li_CFG["printer_label"].ToString();
                }
                catch (Exception)
                {

                    throw new Exception("标签打印机未配置,printer_label未找到");
                }
                string strc_BQ = CPublic.Var.geConn("BQ");

                if (Print_bl) throw new Exception("正在打印标签请稍候");
                DevExpress.XtraGrid.Views.Base.GridCell[] gcell = gridView1.GetSelectedCells();
                int[] rowindex = gridView1.GetSelectedRows();
                if (rowindex.Length == 0) throw new Exception("未选中任何行");
                List<Dictionary<string, string>> li = new List<Dictionary<string, string>>();
                //foreach (int x in rowindex)
                //{
                //    DataRow dr = gridView1.GetDataRow(x);
                //    Dictionary<string, string> dic = new Dictionary<string, string>();
                //    dic.Add("zcdm", dr["资产编码"].ToString());
                //    li.Add(dic);
                //}
                DataRow dr = gridView1.GetDataRow(rowindex[0]);



                Thread BG = new Thread(() =>
                {
                    //List<Dictionary<string, string>> li = new List<Dictionary<string, string>>();
                    //foreach (DataRow PrintRow in dt_2.Rows)
                    //{
                    //    Dictionary<string, string> dic = new Dictionary<string, string>();
                    //    dic.Add("SN", PrintRow["CTNo"].ToString().Trim());
                    //    dic.Add("LockID", PrintRow["LockID"].ToString().Trim());
                    //    dic.Add("FCCID", PrintRow["FCCID"].ToString().Trim());
                    //    li.Add(dic);
                    //}
                    string sn_f11 = dr["CTNo"].ToString().Substring(0, 11); //sn20位 取前 11位 
                    string ruleid = dr["DevType"].ToString();
                    string xx = string.Format(@"select  * from [LockIDRuleInfo] where RuleID='{0}' ", ruleid);
                    DataTable temp = CZMaster.MasterSQL.Get_DataTable(xx, strc_BQ);
                    string str_len = temp.Rows[0]["lshlen"].ToString();
                    int len = 0;
                    int.TryParse(str_len, out len);
                    //if (len== 0)
                    //{
                    //    List<Dictionary<string, string>> li = new List<Dictionary<string, string>>();
                    //    foreach (DataRow PrintRow in dt_2.Rows)
                    //    {
                    //        Dictionary<string, string> dic = new Dictionary<string, string>();
                    //        dic.Add("SN", PrintRow["CTNo"].ToString().Trim());
                    //        //dic.Add("LockID", PrintRow["LockID"].ToString().Trim());
                    //        dic.Add("FCCID", PrintRow["FCCID"].ToString().Trim());
                    //        li.Add(dic);
                    //    }
                    //    string path = Application.StartupPath + string.Format(@"\Mode\SN标签_无锁号.lab");
                    //    Lprinter lp = new Lprinter(path, li, Printer_标签, 1);
                    //    lp.DoWork();
                    //    Print_bl = false;
                    //}
                    //else
                    //{
                    int lock_len = dr["LockID"].ToString().Length;
                    int x = lock_len - len;
                    string lockid_f = "";
                    int idls = 0;
                    if (len > 0)
                    {
                        lockid_f = dr["LockID"].ToString().Substring(0, x);
                        idls = Convert.ToInt32(dr["LockID"].ToString().Substring(x, len));
                    }
                    int snls = Convert.ToInt32(dr["CTNo"].ToString().Substring(11, 6));
                    ERPorg.Corg cg = new ERPorg.Corg();
                    int qsyzm = Convert.ToInt32(cg.total_JY(dr["CTNo"].ToString().Substring(0, 11)));


                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("SN", sn_f11);// sn前11位
                    dic.Add("LockID", lockid_f); //lockID 先去中间表取当前 锁号规则流水号是几位 x    len(LockID)-x
                    dic.Add("FCCID", dr["FCCID"].ToString());

                    dic.Add("idls", idls.ToString()); //锁号的流水号
                    dic.Add("snls", snls.ToString()); //sn的流水号

                    dic.Add("qsyzm", qsyzm.ToString()); //起始验证码
                    string path = "";
                    if (len == 0)
                    {
                        path = Application.StartupPath + string.Format(@"\Mode\SN标签_无锁号.lab");
                    }
                    else
                    {
                        path = Application.StartupPath + string.Format(@"\Mode\SN标签_锁号{0}位.lab", len.ToString());
                    }
                    Lprinter lp = new Lprinter(path, dic, Printer_标签, rowindex.Length);
                    lp.DoWork();
                    Print_bl = false;
                    //cg.kill_lppa();
                    //}
                });
                BG.IsBackground = true;
                BG.Start();
                Print_bl = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //20-6-9 新增删除选中行 可多选
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                int[] index = gridView1.GetSelectedRows();
                if (MessageBox.Show($"已选中{index.Length}条,确认删除?", "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string ss = " and CTNo in ('";
                    int x = 0;
                    foreach (int i in index)
                    {
                        x++;
                        if (x != index.Length)
                            ss += gridView1.GetRowCellValue(i, "CTNo") + "',";
                        else
                            ss += gridView1.GetRowCellValue(i, "CTNo") + "')";
                      
                    
                    }

                    string s = $@"delete  from Print_ShareLockInfo where MakeOrder='{dr_s["生产工单号"].ToString()}'   {ss}
                            update 生产记录生产工单表 set MaxNo=MaxNo-{index.Length} where  生产工单号='{dr_s["生产工单号"].ToString()}'";
                    CZMaster.MasterSQL.ExecuteSQL(s, strcon);
                    s = $"delete  from ShareLockInfo where TaskNo='{dr_s["生产工单号"].ToString()}' {ss}";
                    CZMaster.MasterSQL.ExecuteSQL(s, str_BQ);
                    MessageBox.Show("已删除");
                    fun_load();
                }


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
