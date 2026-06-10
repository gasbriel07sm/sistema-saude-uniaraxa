
public static class UI
{
    public static void ExibirCabecalho(string titulo)
    {
        string linha = new string('=', 62);
        Console.WriteLine("  " + linha);
        Console.WriteLine($"  CLINICA SAUDE UNIARAXA");
        Console.WriteLine($"  {titulo}");
        Console.WriteLine("  " + linha);
        Console.WriteLine();
    }

    public static void ExibirRodape()
    {
        Console.WriteLine("  " + new string('-', 62));
    }

    public static void MensagemSucesso(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n  [OK] {msg}");
        Console.ResetColor();
        Console.Write("  Pressione qualquer tecla para continuar...");
        Console.ReadKey();
    }

    public static void MensagemAviso(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n  [!]  {msg}");
        Console.ResetColor();
        Console.Write("  Pressione qualquer tecla para continuar...");
        Console.ReadKey();
    }

    public static string LerTexto(string campo)
    {
        string valor;
        do
        {
            Console.Write($"  {campo}: ");
            valor = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(valor))
                Console.WriteLine("  >> Campo obrigatorio! Tente novamente.");
        } while (string.IsNullOrWhiteSpace(valor));
        return valor.Trim();
    }

    public static int LerInteiro(string campo)
    {
        int valor;
        while (true)
        {
            Console.Write($"  {campo}: ");
            if (int.TryParse(Console.ReadLine(), out valor))
                return valor;
            Console.WriteLine("  >> Entrada invalida! Digite um numero inteiro.");
        }
    }
}


public static class Historico
{
    public static void Registrar(string descricao)
    {
        string acao = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] {descricao}";
        Dados.HistoricoAcoes.Push(acao);
    }

    public static void Exibir()
    {
        Console.Clear();
        UI.ExibirCabecalho("HISTORICO DE ACOES  [PILHA / STACK]");

        if (Dados.HistoricoAcoes.Count == 0)
        {
            Console.WriteLine("  Nenhuma acao registrada ainda.\n");
            Console.Write("  Pressione qualquer tecla...");
            Console.ReadKey();
            return;
        }

        string[] acoes = Dados.HistoricoAcoes.ToArray();

        Console.WriteLine("  Ordem: mais recente primeiro (topo da pilha):\n");

        for (int i = 0; i < acoes.Length; i++)
        {
            if (i == 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"  [TOPO] {acoes[i]}");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"  [{i + 1,3}]  {acoes[i]}");
            }
        }

        Console.WriteLine($"\n  Total de acoes na pilha: {Dados.HistoricoAcoes.Count}");
        Console.Write("\n  Pressione qualquer tecla...");
        Console.ReadKey();
    }
}
