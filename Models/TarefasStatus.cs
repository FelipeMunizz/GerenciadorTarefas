using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.NetworkInformation;
using System.Security.Principal;

namespace WebApi.Models;

public class TarefasStatus
{
    public int IdTarefa { get; set; }
    public int IdStatus { get; set; }
    public DateTime DataAlteracao { get; set; }
    public virtual Tarefas Tarefas { get; set; }
    public virtual Status Status { get; set; }
}

CREATE TABLE STATUS(
  ID_STATUS INT PRIMARY KEY IDENTITY(1,1),
  NOME_STATUS VARCHAR(50) NOT NULL
);

CREATE TABLE TAREFAS_STATUS(
  ID_TAREFAS INT NOT NULL,
  ID_STATUS INT NOT NULL,
DataAlteracao DATETIME NOT NULL,
  FOREIGN KEY (ID_TAREFAS) REFERENCES TAREFAS(ID_TAREFAS),
  FOREIGN KEY (ID_STATUS) REFERENCES STATUS(ID_STATUS)
);