using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ControlModifiedFiles
{
    internal static class OtherMethods
    {
        internal static Task StartTimerPause(int seconds = 1)
        {
            return Task.Run(() => Thread.Sleep(seconds * 1000));
        }

        internal static Task StartTimerPause(double seconds)
        {
            return Task.Run(() => Thread.Sleep((int)(seconds * 1000)));
        }

        #region Naruny_extremecode

        public static string[] Substrings(this string str, string left, string right,
                    int startIndex, StringComparison comparsion = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new string[0];
            }

            #region Проверка параметров


            if (left == null)
            {
                throw new ArgumentNullException("left");
            }

            if (left.Length == 0)
            {
                throw new ArgumentException("Bad left lenght");
            }

            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            if (right.Length == 0)
            {
                throw new ArgumentException("Bad right lenght");
            }

            if (startIndex < 0)
            {
                throw new ArgumentException("Start index bad");
            }

            if (startIndex >= str.Length)
            {
                throw new ArgumentException("Start index bad");
            }
            #endregion

            int currentStartIndex = startIndex;
            List<string> strings = new List<string>();

            while (true)
            {
                // Ищем начало позиции левой подстроки.
                int leftPosBegin = str.IndexOf(left, currentStartIndex, comparsion);

                if (leftPosBegin == -1)
                {
                    break;
                }

                // Вычисляем конец позиции левой подстроки.
                int leftPosEnd = leftPosBegin + left.Length;

                // Ищем начало позиции правой строки.
                int rightPos = str.IndexOf(right, leftPosEnd, comparsion);

                if (rightPos == -1)
                {
                    break;
                }

                // Вычисляем длину найденной подстроки.
                int length = rightPos - leftPosEnd;

                strings.Add(str.Substring(leftPosEnd, length));

                // Вычисляем конец позиции правой подстроки.
                currentStartIndex = rightPos + right.Length;
            }

            return strings.ToArray();
        }

        public static string Substring(this string str, string left, string right,
           int startIndex, StringComparison comparsion = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            #region Проверка параметров

            if (left == null)
            {
                throw new ArgumentNullException("left");
            }

            if (left.Length == 0)
            {
                throw new ArgumentException("Bad left lenght");
            }

            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            if (right.Length == 0)
            {
                throw new ArgumentException("Bad right lenght");
            }

            if (startIndex < 0)
            {
                throw new ArgumentException("Start index bad");
            }

            if (startIndex >= str.Length)
            {
                throw new ArgumentException("Start index bad");
            }

            #endregion

            // Ищем начало позиции левой подстроки.
            int leftPosBegin = str.IndexOf(left, startIndex, comparsion);

            if (leftPosBegin == -1)
            {
                return string.Empty;
            }

            // Вычисляем конец позиции левой подстроки.
            int leftPosEnd = leftPosBegin + left.Length;

            // Ищем начало позиции правой подстроки.
            int rightPos = str.IndexOf(right, leftPosEnd, comparsion);

            if (rightPos == -1)
            {
                return string.Empty;
            }

            // Вычисляем длину найденной подстроки.
            int length = rightPos - leftPosEnd;

            return str.Substring(leftPosEnd, length);
        }

        public static string[] Substrings(this string str, string left, string right,
           StringComparison comparsion = StringComparison.Ordinal)
        {
            return str.Substrings(left, right, 0, comparsion);
        }
        public static string Substring(this string str, string left, string right,
           StringComparison comparsion = StringComparison.Ordinal)
        {
            return Substring(str, left, right, 0, comparsion);
        }

        #endregion
    }
}