using System;
using System.IO;
using System.Linq;

/*
   Коментар
   коментар2
   коментар3
*/

namespace _7
{
    static class Variables
    {
        public static int DAYS_IN_MONTH = 30;
        public static bool RANDOM = false;
    }

    enum TypeOfWeather
    {
        NotDefined = 0,
        Rain = 1, // **
        ShortTermRain = 2,
        Thunderstorm = 3, // **
        Snow = 4,
        Fog = 5, // *
        Gloomy = 6,
        Sunny = 7
    }

    class WeatherDays
    {
        private WeatherParametersDay[] dataWeatherArray;

        public WeatherDays(WeatherParametersDay[] dataWeatherArray)
        {
            this.dataWeatherArray = dataWeatherArray;
        }

        public int CountRainOrThunderstormDays()
        {
            return CountDays(TypeOfWeather.Rain, TypeOfWeather.Thunderstorm);
        }

        public int CountFogDays()
        {
            return CountDays(TypeOfWeather.Fog);
        }

        private int CountDays(params TypeOfWeather[] typeOfWeather)
        {
            int counter = 0;
            foreach (WeatherParametersDay day in dataWeatherArray)
            {
                counter += (typeOfWeather.Contains(day.TypeOfWeather)) ? 1 : 0;
            }

            return counter;
        }

        public double AveragePressure()
        {
            double sumPressure = 0;
            foreach (WeatherParametersDay day in dataWeatherArray)
            {
                sumPressure += day.AverageAtmosphericPressure;
            }

            return sumPressure / dataWeatherArray.Length;
        }
    }

    class WeatherParametersDay
    {
        private double
            averageTemperatureDay,
            averageTemperatureNight,
            averageAtmosphericPressure,
            precipitation;

        private TypeOfWeather typeOfWeather;

        public double AverageTemperatureNight
        {
            get => averageTemperatureNight;
            private set => averageTemperatureNight = value;
        }

        public double AverageAtmosphericPressure
        {
            get => averageAtmosphericPressure;
            private set => averageAtmosphericPressure = value;
        }

        public double Precipitation
        {
            get => precipitation;
            private set => precipitation = value;
        }

        public TypeOfWeather TypeOfWeather
        {
            get => typeOfWeather;
            private set => typeOfWeather = value;
        }

        public WeatherParametersDay(double averageTemperatureDay, double averageTemperatureNight,
            double averageAtmosphericPressure, double precipitation, int typeOfWeather)
        {
            if (precipitation >= 0 &&
                averageAtmosphericPressure >= 0 &&
                Enumerable.Range(0, 7).Contains(typeOfWeather))
            {
                AverageTemperatureDay = averageTemperatureDay;
                AverageTemperatureNight = averageTemperatureNight;
                AverageAtmosphericPressure = averageAtmosphericPressure;
                Precipitation = precipitation;
                TypeOfWeather = (TypeOfWeather) typeOfWeather;
            }
        }
    }

    class Program
    {
        private static double[][] DataInput(string path)
        {
            string[] lines = new string[Variables.DAYS_IN_MONTH];

            bool exit = false;
            while (!exit)
            {
                switch (UserChoice())
                {
                    case ConsoleKey.F:
                        lines = ReadFile(path);
                        exit = true;
                        break;
                    case ConsoleKey.C:
                        ReadConsole(out lines);
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("\nНеизвестная кнопка");
                        break;
                }
            }

            return StringToDoubleArray(lines);
        }

        private static string[] ReadFile(string path) => File.ReadAllLines(path);

        private static void ReadConsole(out string[] lines)
        {
            Console.WriteLine("\nВведите данные для каждого дня в отдельной строке через пробел");
            lines = new string[Variables.DAYS_IN_MONTH];

            for (int j = 0; j < Variables.DAYS_IN_MONTH; j++)
            {
                while (true)
                {
                    lines[j] = Console.ReadLine();
                    try
                    {
                        if (lines[j].Equals(""))
                            throw new NullReferenceException();
                        break;
                    }
                    catch (NullReferenceException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        private static ConsoleKey UserChoice()
        {
            Console.WriteLine("F - читать данные из файла\n" +
                              "C - ввести данные из консоли");
            return Console.ReadKey().Key;
        }

        private static double[][] StringToDoubleArray(string[] lines)
        {
            double[][] data = new double[lines.Length][];
            for (int i = 0; i < lines.Length; i++)
            {
                string[] linesSplit = lines[i].Split();
                if (linesSplit.Length == 4)
                    data[i] = new double[linesSplit.Length + 1];
                else
                    data[i] = new double[linesSplit.Length];


                for (int j = 0; j < linesSplit.Length; j++)
                {
                    double num = 0;
                    try
                    {
                        num = Convert.ToDouble(linesSplit[j]);
                    }
                    catch (Exception ex) when (ex is FormatException || ex is InvalidCastException)
                    {
                        Console.WriteLine(ex.Message);
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                    }

                    data[i][j] = num;
                }
            }

            return data;
        }

        static void Main()
        {
            string path = @"..\..\..\src\data.txt";
            if (Variables.RANDOM)
                GenerateDataInFile.Run();
            double[][] data = DataInput(path);

            WeatherParametersDay[] weatherParametersDays = new WeatherParametersDay[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                weatherParametersDays[i] = new WeatherParametersDay(data[i][0],
                    data[i][1],
                    data[i][2],
                    data[i][3],
                    (int) data[i][4]);
            }

            WeatherDays weatherDays = new WeatherDays(weatherParametersDays);
            Console.WriteLine($"\nКоличество туманных дней: {weatherDays.CountFogDays()}");
            Console.WriteLine(
                $"Количество дней, когда был дождь или гроза: {weatherDays.CountRainOrThunderstormDays()}");
            Console.WriteLine($"Среднее атмосферное давление за месяц: {weatherDays.AveragePressure()}");
        }
    }
}