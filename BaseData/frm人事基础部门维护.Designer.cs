﻿namespace BaseData
{
    partial class frm人事基础部门维护
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
            this.barLargeButtonItem4 = new DevExpress.XtraBars.BarLargeButtonItem();
            this.bar3 = new DevExpress.XtraBars.Bar();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.panel2 = new System.Windows.Forms.Panel();
            this.gcM = new DevExpress.XtraGrid.GridControl();
            this.gvM = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gc1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gc2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gc3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemSearchLookUpEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit();
            this.repositoryItemSearchLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gc4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemSearchLookUpEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit();
            this.repositoryItemSearchLookUpEdit2View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.barLargeButtonItem5 = new DevExpress.XtraBars.BarLargeButtonItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit2View)).BeginInit();
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
            this.barLargeButtonItem5});
            this.barManager1.MainMenu = this.bar2;
            this.barManager1.MaxItemId = 5;
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
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barLargeButtonItem3, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barLargeButtonItem4, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barLargeButtonItem5, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
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
            this.barLargeButtonItem2.Caption = "新增";
            this.barLargeButtonItem2.Glyph = global::BaseData.Properties.Resources.AddToLibrary_32x32;
            this.barLargeButtonItem2.Id = 1;
            this.barLargeButtonItem2.Name = "barLargeButtonItem2";
            this.barLargeButtonItem2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem2_ItemClick);
            // 
            // barLargeButtonItem3
            // 
            this.barLargeButtonItem3.Caption = "删除";
            this.barLargeButtonItem3.Glyph = global::BaseData.Properties.Resources.Remove_32x32;
            this.barLargeButtonItem3.Id = 2;
            this.barLargeButtonItem3.Name = "barLargeButtonItem3";
            this.barLargeButtonItem3.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem3_ItemClick);
            // 
            // barLargeButtonItem4
            // 
            this.barLargeButtonItem4.Caption = "保存";
            this.barLargeButtonItem4.Glyph = global::BaseData.Properties.Resources.Save_32x32;
            this.barLargeButtonItem4.Id = 3;
            this.barLargeButtonItem4.Name = "barLargeButtonItem4";
            this.barLargeButtonItem4.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem4_ItemClick);
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
            this.barDockControlTop.Size = new System.Drawing.Size(926, 60);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 486);
            this.barDockControlBottom.Size = new System.Drawing.Size(926, 23);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 60);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 426);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(926, 60);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 426);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.gcM);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 60);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(926, 426);
            this.panel2.TabIndex = 5;
            // 
            // gcM
            // 
            this.gcM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcM.Location = new System.Drawing.Point(0, 0);
            this.gcM.MainView = this.gvM;
            this.gcM.MenuManager = this.barManager1;
            this.gcM.Name = "gcM";
            this.gcM.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemSearchLookUpEdit1,
            this.repositoryItemSearchLookUpEdit2});
            this.gcM.Size = new System.Drawing.Size(926, 426);
            this.gcM.TabIndex = 0;
            this.gcM.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvM});
            // 
            // gvM
            // 
            this.gvM.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvM.Appearance.ColumnFilterButton.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gvM.Appearance.ColumnFilterButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvM.Appearance.ColumnFilterButton.ForeColor = System.Drawing.Color.Black;
            this.gvM.Appearance.ColumnFilterButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvM.Appearance.ColumnFilterButton.Options.UseBackColor = true;
            this.gvM.Appearance.ColumnFilterButton.Options.UseBorderColor = true;
            this.gvM.Appearance.ColumnFilterButton.Options.UseForeColor = true;
            this.gvM.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gvM.Appearance.ColumnFilterButtonActive.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(190)))), ((int)(((byte)(243)))));
            this.gvM.Appearance.ColumnFilterButtonActive.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gvM.Appearance.ColumnFilterButtonActive.ForeColor = System.Drawing.Color.Black;
            this.gvM.Appearance.ColumnFilterButtonActive.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvM.Appearance.ColumnFilterButtonActive.Options.UseBackColor = true;
            this.gvM.Appearance.ColumnFilterButtonActive.Options.UseBorderColor = true;
            this.gvM.Appearance.ColumnFilterButtonActive.Options.UseForeColor = true;
            this.gvM.Appearance.Empty.BackColor = System.Drawing.Color.White;
            this.gvM.Appearance.Empty.Options.UseBackColor = true;
            this.gvM.Appearance.EvenRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(242)))), ((int)(((byte)(254)))));
            this.gvM.Appearance.EvenRow.ForeColor = System.Drawing.Color.Black;
            this.gvM.Appearance.EvenRow.Options.UseBackColor = true;
            this.gvM.Appearance.EvenRow.Options.UseForeColor = true;
            this.gvM.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvM.Appearance.FilterCloseButton.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gvM.Appearance.FilterCloseButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvM.Appearance.FilterCloseButton.ForeColor = System.Drawing.Color.Black;
            this.gvM.Appearance.FilterCloseButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvM.Appearance.FilterCloseButton.Options.UseBackColor = true;
            this.gvM.Appearance.FilterCloseButton.Options.UseBorderColor = true;
            this.gvM.Appearance.FilterCloseButton.Options.UseForeColor = true;
            this.gvM.Appearance.FilterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(109)))), ((int)(((byte)(185)))));
            this.gvM.Appearance.FilterPanel.ForeColor = System.Drawing.Color.White;
            this.gvM.Appearance.FilterPanel.Options.UseBackColor = true;
            this.gvM.Appearance.FilterPanel.Options.UseForeColor = true;
            this.gvM.Appearance.FixedLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(97)))), ((int)(((byte)(156)))));
            this.gvM.Appearance.FixedLine.Options.UseBackColor = true;
            this.gvM.Appearance.FocusedCell.BackColor = System.Drawing.Color.White;
            this.gvM.Appearance.FocusedCell.ForeColor = System.Drawing.Color.Black;
            this.gvM.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gvM.Appearance.FocusedCell.Options.UseForeColor = true;
            this.gvM.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(106)))), ((int)(((byte)(197)))));
            this.gvM.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White;
            this.gvM.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gvM.Appearance.FocusedRow.Options.UseForeColor = true;
            this.gvM.Appearance.FooterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvM.Appearance.FooterPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gvM.Appearance.FooterPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvM.Appearance.FooterPanel.ForeColor = System.Drawing.Color.Black;
            this.gvM.Appearance.FooterPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvM.Appearance.FooterPanel.Options.UseBackColor = true;
            this.gvM.Appearance.FooterPanel.Options.UseBorderColor = true;
            this.gvM.Appearance.FooterPanel.Options.UseForeColor = true;
            this.gvM.Appearance.GroupButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvM.Appearance.GroupButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvM.Appearance.GroupButton.ForeColor = System.Drawing.Color.Black;
            this.gvM.Appearance.GroupButton.Options.UseBackColor = true;
            this.gvM.Appearance.GroupButton.Options.UseBorderColor = true;
            this.gvM.Appearance.GroupButton.Options.UseForeColor = true;
            this.gvM.Appearance.GroupFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvM.Appearance.GroupFooter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvM.Appearance.GroupFooter.ForeColor = System.Drawing.Color.Black;
            this.gvM.Appearance.GroupFooter.Options.UseBackColor = true;
            this.gvM.Appearance.GroupFooter.Options.UseBorderColor = true;
            this.gvM.Appearance.GroupFooter.Options.UseForeColor = true;
            this.gvM.Appearance.GroupPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(109)))), ((int)(((byte)(185)))));
            this.gvM.Appearance.GroupPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvM.Appearance.GroupPanel.Options.UseBackColor = true;
            this.gvM.Appearance.GroupPanel.Options.UseForeColor = true;
            this.gvM.Appearance.GroupRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvM.Appearance.GroupRow.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvM.Appearance.GroupRow.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.gvM.Appearance.GroupRow.ForeColor = System.Drawing.Color.Black;
            this.gvM.Appearance.GroupRow.Options.UseBackColor = true;
            this.gvM.Appearance.GroupRow.Options.UseBorderColor = true;
            this.gvM.Appearance.GroupRow.Options.UseFont = true;
            this.gvM.Appearance.GroupRow.Options.UseForeColor = true;
            this.gvM.Appearance.HeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvM.Appearance.HeaderPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gvM.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvM.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.gvM.Appearance.HeaderPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvM.Appearance.HeaderPanel.Options.UseBackColor = true;
            this.gvM.Appearance.HeaderPanel.Options.UseBorderColor = true;
            this.gvM.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvM.Appearance.HideSelectionRow.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.gvM.Appearance.HideSelectionRow.ForeColor = System.Drawing.Color.Black;
            this.gvM.Appearance.HideSelectionRow.Options.UseBackColor = true;
            this.gvM.Appearance.HideSelectionRow.Options.UseForeColor = true;
            this.gvM.Appearance.HorzLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.gvM.Appearance.HorzLine.Options.UseBackColor = true;
            this.gvM.Appearance.OddRow.BackColor = System.Drawing.Color.White;
            this.gvM.Appearance.OddRow.ForeColor = System.Drawing.Color.Black;
            this.gvM.Appearance.OddRow.Options.UseBackColor = true;
            this.gvM.Appearance.OddRow.Options.UseForeColor = true;
            this.gvM.Appearance.Preview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.gvM.Appearance.Preview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(129)))), ((int)(((byte)(185)))));
            this.gvM.Appearance.Preview.Options.UseBackColor = true;
            this.gvM.Appearance.Preview.Options.UseForeColor = true;
            this.gvM.Appearance.Row.BackColor = System.Drawing.Color.White;
            this.gvM.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gvM.Appearance.Row.Options.UseBackColor = true;
            this.gvM.Appearance.Row.Options.UseForeColor = true;
            this.gvM.Appearance.RowSeparator.BackColor = System.Drawing.Color.White;
            this.gvM.Appearance.RowSeparator.Options.UseBackColor = true;
            this.gvM.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(126)))), ((int)(((byte)(217)))));
            this.gvM.Appearance.SelectedRow.ForeColor = System.Drawing.Color.White;
            this.gvM.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gvM.Appearance.SelectedRow.Options.UseForeColor = true;
            this.gvM.Appearance.VertLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.gvM.Appearance.VertLine.Options.UseBackColor = true;
            this.gvM.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gc1,
            this.gc2,
            this.gc3,
            this.gc4});
            this.gvM.GridControl = this.gcM;
            this.gvM.Name = "gvM";
            this.gvM.OptionsCustomization.AllowSort = false;
            this.gvM.OptionsFind.AlwaysVisible = true;
            this.gvM.OptionsView.EnableAppearanceEvenRow = true;
            this.gvM.OptionsView.EnableAppearanceOddRow = true;
            this.gvM.OptionsView.ShowGroupPanel = false;
            this.gvM.OptionsView.ShowViewCaption = true;
            this.gvM.ViewCaption = "人事基础部门维护";
            // 
            // gc1
            // 
            this.gc1.AppearanceHeader.Options.UseTextOptions = true;
            this.gc1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gc1.Caption = "部门编号";
            this.gc1.FieldName = "部门编号";
            this.gc1.MaxWidth = 150;
            this.gc1.MinWidth = 10;
            this.gc1.Name = "gc1";
            this.gc1.Visible = true;
            this.gc1.VisibleIndex = 0;
            this.gc1.Width = 70;
            // 
            // gc2
            // 
            this.gc2.AppearanceHeader.Options.UseTextOptions = true;
            this.gc2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gc2.Caption = "部门名称";
            this.gc2.FieldName = "部门名称";
            this.gc2.MinWidth = 10;
            this.gc2.Name = "gc2";
            this.gc2.Visible = true;
            this.gc2.VisibleIndex = 1;
            // 
            // gc3
            // 
            this.gc3.AppearanceHeader.Options.UseTextOptions = true;
            this.gc3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gc3.Caption = "上级部门";
            this.gc3.ColumnEdit = this.repositoryItemSearchLookUpEdit1;
            this.gc3.FieldName = "上级部门";
            this.gc3.MinWidth = 10;
            this.gc3.Name = "gc3";
            this.gc3.Visible = true;
            this.gc3.VisibleIndex = 2;
            // 
            // repositoryItemSearchLookUpEdit1
            // 
            this.repositoryItemSearchLookUpEdit1.AutoHeight = false;
            this.repositoryItemSearchLookUpEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSearchLookUpEdit1.Name = "repositoryItemSearchLookUpEdit1";
            this.repositoryItemSearchLookUpEdit1.NullText = "";
            this.repositoryItemSearchLookUpEdit1.View = this.repositoryItemSearchLookUpEdit1View;
            // 
            // repositoryItemSearchLookUpEdit1View
            // 
            this.repositoryItemSearchLookUpEdit1View.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2});
            this.repositoryItemSearchLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.repositoryItemSearchLookUpEdit1View.Name = "repositoryItemSearchLookUpEdit1View";
            this.repositoryItemSearchLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.repositoryItemSearchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "部门编号";
            this.gridColumn1.FieldName = "部门编号";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "部门名称";
            this.gridColumn2.FieldName = "部门名称";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // gc4
            // 
            this.gc4.AppearanceHeader.Options.UseTextOptions = true;
            this.gc4.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gc4.Caption = "部门领导";
            this.gc4.ColumnEdit = this.repositoryItemSearchLookUpEdit2;
            this.gc4.FieldName = "部门领导";
            this.gc4.MaxWidth = 150;
            this.gc4.MinWidth = 10;
            this.gc4.Name = "gc4";
            this.gc4.Visible = true;
            this.gc4.VisibleIndex = 3;
            // 
            // repositoryItemSearchLookUpEdit2
            // 
            this.repositoryItemSearchLookUpEdit2.AutoHeight = false;
            this.repositoryItemSearchLookUpEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSearchLookUpEdit2.Name = "repositoryItemSearchLookUpEdit2";
            this.repositoryItemSearchLookUpEdit2.NullText = "";
            this.repositoryItemSearchLookUpEdit2.View = this.repositoryItemSearchLookUpEdit2View;
            // 
            // repositoryItemSearchLookUpEdit2View
            // 
            this.repositoryItemSearchLookUpEdit2View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.repositoryItemSearchLookUpEdit2View.Name = "repositoryItemSearchLookUpEdit2View";
            this.repositoryItemSearchLookUpEdit2View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.repositoryItemSearchLookUpEdit2View.OptionsView.ShowGroupPanel = false;
            // 
            // barLargeButtonItem5
            // 
            this.barLargeButtonItem5.Caption = "关闭界面";
            this.barLargeButtonItem5.Glyph = global::BaseData.Properties.Resources.Close_32x32;
            this.barLargeButtonItem5.Id = 4;
            this.barLargeButtonItem5.Name = "barLargeButtonItem5";
            this.barLargeButtonItem5.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem5_ItemClick);
            // 
            // frm人事基础部门维护
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frm人事基础部门维护";
            this.Size = new System.Drawing.Size(926, 509);
            this.Load += new System.EventHandler(this.frm人事基础部门维护_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit2View)).EndInit();
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
        private System.Windows.Forms.Panel panel2;
        private DevExpress.XtraGrid.GridControl gcM;
        private DevExpress.XtraGrid.Views.Grid.GridView gvM;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem1;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem2;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem3;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem4;
        private DevExpress.XtraGrid.Columns.GridColumn gc1;
        private DevExpress.XtraGrid.Columns.GridColumn gc2;
        private DevExpress.XtraGrid.Columns.GridColumn gc3;
        private DevExpress.XtraGrid.Columns.GridColumn gc4;
        private DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit repositoryItemSearchLookUpEdit1;
        private DevExpress.XtraGrid.Views.Grid.GridView repositoryItemSearchLookUpEdit1View;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit repositoryItemSearchLookUpEdit2;
        private DevExpress.XtraGrid.Views.Grid.GridView repositoryItemSearchLookUpEdit2View;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem5;
    }
}
