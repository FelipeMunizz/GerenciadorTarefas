# Task Master
Task Master é um software de gerenciamento de tarefas que ajuda você a manter suas atividades organizadas e planejadas. Com o Task Master, você pode criar tarefas, definir prazos, atribuir responsabilidades, adicionar comentários e anexos, e acompanhar o status de cada tarefa. Ele também oferece uma visão geral de todos os projetos em andamento, permitindo que você priorize suas atividades e gerencie seu tempo de forma mais eficiente. O Task Master é uma ferramenta essencial para quem busca aumentar a produtividade e melhorar a gestão de tarefas em equipe.



## Autores

- [@FelipeMuniz](https://www.github.com/FelipeMunizz)


## Projeto em Desenvolvimento

O Task Master ainda está em Desenvolvimento


# Documentação da Api

## Autenticação

### POST /api/Usuarios/Registrar

Registra um novo usuário no sistema.

#### Body

```json
{
  "Nome": "string",
  "Sobrenome": "string",
  "Usuario": "string",
  "Senha": "string",
  "Email": "string"
}

``` 
### Respostas
- 200 OK: Usuário criado com sucesso.
- 400 Bad Request: Falha ao criar o usuário.
- 404 Not Found: Usuário já existe no sistema.

### POST /api/Usuarios/Login
Autentica um usuário no sistema.
#### Body

```json
{
  "Usuario": "string",
  "Senha": "string"
}

``` 
### Respostas
- 200 OK: Usuário autenticado com sucesso. Retorna um token JWT.
- 401 Unauthorized: Usuário ou senha incorretos.

### PUT /api/Usuarios/RedefinirSenha
Redefine a senha de um usuário.

#### Body

```json
{
  "Usuario": "string",
  "Email": "string"
}
``` 
### Respostas
- 200 OK: Senha redefinida com sucesso e enviada por e-mail.
- 400 Bad Request: Falha ao redefinir a senha.
- 401 Unauthorized: Usuário não encontrado.
- 500 Internal Server Error: Falha ao enviar o e-mail com a nova senha.
