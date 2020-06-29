using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ERPproduct
{
    public partial class frm查看制令相关工单的状态 : UserControl
    {
        string strcon = CPublic.Var.strConn;
         string str_制令号="";

         DataTable dtM;
         DataTable dt_下拉;
        public frm查看制令相关工单的状态(string s)
        {
            InitializeComponent();
            this.str_制令号=s;
        }
        public frm查看制令相关工单的状态()
        {
            InitializeComponent();
       
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm查看制令相关工单的状态_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_制令列表();
            fun_load(str_制令号);
            
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_制令列表()     //取最近一个半月的
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format(@"SELECT [生产制令单号],sczl.[物料编码],sczl.[物料名称] ,sczl.[规格型号]
                         FROM  [生产记录生产制令表] sczl
                        left join 基础数据物料信息表 base on base.物料编码 = sczl.[物料编码] where sczl.生效日期>'{0}'", System.DateTime.Today.AddMonths(-1).AddDays(-15));

            dt_下拉 = CZMaster.MasterSQL.Get_DataTable(sql,strcon);
            repositoryItemSearchLookUpEdit1.DataSource = dt_下拉;
            repositoryItemSearchLookUpEdit1.DisplayMember = "生产制令单号";
            repositoryItemSearchLookUpEdit1.ValueMember = "生产制令单号";


        
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_load(string ss)
#pragma warning restore IDE1006 // 命名样式
        {

            string sql = string.Format(@"select gd.[生产工单号],[生产制令单号],gd.[物料编码],gd.[物料名称],jyz.已入库数量, 
				                      gd.[规格型号],gd.[生产数量],gd.[生效],gd.[完工],gd.关闭,
                                      gd.[检验完成],jyz.[生产检验单号],jyz.[完成],jyz.[包装确认]
									from  生产记录生产工单表  gd left join	生产记录生产检验单主表 jyz  on gd.[生产工单号]= jyz.生产工单号
                                    left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = gd.[物料编码] where 生产制令单号='{0}'  ", ss);
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dtM;
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (barEditItem1.EditValue != null)
            {
                fun_load(barEditItem1.EditValue.ToString());
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
