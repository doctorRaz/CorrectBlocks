//https://adn-cis.org/forum/index.php?topic=10047.msg45331#msg45331

using System;
#if NC
using Teigha.DatabaseServices;
#else
using Autodesk.AutoCAD.DatabaseServices;
#endif
namespace Bushman.AutoCAD.DatabaseServices
{
    /// <summary>
    /// Изменяя базу данных чертежей, очень важно контролировать то, какая база данных является текущей. 
    /// Класс <c>WorkingDatabaseSwitcher</c>
    /// берёт на себя контроль над тем, чтобы текущей была именно та база данных, которая нужна.
    /// </summary>
    /// <example>
    /// Пример использования класса:
    /// <code>
    /// //db - объект Database
    /// using (WorkingDatabaseSwitcher hlp = new WorkingDatabaseSwitcher(db)) {
    ///     // тут наш код</code>
    /// }</example>
    public sealed class WorkingDatabaseSwitcher : IDisposable
    {
        private Database prevDb = null;
        /// <summary>
        /// База данных, в контексте которой должна производиться работа. Эта база данных на время становится текущей.
        /// По завершению работы текущей станет та база, которая была ею до этого.
        /// </summary>
        /// <param name="db">База данных, которая должна быть установлена текущей</param>
        public WorkingDatabaseSwitcher(Database db)
        {
            prevDb = HostApplicationServices.WorkingDatabase;
            HostApplicationServices.WorkingDatabase = db;
        }
        /// <summary>
        /// Возвращаем свойству <c>HostApplicationServices.WorkingDatabase</c> прежнее значение
        /// </summary>
        public void Dispose()
        {
            HostApplicationServices.WorkingDatabase = prevDb;
        }
    }
}