using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
namespace ERPStock
{
    public partial class frm仓库出入库 : UserControl
    {
        #region 变量

        string strconn = CPublic.Var.strConn;
        DataTable dt;
        DataView dv;
        string sql;
        string s = null;
        DataTable dt_仓库 = new DataTable();
        string sql_ck = string.Format("select * from 人员仓库对应表 where 工号='{0}'", CPublic.Var.LocalUserID);
        string cfgfilepath = "";
        DataTable dtM;
        #endregion

        #region 加载
        public frm仓库出入库()
        {

            InitializeComponent();
            barEditItem2.EditValue = System.DateTime.Today.ToString("yyyy-MM-dd");
    
            barEditItem1.EditValue = System.DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
            dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_ck, strconn);
            sql_ck = "and 仓库出入库明细表.仓库号  in(";
            if (dt_仓库.Rows.Count == 0)
            {
                sql = string.Format(@"select 仓库出入库明细表.* ,基础数据物料信息表.规格型号 from 仓库出入库明细表  with (NOLOCK)
                                    left join   基础数据物料信息表 on 基础数据物料信息表.物料编码= 仓库出入库明细表.物料编码
                                    where  仓库出入库明细表.出入库时间>='{0}' and 仓库出入库明细表.出入库时间< '{1}' order by 出入库时间 desc,出库入库 asc", barEditItem1.EditValue,Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));
            }
            else
            {
                foreach (DataRow dr in dt_仓库.Rows)
                {
                    sql_ck = sql_ck + string.Format("'{0}',", dr["仓库号"]);

                }
                sql_ck = sql_ck.Substring(0, sql_ck.Length - 1) + ")";
                sql = string.Format(@"select 仓库出入库明细表.*,基础数据物料信息表.规格型号 from 仓库出入库明细表 with (NOLOCK) 
                                    left join   基础数据物料信息表 on 基础数据物料信息表.物料编码= 仓库出入库明细表.物料编码
                                    where  仓库出入库明细表.出入库时间>='{0}' and 仓库出入库明细表.出入库时间< '{1}' {2} order by 出入库时间 desc,出库入库 asc", barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1), sql_ck);
            }

        }
        public frm仓库出入库(string s)
        {

            InitializeComponent();
            DateTime t = CPublic.Var.getDatetime().Date ;
            barEditItem2.EditValue = t.ToString("yyyy-MM-dd");
            barEditItem1.EditValue = t.AddMonths(-1).ToString("yyyy-MM-dd");
            this.s = s;
            dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_ck, strconn);
            sql_ck = "and 仓库出入库明细表.仓库号  in(";
            if (dt_仓库.Rows.Count == 0)
            {
              sql = string.Format(@"select 仓库出入库明细表.*,基础数据物料信息表.规格型号  from 仓库出入库明细表 with (NOLOCK)
              left join   基础数据物料信息表 on 基础数据物料信息表.物料编码= 仓库出入库明细表.物料编码
              where  仓库出入库明细表.物料编码='{0}' and  仓库出入库明细表.出入库时间>='{1}' and 仓库出入库明细表.出入库时间< '{2}'  order by 出入库时间 desc,出库入库 asc", s, barEditItem1.EditValue,t.AddDays(1).AddSeconds(-1));
            }
            else
            {
                foreach (DataRow dr in dt_仓库.Rows)
                {
                    sql_ck = sql_ck + string.Format("'{0}',", dr["仓库号"]);

                }
                sql_ck = sql_ck.Substring(0, sql_ck.Length - 1) + ")";
                sql = string.Format(@"select 仓库出入库明细表.*,基础数据物料信息表.规格型号  from 仓库出入库明细表 with (NOLOCK)
                     left join   基础数据物料信息表 on 基础数据物料信息表.物料编码= 仓库出入库明细表.物料编码
                  where  仓库出入库明细表.物料编码='{0}' and  仓库出入库明细表.出入库时间>='{1}' and 仓库出入库明细表.出入库时间< '{2}' {3}  order by 出入库时间 desc,出库入库 asc", s, barEditItem1.EditValue, t.AddDays(1).AddSeconds(-1), sql_ck);
            }





        }
        private void frm仓库出入库_Load(object sender, EventArgs e)
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


        #endregion

        #region 函数
        void fun_load()
        {

            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                dt = new DataTable();
                da.Fill(dt);

                DateTime t1 = Convert.ToDateTime(barEditItem1.EditValue).Date;
                DateTime dtime = Convert.ToDateTime(barEditItem2.EditValue).Date.AddDays(1).AddSeconds(-1);
 
                //string sql = $@"select  明细类型,单号,物料编码,出库入库,实效数量,出入库时间,相关单号,仓库号,仓库名称 from 
                //    仓库出入库明细表 where 物料编码='{s}' and 出入库时间 >'2019-5-1' and 出入库时间>'{t1}' and 出入库时间<'{dtime}' order by 出入库时间 desc ";
                //DataTable t_明细 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                dtM = new DataTable();

                dtM = dt.Clone();
                dtM.Columns.Add("时点库存", typeof(decimal));

                sql = $@"select aa.物料编码,aa.仓库号,aa.仓库名称,(aa.库存总数-isnull(xx.出入数量,0))库存总数 from 仓库物料数量表 aa 
          left join (select 物料编码,SUM(实效数量) as 出入数量, 仓库号 from 仓库出入库明细表 with (NOLOCK)  where 出入库时间 > '{dtime}' group by 物料编码, 仓库号) xx
          on xx.物料编码 = aa.物料编码 and xx.仓库号 = aa.仓库号  where aa.物料编码='{s}'"; //取截止到时间点的库存
                DataTable t_库存 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                foreach (DataRow dr in dt.Rows)
                {
                   
                    DataRow rr = dtM.NewRow();
                    rr.ItemArray = dr.ItemArray;
                    DataRow[] tr = t_库存.Select($"仓库号='{dr["仓库号"].ToString()}'");
                    decimal dec = 0;
                    if (tr.Length > 0)
                    {
                        dec = Convert.ToDecimal(tr[0]["库存总数"]);
                        //先赋值后减 
                        tr[0]["库存总数"] = dec - Convert.ToDecimal(dr["实效数量"]);
                    }
                    rr["时点库存"] = dec;


                    dtM.Rows.Add(rr);

                }

                //dtM.Columns.Remove("GUID");
                //dtM.Columns.Remove("ID");
                dv = new DataView(dtM);
            }

            gridControl1.DataSource = dv;
        }
        //void fun_filter()
        //{
        //    dv = new DataView(dtM);
        //    try
        //    {
        //        if (checkBox1.Checked == true && checkBox6.Checked == true)
        //        {

        //        }
        //        else
        //        {
        //            if (checkBox1.Checked == true)
        //            {
        //                dv.RowFilter = "出库入库='出库'";
        //            }
        //            if (checkBox6.Checked == true)
        //            {
        //                dv.RowFilter = "出库入库='入库'";
        //            }
        //        }
        //        if (checkBox2.Checked == true)
        //        {
        //            dv.RowFilter = "明细类型='采购'";
        //        }
        //        if (checkBox3.Checked == true)
        //        {
        //            dv.RowFilter = "明细类型='销售'";
        //        }
        //        if (checkBox4.Checked == true)
        //        {
        //            dv.RowFilter = "明细类型='生产'";
        //        }
        //        if (checkBox5.Checked == true)
        //        {
        //            dv.RowFilter = "明细类型='领料'";
        //        }
        //        if (checkBox7.Checked == true)
        //        {
        //            dv.RowFilter = "明细类型='其他'";
        //        }
        //        gridControl1.DataSource = dv;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}


        #endregion
        ////筛选 按钮
        //private void button1_Click(object sender, EventArgs e)
        //{
        //    fun_filter();
        //}

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ERPorg.Corg.FlushMemory();
            DateTime dtime = Convert.ToDateTime(barEditItem2.EditValue).Date.AddDays(1).AddSeconds(-1);
            if (s == null)
            {

                sql = string.Format(@"select 仓库出入库明细表.*,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.规格型号,
                  基础数据物料信息表.仓库号,基础数据物料信息表.仓库名称  from 仓库出入库明细表  with (NOLOCK)
                  left join   基础数据物料信息表   on 基础数据物料信息表.物料编码= 仓库出入库明细表.物料编码
                  where  仓库出入库明细表.出入库时间>='{0}' and 仓库出入库明细表.出入库时间<='{1}'  order by 出入库时间 desc,出库入库 asc", barEditItem1.EditValue,dtime);

            }
            else
            {
                sql = string.Format(@"select 仓库出入库明细表.*,基础数据物料信息表.规格型号 from 仓库出入库明细表 with (NOLOCK)
                    left join   基础数据物料信息表   on 基础数据物料信息表.物料编码= 仓库出入库明细表.物料编码
                    where  仓库出入库明细表.物料编码='{0}' and  仓库出入库明细表.出入库时间>='{1}' and 仓库出入库明细表.出入库时间<='{2}' 
                    order by 出入库时间 desc,出库入库 asc", s, barEditItem1.EditValue, dtime);

            }
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            //dtM.Columns.Remove("GUID");
            //dtM.Columns.Remove("ID");


            dtM = new DataTable();

            dtM = dt.Clone();
            dtM.Columns.Add("时点库存", typeof(decimal));

            sql = $@"select aa.物料编码,aa.仓库号,aa.仓库名称,(aa.库存总数-isnull(xx.出入数量,0))库存总数 from 仓库物料数量表 aa  
          left join (select 物料编码,SUM(实效数量) as 出入数量, 仓库号 from 仓库出入库明细表 with (NOLOCK)  where 出入库时间 > '{dtime}' group by 物料编码, 仓库号) xx
          on xx.物料编码 = aa.物料编码 and xx.仓库号 = aa.仓库号  where aa.物料编码='{s}'"; //取截止到时间点的库存
            DataTable t_库存 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

            foreach (DataRow dr in dt.Rows)
            {

                DataRow rr = dtM.NewRow();
                rr.ItemArray = dr.ItemArray;
                DataRow[] tr = t_库存.Select($"仓库号='{dr["仓库号"].ToString()}'");
                decimal dec = 0;
                if (tr.Length > 0)
                {
                    dec = Convert.ToDecimal(tr[0]["库存总数"]);
                    //先赋值后减 
                    tr[0]["库存总数"] = dec - Convert.ToDecimal(dr["实效数量"]);
                }
                rr["时点库存"] = dec;


                dtM.Rows.Add(rr);

            }
            dv = new DataView(dtM);
            gridControl1.DataSource = dv;
            //checkBox1.Checked = false;
            //checkBox2.Checked = false;
            //checkBox3.Checked = false;
            //checkBox4.Checked = false;
            //checkBox5.Checked = false;
            //checkBox6.Checked = false;
            //checkBox7.Checked = false;



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
                //gridControl1.ExportToXls(saveFileDialog.FileName, options);  
                gridControl1.ExportToXlsx(saveFileDialog.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

     
        // 采购明细
        //private void 追踪ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
        //    string ms= dr["明细类型"].ToString().Trim();
        //    string cs = dr["单号"].ToString().Trim();

        //    try
        //    {

        //        if (ms == "采购")
        //        {
        //            string name = string.Format("采购入库明细({0}_{1})", dr["物料编码"].ToString().Trim(), dr["物料名称"].ToString().Trim());
        //            ERPpurchase.frm采购入库视图 frm = new ERPpurchase.frm采购入库视图(cs);
        //            CPublic.UIcontrol.AddNewPage(frm, name);
        //            frm.Dock = DockStyle.Fill;
        //        }
        //        if (ms == "销售")
        //        {
        //            string name = string.Format("销售出库明细({0}_{1})", dr["物料编码"].ToString().Trim(), dr["物料名称"].ToString().Trim());
        //            ERPSale.frm销售记录成品出库详细界面_视图 frm = new ERPSale.frm销售记录成品出库详细界面_视图 (cs);
        //            CPublic.UIcontrol.AddNewPage(frm, name);
        //            frm.Dock = DockStyle.Fill;


        //        }
        //        //
        //        if (ms == "生产")
        //        {

        //        }
        //        //
        //        if (ms == "领料")
        //        {

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }

        //}





    }
}
