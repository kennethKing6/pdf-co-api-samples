# Cloud API asynchronous "Merge PDF" job example.
# Allows to avoid timeout errors when processing huge or scanned PDF documents.

# The authentication key (API Key).
# Get your own by registering at https://app.pdf.co/documentation/api
$API_KEY = "***********************************"

# Direct URLs of PDF documents to merge
$SourceFiles = @(
    "https://s3-us-west-2.amazonaws.com/bytescout-com/files/demo-files/cloud-api/pdf-merge/sample1.pdf",
    "https://s3-us-west-2.amazonaws.com/bytescout-com/files/demo-files/cloud-api/pdf-merge/sample2.pdf"
)
# Destination PDF file name
$DestinationFile = ".\result.pdf"
# (!) Make asynchronous job
$Async = $true


# Prepare URL for `Merge PDF` API call
$query = "https://api.pdf.co/v1/pdf/merge?name={0}&url={1}&async={2}" -f `
    $(Split-Path $DestinationFile -Leaf), $($SourceFiles -join ","), $Async
$query = [System.Uri]::EscapeUriString($query)

try {
    # Execute request
    $jsonResponse = Invoke-RestMethod -Method Get -Headers @{ "x-api-key" = $API_KEY } -Uri $query

    if ($jsonResponse.error -eq $false) {
        # Asynchronous job ID
        $jobId = $jsonResponse.jobId
        # URL of generated PDF file that will available after the job completion
        $resultFileUrl = $jsonResponse.url

        # Check the job status in a loop. 
        do {
            $statusCheckUrl = "https://api.pdf.co/v1/job/check?jobid=" + $jobId
            $jsonStatus = Invoke-RestMethod -Method Get -Headers @{ "x-api-key" = $API_KEY } -Uri $statusCheckUrl

            # Display timestamp and status (for demo purposes)
            Write-Host "$(Get-date): $($jsonStatus.status)"

            if ($jsonStatus.status -eq "success") {
                # Download PDF file
                Invoke-WebRequest -Headers @{ "x-api-key" = $API_KEY } -OutFile $DestinationFile -Uri $resultFileUrl
                Write-Host "Generated PDF file saved as `"$($DestinationFile)`" file."
                break
            }
            elseif ($jsonStatus.status -eq "working") {
                # Pause for a few seconds
                Start-Sleep -Seconds 3
            }
            else {
                Write-Host $jsonStatus.status
                break
            }
        }
        while ($true)
    }
    else {
        # Display service reported error
        Write-Host $jsonResponse.message
    }
}
catch {
    # Display request error
    Write-Host $_.Exception
}
