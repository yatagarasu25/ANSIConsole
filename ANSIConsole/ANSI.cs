namespace ANSIConsole;

using System;
using System.Drawing;
using System.Linq;

public static class ANSI
{
	// Source: https://github.com/spectreconsole/spectre.console/blob/3c5b98123b4c9cd50a37da25ca9a3fd34ac7f479/src/Spectre.Console/Internal/Backends/Ansi/AnsiSequences.cs#L32
	/// <summary>
	/// The ASCII escape character (decimal 27).
	/// </summary>
	public const string ESC = "\u001b";

	/// <summary>
	/// Introduces a control sequence that uses 8-bit characters.
	/// </summary>
	public const string CSI = ESC + "[";
	public static string SGR(params byte[] codes) => $"{CSI}{string.Join(";", codes.Select(c => c.ToString()))}m";

	public static string Clear = SGR(0);
	public static string Bold = SGR(1);
	public static string Faint = SGR(2);
	public static string Italic = SGR(3);
	public static string Underlined = SGR(4);
	public static string Blink = SGR(5);
	public static string Inverted = SGR(7);
	public static string StrikeThrough = SGR(9);
	public static string Overlined = SGR(53);

	public static byte[] ConsoleColors = new byte[] {
		30, // Black,
		34, // DarkBlue,
		32, // DarkGreen,
		36, // DarkCyan,
		31, // DarkRed,
		35, // DarkMagenta,
		33, // DarkYellow,
		37, // Gray,
		90, // DarkGray,
		94, // Blue,
		92, // Green,
		96, // Cyan,
		91, // Red,
		95, // Magenta,
		93, // Yellow,
		97, // White
	};

	public static string Color(ConsoleColor? fg, ConsoleColor? bg)
		=> SGR(new byte[] { fg != null ? ConsoleColors[(int)fg.Value] : (byte)0
			              , bg != null ? (byte)(ConsoleColors[(int)bg.Value] + 10) : (byte)0 }
			.Where(c => c != 0).ToArray());
	public static string Foreground(ConsoleColor color) => SGR(ConsoleColors[(int)color]);
	public static string Background(ConsoleColor color) => SGR((byte)(ConsoleColors[(int)color] + 10));
	public static string Foreground(Color color) => SGR(38, 2, color.R, color.G, color.B);
	public static string Background(Color color) => SGR(48, 2, color.R, color.G, color.B);
	public static string Hyperlink(string text, string link) => $"\u001b]8;;{link}\a{text}\u001b]8;;\a";
}
