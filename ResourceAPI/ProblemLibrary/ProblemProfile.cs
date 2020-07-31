using AutoMapper;

namespace ProblemLibrary
{
    public class ProblemProfile : Profile
    {
        public ProblemProfile()
        {
            CreateMap<Problem, ProblemViewModel>()
                //.ForAllMembers(o=>{})
                .ForMember(dest => dest.Id, x => x.MapFrom(y => y.Id))
                .ForMember(
                    dest => dest.Name,
                    x => x.MapFrom(y => y.Name)
                )
                .ForMember(
                    dest => dest.ContentHtml,
                    x => x.MapFrom(y =>
                        y.Render().ContentHtml
                    )
                )
                .ReverseMap()
                ;
        }
    }
}