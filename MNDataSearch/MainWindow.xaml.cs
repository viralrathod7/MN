using MNDataSearch.ViewModels;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MNDataSearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string all = "All";
        MainWindowViewModel vm = new MainWindowViewModel();
        public MainWindow()
        {
            InitializeComponent();
            //Helper.GlobalClass.RefreshDataAsync();
            Helper.GlobalClass.RefreshExcelDataAsync();
            brAdvSearch.Visibility = Visibility.Collapsed;
            this.DataContext = vm;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Helper.GlobalClass.Data.Count > 0)
                PopulateData();
        }

        private void btnAdvSearchClick_Click(object sender, RoutedEventArgs e)
        {
            brAdvSearch.Visibility = Visibility.Visible;
        }

        private void PopulateData()
        {
            vm.PopulateData();


            lbCategory.ItemsSource = Helper.GlobalClass.Categories;
            lbMainClass.ItemsSource = Helper.GlobalClass.MainClass.Select(v => v.Name);

            lbMainClass.SelectedIndex = 0;
            lbCategory.SelectedIndex = 0;
            cmbDirector.SelectedIndex = 0;
            //Producer.SelectedIndex = 0;
            cmbLanguage.SelectedIndex = 0;
            cmbYear.SelectedIndex = 0;
            //cmbCastCrew.SelectedIndex = 0; 

            sliderDuration.Minimum = Helper.GlobalClass.Data.Min(v => v.Duration);
            sliderDuration.Maximum = Helper.GlobalClass.Data.Max(v => v.Duration);
            sliderDuration.Value = Helper.GlobalClass.Data.Max(v => v.Duration);

            txtSearch.Text = "";
            txtKeyword.Text = "";
        }


        private void lbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Nothing to do while category change
        }

        private void lbMainClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateSubCategories(lbMainClass.SelectedValue.ToString());
        }

        private void PopulateSubCategories(string MainClassName)
        {
            lbSubcategory.ItemsSource = Helper.GlobalClass.MainClass.Where(v => v.Name == MainClassName).FirstOrDefault().SubCategory;
            lbSubcategory.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string url = "http://filmsdivision.org";
            Process.Start(new ProcessStartInfo(url));
        }

        private void btnAdvSearch_Click(object sender, RoutedEventArgs e)
        {
            string dTitle = (sender as Button) == btnSearch ? txtSearch.Text.Trim() : txtTitle.Text.Trim();
            DataWindow dwindow = new DataWindow(vm, dTitle, Convert.ToString(lbCategory.SelectedValue),
                 Convert.ToString(lbMainClass.SelectedValue), Convert.ToString(lbSubcategory.SelectedValue),
                 Convert.ToString(cmbDirector.SelectedValue), txtKeyword.Text.Trim(),
                Convert.ToString(cmbLanguage.SelectedValue),
                sliderDuration.Value, Convert.ToString(cmbYear.SelectedValue), rbBoth.IsChecked.Value, rbColor.IsChecked.Value, rbBW.IsChecked.Value);
            dwindow.ShowDialog();
        }

        private void btnHideAdvSearch_Click(object sender, RoutedEventArgs e)
        {
            brAdvSearch.Visibility = Visibility.Collapsed;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
        }

        private void btnAdvClear_Click(object sender, RoutedEventArgs e)
        {
            PopulateData();
        }
    }
}
