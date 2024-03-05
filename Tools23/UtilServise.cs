using System;
using System.Collections.Generic;
using System.Windows.Forms;

using System.Reflection;
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

namespace DrzCadTools
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
                profilKey = startupKey.OpenSubKey(Path.Combine( profiles, sActiveProfile));
            }

            //Val
            RegistryKey InitDirKey;
            internal Object objSaveInitDir
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

            internal Object objOpenInitDir
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

            internal Object objUserCfgDir => startupKey.GetValue(propUserDataDir);

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
        public static string InterpretationExeption(System.Exception ex,
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
                System.Windows.Forms.MessageBox.Show(
                                              sAnswer,
                                               sTitle,
                                               MessageBoxButtons.OK,
                                               msgIco);
            }
            return sAnswer;
        }



        #region  ПОЛУЧЕНИЕ КОМАНД

        #region от Ривилиса
#if NC || AC
        /// <summary>
        /// Получение списка команд
        /// https://adn-cis.org/programmnoe-opredelenie-dublirovannyix-imen-.net-komand.html
        /// </summary>
        public static void FindCmdDuplicates()
        {
            Dictionary<string, List<MethodInfo>> map =
                new Dictionary<string, List<MethodInfo>>();

            // razygraevaa on 23.09.2022 at 11:37   Assembly asm = Assembly.LoadFile(asmPath);
            Assembly asm = Assembly.GetExecutingAssembly();
            Type[] expTypes = asm.GetTypes();

            foreach (Type type in expTypes)
            {
                MethodInfo[] methods = type.GetMethods();

                foreach (MethodInfo method in methods)
                {
                    Rtm.CommandMethodAttribute attribute =
                          GetCommandMethodAttribute(method);

                    if (attribute == null)
                        continue;

                    if (!map.ContainsKey(attribute.GlobalName))
                    {
                        var methodInfo = new List<MethodInfo>();

                        map.Add(attribute.GlobalName, methodInfo);
                    }

                    map[attribute.GlobalName].Add(method);
                }
            }

            App.Document doc = App.Application.DocumentManager.MdiActiveDocument;
            Ed.Editor ed = doc.Editor;

            foreach (var keyValuePair in map)
            {
                // razygraevaa on 23.09.2022 at 11:38    if (keyValuePair.Value.Count > 1)
                string sDuplicate;//= string.Empty;
                string sDuplicateTab = string.Empty;
                if (keyValuePair.Value.Count > 1)
                {
                    sDuplicate = "\n****************************\tДубль ********\n\t";
                    sDuplicateTab = "\t";
                }
                else
                {
                    sDuplicate = "\n";
                    //sDuplicateTab = "\n\t";
                }
                // razygraevaa on 23.09.2022 at 11:39      "\nДублированный атрибут: " + keyValuePair.Key);
                ed.WriteMessage(
                                sDuplicate + "Команда- " + keyValuePair.Key
                                );

                foreach (var method in keyValuePair.Value)
                {
                    ed.WriteMessage(
                                    sDuplicateTab + "= Метод: " + method.Name
                                    );
                }
                if (keyValuePair.Value.Count > 1)
                {
                    ed.WriteMessage("\n************************");
                }
            }
        }

        /// <summary>
        /// Методы и команды
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static Rtm.CommandMethodAttribute GetCommandMethodAttribute(
                MethodInfo method)
        {
            object[] attributes = method.GetCustomAttributes(true);

            foreach (object attribute in attributes)
            {
                if (attribute is Rtm.CommandMethodAttribute)
                {
                    return attribute as Rtm.CommandMethodAttribute;
                }
            }

            return null;
        }
#endif
#endregion

        #endregion


        #endregion
        
    }
}
