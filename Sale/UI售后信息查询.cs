using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using DevExpress.XtraTreeList.Nodes;
using System.IO;
using System.Reflection;
using CZMaster;
using DevExpress.XtraTreeList;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPrinting;



namespace ERPSale
{
    public partial class UI售后信息查询 : UserControl
    {

        DataTable dt_物料;
        DataTable dtM_显示;
        /// <summary>
        /// 标志位
        /// </summary>
        int flag = 0;
        /// <summary>
        /// 输入的物料编码
        /// </summary>
        string strCpID = "";       
        string strconn = CPublic.Var.strConn;
        DataTable dt_存码;
        DataTable dt_加载录入表;
        string cfgfilepath = "";
        public UI售后信息查询()
        {
            InitializeComponent();
        }

        private void UI售后信息查询_Load(object sender, EventArgs e)
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(splitContainer3, this.Name, cfgfilepath);

                using (SqlDataAdapter da = new SqlDataAdapter("select 产品编码 as 物料编码 from 基础数据物料BOM表 where 1<>1", strconn))
                {
                    dt_存码 = new DataTable();
                    da.Fill(dt_存码);

                }

                fun_物料();

                //自动换行
                //RepositoryItemMemoEdit repoMemo = new RepositoryItemMemoEdit();
                //repoMemo.WordWrap = true;
                //repoMemo.AutoHeight = true;
                //this.gc.RepositoryItems.Add(repoMemo);
                //gv.Columns[3].ColumnEdit = repoMemo;
                //gv.Columns[11].ColumnEdit = repoMemo;
                //gv.Columns[10].ColumnEdit = repoMemo;
                //gv.Columns[9].ColumnEdit = repoMemo;
                //gv.Columns[12].ColumnEdit = repoMemo;
                //gv.OptionsView.RowAutoHeight = true;

                //默认时间
                DateTime dtime = CPublic.Var.getDatetime();
                dtime = new DateTime(dtime.Year, dtime.Month, dtime.Day);
                dateEdit1.EditValue = dtime.AddMonths(-1);
                dateEdit2.EditValue = dtime.AddDays(1).AddSeconds(-1);
                //服务类型

                fun_加载人员信息();

                //panel4.Visible = false;
                //panel2.Visible = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //绑定人员信息
        private void fun_加载人员信息()
        {
            try
            {
                string str = "select 员工号,姓名 from 人事基础员工表";
                using (SqlDataAdapter da = new SqlDataAdapter(str, strconn))
                {
                    DataTable dt_人员 = new DataTable();
                    da.Fill(dt_人员);
                    searchLookUpEdit1.Properties.DataSource = dt_人员;
                    searchLookUpEdit1.Properties.DisplayMember = "姓名";
                    searchLookUpEdit1.Properties.ValueMember = "姓名";
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

         
        }

        //查询
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                gc.DataSource =null;
                // 通过物料查询
                if (comboBox2.Text.ToString() == "查询物料")
                {
                    //cb_物料.Visible = true;
                    //comboBox1.Visible = false;
                    //label4.Text = "选择物料";
                    if (cb_物料.Text.ToString() == "")
                    {
                        throw new Exception("请选择物料！");
                    }
                    if (checkBox3.Checked== true)
                    {
                        if (searchLookUpEdit1.Text=="")
                       {
                           throw new Exception("请选信息来员！");
                       }
                    }
                    if (checkBox1.Checked == true)
                    {
                        if (checkedComboBoxEdit1.Text == "")
                        {
                            throw new Exception("请选择只是分类！");
                        }
                    }
                    fun_物料();

                    flag = 0;
                    this.strCpID = cb_物料.EditValue.ToString();
                



                     fun_BOM子图详细();
                    

                     {
                         
                     }
                }
                //通过类型查询 
             if(comboBox2.Text.ToString() == "知识类型")
             {
                // if (comboBox1.Text=="")
                //{
                //    throw new Exception("请选择知识类型");
                //}
                 string str = string.Format("select 服务类型,sum(数量)as 数量 from 知识平台录入表 where 审核=1 and 录入时间>='{0}'and 录入时间<='{1}' group by 服务类型", dateEdit1.EditValue, dateEdit2.EditValue);
                 using(SqlDataAdapter da= new SqlDataAdapter(str,strconn))
                 {
                     DataTable dt_类型显示 = new DataTable();
                     da.Fill(dt_类型显示);
                     gc1.DataSource = dt_类型显示;
                 }

              }



                if (checkBox6.Checked == true)
                {
                    if (textBox1.Text == "")
                    {
                        throw new Exception("请填写关键字");
                    }
                   // DataRow[] dr = dt_加载录入表.Select(string.Format("售后原因 like '%{0}%'  and 录入时间>='{1}'and 录入时间<='{2}'", textBox1.Text.ToString(), dateEdit1.EditValue, dateEdit2.EditValue));
                    string st = string.Format("select * from 知识平台录入表 where 状况描述 like '%{0}%' and 审核=1  and 录入时间>='{1}'and 录入时间<='{2}'", textBox1.Text.ToString(), dateEdit1.EditValue, dateEdit2.EditValue);
                    using (SqlDataAdapter da = new SqlDataAdapter(st, strconn))
                    {
                        DataTable dt_加载关键字为条件的原因描述 = new DataTable();
                        da.Fill(dt_加载关键字为条件的原因描述);
                        gc.DataSource = dt_加载关键字为条件的原因描述;
                    }         
               }

               
                //知识分类
                //if (checkBox1.Checked == true && cb_物料.EditValue.ToString() != "" && checkBox3.Checked == false)
                //{
                //    if (checkedComboBoxEdit1.Text == "")
                //    {
                //        throw new Exception("请选择知识分类");
                //    }
                //    using (SqlDataAdapter da = new SqlDataAdapter("select * from 知识平台录入表 where 原因分类='" + checkedComboBoxEdit1.Text + "'and 产品编码 ='" + cb_物料.EditValue.ToString() + "'", strconn))
                //    {
                //        DataTable dt_通过分类显示 = new DataTable();
                //        da.Fill(dt_通过分类显示);
                //        gc.DataSource = dt_通过分类显示;
                //    }
                //}
             

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //物料经过勾选框的不同状态
//        private void fun_物料()
//        {
//            try
//            {
//                SqlDataAdapter da;
//                 string sql="";
////                 if (checkBox1.Checked == false && checkBox3.Checked == false)
////                 {
////                     sql = string.Format(@"select a.物料编码,a.物料名称,a.规格型号,a.图纸编号,a.物料类型,c.原因数 from 基础数据物料信息表 a
////                   left join (select sum(数量) as 原因数,产品编码 from [知识平台录入表] where 审核=1 group by 产品编码)c on a.物料编码=c.产品编码 ");//where 物料类型='成品' or 物料类型='半成品'
////                 }
////                 if (checkBox1.Checked == true && checkBox3.Checked == false)
////                 {
////                     sql = string.Format(@"select a.物料编码,a.物料名称,a.规格型号,a.图纸编号,a.物料类型,c.原因数 from 基础数据物料信息表 a
////                   left join (select sum(数量) as 原因数,产品编码 from [知识平台录入表] where 审核=1 and 原因分类='{0}'  group by 产品编码)c on a.物料编码=c.产品编码", checkedComboBoxEdit1.Text.ToString());
////                 }
////                 if (checkBox1.Checked == false && checkBox3.Checked == true)
////                 {
////                     sql = string.Format(@"select a.物料编码,a.物料名称,a.规格型号,a.图纸编号,a.物料类型,c.原因数 from 基础数据物料信息表 a
////                   left join (select sum(数量) as 原因数,产品编码 from [知识平台录入表] where 审核=1 and 信息来员='{0}'  group by 产品编码)c on a.物料编码=c.产品编码", searchLookUpEdit1.EditValue);
////                 }
////                 if (checkBox1.Checked == true && checkBox3.Checked == true)
////                 {
////                     sql = string.Format(@"select a.物料编码,a.物料名称,a.规格型号,a.图纸编号,a.物料类型,c.原因数 from 基础数据物料信息表 a
////                   left join (select sum(数量) as 原因数,产品编码 from [知识平台录入表] where 审核=1 and 信息来员='{0}' and 原因分类='{1}'  group by 产品编码)c on a.物料编码=c.产品编码", searchLookUpEdit1.Text.ToString(), checkedComboBoxEdit1.Text.ToString());
////                 }
//                 if (checkBox1.Checked == false && checkBox3.Checked == false)
//                 {
//                     sql = string.Format(@"select sum(数量) as 数量,产品编码 as 物料编码,产品名称 as 物料名称,产品型号 as 规格型号  from [知识平台录入表] where 审核=1 group by 产品编码,产品名称,产品型号 ");//where 物料类型='成品' or 物料类型='半成品'
//                 }
//                 if (checkBox1.Checked == true && checkBox3.Checked == false)
//                 {
//                     sql = string.Format(@"select sum(数量) as 数量,产品编码 as 物料编码,产品名称 as 物料名称,产品型号 as 规格型号 from [知识平台录入表] where 审核=1 and 原因分类='{0}'  group by 产品编码,产品名称,产品型号 ", checkedComboBoxEdit1.Text.ToString());
//                 }
//                 if (checkBox1.Checked == false && checkBox3.Checked == true)
//                 {
//                     sql = string.Format(@"select sum(数量) as 数量,产品编码 as 物料编码,产品名称 as 物料名称,产品型号 as 规格型号 from [知识平台录入表] where 审核=1 and 信息来员='{0}'  group by 产品编码,产品名称,产品型号", searchLookUpEdit1.EditValue);
//                 }
//                 if (checkBox1.Checked == true && checkBox3.Checked == true)
//                 {
//                     sql = string.Format(@"select sum(数量) as 数量,产品编码 as 物料编码,产品名称 as 物料名称,产品型号 as 规格型号  from [知识平台录入表] where 审核=1 and 信息来员='{0}' and 原因分类='{1}'  group by 产品编码,产品名称,产品型号", searchLookUpEdit1.Text.ToString(), checkedComboBoxEdit1.Text.ToString());
//                 }

//                da = new SqlDataAdapter(sql, strconn);
//                dt_物料 = new DataTable();
//                da.Fill(dt_物料);
//                //cb_物料.Properties.DataSource = dt_物料;
//                //cb_物料.Properties.DisplayMember = "物料编码";
//                //cb_物料.Properties.ValueMember = "物料编码";
//            }
//            catch (Exception ex)
//            {
//                CZMaster.MasterLog.WriteLog(ex.Message + " fun_物料()");
//                throw new Exception(ex.Message);
//            }
//        }
        private void fun_物料()
        {
            try
            {
                SqlDataAdapter da;
                string sql = string.Format(@"select 产品编码 as 物料编码,产品名称 as 物料名称,产品型号 as 规格型号,sum(数量) as 数量,信息来员,原因分类,操作人员  from [知识平台录入表] where 审核=1 and 产品编码<>'' group by 产品编码,产品名称,产品型号,信息来员,原因分类,操作人员 ");//where 物料类型='成品' or 物料类型='半成品';
               
                da = new SqlDataAdapter(sql, strconn);
                dt_物料 = new DataTable();
                da.Fill(dt_物料);
                cb_物料.Properties.DataSource = dt_物料;
                cb_物料.Properties.DisplayMember = "物料编码";
                cb_物料.Properties.ValueMember = "物料编码";
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_物料()");
                throw new Exception(ex.Message);
            }
        }
        private void fun_BOM子图详细()
        {
            try
            {
               
              tv.ClearNodes();          
              newfun_tree();                          
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }
        DataTable dt1;
        private void newfun_tree()
        {

            //dt1 = ERPorg.Corg.get_u8bom(cb_物料.EditValue.ToString());
            string sql_bom = string.Format(@" with temp_bom(产品编码,子项编码,仓库号,仓库名称,wiptype,子项类型,数量,bom类型,bom_level ) as
 (select  产品编码,子项编码,仓库号,仓库名称,WIPType,子项类型,数量,bom类型,1 as level from 基础数据物料BOM表 
   where 产品编码='{0}'
   union all 
   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型,a.数量,a.bom类型,b.bom_level+1  from 基础数据物料BOM表 a
   inner join temp_bom b on a.产品编码=b.子项编码 
   ) 
      select  产品编码,fx.物料名称 as  产品名称,子项编码,base.物料名称 as 子项名称,wiptype,子项类型,数量,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号 as 子项规格 from  temp_bom a
  left join 基础数据物料信息表 base  on base.物料编码=a.子项编码
  left join 基础数据物料信息表  fx  on fx.物料编码=a.产品编码
  group by 产品编码,fx.物料名称 ,子项编码,base.物料名称 ,wiptype,子项类型,数量,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号", cb_物料.EditValue.ToString());
            dt1 = CZMaster.MasterSQL.Get_DataTable(sql_bom, strconn);
            DataRow[] r = dt_物料.Select(string.Format("物料编码='{0}'", cb_物料.EditValue.ToString()));
            TreeListNode head = tv.AppendNode(new object[] { r[0]["物料编码"] }, null);
            //主节点给值
            head.SetValue("产品编码结构", r[0]["物料编码"].ToString());
            //head.SetValue("物料编号", r[0]["物料编码"].ToString());
            head.SetValue("产品名称", r[0]["物料名称"].ToString());
            //head.SetValue("图纸编号", r[0]["图纸编号"].ToString());
            head.SetValue("规格型号", r[0]["规格型号"].ToString());
            head.SetValue("数量", r[0]["数量"].ToString());
            
            head.Tag = r[0];
            fun_TL(head, r[0]["物料编码"].ToString());
           head.ExpandAll();

        }
        /// <summary>
        /// 展开子节点
        /// </summary>
        /// <param name="n"></param>
        /// 
       
       // DataTable dt;
        private void fun_TL(TreeListNode n, string str_fx)
        {
           try
            {
                //string str ="";
              



//                string s ="";
//             dt = new DataTable();
//             if (checkBox1.Checked == false && checkBox3.Checked == false )
//                {
//                 s = string.Format(@" select  a.物料编码 as 父项编号,a.物料类型 as 父项类型,a.大类 as 父项大类,a.小类 as 父项小类,a.规格型号 as 父项规格 ,b.物料编码 as 子项编码,b.物料名称 as 子项名称,
//            b.物料类型 as 子项类型,b.图纸编号 as 子项图号 ,b.大类 as 子项大类,b.小类 as 子项小类,b.规格型号 as 子项规格 ,c.原因数 from 基础数据物料BOM表  base 
//            left join 基础数据物料信息表 a  on base.产品编码=a.物料编码
//            left join 基础数据物料信息表 b  on base.子项编码=b.物料编码
//            left join ( select sum(数量) as 原因数,产品编码 from [知识平台录入表]  where 审核=1 group by 产品编码 )c on base.子项编码=c.产品编码  where 子项类型<>'采购件' and  a.物料编码='{0}'", str_fx);
//                }
                    //在物料查询中只查知识分类
//               if (checkBox1.Checked == true && checkBox3.Checked == false )
//            {
//                 s = string.Format(@" select  a.物料编码 as 父项编号,a.物料类型 as 父项类型,a.大类 as 父项大类,a.小类 as 父项小类,a.规格型号 as 父项规格 ,b.物料编码 as 子项编码,b.物料名称 as 子项名称,
//             b.物料类型 as 子项类型,b.图纸编号 as 子项图号 ,b.大类 as 子项大类,b.小类 as 子项小类,b.规格型号 as 子项规格 ,c.原因数 from 基础数据物料BOM表  base 
//            left join 基础数据物料信息表 a  on base.产品编码=a.物料编码
//            left join 基础数据物料信息表 b  on base.子项编码=b.物料编码
//            left join (select sum(数量) as 原因数,产品编码 from [知识平台录入表] where 原因分类 ='{1}'and 审核=1 group by 产品编码)c on base.子项编码=c.产品编码  where 子项类型<>'采购件' and  a.物料编码='{0}'", str_fx,checkedComboBoxEdit1.Text.ToString());

//            }
//               if (checkBox1.Checked == false  && checkBox3.Checked == true )
//               {
//                   s = string.Format(@" select  a.物料编码 as 父项编号,a.物料类型 as 父项类型,a.大类 as 父项大类,a.小类 as 父项小类,a.规格型号 as 父项规格 ,b.物料编码 as 子项编码,b.物料名称 as 子项名称,
//             b.物料类型 as 子项类型,b.图纸编号 as 子项图号 ,b.大类 as 子项大类,b.小类 as 子项小类,b.规格型号 as 子项规格 ,c.原因数 from 基础数据物料BOM表  base 
//            left join 基础数据物料信息表 a  on base.产品编码=a.物料编码
//            left join 基础数据物料信息表 b  on base.子项编码=b.物料编码
//            left join (select sum(数量) as 原因数,产品编码 from [知识平台录入表] where 信息来员 ='{1}'and 审核=1 group by 产品编码)c on base.子项编码=c.产品编码  where 子项类型<>'采购件' and  a.物料编码='{0}'",str_fx,searchLookUpEdit1.EditValue);

//               }
//               if (checkBox1.Checked == true && checkBox3.Checked == true)
//               {
//                   s = string.Format(@" select  a.物料编码 as 父项编号,a.物料类型 as 父项类型,a.大类 as 父项大类,a.小类 as 父项小类,a.规格型号 as 父项规格 ,b.物料编码 as 子项编码,b.物料名称 as 子项名称,
//             b.物料类型 as 子项类型,b.图纸编号 as 子项图号 ,b.大类 as 子项大类,b.小类 as 子项小类,b.规格型号 as 子项规格 ,c.原因数 from 基础数据物料BOM表  base 
//            left join 基础数据物料信息表 a  on base.产品编码=a.物料编码
//            left join 基础数据物料信息表 b  on base.子项编码=b.物料编码
//            left join (select sum(数量) as 原因数,产品编码 from [知识平台录入表] where 信息来员 ='{1}'and 原因分类 ='{2}'and 审核=1 group by 产品编码)c on base.子项编码=c.产品编码  where 子项类型<>'采购件' and  a.物料编码='{0}'", str_fx, searchLookUpEdit1.EditValue, str_fx, checkedComboBoxEdit1.EditValue);

//               }  
                //using (SqlDataAdapter da = new SqlDataAdapter(s,strconn))
                //{
                //    da.Fill(dt);
                //}

                DataRow[] drr = dt1.Select(string.Format("产品编码='{0}'", str_fx));


                foreach (DataRow r in drr)
                {   
                
                    TreeListNode nc = tv.AppendNode(new object[] { r["子项编码"].ToString() }, n);
                    nc.SetValue("产品编码结构", r["子项编码"].ToString());
                   // nc.SetValue("子项类型", r["子项类型"].ToString());
                    //nc.SetValue("物料编号", r["子项编号"].ToString());
                    nc.SetValue("产品名称", r["子项名称"].ToString());
                    nc.SetValue("规格型号", r["子项规格"].ToString());
                    //nc.SetValue("规格型号", r["子项规格"].ToString());
                    DataRow[] rr= new DataRow[0]; 
                    if (checkBox3.Checked == false && checkBox1.Checked == false)
                    {
                     rr = dt_物料.Select(string.Format("物料编码='{0}'",r["子项编码"].ToString()));
                    }
                    if (checkBox3.Checked == true && checkBox1.Checked == false)
                    {
                        rr = dt_物料.Select(string.Format("信息来员='{0}'and 物料编码='{1}'", searchLookUpEdit1.EditValue,r["子项编码"].ToString()));
                     
                    }
                    if (checkBox3.Checked == false && checkBox1.Checked == true)
                    {
                        rr = dt_物料.Select(string.Format("原因分类='{0}'and 物料编码='{1}'", checkedComboBoxEdit1.Text.ToString(), r["子项编码"].ToString()));
                    }
                    if (checkBox3.Checked == true && checkBox1.Checked == true)
                    {
                        rr = dt_物料.Select(string.Format("原因分类='{0}'and 信息来员='{1}'and 物料编码='{2}'",checkedComboBoxEdit1.Text.ToString(), searchLookUpEdit1.EditValue.ToString(),r["子项编码"].ToString()));
                    }
                    if(rr.Length > 0)
                    {
                    string dd = rr[0]["数量"].ToString();
                    nc.SetValue("数量",dd);
                     }
                    //nc.SetValue("图纸编号", r["子项图号"].ToString());
                    //nc.SetValue("数量", r["数量"].ToString());
                    nc.Tag = r;
                    fun_TL(nc, r["子项编码"].ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
      
        private void fun_存储编码(TreeListNode n)
        {           
                DataRow dr_1 = dt_存码.NewRow();
               dr_1["物料编码"] = n.GetValue("产品编码结构").ToString();
               dt_存码.Rows.Add(dr_1);
        }


        
        private void fun_加载主子节点物料()
        {
            //获取当前行
          //   DataRow dr = tv.Selection[0].Tag as DataRow;
             tv.CloseEditor();
             this.BindingContext[dt_物料].EndCurrentEdit();
            // this.BindingContext[dt].EndCurrentEdit();
             if (tv.Selection[0] == null) return;
             //给DATAROW值
             //DataRow rr;
             //rr = tv.Selection[0].Tag as DataRow;//选择行
             TreeListNode tr = tv.Selection[0]; //选择节点
             fun_存储编码(tr);
             fun_tl(tr);           
        }

        private void fun_tl( TreeListNode tr)
        {
            foreach (TreeListNode n in tr.Nodes)
            {            
                fun_存储编码(n);
                if (n.HasChildren == true)
                {
                    fun_存储编码(n);
                    fun_tl(n);
                }
            }
        }
      

      
       

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        //打印
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {      
          // dtM_显示
            if (MessageBox.Show("是否打印", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {                         
                Dowork();            
            }
        }

        private void Dowork()
        {
           DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
           string str_打印机 = "";
            ItemInspection.print_FMS.fun_P_知识平台打印(dr,str_打印机,false);   
        }
        //导出
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {


                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions(TextExportMode.Text, false, false);

                gc.ExportToXlsx(saveFileDialog.FileName, options);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}
            }
        }
        //预览
        private void 预览ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            知识平台文件上传 fm = new 知识平台文件上传(dr);
            fm.ShowDialog();


      }

        private void gc_MouseClick(object sender, MouseEventArgs e)
        {
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc, new Point(e.X, e.Y));
            }
        }

        private void gv2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
            string str = string.Format("select * from 知识平台录入表  where 审核='1' and 服务类型='{0}' and 录入时间>='{1}'and 录入时间<='{2}'", dr["服务类型"].ToString(), dateEdit1.EditValue, dateEdit2.EditValue);
            using(SqlDataAdapter da = new SqlDataAdapter(str,strconn))
            {
                DataTable dtr = new DataTable();
                da.Fill(dtr);
                gc.DataSource = dtr;
            }

        }

        private void tv_MouseClick_1(object sender, MouseEventArgs e)
        {

            try
            {
                if (dt_存码 != null)
                {
                    dt_存码.Clear();
                }
                fun_加载主子节点物料();
                dtM_显示 = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter("select * from 知识平台录入表 where 1<>1", strconn))
                {
                    da.Fill(dtM_显示);
                }
                string str_加载 = "";
               //按照知识分类查
                if (checkBox1.Checked == true && checkBox3.Checked == false && checkBox6.Checked == false)
                {
                    str_加载 = string.Format("select * from 知识平台录入表 where 审核=1 and 录入时间>='{0}'and 录入时间<='{1}' and 原因分类='{2}'", dateEdit1.EditValue, dateEdit2.EditValue, checkedComboBoxEdit1.Text.ToString());
                }
                //单独物料查
                if(checkBox1.Checked == false && checkBox3.Checked == false && checkBox6.Checked == false)
                {
                str_加载 = string.Format("select * from 知识平台录入表 where 审核=1 and 录入时间>='{0}'and 录入时间<='{1}'",dateEdit1.EditValue, dateEdit2.EditValue);
                }
                //单独信息来员
                if (checkBox1.Checked == false && checkBox3.Checked == true && checkBox6.Checked == false)
                {
                    str_加载 = string.Format("select * from 知识平台录入表 where 审核=1 and 录入时间>='{0}'and 录入时间<='{1}' and 信息来员='{2}'", dateEdit1.EditValue, dateEdit2.EditValue, searchLookUpEdit1.EditValue);
                }
                ////单独关键字
                //if (checkBox1.Checked == true && checkBox3.Checked == false && checkBox6.Checked == true)
                //{
                //    str_加载 = string.Format("select * from 知识平台录入表 where 审核=1 and 录入时间>='{0}'and 录入时间<='{1}' and 信息来员='{2}'", dateEdit1.EditValue, dateEdit2.EditValue, searchLookUpEdit1.EditValue);
                //}
                //信息来员 和知识分类
                if (checkBox1.Checked == true && checkBox3.Checked == true && checkBox6.Checked == false)
                {
                    str_加载 = string.Format("select * from 知识平台录入表 where 审核=1 and 录入时间>='{0}'and 录入时间<='{1}' and 信息来员='{2}' and 原因分类='{3}'", dateEdit1.EditValue, dateEdit2.EditValue, searchLookUpEdit1.EditValue, checkedComboBoxEdit1.Text.ToString());
                }

                using (SqlDataAdapter da1 = new SqlDataAdapter(str_加载, strconn))
                {
                    dt_加载录入表 = new DataTable();
                    da1.Fill(dt_加载录入表);

                }
                DataView dv = dt_存码.DefaultView;

               // dv.Sort = "产品编号 Asc";//升序

                dt_存码 = dv.ToTable(true);//去重

                //查出节点和子节点对应的录入原因
                foreach (DataRow dr in dt_存码.Rows)
                {
                    //string str1 = 
                    DataRow[] r = dt_加载录入表.Select(string.Format("产品编码='{0}'", dr["物料编码"].ToString()));
                    //string str = r[0]["物料编码"].ToString();
                    foreach (DataRow r1 in r)
                    {
                        DataRow dr_a = dtM_显示.NewRow();
                        dr_a["产品编码"] = r1["产品编码"].ToString();
                        dr_a["产品名称"] = r1["产品名称"].ToString();
                        dr_a["产品型号"] = r1["产品型号"].ToString();
                        dr_a["变更点"] = r1["变更点"].ToString();
                        dr_a["改善方法"] = r1["改善方法"].ToString();
                        dr_a["不良反应"] = r1["不良反应"].ToString();
                        dr_a["状况描述"] = r1["状况描述"].ToString();
                        dr_a["服务类型"] = r1["服务类型"].ToString();
                        dr_a["数量"] = Convert.ToDecimal(r1["数量"].ToString());
                        dr_a["录入时间"] = r1["录入时间"].ToString();
                        dr_a["售后单号"] = r1["售后单号"].ToString();
                        dr_a["信息来员"] = r1["信息来员"].ToString();
                        dr_a["原因分类"] = r1["原因分类"].ToString();
                        dr_a["部门编号"] = r1["部门编号"].ToString();
                        dr_a["部门名称"] = r1["部门名称"].ToString();
                        dr_a["问题主因"] = r1["问题主因"].ToString();
                        dr_a["知识点主题"] = r1["知识点主题"].ToString();
                        dr_a["操作人员"] = r1["操作人员"].ToString();
                        dtM_显示.Rows.Add(dr_a);
                    }
                }
                gc.DataSource = dtM_显示;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

     

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            comboBox2.Text = null;
            cb_物料.Text = null;
            checkBox3.Checked = false;
            searchLookUpEdit1.Text = null;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
           // fun_物料();
            //comboBox2.Text = null;
            //cb_物料.Text = null;
            //checkBox6.Checked = false;
            //textBox1.Text = null;
        }

        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            checkBox1.Checked = false;
            checkBox6.Checked = false;
            checkBox3.Checked = false;
            searchLookUpEdit1.Text = null;
            textBox1.Text = null;
            cb_物料.Text = null;
        }

        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if(dr!= null)
                {
                    string sql1 = string.Format("select * from 知识平台文件上传表 where 售后单号='{0}'", dr["售后单号"].ToString());
                    using (SqlDataAdapter da = new SqlDataAdapter(sql1, strconn))
                    {
                        DataTable dt1 = new DataTable();
                        da.Fill(dt1);
                        gcM1.DataSource = dt1;
                    }



                }
                if (e.Clicks == 2 && e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    UI售后信息录入 fm = new UI售后信息录入(dr,true);
                    fm.Dock = System.Windows.Forms.DockStyle.Fill;
                    CPublic.UIcontrol.AddNewPage(fm, "信息录入");
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




        }
        //预览文件
        string strConn_FS = CPublic.Var.geConn("FS");
        private void 预览ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
               
             DataRow drr = gvM1.GetDataRow(gvM1.FocusedRowHandle);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            string strcoo_路径 = "C:\\Users\\Administrator\\Desktop\\下载";        
            string fileName = strcoo_路径 + "\\" + drr["表单名称"].ToString();           
            // string strcoo_路径 = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\PDFTMP";
            saveFileDialog.Title = "下载文件"; 
            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*|图片文件|*.bmp;*.jpg;*.jpeg;*.gif;*.png";

           CFileTransmission.CFileClient.strCONN = strConn_FS;
            CFileTransmission.CFileClient.Receiver(drr["文件GUID"].ToString(), fileName);
            //预览
            System.Diagnostics.Process.Start(fileName);      
        }

        private void gv_ColumnFilterChanged(object sender, EventArgs e)
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gv.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void gv_ColumnPositionChanged(object sender, EventArgs e)
        {
            try
            {
           if (cfgfilepath != "")
                {
                    gv.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        

      

       
    }
}
