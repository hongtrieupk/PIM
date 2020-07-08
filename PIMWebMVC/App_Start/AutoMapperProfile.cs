using AutoMapper;
using PIM.Common.Models;
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
            CreateMap<Project, ProjectForViewTableModel>();
            CreateMap<PagingResultModel<Project>, ProjectsPaginationResult>()
                .ForMember(x => x.Projects, source => source.MapFrom(x => x.Records));
        }
    }
}