const express = require("express")
const moment = require('moment');
const { CanvasRenderService } = require('chartjs-node-canvas');
let app = express()
const port = 3000;

const parseData = (source) => {
	debugger;
	let labels = [];
	let data = [];
	source.forEach(element => {
		labels.push(element.time);
		data.push(element.data);
	});
	return {labels, data};
}

app.get('/chart', async function (req, res) {//, res
    try {
		var weatherJSON = JSON.parse(req.headers.req);
		const parsedData = parseData(weatherJSON);
		const canvasRenderService = new CanvasRenderService(800, 800);
		const image = await canvasRenderService.renderToBuffer({
			type: 'line',
			data: {
				labels: parsedData.labels,
				datasets: [{
					label: 'Weather on ' + moment().format('MMM Do YYYY'),
					data: parsedData.data,
					borderColor: 'rgba(75, 192, 192, 1)',
                    backgroundColor: 'rgba(75, 192, 192, 0.2)'
				}]
			}
		});
		res.type('image/jpeg');
		res.end(image, 'binary');
		fi
		return image;
	} catch (error) {
		console.log(error);
	}
});

app.listen(port, () => console.log(`Example app listening on port ${port}!`))


const testSource = [
	{time: "10:00", data:-7},
	{time: "10:20", data:-3},
	{time: "10:40", data:-5},
	{time: "11:00", data:0},
	{time: "11:20", data:5}
]

const testSource2 =[
	{"time":"18:00","data":0},
	{"time":"21:00","data":-1}
]
