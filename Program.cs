namespace TimerMcTimeface
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.IO;

	class Program
	{
		const string timeFile = "time.txt";

		static void UpdateFile(string fileName, TimeSpan elapsed)
		{
			if (!File.Exists(timeFile))
			{
				File.Create(timeFile);
			}

			var data = File.ReadAllLines(timeFile)
				.Where(line => !string.IsNullOrEmpty(line))
				.Select(line => line.Split(new [] { ';' }, 2))
				.Select(pair => new Tuple<string, TimeSpan>(pair[0], TimeSpan.Parse(pair[1])))
				.ToDictionary(tup => tup.Item1, tup => tup.Item2);

			var name = fileName.ToLower();

			if (!data.ContainsKey(name))
			{
				data.Add(name, TimeSpan.Zero);
			}

			data[name] = data[name].Add(elapsed);

			File.Delete(timeFile);

			File.WriteAllLines(
				timeFile,
				data.Keys
					.Select(key => new Tuple<string, string>(key, data[key].ToString()))
					.Select(tup => tup.Item1 + ";" + tup.Item2));
		}

		static void Main(string[] args)
		{
			if (args.Length <= 0)
			{
				Console.WriteLine("Missing argument 1: path of file to run.");
				return;
			}

			var name = args[0];
			var startInfo = new ProcessStartInfo { FileName = name };
			var start = DateTime.Now;

			using (var process = Process.Start(startInfo))
			{
				process.WaitForExit();

				var elapsed = DateTime.Now - start;
				UpdateFile(name, elapsed);
			}
		}
	}
}