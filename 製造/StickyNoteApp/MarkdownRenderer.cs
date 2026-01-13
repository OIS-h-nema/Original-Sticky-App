using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using Media = System.Windows.Media;

namespace StickyNoteApp;

public static class MarkdownRenderer
{
    private static readonly Regex BoldPattern =
        new(@"\*\*(.+?)\*\*", RegexOptions.Compiled);

    private static readonly Regex ItalicPattern =
        new(@"(?<!\*)\*([^*]+)\*(?!\*)", RegexOptions.Compiled);

    private static readonly Regex BulletPattern =
        new(@"^\s*[-*]\s+(.+)$", RegexOptions.Compiled);

    private static readonly Regex NumberPattern =
        new(@"^\s*(\d+)\.\s+(.+)$", RegexOptions.Compiled);

    public static IEnumerable<Inline> Render(
        string markdown,
        Media.FontFamily fontFamily,
        double fontSize,
        Media.Brush fontColor)
    {
        markdown ??= string.Empty;
        var lines = markdown.Replace("\r\n", "\n").Split('\n');
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            foreach (var inline in RenderLine(line, fontFamily, fontSize, fontColor))
            {
                yield return inline;
            }

            if (i < lines.Length - 1)
            {
                yield return new LineBreak();
            }
        }
    }

    private static IEnumerable<Inline> RenderLine(
        string line,
        Media.FontFamily fontFamily,
        double fontSize,
        Media.Brush fontColor)
    {
        var indentPrefix = string.Empty;
        var content = line;

        var bulletMatch = BulletPattern.Match(line);
        var numberMatch = NumberPattern.Match(line);

        if (bulletMatch.Success)
        {
            indentPrefix = "　";
            content = $"・ {bulletMatch.Groups[1].Value}";
        }
        else if (numberMatch.Success)
        {
            indentPrefix = "　";
            content = $"{numberMatch.Groups[1].Value}. {numberMatch.Groups[2].Value}";
        }

        if (!string.IsNullOrEmpty(indentPrefix))
        {
            yield return CreateRun(indentPrefix, fontFamily, fontSize, fontColor);
        }

        foreach (var inline in RenderInlineMarkdown(content, fontFamily, fontSize, fontColor))
        {
            yield return inline;
        }
    }

    private static IEnumerable<Inline> RenderInlineMarkdown(
        string text,
        Media.FontFamily fontFamily,
        double fontSize,
        Media.Brush fontColor)
    {
        var index = 0;
        while (index < text.Length)
        {
            var boldMatch = BoldPattern.Match(text, index);
            var italicMatch = ItalicPattern.Match(text, index);

            var nextMatch = SelectNextMatch(boldMatch, italicMatch);
            if (!nextMatch.Success)
            {
                yield return CreateRun(text[index..], fontFamily, fontSize, fontColor);
                yield break;
            }

            if (nextMatch.Index > index)
            {
                yield return CreateRun(text[index..nextMatch.Index], fontFamily, fontSize, fontColor);
            }

            var content = nextMatch.Groups[1].Value;
            var run = CreateRun(content, fontFamily, fontSize, fontColor);
            if (nextMatch == boldMatch)
            {
                run.FontWeight = FontWeights.Bold;
            }
            else
            {
                run.FontStyle = FontStyles.Italic;
            }

            yield return run;
            index = nextMatch.Index + nextMatch.Length;
        }
    }

    private static Match SelectNextMatch(Match boldMatch, Match italicMatch)
    {
        if (!boldMatch.Success)
        {
            return italicMatch;
        }

        if (!italicMatch.Success)
        {
            return boldMatch;
        }

        return boldMatch.Index <= italicMatch.Index ? boldMatch : italicMatch;
    }

    private static Run CreateRun(string text, Media.FontFamily fontFamily, double fontSize, Media.Brush fontColor)
    {
        return new Run(text)
        {
            FontFamily = fontFamily,
            FontSize = fontSize,
            Foreground = fontColor
        };
    }
}
