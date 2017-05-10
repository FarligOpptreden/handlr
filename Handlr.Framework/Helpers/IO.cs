using System.IO;

namespace Handlr.Framework
{
    public abstract class IO
    {
        public static bool IsFile(string path)
        {
            try
            {
                FileInfo fi = new FileInfo(path);
                return fi.Exists;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsFileInUse(string path)
        {
            FileStream stream = null;
            try
            {
                FileInfo fi = new FileInfo(path);
                stream = fi.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }

        public static string ReadTextFile(string path)
        {
            if (!IsFile(path))
                return null;
            try
            {
                string result = "";
                using (StreamReader reader = new StreamReader(path))
                {
                    result = reader.ReadToEnd();
                }
                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}
