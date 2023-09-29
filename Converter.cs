using System.Drawing;
using System.Diagnostics;
public static class Converter
{
    public static void Convert(ImageMapper imageMapper, Settings options)
    {
        Console.WriteLine("Starting converting operation");
        foreach (var file in Directory.GetFiles("input"))
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();

            int pixelSize = options.PixelSize;

            Bitmap rawImg = new Bitmap(file);
            int scaledWidth = (int)(rawImg.Width * options.ImageScaling) / pixelSize;
            int scaledHeight = (int)(rawImg.Height * options.ImageScaling) / pixelSize;

            Logger.Debug("pixel: " + pixelSize);
            Logger.Debug("new height: " + scaledHeight * pixelSize);
            Logger.Debug("new width: " + scaledWidth * pixelSize);

            if (scaledWidth * pixelSize > 15000 || scaledHeight * pixelSize > 15000)
            {
                throw new Exception(scaledWidth * pixelSize + "px X " + scaledHeight * pixelSize + "px too large. Max 15000 X 15000");
            }

            Bitmap scaledImage = imageMapper.ImageScaler(rawImg, width: scaledWidth, height: scaledHeight);

            Bitmap newImg = new Bitmap(width: scaledWidth * pixelSize, height: scaledHeight * pixelSize);
            rawImg.Dispose();

            ProgressBar pb = new(scaledImage.Width);

            for (int x = 0; x < scaledImage.Width; x++)
            {
                for (int y = 0; y < scaledImage.Height; y++)
                {
                    Pixel toMatch = new()
                    {
                        Red = scaledImage.GetPixel(x, y).R,
                        Green = scaledImage.GetPixel(x, y).G,
                        Blue = scaledImage.GetPixel(x, y).B
                    };

                    Pixel px = imageMapper.GetMatch(toMatch);

                    using (Graphics graphics = Graphics.FromImage(newImg))
                    {
                        // Draw the overlay image onto the background at the specified position
                        int xO = x * pixelSize;
                        int yO = y * pixelSize;
                        if (x == 0) { xO = x; }
                        if (y == 0) { yO = y; }

                        graphics.DrawImage(px.Img, xO, yO);
                    }
                }
                pb.Tick();
            }
            stopwatch.Stop();
            newImg.Save("output/final.jpg");
            Logger.Debug("Converted img in: " + stopwatch.ElapsedMilliseconds + "MS");
            
        }
    }
}