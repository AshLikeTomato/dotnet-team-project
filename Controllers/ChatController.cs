using DotnetProject2025.Models;
using DotnetProject2025.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] ChatMessage message)
    {
        message.Timestamp = DateTime.Now;
        await _chatService.SaveMessageAsync(message);
        return Ok();
    }

    [HttpGet("messages/{userId}")]
    public async Task<IActionResult> GetMessages(string userId)
    {
        var messages = await _chatService.GetMessagesAsync(userId);
        return Ok(messages);
    }
}
