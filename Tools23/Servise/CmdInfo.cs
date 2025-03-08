//https://autolisp.ru/2024/10/29/nanocad-vyvod-komand-s-ix-opisaniem-cherez-net/
//https://adn-cis.org/programmnoe-opredelenie-dublirovannyix-imen-.net-komand.html
using System.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;

#if NC

using Teigha.Runtime;

#elif AC

using Autodesk.AutoCAD.Runtime;

#endif

namespace drz.Infrastructure.CAD.Service
{
    /// <summary>
    /// Информация о командах CAD
    /// </summary>
    public class CmdInfo
    {
        #region Other

        /// <summary>
        /// Gets or sets the map information.
        /// </summary>
        /// <value>
        /// The map information.
        /// </value>
        public Dictionary<string, List<CmdList>> mapInfo { get; set; }

        /// <summary>
        /// Gets or sets the s command information.
        /// </summary>
        /// <value>
        /// The s command information.
        /// </value>
        public string sCmdInfo { get; set; } = "";

        /// <summary>
        /// Gets or sets the s duplicate information.
        /// </summary>
        /// <value>
        /// The s duplicate information.
        /// </value>
        public string sDuplInfo { get; set; } = "";

        /// <summary>
        /// Список зарегистрированных команд
        /// </summary>
        public class CmdList
        {
            /// <summary>
            /// Имя метода
            /// </summary>
            internal string MethodAttr { get; set; }

            /// <summary>
            ///Описание метода
            /// </summary>
            internal string DescriptionAttr { get; set; }

            /// <summary>
            /// Имя класса
            /// </summary>
            internal string MethodInfo { get; set; }
        }

        /// <summary>Сборка содержащая текущий исполняемый код</summary>
        private Assembly asm { get; set; }

        /// <summary>
        /// Вывод MethodInfo /[b method information].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [b method information]; otherwise, <c>false</c>.
        /// </value>
        private bool bMethodInfo { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CmdInfo"/> class.
        /// </summary>
        public CmdInfo(bool _bMethodInfo = false)
        {
            bMethodInfo = _bMethodInfo;
            asm = Assembly.GetExecutingAssembly();
            Reflection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsmInfo"/> class.
        /// </summary>
        /// <param name="_asm">The asm.</param>
        /// <param name="_bMethodInfo">if set to <c>true</c> [b method information].</param>
        public CmdInfo(Assembly _asm, bool _bMethodInfo = false)
        {
            bMethodInfo = _bMethodInfo;
            asm = _asm;
            Reflection();
        }

        #endregion Other

        /// <summary>
        /// Reflections this instance.
        /// </summary>
        private void Reflection()

        {
            mapInfo = new Dictionary<string, List<CmdList>>();

            Type[] expTypes = asm.GetTypes();

            //собираем
            foreach (Type type in expTypes)
            {
                MethodInfo[] methods = type.GetMethods();

                //собираем методы
                foreach (MethodInfo method in methods)
                {
                    CmdList cinf = GetCmdInf(method);
                    if (cinf == null)
                        continue;

                    if (!mapInfo.ContainsKey(cinf.MethodAttr))
                    {
                        var lCinfo = new List<CmdList>();
                        mapInfo.Add(cinf.MethodAttr, lCinfo);
                    }
                    mapInfo[cinf.MethodAttr].Add(cinf);
                }
            }
            string sMethod;

            foreach (KeyValuePair<string, List<CmdList>> keyValuePair in mapInfo)
            {
                if (keyValuePair.Value.Count > 1)
                {
                    if (!string.IsNullOrEmpty(sDuplInfo)) sDuplInfo += "\n";//если дописываем, то перенос

                    sDuplInfo += "Дублированный атрибут: " + keyValuePair.Key;

                    foreach (CmdList itemList in keyValuePair.Value)
                    {
                        sDuplInfo += "\n\t[" + itemList.MethodInfo + "] " + itemList.DescriptionAttr;
                    }
                }
                else
                {
                    if (bMethodInfo)
                    {
                        sMethod = " [" + keyValuePair.Value[0].MethodInfo + "]";
                    }
                    else
                    {
                        sMethod = "";
                    }

                    if (!string.IsNullOrEmpty(sCmdInfo)) sCmdInfo += "\n";//если дописываем, то перенос

                    sCmdInfo += keyValuePair.Key + sMethod + "\t" + keyValuePair.Value[0].DescriptionAttr;
                }
            }
        }

        private CmdList GetCmdInf(MethodInfo method)
        {
            object[] attributes = method.GetCustomAttributes(true);
            CmdList res = new CmdList();

            foreach (object attribute in attributes)
            {
                if (attribute is CommandMethodAttribute cmdAttr)
                {
                    res.MethodAttr = cmdAttr.GlobalName;

                    res.MethodInfo = method.Name;
                }
                else if (attribute is DescriptionAttribute descrAttr)
                {
                    if (descrAttr != null)
                    {
                        res.DescriptionAttr = descrAttr.Description;
                    }
                    else
                    {
                        res.DescriptionAttr = "";
                    }
                }
            }
            //return res;
            return res.MethodAttr == null ? null : res;
        }
    }
}