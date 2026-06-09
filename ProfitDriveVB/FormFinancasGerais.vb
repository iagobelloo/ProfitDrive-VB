Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports System.Linq
Imports System.Collections.Generic
Imports System.Globalization
Imports System

Public Class FormFinancasGerais
    Inherits Form

    Private _usuarioIdLogado As Integer

    Private pnlLancamento As Panel
    Private pnlExtrato As Panel
    Private pnlKpiReceitas, pnlKpiDespesas, pnlKpiSaldo As Panel

    'Componentes de Entrada
    Private cmbTipoLancamento As ComboBox
    Private txtValorLancamento, txtDescLancamento As TextBox
    Private dtpDataLancamento As DateTimePicker
    Private cmbCategoriaLancamento, cmbMes, cmbAno As ComboBox
    Private btnSalvarLancamento, btnExcluirDespesa, btnVoltar As Button

    'Filtro Rápido
    Private btnFiltroTodos, btnFiltroReceitas, btnFiltroDespesas As Button
    Private _filtroAtual As String = "TODOS" ' TODOS, RECEITAS, DESPESAS

    Private lblKpiReceitasValor, lblKpiDespesasValor, lblKpiSaldoValor As Label

    Private dgvGastosPessoais As DataGridView

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

        Me.Text = "Flow Road - Gestão Financeira Corporativa"
        Me.Size = New Size(980, 680)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.BackColor = COR_CANVAS

        ConstruirLayoutFintech()

        cmbMes.SelectedIndex = DateTime.Today.Month - 1
        cmbAno.SelectedItem = DateTime.Today.Year.ToString()

        AddHandler cmbMes.SelectedIndexChanged, Sub() AtualizarDadosFinanceiros()
        AddHandler cmbAno.SelectedIndexChanged, Sub() AtualizarDadosFinanceiros()

        AtualizarDadosFinanceiros()
    End Sub

    Private Sub ConstruirLayoutFintech()
        'KPIs HORIZONTAIS NO TOPO
        Dim CriarKpiCard = Function(titulo As String, x As Integer, corValor As Color, ByRef lblAlvo As Label) As Panel
                               Dim pnl As New Panel() With {.Location = New Point(x, 20), .Size = New Size(185, 75), .BackColor = COR_CARD}
                               AddHandler pnl.Paint, AddressOf EstilizarBordaPainelGlass
                               Dim lblTit As New Label() With {.Text = titulo, .Location = New Point(15, 12), .AutoSize = True, .Font = New Font("Segoe UI", 8.5, FontStyle.Bold), .ForeColor = COR_TEXTO_MUTED}
                               lblAlvo = New Label() With {.Text = "R$ 0,00", .Location = New Point(12, 34), .Size = New Size(160, 30), .Font = New Font("Segoe UI", 15, FontStyle.Bold), .ForeColor = corValor, .TextAlign = ContentAlignment.MiddleLeft}
                               pnl.Controls.AddRange(New Control() {lblTit, lblAlvo})
                               Me.Controls.Add(pnl)
                               Return pnl
                           End Function

        pnlKpiReceitas = CriarKpiCard("RECEITAS DO MÊS", 380, COR_VERDE, lblKpiReceitasValor)
        pnlKpiDespesas = CriarKpiCard("DESPESAS DO MÊS", 575, COR_VERM, lblKpiDespesasValor)
        pnlKpiSaldo = CriarKpiCard("SALDO LÍQUIDO", 770, COR_TEXTO_PRINCIPAL, lblKpiSaldoValor)

        'PAINEL LATERAL ESQUERDO: PROCESSAMENTO DE ENTRADAS/SAÍDAS
        pnlLancamento = New Panel() With {.Location = New Point(20, 20), .Size = New Size(340, 550), .BackColor = COR_CARD}
        AddHandler pnlLancamento.Paint, AddressOf EstilizarBordaPainelGlass

        Dim lblTitLanca As New Label() With {.Text = "Lançamento Contábil", .Font = New Font("Segoe UI", 11, FontStyle.Bold), .ForeColor = COR_DESTAQUE, .Location = New Point(20, 20), .AutoSize = True}

        Dim yPos As Integer = 65
        Dim lblTipo = CriarLabelForm("Natureza da Operação:", yPos)
        cmbTipoLancamento = New ComboBox() With {.Location = New Point(20, yPos + 22), .Size = New Size(300, 25), .DropDownStyle = ComboBoxStyle.DropDownList, .BackColor = Color.FromArgb(15, 17, 21), .ForeColor = COR_TEXTO_PRINCIPAL, .FlatStyle = FlatStyle.Flat, .Font = New Font("Segoe UI", 10)}
        cmbTipoLancamento.Items.AddRange(New Object() {"Despesa Operacional", "Receita / Entrada"})
        cmbTipoLancamento.SelectedIndex = 0
        AddHandler cmbTipoLancamento.SelectedIndexChanged, AddressOf cmbTipoLancamento_SelectedIndexChanged

        yPos += 65
        Dim lblVal = CriarLabelForm("Valor Total (R$):", yPos)
        txtValorLancamento = CriarTextBoxForm(20, yPos + 22, 300)
        txtValorLancamento.Text = "R$ 0,00"
        AddHandler txtValorLancamento.TextChanged, AddressOf AplicarMascaraMoeda_TextChanged

        yPos += 65
        Dim lblData = CriarLabelForm("Data de Competência (Retroativo):", yPos)
        dtpDataLancamento = New DateTimePicker() With {.Location = New Point(20, yPos + 22), .Size = New Size(300, 25), .Format = DateTimePickerFormat.Short, .Font = New Font("Segoe UI", 10), .CalendarMonthBackground = COR_CARD, .CalendarForeColor = COR_TEXTO_PRINCIPAL}

        yPos += 65
        Dim lblDesc = CriarLabelForm("Descrição do Registro:", yPos)
        txtDescLancamento = CriarTextBoxForm(20, yPos + 22, 300)
        txtDescLancamento.TextAlign = HorizontalAlignment.Left

        yPos += 65
        Dim lblCat = CriarLabelForm("Categoria:", yPos)
        cmbCategoriaLancamento = New ComboBox() With {.Location = New Point(20, yPos + 22), .Size = New Size(300, 25), .DropDownStyle = ComboBoxStyle.DropDownList, .BackColor = Color.FromArgb(15, 17, 21), .ForeColor = COR_TEXTO_PRINCIPAL, .FlatStyle = FlatStyle.Flat, .Font = New Font("Segoe UI", 10)}
        AtualizarCategoriasDisponiveis()

        btnSalvarLancamento = New Button() With {.Text = "Processar Despesa", .Location = New Point(20, 480), .Size = New Size(300, 42), .BackColor = COR_VERM, .ForeColor = Color.White, .Font = New Font("Segoe UI", 10, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnSalvarLancamento.FlatAppearance.BorderSize = 0
        AddHandler btnSalvarLancamento.Paint, AddressOf ArredondarBotaoGDI
        AddHandler btnSalvarLancamento.Click, AddressOf btnSalvarLancamento_Click

        pnlLancamento.Controls.AddRange(New Control() {lblTitLanca, lblTipo, cmbTipoLancamento, lblVal, txtValorLancamento, lblData, dtpDataLancamento, lblDesc, txtDescLancamento, lblCat, cmbCategoriaLancamento, btnSalvarLancamento})
        Me.Controls.Add(pnlLancamento)

        'PAINEL DIREITO: EXTRATO
        pnlExtrato = New Panel() With {.Location = New Point(380, 115), .Size = New Size(575, 455), .BackColor = COR_CARD}
        AddHandler pnlExtrato.Paint, AddressOf EstilizarBordaPainelGlass

        cmbMes = New ComboBox() With {.Location = New Point(380, 15), .Size = New Size(110, 25), .DropDownStyle = ComboBoxStyle.DropDownList, .Font = New Font("Segoe UI", 9), .BackColor = Color.FromArgb(15, 17, 21), .ForeColor = COR_TEXTO_PRINCIPAL, .FlatStyle = FlatStyle.Flat}
        cmbMes.Items.AddRange(New Object() {"Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"})
        cmbAno = New ComboBox() With {.Location = New Point(495, 15), .Size = New Size(65, 25), .DropDownStyle = ComboBoxStyle.DropDownList, .Font = New Font("Segoe UI", 9), .BackColor = Color.FromArgb(15, 17, 21), .ForeColor = COR_TEXTO_PRINCIPAL, .FlatStyle = FlatStyle.Flat}
        cmbAno.Items.AddRange(New Object() {"2024", "2025", "2026", "2027", "2028"})

        Dim ConfigurarChipFiltro = Sub(btn As Button, txt As String, x As Integer)
                                       btn.Text = txt : btn.Location = New Point(x, 14) : btn.Size = New Size(75, 25)
                                       btn.FlatStyle = FlatStyle.Flat : btn.FlatAppearance.BorderSize = 0
                                       btn.Font = New Font("Segoe UI", 8.5, FontStyle.Bold) : btn.Cursor = Cursors.Hand
                                   End Sub
        btnFiltroTodos = New Button() : ConfigurarChipFiltro(btnFiltroTodos, "Todos", 15)
        btnFiltroTodos.BackColor = COR_BTN_DARK : btnFiltroTodos.ForeColor = COR_TEXTO_PRINCIPAL
        AddHandler btnFiltroTodos.Click, Sub() MudarFiltroActivo("TODOS")

        btnFiltroReceitas = New Button() : ConfigurarChipFiltro(btnFiltroReceitas, "Receitas", 95)
        btnFiltroReceitas.BackColor = Color.Transparent : btnFiltroReceitas.ForeColor = COR_TEXTO_MUTED
        AddHandler btnFiltroReceitas.Click, Sub() MudarFiltroActivo("RECEITAS")

        btnFiltroDespesas = New Button() : ConfigurarChipFiltro(btnFiltroDespesas, "Despesas", 175)
        btnFiltroDespesas.BackColor = Color.Transparent : btnFiltroDespesas.ForeColor = COR_TEXTO_MUTED
        AddHandler btnFiltroDespesas.Click, Sub() MudarFiltroActivo("DESPESAS")

        pnlExtrato.Controls.AddRange(New Control() {cmbMes, cmbAno, btnFiltroTodos, btnFiltroReceitas, btnFiltroDespesas})

        ' Tabela Grid Dinâmica
        dgvGastosPessoais = New DataGridView() With {
            .Location = New Point(15, 55),
            .Size = New Size(545, 385),
            .BackgroundColor = COR_CARD,
            .ForeColor = COR_TEXTO_PRINCIPAL,
            .ReadOnly = False, ' Permitir editar para marcar as caixas de seleção
            .AllowUserToAddRows = False, .AllowUserToDeleteRows = False, .AllowUserToResizeRows = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .RowHeadersVisible = False, .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .BorderStyle = BorderStyle.None, .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            .GridColor = Color.FromArgb(51, 65, 85), .EnableHeadersVisualStyles = False
        }
        dgvGastosPessoais.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        dgvGastosPessoais.ColumnHeadersDefaultCellStyle.BackColor = COR_CARD
        dgvGastosPessoais.ColumnHeadersDefaultCellStyle.ForeColor = COR_TEXTO_MUTED
        dgvGastosPessoais.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9, FontStyle.Bold)
        dgvGastosPessoais.ColumnHeadersHeight = 35

        dgvGastosPessoais.DefaultCellStyle.BackColor = COR_CARD
        dgvGastosPessoais.DefaultCellStyle.ForeColor = COR_TEXTO_PRINCIPAL
        dgvGastosPessoais.DefaultCellStyle.SelectionBackColor = Color.FromArgb(45, 55, 72)
        dgvGastosPessoais.DefaultCellStyle.SelectionForeColor = COR_TEXTO_PRINCIPAL
        dgvGastosPessoais.DefaultCellStyle.Font = New Font("Segoe UI", 9)

        AddHandler dgvGastosPessoais.DataBindingComplete, AddressOf dgvGastosPessoais_DataBindingComplete
        pnlExtrato.Controls.Add(dgvGastosPessoais)
        Me.Controls.Add(pnlExtrato)

        btnExcluirDespesa = New Button() With {.Text = "🗑️ Remover Registros Selecionados", .Location = New Point(380, 585), .Size = New Size(575, 42), .BackColor = Color.FromArgb(127, 29, 29), .ForeColor = Color.White, .Font = New Font("Segoe UI", 10, FontStyle.Bold), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnExcluirDespesa.FlatAppearance.BorderSize = 0
        AddHandler btnExcluirDespesa.Paint, AddressOf ArredondarBotaoGDI
        AddHandler btnExcluirDespesa.Click, AddressOf btnExcluirDespesa_Click
        Me.Controls.Add(btnExcluirDespesa)

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

    Private Sub MudarFiltroActivo(novoFiltro As String)
        _filtroAtual = novoFiltro
        btnFiltroTodos.BackColor = If(novoFiltro = "TODOS", COR_BTN_DARK, Color.Transparent)
        btnFiltroTodos.ForeColor = If(novoFiltro = "TODOS", COR_TEXTO_PRINCIPAL, COR_TEXTO_MUTED)

        btnFiltroReceitas.BackColor = If(novoFiltro = "RECEITAS", COR_BTN_DARK, Color.Transparent)
        btnFiltroReceitas.ForeColor = If(novoFiltro = "RECEITAS", COR_VERDE, COR_TEXTO_MUTED)

        btnFiltroDespesas.BackColor = If(novoFiltro = "DESPESAS", COR_BTN_DARK, Color.Transparent)
        btnFiltroDespesas.ForeColor = If(novoFiltro = "DESPESAS", COR_VERM, COR_TEXTO_MUTED)

        AtualizarDadosFinanceiros()
    End Sub

    Private Sub cmbTipoLancamento_SelectedIndexChanged(sender As Object, e As EventArgs)
        AtualizarCategoriasDisponiveis()
        If cmbTipoLancamento.SelectedIndex = 1 Then
            btnSalvarLancamento.Text = "Processar Receita" : btnSalvarLancamento.BackColor = COR_VERDE
        Else
            btnSalvarLancamento.Text = "Processar Despesa" : btnSalvarLancamento.BackColor = COR_VERM
        End If
    End Sub

    Private Sub AtualizarCategoriasDisponiveis()
        cmbCategoriaLancamento.Items.Clear()
        If cmbTipoLancamento.SelectedIndex = 1 Then
            cmbCategoriaLancamento.Items.AddRange(New Object() {"Ganhos em Aplicativos", "Faturamento Particular", "Rendimentos Diversos"})
        Else
            cmbCategoriaLancamento.Items.AddRange(New Object() {"Custo de Abastecimento", "Manutenção ", "Alimentação", "Impostos e Taxas", "Despesas Gerais"})
        End If
        cmbCategoriaLancamento.SelectedIndex = 0
    End Sub

    Private Sub AplicarMascaraMoeda_TextChanged(sender As Object, e As EventArgs)
        Dim txt = DirectCast(sender, TextBox) : RemoveHandler txt.TextChanged, AddressOf AplicarMascaraMoeda_TextChanged
        Dim digitos = New String(txt.Text.Where(AddressOf Char.IsDigit).ToArray())
        Dim val As Decimal = 0
        If Not String.IsNullOrEmpty(digitos) Then val = Decimal.Parse(digitos) / 100D
        txt.Text = val.ToString("C2") : txt.SelectionStart = txt.Text.Length
        AddHandler txt.TextChanged, AddressOf AplicarMascaraMoeda_TextChanged
    End Sub

    Private Function ObterValorDecimalLimpo(textoBox As String) As Decimal
        Dim res As Decimal : Decimal.TryParse(textoBox.Replace("R$", "").Replace(".", "").Replace(",", ".").Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, res) : Return res
    End Function

    Private Sub AtualizarDadosFinanceiros()
        If cmbMes.SelectedIndex = -1 OrElse cmbAno.SelectedItem Is Nothing Then Return
        Dim mesAlvo As Integer = cmbMes.SelectedIndex + 1
        Dim anoAlvo As Integer = Convert.ToInt32(cmbAno.SelectedItem)

        Using db As New AppDbContext()
            Dim listaBruta = db.Despesas.Where(Function(d) d.UsuarioId = _usuarioIdLogado AndAlso d.Data.Month = mesAlvo AndAlso d.Data.Year = anoAlvo).OrderByDescending(Function(d) d.Data).ToList()

            Dim totalGastoNoMes = listaBruta.Where(Function(desp) Not If(desp.Descricao, "").StartsWith("[RECEITA]")).Sum(Function(desp) desp.Valor)
            Dim totalEntradasNoMes = listaBruta.Where(Function(rec) If(rec.Descricao, "").StartsWith("[RECEITA]")).Sum(Function(rec) rec.Valor)
            Dim saldoLiquido = totalEntradasNoMes - totalGastoNoMes

            lblKpiReceitasValor.Text = totalEntradasNoMes.ToString("C2")
            lblKpiDespesasValor.Text = totalGastoNoMes.ToString("C2")
            lblKpiSaldoValor.Text = saldoLiquido.ToString("C2")
            lblKpiSaldoValor.ForeColor = If(saldoLiquido >= 0, COR_VERDE, COR_VERM)

            Dim listaFiltrada = listaBruta
            If _filtroAtual = "RECEITAS" Then
                listaFiltrada = listaBruta.Where(Function(d) If(d.Descricao, "").StartsWith("[RECEITA]")).ToList()
            ElseIf _filtroAtual = "DESPESAS" Then
                listaFiltrada = listaBruta.Where(Function(d) Not If(d.Descricao, "").StartsWith("[RECEITA]")).ToList()
            End If

            Dim itensGrid = listaFiltrada.Select(Function(d)
                                                     Dim textoDesc As String = If(d.Descricao, "")
                                                     Dim isEntrada As Boolean = textoDesc.StartsWith("[RECEITA]")
                                                     Return New With {
                                                         .Selecionar = False,
                                                         .ID = d.Id,
                                                         .Data = d.Data.ToShortDateString(),
                                                         .Categoria = If(d.Categoria, ""),
                                                         .Descricao = textoDesc.Replace("[RECEITA] ", ""),
                                                         .Valor = If(isEntrada, "+ " & d.Valor.ToString("C2"), "- " & d.Valor.ToString("C2"))
                                                     }
                                                 End Function).ToList()

            dgvGastosPessoais.DataSource = Nothing
            dgvGastosPessoais.DataSource = itensGrid
        End Using
    End Sub

    Private Sub btnSalvarLancamento_Click(sender As Object, e As EventArgs)
        Dim val As Decimal = ObterValorDecimalLimpo(txtValorLancamento.Text)
        If String.IsNullOrEmpty(txtDescLancamento.Text) OrElse val <= 0 Then
            MessageBox.Show("Preencha as informações do registro contábil de forma válida.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning) : Return
        End If

        Dim isReceita As Boolean = (cmbTipoLancamento.SelectedIndex = 1)
        Dim descricaoFinal As String = If(isReceita, "[RECEITA] " & txtDescLancamento.Text.Trim(), txtDescLancamento.Text.Trim())

        Using db As New AppDbContext()
            db.Despesas.Add(New Despesa() With {
                .UsuarioId = _usuarioIdLogado,
                .Data = dtpDataLancamento.Value,
                .Valor = val,
                .Descricao = descricaoFinal,
                .Categoria = cmbCategoriaLancamento.SelectedItem.ToString()
            })
            db.SaveChanges()
        End Using

        txtValorLancamento.Text = "R$ 0,00" : txtDescLancamento.Clear() : dtpDataLancamento.Value = DateTime.Today
        MessageBox.Show("Operação computada na base de dados.", "Flow Road", MessageBoxButtons.OK, MessageBoxIcon.Information)
        AtualizarDadosFinanceiros()
    End Sub
    Private Sub btnExcluirDespesa_Click(sender As Object, e As EventArgs)
        If dgvGastosPessoais.Rows.Count = 0 Then Return

        Dim idsParaDeletar As New List(Of Integer)()
        For Each row As DataGridViewRow In dgvGastosPessoais.Rows
            If Convert.ToBoolean(row.Cells("Selecionar").Value) = True Then
                idsParaDeletar.Add(Convert.ToInt32(row.Cells("ID").Value))
            End If
        Next

        If idsParaDeletar.Count = 0 Then
            MessageBox.Show("Marque a caixa de seleção das linhas que deseja remover.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim msgConfirmacao As String = $"Confirma a remoção em massa dos {idsParaDeletar.Count} registro(s) selecionado(s)?"
        If MessageBox.Show(msgConfirmacao, "Aviso de Auditoria", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then
            Using db As New AppDbContext()
                For Each id In idsParaDeletar
                    Dim despesaBanco = db.Despesas.FirstOrDefault(Function(d) d.Id = id)
                    If despesaBanco IsNot Nothing Then
                        If despesaBanco.LancamentoId.HasValue Then
                            Dim idJornada = despesaBanco.LancamentoId.Value
                            db.Despesas.RemoveRange(db.Despesas.Where(Function(d) d.LancamentoId = idJornada).ToList())
                            Try
                                Dim turno = db.Set(Of LancamentoDiario)().FirstOrDefault(Function(l) l.Id = idJornada)
                                If turno IsNot Nothing Then db.Set(Of LancamentoDiario)().Remove(turno)
                            Catch : End Try
                        Else
                            db.Despesas.Remove(despesaBanco)
                        End If
                    End If
                Next
                db.SaveChanges()
            End Using
            MessageBox.Show("Registros estornados com sucesso.", "Contabilidade", MessageBoxButtons.OK, MessageBoxIcon.Information)
            AtualizarDadosFinanceiros()
        End If
    End Sub

    Private Sub dgvGastosPessoais_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs)
        Try
            Dim dgv = DirectCast(sender, DataGridView)

            If dgv.Columns.Contains("Selecionar") Then
                dgv.Columns("Selecionar").Width = 70
                dgv.Columns("Selecionar").HeaderText = "Marcar"
                dgv.Columns("Selecionar").ReadOnly = False ' Permite marcar/desmarcar
            End If

            If dgv.Columns.Contains("ID") Then dgv.Columns("ID").Visible = False : dgv.Columns("ID").ReadOnly = True
            If dgv.Columns.Contains("Data") Then dgv.Columns("Data").Width = 85 : dgv.Columns("Data").ReadOnly = True
            If dgv.Columns.Contains("Categoria") Then dgv.Columns("Categoria").Width = 130 : dgv.Columns("Categoria").ReadOnly = True
            If dgv.Columns.Contains("Descricao") Then dgv.Columns("Descricao").ReadOnly = True
            If dgv.Columns.Contains("Valor") Then
                dgv.Columns("Valor").Width = 100
                dgv.Columns("Valor").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                dgv.Columns("Valor").ReadOnly = True
            End If
        Catch : End Try
    End Sub
End Class