﻿using MahApps.Metro.Controls;
using EnergyLabellingPrototype.Models;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.MessageBox;

namespace EnergyLabellingPrototype.Pages
{
    /// <summary>
    /// Interaction logic for SolutionPage.xaml
    /// </summary>
    public partial class SolutionPage : Page
    {
        private int Pack_Id;
        private string Pack;
        private ObservableCollection<Appliance> _packagedComponents = new ObservableCollection<Appliance>();

        public SolutionPage()
        {
            InitializeComponent();
        }

        public SolutionPage(Solution solution, int id):this()
        {
            Pack_Id = id;
            Pack = "";
            Add_Solution_Too_Shop(solution);
        }

        private void Add_Solution_Too_Shop(Solution solution)
        {
            foreach (var item in solution.Appliances)
            {
                _packagedComponents.Add(item);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Update_Page();
        }

        private void Update_Page()
        {
            RefreshDynamicItemSources();
            dataGridPackage.ItemsSource = _packagedComponents;
        }

        private void RefreshDynamicItemSources()
        {
            var filterText = textBoxSearch.Text.ToLower();

            // Update component list
            dataGridComponents.ItemsSource = App._componentList.Where(c => c.FilterMatch(filterText));
            dataGridComponents.Items.Refresh();
        }

        
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).DataContext as Appliance;
            Appliance c = new Appliance(item.Name, item.Description, item.Type, item.SalesPrice);
            App.MainWindow.Edit.IsOpen = true;
            int count = 0;
            foreach (Appliance VARIABLE in App._componentList)
            {
                if (VARIABLE.Counter.Equals(item.Counter))
                {
                    break;
                }
                count++;
            }
            App.MainWindow.Info_in_fly(c,count);
        }

        private void AddToPackageButton_Click(object sender, RoutedEventArgs e)
        {
            var item = ( sender as Button ).DataContext as Appliance;
            Add_Component_To_Shop(item);
        }

        private void Add_Component_To_Shop(Appliance item)
        {
            _packagedComponents.Add(item);
            dataGridPackage.Items.Refresh();
        }

        private void AddSolToPackageButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).DataContext as Solution;
            Pack_Id = item.Counter;
            Pack = "";
            Add_Solution_Too_Shop(item);
            dataGridPackage.Items.Refresh();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            _packagedComponents.Remove((sender as Button ).DataContext as Appliance);
            dataGridPackage.Items.Refresh();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            if (_packagedComponents.Count >0)
            {
                if (Pack != null)
                {
                    foreach (var test in App._packagedList)
                    {
                        if (test.Counter.Equals(Pack_Id))
                        {
                            App._packagedList[count].Appliances.Clear();
                            foreach (Appliance item in _packagedComponents)
                            {
                                App._packagedList[count].Appliances.Add(item);
                            }
                            break;
                        }
                        count++;
                    }
                    //_packagedComponents.Clear();
                    Update_Page();
                    dataGridPackage.Items.Refresh();
                }
                else
                {
                    await App.MainWindow.ShowMessageAsync("Fejl", "Ingen pakke at gemme til");
                }

                Pack = null;
            }
            else
            {
                await App.MainWindow.ShowMessageAsync("Fejl", "Du skal tilføje komponenter til pakkeløsningen før den kan gemmes");
            }
           
        }
        private void infoButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).DataContext as Solution;
            Solution c = new Solution(item.Name, item.Appliances);
            App.MainWindow.infosolution.IsOpen = true;
            App.MainWindow.Solution_Info(c);
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshDynamicItemSources();
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            _packagedComponents.Clear();
            dataGridPackage.Items.Refresh();
            Pack = null;
        }

        private  async void SaveNewButton_Click(object sender, RoutedEventArgs e)
        {
            if (_packagedComponents.Count > 0)
            {
                Solution s = new Solution("Pakke", _packagedComponents);
                App._packagedList.Add(s);

                string name = await App.MainWindow.ShowInputAsync("Pakkeløsning", "Angiv et navn til den nye pakkeløsning:");
                if (string.IsNullOrEmpty(name))
                    return;
                s.Name = name;

                //_packagedComponents.Clear();
                Update_Page();
                dataGridPackage.Items.Refresh();
            }
            else
            {
                await App.MainWindow.ShowMessageAsync("Fejl", "Du skal tilføje komponenter til pakkeløsningen før den kan gemmes");
            }
        }

        private void RemovePackButton_Click(object sender, RoutedEventArgs e)
        {
            Pack = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = (BaseMetroDialog) Resources["CustomCloseDialogTest"];
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (_packagedComponents.Count > 0)
            {
                await App.MainWindow.ShowMessageAsync("Success", "Dit pakkelabel er blevet printet");
            }
            else
            {
                await App.MainWindow.ShowMessageAsync("Fejl", "Du skal tilføje komponenter til pakkeløsningen før den kan printes");
            }
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var dialog = (BaseMetroDialog) this.Resources["AddBoilerDialog"];

            await App.MainWindow.HideMetroDialogAsync(dialog);
        }

        private void addComponent_Click(object sender, RoutedEventArgs e)
        {
            App.MainWindow.newFlyout.IsOpen = true;
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void DoubleClickOnComponent(object sender, MouseButtonEventArgs e)
        {
            var UIel = Mouse.DirectlyOver as UIElement;
            if (UIel is Button)
            {
                return;
            }
            if (dataGridComponents.SelectedItem == null) return;
            var selectedComponent = dataGridComponents.SelectedItem as Appliance;
            Add_Component_To_Shop(selectedComponent);
        }

        private void RightClickOnComponent(object sender, MouseButtonEventArgs e)
        {
            var UIel = Mouse.DirectlyOver as UIElement;
            if (UIel is Button)
            {
                return;
            }
            if (dataGridComponents.SelectedItem == null) return;
            var selectedComponent = dataGridComponents.SelectedItem as Appliance;
            
            Add_Component_To_Shop(selectedComponent);
        }

        private void dataGridComponents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        
    }
}