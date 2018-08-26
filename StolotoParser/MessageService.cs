using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StolotoParser
{
    public interface IMessageService
    {
        void ShowError(string error);
        void ShowExclamation(string exlamation);
        void ShowMessage(string message);
    }

    public class MessageService : IMessageService
    {
        void IMessageService.ShowExclamation(string exlamation)
        {
            MessageBox.Show(exlamation, "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        void IMessageService.ShowError(string error)
        {
            MessageBox.Show(error, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        void IMessageService.ShowMessage(string message)
        {
            MessageBox.Show(message, "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
