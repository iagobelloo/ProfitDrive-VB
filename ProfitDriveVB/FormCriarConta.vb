Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports System.Linq
Imports System.Globalization

Public Class FormCriarConta
    Inherits Form

    Private txtNovoNome, txtCpfCnpj, txtNovoEmail, txtNovaSenha, txtConfirmaSenha, txtModCarro As TextBox
    Private txtTelefone As MaskedTextBox
    Private btnRegistrar, btnVerSenha As Button
    Private pnlPiloto, pnlVeiculo As Panel

    Private COR_CANVAS As Color = Color.FromArgb(15, 17, 21)
    Private COR_CARD As Color = Color.FromArgb(26, 32, 44)
    Private COR_TEXTO_PRINCIPAL As Color = Color.FromArgb(248, 250, 252)
    Private COR_TEXTO_MUTED As Color = Color.FromArgb(148, 163, 184)
    Private COR_DESTAQUE As Color = Color.FromArgb(56, 189, 248)
    Private COR_VERDE As Color = Color.FromArgb(16, 185, 129)

    Sub New()
        Me.Text = "Flow Road - Abertura de Conta"
        Me.Size = New Size(920, 650)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.BackColor = COR_CANVAS

        MontarLayoutSimetrico()
    End Sub

    Private Sub MontarLayoutSimetrico()
        ' HEADER 
        Dim lblLogo As New Label() With {.Text = "Flow Road", .Font = New Font("Segoe UI", 22, FontStyle.Bold), .ForeColor = COR_TEXTO_PRINCIPAL, .Location = New Point(0, 20), .Size = New Size(900, 40), .TextAlign = ContentAlignment.MiddleCenter, .BackColor = Color.Transparent}
        Dim lblSubLogo As New Label() With {.Text = "Preencha seus dados para iniciar a configuração do seu Cockpit", .Font = New Font("Segoe UI", 10, FontStyle.Regular), .ForeColor = COR_TEXTO_MUTED, .Location = New Point(0, 60), .Size = New Size(900, 20), .TextAlign = ContentAlignment.MiddleCenter, .BackColor = Color.Transparent}
        Me.Controls.AddRange(New Control() {lblLogo, lblSubLogo})

        ' PAINEL PILOTO
        pnlPiloto = New Panel() With {.Location = New Point(30, 105), .Size = New Size(415, 370), .BackColor = COR_CARD}
        AddHandler pnlPiloto.Paint, AddressOf EstilizarBordaSimplesGlass

        Dim lblTitForm As New Label() With {.Text = "Dados do Motorista", .Font = New Font("Segoe UI", 11, FontStyle.Bold), .ForeColor = COR_TEXTO_PRINCIPAL, .Location = New Point(20, 20), .AutoSize = True, .BackColor = Color.Transparent}
        Dim linha1 As New Label() With {.BackColor = Color.FromArgb(51, 65, 85), .Location = New Point(20, 50), .Size = New Size(375, 1)}

        Dim posY As Integer = 65
        Dim lblNome = CriarLabel("Nome Completo", 20, posY)
        txtNovoNome = CriarTextBox(20, posY + 20, 375)

        posY += 60
        Dim lblCPF = CriarLabel("CPF ou CNPJ", 20, posY)
        txtCpfCnpj = CriarTextBox(20, posY + 20, 180)
        txtCpfCnpj.MaxLength = 18
        AddHandler txtCpfCnpj.TextChanged, AddressOf AplicarMascaraDinamicaCpfCnpj

        Dim lblTel = CriarLabel("WhatsApp", 215, posY)
        txtTelefone = New MaskedTextBox() With {.Location = New Point(215, posY + 20), .Size = New Size(180, 25), .Mask = "(00) 00000-0000", .BackColor = Color.FromArgb(15, 17, 21), .ForeColor = COR_TEXTO_PRINCIPAL, .BorderStyle = BorderStyle.FixedSingle, .Font = New Font("Segoe UI", 10)}

        posY += 60
        Dim lblEmail = CriarLabel("E-mail de Acesso", 20, posY)
        txtNovoEmail = CriarTextBox(20, posY + 20, 375)

        posY += 60
        Dim lblSenha = CriarLabel("Criar Senha", 20, posY)
        txtNovaSenha = CriarTextBox(20, posY + 20, 140)
        txtNovaSenha.PasswordChar = "*"c

        btnVerSenha = New Button() With {.Text = "👁️", .Location = New Point(165, posY + 19), .Size = New Size(35, 27), .BackColor = Color.FromArgb(30, 41, 59), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnVerSenha.FlatAppearance.BorderSize = 0
        AddHandler btnVerSenha.Click, AddressOf AlternarVisibilidadeSenha

        Dim lblConfirma = CriarLabel("Confirmar Senha", 215, posY)
        txtConfirmaSenha = CriarTextBox(215, posY + 20, 180)
        txtConfirmaSenha.PasswordChar = "*"c

        pnlPiloto.Controls.AddRange(New Control() {lblTitForm, linha1, lblNome, txtNovoNome, lblCPF, txtCpfCnpj, lblTel, txtTelefone, lblEmail, txtNovoEmail, lblSenha, txtNovaSenha, btnVerSenha, lblConfirma, txtConfirmaSenha})

        ' PAINEL VEÍCULO
        pnlVeiculo = New Panel() With {.Location = New Point(460, 105), .Size = New Size(415, 370), .BackColor = COR_CARD}
        AddHandler pnlVeiculo.Paint, AddressOf EstilizarBordaSimplesGlass

        Dim lblVeiculoTit As New Label() With {.Text = "Veículo Principal", .Font = New Font("Segoe UI", 11, FontStyle.Bold), .ForeColor = COR_TEXTO_PRINCIPAL, .Location = New Point(20, 20), .AutoSize = True, .BackColor = Color.Transparent}
        Dim linha2 As New Label() With {.BackColor = Color.FromArgb(51, 65, 85), .Location = New Point(20, 50), .Size = New Size(375, 1)}

        Dim lblMod = CriarLabel("Modelo e Marca (Ex: Corolla, Onix, BYD)", 20, 65)
        txtModCarro = CriarTextBox(20, 85, 375)

        Dim lblInfoTexto As New Label() With {
            .Text = "Cadastro Expresso: Informe apenas o modelo base." & vbCrLf & vbCrLf &
                    "Você poderá refinar os detalhes de consumo médio (KM/L) e especificar se a motorização é Híbrida ou Elétrica diretamente na sua Central de Perfil." & vbCrLf & vbCrLf &
                    "Iniciaremos seu painel com valores de mercado padrão para garantir o funcionamento do fluxo de caixa.",
            .Location = New Point(20, 140), .Size = New Size(375, 120), .ForeColor = COR_TEXTO_MUTED, .Font = New Font("Segoe UI", 9.5, FontStyle.Regular), .BackColor = Color.Transparent, .TextAlign = ContentAlignment.TopLeft
        }

        pnlVeiculo.Controls.AddRange(New Control() {lblVeiculoTit, linha2, lblMod, txtModCarro, lblInfoTexto})

        ' RODAPÉ
        btnRegistrar = New Button() With {.Text = "Concluir Abertura de Conta", .Location = New Point(30, 500), .Size = New Size(845, 45), .BackColor = COR_VERDE, .ForeColor = Color.White, .Font = New Font("Segoe UI", 11, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnRegistrar.FlatAppearance.BorderSize = 0
        AddHandler btnRegistrar.Paint, AddressOf ArredondarBotaoGDI
        AddHandler btnRegistrar.Click, AddressOf btnRegistrar_Click

        Dim lnkVoltar As New LinkLabel() With {.Text = "Já possui uma conta? Faça Login", .Location = New Point(0, 560), .Size = New Size(900, 20), .TextAlign = ContentAlignment.MiddleCenter, .LinkColor = COR_TEXTO_MUTED, .Font = New Font("Segoe UI", 9.5, FontStyle.Regular), .BackColor = Color.Transparent, .ActiveLinkColor = COR_DESTAQUE}
        AddHandler lnkVoltar.LinkClicked, Sub() Me.Close()

        Me.Controls.AddRange(New Control() {pnlPiloto, pnlVeiculo, btnRegistrar, lnkVoltar})
    End Sub

    Private Function CriarLabel(texto As String, x As Integer, y As Integer) As Label
        Return New Label() With {.Text = texto, .Location = New Point(x, y), .AutoSize = True, .Font = New Font("Segoe UI", 9, FontStyle.Regular), .ForeColor = COR_TEXTO_MUTED, .BackColor = Color.Transparent}
    End Function

    Private Function CriarTextBox(x As Integer, y As Integer, w As Integer) As TextBox
        Return New TextBox() With {.Location = New Point(x, y), .Size = New Size(w, 25), .BackColor = Color.FromArgb(15, 17, 21), .ForeColor = COR_TEXTO_PRINCIPAL, .BorderStyle = BorderStyle.FixedSingle, .Font = New Font("Segoe UI", 10)}
    End Function

    Private Sub EstilizarBordaSimplesGlass(sender As Object, e As PaintEventArgs)
        Dim pnl = DirectCast(sender, Panel)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Using p As New Pen(Color.FromArgb(51, 65, 85), 1) : e.Graphics.DrawRectangle(p, 0, 0, pnl.Width - 1, pnl.Height - 1) : End Using
    End Sub

    Private Sub ArredondarBotaoGDI(sender As Object, e As PaintEventArgs)
        Dim btn = DirectCast(sender, Button)
        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        Dim r = 6
        Dim path As New GraphicsPath()
        path.AddArc(0, 0, r * 2, r * 2, 180, 90) : path.AddArc(btn.Width - r * 2 - 1, 0, r * 2, r * 2, 270, 90)
        path.AddArc(btn.Width - r * 2 - 1, btn.Height - r * 2 - 1, r * 2, r * 2, 0, 90) : path.AddArc(0, btn.Height - r * 2 - 1, r * 2, r * 2, 90, 90)
        path.CloseAllFigures()
        btn.Region = New Region(path)
        Using b As New SolidBrush(btn.BackColor) : g.FillPath(b, path) : End Using
        TextRenderer.DrawText(g, btn.Text, btn.Font, New Rectangle(0, 0, btn.Width, btn.Height), btn.ForeColor, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
    End Sub

    Private Sub AlternarVisibilidadeSenha(sender As Object, e As EventArgs)
        If txtNovaSenha.PasswordChar = "*"c Then
            txtNovaSenha.PasswordChar = ControlChars.NullChar : txtConfirmaSenha.PasswordChar = ControlChars.NullChar : btnVerSenha.Text = "🔒"
        Else
            txtNovaSenha.PasswordChar = "*"c : txtConfirmaSenha.PasswordChar = "*"c : btnVerSenha.Text = "👁️"
        End If
    End Sub

    Private Sub AplicarMascaraDinamicaCpfCnpj(sender As Object, e As EventArgs)
        Dim txt = DirectCast(sender, TextBox)
        RemoveHandler txt.TextChanged, AddressOf AplicarMascaraDinamicaCpfCnpj
        Dim valorPuro As String = New String(txt.Text.Where(AddressOf Char.IsDigit).ToArray())
        If valorPuro.Length > 14 Then valorPuro = valorPuro.Substring(0, 14)
        Dim formatado As String = valorPuro
        If valorPuro.Length <= 11 Then
            If valorPuro.Length >= 4 Then formatado = formatado.Insert(3, ".")
            If valorPuro.Length >= 7 Then formatado = formatado.Insert(7, ".")
            If valorPuro.Length >= 10 Then formatado = formatado.Insert(11, "-")
        Else
            If valorPuro.Length >= 3 Then formatado = formatado.Insert(2, ".")
            If valorPuro.Length >= 6 Then formatado = formatado.Insert(6, ".")
            If valorPuro.Length >= 9 Then formatado = formatado.Insert(10, "/")
            If valorPuro.Length >= 13 Then formatado = formatado.Insert(15, "-")
        End If
        txt.Text = formatado
        txt.SelectionStart = txt.Text.Length
        AddHandler txt.TextChanged, AddressOf AplicarMascaraDinamicaCpfCnpj
    End Sub

    Private Function ValidarCPF(cpf As String) As Boolean
        If cpf.Length <> 11 OrElse cpf.Distinct().Count() = 1 Then Return False
        Dim soma As Integer = 0
        For i As Integer = 0 To 8 : soma += Integer.Parse(cpf(i).ToString()) * (10 - i) : Next
        Dim resto As Integer = soma Mod 11
        Dim digito1 As Integer = If(resto < 2, 0, 11 - resto)
        If Integer.Parse(cpf(9).ToString()) <> digito1 Then Return False
        soma = 0
        For i As Integer = 0 To 9 : soma += Integer.Parse(cpf(i).ToString()) * (11 - i) : Next
        resto = soma Mod 11
        Dim digito2 As Integer = If(resto < 2, 0, 11 - resto)
        Return Integer.Parse(cpf(10).ToString()) = digito2
    End Function

    Private Function ValidarCNPJ(cnpj As String) As Boolean
        If cnpj.Length <> 14 OrElse cnpj.Distinct().Count() = 1 Then Return False
        Dim m1() As Integer = {5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2}
        Dim m2() As Integer = {6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2}
        Dim soma As Integer = 0
        For i As Integer = 0 To 11 : soma += Integer.Parse(cnpj(i).ToString()) * m1(i) : Next
        Dim resto As Integer = (soma Mod 11)
        Dim digito1 As Integer = If(resto < 2, 0, 11 - resto)
        If Integer.Parse(cnpj(12).ToString()) <> digito1 Then Return False
        soma = 0
        For i As Integer = 0 To 12 : soma += Integer.Parse(cnpj(i).ToString()) * m2(i) : Next
        resto = (soma Mod 11)
        Dim digito2 As Integer = If(resto < 2, 0, 11 - resto)
        Return Integer.Parse(cnpj(13).ToString()) = digito2
    End Function

    Private Sub btnRegistrar_Click(sender As Object, e As EventArgs)
        Dim documentoLimpo As String = New String(txtCpfCnpj.Text.Where(AddressOf Char.IsDigit).ToArray())
        Dim email As String = txtNovoEmail.Text.Trim()

        If String.IsNullOrEmpty(txtNovoNome.Text) OrElse String.IsNullOrEmpty(txtNovaSenha.Text) Then
            MessageBox.Show("Preencha o Nome e a Senha.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning) : Return
        End If

        If documentoLimpo.Length = 11 Then
            If Not ValidarCPF(documentoLimpo) Then MessageBox.Show("CPF Inválido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Stop) : Return
        ElseIf documentoLimpo.Length = 14 Then
            If Not ValidarCNPJ(documentoLimpo) Then MessageBox.Show("CNPJ Inválido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Stop) : Return
        Else
            MessageBox.Show("Documento Incompleto.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning) : Return
        End If

        If Not email.Contains("@") Then MessageBox.Show("E-mail inválido.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning) : Return
        If txtNovaSenha.Text <> txtConfirmaSenha.Text Then MessageBox.Show("Senhas não coincidem.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error) : Return
        If String.IsNullOrEmpty(txtModCarro.Text) Then MessageBox.Show("Preencha o modelo do veículo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning) : Return

        Using db As New AppDbContext()
            If db.Usuarios.Any(Function(u) u.CPF = documentoLimpo) Then
                MessageBox.Show("Documento já cadastrado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Stop) : Return
            End If

            Dim novoUsuario As New Usuario() With {.Nome = txtNovoNome.Text.Trim(), .CPF = documentoLimpo, .Telefone = txtTelefone.Text, .Email = email, .Senha = txtNovaSenha.Text, .Endereco = "", .AvatarSelecionado = "", .MetaDiaria = 250D, .CustosFixosMensais = 0D, .CategoriaPlano = "BRONZE", .Ativo = True}
            db.Usuarios.Add(novoUsuario)
            db.SaveChanges()

            Try
                Dim veiculoType = db.Model.GetEntityTypes().FirstOrDefault(Function(t) t.Name.EndsWith("Veiculo")).ClrType
                Dim novoCarro As Object = Activator.CreateInstance(veiculoType)
                veiculoType.GetProperty("UsuarioId").SetValue(novoCarro, novoUsuario.Id)
                veiculoType.GetProperty("Modelo").SetValue(novoCarro, txtModCarro.Text.Trim())
                veiculoType.GetProperty("TipoVeiculo").SetValue(novoCarro, "Combustão")
                veiculoType.GetProperty("Ativo").SetValue(novoCarro, True)
                veiculoType.GetProperty("ConsumoCombustivel").SetValue(novoCarro, 10D)
                veiculoType.GetProperty("PrecoCombustivel").SetValue(novoCarro, 5.5D)
                db.Add(novoCarro)
                db.SaveChanges()
            Catch ex As Exception
            End Try
        End Using

        MessageBox.Show("Conta criada com sucesso!", "Flow Road", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Me.Close()
    End Sub
End Class