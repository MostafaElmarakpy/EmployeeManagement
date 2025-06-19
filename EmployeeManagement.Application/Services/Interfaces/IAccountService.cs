using EmployeeManagement.Application.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Services.Interfaces
{
    public interface IAccountService
    {
        Task<SignInResult> LoginAsync(string username, string password, bool rememberMe);
        Task<IdentityResult> RegisterAsync(RegisterViewModel model);
        Task LogoutAsync();
    }
}
