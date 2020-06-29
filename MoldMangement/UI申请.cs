using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;

using System.Text;

using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;


namespace MoldMangement
{
    public partial class UI申请 : UserControl
    {

        //static string strConn = "Password = a; Persist Security Info = True; User ID = sa; Initial Catalog = fms; Data Source=XINREN";
        string strConn = CPublic.Var.strConn;
        DataTable dt = new DataTable();
        DataTable dt_部门 = new DataTable();
        DataTable dt_已有 = new DataTable();
        DataTable dt_bm = new DataTable();
        //string leibie1 = "";
        //string leibie2 = "";
        //bool flag = false;
        ///// <summary>
        ///// true为新增主表状态
        ///// </summary>
        //bool t = true ;
        ///// <summary>
        ///// true为新增明细状态
        ///// </summary>
        //bool tt = false ;


        public UI申请()
        {
            InitializeComponent();
        }

        private void fun_load()
        {
            string sql = "select * from 计量器具申请明细表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
            {
                dt = new DataTable();
                da.Fill(dt);

            }
            gc1.DataSource = dt;
            //fun_xl();
            fun_下拉();
            dateEdit1.Text = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");
            textBox5.Text  = CPublic.Var.localUserName;
            //textBox4.Text = CPublic.Var.localUser部门名称;
            fun_部门();
        }

        private void UI申请_Load(object sender, EventArgs e)
        {
            fun_load();
            //bool tt = false;
        }

        private void fun_部门()
        {
            string sql5 = string.Format(" select 部门 from 人事基础员工表 where 姓名 ='{0}'", textBox5.Text);
            using (SqlDataAdapter da = new SqlDataAdapter(sql5, strConn))
            {
                dt_bm = new DataTable();
                da.Fill(dt_bm);

            }
            textBox4.Text = dt_bm.Rows[0]["部门"].ToString();
        }



        private void fun_下拉()
        {

            string sql2 = "select 计量器具编号,计量器具名称,计量器具规格,所属大类,所属部门,制造单位,状态 from 计量器具基础信息表";
            using (SqlDataAdapter da = new SqlDataAdapter(sql2, strConn))
            {
                dt_已有 = new DataTable();
                da.Fill(dt_已有);

            }
            searchLookUpEdit1.Properties.DataSource = dt_已有;
            searchLookUpEdit1.Properties.ValueMember = "计量器具编号";
            searchLookUpEdit1.Properties.DisplayMember = "计量器具名称";

        }

        //private void fun_xl()
        //{
        //    string sql3 = "select 部门编号,部门名称 from 人事基础部门表 order by 部门编号 asc";
        //    using (SqlDataAdapter da = new SqlDataAdapter(sql3, strConn))
        //    {
        //        dt_部门 = new DataTable();
        //        da.Fill(dt_部门);

        //    }
        //    searchLookUpEdit2.Properties.DataSource = dt_部门;
        //    searchLookUpEdit2.Properties.ValueMember = "部门名称";
        //    searchLookUpEdit2.Properties.DisplayMember = "部门名称";

        //}

        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_load();
            textBox1.Clear();
            textBox2.Clear();

            //comboBox1.SelectedIndex = -5;
            //comboBox1.Text = null;
            searchLookUpEdit1.Text = "";
        }
        //保存
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (dt.Rows.Count > 0)
                {
                    gv1.CloseEditor();//关闭编辑状态
                    this.BindingContext[dt].EndCurrentEdit();//关闭编辑状态
                    foreach (DataRow r in dt.Rows)
                    {
                        string a = r["计量器具名称"].ToString();
                        string aa = r["计量器具规格"].ToString();
                        string aaa = r["数量"].ToString();
                        if (a == "" || aa == "" || aaa == "")
                        {
                            MessageBox.Show("明细表新增时没有填写数据，请填写数据再保存");
                            return;
                        }
                        else if (Convert.ToInt32(aaa) <= 0)
                        {
                            MessageBox.Show("数量填写错误，请重新填写");
                            return;

                        }

                    }
                    check();
                    
                    fun_主表();
                    fun_子表();

                }
                else
                {
                    MessageBox.Show("明细表没有填写数据，请录入数据再保存");

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);


            }
        }
        //关闭
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void fun_主表()
        {
            try
            {
                DataTable dt_虚拟 = new DataTable();
                string sql = "select * from 计量器具申请主表 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
                {
                    da.Fill(dt_虚拟);
                    DataRow dr = dt_虚拟.NewRow();

                    DateTime t = CPublic.Var.getDatetime();
                    textBox1.Text = string.Format("JL{0}{1:00}{2:00}{3:0000}", t.Year, t.Month,
                            t.Day, CPublic.CNo.fun_得到最大流水号("JL", t.Year, t.Month));

                    //设置当前时间

                    dateEdit1.EditValue = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");
                    dr["申请单号"] = textBox1.Text;
                    dr["申请类别"] = comboBox2.Text;
                    dr["申请原因"] = textBox2.Text;
                    dr["申请时间"] = dateEdit1.Text;

                    dr["申请人员"] = textBox5.Text;
                    dr["申请部门"] = textBox4.Text;
                    //dr["生效"] = false;


                    dt_虚拟.Rows.Add(dr);
                    new SqlCommandBuilder(da);
                    da.Update(dt_虚拟);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);


            }
        }

        private void fun_子表()
        {


            try
            {

                string sql = "select * from 计量器具申请明细表 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
                {
                    int i = 0;

                    //if (comboBox1.Text == "申购")
                    //{
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["申请单号"] = textBox1.Text;
                        i++;
                        dr["申请明细号"] = dr["申请单号"].ToString() + "-" + i.ToString().PadLeft(2, '0');
                        //dr["生效"] = false;
                        //}
                    }

                    new SqlCommandBuilder(da);
                    da.Update(dt);
                }
                MessageBox.Show("保存成功");
                barLargeButtonItem1_ItemClick(null, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);


            }


        }

        //封存,领用,报废时的新增按钮
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.Text != "")
            {
                DataRow dr_已有;
                DataRow[] drr = dt_已有.Select(string.Format("计量器具名称 = '{0}'", searchLookUpEdit1.Text));
                dr_已有 = dt.NewRow();
                foreach (DataRow dr in drr)
                {
                    dr_已有["计量器具名称"] = dr["计量器具名称"];
                    dr_已有["计量器具规格"] = dr["计量器具规格"];
                    dr_已有["数量"] = 1;
                    dr_已有["备注"] = "";

                }
                dt.Rows.Add(dr_已有);
                this.gridColumn1.OptionsColumn.AllowEdit = false;
                this.gridColumn2.OptionsColumn.AllowEdit = false;
                this.gridColumn3.OptionsColumn.AllowEdit = false;
                this.gridColumn4.OptionsColumn.AllowEdit = false;

            }
            else
            {
                MessageBox.Show("请先在旁边下拉按钮选择物品再新增");
                return;
            }

        }

        //申购时的新增按钮
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DataRow dr;
            dr = dt.NewRow();
            dt.Rows.Add(dr);
            this.gridColumn1.OptionsColumn.AllowEdit = true;
            this.gridColumn2.OptionsColumn.AllowEdit = true;
            this.gridColumn3.OptionsColumn.AllowEdit = true;
            this.gridColumn4.OptionsColumn.AllowEdit = true;
        }
        //删除
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DataRow dr;
            dr = gv1.GetDataRow(gv1.FocusedRowHandle);
            if (MessageBox.Show("确认删除吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                if (dr == null)
                {
                    MessageBox.Show("没有选中行，无法删除");

                }
                else
                {
                    dr.Delete();
                }
            }
        }
       
        //private void comboBox1_TextChanged(object sender, EventArgs e)
        //{
          
        //    if (comboBox1.Text == "申购")
        //    {
               
        //        if (dt.Rows.Count > 0)
        //        {
        //            if (comboBox1.Text == "申购")
        //            {
        //                if (MessageBox.Show("填写的数据没保存，确定继续吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
        //                {
        //                    dt.Clear();
        //                    textBox2.Clear();
        //                    searchLookUpEdit1.Text = "";
        //                    simpleButton1.Visible = true;
        //                    simpleButton3.Visible = false;
        //                    searchLookUpEdit1.Visible = false;
        //                    label7.Visible = false;
        //                    this.simpleButton2.Location = new System.Drawing.Point(130, 5);
        //                    //leibie1 = leibie2;
        //                    ////leibie2 = "";
        //                }
        //                else
        //                {
        //                    //flag = true; 
        //                    ////leibie2 = "";
        //                    //comboBox1.Text = leibie1;

        //                    return;

        //                }

        //            }
        //        }
        //        else
        //        {

        //            simpleButton1.Visible = true;
        //            simpleButton3.Visible = false;
        //            searchLookUpEdit1.Visible = false;
        //            label7.Visible = false;
        //            this.simpleButton2.Location = new System.Drawing.Point(130, 5);
        //        }
        //    }
        //    else
        //    {
        //        //comboBox1.Text = textBox3.Text;  
        //        if (dt.Rows.Count > 0)
        //        {
        //            if (comboBox1.Text != "申购")
        //            {

        //                if (MessageBox.Show("填写的数据没保存，确定继续吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
        //                {
        //                    dt.Clear();
        //                    textBox2.Clear();
        //                    searchLookUpEdit1.Text = "";
        //                    simpleButton1.Visible = false;
        //                    simpleButton3.Visible = true;
        //                    this.simpleButton3.Location = new System.Drawing.Point(12, 5);
        //                    searchLookUpEdit1.Visible = true;
        //                    this.searchLookUpEdit1.Location = new System.Drawing.Point(120, 11);
        //                    this.simpleButton2.Location = new System.Drawing.Point(280, 5);
        //                    label7.Visible = true;
        //                    this.label7.Location = new System.Drawing.Point(400, 16);
        //                    //leibie1 = leibie2;
        //                    //leibie2 = "";
        //                }
        //                else
        //                {
        //                    //flag = true; 
        //                    //comboBox1.Text = leibie1;
        //                    ////leibie2 = "";
        //                    return;

        //                }

        //            }
        //        }
        //        else
        //        {
        //            simpleButton1.Visible = false;
        //            simpleButton3.Visible = true;
        //            this.simpleButton3.Location = new System.Drawing.Point(12, 5);
        //            searchLookUpEdit1.Visible = true;
        //            this.searchLookUpEdit1.Location = new System.Drawing.Point(120, 11);
        //            this.simpleButton2.Location = new System.Drawing.Point(280, 5);
        //            label7.Visible = true;
        //            this.label7.Location = new System.Drawing.Point(400, 16);

        //        }
        //    }
        //}
        Regex rgx = new Regex(@"^[0-9]*[1-9][0-9]*$");
        private void gv1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            //if (gridColumn3.Name  == "数量")
            //{
            //    if (rgx.IsMatch(e.Value.ToString ()) == false )//自己写验证条件
            //    {
            //        e.ErrorText = "只能输入大于零的整数,不可以输入小数或小于零的数";
            //        e.Valid = false;
            //        return;
            //    }
            //}
        }


        private void check()
        {
            if (comboBox2.Text.Trim() == "")
            {
                throw new Exception("申请类别不可以为空");

            }
            if (textBox2.Text.Trim() == "")
            {
                throw new Exception("申请原因不可以为空");

            }

        }
        //排序
        private void gv1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        bool flag = false;
        int index = 0;
                
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (flag)
            {
               
                if (comboBox2.Text == "申购")
                {
                    if (dt.Rows.Count > 0)
                    {

                        if (MessageBox.Show("填写的数据没保存，确定继续吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            index = comboBox2.SelectedIndex;
                            dt.Clear();
                            textBox2.Clear();
                            searchLookUpEdit1.Text = "";
                            simpleButton1.Visible = true;
                            simpleButton3.Visible = false;
                            searchLookUpEdit1.Visible = false;
                            label7.Visible = false;
                            this.simpleButton2.Location = new System.Drawing.Point(130, 5);

                        }
                        else
                        {
                            flag = false ;
                            comboBox2.SelectedIndex = index;

                        }
                    }
                    else
                    {
                        index = comboBox2.SelectedIndex;
                        simpleButton1.Visible = true;
                        simpleButton3.Visible = false;
                        searchLookUpEdit1.Visible = false;
                        label7.Visible = false;
                        this.simpleButton2.Location = new System.Drawing.Point(130, 5);

                    }
                }
                else
                {
                    
                    if (dt.Rows.Count > 0)
                    {
                        if (MessageBox.Show("填写的数据没保存，确定继续吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            index = comboBox2.SelectedIndex;
                            dt.Clear();
                            textBox2.Clear();
                            searchLookUpEdit1.Text = "";
                            simpleButton1.Visible = false;
                            simpleButton3.Visible = true;
                            this.simpleButton3.Location = new System.Drawing.Point(12, 5);
                            searchLookUpEdit1.Visible = true;
                            this.searchLookUpEdit1.Location = new System.Drawing.Point(120, 11);
                            this.simpleButton2.Location = new System.Drawing.Point(280, 5);
                            label7.Visible = true;
                            this.label7.Location = new System.Drawing.Point(400, 16);
                        }
                        else
                        {
                            flag = false;
                            comboBox2.SelectedIndex = index;

                        }

                    }
                    else
                    {
                        index = comboBox2.SelectedIndex;
                        simpleButton1.Visible = false;
                        simpleButton3.Visible = true;
                        this.simpleButton3.Location = new System.Drawing.Point(12, 5);
                        searchLookUpEdit1.Visible = true;
                        this.searchLookUpEdit1.Location = new System.Drawing.Point(120, 11);
                        this.simpleButton2.Location = new System.Drawing.Point(280, 5);
                        label7.Visible = true;
                        this.label7.Location = new System.Drawing.Point(400, 16);
                    }
                }
           }
          else
           {
               index = comboBox2.SelectedIndex;
                flag = true  ;
                if (index >0)
                 {
                     simpleButton1.Visible = false;
                     simpleButton3.Visible = true;
                     this.simpleButton3.Location = new System.Drawing.Point(12, 5);
                     searchLookUpEdit1.Visible = true;
                     this.searchLookUpEdit1.Location = new System.Drawing.Point(120, 11);
                     this.simpleButton2.Location = new System.Drawing.Point(280, 5);
                     label7.Visible = true;
                     this.label7.Location = new System.Drawing.Point(400, 16);
                 }
            }


        }

       




    }
}
