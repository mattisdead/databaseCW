namespace ConsoleApp
{
    public class Grade
    {
        public double grade;
        public int studentId;
        public int subjectId;
        public Grade() : base() { }
        public Grade(int grade, int studentId, int subjectId)
        {
            this.subjectId = subjectId;
            this.studentId = studentId;
            this.grade = grade;
        }
    }
}