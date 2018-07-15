using MNDataSearch.Helper;
using MNDataSearch.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MNDataSearch.View
{
    /// <summary>
    /// Interaction logic for SelectColumns.xaml
    /// </summary>
    public partial class SelectColumns : Window
    {
        internal DataGrid dgResult;

        public SelectColumns()
        {
            InitializeComponent();
            this.Loaded += SelectColumns_Loaded;
        }

        private void SelectColumns_Loaded(object sender, RoutedEventArgs e)
        {
            lbColumns.ItemsSource = dgResult.Columns;// SourceToPrint;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
         
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            string columnName = System.Convert.ToString((sender as CheckBox).Content);
            dgResult.Columns.Where(v => v.Header.ToString() == columnName).FirstOrDefault().Visibility =
                (sender as CheckBox).IsChecked.Value ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            string columnName = System.Convert.ToString((sender as CheckBox).Content);
            dgResult.Columns.Where(v => v.Header.ToString() == columnName).FirstOrDefault().Visibility =
                (sender as CheckBox).IsChecked.Value ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
