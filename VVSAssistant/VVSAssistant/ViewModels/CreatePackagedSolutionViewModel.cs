﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Specialized;
using MahApps.Metro.Controls.Dialogs;
using VVSAssistant.Common.ViewModels.VVSAssistant.Common.ViewModels;
using VVSAssistant.ViewModels.MVVM;
using VVSAssistant.Controls.Dialogs.ViewModels;
using VVSAssistant.Controls.Dialogs.Views;
using VVSAssistant.Extensions;
using VVSAssistant.Models;

namespace VVSAssistant.ViewModels
{
    public class CreatePackagedSolutionViewModel : FilterableViewModelBase<Appliance>
    {
        #region Property initializations

        private bool _includeHeatPumps;
        public bool IncludeHeatPumps
        {
            get { return _includeHeatPumps; }
            set
            {
                if (SetProperty(ref _includeHeatPumps, value))
                    FilteredCollectionView.Refresh();
            }
        }

        private bool _includeBoilers;
        public bool IncludeBoilers
        {
            get { return _includeBoilers; }
            set
            {
                if (SetProperty(ref _includeBoilers, value))
                    FilteredCollectionView.Refresh();
            }
        }

        private bool _includeTemperatureControllers;
        public bool IncludeTemperatureControllers
        {
            get { return _includeTemperatureControllers; }
            set
            {
                if (SetProperty(ref _includeTemperatureControllers, value))
                    FilteredCollectionView.Refresh();
            }
        }

        private bool _includeSolarPanels;
        public bool IncludeSolarPanels
        {
            get { return _includeSolarPanels; }
            set
            {
                if (SetProperty(ref _includeSolarPanels, value))
                    FilteredCollectionView.Refresh();
            }
        }

        private bool _includeContainers;
        public bool IncludeContainers
        {
            get { return _includeContainers; }
            set
            {
                if (SetProperty(ref _includeContainers, value))
                    FilteredCollectionView.Refresh();
            }
        }

        private bool _includeLowTempHeatPumps;
        public bool IncludeLowTempHeatPumps
        {
            get { return _includeLowTempHeatPumps; }
            set
            {
                if (SetProperty(ref _includeLowTempHeatPumps, value))
                    FilteredCollectionView.Refresh();
            }
        }

        private bool _includeCentralHeatingPlants;
        public bool IncludeCentralHeatingPlants
        {
            get { return _includeCentralHeatingPlants; }
            set
            {
                if (SetProperty(ref _includeCentralHeatingPlants, value))
                    FilteredCollectionView.Refresh();
            }
        }

        private PackagedSolution _packagedSolution;
        public PackagedSolution PackagedSolution
        {
            get { return _packagedSolution; }
            set
            {
                _packagedSolution = value;
                AppliancesInSolution = new ObservableCollection<Appliance>();
                AppliancesInSolution.CollectionChanged += PackageSolutionAppliances_CollectionChanged;
            }

        }

        private readonly IDialogCoordinator _dialogCoordinator;

        private Appliance _selectedAppliance;
        public Appliance SelectedAppliance
        {
            get { return _selectedAppliance; }
            set
            {
                if (!SetProperty(ref _selectedAppliance, value)) return;

                // Notify if property was changed
                AddApplianceToPackageSolution.NotifyCanExecuteChanged();
                EditAppliance.NotifyCanExecuteChanged();
                RemoveAppliance.NotifyCanExecuteChanged();
            }
        }
        #endregion

        #region Command initializations
        public RelayCommand AddApplianceToPackageSolution { get; }
        public RelayCommand RemoveApplianceFromPackageSolution { get; }
        public RelayCommand EditAppliance { get; }
        public RelayCommand RemoveAppliance { get; }
        public RelayCommand NewPackageSolution { get; }
        public RelayCommand SaveDialog { get; }
        #endregion

        #region Collections
        public ObservableCollection<Appliance> Appliances { get; } = new ObservableCollection<Appliance>();
        public ObservableCollection<Appliance> AppliancesInSolution { get; set; }
        #endregion

        public CreatePackagedSolutionViewModel(IDialogCoordinator dialogCoordinator)
        {
            SetupFilterableView(Appliances);
            PackagedSolution = new PackagedSolution();
            _dialogCoordinator = dialogCoordinator;

            #region Command declarations

            AddApplianceToPackageSolution = new RelayCommand(x => 
            {
                AppliancesInSolution.Add(SelectedAppliance);
            }, x => SelectedAppliance != null);

            RemoveApplianceFromPackageSolution = new RelayCommand(x =>
            {
                AppliancesInSolution.Remove(SelectedAppliance);
            }, x => SelectedAppliance != null);

            EditAppliance = new RelayCommand(x =>
            {
                RunEditDialog();
            }, x => SelectedAppliance != null);

            RemoveAppliance = new RelayCommand(x =>
            {
                RemoveApplianceRENAMEMEPLZ(SelectedAppliance);
            }, x => SelectedAppliance != null);

            NewPackageSolution = new RelayCommand(x =>
            {
                AppliancesInSolution.Clear();
            }, x => PackagedSolution.Appliances.Any());

            SaveDialog = new RelayCommand(x =>
            {
                RunSaveDialog();
            }, x => AppliancesInSolution.Any());
            #endregion
        }

        private void RemoveApplianceRENAMEMEPLZ(Appliance appliance)
        {
            Appliances.Remove(appliance);

            DbContext.Appliances.Remove(appliance);
            DbContext.SaveChanges();
        }

        private void SaveCurrentPackagedSolution()
        {
            PackagedSolution.CreationDate = DateTime.Now;
            PackagedSolution.Appliances = new ApplianceList(AppliancesInSolution.ToList());

            DbContext.PackagedSolutions.Add(PackagedSolution);
            DbContext.SaveChanges();

            PackagedSolution = new PackagedSolution();
        }

        private async void RunSaveDialog()
        {
            var customDialog = new CustomDialog();

            var dialogViewModel = new SaveDialogViewModel("Gem pakkeløsning", "Navn:",
                instanceCancel => _dialogCoordinator.HideMetroDialogAsync(this, customDialog),
                instanceCompleted =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, customDialog);
                    PackagedSolution.Name = instanceCompleted.Input;

                    SaveCurrentPackagedSolution();
                }); 
            
            customDialog.Content = new SaveDialogView { DataContext = dialogViewModel };
            await _dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        private async void RunEditDialog()
        {
            var customDialog = new CustomDialog();

            var dialogViewModel = new EditApplianceViewModel(SelectedAppliance, instanceCancel => _dialogCoordinator.HideMetroDialogAsync(this, customDialog), instanceCompleted => _dialogCoordinator.HideMetroDialogAsync(this, customDialog));

            customDialog.Content = new EditApplianceView { DataContext = dialogViewModel };

            await _dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }
        
        private void PackageSolutionAppliances_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NewPackageSolution.NotifyCanExecuteChanged();
            SaveDialog.NotifyCanExecuteChanged();
        }

        public override void LoadDataFromDatabase()
        {
            DbContext.Appliances.ToList().ForEach(Appliances.Add);
        }

        protected override bool Filter(Appliance obj)
        {
            // Filter based on type first
            if (IncludeBoilers ||
                IncludeCentralHeatingPlants ||
                IncludeContainers ||
                IncludeHeatPumps ||
                IncludeLowTempHeatPumps ||
                IncludeSolarPanels ||
                IncludeTemperatureControllers)
            {
                switch (obj.Type)
                {
                    case ApplianceTypes.Boiler:
                        if (!IncludeBoilers)
                            return false;
                        break;
                    case ApplianceTypes.CHP:
                        if (!IncludeCentralHeatingPlants)
                            return false;
                        break;
                    case ApplianceTypes.Container:
                        if (!IncludeContainers)
                            return false;
                        break;
                    case ApplianceTypes.Heatpump:
                        if (!IncludeHeatPumps)
                            return false;
                        break;
                    case ApplianceTypes.LowTempHeatPump:
                        if (!IncludeLowTempHeatPumps)
                            return false;
                        break;
                    case ApplianceTypes.SolarPanel:
                        if (!IncludeSolarPanels)
                            return false;
                        break;
                    case ApplianceTypes.TemperatureController:
                        if (!IncludeTemperatureControllers)
                            return false;
                        break;
                    default:
                        return false;
                }
            }

            // Filter based on FilterString
            if (!obj.Name.ContainsIgnoreCase(FilterString) && !obj.Description.ContainsIgnoreCase(FilterString))
                return false;

            return true;
        }
    }
}
