using System;
using static System.Console;
using System.Collections.Generic;
using Npgsql;

namespace ConsoleApp
{
    public class StudentLogin
    {
        static Student user;
        static StudentRepo studentRepo;
        static TeacherRepo teacherRepo;
        static SubjectRepo subjectRepo;
        static GradeRepo gradeRepo;
        static NpgsqlConnection connection;
        public static void MainWindow(Student student)
        {
            user = student;
            connection = new NpgsqlConnection("Server=localhost;Port=5432;Database=cwdb;User Id=postgres;Password=wakeupcall;");
            connection.Open();
            studentRepo = new StudentRepo(connection);
            teacherRepo = new TeacherRepo(connection);
            subjectRepo = new SubjectRepo(connection);
            gradeRepo = new GradeRepo(connection);

            WriteLine("To see list of commands type in \"Help\"\r\n");
            while (true)
            {
                WriteLine("Enter your command:");
                string command = ReadLine();
                if (command.Equals("All grades", StringComparison.InvariantCultureIgnoreCase)) // works
                {
                    List<Grade> grades = gradeRepo.GetListOfGradesByStudentId(user.id);
                    for (int i = 0; i < grades.Count; i++)
                    {
                        string subjectName = subjectRepo.GetById(grades[i].subjectId).name;

                        WriteLine("You got {0} in {1}", grades[i].grade, subjectName);
                    }
                }
                else if (command.Equals("Grades", StringComparison.InvariantCultureIgnoreCase)) // works
                {
                    Subject sub;
                    string subj;
                    while (true)
                    {
                        WriteLine("Which subject do you want to check?");
                        subj = ReadLine();
                        sub = subjectRepo.GetByName(subj);
                        if (sub.id == 0)
                        {
                            WriteLine("No subject found");
                            continue;
                        }
                        break;
                    }
                    List<Grade> grades = gradeRepo.GetListOfGradesByStudentIdAndSubjectId(user.id, sub.id);
                    for (int i = 0; i < grades.Count; i++)
                    {
                        WriteLine("You got {0} in {1}", grades[i].grade, subj);
                    }
                }
                else if (command.Equals("Subjects", StringComparison.InvariantCultureIgnoreCase)) // works
                {
                    List<Grade> grades = gradeRepo.GetListOfGradesByStudentId(user.id);
                    List<int> intSubjects = new();
                    List<Subject> subjects = new();
                    for (int i = 0; i < grades.Count; i++)
                    {
                        if (!intSubjects.Contains(grades[i].subjectId))
                        {
                            intSubjects.Add(grades[i].subjectId);
                        }
                    }
                    for (int i = 0; i < intSubjects.Count; i++)
                    {
                        Subject currSub = subjectRepo.GetById(intSubjects[i]);
                        subjects.Add(currSub);
                    }
                    for (int i = 0; i < subjects.Count; i++)
                    {
                        WriteLine(subjects[i].name);
                    }
                }
                else if (command.Equals("Average", StringComparison.InvariantCultureIgnoreCase)) // works
                {
                    List<Grade> grades = gradeRepo.GetListOfGradesByStudentId(user.id);
                    double average = 0;
                    for (int i = 0; i < grades.Count; i++)
                    {
                        average += grades[i].grade;
                    }
                    average /= grades.Count;
                    WriteLine(average);
                }
                else if (command.Equals("Help", StringComparison.InvariantCultureIgnoreCase)) Help();
            }
        }
        static void Help()
        {
            WriteLine(@"All grades - see all of your grades in different subjects
Grades - see all of your grades in the specific subject
Subjects - see all of your subjects
Average - see your average score in all of your classes");
        }
    }
}