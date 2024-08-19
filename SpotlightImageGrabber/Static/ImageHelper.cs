using SkiaSharp;
using System.Drawing;

namespace SpotlightImageGrabber.Static
{
    public class ImageHelper
    {
        /// <summary>
        /// Determine if a file is a known image type.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="extension">The extension if the image is a known type.</param>
        /// <returns></returns>
        public static bool IsImageFile(string filePath, out string extension)
        {
            extension = "";

            byte[] buffer = new byte[8];
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                if (fs.Length < 8)
                    return false;

                fs.Read(buffer, 0, buffer.Length);
            }

            // Check for JPEG (first two bytes should be FF D8)
            if (buffer[0] == 0xFF && buffer[1] == 0xD8)
            {
                extension = "jpg";
                return true;
            }

            // Check for PNG (first eight bytes should be 89 50 4E 47 0D 0A 1A 0A)
            if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47 &&
                buffer[4] == 0x0D && buffer[5] == 0x0A && buffer[6] == 0x1A && buffer[7] == 0x0A)
            {
                extension = "png";
                return true;
            }


            // Check for GIF (first three bytes should be 47 49 46)
            if (buffer[0] == 0x47 && buffer[1] == 0x49 && buffer[2] == 0x46)
            {
                extension = "gif";
                return true;
            }

            // Check for BMP (first two bytes should be 42 4D)
            if (buffer[0] == 0x42 && buffer[1] == 0x4D)
            {
                extension = "bmp";
                return true;
            }

            return false;
        }

        /// <summary>
        /// Open a file and return the image dimensions.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Size ImageDimensions(string filePath)
        {
            try
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var codec = SKCodec.Create(stream);
                    if (codec == null) return Size.Empty;
                    Size s = Size.Empty;
                    s.Width = codec.Info.Width;
                    s.Height = codec.Info.Height;
                    return s;
                }
            }
            catch
            {
                return Size.Empty;
            }
        }
    }
}
