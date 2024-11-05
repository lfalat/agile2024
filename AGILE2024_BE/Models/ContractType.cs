﻿using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models
{
    public class ContractType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string? Name { get; set; }
    }
}
