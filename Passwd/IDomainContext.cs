using System;

namespace Passwd
{
	internal interface IDomainContext: IDisposable
	{
		bool TryFindUser(string accountName, out IDomainUser domainUser);
	}
}