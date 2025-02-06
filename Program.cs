using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MeuApp
{
    public sealed record Usuario
    {
        public int Id { get; init; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
        public required string Nome { get; set; }

        [Range(0, 150, ErrorMessage = "A idade deve estar entre 0 e 150 anos")]
        public required int Idade { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "O email fornecido não é válido")]
        [StringLength(255, ErrorMessage = "O email não pode ter mais de 255 caracteres")]
        public required string Email { get; set; }

        public DateTime DataCadastro { get; set; }
        public DateTime? UltimaAtualizacao { get; set; }
    }

    public class AppDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=MeuAppDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .Property(u => u.DataCadastro)
                .HasDefaultValueSql("GETDATE()");
        }
    }

    public interface IUsuarioService
    {
        Task<Usuario> AdicionarUsuarioAsync(string nome, int idade, string email);
        Task<IEnumerable<Usuario>> ListarUsuariosAsync();
        Task<Usuario> ObterUsuarioPorIdAsync(int id);
        Task<IEnumerable<Usuario>> BuscarUsuariosPorNomeAsync(string nome);
        Task<bool> EditarUsuarioAsync(int id, string novoNome, int novaIdade, string novoEmail);
        Task<bool> DeletarUsuarioAsync(int id);
        Task<bool> EmailJaExisteAsync(string email);
    }

    public sealed class UsuarioService : IUsuarioService, IDisposable
    {
        private readonly AppDbContext _context;

        public UsuarioService()
        {
            _context = new AppDbContext();
        }

        public async Task<Usuario> AdicionarUsuarioAsync(string nome, int idade, string email)
        {
            ValidarDadosUsuario(nome, idade, email);

            if (await EmailJaExisteAsync(email))
                throw new ArgumentException("Este email já está cadastrado.", nameof(email));

            var usuario = new Usuario
            {
                Nome = nome,
                Idade = idade,
                Email = email,
                DataCadastro = DateTime.Now
            };

            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();

            Console.WriteLine("Usuário adicionado com sucesso!");
            return usuario;
        }

        public async Task<IEnumerable<Usuario>> ListarUsuariosAsync()
        {
            var usuarios = await _context.Usuarios
                .OrderBy(u => u.Nome)
                .ToListAsync();

            if (!usuarios.Any())
            {
                Console.WriteLine("Nenhum usuário cadastrado.");
                return Enumerable.Empty<Usuario>();
            }

            foreach (var usuario in usuarios)
            {
                Console.WriteLine($"ID: {usuario.Id}, Nome: {usuario.Nome}, Idade: {usuario.Idade}, " +
                    $"Email: {usuario.Email}, Cadastro: {usuario.DataCadastro:dd/MM/yyyy}");
            }

            return usuarios;
        }

        public async Task<Usuario> ObterUsuarioPorIdAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado.");

            return usuario;
        }

        public async Task<IEnumerable<Usuario>> BuscarUsuariosPorNomeAsync(string nome)
        {
            return await _context.Usuarios
                .Where(u => u.Nome.Contains(nome))
                .OrderBy(u => u.Nome)
                .ToListAsync();
        }

        public async Task<bool> EditarUsuarioAsync(int id, string novoNome, int novaIdade, string novoEmail)
        {
            ValidarDadosUsuario(novoNome, novaIdade, novoEmail);

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                Console.WriteLine("Usuário não encontrado.");
                return false;
            }

            if (usuario.Email != novoEmail && await EmailJaExisteAsync(novoEmail))
                throw new ArgumentException("Este email já está cadastrado por outro usuário.", nameof(novoEmail));

            usuario.Nome = novoNome;
            usuario.Idade = novaIdade;
            usuario.Email = novoEmail;
            usuario.UltimaAtualizacao = DateTime.Now;

            await _context.SaveChangesAsync();
            Console.WriteLine("Usuário atualizado com sucesso!");
            return true;
        }

        public async Task<bool> DeletarUsuarioAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                Console.WriteLine("Usuário não encontrado.");
                return false;
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            Console.WriteLine("Usuário removido com sucesso!");
            return true;
        }

        public async Task<bool> EmailJaExisteAsync(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.Email == email);
        }

        private static void ValidarDadosUsuario(string nome, int idade, string email)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome não pode estar vazio.", nameof(nome));

            if (nome.Length < 3 || nome.Length > 100)
                throw new ArgumentException("O nome deve ter entre 3 e 100 caracteres.", nameof(nome));

            if (idade < 0 || idade > 150)
                throw new ArgumentException("A idade deve estar entre 0 e 150 anos.", nameof(idade));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("O email não pode estar vazio.", nameof(email));

            if (!System.Text.RegularExpressions.Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("O email fornecido não é válido.", nameof(email));
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

    public sealed class Program
    {
        private static readonly IUsuarioService _usuarioService = new UsuarioService();

        public static async Task Main()
        {
            string opcao;

            do
            {
                ExibirMenu();
                opcao = Console.ReadLine();

                try
                {
                    await ProcessarOpcaoAsync(opcao);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Erro de validação: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro inesperado: {ex.Message}");
                }
            } while (opcao != "7");
        }

        private static void ExibirMenu()
        {
            Console.WriteLine("\n=== Menu ===");
            Console.WriteLine("1 - Adicionar usuário");
            Console.WriteLine("2 - Listar usuários");
            Console.WriteLine("3 - Buscar usuário por ID");
            Console.WriteLine("4 - Buscar usuários por nome");
            Console.WriteLine("5 - Editar usuário");
            Console.WriteLine("6 - Deletar usuário");
            Console.WriteLine("7 - Sair");
            Console.Write("Escolha uma opção: ");
        }

        private static async Task ProcessarOpcaoAsync(string opcao)
        {
            switch (opcao)
            {
                case "1":
                    await AdicionarUsuarioAsync();
                    break;
                case "2":
                    await _usuarioService.ListarUsuariosAsync();
                    break;
                case "3":
                    await BuscarUsuarioPorIdAsync();
                    break;
                case "4":
                    await BuscarUsuariosPorNomeAsync();
                    break;
                case "5":
                    await EditarUsuarioAsync();
                    break;
                case "6":
                    await DeletarUsuarioAsync();
                    break;
                case "7":
                    await FinalizarProgramaAsync();
                    break;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }
        }

        private static async Task AdicionarUsuarioAsync()
        {
            Console.Write("Digite o nome: ");
            string nome = Console.ReadLine();

            Console.Write("Digite a idade: ");
            if (!int.TryParse(Console.ReadLine(), out int idade))
            {
                throw new ArgumentException("Idade inválida.");
            }

            Console.Write("Digite o email: ");
            string email = Console.ReadLine();

            await _usuarioService.AdicionarUsuarioAsync(nome, idade, email);
        }

        private static async Task BuscarUsuarioPorIdAsync()
        {
            Console.Write("Digite o ID do usuário: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                throw new ArgumentException("ID inválido.");
            }

            try
            {
                var usuario = await _usuarioService.ObterUsuarioPorIdAsync(id);
                Console.WriteLine($"ID: {usuario.Id}, Nome: {usuario.Nome}, Idade: {usuario.Idade}, " +
                    $"Email: {usuario.Email}, Cadastro: {usuario.DataCadastro:dd/MM/yyyy}");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task BuscarUsuariosPorNomeAsync()
        {
            Console.Write("Digite o nome para busca: ");
            string nome = Console.ReadLine();

            var usuarios = await _usuarioService.BuscarUsuariosPorNomeAsync(nome);
            if (!usuarios.Any())
            {
                Console.WriteLine("Nenhum usuário encontrado com esse nome.");
                return;
            }

            foreach (var usuario in usuarios)
            {
                Console.WriteLine($"ID: {usuario.Id}, Nome: {usuario.Nome}, Idade: {usuario.Idade}, " +
                    $"Email: {usuario.Email}, Cadastro: {usuario.DataCadastro:dd/MM/yyyy}");
            }
        }

        private static async Task EditarUsuarioAsync()
        {
            Console.Write("Digite o ID do usuário que deseja editar: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                throw new ArgumentException("ID inválido.");
            }

            Console.Write("Novo nome: ");
            string novoNome = Console.ReadLine();

            Console.Write("Nova idade: ");
            if (!int.TryParse(Console.ReadLine(), out int novaIdade))
            {
                throw new ArgumentException("Idade inválida.");
            }

            Console.Write("Novo email: ");
            string novoEmail = Console.ReadLine();

            await _usuarioService.EditarUsuarioAsync(id, novoNome, novaIdade, novoEmail);
        }

        private static async Task DeletarUsuarioAsync()
        {
            Console.Write("Digite o ID do usuário que deseja deletar: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                throw new ArgumentException("ID inválido.");
            }

            await _usuarioService.DeletarUsuarioAsync(id);
        }

        private static async Task FinalizarProgramaAsync()
        {
            Console.WriteLine("\nUsuários cadastrados (final):");
            await _usuarioService.ListarUsuariosAsync();
            Console.WriteLine("Encerrando o programa...");
            
            if (_usuarioService is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
