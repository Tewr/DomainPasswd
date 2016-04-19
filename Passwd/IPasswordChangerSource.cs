using System.Security;

namespace Passwd
{
	internal interface IPasswordChangerSource
	{
		SecureString OldPassword();

		SecureString NewPassword();
	}
}