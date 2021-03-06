﻿using System;
using System.Collections.ObjectModel;
using VVSAssistant.Common;
using VVSAssistant.Common.ViewModels;
using VVSAssistant.Functions;
using VVSAssistant.Models;
using VVSAssistant.Models.DataSheets;
using VVSAssistant.ValueConverters;

namespace VVSAssistant.Controls.Dialogs.ViewModels
{
    internal class CreateApplianceDialogViewModel : NotifyPropertyChanged
    {
        /* When the DataSheet (type) of a new appliance is changed during creation */
        public delegate void DataSheetChanged(DataSheet dataSheet);
        public static event DataSheetChanged DataSheetChangedEventHandler;
        public static void OnDataSheetChanged(DataSheet dataSheet)
        {
            DataSheetChangedEventHandler?.Invoke(dataSheet);
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CloseCommand { get; }

        public string ChosenType
        {
            get
            {
                return ApplianceTypeConverter.ConvertTypeToString(_newAppliance.Type);
            }
            set
            {
                if (value == null)
                    return;
                ApplianceTypes type = ApplianceTypeConverter.ConvertStringToType(value); //Save for the new instance
                string name = NewAppliance.Name; //Save for the new instance
                NewAppliance = appFactory.CreateAppliance(type);
                NewAppliance.Name = name;
                NewAppliance.Type = type;
                OnPropertyChanged("NewAppliance");
                OnPropertyChanged("CanEditProperties");
                OnDataSheetChanged(NewAppliance.DataSheet);
            }
        }

        public ObservableCollection<string> Types { get; } = new ObservableCollection<string>(ApplianceTypeConverter.AppliancesNames);

        private Appliance _newAppliance;
        public Appliance NewAppliance
        {
            get { return _newAppliance; }
            set { _newAppliance = value; OnPropertyChanged(); }
        }

        private DataSheet _oldDataSheet;
        public DataSheet OldDataSheet
        {
            get { return _oldDataSheet; }
            set { _oldDataSheet = value; OnPropertyChanged(); }
        }

        public bool IsNewAppliance { get; set; }

        private bool _isHeatingOrSolar;
        public bool IsHeatingOrSolar
        {
            get { return _isHeatingOrSolar; }
            set { _isHeatingOrSolar = value; OnPropertyChanged(); }
        }

        private bool _isContainer;
        public bool IsContainer
        {
            get { return _isContainer; }
            set { _isContainer = value; OnPropertyChanged(); }
        }

        public bool IsWaterContainer
        {
            get
            {
                if (IsContainer == false)
                    return false;
                var sheet = NewAppliance.DataSheet as ContainerDataSheet;
                return sheet != null && sheet.IsWaterContainer;
            }
            set
            {
                var sheet = NewAppliance.DataSheet as ContainerDataSheet;
                if (sheet == null)
                    return;
                sheet.IsWaterContainer = value;
                OnPropertyChanged();
            }
        }

        public bool CanEditProperties => _newAppliance.DataSheet != null;

        private ApplianceFactory appFactory;

        public CreateApplianceDialogViewModel(Appliance newAppliance, bool isNewAppliance, Action<CreateApplianceDialogViewModel> closeHandler, 
                                              Action<CreateApplianceDialogViewModel> completionHandler)
        {
            appFactory = new ApplianceFactory();
            NewAppliance = new Appliance();

            CloseCommand = new RelayCommand(x =>
            {
                if (!IsNewAppliance)
                    newAppliance.DataSheet = OldDataSheet;
                closeHandler(this);
            });

            SaveCommand = new RelayCommand(x =>
            {
                completionHandler(this);
            });

            IsNewAppliance = isNewAppliance;
            if (!IsNewAppliance)
            {
                OnPropertyChanged("IsNewAppliance");
                HandleExistingAppliance(newAppliance);
                NewAppliance = newAppliance;
            }

            DataSheetChangedEventHandler += HandleDataSheetChanged;
        }

        private void HandleExistingAppliance(Appliance appliance)
        {
            OldDataSheet = appliance.DataSheet.MakeCopy() as DataSheet;
            OnDataSheetChanged(NewAppliance.DataSheet);
        }

        /* When a new type is chosen in the dialog, switch visibilities */
        private void HandleDataSheetChanged(DataSheet dataSheet)
        {
            if (dataSheet is HeatingUnitDataSheet || dataSheet is SolarCollectorDataSheet)
            {
                IsHeatingOrSolar = true;
                IsContainer = false;
                OnPropertyChanged("IsWaterContainer");
            }
            else if (dataSheet is ContainerDataSheet)
            {
                IsContainer = true;
                IsHeatingOrSolar = false;
                IsWaterContainer = false; /* We don't know this yet, so just default it */
            }
        }
    }
}
