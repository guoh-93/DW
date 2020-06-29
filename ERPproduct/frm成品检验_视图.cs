using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace ERPproduct
{
    public partial class frm成品检验_视图 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        string str_成品检验单号 = "";
        DataTable dt_检验明细;
        DataTable dt_主;
    
        #endregion

        #region 自用类
        public frm成品检验_视图()
        {
            InitializeComponent();
        }

        public frm成品检验_视图(string str_检验单号,bool bl)
        {
            InitializeComponent();
            str_成品检验单号 = str_检验单号;
            if (bl)
            {
                simpleButton1.Visible = false;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm成品检验_视图_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_载入();

            }
            catch (Exception ex)
            {

            }
        }       
        #endregion

        #region 方法
#pragma warning disable IDE1006 // 命名样式
        private void fun_载入()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format("select * from 成品检验检验记录明细表 where 生产检验单号 = '{0}'", str_成品检验单号);
            dt_检验明细 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_检验明细);
            int i = 1;
            foreach(DataRow r_序号 in dt_检验明细.Rows)
            {
                r_序号["序号"] = i;
                i++;
            }
            gcM.DataSource = dt_检验明细;

            string sql_主 = string.Format(@"select 生产记录生产检验单主表.*,生产记录生产工单表.备注1 as 备注 from 生产记录生产检验单主表 left join 生产记录生产工单表 
                on 生产记录生产检验单主表.生产工单号 = 生产记录生产工单表.生产工单号
                where 生产检验单号 = '{0}'", str_成品检验单号);
             dt_主 = new DataTable();
            SqlDataAdapter da_主 = new SqlDataAdapter(sql_主, strconn);
            da_主.Fill(dt_主);
            if (dt_主.Rows.Count > 0)
            {
                dataBindHelper1.DataFormDR(dt_主.Rows[0]);
            }
            //加载产品序列号
            DataTable dt_序号;
            string sql_序号 = string.Format("select * from 生产检验单与产品序列号对应关系表 where 生产检验单号 = '{0}'", str_成品检验单号);
            using (SqlDataAdapter da1 = new SqlDataAdapter(sql_序号, strconn))
            {
                 dt_序号 = new DataTable();
                da1.Fill(dt_序号);
                gridControl1.DataSource = dt_序号;
            }
            string sql_子 = "";
            if(dt_序号.Rows.Count ==0)
            {
             sql_子 = string.Format("select * from 成品检验检验记录返工表 where 生产检验单号 = '{0}'", str_成品检验单号);
            }
            else
            {
                sql_子 = string.Format("select * from 序列号返工原因对应表 where 生产检验单号 = '{0}'", str_成品检验单号);
            }
            DataTable dt_子 = new DataTable();
            SqlDataAdapter da_子 = new SqlDataAdapter(sql_子, strconn);
            da_子.Fill(dt_子);
            gcP.DataSource = dt_子;
         

        }

        #endregion

        #region 界面操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                //打印
                if (MessageBox.Show("是否打印出厂检验记录？", "询问？", MessageBoxButtons.OKCancel ) == DialogResult.OK)
                {
                    //序号选中行传值
                    DataRow dr_产品序序号 = gridView2.GetDataRow(gridView2.FocusedRowHandle);

                    string sql = string.Format("select 物料编码,物料名称,规格型号 as 规格,大类,小类 from 基础数据物料信息表 where 物料编码 = '{0}'", txt_物料编码.Text);
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_传 = new DataTable();

                    da.Fill(dt_传);
                    dt_传.Columns.Add("发货数量");
                    dt_传.Columns.Add("合格数");
                    dt_传.Columns.Add("生产者");
                    dt_传.Columns.Add("班组");
                    dt_传.Columns.Add("部门");
                    dt_传.Columns.Add("生产日期");

                    DataRow r = dt_传.Rows[0];


                    sql = string.Format(@"select 生产记录生产检验单主表.生产检验单号,(生产记录生产检验单主表.负责人员) as 生产者,(人事基础员工表.岗位) as 班组,人事基础员工表.部门,(生产记录生产检验单主表.检验日期) as 生产日期
                          ,合格数量  from 生产记录生产检验单主表 
                    left join 人事基础员工表 on 生产记录生产检验单主表.负责人员ID = 人事基础员工表.员工号 where 生产记录生产检验单主表.物料编码 = '{0}' order by 生产记录生产检验单主表.检验日期 desc", txt_物料编码.Text);
                    da = new SqlDataAdapter(sql, strconn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    try
                    {
                        r["生产者"] = dt.Rows[0]["生产者"];
                        r["班组"] = dt.Rows[0]["班组"];
                        r["部门"] = dt.Rows[0]["部门"];
                        r["生产日期"] = dt.Rows[0]["生产日期"];
                    }
                    catch
                    {
                        r["生产者"] = "无";
                        r["班组"] = "无";
                        r["部门"] = "无";
                        r["生产日期"] = System.DateTime.Now;
                    }
                    r["发货数量"] = "-";
                    r["合格数"] = dt.Rows[0]["合格数量"];

                    PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                    this.printDialog1.Document = this.printDocument1;
                    DialogResult dr = this.printDialog1.ShowDialog();
                    if (dr == DialogResult.OK)
                    {

                        string PrinterName = this.printDocument1.PrinterSettings.PrinterName;

                        //SetDefaultPrinter(PrinterName);
                        ItemInspection.print_FMS.fun_print_出厂检验报告(dr_产品序序号,dt_传, dt_检验明细, 1, PrinterName);

                    }
                }
         
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
          
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //关闭
            CPublic.UIcontrol.ClosePage();
        }
        #endregion

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (MessageBox.Show(string.Format("确认修改信息？"), "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                string sql="select * from 成品检验检验记录明细表 where  1<>1";

                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt_检验明细);
                    MessageBox.Show("修改完成");
                }

            }
        }
        //保存
#pragma warning disable IDE1006 // 命名样式
        private void simpleButton2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }
        //  输入不合格数量
#pragma warning disable IDE1006 // 命名样式
        private void txt_报废数_TextChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //打印
                if (MessageBox.Show("是否打印出厂检验记录？", "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    //序号选中行传值
                    DataRow dr_产品序序号 = gridView2.GetDataRow(gridView2.FocusedRowHandle);

                    string sql = string.Format("select 物料编码,物料名称,规格型号 as 规格,大类,小类 from 基础数据物料信息表 where 物料编码 = '{0}'", txt_物料编码.Text);
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_传 = new DataTable();

                    da.Fill(dt_传);
                    dt_传.Columns.Add("发货数量");
                    dt_传.Columns.Add("合格数");
                    dt_传.Columns.Add("生产者");
                    dt_传.Columns.Add("班组");
                    dt_传.Columns.Add("部门");
                    dt_传.Columns.Add("生产日期");

                    DataRow r = dt_传.Rows[0];


                    sql = string.Format(@"select 生产记录生产检验单主表.生产检验单号,(生产记录生产检验单主表.负责人员) as 生产者,(人事基础员工表.岗位) as 班组,人事基础员工表.部门,(生产记录生产检验单主表.检验日期) as 生产日期
                          ,合格数量  from 生产记录生产检验单主表 
                    left join 人事基础员工表 on 生产记录生产检验单主表.负责人员ID = 人事基础员工表.员工号 where 生产记录生产检验单主表.物料编码 = '{0}' order by 生产记录生产检验单主表.检验日期 desc", txt_物料编码.Text);
                    da = new SqlDataAdapter(sql, strconn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    try
                    {
                        r["生产者"] = dt.Rows[0]["生产者"];
                        r["班组"] = dt.Rows[0]["班组"];
                        r["部门"] = dt.Rows[0]["部门"];
                        r["生产日期"] = dt.Rows[0]["生产日期"];
                    }
                    catch
                    {
                        r["生产者"] = "无";
                        r["班组"] = "无";
                        r["部门"] = "无";
                        r["生产日期"] = System.DateTime.Now;
                    }
                    r["发货数量"] = "-";
                    r["合格数"] = dt.Rows[0]["合格数量"];



                    ERPreport.出厂检验报告 form = new ERPreport.出厂检验报告(dr_产品序序号, dt_传, dt_检验明细);
                   form.ShowDialog();  
                  //  ItemInspection.print_FMS.fun_print_出厂检验报告(dr_产品序序号, dt_传, dt_检验明细, 1);

                   
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
