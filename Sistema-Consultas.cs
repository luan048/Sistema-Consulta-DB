using System;
using MySql.Data.MySqlClient;
using DotNetEnv;

class Consultas
{
    static string conexaoString = Environment.GetEnvironmentVariable("Dados-ConexaoDB");

    static async Task Main(string[] args)
    {
        Env.Load();

        await ConectarDB();
        Thread.Sleep(1500);
        await Gerenciador();
    }

    static async Task ConectarDB()
    {
        Console.Clear();

        using (MySqlConnection conexao = new MySqlConnection(conexaoString))
        {
            try
            {
                await conexao.OpenAsync();
                Console.WriteLine("Conexão com Banco de Dados concluida com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao conectar com Banco de Dados: " + ex);
            }
        }
    }

    static async Task Gerenciador()
    {
        Console.Clear();
        Console.WriteLine("O que você deseja fazer?");
        Console.WriteLine("[1] - Criar consulta");
        Console.WriteLine("[2] - Listar consultas");
        int opcao = int.Parse(Console.ReadLine());

        switch (opcao)
        {
            case 1:
                await AdicionarConsulta();
                break;
            case 2:
                await ListarConsultas();
                break;
            default:
                Console.Clear();
                Console.WriteLine("Opção inválida. Tente novamente...");
                break;
        }
    }

    static async Task AdicionarConsulta()
    {
        Console.Write("Digite seu nome: ");
        string nome = Console.ReadLine();

        Console.Write("Digite a data da consulta que você deseja: ");
        string data = Console.ReadLine();

        using (MySqlConnection conexao = new MySqlConnection(conexaoString))
        {
            await conexao.OpenAsync();

            string sql = "INSERT INTO consultas (nome_cliente, data) VALUES (@nome, @data)";
            using (MySqlCommand cmd = new MySqlCommand(sql, conexao))
            {
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@data", data);

                try
                {
                    int linhasAfetadas = await cmd.ExecuteNonQueryAsync();
                    Console.WriteLine($"{linhasAfetadas} registros inseridos com sucesso.");
                    Thread.Sleep(1500);

                    Console.Clear();
                    Console.WriteLine("Digite 1 para voltar");
                    int opcao = int.Parse(Console.ReadLine());

                    if (opcao == 1)
                    {
                        await Gerenciador();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao adicionar elemento no banco de dados: " + ex);
                }
            }
        }
    }

    static async Task ListarConsultas()
    {
        Console.Clear();

        string conexaoString = Environment.GetEnvironmentVariable("Dados-ConexaoDB");

        using (MySqlConnection conexao = new MySqlConnection(conexaoString))
        {
            await conexao.OpenAsync();

            string sql = "SELECT * FROM consultas";
            using (MySqlCommand cmd = new MySqlCommand(sql, conexao))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                Console.WriteLine("Registros encontrados:");
                Thread.Sleep(1500);

                while (await reader.ReadAsync())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string nomeColuna = reader.GetName(i);
                        object valor = reader.GetValue(i);

                        Console.Write($"{nomeColuna}: {valor}  ");
                    }

                    Console.WriteLine();
                }
            }
        }
    }


}