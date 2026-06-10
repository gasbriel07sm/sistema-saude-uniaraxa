
public static class Consultas
{
    public static void Menu()
    {
        int opcao;
        do
        {
            Console.Clear();
            UI.ExibirCabecalho("GERENCIAMENTO DE CONSULTAS  [MATRIZ]");
            Console.WriteLine($"  Consultas registradas: {Dados.TotalConsultas} / {Dados.MAX_CONSULTAS}");
            Console.WriteLine();
            Console.WriteLine("  [1] Registrar Nova Consulta");
            Console.WriteLine("  [2] Listar Todas as Consultas");
            Console.WriteLine("  [3] Buscar Consultas por CPF do Paciente");
            Console.WriteLine("  [4] Buscar Consultas por Medico");
            Console.WriteLine("  [5] Buscar Consultas por Data");
            Console.WriteLine("  [0] Voltar ao Menu Principal");
            UI.ExibirRodape();
            opcao = UI.LerInteiro("Opcao");

            switch (opcao)
            {
                case 1: Registrar(); break;
                case 2: Listar(); break;
                case 3: BuscarPorCpf(); break;
                case 4: BuscarPorMedico(); break;
                case 5: BuscarPorData(); break;
                case 0: break;
                default: UI.MensagemAviso("Opcao invalida!"); break;
            }

        } while (opcao != 0);
    }

    static void Registrar()
    {
        Console.Clear();
        UI.ExibirCabecalho("REGISTRAR NOVA CONSULTA  [MATRIZ]");

        if (Dados.TotalConsultas >= Dados.MAX_CONSULTAS)
        {
            UI.MensagemAviso("ERRO: Capacidade maxima de consultas atingida (" + Dados.MAX_CONSULTAS + ").");
            return;
        }

        string cpf = UI.LerTexto("CPF do paciente");
        int idx = Pacientes.BuscarIndice(cpf);

        if (idx == -1)
        {
            UI.MensagemAviso("Paciente nao encontrado. Cadastre-o primeiro.");
            return;
        }

        Console.WriteLine($"\n  Paciente: {Dados.PacNomes[idx]} | Tipo Sang.: {Dados.PacTipoSang[idx]}\n");

        string data = UI.LerTexto("Data da consulta (DD/MM/AAAA)");
        string medico = UI.LerTexto("Nome do medico (ex: Dr. Carlos Silva)");
        string especialid = UI.LerTexto("Especialidade (ex: Cardiologia, Clinico Geral)");
        string diagnostico = UI.LerTexto("Diagnostico / Observacoes");

        int linha = Dados.TotalConsultas;
        Dados.MatrizConsultas[linha, Dados.COL_CPF_PAC] = cpf;
        Dados.MatrizConsultas[linha, Dados.COL_NOME_PAC] = Dados.PacNomes[idx];
        Dados.MatrizConsultas[linha, Dados.COL_DATA] = data;
        Dados.MatrizConsultas[linha, Dados.COL_MEDICO] = medico;
        Dados.MatrizConsultas[linha, Dados.COL_ESPECIALID] = especialid;
        Dados.MatrizConsultas[linha, Dados.COL_DIAGNOSTICO] = diagnostico;
        Dados.TotalConsultas++;

        Historico.Registrar($"CONSULTA: '{Dados.PacNomes[idx]}' com {medico} em {data} registrada na matriz[{linha}]");
        UI.MensagemSucesso($"Consulta registrada na linha {Dados.TotalConsultas} da matriz!");
    }

   static void Listar()
    {
        Console.Clear();
        UI.ExibirCabecalho("LISTA DE CONSULTAS  [MATRIZ]");

        if (Dados.TotalConsultas == 0)
        {
            Console.WriteLine("  Nenhuma consulta registrada.\n");
            Console.Write("  Pressione qualquer tecla...");
            Console.ReadKey();
            return;
        }

        for (int i = 0; i < Dados.TotalConsultas; i++)
        {
            Console.WriteLine($"  ---- Consulta #{i + 1} (linha {i} da matriz) ----");
            Console.WriteLine($"    CPF Paciente  : {Dados.MatrizConsultas[i, Dados.COL_CPF_PAC]}");
            Console.WriteLine($"    Nome          : {Dados.MatrizConsultas[i, Dados.COL_NOME_PAC]}");
            Console.WriteLine($"    Data          : {Dados.MatrizConsultas[i, Dados.COL_DATA]}");
            Console.WriteLine($"    Medico        : {Dados.MatrizConsultas[i, Dados.COL_MEDICO]}");
            Console.WriteLine($"    Especialidade : {Dados.MatrizConsultas[i, Dados.COL_ESPECIALID]}");
            Console.WriteLine($"    Diagnostico   : {Dados.MatrizConsultas[i, Dados.COL_DIAGNOSTICO]}");
            Console.WriteLine();
        }

        Console.WriteLine($"  Total: {Dados.TotalConsultas} consulta(s) registrada(s).");
        Console.Write("\n  Pressione qualquer tecla...");
        Console.ReadKey();
    }

   static void BuscarPorCpf()
    {
        Console.Clear();
        UI.ExibirCabecalho("BUSCAR CONSULTAS POR CPF");

        string cpf = UI.LerTexto("CPF do paciente");
        bool encontrou = false;

        Console.WriteLine($"\n  Consultas para CPF: {cpf}\n");

        for (int i = 0; i < Dados.TotalConsultas; i++)
        {
            if (Dados.MatrizConsultas[i, Dados.COL_CPF_PAC] == cpf)
            {
                Console.WriteLine($"  Consulta #{i + 1}  |  Data: {Dados.MatrizConsultas[i, Dados.COL_DATA]}");
                Console.WriteLine($"    Medico        : {Dados.MatrizConsultas[i, Dados.COL_MEDICO]}");
                Console.WriteLine($"    Especialidade : {Dados.MatrizConsultas[i, Dados.COL_ESPECIALID]}");
                Console.WriteLine($"    Diagnostico   : {Dados.MatrizConsultas[i, Dados.COL_DIAGNOSTICO]}");
                Console.WriteLine();
                encontrou = true;
            }
        }

        if (!encontrou)
            Console.WriteLine("  Nenhuma consulta encontrada para este CPF.");

        Console.Write("\n  Pressione qualquer tecla...");
        Console.ReadKey();
    }

   static void BuscarPorMedico()
    {
        Console.Clear();
        UI.ExibirCabecalho("BUSCAR CONSULTAS POR MEDICO");

        string nomeMedico = UI.LerTexto("Nome do medico (ou parte do nome)");
        bool encontrou = false;

        Console.WriteLine($"\n  Consultas de: {nomeMedico}\n");

        for (int i = 0; i < Dados.TotalConsultas; i++)
        {
            if (Dados.MatrizConsultas[i, Dados.COL_MEDICO]
                    .IndexOf(nomeMedico, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Console.WriteLine($"  Consulta #{i + 1}  |  Data: {Dados.MatrizConsultas[i, Dados.COL_DATA]}");
                Console.WriteLine($"    Paciente      : {Dados.MatrizConsultas[i, Dados.COL_NOME_PAC]} (CPF: {Dados.MatrizConsultas[i, Dados.COL_CPF_PAC]})");
                Console.WriteLine($"    Especialidade : {Dados.MatrizConsultas[i, Dados.COL_ESPECIALID]}");
                Console.WriteLine($"    Diagnostico   : {Dados.MatrizConsultas[i, Dados.COL_DIAGNOSTICO]}");
                Console.WriteLine();
                encontrou = true;
            }
        }

        if (!encontrou)
            Console.WriteLine("  Nenhuma consulta encontrada para este medico.");

        Console.Write("\n  Pressione qualquer tecla...");
        Console.ReadKey();
    }

    static void BuscarPorData()
    {
        Console.Clear();
        UI.ExibirCabecalho("BUSCAR CONSULTAS POR DATA");

        string data = UI.LerTexto("Data (DD/MM/AAAA)");
        bool encontrou = false;

        Console.WriteLine($"\n  Consultas em {data}:\n");

        for (int i = 0; i < Dados.TotalConsultas; i++)
        {
            if (Dados.MatrizConsultas[i, Dados.COL_DATA] == data)
            {
                Console.WriteLine($"  Consulta #{i + 1}");
                Console.WriteLine($"    Paciente      : {Dados.MatrizConsultas[i, Dados.COL_NOME_PAC]} (CPF: {Dados.MatrizConsultas[i, Dados.COL_CPF_PAC]})");
                Console.WriteLine($"    Medico        : {Dados.MatrizConsultas[i, Dados.COL_MEDICO]}");
                Console.WriteLine($"    Especialidade : {Dados.MatrizConsultas[i, Dados.COL_ESPECIALID]}");
                Console.WriteLine($"    Diagnostico   : {Dados.MatrizConsultas[i, Dados.COL_DIAGNOSTICO]}");
                Console.WriteLine();
                encontrou = true;
            }
        }

        if (!encontrou)
            Console.WriteLine("  Nenhuma consulta encontrada nesta data.");

        Console.Write("\n  Pressione qualquer tecla...");
        Console.ReadKey();
    }

   public static int ContarPorCpf(string cpf)
    {
        int contador = 0;
        for (int i = 0; i < Dados.TotalConsultas; i++)
        {
            if (Dados.MatrizConsultas[i, Dados.COL_CPF_PAC] == cpf)
                contador++;
        }
        return contador;
    }
}