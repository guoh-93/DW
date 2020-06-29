namespace ERPproduct
{
    partial class 成品检验不合格评审单上传
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.barLargeButtonItem1 = new DevExpress.XtraBars.BarLargeButtonItem();
            this.barLargeButtonItem2 = new DevExpress.XtraBars.BarLargeButtonItem();
            this.barLargeButtonItem3 = new DevExpress.XtraBars.BarLargeButtonItem();
            this.barLargeButtonItem4 = new DevExpress.XtraBars.BarLargeButtonItem();
            this.barLargeButtonItem6 = new DevExpress.XtraBars.BarLargeButtonItem();
            this.barLargeButtonItem5 = new DevExpress.XtraBars.BarLargeButtonItem();
            this.barLargeButtonItem7 = new DevExpress.XtraBars.BarLargeButtonItem();
            this.bar3 = new DevExpress.XtraBars.Bar();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.gcM1 = new DevExpress.XtraGrid.GridControl();
            this.gvM1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gc1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemSearchLookUpEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcM1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvM1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar2,
            this.bar3});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barLargeButtonItem1,
            this.barLargeButtonItem2,
            this.barLargeButtonItem3,
            this.barLargeButtonItem4,
            this.barLargeButtonItem5,
            this.barLargeButtonItem6,
            this.barLargeButtonItem7});
            this.barManager1.MainMenu = this.bar2;
            this.barManager1.MaxItemId = 8;
            this.barManager1.StatusBar = this.bar3;
            // 
            // bar2
            // 
            this.bar2.BarName = "Main menu";
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barLargeButtonItem1, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barLargeButtonItem2, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barLargeButtonItem3, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barLargeButtonItem4, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barLargeButtonItem6, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barLargeButtonItem5, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barLargeButtonItem7, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            // 
            // barLargeButtonItem1
            // 
            this.barLargeButtonItem1.Caption = "刷新";
            this.barLargeButtonItem1.Glyph = global::ERPproduct.Properties.Resources.GenerateData_32x32;
            this.barLargeButtonItem1.Id = 0;
            this.barLargeButtonItem1.Name = "barLargeButtonItem1";
            this.barLargeButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem1_ItemClick);
            // 
            // barLargeButtonItem2
            // 
            this.barLargeButtonItem2.Caption = "新增";
            this.barLargeButtonItem2.Glyph = global::ERPproduct.Properties.Resources.AddToLibrary_32x32;
            this.barLargeButtonItem2.Id = 1;
            this.barLargeButtonItem2.Name = "barLargeButtonItem2";
            this.barLargeButtonItem2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem2_ItemClick);
            // 
            // barLargeButtonItem3
            // 
            this.barLargeButtonItem3.Caption = "上传";
            this.barLargeButtonItem3.Glyph = global::ERPproduct.Properties.Resources.Arrow_up_32px_1184719_easyicon_net;
            this.barLargeButtonItem3.Id = 2;
            this.barLargeButtonItem3.Name = "barLargeButtonItem3";
            this.barLargeButtonItem3.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem3_ItemClick);
            // 
            // barLargeButtonItem4
            // 
            this.barLargeButtonItem4.Caption = "下载";
            this.barLargeButtonItem4.Glyph = global::ERPproduct.Properties.Resources.Arrow_down_32px_1184716_easyicon_net;
            this.barLargeButtonItem4.Id = 3;
            this.barLargeButtonItem4.Name = "barLargeButtonItem4";
            this.barLargeButtonItem4.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem4_ItemClick);
            // 
            // barLargeButtonItem6
            // 
            this.barLargeButtonItem6.Caption = "预览";
            this.barLargeButtonItem6.Glyph = global::ERPproduct.Properties.Resources.see_47_928251121076px_1195253_easyicon_net;
            this.barLargeButtonItem6.Id = 5;
            this.barLargeButtonItem6.Name = "barLargeButtonItem6";
            this.barLargeButtonItem6.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem6_ItemClick);
            // 
            // barLargeButtonItem5
            // 
            this.barLargeButtonItem5.Caption = "删除";
            this.barLargeButtonItem5.Glyph = global::ERPproduct.Properties.Resources.Remove_32x32;
            this.barLargeButtonItem5.Id = 4;
            this.barLargeButtonItem5.Name = "barLargeButtonItem5";
            this.barLargeButtonItem5.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem5_ItemClick);
            // 
            // barLargeButtonItem7
            // 
            this.barLargeButtonItem7.Caption = "关闭界面";
            this.barLargeButtonItem7.Glyph = global::ERPproduct.Properties.Resources.Close_32x32;
            this.barLargeButtonItem7.Id = 6;
            this.barLargeButtonItem7.Name = "barLargeButtonItem7";
            this.barLargeButtonItem7.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem7_ItemClick);
            // 
            // bar3
            // 
            this.bar3.BarName = "Status bar";
            this.bar3.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.bar3.DockCol = 0;
            this.bar3.DockRow = 0;
            this.bar3.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.bar3.OptionsBar.AllowQuickCustomization = false;
            this.bar3.OptionsBar.DrawDragBorder = false;
            this.bar3.OptionsBar.UseWholeRow = true;
            this.bar3.Text = "Status bar";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1240, 60);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 547);
            this.barDockControlBottom.Size = new System.Drawing.Size(1240, 23);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 60);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 487);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1240, 60);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 487);
            // 
            // gcM1
            // 
            this.gcM1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcM1.Location = new System.Drawing.Point(0, 60);
            this.gcM1.MainView = this.gvM1;
            this.gcM1.MenuManager = this.barManager1;
            this.gcM1.Name = "gcM1";
            this.gcM1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemSearchLookUpEdit1});
            this.gcM1.Size = new System.Drawing.Size(1240, 487);
            this.gcM1.TabIndex = 11;
            this.gcM1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvM1});
            // 
            // gvM1
            // 
            this.gvM1.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvM1.Appearance.ColumnFilterButton.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gvM1.Appearance.ColumnFilterButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvM1.Appearance.ColumnFilterButton.ForeColor = System.Drawing.Color.Black;
            this.gvM1.Appearance.ColumnFilterButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvM1.Appearance.ColumnFilterButton.Options.UseBackColor = true;
            this.gvM1.Appearance.ColumnFilterButton.Options.UseBorderColor = true;
            this.gvM1.Appearance.ColumnFilterButton.Options.UseForeColor = true;
            this.gvM1.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gvM1.Appearance.ColumnFilterButtonActive.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(190)))), ((int)(((byte)(243)))));
            this.gvM1.Appearance.ColumnFilterButtonActive.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gvM1.Appearance.ColumnFilterButtonActive.ForeColor = System.Drawing.Color.Black;
            this.gvM1.Appearance.ColumnFilterButtonActive.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvM1.Appearance.ColumnFilterButtonActive.Options.UseBackColor = true;
            this.gvM1.Appearance.ColumnFilterButtonActive.Options.UseBorderColor = true;
            this.gvM1.Appearance.ColumnFilterButtonActive.Options.UseForeColor = true;
            this.gvM1.Appearance.Empty.BackColor = System.Drawing.Color.White;
            this.gvM1.Appearance.Empty.Options.UseBackColor = true;
            this.gvM1.Appearance.EvenRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(242)))), ((int)(((byte)(254)))));
            this.gvM1.Appearance.EvenRow.ForeColor = System.Drawing.Color.Black;
            this.gvM1.Appearance.EvenRow.Options.UseBackColor = true;
            this.gvM1.Appearance.EvenRow.Options.UseForeColor = true;
            this.gvM1.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvM1.Appearance.FilterCloseButton.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gvM1.Appearance.FilterCloseButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvM1.Appearance.FilterCloseButton.ForeColor = System.Drawing.Color.Black;
            this.gvM1.Appearance.FilterCloseButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvM1.Appearance.FilterCloseButton.Options.UseBackColor = true;
            this.gvM1.Appearance.FilterCloseButton.Options.UseBorderColor = true;
            this.gvM1.Appearance.FilterCloseButton.Options.UseForeColor = true;
            this.gvM1.Appearance.FilterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(109)))), ((int)(((byte)(185)))));
            this.gvM1.Appearance.FilterPanel.ForeColor = System.Drawing.Color.White;
            this.gvM1.Appearance.FilterPanel.Options.UseBackColor = true;
            this.gvM1.Appearance.FilterPanel.Options.UseForeColor = true;
            this.gvM1.Appearance.FixedLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(97)))), ((int)(((byte)(156)))));
            this.gvM1.Appearance.FixedLine.Options.UseBackColor = true;
            this.gvM1.Appearance.FocusedCell.BackColor = System.Drawing.Color.White;
            this.gvM1.Appearance.FocusedCell.ForeColor = System.Drawing.Color.Black;
            this.gvM1.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gvM1.Appearance.FocusedCell.Options.UseForeColor = true;
            this.gvM1.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(106)))), ((int)(((byte)(197)))));
            this.gvM1.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White;
            this.gvM1.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gvM1.Appearance.FocusedRow.Options.UseForeColor = true;
            this.gvM1.Appearance.FooterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvM1.Appearance.FooterPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gvM1.Appearance.FooterPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvM1.Appearance.FooterPanel.ForeColor = System.Drawing.Color.Black;
            this.gvM1.Appearance.FooterPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvM1.Appearance.FooterPanel.Options.UseBackColor = true;
            this.gvM1.Appearance.FooterPanel.Options.UseBorderColor = true;
            this.gvM1.Appearance.FooterPanel.Options.UseForeColor = true;
            this.gvM1.Appearance.GroupButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvM1.Appearance.GroupButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvM1.Appearance.GroupButton.ForeColor = System.Drawing.Color.Black;
            this.gvM1.Appearance.GroupButton.Options.UseBackColor = true;
            this.gvM1.Appearance.GroupButton.Options.UseBorderColor = true;
            this.gvM1.Appearance.GroupButton.Options.UseForeColor = true;
            this.gvM1.Appearance.GroupFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvM1.Appearance.GroupFooter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvM1.Appearance.GroupFooter.ForeColor = System.Drawing.Color.Black;
            this.gvM1.Appearance.GroupFooter.Options.UseBackColor = true;
            this.gvM1.Appearance.GroupFooter.Options.UseBorderColor = true;
            this.gvM1.Appearance.GroupFooter.Options.UseForeColor = true;
            this.gvM1.Appearance.GroupPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(109)))), ((int)(((byte)(185)))));
            this.gvM1.Appearance.GroupPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvM1.Appearance.GroupPanel.Options.UseBackColor = true;
            this.gvM1.Appearance.GroupPanel.Options.UseForeColor = true;
            this.gvM1.Appearance.GroupRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvM1.Appearance.GroupRow.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvM1.Appearance.GroupRow.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.gvM1.Appearance.GroupRow.ForeColor = System.Drawing.Color.Black;
            this.gvM1.Appearance.GroupRow.Options.UseBackColor = true;
            this.gvM1.Appearance.GroupRow.Options.UseBorderColor = true;
            this.gvM1.Appearance.GroupRow.Options.UseFont = true;
            this.gvM1.Appearance.GroupRow.Options.UseForeColor = true;
            this.gvM1.Appearance.HeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvM1.Appearance.HeaderPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gvM1.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvM1.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.gvM1.Appearance.HeaderPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvM1.Appearance.HeaderPanel.Options.UseBackColor = true;
            this.gvM1.Appearance.HeaderPanel.Options.UseBorderColor = true;
            this.gvM1.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvM1.Appearance.HideSelectionRow.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.gvM1.Appearance.HideSelectionRow.ForeColor = System.Drawing.Color.Black;
            this.gvM1.Appearance.HideSelectionRow.Options.UseBackColor = true;
            this.gvM1.Appearance.HideSelectionRow.Options.UseForeColor = true;
            this.gvM1.Appearance.HorzLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.gvM1.Appearance.HorzLine.Options.UseBackColor = true;
            this.gvM1.Appearance.OddRow.BackColor = System.Drawing.Color.White;
            this.gvM1.Appearance.OddRow.ForeColor = System.Drawing.Color.Black;
            this.gvM1.Appearance.OddRow.Options.UseBackColor = true;
            this.gvM1.Appearance.OddRow.Options.UseForeColor = true;
            this.gvM1.Appearance.Preview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.gvM1.Appearance.Preview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(129)))), ((int)(((byte)(185)))));
            this.gvM1.Appearance.Preview.Options.UseBackColor = true;
            this.gvM1.Appearance.Preview.Options.UseForeColor = true;
            this.gvM1.Appearance.Row.BackColor = System.Drawing.Color.White;
            this.gvM1.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gvM1.Appearance.Row.Options.UseBackColor = true;
            this.gvM1.Appearance.Row.Options.UseForeColor = true;
            this.gvM1.Appearance.RowSeparator.BackColor = System.Drawing.Color.White;
            this.gvM1.Appearance.RowSeparator.Options.UseBackColor = true;
            this.gvM1.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(126)))), ((int)(((byte)(217)))));
            this.gvM1.Appearance.SelectedRow.ForeColor = System.Drawing.Color.White;
            this.gvM1.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gvM1.Appearance.SelectedRow.Options.UseForeColor = true;
            this.gvM1.Appearance.VertLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.gvM1.Appearance.VertLine.Options.UseBackColor = true;
            this.gvM1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gc1,
            this.gridColumn2});
            this.gvM1.GridControl = this.gcM1;
            this.gvM1.Name = "gvM1";
            this.gvM1.OptionsCustomization.AllowSort = false;
            this.gvM1.OptionsView.EnableAppearanceEvenRow = true;
            this.gvM1.OptionsView.EnableAppearanceOddRow = true;
            this.gvM1.OptionsView.ShowGroupPanel = false;
            this.gvM1.OptionsView.ShowViewCaption = true;
            this.gvM1.ViewCaption = "不合格评审单";
            // 
            // gc1
            // 
            this.gc1.AppearanceHeader.Options.UseTextOptions = true;
            this.gc1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gc1.Caption = "文件类型";
            this.gc1.FieldName = "文件类型";
            this.gc1.Name = "gc1";
            this.gc1.OptionsColumn.AllowEdit = false;
            this.gc1.Visible = true;
            this.gc1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn2.Caption = "上传文件全名";
            this.gridColumn2.FieldName = "表单名称";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // repositoryItemSearchLookUpEdit1
            // 
            this.repositoryItemSearchLookUpEdit1.AutoHeight = false;
            this.repositoryItemSearchLookUpEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSearchLookUpEdit1.Name = "repositoryItemSearchLookUpEdit1";
            this.repositoryItemSearchLookUpEdit1.NullText = "";
            this.repositoryItemSearchLookUpEdit1.View = this.gridView1;
            // 
            // gridView1
            // 
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // 成品检验不合格评审单上传
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1240, 570);
            this.Controls.Add(this.gcM1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "成品检验不合格评审单上传";
            this.Text = "成品检验不合格评审单上传";
            this.Load += new System.EventHandler(this.成品检验不合格评审单上传_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcM1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvM1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.Bar bar3;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem1;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem2;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem3;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem4;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem5;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem6;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem7;
        private DevExpress.XtraGrid.GridControl gcM1;
        private DevExpress.XtraGrid.Views.Grid.GridView gvM1;
        private DevExpress.XtraGrid.Columns.GridColumn gc1;
        private DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit repositoryItemSearchLookUpEdit1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
    }
}