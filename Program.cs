using System.Reflection;
using System.Drawing;

InputParser inputParser = new InputParser(args);
ImageMapper imageMapper = new ImageMapper();

if(inputParser.FeatureFlags[InputParser.Operations.Map])
{
    Console.WriteLine("Starting mapping operation");
    foreach(var file in Directory.GetFiles("pixels"))
    {
        try{
            //windows only
            Console.WriteLine(file);
            Bitmap img = new Bitmap(file);
            Pixel px = imageMapper.ImageParser(img, file);
            img.Dispose();
            imageMapper.pixels.Add(px);
        } catch(Exception e)
        {
            Console.WriteLine(e);
        }
    }
}

if(inputParser.FeatureFlags[InputParser.Operations.Convert])
{
    Console.WriteLine("Starting converting operation");
    foreach(var file in Directory.GetFiles("input"))
    {
        Bitmap rawImg = new Bitmap(file);
        int widthImages = rawImg.Width/50;
        int heightImages = rawImg.Height/50;

        Bitmap scaledImage = imageMapper.ImageScaler(rawImg, width: widthImages , height: heightImages);
        Bitmap newImg = new Bitmap(width: widthImages*50, height: heightImages*50);
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
                    int xO = x*50;
                    int yO = y*50;
                    if(x == 0) {xO = x;}
                    if(y == 0) {yO = y;}
                    
                    graphics.DrawImage(px.Img, xO, yO);
                }
            }
        }
        newImg.Save("output/final.jpg");

    }
}