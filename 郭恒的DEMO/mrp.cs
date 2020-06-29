using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

//using NPOI;
//using NPOI.HSSF.UserModel;
//using NPOI.SS;
//using NPOI.HSSF;



 

using System.IO;

//using NPOI.XSSF.UserModel;
namespace 郭恒的DEMO
{
    public partial class mrp : Form
    {
        string strcon = CPublic.Var.strConn;

        DataTable dtM = new DataTable();
        DataTable dt_SaleOrder = new DataTable();
        DataTable IncompleteWorkOrder = new DataTable();
        DataTable dt_parent;
        DataTable dt_库存 ;
        DataTable dt_bom = new DataTable();
        /// <summary>
        /// sale need count
        /// </summary>
        DataTable dt_SNC = new DataTable();
        /// <summary>
        /// product need count
        /// </summary>
        DataTable dt_PNC = new DataTable();
        /// <summary>
        /// flag 指示用户进度 ,导入销售明细-1,导入未完成工单-2,同步BOM及库存-3 
        /// </summary>
        int flag = 0;
       // string strcon = CPublic.Var.strConn;
        string strcon_U8 = CPublic.Var.geConn("DW");


        public mrp()
        {
            InitializeComponent();
        }

        private void mrp_Load(object sender, EventArgs e)
        {
            #region
            //            dtM = new DataTable();
            //            dtM.Columns.Add("物料编码");
            //            dtM.Columns.Add("欠缺数量");
            //            dtM.Columns.Add("参考量");
            //            dt_SNC = new DataTable();
            //            dt_库存 = new DataTable();
            //            dt_bom = new DataTable();

            //            string sql = @" select  产品编码,子项编码,数量,b.物料类型 as 父项物料类型,c.物料类型 as 子项物料类型 from 基础数据物料BOM表 a 
            //                    left join 基础数据物料信息表 b on a.产品编码=b.物料编码  left join 基础数据物料信息表 c on a.子项编码=c.物料编码 ";
            //            dt_bom = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            //            sql = " select  物料编码,库存总数 from  仓库物料数量表 ";
            //            dt_库存 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            //            //未销售出库的销售明细 按 物料编码汇总 
            //            sql = @" select x.物料编码,(未完成数量-在制量-在途量+未领量)nc from (
            //            select a.物料编码,sum(未完成数量)未完成数量  from 销售记录销售订单明细表 a
            //            where 明细完成=0 and 生效=1 and 作废=0 and 关闭=0 group by 物料编码)x
            //            left  join 仓库物料数量表 b on x.物料编码=b.物料编码
            //            where 未完成数量-在制量-在途量+未领量>0";
            //            dt_SNC = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            //            sql = @"select 物料编码,sum(制令数量)nc from 生产记录生产制令表 where 关闭=0  and  未排单数量 >0  and 已排单数量<制令数量 and 日期>'2017-1-1'
            //                  group by 物料编码 order  by 物料编码";
            //            dt_PNC = CZMaster.MasterSQL.Get_DataTable(sql,strcon);

            #endregion



        }

        #region
        ///// <summary>
        ///// 销售订单和生产制令哪个需求量大取哪个重合部分  没有的合并 
        ///// </summary>
        //private DataTable fun_订单or制令()
        //{

        //    DataTable dt=dt_PNC.Copy();
        //    foreach (DataRow dr in dt_SNC.Rows)
        //    {
        //      DataRow []r= dt.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
        //      if (r.Length > 0)  //两者都有 比大小 取大
        //      {
        //          if (Convert.ToDecimal(dr["nc"]) > Convert.ToDecimal(r[0]["nc"])) //销售需求小于制令需求
        //          {
        //              r[0]["nc"] = dr["nc"];
        //          }

        //      }
        //      else  //dt_pnc中找不到 添进去
        //      {
        //          dt.ImportRow(dr);
        //      }
        //    }
        //    //最后需求dt
        //    return dt;
        //}
        #endregion
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        // 导入销售订单 step1
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (flag == 0)
            {
                var ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                  string path=  System.IO.Path.GetFileNameWithoutExtension(ofd.FileName);
                //  dtM = ReadStreamToDataTable(path);
                   // dt_SaleOrder = ERPorg.Corg.ExcelXLSX(ofd);
                }
                flag = 1;
            }
            else
            {
                MessageBox.Show("请按步骤操作");
            }
        }
        //未完成工单 step2
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (flag == 1)
            {
                var ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    IncompleteWorkOrder = ERPorg.Corg.ExcelXLSX(ofd);
                }
                flag = 2;
            }
            else
            {
                MessageBox.Show("请按步骤操作");
            }
        }
        //同步相关数据 step3
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            if (flag == 2)
            {

            }
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            flag = 0;
            simpleButton1.Enabled = true;
            simpleButton2.Enabled = true;
            simpleButton3.Enabled = true;

        }
        //合并 导入的 销售和未完成的 成品 半成品 
        private void combine()
        {
            dt_parent = new DataTable();
            dt_parent.Columns.Add("物料编码");
          
            foreach (DataRow dr in dt_SaleOrder.Rows)
            {
                   DataRow r = dt_parent.NewRow();
                   r["物料编码"] = dr["物料编码"];
                   dt_parent.Rows.Add(r);
            }
            foreach (DataRow dr in IncompleteWorkOrder.Rows)
            {
                DataRow[] rr = dt_parent.Select(string.Format("物料编码='{0}'", dr["物料编码"].ToString()));
                if (rr.Length == 0)
                {
                    DataRow r = dt_parent.NewRow();
                    r["物料编码"] = dr["物料编码"];
                    dt_parent.Rows.Add(r);
                }
            }
            foreach (DataRow dr in dt_parent.Rows)
            {
               DataTable temp= ERPorg.Corg.get_u8bom(dr["物料编码"].ToString());
               if (dt_bom == null || dt_bom.Columns.Count == 0)
               {
                   dt_bom = temp.Copy();

               }
               else
               {
                  //这边 取过来的BOM清单 可能 有重复 因为u8中维护的bom可能会有重复的 所以这边不用merge
                   foreach (DataRow rr in temp.Rows)
                   {
                       DataRow [] xx=dt_bom.Select(string.Format("父项编码='{0}' and 子项编码='{1}'", rr["父项编码"], rr["子项编码"]));
                       if (xx.Length == 0)
                       {
                           DataRow x = dt_bom.NewRow();
                           x["父项编码"] = rr["父项编码"];
                           x["父项名称"] = rr["父项名称"];
                           x["父项规格"] = rr["父项规格"];
                           x["子项编码"] = rr["子项编码"];
                           x["子项名称"] = rr["子项名称"];
                           x["子项规格"] = rr["子项规格"];
                           x["数量"] = rr["数量"];
                           dt_bom.Rows.Add(x);
                       }

                   }
               }
            }
  



        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {

        }
    }
}
