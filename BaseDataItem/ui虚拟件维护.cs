using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace BaseData
{
    public partial class ui虚拟件维护 : UserControl
    {

        string strcon = CPublic.Var.strConn;
        DataTable dtM;
        DataTable dt_单位;
        DataTable dt_物料;
        DataRow r_cs;


        public ui虚拟件维护(DataRow dr)
        {
            InitializeComponent();

            r_cs = dr;
            textBox3.Text = dr["物料编码"].ToString();
            textBox1.Text = dr["规格型号"].ToString();
            textBox2.Text = dr["物料名称"].ToString();
        }
        private void fun_load()
        {
            string s = "select  物料编码,规格型号,物料名称,计量单位编码,计量单位,仓库号,仓库名称,货架描述,图纸编号 from 基础数据物料信息表  "; //2018-9-18 这边可能需要 加条件 暂时不知道物料怎么分类
            dt_物料 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            repositoryItemSearchLookUpEdit1.DataSource = dt_物料;
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            s = "select 属性值 计量单位,属性字段1 as  计量单位编码 from 基础数据基础属性表 where 属性类别 = '计量单位'";
            dt_单位 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            repositoryItemSearchLookUpEdit2.DataSource = dt_单位;
            repositoryItemSearchLookUpEdit2.DisplayMember = "计量单位编码";
            repositoryItemSearchLookUpEdit2.ValueMember = "计量单位编码";
            s = string.Format("select  a.*,规格型号,物料名称 as 子项名称,计量单位编码,计量单位 from 虚拟件对应关系表 a,基础数据物料信息表 b  where a.子项编码=b.物料编码 and 父项编码='{0}'",r_cs["物料编码"]);
            dtM = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gc.DataSource = dtM;

        }
        private void ui虚拟件维护_Load(object sender, EventArgs e)
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
        private void fun_check()
        {
            foreach (DataRow dr in dtM.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                if (dr["仓库号"].ToString() == "") throw new Exception("仓库号为空,数据有误");
                if (dr["计量单位编码"].ToString().Trim() == "") throw new Exception("计量单位选择有误");
                decimal dec = 0;
                if (!decimal.TryParse(dr["数量"].ToString(), out dec)) throw new Exception("数量输入有误");
            }
        }

        private void fun_save()
        {
          //暂只要一张表
            CZMaster.MasterSQL.Save_DataTable(dtM, "虚拟件对应关系表", strcon);
        }
        //新增
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DataRow dr = dtM.NewRow();
            dr["父项编码"] = r_cs["物料编码"];
            dtM.Rows.Add(dr);
        }
        //刷新
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                textBox3.Text = r_cs["物料编码"].ToString();
                textBox1.Text = r_cs["规格型号"].ToString();
                textBox2.Text = r_cs["物料名称"].ToString();
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem12_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            dr.Delete();
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow d = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
            DataRow drr = gv.GetDataRow(gv.FocusedRowHandle);
            drr["子项编码"] = d["物料编码"];
            drr["子项名称"] = d["物料名称"];
            drr["规格型号"] = d["规格型号"];
            drr["计量单位编码"] = d["计量单位编码"];
            drr["计量单位"] = d["计量单位"];
            drr["仓库号"] = d["仓库号"];
            drr["仓库名称"] = d["仓库名称"];
           // drr["货架描述"] = d["货架描述"];

        }

        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            DataRow d = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
            DataRow drr = gv.GetDataRow(gv.FocusedRowHandle);

            drr["子项编码"] = d["物料编码"];
            drr["子项名称"] = d["物料名称"];
            drr["规格型号"] = d["规格型号"];
            drr["计量单位编码"] = d["计量单位编码"];
            drr["计量单位"] = d["计量单位"];
            drr["仓库号"] = d["仓库号"];
            drr["仓库名称"] = d["仓库名称"];
         //   drr["货架描述"] = d["货架描述"];


        }

        private void barLargeButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_check();
                fun_save();
                MessageBox.Show("保存成功");
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        //
        private void barLargeButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void gv_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Value != null && e.Column.FieldName == "计量单位编码")
            {
                 dt_单位.Select(string.Format("计量单位编码='{0}'",e.Value));
            }
        }
    }
}
