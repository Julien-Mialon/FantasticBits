using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGatherHelper
{
	class Program
	{
		static void Main(string[] args)
		{
			string[] excludes =
			{
				"AssemblyInfo.cs"
			};
			const string SOURCE_DIRECTORY = "FantasticBits";
			const string OUT_FILE = "run.cs";

			List<string> usings = new List<string>();
			List<string> content = new List<string>();

			DirectoryInfo info = new DirectoryInfo(SOURCE_DIRECTORY);
			foreach(FileInfo file in info.GetFiles("*.cs", SearchOption.AllDirectories))
			{
				if (excludes.Contains(file.Name) || file.FullName.Contains("\\obj\\") || file.FullName.Contains("\\bin\\"))
				{
					continue;
				}

				content.Add($"// File {file.Name}");

				List<string> lines = File.ReadAllLines(file.FullName).ToList();
				usings.AddRange(lines.Where(x => x.StartsWith("using ")).Select(x => x.Trim()));
				content.AddRange(lines.Where(x => !x.StartsWith("using ")));
				content.Add("");
				content.Add("");
			}

			usings = usings.Distinct().ToList();
			usings.Add("");
			usings.Add("");
			content.InsertRange(0, usings);

			File.WriteAllText(OUT_FILE, string.Join("\n", content));
		}
	}
}
