using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
namespace ERPpurchase
{
    public partial class ui来料批次打印 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        DataTable dtM;
        /// <summary>
        /// 指示是否正在打印 防止用户疯狂点击
        /// </summary>
        bool flag = false;
        public ui来料批次打印()
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

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
            SendKeys.Send("{1}");
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
            SendKeys.Send("{2}");
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
            SendKeys.Send("{3}");
        }
        private void simpleButton5_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
            SendKeys.Send("{4}");
        }
        private void simpleButton6_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
            SendKeys.Send("{5}");
        }

        private void simpleButton7_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
            SendKeys.Send("{6}");
        }

        private void simpleButton8_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
            SendKeys.Send("{7}");
        }

        private void simpleButton9_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
            SendKeys.Send("{8}");
        }

        private void simpleButton10_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
            SendKeys.Send("{9}");
        }

        private void simpleButton12_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
            SendKeys.Send("{0}");

        }
        //退格
        private void simpleButton11_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
            SendKeys.Send("{Backspace}");
        }
        //搜索
        private void simpleButton13_Click(object sender, EventArgs e)
        {
            //需要验证 是 供应商编码 还是 供应商卡号
            string s = string.Format(@"select  送检单号,送检数量,送检日期,原ERP物料编号,基础数据物料信息表.物料名称,基础数据物料信息表.图纸编号,基础数据物料信息表.物料编码,供应商 from 采购记录采购送检单明细表
                left  join  基础数据物料信息表 on 基础数据物料信息表.物料编码=采购记录采购送检单明细表.物料编码
                where 检验完成 = 0  and  供应商ID='{0}' and 生效日期>'2017-1-1' order by 生效日期 desc", textBox1.Text);
            using (SqlDataAdapter da = new SqlDataAdapter(s, strcon))
            {
                dtM = new DataTable();
                da.Fill(dtM);
                dtM.Columns.Add("选择", typeof(bool));
                gridControl1.DataSource = dtM;


                if (dtM.Rows.Count == 0)
                {



                    MessageBox.Show("未找到记录");
                }
                else
                {
                    label3.Text = "供应商:" + dtM.Rows[0]["供应商"].ToString();
                }
            }
        }
        //打印 
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_check();
                if (!flag)
                {
                    if (MessageBox.Show("确定打印？请核对。", "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {


                        gridView1.CloseEditor();
                        this.BindingContext[dtM].EndCurrentEdit();
                        DataView dv = new DataView(dtM);
                        dv.RowFilter = "选择=1";


                        DataTable dt = dv.ToTable();


                        Thread thDo;
                        thDo = new Thread(() => fun_打印(dt));
                        thDo.IsBackground = true;
                        thDo.Start();
                        flag = true;
                        simpleButton13_Click(null, null); //刷新
                    }



                }

                else
                {
                    BaseData.frm消息弹框 fm1 = new BaseData.frm消息弹框("正在打印,请稍候...");
                    fm1.ShowDialog();

                }
            }
            catch (Exception ex)
            {
                BaseData.frm消息弹框 fm1 = new BaseData.frm消息弹框(ex.Message);
                fm1.ShowDialog();
            }


        }
        private void fun_check()
        {
            if (textBox1.Text.ToString() == "")
            {
                throw new Exception("未选择供应商");

            }
            if (dtM == null)
            {

                throw new Exception("还未搜索数据");

            }
            DataRow[] dr = dtM.Select("选择=1");
            if (dr.Length == 0)
            {

                throw new Exception("未选择打印记录");

            }
        }
        private void fun_打印(DataTable dt_dy)
        {

            string str_打印机 = this.printDocument1.PrinterSettings.PrinterName;
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            foreach (DataRow r in dt_dy.Rows)
            {
                //dic.Add("gdh", r_六课["生产工单号"].ToString());
                string s = string.Format("select 物料编码,最小包装 from 基础数据物料信息表 where 物料编码 ='{0}'", r["物料编码"]);
                using (SqlDataAdapter da = new SqlDataAdapter(s, strcon))
                {
                    DataTable temp = new DataTable();
                    da.Fill(temp);
                    int i_最小包装 = Convert.ToInt32(temp.Rows[0]["最小包装"]);
                    if (i_最小包装 == 0)
                    {

                        ERPproduct.fm补打箱贴标签 fm = new ERPproduct.fm补打箱贴标签(r);
                        fm.StartPosition = FormStartPosition.CenterParent;
                        fm.ShowDialog();

                        if (fm.zxbz == 0)
                        {
                            throw new Exception(string.Format("未完成对物料 {0} 的最小包装的维护", r["图纸编号"].ToString()));
                        }
                        else
                        {
                            i_最小包装 = fm.zxbz;
                            temp.Rows[0]["最小包装"] = i_最小包装;
                            new SqlCommandBuilder(da);
                            da.Update(temp);


                        }
                    }

                    if (i_最小包装 != 0)
                    {
                        int count = Convert.ToInt32(r["送检数量"]) / i_最小包装;
                        int i_余数 = Convert.ToInt32(r["送检数量"]) % i_最小包装;
                        if (i_余数 != 0)
                        {
                            count++;
                        }
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("gdh", r["送检单号"].ToString());

                        dic.Add("pch", r["送检单号"].ToString() + "-");
                        dic.Add("dyzs", count.ToString("0"));
                        dic.Add("ybh", r["原ERP物料编号"].ToString());
                        dic.Add("tzbh", r["图纸编号"].ToString());
                        dic.Add("scsj", CPublic.Var.getDatetime().ToString("yyyy-MM-dd"));
                        dic.Add("sl", i_最小包装.ToString("0"));
                        dic.Add("ys", i_余数.ToString("0"));


                        string path = Application.StartupPath + @"\Mode\制六标签.lab";
                        ERPproduct.Lprinter lp = new ERPproduct.Lprinter(path, dic, str_打印机, count);
                        lp.Start();

                        //for (int i = 1; i <= count; i++)
                        //{
                        //    Dictionary<string, string> dic = new Dictionary<string, string>();
                        //    dic.Add("gdh", r["送检单号"].ToString());

                        //    dic.Add("pch", r["送检单号"].ToString() + "-" + i.ToString());
                        //    dic.Add("ybh", r["原ERP物料编号"].ToString());
                        //    dic.Add("tzbh", r["图纸编号"].ToString());
                        //    dic.Add("scsj", CPublic.Var.getDatetime().ToString("yyyy-MM-dd"));
                        //    if (i == count && i_余数 != 0)
                        //    {
                        //        dic.Add("sl", i_余数.ToString("0"));
                        //    }
                        //    else
                        //    {
                        //        dic.Add("sl", i_最小包装.ToString("0"));
                        //    }
                        //    list.Add(dic);
                        //}
                    }



                }

                //        //Dictionary<string, string> dic = new Dictionary<string, string>();
                //        //dic.Add("gdh", r_六课["生产工单号"].ToString());





            }
            // 制六标签 和 送检标签 一样 只是 单号 不同 
            //string path = Application.StartupPath + @"\Mode\制六标签.lab";
            //ERPproduct.Lprinter lp = new ERPproduct.Lprinter(path, list, str_打印机, 1);
            //lp.Start();
            flag = false;

        }

        private void simpleButton14_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                gridView1.GetDataRow(i)["选择"] = true;

            }
        }

        private void simpleButton15_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                gridView1.GetDataRow(i)["选择"] = false;

            }
        }

 

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                string sql = string.Format("select  供应商ID from 采购供应商表 where 供应商卡号='{0}' ", textBox1.Text);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        textBox1.Text = dt.Rows[0]["供应商ID"].ToString();
                    }

                }
            }
        }




    }

}
