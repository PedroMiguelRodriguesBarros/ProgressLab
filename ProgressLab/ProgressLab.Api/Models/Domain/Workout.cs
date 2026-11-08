namespace Workouts {
    public class Workout {
        public Guid WorkoutId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset Date { get; set; }
        public string? Notes { get; set; }
        public List<WorkoutExercise> WorkoutExercises { get; set; } = new();

        public Workout() {
        }

        public Workout(string name, string? notes) {
            WorkoutId = Guid.NewGuid();
            Name = name;
            Date = DateTimeOffset.UtcNow;
            Notes = notes;
        }
    }
}