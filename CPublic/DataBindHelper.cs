using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Data;
using System.Reflection;

namespace CPublic
{
    [ProvideProperty("BindFieldName", typeof(Control))]
    public class DataBindHelper : System.ComponentModel.Component , System.ComponentModel.IExtenderProvider
    {
        private System.ComponentModel.Container components;
        Dictionary<Control, string> LIC = new Dictionary<Control, string>();

        public Dictionary<string, string> BindSetting = new Dictionary<string, string>();

        public DataBindHelper()
        {
            this.components = new System.ComponentModel.Container();

            //BindSetting.Add("DevExpress.XtraEditors.ComboBoxEdit", "EditValue");
            BindSetting.Add("DevExpress.XtraEditors.DateEdit", "EditValue");
            BindSetting.Add("System.Windows.Forms.DateTimePicker", "Value");
            BindSetting.Add("System.Windows.Forms.CheckBox", "Checked");
            BindSetting.Add("default", "Text");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        bool IExtenderProvider.CanExtend(object target)
        {
            if (target is Control &&
                !(target is DataBindHelper))
            {

                return true;
            }
            return false;
        }

        #region 属性设定
        [
        DefaultValue(""),
        ]
        public string GetBindFieldName(Control control)
        {
            if (LIC.ContainsKey(control))
            {
                return LIC[control];
            }
            else
            {
                return "";
            }
        }

        public void SetBindFieldName(Control control, string value)
        {
            if (value == null)
            {
                value = "";
            }

            if (LIC.ContainsKey(control))
            {
                LIC[control] = value;
            }
            else
            {
                LIC.Add(control, value);
            }
        }
        #endregion

        #region 数据绑定

        public void DataBind(DataTable ds)
        {
            try
            {
                foreach (KeyValuePair<Control, string> kvp in LIC)
                {
                    if (kvp.Key is DateTimePicker)
                    {
                        kvp.Key.DataBindings.Add("Value", ds, kvp.Value);
                        continue;
                    }
                    if (kvp.Key is CheckBox)
                    {
                        kvp.Key.DataBindings.Add("Checked", ds, kvp.Value);
                        continue;
                    }

                    kvp.Key.DataBindings.Add("Text", ds, kvp.Value);
                   
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 数据读写
        /// <summary>
        /// 从DR把数据读到UI
        /// 
        /// </summary>
        /// <param name="dr"></param>
        public void DataFormDR(DataRow dr)
        {
            object ob;
            try
            {
                foreach (KeyValuePair<Control, string> kvp in LIC)
                {
                    if (dr.Table.Columns.IndexOf(kvp.Value) == -1) continue;
                    ob = kvp;
                    System.Type tp = kvp.Key.GetType();
                    Boolean bl = false;
                    foreach (KeyValuePair<string, string> kvpBind in BindSetting)
                    {
                        if (tp.FullName == kvpBind.Key)
                        {
                            bl = true;
                            setValueToGetProperty(dr,  kvp.Value ,kvp.Key,  kvpBind.Value);
                        }
                    }
                    if (bl == false)
                    {
                        setValueToGetProperty(dr, kvp.Value, kvp.Key, "Text");
                    }
                    //if (kvp.Key is DateTimePicker)
                    //{
                        
                    //    if (dr[kvp.Value] == DBNull.Value) continue;

                    //    ((DateTimePicker)kvp.Key).Value = (DateTime)dr[kvp.Value];
  
                    //    continue;
                    //}
                    //if (kvp.Key is CheckBox)
                    //{
                    //    if (dr[kvp.Value] == DBNull.Value)
                    //    {
                    //        ((CheckBox)kvp.Key).Checked = false;
                    //    }
                    //    else
                    //    {
                    //        ((CheckBox)kvp.Key).Checked = (Boolean)dr[kvp.Value];
                    //    }
                    //    continue;
                    //}

                    //if (dr[kvp.Value] == DBNull.Value)
                    //{
                    //    kvp.Key.Text = "";
                    //}
                    //else
                    //{
                    //    kvp.Key.Text = dr[kvp.Value].ToString();
                    //}
                   

                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        /// <summary>
        /// 动态赋值
        /// </summary>
        /// <param name="dr">DR</param>
        /// <param name="kvp">控件目标</param>
        /// <param name="BindetProperty">控件属性</param>
        /// <param name="tp"></param>
        private static void setValueToGetProperty(DataRow dr, KeyValuePair<Control, string> kvp, string BindetProperty, System.Type tp)
        {
            PropertyInfo pif = tp.GetProperty(BindetProperty);
            if (dr[kvp.Value] == DBNull.Value)
            {
                if (pif.PropertyType == true.GetType())
                {
                    pif.SetValue(kvp.Key, false, null);
                }
                if (pif.PropertyType == "1123".GetType())
                {
                    pif.SetValue(kvp.Key, "", null);
                }
                if (pif.PropertyType == 0.GetType())
                {
                    pif.SetValue(kvp.Key, 0, null);
                }
                if (pif.PropertyType == (1.0).GetType())
                {
                    pif.SetValue(kvp.Key, 0, null);
                }
                if (pif.PropertyType == (1.0M).GetType())
                {
                    pif.SetValue(kvp.Key, 0M, null);
                }
            }
            else
            {
                if (pif.PropertyType == "123".GetType())
                {
                    pif.SetValue(kvp.Key, dr[kvp.Value].ToString(), null);
                }
                else
                {
                    pif.SetValue(kvp.Key, dr[kvp.Value], null);
                }
            }
        }
        /// <summary>
        /// 动态赋值
        /// </summary>
        /// <param name="dr">DR</param>
        /// <param name="strColumn">DR的列名</param>
        /// <param name="ctl">控件</param>
        /// <param name="BindetProperty">控件的属性名</param>
        private static void setValueToGetProperty(DataRow dr, string strColumn,object ctl, string BindetProperty)
        {
            PropertyInfo pif = ctl.GetType().GetProperty(BindetProperty);
            if (dr[strColumn] == DBNull.Value)
            {
                if (pif.PropertyType == true.GetType())
                {
                    pif.SetValue(ctl, false, null);
                }
                if (pif.PropertyType == "1123".GetType())
                {
                    pif.SetValue(ctl, "", null);
                }
                if (pif.PropertyType == 0.GetType())
                {
                    pif.SetValue(ctl, 0, null);
                }
                if (pif.PropertyType == (1.0).GetType())
                {
                    pif.SetValue(ctl, 0, null);
                }
                if (pif.PropertyType == (1.0M).GetType())
                {
                    pif.SetValue(ctl, 0M, null);
                }
            }
            else
            {
                if (pif.PropertyType == "123".GetType())
                {
                    pif.SetValue(ctl, dr[strColumn].ToString(), null);
                }
                else
                {
                    pif.SetValue(ctl, dr[strColumn], null);
                }
            }
        }
        /// <summary>
        /// 从UI把数据回写到DR
        /// </summary>
        /// <param name="dr"></param>
        public void DataToDR(DataRow dr)
        {
            try
            {
                foreach (KeyValuePair<Control, string> kvp in LIC)
                {
                    if (dr.Table.Columns.IndexOf(kvp.Value) == -1) continue;
                    System.Type tp = kvp.Key.GetType();
                    Boolean bl = false;
                    foreach (KeyValuePair<string, string> kvpBind in BindSetting)
                    {                                             
                        if (tp.FullName == kvpBind.Key)
                        {
                            bl = true;
                            PropertyInfo pif = tp.GetProperty(kvpBind.Value);
                            if (pif.PropertyType == "123".GetType())
                            {
                                try
                                {
                                    dr[kvp.Value] = System.Convert.ChangeType(pif.GetValue(kvp.Key, null), dr.Table.Columns[kvp.Value].DataType);
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.Message + kvp.Value.ToString());
                                }
                            }
                            else
                            {
                                //if (pif.GetValue(kvp.Key, null) != null || pif.GetValue(kvp.Key, null) != "")
                                //{
                                //    dr[kvp.Value] = pif.GetValue(kvp.Key, null);
                                //}
                                try
                                {
                                    dr[kvp.Value] = pif.GetValue(kvp.Key, null);
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.Message + kvp.Value.ToString());
                                }
                            }
                        }
                    }
                    if (bl == false)
                    {
                        PropertyInfo pif = tp.GetProperty("Text");
                        if (pif.PropertyType == "123".GetType())
                        {
                            try
                            {
                                dr[kvp.Value] = System.Convert.ChangeType(pif.GetValue(kvp.Key, null), dr.Table.Columns[kvp.Value].DataType);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message + kvp.Value.ToString());
                            }
                        }
                        else
                        {
                            //if (pif.GetValue(kvp.Key, null) != null || pif.GetValue(kvp.Key, null) != "")
                            //{
                            //    dr[kvp.Value] = pif.GetValue(kvp.Key, null);
                            //}
                            try
                            {
                                dr[kvp.Value] = pif.GetValue(kvp.Key, null);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message + kvp.Value.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }

   


}
