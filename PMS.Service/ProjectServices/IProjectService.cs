using PMS.Mapping;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Service.ProjectServices
{
    public interface IProjectService
    {
        ICollection<ProjectModel> GetProjects();
        Task<ICollection<ProjectModel>> GetProjectsAsync();
        Task<ICollection<ProjectModel>> GetProjectsAsyncByCompanyId(int CompanyId);
        ProjectModel GetProjectByCode(string Code);
        ProjectModel AddProject(ProjectModel project);
        ProjectModel UpdateProject(ProjectModel project);
    }
}
