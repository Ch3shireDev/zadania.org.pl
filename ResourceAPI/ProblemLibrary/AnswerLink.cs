﻿namespace ProblemLibrary
{
    public class AnswerLink
    {
        public int Id { get; set; }
        public int ProblemId { get; set; }
        public bool IsApproved { get; set; }
        public string Url => $"/api/v1/problems/{ProblemId}/answers/{Id}";
    }
}