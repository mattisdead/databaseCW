using System;
using Npgsql;
using System.Collections.Generic;

namespace ConsoleApp
{
    public class SubjectRepo
    {
        NpgsqlConnection connection;
        public SubjectRepo(NpgsqlConnection connection)
        {
            this.connection = connection;
        }
        public static Subject GetSubject(NpgsqlDataReader reader)
        {
            Subject sub = new()
            {
                id = reader.GetInt32(0),
                name = reader.GetString(1),
                teacherId = reader.GetInt32(2)
            };
            return sub;
        }
        public Subject GetById(int id)
        {
            string command = $"SELECT * FROM subjects WHERE id = {id}";
            NpgsqlCommand cmd = new(command, connection);

            NpgsqlDataReader reader = cmd.ExecuteReader();
            Subject sub = new();

            if (reader.Read())
            {
                sub.id = reader.GetInt32(0);
                sub.name = reader.GetString(1);
                sub.teacherId = reader.GetInt32(2);
            }
            reader.Close();
            return sub;
        }
        public Subject GetByName(string name)
        {
            string command = $"SELECT * FROM subjects WHERE name = '{name}'";
            NpgsqlCommand cmd = new(command, connection);

            NpgsqlDataReader reader = cmd.ExecuteReader();
            Subject sub = new();

            if (reader.Read())
            {
                sub.id = reader.GetInt32(0);
                sub.name = reader.GetString(1);
                sub.teacherId = reader.GetInt32(2);
            }
            reader.Close();
            return sub;
        }
        public List<Subject> GetByTeacherId(int id)
        {
            string command = $"SELECT * FROM subjects WHERE teacher_id = {id}";
            NpgsqlCommand cmd = new(command, connection);

            NpgsqlDataReader reader = cmd.ExecuteReader();
            List<Subject> subjects = new();

            while (reader.Read())
            {
                Subject sub = GetSubject(reader);
                subjects.Add(sub);
            }
            reader.Close();
            return subjects;
        }
        public bool Insert(Subject sub)
        {
            string command = $"INSERT INTO subjects (name, teacher_id) VALUES ('{sub.name}', {sub.teacherId});";

            NpgsqlCommand cmd = new(command, connection);
            if (cmd.ExecuteScalar() == null) return false;
            return true;
        }
        public bool DeleteById(int id)
        {
            string command = $"DELETE FROM subjects WHERE id = {id};";

            NpgsqlCommand cmd = new(command, connection);

            int res = cmd.ExecuteNonQuery();

            GradeRepo gradeRepo = new(connection);
            gradeRepo.DeleteBySubjectId(id);

            if (res == -1) return false;
            return true;
        }
        public bool DeleteByTeacherId(int id)
        {
            string command = $"DELETE FROM subjects WHERE teacher_id = {id};";

            NpgsqlCommand cmd = new(command, connection);

            int res = cmd.ExecuteNonQuery();

            if (res == -1) return false;
            return true;
        }
        public List<Subject> GetAll()
        {
            string command = "SELECT * FROM subjects";
            NpgsqlCommand cmd = new(command, connection);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            List<Subject> subjects = new();
            while (reader.Read())
            {
                Subject sub = GetSubject(reader);
                subjects.Add(sub);
            }
            reader.Close();
            return subjects;
        }
        public Subject GetRandomSubject()
        {
            Random r = new();
            string command = $"SELECT * FROM subjects WHERE id = {r.Next(1, this.GetNewId())}";
            NpgsqlCommand cmd = new(command, connection);

            NpgsqlDataReader reader = cmd.ExecuteReader();
            Subject sub = new();

            if (reader.Read())
            {
                sub.id = reader.GetInt32(0);
                sub.name = reader.GetString(1);
                sub.teacherId = reader.GetInt32(2);
            }
            reader.Close();
            return sub;
        }
        public int GetNewId()
        {
            string command = $"SELECT * FROM subjects;";
            NpgsqlCommand cmd = new(command, connection);

            NpgsqlDataReader reader = cmd.ExecuteReader();

            int id = 1;
            while (reader.Read())
            {
                int currId = reader.GetInt32(0);
                if (currId > id) id = currId;
            }
            id++;
            reader.Close();
            return id;
        }
    }
}