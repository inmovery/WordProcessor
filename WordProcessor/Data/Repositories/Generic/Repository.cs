using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WordProcessor.Data.Contracts;

namespace WordProcessor.Data.Repositories.Generic
{
	public abstract class Repository<T> : IRepository<T>
		where T : class
	{
		private readonly DatabaseContext _databaseContext;
		protected readonly DbSet<T> DbSet;

		protected Repository(DatabaseContext databaseContext)
		{
			_databaseContext = databaseContext;
			DbSet = _databaseContext.Set<T>();
		}

		public T? GetById(object id)
		{
			var item = DbSet.Find(id);
			return item;
		}

		public IEnumerable<T> GetAll()
		{
			return DbSet.AsEnumerable();
		}

		public IEnumerable<T> Query(Expression<Func<T, bool>> filter)
		{
			return DbSet.Where(filter);
		}

		public void Add(T entity)
		{
			DbSet.Add(entity);
		}

		public void AddRange(IEnumerable<T> entities)
		{
			DbSet.AddRange(entities);
		}

		public async Task AddRangeAsync(IEnumerable<T> entities)
		{
			await DbSet.AddRangeAsync(entities);
		}

		public void Delete(object id)
		{
			var entity = GetById(id);
			if (entity == null)
				return;

			if (_databaseContext.Entry(entity).State == EntityState.Detached)
				DbSet.Attach(entity);

			DbSet.Remove(entity);
		}

		public void Update(T entity)
		{
			DbSet.Attach(entity);
			_databaseContext.Entry(entity).State = EntityState.Modified;
		}

		public void UpdateRange(IEnumerable<T> entities)
		{
			DbSet.UpdateRange(entities);
		}

		public int Count()
		{
			return DbSet.Count();
		}

		public void RemoveAll()
		{
			DbSet.RemoveRange(DbSet);
		}

		public async Task CommitChangesAsync()
		{
			await _databaseContext.SaveChangesAsync();
		}
	}
}
