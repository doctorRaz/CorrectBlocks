using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using drz.Abstractions.Interfaces;
using drz.Infrastructure.CAD.Service;

//using DrzCadTools.Infrastructure;
using drz.NCAD.Infrastructure;

namespace DrzCadTools
{
   partial class CadCommand
    {
        #region LIST CMD

        /// <summary>
        /// Lists the command.
        /// </summary>
        public void ListCMD()
        {
            bool bMethod = false;//не выводить имена методов
#if DEBUG
            bMethod = true;
#endif
            //выводим список команд с описаниями
            CmdInfo CDI = new CmdInfo(Assembly.GetExecutingAssembly(), bMethod);//эта сборка вывод имен классов

            AsmInfo AI = new AsmInfo(Assembly.GetExecutingAssembly());


            string sTitleAttribute = AI.sTitleAttribute;
            string sVersion = AI.sVersionFull;
            string sDateRliz = AI.sDateRelies;

            IAllMessageService msgService = new MessageService();
            msgService.ConsoleMessage(sTitleAttribute + ": v." + sVersion + " от " + sDateRliz);

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
