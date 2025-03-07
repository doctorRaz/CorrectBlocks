using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
#if NC
using HostMgd.ApplicationServices;
using HostMgd.EditorInput;
using Application = HostMgd.ApplicationServices.Application;
#elif AC
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
#endif

namespace DrzCadTools.Infrastructure
{
    public class MessageService
    {

        public void ErrorMessage(string Message, [CallerMemberName] string MethodName = null)
        {
            MessageBox.Show("Ошибка", string.IsNullOrWhiteSpace(MethodName) ? "" : (MethodName + " \n") + Message,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void InfoMessage(string Message, [CallerMemberName] string MethodName = null)
        {
            MessageBox.Show("Ошибка", string.IsNullOrWhiteSpace(MethodName) ? "" : (MethodName + " \n") + Message,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ExceptionMessage(Exception ex, [CallerMemberName] string MethodName = null)
        {
            MessageBox.Show("Ошибка", string.IsNullOrWhiteSpace(MethodName) ? "" : (MethodName + " \n") + ex.Message + "\n" + ex.StackTrace,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ConsoleMessage(string Message, [CallerMemberName] string MethodName = null)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null)
            {
                InfoMessage(Message, MethodName);
                return;
            }
            Editor ed = doc.Editor;
            ed.WriteMessage("\n" + (string.IsNullOrWhiteSpace(MethodName) ? "" : (MethodName + " : ")) + Message);
        }
    }
}
