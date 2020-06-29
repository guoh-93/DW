using System;
using System.Data;
using System.Windows.Forms;

namespace 郭恒的DEMO
{
    public partial class 账龄分析转列 : Form
    {
        public 账龄分析转列()
        {
            InitializeComponent();
        }
        string strcon = CPublic.Var.strConn;

        private void 账龄分析转列_Load(object sender, EventArgs e)
        {
            fun_load();
        }

        private void fun_load()
        {
            string s = @"   select   [产品分类（大类）],b.材料类别,sum(三年内) 三年内,sum(三年以上) 三年以上 from [AAAA] a
  left join[AA_账龄] b on a.原码 = b.物料编码
  group by  [产品分类（大类）],b.材料类别 order by [产品分类（大类）] ";
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            s = "select  材料类别 from [AAAA] group by 材料类别";
            DataTable list = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            DataTable dtM = new DataTable();

 
            dtM.Columns.Add("材料类别");
            dtM.Columns.Add("三年内");
            dtM.Columns.Add("三年以上");
            dtM.Columns.Add("分类");

            foreach (DataRow dr in list.Rows)
            {
                DataRow rr = dtM.NewRow();

               

                DataRow[] list_r = dt.Select($"材料类别='{ dr["材料类别"].ToString()}'");
                rr["材料类别"] = list_r[0]["材料类别"];
                rr["三年内"] = 0;
                rr["三年以上"] = 0;

                int x = 0;
                foreach (DataRow xr in list_r)
                {
                    rr["三年内"]   = Convert.ToDecimal(list_r[0]["三年内"])+ Convert.ToDecimal(rr["三年内"]);
                    rr["三年以上"]  = Convert.ToDecimal(list_r[0]["三年以上"])+ Convert.ToDecimal(rr["三年以上"]);
                    if ( x== list_r.Length-1) rr["分类"] += xr["产品分类（大类）"].ToString()  ;
                    else 

                    rr["分类"] += xr["产品分类（大类）"].ToString() +",";
                    x++;
                }

                dtM.Rows.Add(rr);
            }

            gridControl1.DataSource = dtM;












        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                gridView1.ExportToXlsx(saveFileDialog.FileName);

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
