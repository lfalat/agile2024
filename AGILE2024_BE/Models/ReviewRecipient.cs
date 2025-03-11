﻿using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models
{
    public class ReviewRecipient
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        [ForeignKey(nameof(GoalAssignment) + "Id")]
        public required GoalAssignment goalAssignment { get; set; }
        [ForeignKey(nameof(Review) + "Id")]
        public required Review review { get; set; }
        public string? superiorDescription { get; set; }
        public string? employeeDescription { get; set; }
        public required bool isSavedSuperiorDesc { get; set; } = false;
        public required bool isSavedEmployeeDesc { get; set; } = false;
        public required bool isSentSuperiorDesc { get; set; } = false;
        public required bool isSentEmployeeDesc { get; set; } = false;
    }
}
