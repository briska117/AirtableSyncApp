using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navmii.AirtableSync
{
    public class Logger
    {
        public static string AssemblyDir
        {
            get
            {
                return Path.GetDirectoryName(typeof(Logger).Assembly.Location);
            }
        }

        public static void WriteToDir(string folderPath, string text, params object[] args)
        {
            WriteToDir(true, folderPath, text, args);
        }

        private static void WriteToDir(bool toConsole, string folderPath, string text, params object[] args)
        {
            try
            {
                string logFolder = string.Format("{0}\\Logs", folderPath);
                if (!Directory.Exists(logFolder))
                    Directory.CreateDirectory(logFolder);

                string logPath = string.Format("{0}\\Log{1:yyyyMMdd}.txt", logFolder, DateTime.UtcNow);
                using (StreamWriter writer = new StreamWriter(logPath, true))
                {
                    string st = string.Format("[{0:HH:mm:ss}] {1}", DateTime.UtcNow, string.Format(text, args));
                    writer.WriteLine(st);
                    if (toConsole && Environment.UserInteractive)
                        Console.WriteLine("{0}", st);
                }
            }
            catch { }
        }

        public static void Write(string text, params object[] args)
        {
            WriteToDir(AssemblyDir, text, args);
        }

        public static void HiddenWrite(string text, params object[] args)
        {
            WriteToDir(false, AssemblyDir, text, args);
        }

        public static void AddLine(string text, params object[] args)
        {
            try
            {
                if (Environment.UserInteractive)
                    Console.WriteLine(text, args);
            }
            catch { }
        }

        public static void ChangeLine(string text, params object[] args)
        {
            try
            {
                if (Environment.UserInteractive)
                {
                    Console.Write(new String(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.WriteLine(text, args);
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                }
            }
            catch { }
        }

        public static string GetProgressBar(int value, int count)
        {
            int index = value % count;

            string result = string.Empty;
            for (int i = 0; i < count; i++)
                result += i == index ? "|" : "-";
            return result;
        }
    }
}
