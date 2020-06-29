using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
namespace IAACA
{
    public partial class ui_出入库汇总_财务 : UserControl
    {

        string strcon = CPublic.Var.strConn;
        DataTable dtM;
        string cfgfilepath = "";
        string strcon_u8 = CPublic.Var.geConn("DW");
        string PZ_u8_暂估 = "";
        string PZ_erp_暂估 = "";

        public ui_出入库汇总_财务()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = fun_check();

                fun_search(sql); //生产领料 
                fun_search_other(sql);  //  '形态转换出库','形态转换入库','拆单申请出库','拆单申请入库'
                fun_search_材料出库(sql);
                fun_search_其他入库(sql);
                fun_search_销售(sql);
                fun_search_调拨(sql);
                fun_search_ww();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private string fun_check()
        {
            string s = "";

            DateTime t1 = Convert.ToDateTime(dateEdit1.EditValue);
            t1 = new DateTime(t1.Year, t1.Month, t1.Day);
            DateTime t2 = Convert.ToDateTime(dateEdit2.EditValue).Date.AddDays(1).AddSeconds(-1);
            t2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);
            s = string.Format("  and  出入库时间> '{0}' and 出入库时间<= '{1}'", t1, t2);
            //if (checkBox1.Checked == true)
            //{

            //    if (checkedComboBoxEdit1.EditValue == null || checkedComboBoxEdit1.EditValue.ToString() == "")
            //    {
            //        throw new Exception("未选择出入库类型");
            //    }
            //    else
            //    {
            //        string xx = checkedComboBoxEdit1.EditValue.ToString();
            //        string[] ss = xx.Split(',');
            //        s += " and 明细类型 in (";
            //        foreach (string xs in ss)
            //        {
            //            s += "'" + xs.Trim() + "',";
            //        }
            //        s = s.Substring(0, s.Length - 1) + ")";
            //    }

            //}

            if (checkBox5.Checked == true)
            {
                if (searchLookUpEdit5.EditValue.ToString() == "")
                {
                    throw new Exception("未选择物料");
                }
                else
                {
                    s += string.Format(" and 物料编码 ='{0}'", searchLookUpEdit5.EditValue);
                }
            }
            if (checkBox6.Checked == true)
            {
                if (textBox1.Text.ToString() == "")
                {
                    throw new Exception("未填写单号");
                }
                else
                {
                    s += string.Format(" and 相关单号 like '%{0}%'", textBox1.Text);
                }
            }

            return s;
        }

        DataTable dt_生产领料;
        private void fun_search(string s)
        {
            string sql = "";
            sql = string.Format(@" select 明细类型,出库入库,明细号,相关单号,物料类型,a.仓库名称,a.仓库号, (-1*实效数量)实效数量,出入库时间
                ,b.规格型号,a.物料名称,a.相关单位,a.物料编码,a.单价,round(a.单价*(-1*实效数量),2) as 金额 ,b.计量单位,班组,
                 gd.物料编码 as 产品编码,gd.物料名称 as 产品名称,a.科目编码,a.科目名称 ,kk.成本科目编码,kk.成本科目名称   from  仓库出入库明细表 a 
                 left join 基础数据物料信息表 b on  a.物料编码=b.物料编码   
                 left join 生产记录生产工单表 gd   on gd.生产工单号=相关单号
                left join [科目_生产成本] kk on kk.存货编码=LEFT(gd.物料编码,LEN(kk.存货编码))
                 where  明细类型 in ('领料出库','工单关闭退料','返工退料','工单退料','入库倒冲') {0}", s);
            dt_生产领料 = new DataTable();
            // DataTable dt_rwf = new DataTable();
            dt_生产领料 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            gridControl1.DataSource = dt_生产领料;
            // gridControl1.MainView.PopulateColumns();
            // string str="明细类型,大类,小类,相关单位,物料名称,物料编码,规格型号,n核算单价,计量单位,仓库名称" ;
            //dt_rwf=dset.SelectGroupByInto("",dtM,str+",sum(实效数量) 数量,sum(金额) 金额","",str);
            //gridControl3.DataSource = dt_rwf;
        }
        DataTable dt_other;
        private void fun_search_other(string s)
        {
            //  '形态转换出库','形态转换入库','拆单申请出库','拆单申请入库'
            string sql = string.Format(@"   select 明细类型,出库入库,明细号,相关单号,物料类型,a.仓库名称,a.仓库号,abs(实效数量)实效数量,出入库时间,b.规格型号,b.物料名称
  ,a.相关单位,a.物料编码,a.单价,round(a.单价*abs(实效数量),2) as 金额 ,b.计量单位,a.科目编码,a.科目名称,cc.对方科目编码,cc.对方科目名称 from  仓库出入库明细表 a 
  left join 基础数据物料信息表 b on  a.物料编码=b.物料编码  
  left join (select  收发类别编码,收发类别名称,存货分类编码,存货分类名称,对方科目编码,对方科目名称  from 科目对应关系包含部门 
        where  存货分类编码 <>'' and 收发类别名称 ='调拨出库'  group by 收发类别编码,收发类别名称,存货分类编码,存货分类名称,对方科目编码,对方科目名称
         )cc  on cc.存货分类编码=left(b.物料编码,len(cc.存货分类编码))
  where  明细类型 in ('形态转换出库','形态转换入库','拆单申请出库','拆单申请入库')  {0}", s);
            dt_other = new DataTable();
            // DataTable dt_rwf = new DataTable();
            dt_other = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            gridControl3.DataSource = dt_other;
            //gridControl1.MainView.PopulateColumns();

        }
        //
        DataTable dt_材料出库;
        private void fun_search_材料出库(string s)
        {
            // ------------- 材料出库   这里财务要 出库是正 入库是负 
            string sql = string.Format(@"select a.明细类型,出库入库,明细号,相关单号,物料类型,a.仓库名称,a.仓库号,(-1*实效数量)实效数量,出入库时间,项目名称,qtm.操作人员,qtm.备注 as 申请表头备注 
    ,b.规格型号,a.物料名称,部门名称,a.物料编码,结算单价 as 单价,round(结算单价*(-1*实效数量),2) as 金额,b.计量单位,原因分类,qtm.红字回冲,业务单号,a.科目编码
    ,a.科目名称,存货分类编码,供应商  from  仓库出入库明细表 a 
                 left join 基础数据物料信息表 b on  a.物料编码=b.物料编码   
                 left join 其他出库子表  qmx on qmx.其他出库明细号 =明细号
                 left join 其他出入库申请主表 qtm on qtm.出入库申请单号 =相关单号 
                 left join 采购记录采购单明细表 cmx on 采购明细号=qmx.备注
                 where  a.明细类型 in ('其他出库','材料出库') and 原因分类 <>'调拨出库' {0}", s);
            dt_材料出库 = new DataTable();
            // DataTable dt_rwf = new DataTable();
            dt_材料出库 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);



            gridControl2.DataSource = dt_材料出库;
            //gridControl1.MainView.PopulateColumns();
            // string str="明细类型,大类,小类,相关单位,物料名称,物料编码,规格型号,n核算单价,计量单位,仓库名称" ;
            //dt_rwf=dset.SelectGroupByInto("",dtM,str+",sum(实效数量) 数量,sum(金额) 金额","",str);
            //gridControl3.DataSource = dt_rwf;
        }

        DataTable dt_其他入库;
        private void fun_search_其他入库(string s)
        {
            // ------------- 
            string sql = string.Format(@"select 明细类型,出库入库,明细号,相关单号,物料类型,a.仓库名称,a.仓库号, 实效数量,出入库时间
            ,b.规格型号,a.物料名称,qtm.部门名称,a.物料编码,结算单价 as 单价,round(结算单价*实效数量,2) as 金额,b.计量单位,原因分类,a.科目编码,a.科目名称 
               ,对方科目编码,对方科目名称  from  仓库出入库明细表 a 
                 left join 基础数据物料信息表 b on  a.物料编码=b.物料编码   
                 left join 其他入库子表  qmx on qmx.其他入库明细号 =明细号
                 left join 其他出入库申请主表 qtm on qtm.出入库申请单号 =相关单号 
                 left join (select  收发类别编码,收发类别名称,存货分类编码,存货分类名称,对方科目编码,对方科目名称 
                 , 部门编码, 部门名称  from 科目对应关系包含部门 where 收发类别名称 in ('盘盈','拆旧入库') )xx on 原因分类=收发类别名称
                 where  明细类型='其他入库'and 原因分类 <>'调拨入库'   {0}", s);
            dt_其他入库 = new DataTable();
            // DataTable dt_rwf = new DataTable();
            dt_其他入库 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            gridControl4.DataSource = dt_其他入库;
            //gridControl1.MainView.PopulateColumns();
            // string str="明细类型,大类,小类,相关单位,物料名称,物料编码,规格型号,n核算单价,计量单位,仓库名称" ;
            //dt_rwf=dset.SelectGroupByInto("",dtM,str+",sum(实效数量) 数量,sum(金额) 金额","",str);
            //gridControl3.DataSource = dt_rwf;
        }


        DataTable dt_销售;
        private void fun_search_销售(string s)
        {
            // ------------- 
            string sql = string.Format(@" select 明细类型, 出库入库, 明细号, 相关单号, 物料类型, a.仓库名称,a.仓库号,-1*实效数量 as 实效数量
     ,出入库时间,b.规格型号,a.物料名称,a.相关单位,a.物料编码,发出单价 as 单价,round(发出单价* -1 *实效数量,2) as 金额 
     ,b.计量单位,出库通知单号 ,a.科目编码,a.科目名称,对方科目编码,对方科目名称   from  仓库出入库明细表 a
   left join 基础数据物料信息表 b on a.物料编码=b.物料编码
   left join 销售记录成品出库单明细表 sa on sa.成品出库单明细号= 明细号
   left join 其他出入库申请主表 qtm on qtm.出入库申请单号 = 相关单号 
   left join (select  收发类别编码,收发类别名称,存货分类编码,存货分类名称,对方科目编码,对方科目名称  from 科目对应关系包含部门 
 where  存货分类编码 <>'' and 收发类别名称 ='销售出库'  group by 收发类别编码,收发类别名称,存货分类编码,存货分类名称,对方科目编码,对方科目名称)cc
 on  cc.存货分类编码=left(b.物料编码,len(cc.存货分类编码))
     where  明细类型 in ('销售出库','销售退货') {0}", s);
            dt_销售 = new DataTable();
            // DataTable dt_rwf = new DataTable();
            dt_销售 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            gridControl5.DataSource = dt_销售;
            //gridControl1.MainView.PopulateColumns();
            // string str="明细类型,大类,小类,相关单位,物料名称,物料编码,规格型号,n核算单价,计量单位,仓库名称" ;
            //dt_rwf=dset.SelectGroupByInto("",dtM,str+",sum(实效数量) 数量,sum(金额) 金额","",str);
            //gridControl3.DataSource = dt_rwf;
        }

        /// <summary>

        /// </summary>
        /// <param name="s"></param>
        DataTable dt_调拨;
        private void fun_search_调拨(string s)
        {
            DateTime t1 = Convert.ToDateTime(dateEdit1.EditValue);
            t1 = new DateTime(t1.Year, t1.Month, t1.Day);
            DateTime t2 = Convert.ToDateTime(dateEdit2.EditValue).Date.AddDays(1).AddSeconds(-1);
            t2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);
            // ------------- 
            string sql = string.Format(@" select  a.科目编码,a.科目名称,xx.* from  仓库出入库明细表 a 
            inner join 
                   (select  c.*,r.其他入库明细号,base.物料名称,base.规格型号,base.存货分类,ROUND(数量*c.单价,2) 金额,对方科目编码,对方科目名称     from (
                    select  a.其他出库明细号,物料编码,业务单号,a.数量,结算单价 as 单价,a.生效日期  from 其他出库子表 a 
                    left join 其他出入库申请主表 b on a.出入库申请单号 =b.出入库申请单号   
                    where a.生效日期>'{0}' and a.生效日期<'{1}' and  原因分类='调拨出库'   ) c 
                    left join 
                    ( select  a.其他入库明细号,业务单号,物料编码,结算单价  from 其他入库子表 a 
                    left join 其他出入库申请主表 b on a.出入库申请单号 =b.出入库申请单号   
                    where a.生效日期>'{0}' and a.生效日期<'{1}' and  原因分类='调拨入库')r
                       on r.业务单号=c.业务单号 and c.物料编码=r.物料编码 
                    left join 基础数据物料信息表 base on base.物料编码=c.物料编码 
                    left join (select  收发类别编码,收发类别名称,存货分类编码,存货分类名称,对方科目编码,对方科目名称  from 科目对应关系包含部门 
        where  存货分类编码 <>'' and 收发类别名称 ='调拨出库'  group by 收发类别编码,收发类别名称,存货分类编码,存货分类名称,对方科目编码,对方科目名称
         )cc  on cc.存货分类编码=left(base.物料编码,len(cc.存货分类编码)))xx     on xx.其他出库明细号=a.明细号  ", t1, t2);
            dt_调拨 = new DataTable();
            // DataTable dt_rwf = new DataTable();
            dt_调拨 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl7.DataSource = dt_调拨;
            //gridControl1.MainView.PopulateColumns();
            // string str="明细类型,大类,小类,相关单位,物料名称,物料编码,规格型号,n核算单价,计量单位,仓库名称" ;
            //dt_rwf=dset.SelectGroupByInto("",dtM,str+",sum(实效数量) 数量,sum(金额) 金额","",str);
            //gridControl3.DataSource = dt_rwf;
        }
        DataTable dt_ww;
        private void fun_search_ww()
        {
            DateTime t1 = Convert.ToDateTime(dateEdit1.EditValue);
            t1 = new DateTime(t1.Year, t1.Month, t1.Day);
            DateTime t2 = Convert.ToDateTime(dateEdit2.EditValue).Date.AddDays(1).AddSeconds(-1);
            t2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);
            // ------------- 
            string sql = string.Format(@"    select   a.入库单号,a.采购单号,a.物料编码,a.物料名称,a.规格型号,检验记录单号,送检单号,a.采购数量,入库量,委外核销,仓库ID,a.仓库名称, 
  c.未税单价 as 采购不含税单价, ROUND(c.未税单价*入库量,2)加工费,材料费,a.生效日期 as 入库日期,cc.对方科目编码,cc.对方科目名称,b.供应商   from 采购记录采购单入库明细 a
 left join 采购记录采购单主表 b on a.采购单号=b.采购单号 
 left join 采购记录采购单明细表 c on c.采购明细号=a.采购单明细号
 left join (select  入库单号,sum(isnull(材料费,null))材料费 from (
 select  a.*,b.结算单价,ROUND( 物料核销数*结算单价,2) 材料费 from 委外核销明细表 a
 left join 其他出库子表  b on  a.其他出库明细号=b.其他出库明细号 
 where 核销日期>'{0}' and 核销日期<'{1}')x  group by 入库单号)x  on x.入库单号=a.入库单号
   left join (select  收发类别编码,收发类别名称,存货分类编码,存货分类名称,对方科目编码,对方科目名称  from 科目对应关系包含部门 
 where  存货分类编码 <>'' and 收发类别名称 ='采购入库'  group by 收发类别编码,收发类别名称,存货分类编码,存货分类名称,对方科目编码,对方科目名称)cc
 on  cc.存货分类编码=left(c.物料编码,len(cc.存货分类编码))
 where a.生效日期 >'{0}' and a.生效日期<'{1}' and 采购单类型='委外采购'", t1, t2);
            dt_ww = new DataTable();
            // DataTable dt_rwf = new DataTable();
            dt_ww = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            gridControl8.DataSource = dt_ww;

        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (ActiveControl != null && ActiveControl.GetType().Equals(gridControl1.GetType()))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                    DevExpress.XtraGrid.GridControl gc = (ActiveControl) as DevExpress.XtraGrid.GridControl;

                    gc.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            else
            {

                MessageBox.Show("若要导出请先选中要导出的表格");
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void ui_出入库汇总_财务_Load(object sender, EventArgs e)
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
                x.UserLayout(this.xtraTabControl1, this.Name, cfgfilepath);
                DateTime dtime = CPublic.Var.getDatetime().AddMonths(-1);
                dtime = new DateTime(dtime.Year, dtime.Month, 1);
                dateEdit1.EditValue = dtime;
                dateEdit2.EditValue = dtime.AddMonths(1).AddSeconds(-1);
                textBox2.Text = dtime.Year.ToString();
                textBox3.Text = dtime.Month.ToString();
                textBox5.Text = dtime.Year.ToString();
                textBox4.Text = dtime.Month.ToString();
                textBox7.Text = dtime.Year.ToString();
                textBox6.Text = dtime.Month.ToString();
                dateEdit3.EditValue = dtime;
                dateEdit4.EditValue = dtime.AddMonths(1).AddSeconds(-1);


                simpleButton4_Click(null, null);


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        //截至 暂估
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                int year = Convert.ToInt32(textBox2.Text);
                int month = Convert.ToInt32(textBox3.Text);
                DateTime t = new DateTime(year, month, 1).AddMonths(1);
                //  string 
                //2020-4-20 数据库新建一个计划 每月最后一天 备份 借用未归还 和 每月底暂估
                string s = $"select  * from [每月暂估] where 年={year} and 月={month}";
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                gridControl6.DataSource = dt;
                if(dt.Rows.Count==0)
                {
                    s = "exec Pro_Estimation '" + t + "'";
                    dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    gridControl6.DataSource = dt;
                }
                string ERP_凭证号 = "";
                string U8_凭证号 = "";
                string x = $"select  * from 财务凭证表 where  年={year} and 月={month} and 摘要 like  '%截止{year}年{month}月底暂估%'";
                DataTable t_erp = CZMaster.MasterSQL.Get_DataTable(x, strcon);
                if (t_erp.Rows.Count > 0)
                {
                    ERP_凭证号 = "ERP凭证号:" + t_erp.Rows[0]["凭证号"].ToString();
                    //4-22 u8再搜一遍确认一样
                    x = $"select * from GL_accvouch where iyear={year} and iperiod={month} and cdigest like '%{year}年{month}月底暂估%'";
                    DataTable t_u8 = CZMaster.MasterSQL.Get_DataTable(x, strcon_u8);


                    //U8_凭证号 = t_erp.Rows[0]["U8凭证号"].ToString();
                    if (t_u8.Rows.Count > 0) U8_凭证号 =  " U8凭证号:"+t_u8.Rows[0]["ino_id"].ToString();

                }
                label13.Text = ERP_凭证号 + U8_凭证号;
                PZ_u8_暂估 = U8_凭证号;
                PZ_erp_暂估 = ERP_凭证号;

 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            { e.Handled = true; }

        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            { e.Handled = true; }

        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Clicks == 2)
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                Form1 fm = new Form1(dr);
                fm.ShowDialog();
                if (fm.flag)
                {
                    string s = string.Format("select  * from 仓库出入库明细表 where 明细号='{0}' ", dr["明细号"]);
                    DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    t.Rows[0]["单价"] = fm.de_单价;
                    CZMaster.MasterSQL.Save_DataTable(t, "仓库出入库明细表", strcon);
                    dr["单价"] = fm.de_单价;
                    dr["金额"] = Math.Round(fm.de_单价 * Convert.ToDecimal(dr["实效数量"]), 2, MidpointRounding.AwayFromZero);

                    dr.AcceptChanges();
                }
            }
        }

        private void gridView2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Clicks == 2)
            {
                DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                Form1 fm = new Form1(dr, dr["原因分类"].ToString(), dr["部门名称"].ToString());
                fm.ShowDialog();
                if (fm.flag)
                {
                    string s = string.Format("select  * from 其他出库子表 where 其他出库明细号='{0}' ", dr["明细号"]);
                    DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    if (fm.de_单价 != -1)
                    {
                        t.Rows[0]["结算单价"] = fm.de_单价;
                        dr["单价"] = fm.de_单价;
                        dr["金额"] = Math.Round(fm.de_单价 * Convert.ToDecimal(dr["实效数量"]), 2, MidpointRounding.AwayFromZero);
                    }

                    string ss = string.Format("select  * from 其他出入库申请主表 where 出入库申请单号='{0}' ", dr["相关单号"]);
                    DataTable tt = CZMaster.MasterSQL.Get_DataTable(ss, strcon);
                    tt.Rows[0]["原因分类"] = fm.s_原因;
                    tt.Rows[0]["部门名称"] = fm.s_部门;
                    dr["原因分类"] = fm.s_原因;

                    string sx = string.Format("select  * from 仓库出入库明细表  where 明细号='{0}' ", dr["明细号"]);
                    DataTable t_出入明细 = CZMaster.MasterSQL.Get_DataTable(sx, strcon);
                    t_出入明细.Rows[0]["相关单位"] = fm.s_部门;
                    dr["部门名称"] = fm.s_部门;

                    dr.AcceptChanges();


                    SqlConnection conn = new SqlConnection(strcon);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("r"); //事务的名称
                    SqlCommand cmd1 = new SqlCommand(s, conn, ts);
                    SqlCommand cmd2 = new SqlCommand(ss, conn, ts);
                    SqlCommand cmd3 = new SqlCommand(sx, conn, ts);

                    try
                    {
                        SqlDataAdapter da;
                        da = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da);
                        da.Update(t);
                        da = new SqlDataAdapter(cmd2);
                        new SqlCommandBuilder(da);
                        da.Update(tt);
                        da = new SqlDataAdapter(cmd3);
                        new SqlCommandBuilder(da);
                        da.Update(t_出入明细);
                        ts.Commit();
                    }
                    catch
                    {
                        ts.Rollback();
                        throw new Exception("出错了,请重试");
                    }
                }
            }
        }

        private void gridView3_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Clicks == 2)
            {
                DataRow dr = gridView3.GetDataRow(gridView3.FocusedRowHandle);
                Form1 fm = new Form1(dr);
                fm.ShowDialog();
                if (fm.flag)
                {
                    string s = string.Format("select  * from 仓库出入库明细表 where 明细号='{0}' ", dr["明细号"]);
                    DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    t.Rows[0]["单价"] = fm.de_单价;
                    CZMaster.MasterSQL.Save_DataTable(t, "仓库出入库明细表", strcon);
                    dr["单价"] = fm.de_单价;
                    dr["金额"] = Math.Round(fm.de_单价 * Convert.ToDecimal(dr["实效数量"]), 2, MidpointRounding.AwayFromZero);

                    dr.AcceptChanges();
                }
            }

        }

        private void gridView4_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Clicks == 2)
            {
                DataRow dr = gridView4.GetDataRow(gridView4.FocusedRowHandle);
                Form1 fm = new Form1(dr);
                fm.ShowDialog();
                if (fm.flag)
                {
                    string s = string.Format("select  * from 其他入库子表 where 其他入库明细号='{0}' ", dr["明细号"]);
                    DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    t.Rows[0]["结算单价"] = fm.de_单价;
                    CZMaster.MasterSQL.Save_DataTable(t, "其他入库子表", strcon);
                    dr["单价"] = fm.de_单价;
                    dr["金额"] = Math.Round(fm.de_单价 * Convert.ToDecimal(dr["实效数量"]), 2, MidpointRounding.AwayFromZero);

                    dr.AcceptChanges();
                }
            }
        }

        private void gridView5_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Clicks == 2)
            {
                DataRow dr = gridView5.GetDataRow(gridView5.FocusedRowHandle);
                Form1 fm = new Form1(dr);
                fm.ShowDialog();
                if (fm.flag)
                {
                    string s = string.Format("select  * from 销售记录成品出库单明细表 where 成品出库单明细号='{0}' ", dr["明细号"]);
                    DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    t.Rows[0]["发出单价"] = fm.de_单价;
                    CZMaster.MasterSQL.Save_DataTable(t, "销售记录成品出库单明细表", strcon);
                    dr["单价"] = fm.de_单价;
                    dr["金额"] = Math.Round(fm.de_单价 * Convert.ToDecimal(dr["实效数量"]), 2, MidpointRounding.AwayFromZero);

                    dr.AcceptChanges();
                }
            }
        }

        private void gridView7_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Clicks == 2)
                {
                    DataRow dr = gridView7.GetDataRow(gridView7.FocusedRowHandle);
                    Form1 fm = new Form1(dr);
                    fm.ShowDialog();
                    if (fm.flag)
                    {
                        string s = string.Format("select  * from 其他出库子表 where 其他出库明细号='{0}' ", dr["其他出库明细号"]);
                        DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                        t.Rows[0]["结算单价"] = fm.de_单价;
                        string ss = string.Format("select  * from 其他入库子表 where 其他入库明细号='{0}' ", dr["其他入库明细号"]);
                        DataTable tt = CZMaster.MasterSQL.Get_DataTable(ss, strcon);
                        tt.Rows[0]["结算单价"] = fm.de_单价;
                        SqlConnection conn = new SqlConnection(strcon);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("r"); //事务的名称
                        SqlCommand cmd1 = new SqlCommand(s, conn, ts);
                        SqlCommand cmd2 = new SqlCommand(ss, conn, ts);
                        try
                        {
                            SqlDataAdapter da;
                            da = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da);
                            da.Update(t);
                            da = new SqlDataAdapter(cmd2);
                            new SqlCommandBuilder(da);
                            da.Update(tt);
                            ts.Commit();

                            dr["单价"] = fm.de_单价;
                            dr["金额"] = Math.Round(fm.de_单价 * Convert.ToDecimal(dr["数量"]), 2, MidpointRounding.AwayFromZero);
                            dr.AcceptChanges();
                        }
                        catch
                        {
                            ts.Rollback();
                            throw new Exception("出错了,请重试");
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
                int year = Convert.ToInt32(textBox5.Text);
                int month = Convert.ToInt32(textBox4.Text);


                DateTime t = new DateTime(year, month, 1).AddMonths(1);

                string s = string.Format(@"select  tt.*,base.物料名称,规格型号  from C_存货核算物料单价表 tt
                   left join 基础数据物料信息表 base on base.物料编码 = tt.物料编码
                    where 年 = {0}  and 月 = {1}", year, month);
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                gridControl9.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            try
            {
                int year = Convert.ToInt32(textBox7.Text);
                int month = Convert.ToInt32(textBox6.Text);
                DateTime t = new DateTime(year, month, 1);
                DateTime t2 = t.AddMonths(1);
                string sql = string.Format(@"select  明细号,a.相关单号 生产工单号,a.物料编码 as 产品编码,实效数量 数量,出入库时间 as 入库日期,a.仓库号
             ,a.仓库名称,生产工单类型
             ,base.物料名称,base.规格型号,入库单价,round(入库单价*实效数量,2) 入库金额,科目编码,科目名称,kk.成本科目编码 ,kk.成本科目名称 from  仓库出入库明细表 a 
             left join [C_工单]  b on a.相关单号=b.生产工单号 and 年={0} and 月 ={1}  
             left join 基础数据物料信息表 base on base.物料编码=a.物料编码
             left join [科目_生产成本] kk on kk.存货编码=LEFT(base.物料编码,LEN(kk.存货编码))
             where 明细类型='生产入库' and 出入库时间 >'{2}' and 出入库时间 <'{3}'  ", year, month, t, t2);
                DataTable dt = new DataTable();
                // DataTable dt_rwf = new DataTable();
                dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

                gridControl10.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //匹配科目
        private void simpleButton5_Click(object sender, EventArgs e)
        {

            try
            {
                DateTime t1 = Convert.ToDateTime(dateEdit1.EditValue);
                t1 = new DateTime(t1.Year, t1.Month, t1.Day);
                DateTime t2 = Convert.ToDateTime(dateEdit2.EditValue).Date.AddDays(1).AddSeconds(-1);
                t2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);

                DataTable dt_save = new DataTable();
                string ppp = string.Format("select * from 仓库出入库明细表 where 出入库时间>'{0}' and 出入库时间 <'{1}' " +
                    "and 明细类型 in ('材料出库','其他入库','其他出库','形态转换入库','形态转换出库', '拆单申请入库','拆单申请出库')  ", t1, t2);
                dt_save = CZMaster.MasterSQL.Get_DataTable(ppp, strcon);

                //'领料出库', '工单关闭退料', '返工退料', '工单退料', '入库倒冲','销售出库' 形态转换入库 形态转换出库 拆单申请入库 拆单申请出库
                //foreach (DataRow dr in dt_生产领料.Rows)
                //{
                //    if (dr["科目编码"].ToString().Trim() != "") continue;

                //    DataRow[] r = dt_save.Select(string.Format("明细号='{0}'", dr["明细号"]));
                //    r[0]["科目编码"] = dr["科目编码"] = dr["成本科目编码"];
                //    r[0]["科目名称"] = dr["科目名称"] = dr["成本科目名称"];
                //}
                //dt_生产领料.AcceptChanges();

                string sql_scll = string.Format(@"update 仓库出入库明细表 set 科目编码=成本科目编码,科目名称= 成本科目名称
           from (
           select 明细类型,出库入库,明细号,相关单号,物料类型,a.仓库名称,a.仓库号, (-1*实效数量)实效数量,出入库时间
                ,b.规格型号,a.物料名称,a.相关单位,a.物料编码,a.单价,round(a.单价*(-1*实效数量),2) as 金额 ,b.计量单位,班组,
                 gd.物料编码 as 产品编码,gd.物料名称 as 产品名称,a.科目编码,a.科目名称 ,kk.成本科目编码,kk.成本科目名称   from  仓库出入库明细表 a 
                 left join 基础数据物料信息表 b on  a.物料编码=b.物料编码   
                 left join 生产记录生产工单表 gd   on gd.生产工单号=相关单号
                left join [科目_生产成本] kk on kk.存货编码=LEFT(gd.物料编码,LEN(kk.存货编码))
                 where  明细类型 in ('领料出库','工单关闭退料','返工退料','工单退料','入库倒冲') 
                 and  出入库时间 >'{0}'     and  出入库时间 <'{1}')ll where ll.明细号=仓库出入库明细表.明细号 and 仓库出入库明细表.科目编码='' ", t1, t2);
                CZMaster.MasterSQL.ExecuteSQL(sql_scll, strcon);
                //sql_scll = string.Format("  and  出入库时间> '{0}' and 出入库时间<= '{1}' ", t1, t2);
                //fun_search(sql_scll);



                string s_调拨 = string.Format(@" update  仓库出入库明细表  set 科目编码= 对方科目编码,科目名称=对方科目名称
                  from 
                   (select  c.*,r.其他入库明细号,base.物料名称,base.规格型号,base.存货分类,ROUND(数量*c.单价,2) 金额,对方科目编码,对方科目名称  from (
                    select  a.其他出库明细号,物料编码,业务单号,a.数量,结算单价 as 单价,a.生效日期  from 其他出库子表 a 
                    left join 其他出入库申请主表 b on a.出入库申请单号 =b.出入库申请单号   
                    where a.生效日期>'{0}' and a.生效日期<'{1}' and  原因分类='调拨出库'   ) c 
                    left join 
                    ( select  a.其他入库明细号,业务单号,物料编码,结算单价  from 其他入库子表 a 
                    left join 其他出入库申请主表 b on a.出入库申请单号 =b.出入库申请单号   
                    where a.生效日期>'{0}' and a.生效日期<'{1}' and  原因分类='调拨入库')r
                       on r.业务单号=c.业务单号 and c.物料编码=r.物料编码 
                    left join 基础数据物料信息表 base on base.物料编码=c.物料编码 
                    left join (select  收发类别编码,收发类别名称,存货分类编码,存货分类名称,对方科目编码,对方科目名称  from 科目对应关系包含部门 
        where  存货分类编码 <>'' and 收发类别名称 ='调拨出库'  group by 收发类别编码,收发类别名称,存货分类编码,存货分类名称,对方科目编码,对方科目名称
         )cc  on cc.存货分类编码=left(base.物料编码,len(cc.存货分类编码)))xx   where  xx.其他出库明细号=仓库出入库明细表.明细号  and 仓库出入库明细表.科目编码 =''", t1, t2);
                CZMaster.MasterSQL.ExecuteSQL(s_调拨, strcon);

                //foreach (DataRow dr in dt_调拨.Rows)
                //{
                //    if (dr["科目编码"].ToString().Trim() != "") continue;

                //    DataRow[] r = dt_save.Select(string.Format("明细号='{0}'", dr["其他出库明细号"]));
                //    r[0]["科目编码"] = dr["科目编码"] = dr["对方科目编码"];
                //    r[0]["科目名称"] = dr["科目名称"] = dr["对方科目名称"];
                //}
                //dt_调拨.AcceptChanges();


                string s_sale = string.Format(@"update 仓库出入库明细表   set 科目编码= 对方科目编码,科目名称=对方科目名称
         from(       
          select 明细类型, 出库入库, 明细号, 相关单号, 物料类型, a.仓库名称,a.仓库号,-1*实效数量 as 实效数量
     ,出入库时间,b.规格型号,a.物料名称,a.相关单位,a.物料编码,发出单价 as 单价,round(发出单价* -1 *实效数量,2) as 金额 
     ,b.计量单位,出库通知单号 ,a.科目编码,a.科目名称,对方科目编码,对方科目名称   from  仓库出入库明细表 a
   left join 基础数据物料信息表 b on a.物料编码=b.物料编码
   left join 销售记录成品出库单明细表 sa on sa.成品出库单明细号= 明细号
   left join 其他出入库申请主表 qtm on qtm.出入库申请单号 = 相关单号 
   left join (select  收发类别编码,收发类别名称,存货分类编码,存货分类名称,对方科目编码,对方科目名称  from 科目对应关系包含部门 
 where  存货分类编码 <>'' and 收发类别名称 ='销售出库'  group by 收发类别编码,收发类别名称,存货分类编码,存货分类名称,对方科目编码,对方科目名称)cc
 on  cc.存货分类编码=left(b.物料编码,len(cc.存货分类编码))
     where  明细类型 in ('销售出库','销售退货') and  出入库时间 >'{0}'  and  出入库时间 <'{1}')xx 
     where xx.明细号=仓库出入库明细表.明细号  and 仓库出入库明细表.科目编码 =''", t1, t2);

                CZMaster.MasterSQL.ExecuteSQL(s_sale, strcon);

                //foreach (DataRow dr in dt_销售.Rows)
                //{
                //    if (dr["科目编码"].ToString().Trim() != "") continue;

                //    DataRow[] r = dt_save.Select(string.Format("明细号='{0}'", dr["明细号"]));
                //    r[0]["科目编码"] = dr["科目编码"] = dr["对方科目编码"];
                //    r[0]["科目名称"] = dr["科目名称"] = dr["对方科目名称"];
                //}
                //dt_销售.AcceptChanges();

                foreach (DataRow dr in dt_other.Rows)
                {
                    if (dr["科目编码"].ToString().Trim() != "") continue;

                    DataRow[] r = dt_save.Select(string.Format("明细号='{0}'", dr["明细号"]));
                    r[0]["科目编码"] = dr["科目编码"] = dr["对方科目编码"];
                    r[0]["科目名称"] = dr["科目名称"] = dr["对方科目名称"];
                }
                dt_other.AcceptChanges();

                foreach (DataRow dr in dt_其他入库.Rows)
                {
                    if (dr["科目编码"].ToString().Trim() != "") continue;

                    DataRow[] r = dt_save.Select(string.Format("明细号='{0}'", dr["明细号"]));
                    r[0]["科目编码"] = dr["科目编码"] = dr["对方科目编码"];
                    r[0]["科目名称"] = dr["科目名称"] = dr["对方科目名称"];
                }
                dt_其他入库.AcceptChanges();

                //材料出库和 其他入库
                string s = @"select  收发类别编码,收发类别名称,存货分类编码,存货分类名称,对方科目编码,对方科目名称 
          , 部门编码, 部门名称  from 科目对应关系包含部门 where 收发类别名称 in ('办公领用','盘亏','')  or 存货分类名称 is null ";
                DataTable t_对应 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                foreach (DataRow dr in dt_材料出库.Rows)
                {
                    if (dr["科目编码"].ToString().Trim() != "") continue;

                    if (dr["原因分类"].ToString() == "办公领用")
                    {
                        DataRow[] tt = t_对应.Select(string.Format("收发类别名称='{0}' and 存货分类编码='{1}'  and 部门名称='{2}'",
                            dr["原因分类"], dr["存货分类编码"], dr["部门名称"]));
                        if (tt.Length > 0)
                        {
                            DataRow[] r = dt_save.Select(string.Format("明细号='{0}'", dr["明细号"]));
                            r[0]["科目编码"] = dr["科目编码"] = tt[0]["对方科目编码"];
                            r[0]["科目名称"] = dr["科目名称"] = tt[0]["对方科目名称"];
                        }
                    }
                    else
                    {
                        DataRow[] tt = t_对应.Select(string.Format("收发类别名称='{0}'", dr["原因分类"]));
                        if (tt.Length > 0)
                        {
                            DataRow[] r = dt_save.Select(string.Format("明细号='{0}'", dr["明细号"]));
                            r[0]["科目编码"] = dr["科目编码"] = tt[0]["对方科目编码"];
                            r[0]["科目名称"] = dr["科目名称"] = tt[0]["对方科目名称"];
                        }
                    }
                }
                dt_材料出库.AcceptChanges();

                using (SqlDataAdapter da = new SqlDataAdapter(ppp, strcon))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt_save);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime t1 = Convert.ToDateTime(dateEdit3.EditValue);
                t1 = new DateTime(t1.Year, t1.Month, t1.Day);
                DateTime t2 = Convert.ToDateTime(dateEdit4.EditValue).Date.AddDays(1).AddSeconds(-1);
                string s = string.Format(@"select 采购单明细号,a.入库单号,采购单类型,b.供应商,b.税率,c.物料名称 as  加工入库物料名称,经办人,c.物料编码 as 加工入库物料,c.规格型号 as 加工入库物料规格,a.采购数量,入库量 , 
                a.生效日期 as 入库日期,d.其他出库明细号,e.物料编码  as 委外物料,e.物料名称 as 委外物料名称,e.规格型号 as 委外物料规格
                ,物料核销数,核销日期,核销人员,g.数量 as 出库数量,(g.数量-g.委外已核量)该单未核物料数 ,f.原因分类 as 出库原因,结算单价    from [采购记录采购单入库明细] a    left join 采购记录采购单主表 b on a.采购单号=b.采购单号   
                left join 基础数据物料信息表 c on c.物料编码=a.物料编码      left  join 委外核销明细表  d  on  d.入库单号=a.入库单号
                left join 其他出库子表 g on g.其他出库明细号=d.其他出库明细号 
                left join 其他出入库申请主表 f on g.出入库申请单号=f.出入库申请单号 
                left join 基础数据物料信息表 e on e.物料编码=d.子项编码  where  采购单类型='委外采购' and a.作废=0 and 核销日期>'{0}' and 核销日期<'{1}'", t1, t2);
                DataTable dt_核销明细 = new DataTable();
                dt_核销明细 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                gc_核销明细.DataSource = dt_核销明细;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //生成凭证
        private void simpleButton7_Click(object sender, EventArgs e)
        {
            try
            {
                string[] arr = check_pz();
                Pz_生成(arr[0], arr[1]);



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        private void Pz_生成(string ERP_凭证号, string U8_凭证号)
        {
            DateTime t_now = CPublic.Var.getDatetime();

            int year = Convert.ToInt32(textBox2.Text);
            int month = Convert.ToInt32(textBox3.Text);
            DateTime time = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);



            DataTable t_明细 = ((DataView)gridView6.DataSource).ToTable();

            string x = $"select  * from 财务凭证表 where  年={year} and 月={month} and 摘要 like  '截止{year}年{month}月底暂估'";
            DataTable t_erp = CZMaster.MasterSQL.Get_DataTable(x, strcon);


            //这边需要根据名称 去u8搜一下供应商编码  因为这边可能编码不一样
            string gys = "select cvencode,cVenName from Vendor ";
            DataTable dt_gys = CZMaster.MasterSQL.Get_DataTable(gys, strcon_u8);
            string str_gys = dt_gys.Rows[0]["cvencode"].ToString();
            MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();

            DataTable dt_凭证 = RBQ.SelectGroupByInto("", t_明细, "科目编码,科目名称,sum(暂估金额) 不含税金额 ", "", "科目编码,科目名称");

            DataTable dt_供应商 = RBQ.SelectGroupByInto("", t_明细, "供应商,sum(暂估金额) 不含税金额 ", "", "供应商");
            //匹配该供应商在u8中的编码
            dt_供应商.Columns.Add("编码");
            foreach (DataRow r_gys in dt_供应商.Rows)
            {
                DataRow[] rr_gys = dt_gys.Select($"cVenName='{r_gys["供应商"].ToString()}'");
                if (rr_gys.Length == 0) throw new Exception("因本系统与U8用该名称不一致,未找到供应商编号,请将两个供应商名称一致");
                else r_gys["编码"] = rr_gys[0]["cvencode"].ToString();
            }
            string s = $"select * from GL_accvouch where iyear={year} and iperiod={month} and ino_id='{U8_凭证号}'";
            DataTable dt_u8 = CZMaster.MasterSQL.Get_DataTable(s, strcon_u8);
            if (U8_凭证号 != "") //已有数据 需要把原来的先删除 再增加
            {
                for (int l = dt_u8.Rows.Count - 1; l >= 0; l--)
                {
                    dt_u8.Rows[l].Delete();
                }
                for (int j = t_erp.Rows.Count - 1; j >= 0; j--)
                {
                    t_erp.Rows[j].Delete();
                }
            }
            else
            {
                string xx = string.Format("select isnull(MAX(ino_id),0) 凭证号 from GL_accvouch where iyear={0} and iperiod={1}", year, month);
                DataRow pzh = CZMaster.MasterSQL.Get_DataRow(xx, strcon_u8);
                U8_凭证号 = (Convert.ToInt32(pzh[0]) + 1).ToString();
            }
            int i = 1; //行号
                       //这里新增的是 按科目汇总后的开票明细
            foreach (DataRow r_pz in dt_凭证.Rows)
            {
                DataRow r = dt_u8.NewRow();
                r["iperiod"] = month;
                r["csign"] = "记";
                r["isignseq"] = 1;
                r["ino_id"] = U8_凭证号;
                r["inid"] = i;
                r["dbill_date"] = time;
                r["idoc"] = -1;
                r["bdelete"] = 0;
                r["bvouchedit"] = 1; //可修改
                r["bvouchAddordele"] = 0; //bvouchAddordele 是否可增删
                r["bvouchmoneyhold"] = 0; //凭证合计金额是否保值 
                r["bvalueedit"] = 1; //分录数值是否可修改 
                r["bcodeedit"] = 1; //分录科目是否可修改  
                r["bPCSedit"] = 1; //分录往来项是否可修改   
                r["bDeptedit"] = 1; //分录部门是否可修改    
                r["bItemedit"] = 1; //分录项目是否可修改 
                r["bCusSupInput"] = 0; //分录往来项是否必输  

                r["cbill"] = CPublic.Var.localUserName;
                r["ctext1"] = ERP_凭证号;
                string digest1 = $"截止{year}年{month}月底暂估";
                r["cdigest"] = digest1;


                // ctext1里面存放我们的凭证号
                r["ccode"] = r_pz["科目编码"];
                r["md"] = r_pz["不含税金额"];
                r["ccodeexch_equal"] = r["ccode_equal"] = "22020102"; //对应的都是暂估应付材料款
                                                                      //r["coutaccset"] = "008";
                r["doutbilldate"] = time;
                r["RowGuid"] = System.Guid.NewGuid();
                r["iyear"] = year;
                r["iYPeriod"] = year.ToString() + month.ToString("00");
                r["tvouchtime"] = t_now;
                dt_u8.Rows.Add(r);

                DataRow r_erp = t_erp.NewRow();
                r_erp["凭证号"] = ERP_凭证号;
                r_erp["U8凭证号"] = U8_凭证号;
                r_erp["inid"] = i;
                r_erp["摘要"] = digest1;
                r_erp["制单日期"] = t_now;
                r_erp["制单人"] = CPublic.Var.localUserName;
                r_erp["年"] = year;
                r_erp["月"] = month;
                r_erp["科目编号"] = r_pz["科目编码"];
                r_erp["科目名称"] = r_pz["科目名称"];
                r_erp["借方金额"] = r_pz["不含税金额"];
                //r_erp["单据号"] = txt_kaipiaotzd.Text.Trim();
                t_erp.Rows.Add(r_erp);
                i++;
            }
            //科目明细项已经增加进去了 还要增加 暂估应付材料款 和 贷方金额的总金额
            //20-4-22 财务要求 暂估应付材料款 根据 有几个供应商 增加几行进项税 

            string exch = "";
            int int_ex = 1;
            foreach (DataRow exr in dt_u8.Rows)
            {
                if (exr.RowState == DataRowState.Deleted) continue;
                if ((exch + exr["ccode"].ToString()).Length > 50)
                    break;
                else
                    exch = exch + exr["ccode"];
                if (exch.Length == 50) break;
                if (int_ex++ != dt_u8.Rows.Count) exch = exch + ",";
            }
            #region  贷方 暂估应付材料款
            foreach (DataRow r_jxs in dt_供应商.Rows)
            {
                DataRow r1 = dt_u8.NewRow();
                r1["iperiod"] = month;
                r1["csign"] = "记";
                r1["isignseq"] = 1;
                r1["ino_id"] = U8_凭证号;
                r1["inid"] = i;
                r1["dbill_date"] = time;
                r1["bdelete"] = 0;
                r1["bvouchedit"] = 1; //可修改
                r1["bvouchAddordele"] = 0; //bvouchAddordele 是否可增删
                r1["bvouchmoneyhold"] = 0; //凭证合计金额是否保值 
                r1["bvalueedit"] = 1; //分录数值是否可修改 
                r1["bcodeedit"] = 1; //分录科目是否可修改  
                r1["bPCSedit"] = 1; //分录往来项是否可修改   
                r1["bDeptedit"] = 1; //分录部门是否可修改    
                r1["bItemedit"] = 1; //分录项目是否可修改 
                r1["bCusSupInput"] = 0; //分录往来项是否必输  
                r1["idoc"] = -1;
                r1["cbill"] = CPublic.Var.localUserName;
                r1["ctext1"] = ERP_凭证号;

                string digest1 = $"截止{year}年{month}月底暂估"; ;

                r1["cdigest"] = digest1;
                r1["ccode"] = "22020102"; //暂估应付材料款
                r1["mc"] = Math.Round(Convert.ToDecimal(r_jxs["不含税金额"]), 2, MidpointRounding.AwayFromZero);
                r1["ccodeexch_equal"] = r1["ccode_equal"] = exch;

                r1["csup_id"] = r_jxs["编码"];

                r1["doutbilldate"] = time;
                r1["RowGuid"] = System.Guid.NewGuid();
                r1["iyear"] = year;
                r1["iYPeriod"] = year.ToString() + month.ToString("00");
                r1["tvouchtime"] = t_now;
                dt_u8.Rows.Add(r1);


                DataRow r_erp1 = t_erp.NewRow();
                r_erp1["凭证号"] = ERP_凭证号;
                r_erp1["U8凭证号"] = U8_凭证号;
                r_erp1["inid"] = i;
                r_erp1["摘要"] = digest1;
                r_erp1["制单日期"] = t_now;
                r_erp1["制单人"] = CPublic.Var.localUserName;
                r_erp1["年"] = year;
                r_erp1["月"] = month;
                r_erp1["科目编号"] = "22020102";
                r_erp1["科目名称"] = "暂估应付材料款";

                r_erp1["贷方金额"] = Math.Round(Convert.ToDecimal(r_jxs["不含税金额"]), 2, MidpointRounding.AwayFromZero);
                //r_erp1["单据号"] = txt_kaipiaotzd.Text.Trim();
                t_erp.Rows.Add(r_erp1);
                i++;
            }
            #endregion

            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("zgpz");

            SqlCommand cmd = new SqlCommand(x, conn, ts);

            try
            {

                SqlDataAdapter da;
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(t_erp);
                CZMaster.MasterSQL.Save_DataTable(dt_u8, "GL_accvouch", strcon_u8);
                ts.Commit();
                MessageBox.Show("生成凭证成功");

            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw new Exception(ex.Message);
            }

        }

        private string[] check_pz()
        {

            DataTable t = ((DataView)gridView6.DataSource).ToTable();
            foreach (DataRow dr in t.Rows)
            {
                if (dr["科目编码"] == null || dr["科目编码"].ToString() == "")
                {
                    throw new Exception("存在科目编码为空请检查");
                }
                if (dr["科目名称"] == null || dr["科目名称"].ToString() == "")
                {
                    throw new Exception("存在科目名称为空请检查");
                }
            }
            int year = Convert.ToInt32(textBox2.Text);
            int month = Convert.ToInt32(textBox3.Text);

            string sql = $"select count(*)xx from 仓库月出入库结转表 where 年={year} and 月={month}  ";
            DataRow r_temp = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
            if (Convert.ToInt32(r_temp[0]) > 0)
            {
                throw new Exception($"{year}年{month}月已结账,不可生成凭证");
            }
            bool bl = true;
            if (label13.Text.Trim() != "")
            {
                if (MessageBox.Show("该月已存在凭证是否重新生成？", "询问", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    bl = false;
                }
            }
            if (!bl) throw new Exception("已取消操作");

            string[] s_arr = new string[2];
          
            string ERP_凭证号 = "";
            string U8_凭证号 = "";




            string x = $"select  * from 财务凭证表 where  年={year} and 月={month} and 摘要 like  '截止{year}年{month}月底暂估'";
            DataTable t_erp = CZMaster.MasterSQL.Get_DataTable(x, strcon);
            if (t_erp.Rows.Count == 0)
            {
                ERP_凭证号 = CPublic.CNo.fun_得到最大流水号("PZ", year, month).ToString();
            }
            else
            {
                //4-22 u8再搜一遍确认一样
                x = $"select * from GL_accvouch where iyear={year} and iperiod={month} and cdigest like '%{year}年{month}月底暂估'%";
                DataTable t_u8 = CZMaster.MasterSQL.Get_DataTable(x, strcon_u8);
                ERP_凭证号 = t_erp.Rows[0]["凭证号"].ToString();
                //U8_凭证号 = t_erp.Rows[0]["U8凭证号"].ToString();
                if (t_u8.Rows.Count > 0) U8_凭证号 = t_u8.Rows[0]["ino_id"].ToString();

            }

            s_arr[0] = ERP_凭证号;
            s_arr[1] = U8_凭证号;

            return s_arr;
        }

        private void simpleButton8_Click(object sender, EventArgs e)
        {
            try
            {
                if (PZ_u8_暂估 == "" && PZ_erp_暂估=="") throw new Exception("没有凭证可删,请确认"); 
                int year = Convert.ToInt32(textBox2.Text);
                int month = Convert.ToInt32(textBox3.Text);
                string sql = $"select count(*)xx from 仓库月出入库结转表 where 年={year} and 月={month}  ";
                DataRow r_temp = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
                if (Convert.ToInt32(r_temp[0]) > 0)
                {
                    throw new Exception($"{year}年{month}月已结账不可删除");
                }

                if (MessageBox.Show("是否确认删除凭证？", "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string s = $"delete GL_accvouch where iyear={year} and iperiod={month} and ino_id='{PZ_u8_暂估}'";
                    CZMaster.MasterSQL.ExecuteSQL(s, strcon);
                    s = $@"delete  财务凭证表 where 凭证号='{PZ_erp_暂估}' and 年='{year}' and 月='{month}' ";
                    CZMaster.MasterSQL.ExecuteSQL(s, strcon);
                    MessageBox.Show("凭证已删除");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
