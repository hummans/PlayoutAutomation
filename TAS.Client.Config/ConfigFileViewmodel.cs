﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using TAS.Client.Common;
using TAS.Server.Common;
using TAS.Server.Database;

namespace TAS.Client.Config
{
    public class ConfigFileViewmodel : OkCancelViewmodelBase<Model.ConfigFile>
    {

        protected override void OnDispose() { }
        public ConfigFileViewmodel(Model.ConfigFile configFile)
            : base(configFile, new ConfigFileView(), $"Config file ({configFile.FileName})")
        {
            _commandEditConnectionString = new UICommand() { ExecuteDelegate = _editConnectionString };
            _commandEditConnectionStringSecondary = new UICommand() { ExecuteDelegate = _editConnectionStringSecondary };
            _commandTestConnectivity = new UICommand() { ExecuteDelegate = _testConnectivity, CanExecuteDelegate = o => !string.IsNullOrWhiteSpace(tasConnectionString) };
            _commandTestConnectivitySecodary = new UICommand { ExecuteDelegate = _testConnectivitySecondary, CanExecuteDelegate = o => !string.IsNullOrWhiteSpace(tasConnectionStringSecondary) && _isConnectionStringSecondary };
            _commandCreateDatabase = new UICommand() { ExecuteDelegate = _createDatabase, CanExecuteDelegate = o => !string.IsNullOrWhiteSpace(tasConnectionString) };
            _commandCloneDatabase = new UICommand() { ExecuteDelegate = _clonePrimaryDatabase, CanExecuteDelegate = o => !(string.IsNullOrWhiteSpace(tasConnectionString) || string.IsNullOrWhiteSpace(tasConnectionStringSecondary)) };
        }

        private void _createDatabase(object obj)
        {
            var vm = new CreateDatabaseViewmodel();
            vm.ConnectionString = this.tasConnectionString;
            if (vm.ShowDialog() == true)
                if (vm.ConnectionString == this.tasConnectionString)
                    vm.ShowMessage("Database created successfully", "Create database", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    if (vm.ShowMessage("Database created successfully. Use the new database?", "Create database", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    this.tasConnectionString = vm.ConnectionString;
        }

        protected override void ModelLoad(object source)
        {
            base.ModelLoad(Model.appSettings);
            base.ModelLoad(Model.connectionStrings);
            _isConnectionStringSecondary = !string.IsNullOrWhiteSpace(tasConnectionStringSecondary);
        }

        public override void ModelUpdate(object destObject)
        {
            base.ModelUpdate(Model.appSettings);
            if (!_isConnectionStringSecondary)
                tasConnectionStringSecondary = string.Empty;
            base.ModelUpdate(Model.connectionStrings);
            Model.Save();
        }

        readonly UICommand _commandEditConnectionString;
        public ICommand CommandEditConnectionString { get { return _commandEditConnectionString; } }
        readonly UICommand _commandEditConnectionStringSecondary;
        public ICommand CommandEditConnectionStringSecondary { get { return _commandEditConnectionStringSecondary; } }
        readonly UICommand _commandTestConnectivity;
        public ICommand CommandTestConnectivity { get { return _commandTestConnectivity; } }
        readonly UICommand _commandCreateDatabase;
        public ICommand CommandCreateDatabase { get { return _commandCreateDatabase; } }
        readonly UICommand _commandCloneDatabase;
        public ICommand CommandCloneDatabase { get { return _commandCloneDatabase; } }
        readonly UICommand _commandTestConnectivitySecodary;
        public ICommand CommandTestConnectivitySecodary { get { return _commandTestConnectivitySecodary; } }
        readonly List<CultureInfo> _supportedLanguages = new List<CultureInfo>{ CultureInfo.InvariantCulture, new CultureInfo("en"), new CultureInfo("pl")};
        public List<CultureInfo> SupportedLanguages { get { return _supportedLanguages; } }

        private void _editConnectionString(object obj)
        {
            var vm = new ConnectionStringViewmodel(_tasConnectionString);
            if (vm.ShowDialog() == true)
                tasConnectionString = vm.ConnectionString;
        }

        private void _editConnectionStringSecondary(object obj)
        {
            var vm = new ConnectionStringViewmodel(_tasConnectionStringSecondary);
            if (vm.ShowDialog() == true)
                tasConnectionStringSecondary = vm.ConnectionString;
        }

        private void _testConnectivity(object obj)
        {
            if (Database.TestConnect(tasConnectionString))
            {
                Database.Open(tasConnectionString, tasConnectionStringSecondary);
                if (Database.UpdateRequired())
                {
                    if (ShowMessage("Connection successful, but database should be updated. \nUpdate now?", "Connection test", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        if (Database.UpdateDB())
                            ShowMessage("Database is now up-to-date.", "Connection test", MessageBoxButton.OK, MessageBoxImage.Information);
                        else 
                            ShowMessage("Database update failed.", "Connection test", MessageBoxButton.OK, MessageBoxImage.Error);

                }
                else
                    ShowMessage("Connection successful and database is up-to-date.", "Connection test", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                ShowMessage("Connection failed", "Connection test", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void _testConnectivitySecondary(object obj)
        {
            if (Database.TestConnect(tasConnectionStringSecondary))
                ShowMessage("Connection successful", "Connection test", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                ShowMessage("Connection failed", "Connection test", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void _clonePrimaryDatabase(object obj)
        {
            if (Database.TestConnect(tasConnectionStringSecondary))
            {
                if (ShowMessage("Secondary database already exists. Delete it first?", "Warning - database exists", MessageBoxButton.YesNo, MessageBoxImage.Hand) != MessageBoxResult.Yes)
                    return;
                if (!Database.DropDatabase(tasConnectionStringSecondary))
                {
                    ShowMessage("Database delete failed, cannot proceed.", "Database clone", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            if (Database.CloneDatabase(tasConnectionString, tasConnectionStringSecondary))
                ShowMessage("Database clone successful", "Database clone", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                ShowMessage("Database clonning failed", "Database clone", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        string _ingestFolders;
        public string IngestFolders { get { return _ingestFolders; } set { SetField(ref _ingestFolders, value); } }
        string _localDevices;
        public string LocalDevices { get { return _localDevices; } set { SetField(ref _localDevices, value); } }
        string _tempDirectory;
        public string TempDirectory { get { return _tempDirectory; } set { SetField(ref _tempDirectory, value); } }
        int _instance;
        public int Instance { get { return _instance; } set { SetField(ref _instance, value); } }
        string _tasConnectionString;
        public string tasConnectionString { get { return _tasConnectionString; } set { SetField(ref _tasConnectionString, value); } }
        string _tasConnectionStringSecondary;
        bool _isBackupInstance;
        public bool IsBackupInstance { get { return _isBackupInstance; } set { SetField(ref _isBackupInstance, value); } }

        public string tasConnectionStringSecondary { get { return _tasConnectionStringSecondary; } set { SetField(ref _tasConnectionStringSecondary, value); } }
        private bool _isConnectionStringSecondary;
        public bool IsSConnectionStringSecondary { get { return _isConnectionStringSecondary; } set { SetField(ref _isConnectionStringSecondary, value); } }
        string _uiLanguage;
        public string UiLanguage { get { return _uiLanguage; } set { SetField(ref _uiLanguage, value); } }
        
        public string ExeDirectory { get { return Path.GetDirectoryName(Model.FileName); } }

    }
}
