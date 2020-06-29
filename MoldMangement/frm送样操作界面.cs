using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace MoldMangement
{
    public partial class frm送样操作界面 : UserControl
    {
        DataTable dt_员工号;
        DataTable dt_送样申请表;
        DataTable dt_责任人;
        DataTable dt_送样明细;
        DataTable dt_基础数据物料信息表;
        DataRow dr_当前行;
        CurrencyManager cmM;
        string s_部门 = "";
        string s_职务 = "";
        public frm送样操作界面()
        {
            InitializeComponent();
        }

        private void frm送样操作界面_Load(object sender, EventArgs e)
        {

            dt_员工号 = new DataTable();
            string sql2 = "select * from 人事基础员工表 where  员工号= '" + CPublic.Var.LocalUserID + "'";
            fun_GetDataTable(dt_员工号, sql2);
            s_部门 = dt_员工号.Rows[0]["部门"].ToString();
            s_职务 = dt_员工号.Rows[0]["岗位"].ToString();
            dt_基础数据物料信息表 = new DataTable();
            string sql3 = @"select 基础数据物料信息表.物料编码,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.物料名称,基础数据物料信息表.n原ERP规格型号,
            基础数据物料信息表.图纸编号,仓库物料数量表.库存总数,货架描述,仓库名称 from 基础数据物料信息表 
            left join 仓库物料数量表 on 基础数据物料信息表.物料编码 = 仓库物料数量表.物料编码";
            fun_GetDataTable(dt_基础数据物料信息表, sql3);
            repositoryItemSearchLookUpEdit1.DataSource = dt_基础数据物料信息表;
            repositoryItemSearchLookUpEdit1.DisplayMember = "原ERP物料编号";
            repositoryItemSearchLookUpEdit1.ValueMember = "原ERP物料编号";
            dt_责任人 = new DataTable();
            string sql4 = "select 员工号,姓名 from 人事基础员工表 where 部门 = '开发一部' or 部门 = '开发二部' or 部门 = '开发部'";
            fun_GetDataTable(dt_责任人, sql4);
            repositoryItemSearchLookUpEdit2.DataSource = dt_责任人;
            repositoryItemSearchLookUpEdit2.DisplayMember = "姓名";
            repositoryItemSearchLookUpEdit2.ValueMember = "姓名";
            dt_送样申请表 = new DataTable();
            string sql ="";
            if (s_职务 == "产品线经理" || s_职务 == "副总经理")
            {
                sql = "select * from 送样申请表 where 大批量确认状态 = 'True' and 完成= 'False'";
            }
            else
            {
                sql = "select * from 送样申请表 where 完成= 'False'";
            }
            fun_GetDataTable(dt_送样申请表, sql);
            gridControl1.DataSource = dt_送样申请表;
            dt_送样申请表.Columns.Add("上传");
            dt_送样申请表.Columns.Add("确认状态");
            //dt_送样申请表.Columns.Add("大批量确认状态", typeof(bool));
            foreach (DataRow dr in dt_送样申请表.Rows)
            {
                if (dr["上传备注"].ToString() != "")
                {
                    dr["上传"] = "技术文件";
                }
                if (dr["销售确认"].ToString() == "False")
                {
                    dr["确认状态"] = "未确认";
                }
                if (dr["销售确认"].ToString() == "True")
                {
                    dr["确认状态"] = "销售已确认";
                }
                if (dr["销售确认"].ToString() == "True" && dr["技术确认"].ToString() == "True")
                {
                    dr["确认状态"] = "技术已确认";
                }
                
            }

            if (s_部门 == "开发部" || s_部门 == "开发一部" || s_部门 == "开发二部" || s_部门 == "营销部")
            {
                this.barLargeButtonItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }
            else
            {
                this.barLargeButtonItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }

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

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (dr_当前行["责任人"].ToString() != "")
            {
                if (s_部门 == "开发部" || s_部门 == "开发一部" || s_部门 == "开发二部")
                {
                    if (dr_当前行["销售确认"].ToString() == "True")
                    {
                        dr_当前行["技术确认"] = "True";
                    }
                    else
                    {
                        MessageBox.Show("请先让销售确认");
                    }
                }
                if (s_部门 == "营销部")
                {
                    dr_当前行["销售确认"] = "True";
                }
                if (dr_当前行["大批量确认状态"].ToString() == "True")
                {
                    if (dr_当前行["技术大批量确认"].ToString() == "True")
                    {
                        dr_当前行["确认完成"] = "True";
                    }
                    else
                    {
                        dr_当前行["确认完成"] = "False";
                    }
                }
                if (dr_当前行["大批量确认状态"].ToString() == "False")
                {
                    if (dr_当前行["技术确认"].ToString() == "True")
                    {
                        dr_当前行["确认完成"] = "True";
                    }
                    else
                    {
                        dr_当前行["确认完成"] = "False";
                    }
                }
                string sql = "select * from 送样申请表 where 1<>1";
                fun_SetDataTable(dt_送样申请表, sql);
            }
            else
            {
                MessageBox.Show("责任人不能为空");
            }

        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            dr_当前行 = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            string s_申请编号 = dr_当前行["申请编号"].ToString();
            dt_送样明细 = new DataTable();
            string sql = "select * from 送样明细表 where 申请批号 = '" + s_申请编号 + "'";
            fun_GetDataTable(dt_送样明细, sql);
            gridControl2.DataSource = dt_送样明细;
        }


        private void repositoryItemButtonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            dr_当前行 = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (dr_当前行["上传"].ToString() != "")
            {
                string s_后缀 = dr_当前行["上传备注后缀"].ToString();
                byte[] by = (byte[])(dr_当前行["上传备注"]);//内存byte流
                string s_文件名 = dr_当前行["上传备注文件名"].ToString();
                //string s_路径 = "D://MasterData//" + s_文件名;
                string s_路径2 = System.Environment.CurrentDirectory + "//备注." + s_后缀;
                System.IO.Stream s = new System.IO.MemoryStream(by);
                Image image = Image.FromStream(s);
                image.Save(s_路径2);
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = s_路径2;
                process.StartInfo.Arguments = "rundll32.exe C://WINDOWS//system32//shimgvw.dll,ImageView_Fullscreen";
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                process.Start();
                process.Close();
            }
        }

        private void gridView2_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
           
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            cmM = BindingContext[dt_送样明细] as CurrencyManager;
            gridView2.CloseEditor();
            cmM.EndCurrentEdit();
            try
            {
                cmM.AddNew();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            cmM = BindingContext[dt_送样明细] as CurrencyManager;
            gridView2.CloseEditor();
            cmM.EndCurrentEdit();
            try
            {
                (cmM.Current as DataRowView).Row.Delete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            string sql = "select * from 送样明细表 where 1<>1";
            fun_SetDataTable(dt_送样明细, sql);

        }

        private void gridView2_CellValueChanged_1(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.Caption == "原ERP物料编号")
            {
                DataRow myDataRow = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                DataTable dt = new DataTable();
                string F_DriverName = myDataRow["原ERP物料编号"].ToString();
                string sql = "select * from 基础数据物料信息表 where 原ERP物料编号 ='" + F_DriverName + "'";
                fun_GetDataTable(dt, sql);
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
                //myDataRow["库存总数"] = dt_仓库物料数量表.Rows[0]["库存总数"];
                //myDataRow["物料单价"] = dt_基础数据物料信息表.Rows[0]["n核算单价"];
                //myDataRow["工号"] = CPublic.Var.LocalUserID;
                //myDataRow["申请人"] = CPublic.Var.localUserName;
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frm送样操作界面_Load(null, null);

        }




    }
}
