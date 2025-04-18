﻿using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models
{
    public class FeedbackRequestStatus
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        public required string description { get; set; }
    }
}
