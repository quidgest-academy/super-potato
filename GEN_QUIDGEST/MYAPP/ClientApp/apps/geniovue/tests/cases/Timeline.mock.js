export default {
	simpleUsage()
	{
		return {
			yearlyTimeline: {
				// Timeline Item-1 (Row-1)
				tipoTimeline: 'S',
				timeLineData: {
					rows: [
						{
							Data: '2019-04-11 00:03:44',
							Texto: '2',
							Icon: 'glyphicons-train',
							ImagesColumns: [
								{
									Order: 1,
									Titulo: '',
									Icone: '',
									Valor: '',
									Image: `data:image/png;base64, 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`,
									Url: 'https://quidgest.net/GQT/en-US/0/GQT/Equip/ImageHandlerGet/f8f4cd3d-3bed-4ff2-b481-d6dffddb7179?modelname=Equip&fldname=ValFotograf&formIdentifier=FREPAR'
								},
								{
									Order: 2,
									Titulo: '',
									Icone: '',
									Valor: '',
									Image: `data:image/png;base64, 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`,
									Url: 'https://quidgest.net/GQT/en-US/0/GQT/Equip/ImageHandlerGet/f8f4cd3d-3bed-4ff2-b481-d6dffddb7179?modelname=Equip&fldname=ValFotograf&formIdentifier=FREPAR'
								}
							],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background: '',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Company repair number:',
									Icone: 'glyphicons-train',
									Valor: '2',
									Image: '',
									Url: ''
								},
								{
									Order: 2,
									Titulo: 'Name:',
									Icone: '',
									Valor: 'Álvaro Damas',
									Image: '',
									Url: ''
								},
								{
									Order: 3,
									Titulo: 'Specialty:',
									Icone: '',
									Valor: 'Automotive mechatronics',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-2 (Row-2)
						{
							Data: '2018-04-11 00:03:44',
							Texto: 'test by hand',
							Icon: 'glyphicons-train',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background: 'RGB(0, 255, 0)',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						//Timelien Item-3 (Row-3)
						{
							Data: '2021-05-11 00:03:44',
							Texto: 'Timelien Item-3',
							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background: 'RGB(0, 255, 0)',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-4 (Row-4)
						{
							Data: '2020-06-11 00:03:44',
							Texto: 'Timeline Item-4',
							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background: 'RGB(0, 255, 0)',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-5 (Row-5)
						{
							Data: '2019-06-11 00:03:44',
							Texto: 'Timeline Item-5',
							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background: 'RGB(0, 255, 0)',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-6 (Row-6)
						{
							Data: '2019-06-11 00:03:44',
							Texto: 'Timeline Item-6',
							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background:' RGB(255, 0, 0)',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-7 (Row-7)
						{
							Data: '2019-07-11 00:03:44',
							Texto: 'Timeline Item-7',
							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background: 'RGB(0, 255, 0)',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-8 (Row-8)
						{
							Data: '2019-07-14 00:03:44',
							Texto: 'Timeline Item-8',
							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background:'RGB(0, 50, 255)',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						}
					]
				},
				config: {
					scale: 'yy',
					dateTimeFormat:'dd/MM/yyyy HH:mm'
				}
			},
			// Timeline-2 (Monthly)
			monthlyTimeline: {
				// Timeline Item-1 (Row-1)
				tipoTimeline: 'S',
				timeLineData: {
					rows: [
						{
							Data: '2019-04-11 00:03:44',
							Texto: '2',
							Icon: 'glyphicons-train',
							ImagesColumns: [
								{
									Order: 1,
									Titulo: '',
									Icone: '',
									Valor: '',
									Image: `data:image/png;base64, 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`,
									Url: 'https://quidgest.net/GQT/en-US/0/GQT/Equip/ImageHandlerGet/f8f4cd3d-3bed-4ff2-b481-d6dffddb7179?modelname=Equip&fldname=ValFotograf&formIdentifier=FREPAR'
								},
								{
									Order: 2,
									Titulo: '',
									Icone: '',
									Valor: '',
									Image: `data:image/png;base64, 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`,
									Url: 'https://quidgest.net/GQT/en-US/0/GQT/Equip/ImageHandlerGet/f8f4cd3d-3bed-4ff2-b481-d6dffddb7179?modelname=Equip&fldname=ValFotograf&formIdentifier=FREPAR'
								}
							],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background: '',
							Style: 'D',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Company repair number:',
									Icone: 'glyphicons-train',
									Valor: '2',
									Image: '',
									Url: ''
								},
								{
									Order: 2,
									Titulo: 'Name:',
									Icone: '',
									Valor: 'Álvaro Damas',
									Image: '',
									Url: ''
								},
								{
									Order: 3,
									Titulo: 'Specialty:',
									Icone: '',
									Valor: 'Automotive mechatronics',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-2 (Row-2)
						{
							Data: '2018-04-11 00:03:44',
							Texto: 'test by hand',
							Icon: 'glyphicons-train',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background: 'RGB(0, 255, 0)',
							Style: 'D',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						//Timelien Item-3 (Row-3)
						{
							Data: '2021-05-11 00:03:44',
							Texto: 'Timelien Item-3',
							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background: 'RGB(0, 255, 0)',
							Style: 'D',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-4 (Row-4)
						{
							Data: '2020-06-11 00:03:44',
							Texto: 'Timeline Item-4',
							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background:
								'RGB(0, 255, 0)',
							Style: 'D',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-5 (Row-5)
						{
							Data: '2019-06-11 00:03:44',
							Texto: 'Timeline Item-5',
							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background:
								'RGB(0, 255, 0)',
							Style: 'D',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-6 (Row-6)
						{
							Data: '2019-06-11 00:03:44',
							Texto: 'Timeline Item-6',
							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background:
								'RGB(0, 255, 0)',
							Style: 'D',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-7 (Row-7)
						{
							Data: '2019-07-11 00:03:44',
							Texto: 'Timeline Item-7 Dynamic',
							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background:
								'RGB(255, 0, 0)',
							Style: 'D',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-8 (Row-8)
						{
							Data: '2019-07-14 00:03:44',
							Texto: 'Timeline Item-8',
							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background:
								'RGB(0, 255, 255)',
							Style: 'D',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						}
					]
				},
				config: {
					scale: 'mm',
					dateTimeFormat:'dd/MM/yyyy HH:mm'
				}
			},
			//  Timeline-3 (Daily)
			dailyTimeline: {
				// Timeline Item-1 (Row-1)
				tipoTimeline: 'S',
				timeLineData: {
					rows: [
						{
							Data: '2019-04-11 10:20:30',
							Texto: '2',
							Icon: '',
							ImagesColumns: [
								{
									Order: 1,
									Titulo: '',
									Icone: '',
									Valor: '',
									Image: '',
									Url: 'https://quidgest.net/GQT/en-US/0/GQT/Equip/ImageHandlerGet/f8f4cd3d-3bed-4ff2-b481-d6dffddb7179?modelname=Equip&fldname=ValFotograf&formIdentifier=FREPAR'
								}
							],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background: '',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Company repair number:',
									Icone: 'glyphicons-train',
									Valor: '2',
									Image: '',
									Url: ''
								},
								{
									Order: 2,
									Titulo: 'Name:',
									Icone: '',
									Valor: 'Álvaro Damas',
									Image: '',
									Url: ''
								},
								{
									Order: 3,
									Titulo: 'Specialty:',
									Icone: '',
									Valor: 'Automotive mechatronics',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-2 (Row-2)
						{
							Data: '2019-04-11 10:20:30',
							Texto: 'test by hand',

							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background:
								'RGB(0, 255, 0)',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						//Timelien Item-3 (Row-3)
						{
							Data: '2020-05-11 10:20:30',
							Texto: 'Timelien Item-3',
							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background:
								'RGB(0, 255, 0)',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-4 (Row-4)
						{
							Data: '2020-06-11 10:20:30',
							Texto: 'Timeline Item-4',
							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background:
								'RGB(0, 255, 0)',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-5 (Row-5)
						{
							Data: '2019-06-11 10:20:30',
							Texto: 'Timeline Item-5',
							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background:
								'RGB(0, 255, 0)',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-6 (Row-6)
						{
							Data: '2019-06-11 10:20:30',
							Texto: 'Timeline Item-6',
							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background:
								'RGB(0, 255, 0)',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-7 (Row-7)
						{
							Data: '2019-07-11 10:20:30',
							Texto: 'Timeline Item-7',

							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background:
								'RGB(0, 255, 0)',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-8 (Row-8)
						{
							Data: '2019-07-14 10:20:30',
							Texto: 'Timeline Item-8',

							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background:
								'RGB(0, 255, 0)',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-9 (Row-8)
						{
							Data: '2019-07-20 10:20:30',
							Texto: 'Timeline Item-8',
							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background:
								'RGB(0, 255, 0)',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						}
					]
				},
				config: {
					scale: 'dd',
					dateTimeFormat:'dd/MM/yyyy HH:mm'
				}
			},
			//  Timeline-4 (Weekly)
			weeklyTimeline: {
				// Timeline Item-1 (Row-1)
				tipoTimeline: 'S',
				timeLineData: {
					rows: [
						{
							Data: '2019-04-11 10:20:30',
							Texto: '2',
							Icon: '',
							ImagesColumns: [
								{
									Order: 1,
									Titulo: '',
									Icone: '',
									Valor: '',
									Image: '',
									Url: 'https://quidgest.net/GQT/en-US/0/GQT/Equip/ImageHandlerGet/f8f4cd3d-3bed-4ff2-b481-d6dffddb7179?modelname=Equip&fldname=ValFotograf&formIdentifier=FREPAR'
								}
							],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background: '',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Company repair number:',
									Icone: 'glyphicons-train',
									Valor: '2',
									Image: '',
									Url: ''
								},
								{
									Order: 2,
									Titulo: 'Name:',
									Icone: '',
									Valor: 'Álvaro Damas',
									Image: '',
									Url: ''
								},
								{
									Order: 3,
									Titulo: 'Specialty:',
									Icone: '',
									Valor: 'Automotive mechatronics',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-2 (Row-2)
						{
							Data: '2019-04-11 10:20:30',
							Texto: 'test by hand',

							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background:
								'RGB(0, 255, 0)',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						//Timelien Item-3 (Row-3)
						{
							Data: '2020-05-11 10:20:30',
							Texto: 'Timelien Item-3',
							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background:
								'RGB(0, 255, 0)',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-4 (Row-4)
						{
							Data: '2020-06-11 10:20:30',
							Texto: 'Timeline Item-4',
							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background:
								'RGB(0, 255, 0)',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-5 (Row-5)
						{
							Data: '2019-06-11 10:20:30',
							Texto: 'Timeline Item-5',
							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background:
								'RGB(0, 255, 0)',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-6 (Row-6)
						{
							Data: '2019-06-11 10:20:30',
							Texto: 'Timeline Item-6',
							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background:
								'RGB(0, 255, 0)',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-7 (Row-7)
						{
							Data: '2019-07-11 10:20:30',
							Texto: 'Timeline Item-7',
							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background:
								'RGB(0, 255, 0)',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-8 (Row-8)
						{
							Data: '2019-07-14 10:20:30',
							Texto: 'Timeline Item-8',
							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background:
								'RGB(0, 255, 0)',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						},
						// Timeline Item-9 (Row-8)
						{
							Data: '2019-07-20 10:20:30',
							Texto: 'Timeline Item-8',

							Icon: '',
							ImagesColumns: [],
							Url: 'https://quidgest.net/GQT/en-US/0/GQT/Repar/Repar_Show/d675e786-a19f-46e0-a87c-919656ef094b?nav=Q4B74q6d&amp;niv=1',
							Background:
								'RGB(0, 255, 0)',
							isPopupForm: true,
							Columns: [
								{
									Order: 1,
									Titulo: 'Title:',
									Icone: '',
									Valor: 'Hand Test',
									Image: '',
									Url: ''
								}
							]
						}
					]
				},
				config: {
					scale: 'ww',
					dateTimeFormat:'dd/MM/yyyy HH:mm'
				}
			}
		}
	},
	simpleUsageMethods: {
		runAction(eventName, emittedAction) {
			let str = eventName + ':\n [' + JSON.stringify(emittedAction)
			str += ']'
			alert(str)
		},
		displayEmit(emittedAction) {
			const str = JSON.stringify(emittedAction)
			alert(str)
		},
		formAction(emittedAction) {
			this.runAction('form-popup', emittedAction)
		}
	}
}
