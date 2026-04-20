class ColumnResizeable
{
	constructor(table, options, extraElem, containerElem, wrapperElem)
	{
		//Element for table
		//If parameter is an array, use first element
		if (Array.isArray(table))
			this.table = table[0]
		else
			this.table = table

		//Options
		this.options = undefined
		if (options !== undefined && options !== null)
			this.options = options

		//Element to change width to match table width
		this.extraElem = undefined
		if (extraElem !== undefined && extraElem !== null)
		{
			//If parameter is an array, use first element
			if (Array.isArray(extraElem))
				this.extraElem = extraElem[0]
			else
				this.extraElem = extraElem
		}

		//Element to change width to match table width
		this.containerElem = undefined
		if (containerElem !== undefined && containerElem !== null)
		{
			//If parameter is an array, use first element
			if (Array.isArray(containerElem))
				this.containerElem = containerElem[0]
			else
				this.containerElem = containerElem
		}

		//Element with maximum width to allow for extraElem and containerElem
		this.wrapperElem = undefined
		if (wrapperElem !== undefined && wrapperElem !== null)
		{
			//If parameter is an array, use first element
			if (Array.isArray(wrapperElem))
				this.wrapperElem = wrapperElem[0]
			else
				this.wrapperElem = wrapperElem
		}

		this.pageX = undefined
		this.curCol = undefined
		this.nxtCol = undefined
		this.curColWidth = undefined
		this.nxtColWidth = undefined

		//FOR: Allowing table width to adjust to fit resized columns
		this.tableWidth = undefined
		this.curColWidthPrev = undefined
		this.curColWidthNext = undefined

		this.headerHeight = undefined

		this.cols = undefined

		this.divEventListeners = []
		this.documentEventListeners = []
	}

	destroy()
	{
		// Remove events from div's
		try
		{
			for (let dIdx = 0; dIdx < this.divEventListeners.length; dIdx++)
			{
				for (let eIdx = 0; eIdx < this.divEventListeners[dIdx].eListeners.length; eIdx++)
				{
					this.divEventListeners[dIdx].element.removeEventListener(
						this.divEventListeners[dIdx].eListeners[eIdx].event, this.divEventListeners[dIdx].eListeners[eIdx].fn)
				}
				this.divEventListeners[dIdx].element.remove()
				this.divEventListeners[dIdx].eListeners.splice(0)
			}
		}
		finally
		{
			this.divEventListeners.splice(0)
		}

		// Remove events from document
		try
		{
			for (let eIdx = 0; eIdx < this.documentEventListeners.length; eIdx++)
				document.removeEventListener(this.documentEventListeners[eIdx].event, this.documentEventListeners[eIdx].fn)
		}
		finally
		{
			this.documentEventListeners.splice(0)
		}
	}

	init()
	{
		const row = this.table.getElementsByTagName('tr')[0]
		this.cols = row ? row.children : undefined
		if (!this.cols)
			return

		this.headerHeight = row.offsetHeight

		for (let i = 0; i < this.cols.length; i++)
		{
			const div = this.createDiv(this.headerHeight)
			this.cols[i].appendChild(div)
			this.setListeners(div)
		}
	}

	createDiv()
	{
		const div = document.createElement('div')
		div.style.top = 0
		div.style.right = 0
		div.style.width = '5px'
		div.style.position = 'absolute'
		div.style.cursor = 'col-resize'
		div.style.userSelect = 'none'
		div.style.height = '100%'
		return div
	}

	_mousedownListener(e)
	{
		this.curCol = e.target.parentElement
		this.nxtCol = this.curCol.nextElementSibling
		this.pageX = e.pageX

		//FOR: Allowing table width to adjust to fit resized columns
		//Store table width in pixels
		this.tableWidth = this.table.offsetWidth

		const padding = this.paddingDiff(this.curCol)

		this.curColWidth = this.curCol.offsetWidth - padding
		//FOR: Allowing table width to adjust to fit resized columns
		//Store column width before resizing
		this.curColWidthPrev = this.curColWidth - 1

		// Add mouse move and up listeners only when clicking on the column header
		// so they are not running when the functions they call are not needed
		const onmousemove = {
			event: 'mousemove',
			fn: this._mousemoveListener.bind(this)
		}
		document.addEventListener(onmousemove.event, onmousemove.fn)
		this.documentEventListeners.push(onmousemove)

		const onmouseup = {
			event: 'mouseup',
			fn: this._mouseupListener.bind(this)
		}
		document.addEventListener(onmouseup.event, onmouseup.fn)
		this.documentEventListeners.push(onmouseup)
	}

	_mouseoverListener(e)
	{
		const borderWidthTop = this.getStyleVal(this.table, 'border-top-width')
		const borderWidthBottom = this.getStyleVal(this.table, 'border-bottom-width')
		e.target.style.height = parseFloat(this.table.clientHeight) - parseFloat(borderWidthTop) - parseFloat(borderWidthBottom) + 'px'
		e.target.style.borderRight = '2px solid #0000ff'
	}

	_mouseoutListener(e)
	{
		e.target.style.height = '100%'
		e.target.style.borderRight = ''
	}

	_mousemoveListener(e)
	{
		if (this.curCol)
		{
			const diffX = e.pageX - this.pageX

			this.curCol.style.width = (this.curColWidth + diffX) + 'px'

			//FOR: Allowing table width to adjust to fit resized columns
			//Prevent table width from getting smaller if the current column can't get any smaller
			//Get new column width
			const padding = this.paddingDiff(this.curCol)
			this.curColWidthNext = this.curCol.offsetWidth - padding
			//If column width changed, change table width to fit
			if (parseInt(this.curColWidthNext) !== parseInt(this.curColWidthPrev))
			{
				this.table.style.width = (this.tableWidth + diffX) + 'px'
				//Change width of extra element and container element to match table width
				//but do not allow them to be wider than the wrapper element
				if (this.wrapperElem !== undefined && this.containerElem !== undefined)
				{
					if (this.table.offsetWidth <= this.wrapperElem.offsetWidth)
					{
						this.containerElem.style.width = (this.tableWidth + diffX) + 'px'
						if (this.extraElem !== undefined)
							this.extraElem.style.width = (this.tableWidth + diffX) + 'px'
						//If table must be at least full width
						if (this.options.minFullWidth)
						{
							this.table.style.minWidth = '100%'
							this.containerElem.style.minWidth = '100%'
							if (this.extraElem !== undefined)
								this.extraElem.style.minWidth = '100%'
						}
					}
				}
				else if (this.containerElem !== undefined)
				{
					this.containerElem.style.width = (this.tableWidth + diffX) + 'px'
					if (this.extraElem !== undefined)
						this.extraElem.style.width = (this.tableWidth + diffX) + 'px'

					//If table must be at least full width
					if (this.options.minFullWidth)
					{
						this.table.style.minWidth = '100%'
						this.containerElem.style.minWidth = '100%'
						if (this.extraElem !== undefined)
							this.extraElem.style.minWidth = '100%'
					}
				}
			}
			//Store width of this column to compare on next mousemove event
			this.curColWidthPrev = this.curColWidthNext
		}
	}

	_mouseupListener()
	{
		this.curCol = undefined
		this.nxtCol = undefined
		this.pageX = undefined
		this.nxtColWidth = undefined
		this.curColWidth = undefined

		this.tableWidth = undefined
		this.curColWidthPrev = undefined
		this.curColWidthNext = undefined

		// Remove events from document
		try
		{
			for (let eIdx = 0; eIdx < this.documentEventListeners.length; eIdx++)
				document.removeEventListener(this.documentEventListeners[eIdx].event, this.documentEventListeners[eIdx].fn)
		}
		finally
		{
			this.documentEventListeners.splice(0)
		}
	}

	setListeners(div)
	{
		const objListeners = {
			element: div,
			eListeners: []
		}

		const onmousedown = {
			event: 'mousedown',
			fn: this._mousedownListener.bind(this)
		}
		div.addEventListener(onmousedown.event, onmousedown.fn)
		objListeners.eListeners.push(onmousedown)

		const onmouseover = {
			event: 'mouseover',
			fn: this._mouseoverListener.bind(this)
		}
		div.addEventListener(onmouseover.event, onmouseover.fn)
		objListeners.eListeners.push(onmouseover)

		const onmouseout = {
			event: 'mouseout',
			fn: this._mouseoutListener.bind(this)
		}
		div.addEventListener(onmouseout.event, onmouseout.fn)
		objListeners.eListeners.push(onmouseout)

		this.divEventListeners.push(objListeners)
	}

	getStyleVal(elm, css)
	{
		return (window.getComputedStyle(elm, null).getPropertyValue(css))
	}

	paddingDiff(col)
	{
		if (this.getStyleVal(col, 'box-sizing') === 'border-box')
			return 0

		const padLeft = this.getStyleVal(col, 'padding-left')
		const padRight = this.getStyleVal(col, 'padding-right')
		return (parseInt(padLeft) + parseInt(padRight))
	}
}

export default ColumnResizeable
