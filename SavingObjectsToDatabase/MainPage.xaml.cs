using SavingObjectsToDatabase.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// hi how are u

namespace SavingObjectsToDatabase
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        #region Declarations
        Models.Meal ml = null;
        Models.Benefit bnf = null;
        Models.FoodItem fi = null;
 private void SampleData1()
        {
            //test objects
            bnf = new Models.Benefit();
            fi = new Models.FoodItem();
            ml = new Models.Meal();
            bnf.Name = "Benefit Number 1";
            fi.Name = "Banana";
            ml.Name = "Food";


        }



        #endregion
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            SampleData1();

            // Writedb();
            ReadAllRecords();
            ReturnOneRecord();
        }

        private void Writedb()
        {
            DataBaseTools.FieldListBuilder<Models.Benefit>(bnf, "Benefit");
            DataBaseTools.FieldListBuilder<Models.FoodItem>(fi, "FoodItem");
            DataBaseTools.FieldListBuilder<Models.Meal>(ml, "Meal");

            TBResults.Text = DataBaseTools.CreateDatabase();

            DataBaseTools.WriteRecord();

        }


        private void ReadAllRecords()
        {
            ObservableCollection<Models.Benefit> bens = new ObservableCollection<Models.Benefit>();
            bens = DataBaseTools.ReturnAllRecords<Models.Benefit>("Benefit", "SavingObjectsToDatabase.Models.Benefit");
            lv1.Items.Add(bens[0].Name.ToString());
        }

        private void ReturnOneRecord()
        {
            Models.FoodItem fi = new Models.FoodItem();
            fi=DataBaseTools.SearchRecords<Models.FoodItem>("FoodItem", "SavingObjectsToDatabase.Models.FoodItem","Name","Banana");
            lv1.Items.Add(fi.Name);
        }
    }
}
