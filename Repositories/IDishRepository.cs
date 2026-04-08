using System.Linq.Expressions;
using day1.DTOs;
using day1.DTOs.Dish;
using day1.Models;

namespace day1.Repositories
{
    public interface IDishRepository
    {
        /// <summary>
        /// 获取所有菜品列表，返回包含数据库中所有菜品的列表 这里不推荐 推荐使用分页查询接口 GetDishesPaged 来获取菜品列表，避免一次性加载过多数据导致性能问题
        /// </summary>
        /// <returns></returns>
        List<Dish> GetDishes();
        Dish AddDish(Dish dish);
        /// <summary>
        /// 批量添加菜品，返回添加后的菜品列表，包含数据库生成的ID等信息
        /// </summary>
        /// <param name="dishes"></param>
        /// <returns></returns>
        Task<List<Dish>> AddDishS(List<Dish> dishes);

        IQueryable<Dish> GetDishIQueryable(Expression<Func<Dish,bool>>? filter=null);
        /// <summary>
        /// 获取菜品总数，返回数据库中菜品的总记录数，通常用于分页查询时计算总页数等信息
        /// </summary>
        /// <returns></returns>
        Task<int> GetCountAsync(Expression<Func<Dish,bool>>? filter=null);

        Task<PagedResult<T>> GetDishesPagedAsync<T>(int pageIndex, int pageSize, Expression<Func<Dish, bool>>? filter = null, Expression<Func<Dish,T>>? selector = null,
            CancellationToken cancellationToken = default);
        Task<int> DeleteDishsByIdAsync(params long[] ids);



    }
}
