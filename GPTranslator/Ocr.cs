
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Tesseract;
using Path = System.IO.Path;

namespace GPTranslator
{
    public static class Ocr
    {
        public static string OcrYap(this byte[] dosya)
        {
            if (Directory.Exists(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\tessdata"))
            {
                try
                {
                    return GetOcr(dosya);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "GPTranslator", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return string.Empty;
                }
            }
            MessageBox.Show("Tesseract Engine Klasörünü Kontrol Edin." , "GPTranslator", MessageBoxButton.OK, MessageBoxImage.Error);
            return string.Empty;
        }

        private static string GetOcr(byte[] dosya)
        {
            using (var engine = new TesseractEngine("./tessdata", "tur", EngineMode.TesseractOnly))
            {
                using (var pixImage = Pix.LoadFromMemory(dosya))
                using (var page = engine.Process(pixImage.Scale(3, 3)))
                {
                    return page.GetText();
                }
            }
        }
    }
}