using System.Collections.Generic;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services.Interfaces;
using System;
using Google.Cloud.Vision.V1;
using TeamA.Exogredient.AppConstants;
using System.Threading.Tasks;

namespace TeamA.Exogredient.Services
{
    public class GoogleImageAnalysisService : IImageAnalysisService
    {
        public async Task<AnalysisResult> AnalyzeAsync(Image image, ICollection<string> categories)
        {
            var client = await ImageAnnotatorClient.CreateAsync().ConfigureAwait(false);
            var labels = await client.DetectLabelsAsync(image).ConfigureAwait(false);
            var suggestions = new List<string>();
            var category = Constants.NoValueString;
            var categoryFound = false;
            var bestLabel = Constants.NoValueString;
            var bestLabelFound = false;

            foreach (var label in labels)
            {
                if (!bestLabelFound)
                {
                    var invalid = false;

                    foreach (var word in Constants.InvalidSuggestions)
                    {
                        if (label.Description.ToLower().Equals(word.ToLower()))
                        {
                            invalid = true;
                        }
                    }

                    if (!invalid)
                    {
                        bestLabel = label.Description;
                        bestLabelFound = true;
                    }
                }

                suggestions.Add(label.Description);

                if (!categoryFound)
                {
                    foreach (var cat in categories)
                    {
                        if (!Constants.AllCategoriesToKeywords.ContainsKey(cat))
                        {
                            throw new ArgumentException(Constants.InvalidCategories);
                        }
                        else
                        {
                            foreach (var keyword in Constants.AllCategoriesToKeywords[cat])
                            {
                                if (label.Description.ToLower().Contains(keyword.ToLower()) && !categoryFound)
                                {
                                    category = cat;
                                    categoryFound = true;
                                }
                            }
                        }
                    }
                }
            }

            if (!categoryFound)
            {
                category = Constants.NoCategory;
            }

            // Refactor: apply logic to read labels
            if (category.Equals(Constants.NoCategory) || category.Equals(Constants.ManufacturedCategory))
            {
                bestLabel = "";
            }

            var name = bestLabel;

            AnalysisResult result = new AnalysisResult(suggestions, category, name);

            return result;
        }
    }
}
