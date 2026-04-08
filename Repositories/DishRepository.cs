using System.Linq.Expressions;
using System.Threading.Tasks;
using day1.Data;
using day1.DTOs;
using day1.Models;
using Microsoft.EntityFrameworkCore;

namespace day1.Repositories
{
    public class DishRepository : IDishRepository
    {

        private readonly AppDbContext _context;
        public DishRepository(AppDbContext context)
        {
            _context = context;
        }
        public Dish AddDish(Dish dish)
        {
            _context.Dishes.Add(dish);
            _context.SaveChanges();
            return dish;
        }

        public async Task<List<Dish>> AddDishS(List<Dish> dishes)
        {
            _context.Dishes.AddRange(dishes);
            await _context.SaveChangesAsync();
            return dishes;
        }

        public async Task<int> DeleteDishsByIdAsync(params long[] ids)
        {
            if (ids==null||ids.Length==0)
            {
                return 0;
            }
            var dishesToDelete = await _context.Dishes.Where(d => ids.Contains(d.Id)).ToListAsync();
            if (!dishesToDelete.Any())
                return 0;
            _context.Dishes.RemoveRange(dishesToDelete);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> GetCountAsync(Expression<Func<Dish, bool>>? filter = null)
        {
           var queryable = _context.Dishes.AsNoTracking();
            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }
            return await queryable.CountAsync();
        }

        public List<Dish> GetDishes()
        {
            return _context.Dishes.ToList();
        }

        public async Task<PagedResult<T>> GetDishesPagedAsync<T>(int pageIndex, int pageSize, Expression<Func<Dish, bool>>? filter = null, Expression<Func<Dish, T>>? selector = null, CancellationToken cancellationToken = default)
        {
            var queryable = _context.Dishes.AsNoTracking();
            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }
            var totalCount = await queryable.CountAsync(cancellationToken);
            if (totalCount == 0)
            {
                return new PagedResult<T>
                {
                    Items = new List<T>(),
                    TotalCount = 0,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };
            }
            queryable = queryable.OrderBy(q=>q.Id);
            var pagedData = queryable.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            List<T> items;
            if (selector != null)
            {
                  items=await pagedData.Select(selector).ToListAsync(cancellationToken);
            }
            else
            {
                if (typeof(T) == typeof(Dish)) 
                {
                    var dishList = await pagedData.ToListAsync(cancellationToken);
                    items = dishList.Cast<T>().ToList();
                }
                else
                {
                    throw new InvalidOperationException("必须提供 selector 或将 T 指定为 Dish 类型");
                }
            }
            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

        }

        public IQueryable<Dish> GetDishIQueryable(Expression<Func<Dish, bool>>? filter = null)
        {
            var queryable = _context.Dishes.AsNoTracking();
            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }
            return queryable;
        }
    }
}
