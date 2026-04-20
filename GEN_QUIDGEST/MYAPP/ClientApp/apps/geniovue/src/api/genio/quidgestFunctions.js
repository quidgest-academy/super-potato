import { v4 as uuidv4 } from 'uuid'

// Set this variable to true in order to enable logging.
const enableLogCmd = false

//*********************************************
//*********************************************
//*   API para uso em Formulas ou Scripts
//*********************************************
//*********************************************

function qapi()
{
	this.dummy = "Classe de API para agregar funções para formulas sem risco de duplicar nomes";
}

qapi.prototype.LogCmd = function (id, args)
{ //Fazer log da execução dos comandos.  Uso interno.
	let w = id + "(";
	if (args !== null && args !== undefined)
	{
		for (let i = 0; i < args.length; i++)
		{
			if (i > 0) w += ", ";
			if (args[i] === null || args[i] === undefined)
			{
				if (args[i] === null)
				{
					w += "null"
				} else
				{
					w += "undefined"
				}
			} else
			{
				if (typeof args[i] === "object" && args[i].Id !== undefined)
				{
					w += "'" + args[i].Id + "'";
				} else
				{
					w += "'" + args[i] + "'";
				}
			}
		}
	}
	w += ")";
	if (window.App && typeof (window.App.AddCmdLog) === 'function')
	{
		window.App.AddCmdLog(w);
	} else
	{
		// eslint-disable-next-line
		console.log(w);
	}
	return "";
}

qapi.prototype.Today = function ()
{
	const now = new Date();
	return new Date(Date.UTC(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0));
}

qapi.prototype.Now = function ()
{
	const date = new Date();
	return new Date(Date.UTC(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(), date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds()))
}

qapi.prototype.Hoje = function ()
{
	this.LogCmd("Hoje (deprecated)");
	return this.Today();
}

qapi.prototype.Agora = function ()
{
	this.LogCmd("Agora (deprecated)");
	return this.Now();
}

qapi.prototype.RGB = function (red, green, blue) //** retorna o hexadecimal
{
	let hexa = "#"

	hexa += red.toString(16).length < 2 ? "0" + red.toString(16) : red.toString(16)
	hexa += green.toString(16).length < 2 ? "0" + green.toString(16) : green.toString(16)
	hexa += blue.toString(16).length < 2 ? "0" + blue.toString(16) : blue.toString(16)
	return hexa
}

qapi.prototype.iif = function (test, v1, v2) {
	this.LogCmd("iif", arguments);

	//JGF This function used to do eval(test), but that is unsafe, and the implementation was changed
	if (typeof test === "boolean") {
		return test ? v1 : v2;
	}

	if (typeof test === "number") {
		// maintain legacy: only 1 is true
		return (test === 1) ? v1 : v2;
	}

	if (typeof test === "string") {
		const s = test.trim().toLowerCase();
		if (s === "true" || s === "1") return v1;
		if (s === "false" || s === "0" || s === "") return v2;

		this.LogCmd("WARN: qapi.iif - string expressions are no longer supported");
		return v2;
	}

	this.LogCmd("WARN: qapi.iif -  test type not supported");
	return v2; // default: false
};

qapi.prototype.emptyD = function (date)
{ //** retorna 1 se data vazia ou zero no caso contrario
	this.LogCmd("emptyD", arguments);

	// Not defined value
	if (date === "" || date === "undefined" || date === undefined || date === null)
		return 1;
	else
	{
		const minDate = new Date('0001-01-01').getTime(),
			minDateTime = new Date('0001-01-01T00:00:00').getTime();

		if (date instanceof Date)
		{
			const tDate = date.getTime();
			return tDate === minDate || tDate === minDateTime ? 1 : 0;
		}
		else if (typeof date === 'string')
		{
			// Date in ISO format
			if (/(\d{4}-\d{2}-\d{2})[T](\d{2}:\d{2}:\d{2}.?(\d{3})?)[Z]?/.test(date))
			{
				const tDate = Date.parse(date);
				return !tDate ? 1 : (tDate === minDate || tDate === minDateTime) ? 1 : 0;
			}
			// QWeb format  dd/mm/yyyy hh:mm:ss APM | yyyy/mm/dd hh:mm:ss APM
			// /^(0?[1-9]|[12][0-9]|3[01])[\/](0?[1-9]|1[012])[\/\-]\d{4}$/
			else if (/\d{2}\/\d{2}\/\d{4}( \d{2}:\d{2}(:\d{2})?([AP]M)?)?$/.test(date) || /\d{4}\/\d{2}\/\d{2}( \d{2}:\d{2}(:\d{2})?([AP]M)?)?$/.test(date))
				return 0;
			else return 1;
		}
		else return 1;
	}
}

qapi.prototype.emptyC = function (obj)
{ //** retorna 1 se objecto vazio ou zero no caso contrario
	this.LogCmd("emptyC", arguments);
	if (obj === "" || obj === undefined || obj === null)
	{
		return 1;
	} else
	{
		return 0;
	}
}

qapi.prototype.emptyL = function (obj)
{ //** retorna 1 se numerico vazio ou zero no caso contrario
	this.LogCmd("emptyL", arguments);
	if (obj === "" || obj === undefined || obj === null || isNaN(obj) || obj === 0 || obj === false)
	{
		return 1;
	} else
	{
		return 0;
	}
}

qapi.prototype.emptyN = function (obj)
{ //** retorna 1 se numerico vazio ou zero no caso contrario
	this.LogCmd("emptyN", arguments);
	if (obj === "" || obj === undefined || obj === null || isNaN(obj) || obj === 0)
	{
		return 1;
	} else
	{
		return 0;
	}
}

qapi.prototype.emptyG = function (obj)
{ //** retorna 1 se chave interna vazia ou zero no caso contrario
	this.LogCmd("emptyG", arguments);
	if (obj === "" || obj === undefined || obj === null || obj === "00000000-0000-0000-0000-000000000000" || obj === "{00000000-0000-0000-0000-000000000000}" || obj === "0")
	{
		return 1;
	} else
	{
		return 0;
	}
}

qapi.prototype.emptyT = function (obj)
{ //** retorna 1 se objeto vazio ou zero no caso contrario
	this.LogCmd("emptyT", arguments);
	if (obj === "" || obj === undefined || obj === null || obj === "__:__")
	{
		return 1;
	} else
	{
		return 0;
	}
}

qapi.prototype.IsValid = function (data)
{ 
	//** retorna 1 se data valida, caso contrario 0
	this.LogCmd("IsValid (deprecated)", arguments);
	return this.emptyD(data) === 1 ? 0 : 1;
}

qapi.prototype.KeyToString = function (obj)
{ //** retorna string a partir de chave (guid ou interna)
	this.LogCmd("KeyToString", arguments);
	if (this.emptyG(obj) === 1)
		return "";

	let re = /\{/g;
	let res = obj.replace(re, "");
	re = /\}/g;
	res = res.replace(re, "");
	re = /-/g;
	res = res.replace(re, "");
	return res.toUpperCase();
}

qapi.prototype.minD = function (obj1, obj2)
{ //** Função que dadas duas datas devolve a data mínima
	this.LogCmd("minD", arguments);

	if (this.emptyD(obj1))
	{
		obj1 = '';
	}

	if (this.emptyD(obj2))
	{
		obj2 = '';
	}

	const var1 = this.emptyD(obj1) ? '' : Date.parse(obj1);
	const var2 = this.emptyD(obj2) ? '' : Date.parse(obj2);

	return this.min(var1, var2) === var1 ? obj1 : obj2;
}

qapi.prototype.minN = function (obj1, obj2)
{ //** Função que dados dois doubles devolve o mínimo
	this.LogCmd("minN", arguments);

	if (this.emptyN(obj1))
	{
		obj1 = 0;
	}

	if (this.emptyN(obj2))
	{
		obj2 = 0;
	}

	const var1 = parseFloat(obj1);
	const var2 = parseFloat(obj2);
	return this.min(var1, var2);
}

qapi.prototype.maxD = function (obj1, obj2)
{ //** Função que dadas duas datas devolve a data máxima
	this.LogCmd("maxD", arguments);

	if (this.emptyD(obj1))
	{
		obj1 = '';
	}

	if (this.emptyD(obj2))
	{
		obj2 = '';
	}

	const var1 = this.emptyD(obj1) ? '' : Date.parse(obj1);
	const var2 = this.emptyD(obj2) ? '' : Date.parse(obj2);

	return this.max(var1, var2) === var1 ? obj1 : obj2;
}

qapi.prototype.maxN = function (obj1, obj2)
{ //** Função que dados dois doubles devolve o máximo
	this.LogCmd("maxN", arguments);
	if (this.emptyN(obj1))
	{
		obj1 = 0;
	}

	if (this.emptyN(obj2))
	{
		obj2 = 0;
	}

	const var1 = parseFloat(obj1);
	const var2 = parseFloat(obj2);
	return this.max(var1, var2) === var1 ? var1 : var2;
}

qapi.prototype.min = function (obj1, obj2)
{
	return (obj1 < obj2 ? obj1 : obj2);
}

qapi.prototype.max = function (obj1, obj2)
{
	return (obj1 > obj2 ? obj1 : obj2);
}

qapi.prototype.Year = function (data)
{ //** retorna o ano da data
	this.LogCmd("Year", arguments);
	if (this.emptyD(data) === 1)
	{
		return 0;
	}

	if (typeof (data) === "string" && this.emptyD(data) === 0)
	{
		data = new Date(data);
	}

	return data.getFullYear();
}

qapi.prototype.Month = function (data)
{ //** retorna o mes da data
	this.LogCmd("Month", arguments);
	if (this.emptyD(data) === 1)
	{
		return 0;
	}

	if (typeof (data) === "string" && this.emptyD(data) === 0)
	{
		data = new Date(data);
	}

	return data.getMonth() + 1;
}

qapi.prototype.Day = function (data)
{ //** retorna o dia da data
	this.LogCmd("Day", arguments);
	if (this.emptyD(data) === 1)
	{
		return 0;
	}

	if (typeof (data) === "string" && this.emptyD(data) === 0)
	{
		data = new Date(data);
	}

	return data.getDate();
}

qapi.prototype.GetCurrentYear = function ()
{
	this.LogCmd("GetCurrentYear (deprecated)", arguments);
	return this.Year(this.Today());
}

qapi.prototype.GetCurrentMonth = function ()
{
	this.LogCmd("GetCurrentMonth (deprecated)", arguments);
	return this.Month(this.Today());
}

qapi.prototype.GetCurrentDay = function ()
{
	this.LogCmd("GetCurrentDay (deprecated)", arguments);
	return this.Today();
}

/**
 * Select a number of characters from a string starting from the left.
 */
qapi.prototype.LEFT = function (arg, nrElem)
{
	this.LogCmd("LEFT", arguments);
	if (arg === undefined || arg === null)
	{
		return "";
	}
	if (nrElem === null || nrElem === undefined || nrElem === "" || nrElem < 0)
	{
		return "";
	}
	if (nrElem > arg.length)
	{
		return arg;
	}
	return arg.substring(0, nrElem);
}

/**
 * Select a number of characters from a string starting from the right.
 */
qapi.prototype.RIGHT = function (arg, nrElem)
{
	this.LogCmd("RIGHT", arguments);
	if (arg === undefined || arg === null)
	{
		return "";
	}
	if (nrElem === null || nrElem === undefined || nrElem === "" || nrElem < 0)
	{
		return "";
	}
	if (nrElem > arg.length)
	{
		return arg;
	}
	return arg.substring(arg.length - nrElem);
}

qapi.prototype.SubString = function (arg, inicio, nrElem)
{ //** Função que dada uma string permite obter o nº de elementos a contar de uma posição
	this.LogCmd("SubString", arguments);
	if (arg === undefined || arg === null)
	{
		return "";
	}
	if (inicio === null || inicio === undefined || inicio === "" || inicio < 0 || inicio > arg.length)
	{
		return "";
	}
	if (nrElem === null || nrElem === undefined || nrElem === "" || nrElem < 0)
	{
		return "";
	}
	if (nrElem > arg.length - inicio)
	{
		nrElem = arg.length - inicio;
	}
	return arg.substring(inicio, inicio + nrElem);
}

qapi.prototype.IndexOf = function (str, substr)
{
	if (!str || !substr)
		return -1;
	return str.indexOf(substr);
}

qapi.prototype.LTRIM = function (obj)
{ //** retorna string sem os espaços à esquerda
	this.LogCmd("LTRIM", arguments);
	if (this.emptyC(obj) === 1)
	{
		return "";
	}
	return obj.replace(/^\s+/, "");
}

qapi.prototype.RTRIM = function (obj)
{ //** retorna string sem os espaços à direita
	this.LogCmd("RTRIM", arguments);
	if (this.emptyC(obj) === 1)
	{
		return "";
	}
	return obj.replace(/\s+$/, "");
}
qapi.prototype.atoi = function (obj)
{ //** retorna inteiro correspondente ao objecto
	this.LogCmd("atoi", arguments);
	if (this.emptyC(obj) === 1 || !isFinite(obj))
	{
		return 0;
	}

	return parseInt(obj);
}

qapi.prototype.abs = function (num)
{
	this.LogCmd("abs", arguments);
	return Math.abs(num);
}

qapi.prototype.Round = function (val, dec)
{ //** retorna valor arredondado com numero de casas decimais
	this.LogCmd("Round", arguments);
	if (this.emptyN(val) === 1)
	{
		return 0;
	}
	if (this.emptyN(dec) || dec <= 0)
	{
		return Math.round(val);
	}
	let mut = 1;
	if (val < 0)
	{
		val = Math.abs(val)
		mut = -1;
	}
	const ret = Math.round(val * Math.pow(10, dec)) / Math.pow(10, dec);

	return ret * mut;

}

qapi.prototype.Sqrt = function (val)
{ //** retorna a raiz quadrada
	this.LogCmd("Sqrt", arguments);
	return Math.sqrt(val);
}

qapi.prototype.IntToString = function (a)
{ //** Método que permite converter um inteiro para string
	this.LogCmd("IntToString", arguments);

	if (this.emptyN(a) === 1)
	{
		return "0";
	}

	if (!(a === parseInt(a, 10)))
	{
		return "0"; // "The argument provided as number isn't an integer."
	}

	return a.toString();
}

qapi.prototype.NumericToString = function (valor, decimais)
{ //** retorna string com o valor convertido com as decimais e arredondamente se necessario
	this.LogCmd("NumericToString", arguments);

	let val = this.Round(valor, decimais).toString();
	if (val.indexOf("."))
	{
		val = val.replace(".", ",");
	}
	return val;
}

qapi.prototype.HorasToDouble = function (time)
{ //** retorna horas com minutos em decimais partindo de string de horas  __:__
	this.LogCmd("HorasToDouble", arguments);

	if (this.emptyT(time) || time.match(/[0-9_]:[0-9_]{2}/g) === null)
	{
		return 0.0;
	}

	let hour = 0,
		minute = 0;

	if (this.emptyC(time) === 0 && time.length === 5)
	{
		time = time.replace(/_/g, '0');

		hour = parseInt(time.substr(0, 2));
		minute = parseInt(time.substr(3, 2));

		if (hour < 0 || hour > 23 || minute < 0 || minute > 59)
		{
			return 0.0
		}
	}
	else return 0.0;

	return (hour * 60.0 + minute) / 60.0;
}

qapi.prototype.DoubleToHoras = function (time)
{ //** retorna string __:__ a partir de hora com minutos em decimais
	this.LogCmd("DoubleToHoras", arguments);
	if (this.emptyN(time) === 1 || time < 0)
	{
		return "00:00";
	}

	const formatOption = {
		minimumIntegerDigits: 2,
		maximumFractionDigits: 0
	};

	const totalMinutes = Math.round(time * 60.0);
	const hours = Math.floor(totalMinutes / 60);
	const minutes = totalMinutes % 60;

	return (hours).toLocaleString(undefined, formatOption) + ":" + (minutes).toLocaleString(undefined, formatOption)
}

qapi.prototype.HorasAdd = function (time, minutes)
{ //** Adiciona minutos à representação de tempo __:__
	this.LogCmd("HorasAdd", arguments);

	if (this.emptyC(time) === 1 || time.length !== 5)
	{
		return "__:__";
	}

	if (this.emptyN(minutes))
	{
		minutes = 0;
	}

	time = time.replace(/_/g, "0");

	const h0 = parseInt(time.substr(0, 2)),
		m0 = parseInt(time.substr(3, 2)),
		formatOption = {
			minimumIntegerDigits: 2,
			maximumFractionDigits: 0
		};

	if (h0 < 0 || h0 > 23 || m0 < 0 || m0 > 59)
		return "__:__";

	let resInt = h0 * 60 + m0 + Math.floor(minutes);
	if (resInt < 0) resInt = 0;
	if (resInt > 23 * 60 + 59) resInt = 23 * 60 + 59;

	return (Math.floor(resInt / 60)).toLocaleString(undefined, formatOption) + ':' + (resInt % 60).toLocaleString(undefined, formatOption);
}

qapi.prototype.ValidateDateTime = function (year, month, day, hour, minute, second)
{
	this.LogCmd("ValidateDateTime", arguments);
	if (year === null || year === undefined || year === "" || year < 0 || year.toString().length > 4)
	{
		return false; // "The argument provided as year is invalid."
	}
	if (month === null || month === undefined || month === "" || month < 1 || month > 12 || month.toString().length > 2)
	{
		return false; // "The argument provided as month is invalid."
	}
	if (day === null || day === undefined || day === "" || day < 1 || day > 31 || day.toString().length > 2)
	{
		return false; // "The argument provided as day is invalid."
	}
	if (hour === null || hour === undefined || hour === "" || hour < 0 || hour > 24 || hour.toString.length > 2)
	{
		return false; // "The argument provided as hour is invalid."
	}
	if (minute === null || minute === undefined || minute === "" || minute < 0 || minute > 59 || minute.toString().length > 2)
	{
		return false; // "The argument provided as minute is invalid."
	}
	if (second === null || second === undefined || second === "" || second < 0 || second > 59 || second.toString().lentgh > 2)
	{
		return false; // "The argument provided as second is invalid."
	}

	const month30days = [4, 6, 9, 11];

	//if the given day is 31 checks if can be created a date with the given month. If the given month only has 30 days return false (empty value)
	if (day === 31)
	{
		for (let i = 0; i < month30days.length; i++)
		{
			if (month === month30days[i])
			{
				return false; // "The argument provided as day is invalid for the given month.";
			}
		}
	}
	if (month === 2 && day > 29)
	{
		return false; // "The argument provided as day is invalid for the given month.";
	}
	//if the day is 29 and the month is february it has to be a leap year, if not, return false (empty value)
	if (month === 2 && day === 29 && !(((year % 4 === 0) && (year % 100 !== 0)) || (year % 400 === 0)))
	{
		return false; // "The argument provided as day is invalid as the month is february but the year isn't a leap year";
	}

	return true;
}

qapi.prototype.CreateDateTime = function (year, month, day, hour, minute, second)
{
	this.LogCmd("CreateDateTime", arguments);
	if (this.ValidateDateTime(year, month, day, hour, minute, second))
		return new Date(Date.UTC(year, month - 1, day, hour, minute, second));
	else
		return '';
}

qapi.prototype.DateSetTime = function (date, time)
{
	if (this.emptyD(date) === 1)
	{
		return '';
	}

	let hour = 0,
		minute = 0;

	if (this.emptyT(time) === 0 && time.length === 5)
	{
		time = time.replace(/_/g, '0');

		hour = parseInt(time.substr(0, 2));
		minute = parseInt(time.substr(3, 2));

		if (hour < 0 || hour > 23 || minute < 0 || minute > 59)
		{
			hour = 0;
			minute = 0;
		}
	}

	return this.CreateDateTime(this.Year(date), this.Month(date), this.Day(date), hour, minute, 0);
}

qapi.prototype.CriaData = function (ano, mes, dia, hora, minuto, segundo)
{
	this.LogCmd("CriaData (deprecated)", arguments);
	return this.CreateDateTime(ano, mes, dia, hora, minuto, segundo);
}

qapi.prototype.CriaDataHora = function (data, hora)
{
	this.LogCmd("CriaDataHora (deprecated)", arguments);
	return this.DateSetTime(data, hora);
}

qapi.prototype.DateCompare = function (date1, date2)
{
	this.LogCmd("DateCompare", arguments);

	if (typeof date1 === "string")
	{
		date1 = new Date(date1);
	}
	if (typeof date2 === "string")
	{
		date2 = new Date(date2);
	}

	if (this.emptyD(date1) === 1 && this.emptyD(date2) === 1)
		return 0; // both invalid
	else if (this.emptyD(date1) === 1)
		return -1; // date2 is later
	else if (this.emptyD(date2) === 1)
		return 1; // date1 is later

	if (date1.getTime() > date2.getTime())
		return 1;
	else if (date1.getTime() < date2.getTime())
		return -1;
	return 0;
}

qapi.prototype.CreateDuration = function (days, hours, minutes, seconds)
{
	return {
		getDays: function ()
		{
			return days;
		},
		getHours: function ()
		{
			return hours;
		},
		getMinutes: function ()
		{
			return minutes;
		},
		getSeconds: function ()
		{
			return seconds;
		}
	}
}

qapi.prototype.DateDiff = function (startDate, endDate)
{
	const diff = endDate - startDate;
	return Math.floor(diff / 1000);
}

qapi.prototype.DateDiffPart = function (startDate, endDate, unit)
{
	const diff = endDate - startDate;
	if (unit === "D")
		return Math.floor(diff / 1000 / 3600 / 24);
	if (unit === "H")
		return Math.floor(diff / 1000 / 3600);
	if (unit === "M")
		return Math.floor(diff / 1000 / 60);
	if (unit === "S")
		return Math.floor(diff / 1000);

	return 0;
}

qapi.prototype.DateAddDuration = function (date, duration)
{
	if (!(date instanceof Date))
		return '';
	const newDate = new Date(date.getTime());
	newDate.setUTCSeconds(newDate.getUTCSeconds() + duration);
	return newDate;
}

qapi.prototype.DateSubtractDuration = function (date, duration)
{
	if (!(date instanceof Date))
		return '';
	const newDate = new Date(date.getTime());
	newDate.setUTCSeconds(newDate.getUTCSeconds() - duration);
	return newDate;
}

qapi.prototype.DateAddYears = function (date, duration)
{
	if (!(date instanceof Date))
		return '';
	const newDate = new Date(date.getTime());
	newDate.setUTCFullYear(newDate.getUTCFullYear() + duration);
	return newDate;
}

qapi.prototype.DateAddMonths = function (date, duration)
{
	if (!(date instanceof Date))
		return '';
	const newDate = new Date(date.getTime());
	newDate.setUTCMonth(newDate.getUTCMonth() + duration);
	return newDate;
}

qapi.prototype.DateAddDays = function (date, duration)
{
	if (!(date instanceof Date))
		return '';
	const newDate = new Date(date.getTime());
	newDate.setUTCDate(newDate.getUTCDate() + duration);
	return newDate;
}

qapi.prototype.DateAddHours = function (date, duration)
{
	if (!(date instanceof Date))
		return '';
	const newDate = new Date(date.getTime());
	newDate.setUTCHours(newDate.getUTCHours() + duration);
	return newDate;
}

qapi.prototype.DateAddMinutes = function (date, duration)
{
	if (!(date instanceof Date))
		return '';
	const newDate = new Date(date.getTime());
	newDate.setUTCMinutes(newDate.getUTCMinutes() + duration);
	return newDate;
}

qapi.prototype.DateAddSeconds = function (date, duration)
{
	if (!(date instanceof Date))
		return '';
	const newDate = new Date(date.getTime());
	newDate.setUTCSeconds(newDate.getUTCSeconds() + duration);
	return newDate;
}

qapi.prototype.DateGetYear = function (date)
{
	if (!(date instanceof Date))
		return 0;
	return date.getUTCFullYear();
}

qapi.prototype.DateGetMonth = function (date)
{
	if (!(date instanceof Date))
		return 0;
	return date.getUTCMonth() + 1;
}

qapi.prototype.DateGetDay = function (date)
{
	if (!(date instanceof Date))
		return 0;
	return date.getUTCDate();
}

qapi.prototype.DateGetHour = function (date)
{
	if (!(date instanceof Date))
		return 0;
	return date.getUTCHours();
}

qapi.prototype.DateGetMinute = function (date)
{
	if (!(date instanceof Date))
		return 0;
	return date.getUTCMinutes();
}

qapi.prototype.DateGetSecond = function (date)
{
	if (!(date instanceof Date))
		return 0;
	return date.getUTCSeconds();
}

qapi.prototype.DurationTotalDays = function (duration)
{
	return duration / (24 * 3600.0);
}

qapi.prototype.DurationTotalHours = function (duration)
{
	return duration / 3600.0;
}

qapi.prototype.DurationTotalMinutes = function (duration)
{
	return duration / 60.0;
}

qapi.prototype.DurationTotalSeconds = function (duration)
{
	return duration;
}

qapi.prototype.ComparaDatas = function (data1, data2)
{ //** retorna 0 se iguais, >0 se a primeira é maior e <0 se a segunda é maior
	this.LogCmd("ComparaDatas", arguments);

	if (typeof data1 === "string")
	{
		data1 = new Date(data1);
	}
	if (typeof data2 === "string")
	{
		data2 = new Date(data2);
	}

	//Both dates are invalid ...
	if (this.emptyD(data1) === 1 && this.emptyD(data2) === 1)
		return 0;
	else if (this.emptyD(data1) === 1)
		return -1; //The second is biggest
	else if (this.emptyD(data2) === 1)
		return 1; //The first is biggest

	const res = (data1.getTime() === data2.getTime()) ? 0 : ((data1.getTime() > data2.getTime()) ? 1 : -1);

	return res;
}


qapi.prototype.LengthString = function (str)
{ //** retorna comprimento da string
	this.LogCmd("LengthString", arguments);
	if (str === null || str === undefined || typeof str !== "string")
	{
		return 0;
	} else
	{
		return str.length;
	}
}

qapi.prototype.RoundQG = function (val, prec)
{ //** retorna o arredondamento com x casas decimais
	this.LogCmd("RoundQG", arguments);
	if (val === null || val === undefined)
	{
		return 0;
	}
	if (prec === null || prec === undefined || prec < 0)
	{
		prec = 0;
	}
	const sign = val >= 0 ? 1 : -1;
	const folga = 0.001 * Math.pow(0.1, prec) * sign;
	return this.Round(val + folga, prec);
}

qapi.prototype.ValorIVA = function (incidenc, taxa_iva, preciva, prec)
{ //** retorna o valor do IVA
	this.LogCmd("ValorIVA", arguments);
	/// <summary>
	/// Função que calcula o valor do IVA
	/// A incidencia pode entrar com iva ou sem iva sendo discriminada pelo parametro preciva
	/// </summary>
	/// <param name="incidenc">O valor com iva ou sem iva</param>
	/// <param name="taxa_iva">taxa de IVA</param>
	/// <param name="preciva">1 caso o incidenc seja o preço com iva, 0 caso seja o preço sem iva</param>
	/// <param name="prec">precisão</param>
	/// <returns>valor do IVA</returns>
	if (incidenc === null || incidenc === undefined || incidenc === "")
	{
		return 0; // "The argument provided as incidenc is invalid.";
	}
	if (taxa_iva === null || taxa_iva === undefined || taxa_iva === "")
	{
		return 0; // "The argument provided as taxa_iva is invalid.";
	}
	if (preciva === null || preciva === undefined || preciva === "")
	{
		return 0; // "The argument provided as preciva is invalid.";
	}
	if (prec === null || prec === undefined || prec === "" || prec < 0)
	{
		return 0; // "The argument provided as prec is invalid.";
	}
	if (!(preciva === 0 || preciva === 1))
	{
		return 0; // "The argument provided as preciva is neither 0 nor 1."
	}
	return this.RoundQG(preciva === 1 ? incidenc / (1.0 + taxa_iva / 100.0) * (taxa_iva / 100.0) : incidenc * (taxa_iva / 100.0), prec);
}

qapi.prototype.Incidenc = function (valoruni, quantida, pdescont, prec)
{
	this.LogCmd("Incidenc", arguments);
	/// <summary>
	/// Funcao que calcula o valor da Incidencia
	/// </summary>
	/// <param name="valoruni">valor unitário</param>
	/// <param name="quantida">quantidade</param>
	/// <param name="pdescont">percentagem de desconto</param>
	/// <param name="prec">casas decimais de precisão</param>
	/// <returns>o valor da incidencia</returns>

	if (valoruni === null || valoruni === undefined || valoruni === "" || valoruni < 0)
	{
		return 0; // "The argument provided as valoruni is invalid.";
	}
	if (quantida === null || quantida === undefined || quantida === "" || quantida < 0)
	{
		return 0; // "The argument provided as quantida is invalid.";
	}
	if (pdescont === null || pdescont === undefined || pdescont === "")
	{
		return 0; // "The argument provided as pdescont is invalid.";
	}
	if (prec === null || prec === undefined || prec === "" || prec < 0)
	{
		return 0; // "The argument provided as prec is invalid.";
	}
	const valorart = this.RoundQG(valoruni * quantida, prec);
	return valorart - this.RoundQG(pdescont / 100.0 * valorart, prec);
}

qapi.prototype.strAno = function (data)
{ //** retorna string com o ano da data
	this.LogCmd("strAno", arguments);
	return this.Year(data).toString(); //não necessita de validações porque o método Year já as tem.
}

qapi.prototype.SomaDias = function (data, dias)
{ //** retorna Date com a soma dos dias
	this.LogCmd("SomaDias", arguments);
	if (typeof data === "string" && this.emptyD(data) === 0)
	{
		data = new Date(data); // converter string para data
	}
	if (this.emptyD(data) === 1)
	{
		return '';
	}
	if (dias === null || dias === undefined || isNaN(dias))
	{
		return data;
	}
	return new Date(data.getFullYear(), data.getMonth(), data.getDate() + dias, data.getHours(), data.getMinutes(), data.getSeconds());
}

qapi.prototype.Floor = function (val)
{ //** retorna o metodo Floor
	this.LogCmd("Floor", arguments);
	if (val === null || val === undefined)
		return 0;
	return Math.floor(val);
}

qapi.prototype.Diferenca_entre_Datas = function (dt_inicio, dt_fim, escala)
{
	this.LogCmd("Diferenca_entre_Datas", arguments);
	/// <summary>
	/// Permite calcular a diferença entre duas datas (DateTime).
	/// Retorna um valor (double) da diferença entre as duas datas.
	/// </summary>
	/// <param name="dt_inicio">data de início</param>
	/// <param name="dt_fim">data de fim</param>
	/// <param name="escala">em D(Dias), H(Horas), M(Minutos) ou S(Segundos)</param>
	/// <returns>a diferença entre as datas na escala escolhida</returns>
	if (this.emptyD(dt_inicio) === 1 || this.emptyD(dt_fim) === 1 || this.emptyC(escala))
	{
		return 0;
	}

	// converter string para data
	if (typeof dt_inicio === "string")
	{
		dt_inicio = new Date(dt_inicio);
	}
	if (typeof dt_fim === "string")
	{
		dt_fim = new Date(dt_fim);
	}
	let dif = dt_fim.getTime() - dt_inicio.getTime();
	dif = dif / 1000;

	switch (escala.toString().toUpperCase())
	{
		case "D":
			return Math.floor(dif / 60 / 60 / 24);
		case "H":
			return Math.floor(dif / 60 / 60);
		case "M":
			return Math.floor(dif / 60);
		case "S":
			return Math.floor(dif);
	}
}

qapi.prototype.DateFloorDay = function (date)
{
	this.LogCmd("DateFloorDay", arguments);
	/// <summary>
	/// Truncates the time part of a datetime value.
	/// </summary>
	/// <param name="date">The source date</param>
	/// <returns>A modified date with only the date part of the original datetime</returns>
	return this.CriaData(this.Year(date), this.Month(date), this.Day(date), 0, 0, 0);
}

/**
 * Checks if the specified key is a GUID.
 * @param {string} key The key to check
 * @returns 1 if the key is a GUID or is empty, 0 otherwise.
 */
qapi.prototype.isGuid = function (key)
{
	const isEmptyGuid = this.emptyG(key);
	if (isEmptyGuid === 1)
		return isEmptyGuid;

	const regex = /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/;
	if (regex.test(key))
		return 1;
	return 0;
}
/*********** Support data structures ***********/

const MYAPP_THEME_VARIABLES = {
	'$footer-bg': "transparent",
	'$menu-sidebar-width': "16rem",
	'$menu-behaviour': "partial_collapse",
	'$compactheader': "false",
	'$save-icon': "floppy-disk",
	'$compactstyle': "true",
	'$border-radius': "0.25rem",
	'$table-striped': "false",
	'$table-head-inverse': "false",
	'$table-vertical-border': "true",
	'$enable-table-wrap': "true",
	'$font-size-base': "0.7rem",
	'$font-family-sans-serif': "\"Lato\", Roboto, \"Helvetica Neue\", Arial, sans-serif, \"Apple Color Emoji\", \"Segoe UI Emoji\", \"Segoe UI Symbol\", \"Noto Color Emoji\"",
	'$font-headings': "$font-family-sans-serif",
	'$headings-text-transform': "uppercase",
	'$primary': "#D69E98",
	'$secondary': "#C9B793",
	'$highlight': "#ff8241",
	'$action-focus-width': "2px",
	'$action-focus-style': "solid",
	'$action-focus-color': "#201060",
	'$input-focus-color': "rgba(0, 169, 206, 0.35)",
	'$button-focus-color': "rgba(238, 96, 2, 0.5)",
	'$body-bg': "$white",
	'$body-color': "#202428",
	'$input-btn-padding-y': "0.26rem",
	'$input-btn-padding-x': "0.25rem",
	'$enable-switch-single-label': "false",
	'$wizard-steps': "circle",
	'$wizard-content': "standard",
	'$btn-align-right': "false",
	'$menu-multi-level': "true",
	'$primary-light': "#F3904A",
	'$primary-dark': "#A34C29",
	'$success': "#28a745",
	'$danger': "#b71c1c",
	'$light': "#EAEBEC",
	'$red': "#b71c1c",
	'$info': "#17a2b8",
	'$warning': "#ffa900",
	'$gray': "#7C858D",
	'$gray-light': "#C4C5CA",
	'$gray-dark': "#40474F",
	'$navbar-font-size': "0.8rem",
	'$navbar-font-weight': "400",
	'$tab-style': "line",
	'$group-border-top': "none",
	'$group-border-bottom': "none",
	'$input-bg': "transparent",
	'$input-bg-readonly': "rgb($neutral-light-rgb / 0.25)",
	'$hover-item': "rgb($primary-light-rgb / 0.5)",
	'$header-bg': "$background",
	'$header-color': "$on-background",
	'$navbar-bg': "$primary",
	'$navbar-color': "$on-primary",
	'$menu-multi-level-border': "false"
};

/**
 * Converts the given Genio language id to it's correspondent platform language id.
 * @param {string} languageId The language id to convert
 * @returns A string with the language id, or null if the specified id doesn't exist.
 */
qapi.prototype.GetClientLang = function(languageId)
{
	switch (languageId)
	{
		case 'eng':
			return 'en-US';
	}

	return null;
}

/**
 * Returns the theme variable for the current app
 * @param {string} variable The theme variable name
 * @returns The theme variable value
 */
qapi.prototype.GetAppThemeVariable = function(variable)
{
	return MYAPP_THEME_VARIABLES[variable];
}

/**
 * Returns the theme variable value for a specific app
 * @param {string} appId The id of the application we want the variable of
 * @param {string} variable The theme variable name
 * @returns The theme variable value
 */
qapi.prototype.GetThemeVariable = function(appId, variable)
{
	if ('MYAPP' === appId)
		return MYAPP_THEME_VARIABLES[variable];
	return '';
}

/**
 * Override the quidgest.functions.js to work with Vue.js.
 */
export class VuejsQApi extends qapi
{
	constructor()
	{
		super()
		this.dummy = 'The API with all Quidgest standard functions for the Vue.js projects'
	}

	/**
	 * We don't want to have console.log.
	 * It's just making it difficult to read the unit tests
	 * (when some functions are used... which is the case with QTable).
	 * Later, a log util suitable for Vue.js will be created.
	 */
	LogCmd(id, args)
	{
		if (enableLogCmd)
			super.LogCmd(id, args)
	}

	/**
	 * Creates a new GUID.
	 * @returns The created GUID.
	 */
	GUIDCreate()
	{
		this.LogCmd('GUIDCreate')

		return uuidv4().toString()
	}
}

const qApiInstance = new VuejsQApi()

export default qApiInstance
