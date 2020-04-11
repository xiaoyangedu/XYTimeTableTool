using Ionic.Zip;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace OSKernel.Presentation.Utilities
{
    public class ZipHelper
    {
        private static double _s_Count;

        private static string _s_LastEntry;

        public static bool Compress(string sourceDirectory, string zipFile, out string erroMessage)
        {
            try
            {
                using (ZipFile zip = new ZipFile())
                {
                    zip.AddDirectory(sourceDirectory);
                    zip.SaveProgress += new EventHandler<SaveProgressEventArgs>(Zip_SaveProgress);
                    zip.Save(zipFile);
                }
                erroMessage = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                erroMessage = ex.Message;
                return false;
            }
        }

        public static bool Decompress(string destinationDirectory, string zipFile, out string erroMessage)
        {
            try
            {
                ReadOptions options = new ReadOptions();
                options.Encoding = Encoding.Default;
                //DeleteLeftOvers(destinationDirectory);
                DeleteTempFile(destinationDirectory);
                using (ZipFile zip = ZipFile.Read(zipFile, options))
                {
                    _s_Count = 0;
                    _s_LastEntry = null;
                    zip.ExtractProgress += new EventHandler<ExtractProgressEventArgs>(OnExtractProgress);
                    zip.ExtractAll(destinationDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
                erroMessage = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                erroMessage = ex.Message;
                return false;
            }
        }

        static void OnExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            if (ExtractProgress != null && e.CurrentEntry != null)
            {
                if (e.CurrentEntry.FileName != _s_LastEntry)
                {
                    _s_LastEntry = e.CurrentEntry.FileName;
                    _s_Count++;
                    ExtractProgress(_s_Count / ((ZipFile)sender).Entries.Count, _s_LastEntry);
                }
            }
        }

        /// <summary>
        /// Delete any Files left from Interrupted Unzipping
        /// </summary>
        public static void DeleteLeftOvers(string basepath)
        {
            var paths = Directory.GetDirectories(basepath);
            foreach (var path in paths)
            {
                DeleteLeftOvers(path);
                string[] extensions = { ".tmp", ".pendingoverwrite" };
                var leftOverFiles = Directory.GetFiles(path, "*.*").Where(f => extensions.Contains(new FileInfo(f).Extension.ToLower())).ToArray();

                foreach (var leftOverFile in leftOverFiles)
                {
                    File.Delete(leftOverFile);
                }
            }
        }

        public static void DeleteTempFile(string basepath)
        {
            string[] extensions = { ".tmp", ".pendingoverwrite", ".dll.tmp", ".dll.PendingOverwrite" };
            var leftOverFiles = Directory.GetFiles(basepath, "*.*").Where(f => extensions.Contains(new FileInfo(f).Extension.ToLower())).ToArray();

            foreach (var leftOverFile in leftOverFiles)
            {
                File.Delete(leftOverFile);
            }
        }

        static void Zip_SaveProgress(object sender, SaveProgressEventArgs e)
        {
            if (CompressProgress != null && e.CurrentEntry != null)
            {
                if (e.BytesTransferred != 0 && e.TotalBytesToTransfer != 0)
                {
                    CompressProgress(((double)e.BytesTransferred / e.TotalBytesToTransfer)*100, e.CurrentEntry.FileName);
                }
            }
        }

        public static event Action<double, string> ExtractProgress;

        public static event Action<double, string> CompressProgress;
    }
}
