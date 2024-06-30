class InputParser
{
    public Dictionary<Flags, bool> FeatureFlags;
    public Settings options;

    public InputParser(string[] args)
    {
        FeatureFlags = new Dictionary<Flags, bool>
        {
            {Flags.Convert, false},
            {Flags.Map, false}
        };
        options = ParseInput(args);
        Logger.Debug("Scaling: " + options.Flags[Flags.Scaling]);
        Logger.Debug("Map: " + options.Flags[Flags.Map]);
        Logger.Debug("PixelSize: " + options.PixelSize);
        Logger.Debug("ImageScaling: " + options.ImageScaling);
    }

    private Settings ParseInput(string[] args)
    {
        Flags currentToken = Flags.None;
        Settings settings = new();

        foreach (string token in args)
        {
            if (token.StartsWith("--"))
            {
                string opName = token[2..];
                if (!Enum.TryParse(opName, out Flags flag))
                {
                    throw new Exception($"Unknown token: {token}");
                }
                else
                {
                    currentToken = flag;
                }
            }
            else if (currentToken != Flags.None)
            {
                switch (currentToken)
                {

                    case Flags.PixelSize:
                        int.TryParse(token, out int size);
                        if (size > 10)
                        {
                            settings.PixelSize = size;
                        }
                        break;
                    case Flags.Scaling:
                        float.TryParse(token, out float factor);
                        if (factor > 0.1)
                        {
                            settings.ImageScaling = factor;
                        }
                        break;
                    case Flags.Filename:
                        settings.Filename = token;
                        break;
                }
            }
            else
            {
                throw new Exception("Token error: " + currentToken);
            }
            settings.Flags[currentToken] = true;
        }
        return settings;
    }
}

public enum Flags
{
    None = 0,
    Convert,
    Ascii,
    Map,
    Scaling,
    PixelSize,
    Parallel,
    Invert,
    Filename,
}
public class Settings
{
    public int PixelSize { get; set; }
    public float ImageScaling { get; set; }
    public string Filename { get; set; }
    public Dictionary<Flags, bool> Flags;

    public Settings()
    {
        Flags = new Dictionary<Flags, bool>
        {
            { global::Flags.Convert, false},
            { global::Flags.Map, false},
            { global::Flags.Scaling, false},
            { global::Flags.PixelSize, false},
            { global::Flags.Parallel, false},
            { global::Flags.Invert, false}
        };

        PixelSize = 50;
        ImageScaling = 1f;
    }
}