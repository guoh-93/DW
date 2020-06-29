using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using DevExpress.XtraTreeList.Nodes;
using System.IO;
using System.Reflection;
using CZMaster;
using DevExpress.XtraTreeList;
using DevExpress.XtraEditors.Repository;

namespace ERPSale
{
    public partial class bom查询 : UserControl
    {
        public bom查询()
        {
            InitializeComponent();
        }

        #region 变量
        string conn = CPublic.Var.strConn;
        DataTable dtM;
        DataRow drM;
        DataTable dt_物料,dt_编码;

        #endregion


        private void bom查询_Load(object sender, EventArgs e)
        {
            fun_加载();
            fun_xiala();              
        }



        #region  辅助方法
        private void fun_xiala()
        {
            

        
        }



        private void fun_加载() {

            string sql = "select 产品编码 as 物料编码 from 基础数据物料BOM表 where 1<>1";           
                dt_编码 = new DataTable();
                dt_编码 = fun_select(sql,dt_编码);                   
        }

        private void fun_bom子图()
        {
            try
            {

                treeList1.ClearNodes();
                DataRow[] r = dt_物料.Select(string.Format("物料编码='{0}'", searchLookUpEdit1.EditValue.ToString()));
                TreeListNode head = treeList1.AppendNode(new object[] { r[0]["物料编码"] }, null);
                head.SetValue("物料编号", r[0]["物料编码"].ToString());
                // head.SetValue("物料编号", r[0]["物料编码"].ToString());
                head.SetValue("物料名称", r[0]["物料名称"].ToString());
                //head.SetValue("图纸编号", r[0]["图纸编号"].ToString());
                head.SetValue("规格型号", r[0]["规格型号"].ToString());
                head.SetValue("仓库号", r[0]["仓库号"].ToString());
                head.SetValue("仓库名称", r[0]["仓库名称"].ToString());
                //head.SetValue("子项类型", r[0]["子项类型"].ToString());
                head.Tag = r[0];
                fun_TL(head, r[0]["物料编码"].ToString());
                head.ExpandAll();
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




        }


        private void     fun_TL(TreeListNode n, string str_fx)
        {
            try
            {
                //a.仓库号 as 仓库号,a.物料名称 as 物料名称,a.仓库名称 as 仓库名称
                //b.仓库号 as 子仓库号,b.物料名称 as 子物料名称,b.仓库名称 as 子仓库名称 
                dtM = new DataTable();
                string s = string.Format(@" select  a.物料编码 as 父项编号,a.物料类型 as 父项类型,a.仓库号 as 仓库号,a.物料名称 as 物料名称,a.仓库名称 as 仓库名称,a.大类 as 父项大类,a.小类 as 父项小类,a.规格型号 as 父项规格 , b.物料编码 as 子项编码,b.物料名称 as 子项名称,
            b.原ERP物料编号 as 子项编号,b.物料类型 as 子项类型,b.图纸编号 as 子项图号 ,b.规格型号 as 子项规格,b.物料名称 as 子物料名称,b.仓库名称 as 子仓库名称,b.大类 as 子项大类,b.仓库号 as 子仓库号,b.小类 as 子项小类,c.原因数 from 基础数据物料BOM表  base 
            left join 基础数据物料信息表 a  on base.产品编码=a.物料编码
            left join 基础数据物料信息表 b  on base.子项编码=b.物料编码
            left join (select sum(数量) as 原因数,产品编码 from [知识平台录入表] group by 产品编码)c on base.子项编码=c.产品编码  where 子项类型<>'采购件' and  a.物料编码='{0}'", str_fx);
           
                using (SqlDataAdapter    da = new SqlDataAdapter(s, conn))
                {
                    da.Fill(dtM);
                }

                foreach (DataRow r in dtM.Rows)
                {
                    TreeListNode nc = treeList1.AppendNode(new object[] { r["子项编码"].ToString() }, n);
                    nc.SetValue("产品编码结构", r["子项编码"].ToString());
                    nc.SetValue("子项类型", r["子项类型"].ToString());
                    nc.SetValue("物料编号", r["子项编号"].ToString());
                    nc.SetValue("物料名称", r["子项名称"].ToString());
                    nc.SetValue("规格型号", r["子项规格"].ToString());
                    nc.SetValue("仓库号", r["子仓库号"].ToString());
                    nc.SetValue("仓库名称", r["子仓库名称"].ToString());


                   // nc.SetValue("规格型号", r["子项规格"].ToString());
                    // nc.SetValue("原因数", r["原因数"].ToString());
                    //nc.SetValue("图纸编号", r["子项图号"].ToString());
                    //nc.SetValue("数量", r["数量"].ToString());
                    nc.Tag = r;
                    fun_TL(nc, r["子项编码"].ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

       

        #endregion






        #region 方法
        //加载
        private DataTable fun_select(string sql, DataTable dt)
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
            {
                dt = new DataTable();
                da.Fill(dt);
            }
            return dt;
        }
        //删除
        private void fun_delete(string sql)
        {
            using (SqlConnection sqlconn = new SqlConnection(conn))
            {
                sqlconn.Open();
                SqlCommand sqlcommand = new SqlCommand(sql, sqlconn);
                sqlcommand.ExecuteNonQuery();
                sqlconn.Close();
            }
        }
        //生效
        private void fun_save(string sql)
        {
            using (SqlConnection sqlconn = new SqlConnection(conn))
            {
                sqlconn.Open();
                SqlCommand sqlcommand = new SqlCommand(sql, sqlconn);
                sqlcommand.ExecuteNonQuery();
                sqlconn.Close();
            }
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {

           // string 啊啊啊 = searchLookUpEdit1.EditValue.ToString();

            fun_bom子图();



        }//查询

    }
}
