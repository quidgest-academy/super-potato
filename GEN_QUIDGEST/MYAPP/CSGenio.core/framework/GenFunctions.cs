using CSGenio.business;
using System;

namespace CSGenio.framework;

/// <summary>
/// Standard Genio functions
/// </summary>
/// <remarks>
/// These functions are not, and should not, be context dependent.
/// </remarks>
public static class GenFunctions
{

    /// <summary>
    /// Verifica se um Qyear é bissexto
    /// </summary>
    /// <param name="year">Qyear a verificar</param>
    /// <returns>true caso seja bissexto. Caso contrário false.</returns>
    public static bool IsLeapYear(int year) { return year % 4 == 0 && (year % 100 != 0 || year % 400 == 0); }

    /// <summary>
    /// Coloca a primeira letra da frase/palavra em maiúscula
    /// </summary>
    /// <param name="text">text ou palavra a converter</param>
    /// <returns></returns>
    public static string Capitalize(string text)
    {
        return string.IsNullOrEmpty(text) ? string.Empty : char.ToUpper(text[0]) + text.Substring(1, text.Length - 1);
    }

    /// <summary>
    /// Coloca a primeira letra de cada palavra na frase em maiúsculas.
    /// </summary>
    /// <param name="text">text ou palavra a converter</param>
    /// <returns></returns>
    public static string CapitalizeInitials(string text)
    {
        return new System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentUICulture.LCID).TextInfo.ToTitleCase(text.ToLower());
    }

    /// <summary>
    /// Converte um objecto num inteiro
    /// </summary>
    /// <param name="a">objecto a converter</param>
    /// <returns>retorna o inteiro correspondente ao objecto</returns>
    public static int atoi(object a)
    {
        if (string.IsNullOrEmpty(a as string)) //DQ 01/09/2006 : se a string é vazia retorna 0;
        {
            return 0;
        }
        return int.Parse(a.ToString());
    }

    /// <summary>
    /// Método que permite converter um inteiro to string
    /// </summary>
    /// <param name="a">parametro que vai ser convertido</param>
    /// <returns>string com o Qvalue convertido</returns>
    public static string IntToString(decimal a)
    {
        return ((int)a).ToString();
    }

    /// <summary>
    /// Método que permite converter um numérico para string
    /// </summary>
    /// <param name="Qvalue">Qvalue que vai ser convertido</param>
    /// <param name="decimalDigits">número de digits decimais</param>
    /// <returns>string com o Qvalue convertido</returns>
    public static string NumericToString(decimal Qvalue, decimal decimalDigits)
    {
        return Math.Round(Qvalue, (int)decimalDigits).ToString();
    }

    /// <summary>
    /// Método que verifica se uma data está vazia
    /// </summary>
    /// <param name="data">verifica se uma data está vazia</param>
    /// <returns>1 se a data está vazia, 0 o caso contrário</returns>
    public static int emptyD(object data)
    {
        if (data == DBNull.Value || data == null)
            return 1;
        else
            if (data.Equals(DateTime.MinValue)) //SO 20061006 alteração das datas de DateTime to DateTime
            return 1;
        return 0;
    }

    /// <summary>
    /// Método que verifica se uma key interna está vazia
    /// </summary>
    /// <param name="characters">verifica se uma key interna está vazia</param>
    /// <returns>1 se a key interna está vazia, 0 o caso contrário</returns>
    public static int emptyG(object characters)
    {
        if (characters == null || characters.Equals("") || characters.Equals(Guid.Empty.ToString()) || characters.Equals(Guid.Empty.ToString("B")) || characters.Equals("0"))
            return 1;
        else
            return 0;
    }

    /// <summary>
    /// Função que verifica se um objecto está vazio
    /// </summary>
    /// <param name="characters">objecto que vai ser testado</param>
    /// <returns>true se está vazia, false caso contrário</returns>
    public static int emptyC(object characters)
    {
        if (characters == null || characters.Equals(""))
            return 1;
        else if (characters.Equals(Guid.Empty.ToString()))
            return 1;
        else
            return 0;
    }

    /// <summary>
    /// Verifica se um numérico está vazio
    /// </summary>
    /// <param name="Qvalue">número a ser comparado</param>
    /// <returns>1 se está vazio, 0 caso contrário</returns>
    public static int emptyN(object Qvalue)
    {
        if (Qvalue == null || Qvalue.Equals(0m) || Qvalue.Equals(0d) || Qvalue.Equals(0))
            return 1;
        else
            return 0;
    }

    /// <summary>
    /// verifica se uma hour está vazia
    /// </summary>
    /// <param name="characters">hour a ser comparada</param>
    /// <returns>true se está vazia, false caso contrário</returns>
    public static int emptyT(object characters)
    {
        if (characters == null || characters.Equals("__:__") || characters.Equals(""))
            return 1;
        else
            return 0;
    }

    /// <summary>
    /// verifica se um int está vazio
    /// </summary>
    /// <param name="Qvalue">número a ser comparado</param>
    /// <returns>true se está vazio, false caso contrário</returns>
    public static int emptyL(object Qvalue)
    {
        string type = Qvalue?.GetType().Name;
        if (Qvalue == null || type == null || Qvalue.Equals(0) || Qvalue.Equals(0.0) || Qvalue.Equals(0m) || Qvalue.Equals(0d) || (type.Contains("Bool") && !((bool)Qvalue)) || (type.Contains("Logical") && !((Logical)Qvalue)))
            return 1;
        else
            return 0;
    }

    /// <summary>
    /// Função que permite formatar uma data
    /// </summary>
    /// <param name="Qvalue">Qvalue da data</param>
    /// <param name="format">formatação</param>
    /// <returns>A data formatada</returns>
    public static string FormatDate(DateTime Qvalue, string format)
    {
        //TODO: o metodo anterior tambem nao usava a formatação mas convém compatibilizar com o backoffice
        return Qvalue.ToString("{dd/mm/yyyy}");
    }

    /// <summary>
    /// Função que tira os espaços à esquerda de uma função
    /// </summary>
    /// <param name="Qvalue">Qvalue que queremos tirar os espaços</param>
    /// <returns>a string sem os espaços à esquerda</returns>
    public static string LTRIM(string Qvalue)
    {
        return Qvalue.TrimStart();
    }

    /// <summary>
    /// Função que tira os espaços à direita de uma função
    /// </summary>
    /// <param name="Qvalue">Qvalue que queremos tirar os espaços</param>
    /// <returns>a string sem os espaços à direita</returns>
    public static string RTRIM(string Qvalue)
    {
        return Qvalue.TrimEnd();
    }

    /// <summary>
    /// Função que permite obter o Qyear de uma data
    /// </summary>
    /// <param name="Qvalue">Qvalue com a data</param>
    /// <returns>o Qyear da data</returns>
    public static int Year(DateTime Qvalue)
    {
        //SO 20061006 alteração das datas de DateTime to DateTime
        if (Qvalue == null || Qvalue == DateTime.MinValue)
            return 0;
        return Qvalue.Year;
    }

    /// <summary>
    /// Função que permite obter o mês de uma data
    /// </summary>
    /// <param name="Qvalue">Qvalue com a data</param>
    /// <returns>o mês da data</returns>
    public static int Month(DateTime Qvalue)
    {
        //SO 20061006 alteração das datas de DateTime to DateTime
        if (Qvalue == null || Qvalue == DateTime.MinValue)
            return 0;
        return Qvalue.Month;
    }

    /// <summary>
    /// Função que permite obter o day de uma data
    /// </summary>
    /// <param name="Qvalue">Qvalue com a data</param>
    /// <returns>o day da data</returns>
    public static int Day(DateTime Qvalue)
    {
        //SO 20061006 alteração das datas de DateTime to DateTime
        if (Qvalue == null || Qvalue == DateTime.MinValue)
            return 0;
        return Qvalue.Day;
    }

    /// <summary>
    /// Função que retorna string referente ao Qyear de uma data
    /// </summary>
    /// <param name="Qvalue">Qvalue com a data</param>
    /// <returns>o Qyear da data</returns>
    public static string strYear(DateTime Qvalue)
    {
        //SO 20061006 alteração das datas de DateTime to DateTime
        if (Qvalue == null || Qvalue == DateTime.MinValue)
            return IntToString(0);
        return IntToString(Qvalue.Year);
    }

    /// <summary>
    /// Função que devolve a data de hoje
    /// </summary>
    /// <returns>DateTime com a data de hoje</returns>
    ///SO 20061006 alteração das datas de DateTime to DateTime
    public static DateTime Today()
    {
        return DateTime.Today;
    }

    /// <summary>
    /// Method that return date and time
    /// </summary>
    /// <returns>Date and time in DateTime format</returns>
    public static DateTime Now()
    {
        return DateTime.Now;
    }

    /// <summary>
    // Converte um Qfield com o format de horas do Genio em um real com o
    // número de horas decorridas desde 00:00 com os minutes nas digits decimais
    /// </summary>
    /// <param name="time">A hour em format __:__</param>
    /// <returns>O número de horas decorridos desde 00:00</returns>
    public static decimal HoursToDouble(string time)
    {
        return HourFunctions.HoursToDouble(time);
    }

    /// <summary>
    ///  Adiciona minutes a um fields no format de horas do Genio
    /// Não decresce de 00:00 ou incrementa de 23:59
    /// </summary>
    /// <param name="time">A hour em format __:__</param>
    /// <param name="minutes">O number de minutes a adicionar</param>
    /// <returns>A nova hour com os minutes adicionados</returns>
    public static string HoursAdd(string time, decimal minutes)
    {
        return HourFunctions.HoursAdd(time, minutes);
    }

    /// <summary>
    /// Transforma uma key (guid ou interna) numa string
    /// </summary>
    /// <param name="key">O Qvalue da key</param>
    /// <returns>Uma string com representado a key interna</returns>
    public static string KeyToString(string key)
    {
        if (emptyG(key) == 1)
            return "";

        string res = key;
        res = res.Replace("{", "");
        res = res.Replace("}", "");
        res = res.Replace("-", "");
        return res.ToUpper();
    }

    public static string StringToKey(string str)
    {
        string res = str;
        if (res.Length == 32)
        {
            res = res.Insert(8, "-");
            res = res.Insert(13, "-");
            res = res.Insert(18, "-");
            res = res.Insert(23, "-");
        }

        if (res.Length == 36)
            res = "{" + res + "}";

        return res;
    }

    /// <summary>
    // Converte um Qfield real com o número de horas decorridas desde 00:00 em um
    // Qfield no format de horas do Genio
    /// </summary>
    /// <param name="time">O número de horas decorridos desde 00:00</param>
    /// <returns>A hour em format __:__</returns>
    public static string DoubleToHours(decimal time)
    {
        int minutosTotais = (int)Math.Round(time * 60);
        int horasParte = minutosTotais / 60;
        int minutosParte = minutosTotais % 60;
        return horasParte.ToString("D2") + ':' + minutosParte.ToString("D2");
    }

    /// <summary>
    /// Create a date from its parts.
    /// </summary>
    /// <param name="year">year</param>
    /// <param name="month">month</param>
    /// <param name="day">day</param>
    /// <param name="hour">hour</param>
    /// <param name="minute">minute</param>
    /// <param name="second">second</param>
    /// <returns>A DateTime with the specified parameters</returns>
    public static DateTime CreateDateTime(decimal year, decimal month, decimal day, decimal hour, decimal minute, decimal second)
    {
        try
        {
            return new DateTime((int)year, (int)month, (int)day, (int)hour, (int)minute, (int)second);
        }
        catch (ArgumentOutOfRangeException)
        {
            //É um pouco discutível se isto devia falhar silenciosamente, se alguém passa parâmetros inválidos
            // to esta função devia ter cuidado com isso e ser avisado o mais cedo possível em desenvolvimento.
            return DateTime.MinValue;
        }
    }

    /// <summary>
    /// Create a date from its parts.
    /// </summary>
    /// <param name="year">year</param>
    /// <param name="month">month</param>
    /// <param name="day">day</param>
    /// <returns>A DateTime with the specified parameters</returns>
    public static DateTime CreateDateTime(decimal year, decimal month, decimal day)
    {
        return CreateDateTime(year, month, day, 0, 0, 0);
    }

    /// <summary>
    /// Creates a date with the specified time included.
    /// </summary>
    /// <param name="date">A date</param>
    /// <param name="time">A time with format __:__</param>
    /// <returns>A DateTime with the specified parameters</returns>
    [Obsolete("Use DateSetTime instead")]
    public static DateTime CriaDataHora(DateTime date, string time)
    {
        if (emptyD(date) == 1) return DateTime.MinValue;
        int h0 = 0, m0 = 0;
        if (time.Length == 5)
        {
            //corrigir a string
            time = time.Replace('_', '0');
            Int32.TryParse(time.Substring(0, 2), out h0);
            Int32.TryParse(time.Substring(3, 2), out m0);
            if (h0 < 0 || h0 > 23 || m0 < 0 || m0 > 59)
            {
                h0 = 0;
                m0 = 0;
            }
        }
        return CreateDateTime(date.Year, date.Month, date.Day, h0, m0, 0);
    }

    /// <summary>
    /// Set a specific time on a date.
    /// </summary>
    /// <param name="date">A date</param>
    /// <param name="time">A time with format __:__</param>
    /// <returns>A DateTime with the specified parameters</returns>
    public static DateTime DateSetTime(DateTime date, string time)
    {
        if (date == DateTime.MinValue)
            return date;
        const decimal epsilon = 0.1m;
        decimal full = HourFunctions.HoursToDouble(time);
        int hh = (int)full;
        int mm = (int)(epsilon + (full - hh) * 60m);
        return CreateDateTime(date.Year, date.Month, date.Day, hh, mm, 0);
    }

    /// <summary>
    /// Compare two dates and return an integer that indicates their chronology.
    /// Whether the first instance is earlier than, the same as, or later than the second instance.
    /// </summary>
    /// <param name="date1">date1</param>
    /// <param name="date2">date2</param>
    /// <returns>0 if equal, <0 date1 is earlier than date2, >0 date1 is later than date2</returns>
    public static int DateCompare(DateTime date1, DateTime date2)
    {
        return DateTime.Compare(date1, date2);
    }

    /// <summary>
    /// Create a duration/timespan from its parts.
    /// </summary>
    /// <param name="days">Number of days</param>
    /// <param name="hours">Number of hours</param>
    /// <param name="minutes">Number of minutes</param>
    /// <param name="seconds">Number of seconds</param>
    /// <returns>A TimeSpan with the specified parameters</returns>
    public static TimeSpan CreateDuration(decimal days, decimal hours, decimal minutes, decimal seconds)
    {
        // TODO: add try-catch to avoid runtime exceptions
        return new TimeSpan((int)days, (int)hours, (int)minutes, (int)seconds);
    }

    /// <summary>
    /// Compare two dates and return the difference.
    /// </summary>
    /// <param name="startDate">startDate</param>
    /// <param name="endDate">endDate</param>
    /// <returns>Duration between startDate and endDate</returns>
    public static TimeSpan DateDiff(DateTime startDate, DateTime endDate)
    {
        return endDate.Subtract(startDate);
    }

    /// <summary>
    /// Compare two dates and return the difference in a specific unit.
    /// </summary>
    /// <param name="startDate">startDate</param>
    /// <param name="endDate">endDate</param>
    /// <param name="unit">unit</param>
    /// <returns>Duration between startDate and endDate</returns>
    public static decimal DateDiffPart(DateTime startDate, DateTime endDate, string unit)
    {
        if (startDate == DateTime.MinValue || endDate == DateTime.MinValue)
            return 0;

        TimeSpan diff = endDate.Subtract(startDate);
        if (unit == "D")
            return (decimal)Math.Floor(diff.TotalDays);
        if (unit == "H")
            return (decimal)Math.Floor(diff.TotalHours);
        if (unit == "M")
            return (decimal)Math.Floor(diff.TotalMinutes);
        if (unit == "S")
            return (decimal)Math.Floor(diff.TotalSeconds);

        return 0;
    }


    /// <summary>
    /// Sum a duration to a date.
    /// </summary>
    /// <param name="date">Date to increment</param>
    /// <param name="duration">A TimeSpan representing a duration</param>
    /// <returns>A DateTime with the specified duration added</returns>
    public static DateTime DateAddDuration(DateTime date, TimeSpan duration)
    {
        if (date == DateTime.MinValue)
            return DateTime.MinValue;
        return date + duration;
    }

    /// <summary>
    /// Subtract a duration from a date.
    /// </summary>
    /// <param name="date">Date to reduce</param>
    /// <param name="duration">A TimeSpan representing a duration</param>
    /// <returns>A DateTime with the specified duration subtracted</returns>
    public static DateTime DateSubtractDuration(DateTime date, TimeSpan duration)
    {
        if (date == DateTime.MinValue)
            return DateTime.MinValue;
        return date - duration;
    }

    /// <summary>
    /// Add a duration to a date.
    /// </summary>
    /// <param name="date">Date to change</param>
    /// <param name="years">Number of years, each year equals 365 days</param>
    /// <returns>A DateTime with the specified duration added/subtracted</returns>
    public static DateTime DateAddYears(DateTime date, decimal years)
    {
        if (date == DateTime.MinValue)
            return DateTime.MinValue;
        return date.AddYears((int)years);
    }

    /// <summary>
    /// Add a duration to a date.
    /// </summary>
    /// <param name="date">Date to change</param>
    /// <param name="months">Number of months, each month equals 30 days</param>
    /// <returns>A DateTime with the specified duration added/subtracted</returns>
    public static DateTime DateAddMonths(DateTime date, decimal months)
    {
        if (date == DateTime.MinValue)
            return DateTime.MinValue;
        return date.AddMonths((int)months);
    }

    /// <summary>
    /// Add a duration to a date.
    /// </summary>
    /// <param name="date">Date to change</param>
    /// <param name="days">Number of days</param>
    /// <returns>A DateTime with the specified duration added/subtracted</returns>
    public static DateTime DateAddDays(DateTime date, decimal days)
    {
        if (date == DateTime.MinValue)
            return DateTime.MinValue;
        return date.AddDays((double)days);
    }

    /// <summary>
    /// Add a duration to a date.
    /// </summary>
    /// <param name="date">Date to change</param>
    /// <param name="hours">Number of hours</param>
    /// <returns>A DateTime with the specified duration added/subtracted</returns>
    public static DateTime DateAddHours(DateTime date, decimal hours)
    {
        if (date == DateTime.MinValue)
            return DateTime.MinValue;
        return date.AddHours((double)hours);
    }

    /// <summary>
    /// Add a duration to a date.
    /// </summary>
    /// <param name="date">Date to change</param>
    /// <param name="minutes">Number of minutes</param>
    /// <returns>A DateTime with the specified duration added/subtracted</returns>
    public static DateTime DateAddMinutes(DateTime date, decimal minutes)
    {
        if (date == DateTime.MinValue)
            return DateTime.MinValue;
        return date.AddMinutes((double)minutes);
    }

    /// <summary>
    /// Add a duration to a date.
    /// </summary>
    /// <param name="date">Date to change</param>
    /// <param name="seconds">Number of seconds</param>
    /// <returns>A DateTime with the specified duration added/subtracted</returns>
    public static DateTime DateAddSeconds(DateTime date, decimal seconds)
    {
        if (date == DateTime.MinValue)
            return DateTime.MinValue;
        return date.AddSeconds((double)seconds);
    }

    /// <summary>
    /// Get the year of the date.
    /// </summary>
    /// <param name="date">Date to read</param>
    /// <returns>Year</returns>
    public static int DateGetYear(DateTime date)
    {
        return date.Year;
    }

    /// <summary>
    /// Get the month of year from the date.
    /// </summary>
    /// <param name="date">Date to read</param>
    /// <returns>Month of year</returns>
    public static int DateGetMonth(DateTime date)
    {
        return date.Month;
    }

    /// <summary>
    /// Get the day of month from the date.
    /// </summary>
    /// <param name="date">Date to read</param>
    /// <returns>Day of month</returns>
    public static int DateGetDay(DateTime date)
    {
        return date.Day;
    }

    /// <summary>
    /// Get the hour in day from the date.
    /// </summary>
    /// <param name="date">Date to read</param>
    /// <returns>Hour in day</returns>
    public static int DateGetHour(DateTime date)
    {
        return date.Hour;
    }

    /// <summary>
    /// Get the minute in hour from the date.
    /// </summary>
    /// <param name="date">Date to read</param>
    /// <returns>Minute in hour</returns>
    public static int DateGetMinute(DateTime date)
    {
        return date.Minute;
    }

    /// <summary>
    /// Get the second in minute from the date.
    /// </summary>
    /// <param name="date">Date to read</param>
    /// <returns>Second in minute</returns>
    public static int DateGetSecond(DateTime date)
    {
        return date.Second;
    }

    /// <summary>
    /// Get the total days in the duration.
    /// </summary>
    /// <param name="duration">Duration to read</param>
    /// <returns>duration in days</returns>
    public static decimal DurationTotalDays(TimeSpan duration)
    {
        return (decimal)duration.TotalDays;
    }

    /// <summary>
    /// Get the total hours in the duration.
    /// </summary>
    /// <param name="duration">Duration to read</param>
    /// <returns>duration in hours</returns>
    public static decimal DurationTotalHours(TimeSpan duration)
    {
        return (decimal)duration.TotalHours;
    }

    /// <summary>
    /// Get the total minutes in the duration.
    /// </summary>
    /// <param name="duration">Duration to read</param>
    /// <returns>duration in minutes</returns>
    public static decimal DurationTotalMinutes(TimeSpan duration)
    {
        return (decimal)duration.TotalMinutes;
    }

    /// <summary>
    /// Get the total seconds in the duration.
    /// </summary>
    /// <param name="duration">Duration to read</param>
    /// <returns>duration in seconds</returns>
    public static decimal DurationTotalSeconds(TimeSpan duration)
    {
        return (decimal)duration.TotalSeconds;
    }

    /*****/

    /// <summary>
    /// Truncates the time part of a datetime value.
    /// </summary>
    /// <param name="date">The source date</param>
    /// <returns>A modified date with only the date part of the original datetime</returns>
    public static DateTime DateFloorDay(DateTime date)
    {
        return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, date.Kind);
    }

    /// <summary>
    /// Função que dados dois numéricos devolve o máximo
    /// </summary>
    /// <param name="obj1">numérico a comparar</param>
    /// <param name="obj2">outro numérico a comparar</param>
    /// <returns>o máximo entre os dois</returns>
    public static decimal maxN(decimal obj1, decimal obj2)
    {
        return (obj1 > obj2 ? obj1 : obj2);
    }

    /// <summary>
    /// Função que dados dois numéricos devolve o mínimo
    /// </summary>
    /// <param name="obj1">numérico a comparar</param>
    /// <param name="obj2">outro numérico a comparar</param>
    /// <returns>o mínimo entre os dois</returns>
    public static decimal minN(decimal obj1, decimal obj2)
    {
        return (obj1 < obj2 ? obj1 : obj2);
    }

    /// <summary>
    /// Função que dadas duas datas devolve a data máxima
    /// </summary>
    /// <param name="obj1">data a comparar</param>
    /// <param name="obj2">outra data a comparar</param>
    /// <returns>a maior data</returns>
    public static DateTime maxD(DateTime obj1, DateTime obj2)
    {
        return (obj1 > obj2 ? obj1 : obj2);
    }

    /// <summary>
    /// Função que dadas duas datas devolve a data mínima
    /// </summary>
    /// <param name="obj1">data a comparar</param>
    /// <param name="obj2">outra data a comparar</param>
    /// <returns>a menor data</returns>
    public static DateTime minD(DateTime obj1, DateTime obj2)
    {
        return (obj1 < obj2 ? obj1 : obj2);
    }

    /// <summary>
    /// Função que obtem o day actual
    /// </summary>
    /// <returns>DateTime com o day actual</returns>
    public static DateTime GetCurrentDay()
    {
        return DateTime.Today;
    }

    /// <summary>
    /// Função que obtem o mês actual
    /// </summary>
    /// <returns>int com o mês actual</returns>
    public static int GetCurrentMonth()
    {
        return Month(DateTime.Today);
    }

    /// <summary>
    /// Função que obtem o Qyear actual
    /// </summary>
    /// <returns>int com o Qyear actual</returns>
    public static int GetCurrentYear()
    {
        return Year(DateTime.Today);
    }

    /// <summary>
    /// Função que permite obter o nº de characters desejado à esquerda de uma string
    /// </summary>
    /// <param name="arg">string</param>
    /// <param name="nrElem">nº de characters</param>
    /// <returns>nº de characters da string a count da esquerda</returns>
    public static string LEFT(string arg, decimal nrElem)
    {
        int charNum = (int)nrElem;

        if (arg == null)
            return "";
        if (charNum < 0)
            return "";
        if (charNum > arg.Length)
            return arg;

        return arg.Substring(0, charNum);
    }

    /// <summary>
    /// Função que permite obter o nº de characters desejado à direita de uma string
    /// </summary>
    /// <param name="arg">string</param>
    /// <param name="nrElem">nº de characters</param>
    /// <returns>nº de characters da string a count da direita</returns>
    public static string RIGHT(string arg, decimal nrElem)
    {
        int charNum = (int)nrElem;

        if (arg == null)
            return "";
        if (charNum < 0)
            return "";
        if (charNum > arg.Length)
            return arg;
        return arg.Substring(arg.Length - charNum, charNum);
    }

    /// <summary>
    /// Função que dada uma string permite obter o nº de elementos a count de uma posição
    /// </summary>
    /// <param name="arg">string</param>
    /// <param name="initialPos">posição apartir da qual se querem obter os characters</param>
    /// <param name="nrElem">nº de characters desejados</param>
    /// <returns>characters da string</returns>
    public static string SubString(string arg, decimal initialPos, decimal nrElem)
    {
        int start = (int)initialPos;
        int charNum = (int)nrElem;

        if (arg == null)
            return "";
        if (charNum < 0)
            return "";
        if (start < 0)
            return "";
        if (start > arg.Length)
            return "";
        if (charNum > arg.Length - start)
            charNum = arg.Length - start;
        return arg.Substring(start, charNum);
    }

    /// <summary>
    /// Returns the zero-based index of the first occurrence of the specified substring within the given string.
    /// If the substring is not found or either input string is null or empty, returns -1.
    /// </summary>
    /// <param name="str">The string to search in.</param>
    /// <param name="substr">The substring to search for.</param>
    /// <returns>The zero-based index of the first occurrence of the specified substring, or -1 if not found.</returns>
    public static int IndexOf(string str, string substr)
    {
        if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(substr))
            return -1;
        return str.IndexOf(substr);
    }

    /// <summary>
    /// Função que permite arredondar um numérico com o número de digits decimais definido
    /// </summary>
    /// <param name="num">número a ser arredondamento</param>
    /// <param name="digits">número de digits decimais</param>
    /// <returns>o número arredondado</returns>
    public static decimal Round(decimal num, decimal digits)
    {
        //HAP - Added casts due to diferences when field in Genio is from decimal type.
        //Discussed with Rodrigo Serafim and Joao Ferro (2024/02/28) this solutions and it works with decimal and double/float
        return System.Math.Round(num, (int)digits, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Função que permite obter o módulo de um número
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static decimal abs(decimal num)
    {
        return System.Math.Abs(num);
    }

    /// <summary>
    /// Função que compara duas datas
    /// </summary>
    /// <param name="date1">date1</param>
    /// <param name="date2">date2</param>
    /// <returns>0 se sao iguais >0 se a 1ª é maior e <0 se a 1ª é menor </returns>
    public static int CompareDates(DateTime date1, DateTime date2)
    {
        return DateTime.Compare(date1, date2);
    }

    /// <summary>
    /// Função que retorna o size de uma string
    /// </summary>
    /// <param name="a">string</param>
    /// <returns>inteiro que corresponde ao size de uma string</returns>
    public static int LengthString(string a)
    {
        return a.Length;
    }

    /// <summary>
    /// Funcao que calcula o Qvalue da Incidencia
    /// </summary>
    /// <param name="unitValue">Qvalue unitário</param>
    /// <param name="amount">quantidade</param>
    /// <param name="pdiscount">percentagem de desconto</param>
    /// <param name="prec">digits decimais de precisão</param>
    /// <returns>o Qvalue da incidencia</returns>
    public static decimal Incidenc(decimal unitValue, decimal amount, decimal pdiscount, decimal prec)
    {
        decimal valorart = RoundQG(unitValue * amount, prec);
        return valorart - RoundQG(pdiscount / 100.0m * valorart, prec);
    }

    /// <summary>
    /// Função que calcula o Qvalue do IVA
    /// A incidencia pode entrar com iva ou sem iva sendo discriminada pelo parametro vatprice
    /// </summary>
    /// <param name="incidenc">O Qvalue com iva ou sem iva</param>
    /// <param name="rate_iva">taxa de IVA</param>
    /// <param name="vatprice">1 caso o incidenc seja o preço com iva, 0 caso seja o preço sem iva</param>
    /// <param name="prec">precisão</param>
    /// <returns>Qvalue do IVA</returns>
    public static decimal VATValue(decimal incidenc, decimal rate_iva, decimal vatprice, decimal prec)
    {
        return RoundQG(vatprice == 1
            ? incidenc / (1.0m + rate_iva / 100.0m) * (rate_iva / 100.0m)
            : incidenc * (rate_iva / 100.0m), prec);
    }

    /// <summary>
    /// Função que faz o arredondamento
    /// O arredondamento é feito com uma folga de 0.001 o que significa que por exemplo:
    /// RoundQG(0.499, 0) = 1.0 sendo que apenas 0.49899999 é que arredonda to baixo
    /// </summary>
    /// <param name="x">number a arrendondar</param>
    /// <param name="c">number de digits</param>
    /// <returns>Qvalue arredondado</returns>
    public static decimal RoundQG(decimal x, decimal c)
    {
        //(RS 2010.11.03) Reimplementei to dar os mesmos resultados que no BO e no SQL
        if (c < 0) c = 0;
        decimal folga = (decimal)(0.001 * Math.Pow(0.1, (int)c) * Math.Sign(x));
        return Math.Round(x + folga, (int)c, MidpointRounding.AwayFromZero);
    }
}
