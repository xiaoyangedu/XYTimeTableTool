using System;
using System.IO;

namespace OSKernel.Presentation.Utilities
{
    public class FileHelper
    {
        private static int _s_TotalFileCount;

        private static double _s_Count;

        public static event Action<double, string> CopyProgress;

        public static void Copy(string sourceDirectory, string destDirectory)
        {
            if (!Directory.Exists(sourceDirectory))
            {
                throw new ArgumentException();
            }

            _s_TotalFileCount = Directory.GetFiles(sourceDirectory, "*.*", SearchOption.AllDirectories).Length;
            _s_Count = 0;

            CopyDirectory(sourceDirectory, destDirectory);
        }

        public static string GetTempDirectory()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            return tempDir;
        }

        private static void CopyDirectory(string sourceDirectory, string destDirectory)
        {
            //判断目标目录是否存在，如果不存在，则创建一个目录
            if (!Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }

            //拷贝文件
            CopyFile(sourceDirectory, destDirectory);

            //拷贝子目录
            //获取所有子目录名称
            string[] directionName = Directory.GetDirectories(sourceDirectory);
            foreach (string directionPath in directionName)
            {
                //根据每个子目录名称生成对应的目标子目录名称
                string directionPathTemp = destDirectory + "\\" + directionPath.Substring(directionPath.LastIndexOf('\\') + 1);
                //递归下去
                CopyDirectory(directionPath, directionPathTemp);
            }
        }

        private static void CopyFile(string sourceDirectory, string destDirectory)
        {
            //获取所有文件名称
            string[] fileName = Directory.GetFiles(sourceDirectory);
            foreach (string filePath in fileName)
            {
                //根据每个文件名称生成对应的目标文件名称
                //string filePathTemp = destDirectory + "\\" + filePath.Substring(sourceDirectory.Length + 1);
                string filePathTemp = destDirectory + "\\" + Path.GetFileName(filePath);
                //若不存在，直接复制文件；若存在，覆盖复制
                if (File.Exists(filePathTemp))
                {
                    File.Copy(filePath, filePathTemp, true);
                }
                else
                {
                    File.Copy(filePath, filePathTemp);
                }

                _s_Count++;
                if (CopyProgress != null)
                {
                    CopyProgress(_s_Count / _s_TotalFileCount, Path.GetFileName(filePath));
                }
            }
        }
    }
}
