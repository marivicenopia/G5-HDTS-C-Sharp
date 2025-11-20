using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            var departments = await _unitOfWork.Database.Set<Department>()
                .ToListAsync();

            // Sort by ID as integer on the client side
            return departments.OrderBy(d => int.Parse(d.Id));
        }

        public async Task<IEnumerable<Department>> GetActiveDepartmentsAsync()
        {
            var departments = await _unitOfWork.Database.Set<Department>()
                .Where(d => d.IsActive)
                .ToListAsync();

            // Sort by ID as integer on the client side after fetching from database
            return departments.OrderBy(d => int.Parse(d.Id));
        }

        public async Task<Department> GetDepartmentByIdAsync(string id)
        {
            return await _unitOfWork.Database.Set<Department>()
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Department> GetDepartmentByNameAsync(string name)
        {
            return await _unitOfWork.Database.Set<Department>()
                .FirstOrDefaultAsync(d => d.Name == name);
        }

        public async Task<bool> DepartmentExistsAsync(string name)
        {
            return await _unitOfWork.Database.Set<Department>()
                .AnyAsync(d => d.Name.ToLower() == name.ToLower());
        }

        public async Task<Department> CreateDepartmentAsync(Department department)
        {
            // Check if department already exists
            if (await DepartmentExistsAsync(department.Name))
            {
                throw new ArgumentException($"Department '{department.Name}' already exists.");
            }

            // Generate sequential ID
            var allDepartments = await _unitOfWork.Database.Set<Department>().ToListAsync();
            var maxId = 0;

            foreach (var existingDept in allDepartments)
            {
                if (int.TryParse(existingDept.Id, out int id) && id > maxId)
                {
                    maxId = id;
                }
            }

            department.Id = (maxId + 1).ToString();
            department.IsActive = true;
            department.CreatedTime = DateTime.Now;
            department.UpdatedTime = DateTime.Now;

            await _unitOfWork.Database.Set<Department>().AddAsync(department);
            await _unitOfWork.Database.SaveChangesAsync();

            return department;
        }

        public async Task<Department> UpdateDepartmentAsync(Department department)
        {
            var existingDepartment = await GetDepartmentByIdAsync(department.Id);
            if (existingDepartment == null)
            {
                throw new ArgumentException($"Department with ID '{department.Id}' not found.");
            }

            // Check if new name conflicts with existing departments (excluding current one)
            var existingWithSameName = await _unitOfWork.Database.Set<Department>()
                .FirstOrDefaultAsync(d => d.Name.ToLower() == department.Name.ToLower() && d.Id != department.Id);

            if (existingWithSameName != null)
            {
                throw new ArgumentException($"Department name '{department.Name}' is already in use.");
            }

            existingDepartment.Name = department.Name;
            existingDepartment.Description = department.Description;
            existingDepartment.IsActive = department.IsActive;
            existingDepartment.UpdatedTime = DateTime.Now;
            existingDepartment.UpdatedBy = department.UpdatedBy;

            await _unitOfWork.Database.SaveChangesAsync();
            return existingDepartment;
        }

        public async Task<bool> DeleteDepartmentAsync(string id)
        {
            var department = await GetDepartmentByIdAsync(id);
            if (department == null)
            {
                return false;
            }

            // Soft delete - mark as inactive instead of removing
            department.IsActive = false;
            department.UpdatedTime = DateTime.Now;

            await _unitOfWork.Database.SaveChangesAsync();
            return true;
        }
    }
}