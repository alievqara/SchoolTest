using System;
using System.IO;
using System.Collections.Generic;
using static Program;


public class Program
{
    public static void Main()
    {
        var teachers = new List<Teacher>();
        var students = new List<Student>();
        var exams = new List<Exam>();



        LoadData("teachers.csv", teachers);
        LoadData("students.csv", students);
        LoadData("exams.csv", exams);


        //1. Обработка ошибок, если таковые имеются.
        //2. Перечислите всех учителей.
        //3. Найдите средний возраст студентов на курсах, отличных от математики, в 2023 году.
        //4. Найдите всех учителей со средним баллом выше 90 на экзаменах, кроме Алекса.
        //5. Сортировка по количеству учеников для каждого учителя.

        if (teachers != null)
        {
            try
            {
                Console.WriteLine("------------------------------------------------------------------------------------");
                Console.WriteLine("All Teachers:");
                Console.WriteLine("------------------------------------------------------------------------------------");
                foreach (var teacher in teachers)
                {
                    Console.WriteLine($"{teacher.Name} {teacher.LastName}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(" Error: " + ex.Message);

            }

            try
            {
                Console.WriteLine("------------------------------------------------------------------------------------");
                Console.WriteLine("Teachers with a GPA above 90:");
                Console.WriteLine("------------------------------------------------------------------------------------");

                foreach (var teacher in teachers)
                {
                    if (teacher.Name != "Alex")
                    {
                        var teacherExams = exams.Where(exam => exam.TeacherId == teacher.ID);

                        if (teacherExams.Any())
                        {
                            decimal averageScore = teacherExams.Average(exam => exam.Score);

                            if (averageScore > 90)
                            {
                                Console.WriteLine($"{teacher.Name} {teacher.LastName}");
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(" Error: " + ex.Message);
            }

            try
            {
                var teacherStudentCounts = new Dictionary<Teacher, int>();

                foreach (var teacher in teachers)
                {
                    int studentCount = exams.Count(exam => exam.TeacherId == teacher.ID);
                    teacherStudentCounts.Add(teacher, studentCount);
                }

                var sortedTeachers = teacherStudentCounts.OrderBy(pair => pair.Value);

                Console.WriteLine("------------------------------------------------------------------------------------");
                Console.WriteLine("Ordered List of Teachers According to Number of Students:");
                Console.WriteLine("------------------------------------------------------------------------------------");
                foreach (var pair in sortedTeachers)
                {
                    Console.WriteLine($"{pair.Key.Name} {pair.Key.LastName}: {pair.Value} student");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

        }


        if (exams != null)
        {
            try
            {


                int totalAge = 0;
                int studentCount = 0;

                foreach (var exam in exams)
                {
                    if (exam.ExamDate.Year == 2023)
                    {
                        var student = students.FirstOrDefault(s => s.ID == exam.StudentId);
                        exam.AssignStudent(student);

                        if (exam.Lesson != LessonType.Mathematics)
                        {
                            totalAge += exam.Student.Age;
                            studentCount++;
                        }
                    }
                }

                if (studentCount > 0)
                {
                    double averageAge = (double)totalAge / studentCount;
                    Console.WriteLine("------------------------------------------------------------------------------------");
                    Console.WriteLine($"Average age of students in courses other than mathematics in 2023: {averageAge}");
                    Console.WriteLine("------------------------------------------------------------------------------------");
                }
                else
                {
                    Console.WriteLine("------------------------------------------------------------------------------------");
                    Console.WriteLine("In 2023, no students could be found who had exams in courses other than mathematics.");
                    Console.WriteLine("------------------------------------------------------------------------------------");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }









    }

    public static void LoadData<T>(string fileName, List<T> list) where T : IDataItem, new()
    {
        using (var reader = new StreamReader(fileName))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var data = line.Split(',');
                var item = new T();

                item.LoadData(data);

                list.Add(item);
            }
        }
    }
    public interface IDataItem
    {
        void LoadData(string[] data);
    }


    public class Teacher : IDataItem
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public LessonType Lesson { get; set; }

        public void LoadData(string[] data)
        {
            ID = long.Parse(data[0]);
            Name = data[1];
            LastName = data[2];
            Lesson = (LessonType)Enum.Parse(typeof(LessonType), data[3]);
        }


    }

    public class Student : IDataItem
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        public void LoadData(string[] data)
        {
            ID = long.Parse(data[0]);
            Name = data[1];
            LastName = data[2];
            Age = int.Parse(data[3]);
        }
    }

    public class Exam : IDataItem
    {
        public LessonType Lesson { get; set; }

        public long StudentId { get; set; }
        public long TeacherId { get; set; }

        public decimal Score { get; set; }
        public DateTime ExamDate { get; set; }

        public Student Student { get; set; }
        public Teacher Teacher { get; set; }

        public void LoadData(string[] data)
        {
            Lesson = (LessonType)Enum.Parse(typeof(LessonType), data[0]);
            StudentId = long.Parse(data[1]);
            TeacherId = long.Parse(data[2]);
            Score = decimal.Parse(data[3]);
            ExamDate = DateTime.Parse(data[4]);

        }
        public void AssignStudent(Student student)
        {
            Student = student;
        }

    }

    public enum LessonType
    {
        Mathematics = 1,
        Physics = 2,
    }


}