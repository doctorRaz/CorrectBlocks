using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using drzTools.Abstractions.Interfaces;

using dRzTools;
using dRzTools.Servise;

//using DrzCadTools.Infrastructure;
//using drzTools.ListCmdInfo;

namespace drzTools.Servise
{
     class ListCmdInfo
    {
        #region LIST CMD

        /// <summary>
        /// Lists the command.
        /// </summary>
        public static void ListCMD()
        {
            bool bMethod = false;//не выводить имена методов
#if DEBUG
            bMethod = true;
#endif
            //выводим список команд с описаниями
            CmdInfo CDI = new CmdInfo(Assembly.GetExecutingAssembly(), bMethod);//эта сборка вывод имен классов

            string sTitleAttribute = SysInfo.sTitleAttribute;

            string sVersion = SysInfo.sVersionFull;

            string sDateRliz = SysInfo.sDateRelies;

            IAllMessageService msgService = new MessageService();
            msgService.ConsoleMessage(sTitleAttribute + ": v." + sVersion + " от " + sDateRliz+"\n");

            if (!string.IsNullOrWhiteSpace(CDI.sCmdInfo))
            {
                msgService.ConsoleMessage(CDI.sCmdInfo);
            }
            else
            {
                msgService.ConsoleMessage("Нет зарегистрированных команд");
            }

            //дубликаты команд
            if (!string.IsNullOrEmpty(CDI.sDuplInfo))
            {
                msgService.ConsoleMessage(CDI.sDuplInfo);
            }
        }
        #endregion
    }
}
