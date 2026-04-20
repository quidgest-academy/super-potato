export const operators = {
	fnResources: null,
	setResources(fnResources)
	{
		this.fnResources = fnResources
		return this
	},
	get elements()
	{
		const _this = this
		return [
			{
				key: 'EQ',
				resourceId: 'IGUAL_A20898',
				get value() { return _this.fnResources ? _this.fnResources(this.resourceId) : this.resourceId }
			},
			{
				key: 'GT',
				resourceId: 'MAIOR_QUE44697',
				get value() { return _this.fnResources ? _this.fnResources(this.resourceId) : this.resourceId }
			},
			{
				key: 'GET',
				resourceId: 'MAIOR_OU_IGUAL_QUE43517',
				get value() { return _this.fnResources ? _this.fnResources(this.resourceId) : this.resourceId }
			},
			{
				key: 'LT',
				resourceId: 'MENOR_QUE29572',
				get value() { return _this.fnResources ? _this.fnResources(this.resourceId) : this.resourceId }
			},
			{
				key: 'LET',
				resourceId: 'MENOR_OU_IGUAL_QUE23563',
				get value() { return _this.fnResources ? _this.fnResources(this.resourceId) : this.resourceId }
			},
			{
				key: 'NEQ',
				resourceId: 'DIFERENTE_DE49330',
				get value() { return _this.fnResources ? _this.fnResources(this.resourceId) : this.resourceId }
			},
			{
				key: 'LIKE',
				resourceId: 'INCLUI51979',
				get value() { return _this.fnResources ? _this.fnResources(this.resourceId) : this.resourceId }
			},
			{
				key: 'BETWEEN',
				resourceId: 'ENTRE58723',
				get value() { return _this.fnResources ? _this.fnResources(this.resourceId) : this.resourceId }
			},
			{
				key: 'IN',
				resourceId: 'UM_DE14280',
				get value() { return _this.fnResources ? _this.fnResources(this.resourceId) : this.resourceId }
			},
			{
				key: 'ISNULL',
				resourceId: 'E_VAZIO31588',
				get value() { return _this.fnResources ? _this.fnResources(this.resourceId) : this.resourceId }
			},
			{
				key: 'ISNOTNULL',
				resourceId: 'NAO_E_VAZIO29474',
				get value() { return _this.fnResources ? _this.fnResources(this.resourceId) : this.resourceId }
			}
		]
	}
}

export const logical = {
	fnResources: null,
	setResources(fnResources)
	{
		this.fnResources = fnResources
		return this
	},
	get elements()
	{
		const _this = this
		return [
			{
				key: 0,
				resourceId: 'FALSO56819',
				get value() { return _this.fnResources ? _this.fnResources(this.resourceId) : this.resourceId }
			},
			{
				key: 1,
				resourceId: 'VERDADEIRO41682',
				get value() { return _this.fnResources ? _this.fnResources(this.resourceId) : this.resourceId }
			}
		]
	}
}

export const totals = {
	fnResources: null,
	setResources(fnResources)
	{
		this.fnResources = fnResources
		return this
	},
	get elements()
	{
		const _this = this
		return [
			{
				key: 'COUNT',
				resourceId: 'TOTAL_DE_ELEMENTOS27095',
				get value() { return _this.fnResources ? _this.fnResources(this.resourceId) : this.resourceId }
			},
			{
				key: 'MAX',
				resourceId: 'MAXIMO52072',
				get value() { return _this.fnResources ? _this.fnResources(this.resourceId) : this.resourceId }
			},
			{
				key: 'MIN',
				resourceId: 'MINIMO33485',
				get value() { return _this.fnResources ? _this.fnResources(this.resourceId) : this.resourceId }
			},
			{
				key: 'SUM',
				resourceId: 'SOMATORIO37638',
				get value() { return _this.fnResources ? _this.fnResources(this.resourceId) : this.resourceId }
			},
			{
				key: 'AVG',
				resourceId: 'MEDIA55090',
				get value() { return _this.fnResources ? _this.fnResources(this.resourceId) : this.resourceId }
			}
		]
	}
}

export const accessType = {
	fnResources: null,
	setResources(fnResources)
	{
		this.fnResources = fnResources
		return this
	},
	get elements()
	{
		const _this = this
		return [
			{
				key: 'PUB',
				resourceId: 'PUBLICA03827',
				get value() { return _this.fnResources ? _this.fnResources(this.resourceId) : this.resourceId }
			},
			{
				key: 'PES',
				resourceId: 'PESSOAL41600',
				get value() { return _this.fnResources ? _this.fnResources(this.resourceId) : this.resourceId }
			},
			{
				key: 'INA',
				resourceId: 'INACTIVA17630',
				get value() { return _this.fnResources ? _this.fnResources(this.resourceId) : this.resourceId }
			}
		]
	}
}

export default {
	operators,
	logical,
	accessType
}
