using FishMinder.Client.Models;

namespace FishMinder.Client.Services;

/// <summary>
/// Service for managing species counts and limits
/// </summary>
public class SpeciesCountService : ISpeciesCountService
{
    private readonly IFishingTripService _tripService;
    private readonly IFishSpeciesService _speciesService;

    public SpeciesCountService(
        IFishingTripService tripService,
        IFishSpeciesService speciesService)
    {
        _tripService = tripService;
        _speciesService = speciesService;
    }

    public async Task<IEnumerable<SpeciesCount>> GetCurrentTripSpeciesCountsAsync()
    {
        var currentTrip = await _tripService.GetCurrentTripAsync();
        if (currentTrip == null)
            return new List<SpeciesCount>();

        var speciesCounts = new List<SpeciesCount>();
        var allSpecies = await _speciesService.GetAllAsync();
        var speciesDict = allSpecies.ToDictionary(s => s.Id, s => s);

        // Group catches by species
        var catchesBySpecies = currentTrip.Catches.GroupBy(c => c.SpeciesId);

        foreach (var speciesGroup in catchesBySpecies)
        {
            var speciesId = speciesGroup.Key;
            var catches = speciesGroup.ToList();
            
            if (!speciesDict.TryGetValue(speciesId, out var species))
                continue;

            var speciesCount = new SpeciesCount
            {
                SpeciesId = speciesId,
                SpeciesName = species.Name,
                TotalCaught = catches.Count,
                TotalKept = catches.Count(c => c.CountsTowardLimit),
                TotalReleased = catches.Count(c => c.Disposition == FishDisposition.Released),
                InLivewell = catches.Count(c => c.Disposition == FishDisposition.InLivewell),
                Died = catches.Count(c => c.Disposition == FishDisposition.Died),
                FirstCatch = catches.Min(c => c.CaughtAt),
                LastCatch = catches.Max(c => c.CaughtAt),
                DailyLimit = species.DailyLimit,
                PossessionLimit = species.DailyLimit // Using daily limit as possession limit for now
            };

            // Calculate size statistics
            var sizeCatches = catches.Where(c => c.Length.HasValue).ToList();
            if (sizeCatches.Any())
            {
                speciesCount.LargestFish = sizeCatches.Max(c => c.Length);
                speciesCount.SmallestFish = sizeCatches.Min(c => c.Length);
                speciesCount.AverageSize = sizeCatches.Average(c => c.Length!.Value);
            }

            // Calculate total weight
            var weightCatches = catches.Where(c => c.Weight.HasValue);
            if (weightCatches.Any())
            {
                speciesCount.TotalWeight = weightCatches.Sum(c => c.Weight);
            }

            speciesCounts.Add(speciesCount);
        }

        return speciesCounts.OrderByDescending(sc => sc.TotalCaught);
    }

    public async Task<SpeciesCount?> GetSpeciesCountAsync(Guid speciesId)
    {
        var allCounts = await GetCurrentTripSpeciesCountsAsync();
        return allCounts.FirstOrDefault(sc => sc.SpeciesId == speciesId);
    }

    public async Task<SpeciesCount?> GetAnglerSpeciesCountAsync(Guid anglerId, Guid speciesId)
    {
        var currentTrip = await _tripService.GetCurrentTripAsync();
        if (currentTrip == null)
            return null;

        var species = await _speciesService.GetByIdAsync(speciesId);
        var angler = currentTrip.Anglers.FirstOrDefault(a => a.Id == anglerId);
        
        if (species == null || angler == null)
            return null;

        var catches = currentTrip.Catches
            .Where(c => c.AnglerId == anglerId && c.SpeciesId == speciesId)
            .ToList();

        if (!catches.Any())
            return null;

        var speciesCount = new SpeciesCount
        {
            SpeciesId = speciesId,
            SpeciesName = species.Name,
            AnglerId = anglerId,
            AnglerName = angler.Name,
            TotalCaught = catches.Count,
            TotalKept = catches.Count(c => c.CountsTowardLimit),
            TotalReleased = catches.Count(c => c.Disposition == FishDisposition.Released),
            InLivewell = catches.Count(c => c.Disposition == FishDisposition.InLivewell),
            Died = catches.Count(c => c.Disposition == FishDisposition.Died),
            FirstCatch = catches.Min(c => c.CaughtAt),
            LastCatch = catches.Max(c => c.CaughtAt),
            DailyLimit = species.DailyLimit,
            PossessionLimit = species.DailyLimit
        };

        // Calculate size statistics
        var sizeCatches = catches.Where(c => c.Length.HasValue).ToList();
        if (sizeCatches.Any())
        {
            speciesCount.LargestFish = sizeCatches.Max(c => c.Length);
            speciesCount.SmallestFish = sizeCatches.Min(c => c.Length);
            speciesCount.AverageSize = sizeCatches.Average(c => c.Length!.Value);
        }

        // Calculate total weight
        var weightCatches = catches.Where(c => c.Weight.HasValue);
        if (weightCatches.Any())
        {
            speciesCount.TotalWeight = weightCatches.Sum(c => c.Weight);
        }

        return speciesCount;
    }

    public async Task<IEnumerable<SpeciesCount>> GetAnglerSpeciesCountsAsync(Guid anglerId)
    {
        var currentTrip = await _tripService.GetCurrentTripAsync();
        if (currentTrip == null)
            return new List<SpeciesCount>();

        var angler = currentTrip.Anglers.FirstOrDefault(a => a.Id == anglerId);
        if (angler == null)
            return new List<SpeciesCount>();

        var anglerCatches = currentTrip.Catches.Where(c => c.AnglerId == anglerId);
        var speciesIds = anglerCatches.Select(c => c.SpeciesId).Distinct();

        var speciesCounts = new List<SpeciesCount>();
        
        foreach (var speciesId in speciesIds)
        {
            var count = await GetAnglerSpeciesCountAsync(anglerId, speciesId);
            if (count != null)
            {
                speciesCounts.Add(count);
            }
        }

        return speciesCounts.OrderByDescending(sc => sc.TotalCaught);
    }

    public async Task<LimitCheckResult> CheckDailyLimitAsync(Guid anglerId, Guid speciesId, int additionalCount = 1)
    {
        var species = await _speciesService.GetByIdAsync(speciesId);
        if (species == null || species.DailyLimit <= 0)
        {
            return new LimitCheckResult
            {
                IsWithinLimit = true,
                IsApproachingLimit = false,
                CurrentCount = 0,
                Limit = 0,
                RemainingCount = int.MaxValue,
                Message = "No daily limit set for this species",
                LimitType = LimitType.Daily
            };
        }

        var currentCount = await GetAnglerSpeciesCountAsync(anglerId, speciesId);
        var keptCount = currentCount?.TotalKept ?? 0;
        var newTotal = keptCount + additionalCount;

        var result = new LimitCheckResult
        {
            CurrentCount = keptCount,
            Limit = species.DailyLimit,
            LimitType = LimitType.Daily
        };

        if (newTotal <= species.DailyLimit)
        {
            result.IsWithinLimit = true;
            result.RemainingCount = species.DailyLimit - newTotal;
            result.IsApproachingLimit = newTotal >= (species.DailyLimit * 0.8);
            
            if (result.IsApproachingLimit)
            {
                result.Message = $"Approaching daily limit ({newTotal}/{species.DailyLimit})";
            }
            else
            {
                result.Message = $"Within daily limit ({newTotal}/{species.DailyLimit})";
            }
        }
        else
        {
            result.IsWithinLimit = false;
            result.IsApproachingLimit = false;
            result.RemainingCount = 0;
            result.Message = $"Would exceed daily limit ({newTotal}/{species.DailyLimit})";
        }

        return result;
    }

    public async Task<LimitCheckResult> CheckPossessionLimitAsync(Guid anglerId, Guid speciesId, int additionalCount = 1)
    {
        // For now, using daily limit as possession limit
        // This can be enhanced later with separate possession limits
        return await CheckDailyLimitAsync(anglerId, speciesId, additionalCount);
    }

    public async Task<IEnumerable<SpeciesLimitWarning>> GetLimitWarningsAsync(Guid? anglerId = null)
    {
        var warnings = new List<SpeciesLimitWarning>();
        var speciesCounts = anglerId.HasValue 
            ? await GetAnglerSpeciesCountsAsync(anglerId.Value)
            : await GetCurrentTripSpeciesCountsAsync();

        foreach (var count in speciesCounts)
        {
            if (count.IsApproachingDailyLimit && !count.IsOverDailyLimit)
            {
                warnings.Add(new SpeciesLimitWarning
                {
                    SpeciesId = count.SpeciesId,
                    SpeciesName = count.SpeciesName,
                    AnglerId = count.AnglerId,
                    AnglerName = count.AnglerName,
                    CurrentCount = count.TotalKept,
                    Limit = count.DailyLimit,
                    LimitType = LimitType.Daily,
                    WarningLevel = WarningLevel.Approaching,
                    Message = $"Approaching daily limit: {count.TotalKept}/{count.DailyLimit}"
                });
            }
        }

        return warnings;
    }

    public async Task<IEnumerable<SpeciesLimitWarning>> GetLimitExceedingsAsync(Guid? anglerId = null)
    {
        var warnings = new List<SpeciesLimitWarning>();
        var speciesCounts = anglerId.HasValue 
            ? await GetAnglerSpeciesCountsAsync(anglerId.Value)
            : await GetCurrentTripSpeciesCountsAsync();

        foreach (var count in speciesCounts)
        {
            if (count.IsOverDailyLimit)
            {
                warnings.Add(new SpeciesLimitWarning
                {
                    SpeciesId = count.SpeciesId,
                    SpeciesName = count.SpeciesName,
                    AnglerId = count.AnglerId,
                    AnglerName = count.AnglerName,
                    CurrentCount = count.TotalKept,
                    Limit = count.DailyLimit,
                    LimitType = LimitType.Daily,
                    WarningLevel = WarningLevel.Exceeded,
                    Message = $"Daily limit exceeded: {count.TotalKept}/{count.DailyLimit}"
                });
            }
        }

        return warnings;
    }

    public async Task<TripSpeciesStatistics> GetTripSpeciesStatisticsAsync()
    {
        var currentTrip = await _tripService.GetCurrentTripAsync();
        if (currentTrip == null)
        {
            return new TripSpeciesStatistics();
        }

        var speciesCounts = await GetCurrentTripSpeciesCountsAsync();
        var anglerStats = new List<AnglerStatistics>();

        foreach (var angler in currentTrip.Anglers.Where(a => a.IsActive))
        {
            var anglerSpeciesCounts = await GetAnglerSpeciesCountsAsync(angler.Id);
            var anglerCatches = currentTrip.Catches.Where(c => c.AnglerId == angler.Id).ToList();

            var stat = new AnglerStatistics
            {
                AnglerId = angler.Id,
                AnglerName = angler.Name,
                TotalCatches = anglerCatches.Count,
                TotalKept = anglerCatches.Count(c => c.CountsTowardLimit),
                TotalReleased = anglerCatches.Count(c => c.Disposition == FishDisposition.Released),
                UniqueSpecies = anglerCatches.Select(c => c.SpeciesId).Distinct().Count(),
                SpeciesCounts = anglerSpeciesCounts
            };

            // Find largest fish
            var sizeCatches = anglerCatches.Where(c => c.Length.HasValue).ToList();
            if (sizeCatches.Any())
            {
                var largestCatch = sizeCatches.OrderByDescending(c => c.Length).First();
                stat.LargestFish = largestCatch.Length;
                
                var species = await _speciesService.GetByIdAsync(largestCatch.SpeciesId);
                stat.LargestFishSpecies = species?.Name;
            }

            anglerStats.Add(stat);
        }

        var allCatches = currentTrip.Catches.ToList();
        
        return new TripSpeciesStatistics
        {
            TotalCatches = allCatches.Count,
            TotalKept = allCatches.Count(c => c.CountsTowardLimit),
            TotalReleased = allCatches.Count(c => c.Disposition == FishDisposition.Released),
            UniqueSpecies = allCatches.Select(c => c.SpeciesId).Distinct().Count(),
            ActiveAnglers = currentTrip.Anglers.Count(a => a.IsActive),
            TripDuration = currentTrip.Duration,
            SpeciesCounts = speciesCounts,
            AnglerStats = anglerStats,
            FirstCatch = allCatches.Any() ? allCatches.Min(c => c.CaughtAt) : null,
            LastCatch = allCatches.Any() ? allCatches.Max(c => c.CaughtAt) : null,
            MostCaughtSpecies = speciesCounts.OrderByDescending(sc => sc.TotalCaught).FirstOrDefault()?.SpeciesName,
            TopAngler = anglerStats.OrderByDescending(a => a.TotalCatches).FirstOrDefault()?.AnglerName
        };
    }

    public async Task<IEnumerable<SpeciesCount>> GetHistoricalSpeciesCountsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var trips = await _tripService.GetTripsOrderedByDateAsync();
        
        if (startDate.HasValue)
            trips = trips.Where(t => t.StartTime >= startDate.Value);
        
        if (endDate.HasValue)
            trips = trips.Where(t => t.StartTime <= endDate.Value);

        var allCatches = trips.SelectMany(t => t.Catches).ToList();
        var speciesCounts = new List<SpeciesCount>();
        var allSpecies = await _speciesService.GetAllAsync();
        var speciesDict = allSpecies.ToDictionary(s => s.Id, s => s);

        var catchesBySpecies = allCatches.GroupBy(c => c.SpeciesId);

        foreach (var speciesGroup in catchesBySpecies)
        {
            var speciesId = speciesGroup.Key;
            var catches = speciesGroup.ToList();
            
            if (!speciesDict.TryGetValue(speciesId, out var species))
                continue;

            var speciesCount = new SpeciesCount
            {
                SpeciesId = speciesId,
                SpeciesName = species.Name,
                TotalCaught = catches.Count,
                TotalKept = catches.Count(c => c.CountsTowardLimit),
                TotalReleased = catches.Count(c => c.Disposition == FishDisposition.Released),
                InLivewell = catches.Count(c => c.Disposition == FishDisposition.InLivewell),
                Died = catches.Count(c => c.Disposition == FishDisposition.Died),
                FirstCatch = catches.Min(c => c.CaughtAt),
                LastCatch = catches.Max(c => c.CaughtAt),
                DailyLimit = species.DailyLimit,
                PossessionLimit = species.DailyLimit
            };

            // Calculate size statistics
            var sizeCatches = catches.Where(c => c.Length.HasValue).ToList();
            if (sizeCatches.Any())
            {
                speciesCount.LargestFish = sizeCatches.Max(c => c.Length);
                speciesCount.SmallestFish = sizeCatches.Min(c => c.Length);
                speciesCount.AverageSize = sizeCatches.Average(c => c.Length!.Value);
            }

            // Calculate total weight
            var weightCatches = catches.Where(c => c.Weight.HasValue);
            if (weightCatches.Any())
            {
                speciesCount.TotalWeight = weightCatches.Sum(c => c.Weight);
            }

            speciesCounts.Add(speciesCount);
        }

        return speciesCounts.OrderByDescending(sc => sc.TotalCaught);
    }

    public async Task RefreshCountsAsync()
    {
        // This method can be used to clear any cached data if we implement caching later
        // For now, all data is calculated on-demand, so no action needed
        await Task.CompletedTask;
    }
}
