using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ERPSale
{
    public partial class ui快递公司维护 : UserControl
    {
        #region
        /// <summary>
        /// 指示 新增 修改状态  新增为1  修改为0 
        /// </summary>
        bool flag = false;
        string strcon = CPublic.Var.strConn;
        DataTable dtM;



        #endregion



        public ui快递公司维护()
        {
            InitializeComponent();
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void ui快递公司维护_Load(object sender, EventArgs e)
        {
            fun_load();
        }


        private void fun_load()
        {
            string sql = "select  * from  快递公司基础信息维护表 where 停用=0";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                dtM = new DataTable();
                da.Fill(dtM);


                gridControl1.DataSource = dtM;

            }

        }
        private void fun_check()
        {
            if (textBox1.Text == "")
            {
                throw new Exception("没有内容可以保存");

            }
            if (textBox2.Text == "")
            {
                throw new Exception("快递公司名称为必填项");

            }

        }

        private void fun_update()
        {
            string sql = "select  * from  快递公司基础信息维护表 where 1=2";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                new SqlCommandBuilder(da);

                da.Update(dtM);

            }
        }
        private void fun_save()
        {
            if (flag)  //新增
            {
                DataRow dr = dtM.NewRow();
                dataBindHelper1.DataToDR(dr);
                dr["修改时间"] = CPublic.Var.getDatetime();
                dtM.Rows.Add(dr);

            }
            else  //修改 
            {
                DataRow[] dr = dtM.Select(string.Format("编号='{0}'", textBox1.Text));
                dataBindHelper1.DataToDR(dr[0]);

            }

        }
        //刷新
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            flag = false; //状态重归 修改
            DataRow dr = dtM.NewRow();
            dataBindHelper1.DataFormDR(dr);
            fun_load();

        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        //新增
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (flag) throw new Exception("正在新增中。。");
                flag = true;
                string sql = "select  max(编号)编号  from 快递公司基础信息维护表  ";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    int i = 1;
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows[0][0] != null && dt.Rows[0][0].ToString()!="")
                    {
                        i = Convert.ToInt32(dt.Rows[0][0]) + 1;
                    }

                   
                    if (textBox1.Text != "")
                    {
                        DataRow dr = dtM.NewRow();
                        dataBindHelper1.DataFormDR(dr);

                    }
                    textBox1.Text = i.ToString("0000");

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("未知错误,刷新重试");
            }



        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("确定停用该快递公司信息？请核对。", "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                    dr["停用"] = 1;
                    fun_update();
                    MessageBox.Show("保存成功");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);

                }
                barLargeButtonItem2_ItemClick(null, null);
            }
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            dataBindHelper1.DataFormDR(dr);

        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_check();
                fun_save();
                fun_update();
                fun_load();
                flag = false;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
