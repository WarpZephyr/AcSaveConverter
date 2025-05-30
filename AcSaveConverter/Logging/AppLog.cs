using AcSaveConverter.Text;
using System;
using System.IO;

namespace AcSaveConverter.Logging
{
    internal class AppLog : IDisposable
    {
        #region Save Data Fields

        private const string FileName = $"{Program.AppName}.log";
        private static readonly string FolderPath = Program.AppDataFolder;
        private static readonly string DataPath = Path.Combine(FolderPath, FileName);

        #endregion

        #region Instance Fields

        internal static AppLog Instance { get; private set; }

        #endregion

        #region Members

        private readonly Logger Log;
        private readonly StreamWriter? FileLog;
        private readonly object LogLock;
        private bool disposedValue;

        #endregion

        #region Instance Constructor

        static AppLog()
        {
            var logger = new Logger(5, 3, 3, true);

            Exception? error;
            StreamWriter? fileLog;
            try
            {
                Directory.CreateDirectory(FolderPath);
                fileLog = new StreamWriter(DataPath, true, AppEncoding.ShiftJIS);
                error = null;
            }
            catch (Exception ex)
            {
                fileLog = null;
                error = ex;
            }

            Instance = new AppLog(logger, fileLog);
            if (error != null)
            {
                Instance.DirectWriteLine($"Failed opening file log from path \"{DataPath}\": {error}");
            }
        }

        #endregion

        #region Constructors

        public AppLog(Logger log, StreamWriter? fileLog)
        {
            LogLock = new object();
            Log = log;
            FileLog = fileLog;
            PrintTimeInfo();
            PrintAppInfo();
        }

        public AppLog(Logger log) : this(log, null) { }

        #endregion

        #region Info Methods

        private void PrintTimeInfo()
        {
            WriteLine($"[Log Started: {DateTime.Now:MM-dd-yyyy-hh:mm:ss}]");
        }

        private void PrintAppInfo()
        {
            var appInfo = Program.AppInfo;
            WriteLine($"[App Platform: {appInfo.Platform}]");
            WriteLine($"[App Version: {appInfo.Version}]");
#if DEBUG
            WriteLine("[App Build: Debug]");
#else
            WriteLine("[App Build: Release]");
#endif
            WriteLine($"[App Directory: \"{appInfo.AppDirectory}\"]");
        }

        #endregion

        #region Logging Methods

        public void Write(string value)
        {
            lock (LogLock)
            {
                Log.Write(value);
                FileLog?.Write(value);
            }
        }

        public void WriteLine(string value)
        {
            lock (LogLock)
            {
                Log.WriteLine(value);
                FileLog?.WriteLine(value);
            }
        }

        public void WriteLine()
        {
            lock (LogLock)
            {
                Log.WriteLine();
                FileLog?.WriteLine();
            }
        }

        public void DirectWrite(string value)
        {
            lock (LogLock)
            {
                Log.DirectWrite(value);
                FileLog?.Write(value);
            }
        }

        public void DirectWriteLine(string value)
        {
            lock (LogLock)
            {
                Log.DirectWriteLine(value);
                FileLog?.WriteLine(value);
            }
        }

        public void DirectWriteLine()
        {
            lock (LogLock)
            {
                Log.DirectWriteLine();
                FileLog?.WriteLine();
            }
        }

        public void Flush()
        {
            lock (LogLock)
            {
                Log.Flush();
                FileLog?.Flush();
            }
        }

        #endregion

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    WriteLine($"[Log Ended: {DateTime.Now:MM-dd-yyyy-hh:mm:ss}]");

                    Flush();
                    Log.Dispose();
                    FileLog?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
