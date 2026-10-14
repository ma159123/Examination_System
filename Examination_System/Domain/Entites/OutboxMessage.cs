namespace Domain.Entites
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty; // نوع الحدث، مثل: "UserRegistered_GenerateOtp"
        public string Payload { get; set; } = string.Empty; // البيانات بصيغة JSON لتحقيق المرونة
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedOn { get; set; } // متى تم معالجة الرسالة وإرسالها
        public string? Error { get; set; } // لتسجيل أي خطأ في حال فشل الإرسال مؤقتاً
    }
}
