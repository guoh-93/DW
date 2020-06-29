using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CPublic;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;


namespace 郭恒的DEMO
{
    public partial class Form6 : Form
    {
        string strConn_测试 = "password={0};persist Security Info=True;User ID={1};Initial Catalog={2};Data Source={3}";
        string strcon = CPublic.Var.strConn;
        string strcon_7 = CPublic.Var.strConn;

        bool bl_delay1 = false;

        public Form6()
        {
            InitializeComponent();
            strConn_测试 = string.Format(strConn_测试, "erp", "erp1", @"ERP\MS2008", "weilai");
        }



        private void fun_()
        {
            //string sql = @"select  *  from  ";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sql = string.Format(@"select d.物料编码,SUM(总数*次数)总数,COUNT(物料编码)次数,出入库申请单号  from (select b.*,c.出入库申请单号 from
             (select[仓库出入库明细表].物料编码,单号,COUNT([仓库出入库明细表].物料编码)次数,SUM([仓库出入库明细表].实效数量)总数 from  [仓库出入库明细表],
                 (SELECT [物料编码],[物料名称],[相关单位],[出库入库],[数量],[实效数量] FROM [FMS].[dbo].[仓库出入库明细表] 
            where 实效时间 > '2016/12/1 00:00:50' and (明细类型 = '其他出库' or 明细类型 = '其他入库')
         group by [物料编码],[物料名称],[相关单位],[出库入库] ,[数量] ,[实效数量] having count([物料编码]) >= 2)  a 
         where [仓库出入库明细表].[物料编码]  = a.物料编码  and [仓库出入库明细表].[出库入库] = a.出库入库  
         and [仓库出入库明细表].[数量] = a.数量  and a.[实效数量] = [仓库出入库明细表].[实效数量]
         and a.[相关单位] = [仓库出入库明细表].[相关单位] and 实效时间 > '2016/12/01 00:00:00'
         group by   [仓库出入库明细表].物料编码,单号  )b, (select [其他出库单号],[出入库申请单号] from 其他出库子表 group by [其他出库单号],[出入库申请单号])c 
         where b.单号=c.其他出库单号  )d  group by 物料编码,出入库申请单号  order by 物料编码,出入库申请单号 ");

            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            foreach (DataRow dr in dt.Rows)
            {
                if (Convert.ToInt32(dr["次数"]) > 1)
                {
                    decimal dec = -(Convert.ToDecimal(dr["总数"]) * (Convert.ToDecimal(dr["次数"]) / 2));
                    string sql_1 = string.Format(@"update  仓库物料数量表  set 库存总数=库存总数+'{0}',有效总数=有效总数+'{1}'
                                    where 物料编码='{2}'", dec, dec, dr["物料编码"].ToString());
                    CZMaster.MasterSQL.ExecuteSQL(sql_1, strcon);


                    string sql_2 = string.Format(@"select * from 仓库出入库明细表,(select [其他出库单号],[出入库申请单号] from 其他出库子表 group by [其他出库单号],[出入库申请单号])c
		                 where 仓库出入库明细表.物料编码='{0}' and 仓库出入库明细表.单号= c.其他出库单号 and 出入库申请单号='{1}' order by 出入库时间", dr["物料编码"], dr["出入库申请单号"]);
                    DataTable dt_1 = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(sql_2, strcon))
                    {
                        da.Fill(dt_1);
                        string sql_s = string.Format(@"delete 仓库出入库明细表 where GUID='{0}'", dt_1.Rows[0]["GUID"]);

                        CZMaster.MasterSQL.ExecuteSQL(sql_s, strcon);

                    }
                }
            }

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            string sql = @"select SUM(未完成数量) as 在途量 from 采购记录采购单明细表 where 物料编码 = '{0}' and 生效 = 1
                                            and 明细完成日期 is null and 作废 = 0 and 总完成 = 0 and 生效日期 > '2016-11-01 00:00:00'";
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            if (dt.Rows[0]["在途量"].ToString() == "")
            {
                string str_ = "";
            }
            else
            {
                string str = "";
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DataTable dt_1 = new DataTable();
            DataTable dt_2 = new DataTable();

            SqlConnection conn = new SqlConnection(CPublic.Var.strConn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("生效");
            try
            {
                //{
                //    string sql = "select * from 销售记录销售订单明细表 where 1<>1";
                //    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                //    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                //    {
                //        new SqlCommandBuilder(da);
                //        da.Update(dtP);
                //    }
                //}
                //{
                //    string sql = "select * from 销售记录销售订单主表 where 1<>1";
                //    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                //    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                //    {
                //        new SqlCommandBuilder(da);
                //        da.Update(dtM);
                //    }
                //}
                ts.Commit();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw ex;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string sql = "select  * from [采购记录采购单明细表] where 采购单号 in (select  采购单号 from [采购记录采购单主表] where 采购单类型='开发采购')  ";
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            foreach (DataRow dr in dt.Rows)
            {
                fun_直接生成入库单_2(dr);
            }
        }

        private void fun_直接生成入库单_2(DataRow r)
        {

            DataTable dt = new DataTable();
            DataTable dt_mx = new DataTable();
            string sql = "select  * from 采购记录采购单入库主表 where 1<>1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            da.Fill(dt);
            string sql_mx = "select  * from 采购记录采购单入库明细 where 1<>1";
            da = new SqlDataAdapter(sql_mx, strcon);
            da.Fill(dt_mx);
            if (r.RowState == DataRowState.Deleted) return;

            DataRow dr = dt.NewRow();
            DateTime t = CPublic.Var.getDatetime();
            dr["GUID"] = System.Guid.NewGuid();
            dr["入库单号"] = string.Format("PC{0}{1:00}{2:00}{3:0000}", t.Year,t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("PC", t.Year,t.Month));
            dr["修改日期"] = System.DateTime.Now;
            dr["操作员ID"] = CPublic.Var.LocalUserID;
            dr["操作员"] = CPublic.Var.localUserName;
            dr["录入日期"] = CPublic.Var.getDatetime();
            dr["供应商ID"] = r["供应商ID"];
            dr["供应商"] = r["供应商"];
            dr["供应商负责人"] = r["供应商负责人"];
            dr["供应商电话"] = r["供应商电话"];

            dr["生效"] = true;
            dr["创建日期"] = CPublic.Var.getDatetime();
            dt.Rows.Add(dr);
            int pos = 1;

            DataRow dr_mx = dt_mx.NewRow();
            if (dr_mx["GUID"] == DBNull.Value)
            {
                dr_mx["GUID"] = System.Guid.NewGuid();
            }
            dr_mx["入库单号"] = dr["入库单号"]; //入库单号
            dr_mx["入库POS"] = 1;
            dr_mx["入库明细号"] = dr["入库单号"].ToString() + "-" + pos.ToString("00");


            dr_mx["录入日期"] = CPublic.Var.getDatetime();

            dr_mx["操作员ID"] = CPublic.Var.LocalUserID;
            dr_mx["操作员"] = CPublic.Var.localUserName;
            dr_mx["入库量"] = r["采购数量"];
            dr_mx["入库量"] = r["采购数量"];
            dr_mx["采购数量"] = r["采购数量"];
            dr_mx["采购单号"] = r["采购单号"];
            dr_mx["采购单明细号"] = r["采购明细号"];
            dr_mx["送检单号"] = "1"; //只为在采购开票可区分补开采购和 开发采购

            dr_mx["物料编码"] = r["物料编码"];
            dr_mx["物料名称"] = r["物料名称"];
            dr_mx["图纸编号"] = r["图纸编号"];
            dr_mx["规格型号"] = r["规格型号"];
            dr_mx["未税单价"] = r["未税单价"];
            dr_mx["单价"] = r["单价"];

            dr_mx["税率"] = r["税率"];

            dr_mx["未税金额"] = r["未税金额"];
            dr_mx["金额"] = r["金额"];
            dr_mx["供应商ID"] = r["供应商ID"];
            dr_mx["供应商"] = r["供应商"];
            dr_mx["供应商负责人"] = r["供应商负责人"];
            dr_mx["供应商电话"] = r["供应商电话"];
            dr_mx["生效"] = true;
            dr_mx["入库量"] = r["采购数量"];
            dr_mx["价格核实"] = false;

            dt_mx.Rows.Add(dr_mx);


            CZMaster.MasterSQL.Save_DataTable(dt, "采购记录采购单入库主表", strcon);
            CZMaster.MasterSQL.Save_DataTable(dt_mx, "采购记录采购单入库明细", strcon);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // 
            string sql = "select * from 销售导入未开票 ";    //加载所有导进来的数据
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
            DataTable dt_5 = dt.Copy();
            DataTable dt_订单主表;
            DataTable dt_订单明细;
            DataTable dt_出库主表;
            DataTable dt_出库明细;
            string sql_订单主表 = "select * from L销售记录销售订单主表L where 1=2 ";
            string sql_订单明细 = "select * from L销售记录销售订单明细表L where 1=2 ";
            string sql_出库主表 = "select * from L销售记录成品出库单主表L where 1=2 ";
            string sql_出库明细 = "select * from L销售记录成品出库单明细表L where 1=2 ";
            dt_订单主表 = CZMaster.MasterSQL.Get_DataTable(sql_订单主表, strcon);
            dt_订单明细 = CZMaster.MasterSQL.Get_DataTable(sql_订单明细, strcon);
            dt_出库主表 = CZMaster.MasterSQL.Get_DataTable(sql_出库主表, strcon);
            dt_出库明细 = CZMaster.MasterSQL.Get_DataTable(sql_出库明细, strcon);

            //L销售记录销售订单明细表L
            //L销售记录销售订单主表L
            //L销售记录成品出库单主表L
            //L销售记录成品出库单明细表L
            foreach (DataRow dr in dt.Rows)
            {

                if (dr.RowState == DataRowState.Deleted) continue;
                //L销售记录销售订单主表L
                DataRow r_订主 = dt_订单主表.NewRow();
                dt_订单主表.Rows.Add(r_订主);
                string sql_订主 = string.Format("select 订单号,客户,客户编号,sum(未开票金额)未开票金额 from 销售导入未开票  group by 订单号,客户,客户编号 "); //导进来的表
                DataTable dt_3 = CZMaster.MasterSQL.Get_DataTable(sql_订主, strcon);
                r_订主["GUID"] = System.Guid.NewGuid();
                r_订主["销售订单号"] = dt_3.Rows[0]["订单号"];
                r_订主["客户编号"] = dt_3.Rows[0]["客户编号"];
                r_订主["客户名"] = dt_3.Rows[0]["客户"];
                r_订主["录入人员"] = "批量导入";
                r_订主["税后金额"] = dt_3.Rows[0]["未开票金额"];

                //L销售记录销售订单明细表L 和 L出库明细L

                DataRow[] r_同订单 = dt.Select(string.Format("订单号='{0}'", dr["订单号"]));

                for (int i = 0; i < r_同订单.Length; i++)    //寻找相同订单 生成相应订单明细 和出库明细
                {

                    DataRow r_订明细 = dt_订单明细.NewRow();
                    dt_订单明细.Rows.Add(r_订明细);
                    r_订明细["GUID"] = System.Guid.NewGuid();
                    r_订明细["销售订单号"] = r_同订单[i]["订单号"];
                    r_订明细["POS"] = i + 1;
                    r_订明细["销售订单明细号"] = r_同订单[i]["订单号"] + "-" + (i + 1).ToString();
                    //物料编码
                    string sql_1 = string.Format("select  * from 基础数据物料信息表 where 原ERP物料编号='{0}'", r_同订单[i]["物料编码"]);
                    DataTable dt_1 = new DataTable();
                    dt_1 = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);

                    if (dt_1.Rows.Count > 0) r_订明细["物料编码"] = dt_1.Rows[0]["物料编码"];


                    r_订明细["数量"] = r_同订单[i]["未开票数量"];
                    r_订明细["物料名称"] = r_同订单[i]["名称"];
                    r_订明细["n原ERP规格型号"] = r_同订单[i]["规格"];
                    r_订明细["计量单位"] = r_同订单[i]["计量单位"];
                    r_订明细["n原ERP规格型号"] = r_同订单[i]["规格"];
                    r_订明细["客户"] = r_同订单[i]["客户"];
                    r_订明细["客户编号"] = r_同订单[i]["客户编号"];

                    r_订明细["生效"] = true;
                    r_订明细["生效日期"] = CPublic.Var.getDatetime();
                    r_订明细["税后单价"] = r_同订单[i]["单价"];
                    r_订明细["税前单价"] = Convert.ToDecimal(r_同订单[i]["单价"]) / Convert.ToDecimal(1.17);
                    r_订明细["税前金额"] = Convert.ToDecimal(r_订明细["税前单价"]) * Convert.ToDecimal(r_订明细["数量"]);
                    r_订明细["税后金额"] = r_同订单[i]["未开票金额"];

                    DataRow r_出库明细 = dt_出库明细.NewRow();
                    dt_出库明细.Rows.Add(r_出库明细);
                    r_出库明细["GUID"] = System.Guid.NewGuid();
                    r_出库明细["成品出库单号"] = r_同订单[i]["送货单号"];
                    r_出库明细["POS"] = i + 1;
                    r_出库明细["成品出库单明细号"] = r_同订单[i]["送货单号"] + "-" + (i + 1).ToString();
                    r_出库明细["销售订单号"] = r_订明细["销售订单号"];
                    r_出库明细["销售订单明细号"] = r_订明细["销售订单明细号"];
                    r_出库明细["出库通知单号"] = r_同订单[i]["通知单号"];
                    r_出库明细["出库通知单明细号"] = r_同订单[i]["通知单号"] + "-" + (i + 1).ToString();
                    //物料编码
                    if (dt_1.Rows.Count > 0) r_出库明细["物料编码"] = dt_1.Rows[0]["物料编码"];

                    r_出库明细["物料名称"] = r_同订单[i]["名称"];
                    r_出库明细["出库数量"] = r_同订单[i]["未开票数量"];
                    r_出库明细["已出库数量"] = r_同订单[i]["未开票数量"];
                    r_出库明细["未开票数量"] = r_同订单[i]["未开票数量"];
                    r_出库明细["n原ERP规格型号"] = r_同订单[i]["规格"];
                    r_出库明细["客户编号"] = r_同订单[i]["客户编号"];
                    r_出库明细["客户"] = r_同订单[i]["客户"];
                    r_出库明细["备注1"] = r_同订单[i]["单据类型"];

                    r_出库明细["生效"] = true;
                    r_出库明细["生效日期"] = CPublic.Var.getDatetime();
                    //dt.Rows.Remove(r_同订单[i]);
                    r_同订单[i].Delete();
                }

            }
            //出库主


            string sql_出主 = string.Format("select 送货单号,客户,客户编号 from 销售导入未开票  group by 送货单号,客户,客户编号 "); //导进来的表
            DataTable dt_4 = CZMaster.MasterSQL.Get_DataTable(sql_出主, strcon);
            foreach (DataRow r in dt_4.Rows)
            {
                DataRow r_出主 = dt_出库主表.NewRow();
                dt_出库主表.Rows.Add(r_出主);
                r_出主["GUID"] = System.Guid.NewGuid();
                r_出主["成品出库单号"] = r["送货单号"];

                r_出主["客户"] = r["客户"];
                r_出主["操作员"] = "批量导入";
                r_出主["生效"] = true;
                r_出主["生效日期"] = CPublic.Var.getDatetime();
            }




            CZMaster.MasterSQL.Save_DataTable(dt_出库主表, "L销售记录成品出库单主表L", strcon);
            CZMaster.MasterSQL.Save_DataTable(dt_订单主表, "L销售记录销售订单主表L", strcon);
            CZMaster.MasterSQL.Save_DataTable(dt_出库明细, "L销售记录成品出库单明细表L", strcon);
            CZMaster.MasterSQL.Save_DataTable(dt_订单明细, "L销售记录销售订单明细表L", strcon);

            MessageBox.Show("ok");

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length >= 4)
            {
                string sql = string.Format("select n原ERP规格型号 from 基础数据物料信息表 where 物料编码 like '{0}%'", textBox1.Text);
                DataTable dt = new DataTable();
                dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                listBox1.Items.Clear();
                foreach (DataRow dr in dt.Rows)
                {
                    listBox1.Items.Add(dr["n原ERP规格型号"]);
                }
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            barEditItem1.EditValue = "";
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            string sql_cun = "select * from [客户付款记录表] where 1=2";
            DataTable dt_存 = new DataTable();
            dt_存 = CZMaster.MasterSQL.Get_DataTable(sql_cun, strcon);
            string sql = "select * from [科目余额表] ";   //载入所有记录
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            foreach (DataRow dr in dt.Rows)
            {
                DataRow r = dt_存.NewRow();
                DateTime dtime = CPublic.Var.getDatetime();
                //隔月导
                dtime = new DateTime(dtime.Year, dtime.Month, 1);
                dtime = dtime.AddSeconds(-1);
                //dtime = new DateTime(dtime.Year, dtime.Month, 20);
                string str_流水号 = CPublic.CNo.fun_得到最大流水号("FD", dtime.Year, dtime.Month).ToString("000");
                r["流水号"] = dtime.Year.ToString("00") + dtime.Month.ToString("00") + dtime.Day.ToString("00") + "-" + str_流水号;
                r["单号"] = dtime.Year.ToString("00") + "-" + dtime.Month.ToString("00") + "-" + str_流水号;  //改为存入 财务导入时自带的编号！2017/8/9
                r["客户"] = dr["客户名称"].ToString();
                r["客户编号"] = dr["客户编号"].ToString();
                r["录入日期"] = r["操作日期"] = r["付款日期"] = dr["付款日期"].ToString();
                //r["金额"] = dr["本月到款"].ToString();
                r["金额"] = dr["总金额"].ToString();
                r["模具金额"] = dr["模具金额"].ToString();
                r["货款金额"] = dr["货款金额"].ToString();
                r["其他金额"] = dr["其他金额"].ToString();


                r["录入人员"] = "吴艳妃";
                r["工号"] = "8209";

                dt_存.Rows.Add(r);
            }
            SqlDataAdapter da = new SqlDataAdapter(sql_cun, strcon);
            new SqlCommandBuilder(da);
            da.Update(dt_存);
            MessageBox.Show("ok");
        }
        DataTable dt_待办;



        private void button5_Click_1(object sender, EventArgs e)
        {
            string sql = @"select 销售记录成品出库单明细表.销售订单明细号,原ERP物料编号,销售记录销售订单明细表.n原ERP规格型号,税前单价,(税前单价*出库数量)税前金额,税后单价,(税后单价*出库数量)税后金额 
                            from 销售记录成品出库单明细表,销售记录销售订单明细表,基础数据物料信息表
                           where 销售记录成品出库单明细表.销售订单明细号= 销售记录销售订单明细表.销售订单明细号  and 销售记录销售订单明细表.物料编码= 基础数据物料信息表.物料编码
                              and 销售记录成品出库单明细表.生效 = 1 and (未开票数量 > 0  or (备注1<>'' and 未开票数量<0)) and 销售记录成品出库单明细表.作废=0";
            dt_待办 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn);
            da.Fill(dt_待办);
            gridControl1.DataSource = dt_待办;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsExportOptions options = new DevExpress.XtraPrinting.XlsExportOptions();



               // DevExpress.XtraGrid.GridControl gc = (ActiveControl) as DevExpress.XtraGrid.GridControl;

              gridControl1.ExportToXlsx(saveFileDialog.FileName);



                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
          //  timer1.Start();
        }
        private void fun_config_printer()
        {
            string str_printer箱贴;
            string str_printer小标签;

            StreamReader sr = new StreamReader(Application.StartupPath + string.Format(@"\打印机配置.txt"), Encoding.Default);
            string s;
            int i = 0;
            while ((s = sr.ReadLine()) != null)
            {
                if (i == 0)
                {
                    str_printer箱贴 = s;

                }
                else if (i == 1)
                {
                    str_printer小标签 = s;
                }
                //else
                //{
                //    break;
                //}
                //i++;
            }
            sr.Close();
        }
        private void button8_Click(object sender, EventArgs e)
        {
            //string root = @"E:\\FCFiles\";
            //string path ="2017\10\31\61eb1317-9626-4022-9621-c69378f52040";
            string s = "PDC710280";
            s = "401509" + s.Substring(s.Length - 6, 6);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string sql = string.Format("update 仓库出入库明细表 set 实效时间='2016-11-30',出入库时间='2016-11-30' where 明细号='{0}'", textBox2.Text);
            CZMaster.MasterSQL.ExecuteSQL(sql, strcon);
            sql = string.Format("update 其他入库子表 set 生效日期='2016-11-30' where 其他入库明细号='{0}'", textBox2.Text);
            CZMaster.MasterSQL.ExecuteSQL(sql, strcon);
            sql = string.Format("select * from 其他入库子表  where 其他入库明细号='{0}'", textBox2.Text);

            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            sql = string.Format("update 其他出入库申请主表 set 申请日期='2016-11-30' where 出入库申请单号='{0}'", dt.Rows[0]["出入库申请单号"]);
            CZMaster.MasterSQL.ExecuteSQL(sql, strcon);
            sql = string.Format("update 其他出入库申请子表 set 生效日期='2016-11-30',完成日期='2016-11-30' where 出入库申请明细号='{0}'", dt.Rows[0]["出入库申请明细号"]);
            CZMaster.MasterSQL.ExecuteSQL(sql, strcon);

            MessageBox.Show("ok");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            string sql = @"select 客户名称 from 销售客户期初期末值 where 客户编号='' group by 客户名称";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                da.Fill(dt);
            }
            string s = "select * FROM 客户基础信息表 where 1<>1";
            DataTable dt_客户 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            int i = 1;
            foreach (DataRow dr in dt.Rows)
            {
                DataRow x = dt_客户.NewRow();
                x["客户编号"] = "IM" + i.ToString("000000");
                x["客户名称"] = dr["客户名称"];

                dt_客户.Rows.Add(x);
                i++;

            }
            using (SqlDataAdapter da = new SqlDataAdapter(s, strcon))
            {
                new SqlCommandBuilder(da);
                da.Update(dt_客户);
            }
            MessageBox.Show("ok");
        }




        private void timer1_Tick(object sender, EventArgs e)
        {
            //DateTime t_now = System.DateTime.Now;
            //DateTime t1 = new DateTime(t_now.Year, t_now.Month, 1);
            //DateTime t_today = new DateTime(t_now.Year, t_now.Month, t_now.Day);

            //t1 = t1.AddMonths(1).AddDays(-1).AddHours(23);    //本月最后一天晚上十一点          

            //DateTime t2 = t_today.AddHours(23);            // 今天 晚上十一点
            //if (bl_delay1 == false)    // 每周日晚 晚十一点 和 每月末晚十一点 自动备份 财务所需的 车间在制品
            //{
            //    if ((t_now > t1 && t_now < t1.AddMinutes(5)) || (t2.DayOfWeek.ToString() == "Sunday" && t_now > t2 && t_now < t2.AddMinutes(5)))   //满足 本月最后一天23点过后 或者 大于本天的 23点;满足 未运行过状态
            //    {
            //        timer1.Enabled = false;
            //        bl_delay1 = true;
            //        if (t_now > t1) //每月最后一天  备份借用未归还数据,仓库库存  只有月末要备份 周末不用 
            //        {
            //            string file = @"C:\\errorlog.txt";

            //            if (File.Exists(file) == true)
            //            {

            //                using (StreamWriter SW = File.AppendText(file))
            //                {
            //                    SW.WriteLine(t_now);
            //                    SW.Close();
            //                }
            //            }
            //            else
            //            {
            //                FileStream myFs = new FileStream(file, FileMode.Create);
            //                StreamWriter mySw = new StreamWriter(myFs);
            //                mySw.Write(t_now);
            //                mySw.Close();
            //                myFs.Close();
            //            }

            //        }

            //        timer1.Enabled = true;
            //    }

            //}
            //if (t_now > t_today.AddHours(23).AddMinutes(5))    //每天 晚上11点 零五分  重置状态
            //{
            //    bl_delay1 = false;
            //    timer1.Enabled = true;
            //}
        }

        private void fun_计算数据()
        {
            DataTable dtP = new DataTable();
            dtP.Columns.Add("产品编码");
            dtP.Columns.Add("子项编码");
            dtP.Columns.Add("耗用量", typeof(decimal));
            dtP.Columns.Add("年");


            //加载 所有 成品 
            string sql = string.Format(@"    select  父项物料代码,SUM(入库数量)入库数量,a.产品线,明细类,物料编码  
         from  [{0}产量统计] a,基础数据物料信息表  where 父项物料代码=原ERP物料编号 
         group  by 父项物料代码 ,a.产品线,明细类,物料编码  ", textBox3.Text);
            DataTable dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            int year=Convert.ToInt32(textBox3.Text);
            foreach (DataRow dr in dtM.Rows)
            {
                string s = string.Format(@" with locs(产品编码,子项编码,数量) as(
                select 产品编码,子项编码,数量 FROM 基础数据物料BOM表 WHERE 产品编码='{0}'  
                union all
                select  a.产品编码,a.子项编码,a.数量 from 基础数据物料BOM表 A,locs B,基础数据物料信息表 c where A.产品编码 = B.子项编码 and A.产品编码=c.物料编码 and c.大类<>'微型断路器及附件' and c.大类<>'微型断路器' )
                select 产品编码,子项编码,数量 from locs", dr["物料编码"].ToString());
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                //加载出 该物料 所有层级子项 
                foreach (DataRow r in dt.Rows)
                {
                    
                    DataRow r_1 = dtP.NewRow();
                    r_1["产品编码"] = dr["物料编码"].ToString();
                    r_1["子项编码"] = r["子项编码"].ToString();
                    r_1["年"] = year;
                    if (r["产品编码"].ToString() == dr["物料编码"].ToString())
                    {
                        r_1["耗用量"] = Convert.ToDecimal(dr["入库数量"]) * Convert.ToDecimal(r["数量"]);
                    }
                    else
                    {
                        ////先去 dtp 中判断 是否已经添加过了    因为 如果 某子项有 多个上级 在else 中数量已经循环相加了
                        //DataRow[] x = dtP.Select(string.Format("子项编码='{0}'", r["子项编码"]).ToString());
                        //if (x.Length > 0) continue;

                        decimal a = Convert.ToDecimal(r["数量"]);
                        a = fun_dg(a, r, dt);
                        r_1["耗用量"] =Convert.ToDecimal(dr["入库数量"])*a;
 
                    }
                    dtP.Rows.Add(r_1);

                }
            }

            //CZMaster.MasterSQL.Save_DataTable(dtP,"Lsheet",strcon);
            gridControl1.DataSource = dtP;


        }
        private decimal fun_dg(decimal a, DataRow r, DataTable dt)
        {

          
            DataRow[] xr = dt.Select(string.Format("子项编码='{0}'", r["产品编码"].ToString()));
            decimal b = 0;
            foreach (DataRow x in xr)
            {
                a = a * Convert.ToDecimal(x["数量"]);
                a  =fun_dg(a, x, dt);
                b = b + a;
            }
            if (b == 0) b = a;
            return b;

        }
        private void button11_Click(object sender, EventArgs e)
        {

            fun_计算数据();


        }
    }
}
