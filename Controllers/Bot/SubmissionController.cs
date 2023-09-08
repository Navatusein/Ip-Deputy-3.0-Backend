using AutoMapper;
using IpDeputyApi.Authentication;
using IpDeputyApi.Database;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Bot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;

namespace IpDeputyApi.Controllers.Bot;

[Tags("Bot Submission Controller")]
[Route("api/bot/submission")]
[ApiController]
public class SubmissionController : ControllerBase
{
    private static ILogger Logger => Log.ForContext<SubmissionController>();
    private readonly IpDeputyDbContext _context;
    private readonly IMapper _mapper;

    public SubmissionController(IpDeputyDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    [Authorize(AuthenticationSchemes = BotAuthenticationSchemeOptions.DefaultSchemeName)]
    [Route("submissions-configs")]
    [HttpGet]
    public async Task<ActionResult<List<SubmissionsConfigDataDto>>> GetSubmissionsConfigs()
    {
        Logger.Debug("Start GetSubmissionsConfigs()");
        var submissionsConfigs = await _context.SubmissionsConfigs.ToListAsync();

        var dtos = new List<SubmissionsConfigDataDto>();
        
        foreach (var submissionsConfig in submissionsConfigs)
        {
            dtos.Add(new SubmissionsConfigDataDto
            {
                Id = submissionsConfig.Id,
                Name = submissionsConfig.CustomName ?? submissionsConfig.Subject!.ShortName,
                Type = submissionsConfig.CustomType ?? submissionsConfig.SubjectType!.ShortName,
                Subgroup = submissionsConfig.Subgroup?.Name,
                Submissions = submissionsConfig.SubmissionStudents
                    .OrderBy(x => x.PreferredPosition)
                    .ThenBy(x => x.SubmissionWork.Name)
                    .Select(x => new SubmissionDto()
                    {
                        Name = x.SubmissionWork.Name,
                        Student = $"{x.Student.Surname} {x.Student.Name}"
                    })
            });
        }
        
        Logger.Debug("Result GetSubmissionsConfigs(dtos: {@dtos})", dtos);
        return Ok(dtos);
    }
}