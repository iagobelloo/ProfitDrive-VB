Imports System.Drawing
Imports System.Windows.Forms

Public Class FormDashboard
    Inherits Form

    'Lançamento de Ganhos
    Private txtKmRodados As TextBox
    Private txtValorBruto As TextBox
    Private txtTaxaApp As TextBox
    Private btnSalvarGanho As Button

    'Lançamento de Despesas
    Private txtValorDespesa As TextBox
    Private txtDescDespesa As TextBox
    Private cmbCategoriaDespesa As ComboBox
    Private btnSalvarDespesa As Button

    'Exibição e Painel Inteligente
    Private dgvHistorico As DataGridView
    Private lblCardSaldo As Label
    Private lblCardMetas As Label
    Private lblInsightGastos As Label

    Sub New()
        Me.Text = "Flow Road - Painel de Gestão"
        Me.Size = New Size(950, 650)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.MaximizeBox = False

        MontarLayoutDashboard()

        AtualizarDadosDashboard()
    End Sub

    Private Sub MontarLayoutDashboard()
        'PAINEL ESQUERDO: LANÇAMENTOS
        Dim pnlLancamentos As New Panel() With {.Location = New Point(15, 15), .Size = New Size(320, 580), .BorderStyle = BorderStyle.FixedSingle, .BackColor = Color.WhiteSmoke}

        Dim lblTitGanhos As New Label() With {.Text = "LANÇAR ENTRADA", .Font = New Font("Arial", 10, FontStyle.Bold), .Location = New Point(10, 15), .Size = New Size(280, 20), .ForeColor = Color.DarkGreen}

        Dim lblKm As New Label() With {.Text = "Km Rodados:", .Location = New Point(10, 45), .Size = New Size(100, 20)}
        txtKmRodados = New TextBox() With {.Location = New Point(120, 42), .Size = New Size(180, 20)}

        Dim lblBruto As New Label() With {.Text = "Valor Bruto (R$):", .Location = New Point(10, 80), .Size = New Size(100, 20)}
        txtValorBruto = New TextBox() With {.Location = New Point(120, 77), .Size = New Size(180, 20)}

        Dim lblTaxa As New Label() With {.Text = "Taxas Retidas (R$):", .Location = New Point(10, 115), .Size = New Size(110, 20)}
        txtTaxaApp = New TextBox() With {.Location = New Point(120, 112), .Size = New Size(180, 20), .Text = "0,00"}

        btnSalvarGanho = New Button() With {.Text = "Registrar Ganhos do Dia", .Location = New Point(10, 145), .Size = New Size(290, 30), .BackColor = Color.LightGreen, .Font = New Font("Arial", 9, FontStyle.Bold)}
        AddHandler btnSalvarGanho.Click, AddressOf btnSalvarGanho_Click

        Dim lblLinha As New Label() With {.BorderStyle = BorderStyle.Fixed3D, .Location = New Point(10, 195), .Size = New Size(290, 2)}

        Dim lblTitDespesas As New Label() With {.Text = "LANÇAR DESPESA", .Font = New Font("Arial", 10, FontStyle.Bold), .Location = New Point(10, 215), .Size = New Size(280, 20), .ForeColor = Color.DarkRed}

        Dim lblValDesp As New Label() With {.Text = "Valor Gasto (R$):", .Location = New Point(10, 245), .Size = New Size(100, 20)}
        txtValorDespesa = New TextBox() With {.Location = New Point(120, 242), .Size = New Size(180, 20)}

        Dim lblDescDesp As New Label() With {.Text = "Descrição:", .Location = New Point(10, 280), .Size = New Size(100, 20)}
        txtDescDespesa = New TextBox() With {.Location = New Point(120, 277), .Size = New Size(180, 20), .PlaceholderText = "Ex: Troca de óleo"}

        Dim lblCatDesp As New Label() With {.Text = "Categoria:", .Location = New Point(10, 315), .Size = New Size(100, 20)}
        cmbCategoriaDespesa = New ComboBox() With {.Location = New Point(120, 312), .Size = New Size(180, 21), .DropDownStyle = ComboBoxStyle.DropDownList}
        cmbCategoriaDespesa.Items.AddRange(New Object() {"Combustível", "Manutenção", "Contas Fixas", "Cartão", "Outros"})
        cmbCategoriaDespesa.SelectedIndex = 0

        btnSalvarDespesa = New Button() With {.Text = "Registrar Despesa", .Location = New Point(10, 350), .Size = New Size(290, 30), .BackColor = Color.LightCoral, .Font = New Font("Arial", 9, FontStyle.Bold)}
        AddHandler btnSalvarDespesa.Click, AddressOf btnSalvarDespesa_Click

        Dim btnMenuConfig As New Button() With {.Text = "Ajustar Consumo Combustível", .Location = New Point(10, 530), .Size = New Size(290, 35), .BackColor = Color.LightGray}
        AddHandler btnMenuConfig.Click, AddressOf btnMenuConfig_Click

        pnlLancamentos.Controls.AddRange(New Control() {lblTitGanhos, lblKm, txtKmRodados, lblBruto, txtValorBruto, lblTaxa, txtTaxaApp, btnSalvarGanho, lblLinha, lblTitDespesas, lblValDesp, txtValorDespesa, lblDescDesp, txtDescDespesa, lblCatDesp, cmbCategoriaDespesa, btnSalvarDespesa, btnMenuConfig})

        'PAINEL SUPERIOR DIREITO: INDICADORES
        lblCardSaldo = New Label() With {.Location = New Point(350, 15), .Size = New Size(280, 85), .BorderStyle = BorderStyle.FixedSingle, .Font = New Font("Arial", 11, FontStyle.Bold), .TextAlign = ContentAlignment.MiddleCenter, .BackColor = Color.White}
        lblCardMetas = New Label() With {.Location = New Point(645, 15), .Size = New Size(275, 85), .BorderStyle = BorderStyle.FixedSingle, .Font = New Font("Arial", 10, FontStyle.Regular), .TextAlign = ContentAlignment.MiddleCenter, .BackColor = Color.White}

        'PAINEL DO INSIGHT DE INTELIGÊNCIA
        lblInsightGastos = New Label() With {
            .Location = New Point(350, 115),
            .Size = New Size(570, 50),
            .BorderStyle = BorderStyle.Fixed3D,
            .Font = New Font("Arial", 9.5, FontStyle.Italic),
            .BackColor = Color.LightYellow,
            .TextAlign = ContentAlignment.MiddleLeft,
            .Padding = New Padding(10, 0, 10, 0)
        }

        'TABELA DE HISTÓRICO GERAL
        Dim lblTitTabela As New Label() With {.Text = "HISTÓRICO FINANCEIRO INTEGRADO", .Font = New Font("Arial", 9, FontStyle.Bold), .Location = New Point(350, 180), .Size = New Size(400, 15)}

        dgvHistorico = New DataGridView() With {
            .Location = New Point(350, 200),
            .Size = New Size(570, 395),
            .BackgroundColor = Color.White,
            .ReadOnly = True,
            .AllowUserToAddRows = False,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .RowHeadersVisible = False
        }

        Me.Controls.AddRange(New Control() {pnlLancamentos, lblCardSaldo, lblCardMetas, lblInsightGastos, lblTitTabela, dgvHistorico})
    End Sub

    Private Sub Tela_Load()
        AtualizarDadosDashboard()
    End Sub

    Private Sub AtualizarDadosDashboard()
        Using db As New AppDbContext()
            Dim usuario = db.Usuarios.FirstOrDefault()
            Dim veiculo = db.Veiculos.FirstOrDefault()

            If usuario Is Nothing OrElse veiculo Is Nothing Then Return

            'SOMA DAS ENTRADAS
            Dim listaGanhos = db.Lancamentos.ToList()
            Dim faturamentoBrutoTotal As Decimal = listaGanhos.Sum(Function(g) g.ValorBruto)
            Dim totalTaxasApps As Decimal = listaGanhos.Sum(Function(g) g.TaxaAplicativo)

            'Calcula a estimativa automática do combustível gasto baseado nas distâncias salvas
            Dim custoCombustivelEstimado As Decimal = listaGanhos.Sum(Function(g) g.CalcularCustoCombustivel(veiculo))

            'SOMA DAS DESPESAS LANÇADAS MANUALMENTE
            Dim listaDespesas = db.Despesas.ToList()
            Dim totalDespesasManuais As Decimal = listaDespesas.Sum(Function(d) d.Valor)

            'BALANÇO GERAL (Faturamento Bruto - Taxas - Gasolina do carro - Despesas Gerais)
            Dim saldoLiquidoReal As Decimal = faturamentoBrutoTotal - totalTaxasApps - custoCombustivelEstimado - totalDespesasManuais

            'Atualiza o Card do Saldo 
            lblCardSaldo.Text = $"BALANÇO LÍQUIDO MÊS" & vbCrLf & $"R$ {saldoLiquidoReal:F2}"
            If saldoLiquidoReal >= 0 Then
                lblCardSaldo.ForeColor = Color.Green
                lblCardSaldo.BackColor = Color.FromArgb(230, 245, 230) ' Verde bem claro
            Else
                lblCardSaldo.ForeColor = Color.Red
                lblCardSaldo.BackColor = Color.FromArgb(255, 230, 230) ' Vermelho bem claro
            End If

            'CARD DE METAS FINANCEIRAS
            Dim diasTrabalhados As Integer = Math.Max(listaGanhos.Count, 1)
            Dim mediaFaturamentoDiario As Decimal = faturamentoBrutoTotal / diasTrabalhados
            lblCardMetas.Text = $"Meta de Faturamento Diária: R$ {usuario.MetaDiaria:F2}" & vbCrLf &
                                $"Sua Média por Entrada: R$ {mediaFaturamentoDiario:F2}" & vbCrLf &
                                $"Custos Fixos Mês: R$ {usuario.CustosFixosMensais:F2}"

            If listaDespesas.Any() Then
                Dim maiorGasto = listaDespesas.GroupBy(Function(d) d.Categoria) _
                                              .Select(Function(g) New With {.Cat = g.Key, .Total = g.Sum(Function(d) d.Valor)}) _
                                              .OrderByDescending(Function(x) x.Total) _
                                              .FirstOrDefault()

                Dim percentual As Decimal = (maiorGasto.Total / (totalDespesasManuais + custoCombustivelEstimado + totalTaxasApps)) * 100
                lblInsightGastos.Text = $"💡 Insight do Flow Road: Sua maior área de despesa acumulada é '{maiorGasto.Cat}' com um gasto total de R$ {maiorGasto.Total:F2} (aprox. {percentual:F0}% das saídas)."
            Else
                If custoCombustivelEstimado > 0 Then
                    lblInsightGastos.Text = $"💡 Insight do Flow Road: Seu gasto principal registrado até o momento é com 'Combustível do Veículo' (R$ {custoCombustivelEstimado:F2})."
                Else
                    lblInsightGastos.Text = "💡 Insight do Flow Road: Continue inserindo seus dados diários para receber dicas do seu balanço financeiro."
                End If
            End If

            'CARREGAR A TABELA DE HISTÓRICO
            Dim gridItens As New List(Of Object)()

            For Each g In listaGanhos
                gridItens.Add(New With {
                    .Data = g.Data.ToShortDateString(),
                    .Tipo = "Entrada (Ganhos)",
                    .Descricao = $"Rodou {g.KmRodados} KM nos Apps",
                    .Valor = $"+ R$ {g.ValorBruto:F2}",
                    .CustoCombustivel = If(g.ValorCombustivel > 0D, $"R$ {g.ValorCombustivel:F2}", "R$ 0,00")
                })
            Next
            For Each d In listaDespesas
                gridItens.Add(New With {
                    .Data = d.Data.ToShortDateString(),
                    .Tipo = $"Saída ({d.Categoria})",
                    .Descricao = d.Descricao,
                    .Valor = $"- R$ {d.Valor:F2}",
                    .CustoCombustivel = "-"
                })
            Next

            Dim itensOrdenados = gridItens.OrderByDescending(Function(x) x.Data).ToList()
            dgvHistorico.DataSource = itensOrdenados

            Try
                If dgvHistorico.Columns.Contains("Data") Then
                    dgvHistorico.Columns("Data").HeaderText = "Data"
                    dgvHistorico.Columns("Data").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                    dgvHistorico.Columns("Data").FillWeight = 70
                End If

                If dgvHistorico.Columns.Contains("Tipo") Then
                    dgvHistorico.Columns("Tipo").HeaderText = "Tipo"
                    dgvHistorico.Columns("Tipo").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                    dgvHistorico.Columns("Tipo").FillWeight = 120
                End If

                If dgvHistorico.Columns.Contains("Descricao") Then
                    dgvHistorico.Columns("Descricao").HeaderText = "Descrição"
                    dgvHistorico.Columns("Descricao").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                    dgvHistorico.Columns("Descricao").FillWeight = 200
                End If

                If dgvHistorico.Columns.Contains("Valor") Then
                    dgvHistorico.Columns("Valor").HeaderText = "Valor"
                    dgvHistorico.Columns("Valor").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                    dgvHistorico.Columns("Valor").FillWeight = 80
                End If

                If dgvHistorico.Columns.Contains("CustoCombustivel") Then
                    dgvHistorico.Columns("CustoCombustivel").HeaderText = "Custo Combustível"
                    dgvHistorico.Columns("CustoCombustivel").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                    dgvHistorico.Columns("CustoCombustivel").FillWeight = 90
                End If

                dgvHistorico.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            Catch
            End Try
        End Using
    End Sub

    'Salvar Lançamento do Dia (Ganhos)
    Private Sub btnSalvarGanho_Click(sender As Object, e As EventArgs)
        Dim km, bruto, taxa As Decimal

        If Not Decimal.TryParse(txtKmRodados.Text, km) OrElse
           Not Decimal.TryParse(txtValorBruto.Text, bruto) OrElse
           Not Decimal.TryParse(txtTaxaApp.Text, taxa) Then
            MessageBox.Show("Por favor, digite valores válidos para o fechamento do dia.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Using db As New AppDbContext()
            Dim veiculo = db.Veiculos.FirstOrDefault()

            Dim novoGanho As New LancamentoDiario() With {
                .Data = DateTime.Now,
                .KmRodados = km,
                .ValorBruto = bruto,
                .TaxaAplicativo = taxa
            }

            'Valida se existe veículo cadastrado
            If veiculo Is Nothing Then
                Dim resp = MessageBox.Show("Nenhum veículo cadastrado. Deseja abrir Configurações para adicioná-lo? Selecionar Sim abre as configurações; Não salva sem custo de combustível.", "Veículo não encontrado", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                If resp = DialogResult.Yes Then
                    Dim telaConfig As New FormConfig()
                    If telaConfig.ShowDialog() = DialogResult.OK Then
                        veiculo = db.Veiculos.FirstOrDefault()
                    End If
                End If
            End If

            'Valida campos do veículo (consumo/Preço)
            If veiculo IsNot Nothing AndAlso (veiculo.ConsumoCombustivel <= 0D OrElse veiculo.PrecoCombustivel <= 0D) Then
                Dim resp2 = MessageBox.Show("Os dados de consumo ou preço do combustível do veículo não estão configurados corretamente. Deseja ajustar nas Configurações agora? (Sim = abrir Configurações; Não = salvar sem custo estimado)", "Dados de Veículo Incompletos", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                If resp2 = DialogResult.Yes Then
                    Dim telaConfig As New FormConfig()
                    If telaConfig.ShowDialog() = DialogResult.OK Then
                        veiculo = db.Veiculos.FirstOrDefault()
                    End If
                End If
            End If

            ' Calcula o custo estimado de combustível
            If veiculo IsNot Nothing AndAlso veiculo.ConsumoCombustivel > 0D AndAlso veiculo.PrecoCombustivel > 0D Then
                novoGanho.ValorCombustivel = novoGanho.CalcularCustoCombustivel(veiculo)
            Else
                novoGanho.ValorCombustivel = 0D
            End If

            db.Lancamentos.Add(novoGanho)
            db.SaveChanges()
        End Using

        MessageBox.Show("Ganhos do dia arquivados com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information)
        txtKmRodados.Clear()
        txtValorBruto.Clear()
        txtTaxaApp.Text = "0,00"
        AtualizarDadosDashboard()
    End Sub

    'Salvar Lançamento de Gastos (Despesas por Categoria)
    Private Sub btnSalvarDespesa_Click(sender As Object, e As EventArgs)
        Dim valor As Decimal

        If String.IsNullOrEmpty(txtDescDespesa.Text) OrElse Not Decimal.TryParse(txtValorDespesa.Text, valor) Then
            MessageBox.Show("Por favor, insira o valor e a descrição correta da despesa.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Using db As New AppDbContext()
            Dim novaDespesa As New Despesa() With {
                .Data = DateTime.Now,
                .Valor = valor,
                .Descricao = txtDescDespesa.Text.Trim(),
                .Categoria = cmbCategoriaDespesa.SelectedItem.ToString()
            }
            db.Despesas.Add(novaDespesa)
            db.SaveChanges()
        End Using

        MessageBox.Show("Gasto categorizado e debitado no fluxo de caixa!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information)
        txtValorDespesa.Clear()
        txtDescDespesa.Clear()
        cmbCategoriaDespesa.SelectedIndex = 0
        AtualizarDadosDashboard()
    End Sub

    Private Sub btnMenuConfig_Click(sender As Object, e As EventArgs)
        Dim telaConfig As New FormConfig()
        If telaConfig.ShowDialog() = DialogResult.OK Then
            AtualizarDadosDashboard()
        End If
    End Sub
End Class