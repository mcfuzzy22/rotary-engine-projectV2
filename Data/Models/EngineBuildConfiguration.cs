// In EngineBuildConfiguration.cs
using rotaryproject.Data.Models; // Your namespace for the Part model
using System.Collections.Generic;
using rotaryproject.Data;
namespace rotaryproject.Data // Or your preferred namespace for ViewModels
{
    public class EngineBuildConfiguration
    {
        // Key: CategoryID, Value: The selected Part object
        public Dictionary<int, Part?> SelectedParts { get; set; }

        // You can add other properties later, e.g.:
        public decimal TotalPrice { get; set; }
        // public List<string> CompatibilityIssues { get; set; } = new List<string>();
        // public EngineOverallStats? CalculatedStats { get; set; }

        public EngineBuildConfiguration()
        {
            SelectedParts = new Dictionary<int, Part?>();
        }

        // Optional: Method to add or update a selected part
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
            // TODO: Trigger re-calculation of stats and compatibility here
        }

        // Optional: Method to get a selected part
        public Part? GetSelectedPartForCategory(int categoryId)
        {
            SelectedParts.TryGetValue(categoryId, out Part? part);
            return part;
        }
    }
}