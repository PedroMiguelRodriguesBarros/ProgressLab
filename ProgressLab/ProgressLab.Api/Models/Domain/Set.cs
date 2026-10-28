using Exercises;

namespace Sets {
    public class Set {
        public Guid Id { get; set; }
        public int SetNumber { get; set; }
        public int Repetitions { get; set; }
        public int Weight { get; set; }
        public Exercise Exercise { get; set; }

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