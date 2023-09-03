using AutoMapper;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Bot;
using IpDeputyApi.Dto.Frontend;
using DayOfWeek = IpDeputyApi.Database.Models.DayOfWeek;

namespace IpDeputyApi.Utilities
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            // Frontend mapping
            CreateMap<AdditionalCouple, AdditionalCoupleDto>().ReverseMap();
            CreateMap<Couple, CoupleDto>().ReverseMap();
            CreateMap<CoupleDate, CoupleDateDto>().ReverseMap();
            CreateMap<CoupleTime, CoupleTimeDto>().ReverseMap();
            CreateMap<DayOfWeek, DayOfWeekDto>().ReverseMap();
            CreateMap<Link, Link>().ReverseMap();
            CreateMap<Student, StudentDto>().ReverseMap();
            CreateMap<Subgroup, SubgroupDto>().ReverseMap();
            CreateMap<Subject, SubjectDto>().ReverseMap();
            CreateMap<SubjectType, SubjectTypeDto>().ReverseMap();
            CreateMap<SubmissionsConfig, IpDeputyApi.Dto.Frontend.SubmissionsConfigDto>().ReverseMap();
            CreateMap<SubmissionStudent, SubmissionStudentDto>().ReverseMap();
            CreateMap<SubmissionWork, SubmissionWorkDto>().ReverseMap();
            CreateMap<Teacher, TeacherDto>().ReverseMap();
            CreateMap<Telegram, TelegramDto>().ReverseMap();
            CreateMap<WorkDeadline, WorkDeadlineDto>().ReverseMap();
            
            // Bot mapping
            CreateMap<Telegram, StudentSettingsDto>()
                .ReverseMap()
                .ForMember(x => x.Id, opt => opt.MapFrom(src => 0));
            CreateMap<Student, StudentInformationDto>()
                .ForMember(x => x.Subgroup, opt => opt.MapFrom(src => src.Subgroup!.Name));
            CreateMap<Teacher, TeacherInformationDto>();
            CreateMap<Link, LinkInformationDto>();
        }
    }
}
