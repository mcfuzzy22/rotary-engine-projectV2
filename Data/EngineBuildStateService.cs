using Microsoft.EntityFrameworkCore;
using rotaryproject.Data;
using rotaryproject.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rotaryproject.Services
{
    public class EngineBuildStateService
    {
        private readonly RotaryEngineDbContext _context;

        public EngineBuildConfiguration CurrentBuild { get; private set; } = new EngineBuildConfiguration();
        public HashSet<int> ExpandedCategoryIds { get; set; } = new HashSet<int>();
        public event Action? OnChange;

        public EngineBuildStateService(RotaryEngineDbContext context)
        {
            _context = context;
            Console.WriteLine("DEBUG: EngineBuildStateService CREATED.");
        }

        public async Task SelectPart(int categoryId, Part? part)
        {
            Console.WriteLine($"DEBUG: SelectPart called. CategoryId: {categoryId}, Part: {part?.Name ?? "NULL"}");
            CurrentBuild.SelectPart(categoryId, part);
            await RecalculateAndValidate();
            NotifyStateChanged();
        }
        
        public async Task ResetBuild()
        {
            Console.WriteLine("DEBUG: ResetBuild called.");
            CurrentBuild = new EngineBuildConfiguration();
            ExpandedCategoryIds.Clear();
            await RecalculateAndValidate();
            NotifyStateChanged();
        }

        private async Task RecalculateAndValidate()
        {
            Console.WriteLine("\n----- DEBUG: Starting RecalculateAndValidate -----");

            // 1. Reset state
            CurrentBuild.CompatibilityIssues.Clear();
            CurrentBuild.DeterminedEngineFamily = null;
            CurrentBuild.DeterminedEngineFamilyId = null;
            CurrentBuild.TotalPrice = CurrentBuild.SelectedParts.Values
                                     .Where(p => p != null && p.BasePrice.HasValue)
                                     .Sum(p => p.BasePrice!.Value);
            Console.WriteLine($"DEBUG: TotalPrice recalculated to: {CurrentBuild.TotalPrice:C}");

            var selectedPartIds = CurrentBuild.SelectedParts.Values
                .Where(p => p != null)
                .Select(p => p.PartId)
                .ToList();

            if (!selectedPartIds.Any())
            {
                Console.WriteLine("DEBUG: No parts selected. Exiting validation.");
                Console.WriteLine("----- DEBUG: Finished RecalculateAndValidate -----\n");
                return;
            }
            Console.WriteLine($"DEBUG: Found {selectedPartIds.Count} selected part(s) with IDs: {string.Join(", ", selectedPartIds)}");

            // 2. Re-fetch all parts with their complete fitment data
            var relevantParts = await _context.Parts
                .Include(p => p.Fitments)
                .ThenInclude(f => f.EngineFamily)
                .Where(p => selectedPartIds.Contains(p.PartId) && p.Fitments.Any())
                .ToListAsync();

            if (!relevantParts.Any())
            {
                Console.WriteLine("DEBUG: No selected parts have fitment data. Exiting compatibility check.");
                Console.WriteLine("----- DEBUG: Finished RecalculateAndValidate -----\n");
                return;
            }
            Console.WriteLine($"DEBUG: Found {relevantParts.Count} relevant part(s) with fitment data: {string.Join(", ", relevantParts.Select(p => p.Name))}");

            // 3. Determine the set of possible engine families
            var firstPart = relevantParts.First();
            HashSet<int> possibleFamilyIds = new HashSet<int>(firstPart.Fitments.Select(f => f.EngineFamilyId));
            Console.WriteLine($"DEBUG: Initial possible family IDs from '{firstPart.Name}': [{string.Join(", ", possibleFamilyIds)}]");

            // 4. Find the intersection with all other parts
            foreach (var part in relevantParts.Skip(1))
            {
                var currentPartFamilyIds = part.Fitments.Select(f => f.EngineFamilyId).ToList();
                Console.WriteLine($"DEBUG: Intersecting with '{part.Name}' which has families: [{string.Join(", ", currentPartFamilyIds)}]");
                possibleFamilyIds.IntersectWith(currentPartFamilyIds);
                Console.WriteLine($"DEBUG: Possible family IDs are now: [{string.Join(", ", possibleFamilyIds)}]");
            }

            // 5. Determine final compatibility
            if (possibleFamilyIds.Count == 0)
            {
                Console.WriteLine("DEBUG: FINAL VERDICT - Incompatible. No common families found.");
                CurrentBuild.CompatibilityIssues.Add("Incompatible parts selected. The chosen core components do not share a common engine family.");
            }
            else if (possibleFamilyIds.Count == 1)
            {
                var familyId = possibleFamilyIds.First();
                var family = relevantParts.SelectMany(p => p.Fitments)
                                          .First(f => f.EngineFamilyId == familyId)
                                          .EngineFamily;
                if(family != null)
                {
                    CurrentBuild.DeterminedEngineFamily = family.FamilyCode;
                    CurrentBuild.DeterminedEngineFamilyId = family.EngineFamilyId;
                    Console.WriteLine($"DEBUG: FINAL VERDICT - Compatible. Determined Engine Family: {CurrentBuild.DeterminedEngineFamily}");
                }
            }
            else
            {
                Console.WriteLine($"DEBUG: FINAL VERDICT - Compatible (so far). Multiple possibilities remain: [{string.Join(", ", possibleFamilyIds)}]");
            }

            Console.WriteLine("----- DEBUG: Finished RecalculateAndValidate -----\n");
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
