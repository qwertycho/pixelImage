using SixLabors.ImageSharp;

public struct Pixel
{
        public int Red {get; set;}
        public int Green {get; set;}
        public int Blue {get; set;}
        public Image<Rgba32> Img {get; set;}
        public string name {get; set;}
}
