namespace ANSIConsole;

using System;
using System.Runtime.InteropServices;

internal static class NativeFunctions
{
	public static readonly int STD_OUTPUT_HANDLE = -11;
	public static readonly uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
	public static readonly uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

	[DllImport("kernel32.dll")]
	public static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

	[DllImport("kernel32.dll")]
	public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern IntPtr GetStdHandle(int nStdHandle);

	[DllImport("kernel32.dll")]
	public static extern uint GetLastError();
}
