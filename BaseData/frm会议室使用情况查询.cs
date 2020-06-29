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
    public partial class frm会议室使用情况查询 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        DataTable dtM = null;


        public frm会议室使用情况查询()
        {
            InitializeComponent();
        }

        private void frm会议室使用情况查询_Load(object sender, EventArgs e)
        {
            try
            {
                barEditItem2.EditValue = "";
                barEditItem1.EditValue = System.DateTime.Today;
                string str = "where 日期 >= '" + barEditItem1.EditValue.ToString() + "'";
                fun_载入(str);
                checkBox1.Checked = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_载入(string str)
        {
            string sql = string.Format("select * from 会议室使用情况表 {0}", str);
            dtM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            gc.DataSource = dtM;
        }

        private void fun_保存()
        {
            string sql = "select * from 会议室使用情况表 where 1<>1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dtM);
        }

        private void fun_查询()
        {
            if (barEditItem1.EditValue.ToString() == "")
            {
                throw new Exception("请先选择日期！");
            }
            string str = "where 日期 >= '" + barEditItem1.EditValue.ToString() + "'";
            if(barEditItem2.EditValue.ToString() != "")
            {
                str = str + " and 楼层 = '" + barEditItem2.EditValue.ToString() + "'";
            }
            fun_载入(str);
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = dtM.NewRow();
            dtM.Rows.Add(dr);
            dr["使用人"] = CPublic.Var.localUserName;
            dr["使用人ID"] = CPublic.Var.LocalUserID;
            dr["GUID"] = System.Guid.NewGuid();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            dr.Delete();
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_保存();
            MessageBox.Show("保存成功！");
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_查询();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                //查询
                barLargeButtonItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barLargeButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barStaticItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barEditItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barStaticItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barEditItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                gv.OptionsBehavior.Editable = false;
            }
            else
            {
                //编辑
                barLargeButtonItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barLargeButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barStaticItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barEditItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barStaticItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barEditItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                gv.OptionsBehavior.Editable = true;
                
            }
        }
    }
}
