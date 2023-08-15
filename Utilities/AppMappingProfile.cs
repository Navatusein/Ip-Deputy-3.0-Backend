using AutoMapper;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Frontend;
using DayOfWeek = IpDeputyApi.Database.Models.DayOfWeek;

namespace IpDeputyApi.Utilities
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
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
            CreateMap<SubmissionsConfig, SubmissionsConfigDto>().ReverseMap()
                .ForMember(x => x.SubmissionWorks, opt => opt.Ignore())
                .ForMember(x => x.SubmissionStudents, opt => opt.Ignore());
            CreateMap<SubmissionStudent, SubmissionStudentDto>().ReverseMap();
            CreateMap<SubmissionWork, SubmissionWorkDto>().ReverseMap();
            CreateMap<Teacher, TeacherDto>().ReverseMap();
            CreateMap<Telegram, Telegram>().ReverseMap();
            CreateMap<WorkDeadline, WorkDeadlineDto>().ReverseMap();
        }
    }
}
