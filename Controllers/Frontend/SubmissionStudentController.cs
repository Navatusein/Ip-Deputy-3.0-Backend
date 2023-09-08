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

[Tags("Frontend SubmissionStudent Controller")]
[Route("api/frontend/submission-student")]
[ApiController]
public class SubmissionStudentController : ControllerBase
{
    private static ILogger Logger => Log.ForContext<SubmissionStudentController>();
    private readonly IpDeputyDbContext _context;
    private readonly IMapper _mapper;

    public SubmissionStudentController(IpDeputyDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<SubmissionStudentDto>>> Get()
    {
        Logger.Debug("Start Get()");

        var dtos = await _context.SubmissionStudents
            .OrderBy(x => x.SubmissionsConfigId)
            .ThenBy(x => x.SubmissionWork.Name)
            .Select(x => _mapper.Map<SubmissionStudentDto>(x))
            .ToListAsync();

        Logger.Debug("Result GetByStudent(dtos: {@dtos})", dtos);
        return Ok(dtos);
    }
    
    [Authorize]
    [HttpGet]
    [Route("by-student")]
    public async Task<ActionResult<List<SubmissionStudentDto>>> GetByStudent(int studentId)
    {
        Logger.Debug("Start GetByStudent(studentId: {studentId})", studentId);

        var dtos = await _context.SubmissionStudents
            .Where(x => x.StudentId == studentId)
            .OrderBy(x => x.SubmissionsConfigId)
            .ThenBy(x => x.SubmissionWork.Name)
            .Select(x => _mapper.Map<SubmissionStudentDto>(x))
            .ToListAsync();

        Logger.Debug("Result GetByStudent(dtos: {@dtos})", dtos);
        return Ok(dtos);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<SubmissionStudentDto>> Add(SubmissionStudentDto dto)
    {
        Logger.Debug("Start Add(dto: {@dto})", dto);

        var model = _mapper.Map<SubmissionStudent>(dto);

        var submissions = await _context.SubmissionStudents
            .Where(x => x.SubmissionsConfigId == dto.SubmissionsConfigId && x.StudentId == dto.StudentId)
            .ToListAsync();
        
        submissions.ForEach(x => x.PreferredPosition = dto.PreferredPosition);
        
        await _context.AddAsync(model);
        await _context.SaveChangesAsync();

        dto.Id = model.Id;

        Logger.Debug("Result Add(dto: {@dto})", dto);
        return Ok(dto);
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<SubmissionStudentDto>> Update(SubmissionStudentDto dto)
    {
        Logger.Debug("Start Update(dto: {@dto})", dto);

        if (!_context.SubmissionStudents.Any(x => x.Id == dto.Id))
        {
            Logger.Debug("Error Update: Invalid id");
            return BadRequest("Invalid id");
        }

        var model = _mapper.Map<SubmissionStudent>(dto);

        var submissions = await _context.SubmissionStudents
            .Where(x => x.SubmissionsConfigId == dto.SubmissionsConfigId && x.Id != dto.Id  && x.StudentId == dto.StudentId)
            .ToListAsync();
        
        submissions.ForEach(x => x.PreferredPosition = dto.PreferredPosition);
        
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

        var model = await _context.SubmissionStudents.FirstOrDefaultAsync(x => x.Id == id);

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