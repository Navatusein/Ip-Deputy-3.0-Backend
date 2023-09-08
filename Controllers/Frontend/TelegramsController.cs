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

[Tags("Frontend Telegrams Controller")]
[Route("api/frontend/telegram")]
[ApiController]
public class TelegramsController : ControllerBase
{
    private static ILogger Logger => Log.ForContext<TelegramsController>();
    private readonly IpDeputyDbContext _context;
    private readonly IMapper _mapper;

    public TelegramsController(IpDeputyDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<TelegramDto>>> Get()
    {
        Logger.Debug("Start Get()");

        var dtos = await _context.Telegrams
            .Select(x => _mapper.Map<TelegramDto>(x))
            .ToListAsync();

        Logger.Debug("Result Get(dtos: {@dtos})", dtos);
        return Ok(dtos);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<TelegramDto>> Add(TelegramDto dto)
    {
        Logger.Debug("Start Add(dto: {@dto})", dto);

        var model = _mapper.Map<Telegram>(dto);

        await _context.AddAsync(model);
        await _context.SaveChangesAsync();

        dto.Id = model.Id;

        Logger.Debug("Result Add(dto: {@dto})", dto);
        return Ok(dto);
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<TelegramDto>> Update(TelegramDto dto)
    {
        Logger.Debug("Start Update(dto: {@dto})", dto);

        if (!_context.Telegrams.Any(x => x.Id == dto.Id))
        {
            Logger.Debug("Error Update: Invalid id");
            return BadRequest("Invalid id");
        }
        
        var model = _mapper.Map<Telegram>(dto);

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

        var model = await _context.Telegrams.FirstOrDefaultAsync(x => x.Id == id);

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