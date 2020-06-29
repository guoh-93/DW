using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 赵峰的DEMO
{
    public partial class frm修改仓库名称 : Form
    {
        string strconn = CPublic.Var.strConn;
        DataTable dtM;

        public frm修改仓库名称()
        {
            InitializeComponent();
        }

        private void fun_(string str)
        {
            string sql = string.Format("select 物料编码,n仓库编号,n仓库描述,仓库号,仓库名称 from 基础数据物料信息表 where 仓库号 = '' and 停用 = 0");
            dtM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            gc.DataSource = dtM;
            label1.Text = dtM.Rows.Count.ToString();
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //原材料
            fun_("原材料");
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //成品
            fun_("成品 or 物料类型 = '半成品'");
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //计算
            foreach (DataRow dr in dtM.Rows)
            {
                if (dr["n仓库描述"].ToString() == "成品库")
                {
                    dr["仓库号"] = "00000001";
                    dr["仓库名称"] = "成品库";
                }
                if (dr["n仓库描述"].ToString() == "原材料库")
                {
                    dr["仓库号"] = "00000002";
                    dr["仓库名称"] = "原材料库";
                }
                if (dr["n仓库描述"].ToString() == "电子元器件")
                {
                    dr["仓库号"] = "00000009";
                    dr["仓库名称"] = "电子元器件库";
                }
                if (dr["n仓库描述"].ToString() == "标准件库" || dr["n仓库描述"].ToString() == "环保标准件库")
                {
                    dr["仓库号"] = "00000003";
                    dr["仓库名称"] = "标准件库";
                }
                if (dr["n仓库描述"].ToString() == "焊接零件库")
                {
                    dr["仓库号"] = "00000006";
                    dr["仓库名称"] = "线路板库";
                }
                if (dr["n仓库描述"].ToString() == "内部附件库" || dr["n仓库描述"].ToString() == "未来二库")
                {
                    dr["仓库号"] = "00000005";
                    dr["仓库名称"] = "内部附件库";
                }
                if (dr["n仓库描述"].ToString() == "外部附件库" || dr["n仓库描述"].ToString() == "未来一库")
                {
                    dr["仓库号"] = "00000004";
                    dr["仓库名称"] = "外部附件库";
                }
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //保存
            string sql = string.Format("select 物料编码,n仓库编号,n仓库描述,仓库号,仓库名称 from 基础数据物料信息表 where 1<>1");
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dtM);
            MessageBox.Show("OK");
        }
    }
}
