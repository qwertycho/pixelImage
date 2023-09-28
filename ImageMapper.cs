using System.Drawing;
class ImageMapper
{
    public readonly List<Pixel> pixels = new List<Pixel>();

    public ImageMapper() {}

    public Bitmap ImageScaler(Bitmap bitmap, int width=50, int height=50)
    {
        Bitmap scaled = new Bitmap(width, height);

        using (Graphics graphics = Graphics.FromImage(scaled))
        {
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(bitmap, 0, 0, width, height);
        }
        return scaled;
    }
    
    public Pixel GetMatch(Pixel toMatch)
    {
        int maxOffset= 25;
        int incrementor = 1;
        bool foundMatch = false;
        
        while(!foundMatch)
        {
            foreach(Pixel px in pixels)
            {
                if(IsMatch(px.Red, toMatch.Red, maxOffset) && IsMatch(px.Green, toMatch.Green, maxOffset) && IsMatch(px.Blue, toMatch.Blue, maxOffset))
                {
                    foundMatch = true;
                    return px; // eerste match terugsturen
                }
            }
            maxOffset += incrementor;
        }
        throw new Exception("Compiler shutup");
    }

    private bool IsMatch(int val1, int val2, int max)
    {
        int offset = val1 - val2;
        offset = Math.Abs(offset);
        if(offset >= max){ return false;}
        return true;
    }

    public Pixel ImageParser(Bitmap img, string name, Settings options)
    {
        int TRed = 0;
        int TGreen = 0;
        int TBlue = 0;

        img = ImageScaler(img, width: options.PixelSize, height: options.PixelSize);

        for(int x = 0 ; x < img.Width; x++)
        {
            for(int y = 0; y < img.Height; y++)
            {
                Color pixel = img.GetPixel(x, y);
                TRed += pixel.R;
                TGreen += pixel.G;
                TBlue += pixel.B;
            }
        }
        int numPixels = img.Height * img.Width;
        int avgRed = TRed/numPixels;
        int avgGreen = TGreen/numPixels;
        int avgBlue = TBlue/numPixels;

        Pixel px = new Pixel();
        px.Red = avgRed;
        px.Blue = avgBlue;
        px.Green = avgGreen;
        px.Img = ImageScaler(img);
        px.name = name;

        return px;
    }


}

public struct Pixel
{
        public int Red {get; set;}
        public int Green {get; set;}
        public int Blue {get; set;}
        public Bitmap Img {get; set;}
        public string name {get; set;}
}