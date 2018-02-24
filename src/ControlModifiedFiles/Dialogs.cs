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
        internal static void ShowMessage(string textMessage, int timer = 10)
            => new Message(textMessage, timer).Show();

        internal static bool? ShowQuesttion(string textMessage, int timer = 10)
        {
            bool? resutQuestion = null;

            Message form = new Message(textMessage, timer, true);
            form.ShowDialog();
            if (form.PressButtonOK)
                resutQuestion = true;
            else if (form.PressButtonCancel)
                resutQuestion = false;

            return resutQuestion;
        }
    }
}
