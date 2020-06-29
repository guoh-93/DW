using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERPproduct
{
    public partial class frm现场情况 : Form
    {
        public frm现场情况()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm现场情况_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            string sql1 = string.Format("select * from 人事基础员工表 where 员工号 = '{0}'", CPublic.Var.LocalUserID);
            DataTable dt_用户 = CZMaster.MasterSQL.Get_DataTable(sql1, CPublic.Var.strConn);
            string sql2 = string.Format("select * from 人事基础员工表 where 课室 ='{0}' and 在职状态='在职' order by 班组", dt_用户.Rows[0]["课室"].ToString());
            DataTable dt_用户总表 = CZMaster.MasterSQL.Get_DataTable(sql2, CPublic.Var.strConn);

            string sql = string.Format("select * from 生产记录报工记录表 where 是否结工 = 0");
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
            DataTable dt_M = dt.Clone();
            dt_M.Columns.Add("课室");
            dt_M.Columns.Add("班组");
            dt_M.Columns.Add("工时");
            dt_M.Columns.Add("物料编码");
            dt_M.Columns.Add("物料名称");
            dt_M.Columns.Add("生产数量",typeof(decimal));
            foreach (DataRow dr in dt_用户总表.Rows)
            {
                DataRow[] drr = dt.Select(string.Format("工号='{0}'", dr["员工号"].ToString()));
                if (drr.Length > 0)
                {
                    string sql3 = string.Format("select * from 生产记录生产工单表 where 生产工单号='{0}'", drr[0]["工单号"].ToString());
                    DataTable dt_生产工单表 = CZMaster.MasterSQL.Get_DataTable(sql3, CPublic.Var.strConn);

                    string sql33 = string.Format("select * from 基础数据物料信息表 where 物料编码='{0}'", dt_生产工单表.Rows[0]["物料编码"].ToString());
                    DataTable dt_物料信息表 = CZMaster.MasterSQL.Get_DataTable(sql33, CPublic.Var.strConn);

                    if (dt_生产工单表.Rows.Count > 0)
                    {

                        DataRow dr1 = dt_M.NewRow();
                        dr1.ItemArray = drr[0].ItemArray;
                        dr1["课室"] = dr["课室"].ToString();
                        dr1["班组"] = dr["班组"].ToString();
                        dr1["工时"] = dt_生产工单表.Rows[0]["工时"].ToString();
                        dr1["物料编码"] = dt_物料信息表.Rows[0]["原ERP物料编号"].ToString();
                        dr1["物料名称"] = dt_生产工单表.Rows[0]["物料名称"].ToString();
                        dr1["生产数量"] = dt_生产工单表.Rows[0]["生产数量"];
                        dt_M.Rows.Add(dr1);
                    }
                }
                else
                {
                    DataRow dr1 = dt_M.NewRow();
                    dr1["工号"] = dr["员工号"].ToString();
                    dr1["姓名"] = dr["姓名"].ToString();
                    dr1["课室"] = dr["课室"].ToString();
                    dr1["班组"] = dr["班组"].ToString();
                    dt_M.Rows.Add(dr1);
                }

            }
            //string sql4 = string.Format("select 生产记录报工记录表.*,b.生产数量,b.物料名称,a.原ERP物料编号,a.物料编码,b.车间名称,c.班组 from 生产记录报工记录表,生产记录生产工单表 b ,基础数据物料信息表 a,人事基础员工表 c where 生产记录报工记录表.工单号 = b.生产工单号 and  a.物料编码 = b.物料编码 and 生产记录报工记录表.工号 = c.员工号 and 生产记录报工记录表.是否完工 = 0", CPublic.Var.LocalUserID);
            //DataTable dt_M = CZMaster.MasterSQL.Get_DataTable(sql4, CPublic.Var.strConn);
            //DataView dv1 = new DataView(dt_M);
            //dv1.RowFilter
            gridControl1.DataSource = dt_M;
            gridControl1.UseEmbeddedNavigator = true;
            //gridControl1.EmbeddedNavigator(
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            this.Close();
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            this.gridView1.MovePrevPage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton3_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            this.gridView1.MoveNextPage();
        }
    }
}
