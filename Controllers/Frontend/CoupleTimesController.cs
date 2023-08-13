using AutoMapper;
using IpDeputyApi.Database;
using IpDeputyApi.Dto.Frontend;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;

namespace IpDeputyApi.Controllers.Frontend;

[Tags("Frontend CoupleTimes Controller")]
[Route("api/frontend/couple-time")]
[ApiController]
public class CoupleTimesController : ControllerBase
{
    private static ILogger Logger => Log.ForContext<CoupleTimesController>();
    private readonly IpDeputyDbContext _context;
    private readonly IMapper _mapper;

    public CoupleTimesController(IpDeputyDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<CoupleTimeDto>>> Get()
    {
        Logger.Debug("Start Get()");

        var dtos = await _context.CoupleTimes
            .OrderBy(x => x.TimeStart)
            .Select(x => _mapper.Map<CoupleTimeDto>(x))
            .ToListAsync();

        Logger.Debug("Result Get(dtos: {@dtos})", dtos);
        return Ok(dtos);
    }
}