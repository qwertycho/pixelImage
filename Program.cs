using System.Reflection;
using System.Net;
using System.Diagnostics;

Logger.SetLogLevel(Logger.LogLevel.Debug);
InputParser inputParser = new InputParser(args);
ImageMapper imageMapper = new ImageMapper(inputParser.options);

if(inputParser.options.Flags[Operations.Map])
{
    var files = Directory.GetFiles("pixels");
    ProgressBar pb = new(files.Length);
    if(inputParser.options.Flags[Operations.Parallel])
    {
        Parallel.ForEach(files, file =>
        {
            //windows only
            imageMapper.AddPixelFromFile(file);
            pb.Tick();
        });
    } else 
    {
        foreach (var file in Directory.GetFiles("pixels"))
        {
            Image img = Image.Load(file); 
            Pixel px = imageMapper.ImageParser(img, file);
            img.Dispose();
            imageMapper.pixels.Add(px);
            pb.Tick();
        }
    }
}

if(inputParser.options.Flags[Operations.Convert])
{
    Converter.Convert(imageMapper, inputParser.options);
}
