using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data;


namespace CZMaster
{
    /// <summary>
    /// 功能和LocalDataSetting一样
    /// 可以重复插入数据。没有数据上限。
    /// </summary>
    public static  class LocalDataSettingBIN
    {
        public static string appDesc = "";

        static string path = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\LocalDataBINSetting.cfg";

        public static List<string> getLocalData(string strKey)
        {
            List<string> LI = new List<string>();
            try
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
            }
            catch { }
            DSLocaldata.LocalDataBINDataTable dt = new DSLocaldata.LocalDataBINDataTable();
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
                rM = ds[0];
            }
            try
            {
                System.IO.MemoryStream msm = new System.IO.MemoryStream((byte[])rM["value"]);
                BinaryFormatter bf = new BinaryFormatter();
                return (List<string>)bf.Deserialize(msm);
            }
            catch
            {
                return LI;
            }
            //string[] liser = rM["value"].ToString().Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            //return new List<string>(liser);
        }

        public static void Delete(string strKey)
        {
            List<string> LI = new List<string>();
            try
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
            }
            catch { }
            DSLocaldata.LocalDataBINDataTable dt = new DSLocaldata.LocalDataBINDataTable();
            if (System.IO.File.Exists(path))
            {
                dt.ReadXml(path);
            }
            else
            {
                return ;
            }

            DataRow[] ds = dt.Select(string.Format("key = '{0}'", strKey));
            if (ds.Length == 0)
            {
                return ;
            }
            else
            {
                dt.Rows.Remove(ds[0]);
            }
            try
            {
                dt.WriteXml(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void addLocalData(string strKey, string strValue)
        {
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
            DataRow rM;
            DataRow[] ds = dt.Select(string.Format("key = '{0}'", strKey));

            List<string> li;
            if (ds.Length == 0)
            {
                rM = dt.Rows.Add(strKey, 1);
                li = new List<string>();
            }
            else
            {
                rM = ds[0];
                System.IO.MemoryStream msm = new System.IO.MemoryStream((byte[])rM["value"]);
                BinaryFormatter bf = new BinaryFormatter();
                li = (List<string>)bf.Deserialize(msm);
            }

            li.Add(strValue);

            {
                System.IO.MemoryStream msm = new System.IO.MemoryStream();
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(msm, bf);
                rM["value"] = msm.ToArray();
            }
            try
            {
                dt.WriteXml(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
            //string[] liser = rM["value"].ToString().Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            //List<string> li = new List<string>(liser);
            //if (li.IndexOf(strValue) >= 0)
            //{
            //    li.Remove(strValue);
            //}
            //li.Insert(0, strValue);

            //try
            //{
            //    while (li.Count > iMax)
            //    {
            //        li.RemoveAt(li.Count - 1);
            //    }
            //}
            //catch { };

            //string SS = "";
            //foreach (string s in li)
            //{
            //    SS += s + System.Environment.NewLine;
            //}
            //rM["value"] = SS;

            //dt.WriteXml(path);
        }

        public static void addLocalData(string strKey, string[] strValue)
        {
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
            DataRow rM;
            DataRow[] ds = dt.Select(string.Format("key = '{0}'", strKey));

            List<string> li;
            if (ds.Length == 0)
            {
                rM = dt.Rows.Add(strKey, 1);
                li = new List<string>();
            }
            else
            {
                rM = ds[0];
                System.IO.MemoryStream msm = new System.IO.MemoryStream((byte[])rM["value"]);
                BinaryFormatter bf = new BinaryFormatter();
                li = (List<string>)bf.Deserialize(msm);
            }

            li.AddRange(strValue);

            {
                System.IO.MemoryStream msm = new System.IO.MemoryStream();
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(msm, bf);
                rM["value"] = msm.ToArray();
            }
            try
            {
                dt.WriteXml(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    
}
