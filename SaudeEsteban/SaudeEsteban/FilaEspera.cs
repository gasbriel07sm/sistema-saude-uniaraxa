
public static class FilaEspera
{
    public static void Menu()
    {
        int opcao;
        do
        {
            Console.Clear();
            UI.ExibirCabecalho("FILA DE ESPERA  [QUEUE / FILA]");
            Console.WriteLine($"  Pacientes aguardando: {Dados.FilaEspera.Count}");
            Console.WriteLine();
            Console.WriteLine("  [1] Adicionar Paciente a Fila   (Enqueue)");
            Console.WriteLine("  [2] Chamar Proximo Paciente      (Dequeue)");
            Console.WriteLine("  [3] Ver Proximo sem Remover      (Peek)");
            Console.WriteLine("  [4] Exibir Fila Completa");
            Console.WriteLine("  [5] Limpar Fila de Espera");
            Console.WriteLine("  [0] Voltar ao Menu Principal");
            UI.ExibirRodape();
            opcao = UI.LerInteiro("Opcao");

            switch (opcao)
            {
                case 1: Adicionar(); break;
                case 2: ChamarProximo(); break;
                case 3: VerProximo(); break;
                case 4: ExibirCompleta(); break;
                case 5: Limpar(); break;
                case 0: break;
                default: UI.MensagemAviso("Opcao invalida!"); break;
            }

        } while (opcao != 0);
    }

   static void Adicionar()
    {
        Console.Clear();
        UI.ExibirCabecalho("ADICIONAR NA FILA DE ESPERA  [Enqueue]");

        string cpf = UI.LerTexto("CPF do paciente");
        int idx = Pacientes.BuscarIndice(cpf);

        if (idx == -1)
        {
            UI.MensagemAviso("Paciente nao encontrado. Cadastre-o primeiro em 'Gerenciar Pacientes'.");
            return;
        }

        foreach (string cpfNaFila in Dados.FilaEspera)
        {
            if (cpfNaFila == cpf)
            {
                UI.MensagemAviso($"'{Dados.PacNomes[idx]}' ja esta na fila de espera!");
                return;
            }
        }

        Dados.FilaEspera.Enqueue(cpf);

        Historico.Registrar($"FILA [Enqueue]: '{Dados.PacNomes[idx]}' adicionado - posicao {Dados.FilaEspera.Count}");
        UI.MensagemSucesso($"'{Dados.PacNomes[idx]}' adicionado! Posicao na fila: {Dados.FilaEspera.Count}");
    }

    static void ChamarProximo()
    {
        Console.Clear();
        UI.ExibirCabecalho("CHAMAR PROXIMO PACIENTE  [Dequeue]");

        if (Dados.FilaEspera.Count == 0)
        {
            UI.MensagemAviso("A fila de espera esta vazia! Nenhum paciente aguardando.");
            return;
        }

        string cpfChamado = Dados.FilaEspera.Dequeue();
        int idx = Pacientes.BuscarIndice(cpfChamado);

        string nomePac = (idx != -1) ? Dados.PacNomes[idx] : "(cadastro removido)";
        string tipoSang = (idx != -1) ? Dados.PacTipoSang[idx] : "-";

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n  *** PROXIMO PACIENTE CHAMADO ***\n");
        Console.ResetColor();
        Console.WriteLine($"  Nome           : {nomePac}");
        Console.WriteLine($"  CPF            : {cpfChamado}");
        Console.WriteLine($"  Tipo Sanguineo : {tipoSang}");
        Console.WriteLine($"\n  Pacientes restantes na fila: {Dados.FilaEspera.Count}");

        Historico.Registrar($"FILA [Dequeue]: '{nomePac}' (CPF: {cpfChamado}) chamado para atendimento");
        Console.Write("\n  Pressione qualquer tecla...");
        Console.ReadKey();
    }

    static void VerProximo()
    {
        Console.Clear();
        UI.ExibirCabecalho("PROXIMO DA FILA  [Peek]");

        if (Dados.FilaEspera.Count == 0)
        {
            UI.MensagemAviso("A fila de espera esta vazia!");
            return;
        }

        string cpfProximo = Dados.FilaEspera.Peek();
        int idx = Pacientes.BuscarIndice(cpfProximo);
        string nome = (idx != -1) ? Dados.PacNomes[idx] : "(cadastro removido)";

        Console.WriteLine($"\n  Proximo a ser atendido:");
        Console.WriteLine($"    Nome : {nome}");
        Console.WriteLine($"    CPF  : {cpfProximo}");
        Console.WriteLine($"\n  (O paciente NAO foi removido da fila)");
        Console.WriteLine($"  Total na fila: {Dados.FilaEspera.Count}");

        Console.Write("\n  Pressione qualquer tecla...");
        Console.ReadKey();
    }

   static void ExibirCompleta()
    {
        Console.Clear();
        UI.ExibirCabecalho("FILA DE ESPERA COMPLETA");

        if (Dados.FilaEspera.Count == 0)
        {
            Console.WriteLine("  A fila esta vazia.\n");
            Console.Write("  Pressione qualquer tecla...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine(
            $"  {"Pos.",-8} {"CPF",-14} {"Nome",-28} {"Tipo Sang.",-12}"
        );
        Console.WriteLine("  " + new string('-', 66));

        int posicao = 1;
        foreach (string cpf in Dados.FilaEspera)
        {
            int idx = Pacientes.BuscarIndice(cpf);
            string nome = (idx != -1) ? Dados.PacNomes[idx] : "(removido)";
            string tipoSang = (idx != -1) ? Dados.PacTipoSang[idx] : "-";
            string marcador = (posicao == 1) ? "→" : " ";

            Console.WriteLine(
                $"  {marcador} {posicao,-6} {cpf,-14} {nome,-28} {tipoSang,-12}"
            );
            posicao++;
        }

        Console.WriteLine($"\n  Total na fila: {Dados.FilaEspera.Count} paciente(s)");
        Console.Write("\n  Pressione qualquer tecla...");
        Console.ReadKey();
    }

   static void Limpar()
    {
        Console.Clear();
        UI.ExibirCabecalho("LIMPAR FILA DE ESPERA");

        if (Dados.FilaEspera.Count == 0)
        {
            UI.MensagemAviso("A fila ja esta vazia.");
            return;
        }

        Console.Write($"  Confirma a remocao de todos os {Dados.FilaEspera.Count} paciente(s)? (S/N): ");
        string confirm = Console.ReadLine()?.Trim().ToUpper() ?? "N";

        if (confirm == "S")
        {
            int qtd = Dados.FilaEspera.Count;
            Dados.FilaEspera.Clear();
            Historico.Registrar($"FILA [Clear]: Fila esvaziada ({qtd} paciente(s) removidos)");
            UI.MensagemSucesso("Fila de espera limpa com sucesso!");
        }
        else
        {
            UI.MensagemAviso("Operacao cancelada.");
        }
    }
}
