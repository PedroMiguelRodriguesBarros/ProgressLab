using MuscleGroups;
using Sets;

namespace Exercises {
    public class Exercise {
        private Guid Id { get; set; }
        private string Name { get; set; }
        private string Description { get; set; }
        private int Duration { get; set; }
        private List<Set> Sets { get; set; }
        private MuscleGroup MuscleGroup { get; set; }

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