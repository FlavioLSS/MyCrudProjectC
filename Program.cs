using System;
using System.Collections.Generic;

namespace MeuApp
{
    public record Usuario
    {
        public int Id { get; init; }
        public required string Nome { get; set; }
        public required int Idade { get; set; }
        public required string Email { get; set; }
    }

    public interface IUsuarioService
    {
        void AdicionarUsuario(string nome, int idade, string email);
        void ListarUsuarios();
        bool EditarUsuario(int id, string novoNome, int novaIdade, string novoEmail);
        bool DeletarUsuario(int id);
    }

    public class UsuarioService : IUsuarioService
    {
        private readonly Dictionary<int, Usuario> _usuarios = new();
        private int _idCounter = 1;

        public void AdicionarUsuario(string nome, int idade, string email)
        {
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

        public void ListarUsuarios()
        {
            if (_usuarios.Count == 0)
            {
                Console.WriteLine("Nenhum usuário cadastrado.");
                return;
            }

            foreach (var user in _usuarios.Values)
            {
                Console.WriteLine($"ID: {user.Id}, Nome: {user.Nome}, Idade: {user.Idade}, Email: {user.Email}");
            }
        }

        public bool EditarUsuario(int id, string novoNome, int novaIdade, string novoEmail)
        {
            if (_usuarios.TryGetValue(id, out var user))
            {
                user.Nome = novoNome;
                user.Idade = novaIdade;
                user.Email = novoEmail;
                Console.WriteLine("Usuário atualizado com sucesso!");
                return true;
            }
            Console.WriteLine("Usuário não encontrado.");
            return false;
        }

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
    }

    class Program
    {
        static void Main()
        {
            IUsuarioService usuarioService = new UsuarioService();
            string opcao;

            do
            {
                Console.WriteLine("\n=== Menu ===");
                Console.WriteLine("1 - Adicionar usuário");
                Console.WriteLine("2 - Listar usuários");
                Console.WriteLine("3 - Editar usuário");
                Console.WriteLine("4 - Deletar usuário");
                Console.WriteLine("5 - Sair");
                Console.Write("Escolha uma opção: ");
                opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        AdicionarUsuario(usuarioService);
                        break;
                    case "2":
                        usuarioService.ListarUsuarios();
                        break;
                    case "3":
                        EditarUsuario(usuarioService);
                        break;
                    case "4":
                        DeletarUsuario(usuarioService);
                        break;
                    case "5":
                        Console.WriteLine("\nUsuários cadastrados (final):");
                        usuarioService.ListarUsuarios();
                        Console.WriteLine("Encerrando o programa...");
                        break;
                    default:
                        Console.WriteLine("Opção inválida. Tente novamente.");
                        break;
                }
            } while (opcao != "5");
        }

        static void AdicionarUsuario(IUsuarioService usuarioService)
        {
            Console.Write("Digite o nome: ");
            string nome = Console.ReadLine();

            Console.Write("Digite a idade: ");
            if (int.TryParse(Console.ReadLine(), out int idade))
            {
                Console.Write("Digite o email: ");
                string email = Console.ReadLine();
                usuarioService.AdicionarUsuario(nome, idade, email);
            }
            else
            {
                Console.WriteLine("Idade inválida. Usuário não adicionado.");
            }
        }

        static void EditarUsuario(IUsuarioService usuarioService)
        {
            Console.Write("Digite o ID do usuário que deseja editar: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Console.Write("Novo nome: ");
                string novoNome = Console.ReadLine();

                Console.Write("Nova idade: ");
                if (int.TryParse(Console.ReadLine(), out int novaIdade))
                {
                    Console.Write("Novo email: ");
                    string novoEmail = Console.ReadLine();
                    usuarioService.EditarUsuario(id, novoNome, novaIdade, novoEmail);
                }
                else
                {
                    Console.WriteLine("Idade inválida. Edição cancelada.");
                }
            }
            else
            {
                Console.WriteLine("ID inválido. Edição cancelada.");
            }
        }

        static void DeletarUsuario(IUsuarioService usuarioService)
        {
            Console.Write("Digite o ID do usuário que deseja deletar: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                usuarioService.DeletarUsuario(id);
            }
            else
            {
                Console.WriteLine("ID inválido. Exclusão cancelada.");
            }
        }
    }
}
