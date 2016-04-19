using System;
using System.DirectoryServices.AccountManagement;

namespace Passwd
{
	internal class PasswordChanger
	{
		private readonly IPasswordChangerSource commandLinePasswordChangerSource;
		private readonly IDomainContextFactory domainContextFactory;
		public PasswordChanger(
			IPasswordChangerSource commandLinePasswordChangerSource, 
			IDomainContextFactory domainContextFactory)
		{
			this.commandLinePasswordChangerSource = commandLinePasswordChangerSource;
			this.domainContextFactory = domainContextFactory;
		}

		internal void ChangePassword(ProgramOptions options)
		{
			var optionsUserName = options.Username.Split('\\');
			if (optionsUserName.Length != 2)
			{
				throw new ArgumentException("Username parameter must be on the form DOMAIN\\Username");
			}

			var domain = optionsUserName[0];
			var userName = optionsUserName[1];

			using (var context = this.domainContextFactory.Create(domain, options.UseSsl))
			{
				IDomainUser domainUser;
				if (!context.TryFindUser(userName, out domainUser))
				{
					throw new ArgumentException($"Unable to find the specified user '{userName}' in domain '{domain}'");
				}

				using (domainUser)
				{
					domainUser.ChangePassword(
						this.commandLinePasswordChangerSource.OldPassword(), 
						this.commandLinePasswordChangerSource.NewPassword());
				}
			}
		}
	}
}