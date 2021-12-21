namespace ConsoleApp
{
    public class Teacher : Person
    {
        public Teacher() : base() { }
        public Teacher(int id, string name, string surname)
        {
            this.id = id;
            this.name = name;
            this.surname = surname;
        }
    }
}