﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DiscordBot.Modules.MusicPlayer {
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
    internal class MusicPlayerRessources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MusicPlayerRessources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DiscordBot.Modules.MusicPlayer.MusicPlayerRessources", typeof(MusicPlayerRessources).Assembly);
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
        ///   Looks up a localized string similar to Diese Seite existiert nicht.
        /// </summary>
        internal static string Error_InvlaidPage {
            get {
                return ResourceManager.GetString("Error_InvlaidPage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Du musst in einem Sprachkanal sein.
        /// </summary>
        internal static string Error_MustBeInVoice {
            get {
                return ResourceManager.GetString("Error_MustBeInVoice", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Aktuell spielt keine Musik.
        /// </summary>
        internal static string Error_NoMusic {
            get {
                return ResourceManager.GetString("Error_NoMusic", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Playlist nicht gefunden. Bitte ID verwenden..
        /// </summary>
        internal static string Error_PlaylistNotFound {
            get {
                return ResourceManager.GetString("Error_PlaylistNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Die Playlist &apos;{0}&apos; gehört dir nicht.
        /// </summary>
        internal static string Error_PlaylistNotOwned {
            get {
                return ResourceManager.GetString("Error_PlaylistNotOwned", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kein Song mit dem Namen &apos;{0}&apos; gefunden.
        /// </summary>
        internal static string Error_SongNotFound {
            get {
                return ResourceManager.GetString("Error_SongNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Die Spotify Playlist konnte nicht geladen werden. Bitte stelle sicher, dass der Link stimmt und die Playlist öffentlich einsehbar ist..
        /// </summary>
        internal static string Error_SpotifyPlaylistNotParsable {
            get {
                return ResourceManager.GetString("Error_SpotifyPlaylistNotParsable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Du hast bereits die maximale Anzahl an Playlists erstellt. Bitte bestehende löschen..
        /// </summary>
        internal static string Error_TooManyPlaylists {
            get {
                return ResourceManager.GetString("Error_TooManyPlaylists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}: {1} (von {2})
        ///.
        /// </summary>
        internal static string Line_Playlist {
            get {
                return ResourceManager.GetString("Line_Playlist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Playlist erstellt.
        /// </summary>
        internal static string Message_PlaylistCreated {
            get {
                return ResourceManager.GetString("Message_PlaylistCreated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Playlist &quot;{0}&quot; geladen ({1}/{2} erfolgreich).
        /// </summary>
        internal static string Message_PlaylistLoaded {
            get {
                return ResourceManager.GetString("Message_PlaylistLoaded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Lade Playlist &apos;{0}&apos; von Spotify... ({1} Tracks).
        /// </summary>
        internal static string Status_PlaylistLoading {
            get {
                return ResourceManager.GetString("Status_PlaylistLoading", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Playlists auf {0} (Seite {1}).
        /// </summary>
        internal static string Title_Playlists {
            get {
                return ResourceManager.GetString("Title_Playlists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} Songs in der Warteschlange (Seite {1}/{2}).
        /// </summary>
        internal static string Title_QueueTitle {
            get {
                return ResourceManager.GetString("Title_QueueTitle", resourceCulture);
            }
        }
    }
}
