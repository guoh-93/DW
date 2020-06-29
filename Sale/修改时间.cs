using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ERPSale
{
    public partial class 修改时间 : Form
    {
        public bool bl = false; //表示开票列表界面数据是否需要刷新
        public bool data = false;
        DateTime t;
        DateTime time_结算;
        string s_原号 = "";
        public string  s_改="";
        string strcon = CPublic.Var.strConn;
        public 修改时间(DateTime t_结算,DateTime t_原,string s)
        {
            InitializeComponent();
            t = t_原;
            time_结算 = t_结算;
            s_原号 = s;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //string s = "  select  MAX(结算日期)结算日期 from 仓库月出入库结转表  ";
                //DataRow dr = CZMaster.MasterSQL.Get_DataRow(s, strcon);
                //DateTime ?time;
                //if(dr!=null&& dr["结算日期"].ToString()!="")
                //{
                //    time = Convert.ToDateTime(dr["结算日期"]);
                //    if (time > t) //结算过的单子不允许修改
                //    {
                //        throw new Exception();
                //    }
                //}
    
                if (MessageBox.Show("确定调整开票该记录的开票日期？", "确认!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    if (dateEdit1.EditValue == null || dateEdit1.EditValue.ToString() == "") throw new Exception("日期尚未选择");
                    if(time_结算 > Convert.ToDateTime(dateEdit1.EditValue))
                    {
                        throw new Exception("该时间单据已结算完成,不可调至该时间");
                    }
                    else
                    {
                        string log = "";
                        DateTime t_改 = Convert.ToDateTime(dateEdit1.EditValue);
                        string s_原 = t.Year.ToString() + t.Month.ToString();
                         s_改 = t_改.Year.ToString() + t_改.Month.ToString();

                        if (Convert.ToInt32(s_改)!= Convert.ToInt32(s_原)) //需要 重新生成单号
                        {
                            string  ss = string.Format("SS{0}{1:D2}{2:D4}", t_改.Year, t_改.Month, CPublic.CNo.fun_得到最大流水号("SS", t_改.Year, t_改.Month));
                            string sql =string.Format(@"update [销售记录销售开票主表] set 销售开票通知单号='{0}',开票日期='{2}' where 销售开票通知单号='{1}'
                                       update [销售记录销售开票明细表] set 销售开票通知单号='{0}' where 销售开票通知单号='{1}' ", ss,s_原号, t_改);
                            CZMaster.MasterSQL.ExecuteSQL(sql,strcon);
                           log="修改成功,新号为:"+ ss;
                            s_改 = ss;
                        }
                        else
                        {
                            log = "修改成功";
                            s_改 = s_原号;
                        }
                        bl = true;
                        MessageBox.Show(log);
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
      
            this.Close();
        }
    }
}
