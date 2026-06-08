// ================================================================
//  SISTEMA DE GERENCIAMENTO DE CONSULTAS MEDICAS
//  Clinica Saude UniAraxa
// ================================================================
//
//  ESTRUTURAS DE DADOS UTILIZADAS:
//
//  [1] VETOR (array unidimensional)
//      Usado em: Cadastro de Pacientes
//      Os dados de cada paciente ficam distribuidos em varios
//      vetores paralelos (um vetor por campo). O indice i
//      representa sempre o mesmo paciente em todos os vetores.
//
//  [2] FILA (Queue - FIFO: First In, First Out)
//      Usado em: Fila de Espera
//      O primeiro paciente a entrar na fila eh o primeiro
//      a ser chamado para atendimento. Novos pacientes
//      sempre vao para o final da fila.
//
//  [3] MATRIZ (array bidimensional - string[,])
//      Usado em: Registro de Consultas
//      Cada LINHA da matriz representa uma consulta.
//      Cada COLUNA representa um campo da consulta.
//      Visualizacao:
//        Linha 0: [cpf | nome | data | medico | espec. | diag.]
//        Linha 1: [cpf | nome | data | medico | espec. | diag.]
//        ...
//
//  [4] PILHA (Stack - LIFO: Last In, First Out)
//      Usado em: Historico de Acoes
//      Toda operacao realizada eh empilhada. A acao mais
//      recente fica sempre no topo da pilha.
//
// ================================================================

using System;
using System.Collections.Generic;

namespace SistemaClinica
{
    class Program
    {
        // ============================================================
        // SECAO 1: CONSTANTES GLOBAIS DO SISTEMA
        // ============================================================

        // Limites de capacidade das estruturas
        const int MAX_PACIENTES = 100;
        const int MAX_CONSULTAS = 200;

        // A matriz de consultas tem 6 colunas (uma por campo)
        const int COLUNAS_CONSULTA = 6;

        // Indices das colunas da matriz de consultas
        // Usar constantes evita "numeros magicos" no codigo
        const int COL_CPF_PAC     = 0; // CPF do paciente
        const int COL_NOME_PAC    = 1; // Nome do paciente
        const int COL_DATA        = 2; // Data da consulta (DD/MM/AAAA)
        const int COL_MEDICO      = 3; // Nome do medico responsavel
        const int COL_ESPECIALID  = 4; // Especialidade medica
        const int COL_DIAGNOSTICO = 5; // Diagnostico / observacoes

        // ============================================================
        // SECAO 2: ESTRUTURA 1 — VETOR DE PACIENTES
        // ============================================================
        //
        // Sao 5 vetores paralelos de tamanho MAX_PACIENTES.
        // O paciente no indice i possui:
        //   pacNomes[i], pacCpfs[i], pacIdades[i], etc.
        //
        // Insercao: O(1) — sempre na posicao [totalPacientes]
        // Busca:    O(n) — percorre do inicio ao fim
        // Remocao:  O(n) — precisa compactar (shift) o vetor

        static string[] pacNomes     = new string[MAX_PACIENTES];
        static string[] pacCpfs      = new string[MAX_PACIENTES];
        static int[]    pacIdades    = new int[MAX_PACIENTES];
        static string[] pacTelefones = new string[MAX_PACIENTES];
        static string[] pacTipoSang  = new string[MAX_PACIENTES];

        // totalPacientes aponta sempre para a proxima posicao livre
        static int totalPacientes = 0;

        // ============================================================
        // SECAO 3: ESTRUTURA 2 — FILA DE ESPERA (Queue)
        // ============================================================
        //
        // Queue<string> do namespace System.Collections.Generic
        // Armazena o CPF dos pacientes em espera.
        //
        //   Enqueue(cpf)  → insere no FINAL       O(1)
        //   Dequeue()     → remove do INICIO      O(1)
        //   Peek()        → consulta o inicio     O(1)
        //   Count         → tamanho atual         O(1)

        static Queue<string> filaEspera = new Queue<string>();

        // ============================================================
        // SECAO 4: ESTRUTURA 3 — MATRIZ DE CONSULTAS (Array 2D)
        // ============================================================
        //
        // string[,] eh um array bidimensional retangular em C#.
        // matrizConsultas[linha, coluna]
        //
        //   Cada linha = uma consulta medica
        //   Cada coluna = um campo da consulta (ver constantes acima)
        //
        // Para acessar o medico da consulta 3:
        //   matrizConsultas[2, COL_MEDICO]
        //
        // totalConsultas aponta para a proxima linha livre

        static string[,] matrizConsultas = new string[MAX_CONSULTAS, COLUNAS_CONSULTA];
        static int totalConsultas = 0;

        // ============================================================
        // SECAO 5: ESTRUTURA 4 — HISTORICO (Stack)
        // ============================================================
        //
        // Stack<string> armazena descricoes de acoes realizadas.
        //
        //   Push(acao)   → empilha no TOPO         O(1)
        //   ToArray()    → lista do topo a base    O(n)
        //   Count        → tamanho atual           O(1)

        static Stack<string> historicoAcoes = new Stack<string>();

        // ============================================================
        // SECAO 6: PONTO DE ENTRADA — MENU PRINCIPAL
        // ============================================================

        static void Main(string[] args)
        {
            // Garante que o terminal exiba caracteres especiais corretamente
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding  = System.Text.Encoding.UTF8;

            int opcao;
            do
            {
                Console.Clear();
                ExibirCabecalho("MENU PRINCIPAL");
                Console.WriteLine("  [1] Gerenciar Pacientes     (VETOR)");
                Console.WriteLine("  [2] Fila de Espera          (FILA / QUEUE)");
                Console.WriteLine("  [3] Gerenciar Consultas     (MATRIZ)");
                Console.WriteLine("  [4] Historico de Acoes      (PILHA / STACK)");
                Console.WriteLine("  [0] Sair do Sistema");
                ExibirRodape();
                opcao = LerInteiro("Opcao");

                switch (opcao)
                {
                    case 1: MenuPacientes();  break;
                    case 2: MenuFilaEspera(); break;
                    case 3: MenuConsultas();  break;
                    case 4: ExibirHistorico(); break;
                    case 0:
                        Console.WriteLine("\n  Sistema encerrado. Ate logo!\n");
                        break;
                    default:
                        MensagemAviso("Opcao invalida! Digite um numero do menu.");
                        break;
                }

            } while (opcao != 0);
        }

        // ============================================================
        // SECAO 7: SUBMENU — PACIENTES
        // ============================================================

        static void MenuPacientes()
        {
            int opcao;
            do
            {
                Console.Clear();
                ExibirCabecalho("GERENCIAMENTO DE PACIENTES  [VETOR]");
                Console.WriteLine($"  Pacientes cadastrados: {totalPacientes} / {MAX_PACIENTES}");
                Console.WriteLine();
                Console.WriteLine("  [1] Cadastrar Paciente");
                Console.WriteLine("  [2] Listar Todos os Pacientes");
                Console.WriteLine("  [3] Buscar Paciente por CPF");
                Console.WriteLine("  [4] Remover Paciente");
                Console.WriteLine("  [0] Voltar ao Menu Principal");
                ExibirRodape();
                opcao = LerInteiro("Opcao");

                switch (opcao)
                {
                    case 1: CadastrarPaciente();    break;
                    case 2: ListarPacientes();      break;
                    case 3: BuscarPacientePorCpf(); break;
                    case 4: RemoverPaciente();      break;
                    case 0: break;
                    default: MensagemAviso("Opcao invalida!"); break;
                }

            } while (opcao != 0);
        }

        // ------------------------------------------------------------
        // CadastrarPaciente
        //
        // Insere os dados do novo paciente nas posicoes [totalPacientes]
        // de cada vetor paralelo e entao incrementa o contador.
        //
        // Verificacoes:
        //   - Capacidade maxima do vetor nao atingida
        //   - CPF nao duplicado (busca linear antes de inserir)
        // ------------------------------------------------------------
        static void CadastrarPaciente()
        {
            Console.Clear();
            ExibirCabecalho("CADASTRAR PACIENTE");

            // Guarda nao ha espaco no vetor
            if (totalPacientes >= MAX_PACIENTES)
            {
                MensagemAviso("ERRO: Capacidade maxima atingida (" + MAX_PACIENTES + " pacientes).");
                return;
            }

            Console.WriteLine("  Preencha os dados do paciente:\n");

            // Leitura dos campos obrigatorios
            string cpf  = LerTexto("CPF (somente numeros)");

            // Verifica duplicata antes de continuar o cadastro
            if (BuscarIndicePorCpf(cpf) != -1)
            {
                MensagemAviso("ERRO: Este CPF ja esta cadastrado no sistema!");
                return;
            }

            string nome     = LerTexto("Nome completo");
            int    idade    = LerInteiro("Idade");
            string telefone = LerTexto("Telefone");
            string tipoSang = LerTexto("Tipo Sanguineo (ex: A+, O-, AB-)");

            // Insercao nos vetores paralelos
            // Todos apontam para o mesmo indice [totalPacientes]
            pacCpfs[totalPacientes]      = cpf;
            pacNomes[totalPacientes]     = nome;
            pacIdades[totalPacientes]    = idade;
            pacTelefones[totalPacientes] = telefone;
            pacTipoSang[totalPacientes]  = tipoSang;

            totalPacientes++; // avanca o apontador para a proxima posicao livre

            RegistrarAcao($"CADASTRO: Paciente '{nome}' (CPF: {cpf}) cadastrado");
            MensagemSucesso($"Paciente '{nome}' cadastrado! (ID no vetor: {totalPacientes})");
        }

        // ------------------------------------------------------------
        // ListarPacientes
        //
        // Percorre os vetores do indice 0 ate totalPacientes-1
        // e exibe os dados formatados em tabela.
        // ------------------------------------------------------------
        static void ListarPacientes()
        {
            Console.Clear();
            ExibirCabecalho("LISTA DE PACIENTES  [VETOR]");

            if (totalPacientes == 0)
            {
                Console.WriteLine("  Nenhum paciente cadastrado.\n");
                Console.Write("  Pressione qualquer tecla...");
                Console.ReadKey();
                return;
            }

            // Cabecalho da tabela
            Console.WriteLine(
                $"  {"ID",-5} " +
                $"{"CPF",-14} " +
                $"{"Nome",-28} " +
                $"{"Idade",-7} " +
                $"{"Tipo Sang.",-11} " +
                $"{"Telefone",-15}"
            );
            Console.WriteLine("  " + new string('-', 85));

            // Laco principal: percorre todos os indices validos do vetor
            for (int i = 0; i < totalPacientes; i++)
            {
                Console.WriteLine(
                    $"  {i + 1,-5} " +
                    $"{pacCpfs[i],-14} " +
                    $"{pacNomes[i],-28} " +
                    $"{pacIdades[i],-7} " +
                    $"{pacTipoSang[i],-11} " +
                    $"{pacTelefones[i],-15}"
                );
            }

            Console.WriteLine($"\n  Total: {totalPacientes} paciente(s) cadastrado(s).");
            Console.Write("\n  Pressione qualquer tecla para voltar...");
            Console.ReadKey();
        }

        // ------------------------------------------------------------
        // BuscarPacientePorCpf
        //
        // Realiza busca linear no vetor de CPFs.
        // Chama BuscarIndicePorCpf (metodo auxiliar reutilizavel).
        // ------------------------------------------------------------
        static void BuscarPacientePorCpf()
        {
            Console.Clear();
            ExibirCabecalho("BUSCAR PACIENTE POR CPF");

            string cpf = LerTexto("CPF para busca");
            int idx = BuscarIndicePorCpf(cpf); // busca linear O(n)

            if (idx == -1)
            {
                MensagemAviso("Nenhum paciente encontrado com o CPF informado.");
                return;
            }

            // Exibe os dados do paciente encontrado
            Console.WriteLine("\n  === DADOS DO PACIENTE ===");
            Console.WriteLine($"  ID (posicao no vetor) : {idx + 1}");
            Console.WriteLine($"  CPF                   : {pacCpfs[idx]}");
            Console.WriteLine($"  Nome                  : {pacNomes[idx]}");
            Console.WriteLine($"  Idade                 : {pacIdades[idx]} anos");
            Console.WriteLine($"  Telefone              : {pacTelefones[idx]}");
            Console.WriteLine($"  Tipo Sanguineo        : {pacTipoSang[idx]}");

            // Conta quantas consultas este paciente tem na matriz
            int numConsultas = ContarConsultasPorCpf(cpf);
            Console.WriteLine($"  Consultas registradas : {numConsultas}");

            Console.Write("\n  Pressione qualquer tecla para voltar...");
            Console.ReadKey();
        }

        // ------------------------------------------------------------
        // RemoverPaciente
        //
        // Remove o paciente do vetor e compacta o array.
        //
        // Compactacao (shift para a esquerda):
        //   Apos remover o elemento no indice idx, todos os
        //   elementos a direita sao movidos uma posicao para
        //   a esquerda, fechando o "buraco" criado.
        //
        //   Antes: [A][B][C][D][E]   (remover C, idx=2)
        //   Apos:  [A][B][D][E][ ]   totalPacientes--
        // ------------------------------------------------------------
        static void RemoverPaciente()
        {
            Console.Clear();
            ExibirCabecalho("REMOVER PACIENTE");

            string cpf = LerTexto("CPF do paciente a remover");
            int idx = BuscarIndicePorCpf(cpf);

            if (idx == -1)
            {
                MensagemAviso("Nenhum paciente encontrado com o CPF informado.");
                return;
            }

            string nomeRemovido = pacNomes[idx];
            Console.WriteLine($"\n  Paciente encontrado: {nomeRemovido} (CPF: {cpf})");
            Console.Write("  Confirma a remocao? (S/N): ");
            string confirm = Console.ReadLine()?.Trim().ToUpper() ?? "N";

            if (confirm != "S")
            {
                MensagemAviso("Operacao cancelada.");
                return;
            }

            // Shift para a esquerda: compacta os vetores paralelos
            for (int i = idx; i < totalPacientes - 1; i++)
            {
                pacCpfs[i]      = pacCpfs[i + 1];
                pacNomes[i]     = pacNomes[i + 1];
                pacIdades[i]    = pacIdades[i + 1];
                pacTelefones[i] = pacTelefones[i + 1];
                pacTipoSang[i]  = pacTipoSang[i + 1];
            }

            // Limpa a ultima posicao (agora redundante) e decrementa
            int ultima = totalPacientes - 1;
            pacCpfs[ultima]      = "";
            pacNomes[ultima]     = "";
            pacIdades[ultima]    = 0;
            pacTelefones[ultima] = "";
            pacTipoSang[ultima]  = "";
            totalPacientes--;

            RegistrarAcao($"REMOCAO: Paciente '{nomeRemovido}' (CPF: {cpf}) removido do vetor");
            MensagemSucesso($"Paciente '{nomeRemovido}' removido com sucesso!");
        }

        // ============================================================
        // SECAO 8: SUBMENU — FILA DE ESPERA
        // ============================================================

        static void MenuFilaEspera()
        {
            int opcao;
            do
            {
                Console.Clear();
                ExibirCabecalho("FILA DE ESPERA  [QUEUE / FILA]");
                Console.WriteLine($"  Pacientes aguardando: {filaEspera.Count}");
                Console.WriteLine();
                Console.WriteLine("  [1] Adicionar Paciente a Fila   (Enqueue)");
                Console.WriteLine("  [2] Chamar Proximo Paciente      (Dequeue)");
                Console.WriteLine("  [3] Ver Proximo sem Remover      (Peek)");
                Console.WriteLine("  [4] Exibir Fila Completa");
                Console.WriteLine("  [5] Limpar Fila de Espera");
                Console.WriteLine("  [0] Voltar ao Menu Principal");
                ExibirRodape();
                opcao = LerInteiro("Opcao");

                switch (opcao)
                {
                    case 1: AdicionarNaFila();        break;
                    case 2: ChamarProximoPaciente();  break;
                    case 3: VerProximoDaFila();       break;
                    case 4: ExibirFilaCompleta();     break;
                    case 5: LimparFila();             break;
                    case 0: break;
                    default: MensagemAviso("Opcao invalida!"); break;
                }

            } while (opcao != 0);
        }

        // ------------------------------------------------------------
        // AdicionarNaFila  —  Enqueue
        //
        // Adiciona o CPF do paciente ao FINAL da fila.
        // Verifica:
        //   1. Se o paciente existe no vetor de cadastro
        //   2. Se o paciente ja esta na fila (evita duplicatas)
        // ------------------------------------------------------------
        static void AdicionarNaFila()
        {
            Console.Clear();
            ExibirCabecalho("ADICIONAR NA FILA DE ESPERA  [Enqueue]");

            string cpf = LerTexto("CPF do paciente");
            int idx = BuscarIndicePorCpf(cpf);

            if (idx == -1)
            {
                MensagemAviso("Paciente nao encontrado. Cadastre-o primeiro em 'Gerenciar Pacientes'.");
                return;
            }

            // Verifica se o paciente ja esta na fila (iteracao sobre a fila)
            foreach (string cpfNaFila in filaEspera)
            {
                if (cpfNaFila == cpf)
                {
                    MensagemAviso($"'{pacNomes[idx]}' ja esta na fila de espera!");
                    return;
                }
            }

            // Enqueue: insere no FINAL da fila
            filaEspera.Enqueue(cpf);

            RegistrarAcao($"FILA [Enqueue]: '{pacNomes[idx]}' adicionado - posicao {filaEspera.Count}");
            MensagemSucesso($"'{pacNomes[idx]}' adicionado! Posicao na fila: {filaEspera.Count}");
        }

        // ------------------------------------------------------------
        // ChamarProximoPaciente  —  Dequeue
        //
        // Remove e retorna o elemento do INICIO da fila.
        // O paciente removido deve ser atendido agora.
        // ------------------------------------------------------------
        static void ChamarProximoPaciente()
        {
            Console.Clear();
            ExibirCabecalho("CHAMAR PROXIMO PACIENTE  [Dequeue]");

            if (filaEspera.Count == 0)
            {
                MensagemAviso("A fila de espera esta vazia! Nenhum paciente aguardando.");
                return;
            }

            // Dequeue: remove o primeiro elemento da fila
            string cpfChamado = filaEspera.Dequeue();
            int idx = BuscarIndicePorCpf(cpfChamado);

            string nomePac   = (idx != -1) ? pacNomes[idx]    : "(cadastro removido)";
            string tipoSang  = (idx != -1) ? pacTipoSang[idx] : "-";

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  *** PROXIMO PACIENTE CHAMADO ***\n");
            Console.ResetColor();
            Console.WriteLine($"  Nome           : {nomePac}");
            Console.WriteLine($"  CPF            : {cpfChamado}");
            Console.WriteLine($"  Tipo Sanguineo : {tipoSang}");
            Console.WriteLine($"\n  Pacientes restantes na fila: {filaEspera.Count}");

            RegistrarAcao($"FILA [Dequeue]: '{nomePac}' (CPF: {cpfChamado}) chamado para atendimento");
            Console.Write("\n  Pressione qualquer tecla para continuar...");
            Console.ReadKey();
        }

        // ------------------------------------------------------------
        // VerProximoDaFila  —  Peek
        //
        // Consulta o primeiro elemento da fila SEM remove-lo.
        // Util para verificar quem sera atendido a seguir.
        // ------------------------------------------------------------
        static void VerProximoDaFila()
        {
            Console.Clear();
            ExibirCabecalho("PROXIMO DA FILA  [Peek]");

            if (filaEspera.Count == 0)
            {
                MensagemAviso("A fila de espera esta vazia!");
                return;
            }

            // Peek: consulta sem remover
            string cpfProximo = filaEspera.Peek();
            int idx = BuscarIndicePorCpf(cpfProximo);

            string nome = (idx != -1) ? pacNomes[idx] : "(cadastro removido)";

            Console.WriteLine($"\n  Proximo a ser atendido:");
            Console.WriteLine($"    Nome : {nome}");
            Console.WriteLine($"    CPF  : {cpfProximo}");
            Console.WriteLine($"\n  (O paciente NAO foi removido da fila)");
            Console.WriteLine($"  Total na fila: {filaEspera.Count}");

            Console.Write("\n  Pressione qualquer tecla...");
            Console.ReadKey();
        }

        // ------------------------------------------------------------
        // ExibirFilaCompleta
        //
        // Percorre a fila com foreach sem modifica-la.
        // A seta (→) indica o proximo a ser atendido (inicio da fila).
        // ------------------------------------------------------------
        static void ExibirFilaCompleta()
        {
            Console.Clear();
            ExibirCabecalho("FILA DE ESPERA COMPLETA");

            if (filaEspera.Count == 0)
            {
                Console.WriteLine("  A fila esta vazia.\n");
                Console.Write("  Pressione qualquer tecla...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine(
                $"  {"Pos.",-8} " +
                $"{"CPF",-14} " +
                $"{"Nome",-28} " +
                $"{"Tipo Sang.",-12}"
            );
            Console.WriteLine("  " + new string('-', 66));

            int posicao = 1;
            foreach (string cpf in filaEspera)
            {
                int    idx      = BuscarIndicePorCpf(cpf);
                string nome     = (idx != -1) ? pacNomes[idx]    : "(removido)";
                string tipoSang = (idx != -1) ? pacTipoSang[idx] : "-";

                // Destaca o primeiro da fila com uma seta
                string marcador = (posicao == 1) ? "→" : " ";
                Console.WriteLine(
                    $"  {marcador} {posicao,-6} " +
                    $"{cpf,-14} " +
                    $"{nome,-28} " +
                    $"{tipoSang,-12}"
                );
                posicao++;
            }

            Console.WriteLine($"\n  Total na fila: {filaEspera.Count} paciente(s)");
            Console.Write("\n  Pressione qualquer tecla...");
            Console.ReadKey();
        }

        // ------------------------------------------------------------
        // LimparFila
        //
        // Remove todos os elementos da fila com Clear().
        // Solicita confirmacao antes de executar.
        // ------------------------------------------------------------
        static void LimparFila()
        {
            Console.Clear();
            ExibirCabecalho("LIMPAR FILA DE ESPERA");

            if (filaEspera.Count == 0)
            {
                MensagemAviso("A fila ja esta vazia.");
                return;
            }

            Console.Write($"  Confirma a remocao de todos os {filaEspera.Count} paciente(s) da fila? (S/N): ");
            string confirm = Console.ReadLine()?.Trim().ToUpper() ?? "N";

            if (confirm == "S")
            {
                int qtd = filaEspera.Count;
                filaEspera.Clear(); // esvazia a fila
                RegistrarAcao($"FILA [Clear]: Fila esvaziada ({qtd} paciente(s) removidos)");
                MensagemSucesso("Fila de espera limpa com sucesso!");
            }
            else
            {
                MensagemAviso("Operacao cancelada.");
            }
        }

        // ============================================================
        // SECAO 9: SUBMENU — CONSULTAS (MATRIZ)
        // ============================================================

        static void MenuConsultas()
        {
            int opcao;
            do
            {
                Console.Clear();
                ExibirCabecalho("GERENCIAMENTO DE CONSULTAS  [MATRIZ]");
                Console.WriteLine($"  Consultas registradas: {totalConsultas} / {MAX_CONSULTAS}");
                Console.WriteLine();
                Console.WriteLine("  [1] Registrar Nova Consulta");
                Console.WriteLine("  [2] Listar Todas as Consultas");
                Console.WriteLine("  [3] Buscar Consultas por CPF do Paciente");
                Console.WriteLine("  [4] Buscar Consultas por Medico");
                Console.WriteLine("  [5] Buscar Consultas por Data");
                Console.WriteLine("  [0] Voltar ao Menu Principal");
                ExibirRodape();
                opcao = LerInteiro("Opcao");

                switch (opcao)
                {
                    case 1: RegistrarConsulta();          break;
                    case 2: ListarConsultas();            break;
                    case 3: BuscarConsultasPorCpf();      break;
                    case 4: BuscarConsultasPorMedico();   break;
                    case 5: BuscarConsultasPorData();     break;
                    case 0: break;
                    default: MensagemAviso("Opcao invalida!"); break;
                }

            } while (opcao != 0);
        }

        // ------------------------------------------------------------
        // RegistrarConsulta
        //
        // Preenche a linha [totalConsultas] da matriz com os dados
        // da nova consulta. Cada coluna recebe um campo especifico.
        //
        // Representacao da linha na matriz:
        //   [CPF][NOME][DATA][MEDICO][ESPECIALIDADE][DIAGNOSTICO]
        //    [0]  [1]   [2]   [3]       [4]              [5]
        // ------------------------------------------------------------
        static void RegistrarConsulta()
        {
            Console.Clear();
            ExibirCabecalho("REGISTRAR NOVA CONSULTA  [MATRIZ]");

            // Verifica se a matriz tem espaco disponivel
            if (totalConsultas >= MAX_CONSULTAS)
            {
                MensagemAviso("ERRO: Capacidade maxima de consultas atingida (" + MAX_CONSULTAS + ").");
                return;
            }

            string cpf = LerTexto("CPF do paciente");
            int idx = BuscarIndicePorCpf(cpf);

            if (idx == -1)
            {
                MensagemAviso("Paciente nao encontrado. Cadastre-o primeiro.");
                return;
            }

            Console.WriteLine($"\n  Paciente: {pacNomes[idx]} | Tipo Sang.: {pacTipoSang[idx]}\n");

            // Coleta os dados da consulta
            string data       = LerTexto("Data da consulta (DD/MM/AAAA)");
            string medico     = LerTexto("Nome do medico (ex: Dr. Carlos Silva)");
            string especialid = LerTexto("Especialidade (ex: Cardiologia, Clinico Geral)");
            string diagnostico = LerTexto("Diagnostico / Observacoes");

            // Preenche a linha [totalConsultas] coluna por coluna
            matrizConsultas[totalConsultas, COL_CPF_PAC]     = cpf;
            matrizConsultas[totalConsultas, COL_NOME_PAC]    = pacNomes[idx];
            matrizConsultas[totalConsultas, COL_DATA]        = data;
            matrizConsultas[totalConsultas, COL_MEDICO]      = medico;
            matrizConsultas[totalConsultas, COL_ESPECIALID]  = especialid;
            matrizConsultas[totalConsultas, COL_DIAGNOSTICO] = diagnostico;

            totalConsultas++; // avanca para a proxima linha livre da matriz

            RegistrarAcao($"CONSULTA: '{pacNomes[idx]}' com {medico} em {data} registrada na matriz[{totalConsultas - 1}]");
            MensagemSucesso($"Consulta registrada na linha {totalConsultas} da matriz!");
        }

        // ------------------------------------------------------------
        // ListarConsultas
        //
        // Percorre as linhas da matriz de 0 ate totalConsultas-1.
        // Para cada linha, acessa as 6 colunas usando as constantes.
        // ------------------------------------------------------------
        static void ListarConsultas()
        {
            Console.Clear();
            ExibirCabecalho("LISTA DE CONSULTAS  [MATRIZ]");

            if (totalConsultas == 0)
            {
                Console.WriteLine("  Nenhuma consulta registrada.\n");
                Console.Write("  Pressione qualquer tecla...");
                Console.ReadKey();
                return;
            }

            // Exibe cada linha da matriz formatada
            for (int i = 0; i < totalConsultas; i++)
            {
                Console.WriteLine($"  ---- Consulta #{i + 1} (linha {i} da matriz) ----");
                Console.WriteLine($"    CPF Paciente  : {matrizConsultas[i, COL_CPF_PAC]}");
                Console.WriteLine($"    Nome          : {matrizConsultas[i, COL_NOME_PAC]}");
                Console.WriteLine($"    Data          : {matrizConsultas[i, COL_DATA]}");
                Console.WriteLine($"    Medico        : {matrizConsultas[i, COL_MEDICO]}");
                Console.WriteLine($"    Especialidade : {matrizConsultas[i, COL_ESPECIALID]}");
                Console.WriteLine($"    Diagnostico   : {matrizConsultas[i, COL_DIAGNOSTICO]}");
                Console.WriteLine();
            }

            Console.WriteLine($"  Total: {totalConsultas} consulta(s) registrada(s).");
            Console.Write("\n  Pressione qualquer tecla...");
            Console.ReadKey();
        }

        // ------------------------------------------------------------
        // BuscarConsultasPorCpf
        //
        // Busca linear nas linhas da matriz comparando a coluna [0] (CPF).
        // Exibe todas as consultas do paciente encontradas.
        // ------------------------------------------------------------
        static void BuscarConsultasPorCpf()
        {
            Console.Clear();
            ExibirCabecalho("BUSCAR CONSULTAS POR CPF");

            string cpf = LerTexto("CPF do paciente");
            bool encontrou = false;

            Console.WriteLine($"\n  Consultas para CPF: {cpf}\n");

            for (int i = 0; i < totalConsultas; i++)
            {
                // Compara a coluna CPF da linha i
                if (matrizConsultas[i, COL_CPF_PAC] == cpf)
                {
                    Console.WriteLine($"  Consulta #{i + 1}  |  Data: {matrizConsultas[i, COL_DATA]}");
                    Console.WriteLine($"    Medico        : {matrizConsultas[i, COL_MEDICO]}");
                    Console.WriteLine($"    Especialidade : {matrizConsultas[i, COL_ESPECIALID]}");
                    Console.WriteLine($"    Diagnostico   : {matrizConsultas[i, COL_DIAGNOSTICO]}");
                    Console.WriteLine();
                    encontrou = true;
                }
            }

            if (!encontrou)
                Console.WriteLine("  Nenhuma consulta encontrada para este CPF.");

            Console.Write("\n  Pressione qualquer tecla...");
            Console.ReadKey();
        }

        // ------------------------------------------------------------
        // BuscarConsultasPorMedico
        //
        // Busca parcial (case-insensitive) na coluna COL_MEDICO.
        // Permite encontrar consultas mesmo com nome incompleto.
        // ------------------------------------------------------------
        static void BuscarConsultasPorMedico()
        {
            Console.Clear();
            ExibirCabecalho("BUSCAR CONSULTAS POR MEDICO");

            string nomeMedico = LerTexto("Nome do medico (ou parte do nome)");
            bool encontrou = false;

            Console.WriteLine($"\n  Consultas de: {nomeMedico}\n");

            for (int i = 0; i < totalConsultas; i++)
            {
                // IndexOf com StringComparison.OrdinalIgnoreCase = case-insensitive
                if (matrizConsultas[i, COL_MEDICO]
                        .IndexOf(nomeMedico, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Console.WriteLine($"  Consulta #{i + 1}  |  Data: {matrizConsultas[i, COL_DATA]}");
                    Console.WriteLine($"    Paciente      : {matrizConsultas[i, COL_NOME_PAC]} (CPF: {matrizConsultas[i, COL_CPF_PAC]})");
                    Console.WriteLine($"    Especialidade : {matrizConsultas[i, COL_ESPECIALID]}");
                    Console.WriteLine($"    Diagnostico   : {matrizConsultas[i, COL_DIAGNOSTICO]}");
                    Console.WriteLine();
                    encontrou = true;
                }
            }

            if (!encontrou)
                Console.WriteLine("  Nenhuma consulta encontrada para este medico.");

            Console.Write("\n  Pressione qualquer tecla...");
            Console.ReadKey();
        }

        // ------------------------------------------------------------
        // BuscarConsultasPorData
        //
        // Busca exata na coluna COL_DATA (formato DD/MM/AAAA).
        // ------------------------------------------------------------
        static void BuscarConsultasPorData()
        {
            Console.Clear();
            ExibirCabecalho("BUSCAR CONSULTAS POR DATA");

            string data = LerTexto("Data (DD/MM/AAAA)");
            bool encontrou = false;

            Console.WriteLine($"\n  Consultas em {data}:\n");

            for (int i = 0; i < totalConsultas; i++)
            {
                if (matrizConsultas[i, COL_DATA] == data)
                {
                    Console.WriteLine($"  Consulta #{i + 1}");
                    Console.WriteLine($"    Paciente      : {matrizConsultas[i, COL_NOME_PAC]} (CPF: {matrizConsultas[i, COL_CPF_PAC]})");
                    Console.WriteLine($"    Medico        : {matrizConsultas[i, COL_MEDICO]}");
                    Console.WriteLine($"    Especialidade : {matrizConsultas[i, COL_ESPECIALID]}");
                    Console.WriteLine($"    Diagnostico   : {matrizConsultas[i, COL_DIAGNOSTICO]}");
                    Console.WriteLine();
                    encontrou = true;
                }
            }

            if (!encontrou)
                Console.WriteLine("  Nenhuma consulta encontrada nesta data.");

            Console.Write("\n  Pressione qualquer tecla...");
            Console.ReadKey();
        }

        // ============================================================
        // SECAO 10: HISTORICO DE ACOES (PILHA / STACK)
        // ============================================================

        // ------------------------------------------------------------
        // ExibirHistorico
        //
        // ToArray() converte a pilha em array do TOPO para a BASE,
        // portanto acoes[0] eh sempre a acao mais recente.
        // ------------------------------------------------------------
        static void ExibirHistorico()
        {
            Console.Clear();
            ExibirCabecalho("HISTORICO DE ACOES  [PILHA / STACK]");

            if (historicoAcoes.Count == 0)
            {
                Console.WriteLine("  Nenhuma acao registrada ainda.\n");
                Console.Write("  Pressione qualquer tecla...");
                Console.ReadKey();
                return;
            }

            // ToArray retorna: [topo, ..., base]
            string[] acoes = historicoAcoes.ToArray();

            Console.WriteLine("  Ordem: mais recente primeiro (topo da pilha):\n");

            for (int i = 0; i < acoes.Length; i++)
            {
                // Destaca o topo da pilha
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

            Console.WriteLine($"\n  Total de acoes na pilha: {historicoAcoes.Count}");
            Console.Write("\n  Pressione qualquer tecla...");
            Console.ReadKey();
        }

        // ============================================================
        // SECAO 11: METODOS AUXILIARES (HELPERS)
        // ============================================================

        // ------------------------------------------------------------
        // BuscarIndicePorCpf
        //
        // Busca linear no vetor pacCpfs.
        // Retorna o indice do paciente ou -1 se nao encontrar.
        // Complexidade: O(n) no pior caso.
        // ------------------------------------------------------------
        static int BuscarIndicePorCpf(string cpf)
        {
            for (int i = 0; i < totalPacientes; i++)
            {
                if (pacCpfs[i] == cpf)
                    return i; // encontrado: retorna o indice
            }
            return -1; // nao encontrado
        }

        // ------------------------------------------------------------
        // ContarConsultasPorCpf
        //
        // Percorre a matriz e conta linhas cujo CPF bate com o parametro.
        // Usado para exibir o numero de consultas de um paciente.
        // ------------------------------------------------------------
        static int ContarConsultasPorCpf(string cpf)
        {
            int contador = 0;
            for (int i = 0; i < totalConsultas; i++)
            {
                if (matrizConsultas[i, COL_CPF_PAC] == cpf)
                    contador++;
            }
            return contador;
        }

        // ------------------------------------------------------------
        // RegistrarAcao
        //
        // Empilha (Push) uma string descritiva com data/hora atual
        // no topo da pilha de historico.
        // ------------------------------------------------------------
        static void RegistrarAcao(string descricao)
        {
            string acao = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] {descricao}";
            historicoAcoes.Push(acao); // PUSH: sempre vai para o TOPO
        }

        // ------------------------------------------------------------
        // LerTexto
        //
        // Le uma string nao vazia do usuario com validacao de campo.
        // Loop ate o usuario digitar algo valido.
        // ------------------------------------------------------------
        static string LerTexto(string campo)
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

        // ------------------------------------------------------------
        // LerInteiro
        //
        // Le um numero inteiro valido do usuario.
        // Usa int.TryParse para nao lancar excecao em entradas invalidas.
        // ------------------------------------------------------------
        static int LerInteiro(string campo)
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

        // ------------------------------------------------------------
        // ExibirCabecalho / ExibirRodape
        //
        // Padrao visual consistente para todas as telas do sistema.
        // ------------------------------------------------------------
        static void ExibirCabecalho(string titulo)
        {
            string linha = new string('=', 62);
            Console.WriteLine("  " + linha);
            Console.WriteLine($"  CLINICA SAUDE UNIARAXA");
            Console.WriteLine($"  {titulo}");
            Console.WriteLine("  " + linha);
            Console.WriteLine();
        }

        static void ExibirRodape()
        {
            Console.WriteLine("  " + new string('-', 62));
        }

        // ------------------------------------------------------------
        // MensagemSucesso / MensagemAviso
        //
        // Feedback visual com cores para o usuario.
        // Verde = operacao concluida com sucesso.
        // Amarelo = aviso, atencao ou erro de validacao.
        // ------------------------------------------------------------
        static void MensagemSucesso(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  [OK] {msg}");
            Console.ResetColor();
            Console.Write("  Pressione qualquer tecla para continuar...");
            Console.ReadKey();
        }

        static void MensagemAviso(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n  [!]  {msg}");
            Console.ResetColor();
            Console.Write("  Pressione qualquer tecla para continuar...");
            Console.ReadKey();
        }
    }
}
