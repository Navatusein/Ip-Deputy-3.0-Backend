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

[Tags("Frontend Subject Controller")]
[Route("api/frontend/subject")]
[ApiController]
public class SubjectController : ControllerBase
{
    private static ILogger Logger => Log.ForContext<SubjectController>();
    private readonly IpDeputyDbContext _context;
    private readonly IMapper _mapper;

    public SubjectController(IpDeputyDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<SubjectDto>>> Get()
    {
        Logger.Debug("Start Get()");

        var dtos = await _context.Subjects
            .Select(x => _mapper.Map<SubjectDto>(x))
            .ToListAsync();

        Logger.Debug("Result Get(dtos: {@dtos})", dtos);
        return Ok(dtos);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<SubjectDto>> Add(SubjectDto dto)
    {
        Logger.Debug("Start Add(dto: {@dto})", dto);

        var model = _mapper.Map<Subject>(dto);

        await _context.AddAsync(model);
        await _context.SaveChangesAsync();

        dto.Id = model.Id;

        Logger.Debug("Result Add(dto: {@dto})", dto);
        return Ok(dto);
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<SubjectDto>> Update(SubjectDto dto)
    {
        Logger.Debug("Start Update(dto: {@dto})", dto);

        if (!_context.Subjects.Any(x => x.Id == dto.Id))
        {
            Logger.Debug("Error Update: Invalid id");
            return BadRequest("Invalid id");
        }
        
        var model = _mapper.Map<Subject>(dto);

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

        var model = await _context.Subjects.FirstOrDefaultAsync(x => x.Id == id);

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