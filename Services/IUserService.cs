﻿using day1.Models;
using day1.DTOs;

namespace day1.Services
{
    public interface IUserService
    {
        List<User> GetAllUsers();

        CreateUserDTO CreateUser(DTOs.CreateUserDTO createUserDto);

        User? GetById(long id);

        DTOs.LoginResponseDto Login(DTOs.CreateUserDTO createUserDTO);

        void DeleteByUserName(string username);



    }
}
