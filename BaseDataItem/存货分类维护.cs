using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System.Data.OleDb;


namespace BaseData
{
    public partial class 存货分类维护 : UserControl
    {
        public 存货分类维护()
        {
            InitializeComponent();
        }


        #region 私有成员
        DataTable dtM,dt_数据;
        SqlDataAdapter daM;
        SqlCommandBuilder brM;
      
        string strcon = CPublic.Var.strConn;
        TreeListNode nc;
        #endregion
     
           
        private void 存货分类维护_Load(object sender, EventArgs e)
        {         
          try
          {     
 
              Init();
               dt_数据 = dtM.Clone();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region  其他数据的处理
  
        private void fun_Check()
        {
            tv.DataSource = dtM;
            tv.PopulateColumns();
            tv.Columns.ColumnByFieldName("GUID").Visible = false;
            tv.Columns.ColumnByFieldName("上级类型GUID").Visible = false;
            foreach (DataRow r in dtM.Rows)
            {
                if (r.RowState == DataRowState.Deleted)
                {

                    continue;
                }

                if(r["GUID"].Equals("")){
                    r["GUID"] = Guid.NewGuid();
                }
                
                if (r.RowState == DataRowState.Deleted) continue;

                if (r["存货分类编码"].ToString() == "")
                {
                    throw new Exception("存货分类编码不能为空，请检查！");
                }

                DataRow[] dr = dtM.Select(string.Format("存货分类编码='{0}'", r["存货分类编码"].ToString()));
                if (dr.Length >= 2)
                {
                    throw new Exception(string.Format("存货分类编码\"{0}\"有重复，请检查！", r["存货分类编码"].ToString()));
                }
                if (r["层级"].ToString() == "")
                {
                    throw new Exception("层级不能为空");
                }
                if (r["存货分类名称"].ToString() == "")
                {
                    throw new Exception("存货分类名称不能为空，请检查！");
                }
              
            }
        }

        #endregion

        #region 数据库的读取与保存
  
        private void Init(TreeListNode n)
        {      
            string sqlstr = "上级类型GUID = '{0}'";
            sqlstr = string.Format(sqlstr, (n.Tag as DataRow)["GUID"].ToString());
            DataRow[] rs = dtM.Select(sqlstr, "存货分类编码");
            foreach (DataRow r in rs)
            {
                TreeListNode nc = tv.AppendNode(new object[] { r["GUID"].ToString() }, n);

                nc.SetValue("存货分类编码", r["存货分类编码"].ToString());
                nc.SetValue("存货分类名称", r["存货分类名称"].ToString());
                nc.SetValue("层级", r["层级"].ToString());
                nc.SetValue("是否末级", Convert.ToBoolean(r["是否末级"]));
       
                nc.Tag = r;
                Init(nc);
            }
        }

   
        private void Init()
        {
            string sqlstr;
            sqlstr = "select * from 基础数据存货分类表 order by 存货分类编码  ";
            daM = new SqlDataAdapter(sqlstr, CPublic.Var.strConn);
            brM = new SqlCommandBuilder(daM);
            dtM = new DataTable();
            daM.Fill(dtM);

            DataRow[] t = dtM.Select("上级类型GUID = ''");
            foreach (DataRow r in t)
            {
                TreeListNode n = tv.AppendNode(new object[] { r["GUID"].ToString() }, null);             
                n.SetValue("存货分类编码", r["存货分类编码"].ToString());
                n.SetValue("存货分类名称", r["存货分类名称"].ToString());
                n.SetValue("层级", r["层级"].ToString());
                n.SetValue("是否末级", Convert.ToBoolean(r["是否末级"]));
                n.Tag = r;
                Init(n);
            }
        }

    
      
        private void fun_添加下级()
        {
            if (tv.Nodes.Count > 0)
            {
                if (tv.Selection[0] == null) return;
            }
            else
            {
                return;
            }
            TreeListNode n = tv.Selection[0];     
            TreeListNode nc = tv.AppendNode(new object[] { "" }, n);
          //  nc.SetValue("层级", "");
            DataRow r = dtM.NewRow();
            r["GUID"] = System.Guid.NewGuid();
            dtM.Rows.Add(r);
            nc.Tag = r;
            if (n.GetDisplayText("") == "物料结构")
            {
                r["上级类型GUID"] = "";
            }
            else
            {
                r["上级类型GUID"] = (n.Tag as DataRow)["GUID"].ToString();
            }
         
          //  n.ExpandAll();
        }

    
        private void fun_添加同级()
        {
            if (tv.Nodes.Count > 0)
            {
                if (tv.Selection[0] == null)
                {
                    return;
                }
                else
                {
                    //if (tv.Selection[0].ParentNode == null) return;
                }
            }
            TreeListNode n;
            if (tv.Selection[0] == null || tv.Selection[0].ParentNode == null)
            {
                n = null;
            }
            else
            {
                n = tv.Selection[0].ParentNode;
            }
            TreeListNode nc = tv.AppendNode(new object[] { "" }, n);
          //  nc.SetValue("层级", "");
            DataRow r = dtM.NewRow();
            r["GUID"] = System.Guid.NewGuid();
            dtM.Rows.Add(r);
            nc.Tag = r;
 

            if (n == null)
            {
                r["上级类型GUID"] = "";
            }
            else
            {
                r["上级类型GUID"] = (n.Tag as DataRow)["GUID"].ToString();
                //n.ExpandAll();
            }

        }

        //保存界面数据
        private void saveDataFromNode()
        {
            if (tv.Selection[0] == null) return;
            //tv.Selection[0].SetValue("物料结构", tv.Selection[0].GetValue("存货分类名称"));
            //给DATAROW值
            DataRow r;
            r = tv.Selection[0].Tag as DataRow;
            r["存货分类编码"] = tv.Selection[0].GetValue("存货分类编码");
            r["存货分类名称"] = tv.Selection[0].GetValue("存货分类名称");
            if (tv.Selection[0].ParentNode == null)   //如果是顶层节点，类型级别就一定是大类
            {
                r["层级"] = "1";
            }
            else
            {
                r["层级"] = tv.Selection[0].GetValue("层级");
            }

            r["是否末级"] = tv.Selection[0].GetValue("是否末级");
     
            #region
            //if (tv.Selection[0].ParentNode == null)  //表示这个是根节点:如果是根节点的话，类型级别是大类
            //{
            //    r["类型级别"] = "大类"; 
            //}
            //if (tv.Selection[0].ParentNode!=null && tv.Selection[0].GetValue("类型级别").ToString().Equals(""))  //非根节点，用户没有选择，则类型级别是小类
            //{
            //    r["类型级别"] = "小类";
            //}
            //if (tv.Selection[0].ParentNode != null && !tv.Selection[0].GetValue("类型级别").ToString().Equals(""))  //按照用户的选择
            //{
            //    r["类型级别"] = tv.Selection[0].GetValue("类型级别");
            //}

            //foreach (TreeListNode n in tv.Selection[0].Nodes)
            //{
            //    r = n.Tag as DataRow;
            //    r["上级类型GUID"] = n.ParentNode.GetValue("物料类型GUID");
            //}
            #endregion
        }

        private void deleteDept(TreeListNode n)
        {
            foreach (TreeListNode nc in n.Nodes)
            {
                deleteDept(nc);
            }
            (n.Tag as DataRow).Delete();
        }
        #endregion

        #region 界面操作


        private void tv_HiddenEditor(object sender, EventArgs e)
        {
            try
            {
                saveDataFromNode();
            }
            catch (Exception ex)
            {
                //  throw ex;
                //MessageBox.Show(ex.Message);
            }
        }


        //界面刷新
        private void simpleButton5_Click(object sender, EventArgs e)
        {
            try
            {
                tv.ClearNodes();
            
                Init();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion


        private void simpleButton5_Click_1(object sender, EventArgs e)
        {
            try
            {
                tv.ClearNodes();
             
                Init();
                tv.Columns.ColumnByFieldName("GUID").Visible = false;
                tv.Columns.ColumnByFieldName("上级类型GUID").Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton2_Click_1(object sender, EventArgs e)
        {
            try
            {
                fun_添加同级();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }//同级

        private void simpleButton3_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (tv.Selection[0] == null) return;
                if (MessageBox.Show(string.Format("如果删除物料\"{0}\"，那么它的下级物料都将删除，你确定要删除吗？", tv.Selection[0].GetValue("物料类型名称")), "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    deleteDept(tv.Selection[0]);
                    if (tv.Selection[0].ParentNode == null)
                    {
                        tv.Selection[0].Nodes.Remove(tv.Selection[0]);
                    }
                    else
                    {
                        tv.Selection[0].ParentNode.Nodes.Remove(tv.Selection[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }//删除

        private void simpleButton4_Click_1(object sender, EventArgs e)
        {
            try
            {
                fun_Check();
                string sql = "select  *  from 基础数据存货分类表  where 1<>1";
                using (SqlDataAdapter da  =new SqlDataAdapter(sql,strcon) ){
                    new SqlCommandBuilder(da);
                    da.Update(dtM);               
                }
                tv.Columns.ColumnByFieldName("GUID").Visible = false;
                tv.Columns.ColumnByFieldName("上级类型GUID").Visible = false;

                MessageBox.Show("保存成功！");
              simpleButton5_Click_1(null,null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                simpleButton5_Click_1(null, null);
            }
        }

        private void simpleButton6_Click_1(object sender, EventArgs e)
        {
    
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
               
                    this.tv.ExportToXlsx(saveFileDialog.FileName);
            
                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);


                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
                MessageBox.Show(ex.Message);
            }
        

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_添加下级();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tv_HiddenEditor_1(object sender, EventArgs e)
        {
            try
            {
                saveDataFromNode();
            }
            catch (Exception ex)
            {
                //  throw ex;
                //MessageBox.Show(ex.Message);
            }

        }

        private void tv_HiddenEditor_2(object sender, EventArgs e)
        {

        }

        int a = 0;
        int b = 0;
        int c = 0;
        int dw = 0;
        int eqw = 0;
       
      //  DataRow [] dr_数据=new DataRow[4];
        private void simpleButton7_Click(object sender, EventArgs e)
        {

            try
            {

                输入框 fm = new 输入框();
                fm.ShowDialog();
                DataTable dtM_fu2 = ImportExcelToDataTable2(fm.a.ToString());
                dtM_fu2.Columns.Add("GUID");
                dtM_fu2.Columns.Add("上级类型GUID");
                dtM_fu2.Columns.Add("是否末级");
                //int i=0;
                dtM_fu2.Columns["cInvCCode"].ColumnName = "存货分类编码";
                dtM_fu2.Columns["cInvCName"].ColumnName = "存货分类名称";

                DataTable dtM_fu = new DataTable();
                dtM_fu = dtM_fu2.Clone();

                foreach (  DataRow   d3 in dtM_fu2.Rows ){
                    DataRow d4 = dtM_fu.NewRow();
                    dtM_fu.Rows.Add(d4);
                    d4["存货分类编码"] = d3["存货分类编码"];
                    d4["存货分类名称"] = d3["存货分类名称"];
                    d4["层级"] = d3["层级"];     
                }


                for (int i = 0; i < dtM_fu.Rows.Count; i++)
                {
                    DataRow dr = dtM_fu.Rows[i];//当前行
                    dr["GUID"] = Guid.NewGuid();
                    int a = i;
                    if (i == 0)
                    {
                        a = 1;
                    }
                    else
                    {
                        a = i;
                    }
                    DataRow drr = dtM_fu.Rows[a - 1];//上一行
                    if (i > 3 && dr["层级"].Equals("1"))
                    {
                        dt_数据.Clear();
                        a = 0;
                        b = 0;
                        c = 0;
                        dw = 0;
                        eqw = 0;
                    }
                    if (dr["层级"].Equals("1") && a == 0)
                    {
                        DataRow d = dt_数据.NewRow();
                        dt_数据.Rows.Add(d);
                        d["GUID"] = dtM_fu.Rows[i]["GUID"];
                        d["存货分类编码"] = dtM_fu.Rows[i]["存货分类编码"];
                        d["存货分类名称"] = dtM_fu.Rows[i]["存货分类名称"];
                        d["层级"] = dtM_fu.Rows[i]["层级"];
                        d["是否末级"] = dtM_fu.Rows[i]["是否末级"];
                        d["上级类型GUID"] = "";
                        dr["上级类型GUID"] = "";
                        a = 1;
                    }
                    if (dr["层级"].Equals("2") && b == 0)
                    {
                        DataRow d = dt_数据.NewRow();
                        dt_数据.Rows.Add(d);
                        d["GUID"] = dtM_fu.Rows[i]["GUID"];
                        d["存货分类编码"] = dtM_fu.Rows[i]["存货分类编码"];
                        d["存货分类名称"] = dtM_fu.Rows[i]["存货分类名称"];
                        d["层级"] = dtM_fu.Rows[i]["层级"];
                        d["是否末级"] = dtM_fu.Rows[i]["是否末级"];
                        if (d["上级类型GUID"].ToString() == "")
                        {
                            dr["上级类型GUID"] = drr["GUID"].ToString();
                            d["上级类型GUID"] = drr["GUID"].ToString();
                        }
                        b = 1;
                    }

                    if (dr["层级"].Equals("3") && c == 0)
                    {
                        DataRow d = dt_数据.NewRow();
                        dt_数据.Rows.Add(d);
                        d["GUID"] = dtM_fu.Rows[i]["GUID"];
                        d["存货分类编码"] = dtM_fu.Rows[i]["存货分类编码"];
                        d["存货分类名称"] = dtM_fu.Rows[i]["存货分类名称"];
                        d["层级"] = dtM_fu.Rows[i]["层级"];
                        d["是否末级"] = dtM_fu.Rows[i]["是否末级"];
                        if (d["上级类型GUID"].ToString() == "")
                        {
                            d["上级类型GUID"] = drr["GUID"].ToString();
                            dr["上级类型GUID"] = drr["GUID"].ToString();
                        }
                        c = 1;
                    }

                    if (dr["层级"].Equals("4") && dw == 0)
                    {
                        DataRow d = dt_数据.NewRow();
                        dt_数据.Rows.Add(d);
                        d["GUID"] = dtM_fu.Rows[i]["GUID"];
                        d["存货分类编码"] = dtM_fu.Rows[i]["存货分类编码"];
                        d["存货分类名称"] = dtM_fu.Rows[i]["存货分类名称"];
                        d["层级"] = dtM_fu.Rows[i]["层级"];
                        d["是否末级"] = dtM_fu.Rows[i]["是否末级"];
                        if (d["上级类型GUID"].ToString() == "")
                        {
                            d["上级类型GUID"] = drr["GUID"].ToString();
                            dr["上级类型GUID"] = drr["GUID"].ToString();
                        }
                        dw = 1;
                    }
                    if (dr["层级"].Equals("5") && eqw == 0)
                    {
                        DataRow d = dt_数据.NewRow();
                        dt_数据.Rows.Add(d);
                        d["GUID"] = dtM_fu.Rows[i]["GUID"];
                        d["存货分类编码"] = dtM_fu.Rows[i]["存货分类编码"];
                        d["存货分类名称"] = dtM_fu.Rows[i]["存货分类名称"];
                        d["层级"] = dtM_fu.Rows[i]["层级"];
                        d["是否末级"] = dtM_fu.Rows[i]["是否末级"];
                        if (d["上级类型GUID"].ToString() == "")
                        {
                            d["上级类型GUID"] = drr["GUID"].ToString();
                            dr["上级类型GUID"] = drr["GUID"].ToString();
                        }
                        eqw = 1;
                    }


                    foreach (DataRow dtt in dt_数据.Rows)
                    {

                        if (dr["层级"].ToString() != "1" && dr["层级"].ToString() == dtt["层级"].ToString())
                        {
                            dr["上级类型GUID"] = dtt["上级类型GUID"];
                           
                        }

                    }


                }
                string sql = "select  *  from 基础数据存货分类表 where 1<>1";
                //using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                //{
                //    new SqlCommandBuilder(da);
                //    DataTable dtp = new DataTable();
                //    dtp = dtM_fu.Copy();
                //    da.Update(dtp);
                //}


                daM = new SqlDataAdapter(sql, CPublic.Var.strConn);
                brM = new SqlCommandBuilder(daM);
                daM.Update(dtM_fu);

                MessageBox.Show("保存成功！");
            }
            catch(Exception ex)
            {
                MessageBox.Show(  ex.Message);
            }
        }


            //foreach (DataRow dr  in dtM.Rows ){
            //    dr["GUID"] = Guid.NewGuid();
            //    if (dr["层级"].Equals("1"))
            //    {

            //        dr["上级类型GUID"] = "";

            //    }
            //    else
            //    {
            //        dr["上级类型GUID"]=dr[dtM.Rows.Count[i-1]]["GUID"];
            // string  a=       dr[""][2];
            //    }
                

            //    i++;

            //}

        





            //for (int i = 0; i < dtM.Rows.Count; i++ )
            //{
            //    DataRow dr = dtM.Rows[i];
                
            //    dr["GUID"] = Guid.NewGuid();
            //    if (dr["层级"].Equals("1"))
            //    {

            //        dr["上级类型GUID"] = "";

            //    }
            //    else
            //    {
            //        
            //    }
                  
                   
            //    }
                


            //}


           

       // }//保存


        public static DataTable ImportExcelToDataTable2(string path)
        {
            string conStr = string.Format("Provider=Microsoft.ACE.OLEDB.12.0; Data source={0}; Extended Properties=Excel 12.0;", path);
            using (OleDbConnection conn = new OleDbConnection(conStr))
            {
                conn.Open();
                //获取所有Sheet的相关信息
                DataTable dtSheet = conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);
                //获取第一个 Sheet的名称
                string sheetName = dtSheet.Rows[0]["Table_Name"].ToString();
                string sql = string.Format("select * from [{0}]", sheetName);
                using (OleDbDataAdapter oda = new OleDbDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    oda.Fill(dt);
                    return dt;
                }
            }
        }
    }
}




















