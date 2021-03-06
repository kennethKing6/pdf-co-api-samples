import os
import requests  # pip install requests

# The authentication key (API Key).
# Get your own by registering at https://app.pdf.co/documentation/api
API_KEY = "******************************************"

# Base URL for PDF.co Web API requests
BASE_URL = "https://api.pdf.co/v1"

# Direct URL of source file to search barcodes in.
SourceFileURL = "https://bytescout-com.s3.amazonaws.com/files/demo-files/cloud-api/barcode-reader/sample.pdf"

# Comma-separated list of barcode types to search.
# See valid barcode types in the documentation https://app.pdf.co/documentation/api/1.0/barcode/read_from_url.html
BarcodeTypes = "Code128,Code39,Interleaved2of5,EAN13"

# Comma-separated list of page indices (or ranges) to process. Leave empty for all pages. Example: '0,2-5,7-'.
Pages = ""

# Some of advanced options available through profiles:
# (JSON can be single/double-quoted and contain comments.)
# {
#     "profiles": [
#         {
#             "profile1": {
#                 "ScanArea": "WholePage", // Values: "TopLeftQuarter", "TopRightQuarter", "BottomRightQuarter", "BottomLeftQuarter", "TopHalf", "BottomHalf", "WholePage".
#                 "RequireQuietZones": true, // Whether the quite zone is obligatory for 1D barcodes. Values: true / false
#                 "MaxNumberOfBarcodesPerPage": 0, // 0 - unlimited.
#                 "MaxNumberOfBarcodesPerDocument": 0, // 0 - unlimited.
#                 "ScanStep": 1, // Scan interval for linear (1-dimensional) barcodes.
#                 "MinimalDataLength": 0, // Minimal acceptable length of decoded data.                
#             }
#         }
#     ]
# }

# Conversation Options
Profiles = "{ 'profiles': [ { 'profile1': { 'FastMode': true } } ] }"

def main(args=None):
    readBarcodes(SourceFileURL)

def readBarcodes(uploadedFileUrl):
    """Get Barcode Information using PDF.co Web API"""

    # Prepare URL for 'Barcode Reader' API request
    url = "{}/barcode/read/from/url?types={}&pages={}&url={}&profiles={}".format(
        BASE_URL,
        BarcodeTypes,
        Pages,
        uploadedFileUrl,
        Profiles
    )

    # Execute request and get response as JSON
    response = requests.get(url, headers={"x-api-key": API_KEY, "content-type": "application/octet-stream"})
    if (response.status_code == 200):
        json = response.json()

        if json["error"] == False:
            # Display information
            for barcode in json["barcodes"]:
                print("Found barcode:")
                print(f"  Type: {barcode['TypeName']}")
                print(f"  Value: {barcode['Value']}")
                print(f"  Document Page Index: {barcode['Page']}")
                print(f"  Rectangle: {barcode['Rect']}")
                print(f"  Confidence: {barcode['Confidence']}")
                print("")
        else:
            # Show service reported error
            print(json["message"])
    else:
        print(f"Request error: {response.status_code} {response.reason}")


if __name__ == '__main__':
    main()