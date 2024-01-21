using CzechJudiciaryDecisionsTools.Common.Interfaces;
using CzechJudiciaryDecisionsTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CzechJudiciaryDecisionsTools.Courts.SupremeCourt
{
    public class SupremeCourtIndexer : ICourtIndexer
    {
        private string DecisionFolder = Path.Combine(AppContext.BaseDirectory, @"SupremeCourt/Decisions");
        private string IndexFilePath = Path.Combine(AppContext.BaseDirectory, @"SupremeCourt", "SupremeCourtIndex.json");
        public async Task<List<SupremeCourtDecision>> IndexDecisionsAsync()
        {
            List<SupremeCourtDecision> decisions = new();

            foreach (string filePath in Directory.GetFiles(DecisionFolder, "*.txt"))
            {
                var decision = await ExtractDecisionDataAsync(filePath);
                decisions.Add(decision);
            }

            string json = JsonSerializer.Serialize(decisions, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(IndexFilePath, json);
            return decisions;
        }

        private async Task<SupremeCourtDecision> ExtractDecisionDataAsync(string filePath)
        {
            var decisionText = await File.ReadAllTextAsync(filePath);
            string fileName = Path.GetFileName(filePath);

            var decision = new SupremeCourtDecision { TextFileName = fileName };

            decision.Court = ExtractValueFromText(decisionText, "Soud:");
            decision.CaseNumber = ExtractValueFromText(decisionText, "Spisová značka:");
            decision.DecisionDate = DateTime.Parse(ExtractValueFromText(decisionText, "Datum rozhodnutí:"));
            decision.DecisionType = ExtractValueFromText(decisionText, "Typ rozhodnutí:");
            decision.Keywords = ExtractValueFromText(decisionText, "Heslo:");
            decision.RelevantLaws = ExtractValueFromText(decisionText, "Dotčené předpisy:");
            decision.DecisionCategory = ExtractValueFromText(decisionText, "Kategorie rozhodnutí:");

            return decision;
        }

        private string ExtractValueFromText(string text, string label)
        {
            var start = text.IndexOf(label) + label.Length;
            var end = text.IndexOf('\n', start);
            if (start < label.Length || end == -1) 
                return string.Empty;
            return text[start..end].Trim();
        }
    }
}
