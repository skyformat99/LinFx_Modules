using System.Collections.Generic;
using System.Threading.Tasks;
using LinFx.Identity.Application;
using LinFx.Identity.Application.Services;
using LinFx.Identity.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LinFx.Identity.UI.Pages.Identity.Users
{
    [Authorize(IdentityPermissions.Users.Default)]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationUserService _applicationUserService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            UserManager<ApplicationUser> userManager, 
            ApplicationUserService applicationUserService,
            ILogger<IndexModel> logger)
        {
            _userManager = userManager;
            _applicationUserService = applicationUserService;
            _logger = logger;
        }

        public string ReturnUrl { get; set; }

        [BindProperty]
        public ICollection<ApplicationUser> Items { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            Items = await _applicationUserService.GetListAsync();
        }
    }
}
