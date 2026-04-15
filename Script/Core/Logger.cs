using System;
using Godot;
using SystemEnv = System.Environment;
using File = System.IO.File;
namespace MirrorWorldDemo.Script.Core;

public static class Logger
{
	public static bool ShowInConsole = true;
	public static bool WriteToFile = true;

	public enum Level { Debug, Info, Warn, Error }

	// ── 带格式化参数的重载 ──────────────────────────────
	public static void Debug(string format, params object[] args)
		=> Write(string.Format(format, args), Level.Debug);

	public static void Info(string format, params object[] args)
		=> Write(string.Format(format, args), Level.Info);

	public static void Warn(string format, params object[] args)
		=> Write(string.Format(format, args), Level.Warn);

	public static void Error(string format, params object[] args)
		=> Write(string.Format(format, args), Level.Error);

	// ── 纯字符串重载 ───────────────────────────────────
	public static void Debug(string message) => Write(message, Level.Debug);
	public static void Info(string message)  => Write(message, Level.Info);
	public static void Warn(string message)  => Write(message, Level.Warn);
	public static void Error(string message) => Write(message, Level.Error);

	// ── 核心方法 ───────────────────────────────────────
	public static void Write(string message, Level level = Level.Info)
	{
		string formatted = $"[{level.ToString().ToUpper()}] [{DateTime.Now:HH:mm:ss}] {message}";

		switch (level)
		{
			case Level.Debug:
			case Level.Info:
				if (ShowInConsole) GD.Print(formatted);
				break;

			case Level.Warn:
				if (ShowInConsole) GD.PushWarning(formatted);
				break;

			case Level.Error:
				if (ShowInConsole) GD.PushError(formatted);
				else GD.PrintRich($"[color=red]{formatted}[/color]");
				break;
		}

		if (WriteToFile) AppendToFile(formatted);
	}

	// ── 文件写入 ───────────────────────────────────────
	private static void AppendToFile(string line)
	{
		try
		{
			var path = ProjectSettings.GlobalizePath("user://game.log");
			File.AppendAllText(path, line + SystemEnv.NewLine);
		}
		catch (Exception ex)
		{
			GD.PrintErr($"[Logger] 写文件失败: {ex.Message}");
		}
	}
}
