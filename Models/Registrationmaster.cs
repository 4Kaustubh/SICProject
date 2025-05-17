using System;
using System.Collections.Generic;

namespace SICProject.Models;

public partial class Registrationmaster
{
    public long RegistrationId { get; set; }

    public string? StudentName { get; set; }

    public int? DepartmentId { get; set; }

    public string? DepartmentName { get; set; }

    public string? Email { get; set; }

    public ulong? IsApproved { get; set; }

    public string? Remarks { get; set; }

    public string? MobileNumber { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }
}
