﻿using Microsoft.AspNetCore.Identity;

namespace Myshop.Web.Models
{
    public class AppUser : IdentityUser
    {
        public string? DisplayName { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
    }
}
