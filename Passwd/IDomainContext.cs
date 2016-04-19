using System;

namespace Passwd
{
	internal interface IDomainContext: IDisposable
	{
		bool TryFindUser(string samAccountName, out IDomainUser domainUser);
	}
}