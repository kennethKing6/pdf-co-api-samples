//*******************************************************************************************//
//                                                                                           //
// Download Free Evaluation Version From: https://bytescout.com/download/web-installer       //
//                                                                                           //
// Also available as Web API! Get Your Free API Key: https://app.pdf.co/signup               //
//                                                                                           //
// Copyright © 2017-2020 ByteScout, Inc. All rights reserved.                                //
// https://www.bytescout.com                                                                 //
// https://pdf.co                                                                            //
//                                                                                           //
//*******************************************************************************************//


var https = require("https");
var path = require("path");
var fs = require("fs");


// The authentication key (API Key).
// Get your own by registering at https://app.pdf.co/documentation/api
const API_KEY = "***********************************";


// Direct URL of source CSV file.
const SourceFileUrl = "https://bytescout-com.s3.amazonaws.com/files/demo-files/cloud-api/csv-to-pdf/sample.csv";
// Destination PDF file name
const DestinationFile = "./result.pdf";


// Prepare request to `CSV To PDF` API endpoint
var queryPath = `/v1/pdf/convert/from/csv?name=${path.basename(DestinationFile)}&url=${SourceFileUrl}`;
var reqOptions = {
    host: "api.pdf.co",
    path: encodeURI(queryPath),
    headers: {
        "x-api-key": API_KEY
    }
};
// Send request
https.get(reqOptions, (response) => {
    response.on("data", (d) => {
        // Parse JSON response
        var data = JSON.parse(d);        
        if (data.error == false) {
            // Download PDF file
            var file = fs.createWriteStream(DestinationFile);
            https.get(data.url, (response2) => {
                response2.pipe(file)
                .on("close", () => {
                    console.log(`Generated PDF file saved as "${DestinationFile}" file.`);
                });
            });
        }
        else {
            // Service reported error
            console.log(data.message);
        }
    });
}).on("error", (e) => {
    // Request error
    console.log(e);
});
