class ProgressBar
{
    private int _width;
    private char _symbol;
    private int _progres = 0;
    private int _final;
    public ProgressBar(int final, int width = 50, char symbol = '#')
    {
        _width = width;
        _symbol = symbol;
        _final = final;
    }
    public void Tick()
    {
        _progres++;
        float perc = ((float)_progres/_final );

        int numChars = (int)(_width * perc);

        string prgbr = new(_symbol, numChars);
        string empt = new(' ', _width-numChars);

        Console.Write("\r[" + prgbr + empt + "]" + (int)(perc*100) + "%");
    }
}