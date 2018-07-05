using MNDataSearch.Models;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace MNDataSearch.View
{
    /// <summary>
    /// Interaction logic for SelectColumns.xaml
    /// </summary>
    public partial class SelectColumns : Window
    {
        internal DataGrid dg;

        public SelectColumns()
        {
            InitializeComponent();
            this.Loaded += SelectColumns_Loaded;
        }

        private void SelectColumns_Loaded(object sender, RoutedEventArgs e)
        {
            lbColumns.ItemsSource = SourceToPrint;
        }

        public ObservableCollection<DataGridColumn> SourceToPrint { get; internal set; }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == true)
            {
                pd.PrintVisual(dg, "Catlouge Details");
            }
            DialogResult = true;
        }
    }

}
