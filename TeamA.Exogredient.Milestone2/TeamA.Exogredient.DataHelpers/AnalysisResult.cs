using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DataHelpers
{
    public class AnalysisResult
    {
        public ICollection<string> Suggestions { get; }
        public string Category { get; }
        public string Name { get; }

        public AnalysisResult(ICollection<string> suggestions, string category, string name)
        {
            Suggestions = suggestions;
            Category = category;
            Name = name;
        }
    }
}
