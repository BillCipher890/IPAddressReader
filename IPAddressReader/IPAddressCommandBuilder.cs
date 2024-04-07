using System.CommandLine;
using System.Net;

namespace IPAddressReader
{
    public class IPAddressCommandBuilder
    {
        public static RootCommand CreateRootCommand(Action<FileInfo, FileInfo, DateTime, DateTime, IPAddress?, int?> Filter)
        {
            var fileLogOption = new Option<FileInfo>(
                name: "--file-log",
                description: "Путь к файлу с логами")
            {
                IsRequired = true
            };

            var fileOutputOption = new Option<FileInfo>(
                name: "--file-output",
                description: "Путь к файлу с результатом")
            {
                IsRequired = true
            };

            var timeStartOption = new Option<DateTime>(
            name: "--time-start",
            description: "Нижняя граница временного интервала")
            {
                IsRequired = true
            };

            var timeEndOption = new Option<DateTime>(
                name: "--time-end",
                description: "Верхняя граница временного интервала")
            {
                IsRequired = true
            };

            var addressStartOption = new Option<IPAddress?>(
                name: "--address-start",
                description: "Нижняя граница диапазона адресов",
                parseArgument: result =>
                {
                    if (result.Tokens.Count == 0) return null;
                    if (!IPAddress.TryParse(result.Tokens.Single().Value, out var address))
                    {
                        result.ErrorMessage = "Некорректный IP-адрес";
                    }
                    return address;
                });

            var addressMaskOption = new Option<int?>(
                name: "--address-mask",
                description: "Маска подсети",
                parseArgument: result =>
                {
                    if (result.Tokens.Count == 0) return null;
                    if (!int.TryParse(result.Tokens.Single().Value, out var mask) || mask < 0 || mask > 32)
                    {
                        result.ErrorMessage = "Маска подсети должна быть числом от 0 до 32";
                    }
                    return mask;
                });

            var rootCommand = new RootCommand
            {
                fileLogOption,
                fileOutputOption,
                timeStartOption,
                timeEndOption,
                addressStartOption,
                addressMaskOption
            };

            rootCommand.Description = "Обработка журнала доступа по IP-адресам";

            Handler.SetHandler(
                rootCommand,
                Filter,
                fileLogOption,
                fileOutputOption,
                timeStartOption,
                timeEndOption,
                addressStartOption,
                addressMaskOption);

            return rootCommand;
        }
    }
}
