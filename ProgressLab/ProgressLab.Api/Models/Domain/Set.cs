using Exercises;

namespace Sets {
    public class Set {
        private Guid Id { get; set; }
        private int SetNumber { get; set; }
        private int Repetitions { get; set; }
        private int Weight { get; set; }
        private Exercise Exercise { get; set; }

        public Set() {
        }

        public Set(int setNumber, int repetitions, int weight, Exercise exercise) {
            Id = Guid.NewGuid();
            SetNumber = setNumber;
            Repetitions = repetitions;
            Weight = weight;
            Exercise = exercise;
        }
    }
}