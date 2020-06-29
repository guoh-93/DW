using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace BaseData
{
    public partial class frm基础数据物料信息视图 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        string sss;
        DataTable dtM;
        string strshow;
        int a, b;
        #endregion

        #region 自用类
        public frm基础数据物料信息视图()
        {
            InitializeComponent();
        }

        public frm基础数据物料信息视图(string str)
        {
            InitializeComponent();
            sss = str;
            fun_查询基础属性();
            fun_查询扩展属性();
        }
        #endregion

        #region 数据操作
        public void fun_查询基础属性()
        {
            try
            {
                dtM = new DataTable();
                dtM.Columns.Add("物料属性");
                dtM.Columns.Add("属性值");
                string sql = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", sss);
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (dt.Columns[i].ToString() == "物料类型GUID" || dt.Columns[i].ToString() == "产品线GUID" || dt.Columns[i].ToString() == "大类GUID" || dt.Columns[i].ToString() == "小类GUID" || dt.Columns[i].ToString() == "规格GUID")
                    {
                        continue;
                    }
                    else
                    {
                        DataRow dr = dtM.NewRow();
                        dr[0] = dt.Columns[i].ToString();
                        dtM.Rows.Add(dr);
                    }
                }
                DataRow dr_基础信息 = dt.Rows[0];
                foreach (DataRow dr in dtM.Rows)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (dr["物料属性"].ToString() == dt.Columns[j].ToString())
                        {
                            dr["属性值"] = dr_基础信息[j].ToString();
                        }
                    }
                }
            }
            catch  
            { 
                strshow = "没有该数据!";
                a = 1;
            }
        }

        public void fun_查询扩展属性()
        {
            try
            {
                string sql1 = string.Format("select * from 基础数据物料信息扩展表 where 物料编码 = '{0}'", sss);
                SqlDataAdapter da1 = new SqlDataAdapter(sql1, strconn);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                dtM.Merge(dt1);
                gc.DataSource = dtM;
            }
            catch (Exception ex)
            {
                strshow = ex.Message;
                b = 1;
            }
        }
        #endregion

        #region 界面操作
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            sss = be.EditValue.ToString();
            a = 0; b = 0;
            fun_查询基础属性();
            if (a == 1)
            {
                MessageBox.Show(strshow);
            }
            fun_查询扩展属性();
            if (b == 1)
            {
                MessageBox.Show(strshow);
            }
        }
        #endregion
    }
}
