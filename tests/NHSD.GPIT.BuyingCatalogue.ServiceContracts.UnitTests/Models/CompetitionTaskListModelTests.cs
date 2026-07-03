using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Models;

public static class CompetitionTaskListModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Competition competition)
    {
        var model = new CompetitionTaskListModel(competition);

        model.Id.Should().Be(competition.Id);
        model.Name.Should().Be(competition.Name);
        model.Description.Should().Be(competition.Description);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_NoRecipients_SetsStatuses(
        Competition competition)
    {
        competition.ContractLength = null;
        var model = new CompetitionTaskListModel(competition);

        model.SolutionSelection.Should().Be(TaskProgress.Completed);
        model.ServiceRecipients.Should().Be(TaskProgress.NotStarted);
        model.ContractLength.Should().Be(TaskProgress.CannotStart);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_WithRecipients_SetsStatuses(
        Competition competition,
        List<CompetitionSublocation> competitionSublocations)
    {
        competition.ContractLength = null;
        competition.CompetitionSublocations = competitionSublocations;

        var model = new CompetitionTaskListModel(competition);

        model.ServiceRecipients.Should().Be(TaskProgress.Completed);
        model.ContractLength.Should().Be(TaskProgress.NotStarted);
        model.AwardCriteria.Should().Be(TaskProgress.CannotStart);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_WithContractLength_SetsStatuses(
        Competition competition,
        List<CompetitionSublocation> competitionSublocations)
    {
        competition.ContractLength = 5;
        competition.CompetitionSublocations = competitionSublocations;
        competition.IncludesNonPrice = null;

        var model = new CompetitionTaskListModel(competition);

        model.ServiceRecipients.Should().Be(TaskProgress.Completed);
        model.ContractLength.Should().Be(TaskProgress.Completed);
        model.AwardCriteria.Should().Be(TaskProgress.NotStarted);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_WithoutContractLength_CalculatePriceCannotStart(
        Competition competition,
        List<CompetitionSublocation> competitionSublocations)
    {
        competition.CompetitionSublocations = competitionSublocations;
        competition.ContractLength = null;
        competition.IncludesNonPrice = null;

        var model = new CompetitionTaskListModel(competition);

        model.ServiceRecipients.Should().Be(TaskProgress.Completed);
        model.ContractLength.Should().NotBe(TaskProgress.Completed);
        model.CalculatePrice.Should().Be(TaskProgress.CannotStart);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_WithoutCompetitionSublocations_CalculatePriceCannotStart(
        Competition competition)
    {
        competition.ContractLength = 5;
        competition.IncludesNonPrice = null;

        var model = new CompetitionTaskListModel(competition);

        model.ServiceRecipients.Should().NotBe(TaskProgress.Completed);
        model.ContractLength.Should().NotBe(TaskProgress.Completed);
        model.CalculatePrice.Should().Be(TaskProgress.CannotStart);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_WithoutNonPriceElementAwardCriteria_SetsStatuses(
        Competition competition,
        List<CompetitionSublocation> competitionSublocations)
    {
        competition.ContractLength = 5;
        competition.CompetitionSublocations = competitionSublocations;
        competition.IncludesNonPrice = false;

        var model = new CompetitionTaskListModel(competition);

        model.ContractLength.Should().Be(TaskProgress.Completed);
        model.AwardCriteria.Should().Be(TaskProgress.Completed);
        model.AwardCriteriaWeightings.Should().Be(TaskProgress.NotApplicable);
        model.NonPriceElements.Should().Be(TaskProgress.NotApplicable);
        model.NonPriceWeightings.Should().Be(TaskProgress.NotApplicable);
        model.ReviewCompetitionCriteria.Should().Be(TaskProgress.NotApplicable);
        model.CompareAndScoreSolutions.Should().Be(TaskProgress.NotApplicable);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_WithNonPriceElementAwardCriteria_SetsStatuses(
        Competition competition,
        List<CompetitionSublocation> competitionSublocations)
    {
        competition.ContractLength = 5;
        competition.CompetitionSublocations = competitionSublocations;
        competition.IncludesNonPrice = true;

        var model = new CompetitionTaskListModel(competition);

        model.ContractLength.Should().Be(TaskProgress.Completed);
        model.AwardCriteria.Should().Be(TaskProgress.Completed);
        model.AwardCriteriaWeightings.Should().Be(TaskProgress.NotStarted);
        model.NonPriceElements.Should().Be(TaskProgress.CannotStart);
        model.NonPriceWeightings.Should().Be(TaskProgress.CannotStart);
        model.ReviewCompetitionCriteria.Should().Be(TaskProgress.CannotStart);
        model.CompareAndScoreSolutions.Should().Be(TaskProgress.CannotStart);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_WithAwardCriteriaWeightings_SetsStatuses(
        Competition competition,
        List<CompetitionSublocation> competitionSublocations)
    {
        competition.ContractLength = 5;
        competition.CompetitionSublocations = competitionSublocations;
        competition.IncludesNonPrice = true;
        competition.Weightings = new() { Price = 50, NonPrice = 50 };

        var model = new CompetitionTaskListModel(competition);

        model.AwardCriteria.Should().Be(TaskProgress.Completed);
        model.AwardCriteriaWeightings.Should().Be(TaskProgress.Completed);
        model.NonPriceElements.Should().Be(TaskProgress.NotStarted);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_WithNonPriceElements_SetsStatuses(
        Competition competition,
        List<CompetitionSublocation> competitionSublocations)
    {
        competition.ContractLength = 5;
        competition.CompetitionSublocations = competitionSublocations;
        competition.IncludesNonPrice = true;
        competition.Weightings = new() { Price = 50, NonPrice = 50 };
        competition.NonPriceElements = new();

        var model = new CompetitionTaskListModel(competition);

        model.AwardCriteriaWeightings.Should().Be(TaskProgress.Completed);
        model.NonPriceElements.Should().Be(TaskProgress.Completed);
        model.NonPriceWeightings.Should().Be(TaskProgress.NotStarted);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_WithIncompleteNonPriceElementWeightings_SetsStatuses(
        Competition competition,
        List<CompetitionSublocation> competitionSublocations)
    {
        competition.ContractLength = 5;
        competition.CompetitionSublocations = competitionSublocations;
        competition.IncludesNonPrice = true;
        competition.Weightings = new() { Price = 50, NonPrice = 50 };
        competition.NonPriceElements = new()
        {
            IntegrationTypes = new List<IntegrationType>(),
            Implementation = new(),
            ServiceLevel = new(),
            NonPriceWeights = new() { Implementation = 50, Interoperability = 25, },
        };

        var model = new CompetitionTaskListModel(competition);

        model.NonPriceElements.Should().Be(TaskProgress.Completed);
        model.NonPriceWeightings.Should().Be(TaskProgress.InProgress);
        model.ReviewCompetitionCriteria.Should().Be(TaskProgress.CannotStart);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_WithCompleteNonPriceElementWeightings_SetsStatuses(
        Competition competition,
        List<CompetitionSublocation> competitionSublocations)
    {
        competition.ContractLength = 5;
        competition.CompetitionSublocations = competitionSublocations;
        competition.IncludesNonPrice = true;
        competition.Weightings = new() { Price = 50, NonPrice = 50 };
        competition.HasReviewedCriteria = false;
        competition.NonPriceElements = new()
        {
            IntegrationTypes = new List<IntegrationType>(),
            Implementation = new(),
            ServiceLevel = new(),
            NonPriceWeights = new() { Implementation = 50, Interoperability = 25, ServiceLevel = 25 },
        };

        var model = new CompetitionTaskListModel(competition);

        model.NonPriceElements.Should().Be(TaskProgress.Completed);
        model.NonPriceWeightings.Should().Be(TaskProgress.Completed);
        model.ReviewCompetitionCriteria.Should().Be(TaskProgress.NotStarted);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_ReviewedCriteria_SetsStatuses(
        Competition competition,
        List<CompetitionSublocation> competitionSublocations)
    {
        competition.ContractLength = 5;
        competition.CompetitionSublocations = competitionSublocations;
        competition.IncludesNonPrice = true;
        competition.Weightings = new() { Price = 50, NonPrice = 50 };
        competition.HasReviewedCriteria = true;
        competition.NonPriceElements = new()
        {
            IntegrationTypes = new List<IntegrationType>(),
            Implementation = new(),
            ServiceLevel = new(),
            NonPriceWeights = new() { Implementation = 50, Interoperability = 25, ServiceLevel = 25 },
        };

        var model = new CompetitionTaskListModel(competition);

        model.NonPriceWeightings.Should().Be(TaskProgress.Completed);
        model.ReviewCompetitionCriteria.Should().Be(TaskProgress.Completed);
        model.CompareAndScoreSolutions.Should().Be(TaskProgress.NotStarted);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_WithIncompleteSolutionScores_SetsStatuses(
        CatalogueItemId solutionId,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations)
    {
        competition.ContractLength = 5;
        competition.CompetitionSublocations = competitionSublocations;
        competition.IncludesNonPrice = true;
        competition.Weightings = new() { Price = 50, NonPrice = 50 };
        competition.HasReviewedCriteria = true;
        competition.NonPriceElements = new()
        {
            IntegrationTypes = new List<IntegrationType>(),
            Implementation = new(),
            ServiceLevel = new(),
            NonPriceWeights = new() { Implementation = 50, Interoperability = 25, ServiceLevel = 25 },
        };
        competition.CompetitionSolutions = new List<CompetitionSolution>
        {
            new(competition.Id, solutionId)
            {
                Scores = new List<SolutionScore> { new(ScoreType.Implementation, 5) },
            },
        };

        var model = new CompetitionTaskListModel(competition);

        model.ReviewCompetitionCriteria.Should().Be(TaskProgress.Completed);
        model.CompareAndScoreSolutions.Should().Be(TaskProgress.InProgress);
        model.CalculatePrice.Should().Be(TaskProgress.CannotStart);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_WithCompleteSolutionScores_SetsStatuses(
        CatalogueItemId solutionId,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations)
    {
        competition.ContractLength = 5;
        competition.CompetitionSublocations = competitionSublocations;
        competition.IncludesNonPrice = true;
        competition.Weightings = new() { Price = 50, NonPrice = 50 };
        competition.HasReviewedCriteria = true;
        competition.NonPriceElements = new()
        {
            IntegrationTypes = new List<IntegrationType>(),
            Implementation = new(),
            ServiceLevel = new(),
            NonPriceWeights = new() { Implementation = 50, Interoperability = 25, ServiceLevel = 25 },
        };
        competition.CompetitionSolutions = new List<CompetitionSolution>
        {
            new(competition.Id, solutionId)
            {
                Scores = new List<SolutionScore>
                {
                    new(ScoreType.Implementation, 5),
                    new(ScoreType.Interoperability, 5),
                    new(ScoreType.ServiceLevel, 5),
                },
            },
        };

        var model = new CompetitionTaskListModel(competition);

        model.ReviewCompetitionCriteria.Should().Be(TaskProgress.Completed);
        model.CompareAndScoreSolutions.Should().Be(TaskProgress.Completed);
        model.CalculatePrice.Should().Be(TaskProgress.NotStarted);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_WithIncompletePrices_SetsStatuses(
        CatalogueItemId solutionId,
        Competition competition,
        string odsCode)
    {
        var sublocationRecipient = new CompetitionSublocationRecipient { RecipientOdsCode = odsCode, };
        var competitionSublocation = new CompetitionSublocation { SublocationRecipients = [sublocationRecipient], };
        competition.ContractLength = 5;
        competition.CompetitionSublocations = [competitionSublocation];
        competition.IncludesNonPrice = true;
        competition.Weightings = new() { Price = 50, NonPrice = 50 };
        competition.HasReviewedCriteria = true;
        competition.NonPriceElements = new()
        {
            IntegrationTypes = new List<IntegrationType>(),
            Implementation = new(),
            ServiceLevel = new(),
            NonPriceWeights = new() { Implementation = 50, Interoperability = 25, ServiceLevel = 25 },
        };
        var quantity = new CompetitionItemQuantity { RecipientOdsCode = odsCode, Quantity = null, };
        competition.CompetitionSolutions = new List<CompetitionSolution>
        {
            new(competition.Id, solutionId)
            {
                Scores = new List<SolutionScore>
                {
                    new(ScoreType.Implementation, 5),
                    new(ScoreType.Interoperability, 5),
                    new(ScoreType.ServiceLevel, 5),
                },
                Price = new CompetitionCatalogueItemPrice(),
                Quantities = [quantity],
            },
        };

        var model = new CompetitionTaskListModel(competition);

        model.ReviewCompetitionCriteria.Should().Be(TaskProgress.Completed);
        model.CompareAndScoreSolutions.Should().Be(TaskProgress.Completed);
        model.CalculatePrice.Should().Be(TaskProgress.InProgress);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_WithCompletePrices_SetsStatuses(
        CatalogueItemId solutionId,
        Competition competition,
        string odsCode)
    {
        var sublocationRecipient = new CompetitionSublocationRecipient { RecipientOdsCode = odsCode, };
        var competitionSublocation = new CompetitionSublocation { SublocationRecipients = [sublocationRecipient], };
        competition.ContractLength = 5;
        competition.CompetitionSublocations = [competitionSublocation];
        competition.IncludesNonPrice = true;
        competition.Weightings = new() { Price = 50, NonPrice = 50 };
        competition.HasReviewedCriteria = true;
        competition.NonPriceElements = new()
        {
            IntegrationTypes = new List<IntegrationType>(),
            Implementation = new(),
            ServiceLevel = new(),
            NonPriceWeights = new() { Implementation = 50, Interoperability = 25, ServiceLevel = 25 },
        };
        var quantity = new CompetitionItemQuantity { RecipientOdsCode = odsCode, Quantity = 5, };
        competition.CompetitionSolutions = new List<CompetitionSolution>
        {
            new(competition.Id, solutionId)
            {
                Scores = new List<SolutionScore>
                {
                    new(ScoreType.Implementation, 5),
                    new(ScoreType.Interoperability, 5),
                    new(ScoreType.ServiceLevel, 5),
                },
                Price = new CompetitionCatalogueItemPrice(),
                Quantities = [quantity],
            },
        };

        var model = new CompetitionTaskListModel(competition);

        model.ReviewCompetitionCriteria.Should().Be(TaskProgress.Completed);
        model.CompareAndScoreSolutions.Should().Be(TaskProgress.Completed);
        model.CalculatePrice.Should().Be(TaskProgress.Completed);
    }
}
