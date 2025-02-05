using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MeuApp
{
    /// <summary>
    /// Representa os dados de um usuário no sistema
    /// </summary>
    public sealed record Usuario
    {
        /// <summary>
        /// Identificador único do usuário
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Nome completo do usuário
        /// </summary>
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
        public required string Nome { get; set; }

        /// <summary>
        /// Idade do usuário
        /// </summary>
        [Range(0, 150, ErrorMessage = "A idade deve estar entre 0 e 150 anos")]
        public required int Idade { get; set; }

        /// <summary>
        /// Endereço de email do usuário
        /// </summary>
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "O email fornecido não é válido")]
        [StringLength(255, ErrorMessage = "O email não pode ter mais de 255 caracteres")]
        public required string Email { get; set; }
    }

    /// <summary>
    /// Interface que define as operações disponíveis para gerenciamento de usuários
    /// </summary>
    public interface IUsuarioService
    {
        /// <summary>
        /// Adiciona um novo usuário ao sistema
        /// </summary>
        /// <param name="nome">Nome do usuário</param>
        /// <param name="idade">Idade do usuário</param>
        /// <param name="email">Email do usuário</param>
        /// <exception cref="ArgumentException">Lançada quando os dados do usuário são inválidos</exception>
        void AdicionarUsuario(string nome, int idade, string email);

        /// <summary>
        /// Lista todos os usuários cadastrados no sistema
        /// </summary>
        void ListarUsuarios();

        /// <summary>
        /// Edita os dados de um usuário existente
        /// </summary>
        /// <param name="id">ID do usuário a ser editado</param>
        /// <param name="novoNome">Novo nome do usuário</param>
        /// <param name="novaIdade">Nova idade do usuário</param>
        /// <param name="novoEmail">Novo email do usuário</param>
        /// <returns>true se o usuário foi encontrado e atualizado, false caso contrário</returns>
        /// <exception cref="ArgumentException">Lançada quando os novos dados são inválidos</exception>
        bool EditarUsuario(int id, string novoNome, int novaIdade, string novoEmail);

        /// <summary>
        /// Remove um usuário do sistema
        /// </summary>
        /// <param name="id">ID do usuário a ser removido</param>
        /// <returns>true se o usuário foi encontrado e removido, false caso contrário</returns>
        bool DeletarUsuario(int id);
    }

    /// <summary>
    /// Implementação do serviço de gerenciamento de usuários
    /// </summary>
    public sealed class UsuarioService : IUsuarioService
    {
        private readonly Dictionary<int, Usuario> _usuarios = new();
        private int _idCounter = 1;

        /// <inheritdoc/>
        public void AdicionarUsuario(string nome, int idade, string email)
        {
            ValidarDadosUsuario(nome, idade, email);

            var usuario = new Usuario
            {
                Id = _idCounter++,
                Nome = nome,
                Idade = idade,
                Email = email
            };

            _usuarios.Add(usuario.Id, usuario);
            Console.WriteLine("Usuário adicionado com sucesso!");
        }

        /// <inheritdoc/>
        public void ListarUsuarios()
        {
            if (_usuarios.Count == 0)
            {
                Console.WriteLine("Nenhum usuário cadastrado.");
                return;
            }

            foreach (var usuario in _usuarios.Values)
            {
                Console.WriteLine($"ID: {usuario.Id}, Nome: {usuario.Nome}, Idade: {usuario.Idade}, Email: {usuario.Email}");
            }
        }

        /// <inheritdoc/>
        public bool EditarUsuario(int id, string novoNome, int novaIdade, string novoEmail)
        {
            ValidarDadosUsuario(novoNome, novaIdade, novoEmail);

            if (_usuarios.TryGetValue(id, out var usuario))
            {
                usuario.Nome = novoNome;
                usuario.Idade = novaIdade;
                usuario.Email = novoEmail;
                Console.WriteLine("Usuário atualizado com sucesso!");
                return true;
            }

            Console.WriteLine("Usuário não encontrado.");
            return false;
        }

        /// <inheritdoc/>
        public bool DeletarUsuario(int id)
        {
            if (_usuarios.Remove(id))
            {
                Console.WriteLine("Usuário removido com sucesso!");
                return true;
            }

            Console.WriteLine("Usuário não encontrado.");
            return false;
        }

        /// <summary>
        /// Valida os dados do usuário antes de adicionar ou atualizar
        /// </summary>
        /// <exception cref="ArgumentException">Lançada quando os dados são inválidos</exception>
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
    }

    /// <summary>
    /// Classe principal do programa que gerencia o menu e interação com usuário
    /// </summary>
    public sealed class Program
    {
        private static readonly IUsuarioService _usuarioService = new UsuarioService();

        /// <summary>
        /// Ponto de entrada principal do programa
        /// </summary>
        public static void Main()
        {
            string opcao;

            do
            {
                ExibirMenu();
                opcao = Console.ReadLine();

                try
                {
                    ProcessarOpcao(opcao);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Erro de validação: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro inesperado: {ex.Message}");
                }
            } while (opcao != "5");
        }

        /// <summary>
        /// Exibe o menu principal do programa
        /// </summary>
        private static void ExibirMenu()
        {
            Console.WriteLine("\n=== Menu ===");
            Console.WriteLine("1 - Adicionar usuário");
            Console.WriteLine("2 - Listar usuários");
            Console.WriteLine("3 - Editar usuário");
            Console.WriteLine("4 - Deletar usuário");
            Console.WriteLine("5 - Sair");
            Console.Write("Escolha uma opção: ");
        }

        /// <summary>
        /// Processa a opção escolhida pelo usuário
        /// </summary>
        /// <param name="opcao">Opção selecionada no menu</param>
        private static void ProcessarOpcao(string opcao)
        {
            switch (opcao)
            {
                case "1":
                    AdicionarUsuario();
                    break;
                case "2":
                    _usuarioService.ListarUsuarios();
                    break;
                case "3":
                    EditarUsuario();
                    break;
                case "4":
                    DeletarUsuario();
                    break;
                case "5":
                    FinalizarPrograma();
                    break;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }
        }

        /// <summary>
        /// Processa a adição de um novo usuário
        /// </summary>
        private static void AdicionarUsuario()
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

            _usuarioService.AdicionarUsuario(nome, idade, email);
        }

        /// <summary>
        /// Processa a edição de um usuário existente
        /// </summary>
        private static void EditarUsuario()
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

            _usuarioService.EditarUsuario(id, novoNome, novaIdade, novoEmail);
        }

        /// <summary>
        /// Processa a remoção de um usuário
        /// </summary>
        private static void DeletarUsuario()
        {
            Console.Write("Digite o ID do usuário que deseja deletar: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                throw new ArgumentException("ID inválido.");
            }

            _usuarioService.DeletarUsuario(id);
        }

        /// <summary>
        /// Finaliza o programa exibindo a lista final de usuários
        /// </summary>
        private static void FinalizarPrograma()
        {
            Console.WriteLine("\nUsuários cadastrados (final):");
            _usuarioService.ListarUsuarios();
            Console.WriteLine("Encerrando o programa...");
        }
    }
}
