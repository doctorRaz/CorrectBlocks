using System;
using System.Windows.Forms;

using Microsoft.Win32;

using System.IO;



#if NC
using Teigha.DatabaseServices;

using HostMgd.ApplicationServices;

using App = HostMgd.ApplicationServices;
using Ed = HostMgd.EditorInput;
using Rtm = Teigha.Runtime;
#elif AC

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using App = Autodesk.AutoCAD.ApplicationServices;
using Cad = Autodesk.AutoCAD.ApplicationServices.Application;
using Db = Autodesk.AutoCAD.DatabaseServices;
using Gem = Autodesk.AutoCAD.Geometry;
using Ed = Autodesk.AutoCAD.EditorInput;
using Rtm = Autodesk.AutoCAD.Runtime;

#endif

namespace dRzTools.Settings
{
    /// <summary>Служебные утилиты</summary>
    class McUtilServise
    {
        #region Registry 
        // think обернуть в using
        internal class RegistryMod
        {
            internal RegistryMod()
            {
                RegistryKey curUserKey = Registry.CurrentUser;
#if NC
                startupKey = curUserKey.OpenSubKey(HostApplicationServices.Current.RegistryProductRootKey);
#else
                startupKey = curUserKey.OpenSubKey(HostApplicationServices.Current.UserRegistryProductRootKey);
#endif
                profilKey = startupKey.OpenSubKey(Path.Combine(profiles, sActiveProfile));
            }

            //Val
            RegistryKey InitDirKey;
            internal object objSaveInitDir
            {
                get
                {
                    InitDirKey = profilKey.OpenSubKey(ioSaveProjects);
                    return InitDirKey.GetValue(propSaveInitDir);
                }
                set
                {
                    InitDirKey = profilKey.OpenSubKey(ioSaveProjects, true);
                    InitDirKey.SetValue(propSaveInitDir, value);
                    InitDirKey.Close();
                }

            }

            internal object objOpenInitDir
            {
                get
                {
                    InitDirKey = profilKey.OpenSubKey(ioAllOpenFileFormats);
                    return InitDirKey.GetValue(propOpenInitDir);
                }
                set
                {
                    InitDirKey = profilKey.OpenSubKey(ioAllOpenFileFormats, true);
                    InitDirKey.SetValue(propOpenInitDir, value);
                }
            }

            internal object objUserCfgDir => startupKey.GetValue(propUserDataDir);

            //KEY
            RegistryKey startupKey { get; }
            RegistryKey profilKey { get; }

            //Const

            const string profiles = "Profiles";

            const string ioSaveProjects = @"IO\SaveProjects";

            const string ioAllOpenFileFormats = @"IO\AllOpenFileFormats";


            static readonly string sActiveProfile = App.Application.GetSystemVariable("CCONFIGURATION").ToString();

            //---
            //PROP
            const string propSaveInitDir = "SaveInitDir";

            const string propOpenInitDir = "OpenInitDir";

            const string propUserDataDir = "UserDataDir";

        }

        /// <summary>
        /// Gets the initialize dir.
        /// <br>HKEY_CURRENT_USER\SOFTWARE\Nanosoft\nanoCAD x64\23.1\Profiles\<Profile></Profile>\IO\</br>
        /// </summary>
        /// <param name="sInitDir">The s initialize dir.
        /// <br>SaveInitDir</br>
        /// <br>OpenInitDir</br>
        /// </param>
        /// <returns>Path</returns>
        internal static string GetInitDir(string sInitDir)
        {
            //диалог save
            // HKEY_CURRENT_USER\SOFTWARE\Nanosoft\nanoCAD x64\23.1\Profiles\SPDS\IO\SaveProjects SaveInitDir
            //опен
            // HKEY_CURRENT_USER\SOFTWARE\Nanosoft\nanoCAD x64\23.1\Profiles\SPDS\IO\AllOpenFileFormats OpenInitDir

            /*
            //Database db = HostApplicationServices.WorkingDatabase;
            //Document doc = App.Application.DocumentManager.MdiActiveDocument;
            //Editor ed = doc.Editor;
            //dynamic comDoc = doc.AcadDocument;
            //dynamic sActiveProfile = comDoc.Application.Preferences.Profiles.ActiveProfile;
            //var ppp2 = App.Application.GetSystemVariable("CPROFILE");
            */

            //профиль нанокад SPDS Mex Def
            string sActiveProfile = App.Application.GetSystemVariable("CCONFIGURATION").ToString();

            RegistryKey curUserKey = Registry.CurrentUser;

#if NC
            RegistryKey startupKey = curUserKey.OpenSubKey(HostApplicationServices.Current.RegistryProductRootKey);
#else
            RegistryKey startupKey = curUserKey.OpenSubKey(HostApplicationServices.Current.UserRegistryProductRootKey);
#endif
            RegistryKey initDirKey = startupKey.OpenSubKey(@"Profiles\"
                                                        + sActiveProfile
                                                        + @"\IO\SaveProjects"
                                                        );

            string sSaveInitDir = initDirKey.GetValue(sInitDir).ToString();

            startupKey.Close();
            initDirKey.Close();

            return sSaveInitDir;
        }



        #endregion

        #region  СЛУЖЕБНЫЕ

        /// <summary> Расшифровка исключений </summary>
        /// <param name="ex">Исключение</param>
        /// <param name="sResp">Доп текст</param>
        /// <param name="msgIco">Иконка сообщения</param>
        /// <param name="bMsg">Показывыть MsgBox</param>
        /// <param name="sTitle">Титул сообщения</param>
        /// <returns>Расшифровка исключения</returns>
        public static string InterpretationExeption(Exception ex,
                                                    string sResp = "Работа программы не может быть продолжена!",
                                                    MessageBoxIcon msgIco = MessageBoxIcon.Warning,
                                                    bool bMsg = false,
                                                    string sTitle = "drzTools")
        // by razygraevaa on 02.03.2023 at 14:04 добавить сюда логгер
        {
            string sAnswer = "\nОшибка в работе программы"
                    + "\n"
                    + sResp
                    + "\n\nТехническое описание: "
                    + ex.Message
                    + "\n\nStackTrace: "
                    + ex.StackTrace;

            if (bMsg)
            {
                MessageBox.Show(
                                              sAnswer,
                                               sTitle,
                                               MessageBoxButtons.OK,
                                               msgIco);
            }
            return sAnswer;
        }


        #endregion

    }
}
