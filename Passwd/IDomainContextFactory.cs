namespace Passwd
{
	internal interface IDomainContextFactory
	{
		IDomainContext Create(string domain, bool useSsl);
	}
}