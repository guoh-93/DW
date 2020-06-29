using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;


namespace BaseData
{
    class Class1
    {
        //Class1 cl;
        string strsave = "";
        
        //public void fun_数组来源()
        //{
        //    arr = new ArrayList();
        //    arr.Add("a");
        //    arr.Add("b");
        //    arr.Add("c");
        //    arr.Add("d");
        //    arr.Add("e");
        //}
   
        public void fun_创建和保存(ArrayList a)  //生成一个xml文件
        {
            //创建
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><root></root>");
            XmlElement rootElem = xmlDoc.DocumentElement;   //获取根节点 
            //创建arr
            ArrayList arr = new ArrayList();
            arr = a;
            for (int i = 0; i < arr.Count; i++)
            {
                strsave = strsave + arr[i].ToString();
                if (i == (arr.Count - 1))
                {

                }
                else
                {
                    strsave = strsave + ",";
                }
            }
            XmlElement elemkey = xmlDoc.CreateElement("key");
            rootElem.AppendChild(elemkey);
            XmlElement elemTmp = xmlDoc.CreateElement("value");
            elemTmp.InnerXml = strsave.ToString();
            elemkey.AppendChild(elemTmp);
            string file = "C:/Test.xml";
            xmlDoc.Save(file);
        }  //创建

        public ArrayList fun_读取()  //读取xml文件，输出str字符串
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(@"C:\Test.xml");
            XmlElement rootElem = xml.DocumentElement;
            XmlNodeList personNodes = rootElem.GetElementsByTagName("key");
            XmlNodeList subAgeNodes = ((XmlElement)personNodes[0]).GetElementsByTagName("value");
            string str = subAgeNodes[0].InnerText.ToString();
            ArrayList ar = new ArrayList();
            string[] a = str.Split(',');
            for (int j = 0; j < a.Length; j++)
            {
                ar.Add(a[j]);
            }
            return ar;
            //return str;
        }

        //public ArrayList fun_拆分(string str)  //将字符串拆分成一个arr数组
        //{
        //    ArrayList ar = new ArrayList();
        //    string[] a = str.Split(',');
        //    for (int j = 0; j < a.Length; j++)
        //    {
        //        ar.Add(a[j]);
        //    }
        //    return ar;
        //}
       
    }
}
