﻿using System.ComponentModel.DataAnnotations;

namespace VideStore.Shared.DTOs.Requests.Users

{
    public record ResetPasswordEmailRequest(
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        string Email
    );
}