using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 郭恒的DEMO
{
    public partial class treelist试验 : Form
    {

        string strcon = CPublic.Var.strConn;
        string strcon_U8 = CPublic.Var.geConn("DW");
        public treelist试验()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // string s = "select   *   from  基础数据物料BOM表  "; //BOM中只有1个成品的整套bom     not in ('05010101000006SMT','01020101000005')
                // DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                // dt.Columns.Add("ID",s.GetType());
                // int i = 0;

                // foreach (DataRow dr in dt.Rows)
                // {
                //     dr["ID"] = i++.ToString();

                // }


                // treeList1.OptionsBehavior.PopulateServiceColumns = true;
                // treeList1.KeyFieldName = "ID";  //不可以
                ////  treeList1.KeyFieldName = "子项编码"; 可以 但是 不可以有重复 ,有重复也不行
                // treeList1.ParentFieldName = "产品编码";
                // treeList1.DataSource = dt;


               //string   s = "select  *  from 基础数据存货分类表  ";
               // DataTable tt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
               // treeList1.OptionsBehavior.PopulateServiceColumns = true;
               // treeList1.KeyFieldName = "GUID";
               // treeList1.ParentFieldName = "上级类型GUID";
               // treeList1.DataSource = tt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
          
        }

        private void button2_Click(object sender, EventArgs e)
        {
           DataTable dt= get_u8bom(textBox1.Text);
       

        }

        public DataTable  get_u8bom(string str_物料号)
        {
            DataTable dtM = new DataTable();
            dtM.Columns.Add("父项编码");
            dtM.Columns.Add("父项名称");
            dtM.Columns.Add("父项规格");
            dtM.Columns.Add("子项编码");
            dtM.Columns.Add("子项名称");
            dtM.Columns.Add("子项规格");
           // dtM.Columns.Add("数量");



            string s = string.Format(@"select  bas_part.PartId,bas_part.InvCode 父项编码,fx.cInvName as 父项名称,fx.cInvStd as 父项规格,[bom_parent].BomId,OpComponentId,ComponentId,a.InvCode as 子项编码
  ,zx.cInvName as 子项名称,zx.cInvStd as 子项规格 from bas_part 
  inner  join [bom_parent] on [bom_parent].ParentId=bas_part.PartId 
  inner  join [bom_opcomponent] on [bom_opcomponent].BomId=[bom_parent].BomId
  inner   join bas_part a on  a.PartId=ComponentId
  inner  join  inventory  zx on zx.cInvCode=a.InvCode
  inner  join  inventory fx on fx.cInvCode=bas_part.InvCode  where bas_part.InvCode='{0}'", str_物料号);
            DataTable dt1 = CZMaster.MasterSQL.Get_DataTable(s, strcon_U8);

            if (dt1.Rows.Count > 0) dtM = get_u8_子项(dtM, dt1);
  

            return dtM;
        }

        private DataTable get_u8_子项(DataTable dtM,DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                DataRow r = dtM.NewRow();
                r["父项编码"] = dr["父项编码"];
                r["父项名称"] = dr["父项名称"];
                r["父项规格"] = dr["父项规格"];
                r["子项编码"] = dr["子项编码"];
                r["子项名称"] = dr["子项名称"];
                r["子项规格"] = dr["子项规格"];
                // r["父项编码"] = dr["父项编码"];
                dtM.Rows.Add(r);
                string s = string.Format(@"select  bas_part.PartId,bas_part.InvCode 父项编码,fx.cInvName as 父项名称,fx.cInvStd as 父项规格,[bom_parent].BomId,OpComponentId,ComponentId,a.InvCode as 子项编码
  ,zx.cInvName as 子项名称,zx.cInvStd as 子项规格 from bas_part 
  inner  join [bom_parent] on [bom_parent].ParentId=bas_part.PartId 
  inner  join [bom_opcomponent] on [bom_opcomponent].BomId=[bom_parent].BomId
  inner   join bas_part a on  a.PartId=ComponentId
  inner  join  inventory  zx on zx.cInvCode=a.InvCode
  inner  join  inventory fx on fx.cInvCode=bas_part.InvCode where bas_part.InvCode='{0}'", dr["子项编码"]);
                DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon_U8);
                if(t.Rows.Count>0)   dtM= get_u8_子项(dtM,t);

            }

            return dtM;
        }


    }
}
