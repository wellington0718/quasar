using Domain.Models;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;

namespace SharedKernel.DTOs
{
    public class EvaluationDTO
    {
        public EvaluationDTO()
        {
            DateTimeRange = new DateTimeRange();
        }

        public string? Id { get; set; }
        public string? EvaluationTypeId { get; set; }
        public string? EvaluationTypeName { get; set; }
        public string? State { get; set; } = "Enabled";
        public string? AgentId { get; set; }
        public string TrimmedAgentId => string.IsNullOrEmpty(AgentId) ? string.Empty : AgentId.TrimStart('0');
        public string? AgentName { get; set; }
        public string AgentCommonName
        {
            get
            {
                if (string.IsNullOrEmpty(AgentName))
                {
                    return string.Empty;
                }

                var names = AgentName.Split(' ');
                return names?.Length switch
                {
                    > 2 => $"{TrimmedAgentId}-{names[0]} {names[2]}",
                    _ => $"{TrimmedAgentId}-{AgentName}"
                };
            }
        }
        public string AgentIdCommonName => $"{TrimmedAgentId} - {AgentCommonName}";
        public string? EvaluatorId { get; set; }
        public string TrimmedEvaluatorId => string.IsNullOrEmpty(EvaluatorId) ? string.Empty : EvaluatorId.TrimStart('0');
        public string? EvaluatorName { get; set; }
        public string EvaluatorCommonName
        {
            get
            {
                if (string.IsNullOrEmpty(EvaluatorName))
                {
                    return string.Empty;
                }
                var name = EvaluatorName.Split(' ');
                if (name.Length < 3 && name.Length > 0)
                {
                    return EvaluatorName;
                }
                return $"{EvaluatorName.Split(' ')[0]} {EvaluatorName.Split(' ')[2]}";
            }
        }

        public string EvaluatorIdCommonName => $"{TrimmedEvaluatorId} - {EvaluatorCommonName}";
        public string? ProjectId { get; set; }
        public string? CaseNumber { get; set; }
        public string? AccountNumber { get; set; }
        public string? EditorId { get; set; }
        public string TrimmedEditorId => string.IsNullOrEmpty(EditorId) ? string.Empty : EditorId.TrimStart('0');
        public string EditorCommonName
        {
            get
            {
                if (string.IsNullOrEmpty(EvaluatorName))
                {
                    return string.Empty;
                }
                var name = EvaluatorName.Split(' ');
                if (name.Length < 3 && name.Length > 0)
                {
                    return EvaluatorName;
                }
                return $"{EvaluatorName.Split(' ')[0]} {EvaluatorName.Split(' ')[2]}";
            }
        }

        public string? ReportType { get; set; } = "General";
        public int Score { get; set; }
        public string EditorIdCommonName => $"{TrimmedEditorId} - {EditorCommonName}";
        public string Comments { get; set; } = "Great.";
        public string? EditionNotes { get; set; }
        public bool Edited { get; set; }
        public bool Enabled { get; set; }
        public bool IsUsingScoreValue { get; set; }
        public string EnabledString => Enabled ? "Yes" : "No";
        public string EditedString => Edited ? "Yes" : "No";
        public string ModificationDateString => ModificationDate.ToString("MM/dd/yyyy hh:mm:ss tt");
        public string OriginalDateString => OriginalDate.ToString("MM/dd/yyyy");
        public DateTime OriginalDate { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
        public DateTimeRange DateTimeRange { get; set; }
        public Employee? Agent { get; set; }
        public List<Project>? Projects { get; set; }
        public List<EvaluationScoreDetail> EvaluationScoreDetails { get; set; } = new();
        public IEnumerable<EvaluationType>? EvaluationTypes { get; set; }
        public IEnumerable<Employee>? ProjectEmployees { get; set; }
    }
}
