namespace MauiClient.Models;

public class Monkey
{
    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string Details { get; set; } = default!;
    public string Image { get; set; } = default!;
    public int Population { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
}