using rotaryproject.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace rotaryproject.Data
{
    /// <summary>
    /// A simple data container for the current build state.
    /// The business logic has been moved to EngineBuildStateService.
    /// </summary>
    public class EngineBuildConfiguration
    {
        // Key: CategoryID, Value: The selected Part object
        public Dictionary<int, Part?> SelectedParts { get; set; } = new Dictionary<int, Part?>();

        // These properties are now set by the service, not calculated here.
        public decimal TotalPrice { get; set; }
        public List<string> CompatibilityIssues { get; set; } = new List<string>();
        public string? DeterminedEngineFamily { get; set; }
        public int? DeterminedEngineFamilyId { get; set; } // Store the ID for easier lookups
        public void SelectPart(int categoryId, Part? part)
        {
            if (part == null)
            {
                SelectedParts.Remove(categoryId);
            }
            else
            {
                SelectedParts[categoryId] = part;
            }
        }

        public Part? GetSelectedPartForCategory(int categoryId)
        {
            SelectedParts.TryGetValue(categoryId, out Part? part);
            return part;
        }
    }
}
