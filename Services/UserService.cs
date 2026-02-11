using day1.Data;
using day1.Models;
using day1.Repositories;
using day1.DTOs;

namespace day1.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public List<User> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }

        public User CreateUser(CreateUserDTO createUserDto)
        {

            CheckPassword(createUserDto.PassWord); // ✅ 密码校验逻辑
            var exct= _userRepository.ExctisUserName(createUserDto.UserName);
            if (exct)
            {
                throw new Exception("用户名已存在");
            }
            var user = new User
            {
                UserName = createUserDto.UserName,
                PassWord = createUserDto.PassWord, // 后面我们会加密
                CreateTime = DateTime.Now
            };



            _userRepository.Add(user);  // ✅ 调用 Repository
            return user;
        }

        private void CheckPassword(string password)
        {
            if (password.Length < 6)
                throw new Exception("密码长度不能小于6位");

            if (!password.Any(char.IsDigit) || !password.Any(char.IsLetter))
                throw new Exception("密码必须包含字母和数字");
        }

        public User? GetById(int id)
        {
            return _userRepository.GetById(id);
        }
    }
}
