using System.Collections;
using AutoMapper;
using IpDeputyApi.Authentication;
using IpDeputyApi.Database;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Bot;
using IpDeputyApi.Dto.Frontend;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;

namespace IpDeputyApi.Controllers.Bot;

[Tags("Bot Information Controller")]
[Route("api/bot/information")]
[ApiController]
public class InformationController : ControllerBase
{
    private static ILogger Logger => Log.ForContext<InformationController>();
    private readonly IpDeputyDbContext _context;
    private readonly IMapper _mapper;

    public InformationController(IpDeputyDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [Authorize(AuthenticationSchemes = BotAuthenticationSchemeOptions.DefaultSchemeName)]
    [Route("students")]
    [HttpGet]
    public async Task<ActionResult<List<StudentInformationDto>>> GetStudentsInformation()
    {
        Logger.Debug("Start GetStudentsInformation()");

        var students = await _context.Students
            .OrderBy(x => x.Index)
            .ToListAsync();

        var studentInformationDtos = students
            .Select(x => _mapper.Map<StudentInformationDto>(x))
            .ToList();

        Logger.Debug("Result GetStudentsInformation(studentInformationDtos: {@studentInformationDtos})",
            studentInformationDtos);
        return Ok(studentInformationDtos);
    }

    [Authorize(AuthenticationSchemes = BotAuthenticationSchemeOptions.DefaultSchemeName)]
    [Route("teachers")]
    [HttpGet]
    public async Task<ActionResult<List<TeacherInformationDto>>> GetTeachersInformation()
    {
        Logger.Debug("Start GetTeachersInformation()");

        var teachers = await _context.Teachers
            .OrderBy(x => x.Surname)
            .ToListAsync();

        var teacherInformationDtos = teachers
            .Select(x => _mapper.Map<TeacherInformationDto>(x))
            .ToList();
        
        Logger.Debug("Result GetTeachersInformation(teacherInformationDtos: {@teacherInformationDtos})",
            teacherInformationDtos);
        return Ok(teacherInformationDtos);
    }

    [Authorize(AuthenticationSchemes = BotAuthenticationSchemeOptions.DefaultSchemeName)]
    [Route("subjects")]
    [HttpGet]
    public async Task<ActionResult<List<SubjectInformationDto>>> GetSubjectsInformation()
    {
        Logger.Debug("Start GetSubjectsInformation()");

        var subjects = await _context.Subjects.ToListAsync();
        var subjectInformationDtos = new List<SubjectInformationDto>();
        
        foreach (var subject in subjects)
        {
            var subjectInformationDto = new SubjectInformationDto
            {
                Name = subject.Name,
                ShortName = subject.ShortName,
                LaboratoryCount = subject.LaboratoryCount,
                PracticalCount = subject.PracticalCount,
                LaboratoryDaysCount = await CalculateDays(subject, 1),
                PracticalDaysCount = await CalculateDays(subject, 2),
                LecturesDaysCount = await CalculateDays(subject, 3)
            };
            
            subjectInformationDtos.Add(subjectInformationDto);
        }
        Logger.Debug("Result GetSubjectsInformation(subjectInformationDtos: {@subjectInformationDtos})", 
            subjectInformationDtos);
        return Ok(subjectInformationDtos);
    }
    
    [Authorize(AuthenticationSchemes = BotAuthenticationSchemeOptions.DefaultSchemeName)]
    [Route("links")]
    [HttpGet]
    public async Task<ActionResult<List<LinkInformationDto>>> GetLinksInformation()
    {
        return await _context.Links.Select(x => _mapper.Map<LinkInformationDto>(x)).ToListAsync();
    }
    

    private async Task<int> CalculateDays(Subject subject, int subjectTypeId)
    {
        var couples = subject.Couples
            .Where(x => x.SubjectTypeId == subjectTypeId)
            .GroupBy(x => x.DayOfWeek)
            .Select(x => x.First());
        
        var days = new HashSet<DateTime>();
        
        Logger.Debug("subject: {@subject}", subject.Name);
        
        foreach (var couple in couples)
        {
            if (couple.StartDate != null)
            {
                var startDate = couple.StartDate.Value.ToDateTime(new TimeOnly());
                var endDate = couple.EndDate!.Value.ToDateTime(new TimeOnly());
                
                for (var i = startDate; i <= endDate; i = i.AddDays(couple.IsRolling ? 14 : 7))
                {
                    if (i >= DateTime.Today)
                        days.Add(i);
                }
            }

            days.UnionWith(couple.CoupleDates
                .Where(x => !x.IsRemovedDate)
                .Select(x => x.Date.ToDateTime(new TimeOnly()))
            );

            days.RemoveWhere(x => couple.CoupleDates.Any(y => y.Date == DateOnly.FromDateTime(x)));
        }

        var additionalCouples = await _context.AdditionalCouples
            .Where(x => x.SubjectId == subject.Id && x.SubjectTypeId == subjectTypeId)
            .ToListAsync();
        
        days.UnionWith(additionalCouples
            .Select(x => x.Date.ToDateTime(new TimeOnly()))
        );

        return days.Count;
    }
}