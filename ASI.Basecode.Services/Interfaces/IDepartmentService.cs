using ASI.Basecode.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<IEnumerable<Department>> GetActiveDepartmentsAsync();
        Task<Department> GetDepartmentByIdAsync(string id);
        Task<Department> GetDepartmentByNameAsync(string name);
        Task<Department> CreateDepartmentAsync(Department department);
        Task<Department> UpdateDepartmentAsync(Department department);
        Task<bool> DeleteDepartmentAsync(string id);
        Task<bool> DepartmentExistsAsync(string name);
    }
}