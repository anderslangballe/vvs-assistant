﻿using System;
using VVSAssistant.Common;
using VVSAssistant.Common.ViewModels;

namespace VVSAssistant.Controls.Dialogs.ViewModels
{
    public class SaveDialogViewModel : NotifyPropertyChanged
    {
        public RelayCommand CloseCommand { get; }
        public RelayCommand SaveCommand { get; }

        private string _input;
        public string Input
        {
            get { return _input; }
            set
            {
                SetProperty(ref _input, value);
                SaveCommand.NotifyCanExecuteChanged();
            }
        }

        public SaveDialogViewModel(Action<SaveDialogViewModel> closeHandler, Action<SaveDialogViewModel> completionHandler)
        {
            SaveCommand = new RelayCommand(x =>
            {
                completionHandler(this);
            }, x => !string.IsNullOrEmpty(Input));


            CloseCommand = new RelayCommand(x =>
            {
                closeHandler(this);
            });
        }
    }
}
