using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Passwd
{
	internal class CommandLinePasswordChangerSource : IPasswordChangerSource
	{
		private SecureString oldPassword;

		private SecureString newPassword;

		public SecureString OldPassword() => oldPassword ?? (oldPassword = ReadPassword("Current password"));

		public SecureString NewPassword() => newPassword ?? (newPassword = ReadPassword("New password", "Reenter password"));

		private SecureString ReadPassword(params string[] prompts)
		{
			SecureString last = null;
			foreach (var prompt in prompts)
			{
				Console.Write($"{prompt}: ");
				SecureString current = new SecureString();
				ConsoleKeyInfo keyChar;
				while ((keyChar = Console.ReadKey(true)).Key != ConsoleKey.Enter)
				{
					current.AppendChar(keyChar.KeyChar);
				}
				Console.Error.WriteLine();

				if (last != null && !SecureStringEqual(current, last))
				{
					throw new PasswordMismatchException("The passwords do not match");
				}

				last = current;
			}

			return last;
		}

		bool SecureStringEqual(SecureString s1, SecureString s2)
		{
			if (s1 == null)
			{
				throw new ArgumentNullException("s1");
			}
			if (s2 == null)
			{
				throw new ArgumentNullException("s2");
			}

			if (s1.Length != s2.Length)
			{
				return false;
			}

			IntPtr bstr1 = IntPtr.Zero;
			IntPtr bstr2 = IntPtr.Zero;

			RuntimeHelpers.PrepareConstrainedRegions();

			try
			{
				bstr1 = Marshal.SecureStringToBSTR(s1);
				bstr2 = Marshal.SecureStringToBSTR(s2);

				unsafe
				{
					for (Char* ptr1 = (Char*)bstr1.ToPointer(), ptr2 = (Char*)bstr2.ToPointer();
						*ptr1 != 0 && *ptr2 != 0;
						 ++ptr1, ++ptr2)
					{
						if (*ptr1 != *ptr2)
						{
							return false;
						}
					}
				}

				return true;
			}
			finally
			{
				if (bstr1 != IntPtr.Zero)
				{
					Marshal.ZeroFreeBSTR(bstr1);
				}

				if (bstr2 != IntPtr.Zero)
				{
					Marshal.ZeroFreeBSTR(bstr2);
				}
			}
		}
	}
}