using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL
{
    public class Diagnostics
    {
        public string LogName { get; set; } = Environment.CurrentDirectory + "//" + "log";
        public StringBuilder LogBuilder { get; set; } = new StringBuilder();
        public char NestSymbol { get; set; } = '-';
        public string[] DeconstructExclusion { get; set; } = new string[] { "Delimiter" };
        public string[] FilterEventExclusion { get; set; } = new string[] { "Delimiter" };

        public bool IsLogging { get; set; } = true;
        public bool IsDeconstructLog { get; set; } = true;
        public bool IsDeconstructEventLog { get; set; } = true;
        public bool IsConstructLog { get; set; } = true;
        public bool IsConstructEventLog { get; set; } = true;
        public bool IsSelectEventLog { get; set; } = true;

        public void LOG_Header(string text)
        {
            LogBuilder.AppendLine($"---------------|    {text}    |---------------{Environment.NewLine}");
        }
        public void LOG_Subheader(string text)
        {
            LogBuilder.AppendLine($"-----|    {text}    |-----");
        }
        public void LOG_NestLine(int depth, string text) { LogBuilder.AppendLine(new string(NestSymbol, depth) + " " + text); }
        public void LOG_Nest(int depth, string text) { LogBuilder.Append(new string(NestSymbol, depth) + " " + text); }
        public void LOG_Space() { LogBuilder.Append(Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine); }

        public void SaveLog()
        {
            if (File.Exists(LogName + ".txt"))
                File.Delete(LogName + ".txt");

            File.AppendAllText(LogName + ".txt", LogBuilder.ToString());
            LogBuilder.Clear();
        }
    }
}
