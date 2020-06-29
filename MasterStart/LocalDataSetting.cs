using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Data;

namespace CZMaster
{
    /// <summary>
    /// 提供保存本地数据。重复数据会按后插入的数据为POS。
    /// </summary>
    public static class LocalDataSetting
    {
        public static string appDesc = "";
        
        static string path = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\LocalDataSetting.cfg";
        
        public static  List<string> getLocalData(string strKey)
        {
            List<string> LI = new List<string>();
            try
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
            }
            catch { }
            DSLocaldata.LocalDataDataTable dt = new DSLocaldata.LocalDataDataTable();
            if (System.IO.File.Exists(path))
            {
                dt.ReadXml(path);
            }
            else
            {
                return LI;
            }
            DataRow rM;
            DataRow[] ds = dt.Select(string.Format("key = '{0}'", strKey));
            if (ds.Length == 0)
            {
                return LI;
            }
            else
            {
                rM = ds[1];
            }
            string[] liser = rM["value"].ToString().Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            return new List<string>(liser);
        }
        public static void  Clear(string strKey)
        {
            
        }
        public static void Delete(string strKey)
        {
        
        }
        public static void setLocalKeyMaxCount(string strKey,int MaxCount)
        {
        
        }
        public static void addLocalData(string strKey, string strValue)
        {
            try{
                System.IO.Directory.CreateDirectory( System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
            }
            catch{}
            DSLocaldata.LocalDataDataTable dt = new DSLocaldata.LocalDataDataTable();
            if (System.IO.File.Exists(path))
            {
                dt.ReadXml(path);
            }
            DataRow rM;
            int iMax = 10;
            DataRow[] ds = dt.Select(string.Format("key = '{0}'",strKey));
            if(ds.Length == 0)
            {
                dt.Rows.Add(strKey,0,"10");
                rM = dt.Rows.Add(strKey,1,"");
            }
            else
            {
                iMax = int.Parse(ds[0]["value"].ToString());
                rM = ds[1];
            }
            string[] liser = rM["value"].ToString().Split(new string[]{System.Environment.NewLine},StringSplitOptions.RemoveEmptyEntries);

            List<string> li = new List<string>(liser);
            if (li.IndexOf(strValue) >= 0)
            {
                li.Remove(strValue);
            }
            li.Insert(0, strValue);

            try
            {
                while (li.Count > iMax)
                {
                    li.RemoveAt(li.Count - 1);
                }
            }
            catch { };

            string SS = "";
            foreach (string s in li)
            {
                SS += s + System.Environment.NewLine;
            }
            rM["value"] = SS;

            dt.WriteXml(path);
        }
    }
}
