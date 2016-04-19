using System.DirectoryServices.AccountManagement;

namespace Passwd.SystemDirectoryServicesImpl
{
	internal class PrincipalDomainContextFactory : IDomainContextFactory
	{
		public IDomainContext Create(string domain, bool useSsl)
		{
			var context = useSsl?
				 new PrincipalContext(ContextType.Domain, domain, null, ContextOptions.Negotiate | ContextOptions.SecureSocketLayer):
				new PrincipalContext(ContextType.Domain, domain);

			return new PrincipalDomainContext(context);
		}
	}
}