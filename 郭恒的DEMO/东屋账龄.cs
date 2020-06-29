using System;
using System.Data;
using System.Windows.Forms;

namespace 郭恒的DEMO
{
    public partial class 东屋账龄 : Form
    {
        string strcon = CPublic.Var.strConn;
        DataTable t_一年内;
        DataTable t_两年内;
        DataTable t_三年内;
        DataTable t_qs;

        DataTable dtM;
        DataTable dtP;



        public 东屋账龄()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string s = $@"select   a.物料编码,a.物料名称,a.规格型号,本月结转数量,物料等级,本月结转金额,自制 from 基础数据物料信息表 a  
            inner join 仓库月出入库结转表 b on a.物料编码=b.物料编码 where left(a.物料编码,2) not in ('11','20','30') and 年={textBox1.Text} and 月={textBox2.Text} order by 物料编码";
            dtP = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "  select  物料编码,sum(入库数量)入库总数  from 仓库月出入库结转表   where 年=2019  group by 物料编码";
            t_两年内 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select 物料编码, sum(入库数量)入库总数 from 仓库月出入库结转表 where 年 = 2020  group by 物料编码";
            t_一年内 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select  存货编码 as 物料编码,isnull(收入数量,0)入库总数 from [财务18年收发汇总]";
            t_三年内 = CZMaster.MasterSQL.Get_DataTable(s, strcon);



        }

        private void button2_Click(object sender, EventArgs e)
        {
            dtM = new DataTable();
            dtM.Columns.Add("物料编码");
            dtM.Columns.Add("物料名称");
            dtM.Columns.Add("规格型号");
            dtM.Columns.Add("物料等级");
            dtM.Columns.Add("库存总数");
            dtM.Columns.Add("库存金额");
            dtM.Columns.Add("自制",typeof(bool));
            dtM.Columns.Add("一年内", typeof(decimal));
            dtM.Columns.Add("两年内", typeof(decimal));
            dtM.Columns.Add("三年内", typeof(decimal));
            dtM.Columns.Add("三年以上", typeof(decimal));

            foreach (DataRow dr in dtP.Rows)
            {
                decimal dec = Convert.ToDecimal(dr["本月结转数量"]);
                DataRow rr = dtM.NewRow();
                rr["物料编码"] = dr["物料编码"].ToString();
                rr["物料名称"] = dr["物料名称"].ToString();
                rr["规格型号"] = dr["规格型号"].ToString();
                rr["物料等级"] = dr["物料等级"].ToString();
                rr["库存金额"] = dr["本月结转金额"].ToString();

                rr["自制"] = dr["自制"];


                rr["库存总数"] = dec;
                dtM.Rows.Add(rr);
                if (dec == 0) continue;
                //一年内
                DataRow[] one = t_一年内.Select($"物料编码='{dr["物料编码"].ToString()}'");
                if (one.Length > 0)
                {
                    decimal one_in = Convert.ToDecimal(one[0]["入库总数"]);
                    if (one_in >= dec)
                    {
                        rr["一年内"] = dec;
                        continue;
                    }
                    else
                    {
                        dec -= one_in;
                        rr["一年内"] = one_in;
                    }
 
                }

                DataRow[] two = t_两年内.Select($"物料编码='{dr["物料编码"].ToString()}'");
                if (two.Length > 0)
                {
                    decimal two_in = Convert.ToDecimal(two[0]["入库总数"]);
                    if (two_in >= dec)
                    {
                        rr["两年内"] = dec;
                        continue;
                    }
                    else
                    {
                        dec -= two_in;
                        rr["两年内"] = two_in;
                    }
                }
                DataRow[] three = t_三年内.Select($"物料编码='{dr["物料编码"].ToString()}'");
                if (three.Length > 0)
                {
                    decimal three_in = Convert.ToDecimal(three[0]["入库总数"]);
                    if (three_in >= dec)
                    {
                        rr["三年内"] = dec;
                        continue;
                    }
                    else
                    {
                        dec -= three_in;
                        rr["三年内"] = three_in;
                    }
                }

                if (dec>0)
                {
                    rr["三年以上"] = dec;
                }

            
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
