// In EngineBuildStateService.cs
using rotaryproject.Data.Models; // Your namespace for Part
using System.Collections.Generic;
using rotaryproject.Data;
namespace rotaryproject.Services // Or rotaryproject.Data or your preferred namespace
{
    public class EngineBuildStateService
    {
        public EngineBuildConfiguration CurrentBuild { get; private set; }
        public HashSet<int> ExpandedCategoryIds { get; set; } = new HashSet<int>();
        public event Action? OnChange;
        public EngineBuildStateService()
        {
            CurrentBuild = new EngineBuildConfiguration();
            Console.WriteLine("EngineBuildStateService: New instance created, CurrentBuild initialized.");
        }

        // Optional: Add methods here to modify CurrentBuild if needed,
        // or let components modify it directly if they have a reference.
        // For example, a method to clear the build:
        public void ClearBuild()
        {
            CurrentBuild = new EngineBuildConfiguration(); // Re-initialize
            Console.WriteLine("EngineBuildStateService: Build cleared.");
            // You might need an event here if other components need to know it changed.
        }
        public void SelectPart(int categoryId, Part? part)
        {
            CurrentBuild.SelectPart(categoryId, part);
            NotifyStateChanged();
        }
        
        public void ResetBuild()
        {
            CurrentBuild = new EngineBuildConfiguration();
            // Also reset the expansion state when starting a new build
            ExpandedCategoryIds.Clear();
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}