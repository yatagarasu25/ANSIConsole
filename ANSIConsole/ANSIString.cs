namespace ANSIConsole;

using System;
using System.Collections.Generic;
using System.Drawing;

public class ANSIString
{
	private readonly string _text;
	private string _hyperlink;
	private Color? _colorForeground;
	private Color? _colorBackground;
	private ConsoleColor? _consoleColorForeground;
	private ConsoleColor? _consoleColorBackground;
	private float? _opacity;
	private ANSIFormatting _formatting;

	public ANSIString(string text)
	{
		_text = text;
		_formatting = ANSIFormatting.Clear;
	}

	internal ANSIString AddFormatting(ANSIFormatting add)
	{
		_formatting |= add;
		if (_formatting.HasFlag(ANSIFormatting.UpperCase | ANSIFormatting.LowerCase))
			throw new ArgumentException("formatting cannot include both UpperCase and LowerCase!",
				nameof(_formatting));
		return this;
	}

	internal ANSIString RemoveFormatting(ANSIFormatting rem)
	{
		_formatting &= ~rem;
		return this;
	}

	internal Color GetForegroundColor() => _colorForeground ?? FromConsoleColor(Console.ForegroundColor);
	internal Color GetBackgroundColor() => _colorBackground ?? FromConsoleColor(Console.BackgroundColor);

	internal ANSIString SetForegroundColor(Color color)
	{
		_colorForeground = color;
		_consoleColorForeground = null;
		return this;
	}

	internal ANSIString SetForegroundColor(ConsoleColor color)
	{
		_colorForeground = null;
		_consoleColorForeground = ((int)color != -1) ? color : null;
		return this;
	}

	internal ANSIString SetBackgroundColor(Color color)
	{
		_colorBackground = color;
		_consoleColorBackground = null;
		return this;
	}

	internal ANSIString SetBackgroundColor(ConsoleColor color)
	{
		_colorBackground = null;
		_consoleColorBackground = ((int)color != -1) ? color : null;
		return this;
	}

	internal ANSIString SetOpacity(float opacity)
	{
		_opacity = opacity;
		return this;
	}

	internal ANSIString SetHyperlink(string link)
	{
		_hyperlink = link;
		return this;
	}

	public override string ToString()
	{
		if (!ANSIInitializer.Enabled || _formatting == ANSIFormatting.None) return _text;
		string result = _text;
		if (_formatting.HasFlag(ANSIFormatting.UpperCase)) result = result.ToUpper();
		if (_formatting.HasFlag(ANSIFormatting.LowerCase)) result = result.ToLower();

		List<byte> parameters = new List<byte>();
		if (_formatting.HasFlag(ANSIFormatting.Bold)) parameters.Add(ANSI.nBold);
		if (_formatting.HasFlag(ANSIFormatting.Faint)) parameters.Add(ANSI.nFaint);
		if (_formatting.HasFlag(ANSIFormatting.Italic)) parameters.Add(ANSI.nItalic);
		if (_formatting.HasFlag(ANSIFormatting.Underlined)) parameters.Add(ANSI.nUnderlined);
		if (_formatting.HasFlag(ANSIFormatting.Overlined)) parameters.Add(ANSI.nOverlined);
		if (_formatting.HasFlag(ANSIFormatting.Blink)) parameters.Add(ANSI.nBlink);
		if (_formatting.HasFlag(ANSIFormatting.Inverted)) parameters.Add(ANSI.nInverted);
		if (_formatting.HasFlag(ANSIFormatting.StrikeThrough)) parameters.Add(ANSI.nStrikeThrough);

		if (_consoleColorForeground != null) parameters.Add(ANSI.ForegroundColor4bit(_consoleColorForeground.Value));
		if (_consoleColorBackground != null) parameters.Add(ANSI.BackgroundColor4bit(_consoleColorBackground.Value));

		if (parameters.Count > 0)
			result = ANSI.SGR(parameters.ToArray()) + result;

		if (_colorForeground != null || _colorBackground != null)
		{
			if (_opacity != null)
				result = ANSI.Foreground(
					Interpolate(_colorBackground.Value, _colorForeground.Value, _opacity.Value)
				) + result;
			else if (_colorForeground != null) result = ANSI.Foreground(_colorForeground.Value) + result;
			else if (_colorBackground != null) result = ANSI.Background(_colorBackground.Value) + result;
		}

		if (_hyperlink != null) result = ANSI.Hyperlink(result, _hyperlink);

		if (_formatting.HasFlag(ANSIFormatting.Clear)) result += ANSI.Clear;
		return result;
	}

	public static Color FromConsoleColor(ConsoleColor color)
	{
		try {
			return Color.FromArgb(ConsoleColors[(int)color]);
		} catch {
			throw new ArgumentOutOfRangeException(nameof(color), $"{color} is not a valid color");
		}
	}

	internal static readonly int[] ConsoleColors = {
		0x000000, //Black = 0
		0x000080, //DarkBlue = 1
		0x008000, //DarkGreen = 2
		0x008080, //DarkCyan = 3
		0x800000, //DarkRed = 4
		0x800080, //DarkMagenta = 5
		0x808000, //DarkYellow = 6
		0xC0C0C0, //Gray = 7
		0x808080, //DarkGray = 8
		0x0000FF, //Blue = 9
		0x00FF00, //Green = 10
		0x00FFFF, //Cyan = 11
		0xFF0000, //Red = 12
		0xFF00FF, //Magenta = 13
		0xFFFF00, //Yellow = 14
		0xFFFFFF  //White = 15
	};

	public static Color Interpolate(Color from, Color to, float percentage)
	{
		float rtlPercentage = 1 - percentage;
		int r = (byte)(rtlPercentage * from.R + percentage * to.R);
		int g = (byte)(rtlPercentage * from.G + percentage * to.G);
		int b = (byte)(rtlPercentage * from.B + percentage * to.B);

		return Color.FromArgb(r, g, b);
	}
}
