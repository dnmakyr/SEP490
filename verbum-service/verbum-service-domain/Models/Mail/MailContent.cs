﻿namespace verbum_service_domain.Models.Mail
{
    public class MailContent
    {
        public List<string> To { get; set; }              // Địa chỉ gửi đến
        public string Subject { get; set; }         // Chủ đề (tiêu đề email)
        public string Body { get; set; }            // Nội dung (hỗ trợ HTML) của email

    }
}
