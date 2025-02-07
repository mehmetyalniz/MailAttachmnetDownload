namespace MailAttechmentDowlond;

public class WeatherForecast
{
    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }

    public int Host { get; set; }

    public int Port { get; set; }

    public string Username { get; set; }

    public string? Password { get; set; }
}


