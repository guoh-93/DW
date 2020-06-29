using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;


namespace BaseData
{
    public partial class sop批量上传 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        DataTable dt_大类;
        DataTable dt_小类;
        DataTable dtM;
        DataRow dr_cs;
        public sop批量上传(DataRow  r )
        {
            InitializeComponent();
            this.dr_cs = r;
        }

        private void sop批量上传_Load(object sender, EventArgs e)
        {
            string sql = "select 物料类型名称 from 基础数据物料类型表 where 类型级别 = '大类' order by 物料类型名称";
            dt_大类 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            da.Fill(dt_大类);
            searchLookUpEdit3.Properties.DataSource = dt_大类;
            searchLookUpEdit3.Properties.DisplayMember = "物料类型名称";
            searchLookUpEdit3.Properties.ValueMember = "物料类型名称";
            searchLookUpEdit3.EditValue = dr_cs["大类"].ToString();

             sql = string.Format(@"select 物料类型名称 as 小类 from  [基础数据物料类型表] where 
            上级类型GUID in  (select 物料类型GUID from [基础数据物料类型表]
            where 类型级别='大类' and 物料类型名称='{0}' ) order by 物料类型名称", searchLookUpEdit3.EditValue.ToString());
            dt_小类 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            searchLookUpEdit4.Properties.DataSource = dt_小类;
            searchLookUpEdit4.Properties.DisplayMember = "小类";
            searchLookUpEdit4.Properties.ValueMember = "小类";
            searchLookUpEdit4.EditValue = dr_cs["小类"].ToString();


        }

        private void searchLookUpEdit3_EditValueChanged(object sender, EventArgs e)
        {
            string sql = string.Format(@"select 物料类型名称 as 小类 from  [基础数据物料类型表] where 
            上级类型GUID in  (select 物料类型GUID from [基础数据物料类型表]
            where 类型级别='大类' and 物料类型名称='{0}' ) order by 物料类型名称", searchLookUpEdit3.EditValue.ToString());
            dt_小类 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            searchLookUpEdit4.Properties.DataSource = dt_小类;
            searchLookUpEdit4.Properties.DisplayMember = "小类";
            searchLookUpEdit4.Properties.ValueMember = "小类";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_search();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }
        private void  fun_search()
        {
            if (searchLookUpEdit4.EditValue != null && searchLookUpEdit3.EditValue != null)
            {
                string s = string.Format(@"select  a.物料编码,a.规格型号,a.细类,b.版本 as sop版本 from 基础数据物料信息表 a
                                left join 作业指导书文件表 b on 类别名称=物料编码  where 大类='{0}' and 小类='{1}'", searchLookUpEdit3.EditValue, searchLookUpEdit4.EditValue);
                dtM=new DataTable ();
                dtM=CZMaster.MasterSQL.Get_DataTable(s,strcon);
                gridControl1.DataSource = dtM;
                dtM.Columns.Add("选择", typeof(bool));


            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        //上传    
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = null;
                fun_check();
                DataRow[] rr = dtM.Select("选择=true");
                DataTable dtP=dtM.Clone();
                foreach (DataRow r in  rr)
                {
                    dtP.ImportRow(r);
                }
                if (MessageBox.Show(string.Format("是否确认批量上传选中的{0}条产品的作业指导书？", rr.Length), "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    OpenFileDialog openfile = new OpenFileDialog();
                    if (openfile.ShowDialog() == DialogResult.OK)
                    {
                        fun_上传(openfile.FileName,dtP);
                        //fun_单条刷新();
                        MessageBox.Show("文件批量上传成功！");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void fun_上传(string pathName,DataTable dtP)
        {
            try
            {
                //gridView1.CloseEditor();
                //this.BindingContext[dtP].EndCurrentEdit();
                string strConn_FS = CPublic.Var.geConn("FS");
                CFileTransmission.CFileClient.strCONN = strConn_FS;

                string strguid = CFileTransmission.CFileClient.sendFile(pathName);
                string type = "";
                //type = pathName.Substring(pathName.LastIndexOf("."), pathName.Length - pathName.LastIndexOf(".")).Replace(".", "");
                int s = pathName.LastIndexOf(".") + 1;
                type = pathName.Substring(s, pathName.Length - s);
                string sql = string.Format("select  * from 作业指导书文件表 where 1=2");
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                foreach (DataRow r in dtP.Rows)
                {
                    DataRow dr = dt.NewRow();
                    dt.Rows.Add(dr);
                    dr["类别名称"] = r["物料编码"];
                    dr["类别分组"] = "单个产品";
                    dr["文件地址"] = strguid;
                    dr["后缀"] = type;
                    //版本需要再进行判断
                    string x = string.Format("select max(版本)版本 from 作业指导书文件表 where 类别名称='{0}'", r["物料编码"]);
                    DataTable t = CZMaster.MasterSQL.Get_DataTable(x,strcon);
                    if (t.Rows.Count == 0)
                    {
                        dr["版本"] = 0;
                    }
                    else
                    {
                        if (dr["版本"].ToString().Trim() == "")
                        {
                            dr["版本"] = 0;
                        }
                        else
                        {
                            dr["版本"] = Convert.ToInt32(t.Rows[0][0]) + 1;
                        }
                    }
                    dr["文件名"] = Path.GetFileName(pathName);
                    dr["上传时间"] = CPublic.Var.getDatetime();
                    dr["修改时间"] = CPublic.Var.getDatetime();
                    dr["修改人"] = CPublic.Var.localUserName;
                }
 
                FileInfo info = new FileInfo(pathName);
                long maxinfo = info.Length;

                if (maxinfo > 1024 * 1024 * 8)
                {
                    throw new Exception("上传的文件不能超过1M，请重新选择上传！");
                }



                CZMaster.MasterSQL.Save_DataTable(dt, "作业指导书文件表", strcon);

                //byte[] bs = System.IO.File.ReadAllBytes(pathName);
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void fun_check()
        {
            if (dtM.Rows.Count == 0)
            {
                throw new Exception("尚未筛选出任何数据");
            }
            DataRow [] dr = dtM.Select(string.Format("选择=1"));
            if (dr.Length == 0)
            {
                throw new Exception("未勾选任何物料");
            }


        }

    }
}
