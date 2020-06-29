using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using CPublic;

namespace 郭恒的DEMO
{
    public partial class Form5 : Form
    {

        DataTable dt_生产关系 = ERPorg.Corg.fun_hr("生产","4003");
        string strcon = CPublic.Var.strConn;
        DataTable dtm;
        public Form5()
        {
            InitializeComponent();
        }

        private void lookUpEdit1_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyValue== 13)
            //{

            //    string sql = string.Format("select 物料编码,物料名称,规格型号,n原ERP规格型号 from 基础数据物料信息表 where 物料编码 like '%{0}%'", lookUpEdit1.Text);
            //    DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            //    lookUpEdit1.Properties.DataSource = dt;
            //    lookUpEdit1.Properties.DisplayMember = "物料编码";
            //    lookUpEdit1.Properties.ValueMember = "物料编码";
            //    lookUpEdit1.ShowPopup();
                
            //}
        }

        private void lookUpEdit1_TextChanged(object sender, EventArgs e)
        {
            if (lookUpEdit1.Text.Length > 2)
            {
               
                string sql = string.Format("select 物料编码,物料名称,规格型号,n原ERP规格型号 from 基础数据物料信息表 where 物料编码 like '%{0}%'", lookUpEdit1.Text);
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                repositoryItemCheckedComboBoxEdit1.DataSource= dt;
                repositoryItemCheckedComboBoxEdit1.DisplayMember = "物料编码";
                repositoryItemCheckedComboBoxEdit1.ValueMember  = "物料编码";
                lookUpEdit1.ShowPopup();
            }
        }

        private void lookUpEdit1_TextChanged_1(object sender, EventArgs e)
        {
            //if (lookUpEdit1.Text.Length > 2)
            //{
            //    string sql = string.Format("select 物料编码,物料名称,规格型号,n原ERP规格型号 from 基础数据物料信息表 where 物料编码 like '%{0}%'", lookUpEdit1.Text);
            //    DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            //    lookUpEdit1.Properties.DataSource = dt;
            //    lookUpEdit1.Properties.DisplayMember = "物料编码";
            //    lookUpEdit1.Properties.ValueMember = "物料编码";
            //    lookUpEdit1.ShowPopup();
            //}
        }

        private void lookUpEdit1_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {

                string sql = string.Format("select 物料编码,物料名称,规格型号,n原ERP规格型号 from 基础数据物料信息表 where 物料编码 like '%{0}%'", lookUpEdit1.Text);
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                lookUpEdit1.Properties.DataSource = dt;
                lookUpEdit1.Properties.DisplayMember = "物料编码";
                lookUpEdit1.Properties.ValueMember = "物料编码";
               
             
                

            }
            lookUpEdit1.ShowPopup();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            dtm = new DataTable();
            dtm.Columns.Add("a");
            DataRow drm = dtm.NewRow();
            dtm.Rows.Add(drm);
            gridControl1.DataSource = dtm;
            DataTable dt = new DataTable();
           dt.Columns.Add("x");
           dt.Columns.Add("y");

           DataRow dr = dt.NewRow();
           dr["x"] = 1;
           dr["y"] = 11;

           dt.Rows.Add(dr);
        
           DataRow dr2 = dt.NewRow();
           dr2["x"] = 2;
           dr2["y"] = 22;

           dt.Rows.Add(dr2);


       
            repositoryItemCheckedComboBoxEdit1.DataSource = dt;
            repositoryItemCheckedComboBoxEdit1.DisplayMember = "x";
            repositoryItemCheckedComboBoxEdit1.ValueMember = "x";
            //string  str_打印机;
            // string path = Application.StartupPath + @"\Mode\标签1.lab";
            //    //DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);
               
                   
            //            this.printDialog1.Document = this.printDocument1;
            //            DialogResult dr = this.printDialog1.ShowDialog();
            //            if (dr == DialogResult.OK)
            //            {
            //                //string str_打印机 = new PrintDocument().PrinterSettings.PrinterName;
            //            str_打印机 = this.printDocument1.PrinterSettings.PrinterName;

            //            }
            //        }
                  
            //            Dictionary<string, string> dic = new Dictionary<string, string>();
            //            dic.Add("fore", strMoNo);
            //            int a = (int)i_生产数 / 12;

            //            if (a == 0) a = 12;
            //            else if (i_生产数 % 12 == 0)
            //            {
            //                a = i_生产数;
            //            }
            //            else
            //            {
            //                a = ((int)i_生产数 / 12 + 1) * 12;
            //            }

            
             
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string sql_未开工产品数 = string.Format(@"select sum(生产数量) as 未开工产品数量,基础数据物料信息表.大类 from 生产记录生产工单表  
                                                left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产工单表.物料编码 
                                        where  生产车间='{0}'and 生产记录生产工单表.生效=0 and 生产记录生产工单表.关闭=0 group by 大类", dt_生产关系.Rows[0]["生产车间"]);

            DataTable dt =new DataTable();
            dt=CZMaster.MasterSQL.Get_DataTable(sql_未开工产品数,strcon);

            string sql_已入库产品数 = string.Format(@"select sum(生产数量)as 已入库产品数 ,基础数据物料信息表.大类 from 生产记录成品入库单明细表 left join  基础数据物料信息表
                                                    on   生产记录成品入库单明细表.物料编码=基础数据物料信息表.物料编码
                                                    left join 生产记录生产检验单主表 on  生产记录成品入库单明细表.生产工单号=生产记录生产检验单主表.生产工单号
                                                  where  生产记录成品入库单明细表.入库车间='{0}' and 生产记录成品入库单明细表.生效日期>'{1}' group by 大类", dt_生产关系.Rows[0]["生产车间"], System.DateTime.Today);
            DataTable dt_1 = new DataTable();
            dt_1 = CZMaster.MasterSQL.Get_DataTable(sql_已入库产品数, strcon);
            dt.Merge(dt_1);

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 3)
            {
                string sql = string.Format("select 物料编码,物料名称,规格型号,n原ERP规格型号  from 基础数据物料信息表 where 物料编码 like '%{0}%'", textBox1.Text);
                DataTable dt = new DataTable();
                dt=CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                gridControl1.DataSource = null;
                gridControl1 .DataSource = dt;
               
            }
        }
    }
}
