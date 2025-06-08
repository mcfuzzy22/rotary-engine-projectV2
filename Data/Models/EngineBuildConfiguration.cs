using rotaryproject.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace rotaryproject.Data
{
    public class EngineBuildConfiguration
    {
        // --- UPDATED ---
        // The value is now a List<Part> to hold multiple parts per category.
        public Dictionary<int, List<Part>> SelectedParts { get; private set; } = new Dictionary<int, List<Part>>();

        public decimal TotalPrice { get; set; }
        public List<string> CompatibilityIssues { get; set; } = new List<string>();
        public string? DeterminedEngineFamily { get; set; }
        public int? DeterminedEngineFamilyId { get; set; }

        /// <summary>
        /// Adds a part to the list for a given category.
        /// </summary>
        public void AddPart(int categoryId, Part part)
        {
            if (!SelectedParts.ContainsKey(categoryId))
            {
                SelectedParts[categoryId] = new List<Part>();
            }
            SelectedParts[categoryId].Add(part);
        }

        /// <summary>
        /// Removes a specific instance of a part from a category.
        /// </summary>
        public void RemovePart(int categoryId, int partId)
        {
            if (SelectedParts.ContainsKey(categoryId))
            {
                var partToRemove = SelectedParts[categoryId].FirstOrDefault(p => p.PartId == partId);
                if (partToRemove != null)
                {
                    SelectedParts[categoryId].Remove(partToRemove);
                }
                // If the list for this category is now empty, remove the key.
                if (!SelectedParts[categoryId].Any())
                {
                    SelectedParts.Remove(categoryId);
                }
            }
        }
        
        /// <summary>
        /// Clears all parts for a given category.
        /// </summary>
        public void ClearPartsForCategory(int categoryId)
        {
            SelectedParts.Remove(categoryId);
        }
    }
}
