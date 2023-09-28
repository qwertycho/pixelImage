using System.Reflection;
using System.Drawing;
using System.Net;

Logger.SetLogLevel(Logger.LogLevel.Debug);
InputParser inputParser = new InputParser(args);
ImageMapper imageMapper = new ImageMapper();

if(inputParser.options.Flags[Operations.Map])
{
    foreach(var file in Directory.GetFiles("pixels"))
    {
        try{
            //windows only
            Logger.Debug(file);
            Bitmap img = new Bitmap(file);
            Pixel px = imageMapper.ImageParser(img, file, inputParser.options);
            img.Dispose();
            imageMapper.pixels.Add(px);
        } catch(Exception e)
        {
            Logger.ERROR(e.Message);
        }
    }
}

if(inputParser.options.Flags[Operations.Convert])
{
    foreach(var file in Directory.GetFiles("input"))
    {
        int pixelSize = inputParser.options.PixelSize;

        Bitmap rawImg = new Bitmap(file);
        int scaledWidth = (int)(rawImg.Width*inputParser.options.ImageScaling)/pixelSize;
        int scaledHeight = (int)(rawImg.Height*inputParser.options.ImageScaling)/pixelSize;

        Logger.Debug("pixel: " + pixelSize);
        Logger.Debug("new height: " + scaledHeight * pixelSize);
        Logger.Debug("new width: " + scaledWidth * pixelSize);

        if(scaledWidth*pixelSize > 10000 || scaledHeight*pixelSize > 10000)
        {
            throw new Exception(scaledWidth*pixelSize + "px X " + scaledHeight*pixelSize + "px too large. Max 10000 X 10000" );
        }

        Bitmap scaledImage = imageMapper.ImageScaler(rawImg, width: scaledWidth , height: scaledHeight);

        Bitmap newImg = new Bitmap(width: scaledWidth*pixelSize, height: scaledHeight*pixelSize);
        rawImg.Dispose();

        for(int x = 0; x < scaledImage.Width; x++)
        {
            for(int y = 0; y < scaledImage.Height; y++)
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
                    int xO = x*pixelSize;
                    int yO = y*pixelSize;
                    if(x == 0) {xO = x;}
                    if(y == 0) {yO = y;}
                    
                    graphics.DrawImage(px.Img, xO, yO);
                }
            }
        }
        Logger.Debug("Saving img...");
        newImg.Save("output/final.jpg");
        newImg.Dispose();
    }
}