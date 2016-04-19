using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Passwd.SystemDirectoryServicesImpl;

namespace Passwd
{
	class Program
	{

		static int Main(string[] args)
		{
			EmbeddedAssembly.Init();
			
			return ExecuteFromCommandLine(args);
		}

		private static int ExecuteFromCommandLine(string[] args)
		{
			var options = new ProgramOptions();
			if (!CommandLine.Parser.Default.ParseArguments(args, options))
			{
				return ReturnCodes.ArgumentsError;
			}
			try
			{
				var p = new PasswordChanger(
					new CommandLinePasswordChangerSource(), 
					new PrincipalDomainContextFactory());
				p.ChangePassword(options);
				Console.Error.WriteLine("Password changed.");
				return ReturnCodes.Ok;
			}
			catch (Exception e)
			{
				if (options.Verbose)
				{
					Console.Error.WriteLine(e);
				}
				else
				{
					Console.Error.WriteLine(e.Message);
				}

				return ReturnCodes.ExecutionError;
			}
		}

		private static class ReturnCodes
		{
			public const int ArgumentsError = 1;
			public const int ExecutionError = 2;
			public const int Ok = 0;
		}

		/// <summary>
		/// Enables loading assemblies from embedded resources
		/// </summary>
		/// <remarks>
		/// Based on http://www.codeproject.com/Articles/528178/Load-DLL-From-Embedded-Resource
		/// </remarks>
		private static class EmbeddedAssembly
		{
			public static void Init()
			{
				var embeddedDlls = Assembly.GetExecutingAssembly().GetManifestResourceNames()
					.Where(x => x.EndsWith(".dll"));

				foreach (var embeddedDll in embeddedDlls)
				{
					Load(embeddedDll);
				}

				AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
			}

			static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
			{
				return Get(args.Name);
			}

			static Dictionary<string, Assembly> dic;

			public static void Load(string embeddedResource)
			{
				if (dic == null)
					dic = new Dictionary<string, Assembly>();

				byte[] ba = null;
				Assembly asm = null;
				Assembly curAsm = Assembly.GetExecutingAssembly();

				using (Stream stm = curAsm.GetManifestResourceStream(embeddedResource))
				{
					// Either the file is not existed or it is not mark as embedded resource
					if (stm == null)
						throw new Exception(embeddedResource + " is not found in Embedded Resources.");

					// Get byte[] from the file from embedded resource
					ba = new byte[(int)stm.Length];
					stm.Read(ba, 0, (int)stm.Length);
					try
					{
						asm = Assembly.Load(ba);

						// Add the assembly/dll into dictionary
						dic.Add(asm.FullName, asm);
						return;
					}
					catch
					{
						// Purposely do nothing
						// Unmanaged dll or assembly cannot be loaded directly from byte[]
						// Let the process fall through for next part
					}
				}


				var tempFile = Path.GetTempFileName();

				System.IO.File.WriteAllBytes(tempFile, ba);

				asm = Assembly.LoadFile(tempFile);

				dic.Add(asm.FullName, asm);
			}

			public static Assembly Get(string assemblyFullName)
			{
				if (dic == null || dic.Count == 0)
					return null;

				if (dic.ContainsKey(assemblyFullName))
					return dic[assemblyFullName];

				return null;
			}
		}
	}
}
