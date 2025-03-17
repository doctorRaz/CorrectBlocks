using System.Runtime.CompilerServices;


namespace drzTools.Abstractions.Interfaces
{
    public interface IQuestionService
    {
        /// <summary>
        /// Запрос "ДА" - "НЕТ"
        /// </summary>
        /// <param name="Message">Выводимое сообщение</param>
        /// <param name="Title">Заголовок</param>
        /// <param name="DefaultResult">Ответ по умолчанию</param>
        /// <returns>WindowResult</returns>
        WindowResult QuestionYesNo(string Message, string Title = null, WindowResult DefaultResult = WindowResult.Yes);

        /// <summary>
        /// Запрос "ДА" - "НЕТ" - "ОТМЕНА"
        /// </summary>
        /// <param name="Message">Выводимое сообщение</param>
        /// <param name="Title">Заголовок</param>
        /// <param name="DefaultResult">Ответ по умолчанию</param>
        /// <returns>WindowResult</returns>
        WindowResult QuestionYesNoCancel(string Message, string Title = null, WindowResult DefaultResult = WindowResult.Yes);

        /// <summary>
        /// Запрос "ДА" - "ОТМЕНА"
        /// </summary>
        /// <param name="Message">Выводимое сообщение</param>
        /// <param name="Title">Заголовок</param>
        /// <param name="DefaultResult">Ответ по умолчанию</param>
        /// <returns>WindowResult</returns>
        WindowResult QuestionOKCancel(string Message, string Title = null, WindowResult DefaultResult = WindowResult.OK);


    }

    /// <summary>
    /// Возвращаемое значение
    /// </summary>
    public enum WindowResult
    {
        /// <summary>
        /// The none
        /// </summary>
        None,
        /// <summary>
        /// The ok
        /// </summary>
        OK,
        /// <summary>
        /// The cancel
        /// </summary>
        Cancel,
        /// <summary>
        /// The yes
        /// </summary>
        Yes,
        /// <summary>
        /// The no
        /// </summary>
        No,
    }
}
