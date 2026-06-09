Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports System.Linq
Imports System.Collections.Generic
Imports System

Namespace Services

    Public Class RelatorioService
        Public Shared Sub ExportarPdfMensal(usuarioId As Integer, mes As Integer, ano As Integer)
            Dim htmlPath As String = Path.Combine(Application.StartupPath, "Relatorio_Executivo.html")

            Using db As New AppDbContext()
                Dim usuario = db.Usuarios.FirstOrDefault(Function(u) u.Id = usuarioId)
                If usuario Is Nothing Then Return

                Dim dataInicial As New DateTime(ano, mes, 1)
                Dim dataFinal As DateTime = dataInicial.AddMonths(1).AddDays(-1)

                Dim lancamentos = db.Lancamentos.Where(Function(l) l.UsuarioId = usuarioId AndAlso l.Data >= dataInicial AndAlso l.Data <= dataFinal).ToList()
                Dim despesasReais = db.Despesas.Where(Function(d) d.UsuarioId = usuarioId AndAlso d.Data >= dataInicial AndAlso d.Data <= dataFinal AndAlso Not If(d.Descricao, "").StartsWith("[RECEITA]")).ToList()

                ' Cálculos Matemáticos
                Dim ganhoBruto As Decimal = lancamentos.Sum(Function(l) l.ValorBruto)
                Dim kmRodados As Decimal = lancamentos.Sum(Function(l) l.KmRodados)
                Dim custosTotais As Decimal = despesasReais.Sum(Function(d) d.Valor)
                Dim lucroLiquido As Decimal = ganhoBruto - custosTotais
                Dim rendimentoKm As Decimal = If(kmRodados > 0, ganhoBruto / kmRodados, 0D)
                Dim margem As Double = If(ganhoBruto > 0, (lucroLiquido / ganhoBruto) * 100, 0)

                ' 🎨 HTML E CSS
                Dim sb As New StringBuilder()
                sb.AppendLine("<!DOCTYPE html><html><head><meta charset='UTF-8'>")
                sb.AppendLine("<style>")
                sb.AppendLine("body { font-family: 'Segoe UI', system-ui, sans-serif; color: #f8fafc; margin: 0; padding: 40px; background: #0f1115; line-height: 1.4; }")

                ' Header
                sb.AppendLine(".header { border-bottom: 1px solid #1e293b; padding-bottom: 20px; margin-bottom: 30px; display: block; height: 60px; }")
                sb.AppendLine(".header-left { float: left; width: 60%; }")
                sb.AppendLine(".header-right { float: right; width: 40%; text-align: right; font-size: 9pt; line-height: 1.5; color: #cbd5e1; }")
                sb.AppendLine(".header h1 { margin: 0; font-size: 22pt; font-weight: 800; color: #f8fafc; letter-spacing: -1px; }")
                sb.AppendLine(".header h1 span { color: #38bdf8; }")

                ' Card de Lucro (Destaque Superior)
                sb.AppendLine(".balance-box { background: #1a202c; padding: 25px; border-radius: 12px; margin-bottom: 25px; text-align: center; border: 1px solid #2d3748; clear: both; }")
                sb.AppendLine(".balance-title { font-size: 8.5pt; text-transform: uppercase; color: #94a3b8; letter-spacing: 1.5px; font-weight: 700; }")
                sb.AppendLine(".balance-value { font-size: 34pt; font-weight: 800; margin: 8px 0; color: #10b981; letter-spacing: -1px; }")
                sb.AppendLine(".balance-value.negative { color: #ef4444; }")
                sb.AppendLine(".balance-meta { color: #94a3b8; font-size: 9.5pt; }")

                'GRID
                sb.AppendLine(".stats-table { width: 100%; border-collapse: separate; border-spacing: 15px 0; margin-bottom: 30px; margin-left: -15px; display: table; }")
                sb.AppendLine(".stat-row { display: table-row; }")
                sb.AppendLine(".stat-item { display: table-cell; background: #1a202c; border: 1px solid #2d3748; padding: 18px; border-radius: 10px; text-align: left; vertical-align: top; }")
                sb.AppendLine(".stat-lbl { font-size: 7.5pt; color: #94a3b8; text-transform: uppercase; margin-bottom: 6px; font-weight: 700; letter-spacing: 0.8px; }")
                sb.AppendLine(".stat-val { font-size: 14pt; font-weight: 700; color: #f8fafc; }")

                ' Tabela de Extrato (Garante que limpe os elementos anteriores)
                sb.AppendLine(".table-container { width: 100%; margin-top: 20px; clear: both; display: block; }")
                sb.AppendLine("h3 { font-size: 11pt; font-weight: 700; color: #38bdf8; margin: 0 0 15px 0; text-transform: uppercase; letter-spacing: 1px; border-left: 4px solid #38bdf8; padding-left: 8px; }")
                sb.AppendLine("table { width: 100%; border-collapse: collapse; font-size: 9pt; background: #1a202c; border-radius: 10px; overflow: hidden; border: 1px solid #2d3748; }")
                sb.AppendLine("th { background: #141a24; color: #94a3b8; padding: 14px 18px; text-align: left; font-weight: 700; text-transform: uppercase; font-size: 7.5pt; border-bottom: 1px solid #2d3748; }")
                sb.AppendLine("td { padding: 12px 18px; border-bottom: 1px solid #2d3748; color: #e2e8f0; }")
                sb.AppendLine("tr:nth-child(even) td { background: #161b26; }")
                sb.AppendLine("tr:last-child td { border-bottom: none; }")
                sb.AppendLine(".text-right { text-align: right; }")
                sb.AppendLine(".val-in { color: #10b981; font-weight: 700; }")
                sb.AppendLine(".val-out { color: #ef4444; font-weight: 700; }")
                sb.AppendLine("</style></head><body>")

                'Cabeçalho
                sb.AppendLine("<div class='header'>")
                sb.AppendLine("  <div class='header-left'>")
                sb.AppendLine("    <h1>FLOW <span>ROAD</span></h1>")
                sb.AppendLine("    <p style='margin:2px 0 0 0;color:#94a3b8;font-size:10pt;'>Business Intelligence Report</p>")
                sb.AppendLine("  </div>")
                sb.AppendLine("  <div class='header-right'>")
                sb.AppendLine($"    <strong>Operador:</strong> {usuario.Nome}<br>")
                sb.AppendLine($"    <strong>Competência:</strong> {dataInicial:MMMM / yyyy}<br>")
                sb.AppendLine($"    <strong>Emissão:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}")
                sb.AppendLine("  </div>")
                sb.AppendLine("</div>")

                'Balanço Principal
                Dim classeLucro As String = If(lucroLiquido >= 0, "balance-value", "balance-value negative")
                sb.AppendLine("<div class='balance-box'>")
                sb.AppendLine("  <div class='balance-title'>Resultado Líquido do Período</div>")
                sb.AppendLine($"  <div class='{classeLucro}'>{lucroLiquido:C2}</div>")
                sb.AppendLine($"  <div class='balance-meta'>Margem: <strong>{margem:F1}%</strong> &nbsp;&bull;&nbsp; Distância: <strong>{kmRodados:N1} KM</strong></div>")
                sb.AppendLine("</div>")

                'Grid de Cards Horizontais
                sb.AppendLine("<div class='stats-table'>")
                sb.AppendLine("  <div class='stat-row'>")
                sb.AppendLine("    <div class='stat-item'>")
                sb.AppendLine("      <div class='stat-lbl'>Faturamento Bruto</div>")
                sb.AppendLine($"      <div class='stat-val'>{ganhoBruto:C2}</div>")
                sb.AppendLine("    </div>")
                sb.AppendLine("    <div class='stat-item'>")
                sb.AppendLine("      <div class='stat-lbl'>Custos Operacionais</div>")
                sb.AppendLine($"      <div class='stat-val val-out'>{custosTotais:C2}</div>")
                sb.AppendLine("    </div>")
                sb.AppendLine("    <div class='stat-item'>")
                sb.AppendLine("      <div class='stat-lbl'>Rendimento / KM</div>")
                sb.AppendLine($"      <div class='stat-val' style='color:#38bdf8;'>{rendimentoKm:C2}</div>")
                sb.AppendLine("    </div>")
                sb.AppendLine("  </div>")
                sb.AppendLine("</div>")

                'Tabela de Extrato
                sb.AppendLine("<div class='table-container'>")
                sb.AppendLine("  <h3>Extrato Analítico Consolidado</h3>")
                sb.AppendLine("  <table>")
                sb.AppendLine("    <thead><tr><th>Data</th><th>Descrição das Movimentações</th><th class='text-right'>Impacto</th></tr></thead><tbody>")

                'Lista para unificar e ordenar
                Dim linhasTabela As New List(Of Dictionary(Of String, Object))()

                For Each l In lancamentos
                    Dim plataforma As String = If(Not String.IsNullOrEmpty(l.Origem), l.Origem, l.Canal)
                    If String.IsNullOrEmpty(plataforma) Then plataforma = "Geral"

                    Dim d As New Dictionary(Of String, Object)()
                    d.Add("Data", l.Data)
                    d.Add("Desc", $"Faturamento Corrida ({plataforma})")
                    d.Add("Valor", l.ValorBruto)
                    d.Add("IsIn", True)
                    linhasTabela.Add(d)
                Next

                For Each d In despesasReais
                    Dim dic As New Dictionary(Of String, Object)()
                    dic.Add("Data", d.Data)
                    dic.Add("Desc", $"{d.Categoria} - {d.Descricao}")
                    dic.Add("Valor", d.Valor)
                    dic.Add("IsIn", False)
                    linhasTabela.Add(dic)
                Next

                If linhasTabela.Count = 0 Then
                    sb.AppendLine("<tr><td colspan='3' style='text-align: center; color: #94a3b8; padding: 30px;'>Sem dados para o período selecionado.</td></tr>")
                Else
                    For Each row In linhasTabela.OrderByDescending(Function(r) DirectCast(r("Data"), DateTime))
                        Dim dt = DirectCast(row("Data"), DateTime)
                        Dim dsc = row("Desc").ToString()
                        Dim vl = Convert.ToDecimal(row("Valor"))
                        Dim isIn = Convert.ToBoolean(row("IsIn"))

                        Dim classCss = If(isIn, "val-in", "val-out")
                        Dim sinal = If(isIn, "+", "-")

                        sb.AppendLine($"<tr><td>{dt:dd/MM/yyyy}</td><td>{dsc}</td><td class='text-right {classCss}'>{sinal} {vl:C2}</td></tr>")
                    Next
                End If

                sb.AppendLine("    </tbody></table>")
                sb.AppendLine("</div>")

                sb.AppendLine("<div style='margin-top: 40px; text-align: center; color: #475569; font-size: 7.5pt; clear: both;'>Documento gerado automaticamente pelo Ecossistema Flow Road.</div>")
                sb.AppendLine("</body></html>")

                File.WriteAllText(htmlPath, sb.ToString(), Encoding.UTF8)
            End Using

            ' Exibição do PDF
            Dim frmPreview As New Form() With {.Text = "Flow Road - Relatório Analítico", .Size = New Size(1000, 820), .StartPosition = FormStartPosition.CenterScreen}
            Dim wb As New WebBrowser() With {.Dock = DockStyle.Fill, .ScriptErrorsSuppressed = True}
            frmPreview.Controls.Add(wb)
            wb.Navigate(htmlPath)

            AddHandler wb.DocumentCompleted, Sub()
                                                 wb.ShowPrintDialog()
                                                 Try : If File.Exists(htmlPath) Then File.Delete(htmlPath)
                                                 Catch : End Try
                                             End Sub
            frmPreview.ShowDialog()
        End Sub
    End Class
End Namespace