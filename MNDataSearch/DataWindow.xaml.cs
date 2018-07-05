using MNDataSearch.Models;
using MNDataSearch.View;
using MNDataSearch.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MNDataSearch
{
    /// <summary>
    /// Interaction logic for DataWindow.xaml
    /// </summary>
    public partial class DataWindow : Window
    {
        string all = "All";
        public DataWindow(MainWindowViewModel vm, string pTitle, string pCategory, string pSubCategory, string pDirector, string pMainClass,
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
            cmbMainClass.SelectedValue = pMainClass;
            //cmbProducer.SelectedValue = pProducer;
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
            lbCategory.ItemsSource = Helper.GlobalClass.Categories.Select(v => v.Name);
            lbCategory.SelectedIndex = 0;
        }
        private void lbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateSubCategories(lbCategory.SelectedValue.ToString());
        }
        private void PopulateSubCategories(string CategoryName)
        {
            lbSubcategory.ItemsSource = Helper.GlobalClass.Categories.Where(v => v.Name == CategoryName).FirstOrDefault().SubCategory;
            lbSubcategory.SelectedItem = 0;
        }
        private void FilterData()
        {
            string strTitle = txtSearch.Text.Trim().ToLower();
            string category = lbCategory.SelectedValue == null ? all : lbCategory.SelectedValue.ToString();
            string subCategory = lbSubcategory.SelectedValue == null ? all : lbSubcategory.SelectedValue.ToString();
            string director = cmbDirector.SelectedValue == null ? all : cmbDirector.SelectedValue.ToString();
            string mainClass = cmbMainClass.SelectedValue == null ? all : cmbMainClass.SelectedValue.ToString();
            //string producer = cmbProducer.SelectedValue == null ? all : cmbProducer.SelectedValue.ToString();
            string language = cmbLanguage.SelectedValue == null ? all : cmbLanguage.SelectedValue.ToString();
            string year = cmbYear.SelectedValue == null ? all : cmbYear.SelectedValue.ToString();

            dgResult.ItemsSource = Helper.GlobalClass.Data.Where(v =>
                   (string.IsNullOrEmpty(strTitle) ? true : v.Title.ToLower().Contains(strTitle))
                       && (category == all ? true : v.Category == category)
                           && (subCategory == all ? true : v.SubCategory == subCategory)
                           && (v.Duration <= sliderDuration.Value)
                           && (director == all ? true : v.Director == director)
                           && (mainClass == all ? true : v.MainClass == mainClass )
              //             && (producer == all ? true : v.Producer == producer)
                           && (language == all ? true : v.Language == language)
                           && (year == all ? true : v.Year.ToString() == year)
                           && (rbBoth.IsChecked.Value ? true : (rbBW.IsChecked.Value ? (v.bW.ToLower() == "b&w") : (v.bW.ToLower() == "col")))).ToList();
        }

        private void btnAdvSearch_Click(object sender, RoutedEventArgs e)
        {
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
            //SelectColumns sc = new SelectColumns();
            //sc.dg = dgResult;
            //sc.SourceToPrint = dgResult.Columns;
            //sc.ShowDialog();

            //System.Windows.Controls.PrintDialog Printdlg = new System.Windows.Controls.PrintDialog();
            //if ((bool)Printdlg.ShowDialog().GetValueOrDefault())
            //{
            //    Size pageSize = new Size(Printdlg.PrintableAreaWidth, Printdlg.PrintableAreaHeight);
            //    // sizing of the element.
            //    dgResult.Measure(pageSize);

            //    dgResult.Arrange(new Rect(5, 5, pageSize.Width, pageSize.Height));
            //    Printdlg.PrintVisual(dgResult, Title);
            //}

            

        }
    }
}
