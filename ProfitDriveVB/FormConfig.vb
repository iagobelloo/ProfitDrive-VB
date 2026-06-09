Imports System.Drawing
Imports System.Windows.Forms
Imports System.Linq

Public Class FormConfig
    Inherits Form

    Private txtNome, txtMetaDiaria, txtCustosMensais As TextBox
    Private txtModeloCarro, txtConsumo, txtPrecoCombustivel As TextBox
    Private btnSalvar As Button

    Private COR_CANVAS As Color = Color.FromArgb(15, 17, 21)
    Private COR_CARD As Color = Color.FromArgb(26, 32, 44)
    Private COR_TEXTO_PRINCIPAL As Color = Color.FromArgb(248, 250, 252)
    Private COR_TEXTO_MUTED As Color = Color.FromArgb(148, 163, 184)
    Private COR_VERDE As Color = Color.FromArgb(16, 185, 129)

    Sub New()
        Me.Text = "Flow Road - Setup Rápido"
        Me.Size = New Size(420, 520)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.BackColor = COR_CANVAS

        MontarLayoutPremium()
        CarregarDadosAtuais()
    End Sub

    Private Sub MontarLayoutPremium()
        Dim lblTitulo As New Label() With {.Text = "CONFIGURAÇÃO BÁSICA", .Font = New Font("Segoe UI", 12, FontStyle.Bold), .ForeColor = COR_TEXTO_PRINCIPAL, .Location = New Point(30, 20), .Size = New Size(345, 25), .TextAlign = ContentAlignment.MiddleCenter}
        Me.Controls.Add(lblTitulo)

        ' PAINEL USUÁRIO
        Dim pnlMotorista As New Panel() With {.Location = New Point(30, 60), .Size = New Size(345, 160), .BackColor = COR_CARD}
        Dim lblTitMot As New Label() With {.Text = "DADOS DO MOTORISTA", .Location = New Point(15, 15), .Font = New Font("Segoe UI", 8.5, FontStyle.Bold), .ForeColor = COR_TEXTO_MUTED, .AutoSize = True}

        Dim lblNome = CriarLabelHUD("Seu Nome:", 45)
        txtNome = CriarTextBoxHUD(42)

        Dim lblMeta = CriarLabelHUD("Meta (R$):", 80)
        txtMetaDiaria = CriarTextBoxHUD(77)

        Dim lblCustos = CriarLabelHUD("Custos (Mês):", 115)
        txtCustosMensais = CriarTextBoxHUD(112)

        pnlMotorista.Controls.AddRange(New Control() {lblTitMot, lblNome, txtNome, lblMeta, txtMetaDiaria, lblCustos, txtCustosMensais})
        Me.Controls.Add(pnlMotorista)

        ' PAINEL VEÍCULO
        Dim pnlVeiculo As New Panel() With {.Location = New Point(30, 235), .Size = New Size(345, 160), .BackColor = COR_CARD}
        Dim lblTitVeic As New Label() With {.Text = "DADOS DO VEÍCULO", .Location = New Point(15, 15), .Font = New Font("Segoe UI", 8.5, FontStyle.Bold), .ForeColor = COR_TEXTO_MUTED, .AutoSize = True}

        Dim lblModelo = CriarLabelHUD("Modelo:", 45)
        txtModeloCarro = CriarTextBoxHUD(42)

        Dim lblConsumo = CriarLabelHUD("Média (KM/L):", 80)
        txtConsumo = CriarTextBoxHUD(77)

        Dim lblPreco = CriarLabelHUD("Preço Litro:", 115)
        txtPrecoCombustivel = CriarTextBoxHUD(112)

        pnlVeiculo.Controls.AddRange(New Control() {lblTitVeic, lblModelo, txtModeloCarro, lblConsumo, txtConsumo, lblPreco, txtPrecoCombustivel})
        Me.Controls.Add(pnlVeiculo)

        ' BOTÃO SALVAR
        btnSalvar = New Button() With {.Text = "Concluir Setup e Iniciar", .Location = New Point(30, 415), .Size = New Size(345, 40), .BackColor = COR_VERDE, .ForeColor = Color.White, .Font = New Font("Segoe UI", 10, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnSalvar.FlatAppearance.BorderSize = 0
        AddHandler btnSalvar.Click, AddressOf btnSalvar_Click
        Me.Controls.Add(btnSalvar)
    End Sub

    Private Function CriarLabelHUD(texto As String, y As Integer) As Label
        Return New Label() With {.Text = texto, .Location = New Point(15, y), .Size = New Size(100, 20), .Font = New Font("Segoe UI", 9.5), .ForeColor = COR_TEXTO_PRINCIPAL}
    End Function

    Private Function CriarTextBoxHUD(y As Integer) As TextBox
        Return New TextBox() With {.Location = New Point(120, y), .Size = New Size(210, 25), .BackColor = Color.FromArgb(15, 17, 21), .ForeColor = COR_TEXTO_PRINCIPAL, .BorderStyle = BorderStyle.FixedSingle, .Font = New Font("Segoe UI", 9.5)}
    End Function

    Private Sub CarregarDadosAtuais()
        Using db As New AppDbContext()
            Dim usuario = db.Usuarios.FirstOrDefault()
            Dim veiculo = db.Veiculos.FirstOrDefault()

            If usuario IsNot Nothing Then
                txtNome.Text = usuario.Nome
                txtMetaDiaria.Text = usuario.MetaDiaria.ToString("F2")
                txtCustosMensais.Text = usuario.CustosFixosMensais.ToString("F2")
            End If

            If veiculo IsNot Nothing Then
                txtModeloCarro.Text = veiculo.Modelo
                txtConsumo.Text = veiculo.ConsumoCombustivel.ToString("F2")
                txtPrecoCombustivel.Text = veiculo.PrecoCombustivel.ToString("F2")
            End If
        End Using
    End Sub

    Private Sub btnSalvar_Click(sender As Object, e As EventArgs)
        Dim meta, custos, consumo, preco As Decimal
        If String.IsNullOrEmpty(txtNome.Text) OrElse String.IsNullOrEmpty(txtModeloCarro.Text) Then Return
        If Not Decimal.TryParse(txtMetaDiaria.Text, meta) OrElse Not Decimal.TryParse(txtCustosMensais.Text, custos) OrElse Not Decimal.TryParse(txtConsumo.Text, consumo) OrElse Not Decimal.TryParse(txtPrecoCombustivel.Text, preco) Then Return

        Using db As New AppDbContext()
            Dim usuario = db.Usuarios.FirstOrDefault()
            If usuario Is Nothing Then
                usuario = New Usuario() : db.Usuarios.Add(usuario)
            End If
            usuario.Nome = txtNome.Text.Trim() : usuario.MetaDiaria = meta : usuario.CustosFixosMensais = custos

            Dim veiculo = db.Veiculos.FirstOrDefault()
            If veiculo Is Nothing Then
                veiculo = New Veiculo() : db.Veiculos.Add(veiculo)
            End If
            veiculo.Modelo = txtModeloCarro.Text.Trim() : veiculo.ConsumoCombustivel = consumo : veiculo.PrecoCombustivel = preco

            db.SaveChanges()
            Me.DialogResult = DialogResult.OK : Me.Close()
        End Using
    End Sub
End Class