class InputParser
{
    public Dictionary<Operations, bool> FeatureFlags;
    public Settings options;

    public InputParser(string[] args)
    {
        FeatureFlags = new Dictionary<Operations, bool>
        {
            {Operations.Convert, false},
            {Operations.Map, false}
        };
        options = ParseInput(args);
        Logger.Debug("Scaling: " + options.Flags[Operations.Scaling]);
        Logger.Debug("Map: " + options.Flags[Operations.Map]);
        Logger.Debug("PixelSize: "+ options.PixelSize);
        Logger.Debug("ImageScaling: "+ options.ImageScaling);
    }

    private Settings ParseInput(string[] args)
    {
        Operations currentToken = Operations.None;
        Settings settings = new();

        foreach(string token in args)
        {
            if(token.StartsWith("--"))
            {
                string opName = token[2..];
                if (!Enum.TryParse(opName, out Operations flag)) 
                {
                    throw new Exception("Unknown token!");
                }
                else 
                {
                    currentToken = flag;
                }
            } else if(currentToken != Operations.None)
            {
                switch (currentToken)
                {
                    
                    case Operations.PixelSize:
                        int.TryParse(token, out int size);
                        if(size > 10) 
                        {
                            settings.PixelSize = size;
                        }
                        break;
                    case Operations.Scaling:
                        float.TryParse(token, out float factor);
                        if(factor > 0.1)
                        {
                            settings.ImageScaling = factor;
                        }
                        break;
                }
            } else{
                throw new Exception("Token error: " + currentToken);
            }
            settings.Flags[currentToken] = true;
        }
        return settings;
    }
}

public enum Operations 
{
    None = 0,
    Convert,
    Map,
    Scaling,
    PixelSize,
    Parallel
}
public class Settings
{
    public int PixelSize {get; set;}
    public float ImageScaling {get; set;}
    public Dictionary<Operations, bool> Flags;

    public Settings()
    {
        Flags = new Dictionary<Operations, bool>
        {
            {Operations.Convert, false},
            {Operations.Map, false},
            {Operations.Scaling, false},
            {Operations.PixelSize, false},
            {Operations.Parallel, false}
        };

        PixelSize = 50;
        ImageScaling = 1f;
    }
}