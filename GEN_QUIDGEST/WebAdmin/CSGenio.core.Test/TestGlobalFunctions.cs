using CSGenio.framework;

namespace CSGenio.core.Test;

/// <summary>
/// Summary description for TestFuncoesGlobais
/// </summary>
public class TestFuncoesGlobais
{

    [Test]
    public void TestSomaDias()
    {
        DateTime res = GenFunctions.DateAddDays(new DateTime(2010, 10, 31), 1);
        Assert.That(res, Is.EqualTo(new DateTime(2010, 11, 01)));
        res = GenFunctions.DateAddDays(new DateTime(2010, 12, 01), 31);
        Assert.That(res, Is.EqualTo(new DateTime(2011, 01, 01)));
        res = GenFunctions.DateAddDays(new DateTime(2010, 12, 01), 0);
        Assert.That(res, Is.EqualTo(new DateTime(2010, 12, 01)));
        res = GenFunctions.DateAddDays(new DateTime(2010, 12, 01), -1);
        Assert.That(res, Is.EqualTo(new DateTime(2010, 11, 30)));
        res = GenFunctions.DateAddDays(DateTime.MinValue, 1);
        Assert.That(res, Is.EqualTo(DateTime.MinValue));
    }

    [Test]
    public void TestAtoi()
    {
        int res = GenFunctions.atoi("1234");
        Assert.That(res, Is.EqualTo(1234));
        res = GenFunctions.atoi("");
        Assert.That(res, Is.EqualTo(0));
        res = GenFunctions.atoi(null);
        Assert.That(res, Is.EqualTo(0));
        res = GenFunctions.atoi("-4567");
        Assert.That(res, Is.EqualTo(-4567));
        Assert.Throws<FormatException>(() =>
        {
            res = GenFunctions.atoi("98,76");
        });
        Assert.Throws<FormatException>(() =>
        {
            res = GenFunctions.atoi("xpto");
        });
    }

    [Test]
    public void TestIntToString()
    {
        string res = GenFunctions.IntToString(0);
        Assert.That(res, Is.EqualTo("0"));
        res = GenFunctions.IntToString(-1);
        Assert.That(res, Is.EqualTo("-1"));
        //res = GenFunctions.IntToString(0.5);
        //Assert.AreEqual("0", res); //Não devia truncar?
    }

    [Test]
    public void TestNumericToString()
    {
        //This is all kinds of wrong. Standard functions should not depend on current culture
        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

        string res = GenFunctions.NumericToString(23, 3);
        Assert.That(res, Is.EqualTo("23")); //Não devia ser 23.000?
        res = GenFunctions.NumericToString(23.123m, 3);
        Assert.That(res, Is.EqualTo("23.123")); //Devia sair com virgula?
        res = GenFunctions.NumericToString(100.123m, 0);
        Assert.That(res, Is.EqualTo("100"));
        res = GenFunctions.NumericToString(-100.123m, 1);
        Assert.That(res, Is.EqualTo("-100.1"));
    }

    [Test]
    public void TestEmptyD()
    {
        int res = GenFunctions.emptyD(DateTime.MinValue);
        Assert.That(res, Is.EqualTo(1));
        res = GenFunctions.emptyD(new DateTime(2010, 10, 31));
        Assert.That(res, Is.EqualTo(0));
        res = GenFunctions.emptyD(DateTime.MaxValue);
        Assert.That(res, Is.EqualTo(0));
        res = GenFunctions.emptyD(null);
        Assert.That(res, Is.EqualTo(1));
    }

    [Test]
    public void TestEmptyG()
    {
        int res = GenFunctions.emptyG(Guid.Empty.ToString());
        Assert.That(res, Is.EqualTo(1));
        res = GenFunctions.emptyG(String.Empty);
        Assert.That(res, Is.EqualTo(1));
        res = GenFunctions.emptyG("1234");
        Assert.That(res, Is.EqualTo(0));
        res = GenFunctions.emptyG(Guid.NewGuid().ToString());
        Assert.That(res, Is.EqualTo(0));
        res = GenFunctions.emptyG(null);
        Assert.That(res, Is.EqualTo(1));
    }

    [Test]
    public void TestEmptyC()
    {
        int res = GenFunctions.emptyC(String.Empty);
        Assert.That(res, Is.EqualTo(1));
        res = GenFunctions.emptyC("1234");
        Assert.That(res, Is.EqualTo(0));
        res = GenFunctions.emptyC(null);
        Assert.That(res, Is.EqualTo(1));
    }

    [Test]
    public void TestEmptyN()
    {
        int res = GenFunctions.emptyN(0.0);
        Assert.That(res, Is.EqualTo(1));
        res = GenFunctions.emptyN(double.MinValue);
        Assert.That(res, Is.EqualTo(0));
        res = GenFunctions.emptyN(1234.00);
        Assert.That(res, Is.EqualTo(0));
        res = GenFunctions.emptyN(null);
        Assert.That(res, Is.EqualTo(1));
    }

    [Test]
    public void TestEmptyT()
    {
        int res = GenFunctions.emptyT(null);
        Assert.That(res, Is.EqualTo(1));
        res = GenFunctions.emptyT("__:__");
        Assert.That(res, Is.EqualTo(1));
        res = GenFunctions.emptyT(String.Empty);
        Assert.That(res, Is.EqualTo(1));
        res = GenFunctions.emptyT("12:34");
        Assert.That(res, Is.EqualTo(0));
    }

    [Test]
    public void TestEmptyL()
    {
        int res = GenFunctions.emptyL(0);
        Assert.That(res, Is.EqualTo(1));
        res = GenFunctions.emptyL(1);
        Assert.That(res, Is.EqualTo(0));
        res = GenFunctions.emptyL(null);
        Assert.That(res, Is.EqualTo(1));
        res = GenFunctions.emptyL(0m);
        Assert.That(res, Is.EqualTo(1));
        res = GenFunctions.emptyL(1m);
        Assert.That(res, Is.EqualTo(0));
        res = GenFunctions.emptyL(0d);
        Assert.That(res, Is.EqualTo(1));
        res = GenFunctions.emptyL(1d);
        Assert.That(res, Is.EqualTo(0));
    }

    [Test]
    public void TestLTRIM()
    {
        string res = GenFunctions.LTRIM("tre jolie");
        Assert.That(res, Is.EqualTo("tre jolie"));
        res = GenFunctions.LTRIM(" \r\n tre jolie");
        Assert.That(res, Is.EqualTo("tre jolie"));
        res = GenFunctions.LTRIM("tre jolie \r\n ");
        Assert.That(res, Is.EqualTo("tre jolie \r\n "));
        res = GenFunctions.LTRIM(" \r\n tre jolie \r\n ");
        Assert.That(res, Is.EqualTo("tre jolie \r\n "));
        res = GenFunctions.LTRIM("");
        Assert.That(res, Is.EqualTo(""));
    }

    [Test]
    public void TestRTRIM()
    {
        string res = GenFunctions.RTRIM("tre jolie");
        Assert.That(res, Is.EqualTo("tre jolie"));
        res = GenFunctions.RTRIM(" \r\n tre jolie");
        Assert.That(res, Is.EqualTo(" \r\n tre jolie"));
        res = GenFunctions.RTRIM("tre jolie \r\n ");
        Assert.That(res, Is.EqualTo("tre jolie"));
        res = GenFunctions.RTRIM(" \r\n tre jolie \r\n ");
        Assert.That(res, Is.EqualTo(" \r\n tre jolie"));
        res = GenFunctions.RTRIM("");
        Assert.That(res, Is.EqualTo(""));
    }

    [Test]
    public void TestYear()
    {
        int res = GenFunctions.Year(new DateTime(2012, 12, 14));
        Assert.That(res, Is.EqualTo(2012));
        res = GenFunctions.Year(DateTime.MinValue);
        Assert.That(res, Is.EqualTo(0));
    }

    [Test]
    public void TestDateGetYear()
    {
        int res = GenFunctions.DateGetYear(new DateTime(2012, 12, 14));
        Assert.That(res, Is.EqualTo(2012));
        res = GenFunctions.DateGetYear(DateTime.MinValue);
        Assert.That(res, Is.EqualTo(1));
    }

    [Test]
    public void TestMonth()
    {
        int res = GenFunctions.Month(new DateTime(2012, 12, 14));
        Assert.That(res, Is.EqualTo(12));
        res = GenFunctions.Month(DateTime.MinValue);
        Assert.That(res, Is.EqualTo(0)); //devia mesmo ser 0? não devia ser 1?
    }

    [Test]
    public void TestDay()
    {
        int res = GenFunctions.Day(new DateTime(2012, 12, 14));
        Assert.That(res, Is.EqualTo(14));
        res = GenFunctions.Day(DateTime.MinValue);
        Assert.That(res, Is.EqualTo(0)); //devia mesmo ser 0? não devia ser 1?
    }

    [Test]
    public void TestDateGetDay()
    {
        int res = GenFunctions.DateGetDay(new DateTime(2012, 12, 14));
        Assert.That(res, Is.EqualTo(14));
        res = GenFunctions.DateGetDay(DateTime.MinValue);
        Assert.That(res, Is.EqualTo(1));
        res = GenFunctions.DateGetDay(DateTime.MaxValue);
        Assert.That(res, Is.EqualTo(31));
    }

    [Test]
    public void TestDateGetHour()
    {
        int res = GenFunctions.DateGetHour(new DateTime(2012, 12, 14, 17, 59, 35));
        Assert.That(res, Is.EqualTo(17));
        res = GenFunctions.DateGetHour(DateTime.MinValue);
        Assert.That(res, Is.EqualTo(0));
        res = GenFunctions.DateGetHour(DateTime.MaxValue);
        Assert.That(res, Is.EqualTo(23));
    }

    [Test]
    public void TestDateGetMinute()
    {
        int res = GenFunctions.DateGetMinute(new DateTime(2012, 12, 14, 17, 59, 35));
        Assert.That(res, Is.EqualTo(59));
        res = GenFunctions.DateGetMinute(DateTime.MinValue);
        Assert.That(res, Is.EqualTo(0));
        res = GenFunctions.DateGetMinute(DateTime.MaxValue);
        Assert.That(res, Is.EqualTo(59));
    }

    [Test]
    public void TestDateGetSecond()
    {
        int res = GenFunctions.DateGetSecond(new DateTime(2012, 12, 14, 17, 59, 35));
        Assert.That(res, Is.EqualTo(35));
        res = GenFunctions.DateGetSecond(DateTime.MinValue);
        Assert.That(res, Is.EqualTo(0));
        res = GenFunctions.DateGetSecond(DateTime.MaxValue);
        Assert.That(res, Is.EqualTo(59));
    }

    [Test]
    public void TestHorasToDouble()
    {
        decimal res = GenFunctions.HoursToDouble("10:30");
        Assert.That(res, Is.EqualTo(10 + 30 / 60m));
        res = GenFunctions.HoursToDouble("10:01");
        Assert.That(res, Is.EqualTo(10 + 01 / 60m));
        res = GenFunctions.HoursToDouble("_1:_1");
        Assert.That(res, Is.EqualTo(01 + 01 / 60m));
        res = GenFunctions.HoursToDouble("__:__");
        Assert.That(res, Is.EqualTo(0));
        res = GenFunctions.HoursToDouble("");
        Assert.That(res, Is.EqualTo(0));

        res = GenFunctions.HoursToDouble("25:30");
        Assert.That(res, Is.EqualTo(0));
        res = GenFunctions.HoursToDouble("12:70");
        Assert.That(res, Is.EqualTo(0));

        //TODO: null
    }

    [Test]
    public void TestDoubleToHoras()
    {
        string res = GenFunctions.DoubleToHours(10 + 30 / 60.0m);
        Assert.That(res, Is.EqualTo("10:30"));
        res = GenFunctions.DoubleToHours(10 + 01.0m / 60.0m);
        Assert.That(res, Is.EqualTo("10:01"));
        res = GenFunctions.DoubleToHours(0.0m);
        Assert.That(res, Is.EqualTo("00:00"));
        res = GenFunctions.DoubleToHours(2.999m);
        Assert.That(res, Is.EqualTo("03:00"));
        res = GenFunctions.DoubleToHours(2.991m);
        Assert.That(res, Is.EqualTo("02:59"));
        //TODO: negative
        //TODO: out of bounds
        //TODO: a aplicação devia crashar em debug mas apenas fazer log de um erro em produção de forma a não perder dados
    }

    [Test]
    public void TestHorasAdd()
    {
        string res = GenFunctions.HoursAdd("00:00", 1);
        Assert.That(res, Is.EqualTo("00:01"));
        res = GenFunctions.HoursAdd("02:00", -1);
        Assert.That(res, Is.EqualTo("01:59"));
        res = GenFunctions.HoursAdd("02:03", 57);
        Assert.That(res, Is.EqualTo("03:00"));
        res = GenFunctions.HoursAdd("00:00", 24 * 60);
        Assert.That(res, Is.EqualTo("23:59"));
        res = GenFunctions.HoursAdd("23:59", -24 * 60);
        Assert.That(res, Is.EqualTo("00:00"));
        res = GenFunctions.HoursAdd("20:5_", 5);
        Assert.That(res, Is.EqualTo("20:55"));
        res = GenFunctions.HoursAdd("20:5", 5);
        Assert.That(res, Is.EqualTo("__:__"));
        res = GenFunctions.HoursAdd(null, 5);
        Assert.That(res, Is.EqualTo("__:__"));
    }

    [Test]
    public void TestHorasDoubleRoundtrip()
    {
        //diferenca entre 2 horas
        decimal res = GenFunctions.HoursToDouble("00:30") - GenFunctions.HoursToDouble("00:29");
        //Assert.AreEqual(00.0 + 01.0 / 60.0, res); //Não pode dar exactamente igual por causa das aproximações
        //o que interessa é que seja recuperável de volta a uma string de horas
        Assert.That(GenFunctions.DoubleToHours(res), Is.EqualTo("00:01"));

        res = GenFunctions.HoursToDouble("23:59") - GenFunctions.HoursToDouble("23:58");
        Assert.That(GenFunctions.DoubleToHours(res), Is.EqualTo("00:01"));

        res = GenFunctions.HoursToDouble("23:59") - GenFunctions.HoursToDouble("12:01");
        Assert.That(GenFunctions.DoubleToHours(res), Is.EqualTo("11:58"));

        res = GenFunctions.HoursToDouble("23:59") - GenFunctions.HoursToDouble("00:01");
        Assert.That(GenFunctions.DoubleToHours(res), Is.EqualTo("23:58"));
    }

    [Test]
    public void TestCreateDateTime()
    {
        DateTime minDate = DateTime.MinValue;

        DateTime res = GenFunctions.CreateDateTime(2010, 10, 30, 20, 30, 00);
        Assert.That(res, Is.EqualTo(new DateTime(2010, 10, 30, 20, 30, 00)));

        res = GenFunctions.CreateDateTime(minDate.Year, minDate.Month, minDate.Day, 00, 00, 00);
        Assert.That(res, Is.EqualTo(minDate));

        // when an error occurs, CreateDateTime returns DateTime.MinValue
        res = GenFunctions.CreateDateTime(2010, 10, 30, 30, 30, 00);
        Assert.That(res, Is.EqualTo(minDate));

        res = GenFunctions.CreateDateTime(2010, 10, 30, 20, 80, 00);
        Assert.That(res, Is.EqualTo(minDate));
    }

    [Test]
    public void TestDateSetTime()
    {
        DateTime minDate = DateTime.MinValue;
        DateTime baseDate = GenFunctions.CreateDateTime(2010, 10, 30, 00, 00, 00);
        DateTime res;

        // if passed a DateTime.MinValue, it will not evaluate the time
        res = GenFunctions.DateSetTime(minDate, "20:30");
        Assert.That(res, Is.EqualTo(minDate));

        res = GenFunctions.DateSetTime(baseDate, "20:30");
        Assert.That(res, Is.EqualTo(new DateTime(2010, 10, 30, 20, 30, 00)));

        // invalid time will be ignored and original date returned
        res = GenFunctions.DateSetTime(baseDate, "30:30");
        Assert.That(res, Is.EqualTo(baseDate));
        res = GenFunctions.DateSetTime(baseDate, "20:80");
        Assert.That(res, Is.EqualTo(baseDate));
    }

    /*
    // Deprecated
    [Test]
    public void TestCriaDataHora()
    {
        DateTime res = GenFunctions.CriaDataHora(new DateTime(2010,10,30), "20:30");
        Assert.AreEqual(new DateTime(2010,10,30, 20,30,00), res);

        res = GenFunctions.CriaDataHora(DateTime.MinValue, "20:30");
        Assert.AreEqual(DateTime.MinValue, res);

        res = GenFunctions.CriaDataHora(new DateTime(2010,10,30), "30:30");
        Assert.AreEqual(new DateTime(2010,10,30, 00,00,00), res);
        res = GenFunctions.CriaDataHora(new DateTime(2010,10,30), "20:80");
        Assert.AreEqual(new DateTime(2010,10,30, 00,00,00), res);
    }
    */

    /*
    // Deprecated
    [Test]
    public void TestIsValid()
    {
        int res;
        res = GenFunctions.IsValid(DateTime.MinValue);
        Assert.AreEqual(0, res);
        res = GenFunctions.IsValid(DateTime.MaxValue);
        Assert.AreEqual(1, res);
        res = GenFunctions.IsValid(new DateTime(2012,12,14));
        Assert.AreEqual(1, res);
    }
    */

    [Test]
    public void TestKeyToString()
    {
        string res = GenFunctions.KeyToString("    1");
        Assert.That(res, Is.EqualTo("    1"));
        //test lower case
        res = GenFunctions.KeyToString("{234dceae-7c12-40e9-bbf5-b59f5f6dd890}");
        Assert.That(res, Is.EqualTo("234DCEAE7C1240E9BBF5B59F5F6DD890"));
        //test upper case
        res = GenFunctions.KeyToString("{234DCEAE-7C12-40E9-BBF5-B59F5F6DD890}");
        Assert.That(res, Is.EqualTo("234DCEAE7C1240E9BBF5B59F5F6DD890"));

        //TODO: null
    }

    [Test]
    public void TestMinN()
    {
        decimal res = GenFunctions.minN(0.0m, -1.0m);
        Assert.That(res, Is.EqualTo(-1));
        res = GenFunctions.minN(-1.0m, 0.0m);
        Assert.That(res, Is.EqualTo(-1));
        res = GenFunctions.minN(2.0m, 2.0m);
        Assert.That(res, Is.EqualTo(2));
    }

    [Test]
    public void TestMaxN()
    {
        decimal res = GenFunctions.maxN(0.0m, -1.0m);
        Assert.That(res, Is.EqualTo(0));
        res = GenFunctions.maxN(-1.0m, 0.0m);
        Assert.That(res, Is.EqualTo(0));
        res = GenFunctions.maxN(2.0m, 2.0m);
        Assert.That(res, Is.EqualTo(2));
    }

    [Test]
    public void TestMinD()
    {
        DateTime res = GenFunctions.minD(new DateTime(2010, 10, 30), new DateTime(2010, 10, 31));
        Assert.That(res, Is.EqualTo(new DateTime(2010, 10, 30)));
        res = GenFunctions.minD(new DateTime(2010, 10, 31), new DateTime(2010, 10, 30));
        Assert.That(res, Is.EqualTo(new DateTime(2010, 10, 30)));
        res = GenFunctions.minD(new DateTime(2010, 10, 31), new DateTime(2010, 10, 31));
        Assert.That(res, Is.EqualTo(new DateTime(2010, 10, 31)));
        res = GenFunctions.minD(DateTime.MinValue, new DateTime(2010, 10, 31));
        Assert.That(res, Is.EqualTo(DateTime.MinValue));
    }

    [Test]
    public void TestMaxD()
    {
        DateTime res = GenFunctions.maxD(new DateTime(2010, 10, 30), new DateTime(2010, 10, 31));
        Assert.That(res, Is.EqualTo(new DateTime(2010, 10, 31)));
        res = GenFunctions.maxD(new DateTime(2010, 10, 31), new DateTime(2010, 10, 30));
        Assert.That(res, Is.EqualTo(new DateTime(2010, 10, 31)));
        res = GenFunctions.maxD(new DateTime(2010, 10, 31), new DateTime(2010, 10, 31));
        Assert.That(res, Is.EqualTo(new DateTime(2010, 10, 31)));
        res = GenFunctions.maxD(DateTime.MinValue, new DateTime(2010, 10, 31));
        Assert.That(res, Is.EqualTo(new DateTime(2010, 10, 31)));
    }

    [Test]
    public void TestLEFT()
    {
        string res = GenFunctions.LEFT("ola adeus", 3);
        Assert.That(res, Is.EqualTo("ola"));
        res = GenFunctions.LEFT("ola adeus", 30);
        Assert.That(res, Is.EqualTo("ola adeus"));
        res = GenFunctions.LEFT("ola adeus", -1);
        Assert.That(res, Is.EqualTo(""));
        res = GenFunctions.LEFT(null, 10);
        Assert.That(res, Is.EqualTo(""));
    }

    [Test]
    public void TestRIGHT()
    {
        string res = GenFunctions.RIGHT("ola adeus", 5);
        Assert.That(res, Is.EqualTo("adeus"));
        res = GenFunctions.RIGHT("ola adeus", 30);
        Assert.That(res, Is.EqualTo("ola adeus"));
        res = GenFunctions.RIGHT("ola adeus", -1);
        Assert.That(res, Is.EqualTo(""));
        res = GenFunctions.RIGHT(null, 10);
        Assert.That(res, Is.EqualTo(""));
    }

    [Test]
    public void TestSubString()
    {
        string res = GenFunctions.SubString("ola adeus", 2, 3);
        Assert.That(res, Is.EqualTo("a a"));
        res = GenFunctions.SubString("ola adeus", 4, 30);
        Assert.That(res, Is.EqualTo("adeus"));
        res = GenFunctions.SubString("ola adeus", -1, 2);
        Assert.That(res, Is.EqualTo(""));
        res = GenFunctions.SubString("ola adeus", 2, -1);
        Assert.That(res, Is.EqualTo(""));
        res = GenFunctions.SubString("ola adeus", 30, 2);
        Assert.That(res, Is.EqualTo(""));
        res = GenFunctions.SubString(null, 1, 1);
        Assert.That(res, Is.EqualTo(""));

    }

    [Test]
    public void TestRoundQG()
    {
        //casos especiais da function
        tRoundQG(0.0m, 0, 0);
        tRoundQG(0.50m, 0, 1);
        tRoundQG(0.9999999m, 0, 1);
        tRoundQG(0.499m, 0, 1);
        tRoundQG(0.489m, 0, 0);

        //0
        //aproximação a pares
        tRoundQG(12346.5m, 0, 12347);
        tRoundQG(12346.4m, 0, 12346);
        tRoundQG(12346.49m, 0, 12346);
        tRoundQG(12346.498m, 0, 12346);
        tRoundQG(12346.498999999m, 0, 12346);
        tRoundQG(12346.499m, 0, 12347);

        //aproximação a impares
        tRoundQG(12343.5m, 0, 12344);
        tRoundQG(12343.4m, 0, 12343);
        tRoundQG(12343.49m, 0, 12343);
        tRoundQG(12343.498m, 0, 12343);
        tRoundQG(12343.498999999m, 0, 12343);
        tRoundQG(12343.499m, 0, 12344);

        //1
        //aproximação a pares
        tRoundQG(12345.65m, 1, 12345.7m);
        tRoundQG(12345.64m, 1, 12345.6m);
        tRoundQG(12345.649m, 1, 12345.6m);
        tRoundQG(12345.6498m, 1, 12345.6m);
        tRoundQG(12345.6498999999m, 1, 12345.6m);
        tRoundQG(12345.6499m, 1, 12345.7m);

        //aproximação a impares
        tRoundQG(12345.55m, 1, 12345.6m);
        tRoundQG(12345.54m, 1, 12345.5m);
        tRoundQG(12345.549m, 1, 12345.5m);
        tRoundQG(12345.5498m, 1, 12345.5m);
        tRoundQG(12345.5498999999m, 1, 12345.5m);
        tRoundQG(12345.5499m, 1, 12345.6m);

        //2, negativos
        //aproximação a pares
        tRoundQG(-12345.065m, 2, -12345.07m);
        tRoundQG(-12345.064m, 2, -12345.06m);
        tRoundQG(-12345.0649m, 2, -12345.06m);
        tRoundQG(-12345.06498m, 2, -12345.06m);
        tRoundQG(-12345.06498999999m, 2, -12345.06m);
        tRoundQG(-12345.06499m, 2, -12345.07m);

        //aproximação a impares
        tRoundQG(-12345.055m, 2, -12345.06m);
        tRoundQG(-12345.054m, 2, -12345.05m);
        tRoundQG(-12345.0549m, 2, -12345.05m);
        tRoundQG(-12345.05498m, 2, -12345.05m);
        tRoundQG(-12345.05498999999m, 2, -12345.05m);
        tRoundQG(-12345.05499m, 2, -12345.06m);

    }

    private void tRoundQG(decimal n, int c, decimal expected)
    {
        decimal res;
        res = GenFunctions.RoundQG(n, c);
        System.Diagnostics.Debug.WriteLine("orig=" + n + " prec=" + c + " res=" + res);
        Assert.That(res, Is.EqualTo(expected));
    }

    [Test]
    public void TestRound()
    {
        //casos especiais da function
        tRound(0.0m, 0, 0);
        tRound(0.5000001m, 0, 1);
        tRound(0.50m, 0, 1);
        tRound(0.4999999m, 0, 0);

        //0
        //aproximação a pares
        tRound(12346.5m, 0, 12347);
        tRound(12346.4m, 0, 12346);
        tRound(12346.49m, 0, 12346);
        tRound(12346.4999999m, 0, 12346);

        //aproximação a impares
        tRound(12343.5m, 0, 12344);
        tRound(12343.4m, 0, 12343);
        tRound(12343.49m, 0, 12343);
        tRound(12343.4999999m, 0, 12343);

        //1
        //aproximação a pares
        tRound(12345.65m, 1, 12345.7m);
        tRound(12345.64m, 1, 12345.6m);
        tRound(12345.649m, 1, 12345.6m);
        tRound(12345.649999999m, 1, 12345.6m);

        //aproximação a impares
        tRound(12345.55m, 1, 12345.6m);
        tRound(12345.54m, 1, 12345.5m);
        tRound(12345.549m, 1, 12345.5m);
        tRound(12345.54999999m, 1, 12345.5m);

        //2, negativos
        //aproximação a pares
        tRound(-12345.065m, 2, -12345.07m);
        tRound(-12345.064m, 2, -12345.06m);
        tRound(-12345.0649m, 2, -12345.06m);
        tRound(-12345.064999999m, 2, -12345.06m);

        //aproximação a impares
        tRound(-12345.055m, 2, -12345.06m);
        tRound(-12345.054m, 2, -12345.05m);
        tRound(-12345.0549m, 2, -12345.05m);
        tRound(-12345.054999999m, 2, -12345.05m);
    }

    private void tRound(decimal n, int c, decimal expected)
    {
        decimal res;
        res = GenFunctions.Round(n, c);
        Assert.That(res, Is.EqualTo(expected));
    }

    [Test]
    public void TestDiferenca_entre_Datas()
    {
        decimal res;
        res = GenFunctions.DateDiffPart(DateTime.MinValue, new DateTime(2010, 12, 01), "D");
        Assert.That(res, Is.EqualTo(0));
        res = GenFunctions.DateDiffPart(new DateTime(2010, 12, 01), DateTime.MinValue, "D");
        Assert.That(res, Is.EqualTo(0));

        res = GenFunctions.DateDiffPart(new DateTime(2010, 12, 01), new DateTime(2010, 12, 01), "D");
        Assert.That(res, Is.EqualTo(0));
        res = GenFunctions.DateDiffPart(new DateTime(2010, 11, 30), new DateTime(2010, 12, 01, 00, 02, 00), "D");
        Assert.That(res, Is.EqualTo(1));
        res = GenFunctions.DateDiffPart(new DateTime(2010, 12, 01), new DateTime(2010, 11, 20), "D");
        Assert.That(res, Is.EqualTo(-11));

        res = GenFunctions.DateDiffPart(new DateTime(2010, 11, 30), new DateTime(2010, 12, 01), "M");
        Assert.That(res, Is.EqualTo(60 * 24));
        res = GenFunctions.DateDiffPart(new DateTime(2010, 11, 30), new DateTime(2010, 12, 01, 01, 11, 02), "M");
        Assert.That(res, Is.EqualTo(60 * 24 + 1 * 60 + 11));
        res = GenFunctions.DateDiffPart(new DateTime(2010, 12, 01), new DateTime(2010, 11, 30, 00, 00, 04), "M");
        Assert.That(res, Is.EqualTo(-60 * 24));

        res = GenFunctions.DateDiffPart(new DateTime(2010, 11, 30), new DateTime(2010, 12, 01), "H");
        Assert.That(res, Is.EqualTo(24));
        res = GenFunctions.DateDiffPart(new DateTime(2010, 11, 30), new DateTime(2010, 12, 01, 01, 11, 02), "H");
        Assert.That(res, Is.EqualTo(24 + 1));
        res = GenFunctions.DateDiffPart(new DateTime(2010, 12, 01), new DateTime(2010, 11, 30, 00, 00, 04), "H");
        Assert.That(res, Is.EqualTo(-24));

        res = GenFunctions.DateDiffPart(new DateTime(2010, 11, 30), new DateTime(2010, 12, 01, 01, 11, 02), "S");
        Assert.That(res, Is.EqualTo(60 * 60 * 24 + 1 * 60 * 60 + 60 * 11 + 2));
    }

    private static void tTotalDuration(TimeSpan time, decimal duration, Func<TimeSpan, decimal> Func)
    {
        Assert.That(
            duration,
            Is.EqualTo(Func(time)).Within(duration/10000) // Tolerance of 0.01% due to infinite fractional values
        );
    }

    [Test]
    public void TestTotalDurationDays()
    {
        // Whole time spans
        tTotalDuration(TimeSpan.FromDays(2), 2m, GenFunctions.DurationTotalDays);
        tTotalDuration(TimeSpan.FromHours(48), 2m, GenFunctions.DurationTotalDays);
        tTotalDuration(TimeSpan.FromMinutes(60 * 48), 2m, GenFunctions.DurationTotalDays);
        tTotalDuration(TimeSpan.FromSeconds(3600 * 48), 2m, GenFunctions.DurationTotalDays);
        tTotalDuration(TimeSpan.FromMilliseconds(1000 * 3600 * 48), 2m, GenFunctions.DurationTotalDays);

        // Fractional time spans
        tTotalDuration(TimeSpan.FromDays(12.5), 12.5m, GenFunctions.DurationTotalDays);
        tTotalDuration(TimeSpan.FromHours(12.5), 0.52083m, GenFunctions.DurationTotalDays);
        tTotalDuration(TimeSpan.FromMinutes(60 * 12.5), 0.52083m, GenFunctions.DurationTotalDays);
        tTotalDuration(TimeSpan.FromSeconds(3600 * 12.5), 0.52083m, GenFunctions.DurationTotalDays);
        tTotalDuration(TimeSpan.FromMilliseconds(1000 * 3600 * 12.5), 0.52083m, GenFunctions.DurationTotalDays);
        // Microseconds value overflows with a 1.5-day time span - reduced by a factor of 1000
        tTotalDuration(TimeSpan.FromMicroseconds(1000 * 3600 * 12.5), 0.0005208m, GenFunctions.DurationTotalDays);

        // Negative time spans (for example, when comparing a date with an earlier one)
        tTotalDuration(TimeSpan.FromDays(-2), -2m, GenFunctions.DurationTotalDays);
        tTotalDuration(TimeSpan.FromHours(-48), -2m, GenFunctions.DurationTotalDays);
        tTotalDuration(TimeSpan.FromMinutes(-60 * 48), -2m, GenFunctions.DurationTotalDays);
        tTotalDuration(TimeSpan.FromSeconds(-3600 * 48), -2m, GenFunctions.DurationTotalDays);
        tTotalDuration(TimeSpan.FromMilliseconds(-1000 * 3600 * 48), -2m, GenFunctions.DurationTotalDays);
        tTotalDuration(TimeSpan.FromMicroseconds(-1000 * 3600 * 48), -0.002m, GenFunctions.DurationTotalDays);

        // Zero
        tTotalDuration(TimeSpan.Zero, 0m, GenFunctions.DurationTotalDays);

        // Very large time spans
        tTotalDuration(TimeSpan.FromHours(123456789), (123456789m / 24m), GenFunctions.DurationTotalDays);
    }

    [Test]
    public void TestTotalDurationHours()
    {
        // Whole time spans
        tTotalDuration(TimeSpan.FromDays(2), 48m, GenFunctions.DurationTotalHours);
        tTotalDuration(TimeSpan.FromHours(48), 48m, GenFunctions.DurationTotalHours);
        tTotalDuration(TimeSpan.FromMinutes(60 * 48), 48m, GenFunctions.DurationTotalHours);
        tTotalDuration(TimeSpan.FromSeconds(3600 * 48), 48m, GenFunctions.DurationTotalHours);
        tTotalDuration(TimeSpan.FromMilliseconds(1000 * 3600 * 48), 48m, GenFunctions.DurationTotalHours);

        // Fractional time spans
        tTotalDuration(TimeSpan.FromDays(0.3), 7.2m, GenFunctions.DurationTotalHours);
        tTotalDuration(TimeSpan.FromHours(0.3), 0.3m, GenFunctions.DurationTotalHours);
        tTotalDuration(TimeSpan.FromMinutes(60 * 24 * 0.3), 7.2m, GenFunctions.DurationTotalHours);
        tTotalDuration(TimeSpan.FromSeconds(3600 * 24 * 0.3), 7.2m, GenFunctions.DurationTotalHours);
        tTotalDuration(TimeSpan.FromMilliseconds(1000 * 3600 * 24 * 0.3), 7.2m, GenFunctions.DurationTotalHours);
        // Microseconds overflows with a 1.5-day time span - reduced by a factor of 1000
        tTotalDuration(TimeSpan.FromMicroseconds(1000 * 3600 * 24 * 0.3), 0.0072m, GenFunctions.DurationTotalHours);

        // Negative time spans (for example, when comparing a date with an earlier one)
        tTotalDuration(TimeSpan.FromDays(-2), -48m, GenFunctions.DurationTotalHours);
        tTotalDuration(TimeSpan.FromHours(-48), -48m, GenFunctions.DurationTotalHours);
        tTotalDuration(TimeSpan.FromMinutes(-60 * 48), -48m, GenFunctions.DurationTotalHours);
        tTotalDuration(TimeSpan.FromSeconds(-3600 * 48), -48m, GenFunctions.DurationTotalHours);
        tTotalDuration(TimeSpan.FromMilliseconds(-1000 * 3600 * 48), -48m, GenFunctions.DurationTotalHours);
        tTotalDuration(TimeSpan.FromMicroseconds(-1000 * 3600 * 48), -0.048m, GenFunctions.DurationTotalHours);

        // Zero
        tTotalDuration(TimeSpan.Zero, 0m, GenFunctions.DurationTotalHours);

        // Very large time spans
        tTotalDuration(TimeSpan.FromHours(123456789), 123456789m, GenFunctions.DurationTotalHours);
    }

    [Test]
    public void TestTotalDurationMinutes()
    {
        // Whole time spans
        tTotalDuration(TimeSpan.FromDays(0.25), 360m, GenFunctions.DurationTotalMinutes);
        tTotalDuration(TimeSpan.FromHours(6), 360m, GenFunctions.DurationTotalMinutes);
        tTotalDuration(TimeSpan.FromMinutes(360), 360m, GenFunctions.DurationTotalMinutes);
        tTotalDuration(TimeSpan.FromSeconds(3600 * 6), 360m, GenFunctions.DurationTotalMinutes);
        tTotalDuration(TimeSpan.FromMilliseconds(1000 * 3600 * 6), 360m, GenFunctions.DurationTotalMinutes);

        // Fractional time spans
        tTotalDuration(TimeSpan.FromDays(0.03), 43.2m, GenFunctions.DurationTotalMinutes);
        tTotalDuration(TimeSpan.FromHours(0.03 * 24), 43.2m, GenFunctions.DurationTotalMinutes);
        tTotalDuration(TimeSpan.FromMinutes(43.2), 43.2m, GenFunctions.DurationTotalMinutes);
        tTotalDuration(TimeSpan.FromSeconds(3600 * 24 * 0.03), 43.2m, GenFunctions.DurationTotalMinutes);
        tTotalDuration(TimeSpan.FromMilliseconds(1000 * 3600 * 24 * 0.03), 43.2m, GenFunctions.DurationTotalMinutes);
        // Microseconds overflows with a 1.5-day time span - reduced by a factor of 1000
        tTotalDuration(TimeSpan.FromMicroseconds(1000 * 3600 * 24 * 0.03), 0.0432m, GenFunctions.DurationTotalMinutes);

        // Negative time spans (for example, when comparing a date with an earlier one)
        tTotalDuration(TimeSpan.FromDays(-0.03), -43.2m, GenFunctions.DurationTotalMinutes);
        tTotalDuration(TimeSpan.FromHours(-0.03 * 24), -43.2m, GenFunctions.DurationTotalMinutes);
        tTotalDuration(TimeSpan.FromMinutes(-43.2), -43.2m, GenFunctions.DurationTotalMinutes);
        tTotalDuration(TimeSpan.FromSeconds(-3600 * 24 * 0.03), -43.2m, GenFunctions.DurationTotalMinutes);
        tTotalDuration(TimeSpan.FromMilliseconds(-1000 * 3600 * 24 * 0.03), -43.2m, GenFunctions.DurationTotalMinutes);
        tTotalDuration(TimeSpan.FromMicroseconds(-1000 * 3600 * 24 * 0.03), -0.0432m, GenFunctions.DurationTotalMinutes);

        // Zero
        tTotalDuration(TimeSpan.Zero, 0m, GenFunctions.DurationTotalMinutes);

        // Very large time spans
        tTotalDuration(TimeSpan.FromHours(123456789), (123456789m * 60m), GenFunctions.DurationTotalMinutes);
    }

    [Test]
    public void TestTotalDurationSeconds()
    {
        // Whole time spans
        tTotalDuration(TimeSpan.FromDays(0.04), 3456m, GenFunctions.DurationTotalSeconds);
        tTotalDuration(TimeSpan.FromHours(0.04 * 24), 3456m, GenFunctions.DurationTotalSeconds);
        tTotalDuration(TimeSpan.FromMinutes(0.04 * 24 * 60), 3456m, GenFunctions.DurationTotalSeconds);
        tTotalDuration(TimeSpan.FromSeconds(3456), 3456m, GenFunctions.DurationTotalSeconds);
        tTotalDuration(TimeSpan.FromMilliseconds(1000 * 0.04 * 24 * 60 * 60), 3456m, GenFunctions.DurationTotalSeconds);

        // Fractional time spans
        tTotalDuration(TimeSpan.FromDays(0.043), 3715.2m, GenFunctions.DurationTotalSeconds);
        tTotalDuration(TimeSpan.FromHours(0.043 * 24), 3715.2m, GenFunctions.DurationTotalSeconds);
        tTotalDuration(TimeSpan.FromMinutes(0.043 * 24 * 60), 3715.2m, GenFunctions.DurationTotalSeconds);
        tTotalDuration(TimeSpan.FromSeconds(3715.2), 3715.2m, GenFunctions.DurationTotalSeconds);
        tTotalDuration(TimeSpan.FromMilliseconds(1000 * 0.043 * 24 * 60 * 60), 3715.2m, GenFunctions.DurationTotalSeconds);
        // Microseconds overflows with a 1.5-day time span - reduced by a factor of 1000
        tTotalDuration(TimeSpan.FromMicroseconds(1000 * 0.043 * 24 * 60 * 60), 3.7152m, GenFunctions.DurationTotalSeconds);

        // Negative time spans (for example, when comparing a date with an earlier one)
        tTotalDuration(TimeSpan.FromDays(-0.043), -3715.2m, GenFunctions.DurationTotalSeconds);
        tTotalDuration(TimeSpan.FromHours(-0.043 * 24), -3715.2m, GenFunctions.DurationTotalSeconds);
        tTotalDuration(TimeSpan.FromMinutes(-0.043 * 24 * 60), -3715.2m, GenFunctions.DurationTotalSeconds);
        tTotalDuration(TimeSpan.FromSeconds(-3715.2), -3715.2m, GenFunctions.DurationTotalSeconds);
        tTotalDuration(TimeSpan.FromMilliseconds(-1000 * 0.043 * 24 * 60 * 60), -3715.2m, GenFunctions.DurationTotalSeconds);
        tTotalDuration(TimeSpan.FromMicroseconds(-1000 * 0.043 * 24 * 60 * 60), -3.7152m, GenFunctions.DurationTotalSeconds);

        // Zero
        tTotalDuration(TimeSpan.Zero, 0m, GenFunctions.DurationTotalSeconds);

        // Very large time spans
        tTotalDuration(TimeSpan.FromHours(123456789), (123456789m * 60m * 60m), GenFunctions.DurationTotalSeconds);
    }

    [Test]
    public void TestDateCompareYear()
    {
        Assert.That(GenFunctions.DateCompare(new DateTime(1997, 6, 5, 12, 5, 20), new DateTime(1997, 6, 5, 12, 5, 20)), Is.EqualTo(0));
        Assert.That(GenFunctions.DateCompare(new DateTime(1997, 6, 5, 12, 5, 20), new DateTime(1998, 6, 5, 12, 5, 20)), Is.LessThan(0));
        Assert.That(GenFunctions.DateCompare(new DateTime(1997, 6, 5, 12, 5, 20), new DateTime(1996, 6, 5, 12, 5, 20)), Is.GreaterThan(0));
    }

    [Test]
    public void TestDateCompareMonth()
    {
        Assert.That(GenFunctions.DateCompare(new DateTime(1997, 6, 5, 12, 5, 20), new DateTime(1997, 6, 5, 12, 5, 20)), Is.EqualTo(0));
        Assert.That(GenFunctions.DateCompare(new DateTime(1997, 6, 5, 12, 5, 20), new DateTime(1997, 7, 5, 12, 5, 20)), Is.LessThan(0));
        Assert.That(GenFunctions.DateCompare(new DateTime(1997, 6, 5, 12, 5, 20), new DateTime(1997, 5, 5, 12, 5, 20)), Is.GreaterThan(0));
    }

    [Test]
    public void TestDateCompareDay()
    {
        Assert.That(GenFunctions.DateCompare(new DateTime(1997, 6, 5, 12, 5, 20), new DateTime(1997, 6, 5, 12, 5, 20)), Is.EqualTo(0));
        Assert.That(GenFunctions.DateCompare(new DateTime(1997, 6, 5, 12, 5, 20), new DateTime(1997, 6, 6, 12, 5, 20)), Is.LessThan(0));
        Assert.That(GenFunctions.DateCompare(new DateTime(1997, 6, 5, 12, 5, 20), new DateTime(1997, 6, 4, 12, 5, 20)), Is.GreaterThan(0));
    }

    [Test]
    public void TestDateCompareHour()
    {
        Assert.That(GenFunctions.DateCompare(new DateTime(1997, 6, 5, 12, 5, 20), new DateTime(1997, 6, 5, 12, 5, 20)), Is.EqualTo(0));
        Assert.That(GenFunctions.DateCompare(new DateTime(1997, 6, 5, 12, 5, 20), new DateTime(1997, 6, 5, 13, 5, 20)), Is.LessThan(0));
        Assert.That(GenFunctions.DateCompare(new DateTime(1997, 6, 5, 12, 5, 20), new DateTime(1997, 6, 5, 11, 5, 20)), Is.GreaterThan(0));
    }

    [Test]
    public void TestDateCompareMinute()
    {
        Assert.That(GenFunctions.DateCompare(new DateTime(1997, 6, 5, 12, 5, 20), new DateTime(1997, 6, 5, 12, 5, 20)), Is.EqualTo(0));
        Assert.That(GenFunctions.DateCompare(new DateTime(1997, 6, 5, 12, 5, 20), new DateTime(1997, 6, 5, 12, 6, 20)), Is.LessThan(0));
        Assert.That(GenFunctions.DateCompare(new DateTime(1997, 6, 5, 12, 5, 20), new DateTime(1997, 6, 5, 12, 4, 20)), Is.GreaterThan(0));
    }

    [Test]
    public void TestDateCompareSecond()
    {
        Assert.That(GenFunctions.DateCompare(new DateTime(1997, 6, 5, 12, 5, 20), new DateTime(1997, 6, 5, 12, 5, 20)), Is.EqualTo(0));
        Assert.That(GenFunctions.DateCompare(new DateTime(1997, 6, 5, 12, 5, 20), new DateTime(1997, 6, 5, 12, 5, 21)), Is.LessThan(0));
        Assert.That(GenFunctions.DateCompare(new DateTime(1997, 6, 5, 12, 5, 20), new DateTime(1997, 6, 5, 12, 5, 19)), Is.GreaterThan(0));
    }

    [Test]
    public void TestDateAddYears()
    {
        Assert.That(GenFunctions.DateCompare(GenFunctions.DateAddYears(new DateTime(1997, 6, 5, 12, 5, 20), 1), new DateTime(1998, 6, 5, 12, 5, 20)), Is.EqualTo(0));
    }

    [Test]
    public void TestDateAddMonths()
    {
        Assert.That(GenFunctions.DateCompare(GenFunctions.DateAddMonths(new DateTime(1997, 6, 5, 12, 5, 20), 1), new DateTime(1997, 7, 5, 12, 5, 20)), Is.EqualTo(0));
    }

    [Test]
    public void TestDateAddDays()
    {
        Assert.That(GenFunctions.DateCompare(GenFunctions.DateAddDays(new DateTime(1997, 6, 5, 12, 5, 20), 1), new DateTime(1997, 6, 6, 12, 5, 20)), Is.EqualTo(0));
    }

    [Test]
    public void TestDateAddHours()
    {
        Assert.That(GenFunctions.DateCompare(GenFunctions.DateAddHours(new DateTime(1997, 6, 5, 12, 5, 20), 1), new DateTime(1997, 6, 5, 13, 5, 20)), Is.EqualTo(0));
    }

    [Test]
    public void TestDateAddMinutes()
    {
        Assert.That(GenFunctions.DateCompare(GenFunctions.DateAddMinutes(new DateTime(1997, 6, 5, 12, 5, 20), 1), new DateTime(1997, 6, 5, 12, 6, 20)), Is.EqualTo(0));
    }

    [Test]
    public void TestDateAddSeconds()
    {
        Assert.That(GenFunctions.DateCompare(GenFunctions.DateAddSeconds(new DateTime(1997, 6, 5, 12, 5, 20), 1), new DateTime(1997, 6, 5, 12, 5, 21)), Is.EqualTo(0));
    }
}

