using System.IO;
using System.Text;
using System.Threading;
using FluentAssertions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Utils.Files
{
    public static class FileHelper
    {
        public static void DeleteDownloadFile(string filePath)
        {
            if (FileExists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public static bool FileExists(string filePath) => File.Exists(filePath);

        public static long FileLength(string filePath) => new FileInfo(filePath).Length;

        public static void WaitForDownloadFile(string filePath)
        {
            for (int i = 0; i < 10; i++)
            {
                if (FileExists(filePath))
                {
                    break;
                }

                Thread.Sleep(1000);
            }
        }

        public static void ValidateIsPdf(string filePath)
        {
            var buffer = new byte[5];
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var bytesRead = fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            bytesRead.Should().Be(buffer.Length);
            buffer.Should().BeEquivalentTo(Encoding.ASCII.GetBytes("%PDF-"));
        }
    }
}
