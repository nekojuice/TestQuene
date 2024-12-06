using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TestQuene.Services;

namespace TestQuene.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileHubController : ControllerBase
    {
        private readonly PdfConversionService _pdfConversionService;

        public FileHubController(PdfConversionService pdfConversionService) 
        {
            _pdfConversionService = pdfConversionService;
        }


        [Route("runQuene")]
        [HttpGet]
        public async Task<IActionResult> RunQueneAsync(string? testString)
        {
            try
            {
                string pdfPath = await _pdfConversionService.ConvertToPdfAsync(testString);
                Console.WriteLine($"PDF 已生成，路徑: {pdfPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"轉換失敗: {ex.Message}");
            }

            return Ok();
        }
    }
}
