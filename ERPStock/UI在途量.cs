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
    public partial class UI在途量 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string s;
        string cfgfilepath = "";
        public UI在途量()
        {
            InitializeComponent();
        }
        public UI在途量(string s)
        {
            this.s = s;
            InitializeComponent();
        }

        private void UI在途量_Load(object sender, EventArgs e)
        {
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(panel1, this.Name, cfgfilepath);
            // 在途量
         
            DataTable dt1 = new DataTable();
            string sql1 = string.Format
               (@"  select x.*,物料名称,图纸编号  from (               
  select 采购单号,采购明细号,供应商,物料编码,采购数量,到货日期,完成数量,已送检数,计量单位,未完成数量,拒收数量  from 采购记录采购单明细表 where   生效 = 1
       and 明细完成日期 is null and 作废 = 0 and 总完成 = 0  and 明细完成=0  and 生效日期 > '2017-1-1' and 采购数量-已送检数>0  
  union  
     select a.采购单号,采购单明细号,a.供应商,a.物料编码,a.采购数量,到货日期,完成数量,已送检数,计量单位,未完成数量,a.拒收数量   from 采购记录采购送检单明细表 ax
     left join 采购记录采购单明细表 a on a.采购明细号=ax.采购单明细号
      where ax.生效日期>'2017-1-1' and 检验完成=0 and ax.作废=0  and ax.送检单类型<>'拒收' 
  union 
     select  b.采购单号,b.采购明细号, 供应商, 物料编码, b.采购数量,到货日期,完成数量,已送检数,计量单位,未完成数量,拒收数量   from 采购记录采购单检验主表 bx 
     left join 采购记录采购单明细表 b on b.采购明细号=bx.采购明细号
     where 入库完成=0 and  完成=0 and 关闭=0  and 检验日期>'2017-1-1' and 检验结果<>'不合格'   )x
     left  join  基础数据物料信息表 xx on xx.物料编码=x.物料编码
     where xx.物料编码='{0}'", s);         
//            try
//            {
                using (SqlDataAdapter da = new SqlDataAdapter(sql1, strconn))
                {
                    da.Fill(dt1);
                    //dt1.Columns.Add("送检数量");
                    //加送检数量   
//                    foreach (DataRow dr in dt1.Rows)
//                    {
//                        string sql_111 = string.Format(@"select 采购记录采购单明细表.*,采购记录采购送检单明细表.送检数量 
//                                                         from 采购记录采购单明细表 left join 采购记录采购送检单明细表 
//                                                         on  采购记录采购单明细表.物料编码=采购记录采购送检单明细表.物料编码  
//                                                         where 采购记录采购单明细表.物料编码='{0}' and 采购明细号='{1}'", s,dr["采购明细号"]);
//                        using (SqlDataAdapter da1 = new SqlDataAdapter(sql_111, strconn))
//                        {

//                            DataTable dt_111 =new DataTable ();
//                            //所有 明细号，物料相同的 记录 把 送检数量相加赋值到 dt1中对应的明细号记录中 
//                            da1.Fill(dt_111);
//                            decimal dec =0;
//                            foreach (DataRow drr in dt_111.Rows)
//                            {
//                                if (drr["送检数量"] == DBNull.Value || drr["送检数量"].ToString() == "")
//                                {
//                                    continue;
//                                }
//                                dec = dec + Convert.ToDecimal(drr["送检数量"]);
//                            }
//                            dr["送检数量"] = dec;
//                        }
//                    }



                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(ex.Message);
//            }
            gridControl1.DataSource = dt1;
        }


        //跳转
        private void gotoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string cs = dr["采购单号"].ToString().Trim();

                //string name = string.Format("采购单明细({0}_{1})", dr["物料编码"].ToString().Trim(), dr["物料名称"].ToString().Trim());
                string name = string.Format("采购单明细({0})", cs);
                ERPpurchase.frm采购单明细视图 frm = new ERPpurchase.frm采购单明细视图(cs);
                CPublic.UIcontrol.AddNewPage(frm, name);
                frm.Dock = DockStyle.Fill;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
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
