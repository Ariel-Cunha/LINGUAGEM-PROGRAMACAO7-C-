using System.Globalization;
using System.Text;

namespace SistemaFaturacao
{
    public class Program
    {

        //Regras da loja
        const int MAX_CLIENTES = 100;
        const decimal LIMITE_BONUS = 100000m;     // bónus só acima deste valor
        const decimal PERCENTAGEM_BONUS = 0.10m;  // 10%
        const int IDADE_IDOSO = 60;               // idade mínima para prioridade

        //Dados dos clientes
        static string[] nomes = new string[MAX_CLIENTES];
        static string[] generos = new string[MAX_CLIENTES];
        static int[] idades = new int[MAX_CLIENTES];
        static decimal[] totais = new decimal[MAX_CLIENTES];
        static decimal[] bonus = new decimal[MAX_CLIENTES];
        static decimal[] totaisPagar = new decimal[MAX_CLIENTES];
        static bool[] gravida = new bool[MAX_CLIENTES];
        static bool[] idoso = new bool[MAX_CLIENTES];
        static bool[] deficiente = new bool[MAX_CLIENTES];
        static bool[] prioritario = new bool[MAX_CLIENTES];
        static bool[] atendido = new bool[MAX_CLIENTES];

        static int quantidade = 0;      // clientes registados
        static int proximaFatura = 1;

        // Formato português: 150 000,50
        static CultureInfo pt = new CultureInfo("pt-PT");


        static void Main(string[] args)
        {
            int opcao;
            do
            {
                MostrarMenu();
                opcao = LerInteiro("Escolha uma opção: ", 0, 4);
                Console.WriteLine();

                switch (opcao)
                {
                    case 1: RegistarCliente(); break;
                    case 2: MostrarFila(); break;
                    case 3: AtenderProximo(); break;
                    case 4: MostrarResumo(); break;
                    case 0: Console.WriteLine("Programa encerrado."); break;
                }
            } while (opcao != 0);
        }

        //Menu
        static void MostrarMenu()
        {
            Console.WriteLine();
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("  LOJA DE PRODUTOS ALIMENTARES - FATURAÇÃO");
            Console.WriteLine("_--------------------------------------------");
            Console.WriteLine("1. Registar cliente");
            Console.WriteLine("2. Ver fila de espera");
            Console.WriteLine("3. Atender próximo cliente");
            Console.WriteLine("4. Resumo do dia");
            Console.WriteLine("0. Sair");
        }

        //Registãr cliente
        static void RegistarCliente()
        {
            if (quantidade >= MAX_CLIENTES)
            {
                Console.WriteLine("Lista cheia. Atenda clientes primeiro.");
                return;
            }

            int i = quantidade;

            Console.WriteLine("REGISTO DE CLIENTE");
            nomes[i] = LerTexto("Nome do cliente: ");
            generos[i] = LerGenero();
            idades[i] = LerInteiro("Idade: ", 0, 120);
            totais[i] = LerDecimal("Valor total das compras (Kz): ");

            if (generos[i] == "F")
            {
                gravida[i] = LerSimNao("A cliente está grávida? (S/N): ");
            }
            else
            {
                gravida[i] = false;
            }
            deficiente[i] = LerSimNao("Tem alguma deficiência? (S/N): ");

            //Prioridade de atendimento
            idoso[i] = idades[i] >= IDADE_IDOSO;
            prioritario[i] = gravida[i] || idoso[i] || deficiente[i];

            //Bónus comercial
            bonus[i] = CalcularBonus(totais[i]);
            totaisPagar[i] = totais[i] - bonus[i];

            atendido[i] = false;
            quantidade++;

            Console.WriteLine();
            Console.WriteLine("Cliente registado com sucesso.");
            Console.WriteLine("  Valor das compras : " + Kz(totais[i]));
            if (bonus[i] > 0)
            {
                Console.WriteLine("  Bónus de 10%      : Sim (" +
                    Kz(bonus[i]) + ")");
            }
            else
            {
                Console.WriteLine("  Bónus de 10%      : Não");
            }
            Console.WriteLine("  Prioridade        : " + DescreverPrioridade(i));
        }

        //Cálculos
        static decimal CalcularBonus(decimal total)
        {
            if (total > LIMITE_BONUS)
            {
                return total * PERCENTAGEM_BONUS;
            }
            return 0;
        }

        static string DescreverPrioridade(int i)
        {
            string texto = "";

            if (gravida[i])
            {
                texto = "Grávida";
            }
            if (idoso[i])
            {
                if (texto != "") texto = texto + ", ";
                texto = texto + "Pessoa idosa";
            }
            if (deficiente[i])
            {
                if (texto != "") texto = texto + ", ";
                texto = texto + "Pessoa com deficiência";
            }
            if (texto == "")
            {
                texto = "Sem prioridade";
            }
            return texto;
        }


        //Fila de espera
        static int ProcurarProximo()
        {
            for (int i = 0; i < quantidade; i++)
            {
                if (!atendido[i] && prioritario[i]) return i;
            }
            for (int i = 0; i < quantidade; i++)
            {
                if (!atendido[i]) return i;
            }
            return -1;
        }

        static void MostrarFila()
        {
            int posicao = 0;

            Console.WriteLine("FILA DE ESPERA por ordem de atendimento");

            for (int i = 0; i < quantidade; i++)
            {
                if (!atendido[i] && prioritario[i])
                {
                    posicao++;
                    Console.WriteLine(posicao + ". " + nomes[i] +
                        " - Prioridade (" + DescreverPrioridade(i) + ")");
                }
            }
            for (int i = 0; i < quantidade; i++)
            {
                if (!atendido[i] && !prioritario[i])
                {
                    posicao++;
                    Console.WriteLine(posicao + ". " + nomes[i] + " - Normal");
                }
            }

            if (posicao == 0)
            {
                Console.WriteLine("Não há clientes à espera.");
            }
        }

        static void AtenderProximo()
        {
            int i = ProcurarProximo();

            if (i == -1)
            {
                Console.WriteLine("Não há clientes à espera.");
                return;
            }

            ImprimirFatura(i);
            atendido[i] = true;
        }

        static void ImprimirFatura(int i)
        {
            string genero;
            if (generos[i] == "F") genero = "Feminino";
            else genero = "Masculino";

            Console.WriteLine("            FATURA N.º " +
                proximaFatura.ToString("000"));

            Console.WriteLine("Data         : " +
                DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            Console.WriteLine("Cliente      : " + nomes[i]);
            Console.WriteLine("Género       : " + genero);
            Console.WriteLine("Atendimento  : " + DescreverPrioridade(i));
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("Total das compras : " + Kz(totais[i]));
            if (bonus[i] > 0)
            {
                Console.WriteLine("Bónus de 10%      : - " + Kz(bonus[i]));
            }
            else
            {
                Console.WriteLine("Bónus de 10%      : " + Kz(0));
            }
            Console.WriteLine("TOTAL A PAGAR     : " + Kz(totaisPagar[i]));
            Console.WriteLine("---------------------------------------------");

            if (bonus[i] > 0)
            {
                Console.WriteLine("Bónus atribuído: compras superiores a " +
                    Kz(LIMITE_BONUS) + ".");
            }
            else
            {
                Console.WriteLine("Sem bónus: compras não superiores a " +
                    Kz(LIMITE_BONUS) + ".");
            }

            proximaFatura++;
        }

        //Resumo
        static void MostrarResumo()
        {
            int totalAtendidos = 0;
            int emEspera = 0;
            int comBonus = 0;
            decimal faturado = 0;
            decimal somaBonus = 0;

            for (int i = 0; i < quantidade; i++)
            {
                if (atendido[i])
                {
                    totalAtendidos++;
                    faturado = faturado + totaisPagar[i];
                    somaBonus = somaBonus + bonus[i];
                    if (bonus[i] > 0) comBonus++;
                }
                else
                {
                    emEspera++;
                }
            }

            Console.WriteLine("RESUMO DO DIA");
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("Clientes registados     : " + quantidade);
            Console.WriteLine("Clientes atendidos      : " + totalAtendidos);
            Console.WriteLine("Clientes em espera      : " + emEspera);
            Console.WriteLine("Clientes com bónus      : " + comBonus);
            Console.WriteLine("Total em bónus          : " + Kz(somaBonus));
            Console.WriteLine("Total faturado          : " + Kz(faturado));
        }

        //Funções de leitura com validação 
        static string LerTexto(string mensagem)
        {
            string texto;
            do
            {
                Console.Write(mensagem);
                texto = Console.ReadLine().Trim();
                if (texto == "")
                {
                    Console.WriteLine("O campo não pode ficar vazio.");
                }
            } while (texto == "");
            return texto;
        }

        static int LerInteiro(string mensagem, int minimo, int maximo)
        {
            int valor;
            while (true)
            {
                Console.Write(mensagem);
                string texto = Console.ReadLine();
                if (int.TryParse(texto, out valor) &&
                    valor >= minimo && valor <= maximo)
                {
                    return valor;
                }
                Console.WriteLine("Valor inválido. Digite um número entre " +
                    minimo + " e " + maximo + ".");
            }
        }

        static decimal LerDecimal(string mensagem)
        {
            decimal valor;
            while (true)
            {
                Console.Write(mensagem);
                string texto = Console.ReadLine();
                if (decimal.TryParse(texto, NumberStyles.Number, pt, out valor) &&
                    valor > 0)
                {
                    return valor;
                }
                Console.WriteLine("Valor inválido. Exemplo: 25000 ou 25000,50");
            }
        }

        static string LerGenero()
        {
            while (true)
            {
                Console.Write("Género (M/F): ");
                string g = Console.ReadLine().Trim().ToUpper();
                if (g == "M" || g == "F")
                {
                    return g;
                }
                Console.WriteLine("Responda apenas com M ou F.");
            }
        }

        static bool LerSimNao(string mensagem)
        {
            while (true)
            {
                Console.Write(mensagem);
                string resposta = Console.ReadLine().Trim().ToUpper();
                if (resposta == "S") return true;
                if (resposta == "N") return false;
                Console.WriteLine("Responda apenas com S ou N.");
            }
        }

        //formatação de valores
        static string Kz(decimal valor)
        {
            return valor.ToString("N2", pt) + " Kz";
        }


    }
}
