using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SistemaGestaoCursos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class MigracaoController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public MigracaoController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("alunos-para-usuarios")]
        public async Task<IActionResult> MigrarAlunosParaUsuarios()
        {
            try
            {
                var alunosSemUsuario = await _unitOfWork.Alunos
                    .FindAsync(a => a.UsuarioId == null);

                var resultados = new List<string>();
                var alunosMigrados = 0;

                foreach (var aluno in alunosSemUsuario)
                {
                    var usuarioExistente = await _unitOfWork.Usuarios
                        .GetByEmailAsync(aluno.Email);

                    if (usuarioExistente != null)
                    {
                        aluno.AssociarUsuario(usuarioExistente.Id);
                        resultados.Add($"Aluno {aluno.Nome} associado ao usuário existente");
                    }
                    else
                    {
                        var novoUsuario = new Usuario(
                            aluno.Nome,
                            aluno.Email,
                            aluno.CPF,
                            "Aluno");

                        await _unitOfWork.Usuarios.AddAsync(novoUsuario);
                        await _unitOfWork.SaveChangesAsync();

                        aluno.AssociarUsuario(novoUsuario.Id);
                        resultados.Add($"Aluno {aluno.Nome} - novo usuário criado");
                    }

                    alunosMigrados++;
                }

                await _unitOfWork.SaveChangesAsync();

                return Ok(new
                {
                    message = $"Migração concluída: {alunosMigrados} alunos processados",
                    detalhes = resultados
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erro na migração",
                    erro = ex.Message
                });
            }
        }
    }
}