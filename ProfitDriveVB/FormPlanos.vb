Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports System.Linq

Public Class FormPlanos
    Inherits Form

    Private _usuarioIdLogado As Integer
    Private _planoAtual As String
    Private COR_CANVAS As Color = Color.FromArgb(15, 17, 21)
    Private COR_CARD As Color = Color.FromArgb(26, 32, 44)
    Private COR_TEXTO_PRINCIPAL As Color = Color.FromArgb(248, 250, 252)
    Private COR_TEXTO_MUTED As Color = Color.FromArgb(148, 163, 184)
    Private COR_BTN_DARK As Color = Color.FromArgb(30, 41, 59)
    Private COR_VERDE As Color = Color.FromArgb(16, 185, 129)

    Private COR_ESSENTIAL As Color = Color.FromArgb(203, 213, 225) ' Prata Gelo
    Private COR_PERFORMANCE As Color = Color.FromArgb(56, 189, 248) ' Ciano
    Private COR_BLACK As Color = Color.FromArgb(234, 179, 8) ' Dourado 

    Sub New(usuarioId As Integer)
        Me._usuarioIdLogado = usuarioId
        _planoAtual = ""
        Me.Text = "Flow Road - Upgrade de Ecossistema"
        Me.Size = New Size(960, 600)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.BackColor = COR_CANVAS

        ObterPlanoAtualDoBanco()
        MontarLayoutPlanos()
    End Sub

    Private Sub ObterPlanoAtualDoBanco()
        Using db As New AppDbContext()
            Dim u = db.Usuarios.FirstOrDefault(Function(user) user.Id = _usuarioIdLogado)
            If u IsNot Nothing Then
                Try
                    Dim planoBanco = CallByName(u, "CategoriaPlano", CallType.Get).ToString().Trim().ToUpper()
                    If planoBanco = "BRONZE" Then _planoAtual = "ESSENTIAL"
                    If planoBanco = "PRATA" Then _planoAtual = "PERFORMANCE"
                    If planoBanco = "OURO" Then _planoAtual = "BLACK"
                    If planoBanco = "ESSENTIAL" OrElse planoBanco = "PERFORMANCE" OrElse planoBanco = "BLACK" Then
                        _planoAtual = planoBanco
                    End If
                Catch : End Try
            End If
        End Using
    End Sub

    Private Sub MontarLayoutPlanos()
        'HEADER
        Dim pnlHeader As New Panel() With {.Location = New Point(30, 20), .Size = New Size(885, 90), .BackColor = COR_CARD}
        AddHandler pnlHeader.Paint, AddressOf EstilizarBordaPainelGlass

        Dim lblTitHeader As New Label() With {.Text = "Escolha a licença ideal para a sua operação contábil", .Font = New Font("Segoe UI", 14, FontStyle.Bold), .ForeColor = COR_TEXTO_PRINCIPAL, .Location = New Point(0, 20), .Size = New Size(885, 30), .TextAlign = ContentAlignment.MiddleCenter}

        Dim corAtual As Color = COR_ESSENTIAL
        If _planoAtual = "PERFORMANCE" Then corAtual = COR_PERFORMANCE
        If _planoAtual = "BLACK" Then corAtual = COR_BLACK

        Dim lblSubHeader As New Label() With {.Text = $"Licença vigente: {_planoAtual}", .Font = New Font("Segoe UI", 10.5, FontStyle.Bold), .ForeColor = corAtual, .Location = New Point(0, 55), .Size = New Size(885, 25), .TextAlign = ContentAlignment.MiddleCenter}

        pnlHeader.Controls.AddRange(New Control() {lblTitHeader, lblSubHeader})
        Me.Controls.Add(pnlHeader)

        Dim featuresEssential = {"Lançamentos Avulsos", "Gestão de 1 Veículo", "Dashboard Financeiro", "Acesso Padrão"}
        Dim featuresPerformance = {"Cálculo de Eficiência Real", "Telemetria de Consumo", "Suporte a EV / Híbrido", "Atendimento Prioritário"}
        Dim featuresBlack = {"Gestão de Múltiplos Veículos", "Relatórios Executivos (PDF)", "Business Intelligence (BI)", "Cloud Sync em Tempo Real"}

        Dim pnlEssential = CriarCardPlano("ESSENTIAL", "Gratuito", featuresEssential, 30, 130, COR_ESSENTIAL)
        Dim pnlPerformance = CriarCardPlano("PERFORMANCE", "R$ 14,90/mês", featuresPerformance, 335, 130, COR_PERFORMANCE)
        Dim pnlBlack = CriarCardPlano("BLACK", "R$ 29,90/mês", featuresBlack, 640, 130, COR_BLACK)

        Me.Controls.AddRange(New Control() {pnlEssential, pnlPerformance, pnlBlack})

        Dim btnVoltar As New Button() With {.Text = "Voltar ao Cockpit", .Location = New Point(30, 495), .Size = New Size(885, 45), .BackColor = COR_BTN_DARK, .ForeColor = COR_TEXTO_PRINCIPAL, .Font = New Font("Segoe UI", 10, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnVoltar.FlatAppearance.BorderSize = 0
        AddHandler btnVoltar.Paint, AddressOf ArredondarBotaoGDI
        AddHandler btnVoltar.Click, Sub() Me.Close()
        Me.Controls.Add(btnVoltar)
    End Sub

    Private Function CriarCardPlano(nomePlano As String, preco As String, recursos() As String, x As Integer, y As Integer, corDestaque As Color) As Panel
        Dim pnl As New Panel() With {.Location = New Point(x, y), .Size = New Size(275, 345), .BackColor = COR_CARD, .BorderStyle = BorderStyle.None}

        AddHandler pnl.Paint, Sub(s, e)
                                  Dim g As Graphics = e.Graphics
                                  g.SmoothingMode = SmoothingMode.AntiAlias
                                  g.PixelOffsetMode = PixelOffsetMode.HighQuality

                                  Dim éDestaque As Boolean = (_planoAtual = nomePlano OrElse nomePlano = "BLACK")
                                  Dim espessura As Integer = If(éDestaque, 2, 1)
                                  Dim corBorda As Color = If(éDestaque, corDestaque, Color.FromArgb(51, 65, 85))

                                  Using p As New Pen(corBorda, espessura)
                                      g.DrawRectangle(p, 0, 0, pnl.Width - 1, pnl.Height - 1)
                                  End Using
                              End Sub

        Dim lblNome As New Label() With {.Text = nomePlano, .Font = New Font("Segoe UI", 11, FontStyle.Bold), .ForeColor = corDestaque, .Location = New Point(0, 25), .Size = New Size(275, 25), .TextAlign = ContentAlignment.MiddleCenter}
        Dim lblPreco As New Label() With {.Text = preco, .Font = New Font("Segoe UI", 18, FontStyle.Bold), .ForeColor = COR_TEXTO_PRINCIPAL, .Location = New Point(0, 60), .Size = New Size(275, 35), .TextAlign = ContentAlignment.MiddleCenter}

        pnl.Controls.AddRange(New Control() {lblNome, lblPreco})

        Dim yRecurso As Integer = 120
        For Each recurso In recursos
            Dim lblCheck As New Label() With {.Text = "✓", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .ForeColor = corDestaque, .Location = New Point(25, yRecurso), .AutoSize = True}
            Dim lblRec As New Label() With {.Text = recurso, .Font = New Font("Segoe UI", 10), .ForeColor = COR_TEXTO_MUTED, .Location = New Point(50, yRecurso), .Size = New Size(210, 20)}
            pnl.Controls.AddRange(New Control() {lblCheck, lblRec})
            yRecurso += 35
        Next

        ' Lógica Inteligente do Botão
        If _planoAtual = nomePlano Then
            Dim lblSeloAtivo As New Label() With {.Text = "✅ Licença Ativa", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .ForeColor = COR_VERDE, .Location = New Point(0, 285), .Size = New Size(275, 40), .TextAlign = ContentAlignment.MiddleCenter}
            pnl.Controls.Add(lblSeloAtivo)
        Else
            Dim btnAction As New Button() With {.Text = If(nomePlano = "ESSENTIAL", "REBAIXAR PLANO", "FAZER UPGRADE"), .Location = New Point(25, 285), .Size = New Size(225, 40), .Font = New Font("Segoe UI", 9.5, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
            btnAction.FlatAppearance.BorderSize = 0

            If nomePlano = "ESSENTIAL" Then
                btnAction.BackColor = COR_BTN_DARK : btnAction.ForeColor = COR_TEXTO_MUTED
            Else
                btnAction.BackColor = corDestaque : btnAction.ForeColor = Color.FromArgb(15, 17, 21)
            End If

            btnAction.Tag = nomePlano
            AddHandler btnAction.Paint, AddressOf ArredondarBotaoGDI
            AddHandler btnAction.Click, AddressOf btnAssinar_Click
            pnl.Controls.Add(btnAction)
        End If

        Return pnl
    End Function

    Private Sub btnAssinar_Click(sender As Object, e As EventArgs)
        Dim btn = DirectCast(sender, Button)
        Dim planoSelecionado As String = btn.Tag.ToString().Trim().ToUpper()

        If planoSelecionado = "ESSENTIAL" Then
            If MessageBox.Show("Deseja realmente rebaixar sua licença para a versão gratuita? Alguns recursos como o Executive BI serão bloqueados.", "Confirmação de Downgrade", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                Using db As New AppDbContext()
                    Dim u = db.Usuarios.FirstOrDefault(Function(user) user.Id = _usuarioIdLogado)
                    If u IsNot Nothing Then
                        Try
                            CallByName(u, "CategoriaPlano", CallType.Set, "ESSENTIAL")
                            db.SaveChanges()
                            _planoAtual = "ESSENTIAL"
                            Me.Controls.Clear()
                            MontarLayoutPlanos()
                            MessageBox.Show("Licença rebaixada para Essential.", "Flow Road", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Catch : End Try
                    End If
                End Using
            End If
            Return
        End If

        Dim preco As Decimal = If(planoSelecionado = "PERFORMANCE", 14.9D, 29.9D)
        Dim telaCheckout As New FormCheckoutPix(planoSelecionado, preco)

        Me.Hide()
        telaCheckout.ShowDialog()

        ObterPlanoAtualDoBanco()
        Me.Controls.Clear()
        MontarLayoutPlanos()
        Me.Show()
    End Sub

    Private Sub EstilizarBordaPainelGlass(sender As Object, e As PaintEventArgs)
        Dim pnl = DirectCast(sender, Panel)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Using p As New Pen(Color.FromArgb(51, 65, 85), 1.2F) : e.Graphics.DrawRectangle(p, 0, 0, pnl.Width - 1, pnl.Height - 1) : End Using
    End Sub

    Private Sub ArredondarBotaoGDI(sender As Object, e As PaintEventArgs)
        Dim btn = DirectCast(sender, Button)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear

        Dim r As Integer = 6
        Dim path As New GraphicsPath()

        Dim w As Integer = btn.Width - 1
        Dim h As Integer = btn.Height - 1

        path.AddArc(0, 0, r * 2, r * 2, 180, 90)
        path.AddArc(w - (r * 2), 0, r * 2, r * 2, 270, 90)
        path.AddArc(w - (r * 2), h - (r * 2), r * 2, r * 2, 0, 90)
        path.AddArc(0, h - (r * 2), r * 2, r * 2, 90, 90)
        path.CloseAllFigures()

        btn.Region = New Region(path)

        Using b As New SolidBrush(btn.BackColor)
            e.Graphics.FillPath(b, path)
        End Using

        Using p As New Pen(btn.BackColor, 1.0F)
            e.Graphics.DrawPath(p, path)
        End Using

        TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, New Rectangle(0, 0, btn.Width, btn.Height), btn.ForeColor, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
    End Sub
End Class