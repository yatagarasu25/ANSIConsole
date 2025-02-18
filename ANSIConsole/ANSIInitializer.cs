﻿namespace ANSIConsole;

using System;
using System.Runtime.InteropServices;
using static NativeFunctions;

public class ANSIInitializer : IDisposable
{
	private static bool? _enabled;
	public static bool Enabled {
		get {
			if (_enabled != null) return (bool)_enabled;
			_enabled = Environment.GetEnvironmentVariable("NO_COLOR") == null;
			return Enabled;
		}
		set => _enabled = value;
	}

	protected uint originalConsoleMode;

	/// <summary>
	/// Run once before using the console.
	/// You may not need to initialize the ANSI console mode.
	/// </summary>
	/// <returns>true if initialization was successful</returns>
	public ANSIInitializer(bool printError = true)
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
			if (!GetConsoleMode(iStdOut, out uint originalConsoleMode))
			{
				if (printError) Console.WriteLine("failed to get output console mode");
				return;
			}

			var consoleMode = originalConsoleMode
				| ENABLE_PROCESSED_OUTPUT | ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
			if (!SetConsoleMode(iStdOut, consoleMode))
			{
				if (printError) Console.WriteLine($"failed to set output console mode, error code: {GetLastError()}");
				return;
			}
		}

		Enabled = true;
	}

	public void Dispose()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
			SetConsoleMode(iStdOut, originalConsoleMode);
		}
	}
}
