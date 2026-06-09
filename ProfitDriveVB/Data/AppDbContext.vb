Imports Microsoft.EntityFrameworkCore
Imports System.IO

Public Class AppDbContext
    Inherits DbContext

    Public Property Usuarios As DbSet(Of Usuario)
    Public Property Veiculos As DbSet(Of Veiculo)
    Public Property Lancamentos As DbSet(Of LancamentoDiario)
    Public Property Despesas As DbSet(Of Despesa)
    Public Property FaturamentosAssinaturas As DbSet(Of FaturamentoAssinatura)

    Protected Overrides Sub OnConfiguring(optionsBuilder As DbContextOptionsBuilder)
        If Not optionsBuilder.IsConfigured Then
            'Dados de acesso ao seu MySQL local
            Dim servidor As String = "localhost"
            Dim bancoDados As String = "profitdrive_v2"
            Dim usuario As String = "root"
            Dim senha As String = "Amora2312Cacau!" 'MYSQL

            Dim connectionString As String = $"server={servidor};database={bancoDados};user={usuario};password={senha}"

            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        End If
    End Sub
End Class