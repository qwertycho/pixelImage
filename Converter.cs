using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
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

            Image rawImg = Image.Load(file);
            int scaledWidth = (int)(rawImg.Width * options.ImageScaling) / pixelSize;
            int scaledHeight = (int)(rawImg.Height * options.ImageScaling) / pixelSize;

            Logger.Debug("pixel: " + pixelSize);
            Logger.Debug("new height: " + scaledHeight * pixelSize);
            Logger.Debug("new width: " + scaledWidth * pixelSize);

            if (scaledWidth * pixelSize > 15000 || scaledHeight * pixelSize > 15000)
            {
                throw new Exception(scaledWidth * pixelSize + "px X " + scaledHeight * pixelSize + "px too large. Max 15000 X 15000");
            }

            Image<Rgba32> scaledImage = imageMapper.ImageScaler(rawImg, width: scaledWidth, height: scaledHeight);

            Image<Rgba32> newImg = new Image<Rgba32>(Configuration.Default, scaledWidth * pixelSize, scaledHeight * pixelSize);

            ProgressBar pb = new(scaledImage.Width);

            for (int x = 0; x < scaledImage.Width; x++)
            {
                for (int y = 0; y < scaledImage.Height; y++)
                {
                    Rgba32 pxl = scaledImage[x, y];
                    Pixel toMatch = new()
                    {
                        Red = pxl.R,
                        Green = pxl.G,
                        Blue = pxl.B
                    };

                    Pixel px = imageMapper.GetMatch(toMatch);

                        // Draw the overlay image onto the background at the specified position
                        int xO = x * pixelSize;
                        int yO = y * pixelSize;
                        if (x == 0) { xO = x; }
                        if (y == 0) { yO = y; }

                        newImg.Mutate(x => 
                            x.DrawImage(px.Img, new Point(xO, yO), 1f));
                }
                pb.Tick();
            }
            stopwatch.Stop();
            newImg.Save("output/final.jpg");
            Logger.Debug("Converted img in: " + stopwatch.ElapsedMilliseconds + "MS");
            
        }
    }
}
