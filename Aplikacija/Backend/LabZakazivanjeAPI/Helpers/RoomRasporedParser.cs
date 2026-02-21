using System.Text;

namespace LabZakazivanjeAPI.Helpers;
public record Seat(int? Id, string? IP);

public class RoomRasporedParser
{

    public static List<int> GetSeatIds(string raspored)
    {
        List<int> seatIds = [];
        var seats = ParseRaspored(raspored);

        foreach (var row in seats)
        {
            foreach (var s in row)
            {
                if (s.Id != null)
                    seatIds.Add(s.Id.Value);
            }
        }

        return seatIds;
    }

    public static List<Seat> GetSeatIps(string raspored)
    {
        List<Seat> seatIds = [];
        var seats = ParseRaspored(raspored);

        foreach (var row in seats)
        {
            foreach (var s in row)
            {
                if (s.Id != null && s.IP != null)
                    seatIds.Add(new Seat(s.Id, s.IP));
            }
        }

        return seatIds;
    }

    public static List<List<Seat>> ParseRaspored(string raspored)
    {
        // ((id:ip, id:ip), (id:ip, id:ip, id:ip), (id:ip),)

        var result = new List<List<Seat>>();

        if (string.IsNullOrEmpty(raspored))
        {
            return result;
        }

        int zagrade = 0;
        raspored = raspored.Trim();
        raspored = raspored[1..^1];

        StringBuilder current = new StringBuilder("");

        foreach (char c in raspored)
        {
            if (c == '(')
            {
                zagrade++;
                continue;
            }

            if (c == ')')
            {
                var parsedRed = ParseRed(current.ToString());
                result.Add(parsedRed);
                current.Clear();
                continue;
            }

            if (zagrade == 0 && c == ',')
                continue;
            
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
            {
                parsedRed.Add(new Seat(null, null));
                continue;
            }

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