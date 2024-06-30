using SixLabors.ImageSharp;

Logger.SetLogLevel(Logger.LogLevel.Debug);
InputParser inputParser = new InputParser(args);
ImageMapper imageMapper = new ImageMapper(inputParser.options);

if (inputParser.options.Flags[Flags.Map])
{
    var files = Directory.GetFiles("pixels");
    ProgressBar pb = new(files.Length);
    if (inputParser.options.Flags[Flags.Parallel])
    {
        Parallel.ForEach(files, file =>
        {
            //windows only
            imageMapper.AddPixelFromFile(file);
            pb.Tick();
        });
    }
    else
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


if (inputParser.options.Flags[Flags.Convert])
{
    Converter.Convert(imageMapper, inputParser.options);
}
else if (inputParser.options.Flags[Flags.Ascii])
{   
    string[] lines = AsciiMapper.Convert(inputParser.options.Filename, inputParser.options.Flags[Flags.Invert]);
    Console.WriteLine();
    foreach(var line in lines)
    {
        Console.WriteLine(line);
    }
}