using CsvHelper.Configuration.Attributes;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SharedKernel.DTOs;

public class QualityRateDTO
{
    [Name("Id")]
    [DisplayName("Id")]
    public string? AgentId { get; set; }
    [Name("Agent")]
    [DisplayName("Agent")]
    public string? AgentName { get; set; }
    [Name("Error category")]
    [DisplayName("Error category")]
    public string? ErrorCategory { get; set; }
    public string TrimmedAgentId => string.IsNullOrEmpty(AgentId) ? string.Empty : AgentId.TrimStart('0');
    public string TrimmedEvaluatorId => string.IsNullOrEmpty(EvaluationTypeId) ? string.Empty : EvaluationTypeId.TrimStart('0');
    public string? AgentIdName
    {
        get
        {
            if (string.IsNullOrEmpty(AgentName))
            {
                return string.Empty;
            }

            var names = AgentName?.Split(' ');

            return names?.Length switch
            {
                > 2 => $"{TrimmedAgentId}-{names[0]} {names[2]}",
               _ => $"{TrimmedAgentId}-{AgentName}"
            };
        }
    }
    public string? EvaluationTypeId { get; set; }
    public string? EvaluationId { get; set; }
    public string? ProjectId { get; set; }
    public string? Process { get; set; }
    public string? ProjectName { get; set; }
    public List<EvaluationScoreDetail> EvaluationScoreDetails { get; set; } = new();
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? ComponentName { get; set; }
    public int ComponentValue { get; set; }
    [Name("Orders audited")]
    [DisplayName("Orders audited")]
    public int OrdersAudited { get; set; }
    public decimal QualityRating { get; set; }
    public decimal Score { get; set; }
    public int Errors { get; set; }

}
