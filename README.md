# Sistema de Gerenciamento de Usuários

Um sistema de console em C# para gerenciamento de usuários com operações CRUD (Create, Read, Update, Delete), desenvolvido com boas práticas e arquitetura limpa.

## 🎯 Funcionalidades

- Adicionar novos usuários
- Listar todos os usuários
- Editar usuários existentes
- Deletar usuários
- Validações robustas de dados
- Interface de linha de comando intuitiva

## 📋 Requisitos

- .NET 7.0 ou superior
- Visual Studio 2022 ou VS Code
- Sistema operacional: Windows, Linux ou macOS

## 🚀 Como Executar

1. Clone o repositório:
```bash
git clone https://github.com/seu-usuario/gerenciamento-usuarios.git
```

2. Navegue até a pasta do projeto:
```bash
cd gerenciamento-usuarios
```

3. Compile o projeto:
```bash
dotnet build
```

4. Execute o programa:
```bash
dotnet run
```

## 💻 Como Usar

O programa apresenta um menu interativo com as seguintes opções:

1. **Adicionar usuário**
   - Digite o nome do usuário
   - Digite a idade
   - Digite o email

2. **Listar usuários**
   - Exibe todos os usuários cadastrados

3. **Editar usuário**
   - Digite o ID do usuário
   - Forneça os novos dados

4. **Deletar usuário**
   - Digite o ID do usuário a ser removido

5. **Sair**
   - Finaliza o programa

## 🏗️ Estrutura do Projeto

```
MeuApp/
├── Models/
│   └── Usuario.cs
├── Services/
│   ├── IUsuarioService.cs
│   └── UsuarioService.cs
├── Program.cs
└── README.md
```

## 📊 Modelo de Dados

### Usuario
```csharp
public record Usuario
{
    public int Id { get; init; }
    public required string Nome { get; set; }
    public required int Idade { get; set; }
    public required string Email { get; set; }
}
```

## ✅ Validações

O sistema inclui as seguintes validações:

### Nome
- Obrigatório
- Entre 3 e 100 caracteres
- Não pode ser vazio ou apenas espaços

### Idade
- Deve ser um número positivo
- Máximo de 150 anos
- Deve ser um número válido

### Email
- Formato válido de email
- Obrigatório
- Máximo de 255 caracteres

## 🛠️ Tecnologias Utilizadas

- C# 11
- .NET 7
- System.Collections.Generic
- System.ComponentModel.DataAnnotations

## 🔒 Boas Práticas Implementadas

1. **SOLID Principles**
   - Interfaces bem definidas
   - Responsabilidade única
   - Inversão de dependência

2. **Clean Code**
   - Nomes descritivos
   - Métodos pequenos e focados
   - Comentários significativos

3. **Tratamento de Erros**
   - Validações robustas
   - Mensagens de erro claras
   - Try-catch apropriado

4. **Documentação**
   - XML Comments
   - README detalhado
   - Exemplos de uso

## 🤝 Contribuindo

1. Fork o projeto
2. Crie sua branch: `git checkout -b feature/nova-funcionalidade`
3. Commit suas mudanças: `git commit -m 'Adiciona nova funcionalidade'`
4. Push para a branch: `git push origin feature/nova-funcionalidade`
5. Abra um Pull Request

## 🐛 Reportando Problemas

Use a seção de Issues do GitHub para reportar problemas, seguindo o template:

- Descrição do problema
- Passos para reproduzir
- Comportamento esperado
- Comportamento atual
- Ambiente (SO, versão .NET, etc.)

## 📈 Próximos Passos

- [ ] Persistência de dados
- [ ] Interface gráfica
- [ ] Autenticação de usuários
- [ ] Logs de operações
- [ ] Exportação de dados
- [ ] Testes unitários
