using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace PCC.Pages
{
    public class VenueContactModel : PageModel
    {
        [BindProperty]
        [Required]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Phone]
        public string? Phone { get; set; }

        [BindProperty]
        [Required]
        public string EventType { get; set; } = string.Empty;

        [BindProperty]
        [DataType(DataType.Date)]
        public DateTime? EventDate { get; set; }

        [BindProperty]
        public int? GuestCount { get; set; }

        [BindProperty]
        [Required]
        public string Message { get; set; } = string.Empty;

        public bool SubmitSuccess { get; set; } = false;

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // TODO: Add logic to save the inquiry to a database or send an email
            // For now, we'll just set the success flag
            SubmitSuccess = true;

            return Page();
        }
    }
}
