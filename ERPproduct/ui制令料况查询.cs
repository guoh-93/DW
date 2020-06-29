using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ERPproduct
{
    public partial class ui制令料况查询 : UserControl
    {
        DataRow drM;
        string strcon = CPublic.Var.strConn;
        /// <summary>
        /// 只加载所需要的
        /// </summary>
        DataTable bom;
        DataTable dt_总需;
        DataTable dt_库存;
        /// <summary>
        /// 其他占用量是否需要扣减此单需求量
        /// 是否由制令转过来
        /// </summary>
        bool bl_other = false;
        string cfgfilepath = "";
        public ui制令料况查询(DataRow dr)
        {
            InitializeComponent();
            drM = dr;
            if (drM != null && drM.Table.Columns.Contains("生产制令单号") && drM["生产制令单号"].ToString()!="")
            {

                textBox1.Text = drM["生产制令单号"].ToString();
                textBox3.Text = drM["制令数量"].ToString();
                textBox4.Text = drM["已排单数量"].ToString();
                bl_other = true;
            }
            else
            {
                textBox1.Visible = false;
                label1.Visible = false;
                label3.Text = "数量";
                textBox3.Text = drM["数量"].ToString();
                textBox4.Text = "0";
            }
        }
        public ui制令料况查询()
        {
            InitializeComponent();
        }
        private void ui制令料况查询_Load(object sender, EventArgs e)
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg pz = new ERPorg.Corg();
                pz.UserLayout(this.panel2, this.Name, cfgfilepath);
                fun_load();
                fun_calu();
                int x = 1;
                if (!bl_other) x = 2;
                fun_1(x);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
        private void fun_load()
        {
            string s = string.Format("select  sum(制令数量)其他制令数  from 生产记录生产制令表" +
                " where  关闭=0 and 完成=0 and  生产制令类型 <>'返修制令'and 物料编码='{0}' and 生产制令单号<>'{1}' group by 物料编码 ", drM["物料编码"], textBox1.Text.Trim());
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            textBox2.Text = "0";
            if (dt.Rows.Count > 0)
            {
                textBox2.Text = dt.Rows[0]["其他制令数"].ToString();
            }
            s = " select  产品编码,子项编码,数量,WIPType,组,优先级 from 基础数据物料BOM表 where 主辅料='主料' ";
            bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = @"select  base.物料名称,base.规格型号,kc.* from 基础数据物料信息表 base
                    left join (select 物料编码, sum(库存总数)库存总数,MAX(受订量) 受订量,MAX(在制量)在制量,max(未领量)未领量,max(在途量) as 在途量  from 仓库物料数量表
                 where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 = '仓库类别' and 布尔字段2 = 1) group by 物料编码)kc on kc.物料编码=base.物料编码 ";

            dt_库存 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
        }

        //2020-5-6
        //bl 标识是不是加载 0 表示加载时 1 表示 计算时
        private void xn_zj(string str_wl, DataTable t, int i, bool bl)
        {
            DataRow[] r = bom.Select(string.Format("产品编码='{0}'", str_wl));

            foreach (DataRow xn in r)
            {
                if (xn["WIPType"].ToString() == "虚拟") xn_zj(xn["子项编码"].ToString(), t, i, bl);
                else
                {
                    DataRow rr = t.NewRow();
                    if (bl) //20-5-6
                    {
                       
                        rr["此单需求数量"] = Convert.ToDecimal(t.Rows[i]["此单需求数量"]) * Convert.ToDecimal(xn["数量"]);
                        rr["bom数量"] = Convert.ToDecimal(xn["数量"]) * Convert.ToDecimal(t.Rows[i]["bom数量"]);
                        //2020-4-15
                        rr["组"] = xn["组"];
                        rr["优先级"] = xn["优先级"];
                    }
                    else
                    {
                        rr["总需求数量"] = Convert.ToDecimal(t.Rows[i]["总需求数量"]) * Convert.ToDecimal(xn["数量"]);

                    }
                    //rr["子项编码"] = t.Rows[i]["子项编码"]; //产品编码
                    rr["子项编码"] = xn["子项编码"];
                    rr["WIPType"] = "虚拟件子件";

                    t.Rows.Add(rr);
                }
            }
        }

        private void fun_calu()
        {
            string s = @"select zl.物料编码,子项编码,SUM(制令数量 * bom.数量)总需求数量,WIPType from 生产记录生产制令表 zl
             left join 基础数据物料BOM表 bom  on zl.物料编码 = bom.产品编码
             where 关闭 = 0 and 完成 =0  and 生产制令类型 <>'返修制令' and 子项编码 is not null and WIPType<>'入库倒冲' 
               group by zl.物料编码,子项编码,WIPType";
            DataTable t1 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            for (int i = t1.Rows.Count - 1; i >= 0; i--)
            {
                if (t1.Rows[i]["WIPType"].ToString() == "虚拟")
                {
                    //DataRow[] r = bom.Select(string.Format("产品编码='{0}'", t1.Rows[i]["子项编码"]));
                    //foreach (DataRow xn in r)
                    //{
                    //    DataRow rr = t1.NewRow();
                    //    rr["物料编码"] = t1.Rows[i]["物料编码"]; //产品编码
                    //    rr["子项编码"] = xn["子项编码"];
                    //    rr["总需求数量"] = Convert.ToDecimal(t1.Rows[i]["总需求数量"]) * Convert.ToDecimal(xn["数量"]);
                    //    rr["WIPType"] = "虚拟件子件";
                    //    t1.Rows.Add(rr);
                    //}
                    xn_zj(t1.Rows[i]["子项编码"].ToString(), t1, i, false );  

                    t1.Rows[i].Delete();
                }
            }
            //DataRow []pp= t1.Select(string.Format("子项编码='01010400000015'" ));
            MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
            dt_总需 = RBQ.SelectGroupByInto("", t1, "子项编码,sum(总需求数量) 总需求数量", "", "子项编码");

            s = @"select  x.物料编码,总已领数量-isnull(返库数量,0) as 总已领数量 from (
           select mx.物料编码,SUM(领料数量 ) as 总已领数量 from 生产记录生产领料单明细表 mx
                 left join 生产记录生产领料单主表 zb on zb.领料出库单号=mx.领料出库单号         
            where mx.生产制令单号 in (select  生产制令单号 from 生产记录生产制令表 where 关闭 = 0 and 完成 = 0  and 生产制令类型<>'返修制令')
          and 领料类型<>'生产补料'   group by mx.物料编码)x
          left join (select   a.物料编码,sum(返库数量)返库数量  from 工单返库单明细表  a
          left join 生产记录生产工单表  b on a.工单号=b.生产工单号  
          where  生产制令单号 in (select  生产制令单号 from 生产记录生产制令表 where 关闭 = 0 and 完成 = 0  and 生产制令类型<>'返修制令')
          group by  a.物料编码)y on x.物料编码=y.物料编码";
            DataTable t_已领 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            dt_总需.Columns.Add("总已领数量", typeof(decimal));
            //dt_总需.Columns.Add("还需数量", typeof(decimal));
            foreach (DataRow dr in dt_总需.Rows)
            {
                DataRow[] r = t_已领.Select(string.Format("物料编码='{0}'", dr["子项编码"]));
                if (r.Length > 0)
                {
                    dr["总已领数量"] = r[0]["总已领数量"];
                }
                else
                {
                    dr["总已领数量"] = 0;
                }
            }

            ////此制令所需bom物料及数量
            //string s1 = string.Format(@"select 子项编码,SUM(制令数量*bom.数量)此单需求数量,WIPType from 生产记录生产制令表 zl
            //left join 基础数据物料BOM表 bom  on zl.物料编码=bom.产品编码 
            //where 关闭=0 and 完成=0 and 子项编码 is not null and 生产制令单号 ='{0}' group by 子项编码", textBox1.Text);
            //DataTable t_单个制令 = CZMaster.MasterSQL.Get_DataTable(s1, strcon);
            //for (int i = t_单个制令.Rows.Count - 1; i >= 0; i--)
            //{
            //    if (t_单个制令.Rows[i]["WIPType"].ToString() == "虚拟")
            //    {
            //        DataRow[] r = bom.Select(string.Format("产品编码='{0}'", t_单个制令.Rows[i]["子项编码"]));
            //        foreach (DataRow xn in r)
            //        {
            //            DataRow rr = t_单个制令.NewRow();
            //            rr["物料编码"] = t_单个制令.Rows[i]["物料编码"]; //产品编码
            //            rr["子项编码"] = xn["子项编码"];
            //            rr["此单需求数量"] = Convert.ToDecimal(t_单个制令.Rows[i]["此单需求数量"]) * Convert.ToDecimal(xn["数量"]);
            //            rr["WIPType"] = xn["WIPType"];
            //            t_单个制令.Rows.Add(rr);
            //        }
            //        t_单个制令.Rows[i].Delete();
            //    }
            //}
            //s1 = @"select mx.物料编码,SUM(领料数量) as 已领数量 from 生产记录生产领料单明细表 mx
            //where mx.生产制令单号='{0}'  group by mx.物料编码";
            //DataTable t_单个制令已领 = CZMaster.MasterSQL.Get_DataTable(s1, strcon);
            //from rHead in dt_所需.()
            //join rTail in t_已领.AsEnumerable()
            //on rHead.Field<Int32>("GoodID") equals rTail.Field<Int32>("GoodID")
            //select rHead.ItemArray.Concat(rTail.ItemArray.Skip(1));

            //foreach (var obj in query)
            //{
            //    DataRow dr = DtAll.NewRow();
            //    dr.ItemArray = obj.ToArray();
            //    DtAll.Rows.Add(dr);
            //}



        }

        private void fun_1(int  x )
        {
            //此制令所需bom物料及数量
            DataTable t_单个制令 = new DataTable();
            if (x==1)
            {
                //and 完成=0 
                string s1 = string.Format(@"select  a.*,ISNULL(已检未入数,0)已检未入数,ISNULL(已送未检数,0)已送未检数 from (
  select 子项编码,SUM({1}*bom.数量)此单需求数量,WIPType,bom.数量 as bom数量,bom.组,bom.优先级 from 生产记录生产制令表 zl
   left join 基础数据物料BOM表 bom  on zl.物料编码=bom.产品编码 
    where 关闭=0 and 子项编码 is not null and 生产制令单号 ='{0}' 
    and WIPType<>'入库倒冲'  group by 子项编码,WIPType,bom.数量,bom.组,bom.优先级)a
    left join (  select  物料编码,sum(合格数量-已入库数量)已检未入数 from 生产记录生产检验单主表  
  where 检验日期>'2019-5-5' and 完成=0 group by 物料编码
  union 
  select  产品编号 as 物料编码,SUM(送检数量-已入库数-不合格数量)已检未入数 from 采购记录采购单检验主表
  where 入库完成 =0 and 关闭=0 and 检验结果 in ('合格','免检')  and 完成=0  group by 产品编号  )b on a.子项编码=b.物料编码 
  left join (select  物料编码,sum(送检数量-已检验数)已送未检数  from 采购记录采购送检单明细表 where 检验完成=0 and 作废=0 and 送检数量>0 group by 物料编码
              union select  物料编码,sum(未检验数量)已送未检数 from 生产记录生产工单表 where  关闭=0 and 完工=1 and 检验完成=0 group by 物料编码) x
  on x.物料编码=a.子项编码", textBox1.Text,Convert.ToDecimal(textBox3.Text));
                t_单个制令 = CZMaster.MasterSQL.Get_DataTable(s1, strcon);
            }
            else
            {
                string s1 = string.Format(@"select  a.*,ISNULL(已检未入数,0)已检未入数,ISNULL(已送未检数,0)已送未检数 from (
  select 子项编码,SUM({0}*bom.数量)此单需求数量,WIPType,bom.数量 as bom数量,bom.组,bom.优先级 from 基础数据物料信息表 zl
            left join 基础数据物料BOM表 bom  on zl.物料编码=bom.产品编码 
            where 关闭=0  and 子项编码 is not null and zl.物料编码='{1}' and WIPType<>'入库倒冲'  group by 子项编码,WIPType,bom.数量,bom.组,bom.优先级)a
             left join (  select  物料编码,sum(合格数量-已入库数量)已检未入数 from 生产记录生产检验单主表  
  where 检验日期>'2019-5-5' and 完成=0 group by 物料编码
  union 
  select  产品编号 as 物料编码,SUM(送检数量-已入库数-不合格数量)已检未入数 from 采购记录采购单检验主表
  where 入库完成 =0 and 关闭=0 and 检验结果 in ('合格' ,'免检')  and 完成=0  group by 产品编号  )b on a.子项编码=b.物料编码 
  left join (select  物料编码,sum(送检数量-已检验数)已送未检数  from 采购记录采购送检单明细表 where 检验完成=0 and 作废=0 and 送检数量>0 group by 物料编码
                union select  物料编码,sum(未检验数量)已送未检数 from 生产记录生产工单表 where   关闭=0 and 完工=1 and 检验完成=0 group by 物料编码 ) x
  on x.物料编码=a.子项编码", textBox3.Text, drM["物料编码"].ToString());
                t_单个制令 = CZMaster.MasterSQL.Get_DataTable(s1, strcon);

            }
            for (int i = t_单个制令.Rows.Count - 1; i >= 0; i--)
            {
                if (t_单个制令.Rows[i]["WIPType"].ToString() == "虚拟")
                {
                    string str_子项编码 = t_单个制令.Rows[i]["子项编码"].ToString();

                    xn_zj(str_子项编码, t_单个制令, i, true);
                    //DataRow[] r = bom.Select(string.Format("产品编码='{0}'", t_单个制令.Rows[i]["子项编码"]));
                    //foreach (DataRow xn in r)
                    //{
                    //    DataRow rr = t_单个制令.NewRow();
                    //    rr["子项编码"] = t_单个制令.Rows[i]["子项编码"]; //产品编码
                    //    rr["子项编码"] = xn["子项编码"];
                    //    rr["此单需求数量"] = Convert.ToDecimal(t_单个制令.Rows[i]["此单需求数量"]) * Convert.ToDecimal(xn["数量"]);
                    //    rr["WIPType"] = "虚拟件子件";
                    //    rr["bom数量"] = Convert.ToDecimal(xn["数量"]) * Convert.ToDecimal(t_单个制令.Rows[i]["bom数量"]);
                    //    //2020-4-15
                    //    rr["组"] = xn["组"];
                    //    rr["优先级"] = xn["优先级"];
                    //    t_单个制令.Rows.Add(rr);
                    //}
                    t_单个制令.Rows[i].Delete();
                }
            }
            string s2 = string.Format(@"select mx.物料编码,SUM(领料数量) as 已领数量 from 生产记录生产领料单明细表 mx
    left  join 生产记录生产工单待领料明细表  dlmx on dlmx.待领料单明细号 =mx.待领料单明细号
     left join 生产记录生产工单待领料主表  dlz on dlz.待领料单号 =dlmx.待领料单号
            where mx.生产制令单号='{0}'  and 领料类型<>'生产补料'  group by mx.物料编码", textBox1.Text);
            DataTable t_单个制令已领 = CZMaster.MasterSQL.Get_DataTable(s2, strcon);
            t_单个制令.Columns.Add("此单已领数量", typeof(decimal));
            t_单个制令.Columns.Add("总需数量", typeof(decimal));
            t_单个制令.Columns.Add("库存总数", typeof(decimal));
            t_单个制令.Columns.Add("物料名称");
            t_单个制令.Columns.Add("规格型号");
            t_单个制令.Columns.Add("在制量", typeof(decimal));
            t_单个制令.Columns.Add("在途量", typeof(decimal));

            t_单个制令.Columns.Add("总已领量", typeof(decimal));
            t_单个制令.Columns.Add("其他占用量", typeof(decimal));
            t_单个制令.Columns.Add("此单剩余需求", typeof(decimal));
            foreach (DataRow dr in t_单个制令.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                DataRow[] r = t_单个制令已领.Select(string.Format("物料编码='{0}'", dr["子项编码"]));
                if (r.Length > 0)
                {
                    dr["此单已领数量"] = r[0]["已领数量"]; //此单已领数量
                }
                else
                {
                    dr["此单已领数量"] = 0;
                }
                DataRow[] r_t = dt_总需.Select(string.Format("子项编码='{0}'", dr["子项编码"]));
                if (r_t.Length > 0)
                {
                    dr["总需数量"] = r_t[0]["总需求数量"];
                    dr["总已领量"] = r_t[0]["总已领数量"];
                }
                else
                {
                    dr["总需数量"] = 0;
                    dr["总已领量"] = 0;
                }

                DataRow[] r_kc = dt_库存.Select(string.Format("物料编码='{0}'", dr["子项编码"]));
                if (r_kc.Length > 0)
                {
                    dr["物料名称"] = r_kc[0]["物料名称"];
                    dr["规格型号"] = r_kc[0]["规格型号"];
                    dr["在制量"] = r_kc[0]["在制量"];
                    dr["库存总数"] = r_kc[0]["库存总数"];
                    dr["在途量"] = r_kc[0]["在途量"];


                }
                if (bl_other)
                {
                    //if (dr["子项编码"].ToString() == "01010400000015")
                    //{ string s = ""; }

                    dr["此单剩余需求"] = Convert.ToDecimal(dr["此单需求数量"]) - Convert.ToDecimal(dr["此单已领数量"]);
                    decimal dec = Convert.ToDecimal(dr["总需数量"]) - Convert.ToDecimal(dr["总已领量"]) - Convert.ToDecimal(dr["此单剩余需求"]);
                    dr["其他占用量"] = dec > 0 ? dec:0;
                    decimal dec_总需 = Convert.ToDecimal(dr["总需数量"]);
                    decimal dec_总已领 = Convert.ToDecimal(dr["总已领量"]);
                    decimal dec_此单剩余 = Convert.ToDecimal(dr["此单剩余需求"]);

                }
                else
                {
                    dr["此单剩余需求"] = Convert.ToDecimal(dr["此单需求数量"]);
                    dr["其他占用量"] = Convert.ToDecimal(dr["总需数量"])- Convert.ToDecimal(dr["总已领量"]);
                }

            }
            //虚拟件没有送检未入数 和 已检未入库数
            foreach (DataRow dr in t_单个制令.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                if(dr["已检未入数"] ==null || dr["已检未入数"].ToString()=="")
                {
                    string s = string.Format(@"select* from(select 物料编码, sum(合格数量-已入库数量)已检未入数 from 生产记录生产检验单主表
                    where 检验日期 > '2019-5-5' and 完成 = 0 group by 物料编码
                    union
                    select 产品编号 as 物料编码,SUM(送检数量-已入库数-不合格数量 )已检未入数 from 采购记录采购单检验主表
                     where 入库完成 = 0 and 关闭 = 0 and 检验结果  in ('合格' ,'免检')  and 完成 = 0   group by 产品编号)a  where 物料编码 = '{0}'", dr["子项编码"]);
                    DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    if (temp.Rows.Count == 0) dr["已检未入数"] = 0;
                    else dr["已检未入数"] = temp.Rows[0]["已检未入数"];

                    s =string.Format(@"select  * from (  
                select  物料编码,sum(送检数量-已检验数)已送未检数  from 采购记录采购送检单明细表 where 检验完成=0 and 作废=0 and 送检数量>0 group by 物料编码
                union select  物料编码,sum(未检验数量)已送未检数 from 生产记录生产工单表 where 完工=1 and 检验完成=0 group by 物料编码)a
                where 物料编码='{0}'", dr["子项编码"]);
                     temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    if (temp.Rows.Count == 0) dr["已送未检数"] = 0;
                    else dr["已送未检数"] = temp.Rows[0]["已送未检数"];

            
                }
            }
            t_单个制令.Columns.Add("此单可用", typeof(decimal));
            foreach (DataRow dr in t_单个制令.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                decimal dec = Convert.ToDecimal(dr["库存总数"]) + Convert.ToDecimal(dr["在制量"]) - Convert.ToDecimal(dr["其他占用量"])
               + Convert.ToDecimal(dr["已检未入数"]); // + Convert.ToDecimal(dr["已送未检数"])
                dr["此单可用"] = dec > 0 ? dec : 0;
            }
             gridControl1.DataSource = t_单个制令;

        }
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
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
               gridControl1.ExportToXlsx(saveFileDialog.FileName, options);  
                
                   // ERPorg.Corg.TableToExcel(dtM, saveFileDialog.FileName);
            
                
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (gridView1.GetRow(e.RowHandle) == null)
                {
                    return;
                }

                //此单剩余需求
                if (Convert.ToDecimal(gridView1.GetRowCellValue(e.RowHandle, "此单可用"))< Convert.ToDecimal(gridView1.GetRowCellValue(e.RowHandle,"此单剩余需求")))
                {
                    e.Appearance.BackColor = Color.Pink;
                    e.Appearance.BackColor2 = Color.Pink;
                }
       
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_load();
                fun_calu();
                int x = 1;
                if (!bl_other) x = 2;
                fun_1(x);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
          
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;

            }
        }

        private void 查看其他占用明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                ui其他占用明细 ui = new ui其他占用明细(dr["子项编码"].ToString(), textBox1.Text);
                CPublic.UIcontrol.Showpage(ui, "其他占用明细");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                gridView1.CloseEditor();
                this.ActiveControl = null;
                contextMenuStrip1.Tag = gridView1;
            }
        }
    }
}
