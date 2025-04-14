using IronOcr;

namespace CleanArchitecture.OCR
{
    public class OcrService
    {
        public async Task<string> ExtractTextAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return "Invalid file";

            using var memoryStream = new MemoryStream();
            await imageFile.CopyToAsync(memoryStream);
            var ocr = new IronTesseract();

            using var ocrInput = new OcrInput(memoryStream.ToArray());
            var result = ocr.Read(ocrInput);
            return result.Text;
        }
    }
}
