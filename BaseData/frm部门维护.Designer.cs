namespace BaseData
{
    partial class frm部门维护
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.simpleButton5 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton4 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton3 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.tv = new DevExpress.XtraTreeList.TreeList();
            this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn2 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn3 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn4 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.repositoryItemSearchLookUpEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit();
            this.repositoryItemSearchLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit1View)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel1.Controls.Add(this.simpleButton5);
            this.panel1.Controls.Add(this.simpleButton4);
            this.panel1.Controls.Add(this.simpleButton3);
            this.panel1.Controls.Add(this.simpleButton2);
            this.panel1.Controls.Add(this.simpleButton1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(943, 57);
            this.panel1.TabIndex = 0;
            // 
            // simpleButton5
            // 
            this.simpleButton5.Image = global::BaseData.Properties.Resources.GenerateData_32x32;
            this.simpleButton5.Location = new System.Drawing.Point(13, 12);
            this.simpleButton5.Name = "simpleButton5";
            this.simpleButton5.Size = new System.Drawing.Size(119, 39);
            this.simpleButton5.TabIndex = 4;
            this.simpleButton5.Text = "刷新";
            this.simpleButton5.Click += new System.EventHandler(this.simpleButton5_Click);
            // 
            // simpleButton4
            // 
            this.simpleButton4.Image = global::BaseData.Properties.Resources.Save_32x32;
            this.simpleButton4.Location = new System.Drawing.Point(513, 12);
            this.simpleButton4.Name = "simpleButton4";
            this.simpleButton4.Size = new System.Drawing.Size(119, 39);
            this.simpleButton4.TabIndex = 3;
            this.simpleButton4.Text = "保存";
            this.simpleButton4.Click += new System.EventHandler(this.simpleButton4_Click);
            // 
            // simpleButton3
            // 
            this.simpleButton3.Image = global::BaseData.Properties.Resources.Remove_32x32;
            this.simpleButton3.Location = new System.Drawing.Point(388, 12);
            this.simpleButton3.Name = "simpleButton3";
            this.simpleButton3.Size = new System.Drawing.Size(119, 39);
            this.simpleButton3.TabIndex = 2;
            this.simpleButton3.Text = "删除部门";
            this.simpleButton3.Click += new System.EventHandler(this.simpleButton3_Click);
            // 
            // simpleButton2
            // 
            this.simpleButton2.Image = global::BaseData.Properties.Resources.AddToLibrary_32x321;
            this.simpleButton2.Location = new System.Drawing.Point(263, 12);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(119, 39);
            this.simpleButton2.TabIndex = 1;
            this.simpleButton2.Text = "添加同级部门";
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // simpleButton1
            // 
            this.simpleButton1.Image = global::BaseData.Properties.Resources.Arrow_down_32px_1184716_easyicon1;
            this.simpleButton1.Location = new System.Drawing.Point(138, 12);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(119, 39);
            this.simpleButton1.TabIndex = 0;
            this.simpleButton1.Text = "添加下级部门";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // tv
            // 
            this.tv.Appearance.Empty.BackColor = System.Drawing.Color.White;
            this.tv.Appearance.Empty.Options.UseBackColor = true;
            this.tv.Appearance.EvenRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(242)))), ((int)(((byte)(254)))));
            this.tv.Appearance.EvenRow.ForeColor = System.Drawing.Color.Black;
            this.tv.Appearance.EvenRow.Options.UseBackColor = true;
            this.tv.Appearance.EvenRow.Options.UseForeColor = true;
            this.tv.Appearance.FocusedCell.BackColor = System.Drawing.Color.White;
            this.tv.Appearance.FocusedCell.ForeColor = System.Drawing.Color.Black;
            this.tv.Appearance.FocusedCell.Options.UseBackColor = true;
            this.tv.Appearance.FocusedCell.Options.UseForeColor = true;
            this.tv.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(106)))), ((int)(((byte)(197)))));
            this.tv.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White;
            this.tv.Appearance.FocusedRow.Options.UseBackColor = true;
            this.tv.Appearance.FocusedRow.Options.UseForeColor = true;
            this.tv.Appearance.FooterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.tv.Appearance.FooterPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.tv.Appearance.FooterPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.tv.Appearance.FooterPanel.ForeColor = System.Drawing.Color.Black;
            this.tv.Appearance.FooterPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.tv.Appearance.FooterPanel.Options.UseBackColor = true;
            this.tv.Appearance.FooterPanel.Options.UseBorderColor = true;
            this.tv.Appearance.FooterPanel.Options.UseForeColor = true;
            this.tv.Appearance.GroupButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.tv.Appearance.GroupButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.tv.Appearance.GroupButton.ForeColor = System.Drawing.Color.Black;
            this.tv.Appearance.GroupButton.Options.UseBackColor = true;
            this.tv.Appearance.GroupButton.Options.UseBorderColor = true;
            this.tv.Appearance.GroupButton.Options.UseForeColor = true;
            this.tv.Appearance.GroupFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.tv.Appearance.GroupFooter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.tv.Appearance.GroupFooter.ForeColor = System.Drawing.Color.Black;
            this.tv.Appearance.GroupFooter.Options.UseBackColor = true;
            this.tv.Appearance.GroupFooter.Options.UseBorderColor = true;
            this.tv.Appearance.GroupFooter.Options.UseForeColor = true;
            this.tv.Appearance.HeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.tv.Appearance.HeaderPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.tv.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.tv.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.tv.Appearance.HeaderPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.tv.Appearance.HeaderPanel.Options.UseBackColor = true;
            this.tv.Appearance.HeaderPanel.Options.UseBorderColor = true;
            this.tv.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.tv.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(106)))), ((int)(((byte)(153)))), ((int)(((byte)(228)))));
            this.tv.Appearance.HideSelectionRow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(224)))), ((int)(((byte)(251)))));
            this.tv.Appearance.HideSelectionRow.Options.UseBackColor = true;
            this.tv.Appearance.HideSelectionRow.Options.UseForeColor = true;
            this.tv.Appearance.HorzLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.tv.Appearance.HorzLine.Options.UseBackColor = true;
            this.tv.Appearance.OddRow.BackColor = System.Drawing.Color.White;
            this.tv.Appearance.OddRow.ForeColor = System.Drawing.Color.Black;
            this.tv.Appearance.OddRow.Options.UseBackColor = true;
            this.tv.Appearance.OddRow.Options.UseForeColor = true;
            this.tv.Appearance.Preview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.tv.Appearance.Preview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(129)))), ((int)(((byte)(185)))));
            this.tv.Appearance.Preview.Options.UseBackColor = true;
            this.tv.Appearance.Preview.Options.UseForeColor = true;
            this.tv.Appearance.Row.BackColor = System.Drawing.Color.White;
            this.tv.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.tv.Appearance.Row.Options.UseBackColor = true;
            this.tv.Appearance.Row.Options.UseForeColor = true;
            this.tv.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(126)))), ((int)(((byte)(217)))));
            this.tv.Appearance.SelectedRow.ForeColor = System.Drawing.Color.White;
            this.tv.Appearance.SelectedRow.Options.UseBackColor = true;
            this.tv.Appearance.SelectedRow.Options.UseForeColor = true;
            this.tv.Appearance.TreeLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(97)))), ((int)(((byte)(156)))));
            this.tv.Appearance.TreeLine.Options.UseBackColor = true;
            this.tv.Appearance.VertLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.tv.Appearance.VertLine.Options.UseBackColor = true;
            this.tv.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListColumn1,
            this.treeListColumn2,
            this.treeListColumn3,
            this.treeListColumn4});
            this.tv.Cursor = System.Windows.Forms.Cursors.Default;
            this.tv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tv.Location = new System.Drawing.Point(0, 57);
            this.tv.Name = "tv";
            this.tv.OptionsFind.AllowFindPanel = true;
            this.tv.OptionsFind.AlwaysVisible = true;
            this.tv.OptionsView.EnableAppearanceEvenRow = true;
            this.tv.OptionsView.EnableAppearanceOddRow = true;
            this.tv.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemSearchLookUpEdit1});
            this.tv.Size = new System.Drawing.Size(943, 450);
            this.tv.TabIndex = 1;
            this.tv.HiddenEditor += new System.EventHandler(this.tv_HiddenEditor);
            // 
            // treeListColumn1
            // 
            this.treeListColumn1.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn1.Caption = "组织架构";
            this.treeListColumn1.FieldName = "组织架构";
            this.treeListColumn1.Name = "treeListColumn1";
            this.treeListColumn1.OptionsColumn.AllowEdit = false;
            this.treeListColumn1.OptionsColumn.AllowSort = false;
            this.treeListColumn1.Visible = true;
            this.treeListColumn1.VisibleIndex = 0;
            // 
            // treeListColumn2
            // 
            this.treeListColumn2.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn2.Caption = "部门编号";
            this.treeListColumn2.FieldName = "部门编号";
            this.treeListColumn2.Name = "treeListColumn2";
            this.treeListColumn2.OptionsColumn.AllowSort = false;
            this.treeListColumn2.Visible = true;
            this.treeListColumn2.VisibleIndex = 1;
            // 
            // treeListColumn3
            // 
            this.treeListColumn3.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn3.Caption = "部门名称";
            this.treeListColumn3.FieldName = "部门名称";
            this.treeListColumn3.Name = "treeListColumn3";
            this.treeListColumn3.OptionsColumn.AllowSort = false;
            this.treeListColumn3.Visible = true;
            this.treeListColumn3.VisibleIndex = 2;
            // 
            // treeListColumn4
            // 
            this.treeListColumn4.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn4.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn4.Caption = "部门领导";
            this.treeListColumn4.ColumnEdit = this.repositoryItemSearchLookUpEdit1;
            this.treeListColumn4.FieldName = "部门领导";
            this.treeListColumn4.Name = "treeListColumn4";
            this.treeListColumn4.OptionsColumn.AllowSort = false;
            this.treeListColumn4.Visible = true;
            this.treeListColumn4.VisibleIndex = 3;
            // 
            // repositoryItemSearchLookUpEdit1
            // 
            this.repositoryItemSearchLookUpEdit1.AutoHeight = false;
            this.repositoryItemSearchLookUpEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSearchLookUpEdit1.Name = "repositoryItemSearchLookUpEdit1";
            this.repositoryItemSearchLookUpEdit1.View = this.repositoryItemSearchLookUpEdit1View;
            // 
            // repositoryItemSearchLookUpEdit1View
            // 
            this.repositoryItemSearchLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.repositoryItemSearchLookUpEdit1View.Name = "repositoryItemSearchLookUpEdit1View";
            this.repositoryItemSearchLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.repositoryItemSearchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // frm部门维护
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tv);
            this.Controls.Add(this.panel1);
            this.Name = "frm部门维护";
            this.Size = new System.Drawing.Size(943, 507);
            this.Load += new System.EventHandler(this.frm部门维护_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit1View)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraTreeList.TreeList tv;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn2;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn3;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn4;
        private DevExpress.XtraEditors.SimpleButton simpleButton4;
        private DevExpress.XtraEditors.SimpleButton simpleButton3;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit repositoryItemSearchLookUpEdit1;
        private DevExpress.XtraGrid.Views.Grid.GridView repositoryItemSearchLookUpEdit1View;
        private DevExpress.XtraEditors.SimpleButton simpleButton5;
    }
}
