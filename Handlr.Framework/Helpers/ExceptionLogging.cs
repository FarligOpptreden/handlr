using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handlr.Framework
{
    public class Logging
    {
        private static StringBuilder BuildExceptionMessage(Exception ex, StringBuilder sb = null)
        {
            sb = sb == null ? new StringBuilder() : sb;
            sb.Append(ex.Message);
            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                sb.Append("\n\tStack Trace: ");
                sb.Append(ex.StackTrace);
            }
            if (ex.InnerException != null)
            {
                sb.Append("\n\t----------------");
                sb.Append("\n\tInner Exception: ");
                BuildExceptionMessage(ex.InnerException, sb);
            }
            return sb;
        }

        public static void Log(Exception ex, string logPath)
        {
            Log(BuildExceptionMessage(ex).ToString(), logPath);
        }

        public static void Log(Exception ex, string logPath, long maxFileSize)
        {
            ArchiveFile(logPath, maxFileSize);
            Log(BuildExceptionMessage(ex).ToString(), logPath);
        }

        public static void Log(string message, string logPath)
        {
            int retryCount = 0;
            while (retryCount++ < 3)
            {
                StreamWriter stream = null;
                try
                {
                    using (stream = new StreamWriter(logPath, true))
                    {
                        stream.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("dd MMM yyyy HH:mm:ss"), message));
                        stream.Flush();
                        retryCount = 3;
                    }
                }
                catch { }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                        stream.Dispose();
                        stream = null;
                    }
                }
            }
        }

        public static void Log(string message, string logPath, long maxFileSize)
        {
            ArchiveFile(logPath, maxFileSize);
            Log(message, logPath);
        }

        public static void LogAsync(Exception ex, string logPath)
        {
            LogAsync(BuildExceptionMessage(ex).ToString(), logPath);
        }

        public static void LogAsync(Exception ex, string logPath, long maxFileSize)
        {
            LogAsync(BuildExceptionMessage(ex).ToString(), logPath, maxFileSize);
        }

        public static async void LogAsync(string message, string logPath)
        {
            await Task.Run(async () =>
            {
                int retryCount = 0;
                while (retryCount++ < 3)
                {
                    StreamWriter stream = null;
                    try
                    {
                        using (stream = new StreamWriter(logPath, true))
                        {
                            await stream.WriteLineAsync(string.Format("{0}: {1}", DateTime.Now.ToString("dd MMM yyyy HH:mm:ss"), message));
                            stream.Flush();
                            retryCount = 3;
                        }
                    }
                    catch
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                    finally
                    {
                        if (stream != null)
                        {
                            stream.Close();
                            stream.Dispose();
                            stream = null;
                        }
                    }
                }
            });
        }

        public static void LogAsync(string message, string logPath, long maxFileSize)
        {
            ArchiveFile(logPath, maxFileSize);
            LogAsync(message, logPath);
        }

        private static void ArchiveFile(string logPath, long maxFileSize)
        {
            try
            {
                FileInfo fi = new FileInfo(logPath);
                if (!fi.Exists || fi.Length < maxFileSize)
                    return;
                string fileName = string.Format("{0}_{1}{2}", fi.FullName.Replace(fi.Extension, ""), DateTime.Now.ToString("yyyyMMddHHmmssfff"), fi.Extension);
                File.Move(fi.FullName, fileName);
                int index = 0;
                var files =
                    (
                        from FileInfo f in fi.Directory.GetFiles(fi.Name.Replace(fi.Extension, "*"), SearchOption.TopDirectoryOnly)
                        orderby f.LastWriteTime descending
                        select new
                        {
                            File = f,
                            Index = index++
                        }
                    ).Where(f => f.Index >= 10).Select(f => f.File);
                foreach (var file in files)
                    file.Delete();
            }
            catch { }
        }
    }
}
