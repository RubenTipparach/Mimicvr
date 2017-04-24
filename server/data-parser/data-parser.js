//var logdata = require('./all-logs-slim.json');
var fs = require('fs');
var json2csv = require('json2csv');

const testFolder = './test_data/';

fs.readdir(testFolder, (err, files) => {
    files.forEach((file) => {
        console.log(file);

        fs.readFile(testFolder + file, 'utf8', function (err, data) {
            if (err) { return console.log(err); }

            var arr = data.split('\r\n');
            var vrbots = [];
            var simbots = [];

            // push data in an array of vr runs or simbots runs
            for (let i = 0; i < arr.length -1; i ++) {
                a = arr[i];
                var item = (JSON.parse(a));
                if (item.robot_sim) {

                    simbots.push(item);
                }
                else if (item.robot_vr) {

                    vrbots.push(item);
                }
            }

            //console.log(vrbots);

     
            var fields = ['robot_vr.position.x', 'robot_vr.position.y', 'robot_vr.position.z'];

    
            transformRawData(vrbots, fields, 'robot_vr' + file);

            var fields = ['robot_sim.position.x', 'robot_sim.position.y', 'robot_sim.position.z'];

            transformRawData(simbots, fields, 'robot_sim' + file);
    

            //for (var simbot of simbots) {
            //    transformRawData(simbot, 'robot_sim', fields, file)
            //}
        });
    });
});


// file writing process.
function transformRawData(items, fields, file)
{
    // write csv.
    try {
        var result = json2csv({ data: items, fields: fields });
        //console.log(result);

        fs.writeFile("./"+file+".csv", result, function (err) {
            if (err) {return console.log(err);}

            console.log("The file was saved!");
        });

    } catch (err) {
        console.error(err);
    }

    // write json.
    fs.writeFile("./" + file, JSON.stringify(items), function (err) {
        if (err) { return console.log(err); }

        console.log("The file was saved!");
    });

}
