using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;




namespace ERPorg
{
    [ProvideProperty("gridInder", typeof(DevExpress.XtraGrid.GridControl))]

    public partial class ghinder : System.ComponentModel.Component, System.ComponentModel.IExtenderProvider
    {
        /// <summary>
        /// 1 是gridcontrol 2 是treelist
        /// </summary>
        int xx = 0;
        /// <summary>
        /// 只适用dev gridcontrol
        /// 功能：1.自动保存gridView 界面设置;2.数字列可以拉取多选显示总和;3.支持Ctrl+C复制 4.支持Ctrl+V 复制数据源一列数据到gridcontrol中的可编辑列 5.自动显示行号
        /// 注意事项：如果有选择列触发事件 比如 勾选往另外一个gv赋值的最好不要用这个控件  或者参考 生产制令界面的处理 
        /// 2019-4-4 郭恒
        /// </summary>
        public ghinder()
        {
            InitializeComponent();
        }
        private int width = 40;
        //可设置宽度
        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        private string uiname = "";

        /// <summary>
        /// 当前窗体名称,保存界面gridview设置的时候 命名规则  uiname+"_"+gv.name 确保唯一性
        /// </summary>
        public string UIName
        {

            get { return uiname; }
            set { uiname = value; }
        }

        bool B_v = true;
        /// <summary>
        /// 可设置是否自动显示行号
        /// </summary>
        public bool bool_V
        {
            get { return B_v; }
            set { B_v = value; }
        }
        bool totalcopy = true;
        /// <summary>
        /// 可设置是否自动显示行号
        /// </summary>
        public bool TotalCopy
        {
            get { return totalcopy; }
            set { totalcopy = value; }
        }


        bool bl_CtrlV = false;
        /// <summary>
        /// 是否启用可复制数据进gridView
        /// </summary>
        public bool EnableCtrlV
        {
            get { return bl_CtrlV; }
            set { bl_CtrlV = value; }
        }

        public ghinder(IContainer container)
        {
            this.components = new System.ComponentModel.Container();
        }
        bool IExtenderProvider.CanExtend(object target)
        {
            if (target is DevExpress.XtraGrid.GridControl)
            {
                return true;
            }
            return false;
        }
        [DefaultValue(""),]
        Dictionary<DevExpress.XtraGrid.GridControl, string> Dic = new Dictionary<DevExpress.XtraGrid.GridControl, string>();
        Dictionary<DevExpress.XtraGrid.GridControl, string> Dic_1 = new Dictionary<DevExpress.XtraGrid.GridControl, string>();
        public string GetgridInder(DevExpress.XtraGrid.GridControl control)
        {

            if (Dic.ContainsKey(control))
            {
                return Dic[control];
            }
            else
            {
                return "";
            }

        }

        /// </summary>
        /// <param name="control"></param>
        /// <param name="value"></param>
        public void SetgridInder(DevExpress.XtraGrid.GridControl control, string value)
        {
            if (value == null)
            {
                value = "";
            }
            if (value == "")
                return;
            if (Dic.ContainsKey(control))
            {
                Dic[control] = value;
            }
            else
            {
                Dic.Add(control, value);
                if (this.DesignMode) return;
                if (control.MainView != null)
                {
                    Dic_1.Add(control, "Y");
                }
                control.DataSourceChanged += control_DataSourceChanged;
                control.KeyDown += Control_KeyDown;

            }
        }

        private void Gv_ColumnFilterChanged(object sender, EventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView gv = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            // DevExpress.XtraGrid.GridControl control = gv.GridControl;
            //string ss= Dic[control]; //窗体名称
            string filepath = System.Windows.Forms.Application.StartupPath + @"\FormLayout";
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }
            gv.SaveLayoutToXml(filepath + string.Format(@"\{0}.xml", UIName + "_" + gv.Name));
        }

        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.C)
                {
                    DevExpress.XtraGrid.GridControl c = sender as DevExpress.XtraGrid.GridControl;
                    DevExpress.XtraGrid.Views.Grid.GridView g = c.DefaultView as DevExpress.XtraGrid.Views.Grid.GridView;

                    string s = "";
                    DevExpress.XtraGrid.Views.Base.GridCell[] gcell = g.GetSelectedCells();
                    if (gcell.Length > 0)
                    {
                        IDataObject iData = Clipboard.GetDataObject();
                        string sx = (String)iData.GetData(DataFormats.Text);

                        int index = gcell[0].RowHandle;

                        for (int x = 0; x < gcell.Length; x++)
                        {
                            s += g.GetRowCellValue(gcell[x].RowHandle, gcell[x].Column);
                            if (x + 1 >= gcell.Length)
                            { }
                            else if (gcell[x + 1].RowHandle > gcell[x].RowHandle) s += "\r\n";
                            else
                            {
                                s += "\t";
                            }
                        }
                        Clipboard.SetDataObject(s);
                    }
                    else
                    {

                        Clipboard.SetDataObject(g.GetFocusedRowCellValue(g.FocusedColumn).ToString());
                    }
                    //foreach (DevExpress.XtraGrid.Views.Base.GridCell j in gcell)
                    //{
                    //    s += gridView1.GetRowCellValue(j.RowHandle, j.Column);
                    //    if (index < j.RowHandle)
                    //        s += "\r\n";
                    //    else
                    //        s += "\t";


                    //}

                    //  Clipboard.SetDataObject(g.GetFocusedRowCellValue(g.FocusedColumn).ToString());
                    // object d = g.GetFocusedRowCellValue(g.FocusedColumn);
                    e.Handled = true;
                }
                //有限制,当界面的物料编码列 用的不是 cellvaluechanged或changing事件  不能触发联动，当一个料对应好几个仓库用的 是下拉框控件视图的点击事件

                else if (e.Control && e.KeyCode == Keys.V)
                {
                    if (bl_CtrlV)
                    {

                        IDataObject iData = Clipboard.GetDataObject();
                        if (iData.GetDataPresent(DataFormats.Text))
                        {
                            string s = (String)iData.GetData(DataFormats.Text);
                            DevExpress.XtraGrid.GridControl c = sender as DevExpress.XtraGrid.GridControl;
                            DevExpress.XtraGrid.Views.Grid.GridView g = c.DefaultView as DevExpress.XtraGrid.Views.Grid.GridView;
                            string[] xx = s.Split('\n');
                            xx = xx.Where(r => !string.IsNullOrEmpty(r)).ToArray();
                            //DataTable t = (c.DataSource as DataTable);
                            DataTable t = c.DataSource as DataTable;
                            if (g.RowCount == 0)
                            {
                                foreach (DevExpress.XtraGrid.Columns.GridColumn dc in g.Columns)
                                {
                                    if (dc.FieldName == "物料编码" || dc.FieldName == "子项编码")
                                        g.FocusedColumn = dc;
                                    break;
                                }

                            }
                            //选中列  g.FocusedColumn 如果物料编码新增行  如果不是 根据选中行选中列 index 增加
                            if (g.FocusedColumn.FieldName == "物料编码" || g.FocusedColumn.FieldName == "子项编码")
                            {
                                foreach (string x in xx)
                                {
                                    DataRow dr = t.NewRow();
                                    dr[g.FocusedColumn.FieldName] = x.Trim();

                                    t.Rows.Add(dr);

                                }
                                for (int i = t.Rows.Count - 1; i >= 0; i--)
                                {
                                    if (t.Rows[i][g.FocusedColumn.FieldName].ToString().Trim() == "")
                                    {
                                        t.Rows.RemoveAt(i);
                                    }
                                }

                            }
                            else
                            {
                                //19-10-28 必须列不可编辑才可以 复制进去
                                if (g.Editable && g.FocusedColumn.OptionsColumn.AllowEdit)
                                {
                                    int index = g.GetFocusedDataSourceRowIndex();//焦点行索引
                                    // g.FocusedColumn.OptionsColumn.AllowEdit = false;
                                    if (t.Rows.Count > 0 && t.Rows.Count - index >= xx.Length)
                                    {
                                        foreach (string x in xx)
                                        {
                                            t.Rows[index++][g.FocusedColumn.FieldName] = x.Trim();

                                        }
                                    }
                                    else
                                    {
                                        t.Rows[index++][g.FocusedColumn.FieldName] = DBNull.Value;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("复制的内容格式有误");
            }

        }
        private void control_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.GridControl c = sender as DevExpress.XtraGrid.GridControl;
                //DevExpress.XtraTreeList.TreeList tl = sender as DevExpress.XtraTreeList.TreeList;
                if (B_v == true)
                {
                    fun_load(c);
                }
            }
            catch (Exception)
            {
            }
        }
        private void fun_load(DevExpress.XtraGrid.GridControl gc)
        {
            if (gc is DevExpress.XtraGrid.GridControl)
            {
                DevExpress.XtraGrid.Views.Grid.GridView gv = gc.DefaultView as DevExpress.XtraGrid.Views.Grid.GridView;
                gv.IndicatorWidth = width;

                gv.CustomDrawRowIndicator -= Gv_CustomDrawRowIndicator;
                gv.ColumnFilterChanged -= Gv_ColumnFilterChanged;
                gv.ColumnPositionChanged -= Gv_ColumnPositionChanged;
                gv.ColumnWidthChanged -= Gv_ColumnWidthChanged;
                gv.RowCellClick -= Gv_RowCellClick;

                gv.CustomDrawRowIndicator += Gv_CustomDrawRowIndicator;
                gv.ColumnFilterChanged += Gv_ColumnFilterChanged;
                gv.ColumnPositionChanged += Gv_ColumnPositionChanged;
                gv.ColumnWidthChanged += Gv_ColumnWidthChanged;

                #region 统一界面设置
                gv.Appearance.HideSelectionRow.BackColor = System.Drawing.SystemColors.InactiveCaption;
                gv.Appearance.HideSelectionRow.ForeColor = System.Drawing.Color.Black;
                gv.Appearance.SelectedRow.BackColor = System.Drawing.SystemColors.InactiveCaption;
                gv.Appearance.SelectedRow.ForeColor = System.Drawing.Color.Black;
                gv.Appearance.FilterPanel.ForeColor = System.Drawing.Color.Black;
                gv.OptionsLayout.StoreDataSettings = false;
                gv.Appearance.FilterPanel.Font = new System.Drawing.Font("Tahoma", 12F);
                if (bl_CtrlV || totalcopy)
                {
                    gv.OptionsSelection.MultiSelect = true;
                    gv.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
                    gv.RowCellClick += Gv_RowCellClick;
                }
                foreach (DevExpress.XtraGrid.Columns.GridColumn gcolumn in gv.Columns)
                {
                    gcolumn.AppearanceHeader.Options.UseTextOptions = true;
                    gcolumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

                }

                #endregion
            }


        }


        private void Gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Column.ColumnType.Name == "Decimal" || e.Column.ColumnType.Name == "Double" || e.Column.ColumnType.Name == "Int")
                {
                    DevExpress.XtraGrid.Views.Grid.GridView gv = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    DevExpress.XtraGrid.Views.Base.GridCell[] gcell = gv.GetSelectedCells();
                    if (gcell.Length > 1)
                    {
                        decimal dec = 0;
                        int index = gcell[0].RowHandle;
                        for (int x = 0; x < gcell.Length; x++)
                        {

                            dec += Convert.ToDecimal(gv.GetRowCellValue(gcell[x].RowHandle, gcell[x].Column));
                        }

                        Label la = new Label();
                        la.Text = dec.ToString("#0.##");

                        DevExpress.XtraGrid.GridControl c = sender as DevExpress.XtraGrid.GridControl;

                        c = gv.GridControl;

                        c.Controls.Add(la);
                        la.Left = e.X + 20;
                        la.Top = e.Y;
                        la.BackColor = System.Drawing.SystemColors.ControlLight;
                        la.Font = new System.Drawing.Font("宋体", 12F);
                        la.AutoSize = true;

                        c.Refresh();
                        Thread th = new Thread(() =>
                             {

                                 killlabel(c);

                             });

                        th.Start();

                    }
                }
                else if (e.Column.ColumnType.Name == "Boolean" && !e.Column.ReadOnly && e.Column.OptionsColumn.AllowEdit)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView gv = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (!gv.OptionsBehavior.ReadOnly && gv.OptionsBehavior.Editable)
                    {
                        DevExpress.XtraGrid.Views.Base.GridCell[] gcell = gv.GetSelectedCells();
                        if (gcell.Length > 0)
                        {
                            int index = gcell[0].RowHandle;
                            //DataTable t = gv.GridControl.DataSource as DataTable;
                            for (int x = 0; x < gcell.Length; x++)
                            {
                                if (gcell[x].Column.ColumnType.Name == "Boolean" && !gcell[x].Column.ReadOnly && gcell[x].Column.OptionsColumn.AllowEdit)
                                {

                                    bool bl = false;
                                    if (e.CellValue == null || e.CellValue == DBNull.Value) bl = false;
                                    else
                                    {
                                        bl = Convert.ToBoolean(e.CellValue);
                                    }
                                    gv.SetRowCellValue(gcell[x].RowHandle, gcell[x].Column, !bl);
                                    // t.Rows[gcell[x].RowHandle]["gcell[x].Column"] = !bl;
                                }
                            }
                            gv.CloseEditor();
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        delegate void SetTextCallback(DevExpress.XtraGrid.GridControl gc);
        private void killlabel(DevExpress.XtraGrid.GridControl c)
        {
            if (c.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!c.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (c.Disposing || c.IsDisposed) return;
                }
                SetTextCallback d = new SetTextCallback(killlabel);
                c.Invoke(d, c);
            }
            else
            {
                //foreach (object a in c.Controls)
                //{
                //    var v = a.GetType();
                //    if (v.Name == "Label")
                //    {
                //        Label label = a as Label;
                //        c.Controls.Remove(label);
                //    }
                //}
                Thread.Sleep(800);
                for (int x = 0; x < c.Controls.Count; x++)
                {
                    Control v = c.Controls[x];
                    var f = v.GetType();
                    if (f.Name == "Label")
                    {
                        c.Controls.Remove(v);
                    }
                }
                c.Refresh();
            }
        }

        private void Gv_ColumnWidthChanged(object sender, DevExpress.XtraGrid.Views.Base.ColumnEventArgs e)
        {

            DevExpress.XtraGrid.Views.Grid.GridView gv = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            string filepath = System.Windows.Forms.Application.StartupPath + @"\FormLayout";
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }
            gv.SaveLayoutToXml(filepath + string.Format(@"\{0}.xml", UIName + "_" + gv.Name));
        }

        private void Gv_ColumnPositionChanged(object sender, EventArgs e)
        {
            object v = (sender as DevExpress.XtraGrid.Columns.GridColumn).View;
            DevExpress.XtraGrid.Views.Grid.GridView gv = v as DevExpress.XtraGrid.Views.Grid.GridView;
            //DevExpress.XtraGrid.GridControl control = gv.GridControl;
            //string ss = Dic[control]; //窗体名称

            string filepath = System.Windows.Forms.Application.StartupPath + @"\FormLayout";
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }
            gv.SaveLayoutToXml(filepath + string.Format(@"\{0}.xml", UIName + "_" + gv.Name));
        }

        private void Gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

    }
}
