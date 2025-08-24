using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using graphql_playground.DTOs;
using Microsoft.EntityFrameworkCore;

namespace graphql_playground.Services
{


    public class SchoolDbContext : DbContext
    {

        public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options)
        {

        }


        public DbSet<CourseDTO> Courses { get; set; }
        public DbSet<InstructorDTO> Instructors { get; set; }
        public DbSet<StudentDTO> Students { get; set; }
    }
}