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
        }

        // --- UPDATED METHODS ---

        public async Task AddPart(int categoryId, Part part)
        {
            CurrentBuild.AddPart(categoryId, part);
            await RecalculateAndValidate();
            NotifyStateChanged();
        }

        public async Task RemovePart(int categoryId, int partId)
        {
            CurrentBuild.RemovePart(categoryId, partId);
            await RecalculateAndValidate();
            NotifyStateChanged();
        }
        
        public async Task ClearPartsForCategory(int categoryId)
        {
            CurrentBuild.ClearPartsForCategory(categoryId);
            await RecalculateAndValidate();
            NotifyStateChanged();
        }

        public async Task ResetBuild()
        {
            CurrentBuild = new EngineBuildConfiguration();
            ExpandedCategoryIds.Clear();
            await RecalculateAndValidate();
            NotifyStateChanged();
        }

        private async Task RecalculateAndValidate()
        {
            CurrentBuild.CompatibilityIssues.Clear();
            CurrentBuild.DeterminedEngineFamily = null;
            CurrentBuild.DeterminedEngineFamilyId = null;
            
            // --- UPDATED LOGIC ---
            var allSelectedParts = CurrentBuild.SelectedParts.Values.SelectMany(pList => pList).ToList();

            CurrentBuild.TotalPrice = allSelectedParts.Where(p => p.BasePrice.HasValue).Sum(p => p.BasePrice!.Value);
            
            var selectedPartIds = allSelectedParts.Select(p => p.PartId).ToList();

            if (!selectedPartIds.Any()) { NotifyStateChanged(); return; }

            var relevantParts = await _context.Parts
                .Include(p => p.Fitments).ThenInclude(f => f.EngineFamily)
                .Where(p => selectedPartIds.Contains(p.PartId) && p.Fitments.Any())
                .ToListAsync();

            if (!relevantParts.Any()) { NotifyStateChanged(); return; }

            HashSet<int> possibleFamilyIds = new HashSet<int>(relevantParts.First().Fitments.Select(f => f.EngineFamilyId));

            foreach (var part in relevantParts.Skip(1))
            {
                possibleFamilyIds.IntersectWith(part.Fitments.Select(f => f.EngineFamilyId));
            }

            if (possibleFamilyIds.Count == 0)
            {
                CurrentBuild.CompatibilityIssues.Add("Incompatible parts selected.");
            }
            else if (possibleFamilyIds.Count == 1)
            {
                var familyId = possibleFamilyIds.First();
                var family = relevantParts.SelectMany(p => p.Fitments).First(f => f.EngineFamilyId == familyId).EngineFamily;
                if (family != null)
                {
                    CurrentBuild.DeterminedEngineFamily = family.FamilyCode;
                    CurrentBuild.DeterminedEngineFamilyId = family.EngineFamilyId;
                }
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
