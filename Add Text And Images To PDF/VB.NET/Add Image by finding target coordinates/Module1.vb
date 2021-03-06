'*******************************************************************************************'
'                                                                                           '
' Download Free Evaluation Version From:     https://bytescout.com/download/web-installer   '
'                                                                                           '
' Also available as Web API! Get free API Key https://app.pdf.co/signup                     '
'                                                                                           '
' Copyright © 2017-2020 ByteScout, Inc. All rights reserved.                                '
' https://www.bytescout.com                                                                 '
' https://www.pdf.co                                                                        '
'*******************************************************************************************'


Imports System.IO
Imports System.Net
Imports Newtonsoft.Json.Linq

Module Module1

    ' The authentication key (API Key).
    ' Get your own by registering at https://app.pdf.co/documentation/api
    Const API_KEY As String = "***********************************"

    ' Direct URL of source PDF file.
    Const SourceFileUrl As String = "https://bytescout-com.s3.amazonaws.com/files/demo-files/cloud-api/pdf-edit/sample.pdf"

    ' Comma-separated list of page indices (Or ranges) to process. Leave empty for all pages. Example '0,2-5,7-'.
    Const Pages As String = ""

    ' PDF document password. Leave empty for unprotected documents.
    Const Password As String = ""

    ' Destination PDF file name
    Const DestinationFile As String = ".\result.pdf"

    ' Image params
    Private Const Type1 As String = "image"
    Private Const Width1 As Int32 = 119
    Private Const Height1 As Int32 = 32
    Private Const ImageUrl As String = "https://bytescout-com.s3.amazonaws.com/files/demo-files/cloud-api/pdf-edit/logo.png"

    Sub Main()

        ' Create standard .NET web client instance
        Dim webClient As WebClient = New WebClient()

        ' Set API Key
        webClient.Headers.Add("x-api-key", API_KEY)

        ' Find Text coordinates to add image
        Dim oCoordinates = FindCoordinates(API_KEY, SourceFileUrl, "Your Company Name")

        Dim X1 As Int32 = 450
        Dim Y1 As Int32 = oCoordinates.y

        ' * Add image *
        ' Prepare URL for `PDF Edit` API call
        Dim query As String = Uri.EscapeUriString(String.Format(
            "https://api.pdf.co/v1/pdf/edit/add?name={0}&password={1}&pages={2}&url={3}&type={4}&x={5}&y={6}&width={7}&height={8}&urlimage={9}",
            Path.GetFileName(DestinationFile),
                Password,
                Pages,
                SourceFileUrl,
                Type1,
                X1,
                Y1,
                Width1,
                Height1,
                ImageUrl))

        Try
            ' Execute request
            Dim response As String = webClient.DownloadString(query)

            ' Parse JSON response
            Dim json As JObject = JObject.Parse(response)

            If json("error").ToObject(Of Boolean) = False Then

                ' Get URL of generated PDF file
                Dim resultFileUrl As String = json("url").ToString()

                ' Download PDF file
                webClient.DownloadFile(resultFileUrl, DestinationFile)

                Console.WriteLine("Generated PDF file saved as ""{0}"" file.", DestinationFile)

            Else
                Console.WriteLine(json("message").ToString())
            End If

        Catch ex As WebException
            Console.WriteLine(ex.ToString())
        End Try

        webClient.Dispose()

        Console.WriteLine()
        Console.WriteLine("Press any key...")
        Console.ReadKey()

    End Sub

    ''' <summary>
    ''' Find result coordinates
    ''' </summary>
    Function FindCoordinates(ByVal API_KEY As String, ByVal SourceFileUrl As String, ByVal SearchString As String) As ResultCoOrdinates

        Dim oResult As New ResultCoOrdinates()

        ' Create standard .NET web client instance
        Dim webClient As WebClient = New WebClient()

        ' Set API Key
        webClient.Headers.Add("x-api-key", API_KEY)

        ' Prepare URL for PDF text search API call.
        ' See documentation: https : //app.pdf.co/documentation/api/1.0/pdf/find.html
        Dim query As String = Uri.EscapeUriString(
            String.Format("https://api.pdf.co/v1/pdf/find?url={0}&searchString={1}",
                SourceFileUrl,
                SearchString))

        Try
            ' Execute request
            Dim response As String = webClient.DownloadString(query)

            ' Parse JSON response
            Dim json As JObject = JObject.Parse(response)

            If json("error").ToObject(Of Boolean) = False Then

                Dim item = json("body")(0)

                oResult.x = item("left")
                oResult.y = item("top")
                oResult.width = item("width")
                oResult.height = item("height")

            End If

        Catch ex As WebException
            Console.WriteLine(ex.ToString())
        End Try

        webClient.Dispose()

        Return oResult

    End Function

    Class ResultCoOrdinates

        Public x As Integer
        Public y As Integer
        Public width As Integer
        Public height As Integer

    End Class


End Module
