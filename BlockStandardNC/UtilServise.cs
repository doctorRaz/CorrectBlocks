using System;
using System.Collections.Generic;
using System.Windows.Forms;

using System.Reflection;

#if NC
using App = HostMgd.ApplicationServices;
using Ed = HostMgd.EditorInput;
using Rtm = Teigha.Runtime;
#elif AC

using Autodesk.AutoCAD.ApplicationServices;
using App = Autodesk.AutoCAD.ApplicationServices;
using Cad = Autodesk.AutoCAD.ApplicationServices.Application;
using Db = Autodesk.AutoCAD.DatabaseServices;
using Gem = Autodesk.AutoCAD.Geometry;
using Ed = Autodesk.AutoCAD.EditorInput;
using Rtm = Autodesk.AutoCAD.Runtime;

#endif

namespace drz.Tools
{
    /// <summary>Служебные утилиты</summary>
    class McUtilServise
    {


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
