using System.DirectoryServices.AccountManagement;

namespace Passwd.SystemDirectoryServicesImpl
{
	internal class PrincipalDomainContext : IDomainContext
	{
		private readonly PrincipalContext principalContext;

		public PrincipalDomainContext(PrincipalContext principalContext)
		{
			this.principalContext = principalContext;
		}

		public bool TryFindUser(string accountName, out IDomainUser result)
		{
			result = null;
			var findByIdentity =
				UserPrincipal.FindByIdentity(this.principalContext, IdentityType.SamAccountName, accountName)
				??
				UserPrincipal.FindByIdentity(this.principalContext, IdentityType.UserPrincipalName, accountName);

			if (findByIdentity == null)
				return false;
			result = new PrincipalDomainUser(findByIdentity);
			return true;
		}

		public void Dispose()
		{
			this.principalContext.Dispose();
		}
	}
}