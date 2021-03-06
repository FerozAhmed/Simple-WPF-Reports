﻿Imports System.IO, System.Windows.Documents, System.Windows.Media, System.Windows.Media.Imaging, System.Windows.Xps.Packaging

''' <summary>
''' Provides methods for converting XPS document in to various image format
''' </summary>
Public Class XpsImage
    Private bitmapEncoder As BitmapEncoder

    ''' <summary>
    ''' Sets the XPS file to be read
    ''' </summary>
    Property XpsFileName As String

    ''' <summary>
    ''' Gets or Sets the image format for thumbnail
    ''' </summary>
    Property OutputFormat As Output

    ''' <summary>
    ''' Returns the Memory stream of generated thumbnail
    ''' </summary>
    Property OutputStream As MemoryStream

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="XpsFileName"></param>
    ''' <param name="OutputFormat"></param>
    ''' <remarks></remarks>
    ''' <stepthrough></stepthrough>
    Sub New(XpsFileName As String, OutputFormat As Output)
        Me.XpsFileName = XpsFileName
        Me.OutputFormat = OutputFormat
    End Sub

    ''' <summary>
    ''' Generate the thumbnail of given document and populates the ThumbnailStream property
    ''' </summary>
    Sub GenerateThumbnail()
        Dim xpsDocument As New XpsDocument(Me.XpsFileName, FileAccess.Read)
        Dim documentPageSequence As FixedDocumentSequence = xpsDocument.GetFixedDocumentSequence()

        Dim fileNameWithoutExtension As String = Path.GetFileNameWithoutExtension(Me.XpsFileName)
        Dim fileExtension As String = String.Empty

        Select Case Me.OutputFormat
            Case Output.Jpeg
                fileExtension = ".jpg"
                bitmapEncoder = New JpegBitmapEncoder()
            Case Output.Png
                fileExtension = ".png"
                bitmapEncoder = New PngBitmapEncoder()
            Case Output.Gif
                fileExtension = ".gif"
                bitmapEncoder = New GifBitmapEncoder()
            Case Else
                fileExtension = ".jpg"
                bitmapEncoder = New JpegBitmapEncoder()
        End Select

        Dim documentPage As DocumentPage = documentPageSequence.DocumentPaginator.GetPage(0)
        Dim targetBitmap As New RenderTargetBitmap(documentPage.Size.Width * 5, documentPage.Size.Height * 5, 96.0 * 5, 96.0 * 5, PixelFormats.Pbgra32)
        targetBitmap.Render(documentPage.Visual)

        bitmapEncoder.Frames.Add(BitmapFrame.Create(targetBitmap))
        Dim str4 As String = String.Format("{0}{1}", fileNameWithoutExtension, fileExtension)

        Dim memoryStream As New MemoryStream()
        bitmapEncoder.Save(memoryStream)
        Me.OutputStream = memoryStream
        xpsDocument.Close()
    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <stepthrough></stepthrough>
    Function Save() As MemoryStream
        GenerateThumbnail()

        Dim Data As New IO.MemoryStream
        Using stream As New FileStream(Me.XpsFileName, FileMode.Create, FileAccess.Write)
            Me.OutputStream.WriteTo(Data)
        End Using

        Return Data
    End Function

    ''' <summary>
    ''' Feature Not Constructed Yet
    ''' </summary>
    ''' <param name="Report"></param>
    ''' <remarks></remarks>
    ''' <stepthrough></stepthrough>
    Sub Save(Report As Reports.Basic)
        Throw New ApplicationException("Feature Not Constructed Yet")
    End Sub


End Class


Partial Class XpsImage
    ''' <summary>
    ''' Imageformat
    ''' </summary>
    Public Enum Output
        Jpeg
        Png
        Gif
    End Enum
End Class