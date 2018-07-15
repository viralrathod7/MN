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
        public static string ExcelFileLocation = @"C:\Users\Khushi\Desktop\RajivTomar\Search Engine.xlsx";
        public static string ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + AccessDatabaseLocation + ";Persist Security Info=True";
        public static string ConnectionStringExcel = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source = " + ExcelFileLocation + ";" + "Extended Properties = 'Excel 12.0 Xml;HDR=YES;'";
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
                {
                    AccessDatabaseLocation = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\SearchCatlouge.mdb";
                    ExcelFileLocation = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\Search Engine.xlsx";
                }
            }
            else
            {
                ExcelFileLocation = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\Search Engine.xlsx";
                AccessDatabaseLocation = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\SearchCatlouge.mdb";
            }
            ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + AccessDatabaseLocation + ";Persist Security Info=True";
            ConnectionStringExcel = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source = " + ExcelFileLocation + ";" + "Extended Properties = 'Excel 12.0 Xml;HDR=YES;'";

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
            catch (Exception Ex)
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

        public static async Task RefreshExcelDataAsync()
        {
            LoadSettingsAsync();

            Data = new List<Catlouge>();
            Categories = new List<string>() { "All" };
            MainClass = new List<MainClass>() { new MainClass() { Name = "All", SubCategory = new List<string>() { "All" } } };
            try
            {
                using (var cn = new OleDbConnection(ConnectionStringExcel))
                {
                    cn.Open();
                    OleDbCommand cmd = new OleDbCommand("Select * from [Sheet1$] ", cn);
                    var dr = await cmd.ExecuteReaderAsync();
                    Catlouge ct;
                    while (dr.Read())
                    {
                        ct = new Catlouge();
                        if (dr["Sr No"] == DBNull.Value) break;
                        ct.SrNo = Convert.ToInt32(dr["Sr No"]);
                        ct.UniqueNo = Convert.ToString(dr["UNIQUENO"]).Trim();
                        ct.ImagePath = ImagesFolderPath + ct.UniqueNo + ".jpg";
                        ct.Title = Convert.ToString(dr["Title"]).Trim();
                        ct.Category = Convert.ToString(dr["Category"]).Trim();
                        ct.SubCategory = Convert.ToString(dr["Sub Class"]).Trim();
                        ct.SubCategory2 = Convert.ToString(dr["Sub Class2"]).Trim();
                        ct.Synopsis = Convert.ToString(dr["SYNOPSIS"]).Trim();
                        ct.Language = Convert.ToString(dr["LANGUAGE"]).Trim();
                        ct.Year = Convert.ToInt32(dr["YEAR"] == DBNull.Value ? 1900 : dr["YEAR"]);
                        ct.Duration = Convert.ToDouble(dr["DURATION"] == DBNull.Value ? 0.0 : dr["DURATION"]);
                        ct.bW = Convert.ToString(dr["B&W"]).Trim();
                        ct.Director = Convert.ToString(dr["DIRECTOR"]).Trim();
                        ct.Producer = "";// dt.Columns.Contains("Producer") ? Convert.ToString(dr["Producer"]).Trim() : "";
                        ct.MainClass = Convert.ToString(dr["Main Class"]).Trim();
                        ct.MainClass2 = Convert.ToString(dr["Main Class2"]).Trim();
                        ct.CastCrew = Convert.ToString(dr["Cast & Crew"]).Trim();

                        if (!Categories.Contains(ct.Category) && !string.IsNullOrWhiteSpace(ct.Category))
                            Categories.Add(ct.Category);

                        if (!MainClass[0].SubCategory.Contains(ct.SubCategory) && !string.IsNullOrWhiteSpace(ct.SubCategory))
                        {
                            MainClass[0].SubCategory.Add(ct.SubCategory);
                        }

                        if (!MainClass[0].SubCategory.Contains(ct.SubCategory2) && !string.IsNullOrWhiteSpace(ct.SubCategory2))
                        {
                            MainClass[0].SubCategory.Add(ct.SubCategory2);
                        }

                        if (!File.Exists(ct.ImagePath))
                        {
                            ct.ImagePath = null;
                        }

                        if (!MainClass.Select(v => v.Name).Contains(ct.MainClass) || !MainClass.Select(v => v.Name).Contains(ct.MainClass2))
                        {
                            if (!MainClass.Select(v => v.Name).Contains(ct.MainClass) && !string.IsNullOrWhiteSpace(ct.MainClass))
                            {
                                MainClass.Add(new MainClass
                                {
                                    ID = MainClass.Count + 1,
                                    Name = ct.MainClass,
                                    SubCategory = new List<string>() { "All" }
                                });

                                if (!string.IsNullOrWhiteSpace(ct.SubCategory))
                                    MainClass.Last().SubCategory.Add(ct.SubCategory);

                                if (!string.IsNullOrWhiteSpace(ct.SubCategory2))
                                    MainClass.Last().SubCategory.Add(ct.SubCategory2);
                            }

                            if (!MainClass.Select(v => v.Name).Contains(ct.MainClass2) && !string.IsNullOrWhiteSpace(ct.MainClass2))
                            {
                                MainClass.Add(new MainClass
                                {
                                    ID = MainClass.Count + 1,
                                    Name = ct.MainClass2,
                                    SubCategory = new List<string>() { "All" }
                                });

                                if (!string.IsNullOrWhiteSpace(ct.SubCategory))
                                    MainClass.Last().SubCategory.Add(ct.SubCategory);

                                if (!string.IsNullOrWhiteSpace(ct.SubCategory2))
                                    MainClass.Last().SubCategory.Add(ct.SubCategory2);
                            }
                        }
                        else
                        {
                            MainClass catTemp = MainClass.Where(v => v.Name == ct.MainClass || v.Name == ct.MainClass2).FirstOrDefault();
                            if (!catTemp.SubCategory.Contains(ct.SubCategory) && !string.IsNullOrWhiteSpace(ct.SubCategory))
                            {
                                catTemp.SubCategory.Add(ct.SubCategory);
                            }
                            if (!catTemp.SubCategory.Contains(ct.SubCategory2) && !string.IsNullOrWhiteSpace(ct.SubCategory2))
                            {
                                catTemp.SubCategory.Add(ct.SubCategory2);
                            }
                        }

                        Data.Add(ct);
                    }
                    cn.Close();
                }

                //Apply Sorting in Category
                Categories = Categories.OrderBy(c => c == "All" ? "A" : c).ToList();
                //Apply Sorting in MainClass & SubCategory
                MainClass = MainClass.OrderBy(v => v.Name == "All" ? "A" : v.Name).ToList();
                foreach (var item in MainClass)
                    item.SubCategory = item.SubCategory.OrderBy(v => v == "All" ? "A" : v).ToList();
            }
            catch (Exception Ex)
            {

            }
        }
    }
}
