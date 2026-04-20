import { describe, expect, test } from 'vitest'
import api from '@/api/genio/quidgestFunctions'

describe('quidgestFunctions', () => {
	const MIN_DATE = '';

	test('emptyD', () => {
		expect(api.emptyD(undefined)).toBe(1)
		expect(api.emptyD(null)).toBe(1)
		expect(api.emptyD(MIN_DATE)).toBe(1)
		expect(api.emptyD(new Date())).toBe(0)

		expect(api.emptyD(new Date(2010, 10, 31))).toBe(0)
		expect(api.emptyD('21/12/2022')).toBe(0) // DMA
		expect(api.emptyD('2022/12/21')).toBe(0) // AMD
		expect(api.emptyD('2022/12/21')).toBe(0) // ADM
		expect(api.emptyD('21-12-2022')).toBe(1)
		expect(api.emptyD('21/12/2022 12:38')).toBe(0)
		expect(api.emptyD('21/12/2022 11:38AM')).toBe(0)
		expect(api.emptyD('21/12/2022 12:38:59')).toBe(0)
		expect(api.emptyD('21/12/2022 11:38:59PM')).toBe(0)
		expect(api.emptyD('21/12/2022 11:38:59PM')).toBe(0)
		expect(api.emptyD('21/12/2022 11:38:59PM')).toBe(0)
		expect(api.emptyD('2022/12/21 11:38:59PM')).toBe(0)
		expect(api.emptyD('2022-10-07T11:39:48.067Z')).toBe(0)
		expect(api.emptyD('2022-10-07T11:39:48.067')).toBe(0)
		expect(api.emptyD('2022-10-07')).toBe(1)
		expect(api.emptyD('0001-01-01T00:00:00')).toBe(1)
		expect(api.emptyD(new Date('0001-01-01T00:00:00'))).toBe(1)
	})


	test('emptyC', () => {
		expect(api.emptyC(undefined)).toBe(1)
		expect(api.emptyC(null)).toBe(1)
		expect(api.emptyC("")).toBe(1)

		expect(api.emptyC("1234")).toBe(0)
		expect(api.emptyC("")).toBe(1)
		expect(api.emptyC(" ")).toBe(0)
		expect(api.emptyC("1")).toBe(0)
	})

	test('emptyL', () => {
		expect(api.emptyL(undefined)).toBe(1)
		expect(api.emptyL(null)).toBe(1)
		expect(api.emptyL("")).toBe(1)
		expect(api.emptyL(1234)).toBe(0)
		expect(api.emptyL(0)).toBe(1)
		expect(api.emptyL(0.0)).toBe(1)
		expect(api.emptyL(1)).toBe(0)
		expect(api.emptyL(1.0)).toBe(0)
		expect(api.emptyL(false)).toBe(1)
		expect(api.emptyL(true)).toBe(0)
	})

	test('emptyN', () => {
		expect(api.emptyN(undefined)).toBe(1)
		expect(api.emptyN(null)).toBe(1)
		expect(api.emptyN("")).toBe(1)
		expect(api.emptyN(Number.NaN)).toBe(1)

		expect(api.emptyN(1234.00)).toBe(0)
		expect(api.emptyN(1234)).toBe(0)
		expect(api.emptyN(0)).toBe(1)
		expect(api.emptyN(0.0)).toBe(1)
		expect(api.emptyN(-100)).toBe(0)
		expect(api.emptyN(-100.100)).toBe(0)
		expect(api.emptyN(-100.100)).toBe(0)
	})

	test('emptyG', () => {
		expect(api.emptyG(undefined)).toBe(1)
		expect(api.emptyG(null)).toBe(1)
		expect(api.emptyG("")).toBe(1)
		expect(api.emptyG("1234.00")).toBe(0)
		expect(api.emptyG("00000000-0000-0000-0000-000000000000")).toBe(1)
		expect(api.emptyG(" 1")).toBe(0)
	})

	test('emptyT', () => {
		expect(api.emptyT(undefined)).toBe(1)
		expect(api.emptyT(null)).toBe(1)
		expect(api.emptyT("")).toBe(1)
		expect(api.emptyT("__:__")).toBe(1)
		expect(api.emptyT("00:00")).toBe(0)
		expect(api.emptyT("20:18")).toBe(0)
	})

	test('IsValid', () => {
		expect(api.IsValid(undefined)).toBe(0)
		expect(api.IsValid(null)).toBe(0)
		expect(api.IsValid(MIN_DATE)).toBe(0)
		expect(api.IsValid(new Date(2010, 10, 31))).toBe(1)
	})

	test('KeyToString', () => {
		expect(api.KeyToString(undefined)).toBe("")
		expect(api.KeyToString(null)).toBe("")

		expect(api.KeyToString("")).toBe("")
		expect(api.KeyToString("0")).toBe("")
		expect(api.KeyToString("00000000-0000-0000-0000-000000000000")).toBe("")
		expect(api.KeyToString("{00000000-0000-0000-0000-000000000000}")).toBe("")

		expect(api.KeyToString("1")).toBe("1")
		expect(api.KeyToString("    1")).toBe("    1")
		expect(api.KeyToString("{234dceae-7c12-40e9-bbf5-b59f5f6dd890}")).toBe("234DCEAE7C1240E9BBF5B59F5F6DD890")
		expect(api.KeyToString("{234DCEAE-7C12-40E9-BBF5-B59F5F6DD890}")).toBe("234DCEAE7C1240E9BBF5B59F5F6DD890")
	})

	test('minD', () => {
		const date1 = new Date(2010, 10, 31);
		const date2 = new Date(2010, 10, 30);
		const date3 = new Date(2010, 10, 25);

		expect(api.minD(undefined, date1)).toBe(MIN_DATE)
		expect(api.minD(null, null)).toBe(MIN_DATE)
		expect(api.minD(date1, "")).toBe(MIN_DATE)

		expect(api.minD(date1, date2)).toBe(date2)
		expect(api.minD(date2, date3)).toBe(date3)
		expect(api.minD(MIN_DATE, date3)).toBe(MIN_DATE)
	})

	test('minN', () => {

		expect(api.minN(2.0, undefined)).toBe(0)
		expect(api.minN(null, null)).toBe(0)
		expect(api.minN(2.0, "")).toBe(0)
		expect(api.minN(1.023531, 1.029)).toBe(1.023531)
		expect(api.minN(2.0, Number.NaN)).toBe(0)
		expect(api.minN(2.0, 2.0)).toBe(2)
		expect(api.minN(-1.0, 0.0)).toBe(-1.0)
	})

	test('maxD', () => {
		const date1 = new Date(2010, 10, 31);
		const date2 = new Date(2010, 10, 30);
		const date3 = new Date(2010, 10, 25);

		expect(api.maxD(undefined, date1)).toBe(date1)
		expect(api.maxD(null, undefined)).toBe(MIN_DATE)
		expect(api.maxD(date1, "")).toBe(date1)
		expect(api.maxD(MIN_DATE, null)).toBe(MIN_DATE)

		expect(api.maxD(date1, date2)).toBe(date1)
		expect(api.maxD(date2, date3)).toBe(date2)
		expect(api.maxD(MIN_DATE, date3)).toBe(date3)
	})

	test('maxN', () => {
		expect(api.maxN(2.0, undefined)).toBe(2)
		expect(api.maxN(null, null)).toBe(0)
		expect(api.maxN(2.0, "")).toBe(2)
		expect(api.maxN(1.023531, 1.029)).toBe(1.029)
		expect(api.maxN(2.0, Number.NaN)).toBe(2)
		expect(api.maxN(2.0, 2.0)).toBe(2)
		expect(api.maxN(-1.0, 0.0)).toBe(0.0)
	})

	test('Year', () => {
		expect(api.Year(undefined)).toBe(0)
		expect(api.Year(null)).toBe(0)
		expect(api.Year('')).toBe(0)
		expect(api.Year(new Date(2010, 10, 25))).toBe(2010)
		expect(api.Year(new Date(2012, 11, 15))).toBe(2012)
		expect(api.Year(MIN_DATE)).toBe(0)
	})

	test('Month', () => {
		expect(api.Month(undefined)).toBe(0)
		expect(api.Month(null)).toBe(0)
		expect(api.Month('')).toBe(0)

		expect(api.Month(new Date(2010, 10, 25))).toBe(11)
		expect(api.Month(new Date(2012, 11, 15))).toBe(12)
		expect(api.Year(MIN_DATE)).toBe(0)
	})

	test('Day', () => {
		expect(api.Day(undefined)).toBe(0)
		expect(api.Day(null)).toBe(0)
		expect(api.Day('')).toBe(0)
		expect(api.Day(new Date(2010, 10, 25))).toBe(25)
		expect(api.Day(new Date(2012, 11, 15))).toBe(15)
		expect(api.Year(MIN_DATE)).toBe(0)
	})

	test('LEFT', () => {
		expect(api.LEFT(undefined, 2)).toBe('')
		expect(api.LEFT(null, 2)).toBe('')
		expect(api.LEFT("blabla", undefined)).toBe('')
		expect(api.LEFT("blabla", null)).toBe('')
		expect(api.LEFT("blabla", "")).toBe('')
		expect(api.LEFT("blabla", -2)).toBe('')

		expect(api.LEFT("ola adeus", 3)).toBe("ola")
		expect(api.LEFT("ola adeus", 30)).toBe("ola adeus")
	})

	test('RIGHT', () => {
		expect(api.RIGHT(undefined, 3)).toBe('')
		expect(api.RIGHT(null, 10)).toBe('')
		expect(api.RIGHT("blabla", undefined)).toBe('')
		expect(api.RIGHT("blabla", null)).toBe('')
		expect(api.RIGHT("blabla", "")).toBe('')
		expect(api.RIGHT("blabla", -1)).toBe('')

		expect(api.RIGHT("ola adeus", 5)).toBe("adeus")
		expect(api.RIGHT("ola adeus", 30)).toBe("ola adeus")
	})

	test('SubString', () => {
		expect(api.SubString(undefined, 2, 3)).toBe('')
		expect(api.SubString(null, 1, 1)).toBe('')
		expect(api.SubString("", 2, 3)).toBe('')
		expect(api.SubString("blabla", undefined, 3)).toBe('')
		expect(api.SubString("blabla", null, 3)).toBe('')
		expect(api.SubString("blabla", "", 3)).toBe('')
		expect(api.SubString("blabla", -1, 3)).toBe('')
		expect(api.SubString("blabla", 20, 3)).toBe('')
		expect(api.SubString("blabla", 2, undefined)).toBe('')
		expect(api.SubString("blabla", 2, null)).toBe('')
		expect(api.SubString("blabla", 2, "")).toBe('')
		expect(api.SubString("blabla", 2, -2)).toBe('')

		expect(api.SubString("ola adeus", 2, 3)).toBe("a a")
		expect(api.SubString("ola adeus", 4, 30)).toBe("adeus")
	})


	test('LTRIM', () => {
		expect(api.LTRIM(undefined)).toBe("")
		expect(api.LTRIM(null)).toBe("")
		expect(api.LTRIM("tre jolie")).toBe("tre jolie")
		expect(api.LTRIM(" \r\n tre jolie")).toBe("tre jolie")
		expect(api.LTRIM("tre jolie \r\n")).toBe("tre jolie \r\n")
		expect(api.LTRIM(" \r\n tre jolie \r\n")).toBe("tre jolie \r\n")
		expect(api.LTRIM("")).toBe("")
	})

	test('RTRIM', () => {
		expect(api.RTRIM(undefined)).toBe("")
		expect(api.RTRIM(null)).toBe("")
		expect(api.RTRIM("tre jolie")).toBe("tre jolie")
		expect(api.RTRIM(" \r\n tre jolie")).toBe(" \r\n tre jolie")
		expect(api.RTRIM("tre jolie \r\n")).toBe("tre jolie")
		expect(api.RTRIM(" \r\n tre jolie \r\n")).toBe(" \r\n tre jolie")
		expect(api.RTRIM("")).toBe("")
	})


	test('atoi', () => {
		expect(api.atoi(undefined)).toBe(0)
		expect(api.atoi(null)).toBe(0)
		expect(api.atoi("BLABLA")).toBe(0)
		expect(api.atoi("95,87")).toBe(0)
		expect(api.atoi("95.87")).toBe(95)
		expect(api.atoi("1234")).toBe(1234)
		expect(api.atoi("")).toBe(0)
		expect(api.atoi("-")).toBe(0)
		expect(api.atoi("-4567")).toBe(-4567)
	})

	test('Round', () => {
		expect(api.Round(undefined, 1)).toBe(0)
		expect(api.Round(12345.55, undefined)).toBe(12346)
		expect(api.Round(null, 1)).toBe(0)
		expect(api.Round(12345.55, null)).toBe(12346)
		//Function's Special Cases
		expect(api.Round(0.0, 0)).toBe(0)
		expect(api.Round(0.5000001, 0)).toBe(1)
		expect(api.Round(0.50, 0)).toBe(1)
		expect(api.Round(0.4999999, 0)).toBe(0)
		//Round to even with 0 decimal places
		expect(api.Round(12346.5, 0)).toBe(12347)
		expect(api.Round(12346.4, 0)).toBe(12346)
		expect(api.Round(12346.49, 0)).toBe(12346)
		expect(api.Round(12346.4999999, 0)).toBe(12346)
		//Round to odd with 0 decimal places
		expect(api.Round(12343.5, 0)).toBe(12344)
		expect(api.Round(12343.4, 0)).toBe(12343)
		expect(api.Round(12343.4999999, 0)).toBe(12343)
		expect(api.Round(12343.49, 0)).toBe(12343)
		//Round to even with 1 decimal place
		expect(api.Round(12345.65, 1)).toBe(12345.7)
		expect(api.Round(12345.64, 1)).toBe(12345.6)
		expect(api.Round(12345.649, 1)).toBe(12345.6)
		expect(api.Round(12345.649999999, 1)).toBe(12345.6)
		//Round to odd with 1 decimal places
		expect(api.Round(12345.55, 1)).toBe(12345.6)
		expect(api.Round(12345.54, 1)).toBe(12345.5)
		expect(api.Round(12345.549, 1)).toBe(12345.5)
		expect(api.Round(12345.54999999, 1)).toBe(12345.5)
		//Round to even with 2 decimal places and negatives
		expect(api.Round(-12345.065, 2)).toBe(-12345.07)
		expect(api.Round(-12345.064, 2)).toBe(-12345.06)
		expect(api.Round(-12345.0649, 2)).toBe(-12345.06)
		expect(api.Round(-12345.064999999, 2)).toBe(-12345.06)
		//Round to odd with 2 decimal places and negatives
		expect(api.Round(-12345.055, 2)).toBe(-12345.06)
		expect(api.Round(-12345.054, 2)).toBe(-12345.05)
		expect(api.Round(-12345.0549, 2)).toBe(-12345.05)
		expect(api.Round(-12345.054999999, 2)).toBe(-12345.05)
	})

	test("RoundQG", () => {

		expect(api.RoundQG(undefined, 1)).toBe(0)
		expect(api.RoundQG(12345.55, undefined)).toBe(12346)
		expect(api.RoundQG(null, 1)).toBe(0)
		expect(api.RoundQG(12345.55, null)).toBe(12346)

		//Function's Special Cases
		expect(api.RoundQG(0.0, 0)).toBe(0)
		expect(api.RoundQG(0.50, 0)).toBe(1)
		expect(api.RoundQG(0.9999999, 0)).toBe(1)
		expect(api.RoundQG(0.499, 0)).toBe(1)
		expect(api.RoundQG(0.489, 0)).toBe(0)
		//Round to even with 0 decimal places
		expect(api.RoundQG(12346.5, 0)).toBe(12347)
		expect(api.RoundQG(12346.4, 0)).toBe(12346)
		expect(api.RoundQG(12346.49, 0)).toBe(12346)
		expect(api.RoundQG(12346.498, 0)).toBe(12346)
		expect(api.RoundQG(12346.498999999, 0)).toBe(12346)
		expect(api.RoundQG(12346.499, 0)).toBe(12347)
		//Round to odd with 0 decimal places
		expect(api.RoundQG(12343.5, 0)).toBe(12344)
		expect(api.RoundQG(12343.4, 0)).toBe(12343)
		expect(api.RoundQG(12343.49, 0)).toBe(12343)
		expect(api.RoundQG(12343.498, 0)).toBe(12343)
		expect(api.RoundQG(12343.498999999, 0)).toBe(12343)
		expect(api.RoundQG(12343.499, 0)).toBe(12344)
		//Round to even with 1 decimal place
		expect(api.RoundQG(12345.65, 1)).toBe(12345.7)
		expect(api.RoundQG(12345.64, 1)).toBe(12345.6)
		expect(api.RoundQG(12345.649, 1)).toBe(12345.6)
		expect(api.RoundQG(12345.6498, 1)).toBe(12345.6)
		expect(api.RoundQG(12345.6498999999, 1)).toBe(12345.6)
		expect(api.RoundQG(12345.6499, 1)).toBe(12345.7)
		//Round to odd with 1 decimal place
		expect(api.RoundQG(12345.55, 1)).toBe(12345.6)
		expect(api.RoundQG(12345.54, 1)).toBe(12345.5)
		expect(api.RoundQG(12345.549, 1)).toBe(12345.5)
		expect(api.RoundQG(12345.5498, 1)).toBe(12345.5)
		expect(api.RoundQG(12345.5498999999, 1)).toBe(12345.5)
		expect(api.RoundQG(12345.5499, 1)).toBe(12345.6)
		//Round to even with 2 decimal places and negatives
		expect(api.RoundQG(-12345.065, 2)).toBe(-12345.07)
		expect(api.RoundQG(-12345.064, 2)).toBe(-12345.06)
		expect(api.RoundQG(-12345.0649, 2)).toBe(-12345.06)
		expect(api.RoundQG(-12345.06498, 2)).toBe(-12345.06)
		expect(api.RoundQG(-12345.06498999999, 2)).toBe(-12345.06)
		expect(api.RoundQG(-12345.06499, 2)).toBe(-12345.07)
		//Round to odd with 2 decimal places and negatives
		expect(api.RoundQG(-12345.055, 2)).toBe(-12345.06)
		expect(api.RoundQG(-12345.054, 2)).toBe(-12345.05)
		expect(api.RoundQG(-12345.0549, 2)).toBe(-12345.05)
		expect(api.RoundQG(-12345.05498, 2)).toBe(-12345.05)
		expect(api.RoundQG(-12345.05498999999, 2)).toBe(-12345.05)
		expect(api.RoundQG(-12345.05499, 2)).toBe(-12345.06)
	})


	test("IntToString", () => {
		expect(api.IntToString(undefined)).toBe("0")
		expect(api.IntToString(null)).toBe("0")
		expect(api.IntToString("")).toBe("0")
		expect(api.IntToString(0.5)).toBe("0")
		expect(api.IntToString(-0.5)).toBe("0")
		expect(api.IntToString(0)).toBe("0")
		expect(api.IntToString(-1)).toBe("-1")
		expect(api.IntToString(1234)).toBe("1234")
	})

	test("NumericToString", () => {
		expect(api.NumericToString(undefined, 2)).toBe("0")
		expect(api.NumericToString(null, 3)).toBe("0")
		expect(api.NumericToString(23, undefined)).toBe("23")
		expect(api.NumericToString(23, null)).toBe("23")
		expect(api.NumericToString(-100.1234, -2)).toBe("-100")
		expect(api.NumericToString(100, "")).toBe("100")
		expect(api.NumericToString("", 2)).toBe("0")
		expect(api.NumericToString(23, 3)).toBe("23")
		expect(api.NumericToString(23.123, 3)).toBe("23,123")
		expect(api.NumericToString(100.123, 0)).toBe("100")
		expect(api.NumericToString(-100.123, 1)).toBe("-100,1")
	})

	test("HorasToDouble", () => {
		expect(api.HorasToDouble(undefined)).toBe(0.0)
		expect(api.HorasToDouble(null)).toBe(0.0)
		expect(api.HorasToDouble("asdfasdfad")).toBe(0.0)
		expect(api.HorasToDouble("")).toBe(0.0)
		expect(api.HorasToDouble("__:__")).toBe(0.0)
		expect(api.HorasToDouble("25:30")).toBe(0.0)
		expect(api.HorasToDouble("12:70")).toBe(0.0)
		expect(api.HorasToDouble("10:30")).toBe(10 + 30 / 60)
		expect(api.HorasToDouble("10:01")).toBe(10 + 1.0 / 60)
		expect(api.HorasToDouble("_1:_1")).toBe(1.0 + 1.0 / 60)
		expect(api.HorasToDouble("8:30")).toBe(0)
	})

	test("DoubleToHoras", () => {
		expect(api.DoubleToHoras(undefined)).toBe("00:00")
		expect(api.DoubleToHoras(null)).toBe("00:00")
		expect(api.DoubleToHoras(Number.NaN)).toBe("00:00")
		expect(api.DoubleToHoras(-10 + 1.0 / 60)).toBe("00:00")
		expect(api.DoubleToHoras(25 + 72 / 60)).toBe("26:12")
		expect(api.DoubleToHoras(10 + 30 / 60)).toBe("10:30")
		expect(api.DoubleToHoras(10 + 1.0 / 60)).toBe("10:01")
	})

	test("HorasAdd", () => {
		expect(api.HorasAdd(undefined, 2.0)).toBe("__:__")
		expect(api.HorasAdd(null, 2.0)).toBe("__:__")
		expect(api.HorasAdd("", 2.0)).toBe("__:__")
		expect(api.HorasAdd(Number.NaN, undefined)).toBe("__:__")
		expect(api.HorasAdd("23:", 2.0)).toBe("__:__")
		expect(api.HorasAdd("23:512", 2.0)).toBe("__:__")
		expect(api.HorasAdd("20:5", 5)).toBe("__:__")
		expect(api.HorasAdd("20:30", undefined)).toBe("20:30")
		expect(api.HorasAdd("20:30", null)).toBe("20:30")
		expect(api.HorasAdd("20:30", "")).toBe("20:30")
		expect(api.HorasAdd("00:00", 1.0)).toBe("00:01")
		expect(api.HorasAdd("02:00", -1.0)).toBe("01:59")
		expect(api.HorasAdd("02:03", 57.0)).toBe("03:00")
		expect(api.HorasAdd("00:00", 24.0 * 60.0)).toBe("23:59")
		expect(api.HorasAdd("23:59", -24.0 * 60)).toBe("00:00")
		expect(api.HorasAdd("20:5_", 5.0)).toBe("20:55")
		expect(api.HorasAdd("02:00", 2.0)).toBe("02:02")
	})

	test("CriaDataHora", () => {
		const date1 = new Date(2010, 10, 30)
		const date2 = new Date(2010, 10, 30, 20, 30, 0)

		expect(api.CriaDataHora("", "10:00")).toStrictEqual(MIN_DATE)
		expect(api.CriaDataHora(undefined, "10:00")).toStrictEqual(MIN_DATE)
		expect(api.CriaDataHora(null, "10:00")).toStrictEqual(MIN_DATE)
		expect(api.CriaDataHora(date1, undefined)).toStrictEqual(date1)
		expect(api.CriaDataHora(date1, null)).toStrictEqual(date1)
		expect(api.CriaDataHora(date1, "")).toStrictEqual(date1)
		expect(api.CriaDataHora(date1, "20:352")).toStrictEqual(date1)
		expect(api.CriaDataHora(date1, "20:")).toStrictEqual(date1)
		expect(api.CriaDataHora(date1, "30:30")).toStrictEqual(date1)
		expect(api.CriaDataHora(date1, "20:80")).toStrictEqual(date1)

		expect(api.CriaDataHora(date1, "20:30")).toStrictEqual(date2)
		expect(api.CriaDataHora(MIN_DATE, "20:30")).toStrictEqual(MIN_DATE)
	})


	test("CriaData", () => {
		//Undefined
		expect(api.CriaData(undefined, 2, 20, 20, 20, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2016, undefined, 20, 20, 20, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2016, 2, undefined, 20, 20, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2016, 2, 20, undefined, 20, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2016, 2, 20, 20, undefined, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2016, 2, 20, 20, 20, undefined)).toStrictEqual(MIN_DATE)
		//Null
		expect(api.CriaData(null, 2, 20, 20, 20, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2016, null, 20, 20, 20, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2016, 2, null, 20, 20, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2016, 2, 20, null, 20, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2016, 2, 20, 20, null, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2016, 2, 20, 20, 20, null)).toStrictEqual(MIN_DATE)
		//""
		expect(api.CriaData("", 2, 20, 20, 20, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2016, "", 20, 20, 20, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2016, 2, "", 20, 20, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2016, 2, 20, "", 20, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2016, 2, 20, 20, "", 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2016, 2, 20, 20, 20, "")).toStrictEqual(MIN_DATE)
		//Over limit
		expect(api.CriaData(2016, 2, 32, 20, 20, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2016, 14, 22, 20, 20, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2016, 2, 20, 25, 20, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2016, 1, 20, 20, 70, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2016, 2, 20, 20, 20, 70)).toStrictEqual(MIN_DATE)
		//Number Length
		expect(api.CriaData(20151234, 2, 29, 20, 20, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2015, 2123, 29, 20, 20, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2015, 2, 291, 20, 20, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2015, 2, 29, 2012, 20, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2015, 2, 29, 20, 2012, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2015, 2, 29, 20, 20, 201)).toStrictEqual(MIN_DATE)
		//February
		expect(api.CriaData(2016, 2, 30, 20, 20, 20)).toStrictEqual(MIN_DATE)
		expect(api.CriaData(2015, 2, 29, 20, 20, 20)).toStrictEqual(MIN_DATE)

		expect(api.CriaData(2016, 2, 20, 20, 20, 20)).toStrictEqual(new Date(2016, 1, 20, 20, 20, 20))
		expect(api.CriaData(2016, 2, 29, 20, 20, 20)).toStrictEqual(new Date(2016, 1, 29, 20, 20, 20))
	})


	test("ComparaDatas", () => {
		expect(api.ComparaDatas(new Date(2010, 10, 30), new Date(2010, 11, 5))).toBe(-1)
		expect(api.ComparaDatas(undefined, new Date(2010, 11, 5))).toBe(-1)
		expect(api.ComparaDatas(new Date(2010, 10, 30), new Date(2010, 10, 30))).toBe(0)
		expect(api.ComparaDatas(undefined, undefined)).toBe(0)
		expect(api.ComparaDatas(new Date(2010, 11, 5), new Date(2010, 10, 30))).toBe(1)
		expect(api.ComparaDatas(new Date(2010, 11, 5), undefined)).toBe(1)
	})

	test("LengthString", () => {
		expect(api.LengthString(undefined)).toBe(0)
		expect(api.LengthString(null)).toBe(0)
		expect(api.LengthString("")).toBe(0)
		expect(api.LengthString("bla")).toBe(3)
	})


	test("ValorIVA", () => {
		//Undefined
		expect(api.ValorIVA(undefined, 23, 1, 2)).toBe(0)
		expect(api.ValorIVA(1250, undefined, 1, 2)).toBe(0)
		expect(api.ValorIVA(1250, 23, undefined, 2)).toBe(0)
		expect(api.ValorIVA(1250, 23, 1, undefined)).toBe(0)
		//Null
		expect(api.ValorIVA(null, 23, 1, 2)).toBe(0)
		expect(api.ValorIVA(1250, null, 1, 2)).toBe(0)
		expect(api.ValorIVA(1250, 23, null, 2)).toBe(0)
		expect(api.ValorIVA(1250, 23, 1, null)).toBe(0)
		//""
		expect(api.ValorIVA("", 23, 1, 2)).toBe(0)
		expect(api.ValorIVA(1250, "", 1, 2)).toBe(0)
		expect(api.ValorIVA(1250, 23, "", 2)).toBe(0)
		expect(api.ValorIVA(1250, 23, 1, "")).toBe(0)
		//Precision
		expect(api.ValorIVA(1250, 23, 1, -2)).toBe(0)
		expect(api.ValorIVA(1250, 23, 5, 2)).toBe(0)

		expect(api.ValorIVA(1250, 23, 1, 2)).toBe(233.74)
		expect(api.ValorIVA(1250, 23, 0, 2)).toBe(287.5)
	})


	test("Incidenc", () => {
		//Undefined
		expect(api.Incidenc(undefined, 23, 10, 2)).toBe(0)
		expect(api.Incidenc(1250, undefined, 10, 2)).toBe(0)
		expect(api.Incidenc(1250, 23, undefined, 2)).toBe(0)
		expect(api.Incidenc(1250, 23, 10, undefined)).toBe(0)
		//Null
		expect(api.Incidenc(null, 23, 10, 2)).toBe(0)
		expect(api.Incidenc(1250, null, 10, 2)).toBe(0)
		expect(api.Incidenc(1250, 23, null, 2)).toBe(0)
		expect(api.Incidenc(1250, 23, 10, null)).toBe(0)
		//""
		expect(api.Incidenc("", 23, 10, 2)).toBe(0)
		expect(api.Incidenc(1250, "", 10, 2)).toBe(0)
		expect(api.Incidenc(1250, 23, "", 2)).toBe(0)
		expect(api.Incidenc(1250, 23, 10, "")).toBe(0)
		//Negatives
		expect(api.Incidenc(-1250, 23, 10, 2)).toBe(0)
		expect(api.Incidenc(1250, -23, 10, 2)).toBe(0)
		expect(api.Incidenc(1250, 23, 10, -2)).toBe(0)

		expect(api.Incidenc(1250, 23, 10, 2)).toBe(25875)
	})


	test("strAno", () => {
		expect(api.strAno(undefined)).toBe("0")
		expect(api.strAno(null)).toBe("0")
		expect(api.strAno("")).toBe("0")
		expect(api.strAno(new Date(2010, 10, 30))).toBe("2010")
	})


	test("SomaDias", () => {
		const date1 = new Date(2010, 10, 25)
		const date2 = new Date(2010, 11, 1)

		expect(api.SomaDias(undefined, 6)).toBe(MIN_DATE)
		expect(api.SomaDias(null, 6)).toBe(MIN_DATE)
		expect(api.SomaDias(date1, undefined)).toStrictEqual(date1)
		expect(api.SomaDias(date1, null)).toStrictEqual(date1)
		expect(api.SomaDias(date1, Number.NaN)).toStrictEqual(date1)

		expect(api.SomaDias(date1, 6)).toStrictEqual(date2)
		expect(api.SomaDias(date1, 0)).toStrictEqual(date1)
		expect(api.SomaDias(date2, -6)).toStrictEqual(date1)
	})

	test("Floor", () => {
		expect(api.Floor(undefined)).toBe(0)
		expect(api.Floor(null)).toBe(0)
		expect(api.Floor(24.5)).toBe(24)
	})

	test("Diferenca_entre_Datas", () => {
		const date1 = new Date(2010, 10, 30, 0, 0, 0);
		const date2 = new Date(2010, 11, 1, 0, 0, 0);

		expect(api.Diferenca_entre_Datas(undefined, date1, "D")).toBe(0)
		expect(api.Diferenca_entre_Datas(date1, undefined, "D")).toBe(0)
		expect(api.Diferenca_entre_Datas(null, date1, "D")).toBe(0)
		expect(api.Diferenca_entre_Datas(date1, null, "D")).toBe(0)
		expect(api.Diferenca_entre_Datas(MIN_DATE, date1, "D")).toBe(0)
		expect(api.Diferenca_entre_Datas(date1, date2, undefined)).toBe(0)
		expect(api.Diferenca_entre_Datas(date1, date2, null)).toBe(0)
		expect(api.Diferenca_entre_Datas(date1, date2, "")).toBe(0)

		expect(api.Diferenca_entre_Datas(date1, date2, "D")).toBe(1.0)
		expect(api.Diferenca_entre_Datas(date1, date2, "d")).toBe(1.0)
		expect(api.Diferenca_entre_Datas(date1, date2, "H")).toBe(24)
		expect(api.Diferenca_entre_Datas(date1, date2, "M")).toBe(24 * 60)
		expect(api.Diferenca_entre_Datas(date1, date2, "S")).toBe(24 * 60 * 60)
	})

	test("DateFloorDay", () => {
		expect(api.DateFloorDay(undefined)).toBe(MIN_DATE)
		expect(api.DateFloorDay(null)).toBe(MIN_DATE)
		expect(api.DateFloorDay("")).toBe(MIN_DATE)
		expect(api.DateFloorDay(new Date(2010, 10, 30, 1, 4, 2))).toStrictEqual(new Date(2010, 10, 30, 0, 0, 0))
	})
})