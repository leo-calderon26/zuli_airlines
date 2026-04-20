using Microsoft.EntityFrameworkCore;
using zuli_Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace zuli_Data
{
    public class AppDbContext : DbContext // DbContext es un punte entre C# y sql
    {
        // Esto lo hace es darle contexto de las tablas disponibles en la base de datos
        // base significa que se va usar el constructor por defecto de la case
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){ }
        
        public DbSet<AircraftEntity> Aircraft {  get; set; }
    }
}
