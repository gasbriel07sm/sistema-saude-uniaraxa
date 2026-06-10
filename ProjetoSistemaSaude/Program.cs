

int opcao;
do
{
    Console.Clear();
    UI.ExibirCabecalho("MENU PRINCIPAL");
    Console.WriteLine("  [1] Gerenciar Pacientes    ");
    Console.WriteLine("  [2] Fila de Espera          ");
    Console.WriteLine("  [3] Gerenciar Consultas     ");
    Console.WriteLine("  [4] Historico de Acoes      ");
    Console.WriteLine("  [0] Sair do Sistema");
    UI.ExibirRodape();
    opcao = UI.LerInteiro("Opcao");

    switch (opcao)
    {
        case 1: Pacientes.Menu(); break;
        case 2: FilaEspera.Menu(); break;
        case 3: Consultas.Menu(); break;
        case 4: Historico.Exibir(); break;
        case 0:
            Console.WriteLine("\n  Sistema encerrado. Ate logo!\n");
            break;
        default:
            UI.MensagemAviso("Opcao invalida! Digite um numero do menu.");
            break;
    }

} while (opcao != 0);
