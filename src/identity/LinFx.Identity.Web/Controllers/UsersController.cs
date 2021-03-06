﻿using System.Linq;
using System.Threading.Tasks;
using LinFx.Application.Models;
using LinFx.Identity.Authorization;
using LinFx.Identity.Domain.Models;
using LinFx.Identity.Web.Models.ManageViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static LinFx.Identity.Web.Models.ManageViewModels.ApplicationUserEditModel;

namespace Identity.Web.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // GET: User
        public async Task<IActionResult> Index(int page = 1, int limit = 10)
        {
            var count = await _userManager.Users.LongCountAsync();

            var items = await _userManager.Users
                .PageBy(page, limit)
                .ToListAsync();

            var result = new PagedResult<ApplicationUser>(count, items);

            return View(items);
        }

        // GET: User/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return View(user);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var roles = _roleManager.Roles;

            var model = new ApplicationUserEditModel
            {
                User = new ApplicationUserViewModel
                {
                    Id = user.Id,
                    ConcurrencyStamp = user.ConcurrencyStamp,
                    Name = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                }
            };

            var userRoleNames = await _userManager.GetRolesAsync(user);

            model.Roles = roles.Select(p => new AssignedRoleViewModel
            {
                Name = p.Name,
                IsAssigned = userRoleNames.Any(x => x == p.Name)
            }).ToArray();

            return View(model);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUserEditModel input)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);

                //TryUpdateModelAsync(user);
                user.PhoneNumber = input.User.PhoneNumber;

                await _userManager.UpdateAsync(user);

                foreach (var role in input.Roles)
                {
                    if (role.IsAssigned)
                        await _userManager.AddToRoleAsync(user, role.Name);
                    else
                        await _userManager.RemoveFromRoleAsync(user, role.Name);
                }

                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user != null)
            {
                var result = await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}