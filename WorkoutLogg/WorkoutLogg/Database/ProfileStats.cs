namespace WorkoutLogg.Database
{
    public record ProfileStats(
        int TotalSessions,
        int TotalSets,
        int UniqueExerciseCount,
        int PlanBasedSessions,
        bool HasHeavySet,
        int CurrentStreak,
        int MaxWeekSessions,
        List<PersonalRecordEntry> TopPRs,
        List<PersonalRecordEntry> AllPRs,
        bool HasEarlySession
    );

    public record PersonalRecordEntry(string ExerciseName, double MaxWeightKg);
}
