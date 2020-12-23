using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMS.Data.Entities;
using PMS.Data.Entities.Project;
using PMS.Data.Repository;
using PMS.Mapping;
using PMS.Mapping.AutoMapperConfiguration;

namespace PMS.Service.ProjectServices
{
    public class ProjectService : IProjectService
    {
        private readonly IAutoMapperService _autoMapperService;
        private readonly IRepository<Project> _projectRepository;

        public ProjectService(IRepository<Project> projectRepository,
                              IAutoMapperService autoMapperService)
        {
            _autoMapperService = autoMapperService;
            _projectRepository = projectRepository;
        }

        public ProjectModel AddProject(ProjectModel project)
        {
            var data = _projectRepository.Add(_autoMapperService.Map<ProjectModel,Project>(project));
            return _autoMapperService.Map<Project, ProjectModel>(data);
        }

        public ProjectModel UpdateProject(ProjectModel project)
        {
            var data = _projectRepository.Update(_autoMapperService.Map<ProjectModel, Project>(project));
            return _autoMapperService.Map<Project, ProjectModel>(data);
        }
        public ProjectModel GetProjectByCode(string Code)
        {
            return _autoMapperService.Map<Project,ProjectModel>( 
                _projectRepository.Filter(a => a.ProjectCode == Code).SingleOrDefault() );
        }

        public ICollection<ProjectModel> GetProjects()
        {
            var Projects = _projectRepository.GetAll();
            return _autoMapperService.MapCollection<Project,ProjectModel>(Projects).ToList();
        }

        public async Task<ICollection<ProjectModel>> GetProjectsAsync()
        {
            var Projects = await _projectRepository.FindAllAsync(a => a.isActive == true);
            return _autoMapperService.MapCollection<Project, ProjectModel>(Projects).ToList();
        }

        public async Task<ICollection<ProjectModel>> GetProjectsAsyncByCompanyId(int CompanyId)
        {
            var projects = await _projectRepository.FilterAsync(a => a.LogoFirmCode == CompanyId.ToString());

            return _autoMapperService.MapCollection<Project,ProjectModel>(projects).ToList();
        }
    }
}
