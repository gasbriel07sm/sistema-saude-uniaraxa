
public static class Dados
{

    public const int MAX_PACIENTES = 100;
    public const int MAX_CONSULTAS = 200;
    public const int COLUNAS_CONSULTA = 6;


    public const int COL_CPF_PAC = 0;
    public const int COL_NOME_PAC = 1;
    public const int COL_DATA = 2;
    public const int COL_MEDICO = 3;
    public const int COL_ESPECIALID = 4;
    public const int COL_DIAGNOSTICO = 5;


    public static string[] PacNomes = new string[MAX_PACIENTES];
    public static string[] PacCpfs = new string[MAX_PACIENTES];
    public static int[] PacIdades = new int[MAX_PACIENTES];
    public static string[] PacTelefones = new string[MAX_PACIENTES];
    public static string[] PacTipoSang = new string[MAX_PACIENTES];
    public static int TotalPacientes = 0;


    public static Queue<string> FilaEspera = new Queue<string>();


    public static string[,] MatrizConsultas = new string[MAX_CONSULTAS, COLUNAS_CONSULTA];
    public static int TotalConsultas = 0;


    public static Stack<string> HistoricoAcoes = new Stack<string>();
}
