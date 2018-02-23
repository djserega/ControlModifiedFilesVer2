using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ControlModifiedFiles
{
    internal static class Dialog
    {
        internal static bool DialogQuestion(string textQuestion)
        {
            return MessageBox.Show(textQuestion, "Вопрос", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK;
        }

        internal static void ShowMessage(string textMessage, int timer = 10)
        {
            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                Message form = new Message(textMessage, timer);
                form.Show();
                if (timer > 0)
                {
                    await Task.Delay(timer * 1000);
                    form.Close();
                }
            });
        }
    }
}
