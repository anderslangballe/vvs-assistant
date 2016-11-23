﻿using System;
using System.Threading;
using VVSAssistant.ViewModels.MVVM;
using MahApps.Metro.Controls.Dialogs;
using VVSAssistant.ViewModels;
using System.ComponentModel;
using VVSAssistant.Common;
using VVSAssistant.Common.ViewModels;
using VVSAssistant.Models;

namespace VVSAssistant.Controls.Dialogs.ViewModels
{
    public class EditApplianceViewModel : NotifyPropertyChanged
    {
        public RelayCommand CloseCommand { get; }
        public RelayCommand SaveCommand { get; }

        public Appliance Appliance { get; }
        
        public EditApplianceViewModel(Appliance appliance, Action<EditApplianceViewModel> closeHandler, Action<EditApplianceViewModel> completionHandler)
        {
            Appliance = appliance;

            SaveCommand = new RelayCommand(x =>
            {
                completionHandler(this);
            });


            CloseCommand = new RelayCommand(x =>
            {
                closeHandler(this);
            });
        }
    }
}
