using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

public class ImageMapper
{
    public List<Pixel> pixels = new List<Pixel>();
    public readonly Settings options;

    public ImageMapper(Settings settings) 
    {
        options = settings;
    }

    public Image<Rgba32> ImageScaler(Image image, int width=50, int height=50)
    {
        image.Mutate(x => x.Resize(width, height));
        return image.CloneAs<Rgba32>();
    }

    public async Task AddPixelFromFile(string file)
    {
        Image img =  Image.Load(file);
        Pixel px = ImageParser(img, file);
        pixels.Add(px);
    }
    
    public Pixel GetMatch(Pixel toMatch)
    {
        int maxOffset= 25;
        int incrementor = 1;
        bool foundMatch = false;
        int bestOffset = 765; ///255 x 3 (rgb)
        Pixel bestMatch = pixels[0];
        
        while(!foundMatch)
        {
            foreach(Pixel px in pixels)
            {
                if(IsMatch(px.Red, toMatch.Red, maxOffset) && IsMatch(px.Green, toMatch.Green, maxOffset) && IsMatch(px.Blue, toMatch.Blue, maxOffset))
                {
                    foundMatch = true;
                    int tmpOffset = GetError(px.Red, toMatch.Red) + GetError(px.Green, toMatch.Green) + GetError(px.Blue, toMatch.Blue);

                    if(tmpOffset < bestOffset)
                    {
                        bestOffset = tmpOffset;
                        bestMatch = px;
                    }
                    
                    //return px; // eerste match terugsturen
                }
            }
            maxOffset += incrementor;
        }
        return bestMatch;
    }

    private int GetError(int val1, int val2)
    {
        int offset = val1 - val2;
        return offset;
    }

    private bool IsMatch(int val1, int val2, int max)
    {
        int offset = val1 - val2;
        offset = Math.Abs(offset);
        if(offset >= max){ return false;}
        return true;
    }

    public Pixel ImageParser(Image img, string name)
    {
        int TRed = 0;
        int TGreen = 0;
        int TBlue = 0;

        img = ImageScaler(img, width: options.PixelSize, height: options.PixelSize);
        Image<Rgba32> imgRGB = img.CloneAs<Rgba32>();

        for(int x = 0 ; x < img.Width; x++)
        {
            for(int y = 0; y < img.Height; y++)
            {
                Rgba32 pxl = imgRGB[x, y];
                TRed += pxl.R;
                TGreen += pxl.G;
                TBlue += pxl.B;
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
        px.Img = img.CloneAs<Rgba32>();
        px.name = name;

        return px;
    }


}
