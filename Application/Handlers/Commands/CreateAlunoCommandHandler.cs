using Application.Commands;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Handlers.Commands
{
    public class CreateAlunoCommandHandler : IRequestHandler<CreateAlunoCommand, AlunoDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateAlunoCommandHandler> _logger;

        public CreateAlunoCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateAlunoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<AlunoDto> Handle(CreateAlunoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Iniciando criação de aluno: Nome={request.Nome}, Email={request.Email}, CPF={request.CPF}, CursoId={request.CursoId}");
            var stopwatch = Stopwatch.StartNew();

            _logger.LogDebug($"Verificando se aluno com CPF {request.CPF} já existe");
            var cpfExistente = await _unitOfWork.Alunos.VerificarCpfExistenteAsync(request.CPF, cancellationToken: cancellationToken);
            if (cpfExistente)
            {
                _logger.LogWarning($"Tentativa de criar aluno com CPF já existente: {request.CPF}");
                throw new InvalidOperationException($"Já existe um aluno com o CPF {request.CPF}");
            }

            _logger.LogDebug($"Verificando se aluno com Email {request.Email} já existe");
            var emailExistente = await _unitOfWork.Alunos.VerificarEmailExistenteAsync(request.Email, cancellationToken: cancellationToken);
            if (emailExistente)
            {
                _logger.LogWarning($"Tentativa de criar aluno com Email já existente: {request.Email}");
                throw new InvalidOperationException($"Já existe um aluno com o email {request.Email}");
            }

            _logger.LogDebug($"Verificando se Curso com ID {request.CursoId} já existe");
            var curso = await _unitOfWork.Cursos.GetByIdAsync(request.CursoId, cancellationToken);
            if (curso == null)
            {
                _logger.LogError($"Curso não encontrado para criação de aluno: CursoId={request.CursoId}");
                throw new InvalidOperationException($"Curso com ID {request.CursoId} não encontrado");
            }

            _logger.LogDebug("Validações passadas. Iniciando transação...");
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                _logger.LogDebug($"Criando usuário para aluno: Email={request.Email}");
                var usuario = new Usuario(request.Nome, request.Email, request.CPF, "Aluno");
                await _unitOfWork.Usuarios.AddAsync(usuario, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogDebug("Gerando registro acadêmico único");
                string registroAcademico = await GerarRegistroAcademicoUnicoAsync(cancellationToken);

                _logger.LogDebug("Criando entidade Aluno: RA={RegistroAcademico}", registroAcademico);
                var novoAluno = new Aluno(
                    request.Nome,
                    request.CPF,
                    registroAcademico,
                    request.Email,
                    request.DataNascimento.ToUniversalTime(),
                    request.CursoId,
                    usuario.Id);

                await _unitOfWork.Alunos.AddAsync(novoAluno, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _unitOfWork.CommitTransactionAsync();
                stopwatch.Stop();

                _logger.LogInformation($"Aluno criado com sucesso: AlunoId={novoAluno.Id}, RA={registroAcademico}, UsuarioId={usuario.Id}, Tempo={stopwatch.ElapsedMilliseconds}ms");
                var alunoCriado = await _unitOfWork.Alunos.GetByIdAsync(novoAluno.Id, cancellationToken);
                return MapToDto(alunoCriado);
            }

            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, $"Erro ao criar aluno: Nome={request.Nome}, Email={request.Email}, CPF={request.CPF}, Tempo={stopwatch.ElapsedMilliseconds}ms. Rollback será executado.");
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }


        private async Task<string> GerarRegistroAcademicoUnicoAsync(CancellationToken cancellationToken)
        {
            string ra;
            int tentativas = 0;

            do
            {
                tentativas++;
                ra = $"RA{DateTime.Now:yyyyMMddHHmmssfff}";
                _logger.LogTrace($"Tentativa {tentativas} de gerar RA único: {ra}");
            }

            while (await _unitOfWork.Alunos.GetByRegistroAcademicoAsync(ra, cancellationToken) != null && tentativas < 5);

            if (tentativas >= 5)
            {
                _logger.LogError($"Falha ao gerar RA único após {tentativas} tentativas");
                throw new InvalidOperationException("Não foi possível gerar um registro acadêmico único");
            }

            _logger.LogDebug($"RA único gerado após {tentativas} tentativas: {ra}");
            return ra;
        }

        private AlunoDto MapToDto(Aluno aluno)
        {
            return new AlunoDto
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                CPF = aluno.CPF,
                RegistroAcademico = aluno.RegistroAcademico,
                Email = aluno.Email,
                DataNascimento = aluno.DataNascimento,
                Ativo = aluno.Ativo,
                CursoId = aluno.CursoId,
            };
        }
    }
}
