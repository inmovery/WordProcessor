namespace WordProcessor.Data.Contracts
{
	public interface IUnitOfWork : IDisposable
	{
		IWordRepository Words { get; }

		void CommitAsync();
	}
}
