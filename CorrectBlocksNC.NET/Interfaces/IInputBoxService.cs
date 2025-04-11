namespace drzTools.Abstractions.Interfaces
{
    public interface IInputBoxService
    {
        string GetTextByInputBox(string Message, string Title = null, string DefaultValue = null);

    }
}
