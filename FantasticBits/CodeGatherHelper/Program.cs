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

			List<string> content = new List<string>();

			DirectoryInfo info = new DirectoryInfo(SOURCE_DIRECTORY);
			foreach(FileInfo file in info.GetFiles("*.cs", SearchOption.AllDirectories))
			{
				if (excludes.Contains(file.Name) || file.FullName.Contains("\\obj\\") || file.FullName.Contains("\\bin\\"))
				{
					continue;
				}



				using (Stream fileStream = file.OpenRead())
				{
					using (TextReader reader = new StreamReader(fileStream))
					{
						content.Add($"// File {file.Name}");
						content.Add(reader.ReadToEnd());
						content.Add("");
						content.Add("");
					}
				}
			}

			File.WriteAllText(OUT_FILE, string.Join("\n", content));
		}
	}
}
