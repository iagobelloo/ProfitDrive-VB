Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Windows.Forms
Imports Microsoft.EntityFrameworkCore

Public Class FormCadastroPerfil
    Inherits Form

    ' Componentes
    Private pnlHeaderPerfil, pnlDadosPessoais, pnlDadosVeiculo As Panel
    Private picFotoPiloto As PictureBox
    Private btnAlterarFoto, btnAlterarSenha, btnAdicionarVeiculo, btnExcluirVeiculo, btnSalvar, btnVoltar As Button
    Private lblNomeHeader, lblPlanoTag, lblMensagemPlano As Label

    ' Inputs de Dados
    Private txtNome, txtCPF, txtEmail, txtEndereco, txtMetaDiaria, txtCustosFixos As TextBox
    Private txtModeloCarro, txtConsumoCombustivel, txtPrecoCombustivel, txtConsumoEletrico, txtPrecoKwh As TextBox
    Private txtTelefone As MaskedTextBox
    Private cmbVeiculosCadastrados, cmbTipoVeiculo As ComboBox

    ' Labels Dinâmicas
    Private lblConsC, lblPrcC, lblConsE, lblPrcE As Label

    Private _usuarioIdLogado As Integer
    Private _veiculoSelecionadoId As Integer = 0
    Private _caminhoFotoSelecionada As String = ""

    Private COR_CANVAS As Color = Color.FromArgb(11, 13, 17)
    Private COR_CARD As Color = Color.FromArgb(22, 28, 36)
    Private COR_INPUT_BG As Color = Color.FromArgb(15, 17, 21)
    Private COR_TEXTO_PRINCIPAL As Color = Color.FromArgb(248, 250, 252)
    Private COR_TEXTO_MUTED As Color = Color.FromArgb(148, 163, 184)
    Private COR_DESTAQUE As Color = Color.FromArgb(56, 189, 248)
    Private COR_VERDE As Color = Color.FromArgb(16, 185, 129)
    Private COR_VERM As Color = Color.FromArgb(239, 68, 68)
    Private COR_BTN_DARK As Color = Color.FromArgb(37, 47, 61)
    Private COR_BORDA_CARD As Color = Color.FromArgb(43, 55, 71)

    Sub New(usuarioId As Integer)
        MyBase.New()
        Me._usuarioIdLogado = usuarioId
        Me.Text = "Flow Road - Identidade e Garagem "
        Me.Size = New Size(940, 730)

        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.BackColor = COR_CANVAS

        MontarLayoutPremium()
        CarregarDadosBanco()
    End Sub

    Private Sub MontarLayoutPremium()
        'HEADER
        pnlHeaderPerfil = New Panel() With {.Location = New Point(25, 20), .Size = New Size(870, 110), .BackColor = COR_CARD}
        AddHandler pnlHeaderPerfil.Paint, AddressOf DesenharPainelCard

        picFotoPiloto = New PictureBox() With {.Location = New Point(20, 15), .Size = New Size(80, 80), .BackColor = Color.Transparent, .SizeMode = PictureBoxSizeMode.Zoom, .Cursor = Cursors.Hand}
        AddHandler picFotoPiloto.Click, AddressOf SelecionarFotoComputador

        lblNomeHeader = New Label() With {.Text = "OPERADOR", .Location = New Point(120, 20), .Size = New Size(400, 28), .Font = New Font("Segoe UI", 15, FontStyle.Bold), .ForeColor = COR_TEXTO_PRINCIPAL, .BackColor = Color.Transparent}
        lblPlanoTag = New Label() With {.Text = "🏆 LICENÇA ATIVA", .Location = New Point(120, 52), .Size = New Size(400, 20), .Font = New Font("Segoe UI", 9.5, FontStyle.Bold), .ForeColor = Color.FromArgb(234, 179, 8), .BackColor = Color.Transparent}
        lblMensagemPlano = New Label() With {.Text = "Sincronizado com os servidores Flow Road.", .Location = New Point(120, 75), .Size = New Size(400, 20), .Font = New Font("Segoe UI", 8.5), .ForeColor = COR_TEXTO_MUTED, .BackColor = Color.Transparent}

        btnAlterarFoto = New Button() With {.Text = "Alterar Imagem", .Location = New Point(730, 38), .Size = New Size(120, 35), .BackColor = COR_BTN_DARK, .ForeColor = COR_TEXTO_PRINCIPAL, .Font = New Font("Segoe UI", 9, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnAlterarFoto.FlatAppearance.BorderSize = 0 : AddHandler btnAlterarFoto.Paint, AddressOf ArredondarBotaoGDI : AddHandler btnAlterarFoto.Click, AddressOf SelecionarFotoComputador

        pnlHeaderPerfil.Controls.AddRange(New Control() {picFotoPiloto, lblNomeHeader, lblPlanoTag, lblMensagemPlano, btnAlterarFoto})
        Me.Controls.Add(pnlHeaderPerfil)

        'COLUNA ESQUERDA: DADOS PESSOAIS E METAS
        pnlDadosPessoais = New Panel() With {.Location = New Point(25, 145), .Size = New Size(425, 480), .BackColor = COR_CARD}
        AddHandler pnlDadosPessoais.Paint, AddressOf DesenharPainelCard

        Dim lblTitPess = New Label() With {.Text = "Dados de Registo & Metas", .Location = New Point(20, 15), .Size = New Size(300, 22), .Font = New Font("Segoe UI", 11, FontStyle.Bold), .ForeColor = COR_DESTAQUE, .BackColor = Color.Transparent}
        pnlDadosPessoais.Controls.Add(lblTitPess)

        txtNome = CriarInputPremium(pnlDadosPessoais, "Nome Completo:", 20, 50, 385)
        txtCPF = CriarInputPremium(pnlDadosPessoais, "CPF / CNPJ:", 20, 115, 385)
        txtCPF.ReadOnly = True : txtCPF.ForeColor = COR_TEXTO_MUTED

        txtTelefone = CriarMaskedInputPremium(pnlDadosPessoais, "Telefone/WhatsApp:", 20, 180, 385, "(00) 00000-0000")
        txtEmail = CriarInputPremium(pnlDadosPessoais, "E-mail Principal:", 20, 245, 385)
        txtEndereco = CriarInputPremium(pnlDadosPessoais, "Endereço:", 20, 310, 385)

        ' Metas Financeiras
        txtMetaDiaria = CriarInputPremium(pnlDadosPessoais, "Meta Diária (R$):", 20, 375, 185, True)
        AddHandler txtMetaDiaria.TextChanged, AddressOf AplicarMascaraMoeda_TextChanged

        txtCustosFixos = CriarInputPremium(pnlDadosPessoais, "Custos Fixos Mensais (R$):", 220, 375, 185, True)
        AddHandler txtCustosFixos.TextChanged, AddressOf AplicarMascaraMoeda_TextChanged

        btnAlterarSenha = New Button() With {.Text = "🔐 Atualizar Senha de Acesso", .Location = New Point(20, 435), .Size = New Size(385, 32), .BackColor = Color.FromArgb(127, 29, 29), .ForeColor = Color.White, .Font = New Font("Segoe UI", 9, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnAlterarSenha.FlatAppearance.BorderSize = 0 : AddHandler btnAlterarSenha.Paint, AddressOf ArredondarBotaoGDI : AddHandler btnAlterarSenha.Click, AddressOf btnAlterarSenha_Click
        pnlDadosPessoais.Controls.Add(btnAlterarSenha)
        Me.Controls.Add(pnlDadosPessoais)

        'COLUNA DIREITA: GARAGEM
        pnlDadosVeiculo = New Panel() With {.Location = New Point(470, 145), .Size = New Size(425, 480), .BackColor = COR_CARD}
        AddHandler pnlDadosVeiculo.Paint, AddressOf DesenharPainelCard

        Dim lblTitVeic = New Label() With {.Text = "Garagem ", .Location = New Point(20, 15), .Size = New Size(300, 22), .Font = New Font("Segoe UI", 11, FontStyle.Bold), .ForeColor = COR_DESTAQUE, .BackColor = Color.Transparent}
        pnlDadosVeiculo.Controls.Add(lblTitVeic)

        Dim lblCombo = New Label() With {.Text = "Veículo Selecionado:", .Location = New Point(20, 50), .Size = New Size(300, 15), .ForeColor = COR_TEXTO_MUTED, .Font = New Font("Segoe UI", 8.5, FontStyle.Bold), .BackColor = Color.Transparent}
        cmbVeiculosCadastrados = New ComboBox() With {.Location = New Point(20, 70), .Size = New Size(385, 28), .DropDownStyle = ComboBoxStyle.DropDownList, .BackColor = COR_INPUT_BG, .ForeColor = COR_TEXTO_PRINCIPAL, .FlatStyle = FlatStyle.Flat, .Font = New Font("Segoe UI", 10)}
        pnlDadosVeiculo.Controls.AddRange(New Control() {lblCombo, cmbVeiculosCadastrados})

        btnAdicionarVeiculo = New Button() With {.Text = "➕ Adicionar Veículo", .Location = New Point(20, 110), .Size = New Size(185, 30), .BackColor = COR_BTN_DARK, .ForeColor = COR_VERDE, .Font = New Font("Segoe UI", 9, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnAdicionarVeiculo.FlatAppearance.BorderSize = 0 : AddHandler btnAdicionarVeiculo.Paint, AddressOf ArredondarBotaoGDI : AddHandler btnAdicionarVeiculo.Click, AddressOf btnAdicionarVeiculo_Click

        btnExcluirVeiculo = New Button() With {.Text = "✖ Remover Veículo", .Location = New Point(220, 110), .Size = New Size(185, 30), .BackColor = COR_BTN_DARK, .ForeColor = COR_VERM, .Font = New Font("Segoe UI", 9, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnExcluirVeiculo.FlatAppearance.BorderSize = 0 : AddHandler btnExcluirVeiculo.Paint, AddressOf ArredondarBotaoGDI : AddHandler btnExcluirVeiculo.Click, AddressOf btnExcluirVeiculo_Click
        pnlDadosVeiculo.Controls.AddRange(New Control() {btnAdicionarVeiculo, btnExcluirVeiculo})

        Dim linhaSeparadora As New Label() With {.Location = New Point(20, 155), .Size = New Size(385, 1), .BackColor = COR_BORDA_CARD}
        pnlDadosVeiculo.Controls.Add(linhaSeparadora)

        txtModeloCarro = CriarInputPremium(pnlDadosVeiculo, "Modelo (Ex: Corolla 2023):", 20, 175, 185)

        Dim lblMot = New Label() With {.Text = "Motorização:", .Location = New Point(220, 175), .Size = New Size(185, 15), .ForeColor = COR_TEXTO_MUTED, .Font = New Font("Segoe UI", 8.5, FontStyle.Bold), .BackColor = Color.Transparent}
        cmbTipoVeiculo = New ComboBox() With {.Location = New Point(220, 195), .Size = New Size(185, 28), .DropDownStyle = ComboBoxStyle.DropDownList, .BackColor = COR_INPUT_BG, .ForeColor = COR_TEXTO_PRINCIPAL, .FlatStyle = FlatStyle.Flat, .Font = New Font("Segoe UI", 10)}
        cmbTipoVeiculo.Items.AddRange(New Object() {"Combustão", "Híbrido", "Elétrico"})
        AddHandler cmbTipoVeiculo.SelectedIndexChanged, AddressOf cmbTipoVeiculo_SelectedIndexChanged
        pnlDadosVeiculo.Controls.AddRange(New Control() {lblMot, cmbTipoVeiculo})

        ' Grupo Combustão
        lblConsC = New Label() With {.Text = "Consumo Alvo (KM/L):", .Location = New Point(20, 240), .Size = New Size(185, 15), .ForeColor = COR_TEXTO_MUTED, .Font = New Font("Segoe UI", 8.5, FontStyle.Bold), .BackColor = Color.Transparent}
        txtConsumoCombustivel = CriarInputApenasTextBox(pnlDadosVeiculo, 20, 260, 185, True)

        lblPrcC = New Label() With {.Text = "Preço Médio do Litro:", .Location = New Point(220, 240), .Size = New Size(185, 15), .ForeColor = COR_TEXTO_MUTED, .Font = New Font("Segoe UI", 8.5, FontStyle.Bold), .BackColor = Color.Transparent}
        txtPrecoCombustivel = CriarInputApenasTextBox(pnlDadosVeiculo, 220, 260, 185, True)
        AddHandler txtPrecoCombustivel.TextChanged, AddressOf AplicarMascaraMoeda_TextChanged
        pnlDadosVeiculo.Controls.AddRange(New Control() {lblConsC, lblPrcC})

        ' Grupo Elétrico
        lblConsE = New Label() With {.Text = "Consumo EV (KM/kWh):", .Location = New Point(20, 305), .Size = New Size(185, 15), .ForeColor = COR_TEXTO_MUTED, .Font = New Font("Segoe UI", 8.5, FontStyle.Bold), .BackColor = Color.Transparent}
        txtConsumoEletrico = CriarInputApenasTextBox(pnlDadosVeiculo, 20, 325, 185, True)

        lblPrcE = New Label() With {.Text = "Preço Médio do kWh:", .Location = New Point(220, 305), .Size = New Size(185, 15), .ForeColor = COR_TEXTO_MUTED, .Font = New Font("Segoe UI", 8.5, FontStyle.Bold), .BackColor = Color.Transparent}
        txtPrecoKwh = CriarInputApenasTextBox(pnlDadosVeiculo, 220, 325, 185, True)
        AddHandler txtPrecoKwh.TextChanged, AddressOf AplicarMascaraMoeda_TextChanged
        pnlDadosVeiculo.Controls.AddRange(New Control() {lblConsE, lblPrcE})

        Me.Controls.Add(pnlDadosVeiculo)

        btnVoltar = New Button() With {.Text = "RETORNAR AO COCKPIT", .Location = New Point(25, 635), .Size = New Size(425, 45), .BackColor = COR_BTN_DARK, .ForeColor = COR_TEXTO_PRINCIPAL, .Font = New Font("Segoe UI", 10.5, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnVoltar.FlatAppearance.BorderSize = 0 : AddHandler btnVoltar.Paint, AddressOf ArredondarBotaoGDI : AddHandler btnVoltar.Click, Sub() Me.Close()

        btnSalvar = New Button() With {.Text = "GRAVAR DEFINIÇÕES DA CONTA", .Location = New Point(470, 635), .Size = New Size(425, 45), .BackColor = COR_VERDE, .ForeColor = Color.FromArgb(11, 13, 17), .Font = New Font("Segoe UI", 10.5, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnSalvar.FlatAppearance.BorderSize = 0 : AddHandler btnSalvar.Paint, AddressOf ArredondarBotaoGDI : AddHandler btnSalvar.Click, AddressOf btnSalvar_Click

        Me.Controls.AddRange(New Control() {btnSalvar, btnVoltar})
    End Sub

    Private Function CriarInputPremium(pnlPai As Panel, titulo As String, x As Integer, y As Integer, w As Integer, Optional rightAlign As Boolean = False) As TextBox
        Dim lbl As New Label() With {.Text = titulo, .Location = New Point(x, y), .Size = New Size(w, 15), .Font = New Font("Segoe UI", 8.5, FontStyle.Bold), .ForeColor = COR_TEXTO_MUTED, .BackColor = Color.Transparent}
        pnlPai.Controls.Add(lbl)
        Return CriarInputApenasTextBox(pnlPai, x, y + 20, w, rightAlign)
    End Function

    Private Function CriarMaskedInputPremium(pnlPai As Panel, titulo As String, x As Integer, y As Integer, w As Integer, mask As String) As MaskedTextBox
        Dim lbl As New Label() With {.Text = titulo, .Location = New Point(x, y), .Size = New Size(w, 15), .Font = New Font("Segoe UI", 8.5, FontStyle.Bold), .ForeColor = COR_TEXTO_MUTED, .BackColor = Color.Transparent}
        pnlPai.Controls.Add(lbl)

        Dim pnlBorder As New Panel() With {.Location = New Point(x, y + 20), .Size = New Size(w, 34), .BackColor = COR_INPUT_BG}
        AddHandler pnlBorder.Paint, Sub(s, e)
                                        Using p As New Pen(COR_BORDA_CARD, 1) : e.Graphics.DrawRectangle(p, 0, 0, pnlBorder.Width - 1, pnlBorder.Height - 1) : End Using
                                    End Sub
        Dim txt As New MaskedTextBox() With {.Mask = mask, .Location = New Point(10, 8), .Size = New Size(w - 20, 20), .BorderStyle = BorderStyle.None, .BackColor = COR_INPUT_BG, .ForeColor = COR_TEXTO_PRINCIPAL, .Font = New Font("Segoe UI", 10.5)}
        pnlBorder.Controls.Add(txt) : pnlPai.Controls.Add(pnlBorder)
        Return txt
    End Function

    Private Function CriarInputApenasTextBox(pnlPai As Panel, x As Integer, y As Integer, w As Integer, Optional rightAlign As Boolean = False) As TextBox
        Dim pnlBorder As New Panel() With {.Location = New Point(x, y), .Size = New Size(w, 34), .BackColor = COR_INPUT_BG}
        AddHandler pnlBorder.Paint, Sub(s, e)
                                        Using p As New Pen(COR_BORDA_CARD, 1) : e.Graphics.DrawRectangle(p, 0, 0, pnlBorder.Width - 1, pnlBorder.Height - 1) : End Using
                                    End Sub
        Dim txt As New TextBox() With {.Location = New Point(10, 8), .Size = New Size(w - 20, 20), .BorderStyle = BorderStyle.None, .BackColor = COR_INPUT_BG, .ForeColor = COR_TEXTO_PRINCIPAL, .Font = New Font("Segoe UI", 10.5)}
        If rightAlign Then txt.TextAlign = HorizontalAlignment.Right
        pnlBorder.Controls.Add(txt) : pnlPai.Controls.Add(pnlBorder)
        Return txt
    End Function

    Private Sub DesenharPainelCard(sender As Object, e As PaintEventArgs)
        Dim pnl = DirectCast(sender, Panel)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Using p As New Pen(COR_BORDA_CARD, 1.2F) : e.Graphics.DrawRectangle(p, 0, 0, pnl.Width - 1, pnl.Height - 1) : End Using
    End Sub

    Private Sub ArredondarBotaoGDI(sender As Object, e As PaintEventArgs)
        Dim btn = DirectCast(sender, Button)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear

        e.Graphics.Clear(btn.Parent.BackColor)

        Dim r As Integer = 6
        Dim path As New GraphicsPath()
        Dim w As Integer = btn.Width - 1
        Dim h As Integer = btn.Height - 1

        path.AddArc(0, 0, r * 2, r * 2, 180, 90)
        path.AddArc(w - (r * 2), 0, r * 2, r * 2, 270, 90)
        path.AddArc(w - (r * 2), h - (r * 2), r * 2, r * 2, 0, 90)
        path.AddArc(0, h - (r * 2), r * 2, r * 2, 90, 90)
        path.CloseAllFigures()

        Using b As New SolidBrush(btn.BackColor)
            e.Graphics.FillPath(b, path)
        End Using

        Using p As New Pen(btn.BackColor, 1.0F)
            e.Graphics.DrawPath(p, path)
        End Using

        TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, New Rectangle(0, 0, btn.Width, btn.Height), btn.ForeColor, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
    End Sub

    'SISTEMA DE VEÍCULOS E PERFIL
    Private Sub cmbTipoVeiculo_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim tipo As String = cmbTipoVeiculo.SelectedItem?.ToString()
        Dim isCombustivel = (tipo = "Combustão" OrElse tipo = "Híbrido")
        Dim isEletrico = (tipo = "Elétrico" OrElse tipo = "Híbrido")

        lblConsC.Visible = isCombustivel : txtConsumoCombustivel.Parent.Visible = isCombustivel
        lblPrcC.Visible = isCombustivel : txtPrecoCombustivel.Parent.Visible = isCombustivel

        lblConsE.Visible = isEletrico : txtConsumoEletrico.Parent.Visible = isEletrico
        lblPrcE.Visible = isEletrico : txtPrecoKwh.Parent.Visible = isEletrico
    End Sub

    Private Sub SelecionarFotoComputador(sender As Object, e As EventArgs)
        Using ofd As New OpenFileDialog() With {.Filter = "Imagens|*.jpg;*.jpeg;*.png"}
            If ofd.ShowDialog() = DialogResult.OK Then
                _caminhoFotoSelecionada = ofd.FileName
                picFotoPiloto.Image = Image.FromFile(_caminhoFotoSelecionada)
            End If
        End Using
    End Sub

    Private Sub btnAlterarSenha_Click(sender As Object, e As EventArgs)
        Using db As New AppDbContext()
            Dim u = db.Usuarios.FirstOrDefault(Function(user) user.Id = _usuarioIdLogado)
            If u Is Nothing Then Return
            If u.Senha <> Microsoft.VisualBasic.InputBox("Confirme a senha ATUAL do Cockpit:", "Validação de Segurança", "").Trim() Then
                MessageBox.Show("Senha atual inválida.", "Falha de Autenticação", MessageBoxButtons.OK, MessageBoxIcon.Error) : Return
            End If
            Dim nova = Microsoft.VisualBasic.InputBox("Digite a NOVA senha de acesso:", "Segurança Flow Road", "")
            If Not String.IsNullOrEmpty(nova.Trim()) Then
                u.Senha = nova.Trim() : db.SaveChanges()
                MessageBox.Show("Credencial alterada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End Using
    End Sub

    Private Sub CarregarDadosBanco()
        Using db As New AppDbContext()
            Dim u = db.Usuarios.AsNoTracking().FirstOrDefault(Function(user) user.Id = _usuarioIdLogado)
            If u IsNot Nothing Then
                txtNome.Text = u.Nome : lblNomeHeader.Text = u.Nome.ToUpper()
                txtEmail.Text = u.Email : txtEndereco.Text = u.Endereco
                txtMetaDiaria.Text = u.MetaDiaria.ToString("C2") : txtCustosFixos.Text = u.CustosFixosMensais.ToString("C2")
                Try : txtTelefone.Text = CallByName(u, "Telefone", CallType.Get).ToString() : Catch : End Try
                Try : lblPlanoTag.Text = "💎 LICENÇA " & CallByName(u, "CategoriaPlano", CallType.Get).ToString().ToUpper() : Catch : End Try
                If Not String.IsNullOrEmpty(u.CPF) Then txtCPF.Text = u.CPF

                If Not String.IsNullOrEmpty(u.AvatarSelecionado) AndAlso File.Exists(u.AvatarSelecionado) Then
                    _caminhoFotoSelecionada = u.AvatarSelecionado : picFotoPiloto.Image = Image.FromFile(_caminhoFotoSelecionada)
                Else
                    Dim bmp As New Bitmap(100, 100)
                    Using g As Graphics = Graphics.FromImage(bmp)
                        g.Clear(COR_CARD)
                        Dim sf As New StringFormat() With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
                        g.DrawString("👤", New Font("Segoe UI Emoji", 40), Brushes.White, New Rectangle(0, 0, 100, 100), sf)
                    End Using
                    picFotoPiloto.Image = bmp
                End If

                ' Deixa a foto circular
                Dim path As New GraphicsPath()
                path.AddEllipse(0, 0, 80, 80)
                picFotoPiloto.Region = New Region(path)
            End If

            Dim listaVeh As New List(Of Object)
            Try
                Dim type = db.Model.GetEntityTypes().FirstOrDefault(Function(t) t.Name.EndsWith("Veiculo"))
                If type IsNot Nothing Then
                    Dim rawSet = db.GetType().GetMethod("Set", System.Type.EmptyTypes).MakeGenericMethod(type.ClrType).Invoke(db, Nothing)
                    Dim enumerator = DirectCast(rawSet, System.Collections.IEnumerable).GetEnumerator()
                    While enumerator.MoveNext()
                        Dim obj = enumerator.Current
                        Dim pUserId = obj.GetType().GetProperty("UsuarioId")
                        If pUserId IsNot Nothing AndAlso Convert.ToInt32(pUserId.GetValue(obj)) = _usuarioIdLogado Then listaVeh.Add(obj)
                    End While
                End If
            Catch : End Try

            RemoveHandler cmbVeiculosCadastrados.SelectedIndexChanged, AddressOf cmbVeiculosCadastrados_SelectedIndexChanged
            cmbVeiculosCadastrados.DataSource = Nothing
            If listaVeh.Count > 0 Then
                cmbVeiculosCadastrados.DisplayMember = "Modelo" : cmbVeiculosCadastrados.ValueMember = "Id" : cmbVeiculosCadastrados.DataSource = listaVeh
                Dim carroAtivo = listaVeh.FirstOrDefault(Function(v) Convert.ToBoolean(v.GetType().GetProperty("Ativo").GetValue(v)))
                If carroAtivo Is Nothing Then carroAtivo = listaVeh(0)
                _veiculoSelecionadoId = Convert.ToInt32(carroAtivo.GetType().GetProperty("Id").GetValue(carroAtivo))
                cmbVeiculosCadastrados.SelectedValue = _veiculoSelecionadoId
                ExibirDetalhesVeiculoNoForm(_veiculoSelecionadoId)
            Else
                txtModeloCarro.Clear() : _veiculoSelecionadoId = 0
            End If
            AddHandler cmbVeiculosCadastrados.SelectedIndexChanged, AddressOf cmbVeiculosCadastrados_SelectedIndexChanged
        End Using
    End Sub

    Private Sub ExibirDetalhesVeiculoNoForm(veicId As Integer)
        Using db As New AppDbContext()
            Try
                Dim t = db.Model.GetEntityTypes().FirstOrDefault(Function(x) x.Name.EndsWith("Veiculo"))
                Dim enumerator = DirectCast(db.GetType().GetMethod("Set", System.Type.EmptyTypes).MakeGenericMethod(t.ClrType).Invoke(db, Nothing), System.Collections.IEnumerable).GetEnumerator()
                While enumerator.MoveNext()
                    Dim car = enumerator.Current
                    If Convert.ToInt32(car.GetType().GetProperty("Id").GetValue(car)) = veicId Then
                        txtModeloCarro.Text = car.GetType().GetProperty("Modelo").GetValue(car).ToString()
                        cmbTipoVeiculo.SelectedItem = If(car.GetType().GetProperty("TipoVeiculo").GetValue(car)?.ToString(), "Combustão")
                        txtConsumoCombustivel.Text = Convert.ToDecimal(car.GetType().GetProperty("ConsumoCombustivel").GetValue(car)).ToString("F2")
                        txtPrecoCombustivel.Text = Convert.ToDecimal(car.GetType().GetProperty("PrecoCombustivel").GetValue(car)).ToString("C2")
                        txtConsumoEletrico.Text = Convert.ToDecimal(car.GetType().GetProperty("ConsumoEletrico").GetValue(car)).ToString("F2")
                        txtPrecoKwh.Text = Convert.ToDecimal(car.GetType().GetProperty("PrecoKwh").GetValue(car)).ToString("C2")
                        Exit While
                    End If
                End While
            Catch : End Try
        End Using
    End Sub

    Private Sub cmbVeiculosCadastrados_SelectedIndexChanged(sender As Object, e As EventArgs)
        If cmbVeiculosCadastrados.SelectedValue IsNot Nothing Then
            Try : _veiculoSelecionadoId = Convert.ToInt32(cmbVeiculosCadastrados.SelectedValue) : ExibirDetalhesVeiculoNoForm(_veiculoSelecionadoId) : Catch : End Try
        End If
    End Sub

    Private Sub btnAdicionarVeiculo_Click(sender As Object, e As EventArgs)
        Using db As New AppDbContext()
            Try
                Dim veiculoType = db.Model.GetEntityTypes().FirstOrDefault(Function(t) t.Name.EndsWith("Veiculo")).ClrType
                Dim novoCarro As Object = Activator.CreateInstance(veiculoType)
                veiculoType.GetProperty("UsuarioId").SetValue(novoCarro, _usuarioIdLogado)
                veiculoType.GetProperty("Modelo").SetValue(novoCarro, "Novo Veículo")
                db.Add(novoCarro) : db.SaveChanges() : CarregarDadosBanco()
                cmbVeiculosCadastrados.SelectedValue = Convert.ToInt32(veiculoType.GetProperty("Id").GetValue(novoCarro))
            Catch : End Try
        End Using
    End Sub

    Private Sub btnExcluirVeiculo_Click(sender As Object, e As EventArgs)
        If _veiculoSelecionadoId = 0 Then Return
        If MessageBox.Show("Remover este veículo da garagem?", "Frota Operacional", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then
            Using db As New AppDbContext()
                Try
                    Dim vType = db.Model.GetEntityTypes().FirstOrDefault(Function(t) t.Name.EndsWith("Veiculo")).ClrType
                    Dim dbSet = db.GetType().GetMethod("Set", System.Type.EmptyTypes).MakeGenericMethod(vType).Invoke(db, Nothing)
                    Dim enumerator = DirectCast(dbSet, System.Collections.IEnumerable).GetEnumerator()
                    While enumerator.MoveNext()
                        If Convert.ToInt32(enumerator.Current.GetType().GetProperty("Id").GetValue(enumerator.Current)) = _veiculoSelecionadoId Then
                            db.Remove(enumerator.Current) : db.SaveChanges() : Exit While
                        End If
                    End While
                    CarregarDadosBanco()
                Catch : End Try
            End Using
        End If
    End Sub

    Private Sub AplicarMascaraMoeda_TextChanged(sender As Object, e As EventArgs)
        Dim txt = DirectCast(sender, TextBox) : RemoveHandler txt.TextChanged, AddressOf AplicarMascaraMoeda_TextChanged
        Dim valorDecimal As Decimal = 0
        Dim digitos = New String(txt.Text.Where(AddressOf Char.IsDigit).ToArray())
        If Not String.IsNullOrEmpty(digitos) Then valorDecimal = Decimal.Parse(digitos) / 100D
        txt.Text = valorDecimal.ToString("C2") : txt.SelectionStart = txt.Text.Length
        AddHandler txt.TextChanged, AddressOf AplicarMascaraMoeda_TextChanged
    End Sub

    Private Function ObterValorDecimalLimpo(textoBox As String) As Decimal
        Dim res As Decimal : Decimal.TryParse(textoBox.Replace("R$", "").Replace(".", "").Replace(",", ".").Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, res) : Return res
    End Function

    Private Sub btnSalvar_Click(sender As Object, e As EventArgs)
        Dim cComb, cElec As Decimal
        If String.IsNullOrEmpty(txtNome.Text) Then MessageBox.Show("Nome é obrigatório.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning) : Return

        Dim txtCc = If(String.IsNullOrEmpty(txtConsumoCombustivel.Text) OrElse Not txtConsumoCombustivel.Parent.Visible, "0", txtConsumoCombustivel.Text)
        Dim txtCe = If(String.IsNullOrEmpty(txtConsumoEletrico.Text) OrElse Not txtConsumoEletrico.Parent.Visible, "0", txtConsumoEletrico.Text)

        If Not Decimal.TryParse(txtCc, cComb) OrElse Not Decimal.TryParse(txtCe, cElec) Then Return

        Using db As New AppDbContext()
            'Atualiza as informações do Usuário
            Dim u = db.Usuarios.FirstOrDefault(Function(user) user.Id = _usuarioIdLogado)
            If u IsNot Nothing Then
                u.Nome = txtNome.Text.Trim()
                u.Email = txtEmail.Text.Trim()
                u.Endereco = txtEndereco.Text.Trim()
                If Not String.IsNullOrEmpty(_caminhoFotoSelecionada) Then u.AvatarSelecionado = _caminhoFotoSelecionada
                u.MetaDiaria = ObterValorDecimalLimpo(txtMetaDiaria.Text)
                u.CustosFixosMensais = ObterValorDecimalLimpo(txtCustosFixos.Text)
                Try : CallByName(u, "Telefone", CallType.Set, txtTelefone.Text) : Catch : End Try
            End If

            'Atualiza e gerencia a ativação dos veículos na Garagem MySQL
            If _veiculoSelecionadoId > 0 Then
                Try
                    Dim todosVeiculosMotorista = db.Veiculos.Where(Function(v) v.UsuarioId = _usuarioIdLogado).ToList()

                    For Each car In todosVeiculosMotorista
                        car.Ativo = False
                    Next

                    'Busca o carro que foi selecionado para ser atualizado e ativado
                    Dim veiculoAtual = todosVeiculosMotorista.FirstOrDefault(Function(v) v.Id = _veiculoSelecionadoId)
                    If veiculoAtual IsNot Nothing Then
                        veiculoAtual.Modelo = txtModeloCarro.Text.Trim()
                        veiculoAtual.TipoVeiculo = cmbTipoVeiculo.SelectedItem.ToString()
                        veiculoAtual.ConsumoCombustivel = cComb
                        veiculoAtual.ConsumoEletrico = cElec
                        veiculoAtual.PrecoCombustivel = ObterValorDecimalLimpo(txtPrecoCombustivel.Text)
                        veiculoAtual.PrecoKwh = ObterValorDecimalLimpo(txtPrecoKwh.Text)
                        veiculoAtual.Ativo = True 'Define o veículo como o cockpit oficial atual
                    End If
                Catch ex As Exception
                End Try
            End If

            db.SaveChanges()
        End Using

        MessageBox.Show("Diretrizes de Perfil e Garagem atualizadas!", "Operação Validada", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Me.Close()
    End Sub
End Class