using MuscleGroups;
using Sets;

namespace Exercises {
    public class Exercise {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public List<Set> Sets { get; set; }
        public MuscleGroup MuscleGroup { get; set; }

        public Exercise() {
        }

        public Exercise(string name, string description, int duration, List<Set> sets, MuscleGroup muscleGroup) {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            Duration = duration;
            Sets = sets;
            MuscleGroup = muscleGroup;
        }
    }
}