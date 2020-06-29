using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ERPreport
{
    public partial class ui模具合同台账 : UserControl
    {
        #region 变量
        string strcon = CPublic.Var.strConn;
        DataTable dtP;
        DataTable dtM;
        int POS = 0;
        DataTable dt_客户;
        DataTable dt_模具;
        string str_模具订单号 = "";
        /// <summary>
        /// 是否新增 true 新增  false 修改
        /// </summary>
        bool bool_add = false;
        #endregion
        public ui模具合同台账()
        {
            InitializeComponent();
            bool_add = true;
        }
        public ui模具合同台账(DataRow dr)
        {
            InitializeComponent();
            str_模具订单号 = dr["模具订单号"].ToString();
            bool_add = false;

        }
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        
        //没用
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_load();
                label8.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ui模具合同台账_Load(object sender, EventArgs e)
        {
            try
            {
            
                fun_load();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void fun_clear()
        {

            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            dateEdit1.EditValue = "";
            dateEdit2.EditValue = "";
            searchLookUpEdit1.EditValue = "";
            dtP = dtP.Clone();
            gridControl1.DataSource = dtP;

        }
        private void save()
        {
            if (bool_add) //新增
            {
             
                    string strNO = string.Format("Md{0}{1}{2}{3}", CPublic.Var.getDatetime().Year.ToString(), CPublic.Var.getDatetime().Month.ToString("00"),
                                 CPublic.Var.getDatetime().Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("Md", CPublic.Var.getDatetime().Year, CPublic.Var.getDatetime().Month).ToString("0000"));
                    textBox5.Text = strNO;
               
                //主表
                string sql = "select * from 模具合同台账主表 where 1<>1";
                DataTable dt = new DataTable();
                dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                DataRow dr = dt.NewRow();
                dataBindHelper1.DataToDR(dr);
                dr["开始时间"] = dateEdit1.EditValue;
                dr["结束时间"] = dateEdit2.EditValue;
                dr["修改时间"] = CPublic.Var.getDatetime();
                dr["生效时间"] = CPublic.Var.getDatetime();
                dr["生效"] = true;
                dr["确认人"] = textBox4.Text = CPublic.Var.localUserName;
                dt.Rows.Add(dr);
                CZMaster.MasterSQL.Save_DataTable(dt, "模具合同台账主表", strcon);
                //明细 
                POS = 1;//明细号
                foreach (DataRow mx in dtP.Rows)
                {
                    if (mx.RowState == DataRowState.Deleted) continue;
                    mx["模具订单号"] = textBox5.Text;
                    mx["POS"] = POS++;
                    mx["明细号"] = textBox5.Text+"-"+ POS.ToString("00");
                    mx["生效"] = true;
                    mx["生效日期"] = CPublic.Var.getDatetime();
                }
                CZMaster.MasterSQL.Save_DataTable(dtP, "模具合同台账明细表", strcon);

            }
            else //修改
            {
                //主表 
                string sql = string.Format("select * from 模具合同台账主表 where  模具订单号='{0}'", textBox5.Text.Trim().ToString());
                DataTable dt = new DataTable();
                dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                dataBindHelper1.DataToDR(dt.Rows[0]);
                dt.Rows[0]["开始时间"] = dateEdit1.EditValue;
                dt.Rows[0]["结束时间"] = dateEdit2.EditValue;
                dt.Rows[0]["修改时间"] = CPublic.Var.getDatetime();
                dt.Rows[0]["确认人"] = textBox4.Text = CPublic.Var.localUserName;
                CZMaster.MasterSQL.Save_DataTable(dt, "模具合同台账主表", strcon);

                foreach (DataRow mx in dtP.Rows)
                {
                    if (mx.RowState == DataRowState.Deleted) continue;

                    if (mx["明细号"].ToString() == "")
                    {
                        POS++;
                        mx["模具订单号"] = textBox5.Text;
                        mx["POS"] = POS;
                        mx["明细号"] = textBox5.Text +"-"+ POS.ToString("00");
                        mx["生效"] = true;
                        mx["生效日期"] = CPublic.Var.getDatetime();
                    }
                }
                CZMaster.MasterSQL.Save_DataTable(dtP, "模具合同台账明细表", strcon);

            }
           

        }
        private void fun_load()
        {
            
            //string t1 =Convert.ToDateTime(barEditItem1.EditValue).ToString("yyyy-MM-dd");
            //string  t2 = Convert.ToDateTime(barEditItem2.EditValue).ToString("yyyy-MM-dd");

            //string sql = string .Format("select * from 模具合同台账 where 生效=1 and 生效时间>'{0}' and 生效时间<'{1}' ",t1,Convert.ToDateTime(t2).AddDays(1).AddSeconds(-1));
            //dtM =CZMaster.MasterSQL.Get_DataTable(sql,strcon);
            //gridControl1.DataSource = dtM;
            string m = "select 客户编号,客户名称 from 客户基础信息表 where 停用=0";
            dt_客户 = new DataTable();
            dt_客户 = CZMaster.MasterSQL.Get_DataTable(m, strcon);
            searchLookUpEdit1.Properties.DataSource = dt_客户;
            searchLookUpEdit1.Properties.DisplayMember = "客户编号";
            searchLookUpEdit1.Properties.ValueMember = "客户编号";
            // 模具列表
            string sql_1 = string.Format("select 模具编号,产品型号,零件图号,工装编号,存放库位,在库状态,模具属性  from 模具管理基础信息表");
            dt_模具 = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);
            repositoryItemSearchLookUpEdit1.DataSource = dt_模具;
            repositoryItemSearchLookUpEdit1.DisplayMember = "模具编号";
            repositoryItemSearchLookUpEdit1.ValueMember = "模具编号";

            if (bool_add == false) //修改
            {
                string sql = string.Format("select max(POS)POS from 模具合同台账明细表 where 模具订单号 = '{0}'", str_模具订单号);
                SqlDataAdapter daa = new SqlDataAdapter(sql, strcon);
                DataTable dt = new DataTable();
                daa.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    POS = Convert.ToInt32(dt.Rows[0]["POS"]);
                }
                string s = string.Format("select * from 模具合同台账主表 where 模具订单号='{0}'", str_模具订单号);
                dtM = new DataTable();
                dtM = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                dataBindHelper1.DataFormDR(dtM.Rows[0]);
                dateEdit1.EditValue = dtM.Rows[0]["开始时间"];
                dateEdit2.EditValue = dtM.Rows[0]["结束时间"];

                string ss = string.Format(@"select a.*,b.工装编号,b.零件图号 from 模具合同台账明细表 a 
                                        left join 模具管理基础信息表 b on a.模具编号=b.模具编号  where 模具订单号='{0}'", str_模具订单号);
                dtP = new DataTable();
                dtP = CZMaster.MasterSQL.Get_DataTable(ss, strcon);
                gridControl1.DataSource = dtP;
            }
            else
            {
                string s = string.Format("select * from 模具销售合同台账主表 where 1<>1");
                dtM = new DataTable();
                dtM = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                textBox4.Text = CPublic.Var.localUserName;

                string ss = string.Format(@"select a.*,b.工装编号,b.零件图号 from 模具合同台账明细表 a 
                                        left join 模具管理基础信息表 b on a.模具编号=b.模具编号 where 1<>1");
                dtP = new DataTable();
                dtP = CZMaster.MasterSQL.Get_DataTable(ss, strcon);
                gridControl1.DataSource = dtP;
            }


        }
        private void check()
        {
            if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
            {
                throw new Exception("未选择模具商");
            }
            if (textBox1.Text == "")
            {
                throw new Exception("未填写合同编号");
            } 
            if (dateEdit1.EditValue == null || dateEdit1.EditValue.ToString() == "")
            {
                throw new Exception("开始时间未选择");
            } 
            if (dateEdit2.EditValue == null || dateEdit2.EditValue.ToString() == "")
            {
                throw new Exception("开始时间未选择");
            }
        
            foreach (DataRow dr in dtP.Rows)
            {
         
                if (dr.RowState == DataRowState.Deleted)
                {
                    continue;
                }
                if (dr["数量"].ToString() == "")
                {
                    throw new Exception("数量不能为空");

                }
                else
                {
                    try
                    {
                        if (Convert.ToDecimal(dr["数量"]) <= 0)
                        {

                            throw new Exception("数量不可小于0");

                        }
                    }
                    catch
                    {

                        throw new Exception("数量输入格式不正确");

                    }
                    
                }
                if (dr["模具编号"].ToString() == "")
                {
                    throw new Exception("有空行，删除后再保存");
                }
            }
        
           
        }
        private void fun_aggregate()
        {
            decimal dec = 0;
            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                dec += Convert.ToDecimal(dr["金额"]);
                textBox3.Text = dec.ToString();
            }
        }
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_clear();
            bool_add = true;
            label8.Text = "新增中";
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
      
            //DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            //if (Convert.ToDecimal(dr["已开票数量"]) > 0)
            //{
            //    gridColumn2.OptionsColumn.AllowEdit = false;
            //}
            //else
            //{
            //    gridColumn2.OptionsColumn.AllowEdit = true;
            //}
  


        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
            {
                DataRow[] dr = dt_客户.Select(string.Format("客户编号='{0}'", searchLookUpEdit1.EditValue));
                textBox2.Text = dr[0]["客户名称"].ToString();
            }

        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataRow dr = dtP.NewRow();
            dtP.Rows.Add(dr);
            if (dtP.Rows.Count > 0)
            {
                dr["要求送达日期"] = dtP.Rows[0]["要求送达日期"];
            }

        }
        private void button3_Click(object sender, EventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (bool_add)
            {
                dr.Delete();
            }
            else
            {
                dr["关闭"] = true;
            }
            fun_aggregate();
        }

        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.Caption == "模具编号")
            {
                DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                DataRow[] dr = dt_模具.Select(string.Format("模具编号='{0}'", e.Value));
                r["零件图号"] = dr[0]["零件图号"];
                r["工装编号"] = dr[0]["工装编号"];
                r["模具类型"] = dr[0]["模具类型"];


            }
            if (e.Column.Caption == "数量")
            {
                DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (r["模具单价"].ToString() != "")
                {
                    r["金额"] = Convert.ToDecimal(r["模具单价"]) * Convert.ToDecimal(e.Value);
                    fun_aggregate();
                }
                r["未开票数量"] = r["数量"];
            }
            if (e.Column.Caption == "模具单价")
            {
                DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (r["数量"].ToString() != "")
                {
                    r["金额"] = Convert.ToDecimal(e.Value) * Convert.ToDecimal(r["数量"]);
                    fun_aggregate();
                }
             
            }

          


        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                check();
                save();
                fun_clear();
                MessageBox.Show("生效成功");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

    


    }
}
