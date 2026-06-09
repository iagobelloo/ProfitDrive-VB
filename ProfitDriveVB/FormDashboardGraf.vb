Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports System.Linq
Imports System.Collections.Generic
Imports System

Public Class FormDashboardGraf
    Inherits Form

    Private _usuarioIdLogado As Integer

    ' Componentes de Layout
    Private pnlFiltros As Panel
    Private pnlGrafico As Panel
    Private dgvFluxoCaixaUnificado As DataGridView
    Private btnFiltroHoje, btnFiltro7Dias, btnFiltroMes, btnFiltroTudo As Button
    Private cmbMesFiltro, cmbAnoFiltro As ComboBox
    Private _filtroPeriodoAtual As String = "MES"

    ' Variáveis para o Gráfico de Rosca (GDI+)
    Private _totalReceitasGrafico As Decimal = 0
    Private _totalDespesasGrafico As Decimal = 0

    ' Labels dos Valores dos Cards
    Private valSaldo, valGanhos, valGastos, valLucro, valEficiencia, valMeta As Label
    Private lblVereditoInsight As Label

    Private COR_CANVAS As Color = Color.FromArgb(15, 17, 21)       ' Fundo escuro (matte)
    Private COR_CARD As Color = Color.FromArgb(26, 32, 44)         ' Cards sólidos
    Private COR_TEXTO_PRINCIPAL As Color = Color.FromArgb(248, 250, 252)
    Private COR_TEXTO_MUTED As Color = Color.FromArgb(148, 163, 184)
    Private COR_DESTAQUE As Color = Color.FromArgb(56, 189, 248)   ' Azul claro
    Private COR_VERDE As Color = Color.FromArgb(16, 185, 129)      ' Verde Esmeralda (Sucesso)
    Private COR_VERM As Color = Color.FromArgb(239, 68, 68)        ' Vermelho Coral (Alerta)

    Sub New(usuarioId As Integer)
        InitializeComponent()
        Me._usuarioIdLogado = usuarioId

        Me.Text = "Flow Road - Executive Business Intelligence"
        Me.Size = New Size(960, 740)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.BackColor = COR_CANVAS

        MontarLayoutPremium()

        cmbMesFiltro.SelectedIndex = DateTime.Today.Month - 1
        cmbAnoFiltro.SelectedItem = DateTime.Today.Year.ToString()

        AddHandler cmbMesFiltro.SelectedIndexChanged, Sub() MudarFiltro("COMPETENCIA")
        AddHandler cmbAnoFiltro.SelectedIndexChanged, Sub() MudarFiltro("COMPETENCIA")

        CalcularEMostrarDados()
    End Sub

    Private Sub MontarLayoutPremium()
        pnlFiltros = New Panel() With {.Location = New Point(0, 0), .Size = New Size(960, 60), .BackColor = COR_CARD}

        Dim lblFiltrarPor As New Label() With {.Text = "Período:", .Location = New Point(20, 20), .AutoSize = True, .Font = New Font("Segoe UI", 9.5, FontStyle.Regular), .ForeColor = COR_TEXTO_MUTED}

        Dim CriarBotaoFiltro = Function(texto As String, x As Integer, w As Integer, tag As String) As Button
                                   Dim b As New Button() With {.Text = texto, .Location = New Point(x, 15), .Size = New Size(w, 30), .FlatStyle = FlatStyle.Flat, .BackColor = Color.FromArgb(30, 41, 59), .ForeColor = COR_TEXTO_MUTED, .Font = New Font("Segoe UI", 9, FontStyle.Bold), .Cursor = Cursors.Hand, .Tag = tag}
                                   b.FlatAppearance.BorderSize = 0
                                   AddHandler b.Click, Sub() MudarFiltro(tag)
                                   Return b
                               End Function

        btnFiltroHoje = CriarBotaoFiltro("Hoje", 85, 80, "HOJE")
        btnFiltro7Dias = CriarBotaoFiltro("7 Dias", 175, 80, "7 DIAS")
        btnFiltroMes = CriarBotaoFiltro("Este Mês", 265, 90, "MES")
        btnFiltroTudo = CriarBotaoFiltro("Histórico", 365, 90, "TUDO")

        Dim lblOu As New Label() With {.Text = "Competência:", .Location = New Point(475, 20), .AutoSize = True, .Font = New Font("Segoe UI", 9.5, FontStyle.Regular), .ForeColor = COR_TEXTO_MUTED}

        cmbMesFiltro = New ComboBox() With {.Location = New Point(570, 18), .Size = New Size(110, 21), .DropDownStyle = ComboBoxStyle.DropDownList, .Font = New Font("Segoe UI", 9), .BackColor = Color.FromArgb(30, 41, 59), .ForeColor = COR_TEXTO_PRINCIPAL, .FlatStyle = FlatStyle.Flat}
        cmbMesFiltro.Items.AddRange(New Object() {"Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"})
        cmbAnoFiltro = New ComboBox() With {.Location = New Point(690, 18), .Size = New Size(65, 21), .DropDownStyle = ComboBoxStyle.DropDownList, .Font = New Font("Segoe UI", 9), .BackColor = Color.FromArgb(30, 41, 59), .ForeColor = COR_TEXTO_PRINCIPAL, .FlatStyle = FlatStyle.Flat}
        cmbAnoFiltro.Items.AddRange(New Object() {"2024", "2025", "2026", "2027", "2028"})

        Dim btnExportarPDF As New Button() With {.Text = "Gerar PDF Executivo", .Location = New Point(770, 15), .Size = New Size(150, 30), .BackColor = COR_VERDE, .ForeColor = Color.White, .Font = New Font("Segoe UI", 9, FontStyle.Bold), .Cursor = Cursors.Hand, .FlatStyle = FlatStyle.Flat}
        btnExportarPDF.FlatAppearance.BorderSize = 0
        AddHandler btnExportarPDF.Click, AddressOf btnExportarPDF_Click

        pnlFiltros.Controls.AddRange(New Control() {lblFiltrarPor, btnFiltroHoje, btnFiltro7Dias, btnFiltroMes, btnFiltroTudo, lblOu, cmbMesFiltro, cmbAnoFiltro, btnExportarPDF})
        Me.Controls.Add(pnlFiltros)

        'CARDS KPI
        Dim CriarKpiCard = Function(titulo As String, x As Integer, y As Integer) As Label
                               Dim pnlCard As New Panel() With {.Location = New Point(x, y), .Size = New Size(295, 95), .BackColor = COR_CARD}
                               Dim lblTit As New Label() With {.Text = titulo.ToUpper(), .Location = New Point(20, 15), .Size = New Size(260, 15), .Font = New Font("Segoe UI", 8.5, FontStyle.Bold), .ForeColor = COR_TEXTO_MUTED}
                               Dim lblVal As New Label() With {.Text = "R$ 0,00", .Location = New Point(18, 40), .Size = New Size(260, 40), .Font = New Font("Segoe UI", 22, FontStyle.Bold), .ForeColor = COR_TEXTO_PRINCIPAL}
                               pnlCard.Controls.AddRange(New Control() {lblTit, lblVal})
                               Me.Controls.Add(pnlCard)
                               Return lblVal
                           End Function

        valSaldo = CriarKpiCard("Saldo Atual Em Caixa", 20, 80)
        valGanhos = CriarKpiCard("Faturamento Bruto", 325, 80)
        valGastos = CriarKpiCard("Custos Operacionais", 630, 80)

        valLucro = CriarKpiCard("Lucro Líquido Real", 20, 185)
        valEficiencia = CriarKpiCard("Eficiência de R$ / KM", 325, 185)
        valMeta = CriarKpiCard("Atingimento da Meta", 630, 185)

        'GRÁFICO DE ROSCA
        pnlGrafico = New Panel() With {.Location = New Point(20, 295), .Size = New Size(440, 295), .BackColor = COR_CARD}
        AddHandler pnlGrafico.Paint, AddressOf pnlGrafico_Paint
        Me.Controls.Add(pnlGrafico)

        'GRID DE EXTRATO
        Dim pnlGrid As New Panel() With {.Location = New Point(470, 295), .Size = New Size(455, 295), .BackColor = COR_CARD}
        Dim lblTitExtrato As New Label() With {.Text = "Extrato Analítico Consolidado", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Location = New Point(20, 20), .AutoSize = True, .ForeColor = COR_TEXTO_PRINCIPAL}

        dgvFluxoCaixaUnificado = New DataGridView() With {
            .Location = New Point(20, 55), .Size = New Size(415, 220), .BackgroundColor = COR_CARD, .ForeColor = COR_TEXTO_PRINCIPAL,
            .ReadOnly = True, .AllowUserToAddRows = False, .RowHeadersVisible = False, .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, .BorderStyle = BorderStyle.None, .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            .GridColor = Color.FromArgb(30, 41, 59), .EnableHeadersVisualStyles = False
        }
        dgvFluxoCaixaUnificado.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        dgvFluxoCaixaUnificado.ColumnHeadersDefaultCellStyle.BackColor = COR_CARD
        dgvFluxoCaixaUnificado.ColumnHeadersDefaultCellStyle.ForeColor = COR_TEXTO_MUTED
        dgvFluxoCaixaUnificado.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 8.5, FontStyle.Bold)
        dgvFluxoCaixaUnificado.DefaultCellStyle.BackColor = COR_CARD
        dgvFluxoCaixaUnificado.DefaultCellStyle.ForeColor = COR_TEXTO_PRINCIPAL
        dgvFluxoCaixaUnificado.DefaultCellStyle.SelectionBackColor = Color.FromArgb(51, 65, 85)
        dgvFluxoCaixaUnificado.DefaultCellStyle.SelectionForeColor = COR_TEXTO_PRINCIPAL
        dgvFluxoCaixaUnificado.DefaultCellStyle.Font = New Font("Segoe UI", 9)
        AddHandler dgvFluxoCaixaUnificado.DataBindingComplete, AddressOf AlinharColunasGrid_DataBindingComplete

        pnlGrid.Controls.AddRange(New Control() {lblTitExtrato, dgvFluxoCaixaUnificado})
        Me.Controls.Add(pnlGrid)

        lblVereditoInsight = New Label() With {.Location = New Point(20, 610), .Size = New Size(905, 30), .Font = New Font("Segoe UI", 10, FontStyle.Regular), .ForeColor = COR_DESTAQUE, .TextAlign = ContentAlignment.MiddleLeft}
        Me.Controls.Add(lblVereditoInsight)

        Dim btnVoltar As New Button() With {.Text = "Voltar ao Menu Principal", .Location = New Point(20, 650), .Size = New Size(905, 40), .BackColor = Color.FromArgb(30, 41, 59), .ForeColor = COR_TEXTO_PRINCIPAL, .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Cursor = Cursors.Hand, .FlatStyle = FlatStyle.Flat}
        btnVoltar.FlatAppearance.BorderSize = 0
        AddHandler btnVoltar.Click, Sub() Me.Close()
        Me.Controls.Add(btnVoltar)

        MudarEstiloBotaoAtivo()
    End Sub

    Private Sub AlinharColunasGrid_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs)
        If dgvFluxoCaixaUnificado.Columns.Contains("Valor") Then
            dgvFluxoCaixaUnificado.Columns("Valor").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            dgvFluxoCaixaUnificado.Columns("Valor").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
        End If
    End Sub

    Private Sub MudarFiltro(tipoFiltro As String)
        _filtroPeriodoAtual = tipoFiltro
        MudarEstiloBotaoAtivo()
        CalcularEMostrarDados()
        pnlGrafico.Invalidate()
    End Sub

    Private Sub MudarEstiloBotaoAtivo()
        Dim resetar = Sub(btn As Button)
                          btn.BackColor = Color.FromArgb(30, 41, 59) : btn.ForeColor = COR_TEXTO_MUTED
                      End Sub
        resetar(btnFiltroHoje) : resetar(btnFiltro7Dias) : resetar(btnFiltroMes) : resetar(btnFiltroTudo)

        Dim ativoColor = Color.FromArgb(56, 189, 248)
        Select Case _filtroPeriodoAtual
            Case "HOJE" : btnFiltroHoje.BackColor = ativoColor : btnFiltroHoje.ForeColor = Color.Black
            Case "7 DIAS" : btnFiltro7Dias.BackColor = ativoColor : btnFiltro7Dias.ForeColor = Color.Black
            Case "MES" : btnFiltroMes.BackColor = ativoColor : btnFiltroMes.ForeColor = Color.Black
            Case "TUDO" : btnFiltroTudo.BackColor = ativoColor : btnFiltroTudo.ForeColor = Color.Black
        End Select
    End Sub

    Private Sub CalcularEMostrarDados()
        Using db As New AppDbContext()
            Dim usuario = db.Usuarios.FirstOrDefault(Function(u) u.Id = _usuarioIdLogado)
            If usuario Is Nothing Then Return

            Dim hoje As DateTime = DateTime.Today
            Dim lancamentosFiltrados As New List(Of LancamentoDiario)()
            Dim despesasFiltradas As New List(Of Despesa)()

            Dim totalGanhosGerais As Decimal = db.Lancamentos.Where(Function(l) l.UsuarioId = _usuarioIdLogado).Sum(Function(g) g.ValorBruto)
            Dim totalGastosGerais As Decimal = db.Despesas.Where(Function(d) d.UsuarioId = _usuarioIdLogado AndAlso Not If(d.Descricao, "").StartsWith("[RECEITA]")).Sum(Function(d) d.Valor)
            Dim saldoAtualConta As Decimal = totalGanhosGerais - totalGastosGerais

            If _filtroPeriodoAtual = "COMPETENCIA" Then
                Dim mAlvo As Integer = cmbMesFiltro.SelectedIndex + 1
                Dim aAlvo As Integer = Convert.ToInt32(cmbAnoFiltro.SelectedItem)
                lancamentosFiltrados = db.Lancamentos.Where(Function(l) l.UsuarioId = _usuarioIdLogado AndAlso l.Data.Month = mAlvo AndAlso l.Data.Year = aAlvo).ToList()
                despesasFiltradas = db.Despesas.Where(Function(d) d.UsuarioId = _usuarioIdLogado AndAlso d.Data.Month = mAlvo AndAlso d.Data.Year = aAlvo).ToList()
            Else
                Dim dtInicial As DateTime = New DateTime(2000, 1, 1)
                Select Case _filtroPeriodoAtual
                    Case "HOJE" : dtInicial = hoje
                    Case "7 DIAS" : dtInicial = hoje.AddDays(-7)
                    Case "MES" : dtInicial = New DateTime(hoje.Year, hoje.Month, 1)
                End Select
                lancamentosFiltrados = db.Lancamentos.Where(Function(l) l.UsuarioId = _usuarioIdLogado AndAlso l.Data >= dtInicial).ToList()
                despesasFiltradas = db.Despesas.Where(Function(d) d.UsuarioId = _usuarioIdLogado AndAlso d.Data >= dtInicial).ToList()
            End If

            Dim ganhoBrutoPeriodo As Decimal = lancamentosFiltrados.Sum(Function(g) g.ValorBruto)
            Dim kmRodadosPeriodo As Decimal = lancamentosFiltrados.Sum(Function(g) g.KmRodados)
            Dim despesasReaisPeriodo As Decimal = despesasFiltradas.Where(Function(d) Not If(d.Descricao, "").StartsWith("[RECEITA]")).Sum(Function(d) d.Valor)

            Dim lucroLiquidoPeriodo As Decimal = ganhoBrutoPeriodo - despesasReaisPeriodo
            Dim ganhoPorKm As Decimal = If(kmRodadosPeriodo > 0, ganhoBrutoPeriodo / kmRodadosPeriodo, 0D)

            Dim metaAlvoPeriodo As Decimal = usuario.MetaDiaria
            Select Case _filtroPeriodoAtual
                Case "7 DIAS" : metaAlvoPeriodo = usuario.MetaDiaria * 7D
                Case "MES", "TUDO", "COMPETENCIA" : metaAlvoPeriodo = usuario.MetaDiaria * 30D
            End Select
            Dim percentualMeta As Double = If(metaAlvoPeriodo > 0, (ganhoBrutoPeriodo / metaAlvoPeriodo) * 100, 0)

            ' Prepara Variáveis pro Gráfico
            _totalReceitasGrafico = ganhoBrutoPeriodo
            _totalDespesasGrafico = despesasReaisPeriodo

            ' Atualiza UI
            valSaldo.Text = saldoAtualConta.ToString("C2")
            valGanhos.Text = ganhoBrutoPeriodo.ToString("C2")
            valGastos.Text = despesasReaisPeriodo.ToString("C2")
            valLucro.Text = lucroLiquidoPeriodo.ToString("C2")
            valEficiencia.Text = ganhoPorKm.ToString("C2")
            valMeta.Text = $"{percentualMeta:F1}%"

            valSaldo.ForeColor = If(saldoAtualConta >= 0, COR_VERDE, COR_VERM)
            valLucro.ForeColor = If(lucroLiquidoPeriodo >= 0, COR_VERDE, COR_VERM)
            valMeta.ForeColor = If(percentualMeta >= 100, COR_VERDE, COR_TEXTO_PRINCIPAL)

            If lucroLiquidoPeriodo > 0 Then
                lblVereditoInsight.Text = $"💡 Operação Saudável. Custo representa apenas {If(ganhoBrutoPeriodo > 0, (despesasReaisPeriodo / ganhoBrutoPeriodo) * 100, 0):F0}% do seu faturamento bruto."
            ElseIf ganhoBrutoPeriodo > 0 Then
                lblVereditoInsight.Text = $"⚠️ Atenção. Seus custos consumiram 100% da sua receita. Reveja sua rotina."
                lblVereditoInsight.ForeColor = COR_VERM
            Else
                lblVereditoInsight.Text = "Aguardando dados para análise no período."
                lblVereditoInsight.ForeColor = COR_TEXTO_MUTED
            End If

            Dim listaGrid As New List(Of Object)()
            For Each g In lancamentosFiltrados
                listaGrid.Add(New With {.Data = g.Data.ToShortDateString(), .Descricao = "Corrida " & g.Origem, .Valor = "+ " & g.ValorBruto.ToString("C2")})
            Next
            For Each d In despesasFiltradas.Where(Function(x) Not If(x.Descricao, "").StartsWith("[RECEITA]"))
                listaGrid.Add(New With {.Data = d.Data.ToShortDateString(), .Descricao = d.Categoria, .Valor = "- " & d.Valor.ToString("C2")})
            Next
            dgvFluxoCaixaUnificado.DataSource = listaGrid.OrderByDescending(Function(x) DateTime.Parse(CallByName(x, "Data", CallType.Get).ToString())).ToList()
        End Using
    End Sub

    'GRÁFICO DE ROSCA (DOUGHNUT CHART GDI+)
    Private Sub pnlGrafico_Paint(sender As Object, e As PaintEventArgs)
        Dim g = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias

        g.DrawString("Balanço do Período", New Font("Segoe UI", 10, FontStyle.Bold), New SolidBrush(COR_TEXTO_PRINCIPAL), New PointF(20, 20))

        Dim total = _totalReceitasGrafico + _totalDespesasGrafico
        If total = 0 Then
            g.DrawString("Sem movimentações.", New Font("Segoe UI", 9, FontStyle.Italic), New SolidBrush(COR_TEXTO_MUTED), New PointF(20, 50))
            Return
        End If

        ' Calculando Ângulos da Rosca
        Dim angReceita As Single = CSng((_totalReceitasGrafico / total) * 360)
        Dim angDespesa As Single = CSng((_totalDespesasGrafico / total) * 360)

        Dim rectPie As New Rectangle(30, 60, 180, 180)
        Dim rectHole As New Rectangle(70, 100, 100, 100) ' Buraco da rosca

        ' Fatias da Torta
        If _totalReceitasGrafico > 0 Then g.FillPie(New SolidBrush(COR_VERDE), rectPie, -90, angReceita)
        If _totalDespesasGrafico > 0 Then g.FillPie(New SolidBrush(COR_VERM), rectPie, -90 + angReceita, angDespesa)

        ' Furo Central (Efeito Doughnut)
        g.FillEllipse(New SolidBrush(COR_CARD), rectHole)

        ' Legendas Laterais
        Dim xLeg As Integer = 240
        Dim yLeg As Integer = 100

        g.FillEllipse(New SolidBrush(COR_VERDE), xLeg, yLeg, 12, 12)
        g.DrawString("Receitas Brutas", New Font("Segoe UI", 9), New SolidBrush(COR_TEXTO_MUTED), New PointF(xLeg + 20, yLeg - 2))
        g.DrawString(_totalReceitasGrafico.ToString("C2"), New Font("Segoe UI", 10, FontStyle.Bold), New SolidBrush(COR_TEXTO_PRINCIPAL), New PointF(xLeg + 20, yLeg + 15))

        yLeg += 50
        g.FillEllipse(New SolidBrush(COR_VERM), xLeg, yLeg, 12, 12)
        g.DrawString("Custos & Despesas", New Font("Segoe UI", 9), New SolidBrush(COR_TEXTO_MUTED), New PointF(xLeg + 20, yLeg - 2))
        g.DrawString(_totalDespesasGrafico.ToString("C2"), New Font("Segoe UI", 10, FontStyle.Bold), New SolidBrush(COR_TEXTO_PRINCIPAL), New PointF(xLeg + 20, yLeg + 15))
    End Sub

    Private Sub btnExportarPDF_Click(sender As Object, e As EventArgs)
        Using db As New AppDbContext()
            Dim u = db.Usuarios.FirstOrDefault(Function(user) user.Id = _usuarioIdLogado)
            If u IsNot Nothing Then
                Dim plano As String = ""
                Try : plano = CallByName(u, "CategoriaPlano", CallType.Get).ToString().Trim() : Catch : End Try

                If Not String.Equals(plano, "BLACK", StringComparison.OrdinalIgnoreCase) AndAlso Not String.Equals(plano, "OURO", StringComparison.OrdinalIgnoreCase) Then
                    MessageBox.Show($"Recurso Exclusivo! 👑{vbCrLf}{vbCrLf}A emissão de relatórios em PDF requer o Plano Black.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning) : Return
                End If
                Services.RelatorioService.ExportarPdfMensal(Me._usuarioIdLogado, cmbMesFiltro.SelectedIndex + 1, Convert.ToInt32(cmbAnoFiltro.SelectedItem))
            End If
        End Using
    End Sub
End Class