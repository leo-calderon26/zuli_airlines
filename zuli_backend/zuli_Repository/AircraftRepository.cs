using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using zuli_Data;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class AircraftRepository : IAircraftRepository
    {
        // Inyeccion de dependencias de la capa Data
        private readonly AppDbContext _context;

        public AircraftRepository(AppDbContext context) 
        {
            _context = context; 
        }

        public async Task<AircraftEntity?> CreateAircraft(AircraftEntity aircraft)
        {
            // Lo que va hacer esto es ir a ver si existe una aeronave ya con el mismo id esto lo retorna
            var existingAircraft = await _context.Aircraft.Where(respose => respose.AircraftId.Equals(aircraft.AircraftId)
            ).FirstOrDefaultAsync();
            if (existingAircraft == null) 
            {
                await _context.Aircraft.AddAsync(aircraft);
                await _context.SaveChangesAsync();
                return aircraft;
            }
            else
            {
                // Cambiar esto por un mensaje mas personalizado
                return null; 
            }
        }

    }   
}
