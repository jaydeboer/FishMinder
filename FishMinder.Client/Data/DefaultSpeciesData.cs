using FishMinder.Client.Models;

namespace FishMinder.Client.Data;

/// <summary>
/// Default fish species data for Minnesota and North Dakota
/// </summary>
public static class DefaultSpeciesData
{
    /// <summary>
    /// Gets the default species list for Minnesota and North Dakota
    /// </summary>
    public static List<FishSpecies> GetDefaultSpecies()
    {
        var species = new List<FishSpecies>();
        
        // Minnesota Species
        species.AddRange(GetMinnesotaSpecies());
        
        // North Dakota Species
        species.AddRange(GetNorthDakotaSpecies());
        
        return species;
    }

    private static List<FishSpecies> GetMinnesotaSpecies()
    {
        return new List<FishSpecies>
        {
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Walleye",
                ScientificName = "Sander vitreus",
                Region = "MN",
                DailyLimit = 6,
                MinimumSize = 14.0m,
                IsInSeason = true,
                Notes = "State fish of Minnesota"
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Northern Pike",
                ScientificName = "Esox lucius",
                Region = "MN",
                DailyLimit = 3,
                MinimumSize = 24.0m,
                IsInSeason = true
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Largemouth Bass",
                ScientificName = "Micropterus salmoides",
                Region = "MN",
                DailyLimit = 6,
                MinimumSize = 14.0m,
                IsInSeason = true
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Smallmouth Bass",
                ScientificName = "Micropterus dolomieu",
                Region = "MN",
                DailyLimit = 6,
                MinimumSize = 14.0m,
                IsInSeason = true
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Bluegill",
                ScientificName = "Lepomis macrochirus",
                Region = "MN",
                DailyLimit = 20,
                MinimumSize = 0,
                IsInSeason = true
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Black Crappie",
                ScientificName = "Pomoxis nigromaculatus",
                Region = "MN",
                DailyLimit = 10,
                MinimumSize = 10.0m,
                IsInSeason = true
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "White Crappie",
                ScientificName = "Pomoxis annularis",
                Region = "MN",
                DailyLimit = 10,
                MinimumSize = 10.0m,
                IsInSeason = true
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Yellow Perch",
                ScientificName = "Perca flavescens",
                Region = "MN",
                DailyLimit = 20,
                MinimumSize = 0,
                IsInSeason = true
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Muskellunge",
                ScientificName = "Esox masquinongy",
                Region = "MN",
                DailyLimit = 1,
                MinimumSize = 54.0m,
                IsInSeason = true,
                Notes = "Trophy fish - special regulations may apply"
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Lake Trout",
                ScientificName = "Salvelinus namaycush",
                Region = "MN",
                DailyLimit = 3,
                MinimumSize = 15.0m,
                IsInSeason = true
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Brook Trout",
                ScientificName = "Salvelinus fontinalis",
                Region = "MN",
                DailyLimit = 5,
                MinimumSize = 0,
                IsInSeason = true
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Brown Trout",
                ScientificName = "Salmo trutta",
                Region = "MN",
                DailyLimit = 5,
                MinimumSize = 0,
                IsInSeason = true
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Rainbow Trout",
                ScientificName = "Oncorhynchus mykiss",
                Region = "MN",
                DailyLimit = 5,
                MinimumSize = 0,
                IsInSeason = true
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Channel Catfish",
                ScientificName = "Ictalurus punctatus",
                Region = "MN",
                DailyLimit = 5,
                MinimumSize = 0,
                IsInSeason = true
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Flathead Catfish",
                ScientificName = "Pylodictis olivaris",
                Region = "MN",
                DailyLimit = 5,
                MinimumSize = 0,
                IsInSeason = true
            }
        };
    }

    private static List<FishSpecies> GetNorthDakotaSpecies()
    {
        return new List<FishSpecies>
        {
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Walleye",
                ScientificName = "Sander vitreus",
                Region = "ND",
                DailyLimit = 5,
                MinimumSize = 14.0m,
                IsInSeason = true,
                Notes = "State fish of North Dakota"
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Northern Pike",
                ScientificName = "Esox lucius",
                Region = "ND",
                DailyLimit = 3,
                MinimumSize = 0,
                IsInSeason = true
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Largemouth Bass",
                ScientificName = "Micropterus salmoides",
                Region = "ND",
                DailyLimit = 5,
                MinimumSize = 12.0m,
                IsInSeason = true
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Smallmouth Bass",
                ScientificName = "Micropterus dolomieu",
                Region = "ND",
                DailyLimit = 5,
                MinimumSize = 12.0m,
                IsInSeason = true
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "White Bass",
                ScientificName = "Morone chrysops",
                Region = "ND",
                DailyLimit = 10,
                MinimumSize = 0,
                IsInSeason = true
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Bluegill",
                ScientificName = "Lepomis macrochirus",
                Region = "ND",
                DailyLimit = 20,
                MinimumSize = 0,
                IsInSeason = true
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Black Crappie",
                ScientificName = "Pomoxis nigromaculatus",
                Region = "ND",
                DailyLimit = 10,
                MinimumSize = 0,
                IsInSeason = true
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Yellow Perch",
                ScientificName = "Perca flavescens",
                Region = "ND",
                DailyLimit = 20,
                MinimumSize = 0,
                IsInSeason = true
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Sauger",
                ScientificName = "Sander canadensis",
                Region = "ND",
                DailyLimit = 5,
                MinimumSize = 0,
                IsInSeason = true
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Channel Catfish",
                ScientificName = "Ictalurus punctatus",
                Region = "ND",
                DailyLimit = 5,
                MinimumSize = 0,
                IsInSeason = true
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Chinook Salmon",
                ScientificName = "Oncorhynchus tshawytscha",
                Region = "ND",
                DailyLimit = 3,
                MinimumSize = 0,
                IsInSeason = true
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Rainbow Trout",
                ScientificName = "Oncorhynchus mykiss",
                Region = "ND",
                DailyLimit = 5,
                MinimumSize = 0,
                IsInSeason = true
            },
            new FishSpecies
            {
                Id = Guid.NewGuid(),
                Name = "Lake Trout",
                ScientificName = "Salvelinus namaycush",
                Region = "ND",
                DailyLimit = 3,
                MinimumSize = 0,
                IsInSeason = true
            }
        };
    }
}
