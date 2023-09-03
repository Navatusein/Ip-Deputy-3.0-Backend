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

[Tags("Bot Authentication Controller")]
[Route("api/bot/authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private static ILogger Logger => Log.ForContext<AuthenticationController>();
    private readonly IpDeputyDbContext _context;
    private readonly IMapper _mapper;

    public AuthenticationController(IpDeputyDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    [Authorize(AuthenticationSchemes = BotAuthenticationSchemeOptions.DefaultSchemeName)]
    [Route("authorize")]
    [HttpPost]
    public async Task<ActionResult<string>> Authorize([FromBody]StudentContactDto contactDto)
    {
        Logger.Debug("Start Authorize(contact:{@contact})", contactDto);

        var student = await _context.Students.FirstOrDefaultAsync(x => x.TelegramPhone == contactDto.Phone);

        if (student == null)
        {
            var code = Guid.NewGuid().ToString()[..8];
            var phone = contactDto.Phone;
            Logger.Error("[{@code}] Student with phone: {@phone} not found!", code, phone);
            return Ok(code);
        }
        
        var telegram = new Telegram()
        {
            StudentId = student.Id,
            TelegramId = contactDto.TelegramId,
            Language = "uk",
            ScheduleCompact = false,
            RemindDeadlines = false
        };

        await _context.AddAsync(telegram);
        await _context.SaveChangesAsync();
        
        Logger.Debug("Result Authorize()", contactDto);
        return Ok("Ok");
    }
}