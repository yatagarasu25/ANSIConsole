namespace ANSIConsole;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using SystemEx;

public struct ANSICharacter
{
	public char c = '\x00';
	public Color? colorForeground = null;
	public Color? colorBackground = null;
	public ConsoleColor? consoleColorForeground = null;
	public ConsoleColor? consoleColorBackground = null;
	public float? opacity = null;
	public ANSIFormatting formatting = ANSIFormatting.None;

	public ANSICharacter()
	{
	}

	public ANSICharacter(char c)
	{
		this.c = c;
	}

	public ANSICharacter(ANSIFormatting formatting)
	{
		this.formatting = formatting;
	}

	public ANSICharacter dC(char c)
		=> MemberwiseClone()
			.Let(o => { var ac = (ANSICharacter)o; ac.c = c; return ac; });


	public override string ToString()
	{
		string result = string.Empty;
		if (!ANSIInitializer.Enabled || formatting == ANSIFormatting.None) return result;

		if (formatting.HasFlag(ANSIFormatting.UpperCase)) result = result.ToUpper();
		if (formatting.HasFlag(ANSIFormatting.LowerCase)) result = result.ToLower();

		List<byte> parameters = new List<byte>();
		if (formatting.HasFlag(ANSIFormatting.Bold)) parameters.Add(ANSI.nBold);
		if (formatting.HasFlag(ANSIFormatting.Faint)) parameters.Add(ANSI.nFaint);
		if (formatting.HasFlag(ANSIFormatting.Italic)) parameters.Add(ANSI.nItalic);
		if (formatting.HasFlag(ANSIFormatting.Underlined)) parameters.Add(ANSI.nUnderlined);
		if (formatting.HasFlag(ANSIFormatting.Overlined)) parameters.Add(ANSI.nOverlined);
		if (formatting.HasFlag(ANSIFormatting.Blink)) parameters.Add(ANSI.nBlink);
		if (formatting.HasFlag(ANSIFormatting.Inverted)) parameters.Add(ANSI.nInverted);
		if (formatting.HasFlag(ANSIFormatting.StrikeThrough)) parameters.Add(ANSI.nStrikeThrough);

		if (consoleColorForeground != null) parameters.Add(ANSI.ForegroundColor4bit(consoleColorForeground.Value));
		if (consoleColorBackground != null) parameters.Add(ANSI.BackgroundColor4bit(consoleColorBackground.Value));

		if (parameters.Count > 0)
			result = ANSI.SGR(parameters.ToArray()) + result;

		if (colorForeground != null || colorBackground != null)
		{
			if (opacity != null)
				result = ANSI.Foreground(
					ANSIString.Interpolate(colorBackground.Value, colorForeground.Value, opacity.Value)
				) + result;
			else if (colorForeground != null) result = ANSI.Foreground(colorForeground.Value) + result;
			else if (colorBackground != null) result = ANSI.Background(colorBackground.Value) + result;
		}

		if (formatting.HasFlag(ANSIFormatting.Clear)) result += ANSI.Clear;
		return result;
	}

	public static bool TryParse(string str, out ANSICharacter c)
	{
		var r = new Regex(ANSI.ESCRegex, RegexOptions.Compiled);

		c = new ANSICharacter();

		return false;
	}

	public static bool TryParse(Match m, out ANSICharacter c)
	{
		c = new ANSICharacter();

		if (m.Groups["csicode"].Value != "m")
			return false;

		c = m.Groups["data"].Value switch {
			"0" => new ANSICharacter(ANSIFormatting.Clear),
			"1" => new ANSICharacter(ANSIFormatting.Bold),
			_ => new ANSICharacter()
		};

		return true;
	}
}
