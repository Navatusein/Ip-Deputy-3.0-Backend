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

[Tags("Frontend SubmissionsConfigs Controller")]
[Route("api/frontend/submissions-config")]
[ApiController]
public class SubmissionsConfigsController : ControllerBase
{
    private static ILogger Logger => Log.ForContext<SubmissionsConfigsController>();
    private readonly IpDeputyDbContext _context;
    private readonly IMapper _mapper;

    public SubmissionsConfigsController(IpDeputyDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [Authorize]
    [HttpGet]
    [Route("for-student")]
    public async Task<ActionResult<List<SubmissionsConfigDto>>> GetForStudent(int studentId)
    {
        Logger.Debug("GetForStudent Get(studentId: {studentId})", studentId);

        var student = await _context.Students.FirstOrDefaultAsync(x => x.Id == studentId);
        
        if (student == null)
        {
            Logger.Debug("Error GetForStudent: No such studentId");
            return BadRequest("No such studentId");
        }
        
        var dtos = await _context.SubmissionsConfigs
            .Where(x => x.SubgroupId == null || x.SubgroupId == student.SubgroupId)
            .Select(x => _mapper.Map<SubmissionsConfigDto>(x))
            .ToListAsync();
        
        Logger.Debug("GetForStudent Get(dtos: {@dtos})", dtos);
        return Ok(dtos);
    }
    
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<SubmissionsConfigDto>>> Get()
    {
        Logger.Debug("Start Get()");
        
        var dtos = await _context.SubmissionsConfigs
            .Select(x => _mapper.Map<SubmissionsConfigDto>(x))
            .ToListAsync();
        
        Logger.Debug("Result Get(dtos: {@dtos})", dtos);
        return Ok(dtos);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<SubmissionsConfigDto>> Add(SubmissionsConfigDto dto)
    {
        Logger.Debug("Start Add(dto: {@dto})", dto);

        var model = _mapper.Map<SubmissionsConfig>(dto);

        await _context.AddAsync(model);
        await _context.SaveChangesAsync();

        dto = _mapper.Map<SubmissionsConfigDto>(model);

        await _context.SaveChangesAsync();
        
        Logger.Debug("Result Add(dto: {@dto})", dto);
        return Ok(dto);
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<SubmissionsConfigDto>> Update(SubmissionsConfigDto dto)
    {
        Logger.Debug("Start Update(dto: {@dto})", dto);

        if (!_context.SubmissionsConfigs.Any(x => x.Id == dto.Id))
        {
            Logger.Debug("Error Update: Invalid id");
            return BadRequest("Invalid id");
        }

        var model = _mapper.Map<SubmissionsConfig>(dto);

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

        var model = await _context.SubmissionsConfigs.FirstOrDefaultAsync(x => x.Id == id);

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