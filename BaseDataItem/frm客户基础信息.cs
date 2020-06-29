using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using DevExpress.XtraTreeList.Nodes;
using MySql.Data.MySqlClient;

namespace BaseData
{
    public partial class frm客户基础信息 : UserControl
    {
        #region 成员
        DataTable dtM;               //主表
        SqlDataAdapter da;
        DataView dv;                 //用于显示旧数据
        string strconn = CPublic.Var.strConn;
        //string strconnn = "Persist Security Info=True;User ID=sasa;Password=aa;Initial Catalog=zf;Data Source=.";  //暂用
        DataTable dt_省;
        DataTable dt_市;
        DataTable dt_业务员; //用来限制 业务员只能看到自己的 客户其他人不受限制
        DataTable dtP;
        DataTable dt_币种;
        bool flag_add = false;
        string cfgfilepath = "";

        DataTable dt_下拉物料;
        #endregion

        #region 自用类
        public frm客户基础信息()
        {
            InitializeComponent();
        }

        private void frm客户基础信息_Load(object sender, EventArgs e)
        {
            barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            if (File.Exists(cfgfilepath + string.Format(@"\{0}.xml", this.Name)))
            {

                gv.RestoreLayoutFromXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
            }
            string s = "select  *  from 客户分类表 order by   客户分类编码 ";
            DataTable tt = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            treeList1.OptionsBehavior.PopulateServiceColumns = true;
            treeList1.KeyFieldName = "GUID";
            treeList1.ParentFieldName = "上级类型GUID";
            treeList1.DataSource = tt;
            treeList1.CollapseAll();

            fun_加载业务员();
            fun_读取数据();

            fun_下拉框();
            //frm客户签订的合同 fm = new frm客户签订的合同();
            //tabPage2.Controls.Add(fm);
        }

        //对应快速选择
        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (checkBox4.Checked == true)
                {
                    if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
                    {
                        DataRow rr = gv.GetDataRow(e.RowHandle);
                        fun_dr赋值(rr);
                        textBox1.Enabled = false;
                    }
                }
                //判断右键菜单是否可用
                if (e != null && e.Button == MouseButtons.Right)
                {
                    contextMenuStrip2.Show(gc, new Point(e.X, e.Y));
                }
            }
            catch { }
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Black, 1);
            pen.DashStyle = DashStyle.Dash;
            e.Graphics.DrawRectangle(pen, panel5.DisplayRectangle);
        }

        //省变化后，市跟着变
        private void textBox5_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txt_市.Properties.Items.Clear();
                txt_市.Text = "";
                DataRow[] ds = dt_省.Select(string.Format("ProvinceName = '{0}'", txt_省.Text.ToString()));
                string sql_市 = string.Format("select * from S_City where ProvinceID = '{0}'", ds[0]["ProvinceID"].ToString());
                dt_市 = new DataTable();
                SqlDataAdapter da_市 = new SqlDataAdapter(sql_市, strconn);
                da_市.Fill(dt_市);
                foreach (DataRow r in dt_市.Rows)
                {
                    txt_市.Properties.Items.Add(r["CityName"].ToString());
                }
            }
            catch
            //(Exception ee)
            {
                //MessageBox.Show(ee.Message);
            }
        }

        //市变化后，区/县跟着变
        private void textBox6_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                textBox7.Properties.Items.Clear();
                textBox7.Text = "";
                if (txt_市.Text != "")
                {
                    DataRow[] ds = dt_市.Select(string.Format("CityName = '{0}'", txt_市.Text.ToString()));
                    string sql_区县 = string.Format("select * from S_District where CityID = '{0}'", ds[0]["CityID"].ToString());
                    DataTable dt_区县 = new DataTable();
                    SqlDataAdapter da_区县 = new SqlDataAdapter(sql_区县, strconn);
                    da_区县.Fill(dt_区县);
                    foreach (DataRow r in dt_区县.Rows)
                    {
                        textBox7.Properties.Items.Add(r["DistrictName"].ToString());
                    }
                }
            }
            catch
            //(Exception ee)
            {
                //MessageBox.Show(ee.Message);
            }
        }
        #endregion

        #region 方法

        private void fun_加载业务员()
        {
            string str = "select 业务员 from 客户基础信息表 group by 业务员 ";
            dt_业务员 = CZMaster.MasterSQL.Get_DataTable(str, strconn);

        }

        private void fun_读取数据()
        {
            try
            {
                //DataRow []dr_业务员= dt_业务员.Select(string.Format("业务员='{0}'", CPublic.Var.localUserName));
                //string sql = "select * from 客户基础信息表 ";

                // if (dr_业务员.Length > 0)
                //{
                //    sql = sql + string.Format("where 业务员='{0}'", CPublic.Var.localUserName);
                //}

                dtM = new DataTable();
                string sql = "select * from 客户基础信息表";
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                dv = new DataView(dtM);  //只显示旧数据
                dv.RowFilter = "新数据 = false";
                gc.DataSource = dv;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_清空()
        {
            textBox1.Text = "";
            txt_客户名称.Text = "";
            comboBox1.Text = "";
            textBox4.Text = "";
            txt_省.SelectedIndex = -1;  //省
            txt_市.SelectedIndex = -1;  //市
            textBox7.SelectedIndex = -1;  //区
            textBox8.Text = "";
            textBox9.Text = "";
            textBox10.Text = "";
            textBox11.Text = "";
            textBox12.Text = "";
            textBox13.Text = "";
            textBox14.Text = "";
            textBox15.Text = "";
            txt_业务员.SelectedIndex = -1;  //业务员
            textBox17.SelectedIndex = -1;  //客户等级
            textBox18.SelectedIndex = -1;  //账期
            textBox19.Text = "";
            textBox20.Text = "";
            textBox21.Text = "";
            //textBox22.SelectedIndex = -1;  //片区
            textBox23.Text = "";
            textBox24.Text = "";
            textBox25.Text = "";
            textBox26.Text = "";
            textBox27.Text = "";
            searchLookUpEdit1.EditValue = "";
            txt_客户属性.SelectedIndex = -1;
            checkBox1.CheckState = CheckState.Unchecked;

        }

        private void fun_dr赋值(DataRow r)
        {
            textBox1.Text = r["客户编号"].ToString();
            txt_客户名称.Text = r["客户名称"].ToString();
            comboBox1.Text = r["客户类型"].ToString();
            textBox4.Text = r["地址"].ToString();
            txt_省.Text = r["省"].ToString();
            txt_市.Text = r["市"].ToString();
            textBox7.Text = r["县"].ToString();
            textBox8.Text = r["开户银行"].ToString();
            textBox9.Text = r["税号"].ToString();
            textBox10.Text = r["账号"].ToString();
            textBox11.Text = r["联系人"].ToString();
            textBox12.Text = r["固定电话"].ToString();
            textBox13.Text = r["手机"].ToString();
            textBox14.Text = r["传真"].ToString();
            textBox15.Text = r["邮箱"].ToString();
            txt_业务员.Text = r["业务员"].ToString();
            textBox17.Text = r["客户等级"].ToString();
            textBox18.Text = r["账期"].ToString();
            textBox19.Text = r["合同状态"].ToString();
            textBox20.Text = r["合同内容"].ToString();
            textBox21.Text = r["邮编"].ToString();
           // textBox22.Text = r["片区"].ToString();
            textBox23.Text = r["备用联系人"].ToString();
            textBox24.Text = r["备用固定电话"].ToString();
            textBox25.Text = r["备用手机"].ToString();
            textBox26.Text = r["备用邮箱"].ToString();
            textBox27.Text = r["备注"].ToString();
            textBox2.Text = r["区域"].ToString();
            textBox3.Text = r["修改时间"].ToString();
            textBox5.Text = r["税率"].ToString(); 
            txt_客户属性.Text = r["客户属性"].ToString();
            txt_客户分类编码.Text = r["客户分类编码"].ToString();
            searchLookUpEdit1.EditValue = r["币种"].ToString();
            string s = string.Format("select 类别名称 from 客户分类表 where 客户分类编码='{0}'", r["客户分类编码"].ToString());
            DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            if(t.Rows.Count>0)
            {
                txt_客户分类.Text = t.Rows[0]["类别名称"].ToString();
            }

            if (r["停用"].Equals(true))
            {
                checkBox1.CheckState = CheckState.Checked;
            }
            else
            {
                checkBox1.CheckState = CheckState.Unchecked;
            }


            if (r["国内"].Equals(true))
            {
                checkBox2.CheckState = CheckState.Checked;
            }
            else
            {
                checkBox2.CheckState = CheckState.Unchecked;
            }


            if (r["国外"].Equals(true))
            {
                checkBox5.CheckState = CheckState.Checked;
            }
            else
            {
                checkBox5.CheckState = CheckState.Unchecked;
            }


        }   //datarow → textbox.text

        private void fun_dr复制(DataRow r)
        {
            //textBox1.Text = r["客户编号"].ToString();
            txt_客户名称.Text = r["客户名称"].ToString();
            comboBox1.Text = r["客户类型"].ToString();
            textBox4.Text = r["地址"].ToString();
            txt_省.Text = r["省"].ToString();
            txt_市.Text = r["市"].ToString();
            textBox7.Text = r["县"].ToString();
            textBox8.Text = r["开户银行"].ToString();
            textBox9.Text = r["税号"].ToString();
            textBox10.Text = r["账号"].ToString();
            textBox11.Text = r["联系人"].ToString();
            textBox12.Text = r["固定电话"].ToString();
            textBox13.Text = r["手机"].ToString();
            textBox14.Text = r["传真"].ToString();
            textBox15.Text = r["邮箱"].ToString();
            txt_业务员.Text = r["业务员"].ToString();
            textBox17.Text = r["客户等级"].ToString();
            textBox18.Text = r["账期"].ToString();
            textBox19.Text = r["合同状态"].ToString();
            textBox20.Text = r["合同内容"].ToString();
            textBox21.Text = r["邮编"].ToString();
            //textBox22.Text = r["片区"].ToString();
            textBox23.Text = r["备用联系人"].ToString();
            textBox24.Text = r["备用固定电话"].ToString();
            textBox25.Text = r["备用手机"].ToString();
            textBox26.Text = r["备用邮箱"].ToString();
            textBox27.Text = r["备注"].ToString();
            txt_客户属性.Text = r["客户属性"].ToString();
            textBox5.Text = r["税率"].ToString();
            searchLookUpEdit1.EditValue = r["币种"].ToString();
        }   //复制datarow → textbox.text，客户编号不变

        private void fun_tx赋值(DataRow r)
        {
            r["客户编号"] = textBox1.Text.ToString();
            r["客户名称"] = txt_客户名称.Text.ToString();
            r["客户类型"] = comboBox1.Text.ToString();
            r["地址"] = textBox4.Text.ToString();
            r["省"] = txt_省.Text.ToString();
            r["市"] = txt_市.Text.ToString();
            r["县"] = textBox7.Text.ToString();
            r["开户银行"] = textBox8.Text.ToString();
            r["税号"] = textBox9.Text.ToString();
            r["账号"] = textBox10.Text.ToString();
            r["联系人"] = textBox11.Text.ToString();
            r["固定电话"] = textBox12.Text.ToString();
            r["手机"] = textBox13.Text.ToString();
            r["传真"] = textBox14.Text.ToString();
            r["邮箱"] = textBox15.Text.ToString();
            r["业务员"] = txt_业务员.Text.ToString();
            r["客户等级"] = textBox17.Text.ToString();
            r["账期"] = textBox18.Text.ToString();
            r["合同状态"] = textBox19.Text.ToString();
            r["合同内容"] = textBox20.Text.ToString();
            r["邮编"] = textBox21.Text.ToString();
            //r["片区"] = textBox22.Text.ToString();
            r["备用联系人"] = textBox23.Text.ToString();
            r["备用固定电话"] = textBox24.Text.ToString();
            r["备用手机"] = textBox25.Text.ToString();
            r["备用邮箱"] = textBox26.Text.ToString();
            r["备注"] = textBox27.Text.ToString();
            r["客户属性"] = txt_客户属性.Text.ToString();
            r["税率"] = textBox5.Text.ToString();
            r["客户分类编码"] = txt_客户分类编码.Text;
            if (textBox2.Text != "")
            {
                fun_区域(textBox2.Text.ToString());
            }
            r["区域"] = textBox2.Text.ToString();
            r["修改时间"] = textBox3.Text.ToString();
            r["币种"] = searchLookUpEdit1.EditValue;

            //r["停用"] = checkBox1.Checked;

        }   //textbox.text → datarow

        public void fun_下拉框()
        {
            try
            {
                txt_省.Properties.Items.Clear();
                txt_业务员.Properties.Items.Clear();
                textBox17.Properties.Items.Clear();
                textBox18.Properties.Items.Clear();
                //textBox22.Properties.Items.Clear();
                txt_客户属性.Properties.Items.Clear();
                txt_客户属性.Properties.Items.Add("公司");
                txt_客户属性.Properties.Items.Add("个人");

                string sql = "select * from 基础数据基础属性表 where 属性类别 in ('客户等级','业务员','账期') order by POS";
                DataTable dt_属性 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_属性);
                foreach (DataRow r in dt_属性.Rows)
                {
                    if (r["属性类别"].ToString().Equals("客户等级"))
                    {
                        textBox17.Properties.Items.Add(r["属性值"].ToString());
                    }
                    //if (r["属性类别"].ToString().Equals("片区"))
                    //{
                    //    textBox22.Properties.Items.Add(r["属性值"].ToString());
                    //}
                    if (r["属性类别"].ToString().Equals("业务员"))
                    {
                        txt_业务员.Properties.Items.Add(r["属性值"].ToString());
                    }
                    if (r["属性类别"].ToString().Equals("账期"))
                    {
                        textBox18.Properties.Items.Add(r["属性值"].ToString());
                    }
                }

                string sql_省 = "select * from S_Province";
                dt_省 = new DataTable();
                SqlDataAdapter da_省 = new SqlDataAdapter(sql_省, strconn);
                da_省.Fill(dt_省);
                foreach (DataRow r in dt_省.Rows)
                {
                    txt_省.Properties.Items.Add(r["ProvinceName"].ToString());
                }

                /*
                 * string.Format(@"select  基础数据物料信息表.物料编码,基础数据物料信息表.规格型号 
                ,基础数据物料信息表.物料名称,n销售单价,库存总数,特殊备注 from 基础数据物料信息表,仓库物料数量表 
                 where  可售=1 and 基础数据物料信息表.物料编码=仓库物料数量表.物料编码");
                 * */
                string sql_1 = string.Format(@"select  基础数据物料信息表.物料编码,基础数据物料信息表.规格型号 
                ,基础数据物料信息表.物料名称,n销售单价,特殊备注 from 基础数据物料信息表   where  内销=1 or 外销=1  ");
                dt_下拉物料 = new DataTable();
                dt_下拉物料 = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);
                repositoryItemSearchLookUpEdit1.DataSource = dt_下拉物料;
                repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
                repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";

                string sql_币种 = " select 属性值 as 币种 from 基础数据基础属性表 where 属性类别 = '币种'";
                dt_币种 = CZMaster.MasterSQL.Get_DataTable(sql_币种, strconn);
                searchLookUpEdit1.Properties.DataSource = dt_币种;
                searchLookUpEdit1.Properties.DisplayMember = "币种";
                searchLookUpEdit1.Properties.ValueMember = "币种";

            }
            catch { }
        }
        private void fun_区域(string str_省)
        {

            if (str_省.IndexOf("江苏") > 0 || str_省.IndexOf("浙江") > 0 || str_省.IndexOf("上海") > 0 || str_省.IndexOf("安徽") > 0 || str_省.IndexOf("江西") > 0)
            {
                textBox2.Text = "华东地区";
            }
            if (str_省.IndexOf("北京") > 0 || str_省.IndexOf("天津") > 0 || str_省.IndexOf("河北") > 0 || str_省.IndexOf("山西") > 0 || str_省.IndexOf("内蒙") > 0 || str_省.IndexOf("山东") > 0)
            {
                textBox2.Text = "华北地区";
            }
            if (str_省.IndexOf("河南") > 0 || str_省.IndexOf("湖北") > 0 || str_省.IndexOf("湖南") > 0)
            {
                textBox2.Text = "华中地区";

            }
            if (str_省.IndexOf("广东") > 0 || str_省.IndexOf("广西") > 0 || str_省.IndexOf("海南") > 0 || str_省.IndexOf("福建") > 0)
            {
                textBox2.Text = "华南地区";

            }
            if (str_省.IndexOf("辽宁") > 0 || str_省.IndexOf("吉林") > 0 || str_省.IndexOf("黑龙江") > 0)
            {
                textBox2.Text = "东北地区";

            }
            if (str_省.IndexOf("陕西") > 0 || str_省.IndexOf("甘肃") > 0 || str_省.IndexOf("青海") > 0)
            {
                textBox2.Text = "西北地区";

            }
            if (str_省.IndexOf("重庆") > 0 || str_省.IndexOf("四川") > 0 || str_省.IndexOf("贵州") > 0 || str_省.IndexOf("云南") > 0 || str_省.IndexOf("西藏") > 0)
            {
                textBox2.Text = "西南地区";
            }
            if (str_省.IndexOf("香港") > 0 || str_省.IndexOf("澳门") > 0 || str_省.IndexOf("台湾") > 0)
            {
                textBox2.Text = "港澳台地区";
            }


        }
        public void fun_Check()
        {
            if (txt_客户名称.Text == "")
            {
                throw new Exception("客户名称不能为空！");
            }

            if (txt_客户分类.Text == "")
            {
                throw new Exception("客户分类不能为空！");
            }
            
            if (textBox5.Text == "")
            {
                throw new Exception("税率不能为空！");
            }
            if (searchLookUpEdit1.EditValue == null||searchLookUpEdit1.EditValue.ToString()=="")
            {
                throw new Exception("币种不能为空！");
            }
            if (comboBox1.Text == "")
            {
                throw new Exception("客户类型不能为空");
            }
            if (flag_add == true)
            {
                string sql_验 = string.Format("select * from 客户基础信息表 where  客户名称='{0}' and 停用=0 ", txt_客户名称.Text.ToString().Trim());
                using (SqlDataAdapter da_y = new SqlDataAdapter(sql_验, strconn))
                {
                    DataTable dt = new DataTable();

                    da_y.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        throw new Exception("已有相同客户名称记录！");

                    }
                }
                sql_验 = string.Format("select * from 客户基础信息表 where  客户编号='{0}'", textBox1.Text.ToString().Trim());
                using (SqlDataAdapter da_y = new SqlDataAdapter(sql_验, strconn))
                {
                    DataTable dt = new DataTable();

                    da_y.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        throw new Exception("已有相同客户编号记录！可尝试编号手动加1再进行保存。");

                    }

                }
            }
            else
            {
                if(textBox1.Text=="")
                {
                    throw new Exception("当前不是新增模式,请于前一个选择页选择客户分类后点击新增再进行新增客户信息");
                }
            }
            //if (textBox4.Text == "")
            //{
            //    throw new Exception("公司地址不能为空！");
            //}
            //if (textBox11.Text == "")
            //{
            //    throw new Exception("联系人不能为空！");
            //}

            //if (textBox22.Text == "")
            //{
            //    throw new Exception("片区不能为空！");
            //}
            string str ="";
            if (textBox1.Text!="")
              str= textBox1.Text.Substring(0, 2);
            if (str != "81" && str != "82")
            {
                if (txt_省.EditValue != null && txt_省.EditValue.ToString() == "")
                {
                    throw new Exception("请选择所属省！");
                }

                if (txt_市.EditValue != null && txt_市.EditValue.ToString() == "")
                {
                    throw new Exception("请选择所属市！");
                }
                //if (txt_业务员.EditValue != null && txt_业务员.EditValue.ToString() == "")
                //{
                //    throw new Exception("请选择业务员！");
                //}
            }
        }

        private void fun_保存数据()
        {
            try
            {
                string str = textBox1.Text.Substring(0, 2);
                if(str == "81"|| str == "82")
                {
                    string strcon_aliyun = string.Format("server={0};User Id={1};password={2};Database={3};CharSet=utf8", CPublic.Var.li_CFG["aliyun_server"].ToString(),
                        CPublic.Var.li_CFG["aliyun_UID"].ToString(), CPublic.Var.li_CFG["aliyun_PWD"].ToString(), CPublic.Var.li_CFG["aliyun_database"].ToString());
                    string s = string.Format(" select * from  distributors  where   DistributorCode='{0}'", textBox1.Text);
                    using (MySqlDataAdapter daa = new MySqlDataAdapter(s, strcon_aliyun))
                    {
                        DataTable dt_somain = new DataTable();
                        daa.Fill(dt_somain);
                        DataTable dt_copy = dt_somain.Clone();
                        DataRow dr = dt_copy.NewRow();
                        dt_copy.Rows.Add(dr);
                        dr["DistributorCode"] = textBox1.Text;
                        dr["DistributorName"] = txt_客户名称.Text;
                        if (dt_somain.Rows.Count == 0)//没有新增  有修改
                        {
                            foreach (DataRow rr in dt_copy.Rows)
                            {
                                dt_somain.ImportRow(rr);
                            }
                        }
                        else
                        {
                            for (int i = dt_somain.Rows.Count - 1; i >= 0; i--)
                            {
                                dt_somain.Rows[i].Delete();
                            }
                            foreach (DataRow rr in dt_copy.Rows)
                            {
                                dt_somain.ImportRow(rr);
                            }
                        }
                        //dt_somain.Rows[0]["bl"] = true; 改为到物流确认 赋值为true
                        new MySqlCommandBuilder(daa);
                        daa.Update(dt_somain);
                    }
                }
                string sql = "select * from 客户基础信息表 where 1<>1";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dtM);
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region 客户基础信息界面操作
        //全部
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
            {
                gc.DataSource = dtM;
            }
            else
            {
                gc.DataSource = dv;   //用于显示旧数据
            }
        }

        //快速选择
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked == true)
            {
                button7.Enabled = false;
            }
            if (checkBox4.Checked == false)
            {
                button7.Enabled = true;
            }
        }

        //复制
        private void button7_Click(object sender, EventArgs e)
        {
            DataRow r_选中行 = gv.GetDataRow(gv.FocusedRowHandle);
            fun_dr复制(r_选中行);
        }

        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ERPorg.Corg.FlushMemory();
            fun_读取数据();
            flag_add = false;
        }

        //新增
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_清空();
         // fun_客户编号((dtM.Rows.Count + 1).ToString("000000"));
            flag_add = true;
         //   textBox1.Enabled = true;
            try
            {
                if (treeList1.Nodes.Count > 0)
                {
                    if (treeList1.Selection[0] == null) return;
                }
                else
                {
                    return;
                }
 
                TreeListNode n = treeList1.Selection[0];
                if (n.HasChildren) throw new Exception("此分类还有子级分类,不可在此分类下新增客户");
                
                txt_客户分类.Text = n.GetValue("类别名称").ToString();
                txt_客户分类编码.Text = n.GetValue("客户分类编码").ToString();
                tabControl1.SelectedTab = tabPage1;
               
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void fun_客户编号(string str)
        {
            DataRow[] ds = dtM.Select(string.Format("客户编号 = '{0}'", str));
            if (ds.Length > 0)
            {
                str = (Convert.ToInt32(str) + 1).ToString("000000");
                fun_客户编号(str);
            }
            else
            {
                textBox1.Text = str;
            }
        }

        //删除
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (textBox1.Text.ToString() != "")
                {
                    DataRow[] ds = dtM.Select(string.Format("客户编号 = '{0}'", textBox1.Text.ToString()));
                    DataRow r = ds[0];
                    r.Delete();
                    fun_清空();
                    fun_保存数据();
                    flag_add = false;


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //保存
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_Check();

                textBox3.Text = CPublic.Var.getDatetime().ToString();
                if (flag_add && textBox1.Text =="")
                {
                    string x = txt_客户分类编码.Text;
                    string s = string.Format(@"select  max(客户编号)客户编号 from 客户基础信息表 where 客户分类编码='{0}'", x);
                    x = x.PadRight(10, '0');
                    DataTable temp = new DataTable();
                    temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                    if (temp.Rows.Count == 0 || temp.Rows[0]["客户编号"].ToString() == "") x = x + "0001";
                    else
                    {
                        s = temp.Rows[0]["客户编号"].ToString();
                        s = (Convert.ToInt32(temp.Rows[0]["客户编号"].ToString().Substring(10, 4)) + 1).ToString().PadLeft(4, '0');
                        x = x + s;
                    }
                    textBox1.Text = x;
                }
                 if (textBox1.Text.ToString() != "")
                {
                    DataRow[] ds = dtM.Select(string.Format("客户编号 = '{0}'", textBox1.Text.ToString()));
                    if (ds.Length > 0)
                    {
                        fun_tx赋值(ds[0]);
                    }
                    else
                    {
                        DataRow dr = dtM.NewRow();
                        fun_tx赋值(dr);
                        dr["新数据"] = false;
                        dtM.Rows.Add(dr);
                    }
                }
                
                fun_保存数据();
                flag_add = false;

                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 右键菜单
        private void 客户合同信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                tabPage2.Controls.Clear();
                DataRow r = gv.GetDataRow(gv.FocusedRowHandle);
                string str_客户编号 = r["客户编号"].ToString();
                frm客户签订的合同 fm = new frm客户签订的合同(str_客户编号);
                tabPage2.Controls.Add(fm);
                fm.Dock = DockStyle.Fill;
                tabControl1.SelectedTab = tabPage2;
            }
            catch { }
        }
        private void fun_客户产品单价(string str_客户ID, string str_客户名)
        {
            string sql = string.Format(@"select  客户产品单价表.*,客户名称,规格型号,基础数据物料信息表.物料名称 
                                           from  客户产品单价表,基础数据物料信息表,客户基础信息表
    where 客户基础信息表.客户编号=客户产品单价表.客户编号 and 客户产品单价表.物料编码=基础数据物料信息表.物料编码
     and 客户产品单价表.客户编号='{0}'", str_客户ID);
            dtP = new DataTable();
            dtP = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gridControl1.DataSource = dtP;

            tabControl1.SelectedTab = tabPage3;
            gridView1.ViewCaption = str_客户名 + "对应产品单价";

        }
        private void 客户产品信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                fun_客户产品单价(dr["客户编号"].ToString(), dr["客户名称"].ToString());
                textBox16.Text = dr["客户编号"].ToString();
                textBox6.Text = dr["客户名称"].ToString();


                //tabPage3.Controls.Clear();
                //DataRow r = gv.GetDataRow(gv.FocusedRowHandle);
                //string str_客户编号 = r["客户编号"].ToString();
                //frm产品对应关系 fm = new frm产品对应关系(str_客户编号);
                //tabPage3.Controls.Add(fm);
                //fm.Dock = DockStyle.Fill;
                //tabControl1.SelectedTab = tabPage3;
            }
            catch { }
        }
        #endregion

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                if (checkBox1.CheckState == CheckState.Checked)
                {
                    string sql = "update 客户基础信息表 set 停用 =1 where  客户编号='" + textBox1.Text.Trim().ToString() + "'";
                    CZMaster.MasterSQL.ExecuteSQL(sql, strconn);
                    DataRow[] dr = dtM.Select(string.Format("客户编号='{0}'", textBox1.Text));
                    dr[0]["停用"] = 1;


                }
                else
                {
                    string sql = "update 客户基础信息表 set 停用 =0 where  客户编号='" + textBox1.Text.Trim().ToString() + "'";
                    CZMaster.MasterSQL.ExecuteSQL(sql, strconn);
                    DataRow[] dr = dtM.Select(string.Format("客户编号='{0}'", textBox1.Text));
                    dr[0]["停用"] = 0;
                }
            }
        }

        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void repositoryItemSearchLookUpEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {

        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtP == null)
                {
                    throw new Exception("未选择任何客户进行维护");
                }
                dtP.NewRow();
                dtP.Rows.Add();
                gridView1.FocusedRowHandle = gridView1.LocateByDisplayText(0, gridColumn21, "");
                //  gridView1. = gridView1.LocateByDisplayText(0, gridColumn19, "");

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            dr.Delete();
        }
        private void fun_check_单价保存()
        {

            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted)
                    continue;
                if (dr["物料编码"].ToString().Trim() == "")
                {

                    throw new Exception("有物料未选择,请检查");
                }
                 decimal dec=0;
                try
                {
                    dec= Convert.ToDecimal(dr["单价"]);
                }
                catch (Exception ex)
                {

                    throw new Exception("单价输入不合法");
                }
                if (dec < 0) throw new Exception("单价小于0");

            }

        }
        private void simpleButton4_Click(object sender, EventArgs e)
        {
            try
            {

                gridView1.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                fun_check_单价保存();

                CZMaster.MasterSQL.Save_DataTable(dtP, "客户产品单价表", strconn);
                MessageBox.Show("ok");
                fun_客户产品单价(textBox16.Text, textBox6.Text);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                gridControl1.ExportToXlsx(saveFileDialog.FileName);

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void repositoryItemSearchLookUpEdit1_EditValueChanging_1(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (e.NewValue != null && e.NewValue.ToString() != "")
            {
                DataRow[] r = dt_下拉物料.Select(string.Format("物料编码='{0}'", e.NewValue));
                dr["物料编码"] = r[0]["物料编码"];
                dr["物料名称"] = r[0]["物料名称"];
                dr["规格型号"] = r[0]["规格型号"];
                dr["单价"] = r[0]["n销售单价"];
                dr["客户编号"] = textBox16.Text;
            }
            else
            {
                dr["物料编码"] = "";
                dr["物料名称"] = "";
                dr["规格型号"] = "";
                dr["单价"] = 0;
                dr["客户编号"] = "";
            }
        }
        private void textBox13_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }
        }

        private void treeList1_MouseClick(object sender, MouseEventArgs e)
        {
            if (treeList1.Nodes.Count > 0)
            {
                if (treeList1.Selection[0] == null) return;
            }
            else
            {
                return;
            }
            TreeListNode n = treeList1.Selection[0];
            string s = n.GetValue("客户分类编码").ToString();
            DataView v = new DataView(dtM);
            v.RowFilter = String.Format("客户分类编码 like '{0}%'", s);
            gridControl2.DataSource = v;
        }
        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            if (e.TabPage.Name == "tabPage1")
            {
                barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }
            else if (e.TabPage.Name == "tabPage4")
            {
                barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

            }

        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                if (checkBox2.CheckState == CheckState.Checked)
                {
                    string sql = "update 客户基础信息表 set 国内 =1 where  客户编号='" + textBox1.Text.Trim().ToString() + "'";
                    CZMaster.MasterSQL.ExecuteSQL(sql, strconn);
                    DataRow[] dr = dtM.Select(string.Format("客户编号='{0}'", textBox1.Text));
                    dr[0]["国内"] = 1;


                }
                else
                {
                    string sql = "update 客户基础信息表 set 国内 =0 where  客户编号='" + textBox1.Text.Trim().ToString() + "'";
                    CZMaster.MasterSQL.ExecuteSQL(sql, strconn);
                    DataRow[] dr = dtM.Select(string.Format("客户编号='{0}'", textBox1.Text));
                    dr[0]["国内"] = 0;
                }
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                if (checkBox5.CheckState == CheckState.Checked)
                {
                    string sql = "update 客户基础信息表 set 国外 =1 where  客户编号='" + textBox1.Text.Trim().ToString() + "'";
                    CZMaster.MasterSQL.ExecuteSQL(sql, strconn);
                    DataRow[] dr = dtM.Select(string.Format("客户编号='{0}'", textBox1.Text));
                    dr[0]["国外"] = 1;


                }
                else
                {
                    string sql = "update 客户基础信息表 set 国外 =0 where  客户编号='" + textBox1.Text.Trim().ToString() + "'";
                    CZMaster.MasterSQL.ExecuteSQL(sql, strconn);
                    DataRow[] dr = dtM.Select(string.Format("客户编号='{0}'", textBox1.Text));
                    dr[0]["国外"] = 0;
                }
            }
        }
    }
}
