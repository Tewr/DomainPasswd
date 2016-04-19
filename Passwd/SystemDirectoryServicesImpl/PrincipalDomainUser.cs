using System;
using System.DirectoryServices.AccountManagement;
using System.Runtime.InteropServices;
using System.Security;

namespace Passwd.SystemDirectoryServicesImpl
{
	internal class PrincipalDomainUser : IDomainUser
	{
		private readonly UserPrincipal principal;

		public PrincipalDomainUser(UserPrincipal principal)
		{
			this.principal = principal;
		}

		public void ChangePassword(SecureString oldPassword, SecureString newPassword)
		{
			this.principal.ChangePassword(ConvertToUnsecureString(oldPassword), ConvertToUnsecureString(newPassword));
		}

		public void Dispose()
		{
			this.principal.Dispose();
		}

		public static string ConvertToUnsecureString(SecureString securePassword)
		{
			if (securePassword == null)
				throw new ArgumentNullException("securePassword");

			IntPtr unmanagedString = IntPtr.Zero;
			try
			{
				unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
				return Marshal.PtrToStringUni(unmanagedString);
			}
			finally
			{
				Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
			}
		}
	}
}