﻿using System;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Windows.Data;
using MahApps.Metro.Controls.Dialogs;
using VVSAssistant.Common;
using VVSAssistant.Common.ViewModels;
using VVSAssistant.Controls.Dialogs.ViewModels;
using VVSAssistant.Controls.Dialogs.Views;
using VVSAssistant.Database;
using VVSAssistant.Extensions;
using VVSAssistant.Functions;
using VVSAssistant.Models;
using VVSAssistant.Models.DataSheets;
using VVSAssistant.Functions.Calculation;
using VVSAssistant.ValueConverters;
using MahApps.Metro.Controls;

namespace VVSAssistant.ViewModels
{
    public class CreatePackagedSolutionViewModel : FilterableViewModelBase<Appliance>
    {
        #region Inclusion properties

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

        private bool _includeSolarStations;
        public bool IncludeSolarStations
        {
            get { return _includeSolarStations; }
            set
            {
                if (SetProperty(ref _includeSolarStations, value))
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

        private bool _includeWaterHeaters;
        public bool IncludeWaterHeaters
        {
            get { return _includeWaterHeaters; }
            set
            {
                if (SetProperty(ref _includeWaterHeaters, value))
                    FilteredCollectionView.Refresh();
            }
        }

        #endregion

        #region Base properties

        public PackagedSolution PackagedSolution { get; set; }
        private readonly IDialogCoordinator _dialogCoordinator;

        private Appliance _selectedAppliance;
        public Appliance SelectedAppliance
        {
            get { return _selectedAppliance; }
            set
            {
                if (!SetProperty(ref _selectedAppliance, value)) return;

                // Notify if property was changed
                AddApplianceToPackagedSolutionCmd?.NotifyCanExecuteChanged();
                EditApplianceCmd?.NotifyCanExecuteChanged();
                RemoveApplianceCmd?.NotifyCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        private Appliance _selectedApplianceInSolution;
        public Appliance SelectedApplianceInSolution
        {
            get { return _selectedApplianceInSolution; }
            set
            {
                if (!SetProperty(ref _selectedApplianceInSolution, value)) return;

                RemoveApplianceFromPackagedSolutionCmd?.NotifyCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        private EEICalculationResult _eeiResult;
        public EEICalculationResult EeiResultsRoomHeating
        {
            get { return _eeiResult; }
            set { _eeiResult = value; OnPropertyChanged(); }
        }

        private EEICalculationResult _eeiResultSecondary;
        public EEICalculationResult EeiResultsWaterHeating
        {
            get { return _eeiResultSecondary; }
            set { _eeiResultSecondary = value; OnPropertyChanged(); }
        }

        #endregion

        #region Commands

        public RelayCommand AddApplianceToPackagedSolutionCmd { get; }
        public RelayCommand RemoveApplianceFromPackagedSolutionCmd { get; }
        public RelayCommand EditApplianceCmd { get; }
        public RelayCommand RemoveApplianceCmd { get; }
        public RelayCommand NewPackagedSolutionCmd { get; }
        public RelayCommand SavePackagedSolutionCmd { get; }
        public RelayCommand CreateNewApplianceCmd { get; }
        public RelayCommand PdfExportCmd { get; }
        public RelayCommand NavigateBackCmd { get; }

        #endregion

        #region Collections

        public AsyncObservableCollection<Appliance> Appliances { get; set; }
        
        private AsyncObservableCollection<Appliance> AppliancesInPackagedSolution { get;}
        public ICollectionView AppliancesInPackagedSolutionView { get; set; }

        #endregion

        public CreatePackagedSolutionViewModel(IDialogCoordinator dialogCoordinator)
        {
            #region Initialize collections

            Appliances = new AsyncObservableCollection<Appliance>();
            SetupFilterableView(Appliances);

            AppliancesInPackagedSolution = new AsyncObservableCollection<Appliance>();
            AppliancesInPackagedSolutionView = CollectionViewSource.GetDefaultView(AppliancesInPackagedSolution);

            #endregion

            #region Initialize properties

            _dialogCoordinator = dialogCoordinator;
            PackagedSolution = new PackagedSolution();

            #endregion

            #region Bind event listeners

            AppliancesInPackagedSolution.CollectionChanged += PackageSolutionAppliances_CollectionChanged;

            #endregion

            #region Command declarations

            NavigateBackCmd = new RelayCommand(async x =>
            {
                if (!IsDataSaved)
                {
                    var result = await NavigationService.ConfirmDiscardChanges(_dialogCoordinator);
                    if (result == false) return;
                }
                NavigationService.GoBack();
            });

            AddApplianceToPackagedSolutionCmd = new RelayCommand(
                x =>
                {
                    HandleAddApplianceToPackagedSolution(SelectedAppliance);
                },
                x => SelectedAppliance != null);

            RemoveApplianceFromPackagedSolutionCmd = new RelayCommand(x =>
            {
                if (PackagedSolution.PrimaryHeatingUnit == SelectedApplianceInSolution)
                    PackagedSolution.PrimaryHeatingUnit = null;
                
                AppliancesInPackagedSolution.Remove(SelectedApplianceInSolution);
                
            }, x => SelectedApplianceInSolution != null);

            EditApplianceCmd = new RelayCommand(x =>
            {
                RunEditApplianceDialog();
            }, x => SelectedAppliance != null);

            RemoveApplianceCmd = new RelayCommand(x =>
            {
                DropAppliance(SelectedAppliance);
            }, x => SelectedAppliance != null);

            NewPackagedSolutionCmd = new RelayCommand(x =>
            {
                CreateNewPackagedSolution();
            }, x => AppliancesInPackagedSolution.Any());

            SavePackagedSolutionCmd = new RelayCommand(x =>
            {
                if (string.IsNullOrEmpty(PackagedSolution.Name))
                    RunSaveDialog();
                else
                    SaveCurrentPackagedSolution();

            }, x => AppliancesInPackagedSolution.Any());

            CreateNewApplianceCmd = new RelayCommand(x =>
            {
                RunCreateApplianceDialog();
            }
            );

            PdfExportCmd = new RelayCommand(x =>
            {
                PackagedSolution.Appliances = AppliancesInPackagedSolution.ToList();
                DataUtil.EnergyLabel.ExportEnergyLabel(PackagedSolution);
            }, x => AppliancesInPackagedSolution.Any() && IsDataSaved);
            #endregion
        }

        #region Methods

        /// <summary>
        /// Prompt the user to confirm action
        /// Remove the current packaged solution from the view
        /// Initialize new packaged solution
        /// </summary>
        private async void CreateNewPackagedSolution()
        {
            // Run message dialog and await response
            var result = await _dialogCoordinator.ShowMessageAsync(this, 
                "Ny pakkeløsning",
                "Hvis du opretter en ny pakkeløsning mister du arbejdet på din nuværende pakkeløsning. Vil du fortsætte?",
                MessageDialogStyle.AffirmativeAndNegative, 
                new MetroDialogSettings()
                {
                    NegativeButtonText = "Afbryd"
                });

            // If negative button was pressed => return
            if (result == MessageDialogResult.Negative) return;

            // Clear the appliance list for the current packaged solution
            AppliancesInPackagedSolution.Clear();

            // Reset collections and results
            PackagedSolution = new PackagedSolution();
            EeiResultsRoomHeating = new EEICalculationResult();
            EeiResultsWaterHeating = new EEICalculationResult();

            // Update the EEI (=> 0)
            UpdateEei();
        }

        /// <summary>
        /// Adds an appliance to the viewed packaged solution
        /// </summary>
        /// <param name="appliance"></param>
        private void AddApplianceToPackagedSolution(Appliance appliance)
        {
            AppliancesInPackagedSolution.Add(appliance);

            // Set save state to false
            IsDataSaved = false;

            // Notify the print function of changes
            PdfExportCmd.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Drops an appliance from the database and removes it from the displayed list in the UI
        /// </summary>
        /// <param name="appliance"></param>
        private async void DropAppliance(Appliance appliance)
        {
            // Check if the appliance is used in any packaged solutions
            using (var ctx = new AssistantContext())
            {
                var conflictingSolutions = ctx.PackagedSolutions.Where(s => s.ApplianceInstances.Any(a => a.Appliance.Id == appliance.Id)).ToList();
                if (conflictingSolutions.Count > 0)
                {
                    var formattedSolutionString = string.Join("\n", conflictingSolutions.Select(x => $"- {x.Name}"));
                    await _dialogCoordinator.ShowMessageAsync(this, "Fejl",
                        $"Komponentet kan ikke slettes, da det findes i følgende pakkeløsninger:\n{formattedSolutionString}");
                    return;
                }
            }

            // Remove as primary heating unit
            if (PackagedSolution.PrimaryHeatingUnit == appliance)
                PackagedSolution.PrimaryHeatingUnit = null;

            // Remove from solar containers in current solution
            if (PackagedSolution.SolarContainers.Contains(appliance))
                PackagedSolution.SolarContainers.Remove(appliance);

            // Remove from current solution
            if (AppliancesInPackagedSolution.Contains(appliance))
            {
                AppliancesInPackagedSolution.Remove(appliance);
                IsDataSaved = false;
            }
            
            // Remove from visual list of appliances
            Appliances.Remove(appliance);

            // Remove from database
            using (var ctx = new AssistantContext())
            {
                // Attach appliance to this context, since it was loaded using another context
                ctx.Appliances.Attach(appliance);
                ctx.Appliances.Remove(appliance);
                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// Save the viewed packaged solution to the database
        /// </summary>
        private void SaveCurrentPackagedSolution()
        {
            /* IMPORTANT 
             * A packaged solution should not be saved if there exists
             *  a solar collector without a container tied to it. */
            using (var ctx = new AssistantContext())
            {
                ctx.PackagedSolutions.Attach(PackagedSolution);

                // Attach appliances to avoid duplicates
                foreach (var appliance in PackagedSolution.Appliances)
                {
                    //var apps = ctx.Appliances.ToList();
                    if(ctx.Entry(appliance).State == EntityState.Unchanged)
                        ctx.Appliances.Attach(appliance);
                    else
                        ctx.Appliances.Add(appliance);
                }

                // Set the creation date to now
                if (PackagedSolution.CreationDate == default(DateTime))
                    PackagedSolution.CreationDate = DateTime.Now;

                PackagedSolution.SaveToInstances();

                ctx.Entry(PackagedSolution).State = PackagedSolution.Id == 0 ? EntityState.Added : EntityState.Modified;
                //ctx.PackagedSolutions.AddOrUpdate(PackagedSolution);

                // Save database changes
                ctx.SaveChanges();
            }

            // Set save state to true and notify print function of change
            IsDataSaved = true;
            PdfExportCmd.NotifyCanExecuteChanged();
        }

        #region Run dialog methods
        private async void RunAddSolarPanelDialog(Appliance solarPanel)
        {
            var customDialog = new CustomDialog();

            var dialogViewModel = new AddSolarPanelDialogviewModel(solarPanel, AppliancesInPackagedSolution,
                          closeHandler => _dialogCoordinator.HideMetroDialogAsync(this, customDialog),
                          completionHandler => _dialogCoordinator.HideMetroDialogAsync(this, customDialog));

            customDialog.Content = new AddSolarPanelDialogView { DataContext = dialogViewModel };

            await _dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        private async void RunAddHeatingUnitDialog(Appliance appliance)
        {
            var customDialog = new CustomDialog();

            var dialogViewModel = new AddHeatingUnitDialogViewModel(instanceCancel => _dialogCoordinator.HideMetroDialogAsync(this, customDialog),
                async instanceCompleted =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, customDialog);

                    if (instanceCompleted.IsPrimaryBoiler)
                    {
                        if (PackagedSolution.PrimaryHeatingUnit != null)
                        {
                            // Inform the user that their previous primary heating unit will be replaced
                            await _dialogCoordinator.ShowMessageAsync(this, "Information",
                                    $"Da du har valgt en ny primærkedel er komponentet {PackagedSolution.PrimaryHeatingUnit.Name} nu en sekundærkedel.");
                        }
                        PackagedSolution.PrimaryHeatingUnit = appliance;
                    }
                    AddApplianceToPackagedSolution(appliance);
                });

            customDialog.Content = new AddHeatingUnitDialogView { DataContext = dialogViewModel };

            await _dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        private async void RunSolarContainerDialog(string title, Appliance appliance)
        {
            var customDialog = new CustomDialog();
            var dialogViewModel = new SolarContainerDialogViewModel(title, appliance, PackagedSolution, AppliancesInPackagedSolution,
                                                                    closeHandler => _dialogCoordinator.HideMetroDialogAsync(this, customDialog),
                                                                    completionHandler => _dialogCoordinator.HideMetroDialogAsync(this, customDialog));

            customDialog.Content = new SolarContainerDialogView { DataContext = dialogViewModel };

            await _dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        private async void RunSaveDialog()
        {
            var customDialog = new CustomDialog();

            var dialogViewModel = new SaveDialogViewModel(instanceCancel => _dialogCoordinator.HideMetroDialogAsync(this, customDialog), async instanceCompleted =>
                                                            {
                                                                PackagedSolution.Name = instanceCompleted.Input;
                                                                OnPropertyChanged("PackagedSolution.Name");
                                                                SaveCurrentPackagedSolution();

                                                                await _dialogCoordinator.HideMetroDialogAsync(this, customDialog);
                                                                DisplayTimedMessage("Succes", $"Pakkeløsningen \"{PackagedSolution.Name}\" blev gemt.", 2);
                                                            });

            customDialog.Content = new SaveDialogView { DataContext = dialogViewModel };

            await _dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        private async void RunEditApplianceDialog()
        {
            var customDialog = new CustomDialog();

            var dialogViewModel = new CreateApplianceDialogViewModel(SelectedAppliance, false,
                instanceCancel => _dialogCoordinator.HideMetroDialogAsync(this, customDialog),
                instanceCompleted =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, customDialog);
                    FilteredCollectionView.Refresh();
                    AppliancesInPackagedSolutionView.Refresh();

                    // Update appliance changes in database
                    using (var ctx = new AssistantContext())
                    {
                        ctx.Entry(SelectedAppliance).State = EntityState.Modified;
                        ctx.Entry(SelectedAppliance.DataSheet).State = EntityState.Modified;

                        ctx.SaveChanges();
                    }
                });

            customDialog.Content = new CreateApplianceDialogView { DataContext = dialogViewModel };

            await _dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        private async void RunCreateApplianceDialog()
        {
            var customDialog = new CustomDialog();
            var newAppliance = new Appliance();
            var dialogViewModel = new CreateApplianceDialogViewModel(newAppliance, true,
                closeHandler => _dialogCoordinator.HideMetroDialogAsync(this, customDialog), async completionHandler =>
                {
                    // Save to database
                    using (var ctx = new AssistantContext())
                    {
                        newAppliance.CreationDate = DateTime.Now;
                        ctx.Appliances.Add(newAppliance);
                        ctx.SaveChanges();
                    }

                    // Add to local list
                    //if(!Appliances.Any(item => item.Equals(newAppliance)))
                    Appliances.Add(newAppliance);

                    await _dialogCoordinator.HideMetroDialogAsync(this, customDialog);
                    await _dialogCoordinator.ShowMessageAsync(this, "Succes", $"Komponentet {newAppliance.Name} blev gemt.");
                });

            customDialog.Content = new CreateApplianceDialogView { DataContext = dialogViewModel };

            await _dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }
        #endregion
        
        private void PackageSolutionAppliances_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NewPackagedSolutionCmd.NotifyCanExecuteChanged();
            SavePackagedSolutionCmd.NotifyCanExecuteChanged();
            PdfExportCmd.NotifyCanExecuteChanged();
            UpdateEei();
        }

        public override void LoadDataFromDatabase()
        {
            using (var ctx = new AssistantContext())
            {
                var apps = ctx.Appliances.Include(a => a.DataSheet).ToList();
                apps.Distinct().ToList().ForEach(Appliances.Add);
            }
        }

        public void LoadExistingPackagedSolution(int packagedSolutionId)
        {
            using (var ctx = new AssistantContext())
            {
                var existingPackagedSolution = ctx.PackagedSolutions.Where(p => p.Id == packagedSolutionId)
                    .Include(s => s.SolarContainerInstances.Select(i => i.Appliance.DataSheet))
                    .Include(s => s.PrimaryHeatingUnitInstance.Appliance.DataSheet)
                    .Include(s => s.ApplianceInstances.Select(i => i.Appliance.DataSheet))
                    .FirstOrDefault();

                if (existingPackagedSolution == null) return;

                existingPackagedSolution.LoadFromInstances();

                // Copy primary heating unit
                PackagedSolution.PrimaryHeatingUnit = existingPackagedSolution.PrimaryHeatingUnit;

                // Copy solarcontainers
                foreach (var solarContainer in existingPackagedSolution.SolarContainers)
                    PackagedSolution.SolarContainers.Add(solarContainer);

                // Copy appliances
                foreach (var appliance in existingPackagedSolution.Appliances)
                    AppliancesInPackagedSolution.Add(appliance);
            }
        }

        protected override bool Filter(Appliance obj)
        {
            // Filter based on type first
            if (IncludeBoilers || IncludeCentralHeatingPlants || IncludeContainers || IncludeHeatPumps || IncludeSolarStations
                || IncludeLowTempHeatPumps || IncludeSolarPanels || IncludeTemperatureControllers || IncludeWaterHeaters)
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
                    case ApplianceTypes.HeatPump:
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
                    case ApplianceTypes.SolarStation:
                        if (!IncludeSolarStations)
                            return false;
                        break;
                    case ApplianceTypes.WaterHeater:
                        if (!IncludeWaterHeaters)
                            return false;
                        break;
                    default:
                        return false;
                }
            }

            // Filter based on FilterString
            return obj.Name.ContainsIgnoreCase(FilterString) ||
                   obj.Description.ContainsIgnoreCase(FilterString) ||
                   ApplianceTypeConverter.ConvertTypeToString(obj.Type).ContainsIgnoreCase(FilterString);
        }

        private void HandleAddApplianceToPackagedSolution(Appliance appToAdd)
        {
            if (appToAdd.DataSheet is HeatingUnitDataSheet)
            {
                /* Prompt user for whether or not the heating unit is primary */
                /* If it is primary, ask whether or not it is for water heating, 
                 * room heating, or both. */
                RunAddHeatingUnitDialog(appToAdd);
            }
            else if (appToAdd.DataSheet is ContainerDataSheet)
            {
                /* Prompt the user for whether or not the container is tied to any of the solar collector. */
                const string title = "Vælg om denne beholder er til Solvarme";
                RunSolarContainerDialog(title, appToAdd);
            }
            else if(appToAdd.DataSheet is SolarCollectorDataSheet)
            {
                RunAddSolarPanelDialog(appToAdd);
            }
            else
            {
                AddApplianceToPackagedSolution(appToAdd);
            }
        }

        private void UpdateEei()
        {
            PackagedSolution.Appliances = AppliancesInPackagedSolution.ToList();
            PackagedSolution.EnergyLabel.Clear();
            PackagedSolution.UpdateEEI();
            if (PackagedSolution.EnergyLabel != null && PackagedSolution.EnergyLabel.Count > 1)
            {
                EeiResultsRoomHeating = PackagedSolution.EnergyLabel[0];
                EeiResultsWaterHeating = PackagedSolution.EnergyLabel[1];
            }
            else if(PackagedSolution.EnergyLabel != null && PackagedSolution.EnergyLabel.Count==1)
            {
                EeiResultsRoomHeating = PackagedSolution.EnergyLabel?[0];
                EeiResultsWaterHeating = null;
            }
            else
            {
                EeiResultsRoomHeating = null;
                EeiResultsWaterHeating = null;
            }
        }

        private async void DisplayTimedMessage(string title, string message, double time)
        {
            var customDialog = new CustomDialog();
            var messageViewModel = new TimedMessageViewModel(title, message, time, instanceCancel => _dialogCoordinator.HideMetroDialogAsync(this, customDialog));
            customDialog.Content = new TimesMessageView { DataContext = messageViewModel };
            await _dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        #endregion

    }
}
