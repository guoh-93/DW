using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class ui订单完成情况 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {
        #region 变量
        string strcon = CPublic.Var.strConn;
        DataTable dtM = new DataTable();
        DataTable dtP = new DataTable();


        #endregion 
        public ui订单完成情况()
        {
            InitializeComponent();
            dateEdit12.EditValue = dateEdit1.EditValue = Convert.ToDateTime(System.DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd"));
            dateEdit11.EditValue = dateEdit2.EditValue = Convert.ToDateTime(System.DateTime.Today.ToString("yyyy-MM-dd"));

        }

#pragma warning disable IDE1006 // 命名样式
        private void ui订单完成情况_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            fun_load();
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format(@"select 客户编号,客户名称 from 客户基础信息表");
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            DataTable dt_客户 = new DataTable();
            da.Fill(dt_客户);
            searchLookUpEdit1.Properties.DataSource = dt_客户;
            searchLookUpEdit1.Properties.DisplayMember = "客户名称";
            searchLookUpEdit1.Properties.ValueMember = "客户编号";

            string sql_车间 = @"select 属性值 as 生产车间,属性字段1 as 车间编号 from [基础数据基础属性表] where 属性类别='课室' 
            and 属性值 like '制造_课%' and 属性字段1 <>''";
            DataTable dt_车间 = new DataTable();
            SqlDataAdapter da_车间 = new SqlDataAdapter(sql_车间, strcon);
            da_车间.Fill(dt_车间);
            searchLookUpEdit2.Properties.DataSource = dt_车间;
            searchLookUpEdit2.Properties.ValueMember = "车间编号";
            searchLookUpEdit2.Properties.DisplayMember = "生产车间";
            searchLookUpEdit5.Properties.DataSource = dt_车间;
            searchLookUpEdit5.Properties.ValueMember = "车间编号";
            searchLookUpEdit5.Properties.DisplayMember = "生产车间";
            string sql_计划员 = @"  select  员工号,姓名 from  人事基础员工表 where 班组='计划课' and 在职状态='在职' ";
            DataTable dt_计划员 = new DataTable();
            SqlDataAdapter da_计划员 = new SqlDataAdapter(sql_计划员, strcon);
            da_计划员.Fill(dt_计划员);
            searchLookUpEdit3.Properties.DataSource = dt_计划员;
            searchLookUpEdit3.Properties.ValueMember = "员工号";
            searchLookUpEdit3.Properties.DisplayMember = "姓名";
            searchLookUpEdit4.Properties.DataSource = dt_计划员;
            searchLookUpEdit4.Properties.ValueMember = "员工号";
            searchLookUpEdit4.Properties.DisplayMember = "姓名";
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {
            if (checkBox1.Checked == true)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择客户");
                }
            }


            if (checkBox3.Checked == true)
            {
                if (dateEdit3.EditValue == null || dateEdit4.EditValue == null || dateEdit3.EditValue.ToString() == "" || dateEdit4.EditValue.ToString() == "")
                {
                    throw new Exception("未选择要求交货日期");
                }

            }

            if (checkBox2.Checked == true)
            {
                if (searchLookUpEdit2.EditValue == null || searchLookUpEdit2.EditValue.ToString() == "")
                {
                    throw new Exception("未选择车间");
                }

            }
            if (checkBox4.Checked == true)
            {
                if (dateEdit5.EditValue == null || dateEdit6.EditValue == null || dateEdit5.EditValue.ToString() == "" || dateEdit6.EditValue.ToString() == "")
                {
                    throw new Exception("未选择计划确认日期");
                }

            }
            if (checkBox5.Checked == true)
            {
                if (searchLookUpEdit3.EditValue == null || searchLookUpEdit3.EditValue.ToString() == "")
                {
                    throw new Exception("未选择计划员");
                }

            }

        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_check_1()
#pragma warning restore IDE1006 // 命名样式
        {

            if (checkBox8.Checked == true)
            {
                if (searchLookUpEdit5.EditValue == null || searchLookUpEdit5.EditValue.ToString() == "")
                {
                    throw new Exception("未选择车间");
                }

            }
            if (checkBox7.Checked == true)
            {
                if (dateEdit7.EditValue == null || dateEdit8.EditValue == null || dateEdit7.EditValue.ToString() == "" || dateEdit8.EditValue.ToString() == "")
                {
                    throw new Exception("未选择计划确认日期");
                }

            }
            if (checkBox6.Checked == true)
            {
                if (searchLookUpEdit4.EditValue == null || searchLookUpEdit4.EditValue.ToString() == "")
                {
                    throw new Exception("未选择计划员");
                }

            }

        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_search()
#pragma warning restore IDE1006 // 命名样式
        {
            string str_条件 = "";
            if (checkBox1.Checked == true)
            {
                str_条件 = str_条件 + string.Format(" and 客户编号='{0}'", searchLookUpEdit1.EditValue.ToString());
            }

            if (checkBox2.Checked == true)
            {
                str_条件 = str_条件 + string.Format(" and 车间编号='{0}'", searchLookUpEdit2.EditValue.ToString());

            }
            if (checkBox3.Checked == true)
            {
              str_条件 = str_条件 + string.Format(" and v.送达日期>='{0}' and v.送达日期<='{1}'", dateEdit3.EditValue.ToString(), Convert.ToDateTime(dateEdit4.EditValue).AddDays(1).AddSeconds(-1));
                //str_条件 = str_条件 + string.Format(" and v.预计发货日期>='{0}' and v.预计发货日期<='{1}'", dateEdit3.EditValue.ToString(), Convert.ToDateTime(dateEdit4.EditValue).AddDays(1).AddSeconds(-1));

            }
            if (checkBox4.Checked == true)
            {
                str_条件 = str_条件 + string.Format(" and e.预完工日期>='{0}' and e.预完工日期<='{1}'", dateEdit5.EditValue.ToString(), Convert.ToDateTime(dateEdit6.EditValue).AddDays(1).AddSeconds(-1));
            }
            if (checkBox5.Checked == true)
            {
                str_条件 = str_条件 + string.Format(" and e.生效人员ID='{0}' ", searchLookUpEdit3.EditValue.ToString());
            }

            //case when(aa.受订量 > isnull(s.制令量,0)) then aa.受订量 else isnull(s.制令量,0) end as 受订量a // 2019-9-9 
            string sql = string.Format(@"  select v.* , case when(v.送达日期+1>v.明细完成日期 ) then CONVERT(bit,1) else CONVERT(bit,0) end as 送货达成 
        from ( select a.销售订单号,a.销售订单明细号,b.生效日期,a.物料编码,a.物料名称,a.规格型号,数量,a.税前金额,a.税后金额,完成数量,未完成数量, a.送达日期 
     /*,计划确认日期*/,a.客户,a.客户编号,车间,车间编号,b.创建日期,a.明细完成日期,c.大类,c.小类,case when 国外=1 then '国外' else '国内' end as 国内国外
  from  销售记录销售订单明细表 a,销售记录销售订单主表 b,基础数据物料信息表 c  where a.销售订单号=b.销售订单号  and a.作废=0  and a.生效 =1
   and a.物料编码=c.物料编码 and a.关闭=0) v 
           where   left(v.物料编码,2)<>'20' {0} ", str_条件);

            /*
             select v.*,e.生产制令单号,e.完成日期,e.生效人员 as 计划员,e.预完工日期 as 计划确认日期,
                 case when(v.送达日期+1>v.明细完成日期 ) then CONVERT(bit,1) else CONVERT(bit,0) end as 送货达成,
                 case when(e.预完工日期+1>e.完成日期 or (明细完成日期 is not null and e.预完工日期 is null)) then CONVERT(bit,1) else CONVERT(bit,0) end as 制令达成  from (
                    select a.销售订单号,a.销售订单明细号,a.物料编码,a.物料名称,a.规格型号,数量,a.税前金额,a.税后金额,完成数量,未完成数量,送达日期
                      ,计划确认日期 ,a.客户,a.客户编号,车间,车间编号,b.创建日期,a.明细完成日期,c.大类,c.小类
                      from  销售记录销售订单明细表 a, 销售记录销售订单主表 b,基础数据物料信息表 c  where a.销售订单号 = b.销售订单号  and a.作废 = 0  and a.物料编码 = c.物料编码 and a.关闭 = 0) v
                              left join 生产记录生产制令子表 d on d.销售订单明细号 = v.销售订单明细号
                     left join 生产记录生产制令表 e on d.生产制令单号 = e.生产制令单号
                       where 创建日期> '{0}' and 创建日期<'{1}' and left(e.物料编码,2)<> '20'
                and(制令数量 > 0 or 制令数量 is null) and(e.关闭 = 0 or e.关闭 is null)
             */

            dtM = new DataTable();
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dtM;
            //dtM.Columns.Add("生产达成", typeof(bool));
            //foreach (DataRow dr in dtM.Rows)
            //{
            //    if (dr["制令达成"].Equals(true) || dr["送货达成"].Equals(true))
            //    {
            //        dr["生产达成"] = true;

            //    }
            //    else
            //    {
            //        dr["生产达成"] = false;

            //    }

            //}


        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_search_1()
#pragma warning restore IDE1006 // 命名样式
        {
            string str_条件 = "";


            if (checkBox8.Checked == true)
            {
                str_条件 = str_条件 + string.Format(" and 车间编号='{0}'", searchLookUpEdit5.EditValue.ToString());

            }

            if (checkBox7.Checked == true)
            {
                str_条件 = str_条件 + string.Format(" and a.预完工日期>='{0}' and a.预完工日期<='{1}'", dateEdit7.EditValue.ToString(), Convert.ToDateTime(dateEdit8.EditValue).AddDays(1).AddSeconds(-1));
                //str_条件 = str_条件 + string.Format(" and a.预计完工日期>='{0}' and a.预计完工日期<='{1}'", dateEdit7.EditValue.ToString(), Convert.ToDateTime(dateEdit8.EditValue).AddDays(1).AddSeconds(-1));
                //下面的为后加的字段
            }
            if (checkBox6.Checked == true)
            {
                str_条件 = str_条件 + string.Format(" and e.生效人员ID='{0}' ", searchLookUpEdit4.EditValue.ToString());
            }
            //case when(aa.受订量 > isnull(s.制令量,0)) then aa.受订量 else isnull(s.制令量,0) end as 受订量a
            //预完工日期 《----》预计完工日期
            string sql = string.Format(@"  select  b.物料编码,b.物料名称 ,b.规格型号,生产制令单号,预完工日期 as 计划确认日期,完成日期,a.生效人员 as 计划员,车间, 
                     case when(预完工日期+1>完成日期) then CONVERT(bit,1) else CONVERT(bit,0) end as 生产达成
                     from 生产记录生产制令表 a left join 基础数据物料信息表 b on b.物料编码=a.物料编码
                     where a.生效 = 1 and  a.生效日期>'2019-5-1' and   a.关闭=0  {0} ", str_条件);

            dtP = new DataTable();
            dtP = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            gridControl2.DataSource = dtP;



        }
#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                ERPorg.Corg.FlushMemory();
                fun_check();
                fun_search();
                decimal dec_总条数 = 0;
                decimal dec_生产 = 0;
                decimal dec_销售 = 0;
                decimal dec_x = 0;
                decimal dec_y = 0;


                DataView dv = new DataView(dtM);
                dec_总条数 = dtM.Rows.Count;
                //dv.RowFilter = "制令达成=1";
                //dec_生产 = dv.Count;
                dv = new DataView(dtM);
                dv.RowFilter = "送货达成=1";
                dec_销售 = dv.Count;

                dv = new DataView(dtM);
                // dv.RowFilter = "制令达成=1 or 送货达成=1";
                dv.RowFilter = "国内国外='国内'";
                decimal dec_国内 = dv.Count;
                dv = new DataView(dtM);
                dv.RowFilter = "国内国外='国内' and 送货达成=1";
                decimal dec_国内_达成 = dv.Count;

                dv = new DataView(dtM);
                dv.RowFilter = "国内国外='国外'";
                decimal dec_国外 = dv.Count;
                dv = new DataView(dtM);
                dv.RowFilter = "国内国外='国外' and 送货达成=1";
                decimal dec_国外_达成 = dv.Count;
                // dec_y = dv.Count;

                //dv = new DataView(dtM);
                //dv.RowFilter = "制令达成=1 and  计划员 is null";
                //dec_x = dv.Count;

                

                decimal dec_bl = 0;
                decimal dec_国外_bl = 0;
                decimal dec_国内_bl = 0;
                if (dec_总条数 > 0)
                {
                    dec_bl = dec_销售 / dec_总条数 * 100;
                    if(dec_国外>0)
                    {
                        dec_国外_bl = dec_国外_达成 / dec_国外 * 100;
                    }
                    if (dec_国内 > 0)
                    {
                        dec_国内_bl = dec_国内_达成 / dec_国内 * 100;
                    }
                }
                //label4.Text = "总条数：" + dec_总条数.ToString() + ";  国内：" + dec_国内.ToString() + ";  国外:" + dec_国外.ToString();
                //// label5.Text = "制令达成：" + dec_生产.ToString() + "  " + "未达成：" + (dec_总条数 - dec_生产).ToString();
                //label6.Text = "总送货达成：" + dec_销售.ToString() + " " + "   未达成：" + (dec_总条数 - dec_销售).ToString() + "    达成率：" + Math.Round(dec_bl,2,MidpointRounding.AwayFromZero).ToString() + "%";
                // label7.Text = "国内达成：" + Math.Round(dec_国内_bl,2,MidpointRounding.AwayFromZero).ToString()+"%" + "    国外达成:" + Math.Round(dec_国外_bl,2,MidpointRounding.AwayFromZero).ToString()+"%";

                label4.Text = "      订单总数量：" + dec_总条数.ToString() + "   "+"      按时交付订单数量：" + dec_销售.ToString() + "   " + "      订单按时交货达成率：" + Math.Round(dec_bl, 2, MidpointRounding.AwayFromZero).ToString() + "%";
                label6.Text = "国内订单总数量：" + dec_国内.ToString() + "   "+"国内按时交付订单数量：" + dec_国内_达成.ToString() +"   "+ "国内订单按时交货达成率：" + Math.Round(dec_国内_bl,2,MidpointRounding.AwayFromZero).ToString()+" % ";
                label7.Text = "国外订单总数量：" + dec_国外.ToString()+"   "+ "国外按时交付订单数量：" + dec_国外_达成.ToString() + "   "+"国外订单按时交货达成率：" + Math.Round(dec_国外_bl, 2, MidpointRounding.AwayFromZero).ToString() + "%";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void gridView7_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView2_KeyDown(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView2.GetFocusedRowCellValue(gridView2.FocusedColumn));
                e.Handled = true;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView7_KeyDown(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView7.GetFocusedRowCellValue(gridView7.FocusedColumn));
                e.Handled = true;
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DevExpress.XtraGrid.GridControl gc = (ActiveControl) as DevExpress.XtraGrid.GridControl;
            if (gc == null)
            {
                MessageBox.Show("未选择需导出哪个表格");
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions(DevExpress.XtraPrinting.TextExportMode.Text, false, false);

                gc.ExportToXlsx(saveFileDialog.FileName, options);

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }
        //制令完成情况统计查询
#pragma warning disable IDE1006 // 命名样式
        private void simpleButton2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_check_1();
                fun_search_1();
                decimal dec_总条数 = 0;
                decimal dec_生产 = 0;
                decimal dec_bl = 0;

                DataView dv = new DataView(dtP);
                dec_总条数 = dtP.Rows.Count;
                dv.RowFilter = "生产达成=1";
                dec_生产 = dv.Count;

                dec_bl = dec_生产 / dec_总条数 * 100;
                label13.Text = "生产制令总数：" + dec_总条数.ToString(); 
                label10.Text = "按时完成制令数：" + dec_生产.ToString() + "  " + "未按时完成制令数：" + (dec_总条数 - dec_生产).ToString();


                label12.Text = "生产计划按时完成率：" + Math.Round(dec_bl, 2, MidpointRounding.AwayFromZero).ToString() + "%";




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

   
    }
}
