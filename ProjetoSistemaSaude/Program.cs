using System;
using System.Collections.Generic;

namespace SistemaClinica
{
    class Program
    {
        static string[] nomes = new string[100];
        static int[] idades = new int[100];
        static string[] cpfs = new string[100];
        static int total = 0;

        static List<string> listaNomes = new List<string>();
        static Stack<string> pilhaAcoes = new Stack<string>();

        static void Main(string[] args)
        {
            int opcao;
            do
            {
                Console.Clear();
                Console.WriteLine("1 - Cadastrar Paciente");
                Console.WriteLine("2 - Listar Pacientes");
                Console.WriteLine("3 - Ver Historico");
                Console.WriteLine("0 - Sair");
                Console.Write("Opcao: ");
                opcao = int.Parse(Console.ReadLine());

                if (opcao == 1) Cadastrar();
                else if (opcao == 2) Listar();
                else if (opcao == 3) Historico();

            } while (opcao != 0);
        }

        static void Cadastrar()
        {
            Console.Clear();
            Console.Write("Nome: ");
            nomes[total] = Console.ReadLine();

            Console.Write("Idade: ");
            idades[total] = int.Parse(Console.ReadLine());

            Console.Write("CPF: ");
            cpfs[total] = Console.ReadLine();

            listaNomes.Add(nomes[total]);
            pilhaAcoes.Push("Cadastrou: " + nomes[total]);
            total++;

            Console.WriteLine("Paciente cadastrado!");
            Console.ReadKey();
        }

        static void Listar()
        {
            Console.Clear();
            if (total == 0)
            {
                Console.WriteLine("Nenhum paciente cadastrado.");
            }
            else
            {
                for (int i = 0; i < total; i++)
                {
                    Console.WriteLine($"Nome: {nomes[i]} | Idade: {idades[i]} | CPF: {cpfs[i]}");
                }
            }
            Console.ReadKey();
        }

        static void Historico()
        {
            Console.Clear();
            if (pilhaAcoes.Count == 0)
            {
                Console.WriteLine("Nenhuma acao registrada.");
            }
            else
            {
                string[] hist = pilhaAcoes.ToArray();
                for (int i = 0; i < hist.Length; i++)
                    Console.WriteLine((i == 0 ? "[TOPO] " : "       ") + hist[i]);
            }
            Console.ReadKey();
        }
    }
}