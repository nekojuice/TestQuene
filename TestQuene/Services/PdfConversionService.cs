using System.Diagnostics;

namespace TestQuene.Services
{
    public class PdfConversionService
    {
        private readonly ConversionQueue _conversionQueue = new ConversionQueue();

        public Task<string> ConvertToPdfAsync(string sourceFullPath)
        {
            return _conversionQueue.Enqueue(async () =>
            {
                const int maxRetries = 3;
                string pdfFullPath = string.Empty;

                for (int attempt = 1; attempt <= maxRetries; attempt++)
                {
                    if (!string.IsNullOrEmpty(sourceFullPath))
                    {
                        Console.WriteLine("start: " + sourceFullPath);

                        await Task.Delay(5000);

                        Console.WriteLine("end: " + sourceFullPath);

                        return sourceFullPath;
                    }

                    //pdfFullPath = ConvertToPdf(sourceFullPath);

                    //if (File.Exists(pdfFullPath))
                    //{
                    //    return pdfFullPath;
                    //}

                    await Task.Delay(1000);
                }

                throw new Exception("PDF 轉換失敗");
            });
        }

        private static string ConvertToPdf(string sourceFullPath)
        {
            var startInfo = new ProcessStartInfo
            {
                Arguments =
                    $"--headless --convert-to pdf:writer_pdf_Export --outdir \"{Path.GetDirectoryName(sourceFullPath)}\" \"{sourceFullPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using (var process = Process.Start(startInfo))
            {
                if (!process.WaitForExit(60 * 1000))
                {
                    throw new TimeoutException("PDF 轉檔失敗");
                }
            }

            return Path.Combine(Path.GetDirectoryName(sourceFullPath) ?? string.Empty,
                $"{Path.GetFileNameWithoutExtension(sourceFullPath)}.pdf");
        }
    }

}
