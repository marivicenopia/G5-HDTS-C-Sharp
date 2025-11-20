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
            // First, ensure the Departments table exists
            try
            {
                _context.Database.ExecuteSqlRaw(@"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Departments' AND xtype='U')
                    BEGIN
                        CREATE TABLE [dbo].[Departments](
                            [Id] [nvarchar](450) NOT NULL,
                            [Name] [nvarchar](max) NULL,
                            [Description] [nvarchar](max) NULL,
                            [IsActive] [bit] NOT NULL,
                            [CreatedTime] [datetime2](7) NOT NULL,
                            [CreatedBy] [nvarchar](max) NULL,
                            [UpdatedTime] [datetime2](7) NOT NULL,
                            [UpdatedBy] [nvarchar](max) NULL,
                         CONSTRAINT [PK_Departments] PRIMARY KEY CLUSTERED 
                        (
                            [Id] ASC
                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
                        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
                    END");
            }
            catch (Exception ex)
            {
                // Log error but continue - table might already exist
                Console.WriteLine($"Error creating Departments table: {ex.Message}");
            }

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
                // Log and continue if user seeding fails (schema mismatch possible)
                Console.WriteLine($"Error seeding users: {ex.Message}");
            }

            // Create articles if they don't exist
            try
            {
                if (!_context.Articles.Any())
                {
                    var articles = new[]
                    {
                        new Article
                        {
                            Id = "ART001",
                            Title = "Basics of Computer Hardware and Software",
                            Category = "Introduction to I.T",
                            Author = "System",
                            Content = "Overview of input, output, storage and processing components used in a modern computer."
                        },
                        new Article
                        {
                            Id = "ART002",
                            Title = "Understanding Operating Systems",
                            Category = "Introduction to I.T",
                            Author = "System",
                            Content = "Explains how operating systems manage resources, run applications and keep devices secure every day."
                        },
                        new Article
                        {
                            Id = "ART003",
                            Title = "Intro to Programming",
                            Category = "Coding & Dev",
                            Author = "System",
                            Content = "Covers variables, conditionals, loops and basic debugging steps to build simple programs safely."
                        },
                        new Article
                        {
                            Id = "ART004",
                            Title = "Software Dev Tips",
                            Category = "Coding & Dev",
                            Author = "System",
                            Content = "Practical tips on version control, readable code, peer reviews and small iterative releases."
                        },
                        new Article
                        {
                            Id = "ART005",
                            Title = "What is Cybersecurity?",
                            Category = "Cybersecurity",
                            Author = "System",
                            Content = "Introduces threats, security layers, authentication and monitoring to guard business systems."
                        },
                        new Article
                        {
                            Id = "ART006",
                            Title = "Protecting Accounts",
                            Category = "Cybersecurity",
                            Author = "System",
                            Content = "Guidelines on strong passwords, multi factor authentication and spotting phishing attempts quickly."
                        }
                    };

                    _context.Articles.AddRange(articles);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Log and continue if article seeding fails
                Console.WriteLine($"Error seeding articles: {ex.Message}");
            }
        }
    }
}