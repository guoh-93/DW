using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;

using System.Text;
//using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
using System.Text.RegularExpressions;


namespace MoldMangement
{
    public partial class UI台账 : UserControl
    {
        //static string strConn = "Password = a; Persist Security Info = True; User ID = sa; Initial Catalog = fms; Data Source=XINREN";
         string strConn = CPublic.Var.strConn;
        
        DataTable dt = new DataTable();
        //DataTable dt2 = new DataTable();
        //DataRow dr;
        DataTable dt_部门 = new DataTable();
        DataTable dt_员工 = new DataTable();
        /// <summary>
        /// true为新增状态
        /// </summary>
        bool bl = false;
        /// <summary>
        /// true为修改状态
        /// </summary>
        bool bl_修改 = true;
        /// <summary>
        /// true为保存成功状态
        /// </summary>
        bool bl_保存成功 = true;

        Regex rgx = new Regex(@"^[0-9]*[1-9][0-9]*$");
        Regex rgx2 = new Regex(@"(^[0-9]*[1-9][0-9]*$)|(^([0-9]{1,}[.][0-9]*)$)");

       

        public UI台账()
        {
            InitializeComponent();
            

        }

        private void fun_load()
        {
            string sql = "select * from 计量器具基础信息表 ";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
            {
                dt = new DataTable();
                da.Fill(dt);

            }
            gc1.DataSource = dt;
            
            fun_co();
            textBox8.Text = CPublic.Var.localUserName;
            dt.Columns.Add("选择", typeof(bool));
        }


        private void UI台账_Load(object sender, EventArgs e)
        {
            fun_load();
            
            bl = false;
            bl_修改 = false;
            sc = false;
            dateEdit2.Text = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");
            //comboBox4.SelectedIndex = -1;
        }

        //下拉部门
        //private void fun_xl()
        //{
        //    string sql3 = "select 部门编号,部门名称 from 人事基础部门表 order by 部门编号 asc";
        //    using (SqlDataAdapter da = new SqlDataAdapter(sql3, strConn))
        //    {
        //        dt_部门 = new DataTable();
        //        da.Fill(dt_部门);

        //    }
        //    //searchLookUpEdit1.Properties.DataSource = dt_部门;
        //    //searchLookUpEdit1.Properties.ValueMember = "部门名称";
        //    //searchLookUpEdit1.Properties.DisplayMember = "部门名称";

        //    string sql4 = "select 员工号,姓名,部门 from 人事基础员工表";
        //    using (SqlDataAdapter da = new SqlDataAdapter(sql4, strConn))
        //    {
        //        dt_员工 = new DataTable();
        //        da.Fill(dt_员工);

        //    }
        //    searchLookUpEdit2.Properties.DataSource = dt_员工;
        //    searchLookUpEdit2.Properties.ValueMember = "姓名";
        //    searchLookUpEdit2.Properties.DisplayMember = "姓名";

        //}
        private void fun_co()
        {
            string sql3 = "select 部门编号,部门名称 from 人事基础部门表 order by 部门编号 asc";
            using (SqlDataAdapter da = new SqlDataAdapter(sql3, strConn))
            {
                dt_部门 = new DataTable();
                da.Fill(dt_部门);

            }

            foreach (DataRow dr in dt_部门.Rows)
            {
                if (dr["部门名称"].ToString() == "苏州未来电器股份有限公司" || dr["部门名称"].ToString() == "总经理" || dr["部门名称"].ToString() == "营销副总" || dr["部门名称"].ToString() == "开发副总" || dr["部门名称"].ToString() == "制造副总" || dr["部门名称"].ToString() == "财务总监")
                {
                   
                }
                else
                {
                    comboBoxEdit1.Properties.Items.Add(dr["部门名称"]);
                }
                             
            }
       
          
            string sql4 = "select 员工号,姓名,部门 from 人事基础员工表";
            using (SqlDataAdapter da = new SqlDataAdapter(sql4, strConn))
            {
                dt_员工 = new DataTable();
                da.Fill(dt_员工);

            }
            searchLookUpEdit2.Properties.DataSource = dt_员工;
            searchLookUpEdit2.Properties.ValueMember = "姓名";
            searchLookUpEdit2.Properties.DisplayMember = "姓名";
            
        }

        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_load();
            fun_清空内容();
            fun_可编辑();
            bl = false;
            sc = false;
            bl_修改 = false;
        }

        //新增
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            textBox1.Enabled = true;
            textBox3.Enabled = true;
            textBox11.Enabled = true;
            textBox12.Enabled = true;
            comboBox1.Enabled = true;
            //comboBox5.Enabled = true;
            //searchLookUpEdit1.Enabled = true;
            comboBoxEdit1.Enabled = true;
            dateEdit1.Enabled = true;
            barLargeButtonItem1_ItemClick(null, null);
            label23.Visible = true;
            bl = true;
        }
        //保存
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //DataRow[] drr = dt.Select(string.Format("计量器具编号 = '{0}'", textBox1.Text));
            //if (drr.Length > 0)
            //{
            //    MessageBox.Show("计量器具编号：" + textBox1.Text + "已存在,不可以再新增");

            //    return;

            //}
            //else
            //{

            //if (textBox1.Text != null && textBox1.Text != "" && textBox2.Text != null && textBox2.Text != "")
            //{
            //try
            //{
                if (sc ==false && bl ==false && bl_修改==false )
                {
                    MessageBox.Show("操作错误，请点击按钮新增数据或修改数据再保存");
                }
                else  if (sc)
                { fun_删除保存(); 
                }
                else if (bl)
                {
                    check();
                    
                    DataRow[] drr = dt.Select(string.Format("计量器具编号 = '{0}'", textBox1.Text));
                    if (drr.Length > 0)
                    {
                        MessageBox.Show("计量器具编号：" + textBox1.Text + "已存在,不可以再新增");

                        return;

                    }
                    DataRow dr;
                    dr = dt.NewRow();
                    dt.Rows.Add(dr);
                    if (dateEdit3.Text == "")
                    {
                        dr["出厂日期"] = DBNull.Value;
                    }
                    else
                    {
                        dr["出厂日期"] = dateEdit3.Text;
                    }
                    if (dateEdit4.Text == "")
                    {
                        dr["购置日期"] = DBNull.Value;
                    }
                    else
                    {
                        dr["购置日期"] = dateEdit4.Text;
                    }
                    if (dateEdit5.Text == "")
                    {
                        dr["领用日期"] = DBNull.Value;
                    }
                    else
                    {
                        dr["领用日期"] = dateEdit5.Text;
                    }

                    dataBindHelper1.DataToDR(dr);
                   
                  
                    
                    string sql = "select * from 计量器具基础信息表 where 1<>1";
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
                    {

                        new SqlCommandBuilder(da);
                        da.Update(dt);


                    }
                    if (bl_保存成功)
                    {
                        MessageBox.Show("保存成功");
                        barLargeButtonItem1_ItemClick(null, null);
                    }
                    else
                    {
                        MessageBox.Show("保存失败");
                        return;

                    }




                }

                else if (bl_修改)
                {
                    fun_修改保存();

                }

            //}
            //catch (Exception ex)
            //{

            //    MessageBox.Show(ex.Message);
                
                

            //}
            //}
            //else
            //{
            //    MessageBox.Show("计量编号或名称不能为空");
            //}

            //}
        }
        private void fun_修改保存()
        {
           
            string sql_修改 = "select * from 计量器具基础信息表 where 1<>1";

            SqlConnection conn = new SqlConnection(strConn);
           
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("初始化");      //开始事务
            try
            {
                check();
                DataTable dt_修改 = update();
         

                SqlCommand cm_修改 = new SqlCommand(sql_修改, conn, ts);

                SqlDataAdapter da_修改 = new SqlDataAdapter(cm_修改);

                new SqlCommandBuilder(da_修改);

                try
                {
                    da_修改.Update(dt_修改);

                }

                catch
                {

                }

                ts.Commit();              //事务完成之后提交事务
                
                MessageBox.Show("修改成功");
                fun_sx();
            }
            catch (Exception ex)
            {
                ts.Rollback();           //如果没有完成事务就回滚事务
                MessageBox.Show(ex.Message);
            }
            //barLargeButtonItem1_ItemClick(null, null);
          
           
            //DataRow dr_刷修;
            //gv1.RefreshRow(gv1.FocusedRowHandle);

        }

        private void fun_sx()
        {
            fun_dh();
            fun_清空内容();
            fun_可编辑();
            bl = false;
            sc = false;
            bl_修改 = false;
        
        }

        private void fun_dh()
        {
            DataTable dt_修刷 = new DataTable();
            string sql = string.Format("select * from 计量器具基础信息表 where 计量器具编号='{0}'", textBox1.Text);
            //string sql = "select * from 计量器具基础信息表  ";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
            {
                dt_修刷 = new DataTable();
                da.Fill(dt_修刷);

            }
                               
            DataRow [] a = dt.Select(string.Format("计量器具编号='{0}'",textBox1.Text));
            a[0].ItemArray = dt_修刷.Rows[0].ItemArray;

            fun_co();
            textBox8.Text = CPublic.Var.localUserName;
        }

        private DataTable update()
        {
            check();
            DataTable dt_修改 = new DataTable();
            //DataRow[] drr = dt.Select(string.Format("select * from 计量器具基础信息表 where 计量器具编号='{0}'", textBox1.Text));
            //dataBindHelper1.DataToDR(drr);
            string sql_修改 = string.Format("select * from 计量器具基础信息表 where 计量器具编号='{0}'", textBox1.Text);
            using (SqlDataAdapter da = new SqlDataAdapter(sql_修改, strConn))
            {
                da.Fill(dt_修改);
            }
            dt_修改.Rows[0]["计量器具名称"] = textBox2.Text;
            dt_修改.Rows[0]["出厂编号"] = textBox4.Text;
            dt_修改.Rows[0]["制造单位"] = textBox5.Text;
            dt_修改.Rows[0]["所属大类"] = comboBox1.Text;
            dt_修改.Rows[0]["出厂编号"] = textBox4.Text;
            dt_修改.Rows[0]["证书号"] = textBox6.Text;
            //dt_修改.Rows[0]["精度"] = textBox7.Text;
            //dt_修改.Rows[0]["准用证号"] = textBox9.Text;
            dt_修改.Rows[0]["检定标准"] = textBox10.Text;
            dt_修改.Rows[0]["状态"] = comboBox2.Text;
            dt_修改.Rows[0]["管理级别"] = comboBox3.Text;
            dt_修改.Rows[0]["使用人"] = searchLookUpEdit2.Text;
            dt_修改.Rows[0]["有效期"] = dateEdit1.Text;
            dt_修改.Rows[0]["检定结果"] = comboBox5.Text;
            //dt_修改.Rows[0]["准确度"] = textBox15.Text;
            dt_修改.Rows[0]["测量范围"] = textBox13.Text;
            //dt_修改.Rows[0]["分度值"] = textBox14.Text;
            if (dateEdit3.Text == "")
            {
                dt_修改.Rows[0]["出厂日期"] = DBNull.Value;
            }
            else
            {
                dt_修改.Rows[0]["出厂日期"] = dateEdit3.Text;
            }
            if (dateEdit4.Text == "")
            {
                dt_修改.Rows[0]["购置日期"] = DBNull.Value;
            }
            else
            {
                dt_修改.Rows[0]["购置日期"] = dateEdit4.Text;
            }

            dt_修改.Rows[0]["所属部门"] = comboBoxEdit1.Text;
            //dt_修改.Rows[0]["所属部门"] = searchLookUpEdit1.Text;
            dt_修改.Rows[0]["计量器具规格"] = textBox3.Text;
            dt_修改.Rows[0]["检定单位"] = textBox12.Text;
            dt_修改.Rows[0]["备注"] = textBox16.Text;
            if (dateEdit5.Text == "")
            {
                dt_修改.Rows[0]["领用日期"] = DBNull.Value;
            }
            else
            {
                dt_修改.Rows[0]["领用日期"] = dateEdit5.Text;
            }
            //dt_修改.Rows[0]["领用日期"] = dateEdit5.Text;
            return (dt_修改);
        }
        //删除
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr;
            dr = gv1.GetDataRow(gv1.FocusedRowHandle);

            //MessageBox.Show("确认删除吗？", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);            

            if (MessageBox.Show("确认删除吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                dr.Delete();
                sc = true;
                //try
                //{
                //    string sql = "select * from 计量器具基础信息表 where 1<>1";
                //    using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
                //    {

                //        new SqlCommandBuilder(da);
                //        da.Update(dt);

                //    }
                //    MessageBox.Show("删除成功");

                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message);
                //}
                //barLargeButtonItem1_ItemClick(null, null);

            }
        }
        /// <summary>
        /// true 为删除状态
        /// </summary>
        bool sc = false;

        private void fun_删除保存()
        {
            if (sc)
            {
            try
            {
                string sql = "select * from 计量器具基础信息表 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
                {

                    new SqlCommandBuilder(da);
                    da.Update(dt);

                }
                MessageBox.Show("删除成功");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            barLargeButtonItem1_ItemClick(null, null);
            }
        }
        //导出
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {

                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions(TextExportMode.Text, false, false);
                gc1.ExportToXlsx(saveFileDialog.FileName, options);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
        //打印
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gv1.CloseEditor();//关闭编辑状态
            this.BindingContext[dt].EndCurrentEdit();//关闭编辑状态
            string str = "";
            string str_打印机;
             PrintDialog printDialog1 = new PrintDialog();
             printDialog1.UseEXDialog = true;
             this.printDialog1.Document = this.printDocument1;
             if (printDialog1.ShowDialog() == DialogResult.OK)
             {
                 
                 DataRow[] drr_打印 = dt.Select(string.Format("选择= 'true'"));
                 //DataTable dtt = drr_打印.CopyToDataTable();
                 print.fun_print_基础信息(dt, drr_打印, printDialog1.PrinterSettings.PrinterName, false, str);
             }
        }
        //关闭
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        //清空内容
        private void fun_清空内容()
        {

            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            //textBox7.Clear();
            //textBox9.Clear();
            textBox10.Clear();
            textBox11.Clear();
            textBox12.Clear();
            //textBox15.Clear();
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;

            //comboBox1.Text = "";
            comboBox5.Text = "";
            dateEdit1.Text = "";
            //searchLookUpEdit1.Text = "";
            comboBoxEdit1.Text = "";
            searchLookUpEdit2.Text = "";
            //dateEdit2.Text = "";
            dateEdit3.Text = "";
            dateEdit4.Text = "";
            dateEdit5.Text = "";
            textBox13.Clear();
            //textBox14.Clear();


        }
        private void fun_可编辑()
        {
            textBox1.Enabled = true;
            textBox3.Enabled = true;
            textBox11.Enabled = true;
            textBox12.Enabled = true;
            comboBox1.Enabled = true;
            //comboBox5.Enabled = true;
            //searchLookUpEdit1.Enabled = true;
            comboBoxEdit1.Enabled = true;
            dateEdit1.Enabled = true;

        }

        private void check()
        {
            if (textBox1.Text.Trim() == "")
            {
                throw new Exception("计量器具编号不能为空");

            }
            if (textBox2.Text.Trim() == "")
            {
                throw new Exception("计量器具名称不能为空");
            }
            if (textBox3.Text.Trim() == "")
            {
                throw new Exception("计量器具规格不能为空");
            }
            if (comboBox1.Text.Trim() == "")
            {
                throw new Exception("所属大类不能为空");
            }
            if (textBox6.Text.Trim() == "")
            {
                throw new Exception("证书号不能为空");
            }
            if (textBox11.Text.Trim() == "")
            {
                throw new Exception("点检周期不能为空");
            }
            else if (textBox11.Text.Trim() != "" && rgx.IsMatch(textBox11.Text.Trim()) == false)
            {
                throw new Exception("点检周期输入的格式错误");
            }
            if (dateEdit1.Text.Trim() == "")
            {
                throw new Exception("点检有效期不能为空");
            }
            if (dateEdit1.Text.Trim() != "" && Convert.ToDateTime(dateEdit1.Text) < Convert.ToDateTime(CPublic.Var.getDatetime().ToString("yyyy-MM-dd")))
            {
                throw new Exception("有效期比当前时间小,输入有误");
            }

            //if (textBox7.Text.Trim() != "" && rgx2.IsMatch(textBox7.Text.Trim()) == false)
            //{
            //    throw new Exception("精度的格式错误");
            //}
            //if (dateEdit5.Text.Trim() == "")
            //{
            //    throw new Exception("领用日期不能为空");
            //}
            //if (dateEdit4.Text.Trim() == "")
            //{
            //    throw new Exception("购置日期不能为空");
            //}
            //if (dateEdit3.Text.Trim() == "")
            //{
            //    throw new Exception("出厂日期不能为空");
            //}
            if (dateEdit1 .Text .Trim () != "" && dateEdit4.Text.Trim () != "" && Convert .ToDateTime(dateEdit1 .Text) < Convert .ToDateTime(dateEdit4.Text))
            {
                throw new Exception("有效期比购置日期小,输入有误");
            }
            if (dateEdit4.Text.Trim() != "" && dateEdit1.Text.Trim() != "" && Convert.ToDateTime(dateEdit1.Text) < Convert.ToDateTime(dateEdit4.Text))
            {
                throw new Exception("购置日期比有效期大,输入有误");
            }
            if (dateEdit3.Text.Trim() != "" && dateEdit4.Text.Trim() != "" && Convert.ToDateTime(dateEdit4.Text) < Convert.ToDateTime(dateEdit3.Text))
            {
                throw new Exception("出厂日期比购置日期大,输入有误");
            }

            if (dateEdit5.Text.Trim() != "" && dateEdit4.Text.Trim() != "" && Convert.ToDateTime(dateEdit5.Text) < Convert.ToDateTime(dateEdit4.Text))
            {
                throw new Exception("领用日期比购置日期小,输入有误");
            }
        }

        private void textBox11_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }
        //点击事件
        private void gv1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr;
            if (checkBox1.Checked == true)
            {
                if (bl)
                {
                    //MessageBox.Show("当前为新增状态，不可以点击显示");
                    if (MessageBox.Show("当前为新增状态，有未保存数据，请确认？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        label23.Visible = false ;
                        textBox1.Enabled = false;
                        //textBox3.Enabled = false;
                        //textBox11.Enabled = false;
                        //textBox12.Enabled = false;
                        //comboBox1.Enabled = false;
                        //comboBox5.Enabled = false;
                        //searchLookUpEdit1.Enabled = false;
                        dateEdit1.Enabled = false;
                        dr = gv1.GetDataRow(gv1.FocusedRowHandle);
                        if (dr["领用日期"] != DBNull.Value)
                        {
                            dateEdit5.Text = Convert.ToDateTime(dr["领用日期"]).ToString("yyyy-MM-dd");
                        }
                        if (dr["出厂日期"] != DBNull.Value)
                        {
                            dateEdit3.Text = Convert.ToDateTime(dr["出厂日期"]).ToString("yyyy-MM-dd");
                        }
                        if (dr["购置日期"] != DBNull.Value)
                        {
                            dateEdit4.Text = Convert.ToDateTime(dr["购置日期"]).ToString("yyyy-MM-dd");
                        }

                        comboBox1.Text = null;
                        comboBox2.Text = null;
                        comboBox3.Text = null;
                        dataBindHelper1.DataFormDR(dr);
                        
                    }
                    
                    return;
                }
                else
                {
                    label23.Visible = false ;
                    bl_修改 = true;
                    textBox1.Enabled = false;
                    //textBox3.Enabled = false;
                    //textBox11.Enabled = false;
                    //textBox12.Enabled = false;
                    //comboBox1.Enabled = false;
                    //comboBox5.Enabled = false;
                    //searchLookUpEdit1.Enabled = false;
                    dateEdit1.Enabled = false;
                    dr = gv1.GetDataRow(gv1.FocusedRowHandle);
                    dateEdit1.Text = "";
                    dateEdit1.Enabled = true ;
                    dateEdit3.Text = "";
                    dateEdit4.Text = "";
                    dateEdit5.Text = "";
                    comboBox1.Text = null;
                    comboBox2.Text = null;
                    comboBox3.Text = null;
                    if (dr["领用日期"] != DBNull.Value)
                    {
                    dateEdit5.Text = Convert.ToDateTime(dr["领用日期"]).ToString("yyyy-MM-dd");
                    }
                    if (dr["出厂日期"] != DBNull.Value)
                    {
                        dateEdit3.Text = Convert.ToDateTime(dr["出厂日期"]).ToString("yyyy-MM-dd");
                    }
                    if (dr["购置日期"] != DBNull.Value)
                    {
                        dateEdit4.Text = Convert.ToDateTime(dr["购置日期"]).ToString("yyyy-MM-dd");
                    }
                    
                    dataBindHelper1.DataFormDR(dr);
                }

            }
            //else
            //{

            //    MessageBox.Show("请勾选快速选择");
            //}
        }

        private void 点检记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DataRow dr;
            dr = gv1.GetDataRow(gv1.FocusedRowHandle);
            Form2 f2 = new Form2(dr);
            f2.ShowDialog();
            //UI记录 f2 = new UI记录(dr );
            ////f2.ShowDialog();
        }

        //右击菜单
        private void gc1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc1, new Point(e.X, e.Y));
               
            }
        }

        //排序号
        private void gv1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }
        //数字必输
        private void textBox11_KeyPress_1(object sender, KeyPressEventArgs e)
        {
        
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        //全选
        private void button1_Click(object sender, EventArgs e)
        {
            foreach(DataRow r in dt.Rows)
            {
                r["选择"] = true;
            }
          
        }
        //全取消
        private void button2_Click(object sender, EventArgs e)
        {
            foreach(DataRow r in dt.Rows)
            {
                r["选择"] = false;
            
            }
        }

        private void gv1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            DataRow dr;
            if (checkBox1.Checked == true)
            {
                if (bl)
                {
                    //MessageBox.Show("当前为新增状态，不可以点击显示");
                    if (MessageBox.Show("当前为新增状态，有未保存数据，请确认？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        label23.Visible = false;
                        textBox1.Enabled = false;
                        //textBox3.Enabled = false;
                        //textBox11.Enabled = false;
                        //textBox12.Enabled = false;
                        //comboBox1.Enabled = false;
                        //comboBox5.Enabled = false;
                        //searchLookUpEdit1.Enabled = false;
                        dateEdit1.Enabled = false;
                        dr = gv1.GetDataRow(gv1.FocusedRowHandle);
                        if (dr["领用日期"] != DBNull.Value)
                        {
                            dateEdit5.Text = Convert.ToDateTime(dr["领用日期"]).ToString("yyyy-MM-dd");
                        }
                        if (dr["出厂日期"] != DBNull.Value)
                        {
                            dateEdit3.Text = Convert.ToDateTime(dr["出厂日期"]).ToString("yyyy-MM-dd");
                        }
                        if (dr["购置日期"] != DBNull.Value)
                        {
                            dateEdit4.Text = Convert.ToDateTime(dr["购置日期"]).ToString("yyyy-MM-dd");
                        }

                        comboBox1.Text = null;
                        comboBox2.Text = null;
                        comboBox3.Text = null;
                        dataBindHelper1.DataFormDR(dr);

                    }

                    return;
                }
                else
                {
                    label23.Visible = false;
                    bl_修改 = true;
                    textBox1.Enabled = false;
                    //textBox3.Enabled = false;
                    //textBox11.Enabled = false;
                    //textBox12.Enabled = false;
                    //comboBox1.Enabled = false;
                    //comboBox5.Enabled = false;
                    //searchLookUpEdit1.Enabled = false;
                    dateEdit1.Enabled = false;
                    dr = gv1.GetDataRow(gv1.FocusedRowHandle);
                    dateEdit1.Text = "";
                    dateEdit1.Enabled = true;
                    dateEdit3.Text = "";
                    dateEdit4.Text = "";
                    dateEdit5.Text = "";
                    comboBox1.Text = null;
                    comboBox2.Text = null;
                    comboBox3.Text = null;
                    if (dr["领用日期"] != DBNull.Value)
                    {
                        dateEdit5.Text = Convert.ToDateTime(dr["领用日期"]).ToString("yyyy-MM-dd");
                    }
                    if (dr["出厂日期"] != DBNull.Value)
                    {
                        dateEdit3.Text = Convert.ToDateTime(dr["出厂日期"]).ToString("yyyy-MM-dd");
                    }
                    if (dr["购置日期"] != DBNull.Value)
                    {
                        dateEdit4.Text = Convert.ToDateTime(dr["购置日期"]).ToString("yyyy-MM-dd");
                    }

                    dataBindHelper1.DataFormDR(dr);
                }

            }
            //else
            //{

        }

         

    }
}
