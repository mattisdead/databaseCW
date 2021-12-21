using System;
using Npgsql;
using System.Collections.Generic;

namespace ConsoleApp
{
    public class GradeRepo
    {
        NpgsqlConnection connection;
        public GradeRepo(NpgsqlConnection connection)
        {
            this.connection = connection;
        }
        public static Grade GetGrade(NpgsqlDataReader reader)
        {
            Grade grade = new Grade();
            grade.studentId = reader.GetInt32(0);
            grade.subjectId = reader.GetInt32(1);
            grade.grade = reader.GetDouble(2);
            return grade;
        }
        public bool Insert(Grade grade)
        {
            string command = $"INSERT INTO grades (student_id, subject_id, grade) VALUES ({grade.studentId}, '{grade.subjectId}', {grade.grade});";

            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);
            if (cmd.ExecuteScalar() == null) return false;
            return true;
        }
        public bool DeleteByStudentId(int id)
        {
            string command = $"DELETE FROM grades WHERE student_id = {id};";

            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);

            int res = cmd.ExecuteNonQuery();

            if (res == -1) return false;
            return true;
        }
        public bool DeleteBySubjectId(int id)
        {
            string command = $"DELETE FROM grades WHERE subject_id = {id};";

            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);

            int res = cmd.ExecuteNonQuery();

            if (res == -1) return false;
            return true;
        }
        public List<Grade> GetListOfGradesByStudentId(int id)
        {
            string command = $"SELECT * FROM grades WHERE student_id = {id};";
            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            List<Grade> grades = new List<Grade>();
            while (reader.Read())
            {
                Grade gr = GetGrade(reader);
                grades.Add(gr);
            }
            reader.Close();
            return grades;
        }
        public List<Grade> GetListOfGradesByStudentIdAndSubjectId(int studentId, int subjectId)
        {
            string command = $"SELECT * FROM grades WHERE student_id = {studentId} AND subject_id ={subjectId};";
            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            List<Grade> grades = new List<Grade>();
            while (reader.Read())
            {
                Grade gr = GetGrade(reader);
                grades.Add(gr);
            }
            reader.Close();
            return grades;
        }
        public List<Grade> GetAll()
        {
            string command = $"SELECT * FROM grades";
            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            List<Grade> grades = new List<Grade>();
            while (reader.Read())
            {
                Grade gr = GetGrade(reader);
                grades.Add(gr);
            }
            reader.Close();
            return grades;
        }
    }
}