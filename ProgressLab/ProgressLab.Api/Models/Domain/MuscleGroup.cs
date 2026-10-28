namespace MuscleGroups {
    public class MuscleGroup {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public MuscleGroup() {
        }
        public MuscleGroup(string name, string description) {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
        }
    }
}