//************************************************************************************************
// Copyright © 2016 Steven M Cohn.  All rights reserved.
//************************************************************************************************

namespace xort
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Xml.Linq;


	/// <summary>
	/// Sorts the contents of an XML file based on the contents of a given template file.
	/// This can be used to align two versions of an XML file to enable textual comparison
	/// to find differences between the versions.
	/// </summary>
	/// <remarks>
	/// Usage: xort template.xml unsorted.xml [debug]
	/// </remarks>

	class Program
	{
		private string templatePath;
		private string unsortedPath;
		private bool debugging;


		static void Main (string[] args)
		{
			new Program().Run(args);
		}


		private void Run (string[] args)
		{
			ParseArguments(args);

			var template = XElement.Load(templatePath);
			var unsorted = XElement.Load(unsortedPath);

			Sort(template, unsorted);
			Save(unsorted);
		}


		private void Sort (XElement template, XElement unsorted)
		{
			var ordered = new List<XElement>();
			var buffer = unsorted.Elements().ToList();

			foreach (var element in template.Elements())
			{
				var candidate = buffer.MaxBy(e => e.Equivalence(element));
				if (candidate != null)
				{
					if ((element.Elements().Count() > 0) && (candidate.Elements().Count() > 0))
					{
						Sort(element, candidate);
					}

					ordered.Add(candidate);
					buffer.Remove(candidate);
				}
			}

			unsorted.ReplaceNodes(ordered.Concat(buffer));
        }


		private void Save (XElement unsorted)
		{
			if (debugging)
			{
				Console.OutputEncoding = Encoding.UTF8;
				unsorted.Save(Console.Out, SaveOptions.None);
			}
			else
			{
				var path =
					Path.GetFileNameWithoutExtension(unsortedPath) + "_xorted" +
					Path.GetExtension(unsortedPath);

				if (File.Exists(path))
				{
					File.Delete(path);
				}

				unsorted.Save(path, SaveOptions.None);
			}
		}


		private void ParseArguments (string[] args)
		{
			if (args.Length < 2)
			{
				ShowUsage();
			}

			templatePath = Path.GetFullPath(args[0]);
			if (!File.Exists(templatePath))
			{
				Console.WriteLine("Could not find template file '" + templatePath + "'");
				ShowUsage();
			}

			unsortedPath = Path.GetFullPath(args[1]);
			if (!File.Exists(unsortedPath))
			{
				Console.WriteLine("Could not find work file '" + unsortedPath + "'");
				ShowUsage();
			}

			debugging = (args.Length > 2) && args[2].EqualsICIC("debug");
		}


		private void ShowUsage ()
		{
			Console.WriteLine("Usage: xort <template.xml> <unsorted.xml> [debug]");
			Environment.Exit(1);
		}
	}
}
