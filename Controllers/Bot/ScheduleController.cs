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

[Tags("Bot Schedule Controller")]
[Route("api/bot/schedule")]
[ApiController]
public class ScheduleController : ControllerBase
{
    private static ILogger Logger => Log.ForContext<ScheduleController>();
    private readonly IpDeputyDbContext _context;
    private readonly IMapper _mapper;

    public ScheduleController(IpDeputyDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    [Authorize(AuthenticationSchemes = BotAuthenticationSchemeOptions.DefaultSchemeName)]
    [Route("day")]
    [HttpGet]
    public async Task<ActionResult<ScheduleDayDto>> GetDaySchedule(int telegramId, string dateString)
    {
        Logger.Debug("Start GetDaySchedule(telegramId:{@telegramId}, dateString:{@dateString})", 
            telegramId, dateString);
        
        var date = DateOnly.Parse(dateString);
        var telegram = await _context.Telegrams.FirstOrDefaultAsync(x => x.TelegramId == telegramId);
        
        if (telegram == null)
        {
            Logger.Debug("Error GetDaySchedule: No such student");
            return NoContent();
        }

        var scheduleDayDto = new ScheduleDayDto
        {
            Date = date.ToString("dd.MM"),
            Couples = await GetCouplesForDay(telegram.Student, date)
        };
        
        Logger.Debug("Result GetDaySchedule(scheduleDayDto: {@scheduleDayDto})", scheduleDayDto);
        return Ok(scheduleDayDto);
    }
    
    [Authorize(AuthenticationSchemes = BotAuthenticationSchemeOptions.DefaultSchemeName)]
    [Route("week")]
    [HttpGet]
    public async Task<ActionResult<ScheduleWeekDto>> GetWeekSchedule(int telegramId, string dateString)
    {
        Logger.Debug("Start GetWeekSchedule(telegramId:{@telegramId}, dateString:{@dateString})", 
            telegramId, dateString);
        
        var date = DateOnly.Parse(dateString);
        var startWeek = date.AddDays(-(date.DayOfWeek == 0 ? 6 : (int)date.DayOfWeek - 1));
        
        var telegram = await _context.Telegrams.FirstOrDefaultAsync(x => x.TelegramId == telegramId);
        
        if (telegram == null)
        {
            Logger.Debug("Error GetWeekSchedule: No such student");
            return NoContent();
        }

        var scheduleDayDtos = new List<ScheduleDayDto>();
        
        for (var i = 0; i < 7; i++)
        {
            var day = startWeek.AddDays(i);
            
            scheduleDayDtos.Add(new ScheduleDayDto
            {
                Date = day.ToString("dd.MM"),
                Couples = await GetCouplesForDay(telegram.Student, day)
            });
        }
        
        var scheduleWeekDto = new ScheduleWeekDto()
        {
            CoupleTimes = await _context.CoupleTimes
                .OrderBy(x => x.Index)
                .Select(x => x.GetTimeFormatted())
                .ToListAsync(),
            ScheduleDays = scheduleDayDtos
        };

        Logger.Debug("Result GetWeekSchedule(scheduleWeekDto: {@scheduleWeekDto})", scheduleWeekDto);
        return Ok(scheduleWeekDto);
    }

    private bool FilterCouples(Couple couple, DateOnly date)
    {
        if (couple.CoupleDates.Any(x => !x.IsRemovedDate && x.Date == date))
            return true;

        if (couple.CoupleDates.Any(x => x.IsRemovedDate && x.Date == date))
            return false;

        if (couple.StartDate == null)
            return false;
        
        if (date < couple.StartDate || date > couple.EndDate)
            return false;
        
        var startDate = couple.StartDate.Value.ToDateTime(new TimeOnly());
        var dateTime = date.ToDateTime(new TimeOnly());

        var weekSpan = (int)(dateTime - startDate).TotalDays / 7;

        if (couple.IsRolling && weekSpan % 2 == 1)
            return false;

        return true;
    }
    
    private async Task<List<CoupleDataDto>> GetCouplesForDay(Student student, DateOnly date)
    {
        var couples = await _context.Couples
            .Where(x => x.DayOfWeek.Index == (int)date.DayOfWeek)
            .OrderBy(x => x.CoupleTime.Index)
            .ToListAsync();

        couples = couples.Where(x => FilterCouples(x, date)).ToList(); 
        
        var coupleDataDtos = couples.Select(couple => new CoupleDataDto
        {
            Subject = couple.Subject.ShortName,
            SubjectType = couple.SubjectType.ShortName,
            CoupleIndex = couple.CoupleTime.Index - 1,
            Time = couple.CoupleTime.GetTimeFormatted(),
            IsMySubgroup = couple.SubgroupId == null || couple.SubgroupId == student.SubgroupId,
            Cabinet = couple.Cabinet,
            Link = couple.Link,
            AdditionalInformation = couple.AdditionalInformation,
        }).ToList();
        
        var additionalCouples = await _context.AdditionalCouples
            .Where(x => x.Date == date)
            .Select(x => new CoupleDataDto
            {
                Subject = x.Subject.ShortName,
                SubjectType = x.SubjectType.ShortName,
                Time = x.Time.ToString("HH:mm"),
                IsMySubgroup = x.SubgroupId == null || x.SubgroupId == student.SubgroupId,
                Cabinet = x.Cabinet,
                Link = x.Link,
                AdditionalInformation = x.AdditionalInformation,
            }).ToListAsync();
        
        coupleDataDtos.AddRange(additionalCouples);

        return coupleDataDtos
            .OrderByDescending(x => x.IsMySubgroup)
            .ThenBy(x => x.Time)
            .ToList();
    }
}