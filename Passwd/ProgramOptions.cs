using System;
using System.Linq;
using System.Reflection;
using CommandLine;
using CommandLine.Text;

namespace Passwd
{
	internal class ProgramOptions : IChangePasswordOptions

	{
		[Option('v', "verbose", HelpText = "Outputs verbose error information")]
		public bool Verbose { get; set; }

		[Option('u', "username", HelpText = "Username on the form DOMAIN\\UserName", Required = true)]
		public string Username { get; set; }

		[Option('s', "usessl", HelpText = "Uses Secure Sockets layer to connect to the domain controller", Required = false)]
		public bool UseSsl { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			var assemblyDescription =
				this.GetType()
				.Assembly
				.GetCustomAttributes(typeof (AssemblyDescriptionAttribute))
				.Cast<AssemblyDescriptionAttribute>()
				.First()?.Description;

			return (assemblyDescription == null ? string.Empty : assemblyDescription + Environment.NewLine) 
			+ HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
		}
	}
}