using System;
using Npgsql;
using static System.Console;
using System.IO;
using System.Collections.Generic;

namespace ConsoleApp
{
    class Program
    {
        static StudentRepo studentRepo;
        static TeacherRepo teacherRepo;
        static SubjectRepo subjectRepo;
        static GradeRepo gradeRepo;
        static void Main(string[] args)
        {
            NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;Database=cwdb;User Id=postgres;Password=wake up call;");
            conn.Open();
            studentRepo = new StudentRepo(conn);
            teacherRepo = new TeacherRepo(conn);
            subjectRepo = new SubjectRepo(conn);
            gradeRepo = new GradeRepo(conn);

            WriteLine("Enter your command:");
            string command = ReadLine();
            if(command.Equals("Predict", StringComparison.InvariantCultureIgnoreCase))
            {
                WriteLine("What is the id of the student?");
                int studentId= 0; 
                if(!int.TryParse(ReadLine(), out studentId))
                {
                    WriteLine("Invalid id");
                    return;
                }

                int subjectId = 0;
                WriteLine("What is the id of the subject?");
                if(!int.TryParse(ReadLine(),out subjectId))
                {
                    WriteLine("Invalid id");
                    return;
                }

                float predictedGrade = Analysis.AnalysisProgram.Predict(new Analysis.Grade()
                { 
                    studentId = studentId,
                    subjectId = subjectId,
                    grade = 0
                });

                List<Grade> grades = gradeRepo.GetListOfGradesByStudentId(studentId);
                List<Grade> lastTenGrades = new();
                for (int i = grades.Count - 10; i < grades.Count; i++)
                {
                    lastTenGrades.Add(grades[i]);
                }

                List<double> doubleGrades = new();

                for(int i = 0; i < lastTenGrades.Count; i++)
                {
                    doubleGrades.Add(grades[i].grade);
                }
                Graph.CreateGradeGraph(doubleGrades.ToArray(), "C:/Users/User/projects/databaseCW/data/actualGradesGraph.png");


                doubleGrades.Add(predictedGrade);
                Graph.CreateGradeGraph(doubleGrades.ToArray(), "C:/Users/User/projects/databaseCW/data/predictedGradesGraph.png");
                return;
            }
            else if(command.Equals("Log in", StringComparison.InvariantCultureIgnoreCase))
            {
                WriteLine("Are you a teacher (t) or a student (s)?");
                string role = ReadLine();
                Person user = null;
                if (role == "t") user = new Teacher();
                else if (role == "s") user = new Student();
                else
                {
                    WriteLine("Invalid input.");
                    return;
                }

                WriteLine("What's your name?");
                user.name = ReadLine();

                WriteLine("And what is your surname?");
                user.surname = ReadLine();

                if (user.GetType().Equals(typeof(Teacher)))
                {
                    Teacher t = teacherRepo.GetByFullName(user.name, user.surname);
                    if (t.id == 0)
                    {
                        teacherRepo.Insert(new Teacher(teacherRepo.GetNewId(), user.name, user.surname));
                        t = teacherRepo.GetByFullName(user.name, user.surname);
                    }
                    TeacherLogin.MainWindow(t);
                }
                else
                {
                    Student s = studentRepo.GetByFullName(user.name, user.surname);
                    if (s.id == 0)
                    {
                        studentRepo.Insert(new Student(studentRepo.GetNewId(), user.name, user.surname));
                        s = studentRepo.GetByFullName(user.name, user.surname);
                    }
                    StudentLogin.MainWindow(s);
                }
            }
        }
        static void DataGen(int number)
        {
            for (int i = 0; i < number; i++)
            {
                WriteLine("What data would you like to generate?");
                string command = ReadLine();

                if (command == "students")
                {
                    string allNames = File.ReadAllText("../data/names.txt");
                    string allSurnames = File.ReadAllText("../data/surnames.txt");

                    string[] arrNames = allNames.Split("\r\n");
                    string[] arrSurnames = allSurnames.Split("\r\n");

                    Random r = new Random();

                    string name = arrNames[r.Next(0, arrNames.Length)];
                    string surname = arrSurnames[r.Next(0, arrSurnames.Length)];

                    WriteLine("Meet " + name + " " + surname + "!");
                    studentRepo.Insert(new Student(1, name, surname));
                }
                else if (command == "teachers")
                {
                    string allNames = File.ReadAllText("../data/names.txt");
                    string allSurnames = File.ReadAllText("../data/surnames.txt");

                    string[] arrNames = allNames.Split("\r\n");
                    string[] arrSurnames = allSurnames.Split("\r\n");

                    Random r = new Random();

                    string name = arrNames[r.Next(0, arrNames.Length)];
                    string surname = arrSurnames[r.Next(0, arrSurnames.Length)];

                    WriteLine("Meet " + name + " " + surname + "!");
                    teacherRepo.Insert(new Teacher(1, name, surname));
                }
                else if (command == "grades")
                {
                    Random r = new Random();

                    int perc = r.Next(0, 101);
                    int mark = -1;

                    if (perc < 10)
                    {
                        mark = r.Next(0, 60);
                    }
                    else if (perc < 90)
                    {
                        mark = r.Next(60, 90);
                    }
                    else
                    {
                        mark = r.Next(90, 101);
                    }

                    gradeRepo.Insert(new Grade(mark, studentRepo.GetRandomStudent().id, subjectRepo.GetRandomSubject().id));
                }
                else if (command == "subjects")
                {
                    string allSubjects = File.ReadAllText("../data/subjects.txt");
                    string[] arrSubjects = allSubjects.Split("\r\n");

                    Random r = new Random();

                    string subject = arrSubjects[r.Next(0, arrSubjects.Length)];

                    Teacher t = teacherRepo.GetRandomTeacher();

                    WriteLine(subject);

                    subjectRepo.Insert(new Subject(0, subject, t.id));
                }
            }
        }
        static void CreateCSVFromTable()
        {
            List<Grade> grades = gradeRepo.GetAll();
            string csvtext = "";
            for (int i=0; i < grades.Count; i++)
            {
                csvtext += grades[i].studentId.ToString() + ',' + grades[i].subjectId.ToString() + ',' + grades[i].grade.ToString() + "\r\n";
                if(i < 3) WriteLine(csvtext);
            }
            File.WriteAllText("C:/Users/User/projects/databaseCW/Analysis/Data/GradesTableTrain.csv", csvtext);
        }
    }
}
