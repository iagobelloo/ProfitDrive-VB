Imports System
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Globalization
Imports System.Linq
Imports System.Windows.Forms
Imports Microsoft.EntityFrameworkCore

Public Class FormMenu
    Inherits Form

    Private _usuarioIdLogado As Integer
    Private lblStatusPiloto, lblPlanoEmblema, lblGanhoValor, lblMetaValor As Label
    Private pnlKPIsMenu, pnlSidebarMenu, pnlHUD As Panel
    Private lblCronometro As Label
    Private btnStartTimer, btnStopTimer, btnResetTimer As Button
    Private MenuTimer As Timer

    Private lblVeiculoAtivo, lblKmsHoje, lblGastoPostoHoje, lblLucroHoje, lblMediaRealHoje, lblCustoPorKmHoje, lblManutencao, lblStatusBanco As Label
    Private _tempoDecorrido As TimeSpan = TimeSpan.Zero
    Private _cronometroAtivo As Boolean = False
    Private _licencaAtualUsuario As String

    Private _nomeUsuarioLogado As String = "Operador"

    Private COR_CANVAS As Color = Color.FromArgb(15, 17, 21)
    Private COR_CARD As Color = Color.FromArgb(26, 32, 44)
    Private COR_SIDEBAR As Color = Color.FromArgb(18, 22, 29)
    Private COR_TEXTO_PRINCIPAL As Color = Color.FromArgb(248, 250, 252)
    Private COR_TEXTO_MUTED As Color = Color.FromArgb(148, 163, 184)
    Private COR_DESTAQUE As Color = Color.FromArgb(56, 189, 248)
    Private COR_VERDE As Color = Color.FromArgb(16, 185, 129)
    Private COR_VERM As Color = Color.FromArgb(239, 68, 68)
    Private COR_SIDEBAR_LINHA As Color = Color.FromArgb(30, 41, 59)
    Private COR_BTN_DARK As Color = Color.FromArgb(30, 41, 59)

    Sub New(usuarioId As Integer)
        MyBase.New()
        Me._usuarioIdLogado = usuarioId

        Me.Text = "Flow Road - Cockpit Central"
        Me.Size = New Size(960, 665)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.BackColor = COR_CANVAS

        MontarLayoutPremium()
        InitializeTimerVolante()
        AtualizarDadosDashboard()
    End Sub

    Private Sub InitializeTimerVolante()
        MenuTimer = New Timer() With {.Interval = 1000, .Enabled = True}
        AddHandler MenuTimer.Tick, Sub()
                                       If _cronometroAtivo Then
                                           _tempoDecorrido = _tempoDecorrido.Add(TimeSpan.FromSeconds(1))
                                           lblCronometro.Text = _tempoDecorrido.ToString("hh\:mm\:ss")
                                       End If
                                   End Sub
    End Sub

    Private Sub AbrirCadastroPerfil(sender As Object, e As EventArgs)
        Me.Hide() : Using frm As New FormCadastroPerfil(_usuarioIdLogado) : frm.ShowDialog() : End Using : Me.Show() : AtualizarDadosDashboard()
    End Sub
    Private Sub AbrirControleDespesas(sender As Object, e As EventArgs)
        Me.Hide() : Using frm As New FormFinancasGerais(_usuarioIdLogado) : frm.ShowDialog() : End Using : Me.Show() : AtualizarDadosDashboard()
    End Sub
    Private Sub AbrirViagensJornadas(sender As Object, e As EventArgs)
        Me.Hide() : Using frm As New FormModuloDrive(_usuarioIdLogado) : frm.ShowDialog() : End Using : Me.Show() : AtualizarDadosDashboard()
    End Sub

    Private Sub AbrirBusinessIntelligence(sender As Object, e As EventArgs)
        If Not String.Equals(_licencaAtualUsuario, "BLACK", StringComparison.OrdinalIgnoreCase) Then
            MessageBox.Show($"Recurso Exclusivo! 👑{vbCrLf}{vbCrLf}O ecossistema avançado de Business Intelligence (BI) e a geração de relatórios contábeis executivos requerem o upgrade para o Plano Black.", "Recurso Restrito", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Me.Hide() : Using frm As New FormDashboardGraf(_usuarioIdLogado) : frm.ShowDialog() : End Using : Me.Show() : AtualizarDadosDashboard()
    End Sub

    Private Sub AbrirTelaPlanos(sender As Object, e As EventArgs)
        Using frm As New FormPlanos(_usuarioIdLogado) : frm.ShowDialog() : End Using : AtualizarDadosDashboard() : Me.Refresh()
    End Sub

    Private Sub MontarLayoutPremium()
        ' SIDEBAR
        pnlSidebarMenu = New Panel() With {.Location = New Point(0, 0), .Size = New Size(265, 630), .BackColor = COR_SIDEBAR}
        AddHandler pnlSidebarMenu.Paint, Sub(s, e)
                                             Using p As New Pen(Color.FromArgb(30, 41, 59), 1) : e.Graphics.DrawLine(p, pnlSidebarMenu.Width - 1, 0, pnlSidebarMenu.Width - 1, pnlSidebarMenu.Height) : End Using
                                         End Sub

        'TÍTULO BICOLOR NA SIDEBAR
        Dim lblAppName As New Label() With {
            .Location = New Point(15, 25),
            .Size = New Size(230, 35),
            .BackColor = Color.Transparent
        }

        AddHandler lblAppName.Paint, Sub(s As Object, e As PaintEventArgs)
                                         Dim g As Graphics = e.Graphics
                                         g.SmoothingMode = SmoothingMode.AntiAlias

                                         Dim fonteSidebar As New Font("Segoe UI", 18, FontStyle.Bold)
                                         Dim pincelBranco As New SolidBrush(COR_TEXTO_PRINCIPAL)
                                         Dim pincelCiano As New SolidBrush(COR_DESTAQUE)

                                         Dim parte1 As String = "FLOW "
                                         Dim parte2 As String = "ROAD"

                                         Dim tamanhoParte1 As SizeF = g.MeasureString(parte1, fonteSidebar)

                                         g.DrawString(parte1, fonteSidebar, pincelBranco, 0, 0)
                                         g.DrawString(parte2, fonteSidebar, pincelCiano, tamanhoParte1.Width - 5, 0)
                                     End Sub
        Dim lblSeparador As New Label() With {.Location = New Point(15, 70), .Size = New Size(230, 1), .BackColor = Color.FromArgb(30, 41, 59)}

        lblStatusPiloto = New Label() With {.Font = New Font("Segoe UI", 11, FontStyle.Bold), .Location = New Point(15, 85), .Size = New Size(230, 25), .ForeColor = COR_TEXTO_MUTED}
        lblPlanoEmblema = New Label() With {.Font = New Font("Segoe UI", 10, FontStyle.Bold), .Location = New Point(15, 110), .Size = New Size(230, 25)}
        Dim lblSeparador2 As New Label() With {.Location = New Point(15, 145), .Size = New Size(230, 1), .BackColor = Color.FromArgb(30, 41, 59)}

        Dim lblHardwareTitulo As New Label() With {.Text = "CRONÔMETRO DE TURNO", .Font = New Font("Segoe UI", 8.5, FontStyle.Bold), .Location = New Point(15, 160), .Size = New Size(230, 20), .ForeColor = COR_TEXTO_MUTED}
        lblCronometro = New Label() With {.Text = "00:00:00", .Font = New Font("Consolas", 26, FontStyle.Bold), .Location = New Point(10, 180), .Size = New Size(230, 45), .ForeColor = COR_TEXTO_PRINCIPAL, .TextAlign = ContentAlignment.MiddleCenter}

        Dim btnStyle = Sub(btn As Button, cor As Color)
                           btn.FlatStyle = FlatStyle.Flat : btn.FlatAppearance.BorderSize = 0 : btn.BackColor = COR_BTN_DARK : btn.ForeColor = cor : btn.Font = New Font("Segoe UI", 11, FontStyle.Bold) : btn.Cursor = Cursors.Hand
                       End Sub
        btnStartTimer = New Button() With {.Text = "▶", .Location = New Point(25, 235), .Size = New Size(65, 35)} : btnStyle(btnStartTimer, COR_VERDE)
        AddHandler btnStartTimer.Click, Sub() _cronometroAtivo = True
        btnStopTimer = New Button() With {.Text = "⏸", .Location = New Point(100, 235), .Size = New Size(65, 35)} : btnStyle(btnStopTimer, COR_VERM)
        AddHandler btnStopTimer.Click, Sub() _cronometroAtivo = False
        btnResetTimer = New Button() With {.Text = "🔄", .Location = New Point(175, 235), .Size = New Size(65, 35)} : btnStyle(btnResetTimer, COR_TEXTO_MUTED)
        AddHandler btnResetTimer.Click, Sub()
                                            _cronometroAtivo = False
                                            _tempoDecorrido = TimeSpan.Zero
                                            lblCronometro.Text = "00:00:00"
                                        End Sub

        pnlHUD = New Panel() With {.Location = New Point(15, 285), .Size = New Size(230, 270), .BackColor = COR_CARD}
        AddHandler pnlHUD.Paint, Sub(s, e)
                                     e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
                                     Using p As New Pen(Color.FromArgb(51, 65, 85), 1) : e.Graphics.DrawRectangle(p, 0, 0, pnlHUD.Width - 1, pnlHUD.Height - 1) : End Using
                                 End Sub

        lblVeiculoAtivo = New Label() With {.Text = "Buscando...", .Font = New Font("Segoe UI", 9.5, FontStyle.Bold), .Location = New Point(10, 12), .Size = New Size(210, 20), .ForeColor = COR_DESTAQUE, .TextAlign = ContentAlignment.MiddleCenter}
        Dim linhaHUD As New Label() With {.Location = New Point(10, 38), .Size = New Size(210, 1), .BackColor = Color.FromArgb(51, 65, 85)}

        Dim yHUD As Integer = 82
        lblKmsHoje = CriarLabelHUD("Rodados Hoje:", "0,0 KM", yHUD, COR_TEXTO_PRINCIPAL) : yHUD += 25
        lblGastoPostoHoje = CriarLabelHUD("Gasto Abast.:", "R$ 0,00", yHUD, COR_VERM) : yHUD += 25
        lblLucroHoje = CriarLabelHUD("Receita Livre:", "R$ 0,00", yHUD, COR_VERDE) : yHUD += 25
        lblMediaRealHoje = CriarLabelHUD("Média Real:", "0,0 KM/L", yHUD, COR_TEXTO_PRINCIPAL) : yHUD += 25
        lblCustoPorKmHoje = CriarLabelHUD("Custo/KM:", "R$ 0,00", yHUD, COR_TEXTO_MUTED) : yHUD += 25

        Dim linhaManutencao As New Label() With {.Location = New Point(10, yHUD), .Size = New Size(210, 1), .BackColor = Color.FromArgb(51, 65, 85)} : yHUD += 14
        lblManutencao = CriarLabelHUD("Troca de Óleo:", "10.000 KM", yHUD, Color.FromArgb(234, 179, 8))

        pnlHUD.Controls.AddRange(New Control() {lblVeiculoAtivo, linhaHUD, lblKmsHoje, lblGastoPostoHoje, lblLucroHoje, lblMediaRealHoje, lblCustoPorKmHoje, linhaManutencao, lblManutencao})

        Dim btnLogoff As New Button() With {.Text = "Encerrar Sessão", .Location = New Point(15, 570), .Size = New Size(230, 42), .BackColor = COR_BTN_DARK, .ForeColor = COR_TEXTO_MUTED, .Font = New Font("Segoe UI", 9.5, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnLogoff.FlatAppearance.BorderSize = 0
        AddHandler btnLogoff.Click, Sub()
                                        Me.Hide()
                                        Dim telaLogin As New Form1()
                                        telaLogin.ShowDialog()
                                        Me.Close()
                                    End Sub

        pnlSidebarMenu.Controls.AddRange(New Control() {lblAppName, lblSeparador, lblStatusPiloto, lblPlanoEmblema, lblSeparador2, lblHardwareTitulo, lblCronometro, btnStartTimer, btnStopTimer, btnResetTimer, pnlHUD, btnLogoff})
        Me.Controls.Add(pnlSidebarMenu)

        'PAINEL FINANCEIRO CENTRAL
        pnlKPIsMenu = New Panel() With {.Location = New Point(295, 20), .Size = New Size(620, 85), .BackColor = COR_CARD}
        AddHandler pnlKPIsMenu.Paint, Sub(s, e)
                                          e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
                                          Using p As New Pen(Color.FromArgb(51, 65, 85), 1) : e.Graphics.DrawRectangle(p, 0, 0, pnlKPIsMenu.Width - 1, pnlKPIsMenu.Height - 1) : End Using
                                      End Sub

        Dim lblGanhoTitulo As New Label() With {.Text = "FLUXO LÍQUIDO MENSAL", .Font = New Font("Segoe UI", 8.5, FontStyle.Bold), .Location = New Point(20, 15), .Size = New Size(260, 18), .ForeColor = COR_TEXTO_MUTED, .TextAlign = ContentAlignment.MiddleLeft}
        lblGanhoValor = New Label() With {.Text = "R$ 0,00", .Font = New Font("Segoe UI", 20, FontStyle.Bold), .Location = New Point(18, 35), .Size = New Size(260, 35), .ForeColor = COR_TEXTO_PRINCIPAL, .TextAlign = ContentAlignment.MiddleLeft}

        Dim lblMetaTitulo As New Label() With {.Text = "META FINANCEIRA DIÁRIA", .Font = New Font("Segoe UI", 8.5, FontStyle.Bold), .Location = New Point(330, 15), .Size = New Size(270, 18), .ForeColor = COR_TEXTO_MUTED, .TextAlign = ContentAlignment.MiddleRight}
        lblMetaValor = New Label() With {.Text = "Falta R$ 0,00", .Font = New Font("Segoe UI", 20, FontStyle.Bold), .Location = New Point(330, 35), .Size = New Size(270, 35), .ForeColor = COR_TEXTO_PRINCIPAL, .TextAlign = ContentAlignment.MiddleRight}
        pnlKPIsMenu.Controls.AddRange(New Control() {lblGanhoTitulo, lblGanhoValor, lblMetaTitulo, lblMetaValor})
        Me.Controls.Add(pnlKPIsMenu)

        CriarCardModulo("Central do Piloto", "Gerencie dados da conta, CNH e documentação.", 295, 130, 295, 120, AddressOf AbrirCadastroPerfil)
        CriarCardModulo("Gestão Financeira", "Controle de despesas fixas e fluxo de caixa.", 620, 130, 295, 120, AddressOf AbrirControleDespesas)
        CriarCardModulo("Histórico de Turnos", "Auditoria de quilometragem e plataformas.", 295, 270, 295, 120, AddressOf AbrirViagensJornadas)
        CriarCardModulo("Business Intelligence", "Analytics avançado e relatórios para contabilidade.", 620, 270, 295, 120, AddressOf AbrirBusinessIntelligence)

        Dim pnlAlertasAdmin As New Panel() With {.Location = New Point(295, 415), .Size = New Size(295, 197), .BackColor = COR_CARD}
        AddHandler pnlAlertasAdmin.Paint, Sub(s, e)
                                              e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
                                              Using p As New Pen(Color.FromArgb(234, 179, 8), 1) : e.Graphics.DrawRectangle(p, 0, 0, pnlAlertasAdmin.Width - 1, pnlAlertasAdmin.Height - 1) : End Using
                                          End Sub
        Dim lblAlertaTitulo As New Label() With {.Text = "🔔 NOTIFICAÇÃO DO SISTEMA", .Font = New Font("Segoe UI", 7.5, FontStyle.Bold), .Location = New Point(12, 12), .Size = New Size(270, 14), .ForeColor = Color.FromArgb(234, 179, 8)}
        lblStatusBanco = New Label() With {.Text = "Sistema operando normalmente. Sem alertas.", .Font = New Font("Segoe UI", 9, FontStyle.Italic), .Location = New Point(12, 32), .Size = New Size(270, 150), .ForeColor = COR_TEXTO_PRINCIPAL}
        pnlAlertasAdmin.Controls.AddRange(New Control() {lblAlertaTitulo, lblStatusBanco})
        Me.Controls.Add(pnlAlertasAdmin)

        CriarCardModulo("Licença Flow Road", "Verifique os benefícios do seu plano.", 620, 415, 295, 125, AddressOf AbrirTelaPlanos, True)

        Dim btnAjudaWpp As New Button() With {
            .Text = "💬 CENTRAL DE AJUDA WHATSAPP",
            .Location = New Point(620, 562),
            .Size = New Size(295, 50),
            .BackColor = COR_BTN_DARK,
            .ForeColor = Color.FromArgb(37, 211, 102),
            .Font = New Font("Segoe UI", 9.5, FontStyle.Bold),
            .FlatStyle = FlatStyle.Flat,
            .Cursor = Cursors.Hand
        }
        btnAjudaWpp.FlatAppearance.BorderSize = 0
        AddHandler btnAjudaWpp.Paint, AddressOf ArredondarBotaoGDI

        AddHandler btnAjudaWpp.MouseEnter, Sub(s As Object, ev As EventArgs)
                                               btnAjudaWpp.BackColor = Color.FromArgb(34, 197, 94)    ' Verde WhatsApp Ativo
                                               btnAjudaWpp.ForeColor = Color.FromArgb(15, 17, 21)     ' Letra Escura Contrastante
                                               btnAjudaWpp.Invalidate()
                                           End Sub

        AddHandler btnAjudaWpp.MouseLeave, Sub(s As Object, ev As EventArgs)
                                               btnAjudaWpp.BackColor = COR_BTN_DARK                    ' Retorna ao Fundo Dark
                                               btnAjudaWpp.ForeColor = Color.FromArgb(37, 211, 102)    ' Letra Verde Padrão
                                               btnAjudaWpp.Invalidate()
                                           End Sub

        AddHandler btnAjudaWpp.Click, Sub(s, e)
                                          Dim numeroSuporte As String = "5511999999999"
                                          Dim textoMensagem As String = Uri.EscapeDataString($"Olá! Sou o motorista {_nomeUsuarioLogado} e preciso de auxílio com o cockpit do Flow Road.")
                                          Try : Process.Start(New ProcessStartInfo With {.FileName = $"https://wa.me/{numeroSuporte}?text={textoMensagem}", .UseShellExecute = True}) : Catch : End Try
                                      End Sub
        Me.Controls.Add(btnAjudaWpp)
    End Sub

    Private Function CriarLabelHUD(texto As String, valor As String, y As Integer, corValor As Color) As Label
        Dim lblTitle As New Label() With {.Text = texto, .Location = New Point(10, y), .Size = New Size(100, 20), .Font = New Font("Segoe UI", 9), .ForeColor = COR_TEXTO_MUTED}
        Dim lblValue As New Label() With {.Text = valor, .Location = New Point(110, y), .Size = New Size(110, 20), .Font = New Font("Segoe UI", 9, FontStyle.Bold), .ForeColor = corValor, .TextAlign = ContentAlignment.MiddleRight}
        pnlHUD.Controls.Add(lblTitle)
        Return lblValue
    End Function

    Private Sub CriarCardModulo(titulo As String, subtexto As String, x As Integer, y As Integer, w As Integer, h As Integer, eventoClique As EventHandler, Optional isPremium As Boolean = False)
        Dim pnlCard As New Panel() With {.Location = New Point(x, y), .Size = New Size(w, h), .BackColor = COR_CARD, .Cursor = Cursors.Hand}
        AddHandler pnlCard.Paint, Sub(s, e)
                                      e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
                                      Dim corBorda = If(isPremium, Color.FromArgb(56, 189, 248), Color.FromArgb(51, 65, 85))
                                      Using p As New Pen(corBorda, 1) : e.Graphics.DrawRectangle(p, 0, 0, pnlCard.Width - 1, pnlCard.Height - 1) : End Using
                                  End Sub
        AddHandler pnlCard.Click, eventoClique

        Dim lblT As New Label() With {.Text = titulo, .Font = New Font("Segoe UI", 11, FontStyle.Bold), .Location = New Point(20, 25), .Size = New Size(w - 40, 24), .ForeColor = COR_TEXTO_PRINCIPAL, .Cursor = Cursors.Hand}
        AddHandler lblT.Click, Sub(s, e) eventoClique.Invoke(pnlCard, e)

        Dim lblS As New Label() With {.Text = subtexto, .Font = New Font("Segoe UI", 9.5, FontStyle.Regular), .Location = New Point(20, 55), .Size = New Size(w - 40, h - 65), .ForeColor = COR_TEXTO_MUTED, .Cursor = Cursors.Hand}
        AddHandler lblS.Click, Sub(s, e) eventoClique.Invoke(pnlCard, e)

        pnlCard.Controls.AddRange(New Control() {lblT, lblS})

        Dim acaoEnter = Sub() pnlCard.BackColor = Color.FromArgb(30, 41, 59)
        Dim acaoLeave = Sub() pnlCard.BackColor = COR_CARD
        AddHandler pnlCard.MouseEnter, acaoEnter : AddHandler lblT.MouseEnter, acaoEnter : AddHandler lblS.MouseEnter, acaoEnter
        AddHandler pnlCard.MouseLeave, acaoLeave : AddHandler lblT.MouseLeave, acaoLeave : AddHandler lblS.MouseLeave, acaoLeave

        Me.Controls.Add(pnlCard)
    End Sub

    Private Sub ArredondarBotaoGDI(sender As Object, e As PaintEventArgs)
        Dim btn = DirectCast(sender, Button)
        e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        Dim r As Integer = 6
        Dim path As New Drawing2D.GraphicsPath()
        path.AddArc(0, 0, r * 2, r * 2, 180, 90) : path.AddArc(btn.Width - (r * 2) - 1, 0, r * 2, r * 2, 270, 90)
        path.AddArc(btn.Width - (r * 2) - 1, btn.Height - (r * 2) - 1, r * 2, r * 2, 0, 90) : path.AddArc(0, btn.Height - (r * 2) - 1, r * 2, r * 2, 90, 90)
        path.CloseAllFigures()
        Using b As New SolidBrush(btn.BackColor) : e.Graphics.FillPath(b, path) : End Using
        TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, New Rectangle(0, 0, btn.Width, btn.Height), btn.ForeColor, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
    End Sub

    Public Sub UpdateWppName(name As String)
        _nomeUsuarioLogado = name
    End Sub

    Public Sub AtualizarDadosDashboard()
        Using db As New AppDbContext()
            Dim u = db.Usuarios.AsNoTracking().FirstOrDefault(Function(user) user.Id = _usuarioIdLogado)
            If u IsNot Nothing Then
                Dim plano As String = ""
                Dim planoBanco As String = ""
                Try : planoBanco = CallByName(u, "CategoriaPlano", CallType.Get).ToString().Trim().ToUpper() : Catch : End Try

                Select Case planoBanco
                    Case "BRONZE", "ESSENTIAL" : plano = "ESSENTIAL"
                    Case "PRATA", "PERFORMANCE" : plano = "PERFORMANCE"
                    Case "OURO", "BLACK" : plano = "BLACK"
                    Case Else : plano = "ESSENTIAL"
                End Select

                Try
                                Dim dataVencimentoObj = CallByName(u, "DataVencimentoPlano", CallType.Get)
                                If dataVencimentoObj IsNot Nothing Then
                                    Dim dataVencimento As DateTime = Convert.ToDateTime(dataVencimentoObj)

                                    If Not String.Equals(plano, "ESSENTIAL", StringComparison.OrdinalIgnoreCase) AndAlso dataVencimento.Date < DateTime.Today Then
                                        CallByName(u, "CategoriaPlano", CallType.Set, "ESSENTIAL")
                                        db.Usuarios.Update(u)
                                        db.SaveChanges()
                                        plano = "ESSENTIAL"
                                        MessageBox.Show("Sua assinatura expirou. Sua conta foi rebaixada para a licença Essential.", "Aviso Flow Road", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                    End If
                                End If

                                Dim msgAdminObj = CallByName(u, "MensagemAdmin", CallType.Get)
                                If msgAdminObj IsNot Nothing AndAlso Not String.IsNullOrEmpty(msgAdminObj.ToString().Trim()) Then
                                    lblStatusBanco.Text = msgAdminObj.ToString().Trim()
                                    lblStatusBanco.Font = New Font("Segoe UI", 9, FontStyle.Bold)
                                Else
                                    lblStatusBanco.Text = "Sistema operando normalmente. Sem alertas."
                                    lblStatusBanco.Font = New Font("Segoe UI", 9, FontStyle.Italic)
                                End If
                            Catch
                            End Try

                            _licencaAtualUsuario = plano
                            _nomeUsuarioLogado = u.Nome
                            lblStatusPiloto.Text = $"Olá, {u.Nome}"
                            lblPlanoEmblema.Text = $"Plano {plano}"

                            Select Case True
                                Case String.Equals(plano, "BLACK", StringComparison.OrdinalIgnoreCase)
                                    lblPlanoEmblema.Text = "Plano Black 👑"
                                    lblPlanoEmblema.ForeColor = Color.FromArgb(234, 179, 8)
                                Case String.Equals(plano, "PERFORMANCE", StringComparison.OrdinalIgnoreCase)
                                    lblPlanoEmblema.Text = "Plano Performance ⚡"
                                    lblPlanoEmblema.ForeColor = COR_DESTAQUE
                                Case Else
                                    lblPlanoEmblema.Text = "Plano Essential 🥉"
                                    lblPlanoEmblema.ForeColor = Color.FromArgb(203, 213, 225)
                            End Select

                If String.Equals(plano, "ESSENTIAL", StringComparison.OrdinalIgnoreCase) Then
                    lblVeiculoAtivo.Text = "Módulo Bloqueado"
                    lblKmsHoje.Text = "Upgrade ⚡"
                    lblGastoPostoHoje.Text = "Upgrade ⚡"
                    lblLucroHoje.Text = "Upgrade ⚡"
                    lblMediaRealHoje.Text = "Upgrade ⚡"
                    lblCustoPorKmHoje.Text = "Upgrade ⚡"
                    lblManutencao.Text = "Requer Plano Performance"
                    lblManutencao.ForeColor = COR_TEXTO_MUTED
                    lblGastoPostoHoje.ForeColor = COR_TEXTO_MUTED
                    lblLucroHoje.ForeColor = COR_TEXTO_MUTED
                Else
                    Dim nomeModelo As String = "Polo 1.0 MPI Manual"
                    Dim isEletrico As Boolean = False
                    Dim precoUnidade As Decimal = 5.5D

                    Try
                        Dim veiculoAtivoEncontrado = db.Veiculos.AsNoTracking().FirstOrDefault(Function(car) car.UsuarioId = _usuarioIdLogado AndAlso car.Ativo = True)

                        If veiculoAtivoEncontrado IsNot Nothing Then
                            nomeModelo = veiculoAtivoEncontrado.Modelo

                            isEletrico = veiculoAtivoEncontrado.TipoVeiculo.ToLower().Contains("elét") OrElse veiculoAtivoEncontrado.TipoVeiculo.ToLower().Contains("híbr")
                            precoUnidade = Convert.ToDecimal(If(isEletrico, veiculoAtivoEncontrado.PrecoKwh, veiculoAtivoEncontrado.PrecoCombustivel))
                        End If
                    Catch ex As Exception
                    End Try

                    lblVeiculoAtivo.Text = nomeModelo

                    Dim totalKmHoje As Decimal = 0D
                    Dim totalGastoHoje As Decimal = 0D
                    Dim faturamentoHoje As Decimal = 0D
                    Dim totalKmHistorico As Decimal = 0D

                    Try
                        Dim todosLancamentos = db.Lancamentos.AsNoTracking().Where(Function(l) l.UsuarioId = _usuarioIdLogado).ToList()
                        If todosLancamentos.Any() Then
                            totalKmHistorico = todosLancamentos.Sum(Function(l) l.KmRodados)
                            Dim lancamentosHoje = todosLancamentos.Where(Function(l) l.Data.Date = DateTime.Today).ToList()
                            totalKmHoje = lancamentosHoje.Sum(Function(l) l.KmRodados)
                            totalGastoHoje = lancamentosHoje.Sum(Function(l) l.ValorCombustivel)
                            faturamentoHoje = lancamentosHoje.Sum(Function(l) Math.Abs(l.ValorBruto))
                        End If
                    Catch
                    End Try

                    Dim unMedida As String = If(isEletrico, "kWh", "L")
                    lblKmsHoje.Text = $"{totalKmHoje:F1} KM"
                    lblGastoPostoHoje.Text = $"{totalGastoHoje:C2}"
                    lblGastoPostoHoje.ForeColor = COR_VERM

                    Dim lucroLivreHoje As Decimal = faturamentoHoje - totalGastoHoje
                    lblLucroHoje.Text = $"{lucroLivreHoje:C2}"
                    lblLucroHoje.ForeColor = If(lucroLivreHoje >= 0, COR_VERDE, COR_VERM)

                    Dim mediaReal As Decimal = If(precoUnidade > 0 AndAlso totalGastoHoje > 0, totalKmHoje / (totalGastoHoje / precoUnidade), 0D)
                    Dim custoPorKm As Decimal = If(totalKmHoje > 0, totalGastoHoje / totalKmHoje, 0D)

                    lblMediaRealHoje.Text = If(mediaReal > 0, $"{mediaReal:F1} KM/{unMedida}", "-")
                    lblCustoPorKmHoje.Text = $"{custoPorKm:C2}"

                    Dim faltaParaTroca As Decimal = 10000D - (totalKmHistorico Mod 10000D)
                    lblManutencao.Text = $"Faltam {faltaParaTroca:N0} KM"
                    lblManutencao.ForeColor = If(faltaParaTroca < 1000D, COR_VERM, Color.FromArgb(234, 179, 8))
                End If

                'FLUXO LÍQUIDO MENSAL
                Dim fluxoLiquido As Decimal = 0D
                Try
                    Dim listaDespesas = db.Set(Of Despesa)().AsNoTracking().Where(Function(d) d.UsuarioId = _usuarioIdLogado).ToList()
                    For Each d In listaDespesas
                                    If If(d.Descricao, "").StartsWith("[RECEITA]") OrElse If(d.Categoria, "").ToLower().Contains("faturamento") Then
                                        fluxoLiquido += Math.Abs(d.Valor)
                                    Else
                                        fluxoLiquido -= Math.Abs(d.Valor)
                                    End If
                                Next
                            Catch
                            End Try

                            lblGanhoValor.Text = fluxoLiquido.ToString("C2")
                            lblGanhoValor.ForeColor = If(fluxoLiquido >= 0, COR_VERDE, COR_VERM)

                Dim faturamentoHojeMeta As Decimal = 0D
                Try
                    Dim lancamentosDespesasHoje = db.Set(Of Despesa)() _
                                                    .AsNoTracking() _
                                                    .Where(Function(d) d.UsuarioId = _usuarioIdLogado AndAlso d.Data.Date = DateTime.Today) _
                                                    .ToList()

                    For Each d In lancamentosDespesasHoje
                        If If(d.Descricao, "").StartsWith("[RECEITA]") OrElse If(d.Categoria, "").ToLower().Contains("faturamento") Then
                            faturamentoHojeMeta += Math.Abs(d.Valor)
                        End If
                    Next
                Catch
                End Try

                Dim metaConfigurada As Decimal = u.MetaDiaria

                If faturamentoHojeMeta >= metaConfigurada Then
                    lblMetaValor.Text = "Meta Atingida! 🎉"
                    lblMetaValor.ForeColor = COR_VERDE
                Else
                    lblMetaValor.Text = $"Restam {(metaConfigurada - faturamentoHojeMeta):C2}"
                    lblMetaValor.ForeColor = COR_TEXTO_PRINCIPAL
                End If

                lblMetaValor.Refresh()
                lblGanhoValor.Refresh()
            End If
        End Using
    End Sub
End Class