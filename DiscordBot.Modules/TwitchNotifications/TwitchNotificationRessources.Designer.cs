﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DiscordBot.Modules.TwitchNotifications {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class TwitchNotificationRessources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal TwitchNotificationRessources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DiscordBot.Modules.TwitchNotifications.TwitchNotificationRessources", typeof(TwitchNotificationRessources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Für diesen Streamer sind Benachrichtigungen bereits aktiviert.
        /// </summary>
        internal static string Error_AlreadyRegistered {
            get {
                return ResourceManager.GetString("Error_AlreadyRegistered", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Diese Registrierung existiert nicht.
        /// </summary>
        internal static string Error_RegistrationInvalid {
            get {
                return ResourceManager.GetString("Error_RegistrationInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} hat einen Livestream gestartet.
        /// </summary>
        internal static string Message_NewStream {
            get {
                return ResourceManager.GetString("Message_NewStream", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Registration erfolgreich.
        /// </summary>
        internal static string Message_RegistrationSuccess {
            get {
                return ResourceManager.GetString("Message_RegistrationSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Registration gelöscht.
        /// </summary>
        internal static string Message_UnregistrationSucess {
            get {
                return ResourceManager.GetString("Message_UnregistrationSucess", resourceCulture);
            }
        }
    }
}
