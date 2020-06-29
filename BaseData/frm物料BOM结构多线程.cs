using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using DevExpress.XtraTreeList.Nodes;
namespace BaseData
{
    public partial class frm物料BOM结构多线程 : UserControl
    {


        #region   变量

        DataTable dtM;
        DataTable dtP;
        SqlDataAdapter dam;
        SqlCommandBuilder drm;

        #endregion


        #region  类加载

        public frm物料BOM结构多线程()
        {
            InitializeComponent();
        }


        private void frm物料BOM结构多线程_Load(object sender, EventArgs e)
        {

            System.Threading.Thread th = new System.Threading.Thread(Init);
            th.Start();
        }

        #endregion



        void Init()
        {
            string sqlstr = "select * from 基础数据物料BOM表 ";
            string sqlstr1 = "select 产品编码 from 基础数据物料BOM表 group by 产品编码";
            dtM = new DataTable();
            dtP = new DataTable();
            dam = new SqlDataAdapter(sqlstr,CPublic.Var.strConn);
            drm = new SqlCommandBuilder(dam);
            dam.Fill(dtM);
            using (SqlDataAdapter da = new SqlDataAdapter(sqlstr1, CPublic.Var.strConn))
            {
                da.Fill(dtP);
            }
            foreach (DataRow r in dtP.Rows)
            {
                DataRow[] t = dtM.Select(string.Format("子项编码='{0}'", r["产品编码"].ToString()));
                if (t.Length <= 0)
                {
                    DataRow[] t1 = dtM.Select(string.Format("产品编码='{0}'", r["产品编码"].ToString()));

                    BeginInvoke(new MethodInvoker(delegate()
                    {
                        TreeListNode n = tv.AppendNode(new object[] { t1[0]["产品编码"].ToString() }, null);
                        n.SetValue("产品编码结构", t1[0]["产品编码"]);
                        n.SetValue("子项类型", t1[0]["子项类型"]);
                        n.SetValue("类型", t1[0]["BOM类型"]);
                        n.SetValue("数量", 0);
                        n.Tag = t1[0];
                        Init(n);

                    }));
                }
            }

        }


        private void Init(TreeListNode n)
        {
            DataRow[] t = dtM.Select(string.Format("产品编码='{0}'", (n.Tag as DataRow)["产品编码"].ToString()));
            foreach (DataRow r in t)
            {
                DataRow[] t1 = dtM.Select(string.Format("产品编码='{0}'", r["子项编码"].ToString()));
                if (t1.Length > 0)
                {
                    TreeListNode nc = tv.AppendNode(new object[] { t1[0]["产品编码"].ToString() }, n);
                    nc.SetValue("产品编码结构", t1[0]["产品编码"]);
                    nc.SetValue("子项类型", t1[0]["子项类型"]);
                    nc.SetValue("类型", t1[0]["BOM类型"]);
                    nc.SetValue("数量", t[0]["数量"]);
                    nc.Tag = t1[0];
                    Init(nc);
                    //  nc.ExpandAll();
                }
            }
        }


    }
}
