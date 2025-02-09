using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ServerBackend.Helpers;

public class ImageHelper
{
    //TODO: Da testare e introdurre
    public static byte[] CropImage(byte[] img, int width, int height)
    {
        using var image = SixLabors.ImageSharp.Image.Load(img);
                
        var originalWidth = image.Width;
        var originalHeight = image.Height;
                
        var cropSize = Math.Min(originalWidth, originalHeight);
        var startX = (originalWidth - cropSize) / 2;
        var startY = (originalHeight - cropSize) / 2;
                
        image.Mutate(x => x.Crop(new Rectangle(startX, startY, cropSize, cropSize)));
                
        image.Mutate(x => x.Resize(width, height));
                
        using var ms = new System.IO.MemoryStream();
        image.SaveAsJpeg(ms);
        return ms.ToArray();
    }
}