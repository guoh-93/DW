using System;
using System.Windows.Forms;
using System.Data;
namespace 郭恒的DEMO
{
    public partial class 销售退货 : Form
    {
        string strcon = CPublic.Var.strConn;
        public 销售退货()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            xx(textBox1.Text);

        }
        private void xx(string dh)
        {
            DateTime time = CPublic.Var.getDatetime();
            string s = $@"update  退货申请子表 set 已入库数量=0 where 退货申请单号 = '{dh}' 
                          update 退货申请主表 set 作废=1,作废日期={time} where 退货申请单号 = '{dh}' 
                          delete 退货入库子表 where 退货申请单号 ='{dh}' 
                          delete 退货入库主表 where 退货申请单号 ='{dh}' 
                          delete   仓库出入库明细表 where   明细类型='销售退货'  and  明细号 in
                          (select  出库明细号  from 退货申请子表 where 退货申请单号='{dh}')
                                      ";
            string thsq = $"select * from 退货入库子表 where 退货申请单号 = '{dh}'";
            DataTable t_thsq = CZMaster.MasterSQL.Get_DataTable(thsq,strcon);
            DataTable t_save= ERPorg.Corg.fun_库存(-1, t_thsq);

            //还要看 销售记录成品出库单明细表 里面有没有开过票,红字要去掉 原单累计退货数量 要恢复

            CZMaster.MasterSQL.Save_DataTable(t_save,"仓库物料数量表",strcon);
            CZMaster.MasterSQL.ExecuteSQL(s,strcon);
        }


    }
}
