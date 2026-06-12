public static class Pacientes
{
    public static void Menu()
    {
        int opcao;
        do
        {
            Console.Clear();
            UI.ExibirCabecalho("GERENCIAMENTO DE PACIENTES  [VETOR]");
            Console.WriteLine($"  Pacientes cadastrados: {Dados.TotalPacientes} / {Dados.MAX_PACIENTES}");
            Console.WriteLine();
            Console.WriteLine("  [1] Cadastrar Paciente");
            Console.WriteLine("  [2] Listar Todos os Pacientes");
            Console.WriteLine("  [3] Buscar Paciente por CPF");
            Console.WriteLine("  [4] Remover Paciente");
            Console.WriteLine("  [5] Ordenar pacientes por ordem Alfabética");
            Console.WriteLine("  [0] Voltar ao Menu Principal");
            UI.ExibirRodape();
            opcao = UI.LerInteiro("Opcao");

            switch (opcao)
            {
                case 1: Cadastrar(); break;
                case 2: Listar(); break;
                case 3: BuscarPorCpf(); break;
                case 4: Remover(); break;
                case 5: OrdenarPorNome(); break;
                case 0: break;
                default: UI.MensagemAviso("Opcao invalida!"); break;
            }

        } while (opcao != 0);
    }

   static void Cadastrar()
    {
        Console.Clear();
        UI.ExibirCabecalho("CADASTRAR PACIENTE");

        if (Dados.TotalPacientes >= Dados.MAX_PACIENTES)
        {
            UI.MensagemAviso("ERRO: Capacidade maxima atingida (" + Dados.MAX_PACIENTES + " pacientes).");
            return;
        }

        Console.WriteLine("  Preencha os dados do paciente:\n");

        string cpf = UI.LerTexto("CPF (somente numeros)");

        if (BuscarIndice(cpf) != -1)
        {
            UI.MensagemAviso("ERRO: Este CPF ja esta cadastrado no sistema!");
            return;
        }

        string nome = UI.LerTexto("Nome completo");
        int idade = UI.LerInteiro("Idade");
        string telefone = UI.LerTexto("Telefone");
        string tipoSang = UI.LerTexto("Tipo Sanguineo (ex: A+, O-, AB-)");

        int pos = Dados.TotalPacientes;
        Dados.PacCpfs[pos] = cpf;
        Dados.PacNomes[pos] = nome;
        Dados.PacIdades[pos] = idade;
        Dados.PacTelefones[pos] = telefone;
        Dados.PacTipoSang[pos] = tipoSang;
        Dados.TotalPacientes++;

        Historico.Registrar($"CADASTRO: Paciente '{nome}' (CPF: {cpf}) cadastrado");
        UI.MensagemSucesso($"Paciente '{nome}' cadastrado! (ID no vetor: {Dados.TotalPacientes})");
    }


    static void Listar()
    {
        Console.Clear();
        UI.ExibirCabecalho("LISTA DE PACIENTES  [VETOR]");

        if (Dados.TotalPacientes == 0)
        {
            Console.WriteLine("  Nenhum paciente cadastrado.\n");
            Console.Write("  Pressione qualquer tecla...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine(
            $"  {"ID",-5} {"CPF",-14} {"Nome",-28} {"Idade",-7} {"Tipo Sang.",-11} {"Telefone",-15}"
        );
        Console.WriteLine("  " + new string('-', 85));

        for (int i = 0; i < Dados.TotalPacientes; i++)
        {
            Console.WriteLine(
                $"  {i + 1,-5} " +
                $"{Dados.PacCpfs[i],-14} " +
                $"{Dados.PacNomes[i],-28} " +
                $"{Dados.PacIdades[i],-7} " +
                $"{Dados.PacTipoSang[i],-11} " +
                $"{Dados.PacTelefones[i],-15}"
            );
        }

        Console.WriteLine($"\n  Total: {Dados.TotalPacientes} paciente(s).");
        Console.Write("\n  Pressione qualquer tecla...");
        Console.ReadKey();
    }

    static void BuscarPorCpf()
    {
        Console.Clear();
        UI.ExibirCabecalho("BUSCAR PACIENTE POR CPF");

        string cpf = UI.LerTexto("CPF para busca");
        int idx = BuscarIndice(cpf);

        if (idx == -1)
        {
            UI.MensagemAviso("Nenhum paciente encontrado com o CPF informado.");
            return;
        }

        Console.WriteLine("\n  === DADOS DO PACIENTE ===");
        Console.WriteLine($"  ID (posicao no vetor) : {idx + 1}");
        Console.WriteLine($"  CPF                   : {Dados.PacCpfs[idx]}");
        Console.WriteLine($"  Nome                  : {Dados.PacNomes[idx]}");
        Console.WriteLine($"  Idade                 : {Dados.PacIdades[idx]} anos");
        Console.WriteLine($"  Telefone              : {Dados.PacTelefones[idx]}");
        Console.WriteLine($"  Tipo Sanguineo        : {Dados.PacTipoSang[idx]}");
        Console.WriteLine($"  Consultas registradas : {Consultas.ContarPorCpf(cpf)}");

        Console.Write("\n  Pressione qualquer tecla...");
        Console.ReadKey();
    }


    static void Remover()
    {
        Console.Clear();
        UI.ExibirCabecalho("REMOVER PACIENTE");

        string cpf = UI.LerTexto("CPF do paciente a remover");
        int idx = BuscarIndice(cpf);

        if (idx == -1)
        {
            UI.MensagemAviso("Nenhum paciente encontrado com o CPF informado.");
            return;
        }

        string nomeRemovido = Dados.PacNomes[idx];
        Console.WriteLine($"\n  Paciente encontrado: {nomeRemovido} (CPF: {cpf})");
        Console.Write("  Confirma a remocao? (S/N): ");
        string confirm = Console.ReadLine()?.Trim().ToUpper() ?? "N";

        if (confirm != "S")
        {
            UI.MensagemAviso("Operacao cancelada.");
            return;
        }

        for (int i = idx; i < Dados.TotalPacientes - 1; i++)
        {
            Dados.PacCpfs[i] = Dados.PacCpfs[i + 1];
            Dados.PacNomes[i] = Dados.PacNomes[i + 1];
            Dados.PacIdades[i] = Dados.PacIdades[i + 1];
            Dados.PacTelefones[i] = Dados.PacTelefones[i + 1];
            Dados.PacTipoSang[i] = Dados.PacTipoSang[i + 1];
        }

        int ultima = Dados.TotalPacientes - 1;
        Dados.PacCpfs[ultima] = "";
        Dados.PacNomes[ultima] = "";
        Dados.PacIdades[ultima] = 0;
        Dados.PacTelefones[ultima] = "";
        Dados.PacTipoSang[ultima] = "";
        Dados.TotalPacientes--;

        Historico.Registrar($"REMOCAO: Paciente '{nomeRemovido}' (CPF: {cpf}) removido do vetor");
        UI.MensagemSucesso($"Paciente '{nomeRemovido}' removido com sucesso!");
    }


    public static int BuscarIndice(string cpf)
    {
        for (int i = 0; i < Dados.TotalPacientes; i++)
        {
            if (Dados.PacCpfs[i] == cpf)
                return i;
        }
        return -1;
    }
    public static void OrdenarPorNome()
    {
        for (int i = 0; i < Dados.TotalPacientes - 1; i++)
        {
            for (int j = 0; j < Dados.TotalPacientes - i - 1; j++)
            {
                if (string.Compare(
                        Dados.PacNomes[j],
                        Dados.PacNomes[j + 1],
                        true) > 0)
                {
                    string tempNome = Dados.PacNomes[j];
                    Dados.PacNomes[j] = Dados.PacNomes[j + 1];
                    Dados.PacNomes[j + 1] = tempNome;

                    string tempCpf = Dados.PacCpfs[j];
                    Dados.PacCpfs[j] = Dados.PacCpfs[j + 1];
                    Dados.PacCpfs[j + 1] = tempCpf;

                    int tempIdade = Dados.PacIdades[j];
                    Dados.PacIdades[j] = Dados.PacIdades[j + 1];
                    Dados.PacIdades[j + 1] = tempIdade;
                }
            }
        }
        Console.WriteLine("\nPacientes ordenados por nome com sucesso!\n");
        Listar();
    }
}