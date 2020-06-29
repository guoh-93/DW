namespace approval
{
    partial class ui单据审核界面
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.barLargeButtonItem1 = new DevExpress.XtraBars.BarLargeButtonItem();
            this.barLargeButtonItem2 = new DevExpress.XtraBars.BarLargeButtonItem();
            this.barLargeButtonItem4 = new DevExpress.XtraBars.BarLargeButtonItem();
            this.barLargeButtonItem3 = new DevExpress.XtraBars.BarLargeButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.gc1 = new DevExpress.XtraGrid.GridControl();
            this.gv1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn34 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcP = new DevExpress.XtraGrid.GridControl();
            this.gvP = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn15 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn17 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn20 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn24 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip();
            this.跳转详细信息ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.发货信息完善ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gc1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvP)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar2});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barLargeButtonItem1,
            this.barLargeButtonItem2,
            this.barLargeButtonItem3,
            this.barLargeButtonItem4});
            this.barManager1.MainMenu = this.bar2;
            this.barManager1.MaxItemId = 4;
            // 
            // bar2
            // 
            this.bar2.BarName = "Main menu";
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barLargeButtonItem1, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barLargeButtonItem2, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barLargeButtonItem4, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barLargeButtonItem3, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            // 
            // barLargeButtonItem1
            // 
            this.barLargeButtonItem1.Caption = "刷新";
            this.barLargeButtonItem1.Glyph = global::approval.Properties.Resources.GenerateData_32x32;
            this.barLargeButtonItem1.Id = 0;
            this.barLargeButtonItem1.Name = "barLargeButtonItem1";
            this.barLargeButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem1_ItemClick);
            // 
            // barLargeButtonItem2
            // 
            this.barLargeButtonItem2.Caption = "审核";
            this.barLargeButtonItem2.Glyph = global::approval.Properties.Resources.Mark_32x32;
            this.barLargeButtonItem2.Id = 1;
            this.barLargeButtonItem2.Name = "barLargeButtonItem2";
            this.barLargeButtonItem2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem2_ItemClick);
            // 
            // barLargeButtonItem4
            // 
            this.barLargeButtonItem4.Caption = "驳回";
            this.barLargeButtonItem4.Enabled = false;
            this.barLargeButtonItem4.Glyph = global::approval.Properties.Resources.Cancel_32x32;
            this.barLargeButtonItem4.Id = 3;
            this.barLargeButtonItem4.Name = "barLargeButtonItem4";
            this.barLargeButtonItem4.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem4_ItemClick);
            // 
            // barLargeButtonItem3
            // 
            this.barLargeButtonItem3.Caption = "关闭界面";
            this.barLargeButtonItem3.Glyph = global::approval.Properties.Resources.Close_32x32;
            this.barLargeButtonItem3.Id = 2;
            this.barLargeButtonItem3.Name = "barLargeButtonItem3";
            this.barLargeButtonItem3.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem3_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1473, 60);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 680);
            this.barDockControlBottom.Size = new System.Drawing.Size(1473, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 60);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 620);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1473, 60);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 620);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 60);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.checkBox1);
            this.splitContainer1.Panel1.Controls.Add(this.gc1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.gcP);
            this.splitContainer1.Size = new System.Drawing.Size(1473, 620);
            this.splitContainer1.SplitterDistance = 708;
            this.splitContainer1.SplitterWidth = 8;
            this.splitContainer1.TabIndex = 4;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(15, 15);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(48, 16);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "全部";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // gc1
            // 
            this.gc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gc1.Location = new System.Drawing.Point(0, 0);
            this.gc1.MainView = this.gv1;
            this.gc1.MenuManager = this.barManager1;
            this.gc1.Name = "gc1";
            this.gc1.Size = new System.Drawing.Size(708, 620);
            this.gc1.TabIndex = 1;
            this.gc1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv1});
            // 
            // gv1
            // 
            this.gv1.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv1.Appearance.ColumnFilterButton.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv1.Appearance.ColumnFilterButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv1.Appearance.ColumnFilterButton.ForeColor = System.Drawing.Color.Black;
            this.gv1.Appearance.ColumnFilterButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv1.Appearance.ColumnFilterButton.Options.UseBackColor = true;
            this.gv1.Appearance.ColumnFilterButton.Options.UseBorderColor = true;
            this.gv1.Appearance.ColumnFilterButton.Options.UseForeColor = true;
            this.gv1.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gv1.Appearance.ColumnFilterButtonActive.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(190)))), ((int)(((byte)(243)))));
            this.gv1.Appearance.ColumnFilterButtonActive.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gv1.Appearance.ColumnFilterButtonActive.ForeColor = System.Drawing.Color.Black;
            this.gv1.Appearance.ColumnFilterButtonActive.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv1.Appearance.ColumnFilterButtonActive.Options.UseBackColor = true;
            this.gv1.Appearance.ColumnFilterButtonActive.Options.UseBorderColor = true;
            this.gv1.Appearance.ColumnFilterButtonActive.Options.UseForeColor = true;
            this.gv1.Appearance.Empty.BackColor = System.Drawing.Color.White;
            this.gv1.Appearance.Empty.Options.UseBackColor = true;
            this.gv1.Appearance.EvenRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(242)))), ((int)(((byte)(254)))));
            this.gv1.Appearance.EvenRow.ForeColor = System.Drawing.Color.Black;
            this.gv1.Appearance.EvenRow.Options.UseBackColor = true;
            this.gv1.Appearance.EvenRow.Options.UseForeColor = true;
            this.gv1.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv1.Appearance.FilterCloseButton.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv1.Appearance.FilterCloseButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv1.Appearance.FilterCloseButton.ForeColor = System.Drawing.Color.Black;
            this.gv1.Appearance.FilterCloseButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv1.Appearance.FilterCloseButton.Options.UseBackColor = true;
            this.gv1.Appearance.FilterCloseButton.Options.UseBorderColor = true;
            this.gv1.Appearance.FilterCloseButton.Options.UseForeColor = true;
            this.gv1.Appearance.FilterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(109)))), ((int)(((byte)(185)))));
            this.gv1.Appearance.FilterPanel.ForeColor = System.Drawing.Color.White;
            this.gv1.Appearance.FilterPanel.Options.UseBackColor = true;
            this.gv1.Appearance.FilterPanel.Options.UseForeColor = true;
            this.gv1.Appearance.FixedLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(97)))), ((int)(((byte)(156)))));
            this.gv1.Appearance.FixedLine.Options.UseBackColor = true;
            this.gv1.Appearance.FocusedCell.BackColor = System.Drawing.Color.White;
            this.gv1.Appearance.FocusedCell.ForeColor = System.Drawing.Color.Black;
            this.gv1.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gv1.Appearance.FocusedCell.Options.UseForeColor = true;
            this.gv1.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(106)))), ((int)(((byte)(197)))));
            this.gv1.Appearance.FocusedRow.Font = new System.Drawing.Font("Tahoma", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gv1.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White;
            this.gv1.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gv1.Appearance.FocusedRow.Options.UseFont = true;
            this.gv1.Appearance.FocusedRow.Options.UseForeColor = true;
            this.gv1.Appearance.FooterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv1.Appearance.FooterPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv1.Appearance.FooterPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv1.Appearance.FooterPanel.ForeColor = System.Drawing.Color.Black;
            this.gv1.Appearance.FooterPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv1.Appearance.FooterPanel.Options.UseBackColor = true;
            this.gv1.Appearance.FooterPanel.Options.UseBorderColor = true;
            this.gv1.Appearance.FooterPanel.Options.UseForeColor = true;
            this.gv1.Appearance.GroupButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv1.Appearance.GroupButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv1.Appearance.GroupButton.ForeColor = System.Drawing.Color.Black;
            this.gv1.Appearance.GroupButton.Options.UseBackColor = true;
            this.gv1.Appearance.GroupButton.Options.UseBorderColor = true;
            this.gv1.Appearance.GroupButton.Options.UseForeColor = true;
            this.gv1.Appearance.GroupFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv1.Appearance.GroupFooter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv1.Appearance.GroupFooter.ForeColor = System.Drawing.Color.Black;
            this.gv1.Appearance.GroupFooter.Options.UseBackColor = true;
            this.gv1.Appearance.GroupFooter.Options.UseBorderColor = true;
            this.gv1.Appearance.GroupFooter.Options.UseForeColor = true;
            this.gv1.Appearance.GroupPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(109)))), ((int)(((byte)(185)))));
            this.gv1.Appearance.GroupPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv1.Appearance.GroupPanel.Options.UseBackColor = true;
            this.gv1.Appearance.GroupPanel.Options.UseForeColor = true;
            this.gv1.Appearance.GroupRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv1.Appearance.GroupRow.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv1.Appearance.GroupRow.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.gv1.Appearance.GroupRow.ForeColor = System.Drawing.Color.Black;
            this.gv1.Appearance.GroupRow.Options.UseBackColor = true;
            this.gv1.Appearance.GroupRow.Options.UseBorderColor = true;
            this.gv1.Appearance.GroupRow.Options.UseFont = true;
            this.gv1.Appearance.GroupRow.Options.UseForeColor = true;
            this.gv1.Appearance.HeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv1.Appearance.HeaderPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv1.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv1.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.gv1.Appearance.HeaderPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv1.Appearance.HeaderPanel.Options.UseBackColor = true;
            this.gv1.Appearance.HeaderPanel.Options.UseBorderColor = true;
            this.gv1.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gv1.Appearance.HideSelectionRow.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.gv1.Appearance.HideSelectionRow.ForeColor = System.Drawing.Color.Black;
            this.gv1.Appearance.HideSelectionRow.Options.UseBackColor = true;
            this.gv1.Appearance.HideSelectionRow.Options.UseForeColor = true;
            this.gv1.Appearance.HorzLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.gv1.Appearance.HorzLine.Options.UseBackColor = true;
            this.gv1.Appearance.OddRow.BackColor = System.Drawing.Color.White;
            this.gv1.Appearance.OddRow.ForeColor = System.Drawing.Color.Black;
            this.gv1.Appearance.OddRow.Options.UseBackColor = true;
            this.gv1.Appearance.OddRow.Options.UseForeColor = true;
            this.gv1.Appearance.Preview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.gv1.Appearance.Preview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(129)))), ((int)(((byte)(185)))));
            this.gv1.Appearance.Preview.Options.UseBackColor = true;
            this.gv1.Appearance.Preview.Options.UseForeColor = true;
            this.gv1.Appearance.Row.BackColor = System.Drawing.Color.White;
            this.gv1.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gv1.Appearance.Row.Options.UseBackColor = true;
            this.gv1.Appearance.Row.Options.UseForeColor = true;
            this.gv1.Appearance.RowSeparator.BackColor = System.Drawing.Color.White;
            this.gv1.Appearance.RowSeparator.Options.UseBackColor = true;
            this.gv1.Appearance.SelectedRow.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.gv1.Appearance.SelectedRow.ForeColor = System.Drawing.Color.Black;
            this.gv1.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gv1.Appearance.SelectedRow.Options.UseForeColor = true;
            this.gv1.Appearance.VertLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.gv1.Appearance.VertLine.Options.UseBackColor = true;
            this.gv1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn2,
            this.gridColumn6,
            this.gridColumn1,
            this.gridColumn5,
            this.gridColumn34,
            this.gridColumn4});
            this.gv1.GridControl = this.gc1;
            this.gv1.IndicatorWidth = 35;
            this.gv1.Name = "gv1";
            this.gv1.OptionsBehavior.Editable = false;
            this.gv1.OptionsBehavior.ReadOnly = true;
            this.gv1.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
            this.gv1.OptionsFind.AlwaysVisible = true;
            this.gv1.OptionsFind.FindMode = DevExpress.XtraEditors.FindMode.FindClick;
            this.gv1.OptionsPrint.AutoWidth = false;
            this.gv1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gv1.OptionsSelection.EnableAppearanceHideSelection = false;
            this.gv1.OptionsSelection.MultiSelect = true;
            this.gv1.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            this.gv1.OptionsView.EnableAppearanceEvenRow = true;
            this.gv1.OptionsView.EnableAppearanceOddRow = true;
            this.gv1.OptionsView.ShowGroupPanel = false;
            this.gv1.OptionsView.ShowViewCaption = true;
            this.gv1.ViewCaption = "待审核列表";
            this.gv1.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gv1_RowCellClick);
            this.gv1.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv1_CustomDrawRowIndicator);
            this.gv1.ColumnWidthChanged += new DevExpress.XtraGrid.Views.Base.ColumnEventHandler(this.gv1_ColumnWidthChanged);
            this.gv1.ColumnPositionChanged += new System.EventHandler(this.gv1_ColumnPositionChanged);
            this.gv1.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv1_FocusedRowChanged);
            this.gv1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.gv1_KeyUp);
            // 
            // gridColumn2
            // 
            this.gridColumn2.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn2.Caption = "审核申请单号";
            this.gridColumn2.FieldName = "审核申请单号";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 0;
            this.gridColumn2.Width = 165;
            // 
            // gridColumn6
            // 
            this.gridColumn6.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn6.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn6.Caption = "单据类型";
            this.gridColumn6.FieldName = "单据类型";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.OptionsColumn.AllowEdit = false;
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 1;
            this.gridColumn6.Width = 84;
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn1.Caption = "关联单号";
            this.gridColumn1.FieldName = "关联单号";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 2;
            this.gridColumn1.Width = 142;
            // 
            // gridColumn5
            // 
            this.gridColumn5.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn5.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn5.Caption = "相关单位";
            this.gridColumn5.FieldName = "相关单位";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 3;
            this.gridColumn5.Width = 123;
            // 
            // gridColumn34
            // 
            this.gridColumn34.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn34.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn34.Caption = "申请人";
            this.gridColumn34.FieldName = "申请人";
            this.gridColumn34.Name = "gridColumn34";
            this.gridColumn34.Visible = true;
            this.gridColumn34.VisibleIndex = 4;
            this.gridColumn34.Width = 109;
            // 
            // gridColumn4
            // 
            this.gridColumn4.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn4.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn4.Caption = "待审核人";
            this.gridColumn4.FieldName = "待审核人";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowEdit = false;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 5;
            this.gridColumn4.Width = 99;
            // 
            // gcP
            // 
            this.gcP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcP.Location = new System.Drawing.Point(0, 0);
            this.gcP.MainView = this.gvP;
            this.gcP.MenuManager = this.barManager1;
            this.gcP.Name = "gcP";
            this.gcP.Size = new System.Drawing.Size(757, 620);
            this.gcP.TabIndex = 1;
            this.gcP.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvP});
            // 
            // gvP
            // 
            this.gvP.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvP.Appearance.ColumnFilterButton.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gvP.Appearance.ColumnFilterButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvP.Appearance.ColumnFilterButton.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.ColumnFilterButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvP.Appearance.ColumnFilterButton.Options.UseBackColor = true;
            this.gvP.Appearance.ColumnFilterButton.Options.UseBorderColor = true;
            this.gvP.Appearance.ColumnFilterButton.Options.UseForeColor = true;
            this.gvP.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gvP.Appearance.ColumnFilterButtonActive.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(190)))), ((int)(((byte)(243)))));
            this.gvP.Appearance.ColumnFilterButtonActive.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gvP.Appearance.ColumnFilterButtonActive.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.ColumnFilterButtonActive.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvP.Appearance.ColumnFilterButtonActive.Options.UseBackColor = true;
            this.gvP.Appearance.ColumnFilterButtonActive.Options.UseBorderColor = true;
            this.gvP.Appearance.ColumnFilterButtonActive.Options.UseForeColor = true;
            this.gvP.Appearance.Empty.BackColor = System.Drawing.Color.White;
            this.gvP.Appearance.Empty.Options.UseBackColor = true;
            this.gvP.Appearance.EvenRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(242)))), ((int)(((byte)(254)))));
            this.gvP.Appearance.EvenRow.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.EvenRow.Options.UseBackColor = true;
            this.gvP.Appearance.EvenRow.Options.UseForeColor = true;
            this.gvP.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvP.Appearance.FilterCloseButton.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gvP.Appearance.FilterCloseButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvP.Appearance.FilterCloseButton.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.FilterCloseButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvP.Appearance.FilterCloseButton.Options.UseBackColor = true;
            this.gvP.Appearance.FilterCloseButton.Options.UseBorderColor = true;
            this.gvP.Appearance.FilterCloseButton.Options.UseForeColor = true;
            this.gvP.Appearance.FilterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(109)))), ((int)(((byte)(185)))));
            this.gvP.Appearance.FilterPanel.ForeColor = System.Drawing.Color.White;
            this.gvP.Appearance.FilterPanel.Options.UseBackColor = true;
            this.gvP.Appearance.FilterPanel.Options.UseForeColor = true;
            this.gvP.Appearance.FixedLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(97)))), ((int)(((byte)(156)))));
            this.gvP.Appearance.FixedLine.Options.UseBackColor = true;
            this.gvP.Appearance.FocusedCell.BackColor = System.Drawing.Color.White;
            this.gvP.Appearance.FocusedCell.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gvP.Appearance.FocusedCell.Options.UseForeColor = true;
            this.gvP.Appearance.FocusedRow.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.gvP.Appearance.FocusedRow.Font = new System.Drawing.Font("Tahoma", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvP.Appearance.FocusedRow.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gvP.Appearance.FocusedRow.Options.UseFont = true;
            this.gvP.Appearance.FocusedRow.Options.UseForeColor = true;
            this.gvP.Appearance.FooterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvP.Appearance.FooterPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gvP.Appearance.FooterPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvP.Appearance.FooterPanel.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.FooterPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvP.Appearance.FooterPanel.Options.UseBackColor = true;
            this.gvP.Appearance.FooterPanel.Options.UseBorderColor = true;
            this.gvP.Appearance.FooterPanel.Options.UseForeColor = true;
            this.gvP.Appearance.GroupButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvP.Appearance.GroupButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvP.Appearance.GroupButton.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.GroupButton.Options.UseBackColor = true;
            this.gvP.Appearance.GroupButton.Options.UseBorderColor = true;
            this.gvP.Appearance.GroupButton.Options.UseForeColor = true;
            this.gvP.Appearance.GroupFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvP.Appearance.GroupFooter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvP.Appearance.GroupFooter.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.GroupFooter.Options.UseBackColor = true;
            this.gvP.Appearance.GroupFooter.Options.UseBorderColor = true;
            this.gvP.Appearance.GroupFooter.Options.UseForeColor = true;
            this.gvP.Appearance.GroupPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(109)))), ((int)(((byte)(185)))));
            this.gvP.Appearance.GroupPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvP.Appearance.GroupPanel.Options.UseBackColor = true;
            this.gvP.Appearance.GroupPanel.Options.UseForeColor = true;
            this.gvP.Appearance.GroupRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvP.Appearance.GroupRow.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvP.Appearance.GroupRow.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.gvP.Appearance.GroupRow.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.GroupRow.Options.UseBackColor = true;
            this.gvP.Appearance.GroupRow.Options.UseBorderColor = true;
            this.gvP.Appearance.GroupRow.Options.UseFont = true;
            this.gvP.Appearance.GroupRow.Options.UseForeColor = true;
            this.gvP.Appearance.HeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvP.Appearance.HeaderPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gvP.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvP.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.HeaderPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvP.Appearance.HeaderPanel.Options.UseBackColor = true;
            this.gvP.Appearance.HeaderPanel.Options.UseBorderColor = true;
            this.gvP.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvP.Appearance.HideSelectionRow.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.gvP.Appearance.HideSelectionRow.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.HideSelectionRow.Options.UseBackColor = true;
            this.gvP.Appearance.HideSelectionRow.Options.UseForeColor = true;
            this.gvP.Appearance.HorzLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.gvP.Appearance.HorzLine.Options.UseBackColor = true;
            this.gvP.Appearance.OddRow.BackColor = System.Drawing.Color.White;
            this.gvP.Appearance.OddRow.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.OddRow.Options.UseBackColor = true;
            this.gvP.Appearance.OddRow.Options.UseForeColor = true;
            this.gvP.Appearance.Preview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.gvP.Appearance.Preview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(129)))), ((int)(((byte)(185)))));
            this.gvP.Appearance.Preview.Options.UseBackColor = true;
            this.gvP.Appearance.Preview.Options.UseForeColor = true;
            this.gvP.Appearance.Row.BackColor = System.Drawing.Color.White;
            this.gvP.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.Row.Options.UseBackColor = true;
            this.gvP.Appearance.Row.Options.UseForeColor = true;
            this.gvP.Appearance.RowSeparator.BackColor = System.Drawing.Color.White;
            this.gvP.Appearance.RowSeparator.Options.UseBackColor = true;
            this.gvP.Appearance.SelectedRow.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.gvP.Appearance.SelectedRow.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gvP.Appearance.SelectedRow.Options.UseForeColor = true;
            this.gvP.Appearance.VertLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.gvP.Appearance.VertLine.Options.UseBackColor = true;
            this.gvP.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn15,
            this.gridColumn3,
            this.gridColumn17,
            this.gridColumn20,
            this.gridColumn24,
            this.gridColumn7,
            this.gridColumn8});
            this.gvP.GridControl = this.gcP;
            this.gvP.IndicatorWidth = 38;
            this.gvP.Name = "gvP";
            this.gvP.OptionsBehavior.Editable = false;
            this.gvP.OptionsFind.AlwaysVisible = true;
            this.gvP.OptionsFind.FindMode = DevExpress.XtraEditors.FindMode.FindClick;
            this.gvP.OptionsPrint.AutoWidth = false;
            this.gvP.OptionsSelection.MultiSelect = true;
            this.gvP.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            this.gvP.OptionsView.EnableAppearanceEvenRow = true;
            this.gvP.OptionsView.EnableAppearanceOddRow = true;
            this.gvP.OptionsView.ShowGroupPanel = false;
            this.gvP.OptionsView.ShowViewCaption = true;
            this.gvP.ViewCaption = "明细";
            this.gvP.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvP_CustomDrawRowIndicator);
            this.gvP.KeyUp += new System.Windows.Forms.KeyEventHandler(this.gvP_KeyUp);
            // 
            // gridColumn15
            // 
            this.gridColumn15.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn15.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn15.Caption = "物料名称";
            this.gridColumn15.FieldName = "物料名称";
            this.gridColumn15.Name = "gridColumn15";
            this.gridColumn15.Visible = true;
            this.gridColumn15.VisibleIndex = 3;
            this.gridColumn15.Width = 147;
            // 
            // gridColumn3
            // 
            this.gridColumn3.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn3.Caption = "规格型号";
            this.gridColumn3.FieldName = "规格型号";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 1;
            this.gridColumn3.Width = 144;
            // 
            // gridColumn17
            // 
            this.gridColumn17.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn17.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn17.Caption = "图纸编号";
            this.gridColumn17.FieldName = "图纸编号";
            this.gridColumn17.Name = "gridColumn17";
            this.gridColumn17.Visible = true;
            this.gridColumn17.VisibleIndex = 4;
            this.gridColumn17.Width = 96;
            // 
            // gridColumn20
            // 
            this.gridColumn20.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn20.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn20.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn20.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn20.Caption = "数量";
            this.gridColumn20.DisplayFormat.FormatString = "#0.##";
            this.gridColumn20.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn20.FieldName = "数量";
            this.gridColumn20.Name = "gridColumn20";
            this.gridColumn20.Visible = true;
            this.gridColumn20.VisibleIndex = 2;
            this.gridColumn20.Width = 95;
            // 
            // gridColumn24
            // 
            this.gridColumn24.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn24.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn24.Caption = "物料编码";
            this.gridColumn24.FieldName = "物料编码";
            this.gridColumn24.Name = "gridColumn24";
            this.gridColumn24.Visible = true;
            this.gridColumn24.VisibleIndex = 0;
            this.gridColumn24.Width = 155;
            // 
            // gridColumn7
            // 
            this.gridColumn7.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn7.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn7.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn7.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn7.Caption = "含税单价";
            this.gridColumn7.DisplayFormat.FormatString = "#0.######";
            this.gridColumn7.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn7.FieldName = "含税单价";
            this.gridColumn7.Name = "gridColumn7";
            // 
            // gridColumn8
            // 
            this.gridColumn8.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn8.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn8.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn8.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn8.Caption = "含税金额";
            this.gridColumn8.DisplayFormat.FormatString = "#0.####";
            this.gridColumn8.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn8.FieldName = "含税金额";
            this.gridColumn8.Name = "gridColumn8";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.跳转详细信息ToolStripMenuItem,
            this.发货信息完善ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(149, 48);
            // 
            // 跳转详细信息ToolStripMenuItem
            // 
            this.跳转详细信息ToolStripMenuItem.Name = "跳转详细信息ToolStripMenuItem";
            this.跳转详细信息ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.跳转详细信息ToolStripMenuItem.Text = "跳转详细信息";
            this.跳转详细信息ToolStripMenuItem.Click += new System.EventHandler(this.跳转详细信息ToolStripMenuItem_Click);
            // 
            // 发货信息完善ToolStripMenuItem
            // 
            this.发货信息完善ToolStripMenuItem.Name = "发货信息完善ToolStripMenuItem";
            this.发货信息完善ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.发货信息完善ToolStripMenuItem.Text = "发货信息完善";
            this.发货信息完善ToolStripMenuItem.Visible = false;
            this.发货信息完善ToolStripMenuItem.Click += new System.EventHandler(this.发货信息完善ToolStripMenuItem_Click);
            // 
            // ui单据审核界面
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "ui单据审核界面";
            this.Size = new System.Drawing.Size(1473, 680);
            this.Load += new System.EventHandler(this.ui单据审核界面_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gc1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvP)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem1;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem2;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private DevExpress.XtraGrid.GridControl gc1;
        private DevExpress.XtraGrid.Views.Grid.GridView gv1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn34;
        private DevExpress.XtraGrid.GridControl gcP;
        private DevExpress.XtraGrid.Views.Grid.GridView gvP;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn15;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn17;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn20;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn24;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private System.Windows.Forms.CheckBox checkBox1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 跳转详细信息ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 发货信息完善ToolStripMenuItem;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem4;
    }
}
