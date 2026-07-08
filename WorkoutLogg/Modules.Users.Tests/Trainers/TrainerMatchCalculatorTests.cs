using Modules.Trainers.Infrastructure.Domain;
using Modules.Trainers.Infrastructure.Services;
using Modules.Users.DTO.Users;

namespace Modules.Users.Tests.Trainers;

[TestFixture]
public class TrainerMatchCalculatorTests
{
    private static TrainerProfile Trainer(
        TrainerSpecializations specs = TrainerSpecializations.Strength,
        TrainingFormats formats = TrainingFormats.Online,
        int price = 450) => new()
        {
            Specializations = specs,
            Formats = formats,
            PricePerSession = price
        };

    [Test]
    public void NoPreferences_GivesFullScore()
    {
        var score = TrainerMatchCalculator.CalculateMatch(Trainer(), new StudentPreferences());

        Assert.That(score, Is.EqualTo(100));
    }

    [Test]
    public void FullMatch_GivesFullScore()
    {
        var prefs = new StudentPreferences
        {
            DesiredSpecializations = TrainerSpecializations.Strength,
            DesiredFormats = TrainingFormats.Online,
            Budget = 500
        };

        Assert.That(TrainerMatchCalculator.CalculateMatch(Trainer(), prefs), Is.EqualTo(100));
    }

    [Test]
    public void PartialSpecializationOverlap_GivesProportionalScore()
    {
        var trainer = Trainer(specs: TrainerSpecializations.Strength);
        var prefs = new StudentPreferences
        {
            // хочет силовые + йогу, тренер умеет только силовые → 25 из 50
            DesiredSpecializations = TrainerSpecializations.Strength | TrainerSpecializations.Yoga
        };

        var score = TrainerMatchCalculator.CalculateMatch(trainer, prefs);

        Assert.That(score, Is.EqualTo(25 + TrainerMatchCalculator.FormatWeight + TrainerMatchCalculator.PriceWeight));
    }

    [Test]
    public void WrongFormat_LosesFormatWeight()
    {
        var trainer = Trainer(formats: TrainingFormats.Gym);
        var prefs = new StudentPreferences { DesiredFormats = TrainingFormats.Online };

        var score = TrainerMatchCalculator.CalculateMatch(trainer, prefs);

        Assert.That(score, Is.EqualTo(TrainerMatchCalculator.SpecializationsWeight + TrainerMatchCalculator.PriceWeight));
    }

    [TestCase(450, 500, TrainerMatchCalculator.PriceWeight)]     // в бюджете
    [TestCase(550, 500, TrainerMatchCalculator.PriceWeight / 2)] // до +20% сверх бюджета
    [TestCase(700, 500, 0)]                                      // сильно дороже
    public void PriceScore_DependsOnBudget(int price, int budget, int expectedPriceScore)
    {
        var trainer = Trainer(price: price);
        var prefs = new StudentPreferences { Budget = budget };

        var score = TrainerMatchCalculator.CalculateMatch(trainer, prefs);

        Assert.That(score, Is.EqualTo(
            TrainerMatchCalculator.SpecializationsWeight + TrainerMatchCalculator.FormatWeight + expectedPriceScore));
    }

    [Test]
    public void MapGoals_CoversDesignMapping()
    {
        Assert.Multiple(() =>
        {
            Assert.That(
                TrainerMatchCalculator.MapGoalsToSpecializations([UserGoalVariant.LoseFat]),
                Is.EqualTo(TrainerSpecializations.WeightLoss));
            Assert.That(
                TrainerMatchCalculator.MapGoalsToSpecializations([UserGoalVariant.BuildMuscle, UserGoalVariant.IncreaseStrength]),
                Is.EqualTo(TrainerSpecializations.Strength));
            Assert.That(
                TrainerMatchCalculator.MapGoalsToSpecializations([UserGoalVariant.ImporveEndurance]),
                Is.EqualTo(TrainerSpecializations.Running | TrainerSpecializations.Crossfit));
            Assert.That(
                TrainerMatchCalculator.MapGoalsToSpecializations([UserGoalVariant.Flexibility]),
                Is.EqualTo(TrainerSpecializations.Yoga));
            Assert.That(
                TrainerMatchCalculator.MapGoalsToSpecializations([UserGoalVariant.StayActive]),
                Is.EqualTo(TrainerSpecializations.None));
            Assert.That(
                TrainerMatchCalculator.MapGoalsToSpecializations(null),
                Is.EqualTo(TrainerSpecializations.None));
        });
    }
}
