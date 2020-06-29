using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CPublic;
using CZMaster;

namespace DetectionPlatformBaseData

{
    public partial class frmABB检查动作维护功能 : UserControl
    {
        public frmABB检查动作维护功能()
        {
            InitializeComponent();
        }

        #region 变量

        DataTable dtM;  //检测类型主表
        DataTable dtP;  //检测类型子表  动作组表
        DataTable dtP1;  
        DataRow drM; //操作行
        string jiancemc = "";  //检测名称
        DataTable dt_zifu;

        DataTable dt_组复制;

        DataTable dt_动作子;
        DataTable dt_动作子复制;


        Dictionary<string, string> dic=new Dictionary<string, string>();   //字典
        Dictionary<string, string> dic1=new Dictionary<string, string>();
        Dictionary<string, string> dic2=new Dictionary<string,string>();

        #endregion

        #region  类加载

        private void fun_报表类型()
        {
            //string sql = "";
            string sql = "select * from 检测报表类型表";
            DataTable dt_报表;
            dt_报表 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
            if (dt_报表.Rows.Count > 0)
            {
                foreach (DataRow r in dt_报表.Rows)
                {
                    txt_bbleixing.Properties.Items.Add(r["报表ID"].ToString());
                }
            }
        }

        private void frmABB检查动作维护功能_Load(object sender, EventArgs e)
        {
            try
            {
                //fun_报表类型();
                fun_动作表查询();
                fun_load();
                //多行输入的两个事件
                gvM2.ShownEditor += gvM2_ShownEditor;
                gcM2.EditorKeyUp += gcM2_EditorKeyUp;
                this.barEditItem1.Edit.KeyDown += Edit_KeyDown;  //查询框回车

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

     //查询框回车事件
        void Edit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    fun_主表查询();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        #endregion


        #region gridcontrol多行输入

        void gcM2_EditorKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && (gvM2.ActiveEditor is DevExpress.XtraEditors.MemoEdit))
            {
                gvM2.CloseEditor();
                gvM2.RefreshData();
                gvM2.ShowEditor();
            }
        }

        void gvM2_ShownEditor(object sender, EventArgs e)
        {
            if (gvM2.ActiveEditor is DevExpress.XtraEditors.MemoEdit)
            {
                DevExpress.XtraEditors.MemoEdit me = gvM2.ActiveEditor as DevExpress.XtraEditors.MemoEdit;
                try
                {
                    me.SelectionStart = me.Text.Length;
                }
                catch
                {
                }
            }
        }

        #endregion


        #region    主表  动作组表保存时的数据检查


        //private void fun_check动作子表()
        //{
        //    foreach (DataRow r in dt_动作子.Rows)
        //    {
        //        if (r.RowState == DataRowState.Deleted) continue;

        //        if (r["检测ID"].ToString() != JCID.Text)
        //            r["检测ID"] = JCID.Text;

        //        if (r["检测名称"].ToString() != JCMC.Text)
        //            r["检测名称"] = JCMC.Text;

        //    }
        //}
       



        //检测子表数据
        private void fun_check子表()
        {
            foreach (DataRow r in dtP.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;
                //if (r["检测ID"].ToString() == "")  //检测ID不能为空
                    //throw new Exception("检测ID有空值，请检查一下！");
                if (r["检测ID"].ToString() != JCID.Text)    //检测ID要与保存的文本框是一致的
                    r["检测ID"] = JCID.Text;
                    //throw new Exception("检测ID与上述检测ID文本框中的检测ID有不一致，请检查一下！");
             //   if (r["检测名称"].ToString() == "")
                    //throw new Exception("检测名称有空值，请检查一下！");
                if (r["检测名称"].ToString() != JCMC.Text)   //检测名称也要与保存的文本框是一致的
                    r["检测名称"] = JCMC.Text;
                   // throw new Exception("检测名称与上述检测名称文本框中的检测ID有不一致，请检查一下！");
                if (r["检测组POS"].ToString() == "")
                    throw new Exception("检测组顺序有空值，请检查一下！");
                try
                {
                    int i = Convert.ToInt32(r["检测组POS"].ToString());
                }
                catch
                {
                    throw new Exception(string.Format("检测组顺序\"{0}\"不是是数字，请检查！", r["检测组POS"].ToString()));
                }
                DataRow[] r1 = dtP.Select(string.Format("检测组POS='{0}'", r["检测组POS"].ToString()));
                if (r1.Length > 1)
                    throw new Exception(string.Format("检测组顺序\"{0}\"数据重复，请检查", r["检测组POS"].ToString()));
                if (r["检测要求"].ToString() == "")
                    throw new Exception("检测要求有空值，请检查一下！");
                if (r["设备要求"].ToString() == "")
                    throw new Exception("设备要求有空值，请检查一下！");
                if (r["检测内容"].ToString() == "")
                    throw new Exception("检测内容有空值，请检查一下！");
            }

        }

        //检测主表的数据
        private void fun_check主表()
        {

            //检测ID检查
            if (JCID.Text == "")
            {
                JCID.Focus();
                throw new Exception("检测ID不能为空，请填写！");
            }
            try
            {
                int i = Convert.ToInt32(JCID.Text);
            }
            catch
            {
                JCID.Focus();
                throw new Exception("检测ID是数字，请重新填写！");
            }
            //检测名称检查
            if (JCMC.Text == "")
            {
                JCMC.Focus();
                throw new Exception("检测名称不能够为空，请填写！");
            }
            if (JCMC.Enabled == true)
            {
                DataRow[] r = dtM.Select(string.Format("检测名称='{0}'", JCMC.Text));
                if (r.Length >0)
                {
                    JCMC.Focus();
                    throw new Exception("检测名称有重复，请重新填写！");
                }
            }

            #region  数据非空检查

            //检测描述
            if (JCMS.Text == "")
            {
                throw new Exception("检测描述不能为空，请填写！");
            }
            //检测大类
            if (comboBoxEdit1.Text == "")
                throw new Exception("检测大类不能为空，请选择！");
            //交流电压允许误差
            if (JLDYYXWC.Text == "")
                throw new Exception("交流电压允许误差不能为空，请填写！");
            //直流电压允许误差
            if (ZLDYYXWC.Text == "")
                throw new Exception("直流电压允许误差不能为空，请填写！");
            //分闸激光距离允许误差
            if (FZJGJLYXWC.Text == "")
                throw new Exception("分闸激光距离允许误差不能为空，请填写！");
            //合闸激光距离允许误差
            if (HZJGJLYXWC.Text == "")
                throw new Exception("合闸激光距离允许误差不能为空，请填写！");
            //手自动力下限
            if (SZDLXX.Text == "")
                throw new Exception("手自动力下限不能为空，请填写！");
            //手自动力上限
            if (SZDLSX.Text == "")
                throw new Exception("手自动力上限不能为空，请填写！");
            //电压默认等待时间上限
            if (DYMRSJ.Text == "")
                throw new Exception("电压默认等待时间上限不能为空，请填写！");
            //合闸默认等待时间上限
            if (HZMRSJ.Text == "")
                throw new Exception("合闸默认等待时间上限不能为空，请填写！");
            //分闸默认等待时间上限
            if (FZMRSJ.Text == "")
                throw new Exception("分闸默认等待时间上限不能为空，请填写！");
            //脱扣默认等待时间上限
            if (TKMRSJ.Text == "")
                throw new Exception("脱扣默认等待时间上限不能为空，请填写！");
            //机构合闸动作时间 
            if (HZDZSJ.Text == "")
                throw new Exception("机构合闸动作时间不能为空，请填写！");
            //机构分闸动作时间
            if (FZDZSJ.Text == "")
                throw new Exception("机构分闸动作时间不能为空，请填写！");
            //机构脱扣动作时间
            if (TKDZSJ.Text == "")
                throw new Exception("机构脱扣动作时间不能为空，请填写！");
            //机构手动动作时间
            if (SDDZSJ.Text == "")
                throw new Exception("机构手动动作时间不能为空，请填写！");
            //机构自动动作时间
            if (ZDDZSJ.Text == "")
                throw new Exception("机构自动动作时间不能为空，请填写！");
            //自动合闸动作时间
            if (ZDHZDZSJ.Text == "")
                throw new Exception("自动合闸动作时间不能为空，请填写！");
            //自动分闸动作时间
            if (ZDFZDZSJ.Text == "")
                throw new Exception("自动分闸动作时间不能为空，请填写！");
            //分合闸动作时间误差
            if (FHZDZSJWC.Text == "")
                throw new Exception("分合闸动作时间误差不能为空，请填写！");
            //其他动作时间误差
            if (QTDZSJWC.Text == "")
                throw new Exception("其他动作时间误差不能为空，请填写！");

            #endregion


            #region 数据是否是数字检查

            try
            {
                double i=Convert.ToDouble(JLDYYXWC.Text);
            }
            catch
            {
                JLDYYXWC.Focus();
                throw new Exception("交流电压允许误差应为数字，请检查！");
            }

            try
            {
                double i = Convert.ToDouble(ZLDYYXWC.Text);
            }
            catch
            {
                ZLDYYXWC.Focus();
                throw new Exception("直流电压允许误差应为数字，请检查！");
            }

            try
            {
                double i = Convert.ToDouble(FZJGJLYXWC.Text);
            }
            catch
            {
                FZJGJLYXWC.Focus();
                throw new Exception("分闸激光距离允许误差应为数字，请检查！");
            }

            try
            {
                double i = Convert.ToDouble(HZJGJLYXWC.Text);
            }
            catch
            {
                HZJGJLYXWC.Focus();
                throw new Exception("合闸激光距离允许误差应为数字，请检查！");
            }

            try
            {
                double i = Convert.ToDouble(SZDLXX.Text);
            }
            catch
            {
                SZDLXX.Focus();
                throw new Exception("手自动力下限应为数字，请检查！");
            }

            try
            {
                double i = Convert.ToDouble(SZDLSX.Text);
            }
            catch
            {
                SZDLSX.Focus();
                throw new Exception("手自动力上限应为数字，请检查！");
            }

            try
            {
                double i = Convert.ToDouble(DYMRSJ.Text);
            }
            catch
            {
                DYMRSJ.Focus();
                throw new Exception("电压默认等待时间应为数字，请检查！");
            }

            try
            {
                double i = Convert.ToDouble(HZMRSJ.Text);
            }
            catch
            {
                HZMRSJ.Focus();
                throw new Exception("合闸默认等待时间应为数字，请检查！");
            }

            try
            {
                double i = Convert.ToDouble(FZMRSJ.Text);
            }
            catch
            {
                FZMRSJ.Focus();
                throw new Exception("分闸默认等待时间应为数字，请检查！");
            }

            try
            {
                double i = Convert.ToDouble(TKMRSJ.Text);
            }
            catch
            {
                TKMRSJ.Focus();
                throw new Exception("脱扣默认等待时间应为数字，请检查！");
            }

            try
            {
                double i = Convert.ToDouble(HZDZSJ.Text);
            }
            catch
            {
                HZDZSJ.Focus();
                throw new Exception("机构合闸动作时间应为数字，请检查！");
            }

            try
            {
                double i = Convert.ToDouble(FZDZSJ.Text);
            }
            catch
            {
                FZDZSJ.Focus();
                throw new Exception("机构分闸动作时间应为数字，请检查！");
            }

            try
            {
                double i = Convert.ToDouble(TKDZSJ.Text);
            }
            catch
            {
                TKDZSJ.Focus();
                throw new Exception("机构脱扣动作时间应为数字，请检查！");
            }

            try
            {
                double i = Convert.ToDouble(ZDDZSJ.Text);
            }
            catch
            {
                ZDDZSJ.Focus();
                throw new Exception("机构自动动作时间应为数字，请检查！");
            }

            try
            {
                double i = Convert.ToDouble(SDDZSJ.Text);
            }
            catch
            {
                SDDZSJ.Focus();
                throw new Exception("机构手动动作时间应为数字，请检查！");
            }

            try
            {
                double i = Convert.ToDouble(ZDHZDZSJ.Text);
            }
            catch
            {
                ZDHZDZSJ.Focus();
                throw new Exception("自动合闸动作时间应为数字，请检查！");
            }

            try
            {
                double i = Convert.ToDouble(ZDFZDZSJ.Text);
            }
            catch
            {
                ZDFZDZSJ.Focus();
                throw new Exception("自动分闸动作时间应为数字，请检查！");
            }

            try
            {
                double i = Convert.ToDouble(FHZDZSJWC.Text);
            }
            catch
            {
                FHZDZSJWC.Focus();
                throw new Exception("分合闸动作时间误差应为数字，请检查！");
            }

            try
            {
                double i = Convert.ToDouble(QTDZSJWC.Text);
            }
            catch
            {
                QTDZSJWC.Focus();
                throw new Exception("其他动作时间误差应为数字，请检查！");
            }

            #endregion

            
                               
            dataBindHelper1.DataToDR(drM);  //检查无误，写到drM
                   
            DataRow[] ddr = dtM.Select(string.Format("检测名称='{0}'", drM["检测名称"].ToString()));
            //增加主表数据 进行保存
            if (ddr.Length <= 0)   //说明是新增
            {
                DataView dv = new DataView(dtM);
                dv.Sort = "检测ID DESC";   //从大到小的排序
                foreach (DataRowView drv in dv)
                {
                    DataRow ddr1 = drv.Row;
                    drM["检测ID"] = Convert.ToInt32(ddr1["检测ID"]) + 1;   //检测ID找出最大的加1
                    JCID.Text = drM["检测ID"].ToString();
                    break;
                }
                dtM.Rows.Add(drM);
            }

        }

        #endregion


        #region   主表，子表数据加载

        //加载主表的数据
        private void fun_load()
        {
            string sql = "select * from ABB检测类型主表";
            dtM = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
            gcM1.DataSource = dtM;
            drM = dtM.NewRow();
        }

        //子表动作组表的查询
        private void fun_子表数据查询()
        {
            DataRow r = (this.BindingContext[dtM].Current as DataRowView).Row;   //选择主表dtM上的某一行
            //查询出该dtp子表
            string sql = string.Format("select * from ABB检测类型动作组表 where 检测ID='{0}' and 检测名称='{1}' order by 检测组POS", r["检测ID"].ToString(),r["检测名称"].ToString());
            dtP = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
            gcM2.DataSource = dtP;
            dt_zifu = dtP.Copy();
            drM = r; //赋值
            dataBindHelper1.DataFormDR(drM);
            xtraTabControl1.SelectedTabPage = xtraTabPage1;
        }

        //ABB的动作表查询
        private void fun_动作表查询()
        {
            DataTable dt_动作表 = new DataTable();
            string sql = "select * from 检测机台类型";
            dt_动作表 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
            foreach (DataRow r in dt_动作表.Rows)
            {
                comboBoxEdit1.Properties.Items.Add(r["机台类型"].ToString());
            }
        }

        #endregion


        #region  界面相关调用函数

        //检测类型主表数据查询
        private void fun_主表查询()
        {
            //查询框不能够为空
            if (barEditItem1.EditValue == null)
                throw new Exception("请你输入你需要查询的检测名称！");
            DataRow[] dr = dtM.Select(string.Format("检测名称='{0}'", barEditItem1.EditValue.ToString()));
            if (dr.Length <= 0)
                throw new Exception(string.Format("查询不到检测名称为\"{0}\"的数据", barEditItem1.EditValue.ToString()));
            drM = dr[0];
            dataBindHelper1.DataFormDR(drM);
            jiancemc = dr[0]["检测名称"].ToString();
            JCMC.Enabled = false;   //查询之后，主键不可更改。
            //子表数据
            string sql = string.Format("select * from ABB检测类型动作组表 where 检测名称='{0}' order by 检测组POS", barEditItem1.EditValue);
            dtP = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
            gcM2.DataSource = dtP;
            dt_zifu = dtP.Copy();
        }

        //Dtm主表数据的新增  （检测类型主表）
        private void fun_主表新增()
        {
            if (JCMC.Enabled == false)
            {
                drM = dtM.NewRow();   //new一个属于Dtm主表的行
                dataBindHelper1.DataFormDR(drM);
                JCMC.Enabled = true;  //新增操作的时候，主键恢复可以填写的状态]
                jiancemc = "";  //检测名称的全局变量置空
            }
            else
            {
                drM = dtM.NewRow();
                dataBindHelper1.DataFormDR(drM);
                jiancemc = "";     
            }
            if (dtP != null)  //清空一下子表DTP
            {
                dtP.Clear();
                gcM2.DataSource = dtP;
            }

        }

        //主表数据的删除：主表数据的删除，也会使两张子表数据也随之删除
        private void fun_主表数据删除()
        {
            //查询一遍相关的子表：检测类型动作组表，检测动作组子表
            //string sql = string.Format("select * from ABB检查类型动作组表 where 检测ID='{0}' and 检测名称='{1}'", JCID.Text, JCMC.Text);
            string sql = string.Format("select * from ABB检测类型动作组表 where 检测ID='{0}' and 检测名称='{1}'", drM["检测ID"].ToString(),drM["检测名称"].ToString());
            dtP = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
            foreach (DataRow r in dtP.Rows)
            {
                r.Delete();
            }
            MasterSQL.Save_DataTable(dtP, "ABB检测类型动作组表", CPublic.Var.geConn("PLC"));
            //string sql1 = string.Format("select * from ABB检测动作组子表 where 检测ID='{0}' and 检测名称='{1}'", JCID.Text, JCMC.Text);
            string sql1 = string.Format("select * from ABB检测组动作子表 where 检测ID='{0}' and 检测名称='{1}'", drM["检测ID"].ToString(), drM["检测名称"].ToString());
            dtP1 = MasterSQL.Get_DataTable(sql1, CPublic.Var.geConn("PLC"));
            foreach (DataRow r1 in dtP1.Rows)
            {
                r1.Delete();
            }
            MasterSQL.Save_DataTable(dtP1, "ABB检测组动作子表", CPublic.Var.geConn("PLC"));
            //删除主表数据
            drM.Delete();
            MasterSQL.Save_DataTable(dtM, "ABB检测类型主表", CPublic.Var.geConn("PLC"));
            //删除之后，清空textBOX框
            fun_主表新增();
        }

        //复制一个动作组
        private void fun_动作组复制()
        {
            if (dtP == null || dtP.Rows.Count<=0)
                throw new Exception("没有可以复制的动作组,请新增！");
            DataRow r = (this.BindingContext[dtP].Current as DataRowView).Row;  //选择需要复制的动作组
            DataRow r1 = dtP.NewRow();
            //动作组复制
            r1.ItemArray = r.ItemArray;
            dtP.Rows.Add(r1);
        }

        //新增动作组
        private void fun_新增动作组()
        {
            if (dtP == null)
            {
                string sql = "select * from ABB检测类型动作组表 where 1<>1";
                dtP = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
                gcM2.DataSource = dtP;
            }
            DataRow r = dtP.NewRow();
            r["检测组GUID"] = System.Guid.NewGuid().ToString();
            r["检测ID"] = JCID.Text;                 //保存的时候需要检查 检测ID，检测名称与主数据是否相同，不然就不能保存
            r["检测名称"] = JCMC.Text;
            if (r["检测ID"].ToString() == "" && r["检测名称"].ToString() == "")
                throw new Exception("没有检测类型主数据，请先查询或新增！");
            dtP.Rows.Add(r);
        }



        //动作组的删除
        private void fun_删除动作组(DataRow r)
        {
            //删除某一个动作组的时候，先去查找该该动作组的子表 即ABB检测组动作子表，先把对应的子表数据先删除掉
            DataTable dt_zi = new DataTable();
            string sql = string.Format("select * from ABB检测组动作子表 where 检测ID='{0}' and 检测名称='{1}' and 检测组POS='{2}'", r["检测ID"].ToString(), r["检测名称"].ToString(),r["检测组POS"].ToString());
            dt_zi = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
            //对应的子表如果查到动作就要删除
            if (dt_zi != null && dt_zi.Rows.Count > 0)
            {
                foreach (DataRow r1 in dt_zi.Rows)
                {
                    r1.Delete();
                }
                MasterSQL.Save_DataTable(dt_zi, "ABB检测组动作子表", CPublic.Var.geConn("PLC"));

                //ABB检测组动作子表有对应的数据删除，那么就要对某一个检测ID进行重新排序   
                string sql1 = string.Format("select * from ABB检测组动作子表 where 检测ID='{0}' and 检测名称='{1}'", r["检测ID"].ToString(), r["检测名称"].ToString()); //查询出来
                dt_zi = MasterSQL.Get_DataTable(sql1, CPublic.Var.geConn("PLC"));
                DataView dv = new DataView(dt_zi);
                dv.Sort = "检测组POS,动作POS";
                int i = 1;
                foreach (DataRowView drv in dv)
                {
                    DataRow r1 = drv.Row;
                    if (Convert.ToInt32(drv.Row["检测组POS"].ToString()) > Convert.ToInt32(r["检测组POS"].ToString()))  //如果动作子表的检测组POS，大于被删除的检测组POS就要减去1
                    {
                        drv.Row["检测组POS"] = Convert.ToInt32(drv.Row["检测组POS"].ToString()) - 1;
                    }
                    drv.Row["动作POS"] = i++;
                }
                MasterSQL.Save_DataTable(dt_zi, "ABB检测组动作子表", CPublic.Var.geConn("PLC"));
            }
            //ABB检测动作组，选中的检测动作组进行删除的进行删除
            r.Delete();
            MasterSQL.Save_DataTable(dtP, "ABB检测类型动作组表", CPublic.Var.geConn("PLC"));
            //动作组表的检测组POS进行一个排序
            DataView dv1 = new DataView(dtP);
            dv1.Sort = "检测组POS";
            int j = 1;
            foreach (DataRowView drv in dv1)
            {
                DataRow r1 = drv.Row;
                drv.Row["检测组POS"] = j++;
            }
            MasterSQL.Save_DataTable(dtP, "ABB检测类型动作组表", CPublic.Var.geConn("PLC"));
       
        }


        private void fun_动作组表保存()
        {
            if (dtP != null && dtP.Rows.Count > 0)  //这就说明dtp是有数据的，需要进行子表的保存
            {
                dic.Clear();   //字典每一次都要进行一个清空操作
                dic1.Clear();
                dic2.Clear();
                DataTable dtPfu = new DataTable();  //dtP的副表，准备赋值修改后，但还未进行排序的dtP（动作组表）
                DataView dv = new DataView(dtP);
                dv.RowStateFilter = DataViewRowState.ModifiedOriginal;   //如果dtP有修改的情况下
                int x = dv.Count;
                if (x > 0)   //表示dtP表进行了修改
                {
                    for (int i = 0; i < dt_zifu.Rows.Count; i++)
                    {
                        string key = dt_zifu.Rows[i]["检测组POS"].ToString();
                        string value = dtP.Rows[i]["检测组POS"].ToString();
                        dic.Add(key, value);    //前一个对应关系    字典对应关系：key是最初的检测组排列顺序1，2，3，4，5...value是后来的用户改动的检测组顺序，还没有进行排序
                    }
                    dtPfu = dtP.Copy();     //把改动的顺序，还没有进行排序的进行赋值到另一个datatable中dtPfu中
                }

                #region 注释代码
                //保存dtP中改动过的顺序，先删后增，不然会出现主键调换的错误：即插入重复主键的错误
                //dtP.Clear();
                //fun_子表数据查询(); //重新查询DTP
                //foreach (DataRow r in dtP.Rows)
                //{
                //    r.Delete();
                //}
                //MasterSQL.Save_DataTable(dtP, "ABB检测类型动作组表", CPublic.Var.geConn("PLC"));    //删除原先的数据
                //foreach (DataRow r in dtPfu.Rows)
                //{
                //    dtP.Rows.Add(r.ItemArray);
                //}
                //MasterSQL.Save_DataTable(dtP, "ABB检测类型动作组表", CPublic.Var.geConn("PLC"));   //再保存Dtp
                //fun_子表数据查询(); //重新查询Dtp
                #endregion

                //保存子表数据  检测组POS需要进行排序
                DataView dv1 = new DataView(dtP);
                dv1.Sort = "检测组POS";
                int j = 1;
                foreach (DataRowView drv in dv1)
                {
                    DataRow r = drv.Row;
                    drv.Row["检测组POS"] = j++;
                }
                MasterSQL.Save_DataTable(dtP, "ABB检测类型动作组表", CPublic.Var.geConn("PLC"));

                if (x > 0)  //表示有修改操作
                {
                    foreach (DataRow r in dtPfu.Rows)
                    {
                        foreach (DataRow r1 in dtP.Rows)
                        {
                            if (r["检测组GUID"].ToString() == r1["检测组GUID"].ToString())
                            {
                                string key = r["检测组POS"].ToString();
                                string value = r1["检测组POS"].ToString();
                                dic1.Add(key, value);
                            }
                        }
                    }

                    foreach (var a in dic)
                    {
                        foreach (var b in dic1)
                        {
                            if (a.Value == b.Key)
                            {
                                string key = a.Key;
                                string value = b.Value;
                                dic2.Add(key, value);
                            }
                        }
                    }


                    DataTable dt_zi = new DataTable();
                    string sql = string.Format("select * from ABB检测组动作子表 where 检测ID='{0}' and 检测名称='{1}'", JCID.Text, JCMC.Text);
                    dt_zi = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
                    foreach (var c in dic2)
                    {
                        foreach (DataRow r in dt_zi.Rows)
                        {
                            if (r.RowState != DataRowState.Modified)   //已经做过修改的行就不需要进行赋值了
                            {
                                if (c.Key == r["检测组POS"].ToString())
                                {
                                    r["检测组POS"] = c.Value;
                                }
                            }
                        }
                    }
                    MasterSQL.Save_DataTable(dt_zi, "ABB检测组动作子表", CPublic.Var.geConn("PLC"));

                    #region  主键的调换，需要进行先删后增的操作，不然会出现主键重复的错误

                    //DataTable dt2 = new DataTable();
                    //dt2 = dt_zi.Copy();
                    //foreach (DataRow r in dt_zi.Rows)
                    //{
                    //    r.Delete();
                    //}
                    //MasterSQL.Save_DataTable(dt_zi, "ABB检测组动作子表", CPublic.Var.geConn("PLC"));

                    //foreach (DataRow r in dt2.Rows)
                    //{
                    //    dt_zi.Rows.Add(r.ItemArray);
                    //}
                    //MasterSQL.Save_DataTable(dt_zi, "ABB检测组动作子表", CPublic.Var.geConn("PLC"));

                    #endregion


                    string sql1 = string.Format("select * from ABB检测组动作子表 where 检测ID='{0}' and 检测名称='{1}'", JCID.Text, JCMC.Text);
                    dt_zi = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
                    DataView dv3 = new DataView(dt_zi);
                    dv3.Sort = "检测组POS,检测组内POS";
                    int f = 1;
                    foreach (DataRowView drv in dv3)
                    {
                        DataRow r = drv.Row;
                        drv.Row["动作POS"] = f++;
                    }
                    MasterSQL.Save_DataTable(dt_zi, "ABB检测组动作子表", CPublic.Var.geConn("PLC"));
                }
            }
        }

        //主表数据的保存
        private void fun_保存()
        {

            MasterSQL.Save_DataTable(dtM, "ABB检测类型主表", CPublic.Var.geConn("PLC"));
            JCMC.Enabled = false;  //锁定主键的textbox

            #region   注释代码 ： 删除的方案
            //如果有修改的
            //DataView dv = new DataView(dtP);
            //dv.RowStateFilter = DataViewRowState.ModifiedOriginal;  //原始数据的版本
            //if (dv.Count > 0)    //说明有修改的
            //{
            //    foreach (DataRowView drv in dv)
            //    {
            //        DataTable dt_zi = new DataTable();
            //        string sql = string.Format("select * from ABB检测组动作子表 where 检测ID='{0}' and 检测名称='{1}' and 检测组POS='{2}'", drv.Row["检测ID"].ToString(), drv.Row["检测名称"].ToString(), drv.Row["检测组POS"].ToString());
            //        dt_zi = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
            //        foreach (DataRow r1 in dt_zi.Rows)
            //        {
            //            r1.Delete();
            //        }
            //        MasterSQL.Save_DataTable(dt_zi, "ABB检测组动作子表", CPublic.Var.geConn("PLC"));
            //    }
            //}
            #endregion

            fun_动作组表保存();       
        }

        #endregion



        #region    界面操作

        //新增动作组数据的操作
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtP != null)
                    (this.BindingContext[dtP] as CurrencyManager).EndCurrentEdit();
                gvM2.CloseEditor();
                fun_新增动作组();  //新增一个动作组
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //查询操作  （查询主表某一条数据的信息）
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_主表查询();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //新增操作（新增主表的数据）
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_主表新增();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //主表数据删除的操作
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (JCMC.Enabled == true)
                {
                    MessageBox.Show("请先查询需要删除的数据！");
                }
                else
                {
                    if (MessageBox.Show(string.Format("你确定要删除检测ID为\"{0}\",检测名称为\"{1}\"的主数据吗？\n,删除之后，与其对应相关检测动作也将删除,请谨慎删除！", JCID.Text, JCMC.Text), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        fun_主表数据删除();
                        MessageBox.Show("删除成功！");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //复制动作组数据的操作
        private void simpleButton4_Click(object sender, EventArgs e)
        {
            try
            {
                if(dtP!=null)
                  (this.BindingContext[dtP] as CurrencyManager).EndCurrentEdit();
                gvM2.CloseEditor();
                fun_动作组复制();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //删除某一条动作组的数据
        private void simpleButton6_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtP == null || dtP.Rows.Count <= 0)
                    throw new Exception("无动作组可以删除！");
                (this.BindingContext[dtP] as CurrencyManager).EndCurrentEdit();
                gvM2.CloseEditor();
                DataRow r = (this.BindingContext[dtP].Current as DataRowView).Row;
                if (MessageBox.Show(string.Format("你确定要删除检测ID为\"{0}\",检测名称为\"{1}\",检测组顺序为\"{2}\"的动作组吗？", r["检测ID"].ToString(), r["检测名称"].ToString(), r["检测组POS"].ToString()), "提示？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    fun_删除动作组(r);
                    MessageBox.Show("删除成功！");
                }    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //主表数据的保存(包括子表数据)
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_check主表();  //先进行主表的数据检查
                if (dtP != null && dtP.Rows.Count > 0)   //子表有数据再进行检查
                {
                    (this.BindingContext[dtP] as CurrencyManager).EndCurrentEdit();
                    gvM2.CloseEditor();
                    fun_check子表();
                }
                fun_保存();
                MessageBox.Show("保存成功！");
                //新增之后立即删除会出现并发性错误，避免这个错误，强行的加载一遍
                jiancemc = drM["检测名称"].ToString();
                fun_load();
                DataRow[] dr = dtM.Select(string.Format("检测名称='{0}'", jiancemc));
                drM = dr[0];              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

        //查询   查询动作组表  即子表
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            fun_子表数据查询();
            JCMC.Enabled = false;  //查询的话，主键不可更改。
        }

        private void fun_主复制功能()
        {
            DataRow r = (this.BindingContext[dtM].Current as DataRowView).Row;   //选择主表dtM上的某一行
            drM = dtM.NewRow();  //新增一行
            drM.ItemArray = r.ItemArray;
            dataBindHelper1.DataFormDR(drM);  //复制到

            string sql1 = "select * from ABB检测类型动作组表 where 1<>1";
            dtP = MasterSQL.Get_DataTable(sql1, CPublic.Var.geConn("PLC"));

            string sql = string.Format("select * from ABB检测类型动作组表 where 检测ID='{0}' and 检测名称='{1}' order by 检测组POS", r["检测ID"].ToString(), r["检测名称"].ToString());
            dt_组复制 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
            //新表加数据
            foreach (DataRow r1 in dt_组复制.Rows)
            {
                r1["检测组GUID"] = System.Guid.NewGuid().ToString();
                dtP.Rows.Add(r1.ItemArray);
            }

            //动作子表
            //string sql2 = "select * from ABB检测组动作子表 where 1<>1";
            //dt_动作子 = MasterSQL.Get_DataTable(sql2, CPublic.Var.geConn("PLC"));

            //string sql3 = string.Format("select * from ABB检测组动作子表 where 检测ID='{0}' and 检测名称='{1}' order by 动作POS", r["检测ID"].ToString(), r["检测名称"].ToString());
            //dt_动作子复制 = MasterSQL.Get_DataTable(sql3, CPublic.Var.geConn("PLC"));
            
            //foreach(DataRow r)

            gcM2.DataSource = dtP;

            xtraTabControl1.SelectedTabPage = xtraTabPage1;
        }


        //复制主表功能
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            fun_主复制功能();
           // fun_子表数据查询(); //检测类型动作组子表查询出来
            JCID.Text = "";     //检测ID变为空
            JCMC.Text = "";     //检测名称变为空
            JCMC.Enabled = true;
        }

        //动作的详细设计
        private void simpleButton7_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtP == null || dtP.Rows.Count <= 0)
                    throw new Exception("无选中动作组，无法进行动作详细设计，请先新增动作组！");     
                DataRow r = (this.BindingContext[dtP].Current as DataRowView).Row;  //选中某一个动作组
                DataTable dt_count = MasterSQL.Get_DataTable(string.Format("select * from ABB检测类型动作组表 where 检测ID='{0}' and 检测名称='{1}' and 检测组POS='{2}'", r["检测ID"].ToString(), r["检测名称"].ToString(),r["检测组POS"].ToString()), CPublic.Var.geConn("PLC"));
                if(dt_count.Rows.Count<=0)
                    throw new Exception("新增动作组还没有保存，无法进行动作组的详细设计，请先保存！");
                fm检测动作子表维护 fm = new fm检测动作子表维护(r["检测ID"].ToString(), r["检测名称"].ToString(),r["检测组POS"].ToString());
                fm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion




    }
}
