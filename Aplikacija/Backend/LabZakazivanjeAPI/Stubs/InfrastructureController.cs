using Microsoft.AspNetCore.Mvc;

namespace LabZakazivanjeAPI.Stubs;

[ApiController]
[Route("api/infrastructure")]
public class InfrastructureController : ControllerBase
{
    [HttpGet("CloneVM")]
    public async Task<ActionResult<string>> CloneVM([FromQuery] string template)
    {
        var random = Guid.NewGuid().ToString("N").Substring(8);
        await Task.Delay(5);
        return Ok($"{template}-{random}");
    }

    [HttpPost("PrepareVM")]
    public async Task<ActionResult<bool>> PrepareVM([FromQuery] string vlrid, [FromQuery] int roomId, [FromQuery] int seatId)
    {
        Random rnd = new Random();
        int waitTime = rnd.Next() % 200;

        await Task.Delay(waitTime);
        return Ok(true);
    }

    [HttpPost("ReleaseVM")]
    public async Task<ActionResult<bool>> ReleaseVM([FromQuery] string vlrid, [FromQuery] int roomId, [FromQuery] int seatId)
    {
        Random rnd = new Random();
        int waitTime = rnd.Next() % 200;

        await Task.Delay(waitTime);
        return Ok(true);
    }

    [HttpPost("SetVMIp")]
    public async Task<ActionResult<bool>> SetVMIp([FromQuery] string vlrid, [FromQuery] int roomId, [FromQuery] int seatId, [FromQuery] string ip)
    {
        Random rnd = new Random();
        int waitTime = rnd.Next() % 200;

        await Task.Delay(waitTime);
        return Ok(true);
    }}