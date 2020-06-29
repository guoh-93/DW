using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;
namespace IAACA
{
    public partial class 存货核算 : UserControl
    {
        DataTable dt_存货核算;
        //工单成本
        DataTable dt_工单;
        DataTable dt_工单耗用;

        bool bl_calculate = false;
        string strcon = CPublic.Var.strConn;

        public 存货核算()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                ERPorg.Corg.FlushMemory();
                bool bl_c = true;
                string s= fun_check();

                if (s.Trim() != "")
                {
                    if (MessageBox.Show($"{s}是否要导出清单", "询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.Title = "导出Excel";
                        saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                        DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                        if (dialogResult == DialogResult.OK)
                        {
                            int y= Convert.ToInt32(textBox1.Text);
                            int m = Convert.ToInt32(textBox2.Text);
                            DateTime t1 = new DateTime(y, m, 1);
                            DateTime t2 = t1.AddMonths(1);

                            //判断 工时软件是否都已经赋值
                            s = $@"select  aa.* from (select a.物料编码,b.规格型号,b.物料名称 from 生产记录成品入库单明细表 a
                     left join 基础数据物料信息表 b on a.物料编码 = b.物料编码 where a.生效日期>'{t1}' and  a.生效日期<'{t2}'  group by a.物料编码,b.规格型号,b.物料名称)aa
                     left join[2019财务工时] c on c.产品编码 = aa.物料编码
                     where left(物料编码,2)= 10  and c.工时 is null ";
                          DataTable    tt_工时 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                            tt_工时.TableName = "工时";

                            s = $@"select aa.* from (select a.物料编码,b.规格型号,b.物料名称  from 生产记录成品入库单明细表  a
                left join 基础数据物料信息表 b on a.物料编码=b.物料编码 where   b.大类 not in ('展架类') and  a.生效日期>'{t1}' and  a.生效日期<'{t2}' group by a.物料编码,b.规格型号,b.物料名称)aa 
                left join [2019财务软件费用] c on c.产品编码 =aa.物料编码 
                where left(物料编码,2)=10  and c.单价 is null ";
                            DataTable tt_软件= CZMaster.MasterSQL.Get_DataTable(s, strcon);
                            tt_软件.TableName = "软件";

                            //判断 当月委外入库是否都已经核销
                            s = $@"select a.入库单号,a.采购单号,a.物料编码,a.物料名称,a.规格型号 ,入库量,a.生效日期 as 入库日期 from 采购记录采购单入库明细  a
                left join 采购记录采购单主表 b on a.采购单号 = b.采购单号
                where a.生效日期 > '{t1}'  and a.生效日期 < '{t2}' and 采购单类型 = '委外采购' and 委外核销 = 0";
                            DataTable tt_委外 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                            tt_委外.TableName = "委外";
                            DataSet ds = new DataSet();
                            if(tt_工时.Rows.Count>0)   ds.Tables.Add(tt_工时);
                            if (tt_软件.Rows.Count > 0) ds.Tables.Add(tt_软件);
                            if (tt_委外.Rows.Count > 0) ds.Tables.Add(tt_委外);
                            ERPorg.Corg.TableToExcel(ds, saveFileDialog.FileName);
                   
                           // throw new Exception("导出成功");
                        }
                        //throw new Exception("不可继续结算");
                    }
                    else
                    {
                        //throw new Exception("不可继续结算");
                        //20-4--2 财务李毅又说这里不要限制,到分摊再限制

                    }
                    if (MessageBox.Show("是否要继续计算？", "询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                    {
                        bl_c = false;
                    }
                }
                if (bl_c)
                {
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    int year = Convert.ToInt32(textBox1.Text);
                    int month = Convert.ToInt32(textBox2.Text);
                    if (bl_calculate) MessageBox.Show("正在计算中,请稍候");
                    else
                    {
                        Thread th = new Thread(() =>
                        {
                            BeginInvoke(new MethodInvoker(() =>
                            {
                                simpleButton1.Text = "计算中";
                            }));
                            DateTime t1 = new DateTime(year, month, 1); //结算月 初 
                                                                        // DateTime t2 = new DateTime(2019, 8, 1); //结算月 末
                        DateTime t2 = t1.AddMonths(1); //结算月 末

                        IAACA.IA ia = new IAACA.IA();
                            dt_存货核算 = ia.Cal_inv(t1, t2);
                            BeginInvoke(new MethodInvoker(() =>
                            {
                                simpleButton1.Text = "计算完成";
                                bl_calculate = false;
                            }));
                        });
                        th.IsBackground = true;
                        th.Start();
                        bl_calculate = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //辅材 制造费用 人工 为0  先算 算出后 财务计算出 这三个金额 
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                int year = Convert.ToInt32(textBox1.Text);
                int month = Convert.ToInt32(textBox2.Text);
                if (bl_calculate) MessageBox.Show("正在计算中,请稍候");
                else
                {
                    Thread th = new Thread(() =>
                    {
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            simpleButton2.Text = "计算中";
                        }));
                        DateTime t1 = new DateTime(year, month, 1); //结算月 初 
                        // DateTime t2 = new DateTime(2019, 8, 1); //结算月 末
                        DateTime t2 = t1.AddMonths(1); //结算月 末
                        DateTime tx = t1.AddMonths(-1);
                        string s = string.Format(" select  * from 仓库月出入库结转表 where 年='{0}' and 月='{1}'", tx.Year, tx.Month);
                        DataTable t_结转表 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                        DataColumn[] pk_jz_cb = new DataColumn[1];
                        pk_jz_cb[0] = t_结转表.Columns["物料编码"];
                        t_结转表.PrimaryKey = pk_jz_cb;

                        //19-11-6 为解决 已经 跑过 第3布之后 需要从第2步重新算的问题 已在第一步计算将结果备份至 C_存货核算物料单价表_bak 表 
                        //每次计算 第2布 第三步 都从 C_存货核算物料单价表_bak 表中取数据
                        s = string.Format(@"select a.*,物料名称,规格型号,存货分类,存货分类编码 from C_存货核算物料单价表_bak a
                             left join 基础数据物料信息表 base on a.物料编码 = base.物料编码
                             where 年 = '{0}' and 月 = '{1}'", t1.Year, t1.Month);
                        DataTable t_save = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                        DataColumn[] pk_t_save = new DataColumn[1];
                        pk_t_save[0] = t_save.Columns["物料编码"];
                        t_save.PrimaryKey = pk_t_save;

                        DataSet ds = new DataSet();
                        ds.Tables.Add(t_save);
                        ds.Tables.Add(t_结转表);
                        IAACA.IA ia = new IAACA.IA();
                        decimal dec_sum辅材 = 0;
                        decimal dec_sum制造 = 0;
                        decimal dec_sum人工 = 0;
                        DataSet ds_return = ia.Cal_成本(ds, t1, t2, dec_sum辅材, dec_sum制造, dec_sum人工);
                        dt_工单 = ds_return.Tables[0];
                        dt_工单耗用 = ds_return.Tables[1];
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            simpleButton2.Text = "计算完成";
                            bl_calculate = false;
                        }));
                    });
                    th.IsBackground = true;
                    th.Start();
                    bl_calculate = true;
                }
            }
            catch (Exception ex)
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    label2.Text = "错误信息" + ex.Message;
                    label2.Visible = true;

                }));
                bl_calculate = false;
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            try
            {



                int year = Convert.ToInt32(textBox1.Text);
                int month = Convert.ToInt32(textBox2.Text);

                DateTime t1 = new DateTime(year, month, 1); //结算月 初 
                                                            // DateTime t2 = new DateTime(2019, 8, 1); //结算月 末
                DateTime t2 = t1.AddMonths(1); //结算月 末
                DateTime tx = t1.AddMonths(-1);


                string s = fun_check();

                if (s.Trim() != "")
                {
                    if (MessageBox.Show($"{s}是否要导出清单", "询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.Title = "导出Excel";
                        saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                        DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                        if (dialogResult == DialogResult.OK)
                        {
 
                            //判断 工时软件是否都已经赋值
                            s = $@"select  aa.* from (select a.物料编码,b.规格型号,b.物料名称 from 生产记录成品入库单明细表 a
                     left join 基础数据物料信息表 b on a.物料编码 = b.物料编码 where a.生效日期>'{t1}' and  a.生效日期<'{t2}'  group by a.物料编码,b.规格型号,b.物料名称)aa
                     left join[2019财务工时] c on c.产品编码 = aa.物料编码
                     where left(物料编码,2)= 10  and c.工时 is null ";
                            DataTable tt_工时 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                            tt_工时.TableName = "工时";

                            s = $@"select aa.* from (select a.物料编码,b.规格型号,b.物料名称  from 生产记录成品入库单明细表  a
                left join 基础数据物料信息表 b on a.物料编码=b.物料编码 where   b.大类 not in ('展架类') and  a.生效日期>'{t1}' and  a.生效日期<'{t2}' group by a.物料编码,b.规格型号,b.物料名称)aa 
                left join [2019财务软件费用] c on c.产品编码 =aa.物料编码 
                where left(物料编码,2)=10  and c.单价 is null ";
                            DataTable tt_软件 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                            tt_软件.TableName = "软件";

                            //判断 当月委外入库是否都已经核销
                            s = $@"select a.入库单号,a.采购单号,a.物料编码,a.物料名称,a.规格型号 ,入库量,a.生效日期 as 入库日期 from 采购记录采购单入库明细  a
                left join 采购记录采购单主表 b on a.采购单号 = b.采购单号
                where a.生效日期 > '{t1}'  and a.生效日期 < '{t2}' and 采购单类型 = '委外采购' and 委外核销 = 0";
                            DataTable tt_委外 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                            tt_委外.TableName = "委外";
                            DataSet ds = new DataSet();
                            if (tt_工时.Rows.Count > 0) ds.Tables.Add(tt_工时);
                            if (tt_软件.Rows.Count > 0) ds.Tables.Add(tt_软件);
                            if (tt_委外.Rows.Count > 0) ds.Tables.Add(tt_委外);
                            ERPorg.Corg.TableToExcel(ds, saveFileDialog.FileName);

                            throw new Exception("导出成功");
                        }
                         throw new Exception("不可继续结算");
                    }
                    else
                    {
                        throw new Exception("不可继续结算");
 
                    }
                  
                }



                if (bl_calculate) MessageBox.Show("正在计算中,请稍候");
                else
                {
                    Thread th = new Thread(() =>
                    {
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            simpleButton3.Text = "计算中";
                        }));

                       s = string.Format(" select  * from 仓库月出入库结转表 where 年='{0}' and 月='{1}'", tx.Year, tx.Month);
                        DataTable t_结转表 = CZMaster.MasterSQL.Get_DataTable(s, strcon);

                        DataColumn[] pk_jz_cb = new DataColumn[1];
                        pk_jz_cb[0] = t_结转表.Columns["物料编码"];
                        t_结转表.PrimaryKey = pk_jz_cb;

                        //19-11-6 为解决 已经 跑过 第3布之后 需要从第2步重新算的问题 已在第一步计算将结果备份至 C_存货核算物料单价表_bak 表 
                        //每次计算 第2布 第三步 都从 C_存货核算物料单价表_bak 表中取数据
                        s = string.Format(@"select a.*,物料名称,规格型号,存货分类,存货分类编码 from C_存货核算物料单价表_bak a
                             left join 基础数据物料信息表 base on a.物料编码 = base.物料编码
                             where 年 = '{0}' and 月 = '{1}'", t1.Year, t1.Month);
                        DataTable t_save = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                        DataColumn[] pk_t_save = new DataColumn[1];
                        pk_t_save[0] = t_save.Columns["物料编码"];
                        t_save.PrimaryKey = pk_t_save;
                        DataSet ds = new DataSet();
                        ds.Tables.Add(t_save);
                        ds.Tables.Add(t_结转表);
                        IAACA.IA ia = new IAACA.IA();
                        decimal dec_sum辅材 = Convert.ToDecimal(textBox5.Text);
                        decimal dec_sum制造 = Convert.ToDecimal(textBox4.Text);
                        decimal dec_sum人工 = Convert.ToDecimal(textBox3.Text);
                        DataSet ds_return = ia.Cal_成本(ds, t1, t2, dec_sum辅材, dec_sum制造, dec_sum人工);
                        dt_工单 = ds_return.Tables[0];
                        dt_工单耗用 = ds_return.Tables[1];
                        ia.fun_保存过程数量(ds_return, t1, t2);
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            simpleButton3.Text = "计算完成";
                            bl_calculate = false;
                        }));
                    });
                    th.IsBackground = true;
                    th.Start();
                    bl_calculate = true;
                }
            }
            catch (Exception)
            {
                simpleButton3.Text = "成本--计算出错";
                bl_calculate = false;
            }
        }
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ERPorg.Corg.FlushMemory();
                if (bl_calculate) throw new Exception("正在计算中,请稍候");
                CPublic.UIcontrol.ClosePage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        private string fun_check()
        {
            string log = "";
            int year = Convert.ToInt32(textBox1.Text);
            int month = Convert.ToInt32(textBox2.Text);
            DateTime t1 = new DateTime(year,month,1);
            DateTime t2 = t1.AddMonths(1);
            string s = string.Format("select  count(*)xx from 仓库月出入库结转表 where  年={0} and 月={1}", year, month);
            DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            if (Convert.ToDecimal(temp.Rows[0]["xx"]) > 0)
            {
                //throw new Exception("该月已结转,请确认");
                log = "该月已结转,请确认\r\n";
            }
            //判断 工时软件是否都已经赋值
            s = $@"select COUNT(*)x  from (select a.物料编码,b.规格型号,b.物料名称 from 生产记录成品入库单明细表 a
                  left join 基础数据物料信息表 b on a.物料编码 = b.物料编码 where a.生效日期>'{t1}' and  a.生效日期<'{t2}' group by a.物料编码,b.规格型号,b.物料名称)aa
                  left join[2019财务工时] c on c.产品编码 = aa.物料编码
                  where left(物料编码,2)= 10  and c.工时 is null ";
            temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            if (Convert.ToInt32(temp.Rows[0][0]) > 0) log += "工时尚有未维护的\r\n";
            s = $@"select COUNT(*)x from (select a.物料编码,b.规格型号,b.物料名称  from 生产记录成品入库单明细表  a
                left join 基础数据物料信息表 b on a.物料编码=b.物料编码 where   b.大类 not in ('展架类') and   a.生效日期>'{t1}' and  a.生效日期<'{t2}'  group by a.物料编码,b.规格型号,b.物料名称)aa 
                left join [2019财务软件费用] c on c.产品编码 =aa.物料编码 
                where left(物料编码,2)=10  and c.单价 is null ";
            temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            if (Convert.ToInt32(temp.Rows[0][0]) > 0) log += "软件单价尚有未维护的\r\n";
            //判断 当月委外入库是否都已经核销
            s = $@" select count(*)x from 采购记录采购单入库明细  a
                left join 采购记录采购单主表 b on a.采购单号 = b.采购单号
                where a.生效日期 > '{t1}'  and a.生效日期 < '{t2}'
                and 采购单类型 = '委外采购' and 委外核销 = 0";
            temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            if (Convert.ToInt32(temp.Rows[0][0]) > 0) log += "当月委外入库尚有未核销的\r\n";
            return log;


        }
        private void simpleButton4_Click(object sender, EventArgs e)
        {
            //@Month_N int,
            //@Month_O int,
            //@year_N int,
            //@year_O int,
            //@time1 nvarchar(50),
            //@time2 nvarchar(50) 
            try
            {
                int year = Convert.ToInt32(textBox1.Text);
                int month = Convert.ToInt32(textBox2.Text);
                DateTime t1 = new DateTime(year, month, 1); //结算月 初 
                // DateTime t2 = new DateTime(2019, 8, 1); //结算月 末
                DateTime t2 = t1.AddMonths(1); //结算月 末
                DateTime tx = t1.AddMonths(-1);
                string s = string.Format("exec ICC_Apport_Price  {0},{1},{2},{3},'{4}','{5}'", month, tx.Month, year, tx.Year, t1, t2);
                CZMaster.MasterSQL.ExecuteSQL(s, strcon);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 存货核算_Load(object sender, EventArgs e)
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();
                textBox1.Text = t.Year.ToString();
                textBox2.Text = t.Month.ToString();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
        private void simpleButton5_Click(object sender, EventArgs e)
        {
            try
            {
                string s = "select  top 1 * from C_存货核算物料单价表 order by CONVERT( nvarchar(50),年)+ right('0'+CONVERT(nvarchar(50),月),2) desc";
                DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                int year = Convert.ToInt32(t.Rows[0]["年"]);
                int mon = Convert.ToInt32(t.Rows[0]["月"]);

                if (year != Convert.ToInt32(textBox1.Text) || mon != Convert.ToInt32(textBox2.Text))
                {
                    throw new Exception("年月选择不正确");
                }
                s = $"select  count(*) from 仓库月出入库结转表 where 年={year} and 月={mon}";
                DataTable tt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                if (Convert.ToInt32(tt.Rows[0][0]) > 0) throw new Exception("当月已结转不可进行此操作");
                if (MessageBox.Show($"是否确认重置{year}年{mon}月计算赋值的单价？", "询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    DateTime t1 = new DateTime(year, mon, 1);
                    DateTime t2 = t1.AddMonths(1);
                    string ss = $"exec[ResetPrice] {mon},{year},'{t1}','{t2}'";
                    CZMaster.MasterSQL.ExecuteSQL(ss, strcon);
                    MessageBox.Show("重置成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
