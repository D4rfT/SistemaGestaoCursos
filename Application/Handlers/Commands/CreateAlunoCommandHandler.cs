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
            _logger.LogInformation("Iniciando criação de aluno: Nome={Nome}, Email={Email}, CPF={CPF}, CursoId={CursoId}", request.Nome, request.Email, request.CPF, request.CursoId);
            var stopwatch = Stopwatch.StartNew();

            _logger.LogDebug("Verificando se aluno com CPF {CPF} já existe", request.CPF);
            var cpfExistente = await _unitOfWork.Alunos.VerificarCpfExistenteAsync(request.CPF, cancellationToken: cancellationToken);
            if (cpfExistente)
            {
                _logger.LogWarning("Tentativa de criar aluno com CPF já existente: {CPF}", request.CPF);
                throw new InvalidOperationException($"Já existe um aluno com o CPF {request.CPF}");
            }

            _logger.LogDebug("Verificando se aluno com Email {email} já existe", request.Email);
            var emailExistente = await _unitOfWork.Alunos.VerificarEmailExistenteAsync(request.Email, cancellationToken: cancellationToken);
            if (emailExistente)
            {
                _logger.LogWarning("Tentativa de criar aluno com Email já existente: {Email}", request.Email);
                throw new InvalidOperationException($"Já existe um aluno com o email {request.Email}");
            }

            _logger.LogDebug("Verificando se Curso com ID {CursoId} já existe", request.CursoId);
            var curso = await _unitOfWork.Cursos.GetByIdAsync(request.CursoId, cancellationToken);
            if (curso == null)
            {
                _logger.LogError("Curso não encontrado para criação de aluno: CursoId={CursoId}", request.CursoId);
                throw new InvalidOperationException($"Curso com ID {request.CursoId} não encontrado");
            }

            _logger.LogDebug("Validações passadas. Iniciando transação...");
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                _logger.LogDebug("Criando usuário para aluno: Email={Email}", request.Email);
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

                _logger.LogInformation("Aluno criado com sucesso: AlunoId={AlunoId}, RA={RegistroAcademico}, UsuarioId={UsuarioId}, Tempo={Tempo}ms", novoAluno.Id, registroAcademico, usuario.Id, stopwatch.ElapsedMilliseconds);
                var alunoCriado = await _unitOfWork.Alunos.GetByIdAsync(novoAluno.Id, cancellationToken);
                return MapToDto(alunoCriado);
            }

            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Erro ao criar aluno: Nome={Nome}, Email={Email}, CPF={CPF}, Tempo={Tempo}ms. Rollback será executado.", request.Nome, request.Email, request.CPF, stopwatch.ElapsedMilliseconds);
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
                _logger.LogTrace("Tentativa {Tentativa} de gerar RA único: {RA}", tentativas, ra);
            }

            while (await _unitOfWork.Alunos.GetByRegistroAcademicoAsync(ra, cancellationToken) != null && tentativas < 5);

            if (tentativas >= 5)
            {
                _logger.LogError("Falha ao gerar RA único após {Tentativas} tentativas", tentativas);
                throw new InvalidOperationException("Não foi possível gerar um registro acadêmico único");
            }

            _logger.LogDebug("RA único gerado após {Tentativas} tentativas: {RA}", tentativas, ra);
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
