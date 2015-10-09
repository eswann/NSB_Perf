namespace Q2.Oao.Library.Common.Configuration
{
	public interface IServiceSettings
	{
		string ServiceName { get; }
		string EncryptionKey { get; }
	}
}