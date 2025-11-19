using ASI.Basecode.Data;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Manager;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ASI.Basecode.WebApp
{
    public class DatabaseSeeder
    {
        private readonly NexDeskDbContext _context;

        public DatabaseSeeder(NexDeskDbContext context)
        {
            _context = context;
        }

        public void SeedDatabase()
        {
            // Ensure the database schema is created
            _context.Database.EnsureCreated();

            // Create departments if they don't exist
            try
            {
                if (!_context.Departments.Any())
                {
                    var departments = new[]
                    {
                    new Department
                    {
                        Id = "1",
                        Name = "IT",
                        Description = "Information Technology Department",
                        IsActive = true,
                        CreatedBy = "System",
                        CreatedTime = DateTime.Now,
                        UpdatedTime = DateTime.Now,
                        UpdatedBy = "System"
                    },
                    new Department
                    {
                        Id = "2",
                        Name = "HR",
                        Description = "Human Resources Department",
                        IsActive = true,
                        CreatedBy = "System",
                        CreatedTime = DateTime.Now,
                        UpdatedTime = DateTime.Now,
                        UpdatedBy = "System"
                    },
                    new Department
                    {
                        Id = "3",
                        Name = "Finance",
                        Description = "Finance and Accounting Department",
                        IsActive = true,
                        CreatedBy = "System",
                        CreatedTime = DateTime.Now,
                        UpdatedTime = DateTime.Now,
                        UpdatedBy = "System"
                    },
                    new Department
                    {
                        Id = "4",
                        Name = "Marketing",
                        Description = "Marketing and Communications Department",
                        IsActive = true,
                        CreatedBy = "System",
                        CreatedTime = DateTime.Now,
                        UpdatedTime = DateTime.Now,
                        UpdatedBy = "System"
                    },
                    new Department
                    {
                        Id = "5",
                        Name = "Operations",
                        Description = "Operations and Management Department",
                        IsActive = true,
                        CreatedBy = "System",
                        CreatedTime = DateTime.Now,
                        UpdatedTime = DateTime.Now,
                        UpdatedBy = "System"
                    }
                };

                    _context.Departments.AddRange(departments);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Log error but continue
                Console.WriteLine($"Error seeding departments: {ex.Message}");
            }

            // Create test users if they don't exist
            try
            {
                if (!_context.Users.Any())
                {
                    var testUsers = new[]
                    {
                        new User
                        {
                            Id = "1",
                            UserId = "USR001",
                            Username = "admin",
                            Password = PasswordManager.EncryptPassword("admin123"),
                            FirstName = "Admin",
                            LastName = "User",
                            Email = "admin@nexdesk.com",
                            Role = "admin",
                            DepartmentId = "1", // IT
                            IsActive = true,
                            CreatedBy = "System",
                            CreatedTime = DateTime.Now,
                            UpdatedTime = DateTime.Now
                        },
                        new User
                        {
                            Id = "2",
                            UserId = "USR002",
                            Username = "agent",
                            Password = PasswordManager.EncryptPassword("agent123"),
                            FirstName = "Agent",
                            LastName = "Smith",
                            Email = "agent@nexdesk.com",
                            Role = "agent",
                            DepartmentId = "6", // Customer Support
                            IsActive = true,
                            CreatedBy = "System",
                            CreatedTime = DateTime.Now,
                            UpdatedTime = DateTime.Now
                        },
                        new User
                        {
                            Id = "3",
                            UserId = "USR003",
                            Username = "staff",
                            Password = PasswordManager.EncryptPassword("staff123"),
                            FirstName = "Staff",
                            LastName = "Member",
                            Email = "staff@nexdesk.com",
                            Role = "staff",
                            DepartmentId = "5", // Operations
                            IsActive = true,
                            CreatedBy = "System",
                            CreatedTime = DateTime.Now,
                            UpdatedTime = DateTime.Now
                        },
                        new User
                        {
                            Id = "4",
                            UserId = "USR004",
                            Username = "superadmin",
                            Password = PasswordManager.EncryptPassword("super123"),
                            FirstName = "Super",
                            LastName = "Admin",
                            Email = "superadmin@nexdesk.com",
                            Role = "superadmin",
                            DepartmentId = "5", // Operations/Management
                            IsActive = true,
                            CreatedBy = "System",
                            CreatedTime = DateTime.Now,
                            UpdatedTime = DateTime.Now
                        }
                    };

                    _context.Users.AddRange(testUsers);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Log error but continue
                Console.WriteLine($"Error seeding users: {ex.Message}");
            }
        }
    }
}

// In Program.cs or Startup.cs, inside the Main method or the appropriate method:
