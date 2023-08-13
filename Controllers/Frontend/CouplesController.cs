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

[Tags("Frontend CouplesController")]
[Route("api/frontend/")]
[ApiController]
public class CouplesController : ControllerBase
{
    private static ILogger Logger => Log.ForContext<CouplesController>();
    private readonly IpDeputyDbContext _context;
    private readonly IMapper _mapper;

    public CouplesController(IpDeputyDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<CoupleDto>>> GetByDayOfWeekId(int dayOfWeekId)
    {
        Logger.Debug("Start GetByDayOfWeekId(dayOfWeekId: {@dayOfWeekId})", dayOfWeekId);

        var models = await _context.Couples
            .Where(x => x.DayOfWeekId == dayOfWeekId)
            .OrderBy(x => x.DayOfWeekId)
            .ThenBy(x => x.CoupleTimeId)
            .ThenBy(x => x.StartDate)
            .ToListAsync();

        var dtos = new List<CoupleDto>();
        
        foreach (var couple in models)
        {
            var dto = _mapper.Map<CoupleDto>(couple);

            dto.AdditionalDates = couple.CoupleDates
                .Where(x => x.IsRemovedDate == false)
                .Select(x => _mapper.Map<CoupleDateDto>(x))
                .ToList();
            
            dto.RemovedDates = couple.CoupleDates
                .Where(x => x.IsRemovedDate == true)
                .Select(x => _mapper.Map<CoupleDateDto>(x))
                .ToList();
            
            dtos.Add(dto);
        }
        
        Logger.Debug("Result Get(dtos: {@dtos}", dtos);
        return Ok(dtos);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CoupleDto>> Add(CoupleDto dto)
    {
        Logger.Debug("Start Add(dto: {@dto})", dto);

        var model = _mapper.Map<Couple>(dto);

        await _context.AddAsync(model);
        await _context.SaveChangesAsync();

        await _context.AddRangeAsync(dto.AdditionalDates
            .Select(x => _mapper.Map<CoupleDate>(x, opt =>
                {
                    opt.AfterMap((_, dest) => dest.CoupleId = model.Id);
                    opt.AfterMap((_, dest) => dest.IsRemovedDate = false);
                }))
            .ToList()
        );

        await _context.AddRangeAsync(dto.RemovedDates
            .Select(x => _mapper.Map<CoupleDate>(x, opt =>
            {
                opt.AfterMap((_, dest) => dest.CoupleId = model.Id);
                opt.AfterMap((_, dest) => dest.IsRemovedDate = true);
            }))
            .ToList()
        );
        
        await _context.SaveChangesAsync();
        
        dto.Id = model.Id;

        Logger.Debug("Result Add(dto: {@dto})", dto);
        return Ok(dto);
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<CoupleDto>> Update(CoupleDto dto)
    {
        Logger.Debug("Start Update(dto: {@dto})", dto);

        if (!_context.Couples.Any(x => x.Id == dto.Id))
        {
            Logger.Debug("Error Update: Invalid id");
            return BadRequest("Invalid id");
        }
        
        var model = _mapper.Map<Couple>(dto);

        _context.Update(model);
        _context.RemoveRange(await _context.CoupleDates.Where(x => x.CoupleId == dto.Id).ToListAsync());
        await _context.SaveChangesAsync();

        await _context.AddRangeAsync(dto.AdditionalDates
            .Select(x => _mapper.Map<CoupleDate>(x, opt =>
            {
                opt.AfterMap((_, dest) => dest.CoupleId = model.Id);
                opt.AfterMap((_, dest) => dest.IsRemovedDate = false);
            }))
            .ToList()
        );

        await _context.AddRangeAsync(dto.RemovedDates
            .Select(x => _mapper.Map<CoupleDate>(x, opt =>
            {
                opt.AfterMap((_, dest) => dest.CoupleId = model.Id);
                opt.AfterMap((_, dest) => dest.IsRemovedDate = true);
            }))
            .ToList()
        );
        
        await _context.SaveChangesAsync();
        
        Logger.Debug("Result Update(dto: {@dto})", dto);
        return Ok(dto);
    }

    [Authorize]
    [HttpDelete]
    public async Task<ActionResult<int>> Delete(int id)
    {
        Logger.Debug("Start Delete(id: {@id})", id);

        var model = await _context.Couples.FirstOrDefaultAsync(x => x.Id == id);

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