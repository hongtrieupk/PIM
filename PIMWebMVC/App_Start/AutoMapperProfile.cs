
using AutoMapper;
using PIM.Data.Objects;
using PIMWebMVC.Models.Projects;

namespace PIMWebMVC.App_Start
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Project, ProjectModel>();
            CreateMap<ProjectModel, Project>();
        }
    }
}