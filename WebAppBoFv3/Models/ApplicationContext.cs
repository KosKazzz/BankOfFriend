using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace WebAppBoFv3.Models
{
    public class ApplicationContext : DbContext     // Контекст класс
    {
        public DbSet<User> Users { get; set; }      // Свойство  для доступа к таблице Users
        public DbSet<Role> Roles { get; set; }      // Свойство  для доступа к таблице Roles
        public DbSet<Company> Companies { get; set; }    // Свойство  для доступа к таблице Companies
        public ApplicationContext(DbContextOptions<ApplicationContext> options) // Передача в коструктор базового класса объекта с параметрами конфигурации подключения к БД
            : base(options)
        {
            Database.EnsureCreated(); // Проверка наличия БД  
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)  // В созданой БД всегда будет юзер с правами админа.
        {
            string adminRoleName = "admin";
            string userRoleName = "user";

            string adminEmail = "admin@zxmail.com";
            string adminPassword = "fMtBzYDS";

            // добавляем роли
            Role adminRole = new Role { Id = 1, Name = adminRoleName };
            Role userRole = new Role { Id = 2, Name = userRoleName };
            User adminUser = new User { Id = 1, Email = adminEmail, Password = adminPassword, RoleId = adminRole.Id };

            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, userRole });
            modelBuilder.Entity<User>().HasData(new User[] { adminUser });
            base.OnModelCreating(modelBuilder);
        }

    }
}
