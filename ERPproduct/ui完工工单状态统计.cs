using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace ERPproduct
{
    public partial class ui完工工单状态统计 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dtMX;
        DataTable dtHZ;
        public ui完工工单状态统计()
        {
            InitializeComponent();
        }

        private void ui完工工单状态统计_Load(object sender, EventArgs e)
        {
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(this.panel2, this.Name, cfgfilepath);
            DateTime t = CPublic.Var.getDatetime().Date;
            t = t.AddDays(1).AddSeconds(-1);
            dateEdit2.EditValue = t;
            dateEdit1.EditValue = t.AddMonths(-1).Date;
            fun_下拉框();
        }

        private void fun_下拉框()
        {
            string sql = "select POS,属性值 as 车间名称 ,属性字段1 as 车间编号 from 基础数据基础属性表 where 属性类别 = '生产车间'";
            DataTable dt_车间 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            searchLookUpEdit1.Properties.DataSource = dt_车间;
            searchLookUpEdit1.Properties.DisplayMember = "车间名称";
            searchLookUpEdit1.Properties.ValueMember = "车间编号";
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                ERPorg.Corg.FlushMemory();
                fun_check();
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_check()
        {
            if (checkBox1.Checked == true)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择车间");
                }
            }
        }

        private void fun_load()
        {
            DateTime t1 = Convert.ToDateTime(dateEdit1.EditValue);
            t1 = new DateTime(t1.Year, t1.Month, t1.Day);
            DateTime t2 = Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1);
            t2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);
            string sql = string.Format(@"select a.生产工单号,a.物料编码,a.物料名称,a.规格型号,a.生产数量,a.完工,a.生产车间,a.车间名称,a.已检验数量,a.完工日期,b.已入库数,c.不合格数,c.合格数,c.已入库数 as 入库数,c.送检数,c.重检合格数 from 生产记录生产工单表 a 
                                         left join (select 生产工单号,SUM(入库数量)已入库数 from 生产记录成品入库单明细表 group by 生产工单号) b on a.生产工单号 = b.生产工单号
                                         left join (select 生产工单号,SUM(合格数量)合格数,SUM(不合格数量)不合格数,SUM(送检数量)送检数,SUM(重检合格数)重检合格数,SUM(已入库数量)已入库数 from 生产记录生产检验单主表 group by 生产工单号) c on a.生产工单号 = c.生产工单号 
                                         where  a.完工 = 1 and 关闭 = 0 and a.完工日期>'{0}' and a.完工日期<'{1}'", t1, t2);
            string sql_补 = "";
            if (checkBox1.Checked == true)
            {
                sql_补 = string.Format(" and a.生产车间 = '{0}'",searchLookUpEdit1.EditValue.ToString());
                sql += sql_补;
            }
            dtMX = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            dtMX.Columns.Add("合格率", typeof(string));
            
            if (dtMX.Rows.Count > 0)
            {
                foreach (DataRow dr in dtMX.Rows)
                {
                    if(Convert.ToDecimal(dr["已检验数量"]) == 0)
                    {
                        dr["合格率"] = "0%";
                        dr["已入库数"] = 0;
                    }
                    else if (dr["已入库数"].ToString() == "")
                    {
                        dr["已入库数"] = 0;
                        dr["合格率"] = Math.Round(100 - (Convert.ToDecimal(dr["不合格数"]) - Convert.ToDecimal(dr["重检合格数"])) / Convert.ToDecimal(dr["已检验数量"]) * 100, 2, MidpointRounding.AwayFromZero) + "%";
                    }
                    else
                    {
                        dr["合格率"] = Math.Round(100 - (Convert.ToDecimal(dr["不合格数"]) - Convert.ToDecimal(dr["重检合格数"])) / Convert.ToDecimal(dr["已检验数量"]) * 100, 2, MidpointRounding.AwayFromZero) + "%";

                    }
                }
            }
            gridControl1.DataSource = dtMX;
            MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();

            dtHZ = RBQ.SelectGroupByInto("", dtMX, "物料编码,物料名称,规格型号,sum(生产数量) 完工数量,sum(已入库数) 已入库数,sum(已检验数量) 已检验数量,sum(不合格数) 不合格数,sum(合格数) 合格数,sum(重检合格数) 重检合格数 ", "", "物料编码,物料名称,规格型号");
            dtHZ.Columns.Add("合格率", typeof(string));
            if (dtHZ.Rows.Count > 0)
            {
                foreach (DataRow dr in dtHZ.Rows)
                {
                    if (Convert.ToDecimal(dr["已检验数量"]) == 0)
                    {
                        dr["合格率"] = "0%";
                    }                 
                    else
                    {
                        dr["合格率"] = Math.Round(100 - (Convert.ToDecimal(dr["不合格数"]) - Convert.ToDecimal(dr["重检合格数"])) / Convert.ToDecimal(dr["已检验数量"]) * 100, 2, MidpointRounding.AwayFromZero) + "%";

                    }
                }
            }
            gridControl2.DataSource = dtHZ;

        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
