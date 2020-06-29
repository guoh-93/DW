using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CPublic;
using CZMaster;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BaseData
{
    public partial class fm员工信息列 : Form
    {

        #region   公有变量

        /// <summary>
        /// 获取列名
        /// </summary>
        public ArrayList arry;


        #endregion


        #region   私有变量

        /// <summary>
        /// 选择列表
        /// </summary>
        DataTable dt_选择列;

        /// <summary>
        /// 所有员工
        /// </summary>
        DataTable dt_员工表;




        #endregion


        #region  类加载

        public fm员工信息列()
        {
            InitializeComponent();
        }

        private void fm员工信息列_Load(object sender, EventArgs e)
        {
            //MasterCommon.LocalDataSetting.appDesc = "basedateitem";
            //MasterCommon.LocalDataSetting.setLocalKeyMaxCount("basedateitemchoose", 100);
            //arry = new ArrayList(MasterCommon.LocalDataSetting.getLocalData("basedateitemchoose"));

            //读取
            try
            {
                FileStream fs = new FileStream("user.game", FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                arry = (ArrayList)bf.Deserialize(fs);
                fs.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("读取失败！！！");
            }



            fun_load();

            for (int i = 0; i < arry.Count; i++)
            {
                foreach (DataRow r in dt_选择列.Rows)
                {
                    if (arry[i].ToString().Equals(r["列名"].ToString()))
                    {
                        r["选择"] = true;
                    }
                }
             }





        }

        #endregion

        #region  其他数据处理

      

        #endregion

        #region  数据库的读取

        private void fun_load()
        {
            dt_员工表=MasterSQL.Get_DataTable("select * from 人事基础员工表",CPublic.Var.strConn);
            dt_选择列 = new DataTable();
            dt_选择列.Columns.Add("列名");
            dt_选择列.Columns.Add("选择",true.GetType());

            int i = 0;
            foreach (DataColumn d in dt_员工表.Columns)
            {
                if (i == 0)
                {
                    i++;
                    continue;
                }
                dt_选择列.Rows.Add(d.ColumnName);
            }




            dgv.DataSource = dt_选择列;
        }

        #endregion


        #region   界面操作

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
       {
            arry = new ArrayList();
            foreach (DataRow r in dt_选择列.Rows)
            {
                if (r["选择"].Equals(true))
                {
                    arry.Add(r["列名"].ToString());
                }
            }

            //序列化保存
            try
            {
                FileStream fs = new FileStream("user.game", FileMode.Create);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, arry);
                fs.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("保存失败！！！");
            }


            //for (int i = 0; i < arry.Count; i++)
            //{
            //   MasterCommon.LocalDataSetting.addLocalData("basedateitemchoose", arry[i].ToString());
            //}

            this.Close();





        }

        #endregion




    }
}
