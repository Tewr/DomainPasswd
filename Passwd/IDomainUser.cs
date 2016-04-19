using System;
using System.Security;

namespace Passwd
{
	internal interface IDomainUser : IDisposable
	{
		void ChangePassword(SecureString oldPassword, SecureString newPassword);
	}
}