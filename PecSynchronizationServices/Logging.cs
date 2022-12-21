/*
 * Copyright (C) 2020 Pheinex LLC
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace PecSynchronizationServices
{
    static class Logging
    {
        private const int MaxLogSize = 1048576;

        private static string loggingPath;

        private static string LoggingPath
        {
            get
            {
                if (loggingPath == null)
                {
                    var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    loggingPath = Path.Combine(appDataPath, "Pheinex");
                    Debug.Print($"Log file path: {loggingPath}");
                }

                return loggingPath;
            }
        }
        public static void LogItem(string appName, string message)
        {
            try
            {
                Directory.CreateDirectory(LoggingPath);
                var filePath = Path.Combine(LoggingPath, $"{appName}.log");
                var append = true;
                if (File.Exists(filePath))
                {
                    var fileInfo = new FileInfo(filePath);
                    if (fileInfo.Length > MaxLogSize)
                    {
                        append = false;
                    }
                }

                var logMessage = $"{DateTime.Now}: {message}";
                using (var file = new StreamWriter(filePath, append))
                {
                    file.WriteLine(logMessage);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
            }
        }

        public static void LogItem(string message)
        {
            LogItem(Assembly.GetCallingAssembly().GetName(false).Name, message);
        }
    }
}
