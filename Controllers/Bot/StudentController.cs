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

[Tags("Bot StudentController")]
[Route("api/bot/student")]
[ApiController]
public class StudentController : ControllerBase
{
    private static ILogger Logger => Log.ForContext<StudentController>();
    private readonly IpDeputyDbContext _context;
    private readonly IMapper _mapper;

    public StudentController(IpDeputyDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    [Authorize(AuthenticationSchemes = BotAuthenticationSchemeOptions.DefaultSchemeName)]
    [Route("settings")]
    [HttpGet]
    public async Task<ActionResult<StudentSettingsDto>> GetSettings(int telegramId)
    {
        Logger.Debug("Start GetSettings(telegramId:{@telegramId})", telegramId);

        var telegram = await _context.Telegrams.FirstOrDefaultAsync(x => x.TelegramId == telegramId);
        var settings = _mapper.Map<StudentSettingsDto>(telegram);
        
        Logger.Debug("Result GetSettings(settings: {@settings})", settings);
        return Ok(settings);
    }
    
    [Authorize(AuthenticationSchemes = BotAuthenticationSchemeOptions.DefaultSchemeName)]
    [Route("settings")]
    [HttpPost]
    public async Task<ActionResult<StudentSettingsDto>> UpdateSettings(StudentSettingsDto settingsDto)
    {
        Logger.Debug("Start UpdateSettings(telegramId:{@settingsDto})", settingsDto);

        var telegram = await _context.Telegrams.FirstOrDefaultAsync(x => x.TelegramId == settingsDto.TelegramId);

        if (telegram == null)
        {
            Logger.Debug("Error UpdateSettings: No such student");
            return BadRequest();
        }

        telegram.Language = settingsDto.Language;
        telegram.ScheduleCompact = settingsDto.ScheduleCompact;
        telegram.RemindDeadlines = settingsDto.RemindDeadlines;

        await _context.SaveChangesAsync();
        
        Logger.Debug("Result UpdateSettings(settings: {@settingsDto})", settingsDto);
        return Ok(settingsDto);
    }
}