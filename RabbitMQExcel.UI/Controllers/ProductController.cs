using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RabbitMQExcel.UI.Models;

namespace RabbitMQExcel.UI.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProductController(UserManager<IdentityUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> CreateProductExcel()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var fileName = $"product-excel-{Guid.NewGuid().ToString().Substring(1,10)}";

            UserFile userFile = new()
            {
                UserId = user.Id,
                FileName = fileName,
                FileStatus = FileStatus.Creating
            };
            await _context.UserFiles.AddAsync(userFile);
            await _context.SaveChangesAsync();
            TempData["StartCreatingExcel"]= true;

            return RedirectToAction(nameof(Files));

        }

        public async Task<IActionResult> Files()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            return View(await _context.UserFiles.Where (x=> x.UserId == user.Id).ToListAsync());
        }
    }
}