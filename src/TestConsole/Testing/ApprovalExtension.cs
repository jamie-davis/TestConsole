using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TestConsoleLib.Exceptions;

namespace TestConsoleLib.Testing
{
    public static class ApprovalExtension
    {
        public static void Verify(this string text)
        {
            var frame = FindStackFrame();
            var path = Path.GetDirectoryName(frame.GetFileName());
            var method = frame.GetMethod();

            var baseFileName = $"{method.DeclaringType.Name}.{method.Name}";
            var receivedOutput = $"{baseFileName}.received.txt";
            var approvedOutput = $"{baseFileName}.approved.txt";
            var approvedFile = Path.Combine(path, approvedOutput);
            var receivedFile = Path.Combine(path, receivedOutput);
            if (File.Exists(approvedFile))
            {
                var approved = FixPlatformLineEndings(File.ReadAllText(approvedFile));
                if (approved != FixPlatformLineEndings(text))
                {
                    File.WriteAllText(receivedFile, text);
                    CompareUtil.CompareFiles(receivedFile, approvedFile);
                    throw new ApprovedFileMismatchException();
                }
                
                if (File.Exists(receivedFile))
                    File.Delete(receivedFile);
                return;
            } 
            
            File.WriteAllText(receivedFile, text);
            File.WriteAllText(approvedFile, string.Empty);
            CompareUtil.CompareFiles(receivedFile, approvedFile);
            throw new NoApprovedFileException();
        }

        private static StackFrame FindStackFrame()
        {
            var stackTrace = new StackTrace(true);
            Debug.Assert(stackTrace != null);
            var frames = stackTrace.GetFrames();
            Debug.Assert(frames != null);
            return frames.Select(f => new { Frame = f, Method = f.GetMethod()})
                .First(m => m.Method.DeclaringType != typeof(ApprovalExtension) && m.Method.DeclaringType != typeof(Approvals))
                .Frame;
        }

        private static string FixPlatformLineEndings(string text)
        {
            var sb = new StringBuilder();
            using (var reader = new StringReader(text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    sb.AppendLine(line);
            }

            return sb.ToString();
        }

        public static void Verify(StringBuilder text)
        {
            Verify(text.ToString());
        }
        
        public static string JoinWith(this IEnumerable<string> input, string concatentator)
        {
            return string.Join(concatentator, input);
        }
        
        public static string WritePropertiesToString<T>(this T value)
        {
            return WriteObjectToString(value, WriteProperties);
        }

        private static void WriteProperties<T>(T value, StringBuilder sb, Type t)
        {
            foreach (var p in t.GetProperties())
            {
                if (p.CanRead)
                {
                    var propertyValue = p.GetValue(value, new object[0]) ?? "NULL";
                    sb.AppendFormat("\t{0}: {1}", p.Name, propertyValue).AppendLine();
                }
            }
        }

        public static string WriteFieldsToString<T>(this T value)
        {
            return WriteObjectToString(value, WriteFields);
        }

        private static void WriteFields<T>(T value, StringBuilder sb, Type t)
        {
            foreach (var f in t.GetFields())
            {
                if (f.IsPublic)
                {
                    var propertyValue = f.GetValue(value) ?? "NULL";
                    sb.AppendFormat("\t{0}: {1}", f.Name, propertyValue).AppendLine();
                }
            }
        }

        private static string WriteObjectToString<T>(T value, Action<T, StringBuilder, Type> writer)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var t = typeof (T);
            var sb = new StringBuilder();
            sb.AppendLine(t.Name);
            sb.AppendLine("{");
            writer(value, sb, t);

            sb.AppendLine("}");

            return sb.ToString();
        }

    }

}