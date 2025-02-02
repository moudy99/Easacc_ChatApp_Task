﻿using Application.Interfaces.Services;
using Application.ViewModel;
using AutoMapper;
using Core.Model;
using Microsoft.AspNetCore.Identity;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;


        public UserService(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }
        public async Task<bool> CreateUserAsync(RegisterViewModel model, string password)
        {
            var user = _mapper.Map<ApplicationUser>(model);
            user.Role = "User";
            user.UserName = model.Email.ToLower();
            user.Name = model.Name;
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
            }
            return result.Succeeded;
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser> UpdateUser(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return user;
            }
            return null;
        }

        public async Task<ApplicationUser> getUserById(string userID)
        {
            var user = await _userManager.FindByIdAsync(userID);
            if (user == null)
            {
                return null;
            }
            return user;
        }

    }
}
