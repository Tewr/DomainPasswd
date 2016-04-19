using System;

namespace Passwd
{
	[Serializable]
	internal class PasswordMismatchException : Exception
	{
		public PasswordMismatchException(string message):base(message)
		{
		}
	}
}