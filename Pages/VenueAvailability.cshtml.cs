using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace PCC.Pages
{
    public class VenueAvailabilityModel : PageModel
    {
        [BindProperty]
        [Required]
        [DataType(DataType.Date)]
        public DateTime? SelectedDate { get; set; }

        [BindProperty]
        [Required]
        public string EventType { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        public int? GuestCount { get; set; }

        [BindProperty]
        [Required]
        public string TimeSlot { get; set; } = string.Empty;

        public bool CheckPerformed { get; set; } = false;
        public bool IsAvailable { get; set; } = false;
        public string UnavailableReason { get; set; } = string.Empty;
        public DateTime CurrentMonth { get; set; }

        // Simulated booked dates - in a real application, this would come from a database
        private static readonly HashSet<DateTime> BookedDates = new HashSet<DateTime>
        {
            new DateTime(2025, 11, 25), // Example booked dates
            new DateTime(2025, 11, 30),
            new DateTime(2025, 12, 15),
            new DateTime(2025, 12, 24),
            new DateTime(2025, 12, 25),
            new DateTime(2025, 12, 31),
            new DateTime(2026, 1, 1),
        };

        public void OnGet(string? month = null)
        {
            if (!string.IsNullOrEmpty(month) && DateTime.TryParse(month, out DateTime parsedMonth))
            {
                CurrentMonth = new DateTime(parsedMonth.Year, parsedMonth.Month, 1);
            }
            else
            {
                CurrentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }
        }

        public IActionResult OnPost()
        {
            CurrentMonth = SelectedDate.HasValue
                ? new DateTime(SelectedDate.Value.Year, SelectedDate.Value.Month, 1)
                : new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            CheckPerformed = true;

            // Check if the date is in the past
            if (SelectedDate < DateTime.Now.Date)
            {
                IsAvailable = false;
                UnavailableReason = "The selected date is in the past. Please choose a future date.";
                return Page();
            }

            // Check if the date is already booked
            if (BookedDates.Contains(SelectedDate.Value.Date))
            {
                IsAvailable = false;
                UnavailableReason = "Unfortunately, this date is already booked. Please select another date or contact us for alternative options.";
                return Page();
            }

            // Check capacity constraints
            if (GuestCount > 500)
            {
                IsAvailable = false;
                UnavailableReason = "The guest count exceeds our maximum capacity of 500. Please contact us to discuss options.";
                return Page();
            }

            // Check if booking is too soon (less than 7 days in advance)
            if ((SelectedDate.Value.Date - DateTime.Now.Date).TotalDays < 7)
            {
                IsAvailable = false;
                UnavailableReason = "We require at least 7 days advance notice for bookings. Please select a date at least one week from now.";
                return Page();
            }

            // If all checks pass, the venue is available
            IsAvailable = true;

            return Page();
        }

        public string GetTimeSlotDisplay()
        {
            return TimeSlot switch
            {
                "morning" => "Morning (9:00 AM - 12:00 PM)",
                "afternoon" => "Afternoon (12:00 PM - 5:00 PM)",
                "evening" => "Evening (5:00 PM - 10:00 PM)",
                "full-day" => "Full Day",
                _ => TimeSlot
            };
        }

        public List<CalendarDay> GetCalendarDays()
        {
            var days = new List<CalendarDay>();
            var firstDayOfMonth = CurrentMonth;
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Add empty cells for days before the first day of the month
            int dayOfWeek = (int)firstDayOfMonth.DayOfWeek;
            for (int i = 0; i < dayOfWeek; i++)
            {
                days.Add(new CalendarDay { IsEmpty = true });
            }

            // Add all days of the month
            for (int day = 1; day <= lastDayOfMonth.Day; day++)
            {
                var currentDate = new DateTime(CurrentMonth.Year, CurrentMonth.Month, day);
                var isPast = currentDate < DateTime.Now.Date;
                var isBooked = BookedDates.Contains(currentDate);
                var isSelected = SelectedDate.HasValue && SelectedDate.Value.Date == currentDate;

                var calendarDay = new CalendarDay
                {
                    Date = currentDate,
                    IsEmpty = false,
                    IsAvailable = !isPast && !isBooked,
                    IsBooked = isBooked,
                    IsSelected = isSelected
                };

                // Set CSS class and tooltip
                if (isSelected)
                {
                    calendarDay.CssClass = "selected";
                    calendarDay.Tooltip = "Selected date";
                }
                else if (isPast)
                {
                    calendarDay.CssClass = "booked";
                    calendarDay.Tooltip = "Past date";
                }
                else if (isBooked)
                {
                    calendarDay.CssClass = "booked";
                    calendarDay.Tooltip = "Already booked";
                }
                else
                {
                    calendarDay.CssClass = "available";
                    calendarDay.Tooltip = "Available - Click to select";
                }

                days.Add(calendarDay);
            }

            return days;
        }

        public class CalendarDay
        {
            public DateTime Date { get; set; }
            public bool IsEmpty { get; set; }
            public bool IsAvailable { get; set; }
            public bool IsBooked { get; set; }
            public bool IsSelected { get; set; }
            public string CssClass { get; set; } = string.Empty;
            public string Tooltip { get; set; } = string.Empty;
        }
    }
}
