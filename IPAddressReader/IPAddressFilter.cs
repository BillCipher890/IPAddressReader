using System.CommandLine;
using System.Globalization;
using System.Net;

namespace IPAddressReader
{
    public class IPAddressFilter
    {
        private readonly string logDateFormat = "yyyy-MM-dd HH:mm:ss";
        private readonly RootCommand command;

        public IPAddressFilter()
        {
            command = IPAddressCommandBuilder.CreateRootCommand(Filter);
        }

        public void ExecuteFilter(string[] args)
        {
            command.Invoke(args);
        }

        private void Filter(FileInfo fileLog, FileInfo fileOutput, DateTime timeStart, DateTime timeEnd, IPAddress? addressStart, int? addressMask)
        {
            try
            {
                if (timeStart > timeEnd)
                    throw new Exception("Нижняя граница даты не может превышать верхнюю.");

                string data = ReadLogFile(fileLog);
                var dataDictionary = SplitDataToDictionary(data);
                var filteredData = FilterByDate(dataDictionary, timeStart, timeEnd);

                if (addressStart != null)
                    filteredData = FilterByAddressStartAndMask(filteredData, addressStart, addressMask);

                SaveData(fileOutput, filteredData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static string ReadLogFile(FileInfo fileLog)
        {
            try
            {
                using StreamReader reader = fileLog.OpenText();
                var dataFromFile = reader.ReadToEnd();
                return dataFromFile;
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                throw new Exception($"Файл или папка не существуют по данному пути: {fileLog.FullName}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Возникла непредвиденная ошибка при загрузке файла. {ex.Message}");
            }
        }

        private Dictionary<DateTime, IPAddress> SplitDataToDictionary(string data)
        {
            return data
                .Split("\r\n")
                .Select(s =>
                {
                    int index = s.IndexOf(':');
                    if (index == -1)
                        return null;

                    if (!IPAddress.TryParse(s[..index], out var ipAddress))
                        return null;

                    if (!DateTime.TryParseExact(s[(index + 1)..], logDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                        return null;

                    return new { CallDate = date, IPAdderss = ipAddress };
                })
                .Where(pair => pair != null)
                .ToDictionary(pair => pair.CallDate, pair => pair.IPAdderss);
        }

        private static IEnumerable<KeyValuePair<DateTime, IPAddress>> FilterByDate(Dictionary<DateTime, IPAddress> data, DateTime timeStart, DateTime timeEnd)
        {
            return data.Where(pair => pair.Key >= timeStart && pair.Key <= timeEnd);
        }

        private static IEnumerable<KeyValuePair<DateTime, IPAddress>> FilterByAddressStartAndMask(IEnumerable<KeyValuePair<DateTime, IPAddress>> data, IPAddress addressStart, int? addressMask)
        {
            return data
                .Where(pair =>
                {
                    var ipBytes = pair.Value.GetAddressBytes();
                    var ipAddressValue = BitConverter.ToInt32(ipBytes, 0);

                    var lowerBytes = addressStart.GetAddressBytes();

                    if (ipBytes.Length != lowerBytes.Length)
                        return false;

                    var lowerBoundValue = BitConverter.ToInt32(addressStart.GetAddressBytes(), 0);
                    if (lowerBoundValue > ipAddressValue)
                        return false;


                    if (addressMask != null)
                    {
                        int mask = -1 << (32 - addressMask.Value);

                        for (int i = 0; i < ipBytes.Length; i++)
                        {
                            if ((ipBytes[i] & mask) != (lowerBytes[i] & mask))
                                return false;
                        }
                    }

                    return true;
                });
        }

        private static void SaveData(FileInfo fileOutput, IEnumerable<KeyValuePair<DateTime, IPAddress>> filteredData)
        {
            try
            {
                if (File.Exists(fileOutput.FullName))
                    File.Delete(fileOutput.FullName);

                File.WriteAllLines(fileOutput.FullName, filteredData.Select(pair => $"{pair.Value}:{pair.Key}"));
            }
            catch (DirectoryNotFoundException)
            {
                throw new Exception("Папки не существует по указанному пути.");
            }
            catch (PathTooLongException)
            {
                throw new Exception("Слишком длинное имя файла."); 
            }
            catch (Exception)
            {
                throw new Exception("Произошла непредвиденная ошибка при сохранении результата.");
            }
        }
    }
}
