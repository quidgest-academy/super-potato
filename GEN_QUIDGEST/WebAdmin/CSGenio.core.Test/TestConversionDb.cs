using CSGenio.framework;
using CSGenio.persistence;

namespace CSGenio.core.Test;

/// <summary>
///This is a test class for Test and is intended
///to contain all Test Unit Tests
///</summary>
public class TestConversionDb
{

    /// <summary>
    /// Teste à função ToString
    /// </summary>
    [Test]
    public void TestToString()
    {
        string testStr;
        string expectedStr;

        // null --> empty string
        string res = DBConversion.ToString(null);
        Assert.That(res, Is.EqualTo(""));

        // DBNull --> empty string
        res = DBConversion.ToString(DBNull.Value);
        Assert.That(res, Is.EqualTo(""));

        // empty GUID --> ??
        //res = DBConversion.ToString(Guid.Empty);
        //Assert.AreEqual("", res); // Actualmente retorna 00000000-0000-0000-0000-000000000000. Será suposto?

        // empty string --> empty string
        res = DBConversion.ToString("");
        Assert.That(res, Is.EqualTo(""));

        // string with two double quotes --> same string
        testStr = "\"\"";
        res = DBConversion.ToString(testStr);
        Assert.That(res, Is.EqualTo(testStr));

        // string with two single quotes --> same string
        testStr = "''";
        res = DBConversion.ToString(testStr);
        Assert.That(res, Is.EqualTo(testStr));

        // a relatively long string containing Lorem ipsum in English --> same string
        testStr = "Nor again is there anyone who loves or pursues or desires to obtain pain of itself, because it is pain, but occasionally circumstances occur in which toil and pain can procure him some great pleasure. To take a trivial example, which of us ever undertakes laborious physical exercise, except to obtain some advantage from it? But who has any right to find fault with a man who chooses to enjoy a pleasure that has no annoying consequences, or one who avoids a pain that produces no resultant pleasure?";
        res = DBConversion.ToString(testStr);
        Assert.That(res, Is.EqualTo(testStr));

        // another relatively long string with accented characters --> same string
        testStr = "Este texto contém acentuação; é uma pequena experiência, ok? Vamos juntar mais alguns caracteres: @, €, $, £, º, ª, âêîôû, àÈìòù, æ, œ, <>, «», \\, [], ++, --, '', e já agora: ¡¢£¤¥¦§¨©ª«¬®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿ";
        res = DBConversion.ToString(testStr);
        Assert.That(res, Is.EqualTo(testStr));

        // string containing tabs and newlines --> same string
        testStr = "Tabs \t e \n newlines";
        res = DBConversion.ToString(testStr);
        Assert.That(res, Is.EqualTo(testStr));

        // string containing only spaces (7) --> same string
        testStr = "       ";
        res = DBConversion.ToString(testStr);
        Assert.That(res, Is.EqualTo(testStr));

        // a positive number (one million)
        res = DBConversion.ToString(1e9);
        Assert.That(res, Is.EqualTo("1000000000"));

        // zero
        res = DBConversion.ToString(0);
        Assert.That(res, Is.EqualTo("0"));

        // a negative number
        res = DBConversion.ToString(-5e8);
        Assert.That(res, Is.EqualTo("-500000000"));

        // a floating point number
        res = DBConversion.ToString(1.05);
        // FIXME DBConversion.ToString() should return culture-invariant representation
        expectedStr = "1.05";
        expectedStr = expectedStr.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        Assert.That(res, Is.EqualTo(expectedStr));

        // another floating point number
        res = DBConversion.ToString(2.5e-4);
        // FIXME DBConversion.ToString() should return culture-invariant representation
        expectedStr = "0.00025";
        expectedStr = expectedStr.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        Assert.That(res, Is.EqualTo(expectedStr));

        // smallest representable value in double precision - This test is somewhat ugly, but it should pass
        res = DBConversion.ToString(double.Epsilon);
        // FIXME DBConversion.ToString() should return culture-invariant representation
        expectedStr = double.Epsilon.ToString();
        expectedStr = expectedStr.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        Assert.That(res, Is.EqualTo(expectedStr));

        // a number representable in decimal, but not in 64bit double precision
        res = DBConversion.ToString(1.0000000000000001m);
        // FIXME DBConversion.ToString() should return culture-invariant representation
        expectedStr = "1.0000000000000001";
        expectedStr = expectedStr.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        Assert.That(res, Is.EqualTo(expectedStr));

        // some random object
        //try
        //{
        //    res = DBConversion.ToString(typeof(DBConversion));
        //    Assert.Fail("DBConversion.ToString(<some random object>) should throw an exception");
        //}
        //catch (AssertFailedException)
        //{
        //    throw;
        //}
        //catch { }
    }

    /// <summary>
    /// Teste à função FromString
    /// </summary>
    [Test]
    public void TestFromString()
    {

        // null --> empty string
        string res = DBConversion.FromString(null);
        // Até mais ver, Qvalues "vazios" são representados por strings de comprimento zero, e não por NULL
        Assert.That(res, Is.EqualTo("''"));

        // empty string --> empty string
        res = DBConversion.FromString("");
        // Até mais ver, Qvalues "vazios" são representados por strings de comprimento zero, e não por NULL
        Assert.That(res, Is.EqualTo("''"));

        // single quote --> escaped single quote
        res = DBConversion.FromString("'");
        Assert.That(res, Is.EqualTo("''''"));

        // two single quotes --> two escaped single quote
        res = DBConversion.FromString("''");
        Assert.That(res, Is.EqualTo("''''''"));

        // single space --> single space
        res = DBConversion.FromString(" ");
        Assert.That(res, Is.EqualTo("' '"));

        // string with a number --> string with a number
        res = DBConversion.FromString("134");
        Assert.That(res, Is.EqualTo("'134'"));

        // string beginning with single quote --> same string with escaped quote
        res = DBConversion.FromString("'--; SQL Injection!");
        Assert.That(res, Is.EqualTo("'''--; SQL Injection!'"));

        // bunch of Unicode characters --> same bunch of Unicode characters
        res = DBConversion.FromString("¡¢£¤¥¦§¨©ª«¬®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿ");
        Assert.That(res, Is.EqualTo("'¡¢£¤¥¦§¨©ª«¬®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿ'"));
    }

    /// <summary>
    /// Teste às funções FromNumeric e ToNumeric.
    /// </summary>
    [Test]
    public void TestFromNumericAndToNumeric()
    {
        decimal res;

        // TODO: actualmente tudo o que envolve digits decimais está a falhar
        // Há que ver até que ponto faz sentido o auxFromNumericAndBack,
        // já que o ToNumeric() "supostamente" não recebe strings.

        auxFromNumericAndBack(0);
        auxFromNumericAndBack(-1);
        //auxFromNumericAndBack(double.MinValue);
        //auxFromNumericAndBack(double.MaxValue);
        //auxFromNumericAndBack(0.7);  
        //auxFromNumericAndBack(-12345678.97);
        //auxFromNumericAndBack(Math.PI);
        //auxFromNumericAndBack(Math.E);

        // empty string --> zero
        res = DBConversion.ToNumeric("");
        Assert.That(res, Is.EqualTo(0));

        // "  -8 " --> -8
        res = DBConversion.ToNumeric("  -8 ");
        Assert.That(res, Is.EqualTo(-8));

        // "  +110" --> 110
        res = DBConversion.ToNumeric("  +110");
        Assert.That(res, Is.EqualTo(110));

        //res = DBConversion.ToNumeric(double.MaxValue.ToString());
        //Assert.AreEqual(double.MaxValue, res);

        // "4294967296. " --> 4294967296.0  [that's 2^32, since you're wondering]
        res = DBConversion.ToNumeric("4294967296. ");
        Assert.That(res, Is.EqualTo(4294967296.0m));

        // "Not a Number" --> exception
        //try
        //{
        //    res = DBConversion.ToNumeric("Not a Number");
        //    Assert.Fail("should have thrown an exception, but instead returned " + res);
        //}
        //catch (AssertFailedException)
        //{
        //    throw;
        //}
        //catch { }

        //// "-+0" --> exception
        //try
        //{
        //    res = DBConversion.ToNumeric("-+0");
        //    Assert.Fail("should have thrown an exception, but instead returned " + res);
        //}
        //catch (AssertFailedException)
        //{
        //    throw;
        //}
        //catch { }
    }

    private void auxFromNumericAndBack(decimal original)
    {
        string dbVal = DBConversion.FromNumeric(original);
        decimal result = DBConversion.ToNumeric(dbVal);
        Assert.That(result, Is.EqualTo(original), String.Format("Expected {0} but got {1}. Database value was {2}", original, result, dbVal));
    }

    /// <summary>
    /// Teste à função ToInteger
    /// </summary>
    [Test]
    public void TestToInteger()
    {
        int res;

        // null --> 0
        res = DBConversion.ToInteger(null);
        Assert.That(res, Is.EqualTo(0));

        //
        // From booleans
        //

        // false --> 0
        res = DBConversion.ToInteger(false);
        Assert.That(res, Is.EqualTo(0));

        // true --> 1
        res = DBConversion.ToInteger(true);
        Assert.That(res, Is.EqualTo(1));

        //
        // From numeric types
        //

        // int.MinValue --> int.MinValue
        res = DBConversion.ToInteger(int.MinValue);
        Assert.That(res, Is.EqualTo(int.MinValue));

        // 0 --> 0
        res = DBConversion.ToInteger(0);
        Assert.That(res, Is.EqualTo(0));

        // int.MaxValue --> int.MaxValue
        res = DBConversion.ToInteger(int.MinValue);
        Assert.That(res, Is.EqualTo(int.MinValue));

        // double.MinValue --> int.MinValue
        //res = DBConversion.ToInteger(double.MinValue); // FIXME está a falhar
        //Assert.AreEqual(int.MinValue, res);

        // double.MaxValue --> int.MaxValue
        //res = DBConversion.ToInteger(double.MaxValue); // FIXME está a falhar
        //Assert.AreEqual(int.MaxValue, res);

        // decimal.MinValue --> int.MinValue
        //res = DBConversion.ToInteger(decimal.MinValue); // FIXME está a falhar
        //Assert.AreEqual(int.MinValue, res);

        // decimal.MaxValue --> int.MaxValue
        //res = DBConversion.ToInteger(decimal.MaxValue); // FIXME está a falhar
        //Assert.AreEqual(int.MaxValue, res);

        //
        // From strings
        //

        // "-1" --> -1
        res = DBConversion.ToInteger("-1");
        Assert.That(res, Is.EqualTo(-1));

        // "0" --> 0
        res = DBConversion.ToInteger("0");
        Assert.That(res, Is.EqualTo(0));

        // int.MinValue --> int.MinValue
        res = DBConversion.ToInteger(int.MinValue);
        Assert.That(res, Is.EqualTo(int.MinValue));

        // 0 --> 0
        res = DBConversion.ToInteger(0);
        Assert.That(res, Is.EqualTo(0));

        // int.MaxValue --> int.MaxValue
        res = DBConversion.ToInteger(int.MaxValue);
        Assert.That(res, Is.EqualTo(int.MaxValue));

        // "   42   " --> 42
        res = DBConversion.ToInteger("   42   ");
        Assert.That(res, Is.EqualTo(42));

        // " 8 7  " --> exception
        //try
        //{
        //    res = DBConversion.ToInteger(" 8 7  ");
        //    Assert.Fail("DBConversion.ToInteger(\" 8 7  \") should have thrown an exception, but instead returned " + res);
        //}
        //catch (AssertFailedException)
        //{
        //    throw;
        //}
        //catch { }

        //// "NaN" --> exception
        //try
        //{
        //    res = DBConversion.ToInteger("NaN");
        //    Assert.Fail("DBConversion.ToInteger(\"NaN\") should have thrown an exception, but instead returned " + res);
        //}
        //catch (AssertFailedException)
        //{
        //    throw;
        //}
        //catch { }

        //// empty string --> exception
        //try
        //{
        //    res = DBConversion.ToInteger("");
        //    Assert.Fail("DBConversion.ToInteger(\"\") should have thrown an exception, but instead returned " + res);
        //}
        //catch (AssertFailedException)
        //{
        //    throw;
        //}
        //catch { }
    }

    /// <summary>
    /// Teste à função FromInteger
    /// </summary>
    [Test]
    public void TestFromInteger()
    {

        // Como DBConversion.FromInteger é essencialmente um Convert.ToString(int),
        // este test é algo ingrato

        string res = DBConversion.FromInteger(0);
        Assert.That(res, Is.EqualTo("0"));

        res = DBConversion.FromInteger(-2);
        Assert.That(res, Is.EqualTo("-2"));

        res = DBConversion.FromInteger(9000);
        Assert.That(res, Is.EqualTo("9000"));
    }

    /// <summary>
    /// Teste à função ToDateTime
    /// </summary>
    [Test]
    public void TestToDateTime()
    {
        DateTime res;
        DateTime expected;

        // null --> DateTime.MinValue
        res = DBConversion.ToDateTime(null);
        Assert.That(res, Is.EqualTo(DateTime.MinValue));

        // DateTime.MinValue --> DateTime.MinValue
        res = DBConversion.ToDateTime(DateTime.MinValue);
        Assert.That(res, Is.EqualTo(DateTime.MinValue));

        // DBNull.Value --> DateTime.MinValue
        res = DBConversion.ToDateTime(DBNull.Value);
        Assert.That(res, Is.EqualTo(DateTime.MinValue));

        // DateTime.Now ok
        expected = DateTime.Now;
        res = DBConversion.ToDateTime(expected);
        Assert.That(res, Is.EqualTo(expected));

        // DateTime.Today ok
        expected = DateTime.Today;
        res = DBConversion.ToDateTime(expected);
        Assert.That(res, Is.EqualTo(expected));

        // 1900-01-01 ok
        expected = new DateTime(1900, 1, 1);
        res = DBConversion.ToDateTime(expected);
        Assert.That(res, Is.EqualTo(expected));

        // 1999-12-31 ok
        expected = new DateTime(1999, 12, 31);
        res = DBConversion.ToDateTime(expected);
        Assert.That(res, Is.EqualTo(expected));

        // 2012-12-22 ok [the world has ended yet]
        expected = new DateTime(2012, 12, 22);
        res = DBConversion.ToDateTime(expected);
        Assert.That(res, Is.EqualTo(expected));

        // DateTime.MaxValue ok
        res = DBConversion.ToDateTime(DateTime.MaxValue);
        Assert.That(res, Is.EqualTo(DateTime.MaxValue));

        // a string --> exception
        Assert.That(() => DBConversion.ToDateTime(DateTime.Now.ToString()), Throws.Exception);

        // empty string --> exception
        Assert.That(() => DBConversion.ToDateTime(""), Throws.Exception);

        // an integer --> exception
        Assert.That(() => DBConversion.ToDateTime(20110411), Throws.Exception);
    }

    /// <summary>
    ///Teste à função FromDateTime
    /// </summary>
    [Test]
    public void TestFromDateTime()
    {
        string res = DBConversion.FromDateTime(new DateTime(1900, 2, 1), DatabaseType.SQLSERVER);
        Assert.That(res, Is.EqualTo("convert(datetime, '1/2/1900 00:00:00', 103)"));

        res = DBConversion.FromDateTime(new DateTime(1900, 2, 1), DatabaseType.ORACLE);
        Assert.That(res, Is.EqualTo("TO_DATE('1900/2/1 0:0:0', 'YYYY/MM/DD hh24:mi:ss')"));
    }

    /// <summary>
    /// Teste à função ToLogic
    /// </summary>
    [Test]
    public void TestToLogic()
    {

        // DBNull.Value --> 0
        int res = DBConversion.ToLogic(DBNull.Value);
        Assert.That(res, Is.EqualTo(0));

        // null --> 0
        res = DBConversion.ToLogic(null);
        Assert.That(res, Is.EqualTo(0));

        //
        // From booleans
        //

        // false --> 0
        res = DBConversion.ToLogic(false);
        Assert.That(res, Is.EqualTo(0));

        // true --> 1
        res = DBConversion.ToLogic(true);
        Assert.That(res, Is.EqualTo(1));

        //
        // From integers
        //

        // 0 --> 0
        res = DBConversion.ToLogic(0);
        Assert.That(res, Is.EqualTo(0));

        // 1 --> 1
        res = DBConversion.ToLogic(1);
        Assert.That(res, Is.EqualTo(1));

        // 42 --> 1
        //res = DBConversion.ToLogic(42);  // FIXME está a falhar. Será que devia passar?
        //Assert.AreEqual(1, res);
    }

    /// <summary>
    /// Teste à função FromLogic
    /// </summary>
    [Test]
    public void TestFromLogic()
    {
        string res = DBConversion.FromLogic(int.MinValue);
        //Assert.AreEqual(?, res);

        res = DBConversion.FromLogic(-1);
        Assert.That(res, Is.EqualTo("-1")); // Ou erro?

        res = DBConversion.FromLogic(0);
        Assert.That(res, Is.EqualTo("0"));

        res = DBConversion.FromLogic(1);
        Assert.That(res, Is.EqualTo("1"));

        res = DBConversion.FromLogic(int.MaxValue);
        //Assert.AreEqual(?, res);
    }

    /// <summary>
    /// Teste à função ToKey
    /// </summary>
    [Test]
    public void TestToKey()
    {

        // null --> empty string
        string res = DBConversion.ToKey(null);
        Assert.That(res, Is.EqualTo(""));

        // empty string --> empty string
        res = DBConversion.ToKey(String.Empty);
        Assert.That(res, Is.EqualTo(""));

        // DBNull --> empty string
        res = DBConversion.ToKey(DBNull.Value);
        Assert.That(res, Is.EqualTo(""));

        // empty GUID --> empty string
        res = DBConversion.ToKey(Guid.Empty);
        Assert.That(res, Is.EqualTo(""));

        // zeroed GUID as a SQL string --> empty string
        res = DBConversion.ToKey("'00000000-0000-0000-0000-000000000000'"); // FIXME está a falhar. Será suposto?
        Assert.That(res, Is.EqualTo(""));

        // non-empty empty GUID as a SQL string --> same GUID (with quotes removed)
        res = DBConversion.ToKey("'a6bb0086-8c37-4b88-8235-161f76134a2a'");
        Assert.That(res, Is.EqualTo("a6bb0086-8c37-4b88-8235-161f76134a2a"));

    }

    /// <summary>
    /// Teste à função FromKey
    /// </summary>
    [Test]
    public void TestFromKey()
    {

        // empty string --> NULL
        string res = DBConversion.FromKey(string.Empty);
        Assert.That(res, Is.EqualTo("NULL"));

        // null --> NULL
        res = DBConversion.FromKey(null);
        Assert.That(res, Is.EqualTo("NULL"));

        // empty GUID (as string) --> NULL
        res = DBConversion.FromKey(Guid.Empty.ToString());
        Assert.That(res, Is.EqualTo("NULL"));

        // zeroed GUID (no quotes) --> NULL
        res = DBConversion.FromKey("00000000-0000-0000-0000-000000000000");
        Assert.That(res, Is.EqualTo("NULL"));

        // non-empty GUID (no quotes) --> same GUID (quoted)
        res = DBConversion.FromKey("a6bb0086-8c37-4b88-8235-161f76134a2a");
        Assert.That(res, Is.EqualTo("'a6bb0086-8c37-4b88-8235-161f76134a2a'"));

    }

    /// <summary>
    /// Teste à função ToBinary
    /// </summary>
    [Test]
    public void TestToBinary()
    {

    }

    /// <summary>
    /// Teste à função FromBinary
    /// </summary>
    [Test]
    public void TestFromBinary()
    {
    }

    /// <summary>
    /// Teste às funções ToInternal e FromInternal
    /// </summary>
    [Test]
    public void TestToInterno()
    {

        // var value in System.Enum.GetValues(typeof(FieldFormatting)) ){}
        /*
        FieldFormatting.BINARIO;
        FieldFormatting.CARACTERES;
        FieldFormatting.DATA;
        FieldFormatting.DATAHORA;
        FieldFormatting.DATASEGUNDO;
        FieldFormatting.FLOAT;
        FieldFormatting.GUID;
        FieldFormatting.INTEIRO;
        FieldFormatting.JPEG;
        FieldFormatting.LOGICO;
        */

        //
        // FieldFormatting.BINARIO
        //

        // null --> zero-length byte array
        object res = DBConversion.ToInternal(null, FieldFormatting.BINARIO);
        Assert.IsInstanceOf(typeof(byte[]), res);
        Assert.IsTrue((res as byte[])!.Length == 0);

        // DBNull --> zero-length byte array
        res = DBConversion.ToInternal(DBNull.Value, FieldFormatting.BINARIO);
        Assert.IsInstanceOf(typeof(byte[]), res);
        Assert.IsTrue((res as byte[])!.Length == 0);

        //
        // FieldFormatting.CARACTERES
        //

        // null --> empty string
        res = DBConversion.ToInternal(null, FieldFormatting.CARACTERES);
        Assert.IsInstanceOf(typeof(string), res);
        Assert.That(res, Is.EqualTo(""));

        //
        // FieldFormatting.DATA
        //

        // null --> DateTime.MinValue
        res = DBConversion.ToInternal(null, FieldFormatting.DATA);
        Assert.IsInstanceOf(typeof(DateTime), res);
        Assert.That(res, Is.EqualTo(DateTime.MinValue));

        // DateTime.MinValue --> DateTime.MinValue
        res = DBConversion.ToInternal(DateTime.MinValue, FieldFormatting.DATA);
        Assert.IsInstanceOf(typeof(DateTime), res);
        Assert.That(res, Is.EqualTo(DateTime.MinValue));

        // DBNull --> DateTime.MinValue
        res = DBConversion.ToInternal(DBNull.Value, FieldFormatting.DATA);
        Assert.IsInstanceOf(typeof(DateTime), res);
        Assert.That(res, Is.EqualTo(DateTime.MinValue));

        //
        // FieldFormatting.DATAHORA;
        //

        //
        // FieldFormatting.DATASEGUNDO;
        //

        //
        // FieldFormatting.FLOAT;
        //

        // null --> 0.0
        res = DBConversion.ToInternal(null, FieldFormatting.FLOAT);
        Assert.That(res, Is.EqualTo(0m));

        // DBNull --> 0.0
        res = DBConversion.ToInternal(DBNull.Value, FieldFormatting.FLOAT);
        Assert.That(res, Is.EqualTo(0m));

        // With conversion to String with current culture
        res = DBConversion.ToInternal(0.9464572.ToString(), FieldFormatting.FLOAT);
        Assert.That(res, Is.EqualTo(0.9464572m));

        // With conversion to string in Invariant culture
        res = DBConversion.ToInternal(0.9464572.ToString(System.Globalization.CultureInfo.InvariantCulture), FieldFormatting.FLOAT); 
        Assert.That(res, Is.EqualTo(0.9464572m));

        res = DBConversion.ToInternal(0.987654321, FieldFormatting.FLOAT);
        Assert.That(res, Is.EqualTo(0.987654321m));

        //
        // FieldFormatting.GUID;
        //

        //
        // FieldFormatting.INTEIRO;
        //

        //
        // FieldFormatting.JPEG;
        //

        //
        // FieldFormatting.LOGICO;
        //
    }

    /// <summary>
    /// Teste à função FromInternal
    /// </summary>
    [Test]
    public void TestFromInterno()
    {
        //
        // FieldFormatting.CARACTERES
        //
        
        // null --> empty string
        string res = DBConversion.FromInternal(null, FieldFormatting.CARACTERES);
        Assert.That(res, Is.EqualTo("''"));

        // empty string --> empty string
        res = DBConversion.FromInternal("", FieldFormatting.CARACTERES);
        Assert.That(res, Is.EqualTo("''"));

        // two single quotes --> two escaped single quotes
        res = DBConversion.FromInternal("''", FieldFormatting.CARACTERES);
        Assert.That(res, Is.EqualTo("''''''"));

        // a couple of spaces --> a couple of spaces
        res = DBConversion.FromInternal("  ", FieldFormatting.CARACTERES);
        Assert.That(res, Is.EqualTo("'  '"));

        // a string surrounded by quotes and with an accented character --> same string with escaped quotes
        res = DBConversion.FromInternal("'Abrenúncio'!", FieldFormatting.CARACTERES);
        Assert.That(res, Is.EqualTo("'''Abrenúncio''!'"));

        // a few spaces and a couple of single quotes --> same string with escaped quotes
        res = DBConversion.FromInternal("   ''", FieldFormatting.CARACTERES);
        Assert.That(res, Is.EqualTo("'   '''''"));

        // a string that looks like a number --> same string
        res = DBConversion.FromInternal("123456", FieldFormatting.CARACTERES);
        Assert.That(res, Is.EqualTo("'123456'"));

        //
        // FieldFormatting.DATA
        //
        
        // null --> exception
        Assert.That(() => DBConversion.FromInternal(null, FieldFormatting.DATA), Throws.Exception);

        // DateTime.MinValue --> NULL
        res = DBConversion.FromInternal(DateTime.MinValue, FieldFormatting.DATA);
        Assert.That(res, Is.EqualTo("NULL"));

        // empty string --> NULL should throw exception
        Assert.That(() => DBConversion.FromInternal("", FieldFormatting.DATA), Throws.Exception);
        
        //
        // FieldFormatting.DATAHORA
        //

        //
        // FieldFormatting.DATASEGUNDO
        //

        //
        // FieldFormatting.FLOAT
        //

        //
        // FieldFormatting.GUID
        //
        
        // empty GUID --> empty string
        res = DBConversion.FromInternal(System.Guid.Empty, FieldFormatting.BINARIO);
        Assert.That(res, Is.EqualTo("''"));

        // zeroed GUID --> empty string
        res = DBConversion.FromInternal("00000000-0000-0000-0000-000000000000", FieldFormatting.BINARIO);
        Assert.That(res, Is.EqualTo("''"));

        // empty string --> empty string
        res = DBConversion.FromInternal("''", FieldFormatting.BINARIO);
        Assert.That(res, Is.EqualTo("''"));

        // two double quotes --> ???
        res = DBConversion.FromInternal("\"\"", FieldFormatting.BINARIO);
        Assert.That(res, Is.EqualTo("''"));
        
    }

}
