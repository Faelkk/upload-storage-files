## 📁 Upload Storage Files

O Upload Storage Files é um projeto pessoal desenvolvido para controle e gerenciamento de arquivos, com suporte a upload, armazenamento local ou em nuvem (Cloudinary) e exclusão de arquivos.
Conta com validações para tamanho e tipo de arquivo, sendo ideal para aplicações que necessitam de um sistema robusto e flexível de gerenciamento de arquivos.

## 🚀 Funcionalidades

Upload de arquivos (com validação de tipo e tamanho)

- ☁️ Armazenamento local ou em nuvem (Cloudinary)

- ❌ Exclusão de arquivos enviados

- 🔒 Validação e segurança via variáveis de ambiente

- 🐳 Pronto para execução com Docker

## 🛠️ Tecnologias Utilizadas

- [.NET](https://dotnet.microsoft.com/pt-br/)
- [ASP.NET](https://learn.microsoft.com/pt-br/aspnet/core/?view=aspnetcore-9.0&WT.mc_id=dotnet-35129-website)
- [CloudinaryDotNet](https://cloudinary.com/documentation/dotnet_integration)

🔋 **Controle de versão e deploy**

- [Git](https://git-scm.com)

⚙️ **Como Rodar o Projeto**

Para rodar o projeto em seu ambiente local, siga os passos abaixo:

Primeiramente, clone o repositório do GitHub para sua máquina local:

    $ git clone https://github.com/Faelkk/micro-service-auth.git

Acesse o diretório do projeto e instale as dependências:

        $ dotnet restore

Configurar as variaveis de ambientes

```json
"Cloudinary": {
"CloudName": "SEU_CLOUD_NAME",
"ApiKey": "SUA_API_KEY",
"ApiSecret": "SEU_API_SECRET"
}
```

Finalmente, inicie o projeto rodando

        $ dotnet run

<br>

**🤝 Como Contribuir?**

- ⭐ Dando uma estrela no repositório

- 🧑‍💻 Me seguindo aqui no GitHub

- 🤝 Conectando-se comigo no LinkedIn

<br>

**👨‍💻 Autor**

Desenvolvido com 💙 por<br>
[Rafael Achtenberg](linkedin.com/in/rafael-achtenberg-7a4b12284/)
