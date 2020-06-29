namespace BaseData
{
    partial class frm功能权限人员权限
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
            this.barLargeButtonItem3 = new DevExpress.XtraBars.BarLargeButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel2 = new System.Windows.Forms.Panel();
            this.gc_代办 = new DevExpress.XtraGrid.GridControl();
            this.gv_代办 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.gc_权限 = new DevExpress.XtraGrid.GridControl();
            this.gv_权限 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.panel3 = new System.Windows.Forms.Panel();
            this.gc_权限组 = new DevExpress.XtraGrid.GridControl();
            this.gv_权限组 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gc_代办)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_代办)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gc_权限)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_权限)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gc_权限组)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_权限组)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
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
            this.barLargeButtonItem3});
            this.barManager1.MainMenu = this.bar2;
            this.barManager1.MaxItemId = 3;
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
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barLargeButtonItem3, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            // 
            // barLargeButtonItem1
            // 
            this.barLargeButtonItem1.Caption = "刷新";
            this.barLargeButtonItem1.Glyph = global::BaseData.Properties.Resources.GenerateData_32x32;
            this.barLargeButtonItem1.Id = 0;
            this.barLargeButtonItem1.Name = "barLargeButtonItem1";
            this.barLargeButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem1_ItemClick);
            // 
            // barLargeButtonItem2
            // 
            this.barLargeButtonItem2.Caption = "保存";
            this.barLargeButtonItem2.Glyph = global::BaseData.Properties.Resources.Save_32x32;
            this.barLargeButtonItem2.Id = 1;
            this.barLargeButtonItem2.Name = "barLargeButtonItem2";
            this.barLargeButtonItem2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem2_ItemClick);
            // 
            // barLargeButtonItem3
            // 
            this.barLargeButtonItem3.Caption = "关闭界面";
            this.barLargeButtonItem3.Glyph = global::BaseData.Properties.Resources.Close_32x32;
            this.barLargeButtonItem3.Id = 2;
            this.barLargeButtonItem3.Name = "barLargeButtonItem3";
            this.barLargeButtonItem3.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem3_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1463, 60);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 771);
            this.barDockControlBottom.Size = new System.Drawing.Size(1463, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 60);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 711);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1463, 60);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 711);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 60);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel2);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel4);
            this.splitContainer1.Panel2.Controls.Add(this.panel3);
            this.splitContainer1.Size = new System.Drawing.Size(1463, 711);
            this.splitContainer1.SplitterDistance = 421;
            this.splitContainer1.SplitterWidth = 8;
            this.splitContainer1.TabIndex = 4;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.gc_代办);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 30);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(421, 681);
            this.panel2.TabIndex = 1;
            // 
            // gc_代办
            // 
            this.gc_代办.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gc_代办.Location = new System.Drawing.Point(0, 0);
            this.gc_代办.MainView = this.gv_代办;
            this.gc_代办.MenuManager = this.barManager1;
            this.gc_代办.Name = "gc_代办";
            this.gc_代办.Size = new System.Drawing.Size(421, 681);
            this.gc_代办.TabIndex = 0;
            this.gc_代办.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_代办});
            // 
            // gv_代办
            // 
            this.gv_代办.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_代办.Appearance.ColumnFilterButton.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv_代办.Appearance.ColumnFilterButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_代办.Appearance.ColumnFilterButton.ForeColor = System.Drawing.Color.Black;
            this.gv_代办.Appearance.ColumnFilterButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv_代办.Appearance.ColumnFilterButton.Options.UseBackColor = true;
            this.gv_代办.Appearance.ColumnFilterButton.Options.UseBorderColor = true;
            this.gv_代办.Appearance.ColumnFilterButton.Options.UseForeColor = true;
            this.gv_代办.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gv_代办.Appearance.ColumnFilterButtonActive.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(190)))), ((int)(((byte)(243)))));
            this.gv_代办.Appearance.ColumnFilterButtonActive.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gv_代办.Appearance.ColumnFilterButtonActive.ForeColor = System.Drawing.Color.Black;
            this.gv_代办.Appearance.ColumnFilterButtonActive.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv_代办.Appearance.ColumnFilterButtonActive.Options.UseBackColor = true;
            this.gv_代办.Appearance.ColumnFilterButtonActive.Options.UseBorderColor = true;
            this.gv_代办.Appearance.ColumnFilterButtonActive.Options.UseForeColor = true;
            this.gv_代办.Appearance.Empty.BackColor = System.Drawing.Color.White;
            this.gv_代办.Appearance.Empty.Options.UseBackColor = true;
            this.gv_代办.Appearance.EvenRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(242)))), ((int)(((byte)(254)))));
            this.gv_代办.Appearance.EvenRow.ForeColor = System.Drawing.Color.Black;
            this.gv_代办.Appearance.EvenRow.Options.UseBackColor = true;
            this.gv_代办.Appearance.EvenRow.Options.UseForeColor = true;
            this.gv_代办.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_代办.Appearance.FilterCloseButton.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv_代办.Appearance.FilterCloseButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_代办.Appearance.FilterCloseButton.ForeColor = System.Drawing.Color.Black;
            this.gv_代办.Appearance.FilterCloseButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv_代办.Appearance.FilterCloseButton.Options.UseBackColor = true;
            this.gv_代办.Appearance.FilterCloseButton.Options.UseBorderColor = true;
            this.gv_代办.Appearance.FilterCloseButton.Options.UseForeColor = true;
            this.gv_代办.Appearance.FilterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(109)))), ((int)(((byte)(185)))));
            this.gv_代办.Appearance.FilterPanel.ForeColor = System.Drawing.Color.White;
            this.gv_代办.Appearance.FilterPanel.Options.UseBackColor = true;
            this.gv_代办.Appearance.FilterPanel.Options.UseForeColor = true;
            this.gv_代办.Appearance.FixedLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(97)))), ((int)(((byte)(156)))));
            this.gv_代办.Appearance.FixedLine.Options.UseBackColor = true;
            this.gv_代办.Appearance.FocusedCell.BackColor = System.Drawing.Color.White;
            this.gv_代办.Appearance.FocusedCell.ForeColor = System.Drawing.Color.Black;
            this.gv_代办.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gv_代办.Appearance.FocusedCell.Options.UseForeColor = true;
            this.gv_代办.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(106)))), ((int)(((byte)(197)))));
            this.gv_代办.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White;
            this.gv_代办.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gv_代办.Appearance.FocusedRow.Options.UseForeColor = true;
            this.gv_代办.Appearance.FooterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_代办.Appearance.FooterPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv_代办.Appearance.FooterPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_代办.Appearance.FooterPanel.ForeColor = System.Drawing.Color.Black;
            this.gv_代办.Appearance.FooterPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv_代办.Appearance.FooterPanel.Options.UseBackColor = true;
            this.gv_代办.Appearance.FooterPanel.Options.UseBorderColor = true;
            this.gv_代办.Appearance.FooterPanel.Options.UseForeColor = true;
            this.gv_代办.Appearance.GroupButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_代办.Appearance.GroupButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_代办.Appearance.GroupButton.ForeColor = System.Drawing.Color.Black;
            this.gv_代办.Appearance.GroupButton.Options.UseBackColor = true;
            this.gv_代办.Appearance.GroupButton.Options.UseBorderColor = true;
            this.gv_代办.Appearance.GroupButton.Options.UseForeColor = true;
            this.gv_代办.Appearance.GroupFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_代办.Appearance.GroupFooter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_代办.Appearance.GroupFooter.ForeColor = System.Drawing.Color.Black;
            this.gv_代办.Appearance.GroupFooter.Options.UseBackColor = true;
            this.gv_代办.Appearance.GroupFooter.Options.UseBorderColor = true;
            this.gv_代办.Appearance.GroupFooter.Options.UseForeColor = true;
            this.gv_代办.Appearance.GroupPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(109)))), ((int)(((byte)(185)))));
            this.gv_代办.Appearance.GroupPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_代办.Appearance.GroupPanel.Options.UseBackColor = true;
            this.gv_代办.Appearance.GroupPanel.Options.UseForeColor = true;
            this.gv_代办.Appearance.GroupRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_代办.Appearance.GroupRow.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_代办.Appearance.GroupRow.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.gv_代办.Appearance.GroupRow.ForeColor = System.Drawing.Color.Black;
            this.gv_代办.Appearance.GroupRow.Options.UseBackColor = true;
            this.gv_代办.Appearance.GroupRow.Options.UseBorderColor = true;
            this.gv_代办.Appearance.GroupRow.Options.UseFont = true;
            this.gv_代办.Appearance.GroupRow.Options.UseForeColor = true;
            this.gv_代办.Appearance.HeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_代办.Appearance.HeaderPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv_代办.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_代办.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.gv_代办.Appearance.HeaderPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv_代办.Appearance.HeaderPanel.Options.UseBackColor = true;
            this.gv_代办.Appearance.HeaderPanel.Options.UseBorderColor = true;
            this.gv_代办.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gv_代办.Appearance.HideSelectionRow.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.gv_代办.Appearance.HideSelectionRow.ForeColor = System.Drawing.Color.Black;
            this.gv_代办.Appearance.HideSelectionRow.Options.UseBackColor = true;
            this.gv_代办.Appearance.HideSelectionRow.Options.UseForeColor = true;
            this.gv_代办.Appearance.HorzLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.gv_代办.Appearance.HorzLine.Options.UseBackColor = true;
            this.gv_代办.Appearance.OddRow.BackColor = System.Drawing.Color.White;
            this.gv_代办.Appearance.OddRow.ForeColor = System.Drawing.Color.Black;
            this.gv_代办.Appearance.OddRow.Options.UseBackColor = true;
            this.gv_代办.Appearance.OddRow.Options.UseForeColor = true;
            this.gv_代办.Appearance.Preview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.gv_代办.Appearance.Preview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(129)))), ((int)(((byte)(185)))));
            this.gv_代办.Appearance.Preview.Options.UseBackColor = true;
            this.gv_代办.Appearance.Preview.Options.UseForeColor = true;
            this.gv_代办.Appearance.Row.BackColor = System.Drawing.Color.White;
            this.gv_代办.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gv_代办.Appearance.Row.Options.UseBackColor = true;
            this.gv_代办.Appearance.Row.Options.UseForeColor = true;
            this.gv_代办.Appearance.RowSeparator.BackColor = System.Drawing.Color.White;
            this.gv_代办.Appearance.RowSeparator.Options.UseBackColor = true;
            this.gv_代办.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(126)))), ((int)(((byte)(217)))));
            this.gv_代办.Appearance.SelectedRow.ForeColor = System.Drawing.Color.White;
            this.gv_代办.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gv_代办.Appearance.SelectedRow.Options.UseForeColor = true;
            this.gv_代办.Appearance.VertLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.gv_代办.Appearance.VertLine.Options.UseBackColor = true;
            this.gv_代办.GridControl = this.gc_代办;
            this.gv_代办.Name = "gv_代办";
            this.gv_代办.OptionsBehavior.Editable = false;
            this.gv_代办.OptionsFind.AlwaysVisible = true;
            this.gv_代办.OptionsView.EnableAppearanceEvenRow = true;
            this.gv_代办.OptionsView.EnableAppearanceOddRow = true;
            this.gv_代办.OptionsView.ShowGroupPanel = false;
            this.gv_代办.OptionsView.ShowViewCaption = true;
            this.gv_代办.ViewCaption = "代办";
            this.gv_代办.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gv_代办_RowCellClick);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(421, 30);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "权限对象";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "部门",
            "人员"});
            this.comboBox1.Location = new System.Drawing.Point(68, 6);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 20);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.TextChanged += new System.EventHandler(this.comboBox1_TextChanged);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.gc_权限);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(593, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(441, 711);
            this.panel4.TabIndex = 2;
            // 
            // gc_权限
            // 
            this.gc_权限.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gc_权限.Location = new System.Drawing.Point(0, 0);
            this.gc_权限.MainView = this.gv_权限;
            this.gc_权限.MenuManager = this.barManager1;
            this.gc_权限.Name = "gc_权限";
            this.gc_权限.Size = new System.Drawing.Size(441, 711);
            this.gc_权限.TabIndex = 0;
            this.gc_权限.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_权限});
            // 
            // gv_权限
            // 
            this.gv_权限.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_权限.Appearance.ColumnFilterButton.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv_权限.Appearance.ColumnFilterButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_权限.Appearance.ColumnFilterButton.ForeColor = System.Drawing.Color.Black;
            this.gv_权限.Appearance.ColumnFilterButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv_权限.Appearance.ColumnFilterButton.Options.UseBackColor = true;
            this.gv_权限.Appearance.ColumnFilterButton.Options.UseBorderColor = true;
            this.gv_权限.Appearance.ColumnFilterButton.Options.UseForeColor = true;
            this.gv_权限.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gv_权限.Appearance.ColumnFilterButtonActive.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(190)))), ((int)(((byte)(243)))));
            this.gv_权限.Appearance.ColumnFilterButtonActive.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gv_权限.Appearance.ColumnFilterButtonActive.ForeColor = System.Drawing.Color.Black;
            this.gv_权限.Appearance.ColumnFilterButtonActive.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv_权限.Appearance.ColumnFilterButtonActive.Options.UseBackColor = true;
            this.gv_权限.Appearance.ColumnFilterButtonActive.Options.UseBorderColor = true;
            this.gv_权限.Appearance.ColumnFilterButtonActive.Options.UseForeColor = true;
            this.gv_权限.Appearance.Empty.BackColor = System.Drawing.Color.White;
            this.gv_权限.Appearance.Empty.Options.UseBackColor = true;
            this.gv_权限.Appearance.EvenRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(242)))), ((int)(((byte)(254)))));
            this.gv_权限.Appearance.EvenRow.ForeColor = System.Drawing.Color.Black;
            this.gv_权限.Appearance.EvenRow.Options.UseBackColor = true;
            this.gv_权限.Appearance.EvenRow.Options.UseForeColor = true;
            this.gv_权限.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_权限.Appearance.FilterCloseButton.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv_权限.Appearance.FilterCloseButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_权限.Appearance.FilterCloseButton.ForeColor = System.Drawing.Color.Black;
            this.gv_权限.Appearance.FilterCloseButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv_权限.Appearance.FilterCloseButton.Options.UseBackColor = true;
            this.gv_权限.Appearance.FilterCloseButton.Options.UseBorderColor = true;
            this.gv_权限.Appearance.FilterCloseButton.Options.UseForeColor = true;
            this.gv_权限.Appearance.FilterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(109)))), ((int)(((byte)(185)))));
            this.gv_权限.Appearance.FilterPanel.ForeColor = System.Drawing.Color.White;
            this.gv_权限.Appearance.FilterPanel.Options.UseBackColor = true;
            this.gv_权限.Appearance.FilterPanel.Options.UseForeColor = true;
            this.gv_权限.Appearance.FixedLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(97)))), ((int)(((byte)(156)))));
            this.gv_权限.Appearance.FixedLine.Options.UseBackColor = true;
            this.gv_权限.Appearance.FocusedCell.BackColor = System.Drawing.Color.White;
            this.gv_权限.Appearance.FocusedCell.ForeColor = System.Drawing.Color.Black;
            this.gv_权限.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gv_权限.Appearance.FocusedCell.Options.UseForeColor = true;
            this.gv_权限.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(106)))), ((int)(((byte)(197)))));
            this.gv_权限.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White;
            this.gv_权限.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gv_权限.Appearance.FocusedRow.Options.UseForeColor = true;
            this.gv_权限.Appearance.FooterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_权限.Appearance.FooterPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv_权限.Appearance.FooterPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_权限.Appearance.FooterPanel.ForeColor = System.Drawing.Color.Black;
            this.gv_权限.Appearance.FooterPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv_权限.Appearance.FooterPanel.Options.UseBackColor = true;
            this.gv_权限.Appearance.FooterPanel.Options.UseBorderColor = true;
            this.gv_权限.Appearance.FooterPanel.Options.UseForeColor = true;
            this.gv_权限.Appearance.GroupButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_权限.Appearance.GroupButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_权限.Appearance.GroupButton.ForeColor = System.Drawing.Color.Black;
            this.gv_权限.Appearance.GroupButton.Options.UseBackColor = true;
            this.gv_权限.Appearance.GroupButton.Options.UseBorderColor = true;
            this.gv_权限.Appearance.GroupButton.Options.UseForeColor = true;
            this.gv_权限.Appearance.GroupFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_权限.Appearance.GroupFooter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_权限.Appearance.GroupFooter.ForeColor = System.Drawing.Color.Black;
            this.gv_权限.Appearance.GroupFooter.Options.UseBackColor = true;
            this.gv_权限.Appearance.GroupFooter.Options.UseBorderColor = true;
            this.gv_权限.Appearance.GroupFooter.Options.UseForeColor = true;
            this.gv_权限.Appearance.GroupPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(109)))), ((int)(((byte)(185)))));
            this.gv_权限.Appearance.GroupPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_权限.Appearance.GroupPanel.Options.UseBackColor = true;
            this.gv_权限.Appearance.GroupPanel.Options.UseForeColor = true;
            this.gv_权限.Appearance.GroupRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_权限.Appearance.GroupRow.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_权限.Appearance.GroupRow.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.gv_权限.Appearance.GroupRow.ForeColor = System.Drawing.Color.Black;
            this.gv_权限.Appearance.GroupRow.Options.UseBackColor = true;
            this.gv_权限.Appearance.GroupRow.Options.UseBorderColor = true;
            this.gv_权限.Appearance.GroupRow.Options.UseFont = true;
            this.gv_权限.Appearance.GroupRow.Options.UseForeColor = true;
            this.gv_权限.Appearance.HeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_权限.Appearance.HeaderPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv_权限.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_权限.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.gv_权限.Appearance.HeaderPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv_权限.Appearance.HeaderPanel.Options.UseBackColor = true;
            this.gv_权限.Appearance.HeaderPanel.Options.UseBorderColor = true;
            this.gv_权限.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gv_权限.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(106)))), ((int)(((byte)(153)))), ((int)(((byte)(228)))));
            this.gv_权限.Appearance.HideSelectionRow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(224)))), ((int)(((byte)(251)))));
            this.gv_权限.Appearance.HideSelectionRow.Options.UseBackColor = true;
            this.gv_权限.Appearance.HideSelectionRow.Options.UseForeColor = true;
            this.gv_权限.Appearance.HorzLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.gv_权限.Appearance.HorzLine.Options.UseBackColor = true;
            this.gv_权限.Appearance.OddRow.BackColor = System.Drawing.Color.White;
            this.gv_权限.Appearance.OddRow.ForeColor = System.Drawing.Color.Black;
            this.gv_权限.Appearance.OddRow.Options.UseBackColor = true;
            this.gv_权限.Appearance.OddRow.Options.UseForeColor = true;
            this.gv_权限.Appearance.Preview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.gv_权限.Appearance.Preview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(129)))), ((int)(((byte)(185)))));
            this.gv_权限.Appearance.Preview.Options.UseBackColor = true;
            this.gv_权限.Appearance.Preview.Options.UseForeColor = true;
            this.gv_权限.Appearance.Row.BackColor = System.Drawing.Color.White;
            this.gv_权限.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gv_权限.Appearance.Row.Options.UseBackColor = true;
            this.gv_权限.Appearance.Row.Options.UseForeColor = true;
            this.gv_权限.Appearance.RowSeparator.BackColor = System.Drawing.Color.White;
            this.gv_权限.Appearance.RowSeparator.Options.UseBackColor = true;
            this.gv_权限.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(126)))), ((int)(((byte)(217)))));
            this.gv_权限.Appearance.SelectedRow.ForeColor = System.Drawing.Color.White;
            this.gv_权限.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gv_权限.Appearance.SelectedRow.Options.UseForeColor = true;
            this.gv_权限.Appearance.VertLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.gv_权限.Appearance.VertLine.Options.UseBackColor = true;
            this.gv_权限.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn2});
            this.gv_权限.GridControl = this.gc_权限;
            this.gv_权限.Name = "gv_权限";
            this.gv_权限.OptionsFind.AlwaysVisible = true;
            this.gv_权限.OptionsView.EnableAppearanceEvenRow = true;
            this.gv_权限.OptionsView.EnableAppearanceOddRow = true;
            this.gv_权限.OptionsView.ShowGroupPanel = false;
            this.gv_权限.OptionsView.ShowViewCaption = true;
            this.gv_权限.ViewCaption = "权限类型";
            // 
            // gridColumn2
            // 
            this.gridColumn2.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn2.Caption = "权限类型";
            this.gridColumn2.FieldName = "权限类型";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 0;
            this.gridColumn2.Width = 646;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.gc_权限组);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(593, 711);
            this.panel3.TabIndex = 1;
            // 
            // gc_权限组
            // 
            this.gc_权限组.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gc_权限组.Location = new System.Drawing.Point(0, 0);
            this.gc_权限组.MainView = this.gv_权限组;
            this.gc_权限组.MenuManager = this.barManager1;
            this.gc_权限组.Name = "gc_权限组";
            this.gc_权限组.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.gc_权限组.Size = new System.Drawing.Size(593, 711);
            this.gc_权限组.TabIndex = 0;
            this.gc_权限组.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_权限组});
            // 
            // gv_权限组
            // 
            this.gv_权限组.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_权限组.Appearance.ColumnFilterButton.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv_权限组.Appearance.ColumnFilterButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_权限组.Appearance.ColumnFilterButton.ForeColor = System.Drawing.Color.Black;
            this.gv_权限组.Appearance.ColumnFilterButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv_权限组.Appearance.ColumnFilterButton.Options.UseBackColor = true;
            this.gv_权限组.Appearance.ColumnFilterButton.Options.UseBorderColor = true;
            this.gv_权限组.Appearance.ColumnFilterButton.Options.UseForeColor = true;
            this.gv_权限组.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gv_权限组.Appearance.ColumnFilterButtonActive.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(190)))), ((int)(((byte)(243)))));
            this.gv_权限组.Appearance.ColumnFilterButtonActive.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gv_权限组.Appearance.ColumnFilterButtonActive.ForeColor = System.Drawing.Color.Black;
            this.gv_权限组.Appearance.ColumnFilterButtonActive.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv_权限组.Appearance.ColumnFilterButtonActive.Options.UseBackColor = true;
            this.gv_权限组.Appearance.ColumnFilterButtonActive.Options.UseBorderColor = true;
            this.gv_权限组.Appearance.ColumnFilterButtonActive.Options.UseForeColor = true;
            this.gv_权限组.Appearance.Empty.BackColor = System.Drawing.Color.White;
            this.gv_权限组.Appearance.Empty.Options.UseBackColor = true;
            this.gv_权限组.Appearance.EvenRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(242)))), ((int)(((byte)(254)))));
            this.gv_权限组.Appearance.EvenRow.ForeColor = System.Drawing.Color.Black;
            this.gv_权限组.Appearance.EvenRow.Options.UseBackColor = true;
            this.gv_权限组.Appearance.EvenRow.Options.UseForeColor = true;
            this.gv_权限组.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_权限组.Appearance.FilterCloseButton.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv_权限组.Appearance.FilterCloseButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_权限组.Appearance.FilterCloseButton.ForeColor = System.Drawing.Color.Black;
            this.gv_权限组.Appearance.FilterCloseButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv_权限组.Appearance.FilterCloseButton.Options.UseBackColor = true;
            this.gv_权限组.Appearance.FilterCloseButton.Options.UseBorderColor = true;
            this.gv_权限组.Appearance.FilterCloseButton.Options.UseForeColor = true;
            this.gv_权限组.Appearance.FilterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(109)))), ((int)(((byte)(185)))));
            this.gv_权限组.Appearance.FilterPanel.ForeColor = System.Drawing.Color.White;
            this.gv_权限组.Appearance.FilterPanel.Options.UseBackColor = true;
            this.gv_权限组.Appearance.FilterPanel.Options.UseForeColor = true;
            this.gv_权限组.Appearance.FixedLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(97)))), ((int)(((byte)(156)))));
            this.gv_权限组.Appearance.FixedLine.Options.UseBackColor = true;
            this.gv_权限组.Appearance.FocusedCell.BackColor = System.Drawing.Color.White;
            this.gv_权限组.Appearance.FocusedCell.ForeColor = System.Drawing.Color.Black;
            this.gv_权限组.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gv_权限组.Appearance.FocusedCell.Options.UseForeColor = true;
            this.gv_权限组.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(106)))), ((int)(((byte)(197)))));
            this.gv_权限组.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White;
            this.gv_权限组.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gv_权限组.Appearance.FocusedRow.Options.UseForeColor = true;
            this.gv_权限组.Appearance.FooterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_权限组.Appearance.FooterPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv_权限组.Appearance.FooterPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_权限组.Appearance.FooterPanel.ForeColor = System.Drawing.Color.Black;
            this.gv_权限组.Appearance.FooterPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv_权限组.Appearance.FooterPanel.Options.UseBackColor = true;
            this.gv_权限组.Appearance.FooterPanel.Options.UseBorderColor = true;
            this.gv_权限组.Appearance.FooterPanel.Options.UseForeColor = true;
            this.gv_权限组.Appearance.GroupButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_权限组.Appearance.GroupButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_权限组.Appearance.GroupButton.ForeColor = System.Drawing.Color.Black;
            this.gv_权限组.Appearance.GroupButton.Options.UseBackColor = true;
            this.gv_权限组.Appearance.GroupButton.Options.UseBorderColor = true;
            this.gv_权限组.Appearance.GroupButton.Options.UseForeColor = true;
            this.gv_权限组.Appearance.GroupFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_权限组.Appearance.GroupFooter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_权限组.Appearance.GroupFooter.ForeColor = System.Drawing.Color.Black;
            this.gv_权限组.Appearance.GroupFooter.Options.UseBackColor = true;
            this.gv_权限组.Appearance.GroupFooter.Options.UseBorderColor = true;
            this.gv_权限组.Appearance.GroupFooter.Options.UseForeColor = true;
            this.gv_权限组.Appearance.GroupPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(109)))), ((int)(((byte)(185)))));
            this.gv_权限组.Appearance.GroupPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_权限组.Appearance.GroupPanel.Options.UseBackColor = true;
            this.gv_权限组.Appearance.GroupPanel.Options.UseForeColor = true;
            this.gv_权限组.Appearance.GroupRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_权限组.Appearance.GroupRow.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_权限组.Appearance.GroupRow.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.gv_权限组.Appearance.GroupRow.ForeColor = System.Drawing.Color.Black;
            this.gv_权限组.Appearance.GroupRow.Options.UseBackColor = true;
            this.gv_权限组.Appearance.GroupRow.Options.UseBorderColor = true;
            this.gv_权限组.Appearance.GroupRow.Options.UseFont = true;
            this.gv_权限组.Appearance.GroupRow.Options.UseForeColor = true;
            this.gv_权限组.Appearance.HeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_权限组.Appearance.HeaderPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv_权限组.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_权限组.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.gv_权限组.Appearance.HeaderPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv_权限组.Appearance.HeaderPanel.Options.UseBackColor = true;
            this.gv_权限组.Appearance.HeaderPanel.Options.UseBorderColor = true;
            this.gv_权限组.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gv_权限组.Appearance.HideSelectionRow.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.gv_权限组.Appearance.HideSelectionRow.ForeColor = System.Drawing.Color.Black;
            this.gv_权限组.Appearance.HideSelectionRow.Options.UseBackColor = true;
            this.gv_权限组.Appearance.HideSelectionRow.Options.UseForeColor = true;
            this.gv_权限组.Appearance.HorzLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.gv_权限组.Appearance.HorzLine.Options.UseBackColor = true;
            this.gv_权限组.Appearance.OddRow.BackColor = System.Drawing.Color.White;
            this.gv_权限组.Appearance.OddRow.ForeColor = System.Drawing.Color.Black;
            this.gv_权限组.Appearance.OddRow.Options.UseBackColor = true;
            this.gv_权限组.Appearance.OddRow.Options.UseForeColor = true;
            this.gv_权限组.Appearance.Preview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.gv_权限组.Appearance.Preview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(129)))), ((int)(((byte)(185)))));
            this.gv_权限组.Appearance.Preview.Options.UseBackColor = true;
            this.gv_权限组.Appearance.Preview.Options.UseForeColor = true;
            this.gv_权限组.Appearance.Row.BackColor = System.Drawing.Color.White;
            this.gv_权限组.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gv_权限组.Appearance.Row.Options.UseBackColor = true;
            this.gv_权限组.Appearance.Row.Options.UseForeColor = true;
            this.gv_权限组.Appearance.RowSeparator.BackColor = System.Drawing.Color.White;
            this.gv_权限组.Appearance.RowSeparator.Options.UseBackColor = true;
            this.gv_权限组.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(126)))), ((int)(((byte)(217)))));
            this.gv_权限组.Appearance.SelectedRow.ForeColor = System.Drawing.Color.White;
            this.gv_权限组.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gv_权限组.Appearance.SelectedRow.Options.UseForeColor = true;
            this.gv_权限组.Appearance.VertLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.gv_权限组.Appearance.VertLine.Options.UseBackColor = true;
            this.gv_权限组.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn5});
            this.gv_权限组.GridControl = this.gc_权限组;
            this.gv_权限组.Name = "gv_权限组";
            this.gv_权限组.OptionsFind.AlwaysVisible = true;
            this.gv_权限组.OptionsView.EnableAppearanceEvenRow = true;
            this.gv_权限组.OptionsView.EnableAppearanceOddRow = true;
            this.gv_权限组.OptionsView.ShowGroupPanel = false;
            this.gv_权限组.OptionsView.ShowViewCaption = true;
            this.gv_权限组.ViewCaption = "权限组";
            this.gv_权限组.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gv_权限组_RowCellClick);
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn1.Caption = "权限组";
            this.gridColumn1.FieldName = "权限组";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 211;
            // 
            // gridColumn5
            // 
            this.gridColumn5.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn5.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn5.Caption = "选择";
            this.gridColumn5.ColumnEdit = this.repositoryItemCheckEdit1;
            this.gridColumn5.FieldName = "选择";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 1;
            this.gridColumn5.Width = 259;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            this.repositoryItemCheckEdit1.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.repositoryItemCheckEdit1.EditValueChanged += new System.EventHandler(this.repositoryItemCheckEdit1_EditValueChanged);
            // 
            // frm功能权限人员权限
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frm功能权限人员权限";
            this.Size = new System.Drawing.Size(1463, 771);
            this.Load += new System.EventHandler(this.frm功能权限人员权限_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gc_代办)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_代办)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gc_权限)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_权限)).EndInit();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gc_权限组)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_权限组)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
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
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel2;
        private DevExpress.XtraGrid.GridControl gc_代办;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_代办;
        private System.Windows.Forms.Panel panel4;
        private DevExpress.XtraGrid.GridControl gc_权限;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_权限;
        private System.Windows.Forms.Panel panel3;
        private DevExpress.XtraGrid.GridControl gc_权限组;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_权限组;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem1;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem2;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}
