// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using JobOffers.Models;
using static JobOffers.Configurations.Helper;

namespace JobOffers.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
             Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            public string Image { get; set; }
            public string CV { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = phoneNumber,
                Image = user.Image,
                CV = user.CV
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }
        [BindProperty]
        public IFormFile Upload { get; set; }

        public async Task<IActionResult> OnPostAsync(IFormFile postedImage, IFormFile postedCV)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (postedCV is not null)
            {
                if (Path.GetExtension(postedCV.FileName) is not UploadFile.CvAllowedExtenstions)
                    return BadRequest("Only .pdf is allowed!");

                if (postedCV.Length > UploadFile.MaxAllowedSize)
                    return BadRequest("Max allowed size for Image is 1MB!");

                string wwwPath = _environment.WebRootPath;
                string path = Path.Combine(wwwPath, @"CVs");
                string fileName = Path.GetFileName(postedCV.FileName);

                string oldUserCV = $"{path}\\{user.CV}";

                if (System.IO.File.Exists(oldUserCV))
                    System.IO.File.Delete(oldUserCV);

                using (FileStream stream = new(Path.Combine(path, fileName), FileMode.Create))
                {
                    postedCV.CopyTo(stream);
                }
                user.CV = fileName;
            }

            if (postedImage is not null && postedImage.FileName is not UploadFile.UserDefaultImage)
            {
                if (!UploadFile.ImageAllowedExtenstions.Contains(Path.GetExtension(postedImage.FileName).ToLower()))
                    return BadRequest("Only .png, jpeg and .jpg images are allowed!");

                if (postedImage.Length > UploadFile.MaxAllowedSize)
                    return BadRequest("Max allowed size for Image is 1MB!");

                string wwwPath = _environment.WebRootPath;
                string path = Path.Combine(wwwPath, @"images\UserProfile");
                string fileName = Path.GetFileName(postedImage.FileName);

                string oldUserImage = $"{path}\\{user.Image}";

                if (System.IO.File.Exists(oldUserImage))
                    System.IO.File.Delete(oldUserImage);

                using (FileStream stream = new(Path.Combine(path, fileName), FileMode.Create))
                {
                    postedImage.CopyTo(stream);
                }
                user.Image = fileName;
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var firstName = user.FirstName;
            var lastName = user.LastName;

            if (Input.FirstName != firstName)
                user.FirstName = Input.FirstName;

            if (Input.LastName != lastName)
                user.LastName = Input.LastName;

            await _userManager.UpdateAsync(user);

            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }



            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
