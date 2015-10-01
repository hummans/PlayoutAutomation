﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using TAS.Client.Common;

namespace TAS.Client.Setup
{
    public class CreateDatabaseViewmodel: OkCancelViewmodelBase<Model.CreateDatabase>
    {
        public CreateDatabaseViewmodel(): base(new Model.CreateDatabase(), new CreateDatabaseView(), "Create database") 
        {
            _commandEditConnectionString = new UICommand() { ExecuteDelegate = _editConnectionString };
        }
        private void _editConnectionString(object obj)
        {
            var vm = new ConnectionStringViewmodel(ConnectionString);
            if (vm.Show() == true)
                ConnectionString = vm.ConnectionString;
        }
        protected override void OnDispose() { }
        string _connectionString;
        public string ConnectionString { get { return _connectionString; } set { SetField(ref _connectionString, value, "ConnectionString"); } }
        string _collation;
        public string Collation { get { return _collation; } set { SetField(ref _collation, value, "Collation"); } }
        readonly UICommand _commandEditConnectionString;
        public ICommand CommandEditConnectionString { get { return _commandEditConnectionString; } }
        public static string[] Collations { get { return TAS.Client.Setup.Model.CreateDatabase.Collations; } }
        protected override void Ok(object o)
        {
            UiServices.SetBusyState();
            Save(null);
            View.DialogResult = Model.CreateEmptyDatabase();
        }
    }
}
