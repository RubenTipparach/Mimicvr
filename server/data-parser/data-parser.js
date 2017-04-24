//var logdata = require('./all-logs-slim.json');

var fs = require('fs');

fs.readFile('./all-logs-slim.json', 'utf8', function (err,data) {
  if (err) {
    return console.log(err);
  }
  var arr = data.split('\r\n');
  
  var vrbots = [];
  var simbots = [];
  
  for(var a of arr)
  {
	  var item = (JSON.parse(a));
	if(item.robot_sim ){

		simbots.push(item);
	}
	else if(item.robot_vr)
	{
		
		vrbots.push(item);
	}
  }
  
  console.log(vrbots);
  
	var json2csv = require('json2csv');
	var fields = ['position.x', 'position.y', 'position.z'];
	 
	 
	try {
	  var result = json2csv({ data: vrbots[0].robot_vr, fields: fields });
	  console.log(result);
	  
	  fs.writeFile("./result.csv", result, function(err) {
		if(err) {
			return console.log(err);
		}

		console.log("The file was saved!");
		}); 
		
	} catch (err) {
	  // Errors are thrown for bad options, or if the data is empty and no fields are provided. 
	  // Be sure to provide fields if it is possible that your data array will be empty. 
	  console.error(err);
	}
});

