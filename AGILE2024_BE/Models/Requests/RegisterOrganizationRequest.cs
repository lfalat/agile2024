﻿namespace AGILE2024_BE.Models.Requests
{
    public class RegisterOrganizationRequest
    {
        public required string Name {get; set;}
        public required string Code {get; set;}
        public required string Location {get; set;}
    }
}