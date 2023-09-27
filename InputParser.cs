class InputParser
{
    public readonly Dictionary<Operations, bool> FeatureFlags;

    public InputParser(string[] args)
    {
        FeatureFlags = new Dictionary<Operations, bool>
        {
            {Operations.Convert, false},
            {Operations.Map, false}
        };
        ParseInput(args);
    }

    private void ParseInput(string[] args)
    {
        foreach(string input in args)
        {
            if(!input.StartsWith("--")) {throw new Exception("Unkown input!");}

            string flagName = input.Substring(2);
            if (!Enum.TryParse(flagName, out Operations flag)) {throw new Exception("Unkown flag!");}
            else {FeatureFlags[flag] = true;}
        }
    }


    public enum Operations 
    {
        Convert,
        Map,
}
}