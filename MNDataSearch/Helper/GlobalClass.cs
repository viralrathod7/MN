using MNDataSearch.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MNDataSearch.Helper
{
    public static class GlobalClass
    {
        public static string ImagesFolderPath = @"C:\Users\Khushi\Desktop\RajivTomar\Images\Images\";
        public static string AccessDatabaseLocation = @"C:\Users\Khushi\Desktop\RajivTomar\MNSearch\MNSearch\App_Data\SearchCatlouge.mdb";
        public static string ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + AccessDatabaseLocation + ";Persist Security Info=True";
                                                 //Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\Khushi\Desktop\RajivTomar\MNDataSearch\MNDataSearch\App_Data\SearchCatlouge.mdb;Persist Security Info=True
        public static List<Catlouge> Data = new List<Catlouge>();
        public static List<string> Categories = new List<string>();
        public static List<MainClass> MainClass = new List<MainClass>();

        public static void LoadSettingsAsync()
        {
            if (ConfigurationSettings.AppSettings.HasKeys() && ConfigurationSettings.AppSettings.AllKeys.Contains("ImagesFolderPath"))
            {
                ImagesFolderPath = ConfigurationSettings.AppSettings["ImagesFolderPath"];
                if (ImagesFolderPath == "ROOT")
                    ImagesFolderPath = AppDomain.CurrentDomain.BaseDirectory + @"Images\";
            }
            else
            {
                ImagesFolderPath = AppDomain.CurrentDomain.BaseDirectory + @"Images\";
            }

            if (ConfigurationSettings.AppSettings.HasKeys() && ConfigurationSettings.AppSettings.AllKeys.Contains("AccessDatabaseLocation"))
            {
                AccessDatabaseLocation = ConfigurationSettings.AppSettings["AccessDatabaseLocation"];
                if (AccessDatabaseLocation == "ROOT")
                    AccessDatabaseLocation = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\SearchCatlouge.mdb";
            }
            else
            {
                AccessDatabaseLocation = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\SearchCatlouge.mdb";
            }
            ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + AccessDatabaseLocation + ";Persist Security Info=True";

            if (!IsFileExists())
            {
                MessageBox.Show("Database does not exist on given path : " + AccessDatabaseLocation);
            }
            else if (!IsDatabaseExists())
            {
                MessageBox.Show("Database connectivity issues : " + ConnectionString);
            }
            if (!IsFolderExists())
                MessageBox.Show("Images folder does not exist : " + ImagesFolderPath);
        }

        public static bool IsDatabaseExists()
        {
            try
            {
                using (var cn = new OleDbConnection(ConnectionString))
                {
                    cn.Open();
                    cn.Close();
                }
            }
            catch(Exception Ex)
            {
                return false;
            }
            return true;
        }

        public static bool IsFileExists()
        {
            return File.Exists(AccessDatabaseLocation);
        }

        public static bool IsFolderExists()
        {
            return Directory.Exists(ImagesFolderPath);
        }
        public static async Task RefreshDataAsync()
        {
            LoadSettingsAsync();

            Data = new List<Catlouge>();
            Categories = new List<string>() { "All" };
            MainClass = new List<MainClass>() { new MainClass() { Name = "All", SubCategory = new List<string>() { "All" } } };
            try
            {
                using (var cn = new OleDbConnection(ConnectionString))
                {
                    cn.Open();
                    OleDbCommand cmd = new OleDbCommand("Select * from tbl_ECatlouge;", cn);
                    var dr = await cmd.ExecuteReaderAsync();
                    Catlouge ct;
                    while (dr.Read())
                    {
                        ct = new Catlouge();
                        ct.SrNo = Convert.ToInt32(dr["Sr No"]);
                        ct.UniqueNo = Convert.ToString(dr["UNIQUENO"]).Trim();
                        ct.ImagePath = ImagesFolderPath + ct.UniqueNo + ".jpg";
                        ct.Title = Convert.ToString(dr["Title"]).Trim();
                        ct.Category = Convert.ToString(dr["Category"]).Trim();
                        ct.SubCategory = Convert.ToString(dr["Sub Category"]).Trim();
                        ct.Synopsis = Convert.ToString(dr["SYNOPSIS"]).Trim();
                        ct.Language = Convert.ToString(dr["LANGUAGE"]).Trim();
                        ct.Year = Convert.ToInt32(dr["YEAR"] == DBNull.Value ? 1900 : dr["YEAR"]);
                        ct.Duration = Convert.ToDouble(dr["DURATION"] == DBNull.Value ? 0.0 : dr["DURATION"]);
                        ct.bW = Convert.ToString(dr["B&W"]).Trim();
                        ct.Director = Convert.ToString(dr["DIRECTOR"]).Trim();
                        ct.Producer = Convert.ToString(dr["Producer"]).Trim();
                        ct.MainClass = Convert.ToString(dr["MainClass"]).Trim();
                        ct.CastCrew = Convert.ToString(dr["Cast & Crew"]).Trim();

                        if (!Categories.Contains(ct.Category))
                            Categories.Add(ct.Category);

                        if (!MainClass[0].SubCategory.Contains(ct.SubCategory))
                            MainClass[0].SubCategory.Add(ct.SubCategory);

                        if (!File.Exists(ct.ImagePath))
                        {
                            ct.ImagePath = null;
                        }

                        if (!MainClass.Select(v => v.Name).Contains(ct.Category))
                        {
                            MainClass.Add(new MainClass
                            {
                                ID = MainClass.Count + 1,
                                Name = ct.Category,
                                SubCategory = new List<string>() { "All", ct.SubCategory }
                            });
                        }
                        else
                        {
                            MainClass catTemp = MainClass.Where(v => v.Name == ct.Category).FirstOrDefault();
                            if (!catTemp.SubCategory.Contains(ct.SubCategory))
                            {
                                catTemp.SubCategory.Add(ct.SubCategory);
                            }
                        }

                        Data.Add(ct);
                    }
                    cn.Close();
                }

            }
            catch (Exception Ex)
            {

            }
        }

    }
}
