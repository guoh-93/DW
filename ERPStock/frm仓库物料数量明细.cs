using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
namespace ERPStock
{
    public partial class frm仓库物料数量明细 : UserControl
    {

        #region 变量
        DataTable dt = new DataTable();
        string strconn = CPublic.Var.strConn;
        /// <summary>
        /// 物料编码
        /// </summary>
        string  s=null;
        string s_stock_id=null;
        DataRow r;
        #endregion

        #region  加载
        public frm仓库物料数量明细()
        {
            InitializeComponent();
            if (s == null)
            {
                label18.Visible = true;
                timer1.Start();
            }
        }

        //物料编码 不是 原ERp物料编号
        public frm仓库物料数量明细(string s,string s_stockid)
        {
            this.s = s;
            s_stock_id = s_stockid;
            InitializeComponent();
            timer1.Stop();
        }

        private void frm仓库物料数量明细_Load(object sender, EventArgs e)
        {
            if (s == null)
                return;
            fun_load();
            fun_xtraload();
            //CZMaster.DevGridControlHelper.Helper(this);
           
        }

        #endregion

        #region  函数

        private void fun_load()
        {

            try
            {
                DateTime dtime1 = CPublic.Var.getDatetime().Date;
               // dtime1 = new DateTime(dtime1.Year, dtime1.Month,1);
                //dtime1 =dtime1.AddMonths(-3);
                DateTime dtime2 = dtime1.AddMonths(-3);  //三个月前
                DateTime dtime3 = dtime1.AddMonths(-1);  //一个月前
                DateTime dtime4 = dtime1.AddMonths(-6);  //半年前


                //2020-3-31       
                string s_补条件 = "and 明细类型 not in('借用出库','拆单申请出库','形态转换出库')";

                string sql1 = string.Format($@"select kc.*,基础数据物料信息表.物料名称 as  名称 ,基础数据物料信息表.图纸编号 as 图纸编号1,n原ERP规格型号,规格,kc.仓库名称,kc.货架描述,a.季度用量,b.月度用量,半年用量 from 仓库物料数量表 kc left join
                                            (select 物料编码,-sum(实效数量)as 季度用量  from 仓库出入库明细表 where  出库入库='出库'{s_补条件} and  出入库时间>'{dtime2}' and 出入库时间<'{dtime1}' group by 物料编码) a
                                          on   kc.物料编码=a.物料编码
                                          left join  (select 物料编码,-sum(实效数量)as 月度用量  from 仓库出入库明细表 where  出库入库='出库' {s_补条件} and  出入库时间>'{dtime3}' and  出入库时间<'{dtime1}' group by 物料编码) b
                                          on   kc.物料编码=b.物料编码
                                            left join  (select 物料编码,-sum(实效数量)as 半年用量  from 仓库出入库明细表 where  出库入库='出库' {s_补条件} and  出入库时间>'{dtime4}' and  出入库时间<'{dtime1}' group by 物料编码)c
                                          on   kc.物料编码=c.物料编码
                                            left join 基础数据物料信息表 on 基础数据物料信息表.物料编码=kc.物料编码
                                        where kc.物料编码='{s}' and kc.仓库号='{s_stock_id}'  ");
                using (SqlDataAdapter da_1 = new SqlDataAdapter(sql1, strconn))
                {
                    DataTable dt_1 = new DataTable();
                    da_1.Fill(dt_1);
                    
                    if (dt_1.Rows.Count > 0)
                    {
                        r = dt_1.Rows[0];
                        dataBindHelper1.DataFormDR(r);
                        //textBox8.Text = r["图纸编号"].ToString();
                        //textBox5.Text = r["规格型号"].ToString();
                        //textBox1.Text = r["物料编码"].ToString();
                        //textBox4.Text = r["物料名称"].ToString();
                        //textBox6.Text = r["在途量"].ToString();
                        //textBox3.Text = r["在制量"].ToString();
                        //textBox9.Text = r["受订量"].ToString();
                        //textBox11.Text = r["未领量"].ToString();
                        //textBox12.Text = r["库存总数"].ToString();
                        //textBox13.Text = r["有效总数"].ToString();
                        //textBox14.Text = r["MRP计划采购量"].ToString();
                        //textBox15.Text = r["MRP计划生产量"].ToString();
                        //textBox16.Text = r["MRP库存锁定量"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("未找到数据");
                    }

                   
                }
                //string sql = string.Format("select * from 仓库物料表 where 物料编码='{0}' ", s);
                //using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                //{
                //    da.Fill(dt);
                //    if (dt.Rows.Count > 0)
                //    {
                //        textBox7.Text = dt.Rows[0]["仓库名称"].ToString();
                //        textBox10.Text = dt.Rows[0]["库位号"].ToString();
                //        textBox2.Text = dt.Rows[0]["仓库描述"].ToString();
                //    }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
      
        void fun_xtraload()
        {
            
            frm仓库出入库 frm = new frm仓库出入库(s);
            xtraTabPage5.Controls.Add(frm);
            frm.Dock = DockStyle.Fill;
            

            UI在途量 UI_在途量 = new UI在途量(s);
            xtraTabPage1.Controls.Add(UI_在途量);
            UI_在途量.Dock = DockStyle.Fill;


            UI在制量 UI_在制量 = new UI在制量(s);
            xtraTabPage2.Controls.Add(UI_在制量);
            UI_在制量.Dock = DockStyle.Fill;

            UI受订量 UI_受订量 = new UI受订量(s);
            xtraTabPage3.Controls.Add(UI_受订量);
            UI_受订量.Dock = DockStyle.Fill;

            UI未领量 UI_未领量 = new UI未领量(s);
            xtraTabPage4.Controls.Add(UI_未领量);
            UI_未领量.Dock = DockStyle.Fill;

            //UI虚拟库存明细 UI_车间虚拟库存 = new UI虚拟库存明细(s);
            //xtraTabPage6.Controls.Add(UI_车间虚拟库存);
            //UI_车间虚拟库存.Dock = DockStyle.Fill; 
            Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, "BaseDataItem.dll"));//dr["dll全路径"] = "动态载入dll.dll"
            Type outerForm = outerAsm.GetType("BaseData.ui版本维护", false);//动态载入dll.UI动态载入窗体

            object[] drr = new object[2];
            string xx = string.Format("select  物料编码,规格型号,物料名称 from 基础数据物料信息表 where 物料编码='{0}'", s);
            DataTable t = CZMaster.MasterSQL.Get_DataTable(xx, strconn);
            drr[0] = t.Rows[0];
            drr[1] = 0;
            UserControl ui = Activator.CreateInstance(outerForm,drr) as UserControl;
       
            if (!(ui == null))
            {
                //DevExpress.XtraTab.XtraTabPage xtp =  xtraTabControl1.TabPages.Add("程序版本");
                //xtp.Controls.Add(ui);
                xtraTabPage6.Controls.Add(ui);
                ui.AllowDrop = true;
                xtraTabPage6.AllowDrop = true;
                ui.Dock = DockStyle.Fill;
            }

        }

        #endregion

        #region 界面操作

        //刷新
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ERPorg.Corg.FlushMemory();
            fun_load();
            fun_xtraload();
            //CZMaster.DevGridControlHelper.Helper(this);

        }
        //关闭
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox18.Focus();
        }

        private void textBox18_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
               //这里扫出来为 六位的编号 
                string sql = string.Format("select 物料编码 from 基础数据物料信息表 where 原ERP物料编号='{0}'", textBox18.Text);
                using (SqlDataAdapter da = new SqlDataAdapter(sql,strconn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        this.s = dt.Rows[0][0].ToString();
                    }
                }
                textBox18.Text = "";
                barLargeButtonItem2_ItemClick(null,null);
            
            }
        }
      






    }
}
