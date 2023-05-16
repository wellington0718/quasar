using Domain.Models;
using System;

namespace SharedKernel.DTOs
{
    public class CreateEvaluationTypeDTO
    {
        public CreateEvaluationTypeDTO()
        {
            EvaluationTypeId = Guid.NewGuid().ToString();
        }
        public string? EvaluationTypeId { get; set; }
        public string? Name { get; set; }
        public bool IsInclusive { get; set; }
        public bool Enabled { get; set; }
        public bool IsUsingScoreValue { get; set; }
        public EvaluationType? EvaluationType { get; set; }
        public Project? Project { get; set; }
        public EvaluationComponent EvaluationComponent { get; set; } = new();
    }
}
