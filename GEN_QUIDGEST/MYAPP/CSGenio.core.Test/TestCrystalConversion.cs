using CSGenio.framework;

namespace CSGenio.core.Test;

/// <summary>
///This is a test class for Test and is intended
///to contain all Test Unit Tests
///</summary>
public class TestConversaoCrystal
{

    /// <summary>
    /// Teste aos metodos de To(xxx) que devem retornar uma excepção de operação invalida
    /// </summary>
    [Test]
    public void TestToMethods()
    {
        Assert.That(() => CrystalConversion.ToString(null), Throws.Exception);
        Assert.That(() => CrystalConversion.ToInteger(null), Throws.Exception);
        Assert.That(() => CrystalConversion.ToNumeric(null), Throws.Exception);
        Assert.That(() => CrystalConversion.ToDateTime(null), Throws.Exception);
        Assert.That(() => CrystalConversion.ToKey(null), Throws.Exception);
        Assert.That(() => CrystalConversion.ToLogic(null), Throws.Exception);
        Assert.That(() => CrystalConversion.ToBinary(null), Throws.Exception);
        Assert.That(() => CrystalConversion.ToInternal(null, FieldFormatting.CARACTERES), Throws.Exception);
    }

    /// <summary>
    /// Teste à função FromString
    /// </summary>
    [Test]
    public void TestFromString()
    {
        // null --> empty string
        string res = CrystalConversion.FromString(null);
        // Até mais ver, Qvalues "vazios" são representados por strings de comprimento zero, e não por NULL
        Assert.That(res, Is.EqualTo("\"\""));

        // empty string --> empty string
        res = CrystalConversion.FromString("");
        // Até mais ver, Qvalues "vazios" são representados por strings de comprimento zero, e não por NULL
        Assert.That(res, Is.EqualTo("\"\""));

        // single space --> single space
        res = CrystalConversion.FromString(" ");
        Assert.That(res, Is.EqualTo("\" \""));

        // string with a number --> string with a number
        res = CrystalConversion.FromString("134");
        Assert.That(res, Is.EqualTo("\"134\""));

        // bunch of Unicode characters --> same bunch of Unicode characters
        res = CrystalConversion.FromString("¡¢£¤¥¦§¨©ª«¬®¯°±²³´¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿ");
        Assert.That(res, Is.EqualTo("\"¡¢£¤¥¦§¨©ª«¬®¯°±²³´¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ÷ØÙÚÛÜÝÞŸ\""));
    }

    /// <summary>
    /// Teste à função FromInteger
    /// </summary>
    [Test]
    public void TestFromInteger()
    {
        // Como CrystalConversion.FromInteger é essencialmente um Convert.ToString(int),
        // este test é algo ingrato

        string res = CrystalConversion.FromInteger(0);
        Assert.That(res, Is.EqualTo("0"));

        res = CrystalConversion.FromInteger(-2);
        Assert.That(res, Is.EqualTo("-2"));

        res = CrystalConversion.FromInteger(9000);
        Assert.That(res, Is.EqualTo("9000"));
    }

    /// <summary>
    ///Teste à função FromDateTime
    /// </summary>
    [Test]
    public void TestFromDateTime()
    {
        string res = CrystalConversion.FromDateTime(DateTime.MinValue, true, true);
        Assert.That(res, Is.EqualTo(""));

        res = CrystalConversion.FromDateTime(new DateTime(1900, 2, 1), true, true);
        Assert.That(res, Is.EqualTo("DateTime(1900, 02, 01, 00, 00, 00)"));
    }

    /// <summary>
    /// Teste à função FromLogic
    /// </summary>
    [Test]
    public void TestFromLogic()
    {
        string res = CrystalConversion.FromLogic(int.MinValue);
        //Assert.AreEqual(?, res);

        res = CrystalConversion.FromLogic(-1);
        Assert.That(res, Is.EqualTo("-1")); // Ou erro?

        res = CrystalConversion.FromLogic(0);
        Assert.That(res, Is.EqualTo("0"));

        res = CrystalConversion.FromLogic(1);
        Assert.That(res, Is.EqualTo("1"));

        res = CrystalConversion.FromLogic(int.MaxValue);
        //Assert.AreEqual(?, res);
    }

    /// <summary>
    /// Teste à função FromKey
    /// </summary>
    [Test]
    public void TestFromKey()
    {
        // empty string --> NULL
        string res = CrystalConversion.FromKey(string.Empty);
        Assert.That(res, Is.EqualTo("\"{00000000-0000-0000-0000-000000000000}\""));

        // null --> NULL
        res = CrystalConversion.FromKey(null);
        Assert.That(res, Is.EqualTo("\"{00000000-0000-0000-0000-000000000000}\""));

        // empty GUID (as string) --> NULL
        res = CrystalConversion.FromKey(Guid.Empty.ToString());
        Assert.That(res, Is.EqualTo("\"{00000000-0000-0000-0000-000000000000}\""));

        // zeroed GUID (no quotes) --> NULL
        res = CrystalConversion.FromKey("00000000-0000-0000-0000-000000000000");
        Assert.That(res, Is.EqualTo("\"{00000000-0000-0000-0000-000000000000}\""));

        // non-empty GUID (no quotes) --> same GUID (quoted)
        res = CrystalConversion.FromKey("a6bb0086-8c37-4b88-8235-161f76134a2a");
        Assert.That(res, Is.EqualTo("\"{A6BB0086-8C37-4B88-8235-161F76134A2A}\""));
    }

    /// <summary>
    /// Teste à função FromBinary
    /// </summary>
    [Test]
    public void TestFromBinary()
    {
    }

    /// <summary>
    /// Teste à função FromInternal
    /// </summary>
    [Test]
    public void TestFromInterno()
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
        /*
        res = CrystalConversion.FromInternal(null, FieldFormatting.BINARIO);
        Assert.AreEqual("NULL", res);

        res = CrystalConversion.FromInternal("", FieldFormatting.BINARIO);
        Assert.AreEqual("", res);

        byte[] bytes = new byte[0];
        res = CrystalConversion.FromInternal(bytes, FieldFormatting.BINARIO);
        Assert.AreEqual("", res);
        */

        //
        // FieldFormatting.CARACTERES
        //

        // null --> empty string
        string res = CrystalConversion.FromInternal(null, FieldFormatting.CARACTERES);
        Assert.That(res, Is.EqualTo("\"\""));

        // empty string --> empty string
        res = CrystalConversion.FromInternal("", FieldFormatting.CARACTERES);
        Assert.That(res, Is.EqualTo("\"\""));

        // two single quotes --> two escaped single quotes
        res = CrystalConversion.FromInternal("''", FieldFormatting.CARACTERES);
        Assert.That(res, Is.EqualTo("\"''\""));

        // a couple of spaces --> a couple of spaces
        res = CrystalConversion.FromInternal("  ", FieldFormatting.CARACTERES);
        Assert.That(res, Is.EqualTo("\"  \""));

        // a string surrounded by quotes and with an accented character --> same string with escaped quotes
        res = CrystalConversion.FromInternal("'Abrenúncio'!", FieldFormatting.CARACTERES);
        Assert.That(res, Is.EqualTo("\"'ABRENÚNCIO'!\""));

        // a few spaces and a couple of single quotes --> same string with escaped quotes
        res = CrystalConversion.FromInternal("   ''", FieldFormatting.CARACTERES);
        Assert.That(res, Is.EqualTo("\"   ''\""));

        // a string that looks like a number --> same string
        res = CrystalConversion.FromInternal("123456", FieldFormatting.CARACTERES);
        Assert.That(res, Is.EqualTo("\"123456\""));

        //
        // FieldFormatting.DATA
        //

        // null --> exception
        Assert.That(() => CrystalConversion.FromInternal(null, FieldFormatting.DATA), Throws.Exception);

        // DateTime.MinValue --> NULL
        Assert.That(() => CrystalConversion.FromInternal(DateTime.MinValue, FieldFormatting.DATA), Is.EqualTo(""));

        // empty string --> NULL should throw exception
        Assert.That(() => CrystalConversion.FromInternal("", FieldFormatting.DATA), Throws.Exception);

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
        res = CrystalConversion.FromInternal(System.Guid.Empty, FieldFormatting.GUID);
        Assert.That(res, Is.EqualTo("\"{00000000-0000-0000-0000-000000000000}\""));

        // zeroed GUID --> empty string
        res = CrystalConversion.FromInternal("00000000-0000-0000-0000-000000000000", FieldFormatting.GUID);
        Assert.That(res, Is.EqualTo("\"{00000000-0000-0000-0000-000000000000}\""));

        // empty string --> empty string
        res = CrystalConversion.FromInternal("", FieldFormatting.CARACTERES);
        Assert.That(res, Is.EqualTo("\"\""));

        // two double quotes --> ???
        res = CrystalConversion.FromInternal("\"\"", FieldFormatting.CARACTERES);
        Assert.That(res, Is.EqualTo("\"\"\"\""));

        //
        // FieldFormatting.INTEIRO
        //

        //
        // FieldFormatting.JPEG
        //

        //
        // FieldFormatting.LOGICO
        //
    }
}
