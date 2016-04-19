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

		public bool TryFindUser(string samAccountName, out IDomainUser result)
		{
			result = null;
			var findByIdentity =
				UserPrincipal.FindByIdentity(this.principalContext, IdentityType.SamAccountName, samAccountName)
				??
				UserPrincipal.FindByIdentity(this.principalContext, IdentityType.UserPrincipalName, samAccountName)
				??
				UserPrincipal.FindByIdentity(this.principalContext, IdentityType.Guid, samAccountName);
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