namespace MuscleGroups {
    public class MuscleGroup {
        private Guid Id { get; set; }
        private string Name { get; set; }
        private string Description { get; set; }

        public MuscleGroup() {
        }
        public MuscleGroup(string name, string description) {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
        }
    }
}