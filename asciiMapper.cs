using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

public static class AsciiMapper
{
  public static bool WIDTH_CORRECTION = false;
  public static string[] Convert(string file, bool invert = false)
  {
    Image<Rgba32> img = Image.Load(file).CloneAs<Rgba32>();
    img = Scale(img);

    Ascii charset = new Ascii(invert);

    string[] rows = Map(img, charset.Chars);

    return rows;
  }

  private static string[] Map(Image<Rgba32> img, IEnumerable<Char> charset)
  {
    string[] rows = new string[img.Height];
    StringBuilder sb = null;
    ProgressBar pb = new(img.Height, Console.WindowWidth - 15);

    for(int row = 0; row < img.Height; row++)
    {
      sb = new StringBuilder();
      for(int column = 0; column < img.Width; column++)
      {
        var px = img[column, row];
        float pxVal = ((float)px.R + (float)px.G + (float)px.B) / 765; //255*3
        Char match = GetMatch(pxVal * 10, charset);
        sb.Append((WIDTH_CORRECTION? " " : "") + match.Character);
      }
      rows[row] = sb.ToString();
          pb.Tick();
    }

    return rows;
  }

  private static Char GetMatch(float val, IEnumerable<Char> charset)
  {
    float minErr = 100;
    Char match = charset.First();

    foreach(var c in charset)
    {
      float err = Math.Abs(c.Luminance - val);
      if(err == 0)
      {
        match = c;
        break;
      }
      if(err < minErr)
      {
        minErr = err;
        match = c;
      }
    }
    return match;
  }

  private static Image<Rgba32> Scale(Image i)
  {
    var img = i.CloneAs<Rgba32>();

    int currentWidth = WIDTH_CORRECTION? (Console.WindowWidth-10) / 2 : (Console.WindowWidth-10);

    float scaleFactor = (float)img.Width / (float)currentWidth;
    float newW = img.Width / scaleFactor;
    float newH = img.Height / scaleFactor;

    if(newH > Console.WindowHeight)
    {
        scaleFactor = newH / Console.WindowHeight;
        img.Mutate(x => x.Resize((int)(newW / scaleFactor), (int)(newH / scaleFactor)));
        return img.CloneAs<Rgba32>();
    } else {
        img.Mutate(x => x.Resize((int)(newW), (int)(newH)));
        return img.CloneAs<Rgba32>();
    }
  }
}

public class Char
{
  public int Luminance { get; set; }
  public char Character { get; set; }

  public Char(int l, char c)
  {
    Luminance = l;
    Character = c;
  }
}

public class Ascii
{
  public Char[] Chars = new Char[] {
    new Char(0, ' '),
    new Char(1, '.'),
    new Char(2, '"'),
    new Char(3, '*'),
    new Char(4, '+'),
    new Char(5, 'X'),
    new Char(6, 'O'),
    new Char(7, '&'),
    new Char(8, '@'),
    new Char(9, '#'),
  };

  public Ascii(bool invert)
  {
    if(invert)
    {
      foreach(var c in Chars)
      {
        c.Luminance = c.Luminance * -1;
      }
    }
  }
}
