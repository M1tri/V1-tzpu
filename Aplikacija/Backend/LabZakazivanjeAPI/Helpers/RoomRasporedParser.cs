using System.Text;

namespace LabZakazivanjeAPI.Helpers;

public class RoomRasporedParser
{
    public record Seat(int? Id, string? IP);

    public static List<List<Seat>> ParserRaspored(string raspored)
    {
        // ((id:ip, id:ip), (id:ip, id:ip, id:ip), (id:ip),)

        var result = new List<List<Seat>>();
    
        if (string.IsNullOrEmpty(raspored))
        {
            return result;
        }

        raspored = raspored.Trim();
        raspored = raspored[1..^1];
        
        int zagrade = 0;
        StringBuilder current = new StringBuilder("");

        foreach (char c in raspored)
        {
            if (c == '(')
            {
                zagrade++;
                if (zagrade == 1)
                    continue;
            }

            if (c == ')')
            {
                zagrade--;
                if (zagrade == 0)
                {
                    var parsedRed = ParseRed(current.ToString());
                    result.Add(parsedRed);
                    current.Clear();
                    continue;
                }
            }

            if (zagrade >= 2)
                current.Append(c);
        }

        return result;
    }

    private static List<Seat> ParseRed(string red)
    {
        var parsedRed = new List<Seat>();

        string[] pairs = red.Split(',');

        foreach (var p in pairs)
        {
            var trimmed = p.Trim();

            if (string.IsNullOrEmpty(trimmed))
                parsedRed.Add(new Seat(null, null));
            
            string[] parts = p.Split(':');

            if (parts.Length == 2)
            {
                if (int.TryParse(parts[0], out int seatId))
                {
                    parsedRed.Add(new Seat(seatId, parts[1]));
                }
            }
        }

        return parsedRed;
    }
}