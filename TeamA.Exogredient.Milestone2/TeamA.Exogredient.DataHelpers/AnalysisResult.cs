using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DataHelpers
{
    public class AnalysisResult
    {
        public ICollection<string> Suggestions;
        public string Category;
        public string Name;

        public AnalysisResult(ICollection<string> suggestions, string category, string name)
        {
            Suggestions = suggestions;
            Category = category;
            Name = name;
        }
    }
}
