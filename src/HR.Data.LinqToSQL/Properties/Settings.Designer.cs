﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HR.Data.LinqToSQL.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.0.1.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=C:\\USERS\\AGHYAD\\SOURCE\\REPOS\\C" +
            "ODDEE\\HR.WEB\\DB\\HRDATABASE.MDF;Integrated Security=True;Connect Timeout=30;Encry" +
            "pt=False;TrustServerCertificate=True")]
        public string C__USERS_AGHYAD_SOURCE_REPOS_CODDEE_HR_WEB_DB_HRDATABASE_MDFConnectionString {
            get {
                return ((string)(this["C__USERS_AGHYAD_SOURCE_REPOS_CODDEE_HR_WEB_DB_HRDATABASE_MDFConnectionString"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Aghyad\\Source\\Repos\\" +
            "Coddee\\HR.Web\\DB\\HRDatabase.mdf;Integrated Security=True;Connect Timeout=30")]
        public string HRDatabaseConnectionString {
            get {
                return ((string)(this["HRDatabaseConnectionString"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\HRDatabase.md" +
            "f;Integrated Security=True;Connect Timeout=30")]
        public string HRDatabaseConnectionString1 {
            get {
                return ((string)(this["HRDatabaseConnectionString1"]));
            }
        }
    }
}
