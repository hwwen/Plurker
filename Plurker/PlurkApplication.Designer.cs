﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:2.0.50727.3053
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Plurker {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0")]
    internal sealed partial class PlurkApplication : global::System.Configuration.ApplicationSettingsBase {
        
        private static PlurkApplication defaultInstance = ((PlurkApplication)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new PlurkApplication())));
        
        public static PlurkApplication Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string PlurkAPIKey {
            get {
                return ((string)(this["PlurkAPIKey"]));
            }
            set {
                this["PlurkAPIKey"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string PlurkUserName {
            get {
                return ((string)(this["PlurkUserName"]));
            }
            set {
                this["PlurkUserName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string PlurkPassword {
            get {
                return ((string)(this["PlurkPassword"]));
            }
            set {
                this["PlurkPassword"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("60000")]
        public int TimerBootMSec {
            get {
                return ((int)(this["TimerBootMSec"]));
            }
            set {
                this["TimerBootMSec"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Maintain {
            get {
                return ((bool)(this["Maintain"]));
            }
            set {
                this["Maintain"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool TurnOff {
            get {
                return ((bool)(this["TurnOff"]));
            }
            set {
                this["TurnOff"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Approve {
            get {
                return ((bool)(this["Approve"]));
            }
            set {
                this["Approve"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string Message {
            get {
                return ((string)(this["Message"]));
            }
            set {
                this["Message"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("D:\\Plurker\\AssemblyDLL\\stockData.xml")]
        public string stockDataPath {
            get {
                return ((string)(this["stockDataPath"]));
            }
            set {
                this["stockDataPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("D:\\Plurker\\AssemblyDLL\\fifthData.xml")]
        public string fifthDataPath {
            get {
                return ((string)(this["fifthDataPath"]));
            }
            set {
                this["fifthDataPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("D:\\Plurker\\AssemblyDLL\\food.xml")]
        public string foodDataPath {
            get {
                return ((string)(this["foodDataPath"]));
            }
            set {
                this["foodDataPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AutoSayHello {
            get {
                return ((bool)(this["AutoSayHello"]));
            }
            set {
                this["AutoSayHello"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AutoStock {
            get {
                return ((bool)(this["AutoStock"]));
            }
            set {
                this["AutoStock"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool AutoFifth {
            get {
                return ((bool)(this["AutoFifth"]));
            }
            set {
                this["AutoFifth"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AutoAddFriend {
            get {
                return ((bool)(this["AutoAddFriend"]));
            }
            set {
                this["AutoAddFriend"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AutoTeach {
            get {
                return ((bool)(this["AutoTeach"]));
            }
            set {
                this["AutoTeach"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool DebugMode {
            get {
                return ((bool)(this["DebugMode"]));
            }
            set {
                this["DebugMode"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AutoFaceIt {
            get {
                return ((bool)(this["AutoFaceIt"]));
            }
            set {
                this["AutoFaceIt"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://localhost/wsf/")]
        public string FaceItService {
            get {
                return ((string)(this["FaceItService"]));
            }
            set {
                this["FaceItService"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AutoExrate {
            get {
                return ((bool)(this["AutoExrate"]));
            }
            set {
                this["AutoExrate"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AutoFoodSuggest {
            get {
                return ((bool)(this["AutoFoodSuggest"]));
            }
            set {
                this["AutoFoodSuggest"] = value;
            }
        }
    }
}
