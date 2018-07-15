using MNDataSearch.Helper;
using MNDataSearch.Models;
using MNDataSearch.View;
using MNDataSearch.ViewModels;
using System;
using System.Collections.Generic; 
using System.Linq;
using System.Linq.Expressions;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace MNDataSearch
{
    /// <summary>
    /// Interaction logic for DataWindow.xaml
    /// </summary>
    public partial class DataWindow : Window
    {
        string all = "All";
        public DataWindow(MainWindowViewModel vm, string pTitle, string pCategory, string pMainClass, string pSubCategory, string pDirector, string pKeyword,
            string pLang, double pDuration, string pYear, bool isBoth, bool isColor, bool isBW)
        {
            InitializeComponent();
            this.DataContext = vm;
            PopulateCategories();

            sliderDuration.Minimum = Helper.GlobalClass.Data.Min(v => v.Duration);
            sliderDuration.Maximum = Helper.GlobalClass.Data.Max(v => v.Duration);
            sliderDuration.Value = Helper.GlobalClass.Data.Max(v => v.Duration);

            txtSearch.Text = pTitle;
            lbCategory.SelectedValue = pCategory;
            lbSubcategory.SelectedValue = pSubCategory;
            cmbDirector.SelectedValue = pDirector;
            lbMainClass.SelectedValue = pMainClass;
            cmbLanguage.SelectedValue = pLang;
            cmbYear.SelectedValue = pYear;
            sliderDuration.Value = pDuration;

            if (isBoth)
                rbBoth.IsChecked = isBoth;
            else if (isColor)
                rbColor.IsChecked = isColor;
            else if (isBW)
                rbBW.IsChecked = isBW;

            FilterData();
        }

        private void dgResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgResult.SelectedItem is Catlouge)
            {
                CatalougeDetail cd = new CatalougeDetail();
                cd.DataContext = dgResult.SelectedItem;
                var response = cd.ShowDialog();
                if (response.HasValue && response.Value)
                {
                    //PrintDialog pd = new PrintDialog();
                    //if (pd.ShowDialog() == true)
                    //{
                    //    pd.PrintVisual(cd, "Catlouge Print");
                    //}
                }
            }
        }

        private void PopulateCategories()
        {
            lbCategory.ItemsSource = Helper.GlobalClass.Categories;
            lbCategory.SelectedIndex = 0;
            lbMainClass.ItemsSource = Helper.GlobalClass.MainClass.Select(v => v.Name);
            lbMainClass.SelectedIndex = 0;
        }
        private void lbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterData();
        }
        private void PopulateSubCategories(string MainClassName)
        {
            lbSubcategory.ItemsSource = Helper.GlobalClass.MainClass.Where(v => v.Name == MainClassName).FirstOrDefault().SubCategory;
            lbSubcategory.SelectedItem = 0;
        }
        private void FilterData()
        {
            if (this.DataContext == null) return;

            string strTitle = txtSearch.Text.Trim().ToLower();
            string category = lbCategory.SelectedValue == null ? all : lbCategory.SelectedValue.ToString();
            string subCategory = lbSubcategory.SelectedValue == null ? all : lbSubcategory.SelectedValue.ToString();
            string director = cmbDirector.SelectedValue == null ? all : cmbDirector.SelectedValue.ToString();
            string mainClass = lbMainClass.SelectedValue == null ? all : lbMainClass.SelectedValue.ToString();
            string keyword = string.IsNullOrWhiteSpace(txtKeyword.Text) ? all : txtKeyword.Text.Trim().ToLower();
            string language = cmbLanguage.SelectedValue == null ? all : cmbLanguage.SelectedValue.ToString();
            string year = cmbYear.SelectedValue == null ? all : cmbYear.SelectedValue.ToString();

            dgResult.ItemsSource = Helper.GlobalClass.Data.Where(v =>
                   (string.IsNullOrEmpty(strTitle) ? true : v.Title.ToLower().Contains(strTitle))
                       && (category == all ? true : v.Category == category)
                           && (subCategory == all ? true : v.SubCategory == subCategory)
                           && (v.Duration <= sliderDuration.Value)
                           && (director == all ? true : v.Director == director)
                           && (mainClass == all ? true : v.MainClass == mainClass)
                           //             && (producer == all ? true : v.Producer == producer)
                           && (language == all ? true : v.Language == language)
                           && (year == all ? true : v.Year.ToString() == year)
                           && (rbBoth.IsChecked.Value ? true : (rbBW.IsChecked.Value ? (v.bW.ToLower() == "b&w") : (v.bW.ToLower() == "col")))
                           && (keyword == all ? true : (v.Title.ToLower().Contains(keyword) || v.Category.ToLower().Contains(keyword) ||
                                                       v.MainClass.ToLower().Contains(keyword) || v.SubCategory.ToLower().Contains(keyword) ||
                                                       v.Director.ToLower().Contains(keyword) || v.Language.ToLower().Contains(keyword) ||
                                                       v.Synopsis.ToLower().Contains(keyword)))
                    ).ToList();

        }

        private void btnAdvSearch_Click(object sender, RoutedEventArgs e)
        {
            if ((dgResult.ItemsSource as List<Catlouge>).Count == 0 && txtSearch.Text.Trim().Length > 2)
            {
                var spellingErrorIndex = txtSearch.Text.Length;
                do
                {
                    var spellingError = txtSearch.GetSpellingError(spellingErrorIndex);
                    if (spellingError != null)
                    {
                        var suggestions = spellingError.Suggestions;    //suggests "consideration of"
                        spellingError.Correct(suggestions.First());
                    }
                    spellingErrorIndex = txtSearch.GetNextSpellingErrorCharacterIndex(spellingErrorIndex, System.Windows.Documents.LogicalDirection.Backward);
                } while (spellingErrorIndex >= 0);
            }
            FilterData();
        }

        private void lbSubcategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterData();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterData();
        }

        private void sliderDuration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FilterData();
        }

        private void cmbDirector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterData();
        }

        private void rbBoth_Checked(object sender, RoutedEventArgs e)
        {
            FilterData();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            //double someFixHeight = 520;
            //int count = (dgResult.ItemsSource as List<Catlouge>).Count;
            //dgResult.Height = ((count / 9.5) * someFixHeight);
            try
            {
                PrintDialog printDialog = new PrintDialog();
                PageMediaSize pageSize = new PageMediaSize(PageMediaSizeName.ISOA4);
                printDialog.PrintTicket.PageMediaSize = pageSize;
                printDialog.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;

                bool? pdResult = printDialog.ShowDialog();
                if (pdResult != null && pdResult.Value)
                {
                    FixedDocument document;
                    if (dgResult.SelectedItems.Count > 0)
                    {
                        List<Catlouge> selectedItems =new List<Catlouge>();
                        foreach (var item in dgResult.SelectedItems)  
                            selectedItems.Add(item as Catlouge);  
                        List<Catlouge> existingList = (dgResult.ItemsSource as List<Catlouge>).ToList();

                        dgResult.ItemsSource = selectedItems;
                        dgResult.UpdateLayout();

                        document = PrintHelper.CreateFixedDocument(dgResult, "Header", 30, 60);
                        printDialog.PrintDocument(document.DocumentPaginator, "Search Results");

                        dgResult.ItemsSource = existingList;
                        dgResult.UpdateLayout();
                    }
                    else
                    {
                        document = PrintHelper.CreateFixedDocument(dgResult, "Header", 30, 60);
                        printDialog.PrintDocument(document.DocumentPaginator, "Search Results");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " : " + ex.InnerException);
            }
            //dgResult.Height = someFixHeight;
        }

        private void lbMainClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateSubCategories(lbMainClass.SelectedValue.ToString());
        }

        private void txtKeyword_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterData();
        }

        private void btnColumns_Click(object sender, RoutedEventArgs e)
        {
            SelectColumns sc = new SelectColumns();
            sc.dgResult = dgResult;
            sc.ShowDialog();
        }

        //private void dataGridView1_CellPainting(object sender, DataGridCellEditingEventArgs e)
        //{
        //    if (e.RowIndex >= 0 & e.ColumnIndex >= 0 & IsSelected)
        //    {
        //        e.Handled = true;
        //        e.PaintBackground(e.CellBounds, true);

        //        string sw = txtSearch.Text;

        //        if (!string.IsNullOrEmpty(sw))
        //        {
        //            string val = (string)e.FormattedValue;
        //            int sindx = val.ToLower().IndexOf(sw.ToLower());
        //            if (sindx >= 0)
        //            {
        //                Rectangle hl_rect = new Rectangle();
        //                hl_rect.Y = e.CellBounds.Y + 2;
        //                hl_rect.Height = e.CellBounds.Height - 5;

        //                string sBefore = val.Substring(0, sindx);
        //                string sWord = val.Substring(sindx, sw.Length);
        //                Size s1 = TextRenderer.MeasureText(e.Graphics, sBefore, e.CellStyle.Font, e.CellBounds.Size);
        //                Size s2 = TextRenderer.MeasureText(e.Graphics, sWord, e.CellStyle.Font, e.CellBounds.Size);

        //                if (s1.Width > 5)
        //                {
        //                    hl_rect.X = e.CellBounds.X + s1.Width - 5;
        //                    hl_rect.Width = s2.Width - 6;
        //                }
        //                else
        //                {
        //                    hl_rect.X = e.CellBounds.X + 2;
        //                    hl_rect.Width = s2.Width - 6;
        //                }

        //                SolidBrush hl_brush = default(SolidBrush);
        //                if (((e.State & DataGridViewElementStates.Selected) != DataGridViewElementStates.None))
        //                {
        //                    hl_brush = new SolidBrush(Color.DarkGoldenrod);
        //                }
        //                else
        //                {
        //                    hl_brush = new SolidBrush(Color.Yellow);
        //                }

        //                e.Graphics.FillRectangle(hl_brush, hl_rect);

        //                hl_brush.Dispose();
        //            }
        //        }
        //        e.PaintContent(e.CellBounds);
        //    }
        //} 
        //private void dgResult_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        //{
        //    string str = Convert.ToString(e.Column.GetValue(DataGridCell.ContentProperty));
        //    e.Cancel = true;
        //    DataGridTemplateColumn col = new DataGridTemplateColumn();
        //    //if (((System.Data.DataRowView)((((System.Windows.Controls.ItemsControl)(sender)).Items).CurrentItem)).Row.ItemArray[1] == "X")
        //    //    col.CellTemplate = CreateCellTemplate(e.PropertyName);
        //    //else
        //    //    col.CellTemplate = CreateButtonTemplate(e.PropertyName);
        //    //e.Column = col;
        //    //fieldGrid.Columns.Add(col);
        //    //fieldGrid.ColumnWidth = 10;
        //}
        //private static DataTemplate CreateCellTemplate(string propertyName)
        //{
        //    DataTemplate template = new DataTemplate();
        //    FrameworkElementFactory stk = new FrameworkElementFactory(typeof(StackPanel));
        //    stk.Name = "stackPanel";
        //    stk.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);
        //    //stk.SetValue(TextBlock.MaxWidthProperty,10);
        //    FrameworkElementFactory txtU = new FrameworkElementFactory(typeof(TextBlock));
        //    txtU.Name = "txtUpper";
        //    //txtU.SetValue(TextBlock.BackgroundProperty,new SolidColorBrush( Colors.YellowGreen)); 
        //    //the color will be set from dataset 
        //    txtU.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Top);
        //    //txtU.SetValue(TextBlock.WidthProperty, new GridLength(10));
        //    stk.AppendChild(txtU);
        //    FrameworkElementFactory txtL = new FrameworkElementFactory(typeof(TextBlock));
        //    txtL.Name = "txtLower";
        //    //txtL.SetValue(TextBlock.BackgroundProperty, new SolidColorBrush(Colors.Tomato));
        //    txtL.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Bottom);
        //    // txtL.SetValue(TextBlock.WidthProperty, new GridLength(10));         
        //    stk.AppendChild(txtL);
        //    template.VisualTree = stk;
        //    template.VisualTree.SetBinding(ContentProperty, new System.Windows.Data.Binding(propertyName));
        //    return template;
        //}

    }
}
