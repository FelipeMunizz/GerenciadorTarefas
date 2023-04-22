# Task Master
Task Master é um software de gerenciamento de tarefas que ajuda você a manter suas atividades organizadas e planejadas. Com o Task Master, você pode criar tarefas, definir prazos, atribuir responsabilidades, adicionar comentários e anexos, e acompanhar o status de cada tarefa. Ele também oferece uma visão geral de todos os projetos em andamento, permitindo que você priorize suas atividades e gerencie seu tempo de forma mais eficiente. O Task Master é uma ferramenta essencial para quem busca aumentar a produtividade e melhorar a gestão de tarefas em equipe.



## Autores

- [@FelipeMuniz](https://www.github.com/FelipeMunizz)


## Projeto em Desenvolvimento

O Task Master ainda está em Desenvolvimento


# Documentação da Api

## 1- Autenticação

### Para usuarios não autenticado

### POST /api/Autoriza/Registrar

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

### POST /api/Autoriza/Login
Autentica um usuário no sistema.
#### Body

```json
{
  "Usuario": "string",
  "Senha": "string"
}

``` 
### Respostas
- 201 OK: Usuário autenticado com sucesso. Retorna um token JWT.
- 401 Unauthorized: Usuário ou senha incorretos.

### PUT /api/Autoriza/RedefinirSenha
Redefine a senha de um usuário. Obs.: Gera senha aleatória que será enviada pelo email.

#### Body

```json
{
  "Usuario": "string",
  "Email": "string"
}
``` 
### Respostas
- 201 OK: Senha redefinida com sucesso e enviada por e-mail.
- 400 Bad Request: Falha ao redefinir a senha.
- 401 Unauthorized: Usuário não encontrado.
- 500 Internal Server Error: Falha ao enviar o e-mail com a nova senha.

## 2- Usuarios

### Necessário autenticação

### GET /api/Usuario/ObterUsuario

Obtem usuário no sistema pelo ID

#### Query

```json
{
  "IdUsuario" : "int"
}
``` 
### Respostas
- 201 OK: Usuário
- 400 Bad Request: Falha ao redefinir a senha.

### PUT /api/Usuario/AlterarSenha
Alterar senha do usuário no sistema

#### Body

```json
{
  "Usuario": "string",
  "SenhaAtual": "string",
  "NovaSenha": "string",
  "ConfirmarSenha": "string"
}
``` 
### Respostas
- 201 OK: Senha alterada com sucesso!
- 404 Not Found: As senhas não conferem.
- 404 Not Found: As senhas não condiz com as regas.
- 404 Not Found: Usuario não encontrado.
- 404 Not Found: Não foi possivel atualizar a senha.
- 500 Internal Server Error: Falha ao enviar o e-mail com a nova senha.

### PUT /api/Usuario/AlterarUsuario
Altera usuário no sistema. Obs.: não altera senha do usuário.

#### Body

```json
{
  "IdUsuario" : "int"
  "Nome" : "string"
  "Sobrenome" : "string"
  "Usuario" : "string"
  "Email" : "string"
}
``` 
### Respostas
- 201 OK: Usuario alterado com sucesso.
- 404 Not Found: Usuario não encontrado.
- 404 Not Found: Não foi possivel atualizar as informações do usuario.

### DELETE /api/Usuario/DeletarUsuario

Deletar usuário do sistema

#### Query

```json
{
  "IdUsuario" : "int"
}
``` 
### Respostas
- 201 OK: Usuario excluido com sucesso.
- 404 Not Found: Usuário não encontrado.
- 404 Not Found: Não foi possível remover o usuário.