using WordProcessor.Data.Entities;

namespace WordProcessor.Data.Contracts
{
	public interface IWordRepository : IRepository<Word>, IDisposable
	{
		IEnumerable<Word> GetCloseWords(string wordPart, int count);

		Task<IReadOnlyCollection<Word>> GetCloseWordsAsync(string wordPart, int count);
	}
}
