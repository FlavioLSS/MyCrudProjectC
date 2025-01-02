using System;
using System.Collections.Generic;

// Um programa simples de CRUD (Criar, Listar, Editar, Deletar)
class Program
{
    // Lista para armazenar os usuários
    static List<Usuario> usuarios = new List<Usuario>();

    // Contador para gerar IDs automaticamente
    static int idCounter = 1;

    static void Main(string[] args)
    {
        // Menu principal do programa
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

            // Chamando a função correta com base na opção escolhida
            switch (opcao)
            {
                case "1": AdicionarUsuario(); break;
                case "2": ListarUsuarios(); break;
                case "3": EditarUsuario(); break;
                case "4": DeletarUsuario(); break;
                case "5":
                    Console.WriteLine("\nUsuários cadastrados (final):");
                    ListarUsuarios(); // Mostra os dados finais ao sair
                    Console.WriteLine("Encerrando o programa...");
                    break;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }
        } while (opcao != "5"); // O programa só para quando a opção "5" for escolhida
    }

    // Função para adicionar um novo usuário
    static void AdicionarUsuario()
    {
        Console.Write("Digite o nome: ");
        string nome = Console.ReadLine(); // Recebendo o nome

        Console.Write("Digite a idade: ");
        int idade = int.Parse(Console.ReadLine()); // Recebendo a idade

        Console.Write("Digite o email: ");
        string email = Console.ReadLine(); // Recebendo o email

        // Adicionando um novo usuário na lista
        usuarios.Add(new Usuario { ID = idCounter++, Nome = nome, Idade = idade, Email = email });

        Console.WriteLine("Usuário adicionado com sucesso!");
    }

    // Função para listar todos os usuários cadastrados
    static void ListarUsuarios()
    {
        if (usuarios.Count == 0)
        {
            Console.WriteLine("Nenhum usuário cadastrado.");
            return;
        }

        // Percorrendo a lista e exibindo os dados de cada usuário
        foreach (var user in usuarios)
        {
            Console.WriteLine($"ID: {user.ID}, Nome: {user.Nome}, Idade: {user.Idade}, Email: {user.Email}");
        }
    }

    // Função para editar os dados de um usuário
    static void EditarUsuario()
    {
        Console.Write("Digite o ID do usuário que deseja editar: ");
        int id = int.Parse(Console.ReadLine()); // Pegando o ID do usuário
        var user = usuarios.Find(u => u.ID == id); // Buscando o usuário na lista

        if (user != null)
        {
            // Atualizando os dados
            Console.Write("Novo nome: ");
            user.Nome = Console.ReadLine();

            Console.Write("Nova idade: ");
            user.Idade = int.Parse(Console.ReadLine());

            Console.Write("Novo email: ");
            user.Email = Console.ReadLine();

            Console.WriteLine("Usuário atualizado com sucesso!");
        }
        else
        {
            Console.WriteLine("Usuário não encontrado.");
        }
    }

    // Função para deletar um usuário
    static void DeletarUsuario()
    {
        Console.Write("Digite o ID do usuário que deseja deletar: ");
        int id = int.Parse(Console.ReadLine()); // Pegando o ID do usuário
        var user = usuarios.Find(u => u.ID == id); // Buscando o usuário na lista

        if (user != null)
        {
            // Removendo o usuário da lista
            usuarios.Remove(user);
            Console.WriteLine("Usuário removido com sucesso!");
        }
        else
        {
            Console.WriteLine("Usuário não encontrado.");
        }
    }

    // Classe para representar os dados de um usuário
    class Usuario
    {
        public int ID { get; set; } // Identificador único do usuário
        public string Nome { get; set; } // Nome do usuário
        public int Idade { get; set; } // Idade do usuário
        public string Email { get; set; } // Email do usuário
    }
}