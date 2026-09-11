namespace Arribbaa.MimicParty.ModdingCore.Native;

public sealed class MemoryPattern
{
    private MemoryPattern(byte[] values, bool[] exact, string text)
    {
        Values = values;
        Exact = exact;
        Text = text;

        AnchorIndex = -1;
        for (int i = exact.Length - 1; i >= 0; i--)
        {
            if (exact[i])
            {
                AnchorIndex = i;
                break;
            }
        }

        if (AnchorIndex < 0)
            throw new ArgumentException("A pattern must contain at least one fixed byte.", nameof(text));
    }

    public byte[] Values { get; }
    public bool[] Exact { get; }
    public string Text { get; }
    public int Length => Values.Length;
    public int AnchorIndex { get; }

    public static MemoryPattern Parse(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Pattern is empty.", nameof(text));

        string[] tokens = text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        var values = new byte[tokens.Length];
        var exact = new bool[tokens.Length];

        for (int i = 0; i < tokens.Length; i++)
        {
            string token = tokens[i];
            if (token is "?" or "??")
            {
                values[i] = 0;
                exact[i] = false;
                continue;
            }

            if (token.Length != 2 || !byte.TryParse(token, System.Globalization.NumberStyles.HexNumber, null, out byte value))
                throw new FormatException($"Invalid pattern token '{token}' at index {i}.");

            values[i] = value;
            exact[i] = true;
        }

        return new MemoryPattern(values, exact, text);
    }
}
