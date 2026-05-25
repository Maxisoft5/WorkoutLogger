using SQLite;
using WorkoutLogg.Database.Entities;

namespace WorkoutLogg.Database
{
    public class WorkoutDatabase
    {
        private SQLiteAsyncConnection? _db;

        private async Task<SQLiteAsyncConnection> GetConnection()
        {
            if (_db is not null) return _db;

            var path = Path.Combine(FileSystem.AppDataDirectory, "workoutlogg.db3");
            _db = new SQLiteAsyncConnection(path,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

            await _db.CreateTableAsync<WorkoutEntity>();
            await _db.CreateTableAsync<WorkoutSetEntity>();
            await _db.CreateTableAsync<WorkoutSetDetailEntity>();
            await _db.CreateTableAsync<WorkoutLogSessionEntity>();
            await _db.CreateTableAsync<LogExerciseEntity>();
            await _db.CreateTableAsync<LogSetEntity>();

            return _db;
        }

        // ── Workouts ─────────────────────────────────────────────────────────

        public async Task<List<WorkoutEntity>> GetWorkoutsAsync()
        {
            var db = await GetConnection();
            var workouts = await db.Table<WorkoutEntity>()
                .OrderByDescending(w => w.StartDate)
                .ToListAsync();

            foreach (var w in workouts)
                w.Exercises = await GetExercisesWithSetsAsync(w.Id);

            return workouts;
        }

        public async Task<WorkoutEntity?> GetWorkoutWithExercisesAsync(Guid workoutId)
        {
            var db = await GetConnection();
            var workout = await db.Table<WorkoutEntity>()
                .Where(w => w.Id == workoutId)
                .FirstOrDefaultAsync();

            if (workout is null) return null;
            workout.Exercises = await GetExercisesWithSetsAsync(workoutId);
            return workout;
        }

        public async Task<List<WorkoutEntity>> GetWorkoutsForWeekAsync(DateTime weekStart)
        {
            var db = await GetConnection();
            var weekEnd = weekStart.AddDays(7);
            var workouts = await db.Table<WorkoutEntity>()
                .Where(w => w.StartDate >= weekStart && w.StartDate < weekEnd)
                .ToListAsync();

            foreach (var w in workouts)
                w.Exercises = await GetExercisesWithSetsAsync(w.Id);

            return workouts;
        }

        public async Task<int> SaveWorkoutAsync(WorkoutEntity workout)
        {
            var db = await GetConnection();
            return await db.InsertOrReplaceAsync(workout);
        }

        public async Task DeleteWorkoutAsync(Guid workoutId)
        {
            var db = await GetConnection();
            var exercises = await db.Table<WorkoutSetEntity>()
                .Where(e => e.WorkoutId == workoutId).ToListAsync();

            foreach (var ex in exercises)
                await db.Table<WorkoutSetDetailEntity>()
                    .Where(s => s.WorkoutSetId == ex.Id).DeleteAsync();

            await db.Table<WorkoutSetEntity>().Where(e => e.WorkoutId == workoutId).DeleteAsync();
            await db.DeleteAsync<WorkoutEntity>(workoutId);
        }

        // ── Exercises ────────────────────────────────────────────────────────

        public async Task<List<WorkoutSetEntity>> GetSetsAsync(Guid workoutId)
        {
            var db = await GetConnection();
            return await db.Table<WorkoutSetEntity>()
                .Where(s => s.WorkoutId == workoutId).ToListAsync();
        }

        private async Task<List<WorkoutSetEntity>> GetExercisesWithSetsAsync(Guid workoutId)
        {
            var db = await GetConnection();
            var exercises = await db.Table<WorkoutSetEntity>()
                .Where(e => e.WorkoutId == workoutId).ToListAsync();

            foreach (var ex in exercises)
                ex.Sets = await db.Table<WorkoutSetDetailEntity>()
                    .Where(s => s.WorkoutSetId == ex.Id)
                    .OrderBy(s => s.SetNumber)
                    .ToListAsync();

            return exercises;
        }

        public async Task SaveExerciseAsync(WorkoutSetEntity exercise)
        {
            var db = await GetConnection();
            if (exercise.Id == default) exercise.Id = Guid.NewGuid();
            await db.InsertOrReplaceAsync(exercise);
        }

        public async Task ReplaceExercisesAsync(Guid workoutId, IEnumerable<WorkoutSetEntity> exercises)
        {
            var db = await GetConnection();

            var existing = await db.Table<WorkoutSetEntity>()
                .Where(e => e.WorkoutId == workoutId).ToListAsync();

            foreach (var ex in existing)
                await db.Table<WorkoutSetDetailEntity>()
                    .Where(s => s.WorkoutSetId == ex.Id).DeleteAsync();

            await db.Table<WorkoutSetEntity>().Where(e => e.WorkoutId == workoutId).DeleteAsync();

            foreach (var ex in exercises)
            {
                if (ex.Id == default) ex.Id = Guid.NewGuid();
                ex.WorkoutId = workoutId;
                await db.InsertAsync(ex);

                foreach (var detail in ex.Sets)
                {
                    if (detail.Id == default) detail.Id = Guid.NewGuid();
                    detail.WorkoutSetId = ex.Id;
                    await db.InsertAsync(detail);
                }
            }
        }

        // ── Set details ───────────────────────────────────────────────────────

        public async Task<List<WorkoutSetDetailEntity>> GetSetDetailsAsync(Guid workoutSetId)
        {
            var db = await GetConnection();
            return await db.Table<WorkoutSetDetailEntity>()
                .Where(s => s.WorkoutSetId == workoutSetId)
                .OrderBy(s => s.SetNumber)
                .ToListAsync();
        }

        public async Task SaveSetDetailAsync(WorkoutSetDetailEntity detail)
        {
            var db = await GetConnection();
            if (detail.Id == default) detail.Id = Guid.NewGuid();
            await db.InsertOrReplaceAsync(detail);
        }

        public async Task DeleteSetDetailAsync(Guid detailId)
        {
            var db = await GetConnection();
            await db.DeleteAsync<WorkoutSetDetailEntity>(detailId);
        }

        public async Task DeleteExerciseAsync(Guid exerciseId)
        {
            var db = await GetConnection();
            await db.Table<WorkoutSetDetailEntity>()
                .Where(s => s.WorkoutSetId == exerciseId).DeleteAsync();
            await db.DeleteAsync<WorkoutSetEntity>(exerciseId);
        }

        // ── Log sessions ─────────────────────────────────────────────────────

        public async Task<List<WorkoutLogSessionEntity>> GetLogSessionsForDateAsync(DateTime date)
        {
            var db = await GetConnection();
            var start = date.Date;
            var end = start.AddDays(1);
            var sessions = await db.Table<WorkoutLogSessionEntity>()
                .Where(s => s.Date >= start && s.Date < end)
                .ToListAsync();

            foreach (var s in sessions)
                s.Exercises = await GetLogExercisesWithSetsAsync(s.Id);

            return sessions;
        }

        public async Task<WorkoutLogSessionEntity?> GetLogSessionWithExercisesAsync(Guid sessionId)
        {
            var db = await GetConnection();
            var session = await db.Table<WorkoutLogSessionEntity>()
                .Where(s => s.Id == sessionId)
                .FirstOrDefaultAsync();
            if (session is null) return null;
            session.Exercises = await GetLogExercisesWithSetsAsync(sessionId);
            return session;
        }

        public async Task<List<DateTime>> GetLoggedDatesAsync()
        {
            var db = await GetConnection();
            var sessions = await db.Table<WorkoutLogSessionEntity>().ToListAsync();
            return sessions.Select(s => s.Date.Date).Distinct().ToList();
        }

        public async Task SaveLogSessionAsync(WorkoutLogSessionEntity session)
        {
            var db = await GetConnection();
            await db.InsertOrReplaceAsync(session);
        }

        public async Task ReplaceExerciseLogsAsync(Guid sessionId, IEnumerable<LogExerciseEntity> exercises)
        {
            var db = await GetConnection();

            var existing = await db.Table<LogExerciseEntity>()
                .Where(e => e.SessionId == sessionId).ToListAsync();

            foreach (var ex in existing)
                await db.Table<LogSetEntity>()
                    .Where(s => s.ExerciseLogId == ex.Id).DeleteAsync();

            await db.Table<LogExerciseEntity>().Where(e => e.SessionId == sessionId).DeleteAsync();

            foreach (var ex in exercises)
            {
                if (ex.Id == default) ex.Id = Guid.NewGuid();
                ex.SessionId = sessionId;
                await db.InsertAsync(ex);

                foreach (var set in ex.Sets)
                {
                    if (set.Id == default) set.Id = Guid.NewGuid();
                    set.ExerciseLogId = ex.Id;
                    await db.InsertAsync(set);
                }
            }
        }

        public async Task DeleteLogSessionAsync(Guid sessionId)
        {
            var db = await GetConnection();

            var exercises = await db.Table<LogExerciseEntity>()
                .Where(e => e.SessionId == sessionId).ToListAsync();

            foreach (var ex in exercises)
                await db.Table<LogSetEntity>()
                    .Where(s => s.ExerciseLogId == ex.Id).DeleteAsync();

            await db.Table<LogExerciseEntity>().Where(e => e.SessionId == sessionId).DeleteAsync();
            await db.Table<WorkoutLogSessionEntity>().Where(s => s.Id == sessionId).DeleteAsync();
        }

        private async Task<List<LogExerciseEntity>> GetLogExercisesWithSetsAsync(Guid sessionId)
        {
            var db = await GetConnection();
            var exercises = await db.Table<LogExerciseEntity>()
                .Where(e => e.SessionId == sessionId).ToListAsync();

            foreach (var ex in exercises)
                ex.Sets = await db.Table<LogSetEntity>()
                    .Where(s => s.ExerciseLogId == ex.Id)
                    .OrderBy(s => s.SetNumber)
                    .ToListAsync();

            return exercises;
        }

        // ── Sync ─────────────────────────────────────────────────────────────

        public async Task<List<WorkoutEntity>> GetUnsyncedAsync()
        {
            var db = await GetConnection();
            var workouts = await db.Table<WorkoutEntity>()
                .Where(w => !w.IsSynced).ToListAsync();

            foreach (var w in workouts)
                w.Exercises = await GetExercisesWithSetsAsync(w.Id);

            return workouts;
        }
    }
}
