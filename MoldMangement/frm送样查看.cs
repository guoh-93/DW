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
    public partial class frm送样查看 : UserControl
    {
        DataTable dt_送样申请表;
        DataTable dt_送样明细;
        DataTable dt_其他出库主表;
        DataTable dt_其他出库明细;
        DataRow dr_当前行;
        public frm送样查看()
        {
            InitializeComponent();
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

        private void frm送样查看_Load(object sender, EventArgs e)
        {
            dt_其他出库主表 = new DataTable();
            string sql2 = "select * from 其他出入库申请主表 where 1<>1";
            fun_GetDataTable(dt_其他出库主表, sql2);
            dt_其他出库明细 = new DataTable();
            string sql3 = "select * from 其他出入库申请子表 where 1<>1";
            fun_GetDataTable(dt_其他出库明细, sql3);

            dt_送样申请表 = new DataTable();
            string sql  = "select * from 送样申请表 where 确认完成= 'True'";
            fun_GetDataTable(dt_送样申请表, sql);
            gridControl1.DataSource = dt_送样申请表;
            dt_送样申请表.Columns.Add("上传");
            dt_送样申请表.Columns.Add("确认状态");
            foreach (DataRow dr in dt_送样申请表.Rows)
            {
                if (dr["上传备注"].ToString() != "")
                {
                    dr["上传"] = "技术文件";
                }
            }
        }



        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (dt_送样明细.Rows.Count == 0)
            {
                MessageBox.Show("请选择工单");
                return;
            }

            string s_申请单号 = string.Format("QWSQ{0}{1}{2}{3}", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString("00"),
                            DateTime.Now.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("QWSQ", DateTime.Now.Year, DateTime.Now.Month).ToString("0000"));
            if (dr_当前行["完成"].ToString() == "True")
            {
                MessageBox.Show("不能重复出库");
                return;
            }
            foreach (DataRow dr in dt_送样明细.Rows)
            {
                if (dr["原ERP物料编号"].ToString() == "")
                {
                    MessageBox.Show("请把型号规格填写完成");
                    return;
                }
            }
            DataRow dr_其他出库主表 = dt_其他出库主表.NewRow();
            dr_其他出库主表["GUID"] = System.Guid.NewGuid();
            dr_其他出库主表["出入库申请单号"] = s_申请单号;
            dr_其他出库主表["申请日期"] = System.DateTime.Now;
            dr_其他出库主表["申请类型"] = "送样出库";
            dr_其他出库主表["备注"] = dr_当前行["备注"];
            dr_其他出库主表["操作人员编号"] = CPublic.Var.LocalUserID;
            dr_其他出库主表["操作人员"] = CPublic.Var.localUserName;
            dr_其他出库主表["生效"] = true;
            dr_其他出库主表["生效日期"] = System.DateTime.Now;
            dr_其他出库主表["生效人员编号"] = CPublic.Var.LocalUserID;
            dt_其他出库主表.Rows.Add(dr_其他出库主表);
            int i = 1;
            foreach (DataRow dr in dt_送样明细.Rows)
            {

                DataRow dr_其他出库明细 = dt_其他出库明细.NewRow();
                dr_其他出库明细["GUID"] = System.Guid.NewGuid();
                dr_其他出库明细["出入库申请单号"] = s_申请单号;
                dr_其他出库明细["出入库申请明细号"] = s_申请单号 + i.ToString("00");
                dr_其他出库明细["POS"] = i;
                dr_其他出库明细["物料编码"] = dr["物料编码"];
                dr_其他出库明细["原ERP物料编号"] = dr["原ERP物料编号"];
                dr_其他出库明细["数量"] = dr["数量"];
                dr_其他出库明细["物料名称"] = dr["物料名称"];
                dr_其他出库明细["n原ERP规格型号"] = dr["n原ERP规格型号"];
                dr_其他出库明细["备注"] = dr["备注"];
                dr_其他出库明细["生效"] = "True";
                dr_其他出库明细["生效日期"] = System.DateTime.Now;
                dr_其他出库明细["生效人员编号"] = CPublic.Var.LocalUserID;
                //dr_其他出库明细["完成"] = "";
                //dr_其他出库明细["完成日期"] = "";
                //dr_其他出库明细["完成人员编号"] = "";
                //dr_其他出库明细["作废"] = "";
                //dr_其他出库明细["作废日期"] = "";
                //dr_其他出库明细["作废人员编号"] = "";
                dt_其他出库明细.Rows.Add(dr_其他出库明细);
                i++;
            }
            dr_当前行["完成"] = "True";
            string sql = "select * from 送样申请表 where 1<>1";
            fun_SetDataTable(dt_送样申请表, sql);
            string sql2 = "select * from 其他出入库申请主表 where 1<>1";
            fun_SetDataTable(dt_其他出库主表, sql2);
            string sql3 = "select * from 其他出入库申请子表 where 1<>1";
            fun_SetDataTable(dt_其他出库明细, sql3);


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
            string sql = @"select 送样明细表.*,仓库物料数量表.库存总数,有效总数 from 送样明细表
                         left join 仓库物料数量表 on 送样明细表.物料编码 = 仓库物料数量表.物料编码 where 送样明细表.申请批号 = '" + s_申请编号 + "'";
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

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frm送样查看_Load(null,null);
        }
    }
}
