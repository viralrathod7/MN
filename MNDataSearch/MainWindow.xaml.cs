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
        MainWindowViewModel vm = new MainWindowViewModel();
        public MainWindow()
        {
            InitializeComponent();
            Helper.GlobalClass.RefreshDataAsync();
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

            lbCategory.ItemsSource = Helper.GlobalClass.Categories.Select(v => v.Name);

            lbCategory.SelectedIndex = 0;
            cmbDirector.SelectedIndex = 0;
            //Producer.SelectedIndex = 0;
            cmbMainClass.SelectedIndex = 0;
            cmbLanguage.SelectedIndex = 0;
            cmbYear.SelectedIndex = 0;
            //cmbCastCrew.SelectedIndex = 0; 

            sliderDuration.Minimum = Helper.GlobalClass.Data.Min(v => v.Duration);
            sliderDuration.Maximum = Helper.GlobalClass.Data.Max(v => v.Duration);
            sliderDuration.Value = Helper.GlobalClass.Data.Max(v => v.Duration);
        }


        private void lbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateSubCategories(lbCategory.SelectedValue.ToString());
        }

        private void PopulateSubCategories(string CategoryName)
        {
            lbSubcategory.ItemsSource = Helper.GlobalClass.Categories.Where(v => v.Name == CategoryName).FirstOrDefault().SubCategory;
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
                Convert.ToString(lbSubcategory.SelectedValue), Convert.ToString(cmbDirector.SelectedValue),
                Convert.ToString(cmbMainClass.SelectedValue),
                Convert.ToString(cmbLanguage.SelectedValue),
                sliderDuration.Value, Convert.ToString(cmbYear.SelectedValue), rbBoth.IsChecked.Value, rbColor.IsChecked.Value, rbBW.IsChecked.Value);
            dwindow.ShowDialog();
        }

        private void btnHideAdvSearch_Click(object sender, RoutedEventArgs e)
        {
            brAdvSearch.Visibility = Visibility.Collapsed;
        }
    }
}
