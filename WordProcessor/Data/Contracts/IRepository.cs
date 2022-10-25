using System.Linq.Expressions;

namespace WordProcessor.Data.Contracts
{
	public interface IRepository<T> where T : class
	{
		/// <summary>
		/// Get item by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		T? GetById(object id);

		/// <summary>
		/// Get all items
		/// </summary>
		/// <returns></returns>
		IEnumerable<T> GetAll();

		/// <summary>
		/// Add item to specified storage
		/// </summary>
		/// <param name="entity"></param>
		void Add(T entity);

		/// <summary>
		/// Add items collection to specified storage
		/// </summary>
		/// <param name="entities"></param>
		void AddRange(IEnumerable<T> entities);

		/// <summary>
		/// Add items collection to specified storage asynchronously
		/// </summary>
		/// <param name="entities"></param>
		/// <returns></returns>
		Task AddRangeAsync(IEnumerable<T> entities);

		/// <summary>
		/// Delete specified item from storage
		/// </summary>
		/// <param name="id"></param>
		void Delete(object id);

		/// <summary>
		/// Update specified item inside storage
		/// </summary>
		/// <param name="entity"></param>
		void Update(T entity);

		/// <summary>
		/// Update items collection inside storage
		/// </summary>
		/// <param name="entities"></param>
		void UpdateRange(IEnumerable<T> entities);

		/// <summary>
		/// Get specified items
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		IEnumerable<T> Query(Expression<Func<T, bool>> filter);

		/// <summary>
		/// Get count of items inside storage
		/// </summary>
		/// <returns></returns>
		int Count();

		/// <summary>
		/// Apply changes to database
		/// </summary>
		Task CommitChangesAsync();

		/// <summary>
		/// Remove all rows from table
		/// </summary>
		void RemoveAll();
	}
}
