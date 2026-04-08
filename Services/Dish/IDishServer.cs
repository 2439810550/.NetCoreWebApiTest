using System.Collections.Specialized;
using day1.DTOs;
using day1.DTOs.Dish;

namespace day1.Services.Dish
{
    public interface IDishServer
    {
        List<DishDTO> GetDishes();
        DishDTO AddDish(DishDTO dishDTO);
        List<DishDTO> AddDishS(List<DishDTO> dishes);

        Task<PagedResult<DishDTO>> GetPageDishAsync(int pageindex,int pagesize,string? keyword);
    }
}
