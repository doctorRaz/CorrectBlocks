using System.Runtime.CompilerServices;
using System.Diagnostics;
using System;

namespace drzTools.Abstractions.Interfaces
{
    public interface IMessageService
    {
        /// <summary> Вывод сообщения в консоль </summary>
        /// <param name="Message">Выводимое сообщение</param>
        /// <param name="CallerName">Вызывающий метод. При использовании обязательно использование <code>[CallerMemberName]</code></param>
        void ConsoleMessage(string Message, string Title = null, [CallerMemberName] string CallerName = null);
        /// <summary> Вывод информационного сообщения </summary>
        /// <param name="Message">Выводимое сообщение</param>
        /// <param name="CallerName">Вызывающий метод. При использовании обязательно использование <code>[CallerMemberName]</code></param>
        void InfoMessage(string Message, string Title = null, [CallerMemberName] string CallerName = null);
        /// <summary> Вывод информационного сообщения </summary>
        /// <param name="Message">Выводимое сообщение</param>
        /// <param name="CallerName">Вызывающий метод. При использовании обязательно использование <code>[CallerMemberName]</code></param>
        void WarningMessage(string Message, string Title = null, [CallerMemberName] string CallMethodName = null);
        /// <summary> Вывод сообщения об ошибке </summary>
        /// <param name="Message">Выводимое сообщение</param>
        /// <param name="CallerName">Вызывающий метод. При использовании обязательно использование <code>[CallerMemberName]</code></param>
        void ErrorMessage(string Message, string Title = null, [CallerMemberName] string CallerName = null);
        /// <summary> Вывод сообщения об исключении </summary>
        /// <param name="Ex">Обрабатываемое исключение</param>
        /// <param name="CallerName">Вызывающий метод. При использовании обязательно использование <code>[CallerMemberName]</code></param>
        void ExceptionMessage(Exception Ex, string Title = null, [CallerMemberName] string CallerName = null);
    }

}
