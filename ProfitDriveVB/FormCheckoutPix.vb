Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports System.Diagnostics

Public Class FormCheckoutPix
    Inherits Form

    Private _planoNome As String
    Private _valorPlano As Decimal

    Private COR_CANVAS As Color = Color.FromArgb(15, 17, 21)
    Private COR_CARD As Color = Color.FromArgb(26, 32, 44)
    Private COR_TEXTO_PRINCIPAL As Color = Color.FromArgb(248, 250, 252)
    Private COR_TEXTO_MUTED As Color = Color.FromArgb(148, 163, 184)
    Private COR_DESTAQUE As Color = Color.FromArgb(56, 189, 248)
    Private COR_VERDE As Color = Color.FromArgb(16, 185, 129)
    Private COR_VERDE_WPP As Color = Color.FromArgb(37, 211, 102)

    'INCLUA CHAVE PIX AQUI
    Private CHAVE_PIX_RECEBEDOR As String = "seu-email-ou-cpf@giroliquido.com.br"
    Private NUMERO_WHATSAPP_SUPORTE As String = "5511999999999" ' Coloque seu número com DDI e DDD

    Sub New(planoNome As String, valorPlano As Decimal)
        Me._planoNome = planoNome
        Me._valorPlano = valorPlano

        Me.Text = "Flow Road- Checkout Seguro"
        Me.Size = New Size(500, 650)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.BackColor = COR_CANVAS

        MontarLayoutCheckout()
    End Sub

    Private Sub MontarLayoutCheckout()
        'HEADER
        Dim lblTit As New Label() With {.Text = "Finalizar Assinatura", .Font = New Font("Segoe UI", 16, FontStyle.Bold), .ForeColor = COR_TEXTO_PRINCIPAL, .Location = New Point(0, 20), .Size = New Size(485, 35), .TextAlign = ContentAlignment.MiddleCenter}
        Dim lblSub As New Label() With {.Text = "Você está a um passo de turbinar seu cockpit.", .Font = New Font("Segoe UI", 10), .ForeColor = COR_TEXTO_MUTED, .Location = New Point(0, 55), .Size = New Size(485, 20), .TextAlign = ContentAlignment.MiddleCenter}
        Me.Controls.AddRange(New Control() {lblTit, lblSub})

        'CARD DO PEDIDO
        Dim pnlResumo As New Panel() With {.Location = New Point(35, 95), .Size = New Size(415, 90), .BackColor = COR_CARD}
        AddHandler pnlResumo.Paint, AddressOf EstilizarBordaPainelGlass

        Dim lblPlano = New Label() With {.Text = $"Plano {_planoNome.ToUpper()}", .Location = New Point(20, 20), .Font = New Font("Segoe UI", 11, FontStyle.Bold), .ForeColor = COR_DESTAQUE, .AutoSize = True}
        Dim lblDesc = New Label() With {.Text = "Licença mensal intransferível", .Location = New Point(20, 45), .Font = New Font("Segoe UI", 9), .ForeColor = COR_TEXTO_MUTED, .AutoSize = True}
        Dim lblValor = New Label() With {.Text = _valorPlano.ToString("C2"), .Location = New Point(240, 25), .Size = New Size(155, 40), .Font = New Font("Segoe UI", 18, FontStyle.Bold), .ForeColor = COR_TEXTO_PRINCIPAL, .TextAlign = ContentAlignment.MiddleRight}

        pnlResumo.Controls.AddRange(New Control() {lblPlano, lblDesc, lblValor})
        Me.Controls.Add(pnlResumo)

        'ÁREA DO PIX
        Dim pnlPix As New Panel() With {.Location = New Point(35, 200), .Size = New Size(415, 240), .BackColor = COR_CARD}
        AddHandler pnlPix.Paint, AddressOf EstilizarBordaPainelGlass

        Dim lblPixTit = New Label() With {.Text = "Pagamento via PIX", .Location = New Point(0, 15), .Size = New Size(415, 25), .Font = New Font("Segoe UI", 11, FontStyle.Bold), .ForeColor = COR_TEXTO_PRINCIPAL, .TextAlign = ContentAlignment.MiddleCenter}

        ' Placeholder do QR Code (trocar por um PictureBox com imagem real)
        Dim pnlQRCode As New Panel() With {.Location = New Point(142, 50), .Size = New Size(130, 130), .BackColor = Color.FromArgb(15, 17, 21)}
        AddHandler pnlQRCode.Paint, Sub(s, e)
                                        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
                                        Using p As New Pen(COR_DESTAQUE, 1.5F) : e.Graphics.DrawRectangle(p, 0, 0, 129, 129) : End Using
                                        e.Graphics.DrawString("QR CODE", New Font("Segoe UI", 10, FontStyle.Bold), Brushes.Gray, New PointF(30, 55))
                                    End Sub

        Dim txtChavePix = New TextBox() With {.Text = CHAVE_PIX_RECEBEDOR, .Location = New Point(30, 195), .Size = New Size(260, 25), .Font = New Font("Segoe UI", 10), .BackColor = Color.FromArgb(15, 17, 21), .ForeColor = COR_TEXTO_PRINCIPAL, .BorderStyle = BorderStyle.FixedSingle, .ReadOnly = True, .TextAlign = HorizontalAlignment.Center}
        Dim btnCopiar = New Button() With {.Text = "Copiar", .Location = New Point(300, 194), .Size = New Size(85, 27), .BackColor = Color.FromArgb(51, 65, 85), .ForeColor = COR_TEXTO_PRINCIPAL, .Font = New Font("Segoe UI", 9, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnCopiar.FlatAppearance.BorderSize = 0
        AddHandler btnCopiar.Paint, AddressOf ArredondarBotaoGDI
        AddHandler btnCopiar.Click, Sub()
                                        Clipboard.SetText(CHAVE_PIX_RECEBEDOR)
                                        btnCopiar.Text = "Copiado!"
                                        btnCopiar.BackColor = COR_VERDE
                                    End Sub

        pnlPix.Controls.AddRange(New Control() {lblPixTit, pnlQRCode, txtChavePix, btnCopiar})
        Me.Controls.Add(pnlPix)

        'WHATSAPP
        Dim lblAviso = New Label() With {.Text = "Após o pagamento, envie o comprovante para liberação:", .Location = New Point(0, 455), .Size = New Size(485, 20), .Font = New Font("Segoe UI", 9), .ForeColor = COR_TEXTO_MUTED, .TextAlign = ContentAlignment.MiddleCenter}
        Me.Controls.Add(lblAviso)

        Dim btnWpp As New Button() With {.Text = "Enviar Comprovante via WhatsApp", .Location = New Point(35, 485), .Size = New Size(415, 45), .BackColor = COR_VERDE_WPP, .ForeColor = Color.White, .Font = New Font("Segoe UI", 11, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnWpp.FlatAppearance.BorderSize = 0
        AddHandler btnWpp.Paint, AddressOf ArredondarBotaoGDI
        AddHandler btnWpp.Click, AddressOf AbrirWhatsApp

        Me.Controls.Add(btnWpp)

        Dim btnVoltar As New Button() With {.Text = "Cancelar e Voltar", .Location = New Point(35, 545), .Size = New Size(415, 40), .BackColor = Color.Transparent, .ForeColor = COR_TEXTO_MUTED, .Font = New Font("Segoe UI", 10, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnVoltar.FlatAppearance.BorderSize = 0
        btnVoltar.FlatAppearance.MouseDownBackColor = COR_CANVAS
        btnVoltar.FlatAppearance.MouseOverBackColor = COR_CANVAS
        AddHandler btnVoltar.Click, Sub() Me.Close()
        Me.Controls.Add(btnVoltar)
    End Sub

    Private Sub AbrirWhatsApp(sender As Object, e As EventArgs)
        Dim msg As String = Uri.EscapeDataString($"Olá, equipe Flow Road! Acabei de fazer o PIX para ativação do Plano {_planoNome.ToUpper()}. Segue o meu comprovante:")
        Dim url As String = $"https://wa.me/{NUMERO_WHATSAPP_SUPORTE}?text={msg}"

        Try
            ' Abre o navegador padrão do Windows direto na conversa do WhatsApp
            Process.Start(New ProcessStartInfo With {.FileName = url, .UseShellExecute = True})
            Me.Close()
        Catch ex As Exception
            MessageBox.Show("Não foi possível abrir o navegador. Nosso número é: " & NUMERO_WHATSAPP_SUPORTE, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try
    End Sub

    Private Sub EstilizarBordaPainelGlass(sender As Object, e As PaintEventArgs)
        Dim pnl = DirectCast(sender, Panel)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Using p As New Pen(Color.FromArgb(51, 65, 85), 1.2F) : e.Graphics.DrawRectangle(p, 0, 0, pnl.Width - 1, pnl.Height - 1) : End Using
    End Sub

    Private Sub ArredondarBotaoGDI(sender As Object, e As PaintEventArgs)
        Dim btn = DirectCast(sender, Button)
        e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        Dim r = 6 : Dim path As New Drawing2D.GraphicsPath()
        path.AddArc(0, 0, r * 2, r * 2, 180, 90) : path.AddArc(btn.Width - r * 2 - 1, 0, r * 2, r * 2, 270, 90)
        path.AddArc(btn.Width - r * 2 - 1, btn.Height - r * 2 - 1, r * 2, r * 2, 0, 90) : path.AddArc(0, btn.Height - r * 2 - 1, r * 2, r * 2, 90, 90)
        path.CloseAllFigures() : btn.Region = New Region(path)
        Using b As New SolidBrush(btn.BackColor) : e.Graphics.FillPath(b, path) : End Using
        TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, New Rectangle(0, 0, btn.Width, btn.Height), btn.ForeColor, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
    End Sub

End Class
