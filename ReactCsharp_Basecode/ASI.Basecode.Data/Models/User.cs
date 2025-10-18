using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models;

public partial class User
{
    public string Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public bool IsActive { get; set; }

    public string Role { get; set; }

    public string Department { get; set; }

    public DateTime CreatedTime { get; set; }

    public string CreatedBy { get; set; }

    public DateTime UpdatedTime { get; set; }

    public string UserId { get; set; }
}
