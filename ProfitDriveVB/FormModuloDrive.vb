Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports System.Linq
Imports System.Collections.Generic
Imports System.Globalization
Imports System

Public Class FormModuloDrive
    Inherits Form

    Private _usuarioIdLogado As Integer

    Private pnlLancamento As Panel
    Private pnlExtrato As Panel
    Private pnlKpiKm, pnlKpiReceita, pnlKpiCusto, pnlKpiLucro As Panel

    Private dtpDataTurno As DateTimePicker
    Private cmbPlataforma As ComboBox
    Private txtKmRodados, txtReceitaBruta, txtCustoAbastecimento As TextBox
    Private btnProcessarTurno, btnExcluirTurno, btnVoltar As Button
    Private cmbMes, cmbAno As ComboBox

    Private lblKpiKmValor, lblKpiReceitaValor, lblKpiCustoValor, lblKpiLucroValor As Label

    Private dgvTurnos As DataGridView

    Private COR_CANVAS As Color = Color.FromArgb(15, 17, 21)
    Private COR_CARD As Color = Color.FromArgb(26, 32, 44)
    Private COR_TEXTO_PRINCIPAL As Color = Color.FromArgb(248, 250, 252)
    Private COR_TEXTO_MUTED As Color = Color.FromArgb(148, 163, 184)
    Private COR_DESTAQUE As Color = Color.FromArgb(56, 189, 248)
    Private COR_VERDE As Color = Color.FromArgb(16, 185, 129)
    Private COR_VERM As Color = Color.FromArgb(239, 68, 68)
    Private COR_BTN_DARK As Color = Color.FromArgb(30, 41, 59)

    Sub New(usuarioId As Integer)
        InitializeComponent()
        Me._usuarioIdLogado = usuarioId

        Me.Text = "Flow Road - Histórico de Turnos"
        Me.Size = New Size(980, 680)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.BackColor = COR_CANVAS

        ConstruirLayoutFintech()

        cmbMes.SelectedIndex = DateTime.Today.Month - 1
        cmbAno.SelectedItem = DateTime.Today.Year.ToString()

        AddHandler cmbMes.SelectedIndexChanged, Sub() AtualizarDadosTurnos()
        AddHandler cmbAno.SelectedIndexChanged, Sub() AtualizarDadosTurnos()

        AtualizarDadosTurnos()
    End Sub

    Private Sub ConstruirLayoutFintech()
        'KPIs HORIZONTAIS NO TOPO
        Dim CriarKpiCard = Function(titulo As String, x As Integer, corValor As Color, ByRef lblAlvo As Label) As Panel
                               Dim pnl As New Panel() With {.Location = New Point(x, 20), .Size = New Size(220, 75), .BackColor = COR_CARD}
                               AddHandler pnl.Paint, AddressOf EstilizarBordaPainelGlass
                               Dim lblTit As New Label() With {.Text = titulo, .Location = New Point(15, 12), .AutoSize = True, .Font = New Font("Segoe UI", 8.5, FontStyle.Bold), .ForeColor = COR_TEXTO_MUTED}
                               lblAlvo = New Label() With {.Text = "0", .Location = New Point(12, 34), .Size = New Size(195, 30), .Font = New Font("Segoe UI", 15, FontStyle.Bold), .ForeColor = corValor, .TextAlign = ContentAlignment.MiddleLeft}
                               pnl.Controls.AddRange(New Control() {lblTit, lblAlvo})
                               Me.Controls.Add(pnl)
                               Return pnl
                           End Function

        pnlKpiKm = CriarKpiCard("KM TOTAL RODADO", 20, COR_TEXTO_PRINCIPAL, lblKpiKmValor)
        pnlKpiReceita = CriarKpiCard("RECEITA BRUTA", 255, COR_DESTAQUE, lblKpiReceitaValor)
        pnlKpiCusto = CriarKpiCard("CUSTO DE OPERAÇÃO", 490, COR_VERM, lblKpiCustoValor)
        pnlKpiLucro = CriarKpiCard("LUCRO LÍQUIDO", 725, COR_VERDE, lblKpiLucroValor)

        'PAINEL LATERAL ESQUERDO: LANÇAMENTO OPERACIONAL
        pnlLancamento = New Panel() With {.Location = New Point(20, 115), .Size = New Size(340, 455), .BackColor = COR_CARD}
        AddHandler pnlLancamento.Paint, AddressOf EstilizarBordaPainelGlass

        Dim lblTitLanca As New Label() With {.Text = "Registrar Novo Turno", .Font = New Font("Segoe UI", 11, FontStyle.Bold), .ForeColor = COR_DESTAQUE, .Location = New Point(20, 20), .AutoSize = True}

        Dim yPos As Integer = 65
        Dim lblData = CriarLabelForm("Data de Operação:", yPos)
        dtpDataTurno = New DateTimePicker() With {.Location = New Point(20, yPos + 22), .Size = New Size(300, 25), .Format = DateTimePickerFormat.Short, .Font = New Font("Segoe UI", 10)}

        yPos += 65
        Dim lblPlataforma = CriarLabelForm("Plataforma / Canal:", yPos)
        cmbPlataforma = New ComboBox() With {.Location = New Point(20, yPos + 22), .Size = New Size(300, 25), .DropDownStyle = ComboBoxStyle.DropDownList, .BackColor = Color.FromArgb(15, 17, 21), .ForeColor = COR_TEXTO_PRINCIPAL, .FlatStyle = FlatStyle.Flat, .Font = New Font("Segoe UI", 10)}
        cmbPlataforma.Items.AddRange(New Object() {"Uber Black", "Uber Comfort", "UberX", "99App", "inDrive", "Particular"})
        cmbPlataforma.SelectedIndex = 0

        yPos += 65
        Dim lblKm = CriarLabelForm("Quilometragem do Turno:", yPos)
        txtKmRodados = CriarTextBoxForm(20, yPos + 22, 300)
        txtKmRodados.Text = "0,0"
        AddHandler txtKmRodados.TextChanged, AddressOf AplicarMascaraDecimal_TextChanged

        yPos += 65
        Dim lblReceita = CriarLabelForm("Receita Bruta Gerada (R$):", yPos)
        txtReceitaBruta = CriarTextBoxForm(20, yPos + 22, 300)
        txtReceitaBruta.Text = "R$ 0,00"
        AddHandler txtReceitaBruta.TextChanged, AddressOf AplicarMascaraMoeda_TextChanged

        yPos += 65
        Dim lblCusto = CriarLabelForm("Custo de Abastecimento (R$):", yPos)
        txtCustoAbastecimento = CriarTextBoxForm(20, yPos + 22, 300)
        txtCustoAbastecimento.Text = "R$ 0,00"
        AddHandler txtCustoAbastecimento.TextChanged, AddressOf AplicarMascaraMoeda_TextChanged

        btnProcessarTurno = New Button() With {.Text = "Processar Turno Operacional", .Location = New Point(20, 390), .Size = New Size(300, 42), .BackColor = COR_VERDE, .ForeColor = Color.White, .Font = New Font("Segoe UI", 10, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnProcessarTurno.FlatAppearance.BorderSize = 0
        AddHandler btnProcessarTurno.Paint, AddressOf ArredondarBotaoGDI
        AddHandler btnProcessarTurno.Click, AddressOf btnProcessarTurno_Click

        pnlLancamento.Controls.AddRange(New Control() {lblTitLanca, lblData, dtpDataTurno, lblPlataforma, cmbPlataforma, lblKm, txtKmRodados, lblReceita, txtReceitaBruta, lblCusto, txtCustoAbastecimento, btnProcessarTurno})
        Me.Controls.Add(pnlLancamento)

        'PAINEL DIREITO: EXTRATO DE JORNADAS
        pnlExtrato = New Panel() With {.Location = New Point(380, 115), .Size = New Size(565, 455), .BackColor = COR_CARD}
        AddHandler pnlExtrato.Paint, AddressOf EstilizarBordaPainelGlass

        Dim lblTitTab As New Label() With {.Text = "Histórico Mensal", .Font = New Font("Segoe UI", 10.5, FontStyle.Bold), .Location = New Point(15, 17), .AutoSize = True, .ForeColor = COR_TEXTO_PRINCIPAL}

        cmbMes = New ComboBox() With {.Location = New Point(380, 15), .Size = New Size(100, 25), .DropDownStyle = ComboBoxStyle.DropDownList, .Font = New Font("Segoe UI", 9), .BackColor = Color.FromArgb(15, 17, 21), .ForeColor = COR_TEXTO_PRINCIPAL, .FlatStyle = FlatStyle.Flat}
        cmbMes.Items.AddRange(New Object() {"Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"})
        cmbAno = New ComboBox() With {.Location = New Point(490, 15), .Size = New Size(60, 25), .DropDownStyle = ComboBoxStyle.DropDownList, .Font = New Font("Segoe UI", 9), .BackColor = Color.FromArgb(15, 17, 21), .ForeColor = COR_TEXTO_PRINCIPAL, .FlatStyle = FlatStyle.Flat}
        cmbAno.Items.AddRange(New Object() {"2024", "2025", "2026", "2027", "2028"})

        dgvTurnos = New DataGridView() With {
            .Location = New Point(15, 55),
            .Size = New Size(535, 385),
            .BackgroundColor = COR_CARD,
            .ForeColor = COR_TEXTO_PRINCIPAL,
            .ReadOnly = False,
            .AllowUserToAddRows = False, .AllowUserToDeleteRows = False, .AllowUserToResizeRows = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .RowHeadersVisible = False, .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .BorderStyle = BorderStyle.None, .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            .GridColor = Color.FromArgb(51, 65, 85), .EnableHeadersVisualStyles = False
        }
        dgvTurnos.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        dgvTurnos.ColumnHeadersDefaultCellStyle.BackColor = COR_CARD
        dgvTurnos.ColumnHeadersDefaultCellStyle.ForeColor = COR_TEXTO_MUTED
        dgvTurnos.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9, FontStyle.Bold)
        dgvTurnos.ColumnHeadersHeight = 35

        dgvTurnos.DefaultCellStyle.BackColor = COR_CARD
        dgvTurnos.DefaultCellStyle.ForeColor = COR_TEXTO_PRINCIPAL
        dgvTurnos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(45, 55, 72)
        dgvTurnos.DefaultCellStyle.SelectionForeColor = COR_TEXTO_PRINCIPAL
        dgvTurnos.DefaultCellStyle.Font = New Font("Segoe UI", 9)

        AddHandler dgvTurnos.DataBindingComplete, AddressOf dgvTurnos_DataBindingComplete
        pnlExtrato.Controls.AddRange(New Control() {lblTitTab, cmbMes, cmbAno, dgvTurnos})
        Me.Controls.Add(pnlExtrato)

        btnExcluirTurno = New Button() With {.Text = "🗑️ Remover Turnos Selecionados", .Location = New Point(380, 585), .Size = New Size(565, 42), .BackColor = Color.FromArgb(127, 29, 29), .ForeColor = Color.White, .Font = New Font("Segoe UI", 10, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnExcluirTurno.FlatAppearance.BorderSize = 0
        AddHandler btnExcluirTurno.Paint, AddressOf ArredondarBotaoGDI
        AddHandler btnExcluirTurno.Click, AddressOf btnExcluirTurno_Click
        Me.Controls.Add(btnExcluirTurno)

        btnVoltar = New Button() With {.Text = "Voltar ao Cockpit", .Location = New Point(20, 585), .Size = New Size(340, 42), .BackColor = COR_BTN_DARK, .ForeColor = COR_TEXTO_PRINCIPAL, .Font = New Font("Segoe UI", 10, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnVoltar.FlatAppearance.BorderSize = 0
        AddHandler btnVoltar.Paint, AddressOf ArredondarBotaoGDI
        AddHandler btnVoltar.Click, Sub() Me.Close()
        Me.Controls.Add(btnVoltar)
    End Sub

    Private Function CriarLabelForm(texto As String, y As Integer) As Label
        Return New Label() With {.Text = texto, .Location = New Point(20, y), .Size = New Size(300, 18), .Font = New Font("Segoe UI", 9), .ForeColor = COR_TEXTO_MUTED, .BackColor = Color.Transparent}
    End Function

    Private Function CriarTextBoxForm(x As Integer, y As Integer, w As Integer) As TextBox
        Return New TextBox() With {.Location = New Point(x, y), .Size = New Size(w, 25), .BackColor = Color.FromArgb(15, 17, 21), .ForeColor = COR_TEXTO_PRINCIPAL, .BorderStyle = BorderStyle.FixedSingle, .Font = New Font("Segoe UI", 10), .TextAlign = HorizontalAlignment.Right}
    End Function

    Private Sub EstilizarBordaPainelGlass(sender As Object, e As PaintEventArgs)
        Dim pnl = DirectCast(sender, Panel)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Using p As New Pen(Color.FromArgb(51, 65, 85), 1.2F) : e.Graphics.DrawRectangle(p, 0, 0, pnl.Width - 1, pnl.Height - 1) : End Using
    End Sub

    Private Sub ArredondarBotaoGDI(sender As Object, e As PaintEventArgs)
        Dim btn = DirectCast(sender, Button)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Dim r As Integer = 6
        Dim path As New GraphicsPath()
        path.AddArc(0, 0, r * 2, r * 2, 180, 90) : path.AddArc(btn.Width - (r * 2) - 1, 0, r * 2, r * 2, 270, 90)
        path.AddArc(btn.Width - (r * 2) - 1, btn.Height - (r * 2) - 1, r * 2, r * 2, 0, 90) : path.AddArc(0, btn.Height - (r * 2) - 1, r * 2, r * 2, 90, 90)
        path.CloseAllFigures() : btn.Region = New Region(path)
        Using b As New SolidBrush(btn.BackColor) : e.Graphics.FillPath(b, path) : End Using
        TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, New Rectangle(0, 0, btn.Width, btn.Height), btn.ForeColor, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
    End Sub

    Private Sub AplicarMascaraMoeda_TextChanged(sender As Object, e As EventArgs)
        Dim txt = DirectCast(sender, TextBox) : RemoveHandler txt.TextChanged, AddressOf AplicarMascaraMoeda_TextChanged
        Dim digitos = New String(txt.Text.Where(AddressOf Char.IsDigit).ToArray())
        Dim val As Decimal = 0
        If Not String.IsNullOrEmpty(digitos) Then val = Decimal.Parse(digitos) / 100D
        txt.Text = val.ToString("C2") : txt.SelectionStart = txt.Text.Length
        AddHandler txt.TextChanged, AddressOf AplicarMascaraMoeda_TextChanged
    End Sub

    Private Sub AplicarMascaraDecimal_TextChanged(sender As Object, e As EventArgs)
        Dim txt = DirectCast(sender, TextBox) : RemoveHandler txt.TextChanged, AddressOf AplicarMascaraDecimal_TextChanged
        Dim digitos = New String(txt.Text.Where(AddressOf Char.IsDigit).ToArray())
        Dim val As Decimal = 0
        If Not String.IsNullOrEmpty(digitos) Then val = Decimal.Parse(digitos) / 10D ' 1 casa decimal
        txt.Text = val.ToString("N1") : txt.SelectionStart = txt.Text.Length
        AddHandler txt.TextChanged, AddressOf AplicarMascaraDecimal_TextChanged
    End Sub

    Private Function ObterValorDecimalLimpo(textoBox As String) As Decimal
        Dim res As Decimal : Decimal.TryParse(textoBox.Replace("R$", "").Replace(".", "").Replace(",", ".").Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, res) : Return res
    End Function

    Private Sub AtualizarDadosTurnos()
        If cmbMes.SelectedIndex = -1 OrElse cmbAno.SelectedItem Is Nothing Then Return
        Dim mesAlvo As Integer = cmbMes.SelectedIndex + 1
        Dim anoAlvo As Integer = Convert.ToInt32(cmbAno.SelectedItem)

        Using db As New AppDbContext()
            Dim listaTurnos = db.Set(Of LancamentoDiario)().Where(Function(t) t.UsuarioId = _usuarioIdLogado AndAlso t.Data.Month = mesAlvo AndAlso t.Data.Year = anoAlvo).OrderByDescending(Function(t) t.Data).ToList()

            Dim somaKm As Decimal = listaTurnos.Sum(Function(t) t.KmRodados)
            Dim somaReceita As Decimal = listaTurnos.Sum(Function(t) t.ValorBruto)
            Dim somaCusto As Decimal = listaTurnos.Sum(Function(t) t.ValorCombustivel)
            Dim lucro As Decimal = somaReceita - somaCusto

            lblKpiKmValor.Text = somaKm.ToString("N1") & " KM"
            lblKpiReceitaValor.Text = somaReceita.ToString("C2")
            lblKpiCustoValor.Text = somaCusto.ToString("C2")
            lblKpiLucroValor.Text = lucro.ToString("C2")
            lblKpiLucroValor.ForeColor = If(lucro >= 0, COR_VERDE, COR_VERM)

            Dim itensGrid = listaTurnos.Select(Function(t) New With {
                                                   .Selecionar = False,
                                                   .ID = t.Id,
                                                   .Data = t.Data.ToShortDateString(),
                                                   .Plataforma = If(t.Canal, ""),
                                                   .KMs = t.KmRodados.ToString("N1") & " km",
                                                   .Receita = t.ValorBruto.ToString("C2"),
                                                   .Custo = t.ValorCombustivel.ToString("C2"),
                                                   .Lucro = (t.ValorBruto - t.ValorCombustivel).ToString("C2")
                                               }).ToList()

            dgvTurnos.DataSource = Nothing
            dgvTurnos.DataSource = itensGrid
        End Using
    End Sub

    Private Sub btnProcessarTurno_Click(sender As Object, e As EventArgs)
        Dim km As Decimal = ObterValorDecimalLimpo(txtKmRodados.Text)
        Dim receita As Decimal = ObterValorDecimalLimpo(txtReceitaBruta.Text)
        Dim custo As Decimal = ObterValorDecimalLimpo(txtCustoAbastecimento.Text)

        If km <= 0 OrElse receita <= 0 Then
            MessageBox.Show("Informe pelo menos a Quilometragem e a Receita Bruta para salvar o turno.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning) : Return
        End If

        Dim plataformaSelecionada As String = cmbPlataforma.SelectedItem.ToString()
        Dim dataEscolhida As DateTime = dtpDataTurno.Value

        Using db As New AppDbContext()
            Dim novoTurno As New LancamentoDiario() With {
                .UsuarioId = _usuarioIdLogado,
                .Data = dataEscolhida,
                .Canal = plataformaSelecionada,
                .KmRodados = km,
                .ValorBruto = receita,
                .ValorCombustivel = custo
            }
            db.Set(Of LancamentoDiario)().Add(novoTurno)
            db.SaveChanges()

            db.Despesas.Add(New Despesa() With {
                .UsuarioId = _usuarioIdLogado,
                .LancamentoId = novoTurno.Id,
                .Valor = receita,
                .Descricao = "[RECEITA] Faturamento App - " & plataformaSelecionada,
                .Categoria = "Faturamento App",
                .Data = dataEscolhida
            })

            If custo > 0 Then
                db.Despesas.Add(New Despesa() With {
                    .UsuarioId = _usuarioIdLogado,
                    .LancamentoId = novoTurno.Id,
                    .Valor = custo,
                    .Descricao = "Abastecimento" & plataformaSelecionada,
                    .Categoria = "Custos Carro",
                    .Data = dataEscolhida
                })
            End If

            db.SaveChanges()
        End Using

        txtKmRodados.Text = "0,0" : txtReceitaBruta.Text = "R$ 0,00" : txtCustoAbastecimento.Text = "R$ 0,00"
        dtpDataTurno.Value = DateTime.Today
        MessageBox.Show("Turno sincronizado na base e no módulo financeiro.", "Flow Road", MessageBoxButtons.OK, MessageBoxIcon.Information)
        AtualizarDadosTurnos()
    End Sub

    Private Sub btnExcluirTurno_Click(sender As Object, e As EventArgs)
        If dgvTurnos.Rows.Count = 0 Then Return

        Dim idsParaDeletar As New List(Of Integer)()
        For Each row As DataGridViewRow In dgvTurnos.Rows
            If Convert.ToBoolean(row.Cells("Selecionar").Value) = True Then
                idsParaDeletar.Add(Convert.ToInt32(row.Cells("ID").Value))
            End If
        Next

        If idsParaDeletar.Count = 0 Then
            MessageBox.Show("Marque a caixa de seleção dos turnos que deseja estornar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information) : Return
        End If

        If MessageBox.Show($"Confirma o estorno de {idsParaDeletar.Count} turno(s)?" & vbCrLf & "As despesas e receitas vinculadas na gestão financeira também serão removidas.", "Auditoria", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then
            Using db As New AppDbContext()
                For Each id In idsParaDeletar
                    db.Despesas.RemoveRange(db.Despesas.Where(Function(d) d.LancamentoId = id).ToList())
                    Try
                        Dim turno = db.Set(Of LancamentoDiario)().FirstOrDefault(Function(l) l.Id = id)
                        If turno IsNot Nothing Then db.Set(Of LancamentoDiario)().Remove(turno)
                    Catch : End Try
                Next
                db.SaveChanges()
            End Using
            AtualizarDadosTurnos()
        End If
    End Sub

    Private Sub dgvTurnos_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs)
        Try
            Dim dgv = DirectCast(sender, DataGridView)

            If dgv.Columns.Contains("Selecionar") Then
                dgv.Columns("Selecionar").Width = 70
                dgv.Columns("Selecionar").HeaderText = "Marcar"
                dgv.Columns("Selecionar").ReadOnly = False
            End If

            If dgv.Columns.Contains("ID") Then dgv.Columns("ID").Visible = False : dgv.Columns("ID").ReadOnly = True
            If dgv.Columns.Contains("Data") Then dgv.Columns("Data").Width = 80 : dgv.Columns("Data").ReadOnly = True
            If dgv.Columns.Contains("Plataforma") Then dgv.Columns("Plataforma").Width = 110 : dgv.Columns("Plataforma").ReadOnly = True

            Dim ConfigColNumeric = Sub(nomeCol As String, largura As Integer)
                                       If dgv.Columns.Contains(nomeCol) Then
                                           dgv.Columns(nomeCol).Width = largura
                                           dgv.Columns(nomeCol).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                                           dgv.Columns(nomeCol).ReadOnly = True
                                       End If
                                   End Sub

            ConfigColNumeric("KMs", 75)
            ConfigColNumeric("Receita", 85)
            ConfigColNumeric("Custo", 85)
            ConfigColNumeric("Lucro", 85)
        Catch : End Try
    End Sub
End Class