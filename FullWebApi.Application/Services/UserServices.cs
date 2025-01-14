using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;
using FullWebApi.Application.Interfaces;
using FullWebApi.Domain.Dtos;
using FullWebApi.Domain.Models;
using FullWebApi.Domain.ModelsValidator;
using FullWebApi.Infrastructure.Data;
using LanguageExt;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FullWebApi.Application.Services
{
    public class UserServices : IUserServices
    {
        private readonly AppDBContext _context;
        public UserServices(AppDBContext context)
        {
            _context = context;
        }

        public async Task<List<User>?> GetAllUsers(UserDto req)
        {
           var users = await _context.Users.ToListAsync(); 
           
           if(users == null){
             return null;
            }   
            return users;
        }

        public async Task<Either<UserDto, object>> SignUpUser(UserDto req)
        {    
            User newUser = new User
            {
              Name = req.Name,
              Email = req.Email,
              Password = req.Password
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();     
            return newUser;            
        }
    }
}