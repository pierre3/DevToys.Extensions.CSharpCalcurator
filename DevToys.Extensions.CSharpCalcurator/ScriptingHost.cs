using CsvHelper;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace DevToys.Extensions.CSharpCalculator;

public class ScriptingHost(string source, CultureInfo cultureInfo, StringBuilder output) : IDisposable
{
    public void WriteLine(string s) => output.AppendLine(s);
    public void Write(string s) => output.Append(s);
    public CultureInfo CultureInfo { get => cultureInfo; }

    private StringReader? _strReader;
    private CsvReader? _csvReader;
    public JsonSerializerOptions JsonSerializerOptions { get; } = new() { WriteIndented = true };

    private CsvReader InitializeCsvReader()
    {
        _strReader?.Dispose();
        _strReader?.Dispose();
        _strReader = new StringReader(source);
        _csvReader = new CsvReader(_strReader, cultureInfo);
        return _csvReader;
    }

    public IEnumerable<int> ids { get => IntData(); }
    public IEnumerable<double> fds { get => FloatData(); }
    public IEnumerable<decimal> dds { get => DecimalData(); }
    public IEnumerable<string> sds { get => StringData(); }

    public void ToCsv<T>(IEnumerable<T> records)
    {
        var sb = new StringBuilder();
        using var strWriter = new StringWriter(sb);
        using var csvWriter = new CsvWriter(strWriter, cultureInfo);
        csvWriter.WriteRecords(records);
        Write(sb.ToString());
    }

    public void ToJson<T>(IEnumerable<T> records)
    {
        Write(JsonSerializer.Serialize(records, JsonSerializerOptions));
    }

    public void Join<T>(IEnumerable<T> records, string separator)
    {
        Write(string.Join(separator, records));
    }

    public void Join<T>(IEnumerable<T> records, string separator, string format) where T : IFormattable
    {
        Write(string.Join(separator, records.Select(x => x.ToString(format, null))));
    }

    public void Join<T>(IEnumerable<T> records, string separator, Func<T, string> format)
    {
        Write(string.Join(separator, records.Select(x => format(x))));
    }


    public void Join<T>(IEnumerable<T> records, string separator, string format, IFormatProvider formatProvider) where T : IFormattable
    {
        Write(string.Join(separator, records.Select(x => x.ToString(format, formatProvider))));
    }

    public void Value<T>(T value)
    {
        Write(value?.ToString() ?? "");
    }
    public void Value<T>(T value, string format) where T : IFormattable
    {
        Write(value.ToString(format, null));
    }

    public void Value<T>(T value, string format, IFormatProvider formatProvider) where T : IFormattable
    {
        Write(value.ToString(format, formatProvider));
    }
    public void Value<T>(T value, Func<T, string> format)
    {
        Write(format(value));
    }

    public void ValueLine<T>(T value)
    {
        WriteLine(value?.ToString() ?? "");
    }

    public void ValueLine<T>(T value, string format) where T : IFormattable
    {
        WriteLine(value.ToString(format, null));
    }

    public void ValueLine<T>(T value, string format, IFormatProvider formatProvider) where T : IFormattable
    {
        WriteLine(value.ToString(format, formatProvider));
    }

    public void ValueLine<T>(T value, Func<T, string> format)
    {
        WriteLine(format(value));
    }

    public void Text<T>(T _, string line)
    {
        Write(line);
    }


    public void TextLine<T>(T _, string line)
    {
        WriteLine(line);
    }



    public IEnumerable<T> Data<T>()
    {
        try
        {
            return InitializeCsvReader().GetRecords<T>();
        }
        catch
        {
            return [];
        }
    }

    public IEnumerable<T> Data<T>(T _) => Data<T>();

    private IEnumerable<int> IntData()
    {
        foreach (var range in SplitBy(Environment.NewLine, source))
        {
            yield return int.Parse(source[range]);
        }
    }

    private IEnumerable<double> FloatData()
    {
        foreach (var range in SplitBy(Environment.NewLine, source))
        {
            yield return double.Parse(source[range]);
        }
    }

    private IEnumerable<decimal> DecimalData()
    {
        foreach (var range in SplitBy(Environment.NewLine, source))
        {
            yield return decimal.Parse(source[range]);
        }
    }

    private IEnumerable<string> StringData()
    {
        foreach (var range in SplitBy(Environment.NewLine, source))
        {
            yield return source[range].ToString();
        }
    }

    internal static Range[] SplitBy(string separator, ReadOnlySpan<char> span)
    {
        var count = span.Count(separator) + 1;
        var buffer = new Range[count];
        var resultCount = span.Split(
            buffer,
            separator,
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return buffer[0..resultCount];
    }

    internal static string GetHeadersFromDefinition(string source)
    {
        return string.Join(",", EnumerateHeaders(source));

        static IEnumerable<string> EnumerateHeaders(string source)
        {
            foreach (var r in SplitBy(",", source))
            {
                var column = source[r];
                var ranges = SplitBy(" ", column);
                if (ranges.Length > 1)
                {
                    yield return column[ranges[1]];
                }
            }
        }
    }

    public void Dispose()
    {
        _strReader?.Dispose();
        _csvReader?.Dispose();
    }
}
