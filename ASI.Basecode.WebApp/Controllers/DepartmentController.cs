using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.WebApp.Models;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.WebApp.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous] // For development - you may want to add proper authorization later
    public class DepartmentController : ControllerBase<DepartmentController>
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper,
            IDepartmentService departmentService) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _departmentService = departmentService;
        }

        /// <summary>
        /// Get all departments
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetDepartments()
        {
            try
            {
                var departments = await _departmentService.GetAllDepartmentsAsync();

                var departmentList = departments.Select(dept => new
                {
                    id = dept.Id,
                    name = dept.Name,
                    description = dept.Description,
                    isActive = dept.IsActive,
                    createdTime = dept.CreatedTime,
                    updatedTime = dept.UpdatedTime
                }).ToList();

                return Ok(new ApiResult<object>(Status.Success, departmentList, "Departments retrieved successfully"));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error retrieving departments");
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred while retrieving departments"));
            }
        }

        /// <summary>
        /// Get active departments only
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveDepartments()
        {
            try
            {
                var departments = await _departmentService.GetActiveDepartmentsAsync();

                var departmentList = departments.Select(dept => new
                {
                    id = dept.Id,
                    name = dept.Name,
                    description = dept.Description
                }).ToList();

                return Ok(new ApiResult<object>(Status.Success, departmentList, "Active departments retrieved successfully"));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error retrieving active departments");
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred while retrieving active departments"));
            }
        }

        /// <summary>
        /// Get department by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartment(string id)
        {
            try
            {
                var department = await _departmentService.GetDepartmentByIdAsync(id);

                if (department == null)
                {
                    return NotFound(new ApiResult<object>(Status.Error, null, "Department not found"));
                }

                var response = new
                {
                    id = department.Id,
                    name = department.Name,
                    description = department.Description,
                    isActive = department.IsActive,
                    createdTime = department.CreatedTime,
                    updatedTime = department.UpdatedTime
                };

                return Ok(new ApiResult<object>(Status.Success, response, "Department found"));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error getting department: {Id}", id);
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred while retrieving department"));
            }
        }

        /// <summary>
        /// Create new department
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(new ApiResult<object>(Status.Error, errors, "Invalid request data"));
                }

                var department = new Department
                {
                    Name = request.Name,
                    Description = request.Description,
                    CreatedBy = "System" // You can get this from the authenticated user context
                };

                var createdDepartment = await _departmentService.CreateDepartmentAsync(department);

                var response = new
                {
                    id = createdDepartment.Id,
                    name = createdDepartment.Name,
                    description = createdDepartment.Description,
                    isActive = createdDepartment.IsActive,
                    createdTime = createdDepartment.CreatedTime
                };

                return Ok(new ApiResult<object>(Status.Success, response, "Department created successfully"));
            }
            catch (ArgumentException ex)
            {
                this._logger.LogWarning(ex, "Department creation validation failed: {ErrorMessage}", ex.Message);
                return BadRequest(new ApiResult<object>(Status.Error, null, ex.Message));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error creating department: {ErrorMessage}", ex.Message);
                return StatusCode(500, new ApiResult<object>(Status.Error, null, $"An error occurred while creating department: {ex.Message}"));
            }
        }

        /// <summary>
        /// Update department
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(string id, [FromBody] UpdateDepartmentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(new ApiResult<object>(Status.Error, errors, "Invalid request data"));
                }

                var department = new Department
                {
                    Id = id,
                    Name = request.Name,
                    Description = request.Description,
                    IsActive = request.IsActive,
                    UpdatedBy = "System" // You can get this from the authenticated user context
                };

                var updatedDepartment = await _departmentService.UpdateDepartmentAsync(department);

                var response = new
                {
                    id = updatedDepartment.Id,
                    name = updatedDepartment.Name,
                    description = updatedDepartment.Description,
                    isActive = updatedDepartment.IsActive,
                    updatedTime = updatedDepartment.UpdatedTime
                };

                return Ok(new ApiResult<object>(Status.Success, response, "Department updated successfully"));
            }
            catch (ArgumentException ex)
            {
                this._logger.LogWarning(ex, "Department update validation failed: {ErrorMessage}", ex.Message);
                return BadRequest(new ApiResult<object>(Status.Error, null, ex.Message));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error updating department: {ErrorMessage}", ex.Message);
                return StatusCode(500, new ApiResult<object>(Status.Error, null, $"An error occurred while updating department: {ex.Message}"));
            }
        }

        /// <summary>
        /// Delete department (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(string id)
        {
            try
            {
                var result = await _departmentService.DeleteDepartmentAsync(id);

                if (!result)
                {
                    return NotFound(new ApiResult<object>(Status.Error, null, "Department not found"));
                }

                return Ok(new ApiResult<object>(Status.Success, null, "Department deleted successfully"));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error deleting department: {Id}", id);
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred while deleting department"));
            }
        }
    }

    // Request models
    public class CreateDepartmentRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UpdateDepartmentRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}