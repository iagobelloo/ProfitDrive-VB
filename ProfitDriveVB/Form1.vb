Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports System.Linq

Public Class Form1
    Inherits Form

    Private txtUsuario As TextBox
    Private txtSenha As TextBox
    Private btnEntrar As Button
    Private lblMensagem As Label
    Private btnVerSenhaLogin As Button
    Private pnlLoginBox As Panel

    Private COR_CANVAS As Color = Color.FromArgb(15, 17, 21)
    Private COR_CARD As Color = Color.FromArgb(26, 32, 44)
    Private COR_TEXTO_PRINCIPAL As Color = Color.FromArgb(248, 250, 252)
    Private COR_TEXTO_MUTED As Color = Color.FromArgb(148, 163, 184)
    Private COR_DESTAQUE As Color = Color.FromArgb(56, 189, 248)
    Private COR_VERDE As Color = Color.FromArgb(16, 185, 129)
    Private COR_VERM As Color = Color.FromArgb(239, 68, 68)

    Sub New()
        MyBase.New()
        Me.StartPosition = FormStartPosition.CenterScreen
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializeComponent()
        InicializarBancoEUsuario()
        ConstruirInterfacePremium()
    End Sub

    Private Sub InicializarBancoEUsuario()
        Using db As New AppDbContext()
            db.Database.EnsureCreated()

            If Not db.Usuarios.Any() Then
                Dim motoristaProvisorio As New Usuario() With {
                .Nome = "Admin",
                .CPF = "00000000000",
                .Email = "admin@flowroad.com",
                .Endereco = "",
                .Senha = "1234",
                .AvatarSelecionado = "",
                .MetaDiaria = 250D,
                .CustosFixosMensais = 0D
            }

                Try : CallByName(motoristaProvisorio, "Ativo", CallType.Set, True) : Catch : End Try
                Try : CallByName(motoristaProvisorio, "CategoriaPlano", CallType.Set, "OURO") : Catch : End Try

                db.Usuarios.Add(motoristaProvisorio)
                db.Veiculos.Add(New Veiculo() With {
                .Modelo = "Veículo Teste",
                .ConsumoCombustivel = 10D,
                .PrecoCombustivel = 5.5D,
                .ConsumoEletrico = 0D,
                .PrecoKwh = 0D
            })

                db.SaveChanges()
            End If
        End Using
    End Sub

    Private Sub ConstruirInterfacePremium()
        Me.Text = "Flow Road - Autenticação"
        Me.Size = New Size(400, 480)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.BackColor = COR_CANVAS

        pnlLoginBox = New Panel() With {.Location = New Point(35, 30), .Size = New Size(315, 380), .BackColor = COR_CARD}
        AddHandler pnlLoginBox.Paint, Sub(s, e)
                                          e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
                                          Using p As New Pen(Color.FromArgb(51, 65, 85), 1) : e.Graphics.DrawRectangle(p, 0, 0, pnlLoginBox.Width - 1, pnlLoginBox.Height - 1) : End Using
                                      End Sub

        Dim lblTitulo As New Label() With {.Location = New Point(10, 25), .Size = New Size(295, 35), .BackColor = Color.Transparent}
        AddHandler lblTitulo.Paint, Sub(s, e)
                                        Dim g As Graphics = e.Graphics
                                        g.SmoothingMode = SmoothingMode.AntiAlias

                                        Dim fonteTitulo As New Font("Segoe UI", 18, FontStyle.Bold)
                                        Dim pincelBranco As New SolidBrush(COR_TEXTO_PRINCIPAL)
                                        Dim pincelCiano As New SolidBrush(COR_DESTAQUE)

                                        Dim parte1 As String = "FLOW "
                                        Dim parte2 As String = "ROAD"

                                        Dim tamanhoParte1 As SizeF = g.MeasureString(parte1, fonteTitulo)
                                        Dim larguraTotal As Single = tamanhoParte1.Width + g.MeasureString(parte2, fonteTitulo).Width
                                        Dim posXInicial As Single = (lblTitulo.Width - larguraTotal) / 2

                                        g.DrawString(parte1, fonteTitulo, pincelBranco, posXInicial, 0)
                                        g.DrawString(parte2, fonteTitulo, pincelCiano, posXInicial + tamanhoParte1.Width - 5, 0)
                                    End Sub

        Dim lblUser As New Label() With {.Text = "Usuário ou E-mail", .Location = New Point(30, 85), .Size = New Size(255, 20), .ForeColor = COR_TEXTO_MUTED, .Font = New Font("Segoe UI", 9), .BackColor = Color.Transparent}
        txtUsuario = New TextBox() With {.Location = New Point(30, 105), .Size = New Size(255, 25), .BackColor = Color.FromArgb(15, 17, 21), .ForeColor = COR_TEXTO_PRINCIPAL, .BorderStyle = BorderStyle.FixedSingle, .Font = New Font("Segoe UI", 10)}

        Dim lblPass As New Label() With {.Text = "Senha", .Location = New Point(30, 145), .Size = New Size(255, 20), .ForeColor = COR_TEXTO_MUTED, .Font = New Font("Segoe UI", 9), .BackColor = Color.Transparent}
        txtSenha = New TextBox() With {.Location = New Point(30, 165), .Size = New Size(215, 25), .BackColor = Color.FromArgb(15, 17, 21), .ForeColor = COR_TEXTO_PRINCIPAL, .BorderStyle = BorderStyle.FixedSingle, .Font = New Font("Segoe UI", 10), .PasswordChar = "*"c}

        btnVerSenhaLogin = New Button() With {.Text = "👁️", .Location = New Point(250, 164), .Size = New Size(35, 27), .BackColor = Color.FromArgb(51, 65, 85), .ForeColor = COR_TEXTO_PRINCIPAL, .Cursor = Cursors.Hand, .FlatStyle = FlatStyle.Flat}
        btnVerSenhaLogin.FlatAppearance.BorderSize = 0
        AddHandler btnVerSenhaLogin.Click, Sub()
                                               If txtSenha.PasswordChar = "*"c Then
                                                   txtSenha.PasswordChar = ControlChars.NullChar : btnVerSenhaLogin.Text = "🔒"
                                               Else
                                                   txtSenha.PasswordChar = "*"c : btnVerSenhaLogin.Text = "👁️"
                                               End If
                                           End Sub

        btnEntrar = New Button() With {.Text = "Acessar Plataforma", .Location = New Point(30, 220), .Size = New Size(255, 40), .BackColor = COR_VERDE, .ForeColor = Color.White, .Font = New Font("Segoe UI", 10, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnEntrar.FlatAppearance.BorderSize = 0
        AddHandler btnEntrar.Paint, AddressOf ArredondarBotaoGDI
        AddHandler btnEntrar.Click, AddressOf btnEntrar_Click
        Me.AcceptButton = btnEntrar

        Dim lnkCriarConta As New LinkLabel() With {.Text = "Não tem uma conta? Cadastre-se aqui", .Location = New Point(30, 280), .Size = New Size(255, 20), .TextAlign = ContentAlignment.MiddleCenter, .LinkColor = COR_DESTAQUE, .ActiveLinkColor = COR_VERDE, .Font = New Font("Segoe UI", 9), .BackColor = Color.Transparent, .LinkBehavior = LinkBehavior.HoverUnderline}
        AddHandler lnkCriarConta.LinkClicked, AddressOf lnkCriarConta_LinkClicked

        lblMensagem = New Label() With {.Text = "Insira suas credenciais operacionais.", .Location = New Point(10, 325), .Size = New Size(295, 30), .TextAlign = ContentAlignment.MiddleCenter, .ForeColor = COR_TEXTO_MUTED, .Font = New Font("Segoe UI", 9, FontStyle.Italic), .BackColor = Color.Transparent}

        pnlLoginBox.Controls.AddRange(New Control() {lblTitulo, lblUser, txtUsuario, lblPass, txtSenha, btnVerSenhaLogin, btnEntrar, lnkCriarConta, lblMensagem})
        Me.Controls.Add(pnlLoginBox)
    End Sub

    Private Sub ArredondarBotaoGDI(sender As Object, e As PaintEventArgs)
        Dim btn = DirectCast(sender, Button)
        e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        Dim r As Integer = 6
        Dim path As New Drawing2D.GraphicsPath()
        path.AddArc(0, 0, r * 2, r * 2, 180, 90) : path.AddArc(btn.Width - (r * 2) - 1, 0, r * 2, r * 2, 270, 90)
        path.AddArc(btn.Width - (r * 2) - 1, btn.Height - (r * 2) - 1, r * 2, r * 2, 0, 90) : path.AddArc(0, btn.Height - (r * 2) - 1, r * 2, r * 2, 90, 90)
        path.CloseAllFigures()
        btn.Region = New Region(path)
        Using b As New SolidBrush(btn.BackColor) : e.Graphics.FillPath(b, path) : End Using
        TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, New Rectangle(0, 0, btn.Width, btn.Height), btn.ForeColor, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
    End Sub

    Private Sub btnEntrar_Click(sender As Object, e As EventArgs)
        If txtUsuario.Text.Trim().ToLower() = "adm" AndAlso txtSenha.Text = "adm" Then
            Me.Hide() : Dim telaAdmin As New FormAdminUsuarios(0) : telaAdmin.ShowDialog() : txtUsuario.Clear() : txtSenha.Clear() : Me.Show() : Exit Sub
        End If

        Dim nomeDigitado = txtUsuario.Text.Trim() : Dim senhaDigitada = txtSenha.Text
        If String.IsNullOrEmpty(nomeDigitado) OrElse String.IsNullOrEmpty(senhaDigitada) Then
            lblMensagem.Text = "Preencha todos os campos!" : lblMensagem.ForeColor = COR_VERM : Return
        End If

        Using db As New AppDbContext()
            Dim contaValida = db.Usuarios.FirstOrDefault(Function(u) (u.Nome.ToLower() = nomeDigitado.ToLower() OrElse u.Email.ToLower() = nomeDigitado.ToLower()) AndAlso u.Senha = senhaDigitada)

            If contaValida IsNot Nothing Then
                Dim usuarioAtivo As Boolean = True
                Try : usuarioAtivo = CallByName(contaValida, "Ativo", CallType.Get) : Catch : End Try
                If Not usuarioAtivo Then
                    lblMensagem.Text = "Acesso suspenso pelo Administrador." : lblMensagem.ForeColor = COR_VERM : Return
                End If

                lblMensagem.Text = "Autenticando..." : lblMensagem.ForeColor = COR_VERDE : Me.Refresh()
                System.Threading.Thread.Sleep(300)
                Me.Hide() : Dim menuCentral As New FormMenu(contaValida.Id) : menuCentral.ShowDialog() : Me.Close()
            Else
                lblMensagem.Text = "Credenciais incorretas!" : lblMensagem.ForeColor = COR_VERM
            End If
        End Using
    End Sub

    Private Sub lnkCriarConta_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
        Me.Hide() : Dim telaCadastro As New FormCriarConta() : telaCadastro.ShowDialog() : Me.Show()
    End Sub
End Class