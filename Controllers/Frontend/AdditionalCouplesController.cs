using AutoMapper;
using IpDeputyApi.Database;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Frontend;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;

namespace IpDeputyApi.Controllers.Frontend;

[Tags("Frontend AdditionalCouples Controller")]
[Route("api/frontend/additional-couple")]
[ApiController]
public class AdditionalCouplesController : ControllerBase
{
    private static ILogger Logger => Log.ForContext<AdditionalCouplesController>();
    private readonly IpDeputyDbContext _context;
    private readonly IMapper _mapper;

    public AdditionalCouplesController(IpDeputyDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<AdditionalCoupleDto>>> Get()
    {
        Logger.Debug("Start Get()");

        var dtos = await _context.AdditionalCouples
            .Select(x => _mapper.Map<AdditionalCoupleDto>(x))
            .ToListAsync();

        Logger.Debug("Result Get(dtos: {@dtos})", dtos);
        return Ok(dtos);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AdditionalCoupleDto>> Add(AdditionalCoupleDto dto)
    {
        Logger.Debug("Start Add(dto: {@dto})", dto);

        var model = _mapper.Map<AdditionalCouple>(dto);

        await _context.AddAsync(model);
        await _context.SaveChangesAsync();

        dto.Id = model.Id;

        Logger.Debug("Result Add(dto: {@dto})", dto);
        return Ok(dto);
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<AdditionalCoupleDto>> Update(AdditionalCoupleDto dto)
    {
        Logger.Debug("Start Update(dto: {@dto})", dto);

        if (!_context.AdditionalCouples.Any(x => x.Id == dto.Id))
        {
            Logger.Debug("Error Update: Invalid id");
            return BadRequest("Invalid id");
        }
        
        var model = _mapper.Map<AdditionalCouple>(dto);

        _context.Update(model);
        await _context.SaveChangesAsync();

        Logger.Debug("Result Update(dto: {@dto})", dto);
        return Ok(dto);
    }

    [Authorize]
    [HttpDelete]
    public async Task<ActionResult<int>> Delete(int id)
    {
        Logger.Debug("Start Delete(id: {@id})", id);

        var model = await _context.AdditionalCouples.FirstOrDefaultAsync(x => x.Id == id);

        if (model == null)
        {
            Logger.Debug("Error Update: Invalid id");
            return BadRequest("Invalid id");
        }
        
        _context.Remove(model);
        await _context.SaveChangesAsync();

        Logger.Debug("Result Delete()");
        return Ok(id);
    }
}