using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OTrace.Class {
    static class Log {
        static private string logFilePath = "C:\\Диплом\\OTrace\\Log.txt";
        static StreamWriter sw;

        static Log() {
            return;
            Application.ApplicationExit += end;
            Application.ThreadException += smthGoWrong;

            if (File.Exists(logFilePath) == false) {
                File.Create(logFilePath).Close();
            }
            sw = new StreamWriter(logFilePath);
        }

        public static void log(string str, bool writeToConsole = true) {
            return;
            sw.WriteLine("[" + DateTime.Now + "] " + str);
            if (writeToConsole == false) return;
            Console.WriteLine(str);
        }

        public static void end(object sender, EventArgs e) {
            return;
            sw.Close();
        }
        static void smthGoWrong(object sender, ThreadExceptionEventArgs t) {
            return;
            sw.WriteLine(t.Exception);
            sw.Close();
        }

    }
    class MException : Exception {
        public MException(string message) : base(message) {
            Log.end(null, null);
        }
    }
}
