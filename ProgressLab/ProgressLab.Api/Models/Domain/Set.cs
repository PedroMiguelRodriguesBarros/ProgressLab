using Exercises;

namespace Sets {
    public class Set {
        public Guid SetId { get; set; }
        public int SetNumber { get; set; }
        public int Repetitions { get; set; }
        public float Weight { get; set; }
        public Exercise Exercise { get; set; }

        public Set() {
        }

        public Set(int setNumber, int repetitions, float weight, Exercise exercise) {
            SetId = Guid.NewGuid();
            SetNumber = setNumber;
            Repetitions = repetitions;
            Weight = weight;
            Exercise = exercise;
        }
    }
}