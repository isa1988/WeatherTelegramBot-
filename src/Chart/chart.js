const express = require("express")
const moment = require('moment');
const { CanvasRenderService } = require('chartjs-node-canvas');
let app = express()
const port = 3000;

const parseDictinary = (source) => {
	let retObject = {};
	source.forEach(element => {
		retObject[element.id] = element.desc;
	});
	return retObject;
}

const parseData = (source) => {
	let labels = [];
	let data = [];
	source.forEach(element => {
		labels.push(element.time);
		data.push(element.data);
	});
	return {labels, data};
}

app.get('/charttemperature', async function (req, res) {//, res
    try {
		var weatherJSON = JSON.parse(req.headers.temperature);
		var feelTemperatureJSON = JSON.parse(req.headers.feeltemperature);
		var dateStr = req.headers.datestr;
		const parsedData = parseData(weatherJSON);
		const parsedData2 = parseData(feelTemperatureJSON);
		const canvasRenderService = new CanvasRenderService(800, 800);
		const image = await canvasRenderService.renderToBuffer({
			type: 'line',
			options: {
				title: {
					display: true,
					text: 'Погода на ' + dateStr +  " (°C)"
				},
				legend: {
					display: true,
					labels: {
						fontColor: 'rgb(56, 61, 224)'
					},
				}
			},
			data: {
				labels: parsedData.labels,
				datasets: [{
					label: 'по прогнозу',
					data: parsedData.data,
					borderColor: 'rgb(44, 237, 102)',
					number: 15,
					pointStyle: 'star',
					//backgroundColor: 'rgba(75, 192, 192, 0.2)'
				}, {
					label: "чувствуется",
					data: parsedData2.data,
					borderColor: 'rgb(97, 167, 184)',
					number: 15,
					pointStyle: 'star',
					//backgroundColor: 'rgba(75, 192, 192, 0.2)'
				}]
			},

		});
		res.type('image/jpeg');
		res.end(image, 'binary');

		return image;
	} catch (error) {
		console.log(error);
	}
});

app.get('/chartdesc', async function (req, res) {//, res
	try {
		var descTemperatureJSON = JSON.parse(req.headers.desctemperature);
		var dateStr = req.headers.datestr;
		var weatherInfo = JSON.parse(req.headers.weatherinfo);
		const weatherDsta = parseDictinary(weatherInfo);
		const parsedData = parseData(descTemperatureJSON);
		const canvasRenderService = new CanvasRenderService(800, 800);
		const image = await canvasRenderService.renderToBuffer({
			type: 'line',
			options: {
				title: {
					display: true,
					text: 'Погода на ' + dateStr +  " (°C)"
				},
				legend: {
					display: true,
					labels: {
						fontColor: 'rgb(56, 61, 224)'
					},
				},
				scales: {
					yAxes: [{
						ticks: {
							callback: function(value, index, values) {
								// for a value (tick) equals to 8
								return weatherDsta[value];
								// 'junior-dev' will be returned instead and displayed on your chart
							}
						}
					}]
				},
			},
			data: {
				labels: parsedData.labels,
				datasets: [{
					label: 'по прогнозу',
					data: parsedData.data,
					borderColor: 'rgb(44, 237, 102)',
					number: 15,
					pointStyle: 'star',
					//backgroundColor: 'rgba(75, 192, 192, 0.2)'
				}]
			},

		});
		res.type('image/jpeg');
		res.end(image, 'binary');

		return image;
	} catch (error) {
		console.log(error);
	}
});

app.get('/charttest', async function (req, res) {//, res
	try {
		const parsedData = parseData(testSource);
		const canvasRenderService = new CanvasRenderService(800, 800);
		const image = await canvasRenderService.renderToBuffer({
			type: 'line',
			options: {
				title: {
					display: true,
					text: 'Погода на (°C)'
				},
				legend: {
					display: true,
					labels: {
						fontColor: 'rgb(56, 61, 224)'
					},
				}
			},
			data: {
				labels: parsedData.labels,
				datasets: [{
					label: 'по прогнозу',
					data: parsedData.data,
					borderColor: 'rgb(44, 237, 102)',
					number: 15,
					pointStyle: 'star',
					//backgroundColor: 'rgba(75, 192, 192, 0.2)'
				}]
			},

		});
		res.type('image/jpeg');
		res.end(image, 'binary');

		return image;
	} catch (error) {
		console.log(error);
	}
});


app.listen(port, () => console.log(`Example app listening on port ${port}!`))


const testSource = [
	{time: "10:00", data:"b"},
	{time: "10:20", data:"c"},
	{time: "10:40", data:"a"},
	{time: "11:00", data:"p"},
	{time: "11:20", data:"g"}
]

const testSource2 =[
	{"time":"18:00","data":0},
	{"time":"21:00","data":-1}
]
