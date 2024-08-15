using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetProject2025.Models
{
    public class ChatMessage
    {
        public string Id { get; set; }
        public string SenderId { get; set; } // ID của người gửi (khách hàng hoặc admin)
        public string ReceiverId { get; set; } // ID của người nhận (admin hoặc khách hàng)
        public string Content { get; set; } // Nội dung tin nhắn
        public DateTime Timestamp { get; set; }// Thời gian gửi tin nhắn
    }
}