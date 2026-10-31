namespace MuscleGroups {
    public class MuscleGroup {
        public Guid MuscleGroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public MuscleGroup() {
        }
        public MuscleGroup(string name, string description) {
            MuscleGroupId = Guid.NewGuid();
            Name = name;
            Description = description;
        }
    }
}