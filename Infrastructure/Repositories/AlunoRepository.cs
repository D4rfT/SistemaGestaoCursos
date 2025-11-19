using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AlunoRepository : BaseRepository<Aluno>, IAlunoRepository
    {
        public AlunoRepository(ApplicationDbContext context) :base(context) 
        {
        }
        public async Task<Aluno> GetByCpfAsync(string cpf)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.CPF == cpf);
        }

        public async Task<Aluno> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.Email.ToLower() == email.ToLower());
        }

        public async Task<Aluno> GetByRegistroAcademicoAsync(string registroAcademico)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.RegistroAcademico == registroAcademico);
        }

        public async Task<bool> VerificarCpfExistenteAsync(string cpf, int? excludeAlunoId = null)
        {
            var query = _dbSet.Where(a => a.CPF == cpf);

            if (excludeAlunoId.HasValue)
                query = query.Where(a => a.Id != excludeAlunoId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> VerificarEmailExistenteAsync(string email, int? excludeAlunoId = null)
        {
            var query = _dbSet.Where(a => a.Email.ToLower() == email.ToLower());

            if (excludeAlunoId.HasValue)
                query = query.Where(a => a.Id != excludeAlunoId.Value);

            return await query.AnyAsync();
        }

        public override async Task<Aluno> GetByIdAsync(int id)
        {
            return await _dbSet.Include(a => a.Curso).FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
