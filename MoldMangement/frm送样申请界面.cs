using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace MoldMangement
{
    public partial class frm送样申请界面 : UserControl
    {
        CurrencyManager cmM;
        string strcon = CPublic.Var.strConn;
        DataTable dt_送样申请;
        DataTable dt_送样明细;
        DataTable dt_基础数据物料信息表;
        Image myImage;
        string bt;
        string type = "";
        string ss = "";
        byte[] bts;
        public frm送样申请界面()
        {
            InitializeComponent();
        }

        private void frm送样申请界面_Load(object sender, EventArgs e)
        {
            try
            {
                textBox2.Text = CPublic.Var.localUserName;
                textBox3.Text = CPublic.Var.LocalUserID;
                dt_送样申请 = new DataTable();
                string sql = "select * from 送样申请表 where 1<>1";
                fun_GetDataTable(dt_送样申请, sql);
                dt_送样明细 = new DataTable();
                string sql3 = "select * from 送样明细表 where 1<>1";
                fun_GetDataTable(dt_送样明细, sql3);
                gridControl1.DataSource = dt_送样明细;
                dt_送样明细.Columns.Add("申请编号");
                dt_送样明细.Columns.Add("结束日期");
                dt_送样明细.Columns.Add("申请人");
                dt_送样明细.Columns.Add("工号");
                dt_送样明细.Columns.Add("上传备注");
                dt_送样明细.Columns.Add("上传备注文件名");
                dt_送样明细.Columns.Add("上传备注后缀");
                dt_送样明细.Columns.Add("客户");
                dt_送样明细.Columns.Add("产品");
                dt_送样明细.Columns.Add("生效");
                dt_送样明细.Columns.Add("完成");
                dt_送样明细.Columns.Add("技术确认");
                dt_送样明细.Columns.Add("技术大批量确认");
                dt_送样明细.Columns.Add("销售确认");
                dt_送样明细.Columns.Add("库存总数");
                string sql2 = string.Format(@"select 客户编号,客户名称 from 客户基础信息表");
                SqlDataAdapter da = new SqlDataAdapter(sql2, strcon);
                DataTable dt_客户 = new DataTable();
                da.Fill(dt_客户);
                searchLookUpEdit1.Properties.DataSource = dt_客户;
                searchLookUpEdit1.Properties.DisplayMember = "客户名称";
                searchLookUpEdit1.Properties.ValueMember = "客户编号";
                dt_基础数据物料信息表 = new DataTable();
                string sql4 = @"select 基础数据物料信息表.物料编码,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.物料名称,基础数据物料信息表.n原ERP规格型号,
            基础数据物料信息表.图纸编号,仓库物料数量表.库存总数,货架描述,仓库名称 from 基础数据物料信息表 
            left join 仓库物料数量表 on 基础数据物料信息表.物料编码 = 仓库物料数量表.物料编码";
                fun_GetDataTable(dt_基础数据物料信息表, sql4);
                repositoryItemSearchLookUpEdit1.DataSource = dt_基础数据物料信息表;
                repositoryItemSearchLookUpEdit1.DisplayMember = "原ERP物料编号";
                repositoryItemSearchLookUpEdit1.ValueMember = "原ERP物料编号";
                cmM = BindingContext[dt_送样明细] as CurrencyManager;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DateTime time = CPublic.Var.getDatetime().Date;
            time_申请日期.EditValue = time;
            cmM.EndCurrentEdit();
            gridView1.CloseEditor();
            try
            {
                DataRow dr = dt_送样明细.NewRow();
                dr["申请人"] = CPublic.Var.localUserName;
                dr["工号"] = CPublic.Var.LocalUserID;
                dt_送样明细.Rows.Add(dr);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            cmM.EndCurrentEdit();
            gridView1.CloseEditor();
            try
            {
                (cmM.Current as DataRowView).Row.Delete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (searchLookUpEdit1.EditValue.ToString() == "")
                {
                    MessageBox.Show("请选择客户");
                    return;
                }
                foreach (DataRow dr2 in dt_送样明细.Rows)
                {
                    if (dr2["数量"].ToString() == "")
                    {
                        MessageBox.Show("请填写数量");
                        return;
                    }
                }

                txt_出入库申请单号.Text = string.Format("QWSQ{0}{1}{2}{3}", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString("00"),
                      DateTime.Now.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("QWSQ", DateTime.Now.Year, DateTime.Now.Month).ToString("0000"));
                DataRow dr = dt_送样申请.NewRow();
                dr["申请编号"] = txt_出入库申请单号.Text;
                dr["申请日期"] = time_申请日期.Text;
                dr["技术要求"] = textBox4.Text;
                dr["申请人"] = CPublic.Var.localUserName;
                dr["工号"] = CPublic.Var.LocalUserID;
                dr["上传备注"] = bts;
                dr["上传备注文件名"] = ss;
                dr["上传备注后缀"] = type;
                dr["备注"] = textBox1.Text;
                dr["产品"] = "";
                dr["生效"] = "True";
                dr["完成"] = "False";
                dr["技术确认"] = "False";
                dr["技术大批量确认"] = "False";
                dr["销售确认"] = "False";
                dr["客户"] = searchLookUpEdit1.Text;
                dt_送样申请.Rows.Add(dr);
                string sql = "select * from 送样申请表 where 1<>1";
                fun_SetDataTable(dt_送样申请, sql);
                int i = 1;
                foreach (DataRow dr2 in dt_送样明细.Rows)
                {
                    dr2["申请批号"] = txt_出入库申请单号.Text;
                    dr2["申请批号明细"] = txt_出入库申请单号.Text + "-" + i;
                    dr2["申请日期"] = time_申请日期.Text;
                    i++;
                }
                string sql3 = "select * from 送样明细表 where 1<>1";
                fun_SetDataTable(dt_送样明细, sql3);
                MessageBox.Show("操作成功");
                dt_送样明细.Clear();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
           
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void fun_GetDataTable(DataTable dt, string sql)
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                da.Fill(dt);
            }
        }

        private void fun_SetDataTable(DataTable dt, string sql)
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                new SqlCommandBuilder(da);
                da.Update(dt);
            }
        }

        private void fun_显示() //读取相应数据并匹配显示
        {
            DataRow myDataRow = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            DataTable dt = new DataTable();
            string F_DriverName = myDataRow["产品"].ToString();
            string sql = "select * from 基础数据物料信息表 where 原ERP物料编号 ='" + F_DriverName + "'";
            fun_GetDataTable(dt, sql);
            if (dt.Rows.Count > 0)
            {
                string s_物料编码 = dt.Rows[0]["物料编码"].ToString();
                DataTable dt_仓库物料数量表 = new DataTable();
                string sql2 = "select * from 仓库物料数量表 where 物料编码 ='" + s_物料编码 + "'";
                fun_GetDataTable(dt_仓库物料数量表, sql2);
                DataTable dt_基础数据物料信息表 = new DataTable();
                string sql3 = "select * from 基础数据物料信息表 where 原ERP物料编号 ='" + F_DriverName + "'";
                fun_GetDataTable(dt_基础数据物料信息表, sql3);
                myDataRow["原ERP物料编号"] = dt.Rows[0]["原ERP物料编号"];
                myDataRow["物料名称"] = dt.Rows[0]["物料名称"];
                myDataRow["n原ERP规格型号"] = dt.Rows[0]["n原ERP规格型号"];
                myDataRow["货架描述"] = dt.Rows[0]["货架描述"];
                myDataRow["仓库名称"] = dt.Rows[0]["仓库名称"];
                myDataRow["物料编码"] = dt.Rows[0]["物料编码"];
                myDataRow["库存总数"] = dt_仓库物料数量表.Rows[0]["库存总数"];
            }
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                using (OpenFileDialog op = new OpenFileDialog())
                {
                    if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (op.OpenFile() != null)
                        {
                            string str = op.FileName;//文件带有完整路径的名字
                            bt = Path.GetFileNameWithoutExtension(op.FileName);//只有名字
                            type = op.FileName.Substring(op.FileName.LastIndexOf("."), op.FileName.Length - op.FileName.LastIndexOf(".")).Replace(".", "");
                            ss = bt + (".") + type;
                            bts = System.IO.File.ReadAllBytes(str);
                            byte[] imagedata = (bts);
                            MemoryStream myStream = new MemoryStream();
                            foreach (byte a in imagedata)
                            {
                                myStream.WriteByte(a);
                            }
                            myImage = Image.FromStream(myStream);
                            myStream.Close();

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.Caption == "产品（新产品写备注里）")
            {
                try
                {
                    fun_显示();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
