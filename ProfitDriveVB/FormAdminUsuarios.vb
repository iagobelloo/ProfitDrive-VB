Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports System.Linq
Imports System.Collections.Generic
Imports System
Imports System.IO
Imports System.Text

Public Class FormAdminUsuarios
    Inherits Form

    ' Componentes Principais
    Private txtBusca As TextBox
    Private btnBuscar As Button
    Private btnExportarCSV As Button
    Private dgvUsuarios As DataGridView

    ' KPIs Globais
    Private lblKpiMotoristas As Label
    Private lblKpiGMV As Label
    Private lblKpiMRR As Label
    Private lblKpiInadimplentes As Label

    ' Painel de Dossiê do Motorista
    Private pnlAuditoriaLateral As Panel
    Private lblDetNome As Label
    Private lblDetEmail As Label
    Private lblDetCpf As Label
    Private lblDetStatus As Label
    Private lblAdminShield As Label
    Private lblDetPlano As Label
    Private lblDetVencimento As Label
    Private lblDetDiasRestantes As Label
    Private cmbMudarPlano As ComboBox
    Private lblDetCarroAtivo As Label
    Private lblDetFaturamento As Label
    Private lblDetCustos As Label
    Private lblDetLucroLiquido As Label

    'Componente para o Admin enviar mensagens
    Private txtMensagemAdmin As TextBox
    Private btnSalvarMensagem As Button

    ' Ações
    Private pnlDangerZone As Panel
    Private btnAlternarStatus As Button
    Private btnResetSenha As Button
    Private btnDeletarUsuario As Button
    Private lblStatusRodape As Label
    Private btnVoltar As Button

    Private _usuarioIdLogado As Integer

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

    Sub New(usuarioLogadoId As Integer)
        MyBase.New()
        Me._usuarioIdLogado = usuarioLogadoId
        Me.Text = "Flow Road - Command Center"
        Me.Size = New Size(1200, 780)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.BackColor = COR_CANVAS

        MontarLayoutCommandCenter()
        CarregarTodosUsuariosEIndicadores()
    End Sub

    Private Sub MontarLayoutCommandCenter()
        'KPIs
        Dim pnlKpis As New Panel() With {.Location = New Point(25, 20), .Size = New Size(1135, 95), .BackColor = Color.Transparent}

        lblKpiMotoristas = CriarKpiCard(pnlKpis, "BASE ATIVA", 0, COR_TEXTO_PRINCIPAL)
        lblKpiGMV = CriarKpiCard(pnlKpis, "VOLUME TRANSACIONADO (GMV)", 285, COR_DESTAQUE)
        lblKpiMRR = CriarKpiCard(pnlKpis, "RECEITA (MRR ESTIMADO)", 570, Color.FromArgb(234, 179, 8))
        lblKpiInadimplentes = CriarKpiCard(pnlKpis, "PLANOS VENCIDOS", 855, COR_VERM)
        Me.Controls.Add(pnlKpis)

        'ARRA DE FILTRO
        Dim pnlFiltro As New Panel() With {.Location = New Point(25, 130), .Size = New Size(650, 55), .BackColor = COR_CARD}
        AddHandler pnlFiltro.Paint, AddressOf DesenharCardPainel

        Dim lblBusca As New Label() With {.Text = "🔍 Buscar Operador:", .Location = New Point(15, 18), .AutoSize = True, .Font = New Font("Segoe UI", 9.5, FontStyle.Bold), .ForeColor = COR_TEXTO_MUTED}

        Dim pnlInputBusca As New Panel() With {.Location = New Point(160, 13), .Size = New Size(260, 30), .BackColor = COR_INPUT_BG}
        AddHandler pnlInputBusca.Paint, AddressOf DesenharBordaInputBusca

        txtBusca = New TextBox() With {.Location = New Point(10, 5), .Size = New Size(240, 20), .Font = New Font("Segoe UI", 10), .BackColor = COR_INPUT_BG, .ForeColor = COR_TEXTO_PRINCIPAL, .BorderStyle = BorderStyle.None}
        pnlInputBusca.Controls.Add(txtBusca)

        btnBuscar = New Button() With {.Text = "Filtrar", .Location = New Point(430, 13), .Size = New Size(90, 30), .BackColor = COR_DESTAQUE, .ForeColor = Color.FromArgb(11, 13, 17), .Font = New Font("Segoe UI", 9.5, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnBuscar.FlatAppearance.BorderSize = 0
        AddHandler btnBuscar.Paint, AddressOf ArredondarBotaoGDI
        AddHandler btnBuscar.Click, AddressOf btnBuscar_Click

        btnExportarCSV = New Button() With {.Text = "📥 Relatório", .Location = New Point(530, 13), .Size = New Size(100, 30), .BackColor = COR_BTN_DARK, .ForeColor = COR_VERDE, .Font = New Font("Segoe UI", 9, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnExportarCSV.FlatAppearance.BorderSize = 0
        AddHandler btnExportarCSV.Paint, AddressOf ArredondarBotaoGDI
        AddHandler btnExportarCSV.Click, AddressOf btnExportarCSV_Click

        pnlFiltro.Controls.AddRange(New Control() {lblBusca, pnlInputBusca, btnBuscar, btnExportarCSV})
        Me.Controls.Add(pnlFiltro)

        'DATAGRIDVIEW
        dgvUsuarios = New DataGridView() With {
            .Location = New Point(25, 200), .Size = New Size(650, 480), .AllowUserToAddRows = False, .AllowUserToDeleteRows = False, .ReadOnly = True,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect, .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .BackgroundColor = COR_CARD, .RowHeadersVisible = False, .BorderStyle = BorderStyle.None, .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            .GridColor = COR_BORDA_CARD, .EnableHeadersVisualStyles = False
        }
        dgvUsuarios.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        dgvUsuarios.ColumnHeadersDefaultCellStyle.BackColor = COR_INPUT_BG
        dgvUsuarios.ColumnHeadersDefaultCellStyle.ForeColor = COR_DESTAQUE
        dgvUsuarios.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.5, FontStyle.Bold)
        dgvUsuarios.ColumnHeadersHeight = 40

        Dim cellStyle As New DataGridViewCellStyle()
        cellStyle.BackColor = COR_CARD
        cellStyle.ForeColor = COR_TEXTO_PRINCIPAL
        cellStyle.SelectionBackColor = COR_BTN_DARK
        cellStyle.SelectionForeColor = COR_DESTAQUE
        cellStyle.Font = New Font("Segoe UI", 9.5)

        dgvUsuarios.DefaultCellStyle = cellStyle
        dgvUsuarios.RowsDefaultCellStyle = cellStyle
        AddHandler dgvUsuarios.SelectionChanged, AddressOf dgvUsuarios_SelectionChanged
        Me.Controls.Add(dgvUsuarios)

        'PAINEL DIREITO
        pnlAuditoriaLateral = New Panel() With {.Location = New Point(690, 130), .Size = New Size(470, 550), .BackColor = COR_CARD}
        AddHandler pnlAuditoriaLateral.Paint, AddressOf DesenharCardPainel

        Dim lblTitDet As New Label() With {.Text = "DOSSIÊ DO MOTORISTA", .Location = New Point(25, 20), .Size = New Size(420, 20), .Font = New Font("Segoe UI", 12, FontStyle.Bold), .ForeColor = COR_TEXTO_PRINCIPAL}
        Dim linhaDiv1 As New Label() With {.Location = New Point(25, 50), .Size = New Size(420, 1), .BackColor = COR_BORDA_CARD}
        pnlAuditoriaLateral.Controls.AddRange(New Control() {lblTitDet, linhaDiv1})

        lblDetNome = CriarLinhaDetalhe("Condutor:", 65, True, COR_TEXTO_PRINCIPAL)
        lblDetCpf = CriarLinhaDetalhe("Documento:", 90, False, COR_TEXTO_PRINCIPAL)
        lblDetEmail = CriarLinhaDetalhe("E-mail:", 115, False, COR_TEXTO_PRINCIPAL)
        lblDetStatus = CriarLinhaDetalhe("Status da Conta:", 140, True, COR_TEXTO_PRINCIPAL)

        Dim linhaDiv2 As New Label() With {.Location = New Point(25, 175), .Size = New Size(420, 1), .BackColor = COR_BORDA_CARD}
        pnlAuditoriaLateral.Controls.Add(linhaDiv2)

        Dim lblP = New Label() With {.Text = "Planos:", .Location = New Point(25, 195), .AutoSize = True, .Font = New Font("Segoe UI", 9.5), .ForeColor = COR_TEXTO_MUTED}
        cmbMudarPlano = New ComboBox() With {.Location = New Point(150, 192), .Size = New Size(150, 25), .DropDownStyle = ComboBoxStyle.DropDownList, .Font = New Font("Segoe UI", 9.5, FontStyle.Bold), .BackColor = COR_INPUT_BG, .ForeColor = COR_DESTAQUE, .FlatStyle = FlatStyle.Flat}
        cmbMudarPlano.Items.AddRange(New Object() {"ESSENTIAL", "PERFORMANCE", "BLACK"})
        AddHandler cmbMudarPlano.SelectedIndexChanged, AddressOf cmbMudarPlano_SelectedIndexChanged

        lblAdminShield = New Label() With {.Text = "👑 ADMIN 👑", .Location = New Point(150, 195), .Size = New Size(250, 20), .Font = New Font("Segoe UI", 10, FontStyle.Bold), .ForeColor = Color.FromArgb(234, 179, 8), .Visible = False}

        lblDetVencimento = CriarLinhaDetalhe("Vencimento:", 225, False, COR_TEXTO_PRINCIPAL)
        lblDetDiasRestantes = CriarLinhaDetalhe("Tempo Restante:", 250, True, COR_TEXTO_PRINCIPAL)

        pnlAuditoriaLateral.Controls.AddRange(New Control() {lblP, cmbMudarPlano, lblAdminShield})

        Dim linhaDiv3 As New Label() With {.Location = New Point(25, 285), .Size = New Size(420, 1), .BackColor = COR_BORDA_CARD}
        pnlAuditoriaLateral.Controls.Add(linhaDiv3)

        lblDetCarroAtivo = CriarLinhaDetalhe("Veículo Ativo:", 300, False, COR_TEXTO_PRINCIPAL)
        lblDetFaturamento = CriarLinhaDetalhe("Faturamento Total:", 325, True, COR_VERDE)
        lblDetCustos = CriarLinhaDetalhe("Despesas Totais:", 350, True, COR_VERM)
        lblDetLucroLiquido = CriarLinhaDetalhe("Balanço Total:", 375, True, COR_TEXTO_PRINCIPAL)

        'Campo de alerta Geral do Administrador
        Dim lblMsgM = New Label() With {.Text = "Notificação do Painel:", .Location = New Point(25, 405), .AutoSize = True, .Font = New Font("Segoe UI", 8.5, FontStyle.Bold), .ForeColor = COR_TEXTO_MUTED}
        txtMensagemAdmin = New TextBox() With {.Location = New Point(150, 402), .Size = New Size(200, 23), .BackColor = COR_INPUT_BG, .ForeColor = COR_TEXTO_PRINCIPAL, .BorderStyle = BorderStyle.FixedSingle, .Font = New Font("Segoe UI", 9)}
        btnSalvarMensagem = New Button() With {.Text = "OK", .Location = New Point(355, 401), .Size = New Size(40, 24), .BackColor = COR_BTN_DARK, .ForeColor = COR_DESTAQUE, .Font = New Font("Segoe UI", 8, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnSalvarMensagem.FlatAppearance.BorderSize = 0
        AddHandler btnSalvarMensagem.Click, AddressOf btnSalvarMensagem_Click
        pnlAuditoriaLateral.Controls.AddRange(New Control() {lblMsgM, txtMensagemAdmin, btnSalvarMensagem})

        pnlDangerZone = New Panel() With {.Location = New Point(15, 445), .Size = New Size(440, 100), .BackColor = Color.Transparent}

        btnAlternarStatus = New Button() With {.Location = New Point(10, 5), .Size = New Size(205, 38), .Font = New Font("Segoe UI", 9.5, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnAlternarStatus.FlatAppearance.BorderSize = 0
        AddHandler btnAlternarStatus.Paint, AddressOf ArredondarBotaoGDI
        AddHandler btnAlternarStatus.Click, AddressOf btnAlternarStatus_Click

        btnResetSenha = New Button() With {.Text = "🔑 Reset Senha", .Location = New Point(225, 5), .Size = New Size(205, 38), .BackColor = COR_BTN_DARK, .ForeColor = COR_TEXTO_PRINCIPAL, .Font = New Font("Segoe UI", 9), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnResetSenha.FlatAppearance.BorderSize = 0
        AddHandler btnResetSenha.Paint, AddressOf ArredondarBotaoGDI
        AddHandler btnResetSenha.Click, AddressOf btnResetSenha_Click

        btnDeletarUsuario = New Button() With {.Text = "⚠️ Remover Conta e Dados", .Location = New Point(10, 50), .Size = New Size(420, 38), .BackColor = Color.FromArgb(60, 20, 25), .ForeColor = COR_VERM, .Font = New Font("Segoe UI", 9.5, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnDeletarUsuario.FlatAppearance.BorderSize = 0
        AddHandler btnDeletarUsuario.Paint, AddressOf ArredondarBotaoGDI
        AddHandler btnDeletarUsuario.Click, AddressOf btnDeletarUsuario_Click

        pnlDangerZone.Controls.AddRange(New Control() {btnAlternarStatus, btnResetSenha, btnDeletarUsuario})
        pnlAuditoriaLateral.Controls.Add(pnlDangerZone)
        Me.Controls.Add(pnlAuditoriaLateral)

        'RODAPÉ
        lblStatusRodape = New Label() With {.Location = New Point(25, 705), .Size = New Size(600, 20), .Font = New Font("Segoe UI", 9.5, FontStyle.Italic), .ForeColor = COR_TEXTO_MUTED}
        btnVoltar = New Button() With {.Text = "Encerrar Sessão e Sair", .Location = New Point(940, 695), .Size = New Size(220, 42), .BackColor = COR_BTN_DARK, .ForeColor = COR_TEXTO_PRINCIPAL, .Font = New Font("Segoe UI", 10, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnVoltar.FlatAppearance.BorderSize = 0
        AddHandler btnVoltar.Paint, AddressOf ArredondarBotaoGDI
        AddHandler btnVoltar.Click, AddressOf FecharFormulario

        Me.Controls.AddRange(New Control() {lblStatusRodape, btnVoltar})
    End Sub

    Private Function CriarKpiCard(pnlPai As Panel, titulo As String, x As Integer, corVal As Color) As Label
        Dim pnl As New Panel() With {.Location = New Point(x, 0), .Size = New Size(275, 90), .BackColor = COR_CARD}
        AddHandler pnl.Paint, AddressOf DesenharCardPainel
        Dim lblTit As New Label() With {.Text = titulo, .Location = New Point(20, 18), .Size = New Size(240, 15), .Font = New Font("Segoe UI", 8.5, FontStyle.Bold), .ForeColor = COR_TEXTO_MUTED, .BackColor = Color.Transparent}
        Dim lblVal As New Label() With {.Text = "---", .Location = New Point(18, 42), .Size = New Size(240, 35), .Font = New Font("Segoe UI", 18, FontStyle.Bold), .ForeColor = corVal, .TextAlign = ContentAlignment.MiddleLeft, .BackColor = Color.Transparent}
        pnl.Controls.AddRange(New Control() {lblTit, lblVal})
        pnlPai.Controls.Add(pnl)
        Return lblVal
    End Function

    Private Function CriarLinhaDetalhe(rotulo As String, y As Integer, isDestaque As Boolean, corValor As Color) As Label
        Dim lblRot As New Label() With {.Text = rotulo, .Location = New Point(25, y), .AutoSize = True, .Font = New Font("Segoe UI", 9.5), .ForeColor = COR_TEXTO_MUTED, .BackColor = Color.Transparent}
        Dim lblVal As New Label() With {.Location = New Point(185, y), .Size = New Size(260, 20), .Font = New Font("Segoe UI", 10, If(isDestaque, FontStyle.Bold, FontStyle.Regular)), .ForeColor = corValor, .BackColor = Color.Transparent}
        pnlAuditoriaLateral.Controls.AddRange(New Control() {lblRot, lblVal})
        Return lblVal
    End Function

    Private Sub DesenharCardPainel(sender As Object, e As PaintEventArgs)
        Dim pnl As Panel = DirectCast(sender, Panel)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Dim r As Integer = 8
        Dim path As New GraphicsPath()
        Dim w As Integer = pnl.Width - 1
        Dim h As Integer = pnl.Height - 1
        path.AddArc(0, 0, r * 2, r * 2, 180, 90)
        path.AddArc(w - (r * 2), 0, r * 2, r * 2, 270, 90)
        path.AddArc(w - (r * 2), h - (r * 2), r * 2, r * 2, 0, 90)
        path.AddArc(0, h - (r * 2), r * 2, r * 2, 90, 90)
        path.CloseAllFigures()
        Using p As New Pen(COR_BORDA_CARD, 1.2F) : e.Graphics.DrawPath(p, path) : End Using
    End Sub

    Private Sub DesenharBordaInputBusca(sender As Object, e As PaintEventArgs)
        Dim pnl As Panel = DirectCast(sender, Panel)
        Using p As New Pen(COR_BORDA_CARD, 1) : e.Graphics.DrawRectangle(p, 0, 0, pnl.Width - 1, pnl.Height - 1) : End Using
    End Sub

    Private Sub ArredondarBotaoGDI(sender As Object, e As PaintEventArgs)
        Dim btn As Button = DirectCast(sender, Button)
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

        Using b As New SolidBrush(btn.BackColor) : e.Graphics.FillPath(b, path) : End Using
        TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, New Rectangle(0, 0, btn.Width, btn.Height), btn.ForeColor, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
    End Sub

    Private Sub FecharFormulario(sender As Object, e As EventArgs)
        Me.Close()
    End Sub

    'ANÁLISE E CONTROLE DE DADOS
    Private Sub CarregarTodosUsuariosEIndicadores()
        Using db As New AppDbContext()
            Dim lista = db.Usuarios.Select(Function(u) New With {.ID = u.Id, .Nome = u.Nome, .CPF = u.CPF, .Email = u.Email}).ToList()
            dgvUsuarios.DataSource = lista

            lblKpiMotoristas.Text = lista.Count.ToString()

            'Soma executada diretamente no banco de dados para evitar estouro de memória
            Dim totalGMV As Decimal = 0
            Try
                totalGMV = db.Lancamentos.Sum(Function(l) l.ValorBruto)
            Catch
            End Try
            lblKpiGMV.Text = totalGMV.ToString("C2")

            Dim totalMRR As Decimal = 0
            Dim vencidos As Integer = 0
            Try
                Dim usuariosNoBanco = db.Usuarios.ToList()
                For Each user In usuariosNoBanco
                    Dim p As String = ""
                    Try : p = CallByName(user, "CategoriaPlano", CallType.Get).ToString().ToUpper() : Catch : End Try

                    Select Case p
                        Case "BLACK", "OURO" : totalMRR += 29.9D
                        Case "PERFORMANCE", "PRATA" : totalMRR += 14.9D
                    End Select

                    Dim dtVenc As DateTime = DateTime.MaxValue
                    Try : dtVenc = Convert.ToDateTime(CallByName(user, "DataVencimentoPlano", CallType.Get)) : Catch : End Try

                    If dtVenc < DateTime.Today AndAlso p <> "ESSENTIAL" AndAlso p <> "BRONZE" Then
                        vencidos += 1
                    End If
                Next
            Catch
            End Try

            lblKpiMRR.Text = totalMRR.ToString("C2")
            lblKpiInadimplentes.Text = vencidos.ToString()
            lblStatusRodape.Text = $"Conexão segura. Operadores na base: {lista.Count}"
        End Using
    End Sub

    Private Sub btnBuscar_Click(sender As Object, e As EventArgs)
        Dim busca As String = txtBusca.Text.Trim().ToLower()
        Using db As New AppDbContext()
            Dim query = db.Usuarios.AsQueryable()
            If Not String.IsNullOrEmpty(busca) Then
                Dim idInt As Integer
                If Integer.TryParse(busca, idInt) Then
                    query = query.Where(Function(u) u.Id = idInt)
                Else
                    query = query.Where(Function(u) u.Nome.ToLower().Contains(busca) OrElse u.CPF.Contains(busca) OrElse u.Email.ToLower().Contains(busca))
                End If
            End If
            Dim res = query.Select(Function(u) New With {.ID = u.Id, .Nome = u.Nome, .CPF = u.CPF, .Email = u.Email}).ToList()
            dgvUsuarios.DataSource = res
            lblStatusRodape.Text = $"Pesquisa concluída. Resultados: {res.Count}"
        End Using
    End Sub

    Private Sub btnExportarCSV_Click(sender As Object, e As EventArgs)
        If dgvUsuarios.Rows.Count = 0 Then
            MessageBox.Show("A base de dados está vazia.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning) : Return
        End If

        Dim sfd As New SaveFileDialog() With {.Filter = "Arquivo CSV (*.csv)|*.csv", .FileName = $"Relatorio_Financeiro_Condutores_{DateTime.Now.ToString("yyyyMMdd")}.csv"}
        If sfd.ShowDialog() = DialogResult.OK Then
            Dim sb As New StringBuilder()
            sb.AppendLine("ID;NOME;CPF;EMAIL")
            For Each row As DataGridViewRow In dgvUsuarios.Rows
                sb.AppendLine($"{row.Cells(0).Value};{row.Cells(1).Value};{row.Cells(2).Value};{row.Cells(3).Value}")
            Next
            File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8)
            MessageBox.Show("Exportação financeira gerada com sucesso para ERP/CRM.", "Exportação SaaS", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub dgvUsuarios_SelectionChanged(sender As Object, e As EventArgs)
        If dgvUsuarios.CurrentRow Is Nothing Then Return
        Dim idSelecionado As Integer = Convert.ToInt32(dgvUsuarios.CurrentRow.Cells("ID").Value)

        Using db As New AppDbContext()
            Dim u = db.Usuarios.FirstOrDefault(Function(user) user.Id = idSelecionado)
            If u Is Nothing Then Return

            Dim isAdmin As Boolean = (u.Id = _usuarioIdLogado OrElse u.Email.ToLower().Contains("admin"))

            pnlDangerZone.Visible = Not isAdmin
            cmbMudarPlano.Visible = Not isAdmin
            lblAdminShield.Visible = isAdmin

            Dim carroModelo As String = "Nenhum Registrado"
            Try
                Dim carroAtivo = db.Veiculos.FirstOrDefault(Function(v) v.UsuarioId = idSelecionado AndAlso v.Ativo)
                If carroAtivo IsNot Nothing Then carroModelo = carroAtivo.Modelo
            Catch
            End Try

            lblDetNome.Text = u.Nome
            lblDetEmail.Text = u.Email
            lblDetCpf.Text = If(String.IsNullOrEmpty(u.CPF), "Não Informado", u.CPF)
            lblDetCarroAtivo.Text = carroModelo

            ' Carrega texto da notificação salva no banco
            Try : txtMensagemAdmin.Text = CallByName(u, "MensagemAdmin", CallType.Get).ToString() : Catch : txtMensagemAdmin.Clear() : End Try

            Dim statusAtivo As Boolean = True
            Dim planoNome As String = "ESSENTIAL"
            Dim dataVencimento As DateTime = DateTime.MaxValue

            Try
                statusAtivo = Convert.ToBoolean(CallByName(u, "Ativo", CallType.Get))
                Dim pBanco As String = CallByName(u, "CategoriaPlano", CallType.Get).ToString().ToUpper()
                Select Case pBanco
                    Case "BRONZE", "ESSENTIAL" : planoNome = "ESSENTIAL"
                    Case "PRATA", "PERFORMANCE" : planoNome = "PERFORMANCE"
                    Case "OURO", "BLACK" : planoNome = "BLACK"
                    Case Else : planoNome = pBanco
                End Select
                Try : dataVencimento = Convert.ToDateTime(CallByName(u, "DataVencimentoPlano", CallType.Get)) : Catch : End Try
            Catch
            End Try

            lblDetStatus.Text = If(statusAtivo, "✅ CONTA ATIVA", "❌ CONTA SUSPENSA")
            lblDetStatus.ForeColor = If(statusAtivo, COR_VERDE, COR_VERM)

            If Not isAdmin Then
                RemoveHandler cmbMudarPlano.SelectedIndexChanged, AddressOf cmbMudarPlano_SelectedIndexChanged
                cmbMudarPlano.SelectedItem = planoNome
                AddHandler cmbMudarPlano.SelectedIndexChanged, AddressOf cmbMudarPlano_SelectedIndexChanged

                If planoNome = "ESSENTIAL" Then
                    lblDetVencimento.Text = "Vitalício (Grátis)"
                    lblDetDiasRestantes.Text = "Ilimitado"
                    lblDetDiasRestantes.ForeColor = COR_TEXTO_MUTED
                Else
                    lblDetVencimento.Text = dataVencimento.ToShortDateString()
                    Dim dias As Double = (dataVencimento.Date - DateTime.Today).TotalDays
                    If dias < 0 Then
                        lblDetDiasRestantes.Text = $"Vencido há {Math.Abs(dias)} dias"
                        lblDetDiasRestantes.ForeColor = COR_VERM
                    ElseIf dias <= 5 Then
                        lblDetDiasRestantes.Text = $"{dias} dias (Risco)"
                        lblDetDiasRestantes.ForeColor = Color.FromArgb(234, 179, 8)
                    Else
                        lblDetDiasRestantes.Text = $"{dias} dias restantes"
                        lblDetDiasRestantes.ForeColor = COR_VERDE
                    End If
                End If
            Else
                lblDetVencimento.Text = "---"
                lblDetDiasRestantes.Text = "---"
            End If

            ' Balanço Financeiro
            Dim fat As Decimal = db.Lancamentos.Where(Function(l) l.UsuarioId = idSelecionado).Sum(Function(l) l.ValorBruto)
            Dim desp As Decimal = db.Despesas.Where(Function(d) d.UsuarioId = idSelecionado AndAlso Not If(d.Descricao, "").StartsWith("[RECEITA]")).Sum(Function(d) d.Valor)
            Dim lucro As Decimal = fat - desp

            lblDetFaturamento.Text = fat.ToString("C2")
            lblDetCustos.Text = desp.ToString("C2")
            lblDetLucroLiquido.Text = lucro.ToString("C2")
            lblDetLucroLiquido.ForeColor = If(lucro >= 0, COR_DESTAQUE, COR_VERM)

            btnAlternarStatus.Text = If(statusAtivo, "🚫 Suspender Acesso", "✅ Reativar Acesso")
            btnAlternarStatus.BackColor = If(statusAtivo, Color.FromArgb(100, 30, 30), COR_VERDE)
            btnAlternarStatus.ForeColor = If(statusAtivo, Color.White, Color.FromArgb(11, 13, 17))
        End Using
    End Sub

    Private Sub btnSalvarMensagem_Click(sender As Object, e As EventArgs)
        If dgvUsuarios.CurrentRow Is Nothing Then Return
        Dim id As Integer = Convert.ToInt32(dgvUsuarios.CurrentRow.Cells("ID").Value)
        Using db As New AppDbContext()
            Dim u = db.Usuarios.FirstOrDefault(Function(user) user.Id = id)
            If u IsNot Nothing Then
                Try
                    CallByName(u, "MensagemAdmin", CallType.Set, txtMensagemAdmin.Text.Trim())
                    db.SaveChanges()
                    MessageBox.Show("Notificação transmitida e salva na nuvem do operador.", "Flow Road Core", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch
                End Try
            End If
        End Using
    End Sub

    Private Sub cmbMudarPlano_SelectedIndexChanged(sender As Object, e As EventArgs)
        If dgvUsuarios.CurrentRow Is Nothing Then Return
        Dim id As Integer = Convert.ToInt32(dgvUsuarios.CurrentRow.Cells("ID").Value)
        Using db As New AppDbContext()
            Dim u = db.Usuarios.FirstOrDefault(Function(user) user.Id = id)
            If u IsNot Nothing Then
                Try
                    Dim novoPlano As String = cmbMudarPlano.SelectedItem.ToString()
                    CallByName(u, "CategoriaPlano", CallType.Set, novoPlano)
                    If novoPlano <> "ESSENTIAL" Then
                        CallByName(u, "DataVencimentoPlano", CallType.Set, DateTime.Today.AddDays(30))
                    End If
                    db.SaveChanges()
                    MessageBox.Show($"Assinatura alterada manualmente para {novoPlano}.", "Governança", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    'atualização dos KPIs em tempo real
                    CarregarTodosUsuariosEIndicadores()
                    dgvUsuarios_SelectionChanged(Nothing, Nothing)
                Catch
                End Try
            End If
        End Using
    End Sub

    Private Sub btnAlternarStatus_Click(sender As Object, e As EventArgs)
        If dgvUsuarios.CurrentRow Is Nothing Then Return
        Dim id As Integer = Convert.ToInt32(dgvUsuarios.CurrentRow.Cells("ID").Value)
        Using db As New AppDbContext()
            Dim u = db.Usuarios.FirstOrDefault(Function(user) user.Id = id)
            If u IsNot Nothing Then
                Try
                    Dim statusAtual As Boolean = Convert.ToBoolean(CallByName(u, "Ativo", CallType.Get))
                    CallByName(u, "Ativo", CallType.Set, Not statusAtual)
                    db.SaveChanges()

                    dgvUsuarios_SelectionChanged(Nothing, Nothing)
                Catch
                End Try
            End If
        End Using
    End Sub

    Private Sub btnResetSenha_Click(sender As Object, e As EventArgs)
        If dgvUsuarios.CurrentRow Is Nothing Then Return
        Dim id As Integer = Convert.ToInt32(dgvUsuarios.CurrentRow.Cells("ID").Value)
        If MessageBox.Show("Isso redefinirá a senha do condutor para '123456'. Deseja prosseguir?", "Protocolo de Segurança", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then
            Using db As New AppDbContext()
                Dim u = db.Usuarios.FirstOrDefault(Function(user) user.Id = id)
                If u IsNot Nothing Then
                    u.Senha = "123456"
                    db.SaveChanges()
                    MessageBox.Show("Credenciais redefinidas com sucesso.", "Auditoria", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using
        End If
    End Sub

    Private Sub btnDeletarUsuario_Click(sender As Object, e As EventArgs)
        If dgvUsuarios.CurrentRow Is Nothing Then Return
        Dim id As Integer = Convert.ToInt32(dgvUsuarios.CurrentRow.Cells("ID").Value)
        If MessageBox.Show("⚠️ ALERTA DE REMOÇÃO ⚠️" & vbCrLf & "Todos os dados financeiros, viagens e o registo deste cliente serão destruídos da nuvem permanentemente. Continuar?", "Ação Irreversível", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = DialogResult.Yes Then
            Using db As New AppDbContext()
                Dim u = db.Usuarios.FirstOrDefault(Function(user) user.Id = id)
                If u IsNot Nothing Then
                    db.Usuarios.Remove(u)
                    db.SaveChanges()
                    CarregarTodosUsuariosEIndicadores()
                End If
            End Using
        End If
    End Sub
End Class