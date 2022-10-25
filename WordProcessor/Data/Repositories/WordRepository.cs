using Microsoft.EntityFrameworkCore;
using WordProcessor.Data.Contracts;
using WordProcessor.Data.Entities;
using WordProcessor.Data.Repositories.Generic;

namespace WordProcessor.Data.Repositories
{
	public class WordRepository : Repository<Word>, IWordRepository
	{
		private readonly DatabaseContext _databaseContext;

		public WordRepository(DatabaseContext databaseContext) : base(databaseContext)
		{
			_databaseContext = databaseContext;
		}

		public IEnumerable<Word> GetCloseWords(string wordPart, int count)
		{
			var words = DbSet
				.AsNoTracking()
				.Where(word => word.Content.StartsWith(wordPart) && !word.Content.Equals(wordPart))
				.OrderByDescending(word => word.Frequency)
				.ThenBy(word => word.Content)
				.Take(count);

			return words.AsEnumerable();
		}

		public async Task<IReadOnlyCollection<Word>> GetCloseWordsAsync(string wordPart, int count)
		{
			var words = await DbSet
				.AsNoTracking()
				.Where(word => word.Content.StartsWith(wordPart) && !word.Content.Equals(wordPart))
				.OrderBy(word => word.Frequency)
				.ThenBy(word => word.Content)
				.Take(count)
				.ToListAsync();

			return words.AsReadOnly();
		}

		public void Dispose()
		{
			_databaseContext.Dispose();
		}
	}
}
