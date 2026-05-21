using day1.Domain;
using day1.DTOs;
using day1.DTOs.Dish;
using day1.ProjectHelper;
using day1.Repositories;
using day1.Models;
using System.Linq.Expressions;

namespace day1.Services.Dish
{
    public class DishServer : IDishServer
    {
        public readonly IDishRepository _dishRepository;
        public DishServer(IDishRepository dishRepository)
        {
          _dishRepository =dishRepository;
        }

        public DishDTO AddDish(DishDTO dishDTO)
        {
            var dish = new Models.Dish
            {
                Name = dishDTO.Name,
                Description = dishDTO.Description,
                Price = dishDTO.Price,
                ImageUrl = dishDTO.ImageUrl,
                CreatedDate=DateTime.UtcNow,
                UpdatedDate=DateTime.UtcNow
            };
            var result= _dishRepository.AddDish(dish);
            return new DishDTO {ID= result.Id,Name=result.Name,Description=result.Description,Price=result.Price,ImageUrl=result.ImageUrl };
            //CheckPassword(createUserDto.PassWord); // ✅ 密码校验逻辑
            //var exct = _userRepository.ExctisUserName(createUserDto.UserName);
            //if (exct)
            //{
            //    throw new BusinessException(Domain.Enum.BusinessErrorCode.UserAlreadyExists, "用户名已存在");
            //}
            //var user = new Models.Dish
            //{

            //};
            //return _dishRepository.AddDish(user);  // ✅ 调用 Repository
        }

        public List<DishDTO> AddDishS(List<DishDTO> dishes)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteDishAsync(long id)
        {
            var dish = await _dishRepository.GetByIdAsync(id);
            if (dish == null) return false;

            //同时删除图片文件（wwwroot/uploads 下的文件）
            if (!string.IsNullOrEmpty(dish.ImageUrl))
            {
                if (File.Exists(dish.ImageUrl)) File.Delete(dish.ImageUrl);
            }

            await _dishRepository.DeleteAsync(dish);
            return true;
        }

        public List<DishDTO> GetDishes()
        {
            return _dishRepository.GetDishes().Select(d => new DishDTO
            {
                ID = d.Id,
                Name = d.Name,
                Price = d.Price,
                Description = d.Description,
                ImageUrl=d.ImageUrl
            }).ToList();
        }

        public async Task<PagedResult<DishDTO>> GetPageDishAsync(int pageindex, int pagesize, string? keyword)
        {
            if (pageindex < 1) pageindex = 1;
            if (pagesize < 1) pagesize = 12;
            if (pagesize > 100) pagesize = 12;
            Expression<Func<Models.Dish, bool>>? filter = null;
            if (!string.IsNullOrEmpty(keyword))
            {
                var kw=keyword.Trim();
                filter = d => d.Name.Contains(kw);
            }
            var totalcount =await _dishRepository.GetCountAsync(filter);
            Expression<Func<Models.Dish,DishDTO>> seleter = null;
            seleter = d => new DishDTO
            {
                ID = d.Id,
                Name = d.Name,
                Price = d.Price,
                Description = d.Description,
                ImageUrl = d.ImageUrl
            };
            var pagedData =await _dishRepository.GetDishesPagedAsync(pageindex, pagesize, filter, seleter);
            return new PagedResult<DishDTO>
            {
                Items= pagedData.Items,
                TotalCount = totalcount,
                PageIndex = pageindex,
                PageSize= pagesize,
            };

        }




    }
}
