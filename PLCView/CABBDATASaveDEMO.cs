using System;
using System.Collections.Generic;
using System.Text;

namespace PLCView
{
    class CABBDATASaveDEMO
    {
        string str总GUID = "";
        Dictionary<int, string> li_组GUID = new Dictionary<int, string>();
        Dictionary<int, string> li_主GUID = new Dictionary<int, string>();

        private void fun_删除总动作()
        {

        }

        private void fun_保存总动作(PLCC.ResultR rr)
        {
            //查看str总GUID是否存在，如果不存在，新增，如果存在， 查看R是否不是机构动作，如果不是。 删除后新增
            if (str总GUID == "")
            {
                str总GUID = System.Guid.NewGuid().ToString();
                fun_save保存总动作(rr, str总GUID);
            }
            else
            {
                //R 是否不是机构动作
                //如果是，跳出
                //如果不是，执行如下动作
                fun_删除总动作();
                //fun_save保存总动作(R, str总GUID);
            }
        }

        private void fun_save保存总动作(PLCC.ResultR R,string GUID)
        {



        }

        private void fun_删除组动作(string  str组GUID)
        {

        }
        private void fun_保存组动作(PLCC.ResultR R)
        {
            int i_组POS = 0;

            //通过 R 得到 i_组POS

            if (li_组GUID.ContainsKey(i_组POS) == false)
            {
                li_组GUID.Add(i_组POS, System.Guid.NewGuid().ToString());

                fun_保存组动作(R,str总GUID, li_组GUID[i_组POS]);
            }
            else
            {
                //R 是否不是机构动作
                //如果是，跳出
                //如果不是，执行如下动作
                fun_删除组动作(li_组GUID[i_组POS]);
                fun_保存组动作(R, str总GUID,li_组GUID[i_组POS]);

            }

        }

        private void fun_保存组动作(PLCC.ResultR R, string GUID总,string GUID组)
        {

        }

        private void fun_保存主动作(PLCC.ResultR R)
        {

        }

        private void fun_保存动作(PLCC.ResultR R)
        {

        }

        private void fun(PLCC.ResultR rr)
        {
            fun_保存总动作(rr);
            fun_保存组动作(rr);
            fun_保存主动作(rr);
            fun_保存动作(rr);


        }

    }
}
